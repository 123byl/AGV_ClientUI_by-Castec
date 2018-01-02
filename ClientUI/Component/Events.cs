using AGVDefine;
using MapProcessing;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// 當下車子的位置
        /// </summary>
        CartesianPos CurrentCar { get; set; }

        /// <summary>
        /// 目標點個數
        /// </summary>
        int GoalCount { get; }

        /// <summary>
        /// 加入 Goal 點
        /// </summary>
        void AddGoal(CartesianPosInfo goal);

        /// <summary>
        /// 加入充電站點
        /// </summary>
        /// <param name="power"></param>
        void AddPower(CartesianPosInfo power);

        /// <summary>
        /// 移除目前 Goal 點並加入新的 goal 點
        /// </summary>
        void ClearAndAddGoals(IEnumerable<CartesianPosInfo> goals);

        /// <summary>
        /// 移除所有 Goal 點
        /// </summary>
        void ClearGoal();
        
        /// <summary>
        /// 根據 ID 移除 Goal 點
        /// </summary>
        void DeleteGoal(uint ID);

        /// <summary>
        /// 根據 ID 移除 Goal 點
        /// </summary>
        void DeleteGoals(IEnumerable<uint> ID);

        /// <summary>
        /// 用 ID 尋找 Goal 點所在的引索位置
        /// </summary>
        int FindIndexByID(uint ID);

        /// <summary>
        /// 根據 ID 查詢 Goal 點
        /// </summary>
        CartesianPosInfo GetGoalByID(uint ID);

        /// <summary>
        /// 根據表單的列編號查詢 Goal
        /// </summary>
        CartesianPosInfo GetGoalByIndex(int row);

        /// <summary>
        /// 獲得所有 Goal 點資訊
        /// </summary>
        List<CartesianPosInfo> GetGoals();

        /// <summary>
        /// 獲得所有被選取的 Goal 點資訊
        /// </summary>
        List<CartesianPosInfo> GetSelectedGoals();

        /// <summary>
        /// 設定真實座標
        /// </summary>
        void SetCurrentRealPos(CartesianPos realPos);

        /// <summary>
        /// 設定表單選擇項目
        /// </summary>
        void SetSelectItem(uint id);

        /// <summary>
        /// 重新載入標示物
        /// </summary>
        void ReloadSingle();

        /// <summary>
        /// 解鎖路徑相關操作
        /// </summary>
        /// <param name="enb"></param>
        void EnableGo(bool enb = true);

        /// <summary>
        /// 更新標示物列表
        /// </summary>
        void RefreshSingle();

    }

    /// <summary>
    /// Test視窗公開操作方法
    /// </summary>
    public interface IITesting
    {

        event DelConnect Connect;
        
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

        event DelClearMap ClearMap;

        event DelSettingCarPos SettingCarPos;

        event DelCarPosConfirm CarPosConfirm;

        event DelStartScan StartScan;

        event DelShowMotionController ShowMotionController;

        void ChangedMotorStt(bool servoOn);

        void SetLaserStt(bool isGettingLaser);

        void SetServerStt(bool isConnect);

        void UnLockOriOperator(bool v);
        
        /// <summary>
        /// 設定目標agv IP
        /// </summary>
        /// <param name="host">設定IP</param>
        void SetHostIP(string host);

        /// <summary>
        /// 依照掃圖狀態變更UI介面
        /// </summary>
        /// <param name="isScanning"></param>
        void ChangedScanStt(bool isScanning);

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
            public delegate void DelAddNewGoal();

            /// <summary>
            /// 加入充電站
            /// </summary>
            /// <param name="power"></param>
            public delegate void DelAddNewPower();

            /// <summary>
            /// 清除所有目標點
            /// </summary>
            public delegate void DelClearGoals();

            /// <summary>
            /// 刪除
            /// </summary>
            public delegate void DelDeleteGoals(IEnumerable<CartesianPosInfo> goal);

            /// <summary>
            /// 尋找路徑
            /// </summary>
            public delegate void DelFindPath(CartesianPosInfo goal, int idxGoal);

            /// <summary>
            /// 充電
            /// </summary>
            /// <param name="goal"></param>
            /// <param name="idxGoal"></param>
            public delegate void DelCharging(CartesianPosInfo goal, int idxGoal);

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
            public delegate void DelRunGoal(CartesianPosInfo goal, int idxGoal);

            /// <summary>
            /// 按照順序移動全部
            /// </summary>
            public delegate void DelRunLoop(IEnumerable<CartesianPosInfo> goal);

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
            public delegate void DelUpdateGoal(CartesianPosInfo newGoal);

            /// <summary>
            /// 取得所有Goal點名稱
            /// </summary>
            public delegate void DelGetGoalNames();

            public delegate void DelSwitchCursor(CursorMode mode);
        }

        /// <summary>
        /// Testing控制事件集合
        /// </summary>
        public static class TestingEvents
        {
            public delegate void DelConnect(bool cnn,string hostIP = "");
           
            public delegate void DelGetCar();

            public delegate void DelGetLaser();

            public delegate void DelGetMap();

            public delegate void DelGetOri();

            public delegate void DelLoadOri();

            public delegate void DelMotion_Down(MotionDirection direction);

            public delegate void DelMotion_Up();

            public delegate void DelMotorServoOn(bool servoOn);

            public delegate void DelSendMap();

            public delegate void DelSetCarMode(EMode mode);

            public delegate void DelStartScan(bool scan);

            public delegate void DelSetVelocity(int velocity);

            public delegate void DelSimplifyOri();

            public delegate void DelClearMap();

            public delegate void DelSettingCarPos();

            public delegate void DelCarPosConfirm();

            public delegate void DelShowMotionController();

        }

        public static class FlowTemplate {

            public delegate bool DelSwitchFlag();

            public delegate bool DelIsAllow();

            public delegate void DelExecutingInfo();
        }
    }

}
