using CtDockSuit;
using CtLib.Module.Utility;
using CtOutLookBar.Public;
using SerialCommunication;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehiclePlanner.Partial.VehiclePlannerUI;
using static FactoryMode;

namespace VehiclePlannerAGVBase {

    /// <summary>
    /// 擴充方法
    /// </summary>
    public static class VehiclePlannerExtension {
        /// <summary>
        /// 建構序列傳輸物件
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="receiveDataEvent"></param>
        /// <param name="bypass"></param>
        /// <returns></returns>
        public static ISerialClient SerialClient(this IFactory factory, DelReceiveDataEvent receiveDataEvent, bool bypass) {
            if (bypass) {
                return new FakeSerialClient(receiveDataEvent);
            } else {
                return FactoryMode.Factory.SerialClient(receiveDataEvent);
            }
        }
        
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
