using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using CtLib.Library;
using CtLib.Module.Net;
using CtLib.Module.Utility;

namespace CtLib.Module.UniversalRobot {
    /// <summary>
    /// Universal Robot 相關操作
    /// <para>透過 Socket 作為溝通管道與 UR 進行連接</para>
    /// <para>[Server]UR [Client]CASTEC</para>
    /// </summary>
    public class CtUrServer : ICtVersion {

        #region Version

        /// <summary>CtUrServer 版本相關訊息</summary>
        /// <remarks><code language="C#">
        /// 0.0.0  William [2015/04/16]
        ///     + CtUrServer
        ///     
        /// 1.0.0  Ahern [2015/05/01]
        ///     + Translate from VB
        ///     
        /// </code></remarks>
        public CtVersion Version { get { return new CtVersion(1, 0, 0, "2015/05/01", "Ahern Kuo"); } }

        #endregion

        #region Declaration - Enumerations
        /// <summary>UR 執行狀態</summary>
        public enum URRunStates : byte {
            /// <summary>執行中</summary>
            RUNNING,
            /// <summary>已停止</summary>
            STOPPED
        }

        /// <summary>UR 狀態模式</summary>
        public enum URModes : sbyte {
            /// <summary>尚未連接 UR Controller</summary>
            NO_CONTROLLER = -1,
            /// <summary>執行任務中</summary>
            RUNNING = 0,
            /// <summary>FreeDrive</summary>
            FREEDRIVE = 1,
            /// <summary>待命</summary>
            READY = 2,
            /// <summary>初始化</summary>
            INITIALIZING = 3,
            /// <summary>停止(安全性)</summary>
            SECURITY_STOPPED = 4,
            /// <summary>停止(緊急按鈕)</summary>
            EMERGENCY_STOPPED = 5,
            /// <summary>內部錯誤</summary>
            FAULT = 6,
            /// <summary>尚未 Servo ON</summary>
            NO_POWER = 7,
            /// <summary>尚未連接</summary>
            NOT_CONNECTED = 8,
            /// <summary>關機</summary>
            Shutdown = 9
        }

        /// <summary>UR 程式執行狀態</summary>
        public enum URProgramStates : byte {
            /// <summary>停止</summary>
            STOPPED,
            /// <summary>執行中</summary>
            PLAYING,
            /// <summary>暫停</summary>
            PAUSE
        }

        /// <summary>UR 控制命令</summary>
        public enum URCommand : byte {
            /// <summary>Custom Command</summary>
            UserInputCommand,
            /// <summary>Load</summary>
            load,
            /// <summary>Play</summary>
            play,
            /// <summary>Stop</summary>
            stop,
            /// <summary>Pause</summary>
            pause,
            /// <summary>Quit</summary>
            quit,
            /// <summary>Shutdown</summary>
            shutdown,
            /// <summary>Running</summary>
            running,
            /// <summary>Robot Mode</summary>
            robotmode,
            /// <summary>Get Loaded Program</summary>
            get_loaded_program,
            /// <summary>Pop-Up</summary>
            popup,
            /// <summary>Close Pop-Up</summary>
            close_popup,
            /// <summary>Add to Log</summary>
            addToLog,
            /// <summary>Set User Role</summary>
            setUserRole,
            /// <summary>Is Program Saved</summary>
            isprogramSaved,
            /// <summary>Program State</summary>
            programState,
            /// <summary>Poly Scope Version</summary>
            polyscopeVersion
        }
        #endregion

        #region Declaration - Definitions
        /// <summary>掃描 UR 狀態之執行緒名稱</summary>
        private static readonly string SCAN_THREAD_NAME = "CtUrServer_Scan";
        /// <summary>掃描 UR 狀態之執行緒每一循環之間的延遲時間(毫秒)</summary>
        private static readonly int SCAN_DELAY_TIME = 50;
        #endregion

        #region Declaration - Fields
        /// <summary>用於 Socket 連線之物件</summary>
        private CtTcpSocket mSocket;

        /// <summary>UR 命令集合</summary>
        private Dictionary<URCommand, string> mUrCommands = new Dictionary<URCommand, string>();

        /// <summary>用於佇列收到的 UR 命令</summary>
        private Queue<string> mQueue = new Queue<string>();
        /// <summary>[Thread] 掃描 UR 狀態</summary>
        private Thread mThread_ScanStt;
        #endregion

        #region Declaration - Properties
        /// <summary>取得當前 UR 模式</summary>
        public URModes UR_Mode { get; private set; }
        /// <summary>取得當前 UR 執行狀態</summary>
        public URRunStates UR_RunState { get; private set; }
        /// <summary>取得當前 UR 程式執行狀態</summary>
        public URProgramStates UR_ProgState { get; private set; }
        /// <summary>Polyscope Version</summary>
        public string PolyscopeVersion { get; private set; }

        /// <summary>與 UR 連接之網際網路位址</summary>
        public string UR_IP {
            get {
                return (mSocket != null && mSocket.IsConnected) ? mSocket.EndPoint.IP : "";
            }
        }

        /// <summary>與 UR 連接之埠號</summary>
        public int UR_Port {
            get {
                return (mSocket != null && mSocket.IsConnected) ? mSocket.EndPoint.Port : -1;
            }
        }

        /// <summary>取得當前載入程式之路徑</summary>
        public string UR_ProgPath { get; private set; }

        /// <summary>取得當前與 UR 之連接狀態</summary>
        public bool IsConnected {
            get { return (mSocket == null) ? false : mSocket.IsConnected; }
        }
        #endregion

        #region Declaration - Events
        /// <summary>CtUrServer 事件集合</summary>
        public enum UrEvents : byte {
            /// <summary>
            /// 與 UR 之 Socket 連線狀態改變
            /// <para>Value 參數為 Bool 型態 (<see langword="true"/>)連線  (<see langword="false"/>)中斷連線</para>
            /// </summary>
            Connection,
            /// <summary>
            /// UR 執行狀態改變
            /// <para>Value 參數為 <see cref="URRunStates"/> 型態</para>
            /// </summary>
            RunState,
            /// <summary>
            /// UR 模式改變
            /// <para>Value 參數為 <see cref="URModes"/> 型態</para>
            /// </summary>
            Mode,
            /// <summary>
            /// UR 程式狀態改變
            /// <para>Value 參數為 <see cref="URProgramStates"/> 型態</para>
            /// </summary>
            ProgramState,
            /// <summary>
            /// UR 載入新的程式
            /// <para>Value 參數為 String 型態。即為當前載入的程式名稱</para>
            /// </summary>
            LoadedProgram,
            /// <summary>
            /// UR 關機
            /// <para>Value 不具任何意義！</para>
            /// </summary>
            Shutdown,
            /// <summary>
            /// CtUrServer 發生錯誤
            /// <para>Value 參數為 String 型態。即為 Exception 之 Message</para>
            /// </summary>
            Exception,
            /// <summary>
            /// 從 UR 所接收到的資料
            /// <para>Value 參數為 String 型態</para>
            /// </summary>
            DataReceived
        }

        /// <summary>CtUrServer 事件參數</summary>
        public class UrEventArgs : EventArgs {
            /// <summary>事件</summary>
            public UrEvents Event { get; set; }
            /// <summary>此事件所附帶之數值</summary>
            public object Value { get; set; }
            /// <summary>建立事件參數</summary>
            /// <param name="events">事件</param>
            /// <param name="value">此事件所附帶之數值</param>
            public UrEventArgs(UrEvents events, object value) {
                Event = events;
                Value = value;
            }
        }

        /// <summary>CtUrServer 集合式事件</summary>
        public event EventHandler<UrEventArgs> OnUrEvents;

        /// <summary>觸發事件</summary>
        /// <param name="events">事件</param>
        /// <param name="value">此事件所附帶之數值</param>
        protected virtual void RaiseEvents(UrEvents events, object value) {
            EventHandler<UrEventArgs> handler = OnUrEvents;
            if (handler != null)
                handler(this, new UrEventArgs(events, value));

        }
        #endregion

        #region Function - Constructors
        /// <summary>建構全新的 CtUrServer。請自行施作連接</summary>
        public CtUrServer() {
            CreateCmdString();

			mSocket = new CtTcpSocket(TransDataFormats.String, true);
			mSocket.OnSocketEvents += mSocket_OnSocketEvents;
        }

        /// <summary>建構 CtUrServer，同時帶入 IP 與 Port</summary>
        /// <param name="ip">UR 連接之網際網路位址</param>
        /// <param name="port">UR 連接之埠號</param>
        /// <param name="autoConnect">是否建構後直接連線？  (<see langword="true"/>)直接連線 (<see langword="false"/>)後續再自行連接</param>
        public CtUrServer(string ip, int port, bool autoConnect = false) {
            CreateCmdString();

			mSocket = new CtTcpSocket(TransDataFormats.String, true);
			mSocket.OnSocketEvents += mSocket_OnSocketEvents;
            if (autoConnect) mSocket.ClientConnect(ip, port);
        }

        #endregion

        #region Function - Methods
        /// <summary>統一發報 Exception</summary>
        /// <param name="stt">Status Code</param>
        /// <param name="ex">由系統收集之例外訊息。請參考 <see cref="Exception"/></param>
        private void ExceptionHandler(Stat stt, Exception ex) {
            string title = "";
            CtStatus.Report(stt, ex, out title);
            RaiseEvents(UrEvents.Exception, "[" + title + "] " + ex.Message);
        }

        /// <summary>建立 UR 命令集合</summary>
        private void CreateCmdString() {
            foreach (URCommand item in Enum.GetValues(typeof(URCommand))) {
                mUrCommands.Add(item, Enum.GetName(typeof(URCommand), item).Replace("_", " ").Trim());
            }
        }

        #endregion

        #region Function - Core
        /// <summary>嘗試與 UR 進行連線</summary>
        /// <param name="ip">欲連線之 UR 網際網路位址</param>
        /// <param name="port">欲連線之 UR 埠號</param>
        public void Connect(string ip, int port) {
            if (mSocket != null) {
                mSocket.ClientConnect(ip, port);
            }
        }

        /// <summary>中斷與 UR 之連線</summary>
        public void Disconnect() {
            if (mSocket != null && mSocket.IsConnected) {
                mSocket.ClientDisconnect();
            }
        }

        /// <summary>傳送 UR 命令</summary>
        /// <param name="cmd">UR 命令</param>
        /// <param name="parameter">參數</param>
        public virtual void SendCommand(URCommand cmd, string parameter = "") {
            if (mSocket.IsConnected) {
                if (cmd == URCommand.load || cmd == URCommand.popup) {
                    if (parameter == "") {
                        Exception ex = new Exception("Send command but contains invalid parameter");
                        ExceptionHandler(Stat.ER_SYS_INVARG, ex);
                    } else mSocket.Send(mUrCommands[cmd] + " " + parameter);
                } else if (cmd == URCommand.UserInputCommand)
                    mSocket.Send(parameter);
                else mSocket.Send(mUrCommands[cmd]);
            }
        }
        #endregion

        #region Function - Events

        private void mSocket_OnSocketEvents(object sender, SocketEventArgs e) {
            switch (e.Event) {
                case SocketEvents.ConnectionWithServer:
                    if ((e.Value as SocketConnection).Status)
                        CtThread.CreateThread(ref mThread_ScanStt, SCAN_THREAD_NAME, tsk_ScanUrStt);
                    else CtThread.KillThread(ref mThread_ScanStt);
                    RaiseEvents(UrEvents.Connection, (e.Value as SocketConnection).Status);
                    break;
                case SocketEvents.Exception:
                    break;
                case SocketEvents.DataReceived:
                    CtThread.AddWorkItem(tsk_AddQueue, (e.Value as SocketRxData).Data);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Function - Threads
        /// <summary>將收到的 UR 訊息加入 Queue 裡，並將收到的資料向外拋出</summary>
        /// <param name="obj">收到的訊息</param>
        protected virtual void tsk_AddQueue(object obj) {
            string data = obj.ToString();
            if (data.Length > 0) {
                lock (mQueue) {
                    mQueue.Enqueue(data.Trim());
                }
                RaiseEvents(UrEvents.DataReceived, data);
            }
        }

        /// <summary>[Thread] 掃描 UR 狀態</summary>
        protected virtual void tsk_ScanUrStt() {
            string strQueue = "";
            string strTemp = "";
            string[] strSplit;
            string[] strData;
            do {
                try {

                    lock (mQueue) {
                        if (mQueue.Count > 0) strQueue = mQueue.Dequeue();
                        else strQueue = "";
                    }

                    if (strQueue != "") {
                        strSplit = strQueue.Split(CtConst.CHR_CRLF, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string dataReceived in strSplit) {
                            strData = dataReceived.Split(CtConst.CHR_COLON, StringSplitOptions.RemoveEmptyEntries).Select(val => val.ToLower().Trim()).ToArray();

                            if (strData.Length == 1)
                                strData = dataReceived.Split(CtConst.CHR_SPACE, StringSplitOptions.RemoveEmptyEntries).Select(val => val.ToLower().Trim()).ToArray();

                            strTemp = strData[0];
                            switch (strTemp) {
                                case "connected":
                                    RaiseEvents(UrEvents.Connection, true);
                                    break;
                                case "loading program":
                                    break;
                                case "program running":
                                    UR_RunState = strData[1] == "false" ? URRunStates.STOPPED : URRunStates.RUNNING;
                                    RaiseEvents(UrEvents.RunState, UR_RunState);
                                    break;
                                case "robotmode":
                                    if (strData[1] == "running")
                                        RaiseEvents(UrEvents.Mode, URModes.RUNNING);
                                    else
                                        RaiseEvents(UrEvents.Mode, URModes.FAULT);
                                    break;
                                case "loaded program":
                                    UR_ProgPath = strData[1];
                                    break;
                                case "showing":
                                    break;
                                case "closing":
                                    break;
                                case "stopped":
                                    if (strData.Length > 1) {
                                        UR_ProgState = URProgramStates.STOPPED;
                                        RaiseEvents(UrEvents.ProgramState, UR_ProgState);
                                    }
                                    break;
                                case "staring":
                                    break;
                                case "paused":
                                    if (strData.Length > 1) {
                                        UR_ProgState = URProgramStates.PAUSE;
                                        RaiseEvents(UrEvents.ProgramState, UR_ProgState);
                                    }
                                    break;
                                case "playing":
                                    if (strData.Length > 1) {
                                        UR_ProgState = URProgramStates.PLAYING;
                                        RaiseEvents(UrEvents.ProgramState, UR_ProgState);
                                        RaiseEvents(UrEvents.LoadedProgram, strData[1]);
                                    }
                                    break;
                                case "shutting":
                                    RaiseEvents(UrEvents.Shutdown, false);
                                    break;
                                case "no":
                                    if (strQueue.Trim() == "no program loaded")
                                        UR_ProgPath = "";
                                    break;
                                case "starting":
                                    if (strQueue.Trim() == "starting program") {

                                    }
                                    break;
                                default:
                                    throw (new Exception("Unknown command received. String = " + strQueue.Trim()));
                            }
                        }
                    }

                    CtTimer.Delay(SCAN_DELAY_TIME);

                } catch (ThreadInterruptedException) {
                    /*-- Using to catch exception of exit thread, but not report --*/
                } catch (ThreadAbortException) {
                    /*-- Using to catch exception of exit thread, but not report --*/
                } catch (Exception ex) {
                    ExceptionHandler(Stat.ER_SYSTEM, ex);
                }
            } while (mThread_ScanStt.IsAlive);
        }
        #endregion
    }
}
