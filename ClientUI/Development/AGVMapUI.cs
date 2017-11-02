using GLUI;
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

namespace ClientUI.Development
{
    public partial class AGVMapUI : CtDockContent
    {
        /// <summary>
        /// 共用建構方法
        /// </summary>
        public AGVMapUI(DockState defState = DockState.Float)
            : base(defState)
        {
            InitializeComponent();
            FixedSize = new Size(718, 814);
        }

        /// <summary>
        /// 獲得地圖控制器控制
        /// </summary>
        public IScene Ctrl { get { return uiControl.BaseCtrl; } }
    }
}
