using CtDockSuit;
using CtLib.Module.Utility;
using CtOutLookBar.Public;
using System;
using System.Drawing;
using System.IO;
using VehiclePlanner.Module.Implement;
using VehiclePlanner.Module.Interface;

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

        /// <summary>
        /// 回傳使用者權限
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user">使用者資料</param>
        /// <returns>是否有權限</returns>
        public static bool Authority<T>(this UserData user) where T : ICtDockContainer {
            return Authority(typeof(T), user);
        }

        /// <summary>
        /// 回傳使用者權限
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component">要使用的元件</param>
        /// <param name="user">使用者資料</param>
        /// <returns>是否有權限</returns>
        public static bool Authority<T>(this T component, UserData user) where T : ICtDockContainer {
            return Authority(component.GetType(), user);
        }

        /// <summary>
        /// 回傳使用者權限
        /// </summary>
        /// <param name="type">要使用的元件類型</param>
        /// <param name="user">使用者資料</param>
        /// <returns>是否有權限</returns>
        private static bool Authority(Type type, UserData user) {
            string typeName = type.Name;
            var lv = user.Level;
            switch (typeName) {
                case nameof(CtTesting):
                case nameof(ITesting):
                    return lv > AccessLevel.Operator;

                case nameof(CtConsole):
                case nameof(IConsole):
                    return true;

                case nameof(CtGoalSetting):
                case nameof(IGoalSetting):
                    return lv > AccessLevel.None;

                case nameof(CtToolBox):
                    return lv > AccessLevel.Operator;

                case nameof(AGVMapUI):
                case nameof(IBaseMapGL):
                    return lv > AccessLevel.None;

                default:
                    throw new Exception($"未定義{typeName}權限");
            }
        }
    }
}