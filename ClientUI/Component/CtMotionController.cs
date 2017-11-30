using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientUI.Component {
    public partial class CtMotionController : Form {
        public CtMotionController() {
            InitializeComponent();
            foreach(Control ctrl in Controls) {
                ctrl.KeyDown += CtMotionController_KeyDown;
            }
        }

        private void CtMotionController_KeyDown(object sender, KeyEventArgs e) {
            Console.WriteLine(e.KeyCode);
        }

        private void CtMotionController_Click(object sender, EventArgs e) {
            this.Focus();
        }
    }
}
