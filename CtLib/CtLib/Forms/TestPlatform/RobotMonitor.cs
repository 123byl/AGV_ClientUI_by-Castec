using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;

using CtLib.Module.Adept;
using CtLib.Library;
using CtLib.Module.Utility;
using CtLib.Module.Net;
namespace CtLib.Module.Adept {

    ///<summary>用於監測手臂Joint變化量</summary>
    public partial class RobotMonitor : Form {

        #region Declaration - Fields
      
        private List<Label> mCoordinate = new List<Label>();
        ///<summary>用於顯示Joint的控制項</summary>
        private List<Label> mLocation = new List<Label>();
        ///<summary>用於顯示Joint變化量的控制項</summary>
        private List<Label> mDeltaLocation = new List<Label>();
        ///<summary>觀測物件</summary>
        private CtRobotMonitor rMonitor = null;

        #endregion Declaration - Fields

        #region Function - Constructors

        ///<summary>表單上控制項建置</summary>
        ///<param name="withRef">是否有物件參考</param>
        protected RobotMonitor(bool withRef) {
            InitializeComponent();

            /*- 將Label控制項進行分類 -*/
            CollectLabel();

            if (!withRef) ReadConfig(new CtRobotMonitor(new CtAce()));
            
        }

        ///<summary>無參考建置</summary>
        public RobotMonitor() : this(false) { }

        ///<summary>傳入Ace物件參考進行建構</summary>
        ///<param name="monitor">ACE實例參考</param>
        internal RobotMonitor(CtRobotMonitor monitor):this(monitor != null) {
            /*- 取得觀測物件參考-*/
            if (monitor != null) ReadConfig(monitor);
        }

        #endregion Function - Constructors

        #region Function - Events

        #region rMonitor

        ///<summary>布林狀態變更事件處理</summary>
        private void rMonitor_OnBooleanChecned(object sender, BooleanEventArgs e) {
            switch (e.Event) {
                case BooleanEvents.ConnectSocket:
                    UpdateSocketStt(e.Value);
                    break;
                case BooleanEvents.Coordinate:
                    UpdateCoordinate(e.Value);
                    break;
                case BooleanEvents.Monitoring:
                    UpdateMonitoringStt(e.Value);
                    break;
            }

        }

        ///<summary>手臂位置更新事件</summary>
        private void rMonitor_OnLocationUpdate(object sender, LocationUpdateEventArgs e) {
            /*- 變數宣告 -*/
            List<double> now = null;
            List<double> delta = null;
            double? spend = 0;

            /*- 取得數值 -*/
            spend = e.Spend;
            switch (e.Protocol) {
                case PackageProtocol.World:
                    now = e.wNow;
                    delta = e.wDelta;
                    break;
                case PackageProtocol.Joint:
                    now = e.jNow;
                    delta = e.jDelta;
                    break;
                case PackageProtocol.Both:
                    now = rMonitor.IsWorld ? e.wNow : e.jNow;
                    delta = rMonitor.IsWorld ? e.wDelta : e.jDelta;
                    break;
            }

            /*- 顯示於UI -*/
            for (byte idx = 0; idx < now.Count; idx++) {
                CtInvoke.ControlText(mDeltaLocation[idx], delta[idx].ToString("##0.0##"));
                CtInvoke.ControlText(mLocation[idx], now[idx].ToString("##0.0##"));
            }
            CtInvoke.ControlText(lbSpend, spend?.ToString("0.0(ms)"));
        }

        #endregion rMonitor

        #region Form

        ///<summary>表單關閉事件</summary>
        private void RobotMonitor_FormClosing(object sender, FormClosingEventArgs e) {
            rMonitor.Monitor(false);
            //rMonitor.OnLocationUpdate -= rMonitor_OnLocationUpdate;
            e.Cancel = true;
            this.Hide();
        }

        #endregion Form

        #region Button

        ///<summary>V+連線切換事件</summary>
        private void btnConnect_Click(object sender, EventArgs e) {
            CtInvoke.ControlEnabled(btnConnect, false);
            try {
                if (!rMonitor.IsConnectedToVp) {
                    rMonitor.ConnectAce(chkControl.Checked);
                    UpdateAceConnStt(true);
                } else {
                    rMonitor.DisconnectAce();
                    UpdateAceConnStt(false);
                }

            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex, true);
                CtInvoke.ControlEnabled(btnConnect, true);
            }
        }

        ///<summary>Socket連線切換事件</summary>
        private void btnSocket_Click(object sender, EventArgs e) {
            //CtInvoke.ControlEnabled(btnSocket, false);
            if (!rMonitor.IsConnectedToSocket) {
                int port = int.Parse(txtPort.Text);
                if (cboRole.SelectedIndex == 0) {//Server
                    rMonitor.ListenSocket(port);
                } else {//Client
                    rMonitor.ConnectSocket(txtIP.Text, port);
                }
            } else {
                rMonitor.DisconnectSocket();
            }
        }

        ///<summary>監測狀態切換事件</summary>
        private void btnMonitor_Click(object sender, EventArgs e) {
            rMonitor.Monitor(!rMonitor.IsMonitoring);
        }

        ///<summary>單次執行</summary>
        private void btnGet_Click(object sender, EventArgs e) {
            rMonitor.UpdateLcationInfo();
        }
        
        #endregion Button

        #region ComboBox

        ///<summary>座標系切換事件</summary>
        private void cboMode_SelectedIndexChanged(object sender, EventArgs e) {
            CtInvoke.ControlEnabled(cboMode, false);
            rMonitor.IsWorld = cboMode.SelectedIndex == 0;     
        }

        ///<summary>Socket腳色切換事件</summary>
        private void SocketRole_Changed(object sender,EventArgs e) {

        }

        ///<summary>是否傳送完整封包</summary>
        private void chkFullPackage_CheckedChanged(object sender, EventArgs e) {
            rMonitor.IsFullPackage = chkFullPackage.Checked;
        }

        ///<summary>Socket腳色切換事件</summary>
        private void cboRole_SelectedIndexChanged(object sender, EventArgs e) {
            string content = cboRole.SelectedIndex == 0 ? "Start Listen" : "Connect to Server";
            CtInvoke.ControlText(btnSocket,content);
        }

        ///<summary>Socket傳輸格式切換事件</summary>
        private void cboTransFormat_SelectedIndexChanged(object sender, EventArgs e) {
            rMonitor.SocketFormat = (TransDataFormats)cboTransFormat.SelectedIndex;
        }

        #endregion ComboBox

        #endregion Function - Events

        #region Function - Private Methods

        /// <summary>將各 Label 或是 PictureBox 整理至變數裡</summary>
        private void CollectLabel() {
            mCoordinate.Clear();
            mLocation.Clear();
            mDeltaLocation.Clear();
            for (byte i = 1; i < 7; i++) {
                mCoordinate.Add(Controls.Find("lbLoc" + i, true)[0] as Label);
                mLocation.Add(Controls.Find("lbLocVal" + i, true)[0] as Label);
                mDeltaLocation.Add(Controls.Find("lbDelVal" + i, true)[0] as Label);
            }
        }

        ///<summary>讀取配置</summary>
        private void ReadConfig(CtRobotMonitor monitor) {
            this.rMonitor = monitor;
            this.rMonitor.OnLocationUpdate += rMonitor_OnLocationUpdate;
            this.rMonitor.OnBooleanChanged += rMonitor_OnBooleanChecned;
            
            /*- 開始監測手臂位置 -*/
            if (!rMonitor?.IsMonitoring ?? true) {
                rMonitor.Monitor(true);
            }

            /*- 讀取Ace屬性顯示於UI -*/
            /*-[Socket]-*/
            CtInvoke.ComboBoxSelectedIndex(cboRole, rMonitor.Role == SocketRole.Server ? 0 : 1);
            UpdateSocketStt(rMonitor.IsConnectedToSocket);
            CtInvoke.ComboBoxSelectedIndex(cboTransFormat, (int)monitor.SocketFormat);
            /*-[ACE]-*/
            UpdateAceConnStt(rMonitor.IsConnectedToVp);
            CtInvoke.ComboBoxSelectedIndex(cboMode, rMonitor.IsWorld ? 0 : 1);
            CtInvoke.CheckBoxChecked(chkControl, rMonitor.WithControl);
        }

        #region Update UI

        ///<summary>依照座標系變更UI狀態</summary>
        ///<param name="isWorld">座標系</param>
        private void UpdateCoordinate(bool isWorld) {
            if (isWorld) {
                CtInvoke.ControlText(lbLoc1, "X");
                CtInvoke.ControlText(lbLoc2, "Y");
                CtInvoke.ControlText(lbLoc3, "Z");
                CtInvoke.ControlText(lbLoc4, "Yaw");
                CtInvoke.ControlText(lbLoc5, "Pitch");
                CtInvoke.ControlText(lbLoc6, "Roll");

                /*-- 將有可能隱藏的 Label 顯示出來，例如 s800 切成 Joint 時會隱藏 J5 J6 --*/
                for (int idx = rMonitor.JointLength; idx < 6; idx++) {
                    CtInvoke.ControlVisible(mCoordinate[idx], true);
                    CtInvoke.ControlVisible(mLocation[idx], true);
                    CtInvoke.ControlVisible(mDeltaLocation[idx], true);
                }
            } else {
                /*-- 將多餘的軸隱藏起來 --*/
                for (int idx = rMonitor.JointLength; idx < 6; idx++) {
                    CtInvoke.ControlVisible(mCoordinate[idx], false);
                    CtInvoke.ControlVisible(mLocation[idx], false);
                    CtInvoke.ControlVisible(mDeltaLocation[idx], false);
                }

                /*-- 更改標題為 Joint + ? --*/
                for (byte idx = 0; idx < mCoordinate.Count; idx++) {
                    CtInvoke.ControlText(mCoordinate[idx], "Joint " + (idx + 1));
                }
            }
            CtInvoke.ControlEnabled(cboMode, true);
        }

        ///<summary>依照Ace連線狀態更新UI</summary>
        ///<param name="cnn">Ace連線狀態</param>
        private void UpdateAceConnStt(bool cnn) {
            CtInvoke.ControlEnabled(chkControl, !cnn);
            CtInvoke.ControlEnabled(cboMode, cnn || rMonitor.IsConnectedToSocket);
            Bitmap btnImage = null;
            string btnContent = "";
            if (cnn) {
                btnImage = Properties.Resources.Green_Ball;
                btnContent = "Disconnect ACE";
            }else {
                btnImage = Properties.Resources.Grey_Ball;
                btnContent = "Connect to ACE";
            }
            CtInvoke.ControlText(btnConnect, btnContent);
            CtInvoke.ButtonImage(btnConnect,btnImage);
            CtInvoke.ControlEnabled(btnGet, cnn && !rMonitor.IsMonitoring);
            CtInvoke.ControlEnabled(btnConnect, true);
            CtInvoke.ControlEnabled(btnMonitor, cnn);
        }

        ///<summary>依照Socket連線狀態更新UI</summary>
        ///<param name="cnn">Socket連線狀態</param>
        private void UpdateSocketStt(bool cnn) {
            string btnContent = "";
            if (cnn) {
                btnContent = "Disconnect Socket";
            }else {
                btnContent = CtInvoke.ComboBoxSelectedIndex(cboRole) == 0 ? "Start Listen" : "Connect to Server";
            }
            CtInvoke.ControlText(btnSocket, btnContent);
            CtInvoke.ControlEnabled(cboRole, !cnn);
            CtInvoke.ControlEnabled(cboTransFormat, !cnn);
            CtInvoke.ControlEnabled(txtIP, !cnn);
            CtInvoke.ControlEnabled(txtPort, !cnn);
            CtInvoke.ButtonImage(btnSocket,
                cnn?
                Properties.Resources.Green_Ball : 
                Properties.Resources.Grey_Ball);
            CtInvoke.ControlEnabled(btnSocket, true);
            CtInvoke.ControlEnabled(cboMode, cnn || rMonitor.IsConnectedToVp);
            
        }

        ///<summary>依照Monitoring狀態更新UI</summary>
        ///<param name="isMonitoring">Monitoring狀態</param>
        private void UpdateMonitoringStt(bool isMonitoring) {
            string btnTxt = "";
            Bitmap btnImg = null;
            if (isMonitoring) {
                btnTxt = "Stop Monitor";
                btnImg = Properties.Resources.Green_Ball;
            } else {
                btnTxt = "Start Monitor";
                btnImg = Properties.Resources.Grey_Ball;
                mDeltaLocation?.ForEach(lb => CtInvoke.ControlText(lb, "0"));
                mLocation?.ForEach(lb => CtInvoke.ControlText(lb, "0"));
                CtInvoke.ControlText(lbSpend, "0(ms)");
            }
            CtInvoke.ControlText(btnMonitor, btnTxt);
            CtInvoke.ButtonImage(btnMonitor, btnImg);
            CtInvoke.ControlEnabled(btnGet, !isMonitoring && rMonitor.IsConnectedToVp);            
        }

        #endregion Update UI

        #endregion Function - PrivateMethods

        private void RobotMonitor_Load(object sender, EventArgs e) {
       }

        private void RobotMonitor_Shown(object sender, EventArgs e) {
            
        }


    }
}
