using CtLib.Library;
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

namespace VehiclePlanner.Forms {

    /// <summary>
    /// iTS連線器
    /// </summary>
    public partial class ConnectITS : Form {

        #region Declaration - Fields

        /// <summary>
        /// iTS控制器物件參考
        /// </summary>
        private IBaseITSController rController;

        #endregion Declaration - FIelds

        #region Function - Constructors

        /// <summary>
        /// iTS連線器建構
        /// </summary>
        /// <param name="controller">iTS控制器物件</param>
        public ConnectITS(IBaseITSController controller) {
            InitializeComponent();
            rController = controller;
            rController.ReceivedBoradcast += rController_ReceivedBoradcast;
            Bindings(controller);
        }

        #endregion Funciton - Constructors

        #region Function - Public Methods

        /// <summary>
        /// 使用者輸入目標iTS IP
        /// </summary>
        /// <param name="ip">目標iTS IP</param>
        /// <returns>是否連線</returns>
        public bool GetIP(out string ip) {
            ip = string.Empty;
            this.ShowDialog();
            bool cnn;
            if (cnn = DialogResult == DialogResult.OK) {
                ip = cboHostIP.Text;
            }
            return cnn;
        }

        #endregion Function - Public Methods
       
        #region Funciton - Events

        /// <summary>
        /// 廣播回覆接收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rController_ReceivedBoradcast(object sender, BroadCast.BroadcastEventArgs e) {
            string ip = e.Remote.Address.ToString();
            cboHostIP.InvokeIfNecessary(() => {
                if (!cboHostIP.Items.Contains(ip)) {
                    cboHostIP.Items.Add(ip);
                }
            });
        }

        /// <summary>
        /// 進行連線
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// 發出廣播尋找可用的iTS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFind_Click(object sender, EventArgs e) {
            rController.FindCar();
        }

        /// <summary>
        /// 表單關閉事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectITS_FormClosing(object sender, FormClosingEventArgs e) {
            rController.ReceivedBoradcast -= rController_ReceivedBoradcast;
        }

        #endregion Function - Events

        #region Function - Private Methods

        /// <summary>
        /// 資料綁定
        /// </summary>
        /// <param name="source"></param>
        private void Bindings(IBaseITSController source) {
            if (source == null) return;

            /*-- Invoke方法委派 --*/
            if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);
            /*-- iTS清單 --*/
            cboHostIP.DataSource = source.ITSs;
            cboHostIP.DisplayMember = source.ITSs.Columns[0].Caption;
            /*-- HostIP --*/
            cboHostIP.DataBindings.Add(nameof(cboHostIP.Text), source, nameof(source.HostIP));
            cboHostIP.TextChanged += (sender, e) => { source.HostIP = cboHostIP.Text; };
            /*-- 是否可搜索 --*/
            string dataMember = nameof(source.IsSearchable);
            btnFind.DataBindings.Add(nameof(btnFind.Enabled), source, dataMember);
            cboHostIP.DataBindings.Add(nameof(cboHostIP.Enabled), source, dataMember);
            btnConnect.DataBindings.Add(nameof(btnConnect.Enabled), source, dataMember);
        }

        #endregion Function - Private Methods
    }
}
