using CtBind;
using CtDockSuit;
using CtLib.Forms;
using CtLib.Library;
using CtLib.Module.Utility;
using CtNotifyIcon;
using CtParamEditor.Comm;
using SerialCommunicationData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VehiclePlanner.Core;
using VehiclePlanner.Forms;
using VehiclePlanner.Module;
using VehiclePlanner.Module.Implement;
using VehiclePlanner.Module.Interface;
using VehiclePlanner.Partial.VehiclePlannerUI;
using WeifenLuo.WinFormsUI.Docking;

namespace VehiclePlanner {

    /// <summary>
    /// 客戶端介面
    /// </summary>
    public partial class BaseVehiclePlanner_Ctrl : Form, ICtVersion, IDataDisplay<IBaseVehiclePlanner>, IDataDisplay<IBaseITSController> {

        #region Version - Information

        /// <summary>
        /// AgvClientUI版本資訊
        /// </summary>
        /// <remarks>
        ///     0.0.0   Jay [2017/09/28]
        ///         + 整合AgvClient
        ///         + 加上使用者登入/登出
        ///         + 加上使用者管理
        ///         + 加入版本號
        ///         \ 修改Load File進度條為百分比
        ///         \ 取得Ori檔後解鎖Ori檔操作
        ///         \ 補上SimplfyOri功能
        ///     0.0.1   Jay [2017/10/03]
        ///         \ 通訊機制修改
        ///         + 於GoalSetting加入GetGoalList功能
        ///         \ 連線時加入IP參數，可在介面指定要連接的IP
        ///         \ 專案建置事件修正
        ///     0.0.2   Jay [2017/10/11]
        ///         \ 地圖修正功能修正
        ///         \ 與AGV連線方式調整
        ///         \ Ori繪製後補上雷射圖層清除
        ///     0.0.3   Jay [2017/10/12]
        ///         \ 更新部分語法以符合新版本CtLib
        ///     0.0.4   Jay [2017/10/23]
        ///         \ 命令傳送用Socket連線方式修改，改為持續連接
        ///     0.0.5   Jay [2017/11/08]
        ///         \ 加入Power點相關操作
        ///         \ 補上ClearMap功能
        ///         \ Map檔格式修改
        ///     0.0.6   Jay [2017/11/22]
        ///         \ 地圖匹配功能修正
        ///         \ 路徑規劃、跑點功能修正
        ///         \ 加入Charging功能
        ///         \ Map讀檔方法重構
        ///     0.0.7   Jay [2017/11/27]
        ///         \ 加入鍵盤控制AGV移動功能
        ///         \ 移除Ori修正功能
        ///     0.0.8   Jay [2017/11/29]
        ///         \ 限定MapGL不可隱藏
        ///     0.0.9   Jay [2017/11/30]
        ///         \ 加入Outlookbar控制項實作工具箱視窗UI
        ///         \ 地圖插入控制面板實作
        ///         \ 移除KeyboardHook
        ///         \ 單獨監測Testing視窗之鍵盤事件
        ///     0.0.10  Jay [2017/12/06]
        ///         \ 加入路徑相關操作鎖定，必須在地圖相似度80%以上才可進行路徑相關操作
        ///     0.0.11  Jay [2017/12/07]
        ///         \ 優化Database與GoalSetting之間的聯動性
        ///         \ 於底層路徑相關操作加入相似度門檻值鎖定
        ///     0.0.12  Jay [2017/12/12]
        ///         + 加入AGV移動控制器
        ///     0.0.13  Jay [2017/12/14]
        ///         \ 將所有按鈕解鎖，當無法處於執行按鈕功能狀態，以對話視窗引導使用者進行狀態修正
        ///         \ 重寫模組權限(加入CtToolBox控制)
        ///         \ 重寫AGVMapUI的ShowWindow與HideWindow方法
        ///     1.0.0   Jay [2018/04/18]
        ///         \ 權限綁定
        /// </remarks>
        public CtVersion Version { get { return new CtVersion(1, 0, 0, "2017/04/18", "Jay Chang"); } }

        #endregion Version - Information

        #region Declaration - Fields

        /// <summary>
        /// 是否正在設定Car Position
        /// </summary>
        protected bool mIsSetting = false;

        /// <summary>
        /// MapGL當前滑鼠模式
        /// </summary>
        protected CursorMode mCursorMode = CursorMode.Select;

        /// <summary>
        /// 系統底層物件參考
        /// </summary>
        private IBaseVehiclePlanner rVehiclePlanner = null;

        private IntPtr mHandle = IntPtr.Zero;

        /// <summary>
        /// 系統列圖示標題
        /// </summary>
        protected string mNotifyCaption = "Vehicle planner";

        #endregion Declaration - Fields

        #region Declaration - Members

        #region UI

        /// <summary>
        /// ICtDockContainer與MenuItem對照
        /// </summary>
        protected Dictionary<ToolStripMenuItem, AuthorityDockContainer> mDockContent = new Dictionary<ToolStripMenuItem, AuthorityDockContainer>();

        /// <summary>
        /// AGV移動控制器
        /// </summary>
        private CtMotionController mMotionController = null;

        /// <summary>
        /// 地圖檔選擇清單
        /// </summary>
        private MapList mMapList = null;

        /// <summary>
        /// 系統列圖示物件
        /// </summary>
        private CtNotifyICon mNotifyIcon = null;

        /// <summary>
        /// 系統列圖示右鍵選單
        /// </summary>
        private MenuItems mMenuItems = null;

        #endregion UI

        #region Tool

        /// <summary>
        /// 模組版本集合
        /// </summary>
        private Dictionary<string, string> mModuleVersions = new Dictionary<string, string>();

        /// <summary>
        /// iTS控制器
        /// </summary>
        private IBaseITSController Controller { get => rVehiclePlanner.Controller as IBaseITSController; }

        #endregion Tool


        #endregion Declaration - Members

        #region Declaration - Properties

        /// <summary>
        /// MapGL子視窗
        /// </summary>
        private IBaseMapGL MapGL {
            get {
                return mDockContent.ContainsKey(miMapGL) ? mDockContent[miMapGL] as IBaseMapGL : null;
            }
        }

        /// <summary>
        /// Console子視窗
        /// </summary>
        private IConsole mConsole {
            get {
                return mDockContent.ContainsKey(miConsole) ? mDockContent[miConsole] as IConsole : null;
            }
        }

        /// <summary>
        /// 測試子視窗
        /// </summary>
        private ITesting mTesting {
            get {
                return mDockContent.ContainsKey(miTesting) ? mDockContent[miTesting] as ITesting : null;
            }
        }

        /// <summary>
        /// Goal點設定子視窗
        /// </summary>
        private IBaseGoalSetting mGoalSetting {
            get {
                return mDockContent.ContainsKey(miGoalSetting) ? mDockContent[miGoalSetting] as IBaseGoalSetting : null;
            }
        }

        /// <summary>
        /// 是否可視
        /// </summary>
        public new bool Visible {
            get => base.Visible; set {
                if (base.Visible != value) {
                    if (value) {
                        this.Show();
                        this.TopMost = true;

                        #region 把DocDocument切回來

                        /// 由於主介面關閉的時候會觸發到DockDocument的FormCloseing事件
                        /// 導致子介面被隱藏
                        /// 這邊在手動把他切回來一次

                        #endregion 把DocDocument切回來

                        MapGL.Show();
                        this.TopMost = false;
                    } else {
                        this.Hide();
                    }
                }
            }
        }

        /// <summary>
        /// iTS手動控制速度
        /// </summary>
        public int Velocity { get => Controller.Velocity; }
        
        #endregion Declaration - Properties

        #region Functin - Constructors

        protected BaseVehiclePlanner_Ctrl() {
            InitializeComponent();
        }

        internal BaseVehiclePlanner_Ctrl(IBaseVehiclePlanner vehiclePlanner = null) : this() {
            Initial(vehiclePlanner);
        }

        #endregion Functin - Constructors

        #region Function - Events

        #region CtVehiclePlanner

        /// <summary>
        /// 氣球提示事件處理
        /// </summary>
        /// <param name="title"></param>
        /// <param name="context"></param>
        private void rVehiclePlanner_BalloonTip(string title, string context) {
            SetBalloonTip(title, context);
        }

        /// <summary>
        /// 錯誤訊息事件處理
        /// </summary>
        /// <param name="err">錯誤訊息</param>
        private void rVehiclePlanner_ErrorMessage(string err) {
            CtMsgBox.Show(mHandle, "Error", err, MsgBoxBtn.OK, MsgBoxStyle.Error);
        }

        /// <summary>
        /// VehiclePlanner事件處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rVehiclePlanner_VehiclePlannerEvent(object sender, VehiclePlannerEventArgs e) {
            switch (e.Events) {
                case VehiclePlannerEvents.MarkerChanged:
                    MarkerChanged();
                    break;

                case VehiclePlannerEvents.Dispose:
                    this.Dispose();
                    break;
            }
        }

        /// <summary>
        /// Consle訊息處理
        /// </summary>
        /// <param name="msg">Console訊息</param>
        private void rVehiclePlanner_ConsoleMessage(string msg) {
            OnConsoleMessage(msg);
        }

        /// <summary>
        /// VehiclePlanner屬性變更事件處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rVehiclePlanner_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(IBaseVehiclePlanner.UserData):
                    UserChanged(rVehiclePlanner.UserData);
                    break;
            }
        }

        #endregion CtVehiclePlanner

        #region Form

        /// <summary>
        /// 表單載入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientUI_Load(object sender, EventArgs e) {

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
            #endregion 取消程式關閉

            e.Cancel = true;

            HideWindow();
        }

        #endregion Form

        #region MenuItem

        /// <summary>
        /// DockContent對應MenuItem按下之事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuDock_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            /*-- 確認是否有對應DockContent物件 --*/
            if (mDockContent.ContainsKey(item)) {
                if (item.Checked) {
                    (mDockContent[item] as CtDockContainer).Visible = false;
                } else {
                    mDockContent[item].Visible = true;
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
            using (CtAbout frm = new CtAbout()) {
                //新版本CtLib
                //frm.Start(Assembly.GetExecutingAssembly(), this, Version, module);
                //當前版本CtLib
                frm.Start(Version, mModuleVersions);
            }
        }

        /// <summary>
        /// 切換使用者
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miLogin_Click(object sender, EventArgs e) {
            Stat stt = Stat.SUCCESS;
            var usrData = new UserData("N/A", "", AccessLevel.None);
            if (rVehiclePlanner.UserData.Level == AccessLevel.None) {
                using (CtLogin frmLogin = new CtLogin()) {
                    stt = frmLogin.Start(out usrData);
                }
            }
            if (stt == Stat.SUCCESS) {
                rVehiclePlanner.UserData = usrData;
            }
        }

        /// <summary>
        /// 使用者管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miUserManager_Click(object sender, EventArgs e) {
            using (CtUserManager frmUsrMgr = new CtUserManager(UILanguage.English)) {
                frmUsrMgr.ShowDialog();
            }
        }

        /// <summary>
        /// Socket Bypass
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miBypassSocket_Click(object sender, EventArgs e) {
            Controller.IsBypassSocket = !Controller.IsBypassSocket;
        }

        /// <summary>
        /// Load File Bypass
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miLoadFile_Click(object sender, EventArgs e) {
            rVehiclePlanner.IsBypassLoadFile = !rVehiclePlanner.IsBypassLoadFile;
        }

        /// <summary>
        /// AGV移動控制面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miMotionController_Click(object sender, EventArgs e) {
            ShowMotionController();
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
            CtDockContainer dockWnd = sender as CtDockContainer;

            /*--取得對應MenuItem物件--*/
            ToolStripMenuItem menuItem = mDockContent.First(v => v.Value == dockWnd).Key;

            /*-- 依照DockState切換MenuItem的Check狀態 --*/
            if (menuItem != null) menuItem.Checked = dockWnd.DockState != DockState.Hidden;
        }

        #endregion DockContent

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
        private void ShowWindow_OnClick(object sender, EventArgs e) {
            ShowWindow();
        }

        #endregion NotityIcon

        #region ITest

        /// <summary>
        /// 切換SetCar旗標
        /// </summary>
        internal void ITest_SettingCarPos() => mIsSetting = true;

        /// <summary>
        /// 傳送Map檔
        /// </summary>
        internal void ITest_SendMap() {
            OpenFileDialog openMap = new OpenFileDialog() {
                InitialDirectory = rVehiclePlanner.DefMapDir,
                Filter = "MAP|*.map"
            };
            if (openMap.ShowDialog() == DialogResult.OK) {
                try {
                    Task.Run(() => rVehiclePlanner.Controller.SendAndSetMap(openMap.FileName));
                } catch (Exception ex) {
                    OnConsoleMessage(ex.Message);
                }
            }
        }

        /// <summary>
        /// 要求VehicleConsole自動回傳資料
        /// </summary>
        internal void ITest_GetCar() => Task.Run(() => rVehiclePlanner.Controller.AutoReport(!rVehiclePlanner.Controller.IsAutoReport));

        /// <summary>
        /// 載入Map檔
        /// </summary>
        /// <returns></returns>
        internal void ITest_LoadMap() {
            try {
                LoadFile(FileType.Map);
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 載入Ori檔
        /// </summary>
        /// <returns></returns>
        internal void ITest_LoadOri() {
            try {
                LoadFile(FileType.Ori);
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 停止手動控制
        /// </summary>
        internal void ITest_Motion_Up() => rVehiclePlanner.Controller.MotionContorl(MotionDirection.Stop);

        /// <summary>
        /// 尋找可用的車子
        /// </summary>
        internal void FindCar() => rVehiclePlanner.Controller.FindCar();

        /// <summary>
        /// 開始掃描地圖
        /// </summary>
        /// <param name="scan">是否開始掃描</param>
        internal void StartScan() => Task.Run(() => rVehiclePlanner.Controller.StartScan(!rVehiclePlanner.Controller.IsScanning));

        /// <summary>
        /// 車子位置微調修正
        /// </summary>
        internal void CarPosConfirm() => Task.Run(() =>  rVehiclePlanner.Controller.DoPositionComfirm());
        
        /// <summary>
        /// 啟動馬達激磁
        /// </summary>
        /// <param name="servoOn">是否啟動馬達激磁</param>
        internal void MotorServoOn() => Task.Run(() => rVehiclePlanner.Controller.SetServoMode(!rVehiclePlanner.Controller.IsMotorServoOn));
        
        /// <summary>
        /// 清除地圖
        /// </summary>
        internal void ClearMap() => rVehiclePlanner.ClearMap();
        
        /// <summary>
        /// 設定iTS工作移動速度
        /// </summary>
        /// <param name="velocity">移動速度</param>
        internal void SetVelocity(int velocity) => Task.Run(()=> rVehiclePlanner.Controller.SetWorkVelocity(velocity));
        
        /// <summary>
        /// 要求雷射資料
        /// </summary>
        /// <returns>雷射資料(0筆雷射資料表示失敗)</returns>
        internal void GetLaser()  => Task.Run(() => rVehiclePlanner.Controller.RequestLaser());

        /// <summary>
        /// 取得Map檔
        /// </summary>
        internal void GetMap() => Task.Run(()=> rVehiclePlanner.Controller.GetMap());

        /// <summary>
        /// 連線至iTS
        /// </summary>
        /// <param name="cnn">連線/斷線</param>
        internal void Connect() {
            if (rVehiclePlanner.Controller.IsConnected) {
                Task.Run(() => {
                    rVehiclePlanner.Controller.ConnectToITS();
                });
            } else {
                using (ConnectITS form = new ConnectITS(rVehiclePlanner.Controller)) {
                    if (form.GetIP(out string ip)) {
                        rVehiclePlanner.Controller.HostIP = ip;
                        Task.Run(() => {
                            rVehiclePlanner.Controller.ConnectToITS();
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 顯示當前Goal點名稱清單
        /// </summary>
        internal void GetGoalName() =>rVehiclePlanner.Controller.GetGoalNames();

        /// <summary>
        /// 新增當前位置為Goal點
        /// </summary>
        internal void AddNow() => rVehiclePlanner.AddCurrentAsGoal();

        /// <summary>
        /// 搜尋至Goal點路徑(透過Goal名稱)
        /// </summary>
        /// <param name="goalName">Goal點名稱</param>
        internal void FindPath(string goalName) => Task.Run(() =>  rVehiclePlanner.Controller.FindPath(goalName));

        /// <summary>
        /// 移至Goal(透過Goal點名稱)
        /// </summary>
        /// <param name="goalName">Goal點名稱</param>
        internal void Run(string goalName) => Task.Run(() => rVehiclePlanner.Controller.DoRunningByGoalName(goalName));

        /// <summary>
        /// 刪除指定標記物
        /// </summary>
        /// <param name="markers"></param>
        internal void Delete(List<uint> id) => rVehiclePlanner.DeleteMarker(id);

        /// <summary>
        /// 儲存地圖
        /// </summary>
        internal void SaveMap() => rVehiclePlanner.SaveMap();

        /// <summary>
        /// 到指定充電站進行充電
        /// </summary>
        /// <param name="powerName">充電站名稱</param>
        /// <returns>是否開始進行充電</returns>
        internal void Charging(string powerName) => Task.Run(() => rVehiclePlanner.Controller.DoCharging(powerName));

        #endregion ITest

        #region ToolBox

        /// <summary>
        /// 工具箱切換工具事件
        /// </summary>
        /// <param name="mode"></param>
        protected virtual void ToolBox_SwitchCursor(CursorMode mode) { }

        #endregion ToolBox

        #region ToolStripButton
        
        /// <summary>
        /// 連線至iTS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbConnect_Click(object sender, EventArgs e) {
            Connect();
        }

        /// <summary>
        /// 從iTS下載Map檔
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbGetMap_Click(object sender, EventArgs e) {
            GetMap();
        }

        /// <summary>
        /// 上傳Map檔到iTS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbSendMap_Click(object sender, EventArgs e) {
            ITest_SendMap();
        }

        #endregion ToolStripButton

        #endregion Function - Events

        #region Function - Private Methods

        #region UI

        /// <summary>
        /// 依照使用者權限切換介面配置
        /// </summary>
        /// <param name="usrLv"></param>
        private void UserChanged(UserData usrData) {
            foreach (var kvp in mDockContent) {
                AuthorityDockContainer subForm = kvp.Value;
                subForm.AuthorityVisiable(usrData.Level);
            }
        }
        
        /// <summary>
        /// 顯示iTS手動移動控制面板
        /// </summary>
        internal void ShowMotionController() {
            if (mMotionController == null) {
                mMotionController = new CtMotionController(rVehiclePlanner.Controller);
                mMotionController.MotionDown += rVehiclePlanner.Controller.MotionContorl;
                mMotionController.MotionUp += ITest_Motion_Up;
                miMotionController.Checked = true;
                mMotionController.FormClosing += (fSender, fE) => {
                    mMotionController.MotionDown -= rVehiclePlanner.Controller.MotionContorl;
                    mMotionController.MotionUp -= ITest_Motion_Up;
                    mMotionController = null;
                    miMotionController.Checked = false;
                };
                mMotionController.Show();
            }
        }

        /// <summary>
        /// 顯示氣球提示
        /// </summary>
        /// <param name="title">提示標題</param>
        /// <param name="context">提示內容</param>
        /// <param name="icon">提示Icon</param>
        /// <param name="tmo">顯示時間</param>
        private void SetBalloonTip(string title, string context, ToolTipIcon icon = ToolTipIcon.Info, int tmo = 5) {
            mNotifyIcon.ShowBalloonTip(title, context, tmo, icon);
        }

        /// <summary>
        /// 顯示主介面
        /// </summary>
        public void ShowWindow() {
            mNotifyIcon.HideIcon();
            this.Show();
        }

        /// <summary>
        /// 將主介面縮小至系統列
        /// </summary>
        public void HideWindow() {
            this.Hide();
            mNotifyIcon.ShowIcon();
        }

        /// <summary>
        /// 離開程式
        /// </summary>
        public void Exit() {
            mNotifyIcon.HideIcon();
            rVehiclePlanner.Dispose();
            this.Dispose();
        }

        #endregion UI

        #region DockContent

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
            Size dockSize = new Size() {
                Width = dockContents.Max(v => v.Value.FixedSize.Width),
                Height = dockContents.Max(v => v.Value.FixedSize.Height)
            };

            /*-- 依照停靠區域計算所需顯示大小 --*/
            if (area.CalculatePortion(dockSize, out double portion)) {
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
        
        #endregion DockContent

        #region Draw

        /// <summary>
        /// 載入檔案
        /// </summary>
        /// <param name="type">載入檔案類型</param>
        public void LoadFile(FileType type) {
            OpenFileDialog openMap = new OpenFileDialog() {
                InitialDirectory = rVehiclePlanner.DefMapDir,
                Filter = $"MAP|*.{type.ToString().ToLower()}"
            };
            if (openMap.ShowDialog() == DialogResult.OK) {
                Task.Run(() => {
                    rVehiclePlanner.LoadFile(type, openMap.FileName);
                });
            }
        }

        #endregion Draw

        #region Load

        protected virtual void Initial(IBaseVehiclePlanner vehiclePlanner) {

            mHandle = this.Handle;
            /*-- 系統底層實例取得 --*/
            rVehiclePlanner = vehiclePlanner ?? FactoryMode.Factory.CtVehiclePlanner();
            if (rVehiclePlanner != null) {
                /*-- 初始化 --*/
                rVehiclePlanner.Initial();

                /*-- 事件委派 --*/
                rVehiclePlanner.PropertyChanged += rVehiclePlanner_PropertyChanged;
                rVehiclePlanner.VehiclePlannerEvent += rVehiclePlanner_VehiclePlannerEvent;
                rVehiclePlanner.ErrorMessage += rVehiclePlanner_ErrorMessage;
                if (rVehiclePlanner.Controller != null) {
                    /*-- 方法委派 --*/
                    rVehiclePlanner.Controller.BalloonTip += rVehiclePlanner_BalloonTip; ;
                    rVehiclePlanner.Controller.ConsoleMessage += rVehiclePlanner_ConsoleMessage;
                    rVehiclePlanner.Controller.SelectFile = SelectFile;
                    rVehiclePlanner.Controller.InputBox = InputBox;
                }
                /*-- 載入ICtDockContainer物件 --*/
                LoadICtDockContainer();

                LoadCtNotifyIcon();

            }
        }

        /// <summary>
        /// 設定事件連結
        /// </summary>
        protected virtual void SetEvents() {
            if (rVehiclePlanner != null) {
                

                if (rVehiclePlanner.Controller != null) {
                    
                }
            }
            
        }
        
        protected virtual CtConsole GetConsole(DockState dockState) {
            return new CtConsole(this, dockState) ;
        }

        protected virtual BaseTesting GetTesting(DockState dockState) {
            return new BaseTesting(this,dockState) ;
        }

        protected virtual BaseMapGL GetMapGL(DockState dockState) {
            return new BaseMapGL(this, dockState);
        }
        protected virtual BaseGoalSetting GetGoalSetting(DockState dockState) {
            return new BaseGoalSetting(this,dockState) ;
        }

        /// <summary>
        /// 載入ICtDockContainer物件
        /// </summary>
        protected virtual void LoadICtDockContainer() {
            /*-- 載入DockContent --*/
            AddSubForm(miMapGL, GetMapGL(DockState.Document));
            AddSubForm(miConsole, GetConsole(DockState.DockBottomAutoHide));
            AddSubForm(miGoalSetting, GetGoalSetting(DockState.DockLeft));
            AddSubForm(miParamEditor,new ParamEditor(this,DockState.Document));
            SetEvents();

            /*-- 計算每個固定停靠區域所需的顯示大小 --*/
            foreach (var area in Enum.GetValues(typeof(DockAreas))) {
                CalculateFixedPortion(dockPanel, (DockAreas)area);
            }

            /*-- 遍歷所有DockContent與MenuItem物件 --*/
            foreach (var kvp in mDockContent) {
                ToolStripMenuItem item = kvp.Key;
                CtDockContainer dokContent = kvp.Value as CtDockContainer;

                /*-- 參考分配 --*/
                dokContent.AssignmentDockPanel(dockPanel);

                /*-- 顯示視窗 --*/
                if (dokContent.DefaultDockState != DockState.Hidden &&
                    dokContent.DefaultDockState != DockState.Unknown) {
                    dokContent.Visible = true;
                }

                /*-- 訂閱DockDockStateCHanged事件 --*/
                dokContent.DockStateChanged += Value_DockStateChanged;

                /*-- 依照DockState狀態顯示MenuItem的Check狀態 --*/
                item.Checked = dokContent.DefaultDockState != DockState.Hidden;

                /*-- MenuItem顯示DockContent標題文字(Text) --*/
                item.Text = dokContent.Text;

                /*-- 委派工具列點擊事件 --*/
                item.Click += MenuDock_Click;
            }
            /*-- 資料綁定 --*/
            Bindings(rVehiclePlanner);
            Bindings(Controller);
        }

        /// <summary>
        /// 載入CtNotifyIcon物件
        /// </summary>
        private void LoadCtNotifyIcon() {
            if (mNotifyIcon == null) {
                Icon icon = Properties.Resources.CASTEC;
                mNotifyIcon = new CtNotifyICon(null, mNotifyCaption, icon);
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
        /// 新增MenuItem與子視窗的對照
        /// </summary>
        /// <param name="item"></param>
        /// <param name="subForm"></param>
        protected void AddSubForm(ToolStripMenuItem item,AuthorityDockContainer subForm) {
            mDockContent.Add(item, subForm);
            item.Tag = subForm;
        }

        #endregion Load

        /// <summary>
        /// 從檔案清單選擇一個檔案
        /// </summary>
        /// <param name="fileList">檔案清單</param>
        /// <returns>選擇的檔案</returns>
        private string SelectFile(string fileList) {
            string fileName = null;
            using (mMapList = new MapList(fileList)) {
                var result = this.InvokeIfNecessary(() => mMapList.ShowDialog(this));
                if (result == DialogResult.OK) {
                    fileName = mMapList.strMapList;
                }
            }
            return fileName;
        }

        /// <summary>
        /// 文字輸入方法
        /// </summary>
        /// <param name="oriName"></param>
        /// <param name="title"></param>
        /// <param name="describe"></param>
        /// <returns></returns>
        private bool InputBox(out string oriName, string title, string describe) {
            return Stat.SUCCESS == CtInput.Text(out oriName, title, describe);
        }

        /// <summary>
        /// 顯示Console訊息
        /// </summary>
        /// <param name="msg"></param>
        protected void OnConsoleMessage(string msg) {
            mConsole.AddMsg(msg);
        }

        protected virtual void MarkerChanged() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 選項與子視窗綁定
        /// </summary>
        /// <param name="item">要綁定的選項</param>
        /// <param name="source">要綁定的子視窗</param>
        private void ToolStripMenuItemBinding(Bindable.ToolStripMenuItem item, IBaseVehiclePlanner source) {
            if (item.Tag != null) {
                item.DataBindings.ExAdd(nameof(item.Enabled), source, nameof(source.UserData), (sneder, e) => {
                    e.Value = (e.Value as UserData).Authority(item);
                }, source.UserData.Authority(item));
            } else {
                this.InvokeIfNecessary(() => {
                    item.Visible = false;
                });
            }
        }

        /// <summary>
        /// 快捷鍵
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            bool ret = true;
            switch (keyData) {
                case Keys.C  :
                    Console.WriteLine("Main");
                    //Connect(!Controller.IsConnected);
                    break;
                default:
                    ret = base.ProcessCmdKey(ref msg, keyData);
                    break;
            }
            return ret;
        }

        #endregion Function - Private Methods

        #region Implement - IDataDisplay

        /// <summary>
        /// <see cref="IBaseVehiclePlanner"/>資料綁定
        /// </summary>
        /// <param name="source"></param>
        public void Bindings(IBaseVehiclePlanner source) {
            Bindings<IBaseVehiclePlanner>(source);

            /*-- 是否忽略地圖檔讀寫 --*/
            miLoadFile.DataBindings.Add(nameof(miLoadFile.Checked), source, nameof(source.IsBypassLoadFile));
            /*-- 是否可視 --*/
            this.DataBindings.Add(nameof(Visible), source, nameof(source.MainVisible));
            /*-- 使用者資訊 --*/
            string dataMember = nameof(source.UserData);
            miLogin.DataBindings.ExAdd(nameof(miLogin.Text), source, dataMember, (sender, e) => {
                e.Value = (e.Value as UserData).Level == AccessLevel.None ? "Login" : "Logout";
            });
            tslbAccessLv.DataBindings.ExAdd(nameof(tslbAccessLv.Text), source, dataMember, (sender, e) => {
                e.Value = (e.Value as UserData).Level.ToString();
            });
            tslbUserName.DataBindings.ExAdd(nameof(tslbUserName.Text), source, dataMember, (sneder, e) => {
                e.Value = (e.Value as UserData).Account;
            });
            miUserManager.DataBindings.ExAdd(nameof(miUserManager.Visible), source, dataMember, (sender, e) => {
                e.Value = (e.Value as UserData).Level > AccessLevel.Operator;
            }, source.UserData.Level > AccessLevel.Operator);
            miBypass.DataBindings.ExAdd(nameof(miBypass.Visible), source, dataMember, (sender, e) => {
                e.Value = (e.Value as UserData).Level == AccessLevel.Administrator;
            }, source.UserData.Level == AccessLevel.Administrator);
            ToolStripMenuItemBinding(miTesting, source);
            ToolStripMenuItemBinding(miGoalSetting, source);
            ToolStripMenuItemBinding(miMapGL, source);
            ToolStripMenuItemBinding(miConsole, source);
            ToolStripMenuItemBinding(miParamEditor, source);
        }

        /// <summary>
        /// <see cref="IBaseITSController"/>資料綁定
        /// </summary>
        /// <param name="source">資料來源</param>
        public void Bindings(IBaseITSController source) {
            if (source == null) return;
            Bindings<IBaseITSController>(source);
            /*-- 電池最大電量 --*/
            tsprgBattery.ProgressBar.DataBindings.Add(nameof(ProgressBar.Maximum), source, nameof(source.BatteryMaximum));
            /*-- 電池最小電量 --*/
            tsprgBattery.ProgressBar.DataBindings.Add(nameof(ProgressBar.Minimum), source, nameof(source.BatteryMinimum));
            /*-- 是否忽略Socket*/
            miBypassSocket.DataBindings.Add(nameof(miBypassSocket.Checked), source, nameof(source.IsBypassSocket));
            /*-- iTS IP --*/
            tslbHostIP.DataBindings.Add(nameof(tslbHostIP.Text), source, nameof(source.HostIP));
            /*-- 連線狀態 --*/
            miBypassSocket.DataBindings.ExAdd(nameof(miBypassSocket.Enabled), source, nameof(source.IsConnected), (sender, e) => {
                e.Value = !(bool)e.Value;
            });
        }

        /// <summary>
        /// 子視窗Bind
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        protected void Bindings<TSource>(TSource source) where TSource : IDataSource {
            if (source == null) return;
            if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);
            var subDisplay = mDockContent.Where(kvp => kvp.Value is IDataDisplay<TSource>).Select(kvp => kvp.Value);
            foreach (IDataDisplay<TSource> display in subDisplay) {
                display.Bindings(source);
            }
        }

        #endregion Implement - IDataDisplay


    }

}