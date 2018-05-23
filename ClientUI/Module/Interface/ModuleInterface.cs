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

    }

    /// <summary>
    /// MapGL視窗公開操作方法
    /// </summary>
    public interface IBaseMapGL : ICtDockContainer,IDataDisplay<IBaseVehiclePlanner> , IDataDisplay<IBaseITSController> {
        
        void Show();
    }
}