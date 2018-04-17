using CtOutLookBar.Public;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtOutLookBar.Internal {
    /// <summary>
    /// 功能按鈕
    /// </summary>
	internal class OutlookItem : PictureBox, IOutlookItem {

        #region Declaration - Fields

        /// <summary>
        /// 項目列舉值
        /// </summary>
        public int mEnumIdx;
        
        /// <summary>
        /// 滑鼠進入旗標
        /// </summary>
        private bool mMouseEnter;

        #endregion Declaration - Fields

        #region Function - Constructors

        public OutlookItem(IOutlookCategory parent, Image image, Size itemSize, int index) {
            this.mEnumIdx = index;
            this.Parent = parent;

            SizeMode = PictureBoxSizeMode.StretchImage;
            Size = itemSize;
            Image = image ?? (Image)Resource.Cancel_2;

            Visible = true;
            Tag = this;

            MouseEnter += new EventHandler(OnMouseEnter);
            MouseLeave += new EventHandler(OnMouseLeave);
            MouseMove += new MouseEventHandler(OnMouseMove);
            
            mMouseEnter = false;
        }

        #endregion Function - Constructors

        #region Implement - IOutlookItem
        
        /// <summary>
        /// 父節點
        /// </summary>
        public new IOutlookCategory Parent { get; set; }

        /// <summary>
        /// <see cref="Control"/>型態值
        /// </summary>
        public Control Control { get { return this as Control; } }

        /// <summary>
        /// 物件列舉值，用於分辨滑鼠點擊事件觸發物件
        /// </summary>
        public int EnumIdx {
            get {
                return mEnumIdx;
            }
        }

        #endregion Implement - IOutlookItem

        #region Function - Events

        private void OnMouseMove(object sender, MouseEventArgs args) {
            if ((args.X < Size.Width - 2) &&
                (args.Y < Size.Width - 2) &&
                (!mMouseEnter)) {
                BackColor = Color.LightCyan;
                BorderStyle = BorderStyle.FixedSingle;
                Location = Location - new Size(1, 1);
                mMouseEnter = true;
            }
        }

        private void OnMouseEnter(object sender, EventArgs e) {
        }

        private void OnMouseLeave(object sender, EventArgs e) {
            if (mMouseEnter) {
                BackColor = Parent.BackColor;
                BorderStyle = BorderStyle.None;
                Location = Location + new Size(1, 1);
                mMouseEnter = false;
            }
        }

        #endregion Funciton - Events
    }

}
