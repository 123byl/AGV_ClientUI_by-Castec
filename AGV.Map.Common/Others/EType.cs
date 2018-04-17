using System;

namespace AGV.Map.Common
{
    /// <summary>
    /// 圖案類型
    /// </summary>
    [Serializable]
    public enum EType : int
    {
        /// <summary>
        /// 頂層
        /// </summary>
        Top = 0,

        /// <summary>
        /// 底層
        /// </summary>
        Bottom = -100,

        /// <summary>
        /// 拖曳物件邊框
        /// </summary>
        DragBound = -1,

        /// <summary>
        /// 車
        /// </summary>
        AGV = -10,

        /// <summary>
        /// 目標點
        /// </summary>
        Goal = -20,

        /// <summary>
        /// 禁止線
        /// </summary>
        ForbiddenLine = -24,

        /// <summary>
        /// 禁止面
        /// </summary>
        ForbiddenArea = -25,

        /// <summary>
        /// 雷射點
        /// </summary>
        LaserPoints = -23,

        /// <summary>
        /// 障礙線
        /// </summary>
        ObstacleLines = -91,

        /// <summary>
        /// 障礙點
        /// </summary>
        ObstaclePoints = -90,

        /// <summary>
        /// 網格
        /// </summary>
        Grid = -93,

        /// <summary>
        /// 坐標軸
        /// </summary>
        Axis = -92,

        /// <summary>
        /// 窄道
        /// </summary>
        NarrowLine = -26,

        /// <summary>
        /// 充電站
        /// </summary>
        Power = -21,

        /// <summary>
        /// 窄道暫時停車區
        /// </summary>
        Parking = -22,
    }
}