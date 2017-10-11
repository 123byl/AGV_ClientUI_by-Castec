using AGV.Map.Common;
using System;

namespace AGV.Map.Core
{
    /// <summary>
    /// 提供介於[0,360)之間的角度
    /// </summary>
    [Serializable]
    internal class Angle : IAngle
    {
        private double mValue = 0;

        public Angle()
        {
        }

        public Angle(IAngle angle)
        {
            Theta = angle.Theta;
        }

        public Angle(double angle)
        {
            Theta = angle;
        }

        /// <summary>
        /// 角度值
        /// </summary>
        public double Theta { get { return mValue; } set { mValue = Normalization(value); } }

        /// <summary>
        /// 將角度正規劃在 [0,360) 區間
        /// </summary>
        public static double Normalization(double ang)
        {
            double thetaTmp = ang % 360;
            if (thetaTmp < 0)
                ang = thetaTmp + 360;
            else
                ang = thetaTmp;
            return ang;
        }
    }
}