using AGV.Map.Common;
using System.Collections.Generic;

namespace AGV.Map.Core
{
    /// <summary>
    /// 障礙點
    /// </summary>
    internal class ObstaclePoints : MutiPair, IObstaclePoints
    {
        public ObstaclePoints() : base()
        {
            GLSetting = new GLSetting(EType.ObstaclePoints);
        }

        public ObstaclePoints(IEnumerable<IPair> collection) : base(collection)
        {
            GLSetting = new GLSetting(EType.ObstaclePoints);
        }

        public ObstaclePoints(IPair item) : base(item)
        {
            GLSetting = new GLSetting(EType.ObstaclePoints);
        }
    }
}