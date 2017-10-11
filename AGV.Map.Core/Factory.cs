using AGV.Map.Common;
using System;
using System.Collections.Generic;

namespace AGV.Map.Core
{
    /// <summary>
    /// 物件製造工廠
    /// </summary>
    public static class Factory
    {
        public static class FID
        {
            private static uint id = 0;
            public static uint GetID()
            {
                id++;
                return id;
            }
        }

        public static class FMuti
        {
            #region LaserPoints
            public static ILaserPoints LaserPoints()
            {
                return new LaserPoints();
            }

            public static ILaserPoints LaserPoints(IEnumerable<IPair> collection)
            {
                return new LaserPoints(collection);
            }

            public static ILaserPoints LaserPoints(IPair item)
            {
                return new LaserPoints(item);
            }
            #endregion
            #region ObstaclePoints
            public static IObstaclePoints ObstaclePoints()
            {
                return new ObstaclePoints();
            }

            public static IObstaclePoints ObstaclePoints(IEnumerable<IPair> collection)
            {
                return new ObstaclePoints(collection);
            }

            public static IObstaclePoints ObstaclePoints(IPair item)
            {
                return new ObstaclePoints(item);
            }
            #endregion
            #region ObstacleLines
            public static IObstacleLines ObstacleLines()
            {
                return new ObstacleLines();
            }

            public static IObstacleLines ObstacleLines(IEnumerable<ILine> collection)
            {
                return new ObstacleLines(collection);
            }

            public static IObstacleLines ObstacleLines(ILine item)
            {
                return new ObstacleLines(item);
            }
            #endregion
        }

        /// <summary>
        /// 拖曳元件製造工廠
        /// </summary>
        public static class FDrag
        {
            public static IDragManager DragManager()
            {
                return new DragManager();
            }
        }

        /// <summary>
        /// 幾何製造工廠
        /// </summary>
        public static class FGeometry
        {
            #region Angle

            public static IAngle Angle()
            {
                return new Angle();
            }

            public static IAngle Angle(double angle)
            {
                return new Angle(angle);
            }

            public static IAngle Angle(IAngle angle)
            {
                return new Angle(angle);
            }

            #endregion Angle

            #region Area

            public static IArea Area()
            {
                return new Area();
            }

            public static IArea Area(IArea area)
            {
                return new Area(area);
            }

            public static IArea Area(IPair min, IPair max)
            {
                return new Area(min, max);
            }

            public static IArea Area(int x0, int y0, int x1, int y1)
            {
                return new Area(x0, y0, x1, y1);
            }

            public static IArea Area(IPair center, int width, int height)
            {
                return new Area(center, width, height);
            }

            public static IArea Area(double x0, double y0, double x1, double y1)
            {
                return new Area(x0, y0, x1, y1);
            }

            #endregion Area

            #region Line

            public static ILine Line()
            {
                return new Line();
            }

            public static ILine Line(ILine line)
            {
                return new Line(line);
            }

            public static ILine Line(IPair beg, IPair end)
            {
                return new Line(beg, end);
            }

            public static ILine Line(int x0, int y0, int x1, int y1)
            {
                return new Line(x0, y0, x1, y1);
            }

            public static ILine Line(double x0, double y0, double x1, double y1)
            {
                return new Line(x0, y0, x1, y1);
            }

            #endregion Line

            #region Pair

            public static IPair Pair()
            {
                return new Pair();
            }

            public static IPair Pair(IPair pair)
            {
                return new Pair(pair);
            }

            public static IPair Pair(int x, int y)
            {
                return new Pair(x, y);
            }

            public static IPair Pair(double x, double y)
            {
                return new Pair(x, y);
            }

            #endregion Pair

            #region TowardPair

            public static ITowardPair TowardPair()
            {
                return new TowardPair();
            }

            public static ITowardPair TowardPair(ITowardPair towardPair)
            {
                return new TowardPair(towardPair);
            }

            public static ITowardPair TowardPair(IPair position, double toward)
            {
                return new TowardPair(position, toward);
            }

            public static ITowardPair TowardPair(IPair position, IAngle toward)
            {
                return new TowardPair(position, toward);
            }

            public static ITowardPair TowardPair(int x, int y, double toward)
            {
                return new TowardPair(x, y, toward);
            }

            public static ITowardPair TowardPair(int x, int y, IAngle toward)
            {
                return new TowardPair(x, y, toward);
            }

            public static ITowardPair TowardPair(double x, double y, double toward)
            {
                return new TowardPair(x, y, toward);
            }

            public static ITowardPair TowardPair(double x, double y, IAngle toward)
            {
                return new TowardPair(x, y, toward);
            }

            #endregion TowardPair
        }

        /// <summary>
        /// 其它元件製造工廠
        /// </summary>
        public static class FOthers
        {
            public static IBound<T> Bound<T>(T min, T max) where T : IComparable, new()
            {
                return new Bound<T>(min, max);
            }

            public static IColor Color(byte r, byte g, byte b, byte a = 255)
            {
                return new Color(r, g, b, a);
            }

            public static IColor Color()
            {
                return new Color();
            }

            public static IColor Color(IColor color)
            {
                return new Color(color.R, color.G, color.B, color.A);
            }

            public static IColor Color(System.Drawing.Color color)
            {
                return new Color(color.R, color.G, color.B, color.A);
            }

            public static IColor Color(System.Drawing.Color color, byte a)
            {
                return new Color(color.R, color.G, color.B, a);
            }

            public static IRecode<T> Recode<T>(T old, T now)
            {
                return new Recode<T>(old, now);
            }

            public static IRecode<T> Recode<T>(T initail)
            {
                return new Recode<T>(initail);
            }
        }

        /// <summary>
        /// 標示物製造工廠
        /// </summary>
        public static class FSingle
        {
            /// <summary>
            /// 標示面製造工廠
            /// </summary>
            public static class FArea
            {
                #region ForbiddenArea

                public static IForbiddenArea ForbiddenArea(string name)
                {
                    return new ForbiddenArea(name);
                }

                public static IForbiddenArea ForbiddenArea(int x0, int y0, int x1, int y1, string name)
                {
                    return new ForbiddenArea(x0, y0, x1, y1, name);
                }

                public static IForbiddenArea ForbiddenArea(IArea area, string name)
                {
                    return new ForbiddenArea(area, name);
                }

                public static IForbiddenArea ForbiddenArea(IPair min, IPair max, string name)
                {
                    return new ForbiddenArea(min, max, name);
                }

                #endregion ForbiddenArea
            }

            /// <summary>
            /// 標示線製造工廠
            /// </summary>
            public static class FLine
            {
                #region ForbiddenLine

                public static IForbiddenLine ForbiddenLine(string name)
                {
                    return new ForbiddenLine(name);
                }

                public static IForbiddenLine ForbiddenLine(int x0, int y0, int x1, int y1, string name)
                {
                    return new ForbiddenLine(x0, y0, x1, y1, name);
                }

                public static IForbiddenLine ForbiddenLine(ILine line, string name)
                {
                    return new ForbiddenLine(line, name);
                }

                public static IForbiddenLine ForbiddenLine(IPair beg, IPair end, string name)
                {
                    return new ForbiddenLine(beg, end, name);
                }

                #endregion ForbiddenLine

                #region NarrowLine

                public static INarrowLine NarrowLine(string name)
                {
                    return new NarrowLine(name);
                }

                public static INarrowLine NarrowLine(int x0, int y0, int x1, int y1, string name)
                {
                    return new NarrowLine(x0, y0, x1, y1, name);
                }

                public static INarrowLine NarrowLine(ILine line, string name)
                {
                    return new NarrowLine(line, name);
                }

                public static INarrowLine NarrowLine(IPair beg, IPair end, string name)
                {
                    return new NarrowLine(beg, end, name);
                }

                #endregion NarrowLine
            }

            /// <summary>
            /// 標示點製造工廠
            /// </summary>
            public static class FTowardPair
            {
                #region AGV

                public static IAGV AGV(string name)
                {
                    return new AGV(name);
                }

                public static IAGV AGV(int x, int y, double toward, string name)
                {
                    return new AGV(x, y, toward, name);
                }

                public static IAGV AGV(int x, int y, IAngle toward, string name)
                {
                    return new AGV(x, y, toward, name);
                }

                public static IAGV AGV(ITowardPair towardPair, string name)
                {
                    return new AGV(towardPair, name);
                }

                #endregion AGV

                #region Power

                public static IPower Power(string name)
                {
                    return new Power(name);
                }

                public static IPower Power(int x, int y, double toward, string name)
                {
                    return new Power(x, y, toward, name);
                }

                public static IPower Power(int x, int y, IAngle toward, string name)
                {
                    return new Power(x, y, toward, name);
                }

                public static IPower Power(ITowardPair towardPair, string name)
                {
                    return new Power(towardPair, name);
                }

                #endregion Power

                #region Goal

                public static IGoal Goal(string name)
                {
                    return new Goal(name);
                }

                public static IGoal Goal(int x, int y, double toward, string name)
                {
                    return new Goal(x, y, toward, name);
                }

                public static IGoal Goal(int x, int y, IAngle toward, string name)
                {
                    return new Goal(x, y, toward, name);
                }

                public static IGoal Goal(ITowardPair towardPair, string name)
                {
                    return new Goal(towardPair, name);
                }

                #endregion Goal

                #region Parking

                public static IParking Parking(string name)
                {
                    return new Parking(name);
                }

                public static IParking Parking(int x, int y, double toward, string name)
                {
                    return new Parking(x, y, toward, name);
                }

                public static IParking Parking(int x, int y, IAngle toward, string name)
                {
                    return new Parking(x, y, toward, name);
                }

                public static IParking Parking(ITowardPair towardPair, string name)
                {
                    return new Parking(towardPair, name);
                }

                #endregion Parking
            }
        }
    }
}