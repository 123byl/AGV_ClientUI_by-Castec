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

using CtLib.Library;
using MapProcessing;

namespace ClientUI {


    /// <summary>
    /// Console介面
    /// </summary>
    public partial class CtConsole : CtDockContent {
        
        #region Funciton - Constructors
        
        /// <summary>
        /// 共用建構方法
        /// </summary>
        /// <param name="console">Console方法實作物件參考</param>
        /// <param name="main">主介面參考</param>
        /// <param name="defState">預設停靠狀態，不可為Unknow</param>
        public CtConsole(DockState defState = DockState.Float)
            :base(null,defState) {
            InitializeComponent();
            FixedSize = new Size(424, 300);
        }

        #endregion Function - Constructors

        //#region Function - Events

        ///// <summary>
        ///// 訊息傳輸事件
        ///// </summary>
        ///// <param name="msg"></param>
        //private void rMth_OnMessageTransmission(string msg) {
        //    CtInvoke.TextBoxText(txtMsg, txtMsg.Text + msg);

        //    //txtMsg.SelectionStart = txtMsg.Text.Length;
        //    //txtMsg.ScrollToCaret();
        //}

        //#endregion Funciton - Events

        //#region Funciton - Public Methods

        ///// <summary>
        ///// 清除訊息視窗
        ///// </summary>
        //public void ClearMsg() {
        //    CtInvoke.TextBoxText(txtMsg,"");
        //}

        //#endregion Funciton - Public Methods

        //#region Function - Private Methods

        ///// <summary>
        ///// 事件訂閱
        ///// </summary>
        //protected override void AddEvent() {
        //    if (rActFunc != null) {
        //        rActFunc.MessageTransmission += rMth_OnMessageTransmission;
        //    }
        //}

        //protected override void RemoveEvent() {
        //    rActFunc.MessageTransmission -= rMth_OnMessageTransmission;
        //}

        //#endregion Function - Private Methods

    }
}
