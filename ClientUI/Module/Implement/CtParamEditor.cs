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

        private KeyboardHook mKeyboardHook = null;

        public KeyboardHook KeyboardHook {
            get => mEditor.KeyboardHook;
            set => mEditor.KeyboardHook = value;
        }

        private CtrlParamEditor mEditor = null;

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public ParamEditor(DockState defState = DockState.Float)
            : base(defState) {
            InitializeComponent();
            mEditor = new CtrlParamEditor();
            mEditor.TopLevel = false;           
            mEditor.Dock = DockStyle.Fill;
            mEditor.FormBorderStyle = FormBorderStyle.None;
            mEditor.Show();
            this.Controls.Add(mEditor);
            FixedSize = new Size(718, 814);
        }
    }
}
