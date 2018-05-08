using CtLib.Library;
using Geometry;
using GLCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VehiclePlanner.Core;

namespace VehiclePlannerAGVBase {
    public class VehiclePlanner :BaseVehiclePlanner,IVehiclePlanner {

        #region Declaration - Fields

        /// <summary>
        /// MapGL相關操作
        /// </summary>
        private MapGLController mMapGL = MapGLController.GetInstance();

        #endregion Declaration - Fields

        #region Funciton - Public Methods

        /// <summary>
        /// 系統初始化
        /// </summary>
        public override void Initial() {
            mMapGL.Initial();
            base.Initial();
        }

        /// <summary>
        /// 清除地圖
        /// </summary>
        public override void ClearMap() {
            try {
                mMapGL.ClearAll();
                OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
            } catch (Exception ex) {
                OnConsoleMessage(ex.Message);
            }
        }

        /// <summary>
        /// 載入檔案
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        public override void LoadFile(FileType type, string fileName) {
            try {
                bool isLoaded = false;
                switch (type) {
                    case FileType.Ori:
                        isLoaded = LoadOri(fileName);
                        if (isLoaded) {
                            mMapGL.ClearLaser();
                        }
                        break;
                    case FileType.Map:
                        isLoaded = LoadMap(fileName);
                        break;
                    default:
                        throw new ArgumentException($"無法載入未定義的檔案類型{type}");
                }
                if (isLoaded) {
                    SetBalloonTip($"Load { type}", $"\'{fileName}\' is loaded");
                    if (mITS.IsConnected && type == FileType.Map) {
                        mITS.SendAndSetMap(fileName);
                    }
                } else {
                    OnErrorMessage("File data is wrong, can not read");
                }
            } catch (Exception ex) {
                OnErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// 刪除指定標記物
        /// </summary>
        /// <param name="markers"></param>
        public void DeleteMarker(IEnumerable<uint> markers) {
            foreach (var id in markers) {
                if (mMapGL.ContainGoal(id)) {
                    mMapGL.RemoveGoal(id);
                } else if (mMapGL.ContainPower(id)) {
                    mMapGL.RemovePower(id);
                }
            }
            OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
        }

        /// <summary>
        /// 清除標記物
        /// </summary>
        public void ClearMarker() {
            OnConsoleMessage("[Clear Goal]");
            mMapGL.ClearGoal();
            OnConsoleMessage("[Clear Power]");
            mMapGL.ClearPower();
            OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
        }

        /// <summary>
        /// 新增當前位置為Goal點
        /// </summary>
        public override void AddCurrentAsGoal() {
            /*-- 取得當前位置 --*/
            var currentPosition = mMapGL.Location;
            /*-- 建構Goal點 --*/
            var goal = MapGLController.NewGoal(currentPosition);
            /*-- 分配ID --*/
            var goalID = MapGLController.GenerateID();
            /*-- 將Goal點資料加入Goal點管理集合 --*/
            mMapGL.AddGoal(goalID, goal);
            /*-- 重新載入Goal點資訊 --*/
            OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
        }

        #endregion Funciotn - Public Methods

        /// <summary>
        /// 載入Map檔
        /// </summary>
        /// <param name="mapPath">Map檔路徑</param>
        protected override bool LoadMap(string mapPath) {
            bool isLoaded = true;
            mCurMapPath = mapPath;
            string path = CtFile.GetFileName(mapPath);
            if (mBypassLoadFile) {
                /*-- 空跑1秒模擬載入Map檔 --*/
                SpinWait.SpinUntil(() => false, 1000);
            } else {
                //#region - Retrive information from .map file -
                /*-- 地圖清空 --*/
                OnVehiclePlanner(VehiclePlannerEvents.NewMap);
                /*-- 載入Map並取得Map中心點 --*/
                var center = mMapGL.LoadMap(mCurMapPath)?.Center();
                if (center != null) {
                    MapCenter = center;
                    OnVehiclePlanner(VehiclePlannerEvents.MarkerChanged);
                } else {
                    isLoaded = false;
                }
            }
            return isLoaded;
        }

        /// <summary>
        /// 載入Ori檔
        /// </summary>
        /// <param name="oriPath"></param>
        /// <returns></returns>
        protected override bool LoadOri(string oriPath) {
            bool isLoaded = true;
            CurOriPath = oriPath;
            OnVehiclePlanner(VehiclePlannerEvents.NewMap);
            if (!mBypassLoadFile) {//無BypassLoadFile
                /*-- 載入Map並取得Map中心點 --*/
                IPair center = mMapGL.LoadOri(CurOriPath)?.Center();
                if (center != null) {
                    MapCenter = center;
                } else {
                    isLoaded = false;
                }
            } else {//Bypass LoadFile功能
                    /*-- 空跑一秒，模擬檔案載入 --*/
                SpinWait.SpinUntil(() => false, 1000);
            }
            return isLoaded;
        }

        /// <summary>
        /// 儲存Map檔
        /// </summary>
        /// <param name="path"></param>
        protected override void SaveMap(string path) {
            mMapGL.Save(path);
        }

    }

    /// <summary>
    /// 繪圖控制
    /// </summary>
    internal class MapGLController {

        #region Static

        internal static MapGLController mInstance = null;

        internal static uint GenerateID() {
            return Database.ID.GenerateID();
        }

        internal static IGoal NewGoal(ITowardPair currentPosition) {
            return FactoryMode.Factory.Goal(currentPosition, $"Goal{Database.GoalGM.Count}");
        }

        internal static MapGLController GetInstance() {
            if (mInstance == null) mInstance = new MapGLController();
            return mInstance;
        }

        #endregion Static

        #region Declaration - Fields

        /// <summary>
        /// AGV ID
        /// </summary>
        protected uint mAGVID = 1;

        #endregion Declaration - FIelds

        #region Declaration - Properties

        public ITowardPair Location {
            get {
                return Database.AGVGM[mAGVID].Data;
            }
        }

        #endregion Declaration - Properties

        #region Funciton - Constructors

        private MapGLController() {

        }

        #endregion Funciton - Constructors

        #region Funciton - Public Methods

        public void Initial() {
            /*-- 載入AVG物件 --*/
            if (!Database.AGVGM.ContainsID(mAGVID)) {
                Database.AGVGM.Add(mAGVID, FactoryMode.Factory.AGV(0, 0, 0, "AGV"));
            }
        }

        public void ClearLaser() {
            Database.AGVGM[mAGVID].LaserAPoints.DataList.Clear();
        }

        public void ClearPath() {
            Database.AGVGM[mAGVID].Path.DataList.Clear();
        }

        internal void ClearAll() {
            Database.ClearAllButAGV();
            ClearLaser();
            ClearPath();
        }

        internal int IndexOfGoal(uint goalID) {
            return Database.GoalGM.IndexOf(goalID);
        }

        internal IGoal GetGoal(uint goalID) {
            return Database.GoalGM[goalID];
        }

        internal int IndexOfPower(uint powerID) {
            return Database.PowerGM.IndexOf(powerID);
        }

        internal IPower GetPower(uint powerID) {
            return Database.PowerGM[powerID];
        }

        internal void Save(string curMapPath) {
            Database.Save(curMapPath);
        }

        internal bool ContainGoal(uint id) {
            return Database.GoalGM.ContainsID(id);
        }

        internal void RemoveGoal(uint id) {
            Database.GoalGM.Remove(id);
        }

        internal bool ContainPower(uint id) {
            return Database.PowerGM.ContainsID(id);
        }

        internal void RemovePower(uint id) {
            Database.PowerGM.Remove(id);
        }

        internal void ClearGoal() {
            Database.GoalGM.Clear();
        }

        internal void ClearPower() {
            Database.PowerGM.Clear();
        }

        internal void AddGoal(uint goalID, IGoal goal) {
            Database.GoalGM.Add(goalID, goal);
        }

        internal void SetLocation(ITowardPair position) {
            Database.AGVGM[mAGVID].SetLocation(position);
        }

        internal IArea LoadOri(string curOriPath) {
            return Database.LoadOriToDatabase(curOriPath, mAGVID);
        }

        internal IArea LoadMap(string curMapPath) {
            return Database.LoadMapToDatabase(curMapPath);
        }

        internal void DrawLaser(IEnumerable<IPair> laser) {
            Database.AGVGM[mAGVID]?.LaserAPoints.DataList.Replace(laser);
        }

        internal void DrawPath(IEnumerable<IPair> path) {
            Database.AGVGM[mAGVID].Path.DataList.Replace(path);
        }

        #endregion Funciton - Public Methods

    }

    public interface IVehiclePlanner : IBaseVehiclePlanner {
        /// <summary>
        /// 清除標記物
        /// </summary>
        void ClearMarker();
        /// <summary>
        /// 刪除指定標記物
        /// </summary>
        /// <param name="markers"></param>
        void DeleteMarker(IEnumerable<uint> markers);
    }

}
