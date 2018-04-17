using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using CtLib.Library;
using CtLib.Module.Net;
using CtLib.Module.Utility;

namespace CtLib.Module.Modbus {

    /// <summary>
    /// Modbus TCP
    /// <para>此程式適用 TCP 模式的 Modbus Protocol。 RTU、ASCII 請用另外的模組</para>
    /// </summary>
    public class CtModbus_TCP : CtTcpSocket, ICtModbus, ICtVersion {

        #region Version

        /// <summary>CtModbus_TCP 版本訊息</summary>
        /// <remarks><code language="C#">
        /// 1.0.0  Ahern [2015/01/11]
        ///     + 建立基礎模組，使用 Wago 750-352 Modbus 手冊進行撰寫
        ///     
        /// 1.1.0  Ahern [2015/02/12]
        ///     + 繼承 ICtModbus
        ///     \ 修改部分 Function 回傳值從 List&gt;ushort&lt; 至 List&gt;byte&lt; 以符合 ICtModbus
        ///     
        /// 1.2.0  Ahern [2015/02/16]
        ///     + 多重繼承 CtSyncSocket
        ///     - ModbusTCPEvent
        ///     - mSocket
        ///     \ 修改 Function 以符合 CtSyncSocket 之物件
        ///     + Override CtSyncSocket Function
        /// 
        /// 1.2.1  Ahern [2015/08/12]
        ///     \ WaitResponse 改以 CtTimer.WaitTimeout 實作
		///     
		/// 1.3.0  Ahern [2017/05/15]
		///		\ 套用最新版的 CtSocket
        /// </code></remarks>
        public new CtVersion Version { get { return new CtVersion(1, 3, 0, "2017/05/15", "Ahern Kuo"); } }

        #endregion

        #region Declaration - Support Class

        /// <summary>供傳遞的資料結構</summary>
        public class TransferData {
            /// <summary>Master/Slave Device ID</summary>
            public ushort ID { get; set; }
            /// <summary>Master/Slave Device Node</summary>
            /// <remarks>節點從0開始算起，如目標裝置是整個設備網路中的第100台，則節點有 "可能" 為99</remarks>
            public ushort Node { get; set; }
            /// <summary>此資料功能碼 (Function Code)。請參考 <see cref="ModbusFunction"/></summary>
            public ModbusFunction Function { get; set; }
            /// <summary>此筆資料所帶的內容</summary>
            public int Data { get; set; }
            /// <summary>建立全新的TransferData</summary>
            public TransferData() {
                ID = 31;
                Node = 0;
                Function = ModbusFunction.Exception;
                Data = 0;
            }
            /// <summary>建立一帶有預設值的 TransferData</summary>
            /// <param name="id">Device ID</param>
            /// <param name="node">Device Node</param>
            /// <param name="data">此筆資料所帶的內容</param>
            /// <param name="func">此資料功能碼 (Function Code)</param>
            public TransferData(ushort id, ushort node, int data, ModbusFunction func) {
                ID = id;
                Node = node;
                Data = data;
                Function = func;
            }
        }
        #endregion

        #region Declaration - Definisions
        /// <summary>預設的 Device ID</summary>
        private static readonly byte MODBUS_DEVICE_ID = 31;
        /// <summary>預設的 Device Node</summary>
        private static readonly byte MODBUS_DEVICE_NODE = 0;
        /// <summary>預設的 Modbus TCP IPAddress</summary>
        private static readonly string MODBUS_DEVICE_IP = "192.168.1.1";
        /// <summary>預設的 Modbus TCP Port</summary>
        private static readonly int MODBUS_DEVICE_PORT = 502;
        /// <summary>傳送/接收資料時的逾時時間</summary>
        private static readonly int MODBUS_TIMEOUT = 1000;
        #endregion

        #region Declaration - Properties
        /// <summary>取得或設定 Device ID</summary>
        public byte DeviceID { get; set; }
        /// <summary>取得或設定 Modbus TCP IPAddress</summary>
        public string DeviceIP { get; set; }
        /// <summary>取得或設定 Modbus TCP Port</summary>
        public int DevicePort { get; set; }
        /// <summary>取得或設定 Device Node</summary>
        public byte DeviceNode { get; set; }

		#endregion

		#region Declaration - Fields

        /// <summary>暫存接收到的資料</summary>
        private List<byte> mRxData = new List<byte>();


        ///// <summary>用於檢查最後一次命令傳送與接收狀態</summary>
        //private Stat mRxStt = Stat.SUCCESS;
        #endregion

        #region Function - Constructors
        /// <summary>建立一採用預設值的 CtModbus_TCP</summary>
        /// <param name="autoConnect">是否直接連線?</param>
        public CtModbus_TCP(bool autoConnect = false) : base(TransDataFormats.EnumerableOfByte, false) {
            DeviceID = MODBUS_DEVICE_ID;
            DeviceIP = MODBUS_DEVICE_IP;
            DevicePort = MODBUS_DEVICE_PORT;
            DeviceNode = MODBUS_DEVICE_NODE;
			if (autoConnect) ClientConnect(DeviceIP, DevicePort);
        }

        /// <summary>建立一帶有預設值的 CtModbus_TCP</summary>
        /// <param name="ip">Modbus TCP IPAddress</param>
        /// <param name="port">Modbus TCP Port</param>
        /// <param name="autoConnect">是否直接連線?</param>
        public CtModbus_TCP(string ip, int port, bool autoConnect = false) : base(TransDataFormats.EnumerableOfByte, false) {
            DeviceID = MODBUS_DEVICE_ID;
            DeviceNode = MODBUS_DEVICE_NODE;
            DeviceIP = ip;
            DevicePort = port;
			if (autoConnect) ClientConnect(DeviceIP, DevicePort);
        }

        /// <summary>建立一帶有預設值的 CtModbus_TCP</summary>
        /// <param name="devID">Device ID</param>
        /// <param name="devNode">Device Node</param>
        /// <param name="ip">Modbus TCP IPAddress</param>
        /// <param name="port">Modbus TCP Port</param>
        /// <param name="autoConnect">是否直接連線?</param>
        public CtModbus_TCP(byte devID, byte devNode, string ip, int port, bool autoConnect = false) : base(TransDataFormats.EnumerableOfByte, false) {
            DeviceID = devID;
            DeviceIP = ip;
            DevicePort = port;
            DeviceNode = devNode;
			if (autoConnect) ClientConnect(DeviceIP, DevicePort);
        }
        #endregion

        #region Function - Methods

        /// <summary>
        /// 計算當前長度是否已小於 8bit
        /// <para>用於計算 List(Of bool) 時之 for-loop 大小</para>
        /// </summary>
        /// <param name="idx">當前已跑的 List(Of bool) 之數量</param>
        /// <param name="count">該 List(Of bool) 之數量</param>
        /// <returns>下 8 個 Bit 數字，如不足 8 個則回傳剩餘數量</returns>
        private int CalcLeftBits(int idx, int count) {
            int temp = 0;
            if ((idx * 8 + 8) > count) temp = count % 8;    // index(個數) * 8 = 目前已經轉完的 bit 數。如果已經大於最大數量，則回傳剩餘數量 (餘 8)
            else temp = idx * 8 + 8;                        // 如果還沒到最大數量，回傳下 8 個位置
            return temp;
        }

        /// <summary>建立讀取的 MBAP Header 陣列</summary>
        /// <param name="id">Device ID</param>
        /// <param name="node">Device Node</param>
        /// <param name="addr">I/O 或 Regisiter 位置</param>
        /// <param name="length">欲連續讀取的長度。如要從 0 讀取至 3，則傳入 4 </param>
        /// <param name="func">功能碼</param>
        /// <returns>MBAP Header</returns>
        /// <remarks>MBAP = Modbus Application</remarks>
        private byte[] CreateReadHeader(byte id, byte node, ushort addr, ushort length, ModbusFunction func) {
            byte[] cmd = new byte[12];

            /*-- 將 Integer 轉為 byte[] --*/
            /*-- 如果是 int 會是 byte[4]， short/int16/ushort/uint16 則是 byte[2]，所以請注意轉換內容的格式! 這邊是從引數就限制... --*/
            /*-- BitConverter 出來的會是 6 > [0]06 [1]00 --*/
            /*-- 但在網路傳輸時是高位元先，再低位元，也就是 00 06，所以下面的順序有對調!! --*/
            /*-- 亦有人用 IPAddress.HostToNetworkOrder 來轉。但小弟覺得會有多個轉換，所以這邊拿掉 --*/
            byte[] _id = BitConverter.GetBytes(id);
            byte[] _addr = BitConverter.GetBytes(addr);
            byte[] _length = BitConverter.GetBytes(length);

            cmd[0] = _id[1];                    //Slave Device ID (高)
            cmd[1] = _id[0];                    //Slave Device ID (低)
            cmd[5] = 0x06;                      //Read Header Size
            cmd[6] = node;                      //Slave Device Node
            cmd[7] = CtConvert.CByte(func);     //Function Code
            cmd[8] = _addr[1];                  //IO Address (高)
            cmd[9] = _addr[0];                  //IO Address (低)
            cmd[10] = _length[1];               //Continue Bit/Register Length (高)
            cmd[11] = _length[0];               //Continue Bit/Register Length (低)

            return cmd;
        }

        /// <summary>建立寫入資料的 MBAP Header 陣列</summary>
        /// <param name="id">Device ID</param>
        /// <param name="node">Device Node</param>
        /// <param name="addr">I/O 或 Regisiter 位置</param>
        /// <param name="count">欲連續控制的 I/O 或 Register 數量，以 個數 計算。如要控制 Register 從 512 至 517，則輸入 6</param>
        /// <param name="dataLength">欲寫入的資料長度，以 byte 計算。如要寫入 Register 的資料為 0xF8 0xD0，則此處輸入 2</param>
        /// <param name="func">功能碼 (Function Code)</param>
        /// <returns>MBAP Header</returns>
        /// /// <remarks>MBAP = Modbus Application</remarks>
        private byte[] CreateWriteHeader(byte id, byte node, ushort addr, ushort count, ushort dataLength, ModbusFunction func) {
            byte[] cmd = new byte[dataLength + 11];

            /*-- 將 Integer 轉為 byte[] --*/
            /*-- 如果是 int 會是 byte[4]， short/int16/ushort/uint16 則是 byte[2]，所以請注意轉換內容的格式! 這邊是從引數就限制... --*/
            /*-- BitConverter 出來的會是 6 > [0]06 [1]00 --*/
            /*-- 但在網路傳輸時是高位元先，再低位元，也就是 00 06，所以下面的順序有對調!! --*/
            /*-- 亦有人用 IPAddress.HostToNetworkOrder 來轉。但小弟覺得會有多個轉換，所以這邊拿掉 --*/
            byte[] _id = BitConverter.GetBytes(id);
            byte[] _addr = BitConverter.GetBytes(addr);
            byte[] _length = BitConverter.GetBytes(dataLength + 5);

            cmd[0] = _id[1];                                        //Slave Device ID (高)
            cmd[1] = _id[0];                                        //Slave Device ID (低)
            cmd[4] = _length[1];                                    //Data Length (高)
            cmd[5] = _length[0];                                    //Data Length (低)
            cmd[6] = node;                                          //Slave Device Node
            cmd[7] = CtConvert.CByte(func);                         //Function Code
            cmd[8] = _addr[1];                                      //IO or Register Address (高)
            cmd[9] = _addr[0];                                      //IO or Register Address (低)

            /*-- 如果是多筆性資料，如 MultiCoil、MultiRegister --*/
            if (func >= ModbusFunction.ForceMultiCoils) {
                byte[] _count = BitConverter.GetBytes(count);       //寫入數量
                cmd[10] = _count[1];                                //寫入數量 (高)
                cmd[11] = _count[0];                                //寫入數量 (低)
                cmd[12] = CtConvert.CByte(dataLength - 2);          //後面接續的資料長度 (以 byte 計算)
            }
            return cmd;
        }

        /// <summary>等待 Slave 回應任何訊息</summary>
        /// <returns>(<see langword="true"/>)收到任意資料   (<see langword="false"/>)逾時</returns>
        private bool WaitResponse() {
			//return !CtTimer.WaitTimeout(
			//            MODBUS_TIMEOUT,
			//            obj => {
			//                while (!mFlag_RxData && !obj.IsDone) {
			//                    CtTimer.Delay(10);
			//                }
			//                obj.WorkDone();
			//            }
			//        );

			bool stt = true;
			try {
				var wrap = Receive(MODBUS_TIMEOUT);
				var rxData = wrap.Data as List<byte>;
				stt = rxData != null && rxData.Count > 0;
			} catch (Exception) {
				stt = false;
			}
			return stt;
        }

        /// <summary>回傳資料解碼</summary>
        /// <param name="data">收到的資料陣列</param>
        private Stat Decode(List<byte> data) {
            Stat stt = Stat.SUCCESS;
			byte[] bytTemp = null;
			if (data != null && data.Count >= 7) {
				TransferData rxData = new TransferData();
				rxData.ID = (ushort)(data[0] * 256 + data[1]);
				rxData.Node = data[6];
				rxData.Function = (ModbusFunction)data[7];

				if (rxData.Function >= ModbusFunction.ForceSingleCoil) {
					if (data.Count >= 12) {
						bytTemp = new byte[2];
						data.CopyTo(10, bytTemp, 0, 2);
					} else stt = Stat.ER_SYS_INVIDX;
				} else if (rxData.Function >= ModbusFunction.Exception) {
					stt = Stat.ER3_MB_SLVERR;
					if (data.Count >= 8) bytTemp = new byte[1] { data[7] };  //等同回傳的Error Code
					else stt = Stat.ER_SYS_INVIDX;
				} else if (data.Count >= (9 + data[8])) {
					bytTemp = new byte[data[8]];
					Array.Copy(data.ToArray(), 9, bytTemp, 0, data[8]);
				} else stt = Stat.ER_SYS_INVIDX;
			} else stt = Stat.ER_SYS_INVIDX;

            mRxData.Clear();
            //if (bytTemp.Length > 1) {
            //    //以下為回傳 ushort (高位元加低位元) 之方法
            //    for (int idx = 0; idx < bytTemp.Length; idx += 2) {
            //        mRxData.Add((ushort) (bytTemp[idx] * 256 + bytTemp[idx + 1]));
            //    }
            //} else if (bytTemp.Length > 0) {
            //    mRxData.Add((ushort)bytTemp[0]);
            //}

            //改以 ICtModbus 後，目前均回傳 byte
            if (bytTemp != null) mRxData.AddRange(bytTemp);
            return stt;
        }

        /// <summary>
        /// 將 List(Of bool) 也就是表示二進位的資料，轉換為 byte[]
        /// <para>如 1100110101 轉為 0x03 0x35</para>
        /// </summary>
        /// <param name="data">欲寫入的 bool 陣列資料</param>
        /// <returns>轉換完之 byte[]</returns>
        private byte[] CalcByte(List<bool> data) {
            int size = data.Count / 8;
            if (data.Count % 8 > 0) size++;
            byte[] value = new byte[size];

            for (int idx = 0; idx < value.Length; idx++) {
                for (int subIdx = idx * 8; subIdx < CalcLeftBits(idx, data.Count); subIdx++) {
                    value[idx] <<= 1;
                    value[idx] += (byte)((data[subIdx]) ? 1 : 0);
                }
            }

            return value;
        }

        private void ConvertDataToBool(ushort count, out List<bool> bolVal) {
            byte size = sizeof(byte) * 8;
            byte maxCount = 0;
            byte valTemp = 0;
            List<bool> bRet = new List<bool>();
            for (int idx = 0; idx < mRxData.Count; idx++) {
                valTemp = mRxData[idx];
                if (count <= size) maxCount = (byte)count;
                else maxCount = (byte)(((idx * size) < count) ? size : count % size);
                for (byte bit = 0; bit < maxCount; bit++) {
                    bRet.Add(((valTemp & 0x01) == 1) ? true : false);
                    valTemp >>= 1;
                }
            }
            bolVal = bRet;
        }

        private Stat Sending(byte[] data) {
            Stat stt = Stat.SUCCESS;
            try {
				if (!IsConnected) throw new InvalidOperationException("Connection with device was broken");
				mSocket.Send(data);
				var wrap = Receive(MODBUS_TIMEOUT);
				var rxData = wrap.Data as List<byte>;
				stt = Decode(rxData);
            } catch (Exception ex) {
                stt = Stat.ER3_MB_COMERR;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }
        #endregion

        #region Function - Core
        /// <summary>使用 Socket 傳送資料至 Slave</summary>
        /// <param name="data">欲傳送的 byte[]</param>
        /// <returns>Status Code</returns>
        private void SendData(byte[] data) {
            if (IsConnected) mSocket.Send(data);
        }

        /// <summary>[FC01] 讀取指定裝置的 Coil (InOut, Output) 狀態</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">IO Address</param>
        /// <param name="coilCount">連續讀取的數量。如欲 0~7 則輸入 8 </param>
        /// <param name="state">各點的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
        /// <returns>Status Code</returns>
        public Stat ReadCoilStatus(byte devID, ushort addr, ushort coilCount, out List<bool> state) {
            List<bool> bRet = null;
            byte[] data = CreateReadHeader(devID, DeviceNode, addr, coilCount, ModbusFunction.ReadCoilState);
            Stat stt = Sending(data);
			if (stt == Stat.SUCCESS) CtConvert.ToBoolSequence(mRxData, out bRet, coilCount);
            state = bRet;
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
            byte[] data = CreateReadHeader(devID, DeviceNode, addr, regCount, ModbusFunction.ReadHoldingRegister);
            Stat stt = Sending(data);
            value = mRxData.Count > 0 ? mRxData.ToList() : null;
            return stt;
        }

        /// <summary>[FC02] 讀取指定裝置的輸入 Bit 目前狀態，僅可讀取輸入端點(Input Only)</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">起始的 IO Address</param>
        /// <param name="bitCount">連續讀取的長度。如要讀取 0~7 則 addr=0 length=8</param>
        /// <param name="value">各 IO 的 ON OFF 狀態</param>
        /// <returns>Status Code</returns>
        public Stat ReadInputStatus(byte devID, ushort addr, ushort bitCount, out List<bool> value) {
            List<bool> bTemp = null;
            byte[] data = CreateReadHeader(devID, DeviceNode, addr, bitCount, ModbusFunction.ReadCoilState);
            Stat stt = Sending(data);
            if (stt == Stat.SUCCESS) CtConvert.ToBoolSequence(mRxData, out bTemp, bitCount);
			value = bTemp;
            return stt;
        }

        /// <summary>[FC04] 讀取指定裝置的暫存器(Register)數值，僅可讀取輸入暫存器(Input Only)</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">起始的 Register Address</param>
        /// <param name="regCount">連續讀取的長度。如要讀取 512~514 則 addr=512 length=3</param>
        /// <param name="value">各 Resiger 的數值</param>
        /// <returns>Status Code</returns>
        public Stat ReadInputRegisters(byte devID, ushort addr, ushort regCount, out List<byte> value) {
            byte[] data = CreateReadHeader(devID, DeviceNode, addr, regCount, ModbusFunction.ReadInputRegister);
            Stat stt = Sending(data);
            value = mRxData.Count > 0 ? mRxData.ToList() : null;
            return stt;
        }

        /// <summary>[FC05] 寫入指定裝置的單一 bit 狀態，並回傳結果  [Force Single Coil]</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">IO Address</param>
        /// <param name="value">欲寫入之狀態  (<see langword="true"/>)ON (<see langword="false"/>)OFF</param>
        /// <param name="result">回傳之結果</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleCoil(byte devID, ushort addr, bool value, out List<byte> result) {
            byte[] cmd = CreateWriteHeader(devID, DeviceNode, addr, 1, 1, ModbusFunction.ForceSingleCoil);
            cmd[10] = (byte)((value) ? 0xFF : 0x00);
            Stat stt = Sending(cmd);
            result = (mRxData.Count > 0) ? mRxData.ToList() : null;
            return stt;
        }

        /// <summary>[FC06] 寫入指定裝置的單一 Register 數值，並回傳結果  [Preset Single Register]</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">Register Address</param>
        /// <param name="value">欲寫入之數值</param>
        /// <param name="result">回傳之結果</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleRegister(byte devID, ushort addr, ushort value, out List<byte> result) {
            byte[] cmd = CreateWriteHeader(devID, DeviceNode, addr, 1, 1, ModbusFunction.PresetSingleRegister);
            byte[] data = BitConverter.GetBytes(value);
            cmd[10] = data[1];
            cmd[11] = data[0];
            Stat stt = Sending(cmd);
            result = (mRxData.Count > 0) ? mRxData.ToList() : null;
            return stt;
        }

        /// <summary>[FC15] 寫入指定裝置的連續 Bit 狀態  [Force Multi Coils]</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">起始 IO Address</param>
        /// <param name="value">欲寫入之狀態</param>
        /// <param name="result">回傳之結果</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiCoils(byte devID, ushort addr, List<bool> value, out List<byte> result) {
			/*-- 將 bool 集合轉成傳輸順序的 byte 集合 --*/
			List<byte> tempData, presetData = new List<byte>();
			CtConvert.ToNumericSequence(value, out tempData);

			/*-- 因 Modbus FC15 是先傳送低位元再傳送高位元，故需將之高低位對調 --*/
			for (int seq = 0; seq < tempData.Count; seq += 2) {
				if (seq + 1 < tempData.Count) presetData.Add(tempData[seq + 1]);
				presetData.Add(tempData[seq]);
			}

			//byte[] data = CalcByte(value);
			byte[] cmd = CreateWriteHeader(devID, DeviceNode, addr, (ushort)value.Count, (byte)(presetData.Count + 2), ModbusFunction.ForceMultiCoils);

            int idx = 0;
            foreach (byte item in presetData) {
                cmd[13 + idx] = item;
                idx++;
            }

            Stat stt = Sending(cmd);
            result = (mRxData.Count > 0) ? mRxData.ToList() : null;
            return stt;
        }

        /// <summary>[FC16] 寫入指定裝置的連續 Register 數值  [Preset Multi Registers]</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">起始 Register Address</param>
        /// <param name="data">欲寫入的資料</param>
        /// <param name="result">回傳之結果</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiRegisters(byte devID, ushort addr, List<ushort> data, out List<byte> result) {
            byte[] cmd = CreateWriteHeader(devID, DeviceNode, addr, (byte)(data.Count), (byte)(data.Count * 2 + 2), ModbusFunction.PresetMultiRegisters);

            int idx = 0;
            foreach (ushort value in data) {
                byte[] _val = BitConverter.GetBytes(value);
                cmd[13 + idx] = _val[1];
                cmd[14 + idx] = _val[0];
                idx += 2;
            }
            Stat stt = Sending(cmd);
            result = (mRxData.Count > 0) ? mRxData.ToList() : null;
            return stt;
        }

        /// <summary>[FC05] 寫入指定裝置的單一 bit 狀態，但不等待裝置回應  [Force Single Coil]</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">IO Address</param>
        /// <param name="value">欲寫入之狀態  (<see langword="true"/>)ON (<see langword="false"/>)OFF</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleCoil(byte devID, ushort addr, bool value) {
            Stat stt = Stat.SUCCESS;
            try {
                byte[] cmd = CreateWriteHeader(devID, DeviceNode, addr, 1, 1, ModbusFunction.ForceSingleCoil);
                cmd[10] = (byte)((value) ? 0xFF : 0x00);
                SendData(cmd);
            } catch (Exception ex) {
                stt = Stat.ER3_MB_COMERR;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }

        /// <summary>[FC06] 寫入指定裝置的單一 Register 數值，但不等待裝置回應  [Preset Single Register]</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">Register Address</param>
        /// <param name="value">欲寫入之數值</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleRegister(byte devID, ushort addr, ushort value) {
            Stat stt = Stat.SUCCESS;
            try {
                byte[] cmd = CreateWriteHeader(devID, DeviceNode, addr, 1, 1, ModbusFunction.PresetSingleRegister);
                byte[] data = BitConverter.GetBytes(value);
                cmd[10] = data[1];
                cmd[11] = data[0];
                SendData(cmd);
            } catch (Exception ex) {
                stt = Stat.ER3_MB_COMERR;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }

        /// <summary>[FC15] 寫入指定裝置的連續 Bit 狀態，但不等待裝置回應  [Force Multi Coils]</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">起始 IO Address</param>
        /// <param name="value">欲寫入之狀態</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiCoils(byte devID, ushort addr, List<bool> value) {
            Stat stt = Stat.SUCCESS;
            try {

				/*-- 將 bool 集合轉成傳輸順序的 byte 集合 --*/
				List<byte> tempData, presetData = new List<byte>();
				CtConvert.ToNumericSequence(value, out tempData);

				/*-- 因 Modbus FC15 是先傳送低位元再傳送高位元，故需將之高低位對調 --*/
				for (int seq = 0; seq < tempData.Count; seq += 2) {
					if (seq + 1 < tempData.Count) presetData.Add(tempData[seq + 1]);
					presetData.Add(tempData[seq]);
				}

				//byte[] data = CalcByte(value);
                byte[] cmd = CreateWriteHeader(devID, DeviceNode, addr, (ushort)value.Count, (byte)(presetData.Count + 2), ModbusFunction.ForceMultiCoils);

                int idx = 0;
                foreach (byte item in presetData) {
                    cmd[13 + idx] = item;
                    idx++;
                }
                SendData(cmd);
            } catch (Exception ex) {
                stt = Stat.ER3_MB_COMERR;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }

        /// <summary>[FC16] 寫入指定裝置的連續 Register 數值，但不等待裝置回應  [Preset Multi Registers]</summary>
        /// <param name="devID">指定裝置的局號</param>
        /// <param name="addr">起始 Register Address</param>
        /// <param name="data">欲寫入的資料</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiRegisters(byte devID, ushort addr, List<ushort> data) {
            Stat stt = Stat.SUCCESS;
            try {
                byte[] cmd = CreateWriteHeader(devID, DeviceNode, addr, (byte)(data.Count), (byte)(data.Count * 2 + 2), ModbusFunction.PresetMultiRegisters);

                int idx = 0;
                foreach (ushort value in data) {
                    byte[] _val = BitConverter.GetBytes(value);
                    cmd[13 + idx] = _val[1];
                    cmd[14 + idx] = _val[0];
                    idx += 2;
                }
                SendData(cmd);
            } catch (Exception ex) {
                stt = Stat.ER3_MB_COMERR;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }

        /// <summary>[FC01] 讀取 Coil (InOut, Output) 狀態</summary>
        /// <param name="addr">IO Address</param>
        /// <param name="coilCount">連續讀取的數量。如欲 0~7 則輸入 8 </param>
        /// <param name="state">各點的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
        /// <returns>Status Code</returns>
        public Stat ReadCoilStatus(ushort addr, ushort coilCount, out List<bool> state) {
            List<bool> bRet = null;
            Stat stt = ReadCoilStatus(DeviceID, addr, coilCount, out bRet);
            state = bRet;
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
            List<bool> bTemp = null;
            Stat stt = ReadInputStatus(DeviceID, addr, bitCount, out bTemp);
            value = bTemp;
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

        /// <summary>[FC05] 寫入單一 bit 狀態，並回傳結果  [Force Single Coil]</summary>
        /// <param name="addr">IO Address</param>
        /// <param name="value">欲寫入之狀態  (<see langword="true"/>)ON (<see langword="false"/>)OFF</param>
        /// <param name="result">回傳之結果</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleCoil(ushort addr, bool value, out List<byte> result) {
            List<byte> temp;
            Stat stt = WriteSingleCoil(DeviceID, addr, value, out temp);
            result = temp;
            return stt;
        }

        /// <summary>[FC06] 寫入單一 Register 數值，並回傳結果  [Preset Single Register]</summary>
        /// <param name="addr">Register Address</param>
        /// <param name="value">欲寫入之數值</param>
        /// <param name="result">回傳之結果</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleRegister(ushort addr, ushort value, out List<byte> result) {
            List<byte> temp;
            Stat stt = WriteSingleRegister(DeviceID, addr, value, out temp);
            result = temp;
            return stt;
        }

        /// <summary>[FC15] 寫入連續 Bit 狀態  [Force Multi Coils]</summary>
        /// <param name="addr">起始 IO Address</param>
        /// <param name="value">欲寫入之狀態</param>
        /// <param name="result">回傳之結果</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiCoils(ushort addr, List<bool> value, out List<byte> result) {
            List<byte> temp;
            Stat stt = WriteMultiCoils(DeviceID, addr, value, out temp);
            result = temp;
            return stt;
        }

        /// <summary>[FC16] 寫入連續 Register 數值  [Preset Multi Registers]</summary>
        /// <param name="addr">起始 Register Address</param>
        /// <param name="data">欲寫入的資料</param>
        /// <param name="result">回傳之結果</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiRegisters(ushort addr, List<ushort> data, out List<byte> result) {
            List<byte> temp;
            Stat stt = WriteMultiRegisters(DeviceID, addr, data, out temp);
            result = temp;
            return stt;
        }

        /// <summary>[FC05] 寫入單一 bit 狀態，但不等待裝置回應  [Force Single Coil]</summary>
        /// <param name="addr">IO Address</param>
        /// <param name="value">欲寫入之狀態  (<see langword="true"/>)ON (<see langword="false"/>)OFF</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleCoil(ushort addr, bool value) {
            return WriteSingleCoil(DeviceID, addr, value);
        }

        /// <summary>[FC06] 寫入單一 Register 數值，但不等待裝置回應  [Preset Single Register]</summary>
        /// <param name="addr">Register Address</param>
        /// <param name="value">欲寫入之數值</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleRegister(ushort addr, ushort value) {
            return WriteSingleRegister(DeviceID, addr, value);
        }

        /// <summary>[FC15] 寫入連續 Bit 狀態，但不等待裝置回應  [Force Multi Coils]</summary>
        /// <param name="addr">起始 IO Address</param>
        /// <param name="value">欲寫入之狀態</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiCoils(ushort addr, List<bool> value) {
            return WriteMultiCoils(DeviceID, addr, value);
        }

        /// <summary>[FC16] 寫入連續 Register 數值，但不等待裝置回應  [Preset Multi Registers]</summary>
        /// <param name="addr">起始 Register Address</param>
        /// <param name="data">欲寫入的資料</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiRegisters(ushort addr, List<ushort> data) {
            return WriteMultiRegisters(DeviceID, addr, data);
        }

        #endregion

        #region Function - CtSyncSocket Override
        ///// <summary>[Socket][Client] Socket 接收資料之處理執行緒(Thread)</summary>
        //protected override void tsk_Client_RxData() {
        //    NetworkStream netStream = mClient.GetStream();
        //    List<byte> rxData = new List<byte>();
        //    do {
        //        try {
        //            /*-- If mClient is dispose or disconnect, exit the loop --*/
        //            if (mClient == null) break;

        //            /*-- Read stream buffer into byte[] --*/
        //            rxData.Clear();
        //            do {
        //                rxData.Add((byte)netStream.ReadByte());
        //            } while (netStream.DataAvailable);

        //            /*-- If there are something passing-in --*/
        //            if (rxData.Count > 0) {
        //                mRxStt = Decode(rxData);
        //                mFlag_RxData = true;
        //            }
        //            /*-- Dalay --*/
        //            CtTimer.Delay(1);

        //        } catch (IOException) {
        //            /*-- Raise disconnect event --*/
        //            RaiseEvents(
        //                    SocketEvents.CONNECTED_WITH_SERVER,
        //                    new SocketConnection(
        //                            false,
        //                            DeviceIP,
        //                            DevicePort
        //                )
        //            );
        //            break;
        //        } catch (ThreadInterruptedException) {
        //            /*-- Using to catch exception of exit thread, but not report --*/
        //        } catch (Exception ex) {
        //            CtStatus.Report(Stat.ER_SYSTEM, ex);
        //        }
        //    } while (mThread_ClientReceiveData.IsAlive);
        //}

        ///// <summary>[Socket][Server] 各 Socket 接收資料之處理執行緒(Thread)</summary>
        ///// <param name="socket">此執行緒欲負責接收的 Socket 物件</param>
        //protected override void tsk_Server_RxData(object socket) {
        //    Socket sckTemp = socket as Socket;                      //Temporary Socket from listen thread
        //    NetworkStream netStream = new NetworkStream(sckTemp);   //Get the stream of the Socket that passing
        //    List<byte> rxData = new List<byte>();
        //    try {
        //        do {
        //            try {
        //                /*-- If server or socket disconnect, exit the loop --*/
        //                if (mServer == null) break;

        //                /*-- Create the byte[] to receive data --*/
        //                rxData.Clear();
        //                do {
        //                    rxData.Add((byte)netStream.ReadByte());
        //                } while (netStream.DataAvailable);

        //                /*-- If there are somehting exist --*/
        //                if (rxData.Count > 0) {

        //                    mRxStt = Decode(rxData);
        //                    mFlag_RxData = true;

        //                    /*-- If there are no response from client, close the tube --*/
        //                } else {
        //                    /*-- Remove the Socket from list --*/
        //                    mClientList.Remove(sckTemp);
        //                    sckTemp.Disconnect(true);

        //                    /*-- Raise Event --*/
        //                    RaiseEvents(
        //                        SocketEvents.CLIENT_CONNECTED,
        //                        new SocketConnection(
        //                            false,
        //                            (sckTemp.RemoteEndPoint as IPEndPoint).Address.ToString(),
        //                            (sckTemp.RemoteEndPoint as IPEndPoint).Port
        //                        )
        //                    );
        //                    break;
        //                }

        //                /*-- Delay --*/
        //                CtTimer.Delay(1);

        //            } catch (IOException) {
        //                /*-- Remove the Socket from list --*/
        //                mClientList.Remove(sckTemp);
        //                sckTemp.Disconnect(true);

        //                /*-- Raise Event --*/
        //                RaiseEvents(
        //                    SocketEvents.CLIENT_CONNECTED,
        //                    new SocketConnection(
        //                            false,
        //                            (sckTemp.RemoteEndPoint as IPEndPoint).Address.ToString(),
        //                            (sckTemp.RemoteEndPoint as IPEndPoint).Port
        //                    )
        //                );
        //                break;
        //            } catch (Exception ex) {
        //                CtStatus.Report(Stat.ER_SYSTEM, ex);
        //            }
        //        } while (mThread_ServerListen.IsAlive);
        //    } catch (Exception) {
        //        /*-- Using to catch exception of exit thread --*/
        //    }
        //}
        #endregion
    }
}
