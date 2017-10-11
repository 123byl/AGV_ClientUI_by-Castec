using AGV.Map.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGV.Map.Core
{
    /// <summary>
    /// 共用資料
    /// </summary>
    public static class Database
    {
        public static ISaftyDictionary<IAGV> AGVGM { get; } = new SaftyDictionary<IAGV>();
        public static ISaftyDictionary<IForbiddenArea> ForbiddenAreaGM { get; } = new SaftyDictionary<IForbiddenArea>();
        public static ISaftyDictionary<IForbiddenLine> ForbiddenLineGM { get; } = new SaftyDictionary<IForbiddenLine>();
        public static ISaftyDictionary<IGoal> GoalGM { get; } = new SaftyDictionary<IGoal>();
        public static ISaftyDictionary<ILaserPoints> LaserPointsGM { get; } = new SaftyDictionary<ILaserPoints>();
        public static ISaftyDictionary<INarrowLine> NarrowLinGM { get; } = new SaftyDictionary<INarrowLine>();
        public static ISaftyDictionary<IObstacleLines> ObstacleLinesGM { get; } = new SaftyDictionary<IObstacleLines>();
        public static ISaftyDictionary<IObstaclePoints> ObstaclePointsGM { get; } = new SaftyDictionary<IObstaclePoints>();
        public static ISaftyDictionary<IParking> ParkingGM { get; } = new SaftyDictionary<IParking>();
        public static ISaftyDictionary<IPower> PowerGM { get; } = new SaftyDictionary<IPower>();
    }
}