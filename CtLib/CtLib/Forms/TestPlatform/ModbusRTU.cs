using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Modbus;
using CtLib.Module.SerialPort;

namespace CtLib.Forms.TestPlatform {
    /// <summary>Modbus RTU 測試介面</summary>
    public partial class Test_ModbusRTU : Form {

        private CtModbus_RTU mRTU;

        /// <summary>建立 Modbus RTU 測試介面</summary>
        public Test_ModbusRTU() {
            InitializeComponent();

            mRTU = new CtModbus_RTU();
            mRTU.OnSerialEvents += mRTU_OnSerialEvents;

            CtInvoke.ComboBoxSelectedIndex(cbFC, 0);
        }

        void ShowMessage(string msg) {
            if (lstBxMsg.Items.Count > 199) {
                CtInvoke.ListBoxClear(lstBxMsg);
            }
            CtInvoke.ListBoxInsert(lstBxMsg, 0, DateTime.Now.ToString("[HH:mm:ss.ff] ") + msg);
        }

        void mRTU_OnSerialEvents(object sender, SerialEventArgs e) {
            switch (e.Event) {
                case SerialPortEvents.Connection:
                    ShowMessage(((bool)e.Value) ? "連線成功" : "中斷連線");
                    break;
                case SerialPortEvents.DataReceived:
                    break;
                case SerialPortEvents.Error:
                    ShowMessage(e.Value.ToString());
                    break;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e) {
            if (mRTU.IsOpen) {
                mRTU.Close();
                CtInvoke.ControlText(btnConnect, "Connect");
            } else {
                mRTU.Open();
                CtInvoke.ControlText(btnConnect, "Disconnect");
            }
        }

        private void btnSend_Click(object sender, EventArgs e) {
            switch (cbFC.SelectedIndex) {
                case 0: /* FC01 */
                    List<bool> bolFC01;
                    mRTU.ReadCoilStatus(CtConvert.CUShort(txtAddr.Text), CtConvert.CUShort(nudCount.Value), out bolFC01);
                    if (bolFC01 != null && bolFC01.Count > 0) {
                        string strTemp = "FC01 > ";
                        foreach (bool item in bolFC01) {
                            strTemp += ((item) ? "ON" : "OFF") + " ";
                        }
                        ShowMessage(strTemp);
                    }
                    break;
                case 1: /* FC02 */
                    List<bool> bolFC02;
                    mRTU.ReadInputStatus(CtConvert.CUShort(txtAddr.Text), CtConvert.CUShort(nudCount.Value), out bolFC02);
                    if (bolFC02 != null && bolFC02.Count > 0) {
                        string strTemp = "FC02 > ";
                        foreach (bool item in bolFC02) {
                            strTemp += ((item) ? "ON" : "OFF") + " ";
                        }
                        ShowMessage(strTemp);
                    }
                    break;
                case 2: /* FC03 */
                    List<byte> bolFC03;
                    mRTU.ReadHoldingRegister(CtConvert.CUShort(txtAddr.Text), CtConvert.CUShort(nudCount.Value), out bolFC03);
                    if (bolFC03 != null && bolFC03.Count > 0) {
                        string strTemp = "FC03 > ";
                        foreach (byte item in bolFC03) {
                            strTemp += CtConvert.ToHex(item) + " ";
                        }
                        ShowMessage(strTemp);
                    }
                    break;
                case 3: /* FC04 */
                    List<byte> bolFC04;
                    mRTU.ReadInputRegisters(CtConvert.CUShort(txtAddr.Text), CtConvert.CUShort(nudCount.Value), out bolFC04);
                    if (bolFC04 != null && bolFC04.Count > 0) {
                        string strTemp = "FC04 > ";
                        foreach (byte item in bolFC04) {
                            strTemp += CtConvert.ToHex(item) + " ";
                        }
                        ShowMessage(strTemp);
                    }
                    break;
                case 4: /* FC05 */
                    List<byte> valFC05;
                    mRTU.WriteSingleCoil(CtConvert.CUShort(txtAddr.Text), CtConvert.CBool(cbBolVal.SelectedIndex), out valFC05);
                    if (valFC05 != null && valFC05.Count > 0) {
                        string strTemp = "FC05 > ";
                        foreach (byte item in valFC05) {
                            strTemp += CtConvert.ToHex(item) + " ";
                        }
                        ShowMessage(strTemp);
                    }
                    break;
                case 5: /* FC06 */
                    List<byte> valFC06;
                    mRTU.WriteSingleRegister(CtConvert.CUShort(txtAddr.Text), CtConvert.CUShort(txtData.Text), out valFC06);
                    if (valFC06 != null && valFC06.Count > 0) {
                        string strTemp = "FC06 > ";
                        foreach (byte item in valFC06) {
                            strTemp += CtConvert.ToHex(item) + " ";
                        }
                        ShowMessage(strTemp);
                    }
                    break;
                case 6: /* FC15 */
                    List<byte> valFC15;
                    mRTU.WriteMultiCoils(CtConvert.CUShort(txtAddr.Text), new List<bool> { false, false, false }, out valFC15);
                    if (valFC15 != null && valFC15.Count > 0) {
                        string strTemp = "FC15 > ";
                        foreach (byte item in valFC15) {
                            strTemp += CtConvert.ToHex(item) + " ";
                        }
                        ShowMessage(strTemp);
                    }
                    break;
                case 7: /* FC16 */
                    List<string> strData = txtData.Text.Replace(" ", ",").Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries).ToList();
                    List<ushort> val = strData.ConvertAll(new Converter<string, ushort>(data => CtConvert.CUShort(data)));
                    List<byte> valFC16;
                    mRTU.WriteMultiRegisters(CtConvert.CUShort(txtAddr.Text), val, out valFC16);
                    if (valFC16 != null && valFC16.Count > 0) {
                        string strTemp = "FC16 > ";
                        foreach (byte item in valFC16) {
                            strTemp += CtConvert.ToHex(item) + " ";
                        }
                        ShowMessage(strTemp);
                    }
                    break;
                default:
                    break;
            }
        }

        private void btnCRC_Click(object sender, EventArgs e) {
            List<string> strTemp = txtCRC.Text.Replace(" ", ",").Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<byte> crcOri = strTemp.ConvertAll(new Converter<string, byte>(val => (byte)CtConvert.ToInteger(val, NumericFormats.Hexadecimal)));
            List<byte> crcVal = mRTU.CRC(crcOri);
            if (crcVal != null && crcVal.Count > 0) {
                string strCRC = "";
                foreach (byte bytVal in crcVal) {
                    strCRC += CtConvert.ToHex(bytVal, 2) + " ";
                }
                ShowMessage(strCRC);
            }
        }

        private void txtData_TextChanged(object sender, EventArgs e) {

        }

        private void cbFC_SelectedIndexChanged(object sender, EventArgs e) {
            switch (cbFC.SelectedIndex) {
                case 0: /* FC01 */
                    CtInvoke.ControlVisible(cbBolVal, false);
                    CtInvoke.ControlVisible(txtData, false);
                    CtInvoke.ControlVisible(lbHex, false);
                    break;
                case 1: /* FC02 */
                    CtInvoke.ControlVisible(cbBolVal, false);
                    CtInvoke.ControlVisible(txtData, false);
                    CtInvoke.ControlVisible(lbHex, false);
                    break;
                case 2: /* FC03 */
                    CtInvoke.ControlVisible(cbBolVal, false);
                    CtInvoke.ControlVisible(txtData, false);
                    CtInvoke.ControlVisible(lbHex, false);
                    break;
                case 3: /* FC04 */
                    CtInvoke.ControlVisible(cbBolVal, false);
                    CtInvoke.ControlVisible(txtData, false);
                    CtInvoke.ControlVisible(lbHex,false);
                    break;
                case 4: /* FC05 */
                    CtInvoke.ControlVisible(cbBolVal, true);
                    CtInvoke.ControlVisible(txtData, false);
                    CtInvoke.ControlVisible(lbHex, false);
                    break;
                case 5: /* FC06 */
                    CtInvoke.ControlVisible(cbBolVal, false);
                    CtInvoke.ControlVisible(txtData, true);
                    CtInvoke.ControlVisible(lbHex, true);
                    break;
                case 6: /* FC15 */
                    CtInvoke.ControlVisible(cbBolVal, false);
                    CtInvoke.ControlVisible(txtData, true);
                    CtInvoke.ControlVisible(lbHex, true);
                    break;
                case 7: /* FC16 */
                    CtInvoke.ControlVisible(cbBolVal, false);
                    CtInvoke.ControlVisible(txtData, true);
                    CtInvoke.ControlVisible(lbHex, true);
                    break;
                default:
                    break;
            }
        }
    }
}
