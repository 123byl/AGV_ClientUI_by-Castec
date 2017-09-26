using SharpGL;

namespace AGVMap
{
    /// <summary>
    /// 可控的標示面
    /// </summary>
    public class CtrlArea : ICtrlable
    {
        /// <summary>
        /// 屬性
        /// </summary>
        public CtrlAreaProperty Property = new CtrlAreaProperty();

        /// <summary>
        /// 建構子
        /// </summary>
        public CtrlArea(int x0, int y0, int x1, int y1)
        {
            Property.Area = new Area(x0, x1, y0, y1);
        }

        /// <summary>
        /// 建構子
        /// </summary>
        public CtrlArea(IPair p0, IPair p1)
        {
            Property.Area = new Area(p0, p1);
        }
        /// <summary>
        /// 顯示/隱藏
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// 物件名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 繪圖
        /// </summary>
        public void Draw(OpenGL gl)
        {
            if (!Visible) return;
            gl.PushMatrix();
            {
                gl.Color(Property.Color.ToArray);
                gl.Begin(OpenGL.GL_POLYGON);
                {
                    gl.Vertex(Property.Area.Min.X, Property.Area.Min.Y, (int)Property.Layer);
                    gl.Vertex(Property.Area.Max.X, Property.Area.Min.Y, (int)Property.Layer);
                    gl.Vertex(Property.Area.Max.X, Property.Area.Max.Y, (int)Property.Layer);
                    gl.Vertex(Property.Area.Min.X, Property.Area.Max.Y, (int)Property.Layer);
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
            return new DragPoint[] { new DragPoint(((Pair)Property.Area.Max + (Pair)Property.Area.Min) / 2,MoveCenter, ((Pair)Property.Area.Max - (Pair)Property.Area.Min)),
                new DragPoint(new Pair(Property.Area.Max.X,(Property.Area.Min.Y+Property.Area.Max.Y)/2),MoveMaxX),
                new DragPoint(new Pair(Property.Area.Min.X,(Property.Area.Min.Y+Property.Area.Max.Y)/2),MoveMinX),
                new DragPoint(new Pair((Property.Area.Min.X+Property.Area.Max.X)/2,Property.Area.Max.Y),MoveMaxY),
                new DragPoint(new Pair((Property.Area.Min.X+Property.Area.Max.X)/2,Property.Area.Min.Y),MoveMinY),
                new DragPoint(new Pair(Property.Area.Max),MoveMaxXMaxY),
                new DragPoint(new Pair(Property.Area.Min),MoveMinXMinY),
                new DragPoint(new Pair(Property.Area.Max.X,Property.Area.Min.Y),MoveMaxXMinY),
                new DragPoint(new Pair(Property.Area.Min.X,Property.Area.Max.Y),MoveMinXMaxY),
            };
        }

        /// <summary>
        /// 現實座標 p 是否在圖形上
        /// </summary>
        public bool Intersect(IPair p)
        {
            if (p.X > Property.Area.Max.X) return false;
            if (p.Y > Property.Area.Max.Y) return false;
            if (p.X < Property.Area.Min.X) return false;
            if (p.Y < Property.Area.Min.Y) return false;
            return true;
        }

        /// <summary>
        /// 中心座標改變時所對應的事件
        /// </summary>
        private void MoveCenter(IPair newCenter)
        {
            Pair center = ((Pair)Property.Area.Max + (Pair)Property.Area.Min) / 2;
            Pair delta = new Pair(newCenter) - center;
            ((Area)Property.Area).Set((Pair)Property.Area.Max + delta, (Pair)Property.Area.Min + delta);
        }

        /// <summary>
        /// X 最大值改變時所對應的事件
        /// </summary>
        private void MoveMaxX(IPair p)
        {
            ((Area)Property.Area).Set(Property.Area.Min.X, Property.Area.Min.Y, p.X, Property.Area.Max.Y);
        }

        /// <summary>
        /// MaxX MaxY 同時改變時所對應的事件
        /// </summary>
        private void MoveMaxXMaxY(IPair p)
        {
            ((Area)Property.Area).Set(Property.Area.Min.X, Property.Area.Min.Y, p.X, p.Y);
        }

        /// <summary>
        /// MaxX MinY 同時改變時所對應的事件
        /// </summary>
        private void MoveMaxXMinY(IPair p)
        {
            ((Area)Property.Area).Set(Property.Area.Min.X, p.Y, p.X, Property.Area.Max.Y);
        }

        /// <summary>
        /// Y 最大值改變時所對應的事件
        /// </summary>
        private void MoveMaxY(IPair p)
        {
            ((Area)Property.Area).Set(Property.Area.Min.X, Property.Area.Min.Y, Property.Area.Max.X, p.Y);
        }

        /// <summary>
        /// X 最小值改變時所對應的事件
        /// </summary>
        private void MoveMinX(IPair p)
        {
            ((Area)Property.Area).Set(p.X, Property.Area.Min.Y, Property.Area.Max.X, Property.Area.Max.Y);
        }

        /// <summary>
        /// MinX MaxY 同時改變時所對應的事件
        /// </summary>
        private void MoveMinXMaxY(IPair p)
        {
            ((Area)Property.Area).Set(p.X, Property.Area.Min.Y, Property.Area.Max.X, p.Y);
        }

        /// <summary>
        /// MinX MinY 同時改變時所對應的事件
        /// </summary>
        private void MoveMinXMinY(IPair p)
        {
            ((Area)Property.Area).Set(p.X, p.Y, Property.Area.Max.X, Property.Area.Max.Y);
        }

        /// <summary>
        /// Y 最小值改變時所對應的事件
        /// </summary>
        private void MoveMinY(IPair p)
        {
            ((Area)Property.Area).Set(Property.Area.Min.X, p.Y, Property.Area.Max.X, Property.Area.Max.Y);
        }
    }
}
