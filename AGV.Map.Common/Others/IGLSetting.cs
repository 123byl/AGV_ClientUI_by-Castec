namespace AGV.Map.Common
{
    /// <summary>
    /// OpenGL 繪圖設定
    /// </summary>
    public interface IGLSetting
    {
        /// <summary>
        /// 圖檔來源
        /// </summary>
        string BmpName { get; set; }

        /// <summary>
        /// 線段樣式
        /// </summary>
        ELineStyle LineStyle { get; set; }

        /// <summary>
        /// 線條寬
        /// </summary>
        float LineWidth { get; set; }

        /// <summary>
        /// 主要顏色
        /// </summary>
        IColor MainColor { get; set; }

        /// <summary>
        /// 點大小
        /// </summary>
        float PointSize { get; set; }

        /// <summary>
        /// 繪圖尺寸
        /// </summary>
        IPair Size { get; set; }

        /// <summary>
        /// 次要顏色
        /// </summary>
        IColor SubColor { get; set; }

        /// <summary>
        /// 標示線指示方向
        /// </summary>
        int TowardLength { get; set; }

        /// <summary>
        /// 圖案類型
        /// </summary>
        EType Type { get; set; }
    }
}