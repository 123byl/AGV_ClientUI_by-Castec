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
        bool IsDefineMax { get; }
        bool IsDefineMin { get; }
        bool IsDefineDefault { get; }
        bool RangeDefinable { get; }
        bool SetValue(string val, string columnName);
        Delegates.EnumData.DelContainItem ContainItem { get; set; }
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

    /// <summary>
    /// AGV參數介面
    /// </summary>
    /// <remarks>
    /// 提供參數操作方法
    /// </remarks>
   public interface IParam : IParamColumn {
        bool SetValue(string val, string columnName);
        /// <summary>
        /// 變數變更事件
        /// </summary>
        event EventHandler<string> ValueChanged;
        bool RangeDefinable { get; }
        Type GetParamType();
        //bool IsDefineMax();
        //bool IsDefineMin();
        //bool IsDefineDefault();
        /// <summary>
        /// 是否有欄位包含關鍵字
        /// </summary>
        /// <param name="keyWord"></param>
        bool FieldContains(string keyWord);
    }

}
