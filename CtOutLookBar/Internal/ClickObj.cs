using CtOutLookBar.Public;
using System;
using System.Windows.Forms;

namespace CtOutLookBar.Internal {

    /// <summary>
    /// 滑鼠點擊事件發報者封包
    /// </summary>
    /// <remarks>
    /// 將<see cref="Label"/>與<see cref="IOutlookItem"/>封裝
    /// 讓使用者統一進行事件滑鼠點擊事件委派
    /// </remarks>
    internal class ClickObj : IClickSender {

        #region Declaration - Fields

        private Label mLb = null;

        private IOutlookItem mPanelIcon = null;

        #endregion Declaration - Fields

        #region Funciotn - Constructors

        public ClickObj(Label lb, IOutlookItem icoPanel) {
            mLb = lb;
            mPanelIcon = icoPanel;
        }

        #endregion Funciotn - Constructors

        #region Implement - IClickSender

        public EventHandler Click {
            get {
                return null;
            }
            set {
                if (value != null) {
                    if (mLb != null) mLb.Click += value;
                    if (mPanelIcon != null) mPanelIcon.Click += value;
                }
            }
        }

        #endregion Implement - IClickSender
    }
}