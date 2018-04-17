using AGV.Map.Common;
using System;

namespace AGV.Map.Core
{
    /// <summary>
    /// 面，自動調整 Max 與 Min 使得 Max 總是維持在左上角，Min 總是在右下角
    /// </summary>
    internal class Area : IArea
    {
        private Pair mMax = new Pair();

        private Pair mMin = new Pair();

        public Area()
        {
        }

        public Area(IPair min, IPair max)
        {
            Set(min, max);
        }

        public Area(IPair center, int width, int height)
        {
            Set(center.X - width / 2, center.Y - height / 2, center.X + width / 2, center.Y + height / 2);
        }

        public Area(IArea area)
        {
            Set(area.Min, area.Max);
        }

        public Area(double x0, double y0, double x1, double y1)
        {
            Set((int)x0, (int)y0, (int)x1, (int)y1);
        }

        public Area(int x0, int y0, int x1, int y1)
        {
            Set(x0, y0, x1, y1);
        }

        /// <summary>
        /// 最大值座標
        /// </summary>
        public IPair Max { get { return mMax; } set { Set(mMin, value); } }

        /// <summary>
        /// 最小值座標
        /// </summary>
        public IPair Min { get { return mMin; } set { Set(value, mMin); } }

        /// <summary>
        /// 重設座標，並自動依照座標大小分配給 Min/Max
        /// </summary>
        public void Set(int x0, int y0, int x1, int y1)
        {
            mMin.X = Math.Min(x0, x1);
            mMin.Y = Math.Min(y0, y1);
            mMax.X = Math.Max(x0, x1);
            mMax.Y = Math.Max(y0, y1);
        }

        /// <summary>
        /// 重設座標，並自動依照座標大小分配給 Min/Max
        /// </summary>
        public void Set(IPair p0, IPair p1)
        {
            Set(p0.X, p0.Y, p1.X, p1.Y);
        }
    }
}