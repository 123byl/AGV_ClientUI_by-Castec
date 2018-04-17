using CtParamEditor.Comm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtTesting {
    public partial class CtrlParamGenarator : Form {
        private IParamEditor mEditor = Factory.Param.Editor();


        SaveFileDialog mSdlg = new SaveFileDialog();
        public CtrlParamGenarator() {
            InitializeComponent();
            mSdlg.Filter = "Ini file|*.ini";
            mSdlg.Title = "Select a Ini File";
        }

        private void btnGenarate_Click(object sender, EventArgs e) {
            mEditor.Clear();
            string data = txtData.Text;
            bool isSuccess = GenerateNormal(data);
            if (isSuccess && mSdlg.ShowDialog() == DialogResult.OK) {
                mEditor.SaveToINI(mSdlg.FileName);
            }

        }

        /// <summary>
        /// 一般格式
        /// </summary>
        /// <remarks> 
        /// varName = varVal
        /// </remarks>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool GenerateNormal(string data) {
            string[] descriptions = data.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if ((descriptions?.Count() ?? 0) > 0) {
                foreach (string dsc in descriptions) {
                    string[] split = dsc.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if ((split?.Count() ?? 0) != 2) {
                        CtLib.Forms.CtMsgBox.Show("Error", $"{dsc}格式不符'變數名稱' = '變數值'");
                        return false;
                    }

                    string paramName = split[0].Trim();
                    string sParam = split[1].Trim().Replace(";", "");
                    int iParam = 0;
                    float fParam = 0f;
                    bool bParam = false;
                    if (sParam.Contains("\"")) {
                        sParam = sParam.Replace("\"", "");
                        mEditor.WriteParam(paramName, sParam, paramName, sParam);
                    } else if (int.TryParse(sParam, out iParam)) {
                        mEditor.WriteParam(paramName, iParam, paramName, iParam);
                    } else if (float.TryParse(sParam, out fParam)) {
                        mEditor.WriteParam(paramName, fParam, paramName, fParam);
                    } else if (bool.TryParse(sParam, out bParam)) {
                        mEditor.WriteParam(paramName, bParam, paramName, bParam);
                    } else {
                        CtLib.Forms.CtMsgBox.Show("Error", $"{sParam}無法解析");
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 正則產生
        /// </summary>
        /// <remarks>
        /// <{varType}>("{varName}"
        /// </remarks>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool GenerateRegular(string data) {
            Regex reg = new Regex(@"<(?<type>.*?)>\(""(?<name>.*?)""");
            foreach (Match match in reg.Matches(data)) {
                var type = match.Groups["type"].Value;
                var name = match.Groups["name"].Value;
                Console.WriteLine($"{type} {name}");
                switch (type) {
                    case "string":
                        mEditor.WriteParam(name, "", name, "");
                        break;
                    case "int":
                        mEditor.WriteParam(name, 0, name, 0);
                        break;
                    case "bool":
                        mEditor.WriteParam(name, false, name, false);
                        break;
                    case "float":
                        mEditor.WriteParam(name, 0f, name, 0f);
                        break;
                    default:
                        CtLib.Forms.CtMsgBox.Show("Error", $"格式不符");
                        return false;
                }
            }
            return true;
        }

    }
}
