using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Beckhoff;
using CtLib.Module.Utility;

namespace CtLib.Forms.TestPlatform {
    /// <summary>[測試介面] Beckhoff PLC</summary>
    public partial class Test_BkfPLC : Form, ICtVersion {

        #region Version

        /// <summary>Test_BkfPLC 版本訊息</summary>
        public CtVersion Version { get { return new CtVersion(1, 0, 0, "2014/09/12", "Ahern Kuo"); } }

        /*---------- Version Note ----------
         * 
         * 1.0.0  <Ahern> [2014/09/12]
         *      + 完成初版介面
         *      
         *----------------------------------*/

        #endregion

        #region Declaration - Fields

        /// <summary>Reference of Beckhoff object</summary>
        private CtBeckhoff rBkf;
		private Stopwatch mWatch = new Stopwatch();
		#endregion

		#region Function - Constructor
		/// <summary>開啟測試介面</summary>
		public Test_BkfPLC() {
            InitializeComponent();

			if (rBkf == null) {
				rBkf = new CtBeckhoff();
				rBkf.OnBoolEventChanged += rBkf_OnBoolEventChanged;
				rBkf.OnSymbolChanged += rBkf_OnSymbolChanged;
				rBkf.OnMessage += rBkf_OnMessage;
			}

			ChangeGroupBox(false);
		}
        #endregion

        #region Function - Method

        private void ShowMessage(string info) {
            List<string> lstTemp = new List<string> { DateTime.Now.ToString("HH:mm:ss.ff"), info };
            CtInvoke.DataGridViewAddRow(dgvInfo, lstTemp);
        }

        private void ShowMessage(string info, Color bgColor) {
            List<string> lstTemp = new List<string> { DateTime.Now.ToString("HH:mm:ss.ff"), info };
            CtInvoke.DataGridViewAddRow(dgvInfo, lstTemp, bgColor);
        }

		private void ChangeGroupBox(bool stt) {
			IEnumerable<Control> gbColl  = this.Controls.Cast<Control>().Where(ctrl => ctrl is GroupBox);
			if (gbColl != null && gbColl.Any()) {
				gbColl.ForEach(
					ctrl => ctrl.BeginInvokeIfNecessary(() => ctrl.Enabled = stt)
				);
			}
		}
        #endregion

        #region Function - CtBeckhoff Events

        void rBkf_OnSymbolChanged(object sender, BeckhoffSymbolEventArgs e) {
			mWatch.Restart();

			//用 Linq 稍慢 20 tick 而已...
			//DataGridViewRow tarRow = dgvNotf.Rows.Cast<DataGridViewRow>().FirstOrDefault(row => row.Cells["colNotfName"].Value.ToString() == e.Name);

			DataGridViewRow tarRow = null;
			foreach (DataGridViewRow row in dgvNotf.Rows) {
				if (row.Cells["colNotfName"].Value.ToString() == e.Name) {
					tarRow = row;
					break;
				}
			}
			
			mWatch.Stop();

			if (tarRow != null) {
				tarRow.Cells["colNotfVal"].Value = e.Value;
			}

			Console.WriteLine("OnSymbolChanged >> " + mWatch.ElapsedTicks.ToString() + " Ticks");
		}

        void rBkf_OnBoolEventChanged(object sender, BeckhoffBoolEventArgs e) {
			switch (e.Events) {
				case BeckhoffBoolEvents.Connection:
					ShowMessage(string.Format("已與 {0} {1}連線", rBkf.NetID, (e.Value? "成功" : "中斷")));
					break;
			}
		}

        void rBkf_OnMessage(object sender, BeckhoffMessageEventArgs e) {
            /* According the message type, choose the different background color */
            Color bgColor = Color.Black;
            switch ( e.Type ) {
                case -1:
                    bgColor = Color.Red;
                    break;
                case 0:
                    bgColor = Color.White;
                    break;
                case 1:
                    bgColor = Color.Yellow;
                    break;
            }

            /* Combine the message of Title and Content */
            string strTemp = "[" + e.Title + "] - " + e.Message;

            /* Request show message function to display on DataGridView */
            ShowMessage(strTemp, bgColor);
        }
        #endregion

        private void btnConnect_Click(object sender, EventArgs e) {
			if (rBkf.IsConnected) {
				rBkf.Disconnect();

				ChangeGroupBox(false);
				CtInvoke.ControlEnabled(txtNetID, true);
				CtInvoke.ControlEnabled(txtPort, true);

			} else {
				Stat stt = rBkf.Connect(txtNetID.Text, CtConvert.CInt(txtPort.Text));
				if (stt == Stat.SUCCESS) {

					ChangeGroupBox(true);
					CtInvoke.ControlEnabled(txtNetID, false);
					CtInvoke.ControlEnabled(txtPort, false);
				}
			}
        }

        private void btnVarRead_Click(object sender, EventArgs e) {
            if ( txtVarName.Text != "" ) {
                object objTemp = null;
                rBkf.GetValue(txtVarName.Text, out objTemp);

                if ( objTemp != null ) {
                    CtInvoke.ControlText(txtVarVal, CtConvert.CStr(objTemp));
                }
            }
        }

        private void btnVarWrite_Click(object sender, EventArgs e) {
            if ( txtVarVal.Text != "" ) {
				try {
					rBkf.SetValue(txtVarName.Text, txtVarVal.Text);
				} catch (Exception ex) {
					ShowMessage(ex.Message);
				}
            }
        }

        private void btnVarAdMoni_Click(object sender, EventArgs e) {
            if ( CtConvert.CBool(btnVarAdMoni.Tag) ) {
                rBkf.RemoveNotification(txtVarName.Text);
                CtInvoke.ControlTag(btnVarAdMoni, false);
                CtInvoke.ControlText(btnVarAdMoni, "加入監控");
            } else {
                if ( txtVarName.Text != "" ) {
                    rBkf.AddNotification(txtVarName.Text);
                    CtInvoke.ControlTag(btnVarAdMoni, true);
                    CtInvoke.ControlText(btnVarAdMoni, "停止監控");
                }
            }
        }

        private void txtVarName_TextChanged(object sender, EventArgs e) {
            if ( rBkf != null ) {
                if ( rBkf.IsMonitoring(txtVarName.Text) ) {
                    CtInvoke.ControlTag(btnVarAdMoni, true);
                    CtInvoke.ControlText(btnVarAdMoni, "停止監控");
                } else {
                    CtInvoke.ControlTag(btnVarAdMoni, false);
                    CtInvoke.ControlText(btnVarAdMoni, "加入監控");
                }
            }
        }

        private void btnSttSet_Click(object sender, EventArgs e) {
			try {
				rBkf.SetAdsStatus((AdsStatus)cbSttList.SelectedIndex);
			} catch (Exception ex) {
				ShowMessage(ex.Message);
			}
        }

        private void button2_Click(object sender, EventArgs e) {
			CtInvoke.DataGridViewClear(dgvData);
            List<object> lstObj;
            rBkf.GetValue(txtAryName.Text, out lstObj);
            if (lstObj != null) {
				lstObj.ForEach(
					data => CtInvoke.DataGridViewAddRow(dgvData, data, false, false)
				);				
			}
        }

        private void btnAryWrite_Click(object sender, EventArgs e) {
            if ( dgvData.Rows.Count > 0 ) {
                List<string> lstStr = new List<string>();
                foreach ( DataGridViewRow row in dgvData.Rows ) {
                    lstStr.Add(CtConvert.CStr(row.Cells[0].Value));
                }

				int ofs = (int)CtInvoke.NumericUpDownValue(nudAryOfs);
				for ( int i = 0; i < lstStr.Count; i++ ) {
                    rBkf.SetValue(txtAryName.Text + "[" + CtConvert.CStr(i + ofs) + "]", lstStr[i]);
                }
            }
        }

		private void btnSttGet_Click(object sender, EventArgs e) {
			AdsStatus stt = rBkf.GetAdsStatus();
			CtInvoke.ControlText(txtStt, stt.ToString());
		}

		private void btnNotfAdd_Click(object sender, EventArgs e) {
			string symName;
			Stat stt = CtInput.Text(out symName, "Ads Symbol", "請輸入欲自動取得數值的變數名稱，如 \"MAIN.pF_Start\"");
			if (stt == Stat.SUCCESS && !string.IsNullOrEmpty(symName)) {
				CtInvoke.DataGridViewAddRow(dgvNotf, new object[] { symName, null }, false, false);
				rBkf.AddNotification(symName);
			}
		}

		private void btnNotfDel_Click(object sender, EventArgs e) {
			if (dgvNotf.SelectedRows.Count > 0) {
				DataGridViewRow row = dgvNotf.SelectedRows[0];
				MsgBoxBtn btn = CtMsgBox.Show("Delete Notification", "是否取消監控 " + row.Cells["colNotfName"].Value.ToString() + " ?", MsgBoxBtn.YesNo, MsgBoxStyle.Question);
				if (btn == MsgBoxBtn.Yes) {
					rBkf.RemoveNotification(row.Cells["colNotfName"].Value.ToString());
					dgvNotf.BeginInvokeIfNecessary(() => dgvNotf.Rows.Remove(row));
				}
			}
		}
	}
}
