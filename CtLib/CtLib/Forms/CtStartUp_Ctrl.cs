using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Ultity;

namespace CtLib.Forms {
    /// <summary>CASTEC Style 啟動視窗</summary>
    internal partial class CtStartUp_Ctrl : Form {

        #region Version

        /// <summary>CtStartUp_Ctrl 版本訊息</summary>
        /// <remarks><code>
        /// 1.0.0  Ahern [2014/09/14]
        ///     + 建立基礎介面與功能
        ///     
        /// 1.0.1  Ahern [2014/09/15]
        ///     + 介面拖曳
        /// </code></remarks>
        public static CtVersion @Version = new CtVersion(1, 0, 1, "2014/09/15", "Ahern Kuo");

        #endregion

        #region Declaration - Properties

        /// <summary>總步數</summary>
        [DefaultValue(-1)]
        public int MAX_STEP { get; set; }

        #endregion

        #region Declaration - Members

        /// <summary>拖曳事件用之滑鼠X座標</summary>
        private int mMouseX = 0;
        /// <summary>拖曳事件用之滑鼠Y座標</summary>
        private int mMouseY = 0;

        #endregion

        #region Function - Constructor

        /// <summary>建立一啟動介面。建立後請手動指定MAX_STEP</summary>
        public CtStartUp_Ctrl() {
            InitializeComponent();
            MAX_STEP = -1;
        }

        /// <summary>建立啟動視窗，並帶入最大步數(總步數)</summary>
        /// <param name="maxStep">最大步數(總步數)</param>
        public CtStartUp_Ctrl(int maxStep) {
            InitializeComponent();

            MAX_STEP = maxStep;
        }

        #endregion

        #region Function - Method

        /// <summary>計算目前的百分比。 (目前步數/總步數)*100 </summary>
        /// <param name="currVal">當前步數</param>
        /// <returns>當前百分比</returns>
        private int CalcPercent(int currVal) {
            return (int) ( ( (double) currVal / (double) MAX_STEP ) * 100 );
        }

        #endregion

        #region Function - Core

        /// <summary>
        /// 更新介面
        /// <para>如果介面尚未顯示，將會自動顯示並更新</para>
        /// </summary>
        /// <param name="info">當前進度提示</param>
        /// <param name="currStep">當前進度(步數)</param>
        /// <returns>Status Code</returns>
        public Stat UpdateProcess(string info, int currStep) {
            Stat stt = Stat.SUCCESS;
            try {
                if (!this.Visible) this.Show();

                CtInvoke.LabelText(lbInfo, info);
                int currPercent = CalcPercent(currStep);
                CtInvoke.ProgressBarValue(progProcess, currPercent);
                CtInvoke.LabelText(lbProc, CtConvert.CStr(currPercent) + "%");
                //Application.DoEvents();
            } catch (Exception ex) {
                stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }

        #endregion

        #region Function - Interface Event : Mouse Drag Process

        /// <summary>拖曳開始</summary>
        private void CtStartUp_Ctrl_MouseDown(object sender, MouseEventArgs e) {
            /*-- 紀錄滑鼠點下去的位置 --*/
            mMouseX = e.X;
            mMouseY = e.Y;

            /*-- 加入移動事件 --*/
            this.MouseMove += CtStartUp_Ctrl_MouseMove;
        }

        /// <summary>滑鼠拖曳中，改變Form的位置</summary>
        void CtStartUp_Ctrl_MouseMove(object sender, MouseEventArgs e) {
            this.Left = this.DesktopLocation.X + ( e.X - mMouseX );
            this.Top = this.DesktopLocation.Y + ( e.Y - mMouseY );
            Application.DoEvents();
        }

        /// <summary>拖曳結束</summary>
        private void CtStartUp_Ctrl_MouseUp(object sender, MouseEventArgs e) {
            /*-- 將事件移除 --*/
            this.MouseMove -= CtStartUp_Ctrl_MouseMove;
        }

        #endregion
    }
}
