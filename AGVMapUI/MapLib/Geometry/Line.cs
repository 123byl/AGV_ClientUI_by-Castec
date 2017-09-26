namespace AGVMap
{
    /// <summary>
    /// 定義線
    /// </summary>
    public interface ILine : ICCWVertex
    {
        /// <summary>
        /// 起點數對
        /// </summary>
        IPair Begin { get; set; }

        /// <summary>
        /// 終點數對
        /// </summary>
        IPair End { get; set; }
    }

    /// <summary>
    /// 基本幾何-線
    /// </summary>
    public class Line : ILine
    {
        /// <summary>
        /// 預設建構子 Begin(0,0), End(0,0)
        /// </summary>
        public Line()
        {
        }

        /// <summary>
        /// 線段建構子
        /// </summary>
        public Line(int x0, int y0, int x1, int y1)
        {
            Begin = new Pair(x0, y0);
            End = new Pair(x1, y1);
        }

        /// <summary>
        /// 由介面建構新線段
        /// </summary>
        public Line(ILine line)
        {
            Begin = line.Begin;
            End = line.End;
        }

        /// <summary>
        /// 線段建構子
        /// </summary>
        public Line(IPair p0, IPair p1)
        {
            Begin = p0;
            End = p1;
        }

        #region - 起點 -

        /// <summary>
        /// 起點數對
        /// </summary>
        public IPair Begin { get { return mBegin; } set { mBegin = new Pair(value); } }

        private Pair mBegin = new Pair();

        #endregion - 起點 -

        #region - 終點 -

        /// <summary>
        /// 終點數對
        /// </summary>
        public IPair End { get { return mEnd; } set { mEnd = new Pair(value); } }

        private Pair mEnd = new Pair();

        #endregion - 終點 -

        /// <summary>
        /// 獲得逆時針排序的幾何頂點陣列
        /// </summary>
        public IPair[] VertexArray { get { return new IPair[] { Begin, End }; } }
    }
}
