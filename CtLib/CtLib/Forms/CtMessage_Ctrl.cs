using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CtLib.Library;

namespace CtLib.Forms {
    internal partial class CtMessage_Ctrl : Form {

        #region Declaration - Definitions
        /// <summary>預設的文字訊息字體</summary>
        private static readonly Font DEFAULT_LAB_FONT = new Font("微軟正黑體", 14);
        /// <summary>預設的文字訊息起始座標</summary>
        private static readonly Point DEFAULT_LAB_LOCATION = new Point(180, 20);
        /// <summary>預設的按鈕文字字體</summary>
        private static readonly Font DEFAULT_BTN_FONT = new Font("Century Gothic", 12);
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
        private static readonly Size DEFAULT_MIN_SIZE = new Size(325, 350);
        #endregion

        #region Declaration - Members
        /// <summary>用於暫存使用者所指定的文字</summary>
        private List<string> mMsg = new List<string>();
        /// <summary>文字訊息字體</summary>
        private Font mLabelFont;

        /// <summary>按鈕</summary>
        private MsgBoxButton mUIBtn = MsgBoxButton.NO;
        /// <summary>對話視窗圖樣</summary>
        private MsgBoxStyle mMsgBoxStyle = MsgBoxStyle.NONE;
        #endregion

        #region Declaration - Properties
        /// <summary>取得最後使用者按下的按鈕</summary>
        public MsgBoxButton UIResult { get { return mUIBtn; } }
        #endregion

        #region Function - Constructors

        /// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
        /// <param name="title">標題文字</param>
        /// <param name="message">欲顯示的訊息</param>
        /// <param name="btn">按鈕集合，可用 OR 計算</param>
        /// <param name="style">介面樣式</param>
        public CtMessage_Ctrl(string title, string message, MsgBoxButton btn = MsgBoxButton.OK, MsgBoxStyle style = MsgBoxStyle.NONE) {
            InitializeComponent();

            Text = title;
            mMsg.Add(message);

            mLabelFont = DEFAULT_LAB_FONT;

            GenerateLabel(mMsg);
            GenerateButtons(btn);

            mUIBtn = btn;
            mMsgBoxStyle = style;
        }

        /// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
        /// <param name="title">標題文字</param>
        /// <param name="message">欲顯示的訊息</param>
        /// <param name="fontSize">字體大小</param>
        /// <param name="btn">按鈕集合，可用 OR 計算</param>
        /// <param name="style">介面樣式</param>
        public CtMessage_Ctrl(string title, string message, float fontSize, MsgBoxButton btn = MsgBoxButton.OK, MsgBoxStyle style = MsgBoxStyle.NONE) {
            InitializeComponent();

            Text = title;
            mMsg.Add(message);

            mLabelFont = GenerateFont(fontSize);

            GenerateLabel(mMsg);
            GenerateButtons(btn);

            mUIBtn = btn;
            mMsgBoxStyle = style;
        }

        /// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
        /// <param name="title">標題文字</param>
        /// <param name="message">欲顯示的訊息</param>
        /// <param name="fontName">字型名稱</param>
        /// <param name="fontSize">字體大小</param>
        /// <param name="btn">按鈕集合，可用 OR 計算</param>
        /// <param name="style">介面樣式</param>
        public CtMessage_Ctrl(string title, string message, string fontName, float fontSize, MsgBoxButton btn = MsgBoxButton.OK, MsgBoxStyle style = MsgBoxStyle.NONE) {
            InitializeComponent();

            Text = title;
            mMsg.Add(message);

            mLabelFont = GenerateFont(fontName, fontSize);

            GenerateLabel(mMsg);
            GenerateButtons(btn);

            mUIBtn = btn;
            mMsgBoxStyle = style;
        }

        /// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
        /// <param name="title">標題文字</param>
        /// <param name="message">欲顯示的訊息</param>
        /// <param name="font">自訂字體物件</param>
        /// <param name="btn">按鈕集合，可用 OR 計算</param>
        /// <param name="style">介面樣式</param>
        public CtMessage_Ctrl(string title, string message, Font font, MsgBoxButton btn = MsgBoxButton.OK, MsgBoxStyle style = MsgBoxStyle.NONE) {
            InitializeComponent();

            Text = title;
            mMsg.Add(message);

            mLabelFont = font;

            GenerateLabel(mMsg);
            GenerateButtons(btn);

            mUIBtn = btn;
            mMsgBoxStyle = style;
        }

        /// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
        /// <param name="title">標題文字</param>
        /// <param name="message">欲顯示的訊息</param>
        /// <param name="btn">按鈕集合，可用 OR 計算</param>
        /// <param name="style">介面樣式</param>
        public CtMessage_Ctrl(string title, IEnumerable<string> message, MsgBoxButton btn = MsgBoxButton.OK, MsgBoxStyle style = MsgBoxStyle.NONE) {
            InitializeComponent();

            Text = title;
            mMsg.AddRange(message);

            mLabelFont = DEFAULT_LAB_FONT;

            GenerateLabel(mMsg);
            GenerateButtons(btn);

            mUIBtn = btn;
            mMsgBoxStyle = style;
        }

        /// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
        /// <param name="title">標題文字</param>
        /// <param name="message">欲顯示的訊息</param>
        /// <param name="fontSize">字體大小</param>
        /// <param name="btn">按鈕集合，可用 OR 計算</param>
        /// <param name="style">介面樣式</param>
        public CtMessage_Ctrl(string title, IEnumerable<string> message, float fontSize, MsgBoxButton btn = MsgBoxButton.OK, MsgBoxStyle style = MsgBoxStyle.NONE) {
            InitializeComponent();

            Text = title;
            mMsg.AddRange(message);

            mLabelFont = GenerateFont(fontSize);

            GenerateLabel(mMsg);
            GenerateButtons(btn);

            mUIBtn = btn;
            mMsgBoxStyle = style;
        }

        /// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
        /// <param name="title">標題文字</param>
        /// <param name="message">欲顯示的訊息</param>
        /// <param name="fontName">字型名稱</param>
        /// <param name="fontSize">字體大小</param>
        /// <param name="btn">按鈕集合，可用 OR 計算</param>
        /// <param name="style">介面樣式</param>
        public CtMessage_Ctrl(string title, IEnumerable<string> message, string fontName, float fontSize, MsgBoxButton btn = MsgBoxButton.OK, MsgBoxStyle style = MsgBoxStyle.NONE) {
            InitializeComponent();

            Text = title;
            mMsg.AddRange(message);

            mLabelFont = GenerateFont(fontName, fontSize);

            GenerateLabel(mMsg);
            GenerateButtons(btn);

            mUIBtn = btn;
            mMsgBoxStyle = style;
        }

        /// <summary>建立帶有標題、按鈕、樣式之對話視窗</summary>
        /// <param name="title">標題文字</param>
        /// <param name="message">欲顯示的訊息</param>
        /// <param name="font">自訂字體物件</param>
        /// <param name="btn">按鈕集合，可用 OR 計算</param>
        /// <param name="style">介面樣式</param>
        public CtMessage_Ctrl(string title, IEnumerable<string> message, Font font, MsgBoxButton btn = MsgBoxButton.OK, MsgBoxStyle style = MsgBoxStyle.NONE) {
            InitializeComponent();

            Text = title;
            mMsg.AddRange(message);

            mLabelFont = font;

            GenerateLabel(mMsg);
            GenerateButtons(btn);

            mUIBtn = btn;
            mMsgBoxStyle = style;
        }

        #endregion

        #region Function - Methods
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
        }

        /// <summary>建立單一新按鈕</summary>
        /// <param name="index">從右下往左下數的索引</param>
        /// <param name="text">欲顯示的文字</param>
        /// <param name="image">搭配的圖案</param>
        /// <param name="uiBtn">欲附加至 Tag 的 MsgBoxButton</param>
        /// <returns>按鈕</returns>
        private Button CreateButton(byte index, string text, Image image, MsgBoxButton uiBtn) {
            Button btn = new Button();
            btn.Parent = this;
            btn.Text = text;
            btn.Font = DEFAULT_BTN_FONT;
            btn.TextAlign = ContentAlignment.MiddleRight;
            btn.Image = image;
            btn.ImageAlign = ContentAlignment.MiddleLeft;
            btn.Size = DEFAULT_BTN_SIZE;
            btn.Location = CalculateButtonPoint(index);
            btn.Tag = uiBtn;
            btn.Click += Button_Click;
            SetButtonStyle(btn);
            return btn;
        }

        /// <summary>產生按鈕組</summary>
        /// <param name="uiBtn">按鈕組參數</param>
        private void GenerateButtons(MsgBoxButton uiBtn) {
            byte index = 1; //Start from 1

            if ((uiBtn & MsgBoxButton.CANCEL) == MsgBoxButton.CANCEL) {
                Button btn = CreateButton(index, "Cancel", Properties.Resources.Cancel, MsgBoxButton.OK);
                btn.Show();
                index++;
            }

            if ((uiBtn & MsgBoxButton.NO) == MsgBoxButton.NO) {
                Button btn = CreateButton(index, "No", Properties.Resources.Cancel, MsgBoxButton.NO);
                btn.Show();
                index++;
            }

            if ((uiBtn & MsgBoxButton.OK) == MsgBoxButton.OK) {
                Button btn = CreateButton(index, "OK", Properties.Resources.Check_S, MsgBoxButton.OK);
                btn.Show();
                index++;
            }

            if ((uiBtn & MsgBoxButton.YES) == MsgBoxButton.YES) {
                Button btn = CreateButton(index, "Yes", Properties.Resources.Check_S, MsgBoxButton.YES);
                btn.Show();
                index++;
            }
        }

        /// <summary>產生文字訊息</summary>
        /// <param name="msg">欲顯示的訊息</param>
        private void GenerateLabel(List<string> msg) {
            Label label = new Label();
            label.Parent = this;
            label.MaximumSize = new System.Drawing.Size(DEFAULT_MIN_SIZE.Width, 0);
            label.AutoSize = true;
            label.Font = mLabelFont;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Text = string.Join(CtConst.NewLine, msg.ToArray());

            /*-- 計算 Label 到安全距離所需的長和寬 --*/
            int height = DEFAULT_LAB_LOCATION.Y + label.Height + DEFAULT_BTN_SIZE.Height * 2 + DEFAULT_BTN_GAP * 2;

            /*-- 如果計算完的長寬比預設的最小 Size 還大，改變 Form 的大小 --*/
            if (height > DEFAULT_MIN_SIZE.Height) {
                Height = height;
                label.Location = DEFAULT_LAB_LOCATION;
            } else {
                label.Location = new Point(DEFAULT_LAB_LOCATION.X, ClientSize.Height/2 - label.Height);
            }

            label.Show();
        }

        /// <summary>產生字體物件，以預設的字體為主，修改字體大小</summary>
        /// <param name="fontSize">字體大小</param>
        /// <returns>產生的字體</returns>
        private Font GenerateFont(float fontSize) {
            return new Font(
                DEFAULT_LAB_FONT.FontFamily,
                fontSize,
                DEFAULT_LAB_FONT.Style,
                DEFAULT_LAB_FONT.Unit,
                DEFAULT_LAB_FONT.GdiCharSet,
                DEFAULT_LAB_FONT.GdiVerticalFont
            );
        }

        /// <summary>產生字體物件，自訂字型名稱與大小</summary>
        /// <param name="fontName">字型名稱</param>
        /// <param name="fontSize">字型大小</param>
        /// <returns>產生的字體</returns>
        private Font GenerateFont(string fontName, float fontSize) {
            return new Font(
                fontName,
                fontSize,
                DEFAULT_LAB_FONT.Style,
                DEFAULT_LAB_FONT.Unit,
                DEFAULT_LAB_FONT.GdiCharSet,
                DEFAULT_LAB_FONT.GdiVerticalFont
            );
        }
        #endregion

        #region Function - Interface Events
        /// <summary>按鈕按下，回傳 MsgBoxButton 並關閉視窗</summary>
        private void Button_Click(object sender, EventArgs e) {
            mUIBtn = (MsgBoxButton)(sender as Button).Tag;
            this.Close();
        }

        /// <summary>在繪製 LOGO 時順便繪製樣式圖案</summary>
        private void pbLogo_Paint(object sender, PaintEventArgs e) {
            /*-- 如果不用圖案，直接結束掉 --*/
            if (mMsgBoxStyle == MsgBoxStyle.NONE) return;

            /*-- 取得 Graphic --*/
            Graphics g = e.Graphics;

            /*-- 根據 MsgBoxStyle 選擇相對應的圖 --*/
            Image img = null;
            switch (mMsgBoxStyle) {
                case MsgBoxStyle.INFORMATION:
                    img = Properties.Resources.Info_3;
                    break;
                case MsgBoxStyle.WARNING:
                    img = Properties.Resources.Warning_3;
                    break;
                case MsgBoxStyle.QUESTION:
                    img = Properties.Resources.Question;
                    break;
                case MsgBoxStyle.ERROR:
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
        #endregion
    }
}
