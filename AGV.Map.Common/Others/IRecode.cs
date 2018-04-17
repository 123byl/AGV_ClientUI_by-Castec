namespace AGV.Map.Common
{
    /// <summary>
    /// 可紀錄一筆過往資料的物件
    /// </summary>
    public interface IRecode<T>
    {
        /// <summary>
        /// 目前資料
        /// </summary>
        T Now { get; set; }

        /// <summary>
        /// 過往資料
        /// </summary>
        T Old { get; }

        /// <summary>
        /// 將當目前料存入過往資料中
        /// </summary>
        void Push();
    }
}