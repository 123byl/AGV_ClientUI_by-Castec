using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CtLib.Forms {
    /// <summary>CASTEC Style 之輸入視窗基底表單</summary>
    internal abstract partial class CtInputBase : Form, ICtInput {

		#region Version

		/// <summary>CtInputBase 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0	Ahern	[2016/04/18]
		///     + 基本樣式
		///     
		/// 1.0.1	Ahern	[2016/04/19]
		///		\ 修飾詞改為 internal
		///		+ Inherit ICtInput
		///     
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(1, 0, 1, "2016/04/19", "Ahern Kuo"); } }

		#endregion

		#region Function - Constructors
		/// <summary>建構輸入視窗</summary>
		public CtInputBase() {
			InitializeComponent();
		}
		#endregion

		#region Function - Private Methods
		private void AdjustDescribeHeight() {
			using (Graphics g = lbDesc.InvokeIfNecessary(() => CreateGraphics())) {
				SizeF txtSize = g.MeasureString(CtInvoke.ControlText(lbDesc), CtInvoke.ControlFont(lbDesc));
				Size frmSize = new Size((int)Math.Ceiling(txtSize.Width) + 10, (int)Math.Ceiling(txtSize.Height));
				lbDesc.InvokeIfNecessary(() => lbDesc.Size = frmSize);

				int lbRight = lbDesc.InvokeIfNecessary(() => lbDesc.Right);
				int btnCent = btnCancel.InvokeIfNecessary(() => btnCancel.Left + btnCancel.Width / 2);
				if ( lbRight > btnCent ) {
					int extendWidth = (int)Math.Abs(lbRight - btnCent);
					this.InvokeIfNecessary(() => this.Width += extendWidth);
				}
			}
		}
		#endregion

		#region Function - ICtInput Virtual Implements
		/// <summary>更改視窗標題</summary>
		/// <param name="title">欲更改的標題文字</param>
		public virtual void SetTitle(string title) {
			CtInvoke.ControlText(this, title);
		}

		/// <summary>更改描述標籤</summary>
		/// <param name="desc">欲更改的描述訊息</param>
		public virtual void SetDescribe(string desc) {
			CtInvoke.ControlText(lbDesc, desc);
			AdjustDescribeHeight();
		}

		/// <summary>更改描述標籤</summary>
		/// <param name="desc">欲更改的描述訊息</param>
		public virtual void SetDescribe(IEnumerable<string> desc) {
			CtInvoke.ControlText(lbDesc, desc);
			AdjustDescribeHeight();
		}

		public virtual void SetContentShift(int distance) {
			pbLogo.InvokeIfNecessary(() => pbLogo.Width += distance);
			lbDesc.InvokeIfNecessary(() => lbDesc.Left += distance);
		}
		#endregion

		#region Function - ICtInput Abstract Implements
		/// <summary>調整視窗大小</summary>
		public abstract void AdjustFormSize();

		/// <summary>顯示輸入視窗</summary>
		/// <param name="result">使用者最後的結果</param>
		/// <returns>狀態代碼 (<see cref="Stat.SUCCESS"/>)使用者按下確認 (<seealso cref="Stat.WN_SYS_USRCNC"/>)使用者按下取消</returns>
		public abstract Stat Start(out List<string> result);
		#endregion
	}
}
