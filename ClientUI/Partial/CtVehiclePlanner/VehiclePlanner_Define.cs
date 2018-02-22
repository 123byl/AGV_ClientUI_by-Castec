using BroadCast;
using CtLib.Module.Utility;
using Geometry;
using GLCore;
using SerialCommunication;
using SerialCommunicationData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VehiclePlanner.Partial.CtVehiclePlanner;

namespace VehiclePlanner {

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
    /// 檔案選擇方法委派
    /// </summary>
    /// <param name="fileList"></param>
    /// <returns></returns>
    public delegate string DelSelectFile(string fileList);

    #endregion Declaration - Delegates

    #region Declaration - Const

    /// <summary>
    /// 屬性名稱宣告
    /// </summary>
    public static class PropertyDeclaration {
        public const string iTSs = "iTSs";
        public const string MainVisible = "MainVisible";
        public const string IsMotorServoOn = "IsMotorServoOn";
        public const string IsConnected = "IsConnected";
        public const string IsScanning = "IsScanning";
        public const string Status = "Status";
        public const string IsAutoReport = "IsAutoReport";
        public const string Velocity = "Velocity";
        public const string MapCenter = "MapCenter";
        public const string IsBypassSocket = "IsBypassSocket";
        public const string IsBypassLoadFile = "IsBypassLoadFile";
        public const string HostIP = "HostIP";
        public const string UserData = "UserData";

        public const string Culture = "Culture";
    }

    #endregion Declaration - Const
    
    public partial class CtVehiclePlanner {

        #region Declaration - Fileds

        /// <summary>
        /// 回應等待逾時時間
        /// </summary>
        private int mTimeOut = 1000;

        /// <summary>
        /// 序列化傳輸物件
        /// </summary>
        private ISerialClient mSerialClient = null;

        /// <summary>
        /// 回應等待清單
        /// </summary>
        private List<CtTaskCompletionSource<IProductPacket>> mCmdTsk = new List<CtTaskCompletionSource<IProductPacket>>();

        /// <summary>
        /// 是否正在掃描中
        /// </summary>
        private bool mIsScanning = false;

        /// <summary>
        /// iTS狀態
        /// </summary>
        private IStatus mStatus;

        /// <summary>
        /// 地圖相似度，範圍0%～100%，超過門檻值為-100%
        /// </summary>
        protected double mSimilarity = 0;

        /// <summary>
        /// 車子馬達轉速
        /// </summary>
        protected int mVelocity = 500;

        /// <summary>
        /// 資料是否自動回傳
        /// </summary>
        private bool mIsAutoReport = false;

        /// <summary>
        /// 地圖中心
        /// </summary>
        private IPair mMapCenter = null;

        /// <summary>
        /// 當前Map檔路徑
        /// </summary>
        private string mCurMapPath = string.Empty;

        /// <summary>
        /// 是否在手動移動
        /// </summary>
        private bool mIsManualMoving = false;

        /// <summary>
        /// Vehicle Console端IP
        /// </summary>
        public string HostIP {
            get {
                return Properties.Settings.Default.HostIP;
            }
            private set {
                if (Properties.Settings.Default.HostIP != value && !string.IsNullOrEmpty(value)) {
                    Properties.Settings.Default.HostIP = value;
                    Properties.Settings.Default.Save();
                    OnPropertyChanged(PropertyDeclaration.HostIP);
                }
            }
        }

        /// <summary>
        /// AGV ID
        /// </summary>
        protected uint mAGVID = 1;

        /// <summary>
        /// iTS位置名稱對照表
        /// </summary>
        private Dictionary<IPAddress, string> mAgvList = new Dictionary<IPAddress, string>();

        /// <summary>
        /// 主畫面是否可視
        /// </summary>
        private bool mMainVisible = true;

        /// <summary>
        /// 伺服馬達激磁狀態
        /// </summary>
        private bool mIsMotorServoOn = false;

        /// <summary>
        /// 是否Bypass Socket通訊
        /// </summary>
        protected bool mBypassSocket = false;

        /// <summary>
        /// 是否Bypass LoadFile功能
        /// </summary>
        private bool mBypassLoadFile = false;

        /// <summary>
        /// 使用者操作權限
        /// </summary>
        private UserData mUserData = new UserData("CASTEC", "", AccessLevel.Administrator);

        /// <summary>
        /// 廣播發送物件
        /// </summary>
        private Broadcaster mBroadcast = new Broadcaster();

        ///<summary>全域鍵盤鉤子</summary>
        private KeyboardHook mKeyboardHook = new KeyboardHook();

        #endregion Declaration - Fields

        #region Declaration - Properties

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
        /// iTS IP清單
        /// </summary>
        public List<string> iTSs {
            get {
                /*-- 取得iTS IP清單 --*/
                return mAgvList.Keys.ToList().ConvertAll(v => v.ToString());
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
                    OnPropertyChanged(PropertyDeclaration.MainVisible);
                }
            }
        }

        /// <summary>
        /// 伺服馬達是否激磁
        /// </summary>
        public bool IsMotorServoOn {
            get {
                return mIsMotorServoOn;
            }
            set {
                if (mIsMotorServoOn != value) {
                    mIsMotorServoOn = value;
                    OnPropertyChanged(PropertyDeclaration.IsMotorServoOn);
                }
            }
        }

        /// <summary>
        /// 是否Bypass Socket功能
        /// </summary>
        public bool IsBypassSocket {
            get {
                return mBypassSocket;
            }
            set {
                if (mBypassSocket != value) {
                    mBypassSocket = value;
                    OnPropertyChanged(PropertyDeclaration.IsBypassSocket);
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
                    OnPropertyChanged(PropertyDeclaration.IsBypassLoadFile);
                }
            }
        }

        /// <summary>
        /// 地圖檔儲存路徑
        /// </summary>
        public string DefMapDir { get; private set; } = @"D:\MapInfo\";

        /// <summary>
        /// 使用者操作權限
        /// </summary>
        public UserData UserData {
            get {
                return mUserData;
            }
            set {
                if (mUserData != value && value != null) {
                    mUserData = value;
                    OnPropertyChanged(PropertyDeclaration.UserData);
                }
            }

        }

        /// <summary>
        /// 是否已建立連線
        /// </summary>
        public virtual bool IsConnected {
            get {
                return mSerialClient?.Connected ?? false;
            }
        }

        /// <summary>
        /// 是否掃描中
        /// </summary>
        public bool IsScanning {
            get {
                return mIsScanning;
            }
            private set {
                if (mIsScanning != value) {
                    mIsScanning = value;
                    OnPropertyChanged(PropertyDeclaration.IsScanning);
                    OnConsoleMessage($"iTS - Is {(mIsScanning ? "start" : "stop")} scanning");
                }
            }
        }

        /// <summary>
        /// iTS狀態
        /// </summary>
        public IStatus Status {
            get {
                return mStatus;
            }
            private set {
                if (value != null && mStatus != value) {
                    mStatus = value;
                    OnPropertyChanged(PropertyDeclaration.Status);
                    Database.AGVGM[mAGVID].SetLocation(mStatus.Data);
                }
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
                if (mVelocity != value) {
                    mVelocity = value;
                    OnPropertyChanged(PropertyDeclaration.Velocity);
                    OnConsoleMessage($"iTS - WorkVelocity = {mVelocity}");
                }
            }
        }

        /// <summary>
        /// 資料是否自動回傳中
        /// </summary>
        public bool IsAutoReport {
            get {
                return mIsAutoReport;
            }
            private set {
                if (mIsAutoReport != value) {
                    mIsAutoReport = value;
                    OnPropertyChanged(PropertyDeclaration.IsAutoReport);
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
                    OnPropertyChanged(PropertyDeclaration.MapCenter);
                }
            }
        }

        #endregion Declaration - Porperties

        #region Declaration - Events

        /// <summary>
        /// 屬性變更通知事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = null;

        /// <summary>
        /// Console訊息事件
        /// </summary>
        public event ConsoleMessagEventHandler ConsoleMessage = null;

        /// <summary>
        /// 錯誤訊息事件
        /// </summary>
        public event ErrorMessageEventHandler ErrorMessage = null;

        /// <summary>
        /// VehiclePlanner事件
        /// </summary>
        public event EventHandler<VehiclePlannerEventArgs> VehiclePlannerEvent = null;

        #endregion Declaration - Events

        #region Declaration - Delegates

        /// <summary>
        /// 檔案選擇方法委派
        /// </summary>
        public DelSelectFile SelectFile = null;

        #endregion Declaration - Delegates
    }

}
