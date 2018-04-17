using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace CtLib.Module.Net {

	/// <summary>輕量化的 TCP/IP 端點資訊</summary>
	public class EndPointSlim {

		#region Fields
		private string mIP = string.Empty;
		private int mPort = 0;
		#endregion

		#region Properties
		/// <summary>取得端點的 IP 位址</summary>
		public string IP { get { return mIP; } }
		/// <summary>取得端點的埠號</summary>
		public int Port { get { return mPort; } }
		#endregion

		#region Constructors
		/// <summary>建構端點資訊</summary>
		/// <param name="ip">IP 位址</param>
		/// <param name="port">埠號</param>
		public EndPointSlim(string ip, int port) {
			mIP = ip;
			mPort = port;
		}

		/// <summary>建構端點資訊</summary>
		/// <param name="ip">IP 位址</param>
		/// <param name="port">埠號</param>
		public EndPointSlim(IPAddress ip, int port) {
			mIP = ip.ToString();
			mPort = port;
		}

		/// <summary>建構端點資訊</summary>
		/// <param name="ep">端點資訊</param>
		public EndPointSlim(EndPoint ep) {
			if (ep is IPEndPoint) {
				var ipEP = ep as IPEndPoint;
				mIP = ipEP.Address.ToString();
				mPort = ipEP.Port;
			}
		}

		/// <summary>建構端點資訊</summary>
		/// <param name="uri">URI</param>
		public EndPointSlim(Uri uri) {
			mIP = IPAddress.Parse(uri.Host).ToString();
			mPort = uri.Port;
		}
		#endregion

		#region Public Operations
		/// <summary>取得此物件的複製品</summary>
		/// <returns>複製品</returns>
		public EndPointSlim Clone()  {
			return new EndPointSlim(mIP, mPort);
		}

		/// <summary>取得此物件對應的 <see cref="IPEndPoint"/></summary>
		/// <returns>對應端點</returns>
		public EndPoint ToEndPoint() {
			return new IPEndPoint(IPAddress.Parse(mIP), mPort);
		}
		#endregion

		#region Overrides
		/// <summary>取得端點的文字描述，如 "127.0.0.1:9487"</summary>
		/// <returns>文字描述</returns>
		public override string ToString() {
			return $"{mIP}:{mPort.ToString()}";
		}
		#endregion
	}

	/// <summary>用於非同步資料傳遞</summary>
	internal class StateObject {

		#region Constants
		/// <summary>資料陣列長度</summary>
		public static readonly int BufferSize = 4096;
		#endregion

		#region Fields
		private Socket mSocket = null;
		private SocketFlags mFlag = SocketFlags.None;
		private byte[] mBuf = new byte[BufferSize];
		private List<byte> mRemBuf = new List<byte>();
		/// <summary>取得上次通訊時的錯誤狀態</summary>
		/// <remarks>因要使用 out 關鍵字，故只能將之設為公開欄位</remarks>
		public SocketError Error = SocketError.Success;
		#endregion

		#region Properties
		/// <summary>取得此物件所繫結的 <see cref="Socket"/> 物件</summary>
		public Socket WorkSocket { get { return mSocket; } }
		/// <summary>取得通訊時的行為</summary>
		public SocketFlags Flag { get { return mFlag; } }
		/// <summary>取得或設定從 <see cref="NetworkStream"/> 所收到的資料串流</summary>
		public byte[] Buffer {
			get { return mBuf; }
			set { mBuf = value; }
		}
		/// <summary>取得資料處理後，仍未被處理(如資料斷開、不完全)的剩餘串流</summary>
		public List<byte> RemainBuffer { get { return mRemBuf; } }
		#endregion

		#region Constructors
		/// <summary>建構傳遞資料</summary>
		/// <param name="sock">來源 <see cref="Socket"/></param>
		public StateObject(Socket sock) {
			mSocket = sock;
		}
		#endregion
	}

	/// <summary>
	/// 連線資訊。包含 連接/斷線、時間、對方IP與Port
	/// <para>[Client] 連結至 Server 之訊息。IP、Port 即為當前連線狀態變化之 Server</para>
	/// <para>[Server] 此 Client 連線訊息。IP、Port 為此 Client 之相關訊息</para>
	/// </summary>
	public class SocketConnection {
		/// <summary>取得此事件觸發時間</summary>
		public DateTime Time { get; private set; }
		/// <summary>取得連線狀態 (<see langword="true"/>)已連接  (<see langword="false"/>)斷線</summary>
		public bool Status { get; private set; }
		/// <summary>取得發生變化的端點資訊</summary>
		public EndPointSlim EndPoint { get; private set; }
		/// <summary>建立連線資訊</summary>
		/// <param name="stt">連線狀態 (<see langword="true"/>)已連接  (<see langword="false"/>)斷線</param>
		/// <param name="ip">IP 字串，如 "192.168.0.1"</param>
		/// <param name="port">埠號(Port)</param>
		public SocketConnection(bool stt, string ip, int port) {
			Status = stt;
			EndPoint = new EndPointSlim(ip, port);
			Time = DateTime.Now;
		}
		/// <summary>建立連線資訊</summary>
		/// <param name="stt">連線狀態 (<see langword="true"/>)已連接  (<see langword="false"/>)斷線</param>
		/// <param name="ep">端點資訊</param>
		public SocketConnection(bool stt, EndPointSlim ep) {
			Status = stt;
			EndPoint = ep.Clone();
			Time = DateTime.Now;
		}
	}

	/// <summary>
	/// 傳輸資料
	/// <para>用於發報目前接收到的資料</para>
	/// </summary>
	public class SocketRxData {
		/// <summary>接收到此資料的時間</summary>
		public DateTime Time { get; private set; }
		/// <summary>接收到的資料</summary>
		public object Data { get; private set; }
		/// <summary>此筆資料來源裝置之端點資訊</summary>
		public EndPointSlim EndPoint { get; private set; }
		/// <summary>建立傳輸資料物件</summary>
		/// <param name="data">接收到的資料</param>
		/// <param name="ip">此筆資料來源裝置之網際網路位置(IP)</param>
		/// <param name="port">此筆資料來源裝置之埠號(Port)</param>
		public SocketRxData(object data, string ip, int port) {
			Data = data;
			EndPoint = new EndPointSlim(ip, port);
			Time = DateTime.Now;
		}
		/// <summary>建立傳輸資料物件</summary>
		/// <param name="data">接收到的資料</param>
		/// <param name="ep">來源端點資訊</param>
		public SocketRxData(object data, EndPointSlim ep) {
			Data = data;
			EndPoint = ep.Clone();
			Time = DateTime.Now;
		}
	}

	/// <summary>
	/// 傳輸資料
	/// <para>用於發報傳送至目標端點的資料</para>
	/// </summary>
	public class SocketTxData {
		/// <summary>取得此工作執行的 <see cref="Socket"/></summary>
		public Socket WorkSocket { get; }
		/// <summary>發送到此資料的時間</summary>
		public DateTime Time { get; set; }
		/// <summary>發送的資料</summary>
		public object Data { get; }
		/// <summary>此筆資料來源裝置之端點資訊</summary>
		public EndPointSlim EndPoint { get; private set; }
		/// <summary>已傳送的資料位元數</summary>
		public int ByteToSend { get; private set; }
		/// <summary>建立傳輸資料物件</summary>
		/// <param name="data">欲傳送的資料</param>
		/// <param name="sock">取得此工作執行的 <see cref="Socket"/></param>
		public SocketTxData(Socket sock, object data) {
			Data = data;
			WorkSocket = sock;
		}

		/// <summary>建立傳輸資料物件</summary>
		/// <param name="data">欲傳送的資料</param>
		/// <param name="sock">取得此工作執行的 <see cref="Socket"/></param>
		/// <param name="toSend">已傳送的資料位元數</param>
		public SocketTxData(Socket sock, object data, int toSend) {
			Data = data;
			WorkSocket = sock;
			ByteToSend = toSend;
			IPEndPoint endPoint = WorkSocket.RemoteEndPoint as IPEndPoint;
			if (endPoint != null) {
				EndPoint = new EndPointSlim(endPoint);
			}
			Time = DateTime.Now;
		}

		/// <summary>設定已傳送的位元組數量，並取得 IP、Port 與時間</summary>
		/// <param name="length">欲設定的位元組長度</param>
		public void SetTxLength(int length) {
			ByteToSend = length;
			IPEndPoint endPoint = WorkSocket.RemoteEndPoint as IPEndPoint;
			if (endPoint != null) {
				EndPoint = new EndPointSlim(endPoint);
			}
			Time = DateTime.Now;
		}

		/// <summary>設定已傳送的位元組數量，並取得 IP、Port 與時間</summary>
		/// <param name="length">欲設定的位元組長度</param>
		/// <param name="ep">資料來源的端點資訊</param>
		public void SetTxLength(int length, EndPoint ep) {
			ByteToSend = length;
			EndPoint = new EndPointSlim(ep);
			Time = DateTime.Now;
		}
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
}
