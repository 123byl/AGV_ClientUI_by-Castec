using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.SerialPort;

namespace CtLib.Forms.TestPlatform {
	/// <summary>[測試介面] SerialPort 測試介面</summary>
	public partial class Test_Serial : Form {

		#region Declaration - Fields
		private CtSerial mSerial;
		private TransDataFormats mDataType = TransDataFormats.String;
		private NumericFormats mDataFormat = NumericFormats.Decimal;
		#endregion

		#region Function - Constructors
		/// <summary>開啟測試介面</summary>
		public Test_Serial() {
			InitializeComponent();

			CtInvoke.ComboBoxSelectedIndex(cbNewLine, 0);
			CtInvoke.ComboBoxSelectedIndex(cbDataType, 0);
		}
		#endregion

		#region Function - Mehtods
		private void ShowData(string data) {
			string strTemp = string.Format("[{0}] {1}", DateTime.Now.ToString("HH:mm:ss.ff"), data);
			CtInvoke.ListBoxInsert(lsbxMsg, 0, strTemp);
		}
		#endregion

		#region Function - Interface Events
		private void button1_Click(object sender, EventArgs e) {
			try {
				if (CtConvert.CBool(btnOpen.Tag)) {
					//CtInvoke.CheckBoxChecked(chkManual, false);
					mSerial.Close();
					mSerial.OnSerialEvents -= mSerial_OnSerialEvents;
					CtInvoke.ControlEnabled(cbDataType, true);
					CtInvoke.ControlTag(btnOpen, false);
				} else {
					mSerial = new CtSerial(mDataType) { SubscribeSendEvent = true };
					mSerial.OnSerialEvents += mSerial_OnSerialEvents;
					CtSerialSetup setup = new CtSerialSetup();
					Stat stt = setup.Start(ref mSerial);
					setup.Dispose();
					if (stt == Stat.SUCCESS) {
						CtInvoke.ControlEnabled(cbDataType, false);
						CtInvoke.ControlTag(btnOpen, true);
					}
				}
			} catch (Exception ex) {
				ShowData(ex.Message);
			}
		}

		void mSerial_OnSerialEvents(object sender, SerialEventArgs e) {
			switch (e.Event) {
				case SerialPortEvents.Connection:
					CtInvoke.PictureBoxImage(pbConnectStt, ((bool)e.Value) ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
					CtInvoke.ControlText(btnOpen, ((bool)e.Value) ? "Close" : "Open");
					CtInvoke.ControlEnabled(btnSend, (bool)e.Value);
					CtInvoke.ControlEnabled(cbNewLine, (bool)e.Value);
					//CtInvoke.ControlEnabled(chkManual, (bool)e.Value);
					CtInvoke.ControlEnabled(txtSend, (bool)e.Value);
					CtInvoke.ControlText(cbPorts, mSerial.PortName);
					break;
				case SerialPortEvents.DataReceived:
					if (mDataType == TransDataFormats.String)
						ShowData("[RX] " + e.Value.ToString().Replace(CtConst.NewLine, ""));
					else ShowData("[RX] " + CtConvert.CStr(e.Value as List<byte>, mDataFormat));
					break;
				case SerialPortEvents.Error:
					ShowData(e.Value.ToString().Replace(CtConst.NewLine, ""));
					break;
				case SerialPortEvents.DataSend:
					if (e.Value is string) ShowData("[TX] " + e.Value.ToString().Replace(CtConst.NewLine, ""));
					else ShowData("[TX] " + CtConvert.CStr(e.Value as byte[], mDataFormat));
					break;
			}
		}

		private void button2_Click(object sender, EventArgs e) {
			if (mDataType == TransDataFormats.String) {
				string strTemp = txtSend.Text;

				switch (cbNewLine.SelectedIndex) {
					case 1:
						strTemp += CtConst.NewLine;
						break;
					case 2:
						strTemp += CtConst.Cr;
						break;
					case 3:
						strTemp += CtConst.Lf;
						break;
				}

				mSerial.Send(strTemp, EndChar.None);
			} else {
				List<string> strSplit = txtSend.Text.Split(CtConst.CHR_SEPERATOR, StringSplitOptions.RemoveEmptyEntries).ToList();
				if (strSplit != null && strSplit.Count > 0) {
					List<byte> byteTemp = strSplit.ConvertAll(new Converter<string, byte>(val => Convert.ToByte(val, (int)mDataFormat)));
					mSerial.Send(byteTemp);
				}
			}
		}

		private void button3_Click(object sender, EventArgs e) {
			string strTemp = "";
			mSerial.Receive(out strTemp);
			ShowData(strTemp.Replace(CtConst.NewLine, ""));
		}

		private void Serial_Load(object sender, EventArgs e) {
			if (mSerial != null) {
				foreach (string item in CtSerial.GetPortNames()) {
					CtInvoke.ComboBoxAdd(cbPorts, item);
				}
				if (cbPorts.Items.Count > 0) CtInvoke.ComboBoxSelectedIndex(cbPorts, 0);
			}
		}

		private void chkManual_CheckedChanged(object sender, EventArgs e) {
			//CtInvoke.ControlEnabled(btnReceive, chkManual.Checked);
			//mSerial.EnableReceiveEvent = !chkManual.Checked;
		}

		private void cbNewLine_SelectedIndexChanged(object sender, EventArgs e) {
			if (cbNewLine.SelectedIndex == 0) mDataFormat = NumericFormats.Hexadecimal;
			else mDataFormat = NumericFormats.Decimal;
		}

		private void cbDataType_SelectedIndexChanged(object sender, EventArgs e) {
			if (cbDataType.SelectedIndex > -1) {
				mDataType = (TransDataFormats)cbDataType.SelectedIndex;

				if (mDataType == TransDataFormats.String) {
					CtInvoke.ComboBoxClear(cbNewLine);
					CtInvoke.ComboBoxAdd(cbNewLine, new object[] { "None", "CrLf", "Cr", "Lf" });
				} else {
					CtInvoke.ComboBoxClear(cbNewLine);
					CtInvoke.ComboBoxAdd(cbNewLine, new object[] { "Hex", "Dec" });
				}
				CtInvoke.ComboBoxSelectedIndex(cbNewLine, 0);
			}
		}

		private void miClrMsg_Click(object sender, EventArgs e) {
			CtInvoke.ListBoxClear(lsbxMsg);
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

		#endregion
	}
}
