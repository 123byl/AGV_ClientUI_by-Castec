namespace AGVMap
{
    #region - 可控的標示點屬性 -

    /// <summary>
    /// 可控的標示點屬性
    /// </summary>
    public interface ICtrlMarkProperty
    {
        /// <summary>
        /// 中心座標
        /// </summary>
        IPair Center { get; set; }

        /// <summary>
        /// 顏色
        /// </summary>
        IColor Color { get; set; }

        /// <summary>
        /// 圖層
        /// </summary>
        ELayer Layer { get; set; }

        /// <summary>
        /// 圖示大小
        /// </summary>
        int Size { get; set; }

        /// <summary>
        /// 首向
        /// </summary>
        Angle Toward { get; set; }

        /// <summary>
        /// 方向可控的標示線大小
        /// </summary>
        int TowardLength { get; set; }

        /// <summary>
        /// 種類
        /// </summary>
        EMarkType Type { get; set; }
    }

    /// <summary>
    /// 可控的標示點屬性
    /// </summary>
    public class CtrlMarkProperty : ICtrlMarkProperty, IToward
    {
        private Pair mCenter = new Pair();

        private Color mColor = new Color();

        /// <summary>
        /// 中心座標
        /// </summary>
        public IPair Center { get { return mCenter; } set { mCenter = new Pair(value); } }

        /// <summary>
        /// 顏色
        /// </summary>
        public IColor Color { get { return mColor; } set { mColor = new Color(value); } }

        /// <summary>
        /// 圖層
        /// </summary>
        public ELayer Layer { get; set; }

        /// <summary>
        /// 圖示大小
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 方向
        /// </summary>
        public Angle Toward { get; set; } = new Angle();

        /// <summary>
        /// 方向可控的標示線大小
        /// </summary>
        public int TowardLength { get; set; }

        /// <summary>
        /// 種類
        /// </summary>
        public EMarkType Type { get; set; }
    }

    #endregion - 可控的標示點屬性 -

    #region - 可控的標示面屬性 -

    /// <summary>
    /// 可控的標示面屬性
    /// </summary>
    public interface ICtrlAreaProperty
    {
        /// <summary>
        /// 面
        /// </summary>
        IArea Area { get; set; }

        /// <summary>
        /// 顏色
        /// </summary>
        IColor Color { get; set; }

        /// <summary>
        /// 圖層
        /// </summary>
        ELayer Layer { get; set; }

        /// <summary>
        /// 種類
        /// </summary>
        EAreaType Type { get; set; }
    }

    /// <summary>
    /// 可控的標示面屬性
    /// </summary>
    public class CtrlAreaProperty : ICtrlAreaProperty
    {
        private Area mArea = new Area();

        private Color mColor = new Color();

        /// <summary>
        /// 面
        /// </summary>
        public IArea Area { get { return mArea; } set { mArea = new Area(value); } }

        /// <summary>
        /// 顏色
        /// </summary>
        public IColor Color { get { return mColor; } set { mColor = new Color(value); } }

        /// <summary>
        /// 圖層
        /// </summary>
        public ELayer Layer { get; set; }

        /// <summary>
        /// 種類
        /// </summary>
        public EAreaType Type { get; set; }
    }

    #endregion - 可控的標示面屬性 -

    #region - 可控的標示線屬性 -

    /// <summary>
    /// 可控的標示線屬性
    /// </summary>
    public interface ICtrlLineProperty
    {  /// <summary>
       /// 顏色
       /// </summary>
        IColor Color { get; set; }

        /// <summary>
        /// 圖層
        /// </summary>
        ELayer Layer { get; set; }

        /// <summary>
        /// 線
        /// </summary>
        ILine Line { get; set; }

        /// <summary>
        /// 線寬
        /// </summary>
        float LineWidth { get; set; }

        /// <summary>
        /// 種類
        /// </summary>
        ELineType Type { get; set; }
    }

    /// <summary>
    /// 可控的標示線屬性
    /// </summary>
    public class CtrlLineProperty : ICtrlLineProperty
    {
        private Color mColor = new Color();

        private Line mLine = new Line();

        /// <summary>
        /// 顏色
        /// </summary>
        public IColor Color { get { return mColor; } set { mColor = new Color(value); } }

        /// <summary>
        /// 圖層
        /// </summary>
        public ELayer Layer { get; set; }

        /// <summary>
        /// 線
        /// </summary>
        public ILine Line { get { return mLine; } set { mLine = new Line(value); } }

        /// <summary>
        /// 線寬
        /// </summary>
        public float LineWidth { get; set; }

        /// <summary>
        /// 種類
        /// </summary>
        public ELineType Type { get; set; }
    }

    #endregion - 可控的標示線屬性 -

    #region - 可繪滑鼠拖曳控制管理器屬性 -

    /// <summary>
    /// 可繪滑鼠拖曳控制管理器屬性
    /// </summary>
    public interface IDDragMProperty
    {
        /// <summary>
        /// 顏色
        /// </summary>
        IColor Color { get; set; }

        /// <summary>
        /// 圖層
        /// </summary>
        ELayer Layer { get; set; }

        /// <summary>
        /// 線條寬
        /// </summary>
        float LineWidth { get; set; }

        /// <summary>
        /// 被選擇時線條寬
        /// </summary>
        float SelectdLineWidth { get; set; }

        /// <summary>
        /// 被選擇時顏色
        /// </summary>
        IColor SelectedColor { get; set; }
    }

    public class DDragMProperty : IDDragMProperty
    {
        private Color mColor = new Color();

        private Color mSColor = new Color();

        /// <summary>
        /// 顏色
        /// </summary>
        public IColor Color { get { return mColor; } set { mColor = new Color(value); } }

        /// <summary>
        /// 圖層
        /// </summary>
        public ELayer Layer { get; set; }

        /// <summary>
        /// 線條寬
        /// </summary>
        public float LineWidth { get; set; }

        /// <summary>
        /// 被選擇時線條寬
        /// </summary>
        public float SelectdLineWidth { get; set; }

        /// <summary>
        /// 被選擇時顏色
        /// </summary>
        public IColor SelectedColor { get { return mSColor; } set { mSColor = new Color(value); } }
    }

    #endregion - 可繪滑鼠拖曳控制管理器屬性 -
}
