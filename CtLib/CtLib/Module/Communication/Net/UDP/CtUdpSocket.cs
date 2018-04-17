using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

using CtLib.Library;
using CtLib.Module.Utility;
using System.Runtime.Serialization;

namespace CtLib.Module.Net {
	/// <summary>
	/// UDP 通訊協議的 <see cref="Socket"/> 。其為主從式(用戶端與伺服端)架構，可雙向傳輸資料，但不保證資料傳達。
	/// <para>伺服端 - 給予埠號(Port)並以本機作為伺服監聽程式，隨時監聽訊息。傳輸資料至目標時需指定對方位址與埠號</para>
	/// <para>用戶端 - 給予欲連線 Server 端之 網際網路位置(IP) 與 埠號(Port) 並藉此嘗試連線至 Server</para>
	/// </summary>
	/// <example>
	/// Client 連線方式
	/// <code language="C#">
	/// CtUdpSocket mUdp = new CtUdpSocket(TransDataFormats.String, true);
	/// mUdp.OnSocketEvents += mSocket_OnSocketEvents;		//Add event, it contains connect status, received data and so on
	/// 
	/// mUdp.ClientConnect("192.168.254.254", 7788);		//Connect to "192.168.254.254:7788"
	/// mUdp.WaitConnectToServer(TimeSpan.FromSeconds(5));	//Waiting for connect to server, at most 5 second.
	/// 
	/// /* Do something here */
	/// 
	/// mUdp.ClientDisconnect();							//Close the connection with server
	/// 
	/// </code>
	/// Server 連線方式
	/// <code language="C#">
	/// CtUdpSocket mUdp = new CtUdpSocket(TransDataFormats.String, true);
	/// mUdp.OnSocketEvents += mSocket_OnSocketEvents;   //Add event, it contains connect status, received data and so on
	/// 
	/// mUdp.ServerListen(2909);                         //Start listening stream at "localhost:2909"
	/// 
	/// /* Do something here */
	/// 
	/// mUdp.ServerStopListen();                         //Close all client connect and server stream
	/// </code>
	/// 如果有需要使用自訂義結尾符號，請看以下示範
	/// <code language="C#">
	/// mSocket.EndOfLineSymbol = EndChar.Custom;   //設定使用自定義結尾
	/// mSocket.CustomEndOfLine = "*";              //自訂義的結尾符號為 "*"
	/// 
	/// //如果目標傳送 "Hello*", 你會完整收到 "Hello*" (與 <see cref="SerialPort.CtSerial"/> 不同)!!
	/// </code></example>
	/// <remarks>目前傳送資料部分均不含結尾符號引數，因為大部分走 Socket 比較少含有結尾符號，因為速度較快不像 RS-232 易有斷開情況。如有特殊需求(如 CommunicationProtocol)請自行加於 data 裡</remarks>
	public class CtUdpSocket : CtSocket {

		#region Version

		/// <summary>CtAsyncSocket 版本相關訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0	Ahern	[2017/05/08]
		///		+ 從 origin\master 的 CtAsyncSocket 複製
		///		
		/// 1.1.0	Ahern	[2017/05/16]
		///		+ 區分出 CtTcpSocket 與 CtUdpSocket
		///     
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(1, 1, 0, "2017/05/16", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Fields
		/// <summary>
		/// 用於 SendTo 與 ReceiveFrom 的 EndPoint
		/// [Client] 目標連線的端點
		/// [Server] 本身監聽的端點
		/// </summary>
		private EndPoint mTarEndPoint;
		#endregion

		#region Declaration - Properties

		/// <summary>
		/// [Client] 取得當前與 Server 連線狀態
		/// <para>[Server] 當前是否有任一 Client 連線至 Server</para>
		/// </summary>
		public bool IsConnected {
			get {
				if (Role == SocketRole.Client && mSocket != null) return mSocket.IsConnected();
				else return mClientList.Any(sock => sock.IsConnected());  //就算是 Client 模式且 mSocket 為 null，mClientList 也不會有任何連線，回傳 false
			}
		}

		/// <summary>取得或設定連線協議，此值恆為 <see cref="NetworkProtocol.Udp"/></summary>
		public override NetworkProtocol Protocol { get { return NetworkProtocol.Udp; } }

		/// <summary>[Server] 取得或設定要傳送資料的目標端點</summary>
		public EndPointSlim TargetEndPoint { get; set; }
		#endregion

		#region Function - Constructors

		/// <summary>
		/// 建立資料格式為 <see cref="TransDataFormats.String"/> 的 CtUdpSocket
		/// <para>請自行設定相關環境後再自行連線</para>
		/// </summary>
		/// <param name="format">接收與傳送資料格式 <see cref="TransDataFormats"/>。如欲使用序列化，請指定為 <see cref="TransDataFormats.EnumerableOfByte"/> 並於收到資料後自行返序列化</param>
		/// <param name="rxEvent">是否採用事件接收傳入訊息，若為 false 則請自行使用 Receive 相關方法以接收、等待資料</param>
		public CtUdpSocket(TransDataFormats format, bool rxEvent) : base(format, rxEvent) {

		}

		/// <summary>建立一個以 Server 為主的 CtUdpSocket</summary>
		/// <param name="portNum">欲監聽的埠號</param>
		/// <param name="autoConnect">是否開始監聽</param>
		/// <param name="format">接收與傳送資料格式 <see cref="TransDataFormats"/>。如欲使用序列化，請指定為 <see cref="TransDataFormats.EnumerableOfByte"/> 並於收到資料後自行返序列化</param>
		/// <param name="rxEvent">是否採用事件接收傳入訊息，若為 false 則請自行使用 Receive 相關方法以接收、等待資料</param>
		public CtUdpSocket(int portNum, TransDataFormats format, bool rxEvent, bool autoConnect = true) : base(format, rxEvent) {
			EndPoint = new EndPointSlim("127.0.0.1", portNum);
			Role = SocketRole.Server;
			if (autoConnect) ServerBeginListen(portNum);
		}

		/// <summary>建立一個 CtUdpSocket，並帶入基礎設定</summary>
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
		/// <param name="format">接收與傳送資料格式 <see cref="TransDataFormats"/>。如欲使用序列化，請指定為 <see cref="TransDataFormats.EnumerableOfByte"/> 並於收到資料後自行返序列化</param>
		/// <param name="autoConnect">是否開始監聽</param>
		/// <param name="rxEvent">是否採用事件接收傳入訊息，若為 false 則請自行使用 Receive 相關方法以接收、等待資料</param>
		public CtUdpSocket(string ipAddr, int portNum, SocketRole socketMode, TransDataFormats format, bool rxEvent, bool autoConnect = true) : base(format, rxEvent) {
			if (socketMode == SocketRole.Server) EndPoint = new EndPointSlim("127.0.0.1", portNum);
			else EndPoint = new EndPointSlim(ipAddr, portNum);
			Role = socketMode;
			if (autoConnect && socketMode == SocketRole.Client) ClientBeginConnect(ipAddr, portNum);
			else if (autoConnect) ServerBeginListen(portNum);
		}
		#endregion

		#region Function - Dispose
		/// <summary>關閉各端點連線，並釋放資源</summary>
		public override void Dispose() {
			if (!mDisposed) {
				mDisposed = true;
				Dispose(true);
			}
			GC.SuppressFinalize(this);
		}

		/// <summary>關閉各端點連線，並釋放資源</summary>
		/// <param name="isDisposing">是否為第一次釋放</param>
		protected virtual void Dispose(bool isDisposing) {
			try {
				if (isDisposing) {
					if (Role == SocketRole.Client) {
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

		#region Function - Core

		#region Client
		/// <summary>
		/// [Client] 帶入 Server IP 與 Port 並嘗試進行連線。此方法不會觸發 <see cref="SocketEvents.ConnectionWithServer"/> 事件，請以回傳值為準</summary>
		/// <param name="ip">目標 Server 之 IP Address</param>
		/// <param name="port">目標 Server 之 Port</param>
		/// <returns>(True)成功連線  (False)連線失敗</returns>
		public override bool ClientConnect(string ip, int port) {
			/*-- Ensure mSocket was closed properly --*/
			if (mSocket != null && mSocket.Connected) {
				mSocket.Shutdown(SocketShutdown.Both);
				mSocket.Close();
				mSocket.Dispose();
			}
			mSocket = null;

			/*-- Clear the disconnecting flag --*/
			mDisconnecting = false;

			/* --------------------------------------------------------------------------------
             *  Establish the remote endpoint for the socket.
             *  The name of the remote device is "ip".
             *  The method from MSDN using DNS.GetEntry to analysis ip, like "tw.yahoo.com".
             *  But if there is lan, it will cause exception
             *  So I change the EndPoint with parse ip directly.
             * -------------------------------------------------------------------------------- */
			mTarEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

			/*-- Create a new socket, Of course can use "new Socket()" directly --*/
			Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			/*-- Starting to connect with server. It is synchronous. --*/
			client.Connect(mTarEndPoint);

			this.EndPoint = new EndPointSlim(client.LocalEndPoint);

			if (SubscribeReceiveEvent)
				Client_BeginReceive(client);

			return client.IsConnected();
		}

		/// <summary>
		/// [Client] 帶入 Server IP 與 Port 並嘗試進行非同步連線。實際連線狀態請以 <see cref="SocketEvents.ConnectionWithServer"/> 事件為準</summary>
		/// <param name="ip">目標 Server 之 IP Address</param>
		/// <param name="port">目標 Server 之 Port</param>
		public override void ClientBeginConnect(string ip, int port) {
			/*-- Ensure mSocket was closed properly --*/
			if (mSocket != null && mSocket.Connected) {
				mSocket.Shutdown(SocketShutdown.Both);
				mSocket.Close();
				mSocket.Dispose();
			}
			mSocket = null;

			/*-- Clear the disconnecting flag --*/
			mDisconnecting = false;
			mCltConnSig.Reset();

			/* --------------------------------------------------------------------------------
             *  Establish the remote endpoint for the socket.
             *  The name of the remote device is "ip".
             *  The method from MSDN using DNS.GetEntry to analysis ip, like "tw.yahoo.com".
             *  But if there is lan, it will cause exception
             *  So I change the EndPoint with parse ip directly.
             * -------------------------------------------------------------------------------- */
			mTarEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

			/*-- Create a new socket, Of course can use "new Socket()" directly --*/
			Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			/*-- Start an asynchronous method to connect with server. When connect triggered, it will callback to "Client_ConnectCallback" program --*/
			client.BeginConnect(mTarEndPoint, Client_ConnectCallback, client);
		}

		/// <summary>[Callback][Client] 非同步連線成功所觸發之回呼</summary>
		/// <param name="asyncResult">非同步作業狀態</param>
		protected virtual void Client_ConnectCallback(IAsyncResult asyncResult) {
			try {
				Socket client = asyncResult.AsyncState as Socket;
				client.EndConnect(asyncResult);

				/*-- Set into global member, more easier operation for other functions --*/
				mSocket = client;
				this.EndPoint = new EndPointSlim(client.LocalEndPoint);

				/* Raise event to notify connected with server, and passing IP/Port information */
				RaiseEvents(
					SocketEvents.ConnectionWithServer,
					new SocketConnection(true, this.EndPoint)
				);

				/*-- Start Receiving Data --*/
				if (SubscribeReceiveEvent)
					Client_BeginReceive(client);

			} catch (Exception ex) {
				ExceptionHandler(Stat.ER3_WSK_COMERR, ex);
			} finally {
				mCltConnSig.Set();
			}
		}

		/// <summary>[Callback][Client] 建立新 <see cref="StateObject"/> 並開始非同步接收資料</summary>
		/// <param name="client">已建立連線之 Socket</param>
		protected virtual void Client_BeginReceive(Socket client) {
			var sttObj = new StateObject(client);
			var ep = this.EndPoint.ToEndPoint(); 
			client.BeginReceiveFrom(sttObj.Buffer, 0, StateObject.BufferSize, sttObj.Flag, ref ep, ReceiveCallback, sttObj);
		}
		#endregion

		#region Server
		/// <summary>[Server] 帶入 Port 並開始進行監聽</summary>
		/// <param name="port">欲開放 Client 連線之 Port</param>
		public override void ServerBeginListen(int port) {
			/*-- Clear the disconnecting flag --*/
			mDisconnecting = false;

			/*-- Re-Assign Socket Mode --*/
			Role = SocketRole.Server;
			this.EndPoint = new EndPointSlim("127.0.0.1", port);

			/* ------------------------------------------------------------
             * Establish the local endpoint for the socket.
             * From MSDN, it using DNS.GetEntry to dismantle IP, like "google.com"
             * But when we stand in a lan, it will cause excpetion
             * I change it back to parse IPAddress directly, as CtSyncSocket.
             * ------------------------------------------------------------ */
			mTarEndPoint = new IPEndPoint(IPAddress.Any, port);

			/*-- Create an listen soccket as a Server --*/
			Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			listener.Bind(mTarEndPoint);

			/*-- Set into global member, more easier operation for other function --*/
			mSocket = listener;

			/*-- Raise event to notify start listening --*/
			RaiseEvents(
				SocketEvents.Listen,
				new SocketConnection(true, this.EndPoint)
			);

			/*-- Start to accept connection --*/
			Server_BeginReceive(listener);
		}

		/// <summary>[Callback][Server] 接收 Client 連線之回呼。當有 Client 連線時將會觸發此方法</summary>
		/// <param name="asyncResult">非同步作業狀態</param>
		protected virtual void Server_AcceptCallback(IAsyncResult asyncResult) {
			/*-- 取得 Server Socket --*/
			Socket listener = asyncResult.AsyncState as Socket;

			try {
				/*-- 取得連接上的 Client 之 Socket --*/
				Socket handler = listener.EndAccept(asyncResult);

				/*-- 開始接收資料 --*/
				if (SubscribeReceiveEvent)
					Server_BeginReceive(handler);

				/*-- 將這個連線上的 Client 加入清單，並且觸發事件 --*/
				mClientList.Add(handler);
				RaiseEvents(
					SocketEvents.ClientConnection,
					new SocketConnection(
						true,
						(handler.RemoteEndPoint as IPEndPoint).Address.ToString(),
						(handler.RemoteEndPoint as IPEndPoint).Port
					)
				);
			} catch (ObjectDisposedException) {
				/*-- Socket had already closed. Bypass this exception --*/
			} catch (Exception ex) {
				ExceptionHandler(Stat.ER3_WSK_COMERR, ex);
			} finally {
				/*-- Start to accept connection --*/
				// Start an asynchronous socket to listen for connections.
				if (!mDisconnecting && listener != null) listener.BeginAccept(Server_AcceptCallback, listener);
			}
		}

		/// <summary>[Callback][Server] 建立新 <see cref="StateObject"/> 並開始非同步接收資料</summary>
		/// <param name="server">已建立連線之 Socket</param>
		protected virtual void Server_BeginReceive(Socket server) {
			/*-- 新增一個狀態物件專門給這個連線的 Client --*/
			StateObject sttObj = new StateObject(server);

			/*-- 開始非同步接收資料 --*/
			server.BeginReceiveFrom(sttObj.Buffer, 0, StateObject.BufferSize, sttObj.Flag, ref mTarEndPoint, ReceiveCallback, sttObj);
		}

		#endregion

		#region Receive
		/// <summary>
		/// 從 <see cref="Socket"/> 串流中收取資料，並依照 <see cref="CtSocket.DataFormat"/> 回傳 <see cref="string"/> 或 <see cref="List{T}"/> (T 為 <see cref="byte"/>)
		/// </summary>
		/// <param name="socket">欲收取資料的 <see cref="Socket"/></param>
		/// <param name="waitTmoMM">等待接收的逾時時間，單位為毫秒(Milliseconds)</param>
		/// <returns>接收到的資料</returns>
		protected virtual SocketRxData Receive(Socket socket, int waitTmoMM) {
			object returnObj = null;
			/*-- Using NetworkStream to receive data --*/
			using (NetworkStream netStream = new NetworkStream(socket)) {
				/* Using block method to receive whole stream */
				byte[] buffer = new byte[StateObject.BufferSize];
				if (!netStream.DataAvailable) throw new EndOfStreamException("Without any data in the stream");
				Task<int> readTask = netStream.ReadAsync(buffer, 0, buffer.Length);
				bool tmo = !readTask.Wait(waitTmoMM);   //This wouldn't trigger TimeoutException, we can append custom message of TimeoutException next line
				if (tmo) throw new TimeoutException("Receiving data from stream has timeout");
				/* Decode data */
				if (DataFormat == TransDataFormats.EnumerableOfByte)
					returnObj = buffer.Take(readTask.Result).ToList();
				else returnObj = Encoding.GetEncoding((int) CodePage).GetString(buffer, 0, readTask.Result);
			}
			return new SocketRxData(
				returnObj,
				new EndPointSlim(
					(Role == SocketRole.Client) ? socket.LocalEndPoint : socket.RemoteEndPoint
				)
			);
		}

		/// <summary>
		/// 從 <see cref="Socket"/> 串流中收取資料並直接施作反序列化
		/// </summary>
		/// <param name="socket">欲收取資料的 <see cref="Socket"/></param>
		/// <param name="method">欲實作的序列化方法</param>
		/// <param name="waitTmoMM">等待接收的逾時時間，單位為毫秒(Milliseconds)</param>
		/// <param name="type">如是使用 XML 序列化，請帶入目標類型</param>
		/// <returns>接收到的資料</returns>
		protected virtual SocketRxData Receive(Socket socket, Serialization method, int waitTmoMM, Type type) {
			object returnObj = null;

			/*-- Using NetworkStream to receive data --*/
			using (NetworkStream netStream = new NetworkStream(socket)) {
				if (!netStream.DataAvailable) throw new EndOfStreamException("Without any data in the stream");

				/* Wait data are ready */
				bool allowData = !CtTimer.WaitTimeout(
					waitTmoMM,
					token => {
						while (!token.IsDone) {
							if (netStream.CanWrite && netStream.DataAvailable) token.WorkDone();
							else Task.Delay(10).Wait();
						}
					}
				);

				/* Do deserialize if data ready */
				if (allowData) {
					switch (method) {
						case Serialization.Binary:
							BinaryFormatter binFmt = new BinaryFormatter();
							returnObj = binFmt.Deserialize(netStream);
							break;
						case Serialization.SOAP:
							SoapFormatter soapFmt = new SoapFormatter();
							returnObj = soapFmt.Deserialize(netStream);
							break;
						case Serialization.XML:
							XmlSerializer xmlSer = new XmlSerializer(type);
							returnObj = xmlSer.Deserialize(netStream);
							break;
						case Serialization.DataContract:
							DataContractSerializer dcSer = new DataContractSerializer(type);
							returnObj = dcSer.ReadObject(netStream);
							break;
						default:
							break;
					}
				}
			}
			return new SocketRxData(
				returnObj,
				new EndPointSlim(
					(Role == SocketRole.Client) ? socket.LocalEndPoint : socket.RemoteEndPoint
				)
			);
		}

		/// <summary>
		/// 從網路串流中收取資料，並依照 <see cref="CtSocket.DataFormat"/> 回傳 <see cref="string"/> 或 <see cref="List{T}"/> (T 為 <see cref="byte"/>)
		/// <para>[Client] 從串流中收取資料</para>
		/// <para>[Server] 從最後的連線端點收取資料</para>
		/// </summary>
		/// <param name="waitTmoMM">等待接收的逾時時間，單位為毫秒(Milliseconds)</param>
		/// <returns>接收到的資料</returns>
		public override SocketRxData Receive(int waitTmoMM = 1000) {
			/*-- Select socket --*/
			Socket socket = (Role == SocketRole.Client) ? mSocket : mClientList?.Last();
			/*-- Trying to receive data --*/
			return Receive(socket, waitTmoMM);
		}

		/// <summary>
		/// 從網路串流中收取資料，並依照 <see cref="CtSocket.DataFormat"/> 回傳 <see cref="string"/> 或 <see cref="List{T}"/> (T 為 <see cref="byte"/>)
		/// <para>[Client] 請勿使用此方法！</para>
		/// <para>[Server] 蒐集所有連線的資料</para>
		/// </summary>
		/// <param name="rxData">欲接收回傳結果的集合</param>
		/// <param name="waitTmoMM">等待接收的逾時時間，單位為毫秒(Milliseconds)</param>
		/// <returns>接收到的資料</returns>
		public virtual void Receive(out List<SocketRxData> rxData, int waitTmoMM = 1000) {
			List<SocketRxData> tempData = new List<SocketRxData>();
			foreach (var socket in mClientList) {
				try {
					SocketRxData ret = Receive(socket, waitTmoMM);
					if (ret != null) tempData.Add(ret);
				} catch (Exception ex) {
					Console.WriteLine(ex.Message);
				}
			}
			rxData = tempData;
		}

		/// <summary>
		/// 從網路串流中收取資料，並依照 <see cref="CtSocket.DataFormat"/> 回傳 <see cref="string"/> 或 <see cref="List{T}"/> (T 為 <see cref="byte"/>)
		/// <para>[Server Only] 從指定的連線端點收取資料</para>
		/// </summary>
		/// <param name="ip">欲搜尋的遠端端點網際網路位置，以 <see cref="Socket.RemoteEndPoint"/> 為準</param>
		/// <param name="port">欲搜尋的遠端端點通訊埠位置，以 <see cref="Socket.RemoteEndPoint"/> 為準</param>
		/// <param name="waitTmoMM">等待接收的逾時時間，單位為毫秒(Milliseconds)</param>
		/// <returns>接收到的資料</returns>
		public override SocketRxData Receive(string ip, int port, int waitTmoMM = 1000) {
			/*-- Select socket --*/
			Socket socket = mClientList.Find(
								com => {
									var point = com.RemoteEndPoint as IPEndPoint;
									return point.Address.ToString() == ip && point.Port == port;
								}
							);

			if (socket == null) throw new ArgumentNullException("Can not find specified endpoint.");

			/*-- Trying to receive data --*/
			return Receive(socket, waitTmoMM);
		}

		/// <summary>
		/// 從網路串流中收取資料並直接施作反序列化
		/// <para>[Client] 從串流中收取資料</para>
		/// <para>[Server] 從最後的連線端點收取資料</para>
		/// </summary>
		/// <param name="method">欲實作的序列化方法</param>
		/// <param name="waitTmoMM">等待接收的逾時時間，單位為毫秒(Milliseconds)</param>
		/// <param name="type">如是使用 XML 序列化，請帶入目標類型</param>
		/// <returns>接收到的資料</returns>
		public override SocketRxData Receive(Serialization method, int waitTmoMM = 1000, Type type = null) {
			/*-- Select socket --*/
			Socket socket = (Role == SocketRole.Client) ? mSocket : mClientList?.Last();
			/*-- Trying to receive data --*/
			return Receive(socket, method, waitTmoMM, type);
		}

		/// <summary>
		/// 從網路串流中收取資料並直接施作反序列化
		/// <para>[Server Only] 從指定的連線端點收取資料</para>
		/// </summary>
		/// <param name="ip">欲搜尋的遠端端點網際網路位置，以 <see cref="Socket.RemoteEndPoint"/> 為準</param>
		/// <param name="port">欲搜尋的遠端端點通訊埠位置，以 <see cref="Socket.RemoteEndPoint"/> 為準</param>
		/// <param name="method">欲實作的序列化方法</param>
		/// <param name="waitTmoMM">等待接收的逾時時間，單位為毫秒(Milliseconds)</param>
		/// <param name="type">如是使用 XML 序列化，請帶入目標類型</param>
		/// <returns>接收到的資料</returns>
		public virtual SocketRxData Receive(string ip, int port, Serialization method, int waitTmoMM = 1000, Type type = null) {
			/*-- Select socket --*/
			Socket socket = mClientList.Find(
								com => {
									var point = com.RemoteEndPoint as IPEndPoint;
									return point.Address.ToString() == ip && point.Port == port;
								}
							);

			if (socket == null) throw new ArgumentNullException("Can not find specified endpoint.");

			/*-- Trying to receive data --*/
			return Receive(socket, method, waitTmoMM, type);
		}

		/// <summary>[Callback] 從 NetworkStream 執行非同步讀取資料</summary>
		/// <param name="asyncResult">非同步作業狀態</param>
		protected virtual void ReceiveCallback(IAsyncResult asyncResult) {
			StateObject sttObj = asyncResult.AsyncState as StateObject;
			Socket socket = sttObj.WorkSocket;

			bool isEnd = false;
			int bytesToRead = 0;    //Record how much bytes that read and set into sttObj.Buffer
			var ep = this.EndPoint.ToEndPoint();

			/*-- Read data from the socket. And it will retuen how much bytes read --*/
			try {
				bytesToRead = socket.EndReceiveFrom(asyncResult, ref ep);
			} catch (ObjectDisposedException) {
				if (!mDisconnecting) {
					/*-- Raise Event --*/
					RaiseEvents(
						Role == SocketRole.Client ? SocketEvents.ClientConnection : SocketEvents.ConnectionWithServer,
						new SocketConnection(false, this.EndPoint)
					);
					mSocket.Shutdown(SocketShutdown.Both);
					mSocket.Close();
					mSocket = null;
					mDisconnecting = true;
				}
				return;
			} catch (Exception ex) {
				ExceptionHandler(Stat.ER3_WSK_COMERR, ex);
			}

			/*-- Get socket IP and Port --*/
			string ip = (ep as IPEndPoint).Address.ToString();
			int port = (ep as IPEndPoint).Port;

			/*-- If there are something passing-in --*/
			if (bytesToRead > 0 && sttObj.Error == SocketError.Success) {

				/* --------------------------------------------------------------------------------- 
                 * Because .Buffer always contains 1024 length, it filled with '\0' after data bytes
                 * So take real data that received into RemainBuffer and do operation with it.
                 * Also EndOfLine enabled, if not finished data, it as a temporary data BKFore finished
                 * --------------------------------------------------------------------------------- */
				sttObj.RemainBuffer.AddRange(sttObj.Buffer.Take(bytesToRead));
				isEnd = IsContainEndOfLine(sttObj.RemainBuffer);    //check is it finished? (<see langword="true"/>)Finished (<see langword="false"/>)Not yet

				/*-- If finished, raise data_received event --*/
				if (isEnd) {

					if (DataFormat == TransDataFormats.EnumerableOfByte) {

						/*-- If there are need List<byte>, return it directly --*/
						RaiseEvents(
							SocketEvents.DataReceived,
							new SocketRxData(
								sttObj.RemainBuffer.ToList(),
								ip,
								port
							)
						);

					} else {

						/*-- Convert byte[] back to string --*/
						string strData = Encoding.GetEncoding((int) CodePage).GetString(sttObj.RemainBuffer.ToArray()).Trim();

						/*-- If there are convert successfully, raise the event to push data out --*/
						RaiseEvents(
							SocketEvents.DataReceived,
							new SocketRxData(
								strData,
								ip,
								port
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
				if (SubscribeReceiveEvent) {
					if (!isEnd)
						socket.BeginReceiveFrom(sttObj.Buffer, 0, StateObject.BufferSize, sttObj.Flag, ref ep, ReceiveCallback, sttObj);
					else if (Role == SocketRole.Server)
						Server_BeginReceive(socket);
					else if (Role == SocketRole.Client)
						Client_BeginReceive(socket);
				}

			} else if (Role == SocketRole.Client && !mDisconnecting) { //If this exception is cause by "ClientDisconnect", skip this.
																	   /*-- If bytesToRead is zero, it means stream closed by remote endpoint. Close local socket.  --*/
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
				mSocket = null;
				mDisconnecting = true;

				/*-- Raise event to notify there is stream closed --*/
				RaiseEvents(
					SocketEvents.ConnectionWithServer,
					new SocketConnection(
						false,
						ip,
						port
					)
				);

			} else if (Role == SocketRole.Server && !mDisconnecting) { //If this exception is cause by "ServerStopListen", skip this
																	   /*-- If socket shutdown and already read all data from stream buffer, it will return bytesToRead = 0 --*/
																	   /*-- Remove the Socket from list --*/
				mClientList.Remove(socket);
				socket.Disconnect(true);

				/*-- Raise Event --*/
				RaiseEvents(
					SocketEvents.ClientConnection,
					new SocketConnection(
						false,
						ip,
						port
					)
				);
			}
		}
		#endregion

		#region Send

		/// <summary>
		/// 使用同步方式傳送資料至端點
		/// <para>請自行檢查 <see cref="IsConnected"/> 狀態，或以 try-catch 語法確保連線已開啟並可寫入串流</para>
		/// </summary>
		/// <param name="data">欲傳送的資料字串</param>
		/// <param name="eachOne">此參數於 UDP 無效！</param>
		/// <remarks>目前沒有加入 EndOfLine 於引數，因為大部分走 Socket 比較少含有結尾符號，速度較快不像 RS-232 易有斷開情況。如有特殊需求(如 CommunicationProtocol)請自行加於 data 裡</remarks>
		public override void Send(string data, bool eachOne = true) {
			Send(data, CodePage, eachOne);
		}

		/// <summary>
		/// 使用同步方式傳送資料至端點
		/// <para>請自行檢查 <see cref="IsConnected"/> 狀態，或以 try-catch 語法確保連線已開啟並可寫入串流</para>
		/// </summary>
		/// <param name="data">欲傳送的資料字串</param>
		/// <param name="codePage">編碼。用於 byte、string 互相轉換</param>
		/// <param name="eachOne">此參數於 UDP 無效！</param>
		/// <remarks>目前沒有加入 EndOfLine 於引數，因為大部分走 Socket 比較少含有結尾符號，速度較快不像 RS-232 易有斷開情況。如有特殊需求(如 CommunicationProtocol)請自行加於 data 裡</remarks>
		public override void Send(string data, CodePages codePage, bool eachOne = true) {
			/*-- If empty string, return without excpetion --*/
			if (string.IsNullOrEmpty(data)) return;

			/*-- Reset flag --*/
			mSendSig.Reset();

			/*-- Convert data from string to byte array --*/
			byte[] txBytes = Encoding.GetEncoding((int) codePage).GetBytes(data);

			/*-- Send data --*/
			var ep = (Role == SocketRole.Client) ? mSocket.RemoteEndPoint : TargetEndPoint.ToEndPoint();
			mSocket.SendTo(txBytes, ep);
			if (SubscribeSendEvent) {
				SocketTxData txData = new SocketTxData(mSocket, data, txBytes.Length);
				RaiseEvents(SocketEvents.DataSend, txData);
			}
		}

		/// <summary>
		/// 使用同步方式傳送資料至端點
		/// <para>請自行檢查 <see cref="IsConnected"/> 狀態，或以 try-catch 語法確保連線已開啟並可寫入串流</para>
		/// </summary>
		/// <param name="data">欲傳送的資料集合</param>
		/// <param name="eachOne">此參數於 UDP 無效！</param>
		public override void Send(IEnumerable<byte> data, bool eachOne = true) {
			/*-- Reset flag --*/
			mSendSig.Reset();
			
			byte[] byteData = data.ToArray();

			/*-- Send data --*/
			var ep = (Role == SocketRole.Client) ? mSocket.RemoteEndPoint : TargetEndPoint.ToEndPoint();
			mSocket.SendTo(byteData, ep);
			if (SubscribeSendEvent) {
				SocketTxData txData = new SocketTxData(mSocket, byteData, byteData.Length);
				RaiseEvents(SocketEvents.DataSend, txData);
			}
		}

		/// <summary>
		/// 使用同步方式將物件以序列化方式傳至端點
		/// <para>請自行檢查 <see cref="IsConnected"/> 狀態，或以 try-catch 語法確保連線已開啟並可寫入串流</para>
		/// </summary>
		/// <param name="obj">欲序列化的物件</param>
		/// <param name="method">使用的序列化方法</param>
		/// <param name="eachOne">此參數於 UDP 無效！</param>
		public override void Send<TSer>(TSer obj, Serialization method, bool eachOne = true) {
			/*-- Reset flag --*/
			mSendSig.Reset();

			/*-- Serialize object --*/
			byte[] byteData = null; //Store byte data that serialized
									/* Serialize into MemoryStream */
			using (MemoryStream memStream = new MemoryStream()) {
				switch (method) {
					case Serialization.Binary:
						BinaryFormatter binFmt = new BinaryFormatter();
						binFmt.Serialize(memStream, obj);
						break;
					case Serialization.SOAP:
						SoapFormatter soapFmt = new SoapFormatter();
						soapFmt.Serialize(memStream, obj);
						break;
					case Serialization.XML:
						XmlSerializer xmlSer = new XmlSerializer(typeof(TSer));
						xmlSer.Serialize(memStream, obj);
						break;
					case Serialization.DataContract:
						DataContractSerializer dcSer = new DataContractSerializer(typeof(TSer));
						dcSer.WriteObject(memStream, obj);
						break;
				}
				/* Set stream indexer back to zero */
				memStream.Position = 0;
				/* Get byte data of whole stream */
				byteData = memStream.ToArray();
			}

			/*-- Send Data --*/
			var ep = (Role == SocketRole.Client) ? mSocket.RemoteEndPoint : TargetEndPoint.ToEndPoint();
			mSocket.SendTo(byteData, ep);
			if (SubscribeSendEvent) {
				SocketTxData txData = new SocketTxData(mSocket, obj, byteData.Length);
				RaiseEvents(SocketEvents.DataSend, txData);
			}
		}

		/// <summary>
		/// 使用非同步方式傳送資料至端點，傳送完成時間點請以 <see cref="SocketEvents.DataSend"/> 事件為準
		/// <para>請自行檢查 <see cref="IsConnected"/> 狀態，或以 try-catch 語法確保連線已開啟並可寫入串流</para>
		/// </summary>
		/// <param name="data">欲傳送的資料字串</param>
		/// <param name="eachOne">此參數於 UDP 無效！</param>
		/// <remarks>目前沒有加入 EndOfLine 於引數，因為大部分走 Socket 比較少含有結尾符號，速度較快不像 RS-232 易有斷開情況。如有特殊需求(如 CommunicationProtocol)請自行加於 data 裡</remarks>
		public override void BeginSend(string data, bool eachOne = true) {
			BeginSend(data, CodePage, eachOne);
		}

		/// <summary>
		/// 使用非同步方式傳送資料至端點，傳送完成時間點請以 <see cref="SocketEvents.DataSend"/> 事件為準
		/// <para>請自行檢查 <see cref="IsConnected"/> 狀態，或以 try-catch 語法確保連線已開啟並可寫入串流</para>
		/// </summary>
		/// <param name="data">欲傳送的資料字串</param>
		/// <param name="codePage">編碼。用於 byte、string 互相轉換</param>
		/// <param name="eachOne">此參數於 UDP 無效！</param>
		/// <remarks>目前沒有加入 EndOfLine 於引數，因為大部分走 Socket 比較少含有結尾符號，速度較快不像 RS-232 易有斷開情況。如有特殊需求(如 CommunicationProtocol)請自行加於 data 裡</remarks>
		public override void BeginSend(string data, CodePages codePage, bool eachOne = true) {
			/*-- If empty string, return without exception --*/
			if (string.IsNullOrEmpty(data)) return;

			/*-- Reset flag --*/
			mSendSig.Reset();

			/*-- Convert data from string to byte array --*/
			byte[] txBytes = Encoding.GetEncoding((int) codePage).GetBytes(data);
			
			/*-- Send data --*/
			var ep = (Role == SocketRole.Client) ? TargetEndPoint.ToEndPoint() : TargetEndPoint.ToEndPoint();
			SocketTxData txData = new SocketTxData(mSocket, data);
			mSocket.BeginSendTo(txBytes, 0, txBytes.Length, 0, ep, SendCallback, txData);
		}

		/// <summary>
		/// 使用非同步方式傳送資料至端點，傳送完成時間點請以 <see cref="SocketEvents.DataSend"/> 事件為準
		/// <para>請自行檢查 <see cref="IsConnected"/> 狀態，或以 try-catch 語法確保連線已開啟並可寫入串流</para>
		/// </summary>
		/// <param name="data">欲傳送的資料集合</param>
		/// <param name="eachOne">此參數於 UDP 無效！</param>
		public override void BeginSend(IEnumerable<byte> data, bool eachOne = true) {
			/*-- Reset flag --*/
			mSendSig.Reset();

			byte[] byteData = data.ToArray();

			/*-- Send data --*/
			var ep = (Role == SocketRole.Client) ? mSocket.RemoteEndPoint : TargetEndPoint.ToEndPoint();
			SocketTxData txData = new SocketTxData(mSocket, byteData);
			mSocket.BeginSendTo(byteData, 0, byteData.Length, 0, ep, SendCallback, txData);
		}

		/// <summary>
		/// 使用非同步方式將物件以序列化方式傳至端點，傳送完成時間點請以 <see cref="SocketEvents.DataSend"/> 事件為準
		/// <para>請自行檢查 <see cref="IsConnected"/> 狀態，或以 try-catch 語法確保連線已開啟並可寫入串流</para>
		/// </summary>
		/// <param name="obj">欲序列化的物件</param>
		/// <param name="method">使用的序列化方法</param>
		/// <param name="eachOne">此參數於 UDP 無效！</param>
		public override void BeginSend<TSer>(TSer obj, Serialization method, bool eachOne = true) {
			/*-- Reset flag --*/
			mSendSig.Reset();

			/*-- Serialize object --*/
			byte[] byteData = null; //Store byte data that serialized
									/* Serialize into MemoryStream */
			using (MemoryStream memStream = new MemoryStream()) {
				switch (method) {
					case Serialization.Binary:
						BinaryFormatter binFmt = new BinaryFormatter();
						binFmt.Serialize(memStream, obj);
						break;
					case Serialization.SOAP:
						SoapFormatter soapFmt = new SoapFormatter();
						soapFmt.Serialize(memStream, obj);
						break;
					case Serialization.XML:
						XmlSerializer xmlSer = new XmlSerializer(typeof(TSer));
						xmlSer.Serialize(memStream, obj);
						break;
					case Serialization.DataContract:
						DataContractSerializer dcSer = new DataContractSerializer(typeof(TSer));
						dcSer.WriteObject(memStream, obj);
						break;
				}
				/* Set stream indexer back to zero */
				memStream.Position = 0;
				/* Get byte data of whole stream */
				byteData = memStream.ToArray();
			}

			/*-- Send data --*/
			var ep = (Role == SocketRole.Client) ? mSocket.RemoteEndPoint : TargetEndPoint.ToEndPoint();
			SocketTxData txData = new SocketTxData(mSocket, obj);
			mSocket.BeginSendTo(byteData, 0, byteData.Length, 0, ep, SendCallback, txData);
		}

		/// <summary>[Callback] 傳送資料之回呼。傳送資料完畢後將回呼此方法</summary>
		/// <param name="asyncResult">非同步作業狀態</param>
		protected virtual void SendCallback(IAsyncResult asyncResult) {
			SocketTxData txData = asyncResult.AsyncState as SocketTxData;
			try {
				int bytesToSend = txData.WorkSocket.EndSendTo(asyncResult);
				if (SubscribeSendEvent) {
					var ep = (Role == SocketRole.Client) ? mSocket.RemoteEndPoint : TargetEndPoint.ToEndPoint();
					txData.SetTxLength(bytesToSend, ep);
					RaiseEvents(SocketEvents.DataSend, txData);
				}

				/*-- Trigger flag --*/
				mSendSig.Set();
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER3_WSK_COMERR, ex);
			}
		}
		#endregion

		#endregion
	}
}
