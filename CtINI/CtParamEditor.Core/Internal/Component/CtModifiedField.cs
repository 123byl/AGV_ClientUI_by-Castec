using CtParamEditor.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtParamEditor.Core.Internal.Component {

    /// <summary>
    /// 已修改欄位紀錄管理器
    /// </summary>
    internal class CtModifiedField {

        #region Declaration - Fields

        /// <summary>
        /// 已修改欄位紀錄
        /// </summary>
        private Dictionary<IParamColumn, List<string>> mData = new Dictionary<IParamColumn, List<string>>();

        #endregion Declaration - Fields

        #region Function - Constructors

        public CtModifiedField() { }

        #endregion Function - Constructors

        #region Function - Public Methods

        /// <summary>
        /// 增加已修改欄位紀錄
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="columnName"></param>
        public void Add(IParamColumn prop, string columnName) {
            if (!mData.ContainsKey(prop)) mData.Add(prop, new List<string>());
            if (!mData[prop].Contains(columnName)) mData[prop].Add(columnName);
        }
        
        /// <summary>
        /// 已修改欄位是否包含該筆資料
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public bool ContainsRow(IParam prop) {
            return mData.ContainsKey(prop);
        }

        /// <summary>
        /// 已修改欄位資料是否該儲存格
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public bool ContainsColumn(IParam prop, string columnName) {
            return mData[prop].Contains(columnName);
        }

        /// <summary>
        /// 清除所有資料
        /// </summary>
        public void Clear() {
            mData.Clear();
        }

        #endregion Function - Public Methods
    }

}
