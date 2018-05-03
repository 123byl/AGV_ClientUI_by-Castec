using CtDockSuit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace VehiclePlanner.Module.Implement {
    public partial class CtParamEditor : CtDockContainer {

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public CtParamEditor(DockState defState = DockState.Float)
            : base(defState) {
            InitializeComponent();
            FixedSize = new Size(718, 814);
        }
    }
}
