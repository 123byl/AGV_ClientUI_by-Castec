using CtINI;
using CtParamEditor.Comm;
using DataGridViewRichTextBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtParamEditor.Core.Internal.Component {
    /// <summary>
    /// DataGridView資料來源管理器
    /// </summary>
    internal class CtDgvDataSource : IParamCollection {

        #region Declaration - Fields

        /// <summary>
        /// 完整資料
        /// </summary>
        private List<IParamColumn> mFullData = new List<IParamColumn>();

        /// <summary>
        /// 過濾後的資料
        /// </summary>
        private List<IParamColumn> mFilter = new List<IParamColumn>();

        /// <summary>
        /// 是否已經過濾資料
        /// </summary>
        private bool mIsFilterMode = false;

        /// <summary>
        /// 過濾關鍵字
        /// </summary>
        private string mKeyWord = null;

        #endregion Declaration - Fields

        #region Declaration - Properties

        public IParamColumn this[int indexRow] {
            get {
                if (indexRow >=0 && indexRow < RowCount) {
                    return Data[indexRow];
                } else {
                    return null;
                }
            }
            internal set {
                Data[indexRow] = value;
                OnDataChanged();
            }
        }
        
        public object this[int indexRow, string columnName] {
            get {
                
                return Data[indexRow][columnName];
            }
        }
        
        /// <summary>
        /// 判斷Enum類型定義存在方法委派
        /// </summary>
        public Delegates.EnumData.DelContainType ContainType { get; set; } = null;

        /// <summary>
        /// 判斷列舉定義存在方法委派
        /// </summary>
        public Delegates.EnumData.DelContainItem ContainItem { get; set; } = null;
        
        /// <summary>
        /// Enum定義讀取方法委派
        /// </summary>
        public Delegates.EnumData.DelReadEnum ReadEnum { get; set; } = null;

        /// <summary>
        /// 是否在過濾模式
        /// </summary>
        public bool IsFilterMode {
            get => mIsFilterMode;
            set {
                if (mIsFilterMode != value) {
                    mIsFilterMode = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 過濾關鍵字
        /// </summary>
        public string KeyWord { get => mKeyWord;
            set {
                if (mKeyWord != value) {
                    mKeyWord = value;
                    IsFilterMode = false;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 要顯示的資料
        /// </summary>
        private List<IParamColumn> Data { get { return (IsFilterMode ? mFilter : mFullData) ; } }
        
        public bool IsModified {
            get {
                return mFullData.Any(prop => prop.ModifiedColumn != EmColumn.None);
            }
            set {
                if (!value) {
                    mFullData.ForEach(prop => prop.ModifiedColumn = EmColumn.None);
                    OnDataChanged();
                }
            }
        }

        #endregion Declaration - Properties

        #region Function -  Constructors

        internal CtDgvDataSource() { }

        #endregion Function - Construcotrs

        #region Implement - IParamCollection

        /// <summary>
        /// 資料變更事件
        /// </summary>
        public event EventHandler DataChanged;

        /// <summary>
        /// 尋找是否有目標參數名稱，使用參數讀取委派方法讀取參數
        /// </summary>
        /// <param name="name">要尋找的參數名稱</param>
        /// <param name="reader">參數讀取方法委派</param>
        /// <returns>是否有找到目標參數</returns>
        public bool FindVal(string name, Action<IParamColumn> reader) {
            int idx = Data.ToList().FindIndex(param => param.Name == name);
            bool isFound = idx >= 0;
            if (isFound) {
                reader(Data[idx]);
            }
            return isFound;
        }

        public bool FindVal<T>(string name, ref T val) where T : IConvertible {
            T tVal = default(T);
            bool isFund = findVal(name, out tVal);
            if (isFund) val = tVal;
            return isFund;
        }

        public bool FindVal<T>(string name, Action<T> reader) where T : IConvertible {
            T tVal = default(T);
            bool isFund = findVal(name, out tVal);
            if (isFund) reader(tVal);
            return isFund;
        }

        private bool findVal<T>(string name, out T val) where T : IConvertible {
            val = default(T);
            T tVal = default(T);
            bool isFund = FindVal(name, param => {
                string sVal = param.Value;
                if (tVal is Enum) {
                    tVal = (T)Enum.Parse(typeof(T), sVal);
                } else {
                    tVal = (T)Convert.ChangeType(sVal, typeof(T));
                }
            });
            if (isFund) val = tVal;
            return isFund;
        }
        
        /// <summary>
        /// 回傳是否有符合條件的項目
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Any(Func<IParamColumn,bool> predicate = null) {
            bool ret = false;
            if (predicate == null) {
                ret = mFullData.Any();
            } else {
                ret = mFullData.Any(predicate);
            }
            return ret;
        }

        #endregion Implement - IParamCollection

        #region Implement - IDataSource

        /// <summary>
        /// 屬性變更通知事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invoke方法委派
        /// </summary>
        public Action<MethodInvoker> DelInvoke { get; set; } = null;

        /// <summary>
        /// 屬性變更發報
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") {
            DelInvoke?.Invoke(() => {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
            OnDataChanged();
        }

        #endregion Implement - IDataSource

        #region Function - Public Methods

        /// <summary>
        /// 是否已有參數名稱定義
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name) {
            return mFullData.Any(param => param.Name == name);
        }

        /// <summary>
        /// 增加參數定義
        /// </summary>
        /// <param name="param"></param>
        public IParamValue<T> WriteParam<T>(string name, T val, string description, T def) {
            CtParam prop = new CtParam(Item_ValueChanged);
            if (val is Enum && !ContainType.Invoke(val.GetType().Name)) {
                ReadEnum.Invoke(val as Enum);
                prop.ContainType = ContainType;
                prop.ContainItem = ContainItem;
            }
            prop.SetName(name);
            prop.SetType(val.GetType().Name);
            prop.SetValue(val?.ToString());
            prop.SetDescription(description);
            prop.SetDefault(def?.ToString());
            //prop.SetMax(max?.ToString());
            //prop.SetMin(min?.ToString());
            mFullData.Add(prop);
            return prop.mVal as IParamValue<T>;
        }

        /// <summary>
        /// 依照關鍵字過濾參數項目
        /// </summary>
        /// <param name="keyWord"></param>
        public void Filter() {
            if (IsFilterMode) {
                mFilter.Clear();
            } else {
                mFilter = mFullData.FindAll(p => (p as IParam).FieldContains(KeyWord));
            }
            IsFilterMode = !IsFilterMode;
            OnPropertyChanged(nameof(RowCount));
        }
        
        /// <summary>
        /// 從INI資料集合中讀取資料
        /// </summary>
        /// <param name="inis"></param>
        /// <returns></returns>
        public void ReadINI(List<IniStruct> inis) {
            /*-- 讀取參數定義 --*/
            List<IParamColumn> props = inis.
                /*-- 篩選參數定義 --*/
                FindAll(ini => ini.ContainKey("Type")).
                /*-- 轉換為IAgvToDgvCol型態 --*/
                ConvertAll(v => GetDgvCol(v));
            /*-- 轉換成BindingList用於DataGridView進行資料繫結 --*/
            mFullData = new List<IParamColumn>(props);
            /*-- 更新要顯示的資料數目配合虛擬填充模式使用 --*/
            OnPropertyChanged(nameof(RowCount));;
        }

        /// <summary>
        /// 於指定索引插入新資料
        /// </summary>
        /// <param name="mIdxRow"></param>
        /// <param name="prop"></param>
        public void Insert(int mIdxRow,IParamColumn prop = null) {
            prop = prop ?? new CtParam(Item_ValueChanged);
            /*-- 插入新資料 --*/
            mFullData.Insert(mIdxRow, prop);
            /*-- 更新要顯示的資料筆數 --*/
            OnPropertyChanged(nameof(RowCount));
        }

        /// <summary>
        /// 移除第n列資料
        /// </summary>
        /// <param name="mIdxRow"></param>
        public void Remove(int mIdxRow) {
            IParamColumn prop = mFullData[mIdxRow];
            Remove(prop);
        }

        /// <summary>
        /// 移除指定資料
        /// </summary>
        /// <param name="prop"></param>
        public void Remove(IParamColumn prop) {
            if (mFullData.Contains(prop)) {
                /*-- 移除資料 --*/
                mFullData.Remove(prop);
                /*-- 更新要顯示的資料筆數 --*/
                OnPropertyChanged(nameof(RowCount));
            }
        }

        /// <summary>
        /// 將所有參數定義寫入<see cref="StringBuilder"/>
        /// </summary>
        /// <param name="iniContent"></param>
        public void WriteINI(StringBuilder iniContent) {
            foreach (IParam prop in mFullData) {
                iniContent.AppendLine($"[{prop.Name}]");
                iniContent.AppendLine($"Type={prop.Type}");
                iniContent.AppendLine($"Value={prop.Value}");
                iniContent.AppendLine($"Description={prop.Description}");
                if (prop.Max != null) iniContent.AppendLine($"Max={prop.Max}");
                if (prop.Min != null) iniContent.AppendLine($"Min={prop.Min}");
                if (prop.Default != null) iniContent.AppendLine($"Default={prop.Default}");
                iniContent.AppendLine();
            }
        }

        /// <summary>
        /// 清除所有資料
        /// </summary>
        public void Clear() {
            mFullData.Clear();
            mFilter.Clear();
            OnPropertyChanged(nameof(RowCount));
        }

        /// <summary>
        /// 對完整資料進行Foreach訪問
        /// </summary>
        /// <param name="act"></param>
        public void Foreach(Action<IParamColumn> act) {
            foreach (IParam prop in mFullData) {
                act(prop);
            }
        }

        /// <summary>
        /// 回傳當前顯示資料中RowIndex
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public int IndexOf(IParamColumn prop) {
            return Data.IndexOf(prop);
        }

        /// <summary>
        /// 回傳要顯示的資料筆數
        /// </summary>
        /// <returns></returns>
        public int RowCount {
            get { return Data?.Count() ?? 0; }
        }

        #endregion Function - Public Mehtods

        #region Function - Private Methods

        /// <summary>
        /// 將<see cref="IniStruct"/>轉換為<see cref="IParamColumn"/>型態
        /// </summary>
        /// <param name="ini"></param>
        /// <returns></returns>
        private IParamColumn GetDgvCol(IniStruct ini) {
            CtParam item = new CtParam(Item_ValueChanged) {
                ContainType = ContainType,
                ContainItem = ContainItem
            };
            if (item.ReadIni(ini)) {
                return item;
            } else {
                return null;
            }
        }

        private void Item_ValueChanged(object sender, string e) {            
            OnDataChanged();
        }

        /// <summary>
        /// 資料變更發報
        /// </summary>
        protected virtual void OnDataChanged() {
            DataChanged?.Invoke(this, new EventArgs());
        }

        #endregion Function - Private Methods

    }

}
