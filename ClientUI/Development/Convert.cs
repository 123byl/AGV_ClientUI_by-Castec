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
        public static List<CartesianPos> ToCartesianPos(this List<TowardPos> points)
        {
            return points.ConvertAll(p => new CartesianPos(p.X, p.Y, p.Toward));
        }
        /// <summary>
        /// 轉型
        /// </summary>
        public static AGVMap.Point ToPoint(this CartesianPos p)
        {
            return new AGVMap.Point((int)p.x, (int)p.y);
        }
    }
}
