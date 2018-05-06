using CtBind;
using CtDockSuit;
using Geometry;
using GLUI;
using VehiclePlanner.Core;
using static VehiclePlanner.Partial.VehiclePlannerUI.Events.ConsoleEvents;
using static VehiclePlanner.Partial.VehiclePlannerUI.Events.GoalSettingEvents;
using static VehiclePlanner.Partial.VehiclePlannerUI.Events.TestingEvents;

namespace VehiclePlanner.Module.Interface {
    ///// <summary>
    ///// DockContnet接口
    ///// </summary>
    //public interface ICtDockContainer : IDisposable {
    //    #region Properties

    //    /// <summary>
    //    /// 預設停靠狀態
    //    /// </summary>
    //    DockState DefaultDockState { get; set; }

    //    /// <summary>
    //    /// 表單固定尺寸
    //    /// </summary>
    //    Size FixedSize { get; set; }

    //    /// <summary>
    //    /// 是否可視
    //    /// </summary>
    //    bool Visible { get; set; }

    //    #endregion Properties

    //    #region Methods

    //    /// <summary>
    //    /// 分配<see cref="DockPanel"/>物件參考
    //    /// </summary>
    //    /// <param name="dockPanel"></param>
    //    void AssignmentDockPanel(DockPanel dockPanel);

    //    ///// <summary>
    //    ///// 隱藏視窗
    //    ///// </summary>
    //    //void HideWindow();

    //    ///// <summary>
    //    ///// 依照預設停靠狀態顯示
    //    ///// </summary>
    //    //void ShowWindow();

    //    #endregion Methods

    //    #region 原本就有實作的方法、屬性
    //    DockState DockState { get; set; }
    //    event EventHandler DockStateChanged;
    //    string Text { get; set; }
    //    #endregion

    //}

    /// <summary>
    /// Console 視窗公開操作方法
    /// </summary>
    public interface IConsole : ICtDockContainer, IDataDisplay<ICtVehiclePlanner> {

        /// <summary>
        /// 加入文字
        /// </summary>
        event DelConsoleAdded ConsoleAddedEvent;

        /// <summary>
        /// 顯示文字被清除
        /// </summary>
        event DelConsoleCleared ConsoleClearedEvent;

        /// <summary>
        /// 換行並加入字串
        /// </summary>
        void AddMsg(string msg);

        /// <summary>
        /// 換行並加入字串
        /// </summary>
        void AddMsg(string format, params object[] arg);

        /// <summary>
        /// 清除顯示
        /// </summary>
        void ClearMsg();
    }

    /// <summary>
    /// GoalSetting 視窗公開操作方法
    /// </summary>
    public interface IGoalSetting : ICtDockContainer, IDataDisplay<ICtVehiclePlanner> {

        /// <summary>
        /// 加入 Goal 點
        /// </summary>
        event DelAddCurrentGoal AddCurrentGoalEvent;

        /// <summary>
        /// 清除所有目標點
        /// </summary>
        event DelClearGoals ClearGoalsEvent;

        /// <summary>
        /// 刪除
        /// </summary>
        event DelDeleteSingle DeleteSingleEvent;

        /// <summary>
        /// 尋找路徑
        /// </summary>
        event DelFindPath FindPathEvent;

        /// <summary>
        /// 載入地圖
        /// </summary>
        event DelLoadMap LoadMapEvent;

        /// <summary>
        /// 從 AGV 下載地圖
        /// </summary>
        event DelLoadMapFromAGV LoadMapFromAGVEvent;

        /// <summary>
        /// 移動
        /// </summary>
        event DelRunGoal RunGoalEvent;

        /// <summary>
        /// 按照順序移動全部
        /// </summary>
        event DelRunLoop RunLoopEvent;

        /// <summary>
        /// 儲存
        /// </summary>
        event DelSaveGoal SaveGoalEvent;

        /// <summary>
        /// 上傳地圖
        /// </summary>
        event DelSendMapToAGV SendMapToAGVEvent;

        /// <summary>
        /// 取得所有Goal點名稱
        /// </summary>
        event DelGetGoalNames GetGoalNames;

        event DelCharging Charging;

        event DelClearMap ClearMap;

        /// <summary>
        /// 目標點個數
        /// </summary>
        int GoalCount { get; }

        /// <summary>
        /// 設定真實座標
        /// </summary>
        void UpdateNowPosition(IPair realPos);

        /// <summary>
        /// 設定表單選擇項目
        /// </summary>
        void SetSelectItem(uint id);

        /// <summary>
        /// 重新載入標示物
        /// </summary>
        void ReloadSingle();
    }

    /// <summary>
    /// Test視窗公開操作方法
    /// </summary>
    public interface ITesting : ICtDockContainer, IDataDisplay<IITSController> {

        event DelConnect Connect;

        event DelGetCar GetCar;

        event DelGetLaser GetLaser;

        event DelGetMap GetMap;

        event DelGetOri GetOri;

        event DelLoadMap LoadMap;

        event DelLoadOri LoadOri;

        event DelMotorServoOn MotorServoOn;

        event DelSendMap SendMap;

        event DelSetVelocity SetVelocity;

        event DelSimplifyOri SimplifyOri;

        event DelClearMap ClearMap;

        event DelSettingCarPos SettingCarPos;

        event DelCarPosConfirm CarPosConfirm;

        event DelStartScan StartScan;

        event DelShowMotionController ShowMotionController;

        event DelFind Find;
    }

    /// <summary>
    /// MapGL視窗公開操作方法
    /// </summary>
    public interface IBaseMapGL : ICtDockContainer, IDataDisplay<ICtVehiclePlanner> {
        
        void Show();
    }
}