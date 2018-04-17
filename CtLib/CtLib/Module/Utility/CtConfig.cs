using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using CtLib.Library;
using CtLib.Module.XML;

namespace CtLib.Module.Utility {

	/// <summary>提供可手動匯出與自動匯入的 XML 設定檔，採用反射組件與組態進行操作</summary>
	public class CtConfig : IDisposable {

		#region Version

		/// <summary>CtConfig 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0	Ahern	[2016/08/05]
		///     + 建立基礎模組，可供匯出、寫入與更新
		///     
		/// 1.0.1	Ahern	[2016/08/08]
		///		+ EncryptName 與 DecryptName，避免外部人員輕易看見變數
		///     
		/// </code></remarks>
		public static CtVersion Version { get { return new CtVersion(1, 0, 1, "2016/08/08", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Supported Classes
		/// <summary>設定檔註解</summary>
		private class ConfigComment : IXmlOperable {

			#region Fields
			/// <summary>註解本文</summary>
			private string mCmt = string.Empty;
			#endregion

			#region Properties
			/// <summary>取得註解文字</summary>
			public string Comment { get { return mCmt; } }
			#endregion

			#region Constructors
			/// <summary>使用 <see cref="XmlCmt"/> 建構註解物件</summary>
			/// <param name="data">含有 <see cref="XmlCmt"/> 的資訊物件</param>
			public ConfigComment(XmlCmt data) {
				mCmt = data.Value;
			}

			/// <summary>帶入註解文字並建構物件</summary>
			/// <param name="comment">欲添加的註解文字</param>
			public ConfigComment(string comment) {
				mCmt = comment;
			}
			#endregion

			#region IXmlOperable Implements
			/// <summary>取得對應的 <see cref="IXmlData"/> 供除檔使用</summary>
			/// <param name="nodeName">節點名稱</param>
			/// <returns>對應的 <see cref="IXmlData"/></returns>
			public IXmlData CreateXmlNode(string nodeName) {
				return new XmlCmt(string.Format(" {0} ", mCmt));
			}
			#endregion

		}
		#endregion

		#region Declaration - Fields
		/// <summary>暫存當前的設定檔  (TKey)變數名稱/註解索引 (TValue)數值</summary>
		private Dictionary<string, object> mVarColl = new Dictionary<string, object>();
		/// <summary>註解索引</summary>
		/// <remarks>因 C# 不能用數字當變數名稱開頭，故所以直接用數字當索引即可避開衝突</remarks>
		private int mCmtIdx = 0;
		/// <summary>已載入的 XML 檔案路徑</summary>
		private string mXmlPath = string.Empty;
		#endregion

		#region Function - Constructors
		/// <summary>建構 XML 設定檔物件</summary>
		public CtConfig() {

		}

		/// <summary>建構並直接載入 XML 設定檔</summary>
		/// <param name="instance">欲載入設定檔之物件</param>
		/// <param name="confPath">XML 設定檔路徑，如 @"D:\Config.xml"</param>
		public CtConfig(object instance, string confPath) {
			LoadConfig(instance, confPath);
		}
		#endregion

		#region Function - IDisposable Implements
		/// <summary>釋放資源</summary>
		public void Dispose() {
			if (mVarColl != null) {
				mVarColl.Clear();
				mVarColl = null;
			}
		}
		#endregion

		#region Function - Private Methods
		/// <summary>判斷此 <see cref="Type"/> 是否是可以儲存成自訂 XML 的類型</summary>
		/// <param name="tp">欲判斷的類型</param>
		/// <returns>(<see langword="true"/>)可儲存成 XML  (<see langword="false"/>)不可儲存</returns>
		/// <remarks>判斷條件：實數型態、列舉、字串(非實數、是類別、是矩陣，故特別拉出來處理)、繼承的介面是否有 <see cref="IXmlOperable"/></remarks>
		private bool IsXmlOperable(Type tp) {
			bool ret = false;
			if (tp.IsGenericType) ret = tp.GenericTypeArguments.All(IsXmlOperable); //遞迴判斷
			else ret = tp.IsValueType || tp.IsEnum || tp.IsArray || tp == typeof(string) || tp.GetInterfaces().Contains(typeof(IXmlOperable));
			return ret;
		}

		/// <summary>取得物件的單一 <see cref="XNode"/> 以供產生 XML 資訊。如為泛型物件會進行遞迴</summary>
		/// <param name="name">節點名稱</param>
		/// <param name="obj">欲取得 <see cref="XNode"/> 之物件</param>
		/// <param name="genType">此物件之類別</param>
		/// <returns>對應的 <see cref="XNode"/>，可能為 <see cref="XElement"/> 或 <seealso cref="XComment"/></returns>
		private XNode GetXElement(string name, object obj, Type genType) {
			XNode tempNode = null;
			if (genType.IsEnum || genType.IsValueType || genType == typeof(string)) {   //實數類，直接進行處理
				tempNode = new XElement(name, obj.ToString());
			} else if (typeof(ConfigComment).IsAssignableFrom(genType)) {               //註解，直接進行處理
				tempNode = new XComment(obj.ToString());
			} else if (genType.GetInterfaces().Contains(typeof(IXmlOperable))) {        //可儲存成自訂 XML 的物件，產生 XML 節點並處理
				IXmlOperable op = obj as IXmlOperable;
				tempNode = (op.CreateXmlNode(name) as XmlElmt).ToXElement();
			} else if (genType.IsGenericType) {                                         //泛型，只能進行特殊處理囉...
																						/* 泛型後面有哪些類型。例如 obj=List<int> => genType=List, tps[0]=int */
				Type[] tps = genType.GetGenericArguments();
				/* 準備儲存產生的節點 */
				List<XNode> nodeColl = new List<XNode>();
				if (tps.Length == 1) {
					/* List 或是 Array */
					IEnumerable col = obj as IEnumerable;
					foreach (var item in col) {
						/* 遞迴抓一維集合裡的物件 */
						XNode subNode = GetXElement("Item", item, tps[0]);
						/* 標註類型 */
						(subNode as XElement).Add(new XAttribute("Type", tps[0]));
						/* 丟進去集合 */
						nodeColl.Add(subNode);
					}
				} else if (tps.Length == 2) {
					/* Dictionary、KeyValuePair 或 HashSet 等等 */
					/* 因 GetDictXElemt 內會再包一層 Pair 節點，故這邊直接產生就行 */
					XNode[] subNodeColl = GetDictionaryXElement(obj, genType, tps[0], tps[1]);
					/* 丟進去集合 */
					nodeColl.AddRange(subNodeColl);
				} else throw new NotSupportedException("尚無法解析 " + genType.ToString());

				/* 不管是一維、二維還是 n 維，全部包進一個節點吧！ */
				tempNode = new XElement(name, nodeColl.ToArray());
			}
			return tempNode;
		}

		/// <summary>取得一維集合各內容的 <see cref="XNode"/></summary>
		/// <param name="obj">一維集合物件</param>
		/// <param name="genType">此集合類型</param>
		/// <returns>內容轉換後的 <see cref="XNode"/></returns>
		private XNode[] GetListXElement(object obj, Type genType) {
			IEnumerable col = obj as IEnumerable;
			List<XNode> nodeColl = new List<XNode>();
			foreach (var item in col) {
				XNode subNode = GetXElement("arg", item, genType);
				(subNode as XElement).Add(new XAttribute("Type", genType.ToString()));
				nodeColl.Add(subNode);
			}
			return nodeColl.ToArray();
		}

		/// <summary>取得二維集合各內容的 <see cref="XNode"/></summary>
		/// <param name="obj">二維集合物件</param>
		/// <param name="genType">二維集合物件類型</param>
		/// <param name="keyType">索引類型</param>
		/// <param name="valType">數值類型</param>
		/// <returns>內容轉換後的 <see cref="XNode"/></returns>
		private XNode[] GetDictionaryXElement(object obj, Type genType, Type keyType, Type valType) {
			List<XNode> nodeColl = new List<XNode>();
			if (genType.Name.Contains("KeyValuePair")) {                                //KVP 沒有繼承任何東西，只能硬讀
																						/* Key */
				var keyObj = genType.GetProperty("Key").GetValue(obj);
				XNode xKey = GetXElement("Key", keyObj, keyType);
				(xKey as XElement).Add(new XAttribute("Type", keyType.ToString()));

				/* Value */
				var valObj = genType.GetProperty("Value").GetValue(obj);
				XNode xVal = GetXElement("Value", valObj, valType);
				(xVal as XElement).Add(new XAttribute("Type", valType.ToString()));

				/* 回傳一組 */
				nodeColl.Add(new XElement("Pair", xKey, xVal));
			} else {                                                                    //Dict 或 HashSet 都有繼承可抓
																						/* 取得 Keys 與 Values 集合 */
																						/* 因 ICollection 是用 Item[object] 的方式指定，於 GetValue 階段亦須指定。故直接抓 IEnumerable 雖然比較慢，但是複雜度少很多 */
				var keyColl = (genType.GetProperty("Keys").GetValue(obj) as IEnumerable).Cast<object>();
				var valColl = (genType.GetProperty("Values").GetValue(obj) as IEnumerable).Cast<object>();

				/* 分成 Key 與 Value 去組出來 */
				int count = (obj as ICollection).Count;
				for (int idx = 0; idx < count; idx++) {
					/* Key */
					XNode xKey = GetXElement("Key", keyColl.ElementAt(idx), keyType);
					(xKey as XElement).Add(new XAttribute("Type", keyType.ToString()));

					/* Value */
					XNode xVal = GetXElement("Value", valColl.ElementAt(idx), valType);
					(xVal as XElement).Add(new XAttribute("Type", valType.ToString()));

					/* 丟進集合 */
					nodeColl.Add(new XElement("Pair", xKey, xVal));
				}
			}

			return nodeColl.ToArray();
		}

		/// <summary>取得所有設定物件之對應 <see cref="XNode"/></summary>
		/// <param name="nodes">欲轉換的設定檔物件集合</param>
		/// <returns>對應的 <see cref="XNode"/> 集合</returns>
		private XNode[] GetXElement(Dictionary<string, object> nodes) {
			IEnumerable<XNode> childColl = nodes.Select(
				kvp => {
					Type tp = kvp.Value.GetType();
					if (tp.IsValueType || tp.IsEnum || tp == typeof(string)) {                  //實數類，直接處理
						return new XElement(
							"Config",
							new XAttribute("Link", kvp.Key),
							new XAttribute("Type", kvp.Value.GetType().ToString()),
							kvp.Value.ToString()
						);
					} else if (tp.IsGenericType) {                                              //泛型，各自帶開

						/* 儲存轉換後的內容 XNode */
						XNode[] nodeColl = null;

						/* 取得泛型的內容類型。如 kvp.Value=Dictionary<string, int>，則 tp=Dictionary`2, tps[0]=string, tps[1]=int */
						Type[] tps = tp.GetGenericArguments();
						if (tps.Length == 1) {          /* 一維集合 */
							nodeColl = GetListXElement(kvp.Value, tps[0]);
						} else if (tps.Length == 2) {   /* 二維集合 */
							nodeColl = GetDictionaryXElement(kvp.Value, tp, tps[0], tps[1]);
						} else {                        /* 多維集合 */
							throw new NotSupportedException("無法解析 " + tp.ToString());
						}

						/* 建立節點 */
						return new XElement(
							"Config",
							new XAttribute("Link", kvp.Key),
							new XAttribute("Type", tp.ToString()),
							nodeColl
						);
					} else if (tp.IsArray) {                                                    //陣列，稍微處理一下~

						/* 陣列型態有點麻煩，例如 System.Double[]，且暫時沒有找到其他屬性可以抓取 "Double"，故這邊採用字串處理掉 */
						Type type = Type.GetType(tp.ToString().Replace("[]", string.Empty));

						/* 轉轉轉~ */
						XNode[] nodeColl = GetListXElement(kvp.Value, type);

						/* 建立節點 */
						return new XElement(
							"Config",
							new XAttribute("Link", kvp.Key),
							new XAttribute("Type", tp.ToString()),
							nodeColl
						);
					} else if (kvp.Value is ConfigComment) {                                    //註解，直接處理
						return (XNode)(new XComment(" " + (kvp.Value as ConfigComment).Comment.Trim() + " "));
					} else {                                                                    //其他可轉換為自訂 XML 的類別，抓取資訊並建立節點
						IXmlOperable op = kvp.Value as IXmlOperable;
						XElement opElmt = (op.CreateXmlNode("Config") as XmlElmt).ToXElement();
						opElmt.Add(
							new XAttribute("Link", kvp.Key),
							new XAttribute("Type", tp.ToString())
						);
						return opElmt;
					}
				}
			);
			return childColl.ToArray();
		}

		/// <summary>將含有設定資訊的 <see cref="XElement"/> 轉換為相對應的物件</summary>
		/// <param name="confNode">含有設定資訊的 <see cref="XElement"/></param>
		/// <returns>相對應的物件</returns>
		private object ConfigParser(XElement confNode) {
			/* 儲存最後結果 */
			object retObj = null;

			/* 取得節點屬性所表示的物件類型 */
			Type confTp = Type.GetType(confNode.Attribute("Type").Value);

			/* 進行判斷 */
			if (confTp.IsEnum)                                                          //列舉，直接轉換
				retObj = Enum.Parse(confTp, confNode.Value);
			else if (confTp.IsValueType || confTp == typeof(string))                    //實數類與字串，直接轉換
				retObj = Convert.ChangeType(confNode.Value, confTp);
			else if (confTp.GetInterfaces().Contains(typeof(IXmlOperable))) {           //可自訂 XML 的物件，轉換 IXmlData 並丟進去建構元
				XmlElmt xmlData = new XmlElmt(confNode);
				retObj = Activator.CreateInstance(confTp, xmlData);
			} else if (confTp.IsGenericType) {                                          //泛型，各自帶開~ 散！

				/* 取得泛型子類型 */
				Type[] tps = confTp.GenericTypeArguments;

				if (tps.Length == 1) {  /* 一維集合，List 或 Array */

					/* 取得 XML 子節點，即為集合內的元素 */
					IEnumerable<XElement> args = confNode.Elements();
					/* 儲存集合內的元素物件 */
					List<object> argObj = new List<object>();
					/* 遞迴各 XML 子節點，並取得其對應物件 */
					foreach (XElement arg in args) {
						argObj.Add(ConfigParser(arg));
					}
					/* 建構一維集合 */
					retObj = Activator.CreateInstance(confTp);
					/* 採用 IList 處理，因 IEnumerable、ICollection 不接受添加 */
					IList list = retObj as IList;
					argObj.ForEach(obj => list.Add(obj));

				} else if (tps.Length == 2) {   /* 二維集合，Dictionary、HashTable 等 */

					/* 取得由 Pair 包起來的節點 */
					IEnumerable<XElement> pairs = confNode.Elements("Pair");
					/* 建構二維集合 */
					retObj = Activator.CreateInstance(confTp);
					MethodInfo method = confTp.GetMethod("Add");
					/* 進行轉換 */
					if (pairs != null && pairs.Any()) {
						foreach (XElement pair in pairs) {
							/* Key */
							XElement keyNode = pair.Element("Key");
							object keyObj = ConfigParser(keyNode);
							/* Value */
							XElement valNode = pair.Element("Value");
							object valObj = ConfigParser(valNode);
							/* 直接呼叫方法塞回去吧！ */
							method.Invoke(retObj, new object[] { keyObj, valObj });
						}
					}
				}
			} else if (confTp.IsArray) {                                                //陣列，與 List 不一樣的塞法                         
																						/* 取得 XML 子節點，即為集合內的元素 */
				IEnumerable<XElement> args = confNode.Elements();
				/* 儲存集合內的元素物件 */
				List<object> argObj = new List<object>();
				/* 遞迴各 XML 子節點，並取得其對應物件 */
				foreach (XElement arg in args) {
					argObj.Add(ConfigParser(arg));
				}
				/* 建構一維集合，採用如 double[] value = new double[5] 這種建構元，尚未找到一次塞進去的建構方法 */
				retObj = Activator.CreateInstance(confTp, argObj.Count);
				IList list = retObj as IList;
				for (int idx = 0; idx < argObj.Count; idx++) {
					list[idx] = argObj[idx];
				}
			}

			return retObj;
		}

		/// <summary>將變數名稱進行 Base64 編碼</summary>
		/// <param name="oriName">欲編碼的變數名稱</param>
		/// <returns>對應的 Base64 編碼</returns>
		private string EncryptName(string oriName) {
			byte[] byteData = Encoding.ASCII.GetBytes(oriName);
			return "$" + Convert.ToBase64String(byteData).Replace("=", "");
		}

		/// <summary>還原 Base64 編碼，並回傳來源是否有編碼</summary>
		/// <param name="tarName">已編碼之字串</param>
		/// <param name="decName">解碼後之字串</param>
		/// <returns>(<see langword="true"/>)來源字串有編碼過 (<see langword="false"/>)來源字串沒有編碼</returns>
		private bool DecryptName(string tarName, out string decName) {
			bool encrypted = true;
			if (!tarName.StartsWith("$")) {
				decName = tarName;
				encrypted = false;
				return encrypted;
			}
			string dataStr = tarName.Replace("$", "");
			int eqSign = dataStr.Length % 4;
			if (eqSign > 0) for (int eq = 4; eq > eqSign; eq--) dataStr += "=";
			byte[] base64 = Convert.FromBase64String(dataStr);
			decName = Encoding.ASCII.GetString(base64);
			return encrypted;
		}
		#endregion

		#region Function - Public Operations
		/// <summary>加入物件至設定檔。請以物件為準，勿直接餵入常數字串，如 (✕)AddItem("Hi") (✓)AddItem(srvName)</summary>
		/// <typeparam name="TInst">欲載入設定檔之任意類別</typeparam>
		/// <param name="item">欲加入設定檔的物件</param>
		/// <param name="autoSave">是否自動儲存至已載入的 XML</param>
		public void AddItem<TInst>(TInst item, bool autoSave = false) {
			/* 抓取變數名稱 */
			/* 因 nameof、Expression.Body 會抓到 "item" (方法內的代稱) 而非原始名稱，故用 StackTrace 進行抓取 */
			StackFrame sf = new StackTrace(true).GetFrame(1);
			string fileName = sf.GetFileName();
			int lineNo = sf.GetFileLineNumber();
			IEnumerable<string> cntx = File.ReadLines(fileName);
			string oriLine = cntx.ElementAt(lineNo - 1);
			string varName = oriLine.Split(new char[] { '(', ')', ',' })[1];
			string encName = EncryptName(varName);

			/* 取得類型 */
			Type type = typeof(TInst);

			/* 判斷此類型是否可添加至 XML 資訊 */
			if (IsXmlOperable(type)) mVarColl.Add(encName, item);

			/* 如要自動儲存則儲存之 */
			if (autoSave) {
				if (string.IsNullOrEmpty(mXmlPath)) throw new ArgumentNullException("XmlPath", "CtConfig 尚未載入任何檔案，請先指定");
				SaveConfig(mXmlPath);
			}
		}

		/// <summary>加入註解</summary>
		/// <param name="comment">註解</param>
		/// <param name="autoSave">是否自動儲存至已載入的 XML</param>
		public void AddComment(string comment, bool autoSave = false) {
			ConfigComment cmt = new ConfigComment(comment);
			mVarColl.Add(mCmtIdx.ToString(), cmt);
			mCmtIdx++;

			/* 如要自動儲存則儲存之 */
			if (autoSave) {
				if (string.IsNullOrEmpty(mXmlPath)) throw new ArgumentNullException("XmlPath", "CtConfig 尚未載入任何檔案，請先指定");
				SaveConfig(mXmlPath);
			}
		}

		/// <summary>加入物件與其註解至設定檔。請以物件為準，勿直接餵入常數字串，如 (✕)AddItem("Spider", "Hi") (✓)AddItem(srvName, "Hi")</summary>
		/// <typeparam name="TInst">欲載入設定檔之任意類別</typeparam>
		/// <param name="item">欲加入設定檔的物件</param>
		/// <param name="comment">此物件之註解</param>
		/// <param name="autoSave">是否自動儲存至已載入的 XML</param>
		public void AddItem<TInst>(TInst item, string comment, bool autoSave = false) {
			/* 抓取變數名稱 */
			/* 因 nameof、Expression.Body 會抓到 "item" (方法內的代稱) 而非原始名稱，故用 StackTrace 進行抓取 */
			StackFrame sf = new StackTrace(true).GetFrame(1);
			string fileName = sf.GetFileName();
			int lineNo = sf.GetFileLineNumber();
			IEnumerable<string> cntx = File.ReadLines(fileName);
			string oriLine = cntx.ElementAt(lineNo - 1);
			string varName = oriLine.Split(new char[] { '(', ')', ',' })[1];
			string encName = EncryptName(varName);

			/* 抓取類型，如可加入 XML 資訊才進行儲存 */
			Type type = typeof(TInst);
			if (IsXmlOperable(type)) {
				/* 建立註解 */
				ConfigComment cmt = new ConfigComment(comment);

				/* 加入集合 */
				mVarColl.Add(mCmtIdx.ToString(), cmt);
				mVarColl.Add(encName, item);

				/* 註解索引 + 1 */
				mCmtIdx++;
			}

			/* 如要自動儲存則儲存之 */
			if (autoSave) {
				if (string.IsNullOrEmpty(mXmlPath)) throw new ArgumentNullException("XmlPath", "CtConfig 尚未載入任何檔案，請先指定");
				SaveConfig(mXmlPath);
			}
		}

		/// <summary>儲存 XML 設定檔</summary>
		/// <param name="path">欲儲存的路徑，請帶有副檔名。如 @"D:\Config.xml"</param>
		public void SaveConfig(string path) {
			/* XML Declaration */
			XDeclaration declare = new XDeclaration("1.0", "UTF-8", string.Empty);

			/* XML Document，文件主體 */
			XDocument doc = new XDocument() { Declaration = declare };

			/* 取得設定物件之對應 XNode */
			XNode[] data = GetXElement(mVarColl);

			/* 建立根節點 */
			XElement root = new XElement(
				"Configurations",
				data
			);

			/* 把根節點加入 XML 文件主體 */
			doc.Add(root);

			/* 儲存 */
			doc.Save(path);

			/* 紀錄 */
			mXmlPath = path;
		}

		/// <summary>載入設定檔至特定的物件</summary>
		/// <typeparam name="TInst">欲載入設定檔之任意類別</typeparam>
		/// <param name="instance">欲載入設定檔之類別</param>
		/// <param name="path">XML 檔案路徑</param>
		public void LoadConfig<TInst>(TInst instance, string path) where TInst : class {

			/* 載入 XML，如果壞掉就噴出去囉！ */
			XDocument doc = XDocument.Load(path);

			/* 初始化變數 */
			bool encrypt = true, reSave = false;
			string varName;
			mCmtIdx = 0;
			mVarColl.Clear();

			/* 取得 instance 之類別 */
			Type instTp = typeof(TInst);

			/* 取得根目錄(Configurations)下面所有節點 */
			IEnumerable<XNode> nodeColl = doc.Element("Configurations").Nodes();

			/* 分析並寫入數值 */
			foreach (XNode node in nodeColl) {
				if (node.NodeType == XmlNodeType.Element) {             /* 節點 */

					/* 轉換為 XElement */
					XElement configNode = node as XElement;
					/* 取得對應的數值 */
					object val = ConfigParser(configNode);
					/* 取得對應的變數 */
					string encName = configNode.Attribute("Link").Value;
					encrypt = DecryptName(encName, out varName);
					var temp = instTp.GetField(varName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
					/* 寫入數值 */
					temp.SetValue(instance, val);
					/* 如果沒有編碼，進行編碼 */
					if (!encrypt) {
						encName = EncryptName(encName);
						reSave = true;
					}
					/* 紀錄 */
					mVarColl.Add(encName, val);

				} else if (node.NodeType == XmlNodeType.Comment) {      /* 註解 */

					/* 轉換為 XComment 註解 */
					XComment cmtNode = node as XComment;
					/* 建立註解 */
					ConfigComment cmtObj = new ConfigComment(cmtNode.Value);
					/* 紀錄 */
					mVarColl.Add(mCmtIdx.ToString(), cmtObj);
					mCmtIdx++;
				}
			}

			/* 如果有未編碼的變數，進行儲存 */
			if (reSave) SaveConfig(path);

			/* 紀錄 */
			mXmlPath = path;
		}

		/// <summary>修改設定參數數值，如欲新增請用 <see cref="AddItem{TInst}(TInst, bool)"/></summary>
		/// <typeparam name="TInst">欲載入設定檔之任意類別</typeparam>
		/// <param name="instance">欲載入設定檔之類別</param>
		/// <param name="autoSave">是否自動儲存 XML 檔案</param>
		public void Update<TInst>(TInst instance, bool autoSave = true) where TInst : class {
			/* 抓取變數名稱 */
			/* 因 nameof、Expression.Body 會抓到 "instance" (方法內的代稱) 而非原始名稱，故用 StackTrace 進行抓取 */
			StackFrame sf = new StackTrace(true).GetFrame(1);
			string fileName = sf.GetFileName();
			int lineNo = sf.GetFileLineNumber();
			IEnumerable<string> cntx = File.ReadLines(fileName);
			string oriLine = cntx.ElementAt(lineNo - 1);
			string varName = oriLine.Split(new char[] { '(', ')', ',' })[1];

			/* 更新數值 */
			if (mVarColl.ContainsKey(varName)) {
				mVarColl[varName] = instance;
			} else throw new ArgumentNullException("尚未添加的設定：" + varName);

			/* 如果有需要，就自動儲存吧 */
			if (autoSave) {
				if (string.IsNullOrEmpty(mXmlPath)) throw new ArgumentNullException("XmlPath", "CtConfig 尚未載入任何檔案，請先指定");
				SaveConfig(mXmlPath);
			}
		}

		/// <summary>修改設定參數數值與其註解，如欲新增請用 <see cref="AddItem{TInst}(TInst, string, bool)"/></summary>
		/// <typeparam name="TInst">欲載入設定檔之任意類別</typeparam>
		/// <param name="cmt">註解</param>
		/// <param name="instance">欲載入設定檔之類別</param>
		/// <param name="autoSave">是否自動儲存 XML 檔案</param>
		public void Update<TInst>(TInst instance, string cmt, bool autoSave = true) where TInst : class {
			/* 抓取變數名稱 */
			/* 因 nameof、Expression.Body 會抓到 "instance" (方法內的代稱) 而非原始名稱，故用 StackTrace 進行抓取 */
			StackFrame sf = new StackTrace(true).GetFrame(1);
			string fileName = sf.GetFileName();
			int lineNo = sf.GetFileLineNumber();
			IEnumerable<string> cntx = File.ReadLines(fileName);
			string oriLine = cntx.ElementAt(lineNo - 1);
			string varName = oriLine.Split(new char[] { '(', ')', ',' })[1];

			/* 更新數值 */
			if (mVarColl.ContainsKey(varName)) {
				mVarColl[varName] = instance;
			} else throw new ArgumentNullException("尚未添加的設定：" + varName);

			/* 抓取數值的前一個 */
			KeyValuePair<string, object> preKvp = default(KeyValuePair<string, object>);
			foreach (KeyValuePair<string, object> item in mVarColl) {
				if (item.Key == varName) break;
				preKvp = item;
			}

			/* 寫入註解 */
			if (mVarColl[preKvp.Key] is ConfigComment) mVarColl[preKvp.Key] = new ConfigComment(cmt);
			else throw new InvalidOperationException("參數 " + varName + " 並沒有註解可供複寫");

			/* 如果有需要，就自動儲存吧 */
			if (autoSave) {
				if (string.IsNullOrEmpty(mXmlPath)) throw new ArgumentNullException("XmlPath", "CtConfig 尚未載入任何檔案，請先指定");
				SaveConfig(mXmlPath);
			}
		}
		#endregion
	}
}
