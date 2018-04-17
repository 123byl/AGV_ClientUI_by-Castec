using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static MapGL.CastecMapUI;
using MapGL;
using MapProcessing;
using CtLib.Library;
using System.Net;
using System.Net.Sockets;
using AGVMathOperation;
using System.Threading;
using System.IO;
using System.Diagnostics;
using ServerOperation;
using CtLib.Forms;

namespace CAMPro {
    public partial class GUI : Form {
        Point openTabc;
        Point openMapUI;
        Size initialMapUI;

        //object dataModify;
        private Communication serverComm = new Communication(400, 600, 800);


        //預設主機IP
        Socket sRecvCmd;
        Socket sRecvFile;
        Socket sRecvPath;
        static string hostName = "P1";
        static string hostIP = "192.168.31.84";
        static int recvMapPort = 600;//發送圖片的埠
        static int sendMapPort = 700;//發送地圖的埠
        static int recvCmdPort = 400;//接收請求的埠開啟後就一直進行偵聽
        static int recvDataPort = 800;//接收請求的埠開啟後就一直進行偵聽
        static int recvPathPort = 900;
        public static string receivedPath = "D:\\MapInfo\\";
        public Thread mTdClient;
        public Thread mTdMap;
        public Thread mLoadOriginScanning = null;
        public Thread mLoadMap = null;
        public Thread mMapOperation = null;
        public Thread mRunGoal;

        int mVelocity = 0;

        public MapMatching mapMatch = new MapMatching();
        //public MapSimplication mapSimp = new MapSimplication("D:/MapInfo/Floor2.map");

        private bool bHotRun = false;

        // List<int> laserStrength;
        public double ThetaGyro;
        public double carX;
        public double carY;
        public int powerPercent;
        public string carID;
        public string curMsg;

        public bool bGetLaser;

        private List<Point> ObstaclePoint = new List<Point>();
        private List<CastecMapUI.MapLIne> ObstacleLine = new List<CastecMapUI.MapLIne>();
        private List<Point> obstacle = new List<Point>();
        private List<Point> ptCar = new List<Point>();
        private List<Point> DefineArea = new List<Point>();

        private string oriPath;
        private string oriMap;
        private string mapPath;
        private string SetMode = "";
        private List<string> StrCar = new List<string>();

        private int[] laserData = new int[1081];
        private int[] pos = new int[2];

        public GUI() {
            InitializeComponent();
        }

        private void GUI_Load(object sender, EventArgs e) {
            initialMapUI = MapUI1.Size;
            openTabc = btnPannel.Location;
            openMapUI = MapUI1.Location;
            MapUI1.PosCar = new Pos(0, 0, 0);
            MapUI1.Fit();
            int.TryParse(txtVelocity.Text, out mVelocity);
            SocketRecvOn();
            lbConnectGoal.Visible = false;
        }
        private void GUI_FormClosed(object sender, FormClosedEventArgs e) {
            if (bGetLaser) SendMsg(hostIP, recvCmdPort, "Get:Car:False");
            MapUI1.Dispose();

        }
        #region Mouse Event
        private void MapUI1_MouseClickRealPos(double posX, double posY) {
            txtAddPx.Text = posX.ToString();
            txtAddPy.Text = posY.ToString();
            List<Point> laserDataPoints = new List<Point>();
            int[] obstaclePos = new int[2];
            if (SetMode == "btnSetCar") {
                MapUI1.EnableCar = false;
                ptCar.Add(new Point(Convert.ToInt32(posX), Convert.ToInt32(posY)));
                StrCar.Add("CarTemp" + ptCar.Count.ToString());
                if (ptCar.Count == 2) {
                    #region - car pos setting -

                    double Calx = posX - ptCar[0].X;
                    double Caly = posY - ptCar[0].Y;
                    double Calt = Math.Atan2(Caly, Calx) * 180 / Math.PI;

                    //SetPosition(ptCar[0].X, ptCar[0].Y, Calt);
                    MapUI1.PosCar = new Pos((double)ptCar[0].X, (double)ptCar[0].Y, Calt);
                    carX = ptCar[0].X;
                    carY = ptCar[0].Y;
                    ThetaGyro = Calt;
                    //Send POS to AGV                    
                    SendMsg(hostIP, recvCmdPort, "Set:POS:" + ptCar[0].X + ":" + ptCar[0].Y + ":" + Calt);
                    MapUI1.RemoveGroupPoint(StrCar[0], StrCar[1]);

                    StrCar.Clear();
                    SetMode = "";
                    MapUI1.EnableCar = true;

                    double angle;
                    double Laserangle;
                    SendMsg(hostIP, recvCmdPort, "Get:Laser");
                    ObstaclePoint.Clear();
                    MapUI1.RemoveGroupPoint("LaserLength");
                    for (int i = 0; i < laserData.Length; i++) {
                        if ((laserData[i] >= 30) && (laserData[i] < 15000)) {
                            if (hostName == "P1")
                                pos = Transformation.LaserPoleToCartesian(laserData[i], -135, 0.25, i, -45, 320, -45, carX, carY, ThetaGyro, out angle, out Laserangle);//, out dataAngle, out laserAngle);
                            else
                                pos = Transformation.LaserPoleToCartesian(laserData[i], -135, 0.25, i, 43, 416.75, 43, carX, carY, ThetaGyro, out angle, out Laserangle);//, out dataAngle, out laserAngle);

                            MapUI1.DrawPoint(pos[0], pos[1], Color.Red, 2, 2, "LaserLength");
                            pos = null;
                        }
                    }

                    #endregion

                    ptCar.Clear();
                }
                else {
                    MapUI1.DrawPoint(Convert.ToInt32(posX), Convert.ToInt32(posY), Color.Blue, 3, 3, StrCar[ptCar.Count - 1]);
                }
            }
        }

        private void MapUI1_MouseSelectObj(string name, double x, double y) {
            string[] itemName = name.Split('_');

            // 地圖回傳誰被按下
            string msg = string.Format("Delete {0}({1},{2}) or not?", itemName[0], x, y);

            DialogResult dr = MessageBox.Show(msg, "Delete", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information);

            //判斷使用者是否要刪除點位
            if (dr == DialogResult.OK) {
                if (itemName[0] == CastecMapUI.ItemLayout.Goal.ToString())
                    MapUI1.RemovePositonGoal(uint.Parse(itemName[1]));

                if (itemName[0] == CastecMapUI.ItemLayout.Power.ToString())
                    MapUI1.RemovePositonPower(uint.Parse(itemName[1]));
            }
        }

        private void MapUI1_MouseSelectRange(int beginX, int beginY, int endX, int endY) {
            // 當 GL 畫布中有被框選的時候會回傳框選範圍座標，可用全域變數接下 beginX、beginY、endX、endY
            Pos pos = new Pos();
            Size frame = new Size();
            DefineArea.Add(new Point(beginX, beginY));
            DefineArea.Add(new Point(endX, endY));
            //繪製工作區域設定(顏色區別)
            pos = new Pos((beginX + endX) / 2, (beginY + endY) / 2);
            frame = new Size(Math.Abs(endX - beginX), Math.Abs(endY - beginY));
            switch (SetMode) {
                case "btnErase":
                    MapUI1.DrawRectangleFillFull(pos, frame, Color.White, "Erase", 1, (int)ItemLayout.Line);
                    break;
                case "btnStop":
                    MapUI1.DrawRectangleFillFull(pos, frame, Color.Red, "Stop", 1, (int)ItemLayout.Point);
                    break;
                case "btnPower":
                    MapUI1.DrawRectangleFillFull(pos, frame, Color.Green, "Power", 1, (int)ItemLayout.Point);
                    break;

            }
            pos = null;

        }
        #endregion

        #region Button Event
        private void Button_Click(object sender, EventArgs e) {
            string buttonName = ((Button)sender).Name;
            string[] rtnStr;
            Stat stt;
            switch (buttonName) {
                #region - btnConnect -
                case "btnConnect": {
                        if (GetConnection()) {
                            btnConnect.Image = Properties.Resources.Connect;
                            btnConnect.Text = "AGV Connected";
                            GetInfo();
                        }
                        else {
                            MessageBox.Show("Connect Failed!!");
                            btnConnect.Image = Properties.Resources.Disconnect;
                            btnConnect.Text = "Connect AGV";
                        }

                        break;
                    }
                #endregion

                #region - btnSetIP -
                case "btnSetIP": {
                        string ServerIP;
                        List<string> defaultIP = new List<string>() { "P1", "P2" };
                        CtInput IPBox = new CtInput();
                        stt = IPBox.Start(CtInput.InputStyle.COMBOBOX_LIST, "Set Map File Name", "MAP Name", out ServerIP, defaultIP);
                        if (stt == Stat.SUCCESS) {
                            lbConnectGoal.Visible = true;
                            if (ServerIP == "P2") {
                                hostIP = "192.168.31.217";
                                hostName = "P2";
                                lbConnectGoal.Text = "P2 Connected";
                            }
                            else {
                                hostIP = "192.168.31.84";
                                hostName = "P1";
                                lbConnectGoal.Text = "P1 Connected";
                            }
                        }
                    }
                    break;
                #endregion

                #region - btnPannel -
                case "btnPannel": {
                        if (tabcSetting.Visible) {
                            MapUI1.Location = new Point(0, 0);
                            MapUI1.Size = this.Size;
                            CtInvoke.TabControlVisible(tabcSetting, false);
                            CtInvoke.ButtonText(btnPannel, ">");
                            CtInvoke.ButtonLocation(btnPannel, new Point(0, 0));//openPoint
                        }
                        else {
                            MapUI1.Location = openMapUI;
                            MapUI1.Size = initialMapUI;
                            CtInvoke.TabControlVisible(tabcSetting, true);
                            CtInvoke.ButtonText(btnPannel, "<");
                            CtInvoke.ButtonLocation(btnPannel, openTabc);//openPoint
                        }
                    }
                    break;
                #endregion

                #region DriverControl
                #region - btnServoOnOff - 
                case "btnServoOnOff": {
                        if (((Button)sender).Text == "OFF") {
                            string cmd = "Set:ServoOn";
                            SendMsg(hostIP, recvCmdPort, cmd);
                            //drive.set_servo_on();
                            CtInvoke.ButtonBackColor(((Button)sender), Color.Lime);
                            CtInvoke.ButtonText(((Button)sender), "ON");
                            cmd = null;
                        }
                        else {
                            string cmd = "Set:ServoOff";
                            SendMsg(hostIP, recvCmdPort, cmd);
                            //drive.set_servo_off();
                            CtInvoke.ButtonBackColor(((Button)sender), Color.Gray);
                            CtInvoke.ButtonText(((Button)sender), "OFF");
                            cmd = null;
                        }
                    }
                    break;
                #endregion - btnServoOnOff - 

                #region - btnStartStop - 
                case "btnStartStop": {
                        if (((Button)sender).Tag.ToString() == "Stop") {
                            string cmd = "Set:Start";
                            SendMsg(hostIP, recvCmdPort, cmd);
                            //drive.set_motor_start();
                            CtInvoke.ButtonTag(((Button)sender), "Start");
                            CtInvoke.ButtonImage(((Button)sender), CAMPro.Properties.Resources.Stop);
                            cmd = null;
                        }
                        else {
                            string cmd = "Set:Stop";
                            SendMsg(hostIP, recvCmdPort, cmd);
                            //drive.set_motor_stop();
                            CtInvoke.ButtonTag(((Button)sender), "Stop");
                            CtInvoke.ButtonImage(((Button)sender), CAMPro.Properties.Resources.play);
                            cmd = null;
                        }
                    }
                    break;
                #endregion - btnStartStop - 

                #region - btnBrakes - 
                case "btnBrakes": {
                        string[] rtnCmd;
                        rtnCmd = SendMsg(hostIP, recvCmdPort, "Get:StopMode");
                        if (!bool.Parse(rtnCmd[1])) {
                            //drive.set_motor_stop_mode(true);
                            string cmd = "Set:StopMode:True";
                            SendMsg(hostIP, recvCmdPort, cmd);
                            CtInvoke.ButtonBackColor((Button)sender, Color.Lime);
                            CtInvoke.ButtonText((Button)sender, "Brakes On");

                        }
                        else {
                            //drive.set_motor_stop_mode(false);
                            string cmd = "Set:StopMode:False";
                            SendMsg(hostIP, recvCmdPort, cmd);
                            CtInvoke.ButtonBackColor((Button)sender, Color.Gray);
                            CtInvoke.ButtonText((Button)sender, "Brakes Off");
                        }
                        rtnCmd = null;
                    }
                    break;

                #endregion - btnBrakes -  
                #endregion

                #region - btnGetLaser -
                case "btnGetLaser": {
                        double angle;
                        double Laserangle;
                        SendMsg(hostIP, recvCmdPort, "Get:Laser");
                        ObstaclePoint.Clear();
                        MapUI1.RemoveGroupPoint("LaserLength");
                        for (int i = 0; i < laserData.Length; i++) {
                            if ((laserData[i] >= 30) && (laserData[i] < 15000)) {

                                if (hostName == "P1")
                                    pos = Transformation.LaserPoleToCartesian(laserData[i], -135, 0.25, i, -45, 320, -45, carX, carY, ThetaGyro, out angle, out Laserangle);//, out dataAngle, out laserAngle);
                                else
                                    pos = Transformation.LaserPoleToCartesian(laserData[i], -135, 0.25, i, 43, 416.75, 43, carX, carY, ThetaGyro, out angle, out Laserangle);//, out dataAngle, out laserAngle);
                                MapUI1.DrawPoint(pos[0], pos[1], Color.Red, 2, 2, "LaserLength");
                                //pos = Transformation.LaserPoleToCartesian(laserData[i], 0, 0.25, i, 0, 0, 0, 0);//, out dataAngle, out laserAngle);
                                // ObstaclePoint.Add(new Point(pos[0], pos[1]));
                                pos = null;
                            }
                        }
                        //MapUI1.DrawPoints(ObstaclePoint, Color.Red, "LaserLength", 2);
                    }
                    break;
                #endregion

                #region - GetCarStatus -
                case "btnGetCarStatus": {
                        if (bGetLaser) {
                            rtnStr = SendMsg(hostIP, recvCmdPort, "Get:Car:False");
                            bGetLaser = false;
                            btnGetCarStatus.BackColor = Color.Transparent;
                        }
                        else {

                            bGetLaser = true;
                            if (mTdClient != null) {
                                try {
                                    sRecvCmd.Close();
                                }
                                catch (SocketException se) {
                                    Console.WriteLine("[GetCarStatus]:"+se.ToString());
                                }
                            }
                            CtThread.CreateThread(ref mTdClient, "mTdClient: ", recvCarData);
                            rtnStr = SendMsg(hostIP, recvCmdPort, "Get:Car:True");
                            if (rtnStr[0] == "True") btnGetCarStatus.BackColor = Color.Green;
                            else bGetLaser = false;
                        }
                    }
                    break;
                #endregion

                #region - btnSetWorkVelo -
                case "btnSetVelo": {
                        int numVelo;
                        int.TryParse(txtVelocity.Text, out numVelo);
                        string cmd = "Set:WorkVelo:" + numVelo + ":" + numVelo;
                        SendMsg(hostIP, recvCmdPort, cmd);
                        cmd = null;
                    }
                    break;
                #endregion

                #region Map Control
                #region - btnDLMap -
                case "btnDLOri": {
                        string[] MapList = SendMsg(hostIP, recvCmdPort, "Get:OriList");
                        string strMapSelect;
                        if (MapList[0] != "False") {
                            Module.MapList f = new Module.MapList(MapList[1]);
                            f.ShowDialog();
                            if (f.DialogResult == DialogResult.OK) {
                                CtThread.CreateThread(ref mTdMap, "mTdMap: ", RecvFiles);
                                strMapSelect = f.strMapList;
                                SendMsg(hostIP, recvCmdPort, "Get:Ori:" + strMapSelect);
                            }
                            MapList = null;
                            strMapSelect = null;
                        }
                    }
                    break;
                #endregion

                #region - btnGetMap -
                case "btnGetMap": {
                        string[] recvMsg = SendMsg(hostIP, recvCmdPort, "Get:MapList");
                        string strMapSelect;
                        if (recvMsg[0] != "False") {
                            Module.MapList f = new Module.MapList(recvMsg[1]);
                            f.ShowDialog();
                            if (f.DialogResult == DialogResult.OK) {
                                CtThread.CreateThread(ref mTdMap, "mTdMap: ", RecvFiles);
                                strMapSelect = f.strMapList;
                                SendMsg(hostIP, recvCmdPort, "Get:Map:" + strMapSelect);
                            }
                            recvMsg = null;
                            strMapSelect = null;
                        }
                    }
                    break;
                #endregion

                #region - btnSendMap -
                case "btnSendMap": {
                        OpenFileDialog openMap = new OpenFileDialog();
                        openMap.InitialDirectory = "D:\\MapInfo\\";
                        openMap.Filter = "MAP|*.ori;*.map";
                        string fileName;
                        if (openMap.ShowDialog() == DialogResult.OK) {
                            string[] rtn = SendMsg(hostIP, recvCmdPort, "Send:map");
                            if (rtn[0] == "True") {
                                fileName = openMap.SafeFileName;
                                SendFile(hostIP, sendMapPort, fileName);
                            }
                        }
                    }
                    break;
                #endregion

                #region - btnLoadOri - 
                case "btnLoadOri": {
                        OpenFileDialog openMap = new OpenFileDialog();
                        openMap.InitialDirectory = "D:\\MapInfo\\";
                        openMap.Filter = "MAP|*.ori";

                        if (openMap.ShowDialog() == DialogResult.OK) {
                            //mbLoadMap = true;
                            //GetPointsFromFile(openMap.FileName);
                            oriPath = openMap.FileName;
                            MapUI1.RemoveAllLines();
                            MapUI1.RemoveAllPoints();
                            CtThread.CreateThread(ref mLoadOriginScanning, "mLoadOriginScaning: ", ReadOriginScanningFile);
                        }
                        openMap = null;
                    }
                    break;
                #endregion - btnLoadMap - 

                #region - btnLoadMap - 
                case "btnLoadMap": {
                        OpenFileDialog openMap = new OpenFileDialog();
                        openMap.InitialDirectory = "D:\\MapInfo\\";
                        openMap.Filter = "MAP|*map";
                        if (openMap.ShowDialog() == DialogResult.OK) {
                            mapPath = openMap.FileName;
                            string mPath = openMap.SafeFileName;
                            string cmd = "Set:MapName:" + mPath;
                            SendMsg(hostIP, sendMapPort, cmd);

                            MapUI1.RemoveAllLines();
                            MapUI1.RemoveAllPoints();
                            MapUI1.RemoveAllPositonGoal();
                            CtThread.CreateThread(ref mLoadMap, "mLoadMap: ", ReadMapFile);
                        }
                        openMap = null;
                    }
                    break;
                #endregion - btnLoadMap - 

                #region - btnClrMap -
                case "btnClrMap": {
                        MapUI1.RemoveAllLines();
                        MapUI1.RemoveAllPoints();
                        MapUI1.RemoveAllPositonGoal();
                    }
                    break;
                #endregion

                #region - btnCorrectMap -
                case "btnCorrectMap": {
                        MapUI1.RemoveAllLines();
                        MapUI1.RemoveAllPoints();
                        CtThread.CreateThread(ref mMapOperation, "mLoadOriginScaning: ", FixOriginScanningFile);
                    }
                    break;
                #endregion

                #region - btnSimplifyMap - 
                case "btnSimplify": {
                        MapUI1.RemoveAllLines();
                        MapUI1.RemoveAllPoints();
                        CtThread.CreateThread(ref mMapOperation, "mLoadSimplifyMap: ", Simplifymap);
                    }
                    break;
                #endregion - btnSimplifyMap -  

                #region - btnSaveMap -
                case "btnSaveMap": {
                        List<CartesianPos> goalList = new List<CartesianPos>();
                        for (int i = 0; i < dgvGoalPoint.Rows.Count; i++) {
                            goalList.Add(new CartesianPos(
                            Convert.ToDouble(dgvGoalPoint.Rows[i].Cells["cX"].Value),
                            Convert.ToDouble(dgvGoalPoint.Rows[i].Cells["cY"].Value),
                            Convert.ToDouble(dgvGoalPoint.Rows[i].Cells["cTheta"].Value))
                            );
                        }
                        MapRecording.OverWriteGoal(goalList, mapPath);
                        goalList = null;
                    }
                    break;
                #endregion

                #endregion

                #region - btnSet Car -
                case "btnSetCar":
                    SetMode = "btnSetCar";
                    break;
                #endregion

                #region - btnCarPosConfirm -
                case "btnPosConfirm":
                    CtThread.CreateThread(ref mMapOperation, "mLoadOriginScaning: ", CarPosConfirm);
                    break;
                #endregion

                #region - btnGoGoal -
                case "btnGoGoal": {
                        SendMsg(hostIP, recvCmdPort, "Set:Run:" + cmbGoalList.SelectedIndex);
                    }
                    break;
                #endregion

                case "btnRunAll":
                    if (bHotRun)
                        bHotRun = !bHotRun;
                    else {
                        bHotRun = true;
                        CtThread.CreateThread(ref mRunGoal, "RunAllGoal", RunAllGoal);
                    }
                    break;

                #region - btnPath -
                case "btnPath": {
                        SendMsg(hostIP, recvCmdPort, "Set:PathPlan:" + cmbGoalList.SelectedIndex);
                    }
                    break;
                #endregion

                #region - btnNewPoint - 
                case "btnNewPoint": {
                        double doubleAddPx = 0, doubleAddPy = 0, doubleAddPtheta = 0;
                        int intAddPx = 0, intAddPy = 0, intAddPtheta = 0;

                        if (!(double.TryParse(txtAddPx.Text, out doubleAddPx) && double.TryParse(txtAddPy.Text, out doubleAddPy) && double.TryParse(txtAddPtheta.Text, out doubleAddPtheta)))
                            MessageBox.Show("Please input type of integer position.");
                        else {
                            intAddPx = Convert.ToInt32(doubleAddPx);
                            intAddPy = Convert.ToInt32(doubleAddPy);
                            intAddPtheta = Convert.ToInt32(doubleAddPtheta);
                            dgvGoalPoint.Rows.Add(new Object[] { new CheckBox().Checked = false, intAddPx, intAddPy, intAddPtheta, false });
                            dgvGoalPoint.Rows[dgvGoalPoint.Rows.Count - 1].HeaderCell.Value = string.Format("{0}", dgvGoalPoint.Rows.Count);
                            MapUI1.AddPositonGoal(new Pos(doubleAddPx, doubleAddPy, doubleAddPtheta));
                        }
                    }
                    break;
                #endregion - btnNewPoint - 

                #region - btnDelete - 
                case "btnDelete": {
                        int saveCount = 0;

                        do {
                            saveCount = 0;

                            foreach (DataGridViewRow row in dgvGoalPoint.Rows) {
                                if ((bool)row.Cells[0].Value == true)
                                    dgvGoalPoint.Rows.Remove(row);
                                else
                                    saveCount++;
                            }

                            // Update point id.
                            for (int i = 0; i < dgvGoalPoint.Rows.Count; i++) {
                                dgvGoalPoint.Rows[i].HeaderCell.Value = string.Format("{0}", i + 1);
                            }
                        }
                        while (saveCount != dgvGoalPoint.Rows.Count);
                    }
                    break;
                #endregion - btnDelete - 

                #region - btnDeleteAll - 
                case "btnDeleteAll": {
                        cmbGoalList.Items.Clear();
                        dgvGoalPoint.Rows.Clear();
                        //searchSetting.goal.Clear();
                        MapUI1.RemoveAllPositonGoal();
                        MapUI1.RemoveGroupLine("PathLine");


                    }
                    break;
                #endregion - btnDeleteAll - 

                #region MapMode
                case "btnMapMode":
                    string mapName;
                    string strName = "MAP" + DateTime.Today.Month.ToString("D2") + DateTime.Today.Day.ToString("D2");
                    CtInput txtBox = new CtInput();
                    stt = txtBox.Start(CtInput.InputStyle.TEXT, "Set Map File Name", "MAP Name", out mapName, strName);
                    if (stt == Stat.SUCCESS) {
                        SendMsg(hostIP, recvCmdPort, "Set:OriName:" + mapName + ".ori");
                        SendMsg(hostIP, recvCmdPort, "Set:Mode:Map");
                    }
                    break;
                case "btnWorkMode":
                    SendMsg(hostIP, recvCmdPort, "Set:Mode:Work");
                    break;
                case "btnIdleMode":
                    SendMsg(hostIP, recvCmdPort, "Set:Mode:Idle");
                    break;
                #endregion

                #region - btnStop -
                case "btnStop":
                    MapUI1.GoalShape = Shape.Cursor;
                    SetMode = "btnStop";
                    break;
                #endregion

                #region - btnPower -
                case "btnPower":
                    SetMode = "btnPower";
                    MapUI1.GoalShape = Shape.Cursor;
                    MapUI1.RemovePolygonFillFull("Erase");
                    break;
                #endregion

                #region - btnErase -
                case "btnErase":
                    MapUI1.GoalShape = Shape.Cursor;
                    SetMode = "btnErase";
                    MapUI1.RemovePolygonFillFull("Erase");
                    break;
                #endregion

                #region - btnCursorMode -
                case "btnCursorMode":
                    SetMode = "btnCursorMode";
                    MapUI1.GoalShape = Shape.Rectangle;
                    MapUI1.RemovePolygonFillFull("Erase");
                    MapUI1.RemovePolygonFillFull("Stop");
                    MapUI1.RemovePolygonFillFull("Power");
                    break;
                #endregion

                #region - btnResetThread -
                case "btnResetThread": {
                        SendMsg(hostIP, recvCmdPort, "Set:ResetThread");
                    }
                    break;
                #endregion
                default:
                    break;
            }
        }
        private bool GetConnection() {

            string[] rtn;
            try {
                rtn = SendMsg(hostIP, recvCmdPort, "Get:Hello");
                if (rtn[0] == "True") {
                    btnConnect.Image = Properties.Resources.Connect;
                    btnConnect.Text = "AGV Connected";
                    return true;
                }
                else {
                    MessageBox.Show("Connect Failed!!");
                    btnConnect.Image = Properties.Resources.Disconnect;
                    btnConnect.Text = "Connect AGV";
                    return false;
                }
            }
            catch (SocketException se) {
                Console.WriteLine("[SocketException] : " + se.ToString());
                return false;
            }
        }
        private void GetInfo() {
            try {
                string[] rtn = SendMsg(hostIP, recvCmdPort, "Get:Info");
                if (rtn[1] == "True") {
                    CtInvoke.ButtonBackColor(btnServoOnOff, Color.Lime);
                    CtInvoke.ButtonText(btnServoOnOff, "ON");
                }
                else {
                    CtInvoke.ButtonBackColor(btnServoOnOff, Color.Gray);
                    CtInvoke.ButtonText(btnServoOnOff, "OFF");
                }
            }
            catch (SocketException se) {
                Console.WriteLine("[GetInfo]SE : " + se.ToString());
                throw;
            }
        }
        private void btn_MouseDown(object sender, MouseEventArgs e) {
            string btnName = ((Button)sender).Name;
            // string[] rtnArr;
            CtInvoke.ButtonBackColor(((Button)sender), Color.Yellow);
            if (GetConnection()) {
                CtInvoke.ButtonTag(btnStartStop, "Start");
                CtInvoke.ButtonImage(btnStartStop, Properties.Resources.Stop);
                int.TryParse(txtVelocity.Text, out mVelocity);

                switch (btnName) {
                    case "btnUp": {
                            string cmd = "Set:DriveVelo:" + mVelocity.ToString() + ":" + mVelocity.ToString();
                            SendMsg(hostIP, recvCmdPort, cmd);
                            //drive.set_velocity_same(mVelocity);
                            cmd = "Set:Start";
                            SendMsg(hostIP, recvCmdPort, cmd);
                            cmd = null;
                        }
                        break;

                    case "btnDown": {
                            string cmd = "Set:DriveVelo:" + (-mVelocity).ToString() + ":" + (-mVelocity).ToString();
                            SendMsg(hostIP, recvCmdPort, cmd);
                            //drive.set_velocity_same(-mVelocity);
                            cmd = "Set:Start";
                            SendMsg(hostIP, recvCmdPort, cmd);
                            cmd = null;
                        }
                        break;

                    case "btnLeft": {
                            string cmd = "Set:DriveVelo:" + (-mVelocity).ToString() + ":" + mVelocity.ToString();
                            SendMsg(hostIP, recvCmdPort, cmd);
                            //drive.set_velocity_different(-mVelocity, mVelocity);
                            cmd = "Set:Start";
                            SendMsg(hostIP, recvCmdPort, cmd);
                            cmd = null;
                        }
                        break;

                    case "btnRight": {
                            string cmd = "Set:DriveVelo:" + mVelocity.ToString() + ":" + (-mVelocity).ToString();
                            SendMsg(hostIP, recvCmdPort, cmd);
                            //drive.set_velocity_different(mVelocity, -mVelocity);
                            cmd = "Set:Start";
                            SendMsg(hostIP, recvCmdPort, cmd);
                            cmd = null;
                        }
                        break;

                }
            }
        }
        private void btn_MouseUp(object sender, MouseEventArgs e) {
            CtInvoke.ButtonBackColor(((Button)sender), SystemColors.Control);
            if (GetConnection()) {
                CtInvoke.ButtonTag(btnStartStop, "Stop");
                CtInvoke.ButtonImage(btnStartStop, Properties.Resources.play);
                SendMsg(hostIP, recvCmdPort, "Set:Stop");
            }
        }

        private void RunAllGoal() {
            int numGoal = 0;
            int GoalCount = cmbGoalList.Items.Count;

            if (GoalCount == 0) { return; }
            while (bHotRun) {
                Thread.Sleep(500);
                if (curMsg == "Idle") {
                    if (numGoal < GoalCount) {
                        SendMsg(hostIP, recvCmdPort, "Set:Run:" + numGoal);
                        numGoal++;
                    }
                    else
                        numGoal = 0;
                }
            }

        }
        #endregion

        #region Socket
        private string[] SendMsg(string strPicServerIP, int sendRequestPort, string sendMseeage) {
            string rtnMsg;
            string[] rcvMsg;
            tbMsg.Text = "";
            tbMsg.Text += DateTime.Now + " [Client] : " + sendMseeage + "\r\n";

            rtnMsg = SendStrMsg(strPicServerIP, sendRequestPort, sendMseeage);
            string[] strRemoteEndPoint = rtnMsg.Split(':');
            if (strRemoteEndPoint.Length > 1)
                rcvMsg = strRemoteEndPoint.Where(w => w != strRemoteEndPoint[1] && w != strRemoteEndPoint[0]).ToArray();
            else {
                rcvMsg = new string[2];
                rcvMsg[0] = rtnMsg;
            }
            if (strRemoteEndPoint.Length > 1) {
                rtnMsg = strRemoteEndPoint[2];
                if (strRemoteEndPoint[1] == "Laser" && strRemoteEndPoint[3] != "") {
                    string[] sreRemoteLaser = strRemoteEndPoint[3].Split(',');
                    laserData = sreRemoteLaser.Select(x => int.Parse(x)).ToArray();
                    sreRemoteLaser = null;
                }
                else if (strRemoteEndPoint[1] == "PathPlan" && strRemoteEndPoint[2] == "True") {
                    CtThread.CreateThread(ref mMapOperation, "mtdRecvPath", recvPath);
                }
                else if (strRemoteEndPoint[1] == "Run" && strRemoteEndPoint[2] == "True") {
                    CtThread.CreateThread(ref mMapOperation, "mtdRecvPath", recvPath);
                }
            }
            tbMsg.Text += DateTime.Now + " [Server] : " + rtnMsg + "\r\n";
            tbMsg.SelectionStart = tbMsg.Text.Length;
            tbMsg.ScrollToCaret();
            strRemoteEndPoint = null;
            return rcvMsg;

        }

        private string SendStrMsg(string strPicServerIP, int sendRequestPort, string sendMseeage) {

            //可以在字串編碼上做文章，可以傳送各種資訊內容，目前主要有三種編碼方式：
            //1.自訂連接字串編碼－－微量
            //2.JSON編碼--輕量
            //3.XML編碼--重量
            int state;
            int timeout = 5000;
            byte[] recvBytes = new byte[8192];//開啟一個緩衝區，存儲接收到的資訊

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(hostIP.ToString()), recvCmdPort);
            Socket answerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            answerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);//設置接收資料超時
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

            }
            catch (SocketException se) {
                Console.WriteLine("SocketException : {0}", se.ToString());
                MessageBox.Show("目標拒絕連線!!");
                return "False";
            }
            catch (ArgumentNullException ane) {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                return "False";
            }
            catch (Exception ex) {
                Console.Write(ex.Message);
                return "False";
            }
            finally {
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

        public void SocketRecvOn() {
            #region Recv Cmd
            serverComm.ServoOn(ref sRecvCmd, recvDataPort);
            //IPEndPoint recvCmdLocalEndPoint = new IPEndPoint(IPAddress.Any, recvDataPort);
            //sRecvCmd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //sRecvCmd.Bind(recvCmdLocalEndPoint);
            //sRecvCmd.Listen(10);
            //CtThread.CreateThread(ref mTdClient, "mTdClient: ", recvCarData);
            serverComm.ServoOn(ref sRecvFile, recvMapPort);
            //IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, recvMapPort);
            //sRecvFile = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            //sRecvFile.Bind(ipEnd);
            //sRecvFile.Listen(10);
            //
            serverComm.ServoOn(ref sRecvPath, recvPathPort);
            //IPEndPoint recvPathLocalEndPoint = new IPEndPoint(IPAddress.Any, recvPathPort);
            //sRecvPath = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //sRecvPath.Bind(recvPathLocalEndPoint);
            //sRecvPath.Listen(10);
            #endregion
        }

        public void recvCarData() {

            Socket sRecvCmdTemp = serverComm.ClientAccept(sRecvCmd);

            try {
                while (bGetLaser) {
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
                    if (strArray[0] == "Get") {
                        if (strArray[1] == "Car" && strArray.Length == 9) {
                            carID = strArray[2];
                            double.TryParse(strArray[3], out carX);
                            double.TryParse(strArray[4], out carY);
                            double.TryParse(strArray[5], out ThetaGyro);
                            int.TryParse(strArray[6], out powerPercent);
                            string[] sreRemoteLaser = strArray[7].Split(',');
                            laserData = sreRemoteLaser.Select(x => int.Parse(x)).ToArray();
                            curMsg = strArray[8];
                            CtInvoke.LabelText(lbAGVStatus, "Status : " + curMsg);
                            drawCar();
                            sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Get:Car:True"));
                            sreRemoteLaser = null;
                        }
                    }
                    else
                        sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Get:Car:False"));

                    strRecvCmd = null;
                    strArray = null;


                    //sRecvCmdTemp.Close();
                }
            }
            catch (SocketException se) {
                Console.WriteLine("[Status Recv] : " + se.ToString());
                MessageBox.Show("目標拒絕連線");
            }
            catch (Exception ex) {
                Console.Write(ex.Message);
                //throw ex;
            }
            finally {
                sRecvCmdTemp = null;
            }
        }

        public void recvPath() {
            //Socket sRecvCmdTemp = sRecvCmd.Accept();//Accept 以同步方式從偵聽通訊端的連接請求佇列中提取第一個掛起的連接請求，然後創建並返回新的 Socket
            Socket sRecvCmdTemp = serverComm.ClientAccept(sRecvPath);
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
                MapUI1.RemoveGroupLine("PATH");
                if (strArray[0] == "Path" && strArray[1] != "") {
                    string[] pathArray = strArray[1].Split(',');
                    MapUI1.RemoveGroupLine("PATH");
                    for (int i = 0; i < pathArray.Length - 5; i += 2) {
                        MapUI1.DrawLine(int.Parse(pathArray[i]), int.Parse(pathArray[i + 1]), int.Parse(pathArray[i + 2]), int.Parse(pathArray[i + 3]), Color.Green, 1, 4, "PATH");
                    }
                }
                //else
                //    sRecvCmdTemp.Send(Encoding.UTF8.GetBytes("Path:False"));

                strRecvCmd = null;
                strArray = null;
                sRecvCmdTemp.Close();

            }
            catch (SocketException se) {
                Console.WriteLine("[Status Recv] : " + se.ToString());
                MessageBox.Show("目標拒絕連線");
            }
            catch (Exception ex) {
                Console.Write(ex.Message);
                //throw ex;
            }
            finally {
                sRecvCmdTemp = null;
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
                string filePath = "D:\\MapInfo\\";
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
            }
            catch (Exception ex) {
                if (ex.Message == "No connection could be made because the target machine actively refused it")
                    curMsg = "File Sending fail. Because server not running.";
                else
                    curMsg = "File Sending fail." + ex.Message;
            }
        }

        private void RecvFiles() {
            int fileNameLen = 0;
            int recieve_data_size = 0;
            int first = 1;
            int receivedBytesLen = 0;
            double cal_size = 0;
            BinaryWriter bWrite = null;
            //MemoryStream ms = null;
            string curMsg = "Stopped";
            string fileName = "";
            try {
                curMsg = "Running and waiting to receive file.";
                Socket clientSock = serverComm.ClientAccept(sRecvFile);
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
                        if (!Directory.Exists(receivedPath)) {
                            Directory.CreateDirectory(receivedPath);
                        }
                        if (File.Exists(receivedPath + "/" + fileName)) {
                            File.Delete(receivedPath + "/" + fileName);
                        }
                        bWrite = new BinaryWriter(File.Open(receivedPath + "/" + fileName, FileMode.OpenOrCreate));
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
                    }
                    else //第二筆接收為資料  
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
                bWrite.Close();
                clientSock.Shutdown(SocketShutdown.Both);
                clientSock.Close();
            }
            catch (SocketException se) {
                Console.WriteLine("SocketException : {0}", se.ToString());
                MessageBox.Show("檔案傳輸失敗!");
                curMsg = "File Receiving error.";
            }
            catch (Exception ex) {
                Console.WriteLine("[RecvFiles]" + ex.ToString());
                curMsg = "File Receiving error.";
            }
        }
        #endregion

        #region Map

        private void drawCar() {
            double angle;
            double Laserangle;
            MapUI1.PosCar = new Pos(carX, carY, ThetaGyro);
            CtInvoke.ProgressBarValue(pBarPowerPercent, powerPercent);
            CtInvoke.GroupBoxText(gpbBattery, "Battary : (" + powerPercent + " %)");
            MapUI1.RemoveGroupPoint("LaserLength");
            for (int i = 0; i < laserData.Length; i++) {
                if ((laserData[i] >= 30) && (laserData[i] < 15000)) {
                    if (hostName == "P1")
                        pos = Transformation.LaserPoleToCartesian(laserData[i], -135, 0.25, i, -45, 320, -45, carX, carY, ThetaGyro, out angle, out Laserangle);//, out dataAngle, out laserAngle);
                    else
                        pos = Transformation.LaserPoleToCartesian(laserData[i], -135, 0.25, i, 43, 416.75, 43, carX, carY, ThetaGyro, out angle, out Laserangle);//, out dataAngle, out laserAngle);
                    MapUI1.DrawPoint(pos[0], pos[1], Color.Red, 2, 2, "LaserLength");
                    pos = null;
                }
            }
            MapUI1.ClearBuffer();
        }

        /// <summary>
        /// Read origin scanning information
        /// </summary>
        private void ReadOriginScanningFile() {
            MapReading MapReading = new MapReading(oriPath);
            CartesianPos carPos;
            List<CartesianPos> laserData;
            //List<Point> listMap = new List<Point>();
            int dataLength = MapReading.OpenFile();
            if (dataLength != 0) {
                for (int n = 0; n < dataLength; n++) {
                    MapReading.ReadScanningInfo(n, out carPos, out laserData);
                    MapUI1.PosCar = new Pos(carPos.x, carPos.y, carPos.theta);
                    for (int m = 0; m < laserData.Count; m++) {
                        MapUI1.DrawPoint((int)laserData[m].x, (int)laserData[m].y, Color.Black, 2, 1, "MAP");
                    }
                    carPos = null;
                    laserData = null;
                }
            }
            MapReading = null;
            SetBalloonTip("Load Map", "Load Complele!!", ToolTipIcon.Info, 10);
        }

        /// <summary>
        /// Read origin scanning information
        /// </summary>
        private void FixOriginScanningFile() {
            MapReading MapReading = new MapReading(oriPath);
            CartesianPos carPos;
            List<CartesianPos> laserData;
            List<CartesianPos> filterData = new List<CartesianPos>();
            int dataLength = MapReading.OpenFile();
            if (dataLength == 0) return;

            List<CartesianPos> dataSet = new List<CartesianPos>();
            List<CartesianPos> predataSet = new List<CartesianPos>();
            List<CartesianPos> matchSet = new List<CartesianPos>();
            CartesianPos transResult = new CartesianPos();
            CartesianPos nowOdometry = new CartesianPos();
            CartesianPos preOdometry = new CartesianPos();
            CartesianPos accumError = new CartesianPos();
            CartesianPos diffOdometry = new CartesianPos();
            CartesianPos diffLaser = new CartesianPos();
            Stopwatch sw = new Stopwatch();
            double gValue = 0;
            int mode = 0;
            int corrNum = 0;

            mapMatch.Reset();
            #region  1.Read car position and first laser scanning

            MapReading.ReadScanningInfo(0, out carPos, out laserData);
            MapUI1.PosCar = new Pos(carPos.x, carPos.y, carPos.theta);
            matchSet.AddRange(laserData);
            predataSet.AddRange(laserData);
            mapMatch.GlobalMapUpdate(matchSet);                            //Initial environment model
            preOdometry.SetPosition(carPos.x, carPos.y, carPos.theta);

            #endregion

            for (int n = 1; n < dataLength; n++) {
                #region 2.Read car position and laser scanning 

                List<CartesianPos> addedSet = new List<CartesianPos>();
                transResult.SetPosition(0, 0, 0);
                carPos = null;
                laserData = null;
                MapReading.ReadScanningInfo(n, out carPos, out laserData);
                nowOdometry.SetPosition(carPos.x, carPos.y, carPos.theta);

                #endregion

                #region 3.Correct accumulate error of odometry so far

                mapMatch.NewPosTransformation(nowOdometry, accumError.x, accumError.y, accumError.theta);
                mapMatch.NewPosTransformation(laserData, accumError.x, accumError.y, accumError.theta);
                matchSet.Clear();
                matchSet.AddRange(laserData);

                #endregion

                #region 4.Compute movement from last time to current time;

                if (nowOdometry.theta - preOdometry.theta < -200)
                    diffOdometry.SetPosition(nowOdometry.x - preOdometry.x, nowOdometry.y - preOdometry.y, nowOdometry.theta + 360 - preOdometry.theta);
                else if (nowOdometry.theta - preOdometry.theta > 200)
                    diffOdometry.SetPosition(nowOdometry.x - preOdometry.x, nowOdometry.y - preOdometry.y, -(preOdometry.theta + 360 - nowOdometry.theta));
                else
                    diffOdometry.SetPosition(nowOdometry.x - preOdometry.x, nowOdometry.y - preOdometry.y, nowOdometry.theta - preOdometry.theta);
                Console.WriteLine("Odometry varition:{0:F3} {1:F3} {2:F3}", diffOdometry.x, diffOdometry.y, diffOdometry.theta);

                #endregion

                #region 5.Display current scanning information

                MapUI1.RemoveGroupPoint("ScanPoint");
                //scanPoint.Clear();
                for (int m = 0; m < matchSet.Count; m++) {
                    MapUI1.DrawPoint((int)matchSet[m].x, (int)matchSet[m].y, Color.Red, 1, 3, "ScanPoint");
                    // scanPoint.Add(new Point((int)matchSet[m].x, (int)matchSet[m].y));
                }
                //castecMapUI1.DrawPoints(scanPoint, Color.Red, "ScanPoint", 1);

                #endregion

                #region 6.Inspect odometry variation is not too large.Switch to pose tracking mode if too large.

                sw.Restart();
                if (Math.Abs(diffOdometry.x) >= 400 || Math.Abs(diffOdometry.y) >= 400 || Math.Abs(diffOdometry.theta) >= 30) {
                    mode = 1;
                    gValue = mapMatch.PairwiseMatching(predataSet, matchSet, 4, 1.5, 0.01, 20, 300, false, transResult);
                }
                else {
                    mode = 0;
                    gValue = mapMatch.FindClosetMatching(matchSet, 4, 1.5, 0.01, 20, 300, false, transResult);
                    diffLaser.SetPosition(transResult.x, transResult.y, transResult.theta);
                }

                //If corresponding is too less,truct the odomery variation this time
                if (mapMatch.EstimateCorresponingPoints(matchSet, 10, 10, out corrNum, out addedSet)) {
                    mapMatch.NewPosTransformation(nowOdometry, transResult.x, transResult.y, transResult.theta);
                    accumError.SetPosition(accumError.x + transResult.x, accumError.y + transResult.y, accumError.theta + transResult.theta);
                }
                sw.Stop();

                if (mode == 0)
                    Console.WriteLine("[SLAM-Matching Mode]Corresponding Points:{0} Map Size:{1} Matching Time:{2:F3} Error{3:F3}",
                         corrNum, mapMatch.parseMap.Count, sw.Elapsed.TotalMilliseconds, gValue);
                else
                    Console.WriteLine("[SLAM-Tracking Mode]Matching Time:{0:F3} Error{1:F3}", sw.Elapsed.TotalMilliseconds, gValue);

                #endregion

                #region 7.Update variation

                //Pairwise update
                predataSet.Clear();
                predataSet.AddRange(laserData);

                //Update previous variable
                preOdometry.SetPosition(nowOdometry.x, nowOdometry.y, nowOdometry.theta);

                #endregion

                //Display added new points                     
                for (int m = 0; m < addedSet.Count; m++) {
                    MapUI1.DrawPoint((int)addedSet[m].x, (int)addedSet[m].y, Color.Black, 1, 3, "ScanMap");
                }
                addedSet = null;

                //Display car position
                MapUI1.PosCar = new Pos(nowOdometry.x, nowOdometry.y, nowOdometry.theta);
            }
            MapUI1.RemoveGroupPoint("ScanPoint");
            MapUI1.PosCar = new Pos(0, 0, 0);
            MapReading = null;
            SetBalloonTip("Correct Map", "Correct Complete!!", ToolTipIcon.Info, 10);
        }

        /// <summary>
        /// Simplify map
        /// </summary>
        private void Simplifymap() {
            string[] tmpPath = oriPath.Split('.');
            oriMap = tmpPath[0] + ".map";
            MapSimplication mapSimp = new MapSimplication(oriMap);
            mapSimp.Reset();
            List<MapLIne> obstacleLines = new List<MapLIne>();
            List<Point> obstaclePoints = new List<Point>();
            List<CartesianPos> resultPoints;
            List<MapSimplication.Line> resultlines;
            mapSimp.ReadMapAllTransferToLine(mapMatch.parseMap, mapMatch.minimumPos, mapMatch.maximumPos
                , 100, 0, out resultlines, out resultPoints);

            for (int i = 0; i < resultlines.Count; i++) {
                obstacleLines.Add(
                    new MapLIne(resultlines[i].startX, resultlines[i].startY,
                    resultlines[i].endX, resultlines[i].endY)
                );
            }

            for (int i = 0; i < resultPoints.Count; i++) {
                obstaclePoints.Add(new Point((int)resultPoints[i].x, (int)resultPoints[i].y));
            }

            MapUI1.DrawLines(obstacleLines, Color.Black, "ObstacleLines", true);
            MapUI1.DrawPoints(obstaclePoints, Color.Black, "ObstaclePoints", 1);
            obstacleLines = null;
            obstaclePoints = null;
            resultPoints = null;
            resultlines = null;
            SetBalloonTip("Simplify Map", "Simplify Complete!!", ToolTipIcon.Info, 10);
        }
        /// <summary>
        /// Read map file
        /// </summary>
        private void ReadMapFile() {
            List<MapLIne> dispLines = new List<MapLIne>();
            List<Point> dispPoint = new List<Point>();
            List<CartesianPos> goalList;
            List<CartesianPos> obstaclePoints;
            List<MapLine> obstacleLine;
            CartesianPos minimumPos;
            CartesianPos maximumPos;

            #region - Retrive information from .map file -

            using (MapReading read = new MapReading(mapPath)) {
                read.OpenFile();
                read.ReadMapBoundary(out minimumPos, out maximumPos);
                read.ReadMapGoalList(out goalList);
                read.ReadMapObstacleLines(out obstacleLine);
                read.ReadMapObstaclePoints(out obstaclePoints);
            }

            mapMatch.Reset();
            for (int i = 0; i < obstacleLine.Count; i++) {
                int start = (int)obstacleLine[i].start.x;
                int end = (int)obstacleLine[i].end.x;
                int y = (int)obstacleLine[i].start.y;
                for (int x = start; x < end; x++) {
                    mapMatch.AddPoint(new CartesianPos(x, y));
                }
            }

            for (int i = 0; i < obstaclePoints.Count; i++) {
                mapMatch.AddPoint(obstaclePoints[i]);
            }
            #endregion

            #region  - Map information display -

            for (int i = 0; i < obstacleLine.Count; i++) {
                dispLines.Add(
                    new MapLIne((int)obstacleLine[i].start.x, (int)obstacleLine[i].start.y,
                    (int)obstacleLine[i].end.x, (int)obstacleLine[i].end.y)
                );
            }

            for (int i = 0; i < obstaclePoints.Count; i++) {
                dispPoint.Add(new Point((int)obstaclePoints[i].x, (int)obstaclePoints[i].y));
            }

            MapUI1.DrawLines(dispLines, Color.Black, "ObstacleLines", true);
            MapUI1.DrawPoints(dispPoint, Color.Black, "ObstaclePoints", 1);
            CtInvoke.DataGridViewClear(dgvGoalPoint);
            CtInvoke.ComboBoxClear(cmbGoalList);
            for (int i = 0; i < goalList.Count; i++) {
                CtInvoke.ComboBoxAdd(cmbGoalList, string.Format("{0},{1},{2}", goalList[i].x, goalList[i].y, goalList[i].theta));
                CtInvoke.DataGridViewAddRow(dgvGoalPoint, (new Object[] { new CheckBox().Checked = false, goalList[i].x, goalList[i].y, goalList[i].theta, false }));
                MapUI1.AddPositonGoal(new Pos(goalList[i].x, goalList[i].y, goalList[i].theta));
            }
            if (goalList.Count > 0)
                CtInvoke.ComboBoxSelectedIndex(cmbGoalList, 0);

            #endregion

            goalList = null;
            obstaclePoints = null;
            obstacleLine = null;
            minimumPos = null;
            maximumPos = null;
            dispLines = null;
            dispPoint = null;
        }
        /// <summary>
        /// Confirm car position assigned by user
        /// </summary>
        private void CarPosConfirm() {
            MapReading MapReading = new MapReading(oriPath);
            List<CartesianPos> matchSet = new List<CartesianPos>();
            List<CartesianPos> modelSet = new List<CartesianPos>();
            CartesianPos nowOdometry = new CartesianPos();
            CartesianPos transResult = new CartesianPos();
            List<Point> scanPoint = new List<Point>();
            double angle;
            double Laserangle;
            double gValue = 0;
            double similarity = 0;
            int[] obstaclePos = new int[2];
            //mAGV.GetPosition(out posX, out posY, out posT);

            for (int i = 0; i < laserData.Length; i++) {
                if (hostName == "P1")
                    obstaclePos = Transformation.LaserPoleToCartesian(laserData[i], -135, 0.25, i, -45, 320, -45, MapUI1.PosCar.x, MapUI1.PosCar.y, MapUI1.PosCar.theta, out angle, out Laserangle);//, out dataAngle, out laserAngle);
                else
                    obstaclePos = Transformation.LaserPoleToCartesian(laserData[i], -135, 0.25, i, 43, 416.75, 43, MapUI1.PosCar.x, MapUI1.PosCar.y, MapUI1.PosCar.theta, out angle, out Laserangle);//, out dataAngle, out laserAngle);

                matchSet.Add(new CartesianPos(obstaclePos[0], obstaclePos[1]));
                obstaclePos = null;
            }
            nowOdometry.SetPosition(MapUI1.PosCar.x, MapUI1.PosCar.y, MapUI1.PosCar.theta * Math.PI / 180);
            gValue = mapMatch.FindClosetMatching(matchSet, 4, 1.5, 0.01, 50, 2000, false, transResult, out modelSet);
            //Correct accumulate error this time
            mapMatch.NewPosTransformation(nowOdometry, transResult.x, transResult.y, transResult.theta);
            mapMatch.NewPosTransformation(matchSet, transResult.x, transResult.y, transResult.theta);
            double[] Position = new double[3] { nowOdometry.x, nowOdometry.y, nowOdometry.theta * 180 / Math.PI };

            //Display car position
            //MapUI1.PosCar = new Pos(nowOdometry.x, nowOdometry.y, nowOdometry.theta);
            //SetPosition(nowOdometry.x, nowOdometry.y, nowOdometry.theta);
            //Display current scanning information
            MapUI1.RemoveGroupPoint("LaserLength");
            scanPoint.Clear();
            for (int m = 0; m < matchSet.Count; m++) {
                scanPoint.Add(new Point((int)matchSet[m].x, (int)matchSet[m].y));
            }
            MapUI1.DrawPoints(scanPoint, Color.Red, "LaserLength", 1);
            MapUI1.PosCar = new Pos(Position[0], Position[1], Position[2]);
            similarity = mapMatch.SimilarityEvalute(modelSet, matchSet);
            //if (similarity > 0.85) {
            SendMsg(hostIP, recvCmdPort, "Set:POS:" + (int)Position[0] + ":" + (int)Position[1] + ":" + (int)Position[2]);
            //}
        }
        #endregion

        private void SetBalloonTip(string tipTittle, string tipText, ToolTipIcon Icon, int ToolTipTimer) {
            notifyIcon1.Icon = SystemIcons.Exclamation;
            notifyIcon1.BalloonTipTitle = tipTittle;
            notifyIcon1.BalloonTipText = tipText;
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(ToolTipTimer);
        }
    }
}
