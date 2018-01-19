using AGVDefine;
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
using System.Text;
using System.Threading.Tasks;
using static FactoryMode;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClientUI.Component {
    public class ClientUI_Serial:AgvClientUI {

        #region Declaration - Fields

        /// <summary>
        /// 序列化傳輸物件
        /// </summary>
        ISerialClient mSerialClient = null;

        List<CtTaskCompletionSource<IProductPacket>> mCmdTsk = new List<CtTaskCompletionSource<IProductPacket>>();

        #endregion Declaration  - Fields

        #region Declaration - Properties

        /// <summary>
        /// 目前與AGV的連線狀態
        /// </summary>
        public override bool IsConnected {
            get {
                return mSerialClient?.Connected ?? false;
            }
        }

        #endregion Declaration - Properteis

        #region Function - Constructors

        public ClientUI_Serial():base() {}

        #endregion Function - Constructors

        #region Function - Events

        private void mSerialClient_ReceiveData(object sender,ReceiveDataEventArgs e) {
            if (e.Data is IProductPacket) {
                var product = e.Data as IProductPacket;
                switch (product.Purpose) {
                    case EPurpose.AutoReportLaser:
                        var laser = product.ToIAutoReportLaser().Product;
                        if (laser != null) {
                            IsGettingLaser = true;
                            DrawLaser(product.ToIAutoReportLaser().Product);
                        } else {
                            IsGettingLaser = false;
                            Database.AGVGM[mAGVID].LaserAPoints.DataList.Clear();
                            Database.AGVGM[mAGVID].Path.DataList.Clear();
                        }
                        ITest.SetLaserStt(IsGettingLaser);
                        break;
                    case EPurpose.RequestLaser:
                        DrawLaser(product.ToIRequestLaser().Product);
                        break;
                    case EPurpose.SetServoMode:
                            var servoOn = product.ToISetServoMode().Product;
                            IConsole.AddMsg($"Server - Set:Servo{(servoOn ? "On" : "Off")}:{servoOn}");
                            IsMotorServoOn = servoOn;
                        break;
                    case EPurpose.SetWorkVelocity:
                            IConsole.AddMsg($"Server - Set:SerWorkVelocity:{product.ToISetWorkVelocity().Product}");
                        break;
                    case EPurpose.SetPosition: {
                            var pack = product.ToISetPosition();
                            Database.AGVGM[mAGVID].SetLocation(pack.Order.Design);
                            IConsole.AddMsg($"Server - Set:POS:{pack.Product}");
                            break;
                        }
                    case EPurpose.StartManualControl:
                        bool isMoving = product.ToIStartManualControl().Product;
                        IConsole.AddMsg($"Server - Set:Moving:{isMoving}");
                        break;
                    case EPurpose.SetManualVelocity: {
                            var pack = product.ToISetManualVelocity();
                            if (pack.Product) {
                                var manualVelocity = pack.Order.Design;
                                IConsole.AddMsg($"Server - Set:DriveVelo:{manualVelocity.X},{manualVelocity.Y}");
                            } else {
                                IConsole.AddMsg($"Server - Set:DriveVelo:False");
                            }
                            break;
                        }
                    case EPurpose.StopScaning: {
                            var pack = product.ToIStopScaning();
                            SetAgvStatus(EMode.Idle);
                        }
                        break;
                    case EPurpose.SetScaningOriFileName: {
                            var pack = product.ToISetScaningOriFileName();
                            if (pack.Product) {
                                IConsole.AddMsg($"Server - Set:OriName:{pack.Order.Design}");
                                SetAgvStatus(EMode.Map);
                            } else {
                                IConsole.AddMsg($"Server - Set:OriName:{pack.Product}");
                                SetAgvStatus(EMode.Idle);
                            }
                            break;
                        }
                    case EPurpose.DoPositionComfirm:
                        mSimilarity = product.ToIDoPositionComfirm().Product;
                        IConsole.AddMsg($"Server - Set:POSComfirm:{mSimilarity}");
                        break;
                    case EPurpose.AutoReportPath: {
                            var pack = product.ToIAutoReportPath();
                            //IConsole.AddMsg($"Server - SetPathPlan:idx({pack.Order.Design}):Count({pack.Product.Count})");
                            DrawPath(pack.Product);
                            break;
                        }
                    case EPurpose.DoRuningByGoalIndex: {
                            var pack = product.ToIDoRuningByGoalIndex();
                            IConsole.AddMsg($"Server - SetRun:idx({pack.Order.Design}):{pack.Product}");
                            break;
                        }
                    case EPurpose.DoCharging: {
                            var pack = product.ToIDoCharging();
                            IConsole.AddMsg($"Server - SetCharging:idx({pack.Order.Design}):{pack.Product}");
                            break;
                        }
                    case EPurpose.RequestMapList:
                        var mapList = product.ToIRequestMapList().Product;
                        using (MapList f = new MapList(mapList)) {
                            if (f.ShowDialog() == DialogResult.OK) {
                                mSerialClient.Send(FactoryMode.Factory.Order().RequestMapFile(f.strMapList));
                            }
                        }
                        break;
                    case EPurpose.RequestMapFile:
                        product.ToIRequestMapFile().Product.SaveAs(@"D:\Mapinfo\Client");
                        break;
                    case EPurpose.RequestOriFileList:
                        var oriList = product.ToIRequestOriFileList().Product;
                        using (MapList f = new MapList(oriList)) {
                            if (f.ShowDialog() == DialogResult.OK) {
                                mSerialClient.Send(FactoryMode.Factory.Order().RequestOriFile(f.strMapList));
                            }
                        }
                        break;
                    case EPurpose.RequestOriFile:
                        product.ToIRequestOriFile().Product.SaveAs(@"D:\Mapinfo\Client");
                        break;
                    case EPurpose.UploadMapToAGV: {
                            var pack = product.ToIUploadMapToAGV();
                            IConsole.AddMsg($"Server - Send:Map:{pack.Order.Design.Name}:{pack.Product}");
                            break;
                        }
                    case EPurpose.ChangeMap: {
                            var pack = product.ToIChangeMap();
                            IConsole.AddMsg($"Server - Set:MapName:{(pack.Product ?  pack.Order.Design : "False")}");
                            break;
                        }
                    case EPurpose.RequestGoalList:
                            mCmdTsk.Last().SetResult(product);
                            mCmdTsk.Remove(mCmdTsk.Last());                        
                        break;
                    case EPurpose.AutoReportStatus:
                        var status = product.ToIAutoReportStatus().Product;
                        this.InvokeIfNecessary(() => {
                            if (status.Battery != -1) {
                                tsprgBattery.Value = (int)status.Battery;
                                tslbBattery.Text = $"{status.Battery:0.0}%";
                            }
                            tslbStatus.Text = status.Description.ToString();
                        });
                        Database.AGVGM[mAGVID].SetLocation(status.Data);
                        break;
                    case EPurpose.RequestPath:
                        var path = product.ToIRequestPath().Product;
                        DrawPath(path);
                        break;
                }
            }
        }

        #endregion Function  - Events

        #region Funciton - Private Metdhos

        protected override void ITest_CheckIsServerAlive(bool cnn, string hostIP = "") {
            if (IsConnected != cnn) {
                if (cnn) {
                    if (mSerialClient == null) {
                        mSerialClient = FactoryMode.Factory.SerialClient(mSerialClient_ReceiveData,mBypassSocket);
                    }
                    mHostIP = hostIP;
                    mSerialClient.Connect(mHostIP, (int)EPort.ClientPort);                        
                } else {
                    mSerialClient.Dispose();
                    mSerialClient = null;
                }
                

                ITest.SetServerStt(IsConnected);
                IConsole.AddMsg($"Client - Is {(IsConnected ? "Connected" : "Disconnected")} to {mHostIP}");
            }
        }

        protected override bool GetLaser() {
            if (IsConnected) {
                return mSerialClient.Send(FactoryMode.Factory.Order().RequestLaser());
            }
            
            return false;
        }

        protected override void ITest_GetCar() {
            bool getting = !IsGettingLaser;
            mSerialClient.Send(FactoryMode.Factory.Order().AutoReportLaser(getting));
            mSerialClient.Send(FactoryMode.Factory.Order().AutoReportStatus(getting));
            mSerialClient.Send(FactoryMode.Factory.Order().AutoReportPath(getting));
            IConsole.AddMsg($"Client - Get:Car:{getting}");
        }

        protected override void ITest_MotorServoOn(bool servoOn) {
            mSerialClient.Send(FactoryMode.Factory.Order().SetServoMode(servoOn));
            IConsole.AddMsg($"Client - Set:Servo{(servoOn ? "On" : "Off")}:{servoOn}");
        }

        protected override void ITest_SetVelocity(int velocity) {
            mVelocity = velocity;
            mSerialClient.Send(FactoryMode.Factory.Order().SetWorkVelocity(mVelocity));
            IConsole.AddMsg($"Client - Set:WorkVelocity:{mVelocity}");
        }

        protected override void SetPosition(int x, int y, double theta) {
            var pos = FactoryMode.Factory.TowardPair(x, y, theta);
            var cmd = FactoryMode.Factory.Order().SetPosition(pos);

            mSerialClient.Send(cmd);
            
            IConsole.AddMsg($"Client - Set:POS:{pos.ToString()}");  
        }

        protected override void MotionContorl(MotionDirection direction) {
            if (direction == MotionDirection.Stop) {
                mSerialClient.Send(FactoryMode.Factory.Order().StartManualControl(false));
                IConsole.AddMsg($"Client - Set:Moving:False");
            } else {
                var cmd = GetMotionICmd(direction);
                if (cmd != null) {
                    mSerialClient.Send(cmd);
                    mSerialClient.Send(FactoryMode.Factory.Order().StartManualControl(true));
                    IConsole.AddMsg($"Client - Set:Moving:True");
                }
            }
        }

        protected override void ITest_StartScan(bool scan) {
            EMode mode = scan ? EMode.Map : EMode.Idle;
            if (mCarMode != mode) {
                if (scan) {
                    string oriName = string.Empty;
                    if (Stat.SUCCESS == CtInput.Text(out oriName, "MAP Name", "Set Map File Name")) {
                        mSerialClient.Send(FactoryMode.Factory.Order().SetScaningOriFileName(oriName));
                        IConsole.AddMsg($"Start scan");
                    } else {
                        return;
                    }
                }else {
                    mSerialClient.Send(FactoryMode.Factory.Order().StopScaning());
                    IConsole.AddMsg($"Stop scan");
                }
                mCarMode = mode;
                ChangedMode(mode);
            }
        }

        protected override void ITest_CarPosConfirm() {
            mSerialClient.Send(FactoryMode.Factory.Order().DoPositionComfirm());
            IConsole.AddMsg($"Client - Set:POSComfirm");
        }

        protected override void PathPlan(int numGoal) {
            mSerialClient.Send(FactoryMode.Factory.Order().RequestPath(numGoal));
        }

        protected override void Run(int numGoal) {
            mSerialClient.Send(FactoryMode.Factory.Order().DoRuningByGoalIndex(numGoal));
        }

        protected override void Charging(int numGoal) {
            mSerialClient.Send(FactoryMode.Factory.Order().DoCharging(numGoal));
        }

        protected override void ITest_GetMap() {
            mSerialClient.Send(FactoryMode.Factory.Order().RequestMapList());
        }

        protected override void ITest_GetORi() {
            mSerialClient.Send(FactoryMode.Factory.Order().RequestOriFileList());
        }

        protected override void ITest_SendMap() {
            OpenFileDialog openMap = new OpenFileDialog();
            openMap.InitialDirectory = mDefMapDir;
            openMap.Filter = "MAP|*.map";
            if (openMap.ShowDialog() == DialogResult.OK) {
                SendFile(openMap.FileName);
                mSerialClient.Send(FactoryMode.Factory.Order().UploadMapToAGV(openMap.FileName));
                mSerialClient.Send(FactoryMode.Factory.Order().ChangeMap(openMap.FileName));
            }
        }

        protected override string GetGoalNames() {
            CtTaskCompletionSource<IProductPacket> tskCompSrc = new CtTaskCompletionSource<IProductPacket>(0);
            var tsk = tskCompSrc.Task;
            mCmdTsk.Add(tskCompSrc);
            mSerialClient.Send(FactoryMode.Factory.Order().RequestGoalList());
            tsk.Wait(500);
            return string.Join(",", tsk.Result.ToIRequestGoalList().Product);
        }

        protected override void SendFile(string filePath) {
            IOrderPacket order = null;
            switch (Path.GetExtension(filePath).ToLower()) {
                case ".map":
                    mSerialClient.Send(FactoryMode.Factory.Order().UploadMapToAGV(filePath));
                    mSerialClient.Send(FactoryMode.Factory.Order().ChangeMap(filePath));
                    break;
                case ".ori":

                    break;
            }
        }

        private IOrderPacket<IPair,bool> GetMotionICmd(MotionDirection direction) {
            int r = 0, l = 0,v = mVelocity;
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
            return FactoryMode.Factory.Order().SetManualVelocity(l, r);
        }

        /// <summary>
        /// 設定AGV狀態
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="status"></param>
        private void SetAgvStatus<T>(T status)where T:struct {
            this.InvokeIfNecessary(() => {
                tslbStatus.Text = status.ToString();
            });
        }



        #endregion Function - Private Methods

    }

    internal class FakeSerialClient : ISerialClient {

        private DelReceiveDataEvent mReceiveDataEvent = null;

        public bool Connected { get; private set; } = false;

        public string LocalIPPort {
            get {
                throw new NotImplementedException();
            }
        }

        public string ServerIPPort {
            get {
                throw new NotImplementedException();
            }
        }

        public FakeSerialClient(DelReceiveDataEvent receiveDataEvent) {
            mReceiveDataEvent = receiveDataEvent;
        }

        public void Connect(string IP, int port) {
            Connected = true;
        }

        public void Dispose() {
            Stop();
        }

        public bool Send(string msg) {
            return true;
        }

        public bool Send(ICanSendBySerial msg) {
            //if (msg is IPacket<EPurpose>) {
            //    var cmd = (IPacket<EPurpose>)msg;
            //    switch (cmd.Purpose) {
            //        case EPurpose.GetCar:
            //            if (cmd is ICommandGetCar) {
            //                bool enb = (cmd as ICommandGetCar).Enable;
            //                //mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().GetCar(enb,null,null,0,null), null),null,null);
            //            }
            //            break;
            //        case EPurpose.GetLaser:
            //            if (cmd is ICommandGetLaser) {
            //                List<IPair> laserPoints = new List<IPair>();
            //                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().GetLaser(laserPoints), null),null,null);
            //            }
            //            break;
            //        case EPurpose.SetServo:
            //            if (cmd is ICommandSetServo) {
            //                bool servoOn = (cmd as ICommandSetServo).ServoOn;
            //                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetServo(servoOn), null), null, null);
            //            }
            //            break;
            //        case EPurpose.SetWorkVelo:
            //            if (cmd is ICommandSetWorkVelocity) {
            //                int velocity = (cmd as ICommandSetWorkVelocity).WorkVelocity;
            //                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetWorkVelocity(velocity), null), null, null);
            //            }
            //            break;
            //        case EPurpose.SetPOS:
            //            if (cmd is ICommandSetPOS) {
            //                ITowardPair pos = (cmd as ICommandSetPOS).Position;
            //                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetPOS(true), null), null, null);
            //            }
            //            break;
            //        case EPurpose.SetMoving:
            //            if (cmd is ICommandSetMoving) {
            //                bool start = (cmd as ICommandSetMoving).StartMoving;
            //                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetMoving(start), null), null, null);
            //            }
            //            break;
            //        case EPurpose.SetDriveVelo:
            //            if (cmd is ICommandSetDriveVelo) {
            //                ICommandSetDriveVelo velo = cmd as ICommandSetDriveVelo;
            //                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetDriveVelo(velo.LeftVelocity, velo.RightVelocity), null), null, null);
            //            }
            //            break;
            //        case EPurpose.SetMode:
            //            if (cmd is ICommandSetMode) {
            //                EMode mode = (cmd as ICommandSetMode).Mode;
            //                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetMode(mode), null), null, null);
            //            }
            //            break;
            //        case EPurpose.SetOriName:
            //            if (cmd is ICommandSetOriName) {
            //                string oriName = (cmd as ICommandSetOriName).OriName;
            //                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetOriName(oriName), null), null, null);
            //            }
            //            break;
            //        case EPurpose.SetPOSComfirm:
            //            if (cmd is ICommandSetPOSComfirm) {
            //                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetPOSComfirm(0.99), null), null, null);
            //            }
            //            break;
            //        case EPurpose.SetPathPlan:
            //            if (cmd is ICommandSetPathPlan) {
            //                int idx = (cmd as ICommandSetPathPlan).GoalIndex;
            //                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetPathPlan(true, idx, new List<IPair>()), null), null, null);
            //            }
            //            break;
            //        case EPurpose.SetRun:
            //            if (cmd is ICommandSetRun) {
            //                int idx = (cmd as ICommandSetRun).GoalIndex;
            //                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetRun(true, idx, new List<IPair>()), null), null, null);
            //            }
            //            break;
            //        case EPurpose.Charging:
            //            if (cmd is ICommandSetCharging) {
            //                int idx = (cmd as ICommandSetCharging).PowerIndex;
            //                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().Charging(true, idx, new List<IPair>()), null), null, null);
            //            }
            //            break;
            //        case EPurpose.GetMapList:
            //            if (cmd is ICommandGetMapList) {
            //                string[] mapList = Array.ConvertAll(Directory.GetFiles(@"D:\Mapinfo", "*.map"),v => Path.GetFileName(v));
            //                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().GetMapList(mapList), null), null, null);
            //            }
            //            break;
            //        case EPurpose.GetMap:
            //            if (cmd is ICommandGetMap) {
            //                string mapPath = $@"D:\Mapinfo\{(cmd as ICommandGetMap).MapName}";
            //                if (File.Exists(mapPath)) {
            //                    mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().GetMap(mapPath), null), null, null);
            //                }
            //            }
            //            break;
            //        case EPurpose.GetOriList:
            //            if (cmd is ICommandGetOriList) {
            //                string[] oriList = Array.ConvertAll(Directory.GetFiles(@"D:\Mapinfo", "*.ori"), v => Path.GetFileName(v));
            //                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().GetOriList(oriList), null), null, null);
            //            }
            //            break;
            //        case EPurpose.GetOri:
            //            if (cmd is ICommandGetOri) {
            //                string oriPath = $@"D:\Mapinfo\{(cmd as ICommandGetOri).OriName}";
            //                if (File.Exists(oriPath)) {
            //                    mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().GetOri(oriPath), null), null, null);
            //                }
            //            }
            //            break;
            //        case EPurpose.SendMap:
            //            if (cmd is ICommandSendMap) {
            //                var map = cmd as ICommandSendMap;
            //                map.Save($@"D:\Mapinfo\Client");
            //                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SendMap(true,map.FileName),null), null, null);
            //            }
            //            break;
            //        case EPurpose.SetMapName:
            //            if (cmd is ICommandSetMapName) {
            //                var mapName = (cmd as ICommandSetMapName).MapName;
            //                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetMapName(mapName), null), null, null);
            //            }
            //            break;
            //        case EPurpose.GetGoalList:
            //            if (cmd is ICommandGetGoalList) {
            //                //mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().GetGoalList(new string[] { "GoalA", "GoalB", "GoalC" }), null), null, null);
            //            }
            //            break;
            //    }
            //}
            return true;
        }

        public bool SendBinFile(string path) {
            return true;
        }

        public void Stop() {
            Connected = false;
        }
    }

    internal static class FactoryModeExtension {

        /// <summary>
        /// <para>建立基於 TCP/IP 的序列化同步通訊用戶端物件</para>
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="receiveDataEvent">接收事件委派</param>
        /// <param name="bypass">是否回傳模擬物件</param>
        /// <returns></returns>
        public static ISerialClient SerialClient(this IFactory factory, DelReceiveDataEvent receiveDataEvent,bool bypass) {
            return bypass ? new FakeSerialClient(receiveDataEvent) : factory.SerialClient(receiveDataEvent);
        }
    }

    internal class CtTaskCompletionSource<T> : TaskCompletionSource<T> {
        public int ID{ get; }
        public CtTaskCompletionSource(int id):base() {
            ID = id;
        }
    }
}
