using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtParamEditor.Core.Internal {

    /// <summary>
    /// 欄位名稱常數定義
    /// </summary>
    public static class PropField {

        public static string Mapping(int idx) {
            switch (idx) {
                case Idx.Name: return Str.Name;
                case Idx.ValType: return Str.ValType;
                case Idx.Value: return Str.Value;
                case Idx.Description: return Str.Description;
                case Idx.Max: return Str.Max;
                case Idx.Min: return Str.Min;
                case Idx.Default: return Str.Default;
                default: throw new Exception("未定義Idx");
            }
        }

        public static int Mapping(string str) {
            switch (str) {
                case Str.Name: return Idx.Name;
                case Str.ValType: return Idx.ValType;
                case Str.Value: return Idx.Value;
                case Str.Description: return Idx.Description;
                case Str.Max: return Idx.Max;
                case Str.Min: return Idx.Min;
                case Str.Default: return Idx.Default;
                default: throw new Exception("未定義欄位名稱");
            }
        }

        public static class Str {
            public const string Name = "Name";
            public const string ValType = "Type";
            public const string Value = "Value";
            public const string Description = "Description";
            public const string Max = "Max";
            public const string Min = "Min";
            public const string Default = "Default";
        }
        public static class Idx {
            public const int Name = 0;
            public const int ValType = 1;
            public const int Value = 2;
            public const int Description = 3;
            public const int Max = 4;
            public const int Min = 5;
            public const int Default = 6;
        }
    }

}
