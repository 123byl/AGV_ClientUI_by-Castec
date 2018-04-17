using System;

namespace AGVMap
{
    /// <summary>
    /// 定義面
    /// </summary>
    public interface IArea : ICCWVertex
    {
        /// <summary>
        /// 最大值數對
        /// </summary>
        IPair Max { get; }

        /// <summary>
        /// 最小值數對
        /// </summary>
        IPair Min { get; }
    }

    /// <summary>
    /// 基本幾何-面。
    /// 確保 Min 保持為左下角的座標，且 Max 保持為右上角座標
    /// </summary>
    public class Area : IArea
    {
        /// <summary>
        /// 預設建構子 Max(0,0) Min(0,0)
        /// </summary>
        public Area()
        {
        }

        /// <summary>
        /// 面建構子
        /// </summary>
        public Area(int x0, int y0, int x1, int y1)
        {
            Set(x0, y0, x1, y1);
        }

        /// <summary>
        /// 面建構子
        /// </summary>
        public Area(IPair p0, IPair p1)
        {
            Set(p0.X, p0.Y, p1.X, p1.Y);
        }

        /// <summary>
        /// 由介面建構新面
        /// </summary>
        public Area(IArea area)
        {
            Set(area.Min.X, area.Min.Y, area.Max.X, area.Max.Y);
        }

        /// <summary>
        /// 最大值數對
        /// </summary>
        public IPair Max { get; private set; } = new Pair();

        /// <summary>
        /// 最小值數對
        /// </summary>
        public IPair Min { get; private set; } = new Pair();

        /// <summary>
        /// 獲得逆時針排序的幾何頂點陣列
        /// </summary>
        public IPair[] VertexArray { get { return new IPair[] { Min, new Pair(Max.X, Min.Y), Max, new Pair(Min.X, Max.Y) }; } }

        /// <summary>
        /// 重設座標，並自動依照座標大小分配給 Min/Max
        /// </summary>
        public void Set(int x0, int y0, int x1, int y1)
        {
            Min.X = Math.Min(x0, x1);
            Min.Y = Math.Min(y0, y1);
            Max.X = Math.Max(x0, x1);
            Max.Y = Math.Max(y0, y1);
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