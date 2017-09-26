namespace AGVMap
{
    /// <summary>
    /// 具有逆時針頂點座標
    /// </summary>
    public interface ICCWVertex
    {
        /// <summary>
        /// 獲得逆時針排序的幾何頂點陣列，
        /// 點的資料長度為 1，
        /// 線的資料長度為 2，
        /// 面的資料長度為 4。以此類推
        /// </summary>
        IPair[] VertexArray { get; }
    }
}
