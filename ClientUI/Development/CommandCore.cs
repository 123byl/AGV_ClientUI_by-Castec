using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;
using System.Threading;
using CtLib.Library;
using System.IO;
using static MapProcessing.MapSimplication;
using MapProcessing;

namespace CommandCore {

    #region Declaration - Delegate

    /// <summary>
    /// 命令委派定義
    /// </summary>
    public static class CommandEvents {

        /// <summary>
        /// 通用委派定義
        /// </summary>
        public static class Comm {
            /// <summary>
            /// 傳輸事件
            /// </summary>
            /// <param name="msg">傳輸內容</param>
            public delegate void DelConsole(string msg);
        }

        /// <summary>
        /// Client端委派定義
        /// </summary>
        public static class Client {
            /// <summary>
            /// 車子資訊更新事件
            /// </summary>
            /// <param name="info">車子資訊</param>
            public delegate void DelCarInfoRefresh(CarInfo info);

            /// <summary>
            /// 檔案下載完畢事件
            /// </summary>
            /// <param name="fileNmae">下載的檔案名稱</param>
            public delegate void DelFileDownload(string fileNmae);

            /// <summary>
            /// 規劃路徑更新事件
            /// </summary>
            /// <param name="path">路徑點集合</param>
            public delegate void DelPathRefresh(List<CartesianPos> path);
        }

        /// <summary>
        /// Server端委派定義
        /// </summary>
        public static class Server {
            public delegate void DelSetCarID(int carID);
            public delegate int DelGetCarID();
            public delegate void DelSetOriName(string oriName);
            public delegate bool DelSetMapName(string mapNmae);
            public delegate void DelReadMapFile();
            public delegate void DelPathPlan(CartesianPos goal);
            public delegate CartesianPos DelIdxToGoal(int idxGoal);
            public delegate CartesianPos DelFindGoal(string goalName);
            public delegate void DelRecordMap();
            public delegate List<string> DelGetGoalNames();
            public delegate string[] DelGetOriNames();
            public delegate string[] DelGetMapNames();
            public delegate string DelGetOriDirectory();
            public delegate string DelGetMapDirectory();
            public delegate int DelGetPower();
        }

    }

    #endregion Declaration - Delegate

    #region Declaration - Enum

    /// <summary>
    /// 通訊埠定義
    /// </summary>
    public enum SoxPort {
        /// <summary>
        /// 命令傳遞
        /// </summary>
        Cmd = 400,
        /// <summary>
        /// 檔案下載(Server to Client)
        /// </summary>
        GetFile = 600,
        /// <summary>
        /// 檔案傳送(Client to Server)
        /// </summary>
        SendFile = 700,
        /// <summary>
        /// 車子資訊
        /// </summary>
        Info = 800,
        /// <summary>
        /// 路徑規劃資料
        /// </summary>
        Path = 900
    }

    /// <summary>
    /// AGV車工作模式定義
    /// </summary>
    public enum WorkMode {
        ScanningLine,
        Obstacle,
        Map,
        Work,
        MapView,
        Idle
    }

    /// <summary>
    /// 檔案類型
    /// </summary>
    public enum FileType {
        Ori,
        Map,
    }

    /// <summary>
    /// 車子運動方向
    /// </summary>
    public enum MotionDirection {
        /// <summary>
        /// 往前
        /// </summary>
        Forward = 0,
        /// <summary>
        /// 往後
        /// </summary>
        Backward = 1,
        /// <summary>
        /// 左旋
        /// </summary>
        LeftTrun = 2,
        /// <summary>
        /// 右璇
        /// </summary>
        RightTurn = 3,
        /// <summary>
        /// 停止
        /// </summary>
        Stop = 4
    }

    #endregion Declaration - Enum

    #region Declaration - Interface

    /// <summary>
    /// Client端通用命令
    /// </summary>
    public interface IClientCommand {

        /// <summary>
        /// 車子資訊更新事件
        /// </summary>
        event CommandEvents.Client.DelCarInfoRefresh CarInfoRefresh;

        /// <summary>
        /// 檔案下載完畢事件
        /// </summary>
        event CommandEvents.Client.DelFileDownload FileDownload;

        /// <summary>
        /// 路徑更新事件
        /// </summary>
        event CommandEvents.Client.DelPathRefresh PathRefresh;

        /// <summary>
        /// 傳送檔案
        /// </summary>
        /// <param name="fileName"></param>
        void SendFile(string fileName);

        /// <summary>
        /// 地圖存放路徑
        /// </summary>
        string DefMapDir { get; set; }

        /// <summary>
        /// Server端是否運作中
        /// </summary>
        bool IsServerAlive { get; }

        /// <summary>
        /// 是否正在取得雷射資料
        /// </summary>
        bool IsGettingLaser { get; }

        /// <summary>
        /// 要求所有Goal點名稱
        /// </summary>
        /// <remarks>
        /// 回傳格式為 Goal List:'Goal1','Goal2',...
        /// </remarks>
        /// <returns></returns>
        string GetGoalNames();

        /// <summary>
        /// 激活遠端檔案接收
        /// </summary>
        void EnableFileRecive();

        /// <summary>
        /// 激活車子資訊回傳(含雷射)
        /// </summary>
        /// <returns></returns>
        bool EnableCarInfoReturn(bool enb);

        /// <summary>
        /// 單獨取一次雷射資料
        /// </summary>
        /// <returns></returns>
        IEnumerable<int> GetLaser();

        /// <summary>
        /// 回傳伺服馬達是否ServoOn
        /// </summary>
        /// <returns></returns>
        bool IsServoOn();

        /// <summary>
        /// 伺服馬達停止動作
        /// </summary>
        void MotorStop();

        /// <summary>
        /// 前進
        /// </summary>
        void Forward(int velocity);

        /// <summary>
        /// 後退
        /// </summary>
        void Backward(int velocity);

        /// <summary>
        /// 右轉
        /// </summary>
        /// <param name="velocity"></param>
        void RightTurn(int velocity);

        /// <summary>
        /// 左轉
        /// </summary>
        /// <param name="velocity"></param>
        void LeftTrun(int velocity);

        /// <summary>
        /// 取得Server端檔案清單
        /// </summary>
        /// <param name="type">要取得的檔案類型</param>
        /// <returns>回傳格式:'File1','File2','File3,....'</returns>
        string GetFileNames(FileType type);

        /// <summary>
        /// 要求傳送指定檔案
        /// </summary>
        /// <param name="type">要求的檔案類型</param>
        /// <param name="fileName">要求的檔案名稱</param>
        void RequireFile(FileType type, string fileName);

        /// <summary>
        /// 移動到指定Goal點
        /// </summary>
        /// <param name="idxGoal">目標Goal點索引值</param>
        void Run(int idxGoal);

        /// <summary>
        /// 要求到指定Goal點的路徑
        /// </summary>
        /// <param name="idxGoal">目標Goal點索引值</param>
        void RequirePath(int idxGoal);

        /// <summary>
        /// 要求Server端載入指定地圖檔
        /// </summary>
        /// <param name="mapName"></param>
        void OrderMap(string mapName);

        /// <summary>
        /// 設定馬達激磁
        /// </summary>
        /// <param name="servoOn"></param>
        void SetMotor(bool servoOn);

        /// <summary>
        /// 發送測試封包，測試Server端是否運作中
        /// </summary>
        /// <returns></returns>
        bool Ping();

        /// <summary>
        /// 設定車子速度
        /// </summary>
        /// <param name="velocity"></param>
        void SetVelocity(int velocity);

        /// <summary>
        /// 設定車子模式
        /// </summary>
        /// <param name="mode"></param>
        void SetCarMode(WorkMode mode);

        /// <summary>
        /// 設定掃描的地圖名稱
        /// </summary>
        /// <param name="oriName">掃描的ori檔名</param>
        void SetScanName(string oriName);

    }

    /// <summary>
    /// Server端通用命令
    /// </summary>
    public interface IServerCommand {

        /// <summary>
        /// AGV動作物件參考
        /// </summary>
        IAGV RefAGV { get; set; }

        /// <summary>
        /// CarID設定委派
        /// </summary>
        CommandEvents.Server.DelSetCarID SetCarID { get; set; }

        /// <summary>
        /// CarID取得委派
        /// </summary>
        CommandEvents.Server.DelGetCarID GetCarID { get; set; }

        /// <summary>
        /// Ori檔名設定委派
        /// </summary>
        CommandEvents.Server.DelSetOriName SetOriName { get; set; }

        /// <summary>
        /// Map檔命設定委派
        /// </summary>
        CommandEvents.Server.DelSetMapName SetMapName { get; set; }

        /// <summary>
        /// Map檔讀取委派
        /// </summary>
        CommandEvents.Server.DelReadMapFile ReadMapFile { get; set; }

        /// <summary>
        /// 規劃到Goal點的路徑
        /// </summary>
        CommandEvents.Server.DelPathPlan PathPlan { get; set; }

        /// <summary>
        /// Goal點索引取得Goal點
        /// </summary>
        CommandEvents.Server.DelIdxToGoal IdxToGoal { get; set; }

        /// <summary>
        /// 尋找Goal點
        /// </summary>
        CommandEvents.Server.DelFindGoal FindGoal { get; set; }

        /// <summary>
        /// 地圖掃描
        /// </summary>
        CommandEvents.Server.DelRecordMap RecordMap { get; set; }

        /// <summary>
        /// 取得Goal點清單
        /// </summary>
        CommandEvents.Server.DelGetGoalNames GetGoalNames { get; set; }

        /// <summary>
        /// 取得Ori檔清單
        /// </summary>
        CommandEvents.Server.DelGetOriNames GetOriNames { get; set; }

        /// <summary>
        /// 取得Map檔清單
        /// </summary>
        CommandEvents.Server.DelGetMapNames GetMapNames { get; set; }

        /// <summary>
        /// 取得Ori檔目錄路徑
        /// </summary>
        CommandEvents.Server.DelGetOriDirectory GetOriDirectory { get; set; }

        /// <summary>
        /// 取得Map檔目錄路徑
        /// </summary>
        CommandEvents.Server.DelGetMapDirectory GetMapDirectory { get; set; }

        /// <summary>
        /// 取得當前電池電量
        /// </summary>
        CommandEvents.Server.DelGetPower GetPower { get; set; }

    }

    /// <summary>
    /// Socket通用屬性
    /// </summary>
    public interface ISoxCom {

        /// <summary>
        /// 資料傳輸事件
        /// </summary>
        event CommandEvents.Comm.DelConsole ConsoleRefresh;

        /// <summary>
        /// 命令發送埠
        /// </summary>
        int CmdSendPort { get; set; }

        /// <summary>
        /// 車子資訊接收埠
        /// </summary>
        int InfoRecivePort { get; set; }

        /// <summary>
        /// 檔案接收埠
        /// </summary>
        int FileRecivePort { get; set; }

        /// <summary>
        /// 路徑規劃資料接收埠
        /// </summary>
        int PathRecivePort { get; set; }

        /// <summary>
        /// 地圖發送埠
        /// </summary>
        int SendMapPort { get; set; }
    }

    /// <summary>
    /// 透過Socket進行交握的Server端核心
    /// </summary>
    public interface ISoxCmdServer : IServerCommand, ISoxCom {
        /// <summary>
        /// 是否監聽中
        /// </summary>
        bool IsListening { get; }

        /// <summary>
        /// Create thread for server
        /// </summary>
        void ServoOn();
    }

    /// <summary>
    /// 透過Socket進行交握的Client端核心
    /// </summary>
    public interface ISoxCmdClient : IClientCommand, ISoxCom {

        /// <summary>
        /// Server端IP
        /// </summary>
        string ServerIP { get; set; }

        /// <summary>
        /// 是否與Server已建立連線
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 連線至Server端
        /// </summary>
        /// <param name="ip">Server端IP</param>
        /// <param name="port">Server端Port</param>
        /// <returns>是否連線成功</returns>
        bool Connect();

        /// <summary>
        /// 斷開連線
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 是否略過Socket連接
        /// </summary>
        bool Bypass { get; set; }
    }

    /// <summary>
    /// AGV動作介面
    /// </summary>
    /// <remarks>
    /// 定義AGV屬性與行為
    /// </remarks>
    public interface IAGV {
        double[] Position { get; set; }
        int[] MotorVelocity { get; set; }
        int DriveSpeed { get; set; }
        long[] EncoderValue { get; set; }
        List<int> LaserDistanceData { get; set; }
        int[] MotorAccel { get; set; }
        int[] MotorDccel { get; set; }
        WorkMode mode { get; set; }
        bool DriveConnectState { get; set; }
        bool BrakeMode { get; set; }
        bool f_pathSearching { get; set; }
        IEnumerable<CartesianPos> WorkPath { get; set; }

        bool Drive();
        bool StopDrive();
        bool DriveOn();
        bool DriveOff();
        bool SetAcceleration(int v);
        bool SetDcceleration(int v);
        bool SetBrakeMode(bool v);
        void GetMotorInfo(out bool[] info);
        bool GetDriveVelocity(out int lVelo, out int rVelo);
        void DoPathMovement();
    }

    /// <summary>
    /// 命令傳送者
    /// </summary>
    public interface ICmdSender {
        void Send(string data);
        void Send(byte[] data);
    }

    /// <summary>
    /// 命令接收者
    /// </summary>
    public interface ICmdReceiver {

        /// <summary>
        /// 接收緩衝區資料長度
        /// </summary>
        int BufferLength { get; }

        /// <summary>
        /// 接收資料
        /// </summary>
        /// <param name="data">接收到的資料</param>
        /// <returns>資料長度</returns>
        int Receive(out byte[] data);
    }

    #endregion Declaration - Interface

    #region Declaration - Implement Class

    /// <summary>
    /// Client端命令交握方法實作
    /// </summary>
    public abstract class BaseClientCommand : IClientCommand {

        #region Implement - IClientCommand

        /// <summary>
        /// 資料傳輸事件
        /// </summary>
        public event CommandEvents.Comm.DelConsole ConsoleRefresh;

        /// <summary>
        /// 車子資訊更新事件
        /// </summary>
        public event CommandEvents.Client.DelCarInfoRefresh CarInfoRefresh;

        /// <summary>
        /// 檔案下載完畢事件
        /// </summary>
        public event CommandEvents.Client.DelFileDownload FileDownload;

        /// <summary>
        /// 路徑更新事件
        /// </summary>
        public event CommandEvents.Client.DelPathRefresh PathRefresh;

        /// <summary>
        /// 地圖存放路徑
        /// </summary>
        public string DefMapDir { get; set; } = @"D:\MapInfo\";

        /// <summary>
        /// Server端是否正在運作
        /// </summary>
        public bool IsServerAlive { get; protected set; }

        /// <summary>
        /// 是否正在取得雷射資料
        /// </summary>
        public bool IsGettingLaser { get; protected set; }

        /// <summary>
        /// 傳送檔案
        /// </summary>
        /// <param name="fileName"></param>
        public abstract void SendFile(string fileName);

        #region Get

        /// <summary>
        /// 取得指定類型檔案清單
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual string GetFileNames(FileType type) {
            string[] rtnMsg = SendMsg($"Get:{type}List");
            return rtnMsg[3];
        }

        /// <summary>
        /// 要求所有Goal點名稱
        /// </summary>
        /// <remarks>
        /// 回傳格式為 Goal List:'Goal1','Goal2',...
        /// </remarks>
        /// <returns></returns>
        public virtual string GetGoalNames() {
            string[] rtnMsg = SendMsg($"Get:GoalList");
            if (rtnMsg.Count() < 4) {
                throw new Exception("回傳錯誤");
            }
            return rtnMsg[3];
        }

        /// <summary>
        /// 單獨取一次雷射資料
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetLaser() {
            /*-- 若是雷射資料則更新資料 --*/
            string[] rtnMsg = SendMsg("Get:Laser");
            if (rtnMsg.Length > 3) {
                if (rtnMsg[1] == "Laser") {
                    string[] sreRemoteLaser = rtnMsg[3].Split(',');
                    return sreRemoteLaser.Select(x => int.Parse(x));
                }
            }
            throw new Exception("雷射封包格式不正確");
        }

        /// <summary>
        /// 發送測試封包，測試Server端是否運作中
        /// </summary>
        /// <returns></returns>
        public bool Ping() {
            string[] rtnMsg = SendMsg("Get:Hello", false);
            return rtnMsg.Count() > 2 && rtnMsg[2] == "True";
        }

        #endregion Get

        #region Set

        /// <summary>
        /// 移動到指定Goal點
        /// </summary>
        /// <param name="idxGoal">目標Goal點索引值</param>
        public void Run(int idxGoal) {
            /*-- 若是路徑資料則開始接收資料 --*/
            string[] rtnMsg = SendMsg($"Set:Run:{idxGoal}");
            if ((rtnMsg?.Length ?? 0) > 3 &&
                rtnMsg[1] == "Run" &&
                rtnMsg[3] == "Done") {
                StartReciePath();
            }
        }

        /// <summary>
        /// 設定車子模式
        /// </summary>
        /// <param name="mode"></param>
        public void SetCarMode(WorkMode mode) {
            SendMsg($"Set:Mode:{mode}");
        }

        /// <summary>
        /// 設定掃描的地圖名稱
        /// </summary>
        /// <param name="oriName">掃描的ori檔名</param>
        public void SetScanName(string oriName) {
            if (oriName.Contains('.')) {
                string[] split = oriName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if ((split?.Count() ?? 0) > 0) {
                    oriName = split[0];
                }
            }
            SendMsg($"Set:OriName:{oriName}.Ori");
        }

        /// <summary>
        /// 要求Server端載入指定地圖檔
        /// </summary>
        /// <param name="mapName"></param>
        public void OrderMap(string mapName) {
            SendMsg($"Set:MapName:{mapName}");
        }

        #endregion Set

        #region MotorControl

        /// <summary>
        /// 前進
        /// </summary>
        /// <param name="velocity">移動速度</param>
        public void Forward(int velocity) {
            MotorControl(MotionDirection.Forward, velocity);
        }

        /// <summary>
        /// 回傳伺服馬達是否ServoOn
        /// </summary>
        /// <returns></returns>
        public bool IsServoOn() {
            string[] rtnMsg = SendMsg("Get:IsOpen");
            return (rtnMsg?.Count() ?? 0) > 2 && bool.Parse(rtnMsg[2]);
        }

        /// <summary>
        /// 伺服馬達停止動作
        /// </summary>
        public void MotorStop() {
            SendMsg("Set:Stop");
        }

        /// <summary>
        /// 後退
        /// </summary>
        /// <param name="velocity">移動速度</param>
        public void Backward(int velocity) {
            MotorControl(MotionDirection.Backward, velocity);
        }

        /// <summary>
        /// 左轉
        /// </summary>
        /// <param name="velocity">移動速度</param>
        public void LeftTrun(int velocity) {
            MotorControl(MotionDirection.LeftTrun, velocity);
        }

        /// <summary>
        /// 右轉
        /// </summary>
        /// <param name="velocity">移動速度</param>
        public void RightTurn(int velocity) {
            MotorControl(MotionDirection.RightTurn, velocity);
        }

        /// <summary>
        /// 設定馬達激磁
        /// </summary>
        /// <param name="servoOn"></param>
        public void SetMotor(bool servoOn) {
            SendMsg($"Set:Servo{(servoOn ? "On" : "Off")}");
        }

        /// <summary>
        /// 設定車子速度
        /// </summary>
        /// <param name="velocity"></param>
        public void SetVelocity(int velocity) {
            SendMsg($"Set: WorkVelo:{velocity}:{velocity}");
        }

        #endregion MotorControl

        #region Enable/Require

        /// <summary>
        /// 激活車子資訊回傳
        /// </summary>
        /// <param name="enb">是否激活</param>
        /// <returns>是否正在激活中</returns>
        public virtual bool EnableCarInfoReturn(bool enb) {
            if (enb) {
                /*-- 開啟車子資訊讀取執行緒 --*/

                string[] rtnMsg = SendMsg("Get:Car:True:True");
                IsGettingLaser = (rtnMsg.Count() > 2 && rtnMsg[2] == "True");
                if (!IsGettingLaser) {
                    EnableCarInfoReturn(false);
                }
            } else {
                /*-- 關閉Socket與執行緒 --*/
                SendMsg("Get:Car:False");
                IsGettingLaser = false;
            }
            return IsGettingLaser;
        }

        /// <summary>
        /// 激活遠端檔案接收
        /// </summary>
        public void EnableFileRecive() {
            SendMsg("Send:map");
        }

        /// <summary>
        /// 要求傳送指定檔案
        /// </summary>
        /// <param name="type">要求的檔案類型</param>
        /// <param name="fileName">要求的檔案名稱</param>
        public virtual void RequireFile(FileType type, string fileName) {
            /*-- 向Server端發出檔案請求 --*/
            SendMsg($"Get:{type}:{fileName}");
        }

        /// <summary>
        /// 要求到指定Goal點的路徑
        /// </summary>
        /// <param name="idxGoal">目標Goal點索引值</param>
        public void RequirePath(int idxGoal) {
            /*-- 若是路徑資料則開始接收資料 --*/
            string[] rtnMsg = SendMsg($"Set:PathPlan:{idxGoal}");
            if ((rtnMsg?.Count() ?? 0) > 3 &&
                rtnMsg[1] == "PathPlan" &&
                rtnMsg[2] == "True") {
                StartReciePath();
            }
        }

        #endregion Enable/Require

        #endregion Implement - IClientCommand

        #region Function - Private Methods

        /// <summary>
        /// 開始接收路徑規劃資料
        /// </summary>
        protected abstract void StartReciePath();

        /// <summary>
        /// 方向控制
        /// </summary>
        /// <param name="direction">移動方向</param>
        /// <param name="velocity">移動速度</param>
        protected virtual void MotorControl(MotionDirection direction, int velocity) {
            string[] rtnMsg = SendMsg("Get:IsOpen");

            if (rtnMsg.Count() > 2 && bool.Parse(rtnMsg[2])) {
                if (direction == MotionDirection.Stop) {
                    SendMsg("Set:Stop");
                } else {
                    string cmd = string.Empty;
                    switch (direction) {
                        case MotionDirection.Forward:
                            cmd = $"Set:DriveVelo:{velocity}:{velocity}";
                            break;
                        case MotionDirection.Backward:
                            cmd = $"Set:DriveVelo:-{velocity}:-{velocity}";
                            break;
                        case MotionDirection.LeftTrun:
                            cmd = $"Set:DriveVelo:{velocity}:-{velocity}";
                            break;
                        case MotionDirection.RightTurn:
                            cmd = $"Set:DriveVelo:-{velocity}:{velocity}";
                            break;
                    }
                    SendMsg(cmd);
                    SendMsg("Set:Start");
                }
            }
        }

        /// <summary>
        /// 訊息傳送(會觸發事件)
        /// </summary>
        /// <param name="sendMseeage">傳送訊息內容</param>
        /// <param name="passChkConn">是否略過檢查連線狀態</param>
        /// <returns>Server端回應</returns>
        protected abstract string[] SendMsg(string sendMseeage, bool passChkConn = true);

        /// <summary>
        /// Socket傳輸內容發報
        /// </summary>
        /// <param name="msg"></param>
        protected void RaiseConsole(string msg) {
            ConsoleRefresh?.Invoke(msg);
        }

        /// <summary>
        /// 車子資訊更新發報
        /// </summary>
        /// <param name="info"></param>
        protected void RaiseCarinfoRefresh(CarInfo info) {
            CarInfoRefresh?.Invoke(info);
        }

        /// <summary>
        /// 檔案下載完畢發報
        /// </summary>
        /// <param name="fileName"></param>
        protected void RaiseFileDownload(string fileName) {
            FileDownload?.Invoke(fileName);
        }

        /// <summary>
        /// 路徑更新事件
        /// </summary>
        /// <param name="path"></param>
        protected void RaisePathRefresh(List<CartesianPos> path) {
            PathRefresh?.Invoke(path);
        }

        #endregion Funciton - Private Methods

    }

    /// <summary>
    /// 以Socket進行命令傳送的Client端
    /// </summary>
    public class CtSoxCmdClient : BaseClientCommand, ISoxCmdClient {

        #region Declaration - Fields

        /// <summary>
        /// 命令發送用<see cref="Socket"/>
        /// </summary>
        protected Socket mSoxCmd = null;

        /// <summary>
        /// 車子資訊接收用<see cref="Socket"/>
        /// </summary>
        protected SocketMonitor mSoxInfoRecive = null;

        /// <summary>
        /// 檔案接收用<see cref="Socket"/>
        /// </summary>
        protected SocketMonitor mSoxMonitorFile = null;

        /// <summary>
        /// 路徑接收用<see cref="Socket"/>
        /// </summary>
        protected SocketMonitor mSoxMonitorPath = null;

        /// <summary>
        /// Server端IP
        /// </summary>
        protected string mServerIP = "127.0.0.1";

        /// <summary>
        /// 命令發送埠
        /// </summary>
        protected int mCmdSendPort = (int)SoxPort.Cmd;

        /// <summary>
        /// 檔案接收埠
        /// </summary>
        protected int mFileRecivePort = (int)SoxPort.GetFile;

        /// <summary>
        /// 地圖發送埠
        /// </summary>
        protected int mSendMapPort = (int)SoxPort.SendFile;

        /// <summary>
        /// 車子資訊接收埠
        /// </summary>
        protected int mInfoRecivePort = (int)SoxPort.Info;

        /// <summary>
        /// 路徑規劃資料接收埠
        /// </summary>
        protected int mPathRecivePort = (int)SoxPort.Path;

        #endregion Declarartion - Fields

        #region Funciton - Constructors

        public CtSoxCmdClient() {
            mSoxInfoRecive = new SocketMonitor(mInfoRecivePort, tsk_RecvInfo);
            mSoxMonitorFile = new SocketMonitor(mFileRecivePort, tsk_RecvFile);
            mSoxMonitorPath = new SocketMonitor(mPathRecivePort, tsk_RecvPath);
        }

        #endregion Function - Constructors

        #region Implement - ISocketCommand

        /// <summary>
        /// Server端IP
        /// </summary>
        public string ServerIP {
            get { return mServerIP; }
            set { if (!IsConnected && mServerIP != value) mServerIP = value; }
        }

        /// <summary>
        /// 命令發送埠
        /// </summary>
        public int CmdSendPort {
            get { return mCmdSendPort; }
            set { if (!IsConnected && mCmdSendPort != value) mCmdSendPort = value; }
        }

        /// <summary>
        /// 檔案接收埠
        /// </summary>
        public int FileRecivePort {
            get { return mFileRecivePort; }
            set { if (!IsConnected && mFileRecivePort != value) mFileRecivePort = value; }
        }

        /// <summary>
        /// 地圖發送埠
        /// </summary>
        public int SendMapPort {
            get { return mSendMapPort; }
            set { if (!IsConnected && mSendMapPort != value) mSendMapPort = value; }
        }

        /// <summary>
        /// 車子資訊接收埠
        /// </summary>
        public int InfoRecivePort {
            get { return mInfoRecivePort; }
            set { if (!IsConnected && mInfoRecivePort != value) mInfoRecivePort = value; }
        }

        /// <summary>
        /// 路徑規劃資料接收埠
        /// </summary>
        public int PathRecivePort {
            get { return mPathRecivePort; }
            set { if (!IsConnected && mPathRecivePort != value) mPathRecivePort = value; }
        }

        /// <summary>
        /// 是否與Server已建立連線
        /// </summary>
        public bool IsConnected { get { return mSoxCmd?.Connected ?? false; } }

        /// <summary>
        /// 是否略過Socket傳輸
        /// </summary>
        public bool Bypass { get; set; } = false;

        /// <summary>
        /// 連線至Server
        /// </summary>
        /// <param name="ip">Server端IP</param>
        /// <param name="port">Server端Port</param>
        /// <returns>是否連線成功</returns>
        public bool Connect() {
            try {
                if (mSoxCmd != null) {
                    mSoxCmd.Close();
                }
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ServerIP), mCmdSendPort);
                mSoxCmd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mSoxCmd.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
                mSoxCmd.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);
                mSoxCmd.Connect(ipEndPoint);
            } catch {

            }
            return IsConnected;
        }

        /// <summary>
        /// 斷開與Server的連線
        /// </summary>
        /// <returns></returns>
        public void Disconnect() {
            if (IsConnected) {
                try {
                    mSoxCmd.Shutdown(SocketShutdown.Both);
                    mSoxCmd.Close();
                } catch (SocketException se) {
                    System.Console.WriteLine("[Socket DisConnect ]" + se.ToString());
                }
            }
        }

        /// <summary>
        /// Send file of server to client
        /// </summary>
        /// <param name="fileName">File name</param>
        public override void SendFile(string fileName) {
            string curMsg = "";
            try {
                IPAddress[] ipAddress = Dns.GetHostAddresses(ServerIP);
                IPEndPoint ipEnd = new IPEndPoint(ipAddress[0], mSendMapPort);
                /* Make IP end point same as Server. */
                Socket clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                /* Make a client socket to send data to server. */
                string filePath = DefMapDir;
                /* File reading operation. */
                fileName = fileName.Replace("\\", "/");
                while (fileName.IndexOf("/") > -1) {
                    filePath += fileName.Substring(0, fileName.IndexOf("/") + 1);
                    fileName = fileName.Substring(fileName.IndexOf("/") + 1);
                }
                byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
                if (fileNameByte.Length > 1024 * 1024 * 5) {
                    curMsg = "File size is more than 850kb, please try with small file.";
                    return;
                }
                curMsg = "Buffering ...";
                byte[] fileData = File.ReadAllBytes(filePath + fileName);
                /* Read & store file byte data in byte array. */
                byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
                /* clientData will store complete bytes which will store file name length, 
                file name & file data. */
                byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);
                /* File name length’s binary data. */
                fileNameLen.CopyTo(clientData, 0);
                fileNameByte.CopyTo(clientData, 4);
                fileData.CopyTo(clientData, 4 + fileNameByte.Length);
                /* copy these bytes to a variable with format line [file name length]
                [file name] [ file content] */
                curMsg = "Connection to server ...";
                clientSock.Connect(ipEnd);
                /* Trying to connection with server. */
                curMsg = "File sending...";
                clientSock.Send(clientData);
                /* Now connection established, send client data to server. */
                curMsg = "Disconnecting...";
                clientSock.Close();
                fileNameByte = null;
                clientData = null;
                fileNameLen = null;
                /* Data send complete now close socket. */
                curMsg = "File transferred.";
            } catch (Exception ex) {
                if (ex.Message == "No connection could be made because the target machine actively refused it")
                    curMsg = "File Sending fail. Because server not running.";
                else
                    curMsg = "File Sending fail." + ex.Message;
            }
        }

        #endregion Implement - IScoketCommand

        #region Implement -IClientCommand

        /// <summary>
        /// 要求傳送指定檔案
        /// </summary>
        /// <param name="type">要求的檔案類型</param>
        /// <param name="fileName">要求的檔案名稱</param>
        public override void RequireFile(FileType type, string fileName) {
            /*-- 開啟執行緒準備接收檔案 --*/
            mSoxMonitorFile.Start();
            if (!Bypass) {
                base.RequireFile(type, fileName);
            }
        }

        /// <summary>
        /// 要求所有Goal點名稱
        /// </summary>
        /// <remarks>
        /// 回傳格式為 Goal List:'Goal1','Goal2',...
        /// </remarks>
        /// <returns></returns>
        public override string GetGoalNames() {
            string goalList = string.Empty;
            if (Bypass) {
                return "GoalA,GoalB,GoalC";
            } else {
                return base.GetGoalNames();
            }
        }

        /// <summary>
        /// 激活車子資訊回傳
        /// </summary>
        /// <param name="enb">是否激活</param>
        /// <returns>是否正在激活中</returns>
        public override bool EnableCarInfoReturn(bool enb) {
            if (Bypass) {
                IsGettingLaser = enb;
            } else {
                if (enb) {//開啟Socket與執行緒準備接收
                    mSoxInfoRecive.Listen();
                    IsGettingLaser = base.EnableCarInfoReturn(enb);
                } else {//關閉Socket與執行緒
                    IsGettingLaser = base.EnableCarInfoReturn(enb);
                    mSoxInfoRecive.Stop();
                }
            }
            return IsGettingLaser;
        }

        /// <summary>
        /// 取得指定類型檔案清單
        /// </summary>
        /// <param name="type"></param>
        /// <returns>檔案清單格式為 'File1','FIle2','File3',...</returns>
        public override string GetFileNames(FileType type) {
            return Bypass ? $"{type}1,{type}2,{type}3" : base.GetFileNames(type);
        }

        #endregion Implement - IClientCommand

        #region Funciton - Private Methods

        /// <summary>
        /// 開始接收路徑資料
        /// </summary>
        protected override void StartReciePath() {
            mSoxMonitorPath.Start();
        }

        /// <summary>
        /// 訊息傳送(會觸發事件)
        /// </summary>
        /// <param name="sendMseeage">傳送訊息內容</param>
        /// <param name="passChkConn">是否略過檢查連線狀態</param>
        /// <returns>Server端回應</returns>
        protected override string[] SendMsg(string sendMseeage, bool passChkConn = true) {
            string[] rtnMsg = null;
            /*-- 顯示發送出去的訊息 --*/
            string msg = $"{DateTime.Now} [Client] : {sendMseeage}";
            RaiseConsole(msg);
            if (IsServerAlive) {//Server運作中
                /*-- 等待Server端的回應 --*/
                msg = SendStrMsg(sendMseeage);
                //string rtnMsg = SendStrMsg(mHostIP, mRecvCmdPort, sendMseeage );

                /*-- 顯示Server端回應 --*/
                msg = $"{DateTime.Now} [Server] : {msg}\r\n";
                RaiseConsole(msg);

                rtnMsg = msg.Trim().Split(':');
            } else {
                rtnMsg = sendMseeage.Split(':');
                if (Bypass) {//Bypass =>不論如何回傳Ture模擬正常運作
                    rtnMsg[rtnMsg.Length] = "True";
                } else if (passChkConn) {//連線未建立下的命令皆回傳False
                    rtnMsg[rtnMsg.Length] = "False";
                } else {//連線測試
                    rtnMsg = SendStrMsg(sendMseeage).Trim().Split(':');
                }
                msg = $"{DateTime.Now} [Server] : {string.Join(":", rtnMsg)}\r\n";
                RaiseConsole(msg);
            }
            return rtnMsg;
        }

        /// <summary>
        /// 訊息傳送(具體Socket交握實現，但是不會觸發事件)
        /// </summary>
        /// <param name="serverIP">伺服端IP</param>
        /// <param name="requerPort">通訊埠號</param>
        /// <param name="sendMseeage">傳送訊息內容</param>
        /// <returns>Server端回應</returns>
        private string SendStrMsg(string sendMseeage) {
            //可以在字串編碼上做文章，可以傳送各種資訊內容，目前主要有三種編碼方式：
            //1.自訂連接字串編碼－－微量
            //2.JSON編碼--輕量
            //3.XML編碼--重量
            int state;
            byte[] recvBytes = new byte[8192];//開啟一個緩衝區，存儲接收到的資訊
            try {

                byte[] sendContents = Encoding.UTF8.GetBytes(sendMseeage + "\r\n");
                state = mSoxCmd.Send(sendContents, sendContents.Length, 0);//發送二進位資料
                state = mSoxCmd.Receive(recvBytes);
                string strRecvCmd = Encoding.Default.GetString(recvBytes);//
                strRecvCmd = strRecvCmd.Split(new char[] { '\0' })[0];
                sendContents = null;
                return strRecvCmd;
            } catch (SocketException se) {
                System.Console.WriteLine("SocketException : {0}", se.ToString());
                return "False";
            } catch (ArgumentNullException ane) {
                System.Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                return "False";
            } catch (Exception ex) {
                System.Console.Write(ex.Message);
                return "False";
            } finally {
                recvBytes = null;
            }

        }

        /// <summary>
        /// 移動控制
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="velocity"></param>
        protected override void MotorControl(MotionDirection direction, int velocity) {
            if (!Bypass) base.MotorControl(direction, velocity);
        }

        /// <summary>
        /// 將路徑封包拆解重新包裝
        /// </summary>
        /// <param name="pack">路徑封包</param>
        private List<Line> PathEncoder(string pack) {
            string[] pathArray = pack.Split(',');
            List<Line> rtnLine = new List<Line>();
            for (int i = 0; i < pathArray.Length - 5; i += 2) {
                rtnLine.Add(new Line(
                    int.Parse(pathArray[i]),
                    int.Parse(pathArray[i + 1]),
                    int.Parse(pathArray[i + 2]),
                    int.Parse(pathArray[i + 3])
                    )
                );
            }
            return rtnLine;
        }

        private List<CartesianPos> Encoder(string pack) {
            string[] pathArray = pack.Split(',');
            List<CartesianPos> rtnPoints = new List<CartesianPos>();
            rtnPoints.Add(new CartesianPos(int.Parse(pathArray[0]), int.Parse(pathArray[1])));
            for (int i = 0; i < pathArray.Length - 5; i += 4) {
                rtnPoints.Add(new CartesianPos(
                    int.Parse(pathArray[i + 2]),
                    int.Parse(pathArray[i + 3])
                    )
                );
            }
            return rtnPoints;
        }

        /// <summary>
        /// 車子資訊接收執行緒
        /// </summary>
        protected void tsk_RecvInfo(object obj) {
            SocketMonitor soxMonitor = obj as SocketMonitor;
            Socket sRecvCmdTemp = soxMonitor.Accept();
            try {
                while (IsGettingLaser) {

                    SpinWait.SpinUntil(() => false, 1);//每個執行緒內部的閉環裡面都要加個「短時間」睡眠，使得執行緒佔用資源得到及時釋放
                                                       //Thread.Sleep(1);
                    byte[] recvBytes = new byte[1024 * 500];//開啟一個緩衝區，存儲接收到的資訊
                    sRecvCmdTemp.Receive(recvBytes); //將讀得的內容放在recvBytes中
                    string strRecvCmd = Encoding.Default.GetString(recvBytes);//
                                                                              //程式運行到這個地方，已經能接收到遠端發過來的命令了
                    strRecvCmd = strRecvCmd.Split(new char[] { '\0' })[0];
                    //Console.WriteLine("[Server] : " + strRecvCmd);

                    //*************
                    //解碼命令，並執行相應的操作----如下面的發送本機圖片
                    //*************

                    string[] strArray = strRecvCmd.Split(':');
                    recvBytes = null;
                    CarInfo carInfo = null;
                    if (CarInfo.TryParse(strRecvCmd, out carInfo)) {
                        RaiseCarinfoRefresh(carInfo);
                        sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Get:Car:True:True"));
                    } else {
                        sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Get:Car:True:False"));
                    }

                    strRecvCmd = null;
                    strArray = null;
                }
            } catch (SocketException se) {
                Console.WriteLine("[Status Recv] : " + se.ToString());
                //MessageBox.Show("目標拒絕連線");
            } catch (Exception ex) {
                Console.Write(ex.Message);
                //throw ex;
            } finally {
                sRecvCmdTemp?.Close();
                sRecvCmdTemp = null;
            }
        }

        /// <summary>
        /// 檔案接收執行緒 20170911
        /// </summary>
        /// <param name="obj"></param>
        protected void tsk_RecvFile(object obj) {
            int fileNameLen = 0;
            int recieve_data_size = 0;
            int first = 1;
            int receivedBytesLen = 0;
            double cal_size = 0;
            SocketMonitor soxMonitor = obj as SocketMonitor;
            Socket clientSock = null;
            BinaryWriter bWrite = null;
            //MemoryStream ms = null;
            string curMsg = "Stopped";
            string fileName = "";
            try {
                if (!Bypass) {
                    clientSock = soxMonitor.Accept();
                    curMsg = "Running and waiting to receive file.";

                    //Socket clientSock = sRecvFile.Accept();
                    //clientSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
                    //clientSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);//設置發送資料超時
                    //clientSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 1024); //設置發送緩衝區大小 1K
                    /* When request comes from client that accept it and return 
                    new socket object for handle that client. */
                    byte[] clientData = new byte[1024 * 10000];
                    do {
                        receivedBytesLen = clientSock.Receive(clientData);
                        curMsg = "Receiving data...";
                        if (first == 1) {
                            fileNameLen = BitConverter.ToInt32(clientData, 0);
                            /* I've sent byte array data from client in that format like 
                            [file name length in byte][file name] [file data], so need to know 
                            first how long the file name is. */
                            fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);
                            /* Read file name */
                            if (!Directory.Exists(DefMapDir)) {
                                Directory.CreateDirectory(DefMapDir);
                            }
                            if (File.Exists(DefMapDir + "/" + fileName)) {
                                File.Delete(DefMapDir + "/" + fileName);
                            }
                            bWrite = new BinaryWriter(File.Open(DefMapDir + "/" + fileName, FileMode.OpenOrCreate));
                            /* Make a Binary stream writer to saving the receiving data from client. */
                            // ms = new MemoryStream();
                            bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);
                            //ms.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 -
                            //fileNameLen);
                            //寫入資料 ，呈現於BITMAP用
                            /* Read remain data (which is file content) and 
                            save it by using binary writer. */
                            curMsg = "Saving file...";
                            /* Close binary writer and client socket */
                            curMsg = "Received & Saved file; Server Stopped.";
                        } else //第二筆接收為資料  
                          {
                            //-----------  
                            fileName = Encoding.ASCII.GetString(clientData, 0,
                            receivedBytesLen);
                            //-----------  
                            bWrite.Write(clientData/*, 4 + fileNameLen, receivedBytesLen - 4 -
                            fileNameLen*/, 0, receivedBytesLen);
                            //每筆接收起始 0 結束為當次Receive長度  
                            //ms.Write(clientData, 0, receivedBytesLen);
                            //寫入資料 ，呈現於BITMAP用  
                        }
                        recieve_data_size += receivedBytesLen;
                        //計算資料每筆資料長度並累加，後面可以輸出總值看是否有完整接收  
                        cal_size = recieve_data_size;
                        cal_size /= 1024;
                        cal_size = Math.Round(cal_size, 2);

                        first++;
                        SpinWait.SpinUntil(() => false, 10); //每次接收不能太快，否則會資料遺失  

                    } while (clientSock.Available != 0);
                    clientData = null;
                } else {
                    SpinWait.SpinUntil(() => false, 1000);
                    fileName = "FileName";
                }
                RaiseFileDownload(fileName);
            } catch (SocketException se) {
                Console.WriteLine("SocketException : {0}", se.ToString());
                //MessageBox.Show("檔案傳輸失敗!");
                curMsg = "File Receiving error.";
            } catch (Exception ex) {
                Console.WriteLine("[RecvFiles]" + ex.ToString());
                curMsg = "File Receiving error.";
            } finally {
                bWrite?.Close();
                soxMonitor.Stop();
            }
        }

        /// <summary>
        /// 路徑接收執行緒
        /// </summary>
        protected void tsk_RecvPath(object obj) {
            SocketMonitor soxMonitor = obj as SocketMonitor;
            //Socket sRecvCmdTemp = sRecvCmd.Accept();//Accept 以同步方式從偵聽通訊端的連接請求佇列中提取第一個掛起的連接請求，然後創建並返回新的 Socket
            Socket sRecvCmdTemp = soxMonitor.Accept();
            //sRecvCmdTemp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
            //sRecvCmdTemp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);//設置發送資料超時
            //sRecvCmdTemp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 1024); //設置發送緩衝區大小 1K
            //sRecvCmdTemp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 9000);//設置接收緩衝區大小1K

            try {
                sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Path:Require"));
                byte[] recvBytes = new byte[1024 * 500];//開啟一個緩衝區，存儲接收到的資訊
                sRecvCmdTemp.Receive(recvBytes); //將讀得的內容放在recvBytes中
                string strRecvCmd = Encoding.Default.GetString(recvBytes);
                //程式運行到這個地方，已經能接收到遠端發過來的命令了
                //Console.WriteLine("[Server] : " + strRecvCmd);
                //*************
                //解碼命令，並執行相應的操作----如下面的發送本機圖片
                //*************
                strRecvCmd = strRecvCmd.Split(new char[] { '\0' })[0];

                string[] strArray = strRecvCmd.Split(':');
                recvBytes = null;
                if (strArray[0] == "Path" && !string.IsNullOrEmpty(strArray[1])) {
                    RaisePathRefresh(Encoder(strArray[1]));
                }
                //else
                //    sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Path:False"));

                strRecvCmd = null;
                strArray = null;
                sRecvCmdTemp.Close();

            } catch (SocketException se) {
                Console.WriteLine("[Status Recv] : " + se.ToString());
                //MessageBox.Show("目標拒絕連線");
            } catch (Exception ex) {
                Console.Write(ex.Message);
                //throw ex;
            } finally {
                sRecvCmdTemp.Close();
                sRecvCmdTemp = null;
            }
        }

        #endregion Function - Private Methods

    }

    /// <summary>
    /// Server端命令交握方法實作
    /// </summary>
    public abstract class BaseServerCmooand : IServerCommand {

        #region Declaration - Fields

        protected bool bSendStatus = false;

        protected IAGV mAGV = null;

        protected Thread t_Work;
        protected Thread t_ConsoleUptate;
        protected Thread t_Server;
        protected Thread t_Recver;
        protected Thread t_SendPath;
        protected Thread t_Sender;

        #endregion Declaration - Fields

        #region Implement - IServerCommand

        public IAGV RefAGV { get { return mAGV; } set { mAGV = value; } }

        /// <summary>
        /// CarID設定委派
        /// </summary>
        public CommandEvents.Server.DelSetCarID SetCarID { get; set; } = null;

        /// <summary>
        /// CarID取得委派
        /// </summary>
        public CommandEvents.Server.DelGetCarID GetCarID { get; set; } = null;

        /// <summary>
        /// Ori檔名設定委派
        /// </summary>
        public CommandEvents.Server.DelSetOriName SetOriName { get; set; } = null;

        /// <summary>
        /// Map檔命設定委派
        /// </summary>
        public CommandEvents.Server.DelSetMapName SetMapName { get; set; } = null;

        /// <summary>
        /// Map檔讀取委派
        /// </summary>
        public CommandEvents.Server.DelReadMapFile ReadMapFile { get; set; } = null;

        /// <summary>
        /// 規劃到Goal點的路徑
        /// </summary>
        public CommandEvents.Server.DelPathPlan PathPlan { get; set; } = null;

        /// <summary>
        /// Goal點索引取得Goal點
        /// </summary>
        public CommandEvents.Server.DelIdxToGoal IdxToGoal { get; set; } = null;

        /// <summary>
        /// 尋找Goal點
        /// </summary>
        public CommandEvents.Server.DelFindGoal FindGoal { get; set; }

        /// <summary>
        /// 地圖掃描
        /// </summary>
        public CommandEvents.Server.DelRecordMap RecordMap { get; set; }

        /// <summary>
        /// 取得Goal點清單
        /// </summary>
        public CommandEvents.Server.DelGetGoalNames GetGoalNames { get; set; }

        /// <summary>
        /// 取得Ori檔清單
        /// </summary>
        public CommandEvents.Server.DelGetOriNames GetOriNames { get; set; }

        /// <summary>
        /// 取得Map檔清單
        /// </summary>
        public CommandEvents.Server.DelGetMapNames GetMapNames { get; set; }

        /// <summary>
        /// 取得Ori檔目錄路徑
        /// </summary>
        public CommandEvents.Server.DelGetOriDirectory GetOriDirectory { get; set; }

        /// <summary>
        /// 取得Map檔目錄路徑
        /// </summary>
        public CommandEvents.Server.DelGetMapDirectory GetMapDirectory { get; set; }

        /// <summary>
        /// 取得當前電池電量
        /// </summary>
        public CommandEvents.Server.DelGetPower GetPower { get; set; }

        #endregion Implement - IServerCommand

        #region Function - Private Methods

        #region CmdAnls

        /// <summary>
        /// 命令分類
        /// </summary>
        /// <param name="strArray"></param>
        /// <param name="socket_server"></param>
        protected virtual void AnlsCmd(string[] strArray, ICmdSender socket_server) {
            switch (strArray[0]) {
                case "Set":
                    AnlsSet(strArray, socket_server);
                    break;
                case "Get":
                    AnlsGet(strArray, socket_server);
                    break;
                case "Send":
                    AnlsSend(strArray, socket_server);
                    break;
                default:
                    socket_server.Send(string.Join(":", strArray) + "\r\n");
                    break;
            }
        }

        /// <summary>
        /// Set類命令分析
        /// </summary>
        /// <param name="strArray"></param>
        /// <param name="socket_server"></param>
        protected void AnlsSet(string[] strArray, ICmdSender socket_server) {
            bool status = false;
            CartesianPos goal = null;
            switch (strArray[1]) {
                case "Start":
                    status = mAGV?.Drive() ?? false;
                    socket_server.Send($"Set:Start:{status}");
                    break;
                case "Stop":
                    status = mAGV?.StopDrive() ?? false;
                    socket_server.Send($"Set:Stop:{status}");
                    break;
                case "POS":
                    if (mAGV != null) {
                        mAGV.Position = new double[3] { double.Parse(strArray[2]), double.Parse(strArray[3]), double.Parse(strArray[4]) };
                    }
                    socket_server.Send("Set:POS:True");
                    break;
                case "ServoOn":
                    status = mAGV?.DriveOn() ?? false;
                    socket_server.Send($"Set:ServoOn:{status}");
                    break;
                case "ServoOff":
                    status = mAGV?.DriveOff() ?? false;
                    socket_server.Send($"Set:ServoOff:{status}");
                    break;
                case "DriveVelo":
                    if (mAGV != null) {
                        mAGV.MotorVelocity = new int[2] { int.Parse(strArray[2]), int.Parse(strArray[3]) };
                    }
                    socket_server.Send("Set:DriveVelo:True");
                    break;
                case "WorkVelo":
                    if (mAGV != null) {
                        mAGV.DriveSpeed = int.Parse(strArray[2]);
                    }
                    socket_server.Send("Set:WorkVelo:True");
                    break;
                case "Acce":
                    status = mAGV?.SetAcceleration(int.Parse(strArray[2])) ?? false;
                    socket_server.Send($"Set:Acce:{status}");
                    break;
                case "Dece":
                    status = mAGV?.SetDcceleration(int.Parse(strArray[2])) ?? false;
                    socket_server.Send($"Set:Dece:{status}");
                    break;
                case "StopMode":
                    bool stop = bool.Parse(strArray[2]);
                    status = mAGV?.SetBrakeMode(stop) ?? false;
                    socket_server.Send($"Set:StopMode:{status}:{stop}");
                    break;
                case "ID":
                    //carID = int.Parse(strArray[2]);
                    string id = strArray[2];
                    SetCarID?.Invoke(int.Parse(id));
                    socket_server.Send($"Set:ID:True:{id}");
                    break;
                case "OriName":
                    //oriPath = oriDirectory + strArray[2];
                    string oriName = strArray[2];
                    SetOriName?.Invoke(oriName);
                    socket_server.Send($"Set:OriName:True:{oriName}");
                    break;
                case "MapName":
                    //mapPath = mapDirectory + strArray[2];
                    //if (File.Exists(mapPath)) {
                    string mapName = strArray[2];
                    if (SetMapName?.Invoke(mapName) ?? false) {
                        socket_server.Send($"Set:MapName:True:{mapName}");
                        ReadMapFile?.Invoke();
                    } else {
                        socket_server.Send("Set:MapName:False:Map not exists");
                    }
                    break;
                case "PathPlan":
                    socket_server.Send($"Set:PathPlan:True:{strArray[2]}");
                    int idxGoal = int.Parse(strArray[2]);
                    StartSendPath(idxGoal, socket_server);
                    break;
                case "Goto":
                    //CartesianPos goal = null;
                    //string goalName = strArray[2];
                    //foreach (CartesianPos item in goalList) {
                    //    if (item.name == strArray[2]) {
                    //        goal = item;
                    //        break;
                    //    }
                    //}
                    string goalName = strArray[2];
                    goal = FindGoal?.Invoke(goalName);
                    if (goal != null) {
                        socket_server.Send($"Set:Goto:True:{goalName}");
                        StartSendPath(goal, socket_server);
                        mAGV.DoPathMovement();
                    } else
                        socket_server.Send($"Set:Goto:False:{goalName}");

                    break;
                case "Run":
                    socket_server.Send($"Set:Run:True:{strArray[2]}");
                    idxGoal = int.Parse(strArray[2]);
                    //goal = IdxToGoal(idxGoal);
                    //mAGV.searchSetting.goal.Clear();
                    //mAGV.searchSetting.goal.Add(goalList[idxGoal]);
                    //mAGV.searchSetting.radius = 100;
                    //mAGV.DoPathPlanning();
                    //comm_SendPath = new ClientCommunication(socket_server.StrIPAddress, port_sendPath);
                    //CtThread.CreateThread(ref t_SendPath, "t_SendPath: ", SendPath);
                    StartSendPath(idxGoal, socket_server);
                    mAGV.DoPathMovement();


                    break;
                case "Mode":
                    string mode = strArray[2];
                    switch (mode) {
                        case "Idle":
                            mAGV.mode = WorkMode.Idle;
                            break;
                        case "Work":
                            mAGV.mode = WorkMode.Work;
                            break;
                        case "Map":
                            mAGV.mode = WorkMode.Map;
                            if (RecordMap != null) {
                                CtThread.CreateThread(ref t_Work, "Thread_Console Update", () => RecordMap());
                            }
                            break;
                    }
                    socket_server.Send($"Set:Mode:True:{mode}");
                    break;

                case "ThreadReset":
                    CtThread.KillThread(ref t_Recver, 500);
                    CtThread.KillThread(ref t_SendPath, 500);
                    CtThread.KillThread(ref t_Sender, 500);

                    break;

                default:
                    socket_server.Send(string.Join(":", strArray) + ":False");
                    break;
            }
        }

        /// <summary>
        /// Get類命令分析
        /// </summary>
        /// <param name="strArray"></param>
        /// <param name="socket_server"></param>
        protected void AnlsGet(string[] strArray, ICmdSender socket_server) {
            bool Status = false;
            string rtnVal = string.Empty;
            switch (strArray[1]) {
                case "Hello":
                    socket_server.Send("Get:Hello:True");
                    break;
                //case "GoalInfo":
                //    {
                //        string name = strArray[2];
                //        foreach (CartesianPos goal in goalList)
                //        {
                //            if (goal.name == name)
                //            {
                //               socket_server.Send(Encoding.UTF8.GetBytes("Get:GoalInfo:True:" +
                //                    goal.name + ":" + goal.x + ":" + goal.y + ":" + goal.theta + "\r\n"));
                //                break;
                //            }
                //        }
                //    }
                //    break;
                case "GoalList": {
                        //List<string> data = goalList.ConvertAll(v => v.name);
                        List<string> data = GetGoalNames?.Invoke();
                        string str = data != null ? string.Join(",", data) : null;
                        socket_server.Send($"Get:GoalList:True:{str}");
                    }
                    break;
                case "OriList": {
                        //DirectoryInfo d = new DirectoryInfo(oriDirectory);  //Create directory object 
                        //FileInfo[] Files = d.GetFiles("*.ori");              //Getting Text files
                        //string[] tmp = Array.ConvertAll(Files, v => v.Name);
                        string[] tmp = GetOriNames?.Invoke();
                        string str = tmp != null ? string.Join(",", tmp) : null;
                        socket_server.Send($"Get:OriList:True:{str}");
                    }
                    break;
                case "MapList": {
                        //DirectoryInfo d = new DirectoryInfo(mapDirectory);  //Create directory object 
                        //FileInfo[] Files = d.GetFiles("*.map");              //Getting Text files
                        //string[] tmp = Array.ConvertAll(Files, v => v.Name);
                        string[] tmp = GetMapNames?.Invoke();
                        string str = tmp != null ? string.Join(",", tmp) : null;
                        socket_server.Send($"Get:MapList:True:{str}");
                    }
                    break;
                case "Ori": {
                        string oriName = strArray[2];
                        string oriDir = GetOriDirectory?.Invoke();
                        SendFile(socket_server, oriDir, oriName);
                        //SendFile(socket_server.StrIPAddress, port_sendFile, oriDirectory, strArray[2]); //發送本機Ori檔
                        socket_server.Send("Get:Ori:True");
                    }
                    break;
                case "Map": {
                        string mapName = strArray[2];
                        string mapDir = GetMapDirectory?.Invoke();
                        SendFile(socket_server, mapDir, mapName);
                        //SendFile(socket_server.StrIPAddress, port_sendFile, mapDirectory, strArray[2]); //發送本機Map檔
                        socket_server.Send("Get:Map:True");
                    }
                    break;
                case "Info":
                    bool[] info;
                    if (Status = mAGV != null) {
                        mAGV.GetMotorInfo(out info);
                        rtnVal = $":{info[0]}:{info[1]}:{info[2]}";
                    }
                    socket_server.Send($"Get:Info:{Status}{rtnVal}");
                    break;
                case "Velo":
                    int lVelo = 0;
                    int rVelo = 0;
                    if (Status = mAGV?.GetDriveVelocity(out lVelo, out rVelo) ?? false) {
                        rtnVal = $":{lVelo}:{rVelo}";
                    }
                    socket_server.Send($"Get:Velo:{Status}:{rtnVal}");
                    break;
                case "Acce":
                    int[] accel = mAGV?.MotorAccel ?? null;
                    if (Status = (accel?.Count() ?? 0) > 1) {
                        rtnVal = $":{accel[0]}:{accel[1]}";
                    }
                    socket_server.Send($"Get:Acce:{Status}:{rtnVal}");
                    break;
                case "Dece":
                    int[] dccel = mAGV?.MotorDccel ?? null;
                    if (Status = (dccel?.Count() ?? 0) > 1) {
                        rtnVal = $":{dccel[0]}:{dccel[1]}";
                    }
                    socket_server.Send($"Get:Dece:{Status}:{rtnVal}");
                    break;
                case "IsOpen":
                    Status = mAGV != null;
                    bool cnn = Status ? mAGV.DriveConnectState : false;
                    socket_server.Send($"Get:IsOpen:{Status}:{cnn}");
                    break;
                case "Encoder":
                    long[] encoder = mAGV?.EncoderValue;
                    if (Status = (encoder?.Count() ?? 0) > 1) {
                        rtnVal = $":{encoder[0]}:{encoder[1]}";
                    }
                    socket_server.Send($"Get:Encoder:{Status}:{rtnVal}");
                    break;
                case "StopMode":
                    bool stop = (Status = mAGV != null) ? mAGV.BrakeMode : false;
                    socket_server.Send($"Get:StopMode:{Status}:{stop}");
                    break;
                case "Laser":
                    List<int> laserData = mAGV.LaserDistanceData;
                    string laserVar = string.Join(",", laserData);
                    byte[] byteData = Encoding.UTF8.GetBytes("Get:Laser:True:" + laserVar + "\r\n");
                    socket_server.Send(byteData);
                    laserData = null;
                    break;
                case "Car":
                    if (strArray[2] == "True" && t_Sender == null) {
                        bool getLaser = bool.Parse(strArray[3]);
                        StartSendCarStatus(getLaser, socket_server);
                    } else if (strArray[2] == "False" && t_Sender != null) {
                        bSendStatus = false;
                        CtThread.KillThread(ref t_Sender);
                    }

                    socket_server.Send(Encoding.UTF8.GetBytes("Get:Car:True:" + bSendStatus.ToString() + "\r\n"));
                    break;

                default:
                    socket_server.Send(string.Join(":", strArray) + ":False\r\n");
                    break;
            }
        }

        /// <summary>
        /// Send類命令分析
        /// </summary>
        /// <param name="strArray"></param>
        /// <param name="socket_server"></param>
        protected void AnlsSend(string[] strArray, ICmdSender socket_server) {
            bool correct;
            string directory = "";
            string strRecvCmd = string.Join(",", strArray);
            switch (strArray[1]) {
                case "ori":
                    correct = true;
                    directory = GetOriDirectory?.Invoke();
                    socket_server.Send(strRecvCmd + ":True\r\n");
                    break;
                case "map":
                    correct = true;
                    directory = GetMapDirectory?.Invoke();
                    socket_server.Send(strRecvCmd + ":True\r\n");
                    break;
                default:
                    correct = false;
                    socket_server.Send(strRecvCmd + ":Format Fault\r\n");
                    break;
            }
            if (correct) {
                CtThread.CreateThread(ref t_Recver, "mTdRecvMap: ", StartRecvFiles);
                t_Recver.Start(directory);
            }
        }

        #endregion CmdAnls

        /// <summary>
        /// 以Goal點名稱進行路徑規劃並傳送
        /// </summary>
        /// <param name="goalName"></param>
        /// <param name="sender"></param>
        private void StartSendPath(string goalName, ICmdSender sender) {
            StartSendPath(FindGoal?.Invoke(goalName), sender);
        }

        /// <summary>
        /// 以Goal點索引進行路徑規劃並傳送
        /// </summary>
        /// <param name="idxGoal"></param>
        /// <param name="sender"></param>
        private void StartSendPath(int idxGoal, ICmdSender sender) {
            StartSendPath(IdxToGoal?.Invoke(idxGoal), sender);
        }

        /// <summary>
        /// [Base]開始傳送路徑規劃資料
        /// </summary>
        /// <param name="goal">要規劃的Goal點</param>
        protected virtual void StartSendPath(CartesianPos goal, ICmdSender sender) {
            if (PathPlan != null && goal != null) {
                PathPlan(goal);
                //mAGV.searchSetting.goal.Clear();
                //mAGV.searchSetting.goal.Add(goalList[goalIndex]);
                //mAGV.searchSetting.radius = 100;
                //mAGV.DoPathPlanning();

                CtThread.CreateThread(ref t_SendPath, "t_SendPath: ", SendPath);
            }
        }

        /// <summary>
        /// [Base] Send path data
        /// </summary>
        protected virtual void SendPath() {
            List<CartesianPos> path = new List<CartesianPos>();
            StringBuilder pathData = new StringBuilder();
            pathData.Append("Path:");
            while (mAGV.f_pathSearching) {
                SpinWait.SpinUntil(() => false, 1);
            }
            path.AddRange(mAGV.WorkPath);
            if (path.Count > 0) {
                foreach (CartesianPos pos in path) {
                    pathData.Append(pos.x.ToString() + "," + pos.y.ToString() + ",");
                }
            }
            SendPath(pathData.ToString() + Environment.NewLine);
            //comm_SendPath.SendString(pathData.ToString() + Environment.NewLine, Encoding.UTF8);
        }

        /// <summary>
        /// [Base]將路徑規劃資料傳輸給Server端
        /// </summary>
        /// <param name="path"></param>
        protected abstract void SendPath(string path);

        /// <summary>
        /// [Base]開始傳送車子資訊
        /// </summary>
        /// <param name="getLaser"></param>
        protected virtual void StartSendCarStatus(bool getLaser, ICmdSender sender) {
            bSendStatus = true;
            CtThread.CreateThread(ref t_Sender, "mTdSender: ", SendCarStatus);
            t_Sender.Start(getLaser);
        }

        /// <summary>
        /// [Base]Send laser data and car position to client
        /// </summary>
        protected virtual void SendCarStatus(object getLaser) {
            List<int> laserData;
            double[] carPos;
            byte[] recvBytes = new byte[1024];
            string laserVar = "";
            string strRecvCmd;
            string out_data;
            int carID = GetCarID?.Invoke() ?? 0;
            int powerPercent = GetPower?.Invoke() ?? 0;
            while (bSendStatus) {
                SendStatusCnn();
                //if (!comm_SendAGVInfo.isConnected)
                //    comm_SendAGVInfo.Connect();

                //Request laser and car position from device
                carPos = mAGV.Position;
                if ((bool)getLaser) {
                    laserData = mAGV.LaserDistanceData;
                    laserVar = string.Join(",", laserData);
                }
                out_data = $"Get:Car:{carID}:{carPos[0]}:{carPos[1]}:{carPos[2]}:{powerPercent}:{laserVar}:{mAGV.mode}{Environment.NewLine}";
                SendStatus(out_data);
                //comm_SendAGVInfo.SendString(out_data, Encoding.UTF8);

                //Receive client reply
                //strRecvCmd = comm_SendAGVInfo.ReadString(Encoding.UTF8);
                strRecvCmd = RecvSendStatusAck();
                strRecvCmd = strRecvCmd.Split(new char[] { '\0' })[0];
            }
        }

        /// <summary>
        /// [Base] 建立車子資訊傳輸通訊連接
        /// </summary>
        protected abstract void SendStatusCnn();

        /// <summary>
        /// [Base]傳送車子資訊
        /// </summary>
        /// <param name="data"></param>
        protected abstract void SendStatus(string data);

        /// <summary>
        /// [Base]讀取車子資訊傳送回應
        /// </summary>
        /// <returns></returns>
        protected abstract string RecvSendStatusAck();

        /// <summary>
        /// [Base]傳送檔案
        /// </summary>
        /// <param name="socket_server"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        protected abstract void SendFile(ICmdSender socket_server, string filePath, string fileName);

        /// <summary>
        /// [Base]Receive file from client
        /// </summary>
        /// <param name="directory"></param>
        protected abstract void StartRecvFiles(object directory);

        /// <summary>
        /// [Base]取得地圖接收器
        /// </summary>
        /// <returns></returns>
        protected abstract ICmdReceiver GetMapReceiver();

        /// <summary>
        /// [Base] 接收檔案寫入指定路徑
        /// </summary>
        /// <param name="socket_map">檔案接收器</param>
        /// <param name="savePath">儲存路徑</param>
        protected void RecvFiles(ICmdReceiver socket_map, string savePath) {
            int fileNameLen = 0;
            int recieve_data_size = 0;
            int receivedBytesLen = 0;
            double cal_size = 0;
            byte[] clientData;

            //BinaryWriter bWrite = null;
            string fileName = "";
            receivedBytesLen = socket_map.Receive(out clientData);
            fileNameLen = BitConverter.ToInt32(clientData, 0);
            fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);

            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
            if (File.Exists(savePath + "/" + fileName)) File.Delete(savePath + "/" + fileName);

            using (BinaryWriter bWrite = new BinaryWriter(File.Open(savePath + "/" + fileName, FileMode.OpenOrCreate))) {
                bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);
                do {
                    receivedBytesLen = socket_map.Receive(out clientData);
                    fileName = Encoding.ASCII.GetString(clientData, 0, receivedBytesLen);
                    bWrite.Write(clientData, 0, receivedBytesLen);
                    recieve_data_size += receivedBytesLen;
                    cal_size = recieve_data_size;
                    cal_size /= 1024;
                    cal_size = Math.Round(cal_size, 2);
                    clientData = null;
                    SpinWait.SpinUntil(() => false, 10);
                } while (socket_map.BufferLength != 0);
            }
        }

        #endregion Function - Private Methods
    }

    /// <summary>
    /// 以Socket進行命令傳送的Server端
    /// </summary>
    public class CtSoxCmdServer : BaseServerCmooand, ISoxCmdServer {

        #region Declaration - Fields

        protected ClientCommunication comm_SendPath;
        protected ClientCommunication comm_SendAGVInfo;
        protected ClientCommunication comm_SendMap;
        protected ServerCommunication comm_ReceiveMap;
        protected ServerCommunication comm_Cmd;

        private int port_receiveCmd = (int)SoxPort.Cmd;
        private int port_sendState = (int)SoxPort.Info;
        private int port_sendFile = (int)SoxPort.GetFile;
        private int port_receiveFile = (int)SoxPort.SendFile;
        private int port_sendPath = (int)SoxPort.Path;

        #endregion Declaration - Fields

        #region Implement - ISoxCom

        /// <summary>
        /// 資料傳輸事件
        /// </summary>
        public event CommandEvents.Comm.DelConsole ConsoleRefresh;

        /// <summary>
        /// 命令發送埠
        /// </summary>
        public int CmdSendPort {
            get { return port_receiveCmd; }
            set { if (!IsListening && port_receiveCmd != value) port_receiveCmd = value; }
        }

        /// <summary>
        /// 車子資訊接收埠
        /// </summary>
        public int InfoRecivePort {
            get { return port_sendState; }
            set { if (!IsListening && port_sendState != value) port_sendState = value; }
        }

        /// <summary>
        /// 檔案接收埠
        /// </summary>
        public int FileRecivePort {
            get { return port_sendFile; }
            set { if (!IsListening && port_sendFile != value) port_sendFile = value; }
        }

        /// <summary>
        /// 路徑規劃資料接收埠
        /// </summary>
        public int PathRecivePort {
            get { return port_sendPath; }
            set { if (!IsListening && port_sendPath != value) port_sendPath = value; }
        }

        /// <summary>
        /// 地圖發送埠
        /// </summary>
        public int SendMapPort {
            get { return port_receiveFile; }
            set { if (!IsListening && port_receiveFile != value) port_receiveFile = value; }
        }

        #endregion Implement - ISoxCom

        #region Implement - ISoxCmdServer

        /// <summary>
        /// 是否監聽中
        /// </summary>
        public bool IsListening { get; protected set; } = false;

        /// <summary>
        /// Create thread for server
        /// </summary>
        public void ServoOn() {
            //Start waiting client connection for map sending
            comm_ReceiveMap = new ServerCommunication(port_receiveFile);
            comm_ReceiveMap.StartListen();

            //Start waiting client connection for command sending
            comm_Cmd = new ServerCommunication(port_receiveCmd);
            comm_Cmd.StartListen();
            IsListening = true;

            CtThread.CreateThread(ref t_Server, "mTdServer: ", Server);
        }

        #endregion Implement - ISoxCmdServer

        #region Function - Task

        /// <summary>
        /// Server operation
        /// </summary>
        protected void Server() {
            while (true) {
                try {
                    Thread t_RecvData = new Thread(ReceiveClientCommand);
                    t_RecvData.Start(comm_Cmd.ClientAccept());
                } catch (Exception ex) {
                    Console.Write(ex.Message);
                }
            }
        }

        /// <summary>
        /// [Sox] 命令接收執行緒
        /// </summary>
        /// <param name="sRecvCmd"></param>
        protected void ReceiveClientCommand(Object sRecvCmd) {
            TCPSocket socket_server = new TCPSocket((Socket)sRecvCmd);
            int recvLength;
            byte[] recvBytes = null;
            try {
                while (socket_server.Connected) {
                    while (socket_server.BufferLength == 0) { };
                    recvLength = socket_server.Receive(out recvBytes);
                    if (recvLength > 0) {
                        //Decode byte array to string                
                        string strRecvCmd = Encoding.Default.GetString(recvBytes);
                        recvBytes = null;

                        ConsoleRefresh(strRecvCmd);
                        //txtConsoleInfo.InvokeIfNecessary(() => {
                        //    string strTemp = "[Server] : " + strRecvCmd + txtConsoleInfo.Text;
                        //    if (strTemp.Length > 2048) strTemp = strTemp.Substring(0, 2047);
                        //    txtConsoleInfo.Text = strTemp;
                        //});
                        string[] strArray = strRecvCmd.Trim().Split(':');
                        AnlsCmd(strArray, socket_server);
                    }

                }
            } catch (SocketException ex) {
                Console.Write(ex.Message);
            } finally {
                socket_server.Close();
            }
        }

        /// <summary>
        /// [Sox]Send path data
        /// </summary>
        protected override void SendPath() {
            try {
                comm_SendPath.Connect();
                comm_SendPath.ReadString(Encoding.UTF8);
                base.SendPath();
            } catch (SocketException se) {
                Console.WriteLine("[SendPath] : " + se.ErrorCode + " Interrupt");
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            } finally {
                comm_SendPath.Close();
                comm_SendPath = null;
            }
        }

        /// <summary>
        /// [Sox]以Socket傳送路徑規劃資料
        /// </summary>
        /// <param name="path"></param>
        protected override void SendPath(string path) {
            comm_SendPath.SendString(path, Encoding.UTF8);
        }

        #endregion Function - Task

        #region Function - Private Methods

        /// <summary>
        /// [Sox]開始傳送路徑規劃資料
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="socket_server"></param>
        protected override void StartSendPath(CartesianPos goal, ICmdSender socket_server) {
            comm_SendPath = new ClientCommunication((socket_server as TCPSocket).StrIPAddress, port_sendPath);
            base.StartSendPath(goal, socket_server);
        }

        /// <summary>
        /// [Sox]開始傳送車子資訊
        /// </summary>
        /// <param name="getLaser"></param>
        /// <param name="socket_server"></param>
        protected override void StartSendCarStatus(bool getLaser, ICmdSender socket_server) {
            comm_SendAGVInfo = new ClientCommunication((socket_server as TCPSocket).StrIPAddress, port_sendState);
            base.StartSendCarStatus(getLaser, socket_server);
        }

        /// <summary>
        /// [Sox]Send laser data and car position to client
        /// </summary>
        protected override void SendCarStatus(object getLaser) {
            try {
                base.SendCarStatus(getLaser);
            } catch (SocketException se) {
                Console.WriteLine("[sendCarStatus] : " + se.ErrorCode + " Interrupt");
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            } finally {
                comm_SendAGVInfo.Close();
                comm_SendAGVInfo = null;
            }
        }

        /// <summary>
        /// [Sox] 建立車子資訊傳輸通訊連接
        /// </summary>
        protected override void SendStatusCnn() {
            if (!comm_SendAGVInfo.isConnected)
                comm_SendAGVInfo.Connect();
        }

        /// <summary>
        /// [Base]傳送車子資訊
        /// </summary>
        /// <param name="data"></param>
        protected override void SendStatus(string data) {
            comm_SendAGVInfo.SendString(data, Encoding.UTF8);
        }

        /// <summary>
        /// [Sox]讀取車子資訊傳送回應
        /// </summary>
        /// <returns></returns>
        protected override string RecvSendStatusAck() {
            return comm_SendAGVInfo.ReadString(Encoding.UTF8);
        }

        /// <summary>
        /// [Sox]傳送檔案
        /// </summary>
        /// <param name="socket_server"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        protected override void SendFile(ICmdSender socket_server, string filePath, string fileName) {
            try {
                IPAddress[] ipAddress = Dns.GetHostAddresses((socket_server as TCPSocket).StrIPAddress);
                comm_SendMap = new ClientCommunication(ipAddress[0].ToString(), port_sendFile);

                fileName = fileName.Replace("\\", "/");
                while (fileName.IndexOf("/") > -1) {
                    filePath += fileName.Substring(0, fileName.IndexOf("/") + 1);
                    fileName = fileName.Substring(fileName.IndexOf("/") + 1);
                }
                byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
                if (fileNameByte.Length > 1024 * 1024 * 5) {
                    return;
                }

                byte[] fileData = File.ReadAllBytes(filePath + fileName);
                byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
                byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);
                fileNameLen.CopyTo(clientData, 0);
                fileNameByte.CopyTo(clientData, 4);
                fileData.CopyTo(clientData, 4 + fileNameByte.Length);

                comm_SendMap.Connect();
                comm_SendMap.SendData(clientData);


                fileNameByte = null;
                clientData = null;
                fileNameLen = null;

            } catch {

            } finally {
                comm_SendMap.Close();
                comm_SendMap = null;
            }
        }

        /// <summary>
        /// [Sox]Receive file from client
        /// </summary>
        /// <param name="directory"></param>
        protected override void StartRecvFiles(object directory) {
            TCPSocket socket_map = new TCPSocket(comm_ReceiveMap.ClientAccept());
            try {
                RecvFiles(socket_map, (string)directory);
            } catch (SocketException se) {
                Console.WriteLine("SocketException : {0}", se.ToString());
            } catch (Exception ex) {
                Console.WriteLine("[RecvFiles]" + ex.ToString());
            } finally {
                socket_map.Close();
                socket_map = null;
            }
        }

        /// <summary>
        /// [Sox]取得地圖接收器
        /// </summary>
        /// <returns></returns>
        protected override ICmdReceiver GetMapReceiver() {
            return new TCPSocket(comm_ReceiveMap.ClientAccept()); ;
        }

        #endregion Function - Private Methods
    }

    #endregion Declaration - Implement Class

    #region Support - Class

    /// <summary>
    /// Socket監測參數包
    /// </summary>
    public class SocketMonitor {

        #region Declaration - Fileds

        /// <summary>
        /// 執行緒物件
        /// </summary>
        public Thread Thread = null;

        #endregion Declaration - Fields

        #region Declaration - Porperties

        /// <summary>
        /// 要監測的Socket物件
        /// </summary>
        public Socket Socket { get; private set; }

        /// <summary>
        /// 取消旗標
        /// </summary>
        public bool IsCancel { get; private set; }

        /// <summary>
        /// 執行緒方法
        /// </summary>
        public Action<object> Task { get; private set; }

        /// <summary>
        /// 通訊埠號
        /// </summary>
        public int Port { get; private set; }

        #endregion Declaration - Properties

        #region Function - Constructors

        /// <summary>
        /// 不開放空白建置
        /// </summary>
        private SocketMonitor() { }

        /// <summary>
        /// 共用建構方法
        /// </summary>
        /// <param name="socket">要監測的Socket物件</param>
        /// <param name="port">通訊埠號</param>
        /// <param name="task">執行緒方法</param>
        /// <param name="cancel">取消旗標預設狀態</param>
        private SocketMonitor(Socket socket, int port, Action<object> task, bool cancel) {
            this.Socket = socket;
            this.Port = port;
            this.Task = task;
            this.IsCancel = cancel;


        }

        /// <summary>
        /// 一般建構方法
        /// </summary>
        /// <param name="socket">要監測的Socket物件</param>
        /// <param name="thread">執行緒物件</param>
        /// <param name="task">執行緒方法</param>
        public SocketMonitor(int port, Action<object> task) : this(
            new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp),
            port,
            task,
            false
            ) {
        }

        #endregion Function - Construcotrs

        #region Function - Public Methods

        /// <summary>
        /// 開始監聽
        /// </summary>
        /// <returns></returns>
        public SocketMonitor Listen() {
            if (Socket != null) {
                Stop();
            }
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, Port);
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);
            Socket.Bind(endPoint);
            Socket.Listen(10);
            return this;
        }

        /// <summary>
        /// 開始接收資料
        /// </summary>
        /// <returns>開始接收的<see cref="SocketMonitor"/>實例回傳</returns>
        public void Start() {
            if (Thread?.IsAlive ?? false) {
                this.IsCancel = true;
                CtThread.KillThread(ref Thread);
            }
            CtThread.CreateThread(ref Thread, "mTdClient: ", Task);
            Thread?.Start(this);
        }

        /// <summary>
        /// 停止接收
        /// </summary>
        public void Stop() {
            try {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
                Socket = null;
            } catch {

            }
        }

        public Socket Accept(int receiveTimeOut = 5000, int sendTimeout = 5000, int sendBuffer = 8192, int receiveBuffer = 1024) {
            Socket.Accept();
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, receiveTimeOut);
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, sendTimeout);
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, sendBuffer);
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, receiveBuffer);
            return Socket;
        }

        #endregion Function - Public Methods

    }

    /// <summary>
    /// 實作Client功能之Socket類
    /// </summary>
    public class ClientCommunication {

        #region - Member -

        private Socket socket;
        private IPEndPoint ipe;
        private IPAddress innerIp;
        public string ip;
        public int port;

        #endregion

        #region - Property -


        public bool isConnected {
            get {
                return socket.Connected;
            }
        }


        #endregion

        #region - Constructor -

        public ClientCommunication(string ip, int port) {
            this.ip = ip;
            this.port = port;

            if (IPAddress.TryParse(ip, out innerIp))
                ipe = new IPEndPoint(innerIp, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        #endregion

        #region - Method -

        /// <summary>
        /// Build connection
        /// </summary>
        /// <returns></returns>
        public bool Connect() {

            if (ipe == null && IPAddress.TryParse(ip, out innerIp))
                ipe = new IPEndPoint(innerIp, port);

            if (socket == null)
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            if (!socket.Connected)
                socket.Connect(ipe);
            return socket.Connected;
        }

        /// <summary>
        /// Disconnect
        /// </summary>
        /// <param name="reuse">True:Allow reuse False:Release all resource</param>
        public void Disconnect(bool reuse) {
            if (socket != null) {
                if (socket.Connected) {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Disconnect(true);
                }
            }
        }

        public void Close() {
            if (socket != null) {
                if (socket.Connected) {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Disconnect(false);
                }

                socket.Close();
            }
        }

        public void SendData(byte[] data) {
            if (data.Length > 0)
                socket.Send(data);
        }

        /// <summary>
        /// Send string data of specific encoding type
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encodeType"></param>
        public void SendString(string data, Encoding encodeType) {
            socket.Send(encodeType.GetBytes(data));
        }

        public string ReadString(Encoding encodeType) {
            while (socket.Available == 0) { }
            byte[] data = new byte[socket.Available];
            socket.Receive(data);
            return encodeType.GetString(data);
        }

        #endregion

    }

    /// <summary>
    /// 實作Server功能之Socket類
    /// </summary>
    public class ServerCommunication {

        #region - Member -

        private Socket socket;
        private IPEndPoint ipe;
        private List<Socket> client = new List<Socket>();
        public int port;

        #endregion

        #region - Constructor -

        public ServerCommunication(int port) {
            this.port = port;
            ipe = new IPEndPoint(IPAddress.Any, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        #endregion

        #region - Property -

        public int BufferLength {
            get {
                return socket.Available;
            }
        }



        #endregion

        #region - Method -

        /// <summary>
        /// Bind the port and start listen client connection request
        /// </summary>
        public void StartListen() {
            socket.Bind(ipe);
            socket.Listen(1);
        }

        /// <summary>
        /// Accept the client connection request
        /// </summary>
        /// <param name="receiveTimeOut"></param>
        /// <param name="sendTimeout"></param>
        /// <param name="sendBuffer"></param>
        /// <param name="receiveBuffer"></param>
        /// <returns></returns>
        //public Socket ClientAccept(int receiveTimeOut = 5000,int sendTimeout = 5000,int sendBuffer = 8192,int receiveBuffer = 1024)
        //{
        //    Socket client = socket.Accept();
        //    client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, receiveTimeOut);
        //    client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, sendTimeout);
        //    client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, sendBuffer); 
        //    client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, receiveBuffer);
        //    return client;
        //}


        /// <summary>
        /// Accept the client connection request
        /// </summary>
        public Socket ClientAccept() {
            return socket.Accept();
        }


        /// <summary>
        /// Disconnect
        /// </summary>
        /// <param name="reuse">True:Allow reuse False:Release all resource</param>
        public void Disconnect(bool reuse) {
            if (socket != null) {
                if (socket.Connected) {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Disconnect(true);
                }
            }
        }

        public void Close() {
            if (socket != null) {
                if (socket.Connected) {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Disconnect(false);
                }

                socket.Close();
            }
        }

        /// <summary>
        /// Send string data of specific encoding type
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encodeType"></param>
        public void SendString(string data, Encoding encodeType) {
            socket.Send(encodeType.GetBytes(data));
        }

        public byte[] ReceiveData() {
            while (socket.Available == 0) { };
            byte[] data = new byte[socket.Available];
            socket.Receive(data);
            return data;
        }


        #endregion

    }

    /// <summary>
    /// 以TCP協定建立連線的Socket類
    /// </summary>
    public class TCPSocket : ICmdSender, ICmdReceiver {

        #region - Member -

        private Socket socket;

        #endregion

        #region - Constructor -

        public TCPSocket(Socket socket) {
            this.socket = socket;
        }

        #endregion

        #region - Property -

        public int BufferLength {
            get {
                return socket.Available;
            }
        }

        public string StrIPAddress {
            get {
                string[] strRemoteEndPoint = socket.RemoteEndPoint.ToString().Split(':');
                return strRemoteEndPoint[0];
            }
        }

        public IPAddress[] IPAddress {
            get {
                string[] strRemoteEndPoint = socket.RemoteEndPoint.ToString().Split(':');
                string strRemoteIP = strRemoteEndPoint[0];
                return Dns.GetHostAddresses(strRemoteIP);
            }
        }

        public bool Connected {
            get {
                return socket.Connected;
            }
        }

        #endregion

        #region - Method -

        public int Receive(out byte[] data) {
            while (socket.Available == 0) { };
            data = new byte[socket.Available];
            socket.Receive(data);
            return data.Length;
        }

        public void Close() {
            if (socket != null) {
                if (socket.Connected) {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Disconnect(false);
                    socket.Close();
                }
            }
        }

        public void Send(string data) {
            Send(data, Encoding.UTF8);
        }

        public void Send(string data, Encoding encodeType) {
            socket.Send(encodeType.GetBytes(data + Environment.NewLine));

        }

        public void Send(byte[] data) {
            socket.Send(data);

        }

        #endregion

    }

    /// <summary>
    /// 命令接收參數包
    /// </summary>
    public class CmdEventArgs : EventArgs {

        /// <summary>
        /// 命令資料
        /// </summary>
        public string[] Data { get; }

        /// <summary>
        /// 連接發送命令Client端的Socket物件
        /// </summary>
        public TCPSocket SoxClient { get; }

        public CmdEventArgs(string[] data, TCPSocket soxClient) {
            this.Data = data;
            this.SoxClient = soxClient;
        }
    }

    /// <summary>
    /// 車子狀態資訊
    /// </summary>
    public class CarInfo : CartesianPosInfo {
        /// <summary>
        /// 雷射掃描資料
        /// </summary>
        public IEnumerable<int> LaserData;

        public CarInfo(double x, double y, double theta, string name, uint id) : base(x, y, theta, name, id) {
        }

        /// <summary>
        /// 電池電量百分比
        /// </summary>
        public int PowerPercent { get; set; }

        /// <summary>
        /// 當前訊息
        /// </summary>
        public string Status { get; set; }


        /// <summary>
        /// 嘗試將字串轉換為<see cref="CarInfo"/>
        /// </summary>
        /// <param name="src">來源字串</param>
        /// <param name="info">轉換後的<see cref="CarInfo"/></param>
        /// <returns>是否轉換成功</returns>
        public static bool TryParse(string src, out CarInfo info) {
            IEnumerable<int> laserData = null;
            return TryParse(src, out info, out laserData);
        }

        /// <summary>
        /// 嘗試將字串轉換為<see cref="CarInfo"/>
        /// </summary>
        /// <param name="src">來源字串，格式為"Get:Car:Name:X:Y:Toward:Power:LaserData1,2,3..,n:Status"</param>
        public static bool TryParse(string src, out CarInfo info, out IEnumerable<int> laserData) {
            info = new CarInfo(0, 0, 0, "", 0);
            string[] strArray = src.Split(':');
            laserData = null;
            if (strArray.Length != 9) return false;
            if (strArray[0] != "Get") return false;
            if (strArray[1] != "Car") return false;

            info.name = strArray[2];
            int x; int.TryParse(strArray[3], out x);
            int y; int.TryParse(strArray[4], out y);
            double toward; double.TryParse(strArray[5], out toward);
            int powerPercent; int.TryParse(strArray[6], out powerPercent);
            info.x = x;
            info.y = y;
            info.theta = toward;
            info.PowerPercent = powerPercent;
            string[] sreRemoteLaser = strArray[7].Split(',');
            info.LaserData = sreRemoteLaser.Select(item => int.Parse(item));
            info.Status = strArray[8];
            laserData = info.LaserData;
            return true;
        }
    }

    #endregion Support - Class

}
