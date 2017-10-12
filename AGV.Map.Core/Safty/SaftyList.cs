using AGV.Map.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AGV.Map.Core
{
    /// <summary>
    /// 具執行緒安全的 List 介面
    /// </summary>
    [Serializable]
    internal class SaftyList<T> : ISaftyList<T>
    {
        private readonly object mKey = new object();
        private readonly uint mMaxEditVersion = 65536;
        private List<T> mData = new List<T>();
        private uint mEditVersion = 0;

        public SaftyList()
        {
        }

        public SaftyList(IEnumerable<T> collection)
        {
            AddRange(collection);
        }

        public SaftyList(T item)
        {
            Add(item);
        }

        /// <summary>
        /// 資料數
        /// </summary>
        public int Count { get { lock (mKey) return mData.Count; } }

        /// <summary>
        /// 紀錄最後的編輯版本
        /// </summary>
        public uint EditVersion { get { lock (mKey) return mEditVersion; } }

        /// <summary>
        /// 加入新元素
        /// </summary>
        public void Add(T item)
        {
            lock (mKey)
            {
                mData.Add(item);
                mEditVersion = (mEditVersion + 1) % mMaxEditVersion;
            }
        }

        /// <summary>
        /// 加入多個新元素
        /// </summary>
        public void AddRange(IEnumerable<T> collection)
        {
            lock (mKey)
            {
                mData.AddRange(collection);
                mEditVersion = (mEditVersion + 1) % mMaxEditVersion;
            }
        }

        /// <summary>
        /// 獲得唯讀陣列
        /// </summary>
        public ReadOnlyCollection<T> AsReadOnly()
        {
            lock (mKey) return mData.AsReadOnly();
        }

        /// <summary>
        /// 清除所有資料
        /// </summary>
        public void Clear()
        {
            lock (mKey)
            {
                mData.Clear();
                mEditVersion = (mEditVersion + 1) % mMaxEditVersion;
            }
        }

        /// <summary>
        /// 是否存在符合項目
        /// </summary>
        public bool Exists(Predicate<T> match) { lock (mKey) return mData.Exists(match); }

        /// <summary>
        /// 移除所有符合條件的值
        /// </summary>
        public void RemoveAll(Predicate<T> match)
        {
            lock (mKey)
            {
                mData.RemoveAll(match);
                mEditVersion = (mEditVersion + 1) % mMaxEditVersion;
            }
        }

        /// <summary>
        /// 具執行緒安全的迴圈操作
        /// </summary>
        public void SaftyForLoop(Action<T> action)
        {
            lock (mKey)
            {
                foreach (var item in mData)
                {
                    action(item);
                }
            }
        }
    }
}