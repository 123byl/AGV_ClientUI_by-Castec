using System;

namespace CtLib.Module.SerialPort {

	#region Event List
	/// <summary>CtSeral 集合式事件</summary>
	public enum SerialPortEvents : byte {
		/// <summary>串列埠開啟或關閉</summary>
		Connection = 0,
		/// <summary>
		/// 從緩衝區(Buffer)中擷取資料
		/// <para>附帶數值部分請直接轉換為 (STRING)string (BYTE_ARRAY)List&lt;byte&gt;</para>
		/// </summary>
		DataReceived = 1,
		/// <summary>
		/// 錯誤(Error)或例外(Exception)發生
		/// <para>附加參數可能為 string 或 List&lt;byte&gt;</para>
		/// </summary>
		/// <remarks>Error是指如通訊格式、停止位元錯誤等。Exception是指程式，如無COM、資料引數有誤等</remarks>
		Error = 2,
		/// <summary>
		/// 傳送之資料至用戶端或伺服端，需訂閱 <see cref="CtSerial.SubscribeSendEvent"/> 才會啟用此事件
		/// <para>[STRING] 請將 data 以 <see cref="string"/> 轉換</para>
		/// <para>[BYTE_ARRAY] 請將 data 以 <see cref="byte"/>[] 轉換</para>
		/// </summary>
		DataSend = 3
	}
	#endregion

	#region Event Arguments
	/// <summary>CtSerial 串列埠事件參數</summary>
	public class SerialEventArgs : EventArgs {
		/// <summary>Event</summary>
		public SerialPortEvents Event { get; set; }
		/// <summary>數值</summary>
		public object Value { get; set; }
		/// <summary>建立一新的事件參數</summary>
		/// <param name="events">事件</param>
		/// <param name="value">數值</param>
		public SerialEventArgs(SerialPortEvents events, object value) {
			Event = events;
			Value = value;
		}
	}
	#endregion

}
