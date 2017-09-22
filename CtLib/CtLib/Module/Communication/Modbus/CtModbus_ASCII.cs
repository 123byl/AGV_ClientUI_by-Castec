using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Text;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.SerialPort;
using CtLib.Module.Ultity;

namespace CtLib.Module.Modbus {

    /// <summary>
    /// Modbus 控制，使用 ASCII
    /// <para>命令以 ASCII 文字傳送，具有起始與結尾符號</para>
    /// </summary>
    public class CtModbus_ASCII : CtSerial, ICtModbus {

        #region Version

        /// <summary>CtModbus_ASCII Version Information</summary>
        /// <remarks><code>
        /// 0.0.0  Alva [2014/12/22]
        ///     + 建立 IAI Robo Cylinder 測試平台，使用 Modbus ASCII 方法
        ///     
        /// 1.0.0  Ahern [2014/12/28]
        ///     + 將測試平台整理至 CtModbus_ASCII
        ///     
        /// 1.1.0  Ahern [2015/02/17]
        ///     + 繼承 ICtModbus
        ///     + 繼承 CtSerial
        ///     + Read 相關 Function 以符合 ICtModbus
        ///     \ 修改部分 Function 以 Override CtSerial
        ///     \ Receive 部分改以 .Net 4.0 之 Task 實作，尚未測試!
        ///     
        /// </code></remarks>
        public static readonly new CtVersion @Version = new CtVersion(1, 0, 0, "2014/12/28", "Ahern Kuo");

        #endregion

        #region Declaration - Definitions
        /// <summary>等待命令回傳的逾時時間</summary>
        private static readonly int PROTOCOL_TIMEOUT = 1000;

        /// <summary>Modbus ASCII 起始文字</summary>
        private static readonly string MODBUS_START_OF_FRAME = ":";
        /// <summary>Modbus ASCII 結尾符號</summary>
        private static readonly string MODBUS_END_OF_FRAME = "\r\n";
        /// <summary>預設的 Device ID。或稱 Master / Slave ID</summary>
        private static readonly byte MODBUS_DEVICE_ID = 0x01;
        #endregion

        #region Declaration - Members
        /// <summary>暫存已接收的資料</summary>
        private List<byte> mRxData = new List<byte>();
        /// <summary>[Flag] 是否接收到資料</summary>
        private bool mFlag_RxData;
        /// <summary>當前最後收到的命令解碼狀態</summary>
        private Stat mRxStt = Stat.SUCCESS;
        #endregion

        #region Declaration - Properties
        /// <summary>取得或設定 Device ID</summary>
        public byte DeviceID { get; set; }
        #endregion

        #region Function - Constructors
        /// <summary>建立 CtModbus_ASCII，並採用預設的 Device ID</summary>
        public CtModbus_ASCII() {
            DeviceID = MODBUS_DEVICE_ID;
            DataFormat = TransmissionDataFormats.STRING;
        }

        /// <summary>建立 CtModbus_ASCII</summary>
        /// <param name="devID">Device ID</param>
        public CtModbus_ASCII(byte devID) {
            DeviceID = devID;
            DataFormat = TransmissionDataFormats.STRING;
        }
        #endregion

        #region Function - CtSerial Override
        /// <summary>SerialPort 資料接收函式</summary>
        /// <param name="sender">SerialPort</param>
        /// <param name="e">EventArgs</param>
        protected override void mCom_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e) {
            /*-- If enable events, do the operations --*/
            if (EnableReceiveEvent) {

                /*-- Delay, wait buffer fill finish --*/
                Thread.Sleep(1);

                /*-- Receive data by new task --*/
                CtThread.AddWorkItem(
                    obj => {
                        string strTemp;
                        Receive(out strTemp);
                        if (strTemp != "") {
                            mRxStt = DecodeData(strTemp);
                            mFlag_RxData = true;
                        }
                    }
                );
            }
        }
        #endregion

        #region Function - CtSerial Operations

        /// <summary>透過 SerialPort 傳送資料至裝置</summary>
        /// <param name="cmd">命令字串</param>
        /// <param name="endOfLine">是否含需換行符號</param>
        public override void Send(string cmd, EndChar endOfLine = EndChar.NONE) {
            /*-- Clear flag --*/
            mFlag_RxData = false;

            /*-- Check mCom state --*/
            if (!mCom.IsOpen) throw (new Exception("Please open serial port first"));

            /*-- If it need new line, add it! --*/
            string strData = cmd;
            switch (endOfLine) {
                case EndChar.CR:
                    strData += "\r";
                    break;
                case EndChar.LF:
                    strData += "\n";
                    break;
                case EndChar.CRLF:
                    strData += "\r\n";
                    break;
                case EndChar.CUSTOM:
                    strData += mEOLStr;
                    break;
            }

            /*-- Convert string to byte[] with each char --*/
            byte[] temp = Encoding.ASCII.GetBytes(strData);

            /*-- Write the byte[] into buffer --*/
            mCom.Write(temp, 0, temp.Length);
        }

        /// <summary>透過 SerialPort 傳送資料至裝置，並等待回傳資料</summary>
        /// <param name="cmd">命令字串</param>
        /// <param name="result">收到回傳的資料</param>
        public void Send(string cmd, out List<byte> result) {
            List<byte> retTemp = null;
            mFlag_RxData = false;
            Send(cmd, EndChar.NONE);
            if (WaitResponse()) {
                retTemp = mRxData.ToList();
            }
            result = retTemp;
        }

        #endregion

        #region Function - Methods
        /// <summary>等待訊息回應 (利用無限迴圈等待Flag變化)</summary>
        /// <returns>(True)收到資料 (False)無資料或逾時</returns>
        private bool WaitResponse() {
            bool result = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            do {
                Thread.Sleep(10);
                Application.DoEvents();
            } while (!mFlag_RxData && sw.ElapsedMilliseconds < PROTOCOL_TIMEOUT);
            sw.Stop();

            if (mFlag_RxData) result = true;

            return result;
        }

        /// <summary>訊息解碼</summary>
        /// <param name="data">從特定裝置收到的字串訊息</param>
        private Stat DecodeData(string data) {
            Stat stt = Stat.SUCCESS;
            mRxData.Clear();
            if (data.StartsWith(MODBUS_START_OF_FRAME + CtConvert.ToHex(DeviceID, 2))) {
                string strTemp = data.Replace(MODBUS_START_OF_FRAME, "").Replace(CtConst.NewLine, "");

                ModbusFunction func = (ModbusFunction)CtConvert.CByte(strTemp.Substring(2, 2));

                if (func >= ModbusFunction.EXCEPTION) {
                    mRxData.Add(Convert.ToByte(strTemp.Substring(4, 2), 16));
                    stt = Stat.ER3_MB_SLVERR;
                } else if (func >= ModbusFunction.FORCE_SINGLE_COIL) {
                    mRxData.Add(Convert.ToByte(strTemp.Substring(8, 2), 16));
                    mRxData.Add(Convert.ToByte(strTemp.Substring(10, 2), 16));
                } else {
                    byte dataLength = CtConvert.CByte(strTemp.Substring(4, 2));
                    for (int idx = 6; idx < strTemp.Length - 5; idx += 2) {
                        mRxData.Add(Convert.ToByte(strTemp.Substring(idx, 2), 16));
                    }
                }
            } else stt = Stat.ER_SYSTEM;

            return stt;
        }

        /// <summary>Longitudinal Redundancy Check</summary>
        /// <param name="command">尚未加上檢查碼之命令字串</param>
        /// <returns>計算完畢之 1Byte LRC Hex 字串</returns>
        public string LRC(string command) {
            string strTemp = command.Replace("\r", "").Replace("\n", "").Replace(":", "");
            byte sum = 0;
            try {
                /*-- 將所有 Byte 相加 --*/
                for (int i = 0; i < strTemp.Length; i += 2) {
                    sum += Convert.ToByte(strTemp.Substring(i, 2), 16);
                }

                /*-- 取二的補數。可用 0x100 相減，或是 XOR 0xFFFF --*/
                sum = (byte)(0x100 - sum);
                strTemp = CtConvert.ToHex(sum, 2);
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
            return strTemp;
        }

        /// <summary>建立讀取方法之命令字串</summary>
        /// <param name="devID">Device ID</param>
        /// <param name="addr">起始 IO 或 Register 位置</param>
        /// <param name="length">連續讀取的長度。 如從 9000 讀取至 9004 則輸入 5</param>
        /// <param name="func">Modbus 功能碼</param>
        /// <returns>加入起始與結尾符號之命令字串，可供直接寫入使用</returns>
        private string CreateReadHeader(byte devID, ushort addr, ushort length, ModbusFunction func) {
            string strCMD = "";
            strCMD += CtConvert.ToHex(devID, 2);
            strCMD += CtConvert.ToHex((byte)func, 2);
            strCMD += CtConvert.ToHex(addr, 4);
            strCMD += CtConvert.ToHex(length, 4);
            strCMD += LRC(strCMD);
            strCMD = MODBUS_START_OF_FRAME + strCMD;
            strCMD += MODBUS_END_OF_FRAME;
            return strCMD;
        }

        /// <summary>建立寫入方法之命令字串</summary>
        /// <param name="devID">Device ID</param>
        /// <param name="addr">起始 IO 或 Register 位置</param>
        /// <param name="count">IO 或 Register 數量。 如欲寫入 9000 至 9004 則輸入 5</param>
        /// <param name="dataLength">接續的資料長度，以 byte 數量為單位。 如 9000~9004 每個 Register 寫入 2byte，則此處帶入 10</param>
        /// <param name="data">欲寫入的資料內容</param>
        /// <param name="func">Modbus 功能碼</param>
        /// <returns>加入起始與結尾符號之命令字串，可供直接寫入使用</returns>
        private string CreateWriteHeader(byte devID, ushort addr, ushort count, ushort dataLength, List<byte> data, ModbusFunction func) {
            string strCMD = "";
            strCMD += CtConvert.ToHex(devID, 2); ;
            strCMD += CtConvert.ToHex((byte)func, 2);
            strCMD += CtConvert.ToHex(addr, 4);
            if (count > 0) strCMD += CtConvert.ToHex(count, 4);
            if (dataLength > 0) strCMD += CtConvert.ToHex(dataLength, 2);
            foreach (byte val in data) {
                strCMD += CtConvert.ToHex(val, 2);
            }
            strCMD += LRC(strCMD);
            strCMD = MODBUS_START_OF_FRAME + strCMD;
            strCMD += MODBUS_END_OF_FRAME;
            return strCMD;
        }

        private void ConvertDataToBool(ushort count, out List<bool> bolVal) {
            byte maxCount = 0;
            byte valTemp = 0;
            List<bool> bRet = new List<bool>();
            for (byte idx = 0; idx < mRxData.Count; idx++) {
                valTemp = mRxData[idx];
                maxCount = (byte)(((idx * 8) < count) ? 8 : count % 8);
                for (byte bit = 0; bit < maxCount; bit++) {
                    bRet.Add(((valTemp & 0x01) == 1) ? true : false);
                    valTemp >>= 1;
                }
            }
            bolVal = bRet;
        }

        private void ConvertBoolToData(List<bool> bolVal, out List<byte> bytVal) {
            List<byte> valTemp = new List<byte>();
            int maxCount = 0;
            byte bytTemp = 0;
            if (bolVal != null && bolVal.Count > 0) {
                for (int idx = 0; idx < bolVal.Count; idx += 8) {
                    bytTemp = 0;
                    maxCount = (((idx + 1) * 8) < bolVal.Count) ? (idx + 1) * 8 : bolVal.Count;
                    for (int bit = idx * 8; bit < maxCount; bit++) {
                        bytTemp <<= 1;
                        bytTemp += (byte)((bolVal[idx]) ? 1 : 0);
                    }
                    valTemp.Add(bytTemp);
                }
            }
            bytVal = valTemp;
        }
        #endregion

        #region Function - Core
        ///// <summary>讀取裝置特定 IO 或 Register</summary>
        ///// <param name="addr">起始 IO 或 Register 位址</param>
        ///// <param name="length">欲連續讀取的長度。 如 9000 讀取至 9004 則帶入 5</param>
        ///// <param name="func">Modbus 功能碼</param>
        ///// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        ///// <returns>Status Code</returns>
        //public Stat ReadData(string addr, ushort length, ModbusFunction func, out List<byte> result) {
        //    Stat stt = Stat.SUCCESS;
        //    string strCMD = CreateReadHeader(mSlaveID, addr, length, func);
        //    stt = SendData(strCMD);
        //    result = (WaitResponse()) ? mRxData.ToList() : null;
        //    return stt;
        //}

        /// <summary>[FC01] 讀取 Coil (InOut, Output) 狀態</summary>
        /// <param name="addr">IO Address</param>
        /// <param name="coilCount">連續讀取的數量。如欲 0~7 則輸入 8 </param>
        /// <param name="state">各點的狀態  (True)ON  (False)OFF</param>
        /// <returns>Status Code</returns>
        public Stat ReadCoilStatus(ushort addr, ushort coilCount, out List<bool> state) {
            Stat stt = Stat.SUCCESS;
            List<bool> boolResult = null;
            string cmd = CreateReadHeader(DeviceID, addr, coilCount, ModbusFunction.READ_COIL_STATUS);
            Send(cmd, EndChar.NONE);
            if (WaitResponse()) {
                ConvertDataToBool(coilCount, out boolResult);
                stt = mRxStt;
            } else stt = Stat.ER_SYSTEM;

            state = boolResult;
            return stt;
        }

        /// <summary>[FC03] 讀取暫存器(Register)數值，可為輸入或輸出之暫存器(Input, Output, InOut)</summary>
        /// <param name="addr">Register Address</param>
        /// <param name="regCount">連續讀取的暫存器數量。如 4000~4007 則輸入 8</param>
        /// <param name="value">(True)ON  (False)OFF</param>
        /// <returns>Status Code</returns>
        /// <remarks>以 Wago IO 為例，一個 Register 都是 16bit，所以回傳的資訊以 ushort (uint16) 為主</remarks>
        public Stat ReadHoldingRegister(ushort addr, ushort regCount, out List<byte> value) {
            Stat stt = Stat.SUCCESS;
            string cmd = CreateReadHeader(DeviceID, addr, regCount, ModbusFunction.READ_HOLDING_REGISTERS);
            Send(cmd, EndChar.NONE);
            if (WaitResponse()) stt = mRxStt;
            else stt = Stat.ER_SYSTEM;

            value = (mRxData == null) ? null : mRxData.ToList();
            return stt;
        }

        /// <summary>[FC02] 讀取輸入 Bit 目前狀態，僅可讀取輸入端點(Input Only)</summary>
        /// <param name="addr">起始的 IO Address</param>
        /// <param name="bitCount">連續讀取的長度。如要讀取 0~7 則 addr=0 length=8</param>
        /// <param name="value">各 IO 的 ON OFF 狀態</param>
        /// <returns>Status Code</returns>
        public Stat ReadInputStatus(ushort addr, ushort bitCount, out List<bool> value) {
            Stat stt = Stat.SUCCESS;
            List<bool> boolResult = null;
            string cmd = CreateReadHeader(DeviceID, addr, bitCount, ModbusFunction.READ_INPUT_STATUS);
            Send(cmd, EndChar.NONE);
            if (WaitResponse()) {
                ConvertDataToBool(bitCount, out boolResult);
                stt = mRxStt;
            } else stt = Stat.ER_SYSTEM;

            value = boolResult;
            return stt;
        }

        /// <summary>[FC04] 讀取暫存器(Register)數值，僅可讀取輸入暫存器(Input Only)</summary>
        /// <param name="addr">起始的 Register Address</param>
        /// <param name="regCount">連續讀取的長度。如要讀取 512~514 則 addr=512 length=3</param>
        /// <param name="value">各 Resiger 的數值</param>
        /// <returns>Status Code</returns>
        public Stat ReadInputRegisters(ushort addr, ushort regCount, out List<byte> value) {
            Stat stt = Stat.SUCCESS;
            string cmd = CreateReadHeader(DeviceID, addr, regCount, ModbusFunction.READ_INPUT_REGISTERS);
            Send(cmd, EndChar.NONE);
            if (WaitResponse()) stt = mRxStt;
            else stt = Stat.ER_SYSTEM;

            value = (mRxData == null) ? null : mRxData.ToList();
            return stt;
        }

        /// <summary>[FC05] 設定單一 Output (Bit)</summary>
        /// <param name="addr">IO 位址</param>
        /// <param name="value">欲變更的狀態</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleCoil(ushort addr, bool value, out List<byte> result) {
            Stat stt = Stat.SUCCESS;
            List<byte> valTemp = new List<byte> { (byte)((value) ? 0xFF : 0x00), 0x00 };
            string strCMD = CreateWriteHeader(DeviceID, addr, 0, 0, valTemp, ModbusFunction.FORCE_SINGLE_COIL);
            Send(strCMD, EndChar.NONE);
            if (WaitResponse()) stt = mRxStt;
            else stt = Stat.ER_SYSTEM;
            result = (mRxData != null) ? mRxData.ToList() : null;
            return stt;
        }

        /// <summary>[FC06] 設定單一 Register (通常為 16bit)</summary>
        /// <param name="addr">Register 位址</param>
        /// <param name="value">欲寫入的數值</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleRegister(ushort addr, ushort value, out List<byte> result) {
            Stat stt = Stat.SUCCESS;
            List<byte> valTemp = BitConverter.GetBytes(value).ToList();
            valTemp.Reverse();
            string strCMD = CreateWriteHeader(DeviceID, addr, 0, 0, valTemp, ModbusFunction.PRESET_SINGLE_REGISTER);
            Send(strCMD, EndChar.NONE);
            if (WaitResponse()) stt = mRxStt;
            else stt = Stat.ER_SYSTEM;
            result = (mRxData != null) ? mRxData.ToList() : null;
            return stt;
        }

        /// <summary>[FC15] 設定連續多個 Output (Bit)</summary>
        /// <param name="addr">起始 IO 位址</param>
        /// <param name="value">欲寫入的狀態</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiCoils(ushort addr, List<bool> value, out List<byte> result) {
            Stat stt = Stat.SUCCESS;
            List<byte> valTemp;
            ConvertBoolToData(value, out valTemp);
            string strCMD = CreateWriteHeader(DeviceID, addr, (ushort)value.Count, (ushort)valTemp.Count, valTemp, ModbusFunction.FORCE_MULTI_COILS);
            Send(strCMD, EndChar.NONE);
            if (WaitResponse()) stt = mRxStt;
            else stt = Stat.ER_SYSTEM;
            result = (mRxData != null) ? mRxData.ToList() : null;
            return stt;
        }

        /// <summary>[FC16] 設定連續多個 Register</summary>
        /// <param name="addr">起始 Register 位址</param>
        /// <param name="value">欲寫入的數值</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiRegisters(ushort addr, List<ushort> value, out List<byte> result) {
            Stat stt = Stat.SUCCESS;
            List<byte> valTemp = new List<byte>();
            foreach (ushort val in value) {
                byte[] _val = BitConverter.GetBytes(val);
                valTemp.Add(_val[1]);
                valTemp.Add(_val[0]);
            }
            string strCMD = CreateWriteHeader(DeviceID, addr, (ushort)value.Count, (ushort)valTemp.Count, valTemp, ModbusFunction.PRESET_MULTI_REGISTERS);
            Send(strCMD, EndChar.NONE);
            if (WaitResponse()) stt = mRxStt;
            else stt = Stat.ER_SYSTEM;
            result = (mRxData != null) ? mRxData.ToList() : null;
            return stt;
        }
        #endregion
    }
}
