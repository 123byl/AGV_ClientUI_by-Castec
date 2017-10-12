using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using System.Windows.Forms;

using CtLib.Library;

namespace CtLib.Module.Utility {

	/// <summary>Switching thread language and UI text with specify culture resource</summary>
	/// <example>
	/// Here shows a simply language switcher
	/// <code language="C#">
	/// /*-- Build the object of CtLanguage --*/
	/// CtLanguage lang = new CtLanguage(this); //Assume "this" is refer to windows form object which need to call methods
	/// 
	/// /*-- [Optional] Change Culture Name --*/
	/// lang.SetCultureName(UILanguage.English, "en");  //Set the UILanguage.English culture name from "en-US" to "en"
	/// 
	/// /*-- [Optional] Change FontFamily Name --*/
	/// lang.SetFontFamily(UILanguage.English, "DengXian"); //Set the UILanguage.English font-family from "Candara" to "DengXian"
	/// 
	/// /*-- Change each UI --*/
	/// lang.ChangeUI(UILanguage.English);  //Change simply object, like TextBox, Label etc.
	/// lang.ChangeDataGridView(dgv);       //Change special object, like DataGridView, GroupBox an so on. (With each function)
	/// </code></example>
	public class CtLanguage : ICtVersion {

		#region Version

		/// <summary>CtLanguage Version Information</summary>
		/// <remarks><code language="C#">
		/// 1.0.0  Ahern [2014/11/17]
		///     + Create and move from old version
		///     
		/// 1.0.1  Ahern [2016/01/14]
		///		\ 如果遇更改的文字為空，則不修改
		///		
		/// 1.0.2  Ahern [2016/02/25]
		///		\ 改以 CtInvoke 實作，避免執行緒問題
		///		
		/// 1.1.0  Ahern [2016/06/16]
		///		+ GetUiLangByCult
		///		+ GetMultiLangXmlText
		///		+ Font 改以 Resource 為主，若沒有才以 GetNewFont 取代
		///		\ 重整 FindSubControl 以使用 Type.Name 判斷取代 is 多次轉換判斷
		///		
		/// 1.1.1  Ahern [2016/06/21]
		///		\ ChangeToolStrip 加上 null 保護
		///		
		/// 1.1.2  Ahern [2016/07/19]
		///		+ ContextMenuStrip
		///		
		/// 1.1.3  Ahern [2016/09/23]
		///		\ GetMultiLangXmlText 補上將 @"\r\n" 取代成 "\r\n"
		///		
		/// 1.2.0  Ahern [2017/02/02]
		///		+ GetAllLangXmlText
		/// 
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(1, 2, 0, "2017/02/02", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Definitions
		/// <summary>Default FontFamily of each culture</summary>
		private static readonly Dictionary<UILanguage, string> LANG_FONTFAMILY = new Dictionary<UILanguage, string> {
			{UILanguage.English, "Candara"},
			{UILanguage.TraditionalChinese, "微軟正黑體"},
			{UILanguage.SimplifiedChinese, "SimSun"},
		};

		/// <summary>Default culture name of each culture</summary>
		private static readonly Dictionary<UILanguage, string> LANG_CULTURE_NAME = new Dictionary<UILanguage, string> {
			{UILanguage.English, "en-US"},
			{UILanguage.TraditionalChinese, "zh-TW"},
			{UILanguage.SimplifiedChinese, "zh-CN"},
		};

		/// <summary>The type that want to change UI Text</summary>
		/// <remarks>NOTICE: It should not add "Label", cause it judge from code </remarks>
		private static readonly List<Type> mChangeType = new List<Type> { typeof(Button), typeof(CheckBox), typeof(RadioButton) };
		#endregion

		#region Declaration - Properties
		/// <summary>Get or set current culture name, like "zh-TW" "en-US"</summary>
		public UILanguage CurrentCulture { get; set; }
		#endregion

		#region Declaration - Fields
		/// <summary>Temporary of Form that modified language</summary>
		private Form mForm;
		/// <summary>Temporary of ToolTip hint</summary>
		private ToolTip mTip;
		/// <summary>ResourceManager of specify Form</summary>
		private ResourceManager mResource;
		/// <summary>Temporary of ContextMenuStrip</summary>
		private IEnumerable<ContextMenuStrip> mCntxColl;

		/// <summary>FontFamilies</summary>
		private Dictionary<UILanguage, string> mFontFamily = new Dictionary<UILanguage, string>(LANG_FONTFAMILY);
		/// <summary>Culture names</summary>
		private Dictionary<UILanguage, string> mCultureName = new Dictionary<UILanguage, string>(LANG_CULTURE_NAME);

		private bool mFlag_ExistTip = false;
		#endregion

		#region Function - Constructor
		/// <summary>New a CtLanguage with specify Form and default language "zh-TW"</summary>
		/// <param name="form">The Form that want to modify</param>
		public CtLanguage(Form form) {
			mForm = form;
			CurrentCulture = UILanguage.TraditionalChinese;

			CheckToolTip();
			CheckContextMenuStrip();
		}

		/// <summary>New a CtLanguage with specify Form and language</summary>
		/// <param name="form">The Form that want to modify</param>
		/// <param name="lang">The language culture string as initial. ex. "zh-TW", "en-US"</param>
		public CtLanguage(Form form, UILanguage lang) {
			mForm = form;
			CurrentCulture = lang;

			CheckToolTip();
			CheckContextMenuStrip();
		}
		#endregion

		#region Function - Private Methods

		/// <summary>Using Assembly to find whether specified form have ToolTip</summary>
		private void CheckToolTip() {
			IEnumerable<FieldInfo> infoColl = mForm.GetType().GetRuntimeFields().Where(val => val.FieldType == typeof(ToolTip));
			if (infoColl != null && infoColl.Any()) {
				mTip = infoColl.ElementAt(0).GetValue(mForm) as ToolTip;
				if (mTip != null) mFlag_ExistTip = true;
			}
		}

		/// <summary>Using Assembly to find whether specified form have ContextMenuStrip</summary>
		private void CheckContextMenuStrip() {
			IEnumerable<FieldInfo> infoColl = mForm.GetType().GetRuntimeFields().Where(val => val.FieldType == typeof(ContextMenuStrip));
			if (infoColl != null && infoColl.Any()) {
				mCntxColl = infoColl.Select(
					cntx => cntx.GetValue(mForm) as ContextMenuStrip
				);
			}
		}

		/// <summary>Combine root name and form name. Using to get resource file path</summary>
		/// <param name="form">The form that want to change language</param>
		/// <returns>Full path of the resource file</returns>
		private string CombineRootName(Form form) {
			return string.Format("{0}.{1}", form.GetType().Namespace, form.Name);
		}

		/// <summary>Get new font with current culture font family name</summary>
		/// <param name="font">The original Font object</param>
		/// <returns>Font with current culture font failmy</returns>
		private Font GetNewFont(Font font) {
			return new Font(mFontFamily[CurrentCulture], font.Size, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
		}

		/// <summary>
		/// Change the ToolStripItem Text with current resource
		/// <para>[Warning] Now added this function into <see cref="ChangeUI()"/>, so there is useless of calling this function</para>
		/// </summary>
		/// <param name="toolStrip">The ToolStripItem that want to modify</param>
		private void ChangeToolStrip(ToolStripItem toolStrip) {
			/*-- If the object is Seperator, it dosen't need change culture --*/
			if (toolStrip.GetType() != typeof(ToolStripSeparator)) {
				/* Change the ToolStrip font */
				Font resxFont = mResource.GetObject(toolStrip.Name + ".Font") as Font;
				if (resxFont == null) CtInvoke.ToolStripItemFont(toolStrip, GetNewFont(toolStrip.Owner.Font));
				else CtInvoke.ToolStripItemFont(toolStrip, resxFont);

				/* Chagne the ToolStrip Text */
				CtInvoke.ToolStripItemText(toolStrip, mResource.GetString(toolStrip.Name + ".Text"));

				/* Create the ToolStripMenuItem, 'cause the MenuItem contains the DropDownItems, could use to chagne sub drop-down item */
				ToolStripMenuItem menuItem = toolStrip as ToolStripMenuItem;

				/* If there are exist DropDownItem, change it */
				if (menuItem != null) {
					foreach (ToolStripItem stripItem in menuItem.DropDownItems) {
						ChangeToolStrip(stripItem);
					}
				}
			}
		}

		/// <summary>
		/// Change th GroupBox Text and font with current resuorce
		/// <para>[Warning] Now added this function into <see cref="ChangeUI()"/>, so there is useless of calling this function</para>
		/// </summary>
		/// <param name="groupBox">The GroupBox that want to modify</param>
		private void ChangeGroupBox(GroupBox groupBox) {
			/*-- Change the Font --*/
			Font resxFont = mResource.GetObject(groupBox.Name + ".Font") as Font;
			if (resxFont == null) CtInvoke.ControlFont(groupBox, GetNewFont(groupBox.Font));
			else CtInvoke.ControlFont(groupBox, resxFont);

			/*-- Chagne the Text with loaded resource --*/
			CtInvoke.ControlText(groupBox, mResource.GetString(groupBox.Name + ".Text"));
		}

		/// <summary>
		/// Change the ComboBox Text and Font with current resource
		/// <para>[Warning] Now added this function into <see cref="ChangeUI()"/>, so there is useless of calling this function</para>
		/// </summary>
		/// <param name="comboBox">The ComboBox that want to modify</param>
		private void ChangeComboBox(ComboBox comboBox) {
			string strTemp = "";                                        //Use for resource full path
			strTemp = mResource.GetString(comboBox.Name + ".Items");    //Ensure there have culture resource
			if (!string.IsNullOrEmpty(strTemp)) {
				int itemCount = comboBox.Items.Count;   //Record the count Before we change it
				CtInvoke.ComboBoxClear(comboBox);       //Clear the ComboBox items, it will re-add with specify culture resource
				for (int i = 0; i < itemCount; i++) {
					/* Get the string from resource */
					strTemp = mResource.GetString(comboBox.Name + ".Items" + ((i > 0) ? i.ToString() : ""));
					/* Re-Added */
					CtInvoke.ComboBoxAdd(comboBox, strTemp);
				}

				/* Change font */
				Font resxFont = mResource.GetObject(comboBox.Name + ".Font") as Font;
				if (resxFont != null) CtInvoke.ControlFont(comboBox, resxFont);
				else CtInvoke.ControlFont(comboBox, GetNewFont(comboBox.Font));
			}
		}

		/// <summary>
		/// Change the DataGridView each ColumnHeader Text and Font with current resource
		/// <para>[Warning] Now added this function into <see cref="ChangeUI()"/>, so there is useless of calling this function</para>
		/// </summary>
		/// <param name="dgv">The DataGridView that want to modify</param>
		private void ChangeDataGridView(DataGridView dgv) {
			if (mResource == null) ChangeUI();
			string tipStr = string.Empty;
			for (int i = 0; i < dgv.Columns.Count; i++) {
				dgv.InvokeIfNecessary(
					() => {
						dgv.Columns[i].HeaderText = mResource.GetString(dgv.Columns[i].Name + ".HeaderText");

						tipStr = mResource.GetString(dgv.Columns[i].Name + ".ToolTipText");
						if (!string.IsNullOrEmpty(tipStr)) dgv.Columns[i].ToolTipText = tipStr.Replace(@"\r\n", Environment.NewLine);
					}
				);
			}
		}

		/// <summary>Search the Control object and its sub item then change Text and Font after get the object</summary>
		/// <param name="ctrl">Control object that want to modify</param>
		private void FindSubControl(Control ctrl) {
			/*-- Dismantle control and change it --*/
			string ctrlType = ctrl.GetType().Name;
			switch (ctrlType) {
				case "ComboBox":
					ComboBox comBox = ctrl as ComboBox;
					ChangeComboBox(comBox);
					break;
				case "DataGridView":
					DataGridView dgv = ctrl as DataGridView;
					ChangeDataGridView(dgv);
					break;
				case "GroupBox":
					GroupBox groupBox = ctrl as GroupBox;
					ChangeGroupBox(groupBox);
					break;
				case "MenuStrip":
					MenuStrip menuStrip = ctrl as MenuStrip;

					Font msFont = mResource.GetObject(menuStrip.Name + ".Font") as Font;
					if (msFont != null) CtInvoke.ControlFont(menuStrip, msFont);

					foreach (ToolStripItem item in menuStrip.Items) {
						ChangeToolStrip(item);
					}
					break;
				case "ToolStrip":
					ToolStrip toolStrip = ctrl as ToolStrip;

					Font tsFont = mResource.GetObject(toolStrip.Name + ".Font") as Font;
					if (tsFont != null) CtInvoke.ControlFont(toolStrip, tsFont);

					foreach (ToolStripItem item in toolStrip.Items) {
						ChangeToolStrip(item);
					}
					break;
				case "ContextMenuStrip":
					ContextMenuStrip cntxMenu = ctrl as ContextMenuStrip;

					Font cmFont = mResource.GetObject(cntxMenu.Name + ".Font") as Font;
					if (cmFont != null) CtInvoke.ControlFont(cntxMenu, cmFont);

					foreach (ToolStripItem item in cntxMenu.Items) {
						ChangeToolStrip(item);
					}
					break;
				default:
					if (((ctrl is Label) && ctrl.Name.StartsWith("lbT")) || mChangeType.Contains(ctrl.GetType())) {
						string txt = mResource.GetString(ctrl.Name + ".Text");
						if (!string.IsNullOrEmpty(txt)) {
							CtInvoke.ControlText(ctrl, txt);
						}

						Font resxFont = mResource.GetObject(ctrl.Name + ".Font") as Font;
						if (resxFont == null) CtInvoke.ControlFont(ctrl, GetNewFont(ctrl.Font));
						else CtInvoke.ControlFont(ctrl, resxFont);
					}
					break;
			}

			/*-- Check is it contains tooltip --*/
			if (mFlag_ExistTip) {
				string txt = mResource.GetString(ctrl.Name + ".ToolTip");
				if (!string.IsNullOrEmpty(txt)) CtInvoke.ToolTipSetTip(mTip, ctrl, txt.Replace("\\r","\r").Replace("\\n","\n"));
			}

			/*-- Check childrens --*/
			if (ctrl.HasChildren) {
				foreach (Control subCtrl in ctrl.Controls) {
					FindSubControl(subCtrl);    //if exist sub items, recursive call
				}
			}
		}

		/// <summary>Find items lies on Form, and modify it</summary>
		private void ChangeLanguage() {
			/*-- Search and modify controls --*/
			foreach (Control ctrl in mForm.Controls) {
				FindSubControl(ctrl);
			}

			/*-- Through ContextMenuStrip is inherited Control, but it not add to Form.Controls --*/
			if (mCntxColl != null && mCntxColl.Any()) {
				mCntxColl.ForEach(
					cntx => FindSubControl(cntx)
				);
			}
		}
		#endregion

		#region Function - Core

		#region User Interface
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
		#endregion

		#region Culture Settings
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
		#endregion

		#region Culture Fonts
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

		#region Culture To UILanguage

		/// <summary>根據目前 <see cref="Thread"/> 的文化特性來取得相對應的 <see cref="UILanguage"/></summary>
		/// <returns>文化特性相對應的 <see cref="UILanguage"/></returns>
		public static UILanguage GetUiLangByCult() {
			return GetUiLangByCult(Thread.CurrentThread.CurrentUICulture.Name);
		}

		/// <summary>取得文化特性相對應的 <see cref="UILanguage"/></summary>
		/// <param name="cult">欲轉換的文化特性</param>
		/// <returns>文化特性相對應的 <see cref="UILanguage"/></returns>
		public static UILanguage GetUiLangByCult(string cult) {
			UILanguage lang = UILanguage.TraditionalChinese;
			switch (cult.ToLower()) {
				case "zh-tw":
				case "zh-cht":
				case "zh-hant":
				case "zh-hant-tw":
				case "zh-hant-hk":
					lang = UILanguage.TraditionalChinese;
					break;
				case "zh-cn":
				case "zh-chs":
				case "zh-hans":
				case "zh-hans-cn":
				case "zh-hans-hk":
					lang = UILanguage.SimplifiedChinese;
					break;
				default:
					lang = UILanguage.English;
					break;
			}

			return lang;
		}

		#endregion

		#region Multi-Language Text
		private static string GetLangCultMap(UILanguage lang) {
			string cult = null;
			switch (lang) {
				case UILanguage.TraditionalChinese:
					cult = "zh_Hant";
					break;
				case UILanguage.SimplifiedChinese:
					cult = "zh_Hans";
					break;
				case UILanguage.English:
					cult = "en";
					break;
				default:
					break;
			}
			return cult;
		}

		/// <summary>取得符合內嵌多語系 XML 檔案之當前語系文字，使用 ID 進行搜尋</summary>
		/// <param name="resxName">含有多語系之內嵌 XML 資源名稱，如 "Language.xml"</param>
		/// <param name="id">欲取得的 ID 資訊</param>
		/// <returns>對應於 ID 的語系文字</returns>
		public static Dictionary<int, string> GetMultiLangXmlText(string resxName, params int[] id) {
			string culture = GetLangCultMap(GetUiLangByCult());
			IEnumerable<string> idColl = id.Select(val => val.ToString());
			Stream stream = CtEmbdResx.GetEmbdResx(resxName);
			XDocument xmlDoc = XDocument.Load(stream);
			return xmlDoc.Root?
				.Elements("Data")?
				.Where(xmlNode => idColl.Contains(xmlNode.Attribute("ID").Value))?
				.ToDictionary(
					tarEl => int.Parse(tarEl.Attribute("ID").Value),
					tarEl => tarEl.Element(culture).Value.Replace(@"\r\n", "\r\n")
				);
		}

		/// <summary>取得符合內嵌多語系 XML 檔案之特定語系文字，使用 ID 進行搜尋</summary>
		/// <param name="resxName">含有多語系之內嵌 XML 資源名稱，如 "Language.xml"</param>
		/// <param name="lang">欲取得的目標語系</param>
		/// <param name="id">欲取得的 ID 資訊</param>
		/// <returns>對應於 ID 的語系文字</returns>
		public static Dictionary<int, string> GetMultiLangXmlText(string resxName, UILanguage lang, params int[] id) {
			string culture = GetLangCultMap(lang);
			IEnumerable<string> idColl = id.Select(val => val.ToString());
			Stream stream = CtEmbdResx.GetEmbdResx(resxName);
			XDocument xmlDoc = XDocument.Load(stream);
			return xmlDoc.Root?
				.Elements("Data")?
				.Where(xmlNode => idColl.Contains(xmlNode.Attribute("ID").Value))?
				.ToDictionary(
					tarEl => int.Parse(tarEl.Attribute("ID").Value),
					tarEl => tarEl.Element(culture).Value.Replace(@"\r\n", "\r\n")
				);
		}

		/// <summary>取得符合內嵌多語系 XML 檔案之當前語系文字，使用關鍵字進行搜尋</summary>
		/// <param name="resxName">含有多語系之內嵌 XML 資源名稱，如 "Language.xml"</param>
		/// <param name="keyWord">欲取得的關鍵字</param>
		/// <returns>對應於 ID 的語系文字</returns>
		public static Dictionary<string, string> GetMultiLangXmlText(string resxName, params string[] keyWord) {
			string culture = GetLangCultMap(GetUiLangByCult());
			Stream stream = CtEmbdResx.GetEmbdResx(resxName);
			XDocument xmlDoc = XDocument.Load(stream);
			return xmlDoc.Root?
				.Elements("Data")?
				.Where(xmlNode => keyWord.Contains(xmlNode.Attribute("KeyWord").Value))?
				.ToDictionary(
					tarEl => tarEl.Attribute("KeyWord").Value,
					tarEl => tarEl.Element(culture).Value.Replace(@"\r\n", "\r\n")
				);
		}

		/// <summary>取得符合內嵌多語系 XML 檔案之特定語系文字，使用關鍵字進行搜尋</summary>
		/// <param name="resxName">含有多語系之內嵌 XML 資源名稱，如 "Language.xml"</param>
		/// <param name="lang">欲取得的目標語系</param>
		/// <param name="keyWord">欲取得的關鍵字</param>
		/// <returns>對應於 ID 的語系文字</returns>
		public static Dictionary<string, string> GetMultiLangXmlText(string resxName, UILanguage lang, params string[] keyWord) {
			string culture = GetLangCultMap(lang);
			Stream stream = CtEmbdResx.GetEmbdResx(resxName);
			XDocument xmlDoc = XDocument.Load(stream);
			return xmlDoc.Root?
				.Elements("Data")?
				.Where(xmlNode => keyWord.Contains(xmlNode.Attribute("KeyWord").Value))?
				.ToDictionary(
					tarEl => tarEl.Attribute("KeyWord").Value,
					tarEl => tarEl.Element(culture).Value.Replace(@"\r\n", "\r\n")
				);
		}

		/// <summary>從 <see cref="XElement"/> 中取得 ID 數值</summary>
		/// <param name="elmt">欲處理之 <see cref="XElement"/></param>
		/// <returns>取得對應的數值</returns>
		private static object GetLangXmlTextById(XElement elmt) {
			return int.Parse(elmt.Attribute("ID").Value);
		}

		/// <summary>從 <see cref="XElement"/> 中取得 KeyWord 數值</summary>
		/// <param name="elmt">欲處理之 <see cref="XElement"/></param>
		/// <returns>取得對應的數值</returns>
		private static object GetLangXmlTextByKey(XElement elmt) {
			return elmt.Attribute("KeyWord").Value;
		}

		/// <summary>[委派] 從 <see cref="XElement"/> 取得指定數值之方法</summary>
		/// <param name="elmt">欲處理之 <see cref="XElement"/></param>
		/// <returns>取得對應的數值</returns>
		private delegate object GetLangXmlText(XElement elmt);

		/// <summary>取得所有內嵌多語系 XML 檔案之當前語系文字</summary>
		/// <typeparam name="TIdx">欲當 Dictionary 之索引值之類型，可為 <see cref="int"/> 或 <see cref="string"/></typeparam>
		/// <param name="resxName">含有多語系之內嵌 XML 資源名稱，如 "Language.xml"</param>
		/// <returns>所有的該語系對應表</returns>
		public static Dictionary<TIdx, string> GetAllLangXmlText<TIdx>(string resxName) {
			string culture = GetLangCultMap(GetUiLangByCult());
			Stream stream = CtEmbdResx.GetEmbdResx(resxName);
			XDocument xmlDoc = XDocument.Load(stream);

			GetLangXmlText getMethod;
			if (typeof(TIdx) == typeof(int)) getMethod = GetLangXmlTextById;
			else if (typeof(TIdx) == typeof(string)) getMethod = GetLangXmlTextByKey;
			else throw new ArgumentException($"TIdx only can be \"int\" or \"string\". But got \"{typeof(TIdx).Name}\"", "TIdx");

			return xmlDoc.Root?
				.Elements("Data")?
				.ToDictionary(
					tarEl => (TIdx)getMethod(tarEl),
					tarEl => tarEl.Element(culture).Value.Replace(@"\r\n", "\r\n")
				);
		}

		/// <summary>取得所有內嵌多語系 XML 檔案之指定語系文字</summary>
		/// <typeparam name="TIdx">欲當 Dictionary 之索引值之類型，可為 <see cref="int"/> 或 <see cref="string"/></typeparam>
		/// <param name="resxName">含有多語系之內嵌 XML 資源名稱，如 "Language.xml"</param>
		/// <param name="lang">欲取得的目標語系</param>
		/// <returns>所有的該語系對應表</returns>
		public static Dictionary<TIdx, string> GetAllLangXmlText<TIdx>(string resxName, UILanguage lang) {
			string culture = GetLangCultMap(lang);
			Stream stream = CtEmbdResx.GetEmbdResx(resxName);
			XDocument xmlDoc = XDocument.Load(stream);

			GetLangXmlText getMethod;
			if (typeof(TIdx) == typeof(int)) getMethod = GetLangXmlTextById;
			else if (typeof(TIdx) == typeof(string)) getMethod = GetLangXmlTextByKey;
			else throw new ArgumentException($"TIdx only can be \"int\" or \"string\". But got \"{typeof(TIdx).Name}\"", "TIdx");

			return xmlDoc.Root?
				.Elements("Data")?
				.ToDictionary(
					tarEl => (TIdx)getMethod(tarEl),
					tarEl => tarEl.Element(culture).Value.Replace(@"\r\n", "\r\n")
				);
		}
		#endregion

		#endregion
	}
}
