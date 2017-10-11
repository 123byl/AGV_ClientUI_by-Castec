//using MapProcessing;
//using System.Collections.Generic;

//namespace ClientUI
//{
//    public static class Convert
//    {
//        #region To CartesianPos

//        /// <summary>
//        /// 轉型
//        /// </summary>
//        public static List<MapProcessing.Info> ToCartesianPos(this List<TowardPos> points)
//        {
//            return points.ConvertAll((TowardPos p) => new MapProcessing.Info(p.X, p.Y, p.Toward));
//        }
//        /// <summary>
//        /// 轉型
//        /// </summary>
//        public static List<MapProcessing.Info> ToCartesianPos(this List<Pair> points)
//        {
//            return points.ConvertAll((Pair p) => new MapProcessing.Info(p.X, p.Y, 0));
//        }
//        /// <summary>
//        /// 轉型
//        /// </summary>
//        public static Info ToCartesianPos(this TowardPos points)
//        {
//            return new MapProcessing.Info(points.X, points.Y, points.Toward);
//        }

//        /// <summary>
//        /// 轉型
//        /// </summary>
//        public static Info ToCartesianPos(this Pair points)
//        {
//            return new MapProcessing.Info(points.X, points.Y, 0);
//        }

//        #endregion To CartesianPos

//        #region To Pair

//        /// <summary>
//        /// 轉型
//        /// </summary>
//        public static AGVMap.Pair ToPair(this MapProcessing.Info p)
//        {
//            return new AGVMap.Pair((int)p.x, (int)p.y);
//        }

//        /// <summary>
//        /// 轉型
//        /// </summary>
//        public static AGVMap.Pair ToPair(this TowardPos p)
//        {
//            return new AGVMap.Pair(p.Pos);
//        }

//        /// <summary>
//        /// 轉型
//        /// </summary>
//        internal static List<Pair> ToPair(this List<TowardPos> points)
//        {
//            return points.ConvertAll(p => new Pair(p.Pos));
//        }

//        /// <summary>
//        /// 轉型
//        /// </summary>
//        internal static List<Pair> ToPairs(this List<MapProcessing.Info> points)
//        {
//            return points.ConvertAll(p => new Pair((int)p.x, (int)p.y));
//        }

//        #endregion To Pair

//        #region To TowardPos

//        /// <summary>
//        /// 轉型
//        /// </summary>
//        internal static TowardPos ToTowardPos(this MapProcessing.Info points)
//        {
//            return new TowardPos((int)points.x, (int)points.y, points.theta);
//        }

//        /// <summary>
//        /// 轉型
//        /// </summary>
//        internal static List<TowardPos> ToTowardPos(this List<MapProcessing.Info> points)
//        {
//            return points.ConvertAll(p => new TowardPos((int)p.x, (int)p.y, p.theta));
//        }

//        #endregion To TowardPos
//    }
//}
