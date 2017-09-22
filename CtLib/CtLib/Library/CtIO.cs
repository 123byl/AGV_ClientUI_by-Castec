using CtLib.Module.Ultity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CtLib.Library {
    /// <summary>
    /// I/O 之 XML 檔案操作
    /// <para>使用統一格式之 XML 檔案，方便各專案之使用</para>
    /// <para>如欲建立或修改 XML 檔案可使用 <see cref="CtLib.Forms.TestPlatform.XML_Editor"/></para>
    /// </summary>
    public static class CtIO {

        #region Version

        /// <summary>CtIO 版本訊息</summary>
        /// <remarks><code>
        /// 1.0.0  Ahern [2015/02/04]
        ///     + 從 CtNexcom_IO 獨立至此
        ///     
        /// 1.1.0  Ahern [2015/05/25]
        ///     \ 精簡 SaveToXML
        ///     
        /// 1.1.1  Ahern [2015/05/29]
        ///     \ SaveToXML 改用 CtIOData 自訂的排序
        ///     
        /// 1.2.0  Ahern [2015/06/04]
        ///     + 支援 Delta 相關變數
        /// 
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 2, 0, "2015/06/04", "Ahern Kuo");

        #endregion

        #region Declaration - Definitions
        /// <summary>初始節點之名稱</summary>
        private static readonly string NODE_STARTER = "CASTEC_IO";
        #endregion

        #region Function - Methods
        /// <summary>建立一個新的XML檔案</summary>
        /// <param name="path">儲存檔案之路徑</param>
        private static void CreateXML(string path) {
            List<string> strDoc = new List<string>{
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
                "<" + NODE_STARTER + ">",
                "    <Data>",
                "    </Data>",
                "</" + NODE_STARTER + ">"
            };
            CtFile.WriteFile(path, strDoc);
        }

        private static void SetXmlAttr(CtXML xml, string path, string name, string value) {
            CtXML.XmlAttribute xmlAttr = new CtXML.XmlAttribute();
            xmlAttr.Name = name;
            xmlAttr.Value = value;
            xml.SetAttributes(path, xmlAttr);
        }

        private static void SetAdeptIO(CtXML xml, IOTypes type, List<CtIOData> ioData) {
            if (ioData != null && ioData.Count > 0) {
                string path = "";
                string strInOut = (type == IOTypes.INPUT ? "Input" : (type == IOTypes.OUTPUT ? "Output" : "InOut"));
                string strDIO = (type == IOTypes.INPUT ? "DI" : (type == IOTypes.OUTPUT ? "DO" : "SIO"));
                int index = 0;

                List<CtIOData> ioTemp = ioData.OrderBy(val => val).ToList();
                xml.AddComment(NODE_STARTER + "/Data", "Adept I/O : " + strInOut);
                foreach (CtIOData data in ioTemp) {
                    path = NODE_STARTER + "/Data/ADEPT_" + strDIO + index.ToString("000");

                    SetXmlAttr(xml, path, "Device", "ADEPT");
                    SetXmlAttr(xml, path, "Type", strInOut);
                    SetXmlAttr(xml, path, "IONum", data.IONum.ToString("0000"));
                    SetXmlAttr(xml, path, "Enum", data.EnumIdx.ToString());

                    xml.SetInnerText(path, data.Comment);

                    index++;
                }
            }
        }

        private static void SetBeckhoffIO(CtXML xml, IOTypes type, List<CtIOData> ioData) {
            if (ioData != null && ioData.Count > 0) {
                string path = "";
                string strInOut = (type == IOTypes.INPUT ? "Input" : (type == IOTypes.OUTPUT ? "Output" : "InOut"));
                string strDIO = (type == IOTypes.INPUT ? "DI" : (type == IOTypes.OUTPUT ? "DO" : "SIO"));
                int index = 0;

                List<CtIOData> ioTemp = ioData.OrderBy(val => val).ToList();
                xml.AddComment(NODE_STARTER + "/Data", "Beckhoff I/O : " + strInOut);
                foreach (CtIOData data in ioTemp) {
                    path = NODE_STARTER + "/Data/BKF_" + strDIO + index.ToString("000");

                    SetXmlAttr(xml, path, "Device", "BECKHOFF");
                    SetXmlAttr(xml, path, "Type", strInOut);
                    SetXmlAttr(xml, path, "ID", data.ID);
                    SetXmlAttr(xml, path, "Index", data.Index.ToString());
                    SetXmlAttr(xml, path, "Variable", data.Variable);
                    SetXmlAttr(xml, path, "Enum", data.EnumIdx.ToString());

                    xml.SetInnerText(path, data.Comment);

                    index++;
                }
            }
        }

        private static void SetDeltaIO(CtXML xml, IOTypes type, List<CtIOData> ioData) {
            if (ioData != null && ioData.Count > 0) {
                string path = "";
                string strInOut = (type == IOTypes.INPUT ? "Input" : (type == IOTypes.OUTPUT ? "Output" : "InOut"));
                string strDIO = (type == IOTypes.INPUT ? "DI" : (type == IOTypes.OUTPUT ? "DO" : "SIO"));
                int index = 0;

                List<CtIOData> ioTemp = ioData.OrderBy(val => val).ToList();
                xml.AddComment(NODE_STARTER + "/Data", "Delta I/O : " + strInOut);
                foreach (CtIOData data in ioTemp) {
                    path = NODE_STARTER + "/Data/DELTA_" + strDIO + index.ToString("000");

                    SetXmlAttr(xml, path, "Device", "DELTA");
                    SetXmlAttr(xml, path, "Type", strInOut);
                    SetXmlAttr(xml, path, "ID", data.ID);
                    SetXmlAttr(xml, path, "Variable", data.Variable);
                    SetXmlAttr(xml, path, "Enum", data.EnumIdx.ToString());

                    xml.SetInnerText(path, data.Comment);

                    index++;
                }
            }
        }

        private static void SetWagoIO(CtXML xml, IOTypes type, List<CtIOData> ioData) {
            if (ioData != null && ioData.Count > 0) {
                string path = "";
                string strInOut = (type == IOTypes.INPUT ? "Input" : (type == IOTypes.OUTPUT ? "Output" : "InOut"));
                string strDIO = (type == IOTypes.INPUT ? "DI" : (type == IOTypes.OUTPUT ? "DO" : "SIO"));
                int index = 0;

                List<CtIOData> ioTemp = ioData.OrderBy(val => val).ToList();
                xml.AddComment(NODE_STARTER + "/Data", "Wago I/O : " + strInOut);
                foreach (CtIOData data in ioTemp) {
                    path = NODE_STARTER + "/Data/WAGO_" + strDIO + index.ToString("000");

                    SetXmlAttr(xml, path, "Device", "WAGO");
                    SetXmlAttr(xml, path, "Type", strInOut);
                    SetXmlAttr(xml, path, "ID", data.ID);
                    SetXmlAttr(xml, path, "IONum", data.IONum.ToString());
                    SetXmlAttr(xml, path, "RegNum", data.RegNum.ToString());
                    SetXmlAttr(xml, path, "RegBit", data.RegBit.ToString());
                    SetXmlAttr(xml, path, "Enum", data.EnumIdx.ToString());

                    xml.SetInnerText(path, data.Comment);

                    index++;
                }
            }
        }
        #endregion

        #region Function - Core
        /// <summary>從現有的 XML 檔案載入</summary>
        /// <param name="filePath">檔案路徑</param>
        /// <param name="ioList">欲儲存之 <see cref="CtIOData"/> 集合物件</param>
        public static void LoadFromXML(string filePath, out List<CtIOData> ioList) {
            List<CtIOData> ioTemp = new List<CtIOData>();

            if (filePath.ToLower().EndsWith(".xml")) {
                CtXML xml = new CtXML();
                if (xml.Load(filePath) == Stat.SUCCESS) {
                    List<CtXML.XmlData> xmlData = xml.GetAllValue(NODE_STARTER + "/Data");
                    foreach (CtXML.XmlData subData in xmlData) {
                        CtIOData ioData = new CtIOData();
                        foreach (CtXML.XmlAttribute xmlAttr in subData.Attributes) {
                            if (xmlAttr.Name == "IONum")
                                ioData.IONum = int.Parse(xmlAttr.Value);
                            else if (xmlAttr.Name == "ID")
                                ioData.ID = xmlAttr.Value;
                            else if (xmlAttr.Name == "Device" && xmlAttr.Value == "ADEPT")
                                ioData.Device = Devices.ADEPT_ACE;
                            else if (xmlAttr.Name == "Device" && xmlAttr.Value == "BECKHOFF")
                                ioData.Device = Devices.BECKHOFF_PLC;
                            else if (xmlAttr.Name == "Device" && xmlAttr.Value == "DELTA")
                                ioData.Device = Devices.DELTA;
                            else if (xmlAttr.Name == "Device" && xmlAttr.Value == "WAGO")
                                ioData.Device = Devices.WAGO;
                            else if (xmlAttr.Name == "Type")
                                ioData.Type = (IOTypes)Enum.Parse(typeof(IOTypes), xmlAttr.Value, true);
                            else if (xmlAttr.Name == "Variable")
                                ioData.Variable = xmlAttr.Value;
                            else if (xmlAttr.Name == "Index")
                                ioData.Index = int.Parse(xmlAttr.Value);
                            else if (xmlAttr.Name == "RegNum")
                                ioData.RegNum = ushort.Parse(xmlAttr.Value);
                            else if (xmlAttr.Name == "RegBit")
                                ioData.RegBit = byte.Parse(xmlAttr.Value);
                            else if (xmlAttr.Name == "Enum")
                                ioData.EnumIdx = ushort.Parse(xmlAttr.Value);
                        }
                        ioData.Comment = subData.InnerText;
                        ioTemp.Add(ioData);
                    }
                }
            }

            ioList = ioTemp;
        }

        /// <summary>從現有的 XML 檔案載入，並轉為 Dictionary 供快速查詢使用</summary>
        /// <param name="filePath">檔案路徑</param>
        /// <param name="ioList">欲儲存之 <see cref="CtIOData"/> 集合物件</param>
        public static void LoadFromXML(string filePath, out Dictionary<ushort, CtIOData> ioList) {
            Dictionary<ushort, CtIOData> ioTemp = null;

            if (filePath.ToLower().EndsWith(".xml")) {
                List<CtIOData> tempData;
                LoadFromXML(filePath, out tempData);
                ioTemp = tempData.ToDictionary(val => val.EnumIdx);
            }

            ioList = ioTemp;
        }

        /// <summary>將 <see cref="CtIOData"/> 物件儲存至 XML 檔案</summary>
        /// <param name="filePath">儲存路徑</param>
        /// <param name="ioList">欲儲存之物件集合</param>
        public static void SaveToXML(string filePath, List<CtIOData> ioList) {
            if (filePath.ToLower().EndsWith(".xml")) {

                CreateXML(filePath);
                CtXML xml = new CtXML(filePath);

                /*-- Adept Input --*/
                List<CtIOData> aceIn = ioList.FindAll(data => data.Device == Devices.ADEPT_ACE && data.Type == IOTypes.INPUT);
                SetAdeptIO(xml, IOTypes.INPUT, aceIn);

                /*-- Adept Output --*/
                List<CtIOData> aceOut = ioList.FindAll(data => data.Device == Devices.ADEPT_ACE && data.Type == IOTypes.OUTPUT);
                SetAdeptIO(xml, IOTypes.OUTPUT, aceOut);

                /*-- Adept Output --*/
                List<CtIOData> aceSIO = ioList.FindAll(data => data.Device == Devices.ADEPT_ACE && data.Type == IOTypes.INOUT);
                SetAdeptIO(xml, IOTypes.INOUT, aceSIO);

                /*-- Beckhoff Input --*/
                List<CtIOData> bkfIn = ioList.FindAll(data => data.Device == Devices.BECKHOFF_PLC && data.Type == IOTypes.INPUT);
                SetBeckhoffIO(xml, IOTypes.INPUT, bkfIn);

                /*-- Beckhoff Output --*/
                List<CtIOData> bkfOut = ioList.FindAll(data => data.Device == Devices.BECKHOFF_PLC && data.Type == IOTypes.OUTPUT);
                SetBeckhoffIO(xml, IOTypes.OUTPUT, bkfOut);

                /*-- Wago Input --*/
                List<CtIOData> wgIn = ioList.FindAll(data => data.Device == Devices.WAGO && data.Type == IOTypes.INPUT);
                SetWagoIO(xml, IOTypes.INPUT, wgIn);

                /*-- Wago Output --*/
                List<CtIOData> wgOut = ioList.FindAll(data => data.Device == Devices.WAGO && data.Type == IOTypes.OUTPUT);
                SetWagoIO(xml, IOTypes.OUTPUT, wgOut);

                /*-- Delta Input --*/
                List<CtIOData> dtIn = ioList.FindAll(data => data.Device == Devices.DELTA && data.Type == IOTypes.INPUT);
                SetDeltaIO(xml, IOTypes.INPUT, dtIn);

                /*-- Delta Output --*/
                List<CtIOData> dtOut = ioList.FindAll(data => data.Device == Devices.DELTA && data.Type == IOTypes.OUTPUT);
                SetDeltaIO(xml, IOTypes.OUTPUT, dtOut);
            }
        }
        #endregion
    }
}
