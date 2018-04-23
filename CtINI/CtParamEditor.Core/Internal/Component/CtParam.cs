using CtGenericParser;
using CtINI;
using CtParamEditor.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CtParamEditor.Core.Internal.Component {
    
    internal class ParamValue<T> : IParamValue,IParamValue<T>{

        #region Declaration - Fields

        private bool mIsDefRange = false;
        private Dictionary<string, bool> mIsDef = new Dictionary<string, bool>() {
            { nameof(IParamColumn.Max),false},
            { nameof(IParamColumn.Min),false},
            { nameof(IParamColumn.Default),false}
        };
        private Dictionary<string, T> mVal = new Dictionary<string, T>() {
            { nameof(IParamColumn.Value),default(T)},
            { nameof(IParamColumn.Max),default(T)},
            { nameof(IParamColumn.Min),default(T)},
            { nameof(IParamColumn.Default),default(T)}
        };

        #endregion Declaration - Fields

        public Delegates.EnumData.DelContainItem ContainItem { get; set; } = null;

        #region Funciton - Constructors

        public ParamValue() {
            SetValType(typeof(T).Name);
        }

        public ParamValue(string type) {
            SetValType(type);
        }

        #endregion Funciton - Constructors

        #region Implement - IParamValue

        public string ValType { get; private set; }
        T IParamValue<T>.Value { get { return mVal[nameof(IParamColumn.Value)]; } }
        T IParamValue<T>.Max {
            get {
                if (IsDefMax) {
                    return mVal[nameof(IParamColumn.Max)];
                } else
                    return default(T);
            }
        }
        T IParamValue<T>.Min {
            get {
                if (IsDefMin) {
                    return mVal[nameof(IParamColumn.Min)];
                } else
                    return default(T);
            }
        }
        T IParamValue<T>.Def {
            get {
                if (IsDefDefault) {
                    return mVal[nameof(IParamColumn.Default)];
                } else return default(T);
            }
        }

        public bool IsDefMax {
            get { return mIsDefRange && mIsDef[nameof(IParamColumn.Max)]; }
            set { mIsDef[nameof(IParamColumn.Max)] = value; }
        }
        public bool IsDefMin {
            get { return mIsDefRange && mIsDef[nameof(IParamColumn.Min)]; }
            set { mIsDef[nameof(IParamColumn.Min)] = value; }
        }
        public bool IsDefDefault {
            get { return mIsDef[nameof(IParamColumn.Default)]; }
            set { mIsDef[nameof(IParamColumn.Default)] = value; }
        }

        public bool RangeDefinable {
            get {
                Type type = typeof(T);
                Type[] interfaces = type.GetInterfaces();
                return interfaces != null &&  interfaces.Contains(typeof(IComparable)) && interfaces.Contains(typeof(IFormattable));
            }
        }
        
        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="val">設定值</param>
        /// <param name="idx">要設定的參數索引</param>
        /// <returns>設定值無法轉換為T型態或者超出最大最小則為False</returns>
        public bool SetValue(string val, string columnName) {
            bool IsNullorEmpty = string.IsNullOrEmpty(val);
            T tmpVal = default(T);
            bool suc = SetValue(val, ref tmpVal);
            //suc = GenericParser.TryParse(val, out tmpVal);
            if (suc) {
                if (columnName == nameof(IParamColumn.Value) || columnName == nameof(IParamColumn.Default)) {
                    /*-- 有設定最大值則比對之 --*/
                    if (IsDefMax && (tmpVal as IComparable<T>).CompareTo(mVal[nameof(IParamColumn.Max)]) > 0) return false;
                    /*-- 有設定最小值則比對之 --*/
                    if (IsDefMin && (tmpVal as IComparable<T>).CompareTo(mVal[nameof(IParamColumn.Min)]) < 0) return false;
                }
                if (mIsDef.ContainsKey(columnName)) mIsDef[columnName] = !IsNullorEmpty;
                mVal[columnName] = tmpVal;
            }
            return suc;
        }



        public Type GetParamType() {
            return typeof(T);
        }

        #endregion Implement - IParamValue

        #region Implement - IparamValue<T>
        
        string IParamValue.Value => mVal[nameof(IParamColumn.Value)]?.ToString() ?? "";

        string IParamValue.Max => mVal[nameof(IParamColumn.Max)]?.ToString() ?? "";

        string IParamValue.Min => mVal[nameof(IParamColumn.Min)]?.ToString() ?? "";

        string IParamValue.Def => mVal[nameof(IParamColumn.Default)]?.ToString() ?? "";

        public void SetMaximun(T max) {
            if (RangeDefinable) {
                mVal[nameof(IParamColumn.Max)] = max;
                mIsDefRange = true;
                mIsDef[nameof(IParamColumn.Max)] = true;
            }
        }

        public void RemoveMinimun() {
            mIsDef[nameof(IParamColumn.Min)] = false;
        }

        public void RemoveMaxinum() {
            mIsDef[nameof(IParamColumn.Max)] = false;
        }

        public void SetMinimun(T min) {
            if (RangeDefinable) {
                mVal[nameof(IParamColumn.Min)] = min;
                mIsDefRange = true;
                mIsDef[nameof(IParamColumn.Min)] = true;
            }
        }

        public void SetDefault(T def) {
            mVal[nameof(IParamColumn.Default)] = def;
            mIsDef[nameof(IParamColumn.Default)] = true;
        }

        public void SetRange(T max, T min) {
            SetMaximun(max);
            SetMinimun(min);
        }

        #endregion Implement - IparamValue<T>

        #region Funciton - Private Methods

        private void SetValType(string type) {
            ValType = type;
            mIsDefRange =
                type == typeof(int).Name ||
                type == typeof(float).Name;
        }

        private bool SetValue(string val, ref T oVal) {
            bool ret = false;
            if (ret = val != null) {
                Type tp = typeof(T);
                MethodInfo TryParse = null;
                if (tp == typeof(string)) {
                    oVal = (T)(object)val;
                } else if (tp.Name == "IEnum") {
                    if (oVal == null) oVal = (T)(object)new CtEnum(ValType);
                    IEnum em = (oVal as IEnum);
                    em.ContainItem = ContainItem;
                    ret = em.SetValue(val);
                } else if (oVal is Enum) {
                    TryParse = EnumTryParse(tp);
                } else {
                    TryParse = StructTryParse(tp);                 
                }
                if (TryParse != null) {
                    var parameters = new object[] { val, Activator.CreateInstance(tp) };
                    if (ret = (bool)TryParse.Invoke(null, parameters)) {
                        oVal = (T)parameters[1];
                    }
                }
            }
            return ret;
        }

        private MethodInfo StructTryParse(Type tp) {
            return tp.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder,
                                                    new Type[] { typeof(string), tp.MakeByRefType() },
                                                    new ParameterModifier[] { new ParameterModifier(2) });
        }

        private MethodInfo EnumTryParse(Type tp) {
            return tp
                .GetMethods(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public)
                .First(x => x.Name == "TryParse" && x.GetParameters().Length == 2)
                .MakeGenericMethod(tp);
        }

        private bool TryParseT<T1>(string content,out T1 val) {
            val = default(T1);
            Type type = typeof(T1);
            MethodInfo tryParse = null;
            if (val is Enum) {
                tryParse = EnumTryParse(type);
            }else {
                tryParse = StructTryParse(type);
            }

            bool ret = false;
            if (tryParse != null) {
                var parameters = new object[] { val, Activator.CreateInstance(type) };
                if (ret = (bool)tryParse.Invoke(null, parameters)) {
                    val = (T1)parameters[1];
                }
            }else {
                throw new Exception($"{type.Name}無對應Parse方法");
            }
            return ret;
        }

  

        #endregion Function - Private Methods

    }

    /// <summary>
    /// AGV參數物件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class CtParam : IParam {

        #region Declaration - Fields

        /// <summary>
        /// 參數值操作物件
        /// </summary>
        public IParamValue mVal;

        /// <summary>
        /// Name欄位值
        /// </summary>
        private string mName = null;

        /// <summary>
        /// Description欄位值
        /// </summary>
        private string mDescription = null;

        #endregion Declaration - Fields

        #region Declaration - Properties
        
        public object this[string columnName] {
            get {
                switch (columnName) {
                    case nameof(IParamColumn.Name):
                        return Name;
                    case nameof(IParamColumn.Description):
                        return Description;
                    case nameof(IParamColumn.Type):
                        return mVal.ValType;
                    case nameof(IParamColumn.Value):
                        return mVal.Value;
                    case nameof(IParamColumn.Min):
                        return mVal.Min;
                    case nameof(IParamColumn.Max):
                        return mVal.Max;
                    case nameof(IParamColumn.Default):
                        return mVal.Def;
                    default:
                        throw new Exception($"未定義欄位名稱{columnName}");
                }
            }
        }
        #endregion Declaration - Properteis

        #region Declaration - Events

        /// <summary>
        /// 變數值變更事件
        /// </summary>
        public event EventHandler<string> ValueChanged;

        #endregion Declaration - Events

        #region Function - Constructors

        public CtParam(EventHandler<string> valueChangedHandle) {
            ValueChanged += valueChangedHandle;
        }

        private CtParam(string name, object val, string description, object def, object max, object min) {
            SetType(val.GetType().Name);
            SetValue(val.ToString());
            SetName(name);
            SetDescription(description);
            SetDefault(def?.ToString());
            if (max != null) SetMax(max.ToString());
            if (min != null) SetMin(min.ToString());
        }

        #endregion Function - Constructors
        
        #region Implement - IAgvToDgvCol

        public string Name { get { return mName; } }
        public string Type {
            get {
                return mVal?.ValType;
            }
        }
        public string Value { get { return mVal?.Value; } }
        public string Description { get { return mDescription; } }
        public string Max { get { return mVal?.Max; } }
        public string Min { get { return mVal?.Min; } }
        public string Default { get { return mVal?.Def; } }

        public bool ReadIni(IniStruct ini) {
            this.mName = ini.Section;
            if (SetType(ini) && SetValue(ini)) {                
                SetDescription(ini);
                SetMax(ini);
                SetMin(ini);
                SetDefault(ini);
                return true;
            } else {
                return false;
            }
        }

        public bool SetType(string type) {
            if (type == typeof(string).Name) {
                mVal = new ParamValue<string>();
            } else if (type == typeof(int).Name) {
                mVal = new ParamValue<int>();
            } else if (type == typeof(float).Name) {
                mVal = new ParamValue<float>();
            } else if (type == typeof(bool).Name) {
                mVal = new ParamValue<bool>();
            } else if (ContainType?.Invoke(type) ?? false) {
                mVal = new ParamValue<IEnum>(type);
                mVal.ContainItem = ContainItem;
            } else {
                mVal = null;
                //throw new Exception($"未定義{type}類型");
            }
            return true;
        }

        public bool SetValue(string val) {
            return mVal?.SetValue(val, nameof(IParamColumn.Value)) ?? false;
        }

        public bool SetMax(string val) {
            return mVal.SetValue(val, nameof(IParamColumn.Max));
        }

        public bool SetMin(string val) {
            return mVal.SetValue(val, nameof(IParamColumn.Min));
        }

        public bool SetDefault(string val) {
            return mVal.SetValue(val, nameof(IParamColumn.Default));
        }

        #endregion Implement - IAgvToDgvCol

        #region Implement - IParam

        public string GetParamValue(string columnName) {
            switch (columnName) {
                case nameof(IParamColumn.Name): return mName;
                case nameof(IParamColumn.Type): return mVal?.ValType;
                case nameof(IParamColumn.Value): return mVal?.Value;
                case nameof(IParamColumn.Description): return mDescription;
                case nameof(IParamColumn.Max): return mVal?.Max;
                case nameof(IParamColumn.Min): return mVal?.Min;
                case nameof(IParamColumn.Default): return mVal?.Def;
                default: throw new Exception("未定義的欄位索引");
            }
        }

        /// <summary>
        /// Enum類型是否存在判斷方法委派
        /// </summary>
        public Delegates.EnumData.DelContainType ContainType { get; set; } = null;

        /// <summary>
        /// 列舉值是否存在判斷方法委派
        /// </summary>
        public Delegates.EnumData.DelContainItem ContainItem { get; set; } = null;

        public bool RangeDefinable {
            get {
                return mVal?.RangeDefinable ?? false;
            }
        }

        public bool SetValue(string val, string columnName) {
            bool success = false;
            switch (columnName) {
                case nameof(IParamColumn.Name): success = SetName(val); break ;
                case nameof(IParamColumn.Type): success = SetType(val); break ;
                case nameof(IParamColumn.Description): success = SetDescription(val);break;
                case nameof(IParamColumn.Value):
                case nameof(IParamColumn.Max):
                case nameof(IParamColumn.Min):
                case nameof(IParamColumn.Default):
                    success = mVal.SetValue(val, columnName);
                    break;
                default:
                    throw new Exception($"未定義欄位名稱{columnName}");
            }
            if (success) {
                ValueChanged?.Invoke(this, columnName);
            }
            return success;
        }
        
        public Type GetParamType() {
            return mVal?.GetParamType();
        }
        #endregion Implement - IParam

        #region Function - Private Methods

        public bool SetName(string val) {
            mName = val.ToString();
            return true;
        }

        public bool SetDescription(string description) {
            mDescription = description;
            return true;
        }

        private bool SetType(IniStruct ini) {
            bool isContain = false;
            bool isSuc = false;
            bool ret = (isContain = ini.ContainKey(nameof(IParamColumn.Type)) && (isSuc = SetType(ini[nameof(IParamColumn.Type)])));
            return ret;
        }

        private bool SetValue(IniStruct ini) {
            bool isContain = false;
            bool isSuc = false;
            bool ret = (isContain = ini.ContainKey(nameof(IParamColumn.Value)) && (isSuc = SetValue(ini[nameof(IParamColumn.Value)])));
            return ret;
        }

        private bool SetDescription(IniStruct ini) {
            bool isContain = false;
            bool isSuc = false;
            bool ret = (isContain = ini.ContainKey(nameof(IParamColumn.Description)) && (isSuc = SetDescription(ini[nameof(IParamColumn.Description)])));
            return ret;
        }

        private bool SetMax(IniStruct ini) {
            bool isContain = false;
            bool isSuc = false;
            bool ret = (isContain = ini.ContainKey(nameof(IParamColumn.Max)) && (isSuc = SetMax(ini[nameof(IParamColumn.Max)])));
            return ret;
        }

        private bool SetMin(IniStruct ini) {
            bool isContain = false;
            bool isSuc = false;
            bool ret = (isContain = ini.ContainKey(nameof(IParamColumn.Min)) && (isSuc = SetMin(ini[nameof(IParamColumn.Min)])));
            return ret;
        }

        private bool SetDefault(IniStruct ini) {
            bool isContain = false;
            bool isSuc = false;
            bool ret = (isContain = ini.ContainKey(nameof(IParamColumn.Default)) && (isSuc = SetDefault(ini[nameof(IParamColumn.Default)])));
            return ret;
        }
        
        #endregion Function - Private Methods

    }

}
