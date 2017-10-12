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
using ClientUI.Development;
using System.IO;
using AGVMathOperation;
using System.Diagnostics;
using CtLib.Module.Ultity;
using System.Text.RegularExpressions;
using AGV.Map.Common.UI;
using AGV.Map.Core;
using AGV.Map.Common;

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
        /// </remarks>
        public CtVersion Version { get { return new CtVersion(0, 0, 2, "2017/10/11", "Jay Chang"); } }

        #endregion Version - Information

        #region Declaration - Fields


        /// <summary>
        /// 當前Map檔路徑
        /// </summary>
        private string mCurMapPath = string.Empty;

        /// <summary>
        /// 是否已建立連線
        /// </summary>
        private bool mIsConnected = false;

        /// <summary>Opcode 檔案名稱</summary>
        private static readonly string FILENAME_OPCODE = "D1703.opc";

        /// <summary>CtOpcode Object</summary>
        private CtOpcode mOpcode = new CtOpcode();

        /// <summary>
        /// Server端IP
        /// </summary>
        private static string mHostIP = "127.0.0.1";

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
        /// 地圖檔儲存路徑
        /// </summary>
        public string mDefMapDir = @"D:\MapInfo\";

        /// <summary>
        /// 車子馬達轉速
        /// </summary>
        private int mVelocity = 500;

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
        /// 偵測多餘的呼叫
        /// </summary>
        private bool disposedValue = false;

        /// <summary>
        /// 模組版本集合
        /// </summary>
        private Dictionary<string, string> mModuleVersions = new Dictionary<string, string>();

        /// <summary>
        /// 使用者操作權限
        /// </summary>
        private UserData mUser = new UserData("CASTEC", "", AccessLevel.ADMINISTRATOR);

        /// <summary>
        /// 當前語系
        /// </summary>
        /// <remarks>
        /// 未來開發多語系用
        /// </remarks>
        private UILanguage mCulture = UILanguage.ENGLISH;

        /// <summary>
        /// Socket通訊物件
        /// </summary>
        private Communication serverComm = new Communication(400, 600, 800);

        /// <summary>
        /// Laser ID
        /// </summary>
        private uint mLaserID = 1;

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
        /// 車子資訊
        /// </summary>
        private CarInfo mCarInfo = new CarInfo(0, 0, 0, "AGV", 1);

        /// <summary>
        /// 車子模式
        /// </summary>
        private CarMode mCarMode = CarMode.OffLine;

        private MapMatching mMapMatch = new MapMatching();

        /// <summary>
        /// 是否Bypass Socket通訊
        /// </summary>
        private bool mBypassSocket = false;

        /// <summary>
        /// 是否Bypass LoadFile功能
        /// </summary>
        private bool mBypassLoadFile = false;

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
        private List<CartesianPosInfo> Goals;

        #endregion Declaration - Fields

        #region Declaration - Properties

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
                mIsConnected = value;
                //RaiseGoalSettingEvent(GoalSettingEventType.Connect, value);
            }
        }

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
                //if (value == CarMode.Map) {
                //    string oriName = string.Empty;
                //    CtInput txtBox = new CtInput();
                //    if (Stat.SUCCESS == txtBox.Start(
                //        CtInput.InputStyle.TEXT,
                //        "Set Map File Name", "MAP Name",
                //        out oriName,
                //        $"MAP{DateTime.Today:MMdd}")) {
                //        SendMsg($"Set:OriName:{oriName}.Ori");
                //    } else {
                //        return;
                //    }
                //}
                //mCarMode = value;
                //SendMsg($"Set:Mode:{mCarMode}");
                //RaiseAgvClientEvent(AgvClientEventType.Mode, mCarMode);
            }
        }

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
        private IUIBaseCtrl IMapCtrl { get { return MapGL != null ? MapGL.Ctrl : null; } }
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
        }

        #endregion Function - Constructors

        #region Function - Events

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
                    (mDockContent[item] as DockContent).Hide();
                }
                else
                {
                    mDockContent[item].ShowWindow();
                }
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
            if (mUser.Level == AccessLevel.NONE)
            {
                using (CtLogin frmLogin = new CtLogin())
                {
                    stt = frmLogin.Start(out mUser);
                }
            }
            else
            {
                mUser = new UserData("N/A", "", AccessLevel.NONE);
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
            using (CtUserManager frmUsrMgr = new CtUserManager(mUser))
            {
                frmUsrMgr.ShowDialog();
            }
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
        private void rAgvClient_OnAgvClientEventTrigger(object sender, AgvClientEventArgs e)
        {
            switch (e.Type)
            {
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
                    SetBalloonTip("Send File", e.Value as string, ToolTipIcon.Info, 10);
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
        public void SetBalloonTip(string title, string context, ToolTipIcon icon, int tmo)
        {
            mNotifyIcon.ShowBalloonTip(title, context, tmo, icon);
        }

        #endregion NotifyIcon

        #endregion Function - Events

        #region Functoin - Public Methods

        #endregion Function - Public Methdos

        #region Function - Private Methods

        /// <summary>
        /// 車子資訊接收執行緒
        /// </summary>
        public void tsk_RecvCmd(object obj) {
            SocketMonitor soxMonitor = obj as SocketMonitor;
            Socket sRecvCmdTemp = null;
            sRecvCmdTemp = serverComm.ClientAccept(soxMonitor.Socket);
            try {
                while (IsGettingLaser) {

                    SpinWait.SpinUntil(() => false, 1);//每個執行緒內部的閉環裡面都要加個「短時間」睡眠，使得執行緒佔用資源得到及時釋放
                                                       //Thread.Sleep(1);
                    byte[] recvBytes = new byte[1024 * 500];//開啟一個緩衝區，存儲接收到的資訊
                    sRecvCmdTemp.Receive(recvBytes); //將讀得的內容放在recvBytes中
                    string strRecvCmd = Encoding.Default.GetString(recvBytes);//
                                                                              //程式運行到這個地方，已經能接收到遠端發過來的命令了
                    strRecvCmd = strRecvCmd.Split(new char[] { '\0' })[0];
                    //Console.WriteLine("[Server] : " + strRecvCmd);

                    //*************
                    //解碼命令，並執行相應的操作----如下面的發送本機圖片
                    //*************

                    string[] strArray = strRecvCmd.Split(':');
                    recvBytes = null;
                    if (CarInfo.TryParse(strRecvCmd, out mCarInfo)) {
                        tsprgBattery.Value = mCarInfo.PowerPercent;
                        tslbBattery.Text = string.Format(tslbBattery.Tag.ToString(), mCarInfo.PowerPercent);
                        tslbStatus.Text = mCarInfo.Status;
                        DrawLaser(mCarInfo);
                        sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Get:Car:True:True"));
                    } else {
                        sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Get:Car:False"));
                    }

                    strRecvCmd = null;
                    strArray = null;
                }
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

        ///<summary>IP驗證</summary>
        ///<param name="ip">要驗證的字串</param>
        ///<returns>True:合法IP/False:非法IP</returns>
        private bool VerifyIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 向AGV要求所有Goal點名稱
        /// </summary>
        /// <returns>Goal List:"{goal1},{goal2},{goal3}..."</returns>
        /// <remarks>用來模擬iS向AGV要求所有Goal點名稱</remarks> 
        private string GetGoalNames()
        {
            string goalList = "Empty";
            if (mBypassSocket)
            {
                /*-- 模擬用資料 --*/
                goalList = "GoalA,GoalB,GoalC";
            }
            else
            {
                string[] rtnMsg = SendMsg($"Get:GoalList");
                if (rtnMsg.Count() > 3)
                {
                    goalList = rtnMsg[3];
                }
            }
            return goalList;
        }

        /// <summary>
        /// 連接至Server端(AGV)
        /// </summary>
        /// <returns><see cref="Socket"/>連線狀態 True:已連線/False:已斷開</returns>
        private bool ConnectServer()
        {
            return serverComm.ConnectServer(ref mSoxCmd, mHostIP, mRecvCmdPort);
        }

        /// <summary>
        /// 與Server端(AGV)斷開連線
        /// </summary>
        /// <returns><see cref="Socket"/>連線狀態 True:已連線/False:已斷開</returns>
        public bool DisConnectServer()
        {
            return serverComm.DisconnectServer(mSoxCmd);
        }

        /// <summary>
        /// Send file of server to client
        /// </summary>
        /// <param name="clientIP">Ip address of client</param>
        /// <param name="clientPort">Communication port</param>
        /// <param name="fileName">File name</param>
        /// 
        private void SendFile(string clientIP, int clientPort, string fileName)
        {
            string curMsg = "";
            try
            {
                IPAddress[] ipAddress = Dns.GetHostAddresses(clientIP);
                IPEndPoint ipEnd = new IPEndPoint(ipAddress[0], clientPort);
                /* Make IP end point same as Server. */
                Socket clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                /* Make a client socket to send data to server. */
                string filePath = "D:\\MapInfo\\";
                /* File reading operation. */
                fileName = fileName.Replace("\\", "/");
                while (fileName.IndexOf("/") > -1)
                {
                    filePath += fileName.Substring(0, fileName.IndexOf("/") + 1);
                    fileName = fileName.Substring(fileName.IndexOf("/") + 1);
                }
                byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
                if (fileNameByte.Length > 1024 * 1024 * 5)
                {
                    curMsg = "File size is more than 850kb, please try with small file.";
                    return;
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
            }
            catch (Exception ex)
            {
                if (ex.Message == "No connection could be made because the target machine actively refused it")
                    curMsg = "File Sending fail. Because server not running.";
                else
                    curMsg = "File Sending fail." + ex.Message;
            }
        }

        /// <summary>
        /// 傳送檔案
        /// </summary>
        private async void SendFile()
        {
            string[] rtnMsg = SendMsg("Send:map");
            if (mBypassSocket || rtnMsg.Count() > 2 && "True" == rtnMsg[2])
            {
                OpenFileDialog openMap = new OpenFileDialog();
                openMap.InitialDirectory = mDefMapDir;
                openMap.Filter = "MAP|*.ori;*.map";
                if (openMap.ShowDialog() == DialogResult.OK)
                {
                    CtProgress prog = new CtProgress("Send Map", "The file are being transferred");
                    try
                    {
                        await Task.Run(() =>
                        {
                            string fileName = CtFile.GetFileName(openMap.FileName);
                            if (!mBypassSocket)
                            {
                                SendFile(mHostIP, mSendMapPort, fileName);
                            }
                            else
                            {
                                /*-- 空跑模擬檔案傳送中 --*/
                                SpinWait.SpinUntil(() => false, 1000);
                            }
                            IConsole.AddMsg($"Send File {fileName}");
                            SetBalloonTip("Send File", fileName, ToolTipIcon.Info, 10);
                        });
                    }
                    finally
                    {
                        prog?.Close();
                        prog = null;
                    }
                }
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
        public bool ChangeSendInfo()
        {
            try {
                IsGettingLaser = !IsGettingLaser;
                if (IsGettingLaser) {
                    /*-- 開啟車子資訊讀取執行緒 --*/
                    mSoxMonitorCmd.Start();

                    /*-- 向Server端要求車子資料 --*/
                    string[] rtnMsg = SendMsg("Get:Car:True");
                    IsGettingLaser = mBypassSocket || (rtnMsg.Count() > 2 && "True" == rtnMsg[2]);

                    /*-- 車子未發送資料則關閉Socket --*/
                    if (!IsGettingLaser) {
                        mSoxMonitorCmd.Socket.Shutdown(SocketShutdown.Both);
                        mSoxMonitorCmd.Socket.Close();
                    }
                } else {
                    SendMsg("Get:Car:False");
                }
                ITest.SetLaserStt(IsGettingLaser);
            } catch (Exception ex) {
                System.Console.WriteLine(ex.Message);
            }
            
            return IsGettingLaser;
        }

        private void GetLaser()
        {
            /*-- 若是雷射資料則更新資料 --*/
            string[] rtnMsg = SendMsg("Get:Laser");
            if (rtnMsg.Length > 3)
            {
                if (rtnMsg[1] == "Laser")
                {
                    string[] sreRemoteLaser = rtnMsg[3].Split(',');
                    mCarInfo.LaserData = sreRemoteLaser.Select(x => int.Parse(x));
                    DrawLaser(mCarInfo);
                }
            }
        }

        /// <summary>
        /// 取得雷射
        /// </summary>
        private void DrawLaser(CarInfo info)
        {
            double angle = 0D, Laserangle = 0D;
            ILaserPoints points = Factory.FMuti.LaserPoints();
            int idx = 0;
            foreach (int dist in info.LaserData)
            {
                if (dist >= 30 && dist < 15000)
                {
                    int[] pos = Transformation.LaserPoleToCartesian(dist, -135, 0.25, idx++, 43, 416.75, 43, info.x, info.y, info.theta, out angle, out Laserangle);//, out dataAngle, out laserAngle);
                    points.DataList.Add(Factory.FGeometry.Pair(pos[0], pos[1]));
                    pos = null;
                }
            }
            Database.LaserPointsGM.Add(mLaserID, points);
        }


        /// <summary>
        /// 移動控制
        /// </summary>
        /// <param name="direction">移動方向</param>
        /// <param name="velocity">移動速度</param>
        private void MotionContorl(MotionDirection direction, int velocity = 0)
        {
            string[] rtnMsg = SendMsg("Get:IsOpen");


            if (rtnMsg.Count() > 2 && bool.Parse(rtnMsg[2]))
            {

                if (direction == MotionDirection.Stop)
                {
                    SendMsg("Set:Stop");
                }
                else
                {

                    string cmd = string.Empty;
                    switch (direction)
                    {
                        case MotionDirection.Forward:
                            cmd = $"Set:DriveVelo:{mVelocity}:{mVelocity}";
                            break;
                        case MotionDirection.Backward:
                            cmd = $"Set:DriveVelo:-{mVelocity}:-{mVelocity}";
                            break;
                        case MotionDirection.LeftTrun:
                            cmd = $"Set:DriveVelo:{mVelocity}:-{mVelocity}";
                            break;
                        case MotionDirection.RightTurn:
                            cmd = $"Set:DriveVelo:-{mVelocity}:{mVelocity}";
                            break;
                    }
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
        private bool GetFileList(FileType type, out string fileList)
        {
            bool ret = true;
            fileList = string.Empty;
            if (mBypassSocket)
            {
                fileList = $"{type}1,{type}2,{type}3";
            }
            else
            {
                string[] rtnMsg = SendMsg($"Get:{type}List");
                fileList = rtnMsg[3];
            }
            return ret;
        }

        private void GetFile(FileType type)
        {
            string fileList = string.Empty;
            if (GetFileList(type, out fileList))
            {
                using (MapList f = new MapList(fileList))
                {
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        FileDownload(f.strMapList, type);
                    }
                }
            }
        }

        /// <summary>
        /// 檔案下載
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        public void FileDownload(string fileName, FileType type)
        {
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
        private void Run(int numGoal)
        {
            /*-- 若是路徑資料則開始接收資料 --*/
            string[] rtnMsg = SendMsg($"Set:Run:{numGoal}");
            if ((rtnMsg?.Length ?? 0) > 3 &&
                rtnMsg[1] == "Run" &&
                rtnMsg[3] == "Done")
            {
                mSoxMonitorPath.Start();
            }
        }

        /// <summary>
        /// 路徑規劃
        /// </summary>
        /// <param name="no">目標Goal點編號</param>
        private void PathPlan(int numGoal)
        {
            /*-- 若是路徑資料則開始接收資料 --*/
            string[] rtnMsg = SendMsg($"Set:PathPlan:{numGoal}");
            if ((rtnMsg?.Count() ?? 0) > 3 &&
                rtnMsg[1] == "PathPlan" &&
                rtnMsg[2] == "True")
            {
                mSoxMonitorPath.Start();
            }
        }

        /// <summary>
        /// 訊息傳送(會觸發事件)
        /// </summary>
        /// <param name="sendMseeage">傳送訊息內容</param>
        /// <param name="passChkConn">是否略過檢查連線狀態</param>
        /// <returns>Server端回應</returns>
        private string[] SendMsg(string sendMseeage, bool passChkConn = true)
        {
            if (mBypassSocket)
            {
                /*-- Bypass略過不傳 --*/
                return new string[] { "True" };
            }
            else if (passChkConn && !IsServerAlive)
            {
                /*-- 略過連線檢查且Server端未運作 --*/
                return new string[] { "False" };
            }

            /*-- 顯示發送出去的訊息 --*/
            string msg = $"{DateTime.Now} [Client] : {sendMseeage}\r\n";
            IConsole.AddMsg(msg);

            /*-- 等待Server端的回應 --*/
            string rtnMsg = SendStrMsg(mHostIP, mRecvCmdPort, sendMseeage).Trim();
            
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
        private string SendStrMsg(string serverIP, int requerPort, string sendMseeage)
        {

            //可以在字串編碼上做文章，可以傳送各種資訊內容，目前主要有三種編碼方式：
            //1.自訂連接字串編碼－－微量
            //2.JSON編碼--輕量
            //3.XML編碼--重量
            int state;
            int timeout = 5000;
            byte[] recvBytes = new byte[8192];//開啟一個緩衝區，存儲接收到的資訊

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIP.ToString()), requerPort);
            Socket answerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            answerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
            answerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, timeout);
            try
            {

                answerSocket.Connect(ipEndPoint);//建立Socket連接
                byte[] sendContents = Encoding.UTF8.GetBytes(sendMseeage);
                state = answerSocket.Send(sendContents, sendContents.Length, 0);//發送二進位資料
                state = answerSocket.Receive(recvBytes);
                string strRecvCmd = Encoding.Default.GetString(recvBytes);//
                strRecvCmd = strRecvCmd.Split(new char[] { '\0' })[0];
                sendContents = null;
                return strRecvCmd;

            }
            catch (SocketException se)
            {
                //Console.WriteLine("SocketException : {0}", se.ToString());
                //MessageBox.Show("目標拒絕連線!!");
                return "False";
            }
            catch (ArgumentNullException ane)
            {
                //Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                return "False";
            }
            catch (Exception ex)
            {
                //Console.Write(ex.Message);
                return "False";
            }
            finally
            {
                ipEndPoint = null;
                recvBytes = null;
                // answerSocket.Shutdown(SocketShutdown.Both);
                // answerSocket.Disconnect(false);
                answerSocket.Close();
                // Console.Write("Disconnecting from server...\n");
                //Console.ReadKey();
                answerSocket.Dispose();
            }

        }

        private IObstacleLines ConvertToObstacleLines(List<MapLine> lines)
        {
            IObstacleLines obstacleLines = Factory.FMuti.ObstacleLines();
            foreach (var item in lines)
            {
                obstacleLines.DataList.Add(Factory.FGeometry.Line(item.start.x, item.start.y, item.end.x, item.end.y));
            }
            return obstacleLines;
        }

        private IObstaclePoints ConvertToObstaclePoints(List<CartesianPos> points)
        {
            IObstaclePoints obstaclePoints = Factory.FMuti.ObstaclePoints();
            foreach (var item in points)
            {
                obstaclePoints.DataList.Add(Factory.FGeometry.Pair(item.x, item.y));
            }
            return obstaclePoints;
        }

        private ILaserPoints ConvertToLaserPoints(List<CartesianPos> points)
        {
            ILaserPoints laserPoints = Factory.FMuti.LaserPoints();
            foreach (var item in points)
            {
                laserPoints.DataList.Add(Factory.FGeometry.Pair(item.x, item.y));
            }
            return laserPoints;
        }

        /// <summary>
        /// 載入地圖
        /// </summary>
        /// <param name="mapPath">Map檔路徑</param>
        private void LoadMap(string mapPath)
        {
            List<CartesianPosInfo> goalList = null;
            List<CartesianPos> obstaclePoints = null;
            List<MapLine> obstacleLine = null;
            mCurMapPath = mapPath;
            string mPath = CtFile.GetFileName(mapPath);
            SendMsg($"Set:MapName:{mPath}");
            Stopwatch sw = new Stopwatch();
            CtProgress prog = null;
            int nowProg = 0;
            if (mBypassLoadFile)
            {
                /*-- 空跑1秒模擬載入Map檔 --*/
                SpinWait.SpinUntil(() => false, 1000);
            }
            else
            {
                #region - Retrive information from .map file -
                sw.Start();
                using (MapReading read = new MapReading(mCurMapPath))
                {
                    read.OpenFile();
                    read.ReadMapGoalList(out goalList);
                    read.ReadMapObstacleLines(out obstacleLine);
                    read.ReadMapObstaclePoints(out obstaclePoints);
                }
                int total = obstacleLine.Count + 2;

                prog = new CtProgress(ProgBarStyle.Percent, "Load Map", $"Loading {mapPath}", total);
                System.Console.WriteLine($"Read:{sw.ElapsedMilliseconds}ms");
                sw.Restart();

                Goals = goalList;

                mMapMatch.Reset();

                foreach (var line in obstacleLine)
                {
                    mMapMatch.AddLine(line);
                    prog.UpdateStep(nowProg++);
                }

                System.Console.WriteLine($"Read Line:{sw.ElapsedMilliseconds}ms");
                sw.Restart();

                mMapMatch.AddPoint(obstaclePoints);

                prog.UpdateStep(nowProg++);

                System.Console.WriteLine($"Read Point:{sw.ElapsedMilliseconds}ms");
                sw.Restart();

                #endregion
            }

            IMapCtrl.NewMap();
            Database.ObstacleLinesGM.Add(mObstacleLinesID, ConvertToObstacleLines(obstacleLine));
            Database.ObstaclePointsGM.Add(mObstaclePointsID, ConvertToObstaclePoints(obstaclePoints));
            System.Console.WriteLine($"Draw:{sw.ElapsedMilliseconds}ms");
            sw.Restart();

            for (int i = 0; i < goalList.Count; i++)
            {
                Database.GoalGM.Add(goalList[i].id, Factory.FSingle.FTowardPair.Goal((int)goalList[i].x, (int)goalList[i].y, goalList[i].theta, goalList[i].name));
            }
            prog.UpdateStep(nowProg++);

            System.Console.WriteLine($"GoalList:{sw.ElapsedMilliseconds}ms");
            sw.Restart();

            prog.Close();
            prog = null;
            GoalSetting.LoadGoals(goalList);

            goalList = null;
            obstaclePoints = null;
            obstacleLine = null;
        }
        /// <summary>
        /// 載入Ori檔
        /// </summary>
        /// <param name="oriPath"></param>
        /// <returns></returns>
        private void LoadOri(string oriPath)
        {
            CurOriPath = oriPath;
            IMapCtrl.NewMap();
            Database.ObstaclePointsGM.Add(mObstaclePointsID, Factory.FMuti.ObstaclePoints());
            MapReading MapReading = null;
            if (!mBypassLoadFile)
            {//無BypassLoadFile
                MapReading = new MapReading(CurOriPath);
                CartesianPos carPos = null;
                List<CartesianPos> laserData = null;
                //List<Point> listMap = new List<Point>();
                int dataLength = MapReading.OpenFile();
                if (dataLength != 0)
                {
                    CtProgress prog = new CtProgress(ProgBarStyle.Percent, $"Load Ori", $"Loading {oriPath}...", dataLength - 1);
                    try
                    {
                        IMapCtrl.Zoom.Value = IMapCtrl.Zoom.Max;
                        for (int n = 0; n < dataLength; n++)
                        {
                            MapReading.ReadScanningInfo(n, out carPos, out laserData);
                            Database.AGVGM.Add(mAGVID, Factory.FSingle.FTowardPair.AGV((int)carPos.x, (int)carPos.y, carPos.theta, "AGV"));
                            ILaserPoints points = ConvertToLaserPoints(laserData);
                            Database.LaserPointsGM.Add(mLaserID, points);
                            Database.ObstaclePointsGM.SaftyEdit(mObstaclePointsID, (item) => item.DataList.AddRange(points.DataList.AsReadOnly()));
                            IMapCtrl.Focus((int)carPos.x, (int)carPos.y);
                            Thread.Sleep(10);
                            System.Console.WriteLine(n);
                            prog.UpdateStep(n);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        prog?.Close();
                        prog = null;
                    }

                }
            }
            else
            {//Bypass LoadFile功能
             /*-- 空跑一秒，模擬檔案載入 --*/
                SpinWait.SpinUntil(() => false, 1000);
            }
            MapReading = null;
        }

        /// <summary>
        /// 載入檔案
        /// </summary>
        /// <param name="type">載入檔案類型</param>
        public async void LoadFile(FileType type)
        {
            OpenFileDialog openMap = new OpenFileDialog();
            openMap.InitialDirectory = mDefMapDir;
            openMap.Filter = $"MAP|*.{type.ToString().ToLower()}";
            if (openMap.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    switch (type)
                    {
                        case FileType.Ori:
                            await Task.Run(() => LoadOri(openMap.FileName));
                            Database.LaserPointsGM.Remove(mLaserID);
                            ITest.UnLockOriOperator(true);
                            //RaiseTestingEvent(TestingEventType.CurOriPath);
                            break;
                        case FileType.Map:
                            await Task.Run(() => LoadMap(openMap.FileName));
                            break;
                        default:
                            throw new ArgumentException($"無法載入未定義的檔案類型{type}");
                    }
                    SetBalloonTip($"Load { type}", $"The { type} is loaded", ToolTipIcon.Info, 10);
                }
                catch (Exception ex)
                {
                    CtMsgBox.Show("Error", ex.Message);
                }
            }
            openMap = null;
        }

        /// <summary>
        /// 車子模式切換時
        /// </summary>
        /// <param name="mode"></param>
        private void ChangedMode(CarMode mode)
        {
            tslbStatus.Text = $"{mode}";
        }

        /// <summary>
        /// 載入ICtDockContent物件
        /// </summary>
        private void LoadICtDockContent()
        {
            if (mDockContent != null) return;
            /*-- 載入DockContent --*/
            mDockContent = new Dictionary<ToolStripMenuItem, ICtDockContent>() {
                { miConsole,new CtConsole(DockState.DockBottomAutoHide)},
                { miGoalSetting,new CtGoalSetting(DockState.DockLeft)},
                { miTesting,new CtTesting(DockState.DockLeft)},
                { miMapGL,new AGVMapUI( DockState.Document)}
            };
            SetEvents();

            /*-- 計算每個固定停靠區域所需的顯示大小 --*/
            foreach (var area in Enum.GetValues(typeof(DockAreas)))
            {
                CalculateFixedPortion(dockPanel, (DockAreas)area);
            }

            /*-- 遍歷所有DockContent與MenuItem物件 --*/
            foreach (var kvp in mDockContent)
            {
                ToolStripMenuItem item = kvp.Key;
                ICtDockContent dokContent = kvp.Value as ICtDockContent;

                /*-- 參考分配 --*/
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
        private void LoadCtNotifyIcon()
        {
            if (mNotifyIcon == null)
            {
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
        private void CalculateFixedPortion(DockPanel panel, DockAreas area)
        {
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
            if (DockMth.CalculatePortion(area, dockSize, out portion))
            {
                switch (area)
                {
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
        private void ShowWindow()
        {
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
        private void Exit()
        {
            this.Dispose();
        }

        /// <summary>
        /// 切換CtDockContemt可視狀態
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dockContent">CtDockContent物件</param>
        /// <param name="vis">可視狀態</param>
        private void DockContentVisible(ICtDockContent dockContent, bool vis)
        {
            try
            {
                if (vis)
                {
                    dockContent.ShowWindow();
                }
                else
                {
                    dockContent.HideWindow();
                }
            }
            catch (Exception ex)
            {
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
        private void DockContentEnable(bool mapGL, bool console, bool testing, bool goalSetting)
        {
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
        private void DockContentVisible(ToolStripMenuItem item, bool visible)
        {
            if (mDockContent.ContainsKey(item))
            {
                DockContentVisible(mDockContent[item], visible);
                CtInvoke.ToolStripItemEnable(item, visible);
            }
        }

        /// <summary>
        /// 車子資訊更新事件
        /// </summary>
        ///<param name="info">車子資訊</param>
        private void CarInfoRefresh(CarInfo info)
        {
            int battery = info.PowerPercent;
            tsprgBattery.Value = battery;
            tslbBattery.Text = string.Format(tslbBattery.Tag.ToString(), battery);
            tslbStatus.Text = info.Status;
        }

        /// <summary>
        /// 依照使用者權限切換介面配置
        /// </summary>
        /// <param name="usrLv"></param>
        private void UserChanged(UserData usrData)
        {
            AccessLevel usrLv = usrData.Level;
            string title = string.Empty;//工具列選項標題
            string usrName = string.Empty;//狀態列帳號名稱
            bool allowUsrMan = usrLv < AccessLevel.OPERATOR;

            /*-- 依照權限切換模組可視層級 --*/
            switch (usrLv)
            {
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
            if (usrLv == AccessLevel.NONE)
            {
                title = "Login";
                usrName = "No account";
            }
            else
            {
                title = "Logout";
                usrName = usrData.Account;
            }
            CtInvoke.ToolStripItemText(miLogin, title);
            CtInvoke.ToolStripItemVisible(miUserManager, allowUsrMan);
            tslbAccessLv.Text = usrLv.ToString();
            tslbUserName.Text = usrName;
        }

        #endregion Function - Private Methods

        /// <summary>
        /// 設定事件連結
        /// </summary>
        private void SetEvents()
        {
            #region IGoalSetting 事件連結     
            IGoalSetting.AddNewGoalEvent += IGoalSetting_AddNewGoalEvent;
            IGoalSetting.ClearGoalsEvent += IGoalSetting_ClearGoalsEvent;
            IGoalSetting.DeleteGoalsEvent += IGoalSetting_DeleteGoalsEvent;
            IGoalSetting.FindPathEvent += IGoalSetting_FindPathEvent;
            IGoalSetting.LoadMapEvent += IGoalSetting_LoadMapEvent;
            IGoalSetting.LoadMapFromAGVEvent += IGoalSetting_LoadMapFromAGVEvent;
            IGoalSetting.RunGoalEvent += IGoalSetting_RunGoalEvent;
            IGoalSetting.RunLoopEvent += IGoalSetting_RunLoopEvent;
            IGoalSetting.SaveGoalEvent += IGoalSetting_SaveGoalEvent;
            IGoalSetting.SendMapToAGVEvent += IGoalSetting_SendMapToAGVEvent;
            IGoalSetting.GetGoalNames += IGoalSetting_GetGoalNames;
            #endregion

            #region IMapGL 事件連結
            IMapCtrl.DragEvent += IMapCtrl_DragEvent;
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
            ITest.CorrectOri += ITest_CorrectOri;
            ITest.SetVelocity += ITest_SetVelocity;
            ITest.Connect += ITest_CheckIsServerAlive;
            ITest.MotorServoOn += ITest_MotorServoOn;
            ITest.SimplifyOri += ITest_SimplifyOri;
            #endregion 
        }

        /// <summary>
        /// 取得所有Goal點名稱
        /// </summary>
        private void IGoalSetting_GetGoalNames()
        {
            string goalNames = GetGoalNames();
            IConsole.AddMsg($"Goal Names:{goalNames}");
        }

        private void ITest_SimplifyOri()
        {
            if (mBypassLoadFile)
            {
                /*-- 空跑模擬SimplifyOri --*/
                SpinWait.SpinUntil(() => false, 1000);
                return;
            }
            IMapCtrl.NewMap();
            string[] tmpPath = CurOriPath.Split('.');
            CurMapPath = tmpPath[0] + ".map";
            MapSimplication mapSimp = new MapSimplication(CurMapPath);
            mapSimp.Reset();
            IObstacleLines obstacleLines = Factory.FMuti.ObstacleLines();
            IObstaclePoints obstaclePoints = Factory.FMuti.ObstaclePoints();
            List<CartesianPos> resultPoints;
            List<MapSimplication.Line> resultlines;
            mapSimp.ReadMapAllTransferToLine(mMapMatch.parseMap, mMapMatch.minimumPos, mMapMatch.maximumPos
                , 100, 0, out resultlines, out resultPoints);
            try
            {
                for (int i = 0; i < resultlines.Count; i++)
                {
                    obstacleLines.DataList.Add(
                        Factory.FGeometry.Line(resultlines[i].startX, resultlines[i].startY,
                        resultlines[i].endX, resultlines[i].endY)
                    );
                }
                for (int i = 0; i < resultPoints.Count; i++)
                {
                    obstaclePoints.DataList.Add(Factory.FGeometry.Pair((int)resultPoints[i].x, (int)resultPoints[i].y));
                }

                Database.ObstaclePointsGM.Add(mObstaclePointsID, obstaclePoints);
                Database.ObstacleLinesGM.Add(mObstacleLinesID, obstacleLines);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }


            obstacleLines = null;
            obstaclePoints = null;
            resultPoints = null;
            resultlines = null;
        }

        #region ITest 事件連結


        private void ITest_MotorServoOn(bool servoOn)
        {
            ITest.ChangedMotorStt(servoOn);
            IConsole.AddMsg($"Motor Servo {(servoOn ? "ON" : "OFF")}");
        }



        /// <summary>
        /// 地圖修正
        /// </summary>
        private void CorrectOri()
        {
            IMapCtrl.NewMap();
            Database.ObstaclePointsGM.Add(mObstaclePointsID, Factory.FMuti.ObstaclePoints());
            tsk_FixOriginScanningFile();
            Database.LaserPointsGM.Remove(mLaserID);
        }

        /// <summary>
        /// 地圖修正執行緒
        /// </summary>
        /// <remarks>
        /// Modified by Jay 2017/09/13
        /// </remarks>
        private void tsk_FixOriginScanningFile()
        {
            try {
                if (mBypassLoadFile) {
                    SpinWait.SpinUntil(() => false, 1000);
                    return;
                }
                MapReading MapReading = new MapReading(CurOriPath);
                CartesianPos carPos;
                List<CartesianPos> laserData;
                List<CartesianPos> filterData = new List<CartesianPos>();
                int dataLength = MapReading.OpenFile();
                if (dataLength == 0) return;

                List<CartesianPos> dataSet = new List<CartesianPos>();
                List<CartesianPos> predataSet = new List<CartesianPos>();
                List<CartesianPos> matchSet = new List<CartesianPos>();
                CartesianPos transResult = new CartesianPos();
                CartesianPos nowOdometry = new CartesianPos();
                CartesianPos preOdometry = new CartesianPos();
                CartesianPos accumError = new CartesianPos();
                CartesianPos diffOdometry = new CartesianPos();
                CartesianPos diffLaser = new CartesianPos();
                Stopwatch sw = new Stopwatch();
                double gValue = 0;
                int mode = 0;
                int corrNum = 0;

                mMapMatch.Reset();
                #region  1.Read car position and first laser scanning

                MapReading.ReadScanningInfo(0, out carPos, out laserData);
                Database.AGVGM.Add(mAGVID, Factory.FSingle.FTowardPair.AGV((int)carPos.x, (int)carPos.y, carPos.theta, "AGV"));
                //mCarInfo.SetPos(carPos);
                //RaiseMapEventSync(MapEventType.RefreshPosCar, carPos.ToPos());
                matchSet.AddRange(laserData);
                predataSet.AddRange(laserData);
                mMapMatch.GlobalMapUpdate(matchSet);                            //Initial environment model
                preOdometry.SetPosition(carPos.x, carPos.y, carPos.theta);

                #endregion

                for (int n = 1; n < dataLength; n++) {
                    #region 2.Read car position and laser scanning 

                    List<CartesianPos> addedSet = new List<CartesianPos>();
                    transResult.SetPosition(0, 0, 0);
                    carPos = null;
                    laserData = null;
                    MapReading.ReadScanningInfo(n, out carPos, out laserData);
                    nowOdometry.SetPosition(carPos.x, carPos.y, carPos.theta);

                    #endregion

                    #region 3.Correct accumulate error of odometry so far

                    mMapMatch.NewPosTransformation(nowOdometry, accumError.x, accumError.y, accumError.theta);
                    mMapMatch.NewPosTransformation(laserData, accumError.x, accumError.y, accumError.theta);
                    matchSet.Clear();
                    matchSet.AddRange(laserData);

                    #endregion

                    #region 4.Compute movement from last time to current time;

                    if (nowOdometry.theta - preOdometry.theta < -200)
                        diffOdometry.SetPosition(nowOdometry.x - preOdometry.x, nowOdometry.y - preOdometry.y, nowOdometry.theta + 360 - preOdometry.theta);
                    else if (nowOdometry.theta - preOdometry.theta > 200)
                        diffOdometry.SetPosition(nowOdometry.x - preOdometry.x, nowOdometry.y - preOdometry.y, -(preOdometry.theta + 360 - nowOdometry.theta));
                    else
                        diffOdometry.SetPosition(nowOdometry.x - preOdometry.x, nowOdometry.y - preOdometry.y, nowOdometry.theta - preOdometry.theta);
                    System.Console.WriteLine("Odometry varition:{0:F3} {1:F3} {2:F3}", diffOdometry.x, diffOdometry.y, diffOdometry.theta);

                    #endregion

                    #region 5.Display current scanning information
                    //RaiseMapEventSync(MapEventType.RefreshScanPoint, matchSet);

                    #endregion

                    #region 6.Inspect odometry variation is not too large.Switch to pose tracking mode if too large.

                    sw.Restart();
                    if (Math.Abs(diffOdometry.x) >= 400 || Math.Abs(diffOdometry.y) >= 400 || Math.Abs(diffOdometry.theta) >= 30) {
                        mode = 1;
                        gValue = mMapMatch.PairwiseMatching(predataSet, matchSet, 4, 1.5, 0.01, 20, 300, false, transResult);
                    } else {
                        mode = 0;
                        gValue = mMapMatch.FindClosetMatching(matchSet, 4, 1.5, 0.01, 20, 300, false, transResult);
                        diffLaser.SetPosition(transResult.x, transResult.y, transResult.theta);
                    }

                    //If corresponding is too less,truct the odomery variation this time
                    if (mMapMatch.EstimateCorresponingPoints(matchSet, 10, 10, out corrNum, out addedSet)) {
                        mMapMatch.NewPosTransformation(nowOdometry, transResult.x, transResult.y, transResult.theta);
                        accumError.SetPosition(accumError.x + transResult.x, accumError.y + transResult.y, accumError.theta + transResult.theta);
                    }
                    sw.Stop();

                    if (mode == 0)
                        System.Console.WriteLine("[SLAM-Matching Mode]Corresponding Points:{0} Map Size:{1} Matching Time:{2:F3} Error{3:F3}",
                             corrNum, mMapMatch.parseMap.Count, sw.Elapsed.TotalMilliseconds, gValue);
                    else
                        System.Console.WriteLine("[SLAM-Tracking Mode]Matching Time:{0:F3} Error{1:F3}", sw.Elapsed.TotalMilliseconds, gValue);

                    #endregion

                    #region 7.Update variation

                    //Pairwise update
                    predataSet.Clear();
                    predataSet.AddRange(laserData);

                    //Update previous variable
                    preOdometry.SetPosition(nowOdometry.x, nowOdometry.y, nowOdometry.theta);

                    Database.AGVGM.Add(mAGVID, Factory.FSingle.FTowardPair.AGV((int)nowOdometry.x, (int)nowOdometry.y, nowOdometry.theta, "AGV"));
                    //mCarInfo.SetPos(nowOdometry);

                    #endregion


                    ILaserPoints points = ConvertToLaserPoints(addedSet);
                    Database.LaserPointsGM.Add(mLaserID, points);
                    Database.ObstaclePointsGM.SaftyEdit(mObstaclePointsID, (item) => item.DataList.AddRange(points.DataList.AsReadOnly()));
                    //IMapCtrl.Focus((int)nowOdometry.x, (int)nowOdometry.y);
                    //Display added new points                     
                    //    RaiseMapEventSync(MapEventType.DrawScanMap, addedSet);

                    //    //Display car position
                    //    RaiseMapEventSync(MapEventType.RefreshPosCar, nowOdometry.ToPos());
                }
                //RaiseMapEvent(MapEventType.CorrectOriComplete);
            } catch {

            } finally {
            }
        }
    

        private async void ITest_CheckIsServerAlive(bool cnn, string hostIP = "")
        {
            if (cnn != IsConnected)
            {
                CtProgress prog = new CtProgress("Connect", "Connecting...");
                try
                {
                    if (cnn)
                    {
                        if (VerifyIP(hostIP))
                        {
                            if (mHostIP != hostIP) mHostIP = hostIP;
                            IsConnected = await Task<bool>.Run(() => CheckIsServerAlive());
                        }
                    }
                    else
                    {
                        IsConnected = DisConnectServer();
                    }
                    ITest.SetServerStt(IsConnected);
                    IConsole.AddMsg($"Is {(cnn ? "Connected" : "Disconnected")}");
                }
                finally
                {
                    prog?.Close();
                    prog = null;
                }
            }
        }

        /// <summary>
        /// 檢查Server是否在運作中
        /// </summary>
        /// <returns></returns>
        private bool CheckIsServerAlive()
        {
            if (mBypassSocket)
            {
                IsServerAlive = true;
                Thread.Sleep(1000);
            }
            else
            {
                bool isAlive = false;
                try
                {
                    if (isAlive = ConnectServer())
                    {
                        string[] rtnMsg = SendMsg("Get:Hello", false);
                        isAlive = rtnMsg.Count() > 2 && rtnMsg[2] == "True";
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"[SocketException] : {ex.Message}");
                }
                finally
                {
                    if (!mBypassSocket && !isAlive)
                    {
                        CtMsgBox.Show("Failed", "Connect Failed!!", MsgBoxButton.OK, MsgBoxStyle.ERROR);
                    }
                }
                IsServerAlive = isAlive;
            }
            return IsServerAlive;
        }


        private void ITest_SetVelocity(int velocity)
        {
            IConsole.AddMsg($"Set Velocity {velocity}");
            SendMsg($"Set: WorkVelo:{velocity}:{velocity}");
        }

        private async void ITest_CorrectOri()
        {
            CtProgress prog = new CtProgress("CorrectOri", "Correcting Ori...");
            try
            {
                /*-- 底層觸發事件 --*/
                await Task.Run(() => CorrectOri());
                IConsole.AddMsg("CorrectOri");
            }
            catch
            {
            }
            finally
            {
                prog?.Close();
                prog = null;
            }
        }

        private void ITest_SetCarMode(CarMode mode)
        {
            if (mode == CarMode.Map)
            {
                string oriName = string.Empty;
                CtInput txtBox = new CtInput();
                if (Stat.SUCCESS == txtBox.Start(
                    CtInput.InputStyle.TEXT,
                    "Set Map File Name", "MAP Name",
                    out oriName,
                    $"MAP{DateTime.Today:MMdd}"))
                {
                    SendMsg($"Set:OriName:{oriName}.Ori");
                }
                else
                {
                    return;
                }
            }
            if (mode != CarMode.OffLine)
            {
                SendMsg($"Set:Mode:{mode}");
                IConsole.AddMsg($"Set:Mode {mode}");
            }
            if (mode != mCarMode)
            {
                mode = mCarMode;
                ChangedMode(mCarMode);
            }
        }

        private void ITest_SendMap()
        {
            SendFile();
        }

        private void ITest_GetCar()
        {
            ChangeSendInfo();
            IConsole.AddMsg($"{IsGettingLaser}");
        }

        private void ITest_GetLaser()
        {
            IConsole.AddMsg("Get Laser");
            GetLaser();
        }

        private void ITest_GetMap()
        {
            IConsole.AddMsg("[Get Map]");
            GetFile(FileType.Map);
        }

        private void ITest_GetORi()
        {
            IConsole.AddMsg("[Get Ori]");
            GetFile(FileType.Ori);
        }

        private void ITest_LoadMap()
        {
            IConsole.AddMsg("[Loaded Map]");
            LoadFile(FileType.Map);
        }

        private void ITest_LoadOri()
        {
            IConsole.AddMsg("[Loaded Ori]");
            LoadFile(FileType.Ori);
        }

        private void ITest_Motion_Up()
        {
            IConsole.AddMsg($"[Stop]");
            MotionContorl(MotionDirection.Stop);
            if (CarMode != CarMode.Map) CarMode = CarMode.Idle;
        }

        private void ITest_Motion_Down(MotionDirection direction, int velocity = 0)
        {
            IConsole.AddMsg($"[{direction} Velocity:{velocity}]");
            MotionContorl(direction, Velocity);
            if (CarMode != CarMode.Map) CarMode = CarMode.Work;
        }


        #endregion

        #region IMapGL事件連結


        private void IMapCtrl_DragEvent(ISingle<ITowardPair> single,uint id)
        {
            if (single.GLSetting.Type == EType.Goal)
            {
                GoalSetting.AddGoal(new CartesianPosInfo(single.Data.Position.X, single.Data.Position.Y, single.Data.Toward.Theta,single.Name,id));
            }
        }

        #endregion IMapGL 事件連結

        #region IGoalSetting 事件連結   
        private void IGoalSetting_SendMapToAGVEvent()
        {
            IConsole.AddMsg("[Map Update...]");
            IConsole.AddMsg("[Map Finished Update]");
        }

        private void IGoalSetting_SaveGoalEvent()
        {
            IConsole.AddMsg("[Save {0} Goals]", IGoalSetting.GoalCount);
            List<CartesianPos> goals = IGoalSetting.GetGoals().ConvertAll(v =>
                new CartesianPos(v.x, v.y, v.theta)
                );
            MapRecording.OverWriteGoal(goals, CurMapPath);
        }

        private void IGoalSetting_RunLoopEvent(IEnumerable<CartesianPosInfo> goal)
        {
            IConsole.AddMsg("[AGV Start Moving...]");
            foreach (var item in goal)
            {
                IConsole.AddMsg("[AGV Move To] - {0}", item.ToString());
                IConsole.AddMsg("[AGV Arrived] - {0}", item.ToString());
            }
            IConsole.AddMsg("[AGV Move Finished]");
        }

        private void IGoalSetting_RunGoalEvent(CartesianPosInfo goal, int idxGoal)
        {
            Run(idxGoal);
            IConsole.AddMsg("[AGV Start Moving...]");
            IConsole.AddMsg("[AGV Arrived] - {0}", goal.ToString());
        }

        private void IGoalSetting_LoadMapFromAGVEvent()
        {
            IConsole.AddMsg("[Map Loading... From Remote] - ");
            IConsole.AddMsg("[Map Loaded]");
        }

        private void IGoalSetting_LoadMapEvent()
        {
            IConsole.AddMsg("[Map Loading...]");
            IConsole.AddMsg("[Map Loaded]");
            LoadFile(FileType.Map);
        }

        private void IGoalSetting_FindPathEvent(CartesianPosInfo goal, int idxGoal)
        {
            IConsole.AddMsg("[Find Path] - ", goal.ToString());
            IConsole.AddMsg("[AGV Find A Path]");
            PathPlan(idxGoal);
        }

        private void IGoalSetting_DeleteGoalsEvent(IEnumerable<CartesianPosInfo> goal)
        {
            IConsole.AddMsg("[Delete {0} Goals]", goal.Count());
            List<uint> ID = new List<uint>();
            foreach (var item in goal)
            {
                Database.GoalGM.Remove(item.id);
                ID.Add(item.id);
                IConsole.AddMsg("[Delete Goal] - {0}", item.ToString());
            }
            IGoalSetting.DeleteGoals(ID);
        }

        private void IGoalSetting_ClearGoalsEvent()
        {
            IConsole.AddMsg("[Clear Goal]");
            Database.GoalGM.Clear();
            IGoalSetting.ClearGoal();
        }

        private void IGoalSetting_AddNewGoalEvent(CartesianPosInfo goal)
        {
            IConsole.AddMsg("[Add Goal] - {0}", goal.ToString());
            IGoalSetting.AddGoal(goal);
            Database.GoalGM.Add(goal.id, Factory.FSingle.FTowardPair.Goal((int)goal.x, (int)goal.y, goal.theta, goal.name));
        }
        #endregion

        private void miBypassSocket_Click(object sender, EventArgs e)
        {
            IsBypassSocket = !IsBypassSocket;
            CtInvoke.ToolStripItemChecked(miBypassSocket, IsBypassSocket);
        }

        private void miLoadFile_Click(object sender, EventArgs e)
        {
            IsBypassLoadFile = !IsBypassLoadFile;
            CtInvoke.ToolStripItemChecked(miLoadFile, IsBypassLoadFile);
        }
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

    #endregion Support - Class

}
