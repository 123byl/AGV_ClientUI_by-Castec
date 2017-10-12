using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using CtLib.Library;
using CtLib.Module.Utility;

namespace CtLib.Module.XML {

	/// <summary>可延伸標記式語言(Extensible Markup Language)文件操作</summary>
	/// <example>
	/// 由於 XML 轉換成 <see cref="XmlElmt"/> 時會將整份文件載入，耗時也會較多一些
	/// <para>但如採用 <see cref="XmlReader"/> 讀取，速度雖快但並無法進行儲存動作(沒有記憶性)</para>
	/// <para>故在操作 XML 文件時，請依實際需求進行使用，以下為幾種常見的情況</para>
	/// <para></para>
	/// <para>以下為示範 XML</para>
	/// <code language="XML">
	/// &lt;?xml version="1.0" encoding="utf-8"?&gt;
	/// &lt;Company&gt;
	///		&lt;SW_Dep Population="4"&gt;
	///			&lt;Manager Sexual="Male"&gt;Psi&lt;/Manager&gt;
	///			&lt;Engineer Sexual="Male"&gt;Alpha&lt;/Engineer&gt;
	///			&lt;Engineer Sexual="Female"&gt;Beta&lt;/Engineer&gt;
	///			&lt;Engineer Sexual="Male"&gt;Gamma&lt;/Engineer&gt;
	///		&lt;/SW_Dep&gt;
	///		&lt;HW_Dep Population="6"&gt;
	///			&lt;Manager Sexual="Male"&gt;Omega&lt;/Manager&gt;
	///			&lt;Engineer Sexual="Male"&gt;Delta&lt;/Engineer&gt;
	///			&lt;Engineer Sexual="Male"&gt;Epsilon&lt;/Engineer&gt;
	///			&lt;Engineer Sexual="Female"&gt;Zeta&lt;/Engineer&gt;
	///			&lt;Engineer Sexual="Male"&gt;Eta&lt;/Engineer&gt;
	///			&lt;Engineer Sexual="Female"&gt;Theta&lt;/Engineer&gt;
	///		&lt;/HW_Dep&gt;
	/// &lt;/Company&gt;
	/// </code>
	/// 1. 整份 XML 文件要尋遍所有節點(全部節點都要判斷、操作)，適用 <see cref="XmlElmt"/> 方式
	/// <code language="C#">
	/// List&lt;Employee&gt; swMembers;
	/// List&lt;Employee&gt; hwMembers;
	/// 
	/// /*-- 載入文件 --*/
	/// XmlElmt company = CtXML.Load(@"D:\Demo.xml");
	/// 
	/// /*-- 將所有的 SW_Dep 子節點轉成 Employee 並存到 swMembers --*/
	/// XmlElmt dep = company.Element("SW_Dep");
	/// swMembers = dep.Elements().ConvertAll(node =&gt; new Employee(node.Value));
	/// 
	/// /*-- 將所有的 HW_Dep 子節點轉成 Employee 並存到 hwMembers --*/
	/// dep = company.Element("HW_Dep");
	/// hwMembers = dep.Elements().ConvertAll(node =&gt; new Employee(node.Value));
	/// </code>
	/// 2. 需要修改、新增或移除節點。以下先示範 <see cref="XmlElmt"/> 方法
	/// <code language="C#">
	/// /*-- 載入文件 --*/
	/// XmlElmt company = CtXML.Load(@"D:\Demo.xml");
	/// 
	/// /*-- 找出 SW_Dep 的 Beta 這個員工 --*/
	/// XmlElmt swDep = company.Element("SW_Dep");
	/// XmlElmt beta = swDep.Elements().Find(eng =&gt; "Beta".Equals(eng.Value));
	/// 
	/// /*-- 開除 Beta --*/
	/// swDep.RemoveElement(beta);
	/// 
	/// /*-- 補進新人 Kappa --*/
	/// swDep.Add(
	///		new XmlElmt(
	///			"Engineer",
	///			"Kappa",
	///			new XmlAttr("Sexual", "Male")
	///		)
	/// );
	/// 
	/// /*-- 更新資訊 --*/
	/// company.RemoveElement("SW_Dep");
	/// company.Add(swDep);
	/// 
	/// /*-- 存檔 --*/
	/// company.Save(@"D:\Demo_1.xml");
	/// </code>
	/// 3. 承2，如果你追求高效率，請使用 <see cref="XDocument"/> 進行操作
	/// <para>    <see cref="XDocument"/> 等相關方法，其效率比 <see cref="XmlDocument"/> 稍高</para>
	/// <para>    可再搭配 System.Xml.XPath 相關擴充，讓 <see cref="XDocument"/> 更加的有彈性</para>
	/// <para>    此外，<see cref="XDocument"/> 是將整份文件載入，故與 <see cref="XmlElmt"/> 一樣適用於新增、刪除等節點操作</para>
	/// <para>    且 <see cref="XDocument"/> 效率也比 <see cref="XmlElmt"/> 來的好</para>
	/// <para>    只是 <see cref="XDocument"/> 已夠簡潔，不需要額外包裝，故 CtXML 並未提供此類方法</para>
	/// <code language="C#">
	/// using System.Xml.Linq;
	/// using System.Xml.XPath;
	/// </code>
	/// <code language="C#">
	/// /*-- 載入檔案 --*/
	/// XDocument doc = XDocument.Load(@"D:\Demo.xml");
	/// 
	/// /*-- 找出 SW_Dep 的 Beta 這個員工，採用 XPath --*/
	/// XElement beta = doc
	///					.Root
	///					.XPathSelectElements("SW_Dep/Engineer")
	///					.FirstOrDefault(eng =&gt; "Beta".Equals(eng.Value));
	///					
	/// /*-- 開除 Beta --*/
	///	beta.Remove();
	///	
	/// /*-- 雇用新人 --*/
	///	doc.Root.Add(
	///		new XElement(
	///			"Engineer",
	///			"Kappa",
	///			new XAttribute("Sexual", "Male")
	///		)
	///	);
	///	
	/// /*-- 存檔 --*/
	/// doc.Save(@"D:\Demo_1.xml");
	/// </code>
	/// 4. 僅查詢 XML 文件內特定的資料，適用 <see cref="CtXML"/> 相關靜態方法
	/// <code language="C#">
	/// /*-- 取得 HW_Dep 的 Manager --*/
	/// XmlElmt managerr = CtXML.GetElement(
	///							@"D:\Demo.xml",
	///							"Company/HW_Dep/Manager"
	///						);
	/// </code>
	/// </example>
	public class CtXML : ICtVersion {

		#region Version

		/// <summary>CtXML 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0  Ahern [2014/09/14]
		///     + 從舊版CtLib搬移
		///     
		/// 1.0.1  Ahern [2014/09/22]
		///     + FindAttribute
		///     
		/// 1.0.2  Ahern [2015/02/22]
		///     + AddComment
		///     
		/// 1.0.3  Ahern [2015/05/25]
		///     \ XmlAttr 與 XmlData 改為 struct
		/// 
		/// 1.1.0  Ahern [2015/10/17]
		///     + IXmlData
		///     \ XmlAttr 與 XmlData 改為 class、拉出至 Library Namespace 並繼承 IXmlData
		///     + XmlData 相關擴充性
		///     
		/// 2.0.0  Ahern [2016/11/07]
		///		\ 翻新 XmlAttr、XmlCmt、XmlElmt
		///		\ 改以 Static 方式提供 XmlReader 相關操作
		///     
		/// </code></remarks>
		public CtVersion @Version { get { return new CtVersion(2, 0, 0, "2016/11/07", "Ahern Kuo"); } }

		#endregion

		#region Function - Full Document
		/// <summary>
		/// 從既有檔案載入 XML 文件，並取得根節點資料
		/// <para>此方法適合整份文件均須處理的情況，以及需要進行節點新增、移除的操作</para>
		/// </summary>
		/// <param name="filePath">檔案路徑，如 @"D:\Demo.xml"</param>
		/// <returns>該 XML 文件的根節點資料</returns>
		/// <example>
		/// 示範用 XML 如下
		/// <code language="XML">
		/// &lt;?xml version="1.0" encoding="utf-8"?&gt;
		/// &lt;Company&gt;
		///		&lt;SW_Dep Population="4"&gt;
		///			&lt;Manager Sexual="Male"&gt;Psi&lt;/Manager&gt;
		///			&lt;Engineer Sexual="Male"&gt;Alpha&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Beta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Gamma&lt;/Engineer&gt;
		///		&lt;/SW_Dep&gt;
		///		&lt;HW_Dep Population="6"&gt;
		///			&lt;Manager Sexual="Male"&gt;Omega&lt;/Manager&gt;
		///			&lt;Engineer Sexual="Male"&gt;Delta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Epsilon&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Zeta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Eta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Theta&lt;/Engineer&gt;
		///		&lt;/HW_Dep&gt;
		/// &lt;/Company&gt;
		/// </code>
		/// 因 <see cref="XmlElmt"/> 會將 XML 文件儲存進來，故可進行新增、移除節點的操作
		/// <para>如需高效率，請自行實作 <see cref="System.Xml.Linq.XDocument"/> 等方法，請看 <see cref="CtXML"/> 之 Example</para>
		/// <para>以下為新增、移除節點後儲存至新檔案</para>
		/// <code language="C#">
		/// XmlElmt xml = CtXML.Load(@"D:\Demo.xml");
		/// XmlElmt hwDep = xml.Element("HW_Dep");
		/// 
		/// /*-- Remove engineer "Eta" --*/
		/// XmlElmt eta = hwDep.Elements().Find(eng =&gt; "Eta".Equals(eng.Value));
		/// if (eta != null) hwDep.RemoveElement(eta);
		/// 
		/// /*-- Add new engineer, "Lota" --*/
		/// hwDep.Add(
		///		new XmlElmt(
		///			"Engineer",
		///			"Lota",
		///			new XmlAttr("Sexual", "Female")
		///		)
		/// );
		/// 
		/// /*-- Update to latest HW_Dep --*/
		/// xml.RemoveElement("HW_Dep");
		/// xml.Add(hwDep);
		/// 
		/// /*-- Save to file --*/
		/// CtXML.Save(xml, @"D:\Demo_1.xml");
		/// </code>
		/// </example>
		public static XmlElmt Load(string filePath) {
			return new XmlElmt(filePath);
		}

		/// <summary>
		/// 從 <see cref="Stream"/> 串流中讀取 XML 文件，並回傳根節點資料
		/// <para>此方法適合整份文件均須處理的情況，以及需要進行節點新增、移除的操作</para>
		/// </summary>
		/// <param name="stream">含有 XML 資訊的 <see cref="Stream"/></param>
		/// <returns>該 XML 文件的根節點資料</returns>
		/// <example>
		/// 示範用 XML 如下
		/// <code language="XML">
		/// &lt;?xml version="1.0" encoding="utf-8"?&gt;
		/// &lt;Company&gt;
		///		&lt;SW_Dep Population="4"&gt;
		///			&lt;Manager Sexual="Male"&gt;Psi&lt;/Manager&gt;
		///			&lt;Engineer Sexual="Male"&gt;Alpha&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Beta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Gamma&lt;/Engineer&gt;
		///		&lt;/SW_Dep&gt;
		///		&lt;HW_Dep Population="6"&gt;
		///			&lt;Manager Sexual="Male"&gt;Omega&lt;/Manager&gt;
		///			&lt;Engineer Sexual="Male"&gt;Delta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Epsilon&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Zeta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Eta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Theta&lt;/Engineer&gt;
		///		&lt;/HW_Dep&gt;
		/// &lt;/Company&gt;
		/// </code>
		/// 因 <see cref="XmlElmt"/> 會將 XML 文件儲存進來，故可進行新增、移除節點的操作
		/// <para>如需高效率，請自行實作 <see cref="System.Xml.Linq.XDocument"/> 等方法，請看 <see cref="CtXML"/> 之 Example</para>
		/// <para>以下為新增、移除節點後儲存至新檔案</para>
		/// <code language="C#">
		/// XmlElmt xml = null;
		/// 
		/// using (MemoryStream memStm = new MemoryStream()) {
		///		//Do something here
		///		xml = CtXML.Load(memStm);
		/// }
		/// 
		/// XmlElmt hwDep = xml.Element("HW_Dep");
		/// 
		/// /*-- Remove engineer "Eta" --*/
		/// XmlElmt eta = hwDep.Elements().Find(eng =&gt; "Eta".Equals(eng.Value));
		/// if (eta != null) hwDep.RemoveElement(eta);
		/// 
		/// /*-- Add new engineer, "Lota" --*/
		/// hwDep.Add(
		///		new XmlElmt(
		///			"Engineer",
		///			"Lota",
		///			new XmlAttr("Sexual", "Female")
		///		)
		/// );
		/// 
		/// /*-- Update to latest HW_Dep --*/
		/// xml.RemoveElement("HW_Dep");
		/// xml.Add(hwDep);
		/// 
		/// /*-- Save to stream --*/
		/// using (MemoryStream memStm = new MemoryStream()) {
		///		//Do something here
		///		CtXML.Save(xml, memStm);
		/// }
		/// </code>
		/// </example>
		public static XmlElmt Load(Stream stream) {
			return new XmlElmt(stream);
		}

		/// <summary>將 <see cref="XmlElmt"/> 所表示的節點儲存至檔案</summary>
		/// <param name="xmlElmt">欲儲存的 <see cref="XmlElmt"/> 節點</param>
		/// <param name="filePath">檔案路徑，如 @"D:\Demo.xml"</param>
		/// <example>
		/// 示範用 XML 如下
		/// <code language="XML">
		/// &lt;?xml version="1.0" encoding="utf-8"?&gt;
		/// &lt;Company&gt;
		///		&lt;SW_Dep Population="4"&gt;
		///			&lt;Manager Sexual="Male"&gt;Psi&lt;/Manager&gt;
		///			&lt;Engineer Sexual="Male"&gt;Alpha&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Beta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Gamma&lt;/Engineer&gt;
		///		&lt;/SW_Dep&gt;
		///		&lt;HW_Dep Population="5"&gt;
		///			&lt;Manager Sexual="Male"&gt;Omega&lt;/Manager&gt;
		///			&lt;Engineer Sexual="Male"&gt;Delta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Epsilon&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Zeta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Eta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Theta&lt;/Engineer&gt;
		///		&lt;/HW_Dep&gt;
		/// &lt;/Company&gt;
		/// </code>
		/// 因 <see cref="XmlElmt"/> 會將 XML 文件儲存進來，故可進行新增、移除節點的操作
		/// <para>如需高效率，請自行實作 <see cref="System.Xml.Linq.XDocument"/> 等方法，請看 <see cref="CtXML"/> 之 Example</para>
		/// <para>以下為新增、移除節點後儲存至新檔案</para>
		/// <code language="C#">
		/// XmlElmt xml = CtXML.Load(@"D:\Demo.xml");
		/// XmlElmt hwDep = xml.Element("HW_Dep");
		/// 
		/// /*-- Remove engineer "Eta" --*/
		/// XmlElmt eta = hwDep.Elements().Find(eng =&gt; "Eta".Equals(eng.Value));
		/// if (eta != null) hwDep.RemoveElement(eta);
		/// 
		/// /*-- Add new engineer, "Lota" --*/
		/// hwDep.Add(
		///		new XmlElmt(
		///			"Engineer",
		///			"Lota",
		///			new XmlAttr("Sexual", "Female")
		///		)
		/// );
		/// 
		/// /*-- Update to latest HW_Dep --*/
		/// xml.RemoveElement("HW_Dep");
		/// xml.Add(hwDep);
		/// 
		/// /*-- Save to file --*/
		/// CtXML.Save(xml, @"D:\Demo_1.xml");
		/// </code>
		/// </example>
		public static void Save(XmlElmt xmlElmt, string filePath) {
			xmlElmt.Save(filePath);
		}

		/// <summary>將 <see cref="XmlElmt"/> 所表示的節點儲存至串流(<see cref="Stream"/>)</summary>
		/// <param name="xmlElmt">欲儲存的 <see cref="XmlElmt"/> 節點</param>
		/// <param name="stream">欲存放 XML 文件的串流(<see cref="Stream"/>)</param>
		/// <example>
		/// 示範用 XML 如下
		/// <code language="XML">
		/// &lt;?xml version="1.0" encoding="utf-8"?&gt;
		/// &lt;Company&gt;
		///		&lt;SW_Dep Population="4"&gt;
		///			&lt;Manager Sexual="Male"&gt;Psi&lt;/Manager&gt;
		///			&lt;Engineer Sexual="Male"&gt;Alpha&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Beta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Gamma&lt;/Engineer&gt;
		///		&lt;/SW_Dep&gt;
		///		&lt;HW_Dep Population="6"&gt;
		///			&lt;Manager Sexual="Male"&gt;Omega&lt;/Manager&gt;
		///			&lt;Engineer Sexual="Male"&gt;Delta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Epsilon&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Zeta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Eta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Theta&lt;/Engineer&gt;
		///		&lt;/HW_Dep&gt;
		/// &lt;/Company&gt;
		/// </code>
		/// 因 <see cref="XmlElmt"/> 會將 XML 文件儲存進來，故可進行新增、移除節點的操作
		/// <para>如需高效率，請自行實作 <see cref="System.Xml.Linq.XDocument"/> 等方法，請看 <see cref="CtXML"/> 之 Example</para>
		/// <para>以下為新增、移除節點後儲存至新檔案</para>
		/// <code language="C#">
		/// XmlElmt xml = null;
		/// 
		/// using (MemoryStream memStm = new MemoryStream()) {
		///		//Do something here
		///		xml = CtXML.Load(memStm);
		/// }
		/// 
		/// XmlElmt hwDep = xml.Element("HW_Dep");
		/// 
		/// /*-- Remove engineer "Eta" --*/
		/// XmlElmt eta = hwDep.Elements().Find(eng =&gt; "Eta".Equals(eng.Value));
		/// if (eta != null) hwDep.RemoveElement(eta);
		/// 
		/// /*-- Add new engineer, "Lota" --*/
		/// hwDep.Add(
		///		new XmlElmt(
		///			"Engineer",
		///			"Lota",
		///			new XmlAttr("Sexual", "Female")
		///		)
		/// );
		/// 
		/// /*-- Update to latest HW_Dep --*/
		/// xml.RemoveElement("HW_Dep");
		/// xml.Add(hwDep);
		/// 
		/// /*-- Save to stream --*/
		/// using (MemoryStream memStm = new MemoryStream()) {
		///		//Do something here
		///		CtXML.Save(xml, memStm);
		/// }
		/// </code>
		/// </example>
		public static void Save(XmlElmt xmlElmt, Stream stream) {
			xmlElmt.Save(stream);
		}
		#endregion

		#region Function - Part Operation
		/// <summary>
		/// 取得 XML 檔案內的特定節點
		/// <para>此方法適用於僅搜尋 XML 文件內的特定資料，而不用載入整份文件</para>
		/// </summary>
		/// <param name="filePath">欲讀取的 XML 檔案路徑，如 @"D:\Demo.xml"</param>
		/// <param name="nodePath">節點路徑，請帶入根節點。如 "/Root/Engineer/Beta" 表示取得 XML 文件的 Root → Engineer → Beta 節點</param>
		/// <returns>符合的節點。如無相符節點則回傳 <see langword="null"/></returns>
		/// <example>
		/// 示範用 XML 如下
		/// <code language="XML">
		/// &lt;?xml version="1.0" encoding="utf-8"?&gt;
		/// &lt;Company&gt;
		///		&lt;SW_Dep Population="4"&gt;
		///			&lt;Manager Sexual="Male"&gt;Psi&lt;/Manager&gt;
		///			&lt;Engineer Sexual="Male"&gt;Alpha&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Beta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Gamma&lt;/Engineer&gt;
		///		&lt;/SW_Dep&gt;
		///		&lt;HW_Dep Population="6"&gt;
		///			&lt;Manager Sexual="Male"&gt;Omega&lt;/Manager&gt;
		///			&lt;Engineer Sexual="Male"&gt;Delta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Epsilon&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Zeta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Eta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Theta&lt;/Engineer&gt;
		///		&lt;/HW_Dep&gt;
		/// &lt;/Company&gt;
		/// </code>
		/// 在此提供 <see cref="XmlReader"/> 所實作的高效率 XML 文件操作
		/// <para>直接開啟檔案後進行搜尋，而不會將整份 XML 文件載入，藉以保持較高的效率</para>
		/// <para>也因不會載入整份文件，故此類方法適合僅須查詢特定節點之場所</para>
		/// <code language="C#">
		/// string file = @"D:\Demo.xml";
		/// string targetNode = "SW_Dep/Manager";
		/// XmlElmt manager = CtXML.GetElement(file, targetNode);
		/// Console.WriteLine($"Manager of Software Department is {manager.Value}");
		/// </code>
		/// </example>
		public static XmlElmt GetElement(string filePath, string nodePath) {
			XmlElmt tarElmt = null;

			/*-- 確認檔案路徑不為空 --*/
			if (string.IsNullOrEmpty(filePath))
				throw new ArgumentNullException("FilePath", "Path of loaded file could not be null or empty");

			/*-- 確認檔案存在 --*/
			if (!File.Exists(filePath))
				throw new FileNotFoundException("Can not find target file to load", filePath);

			/*-- 確認節點路徑不為空 --*/
			if (string.IsNullOrEmpty(nodePath))
				throw new ArgumentNullException("NodePath", "The path points to node can not be null or empty");

			/*-- 載入並前往節點 --*/
			using (XmlReader reader = XmlReader.Create(filePath)) {
				/* 分割節點路徑 */
				var split = nodePath.Split(CtConst.CHR_PATH, StringSplitOptions.RemoveEmptyEntries).Select(str => str.Trim());
				/* 往下尋找節點 */
				bool searched = true;
				foreach (var nodeName in split) {
					if (!reader.ReadToDescendant(nodeName)) {
						//找不到特定節點就離開
						searched = false;
						break;
					}
				}
				/* 轉換至 XmlElmt */
				if (searched) tarElmt = new XmlElmt(reader);
			}
			return tarElmt;
		}

		/// <summary>
		/// 搜尋 XML 檔案內具有特定名稱與數值的屬性，回傳符合的節點
		/// <para>此方法適用於僅搜尋 XML 文件內的特定資料，而不用載入整份文件</para>
		/// </summary>
		/// <param name="filePath">欲讀取的 XML 檔案路徑，如 @"D:\Demo.xml"</param>
		/// <param name="attrName">欲比對的屬性名稱</param>
		/// <param name="attrValue">欲比對的屬性數值</param>
		/// <returns>符合的節點。如無相符節點則回傳 <see langword="null"/></returns>
		public static XmlElmt GetElementByAttribute(string filePath, string attrName, string attrValue) {
			XmlElmt tarElmt = null;

			/*-- 確認檔案路徑不為空 --*/
			if (string.IsNullOrEmpty(filePath))
				throw new ArgumentNullException("FilePath", "Path of loaded file could not be null or empty");

			/*-- 確認檔案存在 --*/
			if (!File.Exists(filePath))
				throw new FileNotFoundException("Can not find target file to load", filePath);

			/*-- 確認節點屬性不為空 --*/
			if (string.IsNullOrEmpty(attrName))
				throw new ArgumentNullException("AttributeName", "The attribute name to search elements can not be null or empty");

			/*-- 載入並前往節點 --*/
			using (XmlReader reader = XmlReader.Create(filePath)) {
				reader.MoveToContent();
				while (reader.Read()) {
					if (reader.HasAttributes) {
						if (reader.GetAttribute(attrName) == attrValue) {
							tarElmt = new XmlElmt(reader);
							break;
						}
					}
				}
			}

			return tarElmt;
		}

		/// <summary>
		/// 搜尋 XML 檔案內特定節點的子節點中具有特定名稱與數值的屬性，回傳符合的節點
		/// <para>此方法適用於僅搜尋 XML 文件內的特定資料，而不用載入整份文件</para>
		/// </summary>
		/// <param name="filePath">欲讀取的 XML 檔案路徑，如 @"D:\Demo.xml"</param>
		/// <param name="nodePath">節點路徑，請帶入根節點。如 "/Root/Engineer/Beta" 表示取得 XML 文件要搜尋 Beta 下面的子節點</param>
		/// <param name="attrName">欲比對的屬性名稱</param>
		/// <param name="attrValue">欲比對的屬性數值</param>
		/// <returns>符合的節點。如無相符節點則回傳 <see langword="null"/></returns>
		public static XmlElmt GetElementByAttribute(string filePath, string nodePath, string attrName, string attrValue) {
			XmlElmt tarElmt = null;

			/*-- 確認檔案路徑不為空 --*/
			if (string.IsNullOrEmpty(filePath))
				throw new ArgumentNullException("FilePath", "Path of loaded file could not be null or empty");

			/*-- 確認檔案存在 --*/
			if (!File.Exists(filePath))
				throw new FileNotFoundException("Can not find target file to load", filePath);

			/*-- 確認節點屬性不為空 --*/
			if (string.IsNullOrEmpty(attrName))
				throw new ArgumentNullException("AttributeName", "The attribute name to search elements can not be null or empty");

			/*-- 載入並前往節點 --*/
			using (XmlReader reader = XmlReader.Create(filePath)) {
				/* 分割節點路徑 */
				var split = nodePath.Split(CtConst.CHR_PATH, StringSplitOptions.RemoveEmptyEntries).Select(str => str.Trim());
				/* 往下尋找節點 */
				bool searched = true;
				foreach (var nodeName in split) {
					if (!reader.ReadToDescendant(nodeName)) {
						//找不到特定節點就離開
						searched = false;
						break;
					}
				}
				/* 搜尋相同名稱的節點 */
				if (searched) {
					/* 找到沒了為止 */
					while (reader.Read()) {
						if (reader.HasAttributes) {
							if (reader.GetAttribute(attrName) == attrValue) {
								tarElmt = new XmlElmt(reader);
								break;
							}
						}
					}
				}
			}

			return tarElmt;
		}

		/// <summary>
		/// 取得 XML 檔案內的相同名稱節點
		/// <para>此方法適用於僅搜尋 XML 文件內的特定資料，而不用載入整份文件</para>
		/// </summary>
		/// <param name="filePath">欲讀取的 XML 檔案路徑，如 @"D:\Demo.xml"</param>
		/// <param name="nodePath">節點路徑，請帶入根節點。如 "/Root/Engineer/Beta" 表示取得 XML 文件要搜尋 Beta 下面的子節點</param>
		/// <param name="elmtName">欲搜尋的節點名稱</param>
		/// <returns>符合的節點集合</returns>
		/// <example>
		/// 示範用 XML 如下
		/// <code language="XML">
		/// &lt;?xml version="1.0" encoding="utf-8"?&gt;
		/// &lt;Company&gt;
		///		&lt;SW_Dep Population="4"&gt;
		///			&lt;Manager Sexual="Male"&gt;Psi&lt;/Manager&gt;
		///			&lt;Engineer Sexual="Male"&gt;Alpha&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Beta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Gamma&lt;/Engineer&gt;
		///		&lt;/SW_Dep&gt;
		///		&lt;HW_Dep Population="6"&gt;
		///			&lt;Manager Sexual="Male"&gt;Omega&lt;/Manager&gt;
		///			&lt;Engineer Sexual="Male"&gt;Delta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Epsilon&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Zeta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Eta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Theta&lt;/Engineer&gt;
		///		&lt;/HW_Dep&gt;
		/// &lt;/Company&gt;
		/// </code>
		/// 在此提供 <see cref="XmlReader"/> 所實作的高效率 XML 文件操作
		/// <para>直接開啟檔案後進行搜尋，而不會將整份 XML 文件載入，藉以保持較高的效率</para>
		/// <para>也因不會載入整份文件，故此類方法適合僅須查詢特定節點之場所</para>
		/// <code language="C#">
		/// string file = @"D:\Demo.xml";
		/// string targetNode = "SW_Dep";
		/// string targetElement = "Engineer";
		/// List&lt;XmlElmt&gt; engineers = CtXML.GetElements(file, targetNode, targetElement);
		/// Console.WriteLine(string.Join(engineers.ConvertAll(eng =&gt; eng.Value)));
		/// </code>
		/// </example>
		public static List<XmlElmt> GetElements(string filePath, string nodePath, string elmtName) {
			List<XmlElmt> elmtColl = new List<XmlElmt>();

			/*-- 確認檔案路徑不為空 --*/
			if (string.IsNullOrEmpty(filePath))
				throw new ArgumentNullException("FilePath", "Path of loaded file could not be null or empty");

			/*-- 確認檔案存在 --*/
			if (!File.Exists(filePath))
				throw new FileNotFoundException("Can not find target file to load", filePath);

			/*-- 確認節點路徑不為空 --*/
			if (string.IsNullOrEmpty(nodePath))
				throw new ArgumentNullException("NodePath", "The path points to node can not be null or empty");

			/*-- 載入並前往節點 --*/
			using (XmlReader reader = XmlReader.Create(filePath)) {
				/* 分割節點路徑 */
				var split = nodePath.Split(CtConst.CHR_PATH, StringSplitOptions.RemoveEmptyEntries).Select(str => str.Trim());
				/* 往下尋找節點 */
				bool searched = true;
				foreach (var nodeName in split) {
					if (!reader.ReadToDescendant(nodeName)) {
						//找不到特定節點就離開
						searched = false;
						break;
					}
				}
				/* 搜尋相同名稱的節點 */
				if (searched && reader.ReadToDescendant(elmtName)) {
					/* 找到沒了為止 */
					do {
						elmtColl.Add(new XmlElmt(reader));
					} while (reader.ReadToNextSibling(elmtName));
				}
			}
			return elmtColl;
		}

		/// <summary>
		/// 取得 XML 檔案內的特定屬性
		/// <para>此方法適用於僅搜尋 XML 文件內的特定資料，而不用載入整份文件</para>
		/// </summary>
		/// <param name="filePath">欲讀取的 XML 檔案路徑，如 @"D:\Demo.xml"</param>
		/// <param name="nodePath">節點路徑，請帶入根節點。如 "/Root/Engineer/Beta" 表示取得 XML 文件要搜尋 Beta 節點的屬性</param>
		/// <param name="attrName">欲尋找的屬性節點名稱</param>
		/// <returns>符合的屬性節點。如無符合屬性則回傳 <see langword="null"/></returns>
		/// <example>
		/// 示範用 XML 如下
		/// <code language="XML">
		/// &lt;?xml version="1.0" encoding="utf-8"?&gt;
		/// &lt;Company&gt;
		///		&lt;SW_Dep Population="4"&gt;
		///			&lt;Manager Sexual="Male"&gt;Psi&lt;/Manager&gt;
		///			&lt;Engineer Sexual="Male"&gt;Alpha&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Beta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Gamma&lt;/Engineer&gt;
		///		&lt;/SW_Dep&gt;
		///		&lt;HW_Dep Population="6"&gt;
		///			&lt;Manager Sexual="Male"&gt;Omega&lt;/Manager&gt;
		///			&lt;Engineer Sexual="Male"&gt;Delta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Epsilon&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Zeta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Male"&gt;Eta&lt;/Engineer&gt;
		///			&lt;Engineer Sexual="Female"&gt;Theta&lt;/Engineer&gt;
		///		&lt;/HW_Dep&gt;
		/// &lt;/Company&gt;
		/// </code>
		/// 在此提供 <see cref="XmlReader"/> 所實作的高效率 XML 文件操作
		/// <para>直接開啟檔案後進行搜尋，而不會將整份 XML 文件載入，藉以保持較高的效率</para>
		/// <para>也因不會載入整份文件，故此類方法適合僅須查詢特定節點之場所</para>
		/// <code language="C#">
		/// string file = @"D:\Demo.xml";
		/// string targetNode = "SW_Dep";
		/// string targetAttribute = "Population";
		/// XmlAttr attr = CtXML.GetAttribute(file, targetNode, targetAttribute);
		/// Console.WriteLine($"Software Department have {attr.Value} people");
		/// </code>
		/// </example>
		public static XmlAttr GetAttribute(string filePath, string nodePath, string attrName) {
			/*-- 確認檔案路徑不為空 --*/
			if (string.IsNullOrEmpty(filePath))
				throw new ArgumentNullException("FilePath", "Path of loaded file could not be null or empty");

			/*-- 確認檔案存在 --*/
			if (!File.Exists(filePath))
				throw new FileNotFoundException("Can not find target file to load", filePath);

			/*-- 確認節點路徑不為空 --*/
			if (string.IsNullOrEmpty(nodePath))
				throw new ArgumentNullException("NodePath", "The path points to node can not be null or empty");

			/*-- 確認屬性不為空 --*/
			if (string.IsNullOrEmpty(attrName))
				throw new ArgumentNullException("AttrName", "The attribute name to search can not be null or empty");

			XmlAttr attr = null;

			/*-- 載入並前往節點 --*/
			using (XmlReader reader = XmlReader.Create(filePath)) {
				/* 分割節點路徑 */
				var split = nodePath.Split(CtConst.CHR_PATH, StringSplitOptions.RemoveEmptyEntries).Select(str => str.Trim());
				/* 往下尋找節點 */
				bool searched = true;
				foreach (var nodeName in split) {
					if (!reader.ReadToDescendant(nodeName)) {
						//找不到特定節點就離開
						searched = false;
						break;
					}
				}
				/* 尋找 Attribute */
				if (searched && reader.MoveToAttribute(attrName)) {
					attr = new XmlAttr(reader.LocalName, reader.Value);
				}
			}

			return attr;
		}
		#endregion
	}
}
