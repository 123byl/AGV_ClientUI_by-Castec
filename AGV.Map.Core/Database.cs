using AGV.Map.Common;
using System;
using System.IO;

namespace AGV.Map.Core
{
    /// <summary>
    /// 共用資料
    /// </summary>
    public static class Database
    {
        #region Data

        public static ISaftyDictionary<IAGV> AGVGM { get; } = new SaftyDictionary<IAGV>();
        public static ISaftyDictionary<IForbiddenArea> ForbiddenAreaGM { get; } = new SaftyDictionary<IForbiddenArea>();
        public static ISaftyDictionary<IForbiddenLine> ForbiddenLineGM { get; } = new SaftyDictionary<IForbiddenLine>();
        public static ISaftyDictionary<IGoal> GoalGM { get; } = new SaftyDictionary<IGoal>();
        public static ISaftyDictionary<ILaserPoints> LaserPointsGM { get; } = new SaftyDictionary<ILaserPoints>();
        public static ISaftyDictionary<INarrowLine> NarrowLineGM { get; } = new SaftyDictionary<INarrowLine>();
        public static ISaftyDictionary<IObstacleLines> ObstacleLinesGM { get; } = new SaftyDictionary<IObstacleLines>();
        public static ISaftyDictionary<IObstaclePoints> ObstaclePointsGM { get; } = new SaftyDictionary<IObstaclePoints>();
        public static ISaftyDictionary<IParking> ParkingGM { get; } = new SaftyDictionary<IParking>();
        public static ISaftyDictionary<IPower> PowerGM { get; } = new SaftyDictionary<IPower>();

        #endregion Data

        public static char SplitChar = ',';

        private delegate void DelDecode(string data, uint id);

        /// <summary>
        /// 刪除所有資料
        /// </summary>
        public static void ClearAll()
        {
            AGVGM.Clear();
            ClearAllButAGV();
        }

        /// <summary>
        /// 刪除除了 AGV 以外的所有資料
        /// </summary>
        public static void ClearAllButAGV()
        {
            ForbiddenAreaGM.Clear();
            ForbiddenLineGM.Clear();
            GoalGM.Clear();
            LaserPointsGM.Clear();
            NarrowLineGM.Clear();
            ObstacleLinesGM.Clear();
            ObstaclePointsGM.Clear();
            ParkingGM.Clear();
            PowerGM.Clear();
        }

        /// <summary>
        /// 載入地圖到資料庫中
        /// </summary>
        public static void LoadMapToDatabase(string fileName)
        {
            try
            {
                DelDecode decode = null;
                uint id = 0;
                string[] lines = File.ReadAllLines(fileName);
                ClearAllButAGV();
                foreach (var line in lines)
                {
                    switch (line)
                    {
                        case "GoalList":
                            decode = DecodeGoalList;
                            break;

                        case "Obstacle Lines":
                            decode = DecodeObstacleLines;
                            id = Factory.FID.GetID();
                            ObstacleLinesGM.Add(id, Factory.FMuti.ObstacleLines());
                            break;

                        case "Obstacle Points":
                            decode = DecodeObstaclePoints;
                            id = Factory.FID.GetID();
                            ObstaclePointsGM.Add(id, Factory.FMuti.ObstaclePoints());
                            break;

                        default:
                            try
                            {
                                decode?.Invoke(line, id);
                            }
                            catch (Exception)
                            {
                            }
                            break;
                    }
                }
            }
            catch (System.Exception)
            {
            }
        }

        private static void DecodeGoalList(string data, uint id)
        {
            string[] elm = data.Split(SplitChar);
            Goal goal = new Goal(int.Parse(elm[1]), int.Parse(elm[2]), double.Parse(elm[3]), elm[0]);
            GoalGM.Add(Factory.FID.GetID(), goal);
        }

        private static void DecodeObstacleLines(string data, uint id)
        {
            string[] elm = data.Split(SplitChar);
            Line line = new Line(int.Parse(elm[0]), int.Parse(elm[1]), int.Parse(elm[2]), int.Parse(elm[3]));
            ObstacleLinesGM.SaftyEdit(id, (item) => item.DataList.Add(line));
        }

        private static void DecodeObstaclePoints(string data, uint id)
        {
            string[] elm = data.Split(SplitChar);
            Pair point = new Pair(int.Parse(elm[0]), int.Parse(elm[1]));
            ObstaclePointsGM.SaftyEdit(id, (item) => item.DataList.Add(point));
        }
    }
}