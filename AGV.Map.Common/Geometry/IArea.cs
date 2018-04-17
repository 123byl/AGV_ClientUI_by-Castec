namespace AGV.Map.Common
{
    /// <summary>
    /// 面，自動調整 Max 與 Min 使得 Max 總是維持在左上角，Min 總是在右下角
    /// </summary>
    public interface IArea : IGeometry
    {
        /// <summary>
        /// 最大值座標
        /// </summary>
        IPair Max { get; set; }

        /// <summary>
        /// 最小值座標
        /// </summary>
        IPair Min { get; set; }
    }
}