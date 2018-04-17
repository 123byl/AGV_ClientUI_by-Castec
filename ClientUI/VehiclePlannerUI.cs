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
using System.Threading;
using System.Net;
using System.Net.Sockets;
using VehiclePlanner.Component;
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
using BroadCast;

namespace VehiclePlanner
{

    /// <summary>
    /// 客戶端介面
    /// </summary>
    public partial class VehiclePlannerUI : Form, ICtVersion
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
        /// 是否持續接收資料
        /// </summary>
        private bool mIsAutoReport = false;

        /// <summary>
        /// 是否在手動移動
        /// </summary>
        private bool mIsManualMoving = false;

        /// <summary>
        /// 是否Bypass Socket通訊
        /// </summary>
        protected bool mBypassSocket = false;

        /// <summary>
        /// 是否Bypass LoadFile功能
        /// </summary>
        private bool mBypassLoadFile = false;

        /// <summary>
        /// 是否正在掃描中
        /// </summary>
        private bool mIsScanning = false;

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

        /// <summary>
        /// iTS狀態
        /// </summary>
        private IStatus mStatus;

        private IntPtr mHandle = IntPtr.Zero;
        
        private ConnectFlow mConnectFlow = null;

        private SimilarityFlow mSimilarityFlow = null;

        private ServoOnFlow mServoOnFlow = null;

        /// <summary>
        /// iTS位置名稱對照表
        /// </summary>
        private Dictionary<IPAddress, string> mAgvList = new Dictionary<IPAddress, string>();

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

        /// <summary>
        /// 地圖檔選擇清單
        /// </summary>
        private MapList mMapList = null;

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
        /// VehicleConsole模擬物件
        /// </summary>
        private FakeVehicleConsole mVC = new FakeVehicleConsole();

        private Broadcaster mBroadcast = new Broadcaster();

        #endregion Socket

        #endregion Declaration - Members

        #region Declaration - Properties

        #region Flag

        

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
        
        /// <summary>
        /// iTS狀態
        /// </summary>
        protected IStatus Status {
            get {
                return mStatus;
            }
            private set {
                if (value != null && mStatus != value) {
                    mStatus = value;
                    this.InvokeIfNecessary(() => {
                        if (mStatus.Battery >= 0 && mStatus.Battery <= 100) {
                            tsprgBattery.Value = (int)mStatus.Battery;
                            tslbBattery.Text = $"{mStatus.Battery:0.0}%";
                        }
                        tslbStatus.Text = mStatus.Description.ToString();
                    });
                    Database.AGVGM[mAGVID].SetLocation(mStatus.Data);
                }
            }
        }

        #endregion Declaration - Properties

        #region Functin - Constructors

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public VehiclePlannerUI()
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

            /*-- 委派廣播接收事件 --*/
            mBroadcast.ReceivedData += mBroadcast_ReceivedData;
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

        private  bool CheckServoOn() {
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


        /// <summary>
        /// 通知VehicleConsole開始/停止掃描
        /// </summary>
        /// <param name="scan">True(開始)/False(停止)</param>
        protected virtual void ITest_StartScan(bool scan) {
            try {
                bool? isScanning = null;
                if (mIsScanning != scan) {
                    if (scan) {//開始掃描
                        if (mStatus?.Description == EDescription.Idle) {
                            string oriName = string.Empty;
                            if (Stat.SUCCESS == CtInput.Text(out oriName, "MAP Name", "Set Map File Name")) {
                                isScanning = SetScanningOriFileName(oriName);
                            }
                            if (isScanning == true) {
                                IConsole.AddMsg($"iTS - The new ori name is {oriName}.ori");
                            }
                        } else {
                            IConsole.AddMsg($"The iTS is now in {mStatus?.Description}, can't start scanning");
                        }
                    } else {//停止掃描
                        if (true || mStatus?.Description == EDescription.Map) {
                            isScanning = StopScanning();
                        } else {
                            IConsole.AddMsg($"The iTS is now in {mStatus?.Description}, can't stop scanning");
                        }
                    }
                    if (isScanning != null) {
                        mIsScanning = (bool)isScanning;
                        ITest.ChangedScanStt((bool)isScanning);
                        IConsole.AddMsg($"iTS - Is {(isScanning == true ? "start" : "stop")} scanning");
                    }
                }
            } catch (Exception ex) {
                IConsole.AddMsg("Error:" + ex.Message);
            }
        }

        /// <summary>
        /// 通知VehicleConsole進行位置矯正
        /// </summary>
        protected virtual void ITest_CarPosConfirm() {
            try {
                var similarity = DoPositionComfirm();
                if (similarity != null) {
                    if (similarity >=0 && similarity <= 1) {
                        mSimilarity = (double)similarity;
                        IConsole.AddMsg($"iTS - The map similarity is {mSimilarity:0.0%}");
                    } else if (mSimilarity == -1){
                        mSimilarity = (double)similarity;
                        IConsole.AddMsg($"iTS - The map is now matched");
                    } else {
                        IConsole.AddMsg($"iTS - The map similarity is 0%");
                    }
                }
            } catch (Exception ex) {
                IConsole.AddMsg(ex.Message);
            }
        }
        
        /// <summary>
        /// 切換SetCar旗標
        /// </summary>
        private void ITest_SettingCarPos() {
            mIsSetting = true;
        }

        /// <summary>
        /// 清除地圖
        /// </summary>
        private void ITest_ClearMap() {
            try {
                Database.ClearAllButAGV();
                Database.AGVGM[mAGVID].LaserAPoints.DataList.Clear();
                Database.AGVGM[mAGVID].Path.DataList.Clear();
                IGoalSetting.ReloadSingle();
            } catch (Exception ex) {
                IConsole.AddMsg(ex.Message);
            }
        }

        /// <summary>
        /// 設定伺服馬達激磁
        /// </summary>
        /// <param name="servoOn">是否激磁</param>
        protected virtual void ITest_MotorServoOn(bool servoOn) {
            try {
                var servoOnStt = SetServoMode(servoOn);
                IConsole.AddMsg("Sent");
                if (servoOnStt != null) {
                    IConsole.AddMsg($"iTS - Is Servo{(servoOn ? "On" : "Off")}");
                    IsMotorServoOn = (bool)servoOnStt;
                }
            } catch (Exception ex) {
                IConsole.AddMsg(ex.Message);
            }
        }

        /// <summary>
        /// 設定移動速度
        /// </summary>
        /// <param name="velocity">移動速度</param>
        protected virtual void ITest_SetVelocity(int velocity) {
            try {
                var success = SetWorkVelocity(velocity);
                if (success == true) {
                    mVelocity = velocity;
                    IConsole.AddMsg($"iTS - WorkVelocity = {velocity}");
                }
            }catch(Exception ex) {
                IConsole.AddMsg(ex.Message);
            }
        }

        /// <summary>
        /// 傳送Map檔
        /// </summary>
        protected virtual void ITest_SendMap() {
            try {
                OpenFileDialog openMap = new OpenFileDialog();
                openMap.InitialDirectory = mDefMapDir;
                openMap.Filter = "MAP|*.map";
                if (openMap.ShowDialog() == DialogResult.OK) {
                    Task.Run(() => SendAndSetMap(openMap.FileName));
                }
            } catch (Exception ex) {
                IConsole.AddMsg(ex.Message);
            }
        }
        
        /// <summary>
        /// 要求VehicleConsole自動回傳資料
        /// </summary>
        protected virtual void ITest_GetCar() {
            try {
                bool isAutoReport = !mIsAutoReport;
                var laser =  AutoReportLaser(isAutoReport);
                var status = AutoReportStatus(isAutoReport);
                var path = AutoReportPath(isAutoReport);
                mIsAutoReport = (laser?.Count ?? 0) > 0;
                ITest.SetLaserStt(mIsAutoReport);
            } catch (Exception ex) {
                IConsole.AddMsg(ex.Message);
            }
        }

        /// <summary>
        /// 要求雷射資料
        /// </summary>
        protected virtual void ITest_GetLaser() {
            try {
                var laser = RequestLaser();
                if (laser != null) {
                    if (laser.Count > 0) {
                        IConsole.AddMsg($"iTS - Received {laser.Count} laser data");
                        DrawLaser(laser);
                    } else {
                        IConsole.AddMsg($"iTS - Laser data request failed");
                    }
                }
            } catch (Exception ex) {
                IConsole.AddMsg(ex.Message);
            }
        }

        /// <summary>
        /// 要求VehicleConsole端Map檔清單
        /// </summary>
        protected virtual void ITest_GetMap() {
            if (mMapList == null) {
                bool? success = null;
                string mapName = null;
                try {
                    string mapList = RequestMapList();
                    if (!string.IsNullOrEmpty(mapList)) {
                        mapName = SelectFile(mapList);
                        if (!string.IsNullOrEmpty(mapName)) {
                            var map = RequestMapFile(mapName);
                            if (map != null) {
                                if (map.SaveAs(@"D:\Mapinfo\Client")) {
                                    success = true;
                                    IConsole.AddMsg($"Planner - {map.Name} download completed");
                                } else {
                                    success = false;
                                    IConsole.AddMsg($"Planner - {map.Name} failed to save ");
                                }
                            }
                        }
                    }
                } catch (Exception ex) {
                    IConsole.AddMsg(ex.Message);
                } finally {
                    if (success != null) {
                        SetBalloonTip("Donwload", $"{mapName}.ori is downloaded {(success == true ? "successfully" : "failed")} ");
                    }
                    if (mMapList != null) {
                        mMapList.Dispose();
                        mMapList = null;
                    }
                }
            }
        }

        /// <summary>
        /// 要求VehicleConsole端Ori檔清單
        /// </summary>
        protected virtual void ITest_GetORi() {
            if (mMapList == null) {
                bool? success = null;
                string oriName = null;
                try {
                    string oriList = RequestOriList();
                    if (!string.IsNullOrEmpty(oriList)) {
                        oriName = SelectFile(oriList);
                        if (!string.IsNullOrEmpty(oriName)) {
                            var ori = RequestOriFile(oriName);
                            if (ori != null) {
                                if (ori.SaveAs(@"D:\MapInfo\Client")) {
                                    success = true;
                                    IConsole.AddMsg($"Planner - {ori.Name} download completed");
                                } else {
                                    success = false;
                                    IConsole.AddMsg($"Planner - {ori.Name} failed to save");
                                }
                            }
                        }
                    }
                } catch (Exception ex) {
                    IConsole.AddMsg(ex.Message);
                }finally {
                    if (success != null) {
                        SetBalloonTip("Donwload", $"{oriName}.ori is downloaded {(success == true ? "successfully" : "failed")} ");
                    }
                    if (mMapList != null) {
                        mMapList.Dispose();
                        mMapList = null;
                    }
                }
            }
        }

        /// <summary>
        /// 載入Map檔
        /// </summary>
        /// <returns></returns>
        private void ITest_LoadMap() {
            try {
                LoadFile(FileType.Map);
            } catch (Exception ex) {
                IConsole.AddMsg(ex.Message);
            }
        }

        /// <summary>
        /// 載入Ori檔
        /// </summary>
        /// <returns></returns>
        private void ITest_LoadOri() {
            try {
                LoadFile(FileType.Ori);
            } catch (Exception ex) {
                IConsole.AddMsg(ex.Message);
            }
        }

        /// <summary>
        /// 停止手動控制
        /// </summary>
        private void ITest_Motion_Up() {
            try {
                mConnectFlow.CheckFlag("Motion Controller", () => {
                    MotionContorl(MotionDirection.Stop);
                }, false);
            } catch (Exception ex) {
                IConsole.AddMsg(ex.Message);
            }
        }

        /// <summary>
        /// 開始手動控制
        /// </summary>
        /// <param name="direction">移動方向</param>
        private void ITest_Motion_Down(MotionDirection direction) {
            try {
                mServoOnFlow.CheckFlag($"{direction}", () => {
                    IConsole.AddMsg($"[{direction}]");
                    MotionContorl(direction);
                }, false);
            } catch (Exception ex) {
                IConsole.AddMsg(ex.Message);
            }
        }

        private void ITest_SimplifyOri() {
            try {
                if (mBypassLoadFile) {
                    /*-- 空跑模擬SimplifyOri --*/
                    SpinWait.SpinUntil(() => false, 1000);
                    return;
                }
                string[] tmpPath = CurOriPath.Split('.');
                CurMapPath = tmpPath[0] + ".map";
                Database.Save(CurMapPath);

                NewMap();
            }catch(Exception ex) {
                IConsole.AddMsg(ex.Message);
            }
        }

        private void ITest_Find() {
            if (!mBroadcast.IsReceiving) {
                Task.Run(() => {
                    /*-- 開啟廣播接收 --*/
                    mBroadcast.StartReceive(true);
                    Console.AddMsg("[Planner]: Start searching iTS.");
                    /*-- 清除iTS清單 --*/
                    mAgvList.Clear();
                    /*-- 廣播要求iTS回應 --*/
                    for(int i = 0;i< 3; i++) {
                        mBroadcast.Send("Count off");
                        Thread.Sleep(30);
                    }
                    /*-- 等待iTS回應完畢後停止接收回應 --*/
                    Thread.Sleep(2000);
                    mBroadcast.StartReceive(false);
                    /*-- 取得iTS IP清單 --*/
                    var ipList = mAgvList.Keys.ToList().ConvertAll(v => v.ToString());
                    /*-- 反饋至UI --*/
                    string msg = $"Find {ipList.Count} iTS";
                    Console.AddMsg($"[Planner]:{msg}");
                    SetBalloonTip("Search iTS", msg);
                    ITest.SetIPList(ipList);
                });
            }
        }

        #endregion

        #region IMapGL事件連結

        private void IMapCtrl_GLClickEvent(object sender, GLMouseEventArgs e) {
            if (mIsSetting) {
                if (Database.AGVGM.ContainsID(mAGVID)) {
                    if (mNewPos == null) {
                        mNewPos = e.Position;
                    } else {
                        IConsole.AddMsg($"NewPos{mNewPos.ToString()}");
                        Task.Run(() => {
                            SetPosition(e.Position, mNewPos);

                            mNewPos = null;
                            mIsSetting = false;
                        });
                    }
                }
            } else {//顯示滑鼠點擊的座標
                IGoalSetting.UpdateNowPosition(e.Position);
            }
        }

        private void IMapCtrl_DragTowerPairEvent(object sender, TowerPairEventArgs e) {
            IGoalSetting.ReloadSingle();
        }

        #endregion IMapGL 事件連結

        #region IGoalSetting 事件連結   

        private void IGoalSetting_Charging(uint powerID) {
            try {
                int index = Database.PowerGM.IndexOf(powerID);
                if (index >= 0) {
                    var power = Database.PowerGM[powerID];
                    var success = DoCharging(index);
                    if (success == true) {
                        IConsole.AddMsg($"iTS - Begin charging at {power.Name}");
                    } else if (success == false) {
                        IConsole.AddMsg("iTS - Charging failed");
                    }
                }
            } catch(Exception ex) {
                IConsole.AddMsg(ex.Message);
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

        private void IGoalSetting_RunGoalEvent(uint goalID) {
            try {
                int index = Database.GoalGM.IndexOf(goalID);
                if (index >= 0) {
                    var goal = Database.GoalGM[goalID];
                    var success = DoRunningByGoalIndex(index);
                    if (success == true) {
                        IConsole.AddMsg($"iTS - Start moving to {goal.Name}");
                    } else if (success == false) {
                        IConsole.AddMsg($"Move to goal failure");
                    }
                }       
            } catch(Exception ex) {
                IConsole.AddMsg(ex.Message);
            }
        }
        
        private void IGoalSetting_FindPathEvent(uint id) {
            try {
                int index = Database.GoalGM.IndexOf(id);
                if (index >= 0) {
                    var path = RequestPath(index);
                    if (path != null) {
                        var goal = Database.GoalGM[id];
                        if (path.Count > 0) {
                            IConsole.AddMsg($"iTS - The path to {goal.Name} is completion. The number of path points is {path.Count}");
                        } else {
                            IConsole.AddMsg($"iTS - Can not plan the path to  {goal.Name}");
                        }
                    }
                }
            } catch (Exception ex) {
                IConsole.AddMsg(ex.Message);
            }
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

        /// <summary>
        /// 將當前位置設為Goal點
        /// </summary>
        private void AddCurrentGoalEvent() {
            /*-- 取得當前位置 --*/
            var currentPosition = Database.AGVGM[mAGVID].Data;
            /*-- 建構Goal點 --*/
            var goal = FactoryMode.Factory.Goal(currentPosition, $"Goal{Database.GoalGM.Count}");
            /*-- 分配ID --*/
            var goalID = Database.ID.GenerateID();
            /*-- 將Goal點資料加入Goal點管理集合 --*/
            Database.GoalGM.Add(goalID, goal);
            /*-- 重新載入Goal點資訊 --*/
            IGoalSetting.ReloadSingle();
        }

        /// <summary>
        /// 取得所有Goal點名稱
        /// </summary>
        private void GoalNames() {
            try {
                var goalList = RequestGoalList();
                if (!string.IsNullOrEmpty(goalList)) {
                    IConsole.AddMsg($"iTS - GoalNames:{goalList}");
                }
            } catch(Exception ex) {
                IConsole.AddMsg(ex.Message);
            }
        }

        #endregion

        #region ISerialClient


        #endregion ISerialClient

        #region BroadcastReceiver
        
        /// <summary>
        /// 廣播接收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mBroadcast_ReceivedData(object sender, BroadcastEventArgs e) {
            if (!mAgvList.ContainsKey(e.Remote.Address)) {
                mAgvList.Add(e.Remote.Address, e.Message);
            }
        }

        #endregion BroadcastReceiver

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
            var success = SetPosition(position);
            if (success == true) {
                Database.AGVGM[mAGVID].SetLocation(position);
                IConsole.AddMsg($"iTS - The position are now at {position}");
            }
        }
        
        /// <summary>
        /// 移動控制
        /// </summary>
        /// <param name="direction">移動方向</param>
        /// <param name="velocity">移動速度</param>
        protected virtual void MotionContorl(MotionDirection direction) {
            bool? isManualMoving = null;
            if (direction == MotionDirection.Stop) {
                isManualMoving = StartManualControl(false);
            } else {
                if (SetManualVelocity(direction) == true) {
                    IConsole.AddMsg($"iTS - is {direction},Velocity is {mVelocity}");
                    isManualMoving = StartManualControl(true);
                }
            }
            if (isManualMoving!= null && isManualMoving != mIsManualMoving) {
                mIsManualMoving = (bool)isManualMoving;
                IConsole.AddMsg($"iTS - {(mIsManualMoving ? "Start" : "Stop")} moving");
            }
        }
        
        private bool? SetManualVelocity(MotionDirection direction) {
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
            return SetManualVelocity(FactoryMode.Factory.Pair(l, r));
        }

        #endregion Communication 

        #region UI
        
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
                    SendAndSetMap(mapPath);
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
        public void LoadFile(FileType type) {
            OpenFileDialog openMap = new OpenFileDialog();
            openMap.InitialDirectory = mDefMapDir;
            openMap.Filter = $"MAP|*.{type.ToString().ToLower()}";
            if (openMap.ShowDialog() == DialogResult.OK) {
                Task.Run(() => {
                    try {
                        bool isLoaded = false;
                        switch (type) {
                            case FileType.Ori:
                                isLoaded = LoadOri(openMap.FileName);
                                if (isLoaded) {
                                    Database.AGVGM[mAGVID]?.LaserAPoints.DataList.Clear();
                                    ITest.UnLockOriOperator(true);
                                }
                                break;
                            case FileType.Map:
                                isLoaded = LoadMap(openMap.FileName);
                                break;
                            default:
                                throw new ArgumentException($"無法載入未定義的檔案類型{type}");
                        }
                        if (isLoaded) {
                            SetBalloonTip($"Load { type}", $"\'{openMap.FileName}\' is loaded");
                        } else {
                            CtMsgBox.Show(mHandle, "Error", "File data is wrong, can not read", MsgBoxBtn.OK, MsgBoxStyle.Error);
                        }
                    } catch (Exception ex) {
                        CtMsgBox.Show(mHandle, "Error", ex.Message);
                    }
                });
            }
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
                 
            IGoalSetting.AddCurrentGoalEvent += AddCurrentGoalEvent;
            IGoalSetting.ClearGoalsEvent += IGoalSetting_ClearGoalsEvent;
            IGoalSetting.DeleteSingleEvent += IGoalSetting_DeleteGoalsEvent;
            IGoalSetting.FindPathEvent += IGoalSetting_FindPathEvent;
            IGoalSetting.LoadMapEvent += ITest_LoadMap;
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

            ITest.LoadMap += ITest_LoadMap;
            ITest.LoadOri += ITest_LoadOri;
            ITest.GetOri += ITest_GetORi;
            ITest.GetMap += ITest_GetMap;
            ITest.GetLaser += ITest_GetLaser;
            ITest.GetCar += ITest_GetCar;
            ITest.SendMap += ITest_SendMap;
            ITest.SetVelocity += ITest_SetVelocity;
            ITest.Connect += ConnectToITS;
            ITest.MotorServoOn += ITest_MotorServoOn;
            ITest.SimplifyOri += ITest_SimplifyOri;
            ITest.ClearMap += ITest_ClearMap;
            ITest.SettingCarPos += ITest_SettingCarPos;
            ITest.CarPosConfirm += ITest_CarPosConfirm;
            ITest.StartScan += ITest_StartScan;
            ITest.ShowMotionController += ShowMotionController;
            ITest.Find += ITest_Find;
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

        #region Serial 

        /// <summary>
        /// 傳送並要求載入Map
        /// </summary>
        /// <param name="mapPath">目標Map檔路徑</param>
        private void SendAndSetMap(string mapPath) {
            var success = UploadMapToAGV(mapPath);
            string mapName = Path.GetFileName(mapPath);
            if (success == true) {
                SetBalloonTip("Donwload", $"{mapName}.ori is downloaded {(success == true ? "successfully" : "failed")} ");
                IConsole.AddMsg($"iTS - The {mapName} uploaded");
                success = ChangeMap(mapName);
                if (success == true) {
                    IConsole.AddMsg($"iTS - The {mapName} is now running");
                }else if(success == false) {
                    IConsole.AddMsg($"iTS - The {mapName} failed to run");
                }
            } else if (success == false) {
                IConsole.AddMsg($"iTS - The {mapName} upload failed");
            }
        }

        #endregion Serial 

        ///<summary>IP驗證</summary>
        ///<param name="ip">要驗證的字串</param>
        ///<returns>True:合法IP/False:非法IP</returns>
        private bool VerifyIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

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

        protected abstract string Name { get; set; } 

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
            Console.WriteLine($"{Name} start check. Is executing:{mIsExecuting}");
            if (!mIsExecuting) {
                lock (mKey) {
                    try {
                        mIsExecuting = IsAllow();
                        if (!mIsExecuting) {//當前Flag下不可執行
                            mIsExecuting = UserSwitch(description) && SwitchFlag();
                            if (mIsExecuting) {//Flag切換成功
                                mIsExecuting = cont && UserContinue(description);
                            } else {//Flag切換失敗 
                                FailureMessage();
                            }
                        }
                        if (mIsExecuting) {
                            Console.WriteLine($"{Name} Excuting");
                            act?.Invoke();
                        }
                    } catch (Exception ex) {
                        Console.WriteLine("error", ex.Message);
                    } finally {
                        mIsExecuting = false;
                    }
                }
            }else {
                ExecutingInfo();
            }
            Console.WriteLine($"{Name} checked.Is executing:{mIsExecuting}");
        }

        protected virtual bool UserContinue(string description) {
            return MsgBoxBtn.Yes == CtMsgBox.Show(mMainHandle,"Continue", $"是否繼續{description}?", MsgBoxBtn.YesNo, MsgBoxStyle.Question);
        }
        protected abstract void FailureMessage();
        protected abstract bool UserSwitch(string description);
    }

    public class ConnectFlow : BaseFlowTemplate {
        protected override string Name { get; set; } = "ConnectFlow";

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
        protected override string Name { get; set; } = "ServoOnFlow";

        public ServoOnFlow(IntPtr hWnd, Events.FlowTemplate.DelIsAllow isAllow, Events.FlowTemplate.DelSwitchFlag switchFlag, Events.FlowTemplate.DelExecutingInfo executingInfo) : base(hWnd, isAllow, switchFlag, executingInfo) {
        }

        protected override void FailureMessage() {
            CtMsgBox.Show(mMainHandle, "Failure", "ServoOn失敗", MsgBoxBtn.OK, MsgBoxStyle.Information);
        }

        protected override bool UserSwitch(string description=null) {
            var ret = CtMsgBox.Show(mMainHandle, "Not ServoOn yet", $"尚未Servo On無法{description}\r\n是否要Server On?", MsgBoxBtn.YesNo, MsgBoxStyle.Question);
            return MsgBoxBtn.Yes == ret;
        }
    }

    public class SimilarityFlow : BaseFlowTemplate {

        protected override string Name { get; set; } = "SimilarityFlow";

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
