using AGV.Map.Common;
using System;

namespace AGV.Map.Core
{
    /// <summary>
    /// 座標
    /// </summary>
    [Serializable]
    internal class Pair : IPair
    {
        public Pair(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Pair(IPair pair) : this(pair.X, pair.Y)
        {
        }

        public Pair()
        {
        }

        public Pair(double x, double y) : this((int)x, (int)y)
        {
        }

        /// <summary>
        /// X 座標
        /// </summary>
        public int X { get; set; } = 0;

        /// <summary>
        /// Y 座標
        /// </summary>
        public int Y { get; set; } = 0;
    }
}