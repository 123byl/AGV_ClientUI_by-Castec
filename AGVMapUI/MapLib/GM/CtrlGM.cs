using SharpGL;
using System;
using System.Collections.Generic;

namespace AGVMap
{
    /// <summary>
    /// 具執行緒安全的可控圖形管理器
    /// </summary>
    public abstract class CtrlGM<TKey, TValue> : GM<TKey, TValue> where TValue : ICtrlable
    {
        /// <summary>
        /// 加入新元素
        /// </summary>
        public override void Add(TKey key, TValue item)
        {
            lock (Key)
            {
                mDic[key] = item;
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
                    item.Value.Draw(gl);
                }
            }
        }

        /// <summary>
        /// 編輯
        /// </summary>
        public bool Edit(Action<KeyValuePair<TKey, TValue>> action)
        {
            lock (Key)
            {
                foreach (var item in mDic)
                {
                    action(item);
                }
                return false;
            }
        }

        /// <summary>
        /// 根據條件尋找
        /// </summary>
        public bool Find(Predicate<KeyValuePair<TKey, TValue>> match, ref TValue res)
        {
            lock (Key)
            {
                foreach (var item in mDic)
                {
                    if (match(item))
                    {
                        res = item.Value;
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
