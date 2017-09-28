namespace AGVMap
{
    #region - 點 -

    /// <summary>
    /// 可繪點集合管理器
    /// </summary>
    public class DPSetGM<TKey> : DSetGM<TKey, IPair>
    {
        /// <summary>
        /// 建立相同的資料型態
        /// </summary>
        protected override IDSet<IPair> CreatSameGSet()
        {
            return new DPSet();
        }
    }

    #endregion - 點 -

    #region - 線 -

    /// <summary>
    /// 可繪線集合管理器
    /// </summary>
    public class DLSetGM<TKey> : DSetGM<TKey, ILine>
    {
        /// <summary>
        /// 建立相同的資料型態
        /// </summary>
        protected override IDSet<ILine> CreatSameGSet()
        {
            return new DLSet();
        }
    }

    #endregion - 線 -

    #region - 面 -

    /// <summary>
    /// 可繪面集合管理器
    /// </summary>
    public class DASetGM<TKey> : DSetGM<TKey, IArea>
    {
        /// <summary>
        /// 建立相同的資料型態
        /// </summary>
        protected override IDSet<IArea> CreatSameGSet()
        {
            return new DASet();
        }
    }

    #endregion - 面 -
}