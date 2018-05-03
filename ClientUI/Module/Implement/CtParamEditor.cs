using CtDockSuit;
using INITesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VehiclePlanner.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace VehiclePlanner.Module.Implement {
    public partial class ParamEditor : CtDockContainer {
        
        private CtrlParamEditor mEditor = null;

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public ParamEditor(DockState defState = DockState.Float)
            : base(defState) {
            InitializeComponent();
            mEditor = new CtrlParamEditor() {
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None
            };
            this.Controls.Add(mEditor);
            mEditor.Show();
            FixedSize = new Size(718, 814);
            this.Activated += ParamEditor_Activated;
            this.Deactivate += ParamEditor_Deactivate;
        }
        
        private void ParamEditor_Deactivate(object sender, EventArgs e) {
            Console.WriteLine("Deactivate");
        }
        

        private void ParamEditor_Activated(object sender, EventArgs e) {
            Console.WriteLine($"IsHidden:{this.IsHidden}");
        }
        
    }
}
