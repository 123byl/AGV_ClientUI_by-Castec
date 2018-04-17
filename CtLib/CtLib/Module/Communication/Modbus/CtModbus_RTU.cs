using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using CtLib.Library;
using CtLib.Module.SerialPort;
using CtLib.Module.Utility;

namespace CtLib.Module.Modbus {

    /// <summary>
    /// Modbus RTU 控制
    /// <para>命令以 Byte 資料為主</para>
    /// </summary>
    public class CtModbus_RTU : CtSerial, ICtModbus, ICtVersion {

        #region Version

        /// <summary>CtModbus_RTU Version Information</summary>
        /// <remarks><code language="C#">
        /// 1.0.0  Ahern [2014/12/28]
        ///     + 由 CtModbus_ASCII 整理並轉換成 CtModbus_RTU
        ///     
        /// 1.1.0  Ahern [2015/02/16]
        ///     + 繼承 ICtModbus
        ///     + Read 相關 Function 以符合 ICtModbus
        ///     
        /// 1.1.1  Ahern [2015/02/17]
        ///     + 多重繼承 CtSerial
        ///     \ 修改部分 Function 以 Override CtSerial
        ///     \ Receive 部分改以 .Net 4.0 之 Task 實作，尚未測試!
        /// 
        /// 1.1.2  Ahern [2015/08/12]
        ///     \ WaitResponse 改以 CtTimer.WaitTimeout 實作
        /// 
        /// 1.1.3  Ahern [2015/11/16]
        ///     \ 為了連接不同馬達，Decode Data 取消檢查 DeviceID
		///     
		/// 1.1.4  Ahern [2016/01/23]
		///		+ Send 加上 lock 保護
        ///     
        /// </code></remarks>
        public new CtVersion Version { get { return new CtVersion(1, 1, 4, "2016/01/23", "Ahern Kuo"); } }

        #endregion

        #region Declaration - Enumerations
        /// <summary>收到的資料之解碼狀態</summary>
        private enum DecodeStt : byte {
            /// <summary>完整解析並獲取相關資料</summary>
            Finished,
            /// <summary>目標裝置傳回錯誤代碼，將解析後的 Error Code 回報出去以讓外界知道發生的訊息</summary>
            Exception,
            /// <summary>收到不可解析之資料</summary>
            NoneAccessable
        }
        #endregion

        #region Declaration - Definitions
        /// <summary>等待命令回傳的逾時時間</summary>
        private static readonly int PROTOCOL_TIMEOUT = 3000;
        /// <summary>預設的 Device ID</summary>
        private static readonly byte MODBUS_DEVICE_ID = 0x01;
        #endregion

        #region Declaration - Fields
        /*-- Modbus Data --*/
        /// <summary>暫存已收到的資料</summary>
        private List<byte> mRxData = new List<byte>();
        /// <summary>暫存解碼的資料</summary>
        private List<byte> mDxData = new List<byte>();
        /// <summary>最後一筆資料之解碼狀態</summary>
        private DecodeStt mLastDecodeStt = DecodeStt.NoneAccessable;

        /*-- Flag --*/
        /// <summary>[Flag] 是否收到資料</summary>
        private bool mFlag_RxData;
        /// <summary>[Flag] 資料是否破碎</summary>
        private bool mFlag_RxBreak;
		/// <summary>[Flag] 是否將收到或傳送的 byte 顯示於 Console 上</summary>
		private bool mFlag_ShowConsole = true;
        #endregion

        #region Declaration - Properties
        /// <summary>取得或設定 Deivce ID</summary>
        public byte DeviceID { get; set; }
		#endregion

		#region Function - Constructors
		/// <summary>建立 CtModbus_RTU，並採用預設的 Device ID</summary>
		/// <param name="showConsole">是否將收到或傳送的 byte 顯示於 Console 上</param>
		public CtModbus_RTU(bool showConsole = true) {
            //EnableReceiveEvent = false;
            DeviceID = MODBUS_DEVICE_ID;
            DataFormat = TransDataFormats.EnumerableOfByte;
			mFlag_ShowConsole = showConsole;
		}

		/// <summary>建立 CtModbus_RTU</summary>
		/// <param name="devID">Device ID</param>
		/// <param name="showConsole">是否將收到或傳送的 byte 顯示於 Console 上</param>
		public CtModbus_RTU(byte devID, bool showConsole = true) {
            //EnableReceiveEvent = false;
            DeviceID = devID;
            DataFormat = TransDataFormats.EnumerableOfByte;
			mFlag_ShowConsole = showConsole;
		}
        #endregion

        #region Function - CtSerial Operations

        /// <summary>透過 SerialPort 傳送資料至裝置</summary>
        /// <param name="cmd">命令字串</param>
        public override void Send(List<byte> cmd) {

            /*-- Clear objects --*/
            mFlag_RxData = false;
            mFlag_RxBreak = false;
            mRxData.Clear();
            mDxData.Clear();

            /*-- Check mCom state --*/
            if (!mCom.IsOpen) throw (new Exception("Please open serial port first"));

            /*-- 顯示送出去的命令 --*/
            ConsoleShowData(cmd, "TX");

			/*-- Write the byte[] into buffer --*/
			lock (mCom) {
				mCom.Write(cmd.ToArray(), 0, cmd.Count); 
			}
        }

        /// <summary>透過 SerialPort 傳送資料至裝置，並等待回傳資料</summary>
        /// <param name="cmd">命令字串</param>
        /// <param name="result">收到回傳的資料</param>
        /// <returns>Status Code</returns>
        public Stat Send(List<byte> cmd, out List<byte> result) {
            List<byte> retTemp = null;
            Stat stt = Stat.SUCCESS;
            Send(cmd);
            if (stt == Stat.SUCCESS && WaitResponse()) {
                retTemp = mRxData.ToList();
            }
            if (mLastDecodeStt == DecodeStt.Exception) stt = Stat.ER3_MB_SLVERR;
            result = retTemp;
            return stt;
        }

        #endregion

        #region Function - CtSerial Override
        /// <summary>SerialPort 資料接收函式</summary>
        /// <param name="sender">SerialPort</param>
        /// <param name="e">EventArgs</param>
        protected override void mCom_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e) {
            try {
                /*-- If enable events, do the operations --*/
                if (EnableReceiveEvent) {
                    /*- Status Code --*/
                    Stat stt = Stat.SUCCESS;

                    /*-- Delay, wait buffer fill finish --*/
                    CtTimer.Delay(1);

                    /*-- Receive data by new task --*/
                    Task.Factory.StartNew(
                        () => {
                            List<byte> bytTemp;
                            Receive(out bytTemp);
                            if ((stt == Stat.SUCCESS) && (bytTemp != null)) {
                                ConsoleShowData(bytTemp, "RX");
                                if (VerifyCRC(bytTemp)) mLastDecodeStt = DecodeData(mRxData);
                                mFlag_RxData = true;
                            }
                        }
                    ).Wait();

                    /* ---------- 2015/05/07 -----------------------------------------------------
                     * 測試 ESMS-150-7 馬達時發現他們是一次一個 byte 回來，瞬間傳 n 個...
                     * 因為使用 Task 導致他會觸發 n 次此 function (速度過快? 因處理是交給另外的 Thread)
                     * 原本 CtSerial 是直接在 Event 裡處理之，所以 Event 的 Thread 會卡住
                     * 下次回來時已經沒資料，但是這邊因為 Event 過快離開(?!)
                     * 所以加上個 Wait() 來製造跟 CtSerial 一樣的效果...
                     * 預計以後加個 Property 去卡需不需要等待...
                     * 因為手邊沒有馬達可以測試了 T_T
                     * -------------------------------------------------------------------------- */
                }
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
        }
        #endregion

        #region Function - Methods
        /// <summary>等待訊息回應 (利用無限迴圈等待Flag變化)</summary>
        /// <returns>(<see langword="true"/>)收到資料 (<see langword="false"/>)無資料或逾時</returns>
        private bool WaitResponse() {
            return !CtTimer.WaitTimeout(
                        PROTOCOL_TIMEOUT,
                        obj => {
                            while (!mFlag_RxData && !obj.IsDone) {
                                CtTimer.Delay(10);
                            }
                            obj.WorkDone();
                        }
                    );
        }

        /// <summary>訊息解碼</summary>
        /// <param name="data">從裝置收到的 byte 資料集合</param>
        /// <returns>解碼後資料</returns>
        private DecodeStt DecodeData(List<byte> data) {
            DecodeStt pF_Finished = DecodeStt.NoneAccessable;
            mDxData.Clear();
            if (data.Count > 2) {
                ModbusFunction func = (ModbusFunction)data[1];

                /*-- 超過 0x80 部份, 是 Exception --*/
                if (func >= ModbusFunction.Exception) {
                    mDxData.Add(data[2]);
                    pF_Finished = DecodeStt.Exception;
                    /*-- 寫入的部分都是 DeviceID(1byte) + FunctionCode(1byte) + Address(2byte) + Data(2byte) + CRC(2byte) ，故這邊直接讀固定位置 --*/
                } else if (func >= ModbusFunction.ForceSingleCoil) {
                    mDxData.Add(data[4]);
                    mDxData.Add(data[5]);
                    pF_Finished = DecodeStt.Finished;
                } else {
                    /*-- 讀取的部分是 DeviceID(1byte) + FunctionCode(1byte) + DataLength(1byte) + Data(nbyte) + CRC(2byte) --*/
                    for (int idx = 3; idx < 3 + data[2]; idx++) {
                        if (idx < data.Count)
                            mDxData.Add(data[idx]);
                        else break;
                    }
                    pF_Finished = DecodeStt.Finished;
                }

            } else {
                mDxData = data.ToList();
            }
            return pF_Finished;
        }

        /// <summary>Cyclical Redundancy Check - 16bit</summary>
        /// <param name="cmd">尚未加入 CRC 資訊的 byte 資料</param>
        /// <param name="full">是否為完整(已含CRC)之資料</param>
        /// <returns>CRC Data (2*byte)。 [0]低位元 [1]高位元</returns>
        public List<byte> CRC(List<byte> cmd, bool full = false) {
            List<byte> crc = null;
            int length = (full) ? cmd.Count - 2 : cmd.Count;
            byte crcLSB;     //Least signficant bit
            ushort crcVal = 0xFFFF;
            try {
                for (int i = 0; i < length; i++) {
                    crcVal ^= cmd[i];
                    for (int bit = 0; bit < 8; bit++) {
                        crcLSB = (byte)(crcVal & 0x0001);  // 取得 LSB
                        crcVal >>= 1;                       // 移去 LSB，前補0

                        if (crcLSB == 1) crcVal ^= 0xA001;  // 如果 LSB 為 1，與 0xA001 做 XOR
                    }
                }
                crc = BitConverter.GetBytes(crcVal).ToList();   //BitConvert 轉出來是  [0]低位元 [1]高位元
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
            return crc.ToList();
        }

        /// <summary>驗證串列資料中 CRC 是否正確，需帶入含有 CRC 之資料集合，並直接取最後兩 byte 進行計算</summary>
        /// <param name="data">含有 CRC 資料之集合</param>
        /// <returns>是否為 Modbus RTU 資料。  (<see langword="true"/>)CRC驗證正確，為ModbusRTU資料  (<see langword="false"/>)CRC驗證失敗</returns>
        private bool VerifyCRC(List<byte> data) {
            bool result = false;

            if (data.Count > 0) {
                /*-- 確認在進入此次副程式前，是否曾有破碎情形發生 --*/
                if (!mFlag_RxBreak) mRxData = data.ToList();    //沒有發生過破碎，直接將data轉入全域變數
                else mRxData.AddRange(data);                    //有發生過破碎，將此次data往全域變數後面加上去

                /*-- 計算CRC --*/
                List<byte> crc = CRC(mRxData, true);

                /*-- 判斷 data 最後兩個 byte 是否與 CRC 相同 --*/
                if (mRxData[mRxData.Count - 1] == crc[1] && mRxData[mRxData.Count - 2] == crc[0]) {
                    result = true;
                } else {
                    mFlag_RxBreak = true;
                    //mRxData.AddRange(data);
                }
            }
            return result;
        }

        /// <summary>建立讀取方法之 byte 資料集合，可供直接寫入</summary>
        /// <param name="devID">Device ID</param>
        /// <param name="addr">起始 IO 或 Register 位址</param>
        /// <param name="length">欲連續讀取之長度</param>
        /// <param name="func">Modbus 功能碼</param>
        /// <returns>整合完畢之 byte 資料集合</returns>
        private List<byte> CreateReadHeader(byte devID, ushort addr, ushort length, ModbusFunction func) {
            List<byte> cmd = new List<byte>();
            cmd.Add(devID);
            cmd.Add((byte)func);

            byte[] _addr = BitConverter.GetBytes(addr);
            byte[] _length = BitConverter.GetBytes(length);

            cmd.Add(_addr[1]);
            cmd.Add(_addr[0]);
            cmd.Add(_length[1]);
            cmd.Add(_length[0]);
            cmd.AddRange(CRC(cmd));

            return cmd.ToList();
        }

        /// <summary>建立寫入方法之 byte 資料集合，可供直接寫入</summary>
        /// <param name="devID">Device ID</param>
        /// <param name="addr">起始 IO 或 Register 位置</param>
        /// <param name="count">IO 或 Register 數量。 如欲寫入 9000 至 9004 則輸入 5</param>
        /// <param name="dataLength">接續的資料長度，以 byte 數量為單位。 如 9000~9004 每個 Register 寫入 2byte，則此處帶入 10</param>
        /// <param name="data">欲寫入的資料內容</param>
        /// <param name="func">Modbus 功能碼</param>
        /// <returns>整合完畢之 byte 資料集合</returns>
        private List<byte> CreateWriteHeader(byte devID, ushort addr, ushort count, byte dataLength, List<byte> data, ModbusFunction func) {
            List<byte> cmd = new List<byte>();
            cmd.Add(devID);
            cmd.Add((byte)func);

            byte[] _addr = BitConverter.GetBytes(addr);
            cmd.Add(_addr[1]);
            cmd.Add(_addr[0]);

            if (count > 0) {
                byte[] _count = BitConverter.GetBytes(count);
                cmd.Add(_count[1]);
                cmd.Add(_count[0]);
            }

            if (dataLength > 0) cmd.Add(dataLength);

            cmd.AddRange(data);
            cmd.AddRange(CRC(cmd));

            return cmd.ToList();
        }

        private void ConsoleShowData(List<byte> data, string title) {
			if (mFlag_ShowConsole) {
				List<string> tempStr = data.ConvertAll(val => CtConvert.ToHex(val));
				Console.WriteLine(string.Format("[{0}] [{1}] {2}", DateTime.Now.ToString("HH:mm:ss.ff"), title, string.Join(" ", tempStr))); 
			}
        }

        private void ConvertDataToBool(ushort count, out List<bool> bolVal) {
            byte size = sizeof(byte) * 8;
            byte maxCount = 0;
            byte valTemp = 0;
            List<bool> bRet = new List<bool>();
            for (int idx = 0; idx < mDxData.Count; idx++) {
                valTemp = mDxData[idx];
                if (count <= size) maxCount = (byte)count;
                else maxCount = (byte)(((idx * size) < count) ? size : count % size);
                for (byte bit = 0; bit < maxCount; bit++) {
                    bRet.Add(((valTemp & 0x01) == 1) ? true : false);
                    valTemp >>= 1;
                }
            }
            bolVal = bRet;
        }

        private void ConvertBoolToData(List<bool> bolVal, out List<byte> bytVal) {
            List<byte> valTemp = new List<byte>();
            byte size = sizeof(byte) * 8;
            byte uintTemp = 0;

            byte count = 0;
            for (int idx = bolVal.Count; idx > 0; idx--) {
                uintTemp <<= 1;
                if (bolVal[idx - 1])
                    uintTemp++;
                count++;
                if (count == size || idx == bolVal.Count) {
                    valTemp.Add(uintTemp);
                    uintTemp = 0;
                    count = 0;
                }
            }
            valTemp.Reverse();
            bytVal = valTemp;
        }

        private Stat Sending(List<byte> data) {
            Stat stt = Stat.SUCCESS;
            try {
                Send(data);
                if (WaitResponse()) {
                    if (mLastDecodeStt == DecodeStt.Exception) stt = Stat.ER3_MB_SLVERR;
                    else if (mLastDecodeStt == DecodeStt.NoneAccessable) stt = Stat.ER3_MB_UNDATA;
                } else stt = Stat.ER3_MB_COMTMO;
            } catch (Exception ex) {
                stt = Stat.ER3_MB_COMERR;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }
        #endregion

        #region Function - Core

        /// <summary>讀取指定裝置特定 IO 或 Register</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">起始 IO 或 Register 位址。 如 0x9000</param>
        /// <param name="length">欲連續讀取的長度。 如 9000 讀取至 9004 則帶入 5</param>
        /// <param name="func">Modbus 功能碼</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        private Stat ReadData(byte devID, ushort addr, ushort length, ModbusFunction func, out List<byte> result) {
            List<byte> cmd = CreateReadHeader(devID, addr, length, func);
            Stat stt = Sending(cmd);
            result = mDxData.Count > 0 ? mDxData.ToList() : null;
            return stt;
        }

        /// <summary>[FC01] 讀取指定裝置的 Coil (InOut, Output) 狀態</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">IO Address</param>
        /// <param name="coilCount">連續讀取的數量。如欲 0~7 則輸入 8 </param>
        /// <param name="state">各點的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
        /// <returns>Status Code</returns>
        public Stat ReadCoilStatus(byte devID, ushort addr, ushort coilCount, out List<bool> state) {
            List<bool> boolResult = null;
            List<byte> cmd = CreateReadHeader(devID, addr, coilCount, ModbusFunction.ReadCoilState);
            Stat stt = Sending(cmd);
			if (stt == Stat.SUCCESS) CtConvert.ToBoolSequence(mDxData, out boolResult, coilCount);
            state = boolResult;
            return stt;
        }

        /// <summary>[FC03] 讀取指定裝置的暫存器(Register)數值，可為輸入或輸出之暫存器(Input, Output, InOut)</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">Register Address</param>
        /// <param name="regCount">連續讀取的暫存器數量。如 4000~4007 則輸入 8</param>
        /// <param name="value">(<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
        /// <returns>Status Code</returns>
        /// <remarks>以 Wago IO 為例，一個 Register 都是 16bit，所以回傳的資訊以 ushort (uint16) 為主</remarks>
        public Stat ReadHoldingRegister(byte devID, ushort addr, ushort regCount, out List<byte> value) {
            List<byte> cmd = CreateReadHeader(devID, addr, regCount, ModbusFunction.ReadHoldingRegister);
            Stat stt = Sending(cmd);
            value = (mDxData.Count > 0) ? mDxData.ToList() : null;
            return stt;
        }

        /// <summary>[FC02] 讀取指定裝置的輸入 Bit 目前狀態，僅可讀取輸入端點(Input Only)</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">起始的 IO Address</param>
        /// <param name="bitCount">連續讀取的長度。如要讀取 0~7 則 addr=0 length=8</param>
        /// <param name="value">各 IO 的 ON OFF 狀態</param>
        /// <returns>Status Code</returns>
        public Stat ReadInputStatus(byte devID, ushort addr, ushort bitCount, out List<bool> value) {
            List<bool> boolResult = null;
            List<byte> cmd = CreateReadHeader(devID, addr, bitCount, ModbusFunction.ReadCoilState);
            Stat stt = Sending(cmd);
            if (stt == Stat.SUCCESS) CtConvert.ToBoolSequence(mDxData, out boolResult, bitCount);
			value = boolResult;
            return stt;
        }

        /// <summary>[FC04] 讀取指定裝置的暫存器(Register)數值，僅可讀取輸入暫存器(Input Only)</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">起始的 Register Address</param>
        /// <param name="regCount">連續讀取的長度。如要讀取 512~514 則 addr=512 length=3</param>
        /// <param name="value">各 Resiger 的數值</param>
        /// <returns>Status Code</returns>
        public Stat ReadInputRegisters(byte devID, ushort addr, ushort regCount, out List<byte> value) {
            List<byte> cmd = CreateReadHeader(devID, addr, regCount, ModbusFunction.ReadInputRegister);
            Stat stt = Sending(cmd);
            value = (mDxData.Count > 0) ? mDxData.ToList() : null;
            return stt;
        }

        /// <summary>[FC05] 設定指定裝置的單一 Output (Bit)</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">IO 位址。 如 0x0D03</param>
        /// <param name="value">欲變更的狀態</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleCoil(byte devID, ushort addr, bool value, out List<byte> result) {
            List<byte> cmd = CreateWriteHeader(devID, addr, 0, 0, new List<byte> { (byte)((value) ? 0xFF : 0x00), 0x00 }, ModbusFunction.ForceSingleCoil);
            Stat stt = Sending(cmd);
            result = (mDxData.Count > 0) ? mDxData.ToList() : null;
            return stt;
        }

        /// <summary>[FC06] 設定指定裝置的單一 Register (通常為 16bit)</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">Register 位址。 如 0x04D3</param>
        /// <param name="value">欲寫入的數值</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleRegister(byte devID, ushort addr, ushort value, out List<byte> result) {
            List<byte> data = BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)value)).ToList();
            List<byte> cmd = CreateWriteHeader(devID, addr, 0, 0, data, ModbusFunction.PresetSingleRegister);
            Stat stt = Sending(cmd);
            result = (mDxData.Count > 0) ? mDxData.ToList() : null;
            return stt;
        }

        /// <summary>[FC15] 設定指定裝置的連續多個 Output (Bit)</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">起始 IO 位址。 如 0x04D3</param>
        /// <param name="value">欲寫入的狀態</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiCoils(byte devID, ushort addr, List<bool> value, out List<byte> result) {
			//List<byte> bytVal;
			//ConvertBoolToData(value, out bytVal);

			/*-- 將 bool 集合轉成傳輸順序的 byte 集合 --*/
			List<byte> tempData, presetData = new List<byte>();
			CtConvert.ToNumericSequence(value, out tempData);

			/*-- 因 Modbus FC15 是先傳送低位元再傳送高位元，故需將之高低位對調 --*/
			for (int seq = 0; seq < tempData.Count; seq += 2) {
				if (seq + 1 < tempData.Count) presetData.Add(tempData[seq + 1]);
				presetData.Add(tempData[seq]);
			}

			List<byte> cmd = CreateWriteHeader(devID, addr, (ushort)value.Count, (byte)presetData.Count, presetData, ModbusFunction.ForceMultiCoils);
            Stat stt = Sending(cmd);
            result = (mDxData.Count > 0) ? mDxData.ToList() : null;
            return stt;
        }

        /// <summary>[FC16] 設定指定裝置的連續多個 Register</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">起始 Register 位址。 如 0x9000</param>
        /// <param name="value">欲寫入的數值</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiRegisters(byte devID, ushort addr, List<ushort> value, out List<byte> result) {
            List<byte> bytVal = new List<byte>();
            foreach (ushort val in value) {
                byte[] _val = BitConverter.GetBytes(val);
                bytVal.Add(_val[1]);
                bytVal.Add(_val[0]);
            }
            List<byte> cmd = CreateWriteHeader(devID, addr, (ushort)value.Count, (byte)bytVal.Count, bytVal, ModbusFunction.PresetMultiRegisters);
            Stat stt = Sending(cmd);
            result = (mDxData.Count > 0) ? mDxData.ToList() : null;
            return stt;
        }

        /// <summary>[FC05] 設定指定裝置的單一 Output (Bit)，但不等待裝置回應</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">IO 位址。 如 0x0D03</param>
        /// <param name="value">欲變更的狀態</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleCoil(byte devID, ushort addr, bool value) {
            Stat stt = Stat.SUCCESS;
            List<byte> cmd = CreateWriteHeader(devID, addr, 0, 0, new List<byte> { (byte)((value) ? 0xFF : 0x00), 0x00 }, ModbusFunction.ForceSingleCoil);
            try {
                Send(cmd);
            } catch (Exception ex) {
                stt = Stat.ER3_MB_COMERR;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }

        /// <summary>[FC06] 設定指定裝置的單一 Register (通常為 16bit)，但不等待裝置回應</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">Register 位址。 如 0x04D3</param>
        /// <param name="value">欲寫入的數值</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleRegister(byte devID, ushort addr, ushort value) {
            Stat stt = Stat.SUCCESS;
            List<byte> data = BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)value)).ToList();
            List<byte> cmd = CreateWriteHeader(devID, addr, 0, 0, data, ModbusFunction.PresetSingleRegister);
            try {
                Send(cmd);
            } catch (Exception ex) {
                stt = Stat.ER3_MB_COMERR;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }

        /// <summary>[FC15] 設定指定裝置的連續多個 Output (Bit)，但不等待裝置回應</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">起始 IO 位址。 如 0x04D3</param>
        /// <param name="value">欲寫入的狀態</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiCoils(byte devID, ushort addr, List<bool> value) {
            Stat stt = Stat.SUCCESS;

			//List<byte> bytVal;
			//ConvertBoolToData(value, out bytVal);

			/*-- 將 bool 集合轉成傳輸順序的 byte 集合 --*/
			List<byte> tempData, presetData = new List<byte>();
			CtConvert.ToNumericSequence(value, out tempData);

			/*-- 因 Modbus FC15 是先傳送低位元再傳送高位元，故需將之高低位對調 --*/
			for (int seq = 0; seq < tempData.Count; seq += 2) {
				if (seq + 1 < tempData.Count) presetData.Add(tempData[seq + 1]);
				presetData.Add(tempData[seq]);
			}

			List<byte> cmd = CreateWriteHeader(devID, addr, (ushort)value.Count, (byte)presetData.Count, presetData, ModbusFunction.ForceMultiCoils);
            try {
                Send(cmd);
            } catch (Exception ex) {
                stt = Stat.ER3_MB_COMERR;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }

        /// <summary>[FC16] 設定指定裝置的連續多個 Register，但不等待裝置回應</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">起始 Register 位址。 如 0x9000</param>
        /// <param name="value">欲寫入的數值</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiRegisters(byte devID, ushort addr, List<ushort> value) {
            Stat stt = Stat.SUCCESS;
            List<byte> bytVal = new List<byte>();
            foreach (ushort val in value) {
                byte[] _val = BitConverter.GetBytes(val);
                bytVal.Add(_val[1]);
                bytVal.Add(_val[0]);
            }
            List<byte> cmd = CreateWriteHeader(devID, addr, (ushort)value.Count, (byte)bytVal.Count, bytVal, ModbusFunction.PresetMultiRegisters);
            try {
                Send(cmd);
            } catch (Exception ex) {
                stt = Stat.ER3_MB_COMERR;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }

        /// <summary>讀取裝置特定 IO 或 Register</summary>
        /// <param name="addr">起始 IO 或 Register 位址。 如 0x9000</param>
        /// <param name="length">欲連續讀取的長度。 如 9000 讀取至 9004 則帶入 5</param>
        /// <param name="func">Modbus 功能碼</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        private Stat ReadData(ushort addr, ushort length, ModbusFunction func, out List<byte> result) {
            List<byte> value;
            Stat stt = ReadData(DeviceID, addr, length, func, out value);
            result = value;
            return stt;
        }

        /// <summary>[FC01] 讀取 Coil (InOut, Output) 狀態</summary>
        /// <param name="addr">IO Address</param>
        /// <param name="coilCount">連續讀取的數量。如欲 0~7 則輸入 8 </param>
        /// <param name="state">各點的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
        /// <returns>Status Code</returns>
        public Stat ReadCoilStatus(ushort addr, ushort coilCount, out List<bool> state) {
            List<bool> boolResult = null;
            Stat stt = ReadCoilStatus(DeviceID, addr, coilCount, out boolResult);
            state = boolResult;
            return stt;
        }

        /// <summary>[FC03] 讀取暫存器(Register)數值，可為輸入或輸出之暫存器(Input, Output, InOut)</summary>
        /// <param name="addr">Register Address</param>
        /// <param name="regCount">連續讀取的暫存器數量。如 4000~4007 則輸入 8</param>
        /// <param name="value">(<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
        /// <returns>Status Code</returns>
        /// <remarks>以 Wago IO 為例，一個 Register 都是 16bit，所以回傳的資訊以 ushort (uint16) 為主</remarks>
        public Stat ReadHoldingRegister(ushort addr, ushort regCount, out List<byte> value) {
            List<byte> result;
            Stat stt = ReadHoldingRegister(DeviceID, addr, regCount, out result);
            value = result;
            return stt;
        }

        /// <summary>[FC02] 讀取輸入 Bit 目前狀態，僅可讀取輸入端點(Input Only)</summary>
        /// <param name="addr">起始的 IO Address</param>
        /// <param name="bitCount">連續讀取的長度。如要讀取 0~7 則 addr=0 length=8</param>
        /// <param name="value">各 IO 的 ON OFF 狀態</param>
        /// <returns>Status Code</returns>
        public Stat ReadInputStatus(ushort addr, ushort bitCount, out List<bool> value) {
            List<bool> boolResult = null;
            Stat stt = ReadInputStatus(DeviceID, addr, bitCount, out boolResult);
            value = boolResult;
            return stt;
        }

        /// <summary>[FC04] 讀取暫存器(Register)數值，僅可讀取輸入暫存器(Input Only)</summary>
        /// <param name="addr">起始的 Register Address</param>
        /// <param name="regCount">連續讀取的長度。如要讀取 512~514 則 addr=512 length=3</param>
        /// <param name="value">各 Resiger 的數值</param>
        /// <returns>Status Code</returns>
        public Stat ReadInputRegisters(ushort addr, ushort regCount, out List<byte> value) {
            List<byte> result;
            Stat stt = ReadInputRegisters(DeviceID, addr, regCount, out result);
            value = result;
            return stt;
        }

        /// <summary>[FC05] 設定單一 Output (Bit)</summary>
        /// <param name="addr">IO 位址。 如 0x0D03</param>
        /// <param name="value">欲變更的狀態</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleCoil(ushort addr, bool value, out List<byte> result) {
            List<byte> temp;
            Stat stt = WriteSingleCoil(DeviceID, addr, value, out temp);
            result = temp;
            return stt;
        }

        /// <summary>[FC06] 設定單一 Register (通常為 16bit)</summary>
        /// <param name="addr">Register 位址。 如 0x04D3</param>
        /// <param name="value">欲寫入的數值</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleRegister(ushort addr, ushort value, out List<byte> result) {
            List<byte> temp;
            Stat stt = WriteSingleRegister(DeviceID, addr, value, out temp);
            result = temp;
            return stt;
        }

        /// <summary>[FC15] 設定連續多個 Output (Bit)</summary>
        /// <param name="addr">起始 IO 位址。 如 0x04D3</param>
        /// <param name="value">欲寫入的狀態</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiCoils(ushort addr, List<bool> value, out List<byte> result) {
            List<byte> temp;
            Stat stt = WriteMultiCoils(DeviceID, addr, value, out temp);
            result = temp;
            return stt;
        }

        /// <summary>[FC16] 設定連續多個 Register</summary>
        /// <param name="addr">起始 Register 位址。 如 0x9000</param>
        /// <param name="value">欲寫入的數值</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiRegisters(ushort addr, List<ushort> value, out List<byte> result) {
            List<byte> temp;
            Stat stt = WriteMultiRegisters(DeviceID, addr, value, out temp);
            result = temp;
            return stt;
        }

        /// <summary>[FC05] 設定單一 Output (Bit)，但不等待裝置回應</summary>
        /// <param name="addr">IO 位址。 如 0x0D03</param>
        /// <param name="value">欲變更的狀態</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleCoil(ushort addr, bool value) {
            return WriteSingleCoil(DeviceID, addr, value);
        }

        /// <summary>[FC06] 設定單一 Register (通常為 16bit)，但不等待裝置回應</summary>
        /// <param name="addr">Register 位址。 如 0x04D3</param>
        /// <param name="value">欲寫入的數值</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleRegister(ushort addr, ushort value) {
            return WriteSingleRegister(DeviceID, addr, value);
        }

        /// <summary>[FC15] 設定連續多個 Output (Bit)，但不等待裝置回應</summary>
        /// <param name="addr">起始 IO 位址。 如 0x04D3</param>
        /// <param name="value">欲寫入的狀態</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiCoils(ushort addr, List<bool> value) {
            return WriteMultiCoils(DeviceID, addr, value);
        }

        /// <summary>[FC16] 設定連續多個 Register，但不等待裝置回應</summary>
        /// <param name="addr">起始 Register 位址。 如 0x9000</param>
        /// <param name="value">欲寫入的數值</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiRegisters(ushort addr, List<ushort> value) {
            return WriteMultiRegisters(DeviceID, addr, value);
        }

        #endregion
    }
}
