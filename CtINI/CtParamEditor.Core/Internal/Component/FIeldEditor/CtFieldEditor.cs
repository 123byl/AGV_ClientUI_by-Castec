using CtParamEditor.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtParamEditor.Core.Internal.Component.FIeldEditor {

    /// <summary>
    /// 欄位編輯器
    /// </summary>
    public class CtFieldEditor {

        #region Declaration Properties

        /// <summary>
        /// 移除非法欄位方法委派
        /// </summary>
        internal Delegates.IllegalField.DelRemoveIllegalField RemoveIllegalField { get; set; } = null;

        /// <summary>
        /// 紀錄非法欄位方法委派
        /// </summary>
        internal Delegates.IllegalField.DelRecordIllegalField RecordIllegalField { get; set; } = null;

        /// <summary>
        /// 紀錄修改欄位方法委派
        /// </summary>
        internal Delegates.ModifiedField.DelAddModifiedField AddModifiedField { get; set; } = null;

        /// <summary>
        /// 類型定義檢查方法委派
        /// </summary>
        internal Delegates.EnumData.DelContainType ContainType { get; set; } = null;

        /// <summary>
        /// 列舉清單取得方法委派
        /// </summary>
        internal Delegates.EnumData.DelGetItems GetItems { get; set; } = null;

        /// <summary>
        /// 列舉值定義檢查方法委派
        /// </summary>
        internal Delegates.EnumData.DelContainItem ContainItem { get; set; } = null;

        /// <summary>
        /// 取得所有列舉型態名稱方法委派
        /// </summary>
        internal Delegates.EnumData.DelGetTypes GetTypes { get; set; } = null;

        public Input.Text InputText { get; set; } = null;

        public Input.ComboBox ComboBoxList { get; set; } = null;

        #endregion Declaration - Properties

        #region Funciotn - Public Methods

        /// <summary>
        /// 顯示對話視窗供使用者輸入
        /// </summary>
        /// <param name="idx">欄位索引</param>
        /// <param name="prop">要修改的參數</param>
        /// <returns></returns>
        public bool Edit(int idx, IParam prop,out string returnValue) {
            /*-- 取得對應欄位編輯器 --*/
            BaseFieldEditor editor = GetEditor(idx);
            /*-- 指定委派方法 --*/
            editor.RemoveIllegalField = RemoveIllegalField;
            editor.RecordIllegalField = RecordIllegalField;
            editor.AddModifiedField = AddModifiedField;
            editor.InputText = InputText;
            editor.ComboBoxList = ComboBoxList;
            /*-- 顯示對話視窗供使用者輸入並回傳結果 --*/
            return editor.Edit(prop,out returnValue);
        }

        #endregion Funciton - Public Methods

        #region Funtion- Private Methods

        /// <summary>
        /// 依照欄位索引回傳對應的欄位編輯器
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private BaseFieldEditor GetEditor(int idx) {
            BaseFieldEditor editor = null;
            switch (idx) {
                case PropField.Idx.Name:
                    editor = new NameEditor();
                    break;
                case PropField.Idx.ValType:
                    editor = new ValTypeEditor();
                    editor.ContainType = ContainType;
                    editor.GetTypes = GetTypes;
                    break;
                case PropField.Idx.Description:
                    editor = new DescriptionEditor();
                    break;
                case PropField.Idx.Max:
                    editor = new MaxEditor();
                    break;
                case PropField.Idx.Min:
                    editor = new MinEditor();
                    break;
                case PropField.Idx.Default:
                    editor = new DefEditor();
                    editor.ContainType = ContainType;
                    editor.GetItems = GetItems;
                    break;
                case PropField.Idx.Value:
                    editor = new ValueEditor();
                    editor.ContainType = ContainType;
                    editor.GetItems = GetItems;
                    break;
                default:
                    throw new Exception("未定義欄位");
            }
            return editor;
        }

        #endregion Function - Private Methods
    }

}
