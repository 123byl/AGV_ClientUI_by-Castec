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
                case nameof(CtToolBox):
                    return lv > AccessLevel.Operator;
                    
                default:
                    throw new Exception($"未定義{typeName}權限");
            }
        }

    }

}
