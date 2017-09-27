using SharpGL;
using System;
using System.Collections.Generic;
using static AGVMap.Events;

namespace AGVMap
{
    /// <summary>
    /// 地圖控制器，所有使用者皆透過此介面對地圖操作
    /// </summary>
    public interface IMapCtrl : IMapEvents
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
        void AddAGV(CtrlAGV agv);

        /// <summary>
        /// 加入面集合
        /// </summary>
        void AddAreasSet(DASet areas);

        /// <summary>
        /// 加入可控制區域
        /// </summary>
        void AddCtrlArea(CtrlArea area);

        /// <summary>
        /// 加入可控制點
        /// </summary>
        void AddCtrlLine(CtrlLine line);

        /// <summary>
        /// 加入可控制點
        /// </summary>
        void AddCtrlMark(CtrlMark mark);

        /// <summary>
        /// 加入線集合
        /// </summary>
        void AddLinesSet(DLSet lines);

        /// <summary>
        /// 加入點集合
        /// </summary>
        void AddPointsSet(DPSet points);

        /// <summary>
        /// 設定地圖顯示中心
        /// </summary>
        void Focus(IPair focus, double zoom);

        /// <summary>
        /// 設定地圖顯示中心
        /// </summary>
        void Focus(IPair focus);

        /// <summary>
        /// 獲得控制項的簡易資訊
        /// </summary>
        List<SimpleInfo> GetCtrlSimpleInfo();

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
        /// 建立新地圖(刪除所有資料)
        /// </summary>
        void NewMap();

        /// <summary>
        /// 建立新地圖(刪除所有資料)
        /// </summary>
        void NewMap(IEnumerable<IPoint> obstaclePoints, IEnumerable<ILine> obstacleLines);

        /// <summary>
        /// 移除面集合
        /// </summary>
        void RemoveAreasSet(int id);

        /// <summary>
        /// 移除線集合
        /// </summary>
        void RemoveLinesSet(int id);

        /// <summary>
        /// 移除點集合
        /// </summary>
        void RemovePointsSet(int id);
    }

    /// <summary>
    /// 地圖控制器 - 地圖事件
    /// </summary>
    public interface IMapEvents
    {
        /// <summary>
        /// 滑鼠拖動標示面
        /// </summary>
        event DelAreaDraged AreaDragedEvent;

        /// <summary>
        /// 滑鼠拖動標示線
        /// </summary>
        event DelLineDraged LineDragedEvent;

        /// <summary>
        /// 滑鼠拖動標示物
        /// </summary>
        event DelMarkDraged MarkDragedEvent;

        /// <summary>
        /// 滑鼠按下位置
        /// </summary>
        event DelMouseDown MouseDownEvent;

        /// <summary>
        /// 滑鼠按下移動位置
        /// </summary>
        event DelMouseDownMove MouseDownMoveEvent;

        /// <summary>
        /// 滑鼠放開位置
        /// </summary>
        event DelMouseUp MouseUpEvent;
    }

    /// <summary>
    /// 簡易資訊結構
    /// </summary>
    public struct SimpleInfo
    {
        public SimpleInfo(int id, string name, IPair pos)
        {
            ID = id;
            Name = name;
            Center = pos;
        }

        /// <summary>
        /// 中心位置
        /// </summary>
        public IPair Center { get; }

        /// <summary>
        /// 識別碼
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// 顯示名稱
        /// </summary>
        public string Name { get; }
    }

    /// <summary>
    /// 事件集合
    /// </summary>
    public static class Events
    {
        /// <summary>
        /// 滑鼠拖動標示面
        /// </summary>
        public delegate void DelAreaDraged(EAreaType type, int id, IArea area);

        /// <summary>
        /// 滑鼠拖動標示線
        /// </summary>
        public delegate void DelLineDraged(ELineType type, int id, ILine line);

        /// <summary>
        /// 滑鼠拖動標示物
        /// </summary>
        public delegate void DelMarkDraged(EMarkType type, int id, IPair center, Angle toward);

        /// <summary>
        /// 滑鼠按下位置
        /// </summary>
        public delegate void DelMouseDown(IPair realPos);

        /// <summary>
        /// 滑鼠按下移動位置
        /// </summary>
        public delegate void DelMouseDownMove(IPair realPos);

        /// <summary>
        /// 滑鼠放開位置
        /// </summary>
        public delegate void DelMouseUp(IPair realPos);
    }

    /// <summary>
    /// 地圖控制器
    /// </summary>
    internal class MapCtrl : IMapCtrl, IDrawable
    {
        /// <summary>
        /// 最大放大倍率
        /// </summary>
        public const double ZoomMax = 1;

        /// <summary>
        /// 最小縮小倍率
        /// </summary>
        public const double ZoomMin = 64;

        /// <summary>
        /// 縮放倍率
        /// </summary>
        public const double ZoomStep = 1.1;

        /// <summary>
        /// AGV 車管理器
        /// </summary>
        private CtrlAGVGM<int> mAGVs = new CtrlAGVGM<int>();

        /// <summary>
        /// 標示面管理器
        /// </summary>
        private CtrlAreaGM<int> mAreas = new CtrlAreaGM<int>();

        /// <summary>
        /// 面集合管理器
        /// </summary>
        private DASetGM<int> mAreaSet = new DASetGM<int>();

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
        /// 滑鼠拖動標示面
        /// </summary>
        public event DelAreaDraged AreaDragedEvent;

        /// <summary>
        /// 滑鼠拖動標示線
        /// </summary>
        public event DelLineDraged LineDragedEvent;

        /// <summary>
        /// 滑鼠拖動標示物
        /// </summary>
        public event DelMarkDraged MarkDragedEvent;

        /// <summary>
        /// 滑鼠按下位置
        /// </summary>
        public event DelMouseDown MouseDownEvent;

        /// <summary>
        /// 滑鼠按下移動位置
        /// </summary>
        public event DelMouseDownMove MouseDownMoveEvent;

        /// <summary>
        /// 滑鼠放開位置
        /// </summary>
        public event DelMouseUp MouseUpEvent;

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
        public void AddAGV(CtrlAGV agv)
        {
            AddCtrl(mAGVs, agv.ID, agv);
        }

        /// <summary>
        /// 加入面集合
        /// </summary>
        public void AddAreasSet(DASet areas)
        {
            if (mAreaSet.ContainsKey(areas.ID))
            {
                mAreaSet.AddRange(areas.ID, areas.AsReadOnly());
            }
            else
            {
                mAreaSet.Add(areas.ID, areas);
            }
        }

        /// <summary>
        /// 加入可控制區域
        /// </summary>
        public void AddCtrlArea(CtrlArea area)
        {
            AddCtrl(mAreas, area.ID, area);
        }

        /// <summary>
        /// 加入可控制點
        /// </summary>
        public void AddCtrlLine(CtrlLine line)
        {
            AddCtrl(mLines, line.ID, line);
        }

        /// <summary>
        /// 加入可控制點
        /// </summary>
        public void AddCtrlMark(CtrlMark mark)
        {
            AddCtrl(mMarks, mark.ID, mark);
        }

        /// <summary>
        /// 加入線集合
        /// </summary>
        public void AddLinesSet(DLSet lines)
        {
            if (mLineSets.ContainsKey(lines.ID))
            {
                mLineSets.AddRange(lines.ID, lines.AsReadOnly());
            }
            else
            {
                mLineSets.Add(lines.ID, lines);
            }
        }

        /// <summary>
        /// 加入點集合
        /// </summary>
        public void AddPointsSet(DPSet points)
        {
            if (mPointSets.ContainsKey(points.ID))
            {
                mPointSets.AddRange(points.ID, points.AsReadOnly());
            }
            else
            {
                mPointSets.Add(points.ID, points);
            }
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
        public void Focus(IPair focus, double zoom)
        {
            Focus(focus);
            Zoom = zoom;
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
        /// 獲得控制項的簡易資訊
        /// </summary>
        public List<SimpleInfo> GetCtrlSimpleInfo()
        {
            List<SimpleInfo> res = new List<SimpleInfo>();
            res.AddRange(GetCtrlSimpleInfo(mAGVs));
            res.AddRange(GetCtrlSimpleInfo(mMarks));
            res.AddRange(GetCtrlSimpleInfo(mLines));
            res.AddRange(GetCtrlSimpleInfo(mAreas));
            return res;
        }

        /// <summary>
        /// 隱藏 AGV 車
        /// </summary>
        public void HideAGV(int key)
        {
            mDragManager.SetCtrlObj(null);
            mAGVs.Edit((item) => { if (item.Key == key) item.Value.Visible = false; });
        }

        /// <summary>
        /// 隱藏所有目標位置
        /// </summary>
        public void HideAllGoals()
        {
            mDragManager.SetCtrlObj(null);
            mMarks.Edit((item) => { if (item.Value.Property.Type == EMarkType.Goal) item.Value.Visible = false; });
        }

        /// <summary>
        /// 隱藏可控制區
        /// </summary>
        public void HideCtrlArea(int key)
        {
            mDragManager.SetCtrlObj(null);
            mAreas.Edit((item) => { if (item.Key == key) item.Value.Visible = false; });
        }

        /// <summary>
        /// 隱藏可控制線
        /// </summary>
        public void HideCtrlLine(int key)
        {
            mDragManager.SetCtrlObj(null);
            mLines.Edit((item) => { if (item.Key == key) item.Value.Visible = false; });
        }

        /// <summary>
        /// 隱藏可控制點
        /// </summary>
        public void HideCtrlMark(int key)
        {
            mDragManager.SetCtrlObj(null);
            mMarks.Edit((item) => { if (item.Key == key) item.Value.Visible = false; });
        }

        /// <summary>
        /// 建立新地圖(刪除所有資料)
        /// </summary>
        public void NewMap(IEnumerable<IPoint> obstaclePoints, IEnumerable<ILine> obstacleLines)
        {
            NewMap();
            if (obstaclePoints != null) AddPointsSet(Factory.CreatSet.ObstaclePoints( obstaclePoints));
            if (obstacleLines != null) AddLinesSet(Factory.CreatSet.ObstacleLines(obstacleLines));
        }

        /// <summary>
        /// 建立新地圖(刪除所有資料)
        /// </summary>
        public void NewMap()
        {
            mAGVs.Clear();
            mMarks.Clear();
            mLines.Clear();
            mAreas.Clear();

            mPointSets.Clear();
            mLineSets.Clear();
            mAreas.Clear();
        }

        /// <summary>
        /// 移除面集合
        /// </summary>
        public void RemoveAreasSet(int id)
        {
            mAreaSet.Remove(id);
        }

        /// <summary>
        /// 移除線集合
        /// </summary>
        public void RemoveLinesSet(int id)
        {
            mLineSets.Remove(id);
        }

        /// <summary>
        /// 移除點集合
        /// </summary>
        public void RemovePointsSet(int id)
        {
            mPointSets.Remove(id);
        }

        /// <summary>
        /// 獲得受控對象的名稱
        /// </summary>
        internal string CurrentCtrlObjName()
        {
            return mDragManager.CurrentCtrlObjName;
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
            if (mDragManager.CtrlObject != null)
            {
                if (mDragManager.CtrlObject is CtrlMark)
                {
                    CtrlMark mark = (CtrlMark)mDragManager.CtrlObject;
                    MarkDragedEvent?.Invoke(mark.Property.Type, mark.ID, mark.Property.Center, mark.Property.Toward);
                }
                else if (mDragManager.CtrlObject is CtrlLine)
                {
                    CtrlLine line = (CtrlLine)mDragManager.CtrlObject;
                    LineDragedEvent?.Invoke(line.Property.Type, line.ID, line.Property.Line);
                }
                else if (mDragManager.CtrlObject is CtrlArea)
                {
                    CtrlArea area = (CtrlArea)mDragManager.CtrlObject;
                    AreaDragedEvent?.Invoke(area.Property.Type, area.ID, area.Property.Area);
                }
            }
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
        /// 獲得控制項的簡易資訊
        /// </summary>
        internal List<SimpleInfo> GetCtrlSimpleInfo<TValue>(CtrlGM<int, TValue> gm) where TValue : ICtrlable
        {
            List<SimpleInfo> res = new List<SimpleInfo>();
            gm.Edit((item) =>
            {
                SimpleInfo info = new SimpleInfo(item.Value.ID, item.Value.Name, item.Value.Center);
                res.Add(info);
            });
            return res;
        }

        /// <summary>
        /// 滑鼠按下位置
        /// </summary>
        internal void MouseDown(IPair realPos)
        {
            MouseDownEvent?.Invoke(realPos);
        }

        /// <summary>
        /// 滑鼠按下移動位置
        /// </summary>
        internal void MouseDownMove(IPair realPos)
        {
            MouseDownMoveEvent?.Invoke(realPos);
        }

        /// <summary>
        /// 滑鼠放開位置
        /// </summary>
        internal void MouseUp(IPair realPos)
        {
            MouseUpEvent?.Invoke(realPos);
        }

        /// <summary>
        /// 加入可控元件
        /// </summary>
        private void AddCtrl<TValue>(CtrlGM<int, TValue> gm, int key, TValue obj) where TValue : ICtrlable
        {
            if (obj == null) return;
            mDragManager.SetCtrlObj(null);
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