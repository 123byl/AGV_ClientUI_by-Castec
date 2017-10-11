namespace AGV.Map.Common
{
    /// <summary>
    /// 可拖曳的
    /// </summary>
    public interface IDragable
    {
        /// <summary>
        /// 當下是否可以拖曳
        /// </summary>
        bool CanDrag { get; }

        /// <summary>
        /// 建立拖曳點陣列
        /// </summary>
        IDragPoint[] CreatDragPoints();
    }
}