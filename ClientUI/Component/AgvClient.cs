using System;
using System.Collections.Generic;
using System.Linq;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

using CtLib.Library;

using GLUI;
using GLCore;
using Geometry;
using static FactoryMode;
using CtOutLookBar.Public;
using System.IO;
using SerialCommunication;

namespace VehiclePlanner
{
    #region Declaration Extenstion

    /// <summary>
    /// 擴充方法定義
    /// </summary>
    internal static class AgvExtenstion {

        #region UI
        
        #endregion UI
        
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

        public static ISerialClient SerialClient(this IFactory factory,DelReceiveDataEvent receiveDataEvent, bool bypass) {
            if (bypass) {
                return new FakeSerialClient(receiveDataEvent);
            }else {
                return FactoryMode.Factory.SerialClient(receiveDataEvent);
            }
        }

    }

    #endregion Declaration  - Extenstion

    #region Declaration - Enum

    /// <summary>
    /// 檔案類型
    /// </summary>
    public enum FileType {
        Ori,
        Map,
    }

    #endregion Declaration - Enum
    
}
