using AGVMap;
using MapProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUI
{
    public static class Convert
    {
        /// <summary>
        /// 轉型
        /// </summary>
        internal static List<Pair> ToPairs(this List<CartesianPos> points)
        {
            return points.ConvertAll(p => new Pair((int)p.x, (int)p.y));
        }
        /// <summary>
        /// 轉型
        /// </summary>
        internal static List<Pair> ToPoint(this List<CartesianPos> points)
        {
            return points.ConvertAll(p => new Pair((int)p.x, (int)p.y));
        }
        /// <summary>
        /// 轉型
        /// </summary>
        internal static List<TowardPos> ToTowardPos(this List<CartesianPos> points)
        {
            return points.ConvertAll(p => new TowardPos((int)p.x, (int)p.y,p.theta));
        }
        /// <summary>
        /// 轉型
        /// </summary>
        public static List<CartesianPos> ToCartesianPos(this List<TowardPos> points)
        {
            return points.ConvertAll(p => new CartesianPos(p.X, p.Y, p.Toward));
        }
        /// <summary>
        /// 轉型
        /// </summary>
        public static AGVMap.Pair ToPoint(this CartesianPos p)
        {
            return new AGVMap.Pair((int)p.x, (int)p.y);
        }
    }
}
