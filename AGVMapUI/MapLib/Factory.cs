using System.Collections.Generic;

namespace AGVMap
{
    /// <summary>
    /// 物件製造工廠
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// 建立可控的 AGV 車
        /// </summary>
        public static class CreatAGV
        {
            /// <summary>
            /// 建立可控的 AGV 車
            /// </summary>
            public static CtrlAGV AGV(string name, int x, int y)
            {
                return AGV(name, x, y, 0);
            }

            /// <summary>
            /// 建立可控的 AGV 車
            /// </summary>
            public static CtrlAGV AGV(string name, int x, int y, Angle toward)
            {
                CtrlAGV res = new CtrlAGV(x, y, toward);
                res.Property.Color = new Color(System.Drawing.Color.SkyBlue);
                res.Property.Layer = ELayer.AGV;
                res.Property.Size = 920;
                res.Property.TowardLength = 460;
                res.Property.Type = EMarkType.AGV;
                res.Name = name;
                return res;
            }
        }

        /// <summary>
        /// 建立可控的標示面
        /// </summary>
        public static class CreatCtrlArea
        {
            /// <summary>
            /// 建立可控的禁止區
            /// </summary>
            public static CtrlArea ForbiddenArea(string name, int x0, int y0, int x1, int y1)
            {
                CtrlArea res = new CtrlArea(x0, y0, x1, y1);
                res.Property.Color = new Color(System.Drawing.Color.Goldenrod, 150);
                res.Property.Layer = ELayer.ForbiddenArea;
                res.Property.Type = EAreaType.ForbiddenArea;
                res.Name = name;
                return res;
            }
        }

        /// <summary>
        /// 建立可控的標示線
        /// </summary>
        public static class CreatCtrlLine
        {
            /// <summary>
            /// 建立可控的禁止線
            /// </summary>
            public static CtrlLine ForbiddenLine(string name, int x0, int y0, int x1, int y1)
            {
                CtrlLine res = new CtrlLine(x0, y0, x1, y1);
                res.Property.Color = new Color(System.Drawing.Color.Goldenrod, 150);
                res.Property.Layer = ELayer.ForbiddenLine;
                res.Property.LineWidth = 3.0f;
                res.Property.Type = ESuperLineType.ForbiddenLine;
                res.Name = name;
                return res;
            }
        }

        /// <summary>
        /// 建立可控的標示點
        /// </summary>
        public static class CreatMark
        {
            /// <summary>
            /// 建立可控的 Goal 點
            /// </summary>
            public static CtrlMark Goal(string name, int x, int y)
            {
                return Goal(name, x, y, 0);
            }

            /// <summary>
            /// 建立可控的 Goal 點
            /// </summary>
            public static CtrlMark Goal(string name, int x, int y, Angle toward)
            {
                CtrlMark res = new CtrlMark(x, y, toward);
                res.Property.Color = new Color(System.Drawing.Color.Green);
                res.Property.Layer = ELayer.Goal;
                res.Property.Size = 300;
                res.Property.TowardLength = 150;
                res.Property.Type = EMarkType.Goal;
                res.Name = name;
                return res;
            }

            /// <summary>
            /// 建立可控的充電站
            /// </summary>
            public static CtrlMark Power(string name, int x, int y)
            {
                return Power(name, x, y, 0);
            }

            /// <summary>
            /// 建立可控的充電站
            /// </summary>
            public static CtrlMark Power(string name, int x, int y, Angle toward)
            {
                CtrlMark res = new CtrlMark(x, y, toward);
                res.Property.Color = new Color(System.Drawing.Color.Yellow);
                res.Property.Layer = ELayer.Power;
                res.Property.Size = 300;
                res.Property.TowardLength = 1200;
                res.Property.Type = EMarkType.Power;
                res.Name = name;
                return res;
            }
        }

        /// <summary>
        /// 獲得顯示物件唯一識別碼
        /// </summary>
        public static class CreatID
        {
            private static int mID = 0;

            /// <summary>
            /// 產生唯一的引索值
            /// </summary>
            public static int NewID {
                get {
                    lock (Key)
                    {
                        mID++;
                        return mID;
                    }
                }
            }

            /// <summary>
            /// 執行緒鎖
            /// </summary>
            private static object Key { get; } = new object();
        }

        /// <summary>
        /// 建立可繪集合
        /// </summary>
        public static class CreatObstacle
        {
            /// <summary>
            /// 建立可繪障礙面
            /// </summary>
            public static DASet ObstacleArea(IEnumerable<IArea> data)
            {
                DASet res = new DASet();
                res.AddRange(data);
                res.Color = new Color(System.Drawing.Color.Black);
                res.Layer = ELayer.AreaSet;
                return res;
            }

            /// <summary>
            /// 建立可繪障礙線
            /// </summary>
            public static DLSet ObstacleLine(IEnumerable<ILine> data)
            {
                DLSet res = new DLSet();
                res.AddRange(data);
                res.Color = new Color(System.Drawing.Color.Black);
                res.Layer = ELayer.LineSet;
                return res;
            }

            /// <summary>
            /// 建立可繪障礙點
            /// </summary>
            public static DPSet ObstaclePoint(IEnumerable<IPoint> data)
            {
                DPSet res = new DPSet();
                res.AddRange(data);
                res.Color = new Color(System.Drawing.Color.Black);
                res.Layer = ELayer.PointSet;
                return res;
            }
        }

        /// <summary>
        /// 進階模式
        /// </summary>
        internal static class Advance
        {
            /// <summary>
            /// 建立具執行緒安全的可繪滑鼠拖曳控制管理器
            /// </summary>
            public static class CreatDDragM
            {
                /// <summary>
                /// 建立具執行緒安全的可繪滑鼠拖曳控制管理器
                /// </summary>
                public static DDragM DDragM()
                {
                    DDragM res = new DDragM();
                    res.Property.Color = new Color(System.Drawing.Color.OrangeRed);
                    res.Property.Layer = ELayer.CtrlPoint;
                    res.Property.LineWidth = 1.0f;
                    res.Property.SelectdLineWidth = 2.0f;
                    res.Property.SelectedColor = new Color(System.Drawing.Color.Red);
                    return res;
                }
            }
        }
    }
}
