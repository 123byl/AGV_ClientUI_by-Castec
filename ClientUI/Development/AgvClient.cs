using System;
using System.Collections.Generic;
using System.Linq;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

using CtLib.Library;

using MapProcessing;
using static MapProcessing.MapSimplication;

namespace ClientUI
{
    #region Declaration Extenstion

    /// <summary>
    /// 擴充方法定義
    /// </summary>
    internal static class AgvExtenstion {

        /// <summary>擴充 <see cref="Control"/>，使用 <seealso cref="Control.Invoke(Delegate)"/> 方法執行不具任何簽章之委派</summary>
        /// <param name="ctrl">欲調用的控制項</param>
        /// <param name="action">欲執行的方法</param>
        public static void InvokeIfNecessary(this Control ctrl, MethodInvoker action) {
            if (ctrl.InvokeRequired) ctrl.Invoke(action);
            else action();
        }

        internal static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> src,int size) {
            return src.Select((x, i) => new { Index = i,value = x }).
                GroupBy(x => x.Index / size).
                Select(x => x.Select(v => v.value));
        }

        /// <summary>
        /// 回傳字串表示點位資訊，格式x,y,theta
        /// </summary>
        /// <param name="point">要轉換的點位</param>
        /// <returns></returns>
        public static string ToStr(this CartesianPos point) {
            return $"{point.x:F0},{point.y:F0},{point.theta:F0}";
        }

    }

    #endregion Declaration  - Extenstion

    #region Declaration - Events

    /// <summary>
    /// AGV客戶端事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void AgvClientEvent(object sender, AgvClientEventArgs e);
    
    /// <summary>
    /// AGV客戶端事件參數
    /// </summary>
    public class AgvClientEventArgs : EventArgs {
        /// <summary>
        /// AGV客戶端事件類型
        /// </summary>
        public AgvClientEventType Type { get; }

        /// <summary>
        /// 傳遞事件參數
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// 一般建構方法
        /// </summary>
        /// <param name="type">AGV客戶端事件類型</param>
        /// <param name="value">事件參數</param>
        public AgvClientEventArgs(AgvClientEventType type,object value) {
            this.Type = type;
            this.Value = value;
        }
    }

    /// <summary>
    /// AGV客戶端事件類型
    /// </summary>
    public enum AgvClientEventType {
        /// <summary>
        /// 車子資訊更新
        /// </summary>
        CarInfoRefresh,
        /// <summary>
        /// 使用者變更
        /// </summary>
        UserChanged,
        /// <summary>
        /// 取得雷射
        /// </summary>
        GetLaser,
        /// <summary>
        /// 模式切換
        /// </summary>
        Mode,
        /// <summary>
        /// 檔案傳送
        /// </summary>
        SendFile,
        /// <summary>
        /// 載入檔案
        /// </summary>
        LoadFile,
        /// <summary>
        /// 從AGV取得檔案
        /// </summary>
        GetFile
    }

    #endregion Declaration - Events

    #region Declaration - Enum

    /// <summary>
    /// 車子模式
    /// </summary>
    public enum CarMode:byte {
        /// <summary>
        /// 閒置
        /// </summary>
        Idle,
        /// <summary>
        /// 工作
        /// </summary>
        Work,
        /// <summary>
        /// 掃圖
        /// </summary>
        Map,
        /// <summary>
        /// 離線
        /// </summary>
        OffLine = 255
    }
    
    /// <summary>
    /// 檔案類型
    /// </summary>
    public enum FileType {
        Ori,
        Map,
    }

    #endregion Declaration - Enum

    #region Declaration - Core

    /// <summary>
    /// Agv功能類
    /// </summary>
  //  public class AgvClient : ICtVersion {

  //      #region Version Information

  //      /// <summary>
  //      /// AgvClient版本資訊
  //      /// </summary>
  //      /// <remarks>
  //      ///     0.0.0   Jay [2017/09/05]
  //      ///         + 將GUI重構而來
  //      ///         + 導入DockPanel
  //      ///         + 導入系統列縮圖
  //      ///     0.0.1   Jay [2017/09/06]
  //      ///         + 建立版本註冊機制，在"About"視窗中顯示透過RegisterVersion註冊的模組版本資訊
  //      ///         + 導入使用者權限機制
  //      ///         \ 修改狀態欄，左邊顯示帳戶資訊，右邊顯示車子電量資訊
  //      ///         \ 依照權限開放模組功能
  //      ///     0.0.2   Jay [2017/09/07]
  //      ///         \ 將事件依照模組分類統整
  //      ///         \ CtDockContent<T>重構，加入空白建構方法，參考分配方法
  //      ///         \ 增加定義ICtDockContent屬性與方法，統一在ICtDockContent形態下進行操作
  //      ///         \ 加入文字方塊驗證數值機制
  //      ///         \ 優化CtDockContent<T>方法，自行判斷是否需要Invoke
  //      ///     0.0.3   Jay [2017/09/08]
  //      ///         \ 獨立成單一專案
  //      ///     0.0.4   Jay [2017/09/11]
  //      ///         \ 整合20170906_V2版本Testing功能
  //      ///     0.0.5   Jay [2017/09/12]
  //      ///         \ 移植至20170911版本的方案中
  //      ///         \ 整合GoalSetting部分功能
  //      ///         + 加入Opc讀取機制
  //      ///     0.0.6   Jay [2017/09/13]
  //      ///         \ 事件傳遞引數機制重構，變數定義寫入靜態類VarDef，複數個引數以Dicsionary<string,object>進行包裝
  //      ///         \ 將地圖操作相關執行緒運算部分抽離移至在核心類AgvClient
  //      ///     0.0.7   Jay [2017/09/18]
  //      ///         \ 將部分較耗時的方法以非同步方法進行重構，加入進度條、氣球提示增加操作反饋
  //      ///         \ CtProgress的Close機制修改，以切Flag方式來結束執行緒
  //      ///         \ 移動控制與命令格式修正
  //      ///     0,0,8   Jay [2017/09/20]
  //      ///         \ 移植到20170919版本方案中
  //      ///         \ 命令交握修改
  //      ///     0.0.9   Jay [2017/09/25]
  //      ///         \ GoalSetting介面中DataGridView與ComboBox順序統一
  //      ///         \ LoadMap後DataGridView補上編號
  //      ///         \ 將專案中所使用到的Dll加入專案當中
  //      /// </remarks>
  //      public CtVersion Version { get { return new CtVersion(0, 0, 10, "2017/09/26", "Jay Chang"); } }

  //      #endregion Version Information

  //      #region Declaration - Support Class



  //      #endregion Declaration - Support Class

  //      #region Declaration - Fields

  //      /// <summary>
  //      /// 當前Map檔路徑
  //      /// </summary>
  //      private string mCurMapPath = string.Empty;

  //      /// <summary>
  //      /// 是否已建立連線
  //      /// </summary>
  //      private bool mIsConnected = false;

  //      /// <summary>
  //      /// 進度條物件
  //      /// </summary>
  //      private CtProgress mProg = null;

  //      /// <summary>Opcode 檔案名稱</summary>
  //      private static readonly string FILENAME_OPCODE = "D1703.opc";

  //      /// <summary>CtOpcode Object</summary>
  //      private CtOpcode mOpcode = new CtOpcode();

  //      /// <summary>
  //      /// Server端IP
  //      /// </summary>
  //      private static string mHostIP = "127.0.0.1";

  //      /// <summary>
  //      /// 發送圖片的埠
  //      /// </summary>
  //      private static int mFilePort = 600;

  //      /// <summary>
  //      /// 接收請求的埠開啟後就一直進行偵聽
  //      /// </summary>
  //      private static int mRecvCmdPort = 400;

  //      /// <summary>
  //      /// 接收請求的埠開啟後就一直進行偵聽
  //      /// </summary>
  //      private static int mCmdPort = 800;

  //      /// <summary>
  //      /// 發送地圖的埠
  //      /// </summary>
  //      private static int mSendMapPort = 700;

  //      /// <summary>
  //      /// 路徑規劃接收埠
  //      /// </summary>
  //      private static int mRecvPathPort = 900;

  //      /// <summary>
  //      /// 地圖檔儲存路徑
  //      /// </summary>
  //      public string mDefMapDir = @"D:\MapInfo\";

  //      /// <summary>
  //      /// 車子馬達轉速
  //      /// </summary>
  //      private int mVelocity = 500;

  //      /// <summary>
  //      /// 命令接收物件
  //      /// </summary>
  //      private SocketMonitor mSoxMonitorCmd = null;

  //      /// <summary>
  //      /// 地圖資料接收物件
  //      /// </summary>
  //      private SocketMonitor mSoxMonitorFile = null;

  //      /// <summary>
  //      /// 路徑規劃接收物件
  //      /// </summary>
  //      private SocketMonitor mSoxMonitorPath = null;

  //      /// <summary>
  //      /// 地圖操作執行緒
  //      /// </summary>
  //      private Thread mTdMapOperation = null;

  //      /// <summary>
  //      /// Map檔載入執行緒
  //      /// </summary>
  //      private Thread mTdLoadMap = null;

  //      /// <summary>
  //      /// 地圖載入執行緒
  //      /// </summary>
  //      private Thread mLoadOriginScanning = null;

  //      /// <summary>
  //      /// 偵測多餘的呼叫
  //      /// </summary>
  //      private bool disposedValue = false;

  //      /// <summary>
  //      /// 模組版本集合
  //      /// </summary>
  //      private Dictionary<string, string> mModuleVersions = new Dictionary<string, string>();

  //      /// <summary>
  //      /// 使用者操作權限
  //      /// </summary>
  //      private UserData mUser = new UserData("CASTEC", "", AccessLevel.Administrator);

  //      /// <summary>
  //      /// 當前語系
  //      /// </summary>
  //      /// <remarks>
  //      /// 未來開發多語系用
  //      /// </remarks>
  //      private UILanguage mCulture = UILanguage.ENGLISH;

  //      /// <summary>
  //      /// Socket通訊物件
  //      /// </summary>
  //      private Communication serverComm = new Communication(400, 600, 800);

  //      /// <summary>
  //      /// 車子資訊
  //      /// </summary>
  //      private CarInfo mCarInfo = new CarInfo();

  //      /// <summary>
  //      /// 車子模式
  //      /// </summary>
  //      private CarMode mCarMode = CarMode.OffLine;

  //      private MapMatching mMapMatch = new MapMatching();

  //      /// <summary>
  //      /// 是否Bypass Socket通訊
  //      /// </summary>
  //      private bool mBypassSocket = false;

  //      /// <summary>
  //      /// 是否Bypass LoadFile功能
  //      /// </summary>
  //      private bool mBypassLoadFile = false;

  //      /// <summary>
  //      /// 是否Bypass Server
  //      /// </summary>
  //      private bool mByPassServer = false;

  //      #endregion Declaration - Fields

  //      #region Declaration - Properties

  //      /// <summary>
  //      /// 使用者資料
  //      /// </summary>
  //      public UserData UserData { get { return mUser; } }

  //      /// <summary>
  //      /// 目標設備IP
  //      /// </summary>
  //      public string HostIP { get { return mHostIP; } set { mHostIP = value; } }

  //      /// <summary>
  //      /// 是否Bypass Socket功能
  //      /// </summary>
  //      public bool IsBypassSocket { get { return mBypassSocket; } set { mBypassSocket = value; } }

  //      /// <summary>
  //      /// 是否Bypass LoadFile功能
  //      /// </summary>
  //      public bool IsBypassLoadFile { get { return mBypassLoadFile; } set { mBypassLoadFile = value; } }

  //      /// <summary>
  //      /// 使用者操作權限
  //      /// </summary>
  //      public AccessLevel UserLv { get { return mUser.Level; } }

  //      /// <summary>
  //      /// 當前語系
  //      /// </summary>
  //      /// <remarks>
  //      /// 未來開發多語系的時候使用
  //      /// </remarks>
  //      public UILanguage Culture { get { return mCulture; } }

  //      /// <summary>
  //      /// 是否持續接收雷射資料
  //      /// </summary>
  //      public bool IsGettingLaser { get; set; } = false;

  //      /// <summary>
  //      /// 車子模式
  //      /// </summary>
  //      public CarMode CarMode {
  //          get {
  //              return mCarMode;
  //          }
  //          set {
  //              if (value == CarMode.Map) {
  //                  string oriName = string.Empty;
  //                  CtInput txtBox = new CtInput();
  //                  if (Stat.SUCCESS == txtBox.Start(
  //                      CtInput.InputStyle.TEXT,
  //                      "Set Map File Name", "MAP Name",
  //                      out oriName,
  //                      $"MAP{DateTime.Today:MMdd}")) {
  //                      SendMsg($"Set:OriName:{oriName}.Ori");
  //                  } else {
  //                      return;
  //                  }
  //              }
  //              mCarMode = value;
  //              SendMsg($"Set:Mode:{mCarMode}");
  //              RaiseAgvClientEvent(AgvClientEventType.Mode, mCarMode);
  //          }
  //      }

  //      /// <summary>
  //      /// 要新增的點位
  //      /// </summary>

  //      #endregion Declaration - Properties

  //      #region Declaration - Events

  //      /// <summary>
  //      /// AGV客戶端事件觸發
  //      /// </summary>
  //      public event AgvClientEvent AgvClientEventTrigger;

  //      #endregion Declaration - Events

  //      #region Function - Constructors

  //      /// <summary>
  //      /// 共用建構方法
  //      /// </summary>
  //      public AgvClient() {
  //          RegisterVersion(this.GetType().Name, Version.FullString);
  //          StartUp();
  //      }


  //      #endregion Function - Constructors

  //      #region Funciton - Public Methods



  //      #endregion Funciton - Public Methods

  //      #region Implement 

  //      #region IAgvClient

  //      /// <summary>
  //      /// 是否Bypass Server
  //      /// </summary>
  //      public bool IsBypassServer { get { return mByPassServer; } set { mByPassServer = value; BypassServer(value); } }

  //      /// <summary>
  //      /// Goal點集合
  //      /// </summary>
  //      public List<CartesianPos> Goals { get; set; } = new List<CartesianPos>();

  //      /// <summary>
  //      /// 預設地圖檔資料夾路徑
  //      /// </summary>
  //      public string DefMapDir {
  //          get {
  //              return mDefMapDir;
  //          }
  //      }

  //      /// <summary>
  //      /// 關於視窗
  //      /// </summary>
  //      public void form_About() {
  //          using (CtAbout frm = new CtAbout()) {

  //              //新版本CtLib
  //              //frm.Start(Assembly.GetExecutingAssembly(), this, Version, module);
  //              //當前版本CtLib
  //              frm.Start(Version, mModuleVersions);
  //          }
  //      }

  //      /// <summary>
  //      /// 註冊模組版本
  //      /// </summary>
  //      /// <param name="modName">模組名稱</param>
  //      /// <param name="modVer">模組版本</param>
  //      public void RegisterVersion(string modNmae, string modVer) {
  //          if (!mModuleVersions.ContainsKey(modNmae)) {
  //              mModuleVersions.Add(modNmae, modVer);
  //          }
  //      }

  //      /// <summary>
  //      /// 依照當前權限決定登入或登出
  //      /// </summary>
  //      public void Login() {
  //          Stat stt = Stat.SUCCESS;
  //          if (mUser.Level == AccessLevel.None) {
  //              using (CtLogin frmLogin = new CtLogin()) {
  //                  stt = frmLogin.Start(out mUser);
  //              }
  //          } else {
  //              mUser = new UserData("N/A", "", AccessLevel.None);
  //          }
  //          RaiseUserChanged(mUser);
  //      }

  //      /// <summary>
  //      /// 使用者管理視窗
  //      /// </summary>
  //      public void form_UserManager() {
  //          //新版本CtLib
  //          //using (CtUserManager frmUsrMgr = new CtUserManager(mUser, mCulture)) {
  //          //當前版本CtLib
  //          using (CtUserManager frmUsrMgr = new CtUserManager(mUser)) {
  //              frmUsrMgr.ShowDialog();
  //          }
  //      }

  //      #endregion IAgvClient

  //      #region IMapGL

  //      /// <summary>
  //      /// GL模式
  //      /// </summary>
  //      public GLMode GLMode { get; set; }

  //      /// <summary>
  //      /// Ori檔路徑
  //      /// </summary>
  //      public string CurOriPath { get; set; } = string.Empty;

  //      /// <summary>
  //      /// Map檔路徑
  //      /// </summary>
  //      public string CurMapPath {
  //          get {
  //              return mCurMapPath;
  //          }
  //          set {
  //              mCurMapPath = value;
  //              RaiseGoalSettingEvent(GoalSettingEventType.CurMapPath, !string.IsNullOrEmpty(value));
  //          }
  //      }

  //      /// <summary>
  //      /// 車子資訊
  //      /// </summary>
  //      public CarInfo CarInfo { get { return mCarInfo; } set { mCarInfo = value; } }

  //      public List<CarPos> PtCar { get; set; } = new List<CarPos>();

  //      public List<string> StrCar { get; set; } = new List<string>();

  //      /// <summary>
  //      /// 地圖相關事件
  //      /// </summary>
  //      public event MapEvent MapEventTrigger;

  //      /// <summary>
  //      /// 設定位置
  //      /// </summary>
  //      /// <param name="x">X座標</param>
  //      /// <param name="y">Y座標</param>
  //      /// <param name="theta">夾角</param>
  //      public void SetPosition(double x, double y, double theta) {
  //          mCarInfo.X = x;
  //          mCarInfo.Y = y;
  //          mCarInfo.ThetaGyro = theta;
  //          SendMsg($"Set:POS:{x:F0}:{y:F0}:{theta}");
  //      }

  //      /// <summary>
  //      /// 設定要新增的點位(會觸發GoalSetting事件)
  //      /// </summary>
  //      /// <param name="x">X座標</param>
  //      /// <param name="y">Y座標</param>
  //      public void SetAddPos(double x, double y) {
  //          AddPos.x = x;
  //          AddPos.y = y;
  //          RaiseGoalSettingEvent(GoalSettingEventType.RefreshAddPos, AddPos);
  //      }

  //      #endregion IMapGL

  //      #region IGoalSetting

  //      /// <summary>
  //      /// 要新增的點位
  //      /// </summary>
  //      public CarPos AddPos { get; set; } = new CarPos();

  //      /// <summary>
  //      /// Goal Setting事件
  //      /// </summary>
  //      public event GoalSettingEvent GoalSettingEvent;

  //      /// <summary>
  //      /// Goal Setting事件發報
  //      /// </summary>
  //      /// <param name="type">事件類型</param>
  //      /// <param name="value">傳遞參數</param>
  //      private void RaiseGoalSettingEvent(GoalSettingEventType type, object value = null) {
  //          GoalSettingEvent?.BeginInvoke(this, new GoalSettingEventArgs(type, value), null, null);
  //      }

  //      /// <summary>
  //      /// 新增Goal點
  //      /// </summary>
  //      /// <param name="goal">Goal點</param>
  //      public void AddGoalPos(CarPos goal) {
  //          Goals.Add(new CartesianPos(goal.x, goal.y, goal.theta));
  //          RaiseMapEvent(MapEventType.AddGoalPos, goal);
  //      }

  //      /// <summary>
  //      /// 移除指定索引Goal點
  //      /// </summary>
  //      /// <param name="index">要移除的Goal點索引</param>
  //      public void DeleteGoal(int index) {
  //          Goals.RemoveAt(index);
  //      }

  //      /// <summary>
  //      /// 路徑規劃
  //      /// </summary>
  //      /// <param name="no">目標Goal點編號</param>
  //      public void PathPlan(int numGoal) {
  //          /*-- 若是路徑資料則開始接收資料 --*/
  //          string[] rtnMsg = SendMsg($"Set:PathPlan:{numGoal}");
  //          if ((rtnMsg?.Count() ?? 0) > 3 &&
  //              rtnMsg[1] == "PathPlan" &&
  //              rtnMsg[2] == "True") {
  //              mSoxMonitorPath.Start();
  //          }
  //      }

  //      /// <summary>
  //      /// 前往目標Goal點
  //      /// </summary>
  //      /// <param name="numGoal">目標Goal點</param>
  //      public void Run(int numGoal) {
  //          /*-- 若是路徑資料則開始接收資料 --*/
  //          string[] rtnMsg = SendMsg($"Set:Run:{numGoal}");
  //          if ((rtnMsg?.Length ?? 0) > 3 &&
  //              rtnMsg[1] == "Run" &&
  //              rtnMsg[3] == "Done") {
  //              mSoxMonitorPath.Start();
  //          }
  //      }

  //      /// <summary>
  //      /// 清除所有Goal點
  //      /// </summary>
  //      public void DeleteAllGoal() {
  //          Goals.Clear();
  //          RaiseMapEvent(MapEventType.DeleteAllGoal);
  //      }

  //      /// <summary>
  //      /// 儲存所有Goal點
  //      /// </summary>
  //      public void SaveGoals() {
  //          MapRecording.OverWriteGoal(Goals, CurMapPath);
  //      }

  //      #endregion IGoalSetting

  //      #region ITesting

  //      /// <summary>
  //      /// 取得是否為停止模式
  //      /// </summary>
  //      public bool GetStopMode() {
  //          string[] rtnMsg = SendMsg("Get:StopMode");
  //          return bool.Parse(rtnMsg[2]);
  //      }

  //      /// <summary>
  //      /// 取得伺服馬達激磁狀態
  //      /// </summary>
  //      /// <returns></returns>
  //      public bool GetMotorStatus() {
  //          string[] rtnMsg = SendMsg("Get:Info");
  //          return rtnMsg.Count() > 1 && rtnMsg[2] == "True";
  //      }

  //      /// <summary>
  //      /// 設定車子移動速度
  //      /// </summary>
  //      /// <param name="velocity">移動速度</param>
  //      public void SetVelocity(int velocity) {
  //          mVelocity = velocity;
  //          SendMsg($"Set:WorkVelo:{velocity}:{Velocity}");
  //      }

  //      /// <summary>
  //      /// 伺服端是否還有在運作
  //      /// </summary>
  //      public bool IsServerAlive { get; private set; }

  //      /// <summary>
  //      /// 是否已建立連線
  //      /// </summary>
  //      public bool IsConnected {
  //          get {
  //              return mIsConnected;
  //          }
  //          set {
  //              mIsConnected = value;
  //              RaiseGoalSettingEvent(GoalSettingEventType.Connect, value);
  //          }
  //      }

  //      /// <summary>
  //      /// Testing相關事件
  //      /// </summary>
  //      public event TestingEvent TestingEventTrigger;

  //      /// <summary>
  //      /// 向Server端要求檔案
  //      /// </summary>
  //      /// <param name="type">檔案類型</param>
  //      /// <remarks>modified by Jay 2017/09/20</remarks>
  //      public bool GetFileList(FileType type, out string fileList) {
  //          bool ret = true;
  //          fileList = string.Empty;
  //          if (mBypassSocket) {
  //              fileList = $"{type}1,{type}2,{type}3";
  //          } else {
  //              string[] rtnMsg = SendMsg($"Get:{type}List");
  //              fileList = rtnMsg[3];
  //          }
  //          return ret;
  //      }

  //      /// <summary>
  //      /// 檔案下載
  //      /// </summary>
  //      /// <param name="fileName"></param>
  //      /// <param name="type"></param>
  //      public void FileDownload(string fileName, FileType type) {
  //          /*-- 開啟執行緒準備接收檔案 --*/
  //          mSoxMonitorFile.Start();

  //          /*-- 向Server端發出檔案請求 --*/
  //          SendMsg($"Get:{type}:{fileName}");
  //          if (type == FileType.Map) {
  //              RaiseGoalSettingEvent(GoalSettingEventType.CurMapPath, true);
  //          } else {
  //              RaiseTestingEvent(TestingEventType.CurOriPath, true);
  //          }
  //          RaiseAgvClientEvent(AgvClientEventType.GetFile, type);
  //      }

  //      /// <summary>
  //      /// 地圖簡化
  //      /// </summary>
  //      public void SimplifyOri() {
  //          if (mBypassLoadFile) {
  //              /*-- 空跑模擬SimplifyOri --*/
  //              SpinWait.SpinUntil(() => false, 1000);
  //              return;
  //          }

  //          string[] tmpPath = CurOriPath.Split('.');
  //          CurMapPath = tmpPath[0] + ".map";
  //          MapSimplication mapSimp = new MapSimplication(CurMapPath);
  //          mapSimp.Reset();
  //          List<Line> obstacleLines = new List<Line>();
  //          List<Point> obstaclePoints = new List<Point>();
  //          List<CartesianPos> resultPoints;
  //          List<MapSimplication.Line> resultlines;
  //          mapSimp.ReadMapAllTransferToLine(mMapMatch.parseMap, mMapMatch.minimumPos, mMapMatch.maximumPos
  //              , 100, 0, out resultlines, out resultPoints);

  //          for (int i = 0; i < resultlines.Count; i++) {
  //              obstacleLines.Add(
  //                  new Line(resultlines[i].startX, resultlines[i].startY,
  //                  resultlines[i].endX, resultlines[i].endY)
  //              );
  //          }
  //          for (int i = 0; i < resultPoints.Count; i++) {
  //              obstaclePoints.Add(new Point((int)resultPoints[i].x, (int)resultPoints[i].y));
  //          }
  //          Dictionary<string, object> dic = new Dictionary<string, object>();
  //          dic.Add(obstacleLines, VarDef.ObstacleLines);
  //          dic.Add(obstaclePoints, VarDef.ObstaclePoints);

  //          RaiseMapEvent(MapEventType.SimplifyOri, dic);

  //          obstacleLines = null;
  //          obstaclePoints = null;
  //          resultPoints = null;
  //          resultlines = null;
  //      }

  //      /// <summary>
  //      /// 傳送檔案
  //      /// </summary>
  //      public async void SendMap() {
  //          string[] rtnMsg = SendMsg("Send:map");
  //          if (rtnMsg.Count() > 2 && "True" == rtnMsg[2]) {
  //              OpenFileDialog openMap = new OpenFileDialog();
  //              openMap.InitialDirectory = mDefMapDir;
  //              openMap.Filter = "MAP|*.ori;*.map";
  //              if (openMap.ShowDialog() == DialogResult.OK) {
  //                  CtProgress prog = new CtProgress("Send Map", "The file are being transferred");
  //                  try {

  //                      await Task.Run(() => {
  //                          string fileName = CtFile.GetFileName(openMap.FileName);
  //                          if (!mBypassSocket) {
  //                              SendFile(mHostIP, mSendMapPort, fileName);
  //                          } else {
  //                              /*-- 空跑模擬檔案傳送中 --*/
  //                              SpinWait.SpinUntil(() => false, 1000);
  //                          }
  //                          RaiseAgvClientEvent(AgvClientEventType.SendFile, fileName);
  //                      });
  //                  } finally {
  //                      prog?.Close();
  //                      prog = null;
  //                  }

  //              }
  //          }
  //      }

  //      /// <summary>
  //      /// 設定GL模式
  //      /// </summary>
  //      /// <param name="mode">GL模式</param>
  //      public void SetGLMode(GLMode mode) {
  //          RaiseMapEvent(MapEventType.GLMode, mode);
  //      }

  //      /// <summary>
  //      /// 清除
  //      /// </summary>
  //      public void Erase() {
  //          RaiseMapEvent(MapEventType.GLMode, GLMode.Erase);
  //      }

  //      /// <summary>
  //      /// 車子位置確認
  //      /// </summary>
  //      public void CarPosConfirm() {
  //          if (mBypassSocket) {
  //              /*-- 空跑模擬CarPosConfirm --*/
  //              SpinWait.SpinUntil(() => false, 1000);
  //              return;
  //          }
  //          List<CartesianPos> matchSet = new List<CartesianPos>();
  //          List<CartesianPos> modelSet = new List<CartesianPos>();
  //          CartesianPos nowOdometry = new CartesianPos();
  //          CartesianPos transResult = new CartesianPos();
  //          List<Point> scanPoint = new List<Point>();
  //          double angle;
  //          double Laserangle;
  //          double gValue = 0;
  //          double similarity = 0;
  //          int[] obstaclePos = new int[2];
  //          //mAGV.GetPosition(out posX, out posY, out posT);
  //          int idx = 0;
  //          foreach (int dist in mCarInfo.LaserData) {
  //              obstaclePos = Transformation.LaserPoleToCartesian(dist, -135, 0.25, idx++, 43, 416.75, 43, mCarInfo.X, mCarInfo.Y, mCarInfo.ThetaGyro, out angle, out Laserangle);//, out dataAngle, out laserAngle);

  //              matchSet.Add(new CartesianPos(obstaclePos[0], obstaclePos[1]));
  //              obstaclePos = null;
  //          }
  //          nowOdometry.SetPosition(mCarInfo.X, mCarInfo.Y, mCarInfo.ThetaGyro * Math.PI / 180);
  //          gValue = mMapMatch.FindClosetMatching(matchSet, 4, 1.5, 0.01, 50, 2000, false, transResult, out modelSet);
  //          //Correct accumulate error this time
  //          mMapMatch.NewPosTransformation(nowOdometry, transResult.x, transResult.y, transResult.theta);
  //          mMapMatch.NewPosTransformation(matchSet, transResult.x, transResult.y, transResult.theta);
  //          double[] Position = new double[3] { nowOdometry.x, nowOdometry.y, nowOdometry.theta * 180 / Math.PI };

  //          //Display car position
  //          //MapUI1.PosCar = new Pos(nowOdometry.x, nowOdometry.y, nowOdometry.theta);
  //          //SetPosition(nowOdometry.x, nowOdometry.y, nowOdometry.theta);
  //          //Display current scanning information
  //          //MapUI1.RemoveGroupPoint("LaserLength");
  //          scanPoint.Clear();
  //          for (int m = 0; m < matchSet.Count; m++) {
  //              scanPoint.Add(new Point((int)matchSet[m].x, (int)matchSet[m].y));
  //          }
  //          //MapUI1.DrawPoints(scanPoint, Color.Red, "LaserLength", 1);
  //          //MapUI1.PosCar = new Pos(Position[0], Position[1], Position[2]);


  //          similarity = mMapMatch.SimilarityEvalute(modelSet, matchSet);
  //          //if (similarity > 0.85) {
  //          SetPosition(Position[0], Position[1], Position[2]);
  //          //}

  //          Dictionary<string, object> dic = new Dictionary<string, object>();
  //          dic.Add(matchSet, VarDef.ScanPoint);
  //          dic.Add(mCarInfo.ToPos(), VarDef.PosCar);
  //          RaiseMapEvent(MapEventType.CarPosConfirm, dic);

  //      }

  //      /// <summary>
  //      /// 車子馬達速度
  //      /// </summary>
  //      public int Velocity {
  //          get {
  //              return mVelocity;
  //          }
  //          set {
  //              mVelocity = value;
  //          }
  //      }

  //      /// <summary>
  //      /// 取得雷射
  //      /// </summary>
  //      public void GetLaser() {
  //          /*-- 若是雷射資料則更新資料 --*/
  //          string[] rtnMsg = SendMsg("Get:Laser");
  //          if (rtnMsg.Length > 3) {
  //              if (rtnMsg[1] == "Laser") {
  //                  string[] sreRemoteLaser = rtnMsg[3].Split(',');
  //                  mCarInfo.LaserData = sreRemoteLaser.Select(x => int.Parse(x));
  //                  RaiseMapEvent(MapEventType.GetLaser, mCarInfo);
  //              }
  //          }
  //      }

  //      /// <summary>
  //      /// 變更車子資料發送狀態
  //      /// </summary>
  //      /// <param name="on">true:開啟/false:關閉資訊回傳</param>
  //      /// <remarks>
  //      /// modify by Jay 2017/09/08
  //      /// </remarks>
  //      /// <returns>True:發送中/False:停止發送</returns>
  //      public bool ChangeSendInfo() {
  //          IsGettingLaser = !IsGettingLaser;
  //          if (IsGettingLaser) {
  //              /*-- 開啟車子資訊讀取執行緒 --*/
  //              mSoxMonitorCmd.Start();

  //              /*-- 向Server端要求車子資料 --*/
  //              string[] rtnMsg = SendMsg("Get:Car:True");
  //              IsGettingLaser = rtnMsg.Count() > 2 && "True" == rtnMsg[2];

  //              /*-- 車子未發送資料則關閉Socket --*/
  //              if (!IsGettingLaser) {
  //                  mSoxMonitorCmd.Socket.Shutdown(SocketShutdown.Both);
  //                  mSoxMonitorCmd.Socket.Close();
  //              }
  //          } else {
  //              SendMsg("Get:Car:False");
  //          }
  //          return IsGettingLaser;
  //      }

  //      /// <summary>
  //      /// 地圖修正
  //      /// </summary>
  //      public void CorrectOri() {
  //          RaiseMapEvent(MapEventType.ClearMapGL);
  //          tsk_FixOriginScanningFile();
  //          //CtThread.CreateThread(ref mTdMapOperation, "mLoadOriginScaning: ", tsk_FixOriginScanningFile);
  //      }

  //      /// <summary>
  //      /// 清除Map
  //      /// </summary>
  //      public void ClearMap() {
  //          RaiseMapEvent(MapEventType.ClearMapGL);
  //      }

  //      /// <summary>
  //      /// 載入檔案
  //      /// </summary>
  //      /// <param name="type">載入檔案類型</param>
  //      public async void LoadFile(FileType type) {
  //          OpenFileDialog openMap = new OpenFileDialog();
  //          openMap.InitialDirectory = DefMapDir;
  //          openMap.Filter = $"MAP|*.{type.ToString().ToLower()}";
  //          if (openMap.ShowDialog() == DialogResult.OK) {
  //              CtProgress prog = new CtProgress($"Load {type}", $"Loading {type}...");
  //              try {
  //                  switch (type) {
  //                      case FileType.Ori:
  //                          await Task.Run(() => LoadOri(openMap.FileName));
  //                          RaiseTestingEvent(TestingEventType.CurOriPath);
  //                          break;
  //                      case FileType.Map:
  //                          await Task.Run(() => LoadMap(openMap.FileName));
  //                          break;
  //                      default:
  //                          throw new ArgumentException($"無法載入未定義的檔案類型{type}");
  //                  }
  //                  RaiseAgvClientEvent(AgvClientEventType.LoadFile, type);
  //              } catch (Exception ex) {
  //                  Console.WriteLine(ex.Message);
  //                  CtMsgBox.Show("Error", ex.Message);
  //              } finally {
  //                  prog?.Close();
  //                  prog = null;
  //              }
  //          }
  //          openMap = null;
  //      }

  //      /// <summary>
  //      /// 向AGV要求檔案
  //      /// </summary>
  //      /// <param name="type">檔案類型</param>
  //      public void GetFile(FileType type) {
  //          string fileList = string.Empty;
  //          if (GetFileList(type, out fileList)) {
  //              using (MapList f = new MapList(fileList)) {
  //                  if (f.ShowDialog() == DialogResult.OK) {
  //                      mProg = new CtProgress($"Get {type}", $"Donwloading {type} from AGV");
  //                      FileDownload(f.strMapList, type);
  //                  }
  //              }
  //          }

  //      }

  //      /// <summary>
  //      /// 載入Ori檔
  //      /// </summary>
  //      /// <param name="oriPath"></param>
  //      /// <returns></returns>
  //      private void LoadOri(string oriPath) {
  //          CurOriPath = oriPath;
  //          RaiseMapEvent(MapEventType.ClearMapGL);
  //          MapReading MapReading = null;
  //          if (!mBypassLoadFile) {//無BypassLoadFile
  //              MapReading = new MapReading(CurOriPath);
  //              CartesianPos carPos;
  //              List<CartesianPos> laserData;
  //              //List<Point> listMap = new List<Point>();
  //              int dataLength = MapReading.OpenFile();
  //              if (dataLength != 0) {
  //                  for (int n = 0; n < dataLength; n++) {
  //                      MapReading.ReadScanningInfo(n, out carPos, out laserData);
  //                      Dictionary<string, object> dic = new Dictionary<string, object>();
  //                      dic.Add(carPos.ToPos(), VarDef.PosCar);
  //                      dic.Add(laserData, VarDef.ScanPoint);
  //                      RaiseMapEventSync(MapEventType.LoadingOri, dic);
  //                      carPos = null;
  //                      laserData = null;
  //                  }
  //              }
  //          } else {//Bypass LoadFile功能
  //              /*-- 空跑一秒，模擬檔案載入 --*/
  //              SpinWait.SpinUntil(() => false, 1000);
  //          }
  //          MapReading = null;
  //      }

  //      public async void LoadOriAsync() {
  //          OpenFileDialog openMap = new OpenFileDialog();
  //          openMap.InitialDirectory = mDefMapDir;
  //          openMap.Filter = "MAP|*.ori";

  //          if (openMap.ShowDialog() == DialogResult.OK) {
  //              //mbLoadMap = true;
  //              //GetPointsFromFile(openMap.FileName);
  //              CurOriPath = openMap.FileName;
  //              RaiseMapEvent(MapEventType.ClearMapGL);
  //              await new Task(() => tsk_ReadOriginScanningFile());
  //              //CtThread.CreateThread(ref mLoadOriginScanning, "mLoadOriginScaning: ", tsk_ReadOriginScanningFile);
  //          }
  //          openMap = null;
  //      }

  //      /// <summary>
  //      /// 移動控制
  //      /// </summary>
  //      /// <param name="direction">移動方向</param>
  //      /// <param name="velocity">移動速度</param>
  //      public void MotionContorl(MotionDirection direction, int velocity = 0) {
  //          string[] rtnMsg = SendMsg("Get:IsOpen");


  //          if (rtnMsg.Count() > 2 && bool.Parse(rtnMsg[2])) {

  //              if (direction == MotionDirection.Stop) {
  //                  SendMsg("Set:Stop");
  //              } else {

  //                  string cmd = string.Empty;
  //                  switch (direction) {
  //                      case MotionDirection.Forward:
  //                          cmd = $"Set:DriveVelo:{mVelocity}:{mVelocity}";
  //                          break;
  //                      case MotionDirection.Backward:
  //                          cmd = $"Set:DriveVelo:-{mVelocity}:-{mVelocity}";
  //                          break;
  //                      case MotionDirection.LeftTrun:
  //                          cmd = $"Set:DriveVelo:{mVelocity}:-{mVelocity}";
  //                          break;
  //                      case MotionDirection.RightTurn:
  //                          cmd = $"Set:DriveVelo:-{mVelocity}:{mVelocity}";
  //                          break;
  //                  }
  //                  SendMsg(cmd);
  //                  SendMsg("Set:Start");
  //              }
  //          }
  //      }

  //      /// <summary>
  //      /// 馬達Servo On/Off
  //      /// </summary>
  //      /// <param name="on">是否進行馬達ServerOn</param>
  //      public void MotorServo(bool on) {
  //          SendMsg($"Set:Servo{(on ? "On" : "Off")}");
  //      }

  //      /// <summary>
  //      /// 
  //      /// </summary>
  //      /// <param name="start"></param>
  //      public void StartStop(bool start) {
  //          SendMsg($"Set:{(start ? "Start" : "Stop")}");
  //      }

  //      /// <summary>
  //      /// 載入地圖
  //      /// </summary>
  //      /// <param name="mapPath">Map檔路徑</param>
  //      private void LoadMap(string mapPath) {
  //          List<Line> dispLines = new List<Line>();
  //          List<Point> dispPoint = new List<Point>();
  //          List<CartesianPos> goalList = new List<CartesianPos>();
  //          List<CartesianPos> obstaclePoints = new List<CartesianPos>();
  //          List<MapLine> obstacleLine = new List<MapLine>();
  //          CurMapPath = mapPath;
  //          string mPath = CtFile.GetFileName(mapPath);
  //          SendMsg($"Set:MapName:{mPath}");

  //          if (mBypassLoadFile) {
  //              /*-- 空跑1秒模擬載入Map檔 --*/
  //              SpinWait.SpinUntil(() => false, 1000);
  //          } else {

  //              CartesianPos minimumPos;
  //              CartesianPos maximumPos;

  //              #region - Retrive information from .map file -

  //              Console.WriteLine(CurMapPath);
  //              using (MapReading read = new MapReading(CurMapPath)) {
  //                  read.OpenFile();
  //                  read.ReadMapBoundary(out minimumPos, out maximumPos);
  //                  read.ReadMapGoalList(out goalList);
  //                  read.ReadMapObstacleLines(out obstacleLine);
  //                  read.ReadMapObstaclePoints(out obstaclePoints);
  //              }
  //              Goals = goalList;
  //              Console.WriteLine($"obstacLeLine:{obstacleLine}");
  //              Console.WriteLine($"obstaclePoint:{obstaclePoints}");

  //              mMapMatch.Reset();
  //              for (int i = 0; i < obstacleLine.Count; i++) {
  //                  int start = (int)obstacleLine[i].start.x;
  //                  int end = (int)obstacleLine[i].end.x;
  //                  int y = (int)obstacleLine[i].start.y;
  //                  for (int x = start; x < end; x++) {
  //                      mMapMatch.AddPoint(new CartesianPos(x, y));
  //                  }
  //              }

  //              for (int i = 0; i < obstaclePoints.Count; i++) {
  //                  mMapMatch.AddPoint(obstaclePoints[i]);
  //              }
  //              #endregion

  //              #region  - Map information display -

  //              for (int i = 0; i < obstacleLine.Count; i++) {
  //                  dispLines.Add(
  //                      new Line((int)obstacleLine[i].start.x, (int)obstacleLine[i].start.y,
  //                      (int)obstacleLine[i].end.x, (int)obstacleLine[i].end.y)
  //                  );
  //              }

  //              for (int i = 0; i < obstaclePoints.Count; i++) {
  //                  dispPoint.Add(new Point((int)obstaclePoints[i].x, (int)obstaclePoints[i].y));
  //              }
  //              minimumPos = null;
  //              maximumPos = null;
  //              #endregion

  //          }

  //          Dictionary<string, object> dic = new Dictionary<string, object>();
  //          dic.Add(dispLines, VarDef.ObstacleLines);
  //          dic.Add(dispPoint, VarDef.ObstaclePoints);
  //          dic.Add(goalList, VarDef.Goals);
  //          Console.WriteLine($"Lines:{dispLines.Count}");
  //          Console.WriteLine($"Points:{dispPoint.Count}");
  //          Console.WriteLine();
  //          RaiseMapEvent(MapEventType.LoadMap, dic);
  //          RaiseGoalSettingEvent(GoalSettingEventType.LoadMap, goalList);
  //          CurMapPath = mapPath;

  //          goalList = null;
  //          obstaclePoints = null;
  //          obstacleLine = null;

  //          dispLines = null;
  //          dispPoint = null;
  //      }

  //      /// <summary>
  //      /// 檢查Server是否在運作中
  //      /// </summary>
  //      /// <returns></returns>
  //      public bool CheckIsServerAlive() {
  //          if (mBypassSocket) {
  //              IsServerAlive = true;
  //              Thread.Sleep(1000);
  //          } else {
  //              bool isAlive = false;
  //              try {
  //                  string[] rtnMsg = SendMsg("Get:Hello", false);
  //                  isAlive = rtnMsg.Count() > 2 && rtnMsg[2] == "True";
  //              } catch (Exception ex) {
  //                  Console.WriteLine($"[SocketException] : {ex.Message}");
  //              } finally {
  //                  if (!mBypassSocket && !isAlive) {
  //                      CtMsgBox.Show("Failed", "Connect Failed!!", MsgBoxBtn.OK, MsgBoxStyle.Error);
  //                  }
  //              }
  //              IsServerAlive = isAlive;
  //          }
  //          return IsServerAlive;
  //      }

  //      #endregion ITesting

  //      #region IConsole


  //      #endregion IConsole

  //      #endregion Implement 

  //      #region Function - Private Methods

  //      /// <summary>
  //      /// Bypass Server
  //      /// </summary>
  //      /// <param name="bypass">是否Bypass Server</param>
  //      private void BypassServer(bool bypass) {
  //          if (bypass) {
  //              CtAsyncSocket mCmdSer = new CtAsyncSocket(mRecvCmdPort);
  //              mCmdSer.ServerListen();
  //          } else {

  //          }
  //      }

  //      /// <summary>
  //      /// 開啟常駐tsk
  //      /// </summary>
  //      private void StartUp() {
  //          Stat stt = Stat.SUCCESS;
  //          if (mOpcode != null) {
  //              string opcPath = CtDefaultPath.GetPath(SystemPath.CONFIG) + FILENAME_OPCODE;
  //              if (CtFile.IsFileExist(opcPath)) {
  //                  mOpcode.LoadOpcode(opcPath);
  //                  stt = AllocateOpcode(mOpcode.OpcodeCollection);
  //              }
  //          } else stt = Stat.ER_SYSTEM;

  //          /*-- 車子資訊接收 --*/
  //          mSoxMonitorCmd = new SocketMonitor(mCmdPort, tsk_RecvCmd).Listen();
  //          /*-- 地圖資料接收 --*/
  //          mSoxMonitorFile = new SocketMonitor(mFilePort, RecvFiles).Listen();
  //          /*-- 路徑規劃接收 --*/
  //          mSoxMonitorPath = new SocketMonitor(mRecvPathPort, tsk_RecvPath).Listen();
  //      }

  //      /// <summary>分配讀取進來的OpcodeList，依各Opcode範圍去跑相對應方法</summary>
		///// <param name="opcs">Opcode Data Collection</param>
		///// <returns>Status Code</returns>
		//private Stat AllocateOpcode(List<CtOpcode.OpcodeData> opcs) {
  //          Stat stt = Stat.SUCCESS;
  //          try {
  //              foreach (CtOpcode.OpcodeData item in opcs) {
  //                  if ((int)OpcodeRange.CAMPRO_BASE <= item.Opcode && item.Opcode < (int)OpcodeRange.CAMPRO_RANGE) {
  //                      AllocateCAMPro(item);
  //                  }
  //              }
  //          } catch {
  //              stt = Stat.ER_SYSTEM;
  //              //ExceptionLog(stt, ex);
  //          }
  //          return stt;
  //      }

  //      /// <summary>依照Opcode分配CAMPro之設定</summary>
		///// <param name="opc">Opcode Data</param>
		///// <returns>Status Code</returns>
		//private void AllocateCAMPro(CtOpcode.OpcodeData opc) {
  //          if (opc.Opcode == 1000) {   /* 電腦 MAC 認證 */
  //              /*-- 比對輸入的認證 --*/
  //              string strEncrypt;
  //              CtCrypto.Encrypt(CryptoMode.AES256, opc.Argument[0], out strEncrypt);

  //              ///*-- 如果為初始化密碼，進行初始化動作 --*/
  //              //if (strEncrypt == "UtWAz4xcdPTnUuJj526qHg==") {
  //              //    /* 確認驗證碼 */
  //              //    string pwd, pwsEnc;
  //              //    CtInput.Text(out pwd, "認證", "請輸入驗證碼", "", true);
  //              //    CtCrypto.Encrypt(CryptoMode.AES256, pwd, out pwsEnc);
  //              //    if (pwsEnc != "td1+CY1spGpYHBsye0VoEty8UA0rw7g9bQERA7XaKLM=") throw (new InvalidOperationException("恢復預設，但驗證輸入錯誤: " + pwd));

  //              //    /* 驗證成功則複寫 Opcode */
  //              //    CtCrypto.Encrypt(CryptoMode.AES256, CtCrypto.GetMacAddress()[0], out strEncrypt);
  //              //    mOpcode.SaveOpcode(1000, strEncrypt);
  //              //    mFlag_PcVerified = true;
  //              //} else if (opc.Argument[0] == "esa9kqbsmhzHVhDhUkY+QioGe4pJIQyof4fPlzpi2G0=") { /*-- 趙國源專用 --*/
  //              //    CtCrypto.Encrypt(CryptoMode.AES256, CtCrypto.GetMacAddress()[0], out strEncrypt);
  //              //    mOpcode.SaveOpcode(1000, strEncrypt);
  //              //    mFlag_PcVerified = true;
  //              //} else {    /*-- 非初始化密碼，比對 MacAddress --*/
  //              //    string strDecrypt;
  //              //    CtCrypto.Decrypt(CryptoMode.AES256, opc.Argument[0], out strDecrypt);
  //              //    if (!CtCrypto.IsMacAddress(strDecrypt)) {
  //              //        TraceLog("CAMPro", MsgType.RECORD, "此電腦非 CASTEC 認證之電腦");
  //              //        MessageBox.Show("此電腦非 CASTEC 認證的電腦", "系統錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
  //              //        throw (new InvalidOperationException("此電腦非 CASTEC 認證的電腦。 輸入授權: " + opc.Argument[0]));
  //              //    } else mFlag_PcVerified = true;
  //              //}
  //          } else if (opc.Opcode == 1001) {    /* Show CAMPro Exception */
  //              //ShowSysException = CtConvert.CBool(opc.Argument[0]);
  //          } else if (opc.Opcode == 1002) {    /* Buzzer Enabled */
  //              //mFlag_BuzzerEnabled = CtConvert.CBool(opc.Argument[0]);
  //          } else if (opc.Opcode == 1003) {    /*--Bypass Socket--*/
  //              mBypassSocket = CtConvert.CBool(opc.Argument[0]);
  //          } else if (opc.Opcode == 1004) {     /*-- Bypass LoadFile --*/
  //              mBypassLoadFile = CtConvert.CBool(opc.Argument[0]);
  //          } else if (opc.Opcode == 1050) {    /* Host IP */
  //              mHostIP = opc.Argument[0];
  //          } else if (opc.Opcode == 1051) {     /*-- FilePort --*/
  //              mFilePort = CtConvert.CInt(opc.Argument[0]);
  //          } else if (opc.Opcode == 1052) {     /*-- RecvCmdPort --*/
  //              mRecvCmdPort = CtConvert.CInt(opc.Argument[0]);
  //          } else if (opc.Opcode == 1053) {      /*-- CmdPort --*/
  //              mCmdPort = CtConvert.CInt(opc.Argument[0]);
  //          } else if (opc.Opcode == 1054) {      /*-- SendMapPort --*/
  //              mSendMapPort = CtConvert.CInt(opc.Argument[0]);
  //          } else if (opc.Opcode == 1055) {       /*-- RecvPathPort --*/
  //              mRecvPathPort = CtConvert.CInt(opc.Argument[0]);
  //          } else if (opc.Opcode == 1101) {      /*-- Default Map Directory Path --*/
  //              mDefMapDir = opc.Argument[0];
  //          }

  //      }

  //      /// <summary>
  //      /// 訊息傳送(會觸發事件)
  //      /// </summary>
  //      /// <param name="sendMseeage">傳送訊息內容</param>
  //      /// <param name="passChkConn">是否略過檢查連線狀態</param>
  //      /// <returns>Server端回應</returns>
  //      private string[] SendMsg(string sendMseeage, bool passChkConn = true) {
  //          if (mBypassSocket) {
  //              /*-- Bypass略過不傳 --*/
  //              return new string[] { "True" };
  //          } else if (passChkConn && !IsServerAlive) {
  //              /*-- 略過連線檢查且Server端未運作 --*/
  //              return new string[] { "False" };
  //          }

  //          /*-- 顯示發送出去的訊息 --*/
  //          string msg = $"{DateTime.Now} [Client] : {sendMseeage}\r\n";
  //          RaiseMsgTrans(msg);

  //          /*-- 等待Server端的回應 --*/
  //          string rtnMsg = SendStrMsg(mHostIP, mRecvCmdPort, sendMseeage);

  //          /*-- 顯示Server端回應 --*/
  //          msg = $"{DateTime.Now} [Server] : {rtnMsg}\r\n";
  //          RaiseMsgTrans(msg);

  //          return rtnMsg.Split(':');
  //      }

  //      /// <summary>
  //      /// 訊息傳送(具體Socket交握實現，但是不會觸發事件)
  //      /// </summary>
  //      /// <param name="serverIP">伺服端IP</param>
  //      /// <param name="requerPort">通訊埠號</param>
  //      /// <param name="sendMseeage">傳送訊息內容</param>
  //      /// <returns>Server端回應</returns>
  //      private string SendStrMsg(string serverIP, int requerPort, string sendMseeage) {

  //          //可以在字串編碼上做文章，可以傳送各種資訊內容，目前主要有三種編碼方式：
  //          //1.自訂連接字串編碼－－微量
  //          //2.JSON編碼--輕量
  //          //3.XML編碼--重量
  //          int state;
  //          int timeout = 5000;
  //          byte[] recvBytes = new byte[8192];//開啟一個緩衝區，存儲接收到的資訊

  //          IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIP.ToString()), requerPort);
  //          Socket answerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
  //          answerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
  //          answerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, timeout);
  //          try {

  //              answerSocket.Connect(ipEndPoint);//建立Socket連接
  //              byte[] sendContents = Encoding.UTF8.GetBytes(sendMseeage);
  //              state = answerSocket.Send(sendContents, sendContents.Length, 0);//發送二進位資料
  //              state = answerSocket.Receive(recvBytes);
  //              string strRecvCmd = Encoding.Default.GetString(recvBytes);//
  //              strRecvCmd = strRecvCmd.Split(new char[] { '\0' })[0];
  //              sendContents = null;
  //              return strRecvCmd;

  //          } catch (SocketException se) {
  //              Console.WriteLine("SocketException : {0}", se.ToString());
  //              //MessageBox.Show("目標拒絕連線!!");
  //              return "False";
  //          } catch (ArgumentNullException ane) {
  //              Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
  //              return "False";
  //          } catch (Exception ex) {
  //              Console.Write(ex.Message);
  //              return "False";
  //          } finally {
  //              ipEndPoint = null;
  //              recvBytes = null;
  //              // answerSocket.Shutdown(SocketShutdown.Both);
  //              // answerSocket.Disconnect(false);
  //              answerSocket.Close();
  //              // Console.Write("Disconnecting from server...\n");
  //              //Console.ReadKey();
  //              answerSocket.Dispose();
  //          }

  //      }

  //      /// <summary>
  //      /// Send file of server to client
  //      /// </summary>
  //      /// <param name="clientIP">Ip address of client</param>
  //      /// <param name="clientPort">Communication port</param>
  //      /// <param name="fileName">File name</param>
  //      /// 
  //      public void SendFile(string clientIP, int clientPort, string fileName) {
  //          string curMsg = "";
  //          try {
  //              IPAddress[] ipAddress = Dns.GetHostAddresses(clientIP);
  //              IPEndPoint ipEnd = new IPEndPoint(ipAddress[0], clientPort);
  //              /* Make IP end point same as Server. */
  //              Socket clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
  //              /* Make a client socket to send data to server. */
  //              string filePath = "D:\\MapInfo\\";
  //              /* File reading operation. */
  //              fileName = fileName.Replace("\\", "/");
  //              while (fileName.IndexOf("/") > -1) {
  //                  filePath += fileName.Substring(0, fileName.IndexOf("/") + 1);
  //                  fileName = fileName.Substring(fileName.IndexOf("/") + 1);
  //              }
  //              byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
  //              if (fileNameByte.Length > 1024 * 1024 * 5) {
  //                  curMsg = "File size is more than 850kb, please try with small file.";
  //                  return;
  //              }
  //              curMsg = "Buffering ...";
  //              byte[] fileData = File.ReadAllBytes(filePath + fileName);
  //              /* Read & store file byte data in byte array. */
  //              byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
  //              /* clientData will store complete bytes which will store file name length, 
  //              file name & file data. */
  //              byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);
  //              /* File name length’s binary data. */
  //              fileNameLen.CopyTo(clientData, 0);
  //              fileNameByte.CopyTo(clientData, 4);
  //              fileData.CopyTo(clientData, 4 + fileNameByte.Length);
  //              /* copy these bytes to a variable with format line [file name length]
  //              [file name] [ file content] */
  //              curMsg = "Connection to server ...";
  //              clientSock.Connect(ipEnd);
  //              /* Trying to connection with server. */
  //              curMsg = "File sending...";
  //              clientSock.Send(clientData);
  //              /* Now connection established, send client data to server. */
  //              curMsg = "Disconnecting...";
  //              clientSock.Close();
  //              fileNameByte = null;
  //              clientData = null;
  //              fileNameLen = null;
  //              /* Data send complete now close socket. */
  //              curMsg = "File transferred.";
  //          } catch (Exception ex) {
  //              if (ex.Message == "No connection could be made because the target machine actively refused it")
  //                  curMsg = "File Sending fail. Because server not running.";
  //              else
  //                  curMsg = "File Sending fail." + ex.Message;
  //          }
  //      }

  //      /// <summary>
  //      /// 將路徑封包拆解重新包裝
  //      /// </summary>
  //      /// <param name="pack">路徑封包</param>
  //      private List<Line> PathEncoder(string pack) {
  //          string[] pathArray = pack.Split(',');
  //          List<Line> rtnLine = new List<Line>();
  //          for (int i = 0; i < pathArray.Length - 5; i += 2) {
  //              rtnLine.Add(new Line(
  //                  int.Parse(pathArray[i]),
  //                  int.Parse(pathArray[i + 1]),
  //                  int.Parse(pathArray[i + 2]),
  //                  int.Parse(pathArray[i + 3])
  //                  )
  //              );
  //          }
  //          return rtnLine;
  //          //string[] pathArray = pack.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
  //          //IEnumerable<string> tmp = pathArray.Take(pathArray.Length - 5);
  //          //return tmp.ChunkBy(4).//平均分割以4為單位
  //          //    Select(v => v.Select(x => int.Parse(x)).ToArray()).//轉換為int[]
  //          //    Select(y => new Line(y[0],y[1],y[2],y[3])).ToList();//轉換為List<Line>
  //      }

  //      #region Task

  //      /// <summary>
  //      /// 地圖修正執行緒
  //      /// </summary>
  //      /// <remarks>
  //      /// Modified by Jay 2017/09/13
  //      /// </remarks>
  //      private void tsk_FixOriginScanningFile() {
  //          try {
  //              if (mBypassLoadFile) {
  //                  SpinWait.SpinUntil(() => false, 1000);
  //                  return;
  //              }
  //              MapReading MapReading = new MapReading(CurOriPath);
  //              CartesianPos carPos;
  //              List<CartesianPos> laserData;
  //              List<CartesianPos> filterData = new List<CartesianPos>();
  //              int dataLength = MapReading.OpenFile();
  //              if (dataLength == 0) return;

  //              List<CartesianPos> dataSet = new List<CartesianPos>();
  //              List<CartesianPos> predataSet = new List<CartesianPos>();
  //              List<CartesianPos> matchSet = new List<CartesianPos>();
  //              CartesianPos transResult = new CartesianPos();
  //              CartesianPos nowOdometry = new CartesianPos();
  //              CartesianPos preOdometry = new CartesianPos();
  //              CartesianPos accumError = new CartesianPos();
  //              CartesianPos diffOdometry = new CartesianPos();
  //              CartesianPos diffLaser = new CartesianPos();
  //              Stopwatch sw = new Stopwatch();
  //              double gValue = 0;
  //              int mode = 0;
  //              int corrNum = 0;

  //              mMapMatch.Reset();
  //              #region  1.Read car position and first laser scanning

  //              MapReading.ReadScanningInfo(0, out carPos, out laserData);
  //              mCarInfo.SetPos(carPos);
  //              RaiseMapEventSync(MapEventType.RefreshPosCar, carPos.ToPos());
  //              matchSet.AddRange(laserData);
  //              predataSet.AddRange(laserData);
  //              mMapMatch.GlobalMapUpdate(matchSet);                            //Initial environment model
  //              preOdometry.SetPosition(carPos.x, carPos.y, carPos.theta);

  //              #endregion

  //              for (int n = 1; n < dataLength; n++) {
  //                  #region 2.Read car position and laser scanning 

  //                  List<CartesianPos> addedSet = new List<CartesianPos>();
  //                  transResult.SetPosition(0, 0, 0);
  //                  carPos = null;
  //                  laserData = null;
  //                  MapReading.ReadScanningInfo(n, out carPos, out laserData);
  //                  nowOdometry.SetPosition(carPos.x, carPos.y, carPos.theta);

  //                  #endregion

  //                  #region 3.Correct accumulate error of odometry so far

  //                  mMapMatch.NewPosTransformation(nowOdometry, accumError.x, accumError.y, accumError.theta);
  //                  mMapMatch.NewPosTransformation(laserData, accumError.x, accumError.y, accumError.theta);
  //                  matchSet.Clear();
  //                  matchSet.AddRange(laserData);

  //                  #endregion

  //                  #region 4.Compute movement from last time to current time;

  //                  if (nowOdometry.theta - preOdometry.theta < -200)
  //                      diffOdometry.SetPosition(nowOdometry.x - preOdometry.x, nowOdometry.y - preOdometry.y, nowOdometry.theta + 360 - preOdometry.theta);
  //                  else if (nowOdometry.theta - preOdometry.theta > 200)
  //                      diffOdometry.SetPosition(nowOdometry.x - preOdometry.x, nowOdometry.y - preOdometry.y, -(preOdometry.theta + 360 - nowOdometry.theta));
  //                  else
  //                      diffOdometry.SetPosition(nowOdometry.x - preOdometry.x, nowOdometry.y - preOdometry.y, nowOdometry.theta - preOdometry.theta);
  //                  Console.WriteLine("Odometry varition:{0:F3} {1:F3} {2:F3}", diffOdometry.x, diffOdometry.y, diffOdometry.theta);

  //                  #endregion

  //                  #region 5.Display current scanning information

  //                  RaiseMapEventSync(MapEventType.RefreshScanPoint, matchSet);

  //                  #endregion

  //                  #region 6.Inspect odometry variation is not too large.Switch to pose tracking mode if too large.

  //                  sw.Restart();
  //                  if (Math.Abs(diffOdometry.x) >= 400 || Math.Abs(diffOdometry.y) >= 400 || Math.Abs(diffOdometry.theta) >= 30) {
  //                      mode = 1;
  //                      gValue = mMapMatch.PairwiseMatching(predataSet, matchSet, 4, 1.5, 0.01, 20, 300, false, transResult);
  //                  } else {
  //                      mode = 0;
  //                      gValue = mMapMatch.FindClosetMatching(matchSet, 4, 1.5, 0.01, 20, 300, false, transResult);
  //                      diffLaser.SetPosition(transResult.x, transResult.y, transResult.theta);
  //                  }

  //                  //If corresponding is too less,truct the odomery variation this time
  //                  if (mMapMatch.EstimateCorresponingPoints(matchSet, 10, 10, out corrNum, out addedSet)) {
  //                      mMapMatch.NewPosTransformation(nowOdometry, transResult.x, transResult.y, transResult.theta);
  //                      accumError.SetPosition(accumError.x + transResult.x, accumError.y + transResult.y, accumError.theta + transResult.theta);
  //                  }
  //                  sw.Stop();

  //                  if (mode == 0)
  //                      Console.WriteLine("[SLAM-Matching Mode]Corresponding Points:{0} Map Size:{1} Matching Time:{2:F3} Error{3:F3}",
  //                           corrNum, mMapMatch.parseMap.Count, sw.Elapsed.TotalMilliseconds, gValue);
  //                  else
  //                      Console.WriteLine("[SLAM-Tracking Mode]Matching Time:{0:F3} Error{1:F3}", sw.Elapsed.TotalMilliseconds, gValue);

  //                  #endregion

  //                  #region 7.Update variation

  //                  //Pairwise update
  //                  predataSet.Clear();
  //                  predataSet.AddRange(laserData);

  //                  //Update previous variable
  //                  preOdometry.SetPosition(nowOdometry.x, nowOdometry.y, nowOdometry.theta);
  //                  mCarInfo.SetPos(nowOdometry);

  //                  #endregion

  //                  //Display added new points                     
  //                  RaiseMapEventSync(MapEventType.DrawScanMap, addedSet);

  //                  //Display car position
  //                  RaiseMapEventSync(MapEventType.RefreshPosCar, nowOdometry.ToPos());
  //              }
  //              RaiseMapEvent(MapEventType.CorrectOriComplete);
  //          } catch {

  //          } finally {
  //          }
  //      }

  //      /// <summary>
  //      /// 路徑接收執行緒
  //      /// </summary>
  //      private void tsk_RecvPath(object obj) {
  //          SocketMonitor soxMonitor = obj as SocketMonitor;
  //          //Socket sRecvCmdTemp = sRecvCmd.Accept();//Accept 以同步方式從偵聽通訊端的連接請求佇列中提取第一個掛起的連接請求，然後創建並返回新的 Socket
  //          Socket sRecvCmdTemp = serverComm.ClientAccept(soxMonitor.Socket);
  //          //sRecvCmdTemp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
  //          //sRecvCmdTemp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);//設置發送資料超時
  //          //sRecvCmdTemp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 1024); //設置發送緩衝區大小 1K
  //          //sRecvCmdTemp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 9000);//設置接收緩衝區大小1K

  //          try {
  //              sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Path:Require"));
  //              byte[] recvBytes = new byte[1024 * 500];//開啟一個緩衝區，存儲接收到的資訊
  //              sRecvCmdTemp.Receive(recvBytes); //將讀得的內容放在recvBytes中
  //              string strRecvCmd = Encoding.Default.GetString(recvBytes);
  //              //程式運行到這個地方，已經能接收到遠端發過來的命令了
  //              //Console.WriteLine("[Server] : " + strRecvCmd);
  //              //*************
  //              //解碼命令，並執行相應的操作----如下面的發送本機圖片
  //              //*************
  //              strRecvCmd = strRecvCmd.Split(new char[] { '\0' })[0];

  //              string[] strArray = strRecvCmd.Split(':');
  //              recvBytes = null;
  //              if (strArray[0] == "Path" && !string.IsNullOrEmpty(strArray[1])) {
  //                  RaiseMapEvent(MapEventType.DrawPath, PathEncoder(strArray[1]));
  //              }
  //              //else
  //              //    sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Path:False"));

  //              strRecvCmd = null;
  //              strArray = null;
  //              sRecvCmdTemp.Close();

  //          } catch (SocketException se) {
  //              Console.WriteLine("[Status Recv] : " + se.ToString());
  //              MessageBox.Show("目標拒絕連線");
  //          } catch (Exception ex) {
  //              Console.Write(ex.Message);
  //              //throw ex;
  //          } finally {
  //              sRecvCmdTemp.Close();
  //              sRecvCmdTemp = null;
  //          }
  //      }

  //      /// <summary>
  //      /// 檔案接收執行緒 20170911
  //      /// </summary>
  //      /// <param name="obj"></param>
  //      private void RecvFiles(object obj) {

  //          int fileNameLen = 0;
  //          int recieve_data_size = 0;
  //          int first = 1;
  //          int receivedBytesLen = 0;
  //          double cal_size = 0;
  //          SocketMonitor soxMonitor = obj as SocketMonitor;
  //          Socket clientSock = null;
  //          BinaryWriter bWrite = null;
  //          //MemoryStream ms = null;
  //          string curMsg = "Stopped";
  //          string fileName = "";
  //          try {
  //              if (!mBypassSocket) {
  //                  clientSock = serverComm.ClientAccept(soxMonitor.Socket);
  //                  curMsg = "Running and waiting to receive file.";

  //                  //Socket clientSock = sRecvFile.Accept();
  //                  //clientSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
  //                  //clientSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);//設置發送資料超時
  //                  //clientSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 1024); //設置發送緩衝區大小 1K
  //                  /* When request comes from client that accept it and return 
  //                  new socket object for handle that client. */
  //                  byte[] clientData = new byte[1024 * 10000];
  //                  do {
  //                      receivedBytesLen = clientSock.Receive(clientData);
  //                      curMsg = "Receiving data...";
  //                      if (first == 1) {
  //                          fileNameLen = BitConverter.ToInt32(clientData, 0);
  //                          /* I've sent byte array data from client in that format like 
  //                          [file name length in byte][file name] [file data], so need to know 
  //                          first how long the file name is. */
  //                          fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);
  //                          /* Read file name */
  //                          if (!Directory.Exists(mDefMapDir)) {
  //                              Directory.CreateDirectory(mDefMapDir);
  //                          }
  //                          if (File.Exists(mDefMapDir + "/" + fileName)) {
  //                              File.Delete(mDefMapDir + "/" + fileName);
  //                          }
  //                          bWrite = new BinaryWriter(File.Open(mDefMapDir + "/" + fileName, FileMode.OpenOrCreate));
  //                          /* Make a Binary stream writer to saving the receiving data from client. */
  //                          // ms = new MemoryStream();
  //                          bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);
  //                          //ms.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 -
  //                          //fileNameLen);
  //                          //寫入資料 ，呈現於BITMAP用  
  //                          /* Read remain data (which is file content) and 
  //                          save it by using binary writer. */
  //                          curMsg = "Saving file...";
  //                          /* Close binary writer and client socket */
  //                          curMsg = "Received & Saved file; Server Stopped.";
  //                      } else //第二筆接收為資料  
  //                        {
  //                          //-----------  
  //                          fileName = Encoding.ASCII.GetString(clientData, 0,
  //                          receivedBytesLen);
  //                          //-----------  
  //                          bWrite.Write(clientData/*, 4 + fileNameLen, receivedBytesLen - 4 -
  //                          fileNameLen*/, 0, receivedBytesLen);
  //                          //每筆接收起始 0 結束為當次Receive長度  
  //                          //ms.Write(clientData, 0, receivedBytesLen);
  //                          //寫入資料 ，呈現於BITMAP用  
  //                      }
  //                      recieve_data_size += receivedBytesLen;
  //                      //計算資料每筆資料長度並累加，後面可以輸出總值看是否有完整接收  
  //                      cal_size = recieve_data_size;
  //                      cal_size /= 1024;
  //                      cal_size = Math.Round(cal_size, 2);

  //                      first++;
  //                      SpinWait.SpinUntil(() => false, 10); //每次接收不能太快，否則會資料遺失  

  //                  } while (clientSock.Available != 0);
  //                  clientData = null;
  //              } else {
  //                  SpinWait.SpinUntil(() => false, 1000);
  //                  fileName = "FileName";
  //              }


  //          } catch (SocketException se) {
  //              Console.WriteLine("SocketException : {0}", se.ToString());
  //              MessageBox.Show("檔案傳輸失敗!");
  //              curMsg = "File Receiving error.";
  //          } catch (Exception ex) {
  //              Console.WriteLine("[RecvFiles]" + ex.ToString());
  //              curMsg = "File Receiving error.";
  //          } finally {
  //              bWrite?.Close();
  //              clientSock?.Shutdown(SocketShutdown.Both);
  //              clientSock?.Close();
  //              clientSock = null;
  //              RaiseTestingEvent(TestingEventType.GetFile);
  //          }
  //      }

  //      /// <summary>
  //      /// 檔案接收執行緒
  //      /// </summary>
  //      private void tsk_RecvFile(object obj) {
  //          int recieve_data_size = 0;
  //          int first = 1;
  //          SocketMonitor soxMonitor = obj as SocketMonitor;
  //          Socket clientSock = null;
  //          BinaryWriter bWrite = null;
  //          //MemoryStream ms = null;
  //          string curMsg = "Stopped";
  //          try {
  //              string fileName = string.Empty;
  //              curMsg = "Running and waiting to receive file.";
  //              //Socket clientSock = serverComm.ClientAccept(sRecvFile);
  //              clientSock = soxMonitor.Socket.Accept();
  //              clientSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
  //              clientSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);//設置發送資料超時
  //              clientSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 1024); //設置發送緩衝區大小 1K
  //              /* When request comes from client that accept it and return 
  //              new socket object for handle that client. */
  //              byte[] clientData = new byte[1024 * 10000];
  //              do {
  //                  int receivedBytesLen = clientSock.Receive(clientData);
  //                  curMsg = "Receiving data...";
  //                  if (first == 1) {
  //                      int fileNameLen = BitConverter.ToInt32(clientData, 0);
  //                      /* I've sent byte array data from client in that format like 
  //                      [file name length in byte][file name] [file data], so need to know 
  //                      first how long the file name is. */
  //                      fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);
  //                      /* Read file name */
  //                      if (!Directory.Exists(mDefMapDir)) {
  //                          Directory.CreateDirectory(mDefMapDir);
  //                      }
  //                      if (File.Exists(mDefMapDir + "/" + fileName)) {
  //                          File.Delete(mDefMapDir + "/" + fileName);
  //                      }
  //                      bWrite = new BinaryWriter(File.Open(mDefMapDir + "/" + fileName, FileMode.OpenOrCreate));
  //                      /* Make a Binary stream writer to saving the receiving data from client. */
  //                      // ms = new MemoryStream();
  //                      bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);
  //                      //ms.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 -
  //                      //fileNameLen);
  //                      //寫入資料 ，呈現於BITMAP用  
  //                      /* Read remain data (which is file content) and 
  //                      save it by using binary writer. */
  //                      curMsg = "Saving file...";
  //                      /* Close binary writer and client socket */
  //                      curMsg = "Received & Saved file; Server Stopped.";
  //                  } else //第二筆接收為資料  
  //                    {
  //                      //-----------  
  //                      fileName = Encoding.ASCII.GetString(clientData, 0,
  //                      receivedBytesLen);
  //                      //-----------  
  //                      bWrite.Write(clientData/*, 4 + fileNameLen, receivedBytesLen - 4 -
  //                          fileNameLen*/, 0, receivedBytesLen);
  //                      //每筆接收起始 0 結束為當次Receive長度  
  //                      //ms.Write(clientData, 0, receivedBytesLen);
  //                      //寫入資料 ，呈現於BITMAP用  
  //                  }
  //                  recieve_data_size += receivedBytesLen;
  //                  //計算資料每筆資料長度並累加，後面可以輸出總值看是否有完整接收  
  //                  double cal_size = recieve_data_size;
  //                  cal_size /= 1024;
  //                  cal_size = Math.Round(cal_size, 2);

  //                  first++;
  //                  SpinWait.SpinUntil(() => false, 10); //每次接收不能太快，否則會資料遺失  

  //              } while (clientSock.Available != 0);
  //              clientData = null;

  //          } catch (SocketException se) {
  //              Console.WriteLine("SocketException : {0}", se.ToString());
  //              MessageBox.Show("檔案傳輸失敗!");
  //              curMsg = "File Receiving error.";
  //          } catch (Exception ex) {
  //              Console.WriteLine("[RecvFiles]" + ex.ToString());
  //              curMsg = "File Receiving error.";
  //          } finally {
  //              bWrite.Close();
  //              clientSock.Shutdown(SocketShutdown.Both);
  //              clientSock.Close();
  //              clientSock = null;
  //          }
  //      }

  //      /// <summary>
  //      /// 車子資訊接收執行緒
  //      /// </summary>
  //      public void tsk_RecvCmd(object obj) {
  //          SocketMonitor soxMonitor = obj as SocketMonitor;
  //          Socket sRecvCmdTemp = null;

  //          //Socket sRecvCmdTemp = sRecvCmd.Accept();//Accept 以同步方式從偵聽通訊端的連接請求佇列中提取第一個掛起的連接請求，然後創建並返回新的 Socket
  //          //sRecvCmdTemp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
  //          //sRecvCmdTemp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);//設置發送資料超時
  //          //sRecvCmdTemp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 1024); //設置發送緩衝區大小 1K
  //          //sRecvCmdTemp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 9000);//設置接收緩衝區大小1K
  //          sRecvCmdTemp = serverComm.ClientAccept(soxMonitor.Socket);
  //          try {
  //              while (IsGettingLaser) {

  //                  SpinWait.SpinUntil(() => false, 1);//每個執行緒內部的閉環裡面都要加個「短時間」睡眠，使得執行緒佔用資源得到及時釋放
  //                                                     //Thread.Sleep(1);
  //                  byte[] recvBytes = new byte[1024 * 500];//開啟一個緩衝區，存儲接收到的資訊
  //                  sRecvCmdTemp.Receive(recvBytes); //將讀得的內容放在recvBytes中
  //                  string strRecvCmd = Encoding.Default.GetString(recvBytes);//
  //                                                                            //程式運行到這個地方，已經能接收到遠端發過來的命令了
  //                  strRecvCmd = strRecvCmd.Split(new char[] { '\0' })[0];
  //                  //Console.WriteLine("[Server] : " + strRecvCmd);

  //                  //*************
  //                  //解碼命令，並執行相應的操作----如下面的發送本機圖片
  //                  //*************

  //                  string[] strArray = strRecvCmd.Split(':');
  //                  recvBytes = null;
  //                  if (CarInfo.TryParse(strRecvCmd, out mCarInfo)) {
  //                      RaiseCarInfoRefresh(mCarInfo);
  //                      sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Get:Car:True"));
  //                  } else {
  //                      sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Get:Car:False"));
  //                  }

  //                  strRecvCmd = null;
  //                  strArray = null;
  //              }
  //          } catch (SocketException se) {
  //              Console.WriteLine("[Status Recv] : " + se.ToString());
  //              MessageBox.Show("目標拒絕連線");
  //          } catch (Exception ex) {
  //              Console.Write(ex.Message);
  //              //throw ex;
  //          } finally {
  //              sRecvCmdTemp?.Close();
  //              sRecvCmdTemp = null;
  //          }
  //      }

  //      public void ReadOriginScanningFileAsync() {
  //          CtProgress prog = new CtProgress("LoadOri", "Loading Ori...");
  //          try {
  //              MapReading MapReading = null;
  //              if (!mBypassLoadFile) {//無BypassLoadFile
  //                  MapReading = new MapReading(CurOriPath);
  //                  CartesianPos carPos;
  //                  List<CartesianPos> laserData;
  //                  //List<Point> listMap = new List<Point>();
  //                  int dataLength = MapReading.OpenFile();
  //                  if (dataLength != 0) {
  //                      for (int n = 0; n < dataLength; n++) {
  //                          MapReading.ReadScanningInfo(n, out carPos, out laserData);
  //                          Dictionary<string, object> dic = new Dictionary<string, object>();
  //                          dic.Add(carPos, VarDef.PosCar);
  //                          dic.Add(laserData, VarDef.ScanPoint);
  //                          RaiseMapEventSync(MapEventType.LoadingOri, dic);

  //                          carPos = null;
  //                          laserData = null;
  //                      }
  //                  }
  //              } else {//Bypass LoadFile功能
  //                  /*-- 空跑一秒，模擬檔案載入 --*/
  //                  //SpinWait.SpinUntil(() => false, 1000);
  //                  Thread.Sleep(1000);
  //              }
  //              RaiseMapEvent(MapEventType.LoadComplete);
  //              MapReading = null;
  //          } catch (Exception ex) {
  //          } finally {
  //              prog?.Close();
  //              prog = null;
  //          }
  //      }

  //      /// <summary>
  //      /// LoadOri執行緒
  //      /// </summary>
  //      /// <param name="obj">請輸入string類型地圖路徑</param>
  //      public void tsk_ReadOriginScanningFile() {
  //          MapReading MapReading = null;
  //          if (!mBypassLoadFile) {//無BypassLoadFile
  //              MapReading = new MapReading(CurOriPath);
  //              CartesianPos carPos;
  //              List<CartesianPos> laserData;
  //              //List<Point> listMap = new List<Point>();
  //              int dataLength = MapReading.OpenFile();
  //              if (dataLength != 0) {
  //                  for (int n = 0; n < dataLength; n++) {
  //                      MapReading.ReadScanningInfo(n, out carPos, out laserData);
  //                      Dictionary<string, object> dic = new Dictionary<string, object>();
  //                      dic.Add(carPos, VarDef.PosCar);
  //                      dic.Add(laserData, VarDef.ScanPoint);
  //                      RaiseMapEventSync(MapEventType.LoadingOri, dic);

  //                      carPos = null;
  //                      laserData = null;
  //                  }
  //              }
  //          } else {//Bypass LoadFile功能
  //              /*-- 空跑一秒，模擬檔案載入 --*/
  //              //SpinWait.SpinUntil(() => false, 1000);
  //              Thread.Sleep(1000);
  //          }
  //          RaiseMapEvent(MapEventType.LoadComplete);
  //          MapReading = null;

  //      }

  //      /// <summary>
  //      /// Map檔載入執行緒
  //      /// </summary>
  //      private void tsk_ReadMapFile() {
  //          CtProgress prog = new CtProgress("Load Map", "Loading Map...");

  //          try {
  //              if (mBypassLoadFile) {
  //                  /*-- 空跑1秒模擬載入Map檔 --*/
  //                  SpinWait.SpinUntil(() => false, 1000);
  //                  return;
  //              }
  //              List<Line> dispLines = new List<Line>();
  //              List<Point> dispPoint = new List<Point>();
  //              List<CartesianPos> goalList;
  //              List<CartesianPos> obstaclePoints;
  //              List<MapLine> obstacleLine;
  //              CartesianPos minimumPos;
  //              CartesianPos maximumPos;

  //              #region - Retrive information from .map file -

  //              using (MapReading read = new MapReading(CurMapPath)) {
  //                  read.OpenFile();
  //                  read.ReadMapBoundary(out minimumPos, out maximumPos);
  //                  read.ReadMapGoalList(out goalList);
  //                  read.ReadMapObstacleLines(out obstacleLine);
  //                  read.ReadMapObstaclePoints(out obstaclePoints);
  //              }

  //              mMapMatch.Reset();
  //              for (int i = 0; i < obstacleLine.Count; i++) {
  //                  int start = (int)obstacleLine[i].start.x;
  //                  int end = (int)obstacleLine[i].end.x;
  //                  int y = (int)obstacleLine[i].start.y;
  //                  for (int x = start; x < end; x++) {
  //                      mMapMatch.AddPoint(new CartesianPos(x, y));
  //                  }
  //              }

  //              for (int i = 0; i < obstaclePoints.Count; i++) {
  //                  mMapMatch.AddPoint(obstaclePoints[i]);
  //              }
  //              #endregion

  //              #region  - Map information display -

  //              for (int i = 0; i < obstacleLine.Count; i++) {
  //                  dispLines.Add(
  //                      new Line((int)obstacleLine[i].start.x, (int)obstacleLine[i].start.y,
  //                      (int)obstacleLine[i].end.x, (int)obstacleLine[i].end.y)
  //                  );
  //              }

  //              for (int i = 0; i < obstaclePoints.Count; i++) {
  //                  dispPoint.Add(new Point((int)obstaclePoints[i].x, (int)obstaclePoints[i].y));
  //              }
  //              Dictionary<string, object> dic = new Dictionary<string, object>();
  //              dic.Add(dispLines, VarDef.ObstacleLines);
  //              dic.Add(dispPoint, VarDef.ObstaclePoints);
  //              dic.Add(goalList, VarDef.Goals);
  //              RaiseMapEvent(MapEventType.LoadMap, dic);
  //              RaiseGoalSettingEvent(GoalSettingEventType.LoadMap, goalList);

  //              #endregion

  //              goalList = null;
  //              obstaclePoints = null;
  //              obstacleLine = null;
  //              minimumPos = null;
  //              maximumPos = null;
  //              dispLines = null;
  //              dispPoint = null;
  //          } catch {

  //          } finally {
  //              prog?.Close();
  //              prog = null;
  //          }

  //      }

  //      #endregion Task

  //      #region Event Raise

  //      /// <summary>
  //      /// 以同步方式進行MapGL相關事件發報
  //      /// </summary>
  //      /// <param name="type"></param>
  //      /// <param name="value"></param>
  //      private void RaiseMapEventSync(MapEventType type, object value = null) {
  //          MapEventTrigger?.Invoke(this, new MapEventArgs(type, value));
  //      }

  //      /// <summary>
  //      /// MapGL相關事件發報
  //      /// </summary>
  //      /// <param name="type">事件類型</param>
  //      /// <param name="value">傳遞參數</param>
  //      private void RaiseMapEvent(MapEventType type, object value = null) {
  //          MapEventTrigger?.BeginInvoke(this, new MapEventArgs(type, value), null, null);
  //      }

  //      /// <summary>
  //      /// Testing相關事件發報
  //      /// </summary>
  //      /// <param name="type"></param>
  //      /// <param name="value"></param>
  //      private void RaiseTestingEvent(TestingEventType type, object value = null) {
  //          TestingEventTrigger?.BeginInvoke(this, new TestingEventArgs(type, value), null, null);
  //      }

  //      /// <summary>
  //      /// AgvClient相關事件發報
  //      /// </summary>
  //      /// <param name="type">事件類型</param>
  //      /// <param name="value">傳遞參數</param>
  //      private void RaiseAgvClientEvent(AgvClientEventType type, object value = null) {
  //          AgvClientEventTrigger?.BeginInvoke(this, new AgvClientEventArgs(type, value), null, null);
  //      }

  //      /// <summary>
  //      /// 訊息傳輸發報
  //      /// </summary>
  //      /// <param name="msg">傳輸訊息內容</param>
  //      private void RaiseMsgTrans(string msg) {
  //      }

  //      /// <summary>
  //      /// 車子資訊發報
  //      /// </summary>
  //      /// <param name="e">車子資訊參數</param>
  //      private void RaiseCarInfoRefresh(CarInfo info) {
  //          RaiseAgvClientEvent(AgvClientEventType.CarInfoRefresh, info);
  //          RaiseMapEvent(MapEventType.CarInfoRefresh, info);
  //      }

  //      /// <summary>
  //      /// 使用者變更發報
  //      /// </summary>
  //      /// <param name="user">使用者資料</param>
  //      private void RaiseUserChanged(UserData user) {
  //          RaiseAgvClientEvent(AgvClientEventType.UserChanged, user);
  //      }

  //      #endregion Event Raise

  //      #region IDisposable Support

  //      protected virtual void Dispose(bool disposing) {
  //          if (!disposedValue) {
  //              if (disposing) {
  //                  // TODO: 處置 Managed 狀態 (Managed 物件)。
  //              }

  //              // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
  //              // TODO: 將大型欄位設為 null。

  //              disposedValue = true;
  //          }
  //      }

  //      /// <summary>
  //      /// 
  //      /// </summary>
  //      public void Dispose() {
  //          // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
  //          Dispose(true);
  //          // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
  //          // GC.SuppressFinalize(this);
  //      }

  //      #endregion

  //      #endregion Function - Private Methods

  //  }

    #endregion Declaration - Core

    #region Support Class

    /// <summary>
    /// Socket監測參數包
    /// </summary>
    public class SocketMonitor {

        #region Declaration - Fileds

        /// <summary>
        /// 執行緒物件
        /// </summary>
        public Thread Thread = null;

        #endregion Declaration - Fields

        #region Declaration - Porperties

        /// <summary>
        /// 要監測的Socket物件
        /// </summary>
        public Socket Socket { get; private set; }

        /// <summary>
        /// 取消旗標
        /// </summary>
        public bool IsCancel { get; private set; }

        /// <summary>
        /// 執行緒方法
        /// </summary>
        public Action<object> Task { get; private set; }

        /// <summary>
        /// 通訊埠號
        /// </summary>
        public int Port { get; private set; }

        #endregion Declaration - Properties

        #region Function - Constructors

        /// <summary>
        /// 不開放空白建置
        /// </summary>
        private SocketMonitor() { }

        /// <summary>
        /// 共用建構方法
        /// </summary>
        /// <param name="socket">要監測的Socket物件</param>
        /// <param name="port">通訊埠號</param>
        /// <param name="task">執行緒方法</param>
        /// <param name="cancel">取消旗標預設狀態</param>
        private SocketMonitor(Socket socket, int port, Action<object> task, bool cancel) {
            this.Socket = socket;
            this.Port = port;
            this.Task = task;
            this.IsCancel = cancel;


        }

        /// <summary>
        /// 一般建構方法
        /// </summary>
        /// <param name="socket">要監測的Socket物件</param>
        /// <param name="thread">執行緒物件</param>
        /// <param name="task">執行緒方法</param>
        public SocketMonitor(int port, Action<object> task) : this(
            new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp),
            port,
            task,
            false
            ) {
        }

        #endregion Function - Construcotrs

        #region Function - Public Methods

        /// <summary>
        /// 開始監聽
        /// </summary>
        /// <returns></returns>
        public SocketMonitor Listen() {
            if (this.Socket != null) {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, Port);
                this.Socket.Bind(endPoint);
                this.Socket.Listen(10);
                return this;
            } else {
                return null;
            }
        }

        /// <summary>
        /// 開始接收資料
        /// </summary>
        /// <returns>開始接收的<see cref="SocketMonitor"/>實例回傳</returns>
        public void Start() {
            if (Thread?.IsAlive ?? false) {
                this.IsCancel = true;
                CtThread.KillThread(ref Thread);
            }
            CtThread.CreateThread(ref Thread, "mTdClient: ", Task);
            Thread?.Start(this);
        }

        #endregion Function - Public Methods
    }

    //public class ClientSocket {

    //    #region Declaration - Fields

    //    /// <summary>
    //    /// 車子馬達轉速
    //    /// </summary>
    //    private int mVelocity = 500;

    //    /// <summary>
    //    /// 是否Bypass Socket通訊
    //    /// </summary>
    //    private bool mBypassSocket = false;

    //    /// <summary>
    //    /// Server端IP
    //    /// </summary>
    //    private static string mRemoteIP = "127.0.0.1";

    //    /// <summary>
    //    /// 接收請求的埠開啟後就一直進行偵聽
    //    /// </summary>
    //    private static readonly int mRecvCmdPort = (int)AgvPort.Cmd;

    //    /// <summary>
    //    /// 發送地圖的埠
    //    /// </summary>
    //    private static readonly int mSendMapPort = (int) AgvPort.GetFile;

    //    /// <summary>
    //    /// 命令接收物件
    //    /// </summary>
    //    private SocketMonitor mSoxMonitorCmd = null;

    //    /// <summary>
    //    /// 地圖資料接收物件
    //    /// </summary>
    //    private SocketMonitor mSoxMonitorFile = null;

    //    /// <summary>
    //    /// 路徑規劃接收物件
    //    /// </summary>
    //    private SocketMonitor mSoxMonitorPath = null;

    //    #endregion Declaration - Fields

    //    #region Declaration - Events

    //    /// <summary>
    //    /// 訊息傳輸事件
    //    /// </summary>
    //    public event MessageTransmission MsgTrans;

    //    #endregion Declaration - Events

    //    #region Properties

    //    /// <summary>
    //    /// 伺服端是否還有在運作
    //    /// </summary>
    //    public bool IsServerAlive { get; private set; }

    //    #endregion Properties

    //    #region Function - Public Methods

    //    /// <summary>
    //    /// 移動控制
    //    /// </summary>
    //    /// <param name="direction">移動方向</param>
    //    /// <param name="velocity">移動速度</param>
    //    public void MotionContorl(MotionDirection direction, int velocity = 0) {
    //        string[] rtnMsg = SendMsg("Get:IsOpen");


    //        if (rtnMsg.Count() > 2 && bool.Parse(rtnMsg[2])) {

    //            if (direction == MotionDirection.Stop) {
    //                SendMsg("Set:Stop");
    //            } else {

    //                string cmd = string.Empty;
    //                switch (direction) {
    //                    case MotionDirection.Forward:
    //                        cmd = $"Set:DriveVelo:{mVelocity}:{mVelocity}";
    //                        break;
    //                    case MotionDirection.Backward:
    //                        cmd = $"Set:DriveVelo:-{mVelocity}:-{mVelocity}";
    //                        break;
    //                    case MotionDirection.LeftTrun:
    //                        cmd = $"Set:DriveVelo:{mVelocity}:-{mVelocity}";
    //                        break;
    //                    case MotionDirection.RightTurn:
    //                        cmd = $"Set:DriveVelo:-{mVelocity}:{mVelocity}";
    //                        break;
    //                }
    //                SendMsg(cmd);
    //                SendMsg("Set:Start");
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 馬達Servo On/Off
    //    /// </summary>
    //    /// <param name="on">是否進行馬達ServerOn</param>
    //    public void MotorServo(bool on) {
    //        SendMsg($"Set:Servo{(on ? "On" : "Off")}");
    //    }

    //    /// <summary>
    //    /// 檢查Server是否在運作中
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool CheckIsServerAlive() {
    //        if (mBypassSocket) {
    //            IsServerAlive = true;
    //            Thread.Sleep(1000);
    //        } else {
    //            bool isAlive = false;
    //            try {
    //                string[] rtnMsg = SendMsg("Get:Hello", false);
    //                isAlive = rtnMsg.Count() > 2 && rtnMsg[2] == "True";
    //            } catch (Exception ex) {
    //                Console.WriteLine($"[SocketException] : {ex.Message}");
    //            } finally {
    //                if (!mBypassSocket && !isAlive) {
    //                    CtMsgBox.Show("Failed", "Connect Failed!!", MsgBoxBtn.OK, MsgBoxStyle.Error);
    //                }
    //            }
    //            IsServerAlive = isAlive;
    //        }
    //        return IsServerAlive;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="start"></param>
    //    public void StartStop(bool start) {
    //        SendMsg($"Set:{(start ? "Start" : "Stop")}");
    //    }

    //    /// <summary>
    //    /// 向Server端要求檔案
    //    /// </summary>
    //    /// <param name="type">檔案類型</param>
    //    /// <remarks>modified by Jay 2017/09/20</remarks>
    //    public bool GetFileList(FileType type, out string fileList) {
    //        bool ret = true;
    //        fileList = string.Empty;
    //        if (mBypassSocket) {
    //            fileList = $"{type}1,{type}2,{type}3";
    //        } else {
    //            string[] rtnMsg = SendMsg($"Get:{type}List");
    //            fileList = rtnMsg[2];
    //        }
    //        return ret;
    //    }

    //    /// <summary>
    //    /// 檔案下載
    //    /// </summary>
    //    /// <param name="fileName"></param>
    //    /// <param name="type"></param>
    //    public void FileDownLoad(string fileName, FileType type) {
    //        /*-- 開啟執行緒準備接收檔案 --*/
    //        mSoxMonitorFile.Start();

    //        /*-- 向Server端發出檔案請求 --*/
    //        SendMsg($"Get:{type}:{fileName}");
    //        if (type == FileType.Map) {
    //            RaiseGoalSettingEvent(GoalSettingEventType.CurMapPath, true);
    //        } else {
    //            RaiseTestingEvent(TestingEventType.CurOriPath, true);
    //        }
    //        RaiseAgvClientEvent(AgvClientEventType.GetFile, type);
    //    }

    //    /// <summary>
    //    /// 傳送檔案
    //    /// </summary>
    //    public async void SendMap(string fileName) {
    //        string[] rtnMsg = SendMsg("Send:map");
    //        if (rtnMsg.Count() > 2 && rtnMsg[2] == "True") {
    //            await Task.Run(() => {
    //                if (!mBypassSocket) {
    //                    SendFile(mRemoteIP, mSendMapPort, fileName);
    //                } else {
    //                    /*-- 空跑模擬檔案傳送中 --*/
    //                    SpinWait.SpinUntil(() => false, 1000);
    //                }
    //                RaiseAgvClientEvent(AgvClientEventType.SendFile, fileName);
    //            });
    //        }
    //    }

    //    /// <summary>
    //    /// 設定車子位置
    //    /// </summary>
    //    /// <param name="x"></param>
    //    /// <param name="y"></param>
    //    /// <param name="theta"></param>
    //    public void SetPos(double x,double y ,double theta) {
    //        SendMsg($"Set:POS:{x:F0}:{y:F0}:{theta:F0}");
    //    }

    //    /// <summary>
    //    /// 取得雷射
    //    /// </summary>
    //    public void GetLaser() {
    //        /*-- 若是雷射資料則更新資料 --*/
    //        string[] rtnMsg = SendMsg("Get:Laser");
    //        if (rtnMsg.Length > 2) {
    //            if (rtnMsg[1] == "Laser") {
    //                string[] sreRemoteLaser = rtnMsg[2].Split(',');
    //                IEnumerable<int> LaserData = sreRemoteLaser.Select(x => int.Parse(x));
    //                RaiseMapEvent(MapEventType.GetLaser, LaserData);
    //            }
    //        }
    //    }

    //    #endregion Function - Public Methods

    //    #region Function - Private Methdos

    //    /// <summary>
    //    /// Send file of server to client
    //    /// </summary>
    //    /// <param name="clientIP">Ip address of client</param>
    //    /// <param name="clientPort">Communication port</param>
    //    /// <param name="fileName">File name</param>
    //    /// 
    //    public void SendFile(string clientIP, int clientPort, string fileName) {
    //        string curMsg = "";
    //        try {
    //            IPAddress[] ipAddress = Dns.GetHostAddresses(clientIP);
    //            IPEndPoint ipEnd = new IPEndPoint(ipAddress[0], clientPort);
    //            /* Make IP end point same as Server. */
    //            Socket clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
    //            /* Make a client socket to send data to server. */
    //            string filePath = "D:\\MapInfo\\";
    //            /* File reading operation. */
    //            fileName = fileName.Replace("\\", "/");
    //            while (fileName.IndexOf("/") > -1) {
    //                filePath += fileName.Substring(0, fileName.IndexOf("/") + 1);
    //                fileName = fileName.Substring(fileName.IndexOf("/") + 1);
    //            }
    //            byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
    //            if (fileNameByte.Length > 1024 * 1024 * 5) {
    //                curMsg = "File size is more than 850kb, please try with small file.";
    //                return;
    //            }
    //            curMsg = "Buffering ...";
    //            byte[] fileData = File.ReadAllBytes(filePath + fileName);
    //            /* Read & store file byte data in byte array. */
    //            byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
    //            /* clientData will store complete bytes which will store file name length, 
    //            file name & file data. */
    //            byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);
    //            /* File name length’s binary data. */
    //            fileNameLen.CopyTo(clientData, 0);
    //            fileNameByte.CopyTo(clientData, 4);
    //            fileData.CopyTo(clientData, 4 + fileNameByte.Length);
    //            /* copy these bytes to a variable with format line [file name length]
    //            [file name] [ file content] */
    //            curMsg = "Connection to server ...";
    //            clientSock.Connect(ipEnd);
    //            /* Trying to connection with server. */
    //            curMsg = "File sending...";
    //            clientSock.Send(clientData);
    //            /* Now connection established, send client data to server. */
    //            curMsg = "Disconnecting...";
    //            clientSock.Close();
    //            fileNameByte = null;
    //            clientData = null;
    //            fileNameLen = null;
    //            /* Data send complete now close socket. */
    //            curMsg = "File transferred.";
    //        } catch (Exception ex) {
    //            if (ex.Message == "No connection could be made because the target machine actively refused it")
    //                curMsg = "File Sending fail. Because server not running.";
    //            else
    //                curMsg = "File Sending fail." + ex.Message;
    //        }
    //    }


    //    /// <summary>
    //    /// 訊息傳送(會觸發事件)
    //    /// </summary>
    //    /// <param name="sendMseeage">傳送訊息內容</param>
    //    /// <param name="ckCnn">是否檢查連線狀態</param>
    //    /// <returns>Server端回應</returns>
    //    private string[] SendMsg(string sendMseeage, bool ckCnn = true) {
    //        if (mBypassSocket) {
    //            return new string[] { "True" };
    //        } else if (ckCnn && !IsServerAlive) {
    //            return new string[] { "False" };
    //        }

    //        /*-- 顯示發送出去的訊息 --*/
    //        string msg = $"{DateTime.Now} [Client] : {sendMseeage}\r\n";
    //        RaiseMsgTrans(msg);

    //        /*-- 等待Server端的回應 --*/
    //        string rtnMsg = SendStrMsg(mRemoteIP, mRecvCmdPort, sendMseeage);

    //        /*-- 顯示Server端回應 --*/
    //        msg = $"{DateTime.Now} [Server] : {rtnMsg}\r\n";
    //        RaiseMsgTrans(msg);

    //        return rtnMsg.Split(':');
    //    }

    //    /// <summary>
    //    /// 訊息傳送(具體Socket交握實現，但是不會觸發事件)
    //    /// </summary>
    //    /// <param name="serverIP">伺服端IP</param>
    //    /// <param name="requerPort">通訊埠號</param>
    //    /// <param name="sendMseeage">傳送訊息內容</param>
    //    /// <returns>Server端回應</returns>
    //    private string SendStrMsg(string serverIP, int requerPort, string sendMseeage) {

    //        //可以在字串編碼上做文章，可以傳送各種資訊內容，目前主要有三種編碼方式：
    //        //1.自訂連接字串編碼－－微量
    //        //2.JSON編碼--輕量
    //        //3.XML編碼--重量
    //        int state;
    //        int timeout = 5000;
    //        byte[] recvBytes = new byte[8192];//開啟一個緩衝區，存儲接收到的資訊

    //        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIP.ToString()), requerPort);
    //        Socket answerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    //        answerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
    //        answerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, timeout);
    //        try {

    //            answerSocket.Connect(ipEndPoint);//建立Socket連接
    //            byte[] sendContents = Encoding.UTF8.GetBytes(sendMseeage);
    //            state = answerSocket.Send(sendContents, sendContents.Length, 0);//發送二進位資料
    //            state = answerSocket.Receive(recvBytes);
    //            string strRecvCmd = Encoding.Default.GetString(recvBytes);//
    //            strRecvCmd = strRecvCmd.Split(new char[] { '\0' })[0];
    //            sendContents = null;
    //            return strRecvCmd;

    //        } catch (SocketException se) {
    //            Console.WriteLine("SocketException : {0}", se.ToString());
    //            //MessageBox.Show("目標拒絕連線!!");
    //            return "False";
    //        } catch (ArgumentNullException ane) {
    //            Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
    //            return "False";
    //        } catch (Exception ex) {
    //            Console.Write(ex.Message);
    //            return "False";
    //        } finally {
    //            ipEndPoint = null;
    //            recvBytes = null;
    //            // answerSocket.Shutdown(SocketShutdown.Both);
    //            // answerSocket.Disconnect(false);
    //            answerSocket.Close();
    //            // Console.Write("Disconnecting from server...\n");
    //            //Console.ReadKey();
    //            answerSocket.Dispose();
    //        }

    //    }

    //    /// <summary>
    //    /// 訊息傳輸發報
    //    /// </summary>
    //    /// <param name="msg">傳輸訊息內容</param>
    //    private void RaiseMsgTrans(string msg) {
    //        MsgTrans?.BeginInvoke(msg, null, null);
    //    }

    //    private void StartUp() {

    //    }

    //    #endregion Function - Private Methods
    //}
    
    /// <summary>
    /// 變數名稱定義
    /// </summary>
    internal static class VarDef {

        #region Declaration - Fields

        #region Variable Name Define

        /// <summary>
        /// Goal點集合
        /// </summary>
        internal static readonly string Goals = "Goals";

        /// <summary>
        /// 掃描點集合
        /// </summary>
        internal static readonly string ScanPoint = "ScanPoint";

        /// <summary>
        /// 車子資訊
        /// </summary>
        internal static readonly string CarInfo = "CarInfo";

        /// <summary>
        /// 障礙線段集合
        /// </summary>
        internal static readonly string ObstacleLines = "ObstacleLines";

        /// <summary>
        /// 障礙點集合
        /// </summary>
        internal static readonly string ObstaclePoints = "ObstaclePoints";

        /// <summary>
        /// 車子位置
        /// </summary>
        internal static readonly string PosCar = "PosCar";

        #endregion Variable Name Define

        /// <summary>
        /// 變數名稱與類型對照
        /// </summary>
        internal static readonly Dictionary<string, Type> NameTypeMap = new Dictionary<string, Type>() {
            { CarInfo,typeof(CarInfo) },
            { ObstacleLines,typeof(List<Line>)},
            { ObstaclePoints,typeof(List<Point>)},
            { ScanPoint,typeof(List<CartesianPos>) },
            { Goals,typeof(List<CartesianPos>)}
        };

        /// <summary>
        /// 類型與檢查方法對照
        /// </summary>
        internal static readonly Dictionary<Type, Func<object, bool>> TypeCheckMap = new Dictionary<Type, Func<object, bool>> {
            { typeof(CarInfo),obj => obj is CarInfo},
            { typeof(List<CartesianPos>),obj => obj is List<CartesianPos>},
            { typeof(List<Point>),obj => obj is List<Point>},
            { typeof(List<Line>),obj => obj is List<Line>}
        };

        #endregion Declaration - Fields

        #region Function - Extenstion

        /// <summary>
        /// 新增資料並且進行資料驗證
        /// </summary>
        /// <param name="dic">要加入的變數對照表</param>
        /// <param name="obj">要加入的變數</param>
        /// <param name="varName">變數名稱</param>
        internal static void Add(this Dictionary<string,object> dic,object obj ,string varName) {
            if (!NameTypeMap.ContainsKey(varName)) throw new ArgumentException($"變數名稱{varName}尚未定義");

            Type type = NameTypeMap[varName];
            if (!TypeCheckMap.ContainsKey(type)) throw new ArgumentException($"尚未定義{type}類型檢查方法");

            if (!TypeCheckMap[type](obj)) throw new ArgumentException($"變數名稱{varName}不符合{type}類型");

            dic.Add(varName, obj);
        }

        /// <summary>
        /// 驗證變數後讀取
        /// </summary>
        /// <typeparam name="T">要讀取的變數型態</typeparam>
        /// <param name="dic">要讀取的變數對照表</param>
        /// <param name="varName">要讀取的變數名稱</param>
        /// <returns></returns>
        internal static T ReadVar<T>(this Dictionary<string,object> dic ,string varName) {
            T var = default(T);
            if (!dic.ContainsKey(varName)) throw new ArgumentException($"無{varName}對應值");

            if (NameTypeMap[varName] != typeof(T)) throw new ArgumentException($"變數型態為{NameTypeMap[varName]}，與要求型態{typeof(T)}不符");
            if (dic.ContainsKey(varName) && NameTypeMap[varName] == typeof(T)) {
                var = (T)dic[varName];
            }
            return var;
        }

        #endregion Function - Extensition
    }
    
    #endregion Support Class

}
