using CtDockSuit;
using CtLib.Module.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;

namespace VehiclePlanner.Module {
    
    /// <summary>
    /// 具使用者權限控制可視性之子視窗
    /// </summary>
    public class AuthorityDockContainer :CtDockContainer {

        protected AuthorityDockContainer() : base() { }

        /// <summary>
        /// 具有預設DockState功能的建置方法
        /// </summary>
        public AuthorityDockContainer(DockState defState):base(defState) {}

        /// <summary>
        /// 依照使用者權限進行可視度切換
        /// </summary>
        /// <param name="lv">使用者權限</param>
        public void AuthorityVisiable(AccessLevel lv) {
            if (IsVisiable(lv)) {
                ShowWindow();
            } else {
                HideWindow();
            }
        }

        /// <summary>
        /// 回傳該權限是否可視
        /// </summary>
        /// <param name="lv">使用者權限</param>
        /// <returns>是否可視</returns>
        protected virtual bool IsVisiable(AccessLevel lv) {
            throw new NotImplementedException();
        }
    }
}
