using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGV.Map.Common
{
    /// <summary>
    /// UI 狀態
    /// </summary>
    public interface IUStatus
    {
        /// <summary>
        /// 是否允許滑鼠編輯
        /// </summary>
        bool AllowEdit { get; set; }

        /// <summary>
        /// 是否畫坐標軸
        /// </summary>
        bool ShowAxis { get; set; }

        /// <summary>
        /// 是否顯示
        /// </summary>
        bool ShowFPS { get; set; }

        /// <summary>
        /// 是否畫網格
        /// </summary>
        bool ShowGrid { get; set; }

        /// <summary>
        /// 是否顯示物件名稱
        /// </summary>
        bool ShowNames { get; set; }

        /// <summary>
        /// 獲得狀態控制項
        /// </summary>
        IUStatus Status { get; }

        #region 顏色

        /// <summary>
        /// X 軸顏色
        /// </summary>
        IColor AxisXColor { get; }

        /// <summary>
        /// Y 軸顏色
        /// </summary>
        IColor AxisYColor { get; }

        /// <summary>
        /// 背景色
        /// </summary>
        IColor BackgroundColor { get; }

        /// <summary>
        /// 網格顏色
        /// </summary>
        IColor GridColor { get; }

        /// <summary>
        /// 文字顏色
        /// </summary>
        IColor TextColor { get; }

        #endregion 顏色
    }
}