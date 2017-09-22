using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using CtLib.Library;
using CtLib.Module.Ultity;

namespace CtLib.Module.TCPIP {
    /// <summary>
    /// 實作 <see cref="Socket"/> (TCP/IP) 之非同步控制。分為 用戶端(<see cref="TcpClient"/>) 與 伺服端(<see cref="TcpListener"/>)，並藉此互相傳輸資料。
    /// <para>用戶端 - 給予埠號(Port)並以本機作為伺服器，啟動後等待 Client 連接。可接受多個 Client，傳送資料時可選擇廣播或單一對象傳送；接收資料則會帶有 Client 資訊</para>
    /// <para>伺服端 - 給予欲連線 Server 端之 網際網路位置(IP) 與 埠號(Port) 並藉此嘗試連線至 Server</para>
    /// <para>均採用非同步方式實作。</para>
    /// </summary>
    /// <example>
    /// Client 連線方式
    /// <code>
    /// CtSyncSocket mSocket = new CtSyncSocket(TransmissionDataFormats.STRING);
    /// mSocket.OnSocketEvents += mSocket_OnSocketEvents;   //Add event, it contains connect status, received data and so on
    /// 
    /// mSocket.ClientConnect("192.168.254.254", 7788);     //Connect to "192.168.254.254:7788"
    /// 
    /// /* Do something here */
    /// 
    /// mSocket.ClientDisconnect();                         //Close the connection with server
    /// 
    /// </code>
    /// Server 連線方式
    /// <code>
    /// CtSyncSocket mSocket = new CtSyncSocket(TransmissionDataFormats.STRING);
    /// mSocket.OnSocketEvents += mSocket_OnSocketEvents;   //Add event, it contains connect status, received data and so on
    /// 
    /// mSocket.ServerListen(2909);                         //Start listening stream at "localhost:2909"
    /// 
    /// /* Do something here */
    /// 
    /// mSocket.ServerStopListen();                         //Close all client connect and server stream
    /// </code>
    /// 如果有需要使用自訂義結尾符號，請看以下示範
    /// <code>
    /// mSocket.EndOfLineSymbol = EndChar.CUSTOM;   //設定使用自定義結尾
    /// mSocket.CustomEndOfLine = "*";              //自訂義的結尾符號為 "*"
    /// 
    /// //如果目標傳送 "Hello*", 你會完整收到 "Hello*" (與 CtSerial 不同)!!
    /// </code></example>
    /// <remarks>目前傳送資料部分均不含結尾符號引數，因為大部分走 Socket 比較少含有結尾符號，因為速度較快不像 RS-232 易有斷開情況。如有特殊需求(如 CommunicationProtocol)請自行加於 data 裡</remarks>
    public class CtAsyncSocket : IDisposable {

        #region Version

        /// <summary>CtAsyncSocket 版本相關訊息</summary>
        /// <remarks><code>
        /// 0.0.0  William [2015/04/22]
        ///     + CtAsyncSockets
        ///     
        /// 1.0.0  Ahern [2015/05/01]
        ///     + Translate from VB
        ///     \ 基礎測試完成
        ///     
        /// 1.0.1  Ahern [2015/05/04]
        ///     + ServerStopListen with specified ip and port
        ///     
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 0, 0, "2015/05/01", "Ahern Kuo");

        #endregion

        #region Declaration - Support Class
        /// <summary>
        /// 連線資訊。包含 連接/斷線、時間、對方 IP 與 Port
        /// <para>[Client] 連結至 Server 之訊息。IP、Port 即為當前連線狀態變化之 Server</para>
        /// <para>[Server] 此 Client 連線訊息。IP、Port 為此 Client 之相關訊息</para>
        /// </summary>
        public class ConnectInfo {
            /// <summary>此事件觸發時間</summary>
            public DateTime Time { get; internal set; }
            /// <summary>連線狀態 (True)已連接  (False)斷線</summary>
            public bool Status { get; internal set; }
            /// <summary>IP 字串，如 "192.168.0.1"</summary>
            public string IP { get; internal set; }
            /// <summary>埠號(Port)</summary>
            public int Port { get; internal set; }
            /// <summary>建立連線資訊</summary>
            /// <param name="stt">連線狀態 (True)已連接  (False)斷線</param>
            /// <param name="ip">IP 字串，如 "192.168.0.1"</param>
            /// <param name="port">埠號(Port)</param>
            public ConnectInfo(bool stt, string ip, int port) {
                Status = stt;
                IP = ip;
                Port = port;
                Time = DateTime.Now;
            }
        }

        /// <summary>
        /// 傳輸資料
        /// <para>用於發報目前接收到的資料</para>
        /// </summary>
        public class DataInfo {
            /// <summary>接收到此資料的時間</summary>
            public DateTime Time { get; internal set; }
            /// <summary>接收到的資料</summary>
            public object Data { get; internal set; }
            /// <summary>此筆資料來源裝置之網際網路位置(IP)</summary>
            public string IP { get; internal set; }
            /// <summary>此筆資料來源裝置之埠號(Port)</summary>
            public int Port { get; internal set; }
            /// <summary>建立傳輸資料物件</summary>
            /// <param name="data">接收到的資料</param>
            /// <param name="ip">此筆資料來源裝置之網際網路位置(IP)</param>
            /// <param name="port">此筆資料來源裝置之埠號(Port)</param>
            public DataInfo(object data, string ip, int port) {
                Data = data;
                IP = ip;
                Port = port;
                Time = DateTime.Now;
            }
        }

        /// <summary>用於非同步資料傳遞</summary>
        private class StateObject {
            /// <summary>Socket 物件</summary>
            public Socket WorkSocket = null;
            /// <summary>資料陣列長度</summary>
            public static readonly int BufferSize = 1024;
            /// <summary>從 NetworkStream 讀取到資料</summary>
            public byte[] Buffer = new byte[BufferSize];
            /// <summary>Socket 通訊時的 Error 訊息</summary>
            public SocketError Error = SocketError.Success;
            /// <summary>Socket 通訊時的行為</summary>
            public SocketFlags Flag = SocketFlags.None;
            /// <summary>暫存的 Buffer 資料</summary>
            /// <remarks>用於 Buffer 擷取有資料部分，另可用來儲存未完整收到訊息前所收到的資料</remarks>
            public List<byte> RemainBuffer = new List<byte>();
        }
        #endregion

        #region Declaration - Properties

        /// <summary>
        /// [Client] 取得當前與 Server 連線狀態
        /// <para>[Server] 當前是否有任一 Client 連線至 Server</para>
        /// </summary>
        public bool IsConnected {
            get {
                if (Mode == SocketModes.CLIENT && mSocket != null) return mSocket.Connected;
                else if (Mode == SocketModes.SERVER && mClientList.Count > 0) return true;
                else return false;
            }
        }

        /// <summary>[Server Only] 取得當前是否處於接聽中狀態</summary>
        /// <remarks>因 mThread_ServerListen 用於一直接受連線，故此 Thread 活著表示 Listening</remarks>
        public bool IsListening {
            get { return (mThread_ServerListen != null) ? mThread_ServerListen.IsAlive : false; }
        }

        /// <summary>[Server Only] 取得當前 Client 連線數量</summary>
        public int ClientCount {
            get { return mClientList.Count; }
        }

        /// <summary>取得或設定當前 Socket 連線模式。Client 或 Server</summary>
        public SocketModes Mode { get; set; }

        /// <summary>取得或設定連線協議。 TCP/IP 或 Telnet</summary>
        public CommunicationModes CommunicationMode { get; set; }

        /// <summary>取得或設定編碼。用於傳送、接收時 byte 和 string 之間之轉換</summary>
        public CodePages CodePage { get; set; }

        /// <summary>取得或設定事件回傳的資料格式</summary>
        public TransmissionDataFormats DataFormat { get; set; }

        /// <summary>
        /// 取得或設定 Socket 目標裝置之 網際網路位置(IP)
        /// <para>[Client] 目標 Server 之網際網路位置</para>
        /// <para>[Server] 本機開啟之網際網路位置</para>
        /// </summary>
        public string IP {
            get { return mIP.ToString(); }
            set { mIP = IPAddress.Parse(value); }
        }

        /// <summary>
        /// 取得或設定 Socket 目標裝置之 埠號(Port)
        /// <para>[Client] 連線至 Server 之埠號</para>
        /// <para>[Server] 本機監聽之埠號</para>
        /// </summary>
        public int Port {
            get { return mPort; }
            set { mPort = value; }
        }

        /// <summary>取得或設定讀寫資料時之結尾符號</summary>
        public EndChar EndOfLineSymbol { get; set; }

        /// <summary>取得或設定自訂義之結尾符號</summary>
        public string CustomEndOfLine {
            get { return Encoding.GetEncoding((int)CodePage).GetString(mEOLByte); }
            set { mEOLByte = Encoding.GetEncoding((int)CodePage).GetBytes(value); }
        }
        #endregion

        #region Declaration - Events
        /// <summary>CtSyncSocket 事件集合</summary>
        public enum SocketEvents : byte {
            /// <summary>[Client] Connection status with Server</summary>
            CONNECTED_WITH_SERVER,
            /// <summary>[Server] Listen Status</summary>
            LISTEN,
            /// <summary>[Server] One Client had connected</summary>
            CLIENT_CONNECTED,
            /// <summary>Exception Occurred</summary>
            EXCEPTION,
            /// <summary>
            /// 收到已連線裝置傳送之資料
            /// <para>請用 <see cref="DataInfo"/> 進行拆解</para>
            /// <para>[STRING] 請將 DataInfo.Data 以 string 轉換</para>
            /// <para>[BYTE_ARRAY] 請將 DataInfo.Data 以 List&lt;byte&gt; 轉換</para>
            /// </summary>
            DATA_RECEIVED
        }

        /// <summary>CtSyncSocket 事件參數</summary>
        public class SocketEventArgs : EventArgs {
            /// <summary>事件</summary>
            public SocketEvents Event { get; set; }
            /// <summary>此事件所附帶之數值</summary>
            public object Value { get; set; }
            /// <summary>建立事件參數</summary>
            /// <param name="events">事件</param>
            /// <param name="value">此事件所附帶之數值</param>
            public SocketEventArgs(SocketEvents events, object value) {
                Event = events;
                Value = value;
            }
        }

        /// <summary>CtSyncSocket 集合式事件</summary>
        public event EventHandler<SocketEventArgs> OnSocketEvents;

        /// <summary>觸發事件</summary>
        /// <param name="e">Event Arguments</param>
        protected virtual void RaiseEvents(SocketEventArgs e) {
            EventHandler<SocketEventArgs> handler = OnSocketEvents;
            if (handler != null)
                handler(this, e);

        }
        #endregion

        #region Declaration - Members
        /// <summary>此 CtSyncSocket 欲連線之目標 IPAddress</summary>
        private IPAddress mIP;
        /// <summary>此 CtSyncSocket 欲連線之目標 Port</summary>
        private int mPort;

        /// <summary>
        /// 暫存 Socket 連線物件
        /// <para>[Client] 與 Server 連線之 Socket</para>
        /// <para>[Server] 接收 Client 連線之 Listen Socket</para>
        /// </summary>
        protected Socket mSocket;
        /// <summary>[Server] 當前已連線的 Client 集合</summary>
        protected List<Socket> mClientList = new List<Socket>();

        /// <summary>相對應結尾符號之byte</summary>
        /// <remarks>搭配 EndOfLineSymbol == EndChar.CUSTOM</remarks>
        private byte[] mEOLByte;

        /// <summary>[Thread] [Client] 監控與 Server 連線串流之執行緒</summary>
        protected Thread mThread_ClientReceiveData;
        /// <summary>[Thread] [Server] 持續性的監聽，如有 Client 連上則另開 Thread 來接收資料</summary>
        protected Thread mThread_ServerListen;

        /// <summary>執行緒事件，用於 Server 相關操作</summary>
        private ManualResetEvent mRstEvtSrv = new ManualResetEvent(false);
        /// <summary>執行緒事件，用於 Client 連線操作</summary>
        private ManualResetEvent mRstEvtClnCnt = new ManualResetEvent(false);
        /// <summary>執行緒事件，用於 Client 傳送資料操作</summary>
        private ManualResetEvent mRstEvtClnTx = new ManualResetEvent(false);
        /// <summary>執行緒事件，用於 Client 接受資料操作</summary>
        private ManualResetEvent mRstEvtClnRx = new ManualResetEvent(false);

        /// <summary>[Flag] 中斷連線狀態</summary>
        /// <remarks>由於觸發 Disconnect 時也會觸發一次 ReceiveCallback，屆時將會觸發兩次斷線事件，故用此 Flag 卡住不要讓他發兩次</remarks>
        private bool mDisconnecting = false;
        #endregion

        #region Function - Constructors
        /// <summary>
        /// 建立全新的 CtSyncSocket
        /// <para>請自行設定相關環境後再自行使用 <see cref="ServerListen()"/> 或 <seealso cref="ClientConnect()"/> 連線</para>
        /// </summary>
        public CtAsyncSocket(TransmissionDataFormats format = TransmissionDataFormats.STRING) {
            mIP = IPAddress.Parse("127.0.0.1");
            mPort = 2909;
            DataFormat = format;
        }

        /// <summary>建立一個以 Server 為主的 CtSyncSocket</summary>
        /// <param name="portNum">此 Server Port</param>
        /// <param name="commMode">Communication Protocol, TCP/IP or Telnet</param>
        /// <param name="autoConnect">是否開始監聽</param>
        /// <param name="format">接收與傳送資料格式 <see cref="TransmissionDataFormats"/></param>
        public CtAsyncSocket(int portNum, CommunicationModes commMode = CommunicationModes.TCP_IP, TransmissionDataFormats format = TransmissionDataFormats.STRING, bool autoConnect = true) {
            mIP = IPAddress.Parse("127.0.0.1");
            mPort = portNum;
            CommunicationMode = commMode;
            DataFormat = format;
            Mode = SocketModes.SERVER;
            if (autoConnect) ServerListen(portNum);
        }

        /// <summary>建立一個 CtSyncSocket，並帶入基礎設定</summary>
        /// <param name="ipAddr">
        /// IP Address
        /// <para>[Client Mode] 欲連接之 Server IP</para>
        /// <para>[Server Mode] 忽略此項目！請隨意帶入字串即可</para>
        /// </param>
        /// <param name="portNum">
        /// 埠號 (Port)
        /// <para>[Client Mode] 欲連接之 Server Port</para>
        /// <para>[Server Mode] 提供 Client 連線之 Port</para>
        /// </param>
        /// <param name="socketMode">此 CtSyncSocket 之連線角色。 Client 或 Server</param>
        /// <param name="commMode">Communication protocol, TCP/IP or Telnet</param>
        /// <param name="autoConnect">是否開始監聽</param>
        public CtAsyncSocket(string ipAddr, int portNum, SocketModes socketMode, CommunicationModes commMode = CommunicationModes.TCP_IP, bool autoConnect = true) {
            mIP = (socketMode == SocketModes.CLIENT) ? IPAddress.Parse(ipAddr) : IPAddress.Parse("127.0.0.1");
            mPort = portNum;
            CommunicationMode = commMode;
            Mode = socketMode;
            if (autoConnect && socketMode == SocketModes.CLIENT) ClientConnect(ipAddr, portNum);
            else if (autoConnect) ServerListen(portNum);
        }
        #endregion

        #region Function - Dispose
        /// <summary>關閉各端點連線，並釋放資源</summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>關閉各端點連線，並釋放資源</summary>
        /// <param name="isDisposing">是否為第一次釋放</param>
        protected virtual void Dispose(bool isDisposing) {
            try {
                if (isDisposing) {
                    if (Mode == SocketModes.CLIENT) {
                        ClientDisconnect();
                    } else {
                        ServerStopListen();
                    }
                }
            } catch (Exception ex) {
                ExceptionHandler(Stat.ER_SYSTEM, ex);
            }
        }
        #endregion

        #region Function - Methods
        /// <summary>統一發報 Exception</summary>
        /// <param name="stt">Status Code</param>
        /// <param name="title">標題</param>
        /// <param name="msg">訊息</param>
        private void ExceptionHandler(Stat stt, string title, string msg) {
            CtStatus.Report(stt, title, msg);
            RaiseEvents(new SocketEventArgs(SocketEvents.EXCEPTION, "[" + title + "] " + msg));
        }

        /// <summary>統一發報 Exception</summary>
        /// <param name="stt">Status Code</param>
        /// <param name="ex">由系統收集之例外訊息。請參考 <see cref="Exception"/></param>
        private void ExceptionHandler(Stat stt, Exception ex) {
            string title = "";
            CtStatus.Report(stt, ex, out title);
            RaiseEvents(new SocketEventArgs(SocketEvents.EXCEPTION, "[" + title + "] " + ex.Message));
        }

        /// <summary>Convert socket KeepAlive parameters from integer to byte, using for IOControl</summary>
        /// <param name="OnOff">(0)ON  (1)OFF</param>
        /// <param name="aliveTime">Continuous detect connection status after listen</param>
        /// <param name="aliveInterval">Interval of each detect (millisecond)</param>
        /// <returns>byte array data</returns>
        /// <remarks>Useless</remarks>
        private byte[] KeepAlive(short OnOff, short aliveTime, short aliveInterval) {
            byte[] buffer = new byte[12];
            try {
                BitConverter.GetBytes(OnOff).CopyTo(buffer, 0);
                BitConverter.GetBytes(aliveTime).CopyTo(buffer, 4);
                BitConverter.GetBytes(aliveInterval).CopyTo(buffer, 8);
            } catch (Exception ex) {
                ExceptionHandler(Stat.ER_SYSTEM, ex);
            }
            return buffer;
        }

        /// <summary>檢查 Byte 陣列中是否含有目前設定的結尾符號</summary>
        /// <param name="data">欲檢查的資料</param>
        /// <returns>(True)含有結尾符號  (False)不含結尾符號</returns>
        private bool IsContainEndOfLine(List<byte> data) {
            bool result = false;

            if (EndOfLineSymbol == EndChar.NONE) result = true;
            if (EndOfLineSymbol == EndChar.CRLF) result = data.Contains(0x0D) & data.Contains(0x0A);
            else if (EndOfLineSymbol == EndChar.CR) result = data.Contains(0x0D);
            else if (EndOfLineSymbol == EndChar.LF) result = data.Contains(0x0A);
            else if (EndOfLineSymbol == EndChar.CUSTOM) {
                result = true;
                foreach (byte item in mEOLByte) {
                    if (!data.Contains(item)) {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>[Server] 回傳已連接的 Client 之 IP 與 Port 字串</summary>
        /// <returns>IP 與 Port 字串</returns>
        public List<string> GetClientAddress() {
            List<string> strTemp = null;
            if (mClientList.Count > 0) {
                strTemp = mClientList.ConvertAll(
                    new Converter<Socket, string>(
                        socket =>
                            (socket.RemoteEndPoint as IPEndPoint).ToString()
                    )
                );
            }
            return strTemp;
        }
        #endregion

        #region Function - Core

        #region Client
        /// <summary>[Client] 嘗試使用所設定之 mIP 與 mPort 來進行與 Server 之連線</summary>
        /// <remarks>在執行此方法之前，請先確認 IP 與 Port 是否正確！否則將會以 Exception 方式跳出</remarks>
        public void ClientConnect() {
            ClientConnect(mIP.ToString(), mPort);
        }

        /// <summary>
        /// [Client] 帶入 Server IP 與 Port 並嘗試進行連線。並且會等待連線後才離開</summary>
        /// <param name="ip">目標 Server 之 IP Address</param>
        /// <param name="port">目標 Server 之 Port</param>
        public virtual void ClientConnect(string ip, int port) {
            /*-- Clear the disconnecting flag --*/
            mDisconnecting = false;

            /* --------------------------------------------------------------------------------
             *  Establish the remote endpoint for the socket.
             *  The name of the remote device is "ip".
             *  The method from MSDN using DNS.GetEntry to analysis ip, like "tw.yahoo.com".
             *  But if there is lan, it will cause exception
             *  So I change the EndPoint with parse ip directly, as CtSyncSocket.
             * -------------------------------------------------------------------------------- */
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);

            /*-- Create a new socket, Of course can use "new Socket()" directly --*/
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            /*-- Start an asynchronous method to connect with server. When connect triggered, it will callback to "Client_ConnectCallback" program --*/
            client.BeginConnect(remoteEP, new AsyncCallback(Client_ConnectCallback), client);

            /*-- Wating for a connect is triggered --*/
            mRstEvtClnCnt.WaitOne();

            /*-- If connected successfully, execute thread to monitor receive --*/
            if (client.Connected) {
                CtThread.CreateThread(ref mThread_ClientReceiveData, "CtAsyncSocket_Client", tsk_Client_RxData);
                mThread_ClientReceiveData.Start(client);    //pass-in client socket for monitor
            }
        }

        /// <summary>[Client] 中斷與 Server 之連線</summary>
        public virtual void ClientDisconnect() {
            /*-- Re-Assign Socket Mode --*/
            Mode = SocketModes.CLIENT;

            /*-- Set the disconnecting flag --*/
            mDisconnecting = true;

            /*-- Kill thread BKFore close socket, to avoid "ObjectDisposedException" --*/
            CtThread.KillThread(ref mThread_ClientReceiveData);

            /*-- Shutdown send and receive stream, and close stream --*/
            mSocket.Shutdown(SocketShutdown.Both);
            mSocket.Close();
            mSocket = null;

            /* Raise event to notify connection is disconnected */
            RaiseEvents(
                new SocketEventArgs(
                        SocketEvents.CONNECTED_WITH_SERVER,
                        new ConnectInfo(
                                false,
                                mIP.ToString(),
                                mPort
                        )
                )
            );
        }

        /// <summary>[Callback][Client] 非同步連線成功所觸發之回呼</summary>
        /// <param name="asyncResult">非同步作業狀態</param>
        protected virtual void Client_ConnectCallback(IAsyncResult asyncResult) {
            try {
                Socket client = asyncResult.AsyncState as Socket;
                client.EndConnect(asyncResult);

                /*-- Set into global member, more easier operation for other functions --*/
                mSocket = client;
                mPort = (client.LocalEndPoint as IPEndPoint).Port;

                /* Raise event to notify connected with server, and passing IP/Port information */
                RaiseEvents(
                    new SocketEventArgs(
                            SocketEvents.CONNECTED_WITH_SERVER,
                            new ConnectInfo(
                                    true,
                                    (client.LocalEndPoint as IPEndPoint).Address.ToString(),
                                    (client.LocalEndPoint as IPEndPoint).Port
                            )
                    )
                );
            } catch (Exception ex) {
                ExceptionHandler(Stat.ER3_WSK_COMERR, ex);
            } finally {
                /*-- Cause the "ClientConnect" function is wating for signal, so set the signal no matter which connection state --*/
                mRstEvtClnCnt.Set();
            }
        }

        /// <summary>[Callback][Client] 建立新 <see cref="StateObject"/> 並開始非同步接收資料</summary>
        /// <param name="client">已建立連線之 Socket</param>
        protected virtual void Client_Receive(Socket client) {
            StateObject sttObj = new StateObject();
            sttObj.WorkSocket = client;

            client.BeginReceive(sttObj.Buffer, 0, StateObject.BufferSize, sttObj.Flag, out sttObj.Error, new AsyncCallback(ReceiveCallback), sttObj);
        }

        #endregion

        #region Server
        /// <summary>[Server] 使用已設定的 Port 直接進行監聽</summary>
        /// <remarks>執行前請先確認此 Port 為可供連線之 Port，否則將會以 Exception 方式發報</remarks>
        public void ServerListen() {
            ServerListen(mPort);
        }

        /// <summary>[Server] 帶入 Port 並開始進行監聽</summary>
        /// <param name="port">欲開放 Client 連線之 Port</param>
        public virtual void ServerListen(int port) {
            /*-- Clear the disconnecting flag --*/
            mDisconnecting = false;

            /*-- Re-Assign Socket Mode --*/
            Mode = SocketModes.SERVER;

            /* ------------------------------------------------------------
             * Establish the local endpoint for the socket.
             * From MSDN, it using DNS.GetEntry to dismantle IP, like "google.com"
             * But when we stand in a lan, it will cause excpetion
             * I change it back to parse IPAddress directly, as CtSyncSocket.
             * ------------------------------------------------------------ */
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);

            /*-- Create an listen soccket as a Server --*/
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            /*-- Execute listen action --*/
            listener.Bind(localEndPoint);   //您必須在呼叫 Listen 之前呼叫 Bind 方法，否則 Listen 將會擲回 SocketException。
            listener.Listen(100);           //暫止連接佇列的最大長度。The maximum length of the pending connections queue.

            /*-- Start a thread to monitor connection from client, accept or deny --*/
            CtThread.CreateThread(ref mThread_ServerListen, "CtAsyncSocket_Server", tsk_Server_WaitConnect);
            mThread_ServerListen.Start(listener);

            /*-- Set into global member, more easier operation for other function --*/
            mSocket = listener;

            /*-- Raise event to notify start listening --*/
            RaiseEvents(
                new SocketEventArgs(
                        SocketEvents.LISTEN,
                        new ConnectInfo(
                                true,
                                mIP.ToString(),
                                mPort
                        )
                )
            );
        }

        /// <summary>[Server] 中斷所有 Client 連線，並停止監聽</summary>
        public virtual void ServerStopListen() {
            /*-- Announce disconnecting --*/
            mDisconnecting = true;

            /*-- Close each socket which connected --*/
            foreach (Socket item in mClientList) {
                item.Shutdown(SocketShutdown.Both);
                item.Close();
            }

            /*-- Clear the collection of connected sockets --*/
            mClientList.Clear();

            /*-- Kill Thread --*/
            CtThread.KillThread(ref mThread_ServerListen);

            /*-- Close listener --*/
            mSocket.Close();
            mSocket = null;

            /*-- Raise the event --*/
            RaiseEvents(
                new SocketEventArgs(
                        SocketEvents.LISTEN,
                        new ConnectInfo(
                                false,
                                mIP.ToString(),
                                mPort
                        )
                )
            );
        }

        /// <summary>[Server] 中斷特定 IP 與 Port 之 Client 連線</summary>
        /// <param name="ip">IP</param>
        /// <param name="port">Port</param>
        public virtual void ServerStopListen(string ip, int port) {
            /*-- Announce disconnecting --*/
            mDisconnecting = true;

            List<Socket> temp = mClientList.FindAll(socket => (socket.RemoteEndPoint as IPEndPoint).ToString() == ip + ":" + port.ToString());
            foreach (Socket item in temp) {
                mClientList.Remove(item);
                item.Shutdown(SocketShutdown.Both);
                item.Close();

                /*-- Raise the event --*/
                RaiseEvents(
                    new SocketEventArgs(
                            SocketEvents.CLIENT_CONNECTED,
                            new ConnectInfo(
                                    false,
                                    ip,
                                    port
                            )
                    )
                );
            }
        }

        /// <summary>[Callback][Server] 接收 Client 連線之回呼。當有 Client 連線時將會觸發此方法</summary>
        /// <param name="asyncResult">非同步作業狀態</param>
        protected virtual void Server_AcceptCallback(IAsyncResult asyncResult) {
            try {
                /*-- 喚醒 Thread 繼續執行 --*/
                mRstEvtSrv.Set();

                /*-- 取得連接上的 Client 之 Socket --*/
                Socket listener = asyncResult.AsyncState as Socket;
                Socket handler = listener.EndAccept(asyncResult);

                /*-- 新增一個狀態物件專門給這個連線的 Client --*/
                StateObject sttObj = new StateObject();
                sttObj.WorkSocket = handler;

                /*-- 開始非同步接收資料 --*/
                handler.BeginReceive(sttObj.Buffer, 0, StateObject.BufferSize, sttObj.Flag, out sttObj.Error, new AsyncCallback(ReceiveCallback), sttObj);

                /*-- 將這個連線上的 Client 加入清單，並且觸發事件 --*/
                mClientList.Add(handler);
                RaiseEvents(
                    new SocketEventArgs(
                            SocketEvents.CLIENT_CONNECTED,
                            new ConnectInfo(
                                    true,
                                    (handler.RemoteEndPoint as IPEndPoint).Address.ToString(),
                                    (handler.RemoteEndPoint as IPEndPoint).Port
                            )
                    )
                );
            } catch (ObjectDisposedException) {
                /*-- Socket had already closed. Bypass this exception --*/
            } catch (Exception ex) {
                ExceptionHandler(Stat.ER3_WSK_COMERR, ex);
            }
        }

        #endregion

        #region Transmission
        /// <summary>[Callback] 從 NetworkStream 執行非同步讀取資料</summary>
        /// <param name="asyncResult">非同步作業狀態</param>
        protected virtual void ReceiveCallback(IAsyncResult asyncResult) {
            StateObject sttObj = asyncResult.AsyncState as StateObject;
            Socket socket = sttObj.WorkSocket;

            bool isEnd = false;
            int bytesToRead = 0;    //Record how much bytes that read and set into sttObj.Buffer

            /*-- Read data from the socket. And it will retuen how much bytes read --*/
            try {
                bytesToRead = socket.EndReceive(asyncResult, out sttObj.Error);
            } catch (ObjectDisposedException) {
                if (!mDisconnecting)
                    /*-- Raise Event --*/
                    RaiseEvents(
                        new SocketEventArgs(
                                Mode == SocketModes.CLIENT ? SocketEvents.CLIENT_CONNECTED : SocketEvents.CONNECTED_WITH_SERVER,
                                new ConnectInfo(
                                        false,
                                        mIP.ToString(),
                                        mPort
                                )
                        )
                    );
                return;
            } catch (Exception ex) {
                ExceptionHandler(Stat.ER3_WSK_COMERR, ex);
            }

            /*-- Get socket IP and Port --*/
            string ip = (socket.RemoteEndPoint as IPEndPoint).Address.ToString();
            int port = (socket.RemoteEndPoint as IPEndPoint).Port;

            /*-- If there are something passing-in --*/
            if (bytesToRead > 0 && sttObj.Error == SocketError.Success) {

                /* --------------------------------------------------------------------------------- 
                 * Because .Buffer always contains 1024 length, it filled with '\0' after data bytes
                 * So take real data that received into RemainBuffer and do operation with it.
                 * Also EndOfLine enabled, if not finished data, it as a temporary data BKFore finished
                 * --------------------------------------------------------------------------------- */
                sttObj.RemainBuffer.AddRange(sttObj.Buffer.Take(bytesToRead));
                isEnd = IsContainEndOfLine(sttObj.RemainBuffer);    //check is it finished? (True)Finished (False)Not yet

                /*-- If finished, raise data_received event --*/
                if (isEnd) {

                    if (DataFormat == TransmissionDataFormats.BYTE_ARRAY) {

                        /*-- If there are need List<byte>, return it directly --*/
                        RaiseEvents(
                            new SocketEventArgs(
                                    SocketEvents.DATA_RECEIVED,
                                    new DataInfo(
                                            sttObj.RemainBuffer.ToList(),
                                            ip,
                                            port
                                    )
                            )
                        );

                    } else {

                        /*-- Convert byte[] back to string --*/
                        string strData = Encoding.GetEncoding((int)CodePage).GetString(sttObj.RemainBuffer.ToArray()).Trim();

                        /*-- If there are convert successfully, raise the event to push data out --*/
                        RaiseEvents(
                            new SocketEventArgs(
                                    SocketEvents.DATA_RECEIVED,
                                    new DataInfo(
                                            strData,
                                            ip,
                                            port
                                    )
                            )
                        );

                    }

                    sttObj.RemainBuffer.Clear();    //clear buffer, to avoid data comnined with next data
                }

                /* --------------------------------------------------------------------------------------------------
                 * If listening, 'cause without another thread to monitor stream, doing asynchronous receiving here.
                 * If client, there is a thread to monitor stream "tsk_Client_RxData", so it doesn't matter.
                 * 
                 * If EndOfLine enabled and not finished, receiving data continuously with "current data".
                 * Because I using "RemainBuffer.AddRange" to append recently data into RemainBuffer,
                 * so if not finished, it will passing to next asynchon receive callback.
                 * Otherwise, it clean the RemainBuffer, a clear RemainBuffer passing.
                 * -------------------------------------------------------------------------------------------------- */
                if (Mode == SocketModes.SERVER || !isEnd)
                    socket.BeginReceive(sttObj.Buffer, 0, StateObject.BufferSize, sttObj.Flag, out sttObj.Error, new AsyncCallback(ReceiveCallback), sttObj);

            } else if (Mode == SocketModes.CLIENT && !mDisconnecting) { //If this exception is cause by "ClientDisconnect", skip this.
                /*-- If bytesToRead is zero, it means stream closed by remote endpoint. Close local socket.  --*/
                socket.Close();

                /*-- Kill thread, to avoid receive data continuously and "ObjectDisposedException" --*/
                CtThread.KillThread(ref mThread_ClientReceiveData);

                /*-- Raise event to notify there is stream closed --*/
                RaiseEvents(
                    new SocketEventArgs(
                            SocketEvents.CONNECTED_WITH_SERVER,
                            new ConnectInfo(
                                    false,
                                    ip,
                                    port
                            )
                    )
                );

            } else if (Mode == SocketModes.SERVER && !mDisconnecting) { //If this exception is cause by "ServerStopListen", skip this
                /*-- If socket shutdown and already read all data from stream buffer, it will return bytesToRead = 0 --*/
                /*-- Remove the Socket from list --*/
                mClientList.Remove(socket);
                socket.Disconnect(true);

                /*-- Raise Event --*/
                RaiseEvents(
                    new SocketEventArgs(
                            SocketEvents.CLIENT_CONNECTED,
                            new ConnectInfo(
                                    false,
                                    ip,
                                    port
                            )
                    )
                );
            }

            /*-- Signal that all bytes have been received --*/
            if (Mode == SocketModes.CLIENT) mRstEvtClnRx.Set();
            else mRstEvtSrv.Set();
        }

        /// <summary>傳送資料</summary>
        /// <param name="data">欲傳送的資料字串</param>
        /// <param name="codePage">編碼。用於 byte、string 互相轉換</param>
        /// <param name="eachOne">
        /// [Server Only] 是否為廣播方式? 即傳送至所有 Client 或僅傳送至 最先連線之 Client
        /// <para>[Client] Ignore!!</para>
        /// </param>
        /// <remarks>目前沒有加入 EndOfLine 於引數，因為大部分走 Socket 比較少含有結尾符號，因為速度較快不像 RS-232 易有斷開情況。如有特殊需求(如 CommunicationProtocol)請自行加於 data 裡</remarks>
        public virtual void Send(string data, CodePages codePage = CodePages.BIG5, bool eachOne = true) {
            /*-- Convert data from string to byte array --*/
            byte[] txBytes;
            if (CommunicationMode == CommunicationModes.TELNET && !data.EndsWith(CtConst.NewLine))
                txBytes = Encoding.GetEncoding((int)codePage).GetBytes(data + CtConst.NewLine);
            else
                txBytes = Encoding.GetEncoding((int)codePage).GetBytes(data);

            /*-- If pass-in string is not zero, but can't convert it to byte data, throw a excpetion. It may passing chinese but using CodePages.ANSI --*/
            if ((data != "") && (txBytes == null)) throw (new Exception("Convert string to byte array failed. Origion string is \"" + data + "\""));

            /*-- Send data --*/
            if (Mode == SocketModes.CLIENT) {
                mSocket.BeginSend(txBytes, 0, txBytes.Length, 0, new AsyncCallback(SendCallback), mSocket);
            } else if (Mode == SocketModes.SERVER && !eachOne && mClientList.Count > 0) {
                mClientList.Last().BeginSend(txBytes, 0, txBytes.Length, 0, new AsyncCallback(SendCallback), mSocket);  // if not send to each client, just send to last one.
            } else if (eachOne) {
                foreach (Socket item in mClientList) {
                    item.BeginSend(txBytes, 0, txBytes.Length, 0, new AsyncCallback(SendCallback), item);
                }
            }
        }

        /// <summary>傳送資料</summary>
        /// <param name="data">欲傳送的資料集合</param>
        /// <param name="eachOne">
        /// [Server Only] 是否為廣播方式? 即傳送至所有 Client 或僅傳送至 最先連線之 Client
        /// <para>[Client] Ignore!!</para>
        /// </param>
        public virtual void Send(List<byte> data, bool eachOne = true) {
            if (Mode == SocketModes.CLIENT || (Mode == SocketModes.SERVER && !eachOne && mClientList.Count > 0)) {
                mSocket.BeginSend(data.ToArray(), 0, data.Count, 0, new AsyncCallback(SendCallback), mSocket);
            } else if (Mode == SocketModes.SERVER && !eachOne && mClientList.Count > 0) {   // if not send to each client, just send to last one
                mClientList.Last().BeginSend(data.ToArray(), 0, data.Count, 0, new AsyncCallback(SendCallback), mSocket);
            } else if (eachOne) {
                foreach (Socket item in mClientList) {
                    item.BeginSend(data.ToArray(), 0, data.Count, 0, new AsyncCallback(SendCallback), item);
                }
            }
        }

        /// <summary>[Callback] 傳送資料之回呼。傳送資料完畢後將回呼此方法</summary>
        /// <param name="asyncResult">非同步作業狀態</param>
        protected virtual void SendCallback(IAsyncResult asyncResult) {
            Socket socket = asyncResult.AsyncState as Socket;
            int bytesToSend = socket.EndSend(asyncResult);
        }
        #endregion

        #endregion

        #region Function - Threads
        /// <summary>[Server] 持續等待非同步連接</summary>
        /// <param name="socket">已開啟監聽之 Socekt</param>
        protected virtual void tsk_Server_WaitConnect(object socket) {
            Socket listener = socket as Socket;
            do {
                try {
                    // Set the event to nonsignaled state.
                    mRstEvtSrv.Reset();

                    // Start an asynchronous socket to listen for connections.
                    listener.BeginAccept(
                        new AsyncCallback(Server_AcceptCallback),
                        listener);

                    // Wait until a connection is made BKFore continuing.
                    mRstEvtSrv.WaitOne();

                } catch (ThreadInterruptedException) {
                    /*-- Using to catch exception of exit thread, but not report --*/
                } catch (ThreadAbortException) {
                    /*-- Using to catch exception of exit thread, but not report --*/
                } catch (Exception ex) {
                    if (mClientList.Count > 0) ExceptionHandler(Stat.ER_SYSTEM, ex);
                }
            } while (mThread_ServerListen.IsAlive);
        }

        /// <summary>[Client] 持續施作非同步讀取資料</summary>
        /// <param name="socket">已連接至 Server 之 Socket</param>
        protected virtual void tsk_Client_RxData(object socket) {
            Socket client = socket as Socket;
            do {
                try {
                    // Set the event to nonsignaled state.
                    mRstEvtClnRx.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Client_Receive(client);

                    // Wait until a connection is made BKFore continuing.
                    mRstEvtClnRx.WaitOne();

                } catch (ThreadInterruptedException) {
                    /*-- Using to catch exception of exit thread, but not report --*/
                } catch (ThreadAbortException) {
                    /*-- Using to catch exception of exit thread, but not report --*/
                } catch (Exception ex) {
                    ExceptionHandler(Stat.ER_SYSTEM, ex);
                    break;
                }
            } while (mThread_ClientReceiveData.IsAlive);
        }

        #endregion
    }
}
