using CtOutLookBar.Public;
using System.Drawing;
using System.Windows.Forms;

namespace CtOutLookBar.Internal {

    /// <summary>
    /// 分類控制項基類
    /// </summary>
    internal abstract class ContentPanel : Panel {
        public new OutlookBar Parent { get; set; }

        public ContentPanel() {
            // initial state
            Visible = true;
        }
    }

    /// <summary>
    /// 分類控制項
    /// </summary>
    internal class OutlookCategory : ContentPanel, IOutlookCategory {

        #region Declaration - Fields

        private Graphics mGraph = null;

        protected int mIconSpacing;
        protected int mMargin = 20;
        protected int mSpacing = 20;

        private int mRowCount = 2;

        private Label mBorder = new Label();

        private Size mItemSize = new Size(50, 50);

        private Font mItemFont = new Font("微軟正黑體", 12);

        #endregion Declaration - Fields

        #region Declartion - Properties

        public override string Text {
            get {
                return BandButton?.Text;
            }

            set {
                if (BandButton != null && BandButton.Text != value) {
                    BandButton.Text = value;
                }
            }
        }

        public override Font Font {
            get {
                return BandButton?.Font;
            }

            set {
                if (BandButton != null && BandButton.Font != value) {
                    BandButton.Font = value;
                }
            }
        }

        public override Color BackColor {
            get {
                return base.BackColor;
            }

            set {
                if (base.BackColor != value) {
                    base.BackColor = value;
                    mBorder.BackColor = value;
                }
            }
        }

        public Control Control { get { return this as Control; } }

        /// <summary>
        /// 一列可容納的選項數
        /// </summary>
        public int RowCount {
            get {
                return mRowCount;
            }
            set {
                if (mRowCount != value) {
                    mRowCount = value;
                    RefreshItem();
                }
            }
        }

        /// <summary>
        /// 選項圖示尺寸
        /// </summary>
        public Size ItemSize {
            get {
                return mItemSize;
            }
            set {
                if (mItemSize != value) {
                    mItemSize = value;
                    RefreshIconSpacing();
                }
            }
        }

        /// <summary>
        /// 選項文字字體
        /// </summary>
        public Font ItemFont {
            get {
                return mItemFont;
            }
            set {
                if (mItemFont != value) {
                    mItemFont = value;
                    mBorder.Font = value;
                    RefreshIconSpacing();
                }
            }
        }

        public int IconSpacing {
            get {
                return mIconSpacing;
            }
        }

        public new int Margin {
            get {
                return mMargin;
            }
        }

        /// <summary>
        /// 書籤按鈕參考
        /// </summary>
        public BandButton BandButton { get; set; }

        #endregion Declartion - Properties

        #region Function - Constructors

        public OutlookCategory() {
            //Dock = DockStyle.Fill;
            BackColor = Color.LightBlue;
            mGraph = mBorder.CreateGraphics();
            mBorder.Font = mItemFont;
            mBorder.ForeColor = BackColor;
            mBorder.BackColor = BackColor;
            mBorder.Text = "ITest";
            mBorder.Width = 1;

            //涴爵岆覃誹芞梓潔擒腔
            AutoScroll = true;
            Controls.Add(mBorder);
            RefreshIconSpacing();
            //Dock = DockStyle.Fill;
        }

        #endregion Function - Constructors

        #region Funciton - Public Methods

        public IClickSender AddItem(string caption, Image image = null, int enumIdx = -1) {
            Label lb = null;
            IOutlookItem item = null;
            SetItem(out lb, out item, caption, image, enumIdx);
            return new ClickObj(lb, item);
        }

        #endregion Funciton - Public Methods

        #region Funciton - Private Methods

        private void SetItem(out Label label, out IOutlookItem item, string caption, Image image, int enumIdx) {
            int index = Controls.Count / 2; // two entries per icon

            /*-- 標籤文字建構 --*/
            label = new Label();
            label.Font = mItemFont;//設定字型
            label.Text = caption;//設定文字
            label.Visible = true;
            label.AutoSize = true;

            /*-- 計算基準位置點 --*/
            Point basePoint = GetBasePoint(index);

            /*-- 於基準點建構圖示 --*/
            item = new OutlookItem(this, image, mItemSize, enumIdx);

            /*-- 計算標籤文字位置 --*/
            label.Location = basePoint.Add(GetOffset(label));

            /*-- 計算圖示位置 --*/
            item.Location = basePoint.Add(GetOffset());

            /*-- 圖示與標籤建立關聯 --*/
            label.Tag = item;

            /*-- 將圖示與文字標籤加入控制項集合 --*/
            Controls.Add(item.Control);
            Controls.Add(label);

            /*-- 調整邊界 --*/
            AdaptiveBorder(label);
        }

        /// <summary>
        /// 計算位置基準點
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Point GetBasePoint(int index) {
            int T = Width - SystemInformation.VerticalScrollBarWidth;
            Point basePoint = new Point();
            basePoint.X = T * (1 + index % mRowCount * 2) / (2 * mRowCount);
            basePoint.Y = mMargin + (index / mRowCount) * mIconSpacing;
            return basePoint;
        }

        /// <summary>
        /// 計算標籤位置補償
        /// </summary>
        /// <param name="lb"></param>
        /// <returns></returns>
        private Point GetOffset(Label lb) {
            Point ofs = new Point();
            ofs.X = -((int)mGraph.MeasureString(lb.Text, lb.Font).Width / 2);
            ofs.Y = mItemSize.Height + 5;
            return ofs;
        }

        /// <summary>
        /// 計算圖示位置補償
        /// </summary>
        /// <returns></returns>
        private Point GetOffset() {
            return new Point(-mItemSize.Width / 2, 0);
        }

        /// <summary>
        /// 更新項目單位高度
        /// </summary>
        private void RefreshIconSpacing() {
            // icon height + text height + margin
            mIconSpacing =
                mItemSize.Height +
                (int)mGraph.MeasureString(mBorder.Text, mBorder.Font).Height +
                mSpacing;

            RefreshItem();
        }

        /// <summary>
        /// 配合Label高度調整邊界
        /// </summary>
        /// <param name="lb"></param>
        private void AdaptiveBorder(Label lb) {
            ///不知為何Panel卷軸最大值只能到最下面的控制項的Top值
            ///因此用一個無用的Label控制項將其位置設為最下方控制項Top值+40
            ///以顯示最後一個選項的文字說明
            if (lb.Top > Height) {
                mBorder.Top = lb.Top + 2 * mMargin;
            }
        }

        /// <summary>
        /// 重新調整項目屬性
        /// </summary>
        private void RefreshItem() {
            if (Controls.Count > 2) {
                Label label = null;
                for (int i = 1; i < Controls.Count; i += 2) {
                    int idx = i / 2;
                    OutlookItem panelIcon = Controls[i] as OutlookItem;
                    label = Controls[i + 1] as Label;
                    Point basePoint = GetBasePoint(idx);
                    panelIcon.Location = basePoint.Add(GetOffset());
                    label.Location = basePoint.Add(GetOffset(label));
                }
                AdaptiveBorder(label);
            }
        }

        public void RefreshHorizontal() {
            if (Controls.Count > 2) {
                Label label = null;
                for (int i = 1; i < Controls.Count; i += 2) {
                    int idx = i / 2;
                    OutlookItem panelIcon = Controls[i] as OutlookItem;
                    label = Controls[i + 1] as Label;
                    Point basePoint = GetBasePoint(idx);
                    panelIcon.Left = basePoint.Add(GetOffset()).X;
                    label.Left = basePoint.Add(GetOffset(label)).X;
                }
            }
        }

        #endregion Funciton - Private Methods
    }
}