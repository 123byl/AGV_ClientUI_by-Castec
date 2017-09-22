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
    /// 實作 <see cref="Socket"/> (TCP/IP) 相關控制。分為 用戶端(<see cref="TcpClient"/>) 與 伺服端(<see cref="TcpListener"/>)，並藉此互相傳輸資料。
    /// <para><see cref="TcpClient"/> - 給予埠號(Port)並以本機作為伺服器，啟動後等待 Client 連接。可接受多個 Client，傳送資料時可選擇廣播或單一對象傳送；接收資料則會帶有 Client 資訊</para>
    /// <para><see cref="TcpListener"/> - 給予欲連線 Server 端之 網際網路位置(IP) 與 埠號(Port) 並藉此嘗試連線至 Server</para>
    /// <para>不論何種模式，都將透過執行緒(<see cref="Thread"/>)來監控串流(<see cref="NetworkStream"/>)</para>
    /// </summary>
    /// <example>
    /// Client 連線方式
    /// <code>
    /// CtSyncSocket mSocket = new CtSyncSocket();
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
    /// CtSyncSocket mSocket = new CtSyncSocket();
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
    public class CtSyncSocket : IDisposable {

        #region Version

        /// <summary>CtSyncSocket 版本相關訊息</summary>
        /// <remarks><code>
        /// 0.0.0  William [2013/01/23]
        ///     + CtSockets
        ///     
        /// 1.0.0  Ahern [2014/11/20]
        ///     + Translate from VB
        ///     
        /// 1.0.1  Ahern [2014/12/15]
        ///     \ 註解由 English 轉回 繁體中文，避免詞不達意
        ///     
        /// 1.0.2  Ahern [2015/02/16]
        ///     \ 修改部分物件屬性為 protected 以讓外部繼承
        ///     \ tsk 部分加入 virtual 修飾詞以讓外部 Override
        ///     
        /// 1.0.3  Ahern [2015/03/15]
        ///     + EndOfLineSymbol
        ///     + CustomEndOfLine
        ///     
        /// 1.0.4  Ahern [2015/03/21]
        ///     \ ClientDisconnect 之 mSocket 處置，避免存取已處置物件
        ///     \ tsk_Server_RxData 之 IOException 處置，避免存取已處置物件
        ///     - Try-Catch
        ///     
        /// 1.0.5  Ahern [2015/04/30]
        ///     \ 重新命名為 CtSyncSocket 以區分 CtAsyncSocket
        ///     \ DataFormat 改為 internal set
        ///     
        /// 1.0.6  Ahern [2015/05/04]
        ///     \ 加入 Poll 判斷 Socket 是否斷線
        ///     
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 0, 6, "2015/05/04", "Ahern Kuo");

        #endregion

        #region Declaration - Definitions
        /// <summary>接收每筆資料間的延遲時間，單位為 毫秒(Millisecond, ms)</summary>
        private static readonly int RECEIVE_DATA_DELAY = 20;
        #endregion

        #region Declaration - Support Class
        /// <summary>
        /// 連線資訊。包含 連接/斷線、時間、對方IP與Port
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
        #endregion

        #region Declaration - Properties

        /// <summary>
        /// [Client] 取得當前與 Server 連線狀態
        /// <para>[Server] 當前是否有任一 Client 連線至 Server</para>
        /// </summary>
        public bool IsConnected {
            get {
                if (Mode == SocketModes.CLIENT && mClient != null)
                    return (mSocket == null) ? false : !((mSocket.Poll(1000, SelectMode.SelectRead) && (mSocket.Available == 0)) || !mSocket.Connected);
                else if (Mode == SocketModes.SERVER && mServer != null && mClientList.Count > 0) return true;
                else return false;
            }
        }

        /// <summary>[Server Only] 取得當前 Client 連線數量</summary>
        public int ClientCount {
            get { return (mServer != null) ? mClientList.Count : 0; }
        }

        /// <summary>取得或設定當前 Socket 連線模式。Client 或 Server</summary>
        public SocketModes Mode { get; set; }

        /// <summary>取得或設定連線協議。 TCP/IP 或 Telnet</summary>
        public CommunicationModes CommunicationMode { get; set; }

        /// <summary>取得或設定編碼。用於傳送、接收時 byte 和 string 之間之轉換</summary>
        public CodePages CodePage { get; set; }

        /// <summary>取得或設定事件回傳的資料格式</summary>
        public TransmissionDataFormats DataFormat { get; internal set; }

        /// <summary>取得或設定 Socket 目標裝置之 網際網路位置(IP)</summary>
        public string IP {
            get { return mIP.ToString(); }
            set { mIP = IPAddress.Parse(value); }
        }

        /// <summary>取得或設定 Socket 目標裝置之 埠號(Port)</summary>
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
        /// <summary>[Client] Client object, using for client mode</summary>
        protected TcpClient mClient;
        /// <summary>[Server] Server object, using for server mode</summary>
        protected TcpListener mServer;
        /// <summary>
        /// 暫存 Socket 連線物件
        /// <para>[Client] 與 Server 連線之 Socket</para>
        /// <para>[Server] 最後一個連上線之 Client。此為變動值！</para>
        /// </summary>
        protected Socket mSocket;
        /// <summary>[Thread] [Client] 監控與 Server 連線串流之執行緒</summary>
        protected Thread mThread_ClientReceiveData;
        /// <summary>[Thread] [Server] 持續性的監聽，如有 Client 連上則另開 Thread 來接收資料</summary>
        protected Thread mThread_ServerListen;
        /// <summary>[Server] 當前已連線的 Client 集合</summary>
        protected List<Socket> mClientList = new List<Socket>();
        /// <summary>[Server] 當前已連線的 Client 執行緒</summary>
        protected List<Thread> mServerList = new List<Thread>();
        /// <summary>相對應結尾符號之byte</summary>
        /// <remarks>搭配 EndOfLineSymbol == EndChar.CUSTOM</remarks>
        private byte[] mEOLByte;
        /// <summary>用於判斷是否為手動斷線</summary>
        private bool mFlag_Disconnect = false;
        #endregion

        #region Function - Constructors
        /// <summary>
        /// 建立全新的 CtSyncSocket
        /// <para>請自行設定相關環境後再自行連線</para>
        /// </summary>
        /// <param name="dataFormat">接收與傳送資料格式 <see cref="TransmissionDataFormats"/></param>
        public CtSyncSocket(TransmissionDataFormats dataFormat = TransmissionDataFormats.STRING) {
            mIP = IPAddress.Parse("127.0.0.1");
            mPort = 2909;
            DataFormat = dataFormat;
        }

        /// <summary>建立一個以 Server 為主的 CtSyncSocket</summary>
        /// <param name="portNum">此 Server Port</param>
        /// <param name="commMode">Communication Protocol, TCP/IP or Telnet</param>
        /// <param name="autoConnect">是否開始監聽</param>
        /// <param name="format">接收與傳送資料格式 <see cref="TransmissionDataFormats"/></param>
        public CtSyncSocket(int portNum, CommunicationModes commMode = CommunicationModes.TCP_IP, TransmissionDataFormats format = TransmissionDataFormats.STRING, bool autoConnect = true) {
            mIP = IPAddress.Parse("127.0.0.1");
            mPort = portNum;
            CommunicationMode = commMode;
            DataFormat = format;
            Mode = SocketModes.SERVER;
            if (autoConnect) ServerListen();
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
        /// <param name="format">接收與傳送資料格式 <see cref="TransmissionDataFormats"/></param>
        /// <param name="autoConnect">是否開始監聽</param>
        public CtSyncSocket(string ipAddr, int portNum, SocketModes socketMode, CommunicationModes commMode = CommunicationModes.TCP_IP, TransmissionDataFormats format = TransmissionDataFormats.STRING, bool autoConnect = true) {
            mIP = (socketMode == SocketModes.CLIENT) ? IPAddress.Parse(ipAddr) : IPAddress.Parse("127.0.0.1");
            mPort = portNum;
            CommunicationMode = commMode;
            DataFormat = format;
            Mode = socketMode;
            if (autoConnect && socketMode == SocketModes.CLIENT) ClientConnect();
            else if (autoConnect) ServerListen();
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

        /// <summary>Byte陣列中是否含有目前設定的結尾符號</summary>
        /// <param name="data">欲檢查的資料</param>
        /// <returns>(True)含有結尾符號  (False)不含結尾符號</returns>
        private bool IsContainEndOfLine(List<byte> data) {
            bool result = false;

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
        public virtual void ClientConnect() {

            mFlag_Disconnect = false;

            /*-- Re-Assign Socket Mode --*/
            Mode = SocketModes.CLIENT;

            /*-- Create EndPoint and new a Client with all accessible ip --*/
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 0);
            if (mClient != null) {
                if (mClient.Connected) ClientDisconnect();
                mClient = null;
            }
            mClient = new TcpClient(localEndPoint);

            /*-- Check the client object --*/
            if (mClient == null) throw (new Exception("Couldn't create TcpClient with accessible IPs"));

            /*-- Try to connect --*/
            IPEndPoint remoteEndPoint = new IPEndPoint(mIP, mPort);     //Create the IPEndPoint with Server IP and Port that assigned
            mClient.Connect(remoteEndPoint);    //establish connect

            /*-- If connect sucessfully, start the thread to receive data from stream buffer continuously --*/
            if (mClient.Connected) {
                CtThread.CreateThread(ref mThread_ClientReceiveData, "CtSocket_RxData", tsk_Client_RxData);
                /* Raise the event to notify connection status and informations */
                RaiseEvents(
                    new SocketEventArgs(
                            SocketEvents.CONNECTED_WITH_SERVER,
                            new ConnectInfo(
                                    true,
                                    (mClient.Client.RemoteEndPoint as IPEndPoint).Address.ToString(),
                                    (mClient.Client.RemoteEndPoint as IPEndPoint).Port
                            )
                    )
                );

                mSocket = mClient.Client;

            } else
                throw (new Exception("Connected with Server, but there are no response from server"));
        }

        /// <summary>[Client] 帶入 Server IP 與 Port 並嘗試進行連線</summary>
        /// <param name="ip">目標 Server 之 IP Address</param>
        /// <param name="port">目標 Server 之 Port</param>
        public virtual void ClientConnect(string ip, int port) {
            /*-- Try to parse IP, prevent enter a invalid string --*/
            IPAddress addr;
            if (IPAddress.TryParse(ip, out addr)) mIP = addr;
            else throw (new Exception("Invalid IP Address"));

            /*-- Set port --*/
            mPort = port;

            /*-- Establish connection --*/
            ClientConnect();
        }

        /// <summary>[Client] 中斷與 Server 之連線</summary>
        public virtual void ClientDisconnect() {
            mFlag_Disconnect = true;

            /*-- Disconnect and close --*/
            mSocket.Close();
            //mSocket.Dispose();
            mSocket = null;
            mClient = null;

            /*-- Kill Thread --*/
            CtThread.KillThread(ref mThread_ClientReceiveData);

            /*-- Raise the connection event --*/
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
        #endregion

        #region Server
        /// <summary>[Server] 使用已設定的 Port 直接進行監聽</summary>
        /// <remarks>執行前請先確認此 Port 為可供連線之 Port，否則將會以 Exception 方式發報</remarks>
        public virtual void ServerListen() {

            mFlag_Disconnect = false;

            /*-- Re-Assign Socket Mode --*/
            Mode = SocketModes.SERVER;

            /*-- If the mServer had been built, stop listen --*/
            if (mServer != null) ServerStopListen();

            /*-- Create the TcpListener with specified port properties --*/
            mServer = new TcpListener(IPAddress.Any, mPort);

            if (mServer != null) mServer.Start();
            else throw (new Exception("Couldn't new a TcpListener object"));

            /*-- If create successfully, start to listen with thread! --*/
            CtThread.CreateThread(ref mThread_ServerListen, "CtSocket_Listen", tsk_Server_Listen);

            if (mThread_ServerListen.IsAlive) {
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
        }

        /// <summary>[Server] 帶入 Port 並開始進行監聽</summary>
        /// <param name="port">欲開放 Client 連線之 Port</param>
        public virtual void ServerListen(int port) {
            mPort = port;
            ServerListen();
        }

        /// <summary>[Server] 中斷所有 Client 連線，並停止監聽</summary>
        public virtual void ServerStopListen() {

            mFlag_Disconnect = true;

            /*-- Stop listen --*/
            mServer.Server.Close();
            mServer.Stop();

            /*-- Close each socket which connected --*/
            foreach (Socket item in mClientList) {
                item.Shutdown(SocketShutdown.Both);
                item.Disconnect(true);
                item.Close();
            }

            /*-- Kill receive data thread --*/
            for (int idx = 0; idx < mServerList.Count; idx++) {
                Thread thrRxData = mServerList[idx];
                CtThread.KillThread(ref thrRxData);
            }

            mServerList.Clear();

            /*-- Clear the collection of connected sockets --*/
            mClientList.Clear();

            /*-- Kill Thread --*/
            CtThread.KillThread(ref mThread_ServerListen);

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

        /// <summary>[Server] 中斷所有 Client 連線，並停止監聽</summary>
        public virtual void ServerStopListen(string ip, int port) {

            List<Socket> temp = mClientList.FindAll(socket => (socket.RemoteEndPoint as IPEndPoint).ToString() == ip + ":" + port.ToString());

            /*-- Close each socket which connected --*/
            foreach (Socket item in temp) {
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
        #endregion

        #region Transmission
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
            if (CommunicationMode == CommunicationModes.TELNET)
                txBytes = Encoding.GetEncoding((int)codePage).GetBytes(data + CtConst.NewLine);
            else
                txBytes = Encoding.GetEncoding((int)codePage).GetBytes(data);

            if ((data != "") && (txBytes == null)) throw (new Exception("Convert string to byte array failed"));

            /*-- Create and get network stream of socket, sending data after opened --*/
            NetworkStream netStream;
            if (Mode == SocketModes.CLIENT) {
                netStream = mClient.GetStream();                    //Client Mode, Get the stream from the TcpClient that streaming with Server
                netStream.Write(txBytes, 0, txBytes.Length);        //Wrtie Data
            } else if (eachOne) {
                foreach (Socket socket in mClientList) {
                    netStream = new NetworkStream(socket);          //If server mode and send to everyone, create with each Socket object that client connected
                    netStream.Write(txBytes, 0, txBytes.Length);    //Write Data
                }
            } else if (mClientList.Count > 0) {
                netStream = new NetworkStream(mClientList[0]);      //If just send to first connected client
                netStream.Write(txBytes, 0, txBytes.Length);        //Write Data
            } else throw (new Exception("There are not exists any connected clients"));
        }

        /// <summary>傳送資料</summary>
        /// <param name="data">欲傳送的資料集合</param>
        /// <param name="eachOne">
        /// [Server Only] 是否為廣播方式? 即傳送至所有 Client 或僅傳送至 最先連線之 Client
        /// <para>[Client] Ignore!!</para>
        /// </param>
        public virtual void Send(byte[] data, bool eachOne = true) {
            /*-- Create and get network stream of socket, sending data after opened --*/
            NetworkStream netStream;
            if (Mode == SocketModes.CLIENT) {
                netStream = mClient.GetStream();                    //Client Mode, Get the stream from the TcpClient that streaming with Server
                netStream.Write(data, 0, data.Length);        //Wrtie Data
            } else if (eachOne) {
                foreach (Socket socket in mClientList) {
                    netStream = new NetworkStream(socket);          //If server mode and send to everyone, create with each Socket object that client connected
                    netStream.Write(data, 0, data.Length);    //Write Data
                }
            } else if (mClientList.Count > 0) {
                netStream = new NetworkStream(mClientList[0]);      //If just send to first connected client
                netStream.Write(data, 0, data.Length);        //Write Data
            } else throw (new Exception("There are not exists any connected clients"));
        }
        #endregion
        #endregion

        #region Function - Threads
        /// <summary>[Thread] [Client] Receive Data</summary>
        protected virtual void tsk_Client_RxData() {
            string strData = "";
            List<byte> rxData = new List<byte>();
            NetworkStream netStream = mClient.GetStream();
            do {
                try {

                    /* ---------- 2015/05/04 ------------------------------------------------------------
                     * Using mClient.Client.Connected is incorrectly,
                     * because Connected property is rx/tx state of "last" time !!
                     * 
                     * Poll return "true" as connection failed.
                     * But sometimes when connecting or receiving data, Poll will return true.
                     * So add condition Available to double check, Available is not zero when receiving.
                     * ---------------------------------------------------------------------------------- */
                    if (mClient.Client.Poll(100, SelectMode.SelectRead) && mClient.Client.Available == 0)
                        throw (new ObjectDisposedException("Can't poll, stream already closed"));

                    /*-- Read stream buffer into byte[] --*/
                    rxData.Clear();
                    if (EndOfLineSymbol == EndChar.NONE) {
                        while (netStream.DataAvailable) {
                            rxData.Add((byte)netStream.ReadByte());
                        };
                    } else {
                        while (!IsContainEndOfLine(rxData)) {
                            rxData.Add((byte)netStream.ReadByte());
                        };
                    }

                    /*-- If there are something passing-in --*/
                    if (rxData.Count > 0) {

                        /*-- Remove all useless item --*/
                        //List<byte> byteTemp = rxBytes.ToList();


                        if (DataFormat == TransmissionDataFormats.BYTE_ARRAY) {

                            /*-- If there are need List<byte>, return it directly --*/
                            RaiseEvents(
                                new SocketEventArgs(
                                        SocketEvents.DATA_RECEIVED,
                                        new DataInfo(
                                                rxData,
                                                (mClient.Client.LocalEndPoint as IPEndPoint).Address.ToString(),
                                                (mClient.Client.LocalEndPoint as IPEndPoint).Port
                                        )
                                )
                            );
                        } else {

                            /*-- Convert byte[] back to string --*/
                            strData = Encoding.GetEncoding((int)CodePage).GetString(rxData.ToArray()).Trim();

                            if (strData.Trim() != "") {
                                /*-- If there are convert successfully, raise the event to push data out --*/
                                RaiseEvents(
                                    new SocketEventArgs(
                                            SocketEvents.DATA_RECEIVED,
                                            new DataInfo(
                                                    strData,
                                                    (mClient.Client.LocalEndPoint as IPEndPoint).Address.ToString(),
                                                    (mClient.Client.LocalEndPoint as IPEndPoint).Port
                                            )
                                    )
                                );
                            }
                        }
                    }
                    /*-- Dalay --*/
                    Thread.Sleep(RECEIVE_DATA_DELAY);

                } catch (ObjectDisposedException) {
                    if (!mFlag_Disconnect) {
                        if (mSocket != null) mSocket.Close();

                        /*-- Raise disconnect event --*/
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
                    break;
                } catch (ThreadInterruptedException) {
                    /*-- Using to catch exception of exit thread, but not report --*/
                } catch (ThreadAbortException) {
                    /*-- Using to catch exception of exit thread, but not report --*/
                } catch (Exception ex) {
                    ExceptionHandler(Stat.ER_SYSTEM, ex);
                }
            } while (mThread_ClientReceiveData.IsAlive);
        }

        /// <summary>[Thread] [Server] Listening</summary>
        private void tsk_Server_Listen() {
            do {
                try {
                    /*-- Lock TcpListener and return the Socket that can transmit or receive data --*/
                    mSocket = mServer.AcceptSocket();

                    /*-- If there are one Socket connected, raise the connection event and start a thread to monitor --*/
                    if (mSocket.Connected) {

                        Thread thrRxData = CtThread.CreateThread("CtSocket_SrvRxData", tsk_Server_RxData);
                        thrRxData.Start(mSocket);
                        mClientList.Add(mSocket);   //Add to list for sending or record
                        mServerList.Add(thrRxData);

                        RaiseEvents(
                            new SocketEventArgs(
                                    SocketEvents.CLIENT_CONNECTED,
                                    new ConnectInfo(
                                            true,
                                            (mSocket.RemoteEndPoint as IPEndPoint).Address.ToString(),
                                            (mSocket.RemoteEndPoint as IPEndPoint).Port
                                    )
                            )
                        );
                    }

                    /*-- Dalay --*/
                    Thread.Sleep(RECEIVE_DATA_DELAY);

                } catch (ThreadInterruptedException) {
                    /*-- Using to catch exception of exit thread, but not report --*/
                } catch (ThreadAbortException) {
                    /*-- Using to catch exception of exit thread, but not report --*/
                } catch (Exception ex) {
                    if (mClientList.Count > 0 && !mFlag_Disconnect) ExceptionHandler(Stat.ER_SYSTEM, ex);
                }
            } while (mThread_ServerListen.IsAlive);
        }

        /// <summary>[Thread] [Server] Receive Data</summary>
        protected virtual void tsk_Server_RxData(object socket) {
            string strData = "";
            Socket sckTemp = socket as Socket;                      //Temporary Socket from listen thread
            NetworkStream netStream = new NetworkStream(sckTemp);   //Get the stream of the Socket that passing
            List<byte> rxData = new List<byte>();
            string ip = (sckTemp.RemoteEndPoint as IPEndPoint).Address.ToString();
            int port = (sckTemp.RemoteEndPoint as IPEndPoint).Port;
            do {
                try {
                    /* ---------- 2015/05/04 ---------------------------------------
                     * Using mClient.Client.Connected is incorrectly,
                     * because Connected property is rx/tx state of "last" time !!
                     * 
                     * Poll return "true" as connection failed.
                     * ------------------------------------------------------------- */
                    if (sckTemp.Poll(100, SelectMode.SelectRead))
                        throw (new ObjectDisposedException("Can't poll, stream already closed"));

                    /*-- Create the byte[] to receive data --*/
                    rxData.Clear();
                    if (EndOfLineSymbol == EndChar.NONE) {
                        do {
                            rxData.Add((byte)netStream.ReadByte());
                        } while (netStream.DataAvailable);
                    } else {
                        do {
                            rxData.Add((byte)netStream.ReadByte());
                        } while (!IsContainEndOfLine(rxData));
                    }

                    /* ---------- 2015/02/02 ---------------------------------------------------
                     * 原本有發現如果 Client 中斷連線後，會一直收到 rxData[0]=255 單一筆資料
                     * (因為上面獲取資料是用 ReadByte() 的關係??)
                     * 
                     * 後來先用 rxData[0] < 255 卡，但如果真的是 255 一筆資料，那有可能會掛掉
                     * 測試後發現是通道未關閉!! 在 ClientDisconnect 加上 mSocket.Disconnect() 即可
                     * 通道關閉後這邊會直接跳 Exception 然後離開
                     * 
                     * 結論: 如果關閉後還是一直收到資料，請檢查對方的通道是否正確關閉!!
                     * ------------------------------------------------------------------------- */

                    /*-- If there are somehting exist --*/
                    if (rxData.Count > 0) {

                        /*-- Remove useless byte --*/
                        //List<byte> byteTemp = rxBytes.ToList();

                        if (DataFormat == TransmissionDataFormats.BYTE_ARRAY) {
                            /*-- If there are need List<byte>, return it directly --*/
                            RaiseEvents(
                                new SocketEventArgs(
                                        SocketEvents.DATA_RECEIVED,
                                        new DataInfo(
                                                rxData,
                                                ip,
                                                port
                                        )
                                )
                            );
                        } else {

                            /*-- Convert byte[] back to string --*/
                            strData = Encoding.GetEncoding((int)CodePage).GetString(rxData.ToArray()).Trim();

                            if (strData.Trim() != "") {
                                /*-- Raise the event to put data out --*/
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
                        }

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
                                            ip,
                                            port
                                    )
                            )
                        );
                        break;
                    }

                    /*-- Delay --*/
                    Thread.Sleep(RECEIVE_DATA_DELAY);

                } catch (ObjectDisposedException) {
                    if (!mFlag_Disconnect) {
                        /*-- Remove the Socket from list --*/
                        mClientList.Remove(sckTemp);

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
                    break;
                } catch (ThreadInterruptedException) {
                    /*-- Using to catch exception of exit thread, but not report --*/
                } catch (ThreadAbortException) {
                    /*-- Using to catch exception of exit thread, but not report --*/
                } catch (Exception ex) {
                    ExceptionHandler(Stat.ER_SYSTEM, ex);
                }
            } while (mThread_ServerListen.IsAlive);
        }
        #endregion
    }
}
