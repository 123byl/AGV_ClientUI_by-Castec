namespace AGV.Map.Common
{
    /// <summary>
    /// 車
    /// </summary>
    public interface IAGV : ISingle<ITowardPair>
    {
        /// <summary>
        /// 遠端 IP 位置
        /// </summary>
        string IP { get; set; }

        /// <summary>
        /// 地圖吻合度
        /// </summary>
        IBound<double> Match { get; }

        /// <summary>
        /// 電池電量
        /// </summary>
        IBound<double> Power { get; }
    }
}