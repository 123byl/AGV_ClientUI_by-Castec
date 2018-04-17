using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Net;

namespace CtLib.Forms.TestPlatform {

    /// <summary>[測試介面] Socket</summary>
    public partial class Test_AsyncPipe : Form {

		#region Declaration - Fields
		private CtAsyncPipe mPipe;
        private TransDataFormats mDataType = TransDataFormats.String;
        private NumericFormats mDataFormat = NumericFormats.Decimal;
		private bool mSendWithEnter = true;
        #endregion

        #region Function - Constructors
        /// <summary>開啟 Socket 測試介面</summary>
        public Test_AsyncPipe() {
            InitializeComponent();

            CtInvoke.ComboBoxSelectedIndex(cbMode, 0);
            CtInvoke.ComboBoxSelectedIndex(cbEndLine, 0);
            CtInvoke.ComboBoxSelectedIndex(cbDataType, 0);
        }
        #endregion

        #region Function - Methods
        private void ShowMessage(string data) {
			string strTemp = string.Format("[{0}] {1}", DateTime.Now.ToString("HH:mm:ss.ff"), data);
			CtInvoke.ListBoxInsert(lsbxMsg, 0, strTemp);
        }

        private void ShowMessage(DateTime time, string data) {
			string strTemp = string.Format("[{0}] {1}", time.ToString("HH:mm:ss.ff"), data);
			CtInvoke.ListBoxInsert(lsbxMsg, 0, strTemp);
		}
        #endregion

        #region Function - CtAsyncSocket Events
        void mPipe_OnPipeEvents(object sender, PipeEventArgs e) {
			switch (e.Event) {
				case PipeEvents.WaitForConnection:
					bool waitStt = CtConvert.CBool(e.Value);
					ShowMessage(e.Time, waitStt ? "等候用戶端連線..." : "停止等待連線");
					CtInvoke.ControlText(btnConnect, waitStt ? "Stop" : "Start");
					CtInvoke.ControlTag(btnConnect, waitStt);
					CtInvoke.PictureBoxImage(pbStt, waitStt ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
					CtInvoke.ControlEnabled(cbMode, !waitStt);
					CtInvoke.ControlEnabled(txtPipe, !waitStt);
					break;
				case PipeEvents.Connection:
					bool cntStt = CtConvert.CBool(e.Value);
					if (mPipe.PipeMode == PipeModes.Client) {
						ShowMessage(e.Time, cntStt ? "已連線" : "已中斷連線");
						CtInvoke.ControlText(btnConnect, cntStt ? "Disconnect" : "Connect");
						CtInvoke.ControlTag(btnConnect, cntStt);
						CtInvoke.PictureBoxImage(pbStt, cntStt ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
						CtInvoke.ControlEnabled(cbMode, !cntStt);
						CtInvoke.ControlEnabled(txtPipe, !cntStt);
						CtInvoke.ControlEnabled(txtServer, !cntStt);
					} else {
						ShowMessage(e.Time, cntStt ? "用戶端已連線" : "用戶端已離線");
					}

					CtInvoke.ControlEnabled(cbDataType, !cntStt);
					CtInvoke.ControlEnabled(gbSend, cntStt | chkAutoClose.Checked);
					break;
				case PipeEvents.DataReceived:
					if (mPipe.DataFormat == TransDataFormats.EnumerableOfByte) {
						ShowMessage(e.Time, "[RX] " + CtConvert.CStr(e.Value as List<byte>, mDataFormat));
					} else {
						ShowMessage(e.Time, "[RX] " + e.Value.ToString());
					}
					break;
				case PipeEvents.DataSend:
					if (mPipe.DataFormat == TransDataFormats.EnumerableOfByte) {
						ShowMessage(e.Time, "[TX] " + CtConvert.CStr(e.Value as List<byte>, mDataFormat));
					} else {
						ShowMessage(e.Time, "[TX] " + e.Value.ToString());
					}
					break;
				default:
					break;
			}
        }
        #endregion

        #region Function - Interface Events
        private void cbMode_SelectedIndexChanged(object sender, EventArgs e) {
            if (cbMode.SelectedIndex == 0) {
                CtInvoke.ControlEnabled(txtServer, true);
                CtInvoke.ControlText(btnConnect, "Connect");
                CtInvoke.ControlTag(btnConnect, false);
				CtInvoke.ControlVisible(chkAutoClose, true);
			} else {
                CtInvoke.ControlEnabled(txtServer, false);
                CtInvoke.ControlText(btnConnect, "Start");
                CtInvoke.ControlTag(btnConnect, false);
				CtInvoke.ControlVisible(chkAutoClose, false);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e) {
            try {
                if (CtConvert.CBool(btnConnect.Tag)) {
					mPipe.Disconnect();
					mPipe.OnPipeEvents -= mPipe_OnPipeEvents;
                    CtInvoke.ControlEnabled(cbDataType, true);
                } else {
					mPipe = new CtAsyncPipe(mDataType, true);
					mPipe.OnPipeEvents += mPipe_OnPipeEvents;
					if (cbMode.SelectedIndex == 0) {
						mPipe.ClientConnect(txtPipe.Text, txtServer.Text);
					} else {
						mPipe.ServerListen(txtPipe.Text);
					}
                    CtInvoke.ControlEnabled(cbDataType, false);
                }
            } catch (Exception ex) {
                ShowMessage(ex.Message);
            }
        }

        private void btnSend_Click(object sender, EventArgs e) {
			string sendStr = string.Empty;
			List<byte> sendByte = null;
            if (mDataType == TransDataFormats.String) {
				sendStr = txtSend.Text;

                switch (cbEndLine.SelectedIndex) {
                    case 1:
						sendStr += CtConst.NewLine;
                        break;
                    case 2:
						sendStr += CtConst.Cr;
                        break;
                    case 3:
						sendStr += CtConst.Lf;
                        break;
                }
            } else {
                List<string> strSplit = txtSend.Text.Split(CtConst.CHR_SEPERATOR, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (strSplit != null && strSplit.Count > 0) {
					sendByte = strSplit.ConvertAll(val => Convert.ToByte(val, (int)mDataFormat));
                }
            }

			if (chkAutoClose.Checked) {
				if (cbMode.SelectedIndex == 0) {    //Client
					if (!mPipe.IsConnected) {
						mPipe = new CtAsyncPipe(mDataType, true);
						mPipe.OnPipeEvents += mPipe_OnPipeEvents;
						mPipe.ClientConnect(txtPipe.Text, txtServer.Text);
					}
					while (!mPipe.IsConnected) {
						Thread.Sleep(1);
					}
					if (mDataType == TransDataFormats.String) mPipe.Send(sendStr);
					else mPipe.Send(sendByte);
					mPipe.Disconnect();
				} else {
					if (!mPipe.IsConnected) {
						mPipe = new CtAsyncPipe(mDataType, true);
						mPipe.OnPipeEvents += mPipe_OnPipeEvents;
						mPipe.ServerListen(txtServer.Text);
					}
					if (mDataType == TransDataFormats.String) mPipe.Send(sendStr);
					else mPipe.Send(sendByte);
					mPipe.Disconnect();
				}
			} else if (mDataType == TransDataFormats.String)
				mPipe.Send(sendStr);
			else if (mDataType == TransDataFormats.EnumerableOfByte)
				mPipe.Send(sendByte);
        }
        
        private void cbDataType_SelectedIndexChanged(object sender, EventArgs e) {
            if (cbDataType.SelectedIndex > -1) {
                mDataType = (TransDataFormats)cbDataType.SelectedIndex;

                if (mDataType == TransDataFormats.String) {
                    CtInvoke.ComboBoxClear(cbEndLine);
                    CtInvoke.ComboBoxAdd(cbEndLine, new object[] { "None", "CrLf", "Cr", "Lf" });
                } else {
                    CtInvoke.ComboBoxClear(cbEndLine);
                    CtInvoke.ComboBoxAdd(cbEndLine, new object[] { "Hex", "Dec" });
                }
                CtInvoke.ComboBoxSelectedIndex(cbEndLine, 0);
            }
        }

        private void cbEndLine_SelectedIndexChanged(object sender, EventArgs e) {
            if (cbEndLine.SelectedIndex == 0) mDataFormat = NumericFormats.Hexadecimal;
            else mDataFormat = NumericFormats.Decimal;
        }

		private void miSaveLog_Click(object sender, EventArgs e) {
			IEnumerable<string> content = lsbxMsg
											.InvokeIfNecessary(() => lsbxMsg.Items).
											Cast<object>().
											Select(obj => obj.ToString());

			if (content != null && content.Any()) {
				using (SaveFileDialog dialog = new SaveFileDialog()) {
					dialog.Filter = "紀錄檔 | *.log";
					if (dialog.ShowDialog() == DialogResult.OK) {
						CtFile.WriteFile(dialog.FileName, content);
					}
				}
			}
		}

		private void miClrMsg_Click(object sender, EventArgs e) {
			CtInvoke.ListBoxClear(lsbxMsg);
		}

		private void txtSend_KeyPress(object sender, KeyPressEventArgs e) {
			if (mSendWithEnter && e.KeyChar == '\r') {
				e.Handled = true;
				btnSend.PerformClick();
			}
		}

		private void chkEnter_CheckStateChanged(object sender, EventArgs e) {
			mSendWithEnter = CtInvoke.CheckBoxChecked(chkEnter);
		}

		#endregion
	}
}
