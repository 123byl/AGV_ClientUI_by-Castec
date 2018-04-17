/*没有过多检查，可能会有些许bug*/
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
namespace CtINI {

    /// <summary>
    /// 靜態函式庫
    /// </summary>
    public class IniMth {

        #region Function - Static Methods
        
        /// <summary>
        /// 編碼
        /// </summary>
        public static System.Text.Encoding Encoding { get; set; } = System.Text.Encoding.UTF8;

        /// <summary>
        /// 讀取單一值
        /// </summary>
        /// <param name="file"></param>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static string ReadValue(string file, string key, string section) {
            string comments = "";
            return ReadValue(file, key, section, ref comments);
        }

        /// <summary>
        /// 讀取單一值(含註解)
        /// </summary>
        /// <param name="file"></param>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <param name="comments"></param>
        /// <returns></returns>
        public static string ReadValue(string file, string key, string section, ref string comments) {
            string valueText = "";
            string content = GetText(file);
            if (!string.IsNullOrEmpty(section)) //首先遍历节点
            {
                MatchCollection matches = new Regex(@"\[\s*(?'section'[^\[\]\s]+)\s*\]").Matches(content);
                if (matches.Count <= 0) return "";
                Match currMatch = null;
                Match tailMatch = null;
                foreach (Match match in matches) {
                    string match_section = match.Groups["section"].Value;
                    if (match_section.ToLower() == section.ToLower()) {
                        currMatch = match;
                        continue;
                    } else if (currMatch != null) {
                        tailMatch = match;
                        break;
                    }

                }
                valueText = content.Substring(currMatch.Index + currMatch.Length, (tailMatch != null ? tailMatch.Index : content.Length) - currMatch.Index - currMatch.Length);//截取有效值域


            } else
                valueText = content;
            string[] lines = valueText.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
            foreach (string line in lines) {
                if (string.IsNullOrEmpty(line) || line == "\r\n" || line.Contains("["))
                    continue;
                string valueLine = line;
                if (line.Contains(";")) {
                    string[] seqPairs = line.Split(';');
                    if (seqPairs.Length > 1)
                        comments = seqPairs[1].Trim();
                    valueLine = seqPairs[0];
                }
                string[] keyValuePairs = valueLine.Split('=');
                string line_key = keyValuePairs[0];
                string line_value = "";
                if (keyValuePairs.Length > 1) {
                    line_value = keyValuePairs[1];
                }
                if (key.ToLower().Trim() == line_key.ToLower().Trim()) {
                    return line_value;
                }
            }
            return "";
        }

        /// <summary>
        /// 寫入INI檔
        /// </summary>
        /// <param name="file"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="comment"></param>
        public static void Write(string file, string section, string key, string value, string comment = null) {
            bool isModified = false;
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            string content = string.Empty;
            if (File.Exists(file)) content = GetText(file);
            System.Text.StringBuilder newValueContent = new System.Text.StringBuilder();
            #region 写入了节点
            if (!string.IsNullOrEmpty(section)) {
                string pattern = string.Format(@"\[\s*{0}\s*\](?'valueContent'[^\[\]]*)", section);
                MatchCollection matches = new Regex(pattern).Matches(content);
                if (matches.Count <= 0) {
                    stringBuilder.AppendLine(string.Format("[{0}]", section)); //检查节点是否存在
                    stringBuilder.WriteKey(key, value, comment);
                    //if (!string.IsNullOrEmpty(key)) {
                    //    stringBuilder.AppendLine(string.Format("{0}={1}{2}", key, value, !string.IsNullOrEmpty(comment) ? (";" + comment) : ""));
                    //}
                    stringBuilder.AppendLine(content);
                    isModified = true;
                } else {
                    Match match = matches[0];
                    string valueContent = match.Groups["valueContent"].Value;
                    string[] lines = valueContent.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);

                    newValueContent.AppendLine(string.Format("[{0}]", section));
                    foreach (string line in lines) {
                        if (string.IsNullOrEmpty(line) || line == "\r\n" || line.Contains("[")) {
                            continue;
                        }

                        string valueLine = line;
                        string comments = "";
                        if (line.Contains(";")) {
                            string[] seqPairs = line.Split(';');
                            if (seqPairs.Length > 1)
                                comments = seqPairs[1].Trim();
                            valueLine = seqPairs[0];
                        }
                        string[] keyValuePairs = valueLine.Split('=');
                        string line_key = keyValuePairs[0];
                        string line_value = "";
                        if (keyValuePairs.Length > 1) {
                            line_value = keyValuePairs[1];
                        }
                        if (key?.ToLower()?.Trim() == line_key.ToLower().Trim()) {
                            isModified = true;
                            newValueContent.WriteKey(key, value, comment);
                        } else {
                            newValueContent.AppendLine(line);
                        }
                    }
                    if (!isModified)
                        newValueContent.WriteKey(key, value, comment);
                    string newVal = newValueContent.ToString();
                    content = content.Replace(match.Value, newVal);
                    stringBuilder.Append(content);

                }
            }
            #endregion
            #region 没有指明节点
            else {
                string valueText = "";
                //如果节点为空
                MatchCollection matches = new Regex(@"\[\s*(?'section'[^\[\]\s]+)\s*\](?'valueContent'[^\[\]]*)").Matches(content);
                if (matches.Count > 0) {
                    valueText = matches[0].Index > 0 ? content.Substring(0, matches[0].Index) : "";
                    string[] lines = valueText.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
                    foreach (string line in lines) {
                        if (string.IsNullOrEmpty(line) || line == "\r\n" || line.Contains("[")) {
                            continue;
                        }

                        string valueLine = line;
                        string comments = "";
                        if (line.Contains(";")) {
                            string[] seqPairs = line.Split(';');
                            if (seqPairs.Length > 1)
                                comments = seqPairs[1].Trim();
                            valueLine = seqPairs[0];
                        }
                        string[] keyValuePairs = valueLine.Split('=');
                        string line_key = keyValuePairs[0];
                        string line_value = "";
                        if (keyValuePairs.Length > 1) {
                            line_value = keyValuePairs[1];
                        }
                        if (key?.ToLower()?.Trim() == line_key.ToLower().Trim()) {
                            isModified = true;
                            newValueContent.WriteKey(key, value, comment);
                        } else {
                            newValueContent.AppendLine(line);
                        }


                    }
                    if (!isModified)
                        newValueContent.WriteKey(key, value, comment);
                    string newVal = newValueContent.ToString();
                    content = content.Replace(valueText, newVal);
                    stringBuilder.Append(content);
                } else {
                    stringBuilder.WriteKey(key, value, comment);
                }
            }
            #endregion
            System.IO.File.WriteAllText(file, stringBuilder.ToString(), Encoding);
        }

        /// <summary>
        /// 讀取所有設定值
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static System.Collections.Generic.List<IniStruct> ReadValues(string file) {
            System.Collections.Generic.List<IniStruct> iniStructList = new System.Collections.Generic.List<IniStruct>();
            string content = GetText(file);
            System.Text.RegularExpressions.MatchCollection matches = new System.Text.RegularExpressions.Regex(@"\[\s*(?'section'[^\[\]\s]+)\s*\](?'valueContent'[^\[\]]*)").Matches(content);
            foreach (System.Text.RegularExpressions.Match match in matches) {
                IniStruct iniStruct = new IniStruct();
                string match_section = match.Groups["section"].Value;
                string match_value = match.Groups["valueContent"].Value;
                iniStruct.Section = match_section;

                string[] lines = match_value.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
                foreach (string line in lines) {
                    if (string.IsNullOrEmpty(line) || line == "\r\n" || line.Contains("["))
                        continue;
                    string comments = "";//注释
                    string valueLine = line;
                    if (line.Contains(";")) {
                        string[] seqPairs = line.Split(';');
                        if (seqPairs.Length > 1)
                            comments = seqPairs[1].Trim();
                        valueLine = seqPairs[0];
                    }
                    string[] keyValuePairs = valueLine.Split('=');
                    string line_key = keyValuePairs[0];
                    string line_value = "";
                    if (keyValuePairs.Length > 1) {
                        line_value = keyValuePairs[1];
                    }
                    iniStruct.Add(line_key, line_value, comments);
                }
                iniStructList.Add(iniStruct);
            }

            return iniStructList;
        }

        /// <summary>
        /// 讀取檔案內容
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static string GetText(string file) {
            string content = File.ReadAllText(file);
            if (content.Contains("�")) {
                Encoding = System.Text.Encoding.GetEncoding("GBK");
                content = File.ReadAllText(file, System.Text.Encoding.GetEncoding("GBK"));
            }
            return content;
        }

        #endregion Function - Static Methods

    }

    /// <summary>
    /// INI檔操作類
    /// </summary>
    public class CtINI
    {

        #region Declaration - Fileds

        private string iniFile;

        #endregion Declaration - Fields

        #region Function - Constructors

        public CtINI()
        {

        }

        /// <summary>
        /// 從檔案進行建構
        /// </summary>
        /// <param name="file"></param>
        public CtINI(string file)
        {
            this.iniFile = file;
        }

        #endregion Function - Constructors

        #region Function - Static Methods

        /// <summary>
        /// 編碼
        /// </summary>
        public static System.Text.Encoding Encoding = System.Text.Encoding.UTF8;

        /// <summary>
        /// 讀取單一值
        /// </summary>
        /// <param name="file"></param>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static string ReadValue(string file, string key, string section) {
            string comments = "";
            return ReadValue(file, key, section, ref comments);
        }

        /// <summary>
        /// 讀取單一值(含註解)
        /// </summary>
        /// <param name="file"></param>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <param name="comments"></param>
        /// <returns></returns>
        public static string ReadValue(string file, string key, string section, ref string comments) {
            string valueText = "";
            string content = GetText(file);
            if (!string.IsNullOrEmpty(section)) //首先遍历节点
            {
                MatchCollection matches = new Regex(@"\[\s*(?'section'[^\[\]\s]+)\s*\]").Matches(content);
                if (matches.Count <= 0) return "";
                Match currMatch = null;
                Match tailMatch = null;
                foreach (Match match in matches) {
                    string match_section = match.Groups["section"].Value;
                    if (match_section.ToLower() == section.ToLower()) {
                        currMatch = match;
                        continue;
                    } else if (currMatch != null) {
                        tailMatch = match;
                        break;
                    }

                }
                valueText = content.Substring(currMatch.Index + currMatch.Length, (tailMatch != null ? tailMatch.Index : content.Length) - currMatch.Index - currMatch.Length);//截取有效值域


            } else
                valueText = content;
            string[] lines = valueText.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
            foreach (string line in lines) {
                if (string.IsNullOrEmpty(line) || line == "\r\n" || line.Contains("["))
                    continue;
                string valueLine = line;
                if (line.Contains(";")) {
                    string[] seqPairs = line.Split(';');
                    if (seqPairs.Length > 1)
                        comments = seqPairs[1].Trim();
                    valueLine = seqPairs[0];
                }
                string[] keyValuePairs = valueLine.Split('=');
                string line_key = keyValuePairs[0];
                string line_value = "";
                if (keyValuePairs.Length > 1) {
                    line_value = keyValuePairs[1];
                }
                if (key.ToLower().Trim() == line_key.ToLower().Trim()) {
                    return line_value;
                }
            }
            return "";
        }

        /// <summary>
        /// 寫入INI檔
        /// </summary>
        /// <param name="file"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="comment"></param>
        public static void Write(string file, string section, string key, string value, string comment = null) {
            bool isModified = false;
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            string content = GetText(file);
            System.Text.StringBuilder newValueContent = new System.Text.StringBuilder();
            #region 写入了节点
            if (!string.IsNullOrEmpty(section)) {
                string pattern = string.Format(@"\[\s*{0}\s*\](?'valueContent'[^\[\]]*)", section);
                MatchCollection matches = new Regex(pattern).Matches(content);
                if (matches.Count <= 0) {
                    stringBuilder.AppendLine(string.Format("[{0}]", section)); //检查节点是否存在
                    stringBuilder.WriteKey(key, value, comment);
                    //if (!string.IsNullOrEmpty(key)) {
                    //    stringBuilder.AppendLine(string.Format("{0}={1}{2}", key, value, !string.IsNullOrEmpty(comment) ? (";" + comment) : ""));
                    //}
                    stringBuilder.AppendLine(content);
                    isModified = true;
                } else {
                    Match match = matches[0];
                    string valueContent = match.Groups["valueContent"].Value;
                    string[] lines = valueContent.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);

                    newValueContent.AppendLine(string.Format("[{0}]", section));
                    foreach (string line in lines) {
                        if (string.IsNullOrEmpty(line) || line == "\r\n" || line.Contains("[")) {
                            continue;
                        }

                        string valueLine = line;
                        string comments = "";
                        if (line.Contains(";")) {
                            string[] seqPairs = line.Split(';');
                            if (seqPairs.Length > 1)
                                comments = seqPairs[1].Trim();
                            valueLine = seqPairs[0];
                        }
                        string[] keyValuePairs = valueLine.Split('=');
                        string line_key = keyValuePairs[0];
                        string line_value = "";
                        if (keyValuePairs.Length > 1) {
                            line_value = keyValuePairs[1];
                        }
                        if (key?.ToLower()?.Trim() == line_key.ToLower().Trim()) {
                            isModified = true;
                            newValueContent.WriteKey(key, value,comment);
                        } else {
                            newValueContent.AppendLine(line);
                        }
                    }
                    if (!isModified)
                        newValueContent.WriteKey(key, value, comment);
                    string newVal = newValueContent.ToString();
                    content = content.Replace(match.Value, newVal);
                    stringBuilder.Append(content);

                }
            }
            #endregion
            #region 没有指明节点
            else {
                string valueText = "";
                //如果节点为空
                MatchCollection matches = new Regex(@"\[\s*(?'section'[^\[\]\s]+)\s*\](?'valueContent'[^\[\]]*)").Matches(content);
                if (matches.Count > 0) {
                    valueText = matches[0].Index > 0 ? content.Substring(0, matches[0].Index) : "";
                    string[] lines = valueText.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
                    foreach (string line in lines) {
                        if (string.IsNullOrEmpty(line) || line == "\r\n" || line.Contains("[")) {
                            continue;
                        }

                        string valueLine = line;
                        string comments = "";
                        if (line.Contains(";")) {
                            string[] seqPairs = line.Split(';');
                            if (seqPairs.Length > 1)
                                comments = seqPairs[1].Trim();
                            valueLine = seqPairs[0];
                        }
                        string[] keyValuePairs = valueLine.Split('=');
                        string line_key = keyValuePairs[0];
                        string line_value = "";
                        if (keyValuePairs.Length > 1) {
                            line_value = keyValuePairs[1];
                        }
                        if (key?.ToLower()?.Trim() == line_key.ToLower().Trim()) {
                            isModified = true;
                            newValueContent.WriteKey(key, value,comment);
                        } else {
                            newValueContent.AppendLine(line);
                        }


                    }
                    if (!isModified)
                        newValueContent.WriteKey(key, value,comment);
                    string newVal = newValueContent.ToString();
                    content = content.Replace(valueText, newVal);
                    stringBuilder.Append(content);
                } else {
                    stringBuilder.WriteKey(key, value,comment);
                }
            }
            #endregion
            System.IO.File.WriteAllText(file, stringBuilder.ToString(), Encoding);
        }
        
        /// <summary>
        /// 讀取所有設定值
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static System.Collections.Generic.List<IniStruct> ReadValues(string file) {
            System.Collections.Generic.List<IniStruct> iniStructList = new System.Collections.Generic.List<IniStruct>();
            string content = GetText(file);
            System.Text.RegularExpressions.MatchCollection matches = new System.Text.RegularExpressions.Regex(@"\[\s*(?'section'[^\[\]\s]+)\s*\](?'valueContent'[^\[\]]*)").Matches(content);
            foreach (System.Text.RegularExpressions.Match match in matches) {
                IniStruct iniStruct = new IniStruct();
                string match_section = match.Groups["section"].Value;
                string match_value = match.Groups["valueContent"].Value;
                iniStruct.Section = match_section;

                string[] lines = match_value.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
                foreach (string line in lines) {
                    if (string.IsNullOrEmpty(line) || line == "\r\n" || line.Contains("["))
                        continue;
                    string comments = "";//注释
                    string valueLine = line;
                    if (line.Contains(";")) {
                        string[] seqPairs = line.Split(';');
                        if (seqPairs.Length > 1)
                            comments = seqPairs[1].Trim();
                        valueLine = seqPairs[0];
                    }
                    string[] keyValuePairs = valueLine.Split('=');
                    string line_key = keyValuePairs[0];
                    string line_value = "";
                    if (keyValuePairs.Length > 1) {
                        line_value = keyValuePairs[1];
                    }
                    iniStruct.Add(line_key, line_value, comments);
                }
                iniStructList.Add(iniStruct);
            }

            return iniStructList;
        }

        /// <summary>
        /// 讀取檔案內容
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static string GetText(string file) {
            string content = File.ReadAllText(file);
            if (content.Contains("�")) {
                Encoding = System.Text.Encoding.GetEncoding("GBK");
                content = File.ReadAllText(file, System.Text.Encoding.GetEncoding("GBK"));
            }
            return content;
        }

        #endregion Function - Static Methods

        #region Function - Public Methods
        
        /// <summary>
        /// 讀取單一值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public string ReadValue(string key, string section)
        {
            string comments = "";
            return ReadValue(this.iniFile, key, section, ref comments);
        }

        /// <summary>
        /// 讀取單一值(含註解)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <param name="comments"></param>
        /// <returns></returns>
        public string ReadValue(string key, string section, ref string comments)
        {
            if (string.IsNullOrEmpty(this.iniFile)) throw new System.Exception("没有设置文件路径");
            return ReadValue(this.iniFile, key, section, ref comments);
        }

        /// <summary>
        /// 寫入INI檔
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Write(string section, string key, string value)
        {
            Write(section, key, value, null);
        }

        /// <summary>
        /// 寫入INI檔(含註解)
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="comment"></param>
        public void Write(string section, string key, string value, string comment)
        {
            Write(this.iniFile, section, key, value, comment);
        }

        #endregion Function - Public Methods

    }
    
    /// <summary>
    /// Ini結構類
    /// </summary>
    public class IniStruct:System.Collections.IEnumerable
    {
        #region Declaration - Fields

        /// <summary>
        /// 註解清單
        /// </summary>
        private System.Collections.Generic.List<string> mCommentList;
        
        /// <summary>
        /// KeyVal對照清單
        /// </summary>
        private System.Collections.Generic.SortedList<string, string> mKeyValuePairs;

        #endregion Declaration - Fields

        #region Declaration - Properties
        
        /// <summary>
        /// Section名稱
        /// </summary>
        public string Section { get; set; }
        
        /// <summary>
        /// 索引查詢Value
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index] {
            get {
                if (this.mKeyValuePairs.Count > index)
                    return this.mKeyValuePairs.Values[index];
                else return "";
            }
            set {
                if (this.mKeyValuePairs.Count > index)
                    this.mKeyValuePairs.Values[index] = value;
            }
        }

        /// <summary>
        /// Key查詢Value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key] {
            get {
                if (this.mKeyValuePairs.ContainsKey(key))
                    return this.mKeyValuePairs[key];
                else return "";
            }
            set {
                if (this.mKeyValuePairs.ContainsKey(key))
                    this.mKeyValuePairs[key] = value;
            }
        }

        #endregion Declaration - Properties

        #region Funciton - Constructors

        public IniStruct()
        {
            this.mKeyValuePairs = new System.Collections.Generic.SortedList<string, string>();
            mCommentList = new System.Collections.Generic.List<string>();
        }

        #endregion Function - Constructors

        #region Function - Public Methods

        /// <summary>
        /// 取得指定Key之註解
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetComment(string key)
        {
            if (this.mKeyValuePairs.ContainsKey(key))
            {
                int index = this.mKeyValuePairs.IndexOfKey(key);
                return this.mCommentList[index];
            }
            return "";
        }

        /// <summary>
        /// 增加新的設定值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="commont"></param>
        public void Add(string key, string value, string commont = null)
        {
            this.mKeyValuePairs.Add(key, value);
            this.mCommentList.Add(commont);
        }

        public bool ContainKey(string key) {
            return this.mKeyValuePairs.ContainsKey(key);
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return this.mKeyValuePairs.GetEnumerator();
        }

        public override string ToString() {
            return string.Format("{0}", this.Section);
        }

        #endregion Function - Public Methods

    }

    /// <summary>
    /// 擴充方法定義
    /// </summary>
    internal static class IniExtension {
        public static void WriteKey(this System.Text.StringBuilder sb, string key,string value,string comment) {
            if (!string.IsNullOrEmpty(key)) {
                sb.AppendLine(string.Format("{0}={1}{2}", key, value, !string.IsNullOrEmpty(comment) ? (";" + comment) : ""));
            }
        }
    }

}