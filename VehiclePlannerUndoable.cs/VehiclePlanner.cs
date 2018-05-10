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
    public partial class VehiclePlanner : BaseVehiclePlanner_Ctrl {

        public VehiclePlanner(IBaseVehiclePlanner vehiclePlanner):base(vehiclePlanner) {
            InitializeComponent();
            //rVehiclePlanner = vehiclePlanner;
            //SetEvents();
            //Bindings(rVehiclePlanner.Controller);

            //mVC = new FakeVehicleConsole(!DesignMode);
        }

        protected override BaseMapGL GetMapGL(DockState dockState) {
            return new MapGL(dockState);
        }

    }
}
