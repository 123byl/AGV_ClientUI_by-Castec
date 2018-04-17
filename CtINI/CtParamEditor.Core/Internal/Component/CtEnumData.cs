using CtINI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtParamEditor.Core.Internal {
    /// <summary>
    /// Enum資料型態
    /// </summary>
    internal class CtEnum : IEnum {
        public Delegates.EnumData.DelContainItem ContainItem { get; set; } = null;
        public string Value { get; private set; }
        public string TypeName { get; private set; }
        public bool SetValue(string val) {
            if (ContainItem?.Invoke(TypeName, val) ?? false) {
                Value = val;
                return true;
            } else {
                return false;
            }
        }
        public CtEnum(string typeName) { TypeName = typeName; }
        public override string ToString() {
            return Value;
        }
    }


    /// <summary>
    /// Enum型態定義管理器
    /// </summary>
    internal class CtEnumData {
        public Dictionary<string, Dictionary<string, string>> Data = new Dictionary<string, Dictionary<string, string>>();
        public bool ReadINI(List<IniStruct> data) {
            Data = data.
                FindAll(ini => !ini.ContainKey("Type")).
                ToDictionary(k => k.Section, v => {
                    IEnumerator em = v.GetEnumerator();
                    Dictionary<string, string> tmp = new Dictionary<string, string>();
                    while (em.MoveNext()) {
                        KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)em.Current;
                        tmp.Add(kvp.Key, kvp.Value);
                    }
                    return tmp;
                }
            );
            return true;
        }

        /// <summary>
        /// 將資料以INI格式寫入<see cref="StringBuilder"/>
        /// </summary>
        /// <param name="iniContent"></param>
        public void WriteINI(StringBuilder iniContent) {
            foreach (var enumName in Data.Keys) {
                iniContent.AppendLine($"[{enumName}]");
                foreach (var kvp in Data[enumName]) {
                    iniContent.AppendLine($"{kvp.Key}={kvp.Value}");
                }
                iniContent.AppendLine();
            }
        }
        public bool ContainType(string type) {
            return Data.ContainsKey(type);
        }
        public bool ContainItem(string type, string item) {
            return Data.ContainsKey(type) && Data[type].ContainsKey(item);
        }
        public IEnumerable<string> GetItems(string type) {
            return Data.ContainsKey(type) ? Data[type].Keys : null;
        }
        public IEnumerable<string> GetTypes() {
            return Data.Keys;
        }

        public void ReadEnum(Enum em) {
            string typeName = em.GetType().Name;
            if (!Data.ContainsKey(typeName)) {
                Dictionary<string, string> mapping = new Dictionary<string, string>();
                foreach (var item in Enum.GetValues(em.GetType())) {
                    mapping.Add(item.ToString(), ((int)item).ToString());
                }
                Data.Add(typeName, mapping);
            }
        }
    }

}
