using AGV.Map.Common;
using System;

namespace AGV.Map.Core
{
    /// <summary>
    /// OpenGL 繪圖設定
    /// </summary>
    [Serializable]
    internal class GLSetting : IGLSetting
    {
        private Color mMainColor = new Color();

        private Pair mSize = new Pair(500, 500);

        private Color mSubColor = new Color();

        public GLSetting()
        {
        }

        public GLSetting(EType type)
        {
            Type = type;
            switch (type)
            {
                case EType.Top:
                    return;

                case EType.Bottom:
                    return;

                case EType.DragBound:
                    MainColor = new Color(System.Drawing.Color.Red);
                    LineStyle = ELineStyle._1100110011001100;
                    LineWidth = 2;
                    return;

                case EType.AGV:
                    BmpName = "AGV";
                    TowardLength = 0;
                    return;

                case EType.Goal:
                    BmpName = "Goal";
                    return;

                case EType.ForbiddenLine:
                    MainColor = new Color(System.Drawing.Color.Orange, 150);
                    LineWidth = 3;
                    return;

                case EType.ForbiddenArea:
                    MainColor = new Color(System.Drawing.Color.Orange, 150);
                    return;

                case EType.LaserPoints:
                    MainColor = new Color(System.Drawing.Color.Red);
                    PointSize = 3;
                    return;

                case EType.ObstacleLines:
                    return;

                case EType.ObstaclePoints:
                    return;

                case EType.NarrowLine:
                    BmpName = "NarrowLine";
                    MainColor = new Color(System.Drawing.Color.Red, 150);
                    LineStyle = ELineStyle._1111111011111110;
                    return;

                case EType.Power:
                    BmpName = "Power";
                    TowardLength = 1200;
                    return;

                case EType.Parking:
                    BmpName = "Parking";
                    return;
            }
        }

        /// <summary>
        /// 圖檔來源
        /// </summary>
        public string BmpName { get; set; } = string.Empty;

        /// <summary>
        /// 線段樣式
        /// </summary>
        public ELineStyle LineStyle { get; set; } = ELineStyle._1111111111111111;

        /// <summary>
        /// 線條寬
        /// </summary>
        public float LineWidth { get; set; } = 1.0f;

        /// <summary>
        /// 主要顏色
        /// </summary>
        public IColor MainColor { get { return mMainColor; } set { mMainColor = new Color(value); } }

        /// <summary>
        /// 點大小
        /// </summary>
        public float PointSize { get; set; } = 1.0f;

        /// <summary>
        /// 繪圖尺寸
        /// </summary>
        public IPair Size { get { return mSize; } set { mSize = new Pair(value); } }

        /// <summary>
        /// 次要顏色
        /// </summary>
        public IColor SubColor { get { return mSubColor; } set { mSubColor = new Color(value); } }

        /// <summary>
        /// 標示線指示方向
        /// </summary>
        public int TowardLength { get; set; } = 250;

        /// <summary>
        /// 圖案類型
        /// </summary>
        public EType Type { get; set; } = EType.Top;
    }
}