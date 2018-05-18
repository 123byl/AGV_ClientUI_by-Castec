using CtBind;
using CtDockSuit;
using CtLib.Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using VehiclePlanner.Core;
using VehiclePlanner.Module.Interface;
using VehiclePlanner.Partial.VehiclePlannerUI;
using WeifenLuo.WinFormsUI.Docking;
using CtLib.Module.Utility;

namespace VehiclePlanner.Module.Implement {

    /// <summary>
    /// 測試功能介面
    /// </summary>
    public partial class BaseTesting : AuthorityDockContainer, ITesting {

        #region Declaration - Fields

        private readonly object mKey = new object();

        /// <summary>
        /// 紀錄鍵盤按下的方向
        /// </summary>
        private List<MotionDirection> mDirs = new List<MotionDirection>();

        #endregion Declaration - Fields

        #region Function - Construcotrs

        /// <summary>
        /// 給介面設計師使用的建構式，拿掉後繼承該類的衍生類將無法顯示介面設計
        /// </summary>
        protected BaseTesting():base() {
            InitializeComponent();
        }

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public BaseTesting(BaseVehiclePlanner_Ctrl refUI, DockState defState = DockState.Float)
            : base(refUI,defState) {
            InitializeComponent();
            FixedSize = new Size(718, 814);
        }

        #endregion Function - Construcotrs

        #region Function - Public Methods

        public override bool IsVisiable(AccessLevel lv) {
            return lv > AccessLevel.Operator;
        }

        #endregion Funciotn - Public Methods

        #region Implement - ITest
        
        #endregion Implement - ITest

        #region Function  - UI Events

        private void btnConnect_Click(object sender, EventArgs e) {
            Task.Run(() => {
                bool cnn = btnConnect.Tag == null || (btnConnect.Tag is bool && !(bool)btnConnect.Tag);
                rUI.Connect(cnn);
            });
        }

        private void btnLoadOri_Click(object sender, EventArgs e) {
            rUI.ITest_LoadOri();
        }

        private void btnGetMap_Click(object sender, EventArgs e) {
            Task.Run(() => {
                rUI.GetMap();
            });
        }

        private void btnLoadMap_Click(object sender, EventArgs e) {
            rUI.ITest_LoadMap();
        }

        private void btnGetOri_Click(object sender, EventArgs e) {
            //Task.Run(() => GetOri.Invoke());
        }

        private void btnGetLaser_Click(object sender, EventArgs e) {
            Task.Run(() => rUI.GetLaser());
        }

        private void btnGetCarStatus_Click(object sender, EventArgs e) {
            Task.Run(() => rUI.ITest_GetCar());
        }

        private void btnSendMap_Click(object sender, EventArgs e) {
            rUI.ITest_SendMap();
        }

        private void btnSimplyOri_Click(object sender, EventArgs e) {
            //SimplifyOri?.Invoke();
        }

        private void btnSetVelo_Click(object sender, EventArgs e) {
            Task.Run(() => {
                if (int.TryParse(txtVelocity.Text, out int velocity)) {
                    rUI.SetVelocity(velocity);
                }
            });
        }

        private void btnClrMap_Click(object sender, EventArgs e) {
            rUI.ClearMap();
        }

        private void btnServoOnOff_Click(object sender, EventArgs e) {
            Task.Run(() => {
                bool servoOn = btnServoOnOff.Tag == null || (btnServoOnOff.Tag is bool && !(bool)btnServoOnOff.Tag);
                rUI.MotorServoOn(servoOn);
            });
        }

        private void btnPosConfirm_Click(object sender, EventArgs e) {
            Task.Run(() => {
                rUI.CarPosConfirm();
            });
        }

        private void btnSetCar_Click(object sender, EventArgs e) {
            rUI.ITest_SettingCarPos();
        }

        private void btnScan_Click(object sender, EventArgs e) {
            Task.Run(() => {
                rUI.StartScan();
            });
        }

        private void btnMotionController_Click(object sender, EventArgs e) {
            rUI.ShowMotionController();
        }

        private void btnFind_Click(object sender, EventArgs e) {
            rUI.FindCar();
        }

        #endregion Function  - UI Events

        #region Implement - IDataDisplay<ICtVehiclePlanner>

        /// <summary>
        /// 資料綁定
        /// </summary>
        /// <param name="source">資料來源</param>
        public void Bindings(IBaseITSController source) {
            if (source == null) return;
            
            /*-- Invoke方法委派 --*/
            if ( source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);
            /*-- 地圖掃描狀態 --*/
            string dataMember = nameof(source.IsScanning);
            btnScan.DataBindings.Add(nameof(btnScan.Text), source, dataMember).Format += (sender, e) => {
                e.Value = (bool)e.Value ? "Stop scan" : "Scan";
            };
            btnScan.DataBindings.Add(nameof(btnScan.Tag), source, dataMember);
            /*-- 連線狀態 --*/
            dataMember = nameof(source.IsConnected);
            btnConnect.DataBindings.Add(nameof(btnConnect.Text), source, dataMember).Format += (sender, e) => {
                e.Value = (bool)e.Value ? "iTS Connected" : "Connect iTS";
            };
            btnConnect.DataBindings.Add(nameof(btnConnect.Image), source, dataMember).Format += (sender, e) => {
                e.Value = (bool)e.Value ? Properties.Resources.Connect : Properties.Resources.Disconnect;
            };
            btnConnect.DataBindings.Add(nameof(btnConnect.Tag), source, dataMember);
            /*-- 雷射AutoReport --*/
            btnGetCarStatus.DataBindings.Add(nameof(btnGetCarStatus.BackColor), source, nameof(source.IsAutoReport)).Format += (sender, e) => {
                e.Value = (bool)e.Value ? Color.Green : Color.Transparent;
            };
            /*-- 馬達狀態 --*/
            dataMember = nameof(source.IsMotorServoOn);
            btnServoOnOff.DataBindings.Add(nameof(btnServoOnOff.Text), source, dataMember).Format += (sender, e) => {
                e.Value = (bool)e.Value ? "ON" : "OFF";
            };
            btnServoOnOff.DataBindings.Add(nameof(btnServoOnOff.BackColor), source, dataMember).Format += (sender, e) => {
                e.Value = (bool)e.Value ? Color.Green : Color.Red;
            };
            btnServoOnOff.DataBindings.Add(nameof(btnServoOnOff.Tag), source, dataMember);
            /*-- iTS清單 --*/
            cboHostIP.DataSource = source.ITSs;
            cboHostIP.DisplayMember = source.ITSs.Columns[0].Caption;
            /*-- HostIP --*/
            cboHostIP.DataBindings.Add(nameof(cboHostIP.Text), source, nameof(source.HostIP));
            cboHostIP.TextChanged += (sender, e) => { source.HostIP = cboHostIP.Text; };
            /*-- 是否可搜索 --*/
            dataMember = nameof(source.IsSearchable);
            btnFind.DataBindings.Add(nameof(btnFind.Enabled), source, dataMember);
            cboHostIP.DataBindings.Add(nameof(cboHostIP.Enabled), source, dataMember);
            /*-- 是否可連線 --*/
            btnConnect.DataBindings.Add(nameof(btnConnect.Enabled), source, nameof(source.IsConnectable));
        }

        #endregion Implement - IDataDisplay<ICtVehiclePlanner>
    }
}