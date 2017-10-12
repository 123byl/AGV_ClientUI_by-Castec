using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtLib.Module.IAI {

	#region Event List
	/// <summary>IAI PCON 集合式事件</summary>
	public enum PconEvents : byte {
		/// <summary>
		/// 串列埠狀態改變。此值不表示與 IAI PCON 連線狀態，僅表示連接埠狀態！
		/// <para>附加參數為 <see cref="bool"/>  (<see langword="true"/>)連接埠開啟  (<see langword="false"/>)連接埠關閉</para>
		/// </summary>
		Connection,
		/// <summary>
		/// IAI PCON 回傳錯誤代碼
		/// <para>附加參數為 <see cref="List{T}"/> , T 為 <seealso cref="byte"/></para>
		/// </summary>
		DeviceError,
		/// <summary>
		/// 串列埠通訊錯誤
		/// <para>附加參數為 <see cref="string"/></para>
		/// </summary>
		CommunicationError
	}
	#endregion

	#region Event Arguments
	/// <summary>IAI PCON 串列埠事件參數</summary>
	public class PconEventArgs : EventArgs {
		/// <summary>Event</summary>
		public PconEvents Event { get; set; }
		/// <summary>數值</summary>
		public object Value { get; set; }
		/// <summary>建立一新的事件參數</summary>
		/// <param name="events">事件</param>
		/// <param name="value">數值</param>
		public PconEventArgs(PconEvents events, object value) {
			Event = events;
			Value = value;
		}
	}
	#endregion
}
