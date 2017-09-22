using System;
using System.Collections.Generic;
using System.Drawing;

using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Ultity;

namespace CtLib.Forms
{
    /// <summary>
    /// CASTEC Style 輸入對話視窗
    /// <para>建議使用 "Start" 方法取代 ShowDialog 來啟動，可直接回傳訊息</para>
    /// </summary>
    /// <example>
    /// 以下示範常見的三種應用方式，應用上提供多種多載(Overload)，可依需求進行套用
    /// 
    /// 1. 可輸入單行字串，可用於如 密碼、產品名稱 等單一性質字串
    /// <code>
    /// CtInput input = new CtInput();
    /// 
    /// string strSigText;
    /// Stat stt = input.Start(InputStyle.TEXT, "標題", "說明文字", out strSigText, "預設文字");
    /// if (stt = Stat.SUCCESS)
    ///     MessageBox(strSigText);     //e.g. "你好"
    /// else
    ///     MessageBox("Cancel");       //使用者取消
    /// 
    /// input.Dispose();
    /// </code>
    /// 
    /// 2. 可輸入多行字串，回傳文字以 NewLine 分隔
    /// <code>
    /// CtInput input = new CtInput();
    /// 
    /// string strMultiText;
    /// Stat stt = input.Start(InputStyle.MULTILINE_TEXT, "標題", "說明文字", "預設文字");
    /// if (stt = Stat.SUCCESS)
    ///     MessageBox(strMultiText);   //e.g. "你好 \r\n 我是 Ahern \r\n 請多指教!"
    /// else
    ///     MessageBox("Cancel");       //使用者取消
    /// 
    /// input.Dispose();
    /// </code>
    /// 
    /// 3. 透過下拉式選單(ComboBox)讓使用者選擇
    /// <code>
    /// CtInput input = new CtInput();
    /// 
    /// List(Of string) strProduct = new List(Of string) { "產品A", "產品B", "產品C" };
    /// string strCombo;
    /// Stat stt = input.Start(InputStyle.COMBOBOX_LIST, "標題", "說明文字", strProduct);
    /// if (stt = Stat.SUCCESS)
    ///     MessageBox(strCombo);       //e.g. "產品B"
    /// else
    ///     MessageBox("Cancel");       //使用者取消
    /// 
    /// input.Dispose();
    /// </code>
    /// </example>
    public partial class CtInput : Form, ICtVersion
    {

        #region Version

        /// <summary>CtInput 版本訊息</summary>
        /// <remarks><code>
        /// 1.0.0  Ahern [2014/09/15]
        ///		+ 建立介面與基礎Function
        /// 
        /// 1.0.1  Ahern [2014/09/18]
        ///		\ 補上Button相關事件
        /// 
        /// 1.0.2  Ahern [2014/10/15]
        ///		+ Start可帶有預設數值
        /// 
        /// 1.0.3  Ahern [2014/10/28]
        ///		\ 調整zh-TW之比例，避免TextBox被切掉
        /// 
        /// 1.0.4  Ahern [2014/11/01]
        ///		+ 新增ComboBox選項
        ///		\ Start以符合含有ComboBox樣式
        ///   
        /// 1.0.5  Ahern [2015/02/11]
        ///		\ Start 改以 Stat 回傳，修正不知道使用者是輸入空字串還是按下取消之問題
        /// 
        /// 1.0.6  Ahern [2015/11/23]
        ///		+ 新增 Password 屬性
        /// 
        /// </code></remarks>
        public CtVersion Version { get { return new CtVersion(1, 0, 6, "2015/11/23", "Ahern Kuo"); } }

        #endregion

        #region Declaration - Enumeration

        /// <summary>CtInput 介面樣式</summary>
        public enum InputStyle : byte
        {
            /// <summary>單行文字</summary>
            TEXT,
            /// <summary>多行文字</summary>
            MULTILINE_TEXT,
            /// <summary>下拉式選單</summary>
            COMBOBOX_LIST
        }

        #endregion

        #region Declaration - Properties
        /// <summary>取得或設定是否允許使用者輸入多行文字</summary>
        public bool MultiLine
        {
            set
            {
                txtInput.Multiline = value;
                if (value)
                {
                    txtInput.Size = new Size(275, 75);
                    this.AcceptButton = null;
                }
                else
                {
                    txtInput.Size = new Size(275, 22);
                }
            }
        }

        /// <summary>讀取使用者輸入之文字</summary>
        public string Result
        {
            get;
            private set;
        }

        /// <summary>取得或設定使否使用顯示輸入內容</summary>
        public bool Password
        {
            get { return txtInput.PasswordChar == char.MinValue ? false : true; }
            set { txtInput.PasswordChar = value ? '*' : char.MinValue; }
        }
        #endregion

        #region Function - Constructor
        /// <summary>
        /// CASTEC Style 輸入對話視窗
        /// <para>建議使用 "Start" 方法取代 ShowDialog 來啟動，可直接回傳訊息</para>
        /// </summary>
        public CtInput()
        {
            InitializeComponent();
        }
        #endregion

        #region Function - Core

        /// <summary>啟動並更新相關訊息，等待使用者輸入後回傳</summary>
        /// <param name="style">介面樣式</param>
        /// <param name="formText">表單標題</param>
        /// <param name="info">說明訊息</param>
        /// <param name="returned">使用者輸入的字串</param>
        /// <param name="value">預設數值</param>
        /// <returns>使用者輸入完畢後回傳 <see cref="Stat.SUCCESS"/>，如果使用者取消則回傳 <seealso cref="Stat.WN_SYS_USRCNC"/></returns>
        public Stat Start(InputStyle style, string formText, string info, out string returned, string value = "")
        {
            Stat stt = Stat.SUCCESS;
            string strReturn = "";
            CtInvoke.FormText(this, formText);
            CtInvoke.LabelText(lbInfo, info);

            switch (style)
            {
                case InputStyle.TEXT:
                    this.AcceptButton = btnOK;
                    txtInput.Size = new Size(275, 22);
                    txtInput.Multiline = false;
                    CtInvoke.TextBoxText(txtInput, value);
                    CtInvoke.TextBoxVisible(txtInput, true);
                    CtInvoke.ComboBoxVisible(cbItem, false);
                    break;
                case InputStyle.MULTILINE_TEXT:
                    this.AcceptButton = null;
                    txtInput.Size = new Size(275, 75);
                    txtInput.Multiline = true;
                    CtInvoke.TextBoxText(txtInput, value);
                    CtInvoke.TextBoxVisible(txtInput, true);
                    CtInvoke.ComboBoxVisible(cbItem, false);
                    break;
                case InputStyle.COMBOBOX_LIST:
                    this.AcceptButton = null;
                    CtInvoke.ComboBoxClear(cbItem);
                    CtInvoke.TextBoxVisible(txtInput, false);
                    CtInvoke.ComboBoxAdd(cbItem, value);
                    CtInvoke.ComboBoxSelectedIndex(cbItem, 0);
                    CtInvoke.ComboBoxVisible(cbItem, true);
                    break;
                default:
                    break;
            }

            this.ShowDialog();

            if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if (style == InputStyle.COMBOBOX_LIST)
                {
                    strReturn = cbItem.SelectedItem.ToString();
                }
                else
                {
                    strReturn = txtInput.Text;
                }
            }
            else
            {
                stt = Stat.WN_SYS_USRCNC;
                strReturn = "";
            }

            returned = strReturn;
            return stt;
        }

        /// <summary>啟動並更新相關訊息，等待使用者輸入後回傳</summary>
        /// <param name="style">介面樣式</param>
        /// <param name="formText">表單標題</param>
        /// <param name="info">說明訊息</param>
        /// <param name="returned">使用者輸入的字串</param>
        /// <param name="value">預設數值</param>
        /// <returns>使用者輸入完畢後回傳 <see cref="Stat.SUCCESS"/>，如果使用者取消則回傳 <seealso cref="Stat.WN_SYS_USRCNC"/></returns>
        public Stat Start(InputStyle style, string formText, List<string> info, out string returned, string value = "")
        {
            Stat stt = Stat.SUCCESS;
            string strReturn = "";
            CtInvoke.FormText(this, formText);
            CtInvoke.LabelText(lbInfo, info);

            switch (style)
            {
                case InputStyle.TEXT:
                    this.AcceptButton = btnOK;
                    txtInput.Size = new Size(275, 22);
                    txtInput.Multiline = false;
                    CtInvoke.TextBoxText(txtInput, value);
                    CtInvoke.ComboBoxVisible(cbItem, false);
                    break;
                case InputStyle.MULTILINE_TEXT:
                    this.AcceptButton = null;
                    txtInput.Size = new Size(275, 75);
                    txtInput.Multiline = true;
                    CtInvoke.TextBoxText(txtInput, value);
                    CtInvoke.ComboBoxVisible(cbItem, false);
                    break;
                case InputStyle.COMBOBOX_LIST:
                    this.AcceptButton = null;
                    CtInvoke.ComboBoxClear(cbItem);
                    CtInvoke.TextBoxVisible(txtInput, false);
                    CtInvoke.ComboBoxAdd(cbItem, value);
                    CtInvoke.ComboBoxSelectedIndex(cbItem, 0);
                    break;
                default:
                    break;
            }

            this.ShowDialog();

            if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if (style == InputStyle.COMBOBOX_LIST)
                {
                    strReturn = cbItem.SelectedItem.ToString();
                }
                else
                {
                    strReturn = txtInput.Text;
                }
            }
            else
            {
                stt = Stat.WN_SYS_USRCNC;
                strReturn = "";
            }

            returned = strReturn;
            return stt;
        }

        /// <summary>啟動並更新相關訊息，等待使用者輸入後回傳</summary>
        /// <param name="style">介面樣式</param>
        /// <param name="formText">表單標題</param>
        /// <param name="info">說明訊息</param>
        /// <param name="returned">使用者輸入的字串</param>
        /// <param name="value">預設數值</param>
        /// <returns>使用者輸入完畢後回傳 <see cref="Stat.SUCCESS"/>，如果使用者取消則回傳 <seealso cref="Stat.WN_SYS_USRCNC"/></returns>
        public Stat Start(InputStyle style, string formText, string info, out string returned, List<string> value)
        {
            Stat stt = Stat.SUCCESS;
            string strReturn = "";
            CtInvoke.FormText(this, formText);
            CtInvoke.LabelText(lbInfo, info);

            switch (style)
            {
                case InputStyle.TEXT:
                    this.AcceptButton = btnOK;
                    txtInput.Size = new Size(275, 22);
                    txtInput.Multiline = false;
                    CtInvoke.TextBoxText(txtInput, value);
                    CtInvoke.TextBoxVisible(txtInput, true);
                    CtInvoke.ComboBoxVisible(cbItem, false);
                    break;
                case InputStyle.MULTILINE_TEXT:
                    this.AcceptButton = null;
                    txtInput.Size = new Size(275, 75);
                    txtInput.Multiline = true;
                    CtInvoke.TextBoxText(txtInput, value);
                    CtInvoke.TextBoxVisible(txtInput, true);
                    CtInvoke.ComboBoxVisible(cbItem, false);
                    break;
                case InputStyle.COMBOBOX_LIST:
                    this.AcceptButton = null;
                    CtInvoke.ComboBoxClear(cbItem);
                    foreach (string item in value)
                    {
                        CtInvoke.ComboBoxAdd(cbItem, item);
                    }
                    CtInvoke.ComboBoxSelectedIndex(cbItem, 0);
                    CtInvoke.TextBoxVisible(txtInput, false);
                    CtInvoke.ComboBoxVisible(cbItem, true);
                    break;
                default:
                    break;
            }

            this.ShowDialog();

            if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if (style == InputStyle.COMBOBOX_LIST)
                {
                    strReturn = cbItem.SelectedItem.ToString();
                }
                else
                {
                    strReturn = txtInput.Text;
                }
            }
            else
            {
                stt = Stat.WN_SYS_USRCNC;
                strReturn = "";
            }

            returned = strReturn;
            return stt;
        }

        /// <summary>啟動並更新相關訊息，等待使用者輸入後回傳</summary>
        /// <param name="style">介面樣式</param>
        /// <param name="formText">表單標題</param>
        /// <param name="info">說明訊息</param>
        /// <param name="returned">使用者輸入的字串</param>
        /// <param name="value">預設數值</param>
        /// <returns>使用者輸入完畢後回傳 <see cref="Stat.SUCCESS"/>，如果使用者取消則回傳 <seealso cref="Stat.WN_SYS_USRCNC"/></returns>
        public Stat Start(InputStyle style, string formText, List<string> info, out string returned, List<string> value)
        {
            Stat stt = Stat.SUCCESS;
            string strReturn = "";
            CtInvoke.FormText(this, formText);
            CtInvoke.LabelText(lbInfo, info);

            switch (style)
            {
                case InputStyle.TEXT:
                    this.AcceptButton = btnOK;
                    txtInput.Size = new Size(275, 22);
                    txtInput.Multiline = false;
                    CtInvoke.TextBoxText(txtInput, value);
                    CtInvoke.ComboBoxVisible(cbItem, false);
                    break;
                case InputStyle.MULTILINE_TEXT:
                    this.AcceptButton = null;
                    txtInput.Size = new Size(275, 75);
                    txtInput.Multiline = true;
                    CtInvoke.TextBoxText(txtInput, value);
                    CtInvoke.ComboBoxVisible(cbItem, false);
                    break;
                case InputStyle.COMBOBOX_LIST:
                    this.AcceptButton = null;
                    CtInvoke.TextBoxVisible(txtInput, false);
                    CtInvoke.ComboBoxClear(cbItem);
                    foreach (string item in value)
                    {
                        CtInvoke.ComboBoxAdd(cbItem, item);
                    }
                    CtInvoke.ComboBoxSelectedIndex(cbItem, 0);
                    break;
                default:
                    break;
            }

            this.ShowDialog();

            if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if (style == InputStyle.COMBOBOX_LIST)
                {
                    strReturn = cbItem.SelectedItem.ToString();
                }
                else
                {
                    strReturn = txtInput.Text;
                }
            }
            else
            {
                stt = Stat.WN_SYS_USRCNC;
                strReturn = "";
            }

            returned = strReturn;
            return stt;
        }
        #endregion

        #region Function - Interface Event
        /// <summary>當使用者輸入時更新至變數</summary>
        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            Result = txtInput.Text;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion
    }
}
