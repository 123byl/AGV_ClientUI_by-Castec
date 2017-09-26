using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using WeifenLuo.WinFormsUI.Docking;

using static MapGL.CastecMapUI;
using AGVMathOperation;
using MapProcessing;
using MapGL;

using CtLib.Library;
using CtLib.Forms;
using CtLib.Module.Ultity;
namespace ClientUI {

    /// <summary>
    /// 地圖顯示介面
    /// </summary>
    public partial class CtMapGL : CtDockContent {
        
        #region Function - Constructors


        /// <summary>
        /// 共用建構方法
        /// </summary>
        /// <param name="mapGL"></param>
        /// <param name="main"></param>
        /// <param name="defState"></param>
        public CtMapGL(DockState defState = DockState.Float)
            : base(defState) {
            InitializeComponent();
        }

        #endregion Function - Constructors
        
    }

    /// <summary>
    /// 地圖相關事件參數
    /// </summary>
    public class MapEventArgs : EventArgs {

        #region Declaration - Properties

        /// <summary>
        /// 事件類型
        /// </summary>
        public MapEventType Type { get; }
        /// <summary>
        /// 傳遞參數
        /// </summary>
        public object Value { get; }

        #endregion Declaration Properties

        #region Function - Constructors

        /// <summary>
        /// 一般建構方法
        /// </summary>
        /// <param name="type">事件類型</param>
        /// <param name="value">傳遞參數</param>
        public MapEventArgs(MapEventType type, object value = null) {
            this.Type = type;
            this.Value = value;
        }

        /// <summary>
        /// 車子資訊更新事件參數建構
        /// </summary>
        /// <param name="info"></param>
        public MapEventArgs(CarInfo info) : this(MapEventType.CarInfoRefresh, info) {
        }

        #endregion Function - Constructors
    }

    /// <summary>
    /// 地圖相關事件類型
    /// </summary>
    public enum MapEventType {
        /// <summary>
        /// 車子資訊更新
        /// </summary>
        CarInfoRefresh,
        /// <summary>
        /// 地圖清除
        /// </summary>
        ClearMapGL,
        /// <summary>
        /// 雷射資料取得
        /// </summary>
        GetLaser,
        /// <summary>
        /// 車子位置確認
        /// </summary>
        CarPosConfirm,
        /// <summary>
        /// 切換GL模式
        /// </summary>
        GLMode,
        /// <summary>
        /// 新增Goal點
        /// </summary>
        AddGoalPos,
        /// <summary>
        /// 原始地圖簡化
        /// </summary>
        SimplifyOri,
        /// <summary>
        /// 載入地圖
        /// </summary>
        LoadMap,
        /// <summary>
        /// 清除所有Goal點
        /// </summary>
        DeleteAllGoal,
        /// <summary>
        /// 更新車子當前位置
        /// </summary>
        RefreshPosCar,
        /// <summary>
        /// 更新掃描點
        /// </summary>
        RefreshScanPoint,
        /// <summary>
        /// 繪出掃描地圖
        /// </summary>
        DrawScanMap,
        /// <summary>
        /// 地圖修正完畢
        /// </summary>
        CorrectOriComplete,
        /// <summary>
        /// 原始圖檔載入中
        /// </summary>
        LoadingOri,
        /// <summary>
        /// 原始圖檔載入完畢
        /// </summary>
        LoadComplete,
        /// <summary>
        /// 開始載入Map檔
        /// </summary>
        StartLoadMap,
        /// <summary>
        /// 路徑繪製
        /// </summary>
        DrawPath
    }

    /// <summary>
    /// MapGL模式
    /// </summary>
    public enum GLMode {
        None,
        SetCar,
        Stop,
        Power,
        Erase,
        Cursor

    }

    /// <summary>
    /// 地圖相關事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MapEvent(object sender, MapEventArgs e);

    /// <summary>
    /// 地圖參數包
    /// </summary>
    public class MapDrawParam {
        /// <summary>
        /// 障礙線段集合
        /// </summary>
        public List<Line> ObstacleLines { get; }
        /// <summary>
        /// 障礙點集合
        /// </summary>
        public List<Point> ObstaclePoints { get; }

        /// <summary>
        /// Goal點集合
        /// </summary>
        public List<CartesianPos> Goals { get; }

        /// <summary>
        /// 一般建構方法
        /// </summary>
        /// <param name="obstacleLines">障礙線段集合</param>
        /// <param name="obstaclePoints">障礙點集合</param>
        /// <param name="goals">Goal點集合</param>
        public MapDrawParam(List<Line> obstacleLines, List<Point> obstaclePoints, List<CartesianPos> goals) {
            this.ObstacleLines = obstacleLines;
            this.ObstaclePoints = obstaclePoints;
            this.Goals = goals;
        }
    }

}
