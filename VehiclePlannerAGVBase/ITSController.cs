using AGVDefine;
using CtLib.Library;
using Geometry;
using SerialCommunication;
using SerialCommunicationData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using VehiclePlanner.Core;

namespace VehiclePlannerAGVBase {
    /// <summary>
    /// 序列傳輸實作iTS控制
    /// </summary>
    internal class ITSControllerSerial : BaseiTSController<IBasicPacket, IProductPacket>, IITSController_AGVBase {

        #region Declaration - Fields

        private MapGLController mMapGL = MapGLController.GetInstance();

        /// <summary>
        /// 序列化傳輸物件
        /// </summary>
        private ISerialClient mSerialClient = null;
        
        /// <summary>
        /// iTS狀態
        /// </summary>
        private IStatus mStatus = FactoryMode.Factory.Status();

        #endregion Declaration - Fields

        #region Declaration - Properties

        /// <summary>
        /// 是否已建立連線
        /// </summary>
        public override bool IsConnected { get => mSerialClient?.Connected ?? false; }

        /// <summary>
        /// iTS狀態
        /// </summary>
        public IStatus Status {
            get {
                return mStatus;
            }
            protected set {
                if (value != null && mStatus != value) {
                    mStatus = value;
                    mMapGL.SetLocation(mStatus.Data);
                    OnPropertyChanged();
                }
            }
        }

        #endregion Declaration - Properties

        #region Funciotn - Constructors

        public ITSControllerSerial() : base() {

        }

        #endregion Funciotn - Constructors

        #region Funciton - Events

        /// <summary>
        /// 序列化通訊接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSerialClient_ReceiveData(object sender, ReceiveDataEventArgs e)
        {
            if (e.Data is IProductPacket)
            {
                var product = e.Data as IProductPacket;
                /*-- 查詢是否有等待該封包 --*/
                ReceiveDataCheck(product);
            }
        }

        /// <summary>
        /// 連線狀態變更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSerialClient_OnConnectChange(object sender, ConnectStatusChangeEventArgs e) {
            OnPropertyChanged(nameof(IsConnected));
            OnPropertyChanged(nameof(IsSearchable));
            OnConsoleMessage($"Client - Is {(e.IsConnected ? "Connected" : "Disconnected")} to {e.IP}:{e.Port}");
            if (e.IsConnected) {
                DoPositionComfirm();
                Status = RequestStatus();
            } else {
                if (mSerialClient != null) {
                    mSerialClient.Dispose();
                    mSerialClient = null;
                }
            }
        }

        #endregion Funciton - Events

        #region Funciton - Public Methods
        
        /// <summary>
        /// 雷射命令要求传送
        /// </summary>
        /// <returns></returns>
        protected override BaseRequeLaser ImpRequestLaser() => new RequestLaser(Send(FactoryMode.Factory.Order().RequestLaser()));

        /// <summary>
        /// 马达激磁状态设定命令发送
        /// </summary>
        /// <param name="servoOn">/欲设定的马达激磁状态</param>
        /// <returns></returns>
        protected override BaseBoolReturn ImpSetServoMode(bool servoOn) => new SetServoMode(Send(FactoryMode.Factory.Order().SetServoMode(servoOn)));

        /// <summary>
        /// 設定iTS工作移動速度
        /// </summary>
        /// <param name="velocity">移動速度</param>
        /// <returns>是否設定成功</returns>
        protected override BaseBoolReturn ImpSetWorkVelocity(int velocity) => new SetWorkVelocity(Send(FactoryMode.Factory.Order().SetWorkVelocity(velocity)));

        /// <summary>
        /// 位置校正命令发送
        /// </summary>
        /// <returns>地圖相似度</returns>
        protected override BaseDoPositionComfirm ImpDoPositionComfirm() => new DoPositionComfirm(Send(FactoryMode.Factory.Order().DoPositionComfirm()));

        /// <summary>
        /// 移动至目标点
        /// </summary>
        /// <param name="goalName">目标点名称</param>
        protected override BaseGoTo ImpGoTo(string goalName) => new GoTo(Send(FactoryMode.Factory.Order().DoRuningByGoalName(goalName)));

        /// <summary>
        /// 要求Map档清单
        /// </summary>
        protected override BaseListReturn ImpRequestMapList() => new RequestMapList(Send(FactoryMode.Factory.Order().RequestMapList()));

        /// <summary>
        /// 要求到指定Goal點的路徑
        /// </summary>
        /// <param name="goalIndex">目標Goal點索引</param>
        /// <returns>路徑(Count為0表示規劃失敗)</returns>
        protected override BaseRequestPath ImpRequestPath(string goalName) => new RequestPath(Send(FactoryMode.Factory.Order().RequestPath(goalName)));

        /// <summary>
        /// 要求Goal點清單
        /// </summary>
        /// <returns>Goal點清單</returns>
        protected override BaseRequestGoalList ImpRequestGoalList() => new RequestGoalList(Send(FactoryMode.Factory.Order().RequestGoalList()));

        /// <summary>
        /// 要求Map檔
        /// </summary>
        /// <param name="mapName">要求的Map檔名</param>
        /// <returns>Map檔</returns>
        public override BaseFileReturn RequestMapFile(string mapName) => new RequestMapFile(Send(FactoryMode.Factory.Order().RequestMapFile(mapName)));
        
        public override void StartScan(bool scan) {
            try {
                BaseBoolReturn isScanning = null;
                if (mIsScanning != scan) {
                    if (scan) {//開始掃描
                        if (mStatus?.Description == EDescription.Idle) {
                            string oriName = string.Empty;
                            if (InputBox.Invoke(out oriName, "MAP Name", "Set Map File Name")) {
                                isScanning = SetScanningOriFileName(oriName);
                            }
                            if (isScanning.Requited && isScanning.Value) {
                                OnConsoleMessage($"iTS - The new ori name is {oriName}.ori");
                            }
                        } else {
                            OnConsoleMessage($"The iTS is now in {mStatus?.Description}, can't start scanning");
                        }
                    } else {//停止掃描
                        if (true || mStatus?.Description == EDescription.Map) {
                            isScanning = StopScanning();
                        } else {
                            OnConsoleMessage($"The iTS is now in {mStatus?.Description}, can't stop scanning");
                        }
                    }
                    if (isScanning != null) {
                        IsScanning = isScanning.Value;
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage("Error:" + ex.Message);
            }
        }

        /// <summary>
        /// 要求AGV設定新位置
        /// </summary>
        /// <param name="oldPosition">舊座標</param>
        /// <param name="newPosition">新座標</param>
        public void SetPosition(IPair oldPosition, IPair newPosition) {
            var position = FactoryMode.Factory.TowardPair(newPosition, oldPosition.Angle(newPosition));
            var success = SetPosition(position);
            if (success == true) {
                mMapGL.SetLocation(position);
                OnConsoleMessage($"iTS - The position are now at {position}");
            }
        }
        
        /// <summary>
        /// 取得Ori檔
        /// </summary>
        public override void GetOri() {
            bool? success = null;
            string oriName = null;
            try {
                string oriList = RequestOriList();
                if (!string.IsNullOrEmpty(oriList)) {
                    oriName = SelectFile(oriList);
                    if (!string.IsNullOrEmpty(oriName)) {
                        var ori = RequestOriFile(oriName);
                        if (ori != null) {
                            if (ori.SaveAs(@"D:\MapInfo\Client")) {
                                success = true;
                                OnConsoleMessage($"Planner - {ori.Name} download completed");
                            } else {
                                success = false;
                                OnConsoleMessage($"Planner - {ori.Name} failed to save");
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            } finally {
                if (success != null) {
                    OnBalloonTip("Donwload", $"{oriName}.ori is downloaded {(success == true ? "successfully" : "failed")} ");
                }
            }
        }

        /// <summary>
        /// 切換資料自動回傳
        /// </summary>
        public override void AutoReport(bool auto) {
            try {
                bool isAutoReport = auto;
                var laser = AutoReportLaser(isAutoReport);
                var status = AutoReportStatus(isAutoReport);
                var path = AutoReportPath(isAutoReport);
                IsAutoReport = (laser?.Count ?? 0) > 0;
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }
        
        #endregion Funciotn - Public Methods

        #region Funciton - Private Methods

        /// <summary>
        /// 产生命令标题
        /// </summary>
        /// <param name="packet">命令物件</param>
        /// <returns>命令标题</returns>
        protected override string CmdTitle(IBasicPacket packet) =>  $"{packet.Purpose}({packet.SerialNumber}):";

        /// <summary>
        /// 检测是否等待中的命令
        /// </summary>
        /// <param name="v">等待的命令</param>
        /// <param name="packet">要发送的命令</param>
        /// <returns>是否等待中命令</returns>
        protected override bool IsWaitingCmd(CtTaskCompletionSource<IBasicPacket, IProductPacket> v, IBasicPacket packet) => v.WaitCmd.Purpose == packet.Purpose;

        /// <summary>
        /// 检测是否等待中的回复
        /// </summary>
        /// <param name="v">等待的命令</param>
        /// <param name="response">收到的回复</param>
        /// <returns>是否是等待中的命令</returns>
        protected override bool IsWaitingResponse(CtTaskCompletionSource<IBasicPacket, IProductPacket> v, IProductPacket response) => v.WaitCmd.Purpose == response.Purpose;
        
        /// <summary>
        /// 传送命令
        /// </summary>
        /// <param name="packet">命令物件</param>
        /// <returns>是否传送成功</returns>
        protected override bool ClientSend(IBasicPacket packet) => mSerialClient.Send(packet);
        
        /// <summary>
        /// 要求iTS狀態
        /// </summary>
        /// <returns>iTS狀態</returns>
        protected IStatus RequestStatus() {
            var status = Send(FactoryMode.Factory.Order().RequestStatus())?.ToIRequestStatus()?.Product;
            return status;
        }

        /// <summary>
        /// 要求Ori檔清單
        /// </summary>
        /// <returns>Ori檔清單</returns>
        protected override string RequestOriList() {
            var oriList = Send(FactoryMode.Factory.Order().RequestOriList())?.ToIRequestOriList()?.Product;
            return oriList != null ? string.Join(",", oriList) : null;
        }

        /// <summary>
        /// 要求Ori檔
        /// </summary>
        /// <param name="oriName">Ori檔名</param>
        /// <returns>Ori檔</returns>
        protected IDocument RequestOriFile(string oriName) {
            var oriFile = Send(FactoryMode.Factory.Order().RequestOriFile(oriName))?.ToIRequestOriFile()?.Product;
            return oriFile;
        }

        /// <summary>
        /// 上傳Map檔
        /// </summary>
        /// <param name="mapPath">要上傳的Map檔路徑</param>
        /// <returns>是否上傳成功</returns>
        protected override BaseBoolReturn UploadMapToAGV(string mapPath) => new UploadMapToAGV(Send(FactoryMode.Factory.Order().UploadMapToAGV(mapPath))); 

        /// <summary>
        /// 要求iTS載入指定的Map檔
        /// </summary>
        /// <param name="mapName">要載入的Map檔名</param>
        /// <returns>是否切換成功</returns>
        protected override BaseBoolReturn ChangeMap(string mapName) => new ChangeMap(Send(FactoryMode.Factory.Order().ChangeMap(mapName))); 

        /// <summary>
        /// 要求自動回報iTS狀態
        /// </summary>
        /// <param name="on">是否自動回報</param>
        /// <returns>iTS狀態</returns>
        protected IStatus AutoReportStatus(bool on) {
            var status = Send(FactoryMode.Factory.Order().AutoReportStatus(on))?.ToIAutoReportStatus()?.Product;
            return status;
        }

        /// <summary>
        /// 要求自動回傳雷射資料
        /// </summary>
        /// <param name="on">是否自動回報</param>
        /// <returns>雷射資料</returns>
        protected List<IPair> AutoReportLaser(bool on) {
            var laser = Send(FactoryMode.Factory.Order().AutoReportLaser(on))?.ToIAutoReportLaser()?.Product;
            return laser;
        }

        /// <summary>
        /// 要求自動回傳路徑
        /// </summary>
        /// <param name="on">是否自動回報</param>
        /// <returns>路徑資料</returns>
        protected List<IPair> AutoReportPath(bool on) {
            var path = Send(FactoryMode.Factory.Order().AutoReportPath(on))?.ToIAutoReportPath()?.Product;
            return path;
        }

        /// <summary>
        /// 設定iTS當前位置
        /// </summary>
        /// <returns>是否設定成功</returns>
        protected bool? SetPosition(ITowardPair position) {
            var success = Send(FactoryMode.Factory.Order().SetPosition(position))?.ToISetPosition()?.Product;
            return success;
        }

        /// <summary>
        /// 開始手動控制
        /// </summary>
        /// <param name="start">是否開始手動控制</param>
        /// <remarks>是否為手動控制狀態</remarks>
        protected override BaseBoolReturn StartManualControl(bool start) => new StartManualControl(Send(FactoryMode.Factory.Order().StartManualControl(start))); 

        /// <summary>
        /// 設定手動控制移動速度(方向)
        /// </summary>
        /// <param name="velocity">手動移動速度</param>
        /// <returns>是否設定成功</returns>
        protected override BaseBoolReturn SetManualVelocity(int leftVelocity, int rightVelocity) {
            var velocity = FactoryMode.Factory.Pair(leftVelocity, rightVelocity);
            return new SetManualVelocity(Send(FactoryMode.Factory.Order().SetManualVelocity(velocity)));
        }

        /// <summary>
        /// 停止掃描地圖
        /// </summary>
        /// <returns>是否在掃描中</returns>
        protected override BaseBoolReturn StopScanning() => new StopScanning(Send(FactoryMode.Factory.Order().StopScanning()));

        /// <summary>
        /// 設定地圖檔名
        /// </summary>
        /// <remarks>是否在掃描中</remarks>
        protected override BaseSetScanningOriFileName SetScanningOriFileName(string oriName) => new SetScanningOriFileName(Send(FactoryMode.Factory.Order().SetScanningOriFileName(oriName)));

        /// <summary>
        /// 繪製雷射
        /// </summary>
        /// <param name="laser"></param>
        protected void DrawLaser(IEnumerable<IPair> laser) {
            mMapGL.DrawLaser(laser);
        }

        /// <summary>
        /// 繪製路徑
        /// </summary>
        /// <param name="path"></param>
        protected void DrawPath(IEnumerable<IPair> path) {
            mMapGL.DrawPath(path);
        }

        /// <summary>
        /// 用户端初始化
        /// </summary>
        protected override void ClientInitial()
        {
            if (mSerialClient != null)
            {
                mSerialClient.ConnectChange -= mSerialClient_OnConnectChange;
                mSerialClient.Dispose();
            }
            mSerialClient = FactoryMode.Factory.SerialClient(mSerialClient_ReceiveData, mBypassSocket);
            mSerialClient.ConnectChange += mSerialClient_OnConnectChange;
        }

        /// <summary>
        /// 连线至伺服端
        /// </summary>
        /// <param name="ip">伺服端IP</param>
        /// <param name="port">伺服端埠号</param>
        protected override void ClientConnect(string ip, int port)
        {
            mSerialClient.Connect(ip, port);
        }

        /// <summary>
        /// 停止与伺服端连线
        /// </summary>
        protected override void ClientStop()
        {
            mSerialClient.Stop();
        }

        /// <summary>
        /// 执行回应动作
        /// </summary>
        /// <param name="product">回应物件</param>
        protected override void DoReceiveAction(IProductPacket product)
        {
            switch (product.Purpose)
            {
                case EPurpose.AutoReportLaser:
                    var laser = product.ToIAutoReportLaser().Product;
                    if (laser != null)
                    {
                        DrawLaser(product.ToIAutoReportLaser().Product);
                    }
                    else
                    {
                        IsAutoReport = false;
                        mMapGL.ClearLaser();
                        mMapGL.ClearPath();
                    }
                    break;
                case EPurpose.AutoReportPath:
                    {
                        var path = product.ToIAutoReportPath().Product;
                        if (path != null)
                        {
                            DrawPath(path);
                        }
                        break;
                    }
                case EPurpose.AutoReportStatus:
                    Status = product.ToIAutoReportStatus()?.Product;
                    break;
            }
        }

		protected override void LoadMap(string path)
		{
			throw new NotImplementedException();
		}

		protected override BaseBoolReturn RequireStopAGV()
		{
			throw new NotImplementedException();
		}

		protected override BaseBoolReturn RequireUncharge()
		{
			throw new NotImplementedException();
		}

		public override void StopAGV()
		{
			throw new NotImplementedException();
		}

		public override void Uncharge()
		{
			throw new NotImplementedException();
		}
	}

    #endregion Funciotn - Private Methods

}


