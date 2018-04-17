using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtLib.Module.Net {

	#region Enumerations
	/// <summary>串流管道角色</summary>
	public enum PipeModes {
		/// <summary>用戶端</summary>
		Client,
		/// <summary>伺服端</summary>
		Server
	}

	/// <summary>串流管道事件</summary>
	public enum PipeEvents {
		/// <summary>
		/// [Server] 等候用戶端連線之狀態
		/// <para>附加資料為 <see cref="bool"/>。 (<see langword="true"/>)開始等候連線 (<see langword="false"/>)停止等候連線</para>
		/// </summary>
		WaitForConnection,
		/// <summary>
		/// [Client] 已連線至伺服端    [Server] 用戶端已連線
		/// <para>附加資料為 <see cref="bool"/>。 (<see langword="true"/>)連線成功 (<see langword="false"/>)中斷連線</para>
		/// </summary>
		Connection,
		/// <summary>
		/// [All] 已接收資料，請依照 <see cref="CtAsyncPipe.DataFormat"/> 來進行轉型
		/// <para>附加資料為 <see cref="List{T}"/> 或 <seealso cref="string"/>。  T 為 <seealso cref="byte"/></para>
		/// </summary>
		DataReceived,
		/// <summary>
		/// [All] 資料已傳送，請依照 <see cref="CtAsyncPipe.DataFormat"/> 來進行轉型
		/// <para>附加資料為 <see cref="List{T}"/> 或 <seealso cref="string"/>。  T 為 <seealso cref="byte"/></para>
		/// </summary>
		DataSend
	}
	#endregion

	#region Event Arguments
	/// <summary>Pipe 事件參數</summary>
	public class PipeEventArgs : EventArgs {
		/// <summary>事件</summary>
		public PipeEvents Event { get; private set; }
		/// <summary>事件發生時間</summary>
		public DateTime Time { get; private set; }
		/// <summary>此事件所附帶之數值</summary>
		public object Value { get; private set; }
		/// <summary>建立事件參數</summary>
		/// <param name="time">事件發生時間</param>
		/// <param name="events">事件</param>
		/// <param name="value">此事件所附帶之數值</param>
		public PipeEventArgs(DateTime time, PipeEvents events, object value) {
			Time = time;
			Event = events;
			Value = value;
		}
	}
	#endregion
}
