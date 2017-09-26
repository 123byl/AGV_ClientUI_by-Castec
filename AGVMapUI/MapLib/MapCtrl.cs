using SharpGL;
using System;
using System.Collections.Generic;

namespace AGVMap
{
    /// <summary>
    /// 地圖控制器，所有使用者皆透過此介面對地圖操作
    /// </summary>
    public interface IMapCtrl
    {
        /// <summary>
        /// 是否顯示網格
        /// </summary>
        bool EnableGrid { get; set; }

        /// <summary>
        /// 滑鼠當下座標
        /// </summary>
        IPair MouseCurrentPos { get; }

        /// <summary>
        /// 滑鼠類型
        /// </summary>
        EMouseType MouseType { get; set; }

        /// <summary>
        /// 平移設定
        /// </summary>
        IPair Translate { get; set; }

        /// <summary>
        /// 縮放設定，Zoom 增加：畫面縮小；Zoom 減少：畫面放大
        /// </summary>
        double Zoom { get; set; }

        /// <summary>
        /// 加入 AGV 車
        /// </summary>
        void AddAGV(int key, CtrlAGV agv);

        /// <summary>
        /// 加入可控制區域
        /// </summary>
        void AddCtrlArea(int key, CtrlArea area);

        /// <summary>
        /// 加入可控制點
        /// </summary>
        void AddCtrlLine(int key, CtrlLine line);

        /// <summary>
        /// 加入可控制點
        /// </summary>
        void AddCtrlMark(int key, CtrlMark mark);

        /// <summary>
        /// 加入障礙線
        /// </summary>
        void AddObstacleLine(IEnumerable<ILine> lines);

        /// <summary>
        /// 加入障礙點
        /// </summary>
        void AddObstaclePoint(IEnumerable<IPoint> points);

        /// <summary>
        /// 設定地圖顯示中心
        /// </summary>
        void Focus(IPair focus);

        /// <summary>
        /// 隱藏 AGV 車
        /// </summary>
        void HideAGV(int key);

        /// <summary>
        /// 隱藏所有目標位置
        /// </summary>
        void HideAllGoals();

        /// <summary>
        /// 隱藏可控制區
        /// </summary>
        void HideCtrlArea(int key);

        /// <summary>
        /// 隱藏可控制線
        /// </summary>
        void HideCtrlLine(int key);

        /// <summary>
        /// 隱藏可控制點
        /// </summary>
        void HideCtrlMark(int key);

        /// <summary>
        /// 移除障礙線
        /// </summary>
        void RemoveObstacleLine();

        /// <summary>
        /// 移除障礙點
        /// </summary>
        void RemoveObstaclePoint();
    }

    /// <summary>
    /// 地圖控制器
    /// </summary>
    internal class MapCtrl : IMapCtrl, IDrawable
    {
        /// <summary>
        /// 最大放大倍率 = ZoomStep^0
        /// </summary>
        public const double ZoomMax = 1;

        /// <summary>
        /// 最小縮小倍率 = ZoomStep^6
        /// </summary>
        public const double ZoomMin = 34.012224;

        /// <summary>
        /// 縮放倍率
        /// </summary>
        public const double ZoomStep = 1.8;

        /// <summary>
        /// AGV 車管理器
        /// </summary>
        private CtrlAGVGM<int> mAGVs = new CtrlAGVGM<int>();

        /// <summary>
        /// 標示面管理器
        /// </summary>
        private CtrlAreaGM<int> mAreas = new CtrlAreaGM<int>();

        /// <summary>
        /// 拖曳管理器
        /// </summary>
        private DDragM mDragManager = Factory.Advance.CreatDDragM.DDragM();

        /// <summary>
        /// 標示線管理器
        /// </summary>
        private CtrlLineGM<int> mLines = new CtrlLineGM<int>();

        /// <summary>
        /// 線集合管理器
        /// </summary>
        private DLSetGM<int> mLineSets = new DLSetGM<int>();

        /// <summary>
        /// 標示物管理器
        /// </summary>
        private CtrlMarkGM<int> mMarks = new CtrlMarkGM<int>();

        /// <summary>
        /// 滑鼠當下座標
        /// </summary>
        private Pair mMouseCurrentPos = new Pair();

        /// <summary>
        /// 點集合管理器
        /// </summary>
        private DPSetGM<int> mPointSets = new DPSetGM<int>();

        /// <summary>
        /// 平移設定
        /// </summary>
        private Pair mTranslate = new Pair();

        /// <summary>
        /// 畫面縮放，Zoom 增加：畫面縮小；Zoom 減少：畫面放大
        /// </summary>
        private double mZoom = Math.Pow(ZoomStep, 4);

        /// <summary>
        /// 是否顯示網格
        /// </summary>
        public bool EnableGrid { get; set; } = true;

        /// <summary>
        /// 滑鼠當下座標
        /// </summary>
        public IPair MouseCurrentPos { get { return mMouseCurrentPos; } set { mMouseCurrentPos = new Pair(value); } }

        /// <summary>
        /// 滑鼠類型
        /// </summary>
        public EMouseType MouseType { get; set; } = EMouseType.TranslationMode;

        /// <summary>
        /// 障礙線預設引索
        /// </summary>
        public int ObstacleLineIndex { get; } = Factory.CreatID.NewID;

        /// <summary>
        /// 障礙點預設引索
        /// </summary>
        public int ObstaclePointIndex { get; } = Factory.CreatID.NewID;

        /// <summary>
        /// 平移設定
        /// </summary>
        public IPair Translate { get { return mTranslate; } set { mTranslate = new Pair(value); } }

        /// <summary>
        /// 縮放設定，Zoom 增加：畫面縮小；Zoom 減少：畫面放大
        /// </summary>
        public double Zoom {
            get {
                return mZoom;
            }

            set {
                if (value > ZoomMin)
                    mZoom = ZoomMin;
                else if (value < 1)
                    mZoom = 1;
                else
                    mZoom = value;
            }
        }

        /// <summary>
        /// 加入 AGV 車
        /// </summary>
        public void AddAGV(int key, CtrlAGV agv)
        {
            AddCtrl(mAGVs, key, agv);
        }

        /// <summary>
        /// 加入可控制區域
        /// </summary>
        public void AddCtrlArea(int key, CtrlArea area)
        {
            AddCtrl(mAreas, key, area);
        }

        /// <summary>
        /// 加入可控制點
        /// </summary>
        public void AddCtrlLine(int key, CtrlLine line)
        {
            AddCtrl(mLines, key, line);
        }

        /// <summary>
        /// 加入可控制點
        /// </summary>
        public void AddCtrlMark(int key, CtrlMark mark)
        {
            AddCtrl(mMarks, key, mark);
        }

        /// <summary>
        /// 加入障礙線
        /// </summary>
        public void AddObstacleLine(IEnumerable<ILine> lines)
        {
            if (mLineSets.ContainsKey(ObstacleLineIndex))
            {
                mLineSets.AddRange(ObstacleLineIndex, lines);
            }
            else
            {
                mLineSets.Add(ObstacleLineIndex, Factory.CreatObstacle.ObstacleLine(lines));
            }
        }

        /// <summary>
        /// 加入障礙點
        /// </summary>
        public void AddObstaclePoint(IEnumerable<IPoint> points)
        {
            if (mPointSets.ContainsKey(ObstaclePointIndex))
            {
                mPointSets.AddRange(ObstaclePointIndex, points);
            }
            else
            {
                mPointSets.Add(ObstaclePointIndex, Factory.CreatObstacle.ObstaclePoint(points));
            }
        }

        /// <summary>
        /// 獲得受控對象的名稱
        /// </summary>
        public string CurrentCtrlObjName()
        {
            return mDragManager.CurrentCtrlObjName;
        }

        /// <summary>
        /// 繪圖
        /// </summary>
        public void Draw(OpenGL gl)
        {
            mLines.Draw(gl);
            mAreas.Draw(gl);
            mDragManager.Draw(gl);

            mPointSets.Draw(gl);
            mLineSets.Draw(gl);
            mMarks.Draw(gl);
            mAGVs.Draw(gl);
        }

        /// <summary>
        /// 設定地圖顯示中心
        /// </summary>
        public void Focus(IPair focus)
        {
            Translate.X = -focus.X;
            Translate.Y = -focus.Y;
        }

        /// <summary>
        /// 隱藏 AGV 車
        /// </summary>
        public void HideAGV(int key)
        {
            mAGVs.Edit((item) => { if (item.Key == key) item.Value.Visible = false; });
        }

        /// <summary>
        /// 隱藏所有目標位置
        /// </summary>
        public void HideAllGoals()
        {
            mMarks.Edit((item) => { if (item.Value.Property.Type == EMarkType.Goal) item.Value.Visible = false; });
        }

        /// <summary>
        /// 隱藏可控制區
        /// </summary>
        public void HideCtrlArea(int key)
        {
            mAreas.Edit((item) => { if (item.Key == key) item.Value.Visible = false; });
        }

        /// <summary>
        /// 隱藏可控制線
        /// </summary>
        public void HideCtrlLine(int key)
        {
            mLines.Edit((item) => { if (item.Key == key) item.Value.Visible = false; });
        }

        /// <summary>
        /// 隱藏可控制點
        /// </summary>
        public void HideCtrlMark(int key)
        {
            mMarks.Edit((item) => { if (item.Key == key) item.Value.Visible = false; });
        }

        /// <summary>
        /// 移除障礙線
        /// </summary>
        public void RemoveObstacleLine()
        {
            mLineSets.Remove(ObstacleLineIndex);
        }

        /// <summary>
        /// 移除障礙點
        /// </summary>
        public void RemoveObstaclePoint()
        {
            mPointSets.Remove(ObstaclePointIndex);
        }

        /// <summary>
        /// 是否有被選中的控制點
        /// </summary>
        internal bool DragHasSelectedCtrlPoint()
        {
            return mDragManager.HasSelectedCtrlPoint();
        }

        /// <summary>
        /// 拖曳起始位置
        /// </summary>
        internal void DragMouseDown(IPair mousePos)
        {
            mDragManager.MouseDown(mousePos);
        }

        /// <summary>
        /// 拖曳移動
        /// </summary>
        internal void DragMouseMoving(IPair mousePos)
        {
            mDragManager.MouseMoving(mousePos);
        }

        /// <summary>
        /// 拖曳結束
        /// </summary>
        internal void DragMouseUp()
        {
            mDragManager.MouseUp();
        }

        /// <summary>
        /// 設定拖曳對象
        /// </summary>
        internal void DragSetCtrlObj(ICtrlable obj)
        {
            mDragManager.SetCtrlObj(obj);
        }

        /// <summary>
        /// 根據座標尋找控制對象
        /// </summary>
        internal ICtrlable FindControllTarget(IPair p)
        {
            CtrlAGV agv = null;
            if (FindControllTarget(mAGVs, ref agv, p)) return agv;

            CtrlMark mark = null;
            if (FindControllTarget(mMarks, ref mark, p)) return mark;

            CtrlLine line = null;
            if (FindControllTarget(mLines, ref line, p)) return line;

            CtrlArea area = null;
            if (FindControllTarget(mAreas, ref area, p)) return area;

            return null;
        }

        /// <summary>
        /// 加入可控元件
        /// </summary>
        private void AddCtrl<TValue>(CtrlGM<int, TValue> gm, int key, TValue obj) where TValue : ICtrlable
        {
            if (obj == null) return;
            gm.Add(key, obj);
            TValue find = default(TValue);
            bool res = gm.Find((item) => { return item.Key == key; }, ref find);
            if (res) mDragManager.SetCtrlObj(find);
        }

        /// <summary>
        /// 根據座標尋找控制對象
        /// </summary>
        private bool FindControllTarget<TValue>(CtrlGM<int, TValue> gm, ref TValue res, IPair p) where TValue : ICtrlable
        {
            return gm.Find((item) =>
            {
                return item.Value.Intersect(p);
            }, ref res);
        }
    }
}