using AGVDefine;
using BroadCast;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VehiclePlanner.Core;
using static FactoryMode;

namespace VehiclePlannerAGVBase {
    public class VehiclePlanner :BaseVehiclePlanner,IVehiclePlanner {

        #region Declaration - Fields

        /// <summary>
        /// MapGL相關操作
        /// </summary>
        private MapGLController mMapGL = MapGLController.GetInstance();
        
        /// <summary>
        /// 地圖中心
        /// </summary>
        private IPair mMapCenter = null;

        #endregion Declaration - Fields

        #region Declaration - Properties

        public new IITSController Controller { get { return base.Controller as IITSController; } }

        #endregion Declaration - Properties

        #region Function - Constructors

        public VehiclePlanner():base() {
            base.Controller = new ITSControllerSerial();
        }

        #endregion Function - Constructors

        #region Funciton - Public Methods

        /// <summary>
        /// 系統初始化
        /// </summary>
        public override void Initial() {
            mMapGL.Initial();
            base.Initial();
        }

        /// <summary>
        /// 清除地圖
        /// </summary>
        public override void ClearMap() {
            try {
                mMapGL.ClearAll();
                OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 載入檔案
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        public override void LoadFile(FileType type, string fileName) {
            try {
                bool isLoaded = false;
                switch (type) {
                    case FileType.Ori:
                        isLoaded = LoadOri(fileName);
                        if (isLoaded) {
                            mMapGL.ClearLaser();
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
                    if (Controller.IsConnected && type == FileType.Map) {
                        Controller.SendAndSetMap(fileName);
                    }
                } else {
                    OnErrorMessage("File data is wrong, can not read");
                }
            } catch (Exception ex) {
                OnErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 刪除指定標記物
        /// </summary>
        /// <param name="markers"></param>
        public void DeleteMarker(IEnumerable<uint> markers) {
            foreach (var id in markers) {
                if (mMapGL.ContainGoal(id)) {
                    mMapGL.RemoveGoal(id);
                } else if (mMapGL.ContainPower(id)) {
                    mMapGL.RemovePower(id);
                }
            }
            OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
        }

        /// <summary>
        /// 清除標記物
        /// </summary>
        public void ClearMarker() {
            OnConsoleMessage("[Clear Goal]");
            mMapGL.ClearGoal();
            OnConsoleMessage("[Clear Power]");
            mMapGL.ClearPower();
            OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
        }

        /// <summary>
        /// 新增當前位置為Goal點
        /// </summary>
        public override void AddCurrentAsGoal() {
            /*-- 取得當前位置 --*/
            var currentPosition = mMapGL.Location;
            /*-- 建構Goal點 --*/
            var goal = MapGLController.NewGoal(currentPosition);
            /*-- 分配ID --*/
            var goalID = MapGLController.GenerateID();
            /*-- 將Goal點資料加入Goal點管理集合 --*/
            mMapGL.AddGoal(goalID, goal);
            /*-- 重新載入Goal點資訊 --*/
            OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
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

        #endregion Funciotn - Public Methods

        /// <summary>
        /// 載入Map檔
        /// </summary>
        /// <param name="mapPath">Map檔路徑</param>
        protected override bool LoadMap(string mapPath) {
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
                var center = mMapGL.LoadMap(mCurMapPath)?.Center();
                if (center != null) {
                    MapCenter = center;
                    OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
                } else {
                    isLoaded = false;
                }
            }
            return isLoaded;
        }

        /// <summary>
        /// 載入Ori檔
        /// </summary>
        /// <param name="oriPath"></param>
        /// <returns></returns>
        protected override bool LoadOri(string oriPath) {
            bool isLoaded = true;
            CurOriPath = oriPath;
            OnVehiclePlanner(VehiclePlannerEvents.NewMap);
            if (!mBypassLoadFile) {//無BypassLoadFile
                /*-- 載入Map並取得Map中心點 --*/
                IPair center = mMapGL.LoadOri(CurOriPath)?.Center();
                if (center != null) {
                    MapCenter = center;
                } else {
                    isLoaded = false;
                }
            } else {//Bypass LoadFile功能
                    /*-- 空跑一秒，模擬檔案載入 --*/
                SpinWait.SpinUntil(() => false, 1000);
            }
            return isLoaded;
        }

        /// <summary>
        /// 儲存Map檔
        /// </summary>
        /// <param name="path"></param>
        protected override void SaveMap(string path) {
            mMapGL.Save(path);
        }

    }

    /// <summary>
    /// 繪圖控制
    /// </summary>
    internal class MapGLController {

        #region Static

        internal static MapGLController mInstance = null;

        internal static uint GenerateID() {
            return Database.ID.GenerateID();
        }

        internal static IGoal NewGoal(ITowardPair currentPosition) {
            return FactoryMode.Factory.Goal(currentPosition, $"Goal{Database.GoalGM.Count}");
        }

        internal static MapGLController GetInstance() {
            if (mInstance == null) mInstance = new MapGLController();
            return mInstance;
        }

        #endregion Static

        #region Declaration - Fields

        /// <summary>
        /// AGV ID
        /// </summary>
        protected uint mAGVID = 1;

        #endregion Declaration - FIelds

        #region Declaration - Properties

        public ITowardPair Location {
            get {
                return Database.AGVGM[mAGVID].Data;
            }
        }

        #endregion Declaration - Properties

        #region Funciton - Constructors

        private MapGLController() {

        }

        #endregion Funciton - Constructors

        #region Funciton - Public Methods

        public void Initial() {
            /*-- 載入AVG物件 --*/
            if (!Database.AGVGM.ContainsID(mAGVID)) {
                Database.AGVGM.Add(mAGVID, FactoryMode.Factory.AGV(0, 0, 0, "AGV"));
            }
        }

        public void ClearLaser() {
            Database.AGVGM[mAGVID].LaserAPoints.DataList.Clear();
        }

        public void ClearPath() {
            Database.AGVGM[mAGVID].Path.DataList.Clear();
        }

        internal void ClearAll() {
            Database.ClearAllButAGV();
            ClearLaser();
            ClearPath();
        }

        internal int IndexOfGoal(uint goalID) {
            return Database.GoalGM.IndexOf(goalID);
        }

        internal IGoal GetGoal(uint goalID) {
            return Database.GoalGM[goalID];
        }

        internal int IndexOfPower(uint powerID) {
            return Database.PowerGM.IndexOf(powerID);
        }

        internal IPower GetPower(uint powerID) {
            return Database.PowerGM[powerID];
        }

        internal void Save(string curMapPath) {
            Database.Save(curMapPath);
        }

        internal bool ContainGoal(uint id) {
            return Database.GoalGM.ContainsID(id);
        }

        internal void RemoveGoal(uint id) {
            Database.GoalGM.Remove(id);
        }

        internal bool ContainPower(uint id) {
            return Database.PowerGM.ContainsID(id);
        }

        internal void RemovePower(uint id) {
            Database.PowerGM.Remove(id);
        }

        internal void ClearGoal() {
            Database.GoalGM.Clear();
        }

        internal void ClearPower() {
            Database.PowerGM.Clear();
        }

        internal void AddGoal(uint goalID, IGoal goal) {
            Database.GoalGM.Add(goalID, goal);
        }

        internal void SetLocation(ITowardPair position) {
            Database.AGVGM[mAGVID].SetLocation(position);
        }

        internal IArea LoadOri(string curOriPath) {
            return Database.LoadOriToDatabase(curOriPath, mAGVID);
        }

        internal IArea LoadMap(string curMapPath) {
            return Database.LoadMapToDatabase(curMapPath);
        }

        internal void DrawLaser(IEnumerable<IPair> laser) {
            Database.AGVGM[mAGVID]?.LaserAPoints.DataList.Replace(laser);
        }

        internal void DrawPath(IEnumerable<IPair> path) {
            Database.AGVGM[mAGVID].Path.DataList.Replace(path);
        }

        #endregion Funciton - Public Methods

    }

    public interface IVehiclePlanner : IBaseVehiclePlanner {
        /// <summary>
        /// iTS控制器
        /// </summary>
        new IITSController Controller { get; }
        /// <summary>
        /// 地圖中心點
        /// </summary>
        IPair MapCenter { get; set; }
        /// <summary>
        /// 清除標記物
        /// </summary>
        void ClearMarker();
        /// <summary>
        /// 刪除指定標記物
        /// </summary>
        /// <param name="markers"></param>
        void DeleteMarker(IEnumerable<uint> markers);
    }

    /// <summary>
    /// 序列傳輸實作iTS控制
    /// </summary>
    internal class ITSControllerSerial : BaseiTSController,IITSController {

        #region Declaration - Fields

        private MapGLController mMapGL = MapGLController.GetInstance();

        /// <summary>
        /// 序列化傳輸物件
        /// </summary>
        private ISerialClient mSerialClient = null;

        /// <summary>
        /// 回應等待清單
        /// </summary>
        private List<CtTaskCompletionSource<IProductPacket>> mCmdTsk = new List<CtTaskCompletionSource<IProductPacket>>();

        /// <summary>
        /// iTS狀態
        /// </summary>
        private IStatus mStatus = FactoryMode.Factory.Status();

        #endregion Declaration - Fields

        #region Declaration - Properties

        /// <summary>
        /// 是否已建立連線
        /// </summary>
        public override bool IsConnected { get => mSerialClient?.Connected ?? false; }
        
        /// <summary>
        /// iTS狀態
        /// </summary>
        public IStatus Status {
            get {
                return mStatus;
            }
            protected set {
                if (value != null && mStatus != value) {
                    mStatus = value;
                    mMapGL.SetLocation(mStatus.Data);
                    OnPropertyChanged();
                }
            }
        }
        
        #endregion Declaration - Properties

        #region Funciotn - Constructors

        public ITSControllerSerial() : base() {

        }

        #endregion Funciotn - Constructors

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
                } else {
                    switch (product.Purpose) {
                        case EPurpose.AutoReportLaser:
                            var laser = product.ToIAutoReportLaser().Product;
                            if (laser != null) {
                                DrawLaser(product.ToIAutoReportLaser().Product);
                            } else {
                                IsAutoReport = false;
                                mMapGL.ClearLaser();
                                mMapGL.ClearPath();
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
            OnPropertyChanged(nameof(IsConnected));
            OnPropertyChanged(nameof(IsSearchable));
            OnConsoleMessage($"Client - Is {(e.IsConnected ? "Connected" : "Disconnected")} to {e.IP}:{e.Port}");
            if (e.IsConnected) {
                DoPositionComfirm();
                Status = RequestStatus();
            } else {
                if (mSerialClient != null) {
                    mSerialClient.Dispose();
                    mSerialClient = null;
                }
            }
        }

        #endregion Funciton - Events

        #region Funciton - Public Methods

        /// <summary>
        /// 與指定IP iTS連線/斷線
        /// </summary>
        /// <param name="cnn">連線/斷線</param>
        /// <param name="hostIP">AGV IP</param>
        public override void ConnectToITS(bool cnn) {
            try {
                if (IsConnected != cnn) {
                    if (cnn) {//連線至VC
                        /*-- 實例化物件 --*/
                        if (mSerialClient != null) {
                            mSerialClient.ConnectChange -= mSerialClient_OnConnectChange;
                            mSerialClient.Dispose();
                        }
                        mSerialClient = FactoryMode.Factory.SerialClient(mSerialClient_ReceiveData, mBypassSocket);
                        mSerialClient.ConnectChange += mSerialClient_OnConnectChange;
                        /*-- IP格式驗證 --*/
                        if (!VerifyIP(HostIP)) {
                            throw new FormatException($"{HostIP}是錯誤IP格式");
                        }
                        /*-- 測試IP是否存在 --*/
                        PingStatus pingStt = PingStatus.Unknown;
                        if ((pingStt = CtNetwork.Ping(HostIP, 500).PingState) != PingStatus.Success) {
                            throw new PingException(pingStt.ToString());
                        }
                        /*-- 連線至VehicleConsole --*/
                        mSerialClient.Connect(HostIP, (int)EPort.VehiclePlanner);
                    } else {//斷開與VehicleConsole的連線
                        mSerialClient.Stop();
                    }
                }
            } catch (PingException pe) {
                OnConsoleMessage($"Ping fail:{pe.Message}");
                OnBalloonTip("Connect failed", pe.Message);
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
                OnBalloonTip("Connect failed", ex.Message);
                if (mSerialClient != null) {
                    mSerialClient.Dispose();
                    mSerialClient = null;
                }
            }
        }

        /// <summary>
        /// 要求雷射資料
        /// </summary>
        /// <returns>雷射資料(0筆雷射資料表示失敗)</returns>
        public override void RequestLaser() {
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

        public override void SetServoMode(bool servoOn) {
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
        public override void SetWorkVelocity(int velocity) {
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
        /// 進行位置矯正
        /// </summary>
        /// <returns>地圖相似度</returns>
        public override void DoPositionComfirm() {
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
        public override void DoRunningByGoalName(string goalName) {
            try {
                var success = Send(FactoryMode.Factory.Order().DoRuningByGoalName(goalName))?.ToIDoRuningByGoalName()?.Product;
                if (success == true) {
                    OnConsoleMessage($"iTS - Start moving to {goalName}");
                } else if (success == false) {
                    OnConsoleMessage($"Move to goal failure");
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
        public override void DoCharging(string powerName) {
            try {
                var success = Send(FactoryMode.Factory.Order().DoCharging(powerName))?.ToIDoCharging()?.Product;
                if (success == true) {
                    OnConsoleMessage($"iTS - Begin charging at {powerName}");
                } else if (success == false) {
                    OnConsoleMessage("iTS - Charging failed");
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 要求Map檔清單
        /// </summary>
        /// <returns>Map檔清單</returns>
        public override string RequestMapList() {
            var mapList = Send(FactoryMode.Factory.Order().RequestMapList())?.ToIRequestMapList()?.Product;
            return mapList != null ? string.Join(",", mapList) : null;
        }

        /// <summary>
        /// 要求Map檔
        /// </summary>
        /// <param name="mapName">要求的Map檔名</param>
        /// <returns>Map檔</returns>
        public  IDocument RequestMapFile(string mapName) {
            var mapFile = Send(FactoryMode.Factory.Order().RequestMapFile(mapName))?.ToIRequestMapFile()?.Product;
            return mapFile;
        }

        public override void StartScan(bool scan) {
            try {
                bool? isScanning = null;
                if (mIsScanning != scan) {
                    if (scan) {//開始掃描
                        if (mStatus?.Description == EDescription.Idle) {
                            string oriName = string.Empty;
                            if (InputBox.Invoke(out oriName, "MAP Name", "Set Map File Name")) {
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
        /// 要求AGV設定新位置
        /// </summary>
        /// <param name="oldPosition">舊座標</param>
        /// <param name="newPosition">新座標</param>
        public void SetPosition(IPair oldPosition, IPair newPosition) {
            var position = FactoryMode.Factory.TowardPair(newPosition, oldPosition.Angle(newPosition));
            var success = SetPosition(position);
            if (success == true) {
                mMapGL.SetLocation(position);
                OnConsoleMessage($"iTS - The position are now at {position}");
            }
        }
        
        /// <summary>
        /// 取得Map檔
        /// </summary>
        public override void GetMap() {
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
                    OnBalloonTip("Donwload", $"{mapName}.map is downloaded {(success == true ? "successfully" : "failed")} ");
                }
            }
        }

        /// <summary>
        /// 取得Ori檔
        /// </summary>
        public override void GetOri() {
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
                    OnBalloonTip("Donwload", $"{oriName}.ori is downloaded {(success == true ? "successfully" : "failed")} ");
                }
            }
        }

        /// <summary>
        /// 切換資料自動回傳
        /// </summary>
        public override void AutoReport(bool auto) {
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

        public override void FindPath(string goalName) {
            try {
                var path = RequestPath(goalName);
                if (path != null) {
                    if (path.Count > 0) {
                        OnConsoleMessage($"iTS - The path to {goalName} is completion. The number of path points is {path.Count}");
                    } else {
                        OnConsoleMessage($"iTS - Can not plan the path to  {goalName}");
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        #endregion Funciotn - Public Methods

        #region Funciton - Private Methods

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

        /// <summary>
        /// 要求iTS狀態
        /// </summary>
        /// <returns>iTS狀態</returns>
        protected IStatus RequestStatus() {
            var status = Send(FactoryMode.Factory.Order().RequestStatus())?.ToIRequestStatus()?.Product;
            return status;
        }

        /// <summary>
        /// 要求Ori檔清單
        /// </summary>
        /// <returns>Ori檔清單</returns>
        protected override string RequestOriList() {
            var oriList = Send(FactoryMode.Factory.Order().RequestOriList())?.ToIRequestOriList()?.Product;
            return oriList != null ? string.Join(",", oriList) : null;
        }

        /// <summary>
        /// 要求Ori檔
        /// </summary>
        /// <param name="oriName">Ori檔名</param>
        /// <returns>Ori檔</returns>
        protected  IDocument RequestOriFile(string oriName) {
            var oriFile = Send(FactoryMode.Factory.Order().RequestOriFile(oriName))?.ToIRequestOriFile()?.Product;
            return oriFile;
        }

        /// <summary>
        /// 上傳Map檔
        /// </summary>
        /// <param name="mapPath">要上傳的Map檔路徑</param>
        /// <returns>是否上傳成功</returns>
        protected override bool? UploadMapToAGV(string mapPath) {
            var success = Send(FactoryMode.Factory.Order().UploadMapToAGV(mapPath))?.ToIUploadMapToAGV()?.Product;
            return success;
        }

        /// <summary>
        /// 要求iTS載入指定的Map檔
        /// </summary>
        /// <param name="mapName">要載入的Map檔名</param>
        /// <returns>是否切換成功</returns>
        protected override bool? ChangeMap(string mapName) {
            var success = Send(FactoryMode.Factory.Order().ChangeMap(mapName))?.ToIChangeMap()?.Product;
            return success;
        }

        /// <summary>
        /// 要求Goal點清單
        /// </summary>
        /// <returns>Goal點清單</returns>
        protected override string RequestGoalList() {
            var goalList = Send(FactoryMode.Factory.Order().RequestGoalList())?.ToIRequestGoalList()?.Product;
            return goalList != null ? string.Join(",", goalList) : null;
        }

        /// <summary>
        /// 要求到指定Goal點的路徑
        /// </summary>
        /// <param name="goalIndex">目標Goal點索引</param>
        /// <returns>路徑(Count為0表示規劃失敗)</returns>
        protected List<IPair> RequestPath(string goalName) {
            var path = Send(FactoryMode.Factory.Order().RequestPath(goalName))?.ToIRequestPath()?.Product;
            return path;
        }

        /// <summary>
        /// 要求自動回報iTS狀態
        /// </summary>
        /// <param name="on">是否自動回報</param>
        /// <returns>iTS狀態</returns>
        protected IStatus AutoReportStatus(bool on) {
            var status = Send(FactoryMode.Factory.Order().AutoReportStatus(on))?.ToIAutoReportStatus()?.Product;
            return status;
        }

        /// <summary>
        /// 要求自動回傳雷射資料
        /// </summary>
        /// <param name="on">是否自動回報</param>
        /// <returns>雷射資料</returns>
        protected List<IPair> AutoReportLaser(bool on) {
            var laser = Send(FactoryMode.Factory.Order().AutoReportLaser(on))?.ToIAutoReportLaser()?.Product;
            return laser;
        }

        /// <summary>
        /// 要求自動回傳路徑
        /// </summary>
        /// <param name="on">是否自動回報</param>
        /// <returns>路徑資料</returns>
        protected List<IPair> AutoReportPath(bool on) {
            var path = Send(FactoryMode.Factory.Order().AutoReportPath(on))?.ToIAutoReportPath()?.Product;
            return path;
        }

        /// <summary>
        /// 設定iTS當前位置
        /// </summary>
        /// <returns>是否設定成功</returns>
        protected bool? SetPosition(ITowardPair position) {
            var success = Send(FactoryMode.Factory.Order().SetPosition(position))?.ToISetPosition()?.Product;
            return success;
        }

        /// <summary>
        /// 開始手動控制
        /// </summary>
        /// <param name="start">是否開始手動控制</param>
        /// <remarks>是否為手動控制狀態</remarks>
        protected override bool? StartManualControl(bool start) {
            var isManual = Send(FactoryMode.Factory.Order().StartManualControl(start))?.ToIStartManualControl()?.Product;
            return isManual;
        }

        /// <summary>
        /// 設定手動控制移動速度(方向)
        /// </summary>
        /// <param name="velocity">手動移動速度</param>
        /// <returns>是否設定成功</returns>
        protected override bool? SetManualVelocity(int leftVelocity,int rightVelocity) {
            var velocity = FactoryMode.Factory.Pair(leftVelocity, rightVelocity);
            var success = Send(FactoryMode.Factory.Order().SetManualVelocity(velocity))?.ToISetManualVelocity()?.Product;
            return success;
        }

        /// <summary>
        /// 停止掃描地圖
        /// </summary>
        /// <returns>是否在掃描中</returns>
        protected override bool? StopScanning() {
            var isScanning = Send(FactoryMode.Factory.Order().StopScanning())?.ToIStopScanning()?.Product;
            return isScanning;
        }

        /// <summary>
        /// 設定地圖檔名
        /// </summary>
        /// <remarks>是否在掃描中</remarks>
        protected override bool? SetScanningOriFileName(string oriName) {
            var isScanning = Send(FactoryMode.Factory.Order().SetScanningOriFileName(oriName))?.ToISetScanningOriFileName()?.Product;
            return isScanning;
        }

        /// <summary>
        /// 繪製雷射
        /// </summary>
        /// <param name="laser"></param>
        protected void DrawLaser(IEnumerable<IPair> laser) {
            mMapGL.DrawLaser(laser);
        }

        /// <summary>
        /// 繪製路徑
        /// </summary>
        /// <param name="path"></param>
        protected void DrawPath(IEnumerable<IPair> path) {
            mMapGL.DrawPath(path);
        }
        
        #endregion Funciotn - Private Methods

    }

    public interface IITSController : IBaseITSController {
        /// <summary>
        /// iTS狀態
        /// </summary>
        IStatus Status { get; }
        /// <summary>
        /// 要求Map檔
        /// </summary>
        /// <param name="mapName">要求的Map檔名</param>
        /// <returns>Map檔</returns>
        IDocument RequestMapFile(string mapName);
        /// <summary>
        /// 傳送並要求載入Map
        /// </summary>
        /// <param name="mapPath">目標Map檔路徑</param>
        void SetPosition(IPair oldPosition, IPair newPosition);
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


    #region Fake

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
                    case EPurpose.DoRuningByGoalName:
                        product = order.ToIDoRunningByGoalName().CreatProduct(true);
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
                        var laserData = (order.ToIAutoReportLaser()?.Design == true) ? new List<IPair>() { FactoryMode.Factory.Pair(0, 0) } : null;
                        product = order.ToIAutoReportLaser().CreatProduct(laserData);
                        break;
                    case EPurpose.AutoReportPath:
                        var pathData = (order.ToIAutoReportPath()?.Design == true) ? new List<IPair>() { FactoryMode.Factory.Pair(0, 0) } : null;
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
            ConnectChange?.Invoke(this, new ConnectStatusChangeEventArgs() { IP = mRemotePoint.Address.ToString(), Port = mRemotePoint.Port, IsConnected = false });
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

        public FakeVehicleConsole(bool cnn) {
            mServer = FactoryMode.Factory.SerialServer();
            mServer = FactoryMode.Factory.SerialServer();
            mServer.ConnectedEvent += MServer_ConnectedEvent;
            mServer.StartListening((int)EPort.VehiclePlanner, 3, VehiclePlannerReceiver);
            CtThread.CreateThread(ref t_VPSender, "mTdClientSender", tsk_AutoReportToVehiclePlanner);//iTS狀態自動回報(-> VehiclePlanner)
            mBroadcastReceiver = new BroadcastReceiver(cnn, mBroadcastReceiver_ReceivedData);
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

    #endregion Fake

    public static class VehiclePlannerExtension {
        /// <summary>
        /// 建構序列傳輸物件
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="receiveDataEvent"></param>
        /// <param name="bypass"></param>
        /// <returns></returns>
        public static ISerialClient SerialClient(this IFactory factory, DelReceiveDataEvent receiveDataEvent, bool bypass) {
            if (bypass) {
                return new FakeSerialClient(receiveDataEvent);
            } else {
                return FactoryMode.Factory.SerialClient(receiveDataEvent);
            }
        }
    }

}
