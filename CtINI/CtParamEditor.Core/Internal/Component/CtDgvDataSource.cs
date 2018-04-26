﻿using CtINI;
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
        private BindingList<IParamColumn> mFullData = new BindingList<IParamColumn>();

        /// <summary>
        /// 過濾後的資料
        /// </summary>
        private BindingList<IParamColumn> mFilter = new BindingList<IParamColumn>();

        CtModifiedField mModified = new CtModifiedField();

        CtIllegalField mIllegal = new CtIllegalField();
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
        /// RTF格式轉換器
        /// </summary>
        public RtfConvert RtfConfigurator { get; } = new RtfConvert();

        /// <summary>
        /// 非法欄位增加方法委派
        /// </summary>
        public Delegates.IllegalField.DelAddIllegal AddIllegal { get; set; } = null;

        /// <summary>
        /// 移除非法欄位記錄方法委派
        /// </summary>
        public Delegates.IllegalField.DelRemoveIllegalRecord RemoveIllegalRecord { get; set; } = null;

        /// <summary>
        /// 判斷Enum類型定義存在方法委派
        /// </summary>
        public Delegates.EnumData.DelContainType ContainType { get; set; } = null;

        /// <summary>
        /// 判斷列舉定義存在方法委派
        /// </summary>
        public Delegates.EnumData.DelContainItem ContainItem { get; set; } = null;

        /// <summary>
        /// 顯示資料筆數更新方法委派
        /// </summary>
        //public Delegates.Dgv.DelUpdateRowCount UpdateRowCount { get; set; } = null;

        /// <summary>
        /// Enum定義讀取方法委派
        /// </summary>
        public Delegates.EnumData.DelReadEnum ReadEnum { get; set; } = null;

        /// <summary>
        /// 是否在過濾模式
        /// </summary>
        public bool IsFilterMode { get; set; } = false;

        /// <summary>
        /// 要顯示的資料
        /// </summary>
        private BindingList<IParamColumn> Data { get { return IsFilterMode ? mFilter : mFullData; } }

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

        public bool IsIlleagl(IParam prop, string columnName) {
            return mIllegal.Contains(prop, columnName);
        }

        public bool IsModified(IParam prop) {
            return mModified.ContainsRow(prop);
        }

        public bool IsModified(IParam prop, string columnName) {
            return mModified.ContainsColumn(prop, columnName);
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
        public void Filter(string keyWord) {
            throw new NotImplementedException();
            //mFilter.Clear();
            //foreach (IParam prop in mFullData) {
            //    int idx = 0;
            //    do {
            //        string val = prop.GetParamValue(idx++);
            //        if (!string.IsNullOrEmpty(val) && val.Contains(keyWord)) {
            //            mFilter.Add(prop);
            //            break;
            //        }
            //    } while (idx <= PropField.Idx.Default);
            //}
            //IsFilterMode = true;
            //UpdateRowCount?.Invoke(RowCount());
        }

        /// <summary>
        /// 關閉參數過濾
        /// </summary>
        public void CloseFilter() {
            IsFilterMode = false;
            mFilter.Clear();
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
            mFullData = new BindingList<IParamColumn>(props);
            /*-- 更新要顯示的資料數目配合虛擬填充模式使用 --*/
            OnPropertyChanged(nameof(RowCount));
            //UpdateRowCount?.Invoke(RowCount());
        }

        /// <summary>
        /// 於指定索引插入新資料
        /// </summary>
        /// <param name="mIdxRow"></param>
        /// <param name="prop"></param>
        public void Insert(int mIdxRow,IParamColumn prop = null) {
            prop = prop ?? new CtParam(Item_ValueChanged);
            /*-- 紀錄非法欄位 --*/
            mIllegal.Add(prop);
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
                /*-- 移除非法紀錄 --*/
                RemoveIllegalRecord?.Invoke(prop);
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
        public void Foreach(Action<IParam> act) {
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
            IParamColumn prop = sender as IParamColumn;
            string columnName = e;
            mModified.Add(prop, columnName);
            if (string.IsNullOrEmpty(prop[e].ToString())) {
                /*-- 記錄非法的欄位 --*/
                mIllegal.Add(prop, columnName);
            } else {
                /*-- 移除非法紀錄 --*/
                mIllegal.Remove(prop, columnName);
            }
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
