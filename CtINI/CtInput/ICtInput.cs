using System;
using System.Collections.Generic;


namespace CtLib.Forms {

	/*
	 * 
	 * ICtInput 版本訊息
	 * 
	 * 1.0.0	Ahern	[2016/04/19]
	 *		+ 建立基本方法
	 * 
	 */

	/// <summary>使用者輸入視窗介面</summary>
	public interface ICtInput : IDisposable {

		#region Methods Declaration
		/// <summary>啟動視窗</summary>
		/// <param name="result">回傳使用者按下確認後的結果</param>
		/// <returns>狀態代碼  (<see cref="Stat.SUCCESS"/>)使用者按下確認鍵 (<see cref="Stat.WN_SYS_USRCNC"/>)使用者取消視窗</returns>
		Stat Start(out List<string> result);

		/// <summary>更新輸入視窗之標題</summary>
		/// <param name="title">欲修改之標題文字</param>
		void SetTitle(string title);

		/// <summary>更新輸入視窗之提示文字</summary>
		/// <param name="desc">提示當前需輸入內容之提示文字</param>
		void SetDescribe(string desc);

		/// <summary>更新輸入視窗之提示文字</summary>
		/// <param name="desc">提示當前需輸入內容之提示文字</param>
		void SetDescribe(IEnumerable<string> desc);

		/// <summary>調整視窗大小</summary>
		void AdjustFormSize(); 
		#endregion
	}
}
