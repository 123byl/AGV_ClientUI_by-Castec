using CtOutLookBar.Public;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehiclePlanner.Partial.VehiclePlannerUI {
    internal static class VehiclePlannerUI_Extension {
        #region OutlookBar

        /// <summary>
        /// 增加MapGL功能選項
        /// </summary>
        /// <param name="category">要新增於此的容器</param>
        /// <param name="mode">MapGL滑鼠模式</param>
        /// <param name="img">選項圖示</param>
        /// <returns>選項點擊發報者</returns>
        public static IClickSender AddItem(this IOutlookCategory category, CursorMode mode, Image img = null) {
            return category.AddItem(mode.ToString(), img, (int)mode);
        }

        /// <summary>
        /// 增加MapGL功能選項
        /// </summary>
        /// <param name="category">要新增於此的容器</param>
        /// <param name="mode">MapGL滑鼠模式</param>
        /// <param name="imgPath">選項圖示路徑</param>
        /// <returns>選項點擊發報者</returns>
        public static IClickSender AddItem(this IOutlookCategory category, CursorMode mode, string imgPath) {
            Image img = File.Exists(imgPath) ? Image.FromFile(imgPath) : null;
            return category.AddItem(mode, img);
        }

        #endregion OutlookBar
    }
}
