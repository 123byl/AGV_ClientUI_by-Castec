using CtBind;
using CtLib.Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VehiclePlanner.Core;
using VehiclePlanner.Partial.VehiclePlannerUI;

namespace VehiclePlanner.Forms {

    /// <summary>
    /// 移动控制器
    /// </summary>
    public partial class CtMotionController : Form ,IDataDisplay<IBaseITSController>{

        #region Declaration - Fields
        
        /// <summary>
        /// 紀錄鍵盤按下的方向
        /// </summary>
        private List<MotionDirection> mDirs = new List<MotionDirection>();

        private Dictionary<MotionDirection, PictureBox> mDirCtrlMapping = null;

        /// <summary>
        /// 控制器參考物件
        /// </summary>
        //private IBaseITSController rController = null;

        /// <summary>
        /// 主介面物件參考
        /// </summary>
        private BaseVehiclePlanner_Ctrl rUI = null;

        #endregion Declaration - Fields

        #region Declaration - Events

        public event Events.TestingEvents.DelMotion_Down MotionDown;

        public event Events.TestingEvents.DelMotion_Up MotionUp;

        #endregion Declaration - Events

        #region Function - Constructors

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public CtMotionController(IBaseITSController controller) {
            InitializeComponent();

            //rController = controller;

            /*-- 委派所有PictureBox的事件 --*/
            //RegisterEvent(this);
            /*-- 標記控制方向 --*/
            picBack.Tag = Keys.Down;
            picForward.Tag = Keys.Up;
            picLeftTurn.Tag = Keys.Left;
            picRightTurn.Tag = Keys.Right;
            mDirCtrlMapping = new Dictionary<MotionDirection, PictureBox>() {
                { MotionDirection.Forward,picForward},
                { MotionDirection.Backward,picBack},
                { MotionDirection.LeftTrun,picLeftTurn},
                { MotionDirection.RightTurn,picRightTurn}
            };
            
            Bindings(controller);
            Register(this);

            tstVelocity.LostFocus += TstVelocity_LostFocus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TstVelocity_LostFocus(object sender, EventArgs e) {
            this.InvokeIfNecessary(() => {
                tstVelocity.Text = rUI.Velocity.ToString();
                tstVelocity.ForeColor = Color.Black;
            });
        }

        #endregion Function - Constructors

        #region Function - Events

        #region Form

        /// <summary>
        /// 鍵盤放開事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyUp(object sender,KeyEventArgs e) {
            if (!tstVelocity.Focused && DirParse(e.KeyCode, out MotionDirection dir)) {
                Stop(dir);
            }
        }

        /// <summary>
        /// 鍵盤按下事件
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (!tstVelocity.Focused && DirParse(keyData,out MotionDirection dir)) {
                Motion(dir);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion Form

        #region PictureBox
        
        /// <summary>
        /// 方向控制鈕按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Motion_MouseDown(object sender, MouseEventArgs e) {
                PictureBox pic = sender as PictureBox;
                if (pic.Tag is Keys key && DirParse(key,out MotionDirection dir)) {
                    Motion(dir);
                }
        }

        /// <summary>
        /// 方向控制鈕放開事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Motion_MouseUp(object sender, MouseEventArgs e) {
                PictureBox pic = sender as PictureBox;
                if (pic.Tag is Keys key && DirParse(key,out MotionDirection dir)) {
                    Stop(dir);
                }
        }

        #endregion PictureBox

        #region ToolStripText
        
        /// <summary>
        /// 速度設定修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tstVelocity_KeyUp(object sender, KeyEventArgs e) {
            Color foreColor = Color.Black;//編輯完畢字體顏色
            /*-- 是否輸入完畢 --*/
            if (e.KeyCode == Keys.Enter) {
                /*-- 輸入資料驗證 --*/
                if (int.TryParse(tstVelocity.Text,out int velocity)) {
                    rUI.SetVelocity(velocity);
                } else {
                    this.InvokeIfNecessary(() => {
                        tstVelocity.Text = rUI.Velocity.ToString();
                    });
                }
            } else {
                foreColor = Color.Red;//編輯中字體顏色
            }
            
            /*-- 切換速度字體顏色 --*/
            this.InvokeIfNecessary(() => {
                tstVelocity.ForeColor = foreColor;
            });
        }

        /// <summary>
        /// 限定資料輸入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tstVelocity_KeyPress(object sender, KeyPressEventArgs e) {
            /*-- 限定控制 or 數字輸入 --*/
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) {
                e.Handled = true;
            }
        }

        #endregion ToolStripText

        /// <summary>
        /// 切換馬達激磁狀態
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnServoOnOff_Click(object sender, EventArgs e) {
            rUI.MotorServoOn();
            
        }

        #endregion Function - Events

        #region Function - Private Methods
        
        /// <summary>
        /// 停止移動
        /// </summary>
        private void MotionStop() {
            MotionUp?.BeginInvoke(null, null);
        }

        /// <summary>
        /// <see cref="PictureBox"/>顯示樣式切換Active/Idle
        /// </summary>
        /// <param name="pic"></param>
        /// <param name="isActive"></param>
        private void ActivePic(PictureBox pic, bool isActive = true) {
            if (isActive) {
                pic.BackColor = Color.LightCyan;
                pic.BorderStyle = BorderStyle.FixedSingle;
                pic.Location = pic.Location - new Size(1, 1);
            } else {
                pic.BackColor = this.BackColor;
                pic.BorderStyle = BorderStyle.None;
                pic.Location = pic.Location + new Size(1, 1);
            }
        }

        /// <summary>
        /// 遞迴委派按鍵放開事件
        /// </summary>
        /// <param name="ctrl"></param>
        private void Register(Control ctrl) {
            ctrl.KeyUp += OnKeyUp;
            if (ctrl.HasChildren) {
                foreach (Control c in ctrl.Controls) {
                    Register(c);
                }
            }
        }

        /// <summary>
        /// 移動控制
        /// </summary>
        /// <param name="dir">移動方向</param>
        private void Motion(MotionDirection dir) {
            if (!mDirs.Contains(dir)) {
                mDirs.Add(dir);
                ActivePic(mDirCtrlMapping[dir], true);
                MotionDown?.BeginInvoke(dir, null, null);
            }
        }

        /// <summary>
        /// 停止移動
        /// </summary>
        /// <param name="dir">停止方向</param>
        private void Stop(MotionDirection dir) {
            if (mDirs.Contains(dir)) {
                mDirs.Remove(dir);
                ActivePic(mDirCtrlMapping[dir], false);
                if (mDirs.Any()) {
                    MotionDown?.BeginInvoke(mDirs[0], null, null);
                } else {
                    MotionUp?.BeginInvoke(null, null);
                }
            }
        }

        /// <summary>
        /// 鍵盤按鍵轉移動方向
        /// </summary>
        /// <param name="key">按鍵</param>
        /// <param name="dir">按鍵對應方向</param>
        /// <returns>是否有對應方向</returns>
        private bool DirParse(Keys key, out MotionDirection dir) {
            bool success = true;
            switch (key) {
                case Keys.Up:
                case Keys.W:
                    dir = MotionDirection.Forward;
                    break;
                case Keys.Down:
                case Keys.S:
                    dir = MotionDirection.Backward;
                    break;
                case Keys.Left:
                case Keys.A:
                    dir = MotionDirection.LeftTrun;
                    break;
                case Keys.Right:
                case Keys.D:
                    dir = MotionDirection.RightTurn;
                    break;
                default:
                    dir = MotionDirection.Stop;
                    success = false;
                    break;
            }
            return success;
        }

        #endregion Function - Private Methods

        #region Implement - IDataBind

        /// <summary>
        /// iTS控制器资讯绑定
        /// </summary>
        /// <param name="source"></param>
        public void Bindings(IBaseITSController source) {
            if (source == null) return;
            if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);

            /*馬達激磁狀態*/
            string dataMember = nameof(source.IsMotorServoOn);
            btnServoOnOff.DataBindings.ExAdd(nameof(btnServoOnOff.Text), source, dataMember,(sneder,e) => {
                e.Value = (bool)e.Value ? "ON" : "OFF";
            });
            btnServoOnOff.DataBindings.ExAdd(nameof(btnServoOnOff.BackColor), source, dataMember, (sneder, e) => {
                e.Value = (bool)e.Value ? Color.Green : Color.Red;
            });
            /*-- 手動控制移動速度 --*/
            tstVelocity.DataBindings.ExAdd(nameof(tstVelocity.Text), source, nameof(source.Velocity), (sender, e) => {
                e.Value = e.Value.ToString();
            },source.Velocity.ToString());
        }



        #endregion Implement - IDataBind

    }

}