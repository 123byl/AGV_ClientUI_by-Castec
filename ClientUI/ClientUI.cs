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
//using MapProcessing;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using ClientUI.Component;
using System.IO;
using System.Diagnostics;
using CtLib.Module.Utility;
using System.Text.RegularExpressions;
using Geometry;
using GLCore;
using GLUI;
using UIControl;
using AGVDefine;
using SerialCommunication;
using SerialCommunicationData;
using System.Net.NetworkInformation;

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
        protected bool mIsConnected = false;

        /// <summary>
        /// 是否Bypass Socket通訊
        /// </summary>
        protected bool mBypassSocket = false;

        /// <summary>
        /// 是否Bypass LoadFile功能
        /// </summary>
        private bool mBypassLoadFile = false;

        /// <summary>
        /// 車子模式
        /// </summary>
        protected EMode mCarMode = EMode.OffLine;

        /// <summary>
        /// 當前語系
        /// </summary>
        /// <remarks>
        /// 未來開發多語系用
        /// </remarks>
        private UILanguage mCulture = UILanguage.English;

        /// <summary>
        /// MapGL當前滑鼠模式
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
        /// Vehicle Console端IP
        /// </summary>
        protected string mHostIP {
            get {
                return Properties.Settings.Default.HostIP;
            }
            set {
                if (Properties.Settings.Default.HostIP != value && !string.IsNullOrEmpty(value)) {
                    Properties.Settings.Default.HostIP = value;
                    Properties.Settings.Default.Save();
                }
            }
        }
        
        /// <summary>
        /// 車子馬達轉速
        /// </summary>
        protected int mVelocity = 500;
        
        /// <summary>
        /// 使用者操作權限
        /// </summary>
        private UserData mUser = new UserData("CASTEC", "", AccessLevel.Administrator);
        
        /// <summary>
        /// AGV ID
        /// </summary>
        protected uint mAGVID = 1;

        /// <summary>
        /// 地圖相似度，範圍0%～100%，超過門檻值為-100%
        /// </summary>
        protected double mSimilarity = 0;
        
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
        /// 序列化傳輸物件
        /// </summary>
        private ISerialClient mSerialClient = null;

        private List<TaskCompletionSource<IProductPacket>> mCmdTsk = new List<TaskCompletionSource<IProductPacket>>();

        #endregion Socket

        #endregion Declaration - Members

        #region Declaration - Properties

        #region Flag

        /// <summary>
        /// 是否已建立連線
        /// </summary>
        public virtual bool IsConnected {
            get {
                return mSerialClient?.Connected ?? false;
            }
        }

        /// <summary>
        /// 伺服馬達是否激磁
        /// </summary>
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
        public EMode CarMode {
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
        /// 使用者資料
        /// </summary>
        public UserData UserData { get { return mUser; } }
        
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
        protected IIConsole IConsole { get { return Console; } }
        private IIGoalSetting IGoalSetting { get { return GoalSetting; } }
        private IScene IMapCtrl { get { return MapGL != null ? MapGL.Ctrl : null; } }
        protected IITesting ITest { get { return Testing; } }
        
        #endregion Declaration - Properties

        #region Functin - Constructors

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public AgvClientUI()
        {
            InitializeComponent();
            
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

        /// <summary>
        /// 全域鍵盤按下事件
        /// </summary>
        /// <param name="sneder"></param>
        /// <param name="e"></param>
        private void mKeyboardHook_KeyDownEvent(object sneder,KeyEventArgs e) {
            
        }

        /// <summary>
        /// 全域鍵盤放開事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mKeyboardHook_KeyUpEvent(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Delete://MapGL刪除Goal點快捷鍵
                    /*-- 有可能標示物被刪除，通知GoalSetting界面更新 --*/
                    IGoalSetting.ReloadSingle();
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
            tslbHostIP.Text = mHostIP;
            ITest.SetHostIP(mHostIP);

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

        protected virtual void ITest_StartScan(bool scan) {
            EMode mode = scan ? EMode.Map : EMode.Idle;
            if (mCarMode != mode) {
                if (scan) {
                    string oriName = string.Empty;
                    if (Stat.SUCCESS == CtInput.Text(out oriName, "MAP Name", "Set Map File Name")) {
                        mSerialClient.Send(FactoryMode.Factory.Order().SetScaningOriFileName(oriName));
                        IConsole.AddMsg($"Start scan");
                    } else {
                        return;
                    }
                } else {
                    mSerialClient.Send(FactoryMode.Factory.Order().StopScaning());
                    IConsole.AddMsg($"Stop scan");
                }
                mCarMode = mode;
                ChangedMode(mode);
            }
        }

        protected virtual void ITest_CarPosConfirm() {
            mSerialClient.Send(FactoryMode.Factory.Order().DoPositionComfirm());
            IConsole.AddMsg($"Client - Set:POSComfirm");
        }
        
        private void ITest_SettingCarPos() {
            //mConnectFlow.CheckFlag("Set Car",() => {
                mIsSetting = true;
            //});
        }

        private void ITest_ClearMap() {
            Database.ClearAllButAGV();
            IGoalSetting.ReloadSingle();
            Database.AGVGM[mAGVID].LaserAPoints.DataList.Clear();
            Database.AGVGM[mAGVID].Path.DataList.Clear();
        }

        protected virtual void ITest_MotorServoOn(bool servoOn) {
            mSerialClient.Send(FactoryMode.Factory.Order().SetServoMode(servoOn));
            IConsole.AddMsg($"Client - Set:Servo{(servoOn ? "On" : "Off")}:{servoOn}");
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

        /// <summary>
        /// 與指定IP AGV連線/斷線
        /// </summary>
        /// <param name="cnn">連線/斷線</param>
        /// <param name="hostIP">AGV IP</param>
        /// <exception cref=""
        protected virtual void ITest_ConnectToAGV(bool cnn, string hostIP = "") {
            if (IsConnected != cnn) {
                if (cnn) {
                    if (mSerialClient == null) {
                        mSerialClient = FactoryMode.Factory.SerialClient(mSerialClient_ReceiveData);
                    }
                    PingStatus stt = CtNetwork.Ping(hostIP, 500).PingState;
                    if (CtNetwork.Ping(hostIP,500).PingState != PingStatus.Success) throw new PingException($"PingStatus:{stt}");
                    mSerialClient.Connect(hostIP, (int)EPort.ClientPort);
                    if (IsConnected) {
                        mHostIP = hostIP;
                    }
                } else {
                    mSerialClient.Stop();
                }
                ITest.SetServerStt(IsConnected);
                IConsole.AddMsg($"Client - Is {(IsConnected ? "Connected" : "Disconnected")} to {mHostIP}");
            }
        }

        protected virtual void ITest_SetVelocity(int velocity) {
            mVelocity = velocity;
            mSerialClient.Send(FactoryMode.Factory.Order().SetWorkVelocity(mVelocity));
        }
       
        protected virtual void ITest_SendMap() {
            OpenFileDialog openMap = new OpenFileDialog();
            openMap.InitialDirectory = mDefMapDir;
            openMap.Filter = "MAP|*.map";
            if (openMap.ShowDialog() == DialogResult.OK) {
                SendFile(openMap.FileName);
                mSerialClient.Send(FactoryMode.Factory.Order().UploadMapToAGV(openMap.FileName));
                mSerialClient.Send(FactoryMode.Factory.Order().ChangeMap(openMap.FileName));
            }
        }

        protected virtual void ITest_GetCar() {
            bool getting = !IsGettingLaser;
            mSerialClient.Send(FactoryMode.Factory.Order().AutoReportLaser(getting));
            mSerialClient.Send(FactoryMode.Factory.Order().AutoReportStatus(getting));
            mSerialClient.Send(FactoryMode.Factory.Order().AutoReportPath(getting));
            IConsole.AddMsg($"Client - Get:Car:{getting}");
        }

        protected virtual void ITest_GetLaser() {
            GetLaser();
        }

        protected virtual void ITest_GetMap() {
            mSerialClient.Send(FactoryMode.Factory.Order().RequestMapList());
        }

        protected virtual void ITest_GetORi() {
            mSerialClient.Send(FactoryMode.Factory.Order().RequestOriFileList());
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
                if (CarMode != EMode.Map) CarMode = EMode.Idle;

            },false);
        }

        private void ITest_Motion_Down(MotionDirection direction) {
            mServoOnFlow.CheckFlag($"{direction}",()=> {
                IConsole.AddMsg($"[{direction}]");
                MotionContorl(direction);
                if (CarMode != EMode.Map) CarMode = EMode.Work;                
            },false);
        }

        #endregion

        #region IMapGL事件連結

        private void IMapCtrl_GLClickEvent(object sender, GLMouseEventArgs e) {
            IConsole.AddMsg($"MapClickTrigger - IsSetting:{mIsSetting}");
            if (mIsSetting) {
                if (Database.AGVGM.ContainsID(mAGVID)) {
                    if (mNewPos == null) {
                        IConsole.AddMsg("NewPos=null");
                        mNewPos = e.Position;
                    } else {
                        IConsole.AddMsg($"NewPos{mNewPos.ToString()}");
                        SetPosition(e.Position, mNewPos);
                        mNewPos = null;
                        mIsSetting = false;
                    }
                }
            } else {
                IGoalSetting.SetCurrentRealPos(e.Position);
            }

        }

        private void IMapCtrl_DragTowerPairEvent(object sender, TowerPairEventArgs e) {
            IGoalSetting.ReloadSingle();
        }

        #endregion IMapGL 事件連結

        #region IGoalSetting 事件連結   

        private void IGoalSetting_Charging(IPower power, int idxPower) {
            if (power != null && idxPower >= 0) {
                mSimilarityFlow.CheckFlag("Charging", () => {
                    IConsole.AddMsg($"Client - Charging to idx{idxPower} {power.ToString()}");
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

        private void IGoalSetting_RunLoopEvent(IEnumerable<IGoal> goal) {
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

        private void IGoalSetting_RunGoalEvent(IGoal goal, int idxGoal) {
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

        private void IGoalSetting_FindPathEvent(IGoal goal, int idxGoal) {
            //CheckGoal(goal, idxGoal, () => {
            //    mSimilarityFlow.CheckFlag("Path plann", () => {
                    IConsole.AddMsg("[Find Path] - idx{0} {1}", idxGoal, goal.ToString());
                    IConsole.AddMsg("[AGV Find A Path]");
                    PathPlan(idxGoal);
            //    });
            //});
        }

        private void IGoalSetting_DeleteGoalsEvent(IEnumerable<uint> singles) {
            foreach(var id in singles) {
                if (Database.GoalGM.ContainsID(id)) {
                    Database.GoalGM.Remove(id);
                }else if (Database.PowerGM.ContainsID(id)) {
                    Database.PowerGM.Remove(id);
                }
            }
            IGoalSetting.ReloadSingle();
            
        }

        private void IGoalSetting_ClearGoalsEvent() {
            IConsole.AddMsg("[Clear Goal]");
            Database.GoalGM.Clear();

            IConsole.AddMsg("[Clear Power]");
            Database.PowerGM.Clear();

            IGoalSetting.ReloadSingle();
        }

        private void AddNewGoalEvent(ITowardPair goalPosition) {
            IMapCtrl.SetAddMode(FactoryMode.Factory.Goal(goalPosition,$"Goal{Database.GoalGM.Count}"));
        }

        /// <summary>
        /// 取得所有Goal點名稱
        /// </summary>
        private void GoalNames() {
            TaskCompletionSource<IProductPacket> tskCompSrc = new TaskCompletionSource<IProductPacket>(0);
            var tsk = tskCompSrc.Task;
            mCmdTsk.Add(tskCompSrc);
            mSerialClient.Send(FactoryMode.Factory.Order().RequestGoalList());
            tsk.Wait(500);
            string goalNames = string.Join(",", tsk.Result.ToIRequestGoalList().Product);
            IConsole.AddMsg($"Goal Names:{goalNames}");
        }

        #endregion

        #region ISerialClient
        
        /// <summary>
        /// 序列化通訊接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSerialClient_ReceiveData(object sender, ReceiveDataEventArgs e) {
            if (e.Data is IProductPacket) {
                var product = e.Data as IProductPacket;
                switch (product.Purpose) {
                    case EPurpose.AutoReportLaser:
                        var laser = product.ToIAutoReportLaser().Product;
                        if (laser != null) {
                            IsGettingLaser = true;
                            DrawLaser(product.ToIAutoReportLaser().Product);
                        } else {
                            IsGettingLaser = false;
                            Database.AGVGM[mAGVID].LaserAPoints.DataList.Clear();
                            Database.AGVGM[mAGVID].Path.DataList.Clear();
                        }
                        ITest.SetLaserStt(IsGettingLaser);
                        break;
                    case EPurpose.RequestLaser:
                        DrawLaser(product.ToIRequestLaser().Product);
                        break;
                    case EPurpose.SetServoMode:
                        var servoOn = product.ToISetServoMode().Product;
                        IConsole.AddMsg($"Server - Set:Servo{(servoOn ? "On" : "Off")}:{servoOn}");
                        IsMotorServoOn = servoOn;
                        break;
                    case EPurpose.SetWorkVelocity:
                        IConsole.AddMsg($"Server - Set:SerWorkVelocity:{product.ToISetWorkVelocity().Product}");
                        break;
                    case EPurpose.SetPosition: {
                            var pack = product.ToISetPosition();
                            Database.AGVGM[mAGVID].SetLocation(pack.Order.Design);
                            IConsole.AddMsg($"Server - Set:POS:{pack.Product}");
                            break;
                        }
                    case EPurpose.StartManualControl:
                        bool isMoving = product.ToIStartManualControl().Product;
                        IConsole.AddMsg($"Server - Set:Moving:{isMoving}");
                        break;
                    case EPurpose.SetManualVelocity: {
                            var pack = product.ToISetManualVelocity();
                            if (pack.Product) {
                                var manualVelocity = pack.Order.Design;
                                IConsole.AddMsg($"Server - Set:DriveVelo:{manualVelocity.X},{manualVelocity.Y}");
                            } else {
                                IConsole.AddMsg($"Server - Set:DriveVelo:False");
                            }
                            break;
                        }
                    case EPurpose.StopScaning: {
                            var pack = product.ToIStopScaning();
                            SetAgvStatus(EMode.Idle);
                        }
                        break;
                    case EPurpose.SetScaningOriFileName: {
                            var pack = product.ToISetScaningOriFileName();
                            if (pack.Product) {
                                IConsole.AddMsg($"Server - Set:OriName:{pack.Order.Design}");
                                SetAgvStatus(EMode.Map);
                            } else {
                                IConsole.AddMsg($"Server - Set:OriName:{pack.Product}");
                                SetAgvStatus(EMode.Idle);
                            }
                            break;
                        }
                    case EPurpose.DoPositionComfirm:
                        mSimilarity = product.ToIDoPositionComfirm().Product;
                        IConsole.AddMsg($"Server - Set:POSComfirm:{mSimilarity}");
                        break;
                    case EPurpose.AutoReportPath: {
                            var pack = product.ToIAutoReportPath();
                            //IConsole.AddMsg($"Server - SetPathPlan:idx({pack.Order.Design}):Count({pack.Product.Count})");
                            DrawPath(pack.Product);
                            break;
                        }
                    case EPurpose.DoRuningByGoalIndex: {
                            var pack = product.ToIDoRuningByGoalIndex();
                            IConsole.AddMsg($"Server - SetRun:idx({pack.Order.Design}):{pack.Product}");
                            break;
                        }
                    case EPurpose.DoCharging: {
                            var pack = product.ToIDoCharging();
                            IConsole.AddMsg($"Server - SetCharging:idx({pack.Order.Design}):{pack.Product}");
                            break;
                        }
                    case EPurpose.RequestMapList:
                        var mapList = product.ToIRequestMapList().Product;
                        using (MapList f = new MapList(mapList)) {
                            if (f.ShowDialog() == DialogResult.OK) {
                                mSerialClient.Send(FactoryMode.Factory.Order().RequestMapFile(f.strMapList));
                            }
                        }
                        break;
                    case EPurpose.RequestMapFile:
                        product.ToIRequestMapFile().Product.SaveAs(@"D:\Mapinfo\Client");
                        break;
                    case EPurpose.RequestOriFileList:
                        var oriList = product.ToIRequestOriFileList().Product;
                        using (MapList f = new MapList(oriList)) {
                            if (f.ShowDialog() == DialogResult.OK) {
                                mSerialClient.Send(FactoryMode.Factory.Order().RequestOriFile(f.strMapList));
                            }
                        }
                        break;
                    case EPurpose.RequestOriFile:
                        product.ToIRequestOriFile().Product.SaveAs(@"D:\Mapinfo\Client");
                        break;
                    case EPurpose.UploadMapToAGV: {
                            var pack = product.ToIUploadMapToAGV();
                            IConsole.AddMsg($"Server - Send:Map:{pack.Order.Design.Name}:{pack.Product}");
                            break;
                        }
                    case EPurpose.ChangeMap: {
                            var pack = product.ToIChangeMap();
                            IConsole.AddMsg($"Server - Set:MapName:{(pack.Product ? pack.Order.Design : "False")}");
                            break;
                        }
                    case EPurpose.RequestGoalList:
                        mCmdTsk.Last().SetResult(product);
                        mCmdTsk.Remove(mCmdTsk.Last());
                        break;
                    case EPurpose.AutoReportStatus:
                        var status = product.ToIAutoReportStatus().Product;
                        this.InvokeIfNecessary(() => {
                            if (status.Battery != -1) {
                                tsprgBattery.Value = (int)status.Battery;
                                tslbBattery.Text = $"{status.Battery:0.0}%";
                            }
                            tslbStatus.Text = status.Description.ToString();
                        });
                        Database.AGVGM[mAGVID].SetLocation(status.Data);
                        break;
                    case EPurpose.RequestPath:
                        var path = product.ToIRequestPath().Product;
                        DrawPath(path);
                        break;
                }
            }
        }
        
        #endregion ISerialClient

        #endregion Function - Events
        
        #region Function - Private Methods

        #region Communication
        
        /// <summary>
        /// 要求AGV設定新位置
        /// </summary>
        /// <param name="oldPosition">舊座標</param>
        /// <param name="newPosition">新座標</param>
        protected virtual void SetPosition(IPair oldPosition,IPair newPosition) {
            var position = FactoryMode.Factory.TowardPair(newPosition, oldPosition.Angle(newPosition));
            mSerialClient.Send(FactoryMode.Factory.Order().SetPosition(position));
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
        /// <param name="filePath"></param>
        protected virtual  void SendFile(string filePath) {
            switch (Path.GetExtension(filePath).ToLower()) {
                case ".map":
                    mSerialClient.Send(FactoryMode.Factory.Order().UploadMapToAGV(filePath));
                    mSerialClient.Send(FactoryMode.Factory.Order().ChangeMap(filePath));
                    break;
                case ".ori":

                    break;
            }
        }

        protected void GetLaser() {
            mSerialClient.Send(FactoryMode.Factory.Order().RequestLaser());
        }

        /// <summary>
        /// 移動控制
        /// </summary>
        /// <param name="direction">移動方向</param>
        /// <param name="velocity">移動速度</param>
        protected virtual void MotionContorl(MotionDirection direction) {
            if (direction == MotionDirection.Stop) {
                mSerialClient.Send(FactoryMode.Factory.Order().StartManualControl(false));
                IConsole.AddMsg($"Client - Set:Moving:False");
            } else {
                var cmd = GetMotionICmd(direction);
                if (cmd != null) {
                    mSerialClient.Send(cmd);
                    mSerialClient.Send(FactoryMode.Factory.Order().StartManualControl(true));
                    IConsole.AddMsg($"Client - Set:Moving:True");
                }
            }
        }

        /// <summary>
        /// 前往目標Goal點
        /// </summary>
        /// <param name="numGoal">目標Goal點</param>
        protected virtual void Run(int numGoal) {
            mSerialClient.Send(FactoryMode.Factory.Order().DoRuningByGoalIndex(numGoal));
        }

        /// <summary>
        /// 路徑規劃
        /// </summary>
        /// <param name="no">目標Goal點編號</param>
        protected virtual void PathPlan(int numGoal) {
            mSerialClient.Send(FactoryMode.Factory.Order().RequestPath(numGoal));
        }

        protected virtual void Charging(int numGoal) {
            mSerialClient.Send(FactoryMode.Factory.Order().DoCharging(numGoal));
        }
        
        private IOrderPacket<IPair, bool> GetMotionICmd(MotionDirection direction) {
            int r = 0, l = 0, v = mVelocity;
            switch (direction) {
                case MotionDirection.Forward:
                    r = v;
                    l = v;
                    break;
                case MotionDirection.Backward:
                    r = -v;
                    l = -v;
                    break;
                case MotionDirection.LeftTrun:
                    r = v;
                    l = -v;
                    break;
                case MotionDirection.RightTurn:
                    r = -v;
                    l = v;
                    break;
                default:
                    return null;
            }
            return FactoryMode.Factory.Order().SetManualVelocity(l, r);
        }

        #endregion Communication 

        #region UI

        /// <summary>
        /// 設定AGV狀態
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="status"></param>
        private void SetAgvStatus<T>(T status) where T : struct {
            this.InvokeIfNecessary(() => {
                tslbStatus.Text = status.ToString();
            });
        }

        /// <summary>
        /// 車子模式切換時
        /// </summary>
        /// <param name="mode"></param>
        protected void ChangedMode(EMode mode) {
            tslbStatus.Text = $"{mode}";
            ITest.ChangedScanStt(mode == EMode.Map);
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
        private void CheckGoal(IGoal goal, int idxGoal,Action act) {
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
                
                IGoalSetting.ReloadSingle();

                if (IsConnected) {
                    SendFile(mapPath);
                }
            }
            return true;
        }

        private bool LoadOri(string oriPath) {
            CurOriPath = oriPath;
            NewMap();
            if (!mBypassLoadFile) {//無BypassLoadFile
                
                /*-- 載入Map並取得Map中心點 --*/
                IPair center = Database.LoadOriToDatabase(CurOriPath, mAGVID)?.Center();

                if (center == null) {
                    return false;
                }

                /*-- 移動畫面至Map中心點 --*/
                IMapCtrl.Focus(center);
            } else {//Bypass LoadFile功能
                    /*-- 空跑一秒，模擬檔案載入 --*/
                SpinWait.SpinUntil(() => false, 1000);
            }
            return true;
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
        /// <param name="laser"></param>
        protected void DrawLaser(IEnumerable<IPair> laser) {
            Database.AGVGM[mAGVID]?.LaserAPoints.DataList.Replace(laser);
        }

        /// <summary>
        /// 繪製路徑
        /// </summary>
        /// <param name="path"></param>
        protected void DrawPath(IEnumerable<IPair> path) {
            Database.AGVGM[mAGVID].Path.DataList.Replace(path);
        }

        #endregion Draw

        #region Load

        /// <summary>
        /// 設定事件連結
        /// </summary>
        private void SetEvents() {
            #region IGoalSetting 事件連結     
            IGoalSetting.AddNewGoalEvent += AddNewGoalEvent;
            IGoalSetting.ClearGoalsEvent += IGoalSetting_ClearGoalsEvent;
            IGoalSetting.DeleteSingleEvent += IGoalSetting_DeleteGoalsEvent;
            IGoalSetting.FindPathEvent += IGoalSetting_FindPathEvent;
            IGoalSetting.LoadMapEvent += IGoalSetting_LoadMapEvent;
            IGoalSetting.LoadMapFromAGVEvent += ITest_GetMap;
            IGoalSetting.RunGoalEvent += IGoalSetting_RunGoalEvent;
            IGoalSetting.RunLoopEvent += IGoalSetting_RunLoopEvent;
            IGoalSetting.SaveGoalEvent += IGoalSetting_SaveGoalEvent;
            IGoalSetting.SendMapToAGVEvent += ITest_SendMap;
            IGoalSetting.GetGoalNames += GoalNames;
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
            ITest.SetVelocity += ITest_SetVelocity;
            ITest.Connect += ITest_ConnectToAGV;
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
                    IGoalSetting.ReloadSingle();
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
            return IsConnected;
        }

        private bool GetIsConnect() {
            return IsConnected;
        }

        private bool CheckSimilarity() {
            return mSimilarity==-1;
        }

        private bool GetSimilarity() {
            return mSimilarity ==-1;
        }

        #endregion Flow

        ///<summary>IP驗證</summary>
        ///<param name="ip">要驗證的字串</param>
        ///<returns>True:合法IP/False:非法IP</returns>
        private bool VerifyIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
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
    
    public abstract class BaseFlowTemplate {

        protected object mKey = new object();

        protected bool mIsExecuting = false;

        protected IntPtr mMainHandle = IntPtr.Zero;

        protected Events.FlowTemplate.DelIsAllow IsAllow = null;

        protected Events.FlowTemplate.DelSwitchFlag SwitchFlag = null;

        protected Events.FlowTemplate.DelExecutingInfo ExecutingInfo = null;

        public BaseFlowTemplate(
            IntPtr hWnd,
            Events.FlowTemplate.DelIsAllow isAllow,
            Events.FlowTemplate.DelSwitchFlag switchFlag,
            Events.FlowTemplate.DelExecutingInfo executingInfo) {
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
        public ConnectFlow(IntPtr hWnd, Events.FlowTemplate.DelIsAllow isAllow, Events.FlowTemplate.DelSwitchFlag switchFlag, Events.FlowTemplate.DelExecutingInfo executingInfo) : base(hWnd, isAllow, switchFlag, executingInfo) {
        }

        protected override void FailureMessage() {
            CtMsgBox.Show(mMainHandle,"Failure", "無法與AGV連線", MsgBoxBtn.OK, MsgBoxStyle.Information);
        }
        
        protected override bool UserSwitch(string description) {
            return MsgBoxBtn.Yes == CtMsgBox.Show(mMainHandle,"Not connected yet", $"尚未與AGV連線無法{description}\r\n是否要連線至AGV?", MsgBoxBtn.YesNo, MsgBoxStyle.Information);
        }

    }

    public class ServoOnFlow : BaseFlowTemplate {
        public ServoOnFlow(IntPtr hWnd, Events.FlowTemplate.DelIsAllow isAllow, Events.FlowTemplate.DelSwitchFlag switchFlag, Events.FlowTemplate.DelExecutingInfo executingInfo) : base(hWnd, isAllow, switchFlag, executingInfo) {
        }

        protected override void FailureMessage() {
            CtMsgBox.Show(mMainHandle, "Failure", "ServoOn失敗", MsgBoxBtn.OK, MsgBoxStyle.Information);
        }

        protected override bool UserSwitch(string description=null) {
            return MsgBoxBtn.Yes == CtMsgBox.Show(mMainHandle, "Not ServoOn yet", $"尚未Servo On無法{description}\r\n是否要Server On?",MsgBoxBtn.YesNo,MsgBoxStyle.Question);
        }
    }

    public class SimilarityFlow : BaseFlowTemplate {
        public SimilarityFlow(IntPtr hWnd, Events.FlowTemplate.DelIsAllow isAllow, Events.FlowTemplate.DelSwitchFlag switchFlag, Events.FlowTemplate.DelExecutingInfo executingInfo) : base(hWnd, isAllow, switchFlag, executingInfo) {
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
