using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.Serialization;

using CtLib.Library;

namespace CtLib.Module.XML {

	#region Enumerations
	/// <summary>XML 節點資料類型</summary>
	public enum NodeType {
		/// <summary>屬性(Attribute)</summary>
		Attribute,
		/// <summary>註解(Comment)</summary>
		Comment,
		/// <summary>節點元素(Element)</summary>
		Element,
	}
	#endregion

	#region Interfaces
	/// <summary>表示 XML 節點</summary>
	public interface IXmlData {

		#region Properties
		/// <summary>取得或設定節點名稱</summary>
		string Name { get; set; }
		/// <summary>取得或設定節點資料</summary>
		string Value { get; set; }
		/// <summary>取得或設定自訂義物件</summary>
		object Tag { get; set; }
		/// <summary>取得此節點類型</summary>
		NodeType Type { get; }
		#endregion

		#region Methods
		/// <summary>產生 XML 對應節點並寫入 <see cref="XmlWriter"/></summary>
		/// <param name="writer">欲寫入的 <see cref="XmlWriter"/></param>
		void AppendXml(XmlWriter writer);
		#endregion
	}
	#endregion

	#region XML Implements
	/// <summary>XML 屬性節點</summary>
	[DataContract(Name = "XmlAttr")]
	public class XmlAttr : IXmlData {

		#region Fields
		/// <summary>屬性名稱</summary>
		[DataMember(Name = "Name")]
		private string mName = string.Empty;
		/// <summary>屬性資料</summary>
		[DataMember(Name = "Value")]
		private string mValue = string.Empty;
		/// <summary>自定義資料</summary>
		[DataMember(Name = "Tag")]
		private object mTag = null;
		#endregion

		#region Constructors
		/// <summary>以實際數值建構 XML 屬性節點</summary>
		/// <param name="name">屬性名稱</param>
		/// <param name="value">屬性資料</param>
		public XmlAttr(string name, string value) {
			mName = name;
			mValue = value;
		}

		/// <summary>使用 <see cref="XAttribute"/> 進行建構，僅供內部使用</summary>
		/// <param name="attr">欲建立的 <see cref="XAttribute"/></param>
		internal XmlAttr(XAttribute attr) {
			mName = attr.Name.LocalName;
			mValue = attr.Value;
		}
		#endregion

		#region Public Operations
		/// <summary>取得此屬性節點的複製品</summary>
		/// <returns>複製品</returns>
		public XmlAttr Clone() {
			return new XmlAttr(mName, mValue);
		}

		/// <summary>取得屬性對應的 <see cref="XAttribute"/></summary>
		/// <returns>對應節點</returns>
		public XAttribute ToXAttribute() {
			return new XAttribute(mName, mValue);
		}
		#endregion

		#region Properties
		/// <summary>取得或設定屬性節點名稱</summary>
		public string Name { get { return mName; } set { mName = value; } }
		/// <summary>取得或設定屬性節點資料</summary>
		public string Value { get { return mValue; } set { mValue = value; } }
		/// <summary>取得或設定自訂義資料</summary>
		public object Tag { get { return mTag; } set { mTag = value; } }
		/// <summary>取得此節點類型</summary>
		public NodeType Type { get { return NodeType.Attribute; } }
		#endregion

		#region Overrides
		/// <summary>與另一個物件進行比較是否相同</summary>
		/// <param name="obj">欲比較的物件</param>
		/// <returns>(<see langword="true"/>)兩者相同 (<see langword="false"/>)兩者不同</returns>
		public override bool Equals(object obj) {
			if (!object.ReferenceEquals(obj, null)) {
				XmlAttr attr = obj as XmlAttr;
				if (object.ReferenceEquals(attr, null)) return false;
				return mName == attr.Name && mValue == attr.Value;
			} else return false;
		}

		/// <summary>取得此物件之雜湊碼</summary>
		/// <returns>雜湊碼</returns>
		public override int GetHashCode() {
			int nameHash = mName?.GetHashCode() ?? int.MaxValue;
			int valueHash = mValue?.GetHashCode() ?? int.MinValue;
			return nameHash & valueHash;
		}

		/// <summary>取得此物件的文字描述</summary>
		/// <returns>文字描述</returns>
		public override string ToString() {
			return $"Attribute, {mName}, {mValue}";
		}

		/// <summary>比較兩個 <see cref="XmlAttr"/> 是否相同</summary>
		/// <param name="a">被比較者</param>
		/// <param name="b">比較者</param>
		/// <returns>(<see langword="true"/>)兩者相同 (<see langword="false"/>)兩者不同</returns>
		public static bool operator ==(XmlAttr a, XmlAttr b) {
			if (object.ReferenceEquals(a, null)) {
				return object.ReferenceEquals(b, null);
			}
			if (object.ReferenceEquals(b, null)) {
				return false;
			} else {
				return a.Name == b.Name && a.Value == b.Value;
			}
		}

		/// <summary>比較兩個 <see cref="XmlAttr"/> 是否不同</summary>
		/// <param name="a">被比較者</param>
		/// <param name="b">比較者</param>
		/// <returns>(<see langword="true"/>)兩者不同 (<see langword="false"/>)兩者相同</returns>
		public static bool operator !=(XmlAttr a, XmlAttr b) {
			return !(a == b);
		}
		#endregion

		#region IXmlData Method Implements
		/// <summary>產生 XML 對應節點並寫入 <see cref="XmlWriter"/></summary>
		/// <param name="writer">欲寫入的 <see cref="XmlWriter"/></param>
		public void AppendXml(XmlWriter writer) {
			writer.WriteAttributeString(mName, mValue);
		}
		#endregion

	}

	/// <summary>XML 註解節點</summary>
	[DataContract(Name = "XmlCmt")]
	public class XmlCmt : IXmlData {

		#region Fields
		/// <summary>註解訊息</summary>
		[DataMember(Name = "Value")]
		private string mValue = string.Empty;
		/// <summary>自訂義資料</summary>
		[DataMember(Name = "Tag")]
		private object mTag = null;
		#endregion

		#region Constructors
		/// <summary>以實際數值建構 XML 註解節點</summary>
		/// <param name="cmt">註解</param>
		public XmlCmt(string cmt) {
			mValue = cmt;
		}

		/// <summary>使用 <see cref="XComment"/> 進行建構</summary>
		/// <param name="cmt">欲建立的 <see cref="XComment"/></param>
		public XmlCmt(XComment cmt) {
			mValue = cmt.Value;
		}
		#endregion

		#region Public Operations
		/// <summary>取得此註解節點的複製品</summary>
		/// <returns>複製品</returns>
		public XmlCmt Clone() {
			return new XmlCmt(mValue);
		}

		/// <summary>取得對應的 <see cref="XComment"/></summary>
		/// <returns>對應的節點</returns>
		public XComment ToXComment() {
			return new XComment(mValue);
		}
		#endregion

		#region IXmlData Implements
		/// <summary>取得或設定此節點名稱。無作用</summary>
		public string Name { get { return string.Empty; } set { } }
		/// <summary>取得或設定註解訊息</summary>
		public string Value { get { return mValue; } set { mValue = value; } }
		/// <summary>取得或設定自訂義資料</summary>
		public object Tag { get { return mTag; } set { mTag = value; } }
		/// <summary>取得節點類型</summary>
		public NodeType Type { get { return NodeType.Comment; } }

		/// <summary>產生 XML 對應節點並寫入 <see cref="XmlWriter"/></summary>
		/// <param name="writer">欲寫入的 <see cref="XmlWriter"/></param>
		public void AppendXml(XmlWriter writer) {
			writer.WriteComment(mValue);
		}
		#endregion

		#region Overrides
		/// <summary>與另一個物件進行比較</summary>
		/// <param name="obj">欲比較的物件</param>
		/// <returns>(<see langword="true"/>)兩者相同 (<see langword="false"/>)兩者不同</returns>
		public override bool Equals(object obj) {
			if (!object.ReferenceEquals(obj, null)) {
				XmlCmt cmt = obj as XmlCmt;
				if (object.ReferenceEquals(cmt, null)) return false;
				return mValue == cmt.Value;
			} else return false;
		}

		/// <summary>取得此物件之雜湊碼</summary>
		/// <returns>雜湊碼</returns>
		public override int GetHashCode() {
			return mValue?.GetHashCode() ?? int.MinValue;
		}

		/// <summary>取得此物件的文字描述</summary>
		/// <returns>文字描述</returns>
		public override string ToString() {
			return $"Comment, {mValue}";
		}

		/// <summary>比較兩個 <see cref="XmlCmt"/> 是否相同</summary>
		/// <param name="a">被比較者</param>
		/// <param name="b">比較者</param>
		/// <returns>(<see langword="true"/>)兩者相同 (<see langword="false"/>)兩者不同</returns>
		public static bool operator ==(XmlCmt a, XmlCmt b) {
			if (object.ReferenceEquals(a, null)) {
				return object.ReferenceEquals(b, null);
			}
			if (object.ReferenceEquals(b, null)) {
				return false;
			} else {
				return a.Value == b.Value;
			}
		}

		/// <summary>比較兩個 <see cref="XmlCmt"/> 是否不同</summary>
		/// <param name="a">被比較者</param>
		/// <param name="b">比較者</param>
		/// <returns>(<see langword="true"/>)兩者不同 (<see langword="false"/>)兩者相同</returns>
		public static bool operator !=(XmlCmt a, XmlCmt b) {
			return !(a == b);
		}
		#endregion
	}

	/// <summary>XML 資料節點</summary>
	[DataContract(Name = "XmlElmt")]
	[KnownType(typeof(XmlAttr))]
	[KnownType(typeof(XmlCmt))]
	public class XmlElmt : IXmlData {

		#region Fields
		/// <summary>節點名稱</summary>
		[DataMember(Name = "Name")]
		private string mName = string.Empty;
		/// <summary>節點資料</summary>
		[DataMember(Name = "Value")]
		private string mValue = string.Empty;
		/// <summary>自訂義資料</summary>
		[DataMember(Name = "Tag")]
		private object mTag = null;
		/// <summary>屬性集合</summary>
		[DataMember(Name = "Attributes")]
		private List<XmlAttr> mAttrs = new List<XmlAttr>();
		/// <summary>子節點集合</summary>
		[DataMember(Name = "Nodes")]
		private List<IXmlData> mChilds = new List<IXmlData>();
		#endregion

		#region Properties
		/// <summary>取得或設定此節點名稱</summary>
		public string Name { get { return mName; } set { mName = value; } }
		/// <summary>取得此節點是否含有屬性資料</summary>
		public bool HasAttribute { get { return mAttrs.Count > 0; } }
		/// <summary>取得此節點是否含有子節點</summary>
		public bool HasChildNode { get { return mChilds.Count > 0; } }
		#endregion

		#region Constructors
		/// <summary>載入 XML 檔案並以此進行建構</summary>
		/// <param name="path">檔案路路徑，如 @"D:\Demo.xml"</param>
		/// <example><code language="C#">
		/// XmlElmt elmt = new XmlElmt(@"D:\Demo.xml");
		/// var port = elmt.Element("/Settings/Port");
		/// Console.WriteLine($"Target Port is {port.Value}");
		/// </code></example>
		internal XmlElmt(string path) {
			/*-- 確認檔案路徑不是空的 --*/
			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException("Path", "File path to load XML can not be null");

			/*-- 確認檔案存在 --*/
			if (!File.Exists(path))
				throw new FileNotFoundException("Can not find the file to load");

			/*-- 開始讀取 --*/
			using (XmlReader reader = XmlReader.Create(path)) {
				Initialize(reader);
			}
		}

		/// <summary>載入 <see cref="Stream"/> 串流內的 XML 資訊</summary>
		/// <param name="stream">檔案路路徑，如 @"D:\Demo.xml"</param>
		internal XmlElmt(Stream stream) {
			/*-- 開始讀取 --*/
			using (XmlReader reader = XmlReader.Create(stream)) {
				Initialize(reader);
			}
		}

		/// <summary>使用 <see cref="XmlReader"/> 讀取 "當前節點" 並進行建構，僅供 <see cref="Initialize(XmlReader)"/> 使用</summary>
		/// <param name="reader">當前讀取的 <see cref="XmlReader"/></param>
		internal XmlElmt(XmlReader reader) {
			/* 抓名字 */
			mName = reader.LocalName;

			/* 抓 Attributes */
			if (reader.HasAttributes) {
				for (int attrIdx = 0; attrIdx < reader.AttributeCount; attrIdx++) {
					reader.MoveToAttribute(attrIdx);
					mAttrs.Add(new XmlAttr(reader.LocalName, reader.Value));
				}
				/* 返回 Element 上 */
				reader.MoveToContent();
			}
			
			/* 如果此節點有內容，抓抓~ */
			if (!reader.IsEmptyElement) {
				bool endOfElement = false;                  //指出是否已經讀取完畢
				while (!endOfElement && reader.Read()) {    //讀取下一個節點
					switch (reader.NodeType) {
						case XmlNodeType.Element:       //子節點
							mChilds.Add(new XmlElmt(reader));
							break;
						case XmlNodeType.Text:          //數值
							mValue = reader.Value;
							break;
						case XmlNodeType.Comment:       //註解
							mChilds.Add(new XmlCmt(reader.Value));
							break;
						case XmlNodeType.EndElement:    //讀取完畢
							endOfElement = true;
							break;
						default:
							break;
					}
				}
			}
		}

		/// <summary>使用 <see cref="XElement"/> 進行建構</summary>
		/// <param name="elmt">欲建立的 <see cref="XElement"/></param>
		internal XmlElmt(XElement elmt) {
			mName = elmt.Name.LocalName;

			if (elmt.HasAttributes) {
				mAttrs.AddRange(elmt.Attributes().Select(attr => new XmlAttr(attr)));
			}

			if (elmt.Nodes().Any()) {
				elmt.Nodes().ForEach(
					node => {
						if (node is XElement) mChilds.Add(new XmlElmt(node as XElement));
						else if (node is XComment) mChilds.Add(new XmlCmt(node as XComment));
					}
				);
			} else if (!string.IsNullOrEmpty(elmt.Value)) mValue = elmt.Value;
		}

		/// <summary>建構單一資料節點</summary>
		/// <param name="name">節點名稱</param>
		/// <param name="value">節點資料</param>
		/// <example><code language="C#">
		/// XmlElmt elmt = new XmlElmt("Person", "MyName");
		/// elmt.SaveToFile(@"D:\Demo.xml");
		/// </code></example>
		public XmlElmt(string name, string value) {
			mName = name;
			mValue = value;
		}

		/// <summary>建構含有屬性或子節點的資料節點</summary>
		/// <param name="name">節點名稱</param>
		/// <param name="contents">資料、屬性或子節點</param>
		/// <example>
		/// 此方法模仿 <see cref="System.Xml.Linq.XElement.XElement(System.Xml.Linq.XName, object[])"/>
		/// 提供簡便的方式添加物件
		/// <code language="C#">
		/// XmlElmt elmt = new XmlElmt(
		///		"MyName",
		///		new XmlAttr("Height", "170"),
		///		new XmlAttr("Weight", "75"),
		///		new XmlCmt("This is a comment line"),
		///		new XmlElmt("Age", "28"),
		///		new XmlElmt(
		///			"Body",
		///			new XmlAttr("State", "Good"),
		///			new XmlElmt("Arms", "2"),
		///			new XmlElmt("Legs", "2")
		///		)
		/// );
		/// </code>
		/// </example>
		public XmlElmt(string name, params object[] contents) {
			mName = name;
			if (contents != null && contents.Length > 0) {
				Add(contents);
			}
		}
		#endregion

		#region Private Methods
		/// <summary>從 <see cref="XmlReader"/> 完整載入檔案</summary>
		/// <param name="reader">XML 讀取器</param>
		private void Initialize(XmlReader reader) {
			/* 直接跳到 Root */
			reader.MoveToContent();
			/* 抓名字 */
			mName = reader.LocalName;
			/* 抓 Attributes */
			if (reader.HasAttributes) {
				for (int attrIdx = 0; attrIdx < reader.AttributeCount; attrIdx++) {
					reader.MoveToAttribute(attrIdx);
					mAttrs.Add(new XmlAttr(reader.LocalName, reader.Value));
				}
			}
			/* 抓子節點 */
			bool endOfElement = false;                  //指出是否已經讀取完畢
			while (!endOfElement && reader.Read()) {    //讀取下一個節點

				/* 依照節點類型作對應動作 */
				switch (reader.NodeType) {
					case XmlNodeType.Element:       //子節點
						mChilds.Add(new XmlElmt(reader));
						break;
					case XmlNodeType.Text:          //數值
						mValue = reader.Value;
						break;
					case XmlNodeType.Comment:       //註解
						mChilds.Add(new XmlCmt(reader.Value));
						break;
					case XmlNodeType.EndElement:    //讀取完畢
						endOfElement = true;
						break;
					default:
						break;
				}
			}
		}
		#endregion

		#region IXmlData Implements
		/// <summary>取得或設定註解訊息</summary>
		public string Value { get { return mValue; } set { mValue = value; } }
		/// <summary>取得或設定自訂義資料</summary>
		public object Tag { get { return mTag; } set { mTag = value; } }
		/// <summary>取得節點類型</summary>
		public NodeType Type { get { return NodeType.Element; } }

		/// <summary>產生 XML 對應節點並寫入 <see cref="XmlWriter"/></summary>
		/// <param name="writer">欲寫入的 <see cref="XmlWriter"/></param>
		public void AppendXml(XmlWriter writer) {
			writer.WriteStartElement(mName);
			if (mAttrs.Count > 0) mAttrs.ForEach(attr => attr.AppendXml(writer));
			if (mChilds.Count > 0) mChilds.ForEach(node => node.AppendXml(writer));
			if (!string.IsNullOrEmpty(mValue)) writer.WriteString(mValue);
			writer.WriteEndElement();
		}
		#endregion

		#region Overrides
		/// <summary>與另一個物件進行比較</summary>
		/// <param name="obj">欲比較的物件</param>
		/// <returns>(<see langword="true"/>)兩者相同 (<see langword="false"/>)兩者不同</returns>
		public override bool Equals(object obj) {
			if (!object.ReferenceEquals(obj, null)) {
				XmlElmt b = obj as XmlElmt;
				if (object.ReferenceEquals(b, null)) return false;
				return mName == b.Name
						&& mValue == b.Value
						&& mAttrs.SequenceEqual(b.Attributes())
						&& mChilds.SequenceEqual(b.Elements());
			} else return false;
		}

		/// <summary>取得此物件之雜湊碼</summary>
		/// <returns>雜湊碼</returns>
		public override int GetHashCode() {
			return mName?.GetHashCode() ?? int.MaxValue ^
					mValue?.GetHashCode() ?? int.MinValue ^
					mAttrs.GetHashCode() ^
					mChilds.GetHashCode();
		}

		/// <summary>取得此物件的文字描述</summary>
		/// <returns>文字描述</returns>
		public override string ToString() {
			return $"Element, {mName}, {mAttrs.Count} attributes, {mChilds.Count} childrens";
		}
		#endregion

		#region Public Operations
		/// <summary>取得此資料節點的複製品</summary>
		/// <returns>複製品</returns>
		/// <remarks>已測試過使用序列化、反序列化的方式，但消耗的 CPU Ticks 比遞迴複製多約 200 ~ 300 Ticks，故仍採用遞迴複製</remarks>
		public XmlElmt Clone() {
			return new XmlElmt(
				mName,
				mValue,
				mAttrs.ConvertAll(attr => attr.Clone()),
				mChilds.ConvertAll(
					node => {
						switch (node.Type) {
							case NodeType.Attribute:
								return (node as XmlAttr).Clone() as IXmlData;
							case NodeType.Comment:
								return (node as XmlCmt).Clone() as IXmlData;
							case NodeType.Element:
								return (node as XmlElmt).Clone() as IXmlData;
							default:
								return null;
						}
					}
				)
			);
		}

		/// <summary>儲存此資料節點至檔案，以此節點作為根節點</summary>
		/// <param name="path">欲存檔的路徑，含副檔名。如 @"D:\Demo.xml"</param>
		internal void Save(string path) {
			/*-- 建立設定器，啟用縮排、使用 Tab 字元作為縮排 --*/
			XmlWriterSettings setting = new XmlWriterSettings() {
				Indent = true, IndentChars = "\t"
			};
			/*-- 儲存至檔案 --*/
			using (XmlWriter writer = XmlWriter.Create(path, setting)) {
				writer.WriteStartDocument();
				AppendXml(writer);
				writer.WriteEndDocument();
			}
		}

		/// <summary>儲存此資料節點至串流 <see cref="Stream"/>，以此節點作為根節點</summary>
		/// <param name="stream">欲存放 XML 的 <see cref="Stream"/></param>
		internal void Save(Stream stream) {
			/*-- 建立設定器，啟用縮排、使用 Tab 字元作為縮排 --*/
			XmlWriterSettings setting = new XmlWriterSettings() {
				Indent = true, IndentChars = "\t"
			};
			/*-- 儲存至檔案 --*/
			using (XmlWriter writer = XmlWriter.Create(stream, setting)) {
				writer.WriteStartDocument();
				AppendXml(writer);
				writer.WriteEndDocument();
			}
		}

		/// <summary>尋找特定名稱的子節點，並取得其複製品</summary>
		/// <param name="path">
		/// 欲尋找的子節點路徑。例如：
		/// <para>"/Age" 或 "Age" 表示尋找名為 Age 的子節點，搜尋範圍為此節點的下一層子節點</para>
		/// <para>"/Male/Age" 或 "Male/Age" 表示尋找名為 Age 的子節點，但會先尋找 Male 節點後才尋找 Age 節點，搜尋深度為兩層</para>
		/// </param>
		/// <returns>搜尋到的子節點複製品</returns>
		/// <exception cref="ArgumentNullException">路徑為 <see langword="null"/> 或 <see cref="string.Empty"/></exception>
		public XmlElmt Element(string path) {
			/*-- 確保路徑不是空的 --*/
			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException("Path", "The path to search element is null");

			/*-- 分割路徑，移除空內容、直接做 .Trim() --*/
			IEnumerable<string> split = path
										.Split(CtConst.CHR_PATH, StringSplitOptions.RemoveEmptyEntries)
										.Select(str => str.Trim());

			/*-- 因已有確保 path 不是空的，所以直接尋找第一層 --*/
			XmlElmt tarNode = mChilds.Find(node => node.Name == split.ElementAt(0)) as XmlElmt;

			/*-- 如果搜尋深度有兩層(含)以上，往下找 --*/
			int count = split.Count();
			if (count > 1 && tarNode != null) {
				for (int idx = 1; idx < count; idx++) {
					if (tarNode == null) break;
					tarNode = tarNode.RefElement(split.ElementAt(idx));
				}
			}

			/*-- 回傳 --*/
			return tarNode?.Clone();
		}

		/// <summary>尋找特定名稱的子節點，並取得其複製品</summary>
		/// <param name="path">
		/// 欲尋找的子節點路徑。例如：
		/// <para>"/Age" 或 "Age" 表示尋找名為 Age 的子節點，搜尋範圍為此節點的下一層子節點</para>
		/// <para>"/Male/Age" 或 "Male/Age" 表示尋找名為 Age 的子節點，但會先尋找 Male 節點後才尋找 Age 節點，搜尋深度為兩層</para>
		/// </param>
		/// <param name="elmt">搜尋到的子節點複製品</param>
		/// <returns>(<see langword="true"/>)有找到符合的子節點  (<see langword="false"/>)無符合的節點</returns>
		/// <exception cref="ArgumentNullException">路徑為 <see langword="null"/> 或 <see cref="string.Empty"/></exception>
		public bool Element(string path, out XmlElmt elmt) {
			/*-- 確保路徑不是空的 --*/
			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException("Path", "The path to search element is null");

			/*-- 分割路徑，移除空內容、直接做 .Trim() --*/
			IEnumerable<string> split = path
										.Split(CtConst.CHR_PATH, StringSplitOptions.RemoveEmptyEntries)
										.Select(str => str.Trim());

			/*-- 因已有確保 path 不是空的，所以直接尋找第一層 --*/
			XmlElmt tarNode = mChilds.Find(node => node.Name == split.ElementAt(0)) as XmlElmt;

			/*-- 如果搜尋深度有兩層(含)以上，往下找 --*/
			int count = split.Count();
			if (count > 1 && tarNode != null) {
				for (int idx = 1; idx < count; idx++) {
					if (tarNode == null) break;
					tarNode = tarNode.RefElement(split.ElementAt(idx));
				}
			}

			/*-- 回傳 --*/
			elmt = tarNode?.Clone();
			return elmt != null;
		}

		/// <summary>回傳子節點，不做複製。僅供內部搜尋使用</summary>
		/// <param name="name">欲尋找的子節點名稱</param>
		/// <returns>子節點</returns>
		private XmlElmt RefElement(string name) {
			return mChilds.Find(elmt => elmt.Name == name) as XmlElmt;
		}

		/// <summary>尋找此節點內所有的子節點</summary>
		/// <returns>節點集合</returns>
		public List<XmlElmt> Elements() {
			/*-- 回傳搜節點複製品 --*/
			return mChilds
					.FindAll(elmt => elmt.Type == NodeType.Element)
					.ConvertAll(elmt => (elmt as XmlElmt).Clone());
		}

		/// <summary>尋找此節點內所有相同名字的子節點</summary>
		/// <param name="name">節點名稱，如 "Age"</param>
		/// <returns>找到的節點集合</returns>
		/// <exception cref="ArgumentNullException">路徑為 <see langword="null"/> 或 <see cref="string.Empty"/></exception>
		/// <exception cref="ArgumentException">欲搜尋的節點名稱錯誤，傳入路徑字串</exception>
		public List<XmlElmt> Elements(string name) {
			/*-- 確保名稱不是空的 --*/
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("Name", "The node name to search element is null");

			/*-- 確保沒有餵路徑進來 --*/
			if (name.Contains("/") || name.Contains(@"\"))
				throw new ArgumentException("Name required but get path string", "Name");

			/*-- 回傳搜尋到的節點複製品 --*/
			return mChilds
					.FindAll(elmt => elmt.Name == name)
					.ConvertAll(elmt => (elmt as XmlElmt).Clone());
		}

		/// <summary>尋找特定子節點內所有相同名字的子節點</summary>
		/// <param name="path">
		/// 欲尋找的子節點路徑。例如：
		/// <para>"/Age" 或 "Age" 表示尋找名為 Age 的子節點，搜尋範圍為此節點的下一層子節點</para>
		/// <para>"/Male/Age" 或 "Male/Age" 表示尋找名為 Age 的子節點，但會先尋找 Male 節點後才尋找 Age 節點，搜尋深度為兩層</para>
		/// </param>
		/// <param name="name">
		/// 欲尋找 path 裡的子節點名稱。例如：
		/// <para>path = "/IO"，name = "Adept" 表示尋找子節點 "IO" 後，再尋找 "IO" 裡所有的名為 "Adept" 之子節點</para>
		/// </param>
		/// <returns>找到的節點集合</returns>
		/// <exception cref="ArgumentNullException">路徑為 <see langword="null"/> 或 <see cref="string.Empty"/></exception>
		/// <exception cref="ArgumentException">欲搜尋的節點名稱錯誤，傳入路徑字串</exception>
		/// <remarks>
		/// 因 <see cref="Element(string)"/> 與 <seealso cref="Elements(string, string)"/> 會做複製動作
		/// <para>為避免多次重覆複製，故這邊選擇重新做一次程式內容而非直接呼叫上述兩個方法</para>
		/// </remarks>
		public List<XmlElmt> Elements(string path, string name) {
			/*-- 確保路徑不是空的 --*/
			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException("Path", "The path to search element is null");

			/*-- 分割路徑，移除空內容、直接做 .Trim() --*/
			IEnumerable<string> split = path
										.Split(CtConst.CHR_PATH, StringSplitOptions.RemoveEmptyEntries)
										.Select(str => str.Trim());

			/*-- 因已有確保 path 不是空的，所以直接尋找第一層 --*/
			XmlElmt tarNode = mChilds.Find(node => node.Name == split.ElementAt(0)) as XmlElmt;

			/*-- 如果搜尋深度有兩層(含)以上，往下找 --*/
			int count = split.Count();
			if (count > 1 && tarNode != null) {
				for (int idx = 1; idx < count; idx++) {
					if (tarNode == null) break;
					tarNode = tarNode.RefElement(split.ElementAt(idx));
				}
			}

			/*-- 回傳尋找的子節點 --*/
			return tarNode
					.Elements()
					.FindAll(elmt => elmt.Name == name)
					.ConvertAll(elmt => elmt.Clone());
		}

		/// <summary>尋找此節點內的屬性</summary>
		/// <param name="name">搜尋目標屬性名稱，如 "Time"</param>
		/// <returns>符合條件的屬性複製品</returns>
		public XmlAttr Attribute(string name) {
			/*-- 確保名稱不是空的 --*/
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("Name", "The node name to search element is null");

			/*-- 回傳之 --*/
			return mAttrs
					.Find(attr => attr.Name == name)
					?.Clone();
		}

		/// <summary>尋找此節點內的屬性</summary>
		/// <param name="name">搜尋目標屬性名稱，如 "Time"</param>
		/// <param name="attr">符合條件的屬性複製品</param>
		/// <returns>(<see langword="true"/>)有找到符合的屬性  (<see langword="false"/>)無符合的屬性</returns>
		public bool Attribute(string name, out XmlAttr attr) {
			/*-- 確保名稱不是空的 --*/
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("Name", "The node name to search element is null");

			/*-- 回傳之 --*/
			attr = mAttrs
					.Find(val => val.Name == name)
					?.Clone();
			return attr != null;
		}

		/// <summary>取得當前所有的 <see cref="XmlAttr"/></summary>
		/// <returns>屬性集合</returns>
		public List<XmlAttr> Attributes() {
			return mAttrs.ConvertAll(attr => attr.Clone());
		}

		/// <summary>取得當前所有的子節點，含 <see cref="XmlCmt"/> 與 <see cref="XmlElmt"/></summary>
		/// <returns>子節點集合</returns>
		public List<IXmlData> Nodes() {
			return mChilds.ConvertAll(
				node => {
					switch (node.Type) {
						case NodeType.Attribute:
							return (node as XmlAttr).Clone() as IXmlData;
						case NodeType.Comment:
							return (node as XmlCmt).Clone() as IXmlData;
						case NodeType.Element:
							return (node as XmlElmt).Clone() as IXmlData;
						default:
							return null;
					}
				}
			);
		}

		/// <summary>添加物件至此資料節點，可含 <see cref="XmlAttr"/>、<see cref="XmlCmt"/> 與 <see cref="XmlElmt"/> 及其集合</summary>
		/// <param name="contents">欲添加的物件</param>
		/// <example>
		/// 此方法模仿 <see cref="System.Xml.Linq.XContainer.Add(object[])"/>
		/// 提供簡便的方式添加物件
		/// <code language="C#">
		/// XmlElmt elmt = new XmlElmt(@"D:\Demo.xml");	//載入 XML 檔案
		/// elmt.Add(
		///		new XmlAttr("Height", "170"),
		///		new XmlAttr("Weight", "75"),
		///		new XmlCmt("This is a comment line"),
		///		new XmlElmt("Age", "28"),
		///		new XmlElmt(
		///			"Body",
		///			new XmlAttr("State", "Good"),
		///			new XmlElmt("Arms", "2"),
		///			new XmlElmt("Legs", "2")
		///		)
		/// );
		/// </code>
		/// </example>
		public void Add(params object[] contents) {
			foreach (var obj in contents) {
				if (obj == null) continue;
				else if (obj is XmlAttr) {
					mAttrs.Add(obj as XmlAttr);
				} else if (obj is IXmlData) {
					mChilds.Add(obj as IXmlData);
				} else if (obj is IEnumerable<XmlAttr>) {
					mAttrs.AddRange(obj as IEnumerable<XmlAttr>);
				} else if (obj is IEnumerable<IXmlData>) {
					IEnumerable<IXmlData> dataColl = obj as IEnumerable<IXmlData>;
					foreach (IXmlData data in dataColl) {
						switch (data.Type) {
							case NodeType.Attribute:
								mAttrs.Add(data as XmlAttr);
								break;
							case NodeType.Comment:
							case NodeType.Element:
								mChilds.Add(data);
								break;
							default:
								break;
						}
					}
				} else mValue = obj.ToString();
			}
		}

		/// <summary>移除特定屬性名稱的 <see cref="XmlAttr"/> 節點</summary>
		/// <param name="name">欲移除的屬性名稱</param>
		/// <example>
		/// 假設有一 XML 檔案 Demo.xml 如下
		/// <code language="XML">
		/// &lt;?xml version="1.0" encoding="utf-8"?&gt;
		/// &lt;Root&gt;
		///		&lt;Person Name="MyName" Age="28"&gt;
		///			&lt;!-- 身高 --&gt;
		///			&lt;Height&gt;170&lt;/Height&gt;
		///			&lt;!-- 體重 --&gt;
		///			&lt;Weight&gt;75&lt;/Weight&gt;
		///		&lt;/Person&gt;
		/// &lt;/Root&gt;
		/// </code>
		/// 可使用以下方法移除特定名稱的屬性
		/// <code language="C#">
		/// XmlElmt xml = new XmlElmt(@"Demo.xml");
		/// xml.RemoveAttribute("Age");
		/// </code>
		/// </example>
		public void RemoveAttribute(string name) {
			mAttrs.RemoveAll(attr => attr.Name == name);
		}

		/// <summary>移除 <see cref="XmlAttr"/> 節點</summary>
		/// <param name="attr">欲移除的屬性節點</param>
		/// <example>
		/// 假設有一 XML 檔案 Demo.xml 如下
		/// <code language="XML">
		/// &lt;?xml version="1.0" encoding="utf-8"?&gt;
		/// &lt;Root&gt;
		///		&lt;Person Name="MyName" Age="28"&gt;
		///			&lt;!-- 身高 --&gt;
		///			&lt;Height&gt;170&lt;/Height&gt;
		///			&lt;!-- 體重 --&gt;
		///			&lt;Weight&gt;75&lt;/Weight&gt;
		///		&lt;/Person&gt;
		/// &lt;/Root&gt;
		/// </code>
		/// 可利用搜尋的方法進行移除
		/// <code language="C#">
		/// XmlElmt xml = new XmlElmt(@"Demo.xml");
		/// var attr = xml.Attribute("Age");
		/// if (attr != null) xml.RemoveAttribute(attr);
		/// </code>
		/// </example>
		public void RemoveAttritbue(XmlAttr attr) {
			mAttrs.RemoveAll(val => val == attr);
		}

		/// <summary>移除指定的註解</summary>
		/// <param name="cmt">欲移除的註解資訊</param>
		/// <example>
		/// 假設有一 XML 檔案 Demo.xml 如下
		/// <code language="XML">
		/// &lt;?xml version="1.0" encoding="utf-8"?&gt;
		/// &lt;Root&gt;
		///		&lt;Person Name="MyName" Age="28"&gt;
		///			&lt;!-- 身高 --&gt;
		///			&lt;Height&gt;170&lt;/Height&gt;
		///			&lt;!-- 體重 --&gt;
		///			&lt;Weight&gt;75&lt;/Weight&gt;
		///		&lt;/Person&gt;
		/// &lt;/Root&gt;
		/// </code>
		/// 可使用以下方法移除註解
		/// <code language="C#">
		/// XmlElmt xml = new XmlElmt(@"Demo.xml");
		/// xml.RemoveComment(" 身高 ");
		/// </code>
		/// </example>
		public void RemoveComment(string cmt) {
			mChilds.RemoveAll(node => node.Value == cmt);
		}

		/// <summary>移除 <see cref="XmlCmt"/> 節點</summary>
		/// <param name="cmt">欲移除的註解節點</param>
		/// <example>
		/// 假設有一 XML 檔案 Demo.xml 如下
		/// <code language="XML">
		/// &lt;?xml version="1.0" encoding="utf-8"?&gt;
		/// &lt;Root&gt;
		///		&lt;Person Name="MyName" Age="28"&gt;
		///			&lt;!-- 身高 --&gt;
		///			&lt;Height&gt;170&lt;/Height&gt;
		///			&lt;!-- 體重 --&gt;
		///			&lt;Weight&gt;75&lt;/Weight&gt;
		///		&lt;/Person&gt;
		/// &lt;/Root&gt;
		/// </code>
		/// 可利用搜尋的方法進行移除
		/// <code language="C#">
		/// XmlElmt xml = new XmlElmt(@"Demo.xml");
		/// var cmt = xml.Nodes().Find(node =&gt; node.Value.Contains("體重")) as XmlCmt;
		/// if (cmt != null) xml.RemoveComment(cmt);
		/// </code>
		/// </example>
		public void RemoveComment(XmlCmt cmt) {
			mChilds.RemoveAll(node => node.Equals(cmt));
		}

		/// <summary>移除符合指定名稱的子節點</summary>
		/// <param name="name">欲移除的節點名稱</param>
		/// <example>
		/// 假設有一 XML 檔案 Demo.xml 如下
		/// <code language="XML">
		/// &lt;?xml version="1.0" encoding="utf-8"?&gt;
		/// &lt;Root&gt;
		///		&lt;Person Name="MyName" Age="28"&gt;
		///			&lt;!-- 身高 --&gt;
		///			&lt;Height&gt;170&lt;/Height&gt;
		///			&lt;!-- 體重 --&gt;
		///			&lt;Weight&gt;75&lt;/Weight&gt;
		///		&lt;/Person&gt;
		/// &lt;/Root&gt;
		/// </code>
		/// 可使用以下方法移除 Weight 節點
		/// <code language="C#">
		/// XmlElmt xml = new XmlElmt(@"Demo.xml");
		/// var person = xml.Element("Person");	//先找到 Person 節點
		/// person.RemoveElement("Weight");		//從 Person 的子節點尋找名為 Weight 的節點並移除
		/// </code>
		/// </example>
		public void RemoveElement(string name) {
			mChilds.RemoveAll(node => node.Name == name);
		}

		/// <summary>移除 <see cref="XmlElmt"/> 節點</summary>
		/// <param name="elmt">欲移除的資料節點</param>
		/// <example>
		/// 假設有一 XML 檔案 Demo.xml 如下
		/// <code language="XML">
		/// &lt;?xml version="1.0" encoding="utf-8"?&gt;
		/// &lt;Root&gt;
		///		&lt;Person Name="MyName" Age="28"&gt;
		///			&lt;!-- 身高 --&gt;
		///			&lt;Height&gt;170&lt;/Height&gt;
		///			&lt;!-- 體重 --&gt;
		///			&lt;Weight&gt;75&lt;/Weight&gt;
		///		&lt;/Person&gt;
		/// &lt;/Root&gt;
		/// </code>
		/// 可使用以下方法移除 Weight 節點
		/// <code language="C#">
		/// XmlElmt xml = new XmlElmt(@"Demo.xml");
		/// var person = xml.Element("Person");	//先找到 Person 節點
		/// var weight = person.Element("Weight");	//再找到 Weight 節點
		/// person.RemoveElement(weight);		//從 Person 的子節點清單中移除 Weight 節點
		/// </code>
		/// </example>
		public void RemoveElement(XmlElmt elmt) {
			mChilds.RemoveAll(node => node.Equals(elmt));
		}

		/// <summary>取代 <see cref="XmlElmt"/> 節點</summary>
		/// <param name="elmt">欲取代、更新的資料節點</param>
		/// <example>
		/// 假設有一 XML 檔案 Demo.xml 如下
		/// <code language="XML">
		/// &lt;?xml version="1.0" encoding="utf-8"?&gt;
		/// &lt;Root&gt;
		///		&lt;Person Name="MyName" Age="28"&gt;
		///			&lt;!-- 身高 --&gt;
		///			&lt;Height&gt;170&lt;/Height&gt;
		///			&lt;!-- 體重 --&gt;
		///			&lt;Weight&gt;75&lt;/Weight&gt;
		///		&lt;/Person&gt;
		/// &lt;/Root&gt;
		/// </code>
		/// 可使用以下方法更新 Weight 節點
		/// <code language="C#">
		/// XmlElmt xml = new XmlElmt(@"Demo.xml");
		/// var person = xml.Element("Person");	//先找到 Person 節點
		/// var weight = person.Element("Weight");	//再找到 Weight 節點
		/// weight.Add(new XmlAttr("Unit", "Kg"));	//修改 Weight 屬性
		/// person.Replace(weight);		//從 Person 的子節點清單中取代並更新 Weight 節點
		/// </code>
		/// </example>
		public void Replace(XmlElmt elmt) {
			var idx = mChilds.FindIndex(node => node.Name == elmt.Name);
			if (idx > -1) {
				mChilds.RemoveAt(idx);
				mChilds.Insert(idx, elmt);
			}
		}

		/// <summary>取得對應的 <see cref="XElement"/></summary>
		/// <returns>對應的節點</returns>
		public XElement ToXElement() {
			XElement elmt = new XElement(mName);
			if (!string.IsNullOrEmpty(mValue)) elmt.Value = mValue;

			mAttrs.ForEach(attr => elmt.Add(attr.ToXAttribute()));
			mChilds.ForEach(
				node => {
					if (node is XmlElmt)
						elmt.Add((node as XmlElmt).ToXElement());
					else if (node is XmlCmt)
						elmt.Add((node as XmlCmt).ToXComment());
				}
			);

			return elmt;
		}

		/// <summary>搜尋具有特定名稱與數值的屬性，回傳符合的節點</summary>
		/// <param name="attrName">欲比對的屬性名稱</param>
		/// <param name="attrValue">欲比對的屬性數值</param>
		/// <returns>符合的節點。如無相符節點則回傳 <see langword="null"/></returns>
		public XmlElmt GetElementByAttribute(string attrName, string attrValue) {
			XmlElmt tarElmt = null;
			XmlAttr attr = mAttrs.Find(val => val.Name == attrName && val.Value == attrValue);
			if (attr != null) tarElmt = this.Clone();
			else {
				foreach (var child in mChilds) {
					if (child.Type == NodeType.Element) {
						XmlElmt shad = child as XmlElmt;
						tarElmt = shad.GetElementByAttribute(attrName, attrValue);
						if (tarElmt != null) break;
					}
				}
			}
			return tarElmt?.Clone();
		}

		/// <summary>搜尋子節點內具有特定名稱與數值的屬性，回傳符合的節點</summary>
		/// <param name="path">欲搜尋的子節點</param>
		/// <param name="attrName">欲比對的屬性名稱</param>
		/// <param name="attrValue">欲比對的屬性數值</param>
		/// <returns>符合的節點。如無相符節點則回傳 <see langword="null"/></returns>
		public XmlElmt GetElementByAttribute(string path, string attrName, string attrValue) {
			/*-- 確保路徑不是空的 --*/
			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException("Path", "The path to search element is null");

			/*-- 分割路徑，移除空內容、直接做 .Trim() --*/
			IEnumerable<string> split = path
										.Split(CtConst.CHR_PATH, StringSplitOptions.RemoveEmptyEntries)
										.Select(str => str.Trim());

			/*-- 因已有確保 path 不是空的，所以直接尋找第一層 --*/
			XmlElmt tarNode = mChilds.Find(node => node.Name == split.ElementAt(0)) as XmlElmt;

			/*-- 如果搜尋深度有兩層(含)以上，往下找 --*/
			int count = split.Count();
			if (count > 1 && tarNode != null) {
				for (int idx = 1; idx < count; idx++) {
					if (tarNode == null) break;
					tarNode = tarNode.RefElement(split.ElementAt(idx));
				}
			}

			/*-- 搜尋含有特定屬性的子節點 --*/
			if (tarNode != null) {
				tarNode = tarNode.mChilds.Find(
					node => {
						if (node.Type != NodeType.Element) return false;
						XmlElmt elmt = node as XmlElmt;
						XmlAttr attr = elmt.Attribute(attrName);
						if (attr != null) return attr.Value == attrValue;
						else return false;
					}
				) as XmlElmt;
			}

			/*-- 回傳 --*/
			return tarNode?.Clone();
		}
		#endregion
	}
	#endregion
}