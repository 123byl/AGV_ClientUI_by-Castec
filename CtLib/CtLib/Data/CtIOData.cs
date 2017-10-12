using System;
using System.Collections.Generic;

using CtLib.Module.Utility;
using CtLib.Module.XML;

namespace CtLib.Library {

	/*----- Version --------------------------

	1.0.0	Ahern	[2015/02/04]
		+ 從 CtNexcom_IO 獨立至此

	1.1.0	Ahern	[2015/05/29]
		+ Inherit IComarable for .Sort() and .OrderBy()
		+ Inherit IComparer<CtIOData> where may need Comparator

	1.2.0	Ahern	[2016/03/21]
		- CtIOData
		+ ICtIO
		+ AceIO、BeckhoffIO、DeltaIO、WagoIO 並繼承 ICtIO
		+ 相關複寫方法

	----------------------------------------*/

	/// <summary>
	/// I/O 相關資訊介面
	/// <para>如 I/O 編號、名稱、狀態等等</para>
	/// </summary>
	public interface ICtIO : IComparable {

		#region Properties
		/// <summary>此 I/O 註解</summary>
		string Comment { get; }
		/// <summary>此 I/O 來源裝置，如 Adept、Beckhoff 等</summary>
		Devices Device { get; }
		/// <summary>此 I/O 所對應到的裝置索引 (適用於多台同類裝置)</summary>
		/// <remarks>如同時有兩台 Beckhoff 等，一台為 1 另台為 2，即可用於區分此 I/O 隸屬何台裝置</remarks>
		byte DeviceIndex { get; }
		/// <summary>對照 I/O Enumeration 之索引值。如 HuaweiIO.DO_Y0001 = 30, 則此值即為 30</summary>
		ushort EnumIndex { get; }
		/// <summary>此 I/O 類型，如 Input、Output</summary>
		IOTypes IoType { get; }
		/// <summary>此 I/O 當前狀態</summary>
		bool State { get; }
		#endregion

		#region Methods

		/// <summary>取得 I/O 資訊的淺層複製</summary>
		/// <returns>複製的物件</returns>
		ICtIO Clone();

		/// <summary>建立此 I/O 資料的 XML 節點</summary>
		/// <param name="nodeName">此節點名稱</param>
		/// <returns>此 I/O 資訊所對應的 XML 節點</returns>
		XmlElmt CreateXmlData(string nodeName);

		/// <summary>更改註解</summary>
		/// <param name="cmt">新的註解</param>
		void SetComment(string cmt);

		/// <summary>更改 Enumeration 索引值</summary>
		/// <param name="idx">欲更改的索引值。例如 enum IoList { DO_001 = 29 } 則此處帶入 29</param>
		void SetEnumIndex(ushort idx);

		/// <summary>更改當前 I/O 狀態</summary>
		/// <param name="stt">欲更改的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		void SetState(bool stt);

		
		#endregion

	}

	/// <summary>Adept 專用之 I/O 資訊</summary>
	public class AceIO : ICtIO, IComparer<AceIO> {

		#region Private Fields
		private string mComment = string.Empty;
		private bool mCurStt = false;
		private byte mDevIdx = 0;
		private ushort mEnumIdx = 0;
		private int mIoNum = -1;
		private IOTypes mType = IOTypes.InOut;
		#endregion

		#region Properties
		/// <summary>此 I/O 註解</summary>
		public string Comment { get { return mComment; } }
		/// <summary>此 I/O 來源裝置，為 Adept ACE</summary>
		public Devices Device { get { return Devices.AdeptACE; } }
		/// <summary>此 I/O 所對應到的裝置索引 (適用於多台同類裝置)</summary>
		/// <remarks>例如: (0)CX/EX (1)Robot1 (2)Robot2</remarks>
		public byte DeviceIndex { get { return mDevIdx; } }
		/// <summary>對照 I/O Enumeration 之索引值。如 HuaweiIO.DO_0097 = 30, 則此值即為 30</summary>
		public ushort EnumIndex { get { return mEnumIdx; } }
		/// <summary>於 Adept ACE 或相關軟體上所定義的 I/O 編號</summary>
		public int IoNum { get { return mIoNum; } }
		/// <summary>
		/// 此 I/O 類型，如 Input、Output
		/// <para>如為 Software I/O，請設定為 <see cref="IOTypes.InOut"/></para>
		/// </summary>
		public IOTypes IoType { get { return mType; } }
		/// <summary>此 I/O 當前狀態</summary>
		public bool State { get { return mCurStt; } }
		#endregion

		#region Constructors
		/// <summary>建構 Adept 專用之 I/O 資訊</summary>
		/// <param name="ioNum">於 Adept ACE 或相關軟體上所定義的 I/O 編號</param>
		/// <param name="comment">此 I/O 註解</param>
		/// <param name="enumIdx">對照 I/O Enumeration 之索引值</param>
		/// <param name="defStt">預設的 I/O 狀態</param>
		public AceIO(int ioNum, string comment = "", ushort enumIdx = 0, bool defStt = false) {
			mIoNum = IoNum;
			mComment = comment;
			mEnumIdx = enumIdx;
			mCurStt = false;

			if (mIoNum < 1000) mType = IOTypes.Output;
			else if (mIoNum < 2000) mType = IOTypes.Input;
			else mType = IOTypes.InOut;
		}

		/// <summary>使用 <see cref="XmlElmt"/> 建構 Adept 專用之 I/O 資訊</summary>
		/// <param name="xmlData">含有 Adept I/O 資訊之 XML 節點</param>
		public AceIO(XmlElmt xmlData) {

			XmlAttr attr = xmlData.Attribute("Device");
			if (attr == null || attr.Value != "ADEPT") throw new ArgumentException("此 XML 節點並非 Adept 之 I/O 資料", "Device");

			if (xmlData.Attribute("IONum", out attr)) mIoNum = int.Parse(attr.Value);

			/* 使用 XML 內建 */
			//xmlData.FindAttribute("Type", out attr);
			//if (attr != null) mType = (IOTypes)Enum.Parse(typeof(IOTypes), attr.Value, true);

			/* 直接判斷 */
			if (mIoNum < 1000) mType = IOTypes.Output;
			else if (mIoNum < 2000) mType = IOTypes.Input;
			else mType = IOTypes.InOut;

			if (xmlData.Attribute("Enum", out attr)) mEnumIdx = ushort.Parse(attr.Value);
			
			if (xmlData.Attribute("DevNum", out attr)) mDevIdx = byte.Parse(attr.Value);

			mComment = xmlData.Value;
		}
		#endregion

		#region Public Operations
		/// <summary>取得此 I/O 資訊的淺層複製</summary>
		/// <returns>複製的物件</returns>
		/// <remarks>I/O 非 reference type，故直接用 MemberwiseClone，如未來有問題再改用深層複製</remarks>
		public ICtIO Clone() {
			return this.MemberwiseClone() as AceIO;
		}

		/// <summary>更改對應 Adept ACE 之 I/O 編號</summary>
		/// <param name="num">I/O 編號</param>
		public void SetIoNum(int num) { mIoNum = num; }
		#endregion

		#region Inheritance Implements
		int IComparable.CompareTo(object obj) {
			if (obj is AceIO) return mIoNum.CompareTo((obj as AceIO).IoNum);    //如果 obj 是 null 就直接讓他自己跳 Exception 吧
			else throw new InvalidCastException(string.Format("無法比較。引數型態 {0} 非 AceIO 可比較之型態", obj.GetType()));
		}

		int IComparer<AceIO>.Compare(AceIO x, AceIO y) {
			return x.IoNum.CompareTo(y.IoNum);
		}

		/// <summary>建立可使用於 <see cref="CtIO"/> 之 XML 節點</summary>
		/// <param name="nodeName">此 XML 節點名稱</param>
		/// <returns>此 I/O 資訊所對應的 XML 節點</returns>
		public XmlElmt CreateXmlData(string nodeName) {
			return new XmlElmt(
				nodeName,
				mComment,
				new XmlAttr("Device", "ADEPT"),
				new XmlAttr("DevNum", mDevIdx.ToString()),
				new XmlAttr("Type", (mType == IOTypes.InOut ? "InOut" : (mType == IOTypes.Input ? "Input" : "Output"))),
				new XmlAttr("IONum", mIoNum.ToString()),
				new XmlAttr("Enum", mEnumIdx.ToString())
			);
		}

		/// <summary>更改註解</summary>
		/// <param name="cmt">新的註解</param>
		public void SetComment(string cmt) { mComment = cmt; }

		/// <summary>更改的 Enumeration 索引值</summary>
		/// <param name="idx">欲更改的索引值。例如 enum IoList { DO_001 = 29 } 則此處帶入 29</param>
		public void SetEnumIndex(ushort idx) { mEnumIdx = idx; }

		/// <summary>更改當前 I/O 狀態</summary>
		/// <param name="stt">欲更改的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		public void SetState(bool stt) { mCurStt = stt; }
		#endregion

		#region Overrides
		/// <summary>將 AceIO 轉以 <see cref="string"/> 描述</summary>
		/// <returns>AceIO 之描述字串</returns>
		public override string ToString() {
			return string.Format("{0}, Enum = {1}, DevIdx = {2}", mIoNum.ToString(), mEnumIdx.ToString(), mDevIdx.ToString());
		}

		/// <summary>比較兩個 AceIO 是否相等。以 <see cref="IoNum"/> 做比較</summary>
		/// <param name="obj">欲比較的物件</param>
		/// <returns>(<see langword="true"/>)<see cref="IoNum"/>兩者相等 (<see langword="false"/>)兩者不同</returns>
		public override bool Equals(object obj) {
			if (obj is AceIO && obj != null) {
				AceIO tar = obj as AceIO;
				return mIoNum == tar.IoNum;
			} else return false;
		}

		/// <summary>取得此物件之雜湊碼</summary>
		/// <returns>雜湊碼</returns>
		public override int GetHashCode() {
			return mIoNum ^ mEnumIdx ^ (int)Devices.AdeptACE;
		}

		/// <summary>比較兩個 AceIO 是否相等。以 <see cref="IoNum"/> 做比較</summary>
		/// <param name="x">欲比較之 AceIO</param>
		/// <param name="y">欲比較之 AceIO</param>
		/// <returns>(<see langword="true"/>)<see cref="IoNum"/>兩者相等 (<see langword="false"/>)兩者不同</returns>
		public static bool operator ==(AceIO x, AceIO y) {
			if ((object)x != null && (object)y != null) {
				return x.IoNum == y.IoNum;
			} else return false;
		}

		/// <summary>比較兩個 AceIO 是否不同。以 <see cref="IoNum"/> 做比較</summary>
		/// <param name="x">欲比較之 AceIO</param>
		/// <param name="y">欲比較之 AceIO</param>
		/// <returns>(<see langword="true"/>)兩者不同 (<see langword="false"/>)<see cref="IoNum"/>兩者相等</returns>
		public static bool operator !=(AceIO x, AceIO y) {
			return !(x == y);
		}
		#endregion
	}

	/// <summary>Beckhoff 專用之 I/O 資訊</summary>
	public class BeckhoffIO : ICtIO, IComparer<BeckhoffIO> {

		#region Private Fields
		private string mComment = string.Empty;
		private bool mCurStt = false;
		private byte mDevIdx = 0;
		private ushort mEnumIdx = 0;
		private string mIoName = string.Empty;
		private int mMapIdx = -1;
		private string mVar = string.Empty;
		private IOTypes mType = IOTypes.InOut;
		#endregion

		#region Properties
		/// <summary>
		/// 對應於 Beckhoff PLC 內部 I/P Mapping Array 之索引值
		/// <para>如 TRAYInput[8] := TRAY.pI_Door 則此數值為 8</para>
		/// </summary>
		public int ArrayIndex { get { return mMapIdx; } }
		/// <summary>此 I/O 註解</summary>
		public string Comment { get { return mComment; } }
		/// <summary>此 I/O 來源裝置，為 Beckhoff</summary>
		public Devices Device { get { return Devices.Beckhoff; } }
		/// <summary>此 I/O 所對應到的裝置索引 (適用於多台同類裝置)</summary>
		/// <remarks>例如: (1)Loader PLC (2)Unloader PLC</remarks>
		public byte DeviceIndex { get { return mDevIdx; } }
		/// <summary>對照 I/O Enumeration 之索引值。如 HuaweiIO.DO_Y0011 = 30, 則此值即為 30</summary>
		public ushort EnumIndex { get { return mEnumIdx; } }
		/// <summary>對應 I/O 表之名稱，如 Y0011</summary>
		public string IoName { get { return mIoName; } }
		/// <summary>
		/// 此 I/O 類型，如 Input、Output
		/// <para>如為欲加入 Flag，請設定為 <see cref="IOTypes.InOut"/></para>
		/// </summary>
		public IOTypes IoType { get { return mType; } }
		/// <summary>此 I/O 當前狀態</summary>
		public bool State { get { return mCurStt; } }
		/// <summary>對應 Beckhoff 之變數名稱，如 MAIN.pQ_DoorLock</summary>
		public string Variable { get { return mVar; } }
		#endregion

		#region Constructors
		/// <summary>建構 Beckhoff 專用之 I/O 資訊</summary>
		/// <param name="name">對應 I/O 表之名稱，如 Y0011</param>
		/// <param name="type">此 I/O 類型，如 Input、Output</param>
		/// <param name="varName">對應 Beckhoff 之變數名稱，如 MAIN.pQ_DoorLock</param>
		/// <param name="comment">此 I/O 註解</param>
		/// <param name="aryIdx">對應於 Beeckhoff PLC 內部 I/P Mapping Array 之索引值</param>
		/// <param name="enumIdx">對照 I/O Enumeration 之索引值。如 HuaweiIO.DO_Y0011 = 30, 則此值即為 30</param>
		/// <param name="devIdx">此 I/O 所對應到的裝置索引 (適用於多台同類裝置)</param>
		public BeckhoffIO(string name, IOTypes type, string varName, string comment = "", int aryIdx = -1, ushort enumIdx = 0, byte devIdx = 0) {
			mIoName = name;
			mType = type;
			mVar = varName;
			mComment = comment;
			mMapIdx = aryIdx;
			mEnumIdx = enumIdx;
			mDevIdx = devIdx;
		}

		/// <summary>使用 <see cref="XmlElmt"/> 建構 Beckhoff 專用之 I/O 資訊</summary>
		/// <param name="xmlData">含有 Beckhoff I/O 資訊之 XML 節點</param>
		public BeckhoffIO(XmlElmt xmlData) {

			XmlAttr attr = xmlData.Attribute("Device");
			if (attr == null || attr.Value != "BECKHOFF") throw new ArgumentException("此 XML 節點並非 Beckhoff 之 I/O 資料", "Device");

			if (xmlData.Attribute("ID", out attr)) mIoName = attr.Value;

			if (xmlData.Attribute("Type", out attr)) mType = (IOTypes)Enum.Parse(typeof(IOTypes), attr.Value, true);
			
			if (xmlData.Attribute("Enum", out attr)) mEnumIdx = ushort.Parse(attr.Value);
			
			if (xmlData.Attribute("DevNum", out attr)) mDevIdx = byte.Parse(attr.Value);

			if (xmlData.Attribute("Index", out attr)) mMapIdx = int.Parse(attr.Value);
			
			if (xmlData.Attribute("Variable", out attr)) mVar = attr.Value;

			mComment = xmlData.Value;
		}
		#endregion

		#region Public Operations
		/// <summary>取得此 I/O 資訊的淺層複製</summary>
		/// <returns>複製的物件</returns>
		/// <remarks>I/O 非 reference type，故直接用 MemberwiseClone，如未來有問題再改用深層複製</remarks>
		public ICtIO Clone() {
			return this.MemberwiseClone() as BeckhoffIO;
		}

		/// <summary>設定對應 Beckhoff PLC 之變數</summary>
		/// <param name="varName">對應之變數。例如 Inputs[31] := Main.pF_DoorLock; 則此處帶入 Main.pF_DoorLock</param>
		public void SetVariable(string varName) { mVar = varName; }

		/// <summary>設定對應的 I/O 陣列索引</summary>
		/// <param name="idx">欲指定的對應陣列索引。例如 Inputs[31] := Main.pF_DoorLock; 則此處帶入 31</param>
		public void SetArrayIndex(int idx) { mMapIdx = idx; }
		#endregion

		#region Inheritance Implements
		int IComparable.CompareTo(object obj) {
			if (obj is BeckhoffIO) {
				BeckhoffIO tar = obj as BeckhoffIO;
				/* 先比較是不是都是 X 或 Y */
				if (mIoName[0] == tar.IoName[0]) {
					int ioA, ioB;
					if (int.TryParse(mIoName.ToLower().Replace("x", "").Replace("y", ""), out ioA)
						&& int.TryParse(tar.IoName.ToLower().Replace("x", "").Replace("y", ""), out ioB))
						return ioA.CompareTo(ioB);  //同樣的 X/Y 情況下，將後面的數字轉成 int 並直接比較大小
					else return string.Compare(mIoName, tar.IoName);   //如果是 X/Y 之外的新成員，直接以字串比較

					/* 如果 X/Y 不同，直接使用字串比較就可 */
				} else return string.Compare(mIoName, tar.IoName);
			} else throw new InvalidCastException(string.Format("無法比較。引數型態 {0} 非 BeckhoffIO 可比較之型態", obj.GetType()));
		}

		int IComparer<BeckhoffIO>.Compare(BeckhoffIO x, BeckhoffIO y) {
			/* 先比較是不是都是 X 或 Y */
			if (x.IoName[0] == y.IoName[0]) {
				int ioA, ioB;
				if (int.TryParse(x.IoName.ToLower().Replace("x", "").Replace("y", ""), out ioA)
					&& int.TryParse(y.IoName.ToLower().Replace("x", "").Replace("y", ""), out ioB))
					return ioA.CompareTo(ioB);  //同樣的 X/Y 情況下，將後面的數字轉成 int 並直接比較大小
				else return string.Compare(x.IoName, y.IoName);   //如果是 X/Y 之外的新成員，直接以字串比較

				/* 如果 X/Y 不同，直接使用字串比較就可 */
			} else return string.Compare(x.IoName, y.IoName);
		}

		/// <summary>建立可使用於 <see cref="CtIO"/> 之 XML 節點</summary>
		/// <param name="nodeName">此 XML 節點名稱</param>
		/// <returns>此 I/O 資訊所對應的 XML 節點</returns>
		public XmlElmt CreateXmlData(string nodeName) {
			return new XmlElmt(
				nodeName,
				mComment,
				new XmlAttr("Device", "BECKHOFF"),
				new XmlAttr("DevNum", mDevIdx.ToString()),
				new XmlAttr("Type", (mType == IOTypes.InOut ? "InOut" : (mType == IOTypes.Input ? "Input" : "Output"))),
				new XmlAttr("ID", mIoName),
				new XmlAttr("Index", mMapIdx.ToString()),
				new XmlAttr("Variable", mVar),
				new XmlAttr("Enum", mEnumIdx.ToString())
			);
		}

		/// <summary>更改註解</summary>
		/// <param name="cmt">新的註解</param>
		public void SetComment(string cmt) { mComment = cmt; }

		/// <summary>更改的 Enumeration 索引值</summary>
		/// <param name="idx">欲更改的索引值。例如 enum IoList { DO_001 = 29 } 則此處帶入 29</param>
		public void SetEnumIndex(ushort idx) { mEnumIdx = idx; }

		/// <summary>更改當前 I/O 狀態</summary>
		/// <param name="stt">欲更改的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		public void SetState(bool stt) { mCurStt = stt; }
		#endregion

		#region Overrides
		/// <summary>將 BeckhoffIO 轉以 <see cref="string"/> 描述</summary>
		/// <returns>BeckhoffIO 之描述字串</returns>
		public override string ToString() {
			return string.Format("{0}, {1}, AryIdx = {2}, DevIdx = {3}, Enum = {4}", mIoName, mVar, mMapIdx.ToString(), mDevIdx.ToString(), mEnumIdx.ToString());
		}

		/// <summary>比較兩個 BeckhoffIO 是否相等。以 <see cref="IoName"/> 與 <seealso cref="Variable"/> 做比較</summary>
		/// <param name="obj">欲比較的物件</param>
		/// <returns>(<see langword="true"/>)<see cref="IoName"/> 與 <seealso cref="Variable"/> 兩者均相等 (<see langword="false"/>)兩者不同</returns>
		public override bool Equals(object obj) {
			if (obj is BeckhoffIO && obj != null) {
				BeckhoffIO tar = obj as BeckhoffIO;
				return mIoName == tar.IoName && mVar == tar.Variable;
			} else return false;
		}

		/// <summary>取得此物件之雜湊碼</summary>
		/// <returns>雜湊碼</returns>
		public override int GetHashCode() {
			return mIoName.GetHashCode() ^ mVar.GetHashCode() ^ mEnumIdx ^ (int)Devices.Beckhoff;
		}

		/// <summary>比較兩個 BeckhoffIO 是否相等。以 <see cref="IoName"/> 與 <seealso cref="Variable"/> 做比較</summary>
		/// <param name="x">欲比較之 BeckhoffIO</param>
		/// <param name="y">欲比較之 BeckhoffIO</param>
		/// <returns>(<see langword="true"/>)<see cref="IoName"/> 與 <seealso cref="Variable"/>兩者相等 (<see langword="false"/>)兩者不同</returns>
		public static bool operator ==(BeckhoffIO x, BeckhoffIO y) {
			if ((object)x != null && (object)y != null) {
				return x.IoName == y.IoName && x.Variable == y.Variable;
			} else return false;
		}

		/// <summary>比較兩個 BeckhoffIO 是否不同。以 <see cref="IoName"/> 與 <seealso cref="Variable"/> 做比較</summary>
		/// <param name="x">欲比較之 BeckhoffIO</param>
		/// <param name="y">欲比較之 BeckhoffIO</param>
		/// <returns>(<see langword="true"/>)兩者不同 (<see langword="false"/>)<see cref="IoName"/> 與 <seealso cref="Variable"/>兩者相等</returns>
		public static bool operator !=(BeckhoffIO x, BeckhoffIO y) {
			return !(x == y);
		}
		#endregion
	}

	/// <summary>Delta 專用之 I/O 資訊</summary>
	public class DeltaIO : ICtIO, IComparer<DeltaIO> {

		#region Private Fields
		private string mComment = string.Empty;
		private bool mCurStt = false;
		private byte mDevIdx = 0;
		private ushort mEnumIdx = 0;
		private string mIoName = string.Empty;
		private string mVar = string.Empty;
		private IOTypes mType = IOTypes.InOut;
		#endregion

		#region Properties
		/// <summary>此 I/O 註解</summary>
		public string Comment { get { return mComment; } }
		/// <summary>此 I/O 來源裝置，為 Delta</summary>
		public Devices Device { get { return Devices.DELTA; } }
		/// <summary>此 I/O 所對應到的裝置索引 (適用於多台同類裝置)</summary>
		/// <remarks>例如: (1)Tube (2)Tape</remarks>
		public byte DeviceIndex { get { return mDevIdx; } }
		/// <summary>對照 I/O Enumeration 之索引值。如 HuaweiIO.DO_Y0011 = 30, 則此值即為 30</summary>
		public ushort EnumIndex { get { return mEnumIdx; } }
		/// <summary>對應 I/O 表之名稱，如 Y0011</summary>
		public string IoName { get { return mIoName; } }
		/// <summary>
		/// 此 I/O 類型，如 Input、Output
		/// <para>如為欲加入 M 屬性，請設定為 <see cref="IOTypes.InOut"/></para>
		/// </summary>
		public IOTypes IoType { get { return mType; } }
		/// <summary>此 I/O 當前狀態</summary>
		public bool State { get { return mCurStt; } }
		/// <summary>對應 Delta 之變數名稱，如 M208、Y0、X0</summary>
		public string Variable { get { return mVar; } }
		#endregion

		#region Constructors
		/// <summary>建構 Delta 專用之 I/O 資訊</summary>
		/// <param name="name">對應 I/O 表之名稱，如 Y0011</param>
		/// <param name="type">此 I/O 類型，如 Input、Output</param>
		/// <param name="varName">對應 Delta 之變數名稱，如 M33、Y11、X12</param>
		/// <param name="comment">此 I/O 註解</param>
		/// <param name="enumIdx">對照 I/O Enumeration 之索引值。如 HuaweiIO.DO_Y0011 = 30, 則此值即為 30</param>
		/// <param name="devIdx">此 I/O 所對應到的裝置索引 (適用於多台同類裝置)</param>
		public DeltaIO(string name, IOTypes type, string varName, string comment = "", ushort enumIdx = 0, byte devIdx = 0) {
			mIoName = name;
			mType = type;
			mVar = varName;
			mComment = comment;
			mEnumIdx = enumIdx;
			mDevIdx = devIdx;
		}

		/// <summary>使用 <see cref="XmlElmt"/> 建構 Delta 專用之 I/O 資訊</summary>
		/// <param name="xmlData">含有 Delta I/O 資訊之 XML 節點</param>
		public DeltaIO(XmlElmt xmlData) {

			XmlAttr attr = xmlData.Attribute("Device");
			if (attr == null || attr.Value != "DELTA") throw new ArgumentException("此 XML 節點並非 Delta 之 I/O 資料", "Device");

			if (xmlData.Attribute("ID", out attr)) mIoName = attr.Value;

			if (xmlData.Attribute("Type", out attr)) mType = (IOTypes)Enum.Parse(typeof(IOTypes), attr.Value, true);

			if (xmlData.Attribute("Enum", out attr)) mEnumIdx = ushort.Parse(attr.Value);

			if (xmlData.Attribute("DevNum", out attr)) mDevIdx = byte.Parse(attr.Value);

			if (xmlData.Attribute("Variable", out attr)) mVar = attr.Value;

			mComment = xmlData.Value;
		}
		#endregion

		#region Public Operations
		/// <summary>取得此 I/O 資訊的淺層複製</summary>
		/// <returns>複製的物件</returns>
		/// <remarks>I/O 非 reference type，故直接用 MemberwiseClone，如未來有問題再改用深層複製</remarks>
		public ICtIO Clone() {
			return this.MemberwiseClone() as DeltaIO;
		}

		#endregion

		#region Inheritance Implements
		int IComparable.CompareTo(object obj) {
			if (obj is DeltaIO) {
				DeltaIO tar = obj as DeltaIO;
				/* 先比較是不是都是 X、Y 或 M */
				if (mIoName[0] == tar.IoName[0]) {
					int ioA, ioB;
					if (int.TryParse(mIoName.ToLower().Replace("x", "").Replace("y", "").Replace("m", ""), out ioA)
						&& int.TryParse(tar.IoName.ToLower().Replace("x", "").Replace("y", "").Replace("m", ""), out ioB))
						return ioA.CompareTo(ioB);  //同樣的 X/Y/M 情況下，將後面的數字轉成 int 並直接比較大小
					else return string.Compare(mIoName, tar.IoName);   //如果是 X/Y/M 之外的新成員，直接以字串比較

					/* 如果 X/Y 不同，直接使用字串比較就可 */
				} else return string.Compare(mIoName, tar.IoName);
			} else throw new InvalidCastException(string.Format("無法比較。引數型態 {0} 非 DeltaIO 可比較之型態", obj.GetType()));
		}

		int IComparer<DeltaIO>.Compare(DeltaIO x, DeltaIO y) {
			/* 先比較是不是都是 X、Y 或 M */
			if (x.IoName[0] == y.IoName[0]) {
				int ioA, ioB;
				if (int.TryParse(x.IoName.ToLower().Replace("x", "").Replace("y", "").Replace("m", ""), out ioA)
					&& int.TryParse(y.IoName.ToLower().Replace("x", "").Replace("y", "").Replace("m", ""), out ioB))
					return ioA.CompareTo(ioB);  //同樣的 X/Y/M 情況下，將後面的數字轉成 int 並直接比較大小
				else return string.Compare(x.IoName, y.IoName);   //如果是 X/Y/M 之外的新成員，直接以字串比較

				/* 如果 X/Y 不同，直接使用字串比較就可 */
			} else return string.Compare(x.IoName, y.IoName);
		}

		/// <summary>建立可使用於 <see cref="CtIO"/> 之 XML 節點</summary>
		/// <param name="nodeName">此 XML 節點名稱</param>
		/// <returns>此 I/O 資訊所對應的 XML 節點</returns>
		public XmlElmt CreateXmlData(string nodeName) {
			return new XmlElmt(
				nodeName,
				mComment,
				new XmlAttr("Device", "BECKHOFF"),
				new XmlAttr("DevNum", mDevIdx.ToString()),
				new XmlAttr("Type", (mType == IOTypes.InOut ? "InOut" : (mType == IOTypes.Input ? "Input" : "Output"))),
				new XmlAttr("ID", mIoName),
				new XmlAttr("Variable", mVar),
				new XmlAttr("Enum", mEnumIdx.ToString())
			);
		}

		/// <summary>更改註解</summary>
		/// <param name="cmt">新的註解</param>
		public void SetComment(string cmt) { mComment = cmt; }

		/// <summary>更改的 Enumeration 索引值</summary>
		/// <param name="idx">欲更改的索引值。例如 enum IoList { DO_001 = 29 } 則此處帶入 29</param>
		public void SetEnumIndex(ushort idx) { mEnumIdx = idx; }

		/// <summary>更改當前 I/O 狀態</summary>
		/// <param name="stt">欲更改的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		public void SetState(bool stt) { mCurStt = stt; }
		#endregion

		#region Overrides
		/// <summary>將 DeltaIO 轉以 <see cref="string"/> 描述</summary>
		/// <returns>DeltaIO 之描述字串</returns>
		public override string ToString() {
			return string.Format("{0}, {1}, DevIdx = {2}, Enum = {3}", mIoName, mVar, mDevIdx.ToString(), mEnumIdx.ToString());
		}

		/// <summary>比較兩個 DeltaIO 是否相等。以 <see cref="IoName"/> 與 <seealso cref="Variable"/> 做比較</summary>
		/// <param name="obj">欲比較的物件</param>
		/// <returns>(<see langword="true"/>)<see cref="IoName"/> 與 <seealso cref="Variable"/> 兩者均相等 (<see langword="false"/>)兩者不同</returns>
		public override bool Equals(object obj) {
			if (obj is DeltaIO && obj != null) {
				DeltaIO tar = obj as DeltaIO;
				return mIoName == tar.IoName && mVar == tar.Variable;
			} else return false;
		}

		/// <summary>取得此物件之雜湊碼</summary>
		/// <returns>雜湊碼</returns>
		public override int GetHashCode() {
			return mIoName.GetHashCode() ^ mVar.GetHashCode() ^ mEnumIdx ^ (int)Devices.DELTA;
		}

		/// <summary>比較兩個 DeltaIO 是否相等。以 <see cref="IoName"/> 與 <seealso cref="Variable"/> 做比較</summary>
		/// <param name="x">欲比較之 DeltaIO</param>
		/// <param name="y">欲比較之 DeltaIO</param>
		/// <returns>(<see langword="true"/>)<see cref="IoName"/> 與 <seealso cref="Variable"/>兩者相等 (<see langword="false"/>)兩者不同</returns>
		public static bool operator ==(DeltaIO x, DeltaIO y) {
			if ((object)x != null && (object)y != null) {
				return x.IoName == y.IoName && x.Variable == y.Variable;
			} else return false;
		}

		/// <summary>比較兩個 DeltaIO 是否不同。以 <see cref="IoName"/> 與 <seealso cref="Variable"/> 做比較</summary>
		/// <param name="x">欲比較之 DeltaIO</param>
		/// <param name="y">欲比較之 DeltaIO</param>
		/// <returns>(<see langword="true"/>)兩者不同 (<see langword="false"/>)<see cref="IoName"/> 與 <seealso cref="Variable"/>兩者相等</returns>
		public static bool operator !=(DeltaIO x, DeltaIO y) {
			return !(x == y);
		}
		#endregion
	}

	/// <summary>Wago 專用之 I/O 資訊</summary>
	public class WagoIO : ICtIO, IComparer<WagoIO> {

		#region Private Fields
		private string mComment = string.Empty;
		private bool mCurStt = false;
		private byte mDevIdx = 0;
		private ushort mEnumIdx = 0;
		private int mIoNum = -1;
		private ushort mRegNum = 0;
		private byte mRegBit = 0;
		private IOTypes mType = IOTypes.InOut;
		#endregion

		#region Properties
		/// <summary>於離散 Coil/Input 之 I/O 編號，適用於 FC01、FC02</summary>
		public int DiscreteBit { get { return mIoNum; } }
		/// <summary>此 I/O 註解</summary>
		public string Comment { get { return mComment; } }
		/// <summary>此 I/O 來源裝置，為 Wago</summary>
		public Devices Device { get { return Devices.DELTA; } }
		/// <summary>此 I/O 所對應到的裝置索引 (適用於多台同類裝置)</summary>
		/// <remarks>例如: (1)Tube (2)Tape</remarks>
		public byte DeviceIndex { get { return mDevIdx; } }
		/// <summary>對照 I/O Enumeration 之索引值。如 HuaweiIO.DO_Y0011 = 30, 則此值即為 30</summary>
		public ushort EnumIndex { get { return mEnumIdx; } }
		/// <summary>
		/// 此 I/O 類型，如 Input、Output
		/// <para>如為欲加入 M 屬性，請設定為 <see cref="IOTypes.InOut"/></para>
		/// </summary>
		public IOTypes IoType { get { return mType; } }
		/// <summary>此 I/O 所屬的暫存器(Register)之編號，適用於 FC03、FC04、FC06、FC16</summary>
		public ushort RegisterNo { get { return mRegNum; } }
		/// <summary>所屬暫存器(Register)中的位元索引</summary>
		public byte RegisterBit { get { return mRegBit; } }
		/// <summary>此 I/O 當前狀態</summary>
		public bool State { get { return mCurStt; } }

		#endregion

		#region Constructors
		/// <summary>建構 Wago 專用之 I/O 資訊</summary>
		/// <param name="discreteBit">於離散 Coil/Input 之 I/O 編號，適用於 FC01、FC02</param>
		/// <param name="regNo">此 I/O 所屬的暫存器(Register)之編號，適用於 FC03、FC04、FC06、FC16</param>
		/// <param name="regBit">所屬暫存器(Register)中的位元索引</param>
		/// <param name="type">此 I/O 類型，如 Input、Output</param>
		/// <param name="comment">此 I/O 註解</param>
		/// <param name="enumIdx">對照 I/O Enumeration 之索引值。如 HuaweiIO.DO_Y0011 = 30, 則此值即為 30</param>
		/// <param name="devIdx">此 I/O 所對應到的裝置索引 (適用於多台同類裝置)</param>
		public WagoIO(int discreteBit, ushort regNo, byte regBit, IOTypes type, string comment = "", ushort enumIdx = 0, byte devIdx = 0) {
			mIoNum = discreteBit;
			mRegNum = regNo;
			mRegBit = regBit;
			mType = type;
			mComment = comment;
			mEnumIdx = enumIdx;
			mDevIdx = devIdx;
		}

		/// <summary>使用 <see cref="XmlElmt"/> 建構 Wago 專用之 I/O 資訊</summary>
		/// <param name="xmlData">含有 Wago I/O 資訊之 XML 節點</param>
		public WagoIO(XmlElmt xmlData) {

			XmlAttr attr = xmlData.Attribute("Device");
			if (attr == null || attr.Value != "WAGO") throw new ArgumentException("此 XML 節點並非 Wago 之 I/O 資料", "Device");

			if (xmlData.Attribute("Type", out attr)) mType = (IOTypes)Enum.Parse(typeof(IOTypes), attr.Value, true);

			if (xmlData.Attribute("Enum", out attr)) mEnumIdx = ushort.Parse(attr.Value);

			if (xmlData.Attribute("DevNum", out attr)) mDevIdx = byte.Parse(attr.Value);

			if (xmlData.Attribute("DsctBit", out attr)) mIoNum = int.Parse(attr.Value);

			if (xmlData.Attribute("RegNo", out attr)) mRegNum = ushort.Parse(attr.Value);

			if (xmlData.Attribute("RegBit", out attr)) mRegBit = byte.Parse(attr.Value);

			mComment = xmlData.Value;
		}
		#endregion

		#region Public Operations
		/// <summary>取得此 I/O 資訊的淺層複製</summary>
		/// <returns>複製的物件</returns>
		/// <remarks>I/O 非 reference type，故直接用 MemberwiseClone，如未來有問題再改用深層複製</remarks>
		public ICtIO Clone() {
			return this.MemberwiseClone() as WagoIO;
		}

		/// <summary>更改離散 Coil/Input 編號</summary>
		/// <param name="bit">欲更改的編號</param>
		public void SetDiscreteBit(int bit) { mIoNum = bit; }

		/// <summary>更改所屬暫存器編號</summary>
		/// <param name="num">暫存器號碼</param>
		public void SetRegisterNo(ushort num) { mRegNum = num; }

		/// <summary>更改於所屬暫存器內之索引</summary>
		/// <param name="bit">索引值</param>
		public void SetRegisterBit(byte bit) { mRegBit = bit; }

		#endregion

		#region Inheritance Implements
		int IComparable.CompareTo(object obj) {
			if (obj is WagoIO) {
				WagoIO tar = obj as WagoIO;
				int compare = mIoNum.CompareTo(tar.DiscreteBit);
				if (compare == 0) {
					compare = mRegNum.CompareTo(tar.RegisterNo);
					if (compare == 0) compare = mRegBit.CompareTo(tar.RegisterBit);
				}
				return compare;
			} else throw new InvalidCastException(string.Format("無法比較。引數型態 {0} 非 WagoIO 可比較之型態", obj.GetType()));
		}

		int IComparer<WagoIO>.Compare(WagoIO x, WagoIO y) {
			int compare = x.DiscreteBit.CompareTo(y.DiscreteBit);
			if (compare == 0) {
				compare = x.RegisterNo.CompareTo(y.RegisterNo);
				if (compare == 0) compare = x.RegisterBit.CompareTo(y.RegisterBit);
			}
			return compare;
		}

		/// <summary>建立可使用於 <see cref="CtIO"/> 之 XML 節點</summary>
		/// <param name="nodeName">此 XML 節點名稱</param>
		/// <returns>此 I/O 資訊所對應的 XML 節點</returns>
		public XmlElmt CreateXmlData(string nodeName) {
			return new XmlElmt(
				nodeName,
				mComment,
				new XmlAttr("Device", "BECKHOFF"),
				new XmlAttr("DevNum", mDevIdx.ToString()),
				new XmlAttr("Type", (mType == IOTypes.InOut ? "InOut" : (mType == IOTypes.Input ? "Input" : "Output"))),
				new XmlAttr("DsctBit", mIoNum.ToString()),
				new XmlAttr("RegNo", mRegNum.ToString()),
				new XmlAttr("RegBit", mRegBit.ToString()),
				new XmlAttr("Enum", mEnumIdx.ToString())
			);
		}

		/// <summary>更改註解</summary>
		/// <param name="cmt">新的註解</param>
		public void SetComment(string cmt) { mComment = cmt; }

		/// <summary>更改的 Enumeration 索引值</summary>
		/// <param name="idx">欲更改的索引值。例如 enum IoList { DO_001 = 29 } 則此處帶入 29</param>
		public void SetEnumIndex(ushort idx) { mEnumIdx = idx; }

		/// <summary>更改當前 I/O 狀態</summary>
		/// <param name="stt">欲更改的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		public void SetState(bool stt) { mCurStt = stt; }
		#endregion

		#region Overrides
		/// <summary>將 WagoIO 轉以 <see cref="string"/> 描述</summary>
		/// <returns>WagoIO 之描述字串</returns>
		public override string ToString() {
			return string.Format("DsctBit = {0}, Reg = ({1}, {2}), DevIdx = {3}, Enum = {4}", mIoNum.ToString(), mRegNum.ToString(), mRegBit.ToString(), mDevIdx.ToString(), mEnumIdx.ToString());
		}

		/// <summary>比較兩個 WagoIO 是否相等。以 <see cref="DiscreteBit"/>、<seealso cref="RegisterNo"/> 與 <seealso cref="RegisterBit"/> 做比較</summary>
		/// <param name="obj">欲比較的物件</param>
		/// <returns>(<see langword="true"/>)<see cref="DiscreteBit"/>、<seealso cref="RegisterNo"/> 與 <seealso cref="RegisterBit"/> 兩者均相等 (<see langword="false"/>)兩者不同</returns>
		public override bool Equals(object obj) {
			if (obj is WagoIO && obj != null) {
				WagoIO tar = obj as WagoIO;
				return mIoNum == tar.DiscreteBit && mRegNum == tar.RegisterNo && mRegBit == tar.RegisterBit;
			} else return false;
		}

		/// <summary>取得此物件之雜湊碼</summary>
		/// <returns>雜湊碼</returns>
		public override int GetHashCode() {
			return mIoNum ^ mRegNum ^ mRegBit ^ mEnumIdx ^ (int)Devices.WAGO;
		}

		/// <summary>比較兩個 WagoIO 是否相等。以 <see cref="DiscreteBit"/>、<seealso cref="RegisterNo"/> 與 <seealso cref="RegisterBit"/> 做比較</summary>
		/// <param name="x">欲比較之 WagoIO</param>
		/// <param name="y">欲比較之 WagoIO</param>
		/// <returns>(<see langword="true"/>)<see cref="DiscreteBit"/>、<seealso cref="RegisterNo"/> 與 <seealso cref="RegisterBit"/> 兩者相等 (<see langword="false"/>)兩者不同</returns>
		public static bool operator ==(WagoIO x, WagoIO y) {
			if ((object)x != null && (object)y != null) {
				return x.DiscreteBit == y.DiscreteBit && x.RegisterNo == y.RegisterNo && x.RegisterBit == y.RegisterBit;
			} else return false;
		}

		/// <summary>比較兩個 WagoIO 是否不同。以 <see cref="DiscreteBit"/>、<seealso cref="RegisterNo"/> 與 <seealso cref="RegisterBit"/> 做比較</summary>
		/// <param name="x">欲比較之 WagoIO</param>
		/// <param name="y">欲比較之 WagoIO</param>
		/// <returns>(<see langword="true"/>)兩者不同 (<see langword="false"/>)<see cref="DiscreteBit"/>、<seealso cref="RegisterNo"/> 與 <seealso cref="RegisterBit"/>兩者相等</returns>
		public static bool operator !=(WagoIO x, WagoIO y) {
			return !(x == y);
		}
		#endregion
	}
}
