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
using static CtLib.Forms.CtLogin;
using CtLib.Forms;
using MapProcessing;
using System.Threading;
using ServerOperation;
using System.Net;
using System.Net.Sockets;
using ClientUI.Component;
using System.IO;
using AGVMathOperation;
using System.Diagnostics;
using CtLib.Module.Utility;
using System.Text.RegularExpressions;
using Geometry;
using GLCore;
using GLUI;
using CommandCore;
using UIControl;

namespace ClientUI
{

    /// <summary>
    /// 客戶端介面
    /// </summary>
    public partial class AgvClientUI : Form, ICtVersion
    {

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
        /// </remarks>
        public CtVersion Version { get { return new CtVersion(0, 0, 13, "2017/12/14", "Jay Chang"); } }

        #endregion Version - Information
        
        #region Declaration - Fields

        #region Flag

        /// <summary>
        /// 是否正在設定Car Position
        /// </summary>
        private bool mIsSetting = false;

        /// <summary>
        /// 伺服馬達激磁狀態
        /// </summary>
        private bool mIsMotorServoOn = false;

        /// <summary>
        /// 是否已建立連線
        /// </summary>
        private bool mIsConnected = false;

        /// <summary>
        /// 是否Bypass Socket通訊
        /// </summary>
        private bool mBypassSocket = false;

        /// <summary>
        /// 是否Bypass LoadFile功能
        /// </summary>
        private bool mBypassLoadFile = false;

        /// <summary>
        /// 車子模式
        /// </summary>
        private CarMode mCarMode = CarMode.OffLine;

        /// <summary>
        /// 當前語系
        /// </summary>
        /// <remarks>
        /// 未來開發多語系用
        /// </remarks>
        private UILanguage mCulture = UILanguage.English;

        /// <summary>
        /// 當前滑鼠模式
        /// </summary>
        private CursorMode mCursorMode = CursorMode.Select;

        #endregion Flag

        /// <summary>
        /// 當前Map檔路徑
        /// </summary>
        private string mCurMapPath = string.Empty;

        /// <summary>
        /// 系統列圖示標題
        /// </summary>
        protected string mNotifyCaption = "AGV Client";

        /// <summary>
        /// 地圖檔儲存路徑
        /// </summary>
        public string mDefMapDir = @"D:\MapInfo\";

        /// <summary>Opcode 檔案名稱</summary>
        //private static readonly string FILENAME_OPCODE = "D1703.opc";

        /// <summary>
        /// Server端IP
        /// </summary>
        private static string mHostIP = "192.168.50.152";

        /// <summary>
        /// 發送圖片的埠
        /// </summary>
        private static int mFilePort = 600;

        /// <summary>
        /// 接收請求的埠開啟後就一直進行偵聽
        /// </summary>
        private static int mRecvCmdPort = 400;

        /// <summary>
        /// 接收請求的埠開啟後就一直進行偵聽
        /// </summary>
        private static int mCmdPort = 800;

        /// <summary>
        /// 發送地圖的埠
        /// </summary>
        private static int mSendMapPort = 700;

        /// <summary>
        /// 路徑規劃接收埠
        /// </summary>
        private static int mRecvPathPort = 900;
        
        /// <summary>
        /// 車子馬達轉速
        /// </summary>
        private int mVelocity = 500;
        
        /// <summary>
        /// 使用者操作權限
        /// </summary>
        private UserData mUser = new UserData("CASTEC", "", AccessLevel.Administrator);
        
        /// <summary>
        /// AGV ID
        /// </summary>
        private uint mAGVID = 1;

        /// <summary>
        /// 障礙線 ID
        /// </summary>
        private uint mObstacleLinesID = 1;

        /// <summary>
        /// 障礙點 ID
        /// </summary>
        private uint mObstaclePointsID = 1;

        /// <summary>
        /// 地圖相似度
        /// </summary>
        private double mSimilarity = 0;

        /// <summary>
        /// 地圖相似度門檻值
        /// </summary>
        private double mThrSimilarity = 80;

        /// <summary>
        /// 車子資訊
        /// </summary>
        private CarInfo mCarInfo = new CarInfo(0, 0, 0, "AGV", 1);
        
        /// <summary>
        /// Car Position 設定位置
        /// </summary>
        private IPair mNewPos = null;

        private IntPtr mHandle = IntPtr.Zero;
        
        private ConnectFlow mConnectFlow = null;

        private SimilarityFlow mSimilarityFlow = null;

        private ServoOnFlow mServoOnFlow = null;

        #endregion Declaration - Fields

        #region Declaration - Members

        #region UI

        /// <summary>
        /// 地圖插入控制器
        /// </summary>
        private ICtDockContent mMapInsert = new CtMapInsert();

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
        /// AGV移動控制器
        /// </summary>
        private CtMotionController mMotionController = null;

        #endregion UI

        #region Tool

        ///<summary>全域鍵盤鉤子</summary>
        private KeyboardHook mKeyboardHook = new KeyboardHook();

        /// <summary>CtOpcode Object</summary>
        private CtOpcode mOpcode = new CtOpcode();

        /// <summary>
        /// 模組版本集合
        /// </summary>
        private Dictionary<string, string> mModuleVersions = new Dictionary<string, string>();

        #endregion Tool

        #region Socket

        /// <summary>
        /// 命令接收物件
        /// </summary>
        private SocketMonitor mSoxMonitorCmd = null;

        /// <summary>
        /// 地圖資料接收物件
        /// </summary>
        private SocketMonitor mSoxMonitorFile = null;

        /// <summary>
        /// 路徑規劃接收物件
        /// </summary>
        private SocketMonitor mSoxMonitorPath = null;

        /// <summary>
        /// 命令發送用<see cref="Socket"/>
        /// </summary>
        private Socket mSoxCmd = null;

        /// <summary>
        /// Socket通訊物件
        /// </summary>
        private Communication serverComm = new Communication(400, 600, 800);

        #endregion Socket

        #endregion Declaration - Members

        #region Declaration - Properties

        #region Flag

        /// <summary>
        /// 伺服端是否還有在運作
        /// </summary>
        public bool IsServerAlive { get; private set; }

        /// <summary>
        /// 是否已建立連線
        /// </summary>
        public bool IsConnected {
            get {
                return mIsConnected;
            }
            set {
                if (mIsConnected != value) {
                    mIsConnected = value;
                    ITest.SetServerStt(value);
                    IConsole.AddMsg($"{mHostIP} Is {(mIsConnected ? "Connected" : "Disconnected")}");
                }
                //RaiseGoalSettingEvent(GoalSettingEventType.Connect, value);
            }
        }

        public bool IsMotorServoOn {
            get {
                return mIsMotorServoOn;
            }set {
                if (mIsMotorServoOn != value) {
                    mIsMotorServoOn = value;
                    ITest.ChangedMotorStt(value);
                }
            }
        }

        /// <summary>
        /// 是否Bypass Socket功能
        /// </summary>
        public bool IsBypassSocket { get { return mBypassSocket; } set { mBypassSocket = value; } }

        /// <summary>
        /// 是否Bypass LoadFile功能
        /// </summary>
        public bool IsBypassLoadFile { get { return mBypassLoadFile; } set { mBypassLoadFile = value; } }

        /// <summary>
        /// 使用者操作權限
        /// </summary>
        public AccessLevel UserLv { get { return mUser.Level; } }

        /// <summary>
        /// 當前語系
        /// </summary>
        /// <remarks>
        /// 未來開發多語系的時候使用
        /// </remarks>
        public UILanguage Culture { get { return mCulture; } }

        /// <summary>
        /// 是否持續接收雷射資料
        /// </summary>
        public bool IsGettingLaser { get; set; } = false;

        /// <summary>
        /// 車子模式
        /// </summary>
        public CarMode CarMode {
            get {
                return mCarMode;
            }
            set {
            }
        }

        #endregion Flag

        /// <summary>
        /// 車子馬達速度
        /// </summary>
        public int Velocity {
            get {
                return mVelocity;
            }
            set {
                mVelocity = value;
            }
        }

        /// <summary>
        /// Ori檔路徑
        /// </summary>
        public string CurOriPath { get; set; } = string.Empty;

        /// <summary>
        /// Map檔路徑
        /// </summary>
        public string CurMapPath {
            get {
                return mCurMapPath;
            }
            set {
                mCurMapPath = value;
                //RaiseGoalSettingEvent(GoalSettingEventType.CurMapPath, !string.IsNullOrEmpty(value));
            }
        }

        /// <summary>
        /// 車子資訊
        /// </summary>
        public CarInfo CarInfo { get; }

        /// <summary>
        /// 使用者資料
        /// </summary>
        public UserData UserData { get { return mUser; } }

        /// <summary>
        /// 目標設備IP
        /// </summary>
        public string HostIP { get { return mHostIP; } set { mHostIP = value; } }

        /// <summary>
        /// MapGL子視窗
        /// </summary>
        private AGVMapUI MapGL {
            get {
                return mDockContent.ContainsKey(miMapGL) ? mDockContent[miMapGL] as AGVMapUI : null;
            }
        }

        /// <summary>
        /// Console子視窗
        /// </summary>
        private CtConsole Console {
            get {
                return mDockContent.ContainsKey(miConsole) ? mDockContent[miConsole] as CtConsole : null;
            }
        }

        /// <summary>
        /// 測試子視窗
        /// </summary>
        private CtTesting Testing {
            get {
                return mDockContent.ContainsKey(miTesting) ? mDockContent[miTesting] as CtTesting : null;
            }
        }

        /// <summary>
        /// Goal點設定子視窗
        /// </summary>
        private CtGoalSetting GoalSetting {
            get {
                return mDockContent.ContainsKey(miGoalSetting) ? mDockContent[miGoalSetting] as CtGoalSetting : null;
            }
        }

        /// <summary>
        /// Console子視窗
        /// </summary>
        private IIConsole IConsole { get { return Console; } }
        private IIGoalSetting IGoalSetting { get { return GoalSetting; } }
        private IScene IMapCtrl { get { return MapGL != null ? MapGL.Ctrl : null; } }
        private IITesting ITest { get { return Testing; } }
        
        #endregion Declaration - Properties

        #region Functin - Constructors

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public AgvClientUI()
        {
            InitializeComponent();

            /*-- 車子資訊接收 --*/
            mSoxMonitorCmd = new SocketMonitor(mCmdPort, tsk_RecvCmd).Listen();
            /*-- 檔案接收 --*/
            mSoxMonitorFile = new SocketMonitor(mFilePort, tsk_RecvFiles).Listen();
            /*-- 路徑接收 --*/
            mSoxMonitorPath = new SocketMonitor(mRecvPathPort, tsk_RecvPath).Listen();
            
            /*-- 載入AVG物件 --*/
            if (!Database.AGVGM.ContainsID(mAGVID)) {
                Database.AGVGM.Add(mAGVID, FactoryMode.Factory.AGV(0, 0, 0, "AGV"));
            }

            /*-- 開啟全域鍵盤鉤子 --*/
            mKeyboardHook.KeyDownEvent += mKeyboardHook_KeyDownEvent;
            mKeyboardHook.KeyUpEvent += mKeyboardHook_KeyUpEvent;
            mKeyboardHook.Start();

            mHandle = this.Handle;
            
        }

        #endregion Function - Constructors

        #region Function - Events

        #region KeyboardHook

        private void mKeyboardHook_KeyDownEvent(object sneder,KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    //MotionContorl((MotionDirection)e.KeyCode);
                    break;
            }
        }

        private void mKeyboardHook_KeyUpEvent(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Delete:
                    IGoalSetting.RefreshSingle();
                    break;
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    //MotionContorl(MotionDirection.Stop);
                    break;
            }
        }

        #endregion KeyboardHook

        #region Form

        /// <summary>
        /// 表單載入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientUI_Load(object sender, EventArgs e)
        {
            /*-- 載入ICtDockContent物件 --*/
            LoadICtDockContent();

            /*-- 載入CtNotifyIcon物件 --*/
            LoadCtNotifyIcon();

            /*-- 依照使用者權限進行配置 --*/
            UserChanged(UserData);

            /*-- 檢查Bypass狀態 --*/
            CtInvoke.ToolStripItemChecked(miBypassSocket, IsBypassSocket);
            CtInvoke.ToolStripItemChecked(miLoadFile, IsBypassLoadFile);

            /*-- 檢查遠端設備IP --*/
            tslbHostIP.Text = HostIP;
            ITest.SetHostIP(HostIP);

            mConnectFlow = new ConnectFlow(this.Handle, GetIsConnect, CheckServer, ExecutingInfo);
            mSimilarityFlow = new SimilarityFlow(this.Handle, GetSimilarity, CheckSimilarity, ExecutingInfo);
            mServoOnFlow = new ServoOnFlow(this.Handle, GetIsServoOn, CheckServoOn, ExecutingInfo);
        }

        private bool CheckServoOn() {
            ITest_MotorServoOn(true);
            return IsMotorServoOn;
        }

        private bool GetIsServoOn() {
            return mIsMotorServoOn;
        }


        /// <summary>
        /// 表單關閉中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientUI_FormClosing(object sender, FormClosingEventArgs e)
        {
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
        private void HideWindow()
        {
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
        private void MenuDock_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            /*-- 確認是否有對應DockContent物件 --*/
            if (mDockContent.ContainsKey(item))
            {

                if (item.Checked)
                {
                    (mDockContent[item] as CtDockContent).HideWindow();
                }
                else
                {
                    mDockContent[item].ShowWindow();
                }
                //if (item.Checked != item.Checked) {
                //    item.Checked = !item.Checked;
                //}
            }
        }

        /// <summary>
        /// 離開程式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miExit_Click(object sender, EventArgs e)
        {
            Exit();
        }

        /// <summary>
        /// 關於
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miAbout_Click(object sender, EventArgs e)
        {
            using (CtAbout frm = new CtAbout())
            {
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
        private void miLogin_Click(object sender, EventArgs e)
        {
            Stat stt = Stat.SUCCESS;
            if (mUser.Level == AccessLevel.None)
            {
                using (CtLogin frmLogin = new CtLogin())
                {
                    stt = frmLogin.Start(out mUser);
                }
            }
            else
            {
                mUser = new UserData("N/A", "", AccessLevel.None);
            }
            UserChanged(mUser);
        }

        /// <summary>
        /// 使用者管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miUserManager_Click(object sender, EventArgs e)
        {
            using (CtUserManager frmUsrMgr = new CtUserManager(UILanguage.English))
            {
                frmUsrMgr.ShowDialog();
            }
        }

        /// <summary>
        /// Socket Bypass
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miBypassSocket_Click(object sender, EventArgs e) {
            IsBypassSocket = !IsBypassSocket;
            CtInvoke.ToolStripItemChecked(miBypassSocket, IsBypassSocket);
        }

        /// <summary>
        /// Load File Bypass
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miLoadFile_Click(object sender, EventArgs e) {
            IsBypassLoadFile = !IsBypassLoadFile;
            CtInvoke.ToolStripItemChecked(miLoadFile, IsBypassLoadFile);
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
        private void Value_DockStateChanged(object sender, EventArgs e)
        {

            /*-- 取得發報的DockContent物件 --*/
            CtDockContent dockWnd = sender as CtDockContent;

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
        private void mNotifyIcon_OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowWindow();
        }

        /// <summary>
        /// ShowWindow選項被點擊
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowWindow_OnClick(object sender, EventArgs e)
        {
            ShowWindow();
        }

        /// <summary>
        /// 顯示氣球提示
        /// </summary>
        /// <param name="title">提示標題</param>
        /// <param name="context">提示內容</param>
        /// <param name="icon">提示Icon</param>
        /// <param name="tmo">顯示時間</param>
        public void SetBalloonTip(string title, string context, ToolTipIcon icon = ToolTipIcon.Info, int tmo=5)
        {
            mNotifyIcon.ShowBalloonTip(title, context, tmo, icon);
        }

        #endregion NotifyIcon

        #region ITest

        private void ITest_StartScan(bool scan) {
            string description = (scan ? "Start" : "Stop") + " scan";
            mConnectFlow.CheckFlag(description, () => {
                CarMode mode = scan ? CarMode.Map : CarMode.Idle;
                if (mCarMode != mode) {
                    if (scan) {
                        string oriName = string.Empty;
                        if (Stat.SUCCESS == CtInput.Text(out oriName, "MAP Name", "Set Map File Name")) {
                            SendMsg($"Set:OriName:{oriName}.Ori");
                            ITest.ChangedScanStt(true);
                        } else {
                            return;
                        }
                    }
                    IConsole.AddMsg(description);
                    mCarMode = mode;
                    ChangedMode(mode);
                }
            }); 
        }

        private void ITest_CarPosConfirm() {
            CheckSimilarity();
        }
        
        private void ITest_SettingCarPos() {
            mConnectFlow.CheckFlag("Set Car",() => {
                mIsSetting = true;
            });
        }

        private void ITest_ClearMap() {
            IGoalSetting.ClearGoal();
            Database.ClearAllButAGV();
        }

        private void ITest_MotorServoOn(bool servoOn) {
            mConnectFlow.CheckFlag($"Servo {(servoOn ? "On" : "Off")}",() => {
                string[] rtnMsg = SendMsg($"Set:Servo{(servoOn ? "On" : "Off")}");
                if (rtnMsg.Count() > 2 && bool.Parse(rtnMsg[2])) {
                    IsMotorServoOn = servoOn;
                    IConsole.AddMsg($"Motor Servo {(servoOn ? "ON" : "OFF")}");
                }
            });
        }

        private void ITest_SimplifyOri() {
            if (mBypassLoadFile) {
                /*-- 空跑模擬SimplifyOri --*/
                SpinWait.SpinUntil(() => false, 1000);
                return;
            }
            string[] tmpPath = CurOriPath.Split('.');
            CurMapPath = tmpPath[0] + ".map";
            Database.Save(CurMapPath);

            NewMap();

            //MapSimplication mapSimp = new MapSimplication(CurMapPath);
            //mapSimp.Reset();
            //List<ILine> obstacleLines = new List<ILine>();
            //List<IPair> obstaclePoints = new List<IPair>();
            //List<CartesianPos> resultPoints;
            //List<MapSimplication.Line> resultlines;
            //mapSimp.ReadMapAllTransferToLine(mMapMatch.parseMap, mMapMatch.minimumPos, mMapMatch.maximumPos
            //    , 100, 0, out resultlines, out resultPoints);
            //try {
            //    for (int i = 0; i < resultlines.Count; i++) {
            //        obstacleLines.Add(
            //             FactoryMode.Factory.Line(resultlines[i].startX, resultlines[i].startY,
            //            resultlines[i].endX, resultlines[i].endY)
            //        );
            //    }
            //    for (int i = 0; i < resultPoints.Count; i++) {
            //        obstaclePoints.Add(FactoryMode.Factory.Pair((int)resultPoints[i].x, (int)resultPoints[i].y));
            //    }

            //    Database.ObstaclePointsGM.DataList.AddRange(obstaclePoints);
            //    Database.ObstacleLinesGM.DataList.AddRange(obstacleLines);
            //} catch (Exception ex) {
            //    System.Console.WriteLine(ex.Message);
            //}


            //obstacleLines = null;
            //obstaclePoints = null;
            //resultPoints = null;
            //resultlines = null;
        }

        private async void ITest_CheckIsServerAlive(bool cnn, string hostIP = "") {
            if (cnn != IsConnected) {
                CtProgress prog = new CtProgress("Connect", "Connecting...");
                try {
                    if (cnn) {
                        if (VerifyIP(hostIP)) {
                            if (mHostIP != hostIP) mHostIP = hostIP;
                            await Task.Run(() => IsConnected = CheckIsServerAlive());
                        }
                    } else {
                        IsConnected = DisConnectServer();
                    }
                } finally {
                    prog?.Close();
                    prog = null;
                }
            }
        }

        private void ITest_SetVelocity(int velocity) {
            mConnectFlow.CheckFlag("Set Velocity", () => {
                mVelocity = velocity;
                IConsole.AddMsg($"Set Velocity {velocity}");
                SendMsg($"Set: WorkVelo:{velocity}:{velocity}");
            });
        }
        
        private void ITest_SetCarMode(CarMode mode) {
            //if (mode == CarMode.Map) {
            //    string oriName = string.Empty;
            //    if (Stat.SUCCESS == CtInput.Text(out oriName, "MAP Name", "Set Map File Name")) {
            //        SendMsg($"Set:OriName:{oriName}.Ori");
            //        ITest.ChangedScanStt(true);
            //    } else {
            //        return;
            //    }
            //}
            //if (mode != CarMode.OffLine) {
            //    SendMsg($"Set:Mode:{mode}");
            //    IConsole.AddMsg($"Set:Mode {mode}");
            //}
            IConsole.AddMsg($"Set:Mode {mode}");
            if (mode != mCarMode) {
                mode = mCarMode;
                ChangedMode(mCarMode);
                switch (mode) {
                    case CarMode.OffLine:
                        break;
                    case CarMode.Map:
                        string oriName = string.Empty;
                        if (Stat.SUCCESS == CtInput.Text(out oriName, "MAP Name", "Set Map File Name")) {
                            SendMsg($"Set:OriName:{oriName}.Ori");
                            ITest.ChangedScanStt(true);
                        } else {
                            return;
                        }
                        break;
                    default:
                        SendMsg($"Set:Mode:{mode}");
                        break;
                }
            }
        }

        private void ITest_SendMap() {
            OpenFileDialog openMap = new OpenFileDialog();
            openMap.InitialDirectory = mDefMapDir;
            openMap.Filter = "MAP|*.ori;*.map";
            if (openMap.ShowDialog() == DialogResult.OK) {
                Task.Run(() => mConnectFlow.CheckFlag("Send Map", () => {
                    string[] rtnMsg = SendMsg("Set:Map");
                    bool ack = false;
                    if (rtnMsg.Count() <2 || !bool.TryParse(rtnMsg[2],out ack)) {
                        CtMsgBox.Show("Error", "Server responded incorrectly",MsgBoxBtn.OK,MsgBoxStyle.Error);
                    }else if (!ack) {
                        CtMsgBox.Show("Error", "Server refuses the request", MsgBoxBtn.OK, MsgBoxStyle.Error);
                    }else {
                        SendFile(openMap.FileName);
                    }
                }));
            }

            //mConnectFlow.CheckFlag("Send Map", () => {
            //    SendFile();
            //});
        }

        private void ITest_GetCar() {
            mConnectFlow.CheckFlag("GetCar", () => {
                ChangeSendInfo();
                IConsole.AddMsg($"{IsGettingLaser}");
            });
        }

        private void ITest_GetLaser() {
            mConnectFlow.CheckFlag("GetLaser", () => {
                IConsole.AddMsg("Get Laser");
                GetLaser();
            });
        }

        private void ITest_GetMap() {
            IConsole.AddMsg("[Get Map]");
            GetFile(FileType.Map);
        }

        private void ITest_GetORi() {
            IConsole.AddMsg("[Get Ori]");
            GetFile(FileType.Ori);
        }

        private void ITest_LoadMap() {
            IConsole.AddMsg("[Loaded Map]");
            LoadFile(FileType.Map);
        }

        private void ITest_LoadOri() {
            IConsole.AddMsg("[Loaded Ori]");
            LoadFile(FileType.Ori);
        }

        private void ITest_Motion_Up() {
            mConnectFlow.CheckFlag("Motion Controller",() => {
                IConsole.AddMsg($"[Stop]");
                MotionContorl(MotionDirection.Stop);
                if (CarMode != CarMode.Map) CarMode = CarMode.Idle;

            },false);
        }

        private void ITest_Motion_Down(MotionDirection direction) {
            mServoOnFlow.CheckFlag($"{direction}",()=> {
                IConsole.AddMsg($"[{direction}]");
                MotionContorl(direction);
                if (CarMode != CarMode.Map) CarMode = CarMode.Work;                
            },false);
        }

        #endregion

        #region IMapGL事件連結

        private void IMapCtrl_GLClickEvent(object sender, GLMouseEventArgs e) {
            if (mIsSetting) {
                if (Database.AGVGM.ContainsID(mAGVID)) {
                    if (mNewPos == null) {
                        mNewPos = e.Position;
                    } else {
                        double Calx = e.Position.X - mNewPos.X;
                        double Caly = e.Position.Y - mNewPos.Y;
                        double Calt = Math.Atan2(Caly, Calx) * 180 / Math.PI;
                        Database.AGVGM[mAGVID].Data.Position = mNewPos;
                        Database.AGVGM[mAGVID].Data.Toward.Theta = Calt;
                        //Send POS to AGV   
                        SetPosition(mNewPos.X, mNewPos.Y, Calt);
                        mNewPos = null;
                        mIsSetting = false;
                    }
                }
            } else {
                IGoalSetting.SetCurrentRealPos(new CartesianPos(e.Position.X, e.Position.Y));
            }

        }

        private void IMapCtrl_DragTowerPairEvent(object sender, TowerPairEventArgs e) {
            IGoalSetting.RefreshSingle();
        }

        #endregion IMapGL 事件連結

        #region IGoalSetting 事件連結   

        private void IGoalSetting_Charging(CartesianPosInfo power, int idxPower) {
            if (power != null && idxPower >= 0) {
                mSimilarityFlow.CheckFlag("Charging", () => {
                    IConsole.AddMsg($"Charging to idx{idxPower} {power.ToString()}");
                    Charging(idxPower);
                });
            }else {
                CtMsgBox.Show(mHandle, "No target", "尚未選擇目標Power點",MsgBoxBtn.OK,MsgBoxStyle.Information);
            }
        }

        private void IGoalSetting_SaveGoalEvent() {
            if (string.IsNullOrEmpty(CurMapPath)) {
                if (MsgBoxBtn.Yes == CtMsgBox.Show("Map not loaded yet", "Whether to load the map?", MsgBoxBtn.YesNo, MsgBoxStyle.Question)) {
                    ITest_LoadMap();
                }
                return;
            }
            IConsole.AddMsg("[Save {0} Goals]", IGoalSetting.GoalCount);
            Database.Save(CurMapPath);
        }

        private void IGoalSetting_RunLoopEvent(IEnumerable<CartesianPosInfo> goal) {
            int goalCount = goal?.Count() ?? -1;
            if (goalCount > 0) {
                mSimilarityFlow.CheckFlag("Run all", () => {
                    IConsole.AddMsg("[AGV Start Moving...]");
                    foreach (var item in goal) {
                        IConsole.AddMsg("[AGV Move To] - {0}", item.ToString());
                        IConsole.AddMsg("[AGV Arrived] - {0}", item.ToString());
                    }
                    IConsole.AddMsg("[AGV Move Finished]");
                });
            }else {
                CtMsgBox.Show(mHandle,"No target","尚未選取Goal點，無法進行Run all",MsgBoxBtn.OK,MsgBoxStyle.Information);
            }
        }

        private void IGoalSetting_RunGoalEvent(CartesianPosInfo goal, int idxGoal) {
            CheckGoal(goal, idxGoal, () => {
                mSimilarityFlow.CheckFlag("Run goal", () => {
                    IConsole.AddMsg("[AGV Start Moving...  idx{0} {1}]", idxGoal, goal.ToString());
                    Run(idxGoal);
                    IConsole.AddMsg("[AGV Arrived] - {0}", goal.ToString());
                });
            });
        }

        private void IGoalSetting_LoadMapEvent() {
            IConsole.AddMsg("[Map Loading...]");
            IConsole.AddMsg("[Map Loaded]");
            LoadFile(FileType.Map);
        }

        private void IGoalSetting_FindPathEvent(CartesianPosInfo goal, int idxGoal) {
            CheckGoal(goal, idxGoal, () => {
                mSimilarityFlow.CheckFlag("Path plann", () => {
                    IConsole.AddMsg("[Find Path] - idx{0} {1}", idxGoal, goal.ToString());
                    IConsole.AddMsg("[AGV Find A Path]");
                    PathPlan(idxGoal);
                });
            });
        }

        private void IGoalSetting_DeleteGoalsEvent(IEnumerable<CartesianPosInfo> goal) {
            if ((goal?.Count() ?? 0) > 0) {
                IEnumerable<CartesianPosInfo> goals = goal.Where(v => Database.GoalGM.ContainsID(v.id));
                IEnumerable<CartesianPosInfo> powers = goal.Where(v => Database.PowerGM.ContainsID(v.id));
                List<uint> ID = new List<uint>();
                if (goals?.Any() ?? false) {
                    IConsole.AddMsg($"Delete {goals.Count()}");
                    foreach (var item in goals) {
                        Database.GoalGM.Remove(item.id);
                        ID.Add(item.id);
                        IConsole.AddMsg($"[Delete Goal] - {item.ToString()}");
                    }
                }
                if (powers?.Any() ?? false) {
                    foreach (var item in powers) {
                        Database.PowerGM.Remove(item.id);
                        ID.Add(item.id);
                        IConsole.AddMsg($"[Delete Power - {item.ToString()}]");
                    }
                }
                IGoalSetting.DeleteGoals(ID);
            }
        }

        private void IGoalSetting_ClearGoalsEvent() {
            IConsole.AddMsg("[Clear Goal]");
            Database.GoalGM.Clear();

            IConsole.AddMsg("[Clear Power]");
            Database.PowerGM.Clear();

            IGoalSetting.ClearGoal();
        }

        private void IGoalSetting_AddNewGoalEvent() {
            IMapCtrl.SetAddMode(FactoryMode.Factory.Goal($"Goal{Database.GoalGM.Count}"));
        }

        /// <summary>
        /// 取得所有Goal點名稱
        /// </summary>
        private void IGoalSetting_GetGoalNames() {
            string goalNames = GetGoalNames();
            IConsole.AddMsg($"Goal Names:{goalNames}");
        }

        #endregion

        #endregion Function - Events

        #region Functoin - Public Methods

        #endregion Function - Public Methdos

        #region Function - Private Methods

        #region  Task

        /// <summary>
        /// 檔案接收執行緒
        /// </summary>
        /// <param name="obj"></param>
        private void tsk_RecvFiles(object obj) {
            int fileNameLen = 0;
            int recieve_data_size = 0;
            int first = 1;
            int receivedBytesLen = 0;
            double cal_size = 0;
            SocketMonitor soxMonitor = obj as SocketMonitor;
            Socket clientSock = null;
            BinaryWriter bWrite = null;
            //MemoryStream ms = null;
            string curMsg = "Stopped";
            string fileName = "";
            try {
                if (!mBypassSocket) {
                    clientSock = serverComm.ClientAccept(soxMonitor.Socket);
                    curMsg = "Running and waiting to receive file.";

                    //Socket clientSock = sRecvFile.Accept();
                    //clientSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
                    //clientSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);//設置發送資料超時
                    //clientSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 1024); //設置發送緩衝區大小 1K
                    /* When request comes from client that accept it and return 
                    new socket object for handle that client. */
                    byte[] clientData = new byte[1024 * 10000];
                    do {
                        receivedBytesLen = clientSock.Receive(clientData);
                        curMsg = "Receiving data...";
                        if (first == 1) {
                            fileNameLen = BitConverter.ToInt32(clientData, 0);
                            /* I've sent byte array data from client in that format like 
                            [file name length in byte][file name] [file data], so need to know 
                            first how long the file name is. */
                            fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);
                            /* Read file name */
                            if (!Directory.Exists(mDefMapDir)) {
                                Directory.CreateDirectory(mDefMapDir);
                            }
                            if (File.Exists(mDefMapDir + "/" + fileName)) {
                                File.Delete(mDefMapDir + "/" + fileName);
                            }
                            bWrite = new BinaryWriter(File.Open(mDefMapDir + "/" + fileName, FileMode.OpenOrCreate));
                            /* Make a Binary stream writer to saving the receiving data from client. */
                            // ms = new MemoryStream();
                            bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);
                            //ms.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 -
                            //fileNameLen);
                            //寫入資料 ，呈現於BITMAP用  
                            /* Read remain data (which is file content) and 
                            save it by using binary writer. */
                            curMsg = "Saving file...";
                            /* Close binary writer and client socket */
                            curMsg = "Received & Saved file; Server Stopped.";
                        } else //第二筆接收為資料  
                          {
                            //-----------  
                            fileName = Encoding.ASCII.GetString(clientData, 0,
                            receivedBytesLen);
                            //-----------  
                            bWrite.Write(clientData/*, 4 + fileNameLen, receivedBytesLen - 4 -
                                  fileNameLen*/, 0, receivedBytesLen);
                            //每筆接收起始 0 結束為當次Receive長度  
                            //ms.Write(clientData, 0, receivedBytesLen);
                            //寫入資料 ，呈現於BITMAP用  
                        }
                        recieve_data_size += receivedBytesLen;
                        //計算資料每筆資料長度並累加，後面可以輸出總值看是否有完整接收  
                        cal_size = recieve_data_size;
                        cal_size /= 1024;
                        cal_size = Math.Round(cal_size, 2);

                        first++;
                        SpinWait.SpinUntil(() => false, 10); //每次接收不能太快，否則會資料遺失  

                    } while (clientSock.Available != 0);
                    clientData = null;
                } else {
                    SpinWait.SpinUntil(() => false, 1000);
                    fileName = "FileName";
                }

                SetBalloonTip($"Get File", $"The { fileName} is downloaded");
            } catch (SocketException se) {
                System.Console.WriteLine("SocketException : {0}", se.ToString());
                MessageBox.Show("檔案傳輸失敗!");
                curMsg = "File Receiving error.";
            } catch (Exception ex) {
                System.Console.WriteLine("[RecvFiles]" + ex.ToString());
                curMsg = "File Receiving error.";
            } finally {
                bWrite?.Close();
                clientSock?.Shutdown(SocketShutdown.Both);
                clientSock?.Close();
                clientSock = null;
                //RaiseTestingEvent(TestingEventType.GetFile);
            }
        }

        /// <summary>
        /// 車子資訊接收執行緒
        /// </summary>
        public void tsk_RecvCmd(object obj) {

            System.Console.WriteLine("Start");
            SocketMonitor soxMonitor = obj as SocketMonitor;
            Socket sRecvCmdTemp = null;
            sRecvCmdTemp = serverComm.ClientAccept(soxMonitor.Socket);
            string strRecvCmd;
            System.Console.WriteLine("IsAccept");
            try {
                while (IsGettingLaser) {
                    //SpinWait.SpinUntil(() => false, 1);//每個執行緒內部的閉環裡面都要加個「短時間」睡眠，使得執行緒佔用資源得到及時釋放
                    //Thread.Sleep(1);
                    byte[] recvBytes = new byte[1024 * 500];//開啟一個緩衝區，存儲接收到的資訊
                    sRecvCmdTemp.Receive(recvBytes); //將讀得的內容放在recvBytes中
                    strRecvCmd = Encoding.Default.GetString(recvBytes);//
                                                                       //程式運行到這個地方，已經能接收到遠端發過來的命令了
                    strRecvCmd = strRecvCmd.Split(new char[] { '\0' })[0];
                    //Console.WriteLine("[Server] : " + strRecvCmd);

                    //*************
                    //解碼命令，並執行相應的操作----如下面的發送本機圖片
                    //*************

                    string[] strArray = strRecvCmd.Split(':');
                    recvBytes = null;

                    System.Console.WriteLine("Decoder");
                    if (CarInfo.TryParse(strRecvCmd, ref mCarInfo)) {

                        System.Console.WriteLine("Display");
                        this.InvokeIfNecessary(() => {
                            tsprgBattery.Value = mCarInfo.PowerPercent;
                            tslbBattery.Text = string.Format(tslbBattery.Tag.ToString(), mCarInfo.PowerPercent);
                            tslbStatus.Text = mCarInfo.Status;
                        });
                        System.Console.WriteLine("Draw");
                        Database.AGVGM[mAGVID].SetLocation(mCarInfo);
                        DrawLaser(mCarInfo);
                        sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Get:Car:True:True"));
                    } else {
                        sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Get:Car:False"));
                    }

                    strRecvCmd = null;
                    strArray = null;
                }

                Thread.Sleep(1);
            } catch (SocketException se) {
                System.Console.WriteLine("[Status Recv] : " + se.ToString());
                MessageBox.Show("目標拒絕連線");
            } catch (Exception ex) {
                System.Console.Write(ex.Message);
                //throw ex;
            } finally {
                sRecvCmdTemp?.Close();
                sRecvCmdTemp = null;
            }
        }

        /// <summary>
        /// 路徑接收執行緒
        /// </summary>
        protected void tsk_RecvPath(object obj) {
            SocketMonitor soxMonitor = obj as SocketMonitor;
            Socket sRecvCmdTemp = soxMonitor.Accept();
            
            try {
                sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Path:Require"));
                byte[] recvBytes = new byte[1024 * 500];//開啟一個緩衝區，存儲接收到的資訊
                sRecvCmdTemp.Receive(recvBytes); //將讀得的內容放在recvBytes中
                string strRecvCmd = Encoding.Default.GetString(recvBytes);
                //程式運行到這個地方，已經能接收到遠端發過來的命令了
                //Console.WriteLine("[Server] : " + strRecvCmd);
                //*************
                //解碼命令，並執行相應的操作----如下面的發送本機圖片
                //*************
                strRecvCmd = strRecvCmd.Split(new char[] { '\0' })[0];

                string[] strArray = strRecvCmd.Split(':');
                recvBytes = null;
                if (strArray[0] == "Path" && !string.IsNullOrEmpty(strArray[1])) {
                    DrawPath(PathEncoder(strArray[1]));
                }
                //else
                //    sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Path:False"));

                strRecvCmd = null;
                strArray = null;
                sRecvCmdTemp.Close();

            } catch (SocketException se) {
                System.Console.WriteLine("[Status Recv] : " + se.ToString());
                //MessageBox.Show("目標拒絕連線");
            } catch (Exception ex) {
                System.Console.Write(ex.Message);
                //throw ex;
            } finally {
                sRecvCmdTemp.Close();
                sRecvCmdTemp = null;
            }
        }

        #endregion Task

        #region Communication
        
        /// <summary>
        /// 檢查Server是否在運作中
        /// </summary>
        /// <returns></returns>
        private bool CheckIsServerAlive() {
            if (mBypassSocket) {
                IsServerAlive = true;
                Thread.Sleep(1000);
            } else {
                bool isAlive = false;
                try {
                    if (isAlive = ConnectServer()) {
                        string[] rtnMsg = SendMsg("Get:Hello", false);
                        isAlive = rtnMsg.Count() > 2 && rtnMsg[2] == "True";
                    }
                } catch (Exception ex) {
                    System.Console.WriteLine($"[SocketException] : {ex.Message}");
                } finally {
                    if (!mBypassSocket && !isAlive) {
                        CtMsgBox.Show(mHandle,"Failed", "Connect Failed!!", MsgBoxBtn.OK, MsgBoxStyle.Error);
                    }
                }
                IsServerAlive = isAlive;
            }
            return IsServerAlive;
        }

        private void SetPosition(int x, int y, double theta) {
            Database.AGVGM[mAGVID].Data.Position.X = x;
            Database.AGVGM[mAGVID].Data.Position.Y = y;
            Database.AGVGM[mAGVID].Data.Toward.Theta = theta;
            mCarInfo.x = x;
            mCarInfo.y = y;
            mCarInfo.theta = theta;

            SendMsg($"Set:POS:{x:F0}:{y:F0}:{theta}");
        }

        /// <summary>
        /// 向AGV要求所有Goal點名稱
        /// </summary>
        /// <returns>Goal List:"{goal1},{goal2},{goal3}..."</returns>
        /// <remarks>用來模擬iS向AGV要求所有Goal點名稱</remarks> 
        private string GetGoalNames() {
            string goalList = "Empty";
            if (mBypassSocket) {
                /*-- 模擬用資料 --*/
                goalList = "GoalA,GoalB,GoalC";
            } else {
                string[] rtnMsg = SendMsg($"Get:GoalList");
                if (rtnMsg.Count() > 3) {
                    goalList = rtnMsg[3];
                }
            }
            return goalList;
        }

        /// <summary>
        /// 連接至Server端(AGV)
        /// </summary>
        /// <returns><see cref="Socket"/>連線狀態 True:已連線/False:已斷開</returns>
        private bool ConnectServer() {
            return serverComm.ConnectServer(ref mSoxCmd, mHostIP, mRecvCmdPort);
        }

        /// <summary>
        /// 與Server端(AGV)斷開連線
        /// </summary>
        /// <returns><see cref="Socket"/>連線狀態 True:已連線/False:已斷開</returns>
        public bool DisConnectServer() {
            if (mBypassSocket) {
                return false;
            } else {
                return serverComm.DisconnectServer(mSoxCmd);
            }
        }

        /// <summary>
        /// Send file of server to client
        /// </summary>
        /// <param name="clientIP">Ip address of client</param>
        /// <param name="clientPort">Communication port</param>
        /// <param name="fileName">File name</param>
        /// 
        private bool SendFile(string clientIP, int clientPort, string fileName) {
            string curMsg = "";
            bool isSent = true;
            try {
                IPAddress[] ipAddress = Dns.GetHostAddresses(clientIP);
                IPEndPoint ipEnd = new IPEndPoint(ipAddress[0], clientPort);
                /* Make IP end point same as Server. */
                Socket clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                /* Make a client socket to send data to server. */
                string filePath = "D:\\MapInfo\\";
                /* File reading operation. */
                fileName = fileName.Replace("\\", "/");
                while (fileName.IndexOf("/") > -1) {
                    filePath += fileName.Substring(0, fileName.IndexOf("/") + 1);
                    fileName = fileName.Substring(fileName.IndexOf("/") + 1);
                }
                byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
                if (fileNameByte.Length > 1024 * 1024 * 5) {
                    curMsg = "File size is more than 850kb, please try with small file.";
                    return false;
                }
                curMsg = "Buffering ...";
                byte[] fileData = File.ReadAllBytes(filePath + fileName);
                /* Read & store file byte data in byte array. */
                byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
                /* clientData will store complete bytes which will store file name length, 
                file name & file data. */
                byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);
                /* File name length’s binary data. */
                fileNameLen.CopyTo(clientData, 0);
                fileNameByte.CopyTo(clientData, 4);
                fileData.CopyTo(clientData, 4 + fileNameByte.Length);
                /* copy these bytes to a variable with format line [file name length]
                [file name] [ file content] */
                curMsg = "Connection to server ...";
                clientSock.Connect(ipEnd);
                /* Trying to connection with server. */
                curMsg = "File sending...";
                clientSock.Send(clientData);
                /* Now connection established, send client data to server. */
                curMsg = "Disconnecting...";
                clientSock.Close();
                fileNameByte = null;
                clientData = null;
                fileNameLen = null;
                /* Data send complete now close socket. */
                curMsg = "File transferred.";
            } catch (Exception ex) {
                if (ex.Message == "No connection could be made because the target machine actively refused it")
                    curMsg = "File Sending fail. Because server not running.";
                else
                    curMsg = "File Sending fail." + ex.Message;
                isSent = false;
            }
            return isSent;
        }

        /// <summary>
        /// 傳送檔案
        /// </summary>
        private async void SendFile() {
            string[] rtnMsg = SendMsg("Send:map");
            if (mBypassSocket || rtnMsg.Count() > 2 && "True" == rtnMsg[2]) {
                OpenFileDialog openMap = new OpenFileDialog();
                openMap.InitialDirectory = mDefMapDir;
                openMap.Filter = "MAP|*.ori;*.map";
                if (openMap.ShowDialog() == DialogResult.OK) {
                    CtProgress prog = new CtProgress("Send Map", "The file are being transferred");
                    try {
                        await Task.Run(() => {
                            SendFile(openMap.FileName);
                            SetBalloonTip("Send File", openMap.FileName);
                        }); 
                    } finally {
                        prog?.Close();
                        prog = null;
                    }
                }
            }
        }

        /// <summary>
        /// 傳送檔案
        /// </summary>
        /// <param name="filePath"></param>
        private  void SendFile(string filePath) {
            string fileName = CtFile.GetFileName(filePath);
            bool isSent = true;
            if (!mBypassSocket) {
                isSent = SendFile(mHostIP, mSendMapPort, fileName);
            } else {
                /*-- 空跑模擬檔案傳送中 --*/
                SpinWait.SpinUntil(() => false, 1000);
            }
            if (isSent) {
                IConsole.AddMsg($"{fileName} has been sent");
                SetBalloonTip("Send file", $"{fileName} han been sent");
            } else {
                IConsole.AddMsg($"\'{fileName}\' send failed");
                CtMsgBox.Show("Send file", $"\'{fileName}\' send failed", MsgBoxBtn.OK, MsgBoxStyle.Error);
            }
        }

        /// <summary>
        /// 變更車子資料發送狀態
        /// </summary>
        /// <param name="on">true:開啟/false:關閉資訊回傳</param>
        /// <remarks>
        /// modify by Jay 2017/09/08
        /// </remarks>
        /// <returns>True:發送中/False:停止發送</returns>
        public bool ChangeSendInfo() {
            try {
                IsGettingLaser = !IsGettingLaser;
                if (IsGettingLaser) {
                    /*-- 開啟車子資訊讀取執行緒 --*/
                    if (!mBypassSocket) mSoxMonitorCmd.Start();

                    /*-- 向Server端要求車子資料 --*/
                    string[] rtnMsg = SendMsg("Get:Car:True:True");
                    IsGettingLaser = mBypassSocket || (rtnMsg.Count() > 2 && "True" == rtnMsg[2]);

                    /*-- 車子未發送資料則關閉Socket --*/
                    if (!IsGettingLaser) {
                        Database.AGVGM[mAGVID].LaserAPoints.DataList.Clear();
                        if (!mBypassSocket)mSoxMonitorCmd.Stop();
                    }
                } else {
                    Database.AGVGM[mAGVID].LaserAPoints.DataList.Clear();
                    if (!mBypassSocket) mSoxMonitorCmd.Stop();
                    SendMsg("Get:Car:False");
                }
                ITest.SetLaserStt(IsGettingLaser);
            } catch (Exception ex) {
                System.Console.WriteLine(ex.Message);
            }

            return IsGettingLaser;
        }

        private void GetLaser() {
            /*-- 若是雷射資料則更新資料 --*/
            string[] rtnMsg = SendMsg("Get:Laser");
            if (rtnMsg.Length > 3) {
                if (rtnMsg[1] == "Laser") {
                    string[] sreRemoteLaser = rtnMsg[3].Split(',');
                    mCarInfo.LaserData = sreRemoteLaser.Select(x => int.Parse(x));
                    DrawLaser(mCarInfo);
                }
            }
        }

        /// <summary>
        /// 移動控制
        /// </summary>
        /// <param name="direction">移動方向</param>
        /// <param name="velocity">移動速度</param>
        private void MotionContorl(MotionDirection direction) {
            string[] rtnMsg = SendMsg("Get:IsOpen");
            if (rtnMsg.Count() > 2 && bool.Parse(rtnMsg[2])) {

                if (direction == MotionDirection.Stop) {
                    SendMsg("Set:Stop");
                } else {

                    string cmd = string.Empty;
                    cmd = GetMotionCmd(direction);
                
                    SendMsg(cmd);
                    SendMsg("Set:Start");
                }
            }
        }

        /// <summary>
        /// 向Server端要求檔案
        /// </summary>
        /// <param name="type">檔案類型</param>
        /// <remarks>modified by Jay 2017/09/20</remarks>
        private bool GetFileList(FileType type, out string fileList) {
            bool ret = true;
            fileList = string.Empty;
            if (mBypassSocket) {
                fileList = $"{type}1,{type}2,{type}3";
            } else {
                string[] rtnMsg = SendMsg($"Get:{type}List");
                fileList = rtnMsg[3];
            }
            return ret;
        }

        private void GetFile(FileType type) {
            mConnectFlow.CheckFlag($"Get{type}", () => {
                string fileList = string.Empty;
                if (GetFileList(type, out fileList)) {
                    using (MapList f = new MapList(fileList)) {
                        if (f.ShowDialog() == DialogResult.OK) {
                            FileDownload(f.strMapList, type);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 檔案下載
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        public void FileDownload(string fileName, FileType type) {
            /*-- 開啟執行緒準備接收檔案 --*/
            mSoxMonitorFile.Start();

            /*-- 向Server端發出檔案請求 --*/
            SendMsg($"Get:{type}:{fileName}");
            //if (type == FileType.Map) {
            //    RaiseGoalSettingEvent(GoalSettingEventType.CurMapPath, true);
            //} else {
            //    RaiseTestingEvent(TestingEventType.CurOriPath, true);
            //}
            //RaiseAgvClientEvent(AgvClientEventType.GetFile, type);

        }

        /// <summary>
        /// 前往目標Goal點
        /// </summary>
        /// <param name="numGoal">目標Goal點</param>
        private void Run(int numGoal) {
            /*-- 若是路徑資料則開始接收資料 --*/
            string[] rtnMsg = SendMsg($"Set:Run:{numGoal}");
            if ((rtnMsg?.Length ?? 0) > 3 &&
                rtnMsg[1] == "Run" &&
                rtnMsg[3] == "Done") {
                mSoxMonitorPath.Start();
            }
        }

        /// <summary>
        /// 路徑規劃
        /// </summary>
        /// <param name="no">目標Goal點編號</param>
        private void PathPlan(int numGoal) {
            /*-- 若是路徑資料則開始接收資料 --*/
            string[] rtnMsg = SendMsg($"Set:PathPlan:{numGoal}");
            if ((rtnMsg?.Count() ?? 0) > 3 &&
                rtnMsg[1] == "PathPlan" &&
                rtnMsg[2] == "True") {
                mSoxMonitorPath.Start();
            }
        }

        private void Charging(int numGoal) {
            /*-- 若是路徑資料則開始接收資料 --*/
            string[] rtnMsg = SendMsg($"Set:Charging:{numGoal}");
            if ((rtnMsg?.Count() ?? 0) > 3 &&
                rtnMsg[1] == "PathPlan" &&
                rtnMsg[2] == "True") {
                mSoxMonitorPath.Start();
            }
        }

        /// <summary>
        /// 訊息傳送(會觸發事件)
        /// </summary>
        /// <param name="sendMseeage">傳送訊息內容</param>
        /// <param name="passChkConn">是否略過檢查連線狀態</param>
        /// <returns>Server端回應</returns>
        private string[] SendMsg(string sendMseeage, bool passChkConn = true) {
            /*-- 顯示發送出去的訊息 --*/
            string msg = $"{DateTime.Now} [Client] : {sendMseeage}";
            IConsole.AddMsg(msg);

            if (mBypassSocket) {
                /*-- Bypass略過不傳 --*/
                return new string[] { "","","True" };
            } else if (passChkConn && !IsServerAlive) {
                /*-- 略過連線檢查且Server端未運作 --*/
                return new string[] { "","","False" };
            }
            
            /*-- 等待Server端的回應 --*/
            string rtnMsg = SendStrMsg(sendMseeage);
            //string rtnMsg = SendStrMsg(mHostIP, mRecvCmdPort, sendMseeage );
            rtnMsg = rtnMsg.Trim();
            /*-- 顯示Server端回應 --*/
            msg = $"{DateTime.Now} [Server] : {rtnMsg}\r\n";
            IConsole.AddMsg(msg);

            return rtnMsg.Split(':');
        }

        /// <summary>
        /// 訊息傳送(具體Socket交握實現，但是不會觸發事件)
        /// </summary>
        /// <param name="serverIP">伺服端IP</param>
        /// <param name="requerPort">通訊埠號</param>
        /// <param name="sendMseeage">傳送訊息內容</param>
        /// <returns>Server端回應</returns>
        private string SendStrMsg(string sendMseeage) {
            //可以在字串編碼上做文章，可以傳送各種資訊內容，目前主要有三種編碼方式：
            //1.自訂連接字串編碼－－微量
            //2.JSON編碼--輕量
            //3.XML編碼--重量
            int state;
            byte[] recvBytes = new byte[8192];//開啟一個緩衝區，存儲接收到的資訊
            try {

                byte[] sendContents = Encoding.UTF8.GetBytes(sendMseeage + "\r\n");
                state = mSoxCmd.Send(sendContents, sendContents.Length, 0);//發送二進位資料
                state = mSoxCmd.Receive(recvBytes);
                string strRecvCmd = Encoding.Default.GetString(recvBytes);//
                strRecvCmd = strRecvCmd.Split(new char[] { '\0' })[0];
                sendContents = null;
                return strRecvCmd;
            } catch (SocketException se) {
                System.Console.WriteLine("SocketException : {0}", se.ToString());
                return "False";
            } catch (ArgumentNullException ane) {
                System.Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                return "False";
            } catch (Exception ex) {
                System.Console.Write(ex.Message);
                return "False";
            } finally {
                recvBytes = null;
            }

        }
        
        private string GetMotionCmd(MotionDirection direction) {
            string cmd = "";
            switch (direction) {
                case MotionDirection.Forward:
                    cmd = $"Set:DriveVelo:{mVelocity}:{mVelocity}";
                    break;
                case MotionDirection.Backward:
                    cmd = $"Set:DriveVelo:-{mVelocity}:-{mVelocity}";
                    break;
                case MotionDirection.LeftTrun:
                    cmd = $"Set:DriveVelo:-{mVelocity}:{mVelocity}";
                    break;
                case MotionDirection.RightTurn:
                    cmd = $"Set:DriveVelo:{mVelocity}:-{mVelocity}";
                    break;
            }
            return cmd;
        }

        #endregion Communication 

        #region UI

        /// <summary>
        /// 車子模式切換時
        /// </summary>
        /// <param name="mode"></param>
        private void ChangedMode(CarMode mode) {
            tslbStatus.Text = $"{mode}";
            ITest.ChangedScanStt(mode == CarMode.Map);
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
            mNotifyIcon.HideIcon();
            this.Dispose();
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
            bool allowUsrMan = usrLv > AccessLevel.Operator;

            /*-- 依照權限切換模組可視層級 --*/
            switch (usrLv) {
                case AccessLevel.None:
                    DockContentVisible(miToolBox, false);
                    DockContentVisible(miMapGL, false);
                    DockContentVisible(miConsole, true);
                    DockContentVisible(miTesting, false);
                    DockContentVisible(miGoalSetting, false);
                    miBypass.Visible = false;
                    break;
                case AccessLevel.Operator:
                    DockContentVisible(miToolBox, false);
                    DockContentVisible(miMapGL, true);
                    DockContentVisible(miConsole, true);
                    DockContentVisible(miGoalSetting, true);
                    break;
                case AccessLevel.Engineer:
                case AccessLevel.Administrator:
                    DockContentVisible(miToolBox, true);
                    DockContentVisible(miMapGL, true);
                    DockContentVisible(miConsole, true);
                    DockContentVisible(miGoalSetting, true);
                    DockContentVisible(miTesting, true);
                    miBypass.Visible = true;

                    break;
            }

            /*-- 顯示帳號相關資訊 --*/
            if (usrLv == AccessLevel.None) {
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
        
        /// <summary>
        /// 檢查Goal是否合法
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="idxGoal"></param>
        /// <param name="act"></param>
        private void CheckGoal(CartesianPosInfo goal, int idxGoal,Action act) {
            if (goal != null && idxGoal >= 0) {
                act?.Invoke();
            } else {
                CtMsgBox.Show(mHandle,"No target", "尚未選擇目標Goal點", MsgBoxBtn.OK, MsgBoxStyle.Information);
            }
        }

        private void ShowMotionController() {
            if (mMotionController == null) {
                mMotionController = new CtMotionController();
                mMotionController.MotionDown += ITest_Motion_Down;
                mMotionController.MotionUp += ITest_Motion_Up;
                miMotionController.Checked = true;
                mMotionController.FormClosing += (fSender, fE) => {
                    mMotionController.MotionDown -= ITest_Motion_Down;
                    mMotionController.MotionUp -= ITest_Motion_Up;
                    mMotionController = null;
                    miMotionController.Checked = false;
                };
                mMotionController.Show();
            }
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
            Size dockSize = new Size();
            dockSize.Width = dockContents.Max(v => v.Value.FixedSize.Width);
            dockSize.Height = dockContents.Max(v => v.Value.FixedSize.Height);

            /*-- 依照停靠區域計算所需顯示大小 --*/
            double portion = 0;
            if (DockMth.CalculatePortion(area, dockSize, out portion)) {
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
        /// 切換CtDockContemt可視狀態
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dockContent">CtDockContent物件</param>
        /// <param name="vis">可視狀態</param>
        private void DockContentVisible(ICtDockContent dockContent, bool vis) {
            try {
                    if (vis) {
                        dockContent.ShowWindow();
                    } else {
                        dockContent.HideWindow();
                    }
                
            } catch (Exception ex) {
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
        private void DockContentEnable(bool mapGL, bool console, bool testing, bool goalSetting) {
            CtInvoke.ToolStripItemEnable(miMapGL, mapGL);
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
        private void DockContentVisible(ToolStripMenuItem item, bool visible) {
            if (mDockContent.ContainsKey(item)) {
                DockContentVisible(mDockContent[item], visible);
                CtInvoke.ToolStripItemEnable(item, visible);
            }
        }

        #endregion DockContent

        #region Draw

        /// <summary>
        /// 載入地圖
        /// </summary>
        /// <param name="mapPath">Map檔路徑</param>
        private bool LoadMap(string mapPath) {
            mCurMapPath = mapPath;
            string path = CtFile.GetFileName(mapPath);
            Stopwatch sw = new Stopwatch();
            if (mBypassLoadFile) {
                /*-- 空跑1秒模擬載入Map檔 --*/
                SpinWait.SpinUntil(() => false, 1000);
            } else {
                //#region - Retrive information from .map file -
                sw.Start();
                /*-- 地圖清空 --*/
                NewMap();

                /*-- 載入Map並取得Map中心點 --*/
                IPair center = Database.LoadMapToDatabase(mCurMapPath)?.Center();

                if (center == null) {
                    return false;
                }

                /*-- 移動畫面至Map中心點 --*/
                IMapCtrl.Focus(center);
                
                IGoalSetting.RefreshSingle();

                if (IsConnected) {
                    SendFile(mapPath);

                    SendMsg($"Set:MapName:{path}");
                }
            }
            return true;
        }

        /// <summary>
        /// 載入Ori檔
        /// </summary>
        /// <param name="oriPath"></param>
        /// <returns></returns>
        private bool LoadOri(string oriPath) {
            bool isLoaded = true;
            CurOriPath = oriPath;
            NewMap();
            MapReading MapReading = null;
            if (!mBypassLoadFile) {//無BypassLoadFile
                MapReading = new MapReading(CurOriPath);
                CartesianPos carPos = null;
                List<CartesianPos> laserData = null;
                //List<Point> listMap = new List<Point>();
                int dataLength = MapReading.OpenFile();
                if (dataLength != 0) {
                    CtProgress prog = new CtProgress(ProgBarStyle.Percent, $"Load Ori", $"Loading {oriPath}...", dataLength - 1);
                    try {
                        IMapCtrl.Zoom = 100;
                        for (int n = 0; n < dataLength; n++) {
                            MapReading.ReadScanningInfo(n, out carPos, out laserData);
                            Database.AGVGM[mAGVID].SetLocation(carPos);
                            List<IPair> points = laserData.ToPairs();
                            Database.AGVGM[mAGVID]?.LaserAPoints.DataList.Replace(points);
                            Database.ObstaclePointsGM.DataList.AddRange(points);
                            IMapCtrl.Focus((int)carPos.x, (int)carPos.y);
                            Thread.Sleep(10);
                            System.Console.WriteLine(n);
                            prog.UpdateStep(n);
                        }
                    } catch (Exception ex) {
                        isLoaded = false;
                        System.Console.WriteLine(ex.Message);
                    } finally {
                        prog?.Close();
                        prog = null;
                    }
                }
            } else {//Bypass LoadFile功能
                    /*-- 空跑一秒，模擬檔案載入 --*/
                SpinWait.SpinUntil(() => false, 1000);
            }
            MapReading = null;
            return isLoaded;
        }

        /// <summary>
        /// 載入檔案
        /// </summary>
        /// <param name="type">載入檔案類型</param>
        public async void LoadFile(FileType type) {
            OpenFileDialog openMap = new OpenFileDialog();
            openMap.InitialDirectory = mDefMapDir;
            openMap.Filter = $"MAP|*.{type.ToString().ToLower()}";
            if (openMap.ShowDialog() == DialogResult.OK) {
                try {
                    bool isLoaded = false;
                    switch (type) {
                        case FileType.Ori:
                            await Task.Run(() => 
                                isLoaded = LoadOri(openMap.FileName));
                                if (isLoaded) {
                                    Database.AGVGM[mAGVID]?.LaserAPoints.DataList.Clear();
                                    ITest.UnLockOriOperator(true);
                                }
                            break;
                        case FileType.Map:
                            await Task.Run(() => {
                                isLoaded = LoadMap(openMap.FileName);
                            });
                            break;
                        default:
                            throw new ArgumentException($"無法載入未定義的檔案類型{type}");
                    }
                    if (isLoaded) {
                        SetBalloonTip($"Load { type}", $"\'{openMap.FileName}\' is loaded");
                    }else {
                        CtMsgBox.Show(mHandle, "Error", "File data is wrong, can not read", MsgBoxBtn.OK, MsgBoxStyle.Error);
                    }
                } catch (Exception ex) {
                    CtMsgBox.Show(mHandle,"Error", ex.Message);
                }
            }
            openMap = null;
            var v = Database.ObstaclePointsGM.DataList;
        }
        
        /// <summary>
        /// 清空Map
        /// </summary>
        private void NewMap() {
            /*-- 保留AGV資料 --*/
            var agv = Database.AGVGM[mAGVID];
            /*-- 清除資料 --*/
            IMapCtrl.NewMap();
            /*-- 將AGV寫回DataBase --*/
            Database.AGVGM.Add(mAGVID, agv);
            /*-- 清除雷射 --*/
            Database.AGVGM[mAGVID].LaserAPoints.DataList.Clear();
            /*-- 清除路徑 --*/
            Database.AGVGM[mAGVID].Path.DataList.Clear();
        }


        /// <summary>
        /// 繪製雷射
        /// </summary>
        private void DrawLaser(CarInfo info) {
            List<IPair> points = new List<IPair>();
            int idx = 0;
            foreach (int dist in info.LaserData) {
                if (dist >= 30 && dist < 15000) {
                    int[] pos = CalcLaserPoint(dist,idx++,info);
                    points.Add(FactoryMode.Factory.Pair(pos[0], pos[1]));
                    pos = null;
                }
            }
            Database.AGVGM[mAGVID]?.LaserAPoints.DataList.Replace(points);
        }

        /// <summary>
        /// 繪製路徑路徑
        /// </summary>
        /// <param name="points"></param>
        private void DrawPath(List<CartesianPos> points) {
            Database.AGVGM[mAGVID].Path.DataList.Replace(points.ToIPair());
        }

        /// <summary>
        /// 計算雷射點座標
        /// </summary>
        /// <param name="dist"></param>
        /// <param name="idx"></param>
        /// <param name="carPos"></param>
        /// <returns></returns>
        private int[] CalcLaserPoint(int dist,int idx,CartesianPos carPos) {
            double angle, Laserangle;
            return Transformation.LaserPoleToCartesian(
                dist,
                LaserParam.AngleBase,
                LaserParam.Resolution,
                idx++,
                LaserParam.AngleOffset,
                LaserParam.OffsetLen,
                LaserParam.OffsetTheta,
                carPos.x,carPos.y, carPos.theta,
                out angle, out Laserangle);
        }

        #endregion Draw

        #region Load

        /// <summary>
        /// 設定事件連結
        /// </summary>
        private void SetEvents() {
            #region IGoalSetting 事件連結     
            IGoalSetting.AddNewGoalEvent += IGoalSetting_AddNewGoalEvent;
            IGoalSetting.ClearGoalsEvent += IGoalSetting_ClearGoalsEvent;
            IGoalSetting.DeleteGoalsEvent += IGoalSetting_DeleteGoalsEvent;
            IGoalSetting.FindPathEvent += IGoalSetting_FindPathEvent;
            IGoalSetting.LoadMapEvent += IGoalSetting_LoadMapEvent;
            IGoalSetting.LoadMapFromAGVEvent += ITest_GetMap;
            IGoalSetting.RunGoalEvent += IGoalSetting_RunGoalEvent;
            IGoalSetting.RunLoopEvent += IGoalSetting_RunLoopEvent;
            IGoalSetting.SaveGoalEvent += IGoalSetting_SaveGoalEvent;
            IGoalSetting.SendMapToAGVEvent += ITest_SendMap;
            IGoalSetting.GetGoalNames += IGoalSetting_GetGoalNames;
            IGoalSetting.Charging += IGoalSetting_Charging;
            IGoalSetting.ClearMap += ITest_ClearMap;

            #endregion

            #region IMapGL 事件連結
            IMapCtrl.GLClickEvent += IMapCtrl_GLClickEvent;
            IMapCtrl.DragTowerPairEvent += IMapCtrl_DragTowerPairEvent;
            IMapCtrl.GLMoveUp += IMapCtrl_GLMoveUp;
            #endregion

            #region ITesting 事件連結
            ITest.Motion_Down += ITest_Motion_Down;
            ITest.Motion_Up += ITest_Motion_Up;
            ITest.LoadMap += IGoalSetting_LoadMapEvent;
            ITest.LoadOri += ITest_LoadOri;
            ITest.GetOri += ITest_GetORi;
            ITest.GetMap += ITest_GetMap;
            ITest.GetLaser += ITest_GetLaser;
            ITest.GetCar += ITest_GetCar;
            ITest.SendMap += ITest_SendMap;
            ITest.SetCarMode += ITest_SetCarMode;
            ITest.SetVelocity += ITest_SetVelocity;
            ITest.Connect += ITest_CheckIsServerAlive;
            ITest.MotorServoOn += ITest_MotorServoOn;
            ITest.SimplifyOri += ITest_SimplifyOri;
            ITest.ClearMap += ITest_ClearMap;
            ITest.SettingCarPos += ITest_SettingCarPos;
            ITest.CarPosConfirm += ITest_CarPosConfirm;
            ITest.StartScan += ITest_StartScan;
            ITest.ShowMotionController += ShowMotionController;
            #endregion 

            (mDockContent[miToolBox] as CtToolBox).SwitchCursor += IGoalSetting_SwitchCursor;
        }
        
        private void IMapCtrl_GLMoveUp(object sender, GLMouseEventArgs e) {
            switch (mCursorMode) {
                case CursorMode.Goal:
                case CursorMode.Power:
                    IGoalSetting.RefreshSingle();
                    mCursorMode = CursorMode.Select;
                    break;
            }
        }

        private void IGoalSetting_SwitchCursor(CursorMode mode) {
            mCursorMode = mode;
            switch (mode) {
                case CursorMode.Drag:
                    IMapCtrl.SetDragMode();
                    break;
                case CursorMode.Goal:
                    IMapCtrl.SetAddMode(FactoryMode.Factory.Goal($"Goal{Database.GoalGM.Count}"));
                    break;
                case CursorMode.Power:
                    IMapCtrl.SetAddMode(FactoryMode.Factory.Power($"Power{Database.PowerGM.Count}"));
                    break;
                case CursorMode.Select:
                    IMapCtrl.SetSelectMode();
                    break;
                case CursorMode.Pen:
                    IMapCtrl.SetPenMode();
                    break;
                case CursorMode.Eraser:
                    IMapCtrl.SetEraserMode(500);
                    break;
                case CursorMode.Insert:
                    OpenFileDialog old = new OpenFileDialog();
                    old.Filter = ".Map|*.map";
                    if (old.ShowDialog() == DialogResult.OK) {
                        IMapCtrl.SetInsertMapMode(old.FileName,mMapInsert as IMouseInsertPanel);
                    }
                    break;
                case CursorMode.ForbiddenArea:
                    IMapCtrl.SetAddMode(FactoryMode.Factory.ForbiddenArea("ForbiddenArea"));
                    break;
                default:
                    throw new ArgumentException($"未定義{mode}模式");
                                    
            }
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
                { miMapGL,new AGVMapUI( DockState.Document )},
                { miToolBox,new CtToolBox(DockState.DockRightAutoHide)}
            };
            SetEvents();

            /*-- 計算每個固定停靠區域所需的顯示大小 --*/
            foreach (var area in Enum.GetValues(typeof(DockAreas))) {
                CalculateFixedPortion(dockPanel, (DockAreas)area);
            }

            /*-- 遍歷所有DockContent與MenuItem物件 --*/
            foreach (var kvp in mDockContent) {
                ToolStripMenuItem item = kvp.Key;
                ICtDockContent dokContent = kvp.Value as ICtDockContent;

                /*-- 參考分配 --*/
                dokContent.AssignmentDockPanel(dockPanel);

                /*-- 顯示視窗 --*/
                if (dokContent.DefaultDockState != DockState.Hidden &&
                    dokContent.DefaultDockState != DockState.Unknown) {
                    dokContent.ShowWindow();
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
            mMapInsert.AssignmentDockPanel(dockPanel);

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

        #endregion Load

        #region Flow

        private void ExecutingInfo() {
            SetBalloonTip("Warning", "請先完成當前操作", ToolTipIcon.Warning);
        }

        private bool CheckServer() {
            CtProgress prog = new CtProgress("Connect", "Connecting...");
            try {
                IsConnected = CheckIsServerAlive();
            } catch (Exception ex) {
                System.Console.WriteLine(ex.Message);
            } finally {
                prog?.Close();
            }
            return IsConnected;
        }

        private bool GetIsConnect() {
            return IsConnected;
        }

        private bool CheckSimilarity() {
            mConnectFlow.CheckFlag("Car pos confirm", () => {
                if (mBypassSocket) {
                    /*-- 空跑模擬CarPosConfirm --*/
                    SpinWait.SpinUntil(() => false, 1000);
                    mSimilarity = 100;
                } else {
                    string[] rtnMsg = SendMsg("Set:ConfirmPos");
                    mSimilarity = double.Parse(rtnMsg[2]);
                }
                IConsole.AddMsg("[Confirm] - Similarity:{0}%", mSimilarity);
            });
            return mSimilarity > mThrSimilarity;
        }

        private bool GetSimilarity() {
            return mSimilarity > mThrSimilarity;
        }

        #endregion Flow

        ///<summary>IP驗證</summary>
        ///<param name="ip">要驗證的字串</param>
        ///<returns>True:合法IP/False:非法IP</returns>
        private bool VerifyIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        
        /// <summary>
        /// 路徑資料分析
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        private List<CartesianPos> PathEncoder(string pack) {
            string[] pathArray = pack.Trim().Split(new char[] { Separator.Data }, StringSplitOptions.RemoveEmptyEntries);
            List<CartesianPos> rtnPoints = null;
            int len = pathArray?.Count() ?? 0;
            if (len != 0 && len % 2 == 0) {
                rtnPoints = new List<CartesianPos>();
                double x, y;
                for (int i = 0; i < pathArray.Count() - 1; i += 2) {
                    string strX = pathArray[i];
                    string strY = pathArray[i + 1];
                    if (double.TryParse(pathArray[i], out x) && double.TryParse(pathArray[i + 1], out y)) {
                        rtnPoints.Add(new CartesianPos(x, y));
                    } else {
                        rtnPoints.Clear();
                        rtnPoints = null;
                        break;
                    }
                }
            }
            return rtnPoints;
        }


        #endregion Function - Private Methods

    }

    #region Suppor - Class

    /// <summary>
    /// 系統列圖示類
    /// </summary>
    public class CtNotifyIcon : IDisposable
    {

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
        public CtNotifyIcon(Form form, string caption = "NotifyIcon", Icon icon = null)
        {
            rForm = form;
            mNotifyIcon.Icon = icon ?? rForm.Icon;
            mNotifyIcon.Text = caption;
            mNotifyIcon.ContextMenu = mContextMenu;
        }

        #endregion Function - Consturctors

        #region Function - Public Methods

        /// <summary>
        /// 增加右鍵選項
        /// </summary>
        /// <param name="item"></param>
        public void MenuItemAdd(MenuItem item)
        {
            if (!mContextMenu.MenuItems.Contains(item)) mContextMenu.MenuItems.Add(item);
        }

        /// <summary>
        /// 移除右鍵選項
        /// </summary>
        /// <param name="item"></param>
        public void MenuItemRemove(MenuItem item)
        {
            if (mContextMenu.MenuItems.Contains(item)) mContextMenu.MenuItems.Remove(item);
        }

        /// <summary>
        /// 顯示系統列圖示
        /// </summary>
        public void ShowIcon()
        {
            mNotifyIcon.Visible = true;
        }

        /// <summary>
        /// 隱藏系統列圖示
        /// </summary>
        public void HideIcon()
        {
            mNotifyIcon.Visible = false;
        }

        /// <summary>
        /// 顯示系統列提示
        /// </summary>
        /// <param name="title"></param>
        /// <param name="context"></param>
        /// <param name="tmo">多久以後關閉</param>
        /// <param name="icon">Icon類型</param>
        public void ShowBalloonTip(string title, string context, int tmo = 15, ToolTipIcon icon = ToolTipIcon.Info)
        {
            mNotifyIcon.ShowBalloonTip(tmo, title, context, icon);
        }

        /// <summary>
        /// 顯示右鍵選單
        /// </summary>
        public void ShowMenuItem()
        {
            /*-- 以反射方式執行ShowContextMenu方法顯示右鍵選單 --*/
            Type t = typeof(NotifyIcon);
            MethodInfo mi = t.GetMethod("ShowContextMenu", BindingFlags.NonPublic | BindingFlags.Instance);
            mi.Invoke(this.mNotifyIcon, null);
        }

        #endregion Function - Public Methods

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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
        public void Dispose()
        {
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
    public class MenuItems : IDisposable
    {

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
        public MenuItem AddMenuItem(string caption, Action<object, EventArgs> even = null, bool enable = true)
        {
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
        public void RemoveMenuItem(MenuItem item)
        {
            if (!mMenuItems.Contains(item)) return;

            int index = mMenuItems.IndexOf(item);

            if (mClickEvents[index] != null) item.Click -= mClickEvents[index];
        }

        /// <summary>
        /// 清空右鍵選單集合
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < mMenuItems.Count; i++)
            {
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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
        public void Dispose()
        {
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
    public class CtGroupBox : GroupBox
    {
        private System.Drawing.Color _BorderColor = System.Drawing.Color.Red;
        [Description("設定或取得外框顏色")]
        public System.Drawing.Color BorderColor {
            get { return _BorderColor; }
            set { _BorderColor = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
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

    public class CtFlagGuard {

        #region Declaration - Fields 
        
        private object mKey = new object();

        private bool mIsExecuting = false;

        #endregion Declaration - Fields

        #region Declaration - Delegates

        public Events.FlagGuard.DelSetFlag SetFlag { get; set; }

        public Events.FlagGuard.DelShowInfo FailureMessage { get; set; }

        public Events.FlagGuard.DelSwitchFlag SwitchFlag { get; set; }

        public Events.FlagGuard.DelUserContinue UserContinue { get; set; }

        public Events.FlagGuard.DelUserSwitch UserSwitch { get; set; }

        public Events.FlagGuard.DelIsAllow IsAllow { get; set; }

        public Events.FlagGuard.DelExecutingInfo ExecutingInfo { get; set; }

        #endregion Declaration - Delegates

        #region Funciton - Public Methods

        public void CheckFlag(string description,Action act,bool cont = true) {
            
            if (!mIsExecuting) {
                lock (mKey) {
                    try {
                        mIsExecuting = true;
                        if (!IsAllow()) {
                            if (!UserSwitch(description)) {
                                return;
                            }
                            SwitchFlag();
                            bool isAllow = IsAllow();
                            SetFlag(isAllow); 
                            if (!isAllow) {
                                FailureMessage();
                                return;
                            }
                            if (!cont || !UserContinue(description)) {
                                return;
                            }
                        }
                        act?.Invoke();
                    } catch (Exception ex) {
                        System.Console.WriteLine("error",ex.Message);
                    } finally {
                        mIsExecuting = false;
                    }
                }
            }else {
                ExecutingInfo();
            }
        }

        public async void CheckFlagAsync(string description,Action act,bool cont = true) {
            if (!mIsExecuting) {
                await Task.Run(() => CheckFlag(description, act, cont));
            }else {
                ExecutingInfo();
            }
        }

        #endregion Function - Public Methods

    }

    public abstract class BaseFlowTemplate {

        protected object mKey = new object();

        protected bool mIsExecuting = false;

        protected IntPtr mMainHandle = IntPtr.Zero;

        protected Events.FlagGuard.DelIsAllow IsAllow = null;

        protected Events.FlagGuard.DelSwitchFlag SwitchFlag = null;

        protected Events.FlagGuard.DelExecutingInfo ExecutingInfo = null;

        public BaseFlowTemplate(
            IntPtr hWnd,
            Events.FlagGuard.DelIsAllow isAllow,
            Events.FlagGuard.DelSwitchFlag switchFlag,
            Events.FlagGuard.DelExecutingInfo executingInfo) {
            mMainHandle = hWnd;
            IsAllow = isAllow;
            SwitchFlag = switchFlag;
            ExecutingInfo = executingInfo;
        }

        public void CheckFlag(string description, Action act, bool cont = true) {
            
            if (!mIsExecuting) {
                lock (mKey) {
                    try {
                        mIsExecuting = true;
                        if (!IsAllow()) {
                            if (!UserSwitch(description)) {
                                return;
                            }
                            if (!SwitchFlag()) {
                                FailureMessage();
                                return;
                            }
                            if (!cont || !UserContinue(description)) {
                                return;
                            }
                        }
                        act?.Invoke();
                    } catch (Exception ex) {
                        System.Console.WriteLine("error", ex.Message);
                    } finally {
                        mIsExecuting = false;
                    }
                }
            }else {
                ExecutingInfo();
            }
        }

        protected virtual bool UserContinue(string description) {
            return MsgBoxBtn.Yes == CtMsgBox.Show(mMainHandle,"Continue", $"是否繼續{description}?", MsgBoxBtn.YesNo, MsgBoxStyle.Question);
        }
        protected abstract void FailureMessage();
        protected abstract bool UserSwitch(string description);
    }

    public class ConnectFlow : BaseFlowTemplate {
        public ConnectFlow(IntPtr hWnd, Events.FlagGuard.DelIsAllow isAllow, Events.FlagGuard.DelSwitchFlag switchFlag, Events.FlagGuard.DelExecutingInfo executingInfo) : base(hWnd, isAllow, switchFlag, executingInfo) {
        }

        protected override void FailureMessage() {
            CtMsgBox.Show(mMainHandle,"Failure", "無法與AGV連線", MsgBoxBtn.OK, MsgBoxStyle.Information);
        }
        
        protected override bool UserSwitch(string description) {
            return MsgBoxBtn.Yes == CtMsgBox.Show(mMainHandle,"Not connected yet", $"尚未與AGV連線無法{description}\r\n是否要連線至AGV?", MsgBoxBtn.YesNo, MsgBoxStyle.Information);
        }

    }

    public class ServoOnFlow : BaseFlowTemplate {
        public ServoOnFlow(IntPtr hWnd, Events.FlagGuard.DelIsAllow isAllow, Events.FlagGuard.DelSwitchFlag switchFlag, Events.FlagGuard.DelExecutingInfo executingInfo) : base(hWnd, isAllow, switchFlag, executingInfo) {
        }

        protected override void FailureMessage() {
            CtMsgBox.Show(mMainHandle, "Failure", "ServoOn失敗", MsgBoxBtn.OK, MsgBoxStyle.Information);
        }

        protected override bool UserSwitch(string description=null) {
            return MsgBoxBtn.Yes == CtMsgBox.Show(mMainHandle, "Not ServoOn yet", $"尚未Servo On無法{description}\r\n是否要Server On?",MsgBoxBtn.YesNo,MsgBoxStyle.Question);
        }
    }

    public class SimilarityFlow : BaseFlowTemplate {
        public SimilarityFlow(IntPtr hWnd, Events.FlagGuard.DelIsAllow isAllow, Events.FlagGuard.DelSwitchFlag switchFlag, Events.FlagGuard.DelExecutingInfo executingInfo) : base(hWnd, isAllow, switchFlag, executingInfo) {
        }

        protected override void FailureMessage() {
            CtMsgBox.Show(mMainHandle, "Failure", "地圖相似度過低", MsgBoxBtn.OK, MsgBoxStyle.Information);
        }

        protected override bool UserSwitch(string description) {
            return MsgBoxBtn.Yes == CtMsgBox.Show("Similarity is too low", $"地圖匹配度過低無法{description}，是否再Confirm一次?", MsgBoxBtn.YesNo, MsgBoxStyle.Question);
        }
    }

    #endregion Support - Class

}
