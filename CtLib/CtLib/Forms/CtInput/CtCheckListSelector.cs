using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Utility;

namespace CtLib.Forms {

	/// <summary>勾選清單供使用者選擇之選擇視窗</summary>
	internal partial class CtCheckListSelector : CtInputBase {

		#region Version

		/// <summary>CtCheckListSelector 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0  Ahern [2016/04/20]
		///     + Inherit from CtInputBase
		///     + 基本介面
		///     
		/// </code></remarks>
		public new CtVersion Version { get { return new CtVersion(1, 0, 0, "2016/04/20", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Fields
		private bool mFlag_OnlyOne = true;
		private CheckedListBox mCheckList;
		#endregion

		#region Function - Constructors
		/// <summary>建構勾選清單供使用者選取之視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">勾選選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="CheckedListBox"/> 上的選項</param>
		/// <param name="onlyOne">是否只允許選擇一個?  (<see langword="true"/>)僅能選一個 (<see langword="false"/>)可選多個</param>
		public CtCheckListSelector(string title, string describe, IEnumerable<string> itemList, string defValue = "", bool onlyOne = true) {
			InitializeComponent();

			mFlag_OnlyOne = onlyOne;

			SetAcceptButton();
			SetTitle(title);
			SetDescribe(describe);
			CreateCheckedListBox(itemList, new List<string> { defValue });
			AdjustFormSize();
		}

		/// <summary>建構勾選清單供使用者選取之視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">勾選選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="CheckedListBox"/> 上的選項</param>
		/// <param name="onlyOne">是否只允許選擇一個?  (<see langword="true"/>)僅能選一個 (<see langword="false"/>)可選多個</param>
		public CtCheckListSelector(string title, IEnumerable<string> describe, IEnumerable<string> itemList, string defValue = "", bool onlyOne = true) {
			InitializeComponent();

			mFlag_OnlyOne = onlyOne;

			SetAcceptButton();
			SetTitle(title);
			SetDescribe(describe);
			CreateCheckedListBox(itemList, new List<string> { defValue });
			AdjustFormSize();
		}

		/// <summary>建構勾選清單供使用者選取之視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">勾選選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="CheckedListBox"/> 上的選項</param>
		/// <param name="onlyOne">是否只允許選擇一個?  (<see langword="true"/>)僅能選一個 (<see langword="false"/>)可選多個</param>
		public CtCheckListSelector(string title, string describe, IEnumerable<string> itemList, IEnumerable<string> defValue = null, bool onlyOne = true) {
			InitializeComponent();

			mFlag_OnlyOne = onlyOne;

			SetAcceptButton();
			SetTitle(title);
			SetDescribe(describe);
			CreateCheckedListBox(itemList, defValue);
			AdjustFormSize();
		}

		/// <summary>建構勾選清單供使用者選取之視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">勾選選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="CheckedListBox"/> 上的選項</param>
		/// <param name="onlyOne">是否只允許選擇一個?  (<see langword="true"/>)僅能選一個 (<see langword="false"/>)可選多個</param>
		public CtCheckListSelector(string title, IEnumerable<string> describe, IEnumerable<string> itemList, IEnumerable<string> defValue = null, bool onlyOne = true) {
			InitializeComponent();

			mFlag_OnlyOne = onlyOne;

			SetAcceptButton();
			SetTitle(title);
			SetDescribe(describe);
			CreateCheckedListBox(itemList, defValue);
			AdjustFormSize();
		}
		#endregion

		#region Function - Private Methods
		private void SetAcceptButton() {
			this.AcceptButton = btnOK;
		}

		private void CreateCheckedListBox(IEnumerable<string> itemList, IEnumerable<string> defValue) {
			mCheckList = new CheckedListBox();
			mCheckList.Name = "chkList";
			mCheckList.Parent = this;
			mCheckList.Font = new Font("微軟正黑體", 12);
			mCheckList.Location = new Point(lbDesc.Left, lbDesc.Bottom + 10);
			mCheckList.Width = btnCancel.Left + btnCancel.Width / 2 - mCheckList.Left;
			mCheckList.IntegralHeight = true;
			mCheckList.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			mCheckList.CheckOnClick = true;
			if (mFlag_OnlyOne) mCheckList.ItemCheck += ItemChecked;
			mCheckList.Items.AddRange(itemList.ToArray());
			if (defValue != null && defValue.Count() > 0) {
				int idx = 0;
				List<string> listStr = itemList.ToList();
				foreach (string item in defValue) {
					idx = listStr.FindIndex(str => str == item);
					if (idx > -1) mCheckList.SetItemChecked(idx, true);
				}
			}
			mCheckList.Show();
		}

		private void ItemChecked(object sender, ItemCheckEventArgs e) {
			if (e.NewValue != CheckState.Checked) return;
			if (mCheckList.CheckedIndices.Count > 0)
				mCheckList.BeginInvokeIfNecessary(() => mCheckList.SetItemChecked(mCheckList.CheckedIndices[0], false));
		}
		#endregion

		#region Function - CtInputBase Implements
		/// <summary>調整視窗大小</summary>
		public override void AdjustFormSize() {
			if (mCheckList.Items.Count <= 0) return;

			Size formSize = CtInvoke.ControlSize(this);

			int length = 0;
			string item = mCheckList.Items[0].ToString();
			foreach (string val in mCheckList.Items) {
				if (val.Length > length) {
					length = val.Length;
					item = val;
				}
			}

			Size cbSize = CtInvoke.ControlSize(mCheckList);
			using (Graphics g = mCheckList.CreateGraphics()) {
				SizeF defSize = g.MeasureString(item, CtInvoke.ControlFont(mCheckList));
				if (defSize.Width > cbSize.Width) {
					int extendWidth = (int)Math.Abs(defSize.Width - cbSize.Width) + 10;
					formSize.Width += extendWidth;
				}

				defSize.Height *= 6;
				mCheckList.InvokeIfNecessary(() => mCheckList.Height = (int)defSize.Height);
			}

			int txtBot = mCheckList.InvokeIfNecessary(() => mCheckList.Bottom);
			int btnTop = btnOK.InvokeIfNecessary(() => btnOK.Top);
			if ((btnTop - txtBot) < 50) {
				int extendHeight = 50 - (btnTop - txtBot);
				formSize.Height += extendHeight;
			}

			CtInvoke.ControlSize(this, formSize);
		}

		/// <summary>開啟視窗並等待使用者選擇項目或取消，以回傳方式傳回使用者選擇之項目</summary>
		/// <param name="result">使用者最後選擇之項目</param>
		/// <returns>狀態代碼 (<see cref="Stat.SUCCESS"/>)使用者按下確定並回傳  (<see cref="Stat.WN_SYS_USRCNC"/>)使用者按下取消鈕</returns>
		public override Stat Start(out List<string> result) {
			this.ShowDialog();
			Stat stt = this.DialogResult == DialogResult.OK ? Stat.SUCCESS : Stat.WN_SYS_USRCNC;
			if (stt == Stat.SUCCESS && mCheckList.CheckedItems.Count > 0) {
				result = mCheckList.CheckedItems.Cast<string>().ToList();
			} else result = new List<string>();
			return stt;
		}
		#endregion
	}
}
