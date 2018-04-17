using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AGV.Map.Common
{
    /// <summary>
    /// 具執行緒安全的 List 介面
    /// </summary>
    public interface ISaftyList<T> : ISafty
    {
        /// <summary>
        /// 紀錄最後的編輯版本
        /// </summary>
        uint EditVersion { get; }

        /// <summary>
        /// 加入新元素
        /// </summary>
        void Add(T item);

        /// <summary>
        /// 加入多個新元素
        /// </summary>
        void AddRange(IEnumerable<T> collection);

        /// <summary>
        /// 獲得唯讀陣列
        /// </summary>
        ReadOnlyCollection<T> AsReadOnly();

        /// <summary>
        /// 是否存在符合項目
        /// </summary>
        bool Exists(Predicate<T> match);

        /// <summary>
        /// 移除所有符合條件的值
        /// </summary>
        void RemoveAll(Predicate<T> match);

        /// <summary>
        /// 具執行緒安全的迴圈操作
        /// </summary>
        void SaftyForLoop(Action<T> action);
    }
}