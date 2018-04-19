using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;

namespace CtDockSuit {

    /// <summary>
    /// Dock常用方法
    /// </summary>
    public static class Extension {

        /// <summary>
        /// 將<see cref="DockState"/>轉換為<see cref="DockAreas"/>
        /// </summary>
        /// <param name="dockState"></param>
        /// <returns>對應的<see cref="DockAreas"/></returns>
        public static DockAreas ToAreas(this DockState dockState) {
            DockAreas area = DockAreas.Document;
            switch (dockState) {
                case DockState.DockBottom:
                case DockState.DockBottomAutoHide:
                    area = DockAreas.DockBottom;
                    break;
                case DockState.DockLeft:
                case DockState.DockLeftAutoHide:
                    area = DockAreas.DockLeft;
                    break;
                case DockState.DockTop:
                case DockState.DockTopAutoHide:
                    area = DockAreas.DockTop;
                    break;
                case DockState.DockRight:
                case DockState.DockRightAutoHide:
                    area = DockAreas.DockRight;
                    break;
                case DockState.Document:
                case DockState.Hidden:
                    area = DockAreas.Document;
                    break;
                case DockState.Float:
                    area = DockAreas.Float;
                    break;
                default:
                    throw new ArgumentException($"未定義{dockState}預設停靠行為");
            }

            return area;
        }

        /// <summary>
        /// 將實際Size轉換為Portion值
        /// </summary>
        /// <param name="area">停靠區域</param>
        /// <param name="dockSize">視窗大小</param>
        /// <param name="portion">轉換後的Portion值</param>
        /// <returns>是否轉換成功</returns>
        /// <remarks>
        /// Form與DockContent的的Size似乎並不相同
        /// 下面的參數是試出來的近似值，不同解析度可能有不同的參數
        /// </remarks>
        public static bool CalculatePortion(this DockAreas area, Size dockSize, out double portion) {
            bool ret = true;
            portion = 0;
            switch (area) {
                case DockAreas.DockBottom:
                case DockAreas.DockTop:
                    if (ret = dockSize.Height > 7.43) {
                        portion = (float)((dockSize.Height - 7.43) / 1.32);
                    }
                    break;
                case DockAreas.DockLeft:
                case DockAreas.DockRight:
                    if (ret = dockSize.Width > 6) {
                        portion = (float)((dockSize.Width - 6) / 1.36);
                    }
                    break;
                default:
                    ret = false;
                    break;
            }
            return ret;
        }

    }
}
