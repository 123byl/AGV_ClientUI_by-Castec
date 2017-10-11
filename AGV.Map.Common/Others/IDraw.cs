using SharpGL;

namespace AGV.Map.Common
{
    /// <summary>
    /// 可繪的
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// 繪圖
        /// </summary>
        void Draw(OpenGL gl);
    }
}