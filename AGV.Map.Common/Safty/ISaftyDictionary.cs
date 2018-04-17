using System;

namespace AGV.Map.Common
{
    /// <summary>
    /// 具執行緒安全的字典介面，引索值為 uint
    /// </summary>
    public interface ISaftyDictionary<T> : ISafty, IDrawable
    {
        T this[uint id] { get; set; }

        /// <summary>
        /// 根據ID加入新元素，若ID本身存在，則將原本元素取代成新元素
        /// </summary>
        void Add(uint id, T value);

        /// <summary>
        /// 是否包含該ID
        /// </summary>
        bool ContainsID(uint id);

        /// <summary>
        /// 移除指定元素
        /// </summary>
        void Remove(uint id);

        /// <summary>
        /// 具執行緒安全編輯
        /// </summary>
        void SaftyEdit(uint id, Action<T> action);

        /// <summary>
        /// 具執行緒安全的迴圈操作
        /// </summary>
        void SaftyForLoop(Action<uint, T> action);
    }
}