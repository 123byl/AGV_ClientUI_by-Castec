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

        #region Declaration - Members
        private CtSerial mSerial;
        private TransmissionDataFormats mDataType = TransmissionDataFormats.STRING;
        private NumericFormats mDataFormat = NumericFormats.DECIMAL;
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
            string strTemp = "[" + DateTime.Now.ToString("HH:mm:ss.ff") + "] " + data + CtConst.NewLine;
            CtInvoke.TextBoxText(txtReceive, strTemp, true, true);
        }
        #endregion

        #region Function - Interface Events
        private void button1_Click(object sender, EventArgs e) {
            try {
                if (CtConvert.CBool(btnOpen.Tag)) {
                    //CtInvoke.CheckBoxChecked(chkManual, false);
                    mSerial.Close();
                    mSerial.OnSerialEvents -= mSerial_OnSerialEvents;
                    CtInvoke.ComboBoxEnable(cbDataType, true);
                    CtInvoke.ButtonTag(btnOpen, false);
                } else {
                    mSerial = new CtSerial(mDataType);
                    mSerial.OnSerialEvents += mSerial_OnSerialEvents;
                    CtSerialSetup setup = new CtSerialSetup();
                    Stat stt = setup.Start(ref mSerial);
                    setup.Dispose();
                    if (stt == Stat.SUCCESS) {
                        CtInvoke.ComboBoxEnable(cbDataType, false);
                        CtInvoke.ButtonTag(btnOpen, true);
                    }
                }
            } catch (Exception ex) {
                ShowData(ex.Message);
            }
        }

        void mSerial_OnSerialEvents(object sender, CtSerial.SerialEventArgs e) {
            switch (e.Event) {
                case CtSerial.SerialPortEvents.CONNECTION:
                    CtInvoke.PictureBoxImage(pbConnectStt, ((bool)e.Value) ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
                    CtInvoke.ButtonText(btnOpen, ((bool)e.Value) ? "Close" : "Open");
                    CtInvoke.ButtonEnable(btnSend, (bool)e.Value);
                    CtInvoke.ComboBoxEnable(cbNewLine, (bool)e.Value);
                    //CtInvoke.CheckBoxEnable(chkManual, (bool)e.Value);
                    CtInvoke.TextBoxEnable(txtSend, (bool)e.Value);
                    CtInvoke.ComboBoxText(cbPorts, mSerial.PortName);
                    break;
                case CtSerial.SerialPortEvents.DATA_RECEIVED:
                    if (mDataType == TransmissionDataFormats.STRING)
                        ShowData(e.Value.ToString().Replace(CtConst.NewLine, ""));
                    else ShowData(CtConvert.CStr(e.Value as List<byte>, mDataFormat));
                    break;
                case CtSerial.SerialPortEvents.ERROR:
                    ShowData(e.Value.ToString().Replace(CtConst.NewLine, ""));
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            if (mDataType == TransmissionDataFormats.STRING) {
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

                mSerial.Send(strTemp, EndChar.NONE);
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
                foreach (string item in mSerial.GetPortNames()) {
                    CtInvoke.ComboBoxAdd(cbPorts, item);
                }
                if (cbPorts.Items.Count > 0) CtInvoke.ComboBoxSelectedIndex(cbPorts, 0);
            }
        }

        private void chkManual_CheckedChanged(object sender, EventArgs e) {
            //CtInvoke.ButtonEnable(btnReceive, chkManual.Checked);
            //mSerial.EnableReceiveEvent = !chkManual.Checked;
        }

        #endregion

        private void cbNewLine_SelectedIndexChanged(object sender, EventArgs e) {
            if (cbNewLine.SelectedIndex == 0) mDataFormat = NumericFormats.HEXADECIMAL;
            else mDataFormat = NumericFormats.DECIMAL;
        }

        private void cbDataType_SelectedIndexChanged(object sender, EventArgs e) {
            if (cbDataType.SelectedIndex > -1) {
                mDataType = (TransmissionDataFormats)cbDataType.SelectedIndex;

                if (mDataType == TransmissionDataFormats.STRING) {
                    CtInvoke.ComboBoxClear(cbNewLine);
                    CtInvoke.ComboBoxAdd(cbNewLine, new object[] { "None", "CrLf", "Cr", "Lf" });
                } else {
                    CtInvoke.ComboBoxClear(cbNewLine);
                    CtInvoke.ComboBoxAdd(cbNewLine, new object[] { "Hex", "Dec" });
                }
                CtInvoke.ComboBoxSelectedIndex(cbNewLine, 0);
            }
        }


    }
}
