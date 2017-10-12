using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net;

using Ace.Adept.Server.Motion;
using Ace.Core.Server;
using Ace.Core.Server.Motion;
using Ace.Adept.Server.Desktop.Connection;
using System.Diagnostics;
using CtLib.Module.Adept;
using CtLib.Module.Adept.Extension;
using CtLib.Library;
using CtLib.Module.Net;
using CtLib.Module.Utility;
namespace CtLib.Module.Adept {

    #region Declaration - EventArg

    #region Enum

    ///<summary>布林事件參數</summary>
    public enum BooleanEvents {
        ///<summary>Socket連線狀態變更事件</summary>
        ConnectSocket = 1,
        ///<summary>座標變更事件[True:World/False:Joint]</summary>
        Coordinate = 2,
        ///<summary>監測狀態變更</summary>
        Monitoring = 3,
        ///<summary>Ace連線狀態變更事件</summary>
        ConnectAce = 4,
        ///<summary>WorkSpace載入/卸載事件</summary>
        WorkSpace = 5
    }

    ///<summary>封包格式</summary>
    public enum PackageProtocol {
        ///<summary>世界座標</summary>
        World = 1,
        ///<summary>Joint座標</summary>
        Joint = 2,
        ///<summary>兩者皆有</summary>
        Both = 3
    }

    #endregion Enum

    #region Arg

    ///<summary>手臂觀測事件參數</summary>
    [Serializable]
    public class LocationUpdateEventArgs : EventArgs {

        #region Declaration - Fields

        ///<summary>數值小數位精度</summary>
        private static readonly byte mAccuracy = 3;

        #endregion Declaration - Fields

        #region Declaration - Properties

        ///<summary>[World]現在位置</summary>
        public List<double> wNow { get;private set; } = null;
        ///<summary>[World]位置變化量</summary>
        public List<double> wDelta { get; private set; } = null;
        ///<summary>[Joint]現在位置</summary>
        public List<double> jNow { get; private set; } = null;
        ///<summary>[Joint]位置變化量</summary>
        public List<double> jDelta { get; private set; } = null;
        ///<summary>取得位置花費時間</summary>
        public double? Spend { get; set; } = null;
        ///<summary>Socket封包格式</summary>
        public PackageProtocol? Protocol { get; set; } = null;
        ///<summary>手臂軸數</summary>
        public int JointLength { get; set; } = -1;
        
        #endregion Declaration - Properties

        #region Function - Constructors

        ///<summary>一般手臂觀測事件參數建構</summary>
        public LocationUpdateEventArgs() {
        }

        ///<summary>以Socket字串封包建構</summary>
        ///<param name="pack">Socket字串封包</param>
        public LocationUpdateEventArgs(string pack) {
            /*- 封包格式:移動花費時間;{手臂現在位置};{手臂位置變化量} -*/

            /*- 封包拆解 -*/
            string[] param = pack.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            /*- 取得封包標頭 -*/
            string[] tmp = param[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Protocol = (PackageProtocol)Enum.Parse(typeof(PackageProtocol), tmp[0]);
            JointLength = int.Parse(tmp[1]);

            /*- 取得花費時間與位置變化量 -*/
            tmp = param[1].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            Spend = double.Parse(tmp[0]);
            switch (Protocol) {
                case PackageProtocol.World:
                    wNow = ListDoubleParse(tmp[1]);
                    wDelta = ListDoubleParse(tmp[2]);
                    break;
                case PackageProtocol.Joint:
                    jNow = ListDoubleParse(tmp[1]);
                    jDelta = ListDoubleParse(tmp[2]);
                    break;
                case PackageProtocol.Both:
                    wNow = ListDoubleParse(tmp[1]);
                    wDelta = ListDoubleParse(tmp[2]);
                    jNow = ListDoubleParse(tmp[3]);
                    jDelta = ListDoubleParse(tmp[4]);
                    break;
                default:
                    throw new Exception("未定義通訊協定");
            }
        }

        ///<summary>以Socket位元封包建構</summary>
        ///<param name="pack">Socket位元封包</param>
        public LocationUpdateEventArgs(List<byte> pack) {
            
        }

        #endregion Function - Constructors

        #region Function - Public Methods

        ///<summary>將Socket位元封包進行解碼</summary>
        ///<param name="pack">Socket位元封包</param>
        ///<returns>解碼後的資料</returns>
        public static LocationUpdateEventArgs GetInstance(List<byte> pack) {
            if(!pack?.Any() ?? true) {
                Console.WriteLine("Socket位元封包無資料");
                return null;
            } 
            LocationUpdateEventArgs data = new LocationUpdateEventArgs();
            int step = 0;
            int Uint = 0;
            int i = 0;
            while(i < pack.Count) {
                switch (step) {
                    case 0://Head
                        byte head = pack[i];
                        if (!Enum.IsDefined(typeof(PackageProtocol), (head / 0x10))) {
                            Console.WriteLine("位元封包Protocol未定義");
                            return null;
                        }
                        data.Protocol = (PackageProtocol)(head / 0x10);
                        data.JointLength = head % 0x10;
                        step = 1;
                        i++;
                        break;
                    case 1://Spend
                        Uint = 2;
                        if (i + Uint > pack.Count - 1) {
                            Console.WriteLine("位元花費時間資料有缺");
                            return null;
                        }
                        data.Spend = ToData(pack, ref i, Uint)[0];
                        step = data.Protocol == PackageProtocol.Joint ? 3 : 2;
                        break;
                    case 2://Read World
                        Uint = 6 * 4;//6個座標軸乘上一個軸4個Byte
                        if (i + Uint > pack.Count) {
                            Console.WriteLine("位元資料有缺");
                            return null;
                        }
                        data.wNow = ToData(pack, ref i, Uint);
                        if (i + Uint > pack.Count) {
                            Console.WriteLine("位元資料有缺");
                            return null;
                        }
                        data.wDelta = ToData(pack, ref i, Uint);
                        if (data.Protocol == PackageProtocol.Both) step = 3;
                        break;
                    case 3: //Read Joint
                        Uint = 4 * data.JointLength;
                        if (i + Uint > pack.Count) {
                            Console.WriteLine("位元資料有缺");
                            return null;
                        }
                        data.jNow = ToData(pack, ref i, Uint);
                        if (i + Uint > pack.Count) {
                            Console.WriteLine("位元資料有缺");
                            return null;
                        }
                        data.jDelta = ToData(pack, ref i, Uint);
                        step = -1;
                        i = pack.Count;
                        break;
                    case -1://讀取完畢
                    default:
                        break;
                }
            }
            return data;
        }

        ///<summary>將Socket字串封包進行解碼</summary>
        ///<param name="pack">Sockety字串封包</param>
        ///<returns>解碼後的資料</returns>
        public static LocationUpdateEventArgs GetInstance(string pack) {
            LocationUpdateEventArgs data = new LocationUpdateEventArgs();
            /*- 封包格式:移動花費時間;{手臂現在位置};{手臂位置變化量} -*/

            /*- 封包拆解 -*/
            string[] param = pack.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (!param?.Any() ?? true) {
                Console.WriteLine("字串封包無資料");
                return null;
            }

            /*- 取得封包標頭 -*/
            string[] tmp = param[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (!tmp?.Any() ?? true) {
                Console.WriteLine("字串封包標頭無資料");
                return null;
            }

            PackageProtocol protocol = PackageProtocol.Both;
            if (!Enum.TryParse(tmp[0], out protocol)) {
                Console.WriteLine("字串Protocol轉換失敗");
                return null;
            }
            data.Protocol = protocol;

            int jointLen = 0;
            if (!int.TryParse(tmp[1], out jointLen)) return null;
            data.JointLength = jointLen;

            /*- 取得花費時間與位置變化量 -*/
            tmp = param[1].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if ((tmp?.Count() ?? 0) < 3) {
                Console.WriteLine("花費時間與位置資料有缺");
                return null;
            }
            data.Spend = double.Parse(tmp[0]);
            switch (data.Protocol) {
                case PackageProtocol.World:
                    data.wNow = ListDoubleParse(tmp[1]);
                    data.wDelta = ListDoubleParse(tmp[2]);
                    break;
                case PackageProtocol.Joint:
                    data.jNow = ListDoubleParse(tmp[1]);
                    data.jDelta = ListDoubleParse(tmp[2]);
                    break;
                case PackageProtocol.Both:
                    if (tmp.Count() != 5) {
                        Console.WriteLine("位置資料有缺");
                        return null;
                    }
                    data.wNow = ListDoubleParse(tmp[1]);
                    data.wDelta = ListDoubleParse(tmp[2]);
                    data.jNow = ListDoubleParse(tmp[3]);
                    data.jDelta = ListDoubleParse(tmp[4]);
                    break;
                default:
                    return null;
            }
            return data;
        }

        ///<summary>轉換為Socket位元封包</summary>
        public List<byte> ToBytePack() {
            /*- 標頭 高位元表示通訊格式，低位元表示手臂軸數 -*/
            byte head = (byte)((int)Protocol * 0x10 + JointLength);            
            IEnumerable<byte> spend = To2Byte(Spend);
            IEnumerable<byte> wn = To4Byte(wNow);
            IEnumerable<byte> wd = To4Byte(wDelta);
            IEnumerable<byte> jn = To4Byte(jNow);
            IEnumerable<byte> jd = To4Byte(jDelta);
            
            /*- 將各個小封包串起來 -*/
            List<byte> pack = new List<byte>();
            pack.Add(head);
            pack.AddRange(spend);
            switch (Protocol) {
                /*- World,JointLength/spend;{wNow};{wDelta} -*/
                case PackageProtocol.World:
                    pack.AddRange(wn);
                    pack.AddRange(wd);
                    break;
                /*- Joint,JointLength/spend;{jNow};{jDelta} -*/
                case PackageProtocol.Joint:
                    pack.AddRange(jn);
                    pack.AddRange(jd);
                    break;
                /*- Both,JointLength/spend;{wNow};{wDelta},{jNow},{jDelta} -*/
                case PackageProtocol.Both:
                    pack.AddRange(wn);
                    pack.AddRange(wd);
                    pack.AddRange(jn);
                    pack.AddRange(jd);
                    break;
            }

            return pack;            
        }
        
        ///<summary>轉換為Socket字串封包</summary>
        public string ToStringPack() {
            string wn = ToStringPack(wNow);
            string wd = ToStringPack(wDelta);
            string jn = ToStringPack(jNow);
            string jd = ToStringPack(jDelta);
            string spend =  $"{Spend:F0}";
            string pack = $"{Protocol.ToString()},{JointLength}/{spend}";
            switch (Protocol) {
                /*- World,JointLength/spend;{wNow};{wDelta} -*/
                case PackageProtocol.World:
                    pack = $"{pack};{wn};{wd}";
                    break;
                /*- Joint,JointLength/spend;{jNow};{jDelta} -*/
                case PackageProtocol.Joint:
                    pack = $"{pack};{jn};{jd}";
                    break;
                /*- Both,JointLength/spend;{wNow};{wDelta},{jNow},{jDelta} -*/
                case PackageProtocol.Both:
                    pack = $"{pack};{wn};{wd};{jn};{jd}";
                    break;
            }
            return pack;
        }

        ///<summary>設定現在位置</summary>
        ///<param name="now">手臂現在位置</param>
        ///<param name="isWorld">手臂座標[True:World/False:Joint]</param>
        public void SetNow(List<double> now,bool isWorld) {
            if (isWorld) wNow = now;
            else jNow = now;
        }

        ///<summary>設定位置變化量</summary>
        ///<param name="previous">手臂上一個位置位置</param>
        ///<param name="isWorld">手臂座標[True:World/False:Joint]</param>
        public void CalDelta(List<double> previous, bool isWorld) {
            List<double> now = isWorld ? wNow : jNow;
            List<double> delta = new List<double>();
            for (int i = 0;i< now.Count; i++) {
                delta.Add(Math.Abs(now[i] - previous[i]));
            }
            if (isWorld) wDelta = delta;
            else jDelta = delta;
        }

        #endregion Function - Public Methods

        #region Function - Private Methods

        ///<summary>從封包中指定區段讀取資料轉換為數組</summary>
        ///<param name="pack">來源封包</param>
        ///<param name="index">資料處理指標，表示從何開始處理</param>
        ///<param name="count">目標資料長度</param>
        ///<returns>轉換後的數組</returns>
        public static List<double> ToData(List<byte> pack, ref int index, int count) {
            int p = 0;//目前讀取數值的位元數
            int end = index + count;//要處理的資料結尾位置
            uint cell = 0;//讀值暫存
            List<double> data = new List<double>();//轉換後的資料
            int idx = 1;
            for (int i = index;i < end; i++) {
                /*- 依照位元數乘上權值並全部累加 -*/
                cell += (uint)((idx) * pack[i]);
                idx = idx << 8;
                /*- 數值為32位元即4個Byte，也就表示讀值完畢 -*/
                if (p++ == 3) {
                    int sv = 0;
                    if (cell >= 0x80000000) {
                        sv = (int)(-1 * (0xFFFFFFFF - cell + 1));
                    } else {
                        sv = (int)cell;
                    }
                    
                    /*- 最後再除以精度即是正確數值 -*/
                    data.Add((double)sv/Math.Pow(10,mAccuracy));

                    /*- 數值復歸繼續讀下一個數值 -*/
                    cell = 0;
                    p = 0;
                    idx = 1;
                }
            }
            /*- 若是還有資料則一起塞進去 -*/
            if (cell != 0) data.Add(cell);

            /*- 移動資料處理指標，表示已處理完畢 -*/
            index += count;
            return data;
        }

        ///<summary>數值轉換為長度為4Byte的數列</summary>
        public static IEnumerable<byte> To4Byte(double val) {

            byte[] ret = new byte[4];//轉換後的資料
            int i = 0;//資料指標，表示塞到第幾個資料
            bool symbol = false;
            /*- 針對負號進行處理 -*/
            if (val < 0) {
                /*- 將負號移至符號位元 -*/
                val = (double)(-1 * val);
                symbol = true;
            }

            /*- 將精度以上的位數往前提，後面的數值捨棄 -*/
            uint tmp = (uint)(val * Math.Pow(10, mAccuracy));
            if (symbol) {
                tmp = 0xFFFFFFFF - tmp + 1;
            }
            /*- 從最低位元開始塞 -*/
            while (tmp > 0x100) {
                ret[i++] = ((byte)(tmp % 0x100));
                tmp = tmp / 0x100;                
            }
            ret[i] = (byte)tmp;

            return ret; 
        }

        ///<summary>數列轉換為單位長度4Byte的Byte數列</summary>
        public static IEnumerable<byte> To4Byte(List<double> val) {
            return val?.ConvertAll(v => To4Byte(v))?.SelectMany(a => a);
        }

        ///<summary>字串數列轉換為ListDouble類型數列</summary>
        ///<param name="src">以","做分割字串數列的</param>
        ///<returns>轉換後的數列</returns>
        private static List<double> ListDoubleParse(string src) {
            return src?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)?.
                        ConvertAll(v => double.Parse(v))?.
                        ToList();
        }

        ///<summary>數值轉換為長度為2Byte的數列</summary>
        private IEnumerable<byte> To2Byte(double? val) {
            //Index:0 -> Count
            //L => H
            return new byte[] {(byte)(Spend % 0x100), (byte)(Spend / 0x100) };
        }
        
        ///<summary>將座標轉換為字串封包</summary>
        ///<param name="val">座標值</param>
        ///<returns>字串封包</returns>
        private static string ToStringPack(List<double> val) {
            return val != null ? string.Join(",",val.ConvertAll(v => v.ToString($"F{mAccuracy}"))) : null;
        }

        #endregion Function - Private Methods
    }

    ///<summary>事件參數</summary>
    [Serializable]
    public class BooleanEventArgs : EventArgs {
        ///<summary>連線狀態</summary>
        public bool Value { get; }
        ///<summary>事件代號</summary>
        public BooleanEvents Event {get;}
        ///<summary>Socket事件參數建置</summary>
        public BooleanEventArgs(bool val,BooleanEvents even) {
            this.Value = val;
            this.Event = even;
        }
    }

    #endregion Arg

    #endregion Declaration - EventArg

    #region Class - Core

    ///<summary>手臂動作觀測類</summary>
    [Serializable]
    public class CtRobotMonitor {
        
        #region Declaration - Fields

        ///<summary>單例實體</summary>
        private static CtRobotMonitor mInstance = null;
        ///<summary>AceServer物件參考</summary>
        private IAceServer rAceServer = null;
        ///<summary>手臂物件</summary>
        private IRobot mRobot = null;
        ///<summary>手臂軸數</summary>
        private int mJointLength = 0;
        ///<summary>V+連接器參考</summary>
        private VpObjects rVpLink = null;
        ///<summary>手臂位置觀測執行緒</summary>
        private Thread mTsk_Monitor = null;
        ///<summary>[座標系]True:World/False:Joint</summary>
        private bool mIsWorld = false;
        ///<summary>Socket物件</summary>
        private CtTcpSocket mSocket = null;
        ///<summary>ACE物件參考</summary>
        private CtAce rAce = null;
        ///<summary>顯示介面視窗</summary>
        private RobotMonitor mMonitor = null;
        /// <summary>
        /// 記錄檔儲存路徑
        /// </summary>
        private string mRecordPath = string.Empty;
        /// <summary>
        /// 記錄檔檔名(不含副檔名)
        /// </summary>
        private string mRecordFile = "RobotMonitor";
        #endregion Declaration - Fields

        #region Declaration - Properties

        ///<summary>[座標系]True:World/False:Joint</summary>
        public bool IsWorld { get { return mIsWorld; } set { mIsWorld = value; RaisedBooleanEvent(mIsWorld, BooleanEvents.Coordinate); } }
        ///<summary>手臂軸數</summary>
        public int JointLength { get { return mJointLength; } }
        ///<summary>是否與V+建立連線</summary>
        public bool IsConnectedToVp{ get { return rVpLink?.Obj(link => link.IsAlive) ?? false; } }
        ///<summary>是否正在監看中</summary>
        public bool IsMonitoring { get { return mTsk_Monitor?.IsAlive ?? false; } }
        ///<summary>Socket是否已建立連線</summary>
        public bool IsConnectedToSocket { get; private set; } = false;
        ///<summary>Socket腳色</summary>
        public SocketRole Role { get { return mSocket?.Role ?? SocketRole.Server; } }
        ///<summary>是否有控制器</summary>
        public bool WithControl { get { return rAce?.IsSmartControl ?? false; } }
        ///<summary>傳送封包是否包含World與Joint兩者</summary>
        public bool IsFullPackage { get; set; } = false;
        /// <summary>
        /// 是否儲存紀錄於txt檔
        /// </summary>
        public bool IsSaveTxt { get; set; } = false;
        ///<summary>Socket封包傳輸格式</summary>
        public TransDataFormats SocketFormat { get; set; } = TransDataFormats.EnumerableOfByte;
        ///<summary>Log檔名</summary>
        public static readonly string LogName = "RobotMonitor.log";
        #endregion Declaration - Properties

        #region Delcaration - Events

        ///<summary>手臂位置更新事件</summary>
        public event EventHandler<LocationUpdateEventArgs> OnLocationUpdate;

        ///<summary>布林狀態變更事件</summary>
        public event EventHandler<BooleanEventArgs> OnBooleanChanged;

        ///<summary>手臂位置更新發報</summary>
        ///<param name="e">手臂位置更新事件參數</param>
        private void RaisedLocationUpdate(LocationUpdateEventArgs e) {
            if (IsWorld && e.Protocol == PackageProtocol.Joint) IsWorld = false;
            else if (!IsWorld && e.Protocol == PackageProtocol.World) IsWorld = true; 
            /*-發報-*/
            OnLocationUpdate?.Invoke(this, e);
            /*- 若是已連上的Client則回傳資料至Server端 -*/
            if (IsConnectedToSocket) {
                string packMsg = ""; 
                try {
                    //string packRecord = "";
                    if (mSocket.DataFormat == TransDataFormats.EnumerableOfByte) {
                        var pack = e.ToBytePack();
                        packMsg = $"pack count ={pack?.Count ?? -1}";
                        mSocket.BeginSend(pack);
                        PackRecord(pack);
                    } else {
                        var pack = e.ToStringPack();
                        packMsg = $"pack len = {pack?.Length ?? -1}";
                        mSocket.BeginSend(pack);
                        PackRecord(pack);
                    }

                } catch(Exception ex) {
                    CtStatus.Report(Stat.ER_SYSTEM, ex, false);
                    IsConnectedToSocket = false;
                    RaisedBooleanEvent(false, BooleanEvents.ConnectSocket);
                    CtLog.TraceLog("RaisedLocInfo", $"mSocket i {mSocket?.GetType()}", LogName);
                    if (packMsg != null) CtLog.TraceLog("RaisedLocInfo", packMsg, LogName);
                }

            }
        }
        
        ///<summary>Socket布林狀態事件發報</summary>
        ///<param name="cnn">Socket布林變更事件參數</param>
        ///<param name="even">事件代號</param>
        private void RaisedBooleanEvent(bool cnn, BooleanEvents even) {
            OnBooleanChanged?.Invoke(this, new BooleanEventArgs(cnn, even));
            if (even == BooleanEvents.ConnectSocket){
                IsConnectedToSocket = cnn;
                CtLog.TraceLog("RaisedSocket", $"Socket is {(cnn ? "Connected" : "Disconnected")}", LogName);
            }
        }

        #endregion Declaration - Evetns

        #region Funciton - Constructors

        ///<summary>供Ace建構實例</summary>
        ///<remarks>
        ///若是讓ACE自行建構實例執行監測執行緒
        ///會導致執行緒消失在異次元無法停止
        ///請使用單例模式GetInstance方法來在ACE端取得實例
        /// </remarks>
        ///<param name="aceSrv">ACE Server實例</param>
        private CtRobotMonitor(IAceServer aceSrv) : this(aceSrv,null) {
            CtLog.TraceLog("CtRobotMonitor",$"aceSrv = {(aceSrv == null ? "null" : "HaveInstance")}",LogName);
            aceSrv.WorkspaceLoad += AceSrv_WorkspaceLoad;
            aceSrv.WorkspaceUnload += AceSrv_WorkspaceUnload;
            CtLog.TraceLog("CtRobotMonitor","WorkSpace事件委派完畢", LogName);
        }
        
        ///<summary>供CtAce建構實例</summary>
        public CtRobotMonitor(CtAce ace) : this(ace?.GetServer(),ace?.GetVpLink()) {
            rAce = ace;
            //rAce.OnBoolEventChanged += rAce_OnBoolEventChanged;
        }

        ///<summary>通用建構實例</summary>
        ///<param name="aceSrv">ACE Server實例</param>
        ///<param name="vpLink">V+連接物件</param>
        private CtRobotMonitor(IAceServer aceSrv, VpObjects vpLink) {
            /*- 取得Ace Server物件參考 -*/
            rAceServer = aceSrv;
            rVpLink = vpLink;
            GetRobot();
            mRecordPath = CtDefaultPath.GetPath(SystemPath.Log);
        }

        private void GetRobot() {
            /*- 取得手臂物件 -*/

            var robot = rAceServer.Root.FilterType(typeof(IAdeptRobot));
            if (robot.Any()) {
                mRobot = (IRobot)robot[0];
                CtLog.TraceLog("GetRobot","IsAdeptRobot", LogName);
            } else {
                robot = rAceServer.Root.FilterType(typeof(IRobot));
                if (robot.Any()) {
                    mRobot = (IRobot)robot[0];
                    CtLog.TraceLog("GetRobot", "IsRobot", LogName);
                }
            }

            CtLog.TraceLog("GetRobot", $"mRobot = {(mRobot == null ? "null" : "HaveInstance")}", LogName);
            if (mRobot == null) return;
            /*- 取得手臂軸數 -*/
            mJointLength = mRobot.JointCount;
            CtLog.TraceLog("GetRobot", $"JointCount={mJointLength}", LogName);

            /*- 取得V+物件參考 -*/
            if (rVpLink == null) rVpLink =
                new VpObjects(rAceServer.Root.FilterType(typeof(IVpLinkedObject), true).
                Cast<IVpLinkedObject>());
            CtLog.TraceLog("GetRobot",$"rVpLink is {rVpLink?.GetType()}",LogName);
        }

        #endregion Function - Constructors

        #region Function - Private Methods
        
        ///<summary>Socket封包記錄檔</summary>
        private void PackRecord(List<byte> pack) {
            if (IsSaveTxt) {
                string recordPath = $"{mRecordPath}{mRecordFile}";
                string strPack = string.Join(" ", pack.ConvertAll(v => $"{v:X2}"));
                CtFile.WriteFile($"{recordPath}.txt", strPack, true);
                strPack = string.Join(",", pack.ConvertAll(v => $"0x{v:X2}"));
                CtFile.WriteFile($"{recordPath}.csv", strPack, true);
            }
        }

        ///<summary>Socket封包記錄檔</summary>
        private void PackRecord(string pack) {
            if (IsSaveTxt) {
                pack = pack.Replace(",", "\t,\t");
                pack = pack.Replace("/", "\t/\t");
                pack = pack.Replace(";", "\t;\t");
                
                string recordPath = $"{mRecordPath}{mRecordFile}";
                CtFile.WriteFile($"{recordPath}.txt", pack, true);
                pack = pack.Replace(",", "，");
                pack = pack.Replace("\t", ",");
                CtFile.WriteFile($"{recordPath}.csv", pack, true);
            }
        }


        ///<summary>回傳現在位置</summary>
        ///<param name="isWorld">True:World座標/False:Joint座標</param>
        ///<returns>目前座標位置</returns>
        private List<double> GetLocation(bool isWorld) {
            List<double> ret = null;
            try {
                ret = isWorld ?
                    rVpLink.Link(link => link.WhereWorld(1)).ToList() :
                    rVpLink.Link(link => link.WhereJoint(1)).ToList();
            } catch (Exception ex){//ACE與手臂連線中斷導致無法進入通信渠道
                CtStatus.Report(Stat.ER_SYSTEM, ex, false);
                CtLog.TraceLog("GetLocInfo",$"rVpLink is {rVpLink?.GetType()}",LogName);
                Monitor(false);
                DisconnectSocket();
            }
            return ret;
            
        }

        ///<summary>位元格式Socket封包解碼</summary>
        ///<param name="pack">Socket封包</param>
        ///<param name="data">解碼後資料</param>
        ///<returns>是否解碼成功</returns>
        private bool Decoder(List<byte> pack,out LocationUpdateEventArgs data) {
            return (data = LocationUpdateEventArgs.GetInstance(pack)) != null;
        }

        ///<summary>字串格式Socket封包解碼</summary>
        ///<param name="pack">Socket封包</param>
        ///<param name="data">解碼後資料</param>
        ///<returns>是否解碼成功</returns>
        private bool Decoder(string pack, out LocationUpdateEventArgs data) {            
            return (data = LocationUpdateEventArgs.GetInstance(pack)) != null;
        }

        ///<summary>單次計算手臂位移量</summary>
        private void UpdateLocInfo() {
            /*-取得手臂最新位置及取得花費時間-*/
            /*- 快速從ACE連續撈兩筆手臂位置資訊所花時間除2才是移動花費時間 -*/
            LocationUpdateEventArgs e = new LocationUpdateEventArgs();
            DateTime timeMark = DateTime.Now;
            List<double> nprevious = null;
            List<double> previous =  GetLocation(mIsWorld);
            e.SetNow(GetLocation(mIsWorld),mIsWorld);
            if (!IsFullPackage) {
                e.Spend = DateTime.Now.Subtract(timeMark).TotalMilliseconds / 2;
            } else {
                nprevious = GetLocation(!mIsWorld);
                e.SetNow(GetLocation(!mIsWorld), !mIsWorld);
                e.Spend = DateTime.Now.Subtract(timeMark).TotalMilliseconds / 2;
                e.CalDelta(nprevious, !mIsWorld);
            }
            e.CalDelta(previous, mIsWorld);
            
            e.Protocol = IsFullPackage ? PackageProtocol.Both :
                mIsWorld ?
                PackageProtocol.World :
                PackageProtocol.Joint;

            e.JointLength = mJointLength;

            /*- 發報 -*/
            RaisedLocationUpdate(e);
            List<byte> data = LocationUpdateEventArgs.To4Byte(48.454).ToList();
            int i = 0;
            List<double> val = LocationUpdateEventArgs.ToData(data,ref i,data.Count());
        }

        ///<summary>Socket連線</summary>
        ///<param name="ip">Server端IP</param>
        ///<param name="port">通訊埠號</param>
        ///<param name="listen">連至Server端或等待Client連線</param>
        private void connectSocket(string ip, int port, bool listen) {
            if (!IsConnectedToSocket) {
                if (mSocket == null) {
                    mSocket = new CtTcpSocket(SocketFormat, true);
                    mSocket.OnSocketEvents += mSocket_OnSocketEvents;
                    CtLog.TraceLog("cnnectSocket","Socket物件重新實例化", LogName);
                }
                if (listen) {
                    CtLog.TraceLog("cnnectSocket", "Start Listen",LogName);
                    mSocket.ServerBeginListen(port);
                    RaisedBooleanEvent(true, BooleanEvents.ConnectSocket);
                } else {
                    CtLog.TraceLog("cnnectSocket", "Connetc To Srv",LogName);
                    mSocket.ClientBeginConnect(ip, port);
                }
            }
        }

        #endregion Funciton - Private Methods

        #region Function - Public Methods

        #region Ace Client

        ///<summary>[Client]與ACE連接</summary>
        public void ConnectAce(bool withCtrl) {
            if (!rAce?.IsVpConnected() ?? false) {
                rAce.Connect(withCtrl ? ControllerType.SmartController : ControllerType.Embedded,true);
                Monitor(rAce.IsVpConnected());
            }
        }

        ///<summary>[Client]與ACE斷開連接</summary>
        public void DisconnectAce() {
            if (rAce?.IsVpConnected() ?? false) {
                Monitor(false);
                rVpLink = null;
                rAceServer = null;
                rAce.Disconnect();
            }
        }

        #endregion Ace Client

        #region Socket

        ///<summary>建立Socket連線</summary>
        ///<param name="ip">伺服端IP</param>
        ///<param name="port">伺服端Port號</param>
        public void ConnectSocket(string ip, int port) {
            connectSocket(ip, port, false);
        }
        
        ///<summary>開始等待Socket Client連線</summary>
        ///<param name="port">使用的Port號</param>
        public void ListenSocket(int port) {
            connectSocket(null, port, true);
        }

        ///<summary>中斷Socket連線</summary>
        public void DisconnectSocket() {
            if (IsConnectedToSocket) {
                IsConnectedToSocket = false;
                if (mSocket.Role == SocketRole.Client) {
                    mSocket.ClientDisconnect();
                } else {
                    mSocket.ServerStopListen();
                }
                if (mSocket != null) {
                    mSocket.OnSocketEvents -= mSocket_OnSocketEvents;
                    mSocket.Dispose();
                    mSocket = null;
                }
                RaisedBooleanEvent(false, BooleanEvents.ConnectSocket);
                CtLog.TraceLog("Socket", "Disconnect",LogName);
            }
        }

        #endregion Socket

        ///<summary>開始/停止手臂位置觀測</summary>
        ///<param name="start">True:開始/False:停止</param>
        public void Monitor(bool start) {

            CtLog.TraceLog("Monitor", $"{start}", LogName);

            if (start && (!mTsk_Monitor?.IsAlive ?? true)) {
                

                /*- 只有CtAce.Disconnet會銷毀物件 -*/
                /*- 當CtAce重新連線後重新取得參考 -*/
                if (rAceServer == null) rAceServer = rAce.GetServer();
                if (rVpLink == null) rVpLink = rAce.GetVpLink();
                
                rVpLink = rVpLink ?? rAce.GetVpLink();

                mTsk_Monitor = new Thread(tsk_Monitor);
                mTsk_Monitor.Start();
                
            } else if (!start) {
                CtLog.TraceLog("Monitor", "KillTask", LogName);
                CtThread.KillThread(ref mTsk_Monitor);
            }
        }
        
        ///<summary>顯示介面</summary>
        public void Show() {
            if (mMonitor == null) {
                mMonitor = new RobotMonitor(this);
            }

            mMonitor.Show();
        }

        ///<summary>供ACE使用取得實例</summary>
        ///<param name="aceSrv">ACE Server物件參考</param>
        ///<returns>CtRobotMonitor實例</returns>
        public static CtRobotMonitor GetInstance(IAceServer aceSrv) {
            return mInstance ?? (mInstance = new CtRobotMonitor(aceSrv));
        }
        
        ///<summary>單次發報手臂位置資訊[會停止監測執行緒]</summary>
        public void UpdateLcationInfo() {
            if (IsMonitoring) Monitor(false);
            UpdateLocInfo();
        }

        ///<summary>重新抓取手臂軸數</summary>
        ///<remarks>
        /// ACE端載入WorkSpace時手臂軸數讀取異常
        /// </remarks>
        public void RefreshJointLen() {
            mJointLength = mRobot.JointCount;
            CtLog.TraceLog("RefreshJointLen", $"JointLen = {mJointLength}", LogName);
        }

        #endregion Function - Public Methods

        #region Funciton - Tsk

        ///<summary>手臂位置觀測執行緒</summary>
        private void tsk_Monitor() {
            try {
                RaisedBooleanEvent(true, BooleanEvents.Monitoring);
                do {
                    /*- 手臂位置資訊發報 -*/
                    UpdateLocInfo();
                    CtTimer.Delay(100);
                } while (mTsk_Monitor?.IsAlive ?? false);
            } catch (ThreadAbortException) {
            } catch (ThreadInterruptedException) {
            } catch (Exception ex){
                CtStatus.Report(Stat.ER_SYSTEM, ex, true);
            }finally {
                RaisedBooleanEvent(false, BooleanEvents.Monitoring);
            }
        }

        #endregion Function - Tsk

        #region Function - Events

        ///<summary>Socket事件處理</summary>
        private void mSocket_OnSocketEvents(object sender, SocketEventArgs e) {
            switch (e.Event) {
                case SocketEvents.DataReceived:
                    /*- 自身無監控則顯示遠端監控值 -*/
                    if (!IsMonitoring) {
                        SocketRxData data = e.Value as SocketRxData;
                        LocationUpdateEventArgs eventArg = null;
                        if ( mSocket.DataFormat == TransDataFormats.String ?
                            Decoder(data.Data as string, out eventArg)://以string進行解碼
                            Decoder(data.Data as List<byte>, out eventArg)) {//以byte進行解碼
                            RaisedLocationUpdate(eventArg);
                        }else {
                            Console.WriteLine("錯誤");
                        }
                    }
                    break;
                case SocketEvents.ConnectionWithServer:
                    bool cnn = (e.Value as SocketConnection).Status;
                    RaisedBooleanEvent(cnn, BooleanEvents.ConnectSocket);
                    if (!cnn && mSocket != null) {
                        mSocket.OnSocketEvents -= mSocket_OnSocketEvents;
                        mSocket.Dispose();
                        mSocket = null;
                    }
                    break;
                case SocketEvents.ClientConnection:
                    
                    break;
            }
        }

        ///<summary>CtAce事件發報</summary>
        private void rAce_OnBoolEventChanged(object sender, AceBoolEventArgs e) {
            if (e.Events == AceBoolEvents.Connection) {
                RaisedBooleanEvent(e.Value, BooleanEvents.ConnectAce);
            }
        }

        ///<summary>WorkSpace載入事件</summary>
        private void AceSrv_WorkspaceUnload(object sender, EventArgs e) {
            RaisedBooleanEvent(false, BooleanEvents.WorkSpace);
        }

        ///<summary>WorkSpace卸載事件</summary>
        private void AceSrv_WorkspaceLoad(object sender, Ace.Core.Server.Event.WorkspaceLoadEventArgs e) {
            GetRobot();
            CtLog.TraceLog("WorkSpaceLoad", $"mRobot = {(mRobot == null ? "null" : "HaveInstance")}", LogName);
            RaisedBooleanEvent(true, BooleanEvents.WorkSpace);
        }
        
        #endregion Function - Events

    }

    #endregion Class - Core
}
