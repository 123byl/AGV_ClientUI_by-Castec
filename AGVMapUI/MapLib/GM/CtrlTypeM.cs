namespace AGVMap
{
    /// <summary>
    /// 具執行緒安全的可控 AGV 管理器
    /// </summary>
    public class CtrlAGVGM<TKey> : CtrlGM<TKey, CtrlAGV>
    {
    }



    /// <summary>
    /// 具執行緒安全的可控 標示面 管理器
    /// </summary>
    public class CtrlAreaGM<TKey> : CtrlGM<TKey, CtrlArea>
    {
    }

    /// <summary>
    /// 具執行緒安全的可控 標示線 管理器
    /// </summary>
    public class CtrlLineGM<TKey> : CtrlGM<TKey, CtrlLine>
    {
    }

    /// <summary>
    /// 具執行緒安全的可控 標示物 管理器
    /// </summary>
    public class CtrlMarkGM<TKey> : CtrlGM<TKey, CtrlMark>
    {
    }
}