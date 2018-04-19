using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;

namespace CtDockSuit
{
    /// <summary>
    /// DockContnet接口
    /// </summary>
    public interface ICtDockContainer : IDisposable {

        #region Properties

        /// <summary>
        /// 預設停靠狀態
        /// </summary>
        DockState DefaultDockState { get; set; }

        /// <summary>
        /// 表單固定尺寸
        /// </summary>
        Size FixedSize { get; set; }

        /// <summary>
        /// 是否可視
        /// </summary>
        bool Visible { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 分配<see cref="DockPanel"/>物件參考
        /// </summary>
        /// <param name="dockPanel"></param>
        void AssignmentDockPanel(DockPanel dockPanel);

        ///// <summary>
        ///// 隱藏視窗
        ///// </summary>
        //void HideWindow();

        ///// <summary>
        ///// 依照預設停靠狀態顯示
        ///// </summary>
        //void ShowWindow();

        #endregion Methods

        #region 原本就有實作的方法、屬性
        DockState DockState { get; set; }
        event EventHandler DockStateChanged;
        string Text { get; set; }
        #endregion 

    }
}
