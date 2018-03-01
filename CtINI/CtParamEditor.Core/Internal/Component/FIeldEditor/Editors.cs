using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtParamEditor.Core.Internal.Component.FIeldEditor {

    /// <summary>
    /// 名稱欄位編輯器
    /// </summary>
    internal class NameEditor : BaseFieldEditor {
        protected override int mIdx { get; } = PropField.Idx.Name;
        protected override string mTitle { get; } = "Param name";
        protected override string mDescription { get; } = "Please enter the parameter name";
        public override bool RecordEmpty { get; } = true;
        protected override bool abEdit(string type, string oriVal, out string rtnVal) {
            return TextInput(oriVal, out rtnVal);
        }
        protected override string OriVal(IParam prop) {
            return prop.Name;
        }
    }

    /// <summary>
    /// 參數型態編輯器
    /// </summary>
    internal class ValTypeEditor : BaseFieldEditor {
        protected override int mIdx { get; } = PropField.Idx.ValType;
        protected override string mTitle { get; } = "New data type";
        protected override string mDescription { get; } = "Please select a new data type";
        public override bool RecordEmpty { get; } = true;
        protected override bool abEdit(string type, string oriVal, out string rtnVal) {
            List<string> Types = GetTypes?.Invoke().ToList();
            Types.AddRange(new string[] {
                        typeof(bool).Name,
                        typeof(int).Name,
                        typeof(float).Name,
                        typeof(string).Name
                    });
            return ComboInput(oriVal, out rtnVal, Types);
        }
        protected override string OriVal(IParam prop) {
            return prop.Type;
        }
    }

    /// <summary>
    /// 說明欄位編輯器
    /// </summary>
    internal class DescriptionEditor : BaseFieldEditor {
        protected override int mIdx { get; } = PropField.Idx.Description;
        protected override string mTitle { get; } = "Description";
        protected override string mDescription { get; } = "Please enter the description";
        protected override bool abEdit(string type, string oriVal, out string rtnVal) {
            return TextInput(oriVal, out rtnVal);
        }
        protected override string OriVal(IParam prop) {
            return prop.Description;
        }
    }

    /// <summary>
    /// 最大值欄位編輯器
    /// </summary>
    internal class MaxEditor : BaseFieldEditor {
        protected override int mIdx { get; } = PropField.Idx.Max;
        protected override string mTitle { get; } = "Max";
        protected override string mDescription { get; } = "Please enter the Max";
        protected override bool abEdit(string type, string oriVal, out string rtnVal) {
            return TextInput(oriVal, out rtnVal);
        }
        protected override string OriVal(IParam prop) {
            return prop.Max;
        }
    }

    /// <summary>
    /// 最小值欄位編輯器
    /// </summary>
    internal class MinEditor : BaseFieldEditor {
        protected override int mIdx { get; } = PropField.Idx.Min;
        protected override string mTitle { get; } = "Min";
        protected override string mDescription { get; } = "Please enter the Min";
        protected override bool abEdit(string type, string oriVal, out string rtnVal) {
            return TextInput(oriVal, out rtnVal);
        }
        protected override string OriVal(IParam prop) {
            return prop.Min;
        }
    }

    /// <summary>
    /// 預設值欄位編輯器
    /// </summary>
    internal class DefEditor : BaseFieldEditor {
        protected override int mIdx { get; } = PropField.Idx.Default;
        protected override string mTitle { get; } = "Default";
        protected override string mDescription { get; } = "Please enter the Default";
        protected override bool abEdit(string type, string oriVal, out string rtnVal) {
            return ValueInput(type, oriVal, out rtnVal);
        }
        protected override string OriVal(IParam prop) {
            return prop.Default;
        }
    }

    /// <summary>
    /// 參數值欄位編輯器
    /// </summary>
    internal class ValueEditor : BaseFieldEditor {
        protected override int mIdx { get; } = PropField.Idx.Value;
        protected override string mTitle { get; } = "New value";
        protected override string mDescription { get; } = "Please enter new value";
        protected override bool abEdit(string type, string oriVal, out string rtnVal) {
            return ValueInput(type, oriVal, out rtnVal);
        }
        protected override string OriVal(IParam prop) {
            return prop.Value;
        }
    }

}
