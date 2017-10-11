namespace AGV.Map.Common
{
    /// <summary>
    /// 標示物介面
    /// </summary>
    public interface ISingle<TGeometry> : IDrawable, IName, IDragable, IHasGLSetting where TGeometry : IGeometry
    {
        /// <summary>
        /// 座標資料
        /// </summary>
        TGeometry Data { get; }
    }
}