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
        private Dictionary<int, bool> mIsDef = new Dictionary<int, bool>() {
            { PropField.Idx.Max,false},
            { PropField.Idx.Min,false},
            { PropField.Idx.Default,false}
        };
        private Dictionary<int, T> mVal = new Dictionary<int, T>() {
            { PropField.Idx.Value,default(T)},
            { PropField.Idx.Max,default(T)},
            { PropField.Idx.Min,default(T)},
            { PropField.Idx.Default,default(T)}
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
        public string Value { get { return mVal[PropField.Idx.Value].ToString(); } }
        public string Max {
            get {
                if (IsDefMax) {
                    return mVal[PropField.Idx.Max].ToString();
                } else
                    return null;
            }
        }
        public string Min {
            get {
                if (IsDefMin) {
                    return mVal[PropField.Idx.Min].ToString();
                } else
                    return null;
            }
        }
        public string Def {
            get {
                if (IsDefDefault) {
                    return mVal[PropField.Idx.Default].ToString();
                } else return null;
            }
        }

        public bool IsDefMax {
            get { return mIsDefRange && mIsDef[PropField.Idx.Max]; }
            set { mIsDef[PropField.Idx.Max] = value; }
        }
        public bool IsDefMin {
            get { return mIsDefRange && mIsDef[PropField.Idx.Min]; }
            set { mIsDef[PropField.Idx.Min] = value; }
        }
        public bool IsDefDefault {
            get { return mIsDef[PropField.Idx.Default]; }
            set { mIsDef[PropField.Idx.Default] = value; }
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
        public bool SetValue(string val, int idx) {
            bool IsNullorEmpty = string.IsNullOrEmpty(val);
            T tmpVal = default(T);
            bool suc = SetValue(val, ref tmpVal);
            //suc = GenericParser.TryParse(val, out tmpVal);
            if (suc) {
                if (idx == PropField.Idx.Value || idx == PropField.Idx.Default) {
                    /*-- 有設定最大值則比對之 --*/
                    if (IsDefMax && (tmpVal as IComparable<T>).CompareTo(mVal[PropField.Idx.Max]) > 0) return false;
                    /*-- 有設定最小值則比對之 --*/
                    if (IsDefMin && (tmpVal as IComparable<T>).CompareTo(mVal[PropField.Idx.Min]) < 0) return false;
                }
                if (mIsDef.ContainsKey(idx)) mIsDef[idx] = !IsNullorEmpty;
                mVal[idx] = tmpVal;
            }
            return suc;
        }

        public Type GetParamType() {
            return typeof(T);
        }

        #endregion Implement - IParamValue

        #region Implement - IparamValue<T>

        T IParamValue<T>.Value {get {return mVal[PropField.Idx.Value];}}

        T IParamValue<T>.Max {get {return mVal[PropField.Idx.Max];}}

        T IParamValue<T>.Min {get { return mVal[PropField.Idx.Min]; }}

        T IParamValue<T>.Def { get { return mVal[PropField.Idx.Default];} }
        public void SetMaximun(T max) {
            if (RangeDefinable) {
                mVal[PropField.Idx.Max] = max;
                mIsDefRange = true;
                mIsDef[PropField.Idx.Max] = true;
            }
        }

        public void RemoveMinimun() {
            mIsDef[PropField.Idx.Min] = false;
        }

        public void RemoveMaxinum() {
            mIsDef[PropField.Idx.Max] = false;
        }

        public void SetMinimun(T min) {
            if (RangeDefinable) {
                mVal[PropField.Idx.Min] = min;
                mIsDefRange = true;
                mIsDef[PropField.Idx.Min] = true;
            }
        }

        public void SetDefault(T def) {
            mVal[PropField.Idx.Default] = def;
            mIsDef[PropField.Idx.Default] = true;
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

        #endregion Declaration - Properteis

        #region Function - Constructors

        public CtParam() {

        }

        public CtParam(string name, object val, string description, object def, object max, object min) {
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
            return mVal?.SetValue(val, PropField.Idx.Value) ?? false;
        }

        public bool SetMax(string val) {
            return mVal.SetValue(val, PropField.Idx.Max);
        }

        public bool SetMin(string val) {
            return mVal.SetValue(val, PropField.Idx.Min);
        }

        public bool SetDefault(string val) {
            return mVal.SetValue(val, PropField.Idx.Default);
        }

        #endregion Implement - IAgvToDgvCol

        #region Implement - IParam

        public string GetParamValue(int idxCol) {
            switch (idxCol) {
                case PropField.Idx.Name: return mName;
                case PropField.Idx.ValType: return mVal?.ValType;
                case PropField.Idx.Value: return mVal?.Value;
                case PropField.Idx.Description: return mDescription;
                case PropField.Idx.Max: return mVal?.Max;
                case PropField.Idx.Min: return mVal?.Min;
                case PropField.Idx.Default: return mVal?.Def;
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

        public bool SetValue(string val, int idx) {
            switch (idx) {
                case PropField.Idx.Name: return SetName(val);
                case PropField.Idx.ValType: return SetType(val);
                case PropField.Idx.Description: return SetDescription(val);
                case PropField.Idx.Value:
                case PropField.Idx.Max:
                case PropField.Idx.Min:
                case PropField.Idx.Default:
                    return mVal.SetValue(val, idx);
                default: return false;
            }
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
            bool ret = (isContain = ini.ContainKey(PropField.Str.ValType)) && (isSuc = SetType(ini[PropField.Str.ValType]));
            return ret;
        }

        private bool SetValue(IniStruct ini) {
            bool isContain = false;
            bool isSuc = false;
            bool ret = (isContain = ini.ContainKey(PropField.Str.Value)) && (isSuc = SetValue(ini[PropField.Str.Value]));
            return ret;
        }

        private bool SetDescription(IniStruct ini) {
            bool isContain = false;
            bool isSuc = false;
            bool ret = (isContain = ini.ContainKey(PropField.Str.Description)) && (isSuc = SetDescription(ini[PropField.Str.Description]));
            return ret;
        }

        private bool SetMax(IniStruct ini) {
            bool isContain = false;
            bool isSuc = false;
            bool ret = (isContain = ini.ContainKey(PropField.Str.Max)) && (isSuc = SetMax(ini[PropField.Str.Max]));
            return ret;
        }

        private bool SetMin(IniStruct ini) {
            bool isContain = false;
            bool isSuc = false;
            bool ret = (isContain = ini.ContainKey(PropField.Str.Min)) && (isSuc = SetMin(ini[PropField.Str.Min]));
            return ret;
        }

        private bool SetDefault(IniStruct ini) {
            bool isContain = false;
            bool isSuc = false;
            bool ret = (isContain = ini.ContainKey(PropField.Str.Default)) && (isSuc = SetDefault(ini[PropField.Str.Default]));
            return ret;
        }
        
        #endregion Function - Private Methods

    }

}
