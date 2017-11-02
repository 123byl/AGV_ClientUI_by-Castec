using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommandCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapProcessing;
using System.Threading;
using System.IO;

namespace CommandCore.Tests {

    [TestClass()]
    public class CtSoxCmdClientTests {

        #region Funciton - Test Methods

        /// <summary>
        /// 建構測試，是否能於建構時指定ServerIP
        /// </summary>
        [TestMethod]
        public void ConstructorTest() {
            string remoteIP = "127.0.0.0";
            ISoxCmdClient client = TestFactory.CommandCore.Client(remoteIP);
            Assert.IsTrue(client.ServerIP == remoteIP);
        }

        /// <summary>
        /// 測試是否可以建立連線
        /// </summary>
        [TestMethod()]
        public void ConnectTest() {
            Connect((client, server) => {
                Assert.IsTrue(server.IsListening);
                Assert.IsTrue(client.IsConnected);
                Assert.IsTrue(client.IsServerAlive);
            });
        }

        /// <summary>
        /// 斷線測試
        /// </summary>
        [TestMethod()]
        public void DisconnectTest() {
            Connect((client, server) => {
                client.Disconnect();
                Assert.IsTrue(!client.IsConnected);
                Assert.IsTrue(server.IsListening);
            });
        }

        /// <summary>
        /// Ping指令測試
        /// </summary>
        [TestMethod()]
        public void PingTest() {
            bool isAlive = false;
            Connect((client, server) => {
                client.ConsoleRefresh += msg => { isAlive = true; };
                client.Ping();
                Assert.IsTrue(isAlive);
            });
        }

        /// <summary>
        /// Mode設定指令測試
        /// </summary>
        [TestMethod()]
        public void SetModeTest() {
            Connect((client, server) => {
                server.RefAGV = new EmulationAGV();
                ModeTest(client, server, WorkMode.Idle);
                ModeTest(client, server, WorkMode.Map);
                ModeTest(client, server, WorkMode.Work);
                //foreach(WorkMode mode in Enum.GetValues(typeof(WorkMode))) {
                //    ModeTest(client, server, mode);
                //}
            });
        }

        /// <summary>
        /// ServoOn狀態設定命令測試
        /// </summary>
        [TestMethod()]
        public void SetServoOnTest() {
            Connect((client, server) => {
                server.RefAGV = new EmulationAGV();

                ServoOnTest(client, server, true);
                ServoOnTest(client, server, false);
            });
        }

        /// <summary>
        /// 開始/停止移動測試
        /// </summary>
        [TestMethod]
        public void SetStartMoveTest() {
            Connect((client, server) => {
                server.RefAGV = new EmulationAGV();
                Assert.IsTrue(client.StartMove());
                Assert.IsTrue(client.StopMove());
            });
        }

        /// <summary>
        /// 取得Goal點清單測試
        /// </summary>
        [TestMethod]
        public void GetGoalNamesTest() {
            Connect((client, server) => {
                GoalNamesTest(client, server, "Goal1,Goal2,Goal3");
                GoalNamesTest(client, server, null);
                GoalNamesTest(client, server, string.Empty);
            });
        }

        /// <summary>
        /// 取得Ori檔名清單測試
        /// </summary>
        [TestMethod]
        public void GetOriNamesTest() {
            Connect((client, server) => {
                OriListTest(client, server, "Ori1,Ori2,Ori3");
                OriListTest(client, server, null);
                OriListTest(client, server, string.Empty);
            });
        }

        /// <summary>
        /// 取得Map檔名清單測試
        /// </summary>
        [TestMethod]
        public void GetMapNamesTest() {
            Connect((client, server) => {
                MapListTest(client, server, "Map1,Map2,Map3");
                MapListTest(client, server, null);
                MapListTest(client, server, string.Empty);
            });
        }

        /// <summary>
        /// 取得雷射資料測試
        /// </summary>
        [TestMethod]
        public void GetLaserTest() {
            Connect((client, server) => {
                LaserTest(client, server, 1, 2, 3, 4, 5, 6);
                LaserTest(client, server);
            });
        }

        /// <summary>
        /// Ori檔名設定測試
        /// </summary>
        [TestMethod]
        public void SetOriNameTest() {
            Connect((client, server) => {
                OriNameTest(client, server, "TestOriName");
                OriNameTest(client, server);
            });
        }

        /// <summary>
        /// 設定AGV使用Map檔指令設定測試
        /// </summary>
        [TestMethod]
        public void SetOrderMapTest() {
            Connect((client, server) => {
                OrderMapTest(client, server, "TestMapName");
                OrderMapTest(client, server);
            });
        }

        /// <summary>
        /// 前進測試
        /// </summary>
        [TestMethod]
        public void SetForwardTest() {
            Connect((client, server) => {
                ForwardTest(client, server, 50);
            });
        }

        /// <summary>
        /// 後退測試
        /// </summary>
        [TestMethod]
        public void SetBakcwardTest() {
            Connect((client, server) => {
                BackwardTest(client, server, 20);
            });
        }

        /// <summary>
        /// 左轉測試
        /// </summary>
        [TestMethod]
        public void SetLeftTurnTest() {
            Connect((client, server) => {
                LeftTurnTest(client, server, 10);
            });
        }

        /// <summary>
        /// 右轉測試
        /// </summary>
        [TestMethod]
        public void SetRightTurnTest() {
            Connect((client, server) => {
                RightTurnTest(client, server, -10);
            });
        }

        /// <summary>
        /// ServOn取得指令測試
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="servoOn"></param>
        [TestMethod]
        public void GetServoOnTest() {
            Connect((client, server) => {
                ServoOnTest(client, server, true);
                ServoOnTest(client, server, false);
            });
        }

        /// <summary>
        /// 速度設定測試
        /// </summary>
        [TestMethod]
        public void SetWorkVelocityTest() {
            Connect((client, server) => {
                WorkVelocity(client, server, 10);
            });
        }

        /// <summary>
        /// 啟動車子資訊回傳測試
        /// </summary>
        [TestMethod]
        public void EnableCarInfTest() {
            Connect((client, server) => {
                CarinfoTest(client, server, 12, 2, 5, 6, 7, new List<int>() { 4, 5, 6 }, WorkMode.Idle);
                client.EnableCarInfoReturn(false);
                Assert.IsTrue(!client.IsGettingLaser, "Client端未關閉收值");
            });
        }

        /// <summary>
        /// 路徑規劃取得測試
        /// </summary>
        [TestMethod]
        public void GetPathTest() {
            Connect((client, server) => {
                bool isReturn = false;
                bool isPaln = false;
                List<CartesianPos> path = new List<CartesianPos>() {
                    new CartesianPos(0,0),
                    new CartesianPos(4,5),
                    new CartesianPos(5,9)
                };
                server.RefAGV = new EmulationAGV();
                server.RefAGV.WorkPath = path;
                server.IdxToGoal = IdxToGoal;
                server.PathPlan = goal => {
                    server.RefAGV.f_pathSearching = true;
                    SpinWait.SpinUntil(() => false, 100);//模擬路徑規劃
                    server.RefAGV.f_pathSearching = false;
                    isPaln = true;
                };///模擬Goal點路徑搜尋方法
                List<CartesianPos> recvPath = null;
                Action<List<CartesianPos>> act = p => {

                    isReturn = true;
                    recvPath = p;
                };
                CommandEvents.Client.DelPathRefresh evn = new CommandEvents.Client.DelPathRefresh(act);

                client.PathRefresh += evn;
                client.RequirePath(0);

                SpinWait.SpinUntil(() => server.RefAGV.f_pathSearching && isReturn, 1000);
                //client.PathRefresh -= evn;
                Assert.IsTrue(isReturn, "事件未觸發");
                Assert.IsTrue(isPaln, "未規畫路徑");
                Assert.IsTrue((path?.Count() ?? 0) == (recvPath?.Count() ?? 0), "路徑點位數量不符");
                for (int i = 0; i < path.Count(); i++) {
                    Assert.IsTrue(path[i].x == recvPath[i].x, "路徑點位不符");
                    Assert.IsTrue(path[i].y == recvPath[i].y, "路徑點位不符");
                    Assert.IsTrue(path[i].theta == recvPath[i].theta, "路徑點位不符");
                }
            });
        }

        /// <summary>
        /// 要求檔案測試
        /// </summary>
        [TestMethod]
        public void RequireFileTest() {
            GetFileTest(FileType.Map, "TestMap");
            GetFileTest(FileType.Map, "Test2");
            GetFileTest(FileType.Ori, "TestOri");
            GetFileTest(FileType.Ori, "Test3");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void EnabledFileRecvTest() {
            SendFile(FileType.Map, "MapFile");
            SendFile(FileType.Map, "MapTestFile");
            SendFile(FileType.Ori, "OriFile");
            SendFile(FileType.Ori, "OriTestFile");
        }

        #endregion Function - Test Methods

        #region Function - Private Methods

        private void SendFile(FileType type, string fileName) {
            Connect((client, server) => {
                string fullName = fileName + "." + type.ToString().ToLower();
                string srcFile = DirPath.Client[type] + "\\" + fullName;
                string destFile = DirPath.Server[type] + "\\" + fullName;
                /*-- 檔案存放路徑設定 --*/
                client.DefDirectory[type] = DirPath.Client[type];
                server.GetMapDirectory = () => DirPath.Server[type];
                server.GetOriDirectory = () => DirPath.Server[type];
                if (Directory.Exists(DirPath.Dir)) Directory.Delete(DirPath.Dir, true);
                Directory.CreateDirectory(client.DefDirectory[type]);
                Directory.CreateDirectory(server.GetMapDirectory());
                /*-- 產生測試檔案 --*/
                File.Create(srcFile).Close();
                File.WriteAllText(srcFile, srcFile);

                /*-- 事件委派 --*/
                bool isRaised = false;
                string recvFileName = null;
                Action<string> act = name => {
                    isRaised = true;
                    recvFileName = name;
                };
                CommandEvents.Client.DelUploadFinished evn = new CommandEvents.Client.DelUploadFinished(act);
                client.UploadFinished += evn;

                /*-- 檔案要求 --*/
                client.EnableFileRecive(type, DirPath.Client[type], fullName);
                SpinWait.SpinUntil(() => isRaised, 1000);

                /*-- 檢查檔案是否正確 --*/
                Assert.IsTrue(isRaised, "事件未觸發");

                SpinWait.SpinUntil(() => FileMth.IsIdleFile(fullName), 1000);
                Assert.IsTrue(File.Exists(destFile), "要求的檔案未取得");
                Assert.IsTrue(File.ReadAllText(srcFile) == srcFile, "檔案內容不符");
                Assert.IsTrue(recvFileName == fullName, "要求檔名不符");
                /*-- 刪除測試資料 --*/
                Directory.Delete(DirPath.Dir, true);
                client.UploadFinished -= evn;
            });
        }

        /// <summary>
        /// 要求檔案測試
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        private void GetFileTest(FileType type, string fileName) {
            Connect((client, server) => {
                string fullName = fileName + "." + type.ToString().ToLower();
                string srcFile = DirPath.Server[type] + "\\" + fullName;
                string destFile = DirPath.Client[type] + "\\" + fullName;
                /*--檔案存放路徑設定--*/
                client.DefDirectory[type] = DirPath.Client[type];
                server.GetMapDirectory = () => DirPath.Server[type];
                server.GetOriDirectory = () => DirPath.Server[type];
                if (Directory.Exists(DirPath.Dir)) Directory.Delete(DirPath.Dir, true);
                Directory.CreateDirectory(client.DefDirectory[type]);
                Directory.CreateDirectory(server.GetMapDirectory());
                /*-- 產生測試檔案 --*/
                File.Create(srcFile).Close();
                File.WriteAllText(srcFile, srcFile);

                /*-- 事件委派 --*/
                bool isRaised = false;
                string recvFileName = null;
                Action<string> act = name => {
                    isRaised = true;
                    recvFileName = name;
                };
                CommandEvents.Client.DelFileDownload evn = new CommandEvents.Client.DelFileDownload(act);
                client.FileDownload += evn;

                /*-- 檔案要求 --*/
                client.RequireFile(type, fileName);
                SpinWait.SpinUntil(() => isRaised, 1000);

                /*-- 檢查檔案是否正確 --*/
                Assert.IsTrue(isRaised, "事件未觸發");
                Assert.IsTrue(File.Exists(destFile), "要求的檔案未取得");
                Assert.IsTrue(File.ReadAllText(srcFile) == srcFile, "檔案內容不符");
                Assert.IsTrue(recvFileName == fullName, "要求檔名不符");
                /*-- 刪除測試資料 --*/
                Directory.Delete(DirPath.Dir, true);
                client.FileDownload -= evn;
            });
        }

        /// <summary>
        /// 車子資訊要求測試
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="theta"></param>
        /// <param name="power"></param>
        /// <param name="laser"></param>
        /// <param name="mode"></param>
        private void CarinfoTest(
            IClientCommand client,
            IServerCommand server,
            int id,
            int x, int y, double theta,
            int power,
            List<int> laser,
            WorkMode mode) {
            bool isReturn = false;
            CarInfo carInfo = new CarInfo(id, x, y, theta, power, laser, mode);
            CarInfo recvInfo = null;
            Action<CarInfo> act = info => {
                isReturn = true;
                recvInfo = info;
            };
            CommandEvents.Client.DelCarInfoRefresh evn = new CommandEvents.Client.DelCarInfoRefresh(act);
            client.CarInfoRefresh += evn;
            server.RefAGV = new EmulationAGV();
            server.GetCarID = () => id;
            server.GetPower = () => power;
            server.RefAGV.Position = new double[] { x, y, theta };
            server.RefAGV.LaserDistanceData = laser;
            server.RefAGV.mode = mode;
            client.EnableCarInfoReturn(true);
            SpinWait.SpinUntil(() => isReturn, 1000);
            //client.CarInfoRefresh -= evn;
            Assert.IsTrue(isReturn, "事件未觸發");
            Assert.IsTrue(recvInfo.CarID == id, "ID不符");
            Assert.IsTrue(recvInfo.X == x, "X不符");
            Assert.IsTrue(recvInfo.Y == y, "Y不符");
            Assert.IsTrue(recvInfo.Theta == theta, "Theta不符");
            Assert.IsTrue(recvInfo.PowerPercent == power, "Power不符");
            Assert.IsTrue(recvInfo.Mode == mode, "Mode不符");
            for (int i = 0; i < laser.Count(); i++) {
                Assert.IsTrue(recvInfo.LaserData[i] == laser[i], "雷射資料不符");
            }
            Assert.IsTrue(client.IsGettingLaser, "Client端未開啟收值");
        }

        /// <summary>
        /// 工作速度設定測試
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="setVelo"></param>
        private void WorkVelocity(IClientCommand client, IServerCommand server, int setVelo) {
            server.RefAGV = new EmulationAGV();
            client.SetVelocity(setVelo);
            int recvVelo = server.RefAGV.DriveSpeed;
            Assert.IsTrue(setVelo == recvVelo, ErrMsg(setVelo, recvVelo));
        }

        /// <summary>
        /// 右轉測試
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="setVelo"></param>
        private void RightTurnTest(IClientCommand client, IServerCommand server, int setVelo) {
            server.RefAGV = new EmulationAGV();
            client.RightTurn(setVelo);
            CheckMotorVelocity(server.RefAGV.MotorVelocity, -setVelo, setVelo);
        }

        /// <summary>
        /// 左轉測試
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="setVelo"></param>
        private void LeftTurnTest(IClientCommand client, IServerCommand server, int setVelo) {
            server.RefAGV = new EmulationAGV();
            client.LeftTurn(setVelo);
            CheckMotorVelocity(server.RefAGV.MotorVelocity, setVelo, -setVelo);
        }

        /// <summary>
        /// 後退測試
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="setVelo"></param>
        private void BackwardTest(IClientCommand client, IServerCommand server, int setVelo) {
            server.RefAGV = new EmulationAGV();
            client.Backward(setVelo);
            CheckMotorVelocity(server.RefAGV.MotorVelocity, -setVelo, -setVelo);
        }

        /// <summary>
        /// 前進測試
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="setVelo"></param>
        private void ForwardTest(IClientCommand client, IServerCommand server, int setVelo) {
            server.RefAGV = new EmulationAGV();
            client.Forward(setVelo);
            CheckMotorVelocity(server.RefAGV.MotorVelocity, setVelo, setVelo);
        }

        /// <summary>
        /// 輪速檢查
        /// </summary>
        /// <param name="velo"></param>
        /// <param name="lVelo"></param>
        /// <param name="rVelo"></param>
        private void CheckMotorVelocity(int[] velo, int lVelo, int rVelo) {
            Assert.IsTrue(velo?.Count() == 2, "馬達速度無法讀取");
            Assert.IsTrue(velo[0] == lVelo, "左輪速度不符");
            Assert.IsTrue(velo[1] == rVelo, "右輪速度不符");
        }

        public void IsServoOnTest(IClientCommand client, IServerCommand server, bool servoOn) {
            bool isServoOn = client.IsServoOn();
        }

        /// <summary>
        /// ServoOn設定測試
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="servoOn"></param>
        private void ServoOnTest(IClientCommand client, ISoxCmdServer server, bool servoOn) {
            server.RefAGV = new EmulationAGV();
            Assert.IsTrue(client.SetMotor(servoOn), "馬達激磁設定失敗");
        }

        /// <summary>
        /// 設定AGV使用Map檔指令測試
        /// </summary>
        private void OrderMapTest(IClientCommand client, IServerCommand server, string setMapName = null) {
            string recvMapName = null;
            server.SetMapName = mapName => { recvMapName = mapName; return true; };
            client.OrderMap(setMapName);
            if (!string.IsNullOrEmpty(setMapName) || !string.IsNullOrEmpty(recvMapName)) {
                Assert.IsTrue(setMapName == recvMapName, ErrMsg(setMapName, recvMapName));
            }
        }

        /// <summary>
        /// 掃描圖檔名稱設定
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="setOriName"></param>
        private void OriNameTest(IClientCommand client, IServerCommand server, string setOriName = null) {
            string recvOriName = null;
            server.SetOriName = oriName => { recvOriName = oriName; };
            client.SetScanName(setOriName);
            if (!string.IsNullOrEmpty(setOriName) || !string.IsNullOrEmpty(recvOriName)) {
                Assert.IsTrue(setOriName == recvOriName, ErrMsg(setOriName, recvOriName));
            }
        }

        /// <summary>
        /// 取得雷射資料測試
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="laser"></param>
        private void LaserTest(IClientCommand client, IServerCommand server, params int[] laser) {
            server.RefAGV = new EmulationAGV();
            server.RefAGV.LaserDistanceData = laser?.ToList();
            List<int> recvLaser = client.GetLaser()?.ToList();
            int setCount = server.RefAGV.LaserDistanceData?.Count() ?? 0;
            int recvCount = recvLaser?.Count() ?? 0;
            if (setCount != 0 || recvCount != 0) {
                Assert.IsTrue(setCount == recvCount, $"雷射資料數量{setCount} 收到{recvCount}");
                for (int i = 0; i < setCount; i++) {
                    bool isSame = server.RefAGV.LaserDistanceData[i] == recvLaser[i];
                    Assert.IsTrue(isSame, "發送與接收的雷射資料不符");
                }
            }
        }

        /// <summary>
        /// Goal清單取得測試
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="goals"></param>
        private void GoalNamesTest(IClientCommand client, IServerCommand server, string goals) {
            server.GetGoalNames = (() => { return goals?.Split(Separator.Data)?.ToList(); });
            string recvGoalNames = client.GetGoalNames();
            if (!string.IsNullOrEmpty(goals) || !string.IsNullOrEmpty(recvGoalNames)) {
                Assert.IsTrue(recvGoalNames == goals, ErrMsg(goals, recvGoalNames));
            }
        }

        /// <summary>
        /// Ori檔名清單取得測試
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="oriList"></param>
        private void OriListTest(IClientCommand client, IServerCommand server, string oriList) {
            server.GetOriNames = (() => { return oriList?.Split(Separator.Data); });
            string recvOriList = client.GetFileNames(FileType.Ori);
            if (!string.IsNullOrEmpty(oriList) || !string.IsNullOrEmpty(recvOriList)) {
                Assert.IsTrue(recvOriList == oriList, ErrMsg(oriList, recvOriList));
            }
        }

        /// <summary>
        /// Map檔名清單取得測試
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="mapList"></param>
        private void MapListTest(IClientCommand client, IServerCommand server, string mapList) {
            server.GetMapNames = (() => { return mapList?.Split(Separator.Data); });
            string recvMapList = client.GetFileNames(FileType.Map);
            if (!string.IsNullOrEmpty(mapList) || !string.IsNullOrEmpty(recvMapList)) {
                Assert.IsTrue(recvMapList == mapList, ErrMsg(mapList, recvMapList));
            }
        }

        /// <summary>
        /// 連線環境配置下執行測試方法
        /// </summary>
        /// <param name="testAct"></param>
        private void Connect(Action<ISoxCmdClient, ISoxCmdServer> testAct) {
            ISoxCmdClient client = TestFactory.CommandCore.Client();
            ISoxCmdServer server = TestFactory.CommandCore.Server();
            try {
                server.Listen();
                client.Connect();
                client.Ping();
                testAct(client, server);
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            } finally {
                server.Dispose();
                client.Dispose();
                server = null;
                client = null;
            }
        }

        /// <summary>
        /// 車子模式設定指令測試
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="mode"></param>
        private void ModeTest(IClientCommand client, IServerCommand server, WorkMode mode) {
            client.SetCarMode(mode);
            Assert.IsTrue(server.RefAGV.mode == mode, ErrMsg(mode, server.RefAGV.mode));
        }

        /// <summary>
        /// ServOn取得指令測試
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="servoOn"></param>
        private void ServoOnTest(IClientCommand client, IServerCommand server, bool servoOn) {
            server.RefAGV.DriveConnectState = servoOn;
            bool isServoOn = client.IsServoOn();
            Assert.IsTrue(isServoOn == servoOn, ErrMsg(isServoOn, servoOn));
        }

        /// <summary>
        /// 格式化錯誤訊息
        /// </summary>
        /// <param name="set"></param>
        /// <param name="real"></param>
        /// <returns></returns>
        private string ErrMsg(object set, object real) {
            return $"設定值{set} 實際值{real}";
        }

        /// <summary>
        /// 模擬索引值查詢Goal點功能
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        private bool IdxToGoal(int idx, out CartesianPos goal) {
            goal = new CartesianPos(0, 0);
            return true;
        }

        /// <summary>
        /// 模擬Goal點名稱查詢Goal點
        /// </summary>
        /// <param name="name"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        private bool NameToGoal(string name, CartesianPos goal) {
            goal = new CartesianPos(0, 0);
            return true;
        }

        #endregion Funciton - Private Methods
    }

    /// <summary>
    /// 模擬用AGV類別
    /// </summary>
    internal class EmulationAGV : IAGV {
        public bool BrakeMode { get; set; }

        public bool DriveConnectState { get; set; } = true;

        public int DriveSpeed { get; set; }

        public long[] EncoderValue {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public bool f_pathSearching { get; set; } = false;

        public List<int> LaserDistanceData { get; set; } = new List<int>() { 1, 2, 3, 4, 5 };

        public WorkMode mode { get; set; }

        public int[] MotorAccel {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public int[] MotorDccel {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public int[] MotorVelocity { get; set; } = new int[2];//Left,Right

        public double[] Position { get; set; } = new double[3];

        public List<CartesianPos> WorkPath { get; set; }

        public void DoPathMovement() {
            throw new NotImplementedException();
        }

        public bool Drive() {
            return true;
        }

        public bool DriveOff() {
            return true;
        }

        public bool DriveOn() {
            return true;
        }

        public bool GetDriveVelocity(out int lVelo, out int rVelo) {
            lVelo = MotorVelocity[0];
            rVelo = MotorVelocity[1];
            return true;
        }

        public void GetMotorInfo(out bool[] info) {
            throw new NotImplementedException();
        }

        public bool SetAcceleration(int v) {
            throw new NotImplementedException();
        }

        public bool SetBrakeMode(bool v) {
            throw new NotImplementedException();
        }

        public bool SetDcceleration(int v) {
            throw new NotImplementedException();
        }

        public bool StopDrive() {
            return true;
        }
    }

    /// <summary>
    /// 集成工廠
    /// </summary>
    internal static class TestFactory {
        public static Factory CommandCore = CommandCoreFactory.GetInstance();
    }

    /// <summary>
    /// 測試用檔案存放路徑
    /// </summary>
    internal static class DirPath {
        public const string Dir = @"D:\TestMapInfo";
        public readonly static Dictionary<FileType, string> Server = new Dictionary<FileType, string>() {
            { FileType.Ori,Dir + @"\Server\Ori"},
            { FileType.Map,Dir + @"\Server\Map"}
        };
        public readonly static Dictionary<FileType, string> Client = new Dictionary<FileType, string>() {
            { FileType.Ori,Dir +@"\Client\Ori"},
            { FileType.Map,Dir +@"\Client\Map"}
        };
    }

}