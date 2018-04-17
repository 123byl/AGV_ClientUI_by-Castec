namespace AGVMap
{
    /// <summary>
    /// 滑鼠拖曳控制點，
    /// 當使用者改變 CtrlPoint 的值時程式會自動呼叫 DragEvent 來做對應的動作。
    /// </summary>
    public class DragPoint
    {
        /// <summary>
        /// 建構滑鼠控制點
        /// </summary>
        /// <param name="ctrlPoint">初始控制點座標</param>
        /// <param name="e">控制點被拖曳時的事件</param>
        /// <param name="size">控制區域大小</param>
        public DragPoint(IPair ctrlPoint, DelDragEvent e, IPair size)
        {
            CtrlPoint = ctrlPoint;
            Size = size;
            DragEvent = e;
        }

        /// <summary>
        /// 建構滑鼠控制點
        /// </summary>
        /// <param name="ctrlPoint">控制點</param>
        /// <param name="e">控制點被拖曳時的事件</param>
        /// <param name="size">控制區域</param>
        public DragPoint(IPair ctrlPoint, DelDragEvent e, int size = 100)
        {
            CtrlPoint = ctrlPoint;
            Size = new Pair(size, size);
            DragEvent = e;
        }

        /// <summary>
        /// 滑鼠拖曳事件
        /// </summary>
        /// <param name="newPos">滑鼠移動後位置</param>
        public delegate void DelDragEvent(IPair newPos);

        #region - 控制點 -

        /// <summary>
        /// 控制點
        /// </summary>
        public IPair CtrlPoint { get { return mCtrlPoint; } set { if (DragEvent != null) DragEvent(value); mCtrlPoint = new Pair(value); } }

        private IPair mCtrlPoint = new Pair();

        #endregion - 控制點 -

        /// <summary>
        /// 控制區域
        /// </summary>
        public IPair Size { get; private set; } = new Pair(100, 100);


        /// <summary>
        /// 滑鼠拖曳事件
        /// </summary>
        private event DelDragEvent DragEvent;
    }
}
