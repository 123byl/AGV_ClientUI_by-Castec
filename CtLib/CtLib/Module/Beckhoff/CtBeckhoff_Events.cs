using System;

namespace CtLib.Module.Beckhoff {

	#region Event Enumerations

	/// <summary>Boolean事件列舉</summary>
	public enum BeckhoffBoolEvents : byte {
		/// <summary>開啟連線或是中斷連線之事件</summary>
		Connection = 0
	}

	#endregion

	#region Event Arguments

	/// <summary>Boolean事件參數</summary>
	public class BeckhoffBoolEventArgs : EventArgs {
		/// <summary>事件類別</summary>
		public BeckhoffBoolEvents Events { get; set; }
		/// <summary>數值</summary>
		public bool Value { get; set; }
		/// <summary>建立一個Boolean事件參數</summary>
		/// <param name="boolEvent">事件</param>
		/// <param name="boolValue">數值</param>
		public BeckhoffBoolEventArgs(BeckhoffBoolEvents boolEvent, bool boolValue) {
			Events = boolEvent;
			Value = boolValue;
		}
	}

	/// <summary>Beckhoff Symbol(Variable) 事件參數</summary>
	public class BeckhoffSymbolEventArgs : EventArgs {
		/// <summary>Symbol(Variable)名稱</summary>
		public string Name { get; set; }

		/// <summary>當前數值 (已變化完畢)</summary>
		public object Value { get; set; }

		/// <summary>建立一個Symbol數值變更事件參數</summary>
		/// <param name="strName">變數名稱</param>
		/// <param name="objValue">變更後數值</param>
		public BeckhoffSymbolEventArgs(string strName, object objValue) {
			Name = strName;
			Value = objValue;
		}
	}

	/// <summary>Message事件參數</summary>
	public class BeckhoffMessageEventArgs : EventArgs {
		/// <summary>
		/// 訊息類型
		/// <para>-1: Alarm/Exception Message</para>
		/// <para>0: Normal Message</para>
		/// <para>1: Warning Message</para>
		/// </summary>
		public sbyte Type { get; set; }

		/// <summary>訊息標題</summary>
		public string Title { get; set; }

		/// <summary>訊息內容</summary>
		public string Message { get; set; }

		/// <summary>建構一新的訊息事件參數</summary>
		/// <param name="msgType">
		/// 訊息類型
		/// <para>-1: Alarm/Exception Message</para>
		/// <para>0: Normal Message</para>
		/// <para>1: Warning Message</para>
		/// </param>
		/// <param name="msgTitle">訊息標題</param>
		/// <param name="msgContent">訊息內容</param>
		public BeckhoffMessageEventArgs(sbyte msgType, string msgTitle, string msgContent) {
			Type = msgType;
			Title = msgTitle;
			Message = msgContent;
		}
	}
	#endregion

}
