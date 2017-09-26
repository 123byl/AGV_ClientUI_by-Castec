using SharpGL;
using System;

namespace AGVMap
{
    /// <summary>
    /// 可控的 AGV 車
    /// </summary>
    public class CtrlAGV : ICtrlable
    {
        /// <summary>
        /// AGV 車建構子
        /// </summary>
        public CtrlAGV(IPair center, Angle toward)
        {
            Property.Center = new Pair(center);
            Property.Toward = toward;
        }
        /// <summary>
        /// 顯示/隱藏
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// AGV 車建構子
        /// </summary>
        public CtrlAGV(int x, int y, Angle toward)
        {
            Property.Center = new Pair(x, y);
            Property.Toward = toward;
        }

        /// <summary>
        /// 物件名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 屬性
        /// </summary>
        public CtrlMarkProperty Property { get; } = new CtrlMarkProperty();

        /// <summary>
        /// 繪圖
        /// </summary>
        public void Draw(OpenGL gl)
        {
            if (!Visible) return;
            gl.PushMatrix();
            {
                gl.Translate(Property.Center.X, Property.Center.Y, 0);
                gl.Rotate(0, 0, (float)Property.Toward);
                gl.Color(Property.Color.ToArray);
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
                    gl.Vertex(0, 0, (int)Property.Layer + 0.1f);
                    gl.Vertex(Property.TowardLength, 0, (int)Property.Layer + 0.1f);
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
            return new DragPoint[] { new DragPoint(Property.Center, MoveCenter, Property.Size) };
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
        /// 中心座標改變時所對應的事件
        /// </summary>
        private void MoveCenter(IPair newCenter)
        {
        }
    }
}