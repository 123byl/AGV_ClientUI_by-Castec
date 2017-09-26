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

using CtLib.Library;
using MapProcessing;

namespace ClientUI {


    /// <summary>
    /// Console介面
    /// </summary>
    public partial class CtConsole : CtDockContent {
        
        #region Funciton - Constructors
        
        /// <summary>
        /// 共用建構方法
        /// </summary>
        public CtConsole(DockState defState = DockState.Float)
            :base(defState) {
            InitializeComponent();
            FixedSize = new Size(424, 300);
        }

        #endregion Function - Constructors

    }
}
