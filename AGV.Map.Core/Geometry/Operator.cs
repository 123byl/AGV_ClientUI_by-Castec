using AGV.Map.Common;
using System;

namespace AGV.Map.Core
{
    public static class Operator
    {
        /// <summary>
        /// 與 X 軸夾角
        /// </summary>
        public static IAngle Angle(this IPair pos)
        {
            return new Angle(Math.Atan2(pos.Y, pos.X) * 180 / Math.PI);
        }

        /// <summary>
        /// 座標取絕對值
        /// </summary>
        public static IPair Abs(this IPair value)
        {
            return new Pair(Math.Abs(value.X), Math.Abs(value.Y));
        }

        /// <summary>
        /// 加法
        /// </summary>
        public static IPair Add(this IPair lhs, IPair rhs)
        {
            return new Pair(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

        /// <summary>
        /// 回傳線段的中心座標
        /// </summary>
        public static IPair Center(this ILine line)
        {
            return new Pair(line.Begin.Add(line.End).Division(2));
        }

        /// <summary>
        /// 回傳面的中心座標
        /// </summary>
        public static IPair Center(this IArea area)
        {
            return new Pair(area.Min.Add(area.Max).Division(2));
        }

        /// <summary>
        /// 將面轉換為四條線
        /// </summary>
        public static ILine[] ConvertToLines(this IArea area)
        {
            IPair p0 = area.Min;
            IPair p2 = area.Max;
            IPair p1 = new Pair(p2.X, p0.Y);
            IPair p3 = new Pair(p0.X, p2.Y);
            return new ILine[] { new Line(p0, p1), new Line(p1, p2), new Line(p2, p3), new Line(p3, p0) };
        }

        /// <summary>
        /// 除法
        /// </summary>
        public static IPair Division(this IPair lhs, int rhs)
        {
            return new Pair(lhs.X / rhs, lhs.Y / rhs);
        }

        /// <summary>
        /// 是否相交
        /// </summary>
        public static bool Interference(this ISingle<ITowardPair> single, IPair pos)
        {
            IPair distance = single.Data.Position.Subtraction(pos).Abs();
            return distance.X <= single.GLSetting.Size.X / 2 && distance.Y <= single.GLSetting.Size.Y / 2;
        }

        /// <summary>
        /// 是否相交
        /// </summary>
        public static bool Interference(this ISingle<IArea> single, IPair pos)
        {
            if (pos.X > single.Data.Max.X) return false;
            if (pos.Y > single.Data.Max.Y) return false;
            if (pos.X < single.Data.Min.X) return false;
            if (pos.Y < single.Data.Min.Y) return false;
            return true;
        }

        /// <summary>
        /// 是否相交
        /// </summary>
        public static bool Interference(this ISingle<ILine> single, IPair pos)
        {
            double length = single.Data.End.LengthTo(single.Data.Begin);
            double length0 = pos.LengthTo(single.Data.Begin);
            double length1 = pos.LengthTo(single.Data.End);
            if (length0 <= 100 || length1 <= 100 || length0 + length1 < 1.01 * length) return true;
            return false;
        }

        /// <summary>
        /// 求線段長度
        /// </summary>
        public static double Length(this ILine line)
        {
            int x = line.End.X - line.Begin.X;
            int y = line.End.Y - line.Begin.Y;
            return Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// 兩點間距離
        /// </summary>
        public static double LengthTo(this IPair beg, IPair end)
        {
            return Math.Sqrt((beg.X - end.X) * (beg.X - end.X) + (beg.Y - end.Y) * (beg.Y - end.Y));
        }

        /// <summary>
        /// 回傳右下角座標
        /// </summary>
        public static IPair MaxXMinY(this IArea area)
        {
            return new Pair(area.Max.X, area.Min.Y);
        }

        /// <summary>
        /// 回傳左上角座標
        /// </summary>
        public static IPair MinXMaxY(this IArea area)
        {
            return new Pair(area.Min.X, area.Max.Y);
        }

        /// <summary>
        /// 求移動後座標
        /// </summary>
        public static IPair Shift(this IPair center, IAngle toward, int length)
        {
            return center.Shift(toward.Theta, length);
        }

        /// <summary>
        /// 求移動後座標
        /// </summary>
        public static IPair Shift(this IPair center, double toward, int length)
        {
            return new Pair((int)(center.X + length * Math.Cos(toward * Math.PI / 180)), (int)(center.Y + length * Math.Sin(toward * Math.PI / 180)));
        }

        /// <summary>
        /// 回傳面的長寬尺寸
        /// </summary>
        public static IPair Size(this IArea area)
        {
            return new Pair(area.Max.X - area.Min.X, area.Max.Y - area.Min.Y);
        }

        /// <summary>
        /// 減法
        /// </summary>
        public static IPair Subtraction(this IPair lhs, IPair rhs)
        {
            return new Pair(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }
    }
}