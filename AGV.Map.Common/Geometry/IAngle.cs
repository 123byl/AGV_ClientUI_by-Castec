namespace AGV.Map.Common
{
    /// <summary>
    /// 提供介於[0,360)之間的角度
    /// </summary>
    public interface IAngle : IGeometry
    {
        /// <summary>
        /// 角度值
        /// </summary>
        double Theta { get; set; }
    }
}