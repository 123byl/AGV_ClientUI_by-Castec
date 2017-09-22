using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Forms;
using CtLib.Module.Ultity;

namespace CtLib.Library {

    /// <summary>Switching thread language and UI text with specify culture resource</summary>
    /// <example>
    /// Here shows a simply language switcher
    /// <code>
    /// /*-- Build the object of CtLanguage --*/
    /// CtLanguage lang = new CtLanguage(this); //Assume "this" is refer to windows form object which need to call methods
    /// 
    /// /*-- [Optional] Change Culture Name --*/
    /// lang.SetCultureName(UILanguage.ENGLISH, "en");  //Set the UILanguage.ENGLISH culture name from "en-US" to "en"
    /// 
    /// /*-- [Optional] Change FontFamily Name --*/
    /// lang.SetFontFamily(UILanguage.ENGLISH, "DengXian"); //Set the UILanguage.ENGLISH font-family from "Candara" to "DengXian"
    /// 
    /// /*-- Change each UI --*/
    /// lang.ChangeUI(UILanguage.ENGLISH);  //Change simply object, like TextBox, Label etc.
    /// lang.ChangeDataGridView(dgv);       //Change special object, like DataGridView, GroupBox an so on. (With each function)
    /// </code></example>
    public class CtLanguage {

        #region Version

        /// <summary>CtLanguage Version Information</summary>
        /// <remarks><code>
        /// 1.0.0  Ahern [2014/11/17]
        ///     + Create and move from old version
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 0, 0, "2014/11/17", "Ahern Kuo");

        #endregion

        #region Declaration - Definitions
        /// <summary>Default FontFamily of each culture</summary>
        private static readonly Dictionary<UILanguage, string> LANG_FONTFAMILY = new Dictionary<UILanguage, string> {
            {UILanguage.ENGLISH, "Candara"},
            {UILanguage.TRADITIONAL_CHINESE, "微軟正黑體"},
            {UILanguage.SIMPLIFIED_CHINESE, "SimSun"},
        };

        /// <summary>Default culture name of each culture</summary>
        private static readonly Dictionary<UILanguage, string> LANG_CULTURE_NAME = new Dictionary<UILanguage, string> {
            {UILanguage.ENGLISH, "en-US"},
            {UILanguage.TRADITIONAL_CHINESE, "zh-TW"},
            {UILanguage.SIMPLIFIED_CHINESE, "zh-CN"},
        };

        /// <summary>The type that want to change UI Text</summary>
        /// <remarks>NOTICE: It should not add "Label", cause it judge from code </remarks>
        private static readonly List<Type> mChangeType = new List<Type> { typeof(Button), typeof(CheckBox) };
        #endregion

        #region Declaration - Properties
        /// <summary>Get or set current culture name, like "zh-TW" "en-US"</summary>
        public UILanguage CurrentCulture { get; set; }
        #endregion

        #region Declaration - Members
        /// <summary>Temporary of Form that modified language</summary>
        private Form mForm;
        /// <summary>ResourceManager of specify Form</summary>
        private ResourceManager mResource;

        /// <summary>FontFamilies</summary>
        private Dictionary<UILanguage, string> mFontFamily = new Dictionary<UILanguage, string>(LANG_FONTFAMILY);
        /// <summary>Culture names</summary>
        private Dictionary<UILanguage, string> mCultureName = new Dictionary<UILanguage, string>(LANG_CULTURE_NAME);
        #endregion

        #region Function - Constructor
        /// <summary>New a CtLanguage with specify Form and default language "zh-TW"</summary>
        /// <param name="form">The Form that want to modify</param>
        public CtLanguage(Form form) {
            mForm = form;
            CurrentCulture = UILanguage.TRADITIONAL_CHINESE;
        }

        /// <summary>New a CtLanguage with specify Form and language</summary>
        /// <param name="form">The Form that want to modify</param>
        /// <param name="lang">The language culture string as initial. ex. "zh-TW", "en-US"</param>
        public CtLanguage(Form form, UILanguage lang) {
            mForm = form;
            CurrentCulture = lang;
        }
        #endregion

        #region Function - Methods
        /// <summary>Combine root name and form name. Using to get resource file path</summary>
        /// <param name="form">The form that want to change language</param>
        /// <returns>Full path of the resource file</returns>
        private string CombineRootName(Form form) {
            string strTemp = "";
            strTemp = form.GetType().Namespace + "." + form.Name;
            return strTemp;
        }

        /// <summary>Get new font with current culture font family name</summary>
        /// <param name="font">The original Font object</param>
        /// <returns>Font with current culture font failmy</returns>
        private Font GetNewFont(Font font) {
            return new Font(mFontFamily[CurrentCulture], font.Size, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
        }

        /// <summary>
        /// Change the ToolStripItem Text with current resource
        /// <para>[Warning] Now added this function into <see cref="ChangeUI()"/>, so there are useless of this function in genernal</para>
        /// </summary>
        /// <param name="toolStrip">The ToolStripItem that want to modify</param>
        private void ChangeToolStrip(ToolStripItem toolStrip) {
            /*-- If the object is Seperator, it dosen't need change culture --*/
            if (toolStrip.GetType() != typeof(ToolStripSeparator)) {
                /* Create the ToolStripMenuItem, 'cause the MenuItem contains the DropDownItems, could use to chagne sub drop-down item */
                ToolStripMenuItem item = Convert.ChangeType(toolStrip, typeof(ToolStripMenuItem)) as ToolStripMenuItem;

                /* Change the ToolStrip font */
                toolStrip.Font = GetNewFont(toolStrip.Font);
                
                /* Chagne the ToolStrip Text */
                toolStrip.Text = mResource.GetString(toolStrip.Name + ".Text");

                /* If there are exist DropDownItem, change it */
                foreach (ToolStripItem stripItem in item.DropDownItems) {
                    ChangeToolStrip(stripItem);
                }
            }
        }

        /// <summary>Change th GroupBox Text and font with current resuorce</summary>
        /// <param name="groupBox">The GroupBox that want to modify</param>
        public void ChangeGroupBox(GroupBox groupBox) {
            /*-- Change the Font --*/
            groupBox.Font = GetNewFont(groupBox.Font);
            /*-- Chagne the Text with loaded resource --*/
            groupBox.Text = mResource.GetString(groupBox.Name + ".Text");
        }

        /// <summary>Change the ComboBox Text and Font with current resource</summary>
        /// <param name="comboBox">The ComboBox that want to modify</param>
        public void ChangeComboBox(ComboBox comboBox) {
            string strTemp = "";                    //Use for resource full path
            int itemCount = comboBox.Items.Count;   //Record the count BKFore we change it
            CtInvoke.ComboBoxClear(comboBox);       //Clear the ComboBox items, it will re-add with specify culture resource
            for (int i = 0; i < itemCount; i++) {
                /* Get the string from resource */
                strTemp = mResource.GetString(comboBox.Name + CtConst.CHR_PERIOD + "Items" + ((i > 0) ? i.ToString() : ""));
                /* Re-Added */
                CtInvoke.ComboBoxAdd(comboBox, strTemp);
            }
        }

        /// <summary>Change the DataGridView each ColumnHeader Text and Font with current resource</summary>
        /// <param name="dgv">The DataGridView that want to modify</param>
        public void ChangeDataGridView(DataGridView dgv) {
            if (mResource == null) ChangeUI();
            for (int i = 0; i < dgv.Columns.Count; i++) {
                dgv.Columns[i].HeaderText = mResource.GetString(dgv.Columns[i].Name + ".HeaderText");
                dgv.ColumnHeadersDefaultCellStyle.Font = GetNewFont(dgv.ColumnHeadersDefaultCellStyle.Font);
            }
        }

        /// <summary>Search the Control object and its sub item then change Text and Font after get the object</summary>
        /// <param name="ctrl">Control object that want to modify</param>
        private void FindSubControl(Control ctrl) {
            /*-- Check object contain sub-items or not --*/
            if (ctrl.Controls.Count > 0) {
                foreach (Control subCtrl in ctrl.Controls) {
                    FindSubControl(subCtrl);    //if exist sub items, recursive call
                }
            } else {
                /* If the object is ToolStrip (or inherit), using change ToolStrip function */
                if (ctrl is ToolStrip) {
                    ToolStrip toolStrip = ctrl as ToolStrip;
                    foreach (ToolStripItem item in toolStrip.Items) {
                        ChangeToolStrip(item);
                    }

                    /* If the object is MenuStrip, change it! There are different object in C# but same in VB */
                } else if (ctrl is MenuStrip) {
                    MenuStrip toolStrip = ctrl as MenuStrip;
                    foreach (ToolStripItem item in toolStrip.Items) {
                        ChangeToolStrip(item);
                    }

                    /* If TextBox or ComboBox or Label(without "lbT"), skip it. It should use other function */
                } else if (((ctrl is Label) && ctrl.Name.StartsWith("lbT")) || mChangeType.Contains(ctrl.GetType())) {
                    ctrl.Text = mResource.GetString(ctrl.Name + ".Text");
                    ctrl.Font = GetNewFont(ctrl.Font);
                }
            }
        }

        /// <summary>Find items lies on Form, and modify it</summary>
        private void ChangeLanguage() {
            foreach (Control ctrl in mForm.Controls) {
                FindSubControl(ctrl);
            }
        }
        #endregion

        #region Function - Core
        /// <summary>Change the culture of current thread and modify UI Text and Font
        /// <para>Please set <see cref="CurrentCulture"/> property BKFore execute this function</para>
        /// </summary>
        public void ChangeUI() {
            /*-- Get the ResourceManager to manage resource --*/
            mResource = new ResourceManager(CombineRootName(mForm), mForm.GetType().Assembly);

            /*-- Change the Thread culture --*/
            Thread.CurrentThread.CurrentCulture = new CultureInfo(mCultureName[CurrentCulture]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(mCultureName[CurrentCulture]);

            /*-- Modify UI --*/
            ChangeLanguage();
        }

        /// <summary>Change the culture of current thread and modify UI Text and Font with specify Language</summary>
        /// <param name="lang">The UI language that want to change</param>
        public void ChangeUI(UILanguage lang) {
            CurrentCulture = lang;
            ChangeUI();
        }

        /// <summary>Get the culture name with <see cref="CurrentCulture"/> property</summary>
        /// <returns>Culture Name that requested</returns>
        public string GetCultureName() {
            return mCultureName[CurrentCulture];
        }

        /// <summary>Get culture name</summary>
        /// <param name="lang">The UI language that you want to request</param>
        /// <returns>Culture name that requested</returns>
        public string GetCultureName(UILanguage lang) {
            return mCultureName[lang];
        }

        /// <summary>Set the culture name with <see cref="CurrentCulture"/> property</summary>
        /// <param name="culture">The culture name that want to change, like "en" "en-US" "en-UK"</param>
        public void SetCultureName(string culture) {
            mCultureName[CurrentCulture] = culture;
        }

        /// <summary>Set culture name</summary>
        /// <param name="lang">UI language</param>
        /// <param name="culture">The culture name that want to change, like "zh" "zh-TW" "zh-Hant"</param>
        public void SetCultureName(UILanguage lang, string culture) {
            mCultureName[lang] = culture;
        }

        /// <summary>Get FontFamily name with <see cref="CurrentCulture"/> property</summary>
        /// <returns>FontFamily name</returns>
        public string GetFontFamily() {
            return mFontFamily[CurrentCulture];
        }

        /// <summary>Get FontFamily name</summary>
        /// <param name="lang">UI Language requested</param>
        /// <returns>FontFamily name</returns>
        public string GetFontFamily(UILanguage lang) {
            return mFontFamily[lang];
        }

        /// <summary>Set FontFamily name with <see cref="CurrentCulture"/> property</summary>
        /// <param name="fontName">FontFamily name that want to save. like "Candara" "微軟正黑體"</param>
        public void SetFontFamily(string fontName) {
            mFontFamily[CurrentCulture] = fontName;
        }

        /// <summary>Set FontFamily name</summary>
        /// /// <param name="lang">UI language</param>
        /// <param name="fontName">FontFamily name that want to save. like "Candara" "微軟正黑體"</param>
        public void SetFontFamily(UILanguage lang, string fontName) {
            mFontFamily[lang] = fontName;
        }
        #endregion
    }
}
