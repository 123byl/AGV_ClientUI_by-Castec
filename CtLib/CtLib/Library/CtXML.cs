using CtLib.Module.Ultity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CtLib.Library {

    /// <summary>XML 文件處理</summary>
    /// <example>
    /// 以下示範各項基礎操作
    /// 
    /// 1. 建立與載入檔案
    /// <code>
    /// CtXML xml = new CtXML();
    /// xml.Load(@"D:\CASTEC\Config\IO.xml");
    /// </code>
    /// 
    /// 2. 讀取資料
    /// <code>
    /// List&gt;CtXML.XmlData&lt; multiData = xml.GetAllValue("D1403_IO/Data");                 //將所有 Data 內的子節點丟到 multiData 裡
    /// string value = xml.GetInnerText("D1403_IO/Data/IO_1");                                  //讀取 D1403_IO/Data/IO_1 之 數值 (InnerText)
    /// List&gt;CtXML.XmlAttribute&lt; multiAttr = xml.GetAttributes(("D1403_IO/Data/IO_1");    //將 D1403_IO/Data/IO_1 所有屬性丟到 multiAttr 裡
    /// </code>
    /// 
    /// 3. 設定數值
    /// <code>
    /// xml.SetInnerText("D1403_IO/Data/IO_1", "X0012");                                        //將 D1403_IO/Data/IO_1 之 數值(InnerText) 設為 "X0012"
    /// xml.SetAttributes("D1403_IO/Data/IO_1", multiAttr);                                     //設 multiAttr 為 List&gt;CtXML.XmlAttribute&lt;，此副程式可一次寫入
    /// xml.SetAttributes("D1403_IO/Data/IO_1", new CtXML.XmlAttribute("Device", "Beckhoff"))   //修改(或新增)  D1403_IO/Data/IO_1 之屬性名稱為 Device 內容為 Beckhoff
    /// </code>
    /// 
    /// 由於設定數值時會自動儲存檔案，故不必再另外下 Save() 命令
    /// 除非有特別修改才需自行下 Save()
    /// </example>
    public class CtXML {

        #region Version

        /// <summary>CtXML 版本訊息</summary>
        /// <remarks><code>
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
        ///     \ XmlAttribute 與 XmlData 改為 struct
        ///     
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 0, 3, "2015/05/25", "Ahern Kuo");

        #endregion

        #region Declaration - Support Class

        /// <summary>XML 屬性選項</summary>
        public struct XmlAttribute {
            /// <summary>屬性名稱</summary>
            public string Name;
            /// <summary>屬性內容</summary>
            public string Value;
            /// <summary>建立一帶有預設值之屬性選項</summary>
            /// <param name="atbName">屬性名稱</param>
            /// <param name="atbVal">屬性內容</param>
            public XmlAttribute(string atbName, string atbVal) {
                Name = atbName;
                Value = atbVal;
            }
        }

        /// <summary>XML 節點集合資料</summary>
        public struct XmlData {
            /// <summary>節點路徑</summary>
            public string Node;
            /// <summary>節點屬性集合</summary>
            public List<XmlAttribute> Attributes;
            /// <summary>節點文字內容</summary>
            public string InnerText;
            /// <summary>該元件名稱</summary>
            public string Name;
            /// <summary>建立一帶有預設值之XML節點資料</summary>
            /// <param name="nodAtr">屬性集合</param>
            /// <param name="nodVal">文字內容</param>
            /// <param name="nodName">元件名稱</param>
            /// <param name="nodNode">節點路徑</param>
            public XmlData(List<XmlAttribute> nodAtr, string nodVal, string nodName, string nodNode = "") {
                Attributes = nodAtr;
                InnerText = nodVal;
                Node = nodNode;
                Name = nodName;
            }
        }

        #endregion

        #region Declaration - Properties

        /// <summary>取得或設定檔案路徑</summary>
        public string FILE_PATH { get; set; }

        /// <summary>取得是否已載入檔案</summary>
        public bool IsLoaded { get { return mLoaded; } }

        #endregion

        #region Declaration - Members

        /// <summary>XML Document</summary>
        private XmlDocument mDoc = new XmlDocument();
        /// <summary>XML Node</summary>
        private XmlNode mNode;
        /// <summary>XML Element</summary>
        private XmlElement mElement;
        /// <summary>是否載入</summary>
        private bool mLoaded = false;

        #endregion

        #region Function - Constructor

        /// <summary>建立全新的CtXML元件</summary>
        public CtXML() { }

        /// <summary>建立CtXML元件，並帶入檔案路徑以直接開啟檔案</summary>
        /// <param name="filePath">XML檔案路徑</param>
        public CtXML(string filePath) {
            Load(filePath);
        }

        /// <summary>關閉CtXML</summary>
        public void Close() {
            mDoc = null;
            mNode = null;
            mElement = null;
        }

        #endregion

        #region Function - Mehtod

        /// <summary>尋找單一節點</summary>
        /// <param name="path">節點路徑</param>
        /// <returns>最符合之節點</returns>
        /// <remarks>如果找不到會回傳null</remarks>
        private XmlNode FindSingleNode(string path) {
            XmlNode retNode = null;
            string[] strSplit;

            /*-- 分割路徑 --*/
            strSplit = path.Split(CtConst.CHR_PATH, StringSplitOptions.RemoveEmptyEntries);

            /*-- 第一層要先用Root/Level的方式 --*/
            retNode = mDoc.SelectSingleNode(strSplit[0] + "/" + strSplit[1]);

            /*-- 從切割下來的字串去抓單一節點，抓到後往下一個抓 --*/
            for (int i = 2; i < strSplit.Length; i++) {
                retNode = retNode.SelectSingleNode(strSplit[i]);
                if (retNode == null) break;
            }

            return retNode;
        }

        /// <summary>尋找所有子節點</summary>
        /// <param name="path">層級路徑(不含 "/")</param>
        /// <returns>節點集合</returns>
        private XmlNodeList FindNodes(string path) {
            XmlNode retNode = null;
            XmlNodeList nodList = null;

            /*-- 尋找該層級的節點 --*/
            retNode = FindSingleNode(path);
            /*-- 將該節點的子節點拋出來 --*/
            nodList = retNode.ChildNodes;

            return nodList;
        }

        /// <summary>尋找路徑中最後有存在的節點</summary>
        /// <param name="path">原節點路徑</param>
        /// <returns>路徑中最後存在的節點</returns>
        private XmlNode FindLastNode(string path) {
            XmlNode retNode = null;
            string[] strSplit;

            /*-- 分割路徑 --*/
            strSplit = path.Split(CtConst.CHR_PATH, StringSplitOptions.RemoveEmptyEntries);

            /*-- 第一層要先用Root/Level的方式 --*/
            retNode = mDoc.SelectSingleNode(strSplit[0] + "/" + strSplit[1]);

            /*-- 從切割下來的字串去抓單一節點，抓到後往下一個抓 --*/
            for (int i = 2; i < strSplit.Length; i++) {
                retNode = retNode.SelectSingleNode(strSplit[i]);
                /*-- 如過這層找不到東西，回去抓上一層有東西的 --*/
                if (retNode == null) {
                    string strTemp = string.Join("/", strSplit, 0, strSplit.Length - 1);
                    retNode = FindSingleNode(strTemp);
                    break;
                }
            }

            return retNode;
        }
        #endregion

        #region Function - Core

        /// <summary>載入XML檔案，使用預設路徑</summary>
        /// <returns>Status Code</returns>
        public Stat Load() {
            Stat stt = Stat.SUCCESS;
            try {
                if (FILE_PATH == "") {
                    stt = Stat.ER_SYS_ILFLPH;
                    throw (new Exception("請先設定檔案路徑後再行載入動作"));
                }
                mDoc.Load(FILE_PATH);
                mLoaded = true;
            } catch (XmlException xmlEx) {
                mLoaded = false;
                if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, xmlEx);
            } catch (Exception ex) {
                mLoaded = false;
                if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }

        /// <summary>載入XML檔案</summary>
        /// <param name="path">檔案路徑</param>
        /// <returns>Status Code</returns>
        public Stat Load(string path) {
            Stat stt = Stat.SUCCESS;
            try {
                FILE_PATH = path;
                mDoc.Load(path);
                mLoaded = true;
            } catch (XmlException xmlEx) {
                mLoaded = false;
                stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, xmlEx);
            } catch (Exception ex) {
                mLoaded = false;
                stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }

        /// <summary>儲存XML檔案(覆寫當前檔案)</summary>
        /// <returns>Status Code</returns>
        public Stat Save() {
            Stat stt = Stat.SUCCESS;
            try {
                if (FILE_PATH == "") {
                    stt = Stat.ER_SYS_ILFLPH;
                    throw (new Exception("請先設定檔案路徑後再行存檔動作"));
                }
                mDoc.Save(FILE_PATH);
                mLoaded = false;
            } catch (XmlException xmlEx) {
                stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, xmlEx);
            } catch (Exception ex) {
                stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }

        /// <summary>儲存XML檔案(另存新檔)</summary>
        /// <returns>Status Code</returns>
        public Stat SaveAs(string path) {
            Stat stt = Stat.SUCCESS;
            try {
                if (path == "") {
                    stt = Stat.ER_SYS_ILFLPH;
                    throw (new Exception("請先設定檔案路徑後再行存檔動作"));
                }
                mDoc.Save(path);
                mLoaded = false;
            } catch (XmlException xmlEx) {
                stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, xmlEx);
            } catch (Exception ex) {
                stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }

        /// <summary>取得節點文字內容</summary>
        /// <param name="nodePath">節點路徑(不含最後的 "/")</param>
        /// <returns>節點文字內容</returns>
        public string GetInnerText(string nodePath) {
            return FindSingleNode(nodePath).InnerText;
        }

        /// <summary>取得屬性集合</summary>
        /// <param name="path">節點路徑(不含最後的 "/")</param>
        /// <returns>該節點之屬性集合</returns>
        public List<XmlAttribute> GetAttributes(string path) {
            List<XmlAttribute> atrTemp = new List<XmlAttribute>();

            /*-- 搜尋節點 --*/
            mNode = FindSingleNode(path);

            /*-- 將節點的屬性丟到List裡面 --*/
            for (int i = 0; i < mNode.Attributes.Count; i++) {
                atrTemp.Add(
                    new XmlAttribute(
                        mNode.Attributes[i].Name,
                        mNode.Attributes[i].Value
                    )
                );
            }

            return atrTemp;
        }

        /// <summary>取得該層級之所有節點資料</summary>
        /// <param name="path">層級路徑</param>
        /// <returns>資料集合</returns>
        public List<XmlData> GetAllValue(string path) {
            List<XmlData> xmlData = new List<XmlData>();
            XmlNodeList nodList = FindNodes(path);
            List<XmlAttribute> xmlAttr;
            string tempName = "";

            if (nodList != null) {
                foreach (XmlNode item in nodList) {
                    xmlAttr = new List<XmlAttribute>();
                    if (item.LocalName == "#comment") continue;
                    foreach (System.Xml.XmlAttribute attr in item.Attributes) {
                        if (attr.Name == "Name")
                            tempName = attr.Value;
                        else
                            xmlAttr.Add(new XmlAttribute(attr.Name, attr.Value));
                    }
                    xmlData.Add(new XmlData(xmlAttr, item.InnerText, tempName, path));
                    xmlAttr = null;
                }
            }

            return xmlData;
        }

        /// <summary>取得該層級之所有節點資料</summary>
        public void DeleteSubNode(string path) {
            /*-- 搜尋節點 --*/
            mNode = FindSingleNode(path);
            if (mNode != null) mNode.RemoveAll();
        }

        /// <summary>加入XML註解</summary>
        /// <param name="path">節點路徑</param>
        /// <param name="comment">欲加入的註解</param>
        public void AddComment(string path, string comment) {
            if (mDoc != null) {
                mNode = FindLastNode(path);
                XmlComment xmlCmt = mDoc.CreateComment(comment);
                mNode.AppendChild(xmlCmt);
                mDoc.Save(FILE_PATH);
            }
        }

        /// <summary>設定節點內容</summary>
        /// <param name="path">節點路徑</param>
        /// <param name="value">文字內容</param>
        /// <returns>Status Code</returns>
        public void SetInnerText(string path, string value) {
            string[] strSplit;
            int intIdx = 0;

            /*-- 尋找該節點 --*/
            mNode = FindLastNode(path);
            strSplit = path.Split(CtConst.CHR_PATH, StringSplitOptions.RemoveEmptyEntries);

            /*-- 如果該節點已經存在，更改內容 --*/
            if (mNode.Name == strSplit[strSplit.Length - 1])
                mNode.InnerText = value;
            /*-- 不存在則建新的 --*/
            else {
                /*-- 尋找是第幾層後消失了 --*/
                for (int i = 0; i < strSplit.Length; i++) {
                    if (mNode.Name == strSplit[i]) {
                        intIdx = i + 1;
                        break;
                    }
                }

                /*-- 建立新元素 --*/
                for (int j = intIdx; j < strSplit.Length; j++) {
                    mElement = mDoc.CreateElement(strSplit[j]);
                    /*-- 如果已經建立到最後層，設定內容並Append --*/
                    if (j == strSplit.Length - 1) {
                        mElement.InnerText = value;
                        mNode.AppendChild(mElement);
                    } else {
                        /*-- 還沒到最下層則繼續往下建 --*/
                        mNode.AppendChild(mElement);
                        mNode = mNode.SelectSingleNode(strSplit[j]);
                    }
                }
            }
            /*-- 存檔 --*/
            mDoc.Save(FILE_PATH);
        }

        /// <summary>設定節點屬性</summary>
        /// <param name="path">節點路徑</param>
        /// <param name="attrName">屬性名稱</param>
        /// <param name="attrValue">屬性數值</param>
        public void SetAttributes(string path, string attrName, string attrValue) {
            string[] strSplit;
            int intIdx = 0;

            /*-- 尋找該節點 --*/
            mNode = FindLastNode(path);
            strSplit = path.Split(CtConst.CHR_PATH, StringSplitOptions.RemoveEmptyEntries);

            /*-- 如果該節點已經存在，更改內容 --*/
            if (mNode.Name == strSplit[strSplit.Length - 1]) {
                mElement = mNode as XmlElement;
                mElement.SetAttribute(attrName, attrValue);
                /*-- 不存在則建新的 --*/
            } else {
                /*-- 尋找是第幾層後消失了 --*/
                for (int i = 0; i < strSplit.Length; i++) {
                    if (mNode.Name == strSplit[i]) {
                        intIdx = i + 1;
                        break;
                    }
                }

                /*-- 建立新元素 --*/
                for (int j = intIdx; j < strSplit.Length; j++) {
                    mElement = mDoc.CreateElement(strSplit[j]);
                    /*-- 如果已經建立到最後層，設定內容並Append --*/
                    if (j == strSplit.Length - 1) {
                        mElement.SetAttribute(attrName, attrValue);
                        mNode.AppendChild(mElement);
                    } else {
                        /*-- 還沒到最下層則繼續往下建 --*/
                        mNode.AppendChild(mElement);
                        mNode = mNode.SelectSingleNode(strSplit[j]);
                    }
                }
            }
            /*-- 存檔 --*/
            mDoc.Save(FILE_PATH);
        }

        /// <summary>設定節點屬性</summary>
        /// <param name="path">節點路徑</param>
        /// <param name="value">屬性內容</param>
        public void SetAttributes(string path, XmlAttribute value) {
            SetAttributes(path, value.Name, value.Value);
        }

        /// <summary>設定節點屬性(多重屬性)</summary>
        /// <param name="path">節點路徑</param>
        /// <param name="value">屬性內容</param>
        public void SetAttributes(string path, List<XmlAttribute> value) {
            string[] strSplit;
            int intIdx = 0;

            /*-- 尋找該節點 --*/
            mNode = FindLastNode(path);
            strSplit = path.Split(CtConst.CHR_PATH, StringSplitOptions.RemoveEmptyEntries);

            /*-- 如果該節點已經存在，更改內容 --*/
            if (mNode.Name == strSplit[strSplit.Length - 1]) {
                mElement = mNode as XmlElement;
                foreach (XmlAttribute item in value) {
                    mElement.SetAttribute(item.Name, item.Value);
                }
                /*-- 不存在則建新的 --*/
            } else {
                /*-- 尋找是第幾層後消失了 --*/
                for (int i = 0; i < strSplit.Length; i++) {
                    if (mNode.Name == strSplit[i]) {
                        intIdx = i + 1;
                        break;
                    }
                }

                /*-- 建立新元素 --*/
                for (int j = intIdx; j < strSplit.Length; j++) {
                    mElement = mDoc.CreateElement(strSplit[j]);
                    /*-- 如果已經建立到最後層，設定內容並Append --*/
                    if (j == strSplit.Length - 1) {
                        foreach (XmlAttribute item in value) {
                            mElement.SetAttribute(item.Name, item.Value);
                        }
                        mNode.AppendChild(mElement);
                    } else {
                        /*-- 還沒到最下層則繼續往下建 --*/
                        mNode.AppendChild(mElement);
                        mNode = mNode.SelectSingleNode(strSplit[j]);
                    }
                }
            }
            /*-- 存檔 --*/
            mDoc.Save(FILE_PATH);
        }

        /// <summary>尋找屬性集合中的特定某項目</summary>
        /// <param name="attr">屬性集合</param>
        /// <param name="name">欲搜尋之名稱</param>
        /// <returns>最符合搜尋條件之屬性</returns>
        public static XmlAttribute FindAttribute(List<XmlAttribute> attr, string name) {
            return attr.Find(delegate(XmlAttribute data) { return data.Name == name; });
        }

        /// <summary>尋找XML資料中的特定名稱屬性</summary>
        /// <param name="data">欲搜尋之XmlData</param>
        /// <param name="name">欲搜尋之名稱</param>
        /// <returns>最符合搜尋條件之屬性</returns>
        public static XmlAttribute FindAttribute(XmlData data, string name) {
            return FindAttribute(data.Attributes, name);
        }
        #endregion
    }
}
