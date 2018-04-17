using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtLib.Module.Net {

	/// <summary>Socket 對應端點</summary>
	public enum SocketRole : byte {
		/// <summary>用戶端</summary>
		Client = 0,
		/// <summary>伺服端</summary>
		Server = 1
	}

	/// <summary>網際網路連線協議</summary>
	public enum NetworkProtocol : byte {
		/// <summary>Transmission Control Protocol</summary>
		Tcp,
		/// <summary>Telnet，隸屬於 TCP/IP</summary>
		Telnet,
		/// <summary>User Datagram Protocol</summary>
		Udp
	}

	/// <summary>CtSyncSocket 事件集合</summary>
	public enum SocketEvents : byte {
		/// <summary>
		/// [Client] Connection status with Server
		/// <para>參數型態為 <see cref="SocketConnection"/></para>
		/// </summary>
		ConnectionWithServer,
		/// <summary>
		/// [Server] Listen Status
		/// <para>參數型態為 <see cref="SocketConnection"/></para>
		/// </summary>
		Listen,
		/// <summary>
		/// [Server] One Client had connected
		/// <para>參數型態為 <see cref="SocketConnection"/></para>
		/// </summary>
		ClientConnection,
		/// <summary>Exception Occurred</summary>
		Exception,
		/// <summary>
		/// 收到已連線裝置傳送之資料
		/// <para>請用 <see cref="SocketRxData"/> 進行拆解</para>
		/// <para>[STRING] 請將 <see cref="SocketRxData.Data"/> 以 <see cref="string"/> 轉換</para>
		/// <para>[BYTE_ARRAY] 請將 <see cref="SocketRxData.Data"/> 以 <see cref="List{T}"/> 轉換。    T 為 <see cref="byte"/></para>
		/// <para>[Serialize] 如使用序列化傳送資料，請設定為 <see cref="CtLib.Library.TransDataFormats.EnumerableOfByte"/> 並自行對收到的 <see cref="List{T}"/> 做反序列化</para>
		/// </summary>
		DataReceived,
		/// <summary>
		/// 傳送之資料至用戶端或伺服端，需訂閱 <see cref="CtSocket.SubscribeSendEvent"/> 才會啟用此事件
		/// <para>請用 <see cref="SocketTxData"/> 進行拆解</para>
		/// <para>[STRING] 請將 <see cref="SocketTxData.Data"/> 以 <see cref="string"/> 轉換</para>
		/// <para>[BYTE_ARRAY] 請將 <see cref="SocketTxData.Data"/> 以 <see cref="byte"/>[] 轉換</para>
		/// </summary>
		DataSend
	}
}
