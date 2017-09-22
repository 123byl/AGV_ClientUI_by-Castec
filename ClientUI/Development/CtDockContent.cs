using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
namespace ClientUI {
    
    /// <summary>
    /// DockContnet接口
    /// </summary>
    public interface ICtDockContent : IDisposable {

        #region Properties

        /// <summary>
        /// 預設停靠狀態
        /// </summary>
        DockState DefaultDockState { get; set; }
        
        /// <summary>
        /// 表單固定尺寸
        /// </summary>
        Size FixedSize { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 分配行為方法參考
        /// </summary>
        /// <param name="actFunc"></param>
        void AssignmentActFunc(object actFunc);

        /// <summary>
        /// 分配主介面參考
        /// </summary>
        /// <param name="main">主介面物件參考</param>
        void AssignmentMainUI(AgvClientUI main);

        /// <summary>
        /// 分配<see cref="DockPanel"/>物件參考
        /// </summary>
        /// <param name="dockPanel"></param>
        void AssignmentDockPanel(DockPanel dockPanel);

        /// <summary>
        /// 隱藏視窗
        /// </summary>
        void HideWindow();

        /// <summary>
        /// 依照預設停靠狀態顯示
        /// </summary>
        void ShowWindow();

        #endregion Methods

        #region 原本就有實作的方法、屬性
        DockState DockState { get; set; }
        event EventHandler DockStateChanged;
        string Text { get; set; }
        #endregion 

    }

    /// <summary>
    /// 基礎的DockContent類
    /// </summary>
    public class CtDockContent<T> : DockContent, ICtDockContent {

        #region Declaration - Fiedls
        
        /// <summary>
        /// 預設的DockState狀態
        /// </summary>
        private DockState mDefDockState = DockState.Hidden;

        /// <summary>
        /// 表單固定大小
        /// </summary>
        private Size mFixedSize = new Size(0,0);

        /// <summary>
        /// 功能實例參考
        /// </summary>
        protected T rActFunc = default(T);

        /// <summary>
        /// 主介面參考
        /// </summary>
        protected AgvClientUI rMain = null;

        /// <summary>
        /// <see cref="DockPanel"/>參考
        /// </summary>
        protected DockPanel rDockPanel = null;

        #endregion Declaration - Fields

        #region Declaration - Properties

        /// <summary>
        /// 預設的DockState狀態
        /// </summary>
        public DockState DefaultDockState { get { return mDefDockState; } set {
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
        public Size FixedSize { get { return mFixedSize; } set {
                mFixedSize = value;
                CorrectionAutoHidePortion();
            }
        }

        #endregion Declaration - Properties

        #region Function - Constructors

        /// <summary>
        /// 共用建構方法
        /// </summary>
        private CtDockContent() {
            /*-- 表單關閉中事件訂閱 --*/
            this.FormClosing += CtDockContent_FormClosing;
            this.AutoScroll = true;
        }

        /// <summary>
        /// 具有預設DockState功能的建置方法
        /// </summary>
        /// <param name="defState">預設的停靠狀態，不可為Unknown</param>
        public CtDockContent(DockState defState):this() {
            if (defState == DockState.Unknown) {
                throw new ArgumentException($"預設DockState不可為{defState}");
            }
            DefaultDockState = defState;
        }

        /// <summary>
        /// 泛型建構方法
        /// </summary>
        /// <param name="mth">功能方法實例參考</param>
        /// <param name="defState">預設的停靠狀態，不可為Unknown</param>
        public CtDockContent(T mth,DockState defState = DockState.Float):this(defState) {
            rActFunc = mth;
        }

        /// <summary>
        /// 附加主介面參考
        /// </summary>
        /// <param name="actFunc"></param>
        /// <param name="main"></param>
        /// <param name="defState"></param>
        public CtDockContent(T actFunc, AgvClientUI main ,DockState defState):this(actFunc, defState) {
            rMain = main;
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
            #endregion
            e.Cancel = true;
            this.Hide();
        }

        #endregion Function - Events
        
        #region Function - Public Methods

        /// <summary>
        /// 分配行為方法參考
        /// </summary>
        /// <param name="obj">行為方法參考</param>
        public void AssignmentActFunc(object obj) {
            if (obj is T) {
                AssignmentActFunc((T)obj);
            } else {
                throw new ArgumentException($"ActFunc類型須為{typeof(T)}");
            }
        }
        
        /// <summary>
        /// 分配主介面參考
        /// </summary>
        /// <param name="main">主介面物件參考</param>
        public void AssignmentMainUI(AgvClientUI main) {
            rMain = main;
        }

        /// <summary>
        /// 分配<see cref="DockPanel"/>物件參考
        /// </summary>
        /// <param name="dockPanel"></param>
        public void AssignmentDockPanel(DockPanel dockPanel) {
            rDockPanel = dockPanel;
        }

        /// <summary>
        /// 依照預設來顯示視窗
        /// </summary>
        public void ShowWindow() {
            if (DefaultDockState != DockState.Hidden && rDockPanel != null) {
                BeginInvoke(() => this.Show(rDockPanel, DefaultDockState));
            }
        }

        /// <summary>
        /// 隱藏視窗
        /// </summary>
        public void HideWindow() {
            BeginInvoke(() => this.Hide());
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
            }else {
                action();
            }
        }

        /// <summary>
        /// 分配行為方法類
        /// </summary>
        /// <param name="actFunc">行為方法類</param>
        protected virtual void AssignmentActFunc(T actFunc) {
            /*-- 若是已有參考則先釋放掉 --*/
            if (rActFunc != null) {
                RemoveEvent();
            }

            /*-- 保存參考並訂閱事件 --*/
            rActFunc = actFunc;
            AddEvent();
        }
       
        /// <summary>
        /// 訂閱<see cref="rActFunc"/>事件
        /// </summary>
        /// <remarks>
        /// 提供衍生類進行覆寫
        /// </remarks>
        protected virtual void AddEvent() {

        }

        /// <summary>
        /// 取消訂閱<see cref="rActFunc"/>事件
        /// </summary>
        /// <remarks>
        /// 提供衍生類進行覆寫
        /// </remarks>
        protected virtual void RemoveEvent() {

        }

        /// <summary>
        /// 依照<see cref="FixedSize"/>與<see cref="mDefDockState"/>修正視窗尺寸
        /// </summary>
        private void CorrectionAutoHidePortion() {
            DockAreas area = mDefDockState.ToAreas();//停靠區域
            Size dockPortion = mFixedSize;//視窗預設尺寸
            double portion = 0;//轉換後的Portion值
            if (DockMth.CalculatePortion(area, dockPortion, out portion)){
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
                RemoveEvent();
            }
            base.Dispose(disposing);
        }

        #endregion Funciton - Protected Methods

    }

    /// <summary>
    /// Dock常用方法
    /// </summary>
    public static class DockMth {
        /// <summary>
        /// 將<see cref="DockState"/>轉換為<see cref="DockAreas"/>
        /// </summary>
        /// <param name="dockState"></param>
        /// <returns>對應的<see cref="DockAreas"/></returns>
        public static DockAreas ToAreas(this DockState dockState) {
            DockAreas area = DockAreas.Document;
            switch (dockState) {
                case DockState.DockBottom:
                case DockState.DockBottomAutoHide:
                    area = DockAreas.DockBottom;
                    break;
                case DockState.DockLeft:
                case DockState.DockLeftAutoHide:
                    area = DockAreas.DockLeft;
                    break;
                case DockState.DockTop:
                case DockState.DockTopAutoHide:
                    area = DockAreas.DockTop;
                    break;
                case DockState.DockRight:
                case DockState.DockRightAutoHide:
                    area = DockAreas.DockRight;
                    break;
                case DockState.Document:
                case DockState.Hidden:
                    area = DockAreas.Document;
                    break;
                case DockState.Float:
                    area = DockAreas.Float;
                    break;
                default:
                    throw new ArgumentException($"未定義{dockState}預設停靠行為");
            }

            return area;
        }

        /// <summary>
        /// 將實際Size轉換為Portion值
        /// </summary>
        /// <param name="area">停靠區域</param>
        /// <param name="dockSize">視窗大小</param>
        /// <param name="portion">轉換後的Portion值</param>
        /// <returns>是否轉換成功</returns>
        /// <remarks>
        /// Form與DockContent的的Size似乎並不相同
        /// 下面的參數是試出來的近似值，不同解析度可能有不同的參數
        /// </remarks>
        public static bool CalculatePortion(DockAreas area, Size dockSize, out double portion) {
            bool ret = true;
            portion = 0;
            switch (area) {
                case DockAreas.DockBottom:
                case DockAreas.DockTop:
                    if (ret = dockSize.Height > 7.43) {
                        portion = (float)((dockSize.Height - 7.43) / 1.32);
                    }
                    break;
                case DockAreas.DockLeft:
                case DockAreas.DockRight:
                    if (ret = dockSize.Width > 6) {
                        portion = (float)((dockSize.Width - 6) / 1.36);
                    }
                    break;
                default:
                    ret = false;
                    break;
            }
            return ret;
        }

        /// <summary>
        /// 判斷是否需要用Invoke來執行方法
        /// </summary>
        /// <param name="dockContent">方法相關的<see cref="DockContent"/>物件</param>
        /// <param name="act">要執行的方法</param>
        private static void BeginInvokeIfNecessary(this DockContent dockContent, MethodInvoker act) {
            if (dockContent.InvokeRequired) {
                dockContent.Invoke(act);
            }else {
                act();
            }
        }


    }
}
