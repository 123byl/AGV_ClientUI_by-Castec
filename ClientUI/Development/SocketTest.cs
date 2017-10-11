using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using CtLib.Library;
using CtLib.Forms;
using ServerOperation;

namespace ClientUI
{

    public partial class SocketTest : Form {

        private bool mIsConnected = false;
        private IAgvClientSocket mCliect = new AgvClientSocket();
        private IAgvServerSocket mServer = new AgvServerSocket();
        private bool mIsServoOn = false;
        private bool mIsStart = false;
        private bool mIsGetCar = false;
        private string mDefMapDir = @"D:\MapInfo\";
        private double X {
            get {
                double x = 0D;
                return double.TryParse(txtX.Text, out x) ? x : 0D;
            }
        }

        private double Y {
            get {
                double y = 0D;
                return double.TryParse(txtY.Text, out y) ? y : 0D;
            }
        }

        private double Theta {
            get {
                double theta = 0D;
                return double.TryParse(txtTheta.Text, out theta) ? theta : 0D;
            }
        }

        private string RemoteIP {
            get {
                return txtIP.Text;
            }
        }

        private int Velocity {
            get {
                int velocity = 0;
                return int.TryParse(txtVelocity.Text, out velocity) ? velocity : 0;
            }
        }
        public SocketTest() {
            InitializeComponent();
        }

        private void btnListen_Click(object sender, EventArgs e) {
            if (mServer.IsListen) {
                mServer.StopListen();
            } else {
                //mServer.Listen();
            }
        }

        private void btnConnect_Click(object sender, EventArgs e) {

        }

        private  void btnConnectAgv_Click(object sender, EventArgs e) {
            ConnectAgv(btnConnectAgv, btnServoOnOff);
        }

        private void btnSetVelo_Click(object sender, EventArgs e) {
            mCliect.SetVolecity(Velocity);
        }

        private void btnServoOnOff_Click(object sender, EventArgs e) {
            mIsServoOn = !mIsServoOn;
            mCliect.ServoOn(mIsServoOn);
            btnServoOnOff.Text = mIsServoOn ? "ON" : "OFF";
            btnServoOnOff.BackColor = mIsServoOn ? System.Drawing. Color.Green : System.Drawing.Color.Red;
        }

        private void btnUp_Click(object sender, EventArgs e) {
            mCliect.Forward(Velocity);
        }

        private void btnDown_Click(object sender, EventArgs e) {
            mCliect.Backward(Velocity);
        }

        private void btnLeft_Click(object sender, EventArgs e) {
            mCliect.LeftTrun(Velocity);
        }

        private void btnRight_Click(object sender, EventArgs e) {
            mCliect.RightTrun(Velocity);
        }

        private void btnStartStop_Click(object sender, EventArgs e) {
            mIsStart = !mIsStart;
            Bitmap btnImg = null;
            if (mIsStart) {
                mCliect.Start();
                btnImg = Properties.Resources.Stop;
            } else {
                mCliect.Stop();
                btnImg = Properties.Resources.play;
            }
            btnStartStop.Image = btnImg;
        }

        private void btnMapMode_Click(object sender, EventArgs e) {
            mCliect.SetMode(CarMode.Map);
        }

        private void btnWorkMode_Click(object sender, EventArgs e) {
            mCliect.SetMode(CarMode.Work);
        }

        private void btnIdleMode_Click(object sender, EventArgs e) {
            mCliect.SetMode(CarMode.Idle);
        }

        private void btnGetOri_Click(object sender, EventArgs e) {
            GetFile(FileType.Ori);
        }

        private void btnGetMap_Click(object sender, EventArgs e) {
            GetFile(FileType.Map);
        }

        private void btnSendMap_Click(object sender, EventArgs e) {
            OpenFileDialog openMap = new OpenFileDialog();
            openMap.InitialDirectory = mDefMapDir;
            openMap.Filter = "MAP|*.ori;*.map";
            if (openMap.ShowDialog() == DialogResult.OK) {
                string fileName = CtFile.GetFileName(openMap.FileName);
                mCliect.SendMap(fileName);
            }

        }

        private void btnGetLaser_Click(object sender, EventArgs e) {
            mCliect.GetLaser();
        }

        private void btnGetCarStatus_Click(object sender, EventArgs e) {
            mIsGetCar = !mIsGetCar;
            mCliect.GetCar(mIsGetCar);
            btnGetCarStatus.BackColor = mIsGetCar ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        }

        private void btnPosConfirm_Click(object sender, EventArgs e) {
            mCliect.SetPos(X, Y, Theta);
        }

        private void GetFile(FileType type) {
            string fileList = mCliect.GetFileList(type);
            using (MapList f = new MapList(fileList)) {
                if (f.ShowDialog() == DialogResult.OK) {
                    CtProgress prog = new CtProgress($"Get {type}", $"Donwloading {type} from AGV");
                    try {
                        mCliect.GetFile(mDefMapDir, type, f.strMapList);
                    } catch {

                    } finally {
                        prog?.Close();
                        prog = null;
                    }
                    if (type == FileType.Map) {
                        //RaiseGoalSettingEvent(GoalSettingEventType.CurMapPath, true);
                    } else {
                        //RaiseTestingEvent(TestingEventType.CurOriPath, true);
                    }
                    //RaiseAgvClientEvent(AgvClientEventType.GetFile, type);
                }
            }
        }

        private void Progress(string title, string content, Action act) {
            CtProgress prog = new CtProgress(title, content);
            try {
                act();
            } catch {
            } finally {
                prog?.Close();
                prog = null;
            }
        }

        private void SocketTest_Load(object sender, EventArgs e) {
            mServer.Listen();

            mCliect.RemoteIP = "127.0.0.1";


        }

        protected virtual async void ConnectAgv(Button cnn, Button servo) {
            CtProgress prog = null;
            if (!mCliect.IsConnected) {
                prog = new CtProgress("Connecting", "Connecting to AGV...");
                try {

                } catch {

                } finally {
                    prog?.Close();
                    prog = null;
                }
            }else {

            }
                try {
                bool isConnect = await Task.Run(() => mCliect.ConnectAsync(!mCliect.IsConnected));
                if (cnn != null) {
                    CtInvoke.ButtonText(cnn, isConnect ? "Connected" : "Disconnected");
                    CtInvoke.ButtonImage(cnn, isConnect ?
                        Properties.Resources.Connect :
                        Properties.Resources.Disconnect
                        );
                }
                if (isConnect) {
                    bool isServoOn = mCliect.GetMotorStatus();
                    if (servo != null) {
                        CtInvoke.ButtonText(btnServoOnOff, isServoOn ? "ON" : "OFF");
                        CtInvoke.ButtonBackColor(btnServoOnOff, isServoOn ? System.Drawing.Color.Green : System.Drawing.Color.Red);
                    }
                }
            } catch (Exception ex) {
                CtMsgBox.Show("Error", ex.Message);
            } finally {
                prog?.Close();
                prog = null;
            }
        }
    }

    public enum AgvPort {
        /// <summary>
        /// Client對Server發送Command的Port
        /// </summary>
        Cmd = 400,
        /// <summary>
        /// Client接收Car or Laser等資料所用，常駐型
        /// </summary>
        Data = 800,
        /// <summary>
        /// Client從Server接收Map、Ori等檔案所用，常駐型
        /// </summary>
        GetFile = 600,
        /// <summary>
        /// Client傳送Map、Ori等檔案所用，
        /// </summary>
        SendFile = 700,
        /// <summary>
        /// Client從Server接收路徑規劃結果
        /// </summary>
        Path = 900

    }

    public interface IAgvServerSocket {
        bool IsListen { get; }
        string HostIP { get; }

        void Listen();
        void StopListen();
    }

    public interface IAgvClientSocket {
        string RemoteIP { get; set; }
        bool IsConnected { get; set; }

        bool ConnectAsync(bool cnn);

        /// <summary>
        /// 向Server端要求檔案
        /// </summary>
        /// <param name="type">檔案類型</param>
        string GetFileList(FileType type);
        bool CheckServerIsAlive();
        void SetVolecity(int volecity);
        void ServoOn(bool isOn);
        void Forward(int velocity);
        void Backward(int velocity);
        void LeftTrun(int velocity);
        void RightTrun(int velocity);
        void Start();
        void Stop();
        void GetFile(string savePath, FileType type, string fileName);
        void SendMap(string fileName);
        IEnumerable<int> GetLaser();
        void GetCar(bool isGet);
        void SetPos(double x, double y, double theta);
        void SetMode(CarMode mode);
        /// <summary>
        /// 取得伺服馬達激磁狀態
        /// </summary>
        /// <returns></returns>
        bool GetMotorStatus();

    }

    public static class SocketOperator {
        public static Socket ServerListen(int port) {
            IPEndPoint recvCmdLocalEndPoint = new IPEndPoint(IPAddress.Any, port);
            Socket sox = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sox.Bind(recvCmdLocalEndPoint);
            sox.Listen(1);
            return sox;
        }

        public static Socket ClientAccept(this Socket sox, int receiveTimeOut = 5000, int sendTimeout = 5000, int sendBuffer = 8192, int receiveBuffer = 1024) {
            Socket client = sox.Accept();
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, receiveTimeOut);
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, sendTimeout);
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, sendBuffer);
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, receiveBuffer);
            return client;
        }
    }

    public class AgvServerSocket : IAgvServerSocket {

        private Socket mSoxCmd = null;

        private Thread mTdCmd;
        public string HostIP {
            get {
                throw new NotImplementedException();
            }
        }

        public bool IsListen {
            get {
                throw new NotImplementedException();
            }
        }

        public void Listen() {
            mSoxCmd = SocketOperator.ServerListen((int)AgvPort.Cmd);
            CtThread.CreateThread(ref mTdCmd, "CmdRecv", tsk_CmdHandler);
        }

        private void tsk_CmdHandler() {
            while (true) {
                Socket sRecvCmdTemp = mSoxCmd.ClientAccept();

                //Create buffer to store received information
                byte[] recvBytes = new byte[1024];
                sRecvCmdTemp.Receive(recvBytes);

                //Decode byte array to string                
                string strRecvCmd = Encoding.Default.GetString(recvBytes);

                strRecvCmd = strRecvCmd.Split(new char[] { '\0' })[0];
                //CtInvoke.TextBoxText(txtConsoleInfo, "[Server] : " + strRecvCmd + "\r\n", true, true);
                string[] strArray = strRecvCmd.Split(':');

                SwitchAction(sRecvCmdTemp, strArray);
            }
        }

        private void SwitchAction(Socket sox, string[] strArray) {
            switch (strArray[0]) {
                case "Set":

                    break;
                case "Get":
                    SwitchGetObject(sox, strArray);
                    break;
                case "Send":

                    break;
            }
        }

        private void SwitchGetObject(Socket sox ,string[] strArray) {
            switch (strArray[1]) {
                case "Hello":
                    sox.Send(Encoding.UTF8.GetBytes("Get:Hello:True"));
                    break;
            }
        }

        public void StopListen() {
            throw new NotImplementedException();
        }
    }

        public class AgvClientSocket : IAgvClientSocket {

            #region Declaration - Fields

            private Socket mSoxCmd = null;

            private Socket mSoxFile = null;

            private Socket mSoxPath = null;

            private readonly int mRecvCmdPort = (int)AgvPort.Cmd;

            private bool mBypassSocket = false;

            private string mRemoteIP = "127.0.0.1";

            private CtProgress mProg = null;

            /// <summary>
            /// Socket通訊物件
            /// </summary>
            private Communication serverComm = new Communication(400, 600, 800);

            /// <summary>
            /// 地圖檔儲存路徑
            /// </summary>
            private string mDefMapDir = @"D:\MapInfo\";

            private bool IsGettingLaser = false;

            private Thread mTdCarInfo;

            private bool mIsConnected = false;

            #endregion Declaration - Files

            #region Declaration - Properties

            public bool IsServerAlive { get; private set; }

            public bool IsConnected {get;set;}

            public string RemoteIP {
                get {
                    return mRemoteIP;
                }

                set {
                    mRemoteIP = value;
                }
            }

            #endregion Declaration - Properties

            #region Function - Public Methods

            /// <summary>
            /// 取得伺服馬達激磁狀態
            /// </summary>
            /// <returns></returns>
            public bool GetMotorStatus() {
                string[] rtnMsg = SendMsg("Get:Info");
                return rtnMsg.Count() > 1 && rtnMsg[1] == "True";
            }

            /// <summary>
            /// 向Server端要求檔案
            /// </summary>
            /// <param name="type">檔案類型</param>
            public string GetFileList(FileType type) {
                return mBypassSocket ?
                    $"{type}1,{type}2,{type}3" :
                    string.Join(",", SendMsg($"Get:{type}List"));
            }

            public async void GetFile(string savePath, FileType type, string fileName) {
                Task tskRecv = Task.Factory.StartNew(() => {
                    RecvFiles(savePath);
                });
                /*-- 向Server端發出檔案請求 --*/
                SendMsg($"Get:{type}:{fileName}");
                await tskRecv;
            }

            public void Backward(int velocity) {
                SendMsg($"Set:DriveVelo:-{velocity}:-{velocity}");
                Start();
            }

            public bool ConnectAsync(bool cnn) {
                if (cnn) {
                    mIsConnected = "True" == SendMsg("Get:Hello", false)[0];
                }

                return mIsConnected;
            }

            public bool CheckServerIsAlive() {
                string[] rtnMsg =  SendMsg("Get:Hello",false);
                return IsServerAlive = rtnMsg[0] == "True";
            }

            public void Forward(int velocity) {
                SendMsg($"Set:DriveVelo:{velocity}:{velocity}");
                Start();
            }

            public void GetCar(bool isGet) {
                IsGettingLaser = isGet;
                if (isGet) {
                    CtThread.CreateThread(ref mTdCarInfo, "", tsk_RecvCmd);
                } else {
                    SendMsg("Get:Car:False");
                }
            }

            public IEnumerable<int> GetLaser() {
                /*-- 若是雷射資料則更新資料 --*/
                string[] strRemoteEndPoint = SendMsg("Get:Laser");
                IEnumerable<int> LaserData = null;
                if (strRemoteEndPoint.Length > 1) {
                    if (strRemoteEndPoint[1] == "Laser") {
                        string[] sreRemoteLaser = strRemoteEndPoint[3].Split(',');
                        LaserData = sreRemoteLaser.Select(x => int.Parse(x));
                    }
                }
                //RaiseMapEvent(MapEventType.GetLaser, mCarInfo.LaserData);
                return LaserData;
            }

            public void LeftTrun(int velocity) {
                SendMsg($"Set:DriveVelo:{velocity}:-{velocity}");
                Start();
            }

            public void SetMode(CarMode mode) {
                SendMsg($"Set:Mode:{mode}");
            }

            public void RightTrun(int velocity) {
                SendMsg($"Set:DriveVelo:-{velocity}:{velocity}");
                Start();

            }

            public void SendMap(string fileName) {
                if (!mBypassSocket) {
                    SendFile(RemoteIP, (int)AgvPort.SendFile, fileName);
                } else {
                    /*-- 空跑模擬檔案傳送中 --*/
                    SpinWait.SpinUntil(() => false, 1000);
                }
                //RaiseAgvClientEvent(AgvClientEventType.SendFile, fileName);

            }

            public void ServoOn(bool isOn) {
                SendMsg($"Set:Servo{(isOn ? "On" : "Off")}");
            }

            public void SetPos(double x, double y, double theta) {
                throw new NotImplementedException();
            }

            public void SetVolecity(int volecity) {
                SendMsg($"Set:WorkVelo:{volecity}:{ volecity}");
            }

            public void Start() {
                SendMsg("Set:Start");
            }

            public void Stop() {
                SendMsg("Set:Stop");
            }

            public void Disconnect() {
                throw new NotImplementedException();
            }

            #endregion Function - Public Methods

            #region Function - Private Methods

            /// <summary>
            /// 啟動
            /// </summary>
            private void StartUp() {
                Listen(mSoxCmd, (int)AgvPort.Data);
                Listen(mSoxFile, (int)AgvPort.GetFile);
                Listen(mSoxPath, (int)AgvPort.Path);
            }

            /// <summary>
            /// 使用指定<see cref="Socket"/>監聽指定Port
            /// </summary>
            /// <param name="sox">用來監聽的<see cref="Socket"/></param>
            /// <param name="port">要監聽的Port</param>
            private void Listen(Socket sox, int port) {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
                sox.Bind(endPoint);
                sox.Listen(10);
            }

            /// <summary>
            /// 訊息傳送(會觸發事件)
            /// </summary>
            /// <param name="sendMseeage">傳送訊息內容</param>
            /// <param name="ckCnn">是否檢查連線狀態</param>
            /// <returns>Server端回應</returns>
            private string[] SendMsg(string sendMseeage, bool ckCnn = true) {
                if (mBypassSocket) {
                    return new string[] { "True" };
                } else if (ckCnn && !IsServerAlive) {
                    return new string[] { "False" };
                }

                /*-- 顯示發送出去的訊息 --*/
                string msg = $"{DateTime.Now} [Client] : {sendMseeage}\r\n";
                //RaiseMsgTrans(msg);

                /*-- 等待Server端的回應 --*/
                string rtnMsg = SendStrMsg(RemoteIP, mRecvCmdPort, sendMseeage);

                /*-- 顯示Server端回應 --*/
                msg = $"{DateTime.Now} [Server] : {rtnMsg}\r\n";
                //RaiseMsgTrans(msg);

                return rtnMsg.Split(':');

            }

            /// <summary>
            /// 訊息傳送(具體Socket交握實現，但是不會觸發事件)
            /// </summary>
            /// <param name="remoteIP">伺服端IP</param>
            /// <param name="requerPort">通訊埠號</param>
            /// <param name="sendMseeage">傳送訊息內容</param>
            /// <returns>Server端回應</returns>
            private string SendStrMsg(string remoteIP, int requerPort, string sendMseeage) {

                //可以在字串編碼上做文章，可以傳送各種資訊內容，目前主要有三種編碼方式：
                //1.自訂連接字串編碼－－微量
                //2.JSON編碼--輕量
                //3.XML編碼--重量
                int state;
                int timeout = 5000;
                byte[] recvBytes = new byte[8192];//開啟一個緩衝區，存儲接收到的資訊

                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(remoteIP.ToString()), requerPort);
                Socket answerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                answerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
                answerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, timeout);
                try {

                    answerSocket.Connect(ipEndPoint);//建立Socket連接
                    byte[] sendContents = Encoding.UTF8.GetBytes(sendMseeage);
                    state = answerSocket.Send(sendContents, sendContents.Length, 0);//發送二進位資料
                    state = answerSocket.Receive(recvBytes);
                    string strRecvCmd = Encoding.Default.GetString(recvBytes);//
                    strRecvCmd = strRecvCmd.Split(new char[] { '\0' })[0];
                    sendContents = null;
                    return strRecvCmd;

                } catch (SocketException se) {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                    //MessageBox.Show("目標拒絕連線!!");
                    return "False";
                } catch (ArgumentNullException ane) {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                    return "False";
                } catch (Exception ex) {
                    Console.Write(ex.Message);
                    return "False";
                } finally {
                    ipEndPoint = null;
                    recvBytes = null;
                    // answerSocket.Shutdown(SocketShutdown.Both);
                    // answerSocket.Disconnect(false);
                    answerSocket.Close();
                    // Console.Write("Disconnecting from server...\n");
                    //Console.ReadKey();
                    answerSocket.Dispose();
                }

            }

            /// <summary>
            /// 檔案接收執行緒 20170911
            /// </summary>
            /// <param name="obj"></param>
            private void RecvFiles(string savePath) {

                int fileNameLen = 0;
                int recieve_data_size = 0;
                int first = 1;
                int receivedBytesLen = 0;
                double cal_size = 0;
                Socket clientSock = null;
                BinaryWriter bWrite = null;
                //MemoryStream ms = null;
                string curMsg = "Stopped";
                string fileName = "";
                try {
                    if (!mBypassSocket) {
                        clientSock = serverComm.ClientAccept(mSoxFile);
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
                                if (!Directory.Exists(savePath)) {
                                    Directory.CreateDirectory(savePath);
                                }
                                if (File.Exists($"{savePath}/{fileName}")) {
                                    File.Delete($"{savePath}/{fileName}");
                                }
                                bWrite = new BinaryWriter(File.Open($"{savePath}/{fileName}", FileMode.OpenOrCreate));
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


                } catch (SocketException se) {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                    MessageBox.Show("檔案傳輸失敗!");
                    curMsg = "File Receiving error.";
                } catch (Exception ex) {
                    Console.WriteLine("[RecvFiles]" + ex.ToString());
                    curMsg = "File Receiving error.";
                } finally {
                    bWrite?.Close();
                    clientSock?.Shutdown(SocketShutdown.Both);
                    clientSock?.Close();
                    clientSock = null;
                    mProg?.Close();
                    mProg = null;
                }
            }

            /// <summary>
            /// Send file of server to client
            /// </summary>
            /// <param name="clientIP">Ip address of client</param>
            /// <param name="clientPort">Communication port</param>
            /// <param name="fileName">File name</param>
            /// 
            public void SendFile(string clientIP, int clientPort, string fileName) {
                string curMsg = "";
                try {
                    IPAddress[] ipAddress = Dns.GetHostAddresses(clientIP);
                    IPEndPoint ipEnd = new IPEndPoint(ipAddress[0], clientPort);
                    /* Make IP end point same as Server. */
                    Socket clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    /* Make a client socket to send data to server. */
                    string filePath = mDefMapDir;
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

            /// <summary>
            /// 車子資訊接收執行緒
            /// </summary>
            public void tsk_RecvCmd(object obj) {
                Socket sRecvCmdTemp = null;
                CarInfo carinfo;
                //Socket sRecvCmdTemp = sRecvCmd.Accept();//Accept 以同步方式從偵聽通訊端的連接請求佇列中提取第一個掛起的連接請求，然後創建並返回新的 Socket
                //sRecvCmdTemp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
                //sRecvCmdTemp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);//設置發送資料超時
                //sRecvCmdTemp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 1024); //設置發送緩衝區大小 1K
                //sRecvCmdTemp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 9000);//設置接收緩衝區大小1K

                try {
                    while (IsGettingLaser) {
                        sRecvCmdTemp = serverComm.ClientAccept(mSoxFile);

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
                        if (CarInfo.TryParse(strRecvCmd, out carinfo)) {
                            //RaiseCarInfoRefresh(mCarInfo);
                            sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Get:Car:True"));
                        } else {
                            sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Get:Car:False"));
                        }

                        strRecvCmd = null;
                        strArray = null;
                    }
                } catch (SocketException se) {
                    Console.WriteLine("[Status Recv] : " + se.ToString());
                    MessageBox.Show("目標拒絕連線");
                } catch (Exception ex) {
                    Console.Write(ex.Message);
                    //throw ex;
                } finally {
                    sRecvCmdTemp?.Close();
                    sRecvCmdTemp = null;
                    sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Get:Car:False"));
                }
            }

            #endregion Function - Private Methods


        }

    }
