using System.Collections.Generic;
using System.Linq;

using CtLib.Module.Utility;

namespace CtLib.Module.Adept {
    /// <summary>Mobile robot 各狀態與資訊</summary>
    public class CtMobileStatus : ICtVersion  {

        #region Version

        /// <summary>CtMobileStatus 版本訊息</summary>
        /// <remarks><code language="C#">
        /// 0.0.0  William [2012/09/07]
        ///     + CtMobileStatus.vb
        ///      
        /// 1.0.0  Ahern [2015/03/09]
        ///     \ 從 VB.Net 轉移至 C#
        /// 
        /// </code></remarks>
        public CtVersion Version { get { return new CtVersion(1, 0, 0, "2015/03/09", "William Huang"); } }

        #endregion

        #region Declaration - Definitions
        /// <summary>No Target</summary>
        public static readonly string NO_TARGET = "NOT";
        /// <summary>No Arrived</summary>
        public static readonly string NO_ARRIVED = "NOA";
        #endregion

        #region Declaration - Enumerations
        /// <summary>Mobile robot 動作狀態</summary>
        public enum MRStat : byte {
            /// <summary>到達位置</summary>
            ARRIVED,
            /// <summary>錯誤</summary>
            ERROR,
            /// <summary>緊急停止</summary>
            ESTOP,
            /// <summary>前往中</summary>
            GOING,
            /// <summary>馬達停止</summary>
            MOTOR_DISABLE,
            /// <summary>停止中</summary>
            STOPPING,
            /// <summary>已停止</summary>
            STOPPED,
            /// <summary>Following</summary>
            FOLLOWING,
            /// <summary>Teleop driving</summary>
            TELEOP_DRIVING,
            /// <summary>Complete patrol</summary>
            COMPLETED_PATROL,
            /// <summary>Finished patrolling</summary>
            FINISHED_PATROLLING,
            /// <summary>Completed Macro</summary>
            COMPLETED_MACRO,
            /// <summary>Successfully finished task list</summary>
            SUCCESSFULLY_FINISHED_TASK_LIST,
            /// <summary>Dock</summary>
            DOCK
        }

        /// <summary>Dock 狀態</summary>
        public enum DockingState : byte {
            /// <summary>Undocking</summary>
            UNDOCKING,
            /// <summary>Undocked</summary>
            UNDOCKED,
            /// <summary>Docking</summary>
            DOCKING,
            /// <summary>Docked</summary>
            DOCKED
        }

        /// <summary>強制充電狀態</summary>
        public enum ForcedState : byte {
            /// <summary>不強制充電</summary>
            UNFORCED,
            /// <summary>強制充電</summary>
            FORCED
        }

        /// <summary>充電狀態</summary>
        public enum ChargeState : byte {
            /// <summary>非充電狀態</summary>
            NOT,
            /// <summary>過度充電</summary>
            OVERCHARGE,
            /// <summary>Bulk</summary>
            BULK,
            /// <summary>Balance</summary>
            BALANCE,
            /// <summary>Float</summary>
            FLOAT
        }
        #endregion

        #region Declaration - Support Class
        /// <summary>充電相關狀態</summary>
        public class DockStatus {
            /// <summary>Dock 狀態</summary>
            public DockingState DockStt { get; set; }
            /// <summary>強制模式</summary>
            public ForcedState ForceStt { get; set; }
            /// <summary>充電狀態</summary>
            public ChargeState ChargeStt { get; set; }
            /// <summary>建立充電狀態之物件</summary>
            public DockStatus() {
                DockStt = DockingState.UNDOCKED;
                ForceStt = ForcedState.UNFORCED;
                ChargeStt = ChargeState.NOT;
            }
            /// <summary>建立充電狀態之物件</summary>
            /// <param name="dockStt">Dock 狀態</param>
            /// <param name="forceStt">強制模式</param>
            /// <param name="chargeStt">充電狀態</param>
            public DockStatus(DockingState dockStt, ForcedState forceStt, ChargeState chargeStt) {
                DockStt = dockStt;
                ForceStt = forceStt;
                ChargeStt = chargeStt;
            }
        }

        /// <summary>Mobile robot 於 map 上的座標及角度</summary>
        public class LocationXYT {
            /// <summary>X 座標</summary>
            public int X { get; set; }
            /// <summary>Y 座標</summary>
            public int Y { get; set; }
            /// <summary>旋轉角</summary>
            public int Head { get; set; }
            /// <summary>建立 MobileRobot 位置資訊</summary>
            public LocationXYT() {
                X = 0;
                Y = 0;
                Head = 0;
            }
            /// <summary>建立 MobileRobot 位置資訊</summary>
            /// <param name="x">X 座標</param>
            /// <param name="y">Y 座標</param>
            /// <param name="head">旋轉角</param>
            public LocationXYT(int x, int y, int head) {
                X = x;
                Y = y;
                Head = head;
            }
        }
        /// <summary>站點資訊</summary>
        public class StationSet {
            /// <summary>目標站點</summary>
            public string GoalName { get; set; }
            /// <summary>對應到 GoalName 的站點代號</summary>
            public List<string> StationCode { get; set; }
            /// <summary>建立站點資訊</summary>
            public StationSet() {
                GoalName = "";
                StationCode = new List<string>();
            }
            /// <summary>建立站點資訊</summary>
            /// <param name="goalName">目標站點</param>
            /// <param name="stationCode">對應到 GoalName 的站點代號</param>
            public StationSet(string goalName, List<string> stationCode) {
                GoalName = goalName;
                StationCode = stationCode;
            }
            /// <summary>建立站點資訊</summary>
            /// <param name="goalName">目標站點</param>
            /// <param name="stationCode">對應到 GoalName 的站點代號</param>
            public StationSet(string goalName, params string[] stationCode) {
                GoalName = goalName;
                StationCode = stationCode.ToList();
            }
        }
        #endregion

        #region Declaration - Properties
        /// <summary>儲存目前 Map 中所存在的站點</summary>
        public List<string> GoalList { get; private set; }
        /// <summary>儲存目前 Map 中所有 Route name</summary>
        public List<string> RouteList { get; private set; }
        /// <summary>儲存目前 Map 中的 Macro name</summary>
        public List<string> MacroList { get; private set; }
        /// <summary>儲存 Mobile goal 及對應的客戶站點代號</summary>
        public List<StationSet> StationSets { get; private set; }
        /// <summary>Mobile robot 動作狀態</summary>
        public MRStat MRStt { get; set; }
        /// <summary>到達位置</summary>
        public string ArrivedGoal { get; set; }
        /// <summary>前往目標</summary>
        public string TargetGoal { get; set; }
        /// <summary>目前電量狀態</summary>
        public int ChargeStt { get; set; }
        /// <summary>目前偵測到環境與 map 相似度</summary>
        public double LocalizationScore { get; set; }
        /// <summary>溫度</summary>
        public int Temperature { get; set; }
        /// <summary>MR 擴充狀態</summary>
        public string ExtendStt { get; set; }
        /// <summary>座標（X, Y, 角度）</summary>
        public LocationXYT Locations { get; set; }
        /// <summary>Dock 相關狀態</summary>
        public DockStatus DockStts { get; set; }
        /// <summary>行走總距離</summary>
        public int Odometer { get; set; }
        /// <summary>二點間行走路徑距離</summary>
        public int BTdistance { get; set; }
        /// <summary>最後一筆完成的Route Name</summary>
        public string FinishRouteName { get; set; }
        /// <summary>最後一筆完成的Macro Name</summary>
        public string FinishMacroName { get; set; }
        /// <summary>前一位置</summary>
        public string PreArrivedGoal { get; set; }
        #endregion

        #region Function - Constructors
        /// <summary>建立 MobileRobot 相關狀態資訊</summary>
        public CtMobileStatus() {
            ArrivedGoal = NO_ARRIVED;
            TargetGoal = NO_TARGET;

            GoalList = new List<string>();
            RouteList = new List<string>();
            StationSets = new List<StationSet>();
            Locations = new LocationXYT();
            DockStts = new DockStatus();
        }
        #endregion

        #region Function - Methods

        #endregion

        #region Function - Core
        /// <summary>加入新的一組 StationSet</summary>
        /// <param name="goalName">目標站點名稱</param>
        /// <param name="stationCode">Mobile goal 及對應的客戶站點代號</param>
        public void AddStationSet(string goalName, params string[] stationCode) {
            if (stationCode == null || stationCode.Length < 1) return;
            if (StationSets == null) StationSets = new List<StationSet>();

            StationSet stationSet = StationSets.Find(data => data.GoalName == goalName);
            if (stationSet != null) {
                foreach (string item in stationCode) {
                    if (!stationSet.StationCode.Contains(item)) stationSet.StationCode.Add(item.ToLower());
                }
            } else
                StationSets.Add(new StationSet(goalName, stationCode));
        }

        /// <summary>加入 Map 中所存在的站點</summary>
        /// <param name="goalStation">欲加入的的站點</param>
        public void AddGoals(params string[] goalStation) {
            GoalList.AddRange(goalStation);
        }

        /// <summary>加入 Map 中的 Route name</summary>
        /// <param name="route">欲加入的 Route name</param>
        public void AddRoutes(params string[] route) {
            RouteList.AddRange(route);
        }

        /// <summary>加入 Map 中的 Macro name</summary>
        /// <param name="macro">欲加入的 Macro name</param>
        public void AddMacros(params string[] macro) {
            MacroList.AddRange(macro);
        }

        /// <summary>將當前到達點位儲存到前一位置，並設定目前到點位置</summary>
        /// <param name="goalName">目前到達位置</param>
        public void ArrivedGoalRecord(string goalName) {
            PreArrivedGoal = ArrivedGoal;
            ArrivedGoal = goalName;
        }

        /// <summary>清除目前所有設定的 StationSet</summary>
        public void CleanStationSets() {
            StationSets.Clear();
        }

        /// <summary>清除目前所有設定的 GoalList</summary>
        public void CleanGoals() {
            GoalList.Clear();
        }

        /// <summary>清除目前所有設定的 RouteList</summary>
        public void CleanRoutes() {
            RouteList.Clear();
        }

        /// <summary>清除目前所有設定的 MacroList</summary>
        public void CleanMacros() {
            MacroList.Clear();
        }
        #endregion
    }
}
