using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using CtLib.Module.Utility.Drawing;

namespace CtLib.Module.Utility.Renderer {

	#region ToolStrip Renderer
	/// <summary><see cref="ToolStrip"/> 渲染，於下方繪製一條固定高度之顏色條，由 <see cref="ToolStripItem.Tag"/> 來決定顏色</summary>
	public class ToolStripBottomBarRenderer : ToolStripProfessionalRenderer {

		#region Fields
		/// <summary>渲染 <see cref="ToolStripItem"/> 時之線條高度 (滑鼠進入時)</summary>
		private int mHoverHeight = 4;
		/// <summary><see cref="ToolStripItem"/> 前方(Header)寬度</summary>
		private int mTitleWidth = 33;
		#endregion

		#region Overrides
		/// <summary>[Override] 渲染背景事件</summary>
		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
			if (!e.Item.Selected || !e.Item.Enabled) {
				//base.OnRenderMenuItemBackground(e);
				Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
				e.Graphics.FillRectangle(Color.Transparent.GetSolidBrush(), rect);
			} else {
				int titleWidth = (e.Item.OwnerItem != null) ? mTitleWidth : 0;
				Rectangle rect = new Rectangle(titleWidth, e.Item.Height - mHoverHeight, e.Item.Width - titleWidth, mHoverHeight);
				Color rendClr = e.Item.Tag.ToString().ColorParser();
				e.Graphics.FillRectangle(rendClr.GetSolidBrush(), rect);
			}
		}

		/// <summary>[Override] 渲染整體 ToolStrip 背景事件</summary>
		protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e) {
			base.OnRenderToolStripBackground(e);
			Rectangle rect = new Rectangle(Point.Empty, e.ToolStrip.Size);
			e.Graphics.FillRectangle(e.BackColor.GetSolidBrush(), rect);
		}

		/// <summary>[Override] 渲染 Seperator 事件</summary>
		protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e) {
			Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
			e.Graphics.FillRectangle(Color.Transparent.GetSolidBrush(), rect);
			base.OnRenderSeparator(e);
		}
		#endregion
	}

	/// <summary>模仿 Visual Studio 深色系主題之 <see cref="ToolStrip"/> 渲染</summary>
	public class ToolStripDeepStyleRenderer : ToolStripProfessionalRenderer {

		#region Fields
		/// <summary>深色系主題之文字顏色</summary>
		public static readonly Color COLOR_TEXT = Color.FromArgb(220, 220, 220);
		/// <summary>深色系主題未觸發任何事件時的顏色</summary>
		public static readonly Color COLOR_NORMAL = Color.FromArgb(45, 45, 45);
		/// <summary>深色系主題之滑鼠停留時顏色</summary>
		public static readonly Color COLOR_MOUSE_HOVER = Color.FromArgb(62, 62, 62);
		/// <summary>深色系主題之滑鼠按下時的顏色</summary>
		public static readonly Color COLOR_MOUSE_DOWN = Color.FromArgb(28, 28, 28);
		/// <summary>深色系主題之邊框顏色</summary>
		public static readonly Color COLOR_BORDER = Color.FromArgb(52, 52, 52);
		/// <summary>深色系主題之符號顏色</summary>
		public static readonly Color COLOR_MARK = Color.FromArgb(180, 180, 180);

		private static readonly Font mCheckMark = new Font("Consolas", 12);
		#endregion

		#region Overrides
		/// <summary>[Override] 渲染背景事件</summary>
		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
			Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
			if (e.Item.Pressed) {
				Rectangle bordRect = new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height);
				e.Graphics.FillRectangle(COLOR_MOUSE_DOWN.GetSolidBrush(), rect);
				if (!(e.Item is ToolStripDropDownItem) || !e.Item.IsOnDropDown)
					e.Graphics.DrawRectangle(COLOR_BORDER.GetPen(1), bordRect);
			} else if (e.Item.Selected && e.Item.IsOnDropDown && e.Item.Enabled) {
				Rectangle spaceRect = new Rectangle(rect.X + 4, rect.Y + 2, rect.Width - 7, rect.Height - 4);
				e.Graphics.FillRectangle(COLOR_MOUSE_DOWN.GetSolidBrush(), rect);
				e.Graphics.FillRectangle(COLOR_MOUSE_HOVER.GetSolidBrush(), spaceRect);
			} else if (e.Item.Selected && e.Item.Enabled) {
				e.Graphics.FillRectangle(COLOR_MOUSE_HOVER.GetSolidBrush(), rect);
			} else if (e.Item.IsOnDropDown) {
				e.Graphics.FillRectangle(COLOR_MOUSE_DOWN.GetSolidBrush(), rect);
			} else {
				e.Graphics.FillRectangle(COLOR_NORMAL.GetSolidBrush(), rect);
			}
		}

		/// <summary>[Override] 渲染整體 ToolStrip 背景事件</summary>
		protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e) {
			Rectangle rect = new Rectangle(Point.Empty, e.ToolStrip.Size);
			if (e.ToolStrip.IsDropDown)
				e.Graphics.FillRectangle(COLOR_MOUSE_DOWN.GetSolidBrush(), rect);
			else e.Graphics.FillRectangle(COLOR_NORMAL.GetSolidBrush(), rect);
		}

		/// <summary>[Override] 渲染 Seperator 事件</summary>
		protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e) {
			Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
			e.Graphics.FillRectangle(COLOR_MOUSE_DOWN.GetSolidBrush(), rect);

			float y = rect.Height / 2;
			float x1 = 25;
			float x2 = rect.Width - 5;
			e.Graphics.DrawLine(COLOR_BORDER.GetPen(1), x1, y, x2, y);
		}

		/// <summary>[Override] 渲染 Text</summary>
		/// <param name="e">事件參數</param>
		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e) {
			var brush = e.Item.Enabled ? COLOR_TEXT.GetSolidBrush() : Brushes.DimGray;
			e.Graphics.DrawString(e.Text, e.TextFont, brush, e.TextRectangle.X, e.TextRectangle.Y);
		}

		/// <summary>[Override] 渲染邊緣</summary>
		/// <param name="e">事件參數</param>
		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
			ToolStripDropDownMenu drpDnMenu = e.ToolStrip as ToolStripDropDownMenu;
			if (drpDnMenu != null) {
				Rectangle rect = new Rectangle(e.AffectedBounds.X, e.AffectedBounds.Y, e.AffectedBounds.Width - 1, e.AffectedBounds.Height - 1);
				Rectangle rect2 = new Rectangle(e.AffectedBounds.X + 1, e.AffectedBounds.Y + 1, e.AffectedBounds.Width - 3, e.AffectedBounds.Height - 3);
				e.Graphics.DrawRectangle(COLOR_MOUSE_DOWN.GetPen(1), rect2);
				e.Graphics.DrawRectangle(COLOR_BORDER.GetPen(1), rect);
				e.Graphics.FillRectangle(COLOR_MOUSE_DOWN.GetSolidBrush(), e.ConnectedArea);
			}
		}

		/// <summary>[Override] 渲染右側箭頭</summary>
		/// <param name="e">事件參數</param>
		protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e) {
			e.ArrowColor = COLOR_MARK;
			base.OnRenderArrow(e);
		}

		/// <summary>[Override] 渲染打勾</summary>
		/// <param name="e">事件參數</param>
		protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e) {
			// 221A = 根號 = √   2713 = 打勾 = ✓ ... 但在這裡，似乎根號比較明顯 XD

			Brush brush = e.Item.Selected ? Brushes.White : COLOR_MARK.GetSolidBrush();
			SizeF txtSize = e.Graphics.MeasureString("\u221A", mCheckMark);
			PointF txtPos = new PointF(e.ImageRectangle.X + (e.ImageRectangle.Width - txtSize.Width) / 2, e.ImageRectangle.Y + (e.ImageRectangle.Height - txtSize.Height) / 2);

			e.Graphics.FillRectangle(COLOR_MOUSE_HOVER.GetSolidBrush(), e.ImageRectangle);
			e.Graphics.DrawString("\u221A", mCheckMark, brush, txtPos);
		}

		/// <summary>[Override] 渲染左側圖案區域</summary>
		/// <param name="e">事件參數</param>
		protected override void OnRenderImageMargin(ToolStripRenderEventArgs e) {
			e.Graphics.FillRectangle(COLOR_MOUSE_DOWN.GetSolidBrush(), e.AffectedBounds);
		}
		#endregion
	}
	#endregion

	#region Button Renderer
	/// <summary><see cref="Button"/> 渲染，於下方繪製一條固定高度之顏色條</summary>
	public class ButtonBottomBarRenderer {

		#region Declaration - Definitions
		/// <summary>重繪按鈕樣式之線條高度 (滑鼠未進入時)</summary>
		private int mNormalHeight = 4;
		/// <summary>重繪按鈕樣式之線條高度 (滑鼠進入時)</summary>
		private int mHoverHeight = 8;
		#endregion

		#region Declaration - Fields
		/// <summary>[Flag] 按鈕是否需要重繪線條</summary>
		private bool mFlag_BtnDrawLine = false;
		/// <summary>重繪/渲染時的 Brush 樣式</summary>
		private Brush mBrush;
		#endregion

		#region Declaration - Properties
		/// <summary>取得現在是否有畫出下方線條</summary>
		public bool IsDrawLine { get { return mFlag_BtnDrawLine; } }
		#endregion

		#region Function - Constructors
		/// <summary>建構自定義的按鈕樣式</summary>
		/// <param name="brush"></param>
		public ButtonBottomBarRenderer(Brush brush) {
			mBrush = brush;
		}
		#endregion

		#region Function - Settings
		/// <summary>滑鼠進入控制項，準備觸發 Paint 事件</summary>
		public void MouseIn() { mFlag_BtnDrawLine = true; }
		/// <summary>滑鼠離開控制項，準備觸發 Paint 事件</summary>
		public void MouseOut() { mFlag_BtnDrawLine = false; }
		#endregion

		#region Function - Core
		/// <summary>繪出下方色條</summary>
		/// <param name="btn">欲繪色條的 <see cref="Button"/></param>
		/// <param name="e">繪畫事件參數</param>
		public void Paint(Button btn, PaintEventArgs e) {
			if (mFlag_BtnDrawLine) {
				Pen pen = btn.BackColor.GetPen(6);
				e.Graphics.DrawRectangle(pen, e.ClipRectangle);
				e.Graphics.FillRectangle(
					mBrush,
					new Rectangle(
						0,
						e.ClipRectangle.Height - mHoverHeight,
						e.ClipRectangle.Width,
						mHoverHeight
					)
				);
			} else
				e.Graphics.FillRectangle(Color.Silver.GetSolidBrush(), new Rectangle(0, e.ClipRectangle.Height - mNormalHeight, e.ClipRectangle.Width, mNormalHeight));
		}
		#endregion
	}
	#endregion

	#region ToolTip Renderer
	/// <summary>提供 <see cref="ToolTip"/> 渲染，將訊息視窗改為圓角矩形</summary>
	/// <remarks>目前底色沒有辦法全透明，先刷上 <see cref="SystemColors.Control"/>，未來有辦法刷透明時再改吧</remarks>
	public class ToolTipRoundRenderer {

		#region Fields
		private Font mFont;
		#endregion

		#region Constructors
		/// <summary>建構圓角矩形提示的 <see cref="ToolTip"/> 渲染</summary>
		/// <param name="font"><see cref="ToolTip"/> 訊息字體</param>
		public ToolTipRoundRenderer(Font font) {
			mFont = font;
		}

		/// <summary>建構圓角矩形提示的 <see cref="ToolTip"/> 渲染</summary>
		/// <param name="fontFamily"><see cref="ToolTip"/> 訊息字體</param>
		/// <param name="emSize"><see cref="ToolTip"/> 訊息字體大小</param>
		public ToolTipRoundRenderer(string fontFamily, float emSize) {
			mFont = new Font(fontFamily, emSize);
		}
		#endregion

		#region Event Handler
		/// <summary>承接 <see cref="ToolTip.Draw"/> 事件，繪製訊息視窗</summary>
		/// <param name="sender"><see cref="ToolTip"/></param>
		/// <param name="e"><see cref="ToolTip.Draw"/> 事件參數</param>
		public void OnDraw(object sender, DrawToolTipEventArgs e) {
			ToolTip toolTip = sender as ToolTip;
			Graphics g = e.Graphics;
			GraphicsPath path = CtDraw.GetRoundRectanglePath(e.Bounds);

			Pen pen = Color.Gray.GetPen(1);
			pen.DashStyle = DashStyle.Solid;

			g.Clear(SystemColors.Control);                          //刷底色
			g.FillPath(toolTip.BackColor.GetSolidBrush(), path);    //刷圓角矩形底色
			g.DrawPath(pen, path);                                  //刷圓角矩形邊緣
			g.DrawString(                                           //刷字
				e.ToolTipText,
				mFont,
				toolTip.ForeColor.GetSolidBrush(),
				4F,
				2.5F
			);
		}

		/// <summary>承接 <see cref="ToolTip.Popup"/> 事件，決定要繪製的範圍</summary>
		/// <param name="sender"><see cref="ToolTip"/></param>
		/// <param name="e"><see cref="ToolTip.Popup"/> 事件參數</param>
		public void OnPopup(object sender, PopupEventArgs e) {
			ToolTip toolTip = sender as ToolTip;
			Size txtSize = TextRenderer.MeasureText(toolTip.GetToolTip(e.AssociatedControl), mFont);
			txtSize.Width += 8;     //留白
			txtSize.Height += 8;    //留白
			e.ToolTipSize = txtSize;
		}
		#endregion
	}
	#endregion
}
