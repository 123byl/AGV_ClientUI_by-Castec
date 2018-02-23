using AGVDefine;
using BroadCast;
using CtLib.Forms;
using CtLib.Library;
using CtLib.Module.Utility;
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

using Geometry;

namespace VehiclePlanner.Core {
    
    /// <summary>
    /// 系統主體
    /// </summary>
    internal partial class CtVehiclePlanner : ICtVehiclePlanner {
        
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
        ///     1.0.1   Jay [2018/02/23]
        ///         \ 將底層抽離為獨立專案
        ///         \ 降低與AGVBase之間的耦合性
        /// </remarks>
        public CtVersion Version { get { return new CtVersion(1, 0, 1, "2018/02/23", "Jay Chang"); } }

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

        #endregion Funciton - Events

        #region Funciton - Public Methods

        /// <summary>
        /// 系統初始化
        /// </summary>
        public void Initial() {
            /*-- 建構物件 --*/
            mITS = new iTSControllerSerial();
            mMapGL = MapGLController.GetInstance();
            mKeyboardHook = new KeyboardHook();
            mBroadcast = new Broadcaster();

            mMapGL.Initial();
            
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
                mMapGL.ClearAll();
                OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
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
                            mMapGL.ClearLaser();
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
                    if (IsConnected && type == FileType.Map) {
                        mITS.SendAndSetMap(fileName);
                    }
                } else {
                    OnErrorMessage("File data is wrong, can not read");
                }
            } catch (Exception ex) {
                OnErrorMessage(ex.Message);
            }
        }
        
        /// <summary>
        /// 將ori檔轉為map檔
        /// </summary>
        public void SimplifyOri() {
            try {
                if (mBypassLoadFile) {
                    /*-- 空跑模擬SimplifyOri --*/
                    SpinWait.SpinUntil(() => false, 1000);
                    return;
                }
                string[] tmpPath = CurOriPath.Split('.');
                CurMapPath = tmpPath[0] + ".map";
                mMapGL.Save(CurMapPath);
                OnVehiclePlanner(VehiclePlannerEvents.NewMap);
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 儲存地圖
        /// </summary>
        public void SaveMap() {
            if (!string.IsNullOrEmpty(CurMapPath)) {
                OnConsoleMessage("[Map is Save]");
                mMapGL.Save(CurMapPath);
            }
        }

        /// <summary>
        /// 刪除指定標記物
        /// </summary>
        /// <param name="markers"></param>
        public void DeleteMarker(IEnumerable<uint> markers) {
            foreach (var id in markers) {
                if (mMapGL.ContainGoal(id)) {
                    mMapGL.RemoveGoal(id);
                } else if (mMapGL.ContainPower(id)) {
                    mMapGL.RemovePower(id);
                }
            }
            OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
        }

        /// <summary>
        /// 清除標記物
        /// </summary>
        public void ClearMarker() {
            OnConsoleMessage("[Clear Goal]");
            mMapGL.ClearGoal();
            OnConsoleMessage("[Clear Power]");
            mMapGL.ClearPower();
            OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
        }

        /// <summary>
        /// 新增當前位置為Goal點
        /// </summary>
        public void AddCurrentAsGoal() {
            /*-- 取得當前位置 --*/
            var currentPosition = mMapGL.Location;
            /*-- 建構Goal點 --*/
            var goal = MapGLController.NewGoal(currentPosition);
            /*-- 分配ID --*/
            var goalID = MapGLController.GenerateID();
            /*-- 將Goal點資料加入Goal點管理集合 --*/
            mMapGL.AddGoal(goalID, goal);
            /*-- 重新載入Goal點資訊 --*/
            OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
        }
        
        #endregion Funciton - Public Mehtods

        #region Funciton - Private Methods

        #region Riase Events

        /// <summary>
        /// 屬性變更事件發報
        /// </summary>
        /// <param name="prop"></param>
        protected virtual void OnPropertyChanged(string prop) {
            mPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Console訊息發報
        /// </summary>
        /// <param name="msg"></param>
        protected virtual void OnConsoleMessage(string msg) {
            mConsoleMessage?.Invoke(msg);
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
            mBalloonTip?.Invoke(title,context);
        }

    #endregion Raise Events

        /// <summary>
        /// 載入Map檔
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
                var center = mMapGL.LoadMap(mCurMapPath)?.Center();
                if (center != null) {
                    MapCenter = center;
                    OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
                } else {
                    isLoaded = false;
                }
            }
            return isLoaded;
        }

        /// <summary>
        /// 載入Ori檔
        /// </summary>
        /// <param name="oriPath"></param>
        /// <returns></returns>
        private bool LoadOri(string oriPath) {
            bool isLoaded = true;
            CurOriPath = oriPath;
            OnVehiclePlanner(VehiclePlannerEvents.NewMap);
            if (!mBypassLoadFile) {//無BypassLoadFile
                /*-- 載入Map並取得Map中心點 --*/
                IPair center = mMapGL.LoadOri(CurMapPath)?.Center();
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
