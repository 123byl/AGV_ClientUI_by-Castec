using AGV.Map.Common;
using System;

namespace AGV.Map.Core
{
    /// <summary>
    /// 自動 Mapping 上下邊界的物件
    /// </summary>
    internal class Bound<T> : IBound<T> where T : IComparable, new()
    {
        private T mValue = new T();

        public Bound(T min, T max)
        {
            Max = min.CompareTo(max) >= 0 ? min : max;
            Min = min.CompareTo(max) <= 0 ? min : max;
            Value = new T();
        }

        /// <summary>
        /// 上界
        /// </summary>
        public T Max { get; } = new T();

        /// <summary>
        /// 下界
        /// </summary>
        public T Min { get; } = new T();

        /// <summary>
        /// 值
        /// </summary>
        public T Value {
            get { return mValue; }
            set {
                if (value.CompareTo(Min) == -1)
                {
                    mValue = Min;
                }
                else if (value.CompareTo(Max) == 1)
                {
                    mValue = Max;
                }
                else
                {
                    mValue = value;
                }
            }
        }
    }
}