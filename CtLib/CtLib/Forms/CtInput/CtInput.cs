using System.Collections.Generic;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Utility;

namespace CtLib.Forms {

	/// <summary>提供使用者可輸入或選擇項目之使用者介面</summary>
	public static class CtInput {

		#region Version

		/// <summary>CtInput 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0	Ahern	[2016/04/21]
		///     + Factory
		///     + Dialog
		///     
		/// 1.0.1	Ahern	[2016/04/25]
		///		+ NumericPad
		///		
		/// 1.0.2	Ahern	[2017/05/17]
		///		+ Password
		///     
		/// </code></remarks>
		public static CtVersion Version { get { return new CtVersion(1, 0, 2, "2017/05/17", "Ahern Kuo"); } }

		#endregion

		#region Factory

		#region One Line Text
		/// <summary>取得允許單行文字的輸入視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <param name="pwd">是否隱藏使用者輸入的字元？  (<see langword="true"/>)輸入字元變成「*」 (<see langword="false"/>)不進行隱藏</param>
		/// <returns>可供使用者進行輸入的操作視窗</returns>
		public static ICtInput Text(string title, string describe, string defValue = "", bool pwd = false) {
			return new CtTextInput(title, describe, defValue, pwd);
		}

		/// <summary>取得允許單行文字的輸入視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <param name="pwd">是否隱藏使用者輸入的字元？  (<see langword="true"/>)輸入字元變成「*」 (<see langword="false"/>)不進行隱藏</param>
		/// <returns>可供使用者進行輸入的操作視窗</returns>
		public static ICtInput Text(string title, IEnumerable<string> describe, string defValue = "", bool pwd = false) {
			return new CtTextInput(title, describe, defValue, pwd);
		}
		#endregion

		#region Multi-Lines Text
		/// <summary>取得允許多行文字的輸入視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <returns>可供使用者進行輸入的操作視窗</returns>
		public static ICtInput MultiLinesText(string title, string describe, string defValue = "") {
			return new CtMultiLineTextInput(title, describe, defValue);
		}

		/// <summary>取得允許多行文字的輸入視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <returns>可供使用者進行輸入的操作視窗</returns>
		public static ICtInput MultiLinesText(string title, IEnumerable<string> describe, string defValue = "") {
			return new CtMultiLineTextInput(title, describe, defValue);
		}

		/// <summary>取得允許多行文字的輸入視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <returns>可供使用者進行輸入的操作視窗</returns>
		public static ICtInput MultiLinesText(string title, string describe, IEnumerable<string> defValue = null) {
			return new CtMultiLineTextInput(title, describe, defValue);
		}

		/// <summary>取得允許多行文字的輸入視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <returns>可供使用者進行輸入的操作視窗</returns>
		public static ICtInput MultiLinesText(string title, IEnumerable<string> describe, IEnumerable<string> defValue = null) {
			return new CtMultiLineTextInput(title, describe, defValue);
		}
		#endregion

		#region ComboBox List
		/// <summary>取得下拉式清單供使用者選取之視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">下拉式選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="ComboBox"/> 上的選項</param>
		/// <param name="allowEdit">是否允許使用者修改數值?  (<see langword="true"/>)下拉式選單可編輯輸入 (<see langword="false"/>)不可修改下拉式選單之數值</param>
		/// <returns>可供使用者進行輸入的操作視窗</returns>
		public static ICtInput ComboBoxList(string title, string describe, IEnumerable<string> itemList, string defValue = "", bool allowEdit = false) {
			return new CtComboBoxSelector(title, describe, itemList, defValue, allowEdit);
		}

		/// <summary>取得下拉式清單供使用者選取之視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">下拉式選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="ComboBox"/> 上的選項</param>
		/// <param name="allowEdit">是否允許使用者修改數值?  (<see langword="true"/>)下拉式選單可編輯輸入 (<see langword="false"/>)不可修改下拉式選單之數值</param>
		/// <returns>可供使用者進行輸入的操作視窗</returns>
		public static ICtInput ComboBoxList(string title, IEnumerable<string> describe, IEnumerable<string> itemList, string defValue = "", bool allowEdit = false) {
			return new CtComboBoxSelector(title, describe, itemList, defValue, allowEdit);
		}
		#endregion

		#region Checked List
		/// <summary>取得勾選清單供使用者選取之視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">勾選選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="CheckedListBox"/> 上的選項</param>
		/// <param name="onlyOne">是否只允許選擇一個?  (<see langword="true"/>)僅能選一個 (<see langword="false"/>)可選多個</param>
		/// <returns>可供使用者進行輸入的操作視窗</returns>
		public static ICtInput CheckList(string title, string describe, IEnumerable<string> itemList, string defValue = "", bool onlyOne = false) {
			return new CtCheckListSelector(title, describe, itemList, defValue, onlyOne);
		}

		/// <summary>取得勾選清單供使用者選取之視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">勾選選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="CheckedListBox"/> 上的選項</param>
		/// <param name="onlyOne">是否只允許選擇一個?  (<see langword="true"/>)僅能選一個 (<see langword="false"/>)可選多個</param>
		/// <returns>可供使用者進行輸入的操作視窗</returns>
		public static ICtInput CheckList(string title, IEnumerable<string> describe, IEnumerable<string> itemList, string defValue = "", bool onlyOne = false) {
			return new CtCheckListSelector(title, describe, itemList, defValue, onlyOne);
		}

		/// <summary>取得勾選清單供使用者選取之視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">勾選選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="CheckedListBox"/> 上的選項</param>
		/// <param name="onlyOne">是否只允許選擇一個?  (<see langword="true"/>)僅能選一個 (<see langword="false"/>)可選多個</param>
		/// <returns>可供使用者進行輸入的操作視窗</returns>
		public static ICtInput CheckList(string title, string describe, IEnumerable<string> itemList, IEnumerable<string> defValue = null, bool onlyOne = false) {
			return new CtCheckListSelector(title, describe, itemList, defValue, onlyOne);
		}

		/// <summary>取得勾選清單供使用者選取之視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">勾選選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="CheckedListBox"/> 上的選項</param>
		/// <param name="onlyOne">是否只允許選擇一個?  (<see langword="true"/>)僅能選一個 (<see langword="false"/>)可選多個</param>
		/// <returns>可供使用者進行輸入的操作視窗</returns>
		public static ICtInput CheckList(string title, IEnumerable<string> describe, IEnumerable<string> itemList, IEnumerable<string> defValue = null, bool onlyOne = false) {
			return new CtCheckListSelector(title, describe, itemList, defValue, onlyOne);
		}
		#endregion

		#region Numeric Pad
		/// <summary>取得鍵盤式輸入視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <param name="allowEdit">是否允許使用者直接使用鍵盤輸入編輯？  (<see langword="true"/>)可用鍵盤輸入 (<see langword="false"/>)僅能用滑鼠點選介面</param>
		/// <param name="usePoint">是否允許有小數點？  (<see langword="true"/>)可有小數 (<see langword="false"/>)僅能整數</param>
		/// <param name="limFloat">如允許小數點，是否限制位數？  (-1)不限制 (>=0)限制只能輸入多少小數位</param>
		/// <param name="limInt">是否限制整數位？  (-1)不限制 (>=0)限制只能輸入多少整數</param>
		/// <returns>可供使用者進行輸入數字的操作視窗</returns>
		public static ICtInput NumericPad(string title, string describe, string defValue = "", bool usePoint = true, bool allowEdit = false, int limInt = -1, int limFloat = -1) {
			return new CtNumericPad(title, describe, defValue, usePoint, allowEdit, limInt, limFloat);
		}

		/// <summary>取得鍵盤式輸入視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <param name="allowEdit">是否允許使用者直接使用鍵盤輸入編輯？  (<see langword="true"/>)可用鍵盤輸入 (<see langword="false"/>)僅能用滑鼠點選介面</param>
		/// <param name="usePoint">是否允許有小數點？  (<see langword="true"/>)可有小數 (<see langword="false"/>)僅能整數</param>
		/// <param name="limFloat">如允許小數點，是否限制位數？  (-1)不限制 (>=0)限制只能輸入多少小數位</param>
		/// <param name="limInt">是否限制整數位？  (-1)不限制 (>=0)限制只能輸入多少整數</param>
		/// <returns>可供使用者進行輸入數字的操作視窗</returns>
		public static ICtInput NumericPad(string title, IEnumerable<string> describe, string defValue = "", bool usePoint = true, bool allowEdit = false, int limInt = -1, int limFloat = -1) {
			return new CtNumericPad(title, describe, defValue, usePoint, allowEdit, limInt, limFloat);
		}
		#endregion

		#endregion

		#region Dialogs

		#region One Line Text
		/// <summary>開啟允許單行文字的輸入視窗，並等待使用者結束操作</summary>
		/// <param name="result">回傳使用者按下確認後的結果</param>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <param name="pwd">是否隱藏使用者輸入的字元？  (<see langword="true"/>)輸入字元變成「*」 (<see langword="false"/>)不進行隱藏</param>
		/// <returns>狀態代碼  (<see cref="Stat.SUCCESS"/>)使用者按下確認鍵 (<see cref="Stat.WN_SYS_USRCNC"/>)使用者取消視窗</returns>
		public static Stat Text(out string result, string title, string describe, string defValue = "", bool pwd = false) {
			Stat stt = Stat.SUCCESS;
			List<string> strColl;
			using (ICtInput input = new CtTextInput(title, describe, defValue, pwd)) {
				stt = input.Start(out strColl);
			}
			result = (stt == Stat.SUCCESS && strColl.Count > 0) ? strColl[0] : string.Empty;
			return stt;
		}

		/// <summary>開啟允許單行文字的輸入視窗，並等待使用者結束操作</summary>
		/// <param name="result">回傳使用者按下確認後的結果</param>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <param name="pwd">是否隱藏使用者輸入的字元？  (<see langword="true"/>)輸入字元變成「*」 (<see langword="false"/>)不進行隱藏</param>
		/// <returns>狀態代碼  (<see cref="Stat.SUCCESS"/>)使用者按下確認鍵 (<see cref="Stat.WN_SYS_USRCNC"/>)使用者取消視窗</returns>
		public static Stat Text(out string result, string title, IEnumerable<string> describe, string defValue = "", bool pwd = false) {
			Stat stt = Stat.SUCCESS;
			List<string> strColl;
			using (ICtInput input = new CtTextInput(title, describe, defValue, pwd)) {
				stt = input.Start(out strColl);
			}
			result = (stt == Stat.SUCCESS && strColl.Count > 0) ? strColl[0] : string.Empty;
			return stt;
		}
		#endregion

		#region Multi-Lines Text
		/// <summary>開啟允許多行文字的輸入視窗，並等待使用者結束操作</summary>
		/// <param name="result">使用者最後輸入之文字</param>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <returns>狀態代碼 (<see cref="Stat.SUCCESS"/>)使用者按下確定並回傳  (<see cref="Stat.WN_SYS_USRCNC"/>)使用者按下取消鈕</returns>
		public static Stat MultiLinesText(out List<string> result, string title, string describe, string defValue = "") {
			Stat stt = Stat.SUCCESS;
			using (ICtInput input = new CtMultiLineTextInput(title, describe, defValue)) {
				stt = input.Start(out result);
			}
			return stt;
		}

		/// <summary>開啟允許多行文字的輸入視窗，並等待使用者結束操作</summary>
		/// <param name="result">使用者最後輸入之文字</param>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <returns>狀態代碼 (<see cref="Stat.SUCCESS"/>)使用者按下確定並回傳  (<see cref="Stat.WN_SYS_USRCNC"/>)使用者按下取消鈕</returns>
		public static Stat MultiLinesText(out List<string> result, string title, IEnumerable<string> describe, string defValue = "") {
			Stat stt = Stat.SUCCESS;
			using (ICtInput input = new CtMultiLineTextInput(title, describe, defValue)) {
				stt = input.Start(out result);
			}
			return stt;
		}

		/// <summary>開啟允許多行文字的輸入視窗，並等待使用者結束操作</summary>
		/// <param name="result">使用者最後輸入之文字</param>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <returns>狀態代碼 (<see cref="Stat.SUCCESS"/>)使用者按下確定並回傳  (<see cref="Stat.WN_SYS_USRCNC"/>)使用者按下取消鈕</returns>
		public static Stat MultiLinesText(out List<string> result, string title, string describe, IEnumerable<string> defValue = null) {
			Stat stt = Stat.SUCCESS;
			using (ICtInput input = new CtMultiLineTextInput(title, describe, defValue)) {
				stt = input.Start(out result);
			}
			return stt;
		}

		/// <summary>開啟允許多行文字的輸入視窗，並等待使用者結束操作</summary>
		/// <param name="result">使用者最後輸入之文字</param>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <returns>狀態代碼 (<see cref="Stat.SUCCESS"/>)使用者按下確定並回傳  (<see cref="Stat.WN_SYS_USRCNC"/>)使用者按下取消鈕</returns>
		public static Stat MultiLinesText(out List<string> result, string title, IEnumerable<string> describe, IEnumerable<string> defValue = null) {
			Stat stt = Stat.SUCCESS;
			using (ICtInput input = new CtMultiLineTextInput(title, describe, defValue)) {
				stt = input.Start(out result);
			}
			return stt;
		}
		#endregion

		#region ComboBox List
		/// <summary>開啟下拉式清單供使用者選取之視窗，並等待使用者結束操作</summary>
		/// <param name="result">使用者最後選擇之項目</param>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">下拉式選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="ComboBox"/> 上的選項</param>
		/// <param name="allowEdit">是否允許使用者修改數值?  (<see langword="true"/>)下拉式選單可編輯輸入 (<see langword="false"/>)不可修改下拉式選單之數值</param>
		/// <returns>狀態代碼 (<see cref="Stat.SUCCESS"/>)使用者按下確定並回傳  (<see cref="Stat.WN_SYS_USRCNC"/>)使用者按下取消鈕</returns>
		public static Stat ComboBoxList(out string result, string title, string describe, IEnumerable<string> itemList, string defValue = "", bool allowEdit = false) {
			Stat stt = Stat.SUCCESS;
			List<string> strColl;
			using (ICtInput input = new CtComboBoxSelector(title, describe, itemList, defValue, allowEdit)) {
				stt = input.Start(out strColl);
			}
			result = (stt == Stat.SUCCESS && strColl.Count > 0) ? strColl[0] : string.Empty;
			return stt;
		}

		/// <summary>開啟下拉式清單供使用者選取之視窗，並等待使用者結束操作</summary>
		/// <param name="result">使用者最後選擇之項目</param>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">下拉式選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="ComboBox"/> 上的選項</param>
		/// <param name="allowEdit">是否允許使用者修改數值?  (<see langword="true"/>)下拉式選單可編輯輸入 (<see langword="false"/>)不可修改下拉式選單之數值</param>
		/// <returns>狀態代碼 (<see cref="Stat.SUCCESS"/>)使用者按下確定並回傳  (<see cref="Stat.WN_SYS_USRCNC"/>)使用者按下取消鈕</returns>
		public static Stat ComboBoxList(out string result, string title, IEnumerable<string> describe, IEnumerable<string> itemList, string defValue = "", bool allowEdit = false) {
			Stat stt = Stat.SUCCESS;
			List<string> strColl;
			using (ICtInput input = new CtComboBoxSelector(title, describe, itemList, defValue, allowEdit)) {
				stt = input.Start(out strColl);
			}
			result = (stt == Stat.SUCCESS && strColl.Count > 0) ? strColl[0] : string.Empty;
			return stt;
		}
		#endregion

		#region Checked List
		/// <summary>開啟勾選清單供使用者選取之視窗，並等待使用者結束操作</summary>
		/// <param name="result">使用者最後選擇之項目</param>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">勾選選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="CheckedListBox"/> 上的選項</param>
		/// <param name="onlyOne">是否只允許選擇一個?  (<see langword="true"/>)僅能選一個 (<see langword="false"/>)可選多個</param>
		/// <returns>狀態代碼 (<see cref="Stat.SUCCESS"/>)使用者按下確定並回傳  (<see cref="Stat.WN_SYS_USRCNC"/>)使用者按下取消鈕</returns>
		public static Stat CheckList(out List<string> result, string title, string describe, IEnumerable<string> itemList, string defValue = "", bool onlyOne = false) {
			Stat stt = Stat.SUCCESS;
			using (ICtInput input = new CtCheckListSelector(title, describe, itemList, defValue, onlyOne)) {
				stt = input.Start(out result);
			}
			return stt;
		}

		/// <summary>開啟勾選清單供使用者選取之視窗，並等待使用者結束操作</summary>
		/// <param name="result">使用者最後選擇之項目</param>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">勾選選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="CheckedListBox"/> 上的選項</param>
		/// <param name="onlyOne">是否只允許選擇一個?  (<see langword="true"/>)僅能選一個 (<see langword="false"/>)可選多個</param>
		/// <returns>狀態代碼 (<see cref="Stat.SUCCESS"/>)使用者按下確定並回傳  (<see cref="Stat.WN_SYS_USRCNC"/>)使用者按下取消鈕</returns>
		public static Stat CheckList(out List<string> result, string title, IEnumerable<string> describe, IEnumerable<string> itemList, string defValue = "", bool onlyOne = false) {
			Stat stt = Stat.SUCCESS;
			using (ICtInput input = new CtCheckListSelector(title, describe, itemList, defValue, onlyOne)) {
				stt = input.Start(out result);
			}
			return stt;
		}

		/// <summary>開啟勾選清單供使用者選取之視窗，並等待使用者結束操作</summary>
		/// <param name="result">使用者最後選擇之項目</param>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">勾選選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="CheckedListBox"/> 上的選項</param>
		/// <param name="onlyOne">是否只允許選擇一個?  (<see langword="true"/>)僅能選一個 (<see langword="false"/>)可選多個</param>
		/// <returns>狀態代碼 (<see cref="Stat.SUCCESS"/>)使用者按下確定並回傳  (<see cref="Stat.WN_SYS_USRCNC"/>)使用者按下取消鈕</returns>
		public static Stat CheckList(out List<string> result, string title, string describe, IEnumerable<string> itemList, IEnumerable<string> defValue = null, bool onlyOne = false) {
			Stat stt = Stat.SUCCESS;
			using (ICtInput input = new CtCheckListSelector(title, describe, itemList, defValue, onlyOne)) {
				stt = input.Start(out result);
			}
			return stt;
		}

		/// <summary>開啟勾選清單供使用者選取之視窗，並等待使用者結束操作</summary>
		/// <param name="result">使用者最後選擇之項目</param>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">勾選選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="CheckedListBox"/> 上的選項</param>
		/// <param name="onlyOne">是否只允許選擇一個?  (<see langword="true"/>)僅能選一個 (<see langword="false"/>)可選多個</param>
		/// <returns>狀態代碼 (<see cref="Stat.SUCCESS"/>)使用者按下確定並回傳  (<see cref="Stat.WN_SYS_USRCNC"/>)使用者按下取消鈕</returns>
		public static Stat CheckList(out List<string> result, string title, IEnumerable<string> describe, IEnumerable<string> itemList, IEnumerable<string> defValue = null, bool onlyOne = false) {
			Stat stt = Stat.SUCCESS;
			using (ICtInput input = new CtCheckListSelector(title, describe, itemList, defValue, onlyOne)) {
				stt = input.Start(out result);
			}
			return stt;
		}
		#endregion

		#region Numeric Pad
		/// <summary>取得鍵盤式輸入視窗</summary>
		/// <param name="result">回傳使用者按下確認後的結果</param>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <param name="allowEdit">是否允許使用者直接使用鍵盤輸入編輯？  (<see langword="true"/>)可用鍵盤輸入 (<see langword="false"/>)僅能用滑鼠點選介面</param>
		/// <param name="usePoint">是否允許有小數點？  (<see langword="true"/>)可有小數 (<see langword="false"/>)僅能整數</param>
		/// <param name="limFloat">如允許小數點，是否限制位數？  (-1)不限制 (>=0)限制只能輸入多少小數位</param>
		/// <param name="limInt">是否限制整數位？  (-1)不限制 (>=0)限制只能輸入多少整數</param>
		/// <returns>狀態代碼 (<see cref="Stat.SUCCESS"/>)使用者按下確定並回傳  (<see cref="Stat.WN_SYS_USRCNC"/>)使用者按下取消鈕</returns>
		public static Stat NumericPad(out string result, string title, string describe, string defValue = "", bool usePoint = true, bool allowEdit = false, int limInt = -1, int limFloat = -1) {
			Stat stt = Stat.SUCCESS;
			List<string> strColl;
			using (ICtInput input = new CtNumericPad(title, describe, defValue, usePoint, allowEdit, limInt, limFloat)) {
				stt = input.Start(out strColl);
			}
			if (stt == Stat.SUCCESS && strColl.Count > 0) result = strColl[0];
			else result = string.Empty;
			return stt;
		}

		/// <summary>取得鍵盤式輸入視窗</summary>
		/// <param name="result">回傳使用者按下確認後的結果</param>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <param name="allowEdit">是否允許使用者直接使用鍵盤輸入編輯？  (<see langword="true"/>)可用鍵盤輸入 (<see langword="false"/>)僅能用滑鼠點選介面</param>
		/// <param name="usePoint">是否允許有小數點？  (<see langword="true"/>)可有小數 (<see langword="false"/>)僅能整數</param>
		/// <param name="limFloat">如允許小數點，是否限制位數？  (-1)不限制 (>=0)限制只能輸入多少小數位</param>
		/// <param name="limInt">是否限制整數位？  (-1)不限制 (>=0)限制只能輸入多少整數</param>
		/// <returns>狀態代碼 (<see cref="Stat.SUCCESS"/>)使用者按下確定並回傳  (<see cref="Stat.WN_SYS_USRCNC"/>)使用者按下取消鈕</returns>
		public static Stat NumericPad(out string result, string title, IEnumerable<string> describe, string defValue = "", bool usePoint = true, bool allowEdit = false, int limInt = -1, int limFloat = -1) {
			Stat stt = Stat.SUCCESS;
			List<string> strColl;
			using (ICtInput input = new CtNumericPad(title, describe, defValue, usePoint, allowEdit, limInt, limFloat)) {
				stt = input.Start(out strColl);
			}
			if (stt == Stat.SUCCESS && strColl.Count > 0) result = strColl[0];
			else result = string.Empty;
			return stt;
		}
		#endregion

		#region One Line Text
		/// <summary>開啟密碼輸入視窗，並等待使用者結束操作</summary>
		/// <param name="result">回傳使用者按下確認後的結果</param>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="admin">是否允許 CASTEC 快速登入</param>
		/// <returns>狀態代碼  (<see cref="Stat.SUCCESS"/>)使用者按下確認鍵 (<see cref="Stat.WN_SYS_USRCNC"/>)使用者取消視窗</returns>
		public static Stat Password(out string result, string title, string describe, bool admin = false) {
			Stat stt = Stat.SUCCESS;
			List<string> strColl;
			using (ICtInput input = new CtPasswordInput(title, describe, admin)) {
				stt = input.Start(out strColl);
			}
			result = (stt == Stat.SUCCESS && strColl.Count > 0) ? strColl[0] : string.Empty;
			return stt;
		}

		/// <summary>開啟密碼輸入視窗，並等待使用者結束操作</summary>
		/// <param name="result">回傳使用者按下確認後的結果</param>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="admin">是否允許 CASTEC 快速登入</param>
		/// <returns>狀態代碼  (<see cref="Stat.SUCCESS"/>)使用者按下確認鍵 (<see cref="Stat.WN_SYS_USRCNC"/>)使用者取消視窗</returns>
		public static Stat Password(out string result, string title, IEnumerable<string> describe, bool admin = false) {
			Stat stt = Stat.SUCCESS;
			List<string> strColl;
			using (ICtInput input = new CtPasswordInput(title, describe, admin)) {
				stt = input.Start(out strColl);
			}
			result = (stt == Stat.SUCCESS && strColl.Count > 0) ? strColl[0] : string.Empty;
			return stt;
		}
		#endregion

		#endregion
	}
}
