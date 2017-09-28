namespace AGVMap
{
    /// <summary>
    /// 幾何中心座標
    /// </summary>
    public interface ICenter
    {
        /// <summary>
        /// 中心座標
        /// </summary>
        IPair Center { get; }
    }

    /// <summary>
    /// 可控的
    /// </summary>
    public interface ICtrlable : IDrawable, INameable, IVisible, IID, ICenter
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
    /// 具有識別碼的
    /// </summary>
    public interface IID
    {
        /// <summary>
        /// 識別碼
        /// </summary>
        int ID { get; }
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

    /// <summary>
    /// ID 產生器
    /// </summary>
    public class IDCreater
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; } = Factory.CreatID.NewID;

        public static implicit operator int(IDCreater creater)
        {
            return creater.ID;
        }
    }
}
