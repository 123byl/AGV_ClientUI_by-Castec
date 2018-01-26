using AGVDefine;
using CtLib.Library;
using Geometry;
using GLCore;
using SerialCommunication;
using SerialCommunicationData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace VehiclePlanner {
    public partial class VehiclePlannerUI {

        #region Declaration  - Fields

        /// <summary>
        /// 回應等待逾時時間
        /// </summary>
        private int mTimeOut = 3000;

        /// <summary>
        /// 序列化傳輸物件
        /// </summary>
        private ISerialClient mSerialClient = null;

        /// <summary>
        /// 回應等待清單
        /// </summary>
        private List<CtTaskCompletionSource<IProductPacket>> mCmdTsk = new List<CtTaskCompletionSource<IProductPacket>>();

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

        #endregion Declaration - Properties

        #region Funciton - Events

        /// <summary>
        /// 序列化通訊接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSerialClient_ReceiveData(object sender, ReceiveDataEventArgs e) {
            if (e.Data is IProductPacket) {
                var product = e.Data as IProductPacket;
                var cmdSrc = mCmdTsk.Find(v => v.SerialNumber == product.SerialNumber);
                if (cmdSrc != null) {
                    cmdSrc.SetResult(product);
                    return;
                }
                switch (product.Purpose) {
                    //case EPurpose.RequestStatus:
                    //    Status = product.ToIRequestStatus()?.Product;
                    //    break;
                    case EPurpose.AutoReportLaser:
                        var laser = product.ToIAutoReportLaser().Product;
                        if (laser != null) {
                            DrawLaser(product.ToIAutoReportLaser().Product);
                        } else {
                            mIsAutoReport = false;
                            Database.AGVGM[mAGVID].LaserAPoints.DataList.Clear();
                            Database.AGVGM[mAGVID].Path.DataList.Clear();
                            ITest.SetLaserStt(mIsAutoReport);
                        }
                        break;
                    //case EPurpose.RequestLaser:
                    //    DrawLaser(product.ToIRequestLaser().Product);
                    //    break;
                    //case EPurpose.SetServoMode:
                    //    var servoOn = product.ToISetServoMode().Product;
                    //    IConsole.AddMsg($"Server - Set:Servo{(servoOn ? "On" : "Off")}:{servoOn}");
                    //    IsMotorServoOn = servoOn;
                    //    break;
                    //case EPurpose.SetWorkVelocity:
                    //    IConsole.AddMsg($"Server - Set:SerWorkVelocity:{product.ToISetWorkVelocity().Product}");
                    //    break;
                    //case EPurpose.SetPosition: {
                    //        var pack = product.ToISetPosition();
                    //        Database.AGVGM[mAGVID].SetLocation(pack.Order.Design);
                    //        IConsole.AddMsg($"Server - Set:POS:{pack.Product}");
                    //        break;
                    //    }
                    //case EPurpose.StartManualControl:
                    //    bool isMoving = product.ToIStartManualControl().Product;
                    //    IConsole.AddMsg($"Server - Set:Moving:{isMoving}");
                    //    break;
                    //case EPurpose.SetManualVelocity: {
                    //        var pack = product.ToISetManualVelocity();
                    //        if (pack.Product) {
                    //            var manualVelocity = pack.Order.Design;
                    //            IConsole.AddMsg($"Server - Set:DriveVelo:{manualVelocity.X},{manualVelocity.Y}");
                    //        } else {
                    //            IConsole.AddMsg($"Server - Set:DriveVelo:False");
                    //        }
                    //        break;
                    //    }
                    //case EPurpose.StopScanning: {
                    //        var pack = product.ToIStopScanning();
                    //        if (pack != null) {
                    //            ITest.ChangedScanStt(pack.Product);
                    //        } else {
                    //            IConsole.AddMsg("Error:StopScanning response is null");
                    //        }
                    //    }
                    //    break;
                    //case EPurpose.SetScanningOriFileName: {
                    //        var pack = product.ToISetScanningOriFileName();
                    //        if (pack.Product) {
                    //            ITest.ChangedScanStt(true);
                    //            IConsole.AddMsg($"Server - Set:OriName:{pack.Order.Design}");
                    //        } else {
                    //            ITest.ChangedScanStt(false);
                    //            IConsole.AddMsg($"Server - Set:OriName:{pack.Product}");
                    //        }
                    //        break;
                    //    }
                    //case EPurpose.DoPositionComfirm:
                    //    mSimilarity = product.ToIDoPositionComfirm().Product;
                    //    IConsole.AddMsg($"Server - Set:POSComfirm:{mSimilarity}");
                    //    break;
                    case EPurpose.AutoReportPath: {
                            var pack = product.ToIAutoReportPath();
                            //IConsole.AddMsg($"Server - SetPathPlan:idx({pack.Order.Design}):Count({pack.Product.Count})");
                            DrawPath(pack.Product);
                            break;
                        }
                    //case EPurpose.DoRunningByGoalIndex: {
                    //        var pack = product.ToIDoRunningByGoalIndex();
                    //        IConsole.AddMsg($"Server - SetRun:idx({pack.Order.Design}):{pack.Product}");
                    //        break;
                    //    }
                    //case EPurpose.DoCharging: {
                    //        var pack = product.ToIDoCharging();
                    //        IConsole.AddMsg($"Server - SetCharging:idx({pack.Order.Design}):{pack.Product}");
                    //        break;
                    //    }
                    //case EPurpose.RequestMapList:
                    //    var mapList = product.ToIRequestMapList().Product;
                    //    using (MapList f = new MapList(mapList)) {
                    //        if (f.ShowDialog() == DialogResult.OK) {
                    //            Send(FactoryMode.Factory.Order().RequestMapFile(f.strMapList));
                    //        }
                    //    }
                    //    break;
                    //case EPurpose.RequestMapFile:
                    //    product.ToIRequestMapFile().Product.SaveAs(@"D:\Mapinfo\Client");
                    //    break;
                    //case EPurpose.RequestOriFileList:
                    //    var oriList = product.ToIRequestOriFileList().Product;
                    //    using (MapList f = new MapList(oriList)) {
                    //        if (f.ShowDialog() == DialogResult.OK) {
                    //            Send(FactoryMode.Factory.Order().RequestOriFile(f.strMapList));
                    //        }
                    //    }
                    //    break;
                    //case EPurpose.RequestOriFile:
                    //    product.ToIRequestOriFile().Product.SaveAs(@"D:\Mapinfo\Client");
                    //    break;
                    //case EPurpose.UploadMapToAGV: {
                    //        var pack = product.ToIUploadMapToAGV();
                    //        IConsole.AddMsg($"Server - Send:Map:{pack.Order.Design.Name}:{pack.Product}");
                    //        break;
                    //    }
                    //case EPurpose.ChangeMap: {
                    //        var pack = product.ToIChangeMap();
                    //        IConsole.AddMsg($"Server - Set:MapName:{(pack.Product ? pack.Order.Design : "False")}");
                    //        break;
                    //    }
                    //case EPurpose.RequestGoalList:
                    //    mCmdTsk.Last().SetResult(product);
                    //    mCmdTsk.Remove(mCmdTsk.Last());
                    //    break;
                    case EPurpose.AutoReportStatus:
                        Status = product.ToIAutoReportStatus()?.Product;
                        break;
                    case EPurpose.RequestPath:
                        var path = product.ToIRequestPath().Product;
                        DrawPath(path);
                        break;
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
        protected virtual void ConnectToITS(bool cnn, string hostIP = "") {
            try {
                if (IsConnected != cnn) {
                    if (cnn) {//連線至VC
                        /*-- 實例化物件 --*/
                        if (mSerialClient == null) {
                            mSerialClient = FactoryMode.Factory.SerialClient(mSerialClient_ReceiveData, mBypassSocket);
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
                        mSerialClient.Connect(hostIP, (int)EPort.ClientPort);
                    } else {//斷開與VehicleConsole的連線
                        mSerialClient?.Stop();
                        mSerialClient.Dispose();
                        mSerialClient = null;
                    }
                    /*--依連線狀態設定界面--*/
                    ITest.SetServerStt(IsConnected);
                    IConsole.AddMsg($"Client - Is {(IsConnected ? "Connected" : "Disconnected")} to {mHostIP}");
                    if (IsConnected) {
                        mHostIP = hostIP;
                        ITest_CarPosConfirm();
                        Status = RequestStatus();
                    }
                }
            } catch (PingException pe) {
                IConsole.AddMsg($"Ping fail:{pe.Message}");
            } catch (Exception ex) {
                IConsole.AddMsg(ex.Message);
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
                IConsole.AddMsg("The packet is null, unable to send");
                return null;
            }
            /*--檢查連線--*/
            if (!IsConnected) {
                IConsole.AddMsg("Is not connected, unable to send");
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
                                    IConsole.AddMsg($"{cmdTitle} response is received");
                                } else {
                                    IConsole.AddMsg($"{cmdTitle} response is null");
                                }
                            }
                        } else {
                            IConsole.AddMsg($"{cmdTitle} response timeout");
                        }
                    });
                } else {//已在等待回應
                    IConsole.AddMsg($"{cmdTitle}Waiting for the iTS to respond");
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
        private List<IPair> RequestLaser() {
            var laser = Send(FactoryMode.Factory.Order().RequestLaser())?.ToIRequestLaser()?.Product;
            return laser;
        }

        /// <summary>
        /// 設定伺服馬達激磁
        /// </summary>
        /// <param name="servoOn">激磁狀態設定</param>
        /// <returns>伺服馬達當前激磁狀態</returns>
        private bool? SetServoMode(bool servoOn) {
            var servoOnStt = Send(FactoryMode.Factory.Order().SetServoMode(servoOn))?.ToISetServoMode()?.Product;
            return servoOnStt;
        }

        /// <summary>
        /// 設定iTS工作移動速度
        /// </summary>
        /// <param name="velocity">移動速度</param>
        /// <returns>是否設定成功</returns>
        private bool? SetWorkVelocity(int velocity) {
            var success = Send(FactoryMode.Factory.Order().SetWorkVelocity(velocity))?.ToISetWorkVelocity()?.Product;
            return success;
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
        private double? DoPositionComfirm() {
            var similarity = Send(FactoryMode.Factory.Order().DoPositionComfirm())?.ToIDoPositionComfirm()?.Product;
            return similarity;
        }

        /// <summary>
        /// 移至Goal(透過Goal點索引)
        /// </summary>
        /// <param name="goalIndex">Goal點索引</param>
        /// <returns>是否成功開始移動</returns>
        private bool? DoRunningByGoalIndex(int goalIndex) {
            var success = Send(FactoryMode.Factory.Order().DoRunningByGoalIndex(goalIndex))?.ToIDoRunningByGoalIndex()?.Product;
            return success;
        }

        /// <summary>
        /// 到指定充電站進行充電
        /// </summary>
        /// <param name="powerIndex">充電站索引</param>
        /// <returns>是否開始進行充電</returns>
        private bool? DoCharging(int powerIndex) {
            var success = Send(FactoryMode.Factory.Order().DoCharging(powerIndex))?.ToIDoCharging()?.Product;
            return success;
        }

        /// <summary>
        /// 要求Map檔清單
        /// </summary>
        /// <returns>Map檔清單</returns>
        private string RequestMapList() {
            var mapList = Send(FactoryMode.Factory.Order().RequestMapList())?.ToIRequestMapList()?.Product;
            return mapList != null ? string.Join(",", mapList) : null;
        }

        /// <summary>
        /// 要求Map檔
        /// </summary>
        /// <param name="mapName">要求的Map檔名</param>
        /// <returns>Map檔</returns>
        private IDocument RequestMapFile(string mapName) {
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



        #endregion Methods

        #endregion Function - Private Methods
    }

    /// <summary>
    /// Bypass用的假SerialClient類
    /// </summary>
    public class FakeSerialClient : ISerialClient {

        private string mMapPath = @"D:\MapInfo\Client";

        public bool Connected { get; private set; }

        public string LocalIPPort { get; private set; }

        public string ServerIPPort { get; private set; }
        private DelReceiveDataEvent receiveDataEvent;

        public FakeSerialClient(DelReceiveDataEvent receiveDataEvent) {
            this.receiveDataEvent = receiveDataEvent;
        }

        public void Connect(string IP, int port) {
            Connected = true;
            LocalIPPort = "127.0.0.1:8080";
            ServerIPPort = IP + ":" + port;
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
        }

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

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
