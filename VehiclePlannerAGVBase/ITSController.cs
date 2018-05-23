using AGVDefine;
using CtLib.Library;
using Geometry;
using SerialCommunication;
using SerialCommunicationData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using VehiclePlanner.Core;

namespace VehiclePlannerAGVBase {
    /// <summary>
    /// 序列傳輸實作iTS控制
    /// </summary>
    internal class ITSControllerSerial : BaseiTSController, IITSController {

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
        public override void ConnectToITS() {
            try {
                if (IsConnectable) {
                    if (!IsConnected) {//連線至VC
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
        public IDocument RequestMapFile(string mapName) {
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
        protected IDocument RequestOriFile(string oriName) {
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
        protected override bool? SetManualVelocity(int leftVelocity, int rightVelocity) {
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

}
