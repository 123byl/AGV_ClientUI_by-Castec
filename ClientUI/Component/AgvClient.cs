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

namespace ClientUI
{
    #region Declaration Extenstion

    /// <summary>
    /// 擴充方法定義
    /// </summary>
    internal static class AgvExtenstion {

        #region UI

        /// <summary>擴充 <see cref="Control"/>，使用 <seealso cref="Control.Invoke(Delegate)"/> 方法執行不具任何簽章之委派</summary>
        /// <param name="ctrl">欲調用的控制項</param>
        /// <param name="action">欲執行的方法</param>
        public static void InvokeIfNecessary(this Control ctrl, MethodInvoker action) {
            if (ctrl.InvokeRequired) ctrl.BeginInvoke(action);
            else action();
        }

        #endregion UI

        #region Converter
        

        #endregion Convert

        public static void SetLocation(this IAGV agv,ITowardPair location) {
            agv.Data.Position.X = location.Position.X;
            agv.Data.Position.Y = location.Position.Y;
            agv.Data.Toward.Theta = location.Toward.Theta;
        }



        #region OutlookBar

        public static IClickSender AddItem(this IOutlookCategory iconPanel, CursorMode mode, Image img = null) {
            return iconPanel.AddItem(mode.ToString(), img, (int)mode);
        }

        public static IClickSender AddItem(this IOutlookCategory category, CursorMode mode, string imgPath) {
            Image img = File.Exists(imgPath) ? Image.FromFile(imgPath) : null;
            return category.AddItem(mode, img);
        }

        #endregion OutlookBar

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

    #region Declaration - Const

    /// <summary>
    /// 雷射參數
    /// </summary>
    public struct LaserParam {
        public const double AngleBase = -135;
        public const double Resolution = 0.25;
        public const double AngleOffset = 43;
        public const double OffsetLen = 416.75;
        public const double OffsetTheta = 43;
    }

    #endregion Declaration - Const

}
