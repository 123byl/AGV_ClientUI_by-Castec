using CtDockSuit;
using CtLib.Library;
using Geometry;
using GLUI;
using System;
using System.Drawing;
using System.Windows.Forms;
using VehiclePlanner.Core;
using VehiclePlanner.Module.Interface;
using WeifenLuo.WinFormsUI.Docking;

namespace VehiclePlanner.Module.Implement {

    public partial class AGVMapUI : CtDockContainer, IBaseMapGL {

        #region Declaration  - Fields
        
        #endregion Declaration  - Fields

        #region Declaration - Properties

        public override DockState DockState {
            get {
                return pnlHide.Visible ? DockState.Hidden : DockState.Document;
            }

            set {
                pnlHide.Visible = value != DockState.Document;
                DockStateChanged?.Invoke(this, new EventArgs());
            }
        }

        #endregion Declaration - Properties

        #region Function - Constructors

        protected AGVMapUI():base() { }

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public AGVMapUI(DockState defState = DockState.Float)
            : base(defState) {
            InitializeComponent();
            FixedSize = new Size(718, 814);
        }

        #endregion Function - Constructors

        #region Funciton - Public Methods

        public override event EventHandler DockStateChanged;

        protected override void ShowWindow() {
            base.ShowWindow();
            DockState = DockState.Document;
        }

        protected override void HideWindow() {
            DockState = DockState.Hidden;
        }

        #endregion Funciton - Public Methods

        #region Function - Private Methods

        protected override void OnFormClosing(FormClosingEventArgs e) {
            e.Cancel = true;
        }

        #endregion Function - Private Methods

        #region Implement - IMapGL
        
        #endregion Implement - IMapGL

        #region Implement - IDataDisplay<ICtVehiclePlanner>

        /// <summary>
        /// 資料綁定
        /// </summary>
        /// <param name="source">資料來源</param>
        public virtual void Bindings(ICtVehiclePlanner source) {
            if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);

        }

        #endregion Implement - IDataDisplay<ICtVehiclePlanner>

    }

}