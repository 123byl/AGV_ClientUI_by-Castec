using CtLib.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace VehiclePlanner.Component {
    public partial class CtMotionController : Form {

        #region Declaration - Fields

        private bool mMouseEnter;

        /// <summary>
        /// 紀錄鍵盤按下的方向
        /// </summary>
        private List<MotionDirection> mDirs = new List<MotionDirection>();

        private Dictionary<MotionDirection,PictureBox> mDirCtrlMapping = null;

        #endregion Declaration - Fields

        #region Declaration - Events
        
        public event Events.TestingEvents.DelMotion_Down MotionDown;

        public event Events.TestingEvents.DelMotion_Up MotionUp;

        #endregion Declaration - Events

        #region Function - Constructors

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public CtMotionController(){
            InitializeComponent();

            RegisterEvent(this);
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
        }

        #endregion Function - Constructors

        #region Function - Events

        #region Form

        private void CtMotionController_KeyDown(object sender, KeyEventArgs e) {
            if (Enum.IsDefined(typeof(MotionDirection), (int)e.KeyCode)) {
                MotionDirection dir = (MotionDirection)e.KeyCode;
                if (!mDirs.Contains(dir)) {
                    ActivePic(mDirCtrlMapping[dir]);
                    mDirs.Add(dir);
                    MotionDown?.BeginInvoke(dir,null,null);
                    mDirCtrlMapping[dir].BorderStyle = BorderStyle.FixedSingle;
                }
            }
        }

        private void CtCotionController_KeyUp(object sender,KeyEventArgs e) {
            if (Enum.IsDefined(typeof(MotionDirection), (int)e.KeyCode)) {
                MotionDirection dir = (MotionDirection)e.KeyCode;
                ActivePic(mDirCtrlMapping[dir], false);
                mDirs.Remove(dir);
                if (mDirs.Any()) {                    
                    MotionDown?.BeginInvoke(mDirs[0],null,null);
                } else {
                    MotionUp?.BeginInvoke(null,null);
                }
            }
        }

        #endregion Form

        #region PictureBox

        private void OnMouseMove(object sender, MouseEventArgs args) {
            PictureBox pic = sender as PictureBox;

            if ((args.X < pic.Size.Width - 2) &&
                (args.Y < pic.Size.Width - 2) &&
                (!mMouseEnter)) {
                ActivePic(pic, true);
                mMouseEnter = true;
            }
        }

        private void OnMouseEnter(object sender, EventArgs e) {
        }

        private void OnMouseLeave(object sender, EventArgs e) {
            if (mMouseEnter) {
                PictureBox pic = sender as PictureBox;
                ActivePic(pic, false);
                mMouseEnter = false;
            }
        }

        private void Motion_MouseDown(object sender, MouseEventArgs e) {
            PictureBox pic = sender as PictureBox;
            if (pic.Tag is Keys) {
                MotionControl((Keys)pic.Tag);
            }
        }

        private void Motion_MouseUp(object sender, MouseEventArgs e) {
            PictureBox pic = sender as PictureBox;
            if (pic.Tag is Keys) {
                MotionStop();
            }
        }

        #endregion PictureBox

        #endregion Function - Events

        #region Function - Public Methods


        #endregion Function - Public Methods

        #region Function - Private Methods

        /// <summary>
        /// 移動控制
        /// </summary>
        /// <param name="key"></param>
        private void MotionControl(Keys key) {
            switch (key) {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    MotionDown?.BeginInvoke((MotionDirection)key,null,null);
                    break;
            }
        }

        /// <summary>
        /// 停止移動
        /// </summary>
        private void MotionStop() {
            MotionUp?.BeginInvoke(null,null);
        }
        
        /// <summary>
        /// 以遞迴方式委派所有<see cref="PictureBox"/>控制項的Move與Leave事件
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="reg">委派/取消</param>
        private void RegisterEvent(Control ctrl,bool reg = true) {
            if (ctrl is PictureBox) {
                if (reg) {
                    ctrl.MouseMove += OnMouseMove;
                    ctrl.MouseLeave += OnMouseLeave;
                } else {
                    ctrl.MouseMove -= OnMouseMove;
                    ctrl.MouseLeave -= OnMouseLeave;
                }
            } else if (ctrl.HasChildren) {
                foreach(Control child in ctrl.Controls) {
                    RegisterEvent(child,reg);
                }
            }
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

        #endregion Function - Private Methods

    }
}
