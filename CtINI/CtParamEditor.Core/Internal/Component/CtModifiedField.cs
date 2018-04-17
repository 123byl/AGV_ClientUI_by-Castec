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
        private Dictionary<IParamColumn, List<int>> mData = new Dictionary<IParamColumn, List<int>>();

        #endregion Declaration - Fields

        #region Function - Constructors

        public CtModifiedField() { }

        #endregion Function - Constructors

        #region Function - Public Methods

        /// <summary>
        /// 增加已修改欄位紀錄
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="idxCol"></param>
        public void Add(IParamColumn prop, int idxCol) {
            if (!mData.ContainsKey(prop)) mData.Add(prop, new List<int>());
            if (!mData[prop].Contains(idxCol)) mData[prop].Add(idxCol);
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
        /// <param name="idxCol"></param>
        /// <returns></returns>
        public bool ContainsColumn(IParam prop, int idxCol) {
            return mData[prop].Contains(idxCol);
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
