namespace AGV.Map.Common
{
    /// <summary>
    /// 拖曳點更新時動作
    /// </summary>
    public delegate void DelDragAction(IPair newPoint);

    /// <summary>
    /// 拖曳控制點
    /// </summary>
    public interface IDragPoint
    {
        /// <summary>
        /// 拖曳點更新時動作
        /// </summary>
        event DelDragAction DragActionEvent;

        /// <summary>
        /// 拖曳點
        /// </summary>
        IPair Point { get; }

        /// <summary>
        /// 拖曳點大小
        /// </summary>
        IPair Size { get; }

        /// <summary>
        /// 更新拖曳點座標
        /// </summary>
        void UpdateDragPoint(IPair newPoint);
    }
}