using CtLib.Library;
using CtLib.Module.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace VehiclePlanner.Core {

    /// <summary>
    /// 系統主體
    /// </summary>
    public partial class BaseVehiclePlanner : IBaseVehiclePlanner {

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

        #region Funciont - Constructors

        public BaseVehiclePlanner() {
            
        }

        #endregion Function - Constructors

        #region Funciton - Public Methods

        /// <summary>
        /// 系統初始化
        /// </summary>
        public virtual void Initial() {            
            /*-- 開啟全域鍵盤鉤子 --*/
            mKeyboardHook.KeyDownEvent += mKeyboardHook_KeyDownEvent;
            mKeyboardHook.KeyUpEvent += mKeyboardHook_KeyUpEvent;
            mKeyboardHook.Start();
        }
        
        /// <summary>
        /// 清除地圖
        /// </summary>
        public virtual void ClearMap() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 載入檔案
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        public virtual void LoadFile(FileType type, string fileName) {
            throw new NotImplementedException();
        }

        protected virtual void SaveMap(string path) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 將ori檔轉為map檔
        /// </summary>
        public virtual void SimplifyOri() {
            try {
                if (mBypassLoadFile) {
                    /*-- 空跑模擬SimplifyOri --*/
                    SpinWait.SpinUntil(() => false, 1000);
                    return;
                }
                string[] tmpPath = CurOriPath.Split('.');
                CurMapPath = tmpPath[0] + ".map";
                SaveMap(CurMapPath);
                OnVehiclePlanner(VehiclePlannerEvents.NewMap);
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 儲存地圖
        /// </summary>
        public virtual void SaveMap() {
            if (!string.IsNullOrEmpty(CurMapPath)) {
                OnConsoleMessage("[Map is Save]");
                SaveMap(CurMapPath);
            }
        }
        
        

        /// <summary>
        /// 新增當前位置為Goal點
        /// </summary>
        public virtual void AddCurrentAsGoal() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 刪除指定標記物
        /// </summary>
        /// <param name="markers"></param>
        public virtual void DeleteMarker(IEnumerable<uint> markers) {
            throw new NotImplementedException();
        }

        #endregion Funciton - Public Mehtods

            #region Funciton - Private Methods

            #region Riase Events

            /// <summary>
            /// 屬性變更事件發報
            /// </summary>
            /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName="") {
            DelInvoke?.Invoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
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
            BalloonTip?.Invoke(title,context);
        }

    #endregion Raise Events

        /// <summary>
        /// 載入Map檔
        /// </summary>
        /// <param name="mapPath">Map檔路徑</param>
        protected virtual bool LoadMap(string mapPath) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 載入Ori檔
        /// </summary>
        /// <param name="oriPath"></param>
        /// <returns></returns>
        protected virtual bool LoadOri(string oriPath) {
            throw new NotImplementedException();
        }

        #endregion Funciotn - Private Methods

        #region Implement - IDataSource

        /// <summary>
        /// Invoke方法委派
        /// </summary>
        public Action<MethodInvoker> DelInvoke { get => mITS?.DelInvoke; set { if (mITS != null) mITS.DelInvoke = value; } }

        #endregion Implement - IDataSource

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: 處置 Managed 狀態 (Managed 物件)。


                    mKeyboardHook.KeyUpEvent -= mKeyboardHook_KeyUpEvent;
                    mKeyboardHook.KeyDownEvent -= mKeyboardHook_KeyDownEvent;
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
