using CtBind;
using CtDockSuit;
using VehiclePlanner.Core;
using static VehiclePlanner.Partial.VehiclePlannerUI.Events.ConsoleEvents;
using static VehiclePlanner.Partial.VehiclePlannerUI.Events.GoalSettingEvents;
using static VehiclePlanner.Partial.VehiclePlannerUI.Events.TestingEvents;

namespace VehiclePlanner.Module.Interface {
    
    /// <summary>
    /// Console 視窗公開操作方法
    /// </summary>
    public interface IConsole : ICtDockContainer, IDataDisplay<IBaseVehiclePlanner> {

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
    public interface IBaseGoalSetting : ICtDockContainer, IDataDisplay<IBaseVehiclePlanner> {

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
        /// 設定表單選擇項目
        /// </summary>
        void SetSelectItem(uint id);

    }

    /// <summary>
    /// Test視窗公開操作方法
    /// </summary>
    public interface ITesting : ICtDockContainer, IDataDisplay<IBaseITSController> {

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
    public interface IBaseMapGL : ICtDockContainer, IDataDisplay<IBaseVehiclePlanner> {
        
        void Show();
    }
}