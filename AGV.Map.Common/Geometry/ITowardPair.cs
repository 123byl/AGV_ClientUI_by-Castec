namespace AGV.Map.Common
{
    /// <summary>
    /// 具有方向的點
    /// </summary>
    public interface ITowardPair : IGeometry
    {
        /// <summary>
        /// 座標點
        /// </summary>
        IPair Position { get; set; }

        /// <summary>
        /// 方向
        /// </summary>
        IAngle Toward { get; set; }
    }
}