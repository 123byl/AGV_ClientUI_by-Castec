using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using CtLib.Library;

namespace CtLib.Forms {
	/// <summary>
	/// CASTEC Style 對話視窗
	/// <para>可自訂字體、圖樣，並可透過回傳值取得使用者最後按下的按鈕</para>
	/// </summary>
	/// <example>
	/// 請使用 .Show() 來開啟對話視窗，並接收回傳值
	/// <code language="C#">
	/// MsgBoxBtn button = CtMsgBox.Show("標題", "內容", MsgBoxBtn.OkCancel, MsgBoxStyle.Information);
	/// </code>
	/// 另可在 .Show() 中帶入字型與大小 (共三個多載)
	/// <code language="C#">
	/// MsgBoxBtn button = CtMsgBox.Show("標題", "內容", "微軟正黑體", 28, MsgBoxBtn.OkCancel, MsgBoxStyle.Information);
	/// </code>
	/// 目前預設按鈕組有 OK_CANCEL, YES_NO，如果有需要其他組合，可用 OR 進行變更
	/// <code language="C#">
	/// MsgBoxBtn button = CtMsgBox.Show("標題", "內容", MsgBoxBtn.OK | MsgBoxBtn.No, MsgBoxStyle.None);
	/// </code></example>
	public static class CtMsgBox {

		/// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			string title, string content,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.Information,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog();
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			string title, string content,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.Information,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog(cncSrc);
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			string title, string content,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.Information,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			string title, string content,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.Information,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner, cncSrc));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			UILanguage lang,
			string title, string content,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.Information,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog();
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			UILanguage lang,
			string title, string content,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.Information,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog(cncSrc);
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			UILanguage lang,
			string title, string content,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.Information,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			UILanguage lang,
			string title, string content,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.Information,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner, cncSrc));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			string title, string content,
			float fontSize,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog();
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			string title, string content,
			float fontSize,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog(cncSrc);
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			string title, string content,
			float fontSize,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			string title, string content,
			float fontSize,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner, cncSrc));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			UILanguage lang,
			string title, string content,
			float fontSize,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog();
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			UILanguage lang,
			string title, string content,
			float fontSize,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog(cncSrc);
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			UILanguage lang,
			string title, string content,
			float fontSize,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			UILanguage lang,
			string title, string content,
			float fontSize,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner, cncSrc));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontName">自訂的字型名稱</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			string title, string content,
			string fontName, float fontSize,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontName, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog();
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontName">自訂的字型名稱</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			string title, string content,
			string fontName, float fontSize,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontName, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog(cncSrc);
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontName">自訂的字型名稱</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			string title, string content,
			string fontName, float fontSize,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontName, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontName">自訂的字型名稱</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			string title, string content,
			string fontName, float fontSize,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontName, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner, cncSrc));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontName">自訂的字型名稱</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			UILanguage lang,
			string title, string content,
			string fontName, float fontSize,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, fontName, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog();
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontName">自訂的字型名稱</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			UILanguage lang,
			string title, string content,
			string fontName, float fontSize,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, fontName, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog(cncSrc);
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontName">自訂的字型名稱</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			UILanguage lang,
			string title, string content,
			string fontName, float fontSize,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, fontName, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontName">自訂的字型名稱</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			UILanguage lang,
			string title, string content,
			string fontName, float fontSize,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, fontName, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner, cncSrc));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="font">自訂的字體物件</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			string title, string content,
			Font font,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, font, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog();
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="font">自訂的字體物件</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			string title, string content,
			Font font,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, font, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog(cncSrc);
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="font">自訂的字體物件</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			string title, string content,
			Font font,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, font, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="font">自訂的字體物件</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			string title, string content,
			Font font,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, font, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner, cncSrc));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="font">自訂的字體物件</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			UILanguage lang,
			string title, string content,
			Font font,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, font, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog();
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="font">自訂的字體物件</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			UILanguage lang,
			string title, string content,
			Font font,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, font, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog(cncSrc);
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="font">自訂的字體物件</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			UILanguage lang,
			string title, string content,
			Font font,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, font, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="font">自訂的字體物件</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			UILanguage lang,
			string title, string content,
			Font font,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, font, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner, cncSrc));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			string title, IEnumerable<string> content,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog();
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			string title, IEnumerable<string> content,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog(cncSrc);
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			string title, IEnumerable<string> content,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			string title, IEnumerable<string> content,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner, cncSrc));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			UILanguage lang,
			string title, IEnumerable<string> content,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog();
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			UILanguage lang,
			string title, IEnumerable<string> content,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog(cncSrc);
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			UILanguage lang,
			string title, IEnumerable<string> content,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，以預設的字型為主(微軟正黑體、12pt)</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			UILanguage lang,
			string title, IEnumerable<string> content,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner, cncSrc));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			string title, IEnumerable<string> content,
			float fontSize,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog();
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			string title, IEnumerable<string> content,
			float fontSize,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog(cncSrc);
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			string title, IEnumerable<string> content,
			float fontSize,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			string title, IEnumerable<string> content,
			float fontSize,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner, cncSrc));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			UILanguage lang,
			string title, IEnumerable<string> content,
			float fontSize,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog();
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			UILanguage lang,
			string title, IEnumerable<string> content,
			float fontSize,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog(cncSrc);
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			UILanguage lang,
			string title, IEnumerable<string> content,
			float fontSize,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，字型以預設的字體(微軟正黑體)並自訂字體大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			UILanguage lang,
			string title, IEnumerable<string> content,
			float fontSize,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner, cncSrc));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontName">自訂的字型名稱</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			string title, IEnumerable<string> content,
			string fontName, float fontSize,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontName, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog();
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontName">自訂的字型名稱</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			string title, IEnumerable<string> content,
			string fontName, float fontSize,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontName, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog(cncSrc);
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontName">自訂的字型名稱</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			string title, IEnumerable<string> content,
			string fontName, float fontSize,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontName, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontName">自訂的字型名稱</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			string title, IEnumerable<string> content,
			string fontName, float fontSize,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, fontName, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner, cncSrc));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontName">自訂的字型名稱</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			UILanguage lang,
			string title, IEnumerable<string> content,
			string fontName, float fontSize,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, fontName, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog();
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontName">自訂的字型名稱</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			UILanguage lang,
			string title, IEnumerable<string> content,
			string fontName, float fontSize,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, fontName, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog(cncSrc);
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontName">自訂的字型名稱</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			UILanguage lang,
			string title, IEnumerable<string> content,
			string fontName, float fontSize,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, fontName, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="fontName">自訂的字型名稱</param>
		/// <param name="fontSize">自訂的字體大小</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			UILanguage lang,
			string title, IEnumerable<string> content,
			string fontName, float fontSize,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, fontName, fontSize, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner, cncSrc));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="font">自訂的字體物件</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			string title, IEnumerable<string> content,
			Font font,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, font, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog();
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="font">自訂的字體物件</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			string title, IEnumerable<string> content,
			Font font,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, font, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog(cncSrc);
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="font">自訂的字體物件</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			string title, IEnumerable<string> content,
			Font font,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, font, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="font">自訂的字體物件</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			string title, IEnumerable<string> content,
			Font font,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(title, content, font, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner, cncSrc));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="font">自訂的字體物件</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			UILanguage lang,
			string title, IEnumerable<string> content,
			Font font,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, font, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog();
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="font">自訂的字體物件</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			UILanguage lang,
			string title, IEnumerable<string> content,
			Font font,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, font, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				dr = mMsgBox.ShowDialog(cncSrc);
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="font">自訂的字體物件</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			UILanguage lang,
			string title, IEnumerable<string> content,
			Font font,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, font, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

		/// <summary>顯示自訂的對話視窗，自訂字型名稱與大小</summary>
		/// <param name="handle">欲附加於主視窗的 <see cref="Control.Handle"/></param>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">欲顯示於對話視窗上的標題</param>
		/// <param name="content">欲顯示的內容</param>
		/// <param name="font">自訂的字體物件</param>
		/// <param name="buttons">按鈕集合，可使用 OR 進行組裝</param>
		/// <param name="msgStyle">圖案樣式，無、訊息、警告、詢問、錯誤</param>
		/// <param name="screenNo">欲顯示的螢幕 (-1)以滑鼠當前所在螢幕為主 (Other)螢幕編號，從 0 開始</param>
		/// <param name="cncSrc">表示可關閉視窗之取消旗標</param>
		/// <returns>使用者最後按下的按鈕</returns>
		public static MsgBoxBtn Show(
			IntPtr handle,
			UILanguage lang,
			string title, IEnumerable<string> content,
			Font font,
			CancellationTokenSource cncSrc,
			MsgBoxBtn buttons = MsgBoxBtn.OK, MsgBoxStyle msgStyle = MsgBoxStyle.None,
			int screenNo = -1
		) {
			MsgBoxBtn uiBtn = MsgBoxBtn.OK;
			DialogResult dr = DialogResult.Cancel;
			using (CtMessage_Ctrl mMsgBox = new CtMessage_Ctrl(lang, title, content, font, buttons, msgStyle)) {
				mMsgBox.SetFormScreen(screenNo);
				Control owner = Control.FromHandle(handle);
				dr = owner.InvokeIfNecessary(() => mMsgBox.ShowDialog(owner, cncSrc));
				uiBtn = mMsgBox.UIResult;
			}
			if (dr == DialogResult.Cancel) throw new OperationCanceledException("User close message box from windows task bar.");
			return uiBtn;
		}

	}
}
