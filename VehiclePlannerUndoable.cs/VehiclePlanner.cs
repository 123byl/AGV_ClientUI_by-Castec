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
    public partial class CtVehiclePlanner_Ctrl : BaseVehiclePlanner_Ctrl {

        private IVehiclePlanner rVehiclePlanner = null;

        public CtVehiclePlanner_Ctrl(IVehiclePlanner vehiclePlanner):base() {
            InitializeComponent();
            rVehiclePlanner = vehiclePlanner;

            Initial(vehiclePlanner);

        }

        protected override BaseMapGL GetMapGL(DockState dockState) {
            return new MapGL(dockState);
        }

        protected override BaseGoalSetting GetGoalSetting(DockState dockState) {
            return new GoalSetting(dockState);
        }

    }


    public interface IVehiclePlanner : IBaseVehiclePlanner {

    }

    public class VehiclePlanner : BaseVehiclePlanner, IVehiclePlanner {

    }
}
