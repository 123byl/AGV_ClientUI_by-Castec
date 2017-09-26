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
    public partial class CtMapGL : CtDockContent,IIMapGL {

        private List<CarPos> mPtCar = null;

        private List<string> mStrCar = null;

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

        #region Declaration - Events

        public event DelMouseClickRealPos MouseClickRealPos{
            add {
                glMap.MouseClickRealPos += value;
            }
            remove {
                glMap.MouseClickRealPos -= value;
            }
        }

        public event DelMouseSelectObj MouseSelectObj {
            add {
                glMap.MouseSelectObj += value;
            }
            remove {
                glMap.MouseSelectObj -= value;
            }
        }

        public event DelMouseSelectRange MouseSelectRange {
            add {
                glMap.MouseSelectRange += value;
            }
            remove {
                glMap.MouseSelectRange -= value;
            }
        }

        #endregion Declaration - Events

        #region Function - Events

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
        CartesianPos pos = new CartesianPos();
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

                mPtCar.Add(new CarPos(posX, posY));
                mStrCar.Add($"CarTemp{mPtCar.Count}");
                if (mPtCar.Count == 2) {
                    #region - car pos setting -

                    double Calx = posX - mPtCar[0].x;
                    double Caly = posY - mPtCar[0].y;
                    double Calt = Math.Atan2(Caly, Calx) * 180 / Math.PI;

                    //SetPosition(mPtCar[0].X, mPtCar[0].Y, Calt);
                    glMap.PosCar = new Pos(mPtCar[0].x, mPtCar[0].y, Calt);
                    //Send POS to AGV   
                    rActFunc?.SetPosition(mPtCar[0].x, mPtCar[0].y, Calt);
                    glMap.RemoveGroupPoint(StrCar[0], StrCar[1]);

                    StrCar.Clear();
                    SetGLMode(GLMode.None);
                    glMap.EnableCar = true;

                    #endregion

                    mPtCar.Clear();
                } else {
                    glMap.DrawPoint(Convert.ToInt32(posX), Convert.ToInt32(posY), Color.Blue, 3, 3, StrCar[mPtCar.Count - 1]);
                }
            }
        }


        #endregion Function - Events

        #region Funciton - Public Methods

        /// <summary>
        /// 增加Goal點
        /// </summary>
        /// <param name="goal"></param>
        public void AddGoalPos(Info goal) {
            glMap.AddPositonGoal(new Pos(goal.X,goal.Y,goal.Toward));
        }

        public void LoadMap(List<Line>obstacleLines ,List<Point> obstaclePoints,List<CartesianPos> goals) {
            ClearMap();
            glMap.RemoveAllPositonGoal();
            DrawObstacle(obstacleLines, obstaclePoints);
            glMap.AddPositionGoals(goals);
        }


        public void ClearMap() {
            glMap.RemoveAllLines();
            glMap.RemoveAllPoints();
        }

        /// <summary>
        /// 障礙繪製
        /// </summary>
        /// <param name="obstacleLines">障礙線段集合</param>
        /// <param name="obstaclePoints">障礙點集合</param>
        public void DrawObstacle(List<Line> obstacleLines, List<Point> obstaclePoints) {
            glMap.DrawLines(obstacleLines, Color.Black, "ObstacleLines", true);
            glMap.DrawPoints(obstaclePoints, Color.Black, "ObstaclePoints", 1);
        }

        /// <summary>
        /// Ori載入中
        /// </summary>
        /// <param name="dic"></param>
        public void LoadOri(Pos carPos, List<CartesianPos> scanPoints) {
            glMap.PosCar = carPos;
            DrawPoints(scanPoints, Color.Black, 2, 1, "MAP");
        }
        #endregion Funciton - Public Methods

        #region Funciton - Private Methods



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
        #endregion Function - Private Methods

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
