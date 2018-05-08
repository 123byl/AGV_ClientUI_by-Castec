using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VehiclePlannerAGVBase {
    static class Program {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            IVehiclePlanner mVehiclePlanner = new VehiclePlanner();
            Application.Run(new CtVehiclePlanner_Ctrl(mVehiclePlanner));
        }
    }
}
