﻿using GLCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehiclePlanner.Module.Implement;
using VehiclePlanner.Partial.CtVehiclePlanner;

namespace VehiclePlanner.Partial.VehiclePlannerUI {

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
            public delegate void DelFindPath(uint goalID);

            /// <summary>
            /// 充電
            /// </summary>
            /// <param name="goal"></param>
            /// <param name="idxGoal"></param>
            public delegate void DelCharging(uint powerID);

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
            public delegate void DelRunGoal(uint idxGoal);

            /// <summary>
            /// 按照順序移動全部
            /// </summary>
            public delegate void DelRunLoop(IEnumerable<IGoal> goal);

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
            public delegate void DelUpdateGoal(IGoal newGoal);

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
            public delegate void DelConnect(bool cnn, string hostIP = "");

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
