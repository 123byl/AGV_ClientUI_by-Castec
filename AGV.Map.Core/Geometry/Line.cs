using AGV.Map.Common;
using System;

namespace AGV.Map.Core
{
    /// <summary>
    /// 線段
    /// </summary>
    [Serializable]
    internal class Line : ILine
    {
        private Pair mBegin = new Pair();
        private Pair mEnd = new Pair();

        public Line(int x0, int y0, int x1, int y1)
        {
            mBegin.X = x0;
            mBegin.Y = y0;
            mEnd.X = x1;
            mEnd.Y = y1;
        }

        public Line(double x0, double y0, double x1, double y1) : this((int)x0, (int)y0, (int)x1, (int)y1)
        {
        }

        public Line(ILine line) : this(line.Begin.X, line.Begin.Y, line.End.X, line.End.Y)
        {
        }

        public Line(IPair beg, IPair end) : this(beg.X, beg.Y, end.X, end.Y)
        {
        }

        public Line()
        {
        }

        /// <summary>
        /// 起點座標
        /// </summary>
        public IPair Begin { get { return mBegin; } set { mBegin.X = value.X; mBegin.Y = value.Y; } }

        /// <summary>
        /// 終點座標
        /// </summary>
        public IPair End { get { return mEnd; } set { mEnd.X = value.X; mEnd.Y = value.Y; } }
    }
}