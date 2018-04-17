using SharpGL;

namespace AGVMap
{
    /// <summary>
    /// 座標轉換，實際座標轉 2D 文字座標
    /// </summary>
    public delegate IPair DelGL2Text2D(IPair gl);

    /// <summary>
    /// 具有繪圖功能
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// 繪圖
        /// </summary>
        void Draw(OpenGL gl);
    }
}
