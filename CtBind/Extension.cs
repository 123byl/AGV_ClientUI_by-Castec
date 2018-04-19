using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtBind {
    public static class Extension {

        /// <summary>
        /// 資料綁定
        /// </summary>
        /// <param name="coll">綁定集合</param>
        /// <param name="propertyName">屬性名稱</param>
        /// <param name="dataSource">資料來源</param>
        /// <param name="dataMember">成員名稱</param>
        /// <param name="handle">格式處理方法</param>
        /// <returns></returns>
        /// <remarks>
        /// 由於IBindableComponent在綁定遇到需要格式轉換時，預設讀出的是轉換前的變數形態
        /// 因此用該方法，在指定格式處理方法後立刻讀取綁定變數
        /// </remarks>
        public static Binding ExAdd(
            this ControlBindingsCollection coll,
            string propertyName,
            object dataSource,
            string dataMember,
            ConvertEventHandler handle = null,
            object nullValue = null) {
            var bind = coll.Add(propertyName, dataSource, dataMember, true, DataSourceUpdateMode.OnPropertyChanged, nullValue);
            if (handle != null) {
                bind.Format += handle;
                bind.ReadValue();
            }
            return bind;
        }

    }
}
