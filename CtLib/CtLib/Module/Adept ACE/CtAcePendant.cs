using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using CtLib.Forms;
using CtLib.Library;
using CtLib.Module.Utility;

namespace CtLib.Module.Adept {
    /// <summary>
    /// 手臂控制器
    /// <para>可控制 World/Tool/Joint 等方式移動，並可透過鍵盤按鍵操作</para>
    /// <para>請由 <see cref="CtAce.Pendant"/> 開啟此介面</para>
    /// </summary>
    /// <example><code language="C#">
    /// ace.Pendant(true, true);
    /// </code></example>
    internal partial class CtAcePendant : Form {

        #region Declaration - Windows API
        /// <summary>取得當前按下的按鍵</summary>
        /// <param name="vKey"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        #endregion

        #region Declaration - Fields
        /*-- Temporary Flags --*/
        /// <summary>暫存當前的 Jog 模式</summary>
        private JogMode mJogMode = JogMode.Free;
        /// <summary>[Flag] 滑鼠左鍵是否已 Release？  (<see langword="true"/>)左鍵已放開  (<see langword="false"/>)左鍵仍壓著</summary>
        private volatile bool mMouseUp = false;
        /// <summary>[Flag] 位移模式或連續移動模式？  (<see langword="true"/>)Increment  (<see langword="false"/>)Continue Moving</summary>
        private bool mIncrement = false;
        /// <summary>[Flag] World/Tool 或是 Jog 模式？  (<see langword="true"/>)World/Tool  (<see langword="false"/>)Joint</summary>
        private bool mIsWorld = true;
        /// <summary>[Flag] 用於鍵盤按下時判斷是否已經執行中?  (<see langword="true"/>)執行中，不再觸發  (<see langword="false"/>)未執行，觸發按鈕事件</summary>
        private bool mIsWorking = false;

        /*-- Sort Data --*/
        /// <summary>[Ref] CtAce</summary>
        private CtAce rAce;
        /// <summary>當前選擇的 Robot 所含有的 Joint 數量。即是 n 軸 Robot</summary>
        private int mJointLength = 4;
        /// <summary>當前選擇的 Robot Number</summary>
        private int mRobotNum = 1;

        /*-- UI Objects --*/
        /// <summary>[Label] 座標標題，如 "X"、"Y"、"Joint 1" 等</summary>
        private List<Label> mCoordinate = new List<Label>();
        /// <summary>[Label] 座標數值顯示</summary>
        private List<Label> mLocation = new List<Label>();
        /// <summary>[PictureBox] World 情況下的所有 PicturBox (上下左右加減...)</summary>
        private List<PictureBox> mPbWorld = new List<PictureBox>();
        /// <summary>[PictureBox] Joint +</summary>
        private List<PictureBox> mPbJointAdd = new List<PictureBox>();
        /// <summary>[PictureBox] Joint -</summary>
        private List<PictureBox> mPbJointSub = new List<PictureBox>();
        /// <summary>[Label] Joint 標題，如 "Joint 1"</summary>
        private List<Label> mLbJoint = new List<Label>();
        #endregion

        #region Function - Constructors
        /// <summary>開啟手動控制器</summary>
        /// <param name="ace">已連線之 CtAce</param>
        public CtAcePendant(CtAce ace) {
            InitializeComponent();

            rAce = ace;
        }
        #endregion

        #region Function - Drawing
        /// <summary>繪出橢圓矩形</summary>
        private void DrawRoundRectangle(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;
            Label label = sender as Label;

            float X = label.Width - 1;
            float Y = label.Height - 1;

            PointF[] points = {
                new PointF(2,     0),
                new PointF(X - 2, 0),
                new PointF(X - 1, 1),
                new PointF(X,     2),
                new PointF(X,     Y - 2),
                new PointF(X - 1, Y - 1),
                new PointF(X - 2, Y),
                new PointF(2,     Y),
                new PointF(1,     Y - 1),
                new PointF(0,     Y - 2),
                new PointF(0,     2),
                new PointF(1,     1)
            };

            GraphicsPath path = new GraphicsPath();
            path.AddLines(points);

            Pen pen = new Pen(Color.FromArgb(150, Color.Gray), 1);  //Alpha maximum is 255.
            pen.DashStyle = DashStyle.Solid;
            g.FillPath(Brushes.LightGray, path);
            g.DrawPath(pen, path);

            //'Cause using FillPath, so Text already covered.
            SizeF strSize = g.MeasureString(label.Text, label.Font);
            g.DrawString(label.Text, label.Font, new SolidBrush(label.ForeColor), CalculateCenter(strSize, label.Size));
        }

        /// <summary>計算文字的中心點位於控制項的位置</summary>
        /// <param name="stringSize">字串大小</param>
        /// <param name="labelSize">Label大小</param>
        /// <returns>相對於 Label 之座標</returns>
        private PointF CalculateCenter(SizeF stringSize, Size labelSize) {
            PointF point = new PointF();
            point.X = (labelSize.Width - stringSize.Width) / 2;
            point.Y = (labelSize.Height - stringSize.Height) / 2;
            return point;
        }
        #endregion

        #region Function - Methods

        /// <summary>將各 Label 或是 PictureBox 整理至變數裡</summary>
        private void CollectLabel() {
            for (byte i = 1; i < 7; i++) {
                mCoordinate.Add(Controls.Find("lbLoc" + i, true)[0] as Label);
                mLocation.Add(Controls.Find("lbLocVal" + i, true)[0] as Label);
                mLbJoint.Add(Controls.Find("lbJ" + i, true)[0] as Label);
                mPbJointAdd.Add(Controls.Find("pbJ" + i + "Add", true)[0] as PictureBox);
                mPbJointSub.Add(Controls.Find("pbJ" + i + "Sub", true)[0] as PictureBox);
            }

            for (byte idx = 0; idx < 8; idx++) {
                mPbWorld.Add(Controls.Find("pbWorld" + idx, true)[0] as PictureBox);
            }
        }

        /// <summary>顯示座標標題</summary>
        /// <param name="isWorld">(<see langword="true"/>)World/Tool  (<see langword="false"/>)Joint</param>
        private void ShowCoordinate(bool isWorld) {
            if (isWorld) {
                CtInvoke.ControlText(lbLoc1, "X");
                CtInvoke.ControlText(lbLoc2, "Y");
                CtInvoke.ControlText(lbLoc3, "Z");
                CtInvoke.ControlText(lbLoc4, "Yaw");
                CtInvoke.ControlText(lbLoc5, "Pitch");
                CtInvoke.ControlText(lbLoc6, "Roll");

                /*-- 將有可能隱藏的 Label 顯示出來，例如 s800 切成 Joint 時會隱藏 J5 J6 --*/
                for (int idx = mJointLength; idx < 6; idx++) {
                    CtInvoke.ControlVisible(mCoordinate[idx], true);
                    CtInvoke.ControlVisible(mLocation[idx], true);
                }
            } else {

                /*-- 將多餘的軸隱藏起來 --*/
                for (int idx = mJointLength; idx < 6; idx++) {
                    CtInvoke.ControlVisible(mCoordinate[idx], false);
                    CtInvoke.ControlVisible(mLocation[idx], false);
                }

                /*-- 更改標題為 Joint + ? --*/
                for (byte idx = 0; idx < mCoordinate.Count; idx++) {
                    CtInvoke.ControlText(mCoordinate[idx], "Joint " + (idx + 1));
                }
            }
        }

        /// <summary>顯示座標</summary>
        /// <param name="location">座標數值</param>
        private void ShowLocation(List<double> location) {
            for (byte idx = 0; idx < location.Count; idx++) {
                CtInvoke.ControlText(mLocation[idx], location[idx].ToString("##0.0##"));
            }
        }

        /// <summary>顯示 PicturBox 按鈕</summary>
        /// <param name="isWorld">(<see langword="true"/>)World/Tool  (<see langword="false"/>)Joint</param>
        private void ShowButtons(bool isWorld) {
            if (isWorld) {
                mLbJoint.ForEach(val => CtInvoke.ControlVisible(val, false));
                mPbJointAdd.ForEach(val => CtInvoke.ControlVisible(val, false));
                mPbJointSub.ForEach(val => CtInvoke.ControlVisible(val, false));
                mPbWorld.ForEach(val => CtInvoke.ControlVisible(val, true));
            } else {
                mPbWorld.ForEach(val => CtInvoke.ControlVisible(val, false));
                for (byte idx = 0; idx < mJointLength; idx++) {
                    CtInvoke.ControlVisible(mLbJoint[idx], true);
                    CtInvoke.ControlVisible(mPbJointAdd[idx], true);
                    CtInvoke.ControlVisible(mPbJointSub[idx], true);
                }
            }
        }

        /// <summary>更新現在座標</summary>
        /// <param name="isWorld">(<see langword="true"/>)World/Tool  (<see langword="false"/>)Joint</param>
        private void UpdateCurrentLocation(bool isWorld) {
            List<double> loc;
            rAce.Variable.GetHere(mRobotNum, out loc, isWorld ? VPlusVariableType.Location : VPlusVariableType.PrecisionPoint);
            ShowLocation(loc);
        }
        #endregion

        #region Function - Threads
        private void tsk_WorldJog(int robotNum, JogMode jogMode, double speed, int axis) {
            List<double> loc;
            mIsWorking = true;
            do {
                try {
                    if (jogMode != JogMode.World && jogMode != JogMode.Tool) jogMode = JogMode.World;
                    rAce.Motion.Jog(robotNum, jogMode, speed, axis, out loc);
                    ShowLocation(loc);
                } catch (Exception ex) {
                    CtMsgBox.Show("Error", ex.Message, MsgBoxBtn.OK, MsgBoxStyle.Error);
                    mMouseUp = true;
                    UpdateCurrentLocation(true);
                }
                Application.DoEvents();
            } while (!mMouseUp);
        }

        private void tsk_JointJog(int robotNum, double speed, int axis) {
            List<double> loc;
            mIsWorking = true;
            do {
                try {
                    rAce.Motion.Jog(robotNum, JogMode.Joint, speed, axis, out loc);
                    ShowLocation(loc);
                } catch (Exception ex) {
                    CtMsgBox.Show("Error", ex.Message, MsgBoxBtn.OK, MsgBoxStyle.Error);
                    mMouseUp = true;
                    UpdateCurrentLocation(false);
                }
                Application.DoEvents();
            } while (!mMouseUp);
        }

        private void tsk_WorldIncrement(int robotNum, Axis axis, double dist) {
            List<double> loc;
            mIsWorking = true;
            try {
                rAce.Motion.Jog(mRobotNum, axis, dist, out loc);
                ShowLocation(loc);
            } catch (Exception ex) {
                CtMsgBox.Show("Error", ex.Message, MsgBoxBtn.OK, MsgBoxStyle.Error);
                mMouseUp = true;
                UpdateCurrentLocation(true);
            }
        }

        private void tsk_UpdateLocation(object obj) {
            mMouseUp = true;
            //rAce.Motion.WaitMoveDone(mRobotNum);
            CtTimer.Delay(500);
            UpdateCurrentLocation(mIsWorld);
            mIsWorking = false;
        }
        #endregion

        #region Function - Interface Events
        private void CtAcePendant_Load(object sender, EventArgs e) {

            CollectLabel();
            UpdateCurrentLocation(true);

            CtInvoke.ComboBoxAdd(cbRobot, rAce.Robots.Cast<object>().ToArray());
            CtInvoke.ComboBoxSelectedIndex(cbRobot, 0);
            CtInvoke.ComboBoxSelectedIndex(cbMode, 0);
            CtInvoke.ComboBoxSelectedIndex(cbCtrlMode, 0);

            CtInvoke.ButtonImage(btnPower, rAce.IsHighPower() ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
        }

        private void pbMouseEnter(object sender, EventArgs e) {
            PictureBox pb = sender as PictureBox;
            switch (pb.Name) {
                case "pbWorld2":
                    CtInvoke.PictureBoxImage(pb, Properties.Resources.Arrow_Left_4);
                    break;
                case "pbWorld3":
                    CtInvoke.PictureBoxImage(pb, Properties.Resources.Arrow_Right_4);
                    break;
                case "pbWorld0":
                    CtInvoke.PictureBoxImage(pb, Properties.Resources.Arrow_Up_4);
                    break;
                case "pbWorld1":
                    CtInvoke.PictureBoxImage(pb, Properties.Resources.Arrow_Down_4);
                    break;
                case "pbWorld7":
                    CtInvoke.PictureBoxImage(pb, Properties.Resources.Arrow_CW_2);
                    break;
                case "pbWorld6":
                    CtInvoke.PictureBoxImage(pb, Properties.Resources.Arrow_CCW_2);
                    break;
                case "pbWorld4":
                case "pbJ1Add":
                case "pbJ2Add":
                case "pbJ3Add":
                case "pbJ4Add":
                case "pbJ5Add":
                case "pbJ6Add":
                    CtInvoke.PictureBoxImage(pb, Properties.Resources.Add_5);
                    break;
                case "pbWorld5":
                case "pbJ1Sub":
                case "pbJ2Sub":
                case "pbJ3Sub":
                case "pbJ4Sub":
                case "pbJ5Sub":
                case "pbJ6Sub":
                    CtInvoke.PictureBoxImage(pb, Properties.Resources.Sub_4);
                    break;
            }
        }

        private void pbMouseLeave(object sender, EventArgs e) {
            PictureBox pb = sender as PictureBox;
            switch (pb.Name) {
                case "pbWorld2":
                    CtInvoke.PictureBoxImage(pb, Properties.Resources.Arrow_Left_3);
                    break;
                case "pbWorld3":
                    CtInvoke.PictureBoxImage(pb, Properties.Resources.Arrow_Right_3);
                    break;
                case "pbWorld0":
                    CtInvoke.PictureBoxImage(pb, Properties.Resources.Arrow_Up_3);
                    break;
                case "pbWorld1":
                    CtInvoke.PictureBoxImage(pb, Properties.Resources.Arrow_Down_3);
                    break;
                case "pbWorld7":
                    CtInvoke.PictureBoxImage(pb, Properties.Resources.Arrow_CW);
                    break;
                case "pbWorld6":
                    CtInvoke.PictureBoxImage(pb, Properties.Resources.Arrow_CCW);
                    break;
                case "pbWorld4":
                case "pbJ1Add":
                case "pbJ2Add":
                case "pbJ3Add":
                case "pbJ4Add":
                case "pbJ5Add":
                case "pbJ6Add":
                    CtInvoke.PictureBoxImage(pb, Properties.Resources.Add_4);
                    break;
                case "pbWorld5":
                case "pbJ1Sub":
                case "pbJ2Sub":
                case "pbJ3Sub":
                case "pbJ4Sub":
                case "pbJ5Sub":
                case "pbJ6Sub":
                    CtInvoke.PictureBoxImage(pb, Properties.Resources.Sub_3);
                    break;
            }
        }

        private void nudSpeed_ValueChanged_1(object sender, EventArgs e) {
            CtInvoke.ControlFocus(lbMode);
        }

        private void ComboBoxKeyDown(object sender, KeyEventArgs e) {
            e.Handled = true;
        }

        private void cbMode_SelectedIndexChanged(object sender, EventArgs e) {
            switch (cbMode.SelectedIndex) {
                case 0:
                    mIsWorld = true;
                    ShowCoordinate(true);
                    ShowButtons(true);
                    UpdateCurrentLocation(true);
                    CtInvoke.ControlEnabled(cbCtrlMode, true);
                    CtInvoke.ComboBoxSelectedIndex(cbCtrlMode, 0);
                    mJogMode = JogMode.World;
                    break;
                case 1:
                    mIsWorld = false;
                    ShowCoordinate(false);
                    ShowButtons(false);
                    UpdateCurrentLocation(false);
                    CtInvoke.ControlEnabled(cbCtrlMode, false);
                    CtInvoke.ComboBoxSelectedIndex(cbCtrlMode, 0);
                    mJogMode = JogMode.Joint;
                    break;
                case 2:
                    mIsWorld = true;
                    ShowCoordinate(true);
                    ShowButtons(true);
                    UpdateCurrentLocation(true);
                    CtInvoke.ControlEnabled(cbCtrlMode, true);
                    CtInvoke.ComboBoxSelectedIndex(cbCtrlMode, 0);
                    mJogMode = JogMode.Tool;
                    break;
            }
        }

        private void pbMouseUp(object sender, MouseEventArgs e) {
            CtThread.AddWorkItem(tsk_UpdateLocation);
        }

        private void pbMouseDown(object sender, MouseEventArgs e) {
            mMouseUp = false;
            List<int> value = (sender as PictureBox).Tag.ToString().Split(CtConst.CHR_COMMA).ToList().ConvertAll(val => int.Parse(val));

            if (mIncrement)
                CtThread.AddWorkItem(obj => tsk_WorldIncrement(mRobotNum, (Axis)value[0], (double)nudSpeed.Value * value[1]));
            else
                CtThread.AddWorkItem(obj => tsk_WorldJog(mRobotNum, mJogMode, (double)nudSpeed.Value * 0.01 * value[1], value[0]));
        }

        private void cbCtrlMode_SelectedIndexChanged(object sender, EventArgs e) {
            switch (cbCtrlMode.SelectedIndex) {
                case 0:
                    mIncrement = false;
                    CtInvoke.ControlText(lbUnit, "%");
                    CtInvoke.NumericUpDownValue(nudSpeed, 20);
                    break;

                case 1:
                    mIncrement = true;
                    CtInvoke.ControlText(lbUnit, "mm | °");
                    CtInvoke.NumericUpDownValue(nudSpeed, 1);
                    break;
            }
        }

        private void pbMouseDown_Joint(object sender, MouseEventArgs e) {
            mMouseUp = false;
            List<int> value = (sender as PictureBox).Tag.ToString().Split(CtConst.CHR_COMMA).ToList().ConvertAll(val => int.Parse(val));
            CtThread.AddWorkItem(obj => tsk_JointJog(mRobotNum, (double)nudSpeed.Value * 0.01 * value[1], value[0]));
        }

        private void cbRobot_SelectedIndexChanged(object sender, EventArgs e) {
            if (cbRobot.SelectedIndex > -1) {
                mRobotNum = cbRobot.SelectedIndex + 1;
                mJointLength = rAce.GetJointCount(mRobotNum);
                UpdateCurrentLocation(mIsWorld);
            } else {
                mRobotNum = 1;
                mJointLength = 4;
            }
            CtInvoke.ControlFocus(lbMode);
        }

        private void CtAcePendant_KeyDown(object sender, KeyEventArgs e) {
            if (!mIsWorking) {
                if (e.KeyCode == Keys.Up) pbMouseDown(pbWorld0, null);
                else if (e.KeyCode == Keys.Down) pbMouseDown(pbWorld1, null);
                else if (e.KeyCode == Keys.Left) pbMouseDown(pbWorld2, null);
                else if (e.KeyCode == Keys.Right) pbMouseDown(pbWorld3, null);
                else if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus) {
                    if ((GetAsyncKeyState((int)Keys.D1) != 0))
                        pbMouseDown_Joint(pbJ1Add, null);
                    else if ((GetAsyncKeyState((int)Keys.D2) != 0))
                        pbMouseDown_Joint(pbJ2Add, null);
                    else if (mJointLength > 2 && (GetAsyncKeyState((int)Keys.D3) != 0))
                        pbMouseDown_Joint(pbJ3Add, null);
                    else if (mJointLength > 3 && (GetAsyncKeyState((int)Keys.D4) != 0))
                        pbMouseDown_Joint(pbJ4Add, null);
                    else if (mJointLength > 4 && (GetAsyncKeyState((int)Keys.D5) != 0))
                        pbMouseDown_Joint(pbJ5Add, null);
                    else if (mJointLength > 5 && (GetAsyncKeyState((int)Keys.D6) != 0))
                        pbMouseDown_Joint(pbJ6Add, null);
                    else
                        pbMouseDown(pbWorld4, null);

                } else if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus) {
                    if ((GetAsyncKeyState((int)Keys.D1) != 0))
                        pbMouseDown_Joint(pbJ1Sub, null);
                    else if ((GetAsyncKeyState((int)Keys.D2) != 0))
                        pbMouseDown_Joint(pbJ2Sub, null);
                    else if (mJointLength > 2 && (GetAsyncKeyState((int)Keys.D3) != 0))
                        pbMouseDown_Joint(pbJ3Sub, null);
                    else if (mJointLength > 3 && (GetAsyncKeyState((int)Keys.D4) != 0))
                        pbMouseDown_Joint(pbJ4Sub, null);
                    else if (mJointLength > 4 && (GetAsyncKeyState((int)Keys.D5) != 0))
                        pbMouseDown_Joint(pbJ5Sub, null);
                    else if (mJointLength > 5 && (GetAsyncKeyState((int)Keys.D6) != 0))
                        pbMouseDown_Joint(pbJ6Sub, null);
                    else
                        pbMouseDown(pbWorld5, null);

                } else if (e.KeyCode == Keys.Multiply || e.KeyCode == Keys.D8) pbMouseDown(pbWorld6, null);
                else if (e.KeyCode == Keys.Divide || e.KeyCode == Keys.OemQuestion) pbMouseDown(pbWorld7, null);

                else Console.WriteLine(e.KeyCode);
            }
        }

        private void CtAcePendant_KeyUp(object sender, KeyEventArgs e) {
            CtThread.AddWorkItem(tsk_UpdateLocation);
        }

        private void btnHelp_Click(object sender, EventArgs e) {
            if (btnHelp.Text == ">>") {
                CtInvoke.ControlWidth(this, lbHelp.Left + lbHelp.Width + 25);
                CtInvoke.ControlText(btnHelp, "<<");
            } else {
                CtInvoke.ControlWidth(this, lbSep.Left + 8);
                CtInvoke.ControlText(btnHelp, ">>");
            }
        }

        private void btnPower_Click(object sender, EventArgs e) {
            rAce.SetPower(!rAce.IsHighPower());
            CtInvoke.ButtonImage(btnPower, rAce.IsHighPower() ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
        }

		private void CtAcePendant_FormClosing(object sender, FormClosingEventArgs e) {
			try {
				for (int i = 1; i <= rAce.Robots.Count; i++) {
					rAce.Motion.Detach(i);
				}
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER3_ACE, ex);
			}
		}

		#endregion
	}
}
