namespace AGVMap
{
    /// <summary>
    /// 具有執行緒安全的類別
    /// </summary>
    public interface ISafty
    {
        /// <summary>
        /// 執行緒鎖
        /// </summary>
        object Key { get; }
    }
}
