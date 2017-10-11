using AGV.Map.Common;
using System;

namespace AGV.Map.Core
{
    /// <summary>
    /// 提供 Byte 格式的的顏色操作介面
    /// </summary>
    [Serializable]
    internal class Color : IColor
    {
        public Color(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color()
        {
        }

        public Color(IColor color) : this(color.R, color.G, color.B, color.A)
        {
        }

        public Color(System.Drawing.Color color) : this(color.R, color.G, color.B, color.A)
        {
        }

        public Color(System.Drawing.Color color, byte a) : this(color.R, color.G, color.B, a)
        {
        }

        /// <summary>
        /// 透明度(A=255表示不透明)
        /// </summary>
        public byte A { get; set; } = 255;

        /// <summary>
        /// 藍
        /// </summary>
        public byte B { get; set; } = 0;

        /// <summary>
        /// 綠
        /// </summary>
        public byte G { get; set; } = 0;

        /// <summary>
        /// 紅
        /// </summary>
        public byte R { get; set; } = 0;

        /// <summary>
        /// 獲得介於[0,1]之間的浮點數陣列
        /// </summary>
        public float[] GetFloats() { return new float[] { R / 255f, G / 255f, B / 255f, A / 255f }; }
    }
}