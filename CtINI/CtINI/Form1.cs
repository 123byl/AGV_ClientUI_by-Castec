using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtINI {
    public partial class Form1 : Form {

        //CtrlIni mIni = new CtIni.CtrlIni();

        public Form1() {
            InitializeComponent();
            //Ini.Write(@"D:\Test.ini", "Section", "Key", "Value", "Comment");
            //Ini.Write(@"D:\Test.ini", "Section", "Key2", "Value2", "Comment2");
            //Ini.Write(@"D:\Test.ini", "Section", "Key3", "Value3", "Comment3");
            var v = CtINI.ReadValues(@"D:Test.ini");
            foreach (var ini in v) {
                Console.WriteLine($"{ini.Section}");
                IEnumerator Enumerator =  ini.GetEnumerator();
                while (Enumerator.MoveNext()) {
                    KeyValuePair<string,string> kvp = (KeyValuePair<string, string>)Enumerator.Current;
                    Console.WriteLine($"{kvp.Key}={kvp.Value};{ini.GetComment(kvp.Key)}");
                }
                Console.WriteLine();
            }
            IniProperty<int> ii = new IniProperty<int>();
            int i = ii.Value;

        }


        private void btnIni_Click(object sender, EventArgs e) {
            //if (mIni.Visible == false) mIni.Show();
        }
    }

    public interface IiniProperty {
        
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
        object Value { get;}
        
        /// <summary>
        /// 參數說明
        /// </summary>
        string Description { get;}
        
        /// <summary>
        /// 參數最大值
        /// </summary>
        object Max { get; }
        
        /// <summary>
        /// 參數最小值
        /// </summary>
        object Min { get; }
        
        /// <summary>
        /// 參數最大值
        /// </summary>
        object Default { get; } 
        
        /// <summary>
        /// 參數是否可修改
        /// </summary>
        bool IsLock { get; }

        /// <summary>
        /// 參數值是否受限(具有最大最小值)
        /// </summary>
        bool IsRestricted { get; }

        /// <summary>
        /// 是否被修改過
        /// </summary>
        bool IsModified { get; }

        /// <summary>
        /// 參數值設定方法
        /// </summary>
        /// <returns>是否修改成功</returns>
        bool SerValue();
        
    }

    public class IniProperty<T> {
        
        /// <summary>
        /// 參數名稱
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// 參數類型
        /// </summary>
        public string Type { get { return typeof(T).ToString(); }}
        /// <summary>
        /// 參數值
        /// </summary>
        public T Value { get; set; } = default(T);
        /// <summary>
        /// 參數說明
        /// </summary>
        public string Description { get; protected set; }
        /// <summary>
        /// 參數最大值
        /// </summary>
        public string Max { get; protected set; } = string.Empty;
        /// <summary>
        /// 參數最小值
        /// </summary>
        public string Min { get; protected set; } = string.Empty;
        /// <summary>
        /// 參數最大值
        /// </summary>
        public string Default { get; protected set; } = string.Empty;
        /// <summary>
        /// 參數是否可修改
        /// </summary>
        public bool IsLock { get; }
        
        public IniStruct ToIniStruct(string fileName) {
            IniStruct ini = new IniStruct();
            ini.Section = Name;
            ini.Add("Type", Type);
            ini.Add("Description", Description);
            ini.Add("Value", Value.ToString());
            if (!string.IsNullOrEmpty(Max)) ini.Add("Max", Max);
            if (!string.IsNullOrEmpty(Min)) ini.Add("Min", Min);
            if (!string.IsNullOrEmpty(Default)) ini.Add("Def", Default);
            ini.Add("IsLock", IsLock.ToString());
            return ini;
        }
    }

}
