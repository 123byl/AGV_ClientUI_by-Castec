using AGV.Map.Common;

namespace AGV.Map.Core
{
    /// <summary>
    /// 可紀錄一筆過往資料的物件
    /// </summary>
    internal class Recode<T> : IRecode<T>
    {
        public Recode(T initail)
        {
            Old = initail;
            Now = initail;
        }

        public Recode(T old, T now)
        {
            Old = old;
            Now = now;
        }

        /// <summary>
        /// 目前資料
        /// </summary>
        public T Now { get; set; }

        /// <summary>
        /// 過往資料
        /// </summary>
        public T Old { get; private set; }

        /// <summary>
        /// 將當目前料存入過往資料中
        /// </summary>
        public void Push()
        {
            Old = Now.DeepCopy();
        }
    }
}