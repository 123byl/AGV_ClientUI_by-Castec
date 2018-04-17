using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Utility;

namespace CtLib.Forms {
	internal partial class CtMessage_Ctrl : Form {

		#region Declaration - Definitions
		/// <summary>預設的文字訊息起始座標</summary>
		private static readonly Point DEFAULT_LAB_LOCATION = new Point(20, 20);
		/// <summary>預設的按鈕大小</summary>
		private static readonly Size DEFAULT_BTN_SIZE = new Size(110, 50);
		/// <summary>預設的每顆按鈕之間的距離</summary>
		private static readonly int DEFAULT_BTN_GAP = 10;
		/// <summary>預設的按鈕邊框顏色</summary>
		private static readonly Color DEFAULT_BTN_BORDER = Color.DimGray;
		/// <summary>預設的滑鼠按下時顏色</summary>
		private static readonly Color DEFAULT_BTN_MOUSEDOWN = Color.DeepSkyBlue;
		/// <summary>預設的滑鼠移過時顏色</summary>
		private static readonly Color DEFAULT_BTN_MOUSEOVER = Color.LightSkyBlue;
		/// <summary>預設的表單與訊息範圍。Width = Label 最大寬度  | Height = 最小的 Form 保證長度</summary>
		private static readonly Size DEFAULT_MIN_SIZE = new Size(380, 350);
		#endregion

		#region Declaration - Fields
		/// <summary>用於暫存使用者所指定的文字</summary>
		private List<string> mMsg = new List<string>();
		/// <summary>文字訊息字體</summary>
		private Font mLabelFont;
		/// <summary>關閉視窗之取消旗標</summary>
		private CancellationTokenSource mCncTokSrc;
		/// <summary>動態產生的文字訊息</summary>
		private Label mLabel;
		/// <summary>動態產生的選項按鈕</summary>
		private List<Button> mButtons = new List<Button>();

		/// <summary>按鈕</summary>
		private MsgBoxBtn mUIBtn = MsgBoxBtn.No;
		/// <summary>對話視窗圖樣</summary>
		private MsgBoxStyle mMsgBoxStyle = MsgBoxStyle.None;

		/// <summary>愈顯示的多文化語系</summary>
		private UILanguage mCulture = UILanguage.TraditionalChinese;
		/// <summary>按鈕多語系文字</summary>
		private Dictionary<MsgBoxBtn, Dictionary<UILanguage, string>> mBtnText = new Dictionary<MsgBoxBtn, Dictionary<UILanguage, string>> {
			{
				MsgBoxBtn.Cancel,
				new Dictionary<UILanguage, string> {
					{ UILanguage.English, "Cancel" },
					{ UILanguage.SimplifiedChinese, "取消" },
					{ UILanguage.TraditionalChinese, "取消" }
				}
			},
			{
				MsgBoxBtn.No,
				new Dictionary<UILanguage, string> {
					{ UILanguage.English, "No" },
					{ UILanguage.SimplifiedChinese, "否" },
					{ UILanguage.TraditionalChinese, "否" }
				}
			},
			{
				MsgBoxBtn.OK,
				new Dictionary<UILanguage, string> {
					{ UILanguage.English, "OK" },
					{ UILanguage.SimplifiedChinese, "确认" },
					{ UILanguage.TraditionalChinese, "確認" }
				}
			},
			{
				MsgBoxBtn.Yes,
				new Dictionary<UILanguage, string> {
					{ UILanguage.English, "Yes" },
					{ UILanguage.SimplifiedChinese, "是" },
					{ UILanguage.TraditionalChinese, "是" }
				}
			}
		};
		#endregion

		#region Declaration - Properties
		/// <summary>取得最後使用者按下的按鈕</summary>
		public MsgBoxBtn UIResult { get { return mUIBtn; } }
		#endregion

		#region Function - Constructors

		/// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
		/// <param name="title">標題文字</param>
		/// <param name="message">欲顯示的訊息</param>
		/// <param name="btn">按鈕集合，可用 OR 計算</param>
		/// <param name="style">介面樣式</param>
		public CtMessage_Ctrl(string title, string message, MsgBoxBtn btn = MsgBoxBtn.OK, MsgBoxStyle style = MsgBoxStyle.None) {
			InitializeComponent();

			mCulture = CtLanguage.GetUiLangByCult();

			Text = title;
			mMsg.Add(message);

			mLabelFont = GetFontByCulture(mCulture, 14);

			GenerateLabel(mMsg);
			GenerateButtons(btn);

			mUIBtn = btn;
			mMsgBoxStyle = style;

			SetFormLocation();
            this.TopMost = false;
            this.TopMost = true;
            this.BringToFront();
		}

		/// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">標題文字</param>
		/// <param name="message">欲顯示的訊息</param>
		/// <param name="btn">按鈕集合，可用 OR 計算</param>
		/// <param name="style">介面樣式</param>
		public CtMessage_Ctrl(UILanguage lang, string title, string message, MsgBoxBtn btn = MsgBoxBtn.OK, MsgBoxStyle style = MsgBoxStyle.None) {
			InitializeComponent();

			mCulture = lang;

			Text = title;
			mMsg.Add(message);

			mLabelFont = GetFontByCulture(mCulture, 14);

			GenerateLabel(mMsg);
			GenerateButtons(btn);

			mUIBtn = btn;
			mMsgBoxStyle = style;

			SetFormLocation();
		}

		/// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
		/// <param name="title">標題文字</param>
		/// <param name="message">欲顯示的訊息</param>
		/// <param name="fontSize">字體大小</param>
		/// <param name="btn">按鈕集合，可用 OR 計算</param>
		/// <param name="style">介面樣式</param>
		public CtMessage_Ctrl(string title, string message, float fontSize, MsgBoxBtn btn = MsgBoxBtn.OK, MsgBoxStyle style = MsgBoxStyle.None) {
			InitializeComponent();

			mCulture = CtLanguage.GetUiLangByCult();

			Text = title;
			mMsg.Add(message);

			mLabelFont = GetFontByCulture(mCulture, fontSize);

			GenerateLabel(mMsg);
			GenerateButtons(btn);

			mUIBtn = btn;
			mMsgBoxStyle = style;

			SetFormLocation();
		}

		/// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">標題文字</param>
		/// <param name="message">欲顯示的訊息</param>
		/// <param name="fontSize">字體大小</param>
		/// <param name="btn">按鈕集合，可用 OR 計算</param>
		/// <param name="style">介面樣式</param>
		public CtMessage_Ctrl(UILanguage lang, string title, string message, float fontSize, MsgBoxBtn btn = MsgBoxBtn.OK, MsgBoxStyle style = MsgBoxStyle.None) {
			InitializeComponent();

			mCulture = lang;

			Text = title;
			mMsg.Add(message);

			mLabelFont = GetFontByCulture(mCulture, fontSize);

			GenerateLabel(mMsg);
			GenerateButtons(btn);

			mUIBtn = btn;
			mMsgBoxStyle = style;

			SetFormLocation();
		}

		/// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
		/// <param name="title">標題文字</param>
		/// <param name="message">欲顯示的訊息</param>
		/// <param name="fontName">字型名稱</param>
		/// <param name="fontSize">字體大小</param>
		/// <param name="btn">按鈕集合，可用 OR 計算</param>
		/// <param name="style">介面樣式</param>
		public CtMessage_Ctrl(string title, string message, string fontName, float fontSize, MsgBoxBtn btn = MsgBoxBtn.OK, MsgBoxStyle style = MsgBoxStyle.None) {
			InitializeComponent();

			mCulture = CtLanguage.GetUiLangByCult();

			Text = title;
			mMsg.Add(message);

			mLabelFont = GenerateFont(fontName, fontSize);

			GenerateLabel(mMsg);
			GenerateButtons(btn);

			mUIBtn = btn;
			mMsgBoxStyle = style;

			SetFormLocation();
		}

		/// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">標題文字</param>
		/// <param name="message">欲顯示的訊息</param>
		/// <param name="fontName">字型名稱</param>
		/// <param name="fontSize">字體大小</param>
		/// <param name="btn">按鈕集合，可用 OR 計算</param>
		/// <param name="style">介面樣式</param>
		public CtMessage_Ctrl(UILanguage lang, string title, string message, string fontName, float fontSize, MsgBoxBtn btn = MsgBoxBtn.OK, MsgBoxStyle style = MsgBoxStyle.None) {
			InitializeComponent();

			mCulture = lang;

			Text = title;
			mMsg.Add(message);

			mLabelFont = GenerateFont(fontName, fontSize);

			GenerateLabel(mMsg);
			GenerateButtons(btn);

			mUIBtn = btn;
			mMsgBoxStyle = style;

			SetFormLocation();
		}

		/// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
		/// <param name="title">標題文字</param>
		/// <param name="message">欲顯示的訊息</param>
		/// <param name="font">自訂字體物件</param>
		/// <param name="btn">按鈕集合，可用 OR 計算</param>
		/// <param name="style">介面樣式</param>
		public CtMessage_Ctrl(string title, string message, Font font, MsgBoxBtn btn = MsgBoxBtn.OK, MsgBoxStyle style = MsgBoxStyle.None) {
			InitializeComponent();

			mCulture = CtLanguage.GetUiLangByCult();

			Text = title;
			mMsg.Add(message);

			mLabelFont = font;

			GenerateLabel(mMsg);
			GenerateButtons(btn);

			mUIBtn = btn;
			mMsgBoxStyle = style;

			SetFormLocation();
		}

		/// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">標題文字</param>
		/// <param name="message">欲顯示的訊息</param>
		/// <param name="font">自訂字體物件</param>
		/// <param name="btn">按鈕集合，可用 OR 計算</param>
		/// <param name="style">介面樣式</param>
		public CtMessage_Ctrl(UILanguage lang, string title, string message, Font font, MsgBoxBtn btn = MsgBoxBtn.OK, MsgBoxStyle style = MsgBoxStyle.None) {
			InitializeComponent();

			mCulture = lang;

			Text = title;
			mMsg.Add(message);

			mLabelFont = font;

			GenerateLabel(mMsg);
			GenerateButtons(btn);

			mUIBtn = btn;
			mMsgBoxStyle = style;

			SetFormLocation();
		}

		/// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
		/// <param name="title">標題文字</param>
		/// <param name="message">欲顯示的訊息</param>
		/// <param name="btn">按鈕集合，可用 OR 計算</param>
		/// <param name="style">介面樣式</param>
		public CtMessage_Ctrl(string title, IEnumerable<string> message, MsgBoxBtn btn = MsgBoxBtn.OK, MsgBoxStyle style = MsgBoxStyle.None) {
			InitializeComponent();

			mCulture = CtLanguage.GetUiLangByCult();

			Text = title;
			mMsg.AddRange(message);

			mLabelFont = GetFontByCulture(mCulture, 14);

			GenerateLabel(mMsg);
			GenerateButtons(btn);

			mUIBtn = btn;
			mMsgBoxStyle = style;

			SetFormLocation();
		}

		/// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">標題文字</param>
		/// <param name="message">欲顯示的訊息</param>
		/// <param name="btn">按鈕集合，可用 OR 計算</param>
		/// <param name="style">介面樣式</param>
		public CtMessage_Ctrl(UILanguage lang, string title, IEnumerable<string> message, MsgBoxBtn btn = MsgBoxBtn.OK, MsgBoxStyle style = MsgBoxStyle.None) {
			InitializeComponent();

			mCulture = lang;

			Text = title;
			mMsg.AddRange(message);

			mLabelFont = GetFontByCulture(mCulture, 14);

			GenerateLabel(mMsg);
			GenerateButtons(btn);

			mUIBtn = btn;
			mMsgBoxStyle = style;

			SetFormLocation();
		}

		/// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
		/// <param name="title">標題文字</param>
		/// <param name="message">欲顯示的訊息</param>
		/// <param name="fontSize">字體大小</param>
		/// <param name="btn">按鈕集合，可用 OR 計算</param>
		/// <param name="style">介面樣式</param>
		public CtMessage_Ctrl(string title, IEnumerable<string> message, float fontSize, MsgBoxBtn btn = MsgBoxBtn.OK, MsgBoxStyle style = MsgBoxStyle.None) {
			InitializeComponent();

			mCulture = CtLanguage.GetUiLangByCult();

			Text = title;
			mMsg.AddRange(message);

			mLabelFont = GetFontByCulture(mCulture, fontSize);

			GenerateLabel(mMsg);
			GenerateButtons(btn);

			mUIBtn = btn;
			mMsgBoxStyle = style;

			SetFormLocation();
		}

		/// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">標題文字</param>
		/// <param name="message">欲顯示的訊息</param>
		/// <param name="fontSize">字體大小</param>
		/// <param name="btn">按鈕集合，可用 OR 計算</param>
		/// <param name="style">介面樣式</param>
		public CtMessage_Ctrl(UILanguage lang, string title, IEnumerable<string> message, float fontSize, MsgBoxBtn btn = MsgBoxBtn.OK, MsgBoxStyle style = MsgBoxStyle.None) {
			InitializeComponent();

			mCulture = lang;

			Text = title;
			mMsg.AddRange(message);

			mLabelFont = GetFontByCulture(mCulture, fontSize);

			GenerateLabel(mMsg);
			GenerateButtons(btn);

			mUIBtn = btn;
			mMsgBoxStyle = style;

			SetFormLocation();
		}

		/// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
		/// <param name="title">標題文字</param>
		/// <param name="message">欲顯示的訊息</param>
		/// <param name="fontName">字型名稱</param>
		/// <param name="fontSize">字體大小</param>
		/// <param name="btn">按鈕集合，可用 OR 計算</param>
		/// <param name="style">介面樣式</param>
		public CtMessage_Ctrl(string title, IEnumerable<string> message, string fontName, float fontSize, MsgBoxBtn btn = MsgBoxBtn.OK, MsgBoxStyle style = MsgBoxStyle.None) {
			InitializeComponent();

			mCulture = CtLanguage.GetUiLangByCult();

			Text = title;
			mMsg.AddRange(message);

			mLabelFont = GenerateFont(fontName, fontSize);

			GenerateLabel(mMsg);
			GenerateButtons(btn);

			mUIBtn = btn;
			mMsgBoxStyle = style;

			SetFormLocation();
		}

		/// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">標題文字</param>
		/// <param name="message">欲顯示的訊息</param>
		/// <param name="fontName">字型名稱</param>
		/// <param name="fontSize">字體大小</param>
		/// <param name="btn">按鈕集合，可用 OR 計算</param>
		/// <param name="style">介面樣式</param>
		public CtMessage_Ctrl(UILanguage lang, string title, IEnumerable<string> message, string fontName, float fontSize, MsgBoxBtn btn = MsgBoxBtn.OK, MsgBoxStyle style = MsgBoxStyle.None) {
			InitializeComponent();

			mCulture = lang;

			Text = title;
			mMsg.AddRange(message);

			mLabelFont = GenerateFont(fontName, fontSize);

			GenerateLabel(mMsg);
			GenerateButtons(btn);

			mUIBtn = btn;
			mMsgBoxStyle = style;

			SetFormLocation();
		}

		/// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
		/// <param name="title">標題文字</param>
		/// <param name="message">欲顯示的訊息</param>
		/// <param name="font">自訂字體物件</param>
		/// <param name="btn">按鈕集合，可用 OR 計算</param>
		/// <param name="style">介面樣式</param>
		public CtMessage_Ctrl(string title, IEnumerable<string> message, Font font, MsgBoxBtn btn = MsgBoxBtn.OK, MsgBoxStyle style = MsgBoxStyle.None) {
			InitializeComponent();

			mCulture = CtLanguage.GetUiLangByCult();

			Text = title;
			mMsg.AddRange(message);

			mLabelFont = font;

			GenerateLabel(mMsg);
			GenerateButtons(btn);

			mUIBtn = btn;
			mMsgBoxStyle = style;

			SetFormLocation();
		}

		/// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
		/// <param name="lang">愈顯示的按鈕多文化語系</param>
		/// <param name="title">標題文字</param>
		/// <param name="message">欲顯示的訊息</param>
		/// <param name="font">自訂字體物件</param>
		/// <param name="btn">按鈕集合，可用 OR 計算</param>
		/// <param name="style">介面樣式</param>
		public CtMessage_Ctrl(UILanguage lang, string title, IEnumerable<string> message, Font font, MsgBoxBtn btn = MsgBoxBtn.OK, MsgBoxStyle style = MsgBoxStyle.None) {
			InitializeComponent();

			mCulture = lang;

			Text = title;
			mMsg.AddRange(message);

			mLabelFont = font;

			GenerateLabel(mMsg);
			GenerateButtons(btn);

			mUIBtn = btn;
			mMsgBoxStyle = style;

			SetFormLocation();
		}

		#endregion

		#region Function - Overloads
		/// <summary>將表單顯示為強制回應對話方塊，並可透過取消旗標以關閉視窗</summary>
		/// <param name="cncSrc">取消旗標</param>
		/// <returns>其中一個 <see cref="DialogResult"/> 值</returns>
		/// <exception cref="InvalidOperationException">非法的操作</exception>
		internal DialogResult ShowDialog(CancellationTokenSource cncSrc) {
			DialogResult result = DialogResult.None;
			mCncTokSrc = cncSrc;
			if (cncSrc != null) Task.Run(
				() => {
					do {
						try {
							if (mCncTokSrc.IsCancellationRequested) {
								CtInvoke.FormClose(this);
								break;
							} else Thread.Sleep(10);
						} catch (Exception ex) {
							CtStatus.Report(Stat.ER_SYSTEM, ex);
						}
					} while (true);
				}
			);
			result = this.ShowDialog();
			return result;
		}

		/// <summary>以指定的擁有人將表單顯示為強制回應對話方塊，並可透過取消旗標以關閉視窗</summary>
		/// <param name="owner">實作 <see cref="IWin32Window"/> 的任何物件，代表將擁有強制回應對話方塊的最上層視窗</param>
		/// <param name="cncSrc">取消旗標</param>
		/// <returns>其中一個 <see cref="DialogResult"/> 值</returns>
		/// <exception cref="ArgumentException">owner 有誤</exception>
		/// <exception cref="InvalidOperationException">非法的操作</exception>
		internal DialogResult ShowDialog(IWin32Window owner, CancellationTokenSource cncSrc) {
			DialogResult result = DialogResult.None;
			mCncTokSrc = cncSrc;
			if (cncSrc != null) Task.Run(
				() => {
					do {
						try {
							if (mCncTokSrc.IsCancellationRequested) {
								CtInvoke.FormClose(this);
								break;
							} else Thread.Sleep(10);
						} catch (Exception ex) {
							CtStatus.Report(Stat.ER_SYSTEM, ex);
						}
					} while (true);
				}
			);
			result = this.ShowDialog(owner);
			return result;
		}
		#endregion

		#region Function - Methods

		/// <summary>根據文化特性來建構 <see cref="Font"/>，預設值於方法內</summary>
		/// <param name="lang">欲建構字體的文化語系</param>
		/// <param name="fontSize">字體大小</param>
		/// <returns>相對應文化語系的字型</returns>
		private Font GetFontByCulture(UILanguage lang, float fontSize) {
			string fontFamily = string.Empty;
			switch (lang) {
				case UILanguage.English:
					fontFamily = "Verdana";
					break;

				case UILanguage.SimplifiedChinese:
				case UILanguage.TraditionalChinese:
				default:
					fontFamily = "微軟正黑體";
					break;
			}
			return new Font(fontFamily, fontSize);
		}

		/// <summary>產生字體物件，自訂字型名稱與大小</summary>
		/// <param name="fontName">字型名稱</param>
		/// <param name="fontSize">字型大小</param>
		/// <returns>產生的字體</returns>
		private Font GenerateFont(string fontName, float fontSize) {
			return new Font(
				fontName,
				fontSize
			);
		}

		/// <summary>計算按鈕組的位置</summary>
		/// <param name="index">從右下往左下數的索引</param>
		/// <returns>座標</returns>
		private Point CalculateButtonPoint(byte index) {
			Point point = new Point();
			point.X = ClientSize.Width - index * (DEFAULT_BTN_SIZE.Width + DEFAULT_BTN_GAP);
			point.Y = ClientSize.Height - DEFAULT_BTN_SIZE.Height - DEFAULT_BTN_GAP;
			return point;
		}

		/// <summary>設定按鈕的樣式，以 Flat 為主</summary>
		/// <param name="btn">欲修改樣式的按鈕</param>
		private void SetButtonStyle(Button btn) {
			btn.FlatStyle = FlatStyle.Flat;
			btn.FlatAppearance.BorderColor = DEFAULT_BTN_BORDER;
			btn.FlatAppearance.MouseDownBackColor = DEFAULT_BTN_MOUSEDOWN;
			btn.FlatAppearance.MouseOverBackColor = DEFAULT_BTN_MOUSEOVER;
			btn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		}

		/// <summary>建立單一新按鈕，並加入 mButtons 集合</summary>
		/// <param name="index">從右下往左下數的索引</param>
		/// <param name="text">欲顯示的文字</param>
		/// <param name="image">搭配的圖案</param>
		/// <param name="uiBtn">欲附加至 Tag 的 MsgBoxBtn</param>
		/// <returns>按鈕</returns>
		private Button CreateButton(byte index, string text, Image image, MsgBoxBtn uiBtn) {
			Button btn = new Button();
			btn.Parent = this;
			btn.Text = text;
			btn.Font = GetFontByCulture(mCulture, 12);
			btn.TextAlign = ContentAlignment.MiddleRight;
			btn.Image = image;
			btn.ImageAlign = ContentAlignment.MiddleLeft;
			btn.Size = DEFAULT_BTN_SIZE;
			btn.Location = CalculateButtonPoint(index);
			btn.Tag = uiBtn;
			btn.Click += Button_Click;
			SetButtonStyle(btn);
			mButtons.Add(btn);
			return btn;
		}

		/// <summary>產生按鈕組</summary>
		/// <param name="uiBtn">按鈕組參數</param>
		private void GenerateButtons(MsgBoxBtn uiBtn) {
			byte index = 1; //Start from 1

			if ((uiBtn & MsgBoxBtn.Cancel) == MsgBoxBtn.Cancel) {
				Button btn = CreateButton(index, mBtnText[MsgBoxBtn.Cancel][mCulture], Properties.Resources.Cancel, MsgBoxBtn.Cancel);
				btn.Show();
				index++;
			}

			if ((uiBtn & MsgBoxBtn.No) == MsgBoxBtn.No) {
				Button btn = CreateButton(index, mBtnText[MsgBoxBtn.No][mCulture], Properties.Resources.Cancel, MsgBoxBtn.No);
				btn.Show();
				index++;
			}

			if ((uiBtn & MsgBoxBtn.OK) == MsgBoxBtn.OK) {
				Button btn = CreateButton(index, mBtnText[MsgBoxBtn.OK][mCulture], Properties.Resources.Check_S, MsgBoxBtn.OK);
				btn.Show();
				index++;
			}

			if ((uiBtn & MsgBoxBtn.Yes) == MsgBoxBtn.Yes) {
				Button btn = CreateButton(index, mBtnText[MsgBoxBtn.Yes][mCulture], Properties.Resources.Check_S, MsgBoxBtn.Yes);
				btn.Show();
				index++;
			}
		}

		/// <summary>產生文字訊息</summary>
		/// <param name="msg">欲顯示的訊息</param>
		/// <remarks>於 Shown 事件才決定 Label.Location</remarks>
		private void GenerateLabel(List<string> msg) {
			Label label = new Label();
			label.Parent = this;
			label.MaximumSize = new Size(DEFAULT_MIN_SIZE.Width, 0);
			label.AutoSize = true;
			label.Font = mLabelFont;
			label.TextAlign = ContentAlignment.MiddleLeft;
			label.Text = string.Join(CtConst.NewLine, msg);

			if (mCulture == UILanguage.SimplifiedChinese) label.Text = CtConvert.ToSimplified(label.Text);
			/*else if (mCulture == UILanguage.TraditionalChinese) label.Text = CtConvert.ToTraditional(label.Text);*/

			label.Show();
			mLabel = label;
		}

		/// <summary>設定對話視窗起始位置</summary>
		/// <remarks>目前預設以主畫面為準之中心</remarks>
		private void SetFormLocation() {
			Screen screen = Screen.PrimaryScreen;
			Point point = new Point((screen.WorkingArea.Width - ClientSize.Width) / 2, (screen.WorkingArea.Height - ClientSize.Height) / 2);
			this.Location = point;
		}

		/// <summary>更改對話視窗之顯示螢幕，以該指定螢幕中心為準</summary>
		/// <param name="screenIndex"><see cref="Screen.AllScreens"/> 的螢幕索引</param>
		public void SetFormScreen(int screenIndex) {
			if (screenIndex > -1) {
				Screen screen = Screen.AllScreens[screenIndex];
				Point point = new Point(screen.Bounds.X + (screen.WorkingArea.Width - ClientSize.Width) / 2, screen.Bounds.Y + (screen.WorkingArea.Height - ClientSize.Height) / 2);
				this.InvokeIfNecessary(() => this.Location = point);
			} else SetFormScreen(Cursor.Position);
		}

		/// <summary>更改對話視窗之顯示螢幕，以該指定螢幕中心為準</summary>
		/// <param name="screen">欲顯示的螢幕</param>
		public void SetFormScreen(Screen screen) {
			Point point = new Point(screen.Bounds.X + (screen.WorkingArea.Width - ClientSize.Width) / 2, screen.Bounds.Y + (screen.WorkingArea.Height - ClientSize.Height) / 2);
			this.InvokeIfNecessary(() => this.Location = point);
		}

		/// <summary>更改對話視窗之顯示螢幕，以 <see cref="Point"/> 指定螢幕，並以其中心為準</summary>
		/// <param name="targetPoint">欲顯示的螢幕座標</param>
		public void SetFormScreen(Point targetPoint) {
			Screen screen = Screen.FromPoint(targetPoint);
			Point point = new Point(screen.Bounds.X + (screen.WorkingArea.Width - ClientSize.Width) / 2, screen.Bounds.Y + (screen.WorkingArea.Height - ClientSize.Height) / 2);
			this.InvokeIfNecessary(() => this.Location = point);
		}

		#endregion

		#region Function - Interface Events
		/// <summary>按鈕按下，回傳 MsgBoxBtn 並關閉視窗</summary>
		private void Button_Click(object sender, EventArgs e) {
			mUIBtn = (MsgBoxBtn)(sender as Button).Tag;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		/// <summary>在繪製 LOGO 時順便繪製樣式圖案</summary>
		private void pbLogo_Paint(object sender, PaintEventArgs e) {
			/*-- 如果不用圖案，直接結束掉 --*/
			if (mMsgBoxStyle == MsgBoxStyle.None) return;

			/*-- 取得 Graphic --*/
			Graphics g = e.Graphics;

			/*-- 根據 MsgBoxStyle 選擇相對應的圖 --*/
			Image img = null;
			switch (mMsgBoxStyle) {
				case MsgBoxStyle.Information:
					img = Properties.Resources.Info_3;
					break;
				case MsgBoxStyle.Warning:
					img = Properties.Resources.Warning_3;
					break;
				case MsgBoxStyle.Question:
					img = Properties.Resources.Question;
					break;
				case MsgBoxStyle.Error:
					img = Properties.Resources.Cancel_2;
					break;
			}

			/*-- 使用 DrawImage 直接畫上去，PNG 的透明色彩會保留! --*/
			g.DrawImage(
					img,
					(e.ClipRectangle.Width - img.Width) / 2,
					(e.ClipRectangle.Height - img.Height) / 2 - 20,
					img.Width,
					img.Height
			);
		}

		private void CtMessage_Ctrl_Shown(object sender, EventArgs e) {
			Application.DoEvents();

			/*-- 如果不用圖案，直接結束掉 --*/
			if (mMsgBoxStyle == MsgBoxStyle.None) return;

			/*-- 取得 Graphic --*/
			Graphics g = pbLogo.CreateGraphics();

			/*-- 根據 MsgBoxStyle 選擇相對應的圖 --*/
			Image img = null;
			switch (mMsgBoxStyle) {
				case MsgBoxStyle.Information:
					img = Properties.Resources.Info_3;
					break;
				case MsgBoxStyle.Warning:
					img = Properties.Resources.Warning_3;
					break;
				case MsgBoxStyle.Question:
					img = Properties.Resources.Question;
					break;
				case MsgBoxStyle.Error:
					img = Properties.Resources.Cancel_2;
					break;
			}

			/*-- 使用 DrawImage 直接畫上去，PNG 的透明色彩會保留! --*/
			g.DrawImage(
					img,
					(pbLogo.Width - img.Width) / 2,
					(pbLogo.Height - img.Height) / 2 - 20,
					img.Width,
					img.Height
			);

			/*-- 解掉 --*/
			g.Dispose();

			/*-- 更改 Label 位置，並確認 Form 高度與寬度 --*/
			int formHeight = this.Height;
			int formWidth = this.Width;
			mLabel.InvokeIfNecessary(
				() => {

					/* 計算 Label 到安全距離所需的長和寬 */
					int height = DEFAULT_LAB_LOCATION.Y + mLabel.Height + DEFAULT_BTN_SIZE.Height * 2 + DEFAULT_BTN_GAP * 2;
					int width = pbLogo.Right + DEFAULT_LAB_LOCATION.X;

					/* 如果計算完的長寬比預設的最小 Size 還大，改變 Form 的大小 */
					if (height > DEFAULT_MIN_SIZE.Height) {
						formHeight = height;
						mLabel.Location = new Point(width, DEFAULT_LAB_LOCATION.Y);
					} else if (height <= 200) {
						mLabel.Location = new Point(width, ClientSize.Height / 2 - mLabel.Height);
					} else {
						mLabel.Location = new Point(width, DEFAULT_LAB_LOCATION.Y);
					}

					formWidth = mLabel.Right + 85;
				}
			);

			/*-- 計算按鈕所需寬度 --*/
			int btnWidth = pbLogo.Right + 100 + mButtons.Max(btn => btn.Right) - mButtons.Min(btn => btn.Left);
			formWidth = Math.Max(btnWidth, formWidth);

			/*-- 更改 Form 大小 --*/
			this.InvokeIfNecessary(
				() => {
					this.Height = formHeight;
					this.Width = formWidth;
				}
			);
		}

		#endregion
	}
}
