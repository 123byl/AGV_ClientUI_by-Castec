using AGVDefine;
using BroadCast;
using CtLib.Library;
using Geometry;
using SerialCommunication;
using SerialCommunicationData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VehiclePlanner.Partial.VehiclePlannerUI;
using CtLib.Module.Utility;

namespace VehiclePlannerAGVBase {
    
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
    /// Bypass Socket時用的SerialClient類，用於測試
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
    /// 模擬iTS，用於回傳假Status與廣播測試
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

    
}
