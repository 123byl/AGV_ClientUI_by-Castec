using AGVDefine;
using BroadCast;
using CtLib.Library;
using Geometry;
using GLCore;
using SerialCommunication;
using SerialCommunicationData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VehiclePlanner.Core;
using static FactoryMode;

namespace VehiclePlannerAGVBase {

    /// <summary>
    /// 以AGVBase實作之底層
    /// </summary>
    public class CtVehiclePlanner :BaseVehiclePlanner,IVehiclePlanner {

        #region Declaration - Fields

        /// <summary>
        /// MapGL相關操作
        /// </summary>
        private MapGLController mMapGL = MapGLController.GetInstance();
        
        /// <summary>
        /// 地圖中心
        /// </summary>
        private IPair mMapCenter = null;

        #endregion Declaration - Fields

        #region Declaration - Properties
        /// <summary>
        /// 以AGVBase實作之ITS控制器
        /// </summary>
        public new IITSController Controller { get { return base.Controller as IITSController; } }

        /// <summary>
        /// 地圖中心點
        /// </summary>
        public IPair MapCenter {
            get {
                return mMapCenter;
            }
            set {
                if (mMapCenter != value && value != null) {
                    mMapCenter = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Declaration - Properties

        #region Function - Constructors

        public CtVehiclePlanner():base() {
            base.Controller = new ITSControllerSerial();
        }

        #endregion Function - Constructors

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
                    if (Controller.IsConnected && type == FileType.Map) {
                        Controller.SendAndSetMap(fileName);
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
        public override void DeleteMarker(IEnumerable<uint> markers) {
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

		public override void UpdateValue(uint ID, string colName, object value)
		{
			IGoal Goal = mMapGL.GetGoal(ID);
			switch (colName)
			{
				case "Name":
					string Name = (string)value;
					Goal.Name = Name;
					break;
				case "X":
					int X;
					Goal.Data.Position.X = int.TryParse((string)value, out X) ? X : Goal.Data.Position.X;
					break;
				case "Y":
					int Y;
					Goal.Data.Position.Y = int.TryParse((string)value, out Y)? Y : Goal.Data.Position.Y;
					break;
				case "Toward":
					double Theta;
					Goal.Data.Toward.Theta = double.TryParse((string)value, out Theta) ? Theta : Goal.Data.Toward.Theta;
					break;
			}	
		}
		#endregion Funciotn - Public Methods

		#region Function - Protected Methods

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

		

        #endregion Funciotn - Protected Methods

    }

}
