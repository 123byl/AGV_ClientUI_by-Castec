using CtLib.Library;
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
using VehiclePlanner.Core;
using VehiclePlanner.Module.Interface;
using WeifenLuo.WinFormsUI.Docking;

namespace VehiclePlanner.Module.Implement {
    public partial class AGVMapUI : CtDockContent,IMapGL
    {
        
        #region Function - Constructors

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public AGVMapUI(DockState defState = DockState.Float)
            : base(defState)
        {
            InitializeComponent();
            FixedSize = new Size(718, 814);
        }

        #endregion Functoin - Constructors

        #region Funciton - Public Methods

        public override DockState DockState {
            get {
                return pnlHide.Visible ? DockState.Hidden : DockState.Document;                
            }

            set {
                pnlHide.Visible = value != DockState.Document;
                DockStateChanged?.Invoke(this, new EventArgs());
            }
        }

        public override event EventHandler DockStateChanged;

        protected override void ShowWindow() {
            base.ShowWindow();
            DockState = DockState.Document;
        }

        protected override void HideWindow() {
            DockState = DockState.Hidden;
        }

        #endregion Function - Public Methods

        #region Function - Private Methods

        protected override void OnFormClosing(FormClosingEventArgs e) {
            e.Cancel = true;
        }

        #endregion Funciton - Private Methods

        #region Implement - IMapGL

        /// <summary>
        /// 獲得地圖控制器控制
        /// </summary>
        public IScene Ctrl { get { return uiControl.BaseCtrl; } }

        #endregion Implement - IMapGL

        #region Implement - IDataDisplay<ICtVehiclePlanner>

        /// <summary>
        /// 資料綁定
        /// </summary>
        /// <param name="source">資料來源</param>
        public void Bindings(ICtVehiclePlanner source) {
            if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);
        }

        #endregion Implement - IDataDisplay<ICtVehiclePlanner>

    }
}
