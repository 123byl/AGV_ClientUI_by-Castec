namespace AGV.Map.Common
{
    /// <summary>
    /// 座標
    /// </summary>
    public interface IPair : IGeometry
    {
        /// <summary>
        /// X 座標
        /// </summary>
        int X { get; set; }

        /// <summary>
        /// Y 座標
        /// </summary>
        int Y { get; set; }
    }
}