namespace AGV.Map.Common
{
    /// <summary>
    /// 線段
    /// </summary>
    public interface ILine : IGeometry
    {
        /// <summary>
        /// 起點座標
        /// </summary>
        IPair Begin { get; set; }

        /// <summary>
        /// 終點座標
        /// </summary>
        IPair End { get; set; }
    }
}