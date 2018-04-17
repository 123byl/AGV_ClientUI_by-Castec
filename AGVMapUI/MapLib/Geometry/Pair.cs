using System;

namespace AGVMap
{
    /// <summary>
    /// 定義整數數對
    /// </summary>
    public interface IPair
    {
        /// <summary>
        /// X 座標
        /// </summary>
        int X { get; set; }

        /// <summary>
        /// Y 座標
        /// </summary>
        int Y { get; set; }
    }

    /// <summary>
    /// 整數數對
    /// </summary>
    public class Pair : IPair, ICCWVertex
    {
        /// <summary>
        /// 整數數對預設建構子 X = 0, Y = 0 
        /// </summary>
        public Pair()
        {
        }

        /// <summary>
        /// 由介面建構新數對
        /// </summary>
        public Pair(IPair p)
        {
            X = p.X;
            Y = p.Y;
        }

        /// <summary>
        /// 整數數對建構子
        /// </summary>
        public Pair(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 獲得逆時針排序的幾何頂點陣列
        /// </summary>
        public IPair[] VertexArray { get { return new IPair[] { this }; } }

        /// <summary>
        /// X 座標
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y 座標
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 比較座標是否相等(先比較 X 再比較 Y)
        /// </summary>
        public static int Compare(IPair a, IPair b)
        {
            int cmpX = a.X.CompareTo(b.X);
            if (cmpX != 0) return cmpX;
            return a.Y.CompareTo(b.Y);
        }

        #region - 運算 -

        /// <summary>
        /// 逐一對每個陣列中的元素進行比較，若陣列大小不相等則擲出例外
        /// </summary>
        public static int Compare(IPair[] a, IPair[] b)
        {
            if (a != null && b != null && a.Length == b.Length)
            {
                int cmp = 0;
                for (int i = 0; i < a.Length; i++)
                {
                    cmp = Compare(a[i], (b[i]));
                    if (cmp != 0) return cmp;
                }
                return cmp;
            }
            throw new Exception("list0.Length != list1.Length");
        }

        public static Pair operator -(Pair lhs, Pair rhs)
        {
            return new Pair(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        public static Pair operator /(Pair lhs, int rhs)
        {
            return new Pair(lhs.X / rhs, lhs.Y / rhs);
        }

        public static Pair operator +(Pair lhs, Pair rhs)
        {
            return new Pair(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

        /// <summary>
        /// 到原點(0,0)的長度
        /// </summary>
        public double Length()
        {
            return Math.Sqrt(Length2());
        }

        /// <summary>
        /// 線段長度的平方
        /// </summary>
        public double Length2()
        {
            return X * X + Y * Y;
        }

        #endregion - 運算 -

        #region 運算子

        public static bool operator !=(Pair p1, Pair p2)
        {
            return !(p1 == p2);
        }

        public static bool operator ==(Pair lhs, Pair rhs)
        {
            return lhs.X == rhs.X && lhs.Y == rhs.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Pair)) return false;
            return this == (obj as Pair);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        #endregion 運算子
    }
}