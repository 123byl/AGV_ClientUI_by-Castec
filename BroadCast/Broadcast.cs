using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BroadCast {

    /// <summary>
    /// 執行緒函式庫
    /// </summary>
    internal class CtThread {

        /// <summary>建立執行緒，並將該執行緒之函數指向 "不需" 帶參數之副程式</summary>
        /// <param name="thread">欲建立之執行緒。需使用ref關鍵字，將外部物件傳入以建立，否則建立之物件將不屬於Owner</param>
        /// <param name="name">該執行緒名稱</param>
        /// <param name="method">委派副程式位址，指向不需帶參數、無回傳值之副程式</param>
        /// <param name="background">是否為背景執行緒。  (<see langword="true"/>)背景執行緒，將依附在Owner(前景執行緒)上，關閉時隨之關閉   (<see langword="false"/>)前景執行緒，不依附於任何Owner/Thread上</param>
        /// <param name="start">是否建立後直接開始?  (<see langword="true"/>)直接開始  (<see langword="false"/>)由外部控制該Thread執行</param>
        /// <example>
        /// 以下示範簡單的 Thread 使用方式
        /// <code language="C#">
        /// private void tsk() {
        ///     ;
        /// }
        ///
        /// Thread thread;  //建立執行緒
        /// CtThread.CreateThread(ref thread, "ThreadName", tsk);    //建立執行緒並直接執行
        /// </code></example>
        /// <remarks>
        /// <see cref="System.Threading.ThreadStart"/> 為指向不帶參數之方法的委派
        /// </remarks>
        public static void CreateThread(ref Thread thread, string name, Action method, bool background = true, bool start = true) {
            if (thread != null) KillThread(ref thread);
            thread = new Thread(new ThreadStart(method)) {
                IsBackground = background,
                Name = name
            };
            if (start) thread.Start();
        }

        /// <summary>關閉執行緒</summary>
        /// <param name="thread">欲關閉之執行緒，需帶入ref關鍵字以帶入該物件記憶體位置，避免此處關閉但外實際上沒有之窘境</param>
        /// <param name="timeout">等待 Thread 完整關閉的時間，超過時間將忽略並繼續往下。 "-1" 表示忽略，等待完全停止</param>
        /// <returns>Status Code</returns>
        public static bool KillThread(ref Thread thread, int timeout = -1) {
            bool stt = true;
            try {
                /*-- 檢查是否該執行緒真的存在 --*/
                if (thread != null && thread.IsAlive) {
                    thread.Interrupt(); //插斷執行緒內迴圈
                    thread.Abort();     //停止任何工作，但工作並非馬上會結束
                    if (timeout > 0) thread.Join(timeout); //等待該執行緒丟出ThreadInterruptedException表示已完整結束
                    else thread.Join();
                }

                /*-- 延遲一下下 --*/
                Thread.Sleep(1);
            } catch (ThreadAbortException) {
                /*-- Abort Exception 是關閉時很有可能發生的，這裡的 catch 用來防止 --*/
            } catch (ThreadInterruptedException) {
                /*-- Interrupt Exception 是關閉時很有可能發生的，這裡的 catch 用來防止 --*/
            } catch (Exception ex) {
                stt = false;
                Console.WriteLine(ex.Message);
            } finally {
                /*-- 再次檢查 --*/
                if ((thread != null) && (thread.IsAlive)) {
                    stt = false;  //如果還活著表示沒有關成功
                } else {
                    stt = false;
                    thread = null;             //關閉成功則將記憶體清掉，以利後續重複利用
                }
            }
            return stt;
        }
    }

    /// <summary>
    /// 廣播事件參數
    /// </summary>
    public class BroadcastEventArgs : EventArgs {

        /// <summary>
        /// 廣播訊息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 廣播發布位置
        /// </summary>
        public IPEndPoint Remote { get; set; }
    }

    /// <summary>
    /// 廣播接收器
    /// </summary>
    public class BroadcastReceiver {

        #region Declaration - Fields

        /// <summary>
        /// 網路接口
        /// </summary>
        protected Socket mSocket = null;

        /// <summary>
        /// 本地端IP
        /// </summary>
        protected IPEndPoint mLocal = new IPEndPoint(IPAddress.Any, 1688);

        /// <summary>
        /// 廣播接收執行緒
        /// </summary>
        protected Thread mRecever = null;

        /// <summary>
        /// 執行緒鎖
        /// </summary>
        protected object mKey = new object();

        #endregion Declaration - Fields

        #region Declaration - Properties

        /// <summary>
        /// 是否在接收廣播
        /// </summary>
        public bool IsReceiving {
            get {
                lock (mKey) {
                    return mRecever?.IsAlive ?? false;
                }
            }
        }

        /// <summary>
        /// 廣播埠號
        /// </summary>
        public int Port {
            get {
                return mLocal.Port;
            }
        }

        #endregion Declaration - Properties

        #region Evnents

        /// <summary>
        /// 廣播接收事件
        /// </summary>
        public event EventHandler<BroadcastEventArgs> ReceivedData = null;

        #endregion Evnents

        #region Funciotn - Constructors

        public BroadcastReceiver() {
            SetBroadcastPort(1688);
        }

        public BroadcastReceiver(bool start,EventHandler<BroadcastEventArgs> handle):this() {
            if (handle != null) ReceivedData += handle;
            StartReceive(start);
        }

        #endregion Funciotn - Constructors

        #region Function - Private Methods

        /// <summary>
        /// Socket初始化
        /// </summary>
        protected virtual void SocketInitial() {
            mSocket.Bind(mLocal);
        }

        /// <summary>
        /// 建立廣播接收執行緒
        /// </summary>
        protected virtual void CreatThread() {
            CtThread.CreateThread(ref mRecever, "mTdReceiver", tsk_BroadcastReceiver);
        }

        /// <summary>
        /// 廣播接收事件發報
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RaiseEvent(object sender, BroadcastEventArgs e) {
            ReceivedData?.Invoke(sender, e);
        }

        /// <summary>
        /// 設定要廣播的埠號
        /// </summary>
        /// <param name="port">目標埠號</param>
        protected virtual void SetBroadcastPort(int port) {
            mLocal.Port = port;
        }

        #endregion Function - Private Methods

        #region Function - Public Methods

        /// <summary>
        /// 開始/停止接收廣播
        /// </summary>
        /// <param name="start">是否開始接收廣播</param>
        public virtual void StartReceive(bool start, int port = 1688) {
            lock (mKey) {
                if (start != IsReceiving) {
                    if (start) {
                        if (mSocket == null) {
                            if (mLocal.Port != port) {
                                SetBroadcastPort(port);
                            }
                            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                            SocketInitial();
                        }
                        CreatThread();
                    } else {
                        if (mSocket != null) {
                            mSocket.Dispose();
                            mSocket = null;
                        }
                        CtThread.KillThread(ref mRecever);
                    }
                }
            }
        }

        /// <summary>
        /// 傳遞訊息到指定IP
        /// </summary>
        /// <param name="msg">要傳遞的訊息</param>
        /// <param name="remote">目標IP</param>
        public void Send(string msg, EndPoint remote) {
            lock (mKey) {
                mSocket?.SendTo(Encoding.UTF8.GetBytes(msg), remote);
            }
        }

        #endregion Function - Public Methods

        #region Funciton - Task

        /// <summary>
        /// 廣播接收執行緒
        /// </summary>
        protected void tsk_BroadcastReceiver() {
            try {
                EndPoint IP = mLocal;
                byte[] getdata = new byte[1024]; //要接收的封包大小
                string msg;
                int recv;
                while (true) {
                    if (mSocket?.IsBound ?? false) {
                        recv = mSocket.ReceiveFrom(getdata, ref IP); //把接收的封包放進getdata且傳回大小存入recv
                        msg = Encoding.UTF8.GetString(getdata, 0, recv); //把接收的byte資料轉回string型態
                        Console.WriteLine($"Receive {msg} from {IP}");
                        RaiseEvent(this, new BroadcastEventArgs() { Message = msg, Remote = IP as IPEndPoint });
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine($"[{this.GetType().Name}]{ex.Message}");
                Task.Factory.StartNew(() => StartReceive(false));
            }
        }

        #endregion Funciton - Task
    }

    /// <summary>
    /// 廣播發送器
    /// </summary>
    public class Broadcaster : BroadcastReceiver {

        #region Declaration - Fiedls

        /// <summary>
        /// 要廣播的位置
        /// </summary>
        private List<IPEndPoint> mRemote = new List<IPEndPoint>();

        #endregion Declaration - Fiedls

        #region Funciotn - Consturctors

        public Broadcaster() : base() {
        }

        #endregion Funciotn - Consturctors

        #region Function - Private Methods

        /// <summary>
        /// Socket初始化
        /// </summary>
        protected override void SocketInitial() {
            mSocket.EnableBroadcast = true;
            mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
        }

        /// <summary>
        /// 設定要廣播的埠號
        /// </summary>
        /// <param name="port">要廣播的埠號</param>
        protected override void SetBroadcastPort(int port) {
            base.SetBroadcastPort(port);
            // 取得本機名稱
            string strHostName = Dns.GetHostName();
            // 取得本機的IpHostEntry類別實體，MSDN建議新的用法
            IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

            // 取得所有 IP 位址
            mRemote.Clear();
            foreach (IPAddress ipaddress in iphostentry.AddressList) {
                // 只取得IP V4的Address
                if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                    string[] ip = ipaddress.ToString().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    ip[3] = "255";//最後一位設為255表示從0廣播到255
                    mRemote.Add(new IPEndPoint(IPAddress.Parse(string.Join(".", ip)), mLocal.Port));
                }
            }
        }

        #endregion Function - Private Methods

        #region Funciton - Public Methods

        /// <summary>
        /// 發送廣播
        /// </summary>
        /// <param name="msg">廣播訊息</param>
        public void Send(string msg) {
            lock (mKey) {
                foreach (var remote in mRemote) {
                    Send(msg, remote);
                }
            }
        }

        #endregion Funciton - Public Methods
    }
}