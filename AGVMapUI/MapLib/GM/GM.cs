using SharpGL;
using System.Collections.Generic;

namespace AGVMap
{
    /// <summary>
    /// 具執行緒安全的可繪圖形管理器(Graphic Manager)
    /// </summary>
    public interface IGM<TKey, TValue> : ISafty, IDrawable
    {
        /// <summary>
        /// 元素數量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 加入新元素
        /// </summary>
        void Add(TKey key, TValue item);

        /// <summary>
        /// 是否包含引索值
        /// </summary>
        bool ContainsKey(TKey key);

        /// <summary>
        /// 獲得引索值陣列
        /// </summary>
        List<TKey> GetKeyList();

        /// <summary>
        /// 移除元素
        /// </summary>
        bool Remove(TKey key);

        /// <summary>
        /// 獲得元素
        /// </summary>
        bool TryGetValue(TKey key, out TValue value);
    }

    /// <summary>
    /// 具執行緒安全的可繪圖形管理器(Graphic Manager)
    /// </summary>
    public abstract class GM<TKey, TValue> : IGM<TKey, TValue>
    {
        /// <summary>
        /// 元素數量
        /// </summary>
        public int Count { get { lock (Key) return mDic.Count; } }

        /// <summary>
        /// 執行緒鎖
        /// </summary>
        public object Key { get; } = new object();

        /// <summary>
        /// 加入新元素
        /// </summary>
        public abstract void Add(TKey key, TValue item);

        /// <summary>
        /// 是否包含引索值
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            lock (Key)
            {
                return mDic.ContainsKey(key);
            }
        }

        /// <summary>
        /// 繪圖
        /// </summary>
        public abstract void Draw(OpenGL gl);

        /// <summary>
        /// 獲得引索值陣列
        /// </summary>
        public List<TKey> GetKeyList()
        {
            lock (Key)
            {
                List<TKey> res = new List<TKey>();
                res.AddRange(mDic.Keys);
                return res;
            }
        }

        /// <summary>
        /// 移除元素
        /// </summary>
        public bool Remove(TKey key)
        {
            lock (Key)
            {
                return mDic.Remove(key);
            }
        }
        /// <summary>
        /// 獲得元素
        /// </summary>
        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (Key)
            {
                return mDic.TryGetValue(key, out value);
            }
        }

        /// <summary>
        /// 資料
        /// </summary>
        protected Dictionary<TKey, TValue> mDic = new Dictionary<TKey, TValue>();
    }
}
