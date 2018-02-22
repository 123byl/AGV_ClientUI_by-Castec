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
        
        /// <summary>
        /// 是否正在設定Car Position
        /// </summary>
        private bool mIsSetting = false;
        
        /// <summary>
        /// MapGL當前滑鼠模式
        /// </summary>
        private CursorMode mCursorMode = CursorMode.Select;
        
        /// <summary>
        /// 系統底層物件參考
        /// </summary>
        private CtVehiclePlanner rVehiclePlanner = null;
        
        /// <summary>
        /// Car Position 設定位置
        /// </summary>
        private IPair mNewPos = null;
        
        private IntPtr mHandle = IntPtr.Zero;
        
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
        /// AGV移動控制器
        /// </summary>
        private CtMotionController mMotionController = null;

        /// <summary>
        /// 地圖檔選擇清單
        /// </summary>
        private MapList mMapList = null;

        #endregion UI

        #region Tool
        
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
        
        #endregion Socket

        #endregion Declaration - Members

        #region Declaration - Properties
        
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
        private CtGoalSetting GoalSetting {
            get {
                return mDockContent.ContainsKey(miGoalSetting) ? mDockContent[miGoalSetting] as CtGoalSetting : null;
            }
        }

        private IIGoalSetting IGoalSetting { get { return GoalSetting; } }
        private IScene IMapCtrl { get { return MapGL != null ? MapGL.Ctrl : null; } }
        
        #endregion Declaration - Properties

        #region Functin - Constructors
        
        public VehiclePlannerUI(CtVehiclePlanner vehiclePlanner) {
            if (vehiclePlanner != null) {
                rVehiclePlanner = vehiclePlanner;
                InitializeComponent();

                mHandle = this.Handle;
                rVehiclePlanner = new CtVehiclePlanner();
                rVehiclePlanner.PropertyChanged += rVehiclePlanner_PropertyChanged;
                rVehiclePlanner.ConsoleMessage += rVehiclePlanner_ConsoleMessage;
                rVehiclePlanner.VehiclePlannerEvent += rVehiclePlanner_VehiclePlannerEvent;
                rVehiclePlanner.ErrorMessage += rVehiclePlanner_ErrorMessage;
                rVehiclePlanner.SelectFile = SelectFile;
                rVehiclePlanner.Initial();
            } else {
                this.Close();
            }
        }

        #endregion Function - Constructors

        #region Function - Events

        #region CtVehiclePlanner
        
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
                    IGoalSetting.ReloadSingle();
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
                case PropertyDeclaration.iTSs:
                    var ipList = rVehiclePlanner.iTSs;
                    mTesting.SetIPList(ipList);
                    break;
                case PropertyDeclaration.MainVisible:
                    if (rVehiclePlanner.MainVisible) {
                        this.Show();
                        this.TopMost = true;
                        #region 把DocDocument切回來
                        /// 由於主介面關閉的時候會觸發到DockDocument的FormCloseing事件
                        /// 導致子介面被隱藏
                        /// 這邊在手動把他切回來一次
                        #endregion
                        MapGL.Show();
                        this.TopMost = false;
                    } else {
                        this.Hide();
                    }
                    break;
                case PropertyDeclaration.IsMotorServoOn:
                    mTesting.ChangedMotorStt(rVehiclePlanner.IsMotorServoOn);
                    break;
                case PropertyDeclaration.IsConnected:
                    /*--依連線狀態設定界面--*/
                    mTesting.SetServerStt(rVehiclePlanner.IsConnected);
                    break;
                case PropertyDeclaration.IsScanning:
                    mTesting.ChangedScanStt(rVehiclePlanner.IsScanning);
                    break;
                case PropertyDeclaration.Status:
                    var status = rVehiclePlanner.Status;
                    this.InvokeIfNecessary(() => {
                        if (status.Battery >= 0 && status.Battery <= 100) {
                            tsprgBattery.Value = (int)status.Battery;
                            tslbBattery.Text = $"{status.Battery:0.0}%";
                        }
                        tslbStatus.Text = status.Description.ToString();
                    });
                    break;
                case PropertyDeclaration.IsAutoReport:
                    mTesting.SetLaserStt(rVehiclePlanner.IsAutoReport);
                    break;
                case PropertyDeclaration.MapCenter:
                    IMapCtrl.Focus(rVehiclePlanner.MapCenter);
                    break;
                case PropertyDeclaration.IsBypassSocket:
                    CtInvoke.ToolStripItemChecked(miBypassSocket, rVehiclePlanner.IsBypassSocket);
                    break;
                case PropertyDeclaration.IsBypassLoadFile:
                    CtInvoke.ToolStripItemChecked(miLoadFile, rVehiclePlanner.IsBypassLoadFile);
                    break;
                case PropertyDeclaration.HostIP:
                    this.InvokeIfNecessary(() => {
                        tslbHostIP.Text = rVehiclePlanner.HostIP;
                    });
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
        private void ClientUI_Load(object sender, EventArgs e)
        {
            /*-- 載入ICtDockContent物件 --*/
            LoadICtDockContent();
            
            /*-- 依照使用者權限進行配置 --*/
            UserChanged(rVehiclePlanner.UserData);

            /*-- 檢查Bypass狀態 --*/
            CtInvoke.ToolStripItemChecked(miBypassSocket, rVehiclePlanner.IsBypassSocket);
            CtInvoke.ToolStripItemChecked(miLoadFile, rVehiclePlanner.IsBypassLoadFile);

            /*-- 檢查遠端設備IP --*/
            tslbHostIP.Text = rVehiclePlanner.HostIP;
            mTesting.SetHostIP(rVehiclePlanner.HostIP);
            
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
            rVehiclePlanner.HideWindow();
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
            var usrData = new UserData("N/A", "", AccessLevel.None);
            if (rVehiclePlanner.UserData.Level == AccessLevel.None)
            {   
                using (CtLogin frmLogin = new CtLogin())
                {
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
            rVehiclePlanner.IsBypassSocket = !rVehiclePlanner.IsBypassSocket;
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
        
        #region ITest
        
        /// <summary>
        /// 切換SetCar旗標
        /// </summary>
        private void ITest_SettingCarPos() {
            mIsSetting = true;
        }
        
        /// <summary>
        /// 傳送Map檔
        /// </summary>
        protected virtual void ITest_SendMap() {
            OpenFileDialog openMap = new OpenFileDialog();
            openMap.InitialDirectory = rVehiclePlanner.DefMapDir;
            openMap.Filter = "MAP|*.map";
            if (openMap.ShowDialog() == DialogResult.OK) {
                try {
                    Task.Run(() => rVehiclePlanner.SendAndSetMap(openMap.FileName));
                } catch (Exception ex) {
                    OnConsoleMessage(ex.Message);
                }
            }
        }
        
        /// <summary>
        /// 要求VehicleConsole自動回傳資料
        /// </summary>
        protected virtual void ITest_GetCar() {
            rVehiclePlanner.AutoReport(!rVehiclePlanner.IsAutoReport);
        }
        
        /// <summary>
        /// 載入Map檔
        /// </summary>
        /// <returns></returns>
        private void ITest_LoadMap() {
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
        private void ITest_LoadOri() {
            try {
                LoadFile(FileType.Ori);
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 停止手動控制
        /// </summary>
        private void ITest_Motion_Up() {
            rVehiclePlanner.MotionContorl(MotionDirection.Stop);
        }
        
        #endregion

        #region IMapGL事件連結

        private void IMapCtrl_GLClickEvent(object sender, GLMouseEventArgs e) {
            if (mIsSetting) {
                    if (mNewPos == null) {
                        mNewPos = e.Position;
                    } else {
                        OnConsoleMessage($"NewPos{mNewPos.ToString()}");
                        Task.Run(() => {
                            rVehiclePlanner.SetPosition(e.Position, mNewPos);
                            mNewPos = null;
                            mIsSetting = false;
                        });
                    }
            } 
            //顯示滑鼠點擊的座標
            IGoalSetting.UpdateNowPosition(e.Position);
        }

        private void IMapCtrl_DragTowerPairEvent(object sender, TowerPairEventArgs e) {
            IGoalSetting.ReloadSingle();
        }

        /// <summary>
        /// MapGL滑鼠放開事件處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IMapCtrl_GLMoveUp(object sender, GLMouseEventArgs e) {
            switch (mCursorMode) {
                case CursorMode.Goal:
                case CursorMode.Power:
                    IGoalSetting.ReloadSingle();
                    mCursorMode = CursorMode.Select;
                    break;
            }
        }

        #endregion IMapGL 事件連結

        #region IGoalSetting 事件連結   

        private void IGoalSetting_RunLoopEvent(IEnumerable<IGoal> goal) {
            //int goalCount = goal?.Count() ?? -1;
            //if (goalCount > 0) {
            //    mSimilarityFlow.CheckFlag("Run all", () => {
            //        OnConsoleMessage("[AGV Start Moving...]");
            //        foreach (var item in goal) {
            //            OnConsoleMessage("[AGV Move To] - {0}", item.ToString());
            //            OnConsoleMessage("[AGV Arrived] - {0}", item.ToString());
            //        }
            //        OnConsoleMessage("[AGV Move Finished]");
            //    });
            //}else {
            //    CtMsgBox.Show(mHandle,"No target","尚未選取Goal點，無法進行Run all",MsgBoxBtn.OK,MsgBoxStyle.Information);
            //}
        }

        #endregion

        #region ToolBox

        /// <summary>
        /// 工具箱切換工具事件
        /// </summary>
        /// <param name="mode"></param>
        private void ToolBox_SwitchCursor(CursorMode mode) {
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
                        IMapCtrl.SetInsertMapMode(old.FileName, mMapInsert as IMouseInsertPanel);
                    }
                    break;
                case CursorMode.ForbiddenArea:
                    IMapCtrl.SetAddMode(FactoryMode.Factory.ForbiddenArea("ForbiddenArea"));
                    break;
                default:
                    throw new ArgumentException($"未定義{mode}模式");

            }
        }

        #endregion ToolBox

        #endregion Function - Events

        #region Function - Private Methods

        #region UI

        /// <summary>
        /// 離開程式
        /// </summary>
        private void Exit() {
            rVehiclePlanner.Exit();
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

        /// <summary>
        /// 顯示iTS手動移動控制面板
        /// </summary>
        private void ShowMotionController() {
            if (mMotionController == null) {
                mMotionController = new CtMotionController();
                mMotionController.MotionDown += rVehiclePlanner.MotionContorl;
                mMotionController.MotionUp += ITest_Motion_Up;
                miMotionController.Checked = true;
                mMotionController.FormClosing += (fSender, fE) => {
                    mMotionController.MotionDown -= rVehiclePlanner.MotionContorl;
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
            DockContentVisible(mConsole, console);
            DockContentVisible(GoalSetting, goalSetting);
            DockContentVisible(mTesting, testing);
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
        /// 載入檔案
        /// </summary>
        /// <param name="type">載入檔案類型</param>
        public void LoadFile(FileType type) {
            OpenFileDialog openMap = new OpenFileDialog();
            openMap.InitialDirectory = rVehiclePlanner.DefMapDir;
            openMap.Filter = $"MAP|*.{type.ToString().ToLower()}";
            if (openMap.ShowDialog() == DialogResult.OK) {
                Task.Run(() => {
                    rVehiclePlanner.LoadFile(type,openMap.FileName);
                });
            }
        }
        
        #endregion Draw

        #region Load

        /// <summary>
        /// 設定事件連結
        /// </summary>
        private void SetEvents() {
            
            #region IGoalSetting 事件連結
                 
            IGoalSetting.AddCurrentGoalEvent += rVehiclePlanner.AddCurrentAsGoal;
            IGoalSetting.ClearGoalsEvent += rVehiclePlanner.ClearMarker;
            IGoalSetting.DeleteSingleEvent += rVehiclePlanner.DeleteMarker;
            IGoalSetting.FindPathEvent += rVehiclePlanner.FindPath;
            IGoalSetting.LoadMapEvent += ITest_LoadMap;
            IGoalSetting.LoadMapFromAGVEvent += rVehiclePlanner.GetMap;
            IGoalSetting.RunGoalEvent += rVehiclePlanner.DoRunningByGoalIndex;
            IGoalSetting.RunLoopEvent += IGoalSetting_RunLoopEvent;
            IGoalSetting.SaveGoalEvent += rVehiclePlanner.SaveGoal;
            IGoalSetting.SendMapToAGVEvent += ITest_SendMap;
            IGoalSetting.GetGoalNames += rVehiclePlanner.GetGoalNames;
            IGoalSetting.Charging += rVehiclePlanner.DoCharging;
            IGoalSetting.ClearMap += rVehiclePlanner.ClearMap;

            #endregion

            #region IMapGL 事件連結

            IMapCtrl.GLClickEvent += IMapCtrl_GLClickEvent;
            IMapCtrl.DragTowerPairEvent += IMapCtrl_DragTowerPairEvent;
            IMapCtrl.GLMoveUp += IMapCtrl_GLMoveUp;

            #endregion

            #region ITesting 事件連結

            mTesting.LoadMap += ITest_LoadMap;
            mTesting.LoadOri += ITest_LoadOri;
            mTesting.GetOri += rVehiclePlanner.GetOri;
            mTesting.GetMap += rVehiclePlanner.GetMap;
            mTesting.GetLaser += rVehiclePlanner.RequestLaser;
            mTesting.GetCar += ITest_GetCar;
            mTesting.SendMap += ITest_SendMap;
            mTesting.SetVelocity += rVehiclePlanner.SetWorkVelocity;
            mTesting.Connect += rVehiclePlanner.ConnectToITS;
            mTesting.MotorServoOn += rVehiclePlanner.SetServoMode;
            mTesting.SimplifyOri += rVehiclePlanner.SimplifyOri;
            mTesting.ClearMap += rVehiclePlanner.ClearMap;
            mTesting.SettingCarPos += ITest_SettingCarPos;
            mTesting.CarPosConfirm += rVehiclePlanner.DoPositionComfirm;
            mTesting.StartScan += rVehiclePlanner.StartScan;
            mTesting.ShowMotionController += ShowMotionController;
            mTesting.Find += rVehiclePlanner.FindCar;
            #endregion 

            (mDockContent[miToolBox] as CtToolBox).SwitchCursor += ToolBox_SwitchCursor;

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

        private void OnConsoleMessage(string msg) {
            mConsole.AddMsg(msg);
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
