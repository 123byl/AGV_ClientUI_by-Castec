using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using CtLib.Library;

namespace CtLib.Module.Utility.Drawing {

	/// <summary>提供相關繪製用的靜態方法</summary>
	public static class CtDraw {

		#region Extensions
		/// <summary>將顏色描述字串轉換為相對應的 <see cref="Color"/></summary>
		/// <param name="clrStr">具有顏色描述的字串</param>
		/// <returns>相對應的 <see cref="Color"/></returns>
		public static Color ColorParser(this string clrStr) {
			Color retClr = Color.Empty;
			if (!string.IsNullOrEmpty(clrStr)) {
				KnownColor knwClr;
				int argb;
				if (Enum.TryParse(clrStr, out knwClr)) {
					retClr = Color.FromKnownColor(knwClr);
				} else if (int.TryParse(clrStr, out argb)) {
					retClr = Color.FromArgb(argb);
				} else if (clrStr.StartsWith("#") || clrStr.StartsWith("0x")) {
					string clrVal = clrStr.Replace("#", "").Replace("0x", "");
					if (clrVal.Length == 8) {
						/*-- ARGB --*/
						int a = CtConvert.ToInteger(clrVal.Substring(0, 2), NumericFormats.Hexadecimal);
						int r = CtConvert.ToInteger(clrVal.Substring(2, 2), NumericFormats.Hexadecimal);
						int g = CtConvert.ToInteger(clrVal.Substring(4, 2), NumericFormats.Hexadecimal);
						int b = CtConvert.ToInteger(clrVal.Substring(6, 2), NumericFormats.Hexadecimal);
						retClr = Color.FromArgb(a, r, g, b);
					} else if (clrVal.Length == 6) {
						/*-- RGB --*/
						int r = CtConvert.ToInteger(clrVal.Substring(0, 2), NumericFormats.Hexadecimal);
						int g = CtConvert.ToInteger(clrVal.Substring(2, 2), NumericFormats.Hexadecimal);
						int b = CtConvert.ToInteger(clrVal.Substring(4, 2), NumericFormats.Hexadecimal);
						retClr = Color.FromArgb(r, g, b);
					}
				}
			}
			return retClr;
		}

		/// <summary>變更 <see cref="Color"/> 之 Alpha 值</summary>
		/// <param name="clr">擴充主體</param>
		/// <param name="alpha">透明值， 0(全透) ~ 255(不透)</param>
		/// <returns>相對應的 <see cref="Color"/></returns>
		public static Color ToArgb(this Color clr, int alpha) {
			return Color.FromArgb(alpha, clr);
		}

		/// <summary>取得此 <see cref="Color"/> 之對應的 <see cref="SolidBrush"/></summary>
		/// <param name="clr">擴充主體</param>
		/// <returns>相對應的 <see cref="SolidBrush"/></returns>
		public static SolidBrush GetSolidBrush(this Color clr) {
			return new SolidBrush(clr);
		}

		/// <summary>取得此 <see cref="Color"/> 所對應的 <see cref="Pen"/></summary>
		/// <param name="clr">擴充主體</param>
		/// <param name="penWidth">畫筆寬度</param>
		/// <returns>對應的 <see cref="Pen"/></returns>
		public static Pen GetPen(this Color clr, float penWidth) {
			return new Pen(clr, penWidth);
		}
		#endregion

		#region Round Rectangle
		/// <summary>取得圓角矩形之繪製路徑</summary>
		/// <param name="x">起始 X 座標</param>
		/// <param name="y">起始 Y 座標</param>
		/// <param name="width">繪製範圍之寬</param>
		/// <param name="height">繪製範圍之長</param>
		/// <returns>繪製路徑</returns>
		public static GraphicsPath GetRoundRectanglePath(float x, float y, float width, float height) {
			float w = width - 1;
			float h = height - 1;

			PointF[] points = {
				new PointF(x + 2,             y),
				new PointF(x + w - 2,         y),
				new PointF(x + w - 1,     y + 1),
				new PointF(x + w,         y +2),
				new PointF(x + w,     y + h - 2),
				new PointF(x + w - 1, y + h - 1),
				new PointF(x + w - 2,     y + h),
				new PointF(x + 2,         y + h),
				new PointF(x + 1,     y + h - 1),
				new PointF(x,         y + h - 2),
				new PointF(x,             y + 2),
				new PointF(x + 1,         y + 1)
			};

			GraphicsPath path = new GraphicsPath();
			path.AddLines(points);

			return path;
		}

		/// <summary>取得圓角矩形之繪製路徑</summary>
		/// <param name="rect">繪製範圍</param>
		/// <returns>繪製路徑</returns>
		public static GraphicsPath GetRoundRectanglePath(Rectangle rect) {
			return GetRoundRectanglePath(rect.X, rect.Y, rect.Width, rect.Height);
		}

		/// <summary>取得圓角矩形之繪製路徑</summary>
		/// <param name="rect">繪製範圍</param>
		/// <returns>繪製路徑</returns>
		public static GraphicsPath GetRoundRectanglePath(RectangleF rect) {
			return GetRoundRectanglePath(rect.X, rect.Y, rect.Width, rect.Height);
		}
		#endregion
	}
}
