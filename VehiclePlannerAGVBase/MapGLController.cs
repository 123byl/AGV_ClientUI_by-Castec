using Geometry;
using GLCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehiclePlannerAGVBase {
    /// <summary>
    /// 繪圖控制
    /// </summary>
    internal class MapGLController {

        #region Static

        /// <summary>
        /// 單例模式 - 物件實例
        /// </summary>
        internal static MapGLController mInstance = null;

        /// <summary>
        /// 單例模式 - 取得實例
        /// </summary>
        /// <returns></returns>
        internal static MapGLController GetInstance() {
            if (mInstance == null) mInstance = new MapGLController();
            return mInstance;
        }

        /// <summary>
        /// 由系統分配可用的ID
        /// </summary>
        /// <returns>可用的ID</returns>
        internal static uint GenerateID() {
            return Database.ID.GenerateID();
        }

        /// <summary>
        /// 建構Goal點
        /// </summary>
        /// <param name="currentPosition">Goal點所在座標</param>
        /// <returns>Goal點物件</returns>
        internal static IGoal NewGoal(ITowardPair currentPosition) {
            return FactoryMode.Factory.Goal(currentPosition, $"Goal{Database.GoalGM.Count}");
        }

        #endregion Static

        #region Declaration - Fields

        /// <summary>
        /// AGV ID
        /// </summary>
        protected uint mAGVID = 1;

        #endregion Declaration - FIelds

        #region Declaration - Properties

        /// <summary>
        /// iTS位置
        /// </summary>
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
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initial() {
            /*-- 載入AVG物件 --*/
            if (!Database.AGVGM.ContainsID(mAGVID)) {
                Database.AGVGM.Add(mAGVID, FactoryMode.Factory.AGV(0, 0, 0, "AGV"));
            }
        }

        /// <summary>
        /// 清除雷射標記
        /// </summary>
        public void ClearLaser() {
            Database.AGVGM[mAGVID].LaserAPoints.DataList.Clear();
        }

        /// <summary>
        /// 清除路徑標記
        /// </summary>
        public void ClearPath() {
            Database.AGVGM[mAGVID].Path.DataList.Clear();
        }

        /// <summary>
        /// 清除所有標記
        /// </summary>
        internal void ClearAll() {
            Database.ClearAllButAGV();
            ClearLaser();
            ClearPath();
        }

        /// <summary>
        /// 取得Goal點索引值
        /// </summary>
        /// <param name="goalID">Goal點ID</param>
        /// <returns></returns>
        internal int IndexOfGoal(uint goalID) {
            return Database.GoalGM.IndexOf(goalID);
        }

        /// <summary>
        /// 取得Goal點物件
        /// </summary>
        /// <param name="goalID">Goal點ID</param>
        /// <returns></returns>
        internal IGoal GetGoal(uint goalID) {
            return Database.GoalGM[goalID];
        }

        /// <summary>
        /// 取得充電站索引
        /// </summary>
        /// <param name="powerID">充電站ID</param>
        /// <returns></returns>
        internal int IndexOfPower(uint powerID) {
            return Database.PowerGM.IndexOf(powerID);
        }

        /// <summary>
        /// 取得充電站物件
        /// </summary>
        /// <param name="powerID">充電站物件</param>
        /// <returns></returns>
        internal IPower GetPower(uint powerID) {
            return Database.PowerGM[powerID];
        }

        /// <summary>
        /// 儲存Map檔
        /// </summary>
        /// <param name="curMapPath"></param>
        internal void Save(string curMapPath) {
            Database.Save(curMapPath);
        }

        /// <summary>
        /// 是否包含該ID之Goal點
        /// </summary>
        /// <param name="id">目標Goal點ID</param>
        /// <returns></returns>
        internal bool ContainGoal(uint id) {
            return Database.GoalGM.ContainsID(id);
        }

        /// <summary>
        /// 移除目標ID之Goal點
        /// </summary>
        /// <param name="id">目標Goal點ID</param>
        internal void RemoveGoal(uint id) {
            Database.GoalGM.Remove(id);
        }

        /// <summary>
        /// 是否包含該充電站之ID
        /// </summary>
        /// <param name="id">目標充電站ID</param>
        /// <returns></returns>
        internal bool ContainPower(uint id) {
            return Database.PowerGM.ContainsID(id);
        }

        /// <summary>
        /// 移除目標ID之充電站
        /// </summary>
        /// <param name="id">目標充電站ID</param>
        internal void RemovePower(uint id) {
            Database.PowerGM.Remove(id);
        }

        /// <summary>
        /// 清除所有Goal點
        /// </summary>
        internal void ClearGoal() {
            Database.GoalGM.Clear();
        }

        /// <summary>
        /// 清除所有充電站
        /// </summary>
        internal void ClearPower() {
            Database.PowerGM.Clear();
        }

        /// <summary>
        /// 新增Goal點
        /// </summary>
        /// <param name="goalID">Goal點ID</param>
        /// <param name="goal">Goal點物件</param>
        internal void AddGoal(uint goalID, IGoal goal) {
            Database.GoalGM.Add(goalID, goal);
        }

        /// <summary>
        /// 設定ITS當前位置
        /// </summary>
        /// <param name="position">設定位置</param>
        internal void SetLocation(ITowardPair position) {
            Database.AGVGM[mAGVID].SetLocation(position);
        }

        /// <summary>
        /// 載入ori檔
        /// </summary>
        /// <param name="curOriPath">ori檔路徑</param>
        /// <returns>ori範圍</returns>
        internal IArea LoadOri(string curOriPath) {
            return Database.LoadOriToDatabase(curOriPath, mAGVID);
        }

        /// <summary>
        /// 載入map檔
        /// </summary>
        /// <param name="curMapPath">map檔路徑</param>
        /// <returns>map範圍</returns>
        internal IArea LoadMap(string curMapPath) {
            return Database.LoadMapToDatabase(curMapPath);
        }

        /// <summary>
        /// 繪製雷射
        /// </summary>
        /// <param name="laser">雷射點集合</param>
        internal void DrawLaser(IEnumerable<IPair> laser) {
            Database.AGVGM[mAGVID]?.LaserAPoints.DataList.Replace(laser);
        }

        /// <summary>
        /// 繪製路徑
        /// </summary>
        /// <param name="path">路徑點集合</param>
        internal void DrawPath(IEnumerable<IPair> path) {
            Database.AGVGM[mAGVID].Path.DataList.Replace(path);
        }

        #endregion Funciton - Public Methods

    }


}
