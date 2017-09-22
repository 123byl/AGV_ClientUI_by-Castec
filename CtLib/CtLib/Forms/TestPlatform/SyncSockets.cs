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
using CtLib.Module.TCPIP;

namespace CtLib.Forms.TestPlatform {

    /// <summary>[測試介面] Socket</summary>
    public partial class Test_Sockets : Form {

        #region Declaration - Members
        private CtSyncSocket mSocket;
        private TransmissionDataFormats mDataType = TransmissionDataFormats.STRING;
        private NumericFormats mDataFormat = NumericFormats.DECIMAL;
        #endregion

        #region Function - Constructors
        /// <summary>開啟 Socket 測試介面</summary>
        public Test_Sockets() {
            InitializeComponent();

            CtInvoke.ComboBoxSelectedIndex(cbMode, 0);
            CtInvoke.ComboBoxSelectedIndex(cbEndLine, 0);
            CtInvoke.ComboBoxSelectedIndex(cbDataType, 0);
        }
        #endregion

        #region Function - Methods
        private void ShowMessage(string data) {
            string strTemp = "[" + DateTime.Now.ToString("HH:mm:ss.ff") + "] " + data + CtConst.NewLine;
            CtInvoke.TextBoxText(txtReceive, strTemp, true, true);
        }

        private void ShowMessage(DateTime time, string data) {
            string strTemp = "[" + time.ToString("HH:mm:ss.ff") + "] " + data + CtConst.NewLine;
            CtInvoke.TextBoxText(txtReceive, strTemp, true, true);
        }
        #endregion

        #region Function - CtSyncSocket Events
        void mSocket_OnSocketEvents(object sender, CtSyncSocket.SocketEventArgs e) {
            switch (e.Event) {
                case CtSyncSocket.SocketEvents.CONNECTED_WITH_SERVER:
                    CtSyncSocket.ConnectInfo infoClient = e.Value as CtSyncSocket.ConnectInfo;
                    ShowMessage(infoClient.Time, (infoClient.Status) ? "已連上Server，IP = " + infoClient.IP + "  Port = " + infoClient.Port.ToString() : "已中斷連線");

                    CtInvoke.ButtonText(btnConnect, (infoClient.Status) ? "Disconnect" : "Connect");
                    CtInvoke.ButtonTag(btnConnect, infoClient.Status);
                    CtInvoke.PictureBoxImage(pbStt, (infoClient.Status) ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
                    CtInvoke.ComboBoxEnable(cbMode, !infoClient.Status);

                    CtInvoke.ButtonEnable(btnSend, infoClient.Status);
                    CtInvoke.ComboBoxEnable(cbEndLine, infoClient.Status);
                    CtInvoke.TextBoxEnable(txtSend, infoClient.Status);
                    CtInvoke.ComboBoxEnable(cbDataType, !infoClient.Status);
                    break;

                case CtSyncSocket.SocketEvents.CLIENT_CONNECTED:
                    CtSyncSocket.ConnectInfo infoSrv = e.Value as CtSyncSocket.ConnectInfo;
                    ShowMessage(infoSrv.Time, (infoSrv.Status) ? "有Client連接，IP = " + infoSrv.IP + "  Port = " + infoSrv.Port.ToString() : "Client中斷連線，IP = " + infoSrv.IP + "  Port = " + infoSrv.Port.ToString());

                    CtInvoke.LabelText(lbSrvCount, "Client Count: " + mSocket.ClientCount.ToString());

                    CtInvoke.ButtonEnable(btnSend, infoSrv.Status);
                    CtInvoke.ComboBoxEnable(cbEndLine, infoSrv.Status);
                    CtInvoke.TextBoxEnable(txtSend, infoSrv.Status);
                    break;

                case CtSyncSocket.SocketEvents.EXCEPTION:
                    ShowMessage(e.Value.ToString());
                    break;

                case CtSyncSocket.SocketEvents.DATA_RECEIVED:
                    CtSyncSocket.DataInfo data = e.Value as CtSyncSocket.DataInfo;
                    if (mSocket.Mode == SocketModes.CLIENT)
                        if (mDataType == TransmissionDataFormats.STRING) ShowMessage(data.Time, data.Data.ToString());
                        else ShowMessage(data.Time, CtConvert.CStr(data.Data as List<byte>, mDataFormat));
                    else
                        if (mDataType == TransmissionDataFormats.STRING) ShowMessage(data.Time, "[From " + data.IP + ":" + data.Port.ToString() + "] " + data.Data);
                        else ShowMessage(data.Time, "[From " + data.IP + ":" + data.Port.ToString() + "] " + CtConvert.CStr(data.Data as List<byte>, mDataFormat));
                    break;

                case CtSyncSocket.SocketEvents.LISTEN:
                    CtSyncSocket.ConnectInfo infoListen = e.Value as CtSyncSocket.ConnectInfo;
                    ShowMessage(infoListen.Time, (infoListen.Status) ? "開始監聽" : "停止監聽");

                    CtInvoke.ButtonText(btnConnect, (infoListen.Status) ? "Stop" : "Start");
                    CtInvoke.ButtonTag(btnConnect, infoListen.Status);
                    CtInvoke.PictureBoxImage(pbStt, (infoListen.Status) ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
                    CtInvoke.ComboBoxEnable(cbMode, !infoListen.Status);
                    CtInvoke.LabelText(lbSrvCount, "Client Count: " + mSocket.ClientCount.ToString());

                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Function - Interface Events
        private void cbMode_SelectedIndexChanged(object sender, EventArgs e) {
            if (cbMode.SelectedIndex == 0) {
                CtInvoke.TextBoxEnable(txtIP, true);
                CtInvoke.LabelVisible(lbSrvCount, false);
                CtInvoke.ButtonText(btnConnect, "Connect");
                CtInvoke.ButtonTag(btnConnect, false);
            } else {
                CtInvoke.TextBoxEnable(txtIP, false);
                CtInvoke.TextBoxText(txtIP, "127.0.0.1");
                CtInvoke.LabelVisible(lbSrvCount, true);
                CtInvoke.ButtonText(btnConnect, "Start");
                CtInvoke.ButtonTag(btnConnect, false);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e) {

            try {
                if (CtConvert.CBool(btnConnect.Tag)) {
                    if (cbMode.SelectedIndex == 0) {
                        mSocket.ClientDisconnect();
                    } else {
                        mSocket.ServerStopListen();
                    }
                    mSocket.OnSocketEvents -= mSocket_OnSocketEvents;
                    CtInvoke.ComboBoxEnable(cbDataType, true);
                } else {
                    mSocket = new CtSyncSocket(mDataType);
                    mSocket.OnSocketEvents += mSocket_OnSocketEvents;
                    if (cbMode.SelectedIndex == 0) {
                        mSocket.ClientConnect(txtIP.Text, CtConvert.CInt(txtPort.Text));
                    } else {
                        mSocket.ServerListen(CtConvert.CInt(txtPort.Text));
                    }
                    CtInvoke.ComboBoxEnable(cbDataType, false);
                }
            } catch (Exception ex) {
                ShowMessage(ex.Message);
            }
        }

        private void btnSend_Click(object sender, EventArgs e) {
            if (mDataType == TransmissionDataFormats.STRING) {
                string strTemp = txtSend.Text;

                switch (cbEndLine.SelectedIndex) {
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

                mSocket.Send(strTemp);
            } else {
                List<string> strSplit = txtSend.Text.Split(CtConst.CHR_SEPERATOR, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (strSplit != null && strSplit.Count > 0) {
                    List<byte> byteTemp = strSplit.ConvertAll(new Converter<string, byte>(val => Convert.ToByte(val, (int)mDataFormat)));
                    mSocket.Send(byteTemp.ToArray());
                }
            }
        }
        #endregion

        private void cbDataType_SelectedIndexChanged(object sender, EventArgs e) {
            if (cbDataType.SelectedIndex > -1) {
                mDataType = (TransmissionDataFormats)cbDataType.SelectedIndex;

                if (mDataType == TransmissionDataFormats.STRING) {
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
            if (cbEndLine.SelectedIndex == 0) mDataFormat = NumericFormats.HEXADECIMAL;
            else mDataFormat = NumericFormats.DECIMAL;
        }

        private void button1_Click(object sender, EventArgs e) {
            List<string> temp = mSocket.GetClientAddress();
            foreach (string item in temp) {
                string[] split = item.Split(CtConst.CHR_COLON, StringSplitOptions.RemoveEmptyEntries);
                mSocket.ServerStopListen(split[0], CtConvert.CInt(split[1]));
            }
        }

    }
}
