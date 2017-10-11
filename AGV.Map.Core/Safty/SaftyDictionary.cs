using AGV.Map.Common;
using SharpGL;
using System;
using System.Collections.Generic;

namespace AGV.Map.Core
{
    /// <summary>
    /// 具執行緒安全的字典介面，引索值為 uint
    /// </summary>
    [Serializable]
    internal class SaftyDictionary<T> : ISaftyDictionary<T> where T : IDrawable
    {
        private readonly object mKey = new object();

        private Dictionary<uint, T> mData = new Dictionary<uint, T>();

        public SaftyDictionary()
        {
        }

        /// <summary>
        /// 資料數
        /// </summary>
        public int Count { get { lock (mKey) return mData.Count; } }

        public T this[uint id] { get { lock (mKey) return mData[id]; } set { lock (mKey) mData[id] = value; } }

        /// <summary>
        /// 根據ID加入新元素，若ID本身存在，則將原本元素取代成新元素
        /// </summary>
        public void Add(uint id, T value) {
            lock (mKey)
            {
                if (mData.ContainsKey(id))
                    mData[id] = value;
                else
                    mData.Add(id, value);
            }
        }

        /// <summary>
        /// 清除所有資料
        /// </summary>
        public void Clear()
        { lock (mKey) mData.Clear(); }

        /// <summary>
        /// 是否包含該ID
        /// </summary>
        public bool ContainsID(uint id) { lock (mKey) return mData.ContainsKey(id); }

        /// <summary>
        /// 繪圖
        /// </summary>
        public void Draw(OpenGL gl)
        {
            lock (mKey)
            {
                foreach (var item in mData)
                {
                    item.Value.Draw(gl);
                }
            }
        }

        /// <summary>
        /// 移除指定元素
        /// </summary>
        public void Remove(uint id) { lock (mKey) mData.Remove(id); }

        /// <summary>
        /// 具執行緒安全編輯
        /// </summary>
        public void SaftyEdit(uint id, Action<T> action)
        {
            lock (mKey)
            {
                foreach (var item in mData)
                {
                    if (item.Key == id)
                    {
                        action(item.Value);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 具執行緒安全的迴圈操作
        /// </summary>
        public void SaftyForLoop(Action<uint, T> action)
        {
            lock (mKey)
            {
                foreach (var item in mData)
                {
                    action(item.Key, item.Value);
                }
            }
        }
    }
}