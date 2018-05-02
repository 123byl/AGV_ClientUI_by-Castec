using CtCommandPattern.cs;
using CtINI;
using CtParamEditor.Comm;
using CtParamEditor.Core.Internal;
using CtParamEditor.Core.Internal.Component;
using CtParamEditor.Core.Internal.Component.FIeldEditor;
using DataGridViewRichTextBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtParamEditor.Core
{
    
    /// <summary>
    /// 參數編輯器
    /// </summary>
    internal class ParamEditor: IParamEditor {

        #region Declaration - Fields
        
        /// <summary>
        /// INI檔案路徑
        /// </summary>
        private string mIniPath = null;
        
        /// <summary>
        /// 被點選的DataGridCell列數
        /// </summary>
        private int mSelectedRow = -1;

        /// <summary>
        /// 被點選的DataGridCell行數
        /// </summary>
        private string mSelectedColumnName;
        
        private Action<MethodInvoker> mDelinvoke = null;

        /// <summary>
        /// 要顯示的選項
        /// </summary>
        private CmsOption mShowOption = CmsOption.None;

        /// <summary>
        /// 要鎖住的選項
        /// </summary>
        private CmsOption mDisableOption = CmsOption.None;
        
        #endregion Declaration - Fields

        #region Declaration - Properties

        /// <summary>
        /// 被選取的行索引
        /// </summary>
        public string SelectedColumnName {
            get => mSelectedColumnName;
            set {
                if (mSelectedColumnName != value) {
                    mSelectedColumnName = value;
                    DecidedShow(SelectedColumnName, SelectedRow);
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 被選取的列索引
        /// </summary>
        public int SelectedRow {
            get => mSelectedRow;
            set {
                if (mSelectedRow != value) {
                    mSelectedRow = value;
                    DecidedShow(SelectedColumnName, SelectedRow);
                    OnPropertyChanged();
                }
            }
        }
        
        public IParamCollection ParamCollection { get { return DataSource; } }

        /// <summary>
        /// 文字輸入視窗方法委派
        /// </summary>
        public Input.Text InputText { get; set; }

        /// <summary>
        /// 下拉選單輸入視窗方法委派
        /// </summary>
        public Input.ComboBox ComboBoxList { get; set; }

        /// <summary>
        /// 列舉定義管理器
        /// </summary>
        public CtEnumData EnumData = new CtEnumData();

        /// <summary>
        /// 參數資料
        /// </summary>
        public CtDgvDataSource DataSource  = new CtDgvDataSource();
        
        /// <summary>
        /// 要顯示的選項
        /// </summary>
        public CmsOption ShowOption {
            get => mShowOption;
            set {
                if (mShowOption != value) {
                    mShowOption = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 要鎖住的選項
        /// </summary>
        public CmsOption DisableOption {
            get => mDisableOption;
            set {
                if (mDisableOption != value) {
                    mDisableOption = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// INI檔路徑
        /// </summary>
        public string IniPath { get => mIniPath;
            set {
                if (mIniPath != value) {
                    mIniPath = value;
                    OnPropertyChanged();
                }
            }
        }
        
        #endregion Declaration - Properteis

        #region Declaration - Enum

        

        #endregion Declaration - Enum
        
        #region Function - Constructors

        internal ParamEditor() {

            /*-- 參考分配 --*/
            ExtensionCommand.RefEditor = this;

            /*-- 命令管理器屬性變更事件訂閱 --*/
            mCommandManager.PropertyChanged += (sender, e) => {
                OnPropertyChanged(e.PropertyName);
            };

            /*-- 分配委派 --*/
            AssignDelegation();
            

        }

        #endregion Funciton - Constructors

        #region Function - Public Methods

        /// <summary>
        /// 標記關鍵字
        /// </summary>
        /// <param name="keyWord"></param>
        public void Highlight(string keyWord) {

        }

        /// <summary>
        /// 將含有
        /// </summary>
        /// <param name="keyWord"></param>
        public void Filter() {
            DataSource.Filter();
        }

        public void CloseFilter() {
            DataSource.CloseFilter();
        }

        /// <summary>
        /// 讀取INI檔
        /// </summary>
        /// <param name="fileName"></param>
        public void ReadINI(string fileName) {
            /*-- 讀取INI資料 --*/
            List<IniStruct> inis = CtINI.CtINI.ReadValues(fileName);
            /*-- 讀取Enum定義 --*/
            EnumData.ReadINI(inis);
            /*-- 讀取參數定義 --*/
            DataSource.ReadINI(inis);
            /*-- 記錄INI檔路徑 --*/
            IniPath = fileName;
        }

        /// <summary>
        /// 在指定列數插入新參數
        /// </summary>
        /// <param name="idxRow"></param>
        public void Insert(int idxRow) {
            idxRow = idxRow != -1 ? idxRow : DataSource.RowCount;
            mCommandManager.Insert(idxRow);
        }

        /// <summary>
        /// 在選定列插入新行
        /// </summary>
        public void Insert() {
            Insert(SelectedRow);
        }

        /// <summary>
        /// 移除指定列數的參數
        /// </summary>
        /// <param name="idxRow"></param>
        public void Remove(int idxRow) {
            mCommandManager.Remove(idxRow);
        }

        /// <summary>
        /// 移除選定列
        /// </summary>
        public void Remove(){
            Remove(SelectedRow);
        }

        /// <summary>
        /// 要編輯的欄位
        /// </summary>
        /// <param name="columnName"></param>
        public void Edit(string columnName) {
            /*-- 取得要編輯的資料列 --*/
            IParam prop = DataSource[mSelectedRow] as IParam;

            /*-- 取得對應欄位編輯器 --*/
            BaseFieldEditor editor = GetEditor(columnName);

            /*-- 使用者輸入 --*/
            if (editor.Edit(prop,out string newValue)) {
                /*-- 寫入參數 --*/
                mCommandManager.Edit(newValue, SelectedRow, columnName);
            }

        }

        /// <summary>
        /// 將參數寫入INI檔
        /// </summary>
        public void SaveToINI(string path = null) {
            bool isIlleagl = DataSource.Any(prop =>  (prop as IParam).IlleaglColumn() != EmColumn.None);
            if (isIlleagl) {
                throw new Exception("有非法欄位導致無法儲存");
            } else {
                string savePath = string.IsNullOrEmpty(path) ? IniPath : path;
                StringBuilder iniContent = new StringBuilder();
                ///Note: Section定義不可相同
                ///     WriteINI方法並不會檢查已寫入的Section名稱
                ///     若是有重複Section名稱可能在讀取的時候會有問題
                ///     因此要自行確保沒有相同的Section名稱
                /*-- 寫入Enum類型定義 --*/
                EnumData.WriteINI(iniContent);
                /*-- 寫入參數定義 --*/
                DataSource.WriteINI(iniContent);

                /*-- 寫入INI檔 --*/
                File.WriteAllText(savePath, iniContent.ToString(), Encoding.UTF8);
                IniPath = path;
                mCommandManager.Clear();
                DataSource.IsModified = false;
                
            }
        }
        
        /// <summary>
        /// 清除所有資料
        /// </summary>
        public void Clear() {
            DataSource.Clear();
            EnumData.Data.Clear();
            mCommandManager.Clear();
            IniPath = string.Empty;
        }

        /// <summary>
        /// 恢復預設值
        /// </summary>
        public void RestoreDefault() {
            DataSource.Foreach(prop => {
                if (prop.Default != null) {
                    prop.SetValue(prop.Default.ToString(), nameof(IParamColumn.Value));
                }
            });
        }

        /// <summary>
        /// 將值寫入選定的儲存格
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(string value) {
            //IParamColumn prop = DataSource[SelectedRow];
            //ModifiedField.Add(prop, SelectedColumn);
            //AddModifiedField.Invoke(prop, mIdx);
            //if (string.IsNullOrEmpty(returnValue)) {
            //    /*-- 記錄非法的欄位 --*/
            //    RecordIllegal(prop, mIdx);
            //} else {
            //    /*-- 移除非法紀錄 --*/
            //    RemoveIllegal(prop, mIdx);
            //}
        }

        //public void WriteParam<T>(string name, T val, string description, T def) {
        //    if (DataSource.Contains(name)) throw new ArgumentException($"已有{name}參數名稱定義");

        //    DataSource.WriteParam(name, val, description, def);
        //}
        
        #endregion Function - Public Methods
            
        #region Function-  Private Methods
            
        /// <summary>
        /// 將參數設定寫入<see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="data"></param>
        private void WriteProperty(StringBuilder sb, IParam data) {
            sb.AppendLine($"[{data.Name}]");
            sb.AppendLine($"Type={data.Type}");
            sb.AppendLine($"Value={data.Value}");
            sb.AppendLine($"Description={data.Description}");
            if (data.Max != null) sb.AppendLine($"Max={data.Max}");
            if (data.Min != null) sb.AppendLine($"Min={data.Min}");
            if (data.Default != null) sb.AppendLine($"Default={data.Default}");
            sb.AppendLine();
        }
        
        /// <summary>
        /// 分配委派
        /// </summary>
        private void AssignDelegation() {
            
            DataSource.ContainType = EnumData.ContainType;

            DataSource.ContainItem = EnumData.ContainItem;
            
            DataSource.ReadEnum = EnumData.ReadEnum;
            
        }
        
        /// <summary>
        /// 決策是否顯示&鎖住右鍵選單
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="idxRow"></param>
        private void DecidedShow(string columnName,int idxRow) {
            ShowOption = CmsOption.None;
            DisableOption = CmsOption.None;
            if (idxRow == -1) {
                ShowOption = CmsOption.Add;
            } else {
                switch (columnName) {
                    case nameof(IParamColumn.Name):
                    case nameof(IParamColumn.Description):
                    case nameof(IParamColumn.Type):
                        ShowOption = CmsOption.Edit;
                        break;
                    case nameof(IParamColumn.Value):
                    case nameof(IParamColumn.Max):
                    case nameof(IParamColumn.Min):
                    case nameof(IParamColumn.Default):
                        ShowOption = CmsOption.Edit;
                        IParam prop = DataSource[idxRow] as IParam;
                        if (string.IsNullOrEmpty(prop.Type)) {
                            DisableOption = CmsOption.Edit;
                        } else {
                            if (columnName == nameof(IParamColumn.Max) || columnName == nameof(IParamColumn.Min)) {
                                if (!prop.RangeDefinable) {
                                    DisableOption = CmsOption.Edit;
                                }
                            }
                        }
                        break;
                    default:
                        ShowOption = CmsOption.Add | CmsOption.Delete;
                        break;
                }
            }
        }

        /// <summary>
        /// 依照欄位索引回傳對應的欄位編輯器
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private BaseFieldEditor GetEditor(string columnName) {
            BaseFieldEditor editor = null;
            switch (columnName) {
                case nameof(IParamColumn.Name):
                    editor = new NameEditor();
                    break;
                case nameof(IParamColumn.Type):
                    editor = new ValTypeEditor() {
                        ContainType = EnumData.ContainType,
                        GetTypes = EnumData.GetTypes
                    };
                    break;
                case nameof(IParamColumn.Description):
                    editor = new DescriptionEditor();
                    break;
                case nameof(IParamColumn.Max):
                    editor = new MaxEditor();
                    break;
                case nameof(IParamColumn.Min):
                    editor = new MinEditor();
                    break;
                case nameof(IParamColumn.Default):
                    editor = new DefEditor() {
                        ContainType = EnumData.ContainType,
                        GetItems = EnumData.GetItems
                    };
                    break;
                case nameof(IParamColumn.Value):
                    editor = new ValueEditor() {
                        ContainType = EnumData.ContainType,
                        GetItems = EnumData.GetItems
                    };
                    break;
                default:
                    throw new Exception("未定義欄位");
            }
            editor.ComboBoxList = ComboBoxList;
            editor.InputText = InputText;
            return editor;
        }


        /// <summary>
        /// 屬性變更發報
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") {
            DelInvoke?.Invoke(() => PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName)));
        }

        public IParamValue<T> WriteParam<T>(string name, T val, string description) {
            return WriteParam<T>(name, val, description, default(T));
        }

        public IParamValue<T> WriteParam<T>(string name, T val, string description, T def) {
            if (DataSource.Contains(name)) throw new ArgumentException($"已有{name}參數名稱定義");
            return DataSource.WriteParam(name, val, description, def);
        }

        #endregion Funciton - Private Methods

        #region Implement - IDataSource

        /// <summary>
        /// Invoke方法委派
        /// </summary>
        public Action<MethodInvoker> DelInvoke {
            get => mDelinvoke; set {
                if (mDelinvoke != value && value != null) {
                    mDelinvoke = value;
                }
            }
        }

        /// <summary>
        /// 屬性變更事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        #endregion Implement - IDataSource

        #region Implement - IUndoable

        /// <summary>
        /// 命令管理器
        /// </summary>
        private CommandManager mCommandManager = new CommandManager();

        /// <summary>
        /// 可撤銷次數
        /// </summary>
        public int UndoLimit { get => mCommandManager.UndoLimit; set => mCommandManager.UndoLimit = value; }

        /// <summary>
        /// 可撤銷次數
        /// </summary>
        public int UndoCount => mCommandManager.UndoCount;

        /// <summary>
        /// 可重做次數
        /// </summary>
        public int RedoCount => mCommandManager.RedoCount;

        /// <summary>
        /// 撤銷
        /// </summary>
        public void Undo() {
            mCommandManager.Undo();
        }

        /// <summary>
        /// 重做
        /// </summary>
        public void Redo() {
            mCommandManager.Redo();
        }        
        #endregion Implement - IUndoable

    }

    /// <summary>
    /// 擴充命令
    /// </summary>
    internal static class ExtensionCommand {

        /// <summary>
        /// 編輯器物件參考
        /// </summary>
        public static ParamEditor RefEditor = null;
        
        /// <summary>
        /// 插入資料列
        /// </summary>
        /// <param name="cmdManager"></param>
        /// <param name="idxRow"></param>
        public static void Insert(this CommandManager cmdManager, int idxRow) {
            cmdManager.ExecutCmd(new CmdInsert(RefEditor, idxRow));
        }

        /// <summary>
        /// 移除資料列
        /// </summary>
        /// <param name="cmdManager"></param>
        /// <param name="idxRow"></param>
        public static void Remove(this CommandManager cmdManager,int idxRow) {
            cmdManager.ExecutCmd(new CmdRemove(RefEditor, idxRow));
        }

        /// <summary>
        /// 編輯資料
        /// </summary>
        /// <param name="cmdManager"></param>
        /// <param name="value"></param>
        /// <param name="idxRow"></param>
        /// <param name="columnName"></param>
        public static void Edit(this CommandManager cmdManager,string value,int idxRow,string columnName) {
            cmdManager.ExecutCmd(new CmdEdit(RefEditor,value, idxRow, columnName));
        }
    }

    /// <summary>
    /// 插入新資料列
    /// </summary>
    internal class CmdInsert : BaseCommand<ParamEditor> {

        private int mIdxRow;

        public CmdInsert(ParamEditor receiver,int idxRow) : base(receiver) {
            mIdxRow = idxRow;
        }

        public override bool Execute() {
            mReceiver.DataSource.Insert(mIdxRow);
            return true;
        }

        public override void Undo() {
            mReceiver.DataSource.Remove(mIdxRow);
        }
    }

    /// <summary>
    /// 移除資料
    /// </summary>
    internal class CmdRemove : BaseCommand<ParamEditor> {
        private IParamColumn rParam = null;
        private int mIdxRow = -1;
        public CmdRemove(ParamEditor receiver,int idxRow) : base(receiver) {
            rParam = mReceiver.DataSource[idxRow];
            mIdxRow = idxRow;
        }

        public override bool Execute() {
            mReceiver.DataSource.Remove(mIdxRow);
            return true;
        }

        public override void Undo() {
            mReceiver.DataSource.Insert(mIdxRow, rParam);
        }
    }

    /// <summary>
    /// 編輯資料
    /// </summary>
    internal class CmdEdit : BaseCommand<ParamEditor> {

        /// <summary>
        /// 編輯目標列索引
        /// </summary>
        private int mIdxRow;

        /// <summary>
        /// 設定值
        /// </summary>
        private string mValue;

        /// <summary>
        /// 編輯欄位名稱
        /// </summary>
        private string mColumnName;

        /// <summary>
        /// 原始參數備份
        /// </summary>
        private IParam mParamClone;

        public CmdEdit(ParamEditor receiver,string value,int idxRow,string columnName) : base(receiver) {
            mParamClone = mReceiver.DataSource[idxRow].Clone() as IParam;
            mIdxRow = idxRow;
            mValue = value;
            mColumnName = columnName;
        }

        public override bool Execute() {
            /*-- 取得目標參數 --*/
            var param = mReceiver.DataSource[mIdxRow] as IParam;
            /*-- 寫入設定值 --*/
            bool success =  param.SetValue(mValue, mColumnName);
            return success;
        }

        public override void Undo() {
            /*-- 將參數備份副本覆蓋回修改後的資料 --*/
            mReceiver.DataSource[mIdxRow] = mParamClone.Clone() as IParamColumn;
        }

    }

}
