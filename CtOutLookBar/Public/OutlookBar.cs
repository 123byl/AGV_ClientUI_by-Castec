using CtOutLookBar.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CtOutLookBar.Public {

    /// <summary>
    /// 主要容器控制項
    /// </summary>
    public class OutlookBar : Panel {

        #region Dedclaration - Fields

        private int buttonHeight = 25;
        private int mSelectedCategory = 0;
        private int selectedBandHeight = 0;

        private Control rParent = null;

        /// <summary>
        /// 分類物件集合
        /// </summary>
        private List<IOutlookCategory> mCategorys = new List<IOutlookCategory>();

        #endregion Dedclaration - Fields

        #region Declaration - Properties

        public int ButtonHeight {
            get {
                return buttonHeight;
            }

            set {
                buttonHeight = value;
                // do recalc layout for entire bar
            }
        }

        public int SelectedBand {
            get {
                return mSelectedCategory;
            }
            set {
                SelectCategory(value);
            }
        }

        #endregion Declaration - Properties

        #region Funciton - Constructors

        public OutlookBar() {
            this.Resize += OutlookBar_Resize;
            this.ParentChanged += OutlookBar_ParentChanged;
        }

        private void OutlookBar_ParentChanged(object sender, EventArgs e) {
            if (rParent != null) {
                rParent.SizeChanged -= SizeChangedEvent;
            }
            rParent = Parent;
            rParent.SizeChanged += SizeChangedEvent;
        }

        private void OutlookBar_Resize(object sender, EventArgs e) {
            int index = 0;
            foreach (Control ctrl in Controls) {
                if (ctrl is BandPanel) {
                    RecalcLayout(ctrl as BandPanel, index);
                }
                index++;
            }
        }

        #endregion Funciton - Constructors

        #region Function - Public Methods

        /// <summary>
        /// 新增分類
        /// </summary>
        /// <param name="caption"></param>
        /// <returns></returns>
        public IOutlookCategory AddCategory(string caption) {
            OutlookCategory content = new OutlookCategory();
            content.Parent = this;
            int index = Controls.Count;
            BandTagInfo bti = new BandTagInfo(this, index);
            BandPanel bandPanel = new BandPanel(caption, content, bti);
            Controls.Add(bandPanel);
            UpdateBarInfo();
            RecalcLayout(bandPanel, index);
            mCategorys.Add(content);
            return content;
        }

        public void SelectCategory(int index) {
            mSelectedCategory = index;
            RedrawBands();
        }

        #endregion Function - Public Methods

        #region Functoin - Private Methods

        private void RedrawBands() {
            for (int i = 0; i < Controls.Count; i++) {
                if (Controls[i] is BandPanel) {
                    BandPanel bp = Controls[i] as BandPanel;
                    RecalcLayout(bp, i);
                }
            }
            foreach (OutlookCategory item in mCategorys) {
                item.RefreshHorizontal();
            }
        }

        private void UpdateBarInfo() {
            selectedBandHeight = ClientRectangle.Height - (Controls.Count * buttonHeight);
        }

        private void RecalcLayout(BandPanel bandPanel, int index) {
            int vPos = (index <= mSelectedCategory) ? buttonHeight * index : buttonHeight * index + selectedBandHeight;
            int height = mSelectedCategory == index ? selectedBandHeight + buttonHeight : buttonHeight;

            // the band dimensions
            bandPanel.Location = new Point(0, vPos);
            bandPanel.Size = new Size(ClientRectangle.Width, height);

            // the contained button dimensions
            bandPanel.Controls[0].Location = new Point(0, 0);
            bandPanel.Controls[0].Size = new Size(ClientRectangle.Width, buttonHeight);

            // the contained content panel dimensions
            bandPanel.Controls[1].Location = new Point(0, buttonHeight);
            bandPanel.Controls[1].Size = new Size(ClientRectangle.Width - 2, height - 8);
        }

        #endregion Functoin - Private Methods

        #region Function - Events

        private void SizeChangedEvent(object sender, EventArgs e) {
            Size = new Size(Size.Width, ((Control)sender).ClientRectangle.Size.Height);
            UpdateBarInfo();
            RedrawBands();
        }

        #endregion Function - Events
    }
}