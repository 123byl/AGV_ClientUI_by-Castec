using CtLib.Library;
using CtLib.Module.Utility;
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

namespace CtLib.Module.Net {

	/// <summary>
	/// 封裝 Berkeley Socket 之基底類別
	/// <para>提供作業系統行程間的通訊機制，以進行資料交換。可搭配 TCP 或 UDP 通訊協定</para>
	/// </summary>
	public abstract class CtSocket : IDisposable {

		#region Declaration - Fields
		/// <summary>
		/// 暫存 Socket 連線物件
		/// <para>[Client] 與 Server 連線之 Socket</para>
		/// <para>[Server] 接收 Client 連線之 Listen Socket</para>
		/// </summary>
		protected Socket mSocket;
		/// <summary>[Server] 當前已連線的 Client 集合</summary>
		protected List<Socket> mClientList = new List<Socket>();

		/// <summary>相對應結尾符號之byte</summary>
		/// <remarks>搭配 EndOfLineSymbol == EndChar.Custom</remarks>
		protected byte[] mEOLByte;

		/// <summary>[Flag] 中斷連線狀態</summary>
		/// <remarks>由於觸發 Disconnect 時也會觸發一次 ReceiveCallback，屆時將會觸發兩次斷線事件，故用此 Flag 卡住不要讓他發兩次</remarks>
		protected bool mDisconnecting = false;

		/// <summary>[Flag] 指出是否已釋放過資源</summary>
		protected bool mDisposed = false;

		/// <summary>非同步 Client 等待連線至 Server 之訊號</summary>
		protected ManualResetEventSlim mCltConnSig = new ManualResetEventSlim();

		/// <summary>非同步傳送資料時的完成訊號</summary>
		protected ManualResetEventSlim mSendSig = new ManualResetEventSlim();
		#endregion

		#region Declaration - Properties
		/// <summary>取得當前 Socket 的主從式角色，用戶端(Client)或伺服端(Server)</summary>
		public SocketRole Role { get; protected set; }

		/// <summary>取得當前所採用的通訊協定</summary>
		public abstract NetworkProtocol Protocol { get; }

		/// <summary>取得或設定編碼。用於傳送、接收時 byte 和 string 之間之轉換</summary>
		public CodePages CodePage { get; set; }

		/// <summary>取得事件回傳的資料格式</summary>
		public TransDataFormats DataFormat { get; }

		/// <summary>
		/// 取得本機的連線端點(IPAddress + Port)
		/// <para>[Client] 連上 Server 後所分配的端點資訊</para>
		/// <para>[Server] 本機監聽的端點資訊</para>
		/// </summary>
		public EndPointSlim EndPoint { get; protected set; }

		/// <summary>取得或設定讀寫資料時之結尾符號</summary>
		public EndChar EndOfLineSymbol { get; set; }

		/// <summary>取得或設定自訂義之結尾符號</summary>
		public string CustomEndOfLine {
			get { return Encoding.GetEncoding((int) CodePage).GetString(mEOLByte); }
			set { mEOLByte = Encoding.GetEncoding((int) CodePage).GetBytes(value); }
		}

		/// <summary>取得或設定是否訂閱發布傳送資料之事件。如訂閱，將於傳送資料後發布相關事件</summary>
		public bool SubscribeSendEvent { get; set; } = false;

		/// <summary>
		/// 取得或設定是否訂閱發布接收資料之事件
		/// <para>如訂閱，將於接收到資料後發布事件；如未訂閱則請自行觸發 Receive 方法，並自行判斷是否斷線</para>
		/// </summary>
		/// <remarks>因斷線時會觸發 Receive_Callback 並收到 Exception 或長度為 0 的資料，故如未訂閱事件，則無法自動得知斷線</remarks>
		public bool SubscribeReceiveEvent { get; } = true;
		#endregion

		#region Declaration - Events

		/// <summary>Socket 相關事件，採用統一事件發布</summary>
		public event EventHandler<SocketEventArgs> OnSocketEvents;

		/// <summary>觸發事件</summary>
		/// <param name="events">事件</param>
		/// <param name="args">此事件所附帶之數值</param>
		protected virtual void RaiseEvents(SocketEvents events, object args) {
			OnSocketEvents?.Invoke(this, new SocketEventArgs(events, args));
		}
		#endregion

		#region Function - Construtor
		/// <summary>建構子，強制要帶入資料格式並決定是否訂閱接收資料事件</summary>
		/// <param name="fmt">資料格式</param>
		/// <param name="rxEvent">是否訂閱接收事件</param>
		internal CtSocket(TransDataFormats fmt, bool rxEvent) {
			DataFormat = fmt;
			SubscribeReceiveEvent = rxEvent;
		}
		#endregion

		#region Function - Dispose
		/// <summary>關閉連線並釋放資源</summary>
		public abstract void Dispose();
		#endregion

		#region Function - Methods
		/// <summary>統一發報 Exception</summary>
		/// <param name="stt">Status Code</param>
		/// <param name="title">標題</param>
		/// <param name="msg">訊息</param>
		protected void ExceptionHandler(Stat stt, string title, string msg) {
			CtStatus.Report(stt, title, msg);
			RaiseEvents(SocketEvents.Exception, string.Format("[{0}] {1}", title, msg));
		}

		/// <summary>統一發報 Exception</summary>
		/// <param name="stt">Status Code</param>
		/// <param name="ex">由系統收集之例外訊息。請參考 <see cref="Exception"/></param>
		protected void ExceptionHandler(Stat stt, Exception ex) {
			string title = string.Empty;
			CtStatus.Report(stt, ex, out title);
			RaiseEvents(SocketEvents.Exception, string.Format("[{0}] {1}", title, ex.Message));
		}

		/// <summary>檢查 Byte 集合中是否含有目前設定的結尾符號</summary>
		/// <param name="data">欲檢查的資料</param>
		/// <returns>(<see langword="true"/>)含有結尾符號  (<see langword="false"/>)不含結尾符號</returns>
		protected bool IsContainEndOfLine(List<byte> data) {
			bool result = false;

			switch (EndOfLineSymbol) {
				case EndChar.None:
					result = true;
					break;
				case EndChar.Cr:
					result = data.Contains(0x0D);
					break;
				case EndChar.Lf:
					result = data.Contains(0x0A);
					break;
				case EndChar.CrLf:
					result = data.Contains(0x0D) & data.Contains(0x0A);
					break;
				case EndChar.Custom:
					result = mEOLByte.All(eol => data.Contains(eol));
					break;
				default:
					break;
			}

			return result;
		}
		#endregion

		#region Function - Clients
		/// <summary>
		/// [Client] 帶入目標伺服端 IP 與 Port 並嘗試進行連線。此方法不會觸發 <see cref="SocketEvents.ConnectionWithServer"/> 事件，請以回傳值為準</summary>
		/// <param name="ip">目標 Server 之 IP Address</param>
		/// <param name="port">目標 Server 之 Port</param>
		/// <returns>(True)成功連線  (False)連線失敗</returns>
		public abstract bool ClientConnect(string ip, int port);
		/// <summary>
		/// [Client] 帶入目標伺服端 IP 與 Port 並嘗試進行非同步連線。實際連線狀態請以 <see cref="SocketEvents.ConnectionWithServer"/> 事件為準或自行等待</summary>
		/// <param name="ip">目標 Server 之 IP Address</param>
		/// <param name="port">目標 Server 之 Port</param>
		public abstract void ClientBeginConnect(string ip, int port);
		/// <summary>[Client] 中斷與 Server 之連線</summary>
		public void ClientDisconnect() {
			/*-- Set the disconnecting flag --*/
			mDisconnecting = true;

			/*-- Shutdown send and receive stream, and close stream --*/
			if (mSocket != null) {
				mSocket.Shutdown(SocketShutdown.Both);
				mSocket.Close();
				mSocket = null;
			}

			/* Raise event to notify connection is disconnected */
			RaiseEvents(
				SocketEvents.ConnectionWithServer,
				new SocketConnection(false, EndPoint)
			);
		}
		#endregion

		#region Function - Server
		/// <summary>[Server] 帶入 Port 並開始進行監聽</summary>
		/// <param name="port">欲開放 Client 連線之 Port</param>
		public abstract void ServerBeginListen(int port);
		/// <summary>[Server] 中斷所有 Client 連線，並停止監聽</summary>
		public void ServerStopListen() {
			/*-- Announce disconnecting --*/
			mDisconnecting = true;

			/*-- Close each socket which connected --*/
			foreach (Socket item in mClientList) {
				try {
					item.Shutdown(SocketShutdown.Both);
				} catch (Exception) {
					/* If exception occurred when closing, never mind~ */
				}
				item.Close();
			}

			/*-- Clear the collection of connected sockets --*/
			mClientList.Clear();

			/*-- Close listener --*/
			try {
				mSocket?.Shutdown(SocketShutdown.Both);
			} catch (Exception) {
				/* If exception occurred when closing, never mind~ */
			}
			mSocket?.Close();

			/*-- Raise the event --*/
			RaiseEvents(
				SocketEvents.Listen,
				new SocketConnection(false, this.EndPoint)
			);

			mSocket = null;
		}
		#endregion

		#region Function - Receive
		/// <summary>
		/// 從網路串流中收取資料，並依照 <see cref="DataFormat"/> 回傳 <see cref="string"/> 或 <see cref="List{T}"/> (T 為 <see cref="byte"/>)
		/// <para>[Client] 從串流中收取資料</para>
		/// <para>[Server] 從最後的連線端點收取資料</para>
		/// </summary>
		/// <param name="waitTmo">等待接收的逾時時間，單位為毫秒(Milliseconds)</param>
		/// <returns>接收到的資料</returns>
		public abstract SocketRxData Receive(int waitTmo = 1000);
		/// <summary>
		/// 從網路串流中收取資料，並依照 <see cref="DataFormat"/> 回傳 <see cref="string"/> 或 <see cref="List{T}"/> (T 為 <see cref="byte"/>)
		/// <para>[Server Only] 從指定的連線端點收取資料</para>
		/// </summary>
		/// <param name="ip">欲搜尋的遠端端點網際網路位置，以 <see cref="Socket.RemoteEndPoint"/> 為準</param>
		/// <param name="port">欲搜尋的遠端端點通訊埠位置，以 <see cref="Socket.RemoteEndPoint"/> 為準</param>
		/// <param name="waitTmo">等待接收的逾時時間，單位為毫秒(Milliseconds)</param>
		/// <returns>接收到的資料</returns>
		public abstract SocketRxData Receive(string ip, int port, int waitTmo = 1000);
		/// <summary>
		/// 從網路串流中收取資料並直接施作反序列化
		/// <para>[Client] 從串流中收取資料</para>
		/// <para>[Server] 從最後的連線端點收取資料</para>
		/// </summary>
		/// <param name="ser">欲實作的序列化方法</param>
		/// <param name="waitTmo">等待接收的逾時時間，單位為毫秒(Milliseconds)</param>
		/// <param name="type">如是使用 XML、DataContract 序列化，請帶入目標類型</param>
		/// <returns>接收到的資料</returns>
		public abstract SocketRxData Receive(Serialization ser, int waitTmo = 1000, Type type = null);
		#endregion

		#region Function - Send
		/// <summary>
		/// 使用同步方式傳送資料至端點
		/// <para>請自行檢查連線狀態，或以 try-catch 語法確保連線已開啟並可寫入串流</para>
		/// </summary>
		/// <param name="data">欲傳送的資料字串</param>
		/// <param name="eachOne">
		/// [Server Only] 是否為廣播方式? 即傳送至所有 Client 或僅傳送至 最先連線之 Client
		/// <para>[Client] Ignore!!</para>
		/// </param>
		public abstract void Send(string data, bool eachOne = true);
		/// <summary>
		/// 使用同步方式傳送資料至端點
		/// <para>請自行檢查連線狀態，或以 try-catch 語法確保連線已開啟並可寫入串流</para>
		/// </summary>
		/// <param name="data">欲傳送的資料字串</param>
		/// <param name="codPag">編碼。用於 byte、string 互相轉換</param>
		/// <param name="eachOne">
		/// [Server Only] 是否為廣播方式? 即傳送至所有 Client 或僅傳送至 最先連線之 Client
		/// <para>[Client] Ignore!!</para>
		/// </param>
		/// <remarks>目前沒有加入 EndOfLine 於引數，因為大部分走 Socket 比較少含有結尾符號，速度較快不像 RS-232 易有斷開情況。如有特殊需求(如 CommunicationProtocol)請自行加於 data 裡</remarks>
		public abstract void Send(string data, CodePages codPag, bool eachOne = true);
		/// <summary>
		/// 使用同步方式傳送資料至端點
		/// <para>請自行檢查連線狀態，或以 try-catch 語法確保連線已開啟並可寫入串流</para>
		/// </summary>
		/// <param name="data">欲傳送的資料集合</param>
		/// <param name="eachOne">
		/// [Server Only] 是否為廣播方式? 即傳送至所有 Client 或僅傳送至 最先連線之 Client
		/// <para>[Client] Ignore!!</para>
		/// </param>
		public abstract void Send(IEnumerable<byte> data, bool eachOne = true);
		/// <summary>
		/// 使用同步方式將物件以序列化方式傳至端點
		/// <para>請自行檢查連線狀態，或以 try-catch 語法確保連線已開啟並可寫入串流</para>
		/// </summary>
		/// <param name="obj">欲序列化的物件</param>
		/// <param name="ser">使用的序列化方法</param>
		/// <param name="eachOne">
		/// [Server Only] 是否為廣播方式? 即傳送至所有 Client 或僅傳送至 最先連線之 Client
		/// <para>[Client] Ignore!!</para>
		/// </param>
		public abstract void Send<TSer>(TSer obj, Serialization ser, bool eachOne = true);
		/// <summary>
		/// 使用非同步方式傳送資料至端點，傳送完成時間點請以 <see cref="SocketEvents.DataSend"/> 事件或自行等待為準
		/// <para>請自行檢查連線狀態，或以 try-catch 語法確保連線已開啟並可寫入串流</para>
		/// </summary>
		/// <param name="data">欲傳送的資料字串</param>
		/// <param name="eachOne">
		/// [Server Only] 是否為廣播方式? 即傳送至所有 Client 或僅傳送至 最先連線之 Client
		/// <para>[Client] Ignore!!</para>
		/// </param>
		/// <remarks>目前沒有加入 EndOfLine 於引數，因為大部分走 Socket 比較少含有結尾符號，速度較快不像 RS-232 易有斷開情況。如有特殊需求(如 CommunicationProtocol)請自行加於 data 裡</remarks>
		public abstract void BeginSend(string data, bool eachOne = true);
		/// <summary>
		/// 使用非同步方式傳送資料至端點，傳送完成時間點請以 <see cref="SocketEvents.DataSend"/> 事件或自行等待為準
		/// <para>請自行檢查連線狀態，或以 try-catch 語法確保連線已開啟並可寫入串流</para>
		/// </summary>
		/// <param name="data">欲傳送的資料字串</param>
		/// <param name="codPag">編碼。用於 byte、string 互相轉換</param>
		/// <param name="eachOne">
		/// [Server Only] 是否為廣播方式? 即傳送至所有 Client 或僅傳送至 最先連線之 Client
		/// <para>[Client] Ignore!!</para>
		/// </param>
		/// <remarks>目前沒有加入 EndOfLine 於引數，因為大部分走 Socket 比較少含有結尾符號，速度較快不像 RS-232 易有斷開情況。如有特殊需求(如 CommunicationProtocol)請自行加於 data 裡</remarks>
		public abstract void BeginSend(string data, CodePages codPag, bool eachOne = true);
		/// <summary>
		/// 使用非同步方式傳送資料至端點，傳送完成時間點請以 <see cref="SocketEvents.DataSend"/> 事件或自行等待為準
		/// <para>請自行檢查連線狀態，或以 try-catch 語法確保連線已開啟並可寫入串流</para>
		/// </summary>
		/// <param name="data">欲傳送的資料集合</param>
		/// <param name="eachOne">
		/// [Server Only] 是否為廣播方式? 即傳送至所有 Client 或僅傳送至 最先連線之 Client
		/// <para>[Client] Ignore!!</para>
		/// </param>
		public abstract void BeginSend(IEnumerable<byte> data, bool eachOne = true);
		/// <summary>
		/// 使用非同步方式將物件以序列化方式傳至端點，傳送完成時間點請以 <see cref="SocketEvents.DataSend"/> 事件或自行等待為準
		/// <para>請自行檢查連線狀態，或以 try-catch 語法確保連線已開啟並可寫入串流</para>
		/// </summary>
		/// <param name="obj">欲序列化的物件</param>
		/// <param name="ser">使用的序列化方法</param>
		/// <param name="eachOne">
		/// [Server Only] 是否為廣播方式? 即傳送至所有 Client 或僅傳送至 最先連線之 Client
		/// <para>[Client] Ignore!!</para>
		/// </param>
		public abstract void BeginSend<TSer>(TSer obj, Serialization ser, bool eachOne = true);
		#endregion
	}

}
