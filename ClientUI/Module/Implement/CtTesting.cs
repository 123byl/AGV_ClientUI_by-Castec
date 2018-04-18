using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
using CtLib.Library;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VehiclePlanner.Module.Interface;
using VehiclePlanner.Partial.VehiclePlannerUI;
using VehiclePlanner.Core;
using CtLib.Module.Utility;

namespace VehiclePlanner.Module.Implement {
    /// <summary>
    /// 測試功能介面
    /// </summary>
    public partial class CtTesting : CtDockContent ,ITesting{

        #region Declaration - Fields

        private readonly object mKey = new object();

        /// <summary>
        /// 紀錄鍵盤按下的方向
        /// </summary>
        private List<MotionDirection> mDirs = new List<MotionDirection>();

        private AGV mAGV = new AGV();
        #endregion Declaration - Fields

        #region Function - Construcotrs

        /// <summary>
        /// 共用建構方法
        /// </summary>

        public CtTesting(DockState defState = DockState.Float)
            :base(defState) {
            InitializeComponent();
            FixedSize = new Size(718, 814);
        }

        #endregion Function - Constructors

        #region Implement - ITest

        public event Events.GoalSettingEvents.DelLoadMap LoadMap;
        public event Events.TestingEvents.DelLoadOri LoadOri;
        public event Events.TestingEvents.DelGetOri GetOri;
        public event Events.TestingEvents.DelGetMap GetMap;
        public event Events.TestingEvents.DelGetLaser GetLaser;
        public event Events.TestingEvents.DelGetCar GetCar;
        public event Events.TestingEvents.DelSendMap SendMap;
        public event Events.TestingEvents.DelSimplifyOri SimplifyOri;
        public event Events.TestingEvents.DelSetVelocity SetVelocity;
        public event Events.TestingEvents.DelConnect Connect;
        public event Events.TestingEvents.DelMotorServoOn MotorServoOn;
        public event Events.TestingEvents.DelClearMap ClearMap;
        public event Events.TestingEvents.DelSettingCarPos SettingCarPos;
        public event Events.TestingEvents.DelCarPosConfirm CarPosConfirm;
        public event Events.TestingEvents.DelStartScan StartScan;
        public event Events.TestingEvents.DelShowMotionController ShowMotionController;
        public event Events.TestingEvents.DelFind Find;

        /// <summary>
        /// 依照連線狀態變更UI介面狀態
        /// </summary>
        /// <param name="isConnect"></param>
        public void SetServerStt(bool isConnect) {
            //string btnTxt = "Connect AGV";
            //Bitmap btnImg = Properties.Resources.Disconnect;
            //if (isConnect) {
            //    btnImg = Properties.Resources.Connect;
            //    btnTxt = "AGV Connected";
            //}
            //CtInvoke.ControlTag(btnConnect, isConnect);
            //CtInvoke.ControlText(btnConnect, btnTxt);
            //CtInvoke.ButtonImage(btnConnect, btnImg);

            //CtInvoke.ControlEnabled(btnGetLaser, isConnect);
            //CtInvoke.ControlEnabled(btnGetOri, isConnect);
            //CtInvoke.ControlEnabled(btnIdleMode, isConnect);
            //CtInvoke.ControlEnabled(btnWorkMode, isConnect);
            //CtInvoke.ControlEnabled(btnMapMode, isConnect);
            //CtInvoke.ControlEnabled(btnGetCarStatus, isConnect);
            //CtInvoke.ControlEnabled(btnPosConfirm, isConnect);
            //CtInvoke.ControlEnabled(btnServoOnOff, isConnect);

            //CtInvoke.ControlEnabled(btnSetVelo, isConnect);
            //CtInvoke.ControlEnabled(btnGetMap, isConnect);
            //CtInvoke.ControlEnabled(btnSendMap, isConnect);
            //CtInvoke.ControlEnabled(txtVelocity, isConnect);

            if (!isConnect) {
                //CarMode = CarMode.OffLine;
                //CtInvoke.ControlEnabled(btnUp, false);
                //CtInvoke.ControlEnabled(btnStartStop, false);
                //CtInvoke.ControlEnabled(btnLeft, false);
                //CtInvoke.ControlEnabled(btnRight, false);
                //CtInvoke.ControlEnabled(btnDown, false);
                //} else if (CarMode == CarMode.OffLine) {
                //    CarMode = CarMode.Idle;
            }
        }

        /// <summary>
        /// 依照雷射自動取得狀態變更UI介面
        /// </summary>
        /// <param name="isGettingLaser"></param>
        public void SetLaserStt(bool isGettingLaser) {
            //CtInvoke.ControlBackColor(btnGetCarStatus, isGettingLaser ? Color.Green : Color.Transparent);
        }

        /// <summary>
        /// 是否解鎖Ori檔操作
        /// </summary>
        /// <param name="isUnLock"></param>
        public void UnLockOriOperator(bool isUnLock) {
            CtInvoke.ControlEnabled(btnSimplyOri, isUnLock);
        }

        /// <summary>
        /// 依照馬達狀態變更UI介面
        /// </summary>
        /// <param name="isOn"></param>
        public void ChangedMotorStt(bool isOn) {
            //Color color = Color.Empty;
            //string content = string.Empty;
            //if (isOn) {
            //    color = Color.Green;
            //    content = "ON";
            //} else {
            //    color = Color.Red;
            //    content = "OFF";
            //}
            //CtInvoke.ControlTag(btnServoOnOff, isOn);
            //CtInvoke.ControlBackColor(btnServoOnOff, color);
            //CtInvoke.ControlText(btnServoOnOff, content);
        }

        /// <summary>
        /// 設定目標agv IP
        /// </summary>
        /// <param name="host">設定IP</param>
        public void SetHostIP(string host) {
            //if (host != cboHostIP.Text) {
            //    CtInvoke.ControlText(cboHostIP, host);
            //}
        }

        /// <summary>
        /// 依照掃圖狀態變更UI介面
        /// </summary>
        /// <param name="isScanning"></param>
        public void ChangedScanStt(bool isScanning) {
            CtInvoke.ControlText(btnScan, isScanning ? "Stop scan" : " Scan");
            CtInvoke.ControlTag(btnScan, isScanning);
        }

        /// <summary>
        /// 設定可用的iTS IP
        /// </summary>
        /// <param name="ipList">iTS IP 清單</param>
        public void SetIPList(IEnumerable<string> ipList) {
            cboHostIP.InvokeIfNecessary(() => {
                cboHostIP.Items.Clear();
                foreach(string ip in ipList) {
                    cboHostIP.Items.Add(ip);
                }
            });
        }

        #endregion Implement - ITest

        #region Function  - UI Events

        private void btnConnect_Click(object sender, EventArgs e) {
            Task.Run(() => {
                if (btnConnect.Tag == null || (btnConnect.Tag is bool && !(bool)btnConnect.Tag)) {
                    Connect.Invoke(true, cboHostIP.InvokeIfNecessary(() =>cboHostIP.Text));
                } else {
                    Connect.Invoke(false);
                }
            });
        }
        
        private void btnLoadOri_Click(object sender, EventArgs e) {
            LoadOri?.Invoke();
        }
        
        private void btnGetMap_Click(object sender, EventArgs e) {
            Task.Run(() => {
                GetMap?.Invoke();
            });
        }

        private void btnLoadMap_Click(object sender, EventArgs e) {
            LoadMap?.Invoke();
        }

        private void btnGetOri_Click(object sender, EventArgs e) {
            Task.Run(() => GetOri.Invoke());
        }

        private void btnGetLaser_Click(object sender, EventArgs e) {
            Task.Run(() => GetLaser?.Invoke());
        }

        private void btnGetCarStatus_Click(object sender, EventArgs e) {
            Task.Run(() => GetCar?.Invoke());
        }

        private void btnSendMap_Click(object sender, EventArgs e) {
            //Task.Run(() => {
                SendMap?.Invoke();
            //});
        }

        private void btnSimplyOri_Click(object sender, EventArgs e) {
            SimplifyOri?.Invoke();
        }

        private void btnSetVelo_Click(object sender, EventArgs e) {
            Task.Run(() => {
                int velocity = 0;
                if (int.TryParse(txtVelocity.Text, out velocity)) {
                    SetVelocity?.Invoke(velocity);
                }
            });
        }

        private void btnClrMap_Click(object sender, EventArgs e) {
            ClearMap?.Invoke();
        }
        
        private void btnServoOnOff_Click(object sender, EventArgs e) {
            Task.Run(() => {
                if (btnServoOnOff.Tag == null || (btnServoOnOff.Tag is bool && !(bool)btnServoOnOff.Tag)) {
                    MotorServoOn.Invoke(true);
                } else {
                    MotorServoOn.Invoke(false);
                }
            });
        }

        private void btnPosConfirm_Click(object sender, EventArgs e) {
            Task.Run(() => {
                CarPosConfirm?.Invoke();
            });
        }

        private void btnSetCar_Click(object sender, EventArgs e) {
            SettingCarPos?.Invoke();
        }

        private void btnScan_Click(object sender, EventArgs e) {
            Task.Run(() => {
                bool isSacn = btnScan.Tag is bool ? ((bool)btnScan.Tag) : false;
                StartScan?.Invoke(!isSacn);
            });
        }

        private void btnMotionController_Click(object sender, EventArgs e) {
            ShowMotionController.Invoke();
        }

        private void btnFind_Click(object sender, EventArgs e) {
            Find.Invoke();
        }

        #endregion Funciton - UI Events

        #region Implement - IDataDisplay<ICtVehiclePlanner>
        
        /// <summary>
        /// 資料綁定
        /// </summary>
        /// <param name="source">資料來源</param>
        public void Bindings(ICtVehiclePlanner source) {
            /*-- Invoke方法委派 --*/
            if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);
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
            cboHostIP.DataSource = source.iTSs;
            cboHostIP.DisplayMember = source.iTSs.Columns[0].Caption;
        }

        #endregion Implement - IDataDisplay<ICtVehiclPlanner>

    }
    
}
