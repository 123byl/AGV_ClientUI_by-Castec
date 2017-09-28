﻿using AGVMap;
using System.Collections.Generic;
using static ClientUI.Events.ConsoleEvents;
using static ClientUI.Events.GoalSettingEvents;
using static ClientUI.Events.TestingEvents;

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
    public interface IIGoalSetting
    {
        /// <summary>
        /// 加入 Goal 點
        /// </summary>
        event DelAddNewGoal AddNewGoalEvent;

        /// <summary>
        /// 清除所有目標點
        /// </summary>
        event DelClearGoals ClearGoalsEvent;

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
        event Events.GoalSettingEvents.DelLoadMap LoadMapEvent;

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
        /// 當下車子的位置
        /// </summary>
        TowardPos CurrentCar { get; set; }

        /// <summary>
        /// 目標點個數
        /// </summary>
        int GoalCount { get; }

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
        /// 用 ID 尋找 Goal 點所在的引索位置
        /// </summary>
        int FindIndexByID(int ID);

        /// <summary>
        /// 根據 ID 查詢 Goal 點
        /// </summary>
        Info GetGoalByID(int ID);

        /// <summary>
        /// 根據表單的列編號查詢 Goal
        /// </summary>
        Info GetGoalByIndex(int row);

        /// <summary>
        /// 獲得所有 Goal 點資訊
        /// </summary>
        List<Info> GetGoals();

        /// <summary>
        /// 獲得所有被選取的 Goal 點資訊
        /// </summary>
        List<Info> GetSelectedGoals();

        /// <summary>
        /// 設定真實座標
        /// </summary>
        void SetCurrentRealPos(IPair realPos);

        /// <summary>
        /// 設定表單選擇項目
        /// </summary>
        void SetSelectItem(int id);
    }

    /// <summary>
    /// MapGL視窗公開操作方法
    /// </summary>
    public interface IIMapGL
    {
    }

    public interface IITesting
    {
        event DelConnect Connect;

        event DelCorrectOri CorrectOri;

        event DelGetCar GetCar;

        event DelGetLaser GetLaser;

        event DelGetMap GetMap;

        event DelGetOri GetOri;

        event DelLoadMap LoadMap;

        event DelLoadOri LoadOri;

        event DelMotion_Down Motion_Down;

        event DelMotion_Up Motion_Up;

        event DelMotorServoOn MotorServoOn;

        event DelSendMap SendMap;

        event DelSetCarMode SetCarMode;

        event DelSetVelocity SetVelocity;

        event DelSimplifyOri SimplifyOri;

        void ChangedMotorStt(bool servoOn);

        void SetLaserStt(bool isGettingLaser);

        void SetServerStt(bool isConnect);

        void UnLockOriOperator(bool v);
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
            /// 清除所有目標點
            /// </summary>
            public delegate void DelClearGoals();

            /// <summary>
            /// 刪除
            /// </summary>
            public delegate void DelDeleteGoals(IEnumerable<Info> goal);

            /// <summary>
            /// 尋找路徑
            /// </summary>
            public delegate void DelFindPath(Info goal, int idxGoal);

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
            public delegate void DelRunGoal(Info goal, int idxGoal);

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

        /// <summary>
        /// Testing控制事件集合
        /// </summary>
        public static class TestingEvents
        {
            public delegate void DelConnect(bool cnn);

            public delegate void DelCorrectOri();

            public delegate void DelGetCar();

            public delegate void DelGetLaser();

            public delegate void DelGetMap();

            public delegate void DelGetOri();

            public delegate void DelLoadOri();

            public delegate void DelMotion_Down(MotionDirection direction, int velocity = 0);

            public delegate void DelMotion_Up();

            public delegate void DelMotorServoOn(bool servoOn);

            public delegate void DelSendMap();

            public delegate void DelSetCarMode(CarMode mode);

            public delegate void DelSetVelocity(int velocity);

            public delegate void DelSimplifyOri();
        }
    }
}
