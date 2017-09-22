using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Reflection;

using WeifenLuo.WinFormsUI.Docking;

using CtLib.Library;
using CtLib.Module.Ultity;
using static CtLib.Forms.CtLogin;
using CtLib.Forms;
namespace ClientUI {
    
    /// <summary>
    /// AGV Client端接口
    /// </summary>
    public interface IAgvClient :
    #region 繼承的功能接口
        IConsole,
        IMapGL,
        IGoalSetting,
        ITesting,
        IDisposable
    #endregion 繼承的功能接口
        {

        UserData UserData { get; }
        

        /// <summary>
        /// 是否Bypass Socket功能
        /// </summary>
        bool IsBypassSocket { get; set; }

        /// <summary>
        /// 是否Bypass LoadFile功能
        /// </summary>
        bool IsBypassLoadFile { get; set; }

        /// <summary>
        /// 當前語系
        /// </summary>
        /// <remarks>
        /// 暫時用不到，未來開發多語系用
        /// </remarks>
        UILanguage Culture { get; }

        /// <summary>
        /// AGV客戶端事件
        /// </summary>
        event AgvClientEvent AgvClientEventTrigger;

        ///// <summary>
        ///// 車子資訊更新事件
        ///// </summary>
        //event CarInfoRefresh InfoRefresh;
        
        ///// <summary>
        ///// 使用者變更事件
        ///// </summary>
        //event UserChanged UserChanged;

        /// <summary>
        /// 顯示關於視窗
        /// </summary>
        void form_About();
        
        /// <summary>
        /// 模組版本註冊
        /// </summary>
        /// <remarks>
        /// 用於紀錄使用到的模組版本
        /// </remarks>
        /// <param name="modName">模組名稱</param>
        /// <param name="modVer">模組版本</param>
        void RegisterVersion(string modName,string modVer);

        /// <summary>
        /// 依照當前權限決定登入或登出
        /// </summary>
        void Login();

        /// <summary>
        /// 使用者管理視窗
        /// </summary>
        void form_UserManager();
        
    }
    
    /// <summary>
    /// 客戶端介面
    /// </summary>
    public partial class AgvClientUI : Form {

        #region Declaration - Fields

        /// <summary>
        /// Agv Client端物件參考
        /// </summary>
        private IAgvClient rAgvClient = null;

        /// <summary>
        /// ICtDockContent與MenuItem對照
        /// </summary>
        private Dictionary<ToolStripMenuItem, ICtDockContent> mDockContent = null;

        /// <summary>
        /// 系統列圖示物件
        /// </summary>
        private CtNotifyIcon mNotifyIcon = null;

        /// <summary>
        /// 系統列圖示右鍵選單
        /// </summary>
        private MenuItems mMenuItems = null;

        /// <summary>
        /// 系統列圖示標題
        /// </summary>
        protected string mNotifyCaption = "AGV Client";

        #endregion Declaration - Fields

        #region Declaration - Properties

        /// <summary>
        /// MapGL子視窗
        /// </summary>
        protected CtMapGL MapGL {
            get {
                return  mDockContent.ContainsKey(miMapGL) ? mDockContent[miMapGL] as CtMapGL : null;                
            }
        }

        /// <summary>
        /// Console子視窗
        /// </summary>
        protected CtConsole Console {
            get {
                return mDockContent.ContainsKey(miConsole) ? mDockContent[miConsole] as CtConsole : null;
            }
        }

        /// <summary>
        /// 測試子視窗
        /// </summary>
        protected CtTesting Testing {
            get {
                return mDockContent.ContainsKey(miTesting) ? mDockContent[miTesting] as CtTesting : null;
            }
        }

        /// <summary>
        /// Goal點設定子視窗
        /// </summary>
        protected CtGoalSetting GoalSetting {
            get {
                return mDockContent.ContainsKey(miGoalSetting) ? mDockContent[miGoalSetting] as CtGoalSetting : null;
            }
        }
        
        #endregion Declaration - Properties

        #region Functin - Constructors

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public AgvClientUI() {
            InitializeComponent();
            
        }

        /// <summary>
        /// 傳入<see cref="IAgvClient"/>參考進行建置
        /// </summary>
        /// <param name="agvClient"><see cref="IAgvClient"/>參考實例</param>
        public AgvClientUI(IAgvClient agvClient):this() {
            rAgvClient = agvClient;
            rAgvClient.AgvClientEventTrigger += rAgvClient_OnAgvClientEventTrigger; ;
            
        }

        #endregion Function - Constructors

        #region Function - Events

        #region Form

        /// <summary>
        /// 表單載入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientUI_Load(object sender, EventArgs e) {
            /*-- 載入ICtDockContent物件 --*/
            LoadICtDockContent();

            /*-- 載入CtNotifyIcon物件 --*/
            LoadCtNotifyIcon();

            /*-- 依照使用者權限進行配置 --*/
            UserChanged(rAgvClient.UserData);

            /*-- 檢查Bypass狀態 --*/
            CtInvoke.ToolStripItemChecked(miBypassSocket, rAgvClient.IsBypassSocket);
            CtInvoke.ToolStripItemChecked(miLoadFile, rAgvClient.IsBypassLoadFile);

            /*-- 檢查遠端設備IP --*/
            tslbHostIP.Text = rAgvClient.HostIP;
            
        }
        
        /// <summary>
        /// 表單關閉中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientUI_FormClosing(object sender, FormClosingEventArgs e) {
            #region 取消程式關閉
            //由於CtDockContetn中在表單關閉中事件會把e.Cancel寫為true
            //為了確實關閉程式，需再把e.Cancl寫為false
            //
            //當直接關閉表單時，改為隱藏至系統列
            #endregion 
            e.Cancel = true;
            HideWindow();
        }

        /// <summary>
        /// 將主介面縮小至系統列
        /// </summary>
        private void HideWindow() {
            this.Hide();
            mNotifyIcon.ShowIcon();

        }

        #endregion Form

        #region MenuItem

        /// <summary>
        /// DockContent對應MenuItem按下之事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuDock_Click(object sender,EventArgs e) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            /*-- 確認是否有對應DockContent物件 --*/
            if (mDockContent.ContainsKey(item)) {
                
                if (item.Checked) {
                    (mDockContent[item] as DockContent).Hide();
                }else {
                    mDockContent[item].ShowWindow();
                }
            }
        }

        /// <summary>
        /// 離開程式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miExit_Click(object sender, EventArgs e) {
            Exit();
        }

        /// <summary>
        /// 關於
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miAbout_Click(object sender, EventArgs e) {
            rAgvClient.form_About();
        }

        /// <summary>
        /// 切換使用者
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miLogin_Click(object sender, EventArgs e) {
            rAgvClient.Login();
        }

        /// <summary>
        /// 使用者管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miUserManager_Click(object sender, EventArgs e) {
            rAgvClient.form_UserManager();
        }

        #endregion MenuItem

        #region DockContent

        /// <summary>
        /// DockContent的停靠狀態變更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Value_DockStateChanged(object sender, EventArgs e) {

            /*-- 取得發報的DockContent物件 --*/
            DockContent dockWnd = sender as DockContent;

            /*--取得對應MenuItem物件--*/
            ToolStripMenuItem menuItem = mDockContent.First(v => v.Value == dockWnd).Key;

            /*-- 依照DockState切換MenuItem的Check狀態 --*/
            if (menuItem != null) menuItem.Checked = dockWnd.DockState != DockState.Hidden;
        }

        #endregion DockContent

        #region AgvClient

        /// <summary>
        /// AGV客戶端事件處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rAgvClient_OnAgvClientEventTrigger(object sender, AgvClientEventArgs e) {
            switch (e.Type) {
                case AgvClientEventType.CarInfoRefresh:
                    CarInfoRefresh((CarInfo)e.Value);
                    break;
                case AgvClientEventType.GetLaser:

                    break;
                case AgvClientEventType.UserChanged:
                    UserChanged(e.Value as UserData);
                    break;
                case AgvClientEventType.Mode:
                    ChangedMode((CarMode)e.Value);
                    break;
                case AgvClientEventType.SendFile:
                    SetBalloonTip("Send File", e.Value as string, ToolTipIcon.Info,10);
                    break;
                case AgvClientEventType.LoadFile:
                    SetBalloonTip($"Load { (FileType)e.Value}", $"The { (FileType)e.Value} is loaded", ToolTipIcon.Info, 10);
                    break;
                case AgvClientEventType.GetFile:
                    SetBalloonTip($"Get{(FileType)e.Value}", $"The {(FileType)e.Value} is downloaded", ToolTipIcon.Info, 10);
                    break;
            }
        }

        
        #endregion AgvClient

        #region NotityIcon

        /// <summary>
        /// 滑鼠雙擊系統列圖示事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mNotifyIcon_OnMouseDoubleClick(object sender, MouseEventArgs e) {
            ShowWindow();
        }

        /// <summary>
        /// ShowWindow選項被點擊
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowWindow_OnClick(object sender,EventArgs e) {
            ShowWindow();
        }

        /// <summary>
        /// 顯示氣球提示
        /// </summary>
        /// <param name="title">提示標題</param>
        /// <param name="context">提示內容</param>
        /// <param name="icon">提示Icon</param>
        /// <param name="tmo">顯示時間</param>
        public void SetBalloonTip(string title, string context, ToolTipIcon icon, int tmo) {
            mNotifyIcon.ShowBalloonTip(title, context, tmo, icon);
        }

        #endregion NotifyIcon

        #region ToolStripMenuItem

        private void miBypassSocket_Click(object sender, EventArgs e) {
            bool isBypass = !miBypassSocket.Checked;
            rAgvClient.IsBypassSocket = isBypass;
            CtInvoke.ToolStripItemChecked(miBypassSocket, isBypass);
        }

        private void miLoadFile_Click(object sender, EventArgs e) {
            bool isBypass = !miLoadFile.Checked;
            rAgvClient.IsBypassLoadFile = isBypass;
            CtInvoke.ToolStripItemChecked(miLoadFile, isBypass);
        }

        private void miServer_Click(object sender, EventArgs e) {
        
        }

        #endregion ToolStripMenuItem

        #endregion Function - Events

        #region Functoin - Public Methods

        /// <summary>
        /// 清除所有Goal點
        /// </summary>
        public void DeleteAllGoal() {
            MapGL?.DeleteAllGoal();
        }

        /// <summary>
        /// 增加Goal點
        /// </summary>
        /// <param name="goal">Goal點</param>
        public void AddGoalPos(CarPos goal) {
            MapGL?.AddGoalPos(goal);
        }

        /// <summary>
        /// 設定GL模式
        /// </summary>
        public void SetGLMode(GLMode mode) {
            MapGL?.SetGLMode(mode);
        }

        /// <summary>
        /// 清除地圖
        /// </summary>
        public void ClearMap() {
            MapGL?.ClearMap();
        }
        
        #endregion Function - Public Methdos

        #region Function - Private Methods

        /// <summary>
        /// 車子模式切換時
        /// </summary>
        /// <param name="mode"></param>
        private void ChangedMode(CarMode mode) {
            tslbStatus.Text = $"{mode}";
        }

        /// <summary>
        /// 載入ICtDockContent物件
        /// </summary>
        private void LoadICtDockContent() {
            if (mDockContent != null) return;
            /*-- 載入DockContent --*/
            mDockContent = new Dictionary<ToolStripMenuItem, ICtDockContent>() {
                { miConsole,new CtConsole(DockState.DockBottomAutoHide)},
                { miGoalSetting,new CtGoalSetting(DockState.DockLeft)},
                { miTesting,new CtTesting(DockState.DockLeft)},
                { miMapGL,new CtMapGL( DockState.Document)}
            };

            /*-- 計算每個固定停靠區域所需的顯示大小 --*/
            foreach (var area in Enum.GetValues(typeof(DockAreas))) {
                CalculateFixedPortion(dockPanel, (DockAreas)area);
            }

            /*-- 遍歷所有DockContent與MenuItem物件 --*/
            foreach (var kvp in mDockContent) {
                ToolStripMenuItem item = kvp.Key;
                ICtDockContent dokContent = kvp.Value as ICtDockContent;

                /*-- 參考分配 --*/
                dokContent.AssignmentActFunc(rAgvClient);
                dokContent.AssignmentMainUI(this);
                dokContent.AssignmentDockPanel(dockPanel);

                /*-- 顯示視窗 --*/
                dokContent.ShowWindow();

                /*-- 訂閱DockDockStateCHanged事件 --*/
                dokContent.DockStateChanged += Value_DockStateChanged;

                /*-- 依照DockState狀態顯示MenuItem的Check狀態 --*/
                item.Checked = dokContent.DockState != DockState.Hidden;

                /*-- MenuItem顯示DockContent標題文字(Text) --*/
                item.Text = dokContent.Text;
                
            }
        }

        /// <summary>
        /// 載入CtNotifyIcon物件
        /// </summary>
        private void LoadCtNotifyIcon() {
            if (mNotifyIcon == null) {
                mNotifyIcon = new CtNotifyIcon(this, mNotifyCaption);
                mNotifyIcon.OnMouseDoubleClick += mNotifyIcon_OnMouseDoubleClick;
                mNotifyIcon.Visible = true;
                mMenuItems = new MenuItems();
                
                mMenuItems.AddMenuItem(
                    "Show Window",
                    ShowWindow_OnClick
                );

                mMenuItems.AddMenuItem(
                    "Exit",
                    (sender, e) => Exit()
                    );
                mNotifyIcon.MenuItems = mMenuItems;
            }
        }

        /// <summary>
        /// 統計<see cref="mDockContent"/>中相同停靠區域最大區塊大小，並寫入<see cref="DockPanel"/>設定
        /// </summary>
        /// <param name="panel">要設定的<see cref="DockPanel"/></param>控制項
        /// <param name="area">要統計的停靠區域</param>
        private void CalculateFixedPortion(DockPanel panel, DockAreas area) {
            /*-- 取得相同停靠區域的DockContent集合 --*/
            var dockContents = mDockContent.Where(kvp =>
               kvp.Value.DefaultDockState.ToAreas() == area
            );

            /*-- 沒有DockContent存在則離開 --*/
            if (!dockContents.Any()) return;

            /*-- 計算集合中最大的Width與Height值 --*/
            Size dockSize = new Size();
            dockSize.Width = dockContents.Max(v => v.Value.FixedSize.Width);
            dockSize.Height = dockContents.Max(v => v.Value.FixedSize.Height);

            /*-- 依照停靠區域計算所需顯示大小 --*/
            double portion = 0;
            if (DockMth.CalculatePortion(area,dockSize,out portion)){
                switch (area) {
                    case DockAreas.DockBottom:
                        panel.DockBottomPortion = portion;
                        break;
                    case DockAreas.DockLeft:
                        panel.DockLeftPortion = portion;
                        break;
                    case DockAreas.DockRight:
                        panel.DockRightPortion = portion;
                        break;
                    case DockAreas.DockTop:
                        panel.DockTopPortion = portion;
                        break;
                }
            }
        }

        /// <summary>
        /// 顯示主介面
        /// </summary>
        private void ShowWindow() {
            mNotifyIcon.HideIcon();
            this.Show();
            this.TopMost = true;
            #region 把DocDocument切回來
            ///由於主介面關閉的時候會觸發到DockDocument的FormCloseing事件
            ///導致子介面被隱藏
            ///這邊在手動把他切回來一次
            #endregion 
            MapGL.Show();
            this.TopMost = false;
        }

        /// <summary>
        /// 離開程式
        /// </summary>
        private void Exit() {
            this.Dispose();
        }

        /// <summary>
        /// 切換CtDockContemt可視狀態
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dockContent">CtDockContent物件</param>
        /// <param name="vis">可視狀態</param>
        private void DockContentVisible(ICtDockContent dockContent,bool vis) {
            try {
                if (vis) {
                    dockContent.ShowWindow();
                } else {
                    dockContent.HideWindow();
                }
            } catch(Exception ex) {
                string msg = ex.Message;
            }

        }

        /// <summary>
        /// 控制DockContent物件致能
        /// </summary>
        /// <param name="mapGL"></param>
        /// <param name="console"></param>
        /// <param name="testing"></param>
        /// <param name="goalSetting"></param>
        private void DockContentEnable(bool mapGL,bool console,bool testing,bool goalSetting) {
            CtInvoke.ToolStripItemEnable(miMapGL,mapGL);
            CtInvoke.ToolStripItemEnable(miConsole, console);
            CtInvoke.ToolStripItemEnable(miTesting, testing);
            CtInvoke.ToolStripItemEnable(miGoalSetting, goalSetting);

            DockContentVisible(MapGL, mapGL);
            DockContentVisible(Console, console);
            DockContentVisible(GoalSetting, goalSetting);
            DockContentVisible(Testing, testing);
        }

        /// <summary>
        /// 設定DockContent以及與之相關ToolStripMenuItem控制項之Visible屬性
        /// </summary>
        /// <param name="item">相關<see cref="ToolStripMenuItem"/>控制項</param>
        /// <param name="visible">是否可視</param>
        private void DockContentVisible(ToolStripMenuItem item,bool visible) {
            if (mDockContent.ContainsKey(item)) {
                DockContentVisible(mDockContent[item], visible);
                CtInvoke.ToolStripItemEnable(item, visible);
            }
        }

        /// <summary>
        /// 車子資訊更新事件
        /// </summary>
        ///<param name="info">車子資訊</param>
        private void CarInfoRefresh(CarInfo info) {
            int battery = info.PowerPercent;
            tsprgBattery.Value = battery;
            tslbBattery.Text = string.Format(tslbBattery.Tag.ToString(), battery);
            tslbStatus.Text = info.Status;
        }

        /// <summary>
        /// 依照使用者權限切換介面配置
        /// </summary>
        /// <param name="usrLv"></param>
        private void UserChanged(UserData usrData) {
            AccessLevel usrLv = usrData.Level;
            string title = string.Empty;//工具列選項標題
            string usrName = string.Empty;//狀態列帳號名稱
            bool allowUsrMan = usrLv < AccessLevel.OPERATOR;

            /*-- 依照權限切換模組可視層級 --*/
            switch (usrLv) {
                case AccessLevel.NONE:
                    DockContentVisible(miMapGL, false);
                    DockContentVisible(miConsole, true);
                    DockContentVisible(miTesting, false);
                    DockContentVisible(miGoalSetting, false);
                    miBypass.Visible = false;
                    break;
                case AccessLevel.OPERATOR:
                    DockContentVisible(miMapGL, true);
                    DockContentVisible(miConsole, true);
                    DockContentVisible(miGoalSetting, true);
                    break;
                case AccessLevel.ENGINEER:
                case AccessLevel.ADMINISTRATOR:
                    DockContentVisible(miMapGL, true);
                    DockContentVisible(miConsole, true);
                    DockContentVisible(miGoalSetting, true);
                    DockContentVisible(miTesting, true);
                    miBypass.Visible = true;
                        
                    break;
            }

            /*-- 顯示帳號相關資訊 --*/
            if (usrLv == AccessLevel.NONE) {
                title = "Login";
                usrName = "No account";
            } else {
                title = "Logout";
                usrName = usrData.Account;
            }
            CtInvoke.ToolStripItemText(miLogin, title);
            CtInvoke.ToolStripItemVisible(miUserManager, allowUsrMan);
            tslbAccessLv.Text = usrLv.ToString();
            tslbUserName.Text = usrName;
        }


        #endregion Function - Private Methods
        SocketTest testFrom = null;
        private void testToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!testFrom?.IsDisposed ?? true) {
                testFrom = new SocketTest();
                testFrom.Show();
            }
        }
    }

    #region Suppor - Class

    /// <summary>
    /// 系統列圖示類
    /// </summary>
    public class CtNotifyIcon : IDisposable {

        #region Declaration - Field

        /// <summary>
        /// 系統列圖示右鍵選單物件
        /// </summary>
        private ContextMenu mContextMenu = new ContextMenu();

        /// <summary>
        /// 右鍵圖示物件
        /// </summary>
        private NotifyIcon mNotifyIcon = new NotifyIcon();

        /// <summary>
        /// 要隱藏的表單參考
        /// </summary>
        private Form rForm = null;

        /// <summary>
        /// 右鍵選項集合
        /// </summary>
        private MenuItems mMenuItems = null;

        #endregion Declaration - Field

        #region Declaration - Properties

        /// <summary>
        /// 系統列圖示是否可視
        /// </summary>
        public bool Visible { get { return mNotifyIcon.Visible; } set { mNotifyIcon.Visible = value; } }

        /// <summary>
        /// 右鍵選項集合
        /// </summary>
        public MenuItems MenuItems {
            get {
                return mMenuItems;
            }
            set {
                mMenuItems = value;
                mContextMenu.MenuItems.AddRange(mMenuItems.Items.ToArray());
            }
        }

        #endregion Declaration - Properties

        #region Declaration - Events

        /// <summary>
        /// 滑鼠雙擊系統列視窗事件
        /// </summary>
        public event MouseEventHandler OnMouseDoubleClick {
            add {
                mNotifyIcon.MouseDoubleClick += value;
            }
            remove {
                mNotifyIcon.MouseDoubleClick -= value;
            }
        }

        /// <summary>
        /// 滑鼠放開事件
        /// </summary>
        public event MouseEventHandler OnMouseUp {
            add {
                mNotifyIcon.MouseUp += value;
            }
            remove {
                mNotifyIcon.MouseUp -= value;
            }

        }

        #endregion Declaration - Events

        #region Function - Consturctors

        /// <summary>
        /// 一般建構方法
        /// </summary>
        /// <param name="form">要隱藏的表單參考</param>
        /// <param name="caption">系統列圖示標題</param>
        /// <param name="icon">系統列圖示Icon</param>
        public CtNotifyIcon(Form form, string caption = "NotifyIcon", Icon icon = null) {
            rForm = form;
            mNotifyIcon.Icon = icon  ?? rForm.Icon;
            mNotifyIcon.Text = caption;
            mNotifyIcon.ContextMenu = mContextMenu;
        }

        #endregion Function - Consturctors

        #region Function - Public Methods

        /// <summary>
        /// 增加右鍵選項
        /// </summary>
        /// <param name="item"></param>
        public void MenuItemAdd(MenuItem item) {
            if (!mContextMenu.MenuItems.Contains(item)) mContextMenu.MenuItems.Add(item);
        }

        /// <summary>
        /// 移除右鍵選項
        /// </summary>
        /// <param name="item"></param>
        public void MenuItemRemove(MenuItem item) {
            if (mContextMenu.MenuItems.Contains(item)) mContextMenu.MenuItems.Remove(item);
        }

        /// <summary>
        /// 顯示系統列圖示
        /// </summary>
        public void ShowIcon() {
            mNotifyIcon.Visible = true;
        }

        /// <summary>
        /// 隱藏系統列圖示
        /// </summary>
        public void HideIcon() {
            mNotifyIcon.Visible = false;
        }

        /// <summary>
        /// 顯示系統列提示
        /// </summary>
        /// <param name="title"></param>
        /// <param name="context"></param>
        /// <param name="tmo">多久以後關閉</param>
        /// <param name="icon">Icon類型</param>
        public void ShowBalloonTip(string title, string context, int tmo = 15, ToolTipIcon icon = ToolTipIcon.Info) {
            mNotifyIcon.ShowBalloonTip(tmo, title, context, icon);
        }

        /// <summary>
        /// 顯示右鍵選單
        /// </summary>
        public void ShowMenuItem() {
            /*-- 以反射方式執行ShowContextMenu方法顯示右鍵選單 --*/
            Type t = typeof(NotifyIcon);
            MethodInfo mi = t.GetMethod("ShowContextMenu", BindingFlags.NonPublic | BindingFlags.Instance);
            mi.Invoke(this.mNotifyIcon, null);
        }

        #endregion Function - Public Methods

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: 處置 Managed 狀態 (Managed 物件)。

                }

                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。

                HideIcon();

                (mMenuItems as MenuItems)?.Dispose();
                mMenuItems = null;

                mNotifyIcon?.Dispose();
                mNotifyIcon = null;

                mContextMenu?.Dispose();
                mContextMenu = null;

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        // ~CtNotifyIcon() {
        //   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose() {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
            // GC.SuppressFinalize(this);
        }
        #endregion


    }

    /// <summary>
    /// 右鍵選單管理類
    /// </summary>
    /// <remarks>
    /// 用於建立
    /// </remarks>
    public class MenuItems : IDisposable {

        #region Declaration - Fields

        /// <summary>
        /// 右鍵選單集合
        /// </summary>
        private List<MenuItem> mMenuItems = new List<MenuItem>();

        /// <summary>
        /// 右鍵選單事件集合
        /// </summary>
        private List<EventHandler> mClickEvents = new List<EventHandler>();

        #endregion Declaration -Fields

        #region Declaration Properties

        /// <summary>
        /// 右鍵選單集合
        /// </summary>
        public List<MenuItem> Items {
            get {
                return mMenuItems;
            }
        }

        #endregion Declaration - Properties

        #region Function - Public Methods

        /// <summary>
        /// 傳入選單標題、方法、是否可視，建立新的右鍵選單後加入集合
        /// </summary>
        /// <param name="caption">選單標題</param>
        /// <param name="even">選單觸發時的處理方法</param>
        /// <param name="enable">是否可視</param>
        /// <returns>建構出的<see cref="MenuItem"/></returns>
        public MenuItem AddMenuItem(string caption, Action<object, EventArgs> even = null, bool enable = true) {
            MenuItem item = new MenuItem();
            item.Text = caption;
            item.Index = mMenuItems.Count;
            item.Enabled = enable;
            EventHandler handler = even == null ? null : new EventHandler(even);
            mClickEvents.Add(handler);

            if (even != null) item.Click += handler;

            mMenuItems.Add(item);
            return item;
        }

        /// <summary>
        /// 從右鍵選單集合中移除指定物件
        /// </summary>
        /// <param name="item">目標物件</param>
        public void RemoveMenuItem(MenuItem item) {
            if (!mMenuItems.Contains(item)) return;

            int index = mMenuItems.IndexOf(item);

            if (mClickEvents[index] != null) item.Click -= mClickEvents[index];
        }

        /// <summary>
        /// 清空右鍵選單集合
        /// </summary>
        public void Clear() {
            for (int i = 0; i < mMenuItems.Count; i++) {
                if (mClickEvents[i] != null) mMenuItems[i].Click -= mClickEvents[i];
                mMenuItems[i].Dispose();
                mMenuItems[i] = null;
            }
            mMenuItems.Clear();
            mMenuItems = null;

            mClickEvents.Clear();
            mClickEvents = null;
        }

        #endregion Funciton - Public Methods

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: 處置 Managed 狀態 (Managed 物件)。
                    Clear();
                }

                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        // ~MenuItems() {
        //   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose() {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

    /// <summary>
    /// 提供框線顏色設定功能之GroupBox
    /// </summary>
    public class CtGroupBox : GroupBox {
        private Color _BorderColor = Color.Red;
        [Description("設定或取得外框顏色")]
        public Color BorderColor {
            get { return _BorderColor; }
            set { _BorderColor = value; }
        }

        protected override void OnPaint(PaintEventArgs e) {
            //取得text字型大小
            Size FontSize = TextRenderer.MeasureText(this.Text,
                                                     this.Font);

            //畫框線
            #region 原始範例
            //Rectangle rec = new Rectangle(e.ClipRectangle.Y,
            //                              this.Font.Height / 2,
            //                              e.ClipRectangle.Width - 1,
            //                              e.ClipRectangle.Height - 1 -
            //                              this.Font.Height / 2);
            //e.ClipRectangle這個值在自動隱藏停靠視窗下似乎是變動
            //因此改用固定的GroupBox.Size
            #endregion 原始範例

            Rectangle rec = new Rectangle(0,
                                          this.Font.Height / 2,
                                          this.Width - 1,
                                          this.Height - 1 -
                                          this.Font.Height / 2);


            e.Graphics.DrawRectangle(new Pen(BorderColor), rec);

            //填滿text的背景
            e.Graphics.FillRectangle(new SolidBrush(this.BackColor),
                new Rectangle(6, 0, FontSize.Width, FontSize.Height));

            //text
            e.Graphics.DrawString(this.Text, this.Font,
                new Pen(this.ForeColor).Brush, 6, 0);
        }
    }

    #endregion Support - Class

}
