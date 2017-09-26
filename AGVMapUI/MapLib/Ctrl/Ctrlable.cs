namespace AGVMap
{
    /// <summary>
    /// 可控的
    /// </summary>
    public interface ICtrlable : IDrawable, INameable
    {
        /// <summary>
        /// 獲得滑鼠拖曳點陣列
        /// </summary>
        DragPoint[] GetMouseDragPoint();

        /// <summary>
        /// 現實座標 p 是否在圖形上
        /// </summary>
        bool Intersect(IPair p);
    }

    /// <summary>
    /// 可命名的
    /// </summary>
    public interface INameable
    {
        /// <summary>
        /// 物件名稱
        /// </summary>
        string Name { get; set; }
    }

    /// <summary>
    /// 可顯示或隱藏的
    /// </summary>
    public interface IVisible
    {
        /// <summary>
        /// 顯示/隱藏
        /// </summary>
        bool Visible { get; set; }
    }
}
