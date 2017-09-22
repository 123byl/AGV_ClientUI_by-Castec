using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.TCPIP;
using CtLib.Module.Ultity;

namespace CtLib.Module.Modbus {

    /// <summary>
    /// Modbus TCP
    /// <para>此程式適用 TCP 模式的 Modbus Protocol。 RTU、ASCII 請用另外的模組</para>
    /// </summary>
    public class CtModbus_TCP : CtSyncSocket, ICtModbus {

        #region Version

        /// <summary>CtModbus_TCP 版本訊息</summary>
        /// <remarks><code>
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
        /// </code></remarks>
        public static readonly new CtVersion @Version = new CtVersion(1, 2, 0, "2015/02/16", "Ahern Kuo");

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
                Function = ModbusFunction.EXCEPTION;
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
        private static readonly long MODBUS_TIMEOUT = 1000;
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

        #region Declaration - Members

        /// <summary>暫存接收到的資料</summary>
        private List<byte> mRxData = new List<byte>();
        /// <summary>[Flag] 是否收到回傳資料</summary>
        private bool mFlag_RxData;

        /// <summary>用於檢查最後一次命令傳送與接收狀態</summary>
        private Stat mRxStt = Stat.SUCCESS;
        #endregion

        #region Function - Constructors
        /// <summary>建立一採用預設值的 CtModbus_TCP</summary>
        /// <param name="autoConnect">是否直接連線?</param>
        public CtModbus_TCP(bool autoConnect = false) {
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
        public CtModbus_TCP(string ip, int port, bool autoConnect = false) {
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
        public CtModbus_TCP(byte devID, byte devNode, string ip, int port, bool autoConnect = false) {
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
            if (func >= ModbusFunction.FORCE_MULTI_COILS) {
                byte[] _count = BitConverter.GetBytes(count);       //寫入數量
                cmd[10] = _count[1];                                //寫入數量 (高)
                cmd[11] = _count[0];                                //寫入數量 (低)
                cmd[12] = CtConvert.CByte(dataLength - 2);          //後面接續的資料長度 (以 byte 計算)
            }
            return cmd;
        }

        /// <summary>等待 Slave 回應任何訊息</summary>
        /// <returns>(True)收到任意資料   (False)逾時</returns>
        private bool WaitResponse() {
            bool bRet = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            do {
                Thread.Sleep(1);
                Application.DoEvents();
            } while (!mFlag_RxData && sw.ElapsedMilliseconds < MODBUS_TIMEOUT);
            sw.Stop();
            bRet = mFlag_RxData;
            return bRet;
        }

        /// <summary>回傳資料解碼</summary>
        /// <param name="data">收到的資料陣列</param>
        private Stat Decode(List<byte> data) {
            Stat stt = Stat.SUCCESS;
            TransferData rxData = new TransferData();
            rxData.ID = (ushort)(data[0] * 256 + data[1]);
            rxData.Node = data[6];
            rxData.Function = (ModbusFunction)data[7];

            byte[] bytTemp;

            if (rxData.Function >= ModbusFunction.FORCE_SINGLE_COIL) {
                bytTemp = new byte[2];
                data.CopyTo(10, bytTemp, 0, 2);
            } else if (rxData.Function >= ModbusFunction.EXCEPTION) {
                stt = Stat.ER3_MB_SLVERR;
                bytTemp = new byte[1] { data[7] };  //等同回傳的Error Code
            } else {
                bytTemp = new byte[data[8]];
                Array.Copy(data.ToArray(), 9, bytTemp, 0, data[8]);
            }

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
            mRxData.AddRange(bytTemp);
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
        #endregion

        #region Function - Core
        /// <summary>使用 Socket 傳送資料至 Slave</summary>
        /// <param name="data">欲傳送的 byte[]</param>
        /// <returns>Status Code</returns>
        private void SendData(byte[] data) {
            mFlag_RxData = false;
            if (IsConnected) Send(data);
        }

        /// <summary>[FC01] 讀取 Coil (InOut, Output) 狀態</summary>
        /// <param name="addr">IO Address</param>
        /// <param name="coilCount">連續讀取的數量。如欲 0~7 則輸入 8 </param>
        /// <param name="state">各點的狀態  (True)ON  (False)OFF</param>
        /// <returns>Status Code</returns>
        public Stat ReadCoilStatus(ushort addr, ushort coilCount, out List<bool> state) {
            Stat stt = Stat.SUCCESS;
            List<bool> bRet = null;
            try {
                byte[] data = CreateReadHeader(DeviceID, DeviceNode, addr, coilCount, ModbusFunction.READ_COIL_STATUS);
                SendData(data);
                if (WaitResponse()) {
                    if (mRxData != null) ConvertDataToBool(coilCount, out bRet);
                    stt = mRxStt;
                    //bRet = mRxData.ConvertAll(new Converter<byte, bool>(val => CtConvert.CBool(val)));
                }
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
            state = bRet;
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
            List<byte> usRet = null;
            try {
                byte[] data = CreateReadHeader(DeviceID, DeviceNode, addr, regCount, ModbusFunction.READ_HOLDING_REGISTERS);
                SendData(data);
                if (WaitResponse()) {
                    usRet = mRxData.ToList();
                    stt = mRxStt;
                }
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
            value = usRet;
            return stt;
        }

        /// <summary>[FC02] 讀取輸入 Bit 目前狀態，僅可讀取輸入端點(Input Only)</summary>
        /// <param name="addr">起始的 IO Address</param>
        /// <param name="bitCount">連續讀取的長度。如要讀取 0~7 則 addr=0 length=8</param>
        /// <param name="value">各 IO 的 ON OFF 狀態</param>
        /// <returns>Status Code</returns>
        public Stat ReadInputStatus(ushort addr, ushort bitCount, out List<bool> value) {
            Stat stt = Stat.SUCCESS;
            List<bool> bTemp = null;
            try {
                byte[] data = CreateReadHeader(DeviceID, DeviceNode, addr, bitCount, ModbusFunction.READ_INPUT_STATUS);
                SendData(data);
                if (WaitResponse()) {
                    if (mRxData != null) ConvertDataToBool(bitCount, out bTemp);
                    stt = mRxStt;
                    //bTemp = mRxData.ConvertAll(new Converter<ushort, bool>(val => CtConvert.CBool(val)));
                }
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
            value = bTemp.ToList();
            return stt;
        }

        /// <summary>[FC04] 讀取暫存器(Register)數值，僅可讀取輸入暫存器(Input Only)</summary>
        /// <param name="addr">起始的 Register Address</param>
        /// <param name="regCount">連續讀取的長度。如要讀取 512~514 則 addr=512 length=3</param>
        /// <param name="value">各 Resiger 的數值</param>
        /// <returns>Status Code</returns>
        public Stat ReadInputRegisters(ushort addr, ushort regCount, out List<byte> value) {
            Stat stt = Stat.SUCCESS;
            List<byte> bTemp = new List<byte>();
            try {
                byte[] data = CreateReadHeader(DeviceID, DeviceNode, addr, regCount, ModbusFunction.READ_INPUT_REGISTERS);
                SendData(data);
                if (WaitResponse()) {
                    bTemp = mRxData.ToList();
                    stt = mRxStt;
                }
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
            value = bTemp.ToList();
            return stt;
        }

        /// <summary>[FC05] 寫入單一 bit 狀態，並回傳結果  [Force Single Coil]</summary>
        /// <param name="addr">IO Address</param>
        /// <param name="value">欲寫入之狀態  (True)ON (False)OFF</param>
        /// <param name="result">回傳之結果</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleCoil(ushort addr, bool value, out List<byte> result) {
            Stat stt = Stat.SUCCESS;
            try {
                byte[] cmd = CreateWriteHeader(DeviceID, DeviceNode, addr, 1, 1, ModbusFunction.FORCE_SINGLE_COIL);
                cmd[10] = (byte)((value) ? 0xFF : 0x00);
                SendData(cmd);
                WaitResponse();
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
            result = (mRxData != null) ? mRxData.ToList() : null;
            stt = mRxStt;
            return stt;
        }

        /// <summary>[FC06] 寫入單一 Register 數值，並回傳結果  [Preset Single Register]</summary>
        /// <param name="addr">Register Address</param>
        /// <param name="value">欲寫入之數值</param>
        /// <param name="result">回傳之結果</param>
        /// <returns>Status Code</returns>
        public Stat WriteSingleRegister(ushort addr, ushort value, out List<byte> result) {
            Stat stt = Stat.SUCCESS;
            try {
                byte[] cmd = CreateWriteHeader(DeviceID, DeviceNode, addr, 1, 1, ModbusFunction.PRESET_SINGLE_REGISTER);
                byte[] data = BitConverter.GetBytes(value);
                cmd[10] = data[1];
                cmd[11] = data[0];
                SendData(cmd);
                WaitResponse();
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
            result = (mRxData != null) ? mRxData.ToList() : null;
            stt = mRxStt;
            return stt;
        }

        /// <summary>[FC15] 寫入連續 Bit 狀態  [Force Multi Coils]</summary>
        /// <param name="addr">起始 IO Address</param>
        /// <param name="value">欲寫入之狀態</param>
        /// <param name="result">回傳之結果</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiCoils(ushort addr, List<bool> value, out List<byte> result) {
            Stat stt = Stat.SUCCESS;
            try {
                byte[] data = CalcByte(value);
                byte[] cmd = CreateWriteHeader(DeviceID, DeviceNode, addr, (ushort)value.Count, (byte)(data.Length + 2), ModbusFunction.FORCE_MULTI_COILS);

                int idx = 0;
                foreach (byte item in data) {
                    cmd[13 + idx] = item;
                    idx++;
                }
                SendData(cmd);
                WaitResponse();
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
            result = (mRxData != null) ? mRxData.ToList() : null;
            stt = mRxStt;
            return stt;
        }

        /// <summary>[FC16] 寫入連續 Register 數值  [Preset Multi Registers]</summary>
        /// <param name="addr">起始 Register Address</param>
        /// <param name="data">欲寫入的資料</param>
        /// <param name="result">回傳之結果</param>
        /// <returns>Status Code</returns>
        public Stat WriteMultiRegisters(ushort addr, List<ushort> data, out List<byte> result) {
            Stat stt = Stat.SUCCESS;
            try {
                byte[] cmd = CreateWriteHeader(DeviceID, DeviceNode, addr, (byte)(data.Count), (byte)(data.Count * 2 + 2), ModbusFunction.PRESET_MULTI_REGISTERS);

                int idx = 0;
                foreach (ushort value in data) {
                    byte[] _val = BitConverter.GetBytes(value);
                    cmd[13 + idx] = _val[1];
                    cmd[14 + idx] = _val[0];
                    idx += 2;
                }
                SendData(cmd);
                WaitResponse();
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
            result = (mRxData != null) ? mRxData.ToList() : null;
            stt = mRxStt;
            return stt;
        }
        #endregion

        #region Function - CtSyncSocket Override
        /// <summary>[Socket][Client] Socket 接收資料之處理執行緒(Thread)</summary>
        protected override void tsk_Client_RxData() {
            NetworkStream netStream = mClient.GetStream();
            List<byte> rxData = new List<byte>();
            do {
                try {
                    /*-- If mClient is dispose or disconnect, exit the loop --*/
                    if (mClient == null) break;

                    /*-- Read stream buffer into byte[] --*/
                    rxData.Clear();
                    do {
                        rxData.Add((byte)netStream.ReadByte());
                    } while (netStream.DataAvailable);

                    /*-- If there are something passing-in --*/
                    if (rxData.Count > 0) {
                        mRxStt = Decode(rxData);
                        mFlag_RxData = true;
                    }
                    /*-- Dalay --*/
                    Thread.Sleep(1);

                } catch (IOException) {
                    /*-- Raise disconnect event --*/
                    RaiseEvents(
                        new SocketEventArgs(
                                SocketEvents.CONNECTED_WITH_SERVER,
                                new ConnectInfo(
                                        false,
                                        DeviceIP,
                                        DevicePort
                                )
                        )
                    );
                    break;
                } catch (ThreadInterruptedException) {
                    /*-- Using to catch exception of exit thread, but not report --*/
                } catch (Exception ex) {
                    CtStatus.Report(Stat.ER_SYSTEM, ex);
                }
            } while (mThread_ClientReceiveData.IsAlive);
        }

        /// <summary>[Socket][Server] 各 Socket 接收資料之處理執行緒(Thread)</summary>
        /// <param name="socket">此執行緒欲負責接收的 Socket 物件</param>
        protected override void tsk_Server_RxData(object socket) {
            Socket sckTemp = socket as Socket;                      //Temporary Socket from listen thread
            NetworkStream netStream = new NetworkStream(sckTemp);   //Get the stream of the Socket that passing
            List<byte> rxData = new List<byte>();
            try {
                do {
                    try {
                        /*-- If server or socket disconnect, exit the loop --*/
                        if (mServer == null) break;

                        /*-- Create the byte[] to receive data --*/
                        rxData.Clear();
                        do {
                            rxData.Add((byte)netStream.ReadByte());
                        } while (netStream.DataAvailable);

                        /*-- If there are somehting exist --*/
                        if (rxData.Count > 0) {

                            mRxStt = Decode(rxData);
                            mFlag_RxData = true;

                            /*-- If there are no response from client, close the tube --*/
                        } else {
                            /*-- Remove the Socket from list --*/
                            mClientList.Remove(sckTemp);
                            sckTemp.Disconnect(true);

                            /*-- Raise Event --*/
                            RaiseEvents(
                                new SocketEventArgs(
                                    SocketEvents.CLIENT_CONNECTED,
                                    new ConnectInfo(
                                            false,
                                            (sckTemp.RemoteEndPoint as IPEndPoint).Address.ToString(),
                                            (sckTemp.RemoteEndPoint as IPEndPoint).Port
                                    )
                                )
                            );
                            break;
                        }

                        /*-- Delay --*/
                        Thread.Sleep(1);

                    } catch (IOException) {
                        /*-- Remove the Socket from list --*/
                        mClientList.Remove(sckTemp);
                        sckTemp.Disconnect(true);

                        /*-- Raise Event --*/
                        RaiseEvents(
                            new SocketEventArgs(
                                    SocketEvents.CLIENT_CONNECTED,
                                    new ConnectInfo(
                                            false,
                                            (sckTemp.RemoteEndPoint as IPEndPoint).Address.ToString(),
                                            (sckTemp.RemoteEndPoint as IPEndPoint).Port
                                    )
                            )
                        );
                        break;
                    } catch (Exception ex) {
                        CtStatus.Report(Stat.ER_SYSTEM, ex);
                    }
                } while (mThread_ServerListen.IsAlive);
            } catch (Exception) {
                /*-- Using to catch exception of exit thread --*/
            }
        }
        #endregion
    }
}
