using System;
using System.Collections.Generic;

namespace CtLib.Module.Oriental {

	#region Event List
	/// <summary>Oriental Motor 集合式事件</summary>
	public enum BleEvents : byte {
		/// <summary>
		/// 串列埠狀態改變。此值不表示與 Oriental Motor 連線狀態，僅表示連接埠狀態！
		/// <para>附加參數為 <see cref="bool"/>  (<see langword="true"/>)連接埠開啟  (<see langword="false"/>)連接埠關閉</para>
		/// </summary>
		Connection,
		/// <summary>
		/// Oriental Motor 回傳錯誤代碼
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
	/// <summary>Oriental Motor 串列埠事件參數</summary>
	public class BleEventArgs : EventArgs {
		/// <summary>Event</summary>
		public BleEvents Event { get; set; }
		/// <summary>數值</summary>
		public object Value { get; set; }
		/// <summary>建立一新的事件參數</summary>
		/// <param name="events">事件</param>
		/// <param name="value">數值</param>
		public BleEventArgs(BleEvents events, object value) {
			Event = events;
			Value = value;
		}
	} 
	#endregion

}
