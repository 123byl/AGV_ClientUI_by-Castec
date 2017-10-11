﻿namespace AGV.Map.Common
{
    /// <summary>
    /// 顯示集合介面
    /// </summary>
    public interface IMuti<TGeometry> : IDrawable, IName, IHasGLSetting where TGeometry : IGeometry
    {
        /// <summary>
        /// 集合資料
        /// </summary>
        ISaftyList<TGeometry> DataList { get; }
    }
}