using BroadCast;
using CtBind;
using CtLib.Module.Utility;
using Geometry;
using SerialCommunication;
using SerialCommunicationData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VehiclePlanner.Core {

    #region Declration - Enums

    /// <summary>
    /// 車子運動方向
    /// </summary>
    /// <remarks>
    /// 前後左右列舉值為鍵盤方向鍵之對應值
    /// 若是任意更改則會造成鍵盤控制發生例外
    /// </remarks>
    public enum MotionDirection {
        /// <summary>
        /// 往前
        /// </summary>
        Forward = 38,
        /// <summary>
        /// 往後
        /// </summary>
        Backward = 40,
        /// <summary>
        /// 左旋
        /// </summary>
        LeftTrun = 37,
        /// <summary>
        /// 右旋
        /// </summary>
        RightTurn = 39,
        /// <summary>
        /// 停止
        /// </summary>
        Stop = 4
    }

    /// <summary>
    /// 檔案類型
    /// </summary>
    public enum FileType {
        Ori,
        Map,
    }

    /// <summary>
    /// VehiclePlanner事件列舉
    /// </summary>
    public enum VehiclePlannerEvents {
        /// <summary>
        /// 標示物變更事件
        /// </summary>
        MarkerChanged,
        /// <summary>
        /// 清空地圖
        /// </summary>
        NewMap,
        /// <summary>
        /// 程式關閉
        /// </summary>
        Dispose,

    }

    #endregion Declaration - Enums

    #region Declaration - Delegates

    /// <summary>
    /// Console顯示訊息事件
    /// </summary>
    /// <param name="msg"></param>
    public delegate void ConsoleMessagEventHandler(string msg);

    /// <summary>
    /// 錯誤訊息事件
    /// </summary>
    /// <param name="err"></param>
    public delegate void ErrorMessageEventHandler(string err);

    /// <summary>
    /// 氣球提示事件
    /// </summary>
    /// <param name="title">標題</param>
    /// <param name="context">內容</param>
    public delegate void BalloonTipEventHandler(string title, string context);

    /// <summary>
    /// 檔案選擇方法委派
    /// </summary>
    /// <param name="fileList"></param>
    /// <returns></returns>
    public delegate string DelSelectFile(string fileList);

    /// <summary>
    /// 文字輸入方法委派
    /// </summary>
    /// <param name="oriName"></param>
    /// <param name="title"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public delegate bool DelInputBox(out string oriName, string title, string description);

    #endregion Declaration - Delegates

    #region Declaration - Const

    #endregion Declaration - Const

    #region Declaration - Interface

    /// <summary>
    /// iTS控制器
    /// </summary>
    public interface IBaseITSController:IDataSource {
        /// <summary>
        /// 資料是否自動回傳中
        /// </summary>
        bool IsAutoReport { get; }
        /// <summary>
        /// 是否已建立連線
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// 伺服馬達是否激磁
        /// </summary>
        bool IsMotorServoOn { get; }
        /// <summary>
        /// 是否掃描中
        /// </summary>
        bool IsScanning { get; }
        /// <summary>
        /// 是否可搜索
        /// </summary>
        bool IsSearchable { get; }
        /// <summary>
        /// 是否可連線
        /// </summary>
        bool IsConnectable { get; }
        /// <summary>
        /// 車子馬達速度
        /// </summary>
        int Velocity { get; set; }
        /// <summary>
        /// 檔案選擇方法委派
        /// </summary>
        DelSelectFile SelectFile { get; set; }
        /// <summary>
        /// 文字輸入方法委派
        /// </summary>
        DelInputBox InputBox { get; set; }
        /// <summary>
        /// 電池電量最大值
        /// </summary>
        double BatteryMaximum { get; }
        /// <summary>
        /// 電池電量最小值
        /// </summary>
        double BatteryMinimum { get; }
        /// <summary>
        /// Vehicle Console端IP
        /// </summary>
        string HostIP { get; set; }
        /// <summary>
        /// 是否Bypass Socket功能
        /// </summary>
        bool IsBypassSocket { get; set; }
        /// <summary>
        /// 可用iTS IP清單
        /// </summary>
        DataTable ITSs { get; }

        /// <summary>
        /// Console訊息事件
        /// </summary>
        event ConsoleMessagEventHandler ConsoleMessage;
        /// <summary>
        /// 氣球提示事件
        /// </summary>
        event BalloonTipEventHandler BalloonTip;

        /// <summary>
        /// 切換資料自動回傳
        /// </summary>
        void AutoReport(bool auto);
        /// <summary>
        /// 到指定充電站進行充電
        /// </summary>
        /// <param name="powerName">充電站名稱</param>
        /// <returns>是否開始進行充電</returns>
        void DoCharging(string powerName);
        /// <summary>
        /// 進行位置矯正
        /// </summary>
        /// <returns>地圖相似度</returns>
        void DoPositionComfirm();
        /// <summary>
        /// 移至Goal(透過Goal點索引)
        /// </summary>
        /// <param name="goalIndex">Goal點索引</param>
        /// <returns>是否成功開始移動</returns>
        void DoRunningByGoalName(string goalName);
        /// <summary>
        /// 搜尋至Goal點路徑(透過Goal索引)
        /// </summary>
        /// <param name="id"></param>
        void FindPath(string goalName);
        /// <summary>
        /// 顯示當前Goal點名稱清單
        /// </summary>
        void GetGoalNames();
        /// <summary>
        /// 取得Map檔
        /// </summary>
        void GetMap();
        /// <summary>
        /// 取得Ori檔
        /// </summary>
        void GetOri();
        /// <summary>
        /// 移動控制
        /// </summary>
        /// <param name="direction">移動方向</param>
        /// <param name="velocity">移動速度</param>
        void MotionContorl(MotionDirection direction);
        /// <summary>
        /// 要求雷射資料
        /// </summary>
        /// <returns>雷射資料(0筆雷射資料表示失敗)</returns>
        void RequestLaser();
        /// <summary>
        /// 要求Map檔清單
        /// </summary>
        /// <returns>Map檔清單</returns>
        string RequestMapList();
        void SendAndSetMap(string mapPath);
        /// <summary>
        /// 要求AGV設定新位置
        /// </summary>
        /// <param name="oldPosition">舊座標</param>
        /// <param name="newPosition">新座標</param>
        void SetServoMode(bool servoOn);
        /// <summary>
        /// 設定iTS工作移動速度
        /// </summary>
        /// <param name="velocity">移動速度</param>
        /// <returns>是否設定成功</returns>
        void SetWorkVelocity(int velocity);
        /// <summary>
        /// 開始掃描地圖
        /// </summary>
        /// <param name="scan">開始/停止掃描地圖</param>
        void StartScan(bool scan);
        /// <summary>
        /// 連線至iTS
        /// </summary>
        /// <param name="cnn">連線/斷線</param>
        void ConnectToITS(bool cnn);
        /// <summary>
        /// 搜尋可用的iTS設備
        /// </summary>
        void FindCar();

    }
    
    /// <summary>
    /// 車輛規劃器
    /// </summary>
    public interface IBaseVehiclePlanner : ICtVersion ,IDisposable,IDataSource{
        /// <summary>
        /// iTS控制器
        /// </summary>
        IBaseITSController Controller { get; }
        /// <summary>
        /// 當前Map檔路徑
        /// </summary>
        string CurMapPath { get; set; }
        /// <summary>
        /// 當前Ori檔路徑
        /// </summary>
        string CurOriPath { get; set; }
        /// <summary>
        /// 預設地圖存放路徑
        /// </summary>
        string DefMapDir { get; }
        /// <summary>
        /// 主畫面是否可視
        /// </summary>
        bool MainVisible { get; }
        /// <summary>
        /// 地圖中心點
        /// </summary>
        IPair MapCenter { get; set; }
        /// <summary>
        /// 使用者資料
        /// </summary>
        UserData UserData { get; set; }
        /// <summary>
        /// 是否Bypass LoadFile功能
        /// </summary>
        bool IsBypassLoadFile { get; set; }
        /// <summary>
        /// 全域鍵盤檢測
        /// </summary>
        KeyboardHook KeyboardHook { get; }
        /// <summary>
        /// 錯誤訊息事件
        /// </summary>
        event ErrorMessageEventHandler ErrorMessage;
        /// <summary>
        /// VehiclePlanner事件
        /// </summary>
        event EventHandler<VehiclePlannerEventArgs> VehiclePlannerEvent;

        /// <summary>
        /// 新增當前位置為Goal點
        /// </summary>
        void AddCurrentAsGoal();
        /// <summary>
        /// 清除地圖
        /// </summary>
        void ClearMap();
        /// <summary>
        /// 系統初始化
        /// </summary>
        void Initial();
        /// <summary>
        /// 載入檔案
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        void LoadFile(FileType type, string fileName);
        /// <summary>
        /// 儲存地圖
        /// </summary>
        void SaveMap();
        /// <summary>
        /// 將ori檔轉為map檔
        /// </summary>
        void SimplifyOri();

    }

    #endregion Declaration - Interface

    /// <summary>
    /// 系統定義
    /// </summary>
    public partial class BaseVehiclePlanner {

        #region Declaration - Fileds
        
        /// <summary>
        /// 地圖中心
        /// </summary>
        private IPair mMapCenter = null;

        /// <summary>
        /// 當前Map檔路徑
        /// </summary>
        protected string mCurMapPath = string.Empty;
        
        /// <summary>
        /// 主畫面是否可視
        /// </summary>
        private bool mMainVisible = true;

        /// <summary>
        /// 是否Bypass LoadFile功能
        /// </summary>
        protected bool mBypassLoadFile = false;

        /// <summary>
        /// 使用者操作權限
        /// </summary>
        private UserData mUserData = new UserData("CASTEC", "", AccessLevel.Administrator);

        ///<summary>全域鍵盤鉤子</summary>
        private KeyboardHook mKeyboardHook = new KeyboardHook();

        private IBaseITSController mITS = null;

        #endregion Declaration - Fields

        #region Declaration - Properties

        /// <summary>
        /// iTS控制器
        /// </summary>
        public IBaseITSController Controller { get => mITS;protected set {
                if (mITS != value && value != null) {
                    mITS = value;
                }
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
            }
        }

        /// <summary>
        /// 主畫面是否可視
        /// </summary>
        public bool MainVisible {
            get {
                return mMainVisible;
            }
            private set {
                if (mMainVisible != value) {
                    mMainVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否Bypass LoadFile功能
        /// </summary>
        public bool IsBypassLoadFile {
            get {
                return mBypassLoadFile;
            }
            set {
                if (mBypassLoadFile != value) {
                    mBypassLoadFile = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 地圖檔儲存路徑
        /// </summary>
        public string DefMapDir { get; private set; } = @"D:\MapInfo\";

        /// <summary>
        /// 使用者資料
        /// </summary>
        public UserData UserData {
            get {
                return mUserData;
            }
            set {
                if (mUserData != value && value != null) {
                    mUserData = value;
                    OnPropertyChanged();
                }
            }

        }

        /// <summary>
        /// 地圖中心點
        /// </summary>
        public IPair MapCenter {
            get {
                return mMapCenter;
            }
            set {
                if (mMapCenter != value && value != null) {
                    mMapCenter = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 全域鍵盤檢測
        /// </summary>
        public KeyboardHook KeyboardHook {
            get => mKeyboardHook;
        }

        #endregion Declaration - Porperties

        #region Declaration - Events
        /// <summary>
        /// 屬性變更通知事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// 錯誤訊息事件
        /// </summary>
        public event ErrorMessageEventHandler ErrorMessage = null;
       
        /// <summary>
        /// Console訊息事件
        /// </summary>
        public event ConsoleMessagEventHandler ConsoleMessage {
            add {
                mConsoleMessage += value;
                mITS.ConsoleMessage += value;
            }
            remove {
                mConsoleMessage -= value;
                mITS.ConsoleMessage -= value;
            }
        }
        /// <summary>
        /// Console訊息事件
        /// </summary>
        private event ConsoleMessagEventHandler mConsoleMessage = null;

        /// <summary>
        /// 氣球提示事件
        /// </summary>
        public event BalloonTipEventHandler BalloonTip {
            add {
                mBalloonTip += value;
                mITS.BalloonTip += value;
            }
            remove {
                mBalloonTip -= value;
                mITS.BalloonTip += value;
            }
        }        
        /// <summary>
        /// 氣球提示事件
        /// </summary>
        private event BalloonTipEventHandler mBalloonTip = null;

        /// <summary>
        /// VehiclePlanner事件
        /// </summary>
        public event EventHandler<VehiclePlannerEventArgs> VehiclePlannerEvent = null;
        
        #endregion Declaration - Events

    }

}
