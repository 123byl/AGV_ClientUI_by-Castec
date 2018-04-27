using CtBind;
using CtCommandPattern.cs;
using DataGridViewRichTextBox;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtParamEditor.Comm
{

    /// <summary>
    /// 參數編輯器
    /// </summary>
    public interface IParamEditor :IParamEdit,IDataSource,IUndoable{
        /// <summary>
        /// 被選取的欄索引
        /// </summary>
        string SelectedColumnName { get; set; }
        /// <summary>
        /// 被選取的列索引
        /// </summary>
        int SelectedRow { get; set; }
        Input.ComboBox ComboBoxList { get; set; }
        Input.Text InputText { get; set; }
        /// <summary>
        /// INI檔路徑
        /// </summary>
        string IniPath { get; }
        /// <summary>
        /// 參數集合
        /// </summary>
        IParamCollection ParamCollection { get; }
        /// <summary>
        /// 要顯示的選項
        /// </summary>
        CmsOption ShowOption { get; }
        /// <summary>
        /// 要鎖住的選項
        /// </summary>
        CmsOption DisableOption { get; }
        
        void CloseFilter();
        void Filter(string keyWord);
        void Highlight(string keyWord);
        void ReadINI(string fileName);
        void RestoreDefault();
        void SaveToINI(string path = null);
        /// <summary>
        /// 移除選定列
        /// </summary>
        void Remove();
        /// <summary>
        /// 移除指定列
        /// </summary>
        void Remove(int idxRow);
        /// <summary>
        /// 在選定列插入新列
        /// </summary>
        void Insert();
        /// <summary>
        /// 在指定列插入新列
        /// </summary>
        /// <param name="idxRow"></param>
        void Insert(int idxRow);
        /// <summary>
        /// 編輯使用者選定參數
        /// </summary>
        void Edit(string columnName);
        /// <summary>
        /// 將值寫入選定儲存格
        /// </summary>
        /// <param name="value"></param>
        void SetValue(string value);
    }

    public interface IParamEdit {
        IParamValue<T> WriteParam<T>(string name, T val, string description, T def);
        IParamValue<T> WriteParam<T>(string name, T val, string description);
    }

    /// <summary>
    /// 參數集合
    /// </summary>
    public interface IParamCollection:IDataSource {

        /// <summary>
        /// 對應列索引的資料列
        /// </summary>
        /// <param name="indxtRow"></param>
        /// <returns></returns>
        IParamColumn this[int indxtRow] { get; }
        /// <summary>
        /// 指定的資料
        /// </summary>
        /// <param name="indexRow"></param>
        /// <param name="indexColumn"></param>
        /// <returns></returns>
        object this[int indexRow, string columnName] { get; }
        /// <summary>
        /// 資料筆數
        /// </summary>
        int RowCount { get; }

        /// <summary>
        /// 資料變更事件
        /// </summary>
        event EventHandler DataChanged;
        /// <summary>
        /// 是否有符合條件的項目
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Any(Func<IParamColumn, bool> predicate = null);
        /// <summary>
        /// 對完整資料進行Foreach訪問
        /// </summary>
        /// <param name="act"></param>
        void Foreach(Action<IParam> act);
        /// <summary>
        /// 參數值讀取，使用IAgvToDgvCol型別進行讀取，可增加其他判斷
        /// </summary>
        /// <param name="name">要尋找的參數名稱</param>
        /// <param name="reader">參數讀取方法委派</param>
        /// <returns>是否有找到目標參數</returns>
        bool FindVal(string name, Action<IParamColumn> reader);
        /// <summary>
        /// 參數值讀取，用於寫入物件屬性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        bool FindVal<T>(string name, Action<T> reader) where T : IConvertible;
        /// <summary>
        /// 參數讀取，將參數直接寫入變數
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool FindVal<T>(string name, ref T val) where T : IConvertible;

    }

    /// <summary>
    /// AGV參數介面
    /// </summary>
    /// <remarks>
    /// 提供參數操作方法
    /// </remarks>
    public interface IParam : IParamColumn {
        bool SetValue(string val, string columnName);
        string GetParamValue(string columnName);
        /// <summary>
        /// 變數變更事件
        /// </summary>
        event EventHandler<string> ValueChanged;
        //Delegates.EnumData.DelContainItem ContainItem { get; set; }
        //Delegates.EnumData.DelContainType ContainType { get; set; }
        bool RangeDefinable { get; }
        Type GetParamType();

    }

    /// <summary>
    /// DGV欄位定義
    /// </summary>
    /// <remarks>
    /// 僅用於定義dgv控制項中的欄位名稱與順序
    /// </remarks>
    public interface IParamColumn:ICloneable {
        /// <summary>
        /// 欄位對應參數值
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        object this[string columnName] { get; }
        /// <summary>
        /// 參數名稱
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 參數類型
        /// </summary>
        string Type { get; }
        /// <summary>
        /// 參數值
        /// </summary>
        string Value { get; }
        /// <summary>
        /// 參數說明
        /// </summary>
        string Description { get; }
        /// <summary>
        /// 參數最大值
        /// </summary>
        string Max { get; }
        /// <summary>
        /// 參數最小值
        /// </summary>
        string Min { get; }
        /// <summary>
        /// 參數預設值
        /// </summary>
        string Default { get; }

        /// <summary>
        /// 非法欄位
        /// </summary>
        /// <returns></returns>
        EmColumn IlleaglColumn();
        /// <summary>
        /// 修改欄位
        /// </summary>
        /// <returns></returns>
        EmColumn ModifiedColumn();

        bool IsDefineMax();
        bool IsDefineMin();
        bool IsDefineDefault();
        /// <summary>
        /// 是否有欄位包含關鍵字
        /// </summary>
        /// <param name="keyWord"></param>
        bool FieldContains(string keyWord);
    }

    /// <summary>
    /// 參數值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IParamValue<T> {
        /// <summary>
        /// 參數值
        /// </summary>
        T Value { get; }
        /// <summary>
        /// 參數最大值
        /// </summary>
        T Max { get; }
        /// <summary>
        /// 參數最小值
        /// </summary>
        T Min { get; }
        /// <summary>
        /// 參數預設值
        /// </summary>
        T Def { get; }
        /// <summary>
        /// 是否有最大值定義
        /// </summary>
        bool IsDefineMax { get; }
        /// <summary>
        /// 是否有最小值定義
        /// </summary>
        bool IsDefineMin { get; }
        /// <summary>
        /// 是否有最小值定義
        /// </summary>
        bool IsDefineDefault { get; }
        /// <summary>
        /// 是否可以定義範圍
        /// </summary>
        bool RangeDefinable { get; }
        /// <summary>
        /// 設定參數最大值
        /// </summary>
        /// <param name="max"></param>
        void SetMaximun(T max);
        /// <summary>
        /// 設定參數最小值
        /// </summary>
        /// <param name="min"></param>
        void SetMinimun(T min);
        /// <summary>
        /// 設定預設值
        /// </summary>
        void SetDefault(T Def);
        /// <summary>
        /// 清除最小值
        /// </summary>
        void RemoveMaxinum();
        /// <summary>
        /// 清除最小值
        /// </summary>
        void RemoveMinimun();
        /// <summary>
        /// 設定參數範圍
        /// </summary>
        /// <param name="max"></param>
        /// <param name="min"></param>
        void SetRange(T max, T min);
    }

    /// <summary>
    /// 儲存格樣式管理器
    /// </summary>
    public interface ICellStyles {

        /// <summary>
        /// 一般樣式
        /// </summary>
        IExFont Regular { get; }
        /// <summary>
        /// 標記樣式
        /// </summary>
        IExFont Highlight { get; }
        /// <summary>
        /// 必填儲存格樣式
        /// </summary>
        IExFont RequiredCell { get; }
        /// <summary>
        /// 已編輯欄位樣式
        /// </summary>
        IExFont ModifiedRow { get;  }
        /// <summary>
        /// 已編輯儲存格樣式
        /// </summary>
        IExFont ModifiedCell { get; }
    }


}
