using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Ace.HSVision.Client.ImageDisplay;
using Ace.HSVision.Server.Integration;
using Ace.HSVision.Server.Integration.ImageSources;
using Ace.HSVision.Server.Integration.Tools;
using Ace.HSVision.Server.Parameters;
using Ace.HSVision.Server.Tools;

using CtLib.Library;
namespace CtLib.Module.Adept {

	#region Eumerations
	/// <summary>可繪製的重疊圖像 (OverlayMark) 形狀</summary>
	public enum OverlayMarker : byte {
		/// <summary>Invalid</summary>
		Invalid,
		/// <summary>Arc Shape</summary>
		Arc,
		/// <summary>Line with two points</summary>
		Line,
		/// <summary>Two axies arrow</summary>
		Locator,
		/// <summary>Point</summary>
		Point,
		/// <summary>Rectangle shape</summary>
		Rectangle,
		/// <summary>String</summary>
		Text
	}
	#endregion

	#region Support Class

	#region Base Class
	/// <summary>重疊圖像物件基底</summary>
	public abstract class OverlayMarkerObjectBase {

		#region Fields
		/// <summary>GDI+ 繪製形狀的畫筆顏色</summary>
		protected Color mPenColor;
		/// <summary>GDI+ 繪製形狀的畫筆寬度</summary>
		protected float mPenWidth = 0F;
		/// <summary>相對於 <see cref="MarkerPenWidth.Thin"/> 之像素寬度</summary>
		protected float mBordThinWidth = 2F;
		#endregion

		#region Properties
		/// <summary>取得 GDI+ 繪製用畫筆</summary>
		public Pen DrawPen {
			get {
				Pen pen = new Pen(mPenColor, mPenWidth);
				pen.StartCap = LineCap.SquareAnchor;
				pen.EndCap = LineCap.SquareAnchor;
				return pen;
			}
		}
		/// <summary>取得 GDI+ 繪製用筆刷顏色</summary>
		public Brush DrawBrush { get { return new SolidBrush(mPenColor); } }
		/// <summary>取得此重疊圖像的旋轉角度，以 ACE 內的 Offset 屬性為準</summary>
		public float Rotation { get; protected set; }
		/// <summary>取得此重疊圖像所代表的形狀</summary>
		public OverlayMarker Shape { get; protected set; }
		#endregion

		#region Functions
		/// <summary>設定 GDI+ 之筆刷顏色</summary>
		/// <param name="color">欲更改的顏色</param>
		public virtual void SetColor(Color color) {
			mPenColor = color;
		}

		/// <summary>設定 GDI+ 筆刷樣式，含顏色與寬度</summary>
		/// <param name="color">指定的筆刷顏色</param>
		/// <param name="width">指定的筆刷寬度</param>
		public virtual void SetPen(Color color, float width) {
			mPenColor = color;
			mPenWidth = width;
		}

		/// <summary>根據現有 <see cref="MarkerDescriptor"/> 相關屬性設定 GDI+ 筆刷樣式</summary>
		/// <param name="color">由 <see cref="MarkerDescriptor"/> 描述的重疊圖像顏色</param>
		/// <param name="width">由 <see cref="MarkerDescriptor"/> 描述的重疊圖像粗細</param>
		protected virtual void SetPen(MarkerColor color, MarkerPenWidth width) {
			int aceClrVal = (int)color;
			int r = (aceClrVal & 0x0000FF);
			int g = (aceClrVal & 0x00FF00) >> 8;
			int b = (aceClrVal & 0xFF0000) >> 16;
			mPenColor = Color.FromArgb(205, r, g, b);
			SetPenWidth(width);
		}

		/// <summary>將 <see cref="MarkerPenWidth"/> 轉換為相對應的像素大小</summary>
		/// <param name="width">重疊圖像粗細</param>
		protected virtual void SetPenWidth(MarkerPenWidth width) {
			switch (width) {
				case MarkerPenWidth.None:
					mPenWidth = 0;
					break;
				case MarkerPenWidth.Thin:
					mPenWidth = mBordThinWidth;
					break;
				case MarkerPenWidth.Thick:
					mPenWidth = mBordThinWidth * 2;
					break;
			}
		}

		#endregion

	}
	#endregion

	#region Implement Class
	/// <summary>矩形重疊圖像</summary>
	public class OverlayRectangle : OverlayMarkerObjectBase {

		#region Properties
		/// <summary>取得此重疊圖像之座標</summary>
		public PointF Locator { get; private set; }
		/// <summary>取得此重疊圖像之大小</summary>
		public SizeF Size { get; private set; }
		#endregion

		#region Constructors
		/// <summary>使用現有的 <see cref="RectangleMarkerDescriptor"/> 來建構矩形重疊圖像</summary>
		/// <param name="descriptor">欲套用的 <see cref="RectangleMarkerDescriptor"/></param>
		/// <param name="thinWidth">相對於 <see cref="MarkerPenWidth.Thin"/> 之像素寬度</param>
		public OverlayRectangle(RectangleMarkerDescriptor descriptor, float thinWidth = -1) {
			if (thinWidth > -1) mBordThinWidth = thinWidth;
			Locator = new PointF((float)descriptor.Origin.X, (float)descriptor.Origin.Y);
			Size = new SizeF((float)descriptor.Width, (float)descriptor.Height);
			SetPen(descriptor.Color, descriptor.PenWidth);
			Rotation = (float)descriptor.Origin.Degrees;
			Shape = OverlayMarker.Rectangle;
		}
		#endregion
	}

	/// <summary>直線重疊圖像</summary>
	public class OverlayLine : OverlayMarkerObjectBase {

		#region Properties
		/// <summary>取得直線起點</summary>
		public PointF Start { get; private set; }
		/// <summary>取得直線終點</summary>
		public PointF End { get; private set; }
		#endregion

		#region Constructors
		/// <summary>使用現有的 <see cref="LineMarkerDescriptor"/> 來建構矩形重疊圖像</summary>
		/// <param name="descriptor">欲套用的 <see cref="LineMarkerDescriptor"/></param>
		/// <param name="thinWidth">相對於 <see cref="MarkerPenWidth.Thin"/> 之像素寬度</param>
		public OverlayLine(LineMarkerDescriptor descriptor, float thinWidth = -1) {
			if (thinWidth > -1) mBordThinWidth = thinWidth;
			Start = new PointF((float)descriptor.XStart, (float)descriptor.YStart);
			End = new PointF((float)descriptor.XEnd, (float)descriptor.YEnd);
			SetPen(descriptor.Color, descriptor.PenWidth);
			Shape = OverlayMarker.Line;
		}
		#endregion
	}

	/// <summary>含有 X、Y 軸的重疊圖像</summary>
	public class OverlayLocator : OverlayMarkerObjectBase {

		#region Properties
		/// <summary>取得重疊圖像座標</summary>
		public PointF Locator { get; private set; }
		/// <summary>取得重疊圖像的 X 軸長度</summary>
		public float XLength { get; private set; }
		/// <summary>取得重疊圖像的 Y 軸長度</summary>
		public float YLength { get; private set; }
		/// <summary>取得印出 X、Y 文字的字體</summary>
		public Font FontStyle { get; private set; }
		#endregion

		#region Constructors
		/// <summary>使用現有的 <see cref="AxesMarkerDescriptor"/> 來建構矩形重疊圖像</summary>
		/// <param name="descriptor">欲套用的 <see cref="AxesMarkerDescriptor"/></param>
		/// <param name="thinWidth">相對於 <see cref="MarkerPenWidth.Thin"/> 之像素寬度</param>
		public OverlayLocator(AxesMarkerDescriptor descriptor, float thinWidth = -1) {
			if (thinWidth > -1) mBordThinWidth = thinWidth;
			Locator = new PointF((float)descriptor.Origin.X, (float)descriptor.Origin.Y);
			XLength = (float)descriptor.XLength;
			YLength = (float)descriptor.YLength;
			SetPen(descriptor.Color, descriptor.PenWidth);
			Rotation = (float)descriptor.Origin.Degrees;
			Shape = OverlayMarker.Locator;
			FontStyle = new Font("Consolas", 16);
		}
		#endregion

		#region Methods
		/// <summary>設定印出 X、Y 文字之字體</summary>
		/// <param name="familyName">字型名稱，如 "Consolas"</param>
		/// <param name="emSize">字型大小</param>
		public void SetTextFont(string familyName, float emSize) {
			FontStyle = new Font(familyName, emSize);
		}
		/// <summary>設定印出 X、Y 文字之字體</summary>
		/// <param name="font">指定的字體</param>
		public void SetTextFont(Font font) {
			FontStyle = new Font(font.FontFamily, font.Size, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
		}
		#endregion
	}

	/// <summary>文字標籤重疊圖像</summary>
	public class OverlayText : OverlayMarkerObjectBase {

		#region Properties
		/// <summary>取得文字標籤的座標</summary>
		public PointF Locator { get; private set; }
		/// <summary>取得欲顯示之文字</summary>
		public string Text { get; private set; }
		/// <summary>取得顯示文字之字體</summary>
		public Font FontStyle { get; private set; }
		#endregion

		#region Constructors
		/// <summary>使用現有的 <see cref="LabelMarkerDescriptor"/> 來建構矩形重疊圖像</summary>
		/// <param name="descriptor">欲套用的 <see cref="LabelMarkerDescriptor"/></param>
		/// <param name="font">Font style of the text</param>
		/// <param name="thinWidth">相對於 <see cref="MarkerPenWidth.Thin"/> 之像素寬度</param>
		public OverlayText(LabelMarkerDescriptor descriptor, float thinWidth = -1, Font font = null) {
			if (thinWidth > -1) mBordThinWidth = thinWidth;
			Locator = new PointF((float)descriptor.X, (float)descriptor.Y);
			Text = descriptor.Text;
			SetPen(descriptor.Color, descriptor.PenWidth);
			FontStyle = font == null ? new Font("微軟正黑體", 15) : font;
			Shape = OverlayMarker.Text;
		}
		#endregion

		#region Methods
		/// <summary>設定印出 X、Y 文字之字體</summary>
		/// <param name="familyName">字型名稱，如 "Consolas"</param>
		/// <param name="emSize">字型大小</param>
		public void SetTextFont(string familyName, float emSize) {
			FontStyle = new Font(familyName, emSize);
		}
		/// <summary>設定印出 X、Y 文字之字體</summary>
		/// <param name="font">指定的字體</param>
		public void SetTextFont(Font font) {
			FontStyle = new Font(font.FontFamily, font.Size, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
		}
		#endregion
	}

	/// <summary>點狀重疊圖像</summary>
	public class OverlayPoint : OverlayMarkerObjectBase {

		#region Properties
		/// <summary>取得點座標</summary>
		public PointF Locator { get; private set; }
		/// <summary>取得點狀大小</summary>
		public SizeF PointSize { get; private set; }
		#endregion

		#region Constructors
		/// <summary>使用現有的 <see cref="PointMarkerDescriptor"/> 來建構矩形重疊圖像</summary>
		/// <param name="descriptor">欲套用的 <see cref="PointMarkerDescriptor"/></param>
		/// <param name="thinWidth">相對於 <see cref="MarkerPenWidth.Thin"/> 之像素寬度</param>
		public OverlayPoint(PointMarkerDescriptor descriptor, float thinWidth = -1) {
			if (thinWidth > -1) mBordThinWidth = thinWidth;
			Locator = new PointF((float)descriptor.X, (float)descriptor.Y);
			SetPen(descriptor.Color, descriptor.PenWidth);
			Shape = OverlayMarker.Point;
			PointSize = new SizeF(15, 15);
		}
		#endregion
	}

	/// <summary>弧狀重疊圖像</summary>
	public class OverlayArc : OverlayMarkerObjectBase {

		#region Properties
		/// <summary>取得弧狀中心點</summary>
		public PointF Locator { get; private set; }
		/// <summary>取得弧狀半徑</summary>
		public float Radius { get; private set; }
		/// <summary>取得 <see cref="VisionArc.Thickness"/></summary>
		public float Thickness { get; private set; }
		/// <summary>取得 <see cref="VisionArc.Opening"/></summary>
		public float Opening { get; private set; }
		#endregion

		#region Constructors
		/// <summary>使用現有的 <see cref="ArcMarkerDescriptor"/> 來建構矩形重疊圖像</summary>
		/// <param name="descriptor">欲套用的 <see cref="ArcMarkerDescriptor"/></param>
		/// <param name="thinWidth">相對於 <see cref="MarkerPenWidth.Thin"/> 之像素寬度</param>
		public OverlayArc(ArcMarkerDescriptor descriptor, float thinWidth = -1) {
			if (thinWidth > -1) mBordThinWidth = thinWidth;
			Locator = new PointF((float)descriptor.X, (float)descriptor.Y);
			Radius = (float)descriptor.Radius;
			Thickness = (float)descriptor.Thickness;
			Opening = (float)descriptor.Opening;
			Rotation = (float)descriptor.Rotation;
			SetPen(descriptor.Color, descriptor.PenWidth);
			Shape = OverlayMarker.Arc;
		}
		#endregion
	}
	#endregion

	#endregion

	#region Static Main Process
	/// <summary>提供將 <seealso cref="MarkerOverlayCollection"/> 繪製於 <see cref="IImageBuffer"/> 之檔案操作 </summary>
	public static class CtDrawOverlayMarkers {
		#region Declaration - Fields
		/// <summary>繪製的重疊圖像集合</summary>
		private static List<OverlayMarkerObjectBase> mDrawObj = new List<OverlayMarkerObjectBase>();
		#endregion

		#region Function - Methods
		/// <summary>將 <see cref="ImageType"/> 轉換為 Windows 可用之 <see cref="PixelFormat"/> 色彩資訊</summary>
		/// <param name="imgType">由 <see cref="IImageBuffer"/> 指定的影像格式 <see cref="ImageType"/></param>
		/// <returns>相對應的色彩資料格式</returns>
		private static PixelFormat GetPixelFormat(ImageType imgType) {
			PixelFormat px = PixelFormat.Format8bppIndexed;

			switch (imgType) {
				case ImageType.EightBitsPerPixelGreyScale:
					px = PixelFormat.Format8bppIndexed;
					break;
				case ImageType.ThirtyTwoBitsPerPixelRgb:
					px = PixelFormat.Format32bppRgb;
					break;
				default:
					px = PixelFormat.Format32bppRgb;
					break;
			}
			return px;
		}

		/// <summary>將校正座標(單位為 Millimeter)轉換為像素座標(單位為 Pixel)</summary>
		/// <param name="oriPoint">校正座標(單位為 Millimeter)</param>
		/// <param name="imgCent">影像中心點(單位為 Pixel)</param>
		/// <param name="mmPerPx">描述每點像素對應多少毫米之資訊</param>
		/// <returns>像素座標(單位為 Pixel)</returns>
		private static PointF CalculatePointToPixel(PointF oriPoint, SizeF mmPerPx, Point imgCent) {
			return new PointF(oriPoint.X / mmPerPx.Width + imgCent.X, imgCent.Y - oriPoint.Y / mmPerPx.Height);
		}

		/// <summary>將像素座標(單位為 Pixel)轉換為校正座標(單位為 Millimeter)</summary>
		/// <param name="oriPoint">像素座標(單位為 Pixel)</param>
		/// <param name="imgCent">影像中心點(單位為 Pixel)</param>
		/// <param name="mmPerPx">描述每點像素對應多少毫米之資訊</param>
		/// <returns>校正座標(單位為 Millimeter)</returns>
		private static PointF CalculatePixelToPoint(PointF oriPoint, SizeF mmPerPx, Point imgCent) {
			return new PointF((oriPoint.X - imgCent.X) * mmPerPx.Width, (imgCent.Y * 3 - oriPoint.Y) * mmPerPx.Height);
		}

		/// <summary>將校正大小(單位為 Millimeter)轉換為像素大小(單位為 Pixel)</summary>
		/// <param name="oriSize">校正大小(單位為 Millimeter)</param>
		/// <param name="mmPerPx">描述每點像素對應多少毫米之資訊</param>
		/// <returns>像素大小(單位為 Pixel)</returns>
		private static SizeF CalculateSizeToPixel(SizeF oriSize, SizeF mmPerPx) {
			SizeF size = new SizeF();
			size.Width = oriSize.Width / mmPerPx.Width;
			size.Height = oriSize.Height / mmPerPx.Height;
			return size;
		}

		/// <summary>將校正距離(單位為 Millimeter)轉換為像素距離(單位為 Pixel)</summary>
		/// <param name="dist">校正距離(單位為 Millimeter)</param>
		/// <param name="mmPerPx">描述每點像素對應多少毫米之資訊</param>
		/// <returns>像素距離(單位為 Pixel)</returns>
		private static float CalculateDistToPixel(float dist, SizeF mmPerPx) {
			return dist / mmPerPx.Width;
		}

		/// <summary>取得指定點旋轉後之目標座標</summary>
		/// <param name="oriPoint">指定的旋轉中心點</param>
		/// <param name="deg">欲旋轉的角度(單位為°)</param>
		/// <param name="length">旋轉半徑長度</param>
		/// <returns>旋轉後之目標座標</returns>
		private static PointF CalculateRotationPoint(PointF oriPoint, float deg, float length) {
			PointF point = new PointF();
			point.X = oriPoint.X + length * (float)Math.Cos(CalculateDegToRad(deg));
			point.Y = oriPoint.Y + length * (float)Math.Sin(CalculateDegToRad(deg));
			return point;
		}

		/// <summary>將徑度(Radin)轉換為角度(Degree)</summary>
		/// <param name="rad">欲計算的徑度 (0 ~ 2π)</param>
		/// <returns>角度(°)</returns>
		private static float CalculateRadToDeg(float rad) {
			return rad * 180 / (float)Math.PI;
		}

		/// <summary>將角度(Degree)轉換為徑度(Radin)</summary>
		/// <param name="deg">欲計算的角度 (0° ~ 360°)</param>
		/// <returns>徑度 (0 ~ 2π)</returns>
		private static float CalculateDegToRad(float deg) {
			return deg * (float)Math.PI / 180;
		}

        /// <summary>
        /// 保存NG原因
        /// </summary>
        /// <param name="fileName">NG圖片完整路徑</param>
        /// <param name="ngReason">NG原因</param>
        /// <remarks>
        /// 理想上fileName格式應該可分為兩個部分
        ///     1.檔案儲存路徑
        ///     2.檔名(紀錄時間:17102521 => 17:10:25.21)
        /// </remarks>
        private static void SaveNgReason(string fileName, string ngReason) {
            string savePath = Path.GetDirectoryName(fileName);
            string time = Path.GetFileNameWithoutExtension(fileName);
            time = time.Insert(2, ":").Insert(5,":").Insert(8,".");
            if (!CtFile.IsDirectoryExist(savePath)) CtFile.CreateDirectory(savePath);
            string[] split = ngReason.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string reason in split) {
                CtFile.WriteFile($@"{savePath}\log.txt",$"[{time}] - {reason}");
            }
        }
        #endregion

        #region Function - Core

        /// <summary>將圖像資訊繪出至 <seealso cref="Bitmap"/></summary>
        /// <param name="imgWidth">原始圖像寬度</param>
        /// <param name="imgHeight">原始圖像高度</param>
        /// <param name="imgData">原始圖像資訊</param>
        /// <param name="imgType">原始圖像色彩資訊</param>
        /// <returns>繪製的 GDI+ 物件</returns>
        private static Bitmap GetBufferImage(int imgWidth, int imgHeight, byte[] imgData, ImageType imgType) {
			/* Create a rectangle region and determine where need to copy raw data from buffer */
			Rectangle rect = new Rectangle(0, 0, imgWidth, imgHeight);

			/* Create a Bitmap, and set the width/height/pixelformat as same as buffer */
			Bitmap bmpDest = new Bitmap(imgWidth, imgHeight, GetPixelFormat(imgType));

			/* Because 8bpp GrayScale image is indexed, so using LockBits ans Scan0 to read/write each pixel instead of SetPixel */
			BitmapData bmpData = bmpDest.LockBits(rect, ImageLockMode.ReadWrite, GetPixelFormat(imgType));

			/* 'Cause the Bitmap's palette is a RGB palette as default, change it as a grayscale palette */
			if (bmpData.PixelFormat == PixelFormat.Format8bppIndexed) {
				ColorPalette palette = bmpDest.Palette; //Copy from bitmap
				for (int grayValue = 0; grayValue < 255; grayValue++) {
					palette.Entries[grayValue] = Color.FromArgb(grayValue, grayValue, grayValue);
				}
				bmpDest.Palette = palette;              //Assign new palette to bitmap
			}

			/*-- Writing the each pixel  --*/
			int width = bmpData.Width * (imgType == ImageType.EightBitsPerPixelGreyScale ? 1 : 4);
			if (bmpData.Stride == width) {
				/* 'Cause operate with LockBits and Scan0, using Marshal.Copy to copy data into Pointer(IntPtr) */
				Marshal.Copy(
					imgData,                    //Source raw data
					0,                          //Start index of source array
					bmpData.Scan0,              //Pointer(IntPtr) of destination bitmap
					width * bmpData.Height  //Length of raw data. Or use imgData.Length instead
				);
			} else {
				/* If width cannot be divide by 4, bmpDest's Stride will be floor 4 times. That cannot use Marshal.Copy which will cause array size mismatch. Calculate offset and add it back */
				unsafe
				{
					int offset = bmpData.Stride - width;
					byte* ptr = (byte*)bmpData.Scan0;
					for (int row = 0; row < bmpData.Height; row++) {
						for (int col = 0; col < width; col++) {
							ptr[0] = ptr[1] = ptr[2] = imgData[row * width + col];
							ptr++;
						}
						ptr += offset;
					}
				}
			}

			/*-- After operation, release bits  --*/
			bmpDest.UnlockBits(bmpData);

			/*-- Return --*/
			return bmpDest;
		}

		/// <summary>將 <see cref="IImageBuffer"/> 繪製成 <see cref="Bitmap"/></summary>
		/// <param name="buffer">欲繪製的 <see cref="IImageBuffer"/></param>
		/// <returns>繪出的 <see cref="Bitmap"/></returns>
		private static Bitmap GetBufferImage(IImageBuffer buffer) {

			/*-- Get the image raw data from buffer --*/
			byte[] imgData = new byte[buffer.CachedImage.ImageData.Length];
			Array.Copy(buffer.CachedImage.ImageData, imgData, buffer.CachedImage.ImageData.Length);

			/*-- Create and setting a bitmap --*/
			/* Get the width and height from buffer */
			int imgWidth = buffer.CachedImage.ImageWidth;
			int imgHeight = buffer.CachedImage.ImageHeight;
			ImageType imgType = buffer.ImageType;

			/*-- Build bitmap by byte data --*/
			return GetBufferImage(imgWidth, imgHeight, imgData, imgType);

		}

		/// <summary>將重疊圖像畫至新的 <see cref="Bitmap"/></summary>
		/// <param name="bmpOri">原始的 GDI+ 圖像</param>
		/// <param name="drawObj">需繪製的重疊圖像集合</param>
		/// <param name="mmPerPx">描述每點像素對應多少毫米之資訊</param>
		/// <returns>將原始 GDI+ 圖像畫上重疊圖像之新圖</returns>
		private static Bitmap DrawObjects(Bitmap bmpOri, List<OverlayMarkerObjectBase> drawObj, SizeF mmPerPx) {
            /*-- 'Cause the real image are up-side-down with .Net Y coordinate, so flip it --*/
            bmpOri.RotateFlip(RotateFlipType.RotateNoneFlipY);

            /*-- Create a bitmap to save different pixelformat image from buffer --*/
            /* Creation */
            Bitmap bmpTemp = new Bitmap(bmpOri.Width, bmpOri.Height, PixelFormat.Format32bppRgb);
            /* Create Graphics handle, convert pixelformat and paint objects on bitmap */
            Graphics g = Graphics.FromImage(bmpTemp);
            float fontsize = AdaptiveTextSize(bmpOri.Height, g);
            Font font = new Font("微軟正黑體", fontsize);

            /* Paint bmpOri onto bmpTemp */
            g.DrawImage(bmpOri, 0, 0);

            /* Center Point */
            Point center = new Point(bmpOri.Width / 2, bmpOri.Height / 2);

            /*-- Draw objects on bitmap by Graphics --*/
            Pen pen = null;

            Color clrNg = Color.FromArgb(255,255,0,0);//Ng顏色
            SolidBrush brsNg = new SolidBrush(clrNg);//Ng筆刷
            bool isNg = false;
            PointF pPoint;

            drawObj.ForEach(
                obj => {
                    pen = obj.DrawPen;
                    if (pen.Color == clrNg) isNg = true;
                    switch (obj.Shape) {
                        case OverlayMarker.Arc:    /* Non-Handled */
                            OverlayArc sArc = obj as OverlayArc;
                            DrawArc(g,sArc,center,mmPerPx);
                            break;

                        case OverlayMarker.Line:
                            //Convert type
                            OverlayLine sLine = obj as OverlayLine;
                            //Draw line directly, the point are re-calculated.
                            g.DrawLine(pen, CalculatePointToPixel(sLine.Start, mmPerPx, center), CalculatePointToPixel(sLine.End, mmPerPx, center));
                            bool isRed = pen.Color == Color.Red;
                            break;

                        case OverlayMarker.Locator:
                            //Convert type
                            OverlayLocator sLocator = obj as OverlayLocator;

                            //Create a arrow cap with Pen, it will like "→". The LineCap.ArrowAnchor are too small, so make it by ourself.
                            using (GraphicsPath capPath = new GraphicsPath()) {
                                capPath.AddLine(new Point(0, 0), CalculateRotationPoint(new Point(0, 0), -45, CalculateDistToPixel(sLocator.XLength * 0.05F, mmPerPx)));
                                capPath.AddLine(new Point(0, 0), CalculateRotationPoint(new Point(0, 0), 225, CalculateDistToPixel(sLocator.XLength * 0.05F, mmPerPx)));
                                pen.CustomEndCap = new CustomLineCap(null, capPath);    //Assign to the Pen object
                            }

                            //Convert locator from millimeter units to the pixel position.
                            PointF locStart = CalculatePointToPixel(sLocator.Locator, mmPerPx, center);

                            //Paint X component
                            PointF locEnd = CalculateRotationPoint(locStart, -sLocator.Rotation, CalculateDistToPixel(sLocator.XLength, mmPerPx));
                            g.DrawLine(pen, locStart, locEnd);

                            //Paint "X" word beside end of arrow
                            locEnd = CalculateRotationPoint(locEnd, sLocator.Rotation, 5);  //calculate point with rotation (Trigonometric)
                            g.TranslateTransform(locEnd.X, locEnd.Y);                       //Set the picture center to the shape locator
                            g.RotateTransform(-sLocator.Rotation);                          //the word must rotation with entire shape
                            SizeF labSize = g.MeasureString("X", sLocator.FontStyle);       //calculate the word size
                            g.DrawString("X", sLocator.FontStyle, sLocator.DrawBrush, 0, -labSize.Height / 2);  //because the translateTransform, so with shift up with half of height.

                            //Reset translate and rotation transform
                            g.ResetTransform();

                            //Paint Y component
                            locEnd = CalculateRotationPoint(locStart, -sLocator.Rotation - 90, CalculateDistToPixel(sLocator.YLength, mmPerPx));
                            g.DrawLine(pen, locStart, locEnd);

                            //Paint "Y" word up to end of arrow
                            locEnd = CalculateRotationPoint(locEnd, -sLocator.Rotation - 90, 5);    //calculate point with rotation (Trigonometric)
                            g.TranslateTransform(locEnd.X, locEnd.Y);                               //Set the picture center to the shape locator
                            g.RotateTransform(-sLocator.Rotation);                                  //the word must rotation with entire shape
                            labSize = g.MeasureString("Y", sLocator.FontStyle);                     //calculate the word size
                            g.DrawString("Y", sLocator.FontStyle, sLocator.DrawBrush, -labSize.Width / 2, -labSize.Height); //because the translateTransform, so with shift left-up

                            break;

                        case OverlayMarker.Point:
                            //Convert type
                            OverlayPoint sPoint = obj as OverlayPoint;

                            //Convert locator from millimeter units to the pixel position.
                            pPoint = CalculatePointToPixel(sPoint.Locator, mmPerPx, center);

                            //Set rectangle with pixel position calculated
                            RectangleF pRect = new RectangleF(
                                                                pPoint.X - sPoint.PointSize.Width / 2,
                                                                pPoint.Y - sPoint.PointSize.Height / 2,
                                                                sPoint.PointSize.Width,
                                                                sPoint.PointSize.Height
                                                );

                            //Paint a rectangle on it
                            g.FillRectangle(sPoint.DrawBrush, pRect);

                            break;

                        case OverlayMarker.Rectangle:
                            //Convert type
                            OverlayRectangle sRectangle = obj as OverlayRectangle;

                            //Convert locator from millimeter units to the pixel position.
                            PointF recPoint = CalculatePointToPixel(sRectangle.Locator, mmPerPx, center);
                            SizeF recSize = CalculateSizeToPixel(sRectangle.Size, mmPerPx);

                            //Rotate to the rectangle center
                            g.TranslateTransform(recPoint.X, recPoint.Y);
                            g.RotateTransform(-sRectangle.Rotation);

                            //Paint it! but the SearchRegion anchor at rectangle center, so it must be half of width and height
                            g.DrawRectangle(pen, -recSize.Width / 2, -recSize.Height / 2, recSize.Width, recSize.Height);

                            break;

                        case OverlayMarker.Text:
                            //Convert type
                            OverlayText sText = obj as OverlayText;

                            //Convert locator from millimeter units to the pixel position.
                            PointF txtPoint = CalculatePointToPixel(sText.Locator, mmPerPx, center);

                            //Set the picture center to the shape locator
                            g.TranslateTransform(txtPoint.X, txtPoint.Y);
                            
                            //Measure string size and paint it with left-bottom anchor (As Adept ACE)
                            SizeF txtMesSize = g.MeasureString(sText.Text, font);
                            g.DrawString(sText.Text, font, sText.DrawBrush, 0, -txtMesSize.Height);  //Anchor from left-top to left-bottom, so shift up.
                            break;

                        default:
                            break;
                    }

                    //Rotate back
                    g.ResetTransform();
                }
            );

            /*-- 繪製Ng敘述 --*/
            string ngDescription = VisionExecutor.NgReason;
            string time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff");
            if (!string.IsNullOrEmpty(ngDescription)) {
                g.DrawString(ngDescription, font, brsNg, 0, 0);
            }else if (isNg) {
                g.DrawString("料件规格不符",font, brsNg, 0,0);
            }

            /*-- 繪製辨識時間 --*/
            SizeF txtSize = g.MeasureString(time, font);
            g.DrawString(time, font, brsNg, 0, bmpOri.Height - txtSize.Height);

            /*-- Dispose Objects --*/
            if (pen != null) pen.Dispose();

            return bmpTemp;
        }

        /// <summary>
        /// 用於繪製Ng圖中Arc的ROI
        /// </summary>
        /// <param name="g">繪圖工具</param>
        /// <param name="sArc">弧線物件</param>
        /// <param name="center">輸出中心點</param>
        /// <param name="mmPerPx">pixel轉換比例</param>
        /// <remarks>
        /// Ace與C#的Arc表示法並不相同
        /// [ACE]
        ///     Locator:圓心
        ///     Radius:圓半徑
        ///     Thickness:搜尋寬度
        ///     Rotation:弧線中心點與圓心夾角角度
        ///     Opening:從弧線中心展開的角度
        /// [C#]
        ///     Rect:包住該圓的矩形範圍
        ///     StartAngle:起始角度
        ///     SweepAngle:從起始角度開始畫的弧線角度
        /// 座標方向也不相同，都是從正右方開始計算
        /// [ACE]
        ///     逆時針方向
        /// [C#]
        ///     順時針方向
        /// </remarks>
        private static void DrawArc(Graphics g, OverlayArc sArc, Point center, SizeF mmPerPx) {
            /*-- 參數設置 --*/
            Pen pen = sArc.DrawPen;//指定顏色之畫筆
            Pen dashPen = sArc.DrawPen.Clone() as Pen;//同樣顏色，但是為虛線的畫筆
            dashPen.DashStyle = DashStyle.Dash;
            PointF arcCenter = CalculatePointToPixel(sArc.Locator, mmPerPx, center);//弧線中心
            float startAngle = -sArc.Rotation - sArc.Opening / 2;//起始角度
            float radius = CalculateDistToPixel(sArc.Radius, mmPerPx);//半徑
            float thickness = CalculateDistToPixel(sArc.Thickness / 2, mmPerPx);//搜尋寬度
            float inRadius = radius - thickness;//內徑
            float outRadius = radius + thickness;//外徑
            
            /*--判斷是Arc Result 還是 Arc ROI--*/
            if (thickness > 0) {//有搜尋寬度的是ROI
                /*-- 繪製半徑 --*/
                DrawArc(g, dashPen, arcCenter, radius, startAngle, sArc.Opening);

                /*-- 繪製內徑 --*/
                DrawArc(g, pen, arcCenter, inRadius, startAngle, sArc.Opening);

                /*-- 繪製外徑 --*/
                DrawArc(g, pen, arcCenter, outRadius, startAngle, sArc.Opening);

                /*-- 繪製中心線 --*/
                DrawArcPoint(g, dashPen, arcCenter, outRadius, -sArc.Rotation);

                /*-- 繪製起始邊界 --*/
                DrawArcPoint(g, dashPen, arcCenter, outRadius, startAngle);

                /*-- 繪製結束邊界 --*/
                DrawArcPoint(g, dashPen, arcCenter, outRadius, startAngle + sArc.Opening);
            } else if (thickness == 0){//Arc Result是沒有搜尋寬度的
                pen = new Pen(Color.Magenta, pen.Width);
                /*-- 繪製半徑 --*/
                DrawArc(g, pen, arcCenter, radius, startAngle, sArc.Opening);
            } else {
                throw new ArgumentException("Arc thickness小於0無法繪製");
            }
        }

        /// <summary>
        /// 繪製點到弧線之直線
        /// </summary>
        /// <param name="g">繪圖工具</param>
        /// <param name="pen">畫筆</param>
        /// <param name="arcCenter">弧線中心</param>
        /// <param name="radius">弧線半徑</param>
        /// <param name="angle">弧線上指定一點之角度</param>
        private static void DrawArcPoint(Graphics g, Pen pen, PointF arcCenter, float radius, float angle) {
            PointF arcPoint = new PointF(arcCenter.X, arcCenter.Y);
            arcPoint.X += (float)(radius * Math.Cos(angle * Math.PI / 180));
            arcPoint.Y += (float)(radius * Math.Sin(angle * Math.PI / 180));
            g.DrawLine(pen,arcCenter,arcPoint);
        }

        /// <summary>
        /// 弧線繪製
        /// </summary>
        /// <param name="g">繪圖介面</param>
        /// <param name="dashPen">畫筆</param>
        /// <param name="arcCenter">弧線中心</param>
        /// <param name="radius">弧線半徑</param>
        /// <param name="startAngle">弧線起始角度</param>
        /// <param name="opening">弧線展開角度</param>
        private static void DrawArc(Graphics g, Pen dashPen, PointF arcCenter, float radius, float startAngle, float opening) {
            RectangleF rct = new RectangleF(arcCenter.X - radius, arcCenter.Y - radius, radius * 2, radius * 2);
            g.DrawArc(dashPen, rct, startAngle, opening);
        }

        /// <summary>
        /// 回傳合適比例的字體大小
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="bmHeight"></param>
        /// <param name="g">圖片繪圖工具，用於量測字體高度</param>
        /// <param name="proportion">字體高度與圖片高度比例</param>
        /// <returns></returns>
        private static float AdaptiveTextSize(float bmHeight, Graphics g, float proportion = 22.5f) {
            float fontsize = 1;
            SizeF mesSize = new SizeF(0, 0);
            float s = bmHeight / proportion;
            while (Math.Abs(mesSize.Height - s) > 1 || mesSize.Height > 100) {
                fontsize++;
                mesSize = g.MeasureString("Test", new Font("微軟正黑體", fontsize));
            }

            return fontsize;
        }

        /// <summary>根據檔名分析相對應的圖片儲存格式</summary>
        /// <param name="fileName">圖片檔案名稱</param>
        /// <returns>檔案名稱相對應的儲存格式</returns>
        private static ImageFormat GetImageFormat(string fileName) {
			ImageFormat imgFormat = null;
			string name = Path.GetExtension(fileName).ToLower();
			switch (name) {
				case "bmp":
					imgFormat = ImageFormat.Bmp;
					break;
				case "gif":
					imgFormat = ImageFormat.Gif;
					break;
				case "jpg":
				case "jpeg":
					imgFormat = ImageFormat.Jpeg;
					break;
				case "png":
				default:
					imgFormat = ImageFormat.Png;
					break;
			}
			return imgFormat;
		}

		/// <summary>將 <see cref="MarkerOverlayCollection"/> 繪製於 <see cref="IVisionImageSource"/> 之影像儲存成圖片檔案</summary>
		/// <param name="imgSrc">含有原始背景影像的 <see cref="IVisionImageSource"/></param>
		/// <param name="fileName">欲儲存的圖片檔名，如 @"D:\CASTEC\Log\ACE\Error.png"</param>
		/// <param name="drawObj">欲繪製的重疊圖像集合</param>
		private static void DrawObjects(string fileName, IVisionImageSource imgSrc, List<OverlayMarkerObjectBase> drawObj) {
			/*-- Get Millimeter/Pixel --*/
			SizeF mmPerPx = imgSrc.ActiveCalibration.ImagePixelSize;

			/*-- Get image from buffer --*/
			Bitmap bmpOri = GetBufferImage(imgSrc.Buffer);

			/*-- Paint to another bitmap --*/
			Bitmap bmpTar = DrawObjects(bmpOri, drawObj, mmPerPx);

			/*-- Save image, if necessary --*/
			bmpTar.Save(fileName, GetImageFormat(fileName));

			/*-- Clear memory --*/
			bmpTar.Dispose();
			bmpOri.Dispose();
		}

		/// <summary>確保檔案儲存路徑資料夾</summary>
		/// <param name="fileName">欲儲存的檔案路徑</param>
		private static void EnsurePath(string fileName) {
			string dir = Path.GetDirectoryName(fileName);
			if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
		}
		#endregion

		#region Function - Entrance

		/// <summary>取得 <see cref="ICSharpCustomTool"/> 內的 <see cref="MarkerOverlayCollection"/> 並轉換為相對應的重疊圖像集合</summary>
		/// <param name="cvt">欲取得資訊的 <see cref="ICSharpCustomTool"/></param>
		/// <param name="thinWidth">相對應於 <see cref="MarkerPenWidth.Thin"/> 之像素大小</param>
		/// <returns><see cref="OverlayMarkerObjectBase"/> 集合</returns>
		public static List<OverlayMarkerObjectBase> GetShapeObjects(ICSharpCustomTool cvt, float thinWidth = -1) {
			string typeName = string.Empty;
			List<OverlayMarkerObjectBase> drawObj = new List<OverlayMarkerObjectBase>();
			Array.ForEach(
				cvt.OverlayMarkers.Markers,
				marker => {
					typeName = marker.GetType().Name;
					switch (typeName) {
						case "ArcMarkerDescriptor":
							drawObj.Add(new OverlayArc(marker as ArcMarkerDescriptor, thinWidth));
							break;
						case "LineMarkerDescriptor":
							drawObj.Add(new OverlayLine(marker as LineMarkerDescriptor, thinWidth));
							break;
						case "AxesMarkerDescriptor":
							drawObj.Add(new OverlayLocator(marker as AxesMarkerDescriptor, thinWidth));
							break;
						case "PointMarkerDescriptor":
							drawObj.Add(new OverlayPoint(marker as PointMarkerDescriptor, thinWidth));
							break;
						case "RectangleMarkerDescriptor":
							drawObj.Add(new OverlayRectangle(marker as RectangleMarkerDescriptor, thinWidth));
							break;
						case "LabelMarkerDescriptor":
							drawObj.Add(new OverlayText(marker as LabelMarkerDescriptor, thinWidth));
							break;
						default:
							break;
					}
				}
			);
			return drawObj;
		}

		/// <summary>取得 <see cref="ICSharpCustomTool"/> 內的 <see cref="MarkerOverlayCollection"/> 並轉換為相對應的重疊圖像集合</summary>
		/// <param name="cvt">欲取得資訊的 <see cref="ICSharpCustomTool"/></param>
		/// <param name="thinWidth">相對應於 <see cref="MarkerPenWidth.Thin"/> 之像素大小</param>
		/// <returns><see cref="OverlayMarkerObjectBase"/> 集合</returns>
		public static List<OverlayMarkerObjectBase> GetOverlayObjects(this ICSharpCustomTool cvt, float thinWidth = -1) {
			string typeName = string.Empty;
			List<OverlayMarkerObjectBase> drawObj = new List<OverlayMarkerObjectBase>();
			Array.ForEach(
				cvt.OverlayMarkers.Markers,
				marker => {
					typeName = marker.GetType().Name;
					switch (typeName) {
						case "ArcMarkerDescriptor":
							drawObj.Add(new OverlayArc(marker as ArcMarkerDescriptor, thinWidth));
							break;
						case "LineMarkerDescriptor":
							drawObj.Add(new OverlayLine(marker as LineMarkerDescriptor, thinWidth));
							break;
						case "AxesMarkerDescriptor":
							drawObj.Add(new OverlayLocator(marker as AxesMarkerDescriptor, thinWidth));
							break;
						case "PointMarkerDescriptor":
							drawObj.Add(new OverlayPoint(marker as PointMarkerDescriptor, thinWidth));
							break;
						case "RectangleMarkerDescriptor":
							drawObj.Add(new OverlayRectangle(marker as RectangleMarkerDescriptor, thinWidth));
							break;
						case "LabelMarkerDescriptor":
							drawObj.Add(new OverlayText(marker as LabelMarkerDescriptor, thinWidth));
							break;
						default:
							break;
					}
				}
			);
			return drawObj;
		}

		/// <summary>將當前 <see cref="ICSharpCustomTool"/> 之影像(含 OverlayMarkers)繪製成檔案</summary>
		/// <param name="cvt">欲繪製影像檔案的 <see cref="ICSharpCustomTool"/></param>
		/// <param name="fileName">欲存檔的檔案名稱，如 @"D:\CASTEC\Log\ACE\Error.png"</param>
		public static void DrawShape(ICSharpCustomTool cvt, string fileName) {
			float thinWidth = 0.05F / cvt.ImageSource.ActiveCalibration.ImagePixelSize.Width;
			mDrawObj.Clear();
			mDrawObj.AddRange(GetShapeObjects(cvt, thinWidth));

			DrawObjects(fileName, cvt.ImageSource, mDrawObj);
		}

		/// <summary>將當前 <see cref="ICSharpCustomTool"/> 之影像(含 OverlayMarkers)繪製成檔案</summary>
		/// <param name="cvt">欲繪製影像檔案的 <see cref="ICSharpCustomTool"/></param>
		/// <param name="fileName">欲存檔的檔案名稱，如 @"D:\CASTEC\Log\ACE\Error.png"</param>
		public static void DrawImage(this ICSharpCustomTool cvt, string fileName) {
			float thinWidth = 0.05F / cvt.ImageSource.ActiveCalibration.ImagePixelSize.Width;
			mDrawObj.Clear();
			mDrawObj.AddRange(GetShapeObjects(cvt, thinWidth));

			DrawObjects(fileName, cvt.ImageSource, mDrawObj);
		}

        /// <summary>採用半非同步的方式將當前 <see cref="ICSharpCustomTool"/> 之影像(含 OverlayMarkers)繪製成檔案</summary>
        /// <param name="cvt">欲繪製影像檔案的 <see cref="ICSharpCustomTool"/></param>
        /// <param name="fileName">欲存檔的檔案名稱，如 @"D:\CASTEC\Log\ACE\Error.png"</param>
        /// <param name="colorDeep">影像來源之色彩深度</param>
        /// <param name="ngReason">NG原因</param>
        /// <returns>非同步的工作，可供檢視其結果或回傳值</returns>
        /// <remarks>因 CVT 是執行時建立執行個體，故如要先用同步方法把必要資訊複製至記憶體後才可交由 Task 執行成為非同步，否則資訊有可能被新的執行個體覆蓋</remarks>
        public static Task BeginDrawShape(ICSharpCustomTool cvt, string fileName,string ngReason, ImageType colorDeep = ImageType.EightBitsPerPixelGreyScale) {
            List<OverlayMarkerObjectBase> drawObj = null;
            IVisionImageSource imgSrc = cvt.ImageSource;
            IImageBuffer imgBuf = imgSrc.Buffer;

            SizeF mmPerPx = new SizeF(imgSrc.ActiveCalibration.ImagePixelSize);

            Size imgSize = imgBuf.ImageSize;

            int imgWidth = imgSize.Width;
            int imgHeight = imgSize.Height;

            int imgLen = imgWidth * imgHeight * (colorDeep == ImageType.EightBitsPerPixelGreyScale ? 1 : 4);
            byte[] bufData = new byte[imgLen];

            float thinWidth = 0.05F / mmPerPx.Width;
            Task tskObj = Task.Run(() => drawObj = GetShapeObjects(cvt, thinWidth));
            Task tskData = Task.Run(
                () => Array.Copy(imgBuf.CachedImage.ImageData, bufData, imgLen));
            Task.WaitAll(tskData, tskObj);

            Task tsk = Task.Run(
                () => {
                    try {
                        Bitmap imgOri = GetBufferImage(
                                                        imgWidth,
                                                        imgHeight,
                                                        bufData,
                                                        colorDeep
                                                    );
                        Bitmap imgTar = DrawObjects(imgOri, drawObj, mmPerPx);
                        SaveNgReason(fileName,ngReason);
                        EnsurePath(fileName);
                        imgTar.Save(fileName, GetImageFormat(fileName));
                        imgOri.Dispose();
                        imgTar.Dispose();
                    } catch (Exception ex) {
                        /*-- 區域變數 --*/
                        DateTime time = DateTime.Now;
                        string path = string.Format(@"D:\CASTEC\Log\{0}", time.ToString("yyyyMMdd"));
                        string file = string.Format(@"{0}\{1}", path, "DynamicException.log");

                        /*-- 檢查資料夾存不存在，不存在則建立之 --*/
                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                        /*-- 抓取 Exception 資訊 --*/
                        string execMsg = ex.Message.Replace(Environment.NewLine, " ");
                        string traceMsg = string.Empty, methodName = string.Empty;
                        StackTrace stkTrace = new StackTrace(true);
                        if (stkTrace.FrameCount > 0) {
                            /* 只找有行號的，如果是 .Net 內部的則不會有行號！ */
                            List<StackFrame> stFrames = stkTrace.GetFrames().ToList().FindAll(data => data.GetFileLineNumber() > 0);
                            if (stFrames != null && stFrames.Count > 0) {
                                methodName = stFrames.Last().GetMethod().Name;  //取得方法名稱
                                traceMsg = string.Join(                         //將 StackTrace 資訊存出來
                                            " | ",
                                            stFrames.ConvertAll(
                                                data =>
                                                    string.Format(
                                                        "{0}({1}:{2})",
                                                        data.GetMethod().Name,
                                                        Path.GetFileNameWithoutExtension(data.GetFileName()),
                                                        data.GetFileLineNumber().ToString()
                                                    )
                                            )
                                        );
                            }
                        }

                        /*-- 組裝 Log 內容 --*/
                        string content = string.Format("[{0}] <{1}> {2} {{{3}}}]", time.ToString("HH:mm:ss.fff"), methodName, execMsg, traceMsg);

                        /*-- 寫至檔案 --*/
                        using (StreamWriter sw = new StreamWriter(file, true, Encoding.UTF8)) {
                            sw.WriteLine(content);
                        }
                    }
                }
            );

            return tsk;
        }
        
        /// <summary>採用半非同步的方式將當前 <see cref="ICSharpCustomTool"/> 之影像(含 OverlayMarkers)繪製成檔案</summary>
        /// <param name="cvt">欲繪製影像檔案的 <see cref="ICSharpCustomTool"/></param>
        /// <param name="fileName">欲存檔的檔案名稱，如 @"D:\CASTEC\Log\ACE\Error.png"</param>
        /// <param name="colorDeep">影像來源之色彩深度</param>
        /// <returns>非同步的工作，可供檢視其結果或回傳值</returns>
        /// <remarks>因 CVT 是執行時建立執行個體，故如要先用同步方法把必要資訊複製至記憶體後才可交由 Task 執行成為非同步，否則資訊有可能被新的執行個體覆蓋</remarks>
        public static Task BeginDrawShape(ICSharpCustomTool cvt, string fileName, ImageType colorDeep = ImageType.EightBitsPerPixelGreyScale) {
            List<OverlayMarkerObjectBase> drawObj = null;
			IVisionImageSource imgSrc = cvt.ImageSource;
            IImageBuffer imgBuf = imgSrc.Buffer;
          
            SizeF mmPerPx = new SizeF(imgSrc.ActiveCalibration.ImagePixelSize);

            Size imgSize = imgBuf.ImageSize;

			int imgWidth = imgSize.Width;
			int imgHeight = imgSize.Height;

			int imgLen = imgWidth * imgHeight * (colorDeep == ImageType.EightBitsPerPixelGreyScale ? 1 : 4);
			byte[] bufData = new byte[imgLen];

			float thinWidth = 0.05F / mmPerPx.Width;
			Task tskObj = Task.Run(() => drawObj = GetShapeObjects(cvt, thinWidth));
			Task tskData = Task.Run(
				() => Array.Copy(imgBuf.CachedImage.ImageData, bufData, imgLen));
			Task.WaitAll(tskData, tskObj);

			Task tsk = Task.Run(
				() => {
					try {
						Bitmap imgOri = GetBufferImage(
														imgWidth,
														imgHeight,
														bufData,
														colorDeep
													);
						Bitmap imgTar = DrawObjects(imgOri, drawObj, mmPerPx);
						EnsurePath(fileName);
						imgTar.Save(fileName, GetImageFormat(fileName));
						imgOri.Dispose();
						imgTar.Dispose();
					} catch (Exception ex) {
						/*-- 區域變數 --*/
						DateTime time = DateTime.Now;
						string path = string.Format(@"D:\CASTEC\Log\{0}", time.ToString("yyyyMMdd"));
						string file = string.Format(@"{0}\{1}", path, "DynamicException.log");

						/*-- 檢查資料夾存不存在，不存在則建立之 --*/
						if (!Directory.Exists(path)) Directory.CreateDirectory(path);

						/*-- 抓取 Exception 資訊 --*/
						string execMsg = ex.Message.Replace(Environment.NewLine, " ");
						string traceMsg = string.Empty, methodName = string.Empty;
						StackTrace stkTrace = new StackTrace(true);
						if (stkTrace.FrameCount > 0) {
							/* 只找有行號的，如果是 .Net 內部的則不會有行號！ */
							List<StackFrame> stFrames = stkTrace.GetFrames().ToList().FindAll(data => data.GetFileLineNumber() > 0);
							if (stFrames != null && stFrames.Count > 0) {
								methodName = stFrames.Last().GetMethod().Name;  //取得方法名稱
								traceMsg = string.Join(                         //將 StackTrace 資訊存出來
											" | ",
											stFrames.ConvertAll(
												data =>
													string.Format(
														"{0}({1}:{2})",
														data.GetMethod().Name,
														Path.GetFileNameWithoutExtension(data.GetFileName()),
														data.GetFileLineNumber().ToString()
													)
											)
										);
							}
						}

						/*-- 組裝 Log 內容 --*/
						string content = string.Format("[{0}] <{1}> {2} {{{3}}}]", time.ToString("HH:mm:ss.fff"), methodName, execMsg, traceMsg);

						/*-- 寫至檔案 --*/
						using (StreamWriter sw = new StreamWriter(file, true, Encoding.UTF8)) {
							sw.WriteLine(content);
						}
					}
				}
			);

			return tsk;
		}

		/// <summary>採用半非同步的方式將當前 <see cref="ICSharpCustomTool"/> 之影像(含 OverlayMarkers)繪製成檔案</summary>
		/// <param name="cvt">欲繪製影像檔案的 <see cref="ICSharpCustomTool"/></param>
		/// <param name="fileName">欲存檔的檔案名稱，如 @"D:\CASTEC\Log\ACE\Error.png"</param>
		/// <param name="colorDeep">影像來源之色彩深度</param>
		/// <returns>非同步的工作，可供檢視其結果或回傳值</returns>
		/// <remarks>因 CVT 是執行時建立執行個體，故如要先用同步方法把必要資訊複製至記憶體後才可交由 Task 執行成為非同步，否則資訊有可能被新的執行個體覆蓋</remarks>
		public static Task BeginDrawImage(this ICSharpCustomTool cvt, string fileName, ImageType colorDeep = ImageType.EightBitsPerPixelGreyScale) {
			List<OverlayMarkerObjectBase> drawObj = null;
			IVisionImageSource imgSrc = cvt.ImageSource;
			IImageBuffer imgBuf = imgSrc.Buffer;
			SizeF mmPerPx = new SizeF(imgSrc.ActiveCalibration.ImagePixelSize);
			Size imgSize = imgBuf.ImageSize;

			int imgWidth = imgSize.Width;
			int imgHeight = imgSize.Height;

			int imgLen = imgWidth * imgHeight * (colorDeep == ImageType.EightBitsPerPixelGreyScale ? 1 : 4);
			byte[] bufData = new byte[imgLen];

			float thinWidth = 0.05F / mmPerPx.Width;

			Task tskObj = Task.Run(() => drawObj = GetShapeObjects(cvt, thinWidth));
			Task tskData = Task.Run(
				() => Array.Copy(imgBuf.CachedImage.ImageData, bufData, imgLen));
			Task.WaitAll(tskData, tskObj);

			Task tsk = Task.Run(
				() => {
					try {
						Bitmap imgOri = GetBufferImage(
														imgWidth,
														imgHeight,
														bufData,
														colorDeep
													);
						Bitmap imgTar = DrawObjects(imgOri, drawObj, mmPerPx);
						EnsurePath(fileName);
						imgTar.Save(fileName, GetImageFormat(fileName));
						imgOri.Dispose();
						imgTar.Dispose();
					} catch (Exception ex) {
						/*-- 區域變數 --*/
						DateTime time = DateTime.Now;
						string path = string.Format(@"D:\CASTEC\Log\{0}", time.ToString("yyyyMMdd"));
						string file = string.Format(@"{0}\{1}", path, "DynamicException.log");

						/*-- 檢查資料夾存不存在，不存在則建立之 --*/
						if (!Directory.Exists(path)) Directory.CreateDirectory(path);

						/*-- 抓取 Exception 資訊 --*/
						string execMsg = ex.Message.Replace(Environment.NewLine, " ");
						string traceMsg = string.Empty, methodName = string.Empty;
						StackTrace stkTrace = new StackTrace(true);
						if (stkTrace.FrameCount > 0) {
							/* 只找有行號的，如果是 .Net 內部的則不會有行號！ */
							List<StackFrame> stFrames = stkTrace.GetFrames().ToList().FindAll(data => data.GetFileLineNumber() > 0);
							if (stFrames != null && stFrames.Count > 0) {
								methodName = stFrames.Last().GetMethod().Name;  //取得方法名稱
								traceMsg = string.Join(                         //將 StackTrace 資訊存出來
											" | ",
											stFrames.ConvertAll(
												data =>
													string.Format(
														"{0}({1}:{2})",
														data.GetMethod().Name,
														Path.GetFileNameWithoutExtension(data.GetFileName()),
														data.GetFileLineNumber().ToString()
													)
											)
										);
							}
						}

						/*-- 組裝 Log 內容 --*/
						string content = string.Format("[{0}] <{1}> {2} {{{3}}}]", time.ToString("HH:mm:ss.fff"), methodName, execMsg, traceMsg);

						/*-- 寫至檔案 --*/
						using (StreamWriter sw = new StreamWriter(file, true, Encoding.UTF8)) {
							sw.WriteLine(content);
						}
					}
				}
			);

			return tsk;
		}

		/// <summary>使用自定義的重疊圖像集合搭配特定的影像來源繪製成圖片檔案</summary>
		/// <param name="drawObj">欲繪製的重疊圖像集合</param>
		/// <param name="imgSrc">影像來源</param>
		/// <param name="fileName">欲儲存的圖片檔名，如 @"D:\Error.png"</param>
		public static void DrawCustomShape(List<OverlayMarkerObjectBase> drawObj, IVisionImageSource imgSrc, string fileName) {
			DrawObjects(fileName, imgSrc, drawObj);
		}
		#endregion
	}
	#endregion
}
