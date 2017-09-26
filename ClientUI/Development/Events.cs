using System.Collections.Generic;
using static ClientUI.Events.ConsoleEvents;
using static ClientUI.Events.GoalSettingEvents;

namespace ClientUI
{
    /// <summary>
    /// Console 視窗公開操作方法
    /// </summary>
    public interface IIConsole
    {
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
        /// 清除顯示
        /// </summary>
        void ClearMsg();
    }

    /// <summary>
    /// GoalSetting 視窗公開操作方法
    /// </summary>
    public interface IIGoalSetting
    {
        /// <summary>
        /// 加入 Goal 點
        /// </summary>
        event DelAddNewGoal AddNewGoalEvent;

        /// <summary>
        /// 刪除
        /// </summary>
        event DelDeleteGoals DeleteGoalsEvent;

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
        /// 更新 Goal 點
        /// </summary>
        event DelUpdateGoal UpdateGoalEvent;


        /// <summary>
        /// 加入 Goal 點
        /// </summary>
        void AddGoal(Info goal);

        /// <summary>
        /// 移除目前 Goal 點並加入新的 goal 點
        /// </summary>
        void ClearAndAddGoals(IEnumerable<Info> goals);

        /// <summary>
        /// 移除所有 Goal 點
        /// </summary>
        void ClearGoal();

        /// <summary>
        /// 根據 ID 移除 Goal 點
        /// </summary>
        void DeleteGoal(int ID);

        /// <summary>
        /// 根據 ID 移除 Goal 點
        /// </summary>
        void DeleteGoals(IEnumerable<int> ID);

        /// <summary>
        /// 獲得所有 Goal 點資訊
        /// </summary>
        List<Info> GetGoals();

        /// <summary>
        /// 根據 ID 查詢 Goal 點
        /// </summary>
        Info GetGoalByID(int ID);

        /// <summary>
        /// 根據表單的列編號查詢 Goal
        /// </summary>
        Info GetGoalByIndex(int row);

        /// <summary>
        /// 更新 Goal 點
        /// </summary>
        void UpdateGoal(Info newGoal);
    }

    /// <summary>
    /// 點狀態資訊
    /// </summary>
    public struct Info
    {
        public Info(int id, string name, int x, int y, double toward)
        {
            ID = id;
            Name = name;
            X = x;
            Y = y;
            Toward = toward;
        }

        /// <summary>
        /// 唯一識別碼
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 方向
        /// </summary>
        public double Toward { get; set; }

        /// <summary>
        /// X 座標
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y 座標
        /// </summary>
        public int Y { get; set; }
    }

    /// <summary>
    /// 頂層 UI 控制事件集合
    /// </summary>
    public static class Events
    {
        /// <summary>
        /// Console 控制事件集合
        /// </summary>
        public static class ConsoleEvents
        {
            /// <summary>
            /// 文字已被加入
            /// </summary>
            public delegate void DelConsoleAdded(string msg);

            /// <summary>
            /// 顯示文字被清除
            /// </summary>
            public delegate void DelConsoleCleared();
        }

        /// <summary>
        /// GoalSetting 控制事件集合
        /// </summary>
        public static class GoalSettingEvents
        {
            /// <summary>
            /// 加入 Goal 點
            /// </summary>
            public delegate void DelAddNewGoal(Info goal);

            /// <summary>
            /// 刪除
            /// </summary>
            public delegate void DelDeleteGoals(IEnumerable<Info> goal);

            /// <summary>
            /// 尋找路徑
            /// </summary>
            public delegate void DelFindPath(Info goal);

            /// <summary>
            /// 載入地圖
            /// </summary>
            public delegate void DelLoadMap();

            /// <summary>
            /// 從 AGV 下載地圖
            /// </summary>
            public delegate void DelLoadMapFromAGV();

            /// <summary>
            /// 移動
            /// </summary>
            public delegate void DelRunGoal(Info goal);

            /// <summary>
            /// 按照順序移動全部
            /// </summary>
            public delegate void DelRunLoop(IEnumerable<Info> goal);

            /// <summary>
            /// 儲存
            /// </summary>
            public delegate void DelSaveGoal();

            /// <summary>
            /// 上傳地圖
            /// </summary>
            public delegate void DelSendMapToAGV();

            /// <summary>
            /// 更新 Goal 點
            /// </summary>
            public delegate void DelUpdateGoal(Info newGoal);
        }
    }
}

