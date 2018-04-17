namespace AGV.Map.Common
{
    /// <summary>
    /// 有繪圖設定屬性的
    /// </summary>
    public interface IHasGLSetting
    {
        /// <summary>
        /// 繪圖設定
        /// </summary>
        IGLSetting GLSetting { get; }
    }
}