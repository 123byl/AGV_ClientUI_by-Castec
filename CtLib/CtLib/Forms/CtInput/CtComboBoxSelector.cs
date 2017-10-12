using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Utility;

namespace CtLib.Forms {

	/// <summary>下拉式選單供使用者選擇之選擇視窗</summary>
	internal partial class CtComboBoxSelector : CtInputBase {

		#region Version

		/// <summary>CtComboBoxSelector 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0  Ahern [2016/04/20]
		///     + Inherit from CtInputBase
		///     + 基本介面
		///     
		/// </code></remarks>
		public new CtVersion Version { get { return new CtVersion(1, 0, 0, "2016/04/20", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Fields
		private bool mFlag_AllowEdit = false;
		private ComboBox mComboBox;
		#endregion

		#region Function - Constructors
		/// <summary>建構下拉式清單供使用者選取之視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">下拉式選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="ComboBox"/> 上的選項</param>
		/// <param name="allowEdit">是否允許使用者修改數值?  (<see langword="true"/>)下拉式選單可編輯輸入 (<see langword="false"/>)不可修改下拉式選單之數值</param>
		public CtComboBoxSelector(string title, string describe, IEnumerable<string> itemList, string defValue = "", bool allowEdit = false) {
			InitializeComponent();

			mFlag_AllowEdit = allowEdit;

			SetAcceptButton();
			SetTitle(title);
			SetDescribe(describe);
			CreateComboBox(itemList, defValue);
			AdjustFormSize();
		}

		/// <summary>建構下拉式清單供使用者選取之視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="itemList">下拉式選單所顯示的內容</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="ComboBox"/> 上的選項</param>
		/// <param name="allowEdit">是否允許使用者修改數值?  (<see langword="true"/>)下拉式選單可編輯輸入 (<see langword="false"/>)不可修改下拉式選單之數值</param>
		public CtComboBoxSelector(string title, IEnumerable<string> describe, IEnumerable<string> itemList, string defValue = "", bool allowEdit = false) {
			InitializeComponent();

			mFlag_AllowEdit = allowEdit;

			SetAcceptButton();
			SetTitle(title);
			SetDescribe(describe);
			CreateComboBox(itemList, defValue);
			AdjustFormSize();
		}
		#endregion

		#region Function - Private Methods
		private void SetAcceptButton() {
			this.AcceptButton = btnOK;
		}

		private void CreateComboBox(IEnumerable<string> itemList, string defValue) {
			mComboBox = new ComboBox();
			mComboBox.Name = "comboList";
			mComboBox.Parent = this;
			mComboBox.DropDownStyle = mFlag_AllowEdit ? ComboBoxStyle.DropDown : ComboBoxStyle.DropDownList;
			mComboBox.Font = new Font("微軟正黑體", 12);
			mComboBox.Location = new Point(lbDesc.Left, lbDesc.Bottom + 10);
			mComboBox.Width = btnCancel.Left + btnCancel.Width / 2 - mComboBox.Left;
			mComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			mComboBox.Items.AddRange(itemList.ToArray());
			if (!string.IsNullOrEmpty(defValue)) {
				if (itemList.Contains(defValue)) mComboBox.SelectedItem = defValue;
				else mComboBox.Text = defValue;
			} else if (mComboBox.Items.Count > 0) mComboBox.SelectedIndex = 0;
			mComboBox.Show();
		}
		#endregion

		#region Function - CtInputBase Implements
		/// <summary>調整視窗大小</summary>
		public override void AdjustFormSize() {
			Size formSize = CtInvoke.ControlSize(this);

			string item = mComboBox.Items.Cast<string>().Max();
			Size cbSize = CtInvoke.ControlSize(mComboBox);
			using (Graphics g = mComboBox.CreateGraphics()) {
				SizeF defSize = g.MeasureString(item, CtInvoke.ControlFont(mComboBox));
				if (defSize.Width > cbSize.Width) {
					int extendWidth = (int)Math.Abs(defSize.Width - cbSize.Width);
					formSize.Width += extendWidth;
				}
			}

			int txtBot = mComboBox.InvokeIfNecessary(() => mComboBox.Bottom);
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
			if (stt == Stat.SUCCESS) {
				result = new List<string> { CtInvoke.ControlText(mComboBox) };
			} else result = new List<string>();
			return stt;
		}
		#endregion
	}
}
