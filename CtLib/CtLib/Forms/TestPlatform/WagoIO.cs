using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

using CtLib.Library;
using CtLib.Module.Wago;

namespace CtLib.Forms.TestPlatform {
	/// <summary>[測試介面] Wago 750-352</summary>
	public partial class Test_WagoIO : Form {

		private string mIP = "";
		private int mPort = 0;
		private CtWagoIO mWago = new CtWagoIO();

		/// <summary>開啟 Wago 測試介面</summary>
		public Test_WagoIO() {
			InitializeComponent();

			mWago.OnWagoEvents += mWago_OnWagoEvents;
			CtInvoke.ComboBoxSelectedIndex(cbSetSig, 0);
		}

		private void ShowMessage(string text, Color color) {
			string msg = string.Format("[{0}] {1}\r\n", DateTime.Now.ToString("HH:mm:ss.ff"), text);
			CtInvoke.RichTextBoxAppendText(richInfo, msg, color, true);
		}

		void mWago_OnWagoEvents(object sender, CtWagoIO.WagoIOEventArgs e) {
			switch (e.Event) {
				case CtWagoIO.WagoIOEvents.Connection:
					if (CtConvert.CBool(e.Value)) {
						ShowMessage("已連接至 " + mIP + ":" + mPort.ToString(), Color.Green);
						CtInvoke.PictureBoxImage(picConnect, Properties.Resources.Green_Ball);
					} else {
						ShowMessage("已中斷連線", Color.Red);
						CtInvoke.PictureBoxImage(picConnect, Properties.Resources.Grey_Ball);
					}
					break;

				case CtWagoIO.WagoIOEvents.Exception:
					ShowMessage(e.Value.ToString(), Color.Red);
					break;
			}
		}

		private void btnConnect_Click(object sender, EventArgs e) {
			if (mWago.IsConnect) {
				mWago.Disconnect();
			} else {
				mIP = txtIP.Text;
				mPort = CtConvert.CInt(txtPort.Text);
				mWago.Connect(mIP, mPort);
			}
		}

		private void btnGetCoil_Click(object sender, EventArgs e) {
			bool result = false;
			Stat stt = mWago.GetIO(CtConvert.CUShort(txtGetSigAddr.Text), out result);
			if (stt == Stat.SUCCESS) {
				ShowMessage("I/O: " + txtGetSigAddr.Text + "  State: " + result.ToString(), Color.Black);
			} else {
				ShowMessage("無法取得該I/O", Color.Red);
			}
		}

		private void btnGetReg_Click(object sender, EventArgs e) {
			ushort result = 0;
			Stat stt = mWago.GetRegister(CtConvert.CUShort(txtGetSigAddr.Text), out result);
			if (stt == Stat.SUCCESS) {
				ShowMessage("Register: " + txtGetSigAddr.Text + "  Data: " + result.ToString(), Color.Black);
			} else {
				ShowMessage("無法取得該 Register", Color.Red);
			}
		}

		private void btnGetCoils_Click(object sender, EventArgs e) {
			List<bool> result;
			Stat stt = mWago.GetIO(CtConvert.CUShort(txtGetMutiAddr.Text), CtConvert.CUShort(txtGetMultiCount.Text), out result);
			if (stt == Stat.SUCCESS) {
				string msg = string.Format("I/O 從 {0} 往後讀 {1} 個，狀態為 {2}", txtGetMutiAddr.Text, txtGetMultiCount.Text, string.Join(" ", result.ConvertAll(val => val ? "1" : "0")));
				ShowMessage(msg, Color.Black);
			} else {
				ShowMessage("無法取得該 I/O", Color.Red);
			}
		}

		private void btnGetRegs_Click(object sender, EventArgs e) {
			List<ushort> result;
			Stat stt = mWago.GetRegister(CtConvert.CUShort(txtGetMutiAddr.Text), CtConvert.CUShort(txtGetMultiCount.Text), out result);
			if (stt == Stat.SUCCESS) {
				string msg = string.Format("Register 從 {0} 往後讀 {1} 個，數值為 {2}", txtGetMutiAddr.Text, txtGetMultiCount.Text, string.Join(" ", result));
				ShowMessage(msg, Color.Black);
			} else {
				ShowMessage("無法取得該 Register", Color.Red);
			}
		}

		private void btnSetCoil_Click(object sender, EventArgs e) {
			Stat stt = mWago.SetIO(CtConvert.CUShort(txtSetSigIOAddr.Text), CtConvert.CBool(cbSetSig.SelectedIndex));
			if (stt == Stat.SUCCESS) {
				ShowMessage("設定成功!", Color.Black);
			} else {
				ShowMessage("無法設定該 I/O", Color.Red);
			}
		}

		private void btnSetReg_Click(object sender, EventArgs e) {
			Stat stt = mWago.SetRegister(CtConvert.CUShort(txtSetSigRegAddr.Text), CtConvert.CUShort(txtSetSigRegVal.Text));
			if (stt == Stat.SUCCESS) {
				ShowMessage("設定成功!", Color.Black);
			} else {
				ShowMessage("無法設定該 Register", Color.Red);
			}
		}

		private void btnSetCoils_Click(object sender, EventArgs e) {
			List<bool> data = txtSetMultiVal.Text.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(val => val == "1" ? true : false).ToList();
			Stat stt = mWago.SetIO(CtConvert.CUShort(txtSetMultiAddr.Text), data);
			if (stt == Stat.SUCCESS) {
				ShowMessage("設定成功!", Color.Black);
			} else {
				ShowMessage("無法設定 I/O", Color.Red);
			}
		}

		private void btnSetRegs_Click(object sender, EventArgs e) {
			List<ushort> data = txtSetMultiVal.Text.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(val => ushort.Parse(val)).ToList();
			Stat stt = mWago.SetRegister(CtConvert.CUShort(txtSetMultiAddr.Text), data);
			if (stt == Stat.SUCCESS) {
				ShowMessage("設定成功!", Color.Black);
			} else {
				ShowMessage("無法設定 Registers", Color.Red);
			}
		}

		private void btnRun_Click(object sender, EventArgs e) {
			Thread threadRun = CtThread.CreateThread("TestRun", tsk_Running);
		}

		private void tsk_Running() {
			byte proc = 0;
			ushort value = 0;
			//int delay = 1;
			do {

				switch (proc) {
					case 0:
						mWago.SetRegister(0, 0);
						//CtTimer.Delay(delay);
						proc++;
						break;
					case 1:
						mWago.SetRegister(0, 0xFFFF);
						//CtTimer.Delay(delay);
						proc++;
						break;
					case 2:
						value = 1;
						mWago.SetRegister(0, value);
						//CtTimer.Delay(delay);

						for (int i = 0; i < 15; i++) {
							value <<= 1;
							mWago.SetRegister(0, value);
							//CtTimer.Delay(delay);
						}
						proc++;
						break;
					case 3:
						//value = 0x8000;
						for (int i = 0; i < 16; i++) {
							value >>= 1;
							mWago.SetRegister(0, value);
							//CtTimer.Delay(delay);
						}
						proc++;
						break;
					case 4:
						value = 0;
						for (int i = 0; i < 16; i++) {
							value <<= 1;
							value++;
							mWago.SetRegister(0, value);
							//CtTimer.Delay(delay);
						}
						proc++;
						break;
					case 5:
						for (int i = 0; i < 15; i++) {
							value >>= 1;
							mWago.SetRegister(0, value);
							//CtTimer.Delay(delay);
						}
						proc = 0;
						break;
				}

			} while (mWago.IsConnect);
		}
	}
}
