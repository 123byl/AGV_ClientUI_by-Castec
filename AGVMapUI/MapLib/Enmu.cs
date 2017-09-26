namespace AGVMap
{
    /// <summary>
    /// 標示區類型
    /// </summary>
    public enum EAreaType : int
    {
        /// <summary>禁止區</summary>
        ForbiddenArea = ELayer.ForbiddenArea
    }

    /// <summary>
    /// 圖層定義(數值越大，圖越在頂端)
    /// </summary>
    public enum ELayer : int
    {
        /// <summary>頂層</summary>
        Top = -0,

        /// <summary>底層</summary>
        Buttom = -100,

        /// <summary>控制點</summary>
        CtrlPoint = -1,

        /// <summary>選取</summary>
        Selected = -2,

        /// <summary>AGV 車</summary>
        AGV = -3,

        /// <summary>標示物預設</summary>
        MarkDefault = -10,

        /// <summary>充電站</summary>
        Power = -11,

        /// <summary>目標點</summary>
        Goal = -12,

        /// <summary>標示線預設</summary>
        LineDefault = -20,

        /// <summary>禁止線</summary>
        ForbiddenLine = -21,

        /// <summary>標示區預設</summary>
        SuperAreaDefault = -30,

        /// <summary>禁止區</summary>
        ForbiddenArea = -31,

        /// <summary>坐標軸</summary>
        Axis = -80,

        /// <summary>網格</summary>
        Grid = -81,

        /// <summary>障礙點</summary>
        PointSet = -90,

        /// <summary>障礙線</summary>
        LineSet = -91,

        /// <summary>障礙面</summary>
        AreaSet = -92,
    }

    /// <summary>
    /// 標示點類型
    /// </summary>
    public enum EMarkType : int
    {
        /// <summary>AGV 車</summary>
        AGV = ELayer.AGV,

        /// <summary>充電站</summary>
        Power = ELayer.Power,

        /// <summary>目標點</summary>
        Goal = ELayer.Goal,
    }

    /// <summary>滑鼠類型</summary>
    public enum EMouseType : int
    {
        /// <summary>平移模式</summary>
        TranslationMode = 0,

        /// <summary>選擇模式</summary>
        SelectedMode = 1,

        /// <summary>編輯模式</summary>
        EditMode = 2,
    }

    /// <summary>
    /// 標示線類型
    /// </summary>
    public enum ESuperLineType : int
    {
        /// <summary>禁止線</summary>
        ForbiddenLine = ELayer.ForbiddenLine
    }
}