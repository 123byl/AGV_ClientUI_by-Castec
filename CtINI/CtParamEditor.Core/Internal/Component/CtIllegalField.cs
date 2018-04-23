using CtParamEditor.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtParamEditor.Core.Internal.Component {

    /// <summary>
    /// 非法欄位記錄管理器
    /// </summary>
    internal class CtIllegalField {

        #region Declaration - Fields

        /// <summary>
        /// 非法欄位紀錄
        /// </summary>
        private Dictionary<IParamColumn, List<string>> Data { get; set; } = new Dictionary<IParamColumn, List<string>>();

        #endregion Declaration - Fields

        #region Declaration - Properties

        public KeyValuePair<IParamColumn, List<string>> this[int idx] {
            get {
                return Data.ElementAt(idx);
            }
        }

        #endregion Declaration - Properties

        #region Function - Constructors

        public CtIllegalField() { }

        #endregion Function - Constructors

        #region Funciton - Public Methods

        /// <summary>
        /// 檢查是否為非法欄位
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public bool Contains(IParamColumn prop, string columnName) {
            return Data.ContainsKey(prop) && Data[prop].Contains(columnName);
        }

        /// <summary>
        /// 增加非法欄位紀錄
        /// </summary>
        /// <param name="prop"></param>
        public void Add(IParamColumn prop) {
            /*-- 是否以有紀錄 --*/
            if (!Data.ContainsKey(prop)) Data.Add(prop, new List<string>());
            /*-- 是否已有名稱欄位紀錄 --*/
            if (!Data[prop].Contains(nameof(IParamColumn.Name))) Data[prop].Add(nameof(IParamColumn.Name));
            /*-- 是否已有資料型態欄位紀錄 --*/
            if (!Data[prop].Contains(nameof(IParamColumn.Type))) Data[prop].Add(nameof(IParamColumn.Type));
        }

        /// <summary>
        /// 移除非法欄位紀錄
        /// </summary>
        /// <param name="prop"></param>
        public void Remove(IParamColumn prop) {
            if (Data.ContainsKey(prop)) {
                Data.Remove(prop);
            }
        }

        /// <summary>
        /// 增加非法欄位紀錄
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="columnName"></param>
        public void Add(IParamColumn prop, string columnName) {
            /*-- 是否以有紀錄 --*/
            if (!Data.ContainsKey(prop)) Data.Add(prop, new List<string>());
            /*-- 是否已有對應欄位紀錄 --*/
            if (!Data[prop].Contains(columnName)) Data[prop].Add(columnName);
        }

        /// <summary>
        /// 移除非法欄位紀錄
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="columnName"></param>
        public void Remove(IParamColumn prop, string columnName) {
            if (Data.ContainsKey(prop) && Data[prop].Contains(columnName)) {
                Data[prop].Remove(columnName);
                if (Data[prop].Count() == 0) {
                    Data.Remove(prop);
                }
            }
        }

        /// <summary>
        /// 非法欄位資料筆數
        /// </summary>
        /// <returns></returns>
        public int Count() {
            return Data.Count();
        }

        /// <summary>
        /// 清除非法欄位資料
        /// </summary>
        public void Clear() {
            Data.Clear();
        }

        #endregion Function - Public Methods
    }


}
