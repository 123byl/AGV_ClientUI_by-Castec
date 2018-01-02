using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using static MapProcessing.MapSimplication;
using MapProcessing;
using System.Runtime.InteropServices;
using CtLib.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            
            /// <summary>
            /// 檔案上傳完畢事件
            /// </summary>
            /// <param name="fileName"></param>
            public delegate void DelUploadFinished(string fileName);

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
            public delegate bool DelIdxToGoal(int idxGoal,out CartesianPos goal);
            public delegate bool DelNameToGoal(string goalName,out CartesianPos goal);
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
    internal enum SoxPort {
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
    internal enum MotionDirection {
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

    #region Declaration - Constant

    /// <summary>
    /// 分隔符號常數
    /// </summary>
    public static class Separator {
        /// <summary>
        /// 參數分隔符
        /// </summary>
        public const string Param = ":";
        /// <summary>
        /// 資料分隔符
        /// </summary>
        public const char Data = ',';
    }

    /// <summary>
    /// 主參數常數
    /// </summary>
    internal static class MCmd {
        public const string Get = "Get";
        public const string Set = "Set";
        public const string Send = "Send";
    }

    /// <summary>
    /// 次參數常數
    /// </summary>
    internal static class SCmd {
        public const string OriList = "oriList";
        public const string MapList = "mapList";
        public const string GoalList = "GoalList";
        public const string Laser = "Laser";
        public const string Hello = "Hello";
        public const string Run = "Run";
        public const string Mode = "mode";
        public const string OriName = "OriName";
        public const string Ori = "Ori";
        public const string Map = "map";
        public const string MapName = "MapName";
        public const string DriveVelo = "DriveVelo";
        public const string IsOpen = "IsOpen";
        public const string Stop = "Stop";
        public const string Start = "Start";
        public const string ServoOn = "ServoOn";
        public const string ServoOff = "ServoOff";
        public const string WorkVelo = "WorkVelo";
        public const string Car = "Car";
        public const string PathPlan = "PathPlan";
        public const string POS = "POS";
        public const string Acce = "Acce";
        public const string Dece = "Dece";
        public const string StopMode = "StopMode";
        public const string ID = "ID";
        public const string Goto = "Goto";
        public const string ThreadReset = "ThreadReset";
        public const string Info = "Info";
        public const string Velo = "Velo";
        public const string Encoder = "Encoder";

    }

    /// <summary>
    /// 回應常數
    /// </summary>
    internal static class Ack {
        public const string Done = "Done";
        public const string True = "True";
        public const string False = "False";
        public static readonly Dictionary<bool, string> Mapping = new Dictionary<bool, string>() {
            { true,True },
            { false,False}
        };

        /// <summary>
        /// 比對是否為期望回應
        /// </summary>
        /// <param name="rtnMsg">回應封包</param>
        /// <param name="idx">檢查索引</param>
        /// <param name="ack">期望回應字串</param>
        /// <returns></returns>
        public static bool Check(string[] rtnMsg,int idx,string ack = Ack.True) {
            return rtnMsg.ElementAtOrDefault(idx)?.Equals(ack) ?? false;
        }

    }

    #endregion Declaration - Constant

    #region Declaration - Interface

    /// <summary>
    /// Client端通用命令
    /// </summary>
    public interface IClientCommand:IDisposable {

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
        /// 檔案上傳完畢事件
        /// </summary>
        event CommandEvents.Client.DelUploadFinished UploadFinished;

        /// <summary>
        /// 預設存放路徑
        /// </summary>
        Dictionary<FileType, string> DefDirectory { get; }
       
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
        void EnableFileRecive(FileType type,string filePath, string fileName);

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
        /// <returns>命令是否成功</returns>
        bool StopMove();

        /// <summary>
        /// 開始移動
        /// </summary>
        /// <returns>命令是否成功</returns>
        bool StartMove();

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
        void LeftTurn(int velocity);

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
        bool SetMotor(bool servoOn);

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

        /// <summary>
        /// 設定AGV當前座標
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="theta"></param>
        void SetPosition(int x,int y,double theta);

    }

    /// <summary>
    /// Server端通用命令
    /// </summary>
    public interface IServerCommand :IDisposable{

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
        CommandEvents.Server.DelNameToGoal NameToGoal { get; set; }

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
        void Listen();

        /// <summary>
        /// 停止監聽
        /// </summary>
        void StopListen();
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
        List<CartesianPos> WorkPath { get; set; }

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
    internal abstract class BaseClientCommand : IClientCommand {

        #region Declaration - Fields

        /// <summary>
        /// 檔案接收路徑
        /// </summary>
        protected string mRecvPath = string.Empty;

        #endregion Declaration - Fields

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
        /// 檔案上傳完畢事件
        /// </summary>
        public event CommandEvents.Client.DelUploadFinished UploadFinished;

        public Dictionary<FileType, string> DefDirectory { get; } = new Dictionary<FileType, string>() {
            { FileType.Ori,@"D:\MapInfo\Ori"},
            { FileType.Map,@"D:\MapInfo\Map"}
        };
        
        /// <summary>
        /// Server端是否正在運作
        /// </summary>
        public bool IsServerAlive { get; protected set; }

        /// <summary>
        /// 是否正在取得雷射資料
        /// </summary>
        public bool IsGettingLaser { get; protected set; }
        
        #region Get

        /// <summary>
        /// 取得指定類型檔案清單
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual string GetFileNames(FileType type) {
            string fileList = string.Empty;
            switch (type) {
                case FileType.Ori: fileList = SCmd.OriList;break;
                case FileType.Map: fileList = SCmd.MapList;break;
                default: throw new Exception($"未定義{type}類型檔案");
            }
            string[] rtnMsg = SendMsg(MCmd.Get,fileList);
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
            string[] rtnMsg = SendMsg(MCmd.Get,SCmd.GoalList);
            string goals = string.Empty;
            int len = rtnMsg?.Count() ?? 0;
            if (len > 2) {
                if (Ack.Check(rtnMsg, 2)) {
                    if (len == 4) {
                        goals = rtnMsg[3];
                    }else {
                        throw new Exception("Goal點資料缺失");
                    }
                }
            }else {
                throw new Exception($"{string.Join(":",rtnMsg)}無法解析");
            }
            return goals;
        }

        /// <summary>
        /// 單獨取一次雷射資料
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetLaser() {
            /*-- 若是雷射資料則更新資料 --*/
            string[] rtnMsg = SendMsg(MCmd.Get,SCmd.Laser);
            if (rtnMsg.Length > 3) {
                if (Ack.Check(rtnMsg,1,SCmd.Laser)) {
                    string[] sreRemoteLaser = rtnMsg[3].Split(new char[] { Separator.Data },StringSplitOptions.RemoveEmptyEntries);
                    IEnumerable<int> laser = null;
                    try {
                        laser = sreRemoteLaser.Select(x => int.Parse(x));
                    } catch {}
                    return laser;
                }
            }
            throw new Exception("雷射封包格式不正確");
        }

        /// <summary>
        /// 發送測試封包，測試Server端是否運作中
        /// </summary>
        /// <returns></returns>
        public bool Ping() {
            string[] rtnMsg = SendMsg(MCmd.Get + Separator.Param + SCmd.Hello, false);
            IsServerAlive = rtnMsg.Count() > 2 && Ack.Check(rtnMsg,2);
            return IsServerAlive;
        }

        #endregion Get

        #region Set

        /// <summary>
        /// 移動到指定Goal點
        /// </summary>
        /// <param name="idxGoal">目標Goal點索引值</param>
        public void Run(int idxGoal) {
            /*-- 若是路徑資料則開始接收資料 --*/
            string[] rtnMsg = SendMsg(MCmd.Send,SCmd.Run,idxGoal.ToString());
            if ((rtnMsg?.Length ?? 0) > 3 &&
                Ack.Check(rtnMsg,1,SCmd.Run) &&
                Ack.Check(rtnMsg,3,Ack.Done)) {
                StartReciePath();
            }
        }

        /// <summary>
        /// 設定車子模式
        /// </summary>
        /// <param name="mode"></param>
        public void SetCarMode(WorkMode mode) {
            SendMsg(MCmd.Set,SCmd.Mode,mode.ToString());
        }

        /// <summary>
        /// 設定掃描的地圖名稱
        /// </summary>
        /// <param name="oriName">掃描的ori檔名</param>
        public void SetScanName(string oriName) {            
            SendMsg(MCmd.Set,SCmd.OriName,oriName);
        }

        /// <summary>
        /// 要求Server端載入指定地圖檔
        /// </summary>
        /// <param name="mapName"></param>
        public void OrderMap(string mapName) {
            SendMsg(MCmd.Set,SCmd.MapName,mapName);
        }

        public void SetPosition(int x, int y, double theta) {
            throw new NotImplementedException();
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
            string[] rtnMsg = SendMsg(MCmd.Get,SCmd.IsOpen);
            //bool rst = false;
            //if (rst = (rtnMsg?.Count() ?? 0) == 4) {
            //    bool suc = bool.TryParse(rtnMsg[2], out rst) ? rst : false;
            //    bool servoOn = bool.TryParse(rtnMsg[3], out rst) ? rst : false;
            //    rst = suc && servoOn; 
            //}
            //是否成功回傳 && 馬達激磁狀態
            return Ack.Check(rtnMsg,2) && Ack.Check(rtnMsg,3);
        }

        /// <summary>
        /// 停止移動
        /// </summary>
        /// <returns>命令是否成功</returns>
        public bool StopMove() {
            string[] rtnMsg = SendMsg(MCmd.Set,SCmd.Stop);
            //bool rst = (rtnMsg?.Count() ?? 0) == 3;
            //if (rst) {
            //    rst = bool.TryParse(rtnMsg[2], out rst) ? rst : false;
            //}
            return Ack.Check(rtnMsg,2);
        }

        /// <summary>
        /// 開始移動
        /// </summary>
        /// <returns>命令是否成功</returns>
        public bool StartMove() {
            string[] rtnMsg = SendMsg(MCmd.Set,SCmd.Start);
            //bool rst = (rtnMsg?.Count() ?? 0) == 3;
            //if (rst) {
            //    rst = bool.TryParse(rtnMsg[2], out rst) ? rst : false;
            //}
            return Ack.Check(rtnMsg,2);
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
        public void LeftTurn(int velocity) {
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
        public bool SetMotor(bool servoOn) {
            string cmd = servoOn ? SCmd.ServoOn : SCmd.ServoOff;
            string[] rtnMsg = SendMsg(MCmd.Set,cmd);
            return Ack.Check(rtnMsg, 2, Ack.True);
        }

        /// <summary>
        /// 設定車子速度
        /// </summary>
        /// <param name="velocity"></param>
        public void SetVelocity(int velocity) {
            string[] rtnMsg =  SendMsg(MCmd.Set,SCmd.WorkVelo,velocity.ToString(),velocity.ToString());
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
                
                string[] rtnMsg = SendMsg(MCmd.Get,SCmd.Car,Ack.True,Ack.True);
                IsGettingLaser = Ack.Check(rtnMsg,2);
                if (!IsGettingLaser) {
                    EnableCarInfoReturn(false);
                }
            } else {
                /*-- 關閉Socket與執行緒 --*/
                SendMsg(MCmd.Get,SCmd.Car,Ack.False);
                IsGettingLaser = false;
            }
            return IsGettingLaser;
        }

        /// <summary>
        /// 激活遠端檔案接收
        /// </summary>
        public virtual async void EnableFileRecive(FileType type,string filePath, string fileName) {
            string fileType = null;
            switch (type) {
                case FileType.Ori: fileType = SCmd.Ori; break;
                case FileType.Map: fileType = SCmd.Map; break;
                default: throw new Exception($"未定義{type}傳送命令");
            }
            string[] rtnMsg =  SendMsg(MCmd.Send,fileType);
            if (Ack.Check(rtnMsg, 2)) {
                /*-- 統一檔名格式為"檔名.副檔名" --*/
                string name = Path.GetFileName(fileName);
                await Task.Run(() => {
                    /*-- 以衍生類實作之通訊方式進行檔案傳輸 --*/
                    SendFile(filePath,name);
                    /*-- 傳送完畢後發報事件 --*/
                    UploadFinished?.Invoke(name);
                });
            }
        }
        
        /// <summary>
        /// 要求傳送指定檔案
        /// </summary>
        /// <param name="type">要求的檔案類型</param>
        /// <param name="fileName">要求的檔案名稱</param>
        public virtual void RequireFile(FileType type, string fileName) {
            string fileType = string.Empty;
            switch (type) {
                case FileType.Ori: fileType = SCmd.Ori; break;
                case FileType.Map: fileType = SCmd.Map; break;
                default: throw new Exception($"未定義{type}傳送命令");
            }
            /*-- 向Server端發出檔案請求 --*/
            SendMsg(MCmd.Get,fileType,fileName);
        }

        /// <summary>
        /// 要求到指定Goal點的路徑
        /// </summary>
        /// <param name="idxGoal">目標Goal點索引值</param>
        public void RequirePath(int idxGoal) {
            /*-- 若是路徑資料則開始接收資料 --*/
            string[] rtnMsg = SendMsg(MCmd.Set,SCmd.PathPlan,idxGoal.ToString());
            if ((rtnMsg?.Count() ?? 0) > 3 &&
                Ack.Check(rtnMsg,1,SCmd.PathPlan) &&
                Ack.Check(rtnMsg,2)){
                StartReciePath();
            }
        }

        #endregion Enable/Require

        #endregion Implement - IClientCommand

        #region Implement - IDisposable

        private bool disposedValue = false; // 偵測多餘的呼叫

        /// <summary>
        /// [Base] 資源釋放具體行為
        /// </summary>
        protected virtual void dispose() {

        }

        protected void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: 處置 Managed 狀態 (Managed 物件)。
                    dispose();
                }

                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        // ~BaseClientCommand() {
        //   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose() {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
            GC.SuppressFinalize(this);
        }
        #endregion Implement - IDisposable

        #region Function - Private Methods

        /// <summary>
        /// 傳送檔案
        /// </summary>
        /// <param name="fileName"></param>
        protected void SendFile(string filePath,string fileName) {
            /* File reading operation. */
            byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);//將檔名以ASCII進行編碼
            if (fileNameByte.Length > 1024 * 1024 * 5) {//檔名超出長度則不傳
                return;
            }
            byte[] fileData = File.ReadAllBytes(filePath + "\\" +  fileName);//讀取檔案並轉為位元陣列
            /* clientData will store complete bytes which will store file name length, 
            file name & file data. */
            byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);//將檔名長度(int)轉為4位元Byte陣列
            /* Read & store file byte data in byte array. */
            byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];//建立資料所需空間
            /* File name length’s binary data. */
            fileNameLen.CopyTo(clientData, 0);//寫入檔名長度
            fileNameByte.CopyTo(clientData, 4);//寫入檔名
            fileData.CopyTo(clientData, 4 + fileNameByte.Length);//寫入檔案內容
            /* copy these bytes to a variable with format line [file name length]
            [file name] [ file content] */
            SendFile(clientData);            
        }

        /// <summary>
        /// 以衍生類實作的通訊進行檔案傳送
        /// </summary>
        /// <param name="data"></param>
        protected abstract void SendFile(byte[] data);

        /// <summary>
        /// 開始接收路徑規劃資料
        /// </summary>
        protected abstract void StartReciePath();

        /// <summary>
        /// 方向控制
        /// </summary>
        /// <param name="direction">移動方向</param>
        protected virtual void MotorControl(MotionDirection direction, int velocity) {
            if (IsServoOn()) {
                if (direction == MotionDirection.Stop) {
                    StopMove();
                } else {
                    string cmd = string.Empty;
                    int lVeol = velocity;
                    int rVeol = velocity;
                    switch (direction) {
                        case MotionDirection.Forward:
                            break;
                        case MotionDirection.Backward:
                            lVeol = -velocity;
                            rVeol = -velocity;
                            break;
                        case MotionDirection.LeftTrun:
                            rVeol = -velocity;
                            break;
                        case MotionDirection.RightTurn:
                            lVeol = -velocity;
                            break;
                    }
                    SendMsg(MCmd.Set,SCmd.DriveVelo,lVeol.ToString(),rVeol.ToString());
                    StartMove();
                }
            }
        }

        /// <summary>
        /// 訊息傳送
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        protected string[] SendMsg(params string[] param) {
            return SendMsg(string.Join(Separator.Param, param));
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
    internal class CtSoxCmdClient : BaseClientCommand, ISoxCmdClient {

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

        internal CtSoxCmdClient(string ip = "127.0.0.1") {
            mServerIP = ip;
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
                if (!IsConnected) {
                    mSoxCmd?.Close();
                    IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ServerIP), mCmdSendPort);
                    mSoxCmd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    mSoxCmd.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
                    mSoxCmd.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);
                    mSoxCmd.Connect(ipEndPoint);
                }
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
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



        #endregion Implement - IScoketCommand

        #region Implement -IClientCommand

        /// <summary>
        /// 要求傳送指定檔案
        /// </summary>
        /// <param name="type">要求的檔案類型</param>
        /// <param name="fileName">要求的檔案名稱</param>
        public override void RequireFile(FileType type, string fileName) {
            /*-- 開啟執行緒準備接收檔案 --*/
            mRecvPath = DefDirectory[type];
            mSoxMonitorFile.Listen();
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
                IsGettingLaser = enb;
                if (enb) {//開啟Socket與執行緒準備接收
                    mSoxInfoRecive.Listen();
                    mSoxInfoRecive.Start();
                    IsGettingLaser = base.EnableCarInfoReturn(enb);
                } else {//關閉Socket與執行緒
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

        #region Implement - IDisposable

        /// <summary>
        /// [Sox]資源釋放具體行為
        /// </summary>
        protected override void dispose() {
            base.dispose();
        }

        #endregion Implement - IDisposable

        #region Funciton - Private Methods

        /// <summary>
        /// Send file of server to client
        /// </summary>
        /// <param name="fileName">File name</param>
        protected override void SendFile(byte[] data) {
            Socket clientSock = null;
            try {
                clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                IPAddress[] ipAddress = Dns.GetHostAddresses(ServerIP);
                IPEndPoint ipEnd = new IPEndPoint(ipAddress[0], mSendMapPort);
                /* Make IP end point same as Server. */
                clientSock.Connect(ipEnd);
                clientSock.Send(data);
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            } finally {
                if (clientSock != null) {
                    clientSock.Shutdown(SocketShutdown.Both);
                    clientSock.Close();
                    clientSock = null;
                }
            }
        }

        /// <summary>
        /// 開始接收路徑資料
        /// </summary>
        protected override void StartReciePath() {
            mSoxMonitorPath.Listen();
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

                rtnMsg = msg.Trim().Split(':');

                /*-- 顯示Server端回應 --*/
                msg = $"{DateTime.Now} [Server] : {msg}\r\n";
                RaiseConsole(msg);

            } else {
                rtnMsg = sendMseeage.Split(':');
                if (Bypass) {//Bypass =>不論如何回傳Ture模擬正常運作
                    rtnMsg[rtnMsg.Length-1] = Ack.True;
                } else if (passChkConn) {//連線未建立下的命令皆回傳False
                    rtnMsg[rtnMsg.Length-1] = Ack.False;
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
                return Ack.False;
            } catch (ArgumentNullException ane) {
                System.Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                return Ack.False;
            } catch (Exception ex) {
                System.Console.Write(ex.Message);
                return Ack.False;
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
            string[] pathArray = pack.Split(Separator.Data);
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
            string[] pathArray = pack.Trim().Split(new char[] { Separator.Data},StringSplitOptions.RemoveEmptyEntries);
            List<CartesianPos> rtnPoints = null;
            int len = pathArray?.Count() ?? 0;
            if (len != 0 && len % 2 == 0) {
                rtnPoints = new List<CartesianPos>();
                int x, y;
                for (int i = 0; i < pathArray.Count() - 1; i += 2) {
                    if (int.TryParse(pathArray[i], out x) && int.TryParse(pathArray[i+1], out y)) {
                        rtnPoints.Add(new CartesianPos(x, y));
                    } else {
                        rtnPoints.Clear();
                        rtnPoints = null;
                        break;
                    }
                }
            }
            return rtnPoints;
        }

        /// <summary>
        /// 車子資訊接收執行緒
        /// </summary>
        protected void tsk_RecvInfo(object obj) {
            SocketMonitor soxMonitor = obj as SocketMonitor;
            Socket sRecvCmdTemp = soxMonitor.Accept();
            Console.WriteLine("接受遠端連線請求");
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
                    bool suc = false;
                    if (suc = CarInfo.TryParse(strRecvCmd, out carInfo)) {
                        RaiseCarinfoRefresh(carInfo);
                    }
                    string data = string.Join(Separator.Param, new object[] { MCmd.Get, SCmd.Car, true, suc });
                    sRecvCmdTemp.Send(Encoding.UTF8.GetBytes(data));

                    strRecvCmd = null;
                    strArray = null;
                }
            } catch (SocketException se) {
                Console.WriteLine("[Status Recv] : " + se.ToString());
                //MessageBox.Show("目標拒絕連線");
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
                //Console.Write(ex.Message);
                //throw ex;
            } finally {
                Console.WriteLine("車子資訊接收執行緒停止");
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
            string fileName = "";
            string fullName =null;
            try {
                if (!Bypass) {
                    clientSock = soxMonitor.Accept();

                    //Socket clientSock = sRecvFile.Accept();
                    //clientSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
                    //clientSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);//設置發送資料超時
                    //clientSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 1024); //設置發送緩衝區大小 1K
                    /* When request comes from client that accept it and return 
                    new socket object for handle that client. */
                    byte[] clientData = new byte[1024 * 10000];
                    do {
                        receivedBytesLen = clientSock.Receive(clientData);
                        if (first == 1) {
                            fileNameLen = BitConverter.ToInt32(clientData, 0);
                            /* I've sent byte array data from client in that format like 
                            [file name length in byte][file name] [file data], so need to know 
                            first how long the file name is. */
                            fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);
                            /* Read file name */
                            if (!Directory.Exists(mRecvPath)) {
                                Directory.CreateDirectory(mRecvPath);
                            }
                            if (File.Exists(mRecvPath + "/" + fileName)) {
                                File.Delete(mRecvPath + "/" + fileName);
                            }
                            fullName = mRecvPath + "/" + fileName;
                            bWrite = new BinaryWriter(File.Open(mRecvPath + "/" + fileName, FileMode.OpenOrCreate));
                            /* Make a Binary stream writer to saving the receiving data from client. */
                            // ms = new MemoryStream();
                            bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);
                            //ms.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 -
                            //fileNameLen);
                            //寫入資料 ，呈現於BITMAP用
                            /* Read remain data (which is file content) and 
                            save it by using binary writer. */
                            /* Close binary writer and client socket */
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
            } catch (Exception ex) {
                Console.WriteLine("[RecvFiles]" + ex.ToString());
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
    internal abstract class BaseServerCmooand : IServerCommand {

        #region Declaration - Fields

        protected bool bSendStatus = false;

        protected IAGV mAGV = null;
        
        protected Thread t_Work = null;
        protected Thread t_ConsoleUptate = null;
        protected Thread t_Server = null;
        protected Thread t_Recver = null;
        protected Thread t_SendPath = null;
        protected Thread t_Sender = null;

        #endregion Declaration - Fields

        #region Declaration - Properteis
        

        #endregion Declaration - Properties

        #region Function - Constructors

        public BaseServerCmooand() {

        }

        #endregion Function - Constructors

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
        public CommandEvents.Server.DelNameToGoal NameToGoal { get; set; }

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
        
        #region Implement - IDisposable

        private bool disposedValue = false; // 偵測多餘的呼叫

        /// <summary>
        /// [Base] 資源釋放具體行為
        /// </summary>
        protected virtual void dispose() {
            try {
                CtThread.KillThread(ref t_Work);
                CtThread.KillThread(ref t_Server);
                CtThread.KillThread(ref t_SendPath);
                CtThread.KillThread(ref t_Sender);
                CtThread.KillThread(ref t_Recver);
                CtThread.KillThread(ref t_ConsoleUptate);
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        protected void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: 處置 Managed 狀態 (Managed 物件)。
                    dispose();
                }

                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        // ~BaseServerCmooand() {
        //   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose() {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
            GC.SuppressFinalize(this);
        }

        #endregion Implement - IDisposable

        #region Function - Private Methods

        #region CmdAnls

        /// <summary>
        /// 命令分類
        /// </summary>
        /// <param name="strArray"></param>
        /// <param name="socket_server"></param>
        protected virtual void AnlsCmd(string[] strArray, ICmdSender socket_server) {
            switch (strArray[0]) {
                case MCmd.Set:
                    AnlsSet(strArray, socket_server);
                    break;
                case MCmd.Get:
                    AnlsGet(strArray, socket_server);
                    break;
                case MCmd.Send:
                    AnlsSend(strArray, socket_server);
                    break;
                default:
                    socket_server.Send(string.Join(Separator.Param, strArray) + "\r\n");
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
            string suc = null;
            CartesianPos goal = null;
            switch (strArray[1]) {
                case SCmd.Start:
                    status = mAGV?.Drive() ?? false;
                    socket_server.Send(MCmd.Set,SCmd.Start,Ack.Mapping[status]);
                    break;
                case SCmd.Stop:
                    status = mAGV?.StopDrive() ?? false;
                    socket_server.Send(MCmd.Set,SCmd.Stop,Ack.Mapping[status]);
                    break;
                case SCmd.POS:
                    if (mAGV != null) {
                        mAGV.Position = new double[3] { double.Parse(strArray[2]), double.Parse(strArray[3]), double.Parse(strArray[4]) };
                    }
                    socket_server.Send(MCmd.Set,SCmd.POS,Ack.True);
                    break;
                case SCmd.ServoOn:
                    status = mAGV?.DriveOn() ?? false;
                    socket_server.Send(MCmd.Set,SCmd.ServoOn,Ack.Mapping[status]);
                    break;
                case SCmd.ServoOff:
                    status = mAGV?.DriveOff() ?? false;
                    socket_server.Send(MCmd.Set,SCmd.ServoOff,Ack.Mapping[status]);
                    break;
                case SCmd.DriveVelo:
                    if ((mAGV?.MotorVelocity.Count() ?? 0) == 2) {
                        mAGV.MotorVelocity[0] = int.Parse(strArray[2]);
                        mAGV.MotorVelocity[1] = int.Parse(strArray[3]);
                    }
                    socket_server.Send(MCmd.Set,SCmd.DriveVelo,Ack.True);
                    break;
                case SCmd.WorkVelo:
                    if (mAGV != null) {
                        mAGV.DriveSpeed = int.Parse(strArray[2]);
                    }
                    socket_server.Send(MCmd.Set,SCmd.WorkVelo,Ack.True);
                    break;
                case SCmd.Acce:
                    status = mAGV?.SetAcceleration(int.Parse(strArray[2])) ?? false;
                    socket_server.Send(MCmd.Set,SCmd.Acce,Ack.Mapping[status]);
                    break;
                case SCmd.Dece :
                    status = mAGV?.SetDcceleration(int.Parse(strArray[2])) ?? false;
                    socket_server.Send(MCmd.Set,SCmd.Dece,Ack.Mapping[status]);
                    break;
                case SCmd.StopMode:
                    bool stop = bool.Parse(strArray[2]);
                    status = mAGV?.SetBrakeMode(stop) ?? false;
                    socket_server.Send(MCmd.Set,SCmd.StopMode,Ack.Mapping[status],Ack.Mapping[stop]);
                    break;
                case SCmd.ID:
                    //carID = int.Parse(strArray[2]);
                    string id = strArray[2];
                    SetCarID?.Invoke(int.Parse(id));
                    socket_server.Send(MCmd.Set,SCmd.ID,id);
                    break;
                case SCmd.OriName:
                    //oriPath = oriDirectory + strArray[2];
                    string oriName = strArray.ElementAtOrDefault(2);
                    if (status =  !string.IsNullOrEmpty(oriName)) SetOriName?.Invoke(oriName);
                    socket_server.Send(MCmd.Set,SCmd.OriName,status.ToString(),oriName);
                    break;
                case SCmd.MapName:
                    //mapPath = mapDirectory + strArray[2];
                    //if (File.Exists(mapPath)) {
                    string mapName = strArray[2];
                    if (SetMapName?.Invoke(mapName) ?? false) {
                        socket_server.Send(MCmd.Set,SCmd.MapName,Ack.True,mapName);
                        ReadMapFile?.Invoke();
                    } else {
                        socket_server.Send(MCmd.Set,SCmd.MapName,Ack.False,"Map not exists");
                    }
                    break;
                case SCmd.PathPlan:
                    int idx = 0;
                    if (int.TryParse(strArray[2], out idx) &&
                        (IdxToGoal?.Invoke(idx,out goal) ?? false)) {
                        StartSendPath(goal, socket_server);
                        suc = Ack.True;
                    } else {
                        suc = Ack.False;
                    }
                    socket_server.Send(MCmd.Set,SCmd.PathPlan,suc,strArray[2]);
                    break;
                case SCmd.Goto:
                    string goalName = strArray[2];
                    if ((NameToGoal?.Invoke(goalName, out goal) ?? false)) {
                        StartSendPath(goal, socket_server);
                        mAGV?.DoPathMovement();
                        suc = Ack.True;
                    } else {
                        suc = Ack.False;
                    }
                    socket_server.Send(MCmd.Set, SCmd.Goto, suc,goalName);                    
                    break;
                case SCmd.Run:
                    if (int.TryParse(strArray[2], out idx) &&
                        (IdxToGoal?.Invoke(idx, out goal) ?? false)) {
                        StartSendPath(goal, socket_server);
                        mAGV.DoPathMovement();
                        suc = Ack.True;
                    } else {
                        suc = Ack.False;
                    }
                    break;
                case SCmd.Mode:
                    string mode = strArray[2];
                    WorkMode m = WorkMode.Idle;
                    if (status =  Enum.TryParse(mode,out m)) {
                        switch (m) {
                            case WorkMode.Idle:
                                mAGV.mode = WorkMode.Idle;
                                break;
                            case WorkMode.Work:
                                mAGV.mode = WorkMode.Work;
                                break;
                            case WorkMode.Map:
                                if (mAGV.mode != WorkMode.Map) {
                                    mAGV.mode = WorkMode.Map;
                                    if (RecordMap != null) {
                                        CtThread.CreateThread(ref t_Work, "Thread_Console Update", () => { RecordMap(); });
                                    }
                                }
                                break;
                            default:
                                status = false;
                                break;
                        }
                    }
                    socket_server.Send(MCmd.Set,SCmd.Mode,Ack.Mapping[status],mode.ToString());
                    break;

                case SCmd.ThreadReset:
                    CtThread.KillThread(ref t_Recver, 500);
                    CtThread.KillThread(ref t_SendPath, 500);
                    CtThread.KillThread(ref t_Sender, 500);
                    break;
                default:
                    socket_server.Send(string.Join(Separator.Param, strArray),Ack.False);
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
            string suc = null;
            switch (strArray[1]) {
                case SCmd.Hello:
                    socket_server.Send(MCmd.Get,SCmd.Hello,Ack.True);
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
                case SCmd.GoalList: {
                        //List<string> data = goalList.ConvertAll(v => v.name);
                        List<string> data = GetGoalNames?.Invoke();
                        Status = data?.Any() ?? false;
                        string goals = Status ? string.Join(Separator.Data.ToString(), data) : string.Empty;
                        socket_server.Send(MCmd.Get,SCmd.GoalList,Status.ToString(),goals);
                    }
                    break;
                case SCmd.OriList: {
                        //DirectoryInfo d = new DirectoryInfo(oriDirectory);  //Create directory object 
                        //FileInfo[] Files = d.GetFiles("*.ori");              //Getting Text files
                        //string[] tmp = Array.ConvertAll(Files, v => v.Name);
                        string[] data = GetOriNames?.Invoke();
                        Status = data?.Any() ?? false;
                        string oriList = Status ? string.Join(Separator.Data.ToString(), data) : string.Empty;
                        socket_server.Send(MCmd.Get,SCmd.OriList,Status.ToString(),oriList);
                    }
                    break;
                case SCmd.MapList: {
                        //DirectoryInfo d = new DirectoryInfo(mapDirectory);  //Create directory object 
                        //FileInfo[] Files = d.GetFiles("*.map");              //Getting Text files
                        //string[] tmp = Array.ConvertAll(Files, v => v.Name);
                        string[] data = GetMapNames?.Invoke();
                        Status = data?.Any() ?? false;
                        string mapList = Status ? string.Join(Separator.Data.ToString(), data) : string.Empty;
                        socket_server.Send(MCmd.Get,SCmd.MapList,Ack.Mapping[Status],mapList);
                    }
                    break;
                case SCmd.Ori: {
                        string oriName = strArray[2] + ".ori";
                        string oriDir = GetOriDirectory?.Invoke();
                        string filePath = oriDir + "\\" + oriName;
                        if (File.Exists(filePath)) {
                            SendFile(socket_server, filePath, oriName);
                            suc = Ack.True;
                        } else {
                            suc = Ack.False;
                        }
                        socket_server.Send(MCmd.Get, SCmd.Ori, suc);
                    }
                    break;
                case SCmd.Map: {
                        string mapName = strArray[2] + ".map";
                        string mapDir = GetMapDirectory?.Invoke();
                        string filePath = mapDir + "\\" + mapName ;
                        if (File.Exists(filePath)) {
                            SendFile(socket_server, filePath, mapName);
                            suc = Ack.True;
                        }else {
                            suc = Ack.False;
                        }
                        //SendFile(socket_server.StrIPAddress, port_sendFile, mapDirectory, strArray[2]); //發送本機Map檔
                        socket_server.Send(MCmd.Get,SCmd.Map,suc);
                    }
                    break;
                case SCmd.Info:
                    bool[] info = new bool[3];
                    if (Status = mAGV != null) {
                        mAGV.GetMotorInfo(out info);
                    }
                    socket_server.Send(MCmd.Get,SCmd.Info,
                        Ack.Mapping[Status],
                        Ack.Mapping[info[0]],Ack.Mapping[info[1]],Ack.Mapping[info[2]]);
                    break;
                case SCmd.Velo:
                    int lVelo = 0;
                    int rVelo = 0;
                    Status = mAGV?.GetDriveVelocity(out lVelo, out rVelo) ?? false;
                    socket_server.Send(MCmd.Get,SCmd.Velo,
                        Ack.Mapping[Status],
                        lVelo.ToString(),rVelo.ToString());
                    break;
                case SCmd.Acce:
                    int[] accel = new int[2];
                    if (Status = ((mAGV?.MotorAccel?.Count() ?? 0) == 2)) {
                        accel = mAGV.MotorAccel;
                    }
                    socket_server.Send(MCmd.Get,SCmd.Acce,
                        Ack.Mapping[Status],
                        accel[0].ToString(),accel[1].ToString());
                    break;
                case SCmd.Dece:
                    int[] dccel = new int[2];
                    if (Status = ((mAGV?.MotorDccel?.Count() ?? 0) == 2)) {
                        dccel = mAGV.MotorDccel;
                    }
                    socket_server.Send(MCmd.Get,SCmd.Dece,
                        Ack.Mapping[Status],
                        dccel[0].ToString(),dccel[1].ToString());
                    break;
                case SCmd.IsOpen:
                    Status = mAGV != null;
                    bool cnn = Status ? mAGV.DriveConnectState : false;
                    socket_server.Send(MCmd.Get,SCmd.IsOpen,Ack.Mapping[Status],Ack.Mapping[cnn]);
                    break;
                case SCmd.Encoder:
                    long[] encoder = new long[2] ;
                    if (Status = (mAGV?.EncoderValue?.Count() ?? 0) == 2) {
                        encoder = mAGV.EncoderValue;
                        rtnVal = $":{encoder[0]}:{encoder[1]}";
                    }
                    socket_server.Send(MCmd.Get,SCmd.Encoder,
                        Ack.Mapping[Status],
                        encoder[0].ToString(),encoder[1].ToString());
                    break;
                case SCmd.StopMode:
                    bool stop = (Status = mAGV != null) ? mAGV.BrakeMode : false;
                    socket_server.Send(MCmd.Get,SCmd.StopMode,Ack.Mapping[Status],Ack.Mapping[stop]);
                    break;
                case SCmd.Laser:
                    List<int> laserData = mAGV.LaserDistanceData;
                    string laserVar = string.Join(Separator.Data.ToString(), laserData);
                    suc = string.IsNullOrEmpty(laserVar) ? Ack.True : Ack.False;
                    socket_server.Send(MCmd.Get,SCmd.Laser, Ack.True,laserVar + "\r\n");
                    laserData = null;
                    break;
                case SCmd.Car:
                    if (Ack.Check(strArray,2) && t_Sender == null) {
                        bool getLaser = bool.Parse(strArray[3]);
                        StartSendCarStatus(getLaser, socket_server);
                    } else if (Ack.Check(strArray,2,Ack.False) && t_Sender != null) {
                        bSendStatus = false;
                        CtThread.KillThread(ref t_Sender);
                    }
                    socket_server.Send(MCmd.Get,SCmd.Car,Ack.True,bSendStatus + "\r\n");
                    break;

                default:
                    socket_server.Send(string.Join(Separator.Param , strArray),Ack.False+ "\r\n");
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
            string strRecvCmd = string.Join(Separator.Param, strArray);
            switch (strArray[1]) {
                case SCmd.Ori:
                    correct = true;
                    directory = GetOriDirectory?.Invoke();
                    socket_server.Send(strRecvCmd ,Ack.True + "\r\n");
                    break;
                case SCmd.Map:
                    correct = true;
                    directory = GetMapDirectory?.Invoke();
                    socket_server.Send(strRecvCmd ,Ack.True + "\r\n");
                    break;
                default:
                    correct = false;
                    socket_server.Send(strRecvCmd ,"Format Fault\r\n");
                    break;
            }
            if (correct) {
                CtThread.CreateThread(ref t_Recver, "mTdRecvMap: ", StartRecvFiles);
                t_Recver.Start(directory);
            }
        }

        #endregion CmdAnls

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
                    laserVar = string.Join(Separator.Data.ToString(), laserData);
                }
                object[] data = new object[] { MCmd.Get, SCmd.Car, carID, carPos[0], carPos[1], carPos[2], powerPercent, laserVar, mAGV.mode + Environment.NewLine };
                SendStatus(string.Join(Separator.Param,data));
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
            if (File.Exists(savePath + "\\" + fileName)) File.Delete(savePath + "\\" + fileName);
            string fullPath = savePath + "\\" + fileName;
            FileStream fileStream = new FileStream(fullPath, FileMode.OpenOrCreate);
            BinaryWriter bWrite = new BinaryWriter(fileStream);
            try {
                bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);
                while (socket_map.BufferLength != 0) {
                    receivedBytesLen = socket_map.Receive(out clientData);
                    fileName = Encoding.ASCII.GetString(clientData, 0, receivedBytesLen);
                    bWrite.Write(clientData, 0, receivedBytesLen);
                    recieve_data_size += receivedBytesLen;
                    cal_size = recieve_data_size;
                    cal_size /= 1024;
                    cal_size = Math.Round(cal_size, 2);
                    clientData = null;
                    SpinWait.SpinUntil(() => false, 10);
                }
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            } finally {
                bWrite?.Close();
            }
        }

        #endregion Function - Private Methods
    }

    /// <summary>
    /// 以Socket進行命令傳送的Server端
    /// </summary>
    internal class CtSoxCmdServer : BaseServerCmooand, ISoxCmdServer {

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
        public void Listen() {
            if (!IsListening) {
                //Start waiting client connection for map sending
                comm_ReceiveMap = new ServerCommunication(port_receiveFile);
                comm_ReceiveMap.StartListen();

                //Start waiting client connection for command sending
                comm_Cmd = new ServerCommunication(port_receiveCmd);
                comm_Cmd.StartListen();
                IsListening = true;

                CtThread.CreateThread(ref t_Server, "mTdServer: ", Server);
            }
        }

        /// <summary>
        /// 停止監聽
        /// </summary>
        public void StopListen() {
            if (IsListening) {
                /*-- 若是檔案傳輸中，則先停止傳輸執行緒 --*/
                if (t_Sender?.IsAlive ?? false) {
                    CtThread.KillThread(ref t_Sender);
                }
                comm_ReceiveMap?.Close();

                /*-- 若是命令接收中，則先停止接收執行緒 --*/
                if (t_Server?.IsAlive ?? false) {
                    CtThread.KillThread(ref t_Server);
                }
                comm_Cmd?.Close();
            }
        }

        #endregion Implement - ISoxCmdServer

        #region Implement - IDisposable

        /// <summary>
        /// [Sox] 資源釋放具體行為
        /// </summary>
        protected override void dispose() {
            comm_Cmd?.Close();
            comm_ReceiveMap?.Close();
            comm_SendAGVInfo?.Close();
            comm_SendMap?.Close();
            comm_SendPath?.Close();

            comm_Cmd = null;
            comm_ReceiveMap = null;
            comm_SendAGVInfo = null;
            comm_SendMap = null;
            comm_SendPath = null;

            base.dispose();
        }

        #endregion Implement - IDisposable

        #region Function - Task

        /// <summary>
        /// Server operation
        /// </summary>
        protected void Server() {
            bool isAbort = false;            
            while (!isAbort) {
                try {
                    Thread t_RecvData = new Thread(ReceiveClientCommand);
                    t_RecvData.Start(comm_Cmd.ClientAccept());
                } catch (ThreadAbortException exAbort) {
                    Console.WriteLine(exAbort.Message);
                    isAbort = true;
                } catch (ThreadInterruptedException exInterrup) {
                    Console.WriteLine(exInterrup.Message);
                    isAbort = true;
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
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

                        ConsoleRefresh?.Invoke(strRecvCmd);
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
                if (comm_SendPath != null) {
                    comm_SendPath.Close();
                    comm_SendPath = null;
                }
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
            bool suc =  comm_SendAGVInfo.Connect();
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
                comm_SendAGVInfo?.Close();
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

                //fileName = fileName.Replace("\\", "/");
                //while (fileName.IndexOf("/") > -1) {
                //    filePath += fileName.Substring(0, fileName.IndexOf("/") + 1);
                //    fileName = fileName.Substring(fileName.IndexOf("/") + 1);
                //}
                byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
                if (fileNameByte.Length > 1024 * 1024 * 5) {
                    return;
                }

                byte[] fileData = File.ReadAllBytes(filePath);
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
            } catch (Exception ex){
                Console.WriteLine(ex.Message);

            } finally {
                comm_SendMap?.Close();
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

    public static class FileMth {
        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public static IntPtr HFILE_ERROR = new IntPtr(-1);

        /// <summary>
        /// 確認檔案是否被占用中
        /// </summary>
        /// <param name="lpPathName"></param>
        /// <param name="iReadWrite"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern IntPtr _lopen(string lpPathName, int iReadWrite);

        /// <summary>
        /// 檢查檔案是否可用
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsIdleFile(string fileName) {
            bool rtn = true;
            if (rtn = File.Exists(fileName)) {
                rtn = HFILE_ERROR != _lopen(fileName, OF_READWRITE | OF_SHARE_DENY_NONE);
            }
            return rtn;
        }

    }

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
            IPEndPoint endPoint = new IPEndPoint(new IPAddress(new byte[] { 127,0,0,1}), Port);
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
            if (Socket == null) {
                Listen();
            }
            CtThread.CreateThread(ref Thread, "mTdClient: ", Task);
            Thread?.Start(this);
        }

        /// <summary>
        /// 停止接收
        /// </summary>
        public void Stop() {
            try {
                if (Socket?.Connected ?? false)Socket.Shutdown(SocketShutdown.Both);
                Socket?.Close();
                Socket = null;
                this.IsCancel = false;
                CtThread.KillThread(ref Thread);
            } catch (Exception ex){
                Console.WriteLine(ex.Message);
            }
        }

        public Socket Accept(int receiveTimeOut = 5000, int sendTimeout = 5000, int sendBuffer = 8192, int receiveBuffer = 1024) {
            Socket client =  Socket.Accept();
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, receiveTimeOut);
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, sendTimeout);
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, sendBuffer);
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, receiveBuffer);
            return client;
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
    public class CarInfo {

        #region Properteis

        public int CarID { get; private set; }

        public double X { get; private set; }

        public double Y { get; private set; }

        public double Theta { get; private set; }

        public int PowerPercent { get; private set; }

        public List<int> LaserData { get; private set; }

        public WorkMode Mode { get; private set; }

        #endregion Properties

        #region Function - Constructors

        private CarInfo() {

        }

        public CarInfo(int id ,double x,double y,double theta, int power,List<int> laser,WorkMode mode) {
            this.CarID = id;
            this.X = x;
            this.Y = y;
            this.Theta = theta;
            this.PowerPercent = power;
            this.LaserData = laser;
            this.Mode = mode;
        }

        #endregion Function - Constructors

        #region Function - Public Methods
        
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
            int carID = 0;
            int x = 0, y = 0, power = 0;
            double theta = 0;
            List<int> laser = null;
            WorkMode mode = WorkMode.Idle;
            info = new CarInfo();
            laserData = null;
            string[] strArray = src.Split(Separator.Param.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            bool suc =
                ((strArray?.Count() ?? 0) == 9) &&
                strArray[0] == MCmd.Get &&
                strArray[1] == SCmd.Car &&
                int.TryParse(strArray[2], out carID) &&
                int.TryParse(strArray[3], out x) &&
                int.TryParse(strArray[4], out y) &&
                double.TryParse(strArray[5], out theta) &&
                int.TryParse(strArray[6], out power) &&
                TryLaser(strArray[7], out laser) && 
                Enum.TryParse(strArray[8],out mode) ;
            if (suc) {
                info.CarID = carID;
                info.X = x;
                info.Y = y;
                info.Theta = theta;
                info.PowerPercent = power;
                info.LaserData = laser;
                laserData = laser;
                info.Mode = mode;
                }
            return suc;
        }

        /// <summary>
        /// 嘗試將字串轉為雷射資料
        /// </summary>
        /// <param name="context"></param>
        /// <param name="laser"></param>
        /// <returns></returns>
        public static bool TryLaser(string context,out List<int> laser) {
            string[] sreRemoteLaser = context.Split(new char[] {Separator.Data },StringSplitOptions.RemoveEmptyEntries);
            bool suc = true;
            laser = null;
            if (suc = (sreRemoteLaser?.Count() ?? 0) > 0) {
                int iVal = 0;
                laser = sreRemoteLaser.Select(item => { return int.TryParse(item,out iVal) ? iVal : 0; }).ToList();
            }
            return suc;
        }

        #endregion Function - Public Methods
    }

    /// <summary>
    /// 命令擴充方法
    /// </summary>
    public static class CmdExtension {

        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize) {
            return source.Select((x, i) => new { Index = i, Value = x }).
                GroupBy(x => x.Index / chunkSize).
                Select(x => x.Select(v => v.Value));
        }

        /// <summary>
        /// 以固定格式傳送命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="param"></param>
        public static void Send(this ICmdSender sender,params string[] param) {
            sender.Send(string.Join(Separator.Param, param));
        }
    }

    #endregion Support - Class

    #region Class - Factory

    /// <summary>
    /// 單例模式 - 命令核心物件工廠實例供應類
    /// </summary>
    public static class CommandCoreFactory {
        
        /// <summary>
        /// 工廠實例
        /// </summary>
        private static Factory mInstance = null;

        /// <summary>
        /// 取得工廠實例
        /// </summary>
        /// <returns></returns>
        public static Factory GetInstance() {
            if (mInstance == null) mInstance = new Factory();
            return mInstance;
        }

    }

    /// <summary>
    /// 命令核心物件工廠
    /// </summary>
    /// <remarks>
    /// 僅定義命令核心物件工廠，但不宣告為靜態立刻實例化
    /// 使用者透過單例模式來取得命令核心物件工廠實例
    /// </remarks>
    public class Factory {

        /// <summary>
        /// 不允許外部建立實例
        /// </summary>
        internal Factory() {}

        /// <summary>
        /// 取得Client端命令核心
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public ISoxCmdClient Client(string ip = "127.0.0.1") {
            return new CtSoxCmdClient(ip);
        }

        /// <summary>
        /// 取得Server端命令核心
        /// </summary>
        /// <returns></returns>
        public ISoxCmdServer Server() {
            return new CtSoxCmdServer();
        }

    }

    #endregion Class - Facotry

}
