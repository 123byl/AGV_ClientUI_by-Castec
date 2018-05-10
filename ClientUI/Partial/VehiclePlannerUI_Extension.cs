using CtDockSuit;
using CtLib.Module.Utility;
using CtOutLookBar.Public;
using System;
using System.Drawing;
using System.IO;
using VehiclePlanner.Module.Implement;
using VehiclePlanner.Module.Interface;

namespace VehiclePlanner.Partial.VehiclePlannerUI {

    /// <summary>
    /// 擴充方法
    /// </summary>
    public static class VehiclePlannerUI_Extension {

        /// <summary>
        /// 子視窗權限定義物件
        /// </summary>
        private static DockContainerAuthority mDockAuthority = new DockContainerAuthority();

        /// <summary>
        /// 子視窗權限定義
        /// </summary>
        public static DockContainerAuthority DockContainerAuthority {
            get {
                return mDockAuthority;
            }
            set {
                if (mDockAuthority != value & value != null) {
                    mDockAuthority = value;
                }
            }
        }

        /// <summary>
        /// 回傳子視窗使用者權限
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user">使用者資料</param>
        /// <returns>是否有權限</returns>
        public static bool Authority<T>(this UserData user) where T : ICtDockContainer {
            return Authority(typeof(T), user);
        }

        /// <summary>
        /// 回傳子視窗使用者權限
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
            return  mDockAuthority.Authority(type.Name, user.Level);
        }
    }

}