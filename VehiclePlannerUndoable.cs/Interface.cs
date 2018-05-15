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
    }

    public interface IGoalSetting : IBaseGoalSetting {
        
        /// <summary>
        /// 地圖控制項參考
        /// </summary>
        GLUICtrl RefMapControl { get; set; }

    }
}
