using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Utility;

namespace CtLib.Forms {

	/// <summary>提供鍵盤供使用者點選之輸入視窗</summary>
	internal partial class CtNumericPad : CtInputBase {

		#region Version

		/// <summary>CtNumericPad 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0	Ahern	[2016/04/25]
		///     + Inherit from CtInputBase
		///     + 基本介面
		///     
		/// </code></remarks>
		public new CtVersion Version { get { return new CtVersion(1, 0, 0, "2016/04/25", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Fields
		private TextBox mTxtBox;
		private List<Button> mNumBtn = new List<Button>();

		private int mLimitInt = 10;
		private int mLimitFloat = 5;

		private bool mFlag_AllowPoint = true;
		private bool mFlag_AllowEdit = false;
		#endregion

		#region Function - Constructors
		/// <summary>建構鍵盤式輸入視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <param name="allowEdit">是否允許使用者直接使用鍵盤輸入編輯？  (<see langword="true"/>)可用鍵盤輸入 (<see langword="false"/>)僅能用滑鼠點選介面</param>
		/// <param name="usePoint">是否允許有小數點？  (<see langword="true"/>)可有小數 (<see langword="false"/>)僅能整數</param>
		/// <param name="limFloat">如允許小數點，是否限制位數？  (-1)不限制 (>=0)限制只能輸入多少小數位</param>
		/// <param name="limInt">是否限制整數位？  (-1)不限制 (>=0)限制只能輸入多少整數</param>
		public CtNumericPad(string title, string describe, string defValue = "", bool usePoint = true, bool allowEdit = false, int limInt = -1, int limFloat = -1) {
			InitializeComponent();

			mLimitFloat = limFloat;
			mLimitInt = limInt;

			mFlag_AllowEdit = allowEdit;
			mFlag_AllowPoint = usePoint;

			SetContentShift(50);
			SetAcceptButton();
			SetTitle(title);
			SetDescribe(describe);
			CreateTextBox(defValue);
			CreateButtons();
			AdjustFormSize();

			this.Shown += (sender, e) => CtInvoke.ControlFocus(mTxtBox);

			if (!mFlag_AllowPoint) CtInvoke.ControlEnabled(mNumBtn.Find(btn => btn.Text == "."), false);
		}

		/// <summary>建構鍵盤式輸入視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="defValue">一開始欲顯示在 <see cref="TextBox"/> 上的文字</param>
		/// <param name="allowEdit">是否允許使用者直接使用鍵盤輸入編輯？  (<see langword="true"/>)可用鍵盤輸入 (<see langword="false"/>)僅能用滑鼠點選介面</param>
		/// <param name="usePoint">是否允許有小數點？  (<see langword="true"/>)可有小數 (<see langword="false"/>)僅能整數</param>
		/// <param name="limFloat">如允許小數點，是否限制位數？  (-1)不限制 (>=0)限制只能輸入多少小數位</param>
		/// <param name="limInt">是否限制整數位？  (-1)不限制 (>=0)限制只能輸入多少整數</param>
		public CtNumericPad(string title, IEnumerable<string> describe, string defValue = "", bool usePoint = true, bool allowEdit = false, int limInt = -1, int limFloat = -1) {
			InitializeComponent();

			mLimitFloat = limFloat;
			mLimitInt = limInt;

			mFlag_AllowEdit = allowEdit;
			mFlag_AllowPoint = usePoint;

			SetContentShift(50);
			SetAcceptButton();
			SetTitle(title);
			SetDescribe(describe);
			CreateTextBox(defValue);
			CreateButtons();
			AdjustFormSize();

			this.Shown += (sender, e) => CtInvoke.ControlFocus(mTxtBox);

			if (!mFlag_AllowPoint) CtInvoke.ControlEnabled(mNumBtn.Find(btn => btn.Text == "."), false);
		}

		#endregion

		#region Function - Private Methods
		private void SetAcceptButton() {
			this.AcceptButton = btnOK;
		}

		private void CreateTextBox(string def) {
			mTxtBox = new TextBox();
			mTxtBox.Name = "txtInput";
			mTxtBox.Parent = this;
			mTxtBox.Font = new Font("Verdana", 12);
			mTxtBox.Location = new Point(lbDesc.Left, lbDesc.Bottom + 10);
			mTxtBox.Width = 30;
			mTxtBox.ReadOnly = !mFlag_AllowEdit;
			mTxtBox.ImeMode = ImeMode.Disable;
			mTxtBox.TextAlign = HorizontalAlignment.Right;
			mTxtBox.Text = def;
			mTxtBox.Show();
			mTxtBox.KeyPress += TextBoxKeyPressed;
		}

		private Button CreateButtonInstance(int idx, Size btnSize, Font btnFont) {
			Button btn = new Button();
			btn.Name = string.Format("btnNum{0}", idx);
			btn.Parent = this;
			btn.Font = btnFont;
			btn.Size = btnSize;
			btn.TextAlign = ContentAlignment.MiddleCenter;
			btn.Text = idx.ToString();
			btn.FlatAppearance.BorderColor = Color.Silver;
			btn.FlatAppearance.BorderSize = 1;
			btn.FlatAppearance.MouseDownBackColor = Color.DeepSkyBlue;
			btn.FlatAppearance.MouseOverBackColor = Color.LightSkyBlue;
			btn.FlatStyle = FlatStyle.Flat;
			btn.Click += ButtonClicked;
			btn.Show();
			return btn;
		}

		private void CreateButtons() {
			Size btnSize = new Size(50, 50);
			Font btnFont = new Font("Verdana", 12);
			Point btnStartLoc = new Point(mTxtBox.Left, mTxtBox.Bottom + btnSize.Height * 2 + 20);
			for (int idx = 1; idx < 10; idx++) {
				Button btn = CreateButtonInstance(idx, btnSize, btnFont);
				btn.Location = new Point(btnStartLoc.X + ((idx - 1) % 3) * (btnSize.Width + 5), btnStartLoc.Y - ((idx - 1) / 3) * (btnSize.Height + 5));
				mNumBtn.Add(btn);
			}

			Button btnZero = CreateButtonInstance(0, new Size(btnSize.Width * 2 + 5, btnSize.Height), btnFont);
			btnZero.Location = new Point(btnStartLoc.X, btnStartLoc.Y + btnSize.Height + 5);
			mNumBtn.Add(btnZero);

			Button btnPoint = CreateButtonInstance(100, btnSize, btnFont);
			btnPoint.Text = ".";
			btnPoint.Location = new Point(mNumBtn.Find(ctrl => ctrl.Text == "3").Left, btnStartLoc.Y + btnSize.Height + 5);
			mNumBtn.Add(btnPoint);

			Button btnBak = CreateButtonInstance(101, btnSize, new Font("Consolas", btnFont.Size));
			btnBak.Text = "←";
			btnBak.Location = new Point(mNumBtn.Find(ctrl => ctrl.Text == "9").Right + 5, mNumBtn.Find(ctrl => ctrl.Text == "9").Top);
			mNumBtn.Add(btnBak);

			Button btnPN = CreateButtonInstance(102, btnSize, btnFont);
			btnPN.Text = "±";
			btnPN.Location = new Point(mNumBtn.Find(ctrl => ctrl.Text == "6").Right + 5, mNumBtn.Find(ctrl => ctrl.Text == "6").Top);
			mNumBtn.Add(btnPN);

			Button btnClr = CreateButtonInstance(103, new Size(btnSize.Width, btnSize.Height * 2 + 5), btnFont);
			btnClr.Text = "CLR";
			btnClr.Location = new Point(mNumBtn.Find(ctrl => ctrl.Text == "3").Right + 5, mNumBtn.Find(ctrl => ctrl.Text == "3").Top);
			mNumBtn.Add(btnClr);
		}

		private bool IsDigitAllowed(string value) {
			bool allow = true;
			int pointIdx = value.IndexOf(".");

			if (pointIdx > -1 && mLimitFloat > -1) {
				string numI = value.Substring(0, pointIdx).Replace("-", "");
				string numF = value.Substring(pointIdx + 1); //因小數點是用 Index 外加有限定負號位置，所以可以直接用 index 即可，不用 Replace
				allow = (numI.Length <= mLimitInt) && (numF.Length <= mLimitFloat);
			} else if (mLimitInt > -1) allow = value.Replace("-", "").Length <= mLimitInt;
			return allow;
		}
		#endregion

		#region Function - CtInputBase Implements
		/// <summary>調整視窗大小</summary>
		public override void AdjustFormSize() {
			Size formSize = CtInvoke.ControlSize(this);

			int txtWidth = Math.Abs(mNumBtn.Find(btn => btn.Text == "CLR").Right - mNumBtn.Find(btn => btn.Text == "7").Left);
			mTxtBox.InvokeIfNecessary(() => mTxtBox.Width = txtWidth);

			formSize.Width = mTxtBox.Right + btnCancel.Width / 2 + 20;
			formSize.Height = mNumBtn.Find(btn => btn.Text == "CLR").Bottom + btnOK.Height + 80;

			CtInvoke.ControlSize(this, formSize);
		}

		/// <summary>開啟視窗並等待使用者輸入文字或取消，以回傳方式傳回使用者輸入文字</summary>
		/// <param name="result">使用者最後輸入之文字</param>
		/// <returns>狀態代碼 (<see cref="Stat.SUCCESS"/>)使用者按下確定並回傳  (<see cref="Stat.WN_SYS_USRCNC"/>)使用者按下取消鈕</returns>
		public override Stat Start(out List<string> result) {
			this.ShowDialog();
			Stat stt = this.DialogResult == DialogResult.OK ? Stat.SUCCESS : Stat.WN_SYS_USRCNC;
			if (stt == Stat.SUCCESS) {
				result = new List<string> { mTxtBox.Text };
			} else result = new List<string>();
			return stt;
		}
		#endregion

		#region Function - Events
		private void TextBoxKeyPressed(object sender, KeyPressEventArgs e) {
			string newText = string.Empty;
			string txtText = CtInvoke.ControlText(mTxtBox);
			if (e.KeyChar == '-') {
				double val;
				if (txtText == "-") newText = string.Empty;
				else if (string.IsNullOrEmpty(txtText)) newText = "-";
				else if (double.TryParse(txtText, out val)) newText = (val * -1).ToString();
				else newText = txtText; //懶得再加 flag 判斷允不允許，不允許就直接原樣就好
			} else if (e.KeyChar == '.') {
				if (!txtText.Contains(".")) newText = string.Format("{0}{1}", txtText, ".");
				else newText = txtText; //懶得再加 flag 判斷允不允許，不允許就直接原樣就好
			} else if (char.IsDigit(e.KeyChar)) {
				newText = string.Format("{0}{1}", txtText, e.KeyChar);
			} else newText = txtText;
			if (IsDigitAllowed(newText)) mTxtBox.InvokeIfNecessary(() => mTxtBox.Text = newText);
			e.Handled = !char.IsControl(e.KeyChar);
		}
		private void ButtonClicked(object sender, EventArgs e) {
			Button btn = sender as Button;
			string btnText = CtInvoke.ControlText(btn);
			string txtText = CtInvoke.ControlText(mTxtBox);
			string newText = string.Empty;
			switch (btnText) {
				case "0":
				case "1":
				case "2":
				case "3":
				case "4":
				case "5":
				case "6":
				case "7":
				case "8":
				case "9":
					newText = string.Format("{0}{1}", txtText, btnText);
					break;
				case ".":
					if (!txtText.Contains(btnText)) newText = string.Format("{0}{1}", txtText, btnText);
					else newText = txtText; //懶得再加 flag 判斷允不允許，不允許就直接原樣就好
					break;
				case "CLR":
					newText = string.Empty;
					break;
				case "±":
					double val;
					if (!string.IsNullOrEmpty(txtText) && double.TryParse(txtText, out val)) {
						val = val * -1;
						newText = val.ToString();
					} else if (txtText == "-") newText = string.Empty;
					else newText = "-";
					break;
				case "←":
					if (!string.IsNullOrEmpty(txtText) && txtText.Length > 0)
						newText = txtText.Substring(0, txtText.Length - 1);
					break;
				default:
					break;
			}

			if (IsDigitAllowed(newText)) CtInvoke.ControlText(mTxtBox, newText);
		}
		#endregion
	}
}
