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

using MapProcessing;
using CtLib.Library;
namespace ClientUI {
    
    /// <summary>
    /// Goal點設定介面
    /// </summary>
    public partial class CtGoalSetting : CtDockContent {


        #region Funciton - Construcotrs
        

        /// <summary>
        /// 共用建構方法
        /// </summary>
        /// <param name="goalsetting">GoalSetting方法實作物件參考</param>
        /// <param name="main">主介面參考</param>
        /// <param name="defState">預設停靠方式</param>
        public CtGoalSetting(DockState defState = DockState.Float)
            : base(defState) {
            InitializeComponent();
            FixedSize = new Size(776, 860);
        }

        #endregion Funciton - Constructors

    }

    /// <summary>
    /// Goal Setting 事件類型
    /// </summary>
    public enum GoalSettingEventType {
        /// <summary>
        /// 載入地圖
        /// </summary>
        LoadMap,
        /// <summary>
        /// 更新要新增的點位
        /// </summary>
        RefreshAddPos,
        /// <summary>
        /// 連線狀態變更事件
        /// </summary>
        Connect,
        /// <summary>
        /// Map檔路徑變更
        /// </summary>
        CurMapPath

    }

    /// <summary>
    /// Goal Setting 事件參數
    /// </summary>
    public class GoalSettingEventArgs : EventArgs {
        /// <summary>
        /// 事件類型
        /// </summary>
        public GoalSettingEventType Type { get; }

        /// <summary>
        /// 傳遞參數
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// 一般建構方法
        /// </summary>
        /// <param name="type">事件類型</param>
        /// <param name="value">傳遞參數</param>
        public GoalSettingEventArgs(GoalSettingEventType type, object value = null) {
            this.Type = type;
            this.Value = value;
        }
    }

    /// <summary>
    /// Goal Setting 事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void GoalSettingEvent(object sender, GoalSettingEventArgs e);

}
