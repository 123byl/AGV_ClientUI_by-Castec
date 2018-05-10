using CtDockSuit;
using CtLib.Module.Utility;
using CtOutLookBar.Public;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VehiclePlanner.Module;
using VehiclePlanner.Module.Implement;
using VehiclePlanner.Module.Interface;

namespace VehiclePlanner.Partial.VehiclePlannerUI {

    /// <summary>
    /// 擴充方法
    /// </summary>
    public static class VehiclePlannerUI_Extension {
        
        /// <summary>
        /// 回傳子視窗使用者權限
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user">使用者資料</param>
        /// <returns>是否有權限</returns>
        public static bool Authority(this UserData user,ToolStripMenuItem item){
            if (item.Tag is AuthorityDockContainer subForm) {
                return subForm.IsVisiable(user.Level);
            } else {
                throw new Exception($"{item.Name}並無對應的子視窗物件");
            }
        }
        
    }

}