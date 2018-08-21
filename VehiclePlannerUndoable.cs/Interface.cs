using GLUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehiclePlanner.Module.Interface;

namespace VehiclePlannerUndoable.cs {
    public interface IMapGL : IBaseMapGL {
        /// <summary>
        /// 地圖控制項
        /// </summary>
        GLUICtrl MapControl { get; }

		/// <summary>
		/// 斷連線按鍵顯示與否
		/// </summary>
		/// <param name="enable"></param>
		void ButtonEnable(bool enable);
    }

    public interface IGoalSetting : IBaseGoalSetting {
        
        /// <summary>
        /// 地圖控制項參考
        /// </summary>
        GLUICtrl RefMapControl { get; set; }

    }
}
