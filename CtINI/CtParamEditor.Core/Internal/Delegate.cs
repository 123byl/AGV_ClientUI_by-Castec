using CtParamEditor.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtParamEditor.Core.Internal {

    /// <summary>
    /// 委派定義
    /// </summary>
    public static class Delegates {

        /// <summary>
        /// 非法欄位紀錄委派方法
        /// </summary>
        public static class IllegalField {

            /// <summary>
            /// 增加非法欄位紀錄
            /// </summary>
            /// <param name="prop"></param>
            public delegate void DelAddIllegal(IParamColumn prop);

            /// <summary>
            /// 移除非法欄位紀錄
            /// </summary>
            /// <param name="prop"></param>
            public delegate void DelRemoveIllegalRecord(IParamColumn prop);

            /// <summary>
            /// 移除非法欄位紀錄
            /// </summary>
            /// <param name="prop"></param>
            /// <param name="idx"></param>
            public delegate void DelRemoveIllegalField(IParamColumn prop, int idx);

            /// <summary>
            /// 紀錄非法欄位
            /// </summary>
            /// <param name="prop"></param>
            /// <param name="idx"></param>
            public delegate void DelRecordIllegalField(IParamColumn prop, int idx);

        }

        /// <summary>
        /// 已修改欄位紀錄委派方法
        /// </summary>
        public static class ModifiedField {

            /// <summary>
            /// 紀錄已修改的欄位
            /// </summary>
            /// <param name="prop"></param>
            /// <param name="idx"></param>
            public delegate void DelAddModifiedField(IParamColumn prop, int idx);

        }

        /// <summary>
        /// Enum定義管理委派方法
        /// </summary>
        public static class EnumData {

            /// <summary>
            /// 取得Enum定義的所有Item
            /// </summary>
            /// <returns></returns>
            public delegate IEnumerable<string> DelGetItems(string type);

            /// <summary>
            /// 是否有包含Enum類型定義
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public delegate bool DelContainType(string type);

            /// <summary>
            /// 是否有Item定義
            /// </summary>
            /// <param name="type"></param>
            /// <param name="item"></param>
            /// <returns></returns>
            public delegate bool DelContainItem(string type, string item);

            /// <summary>
            /// 取得所有列舉型態名稱
            /// </summary>
            /// <returns></returns>
            public delegate IEnumerable<string> DelGetTypes();

            /// <summary>
            /// 讀取Enum定義
            /// </summary>
            /// <param name="em"></param>
            public delegate void DelReadEnum(Enum em);

        }

        public static class Dgv {

            /// <summary>
            /// 更新顯示的資料筆數
            /// </summary>
            /// <param name="rowCount"></param>
            public delegate void DelUpdateRowCount(int rowCount);

        }

    }

}
