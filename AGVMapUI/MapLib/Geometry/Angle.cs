namespace AGVMap
{
    /// <summary>
    /// 具有角度方向的
    /// </summary>
    public interface IToward
    {
        /// <summary>
        /// 首向
        /// </summary>
        Angle Toward{ get; set; }
    }

    /// <summary>
    /// 提供一個介於 [0,360) 之間的角度，
    /// 資料以 double 格式儲存
    /// </summary>
    public class Angle
    {
        /// <summary>
        /// 提供一個介於 [0,360) 之間的角度，
        /// 資料以 double 格式儲存
        /// </summary>
        public Angle()
        {
        }

        /// <summary>
        /// 提供一個介於 [0,360) 之間的角度，
        /// 資料以 double 格式儲存
        /// </summary>
        public Angle(double ang)
        {
            Value = ang;
        }

        /// <summary>
        /// 介於 [0,360) 之間的角度
        /// </summary>
        public double Value { get { return mData; } set { mData = Normalization(value); } }

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

        private double mData = 0.0;

        #region - 轉型 -

        public static implicit operator Angle(double ang)
        {
            return new Angle(ang);
        }

        public static implicit operator double(Angle ang)
        {
            return ang.Value;
        }

        #endregion - 轉型 -
    }
}