using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehiclePlanner.Partial.VehiclePlannerUI {

    #region Declaration - Enum

    /// <summary>
    /// 鼠標模式
    /// </summary>
    public enum CursorMode {
        /// <summary>
        /// 選擇模式
        /// </summary>
        Select,
        /// <summary>
        /// 新增Goal點模式
        /// </summary>
        Goal,
        /// <summary>
        /// 新增充電站模式
        /// </summary>
        Power,
        /// <summary>
        /// 拖曳模式
        /// </summary>
        Drag,
        /// <summary>
        /// 畫筆模式
        /// </summary>
        Pen,
        /// <summary>
        /// 橡皮擦模式
        /// </summary>
        Eraser,
        /// <summary>
        /// 地圖插入模式
        /// </summary>
        Insert,
        /// <summary>
        /// 禁止區模式
        /// </summary>
        ForbiddenArea
    }

    #endregion Declaration - Enum
}
