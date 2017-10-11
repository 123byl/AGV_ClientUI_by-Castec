using AGV.Map.Common;

namespace AGV.Map.Core
{
    /// <summary>
    /// 拖曳控制點
    /// </summary>
    internal class DragPoint : IDragPoint
    {
        public DragPoint(IPair point, int size, DelDragAction action)
        {
            Point = new Pair(point);
            Size = new Pair(size, size);
            DragActionEvent = action;
        }

        public DragPoint(IPair point, IPair size, DelDragAction action)
        {
            Point = new Pair(point);
            Size = new Pair(size);
            DragActionEvent = action;
        }

        public DragPoint(IPair point, DelDragAction action)
        {
            Point = new Pair(point);
            Size = new Pair(100, 100);
            DragActionEvent = action;
        }

        /// <summary>
        /// 拖曳點更新時動作
        /// </summary>
        public event DelDragAction DragActionEvent;

        /// <summary>
        /// 拖曳點
        /// </summary>
        public IPair Point { get; private set; } = new Pair();

        /// <summary>
        /// 拖曳點大小
        /// </summary>
        public IPair Size { get; private set; } = new Pair();

        /// <summary>
        /// 更新拖曳點座標
        /// </summary>
        public void UpdateDragPoint(IPair newPoint)
        {
            DragActionEvent?.Invoke(newPoint);
        }
    }
}