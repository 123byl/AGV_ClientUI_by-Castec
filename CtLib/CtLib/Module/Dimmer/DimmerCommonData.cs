using System;
using System.Collections.Generic;
using System.Linq;

using CtLib.Library;
using CtLib.Module.XML;

namespace CtLib.Module.Dimmer {

	#region Declaration - Enumerations
	/// <summary>調光器組別(通道)</summary>
	public enum Channels : byte {
		/// <summary>第 1 組</summary>
		/// <remarks>如為雙通道調光器，第一組為右側接口</remarks>
		Channel1 = 0,
		/// <summary>第 2 組</summary>
		/// <remarks>如為雙通道調光器，第二組為左側接口</remarks>
		Channel2 = 1,
		/// <summary>第 3 組</summary>
		Channel3 = 2,
		/// <summary>第 4 組</summary>
		Channel4 = 3,
		/// <summary>第 5 組</summary>
		Channel5 = 4
	}

	/// <summary>調光器模式</summary>
	public enum Modes : byte {
		/// <summary>常量</summary>
		Constant = 0,
		/// <summary>外部設定</summary>
		External = 1,
		/// <summary>Strobe</summary>
		Strobe = 2
	}

	/// <summary>燈條顏色</summary>
	public enum LightColor : byte {
		/// <summary>尚未定義的顏色</summary>
		Undefined,
		/// <summary>藍色</summary>
		Blue,
		/// <summary>紅色</summary>
		Red,
		/// <summary>白色</summary>
		White
	}

	/// <summary>燈條樣式</summary>
	public enum LightStyle : byte {
		/// <summary>尚未定義的樣式</summary>
		Undefined,
		/// <summary>條狀光</summary>
		Strip,
		/// <summary>環形光</summary>
		Circle,
		/// <summary>同軸光</summary>
		Coaxial,
		/// <summary>日光燈</summary>
		Fluorescent
	}

	/// <summary>調光器品牌</summary>
	public enum DimmerBrand : byte {
		/// <summary>荃達。2CH/4CH/5CH、500mA、RS232</summary>
		QUAN_DA,
		/// <summary>弘積。5CH、700mA、RS232</summary>
		HJ_CH5I700RS232,
		/// <summary>弘積。4CH、700mA、RS485</summary>
		HJ_CH4I700RS485
	}
	#endregion

	#region Declaration - Supported Class
	/// <summary>調光器相關通訊數值</summary>
	public class LightValue {
		/// <summary>組別 (1~4)</summary>
		public Channels Channel { get; set; }
		/// <summary>電流模式</summary>
		public Modes Mode { get; private set; }
		/// <summary>電流數值 (mA)</summary>
		public int? Value { get; private set; }
		/// <summary>建構調光器數值</summary>
		/// <param name="ch">組別</param>
		/// <param name="mode">電流模式</param>
		/// <param name="lightValue">電流數值 (mA)</param>
		public LightValue(Channels ch, Modes mode, int lightValue) {
			Channel = ch;
			Mode = mode;
			Value = lightValue;
		}
		/// <summary>初始數值以收到的資料為主，並以此解析為調光器數值</summary>
		/// <param name="data">從 RS-232 接收到的資料</param>
		public LightValue(List<byte> data) {
			if (data.Count == 4) {  //數量為4，表示是單一調光器數值，帶有 Channel 數值
				Channel = (Channels)data[0];
				Mode = (Modes)(data[1] & 0x07);
				Value = ((data[1] & 0xF0) << 4) + data[2];
			} else if (data.Count == 3) {   //數量為3，表示是 All Channel，沒有帶 Channel
				Mode = (Modes)(data[0] & 0x07);
				Value = ((data[0] & 0xF0) << 4) + data[1];
			}
		}
	}

	/// <summary>調光器通道資訊</summary>
	public class DimmerChannel : IComparable<DimmerChannel>, IComparer<DimmerChannel> {
		#region Fields
		private LightStyle mStyle = LightStyle.Undefined;
		private LightColor mColor = LightColor.Undefined;
		private Channels mCh = Channels.Channel1;
		private string mComPort = string.Empty;
		private int mValue = -1;
		private string mCmt = string.Empty;
		#endregion

		#region Properties
		/// <summary>取得或設定光源樣式</summary>
		public LightStyle LightShape { get { return mStyle; } set { mStyle = value; } }
		/// <summary>取得或設定光源顏色</summary>
		public LightColor EmitColor { get { return mColor; } set { mColor = value; } }
		/// <summary>取得或設定來源的 COM Port</summary>
		public string Port { get { return mComPort; } set { mComPort = value; } }
		/// <summary>取得或設定電流數值，單位為毫安培(mA)</summary>
		public int CurrentValue { get { return mValue; } set { mValue = value; } }
		/// <summary>取得或設定於調光器內的通道編號</summary>
		public Channels Channel { get { return mCh; } set { mCh = value; } }
		/// <summary>取得或設定此通道的註解</summary>
		public string Comment { get { return mCmt; } set { mCmt = value; } }
		#endregion

		#region Constructors
		/// <summary>建構空白的調光器通道資訊</summary>
		public DimmerChannel() { }

		/// <summary>透過含有參數設定的 <see cref="IXmlData"/> 建構調光器通道資訊</summary>
		/// <param name="xmlData">含有參數設定的 <see cref="IXmlData"/></param>
		public DimmerChannel(XmlElmt xmlData) {
			XmlAttr attr;
			if (xmlData.Attribute("Tunnel", out attr))
				mCh = (Channels)Enum.Parse(typeof(Channels), attr.Value.Replace("_", string.Empty), true);

			XmlElmt subData;
			if (xmlData.Element("Style", out subData))
				mStyle = (LightStyle)Enum.Parse(typeof(LightStyle), subData.Value, true);

			if (xmlData.Element("Color", out subData))
				mColor = (LightColor)Enum.Parse(typeof(LightColor), subData.Value, true);

			if (xmlData.Element("Comment", out subData))
				mCmt = subData.Value;
		}
		#endregion

		#region Public Functions
		/// <summary>取得此調光器通道所描述的 <see cref="IXmlData"/> 設定資訊</summary>
		/// <param name="nodeName">節點名稱</param>
		/// <returns>節點資訊</returns>
		public XmlElmt CreateSettingXml(string nodeName) {
			List<XmlAttr> attrColl = new List<XmlAttr> {
					new XmlAttr("Tunnel", mCh.ToString())
				};
			List<XmlElmt> dataColl = new List<XmlElmt> {
					new XmlElmt("Style", mStyle.ToString()),
					new XmlElmt("Color", mColor.ToString()),
					new XmlElmt("Comment", mCmt)
				};
			return new XmlElmt(
				nodeName,
				string.Empty,
				attrColl,
				dataColl
			);
		}

		/// <summary>取得此調光器通道所描述的 <see cref="IXmlData"/> 數值資訊</summary>
		/// <param name="nodeName">節點名稱</param>
		/// <returns>節點資訊</returns>
		public XmlElmt CreateValueXml(string nodeName) {
			return new XmlElmt(
				nodeName,
				mValue.ToString(),
				new XmlAttr("Port", mComPort),
				new XmlAttr("Channel", mCh.ToString())
			);
		}

		/// <summary>取得此調光器通道所描述的 <see cref="IXmlData"/> 數值資訊</summary>
		/// <param name="dimName">調光器名稱 <see cref="DimmerPack.Name"/></param>
		/// <param name="nodeName">節點名稱</param>
		/// <returns>節點資訊</returns>
		public XmlElmt CreateValueXml(string nodeName, string dimName) {
			return new XmlElmt(
				nodeName,
				mValue.ToString(),
				new XmlAttr("Name", dimName),
				new XmlAttr("Port", mComPort),
				new XmlAttr("Channel", mCh.ToString())
			);
		}

		/// <summary>取得此調光器通道之淺層複製物件</summary>
		/// <returns>複製物件</returns>
		public DimmerChannel Clone() {
			return this.MemberwiseClone() as DimmerChannel;
		}
		#endregion

		#region Overrides
		/// <summary>取得此調光器通道之文字描述</summary>
		/// <returns>描述文字</returns>
		public override string ToString() {
			return string.Format("{0}, {1}, {2}, {3}", mStyle, mColor, mCh, mValue);
		}

		/// <summary>與其他物件進行比較，取得兩物件是否相同。比較除了 <see cref="CurrentValue"/> 之外的屬性</summary>
		/// <param name="obj">欲比較的物件</param>
		/// <returns>(<see langword="true"/>)兩者相同  (<see langword="false"/>)兩者不同</returns>
		public override bool Equals(object obj) {
			if ((object)this == null || obj == null) return false;
			else {
				DimmerChannel ch = obj as DimmerChannel;
				return this == ch;
			}
		}

		/// <summary>取得此物件的湊雜碼</summary>
		/// <returns>湊雜碼</returns>
		public override int GetHashCode() {
			return (int)mStyle ^ (int)mColor ^ (int)mCh ^ mComPort.GetHashCode();
		}

		/// <summary>比較兩個 <see cref="DimmerChannel"/> 是否相等。比較除了 <see cref="CurrentValue"/> 之外的屬性</summary>
		/// <param name="left">欲比較之 <see cref="DimmerChannel"/></param>
		/// <param name="right">欲比較之 <see cref="DimmerChannel"/></param>
		/// <returns>(<see langword="true"/>)<see cref="Port"/>、<seealso cref="LightShape"/>、<seealso cref="Channel"/> 與 <seealso cref="EmitColor"/> 四者相等 (<see langword="false"/>)四者不同</returns>
		public static bool operator ==(DimmerChannel left, DimmerChannel right) {
			if (object.ReferenceEquals(left, right)) return true;
			if ((object)left == null || (object)right == null) return false;
			return left.Port == right.Port && left.LightShape == right.LightShape && left.Channel == right.Channel && left.EmitColor == right.EmitColor;
		}

		/// <summary>比較兩個 <see cref="DimmerChannel"/> 是否不相等。比較除了 <see cref="CurrentValue"/> 之外的屬性</summary>
		/// <param name="left">欲比較之 <see cref="DimmerChannel"/></param>
		/// <param name="right">欲比較之 <see cref="DimmerChannel"/></param>
		/// <returns>(<see langword="true"/>)四者不同 (<see langword="false"/>)<see cref="Port"/>、<seealso cref="LightShape"/>、<seealso cref="Channel"/> 與 <seealso cref="EmitColor"/> 四者相等</returns>
		public static bool operator !=(DimmerChannel left, DimmerChannel right) {
			return !(left == right);
		}

		/// <summary>比較兩個 <see cref="DimmerChannel"/></summary>
		/// <param name="ch">欲比較的通道資訊</param>
		/// <returns>(-1)當前物件小於引數 (0)當前物件等於引數 (1)當前物件大於引數</returns>
		public int CompareTo(DimmerChannel ch) {
			int compare = mStyle.CompareTo(ch.LightShape);
			if (compare != 0) return compare;
			compare = mColor.CompareTo(ch.EmitColor);
			if (compare != 0) return compare;
			compare = mCh.CompareTo(ch.Channel);
			if (compare != 0) return compare;
			compare = mComPort.CompareTo(ch.Port);
			if (compare != 0) return compare;
			compare = mValue.CompareTo(ch.CurrentValue);
			return compare;
		}

		/// <summary>比較兩個 <see cref="DimmerChannel"/></summary>
		/// <param name="ch1">欲被比較的通道資訊</param>
		/// <param name="ch2">欲比較的通道資訊</param>
		/// <returns>(-1)當前物件小於引數 (0)當前物件等於引數 (1)當前物件大於引數</returns>
		public int Compare(DimmerChannel ch1, DimmerChannel ch2) {
			int compare = ch1.LightShape.CompareTo(ch2.LightShape);
			if (compare != 0) return compare;
			compare = ch1.EmitColor.CompareTo(ch2.EmitColor);
			if (compare != 0) return compare;
			compare = ch1.Channel.CompareTo(ch2.Channel);
			if (compare != 0) return compare;
			compare = ch1.Port.CompareTo(ch2.Port);
			if (compare != 0) return compare;
			compare = ch1.CurrentValue.CompareTo(ch2.CurrentValue);
			return compare;
		}
		#endregion
	}

	/// <summary>調光器資訊</summary>
	public class DimmerPack {

		#region Fields
		private string mName = string.Empty;
		private string mComPort = string.Empty;
		private DimmerBrand mBrand = DimmerBrand.QUAN_DA;
		private List<DimmerChannel> mChannels = new List<DimmerChannel>();
		#endregion

		#region Properties
		/// <summary>取得此調光器的自訂義名稱</summary>
		public string Name { get { return mName; } set { mName = value; } }
		/// <summary>取得調光器品牌</summary>
		public DimmerBrand Brand { get { return mBrand; } }
		/// <summary>取得或設定通訊串列埠</summary>
		public string Port { get { return mComPort; } set { mComPort = value; } }
		/// <summary>取得此調光器內部的通道資訊</summary>
		public List<DimmerChannel> Channels { get { return mChannels.ConvertAll(ch => ch.Clone()); } }
		/// <summary>取得此調光器共有多少通道</summary>
		public int TotalChannel { get { return mChannels.Count; } }
		#endregion

		#region Constructors
		/// <summary>建構調光器資訊</summary>
		/// <param name="brand">調光器品牌</param>
		/// <param name="comPort">串列埠，如 "COM3"</param>
		/// <param name="chColl">調光器通道集合</param>
		public DimmerPack(DimmerBrand brand, string comPort, IEnumerable<DimmerChannel> chColl = null) {
			long hashCode = DateTime.Now.ToBinary();        //目前時間的 long 幾乎是唯一的
			mName = "DIMMER " + CtConvert.ToHex(hashCode);
			mBrand = brand;
			mComPort = comPort;

			if (chColl != null) mChannels.AddRange(chColl);
		}

		/// <summary>建構調光器資訊</summary>
		/// <param name="name">此調光器之名稱</param>
		/// <param name="brand">調光器品牌</param>
		/// <param name="comPort">串列埠，如 "COM3"</param>
		/// <param name="chColl">調光器通道集合</param>
		public DimmerPack(string name, DimmerBrand brand, string comPort, IEnumerable<DimmerChannel> chColl = null) {
			mName = name;
			mBrand = brand;
			mComPort = comPort;

			if (chColl != null) mChannels.AddRange(chColl);
		}

		/// <summary>透過 <see cref="IXmlData"/> 來建構調光器資訊</summary>
		/// <param name="xmlData">含有調光器資訊的 <see cref="IXmlData"/></param>
		public DimmerPack(XmlElmt xmlData) {
			mName = xmlData.Attribute("Name").Value;

			XmlElmt childData;
			if (xmlData.Element("/Brand", out childData))
				mBrand = (DimmerBrand)Enum.Parse(typeof(DimmerBrand), childData.Value);

			if (xmlData.Element("/Port", out childData)) mComPort = childData.Value;
			if (xmlData.Element("/Channels", out childData)) {
				childData.Elements().ForEach(
					xml => {
						DimmerChannel ch = new DimmerChannel(xml);
						ch.Port = mComPort;
						mChannels.Add(ch);
					}
				);
			}
		}
		#endregion

		#region Public Methods
		/// <summary>取得此調光器的 <see cref="IXmlData"/> 資訊節點</summary>
		/// <param name="nodeName">節點名稱</param>
		/// <returns>XML 節點</returns>
		public XmlElmt CreateXmlData(string nodeName) {
			int chIdx = 0;
			List<XmlElmt> chXml = mChannels.ConvertAll(ch => ch.CreateSettingXml(string.Format("NO_{0:D2}", chIdx++)));
			List<XmlAttr> attrColl = new List<XmlAttr> {
				new XmlAttr("Name", mName)
			};
			List<XmlElmt> dataColl = new List<XmlElmt> {
				new XmlElmt("Brand", mBrand.ToString()),
				new XmlElmt("Port", mComPort),
				new XmlElmt(
					"Channels",
					chXml
				)
			};
			return new XmlElmt(
				nodeName,
				attrColl,
				dataColl
			);
		}

		/// <summary>取得此調光器數值轉換為相對應的 <see cref="Adept.CtLightCtrlData"/> 建構字串</summary>
		/// <returns>相對應的字串</returns>
		public string CreateLightCtrlInstStr() {
			string codFormat = "new CtLightCtrlData(DimmerBrand.{0}, \"{1}\", {2})";
			List<int> lightValue = new List<int>();
			for (int chCount = 0; chCount < mChannels.Count; chCount++) lightValue.Add(-1);
			mChannels.ForEach(ch => lightValue[(int)ch.Channel] = ch.CurrentValue);
			return string.Format(codFormat, mBrand.ToString(), mComPort, string.Join(", ", lightValue));
		}

		/// <summary>取得此調光器數值轉換為相對應的 <see cref="Adept.ICvtExecutor"/> 建構字串</summary>
		/// <returns>相對應的字串</returns>
		public string CreateExecObjInsStr() {
			string codFormat = "new LightExecutor(DimmerBrand.{0}, \"{1}\", {2})";
			List<int> lightValue = new List<int>();
			for (int chCount = 0; chCount < mChannels.Count; chCount++) lightValue.Add(-1);
			mChannels.ForEach(ch => lightValue[(int)ch.Channel] = ch.CurrentValue);
			return string.Format(codFormat, mBrand.ToString(), mComPort, string.Join(", ", lightValue));
		}

		/// <summary>取得調光器的複製品</summary>
		/// <returns>複製的調光器資訊</returns>
		public DimmerPack Clone() {
			return new DimmerPack(mName, mBrand, mComPort, mChannels.ConvertAll(ch => ch.Clone()));
		}

		/// <summary>清空所有調光器通道</summary>
		public void ClearChannels() {
			mChannels.Clear();
		}

		/// <summary>重新指定調光器通道，此方法不會進行 <see cref="DimmerChannel.Clone()"/>，請確保好參考內容</summary>
		/// <param name="chColl">通道資訊集合</param>
		public void AssignChannels(IEnumerable<DimmerChannel> chColl) {
			mChannels.Clear();
			mChannels.AddRange(chColl);
		}

		/// <summary>加入調光器通道，此方法不會進行 <see cref="DimmerChannel.Clone()"/>，請確保好參考內容</summary>
		/// <param name="ch">通道資訊</param>
		public void AddChannels(params DimmerChannel[] ch) {
			mChannels.AddRange(ch);
		}

		/// <summary>移除調光器通道</summary>
		/// <param name="ch">通道資訊</param>
		public void RemoveChannels(params DimmerChannel[] ch) {
			foreach (var item in ch) {
				mChannels.RemoveAll(val => val == item);
			}
		}

		/// <summary>重設所有通道內的光源電流數值</summary>
		public void ResetChannelValue() {
			for (int idx = 0; idx < mChannels.Count; idx++) {
				mChannels[idx].CurrentValue = -1;
			}
		}

		/// <summary>設定指定通道的光源電流數值</summary>
		/// <param name="ch">指定的通道</param>
		/// <param name="value">數值</param>
		public void SetChannelValue(Channels ch, int value) {
			DimmerChannel chObj = mChannels.Find(val => val.Channel == ch);
			if (chObj != null) chObj.CurrentValue = value;
		}

		/// <summary>複寫通道數值</summary>
		/// <param name="chColl">欲更新的數值來源集合</param>
		public void OverwriteChannelValue(List<DimmerChannel> chColl) {
			foreach (DimmerChannel ch in mChannels) {
				DimmerChannel tar = chColl.Find(val => val == ch);
				if (tar != null) {
					ch.CurrentValue = tar.CurrentValue;
				}
			}
		}

		/// <summary>嘗試切換調光器數值，以當前 <see cref="DimmerChannel.CurrentValue"/> 為準</summary>
		/// <returns>切換調光器是否成功 (<see langword="true"/>)成功 (<see langword="false"/>)失敗</returns>
		public bool TrySwitchLight() {
			bool ret = true;
			try {
				if (mChannels.Exists(ch => ch.CurrentValue > -1)) {
					using (ICtDimmer dimmer = CtDimmerBase.Factory(mBrand)) {
						ret = dimmer.Connect(mComPort) == Stat.SUCCESS;
						var sortLight = mChannels.OrderBy(ch => ch.Channel).Select(ch => ch.CurrentValue);
						if (ret) ret = dimmer.SetLight(sortLight);
					}
				}
			} catch (Exception ex) {
				ret = false;
				Console.WriteLine(ex.Message);
			}
			return ret;
		}
		#endregion

		#region Overrides
		/// <summary>取得此調光器的文字描述</summary>
		/// <returns>文字描述</returns>
		public override string ToString() {
			return string.Format("{0}, {1}, {2} Channels", mBrand, mComPort, TotalChannel);
		}

		/// <summary>與另外的 <see cref="DimmerPack"/> 進行比較</summary>
		/// <param name="obj">欲比較的 <see cref="DimmerPack"/></param>
		/// <returns>(<see langword="true"/>)兩者相等  (<see langword="false"/>)兩者不同</returns>
		public override bool Equals(object obj) {
			bool compare = false;
			DimmerPack pack = obj as DimmerPack;
			if (pack == null) return false;
			compare = (mComPort == pack.Port) && (mBrand == pack.Brand) && (mName == pack.Name);
			if (!compare) return compare;
			compare = mChannels.Count == pack.TotalChannel;
			if (!compare) return compare;
			List<DimmerChannel> tarChColl = pack.Channels;
			for (int idx = 0; idx < mChannels.Count; idx++) {
				if (mChannels[idx] != tarChColl[idx]) {
					compare = false;
					break;
				}
			}
			return compare;
		}

		/// <summary>取得此調光器資訊的湊雜碼</summary>
		/// <returns>湊雜碼</returns>
		public override int GetHashCode() {
			return mName.GetHashCode() ^ mComPort.GetHashCode() ^ (int)mBrand ^ mChannels.GetHashCode();
		}

		/// <summary>比較兩個 <see cref="DimmerPack"/> 是否相同</summary>
		/// <param name="left">欲比較的 <see cref="DimmerPack"/></param>
		/// <param name="right">被比較的 <see cref="DimmerPack"/></param>
		/// <returns>(<see langword="true"/>)兩者相等  (<see langword="false"/>)兩者不同</returns>
		public static bool operator ==(DimmerPack left, DimmerPack right) {
			if (object.ReferenceEquals(left, right)) return true;
			if ((object)left == null || (object)right == null) return false;
			bool compare = (left.Port == right.Port) && (left.Brand == right.Brand) && (left.Name == right.Name);
			if (!compare) return compare;
			compare = left.TotalChannel == right.TotalChannel;
			if (!compare) return compare;
			List<DimmerChannel> beChColl = left.Channels;
			List<DimmerChannel> tarChColl = right.Channels;
			for (int idx = 0; idx < beChColl.Count; idx++) {
				if (beChColl[idx] != tarChColl[idx]) {
					compare = false;
					break;
				}
			}
			return compare;
		}

		/// <summary>比較兩個 <see cref="DimmerPack"/> 是否不同</summary>
		/// <param name="left">欲比較的 <see cref="DimmerPack"/></param>
		/// <param name="right">被比較的 <see cref="DimmerPack"/></param>
		/// <returns>(<see langword="true"/>)兩者不同  (<see langword="false"/>)兩者相等</returns>
		public static bool operator !=(DimmerPack left, DimmerPack right) {
			return !(left == right);
		}
		#endregion
	}
	#endregion
}
