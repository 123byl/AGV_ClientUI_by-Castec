using SharpGL;

namespace AGVMap
{
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
