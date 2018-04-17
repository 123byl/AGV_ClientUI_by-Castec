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
        private Dictionary<IParamColumn, List<int>> Data { get; set; } = new Dictionary<IParamColumn, List<int>>();

        #endregion Declaration - Fields

        #region Declaration - Properties

        public KeyValuePair<IParamColumn, List<int>> this[int idx] {
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
        /// <param name="idxCol"></param>
        /// <returns></returns>
        public bool Contains(IParamColumn prop, int idxCol) {
            return Data.ContainsKey(prop) && Data[prop].Contains(idxCol);
        }

        /// <summary>
        /// 增加非法欄位紀錄
        /// </summary>
        /// <param name="prop"></param>
        public void Add(IParamColumn prop) {
            /*-- 是否以有紀錄 --*/
            if (!Data.ContainsKey(prop)) Data.Add(prop, new List<int>());
            /*-- 是否已有名稱欄位紀錄 --*/
            if (!Data[prop].Contains(PropField.Idx.Name)) Data[prop].Add(PropField.Idx.Name);
            /*-- 是否已有資料型態欄位紀錄 --*/
            if (!Data[prop].Contains(PropField.Idx.ValType)) Data[prop].Add(PropField.Idx.ValType);
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
        /// <param name="idx"></param>
        public void Add(IParamColumn prop, int idx) {
            /*-- 是否以有紀錄 --*/
            if (!Data.ContainsKey(prop)) Data.Add(prop, new List<int>());
            /*-- 是否已有對應欄位紀錄 --*/
            if (!Data[prop].Contains(idx)) Data[prop].Add(idx);
        }

        /// <summary>
        /// 移除非法欄位紀錄
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="idx"></param>
        public void Remove(IParamColumn prop, int idx) {
            if (Data.ContainsKey(prop) && Data[prop].Contains(idx)) {
                Data[prop].Remove(idx);
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
