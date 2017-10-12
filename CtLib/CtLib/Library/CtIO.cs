using System;
using System.Collections.Generic;
using System.Linq;

using CtLib.Module.Utility;
using CtLib.Module.XML;

namespace CtLib.Library {
	/// <summary>
	/// I/O 之 XML 檔案操作
	/// <para>使用統一格式之 XML 檔案，方便各專案之使用</para>
	/// <para>如欲建立或修改 XML 檔案可使用 <see cref="CtLib.Forms.TestPlatform.XML_Editor"/></para>
	/// </summary>
	public static class CtIO {

		#region Version

		/// <summary>CtIO 版本訊息</summary>
		/// <remarks><code language="C#">
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
		/// 1.3.0  Ahern [2016/03/21]
		///		\ 以 ICtIO 改寫主功能
		/// 
		/// </code></remarks>
		public static CtVersion Version { get { return new CtVersion(1, 3, 0, "2016/03/21", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Definitions
		/// <summary>初始節點之名稱</summary>
		private static readonly string NODE_STARTER = "CASTEC_IO";
		#endregion

		#region Function - Core
		/// <summary>從現有的 XML 檔案載入</summary>
		/// <param name="filePath">檔案路徑</param>
		/// <param name="ioList">欲儲存之 <see cref="ICtIO"/> 集合物件</param>
		public static void LoadFromXML(string filePath, out List<ICtIO> ioList) {
			List<ICtIO> ioTemp = new List<ICtIO>();

			if (filePath.ToLower().EndsWith(".xml")) {
				XmlAttr devAttr;
				XmlElmt ioXml = CtXML.Load(filePath);
				List<XmlElmt> xmlData = ioXml.Element("Data").Elements();
				foreach (XmlElmt subNode in xmlData) {
					if (subNode.Attribute("Device", out devAttr)) {
						switch (devAttr.Value) {
							case "ADEPT":
								ioTemp.Add(new AceIO(subNode));
								break;
							case "BECKHOFF":
								ioTemp.Add(new BeckhoffIO(subNode));
								break;
							case "DELTA":
								ioTemp.Add(new DeltaIO(subNode));
								break;
							case "WAGO":
								ioTemp.Add(new WagoIO(subNode));
								break;
							default:
								throw new InvalidOperationException("無法解析的 I/O 資料 : " + devAttr.Value);
						}
					} else throw new ArgumentNullException("Device", "無法解析的 I/O 來源裝置資料");
				}
			}

			ioList = ioTemp;
		}

		/// <summary>從現有的 XML 檔案載入，並轉為 Dictionary 供快速查詢使用</summary>
		/// <param name="filePath">檔案路徑</param>
		/// <param name="ioList">欲儲存之 <see cref="ICtIO"/> 集合物件</param>
		public static void LoadFromXML(string filePath, out Dictionary<ushort, ICtIO> ioList) {
			Dictionary<ushort, ICtIO> ioTemp = null;

			if (filePath.ToLower().EndsWith(".xml")) {
				List<ICtIO> tempData;
				LoadFromXML(filePath, out tempData);
				ioTemp = tempData.ToDictionary(val => val.EnumIndex);
			}

			ioList = ioTemp;
		}

		/// <summary>將 <see cref="ICtIO"/> 物件儲存至 XML 檔案</summary>
		/// <param name="filePath">儲存路徑</param>
		/// <param name="ioList">欲儲存之物件集合</param>
		public static void SaveToXML(string filePath, List<ICtIO> ioList) {
			if (filePath.ToLower().EndsWith(".xml")) {
				List<IXmlData> xmlColl = new List<IXmlData>();

				int idx = 0;
				if (ioList.Exists(io => io.Device == Devices.AdeptACE && io.IoType == IOTypes.Output)) {
					xmlColl.Add(new XmlCmt("Adept I/O : Output"));
					xmlColl.AddRange(
						ioList.FindAll(
							io => io.Device == Devices.AdeptACE && io.IoType == IOTypes.Output
						).ConvertAll(
							io => io.CreateXmlData(string.Format("ADEPT_DO{0:D3}", idx++))
						)
					); 
				}

				idx = 0;
				if (ioList.Exists(io => io.Device == Devices.AdeptACE && io.IoType == IOTypes.Input)) {
					xmlColl.Add(new XmlCmt("Adept I/O : Input"));
					xmlColl.AddRange(
						ioList.FindAll(
							io => io.Device == Devices.AdeptACE && io.IoType == IOTypes.Input
						).ConvertAll(
							io => io.CreateXmlData(string.Format("ADEPT_DI{0:D3}", idx++))
						)
					); 
				}

				idx = 0;
				if (ioList.Exists(io => io.Device == Devices.AdeptACE && io.IoType == IOTypes.InOut)) {
					xmlColl.Add(new XmlCmt("Adept I/O : Software"));
					xmlColl.AddRange(
						ioList.FindAll(
							io => io.Device == Devices.AdeptACE && io.IoType == IOTypes.InOut
						).ConvertAll(
							io => io.CreateXmlData(string.Format("ADEPT_SIO{0:D3}", idx++))
						)
					); 
				}

				idx = 0;
				if (ioList.Exists(io => io.Device == Devices.Beckhoff && io.IoType == IOTypes.Output)) {
					xmlColl.Add(new XmlCmt("Beckhoff I/O : Output"));
					xmlColl.AddRange(
						ioList.FindAll(
							io => io.Device == Devices.Beckhoff && io.IoType == IOTypes.Output
						).ConvertAll(
							io => io.CreateXmlData(string.Format("BKF_DO{0:D3}", idx++))
						)
					); 
				}

				idx = 0;
				if (ioList.Exists(io => io.Device == Devices.Beckhoff && io.IoType == IOTypes.Input)) {
					xmlColl.Add(new XmlCmt("Beckhoff I/O : Input"));
					xmlColl.AddRange(
						ioList.FindAll(
							io => io.Device == Devices.Beckhoff && io.IoType == IOTypes.Input
						).ConvertAll(
							io => io.CreateXmlData(string.Format("BKF_DI{0:D3}", idx++))
						)
					); 
				}

				idx = 0;
				if (ioList.Exists(io => io.Device == Devices.Beckhoff && io.IoType == IOTypes.InOut)) {
					xmlColl.Add(new XmlCmt("Beckhoff I/O : Flag"));
					xmlColl.AddRange(
						ioList.FindAll(
							io => io.Device == Devices.Beckhoff && io.IoType == IOTypes.InOut
						).ConvertAll(
							io => io.CreateXmlData(string.Format("BKF_F{0:D3}", idx++))
						)
					); 
				}

				idx = 0;
				if (ioList.Exists(io => io.Device == Devices.DELTA && io.IoType == IOTypes.Output)) {
					xmlColl.Add(new XmlCmt("Delta I/O : Output"));
					xmlColl.AddRange(
						ioList.FindAll(
							io => io.Device == Devices.DELTA && io.IoType == IOTypes.Output
						).ConvertAll(
							io => io.CreateXmlData(string.Format("DELTA_DO{0:D3}", idx++))
						)
					);
				}

				idx = 0;
				if (ioList.Exists(io => io.Device == Devices.DELTA && io.IoType == IOTypes.Input)) {
					xmlColl.Add(new XmlCmt("Delta I/O : Input"));
					xmlColl.AddRange(
						ioList.FindAll(
							io => io.Device == Devices.DELTA && io.IoType == IOTypes.Input
						).ConvertAll(
							io => io.CreateXmlData(string.Format("DELTA_DI{0:D3}", idx++))
						)
					);
				}

				idx = 0;
				if (ioList.Exists(io => io.Device == Devices.DELTA && io.IoType == IOTypes.InOut)) {
					xmlColl.Add(new XmlCmt("Delta I/O : M"));
					xmlColl.AddRange(
						ioList.FindAll(
							io => io.Device == Devices.DELTA && io.IoType == IOTypes.InOut
						).ConvertAll(
							io => io.CreateXmlData(string.Format("DELTA_M{0:D3}", idx++))
						)
					);
				}

				idx = 0;
				if (ioList.Exists(io => io.Device == Devices.WAGO && io.IoType == IOTypes.Output)) {
					xmlColl.Add(new XmlCmt("Wago I/O : Output"));
					xmlColl.AddRange(
						ioList.FindAll(
							io => io.Device == Devices.WAGO && io.IoType == IOTypes.Output
						).ConvertAll(
							io => io.CreateXmlData(string.Format("WAGO_DO{0:D3}", idx++))
						)
					);
				}

				idx = 0;
				if (ioList.Exists(io => io.Device == Devices.WAGO && io.IoType == IOTypes.Input)) {
					xmlColl.Add(new XmlCmt("Wago I/O : Input"));
					xmlColl.AddRange(
						ioList.FindAll(
							io => io.Device == Devices.WAGO && io.IoType == IOTypes.Input
						).ConvertAll(
							io => io.CreateXmlData(string.Format("WAGO_DI{0:D3}", idx++))
						)
					);
				}

				idx = 0;
				if (ioList.Exists(io => io.Device == Devices.WAGO && io.IoType == IOTypes.InOut)) {
					xmlColl.Add(new XmlCmt("Wago I/O : M"));
					xmlColl.AddRange(
						ioList.FindAll(
							io => io.Device == Devices.WAGO && io.IoType == IOTypes.InOut
						).ConvertAll(
							io => io.CreateXmlData(string.Format("WAGO_M{0:D3}", idx++))
						)
					);
				}

				XmlElmt xml = new XmlElmt(
					NODE_STARTER,
					new XmlElmt(
						"Data",
						xmlColl
					)
				);
				
				CtXML.Save(xml, filePath);
			}
		}
		#endregion
	}
}
