using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Utility;

namespace CtLib.Forms {
	/// <summary>適用於密碼之輸入視窗</summary>
	internal partial class CtPasswordInput : CtInputBase {

		#region Version

		/// <summary>CtPasswordInput 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0  Ahern [2017/05/17]
		///     + 從 CtTextInput 複製
		///     
		/// </code></remarks>
		//public new CtVersion Version { get { return new CtVersion(1, 0, 0, "2017/05/17", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Fields
		private TextBox mTxtBox;
		private Stopwatch mAdminTimr = new Stopwatch();
		private bool mAdminActive = false;
		#endregion

		#region Function - Constructors
		/// <summary>建構允許單行文字的輸入視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="admin">是否允許 CASTEC 快速登入</param>
		public CtPasswordInput(string title, string describe, bool admin = false) {
			InitializeComponent();

			SetAcceptButton();
			SetTitle(title);
			SetDescribe(describe);
			CreateTextBox(admin);
			AdjustFormSize();
			
			this.Shown += (sender, e) => CtInvoke.ControlFocus(mTxtBox);
		}

		/// <summary>建構允許單行文字的輸入視窗</summary>
		/// <param name="title">視窗標題</param>
		/// <param name="describe">欲提示使用者的文字</param>
		/// <param name="admin">是否允許 CASTEC 快速登入</param>
		public CtPasswordInput(string title, IEnumerable<string> describe, bool admin = false) {
			InitializeComponent();

			SetAcceptButton();
			SetTitle(title);
			SetDescribe(describe);
			CreateTextBox(admin);
			AdjustFormSize();

			this.Shown += (sender, e) => CtInvoke.ControlFocus(mTxtBox);
		}
		#endregion

		#region Function - Private Methods
		private void SetAcceptButton() {
			this.AcceptButton = btnOK;
		}

		private void CreateTextBox(bool fstLog) {
			mTxtBox = new TextBox() {
				Name = "txtInput",
				Parent = this,
				Font = new Font("微軟正黑體", 12),
				Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
				PasswordChar = '*'
			};
			mTxtBox.Location = new Point(lbDesc.Left, lbDesc.Bottom + 10);
			mTxtBox.Width = btnCancel.Left + btnCancel.Width / 2 - mTxtBox.Left;

			if (fstLog) {
				bool hover = false;
				btnOK.MouseEnter += (sender, e) => hover = true;
				btnOK.MouseLeave += (sender, e) => hover = false;
				mTxtBox.KeyPress += (sender, e) => {
					if (hover) {
						string compare = e.KeyChar.ToString();
						CtCrypto.Encrypt(CryptoMode.AES256, ref compare);
						if ("0bCnNjrk4VEbmr/kE/rglA==".Equals(compare)) {
							mAdminActive = true;
							mAdminTimr.Restart();
						} else if (mAdminActive && mAdminTimr.ElapsedMilliseconds < 500 && "w4jjC6POQaT1sHwSGWzkog==".Equals(compare)) {
							var pwd = "td1+CY1spGpYHBsye0VoEty8UA0rw7g9bQERA7XaKLM=";
							CtCrypto.Decrypt(CryptoMode.AES256, ref pwd);
							CtInvoke.ControlText(mTxtBox, pwd);
							this.InvokeIfNecessary(() => this.DialogResult = DialogResult.OK);
							mAdminTimr.Stop();
						} else {
							mAdminActive = false;
							if (mAdminTimr.IsRunning) mAdminTimr.Stop();
						}
					}
				};
			}

			mTxtBox.Show();
		}

		#endregion

		#region Function - CtInputBase Implements
		/// <summary>調整視窗大小</summary>
		public override void AdjustFormSize() {
			Size formSize = CtInvoke.ControlSize(this);

			string txt = CtInvoke.ControlText(mTxtBox);
			int txtWidth = CtInvoke.ControlSize(mTxtBox).Width;
			if (!string.IsNullOrEmpty(txt)) {
				using (Graphics g = mTxtBox.CreateGraphics()) {
					SizeF defSize = g.MeasureString(txt, CtInvoke.ControlFont(mTxtBox));
					if (defSize.Width > txtWidth) {
						int extendWidth = (int) Math.Abs(defSize.Width - txtWidth);
						formSize.Width += extendWidth;
					}
				}
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
			if (stt == Stat.SUCCESS) result = new List<string> { CtInvoke.ControlText(mTxtBox) };
			else result = new List<string>();
			return stt;
		}
		#endregion
	}
}
