using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.IAI;

namespace CtLib.Forms.TestPlatform {

    /// <summary>IAI PCON 測試介面</summary>
    public partial class IAI_PCON : Form {

        private CtIAI_PCON mIAI = new CtIAI_PCON();

        /// <summary>建構 IAI PCON 測試介面</summary>
        public IAI_PCON() {
            InitializeComponent();

            mIAI.OnIAIEvents += mIAI_OnIAIEvents;
        }

        private void ShowMessage(string msg) {
            string temp = DateTime.Now.ToString("[HH:mm:ss.ff] ") + msg;
            CtInvoke.ListBoxInsert(lsbxMsg, 0, temp);
        }

        private void mIAI_OnIAIEvents(object sender, PconEventArgs e) {
            switch (e.Event) {
                case PconEvents.Connection:
                    if (CtConvert.CBool(e.Value)) {
                        CtInvoke.PictureBoxImage(picConStt, Properties.Resources.Green_Ball);
                        CtInvoke.ControlText(btnConnect, "Disconnect");
                        ShowMessage("Connected");
                    } else {
                        CtInvoke.PictureBoxImage(picConStt, Properties.Resources.Grey_Ball);
                        CtInvoke.ControlText(btnConnect, "Connect");
                        ShowMessage("Disconnected");
                    }
                    break;
                case PconEvents.DeviceError:
                    ShowMessage("[Device Error] " + string.Join(" ", e.Value as List<byte>));
                    break;
                case PconEvents.CommunicationError:
                    ShowMessage("[Communication Error] " + e.Value as string);
                    break;
                default:
                    break;
            }
        }

        private void CheckStatus() {
            if (mIAI.IsConnected) {
                CtIAI_PCON.AxisStatus axStt = mIAI.GetAxisStatus();
                if (axStt != null) {
                    CtInvoke.PictureBoxImage(picServo, axStt.Servo ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
                    CtInvoke.PictureBoxImage(picBreak, axStt.Break ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
                    CtInvoke.PictureBoxImage(picLightErr, axStt.LightAlarm ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
                    CtInvoke.PictureBoxImage(picFatalErr, axStt.FatalAlarm ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
                    CtInvoke.PictureBoxImage(picEncErr, axStt.EncoderAlarm ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
                    CtInvoke.ControlTag(btnSetServo, axStt.Servo);
                    CtInvoke.ControlTag(btnSetBreak, axStt.Break);
                }

                float? pos = mIAI.GetPosition();
                if (pos.HasValue) CtInvoke.ControlText(txtCurPos, pos.Value.ToString("F3"));

                float? spd = mIAI.GetSpeed();
                if (spd.HasValue) CtInvoke.ControlText(txtCurSpd, spd.Value.ToString("F3"));
            }

        }

        private void btnConnect_Click(object sender, EventArgs e) {
            if (mIAI.IsConnected) mIAI.Disconnect();
            else {
                mIAI.DeviceID = (byte)nudDevID.Value;
                if (mIAI.Connect() == Stat.SUCCESS) {
                    CtIAI_PCON.AxisStatus axStt = mIAI.GetAxisStatus();
                    if (axStt != null) {
                        CtInvoke.PictureBoxImage(picServo, axStt.Servo ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
                        CtInvoke.PictureBoxImage(picBreak, axStt.Break ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
                        CtInvoke.ControlTag(btnSetServo, axStt.Servo);
                        CtInvoke.ControlTag(btnSetBreak, axStt.Break);
                    }
                }
            }
        }

        private void btnGetState_Click(object sender, EventArgs e) {
            CheckStatus();
        }

        private void btnSetServo_Click(object sender, EventArgs e) {
            if (mIAI.IsConnected) {
                bool servo = !CtConvert.CBool(btnSetServo.Tag);
                if (mIAI.Servo(servo) == Stat.SUCCESS)
                    CtInvoke.ControlTag(btnSetServo, servo);
            }
        }

        private void btnSetBreak_Click(object sender, EventArgs e) {
            if (mIAI.IsConnected) {
                bool breaker = !CtConvert.CBool(btnSetBreak.Tag);
                mIAI.Breaker(breaker);
                CtInvoke.ControlTag(btnSetBreak, breaker);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            if (mIAI.IsConnected)
                mIAI.Home();
        }

        private void btnJogR_MouseDown(object sender, MouseEventArgs e) {
            if (mIAI.IsConnected) mIAI.Jog(CtIAI_PCON.JogDirection.Reverse, true);
        }

        private void btnJogP_MouseDown(object sender, MouseEventArgs e) {
            if (mIAI.IsConnected) mIAI.Jog(CtIAI_PCON.JogDirection.Forward, true);
        }

        private void btnJogR_MouseUp(object sender, MouseEventArgs e) {
            if (mIAI.IsConnected) mIAI.Jog(CtIAI_PCON.JogDirection.Reverse, false);
        }

        private void btnJogP_MouseUp(object sender, MouseEventArgs e) {
            if (mIAI.IsConnected) mIAI.Jog(CtIAI_PCON.JogDirection.Forward, false);
        }

        private void chkJog_CheckedChanged(object sender, EventArgs e) {
            if (mIAI.IsConnected) mIAI.SetJogMode(chkJog.Checked);
        }

        private void btnAbsMove_Click(object sender, EventArgs e) {
            if (mIAI.IsConnected) {
                float pos = float.Parse(txtTarPos.Text);
                int spd = int.Parse(txtTarSpd.Text);
                mIAI.MoveTo(pos, 0.1F, spd, 0.5F, 0, CtIAI_PCON.MotionMode.Normal, CtIAI_PCON.PushMode.AfterReached, CtIAI_PCON.Coordinate.Absolute);
            }
        }

        private void btnPathMove_Click(object sender, EventArgs e) {
            if (mIAI.IsConnected) {
                byte pathNo = byte.Parse(txtPath.Text);
                mIAI.MoveTo(pathNo);
            }
        }

        private void btnTeach_Click(object sender, EventArgs e) {
            if (mIAI.IsConnected) {
                byte pathNo = byte.Parse(txtPath.Text);
                mIAI.Teach(pathNo);
            }
        }

        private void btnGetPath_Click(object sender, EventArgs e) {
            CtInvoke.DataGridViewClear(dgvPos);
            if (mIAI.IsConnected) {
                byte stIdx = (byte)nudPathStart.Value;
                byte enIdx = (byte)(nudPathEnd.Value - nudPathStart.Value + 1);
                List<CtIAI_PCON.PathData> data = mIAI.GetPath(stIdx, enIdx);
                List<string> strDGV = new List<string>();
                foreach (CtIAI_PCON.PathData item in data) {
                    strDGV.Clear();
                    strDGV.Add(item.No.ToString());
                    strDGV.Add(item.Position.ToString("F3"));
                    strDGV.Add(item.PositioningBand.ToString("F3"));
                    strDGV.Add(item.Speed.ToString("F3"));
                    strDGV.Add(item.IdvZoneBound_P.ToString("F3"));
                    strDGV.Add(item.IdvZoneBound_M.ToString("F3"));
                    strDGV.Add(item.Accel.ToString("F3"));
                    strDGV.Add(item.Decel.ToString("F3"));
                    strDGV.Add(item.PushCurrent.ToString("F3"));
                    strDGV.Add(item.LoadCurrent.ToString("F3"));
                    strDGV.Add(item.CtrlFlag.ToString());
                    CtInvoke.DataGridViewAddRow(dgvPos, strDGV, false, false);
                }
            }
        }

        private void btnCleanAlarm_Click(object sender, EventArgs e) {
            if (mIAI.IsConnected) {
                mIAI.CleanAlarm();
            }
        }

        private void btnSetPath_Click(object sender, EventArgs e) {
            if (mIAI.IsConnected) {
                byte index = 0;
                float pos = 0, posBand = 0, speed = 0, izbP = 0, izbN = 0, accel = 0, decel = 0, psc = 0, load = 0;
                int flag = 0;
                foreach (DataGridViewRow item in dgvPos.Rows) {
                    index = CtConvert.CByte(item.Cells[0].Value);
                    pos = CtConvert.CFloat(item.Cells[1].Value);
                    posBand = CtConvert.CFloat(item.Cells[2].Value);
                    speed = CtConvert.CFloat(item.Cells[3].Value);
                    izbP = CtConvert.CFloat(item.Cells[4].Value);
                    izbN = CtConvert.CFloat(item.Cells[5].Value);
                    accel = CtConvert.CFloat(item.Cells[6].Value);
                    decel = CtConvert.CFloat(item.Cells[7].Value);
                    psc = CtConvert.CFloat(item.Cells[8].Value);
                    load = CtConvert.CFloat(item.Cells[9].Value);
                    flag = CtConvert.CInt(item.Cells[10].Value);
                    CtIAI_PCON.MotionMode moMode = (flag & 0x02) == 0x02 ? CtIAI_PCON.MotionMode.Push : CtIAI_PCON.MotionMode.Normal;
                    CtIAI_PCON.PushMode psMode = (flag & 0x04) == 0x04 ? CtIAI_PCON.PushMode.BeforeReached : CtIAI_PCON.PushMode.AfterReached;
                    CtIAI_PCON.Coordinate coor = (flag & 0x08) == 0x08 ? CtIAI_PCON.Coordinate.Increment : CtIAI_PCON.Coordinate.Absolute;
                    mIAI.SetPath(index, pos, posBand, speed, izbP, izbN, accel, decel, psc, load, moMode, psMode, coor);
                }
            }
        }
    }
}
