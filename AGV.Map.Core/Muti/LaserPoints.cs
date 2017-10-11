using AGV.Map.Common;
using System.Collections.Generic;

namespace AGV.Map.Core
{
    /// <summary>
    /// 雷射點
    /// </summary>
    internal class LaserPoints : MutiPair, ILaserPoints
    {
        public LaserPoints() : base()
        {
            GLSetting = new GLSetting(EType.LaserPoints);
        }

        public LaserPoints(IEnumerable<IPair> collection) : base(collection)
        {
            GLSetting = new GLSetting(EType.LaserPoints);
        }

        public LaserPoints(IPair item) : base(item)
        {
            GLSetting = new GLSetting(EType.LaserPoints);
        }
    }
}