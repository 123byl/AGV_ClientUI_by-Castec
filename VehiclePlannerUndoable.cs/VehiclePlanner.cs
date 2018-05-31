using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VehiclePlanner;
using VehiclePlanner.Core;
using VehiclePlanner.Module.Implement;
using WeifenLuo.WinFormsUI.Docking;

namespace VehiclePlannerUndoable.cs {

    /// <summary>
    /// 介面實作
    /// </summary>
    public partial class CtVehiclePlanner_Ctrl : BaseVehiclePlanner_Ctrl {

        private IVehiclePlanner rVehiclePlanner = null;

        private IMapGL MapGL { get => mDockContent[miMapGL] as IMapGL; }

        private IGoalSetting GoalSetting { get => mDockContent[miGoalSetting] as IGoalSetting; }

        public CtVehiclePlanner_Ctrl(IVehiclePlanner vehiclePlanner):base() {
            InitializeComponent();
            rVehiclePlanner = vehiclePlanner;

            Initial(vehiclePlanner);

        }

        protected override void Initial(IBaseVehiclePlanner vehiclePlanner) {
            base.Initial(vehiclePlanner);
            GoalSetting.RefMapControl =  MapGL.MapControl;
        }

        protected override BaseMapGL GetMapGL(DockState dockState) {
            return new MapGL(this,dockState);
        }

        protected override BaseGoalSetting GetGoalSetting(DockState dockState) {
            return new GoalSetting(this,dockState);
        }

    }

    /// <summary>
    /// 底層介面定義
    /// </summary>
    public interface IVehiclePlanner : IBaseVehiclePlanner {

    }

    /// <summary>
    /// 底層實作
    /// </summary>
    public class VehiclePlanner : BaseVehiclePlanner, IVehiclePlanner {
        
    }
}
