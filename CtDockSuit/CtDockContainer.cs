using System;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace CtDockSuit {

    /// <summary>
    /// 基礎的DockContent類
    /// </summary>
    public class CtDockContainer : DockContent, ICtDockContainer {

        #region Declaration - Fiedls

        /// <summary>
        /// 預設的DockState狀態
        /// </summary>
        private DockState mDefDockState = DockState.Hidden;

        /// <summary>
        /// 表單固定大小
        /// </summary>
        private Size mFixedSize = new Size(0, 0);

        /// <summary>
        /// <see cref="DockPanel"/>參考
        /// </summary>
        protected DockPanel rDockPanel = null;

        /// <summary>
        /// 是否可視
        /// </summary>
        protected bool mVisible = false;

        #endregion Declaration - Fiedls

        #region Declaration - Events

        public new virtual event EventHandler DockStateChanged {
            add {
                base.DockStateChanged += value;
            }
            remove {
                base.DockStateChanged -= value;
            }
        }

        #endregion Declaration - Events

        #region Declaration - Properties

        /// <summary>
        /// 預設的DockState狀態
        /// </summary>
        public DockState DefaultDockState {
            get { return mDefDockState; }
            set {
                mDefDockState = value;
                CorrectionAutoHidePortion();
            }
        }

        /// <summary>
        /// 表單固定大小
        /// </summary>
        /// <remarks>
        /// 執行階段所讀取到的表單Size為DockPanel的DefaultFloatWindowSize
        /// 因此只能將設計階段設定的尺寸手動輸入FixedSize變數當中
        /// 透過將該屬性設為abstract強迫繼承時必須設定表單尺寸
        /// </remarks>
        public Size FixedSize {
            get { return mFixedSize; }
            set {
                mFixedSize = value;
                CorrectionAutoHidePortion();
            }
        }

        public new virtual DockState DockState { get { return base.DockState; } set { base.DockState = value; } }

        public new bool Visible {
            get => mVisible; set {
                if (mVisible != value) {
                    mVisible = value;
                    if (value) {
                        ShowWindow();
                    } else {
                        HideWindow();
                    }
                }
            }
        }

        #endregion Declaration - Properties

        #region Function - Constructors

        /// <summary>
        /// 具有預設DockState功能的建置方法
        /// </summary>
        public CtDockContainer(DockState defState) {
            if (defState == DockState.Unknown) {
                throw new ArgumentException($"預設DockState不可為{defState}");
            }
            DefaultDockState = defState;
            /*-- 表單關閉中事件訂閱 --*/
            this.FormClosing += CtDockContent_FormClosing;
            this.AutoScroll = true;
        }

        private CtDockContainer() {
        }

        #endregion Function - Constructors

        #region Function - Events

        /// <summary>
        /// 表單關閉中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void CtDockContent_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e) {

            #region 以隱藏代替關閉

            /// 網路上的參考資料是說將表單的HideOnClose設為True，即可用隱藏代替關閉
            /// 但是實際測試上多次的開開關關還是會有例外跳出
            /// 因此還是以取消的方式來隱藏視窗

            #endregion 以隱藏代替關閉

            e.Cancel = true;
            this.Hide();
        }

        #endregion Function - Events

        #region Function - Public Methods

        /// <summary>
        /// 分配<see cref="DockPanel"/>物件參考
        /// </summary>
        /// <param name="dockPanel"></param>
        public void AssignmentDockPanel(DockPanel dockPanel) {
            rDockPanel = dockPanel;
        }

        #endregion Function - Public Methods

        #region Function - Private Methods

        /// <summary>
        /// 判斷是否需要用Invoke來執行
        /// </summary>
        /// <param name="action"></param>
        private void BeginInvoke(MethodInvoker action) {
            if (this.InvokeRequired) {
                this.Invoke(action);
            } else {
                action();
            }
        }

        /// <summary>
        /// 依照<see cref="FixedSize"/>與<see cref="mDefDockState"/>修正視窗尺寸
        /// </summary>
        private void CorrectionAutoHidePortion() {
            DockAreas area = mDefDockState.ToAreas();//停靠區域
            Size dockPortion = mFixedSize;//視窗預設尺寸
            //轉換後的Portion值
            if (area.CalculatePortion(dockPortion, out double portion)) {
                this.AutoHidePortion = portion;
            }
        }

        /// <summary>
        /// 釋放資源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                this.FormClosing -= CtDockContent_FormClosing;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 依照預設來顯示視窗
        /// </summary>
        protected virtual void ShowWindow() {
            if (rDockPanel != null) {
                if (DefaultDockState == DockState.Float || DefaultDockState == DockState.Hidden) {
                    rDockPanel.DefaultFloatWindowSize = this.mFixedSize;

                    BeginInvoke(() => {
                        this.Show(rDockPanel, new Rectangle(this.Location, this.mFixedSize));
                    });
                } else if (DefaultDockState < DockState.Float || DefaultDockState >= DockState.Hidden) {
                    return;
                } else {
                    BeginInvoke(() => this.Show(rDockPanel, DefaultDockState));
                }
            } else {
                throw new Exception("找不到DockPanel物件參考");
            }
        }

        /// <summary>
        /// 隱藏視窗
        /// </summary>
        protected virtual void HideWindow() {
            BeginInvoke(() => this.Hide());
        }

        #endregion Function - Private Methods

        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtDockContainer));
            this.SuspendLayout();
            //
            // CtDockContent
            //
            this.ClientSize = new System.Drawing.Size(588, 433);
            this.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CtDockContent";
            this.Load += new System.EventHandler(this.CtDockContent_Load);
            this.ResumeLayout(false);
        }

        private void CtDockContent_Load(object sender, EventArgs e) {
        }
    }
}