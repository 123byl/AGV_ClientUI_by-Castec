using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using CtLib.Library;
using CtLib.Module.Utility;

namespace CtLib.Forms {
	/// <summary>灰色透明玻璃，用於覆蓋 <see cref="Form"/> 以隱藏其背景物件</summary>
	/// <remarks>Copy from http://stackoverflow.com/questions/4503210/draw-semi-transparent-overlay-image-all-over-the-windows-form-having-some-contro </remarks>
	/// <example><code language="C#">
	/// //於需要時建立此介面，即會顯示並覆蓋整個Form
	/// CtPlexiglass overlayForm = new CtPlexiglass(this);
	/// //於使用完畢後，下 Close 即可解除
	/// overlayForm.Close();
	/// </code></example>
	public partial class CtPlexiglass : Form {

		#region Version

		/// <summary>CtPlexiglass 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0  Ahern [2016/02/15]
		///		+ 從 Stackoverflow 文章複製並建立此視窗，原作者為 "Hans Passant"
		/// 
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(1, 0, 0, "2016/02/15", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Definitions
		private static readonly int DWMWA_TRANSITIONS_FORCEDISABLED = 3;

		/// <summary>覆蓋視窗顏色。亦可於建立後使用 <see cref="Form.BackColor"/> 進行更改</summary>
		private static readonly Color OVERLAY_COLOR = Color.Black;
		/// <summary>覆蓋視窗透明度。亦可於建立後使用 <see cref="Form.Opacity"/> 進行更改</summary>
		private static readonly double OVERLAY_OPCAITY = 0.6;

		#endregion

		#region Declaration - Extern DLL
		/// <summary>Sets the value of non-client rendering attributes for a window</summary>
		/// <param name="hWnd">The handle to the window that will receive the attributes</param>
		/// <param name="attr">A single DWMWINDOWATTRIBUTE flag to apply to the window. This parameter specifies the attribute and the pvAttribute parameter points to the value of that attribute</param>
		/// <param name="value">A pointer to the value of the attribute specified in the dwAttribute parameter. Different DWMWINDOWATTRIBUTE flags require different value types</param>
		/// <param name="attrLen">The size, in bytes, of the value type pointed to by the pvAttribute parameter</param>
		/// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code</returns>
		/// <remarks>https://msdn.microsoft.com/zh-tw/library/windows/desktop/aa969524(v=vs.85).aspx</remarks>
		[DllImport("dwmapi.dll")]
		private static extern int DwmSetWindowAttribute(IntPtr hWnd, int attr, ref int value, int attrLen);
		#endregion

		#region Function - Constructors
		/// <summary>開啟並覆蓋原有視窗</summary>
		/// <param name="tocover">欲覆蓋的視窗</param>
		public CtPlexiglass(Form tocover) {
			/*-- 設定介面參數 --*/
			this.BackColor = OVERLAY_COLOR;
			this.Opacity = OVERLAY_OPCAITY;
			this.FormBorderStyle = FormBorderStyle.None;
			this.ControlBox = false;
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.Manual;
			this.AutoScaleMode = AutoScaleMode.None;
			this.Location = tocover.InvokeIfNecessary(() => tocover.PointToScreen(Point.Empty));
			this.ClientSize = tocover.ClientSize;

			/*-- 添加事件 --*/
			tocover.LocationChanged += Cover_LocationChanged;
			tocover.ClientSizeChanged += Cover_ClientSizeChanged;

			/*-- 顯示介面 --*/
			tocover.InvokeIfNecessary(() => this.Show(tocover));
			CtInvoke.ControlFocus(tocover);
			
			// Disable Aero transitions, the plexiglass gets too visible
			if (Environment.OSVersion.Version.Major >= 6) {
				int value = 1;
				tocover.InvokeIfNecessary(() => DwmSetWindowAttribute(tocover.Handle, DWMWA_TRANSITIONS_FORCEDISABLED, ref value, 4));
			}
		}
		#endregion

		#region Function - Interface Events
		private void Cover_LocationChanged(object sender, EventArgs e) {
			// Ensure the plexiglass follows the owner
			this.Location = this.Owner.PointToScreen(Point.Empty);
		}

		private void Cover_ClientSizeChanged(object sender, EventArgs e) {
			// Ensure the plexiglass keeps the owner covered
			this.ClientSize = this.Owner.ClientSize;
		} 
		#endregion

		#region Function - Overrides
		/// <summary>介面關閉中</summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e) {
			// Restore owner
			this.Owner.LocationChanged -= Cover_LocationChanged;
			this.Owner.ClientSizeChanged -= Cover_ClientSizeChanged;
			if (!this.Owner.IsDisposed && Environment.OSVersion.Version.Major >= 6) {
				int value = 1;
				DwmSetWindowAttribute(this.Owner.Handle, DWMWA_TRANSITIONS_FORCEDISABLED, ref value, 4);
			}
			base.OnFormClosing(e);
		}

		/// <summary>介面激活</summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e) {
			// Always keep the owner activated instead
			this.BeginInvokeIfNecessary(() => this.Owner.Activate());
		}
		#endregion

		#region Function - Drawing
		/// <summary>於覆蓋頁面上顯示文字</summary>
		/// <param name="text">欲顯示的文字</param>
		/// <param name="color">文字顏色</param>
		/// <param name="font">文字字型</param>
		public void DrawText(string text, Color color, Font font) {
			using (Graphics g = this.CreateGraphics()) {
				SizeF size = g.MeasureString(text, font);
				PointF point = new PointF((this.ClientRectangle.Width - size.Width) / 2, (this.ClientRectangle.Height - size.Height) / 2);
				g.DrawString(text, font, new SolidBrush(color), point);
			}
		}
		#endregion
	}
}
