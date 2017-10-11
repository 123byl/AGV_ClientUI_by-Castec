using System;

namespace AGV.Map.Common
{
    /// <summary>
    /// 自動 Mapping 上下邊界的物件
    /// </summary>
    public interface IBound<T> where T : IComparable
    {
        /// <summary>
        /// 上界
        /// </summary>
        T Max { get; }

        /// <summary>
        /// 下界
        /// </summary>
        T Min { get; }

        /// <summary>
        /// 值
        /// </summary>
        T Value { get; set; }
    }
}