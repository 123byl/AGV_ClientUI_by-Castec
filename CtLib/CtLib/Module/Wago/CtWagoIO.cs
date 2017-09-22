using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Modbus;
using CtLib.Module.TCPIP;

namespace CtLib.Module.Wago {

    /// <summary>
    /// Wago IO 模組控制
    /// <para>此類別以 Wago 750-352 為基礎撰寫</para>
    /// </summary>
    public class CtWagoIO {

        #region Declaration - Enumerations
        /// <summary>輸入輸出(Input_Output, IO) 類型</summary>
        public enum IOType : byte {
            /// <summary>輸入 Input</summary>
            INPUT,
            /// <summary>輸出 Output</summary>
            OUTPUT,
            /// <summary>輸入輸出 InOut</summary>
            INOUT
        }
        #endregion

        #region Declaration - Members
        /// <summary>Modbus TCP Object</summary>
        private CtModbus_TCP mModbus;
        #endregion

        #region Declaration - Properties
        /// <summary>Wago Modbus TCP - IP</summary>
        public string ModbusIP { get; set; }
        /// <summary>Wago Modbus TCP - Port</summary>
        public int ModbusPort { get; set; }

        /// <summary>取的目前是否已連接至裝置</summary>
        public bool IsConnect {
            get {
                return (mModbus != null) ? mModbus.IsConnected : false;
            }
        }
        #endregion

        #region Declaration - Events
        /// <summary>CtWagoIO 事件集合</summary>
        public enum WagoIOEvents : byte {
            /// <summary>
            /// 與 Slave/Master 連線狀態改變
            /// <para>事件附加的數值型態為 bool</para>
            /// </summary>
            CONNECTION,
            /// <summary>
            /// Socket 連線通訊異常
            /// <para>事件附加的數值型態為 string</para>
            /// </summary>
            EXCEPTION
        }

        /// <summary>CtWagoIO 事件參數</summary>
        public class WagoIOEventArgs : EventArgs {
            /// <summary>事件</summary>
            public WagoIOEvents Event { get; set; }
            /// <summary>此事件所附帶之數值</summary>
            public object Value { get; set; }
            /// <summary>建立事件參數</summary>
            /// <param name="events">事件</param>
            /// <param name="value">此事件所附帶之數值</param>
            public WagoIOEventArgs(WagoIOEvents events, object value) {
                Event = events;
                Value = value;
            }
        }

        /// <summary>CtWagoIO 集合式事件</summary>
        public event EventHandler<WagoIOEventArgs> OnWagoEvents;

        /// <summary>觸發事件</summary>
        /// <param name="e">Event Arguments</param>
        protected virtual void RaiseEvents(WagoIOEventArgs e) {
            EventHandler<WagoIOEventArgs> handler = OnWagoEvents;
            if (handler != null) handler(this, e);

        }
        #endregion

        #region Function - Constructors
        /// <summary>建立 CtWagoIO。請於連線時帶入 Wago IP與Port</summary>
        public CtWagoIO() {
            mModbus = new CtModbus_TCP();
            mModbus.OnSocketEvents += mModbus_OnSocketEvents;
        }

        /// <summary>建立 CtWagoIO。帶入 Wago IP 與 Port 並直接執行連線動作</summary>
        /// <param name="ip">Wago Modbus TCP 之 IP。  如 "192.168.1.1"</param>
        /// <param name="port">Wago Modbus TCP 之 Port。 如 502</param>
        public CtWagoIO(string ip, int port) {

            ModbusIP = ip;
            ModbusPort = port;

            mModbus = new CtModbus_TCP(ip, port, true);
            mModbus.OnSocketEvents += mModbus_OnSocketEvents;

        }
        #endregion

        #region Function - CtModbus_TCP Events
        void mModbus_OnSocketEvents(object sender, CtSyncSocket.SocketEventArgs e) {
            switch (e.Event) {
                case CtSyncSocket.SocketEvents.CONNECTED_WITH_SERVER:
                    RaiseEvents(new WagoIOEventArgs(WagoIOEvents.CONNECTION, (e.Value as CtSyncSocket.ConnectInfo).Status));
                    break;
                case CtSyncSocket.SocketEvents.EXCEPTION:
                    RaiseEvents(new WagoIOEventArgs(WagoIOEvents.EXCEPTION, e.Value.ToString()));
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Function - Connections

        /// <summary>執行連線動作</summary>
        /// <param name="ip">Wago Modbus TCP 之 IP</param>
        /// <param name="port">Wago Modbus TCP 之 Port</param>
        public void Connect(string ip, int port) {
            ModbusIP = ip;
            ModbusPort = port;
            if (mModbus == null) mModbus = new CtModbus_TCP();      //如果尚未建立則先建立
            if (mModbus.IsConnected) mModbus.ClientDisconnect();    //如是重新連線，則先進行斷線再行連線
            mModbus.ClientConnect(ip, port);                        //連線
        }

        /// <summary>
        /// 執行連線動作。採用 ModbusIP 與 ModbusPort 屬性來進行連線
        /// <para>執行此動作前請先確認已有設定 IP 與 Port 兩屬性！</para>
        /// </summary>
        public void Connect() {
            Connect(ModbusIP, ModbusPort);
        }

        /// <summary>斷開連線</summary>
        public void Disconnect() {
            if (mModbus.IsConnected) mModbus.ClientDisconnect();
        }
        #endregion

        #region Function - Get I/O

        /// <summary>取得單點 I/O 之狀態，並直接回傳</summary>
        /// <param name="io">
        /// I/O 編號
        /// <para>以 Wago 750-352 為例， Input: 0~255,24576~25339  Output: 512~767,28672~29435。 順序從第一張卡第一點開始算起</para>
        /// </param>
        /// <returns>當前狀態</returns>
        public bool GetIO(ushort io) {
            List<bool> bolTemp = null;
            mModbus.ReadCoilStatus(io, 1, out bolTemp);
            return (bolTemp == null) ? false : bolTemp[0];
        }

        /// <summary>取得單點 I/O 之狀態</summary>
        /// <param name="io">
        /// I/O 編號
        /// <para>以 Wago 750-352 為例， Input: 0~255,24576~25339  Output: 512~767,28672~29435。 順序從第一張卡第一點開始算起</para>
        /// </param>
        /// <param name="state">當前狀態  (True)ON (False)OFF</param>
        /// <returns>Status Code</returns>
        public Stat GetIO(ushort io, out bool state) {
            List<bool> bolTemp = null;
            Stat stt = mModbus.ReadCoilStatus(io, 1, out bolTemp);
            state = (bolTemp == null) ? false : bolTemp[0];
            return stt;
        }

        /// <summary>取得分散的單點 I/O 之狀態。 如 3,5,8,15 等</summary>
        /// <param name="io">
        /// I/O 編號
        /// <para>以 Wago 750-352 為例， Input: 0~255,24576~25339  Output: 512~767,28672~29435。 順序從第一張卡第一點開始算起</para>
        /// </param>
        /// <returns>Status Code</returns>
        public List<bool> GetIO(params ushort[] io) {
            List<bool> bolTemp = new List<bool>();
            foreach (ushort item in io) {
                bolTemp.Add(GetIO(item));
            }
            return bolTemp.ToList();
        }

        /// <summary>取得連續性 I/O 之狀態。 如從 8 開始讀取 7bit，即 IO(8) Length(7)</summary>
        /// <param name="io">
        /// 起始 I/O 編號
        /// <para>以 Wago 750-352 為例， Input: 0~255,24576~25339  Output: 512~767,28672~29435。 順序從第一張卡第一點開始算起</para>
        /// </param>
        /// <param name="length">欲連續讀取多少 bit</param>
        /// <param name="value">回傳的各點狀態</param>
        /// <returns>Status Code</returns>
        public Stat GetIO(ushort io, ushort length, out List<bool> value) {
            List<bool> bolTemp = null;
            Stat stt = mModbus.ReadCoilStatus(io, length, out bolTemp);
            value = (bolTemp == null) ? null : bolTemp.ToList();
            return stt;
        }

        /// <summary>取得單一暫存器當前數值</summary>
        /// <param name="reg">
        /// 暫存器編號
        /// <para>以 Wago 750-352 為例， Input: 0~255,24576~25339  Output: 512~767,28672~29435。 順序從第一張卡第一點開始算起</para>
        /// </param>
        /// <param name="value">回傳的當前數值</param>
        /// <returns>Status Code</returns>
        public Stat GetRegister(ushort reg, out ushort value) {
            List<byte> usTemp = null;
            Stat stt = mModbus.ReadHoldingRegister(reg, 1, out usTemp);
            value = (ushort)(((usTemp == null) || (usTemp.Count != 2)) ? 0 : usTemp[0] * 256 + usTemp[1]);
            return stt;
        }

        /// <summary>取得連續多個暫存器數值</summary>
        /// <param name="reg">
        /// 起始暫存器編號
        /// <para>以 Wago 750-352 為例， Input: 0~255,24576~25339  Output: 512~767,28672~29435。 順序從第一張卡第一點開始算起</para>
        /// </param>
        /// <param name="length">欲取得連續多少暫存器數量</param>
        /// <param name="value">回傳的數值</param>
        /// <returns>Status Code</returns>
        public Stat GetRegister(ushort reg, ushort length, out List<ushort> value) {
            List<byte> bytTemp = null;
            List<ushort> usTemp = null;
            Stat stt = mModbus.ReadHoldingRegister(reg, length, out bytTemp);
            if ((bytTemp != null) && (bytTemp.Count % 2 == 0)) {
                usTemp = new List<ushort>();
                for (int idx = 0; idx < bytTemp.Count; idx += 2) {
                    usTemp.Add((ushort)(bytTemp[idx] * 256 + bytTemp[idx + 1]));
                }
            }
            value = usTemp;
            return stt;
        }
        #endregion

        #region Function - Set I/O

        /// <summary>設定單點 I/O 狀態。 ON/OFF 狀態直接採數字控制，如 (3) = ON , (-3) = OFF</summary>
        /// <param name="io">
        /// I/O 編號
        /// <para>以 Wago 750-352 為例， Output: 0~2040。 順序從第一張卡第一點開始算起</para>
        /// </param>
        /// <returns>Status Code</returns>
        public Stat SetIO(short io) {
            List<byte> result;
            ushort usIO = (ushort)((io > -1) ? io : io * -1);
            bool bolVal = (io > -1) ? true : false;
            Stat stt = mModbus.WriteSingleCoil(usIO, bolVal, out result);
            if (stt == Stat.SUCCESS) {
                if ((bolVal && result[0] != 0xFF) || (!bolVal && result[0] != 0x00))
                    stt = Stat.ER3_WGO_WRITIO;
            }
            return stt;
        }

        /// <summary>設定單點 I/O 狀態</summary>
        /// <param name="io">
        /// I/O 編號
        /// <para>以 Wago 750-352 為例， Output: 0~2040。 順序從第一張卡第一點開始算起</para>
        /// </param>
        /// <param name="state">欲更改的狀態  (True)ON (False)OFF</param>
        /// <returns>Status Code</returns>
        public Stat SetIO(ushort io, bool state) {
            List<byte> result;
            Stat stt = mModbus.WriteSingleCoil(io, state, out result);
            if (stt == Stat.SUCCESS) {
                if ((state && result[0] != 0xFF) || (!state && result[0] != 0))
                    stt = Stat.ER3_WGO_WRITIO;
            }
            return stt;
        }

        /// <summary>設定分散的單點 I/O 狀態。 ON/OFF 狀態直接採數字控制，如 (3) = ON , (-3) = OFF</summary>
        /// <param name="io">
        /// I/O 編號，如 3,5,-8,10,-15
        /// <para>以 Wago 750-352 為例， Output: 0~2040。 順序從第一張卡第一點開始算起</para>
        /// </param>
        /// <returns>Status Code</returns>
        public Stat SetIO(params short[] io) {
            Stat stt = Stat.SUCCESS;
            List<byte> retTemp;
            ushort usIO = 0;
            bool bolVal = false;

            foreach (short value in io) {
                usIO = (ushort)((value > -1) ? value : value * -1);
                bolVal = (value > -1) ? true : false;
                stt = mModbus.WriteSingleCoil(usIO, bolVal, out retTemp);
                if ((retTemp == null) || (bolVal && retTemp[0] != 0xFF) || (!bolVal && retTemp[0] != 0))
                    stt = Stat.ER3_WGO_WRITIO;
            }

            return stt;
        }

        /// <summary>設定連續多點 I/O 狀態</summary>
        /// <param name="io">
        /// 起始 I/O 編號
        /// <para>以 Wago 750-352 為例， Output: 0~2040。 順序從第一張卡第一點開始算起</para>
        /// </param>
        /// <param name="value">從起始 I/O 開始欲寫入的狀態，如要連續更改 3 個，可帶入 { true, false, true }</param>
        /// <returns>Status Code</returns>
        public Stat SetIO(ushort io, List<bool> value) {
            List<byte> result;
            Stat stt = Stat.SUCCESS;
            stt = mModbus.WriteMultiCoils(io, value, out result);
            if (stt == Stat.SUCCESS) {
                if (value.Count != (result[0] * 256 + result[1]))
                    stt = Stat.ER3_WGO_WRITIO;
            }
            return stt;
        }

        /// <summary>設定單一暫存器資料</summary>
        /// <param name="reg">
        /// 暫存器編號
        /// <para>以 Wago 750-352 為例， Input: 0~255,24576~25339  Output: 512~767,28672~29435。 順序從第一張卡第一點開始算起</para>
        /// </param>
        /// <param name="value">欲寫入的資料</param>
        /// <returns>Status Code</returns>
        public Stat SetRegister(ushort reg, ushort value) {
            List<byte> result;
            Stat stt = Stat.SUCCESS;
            stt = mModbus.WriteSingleRegister(reg, value, out result);
            if (stt == Stat.SUCCESS) {
                if (value != (result[0] * 256 + result[1]))
                    stt = Stat.ER3_WGO_WRITIO;
            }
            return stt;
        }

        /// <summary>設定連續多個暫存器數值</summary>
        /// <param name="reg">
        /// 起始暫存器編號
        /// <para>以 Wago 750-352 為例， Input: 0~255,24576~25339  Output: 512~767,28672~29435。 順序從第一張卡第一點開始算起</para>
        /// </param>
        /// <param name="value">從起始 I/O 開始欲寫入的狀態，如要連續更改 3 個，可帶入 { 0xFFFF, 0x0D03, 0x040C }</param>
        /// <returns>Status Code</returns>
        public Stat SetRegister(ushort reg, List<ushort> value) {
            List<byte> result;
            Stat stt = Stat.SUCCESS;
            stt = mModbus.WriteMultiRegisters(reg, value, out result);
            if (stt == Stat.SUCCESS) {
                if (value.Count != (result[0] * 256 + result[1]))
                    stt = Stat.ER3_WGO_WRITIO;
            }
            return stt;
        }
        #endregion

    }
}
