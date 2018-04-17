using CtParamEditor.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtParamEditor.Core.Internal {
    /// <summary>
    /// 參數值儲存器
    /// </summary>
    internal interface IParamValue {
        string ValType { get; }
        string Value { get; }
        string Max { get; }
        string Min { get; }
        string Def { get; }
        bool RangeDefinable { get; }
        bool SetValue(string val, int idx);
        Delegates.EnumData.DelContainItem ContainItem { get; set; }
        Type GetParamType();
    }

    /// <summary>
    /// AGV參數介面
    /// </summary>
    /// <remarks>
    /// 提供參數操作方法
    /// </remarks>
    internal interface IParam : IParamColumn {
        bool SetValue(string val, int idx);
        string GetParamValue(int idxCol);
        Delegates.EnumData.DelContainItem ContainItem { get; set; }
        Delegates.EnumData.DelContainType ContainType { get; set; }
        bool RangeDefinable { get; }
        Type GetParamType();
    }

    /// <summary>
    /// Enum資料型態定義
    /// </summary>
    internal interface IEnum {
        string TypeName { get; }
        string Value { get; }
        bool SetValue(string val);
        Delegates.EnumData.DelContainItem ContainItem { get; set; }
    }

}
