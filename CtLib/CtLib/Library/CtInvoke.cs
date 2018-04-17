using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CtLib.Module.Utility;

namespace CtLib.Library {
	/// <summary>
	/// 調用各元件進行 Invoke 方法
	/// <para>用於避免元件因執行緒不同而無法進行操作</para>
	/// </summary>
	/// <remarks>
	/// 關於 Invoke 請參考 <see cref="Control.BeginInvoke(Delegate)"/> 與 <seealso cref="Control.BeginInvoke(Delegate)"/>
	/// 其中，這部分有大量使用到  委派 <see cref="Delegate"/>
	/// </remarks>
	public static class CtInvoke {

		#region Version

		/// <summary>CtInvoke 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 0.0.0  Chi Sha [2007/08/15]
		///     + LibText
		///     
		/// 1.0.0  Ahern [2014/07/20]
		///     + 從舊版CtLib搬移
		///     
		/// 1.0.1  Ahern [2014/09/11]
		///     + PictureBoxTag
		///     
		/// 1.0.2  Ahern [2014/09/29]
		///     + RadioButton 相關
		///     
		/// 1.1.0  Ahern [2014/12/08]
		///     - 移除所有Try-Catch，由使用的Function自行去包
		///     - 移除多餘的 delegate
		///     
		/// 1.1.1  Ahern [2015/03/12]
		///     \ DataGridViewAddRow 改以泛型表示
		///     
		/// 1.1.2  Ahern [2015/05/12]
		///     + RichTextBoxClear
		///     + ListBoxSelectedIndex
		///     \ ListBoxAdd 改以泛型表示
		///     
		/// 1.1.3  Ahern [2015/05/25]
		///     \ 部分含有 string[] 與 List&lt;string&gt; 改以 IEnumerable&lt;string&gt; 取代
		///     
		/// 1.1.4  Ahern [2015/06/02]
		///     \ 含有 IEnumerable 之方法改以泛型方法 &lt;T&gt; 取代
		///     
		/// 1.1.5  Ahern [2015/06/03]
		///     \ 修正 string 視為 IEnumerable 問題
		/// 
		/// 1.1.6  Ahern [2015/10/31]
		///     \ 修正集合資料(Array、List、IEnumerable)可能會因 Invoke 較慢而導致集合資料更改，現已加上 .ToList()
		/// 
		/// 1.2.0  Ahern [2015/11/24]
		///		+ ToolTip 之 SetToolTip 操作
		///     
		/// 2.0.0  Ahern [2016/01/06]
		///		- Delegate
		///		+ InvokeIfNecessary (Control 擴充方法)
		///		+ Control 調用，取代舊有重複的調用方法
		///		
		/// 2.0.1  Ahern [2016/01/07]
		///		\ 區分 BeginInvokeIfNecessary 與 InvokeIfNecessary
		///		\ Clear 物件改為使用 InvokeIfNecessary 避免尚未清除就接續動作造成例外狀況
		///		
		/// 2.0.2  Ahern [2016/02/25]
		///		+ ToolStripItemFont
		///		
		/// 2.0.3  Ahern [2016/07/20]
		///		\ ListBoxAdd / ListBoxInsert 補上 delete 參數
		///		
		/// 2.0.4  Ahern [2016/08/01]
		///		\ ToolStrip.GetCurrentParent() 改以 ToolStrip.Owner 取代
		///
		/// </code></remarks>
		public static CtVersion Version { get { return new CtVersion(2, 0, 4, "2016/08/01", "Ahern Kuo"); } }

		#endregion

		#region Functions - Operations

		#region Control Extension

		/// <summary>擴充 <see cref="Control"/>，使用 <seealso cref="Control.BeginInvoke(Delegate)"/> 非同步方法執行不具任何簽章之委派</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="action">欲執行的方法</param>
		public static void BeginInvokeIfNecessary(this Control ctrl, MethodInvoker action) {
			if (ctrl.InvokeRequired) ctrl.BeginInvoke(action);
			else action();
		}

		/// <summary>擴充 <see cref="Control"/>，使用 <seealso cref="Control.Invoke(Delegate)"/> 方法執行不具任何簽章之委派</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="action">欲執行的方法</param>
		public static void InvokeIfNecessary(this Control ctrl, MethodInvoker action) {
			if (ctrl.InvokeRequired) ctrl.Invoke(action);
			else action();
		}

		/// <summary>擴充 <see cref="Control"/>，使用 <seealso cref="Control.Invoke(Delegate)"/> 取得特定方法之回傳值</summary>
		/// <typeparam name="TResult">由引數 func 委派方法所回傳的物件型態</typeparam>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="func">欲執行的方法</param>
		/// <returns>由特定方法所回傳的物件</returns>
		public static TResult InvokeIfNecessary<TResult>(this Control ctrl, Func<TResult> func) {
			if (ctrl.InvokeRequired) return (TResult)ctrl.Invoke(func);
			else return func();
		}

		#endregion

		#region Control Operations

		/// <summary>調用 <see cref="Control.BackColor"/> 以更改背景顏色</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="color">欲更改的背景顏色</param>
		public static void ControlBackColor(Control ctrl, Color color) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.BackColor = color);
		}

		/// <summary>調用 <see cref="Control.BackColor"/> 以取得其背景顏色</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>當前控制項之背景顏色</returns>
		public static Color ControlBackColor(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.BackColor);
		}

		/// <summary>調用 <see cref="Control.BackgroundImage"/> 以更改背景圖片</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="image">欲更改的背景圖片</param>
		public static void ControlBackgroundImage(Control ctrl, Image image) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.BackgroundImage = image);
		}

		/// <summary>調用 <see cref="Control.BackgroundImage"/> 以取得其背景圖片</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>當前控制項之背景圖片</returns>
		public static Image ControlBackgroundImage(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.BackgroundImage);
		}

		/// <summary>調用 <see cref="Control"/> 以將顯示順序拉至最外層</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		public static void ControlBringToFront(Control ctrl) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.BringToFront());
		}

		/// <summary>調用 <see cref="Control.Cursor"/> 以更改滑鼠停留時的游標樣式</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="cursor">欲更改的游標樣式</param>
		public static void ControlCursor(Control ctrl, Cursor cursor) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Cursor = cursor);
		}

		/// <summary>調用 <see cref="Control.Cursor"/> 以取得滑鼠停留時的游標樣式</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>當前控制項所設定的滑鼠游標樣式</returns>
		public static Cursor ControlCursor(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.Cursor);
		}

		/// <summary>調用 <see cref="Control.Enabled"/> 以更改啟用狀態</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="enabled">欲更改的啟用狀態  (<see langword="true"/>)啟用 (<see langword="false"/>)禁用</param>
		public static void ControlEnabled(Control ctrl, bool enabled) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Enabled = enabled);
		}

		/// <summary>調用 <see cref="Control.Enabled"/> 以取得其啟用狀態</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>當前控制項之啟用狀態</returns>
		public static bool ControlEnabled(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.Enabled);
		}

		/// <summary>調用 <see cref="Control.Focus"/> 以取得視窗焦點</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		public static void ControlFocus(Control ctrl) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Focus());
		}

		/// <summary>調用 <see cref="Control.Focused"/> 以取得其目前是否為焦點</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>當前控制項之焦點狀態</returns>
		public static bool ControlFocused(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.Focused);
		}

		/// <summary>調用 <see cref="Control.Font"/> 以更改字體</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="font">欲更改的字體</param>
		public static void ControlFont(Control ctrl, Font font) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Font = font);
		}

		/// <summary>調用 <see cref="Control.Font"/> 以更改字型，但不更改其他字體屬性(大小、粗體細體 等)</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="fontFamily">欲更改的字體名稱。如 "微軟正黑體"</param>
		public static void ControlFont(Control ctrl, string fontFamily) {
			ctrl.BeginInvokeIfNecessary(
				() => {
					Font tempFont = new Font(fontFamily, ctrl.Font.Size, ctrl.Font.Style, ctrl.Font.Unit, ctrl.Font.GdiCharSet, ctrl.Font.GdiVerticalFont);
					ctrl.Font = tempFont;
				}
			);
		}

		/// <summary>調用 <see cref="Control.Font"/> 以取得其字體</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>當前控制項之字體</returns>
		public static Font ControlFont(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.Font);
		}

		/// <summary>調用 <see cref="Control.ForeColor"/> 以更改前景顏色</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="color">欲更改的前景顏色</param>
		public static void ControlForeColor(Control ctrl, Color color) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.ForeColor = color);
		}

		/// <summary>調用 <see cref="Control.ForeColor"/> 以取得其前景顏色</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>當前控制項之前景顏色</returns>
		public static Color ControlForeColor(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.ForeColor);
		}

		/// <summary>調用 <see cref="Control"/> 並使用 <seealso cref="Graphics.Clear(Color)"/> 來刷為特定顏色</summary>
		/// <param name="ctrl">欲調用之Control</param>
		/// <param name="color">欲套用之顏色</param>
		public static void ControlFillColor(Control ctrl, Color color) {
			ctrl.BeginInvokeIfNecessary(
				() => {
					Graphics g = ctrl.CreateGraphics();
					g.Clear(color);
					g.Dispose();
				}
			);
		}

		/// <summary>調用 <see cref="Control"/> 並使用 <seealso cref="Graphics.DrawString(string, Font, Brush, PointF)"/> 以繪出文字於圖案正中心</summary>
		/// <param name="ctrl">欲調用之Control</param>
		/// <param name="text">欲顯示之文字</param>
		/// <param name="font">顯示文字之字體</param>
		/// <param name="brush">繪畫文字之筆刷樣式</param>
		public static void ControlDrawString(Control ctrl, string text, Font font, Brush brush) {
			ctrl.BeginInvokeIfNecessary(
				() => {
					Graphics g = ctrl.CreateGraphics();
					SizeF textSize = g.MeasureString(text, font);
					PointF paintLoc = new PointF((ctrl.Width - textSize.Width) / 2, (ctrl.Height - textSize.Height) / 2);
					g.DrawString(text, font, brush, paintLoc);
				}
			);
		}

		/// <summary>調用 <see cref="Control"/> 並使用 <seealso cref="Graphics.DrawString(string, Font, Brush, float, float)"/> 以繪出文字於特定座標</summary>
		/// <param name="ctrl">欲調用之Control</param>
		/// <param name="text">欲顯示之文字</param>
		/// <param name="font">顯示文字之字體</param>
		/// <param name="brush">繪畫文字之筆刷樣式</param>
		/// <param name="x">繪出文字之起始X座標</param>
		/// <param name="y">繪出文字之起始Y座標</param>
		public static void ControlDrawString(Control ctrl, string text, Font font, Brush brush, float x, float y) {
			ctrl.BeginInvokeIfNecessary(
				() => {
					Graphics g = ctrl.CreateGraphics();
					g.DrawString(text, font, brush, x, y);
				}
			);
		}

		/// <summary>調用 <see cref="Control.CreateGraphics"/> 以取得其 GDI+ 繪圖物件 <seealso cref="Graphics"/></summary>
		/// <param name="ctrl">欲調用之控制項</param>
		/// <returns>與控制項關聯的 <see cref="Graphics"/></returns>
		public static Graphics ControlGraphics(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.CreateGraphics());
		}

		/// <summary>調用 <see cref="Control.Height"/> 以更改高度</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="height">欲更改的高度</param>
		public static void ControlHeight(Control ctrl, int height) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Height = height);
		}

		/// <summary>調用 <see cref="Control.Height"/> 以取得其高度</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>當前控制項之高度</returns>
		public static int ControlHeight(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.Height);
		}

		/// <summary>調用 <see cref="Control.Hide"/> 以隱藏控制項</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		public static void ControlHide(Control ctrl) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Hide());
		}

		/// <summary>調用 <see cref="Control.Left"/> 以更改 X 方向位置 (錨點為左上、單位為像素)</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="left">欲更改的座標位置</param>
		public static void ControlLeft(Control ctrl, int left) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Left = left);
		}

		/// <summary>調用 <see cref="Control.Left"/> 以取得其 X 方向位置 (錨點為左上、單位為像素)</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>當前控制項之 X 座標位置</returns>
		public static int ControlLeft(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.Left);
		}

		/// <summary>調用 <see cref="Control.Location"/> 以更改位置 (錨點為左上)</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="location">欲更改的位置</param>
		public static void ControlLocation(Control ctrl, Point location) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Location = location);
		}

		/// <summary>調用 <see cref="Control.Location"/> 以更改位置 (錨點為左上)</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="x">欲更改的座標 X 量</param>
		/// <param name="y">欲更改的座標 Y 量</param>
		public static void ControlLocation(Control ctrl, int x, int y) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Location = new Point(x, y));
		}

		/// <summary>調用 <see cref="Control.Location"/> 以取得其座標位置 (錨點為左上)</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>當前控制項之座標位置</returns>
		public static Point ControlLocation(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.Location);
		}

		/// <summary>調用 <see cref="Control.Parent"/> 以更改其父項</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="parent">欲更改的父項</param>
		public static void ControlParent(Control ctrl, Control parent) {
			ctrl.InvokeIfNecessary(() => ctrl.Parent = parent);
		}

		/// <summary>調用 <see cref="Control.Parent"/> 以取得其父項</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>控制項之 <see cref="Control.Parent"/> 父層物件</returns>
		public static Control ControlParent(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.Parent);
		}

		/// <summary>調用 <see cref="Control.Size"/> 以更改大小</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="size">欲更改的大小</param>
		public static void ControlSize(Control ctrl, Size size) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Size = size);
		}

		/// <summary>調用 <see cref="Control.Size"/> 以更改大小</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="width">欲更改的寬度</param>
		/// <param name="height">欲更改的高度</param>
		public static void ControlSize(Control ctrl, int width, int height) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Size = new Size(width, height));
		}

		/// <summary>調用 <see cref="Control.Size"/> 以取得其大小</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>當前控制項之大小</returns>
		public static Size ControlSize(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.Size);
		}

		/// <summary>調用 <see cref="Control.Show"/> 以顯示控制項</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		public static void ControlShow(Control ctrl) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Show());
		}

		/// <summary>調用 <see cref="Control.Tag"/> 以更改自訂資料</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="tag">欲更改的物件</param>
		public static void ControlTag(Control ctrl, object tag) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Tag = tag);
		}

		/// <summary>調用 <see cref="Control.Tag"/> 以取得其自訂資料</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>當前控制項之自訂資料</returns>
		public static object ControlTag(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.Tag);
		}

		/// <summary>調用 <see cref="Control.Text"/> 以更改文字</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="text">欲更改的文字</param>
		public static void ControlText(Control ctrl, string text) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Text = text);
		}

		/// <summary>調用 <see cref="Control.Text"/> 以更改為多行文字</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="text">欲更改的文字</param>
		public static void ControlText(Control ctrl, IEnumerable<string> text) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Text = string.Join(Environment.NewLine, text));
		}

		/// <summary>調用 <see cref="Control.Text"/> 以附加文字</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="text">欲新增的文字</param>
		/// <param name="top">是否放置於最上方？  (<see langword="true"/>)附加於頂端 (<see langword="false"/>)附加於尾端</param>
		public static void ControlAppendText(Control ctrl, string text, bool top = false) {
			ctrl.BeginInvokeIfNecessary(
				() => {
					StringBuilder sb = new StringBuilder(ctrl.Text);
					if (top) sb.Insert(0, text);
					else sb.Append(text);
					if (sb.Length > 65536) sb.Remove(top ? 0 : 65535, sb.Length - 65536);
					ctrl.Text = sb.ToString();
				}
			);
		}

		/// <summary>調用 <see cref="Control.Text"/> 以附加多行文字</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="text">欲新增的文字</param>
		/// /// <param name="top">是否放置於最上方？  (<see langword="true"/>)附加於頂端 (<see langword="false"/>)附加於尾端</param>
		public static void ControlAppendText(Control ctrl, IEnumerable<string> text, bool top = false) {
			ctrl.BeginInvokeIfNecessary(
				() => {
					StringBuilder sb = new StringBuilder(ctrl.Text);
					string newStr = string.Join(Environment.NewLine, text);
					if (top) sb.Insert(0, newStr);
					else sb.Append(newStr);
					if (sb.Length > 65536) sb.Remove(top ? 0 : 65535, sb.Length - 65536);
					ctrl.Text = sb.ToString();
				}
			);
		}

		/// <summary>調用 <see cref="Control.Text"/> 以取得其文字</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>當前控制項之文字</returns>
		public static string ControlText(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.Text);
		}

		/// <summary>調用 <see cref="Control.Top"/> 以更改 Y 方向位置 (錨點為左上、單位為像素)</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="top">欲更改的座標位置</param>
		public static void ControlTop(Control ctrl, int top) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Top = top);
		}

		/// <summary>調用 <see cref="Control.Top"/> 以取得其 Y 方向位置 (錨點為左上、單位為像素)</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>當前控制項之 Y 座標位置</returns>
		public static int ControlTop(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.Top);
		}

		/// <summary>調用 <see cref="Control.Visible"/> 以更改顯示狀態</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="visible">欲更改的顯示狀態  (<see langword="true"/>)顯示 (<see langword="false"/>)隱藏</param>
		public static void ControlVisible(Control ctrl, bool visible) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Visible = visible);
		}

		/// <summary>調用 <see cref="Control.Visible"/> 以取得其顯示狀態</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>當前控制項之顯示狀態</returns>
		public static bool ControlVisible(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.Visible);
		}

		/// <summary>調用 <see cref="Control.Width"/> 以更改寬度</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <param name="width">欲更改的寬度</param>
		public static void ControlWidth(Control ctrl, int width) {
			ctrl.BeginInvokeIfNecessary(() => ctrl.Width = width);
		}

		/// <summary>調用 <see cref="Control.Width"/> 以取得其寬度</summary>
		/// <param name="ctrl">欲調用的控制項</param>
		/// <returns>當前控制項之寬度</returns>
		public static int ControlWidth(Control ctrl) {
			return ctrl.InvokeIfNecessary(() => ctrl.Width);
		}
		#endregion

		#region Button Operations

		/// <summary>調用 <see cref="Button"/> 以更改圖片</summary>
		/// <param name="button">欲調用之 <see cref="Button"/></param>
		/// <param name="image">欲更改之圖片</param>
		public static void ButtonImage(Button button, Image image) {
			button.BeginInvokeIfNecessary(() => button.Image = image);
		}

		/// <summary>調用 <see cref="Button"/> 以取得當前圖片</summary>
		/// <param name="button">欲調用之 <see cref="Button"/></param>
		/// <returns>當前的圖片</returns>
		public static Image ButtonImage(Button button) {
			return button.InvokeIfNecessary(() => button.Image);
		}

		#endregion

		#region ComboBox Operations

		/// <summary>調用 <see cref="ComboBox"/> 以插入選項</summary>
		/// <param name="combobox">欲調用之 <see cref="ComboBox"/></param>
		/// <param name="index">欲插入之位置索引</param>
		/// <param name="value">欲插入之物件</param>
		public static void ComboBoxInsert(ComboBox combobox, int index, object value) {
			combobox.BeginInvokeIfNecessary(() => combobox.Items.Insert(index, value));
		}

		/// <summary>調用 <see cref="ComboBox"/> 以增加選項</summary>
		/// <param name="combobox">欲調用之 <see cref="ComboBox"/></param>
		/// <param name="value">欲新增之物件</param>
		public static void ComboBoxAdd(ComboBox combobox, object value) {
			combobox.BeginInvokeIfNecessary(() => combobox.Items.Add(value));
		}

		/// <summary>調用 <see cref="ComboBox"/> 以增加多個選項</summary>
		/// <typeparam name="TValue">可添加至 <see cref="ComboBox.Items"/> 之類型。常見為 <see cref="string"/> 及 <see cref="int"/> 等實值型態</typeparam>
		/// <param name="combobox">欲調用之 <see cref="ComboBox"/></param>
		/// <param name="value">欲新增之物件</param>
		public static void ComboBoxAdd<TValue>(ComboBox combobox, IEnumerable<TValue> value) {
			combobox.BeginInvokeIfNecessary(
				() => {
					if (value is string) combobox.Items.Add(value);
					else combobox.Items.AddRange(value.Cast<object>().ToArray());
				}
			);
		}

		/// <summary>調用 <see cref="ComboBox"/> 以刪除選項</summary>
		/// <param name="combobox">欲調用之 <see cref="ComboBox"/></param>
		/// <param name="value">欲刪除之物件</param>
		public static void ComboBoxRemove(ComboBox combobox, object value) {
			combobox.BeginInvokeIfNecessary(() => combobox.Items.Remove(value));
		}

		/// <summary>調用 <see cref="ComboBox"/> 以刪除特定索引選項</summary>
		/// <param name="combobox">欲調用之 <see cref="ComboBox"/></param>
		/// <param name="index">欲刪除之索引</param>
		public static void ComboBoxRemove(ComboBox combobox, int index) {
			combobox.BeginInvokeIfNecessary(() => combobox.Items.RemoveAt(index));
		}

		/// <summary>調用 <see cref="ComboBox"/> 以清空所有選項</summary>
		/// <param name="combobox">欲調用之 <see cref="ComboBox"/></param>
		public static void ComboBoxClear(ComboBox combobox) {
			combobox.InvokeIfNecessary(() => combobox.Items.Clear());
		}

		/// <summary>調用 <see cref="ComboBox.SelectedIndex"/> 以更改目前選取項目</summary>
		/// <param name="combobox">欲調用之 <see cref="ComboBox"/></param>
		/// <param name="index">欲更改之選項索引</param>
		public static void ComboBoxSelectedIndex(ComboBox combobox, int index) {
			combobox.BeginInvokeIfNecessary(() => combobox.SelectedIndex = index);
		}

		/// <summary>調用 <see cref="ComboBox.SelectedIndex"/> 以取得目前選取項目索引</summary>
		/// <param name="combobox">欲調用之 <see cref="ComboBox"/></param>
		/// <returns>當前的 <see cref="ComboBox.SelectedIndex"/> 數值</returns>
		public static int ComboBoxSelectedIndex(ComboBox combobox) {
			return combobox.InvokeIfNecessary(() => combobox.SelectedIndex);
		}

		/// <summary>調用 <see cref="ComboBox.SelectedItem"/> 以更改目前選取項目</summary>
		/// <param name="combobox">欲調用之 <see cref="ComboBox"/></param>
		/// <param name="item">欲更改之選項物件。此物件必須已存在於集合(Items)中</param>
		public static void ComboBoxSelectedItem(ComboBox combobox, object item) {
			combobox.BeginInvokeIfNecessary(() => combobox.SelectedItem = item);
		}

		/// <summary>調用 <see cref="ComboBox.SelectedItem"/> 以取得目前選取項目</summary>
		/// <param name="combobox">欲調用之 <see cref="ComboBox"/></param>
		/// <returns>目前的選取項目</returns>
		public static object ComboBoxSelectedItem(ComboBox combobox) {
			return combobox.InvokeIfNecessary(() => combobox.SelectedItem);
		}

		#endregion

		#region CheckBox Operations

		/// <summary>調用 <see cref="CheckBox.Checked"/> 以更改勾選狀態</summary>
		/// <param name="chkbox">欲調用之 <see cref="CheckBox"/></param>
		/// <param name="value">欲更改之樣式   (<see langword="true"/>)勾選 (<see langword="false"/>)取消勾選</param>
		/// <return>勾選狀態   (<see langword="true"/>)勾選 (<see langword="false"/>)取消勾選</return>
		public static void CheckBoxChecked(CheckBox chkbox, bool value) {
			chkbox.BeginInvokeIfNecessary(() => chkbox.Checked = value);
		}

		/// <summary>調用 <see cref="CheckBox.Checked"/> 以取得勾選狀態</summary>
		/// <param name="chkbox">欲調用之 <see cref="CheckBox"/></param>
		/// <returns>勾選狀態   (<see langword="true"/>)勾選 (<see langword="false"/>)取消勾選</returns>
		public static bool CheckBoxChecked(CheckBox chkbox) {
			return chkbox.InvokeIfNecessary(() => chkbox.Checked);
		}

		#endregion

		#region DataGridView Operations

		/// <summary>>調用 <see cref="DataGridView"/> 以更改特定欄位數值</summary>
		/// <param name="dgv">欲調用之 <see cref="DataGridView"/></param>
		/// <param name="row">欲更改欄位之列數</param>
		/// <param name="col">欲更改欄位之行數</param>
		/// <param name="cell">資料格內容</param>
		public static void DataGridViewCellObject(DataGridView dgv, int row, int col, DataGridViewCell cell) {
			dgv.BeginInvokeIfNecessary(() => dgv.Rows[row].Cells[col] = cell);
		}

		/// <summary>>調用 <see cref="DataGridView"/> 以取得特定欄位</summary>
		/// <param name="dgv">欲調用之 <see cref="DataGridView"/></param>
		/// <param name="row">欲取得欄位之列數</param>
		/// <param name="col">欲取得欄位之行數</param>
		/// <returns>指定的欄位</returns>
		public static DataGridViewCell DataGridViewCellObject(DataGridView dgv, int row, int col) {
			return dgv.InvokeIfNecessary(() => dgv.Rows[row].Cells[col]);
		}

		/// <summary>>調用 <see cref="DataGridView"/> 以更改特定欄位數值</summary>
		/// <param name="dgv">欲調用之 <see cref="DataGridView"/></param>
		/// <param name="row">欲更改欄位之列數</param>
		/// <param name="col">欲更改欄位之行數</param>
		/// <param name="value">資料格內容</param>
		public static void DataGridViewCellValue(DataGridView dgv, int row, int col, object value) {
			dgv.BeginInvokeIfNecessary(() => dgv.Rows[row].Cells[col].Value = value);
		}

		/// <summary>>調用 <see cref="DataGridView"/> 以特定欄位數值</summary>
		/// <param name="dgv">欲調用之 <see cref="DataGridView"/></param>
		/// <param name="row">欲取得數值之列數</param>
		/// <param name="col">欲取得數值之行數</param>
		/// <returns>指定的欄位數值</returns>
		public static object DataGridViewCellValue(DataGridView dgv, int row, int col) {
			return dgv.InvokeIfNecessary(() => dgv.Rows[row].Cells[col].Value);
		}

		/// <summary>>調用 <see cref="DataGridView"/> 以更改特定欄位註記</summary>
		/// <param name="dgv">欲調用之 <see cref="DataGridView"/></param>
		/// <param name="row">欲更改欄位之列數</param>
		/// <param name="col">欲更改欄位之行數</param>
		/// <param name="tag">資料格內容</param>
		public static void DataGridViewCellTag(DataGridView dgv, int row, int col, object tag) {
			dgv.BeginInvokeIfNecessary(() => dgv.Rows[row].Cells[col].Tag = tag);
		}

		/// <summary>>調用 <see cref="DataGridView"/> 以取得特定欄位註記</summary>
		/// <param name="dgv">欲調用之 <see cref="DataGridView"/></param>
		/// <param name="row">欲取得註記之列數</param>
		/// <param name="col">欲取得註記之行數</param>
		/// <returns>指定欄位的註記</returns>
		public static object DataGridViewCellTag(DataGridView dgv, int row, int col) {
			return dgv.InvokeIfNecessary(() => dgv.Rows[row].Cells[col].Tag);
		}

		/// <summary>調用 <see cref="DataGridView"/> 以增加一新列</summary>
		/// <typeparam name="TValue">可添加於 <see cref="DataGridViewCell.Value"/> 之類型</typeparam>
		/// <param name="dgv">欲調用之 <see cref="DataGridView"/></param>
		/// <param name="value">各欄位數值</param>
		/// <param name="top">是否加入於最上方?  (<see langword="true"/>)於最上方加入 (<see langword="false"/>)於最後面新增一列</param>
		/// <param name="delete">如總列數大於200是否先刪除最後一列再新增?  (<see langword="true"/>)刪除最後列，避免過長 (<see langword="false"/>)直接新增</param>
		public static void DataGridViewAddRow<TValue>(DataGridView dgv, IEnumerable<TValue> value, bool top = true, bool delete = true) {
			object[] copyData = value.Cast<object>().ToArray();
			dgv.BeginInvokeIfNecessary(
				() => {
					/*-- 資料太多則先刪除最後一列 --*/
					if (delete && (dgv.Rows.Count > 200))
						dgv.Rows.RemoveAt(dgv.Rows.Count - 1);

					/*-- 組合資料 --*/
					DataGridViewRow dgvRow = new DataGridViewRow();
					dgvRow.CreateCells(dgv, copyData);

					/*-- 根據top屬性來插在最上面或最後 --*/
					if (top) dgv.Rows.Insert(0, dgvRow);
					else dgv.Rows.Add(dgvRow);

					dgv.ClearSelection();   //清除選取格數
				}
			);
		}

		/// <summary>調用 <see cref="DataGridView"/> 以增加一帶有背景顏色的新列</summary>
		/// <typeparam name="TValue">可添加於 <see cref="DataGridViewCell.Value"/> 之類型</typeparam>
		/// <param name="dgv">欲調用之 <see cref="DataGridView"/></param>
		/// <param name="value">各欄位數值</param>
		/// <param name="color">新增列之背景顏色</param>
		/// <param name="top">是否加入於最上方?  (<see langword="true"/>)於最上方加入 (<see langword="false"/>)於最後面新增一列</param>
		/// <param name="delete">如總列數大於200是否先刪除最後一列再新增?  (<see langword="true"/>)刪除最後列，避免過長 (<see langword="false"/>)直接新增</param>
		public static void DataGridViewAddRow<TValue>(DataGridView dgv, IEnumerable<TValue> value, Color color, bool top = true, bool delete = true) {
			object[] copyData = value.Cast<object>().ToArray();
			dgv.BeginInvokeIfNecessary(
				() => {
					/*-- 資料太多則先刪除最後一列 --*/
					if (delete && (dgv.Rows.Count > 200))
						dgv.Rows.RemoveAt(dgv.Rows.Count - 1);

					/*-- 組合資料 --*/
					DataGridViewRow dgvRow = new DataGridViewRow();
					dgvRow.CreateCells(dgv, copyData);

					/*-- 根據top屬性來插在最上面或最後 --*/
					if (top) {
						dgv.Rows.Insert(0, dgvRow);
						dgv.Rows[0].DefaultCellStyle.BackColor = color;
					} else {
						dgv.Rows.Add(dgvRow);
						dgv.Rows[dgv.Rows.Count - 1].DefaultCellStyle.BackColor = color;
					}
					dgv.ClearSelection();   //清除選取格數
				}
			);
		}

		/// <summary>調用 <see cref="DataGridView"/> 以增加一新列</summary>
		/// <param name="dgv">欲調用之 <see cref="DataGridView"/></param>
		/// <param name="value">各欄位數值</param>
		/// <param name="top">是否加入於最上方?  (<see langword="true"/>)於最上方加入 (<see langword="false"/>)於最後面新增一列</param>
		/// <param name="delete">如總列數大於200是否先刪除最後一列再新增?  (<see langword="true"/>)刪除最後列，避免過長 (<see langword="false"/>)直接新增</param>
		public static void DataGridViewAddRow(DataGridView dgv, object value, bool top = true, bool delete = true) {
			dgv.BeginInvokeIfNecessary(
				() => {
					/*-- 資料太多則先刪除最後一列 --*/
					if (delete && (dgv.Rows.Count > 200))
						dgv.Rows.RemoveAt(dgv.Rows.Count - 1);

					/*-- 組合資料 --*/
					DataGridViewRow dgvRow = new DataGridViewRow();
					dgvRow.CreateCells(dgv, value);

					/*-- 根據top屬性來插在最上面或最後 --*/
					if (top) dgv.Rows.Insert(0, dgvRow);
					else dgv.Rows.Add(dgvRow);

					dgv.ClearSelection();   //清除選取格數
				}
			);
		}

		/// <summary>調用 <see cref="DataGridView"/> 以清除目前所有資料</summary>
		/// <param name="dgv">欲調用的 <see cref="DataGridView"/></param>
		public static void DataGridViewClear(DataGridView dgv) {
			dgv.InvokeIfNecessary(() => dgv.Rows.Clear());
		}

		/// <summary>調用 <see cref="DataGridView"/> 以取得當前所有選取的 <seealso cref="DataGridViewRow"/></summary>
		/// <param name="dgv">欲調用之 <see cref="DataGridView"/></param>
		/// <returns>當前選取的列集合</returns>
		public static IEnumerable<DataGridViewRow> DataGridViewSelectedRow(DataGridView dgv) {
			return dgv.InvokeIfNecessary(() => dgv.SelectedRows.Cast<DataGridViewRow>());
		}

		/// <summary>調用 <see cref="DataGridView"/> 以取得當前所有選取的 <seealso cref="DataGridViewCell"/></summary>
		/// <param name="dgv">欲調用之 <see cref="DataGridView"/></param>
		/// <returns>當前選取的欄位集合</returns>
		public static IEnumerable<DataGridViewCell> DataGridViewSelectedCell(DataGridView dgv) {
			return dgv.InvokeIfNecessary(() => dgv.SelectedCells.Cast<DataGridViewCell>());
		}

		#endregion

		#region DateTimePicker Operations

		/// <summary>調用 <see cref="DateTimePicker.Checked"/> 以更改勾選狀態</summary>
		/// <param name="timePicker">欲調用之 <see cref="DateTimePicker"/></param>
		/// <param name="check">勾選?  (<see langword="true"/>)勾選 (<see langword="false"/>)取消勾選</param>
		public static void DateTimePickerChecked(DateTimePicker timePicker, bool check) {
			timePicker.BeginInvokeIfNecessary(() => timePicker.Checked = check);
		}

		/// <summary>調用 <see cref="DateTimePicker.Checked"/> 以取得勾選狀態</summary>
		/// <param name="timePicker">欲調用之 <see cref="DateTimePicker"/></param>
		/// <returns>當前勾選狀態</returns>
		public static bool DateTimePickerChecked(DateTimePicker timePicker) {
			return timePicker.InvokeIfNecessary(() => timePicker.Checked);
		}

		/// <summary>調用 <see cref="DateTimePicker.Value"/> 以更改當前日期</summary>
		/// <param name="timePicker">欲調用之 <see cref="DateTimePicker"/></param>
		/// <param name="value">欲更改之日期</param>
		public static void DateTimePickerValue(DateTimePicker timePicker, DateTime value) {
			timePicker.BeginInvokeIfNecessary(() => timePicker.Value = value);
		}

		/// <summary>調用 <see cref="DateTimePicker.Value"/> 以更改當前日期</summary>
		/// <param name="timePicker">欲調用之 <see cref="DateTimePicker"/></param>
		/// <returns>當前選取的時間</returns>
		public static DateTime DateTimePickerValue(DateTimePicker timePicker) {
			return timePicker.InvokeIfNecessary(() => timePicker.Value);
		}

		#endregion

		#region Form Operations

		/// <summary>調用 <see cref="Form.Close"/> 以關閉表單</summary>
		/// <param name="form">欲調用之 <see cref="Form"/></param>
		/// <param name="owner">是否連擁有者都一起關閉?  (<see langword="true"/>)一起關閉 (<see langword="false"/>)單純關閉此表單</param>
		public static void FormClose(Form form, bool owner = false) {
			if (owner && form.Owner != null) {
				form.Owner.InvokeIfNecessary(() => form.Owner.Close());
			} else {
				form.InvokeIfNecessary(() => form.Close());
			}
		}

		#endregion

		#region GroupBox Operations

		#endregion

		#region Label Operations

		#endregion

		#region ListBox Operations

		/// <summary>調用 <see cref="ListBox"/> 以插入選項</summary>
		/// <param name="listbox">欲調用之 <see cref="ListBox"/></param>
		/// <param name="index">欲插入之位置索引</param>
		/// <param name="value">欲插入之物件</param>
		/// <param name="delete">如果資料行數已超過 200 行，是否先刪掉最後一筆</param>
		public static void ListBoxInsert(ListBox listbox, int index, object value, bool delete = false) {
			listbox.BeginInvokeIfNecessary(
				() => {
					if (delete && listbox.Items.Count > 200)
						listbox.Items.RemoveAt(listbox.Items.Count - 1);
					listbox.Items.Insert(index, value);
				}
			);
		}

		/// <summary>調用 <see cref="ListBox"/> 以增加選項</summary>
		/// <param name="listbox">欲調用之 <see cref="ListBox"/></param>
		/// <param name="value">欲新增之物件</param>
		/// <param name="delete">如果資料行數已超過 200 行，是否先刪掉最後一筆</param>
		public static void ListBoxAdd(ListBox listbox, object value, bool delete = false) {
			listbox.BeginInvokeIfNecessary(
				() => {
					if (delete && listbox.Items.Count > 200)
						listbox.Items.RemoveAt(listbox.Items.Count - 1);
					listbox.Items.Add(value);
				}
			);
		}

		/// <summary>調用 <see cref="ListBox"/> 以增加選項</summary>
		/// <typeparam name="TValue">可添加於 <see cref="ListBox.Items"/> 之類型</typeparam>
		/// <param name="listbox">欲調用之 <see cref="ListBox"/></param>
		/// <param name="value">欲新增之物件</param>
		/// <param name="delete">如果資料行數已超過 200 行，是否先刪掉最後一筆</param>
		public static void ListBoxAdd<TValue>(ListBox listbox, IEnumerable<TValue> value, bool delete = false) {
			listbox.BeginInvokeIfNecessary(
				() => {
					if (delete && listbox.Items.Count > 200)
						listbox.Items.RemoveAt(listbox.Items.Count - 1);

					if (value is string) listbox.Items.Add(value);
					else listbox.Items.AddRange(value.Cast<object>().ToArray());
				}
			);
		}

		/// <summary>調用 <see cref="ListBox"/> 以刪除選項</summary>
		/// <param name="listbox">欲調用之 <see cref="ListBox"/></param>
		/// <param name="value">欲刪除之物件</param>
		public static void ListBoxRemove(ListBox listbox, object value) {
			listbox.BeginInvokeIfNecessary(() => listbox.Items.Remove(value));
		}

		/// <summary>調用 <see cref="ListBox"/> 以刪除特定索引選項</summary>
		/// <param name="listbox">欲調用之 <see cref="ListBox"/></param>
		/// <param name="index">欲刪除之索引</param>
		public static void ListBoxRemove(ListBox listbox, int index) {
			listbox.BeginInvokeIfNecessary(() => listbox.Items.RemoveAt(index));
		}

		/// <summary>調用 <see cref="ListBox"/> 以清空所有選項</summary>
		/// <param name="listbox">欲調用之 <see cref="ListBox"/></param>
		public static void ListBoxClear(ListBox listbox) {
			listbox.InvokeIfNecessary(() => listbox.Items.Clear());
		}

		/// <summary>調用 <see cref="ListBox"/> 以調整選取選項索引</summary>
		/// <param name="listbox">欲調用之 <see cref="ListBox"/></param>
		/// <param name="index">欲調整之索引</param>
		public static void ListBoxSelectedIndex(ListBox listbox, int index) {
			listbox.BeginInvokeIfNecessary(() => { if (listbox.Items.Count > 0) listbox.SelectedIndex = index; });
		}

		#endregion

		#region ListView Operations
		/// <summary>調用 <see cref="ListView"/> 以插入項目</summary>
		/// <param name="listview">欲調用之 <see cref="ListView"/></param>
		/// <param name="index">欲插入之物件索引</param>
		/// <param name="value">欲插入之物件</param>
		public static void ListViewInsert(ListView listview, int index, ListViewItem value) {
			listview.BeginInvokeIfNecessary(() => listview.Items.Insert(index, value));
		}

		/// <summary>調用 <see cref="ListView"/> 以增加項目</summary>
		/// <param name="listview">欲調用之 <see cref="ListView"/></param>
		/// <param name="value">欲新增之物件</param>
		public static void ListViewAdd(ListView listview, ListViewItem value) {
			listview.BeginInvokeIfNecessary(() => listview.Items.Add(value));
		}

		/// <summary>調用 <see cref="ListView"/> 以移除項目</summary>
		/// <param name="listview">欲調用之 <see cref="ListView"/></param>
		/// <param name="value">欲移除之物件</param>
		public static void ListViewRemove(ListView listview, ListViewItem value) {
			listview.BeginInvokeIfNecessary(() => listview.Items.Remove(value));
		}

		private delegate void DlgListViewRemoveAt(ListView listview, int index);
		/// <summary>調用 <see cref="ListView"/> 以移除項目</summary>
		/// <param name="listview">欲調用之 <see cref="ListView"/></param>
		/// <param name="index">欲移除之物件索引</param>
		public static void ListViewRemove(ListView listview, int index) {
			listview.BeginInvokeIfNecessary(() => listview.Items.RemoveAt(index));
		}

		/// <summary>調用 <see cref="ListView.Sort"/> 以排序項目</summary>
		/// <param name="listview">欲調用之 <see cref="ListView"/></param>
		public static void ListViewSort(ListView listview) {
			listview.BeginInvokeIfNecessary(() => listview.Sort());
		}

		/// <summary>調用 <see cref="ListView"/> 以取得當前選取的 <seealso cref="ListViewItem"/></summary>
		/// <param name="listview">欲調用的 <see cref="ListView"/></param>
		/// <returns>當前所有選取的項目</returns>
		public static IEnumerable<ListViewItem> ListViewSelectedItem(ListView listview) {
			return listview.InvokeIfNecessary(() => listview.SelectedItems.Cast<ListViewItem>());
		}

		#endregion

		#region NumericUpDown Operations

		/// <summary>調用 <see cref="NumericUpDown.Value"/> 以更改當前數值</summary>
		/// <param name="numeric">欲調用之 <see cref="NumericUpDown"/></param>
		/// <param name="value">欲更改之數值</param>
		public static void NumericUpDownValue(NumericUpDown numeric, decimal value) {
			numeric.BeginInvokeIfNecessary(() => numeric.Value = value);
		}

		/// <summary>調用 <see cref="NumericUpDown.Value"/> 以取得當前數值</summary>
		/// <param name="numeric">欲調用之 <see cref="NumericUpDown"/></param>
		/// <returns>當前數值</returns>
		public static decimal NumericUpDownValue(NumericUpDown numeric) {
			return numeric.InvokeIfNecessary(() => numeric.Value);
		}

		/// <summary>調用 <see cref="NumericUpDown.Maximum"/> 以更改最大範圍</summary>
		/// <param name="numeric">欲調用之 <see cref="NumericUpDown"/></param>
		/// <param name="maximum">欲更改之數值</param>
		public static void NumericUpDownMaximum(NumericUpDown numeric, decimal maximum) {
			numeric.BeginInvokeIfNecessary(() => numeric.Maximum = maximum);
		}

		/// <summary>調用 <see cref="NumericUpDown.Maximum"/> 以取得最大範圍</summary>
		/// <param name="numeric">欲調用之 <see cref="NumericUpDown"/></param>
		/// <returns>當前最大範圍</returns>
		public static decimal NumericUpDownMaximum(NumericUpDown numeric) {
			return numeric.InvokeIfNecessary(() => numeric.Maximum);
		}

		/// <summary>調用 <see cref="NumericUpDown.Minimum"/> 以更改最小範圍</summary>
		/// <param name="numeric">欲調用之 <see cref="NumericUpDown"/></param>
		/// <param name="minimum">欲更改之數值</param>
		public static void NumericUpDownMinimum(NumericUpDown numeric, decimal minimum) {
			numeric.BeginInvokeIfNecessary(() => numeric.Minimum = minimum);
		}

		/// <summary>調用 <see cref="NumericUpDown.Minimum"/> 以取得最小範圍</summary>
		/// <param name="numeric">欲調用之 <see cref="NumericUpDown"/></param>
		/// <returns>當前最小範圍</returns>
		public static decimal NumericUpDownMinimum(NumericUpDown numeric) {
			return numeric.InvokeIfNecessary(() => numeric.Minimum);
		}

		#endregion

		#region Panel Operations

		/// <summary>調用 <see cref="Panel"/> 以加入新的介面，屆時將會依附在 Panel 裡</summary>
		/// <param name="panel">欲調用之 <see cref="Panel"/></param>
		/// <param name="form">欲依附的表單或控制項</param>
		/// <param name="dock">停駐方式</param>
		/// <param name="show">依附後是否直接顯示</param>
		public static void PanelAddControl(Panel panel, Control form, DockStyle dock = DockStyle.Fill, bool show = true) {
			panel.BeginInvokeIfNecessary(
				() => {
					panel.Controls.Add(form);
					form.Dock = dock;
					if (show) form.Show();
				}
			);
		}

		#endregion

		#region PictureBox Operations

		/// <summary>調用 <see cref="PictureBox.Image"/> 以更改其顯示圖片</summary>
		/// <param name="picbox">欲調用之 <see cref="PictureBox"/></param>
		/// <param name="img">欲更換之圖片</param>
		public static void PictureBoxImage(PictureBox picbox, Image img) {
			picbox.BeginInvokeIfNecessary(() => picbox.Image = img);
		}

		/// <summary>調用 <see cref="PictureBox.Image"/> 以取得其顯示圖片</summary>
		/// <param name="picbox">欲調用之 <see cref="PictureBox"/></param>
		/// <returns>當前圖片</returns>
		public static Image PictureBoxImage(PictureBox picbox) {
			return picbox.InvokeIfNecessary(() => picbox.Image);
		}

		#endregion

		#region ProgressBar Operations

		/// <summary>調用 <see cref="ProgressBar.Value"/> 以更改當前數值</summary>
		/// <param name="progress">欲調用之 <see cref="ProgressBar"/></param>
		/// <param name="value">欲更改之數值</param>
		public static void ProgressBarValue(ProgressBar progress, int value) {
			progress.BeginInvokeIfNecessary(() => progress.Value = value);
		}

		/// <summary>調用 <see cref="ProgressBar.Value"/> 以取得當前數值</summary>
		/// <param name="progress">欲調用之 <see cref="ProgressBar"/></param>
		/// <returns>當前數值</returns>
		public static int ProgressBarValue(ProgressBar progress) {
			return progress.InvokeIfNecessary(() => progress.Value);
		}

		/// <summary>調用 <see cref="ProgressBar.Maximum"/> 以更改數值最大範圍</summary>
		/// <param name="progress">欲調用之 <see cref="ProgressBar"/></param>
		/// <param name="max">欲更改之數值</param>
		public static void ProgressBarMaximum(ProgressBar progress, int max) {
			progress.BeginInvokeIfNecessary(() => progress.Maximum = max);
		}

		/// <summary>調用 <see cref="ProgressBar.Maximum"/> 以取得數值最大範圍</summary>
		/// <param name="progress">欲調用之 <see cref="ProgressBar"/></param>
		/// <returns>最大範圍</returns>
		public static int ProgressBarMaximum(ProgressBar progress) {
			return progress.InvokeIfNecessary(() => progress.Maximum);
		}

		/// <summary>調用 <see cref="ProgressBar.Minimum"/> 以更改數值最小範圍</summary>
		/// <param name="progress">欲調用之 <see cref="ProgressBar"/></param>
		/// <param name="min">欲更改之數值</param>
		public static void ProgressBarMinimum(ProgressBar progress, int min) {
			progress.BeginInvokeIfNecessary(() => progress.Minimum = min);
		}

		/// <summary>調用 <see cref="ProgressBar.Minimum"/> 以取得數值最小範圍</summary>
		/// <param name="progress">欲調用之 <see cref="ProgressBar"/></param>
		/// <returns>最小範圍</returns>
		public static int ProgressBarMinimum(ProgressBar progress) {
			return progress.InvokeIfNecessary(() => progress.Minimum);
		}

		/// <summary>調用 <see cref="ProgressBar"/> 以更改最大與最小值範圍</summary>
		/// <param name="progress">欲調用之 <see cref="ProgressBar"/></param>
		/// <param name="min">最小值</param>
		/// <param name="max">最大值</param>
		public static void ProgressBarRange(ProgressBar progress, int min, int max) {
			progress.BeginInvokeIfNecessary(
				() => {
					progress.Minimum = min;
					progress.Maximum = max;
				}
			);
		}

		/// <summary>調用 <see cref="ProgressBar"/> 以取得最大與最小值範圍</summary>
		/// <param name="progress">欲調用之 <see cref="ProgressBar"/></param>
		/// <returns>最大與最小範圍， [0]Maximum [1]Minimum</returns>
		public static int[] ProgressBarRange(ProgressBar progress) {
			return progress.InvokeIfNecessary(() => new int[] { progress.Maximum, progress.Minimum });
		}

		/// <summary>調用 <see cref="ProgressBar.Style"/> 以更改當前樣式</summary>
		/// <param name="progress">欲調用之 <see cref="ProgressBar"/></param>
		/// <param name="style">欲更改之樣式</param>
		public static void ProgressBarStyle(ProgressBar progress, ProgressBarStyle style) {
			progress.BeginInvokeIfNecessary(() => progress.Style = style);
		}

		/// <summary>調用 <see cref="ProgressBar.Style"/> 以取得當前樣式</summary>
		/// <param name="progress">欲調用之 <see cref="ProgressBar"/></param>
		/// <returns>當前進度條樣式</returns>
		public static ProgressBarStyle ProgressBarStyle(ProgressBar progress) {
			return progress.InvokeIfNecessary(() => progress.Style);
		}

		#endregion

		#region RadioButton Operations

		/// <summary>調用 <see cref="RadioButton.Checked"/> 勾選狀態</summary>
		/// <param name="radio">欲調用之 <see cref="RadioButton"/></param>
		/// <param name="check">勾選?  (<see langword="true"/>)勾選 (<see langword="false"/>)取消勾選</param>
		public static void RadioButtonChecked(RadioButton radio, bool check) {
			radio.BeginInvokeIfNecessary(() => radio.Checked = check);
		}

		/// <summary>調用 <see cref="RadioButton.Checked"/> 以取得勾選狀態</summary>
		/// <param name="radio">欲調用之 <see cref="RadioButton"/></param>
		/// <returns>當前勾選狀態</returns>
		public static bool RadioButtonChecked(RadioButton radio) {
			return radio.InvokeIfNecessary(() => radio.Checked);
		}

		#endregion

		#region RichTextBox Operations

		/// <summary>調用 <see cref="RichTextBox.SelectedText"/> 以附加帶有顏色的自串至內容區</summary>
		/// <param name="richTxt">欲調用之 <see cref="RichTextBox"/></param>
		/// <param name="text">欲附加的文字</param>
		/// <param name="color">字體顏色</param>
		/// <param name="top">是否附加於最上方？ (<see langword="true"/>)最上方  (<see langword="false"/>)尾端</param>
		public static void RichTextBoxAppendText(RichTextBox richTxt, string text, Color color, bool top = false) {
			richTxt.BeginInvokeIfNecessary(
				() => {
					richTxt.SelectionStart = top ? 0 : richTxt.Text.Length;
					richTxt.SelectionColor = color;
					richTxt.SelectedText = text;
				}
			);
		}

		#endregion

		#region TabControl Operations

		/// <summary>調用 <see cref="TabControl"/> 以新增頁面至最後端</summary>
		/// <param name="tab">欲調用之 <see cref="TabControl"/></param>
		/// <param name="page">欲加入的頁面</param>
		public static void TabControlAdd(TabControl tab, TabPage page) {
			tab.BeginInvokeIfNecessary(() => tab.TabPages.Add(page));
		}

		/// <summary>調用 <see cref="TabControl"/> 以插入頁面至特定索引</summary>
		/// <param name="tab">欲調用之 <see cref="TabControl"/></param>
		/// <param name="index">欲插入的索引</param>
		/// <param name="page">欲加入的頁面</param>
		public static void TabControlInsert(TabControl tab, int index, TabPage page) {
			tab.BeginInvokeIfNecessary(() => tab.TabPages.Insert(index, page));
		}

		/// <summary>調用 <see cref="TabControl"/> 以移除頁面</summary>
		/// <param name="tab">欲調用之 <see cref="TabControl"/></param>
		/// <param name="page">欲移除的頁面</param>
		public static void TabControlRemove(TabControl tab, TabPage page) {
			tab.BeginInvokeIfNecessary(() => tab.TabPages.Remove(page));
		}

		/// <summary>調用 <see cref="TabControl"/> 以移除特定索引的頁面</summary>
		/// <param name="tab">欲調用之 <see cref="TabControl"/></param>
		/// <param name="index">欲移除的頁面索引</param>
		public static void TabControlRemove(TabControl tab, int index) {
			tab.BeginInvokeIfNecessary(() => tab.TabPages.RemoveAt(index));
		}

		/// <summary>調用 <see cref="TabControl.SelectedIndex"/> 以更改顯示頁面索引</summary>
		/// <param name="tab">欲調用之 <see cref="TabControl"/></param>
		/// <param name="value">欲更改之頁面索引</param>
		public static void TabControlSelectedIndex(TabControl tab, int value) {
			tab.BeginInvokeIfNecessary(() => tab.SelectedIndex = value);
		}

		/// <summary>調用 <see cref="TabControl.SelectedIndex"/> 以取得當前顯示的頁面索引</summary>
		/// <param name="tab">欲調用之 <see cref="TabControl"/></param>
		/// <returns>當前選取的頁面索引</returns>
		public static int TabControlSelectedIndex(TabControl tab) {
			return tab.InvokeIfNecessary(() => tab.SelectedIndex);
		}

		/// <summary>調用 <see cref="TabControl.SelectedTab"/> 以更改選擇頁面</summary>
		/// <param name="tab">欲調用之 <see cref="TabControl"/></param>
		/// <param name="page">欲更改之頁面</param>
		public static void TabControlSelectedTab(TabControl tab, TabPage page) {
			tab.BeginInvokeIfNecessary(() => tab.SelectedTab = page);
		}

		/// <summary>調用 <see cref="TabControl.SelectedTab"/> 以取得當前選擇頁面</summary>
		/// <param name="tab">欲調用之 <see cref="TabControl"/></param>
		/// <returns>當前選取的頁面</returns>
		public static TabPage TabControlSelectedTab(TabControl tab) {
			return tab.InvokeIfNecessary(() => tab.SelectedTab);
		}

		#endregion

		#region TextBox Operations

		#endregion

		#region ToolStrip & ToolStripMenuItem Operations

		/* MenuStrip → 系統選單(子類別僅有選單，即 ToolStripMenuItem)，如一般程式上的檔案、檢視、幫助等
           ToolStrip → 工具欄，有眾多子元件，如Button、Spliter、TextBox等，也就是一般程式上面可移動、可更改內容的工具列
           MenuStrip 是繼承於 ToolStrip，故 MenuStrip 可以直接丟入以下的 ToolStrip Function 
           其子元件均是繼承於 ToolStripItem，故不管是 ToolStripMenuItem、ToolStripButton都可以用以下的 ToolStripItem Function */


		/// <summary>調用 <see cref="ToolStripItem.Enabled"/> 啟用選項(Enabled)</summary>
		/// <param name="item">欲調用之 <see cref="ToolStripItem"/></param>
		/// <param name="enabled">啟用?  (<see langword="true"/>)啟用 (<see langword="false"/>)禁用(變灰色)</param>
		public static void ToolStripItemEnable(ToolStripItem item, bool enabled) {
			item.Owner.BeginInvokeIfNecessary(() => item.Enabled = enabled);
		}

		/// <summary>調用 <see cref="ToolStripItem.Enabled"/> 以取得當前啟用(Enabled)狀態</summary>
		/// <param name="item">欲調用之 <see cref="ToolStripItem"/></param>
		/// <returns>當前 Enabled 狀態</returns>
		public static bool ToolStripItemEnable(ToolStripItem item) {
			return item.Owner.InvokeIfNecessary(() => item.Enabled);
		}

		/// <summary>調用 <see cref="ToolStripItem.Text"/>Item 以更改顯示字體</summary>
		/// <param name="item">欲調用之 <see cref="ToolStripItem"/></param>
		/// <param name="font">欲更換之字型</param>
		public static void ToolStripItemFont(ToolStripItem item, Font font) {
			item.Owner.BeginInvokeIfNecessary(() => item.Font = font);
		}

		/// <summary>調用 <see cref="ToolStripItem.Text"/>Item 以取得顯示字體</summary>
		/// <param name="item">欲調用之 <see cref="ToolStripItem"/></param>
		/// <returns>當前顯示的字型</returns>
		public static Font ToolStripItemFont(ToolStripItem item) {
			return item.Owner.InvokeIfNecessary(() => item.Font);
		}

		/// <summary>調用 <see cref="ToolStripItem.Visible"/> 可視選項(Visible)</summary>
		/// <param name="item">欲調用之 <see cref="ToolStripItem"/></param>
		/// <param name="visible">可見?  (<see langword="true"/>)可見 (<see langword="false"/>)不可見</param>
		public static void ToolStripItemVisible(ToolStripItem item, bool visible) {
			item.Owner.BeginInvokeIfNecessary(() => item.Visible = visible);
		}

		/// <summary>調用 <see cref="ToolStripItem.Visible"/> 以取得當前的可視(Visible)狀態</summary>
		/// <param name="item">欲調用之 <see cref="ToolStripItem"/></param>
		/// <returns>當前是否可見(Visible)</returns>
		public static bool ToolStripItemVisible(ToolStripItem item) {
			return item.Owner.InvokeIfNecessary(() => item.Visible);
		}

		/// <summary>調用 <see cref="ToolStripMenuItem.Checked"/> 並更改勾選狀態 (此功能僅支援 ToolStripMenuItem，其他如 ToolStripItem、ToolStripButton 等並不支援)</summary>
		/// <param name="item">欲調用之ToolStripMenuItem</param>
		/// <param name="check">是否勾選?  (<see langword="true"/>)勾選 (<see langword="false"/>)取消勾選</param>
		public static void ToolStripItemChecked(ToolStripMenuItem item, bool check) {
			item.Owner.BeginInvokeIfNecessary(() => item.Checked = check);
		}

		/// <summary>調用 <see cref="ToolStripMenuItem.Checked"/> 以取得勾選狀態 (此功能僅支援 ToolStripMenuItem，其他如 ToolStripItem、ToolStripButton 等並不支援)</summary>
		/// <param name="item">欲調用之ToolStripMenuItem</param>
		/// <returns>當前的勾選(Checked)狀態</returns>
		public static bool ToolStripItemChecked(ToolStripMenuItem item) {
			return item.Owner.InvokeIfNecessary(() => item.Checked);
		}

		/// <summary>調用 <see cref="ToolStripItem.Text"/>Item 以更改顯示文字</summary>
		/// <param name="item">欲調用之 <see cref="ToolStripItem"/></param>
		/// <param name="text">欲更換之文字</param>
		public static void ToolStripItemText(ToolStripItem item, string text) {
			item.Owner.BeginInvokeIfNecessary(() => item.Text = text);
		}

		/// <summary>調用 <see cref="ToolStripItem.Text"/>Item 以取得顯示文字</summary>
		/// <param name="item">欲調用之 <see cref="ToolStripItem"/></param>
		/// <returns>當前顯示的文字</returns>
		public static string ToolStripItemText(ToolStripItem item) {
			return item.Owner.InvokeIfNecessary(() => item.Text);
		}

		/// <summary>調用 <see cref="ToolStripItem.Tag"/> 以更改顯示文字</summary>
		/// <param name="item">欲調用之 <see cref="ToolStripItem"/></param>
		/// <param name="tag">欲更換之文字</param>
		public static void ToolStripItemTag(ToolStripItem item, object tag) {
			item.Owner.BeginInvokeIfNecessary(() => item.Tag = tag);
		}

		/// <summary>調用 <see cref="ToolStripItem.Tag"/> 以取得物件自訂註記</summary>
		/// <param name="item">欲調用之 <see cref="ToolStripItem"/></param>
		/// <returns>物件的自訂註記</returns>
		public static object ToolStripItemTag(ToolStripItem item) {
			return item.Owner.InvokeIfNecessary(() => item.Tag);
		}

		/// <summary>調用 <see cref="ToolStripItem.Image"/> 以更改顯示圖片</summary>
		/// <param name="item">欲調用之 <see cref="ToolStripItem"/></param>
		/// <param name="img">欲更換之文字</param>
		public static void ToolStripItemImage(ToolStripItem item, Image img) {
			item.Owner.BeginInvokeIfNecessary(() => item.Image = img);
		}

		/// <summary>調用 <see cref="ToolStripItem.Image"/> 以更改顯示圖片</summary>
		/// <param name="item">欲調用之 <see cref="ToolStripItem"/></param>
		/// <returns>顯示圖片</returns>
		public static Image ToolStripItemImage(ToolStripItem item) {
			return item.Owner.InvokeIfNecessary(() => item.Image);
		}

		#endregion

		#region ToolTip Operations

		/// <summary>調用 <see cref="ToolTip"/> 與 <seealso cref="Control"/> 以更改提示文字</summary>
		/// <param name="toolTip">欲調用之 <see cref="ToolTip"/></param>
		/// <param name="ctrl">欲提示之控制項</param>
		/// <param name="tip">提示文字</param>
		public static void ToolTipSetTip(ToolTip toolTip, Control ctrl, string tip) {
			ctrl.BeginInvokeIfNecessary(() => toolTip.SetToolTip(ctrl, tip));
		}

		/// <summary>調用 <see cref="ToolTip"/> 與 <seealso cref="Control"/> 以取得提示文字</summary>
		/// <param name="toolTip">欲調用之 <see cref="ToolTip"/></param>
		/// <param name="ctrl">欲取得提示的控制項</param>
		/// <returns>提示文字</returns>
		public static string ToolTipGetTip(ToolTip toolTip, Control ctrl) {
			return ctrl.InvokeIfNecessary(() => toolTip.GetToolTip(ctrl));
		}

		#endregion

		#region TrackBar Operations

		/// <summary>調用 <see cref="TrackBar.Value"/> 以更改當前數值</summary>
		/// <param name="track">欲調用之 <see cref="TrackBar"/></param>
		/// <param name="value">欲更改之數值</param>
		public static void TrackBarValue(TrackBar track, int value) {
			track.BeginInvokeIfNecessary(() => track.Value = value);
		}

		/// <summary>調用 <see cref="TrackBar.Value"/> 以更改當前數值</summary>
		/// <param name="track">欲調用之 <see cref="TrackBar"/></param>
		/// <returns>當前數值</returns>
		public static int TrackBarValue(TrackBar track) {
			return track.InvokeIfNecessary(() => track.Value);
		}

		#endregion

		#endregion

	}
}
