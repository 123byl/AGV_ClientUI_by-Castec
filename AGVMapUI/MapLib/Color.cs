namespace AGVMap
{
    /// <summary>
    /// 顏色定義
    /// </summary>
    public interface IColor
    {
        #region - 資料 -

        /// <summary>
        /// Alpha = 255 表示不透明
        /// </summary>
        byte A { get; set; }

        /// <summary>
        /// 藍
        /// </summary>
        byte B { get; set; }

        /// <summary>
        /// 綠
        /// </summary>
        byte G { get; set; }

        /// <summary>
        /// 紅
        /// </summary>
        byte R { get; set; }

        #endregion - 資料 -

        /// <summary>
        /// 依照 R G B A 回傳浮點數陣列，各項值介於 [0.0f,1.0f]
        /// </summary>
        float[] ToArray { get; }
    }

    /// <summary>
    /// 顏色
    /// </summary>
    public class Color : IColor
    {
        /// <summary>
        /// 由介面建構新的顏色
        /// </summary>
        public Color(IColor color) : this(color.R, color.G, color.B, color.A)
        {
        }

        /// <summary>
        /// 預設不透明黑色
        /// </summary>
        public Color() : this(0, 0, 0, 255)
        {
        }

        /// <summary>
        /// 由 .Net Color 建構新的顏色
        /// </summary>
        public Color(System.Drawing.Color color) : this(color.R, color.G, color.B, color.A)
        {
        }

        /// <summary>
        /// 由 .Net Color 建構新的顏色，Alpha = 255 表示不透明
        /// </summary>
        public Color(System.Drawing.Color color, byte a) : this(color.R, color.G, color.B, a)
        {
        }

        /// <summary>
        /// 自訂顏色，Alpha = 255 表示不透明
        /// </summary>
        public Color(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        #region - 資料 -

        /// <summary>
        /// Alpha = 255 表示不透明
        /// </summary>
        public byte A { get; set; } = 255;

        /// <summary>
        /// 藍
        /// </summary>
        public byte B { get; set; }

        /// <summary>
        /// 綠
        /// </summary>
        public byte G { get; set; }

        /// <summary>
        /// 紅
        /// </summary>
        public byte R { get; set; }

        #endregion - 資料 -

        /// <summary>
        /// 依照 R G B A 回傳浮點數陣列，各項值介於 [0.0f,1.0f]
        /// </summary>
        public float[] ToArray { get { return new float[] { R / 255.0f, G / 255.0f, B / 255.0f, A / 255.0f }; } }

        /// <summary>
        /// 色彩反轉
        /// </summary>
        public Color InvertColor()
        {
            return new Color((byte)(255 - R), (byte)(255 - G), (byte)(255 - B), A);
        }

        #region - 轉型 -

        public static implicit operator System.Drawing.Color(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static implicit operator Color(System.Drawing.Color color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }

        public static implicit operator float[] (Color color)
        {
            return color.ToArray;
        }

        #endregion - 轉型 -
    }
}