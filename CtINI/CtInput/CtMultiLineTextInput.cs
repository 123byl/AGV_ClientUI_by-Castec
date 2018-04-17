using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace CtLib.Forms {

	/// <summary>適用於多行文字之輸入視窗</summary>
	internal partial class CtMultiLineTextInput : CtInputBase {

		#region Version

		/// <summary>CtMultiLineTextInput 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0  Ahern [2016/04/19]
		///     + Inherit from CtInputBase
		///     + 基本介面
		///     
		/// </code></remarks>
		//public new CtVersion Version { get { return new CtVersion(1, 0, 0, "2016/04/19", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Fields
		private TextBox mTxtBox;
		#endregion

		#region Function - Constructors
		/// <summary>建構允許多行文字的輸入視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		public CtMultiLineTextInput(string title, string describe, string defValue = "") {
			InitializeComponent();

			SetAcceptButton();
			SetTitle(title);
			SetDescribe(describe);
			CreateTextBox(new string[] { defValue });
			AdjustFormSize();

			this.Shown += (sender, e) => CtInvoke.ControlFocus(mTxtBox);
		}

		/// <summary>建構允許多行文字的輸入視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		public CtMultiLineTextInput(string title, IEnumerable<string> describe, string defValue = "") {
			InitializeComponent();

			SetAcceptButton();
			SetTitle(title);
			SetDescribe(describe);
			CreateTextBox(new string[] { defValue });
			AdjustFormSize();

			this.Shown += (sender, e) => CtInvoke.ControlFocus(mTxtBox);
		}

		/// <summary>建構允許多行文字的輸入視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		public CtMultiLineTextInput(string title, string describe, IEnumerable<string> defValue = null) {
			InitializeComponent();

			SetAcceptButton();
			SetTitle(title);
			SetDescribe(describe);
			CreateTextBox(defValue);
			AdjustFormSize();

			this.Shown += (sender, e) => CtInvoke.ControlFocus(mTxtBox);
		}

		/// <summary>建構允許多行文字的輸入視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		public CtMultiLineTextInput(string title, IEnumerable<string> describe, IEnumerable<string> defValue = null) {
			InitializeComponent();

			SetAcceptButton();
			SetTitle(title);
			SetDescribe(describe);
			CreateTextBox(defValue);
			AdjustFormSize();

			this.Shown += (sender, e) => CtInvoke.ControlFocus(mTxtBox);
		}
		#endregion

		#region Function - Private Methods
		private void SetAcceptButton() {
			this.AcceptButton = null;   //要讓使用者按下 Enter 換行
		}

		private void CreateTextBox(IEnumerable<string> def) {
			string defStr = def == null ? string.Empty : string.Join(Environment.NewLine, def);
			mTxtBox = new TextBox();
			mTxtBox.Name = "txtInput";
			mTxtBox.Parent = this;
			mTxtBox.Font = new Font("微軟正黑體", 12);
			mTxtBox.Location = new Point(lbDesc.Left, lbDesc.Bottom + 10);
			mTxtBox.Multiline = true;
			mTxtBox.Width = btnCancel.Left + btnCancel.Width / 2 - mTxtBox.Left;
			mTxtBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			mTxtBox.ScrollBars = ScrollBars.Both;
			mTxtBox.Text = defStr;
			mTxtBox.Show();
		}
		#endregion

		#region Function - CtInputBase Implements
		/// <summary>調整視窗大小</summary>
		public override void AdjustFormSize() {
			Size formSize = CtInvoke.ControlSize(this);

			string txt = CtInvoke.ControlText(mTxtBox).Split(CtConst.CHR_CRLF, StringSplitOptions.RemoveEmptyEntries).Max();
			Size txtSize = CtInvoke.ControlSize(mTxtBox);
			SizeF defSize = SizeF.Empty;
			using (Graphics g = mTxtBox.CreateGraphics()) {
				defSize = g.MeasureString(string.IsNullOrEmpty(txt) ? "Initialize" : txt, CtInvoke.ControlFont(mTxtBox));
				if (defSize.Width > txtSize.Width) {
					int extendWidth = (int)Math.Abs(defSize.Width - txtSize.Width);
					formSize.Width += extendWidth;
				}

				defSize.Height *= 5;
				mTxtBox.InvokeIfNecessary(() => mTxtBox.Height = (int)defSize.Height);
			}

			int txtBot = mTxtBox.InvokeIfNecessary(() => mTxtBox.Bottom);
			int btnTop = btnOK.InvokeIfNecessary(() => btnOK.Top);
			if ((btnTop - txtBot) < 50) {
				int extendHeight = 50 - (btnTop - txtBot);
				formSize.Height += extendHeight;
			}

			CtInvoke.ControlSize(this, formSize);
		}

		/// <summary>開啟視窗並等待使用者輸入文字或取消，以回傳方式傳回使用者輸入文字</summary>
		/// <param name="result">使用者最後輸入之文字</param>
		/// <returns>狀態代碼 (<see cref="Stat.SUCCESS"/>)使用者按下確定並回傳  (<see cref="Stat.WN_SYS_USRCNC"/>)使用者按下取消鈕</returns>
		public override Stat Start(out List<string> result) {
			this.ShowDialog();
			Stat stt = this.DialogResult == DialogResult.OK ? Stat.SUCCESS : Stat.WN_SYS_USRCNC;
			if (stt == Stat.SUCCESS) {
				result = CtInvoke.ControlText(mTxtBox).Split(CtConst.CHR_CRLF, StringSplitOptions.RemoveEmptyEntries).ToList();
			} else result = new List<string>();
			return stt;
		}
		#endregion
	}
}
