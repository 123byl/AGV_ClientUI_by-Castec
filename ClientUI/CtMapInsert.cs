using GLCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UIControl;
using WeifenLuo.WinFormsUI.Docking;

namespace ClientUI {
    public partial class CtMapInsert : CtDockContent , IMouseInsertPanel {

        #region Declaration - Declaration - Fields

        /// <summary>
        /// 控制面板實例
        /// </summary>
        IMouseInsertPanel mPanel = null;

        Size mFloatFormSize = new Size();

        #endregion Declaration  -Fields

        #region Function - Constructors

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public CtMapInsert(DockState defState = DockState.Float)
            : base(defState)
        {
            InitializeComponent();
            /*-- 取得控制面板實例 --*/
            mPanel = StaticControl.MapInsert;

            /*-- 對控制面板進行配置 --*/
            Form form = mPanel as Form;
            form.FormBorderStyle = FormBorderStyle.None;//去除邊框
            form.TopMost = false;//取消最上層顯示
            form.TopLevel = false;//取消最上層級別
            this.Controls.Add(form);//嵌入主面板
            this.Text = form.Text;
            
            /*-- 依控制面板計算主面板尺寸 --*/
            this.FixedSize = new Size(form.Width + SystemInformation.VerticalScrollBarWidth, form.Height + SystemInformation.CaptionHeight + SystemInformation.MenuHeight);
        }

        #endregion Function - Constructors

        #region Implement - IMouseInsertPanel

        void IMouseInsertPanel.Hide() {
            this.Hide();
        }

        void IMouseInsertPanel.SetMouse(IMouseInsert mouse) {
            mPanel.SetMouse(mouse);
        }

        void IMouseInsertPanel.Show() {
            /*-- 顯示控制面板 --*/
            mPanel.Show();

            /*-- 顯示主面板 --*/
            this.ShowWindow();
        }

        #endregion Implement - IMouseInsertPanel

        #region Funciton - Events

        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);

            /*-- 透過關閉控制面板觸發取消事件 --*/
            (mPanel as Form).Close();
        }

        #endregion Function - Evnets

    }
}
