using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CtLib.Library;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Data;
using BroadCast;
using System.Threading;

namespace VehiclePlanner.Core {

    
    /// <summary>
    /// iTS控制抽象定義
    /// </summary>
    public abstract class BaseiTSController : IBaseITSController {

        #region Declaration - Fields
    
        /// <summary>
        /// 回應等待逾時時間
        /// </summary>
        protected int mTimeOut = 1000;

        /// <summary>
        /// 地圖相似度，範圍0%～100%，超過門檻值為-100%
        /// </summary>
        protected double mSimilarity = 0;
        
        /// <summary>
        /// 資料是否自動回傳
        /// </summary>
        private bool mIsAutoReport = false;

        /// <summary>
        /// 是否在搜索中
        /// </summary>
        private bool mIsSearching = false;

        /// <summary>
        /// 車子馬達轉速
        /// </summary>
        private int mVelocity = 500;
        
        /// <summary>
        /// 伺服馬達激磁狀態
        /// </summary>
        private bool mIsMotorServoOn = false;

        /// <summary>
        /// 是否正在掃描中
        /// </summary>
        protected bool mIsScanning = false;

        /// <summary>
        /// 是否在手動移動
        /// </summary>
        private bool mIsManualMoving = false;

        /// <summary>
        /// 是否Bypass Socket通訊
        /// </summary>
        protected bool mBypassSocket = false;
        
        /// <summary>
        /// iTS位置名稱對照表
        /// </summary>
        private DataTable mAgvList = new DataTable("iTS");

        /// <summary>
        /// 廣播發送物件
        /// </summary>
        private Broadcaster mBroadcast = new Broadcaster();

        #endregion Declaration - Fields

        #region Declaration - Properties
        
        /// <summary>
        /// Vehicle Console端IP
        /// </summary>
        public string HostIP {
            get {
                return Properties.Settings.Default.HostIP;
            }
            set {
                if (Properties.Settings.Default.HostIP != value ) {
                    Properties.Settings.Default.HostIP = value;
                    Properties.Settings.Default.Save();
                    OnPropertyChanged();
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
            protected set {
                if (mIsAutoReport != value) {
                    mIsAutoReport = value;
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
            protected set {
                if (mVelocity != value) {
                    mVelocity = value;
                    OnConsoleMessage($"iTS - WorkVelocity = {mVelocity}");
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否掃描中
        /// </summary>
        public bool IsScanning {
            get {
                return mIsScanning;
            }
            protected set {
                if (mIsScanning != value) {
                    mIsScanning = value;
                    OnPropertyChanged();
                    OnConsoleMessage($"iTS - Is {(mIsScanning ? "start" : "stop")} scanning");
                }
            }
        }
        
        /// <summary>
        /// 電池最大電量
        /// </summary>
        public double BatteryMaximum { get; } = 100;

        /// <summary>
        /// 電池最小電量
        /// </summary>
        public double BatteryMinimum { get; } = 0;

        /// <summary>
        /// iTS IP清單
        /// </summary>
        public DataTable ITSs {
            get {
                /*-- 取得iTS IP清單 --*/
                return mAgvList;
            }
        }

        /// <summary>
        /// 是否已建立連線
        /// </summary>
        public abstract bool IsConnected { get; }

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
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否可搜索
        /// </summary>
        public bool IsSearchable {
            get => !mIsSearching && !IsConnected;
            protected set {
                mIsSearching = !value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsConnectable));
            }
        }

        /// <summary>
        /// 是否可連線
        /// </summary>
        public bool IsConnectable {
            get => !mIsSearching;
        }

        #endregion Declaration - Properties

        #region Declaration - Events

        /// <summary>
        /// 屬性變更通知事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Console訊息事件
        /// </summary>
        public event ConsoleMessagEventHandler ConsoleMessage = null;

        /// <summary>
        /// 氣球提示事件
        /// </summary>
        public event BalloonTipEventHandler BalloonTip = null;

        /// <summary>
        /// 廣播回覆接收事件
        /// </summary>
        public event EventHandler<BroadcastEventArgs> ReceivedBoradcast {
            add {
                mBroadcast.ReceivedBoradcast += value;
            }
            remove {
                mBroadcast.ReceivedBoradcast -= value;
            }
        }

        #endregion Declaration - Evnets

        #region Declaration - Delegates

        /// <summary>
        /// 檔案選擇方法委派
        /// </summary>
        public DelSelectFile SelectFile { get; set; } = null;

        /// <summary>
        /// 文字輸入方法委派
        /// </summary>
        public DelInputBox InputBox { get; set; } = null;

        #endregion Declaration - Delegates

        #region Funciton - Constructors

        public BaseiTSController() {
            /*-- 委派廣播接收事件 --*/
            mBroadcast.ReceivedBoradcast += mBroadcast_ReceivedData;

            mAgvList.Columns.Add("IP");
            mAgvList.Columns.Add("Description");

            HostIP = string.Empty;
        }

        #endregion Funciton - Constructors

        #region Function - Events

        /// <summary>
        /// 廣播接收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mBroadcast_ReceivedData(object sender, BroadcastEventArgs e) {
            /*-- 紀錄有回應的iTS IP位址 --*/
            string ip = e.Remote.Address.ToString();
            if (!mAgvList.AsEnumerable().Any(v => (v["IP"].ToString() == ip))) {
                DelInvoke?.Invoke(() => mAgvList.Rows.Add(ip, e.Message));
            }
        }

        #endregion Function - Events

        #region Funciotn - Public Methods

        /// <summary>
        /// 取得Map檔
        /// </summary>
        public abstract void GetMap();

        /// <summary>
        /// 取得Ori檔
        /// </summary>
        public abstract void GetOri();

        /// <summary>
        /// 要求雷射資料
        /// </summary>
        /// <returns>雷射資料(0筆雷射資料表示失敗)</returns>
        public abstract void RequestLaser();

        public abstract void SetServoMode(bool servoOn);

        /// <summary>
        /// 設定iTS工作移動速度
        /// </summary>
        /// <param name="velocity">移動速度</param>
        /// <returns>是否設定成功</returns>
        public abstract void SetWorkVelocity(int velocity);

        /// <summary>
        /// 進行位置矯正
        /// </summary>
        /// <returns>地圖相似度</returns>
        public abstract void DoPositionComfirm();

        /// <summary>
        /// 移至Goal(透過Goal點索引)
        /// </summary>
        /// <param name="goalIndex">Goal點索引</param>
        /// <returns>是否成功開始移動</returns>
        public abstract void DoRunningByGoalName(string goalName);

        /// <summary>
        /// 到指定充電站進行充電
        /// </summary>
        /// <param name="powerName">充電站名稱</param>
        /// <returns>是否開始進行充電</returns>
        public abstract void DoCharging(string powerName);

        /// <summary>
        /// 要求Map檔清單
        /// </summary>
        /// <returns>Map檔清單</returns>
        public abstract string RequestMapList();

        ///// <summary>
        ///// 要求Map檔
        ///// </summary>
        ///// <param name="mapName">要求的Map檔名</param>
        ///// <returns>Map檔</returns>
        //public abstract IDocument RequestMapFile(string mapName);

        public virtual void StartScan(bool scan) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 傳送並要求載入Map
        /// </summary>
        /// <param name="mapPath">目標Map檔路徑</param>
        public void SendAndSetMap(string mapPath) {
            var success = UploadMapToAGV(mapPath);
            string mapName = Path.GetFileName(mapPath);
            if (success == true) {
                OnBalloonTip("Upload", $"{mapName} is uploaded {(success == true ? "successfully" : "failed")} ");
                OnConsoleMessage($"iTS - The {mapName} uploaded");
                success = ChangeMap(mapName);
                if (success == true) {
                    OnConsoleMessage($"iTS - The {mapName} is now running");
                } else if (success == false) {
                    OnConsoleMessage($"iTS - The {mapName} failed to run");
                }
            } else if (success == false) {
                OnConsoleMessage($"iTS - The {mapName} upload failed");
            }
        }

        /// <summary>
        /// 切換資料自動回傳
        /// </summary>
        public abstract void AutoReport(bool auto);

        /// <summary>
        /// 移動控制
        /// </summary>
        /// <param name="direction">移動方向</param>
        /// <param name="velocity">移動速度</param>
        public void MotionContorl(MotionDirection direction) {
            try {
                bool? isManualMoving = null;
                if (direction == MotionDirection.Stop) {
                    isManualMoving = StartManualControl(false);
                } else {
                    if (SetManualVelocity(direction) == true) {
                        OnConsoleMessage($"iTS - is {direction},Velocity is {mVelocity}");
                        isManualMoving = StartManualControl(true);
                    }
                }
                if (isManualMoving != null && isManualMoving != mIsManualMoving) {
                    mIsManualMoving = (bool)isManualMoving;
                    OnConsoleMessage($"iTS - {(mIsManualMoving ? "Start" : "Stop")} moving");
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }
        /// <summary>
        /// 顯示當前Goal點名稱清單
        /// </summary>
        public void GetGoalNames() {
            try {
                var goalList = RequestGoalList();
                if (!string.IsNullOrEmpty(goalList)) {
                    OnConsoleMessage($"iTS - GoalNames:{goalList}");
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        public abstract void FindPath(string goalName);

        /// <summary>
        /// 連線至iTS
        /// </summary>
        /// <param name="cnn"></param>
        public abstract void ConnectToITS();

        public void FindCar() {
            if (!mBroadcast.IsReceiving) {
                Task.Run(() => {
                    /*-- 開啟廣播接收 --*/
                    IsSearchable = false;
                    mBroadcast.StartReceive(true);
                    OnConsoleMessage("[Planner]: Start searching iTS.");
                    /*-- 清除iTS清單 --*/
                    DelInvoke?.Invoke(() => mAgvList.Clear());
                    /*-- 廣播要求iTS回應 --*/
                    for (int i = 0; i < 3; i++) {
                        mBroadcast.Send("Count off");
                        Thread.Sleep(30);
                    }
                    /*-- 等待iTS回應完畢後停止接收回應 --*/
                    Thread.Sleep(2000);
                    mBroadcast.StartReceive(false);
                    IsSearchable = true;
                    int count = ITSs.Rows.Count;
                    if (count > 0) {
                        HostIP = mAgvList.Rows[0][0].ToString();
                    }
                    /*-- 反饋至UI --*/
                    string msg = $"Find {count} iTS";
                    OnConsoleMessage($"[Planner]:{msg}");
                    OnBalloonTip("Search iTS", msg);
                    OnPropertyChanged(nameof(ITSs));
                });
            }
        }

        #endregion Funciton - Public Methods

        #region Funciton - Private Methods

        /// <summary>
        /// Console訊息發報
        /// </summary>
        /// <param name="msg"></param>
        protected virtual void OnConsoleMessage(string msg) {
            ConsoleMessage?.Invoke(msg);
        }

        /// <summary>
        /// 屬性變更事件發報
        /// </summary>
        /// <param name="prop"></param>
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") {
            DelInvoke?.Invoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
        }

        protected virtual void OnBalloonTip(string title, string context) {
            BalloonTip?.Invoke(title, context);
        }
        
        ///<summary>IP驗證</summary>
        ///<param name="ip">要驗證的字串</param>
        ///<returns>True:合法IP/False:非法IP</returns>
        protected bool VerifyIP(string ip) {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        
        /// <summary>
        /// 要求Ori檔清單
        /// </summary>
        /// <returns>Ori檔清單</returns>
        protected abstract string RequestOriList();
        
        /// <summary>
        /// 上傳Map檔
        /// </summary>
        /// <param name="mapPath">要上傳的Map檔路徑</param>
        /// <returns>是否上傳成功</returns>
        protected abstract bool? UploadMapToAGV(string mapPath);

        /// <summary>
        /// 要求iTS載入指定的Map檔
        /// </summary>
        /// <param name="mapName">要載入的Map檔名</param>
        /// <returns>是否切換成功</returns>
        protected abstract bool? ChangeMap(string mapName);

        /// <summary>
        /// 要求Goal點清單
        /// </summary>
        /// <returns>Goal點清單</returns>
        protected abstract string RequestGoalList();
        
        /// <summary>
        /// 開始手動控制
        /// </summary>
        /// <param name="start">是否開始手動控制</param>
        /// <remarks>是否為手動控制狀態</remarks>
        protected abstract bool? StartManualControl(bool start);
        
        /// <summary>
        /// 停止掃描地圖
        /// </summary>
        /// <returns>是否在掃描中</returns>
        protected abstract bool? StopScanning();

        /// <summary>
        /// 設定地圖檔名
        /// </summary>
        /// <remarks>是否在掃描中</remarks>
        protected abstract bool? SetScanningOriFileName(string oriName);

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
            return SetManualVelocity(l, r);
        }

        protected abstract bool? SetManualVelocity(int leftVelocity, int rightVelocity);

        #endregion Funciton - Private Methdos

        #region Implement - IDataSource

        /// <summary>
            /// Invoke委派方法
            /// </summary>
        protected Action<MethodInvoker> mInvoke = null;

        /// <summary>
        /// 預設Invoke委派方法
        /// </summary>
        protected Action<MethodInvoker> mDefInvoke = invk => invk();

        /// <summary>
        /// Invoke委派方法
        /// </summary>
        public Action<MethodInvoker> DelInvoke {
            get => mInvoke;
            set {
                if (value != null) {
                    mInvoke = value;
                }
            }
        }

        #endregion Implement - IDataSource

    }

    /// <summary>
    /// 序列傳輸實作iTS控制
    /// </summary>
    //internal class ITSControllerSerial : BaseiTSController {

    //    #region Declaration - Fields

    //    /// <summary>
    //    /// 序列化傳輸物件
    //    /// </summary>
    //    private ISerialClient mSerialClient = null;

    //    /// <summary>
    //    /// 回應等待清單
    //    /// </summary>
    //    private List<CtTaskCompletionSource<IProductPacket>> mCmdTsk = new List<CtTaskCompletionSource<IProductPacket>>();

    //    #endregion Declaration - Fields

    //    #region Declaration - Properties

    //    /// <summary>
    //    /// 是否已建立連線
    //    /// </summary>
    //    public override bool IsConnected { get => mSerialClient?.Connected ?? false;}

    //    #endregion Declaration - Properties

    //    #region Funciotn - Constructors

    //    public ITSControllerSerial():base() {

    //    }

    //    #endregion Funciotn - Constructors

    //    #region Funciton - Events

    //    /// <summary>
    //    /// 序列化通訊接收
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    private void mSerialClient_ReceiveData(object sender, ReceiveDataEventArgs e) {
    //        if (e.Data is IProductPacket) {
    //            var product = e.Data as IProductPacket;
    //            /*-- 查詢是否有等待該封包 --*/
    //            var cmdSrc = mCmdTsk.Find(v => v.SerialNumber == product.SerialNumber);
    //            if (cmdSrc != null) {
    //                cmdSrc.SetResult(product);
    //            } else {
    //                switch (product.Purpose) {
    //                    case EPurpose.AutoReportLaser:
    //                        var laser = product.ToIAutoReportLaser().Product;
    //                        if (laser != null) {
    //                            DrawLaser(product.ToIAutoReportLaser().Product);
    //                        } else {
    //                            IsAutoReport = false;
    //                            mMapGL.ClearLaser();
    //                            mMapGL.ClearPath();
    //                        }
    //                        break;
    //                    case EPurpose.AutoReportPath: {
    //                            var path = product.ToIAutoReportPath().Product;
    //                            if (path != null) {
    //                                DrawPath(path);
    //                            }
    //                            break;
    //                        }
    //                    case EPurpose.AutoReportStatus:
    //                        Status = product.ToIAutoReportStatus()?.Product;
    //                        break;
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 連線狀態變更事件
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    private void mSerialClient_OnConnectChange(object sender, ConnectStatusChangeEventArgs e) {
    //        OnPropertyChanged(nameof(IsConnected));
    //        OnPropertyChanged(nameof(IsSearchable));
    //        OnConsoleMessage($"Client - Is {(e.IsConnected ? "Connected" : "Disconnected")} to {e.IP}:{e.Port}");
    //        if (e.IsConnected) {
    //            DoPositionComfirm();
    //            Status = RequestStatus();
    //        } else {
    //            if (mSerialClient != null) {
    //                mSerialClient.Dispose();
    //                mSerialClient = null;
    //            }
    //        }
    //    }
        
    //    #endregion Funciton - Events

    //    #region Funciton - Public Methods
        
    //    /// <summary>
    //    /// 與指定IP iTS連線/斷線
    //    /// </summary>
    //    /// <param name="cnn">連線/斷線</param>
    //    /// <param name="hostIP">AGV IP</param>
    //    public override void ConnectToITS(bool cnn) {
    //        try {
    //            if (IsConnected != cnn) {
    //                if (cnn) {//連線至VC
    //                    /*-- 實例化物件 --*/
    //                    if (mSerialClient != null) {
    //                        mSerialClient.ConnectChange -= mSerialClient_OnConnectChange;
    //                        mSerialClient.Dispose();
    //                    }
    //                    mSerialClient = FactoryMode.Factory.SerialClient(mSerialClient_ReceiveData, mBypassSocket);
    //                    mSerialClient.ConnectChange += mSerialClient_OnConnectChange;
    //                    /*-- IP格式驗證 --*/
    //                    if (!VerifyIP(HostIP)) {
    //                        throw new FormatException($"{HostIP}是錯誤IP格式");
    //                    }
    //                    /*-- 測試IP是否存在 --*/
    //                    PingStatus pingStt = PingStatus.Unknown;
    //                    if ((pingStt = CtNetwork.Ping(HostIP, 500).PingState) != PingStatus.Success) {
    //                        throw new PingException(pingStt.ToString());
    //                    }
    //                    /*-- 連線至VehicleConsole --*/
    //                    mSerialClient.Connect(HostIP, (int)EPort.VehiclePlanner);
    //                } else {//斷開與VehicleConsole的連線
    //                    mSerialClient.Stop();
    //                }
    //            }
    //        } catch (PingException pe) {
    //            OnConsoleMessage($"Ping fail:{pe.Message}");
    //            OnBalloonTip("Connect failed", pe.Message);
    //        } catch (Exception ex) {
    //            OnConsoleMessage(ex.Message);
    //            OnBalloonTip("Connect failed", ex.Message);
    //            if (mSerialClient != null) {
    //                mSerialClient.Dispose();
    //                mSerialClient = null;
    //            }
    //        }
    //    }
        
    //    /// <summary>
    //    /// 要求雷射資料
    //    /// </summary>
    //    /// <returns>雷射資料(0筆雷射資料表示失敗)</returns>
    //    public override void RequestLaser() {
    //        var laser = Send(FactoryMode.Factory.Order().RequestLaser())?.ToIRequestLaser()?.Product;
    //        try {
    //            if (laser != null) {
    //                if (laser.Count > 0) {
    //                    OnConsoleMessage($"iTS - Received {laser.Count} laser data");
    //                    DrawLaser(laser);
    //                } else {
    //                    OnConsoleMessage($"iTS - Laser data request failed");
    //                }
    //            }
    //        } catch (Exception ex) {
    //            OnConsoleMessage(ex.Message);
    //        }
    //    }

    //    public override void SetServoMode(bool servoOn) {
    //        try {
    //            var servoOnStt = Send(FactoryMode.Factory.Order().SetServoMode(servoOn))?.ToISetServoMode()?.Product;
    //            if (servoOnStt != null) {
    //                OnConsoleMessage($"iTS - Is Servo{(servoOn ? "On" : "Off")}");
    //                IsMotorServoOn = (bool)servoOnStt;
    //            }
    //        } catch (Exception ex) {
    //            OnConsoleMessage(ex.Message);
    //        }
    //    }

    //    /// <summary>
    //    /// 設定iTS工作移動速度
    //    /// </summary>
    //    /// <param name="velocity">移動速度</param>
    //    /// <returns>是否設定成功</returns>
    //    public override void SetWorkVelocity(int velocity) {
    //        var success = Send(FactoryMode.Factory.Order().SetWorkVelocity(velocity))?.ToISetWorkVelocity()?.Product;
    //        try {
    //            if (success == true) {
    //                Velocity = velocity;
    //            }
    //        } catch (Exception ex) {
    //            OnConsoleMessage(ex.Message);
    //        }
    //    }

    //    /// <summary>
    //    /// 進行位置矯正
    //    /// </summary>
    //    /// <returns>地圖相似度</returns>
    //    public override void DoPositionComfirm() {
    //        var similarity = Send(FactoryMode.Factory.Order().DoPositionComfirm())?.ToIDoPositionComfirm()?.Product;
    //        try {
    //            if (similarity != null) {
    //                if (similarity >= 0 && similarity <= 1) {
    //                    mSimilarity = (double)similarity;
    //                    OnConsoleMessage($"iTS - The map similarity is {mSimilarity:0.0%}");
    //                } else if (mSimilarity == -1) {
    //                    mSimilarity = (double)similarity;
    //                    OnConsoleMessage($"iTS - The map is now matched");
    //                } else {
    //                    OnConsoleMessage($"iTS - The map similarity is 0%");
    //                }
    //            }
    //        } catch (Exception ex) {
    //            OnConsoleMessage(ex.Message);
    //        }
    //    }

    //    /// <summary>
    //    /// 移至Goal(透過Goal點索引)
    //    /// </summary>
    //    /// <param name="goalIndex">Goal點索引</param>
    //    /// <returns>是否成功開始移動</returns>
    //    public override void DoRunningByGoalName(string goalName) {
    //        try {
    //            var success = Send(FactoryMode.Factory.Order().DoRuningByGoalName(goalName))?.ToIDoRuningByGoalName()?.Product;
    //            if (success == true) {
    //                OnConsoleMessage($"iTS - Start moving to {goalName}");
    //            } else if (success == false) {
    //                OnConsoleMessage($"Move to goal failure");
    //            }
    //        } catch (Exception ex) {
    //            OnConsoleMessage(ex.Message);
    //        }
    //    }

    //    /// <summary>
    //    /// 到指定充電站進行充電
    //    /// </summary>
    //    /// <param name="powerIndex">充電站索引</param>
    //    /// <returns>是否開始進行充電</returns>
    //    public override void DoCharging(string powerName) {
    //        try {
    //            var success = Send(FactoryMode.Factory.Order().DoCharging(powerName))?.ToIDoCharging()?.Product;
    //            if (success == true) {
    //                OnConsoleMessage($"iTS - Begin charging at {powerName}");
    //            } else if (success == false) {
    //                OnConsoleMessage("iTS - Charging failed");
    //            }
    //        } catch (Exception ex) {
    //            OnConsoleMessage(ex.Message);
    //        }
    //    }

    //    /// <summary>
    //    /// 要求Map檔清單
    //    /// </summary>
    //    /// <returns>Map檔清單</returns>
    //    public override string RequestMapList() {
    //        var mapList = Send(FactoryMode.Factory.Order().RequestMapList())?.ToIRequestMapList()?.Product;
    //        return mapList != null ? string.Join(",", mapList) : null;
    //    }

    //    /// <summary>
    //    /// 要求Map檔
    //    /// </summary>
    //    /// <param name="mapName">要求的Map檔名</param>
    //    /// <returns>Map檔</returns>
    //    public override IDocument RequestMapFile(string mapName) {
    //        var mapFile = Send(FactoryMode.Factory.Order().RequestMapFile(mapName))?.ToIRequestMapFile()?.Product;
    //        return mapFile;
    //    }
        
    //    #endregion Funciotn - Public Methods

    //    #region Funciton - Private Methods

    //    /// <summary>
    //    /// 序列傳輸命令
    //    /// </summary>
    //    /// <param name="packet">序列命令</param>
    //    /// <returns>是否傳輸成功</returns>
    //    private IProductPacket Send(IBasicPacket packet) {
    //        IProductPacket product = null;//回應封包
    //        Task tsk = null;//等待逾時執行緒
    //        CtTaskCompletionSource<IProductPacket> tskCompSrc = null;//封包接受完成觸發源
    //        /*-- 檢查封包 --*/
    //        if (packet == null) {
    //            OnConsoleMessage("The packet is null, unable to send");
    //            return null;
    //        }
    //        /*--檢查連線--*/
    //        if (!IsConnected) {
    //            OnConsoleMessage("Is not connected, unable to send");
    //            return null;
    //        }
    //        if (packet is IOrderPacket) {
    //            string cmdTitle = $"{packet.Purpose}({packet.SerialNumber}):";
    //            /*-- 檢查是否沒有在等待回應 --*/
    //            if (!mCmdTsk.Exists(v => v.Purpose == packet.Purpose)) {
    //                /*-- 加入回應等待任務 --*/
    //                tskCompSrc = new CtTaskCompletionSource<IProductPacket>(packet as IOrderPacket);
    //                mCmdTsk.Add(tskCompSrc);
    //                /*-- 等待回應 --*/
    //                tsk = Task.Run(() => {
    //                    bool isTimeout = !tskCompSrc.Task.Wait(mTimeOut);
    //                    /*--從等待清單中刪除--*/
    //                    mCmdTsk.Remove(tskCompSrc);
    //                    if (!isTimeout) {
    //                        if (tskCompSrc.Task.IsCompleted) {
    //                            if (tskCompSrc.Task.Result != null) {
    //                                product = tskCompSrc.Task.Result;
    //                                OnConsoleMessage($"{cmdTitle} response is received");
    //                            } else {
    //                                OnConsoleMessage($"{cmdTitle} response is null");
    //                            }
    //                        }
    //                    } else {
    //                        OnConsoleMessage($"{cmdTitle} response timeout");
    //                    }
    //                });
    //            } else {//已在等待回應
    //                OnConsoleMessage($"{cmdTitle}Waiting for the iTS to respond");
    //                return null;
    //            }
    //        }
    //        /*-- 發送命令 --*/
    //        if (!mSerialClient.Send(packet)) tskCompSrc?.SetResult(null);
    //        /*-- 等回應接收完畢 --*/
    //        tsk?.Wait();
    //        return product;
    //    }

    //    /// <summary>
    //    /// 要求iTS狀態
    //    /// </summary>
    //    /// <returns>iTS狀態</returns>
    //    protected override IStatus RequestStatus() {
    //        var status = Send(FactoryMode.Factory.Order().RequestStatus())?.ToIRequestStatus()?.Product;
    //        return status;
    //    }

    //    /// <summary>
    //    /// 要求Ori檔清單
    //    /// </summary>
    //    /// <returns>Ori檔清單</returns>
    //    protected override string RequestOriList() {
    //        var oriList = Send(FactoryMode.Factory.Order().RequestOriList())?.ToIRequestOriList()?.Product;
    //        return oriList != null ? string.Join(",", oriList) : null;
    //    }

    //    /// <summary>
    //    /// 要求Ori檔
    //    /// </summary>
    //    /// <param name="oriName">Ori檔名</param>
    //    /// <returns>Ori檔</returns>
    //    protected override IDocument RequestOriFile(string oriName) {
    //        var oriFile = Send(FactoryMode.Factory.Order().RequestOriFile(oriName))?.ToIRequestOriFile()?.Product;
    //        return oriFile;
    //    }

    //    /// <summary>
    //    /// 上傳Map檔
    //    /// </summary>
    //    /// <param name="mapPath">要上傳的Map檔路徑</param>
    //    /// <returns>是否上傳成功</returns>
    //    protected override bool? UploadMapToAGV(string mapPath) {
    //        var success = Send(FactoryMode.Factory.Order().UploadMapToAGV(mapPath))?.ToIUploadMapToAGV()?.Product;
    //        return success;
    //    }

    //    /// <summary>
    //    /// 要求iTS載入指定的Map檔
    //    /// </summary>
    //    /// <param name="mapName">要載入的Map檔名</param>
    //    /// <returns>是否切換成功</returns>
    //    protected override bool? ChangeMap(string mapName) {
    //        var success = Send(FactoryMode.Factory.Order().ChangeMap(mapName))?.ToIChangeMap()?.Product;
    //        return success;
    //    }

    //    /// <summary>
    //    /// 要求Goal點清單
    //    /// </summary>
    //    /// <returns>Goal點清單</returns>
    //    protected override string RequestGoalList() {
    //        var goalList = Send(FactoryMode.Factory.Order().RequestGoalList())?.ToIRequestGoalList()?.Product;
    //        return goalList != null ? string.Join(",", goalList) : null;
    //    }

    //    /// <summary>
    //    /// 要求到指定Goal點的路徑
    //    /// </summary>
    //    /// <param name="goalIndex">目標Goal點索引</param>
    //    /// <returns>路徑(Count為0表示規劃失敗)</returns>
    //    protected override List<IPair> RequestPath(string goalName) {
    //        var path = Send(FactoryMode.Factory.Order().RequestPath(goalName))?.ToIRequestPath()?.Product;
    //        return path;
    //    }

    //    /// <summary>
    //    /// 要求自動回報iTS狀態
    //    /// </summary>
    //    /// <param name="on">是否自動回報</param>
    //    /// <returns>iTS狀態</returns>
    //    protected override IStatus AutoReportStatus(bool on) {
    //        var status = Send(FactoryMode.Factory.Order().AutoReportStatus(on))?.ToIAutoReportStatus()?.Product;
    //        return status;
    //    }

    //    /// <summary>
    //    /// 要求自動回傳雷射資料
    //    /// </summary>
    //    /// <param name="on">是否自動回報</param>
    //    /// <returns>雷射資料</returns>
    //    protected override List<IPair> AutoReportLaser(bool on) {
    //        var laser = Send(FactoryMode.Factory.Order().AutoReportLaser(on))?.ToIAutoReportLaser()?.Product;
    //        return laser;
    //    }

    //    /// <summary>
    //    /// 要求自動回傳路徑
    //    /// </summary>
    //    /// <param name="on">是否自動回報</param>
    //    /// <returns>路徑資料</returns>
    //    protected override List<IPair> AutoReportPath(bool on) {
    //        var path = Send(FactoryMode.Factory.Order().AutoReportPath(on))?.ToIAutoReportPath()?.Product;
    //        return path;
    //    }

    //    /// <summary>
    //    /// 設定iTS當前位置
    //    /// </summary>
    //    /// <returns>是否設定成功</returns>
    //    protected override bool? SetPosition(ITowardPair position) {
    //        var success = Send(FactoryMode.Factory.Order().SetPosition(position))?.ToISetPosition()?.Product;
    //        return success;
    //    }

    //    /// <summary>
    //    /// 開始手動控制
    //    /// </summary>
    //    /// <param name="start">是否開始手動控制</param>
    //    /// <remarks>是否為手動控制狀態</remarks>
    //    protected override bool? StartManualControl(bool start) {
    //        var isManual = Send(FactoryMode.Factory.Order().StartManualControl(start))?.ToIStartManualControl()?.Product;
    //        return isManual;
    //    }

    //    /// <summary>
    //    /// 設定手動控制移動速度(方向)
    //    /// </summary>
    //    /// <param name="velocity">手動移動速度</param>
    //    /// <returns>是否設定成功</returns>
    //    protected override bool? SetManualVelocity(IPair velocity) {
    //        var success = Send(FactoryMode.Factory.Order().SetManualVelocity(velocity))?.ToISetManualVelocity()?.Product;
    //        return success;
    //    }

    //    /// <summary>
    //    /// 停止掃描地圖
    //    /// </summary>
    //    /// <returns>是否在掃描中</returns>
    //    protected override bool? StopScanning() {
    //        var isScanning = Send(FactoryMode.Factory.Order().StopScanning())?.ToIStopScanning()?.Product;
    //        return isScanning;
    //    }

    //    /// <summary>
    //    /// 設定地圖檔名
    //    /// </summary>
    //    /// <remarks>是否在掃描中</remarks>
    //    protected override bool? SetScanningOriFileName(string oriName) {
    //        var isScanning = Send(FactoryMode.Factory.Order().SetScanningOriFileName(oriName))?.ToISetScanningOriFileName()?.Product;
    //        return isScanning;
    //    }
    //    #endregion Funciotn - Private Methods

    //}

}
