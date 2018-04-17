namespace AGV.Map.Common
{
    /// <summary>
    /// 拖曳控制點管理器
    /// </summary>
    public interface IDragManager : IHasGLSetting, IDrawable
    {
        /// <summary>
        /// 拖曳對象的邊緣
        /// </summary>
        ISaftyList<ILine> DragBound { get; }

        /// <summary>
        /// 當下拖曳點的邊緣
        /// </summary>
        ISaftyList<ILine> DragPointBound { get; }

        /// <summary>
        /// 拖曳對象
        /// </summary>
        IDragable DragTaeget { get; set; }

        /// <summary>
        /// 控制器狀態
        /// </summary>
        EDragStatus Status { get; }

        /// <summary>
        /// 拖曳
        /// </summary>
        void Drag(IPair newPosition);

        /// <summary>
        /// 釋放拖曳控制點
        /// </summary>
        void ReleaseControl();

        /// <summary>
        /// 使用座標嘗試嘗試取得拖曳控制點
        /// </summary>
        void TakeControl(IPair position);
    }
}