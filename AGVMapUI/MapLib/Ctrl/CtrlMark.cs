using SharpGL;
using System;

namespace AGVMap
{
    /// <summary>
    /// 可控的標示點
    /// </summary>
    public class CtrlMark : ICtrlable
    {
        /// <summary>
        /// 建構子
        /// </summary>
        public CtrlMark(IPair center, Angle tower)
        {
            Property.Center = new Pair(center);
            Property.Toward = tower;
        }

        /// <summary>
        /// 建構子
        /// </summary>
        public CtrlMark(int x, int y, Angle tower)
        {
            Property.Center = new Pair(x, y);
            Property.Toward = tower;
        }

        /// <summary>
        /// 中心座標
        /// </summary>
        public IPair Center { get { return Property.Center; } }

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
        public CtrlMarkProperty Property { get; } = new CtrlMarkProperty();

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
                gl.Translate(Center.X, Center.Y, 0);
                gl.Begin(OpenGL.GL_POLYGON);
                {
                    gl.Vertex(+Property.Size / 2, -Property.Size / 2, (int)Property.Layer);
                    gl.Vertex(+Property.Size / 2, +Property.Size / 2, (int)Property.Layer);
                    gl.Vertex(-Property.Size / 2, +Property.Size / 2, (int)Property.Layer);
                    gl.Vertex(-Property.Size / 2, -Property.Size / 2, (int)Property.Layer);
                }
                gl.End();
                gl.LineWidth(2.0f);
                gl.Color(new Color(Property.Color).InvertColor());
                gl.Begin(OpenGL.GL_LINES);
                {
                    IPair toward = new Pair(GetTowardControllPoint()) - new Pair(Property.Center);
                    gl.Vertex(0, 0, (int)Property.Layer + 0.1f);
                    gl.Vertex(toward.X, toward.Y, (int)Property.Layer + 0.1f);
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
            return new DragPoint[] { new DragPoint(Property.Center, MoveCenter, Property.Size),
                new DragPoint(GetTowardControllPoint(), MoveToward) };
        }

        /// <summary>
        /// 現實座標 p 是否在圖形上
        /// </summary>
        public bool Intersect(IPair p)
        {
            if (Math.Abs(p.X - Property.Center.X) > Property.Size / 2) return false;
            if (Math.Abs(p.Y - Property.Center.Y) > Property.Size / 2) return false;
            return true;
        }

        /// <summary>
        /// 獲得方向角控制點座標
        /// </summary>
        private IPair GetTowardControllPoint()
        {
            return new Pair(Property.Center) + new Pair((int)(Property.TowardLength * Math.Cos(Property.Toward * Math.PI / 180)), (int)(Property.TowardLength * Math.Sin(Property.Toward * Math.PI / 180)));
        }

        /// <summary>
        /// 中心座標改變時所對應的事件
        /// </summary>
        private void MoveCenter(IPair newCenter)
        {
            Property.Center = new Pair(newCenter.X, newCenter.Y);
        }

        /// <summary>
        /// 方向角改變時所對應的事件
        /// </summary>
        private void MoveToward(IPair newToward)
        {
            Pair direction = new Pair(newToward) - new Pair(Property.Center);
            Property.Toward = Math.Atan2(direction.Y, direction.X) * 180 / Math.PI;
        }
    }
}
