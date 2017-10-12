using System;

namespace CtLib.Module.Adept {

	#region Event Enumerations

	/// <summary>Boolean事件集合</summary>
	public enum AceBoolEvents : byte {
		/// <summary>電源狀態變更</summary>
		PowerChanged,
		/// <summary>連線狀態變更</summary>
		Connection,
		/// <summary>手臂校正狀態變更</summary>
		Calibration,
		/// <summary>影像工具執行方法，重新取像與否</summary>
		/// <remarks>目前僅使用於影像建置器，供判斷是否需要重新取像</remarks>
		VisionExecution
	}

	/// <summary>數值變化事件集合</summary>
	public enum AceNumericEvents : byte {
		/// <summary>
		/// Monitor Speed 變更
		/// <para>事件的數值型態為 int</para>
		/// </summary>
		SpeedChanged,
		/// <summary>
		/// Task狀態發生變動
		/// <para>事件的數值為 object[] { (string)TaskName, (TaskState)CurrentTaskState }</para>
		/// </summary>
		TaskStateChanged,
		/// <summary>
		/// 根目錄物件變更，新增或移除物件
		/// <para>事件的數值型態為 <see cref="string"/></para>
		/// </summary>
		Contents,
	}

	/// <summary>通知事件集合</summary>
	public enum AceNotifyEvents : byte {
		/// <summary>Workspace已載入</summary>
		WorkspaceLoaded,
		/// <summary>Workspace已完成儲存</summary>
		WorkspaceSaved,
		/// <summary>Workspace已卸載，或是Adept ACE關閉</summary>
		WorkspaceUnloaded,
		/// <summary>程式已被編輯</summary>
		ProgramModified,
		/// <summary>ACE軟體關閉</summary>
		AceShutdown
	}

	#endregion

	#region Event Arguments
	/// <summary>Boolean事件參數</summary>
	public class AceBoolEventArgs : EventArgs {
		/// <summary>事件類別</summary>
		public AceBoolEvents Events { get; set; }
		/// <summary>數值</summary>
		public bool Value { get; set; }
		/// <summary>建立一個Boolean事件參數</summary>
		/// <param name="boolEvent">事件類別</param>
		/// <param name="boolValue">數值</param>
		public AceBoolEventArgs(AceBoolEvents boolEvent, bool boolValue) {
			Events = boolEvent;
			Value = boolValue;
		}
	}

	/// <summary>數值變化事件參數</summary>
	public class AceNumericEventArgs : EventArgs {
		/// <summary>事件類別</summary>
		public AceNumericEvents Events { get; set; }
		/// <summary>數值</summary>
		public object Value { get; set; }
		/// <summary>建立一個數值變化的事件參數</summary>
		/// <param name="numEvent">事件類別</param>
		/// <param name="numValue">數值</param>
		public AceNumericEventArgs(AceNumericEvents numEvent, object numValue) {
			Events = numEvent;
			Value = numValue;
		}
	}

	/// <summary>通知事件參數</summary>
	public class AceNotifyEventArgs : EventArgs {
		/// <summary>事件類別</summary>
		public AceNotifyEvents Events { get; set; }
		/// <summary>建立一個通知事件參數</summary>
		/// <param name="notifyEvent">事件類別</param>
		public AceNotifyEventArgs(AceNotifyEvents notifyEvent) {
			Events = notifyEvent;
		}
	}

	/// <summary>訊息事件參數</summary>
	public class AceMessageEventArgs : EventArgs {
		/// <summary>
		/// 訊息類型
		/// <para>-1: Alarm/Exception Message</para>
		/// <para>0: Normal Message</para>
		/// <para>1: Warning Message</para>
		/// </summary>
		public sbyte @Type { get; set; }

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
		public AceMessageEventArgs(sbyte msgType, string msgTitle, string msgContent) {
			Type = msgType;
			Title = msgTitle;
			Message = msgContent;
		}
	} 
	#endregion
}
