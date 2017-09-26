namespace AGVMap
{
    /// <summary>
    /// 定義點
    /// </summary>
    public interface IPoint : ICCWVertex, IPair
    {
    }

    /// <summary>
    /// 基本幾何-點
    /// </summary>
    public class Point : Pair, IPoint
    {
        /// <summary>
        /// 預設建構子 X = 0, Y = 0 
        /// </summary>
        public Point() : base()
        {
        }

        /// <summary>
        /// 座標點建構子
        /// </summary>
        public Point(int x, int y) : base(x, y)
        {
        }

        /// <summary>
        /// 由介面建構新座標
        /// </summary>
        public Point(IPair p) : base(p)
        {
        }

        /// <summary>
        /// 獲得逆時針排序的幾何頂點陣列
        /// </summary>
        public IPair[] VertexArray { get { return new IPair[] { this }; } }
    }
}
