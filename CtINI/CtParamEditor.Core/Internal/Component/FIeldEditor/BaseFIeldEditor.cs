using CtParamEditor.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtParamEditor.Core.Internal.Component.FIeldEditor {

    /// <summary>
    /// 欄位編輯器基類
    /// </summary>
    internal abstract class BaseFieldEditor {

        #region Declaration - Properties

        /// <summary>
        /// 編輯欄位索引值
        /// </summary>
        protected abstract int mIdx { get; }
        /// <summary>
        /// 輸入對話視窗標題
        /// </summary>
        protected abstract string mTitle { get; }
        /// <summary>
        /// 輸入對話視窗說明
        /// </summary>
        protected abstract string mDescription { get; }
        /// <summary>
        /// 是否紀錄空白欄位
        /// </summary>
        public virtual bool RecordEmpty { get; } = false;
        /// <summary>
        /// 非法欄位紀錄委派
        /// </summary>
        public Delegates.IllegalField.DelRecordIllegalField RecordIllegalField { get; set; } = null;
        /// <summary>
        /// 註銷非法欄位紀錄委派
        /// </summary>
        public Delegates.IllegalField.DelRemoveIllegalField RemoveIllegalField { get; set; } = null;
        /// <summary>
        /// 紀錄已修改欄位委派
        /// </summary>
        public Delegates.ModifiedField.DelAddModifiedField AddModifiedField { get; set; } = null;

        /// <summary>
        /// 取得列舉清單委派
        /// </summary>
        public Delegates.EnumData.DelGetItems GetItems { get; set; } = null;

        /// <summary>
        /// 類型定義檢查委派
        /// </summary>
        public Delegates.EnumData.DelContainType ContainType { get; set; } = null;

        public Delegates.EnumData.DelGetTypes GetTypes { get; set; } = null;

        public Input.Text InputText { get; set; } = null;

        public Input.ComboBox ComboBoxList { get; set; } = null;

        #endregion Declaration - Properties

        #region Function - Public Methods

        /// <summary>
        /// 編輯參數欄位
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public bool Edit(IParam prop) {
            string rtn = string.Empty;
            /*-- 取得原始參數值 --*/
            string oriVal = OriVal(prop);
            /*-- 讓使用者輸入新值 --*/
            bool suc = abEdit(prop.Type, oriVal, out rtn);
            if (suc) {
                /*-- 將新值寫入欄位 --*/
                suc = prop.SetValue(rtn, mIdx);
                if (suc) {
                    AddModifiedField.Invoke(prop, mIdx);
                    if (string.IsNullOrEmpty(rtn)) {
                        /*-- 記錄非法的欄位 --*/
                        RecordIllegal(prop, mIdx);
                    } else {
                        /*-- 移除非法紀錄 --*/
                        RemoveIllegal(prop, mIdx);
                    }
                }
            }
            return suc;
        }

        #endregion Funciton - Public Methods

        #region Function - Private Methods

        /// <summary>
        /// 以符合欄位的方式請使用者輸入編輯值
        /// </summary>
        /// <param name="rtn">使用者輸入值</param>
        /// <returns>使用者是否確認</returns>
        protected abstract bool abEdit(string type, string oriVal, out string rtnVal);

        /// <summary>
        /// 回傳欄位原始數值
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        protected abstract string OriVal(IParam prop);

        /// <summary>
        /// 以輸入字串方式供使用者編輯
        /// </summary>
        /// <param name="oriVal"></param>
        /// <param name="rtnVal"></param>
        /// <returns></returns>
        protected bool TextInput(string oriVal, out string rtnVal) {
            return InputText(out rtnVal, mTitle, mDescription, oriVal);
        }

        /// <summary>
        /// 以有限選項供使用者選擇
        /// </summary>
        /// <param name="oriVal"></param>
        /// <param name="rtnVal"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        protected bool ComboInput(string oriVal, out string rtnVal, IEnumerable<string> option) {
            return ComboBoxList(out rtnVal, mTitle, mDescription, option, oriVal);
        }

        /// <summary>
        /// Value、Default所使用的編輯方法，考慮Type欄位提供合適的編輯方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="oriVal"></param>
        /// <param name="rtnVal"></param>
        /// <returns></returns>
        protected bool ValueInput(string type, string oriVal, out string rtnVal) {
            /*-- 列舉型態值設定 --*/
            if (ContainType?.Invoke(type) ?? false) {
                IEnumerable<string> Types = GetItems?.Invoke(type);
                return ComboInput(oriVal, out rtnVal, Types);
                /*-- 布林型態值設定 --*/
            } else if (type == typeof(bool).Name) {
                return ComboInput(oriVal, out rtnVal, new string[] { "True", "False" });
                /*-- 通用型態值設定 --*/
            } else {
                return TextInput(oriVal, out rtnVal);
            }
        }

        /// <summary>
        /// 記錄空白欄位
        /// </summary>
        /// <param name="prop"></param>
        private void RecordIllegal(IParam prop, int idx) {
            if (RecordEmpty) {
                RecordIllegalField?.Invoke(prop, idx);
            }
        }

        private void RemoveIllegal(IParam prop, int idx) {
            if (RecordEmpty) {
                RemoveIllegalField?.Invoke(prop, idx);
            }
        }

        #endregion Function - Private Methods

    }

}
