using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtParamEditor.Comm {
    
    /// <summary>
    /// 欄位列舉
    /// </summary>
    public enum EmColumn :int{
        /// <summary>
        /// 無
        /// </summary>
        None = 0,
        /// <summary>
        /// 參數名稱
        /// </summary>
        Name = 1,
        /// <summary>
        /// 參數說明
        /// </summary>
        Description = 2,
        /// <summary>
        /// 參數類型
        /// </summary>
        Type = 4,
        /// <summary>
        /// 參數值
        /// </summary>
        Value = 8,
        /// <summary>
        /// 參數最大值
        /// </summary>
        Max = 0x10,
        /// <summary>
        /// 參數最小值
        /// </summary>
        Min = 0x20,
        /// <summary>
        /// 參數預設值
        /// </summary>
        Default = 0x40,
    }

    /// <summary>
    /// 右鍵選單列舉
    /// </summary>
    public enum CmsOption {
        None = 0,
        Add = 1,
        Edit = 2,
        Delete = 4
    }

    /// <summary>
    /// 輸入方法委派定義
    /// </summary>
    public static class Input {
        /// <summary>
        /// 文字方塊輸入方法委派
        /// </summary>
        /// <param name="result"></param>
        /// <param name="title"></param>
        /// <param name="describe"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public delegate bool Text(out string result, string title, string describe, string defValue = "");
        /// <summary>
        /// 下拉選單輸入方法委派
        /// </summary>
        /// <param name="result"></param>
        /// <param name="title"></param>
        /// <param name="describe"></param>
        /// <param name="itemList"></param>
        /// <param name="defValue"></param>
        /// <param name="allowEdit"></param>
        /// <returns></returns>
        public delegate bool ComboBox(out string result, string title, string describe, IEnumerable<string> itemList, string defValue = "", bool allowEdit = false);
    }

}
