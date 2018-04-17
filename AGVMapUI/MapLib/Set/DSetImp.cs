using SharpGL;

namespace AGVMap
{
    /// <summary>
    /// 提供執行緒安全的可繪面集合
    /// </summary>
    public class DASet : DSet<IArea>
    {
        /// <summary>
        /// 重繪
        /// </summary>
        protected override void ReDraw(OpenGL gl)
        {
            gl.Begin(OpenGL.GL_POLYGON);
            {
                ForEach((area) =>
                {
                    gl.Vertex(area.Min.X, area.Min.Y);
                    gl.Vertex(area.Max.X, area.Min.Y);
                    gl.Vertex(area.Max.X, area.Max.Y);
                    gl.Vertex(area.Min.X, area.Max.Y);
                });
            }
            gl.End();
        }
    }

    /// <summary>
    /// 提供執行緒安全的可繪線集合
    /// </summary>
    public class DLSet : DSet<ILine>
    {
        /// <summary>
        /// 線段粗細
        /// </summary>
        public float LineWidth { get; set; } = 1.0f;

        /// <summary>
        /// 重繪
        /// </summary>
        protected override void ReDraw(OpenGL gl)
        {
            gl.LineWidth(LineWidth);
            gl.Begin(OpenGL.GL_LINES);
            {
                ForEach((line) =>
                {
                    gl.Vertex(line.Begin.X, line.Begin.Y);
                    gl.Vertex(line.End.X, line.End.Y);
                });
            }
            gl.End();
        }
    }

    /// <summary>
    /// 提供執行緒安全的可繪點集合
    /// </summary>
    public class DPSet : DSet<IPair>
    {
        /// <summary>
        /// 點大小
        /// </summary>
        public float PointSize { get; set; } = 2.0f;

        /// <summary>
        /// 重繪
        /// </summary>
        protected override void ReDraw(OpenGL gl)
        {
            gl.PointSize(PointSize);
            gl.Begin(OpenGL.GL_POINTS);
            {
                ForEach((point) =>
                {
                    gl.Vertex(point.X, point.Y);
                });
            }
            gl.End();
        }
    }
}