using AGVDefine;
using BroadCast;
using CtLib.Forms;
using CtLib.Library;
using CtLib.Module.Utility;
using Geometry;
using GLCore;
using SerialCommunication;
using SerialCommunicationData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VehiclePlanner.Partial.CtVehiclePlanner;
using VehiclePlanner.Partial.VehiclePlannerUI;

namespace VehiclePlanner {
    
    public partial class CtVehiclePlanner : INotifyPropertyChanged, ICtVersion,IDisposable {
        
        #region Version - Information

        /// <summary>
        /// AgvClientUI版本資訊
        /// </summary>
        /// <remarks>
        ///     0.0.0   Jay [2017/09/28]
        ///         + 整合AgvClient
        ///         + 加上使用者登入/登出
        ///         + 加上使用者管理
        ///         + 加入版本號
        ///         \ 修改Load File進度條為百分比
        ///         \ 取得Ori檔後解鎖Ori檔操作
        ///         \ 補上SimplfyOri功能
        ///     0.0.1   Jay [2017/10/03]
        ///         \ 通訊機制修改
        ///         + 於GoalSetting加入GetGoalList功能
        ///         \ 連線時加入IP參數，可在介面指定要連接的IP
        ///         \ 專案建置事件修正
        ///     0.0.2   Jay [2017/10/11]
        ///         \ 地圖修正功能修正
        ///         \ 與AGV連線方式調整
        ///         \ Ori繪製後補上雷射圖層清除
        ///     0.0.3   Jay [2017/10/12]
        ///         \ 更新部分語法以符合新版本CtLib
        ///     0.0.4   Jay [2017/10/23]
        ///         \ 命令傳送用Socket連線方式修改，改為持續連接
        ///     0.0.5   Jay [2017/11/08]
        ///         \ 加入Power點相關操作
        ///         \ 補上ClearMap功能
        ///         \ Map檔格式修改
        ///     0.0.6   Jay [2017/11/22]
        ///         \ 地圖匹配功能修正
        ///         \ 路徑規劃、跑點功能修正
        ///         \ 加入Charging功能
        ///         \ Map讀檔方法重構
        ///     0.0.7   Jay [2017/11/27]
        ///         \ 加入鍵盤控制AGV移動功能
        ///         \ 移除Ori修正功能
        ///     0.0.8   Jay [2017/11/29]
        ///         \ 限定MapGL不可隱藏
        ///     0.0.9   Jay [2017/11/30]
        ///         \ 加入Outlookbar控制項實作工具箱視窗UI
        ///         \ 地圖插入控制面板實作
        ///         \ 移除KeyboardHook
        ///         \ 單獨監測Testing視窗之鍵盤事件
        ///     0.0.10  Jay [2017/12/06]
        ///         \ 加入路徑相關操作鎖定，必須在地圖相似度80%以上才可進行路徑相關操作
        ///     0.0.11  Jay [2017/12/07]
        ///         \ 優化Database與GoalSetting之間的聯動性
        ///         \ 於底層路徑相關操作加入相似度門檻值鎖定
        ///     0.0.12  Jay [2017/12/12] 
        ///         + 加入AGV移動控制器        
        ///     0.0.13  Jay [2017/12/14]
        ///         \ 將所有按鈕解鎖，當無法處於執行按鈕功能狀態，以對話視窗引導使用者進行狀態修正
        ///         \ 重寫模組權限(加入CtToolBox控制)
        ///         \ 重寫AGVMapUI的ShowWindow與HideWindow方法
        ///     1.0.0   Jay [2018/02/22]
        ///         \ 架構重整，分離底層與介面
        /// </remarks>
        public CtVersion Version { get { return new CtVersion(1, 0, 0, "2018/02/22", "Jay Chang"); } }

        #endregion Version - Information

        #region Funciton - Events

        #region BroadcastReceiver

        /// <summary>
        /// 廣播接收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mBroadcast_ReceivedData(object sender, BroadcastEventArgs e) {
            /*-- 紀錄有回應的iTS IP位址 --*/
            if (!mAgvList.ContainsKey(e.Remote.Address)) {
                mAgvList.Add(e.Remote.Address, e.Message);
            }
        }

        #endregion BroadcastReceiver

        #region KeyboardHook

        /// <summary>
        /// 全域鍵盤按下事件
        /// </summary>
        /// <param name="sneder"></param>
        /// <param name="e"></param>
        private void mKeyboardHook_KeyDownEvent(object sneder, KeyEventArgs e) {

        }

        /// <summary>
        /// 全域鍵盤放開事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mKeyboardHook_KeyUpEvent(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Delete://MapGL刪除Goal點快捷鍵
                    /*-- 有可能標示物被刪除，通知GoalSetting界面更新 --*/
                    OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
                    break;
            }
        }

        #endregion KeyboardHook

        #region SerialClient

        /// <summary>
        /// 序列化通訊接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSerialClient_ReceiveData(object sender, ReceiveDataEventArgs e) {
            if (e.Data is IProductPacket) {
                var product = e.Data as IProductPacket;
                /*-- 查詢是否有等待該封包 --*/
                var cmdSrc = mCmdTsk.Find(v => v.SerialNumber == product.SerialNumber);
                if (cmdSrc != null) {
                    cmdSrc.SetResult(product);
                } else {
                    switch (product.Purpose) {
                        case EPurpose.AutoReportLaser:
                            var laser = product.ToIAutoReportLaser().Product;
                            if (laser != null) {
                                DrawLaser(product.ToIAutoReportLaser().Product);
                            } else {
                                IsAutoReport = false;
                                Database.AGVGM[mAGVID].LaserAPoints.DataList.Clear();
                                Database.AGVGM[mAGVID].Path.DataList.Clear();
                            }
                            break;
                        case EPurpose.AutoReportPath: {
                                var path = product.ToIAutoReportPath().Product;
                                if (path != null) {
                                    DrawPath(path);
                                }
                                break;
                            }
                        case EPurpose.AutoReportStatus:
                            Status = product.ToIAutoReportStatus()?.Product;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 連線狀態變更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSerialClient_OnConnectChange(object sender, ConnectStatusChangeEventArgs e) {
            OnPropertyChanged(PropertyDeclaration.IsConnected);

            OnConsoleMessage($"Client - Is {(e.IsConnected ? "Connected" : "Disconnected")} to {e.IP}:{e.Port}");
            if (e.IsConnected) {
                HostIP = e.IP;
                DoPositionComfirm();
                Status = RequestStatus();
            } else {
                if (mSerialClient != null) {
                    mSerialClient.Dispose();
                    mSerialClient = null;
                }
            }
        }

        #endregion SerialClient

        #endregion Funciton - Events

        #region Funciton - Public Methods

        /// <summary>
        /// 系統初始化
        /// </summary>
        public void Initial() {
            /*-- 載入AVG物件 --*/
            if (!Database.AGVGM.ContainsID(mAGVID)) {
                Database.AGVGM.Add(mAGVID, FactoryMode.Factory.AGV(0, 0, 0, "AGV"));
            }

            /*-- 開啟全域鍵盤鉤子 --*/
            mKeyboardHook.KeyDownEvent += mKeyboardHook_KeyDownEvent;
            mKeyboardHook.KeyUpEvent += mKeyboardHook_KeyUpEvent;
            mKeyboardHook.Start();

            /*-- 委派廣播接收事件 --*/
            mBroadcast.ReceivedData += mBroadcast_ReceivedData;
        }

        /// <summary>
        /// 搜尋可用的iTS設備
        /// </summary>
        public void FindCar() {
            if (!mBroadcast.IsReceiving) {
                Task.Run(() => {
                    /*-- 開啟廣播接收 --*/
                    mBroadcast.StartReceive(true);
                    OnConsoleMessage("[Planner]: Start searching iTS.");
                    /*-- 清除iTS清單 --*/
                    mAgvList.Clear();
                    /*-- 廣播要求iTS回應 --*/
                    for (int i = 0; i < 3; i++) {
                        mBroadcast.Send("Count off");
                        Thread.Sleep(30);
                    }
                    /*-- 等待iTS回應完畢後停止接收回應 --*/
                    Thread.Sleep(2000);
                    mBroadcast.StartReceive(false);
                    /*-- 反饋至UI --*/
                    string msg = $"Find {iTSs.Count} iTS";
                    OnConsoleMessage($"[Planner]:{msg}");
                    SetBalloonTip("Search iTS", msg);
                    OnPropertyChanged(PropertyDeclaration.iTSs);
                });
            }
        }
        
        /// <summary>
        /// 清除地圖
        /// </summary>
        public void ClearMap() {
            try {
                Database.ClearAllButAGV();
                Database.AGVGM[mAGVID].LaserAPoints.DataList.Clear();
                Database.AGVGM[mAGVID].Path.DataList.Clear();
                OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }
        
        /// <summary>
        /// 與指定IP iTS連線/斷線
        /// </summary>
        /// <param name="cnn">連線/斷線</param>
        /// <param name="hostIP">AGV IP</param>
        /// <exception cref=""
        public virtual void ConnectToITS(bool cnn, string hostIP = "") {
            try {
                if (IsConnected != cnn) {
                    if (cnn) {//連線至VC
                        /*-- 實例化物件 --*/
                        if (mSerialClient == null) {
                            mSerialClient = FactoryMode.Factory.SerialClient(mSerialClient_ReceiveData, mBypassSocket);
                            mSerialClient.ConnectChange += mSerialClient_OnConnectChange;
                        }
                        /*-- IP格式驗證 --*/
                        if (!VerifyIP(hostIP)) {
                            throw new FormatException($"{hostIP}是錯誤IP格式");
                        }
                        /*-- 測試IP是否存在 --*/
                        PingStatus pingStt = PingStatus.Unknown;
                        if ((pingStt = CtNetwork.Ping(hostIP, 500).PingState) != PingStatus.Success) {
                            throw new PingException(pingStt.ToString());
                        }
                        /*-- 連線至VehicleConsole --*/
                        mSerialClient.Connect(hostIP, (int)EPort.VehiclePlanner);
                    } else {//斷開與VehicleConsole的連線
                        mSerialClient.Stop();
                    }
                }
            } catch (PingException pe) {
                OnConsoleMessage($"Ping fail:{pe.Message}");
                SetBalloonTip("Connect failed", pe.Message);
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
                SetBalloonTip("Connect failed", ex.Message);
                if (mSerialClient != null) {
                    mSerialClient.Dispose();
                    mSerialClient = null;
                }
            }
        }

        /// <summary>
        /// 載入檔案
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        public void LoadFile(FileType type, string fileName) {
            try {
                bool isLoaded = false;
                switch (type) {
                    case FileType.Ori:
                        isLoaded = LoadOri(fileName);
                        if (isLoaded) {
                            Database.AGVGM[mAGVID]?.LaserAPoints.DataList.Clear();
                        }
                        break;
                    case FileType.Map:
                        isLoaded = LoadMap(fileName);
                        break;
                    default:
                        throw new ArgumentException($"無法載入未定義的檔案類型{type}");
                }
                if (isLoaded) {
                    SetBalloonTip($"Load { type}", $"\'{fileName}\' is loaded");
                } else {
                    OnErrorMessage("File data is wrong, can not read");
                }
            } catch (Exception ex) {
                OnErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 取得Ori檔
        /// </summary>
        public void GetOri() {
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
                    SetBalloonTip("Donwload", $"{oriName}.ori is downloaded {(success == true ? "successfully" : "failed")} ");
                }
            }
        }

        /// <summary>
        /// 取得Map檔
        /// </summary>
        public void GetMap() {
            bool? success = null;
            string mapName = null;
            try {
                string mapList = RequestMapList();
                if (!string.IsNullOrEmpty(mapList)) {
                    mapName = SelectFile(mapList);
                    if (!string.IsNullOrEmpty(mapName)) {
                        var map = RequestMapFile(mapName);
                        if (map != null) {
                            if (map.SaveAs(@"D:\Mapinfo\Client")) {
                                success = true;
                                OnConsoleMessage($"Planner - {map.Name} download completed");
                            } else {
                                success = false;
                                OnConsoleMessage($"Planner - {map.Name} failed to save ");
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            } finally {
                if (success != null) {
                    SetBalloonTip("Donwload", $"{mapName}.ori is downloaded {(success == true ? "successfully" : "failed")} ");
                }
            }
        }

        /// <summary>
        /// 要求雷射資料
        /// </summary>
        /// <returns>雷射資料(0筆雷射資料表示失敗)</returns>
        public void RequestLaser() {
            var laser = Send(FactoryMode.Factory.Order().RequestLaser())?.ToIRequestLaser()?.Product;
            try {
                if (laser != null) {
                    if (laser.Count > 0) {
                        OnConsoleMessage($"iTS - Received {laser.Count} laser data");
                        DrawLaser(laser);
                    } else {
                        OnConsoleMessage($"iTS - Laser data request failed");
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        public void SetServoMode(bool servoOn) {
            try {
                var servoOnStt = Send(FactoryMode.Factory.Order().SetServoMode(servoOn))?.ToISetServoMode()?.Product;
                if (servoOnStt != null) {
                    OnConsoleMessage($"iTS - Is Servo{(servoOn ? "On" : "Off")}");
                    IsMotorServoOn = (bool)servoOnStt;
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 設定iTS工作移動速度
        /// </summary>
        /// <param name="velocity">移動速度</param>
        /// <returns>是否設定成功</returns>
        public void SetWorkVelocity(int velocity) {
            var success = Send(FactoryMode.Factory.Order().SetWorkVelocity(velocity))?.ToISetWorkVelocity()?.Product;
            try {
                if (success == true) {
                    Velocity = velocity;
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 進行位置矯正
        /// </summary>
        /// <returns>地圖相似度</returns>
        public void DoPositionComfirm() {
            var similarity = Send(FactoryMode.Factory.Order().DoPositionComfirm())?.ToIDoPositionComfirm()?.Product;
            try {
                if (similarity != null) {
                    if (similarity >= 0 && similarity <= 1) {
                        mSimilarity = (double)similarity;
                        OnConsoleMessage($"iTS - The map similarity is {mSimilarity:0.0%}");
                    } else if (mSimilarity == -1) {
                        mSimilarity = (double)similarity;
                        OnConsoleMessage($"iTS - The map is now matched");
                    } else {
                        OnConsoleMessage($"iTS - The map similarity is 0%");
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 移至Goal(透過Goal點索引)
        /// </summary>
        /// <param name="goalIndex">Goal點索引</param>
        /// <returns>是否成功開始移動</returns>
        public void DoRunningByGoalIndex(uint goalID) {
            try {
                int index = Database.GoalGM.IndexOf(goalID);
                if (index >= 0) {
                    var goal = Database.GoalGM[goalID];
                    var success = Send(FactoryMode.Factory.Order().DoRunningByGoalIndex(index))?.ToIDoRunningByGoalIndex()?.Product;
                    if (success == true) {
                        OnConsoleMessage($"iTS - Start moving to {goal.Name}");
                    } else if (success == false) {
                        OnConsoleMessage($"Move to goal failure");
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 到指定充電站進行充電
        /// </summary>
        /// <param name="powerIndex">充電站索引</param>
        /// <returns>是否開始進行充電</returns>
        public void DoCharging(uint powerID) {
            try {
                int index = Database.PowerGM.IndexOf(powerID);
                if (index >= 0) {
                    var power = Database.PowerGM[powerID];
                    var success = Send(FactoryMode.Factory.Order().DoCharging(index))?.ToIDoCharging()?.Product;
                    if (success == true) {
                        OnConsoleMessage($"iTS - Begin charging at {power.Name}");
                    } else if (success == false) {
                        OnConsoleMessage("iTS - Charging failed");
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 要求Map檔清單
        /// </summary>
        /// <returns>Map檔清單</returns>
        public string RequestMapList() {
            var mapList = Send(FactoryMode.Factory.Order().RequestMapList())?.ToIRequestMapList()?.Product;
            return mapList != null ? string.Join(",", mapList) : null;
        }

        /// <summary>
        /// 要求Map檔
        /// </summary>
        /// <param name="mapName">要求的Map檔名</param>
        /// <returns>Map檔</returns>
        public IDocument RequestMapFile(string mapName) {
            var mapFile = Send(FactoryMode.Factory.Order().RequestMapFile(mapName))?.ToIRequestMapFile()?.Product;
            return mapFile;
        }

        public void StartScan(bool scan) {
            try {
                bool? isScanning = null;
                if (mIsScanning != scan) {
                    if (scan) {//開始掃描
                        if (mStatus?.Description == EDescription.Idle) {
                            string oriName = string.Empty;
                            if (Stat.SUCCESS == CtInput.Text(out oriName, "MAP Name", "Set Map File Name")) {
                                isScanning = SetScanningOriFileName(oriName);
                            }
                            if (isScanning == true) {
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
                        IsScanning = (bool)isScanning;
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage("Error:" + ex.Message);
            }
        }

        /// <summary>
        /// 傳送並要求載入Map
        /// </summary>
        /// <param name="mapPath">目標Map檔路徑</param>
        public void SendAndSetMap(string mapPath) {
            var success = UploadMapToAGV(mapPath);
            string mapName = Path.GetFileName(mapPath);
            if (success == true) {
                SetBalloonTip("Donwload", $"{mapName}.ori is downloaded {(success == true ? "successfully" : "failed")} ");
                OnConsoleMessage($"iTS - The {mapName} uploaded");
                success = ChangeMap(mapName);
                if (success == true) {
                    OnConsoleMessage($"iTS - The {mapName} is now running");
                } else if (success == false) {
                    OnConsoleMessage($"iTS - The {mapName} failed to run");
                }
            } else if (success == false) {
                OnConsoleMessage($"iTS - The {mapName} upload failed");
            }
        }

        /// <summary>
        /// 切換資料自動回傳
        /// </summary>
        public void AutoReport(bool auto) {
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

        /// <summary>
        /// 移動控制
        /// </summary>
        /// <param name="direction">移動方向</param>
        /// <param name="velocity">移動速度</param>
        public void MotionContorl(MotionDirection direction) {
            try {
                bool? isManualMoving = null;
                if (direction == MotionDirection.Stop) {
                    isManualMoving = StartManualControl(false);
                } else {
                    if (SetManualVelocity(direction) == true) {
                        OnConsoleMessage($"iTS - is {direction},Velocity is {mVelocity}");
                        isManualMoving = StartManualControl(true);
                    }
                }
                if (isManualMoving != null && isManualMoving != mIsManualMoving) {
                    mIsManualMoving = (bool)isManualMoving;
                    OnConsoleMessage($"iTS - {(mIsManualMoving ? "Start" : "Stop")} moving");
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        public void SimplifyOri() {
            try {
                if (mBypassLoadFile) {
                    /*-- 空跑模擬SimplifyOri --*/
                    SpinWait.SpinUntil(() => false, 1000);
                    return;
                }
                string[] tmpPath = CurOriPath.Split('.');
                CurMapPath = tmpPath[0] + ".map";
                Database.Save(CurMapPath);
                OnVehiclePlanner(VehiclePlannerEvents.NewMap);
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        public void SaveMap() {
            if (!string.IsNullOrEmpty(CurMapPath)) {
                OnConsoleMessage("[Map is Save]");
                Database.Save(CurMapPath);
            }
        }

        /// <summary>
        /// 刪除指定標記物
        /// </summary>
        /// <param name="markers"></param>
        public void DeleteMarker(IEnumerable<uint> markers) {
            foreach (var id in markers) {
                if (Database.GoalGM.ContainsID(id)) {
                    Database.GoalGM.Remove(id);
                } else if (Database.PowerGM.ContainsID(id)) {
                    Database.PowerGM.Remove(id);
                }
            }
            OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
        }

        /// <summary>
        /// 清除標記物
        /// </summary>
        public void ClearMarker() {
            OnConsoleMessage("[Clear Goal]");
            Database.GoalGM.Clear();
            OnConsoleMessage("[Clear Power]");
            Database.PowerGM.Clear();
            OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
        }

        /// <summary>
        /// 新增當前位置為Goal點
        /// </summary>
        public void AddCurrentAsGoal() {
            /*-- 取得當前位置 --*/
            var currentPosition = Database.AGVGM[mAGVID].Data;
            /*-- 建構Goal點 --*/
            var goal = FactoryMode.Factory.Goal(currentPosition, $"Goal{Database.GoalGM.Count}");
            /*-- 分配ID --*/
            var goalID = Database.ID.GenerateID();
            /*-- 將Goal點資料加入Goal點管理集合 --*/
            Database.GoalGM.Add(goalID, goal);
            /*-- 重新載入Goal點資訊 --*/
            OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
        }

        /// <summary>
        /// 顯示當前Goal點名稱清單
        /// </summary>
        public void GetGoalNames() {
            try {
                var goalList = RequestGoalList();
                if (!string.IsNullOrEmpty(goalList)) {
                    OnConsoleMessage($"iTS - GoalNames:{goalList}");
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
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
                Database.AGVGM[mAGVID].SetLocation(position);
                OnConsoleMessage($"iTS - The position are now at {position}");
            }
        }

        public void FindPath(uint id) {
            try {
                int index = Database.GoalGM.IndexOf(id);
                if (index >= 0) {
                    var path = RequestPath(index);
                    if (path != null) {
                        var goal = Database.GoalGM[id];
                        if (path.Count > 0) {
                            OnConsoleMessage($"iTS - The path to {goal.Name} is completion. The number of path points is {path.Count}");
                        } else {
                            OnConsoleMessage($"iTS - Can not plan the path to  {goal.Name}");
                        }
                    }
                }
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        #endregion Funciton - Public Mehtods

        #region Funciton - Private Methods

        #region Riase Events

        /// <summary>
        /// 屬性變更事件發報
        /// </summary>
        /// <param name="prop"></param>
        protected virtual void OnPropertyChanged(string prop) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Console訊息發報
        /// </summary>
        /// <param name="msg"></param>
        protected virtual void OnConsoleMessage(string msg) {
            ConsoleMessage?.Invoke(msg);
        }

        /// <summary>
        /// VehiclePlanner事件發報
        /// </summary>
        /// <param name="events"></param>
        protected virtual void OnVehiclePlanner(VehiclePlannerEvents events) {
            VehiclePlannerEvent?.Invoke(this, new VehiclePlannerEventArgs(events));
        }

        /// <summary>
        /// Error事件發報
        /// </summary>
        /// <param name="err"></param>
        protected virtual void OnErrorMessage(string err) {
            ErrorMessage?.Invoke(err);
        }

        /// <summary>
        /// 氣球提示發報
        /// </summary>
        /// <param name="title"></param>
        /// <param name="context"></param>
        protected virtual void SetBalloonTip(string title, string context) {

        }

        #endregion Raise Events

        /// <summary>
        /// 序列傳輸命令
        /// </summary>
        /// <param name="packet">序列命令</param>
        /// <returns>是否傳輸成功</returns>
        private IProductPacket Send(IBasicPacket packet) {
            IProductPacket product = null;//回應封包
            Task tsk = null;//等待逾時執行緒
            CtTaskCompletionSource<IProductPacket> tskCompSrc = null;//封包接受完成觸發源
            /*-- 檢查封包 --*/
            if (packet == null) {
                OnConsoleMessage("The packet is null, unable to send");
                return null;
            }
            /*--檢查連線--*/
            if (!IsConnected) {
                OnConsoleMessage("Is not connected, unable to send");
                return null;
            }
            if (packet is IOrderPacket) {
                string cmdTitle = $"{packet.Purpose}({packet.SerialNumber}):";
                /*-- 檢查是否沒有在等待回應 --*/
                if (!mCmdTsk.Exists(v => v.Purpose == packet.Purpose)) {
                    /*-- 加入回應等待任務 --*/
                    tskCompSrc = new CtTaskCompletionSource<IProductPacket>(packet as IOrderPacket);
                    mCmdTsk.Add(tskCompSrc);
                    /*-- 等待回應 --*/
                    tsk = Task.Run(() => {
                        bool isTimeout = !tskCompSrc.Task.Wait(mTimeOut);
                        /*--從等待清單中刪除--*/
                        mCmdTsk.Remove(tskCompSrc);
                        if (!isTimeout) {
                            if (tskCompSrc.Task.IsCompleted) {
                                if (tskCompSrc.Task.Result != null) {
                                    product = tskCompSrc.Task.Result;
                                    OnConsoleMessage($"{cmdTitle} response is received");
                                } else {
                                    OnConsoleMessage($"{cmdTitle} response is null");
                                }
                            }
                        } else {
                            OnConsoleMessage($"{cmdTitle} response timeout");
                        }
                    });
                } else {//已在等待回應
                    OnConsoleMessage($"{cmdTitle}Waiting for the iTS to respond");
                    return null;
                }
            }
            /*-- 發送命令 --*/
            if (!mSerialClient.Send(packet)) tskCompSrc?.SetResult(null);
            /*-- 等回應接收完畢 --*/
            tsk?.Wait();
            return product;
        }

        ///<summary>IP驗證</summary>
        ///<param name="ip">要驗證的字串</param>
        ///<returns>True:合法IP/False:非法IP</returns>
        private bool VerifyIP(string ip) {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 繪製雷射
        /// </summary>
        /// <param name="laser"></param>
        protected void DrawLaser(IEnumerable<IPair> laser) {
            Database.AGVGM[mAGVID]?.LaserAPoints.DataList.Replace(laser);
        }

        /// <summary>
        /// 繪製路徑
        /// </summary>
        /// <param name="path"></param>
        protected void DrawPath(IEnumerable<IPair> path) {
            Database.AGVGM[mAGVID].Path.DataList.Replace(path);
        }

        private bool? SetManualVelocity(MotionDirection direction) {
            int r = 0, l = 0, v = mVelocity;
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
            return SetManualVelocity(FactoryMode.Factory.Pair(l, r));
        }


        /// <summary>
        /// 要求Ori檔清單
        /// </summary>
        /// <returns>Ori檔清單</returns>
        private string RequestOriList() {
            var oriList = Send(FactoryMode.Factory.Order().RequestOriList())?.ToIRequestOriList()?.Product;
            return oriList != null ? string.Join(",", oriList) : null;
        }

        /// <summary>
        /// 要求Ori檔
        /// </summary>
        /// <param name="oriName">Ori檔名</param>
        /// <returns>Ori檔</returns>
        private IDocument RequestOriFile(string oriName) {
            var oriFile = Send(FactoryMode.Factory.Order().RequestOriFile(oriName))?.ToIRequestOriFile()?.Product;
            return oriFile;
        }

        /// <summary>
        /// 上傳Map檔
        /// </summary>
        /// <param name="mapPath">要上傳的Map檔路徑</param>
        /// <returns>是否上傳成功</returns>
        private bool? UploadMapToAGV(string mapPath) {
            var success = Send(FactoryMode.Factory.Order().UploadMapToAGV(mapPath))?.ToIUploadMapToAGV()?.Product;
            return success;
        }

        /// <summary>
        /// 要求iTS載入指定的Map檔
        /// </summary>
        /// <param name="mapName">要載入的Map檔名</param>
        /// <returns>是否切換成功</returns>
        private bool? ChangeMap(string mapName) {
            var success = Send(FactoryMode.Factory.Order().ChangeMap(mapName))?.ToIChangeMap()?.Product;
            return success;
        }

        /// <summary>
        /// 要求Goal點清單
        /// </summary>
        /// <returns>Goal點清單</returns>
        private string RequestGoalList() {
            var goalList = Send(FactoryMode.Factory.Order().RequestGoalList())?.ToIRequestGoalList()?.Product;
            return goalList != null ? string.Join(",", goalList) : null;
        }

        /// <summary>
        /// 要求到指定Goal點的路徑
        /// </summary>
        /// <param name="goalIndex">目標Goal點索引</param>
        /// <returns>路徑(Count為0表示規劃失敗)</returns>
        private List<IPair> RequestPath(int goalIndex) {
            var path = Send(FactoryMode.Factory.Order().RequestPath(goalIndex))?.ToIRequestPath()?.Product;
            return path;
        }

        /// <summary>
        /// 要求自動回報iTS狀態
        /// </summary>
        /// <param name="on">是否自動回報</param>
        /// <returns>iTS狀態</returns>
        private IStatus AutoReportStatus(bool on) {
            var status = Send(FactoryMode.Factory.Order().AutoReportStatus(on))?.ToIAutoReportStatus()?.Product;
            return status;
        }

        /// <summary>
        /// 要求自動回傳雷射資料
        /// </summary>
        /// <param name="on">是否自動回報</param>
        /// <returns>雷射資料</returns>
        private List<IPair> AutoReportLaser(bool on) {
            var laser = Send(FactoryMode.Factory.Order().AutoReportLaser(on))?.ToIAutoReportLaser()?.Product;
            return laser;
        }

        /// <summary>
        /// 要求自動回傳路徑
        /// </summary>
        /// <param name="on">是否自動回報</param>
        /// <returns>路徑資料</returns>
        private List<IPair> AutoReportPath(bool on) {
            var path = Send(FactoryMode.Factory.Order().AutoReportPath(on))?.ToIAutoReportPath()?.Product;
            return path;
        }

        /// <summary>
        /// 設定iTS當前位置
        /// </summary>
        /// <returns>是否設定成功</returns>
        private bool? SetPosition(ITowardPair position) {
            var success = Send(FactoryMode.Factory.Order().SetPosition(position))?.ToISetPosition()?.Product;
            return success;
        }

        /// <summary>
        /// 開始手動控制
        /// </summary>
        /// <param name="start">是否開始手動控制</param>
        /// <remarks>是否為手動控制狀態</remarks>
        private bool? StartManualControl(bool start) {
            var isManual = Send(FactoryMode.Factory.Order().StartManualControl(start))?.ToIStartManualControl()?.Product;
            return isManual;
        }

        /// <summary>
        /// 設定手動控制移動速度(方向)
        /// </summary>
        /// <param name="velocity">手動移動速度</param>
        /// <returns>是否設定成功</returns>
        private bool? SetManualVelocity(IPair velocity) {
            var success = Send(FactoryMode.Factory.Order().SetManualVelocity(velocity))?.ToISetManualVelocity()?.Product;
            return success;
        }

        /// <summary>
        /// 停止掃描地圖
        /// </summary>
        /// <returns>是否在掃描中</returns>
        private bool? StopScanning() {
            var isScanning = Send(FactoryMode.Factory.Order().StopScanning())?.ToIStopScanning()?.Product;
            return isScanning;
        }

        /// <summary>
        /// 設定地圖檔名
        /// </summary>
        /// <remarks>是否在掃描中</remarks>
        private bool? SetScanningOriFileName(string oriName) {
            var isScanning = Send(FactoryMode.Factory.Order().SetScanningOriFileName(oriName))?.ToISetScanningOriFileName()?.Product;
            return isScanning;
        }

        /// <summary>
        /// 載入地圖
        /// </summary>
        /// <param name="mapPath">Map檔路徑</param>
        private bool LoadMap(string mapPath) {
            bool isLoaded = true;
            mCurMapPath = mapPath;
            string path = CtFile.GetFileName(mapPath);
            if (mBypassLoadFile) {
                /*-- 空跑1秒模擬載入Map檔 --*/
                SpinWait.SpinUntil(() => false, 1000);
            } else {
                //#region - Retrive information from .map file -
                /*-- 地圖清空 --*/
                OnVehiclePlanner(VehiclePlannerEvents.NewMap);
                /*-- 載入Map並取得Map中心點 --*/
                var center = Database.LoadMapToDatabase(mCurMapPath)?.Center();
                if (center != null) {
                    MapCenter = center;
                    OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
                    if (IsConnected) {
                        SendAndSetMap(mapPath);
                    }
                } else {
                    isLoaded = false;
                }
            }
            return isLoaded;
        }

        private bool LoadOri(string oriPath) {
            bool isLoaded = true;
            CurOriPath = oriPath;
            OnVehiclePlanner(VehiclePlannerEvents.NewMap);
            if (!mBypassLoadFile) {//無BypassLoadFile
                /*-- 載入Map並取得Map中心點 --*/
                IPair center = Database.LoadOriToDatabase(CurOriPath, mAGVID)?.Center();
                if (center != null) {
                    MapCenter = center;
                } else {
                    isLoaded = false;
                }
            } else {//Bypass LoadFile功能
                    /*-- 空跑一秒，模擬檔案載入 --*/
                SpinWait.SpinUntil(() => false, 1000);
            }
            return isLoaded;
        }

        /// <summary>
        /// 要求iTS狀態
        /// </summary>
        /// <returns>iTS狀態</returns>
        private IStatus RequestStatus() {
            var status = Send(FactoryMode.Factory.Order().RequestStatus())?.ToIRequestStatus()?.Product;
            return status;
        }

        #endregion Funciotn - Private Methods

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: 處置 Managed 狀態 (Managed 物件)。


                    mKeyboardHook.KeyUpEvent -= mKeyboardHook_KeyUpEvent;
                    mKeyboardHook.Stop();
                }

                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        // ~CtVehiclePlanner() {
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
    
}
