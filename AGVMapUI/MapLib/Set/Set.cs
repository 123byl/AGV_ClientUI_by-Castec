using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AGVMap
{
    /// <summary>
    /// 定義具執行緒安全的集合
    /// </summary>
    public interface ISet<T> : ISafty
    {
        /// <summary>
        /// 元素數
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 獲得唯讀值
        /// </summary>
        ReadOnlyCollection<T> AsReadOnly();

        /// <summary>
        /// 對所有陣列操作
        /// </summary>
        void ForEach(Action<T> action);

        /// <summary>
        /// 排序
        /// </summary>
        void Sort(Comparison<T> comparison);

        #region - 新增 -

        /// <summary>
        /// 加入新元素
        /// </summary>
        void Add(T item);

        /// <summary>
        /// 加入新元素
        /// </summary>
        void AddRange(ISet<T> collection);

        /// <summary>
        /// 加入新元素
        /// </summary>
        void AddRange(IEnumerable<T> collection);

        #endregion - 新增 -

        #region - 搜尋 -

        /// <summary>
        /// 由引索值獲得元素
        /// </summary>
        T At(int index);

        /// <summary>
        /// 搜尋
        /// </summary>
        int BinarySearch(T item);

        /// <summary>
        /// 搜尋
        /// </summary>
        T Find(Predicate<T> match);

        #endregion - 搜尋 -

        #region - 清除 -

        /// <summary>
        /// 清除資料
        /// </summary>
        void Clear();

        /// <summary>
        /// 清除舊資料，並加入新元素
        /// </summary>
        void ClearAndAdd(T item);

        /// <summary>
        /// 清除舊資料，並加入新元素
        /// </summary>
        void ClearAndAddRange(ISet<T> collection);

        /// <summary>
        /// 清除舊資料，並加入新元素
        /// </summary>
        void ClearAndAddRange(IEnumerable<T> collection);

        /// <summary>
        /// 移除符合的項目
        /// </summary>
        int RemoveAll(Predicate<T> match);

        #endregion - 清除 -
    }

    /// <summary>
    /// 以 List 實作具執行緒安全的集合
    /// </summary>
    public class ListSet<T> : ISet<T>
    {
        /// <summary>
        /// 元素數
        /// </summary>
        public int Count { get { lock (Key) return mDataList.Count; } }

        /// <summary>
        /// 執行續鎖
        /// </summary>
        public object Key { get; } = new object();

        /// <summary>
        /// 獲得唯讀值
        /// </summary>
        public ReadOnlyCollection<T> AsReadOnly()
        {
            lock (Key) return mDataList.AsReadOnly();
        }

        /// <summary>
        /// 對所有陣列操作
        /// </summary>
        public void ForEach(Action<T> action)
        {
            lock (Key) mDataList.ForEach(action);
        }

        /// <summary>
        /// 排序
        /// </summary>
        public void Sort(Comparison<T> comparison)
        {
            lock (Key) mDataList.Sort(comparison);
        }

        #region - 新增 -

        /// <summary>
        /// 加入新元素
        /// </summary>
        public void Add(T item)
        {
            lock (Key) mDataList.Add(item);
        }

        /// <summary>
        /// 加入新元素
        /// </summary>
        public void AddRange(ISet<T> collection)
        {
            lock (Key)
            {
                if (collection is ListSet<T>)
                {
                    ListSet<T> tmp = (ListSet<T>)collection;
                    mDataList.AddRange(tmp.mDataList);
                    return;
                }
                throw new Exception("Type Error");
            }
        }

        /// <summary>
        /// 加入新元素
        /// </summary>
        public void AddRange(IEnumerable<T> collection)
        {
            lock (Key) mDataList.AddRange(collection);
        }

        #endregion - 新增 -

        #region - 搜尋 -

        /// <summary>
        /// 由引索值獲得元素，當 index 不在陣列範圍時，擲出錯誤
        /// </summary>
        public T At(int index)
        {
            lock (Key)
            {
                if (index >= 0 && index < mDataList.Count) return mDataList[index];
            }
            throw new Exception("Out off range");
        }

        /// <summary>
        /// 搜尋
        /// </summary>
        public int BinarySearch(T item)
        {
            lock (Key) return mDataList.BinarySearch(item);
        }

        /// <summary>
        /// 搜尋
        /// </summary>
        public T Find(Predicate<T> match)
        {
            lock (Key)
            {
                return mDataList.Find(match);
            }
        }

        #endregion - 搜尋 -

        #region - 清除 -

        /// <summary>
        /// 清除資料
        /// </summary>
        public void Clear()
        {
            lock (Key) mDataList.Clear();
        }

        /// <summary>
        /// 清除舊資料，並加入新元素
        /// </summary>
        public void ClearAndAdd(T item)
        {
            lock (Key)
            {
                mDataList.Clear();
                mDataList.Add(item);
            }
        }

        /// <summary>
        /// 清除舊資料，並加入新元素
        /// </summary>
        public void ClearAndAddRange(ISet<T> collection)
        {
            lock (Key)
            {
                mDataList.Clear();
                if (collection is ListSet<T>)
                {
                    ListSet<T> tmp = (ListSet<T>)collection;
                    mDataList.AddRange(tmp.mDataList);
                    return;
                }
                throw new Exception("Type Error");
            }
        }

        /// <summary>
        /// 清除舊資料，並加入新元素
        /// </summary>
        public void ClearAndAddRange(IEnumerable<T> collection)
        {
            lock (Key)
            {
                mDataList.Clear();
                mDataList.AddRange(collection);
            }
        }

        /// <summary>
        /// 移除符合的項目
        /// </summary>
        public int RemoveAll(Predicate<T> match)
        {
            lock (Key) return mDataList.RemoveAll(match);
        }

        #endregion - 清除 -

        /// <summary>
        /// 資料 List 陣列
        /// </summary>
        private List<T> mDataList = new List<T>();
    }
}