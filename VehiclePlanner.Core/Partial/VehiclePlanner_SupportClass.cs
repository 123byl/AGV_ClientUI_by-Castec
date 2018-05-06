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
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Windows.Forms;

namespace VehiclePlanner.Core {

    #region Object

    /// <summary>
    /// 键盘钩子
    /// [以下代码来自某网友，并非本人原创]
    /// </summary>
    public class KeyboardHook {

        #region Declaration - Fields

        private static int hKeyboardHook = 0; //声明键盘钩子处理的初始值
                                              //值在Microsoft SDK的Winuser.h里查询
                                              // http://www.bianceng.cn/Programming/csharp/201410/45484.htm
        private HookProc KeyboardHookProcedure; //声明KeyboardHookProcedure作为HookProc类型

        private List<Keys> mKeyDowns = new List<Keys>();
        
        #endregion Declaration - Fields

        #region Declartion - Events

        /// <summary>
        /// 鍵盤按下事件
        /// </summary>
        public event KeyEventHandler KeyDownEvent;

        /// <summary>
        /// 鍵盤持續按壓事件
        /// </summary>
        public event KeyPressEventHandler KeyPressEvent;

        /// <summary>
        /// 鍵盤放開事件
        /// </summary>
        public event KeyEventHandler KeyUpEvent;

        #endregion Declaration - Events

        #region Declaration - DllImport

        //ToAscii职能的转换指定的虚拟键码和键盘状态的相应字符或字符
        [DllImport("user32")]
        private static extern int ToAscii(int uVirtKey, //[in] 指定虚拟关键代码进行翻译。
                                         int uScanCode, // [in] 指定的硬件扫描码的关键须翻译成英文。高阶位的这个值设定的关键，如果是（不压）
                                         byte[] lpbKeyState, // [in] 指针，以256字节数组，包含当前键盘的状态。每个元素（字节）的数组包含状态的一个关键。如果高阶位的字节是一套，关键是下跌（按下）。在低比特，如果设置表明，关键是对切换。在此功能，只有肘位的CAPS LOCK键是相关的。在切换状态的NUM个锁和滚动锁定键被忽略。
                                         byte[] lpwTransKey, // [out] 指针的缓冲区收到翻译字符或字符。
                                         int fuState); // [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise.

        //获取按键的状态
        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        //使用此功能，安装了一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        //调用此函数卸载钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);

        //使用此功能，通过信息钩子继续下一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        // 取得当前线程编号（线程钩子需要用到）
        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        //使用WINDOWS API函数代替获取当前实例的函数,防止钩子失效
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string name);

        #endregion Declaration - DllImport

        #region Declaration - Delegates

        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

        #endregion Declaration - Delegates

        #region Declaration - Const

        public const int WH_KEYBOARD_LL = 13;   //线程键盘钩子监听鼠标消息设为2，全局键盘监听鼠标消息设为13
        private const int WM_KEYDOWN = 0x100;//KEYDOWN
        private const int WM_KEYUP = 0x101;//KEYUP
        private const int WM_SYSKEYDOWN = 0x104;//SYSKEYDOWN
        private const int WM_SYSKEYUP = 0x105;//SYSKEYUP

        #endregion Declaration - Const

        #region Declaration - Struct

        //键盘结构
        [StructLayout(LayoutKind.Sequential)]
        public class KeyboardHookStruct {
            public int vkCode;  //定一个虚拟键码。该代码必须有一个价值的范围1至254
            public int scanCode; // 指定的硬件扫描码的关键
            public int flags;  // 键标志
            public int time; // 指定的时间戳记的这个讯息
            public int dwExtraInfo; // 指定额外信息相关的信息
        }

        #endregion Declaration - Struct

        #region Function - Public Methods

        /// <summary>
        /// 開始監聽鍵盤
        /// </summary>
        public void Start() {
            // 安装键盘钩子
            if (hKeyboardHook == 0) {
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName), 0);
                //hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                //************************************
                //键盘线程钩子
                //SetWindowsHookEx( 2,KeyboardHookProcedure, IntPtr.Zero, GetCurrentThreadId());//指定要监听的线程idGetCurrentThreadId(),
                //键盘全局钩子,需要引用空间(using System.Reflection;)
                //SetWindowsHookEx( 13,MouseHookProcedure,Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),0);
                //
                //关于SetWindowsHookEx (int idHook, HookProc lpfn, IntPtr hInstance, int threadId)函数将钩子加入到钩子链表中，说明一下四个参数：
                //idHook 钩子类型，即确定钩子监听何种消息，上面的代码中设为2，即监听键盘消息并且是线程钩子，如果是全局钩子监听键盘消息应设为13，
                //线程钩子监听鼠标消息设为7，全局钩子监听鼠标消息设为14。lpfn 钩子子程的地址指针。如果dwThreadId参数为0 或是一个由别的进程创建的
                //线程的标识，lpfn必须指向DLL中的钩子子程。 除此以外，lpfn可以指向当前进程的一段钩子子程代码。钩子函数的入口地址，当钩子钩到任何
                //消息后便调用这个函数。hInstance应用程序实例的句柄。标识包含lpfn所指的子程的DLL。如果threadId 标识当前进程创建的一个线程，而且子
                //程代码位于当前进程，hInstance必须为NULL。可以很简单的设定其为本应用程序的实例句柄。threaded 与安装的钩子子程相关联的线程的标识符
                //如果为0，钩子子程与所有的线程关联，即为全局钩子
                //************************************
                //如果SetWindowsHookEx失败
                if (hKeyboardHook == 0) {
                    Stop();
                    throw new Exception("安装键盘钩子失败");
                }
            }
        }

        /// <summary>
        /// 停止監聽鍵盤
        /// </summary>
        public void Stop() {
            bool retKeyboard = true;


            if (hKeyboardHook != 0) {
                retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }

            //if (!(retKeyboard)) throw new Exception("卸载钩子失败！");
        }

        #endregion Function - Public Methods

        #region Function - Private Methods

        /// <summary>
        /// 按鈕事件分析
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam) {
            // 侦听键盘事件
            if (nCode >= 0) {
                KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                Keys auxiliaryKey = Keys.None;
                // raise KeyDown
                if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    if (!mKeyDowns.Contains(keyData)) {
                        mKeyDowns.Add(keyData);
                        if (mKeyDowns.Any(k => k == Keys.LControlKey || k == Keys.RControlKey)) {
                            auxiliaryKey |= Keys.Control;
                        }
                        if (mKeyDowns.Any(k => k == Keys.LMenu || k == Keys.RMenu)) {
                            auxiliaryKey |= Keys.Alt;
                        }
                        if (mKeyDowns.Any(k => k == Keys.LShiftKey || k == Keys.RShiftKey)) {
                            auxiliaryKey |= Keys.Shift;
                        }
                        KeyEventArgs e = new KeyEventArgs(keyData | auxiliaryKey);
                        KeyDownEvent?.Invoke(this, e);
                    }
                }

                //键盘按下
                if (wParam == WM_KEYDOWN) {
                    byte[] keyState = new byte[256];
                    GetKeyboardState(keyState);

                    byte[] inBuffer = new byte[2];
                    if (ToAscii(MyKeyboardHookStruct.vkCode, MyKeyboardHookStruct.scanCode, keyState, inBuffer, MyKeyboardHookStruct.flags) == 1) {
                        KeyPressEventArgs e = new KeyPressEventArgs((char)inBuffer[0]);
                        KeyPressEvent?.Invoke(this, e);
                    }
                }

                // 键盘抬起
                if (wParam == WM_KEYUP || wParam == WM_SYSKEYUP) {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    if (mKeyDowns.Contains(keyData)) {
                        mKeyDowns.Remove(keyData);
                        if (mKeyDowns.Any(k => k == Keys.LControlKey || k == Keys.RControlKey)) {
                            auxiliaryKey |= Keys.Control;
                        }
                        if (mKeyDowns.Any(k => k == Keys.LMenu || k == Keys.RMenu)) {
                            auxiliaryKey |= Keys.Alt;
                        }
                        if (mKeyDowns.Any(k => k == Keys.LShiftKey || k == Keys.RShiftKey)) {
                            auxiliaryKey |= Keys.Shift;
                        }
                    }
                    KeyEventArgs e = new KeyEventArgs(keyData | auxiliaryKey);
                    KeyUpEvent?.Invoke(this, e);
                }

            }
            //如果返回1，则结束消息，这个消息到此为止，不再传递。
            //如果返回0或调用CallNextHookEx函数则消息出了这个钩子继续往下传递，也就是传给消息真正的接受者
            return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// 解構子
        /// </summary>
        ~KeyboardHook() {
            Stop();
        }

        #endregion Function - Private Methods
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

    #endregion Object
    
    #region EventArgs

    /// <summary>
    /// VehiclePlanner事件參數
    /// </summary>
    public class VehiclePlannerEventArgs : EventArgs {
        public VehiclePlannerEvents Events { get; }
        public VehiclePlannerEventArgs(VehiclePlannerEvents events) {
            this.Events = events;
        }
    }

    #endregion EvnetArgs

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

        public FakeVehicleConsole() {
            mServer = FactoryMode.Factory.SerialServer();
            mServer = FactoryMode.Factory.SerialServer();
            mServer.ConnectedEvent += MServer_ConnectedEvent;
            mServer.StartListening((int)EPort.VehiclePlanner, 3, VehiclePlannerReceiver);
            CtThread.CreateThread(ref t_VPSender, "mTdClientSender", tsk_AutoReportToVehiclePlanner);//iTS狀態自動回報(-> VehiclePlanner)
            mBroadcastReceiver = new BroadcastReceiver(false,mBroadcastReceiver_ReceivedData);
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
