using SharpGL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AGVMap
{
    /// <summary>
    /// 具執行緒安全的可繪集合管理器
    /// </summary>
    public abstract class DSetGM<TKey, TValue> : GM<TKey, IDSet<TValue>>
    {
        /// <summary>
        /// 加入新元素
        /// </summary>
        public void Add(TKey key, TValue item)
        {
            lock (Key)
            {
                if (mDic.ContainsKey(key))
                {
                    mDic[key].Add(item);
                }
                else
                {
                    mDic[key] = CreatSameGSet();
                    mDic[key].Add(item);
                }
            }
        }

        /// <summary>
        /// 加入新元素
        /// </summary>
        public override void Add(TKey key, IDSet<TValue> collection)
        {
            lock (Key)
            {
                if (mDic.ContainsKey(key))
                {
                    mDic[key].AddRange(collection);
                }
                else
                {
                    mDic[key] = CreatSameGSet();
                    mDic[key].AddRange(collection);
                }
            }
        }

        /// <summary>
        /// 加入新元素
        /// </summary>
        public void AddRange(TKey key, IEnumerable<TValue> collection)
        {
            lock (Key)
            {
                if (mDic.ContainsKey(key))
                {
                    mDic[key].AddRange(collection);
                }
                else
                {
                    mDic[key] = CreatSameGSet();
                    mDic[key].AddRange(collection);
                }
            }
        }

        /// <summary>
        /// 獲得唯讀值
        /// </summary>
        public ReadOnlyCollection<TValue> AsReadOnly(TKey key)
        {
            lock (Key)
            {
                if (mDic.ContainsKey(key))
                {
                    return mDic[key].AsReadOnly();
                }
                else
                {
                    mDic[key] = CreatSameGSet();
                    return mDic[key].AsReadOnly();
                }
            }
        }

        /// <summary>
        /// 清除資料
        /// </summary>
        public void Clear()
        {
            lock (Key)
            {
                mDic.Clear();
            }
        }

        /// <summary>
        /// 繪圖
        /// </summary>
        public override void Draw(OpenGL gl)
        {
            lock (Key)
            {
                foreach (var item in mDic)
                {
                    if (item.Value != null) item.Value.Draw(gl);
                }
            }
        }

        /// <summary>
        /// 對所有陣列操作
        /// </summary>
        public void ForEach(TKey key, Action<TValue> action)
        {
            lock (Key)
            {
                if (mDic.ContainsKey(key))
                {
                    mDic[key].ForEach(action);
                }
            }
        }

        /// <summary>
        /// 移除符合的項目
        /// </summary>
        public int RemoveAll(TKey key, Predicate<TValue> match)
        {
            lock (Key)
            {
                if (mDic.ContainsKey(key))
                {
                    return mDic[key].RemoveAll(match);
                }
                return 0;
            }
        }

        /// <summary>
        /// 建立相同的資料型態
        /// </summary>
        protected abstract IDSet<TValue> CreatSameGSet();
    }
}
