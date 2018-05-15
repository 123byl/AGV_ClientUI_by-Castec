using Geometry;
using GLCore;
using GLUI;
using SerialCommunicationData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehiclePlanner.Core;
using VehiclePlanner.Module.Interface;

namespace VehiclePlannerAGVBase {

    /// <summary>
    /// 以AGVBase實作之底層介面
    /// </summary>
    public interface IVehiclePlanner : IBaseVehiclePlanner {
        /// <summary>
        /// iTS控制器
        /// </summary>
        new IITSController Controller { get; }
        /// <summary>
        /// 地圖中心點
        /// </summary>
        IPair MapCenter { get; set; }
        /// <summary>
        /// 清除標記物
        /// </summary>
        void ClearMarker();
    }

    /// <summary>
    /// 以AGVBase實作之ITS控制器介面
    /// </summary>
    public interface IITSController : IBaseITSController {
        /// <summary>
        /// iTS狀態
        /// </summary>
        IStatus Status { get; }
        /// <summary>
        /// 要求Map檔
        /// </summary>
        /// <param name="mapName">要求的Map檔名</param>
        /// <returns>Map檔</returns>
        IDocument RequestMapFile(string mapName);
        /// <summary>
        /// 傳送並要求載入Map
        /// </summary>
        /// <param name="mapPath">目標Map檔路徑</param>
        void SetPosition(IPair oldPosition, IPair newPosition);
    }

    /// <summary>
    /// 以AGVBase實作之MapGL介面
    /// </summary>
    public interface IMapGL : IBaseMapGL {
        /// <summary>
        /// 地圖中心點
        /// </summary>
        IPair MapCenter { get; set; }

        IScene Ctrl { get; }
    }

    /// <summary>
    /// 以AGVBase實作之GoalSetting介面
    /// </summary>
    public interface IGoalSetting : IBaseGoalSetting {
        /// <summary>
        /// 按照順序移動全部
        /// </summary>
        event DelRunLoop RunLoopEvent;

        /// <summary>
        /// 設定真實座標
        /// </summary>
        void UpdateNowPosition(IPair realPos);

        /// <summary>
        /// 重新載入標示物
        /// </summary>
        void ReloadSingle();

    }
    
}
