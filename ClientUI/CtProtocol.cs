using AGVDefine;
using BroadCast;
using CtLib.Forms;
using CtLib.Library;
using Geometry;
using GLCore;
using SerialCommunication;
using SerialCommunicationData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace VehiclePlanner {
    public partial class CtVehiclePlanner {

        #region Declaration  - Fields

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

        #endregion Declaration - Fields

        #region Declaration - Properties

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

        #endregion Declaration - Properties

        #region Declration - Delegate

        /// <summary>
        /// 檔案選擇方法委派
        /// </summary>
        public DelSelectFile SelectFile = null;

        #endregion Declaration - Delegate

        #region Funciton - Events

        /// <summary>
        /// 序列化通訊接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSerialClient_ReceiveData(object sender, ReceiveDataEventArgs e) {
            if (e.Data is IProductPacket) {
                var product = e.Data as IProductPacket;
                /*-- 查詢是否有等待該封包 --*/
                var cmdSrc = mCmdTsk.Find(v => v.SerialNumber == product.SerialNumber);
                if (cmdSrc != null) {
                    cmdSrc.SetResult(product);
                }else {
                    switch (product.Purpose) {
                        case EPurpose.AutoReportLaser:
                            var laser = product.ToIAutoReportLaser().Product;
                            if (laser != null) {
                                DrawLaser(product.ToIAutoReportLaser().Product);
                            } else {
                                IsAutoReport = false;
                                Database.AGVGM[mAGVID].LaserAPoints.DataList.Clear();
                                Database.AGVGM[mAGVID].Path.DataList.Clear();
                            }
                            break;
                        case EPurpose.AutoReportPath: {
                                var path = product.ToIAutoReportPath().Product;
                                if (path != null) {
                                    DrawPath(path);
                                }
                                break;
                            }
                        case EPurpose.AutoReportStatus:
                            Status = product.ToIAutoReportStatus()?.Product;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 連線狀態變更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSerialClient_OnConnectChange(object sender, ConnectStatusChangeEventArgs e) {
            OnPropertyChanged(PropertyDeclaration.IsConnected);
            
            OnConsoleMessage($"Client - Is {(e.IsConnected ? "Connected" : "Disconnected")} to {e.IP}:{e.Port}");
            if (e.IsConnected) {
                HostIP = e.IP;
                DoPositionComfirm();
                Status = RequestStatus();
            }else {
                if (mSerialClient != null) {
                    mSerialClient.Dispose();
                    mSerialClient = null;
                }
            }
        }

        #endregion Funciton - Evnets

        #region Function - Private Methods

        #region SerialClient - Operating

        /// <summary>
        /// 與指定IP iTS連線/斷線
        /// </summary>
        /// <param name="cnn">連線/斷線</param>
        /// <param name="hostIP">AGV IP</param>
        /// <exception cref=""
        public virtual void ConnectToITS(bool cnn, string hostIP = "") {
            try {
                if (IsConnected != cnn) {
                    if (cnn) {//連線至VC
                        /*-- 實例化物件 --*/
                        if (mSerialClient == null) {
                            mSerialClient = FactoryMode.Factory.SerialClient(mSerialClient_ReceiveData, mBypassSocket);
                            mSerialClient.ConnectChange += mSerialClient_OnConnectChange;
                        }
                        /*-- IP格式驗證 --*/
                        if (!VerifyIP(hostIP)) {
                            throw new FormatException($"{hostIP}是錯誤IP格式");
                        }
                        /*-- 測試IP是否存在 --*/
                        PingStatus pingStt = PingStatus.Unknown;
                        if ((pingStt = CtNetwork.Ping(hostIP, 500).PingState) != PingStatus.Success) {
                            throw new PingException(pingStt.ToString());
                        }
                        /*-- 連線至VehicleConsole --*/
                        mSerialClient.Connect(hostIP, (int)EPort.VehiclePlanner);
                    } else {//斷開與VehicleConsole的連線
                        mSerialClient.Stop();
                    }
                }
            } catch (PingException pe) {
                OnConsoleMessage($"Ping fail:{pe.Message}");
                SetBalloonTip("Connect failed", pe.Message);
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
                SetBalloonTip("Connect failed", ex.Message);
                if (mSerialClient != null) {
                    mSerialClient.Dispose();
                    mSerialClient = null;
                }
            }
        }

        /// <summary>
        /// 序列傳輸命令
        /// </summary>
        /// <param name="packet">序列命令</param>
        /// <returns>是否傳輸成功</returns>
        private IProductPacket Send(IBasicPacket packet) {
            IProductPacket product = null;//回應封包
            Task tsk = null;//等待逾時執行緒
            CtTaskCompletionSource<IProductPacket> tskCompSrc = null;//封包接受完成觸發源
            /*-- 檢查封包 --*/
            if (packet == null) {
                OnConsoleMessage("The packet is null, unable to send");
                return null;
            }
            /*--檢查連線--*/
            if (!IsConnected) {
                OnConsoleMessage("Is not connected, unable to send");
                return null;
            }
            if (packet is IOrderPacket) {
                string cmdTitle = $"{packet.Purpose}({packet.SerialNumber}):";
                /*-- 檢查是否沒有在等待回應 --*/
                if (!mCmdTsk.Exists(v => v.Purpose == packet.Purpose)) {
                    /*-- 加入回應等待任務 --*/
                    tskCompSrc = new CtTaskCompletionSource<IProductPacket>(packet as IOrderPacket);
                    mCmdTsk.Add(tskCompSrc);
                    /*-- 等待回應 --*/
                    tsk = Task.Run(() => {
                        bool isTimeout = !tskCompSrc.Task.Wait(mTimeOut);
                        /*--從等待清單中刪除--*/
                        mCmdTsk.Remove(tskCompSrc);
                        if (!isTimeout) {
                            if (tskCompSrc.Task.IsCompleted) {
                                if (tskCompSrc.Task.Result != null) {
                                    product = tskCompSrc.Task.Result;
                                    OnConsoleMessage($"{cmdTitle} response is received");
                                } else {
                                    OnConsoleMessage($"{cmdTitle} response is null");
                                }
                            }
                        } else {
                            OnConsoleMessage($"{cmdTitle} response timeout");
                        }
                    });
                } else {//已在等待回應
                    OnConsoleMessage($"{cmdTitle}Waiting for the iTS to respond");
                    return null;
                }
            }
            /*-- 發送命令 --*/
            if (!mSerialClient.Send(packet)) tskCompSrc?.SetResult(null);
            /*-- 等回應接收完畢 --*/
            tsk?.Wait();
            return product;
        }

        #endregion SerialClient - Operating

        #region Methods

        /// <summary>
        /// 載入地圖
        /// </summary>
        /// <param name="mapPath">Map檔路徑</param>
        private bool LoadMap(string mapPath) {
            bool isLoaded = true;
            mCurMapPath = mapPath;
            string path = CtFile.GetFileName(mapPath);
            if (mBypassLoadFile) {
                /*-- 空跑1秒模擬載入Map檔 --*/
                SpinWait.SpinUntil(() => false, 1000);
            } else {
                //#region - Retrive information from .map file -
                /*-- 地圖清空 --*/
                OnVehiclePlanner(VehiclePlannerEvents.NewMap);
                /*-- 載入Map並取得Map中心點 --*/
                var center = Database.LoadMapToDatabase(mCurMapPath)?.Center();
                if (center != null) {
                    MapCenter = center;
                    OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
                    if (IsConnected) {
                        SendAndSetMap(mapPath);
                    }
                }else {
                    isLoaded = false;
                }
            }
            return isLoaded;
        }

        private bool LoadOri(string oriPath) {
            bool isLoaded = true;
            CurOriPath = oriPath;
            OnVehiclePlanner(VehiclePlannerEvents.NewMap);
            if (!mBypassLoadFile) {//無BypassLoadFile
                /*-- 載入Map並取得Map中心點 --*/
                IPair center = Database.LoadOriToDatabase(CurOriPath, mAGVID)?.Center();
                if (center != null) {
                    MapCenter = center;
                }else {
                    isLoaded = false;
                }
            } else {//Bypass LoadFile功能
                    /*-- 空跑一秒，模擬檔案載入 --*/
                SpinWait.SpinUntil(() => false, 1000);
            }
            return isLoaded;
        }

        public void LoadFile(FileType type,string fileName) {
            try {
                bool isLoaded = false;
                switch (type) {
                    case FileType.Ori:
                        isLoaded = LoadOri(fileName);
                        if (isLoaded) {
                            Database.AGVGM[mAGVID]?.LaserAPoints.DataList.Clear();
                        }
                        break;
                    case FileType.Map:
                        isLoaded = LoadMap(fileName);
                        break;
                    default:
                        throw new ArgumentException($"無法載入未定義的檔案類型{type}");
                }
                if (isLoaded) {
                    SetBalloonTip($"Load { type}", $"\'{fileName}\' is loaded");
                } else {
                    OnErrorMessage("File data is wrong, can not read");
                }
            } catch (Exception ex) {
                OnErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 取得Ori檔
        /// </summary>
        public void GetOri() {
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
                                OnConsoleMessage($"Planner - {ori.Name} download completed");
                            } else {
                                success = false;
                                OnConsoleMessage($"Planner - {ori.Name} failed to save");
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            } finally {
                if (success != null) {
                    SetBalloonTip("Donwload", $"{oriName}.ori is downloaded {(success == true ? "successfully" : "failed")} ");
                }
            }
        }

        /// <summary>
        /// 取得Map檔
        /// </summary>
        public void GetMap() {
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
                                OnConsoleMessage($"Planner - {map.Name} download completed");
                            } else {
                                success = false;
                                OnConsoleMessage($"Planner - {map.Name} failed to save ");
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            } finally {
                if (success != null) {
                    SetBalloonTip("Donwload", $"{mapName}.ori is downloaded {(success == true ? "successfully" : "failed")} ");
                }
            }
        }
        

        /// <summary>
        /// 要求iTS狀態
        /// </summary>
        /// <returns>iTS狀態</returns>
        private IStatus RequestStatus() {
            var status = Send(FactoryMode.Factory.Order().RequestStatus())?.ToIRequestStatus()?.Product;
            return status;
        }

        /// <summary>
        /// 要求雷射資料
        /// </summary>
        /// <returns>雷射資料(0筆雷射資料表示失敗)</returns>
        public void RequestLaser() {
            var laser = Send(FactoryMode.Factory.Order().RequestLaser())?.ToIRequestLaser()?.Product;
            try {
                if (laser != null) {
                    if (laser.Count > 0) {
                        OnConsoleMessage($"iTS - Received {laser.Count} laser data");
                        DrawLaser(laser);
                    } else {
                        OnConsoleMessage($"iTS - Laser data request failed");
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }
        
        public void SetServoMode(bool servoOn) {
            try {
                var servoOnStt = Send(FactoryMode.Factory.Order().SetServoMode(servoOn))?.ToISetServoMode()?.Product;
                if (servoOnStt != null) {
                    OnConsoleMessage($"iTS - Is Servo{(servoOn ? "On" : "Off")}");
                    IsMotorServoOn = (bool)servoOnStt;
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 設定iTS工作移動速度
        /// </summary>
        /// <param name="velocity">移動速度</param>
        /// <returns>是否設定成功</returns>
        public void SetWorkVelocity(int velocity) {
            var success = Send(FactoryMode.Factory.Order().SetWorkVelocity(velocity))?.ToISetWorkVelocity()?.Product;
            try {
                if (success == true) {
                    Velocity = velocity;
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 設定iTS當前位置
        /// </summary>
        /// <returns>是否設定成功</returns>
        private bool? SetPosition(ITowardPair position) {
            var success = Send(FactoryMode.Factory.Order().SetPosition(position))?.ToISetPosition()?.Product;
            return success;
        }

        /// <summary>
        /// 開始手動控制
        /// </summary>
        /// <param name="start">是否開始手動控制</param>
        /// <remarks>是否為手動控制狀態</remarks>
        private bool? StartManualControl(bool start) {
            var isManual = Send(FactoryMode.Factory.Order().StartManualControl(start))?.ToIStartManualControl()?.Product;
            return isManual;
        }

        /// <summary>
        /// 設定手動控制移動速度(方向)
        /// </summary>
        /// <param name="velocity">手動移動速度</param>
        /// <returns>是否設定成功</returns>
        private bool? SetManualVelocity(IPair velocity) {
            var success = Send(FactoryMode.Factory.Order().SetManualVelocity(velocity))?.ToISetManualVelocity()?.Product;
            return success;
        }

        /// <summary>
        /// 停止掃描地圖
        /// </summary>
        /// <returns>是否在掃描中</returns>
        private bool? StopScanning() {
            var isScanning = Send(FactoryMode.Factory.Order().StopScanning())?.ToIStopScanning()?.Product;
            return isScanning;
        }

        /// <summary>
        /// 設定地圖檔名
        /// </summary>
        /// <remarks>是否在掃描中</remarks>
        private bool? SetScanningOriFileName(string oriName) {
            var isScanning = Send(FactoryMode.Factory.Order().SetScanningOriFileName(oriName))?.ToISetScanningOriFileName()?.Product;
            return isScanning;
        }

        /// <summary>
        /// 進行位置矯正
        /// </summary>
        /// <returns>地圖相似度</returns>
        public void DoPositionComfirm() {
            var similarity = Send(FactoryMode.Factory.Order().DoPositionComfirm())?.ToIDoPositionComfirm()?.Product;
            try {
                if (similarity != null) {
                    if (similarity >= 0 && similarity <= 1) {
                        mSimilarity = (double)similarity;
                        OnConsoleMessage($"iTS - The map similarity is {mSimilarity:0.0%}");
                    } else if (mSimilarity == -1) {
                        mSimilarity = (double)similarity;
                        OnConsoleMessage($"iTS - The map is now matched");
                    } else {
                        OnConsoleMessage($"iTS - The map similarity is 0%");
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 移至Goal(透過Goal點索引)
        /// </summary>
        /// <param name="goalIndex">Goal點索引</param>
        /// <returns>是否成功開始移動</returns>
        public void DoRunningByGoalIndex(uint goalID) {
            try {
                int index = Database.GoalGM.IndexOf(goalID);
                if (index >= 0) {
                    var goal = Database.GoalGM[goalID];
                    var success = Send(FactoryMode.Factory.Order().DoRunningByGoalIndex(index))?.ToIDoRunningByGoalIndex()?.Product;
                    if (success == true) {
                        OnConsoleMessage($"iTS - Start moving to {goal.Name}");
                    } else if (success == false) {
                        OnConsoleMessage($"Move to goal failure");
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 到指定充電站進行充電
        /// </summary>
        /// <param name="powerIndex">充電站索引</param>
        /// <returns>是否開始進行充電</returns>
        public void DoCharging(uint powerID) {
            try {
                int index = Database.PowerGM.IndexOf(powerID);
                if (index >= 0) {
                    var power = Database.PowerGM[powerID];
                    var success = Send(FactoryMode.Factory.Order().DoCharging(index))?.ToIDoCharging()?.Product;
                    if (success == true) {
                        OnConsoleMessage($"iTS - Begin charging at {power.Name}");
                    } else if (success == false) {
                        OnConsoleMessage("iTS - Charging failed");
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 要求Map檔清單
        /// </summary>
        /// <returns>Map檔清單</returns>
        public string RequestMapList() {
            var mapList = Send(FactoryMode.Factory.Order().RequestMapList())?.ToIRequestMapList()?.Product;
            return mapList != null ? string.Join(",", mapList) : null;
        }

        /// <summary>
        /// 要求Map檔
        /// </summary>
        /// <param name="mapName">要求的Map檔名</param>
        /// <returns>Map檔</returns>
        public IDocument RequestMapFile(string mapName) {
            var mapFile = Send(FactoryMode.Factory.Order().RequestMapFile(mapName))?.ToIRequestMapFile()?.Product;
            return mapFile;
        }

        /// <summary>
        /// 要求Ori檔清單
        /// </summary>
        /// <returns>Ori檔清單</returns>
        private string RequestOriList() {
            var oriList = Send(FactoryMode.Factory.Order().RequestOriList())?.ToIRequestOriList()?.Product;
            return oriList != null ? string.Join(",", oriList) : null;
        }

        /// <summary>
        /// 要求Ori檔
        /// </summary>
        /// <param name="oriName">Ori檔名</param>
        /// <returns>Ori檔</returns>
        private IDocument RequestOriFile(string oriName) {
            var oriFile = Send(FactoryMode.Factory.Order().RequestOriFile(oriName))?.ToIRequestOriFile()?.Product;
            return oriFile;
        }

        /// <summary>
        /// 上傳Map檔
        /// </summary>
        /// <param name="mapPath">要上傳的Map檔路徑</param>
        /// <returns>是否上傳成功</returns>
        private bool? UploadMapToAGV(string mapPath) {
            var success = Send(FactoryMode.Factory.Order().UploadMapToAGV(mapPath))?.ToIUploadMapToAGV()?.Product;
            return success;
        }

        /// <summary>
        /// 要求iTS載入指定的Map檔
        /// </summary>
        /// <param name="mapName">要載入的Map檔名</param>
        /// <returns>是否切換成功</returns>
        private bool? ChangeMap(string mapName) {
            var success = Send(FactoryMode.Factory.Order().ChangeMap(mapName))?.ToIChangeMap()?.Product;
            return success;
        }

        /// <summary>
        /// 要求Goal點清單
        /// </summary>
        /// <returns>Goal點清單</returns>
        private string RequestGoalList() {
            var goalList = Send(FactoryMode.Factory.Order().RequestGoalList())?.ToIRequestGoalList()?.Product;
            return goalList != null ? string.Join(",", goalList) : null;
        }

        /// <summary>
        /// 要求到指定Goal點的路徑
        /// </summary>
        /// <param name="goalIndex">目標Goal點索引</param>
        /// <returns>路徑(Count為0表示規劃失敗)</returns>
        private List<IPair> RequestPath(int goalIndex) {
            var path = Send(FactoryMode.Factory.Order().RequestPath(goalIndex))?.ToIRequestPath()?.Product;
            return path;
        }

        /// <summary>
        /// 要求自動回報iTS狀態
        /// </summary>
        /// <param name="on">是否自動回報</param>
        /// <returns>iTS狀態</returns>
        private IStatus AutoReportStatus(bool on) {
            var status = Send(FactoryMode.Factory.Order().AutoReportStatus(on))?.ToIAutoReportStatus()?.Product;
            return status;
        }

        /// <summary>
        /// 要求自動回傳雷射資料
        /// </summary>
        /// <param name="on">是否自動回報</param>
        /// <returns>雷射資料</returns>
        private List<IPair> AutoReportLaser(bool on) {
            var laser = Send(FactoryMode.Factory.Order().AutoReportLaser(on))?.ToIAutoReportLaser()?.Product;
            return laser;
        }

        /// <summary>
        /// 要求自動回傳路徑
        /// </summary>
        /// <param name="on">是否自動回報</param>
        /// <returns>路徑資料</returns>
        private List<IPair> AutoReportPath(bool on) {
            var path = Send(FactoryMode.Factory.Order().AutoReportPath(on))?.ToIAutoReportPath()?.Product;
            return path;
        }

        public void StartScan(bool scan) {
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
                                OnConsoleMessage($"iTS - The new ori name is {oriName}.ori");
                            }
                        } else {
                            OnConsoleMessage($"The iTS is now in {mStatus?.Description}, can't start scanning");
                        }
                    } else {//停止掃描
                        if (true || mStatus?.Description == EDescription.Map) {
                            isScanning = StopScanning();
                        } else {
                            OnConsoleMessage($"The iTS is now in {mStatus?.Description}, can't stop scanning");
                        }
                    }
                    if (isScanning != null) {
                        IsScanning = (bool)isScanning;
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage("Error:" + ex.Message);
            }
        }
        
        /// <summary>
        /// 傳送並要求載入Map
        /// </summary>
        /// <param name="mapPath">目標Map檔路徑</param>
        public void SendAndSetMap(string mapPath) {
            var success = UploadMapToAGV(mapPath);
            string mapName = Path.GetFileName(mapPath);
            if (success == true) {
                SetBalloonTip("Donwload", $"{mapName}.ori is downloaded {(success == true ? "successfully" : "failed")} ");
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
        public void AutoReport(bool auto) {
            try {
                bool isAutoReport = auto;
                var laser = AutoReportLaser(isAutoReport);
                var status = AutoReportStatus(isAutoReport);
                var path = AutoReportPath(isAutoReport);
                IsAutoReport = (laser?.Count ?? 0) > 0;
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

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

        public void SimplifyOri() {
            try {
                if (mBypassLoadFile) {
                    /*-- 空跑模擬SimplifyOri --*/
                    SpinWait.SpinUntil(() => false, 1000);
                    return;
                }
                string[] tmpPath = CurOriPath.Split('.');
                CurMapPath = tmpPath[0] + ".map";
                Database.Save(CurMapPath);
                OnVehiclePlanner(VehiclePlannerEvents.NewMap);
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        public void SaveGoal() {
            if (!string.IsNullOrEmpty(CurMapPath)) {
                OnConsoleMessage("[Map is Save]");
                Database.Save(CurMapPath);
            }
        }
        
        /// <summary>
        /// 刪除指定標記物
        /// </summary>
        /// <param name="markers"></param>
        public void DeleteMarker(IEnumerable<uint> markers) {
            foreach (var id in markers) {
                if (Database.GoalGM.ContainsID(id)) {
                    Database.GoalGM.Remove(id);
                } else if (Database.PowerGM.ContainsID(id)) {
                    Database.PowerGM.Remove(id);
                }
            }
            OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
        }

        /// <summary>
        /// 清除標記物
        /// </summary>
        public void ClearMarker() {
            OnConsoleMessage("[Clear Goal]");
            Database.GoalGM.Clear();
            OnConsoleMessage("[Clear Power]");
            Database.PowerGM.Clear();
            OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
        }

        /// <summary>
        /// 新增當前位置為Goal點
        /// </summary>
        public void AddCurrentAsGoal() {
            /*-- 取得當前位置 --*/
            var currentPosition = Database.AGVGM[mAGVID].Data;
            /*-- 建構Goal點 --*/
            var goal = FactoryMode.Factory.Goal(currentPosition, $"Goal{Database.GoalGM.Count}");
            /*-- 分配ID --*/
            var goalID = Database.ID.GenerateID();
            /*-- 將Goal點資料加入Goal點管理集合 --*/
            Database.GoalGM.Add(goalID, goal);
            /*-- 重新載入Goal點資訊 --*/
            OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
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


        /// <summary>
        /// 要求AGV設定新位置
        /// </summary>
        /// <param name="oldPosition">舊座標</param>
        /// <param name="newPosition">新座標</param>
        public void SetPosition(IPair oldPosition, IPair newPosition) {
            var position = FactoryMode.Factory.TowardPair(newPosition, oldPosition.Angle(newPosition));
            var success = SetPosition(position);
            if (success == true) {
                Database.AGVGM[mAGVID].SetLocation(position);
                OnConsoleMessage($"iTS - The position are now at {position}");
            }
        }

        public void FindPath(uint id) {
            try {
                int index = Database.GoalGM.IndexOf(id);
                if (index >= 0) {
                    var path = RequestPath(index);
                    if (path != null) {
                        var goal = Database.GoalGM[id];
                        if (path.Count > 0) {
                            OnConsoleMessage($"iTS - The path to {goal.Name} is completion. The number of path points is {path.Count}");
                        } else {
                            OnConsoleMessage($"iTS - Can not plan the path to  {goal.Name}");
                        }
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        #endregion Methods

        ///<summary>IP驗證</summary>
        ///<param name="ip">要驗證的字串</param>
        ///<returns>True:合法IP/False:非法IP</returns>
        private bool VerifyIP(string ip) {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
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

        #endregion Function - Private Methods

    }

    /// <summary>
    /// Bypass用的假SerialClient類
    /// </summary>
    public class FakeSerialClient : ISerialClient {

        private string mMapPath = @"D:\MapInfo\Client";

        private IPEndPoint mRemotePoint = null;

        public bool Connected { get; private set; }

        public string LocalIPPort { get; private set; }

        public string ServerIPPort { get; private set; }
        private DelReceiveDataEvent receiveDataEvent;

        public FakeSerialClient(DelReceiveDataEvent receiveDataEvent) {
            this.receiveDataEvent = receiveDataEvent;
        }

        public void Connect(string IP, int port) {
            mRemotePoint = new IPEndPoint(IPAddress.Parse(IP), port);
            Connected = true;
            LocalIPPort = "127.0.0.1:8080";
            ServerIPPort = IP + ":" + port;
            ConnectChange?.Invoke(this, new ConnectStatusChangeEventArgs() { IP = mRemotePoint.Address.ToString(), Port = mRemotePoint.Port, IsConnected = true });
        }

        public bool Send(string msg) { return true; }

        public bool Send(ICanSendBySerial msg) {
            IProductPacket product = null;

            //Thread.Sleep(3000);
            if (msg is IOrderPacket) {
                var order = msg as IOrderPacket;
                switch (order.Purpose) {
                    case EPurpose.RequestStatus:
                        product = order.ToIRequestStatus().CreatProduct(FactoryMode.Factory.Status());
                        break;
                    case EPurpose.RequestLaser:
                        var laser = new List<IPair>() { FactoryMode.Factory.Pair(0, 0) };
                        product = order.ToIRequestLaser().CreatProduct(laser);
                        break;
                    case EPurpose.SetServoMode:
                        product = order.ToISetServoMode().CreatProduct(order.ToISetServoMode().Design);
                        break;
                    case EPurpose.SetWorkVelocity:
                        product = order.ToISetWorkVelocity().CreatProduct(true);
                        break;
                    case EPurpose.SetPosition:
                        product = order.ToISetPosition().CreatProduct(true);
                        break;
                    case EPurpose.StartManualControl:
                        product = order.ToIStartManualControl().CreatProduct(order.ToIStartManualControl().Design);
                        break;
                    case EPurpose.SetManualVelocity:
                        product = order.ToISetManualVelocity().CreatProduct(true);
                        break;
                    case EPurpose.StopScanning:
                        product = order.ToIStopScanning().CreatProduct(false);
                        break;
                    case EPurpose.SetScanningOriFileName:
                        product = order.ToISetScanningOriFileName().CreatProduct(true);
                        break;
                    case EPurpose.DoPositionComfirm:
                        product = order.ToIDoPositionComfirm().CreatProduct(-1);
                        break;
                    case EPurpose.DoRunningByGoalIndex:
                        product = order.ToIDoRunningByGoalIndex().CreatProduct(true);
                        break;
                    case EPurpose.DoCharging:
                        product = order.ToIDoCharging()?.CreatProduct(true);
                        break;
                    case EPurpose.RequestMapList:
                        var mapList = Directory.GetFiles(@"D:\MapInfo\", "*.map").Select(v => Path.GetFileNameWithoutExtension(v)).ToList();
                        product = order.ToIRequestMapList().CreatProduct(mapList);
                        break;
                    case EPurpose.RequestMapFile:
                        string mapFile = @"D:\MapInfo\" + Path.GetFileNameWithoutExtension(order.ToIRequestMapFile().Design) + ".map";
                        product = order.ToIRequestMapFile().CreatProduct(mapFile);
                        break;
                    case EPurpose.RequestOriList:
                        var oriList = Directory.GetFiles(@"D:\MapInfo\", "*.ori").Select(v => Path.GetFileNameWithoutExtension(v)).ToList();
                        product = order.ToIRequestOriList().CreatProduct(oriList);
                        break;
                    case EPurpose.RequestOriFile:
                        var oriFile = @"D:\MapInfo\" + Path.GetFileNameWithoutExtension(order.ToIRequestOriFile().Design) + ".ori";
                        product = order.ToIRequestOriFile().CreatProduct(oriFile);
                        break;
                    case EPurpose.UploadMapToAGV:
                        var map = order.ToIUploadMapToAGV()?.Design;
                        bool success = map?.SaveAs(mMapPath) ?? false;
                        product = order.ToIUploadMapToAGV().CreatProduct(success);
                        break;
                    case EPurpose.ChangeMap:
                        product = order.ToIChangeMap().CreatProduct(true);
                        break;
                    case EPurpose.RequestGoalList:
                        List<string> goalList = new List<string>() { "GoalA", "GoalB", "GoalC" };
                        product = order.ToIRequestGoalList().CreatProduct(goalList);
                        break;
                    case EPurpose.RequestPath:
                        var path = new List<IPair>() { FactoryMode.Factory.Pair(0, 0) };
                        product = order.ToIRequestPath().CreatProduct(path);
                        break;
                    case EPurpose.AutoReportStatus:
                        var status = (order.ToIAutoReportStatus()?.Design == true) ? FactoryMode.Factory.Status() : null;
                        product = order.ToIAutoReportStatus().CreatProduct(status);
                        break;
                    case EPurpose.AutoReportLaser:
                        var laserData = (order.ToIAutoReportLaser()?.Design == true) ? new List<IPair>() {FactoryMode.Factory.Pair(0,0) } : null;
                        product = order.ToIAutoReportLaser().CreatProduct(laserData);
                        break;
                    case EPurpose.AutoReportPath:
                        var pathData = (order.ToIAutoReportPath()?.Design == true) ? new List<IPair>() { FactoryMode.Factory.Pair(0, 0)} : null;
                        product = order.ToIAutoReportPath().CreatProduct(pathData);
                        break;
                }
                if (product != null) {
                    receiveDataEvent.Invoke(this, new ReceiveDataEventArgs(product, null));
                }
            }
            return true;
        }

        public bool SendBinFile(string path) { return true; }

        public void Stop() {
            Connected = false;
            ConnectChange?.Invoke(this, new ConnectStatusChangeEventArgs() { IP = mRemotePoint.Address.ToString(),Port = mRemotePoint.Port,IsConnected= false});
        }

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        public event DelConnectStatusChangeEvent ConnectChange;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: 處置 Managed 狀態 (Managed 物件)。
                }

                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        // ~FakeSerialClient() {
        //   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose() {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    /// <summary>
    /// 模擬VehicleConle交握
    /// </summary>
    public class FakeVehicleConsole {

        #region Declaration - Fields

        /// <summary>
        /// 序列傳輸Server
        /// </summary>
        private ISerialServer mServer = null;

        /// <summary>
        /// 自動回報執行緒
        /// </summary>
        private Thread t_VPSender = null;

        private BroadcastReceiver mBroadcastReceiver = null;

        #endregion Declaration - Fields

        #region Function - Constructors

        public FakeVehicleConsole() {
            mServer = FactoryMode.Factory.SerialServer();
            mServer = FactoryMode.Factory.SerialServer();
            mServer.ConnectedEvent += MServer_ConnectedEvent;
            mServer.StartListening((int)EPort.VehiclePlanner, 3, VehiclePlannerReceiver);
            CtThread.CreateThread(ref t_VPSender, "mTdClientSender", tsk_AutoReportToVehiclePlanner);//iTS狀態自動回報(-> VehiclePlanner)
            mBroadcastReceiver = new BroadcastReceiver();
            mBroadcastReceiver.ReceivedData += mBroadcastReceiver_ReceivedData;
            mBroadcastReceiver.StartReceive(true);
        }

        #endregion Function - Constructors

        #region Function - Events
        private int count = 0;
        /// <summary>
        /// 廣播接收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mBroadcastReceiver_ReceivedData(object sender, BroadcastEventArgs e) {
            if (e.Message == "Count off") {
                mBroadcastReceiver.Send($"VehicleConsole {count++}", e.Remote);
            }
        }

        /// <summary>
        /// VP連線事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MServer_ConnectedEvent(object sender, ConnectStatusChangeEventArgs e) {
            ConsoleDisplay($"[VP]:{e.IP}:{e.Port} connected");
        }

        /// <summary>
        /// 接收來自Client指令
        /// </summary>
        private void VehiclePlannerReceiver(object sneder, ReceiveDataEventArgs e) {
            if (e.Data is IOrderPacket) {
                var pack = e.Data as IOrderPacket;
                IProductPacket product = null;
                switch (pack.Purpose) {
                    case EPurpose.RequestStatus:
                        var order = pack.ToIRequestStatus();
                        product = order.CreatProduct(FactoryMode.Factory.Status());
                        break;
                }
                if (product != null) {
                    string ipport = e.Remote.RemoteEndPoint.ToString();
                    mServer.Send(ipport, product);
                }
            }
        }

        #endregion Funciton - Events

        #region Function - Task

        /// <summary>
        /// iTS狀態自動回報(ToVehiclePlanner)
        /// </summary>
        private void tsk_AutoReportToVehiclePlanner() {
            while (mServer.IsListening) {
                //var laser = CreateLaser();
                //var path = CreatePath();
                //var status = CreateStatus();
                //if (mStatusPacket != null && mStatusSubscribers.Any()) {
                //    var product = mStatusPacket.ToIAutoReportStatus().CreatProduct(CreateStatus());
                //    foreach (string ipport in mStatusSubscribers) {
                //        mServer.Send(ipport, product);
                //    }
                //}
                //if (laser != null && mLaserPacket != null && mLaserSubscribers.Any()) {
                //    var product = mLaserPacket.ToIAutoReportLaser().CreatProduct(laser);
                //    foreach (string ipport in mLaserSubscribers) {
                //        mServer.Send(ipport, product);
                //    }
                //}
                //if (path != null && mPathPacket != null && mPathSubscribers.Any()) {
                //    var product = mPathPacket.ToIAutoReportPath().CreatProduct(path);
                //    foreach (string ipport in mPathSubscribers) {
                //        mServer.Send(ipport, product);
                //    }
                //}
                Thread.Sleep(200);
            }
        }
        
        #endregion Funciton - Task

        #region Function - Private Methods

        private void ConsoleDisplay(string msg) {
            Console.WriteLine(msg);
        }

        #endregion Funciton - Privagte Methods

    }

    /// <summary>
    /// 等待任務
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CtTaskCompletionSource<T> : TaskCompletionSource<T> {
        /// <summary>
        /// 任務序列號
        /// </summary>
        public uint SerialNumber { get; }
        /// <summary>
        /// 任務目的
        /// </summary>
        public EPurpose Purpose { get; }
        public CtTaskCompletionSource(uint serialNumber, EPurpose purpose) {
            this.SerialNumber = serialNumber;
            this.Purpose = purpose;
        }
        public CtTaskCompletionSource(IOrderPacket packet) : this(packet.SerialNumber, packet.Purpose) { }

    }

}
