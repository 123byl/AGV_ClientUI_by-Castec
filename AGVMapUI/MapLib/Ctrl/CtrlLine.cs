using SharpGL;

namespace AGVMap
{
    /// <summary>
    /// 可控的標示線
    /// </summary>
    public class CtrlLine : ICtrlable
    {
        /// <summary>
        /// 建構子
        /// </summary>
        public CtrlLine(int x0, int y0, int x1, int y1)
        {
            Property.Line = new Line(x0, y0, x1, y1);
        }

        /// <summary>
        /// 建構子
        /// </summary>
        public CtrlLine(IPair p0, IPair p1)
        {
            Property.Line = new Line(p0, p1);
        }

        /// <summary>
        /// 中心座標
        /// </summary>
        public IPair Center { get { return (new Pair(Property.Line.Begin) + new Pair(Property.Line.End)) / 2; } }

        /// <summary>
        /// 識別碼
        /// </summary>
        public int ID { get; internal set; }

        /// <summary>
        /// 物件名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 屬性
        /// </summary>
        public CtrlLineProperty Property { get; } = new CtrlLineProperty();

        /// <summary>
        /// 顯示/隱藏
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// 繪圖
        /// </summary>
        public void Draw(OpenGL gl)
        {
            if (!Visible) return;
            gl.PushText(Center, Name);
            gl.PushMatrix();
            {
                gl.Color(Property.Color.ToArray);
                gl.LineWidth(Property.LineWidth);
                gl.Begin(OpenGL.GL_LINES);
                {
                    gl.Vertex(Property.Line.Begin.X, Property.Line.Begin.Y, (int)Property.Layer);
                    gl.Vertex(Property.Line.End.X, Property.Line.End.Y, (int)Property.Layer);
                }
                gl.End();
            }
            gl.PopMatrix();
        }

        /// <summary>
        /// 獲得滑鼠拖曳點陣列
        /// </summary>
        public DragPoint[] GetMouseDragPoint()
        {
            return new DragPoint[] { new DragPoint(((Pair)Property.Line.Begin + (Pair)Property.Line.End) / 2, MoveCenter),
            new DragPoint(Property.Line.Begin,MoveBegin),new DragPoint(Property.Line.End,MoveEnd)};
        }

        /// <summary>
        /// 現實座標 p 是否在圖形上
        /// </summary>
        public bool Intersect(IPair p)
        {
            double length = (new Pair(Property.Line.End) - new Pair(Property.Line.Begin)).Length();
            double length0 = (new Pair(p) - new Pair(Property.Line.Begin)).Length();
            double length1 = (new Pair(p) - new Pair(Property.Line.End)).Length();
            if (length0 <= 100 || length1 <= 100 || length0 + length1 < 1.01 * length) return true;
            return false;
        }

        /// <summary>
        /// 起點座標改變時所對應的事件
        /// </summary>
        private void MoveBegin(IPair newBegin)
        {
            Property.Line.Begin = new Pair(newBegin);
        }

        /// <summary>
        /// 中心座標改變時所對應的事件
        /// </summary>
        private void MoveCenter(IPair newCenter)
        {
            Pair center = ((Pair)Property.Line.Begin + (Pair)Property.Line.End) / 2;
            Pair delta = new Pair(newCenter) - center;
            Property.Line.Begin = (Pair)Property.Line.Begin + delta;
            Property.Line.End = (Pair)Property.Line.End + delta;
        }

        /// <summary>
        /// 終點座標改變時所對應的事件
        /// </summary>
        private void MoveEnd(IPair newEnd)
        {
            Property.Line.End = new Pair(newEnd);
        }
    }
}