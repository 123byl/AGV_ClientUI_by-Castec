namespace AGV.Map.Common
{
    /// <summary>
    /// 拖曳控制器狀態
    /// </summary>
    public enum EDragStatus
    {
        /// <summary>
        /// 閒置，沒有可拖曳物件
        /// </summary>
        Idle = 0,

        /// <summary>
        /// 已取得拖曳物件
        /// </summary>
        Ready = 1,

        /// <summary>
        /// 正在拖曳，已取得拖曳控制點
        /// </summary>
        Dragging = 2
    }
}