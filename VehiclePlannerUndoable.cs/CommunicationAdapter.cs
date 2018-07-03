using Geometry;
using GLCore;
using Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehiclePlanner.Core;

namespace VehiclePlannerUndoable.cs
{

    public enum EmID
    {
        ITS = 0,
        Laser = 1

    }

    public class RequestLaser : BaseRequeLaser
    {
        

        private List<IPair> mLaser = null;

        public RequestLaser(Serializable response)
        {
            if (Requited = mLaser != null)
            {
                Count = mLaser.Count();
            }
        }

        public override void DrawLaser()
        {
            GLCMD.CMD.SaftyEditMultiGeometry<IPair>((int)EmID.Laser, true, laser =>
            {
                laser.Clear();
                laser.AddRange(mLaser);
            });
        }
    }
}
