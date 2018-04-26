using CtParamEditor.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtParamEditor.Core {
    public static class Extension {

        /// <summary>
        /// 將欄位名稱轉換為Enum形態
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static EmColumn ToEnumColumn(this string columnName) {
            EmColumn emColumn = EmColumn.None;
            switch (columnName) {
                case nameof(IParamColumn.Name):
                    emColumn = EmColumn.Name;
                    break;
                case nameof(IParamColumn.Type):
                    emColumn = EmColumn.Type; 
                    break;
                case nameof(IParamColumn.Description):
                    emColumn = EmColumn.Description;
                    break;
                case nameof(IParamColumn.Value):
                    emColumn = EmColumn.Value;
                    break;
                case nameof(IParamColumn.Max):
                    emColumn = EmColumn.Max;
                    break;
                case nameof(IParamColumn.Min):
                    emColumn = EmColumn.Min;
                    break;
                case nameof(IParamColumn.Default):
                    emColumn = EmColumn.Default;
                    break;
                default:
                    throw new Exception($"未定義欄位名稱{columnName}");
            }
            return emColumn;
        }

    }
}
