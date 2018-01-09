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

        List<CtTaskCompletionSource<IPacket<EPurpose>>> mCmdTsk = new List<CtTaskCompletionSource<IPacket<EPurpose>>>();

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
            if (e.Data is IPacket<EPurpose>) {
                var response = (IPacket<EPurpose>)e.Data;
                switch (response.Purpose) {
                    case EPurpose.GetCar:
                        if (response is IResponseGetCar) {
                            IsGettingLaser = (response as IResponseGetCar).Enable;
                            ITest.SetLaserStt(IsGettingLaser);
                            IConsole.AddMsg($"Server - Get:Car:{IsGettingLaser}");
                        }
                        break;
                    case EPurpose.GetLaser:
                        if (response is IResponseGetLaser) {
                            var laser = (response as IResponseGetLaser).LaserPoints;
                            IConsole.AddMsg($"Server - GetLaser {laser.Count()}points");
                            DrawLaser(laser);
                        }
                        break;
                    case EPurpose.SetServo:
                        if (response is IResponseSetServo) {
                            var servoOn = (response as IResponseSetServo).ServoOn;
                            IConsole.AddMsg($"Server - Set:Servo{(servoOn ? "On" : "Off")}:{servoOn}");
                            IsMotorServoOn = servoOn;
                        }
                        break;
                    case EPurpose.SetWorkVelo:
                        if (response is IResponseSetWorkVelocity) {
                            int velocity = (response as IResponseSetWorkVelocity).WorkVelocity;
                            IConsole.AddMsg($"Server - Set:SerWorkVelocity:{velocity}");
                        }
                        break;
                    case EPurpose.SetPOS:
                        if (response is IResponseSetPOS) {
                            bool suc = (response as IResponseSetPOS).Success;
                            IConsole.AddMsg($"Server - Set:POS:{suc}");
                        }
                        break;
                    case EPurpose.SetMoving:
                        if (response is IResponseSetMoving) {
                            bool isMoving = (response as IResponseSetMoving).IsMoving;
                            IConsole.AddMsg($"Server - Set:Moving:{isMoving}");
                        }
                        break;
                    case EPurpose.SetDriveVelo:
                        if (response is IResponseSetDriveVelo) {
                            IResponseSetDriveVelo velo = response as IResponseSetDriveVelo;
                            IConsole.AddMsg($"Server - Set:DriveVelo:{velo.LeftVelocity},{velo.RightVelocity}");
                        }
                        break;
                    case EPurpose.SetMode:
                        if (response is IResponseSetMode) {
                            EMode mode = (response as IResponseSetMode).Mode;
                            IConsole.AddMsg($"Server - Set:Mode:{mode}");
                        }
                        break;
                    case EPurpose.SetOriName:
                        if (response is IResponseSetOriName) {
                            string oriName = (response as IResponseSetOriName).OriName;
                            IConsole.AddMsg($"Server - Set:OriName:{oriName}");
                        }
                        break;
                    case EPurpose.SetPOSComfirm:
                        if (response is IResponseSetPOSComfirm) {
                            mSimilarity = (response as IResponseSetPOSComfirm).Similarity;
                            IConsole.AddMsg($"Server - Set:POSComfirm:{mSimilarity}");
                        }
                        break;
                    case EPurpose.SetPathPlan:
                        if (response is IResponseSetPathPlan) {
                            var val = (response as IResponseSetPathPlan);
                            IConsole.AddMsg($"Server - SetPathPlan:idx({val.GoalIndex}):{val.Success}:Count({val.Path?.Count ?? 0})");
                            DrawPath(val.Path);
                        }
                        break;
                    case EPurpose.SetRun:
                        if (response is IResponseSetRun) {
                            var val = (response as IResponseSetRun);
                            IConsole.AddMsg($"Server - SetRun:idx({val.GoalIndex}):{val.Success}:Count({val.Path?.Count ?? 0})");
                            DrawPath(val.Path);
                        }
                        break;
                    case EPurpose.Charging:
                        if (response is IResponseCharging) {
                            var val = (response as IResponseCharging);
                            IConsole.AddMsg($"Server - SetCharging:idx({val.PowerIndex}):{val.Success}:Count({val.Path?.Count ??0})");
                            DrawPath(val.Path);
                        }
                        break;
                    case EPurpose.GetMapList:
                        if (response is IResponseGetMapList) {
                            string[] mapList = (response as IResponseGetMapList).MapList;
                            using (MapList f = new MapList(mapList)) {
                                if (f.ShowDialog() == DialogResult.OK) {
                                    mSerialClient.Send(FactoryMode.Factory.Command().GetMap(f.strMapList));
                                }
                            }
                        }
                        break;
                    case EPurpose.GetMap:
                        if (response is IResponseGetMap) {
                            var map = (response as IResponseGetMap);
                            map.Save(@"D:\Mapinfo\Client");
                        }
                        break;
                    case EPurpose.GetOriList:
                        if (response is IResponseGetOriList) {
                            string[] oriList = (response as IResponseGetOriList).OriList;
                            using (MapList f = new MapList(oriList)) {
                                if (f.ShowDialog() == DialogResult.OK) {
                                    mSerialClient.Send(FactoryMode.Factory.Command().GetOri(f.strMapList));
                                }
                            }
                        }
                        break;
                    case EPurpose.GetOri:
                        if (response is IResponseGetOri) {
                            var ori = (response as IResponseGetOri);
                            ori.Save(@"D:\Mapinfo\Client");
                        }
                        break;
                    case EPurpose.SendMap:
                        if (response is IResponseSendMap) {
                            var map = response as IResponseSendMap;
                            IConsole.AddMsg($"Server - Send:Map:{map.FileName}:{map.Success}");
                            mSerialClient.Send(FactoryMode.Factory.Command().SetMapName(map.FileName));
                        }
                        break;
                    case EPurpose.SetMapName:
                        if (response is IResponseSetMapName) {
                            IConsole.AddMsg($"Server - Set:MapName:{(response as IResponseSetMapName).MapName}");
                        }
                        break;
                    case EPurpose.GetGoalList:
                        if (response is IResponseGetGoalList) {
                            mCmdTsk.Last().SetResult(response);
                            mCmdTsk.Remove(mCmdTsk.Last());
                        }
                        break;
                    case EPurpose.TakeStatus:
                        if (response is IResponseTakeStatus) {
                            var status = (response as IResponseTakeStatus).Status;
                            this.InvokeIfNecessary(() => {
                                if (status.Battery != -1) {
                                    tsprgBattery.Value = (int)status.Battery;
                                    tslbBattery.Text = $"{status.Battery:0.0}%";
                                }
                                tslbStatus.Text = status.Description.ToString();
                            });
                            Database.AGVGM[mAGVID].SetLocation(status.Data);
                        }
                        break;
                    case EPurpose.TakePath:
                        if (response is IResponseTakePath) {
                            var path = (response as IResponseTakePath).Path;
                            Database.AGVGM[mAGVID].Path.DataList.Replace(path);
                        }
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
                return mSerialClient.Send(FactoryMode.Factory.Command().GetLaser());
            }
            
            return false;
        }

        protected override void ITest_GetCar() {
            mSerialClient.Send(FactoryMode.Factory.Command().GetCar(!IsGettingLaser));
            IConsole.AddMsg($"Client - Get:Car:{!IsGettingLaser}");
        }

        protected override void ITest_MotorServoOn(bool servoOn) {
            mSerialClient.Send(FactoryMode.Factory.Command().SetServo(servoOn));
            IConsole.AddMsg($"Client - Set:Servo{(servoOn ? "On" : "Off")}:{servoOn}");
        }

        protected override void ITest_SetVelocity(int velocity) {
            mVelocity = velocity;
            mSerialClient.Send(FactoryMode.Factory.Command().SetWorkVelocity(mVelocity));
            IConsole.AddMsg($"Client - Set:WorkVelocity:{mVelocity}");
        }

        protected override void SetPosition(int x, int y, double theta) {
            var pos = FactoryMode.Factory.TowardPair(x, y, theta);
            var cmd = FactoryMode.Factory.Command().SetPOS(pos);

            mSerialClient.Send(cmd);
            
            IConsole.AddMsg($"Client - Set:POS:{pos.ToString()}");  
        }

        protected override void MotionContorl(MotionDirection direction) {
            if (direction == MotionDirection.Stop) {
                mSerialClient.Send(FactoryMode.Factory.Command().SetMoving(false));
                IConsole.AddMsg($"Client - Set:Moving:False");
            } else {
                var cmd = GetMotionICmd(direction);
                if (cmd != null) {
                    mSerialClient.Send(cmd);
                    mSerialClient.Send(FactoryMode.Factory.Command().SetMoving(true));
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
                        mSerialClient.Send(FactoryMode.Factory.Command().SetMode(mode));
                        mSerialClient.Send(FactoryMode.Factory.Command().SetOriName(oriName));
                        IConsole.AddMsg($"Start scan");
                    } else {
                        return;
                    }
                }else {
                    mSerialClient.Send(FactoryMode.Factory.Command().SetMode(mode));
                    IConsole.AddMsg($"Stop scan");
                }
                mCarMode = mode;
                ChangedMode(mode);
            }
        }

        protected override void ITest_CarPosConfirm() {
            mSerialClient.Send(FactoryMode.Factory.Command().SetPOSComfirm());
            IConsole.AddMsg($"Client - Set:POSComfirm");
        }

        protected override void PathPlan(int numGoal) {
            mSerialClient.Send(FactoryMode.Factory.Command().SetPathPlan(numGoal));
        }

        protected override void Run(int numGoal) {
            mSerialClient.Send(FactoryMode.Factory.Command().SetRun(numGoal));
        }

        protected override void Charging(int numGoal) {
            mSerialClient.Send(FactoryMode.Factory.Command().SetCharging(numGoal));
        }

        protected override void ITest_GetMap() {
            mSerialClient.Send(FactoryMode.Factory.Command().GetMapList());
        }

        protected override void ITest_GetORi() {
            mSerialClient.Send(FactoryMode.Factory.Command().GetOriList());
        }

        protected override void ITest_SendMap() {
            OpenFileDialog openMap = new OpenFileDialog();
            openMap.InitialDirectory = mDefMapDir;
            openMap.Filter = "MAP|*.map";
            if (openMap.ShowDialog() == DialogResult.OK) {
                mSerialClient.Send(FactoryMode.Factory.Command().SendMap(openMap.FileName));
            }
        }

        protected override string GetGoalNames() {
            CtTaskCompletionSource<IPacket<EPurpose>> tskCompSrc = new CtTaskCompletionSource<IPacket<EPurpose>>(0);
            var tsk = tskCompSrc.Task;
            mCmdTsk.Add(tskCompSrc);
            mSerialClient.Send(FactoryMode.Factory.Command().GetGoalList());
            tsk.Wait(500);
            return string.Join(",", (tsk.Result as IResponseGetGoalList).GoalNames);
        }

        private ICommandSetDriveVelo GetMotionICmd(MotionDirection direction) {
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
            return FactoryMode.Factory.Command().SetDriveVelo(l, r);
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
            if (msg is IPacket<EPurpose>) {
                var cmd = (IPacket<EPurpose>)msg;
                switch (cmd.Purpose) {
                    case EPurpose.GetCar:
                        if (cmd is ICommandGetCar) {
                            bool enb = (cmd as ICommandGetCar).Enable;
                            //mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().GetCar(enb,null,null,0,null), null),null,null);
                        }
                        break;
                    case EPurpose.GetLaser:
                        if (cmd is ICommandGetLaser) {
                            List<IPair> laserPoints = new List<IPair>();
                            mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().GetLaser(laserPoints), null),null,null);
                        }
                        break;
                    case EPurpose.SetServo:
                        if (cmd is ICommandSetServo) {
                            bool servoOn = (cmd as ICommandSetServo).ServoOn;
                            mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetServo(servoOn), null), null, null);
                        }
                        break;
                    case EPurpose.SetWorkVelo:
                        if (cmd is ICommandSetWorkVelocity) {
                            int velocity = (cmd as ICommandSetWorkVelocity).WorkVelocity;
                            mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetWorkVelocity(velocity), null), null, null);
                        }
                        break;
                    case EPurpose.SetPOS:
                        if (cmd is ICommandSetPOS) {
                            ITowardPair pos = (cmd as ICommandSetPOS).Position;
                            mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetPOS(true), null), null, null);
                        }
                        break;
                    case EPurpose.SetMoving:
                        if (cmd is ICommandSetMoving) {
                            bool start = (cmd as ICommandSetMoving).StartMoving;
                            mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetMoving(start), null), null, null);
                        }
                        break;
                    case EPurpose.SetDriveVelo:
                        if (cmd is ICommandSetDriveVelo) {
                            ICommandSetDriveVelo velo = cmd as ICommandSetDriveVelo;
                            mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetDriveVelo(velo.LeftVelocity, velo.RightVelocity), null), null, null);
                        }
                        break;
                    case EPurpose.SetMode:
                        if (cmd is ICommandSetMode) {
                            EMode mode = (cmd as ICommandSetMode).Mode;
                            mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetMode(mode), null), null, null);
                        }
                        break;
                    case EPurpose.SetOriName:
                        if (cmd is ICommandSetOriName) {
                            string oriName = (cmd as ICommandSetOriName).OriName;
                            mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetOriName(oriName), null), null, null);
                        }
                        break;
                    case EPurpose.SetPOSComfirm:
                        if (cmd is ICommandSetPOSComfirm) {
                            mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetPOSComfirm(0.99), null), null, null);
                        }
                        break;
                    case EPurpose.SetPathPlan:
                        if (cmd is ICommandSetPathPlan) {
                            int idx = (cmd as ICommandSetPathPlan).GoalIndex;
                            mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetPathPlan(true, idx, new List<IPair>()), null), null, null);
                        }
                        break;
                    case EPurpose.SetRun:
                        if (cmd is ICommandSetRun) {
                            int idx = (cmd as ICommandSetRun).GoalIndex;
                            mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetRun(true, idx, new List<IPair>()), null), null, null);
                        }
                        break;
                    case EPurpose.Charging:
                        if (cmd is ICommandSetCharging) {
                            int idx = (cmd as ICommandSetCharging).PowerIndex;
                            mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().Charging(true, idx, new List<IPair>()), null), null, null);
                        }
                        break;
                    case EPurpose.GetMapList:
                        if (cmd is ICommandGetMapList) {
                            string[] mapList = Array.ConvertAll(Directory.GetFiles(@"D:\Mapinfo", "*.map"),v => Path.GetFileName(v));
                            mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().GetMapList(mapList), null), null, null);
                        }
                        break;
                    case EPurpose.GetMap:
                        if (cmd is ICommandGetMap) {
                            string mapPath = $@"D:\Mapinfo\{(cmd as ICommandGetMap).MapName}";
                            if (File.Exists(mapPath)) {
                                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().GetMap(mapPath), null), null, null);
                            }
                        }
                        break;
                    case EPurpose.GetOriList:
                        if (cmd is ICommandGetOriList) {
                            string[] oriList = Array.ConvertAll(Directory.GetFiles(@"D:\Mapinfo", "*.ori"), v => Path.GetFileName(v));
                            mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().GetOriList(oriList), null), null, null);
                        }
                        break;
                    case EPurpose.GetOri:
                        if (cmd is ICommandGetOri) {
                            string oriPath = $@"D:\Mapinfo\{(cmd as ICommandGetOri).OriName}";
                            if (File.Exists(oriPath)) {
                                mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().GetOri(oriPath), null), null, null);
                            }
                        }
                        break;
                    case EPurpose.SendMap:
                        if (cmd is ICommandSendMap) {
                            var map = cmd as ICommandSendMap;
                            map.Save($@"D:\Mapinfo\Client");
                            mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SendMap(true,map.FileName),null), null, null);
                        }
                        break;
                    case EPurpose.SetMapName:
                        if (cmd is ICommandSetMapName) {
                            var mapName = (cmd as ICommandSetMapName).MapName;
                            mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().SetMapName(mapName), null), null, null);
                        }
                        break;
                    case EPurpose.GetGoalList:
                        if (cmd is ICommandGetGoalList) {
                            //mReceiveDataEvent.BeginInvoke(this, new ReceiveDataEventArgs(FactoryMode.Factory.Response().GetGoalList(new string[] { "GoalA", "GoalB", "GoalC" }), null), null, null);
                        }
                        break;
                }
            }
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
