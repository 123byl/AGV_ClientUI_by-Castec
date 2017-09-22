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
    /// MapGL功能接口
    /// </summary>
    public interface IMapGL {
        
        /// <summary>
        /// 原始地圖路徑
        /// </summary>
        string CurOriPath { get; set; }

        /// <summary>
        /// 車子資料
        /// </summary>
        CarInfo CarInfo { get; }
        
        /// <summary>
        /// 預設地圖檔資料夾路徑
        /// </summary>
        string DefMapDir { get; }

        /// <summary>
        /// GL模式
        /// </summary>
        GLMode GLMode { get; set; }

        /// <summary>
        /// 要新增的點位
        /// </summary>
        CarPos AddPos { get; }

        List<CarPos> PtCar { get; set; }

        List<string> StrCar { get; set; }

        /// <summary>
        /// 地圖相關事件觸發
        /// </summary>
        event MapEvent MapEventTrigger;

        /// <summary>
        /// 設定要新增點位座標
        /// </summary>
        /// <param name="posX">X座標</param>
        /// <param name="posY">Y座標</param>
        void SetAddPos(double posX,double posY);
        
        /// <summary>
        /// 設定位置
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="theta">夾角</param>
        void SetPosition(double x, double y, double theta);



    }

    /// <summary>
    /// 地圖顯示介面
    /// </summary>
    public partial class CtMapGL : CtDockContent<IMapGL> {

        #region Declaration - Fields

        /// <summary>
        /// 障礙點
        /// </summary>
        private List<Point> ObstaclePoint = new List<Point>();

        /// <summary>
        /// 障礙線段
        /// </summary>
        private List<Line> ObstacleLine = new List<Line>();

        /// <summary>
        /// 當前地圖檔路徑(rActFunc無參考時使用)
        /// </summary>
        private string mCurOriPath = string.Empty;

        /// <summary>
        /// 地圖檔資料夾路徑(rActFunc無參考時使用)
        /// </summary>
        private string mDefMapDir = @"D:\MapInfo\";

        /// <summary>
        /// GL模式，rActFunc無參考時使用
        /// </summary>
        private GLMode mGLMode = GLMode.Cursor;

        private MapMatching mapMatch = new MapMatching();

        private List<CarPos> mPtCar = null;

        private List<string> mStrCar = null;
        
        /// <summary>
        /// 要新增的點位，rActFunc無參考時使用
        /// </summary>
        private CarPos mAddPos = null;

        #endregion Declaration - Fields

        #region Declaration - Properties

        /// <summary>
        /// 預設地圖檔資料夾路徑
        /// </summary>
        private string DefMapDir {
            get {
                return rActFunc?.DefMapDir ?? mDefMapDir;
            }
        }

        /// <summary>
        /// 地圖檔路徑
        /// </summary>
        private string CurOriPath {
            get {
                return rActFunc?.CurOriPath ?? mCurOriPath;
            }
            set {
                if (rActFunc != null) {
                    rActFunc.CurOriPath = value;
                } else {
                    mCurOriPath = value;
                }
            }
        }

        /// <summary>
        /// GL模式
        /// </summary>
        private GLMode GLMode {
            get {
                return rActFunc?.GLMode ?? mGLMode;
            }
            set {
                if (rActFunc != null) {
                    rActFunc.GLMode = value;
                } else {
                    mGLMode = value;
                }
            }
        }

        private List<CarPos> PtCar {
            get {
                return rActFunc?.PtCar ?? mPtCar;
            }
            set {
                if (rActFunc != null) {
                    rActFunc.PtCar = value;
                } else {
                    mPtCar = value;
                }
            }
        }

        private List<string> StrCar {
            get {
                return rActFunc?.StrCar ?? mStrCar;
            }
            set {
                mStrCar = value;
            }
        }

        /// <summary>
        /// 要新增的點位
        /// </summary>
        private CarPos AddPos {
            get {
                return rActFunc?.AddPos ?? mAddPos;
            }
        }

        #endregion Declaration - Properties

        #region Function - Constructors

        /// <summary>
        /// 空白建構方法
        /// </summary>
        /// <param name="defState">預設停靠狀態</param>
        public CtMapGL(DockState defState = DockState.Float) : this(null, null, defState) {

        }

        /// <summary>
        /// 傳入<see cref="IMapGL"/>參考進行建構
        /// </summary>
        /// <param name="mapGL"><see cref="IMapGL"/>參考</param>
        /// <param name="defState">預設的停靠狀態，不可為Unknown</param>
        public CtMapGL(IMapGL mapGL, DockState defState = DockState.Float)
            : this(mapGL, null, defState) {
        }

        /// <summary>
        /// 共用建構方法
        /// </summary>
        /// <param name="mapGL"></param>
        /// <param name="main"></param>
        /// <param name="defState"></param>
        public CtMapGL(IMapGL mapGL, AgvClientUI main, DockState defState = DockState.Float)
            : base(mapGL, main, defState) {
            InitializeComponent();
            /*-- 無動作方法參考則自行新增變數實例 --*/
            //if (rActFunc == null) {
            //    mPtCar = new List<Point>();
            //    mStrCar = new List<string>();
            //}
        }

        #endregion Function - Constructors

        #region Funciton - Events

        #region rActFunc

        /// <summary>
        /// 地圖事件處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rActFunc_OnMapEventTrigger(object sender, MapEventArgs e) {
            switch (e.Type) {
                case MapEventType.CarInfoRefresh:
                    InfoRefresh((CarInfo)e.Value);
                    break;
                case MapEventType.ClearMapGL:
                    ClearMap();
                    break;
                case MapEventType.LoadingOri:
                    LoadingOri(e.Value as Dictionary<string,object>);
                    break;
                case MapEventType.GetLaser:
                    GetLaser((CarInfo)e.Value);
                    break;
                case MapEventType.CarPosConfirm:
                    CarPosConfirm(e.Value as Dictionary<string,object>);
                    break;
                case MapEventType.GLMode:
                    SetGLMode((GLMode)e.Value);
                    break;
                case MapEventType.AddGoalPos:
                    AddGoalPos(e.Value as CarPos);
                    break;
                case MapEventType.SimplifyOri:
                    SimplifyOri(e.Value as Dictionary<string,object>);
                    break;
                case MapEventType.LoadMap:
                    LoadMap(e.Value as Dictionary<string,object>);
                    break;
                case MapEventType.DeleteAllGoal:
                    DeleteAllGoal();
                    break;
                case MapEventType.RefreshPosCar:
                    RefreshPosCar(e.Value as Pos);
                    break;
                case MapEventType.RefreshScanPoint:
                    RefreshScanPoint(e.Value as List<CartesianPos>);
                    break;
                case MapEventType.DrawScanMap:
                    DrawScanMap(e.Value as List<CartesianPos>);
                    break;
                case MapEventType.CorrectOriComplete:
                    CorrectOriComplete();
                    break;
                case MapEventType.DrawPath:
                    DrawPath(e.Value as List<Line>);
                    break;
            }
        }
        
        #endregion rActFunc

        #region Form

        /// <summary>
        /// 表單載入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CtMapGL_Load(object sender, EventArgs e) {
            glMap.PosCar = new Pos(0, 0);
            glMap.Fit();
        }

        #endregion Form

        #region glMap

        /// <summary>
        /// 物件被點選
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void glMap_MouseSelectObj(string name, double x, double y) {
            string[] itemName = name.Split('_');

            // 地圖回傳誰被按下
            string msg = string.Format($"Delete {itemName[0]}({x},{y}) or not?");

            MsgBoxButton dr = CtMsgBox.Show(msg, "Delete", MsgBoxButton.OK_CANCEL, MsgBoxStyle.INFORMATION);

            //判斷使用者是否要刪除點位
            if (dr == MsgBoxButton.OK) {
                if (itemName[0] == CastecMapUI.ItemLayout.Goal.ToString()) {
                    glMap.RemovePositonGoal(uint.Parse(itemName[1]));
                } else if (itemName[0] == CastecMapUI.ItemLayout.Power.ToString()) {
                    glMap.RemovePositonPower(uint.Parse(itemName[1]));
                }
            }
        }

        /// <summary>
        /// 選取事件
        /// </summary>
        /// <param name="beginX"></param>
        /// <param name="beginY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        private void glMap_MouseSelectRange(int beginX, int beginY, int endX, int endY) {
            // 當 GL 畫布中有被框選的時候會回傳框選範圍座標，可用全域變數接下 beginX、beginY、endX、endY
            Pos pos = new Pos();
            Size frame = new Size();
            //DefineArea.Add(new Point(beginX, beginY));
            //DefineArea.Add(new Point(endX, endY));
            //繪製工作區域設定(顏色區別)
            pos = new Pos((beginX + endX) / 2, (beginY + endY) / 2);
            frame = new Size(Math.Abs(endX - beginX), Math.Abs(endY - beginY));
            switch (GLMode) {
                case GLMode.Erase:
                    glMap.DrawRectangleFillFull(pos, frame, Color.White, "Erase", 1, (int)ItemLayout.Line);
                    break;
                case GLMode.Stop:
                    glMap.DrawRectangleFillFull(pos, frame, Color.Red, "Stop", 1, (int)ItemLayout.Point);
                    break;
                case GLMode.Power:
                    glMap.DrawRectangleFillFull(pos, frame, Color.Green, "Power", 1, (int)ItemLayout.Point);
                    break;
            }
            pos = null;

        }

        /// <summary>
        /// 點擊事件
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        private void glMap_MouseClickRealPos(double posX, double posY) {
            /*-- 設定要新增的點位 --*/
            SetAddPos(posX, posY);
            if (GLMode == GLMode.SetCar) {
                glMap.EnableCar = false;
                
                PtCar.Add(new CarPos(posX,posY));
                StrCar.Add($"CarTemp{PtCar.Count}");
                if (PtCar.Count == 2) {
                    #region - car pos setting -

                    double Calx = posX - PtCar[0].x;
                    double Caly = posY - PtCar[0].y;
                    double Calt = Math.Atan2(Caly, Calx) * 180 / Math.PI;

                    //SetPosition(ptCar[0].X, ptCar[0].Y, Calt);
                    glMap.PosCar = new Pos(PtCar[0].x, PtCar[0].y, Calt);
                    //Send POS to AGV   
                    rActFunc?.SetPosition(PtCar[0].x, PtCar[0].y, Calt);
                    glMap.RemoveGroupPoint(StrCar[0], StrCar[1]);

                    StrCar.Clear();
                    SetGLMode(GLMode.None);
                    glMap.EnableCar = true;

                    #endregion

                    PtCar.Clear();
                } else {
                    glMap.DrawPoint(Convert.ToInt32(posX), Convert.ToInt32(posY), Color.Blue, 3, 3, StrCar[PtCar.Count - 1]);
                }
            }
        }

        #endregion glMap

        #endregion Function - Events

        #region Function - Public Methods

        /// <summary>
        /// 增加Goal點
        /// </summary>
        /// <param name="goal"></param>
        public void AddGoalPos(CarPos goal) {
            glMap.AddPositonGoal(goal.ToPos());
        }

        /// <summary>
        /// 設定GL模式
        /// </summary>
        /// <param name="mode"></param>
        public void SetGLMode(GLMode mode) {
            GLMode = mode;
            switch (mode) {
                case GLMode.SetCar:

                    break;
                case GLMode.Cursor:
                    glMap.GoalShape = Shape.Rectangle;
                    glMap.RemovePolygonFillFull("Erase");
                    glMap.RemovePolygonFillFull("Stop");
                    glMap.RemovePolygonFillFull("Power");
                    break;
                case GLMode.Erase:
                    glMap.GoalShape = Shape.Cursor;
                    glMap.RemovePolygonFillFull("Erase");
                    break;
                case GLMode.Stop:
                    glMap.GoalShape = Shape.Cursor;
                    break;
                case GLMode.Power:
                    glMap.GoalShape = Shape.Cursor;
                    glMap.RemovePolygonFillFull("Erase");
                    break;
            }
        }

        /// <summary>
        /// 車子位置確認
        /// </summary>
        public void CarPosConfirm(Dictionary<string,object>dic) {
            glMap.RemoveGroupPoint("LaserLength");
            DrawPoints(dic.ReadVar<List<CartesianPos>>(VarDef.ScanPoint), Color.Red, "LaserLength", 2);
            glMap.PosCar = dic.ReadVar<Pos>(VarDef.PosCar);
        }

        /// <summary>
        /// 地圖簡化
        /// </summary>
        /// <remarks>
        /// Modified by Jay 2017/09/13
        /// </remarks>
        public void SimplifyOri(Dictionary<string,object> dic) {
            ClearMap();
            //glMap.PosCar = dic.ReadVar<Pos>(VarDef.PosCar);
            DrawObstacle(dic.ReadVar<List<Line>>(VarDef.ObstacleLines),dic.ReadVar<List<Point>>(VarDef.ObstaclePoints));
            rMain.SetBalloonTip("Simplify Map", "Simplify Complete!!", ToolTipIcon.Info,10);
        }

        /// <summary>
        /// 清除地圖
        /// </summary>
        public void ClearMap() {
            glMap.RemoveAllLines();
            glMap.RemoveAllPoints();
        }
        
        /// <summary>
        /// 清除所有Goal點
        /// </summary>
        public void DeleteAllGoal() {
            glMap.RemoveAllPositonGoal();
            glMap.RemoveGroupLine("PathLine");
        }

        #endregion Function - Public Methods

        #region Funciton - Private Methods

        /// <summary>
        /// 繪製路徑
        /// </summary>
        /// <param name="linePath">路徑線段集合</param>
        private void DrawPath(List<Line> linePath) {
            glMap.RemoveGroupLine("PATH");
            glMap.DrawLines(linePath, Color.Green, "PATH", false, 1, 4);
        }

        /// <summary>
        /// 載入地圖
        /// </summary>
        private void LoadMap(Dictionary<string, object> dic) {
            ClearMap();
            glMap.RemoveAllPositonGoal();
            DrawObstacle(dic.ReadVar<List<Line>>(VarDef.ObstacleLines), dic.ReadVar<List<Point>>(VarDef.ObstaclePoints));
            glMap.AddPositionGoals(dic.ReadVar<List<CartesianPos>>(VarDef.Goals));
        }

        /// <summary>
        /// 障礙繪製
        /// </summary>
        /// <param name="obstacleLines">障礙線段集合</param>
        /// <param name="obstaclePoints">障礙點集合</param>
        private void DrawObstacle(List<Line> obstacleLines,List<Point> obstaclePoints) {
            glMap.DrawLines(obstacleLines, Color.Black, "ObstacleLines", true);
            glMap.DrawPoints(obstaclePoints, Color.Black, "ObstaclePoints", 1);
        }

        /// <summary>
        /// 設定要新增的點位座標
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        private void SetAddPos(double x,double y) {
            if (rActFunc != null) {
                rActFunc.SetAddPos(x, y);
            }else {
                mAddPos.x = x;
                mAddPos.y = y;
            }
        }

        /// <summary>
        /// 畫多點
        /// </summary>
        /// <param name="carPos"><see cref="CartesianPos"/>點集合</param>
        /// <param name="color">使用顏色</param>
        /// <param name="pointSize">點尺寸</param>
        /// <param name="layer">圖層數</param>
        /// <param name="nameGroup">群組名稱</param>
        private void DrawPoints(List<CartesianPos> carPos, Color color, float pointSize, int layer, string nameGroup) {
            DrawPoints(carPos, color, nameGroup, pointSize, layer);
        }

        /// <summary>
        /// 畫多點
        /// </summary>
        /// <param name="carPos"></param>
        /// <param name="color"></param>
        /// <param name="nameGroup"></param>
        /// <param name="pointSize"></param>
        /// <param name="layer"></param>
        private void DrawPoints(List<CartesianPos> carPos, Color color, string nameGroup, float pointSize, int layer = -1) {
            List<Point> points = carPos.ConvertAll(v => v.ToPoint());
            if (layer != -1) {
                glMap.DrawPoints(points, color, nameGroup, pointSize, false, layer);
            } else {
                glMap.DrawPoints(points, color, nameGroup, pointSize);
            }
        }

        /// <summary>
        /// 事件訂閱
        /// </summary>
        protected override void AddEvent() {
            /*-- 事件訂閱 --*/
            rActFunc.MapEventTrigger += rActFunc_OnMapEventTrigger;
            /*-- 若有實作IAgvClient與ICtVersion接口則註冊MapGL版本 --*/
            if (rActFunc is IAgvClient && glMap is ICtVersion) {
                /*-- 登記MapGL版本 --*/
                (rActFunc as IAgvClient).RegisterVersion(
                    glMap.GetType().Name,//模組名稱
                    (glMap as ICtVersion).Version.FullString//版本號
                );
            }
        }

        /// <summary>
        /// 取消訂閱事件
        /// </summary>
        protected override void RemoveEvent() {
            rActFunc.MapEventTrigger -= rActFunc_OnMapEventTrigger;
        }

        /// <summary>
        /// 車子資訊更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoRefresh(CarInfo info) {
            double angle;
            double Laserangle;

            /*-- 設定車子位置資訊 --*/
            glMap.PosCar = new Pos(info.X, info.Y, info.ThetaGyro);

            /*-- 畫面清除 --*/
            ObstaclePoint.Clear();
            glMap.RemoveGroupPoint("LaserLength");

            /*-- 繪製雷射資料 --*/
            int idx = 0;

            ObstaclePoint = info.LaserData.ToList().ConvertAll(dist => {
                int[] pos = Transformation.LaserPoleToCartesian(
                        dist,
                        -135,
                        0.25,
                        idx++,
                         43, 416.75, 43,
                        info.X, info.Y, info.ThetaGyro,
                        out angle, out Laserangle);
                return new Point(pos[0],pos[1]);
            });

            glMap.DrawPoints(ObstaclePoint, Color.Red, "LaserLength", 2);
        }

        /// <summary>
        /// 繪製雷射資料
        /// </summary>
        /// <param name="laserData">雷射距離資料</param>
        private void GetLaser(CarInfo info) {
            ObstaclePoint.Clear();
            glMap.RemoveGroupPoint("LaserLength");
            int idx = 0;
            double angle = 0D, Laserangle = 0D;
            foreach (int dist in info.LaserData) {
                if (dist >= 30 && dist < 15000) {
                    int[] pos = Transformation.LaserPoleToCartesian(dist, -135, 0.25, idx++, 43, 416.75, 43, info.X, info.Y, info.ThetaGyro, out angle, out Laserangle);//, out dataAngle, out laserAngle);
                    ObstaclePoint.Add(new Point(pos[0], pos[1]));
                    pos = null;
                }
            }
            glMap.DrawPoints(ObstaclePoint, Color.Red, "LaserLength", 2);
        }
        
        /// <summary>
        /// 更新車子位置
        /// </summary>
        /// <param name="posCar">車子位置</param>
        private void RefreshPosCar(Pos posCar) {
            glMap.PosCar = posCar;
        }

        /// <summary>
        /// 更新掃描點
        /// </summary>
        /// <param name="list">掃描點</param>
        private void RefreshScanPoint(List<CartesianPos> scanPoints) {
            glMap.RemoveGroupPoint("ScanPoint");
            //scanPoint.Clear();
            DrawPoints(scanPoints, Color.Red, 1, 3, "ScanPoint");
        }

        /// <summary>
        /// 繪出掃描地圖
        /// </summary>
        /// <param name="points">地圖點集合</param>
        private void DrawScanMap(List<CartesianPos> points) {
            DrawPoints(points, Color.Black, 1, 3, "ScanMap");
        }

        /// <summary>
        /// 地圖修正完畢
        /// </summary>
        private void CorrectOriComplete() {
            glMap.RemoveGroupPoint("ScanPoint");
            glMap.PosCar = new Pos(0, 0, 0);
            rMain.SetBalloonTip("Correct Map", "Correct Complete!!", ToolTipIcon.Info, 10);
        }

        /// <summary>
        /// Ori載入中
        /// </summary>
        /// <param name="dic"></param>
        private void LoadingOri(Dictionary<string,object> dic) {
            glMap.PosCar = dic.ReadVar<Pos>(VarDef.PosCar);
            DrawPoints(dic.ReadVar<List<CartesianPos>>(VarDef.ScanPoint),Color.Black,2,1,"MAP");
        }

        #endregion Function - Private Methods

        #region Support class

        /// <summary>
        /// 地圖參數類
        /// </summary>
        public class MapParam {
            /// <summary>
            /// 地圖路徑
            /// </summary>
            public string Path { get; }
            /// <summary>
            /// 參數值
            /// </summary>
            public object Param { get; }

            /// <summary>
            /// 一般建構方法
            /// </summary>
            /// <param name="path">地圖路徑</param>
            /// <param name="param">參數值</param>
            public MapParam(string path, object param) {
                this.Path = path;
                this.Param = param;
            }
        }

        #endregion Support class
        
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
