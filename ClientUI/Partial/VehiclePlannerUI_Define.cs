
using System.Collections.Generic;
using VehiclePlanner.Core;

namespace VehiclePlanner.Partial.VehiclePlannerUI {

    #region Declaration - Enum

    /// <summary>
    /// 鼠標模式
    /// </summary>
    public enum CursorMode {

        /// <summary>
        /// 選擇模式
        /// </summary>
        Select,

        /// <summary>
        /// 新增Goal點模式
        /// </summary>
        Goal,

        /// <summary>
        /// 新增充電站模式
        /// </summary>
        Power,

        /// <summary>
        /// 拖曳模式
        /// </summary>
        Drag,

        /// <summary>
        /// 畫筆模式
        /// </summary>
        Pen,

        /// <summary>
        /// 橡皮擦模式
        /// </summary>
        Eraser,

        /// <summary>
        /// 地圖插入模式
        /// </summary>
        Insert,

        /// <summary>
        /// 禁止區模式
        /// </summary>
        ForbiddenArea
    }

    #endregion Declaration - Enum

    /// <summary>
    /// 頂層 UI 控制事件集合
    /// </summary>
    public static class Events {

        /// <summary>
        /// Console 控制事件集合
        /// </summary>
        public static class ConsoleEvents {

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
        public static class GoalSettingEvents {

            /// <summary>
            /// 將目前位置設為Goal點
            /// </summary>
            public delegate void DelAddCurrentGoal();

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
            /// 刪除標示物
            /// </summary>
            public delegate void DelDeleteSingle(IEnumerable<uint> goal);

            /// <summary>
            /// 尋找路徑
            /// </summary>
            public delegate void DelFindPath(string goalName);

            /// <summary>
            /// 充電
            /// </summary>
            /// <param name="powerName">充電站名稱</param>
            public delegate void DelCharging(string powerNmae);

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
            public delegate void DelRunGoal(string goalName);



            /// <summary>
            /// 儲存
            /// </summary>
            public delegate void DelSaveGoal();

            /// <summary>
            /// 上傳地圖
            /// </summary>
            public delegate void DelSendMapToAGV();

            /// <summary>
            /// 取得所有Goal點名稱
            /// </summary>
            public delegate void DelGetGoalNames();

            public delegate void DelSwitchCursor(CursorMode mode);
        }

        /// <summary>
        /// Testing控制事件集合
        /// </summary>
        public static class TestingEvents {

            public delegate void DelConnect(bool cnn);

            public delegate void DelGetCar();

            public delegate void DelGetLaser();

            public delegate void DelGetMap();

            public delegate void DelGetOri();

            public delegate void DelLoadOri();

            public delegate void DelMotion_Down(MotionDirection direction);

            public delegate void DelMotion_Up();

            public delegate void DelMotorServoOn(bool servoOn);

            public delegate void DelSendMap();

            public delegate void DelStartScan(bool scan);

            public delegate void DelSetVelocity(int velocity);

            public delegate void DelSimplifyOri();

            public delegate void DelClearMap();

            public delegate void DelSettingCarPos();

            public delegate void DelCarPosConfirm();

            public delegate void DelShowMotionController();

            public delegate void DelFind();
        }

        public static class FlowTemplate {

            public delegate bool DelSwitchFlag();

            public delegate bool DelIsAllow();

            public delegate void DelExecutingInfo();
        }
    }
}