namespace AGV.Map.Common
{
    /// <summary>
    /// 具執行續安全的存取集合基底介面
    /// </summary>
    public interface ISafty
    {
        /// <summary>
        /// 資料數
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 清除所有資料
        /// </summary>
        void Clear();
    }
}