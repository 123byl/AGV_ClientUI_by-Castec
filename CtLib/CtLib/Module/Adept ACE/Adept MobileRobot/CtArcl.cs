using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using CtLib.Library;
using CtLib.Module.SerialPort;
using CtLib.Module.Net;
using CtLib.Module.Utility;

namespace CtLib.Module.Adept {
    /// <summary>
    /// 透過 Serial port 底層與 Mobile robot 連結
    /// <para>組合及解析 ARCL 指令讀取及控制 Mobile robot</para>
    /// </summary>
    public class CtArcl : IDisposable, ICtVersion {

        #region Version

        /// <summary>CtArcl 版本訊息</summary>
        /// <remarks><code language="C#">
        /// 0.0.0  William [2012/09/07]
        ///     + CtArcl.vb
        ///      
        /// 1.0.0  Ahern [2015/03/12]
        ///     \ 從 VB.Net 轉移至 C#
        /// 
        /// </code></remarks>
        public CtVersion Version { get { return new CtVersion(1, 0, 0, "2015/03/12", "William Huang"); } }

        #endregion

        #region Declaration - Definitions
        /// <summary>預設掃瞄 SerialPort 執行緒名稱</summary>
        private static readonly string THREAD_SCAN_NAME = "CtArcl_ScanMRStt";
        /// <summary>預設每次掃瞄 MR 之狀態間隔 （毫秒）</summary>
        private static readonly int THREAD_SCAN_DELAY = 300;
        ///// <summary>預設平均 MobileRobot 移動速度，用來計算移動時間</summary>
        //private static readonly int DEFAULT_MOVE_SPEED = 500;
        /// <summary>預設 Socket Client 監聽的 IP</summary>
        private static readonly string DEFAULT_SOCKET_IP = "1.2.3.4";
        /// <summary>預設 Socket Client 監聽的 Port</summary>
        private static readonly int DEFAULT_SOCKET_PORT = 6000;
        /// <summary>儲存未知命令之檔案名稱</summary>
        private static readonly string FILENAME_UNKNOWN_COMMANDS = "UnknownCommands.log";
        #endregion

        #region Declaration - Enumerations
        /// <summary>ARCL 命令</summary>
        public enum ArclCommand : byte {
            /// <summary>User Custom Commands</summary>
            UNKNOW = 0,
            ///// <summary></summary>
            //ANALOGINPUTLIST,
            ///// <summary></summary>
            //ANALOGINPUTQUERYRAW,
            ///// <summary></summary>
            //ANALOGINPUTQUERYVOLTAGE,
            ///// <summary></summary>
            //CLEARALLOBSTACLES,
            ///// <summary>&lt;HOST / IP&gt; &lt;PORT&gt; 使用 TCP 對外連線（目前不清楚用途）</summary>
            //CONNECTOUTGOING,
            ///// <summary></summary>
            //CONFIGADD,
            ///// <summary></summary>
            //CONFIGPARSE,
            ///// <summary></summary>
            //CONFIGSTART,
            ///// <summary></summary>
            //CREATEINFO,
            ///// <summary></summary>
            //CUSTOMREADINGADD,
            ///// <summary></summary>
            //CUSTOMREADINGADDABSOLUTE,
            ///// <summary></summary>
            //CUSTOMREADINGCLEAR,

            /// <summary>&lt;出發點 GOAL&gt; &lt;目的地 GOAL&gt;計算二個 GOAL 間移動距離，因消耗CPU，避免移動過程中使用此指令</summary>
            DISTANCEBETWEEN,
            /// <summary>前往充電站充電</summary>
            DOCK,
            /// <summary>&lt;TASK NAME&gt; 執行 TASK</summary>
            DOTASK,
            ///// <summary></summary>
            //DOTASKINSTANT,
            /// <summary>&lt;OPTIONAL ON/OFF&gt; 設定指令是否回音，無參數為查詢目前是否指令回音狀態</summary>
            ECHO,
            /// <summary>當觸發 E-STOP 解除後，使用此指令恢復 MOTORS 功能</summary>
            ENABLEMOTORS,
            /// <summary>偵測目前離目的地還需多少時間(S)及距離(MM)</summary>
            ETAREQUEST,
            /// <summary>&lt;MARCRO NAME&gt; 執行 MACRO 指令</summary>
            EXECUTEMACRO,
            ///// <summary></summary>
            //GETCONFIGSECTIONINFO,
            ///// <summary></summary>
            //GETCONFIGSECTIONLIST,
            ///// <summary></summary>
            //GETCONFIGSECTIONVALUES,
            /// <summary>開啟跟隨模式</summary>
            FOLLOW,
            /// <summary>取得目前地圖中所有 GOAL 名稱</summary>
            GETGOALS,
            ///// <summary></summary>
            //GETINFO,
            ///// <summary></summary>
            //GETINFOLIST,
            /// <summary>取得目前地圖中所有 MACRO 名稱</summary>       
            GETMACROS,
            /// <summary>顯示目前 PAYLOAD 訊息</summary>
            GETPAYLOAD,
            ///// <summary>查詢優先權值在多台MR時才可使用，數值越小優先權越高</summary>
            //GETPRECEDENCE,
            /// <summary>取得目前地圖中所有 ROUTE 名稱</summary>
            GETROUTES,
            /// <summary>&lt;GOAL NAME&gt; 命令 MR 前往該 GOAL 位置</summary>
            GOTO,
            /// <summary>&lt;X 座標&gt; &lt;Y 座標&gt; &lt;HEADING 角度&gt; 命令 MR 前往該座標位置</summary>
            GOTOPOINT,
            /// <summary>&lt;ROUTE NAME&gt; &lt;GOAL NAME&gt; 直接移動到 ROTUE 中的 GOAL 目標</summary>
            GOTOROUTEGOAL,
            /// <summary>列出所有指令</summary>
            HELP,
            /// <summary>查詢所有 INPUT 名稱 </summary>
            INPUTLIST,
            /// <summary>&lt;INPUT NAME&gt; 查詢此 INPUT 狀態</summary>
            INPUTQUERY,
            /// <summary>&lt;TASK NAME&gt; 新增 TASK 至 LIST 中</summary>
            LISTADD,
            /// <summary>執行現有的 LIST</summary>
            LISTEXECUTE,
            /// <summary>清除並重新開始新增 TASK</summary>
            LISTSTART,
            /// <summary>&lt;X 座標&gt; &lt;Y 座標&gt; &lt;HEADING 角度&gt; 以座標作 LOCALIZE</summary>
            LOCALIZETOPOINT,
            ///// <summary></summary>
            //NEWCONFIGPARAM,
            ///// <summary></summary>
            //NEWCONFIGSECTIONCOMMENT,
            /// <summary>查詢 MR 總行走距離</summary>
            ODOMETER,
            /// <summary>清除 MR 總行走距離紀錄</summary>
            ODOMETERRESET,
            /// <summary>查詢 MR 狀態，以一行的方式輸出</summary>
            ONELINESTATUS,
            /// <summary>查詢所有 OUTPUT 名稱</summary>
            OUTPUTLIST,
            /// <summary>&lt;OUTPUT NAME&gt; 將此IO輸出關閉</summary>
            OUTPUTOFF,
            /// <summary>&lt;OUTPUT NAME&gt; 將此IO輸出開啟</summary>
            OUTPUTON,
            /// <summary>&lt;OUTPUT NAME&gt; 查詢此 INPUT 狀態</summary>
            OUTPUTQUERY,
            /// <summary>循環執行 ROUTE</summary>
            PATROL,
            /// <summary>&lt;ROUTE NAME&gt; &lt;OPTIONAL INDEX&gt; 執行一次 ROUTE 若有 INDEX 則是從 ROUTE 中的第 INDEX 個 TASK 開始執行</summary>
            PATROLONCE,
            /// <summary>&lt;WAV檔路徑&gt; 播放 MT400 裡儲存的 .WAV 檔</summary>
            PLAY,
            /// <summary>&lt;TITLE&gt; &lt;MESSAGE&gt; &lt;BUTTON LABEL&gt; &lt;TIMEOUT&gt; 在 CLIENT 端顯示訊息視窗，TIMEOUT後會自行關閉</summary>
            POPUPSIMPLE,
            /// <summary>查詢目前充電相關狀態</summary>
            QUERYDOCKSTATUS,
            /// <summary>查詢目前馬達狀態</summary>
            QUERYMOTORS,
            /// <summary>切斷 ARCL 連線</summary>
            QUIT,
            ///// <summary></summary>
            //RANGEDEVICEGETCUMULATIVE,
            ///// <summary></summary>
            //RANGEDEVICEGETCURRENT,
            ///// <summary></summary>
            //RANGEDEVICEGETLIST,
            /// <summary>&lt;要發聲的字串&gt; 利用 MT400 本身發出聲音</summary>
            SAY,
            /// <summary>&lt;GOAL NAME&gt; &lt;描述&gt; 在掃瞄地圖過程中，加入 GOAL 點</summary>
            SCANADDGOAL,
            /// <summary>&lt;INFO&gt;</summary>
            SCANADDINFO,
            /// <summary>&lt;TAG&gt;</summary>
            SCANADDTAG,
            /// <summary>&lt;檔名&gt; 開始掃瞄地圖</summary>
            SCANSTART,
            /// <summary>停止掃瞄地圖，並將掃瞄結果存成 .2D 檔</summary>
            SCANSTOP,
            /// <summary>設定 PAYLOAD INFO</summary>
            SETPAYLOAD,
            ///// <summary>&lt;優先權值&gt; 設定優先權值，在多台MR時才可使用，數值越小優先權越高</summary>
            //SETPRECEDENCE,
            /// <summary>關閉 MOBLIE ROBOT</summary>
            SHUTDOWN,
            /// <summary>關閉 ARCL SERVER</summary>
            SHUTDOWNSERVER,
            /// <summary>查詢 MR 狀態</summary>
            STATUS,
            /// <summary>停止 MR 目前執行動作</summary>
            STOP,
            ///// <summary></summary>
            //TRACKSECTORS,
            ///// <summary></summary>
            //TRACKSECTORSATGOAL,
            ///// <summary></summary>
            //TRACKSECTORSATPOINT,
            ///// <summary></summary>
            //TRACKSECTORSPATH,
            ///// <summary></summary>
            //UCCOM,
            ///// <summary></summary>
            //UCPOSES,
            ///// <summary></summary>
            //UCVELS,
            /// <summary>退出充電站</summary>
            UNDOCK,
            ///// <summary></summary>
            //UPDATEINFO,
            /// <summary>額外使用 TELNET 登入指令，不包含在原始的ARCL指令內</summary>
            CUSTOMLOGIN

        }
        #endregion

        #region Declaration - Support Class
        /// <summary>ARCL 命令集合</summary>
        public class ArclCommandSet {
            /// <summary>ARCL 命令</summary>
            public ArclCommand ArclCmd { get; set; }
            /// <summary>命令參數</summary>
            public string CmdPara { get; set; }
            /// <summary>建立 ARCL 命令集合</summary>
            public ArclCommandSet() { }
            /// <summary>建立 ARCL 命令集合</summary>
            /// <param name="arclCmd">ARCL 命令</param>
            /// <param name="parameters">命令參數</param>
            public ArclCommandSet(ArclCommand arclCmd, string parameters) {
                ArclCmd = arclCmd;
                CmdPara = parameters;
            }
        }
        #endregion

        #region Declaration - Events
        /// <summary>CtArcl 集合式事件</summary>
        public enum ArclEvents : byte {
            /// <summary>
            /// 與 ARCL 之連線狀態改變
            /// <para>回傳數值型態為 bool  (<see langword="true"/>)連接 (<see langword="false"/>)斷線</para>
            /// </summary>
            SOCKET_CONNECTION,
            /// <summary>接收到命令</summary>
            RECEIVE_CMD,
            /// <summary>
            /// 充電狀態
            /// <para>回傳數值型態為 int</para>
            /// </summary>
            STATE_OF_CHARGE,
            /// <summary>
            /// 位置比對分數
            /// <para>回傳數值型態為 double</para>
            /// </summary>
            LOCALIZATION_SCORE,
            /// <summary>
            /// 溫度
            /// <para>數值回傳型態為 int</para>
            /// </summary>
            TEMPERATURE,
            /// <summary>
            /// Extended Status
            /// <para>回傳數值型態為 string</para>
            /// </summary>
            EXTENDED_STATUS,
            /// <summary>
            /// 位置更改
            /// <para>數值回傳型態為 <see cref="CtMobileStatus.LocationXYT"/></para>
            /// </summary>
            LOCATION,
            /// <summary>
            /// Dock 狀態
            /// <para>回傳資料型態為 <see cref="CtMobileStatus.DockingState"/></para>
            /// </summary>
            DOCKING_STATE,
            /// <summary>
            /// 強制狀態
            /// <para>回傳資料型態為 <see cref="CtMobileStatus.ForcedState"/></para>
            /// </summary>
            FORCED_STATE,
            /// <summary>
            /// 充電狀態
            /// <para>回傳資料型態為 <see cref="CtMobileStatus.ChargeState"/></para>
            /// </summary>
            CHARGE_STATE,
            /// <summary>
            /// 到達站點
            /// <para>回傳資料型態為 string</para>
            /// </summary>
            ARRIVED_GOAL,
            /// <summary>
            /// Motion 狀態
            /// <para>資料回傳型態為 string</para>
            /// </summary>
            MOTION_STATUS,
            /// <summary>
            /// 更新目標
            /// <para>回傳資料型態為 List&lt;string&gt; 並已使用 ToList()</para>
            /// </summary>
            UPDATE_GOALS,
            /// <summary>
            /// 更新路徑
            /// <para>回傳資料型態為 List&lt;string&gt; 並已使用 ToList()</para>
            /// </summary>
            UPDATE_ROUTES,
            /// <summary>
            /// 更新巨集
            /// <para>回傳資料型態為 List&lt;string&gt; 並已使用 ToList()</para>
            /// </summary>
            UPDATE_MACROS,
            /// <summary>
            /// End of Goals
            /// 通知用，回傳資料請忽略!
            /// </summary>
            GET_GOALS_END,
            /// <summary>
            /// End of Routes
            /// 通知用，回傳資料請忽略!
            /// </summary>
            GET_ROUTE_END,
            /// <summary>
            /// End of Macro
            /// 通知用，回傳資料請忽略!
            /// </summary>
            GET_MACRO_END,
            /// <summary>
            /// Patrol Completed
            /// <para>資料回傳型態為 string</para>
            /// </summary>
            COMPLETE_PATROL,
            /// <summary>
            /// Macro Completed
            /// <para>資料回傳型態為 string</para>
            /// </summary>
            COMPLETE_MACRO,
            /// <summary>
            /// Finished Patrol
            /// <para>資料回傳型態為 string</para>
            /// </summary>
            FINISHED_PATROL,
            /// <summary>
            /// Odometer
            /// <para>資料回傳型態為 string</para>
            /// </summary>
            ODOMETER,
            /// <summary>Password Request. For notification, Ignore data!</summary>
            LOGIN_PASSWORD_REQUEST,
            /// <summary>Sign-In successfully. For notification, Ignore data!</summary>
            LOGIN_SUCCESS,
            /// <summary>Command Error. For notification, Ignore data!</summary>
            COMMAND_ERROR,
            /// <summary>Command Error Description. For notification, Ignore data!</summary>
            COMMAND_ERR_DESCRIPT,
            /// <summary>
            /// Unknown Route Name
            /// <para>資料回傳型態為 string</para>
            /// </summary>
            UNKNOWN_ROUTE_NAME,
            /// <summary>
            /// Receive undefined ARCL message
            /// <para>資料回傳型態為 string</para>
            /// </summary>
            UNDEFINED_ARCL_MSG,
            /// <summary>
            /// Emergency Stop Status
            /// <para>資料回傳型態為 bool  (<see langword="true"/>)觸發E-Stop  (<see langword="false"/>)安全無事</para>
            /// </summary>
            E_STOP
        }

        /// <summary>CtArcl 串列埠事件參數</summary>
        public class ArclEventArgs : EventArgs {
            /// <summary>Event</summary>
            public ArclEvents Event { get; set; }
            /// <summary>數值</summary>
            public object Value { get; set; }
            /// <summary>建立一新的事件參數</summary>
			/// <param name="events">事件</param>
			/// <param name="value">數值</param>
            public ArclEventArgs(ArclEvents events, object value) {
                Event = events;
                Value = value;
            }
        }

        /// <summary>CtArcl 串列埠事件</summary>
        public event EventHandler<ArclEventArgs> OnArclEvents;

        /// <summary>觸發 ARCL 事件</summary>
        /// <param name="events">ARCL 事件</param>
        /// <param name="value">事件所帶的參數</param>
        protected virtual void RaiseEvents(ArclEvents events, object value) {
            EventHandler<ArclEventArgs> handler = OnArclEvents;
            if (handler != null) handler(this, new ArclEventArgs(events, value));
        }
        #endregion

        #region Declaration - Fields
        /// <summary>監控 Serial 是否有資料，如有資料則解析</summary>
        private Thread mThread_ScanMRStt;

        /// <summary>CtSerial</summary>
        private CtSerial mSerial;
        /// <summary>CtSyncSocket</summary>
        private CtTcpSocket mSocket;

        /// <summary>接收資訊用佇列</summary>
        private Queue<string> mQueue = new Queue<string>();

        ///// <summary>MobileRobot Command 列舉值</summary>
        //private ArclCommand mArclCmd = ArclCommand.DOCK;
        /// <summary>MobileRobot Command 及其對應命令字串</summary>
        private Dictionary<ArclCommand, string> mArclCmdSet = new Dictionary<ArclCommand, string>();

        /// <summary>儲存 MobileRobot 相關狀態資訊</summary>
        private CtMobileStatus mCurrStt = new CtMobileStatus();
        /// <summary>暫存 MobileRobot 相關狀態資訊</summary>
        private CtMobileStatus mTempStt = new CtMobileStatus();

        ///// <summary>儲存 MobileRobot 相關狀態資訊</summary>
        //private string mLocalizationToDock;
        ///// <summary>暫存 MobileRobot 相關狀態資訊</summary>
        //private int mSerialCmdCount;

        /// <summary>Arcl 之密碼(Password)</summary>
        private string mArclPwd = "";

        #endregion

        #region Declaration - Properties
        /// <summary>取得指令傳送模式為 Serial 或是 Socekt</summary>
        public bool TcpTransferMode { get; private set; }
        /// <summary>充電設定值</summary>
        public int NeedChargeValue { get; set; }
        /// <summary>充電臨界值</summary>
        public int LimitChargeValue { get; set; }
        /// <summary>是否要儲存無法辨識的 ARCL 指令</summary>
        public bool EnableUnknowLog { get; set; }
        /// <summary>儲存無法辨識 ARCL command log檔路徑</summary>
        public string UnknowCmdLogPath { get; set; }
        /// <summary>取得當前是否有連線</summary>
        public bool IsConnected {
            get {
                if (TcpTransferMode) return (mSocket != null) ? mSocket.IsConnected : false;
                else return (mSerial != null) ? mSerial.IsOpen : false;
            }
        }
        /// <summary>取得當前 Mobile Robot 狀態</summary>
        public CtMobileStatus CurrentStatus {
            get { return mCurrStt; }
        }
        #endregion

        #region Declaration - Disposable
        /// <summary>中斷與 SerialPort 或 Socket 之連線，並釋放相關資源</summary>
        public void Dispose() {
            try {
                Dispose(true);
                GC.SuppressFinalize(this);
            } catch (ObjectDisposedException ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
        }

        /// <summary>中斷與 SerialPort 或 Socket 之連線，並釋放相關資源</summary>
        /// <param name="isDisposing">是否為第一次釋放</param>
        protected virtual void Dispose(bool isDisposing) {
            try {
                if (isDisposing) {
                    if (mSerial != null && mSerial.IsOpen) mSerial.Close();
                    if (mSocket != null && mSocket.IsConnected) mSocket.ClientDisconnect();
                }
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
        }
        #endregion

        #region Function - Constructors
        /// <summary>建立全空的 CtARCL，並請利用 Connect 方法進行連線</summary>
        public CtArcl() { CreateCmdSet(); TcpTransferMode = true; }

        /// <summary>
        /// 建立 CtARCL
        /// <para>如是 TCP/IP 模式則直接以預設 IP、Port 開始執行</para>
        /// <para>如是 SerialPort 模式則跳出視窗由使用者選擇連接埠</para>
        /// </summary>
        /// <param name="tcpMode">是否為 TCP/IP 模式? (<see langword="true"/>)使用TCP/IP，以CtSyncSocket進行連線    (<see langword="false"/>)使用串列埠，以CtSerial連線</param>
        public CtArcl(bool tcpMode) {
            CreateCmdSet();

            TcpTransferMode = tcpMode;

            if (tcpMode) {
                mSocket = new CtTcpSocket(TransDataFormats.String, true);
                mSocket.OnSocketEvents += mSocket_OnSocketEvents;
                mSocket.ClientConnect(DEFAULT_SOCKET_IP, DEFAULT_SOCKET_PORT);
            } else {
                mSerial = new CtSerial();
                mSerial.OnSerialEvents += mSerial_OnSerialEvents;
                mSerial.EndOfLineSymbol = EndChar.CrLf;
                mSerial.Open();
            }

            CtThread.CreateThread(ref mThread_ScanMRStt, THREAD_SCAN_NAME, tsk_ScanMRStt);

        }

        /// <summary>建立使用 TCP/IP (Socket) 之 CtARCL</summary>
        /// <param name="socket">CtSyncSocket</param>
        /// <remarks>如果要帶入此方法，請盡量帶入尚未加入 OnSocketEvents 之物件(或者移除之)，不確定雙重事件會如何運作</remarks>
        public CtArcl(CtTcpSocket socket) {
            TcpTransferMode = true;

            mSocket = socket;
            if (mSocket == null) mSocket = new CtTcpSocket(TransDataFormats.String, true);
            else mSocket.ClientDisconnect();
            mSocket.OnSocketEvents += mSocket_OnSocketEvents;
            mSocket.ClientConnect(DEFAULT_SOCKET_IP, DEFAULT_SOCKET_PORT);

            CtThread.CreateThread(ref mThread_ScanMRStt, THREAD_SCAN_NAME, tsk_ScanMRStt);

            CreateCmdSet();
        }

        /// <summary>建立使用 SerialPort 之 CtARCL</summary>
        /// <param name="serial">CtSerial</param>
        /// <remarks>如果要帶入此方法，請盡量帶入尚未加入 OnSerialEvents 之物件(或者移除之)，不確定雙重事件會如何運作</remarks>
        public CtArcl(CtSerial serial) {
            TcpTransferMode = false;

            mSerial = serial;
            if (mSerial == null) mSerial = new CtSerial();
            else if (!mSerial.IsOpen) mSerial.Open();
            mSerial.EndOfLineSymbol = EndChar.CrLf;
            mSerial.OnSerialEvents += mSerial_OnSerialEvents;

            CtThread.CreateThread(ref mThread_ScanMRStt, THREAD_SCAN_NAME, tsk_ScanMRStt);

            CreateCmdSet();
        }

        /// <summary>建立使用 TCP/IP (Socket) 之 CtARCL</summary>
        /// <param name="srvIp">欲連接至 Server 之網際網路位置</param>
        /// <param name="srvPort">欲連接至 Server 之連接埠</param>
        public CtArcl(string srvIp, int srvPort) {
            TcpTransferMode = true;
            CreateCmdSet();

            mSocket = new CtTcpSocket(TransDataFormats.String, true);
            mSocket.OnSocketEvents += mSocket_OnSocketEvents;
            mSocket.ClientConnect(srvIp, srvPort);

            CtThread.CreateThread(ref mThread_ScanMRStt, THREAD_SCAN_NAME, tsk_ScanMRStt);
        }

        /// <summary>建立使用 SerialPort 之 CtARCL</summary>
        /// <param name="portName">串列埠名稱, e.g."COM1"</param>
        /// <param name="baudRate">鮑率(BaudRate)，即每秒傳輸速度(bits/s), e.g. 9600</param>
        /// <param name="dataBits">資料位元數(Data Bits), 一般為 8 位元</param>
        /// <param name="stopBit">停止位元(StopBits)</param>
        /// <param name="parity">同位檢查位元(Parity)</param>
        /// <param name="handshake">交握控制(Handshake)</param>
        /// <param name="timeout">讀取/寫入之逾時時間(Timeout)</param>
        public CtArcl(string portName,
                        int baudRate = 9600,
                        int dataBits = 8,
                        CtSerial.StopBits stopBit = CtSerial.StopBits.One,
                        CtSerial.Parity parity = CtSerial.Parity.None,
                        CtSerial.Handshake handshake = CtSerial.Handshake.None,
                        int timeout = 1000) {

            TcpTransferMode = false;

            mSerial = new CtSerial();
            mSerial.OnSerialEvents += mSerial_OnSerialEvents;
            mSerial.EndOfLineSymbol = EndChar.CrLf;
            mSerial.Open(portName, baudRate, dataBits, stopBit, parity, handshake, timeout);

            CtThread.CreateThread(ref mThread_ScanMRStt, THREAD_SCAN_NAME, tsk_ScanMRStt);

            CreateCmdSet();
        }
        #endregion

        #region Function - Events
        void mSocket_OnSocketEvents(object sender, SocketEventArgs e) {
            if (e.Event == SocketEvents.DataReceived) {
                mQueue.Enqueue((e.Value as SocketRxData).Data.ToString());
            } else if (e.Event == SocketEvents.ConnectionWithServer) {
                RaiseEvents(ArclEvents.SOCKET_CONNECTION, (e.Value as SocketConnection).Status);
            }
        }

        void mSerial_OnSerialEvents(object sender, SerialEventArgs e) {
            if (e.Event == SerialPortEvents.DataReceived) {
                string strTemp = e.Value.ToString();
                string[] strSplit = strTemp.Trim().Split(CtConst.CHR_SPACE, StringSplitOptions.RemoveEmptyEntries);
                strTemp = strSplit[0].ToLower();
                if (strTemp == "playing" || strTemp == "routes" || strTemp == "saying") {
                    ;
                } else if (strTemp == "goto" || strTemp == "getgoals" || strTemp == "getroute" ||
                           strTemp == "play" || strTemp == "say" || strTemp == "patrolonce" ||
                           strTemp == "odometer" || strTemp == "onelines") {
                    ;
                } else mQueue.Enqueue(e.Value.ToString());
            }
        }
        #endregion

        #region Function - Threads

        /// <summary>[Thread] 持續掃描 MobileRobot 狀態</summary>
        private void tsk_ScanMRStt() {
            try {
                string strQueue = "";
                List<string> strGoals = new List<string>();     //用於儲存Goal
                List<string> strRoutes = new List<string>();    //用於儲存Route
                List<string> strMacros = new List<string>();    //用於儲存Macro

                do {
                    try {
                        /*-- 檢查 Queue 裡面是否還有東西 --*/
                        if (mQueue.Count > 0) strQueue = mQueue.Dequeue();
                        else strQueue = "";

                        /*-- 如果裡面確實有命令存在，分析之 --*/
                        if (strQueue != "") {
                            /* 向外發布 */
                            RaiseEvents(ArclEvents.RECEIVE_CMD, strQueue);

                            /* 避免一次收到多筆命令，使用 \r\n 去分割，然後用迴圈跑完 */
                            string[] strSplitCrLf = strQueue.Split(CtConst.CHR_CRLF, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string strCrLfData in strSplitCrLf) {
                                /* 使用 ":" 進行命令拆解 */
                                string[] strSplitCmd = strCrLfData.Split(CtConst.CHR_COLON, StringSplitOptions.RemoveEmptyEntries);

                                /* 如果確定有一串東西，但是 ":" 拆不掉，換成用 " " 拆拆看 */
                                if (strSplitCmd.Length == 1) strSplitCmd = strCrLfData.Split(CtConst.CHR_SPACE, StringSplitOptions.RemoveEmptyEntries);

                                /* 全部的東西做 Trim() 去除一些奇奇怪怪的字元 */
                                for (int idx = 0; idx < strSplitCmd.Length; idx++) strSplitCmd[idx] = strSplitCmd[idx].Trim();

                                /* 分析命令 */
                                string strCmd = strSplitCmd[0].ToLower();
                                #region Command Judge
                                if (strCmd == "status") {
                                    /* 取得目前 MobileRobot 狀態 */
                                    string[] strStt = strCrLfData.Replace(":", "").Split(CtConst.CHR_SPACE, StringSplitOptions.RemoveEmptyEntries);
                                    if (strStt.Length >= 10) CmdInfo_Status(strStt);
                                    else {
                                        string[] strRemoveHead = new string[strStt.Length - 1];
                                        Array.ConstrainedCopy(strStt, 1, strRemoveHead, 0, strStt.Length - 1);
                                        CmdInfo_Status(strRemoveHead);
                                    }

                                } else if (strCmd == "arrived" || strCmd == "going" || strCmd == "stopping" || strCmd == "stopped" ||
                                            strCmd == "following" || strCmd == "teleop" || strCmd == "dockingstate" || strCmd == "motors") {
                                    /* 狀態 */
                                    string[] strStt = strCrLfData.Replace(":", "").Split(CtConst.CHR_SPACE, StringSplitOptions.RemoveEmptyEntries);
                                    CmdInfo_Status(strStt);

                                } else if (strCmd == "completed patrol, patroling again") {
                                    string[] strStt = strSplitCmd[0].Trim().Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries);
                                    mCurrStt.MRStt = CtMobileStatus.MRStat.COMPLETED_PATROL;
                                    RaiseEvents(ArclEvents.MOTION_STATUS, strStt[1]);

                                } else if (strCmd == "distancebetween" && strSplitCmd.Length == 2) {
                                    /* 兩點之間距離 格式: 0 "Goal_1" "Goal_1" */
                                    string[] strDis = strSplitCmd[1].Split(CtConst.CHR_SPACE, StringSplitOptions.RemoveEmptyEntries);
                                    if (strDis.Length == 3) mCurrStt.BTdistance = CtConvert.CInt(strDis[0]);

                                } else if (strCmd == "patrolling") {
                                    /*  */
                                    mCurrStt.FinishRouteName = "";
                                    mCurrStt.FinishMacroName = "";
                                    mCurrStt.ArrivedGoalRecord(CtMobileStatus.NO_ARRIVED);

                                } else if (strCmd == "finished") {
                                    /*  */
                                    if (strSplitCmd[1].ToLower() == "patrolling") {
                                        mCurrStt.MRStt = CtMobileStatus.MRStat.FINISHED_PATROLLING;
                                        mCurrStt.FinishRouteName = strSplitCmd[3];
                                        RaiseEvents(ArclEvents.FINISHED_PATROL, strSplitCmd[3]);
                                    }

                                } else if (strCmd == "completed") {
                                    /*  */
                                    if (strSplitCmd[1].ToLower() == "macro") {
                                        mCurrStt.MRStt = CtMobileStatus.MRStat.COMPLETED_MACRO;
                                        mCurrStt.FinishMacroName = strSplitCmd[2];
                                        RaiseEvents(ArclEvents.COMPLETE_MACRO, strSplitCmd[2]);
                                    }

                                } else if (strCmd == "enter password") {
                                    /* 要求輸入密碼，由使用者自行傳送 ArclCommand.CUSTOMLOGIN 之命令 */
                                    RaiseEvents(ArclEvents.LOGIN_PASSWORD_REQUEST, false);

                                } else if (strCmd == "welcome") {
                                    /* 登入成功 */
                                    RaiseEvents(ArclEvents.LOGIN_SUCCESS, true);
                                }
                                /* 取得目前座標 */
                                else if (strCmd == "location") CmdInfo_Location(strSplitCmd);

                                /* 取得目前電量 */
                                else if (strCmd == "stateofcharge") CmdInfo_StateOfCharge(strSplitCmd);

                                /* 取得總行走距離 (mm) */
                                else if (strCmd == "odometer") CmdInfo_Odometer(strSplitCmd);

                                /* 取得現在與地圖比對相似度 */
                                else if (strCmd == "localizationscore") CmdInfo_LocalizationScore(strSplitCmd);

                                /* 取得溫度資料 */
                                else if (strCmd == "temperature") CmdInfo_Temperature(strSplitCmd);

                                /*  */
                                else if (strCmd == "extendedstatusforhumans") CmdInfo_ExtendedStatus(strSplitCmd);

                                /* 取得目前 map 中所有站點 */
                                else if (strCmd == "goal") CmdInfo_Goals(strSplitCmd, ref strGoals);

                                /* 取得目前 map 中所有 route */
                                else if (strCmd == "route") CmdInfo_Routes(strSplitCmd, ref strRoutes);

                                /* 取得目前 map 中所有 macro */
                                else if (strCmd == "macro") CmdInfo_Macros(strSplitCmd, ref strMacros);

                                /* Emergency Stop */
                                else if (strCmd == "estop") CmdInfo_EStop(strSplitCmd);

                                /*  */
                                else if (strCmd == "teleop") CmdInfo_Teleop(strSplitCmd);

                                /* 動作遭中斷 */
                                else if (strCmd == "interrupted") CtStatus.Report(Stat.ER_SYSTEM, "MRState", "Interrupted Command = " + strQueue);

                                /*  */
                                else if (strCmd == "error") CmdInfo_Error(strSplitCmd);

                                /* 地圖切換 */
                                else if (strCmd == "map") CmdInfo_MapChange(strSplitCmd);

                                /* End of goals */
                                else if (strCmd == "end") CmdInfo_End(strSplitCmd);

                                /*  */
                                else if (strCmd == "setuperror") CmdInfo_SetupError(strSplitCmd);

                                /* 充電，如果是 MR 要求充電則撥放音效 */
                                else if (strCmd == "charge" && strSplitCmd[1].Trim().ToLower() == "requested") SendCommand(ArclCommand.PLAY, "ring1.wav");

                                /*  */
                                else if (strCmd == "commanderrordescription") CmdInfo_CommandErrDescription(strSplitCmd);

                                /*  */
                                else if (strCmd == "commanderror") CmdInfo_CommandError(strSplitCmd);

                                /* 以下為排除名單，若非 回應、動作類 則將命令儲存起來 */
                                else if (strCmd != "say" && strCmd != "saying" && strCmd != "executing" && strCmd != "playing" &&
                                           strCmd != "routes" && strCmd != "will" && strCmd != "getgoals" && strCmd != "getroute" &&
                                           strCmd != "play" && strCmd != "patrolonce")
                                    UnknowCommandLog("tsk_ScanMRStt", new string[] { strQueue });
                                #endregion
                            }
                        }

                        CtTimer.Delay(THREAD_SCAN_DELAY);
                        System.Windows.Forms.Application.DoEvents();

                    } catch (Exception ex) {
                        CtStatus.Report(Stat.ER_SYSTEM, ex);
                    }
                } while (mThread_ScanMRStt.IsAlive);
            } catch (ThreadAbortException) {
            } catch (ThreadInterruptedException) {
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
        }
        #endregion

        #region Function - Mehtods
        /// <summary>將 ArclCommand 依序轉成 Command String 至 mArclCmdSet 裡</summary>
        private void CreateCmdSet() {
            foreach (ArclCommand item in Enum.GetValues(typeof(ArclCommand))) {
                mArclCmdSet.Add(item, Enum.GetName(typeof(ArclCommand), item).ToLower().Replace("_", ""));
            }
        }

        /// <summary>紀錄未知命令</summary>
        /// <param name="title">標題</param>
        /// <param name="data">資料</param>
        private void UnknowCommandLog(string title, string[] data) {
            string strTemp = DateTime.Now.ToString("[HH:mm:ss.ff] [") + title + "] " + string.Join(" ", data);
            CtLog.ReportLog(strTemp, FILENAME_UNKNOWN_COMMANDS);
        }

        /// <summary>設定 ARCL 密碼</summary>
        /// <param name="pwd">密碼</param>
        public void SetPassword(string pwd) {
            mArclPwd = pwd;
        }
        #endregion

        #region Function - Core
        /// <summary>
        /// 進行連線
        /// <para>[TCP/IP] 以預設 Port 開始進行監聽</para>
        /// <para>[Serial] 跳出視窗進行設定</para>
        /// </summary>
        /// <param name="tcpMode">是否為 TCP/IP 模式? (<see langword="true"/>)使用TCP/IP，以CtSyncSocket進行連線    (<see langword="false"/>)使用串列埠，以CtSerial連線</param>
        public void Connect(bool tcpMode) {
            TcpTransferMode = tcpMode;

            if (tcpMode && mSocket == null) {
                mSocket = new CtTcpSocket(TransDataFormats.String, true);
                mSocket.OnSocketEvents += mSocket_OnSocketEvents;
                mSocket.ClientConnect(DEFAULT_SOCKET_IP, DEFAULT_SOCKET_PORT);
            } else if (!tcpMode && mSerial == null) {
				mSerial = new CtSerial() {
					EndOfLineSymbol = EndChar.CrLf
				};
				mSerial.OnSerialEvents += mSerial_OnSerialEvents;
                mSerial.Open();
            }

            if (mThread_ScanMRStt != null && mThread_ScanMRStt.IsAlive) CtThread.KillThread(ref mThread_ScanMRStt);
            CtThread.CreateThread(ref mThread_ScanMRStt, THREAD_SCAN_NAME, tsk_ScanMRStt);
        }

        /// <summary>進行連線，以當前 <see cref="TcpTransferMode"/> 選項進行連線</summary>
        public void Connect() {
            Connect(TcpTransferMode);
        }

        /// <summary>透過 TCP/IP (Socket) 進行連線</summary>
        /// <param name="srvIP">欲連接至 Server 之網際網路位置</param>
        /// <param name="srvPort">欲連接至 Server 之連接埠</param>
        public void Connect(string srvIP, int srvPort) {
            if (mSocket == null) {
                TcpTransferMode = true;

                mSocket = new CtTcpSocket(TransDataFormats.String, true);
                mSocket.OnSocketEvents += mSocket_OnSocketEvents;
                mSocket.ClientConnect(srvIP, srvPort);
            }

            if (mThread_ScanMRStt != null && mThread_ScanMRStt.IsAlive) CtThread.KillThread(ref mThread_ScanMRStt);
            CtThread.CreateThread(ref mThread_ScanMRStt, THREAD_SCAN_NAME, tsk_ScanMRStt);
        }

        /// <summary>透過 SerialPort 進行連線</summary>
        /// <param name="portName">串列埠名稱, e.g."COM1"</param>
        /// <param name="baudRate">鮑率(BaudRate)，即每秒傳輸速度(bits/s), e.g. 9600</param>
        /// <param name="dataBits">資料位元數(Data Bits), 一般為 8 位元</param>
        /// <param name="stopBit">停止位元(StopBits)</param>
        /// <param name="parity">同位檢查位元(Parity)</param>
        /// <param name="handshake">交握控制(Handshake)</param>
        /// <param name="timeout">讀取/寫入之逾時時間(Timeout)</param>
        public void Connect(string portName,
                            int baudRate = 9600,
                            int dataBits = 8,
                            CtSerial.StopBits stopBit = CtSerial.StopBits.One,
                            CtSerial.Parity parity = CtSerial.Parity.None,
                            CtSerial.Handshake handshake = CtSerial.Handshake.None,
                            int timeout = 1000) {

            if (mSerial == null) {
                TcpTransferMode = false;

                mSerial = new CtSerial();
                mSerial.OnSerialEvents += mSerial_OnSerialEvents;
                mSerial.EndOfLineSymbol = EndChar.CrLf;
                mSerial.Open(portName, baudRate, dataBits, stopBit, parity, handshake, timeout);
            }

            if (mThread_ScanMRStt != null && mThread_ScanMRStt.IsAlive) CtThread.KillThread(ref mThread_ScanMRStt);
            CtThread.CreateThread(ref mThread_ScanMRStt, THREAD_SCAN_NAME, tsk_ScanMRStt);
        }

        /// <summary>中斷與 ARCL 之連線</summary>
        public void Disconnect() {
            if (TcpTransferMode) mSocket.ClientDisconnect();
            else mSerial.Close();

            CtThread.KillThread(ref mThread_ScanMRStt);
        }

        /// <summary>傳送命令至 MobileRobot，藉由 <see cref="TcpTransferMode"/> 決定由 Socket 或 Serial 決定如何傳送</summary>
        /// <param name="arclCmd">ARCL 命令</param>
        /// <param name="parm">附帶參數</param>
        public void SendCommand(ArclCommand arclCmd, string parm = "") {
            if (arclCmd == ArclCommand.CUSTOMLOGIN) {
                mSocket.Send(parm);
            } else {
                if (parm.Length > 0) parm = " " + parm;
                if (TcpTransferMode && mSocket != null) {
                    mSocket.Send(mArclCmdSet[arclCmd] + parm);
                } else if (!TcpTransferMode && mSerial != null && mSerial.IsOpen) {
                    mSerial.Send(mArclCmdSet[arclCmd] + parm, EndChar.CrLf);
                }
            }
        }
        #endregion

        #region Function - Command Information
        private void CmdInfo_SetupError(string[] data) {
            if (data[1].ToLower() != "the file given did not exist") UnknowCommandLog("CmdInfo_SetupError", data);
        }

        private void CmdInfo_End(string[] data) {
            if (data.Length > 2) {
                if (data[2].ToLower() != "goals" && data[2].ToLower() != "routes") UnknowCommandLog("CmdInfo_End", data);
            } else UnknowCommandLog("CmdInfo_End", data);
        }

        private void CmdInfo_MapChange(string[] data) {
            if (data[1] != "changed") UnknowCommandLog("CmdInfo_MapChange", data);
        }

        private void CmdInfo_Error(string[] data) {
            string[] strTemp = data[1].Split(CtConst.CHR_SPACE, StringSplitOptions.RemoveEmptyEntries);
            for (int idx = 0; idx < strTemp.Length; idx++) strTemp[idx] = strTemp[idx].Trim();
            if (strTemp[0].ToLower() == "failed going to goal") mCurrStt.MRStt = CtMobileStatus.MRStat.ERROR;
            else UnknowCommandLog("CmdInfo_Error", data);
        }

        private void CmdInfo_Motors(string[] data) {
            if (data.Length == 2) {
                if (data[1] == "enabled") mCurrStt.MRStt = CtMobileStatus.MRStat.STOPPED;
                else if (data[1] == "disabled") mCurrStt.MRStt = CtMobileStatus.MRStat.MOTOR_DISABLE;
                else UnknowCommandLog("CmdInfo_Motors", data);
            } else UnknowCommandLog("CmdInfo_Motors", data);
        }

        private void CmdInfo_Teleop(string[] data) {
            mCurrStt.MRStt = CtMobileStatus.MRStat.TELEOP_DRIVING;
        }

        private void CmdInfo_EStop(string[] data) {
            if (data.Length > 1) {
                if (data[1].ToLower() == "pressed") mCurrStt.MRStt = CtMobileStatus.MRStat.ESTOP;
                else if (data[1].ToLower() == "relieved") mCurrStt.MRStt = CtMobileStatus.MRStat.MOTOR_DISABLE;
                else UnknowCommandLog("CmdInfo_EStop", data);
            } else UnknowCommandLog("CmdInfo_EStop", data);
        }

        private void CmdInfo_Routes(string[] data, ref List<string> routes) {
            if (routes == null) routes = new List<string>();
            if (!routes.Contains(data[1].Trim())) {
                routes.Add(data[1].Trim());
                RaiseEvents(ArclEvents.UPDATE_ROUTES, routes.ToList());
            }
        }

        private void CmdInfo_Goals(string[] data, ref List<string> goals) {
            if (goals == null) goals = new List<string>();
            if (!goals.Contains(data[1])) {
                goals.Add(data[1]);
                RaiseEvents(ArclEvents.UPDATE_GOALS, goals.ToList());
                mCurrStt.CleanGoals();
                mCurrStt.AddGoals(goals.ToArray());
            }
        }

        private void CmdInfo_Macros(string[] data, ref List<string> macro) {
            if (macro == null) macro = new List<string>();
            if (!macro.Contains(data[1])) {
                macro.Add(data[1]);
                RaiseEvents(ArclEvents.UPDATE_MACROS, macro.ToList());
                mCurrStt.CleanMacros();
                mCurrStt.AddMacros(macro.ToArray());
            }
        }

        private void CmdInfo_Temperature(string[] data) {
            mCurrStt.Temperature = CtConvert.CInt(data[1].Trim());
            RaiseEvents(ArclEvents.TEMPERATURE, mCurrStt.Temperature);
        }

        private void CmdInfo_ExtendedStatus(string[] data) {
            mCurrStt.ExtendStt = data[1].Trim();
            RaiseEvents(ArclEvents.EXTENDED_STATUS, data[1].Trim());
        }

        private void CmdInfo_LocalizationScore(string[] data) {
            mCurrStt.LocalizationScore = CtConvert.CDbl(data[1].Trim());
            RaiseEvents(ArclEvents.LOCALIZATION_SCORE, mCurrStt.LocalizationScore);
        }

        private void CmdInfo_Location(string[] data) {
            string[] strTemp = data[1].Split(CtConst.CHR_SPACE, StringSplitOptions.RemoveEmptyEntries);
            CtMobileStatus.LocationXYT loc = new CtMobileStatus.LocationXYT(CtConvert.CInt(strTemp[0]), CtConvert.CInt(strTemp[1]), CtConvert.CInt(strTemp[2]));
            mCurrStt.Locations = loc;
            RaiseEvents(ArclEvents.LOCATION, loc);
        }

        private void CmdInfo_Odometer(string[] data) {
            if (data.Length == 2) {
                string[] strTemp = data[1].Split(CtConst.CHR_SPACE, StringSplitOptions.RemoveEmptyEntries);
                mCurrStt.Odometer = CtConvert.CInt(strTemp[0].Trim());
                RaiseEvents(ArclEvents.ODOMETER, mCurrStt.Odometer);
            } else if (data.Length != 1) UnknowCommandLog("CmdInfo_Odometer", data);
        }

        private void CmdInfo_StateOfCharge(string[] data) {
            mCurrStt.ChargeStt = CtConvert.CInt(data[1].Trim());
            RaiseEvents(ArclEvents.STATE_OF_CHARGE, mCurrStt.ChargeStt);
        }

        private void CmdInfo_OneLineStatus(string[] data) {
            string[] strTemperature = new string[2];
            strTemperature[0] = data[data.Length - 2];
            strTemperature[1] = data[data.Length - 1];
            CmdInfo_Temperature(strTemperature);

            string[] strLoc = new string[2];
            strLoc[0] = data[data.Length - 6];
            strLoc[1] = data[data.Length - 5] + " " + data[data.Length - 4] + " " + data[data.Length - 3];
            CmdInfo_Location(strLoc);

            string[] strCharge = new string[2];
            strCharge[0] = data[data.Length - 8];
            strCharge[1] = data[data.Length - 7];
            CmdInfo_StateOfCharge(strCharge);

            string[] strStatus = new string[data.Length - 10];
            Array.ConstrainedCopy(data, 1, strStatus, 0, data.Length - 9);  /* ConstrainedCopy 與 Copy 不同的是，ConstrainedCopy 會先檢查相容性才執行，若不相容則 destinationArray 不會更改 */
            if (strStatus != null) CmdInfo_Status(strStatus);
        }

        private void CmdInfo_Status(string[] data) {
            string arriveGoal = "";

            switch (data[0].ToLower().Trim()) {
                case "arrived":
                    if (data.Length == 3) arriveGoal = data[2].Trim();
                    else if (data.Length == 4) arriveGoal = data[2].Trim() + " " + data[3].Trim();
                    else if (data.Length == 6) arriveGoal = data[3].Trim() + " " + data[4].Trim() + " " + data[5].Trim();
                    mCurrStt.MRStt = CtMobileStatus.MRStat.ARRIVED;
                    mCurrStt.ArrivedGoalRecord(arriveGoal);
                    RaiseEvents(ArclEvents.ARRIVED_GOAL, arriveGoal);
                    break;

                case "going":
                    if (data.Length == 3) {
                        mCurrStt.ArrivedGoalRecord(CtMobileStatus.NO_ARRIVED);
                        mCurrStt.TargetGoal = data[2].Trim();
                    } else if (data.Length == 6) mCurrStt.TargetGoal = data[3].Trim() + " " + data[4].Trim() + " " + data[5].Trim();
                    else UnknowCommandLog("CmdInfo_Status", data);
                    mCurrStt.MRStt = CtMobileStatus.MRStat.GOING;
                    RaiseEvents(ArclEvents.MOTION_STATUS, data[0].Trim());
                    break;

                case "stopping":
                    mCurrStt.MRStt = CtMobileStatus.MRStat.STOPPING;
                    RaiseEvents(ArclEvents.MOTION_STATUS, data[0].Trim());
                    break;

                case "stopped":
                    mCurrStt.MRStt = CtMobileStatus.MRStat.STOPPED;
                    RaiseEvents(ArclEvents.MOTION_STATUS, data[0].Trim());
                    break;

                case "following":
                    mCurrStt.MRStt = CtMobileStatus.MRStat.FOLLOWING;
                    RaiseEvents(ArclEvents.MOTION_STATUS, data[0].Trim());
                    break;

                case "teleop":
                    mCurrStt.MRStt = CtMobileStatus.MRStat.TELEOP_DRIVING;
                    RaiseEvents(ArclEvents.MOTION_STATUS, data[0].Trim());
                    break;

                case "dockingstate":
                    CmdInfo_DockingState(data);
                    mCurrStt.MRStt = CtMobileStatus.MRStat.DOCK;
                    break;

                case "finished":
                    if (data[1] == "patrolling") {
                        mCurrStt.MRStt = CtMobileStatus.MRStat.FINISHED_PATROLLING;
                        mCurrStt.FinishRouteName = data[3].Trim();
                        RaiseEvents(ArclEvents.FINISHED_PATROL, mCurrStt.FinishRouteName);
                    }
                    break;

                case "motors":
                    CmdInfo_Motors(data);
                    break;
                default:
                    UnknowCommandLog("CmdInfo_Status", data);
                    break;
            }
        }

        private void CmdInfo_DockingState(string[] data) {
            if (data.Length != 6) {
                UnknowCommandLog("CmdInfo_DockingState", data);
            } else {
                string tempDock = data[1];
                string tempForce = data[3];
                string tempCharge = data[5];

                CtMobileStatus.DockingState dockStt = (CtMobileStatus.DockingState)Enum.Parse(typeof(CtMobileStatus.DockingState), tempDock, true);
                if (Enum.IsDefined(typeof(CtMobileStatus.DockingState), dockStt) || dockStt.ToString().Contains(".")) {

                    mCurrStt.DockStts.DockStt = dockStt;

                    switch (dockStt) {
                        case CtMobileStatus.DockingState.UNDOCKING:
                        case CtMobileStatus.DockingState.UNDOCKED:
                            mCurrStt.ArrivedGoalRecord(CtMobileStatus.NO_ARRIVED);
                            break;
                        case CtMobileStatus.DockingState.DOCKING:
                            mCurrStt.TargetGoal = "dock";
                            break;
                        case CtMobileStatus.DockingState.DOCKED:
                            mCurrStt.ArrivedGoalRecord("dock");
                            break;
                        default:
                            break;
                    }
                    RaiseEvents(ArclEvents.DOCKING_STATE, dockStt);
                }

                CtMobileStatus.ForcedState forceStt = (CtMobileStatus.ForcedState)Enum.Parse(typeof(CtMobileStatus.ForcedState), tempForce, true);
                if (Enum.IsDefined(typeof(CtMobileStatus.ForcedState), forceStt) || forceStt.ToString().Contains(".")) {
                    mCurrStt.DockStts.ForceStt = forceStt;
                    RaiseEvents(ArclEvents.FORCED_STATE, forceStt);
                }

                CtMobileStatus.ChargeState chargeStt = (CtMobileStatus.ChargeState)Enum.Parse(typeof(CtMobileStatus.ChargeState), tempCharge, true);
                if (Enum.IsDefined(typeof(CtMobileStatus.ChargeState), chargeStt) || chargeStt.ToString().Contains(".")) {
                    mCurrStt.DockStts.ChargeStt = chargeStt;
                    RaiseEvents(ArclEvents.CHARGE_STATE, chargeStt);
                }
            }
        }

        private void CmdInfo_CommandErrDescription(string[] data) {
            string[] temp = data[1].Split(CtConst.CHR_SPACE, StringSplitOptions.RemoveEmptyEntries);
            if (temp[0].ToLower() == "bad") RaiseEvents(ArclEvents.UNKNOWN_ROUTE_NAME, temp[4]);
            RaiseEvents(ArclEvents.COMMAND_ERR_DESCRIPT, false);
        }

        private void CmdInfo_CommandError(string[] data) {
            RaiseEvents(ArclEvents.COMMAND_ERROR, false);
        }
        #endregion
    }
}
