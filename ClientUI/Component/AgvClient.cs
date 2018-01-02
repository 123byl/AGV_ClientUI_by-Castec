using System;
using System.Collections.Generic;
using System.Linq;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

using CtLib.Library;

using MapProcessing;
using static MapProcessing.MapSimplication;
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
            if (ctrl.InvokeRequired) ctrl.Invoke(action);
            else action();
        }

        #endregion UI

        #region Converter

        public static IEnumerable<IPair> ToIPair(this IEnumerable<CartesianPos> data) {
            return data.Select(v => FactoryMode.Factory.Pair(v.x, v.y));
        }

        public static CartesianPos ToCartesianPos(this IPair pair) {
            return new CartesianPos(pair.X, pair.Y);
        }

        public static MapLine ToMapLine(this ILine line) {
            return new MapLine(line.Begin.X, line.Begin.Y, line.End.X, line.End.Y);
        }

        public static CartesianPosInfo CartesianPosInfo(this IFactory factory, uint uid, ISingle<ITowardPair> single) {
            return new CartesianPosInfo(
                single.Data.Position.X,
                single.Data.Position.Y,
                single.Data.Toward.Theta,
                single.Name,
                uid);
        }

        public static List<IPair> ToPairs(this List<CartesianPos> points) {
            List<IPair> pairs = new List<IPair>();
            foreach (var item in points) {
                pairs.Add(FactoryMode.Factory.Pair(item.x, item.y));
            }
            return pairs;
        }

        public static CartesianPosInfo ToCarTesianPosinfo(this TowerPairEventArgs e) {
            return new CartesianPosInfo(
                e.DargTarget.Data.Position.X,
                e.DargTarget.Data.Position.Y,
                e.DargTarget.Data.Toward.Theta,
                e.DargTarget.Name,
                e.ID
            );
        }

        #endregion Convert

        /// <summary>
        /// 回傳字串表示點位資訊，格式x,y,theta
        /// </summary>
        /// <param name="point">要轉換的點位</param>
        /// <returns></returns>
        public static string ToStr(this CartesianPos point) {
            return $"{point.x:F0},{point.y:F0},{point.theta:F0}";
        }

        public static void SetPosition(this CartesianPos pos, IAGV agv) {
            pos.x = agv.Data.Position.X;
            pos.y = agv.Data.Position.Y;
            pos.theta = agv.Data.Toward.Theta * Math.PI / 180;
        }

        public static void SetLocation(this IAGV agv, CartesianPos location) {
            agv.Data.Position.X = (int)location.x;
            agv.Data.Position.Y = (int)location.y;
            agv.Data.Toward.Theta = location.theta;
        }

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
