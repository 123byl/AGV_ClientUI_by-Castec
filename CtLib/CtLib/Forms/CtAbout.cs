using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Utility;

namespace CtLib.Forms {

	/// <summary>
	/// 「關於」視窗
	/// <para>用於顯示相關版本或是程式資訊</para>
	/// </summary>
	public partial class CtAbout : Form {

		/*------------------------------------
         * 
         * 各月份英文:
         *      一月:  January
         *      二月:  February
         *      三月:  March
         *      四月:  April
         *      五月:  May
         *      六月:  June
         *      七月:  July
         *      八月:  August
         *      九月:  September
         *      十月:  October
         *    十一月:  November
         *    十二月:  December
         *
         *------------------------------------*/

		#region Function - Drawing
		/// <summary>繪出橢圓矩形</summary>
		private void DrawRoundRectangle(object sender, PaintEventArgs e) {
			Graphics g = e.Graphics;
			Label label = sender as Label;

			float X = label.Width - 1;
			float Y = label.Height - 1;

			PointF[] points = {
				new PointF(2,     0),
				new PointF(X - 2, 0),
				new PointF(X - 1, 1),
				new PointF(X,     2),
				new PointF(X,     Y - 2),
				new PointF(X - 1, Y - 1),
				new PointF(X - 2, Y),
				new PointF(2,     Y),
				new PointF(1,     Y - 1),
				new PointF(0,     Y - 2),
				new PointF(0,     2),
				new PointF(1,     1)
			};

			GraphicsPath path = new GraphicsPath();
			path.AddLines(points);

			Pen pen = new Pen(Color.FromArgb(150, Color.Gray), 1);  //Alpha maximum is 255.
			pen.DashStyle = DashStyle.Solid;
			g.FillPath(Brushes.LightGray, path);
			g.DrawPath(pen, path);

			//'Cause using FillPath, so Text already covered.
			SizeF strSize = g.MeasureString(label.Text, label.Font);
			g.DrawString(label.Text, label.Font, new SolidBrush(label.ForeColor), CalculateCenter(strSize, label.Size));
		}

		/// <summary>計算文字的中心點位於控制項的位置</summary>
		/// <param name="stringSize">字串大小</param>
		/// <param name="labelSize">Label大小</param>
		/// <returns>相對於 Label 之座標</returns>
		private PointF CalculateCenter(SizeF stringSize, Size labelSize) {
			PointF point = new PointF();
			point.X = (labelSize.Width - stringSize.Width) / 2;
			point.Y = (labelSize.Height - stringSize.Height) / 2;
			return point;
		}
		#endregion

		#region Declaration - Definitions
		/// <summary>顯示模組名稱之 Label 字型</summary>
		private static readonly Font DEFAULT_INFO_FONT = new Font("Candara", 10);
		/// <summary>顯示模組版本號碼之 Label 字型</summary>
		private static readonly Font DEFAULT_VERSION_FONT = new Font("Consolas", 10);
		/// <summary>顯示模組名稱與號碼之 Label 大小</summary>
		private static readonly Size DEFAULT_SIZE = new Size(115, 22);
		/// <summary>模組名稱與版本號碼之間的間隙</summary>
		private static readonly int DEFAULT_GAP_INFO = 0;
		/// <summary>模組與模組之間的間隙</summary>
		private static readonly int DEFAULT_GAP_MODULE = 20;
		#endregion

		#region Declaration - Fields
		private string mBuildDate = string.Empty;
		private string mTitle = string.Empty;
		private string mDescription = string.Empty;
		private string mProduct = string.Empty;
		private string mVersion = string.Empty;
		private string mCopyright = string.Empty;
		private Dictionary<string, CtVersion> mModule;
		#endregion

		#region Declaration - Properties
		/// <summary>建立日期</summary>
		public string AppBuildDate { get { return mBuildDate; } }
		/// <summary>標題</summary>
		public string AppTitle { get { return mTitle; } }
		/// <summary>版權聲明</summary>
		public string AppCopyright { get { return mCopyright; } }
		/// <summary>描述</summary>
		public string AppDescription { get { return mDescription; } }
		/// <summary>產品名稱</summary>
		public string AppProductName { get { return mProduct; } }
		/// <summary>版本號</summary>
		public string AppVersion { get { return mVersion; } }
		/// <summary>各模組版本</summary>
		public Dictionary<string, CtVersion> AppModules { get { return new Dictionary<string, CtVersion>(mModule); } }
		#endregion

		#region Function - Constructor
		/// <summary>建立 About CAMPro 視窗</summary>
		public CtAbout() {
			InitializeComponent();

			mModule = new Dictionary<string, CtVersion>();
		}
		#endregion

		#region Function - Core
		/// <summary>開啟 About CAMPro 視窗，並顯示 CtLib 相關視窗</summary>
		/// <returns>Status Code</returns>
		public Stat Start() {
			Stat stt = Stat.SUCCESS;
			try {
				LoadAssemblyInfo();

				mBuildDate = GetAssemblyFileDate();

				ShowInformation();
				AdjustFormSize();
				this.ShowDialog();
			} catch (Exception ex) {
				stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		/// <summary>開啟 About CAMPro 視窗，包含 CtLib 組件訊息與指定的專案版本訊息</summary>
		/// <param name="ver">專案版本訊息</param>
		/// <returns>Status Code</returns>
		public Stat Start(CtVersion ver) {
			Stat stt = Stat.SUCCESS;
			try {
				LoadAssemblyInfo();

				mVersion = ver.FullString;
				mBuildDate = ver.Date.ToString("yyyy/MM/dd");

				ShowInformation();
				AdjustFormSize();
				this.ShowDialog();
			} catch (Exception ex) {
				stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		/// <summary>開啟 About CAMPro 視窗，包含 CtLib 組件訊息、指定的專案版本訊息與子模組訊息</summary>
		/// <param name="ver">主專案版本訊息</param>
		/// <param name="module">子模組版本訊息</param>
		/// <returns>Status Code</returns>
		public Stat Start(CtVersion ver, Dictionary<string, string> module) {
			Stat stt = Stat.SUCCESS;
			try {
				LoadAssemblyInfo();
				LoadModuleVersion(module);

				mVersion = ver.FullString;
				mBuildDate = ver.Date.ToString("yyyy/MM/dd");

				ShowInformation();
				AdjustFormSize();
				this.ShowDialog();
			} catch (Exception ex) {
				stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		/// <summary>開啟 About CAMPro 視窗，包含指定的組件訊息、專案版本訊息與子模組訊息</summary>
		/// <param name="obj">欲取得相關組件的物件</param>
		/// <param name="ver">主專案版本訊息</param>
		/// <returns>Status Code</returns>
		/// <example><code language="C#">
		/// CtAbout about = new CtAbout();
		/// CtVersion mVersion = new CtVersion(0, 0, 0, "2099/12/31");
		/// Dictionary&lt;string, string&gt; mModule = new Dictionary&lt;string, string&gt; { { "Adept ACE", "3.5.3.0" } };
		/// Stat stt = about.Start(Assembly.GetExecutingAssembly(), mVersion, mModule);
		/// </code></example>
		public Stat Start(object obj, CtVersion ver) {
			Stat stt = Stat.SUCCESS;
			try {
				LoadAssemblyInfo(obj);

				mVersion = ver.FullString;
				mBuildDate = ver.Date.ToString("yyyy/MM/dd");

				ShowInformation();
				AdjustFormSize();
				this.ShowDialog();
			} catch (Exception ex) {
				stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		/// <summary>開啟 About CAMPro 視窗，包含指定的組件訊息、專案版本訊息與子模組訊息</summary>
		/// <param name="asm">專案組譯訊息</param>
		/// <param name="ver">主專案版本訊息</param>
		/// <returns>Status Code</returns>
		/// <example><code language="C#">
		/// CtAbout about = new CtAbout();
		/// CtVersion mVersion = new CtVersion(0, 0, 0, "2099/12/31");
		/// Dictionary&lt;string, string&gt; mModule = new Dictionary&lt;string, string&gt; { { "Adept ACE", "3.5.3.0" } };
		/// Stat stt = about.Start(Assembly.GetExecutingAssembly(), mVersion, mModule);
		/// </code></example>
		public Stat Start(Assembly asm, CtVersion ver) {
			Stat stt = Stat.SUCCESS;
			try {
				LoadAssemblyInfo(asm);

				mVersion = ver.FullString;
				mBuildDate = ver.Date.ToString("yyyy/MM/dd");

				ShowInformation();
				AdjustFormSize();
				this.ShowDialog();
			} catch (Exception ex) {
				stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		/// <summary>開啟 About CAMPro 視窗，包含指定的組件訊息、專案版本訊息與子模組訊息</summary>
		/// <param name="asm">專案組譯訊息</param>
		/// <param name="ver">主專案版本訊息</param>
		/// <param name="module">子模組版本訊息</param>
		/// <returns>Status Code</returns>
		/// <example><code language="C#">
		/// CtAbout about = new CtAbout();
		/// CtVersion mVersion = new CtVersion(0, 0, 0, "2099/12/31");
		/// Dictionary&lt;string, string&gt; mModule = new Dictionary&lt;string, string&gt; { { "Adept ACE", "3.5.3.0" } };
		/// Stat stt = about.Start(Assembly.GetExecutingAssembly(), mVersion, mModule);
		/// </code></example>
		public Stat Start(Assembly asm, CtVersion ver, Dictionary<string, string> module) {
			Stat stt = Stat.SUCCESS;
			try {
				LoadAssemblyInfo(asm);
				LoadModuleVersion(module);

				mVersion = ver.FullString;
				mBuildDate = ver.Date.ToString("yyyy/MM/dd");

				ShowInformation();
				AdjustFormSize();
				this.ShowDialog();
			} catch (Exception ex) {
				stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		/// <summary>開啟 About CAMPro 視窗，包含 CtLib 組件訊息、指定的載入模組訊息與專案版本訊息</summary>
		/// <param name="obj">欲取得相關組件的物件</param>
		/// <param name="module">子模組版本訊息</param>
		/// <returns>Status Code</returns>
		/// <example><code language="C#">
		/// AseProject proj = new AseProject();
		/// CtAbout about = new CtAbout();
		/// Dictionary&lt;string, string&gt; mModule = new Dictionary&lt;string, string&gt; { { "Adept ACE", "3.5.3.0" } };
		/// Stat stt = about.Start(proj, mModule);
		/// </code></example>
		public Stat Start(object obj, Dictionary<string, string> module) {
			Stat stt = Stat.SUCCESS;
			try {
				LoadAssemblyInfo(obj);
				LoadModuleVersion(module);

				mBuildDate = GetAssemblyFileDate();

				ShowInformation();
				AdjustFormSize();
				this.ShowDialog();
			} catch (Exception ex) {
				stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		/// <summary>開啟 About CAMPro 視窗，包含指定的組件訊息、專案版本訊息與子模組訊息</summary>
		/// <param name="asmVerify">專案組譯訊息</param>
		/// <param name="asmVersion">其他組件組譯訊息</param>
		/// <param name="ver">主專案版本訊息</param>
		/// <param name="module">子模組版本訊息</param>
		/// <returns>Status Code</returns>
		/// <example><code language="C#">
		/// AseProject proj = new AseProject();
		/// CtAbout about = new CtAbout();
		/// CtVersion mVersion = new CtVersion(0, 0, 0, "2099/12/31");
		/// Dictionary&lt;string, string&gt; mModule = new Dictionary&lt;string, string&gt; { { "Adept ACE", "3.5.3.0" } };
		/// Stat stt = about.Start(Assembly.GetExecutingAssembly(), proj.GetType().Assembly, mVersion, mModule);
		/// </code></example>
		public Stat Start(Assembly asmVerify, Assembly asmVersion, CtVersion ver, Dictionary<string, string> module) {
			Stat stt = Stat.SUCCESS;
			try {
				LoadAssemblyInfo(asmVerify, asmVersion);
				LoadModuleVersion(module);

				mVersion = ver.FullString;
				mBuildDate = ver.Date.ToString("yyyy/MM/dd");

				ShowInformation();
				AdjustFormSize();
				this.ShowDialog();
			} catch (Exception ex) {
				stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		/// <summary>開啟 About CAMPro 視窗，包含指定的組件訊息、載入模組訊息與子模組訊息</summary>
		/// <param name="asmVerify">專案組譯訊息</param>
		/// <param name="obj">欲取得相關組件的物件</param>
		/// <returns>Status Code</returns>
		/// <example><code language="C#">
		/// AseProject proj = new AseProject();
		/// CtAbout about = new CtAbout();
		/// Dictionary&lt;string, string&gt; mModule = new Dictionary&lt;string, string&gt; { { "Adept ACE", "3.5.3.0" } };
		/// Stat stt = about.Start(Assembly.GetExecutingAssembly(), proj, mModule);
		/// </code></example>
		public Stat Start(Assembly asmVerify, object obj) {
			Stat stt = Stat.SUCCESS;
			try {
				LoadAssemblyInfo(asmVerify, obj);

				mBuildDate = GetAssemblyFileDate();

				ShowInformation();
				AdjustFormSize();
				this.ShowDialog();
			} catch (Exception ex) {
				stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		/// <summary>開啟 About CAMPro 視窗，包含指定的組件訊息、載入模組訊息與子模組訊息</summary>
		/// <param name="asmVerify">專案組譯訊息</param>
		/// <param name="obj">欲取得相關組件的物件</param>
		/// <param name="ver">主專案版本訊息</param>
		/// <returns>Status Code</returns>
		/// <example><code language="C#">
		/// AseProject proj = new AseProject();
		/// CtAbout about = new CtAbout();
		/// Dictionary&lt;string, string&gt; mModule = new Dictionary&lt;string, string&gt; { { "Adept ACE", "3.5.3.0" } };
		/// Stat stt = about.Start(Assembly.GetExecutingAssembly(), proj, mModule);
		/// </code></example>
		public Stat Start(Assembly asmVerify, object obj, CtVersion ver) {
			Stat stt = Stat.SUCCESS;
			try {
				LoadAssemblyInfo(asmVerify, obj);

				mVersion = ver.FullString;
				mBuildDate = ver.Date.ToString("yyyy/MM/dd");

				ShowInformation();
				AdjustFormSize();
				this.ShowDialog();
			} catch (Exception ex) {
				stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		/// <summary>開啟 About CAMPro 視窗，包含指定的組件訊息、載入模組訊息與子模組訊息</summary>
		/// <param name="asmVerify">專案組譯訊息</param>
		/// <param name="obj">欲取得相關組件的物件</param>
		/// <param name="module">子模組版本訊息</param>
		/// <returns>Status Code</returns>
		/// <example><code language="C#">
		/// AseProject proj = new AseProject();
		/// CtAbout about = new CtAbout();
		/// Dictionary&lt;string, string&gt; mModule = new Dictionary&lt;string, string&gt; { { "Adept ACE", "3.5.3.0" } };
		/// Stat stt = about.Start(Assembly.GetExecutingAssembly(), proj, mModule);
		/// </code></example>
		public Stat Start(Assembly asmVerify, object obj, Dictionary<string, string> module) {
			Stat stt = Stat.SUCCESS;
			try {
				LoadAssemblyInfo(asmVerify, obj);
				LoadModuleVersion(module);

				mBuildDate = GetAssemblyFileDate();

				ShowInformation();
				AdjustFormSize();
				this.ShowDialog();
			} catch (Exception ex) {
				stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		/// <summary>開啟 About CAMPro 視窗，包含指定的組件訊息、載入模組訊息與子模組訊息</summary>
		/// <param name="asmVerify">專案組譯訊息</param>
		/// <param name="obj">欲取得相關組件的物件</param>
		/// <param name="module">子模組版本訊息</param>
		/// <param name="ver">主專案版本訊息</param>
		/// <returns>Status Code</returns>
		/// <example><code language="C#">
		/// AseProject proj = new AseProject();
		/// CtAbout about = new CtAbout();
		/// Dictionary&lt;string, string&gt; mModule = new Dictionary&lt;string, string&gt; { { "Adept ACE", "3.5.3.0" } };
		/// Stat stt = about.Start(Assembly.GetExecutingAssembly(), proj, mModule);
		/// </code></example>
		public Stat Start(Assembly asmVerify, object obj, CtVersion ver, Dictionary<string, string> module) {
			Stat stt = Stat.SUCCESS;
			try {
				LoadAssemblyInfo(asmVerify, obj);
				LoadModuleVersion(module);

				mVersion = ver.FullString;
				mBuildDate = ver.Date.ToString("yyyy/MM/dd");

				ShowInformation();
				AdjustFormSize();
				this.ShowDialog();
			} catch (Exception ex) {
				stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		/// <summary>開啟 About CAMPro 視窗，以自訂的標題、版權資訊等為主</summary>
		/// <param name="obj">欲載入引用含有 <see cref="CtVersion"/> 類別的來源物件</param>
		/// <param name="ver">欲顯示版本與日期的 <see cref="CtVersion"/></param>
		/// <param name="module">欲顯示於右側模組資訊的集合</param>
		/// <param name="title">標題</param>
		/// <param name="product">產品名稱</param>
		/// <param name="copyright">版權所有</param>
		/// <param name="descrip">描述資訊</param>
		/// <returns>Status Code</returns>
		public Stat Start(object obj, CtVersion ver, Dictionary<string, string> module, string title, string product, string copyright, string descrip) {
			Stat stt = Stat.SUCCESS;
			try {
				LoadUseClass(obj);
				LoadModuleVersion(module);

				mVersion = ver.FullString;
				mBuildDate = ver.Date.ToString("yyyy/MM/dd");
				mTitle = title;
				mProduct = product;
				mCopyright = copyright;
				mDescription = descrip;

				ShowInformation();
				AdjustFormSize();
				this.ShowDialog();
			} catch (Exception ex) {
				stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}
		#endregion

		#region Function - Method

		private string GetAssemblyFileDate() {
			DateTime time = CtFile.GetFileInformation(Application.ExecutablePath).LastEditTime;
			return time.ToString("yyyy/MM/dd");
		}

		/// <summary>讀取 <see cref="Assembly"/> 以取得組件版權、組件版本等資訊，並儲存至全域變數</summary>
		/// <param name="asm">欲讀取的 <see cref="Assembly"/></param>
		private void SetAttributes(Assembly asm) {
			Array.ForEach(
				asm.GetCustomAttributes(false),
				attr => {
					if (attr is AssemblyCopyrightAttribute)
						mCopyright = (attr as AssemblyCopyrightAttribute).Copyright;
					else if (attr is AssemblyTitleAttribute)
						mTitle = (attr as AssemblyTitleAttribute).Title;
					else if (attr is AssemblyVersionAttribute)
						mVersion = (attr as AssemblyVersionAttribute).Version;
					else if (attr is AssemblyDescriptionAttribute)
						mDescription = (attr as AssemblyDescriptionAttribute).Description;
					else if (attr is AssemblyProductAttribute)
						mProduct = (attr as AssemblyProductAttribute).Product;
				}
			);
		}

		/// <summary>從 <see cref="Type"/> 撈取 <see cref="CtVersion"/> 並加入清單</summary>
		/// <param name="item">欲撈取的 <see cref="Type"/></param>
		private void GetCtVersion(Type item) {
			Type verType = typeof(CtVersion);
			BindingFlags bindFlag = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
			if (item.IsClass) {
				var props = item.GetProperties(bindFlag).Where(prop => prop.PropertyType == verType);
				if (props.Any()) {
					props.ForEach(
						prop => {
							try {
								object inst = Activator.CreateInstance(item);
								mModule.Add(item.Name, prop.GetValue(inst) as CtVersion);
								(inst as IDisposable)?.Dispose();
							} catch (Exception) {
									/*-- 防止有些 Class 沒有空建構元，不做事 --*/
							}
						}
					);
				}

				var fields = item.GetFields(bindFlag).Where(field => field.FieldType == verType);
				if (fields.Any()) {
					fields.ForEach(
						field => {
							try {
								object inst = Activator.CreateInstance(item);
								mModule.Add(item.Name, field.GetValue(inst) as CtVersion);
								(inst as IDisposable)?.Dispose();
							} catch (Exception) {
									/*-- 防止有些 Class 沒有空建構元，不做事 --*/
							}
						}
					);
				}
			}
		}

		/// <summary>從 <see cref="Assembly"/> 撈取 <see cref="CtVersion"/> 並加入清單</summary>
		/// <param name="asm">欲讀取的 <see cref="Assembly"/></param>
		private void GetCtVersion(Assembly asm) {
			asm.GetTypes().ForEach(tp => GetCtVersion(tp));
		}

		/// <summary>載入 CtLib 組件，並取得相關組件資訊</summary>
		private void LoadAssemblyInfo() {
			Assembly asm = Assembly.GetExecutingAssembly();
			SetAttributes(asm);
			GetCtVersion(asm);
		}

		/// <summary>載入指定的組件，並取得相關組件資訊</summary>
		private void LoadAssemblyInfo(Assembly asm) {
			SetAttributes(asm);
			GetCtVersion(asm);
		}

		/// <summary>載入指定的授權版本等資訊組件，及欲顯示 <see cref="CtVersion"/> 之組件</summary>
		private void LoadAssemblyInfo(Assembly asmInfo, Assembly asmVer) {
			SetAttributes(asmInfo);
			GetCtVersion(asmVer);
		}

		/// <summary>載入物件所使用到的類別，以含有 <see cref="CtVersion"/> 的物件為主</summary>
		/// <param name="obj">欲載入的物件</param>
		private void LoadUseClass(object obj) {
			var fields = obj
							.GetType()
							.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
							.Where(data => data.FieldType.IsClass);

			foreach (var info in fields) {
				GetCtVersion(info.FieldType);
			}
		}

		/// <summary>以傳入物件之組件以顯示 <see cref="CtVersion"/> 與授權資訊</summary>
		/// <param name="obj">欲顯示的物件</param>
		private void LoadAssemblyInfo(object obj) {
			SetAttributes(obj.GetType().Assembly);
			LoadUseClass(obj);
		}

		/// <summary>以傳入的組件作為版本授權資訊，並以傳入物件作為顯示 <see cref="CtVersion"/> 之訊息</summary>
		/// <param name="asm">欲顯示版權等等資訊的 <see cref="Assembly"/></param>
		/// <param name="obj">欲載入的物件</param>
		private void LoadAssemblyInfo(Assembly asm, object obj) {
			SetAttributes(asm);
			LoadUseClass(obj);
		}

		/// <summary>加入子模組版本至右側</summary>
		/// <param name="module">模組訊息</param>
		private void LoadModuleVersion(Dictionary<string, string> module) {
			int x = 40;
			int y = 15 - DEFAULT_SIZE.Height - DEFAULT_GAP_MODULE;
			int idx = 0;

			foreach (KeyValuePair<string, string> mod in module) {
				Label lbMod = new Label();
				lbMod.Parent = pnModule;
				lbMod.Font = DEFAULT_INFO_FONT;
				lbMod.AutoSize = false;
				lbMod.Size = DEFAULT_SIZE;
				lbMod.TextAlign = ContentAlignment.MiddleLeft;
				y += DEFAULT_SIZE.Height + DEFAULT_GAP_MODULE;
				lbMod.Location = new Point(x, y);
				lbMod.Text = mod.Key;
				lbMod.Show();

				Label lbVer = new Label();
				lbVer.Paint += DrawRoundRectangle;
				lbVer.Parent = pnModule;
				lbVer.Font = DEFAULT_VERSION_FONT;
				lbVer.AutoSize = false;
				lbVer.Size = DEFAULT_SIZE;
				y += DEFAULT_SIZE.Height + DEFAULT_GAP_INFO;
				lbVer.Location = new Point(x, y);
				lbVer.Text = mod.Value;
				lbVer.Show();

				idx++;
			}

		}

		/// <summary>顯示相關訊息</summary>
		private void ShowInformation() {
			CtInvoke.ControlText(this, mTitle);
			CtInvoke.ControlText(lbProduct, mProduct);
			CtInvoke.ControlText(lbCopyright, mCopyright);
			CtInvoke.ControlText(lbVersion_val, mVersion);
			CtInvoke.ControlText(lbDate_val, mBuildDate);
			var ordMod = mModule.OrderBy(kvp => kvp.Key);
			foreach (KeyValuePair<string, CtVersion> item in ordMod) {
				CtInvoke.DataGridViewAddRow(dgvModule, new List<string> { item.Key, item.Value.ToString() }, false, false);
			}

			txtDescription.InvokeIfNecessary(
				() => {
					var size = TextRenderer.MeasureText(
								mDescription,
								txtDescription.Font,
								new Size(txtDescription.Width, int.MaxValue),
								TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);

					if (size.Height > txtDescription.Height) txtDescription.ScrollBars = ScrollBars.Vertical;
					else txtDescription.ScrollBars = ScrollBars.None;
					txtDescription.Text = mDescription;
				}
			);
		}

		/// <summary>根據訊息長度改變視窗大小</summary>
		private void AdjustFormSize() {
			int width = 20;

			if (lbProduct.Left + lbProduct.Width >= lbCopyright.Left + lbCopyright.Width) {
				width += lbProduct.Right + pnModule.Width;
			} else {
				width += lbCopyright.Right + pnModule.Width;
			}

			if (width > Width) CtInvoke.ControlWidth(this, width);
		}
		#endregion
	}
}
