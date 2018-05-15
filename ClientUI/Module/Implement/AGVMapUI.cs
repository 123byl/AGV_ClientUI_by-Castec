using CtDockSuit;
using CtLib.Library;
using System;
using System.Drawing;
using System.Windows.Forms;
using VehiclePlanner.Core;
using VehiclePlanner.Module.Interface;
using WeifenLuo.WinFormsUI.Docking;
using CtLib.Module.Utility;

namespace VehiclePlanner.Module.Implement {

    public partial class BaseMapGL : AuthorityDockContainer, IBaseMapGL {

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

        #region Declaration - Events

        public override event EventHandler DockStateChanged;

        #endregion Declaration - Events

        #region Function - Constructors

        /// <summary>
        /// 給介面設計師使用的建構式，拿掉後繼承該類的衍生類將無法顯示介面設計
        /// </summary>
        protected BaseMapGL():base() {
            InitializeComponent();
        }

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public BaseMapGL(BaseVehiclePlanner_Ctrl refUI, DockState defState = DockState.Float)
            : base(refUI,defState) {
            InitializeComponent();
            FixedSize = new Size(718, 814);
        }

        #endregion Function - Constructors

        #region Funciton - Public Methods

        public override bool IsVisiable(AccessLevel lv) {
            return lv > AccessLevel.None;
        }
        
        #endregion Funciton - Public Methods

        #region Function - Private Methods

        protected override void ShowWindow() {
            base.ShowWindow();
            DockState = DockState.Document;
        }

        protected override void HideWindow() {
            DockState = DockState.Hidden;
        }

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
        public virtual void Bindings(IBaseVehiclePlanner source) {
            if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);

        }

        #endregion Implement - IDataDisplay<ICtVehiclePlanner>

    }

}