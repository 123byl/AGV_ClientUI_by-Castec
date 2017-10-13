using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

//using CtLib.Module.Beckhoff;
//using CtLib.Module.Adept;
//using CtLib.Module.Delta;
using CtLib.Module.XML;

namespace CtLib.Library {

	/*----- Version --------------------------

	1.0.0	Ahern	[2015/02/04]
		+ 從 CtNexcom_Recipe 獨立至此

	1.1.0	Ahern	[2016/03/29]
		\ 使用 ICtRecipe 拆開以擴充相關功能

	----------------------------------------*/

	/// <summary>儲存 Recipe 相關資訊</summary>
	public class RecipeInfo {

		#region Fields
		private DateTime mTimeBuilt = DateTime.Now;
		private DateTime mTimeLoad = DateTime.Now;
		private string mName = string.Empty;
		private string mCmt = string.Empty;
		private string mFilePath = string.Empty;
		#endregion

		#region Properties
		/// <summary>Recipe 存檔時間</summary>
		public DateTime BuildTime { get { return mTimeBuilt; } }
		/// <summary>寫入設備之時間</summary>
		public DateTime LoadTime { get { return mTimeLoad; } }
		/// <summary>此 Recipe 名稱</summary>
		public string Name { get { return mName; } }
		/// <summary>此 Recipe 註解</summary>
		public string Comment { get { return mCmt; } }
		/// <summary>此 Recipe 完整檔案名稱路徑</summary>
		public string Path { get { return mFilePath; } }
		#endregion

		#region Constructors
		/// <summary>建構 Recipe 資訊，並自動抓取檔案建構時間</summary>
		/// <param name="name">Recipe 名稱</param>
		/// <param name="comment">Recipe 註解</param>
		public RecipeInfo(string name, string comment) {
			mName = name;
			mCmt = comment;
			mFilePath = string.Format("{0}{1}.xml", CtDefaultPath.GetPath(SystemPath.Recipe), name);
			if (CtFile.IsFileExist(mFilePath)) mTimeBuilt = CtFile.GetFileInformation(mFilePath).CreateTime;
		}

		/// <summary>建構 Recipe 資訊，並自動抓取檔案建構時間</summary>
		/// <param name="name">Recipe 名稱</param>
		/// <param name="comment">Recipe 註解</param>
		/// <param name="path">Recipe 檔案路徑</param>
		public RecipeInfo(string name, string comment, string path) {
			mName = name;
			mCmt = comment;
			mFilePath = path;
			if (CtFile.IsFileExist(mFilePath)) mTimeBuilt = CtFile.GetFileInformation(mFilePath).CreateTime;
		}
		#endregion

		#region Public Operations
		/// <summary>更改 Recipe 載入時間</summary>
		/// <param name="time">指定的載入時間</param>
		public void SetLoadTime(DateTime time) {
			mTimeLoad = time;
		}

		/// <summary>更改載入 Recipe 時間為當下</summary>
		public void SetLoadTime() {
			mTimeLoad = DateTime.Now;
		}
		#endregion

		#region Overrides
		/// <summary>取得此 Recipe 之描述字串</summary>
		/// <returns>Recipe 描述字串</returns>
		public override string ToString() {
			return string.Format("{0},{1}", mName, mTimeBuilt.ToString("yyyy/MM/dd"));
		}
		#endregion
	}

	/// <summary>
	/// Recipe 資訊介面
	/// <para>如裝置、名稱、註解、數值等</para>
	/// </summary>
	public interface ICtRecipe : IComparable {

		#region Properties
		/// <summary>對應裝置設備</summary>
		Devices Device { get; }
		/// <summary>變數或特定名稱</summary>
		string Name { get; }
		/// <summary>此資訊之註解</summary>
		string Comment { get; }
		/// <summary>對應相同裝置時之裝置索引 (適用於多台相同裝置)</summary>
		/// <remarks>如同時有兩台 Beckhoff 等，一台為 1 另台為 2，即可用於區分此 Recipe 隸屬何台裝置</remarks>
		int DeviceIndex { get; }
		/// <summary>儲存當前數值</summary>
		object Value { get; }
		/// <summary>於介面應用時代表此 Recipe 之顏色</summary>
		/// <remarks>如於 DataGridView 顯示此行之背景顏色</remarks>
		Color BackgroundColor { get; }
		#endregion

		#region Methods
		/// <summary>更改對應此 Recipe 之註解</summary>
		/// <param name="comment">欲套用的新註解</param>
		void SetComment(string comment);

		/// <summary>更改當前 Recipe 所儲存的數值</summary>
		/// <param name="value">欲套用的新數值</param>
		void SetValue(string value);

		/// <summary>取得此 Recipe 所對應的 XML 節點資料</summary>
		/// <param name="nodeName">欲建立的節點名稱</param>
		/// <returns>節點資料</returns>
		XmlElmt CreateXmlData(string nodeName);

		/// <summary>取得數值所相對應的字串</summary>
		/// <returns>數值相對應字串</returns>
		string EncodeValue();

		#endregion
	}

	///// <summary>適用於 Adept ACE V+ 變數之 Recipe 資訊</summary>
	//public class AceVPlusRecipe : ICtRecipe, IComparer<AceVPlusRecipe> {

	//	#region Fields
	//	private string mVar = string.Empty;
	//	private string mCmt = string.Empty;
	//	private int mDevIdx = 0;
	//	private object mVal = null;
	//	private VPlusVariableType mVarType = VPlusVariableType.Real;
	//	private Color mColor = SystemColors.Window;
	//	#endregion

	//	#region Properties
	//	/// <summary>於介面應用時代表此 Recipe 之顏色</summary>
	//	public Color BackgroundColor { get { return mColor; } }
	//	/// <summary>此資訊之註解</summary>
	//	public string Comment { get { return mCmt; } }
	//	/// <summary>對應裝置設備，為 Adept ACE</summary>
	//	public Devices Device { get { return Devices.AdeptACE; } }
	//	/// <summary>
	//	/// 對應相同裝置時之裝置索引 (適用於多台相同裝置)
	//	/// <para>例如同時有兩台 Controller/Robot 等，EX 為 0、Robot1 為 1、Robot2 為 2，可用於區分此 Recipe 隸屬何台裝置</para>
	//	/// </summary>
	//	public int DeviceIndex { get { return mDevIdx; } }
	//	/// <summary>對應 V+ 之變數名稱</summary>
	//	public string Name { get { return mVar; } }
	//	/// <summary>當前數值</summary>
	//	public object Value { get { return mVal; } }
	//	/// <summary>對應的 V+ 變數類型</summary>
	//	public VPlusVariableType VaribleType { get { return mVarType; } }
	//	#endregion

	//	#region Constructors
	//	/// <summary>建構 Adept ACE V+ 變數專用的 Recipe 資訊</summary>
	//	/// <param name="varName">對應 V+ 之變數名稱</param>
	//	/// <param name="varType">對應的 V+ 變數類型</param>
	//	/// <param name="comment">此資訊之註解</param>
	//	/// <param name="defVal">預設數值</param>
	//	/// <param name="devIdx">對應相同裝置時之裝置索引 (適用於多台相同裝置)</param>
	//	public AceVPlusRecipe(string varName, VPlusVariableType varType, string comment, object defVal = null, int devIdx = 0) {
	//		mVar = varName;
	//		mCmt = comment;
	//		mVarType = varType;
	//		mVal = defVal;
	//		mDevIdx = devIdx;
	//		mColor = SystemColors.Window;
	//	}

	//	/// <summary>建構 Adept ACE V+ 變數專用的 Recipe 資訊</summary>
	//	/// <param name="varName">對應 V+ 之變數名稱</param>
	//	/// <param name="varType">對應的 V+ 變數類型</param>
	//	/// <param name="comment">此資訊之註解</param>
	//	/// <param name="defVal">預設數值</param>
	//	/// <param name="devIdx">對應相同裝置時之裝置索引 (適用於多台相同裝置)</param>
	//	/// <param name="color">於介面應用時代表此 Recipe 之顏色</param>
	//	public AceVPlusRecipe(string varName, VPlusVariableType varType, string comment, Color color, object defVal = null, int devIdx = 0) {
	//		mVar = varName;
	//		mCmt = comment;
	//		mVarType = varType;
	//		mVal = defVal;
	//		mDevIdx = devIdx;
	//		mColor = color;
	//	}

	//	/// <summary>使用 <see cref="XmlElmt"/> 建構 Adept ACE V+ 專用之 Recipe 資訊</summary>
	//	/// <param name="xmlData">具有資訊節點的 <see cref="XmlElmt"/></param>
	//	public AceVPlusRecipe(XmlElmt xmlData) {

	//		XmlAttr attr = xmlData.Attribute("Device");
	//		if (attr != null) {
	//			if (attr.Value != "ACE") throw new InvalidCastException("此節點並非 Ace V+ 之參數");
	//		} else throw new ArgumentNullException("Device", "Device 節點為空，無法判斷類型");

	//		if (xmlData.Attribute("Name", out attr)) mVar = attr.Value;

	//		if (xmlData.Attribute("Comment", out attr)) mCmt = attr.Value;

	//		if (!xmlData.Attribute("DeviceIndex", out attr)) xmlData.Attribute("ListIndex", out attr);
	//		if (attr != null) mDevIdx = int.Parse(attr.Value);

	//		if (xmlData.Attribute("Type", out attr)) mVarType = (VPlusVariableType)(int.Parse(attr.Value));

	//		if (!xmlData.Attribute("Color", out attr)) xmlData.Attribute("LineColor", out attr);
	//		if (attr != null) {
	//			if (attr.Value.StartsWith("#")) {
	//				int argb = CtConvert.ToInteger(attr.Value.Replace("#", ""), NumericFormats.Hexadecimal);
	//				mColor = Color.FromArgb(argb);
	//			} else mColor = Color.FromName(attr.Value);
	//			if (mColor.ToArgb() == 0) mColor = SystemColors.Window;
	//		}

	//		DecodeValue(xmlData.Value);
	//	}

	//	#endregion

	//	#region Private Operations

	//	private void DecodeValue(string data) {
	//		switch (mVarType) {
	//			case VPlusVariableType.Real:
	//				mVal = float.Parse(data);
	//				break;
	//			case VPlusVariableType.Location:
	//			case VPlusVariableType.PrecisionPoint:
	//				IEnumerable<double> val = data.Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries).Select(str => double.Parse(str.Trim()));
	//				mVal = val.ToList();
	//				break;
	//			case VPlusVariableType.String:
	//			default:
	//				mVal = data;
	//				break;
	//		}
	//	}

	//	#endregion

	//	#region Public Operations

	//	/// <summary>將當前數值寫入設備</summary>
	//	/// <param name="ace">欲寫入的 Adept ACE 來源</param>
	//	public void WriteValue(CtAce ace) {
	//		if (ace != null) {
	//			if (mVal != null) {
	//				switch (mVarType) {
	//					case VPlusVariableType.Real:
	//						ace.Variable.SetValue(mVar, (float)mVal);
	//						break;
	//					case VPlusVariableType.String:
	//						ace.Variable.SetValue(mVar, mVal.ToString());
	//						break;
	//					case VPlusVariableType.Location:
	//					case VPlusVariableType.PrecisionPoint:
	//						ace.Variable.SetValue(mVar, mVal as List<double>, mVarType);
	//						break;
	//					default:
	//						throw new ArgumentOutOfRangeException("VarType", "無效的 VPlusVariableType");
	//				}
	//			} else throw new ArgumentNullException("Value", "數值為空，請先設定數值");
	//		}
	//	}

	//	/// <summary>讀取當前的數值</summary>
	//	/// <param name="ace">欲讀取的 Adept ACE</param>
	//	public void ReadValue(CtAce ace) {
	//		if (ace != null) {
	//			switch (mVarType) {
	//				case VPlusVariableType.Real:
	//					float sngTemp;
	//					ace.Variable.GetValue(mVar, out sngTemp);
	//					mVal = sngTemp;
	//					break;
	//				case VPlusVariableType.String:
	//					string strTemp;
	//					ace.Variable.GetValue(mVar, out strTemp);
	//					mVal = strTemp;
	//					break;
	//				case VPlusVariableType.Location:
	//				case VPlusVariableType.PrecisionPoint:
	//					List<double> dblTemp;
	//					ace.Variable.GetValue(mVar, out dblTemp, mVarType);
	//					mVal = dblTemp;
	//					break;
	//				default:
	//					throw new ArgumentOutOfRangeException("VarType", "無效的 VPlusVariableType");
	//			}
	//		}
	//	}
	//	#endregion

	//	#region Inheritance Implements
	//	int IComparable.CompareTo(object obj) {
	//		if (obj is AceVPlusRecipe) return mVar.CompareTo((obj as AceVPlusRecipe).Name);    //如果 obj 是 null 就直接讓他自己跳 Exception 吧
	//		else throw new InvalidCastException(string.Format("無法比較。引數型態 {0} 非 AceVPlusRecipe 可比較之型態", obj.GetType()));
	//	}

	//	int IComparer<AceVPlusRecipe>.Compare(AceVPlusRecipe x, AceVPlusRecipe y) {
	//		return x.Name.CompareTo(y.Name);
	//	}

	//	/// <summary>取得此 Recipe 所對應的 XML 節點資料</summary>
	//	/// <param name="nodeName">欲建立的節點名稱</param>
	//	/// <returns>節點資料</returns>
	//	public XmlElmt CreateXmlData(string nodeName) {
	//		return new XmlElmt(
	//			nodeName,
	//			EncodeValue(),
	//			new XmlAttr("Name", mVar),
	//			new XmlAttr("Comment", mCmt),
	//			new XmlAttr("Device", "ACE"),
	//			new XmlAttr("DeviceIndex", mDevIdx.ToString()),
	//			new XmlAttr("Type", ((int)mVarType).ToString()),
	//			new XmlAttr("Color", mColor.IsNamedColor ? mColor.Name : "#" + CtConvert.ToHex(mColor.ToArgb()))
	//		);
	//	}

	//	/// <summary>取得數值所相對應的字串</summary>
	//	/// <returns>數值相對應字串</returns>
	//	public string EncodeValue() {
	//		string val = string.Empty;
	//		if (mVal != null) {
	//			switch (mVarType) {
	//				case VPlusVariableType.Real:
	//					val = ((float)mVal).ToString("F3");
	//					break;
	//				case VPlusVariableType.String:
	//				default:
	//					val = mVal.ToString();
	//					break;
	//				case VPlusVariableType.Location:
	//				case VPlusVariableType.PrecisionPoint:
	//					val = string.Join(",", (mVal as List<double>).ConvertAll(dbl => dbl.ToString("F3")));
	//					break;
	//			}
	//		}
	//		return val;
	//	}

	//	/// <summary>更改對應此 Recipe 之註解</summary>
	//	/// <param name="cmt">欲套用的新註解</param>
	//	public void SetComment(string cmt) { mCmt = cmt; }

	//	/// <summary>更改當前 Recipe 所儲存的數值</summary>
	//	/// <param name="val">欲套用的新數值</param>
	//	public void SetValue(string val) { DecodeValue(val); }
	//	#endregion

	//	#region Overrides
	//	/// <summary>將 AceVPlusRecipe 轉以 <see cref="string"/> 描述</summary>
	//	/// <returns>AceVPlusRecipe 之描述字串</returns>
	//	public override string ToString() {
	//		return string.Format("AceV+, {0}, {1}, {2}", mVar, mVarType.ToString(), (mVal == null ? string.Empty : mVal.ToString()));
	//	}

	//	/// <summary>比較兩個 AceVPlusRecipe 是否相等。以 <see cref="VaribleType"/>, <seealso cref="Name"/> 與 <seealso cref="Value"/> 為準</summary>
	//	/// <param name="obj">欲比較的物件</param>
	//	/// <returns>(<see langword="true"/>)兩者相等 (<see langword="false"/>)兩者不同</returns>
	//	public override bool Equals(object obj) {
	//		if (obj != null && obj is AceVPlusRecipe) {
	//			if (ReferenceEquals(this, obj)) return true;
	//			AceVPlusRecipe aceVp = obj as AceVPlusRecipe;
	//			return (mVarType == aceVp.VaribleType) && (mVar == aceVp.Name) && (mVal == aceVp.Value);
	//		} else return false;
	//	}

	//	/// <summary>取得此物件之雜湊碼</summary>
	//	/// <returns>雜湊碼</returns>
	//	public override int GetHashCode() {
	//		return (int)mVarType ^ mVar.GetHashCode() ^ mVal.GetHashCode();
	//	}

	//	/// <summary>比較兩個 AceVPlusRecipe 是否相等。以 <see cref="VaribleType"/>, <seealso cref="Name"/> 與 <seealso cref="Value"/> 做比較</summary>
	//	/// <param name="x">欲比較之物件</param>
	//	/// <param name="y">欲被比較之物件</param>
	//	/// <returns>(<see langword="true"/>)兩者相等 (<see langword="false"/>)兩者不同</returns>
	//	public static bool operator ==(AceVPlusRecipe x, AceVPlusRecipe y) {
	//		if ((object)x != null && (object)y != null) {
	//			if (ReferenceEquals(x, y)) return true;
	//			return (x.VaribleType == y.VaribleType) && (x.Name == y.Name) && (x.Value == y.Value);
	//		} else return false;
	//	}

	//	/// <summary>比較兩個 AceVPlusRecipe 是否不同。以 <see cref="VaribleType"/>, <seealso cref="Name"/> 與 <seealso cref="Value"/> 做比較</summary>
	//	/// <param name="x">欲比較之物件</param>
	//	/// <param name="y">欲被比較之物件</param>
	//	/// <returns>(<see langword="true"/>)兩者不同 (<see langword="false"/>)兩者相等</returns>
	//	public static bool operator !=(AceVPlusRecipe x, AceVPlusRecipe y) {
	//		return !(x == y);
	//	}
	//	#endregion
	//}

	///// <summary>適用於 Adept ACE 外部數值之 Recipe 資訊</summary>
	//public class AceNumRecipe : ICtRecipe, IComparer<AceNumRecipe> {

	//	#region Fields
	//	private string mVar = string.Empty;
	//	private string mCmt = string.Empty;
	//	private int mDevIdx = 0;
	//	private object mVal = null;
	//	private VariableType mVarType = VariableType.Numeric;
	//	private Color mColor = SystemColors.Window;
	//	#endregion

	//	#region Properties
	//	/// <summary>於介面應用時代表此 Recipe 之顏色</summary>
	//	public Color BackgroundColor { get { return mColor; } }
	//	/// <summary>此資訊之註解</summary>
	//	public string Comment { get { return mCmt; } }
	//	/// <summary>對應裝置設備，為 Adept ACE</summary>
	//	public Devices Device { get { return Devices.AdeptACE; } }
	//	/// <summary>
	//	/// 對應相同裝置時之裝置索引 (適用於多台相同裝置)
	//	/// <para>例如同時有兩台 Controller/Robot 等，EX 為 0、Robot1 為 1、Robot2 為 2，可用於區分此 Recipe 隸屬何台裝置</para>
	//	/// </summary>
	//	public int DeviceIndex { get { return mDevIdx; } }
	//	/// <summary>對應 外部數值 之變數名稱</summary>
	//	public string Name { get { return mVar; } }
	//	/// <summary>當前數值</summary>
	//	public object Value { get { return mVal; } }
	//	/// <summary>對應的外部數值類型</summary>
	//	public VariableType VaribleType { get { return mVarType; } }
	//	#endregion

	//	#region Constructors
	//	/// <summary>建構 Adept ACE 外部數值 變數專用的 Recipe 資訊</summary>
	//	/// <param name="varName">對應外部數值之名稱</param>
	//	/// <param name="varType">對應的外部數值類型</param>
	//	/// <param name="comment">此資訊之註解</param>
	//	/// <param name="defVal">預設數值</param>
	//	/// <param name="devIdx">對應相同裝置時之裝置索引 (適用於多台相同裝置)</param>
	//	public AceNumRecipe(string varName, VariableType varType, string comment, object defVal = null, int devIdx = 0) {
	//		mVar = varName;
	//		mCmt = comment;
	//		mVarType = varType;
	//		mVal = defVal;
	//		mDevIdx = devIdx;
	//	}

	//	/// <summary>建構 Adept ACE 外部數值 變數專用的 Recipe 資訊</summary>
	//	/// <param name="varName">對應外部數值之名稱</param>
	//	/// <param name="varType">對應的外部數值類型</param>
	//	/// <param name="comment">此資訊之註解</param>
	//	/// <param name="defVal">預設數值</param>
	//	/// <param name="devIdx">對應相同裝置時之裝置索引 (適用於多台相同裝置)</param>
	//	/// <param name="color">於介面應用時代表此 Recipe 之顏色</param>
	//	public AceNumRecipe(string varName, VariableType varType, string comment, Color color, object defVal = null, int devIdx = 0) {
	//		mVar = varName;
	//		mCmt = comment;
	//		mVarType = varType;
	//		mVal = defVal;
	//		mDevIdx = devIdx;
	//		mColor = color;
	//	}

	//	/// <summary>使用 <see cref="XmlElmt"/> 建構 Adept ACE 外部數值 專用之 Recipe 資訊</summary>
	//	/// <param name="xmlData">具有資訊節點的 <see cref="XmlElmt"/></param>
	//	public AceNumRecipe(XmlElmt xmlData) {

	//		XmlAttr attr = xmlData.Attribute("Device");
	//		if (attr != null) {
	//			if (attr.Value != "ACE_NUM") throw new InvalidCastException("此節點並非 Ace 外部數值 之參數");
	//		} else throw new ArgumentNullException("Device", "Device 節點為空，無法判斷類型");

	//		if (xmlData.Attribute("Name", out attr)) mVar = attr.Value;

	//		if (xmlData.Attribute("Comment", out attr)) mCmt = attr.Value;

	//		if (!xmlData.Attribute("DeviceIndex", out attr)) xmlData.Attribute("ListIndex", out attr);
	//		if (attr != null) mDevIdx = int.Parse(attr.Value);

	//		if (xmlData.Attribute("Type", out attr)) mVarType = (VariableType)(int.Parse(attr.Value));
	//		if (mVarType == VariableType.VPlus) throw new ArgumentOutOfRangeException("Type", "無法套用 V+ 節點至外部數值資訊");

	//		if (!xmlData.Attribute("Color", out attr)) xmlData.Attribute("LineColor", out attr);
	//		if (attr != null) {
	//			if (attr.Value.StartsWith("#")) {
	//				int argb = CtConvert.ToInteger(attr.Value.Replace("#", ""), NumericFormats.Hexadecimal);
	//				mColor = Color.FromArgb(argb);
	//			} else mColor = Color.FromName(attr.Value);
	//			if (mColor.ToArgb() == 0) mColor = SystemColors.Window;
	//		}

	//		DecodeValue(xmlData.Value);
	//	}

	//	#endregion

	//	#region Private Operations

	//	private void DecodeValue(string data) {
	//		switch (mVarType) {
	//			case VariableType.Numeric:
	//				mVal = double.Parse(data);
	//				break;
	//			case VariableType.String:
	//			default:
	//				mVal = data.ToString();
	//				break;
	//		}
	//	}

	//	#endregion

	//	#region Public Operations

	//	/// <summary>將當前數值寫入設備</summary>
	//	/// <param name="ace">欲寫入的 Adept ACE 來源</param>
	//	public void WriteValue(CtAce ace) {
	//		if (ace != null) {
	//			if (mVal != null) {
	//				switch (mVarType) {
	//					case VariableType.Numeric:
	//						ace.Variable.SetNumericValue(mVar, (float)mVal);
	//						break;
	//					case VariableType.String:
	//						ace.Variable.SetStringValue(mVar, mVal.ToString());
	//						break;
	//					default:
	//						throw new ArgumentOutOfRangeException("VarType", "無效的 VariableType");
	//				}
	//			} else throw new ArgumentNullException("Value", "數值為空，請先設定數值");
	//		}
	//	}

	//	/// <summary>讀取當前的數值</summary>
	//	/// <param name="ace">欲讀取的 Adept ACE</param>
	//	public void ReadValue(CtAce ace) {
	//		if (ace != null) {
	//			switch (mVarType) {
	//				case VariableType.Numeric:
	//					double sngTemp;
	//					ace.Variable.GetNumericValue(mVar, out sngTemp);
	//					break;
	//				case VariableType.String:
	//					string strTemp;
	//					ace.Variable.GetStringValue(mVar, out strTemp);
	//					break;
	//				default:
	//					throw new ArgumentOutOfRangeException("VarType", "無效的 VariableType");
	//			}
	//		}
	//	}
	//	#endregion

	//	#region Inheritance Implements
	//	int IComparable.CompareTo(object obj) {
	//		if (obj is AceNumRecipe) return mVar.CompareTo((obj as AceNumRecipe).Name);    //如果 obj 是 null 就直接讓他自己跳 Exception 吧
	//		else throw new InvalidCastException(string.Format("無法比較。引數型態 {0} 非 AceNumRecipe 可比較之型態", obj.GetType()));
	//	}

	//	int IComparer<AceNumRecipe>.Compare(AceNumRecipe x, AceNumRecipe y) {
	//		return x.Name.CompareTo(y.Name);
	//	}

	//	/// <summary>取得此 Recipe 所對應的 XML 節點資料</summary>
	//	/// <param name="nodeName">欲建立的節點名稱</param>
	//	/// <returns>節點資料</returns>
	//	public XmlElmt CreateXmlData(string nodeName) {
	//		return new XmlElmt(
	//			nodeName,
	//			EncodeValue(),
	//			new XmlAttr("Name", mVar),
	//			new XmlAttr("Comment", mCmt),
	//			new XmlAttr("Device", "ACE_NUM"),
	//			new XmlAttr("DeviceIndex", mDevIdx.ToString()),
	//			new XmlAttr("Type", ((int)mVarType).ToString()),
	//			new XmlAttr("Color", mColor.IsNamedColor ? mColor.Name : "#" + CtConvert.ToHex(mColor.ToArgb()))
	//		);
	//	}

	//	/// <summary>取得數值所相對應的字串</summary>
	//	/// <returns>數值相對應字串</returns>
	//	public string EncodeValue() {
	//		string val = string.Empty;
	//		if (mVal != null) {
	//			switch (mVarType) {
	//				case VariableType.Numeric:
	//					val = ((double)mVal).ToString("F3");
	//					break;
	//				case VariableType.String:
	//				default:
	//					val = mVal.ToString();
	//					break;
	//			}
	//		}
	//		return val;
	//	}

	//	/// <summary>更改對應此 Recipe 之註解</summary>
	//	/// <param name="cmt">欲套用的新註解</param>
	//	public void SetComment(string cmt) { mCmt = cmt; }

	//	/// <summary>更改當前 Recipe 所儲存的數值</summary>
	//	/// <param name="val">欲套用的新數值</param>
	//	public void SetValue(string val) { DecodeValue(val); }
	//	#endregion

	//	#region Overrides
	//	/// <summary>將 AceNumRecipe 轉以 <see cref="string"/> 描述</summary>
	//	/// <returns>AceNumRecipe 之描述字串</returns>
	//	public override string ToString() {
	//		return string.Format("AceNum, {0}, {1}, {2}", mVar, mVarType.ToString(), (mVal == null ? string.Empty : mVal.ToString()));
	//	}

	//	/// <summary>比較兩個 AceNumRecipe 是否相等。以 <see cref="VaribleType"/>, <seealso cref="Name"/> 與 <seealso cref="Value"/> 為準</summary>
	//	/// <param name="obj">欲比較的物件</param>
	//	/// <returns>(<see langword="true"/>)兩者相等 (<see langword="false"/>)兩者不同</returns>
	//	public override bool Equals(object obj) {
	//		if (obj != null && obj is AceNumRecipe) {
	//			if (ReferenceEquals(this, obj)) return true;
	//			AceNumRecipe aceVp = obj as AceNumRecipe;
	//			return (mVarType == aceVp.VaribleType) && (mVar == aceVp.Name) && (mVal == aceVp.Value);
	//		} else return false;
	//	}

	//	/// <summary>取得此物件之雜湊碼</summary>
	//	/// <returns>雜湊碼</returns>
	//	public override int GetHashCode() {
	//		return (int)mVarType ^ mVar.GetHashCode() ^ mVal.GetHashCode();
	//	}

	//	/// <summary>比較兩個 AceNumRecipe 是否相等。以 <see cref="VaribleType"/>, <seealso cref="Name"/> 與 <seealso cref="Value"/> 做比較</summary>
	//	/// <param name="x">欲比較之物件</param>
	//	/// <param name="y">欲被比較之物件</param>
	//	/// <returns>(<see langword="true"/>)兩者相等 (<see langword="false"/>)兩者不同</returns>
	//	public static bool operator ==(AceNumRecipe x, AceNumRecipe y) {
	//		if ((object)x != null && (object)y != null) {
	//			if (ReferenceEquals(x, y)) return true;
	//			return (x.VaribleType == y.VaribleType) && (x.Name == y.Name) && (x.Value == y.Value);
	//		} else return false;
	//	}

	//	/// <summary>比較兩個 AceNumRecipe 是否不同。以 <see cref="VaribleType"/>, <seealso cref="Name"/> 與 <seealso cref="Value"/> 做比較</summary>
	//	/// <param name="x">欲比較之物件</param>
	//	/// <param name="y">欲被比較之物件</param>
	//	/// <returns>(<see langword="true"/>)兩者不同 (<see langword="false"/>)兩者相等</returns>
	//	public static bool operator !=(AceNumRecipe x, AceNumRecipe y) {
	//		return !(x == y);
	//	}
	//	#endregion
	//}

	///// <summary>適用於 Adept ACE Vision 之 Recipe 資訊</summary>
	//public class AceVisionRecipe : ICtRecipe, IComparer<AceVisionRecipe> {

	//	#region Fields
	//	private string mPath = string.Empty;
	//	private string mCmt = string.Empty;
	//	private int mDevIdx = 0;
	//	private List<string> mVal;
	//	private VisionToolType mToolType = VisionToolType.Locator;
	//	private Color mColor = SystemColors.Window;
	//	#endregion

	//	#region Properties
	//	/// <summary>於介面應用時代表此 Recipe 之顏色</summary>
	//	public Color BackgroundColor { get { return mColor; } }
	//	/// <summary>此資訊之註解</summary>
	//	public string Comment { get { return mCmt; } }
	//	/// <summary>對應裝置設備，為 Adept ACE</summary>
	//	public Devices Device { get { return Devices.AdeptACE; } }
	//	/// <summary>
	//	/// 對應相同裝置時之裝置索引 (適用於多台相同裝置)
	//	/// <para>例如同時有兩台 Controller/Robot 等，EX 為 0、Robot1 為 1、Robot2 為 2，可用於區分此 Recipe 隸屬何台裝置</para>
	//	/// </summary>
	//	public int DeviceIndex { get { return mDevIdx; } }
	//	/// <summary>對應 Vision 之變數名稱</summary>
	//	public string Name { get { return mPath; } }
	//	/// <summary>當前數值，為 <see cref="List{T}"/>, T 為 <seealso cref="string"/></summary>
	//	public object Value { get { return mVal; } }
	//	/// <summary>對應的 Vision 變數類型</summary>
	//	public VisionToolType VaribleType { get { return mToolType; } }
	//	#endregion

	//	#region Constructors
	//	/// <summary>建構 Adept ACE Vision 專用的 Recipe 資訊</summary>
	//	/// <param name="path">對應 Vision 之路徑</param>
	//	/// <param name="varType">對應的 Vision 類型</param>
	//	/// <param name="comment">此資訊之註解</param>
	//	/// <param name="defVal">預設數值</param>
	//	/// <param name="devIdx">對應相同裝置時之裝置索引 (適用於多台相同裝置)</param>
	//	public AceVisionRecipe(string path, VisionToolType varType, string comment, List<string> defVal = null, int devIdx = 0) {
	//		mPath = path;
	//		mCmt = comment;
	//		mToolType = varType;
	//		mVal = defVal;
	//		mDevIdx = devIdx;
	//	}

	//	/// <summary>建構 Adept ACE Vision 專用的 Recipe 資訊</summary>
	//	/// <param name="path">對應 Vision 之路徑</param>
	//	/// <param name="varType">對應的 Vision 類型</param>
	//	/// <param name="comment">此資訊之註解</param>
	//	/// <param name="defVal">預設數值</param>
	//	/// <param name="devIdx">對應相同裝置時之裝置索引 (適用於多台相同裝置)</param>
	//	/// <param name="color">於介面應用時代表此 Recipe 之顏色</param>
	//	public AceVisionRecipe(string path, VisionToolType varType, string comment, Color color, List<string> defVal = null, int devIdx = 0) {
	//		mPath = path;
	//		mCmt = comment;
	//		mToolType = varType;
	//		mVal = defVal;
	//		mDevIdx = devIdx;
	//		mColor = color;
	//	}

	//	/// <summary>使用 <see cref="XmlElmt"/> 建構 Adept ACE Vision 專用之 Recipe 資訊</summary>
	//	/// <param name="xmlData">具有資訊節點的 <see cref="XmlElmt"/></param>
	//	public AceVisionRecipe(XmlElmt xmlData) {

	//			XmlAttr attr = xmlData.Attribute("Device");
	//			if (attr != null) {
	//				if (attr.Value != "ACE_VIS" && attr.Value != "CAMPro") throw new InvalidCastException("此節點並非 Ace Vision 之參數");
	//			} else throw new ArgumentNullException("Device", "Device 節點為空，無法判斷類型");

	//			if (xmlData.Attribute("Name", out attr)) mPath = attr.Value;

	//			if (xmlData.Attribute("Comment", out attr)) mCmt = attr.Value;

	//			if (!xmlData.Attribute("DeviceIndex", out attr)) xmlData.Attribute("ListIndex", out attr);
	//			if (attr != null) mDevIdx = int.Parse(attr.Value);

	//			if (xmlData.Attribute("Type", out attr)) mToolType = (VisionToolType)(int.Parse(attr.Value));

	//			if (!xmlData.Attribute("Color", out attr)) xmlData.Attribute("LineColor", out attr);
	//			if (attr != null) {
	//				if (attr.Value.StartsWith("#")) {
	//					int argb = CtConvert.ToInteger(attr.Value.Replace("#", ""), NumericFormats.Hexadecimal);
	//					mColor = Color.FromArgb(argb);
	//				} else mColor = Color.FromName(attr.Value);
	//				if (mColor.ToArgb() == 0) mColor = SystemColors.Window;
	//			}

	//			DecodeValue(xmlData.Value);
	//	}

	//	#endregion

	//	#region Private Operations

	//	private void DecodeValue(string data) {
	//		mVal = data.Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries).ToList();
	//	}

	//	#endregion

	//	#region Public Operations
	//	/// <summary>將當前數值寫入設備</summary>
	//	/// <param name="ace">欲寫入的 Adept ACE 來源</param>
	//	public void WriteValue(CtAce ace) {
	//		if (ace != null) {
	//			if (mVal != null) {
	//				ace.Vision.RemoveLocatorModel(mPath);
	//				mVal.ForEach(model => ace.Vision.AddLocatorModel(mPath, model));
	//			} else throw new ArgumentNullException("Value", "數值為空，請先設定數值");
	//		}
	//	}

	//	/// <summary>讀取當前的數值</summary>
	//	/// <param name="ace">欲讀取的 Adept ACE</param>
	//	public void ReadValue(CtAce ace) {
	//		if (ace != null) {
	//			mVal = ace.Vision.GetCurrentModelNames(mPath);
	//		}
	//	}
	//	#endregion

	//	#region Inheritance Implements
	//	int IComparable.CompareTo(object obj) {
	//		if (obj is AceVisionRecipe) return mPath.CompareTo((obj as AceVisionRecipe).Name);    //如果 obj 是 null 就直接讓他自己跳 Exception 吧
	//		else throw new InvalidCastException(string.Format("無法比較。引數型態 {0} 非 AceVisionRecipe 可比較之型態", obj.GetType()));
	//	}

	//	int IComparer<AceVisionRecipe>.Compare(AceVisionRecipe x, AceVisionRecipe y) {
	//		return x.Name.CompareTo(y.Name);
	//	}

	//	/// <summary>取得數值所相對應的字串</summary>
	//	/// <returns>數值相對應字串</returns>
	//	public string EncodeValue() {
	//		return string.Join(",", mVal);
	//	}

	//	/// <summary>取得此 Recipe 所對應的 XML 節點資料</summary>
	//	/// <param name="nodeName">欲建立的節點名稱</param>
	//	/// <returns>節點資料</returns>
	//	public XmlElmt CreateXmlData(string nodeName) {
	//		return new XmlElmt(
	//			nodeName,
	//			EncodeValue(),
	//			new XmlAttr("Name", mPath),
	//			new XmlAttr("Comment", mCmt),
	//			new XmlAttr("Device", "ACE_VIS"),
	//			new XmlAttr("DeviceIndex", mDevIdx.ToString()),
	//			new XmlAttr("Type", ((int)mToolType).ToString()),
	//			new XmlAttr("Color", mColor.IsNamedColor ? mColor.Name : "#" + CtConvert.ToHex(mColor.ToArgb()))
	//		);
	//	}

	//	/// <summary>更改對應此 Recipe 之註解</summary>
	//	/// <param name="cmt">欲套用的新註解</param>
	//	public void SetComment(string cmt) { mCmt = cmt; }

	//	/// <summary>更改當前 Recipe 所儲存的數值</summary>
	//	/// <param name="val">欲套用的新數值</param>
	//	public void SetValue(string val) { DecodeValue(val); }
	//	#endregion

	//	#region Overrides
	//	/// <summary>將 AceVisionRecipe 轉以 <see cref="string"/> 描述</summary>
	//	/// <returns>AceVisionRecipe 之描述字串</returns>
	//	public override string ToString() {
	//		return string.Format("AceVision, {0}, {1}", mPath, mToolType.ToString());
	//	}

	//	/// <summary>比較兩個 AceVisionRecipe 是否相等。以 <see cref="VaribleType"/>, <seealso cref="Name"/> 與 <seealso cref="Value"/> 為準</summary>
	//	/// <param name="obj">欲比較的物件</param>
	//	/// <returns>(<see langword="true"/>)兩者相等 (<see langword="false"/>)兩者不同</returns>
	//	public override bool Equals(object obj) {
	//		if (obj != null && obj is AceVisionRecipe) {
	//			if (ReferenceEquals(this, obj)) return true;
	//			AceVisionRecipe aceVp = obj as AceVisionRecipe;
	//			return (mToolType == aceVp.VaribleType) && (mPath == aceVp.Name) && (mVal.SequenceEqual(aceVp.Value as List<string>));
	//		} else return false;
	//	}

	//	/// <summary>取得此物件之雜湊碼</summary>
	//	/// <returns>雜湊碼</returns>
	//	public override int GetHashCode() {
	//		return (int)mToolType ^ mPath.GetHashCode() ^ mVal.GetHashCode();
	//	}

	//	/// <summary>比較兩個 AceVisionRecipe 是否相等。以 <see cref="VaribleType"/>, <seealso cref="Name"/> 與 <seealso cref="Value"/> 做比較</summary>
	//	/// <param name="x">欲比較之物件</param>
	//	/// <param name="y">欲被比較之物件</param>
	//	/// <returns>(<see langword="true"/>)兩者相等 (<see langword="false"/>)兩者不同</returns>
	//	public static bool operator ==(AceVisionRecipe x, AceVisionRecipe y) {
	//		if ((object)x != null && (object)y != null) {
	//			if (ReferenceEquals(x, y)) return true;
	//			return (x.VaribleType == y.VaribleType) && (x.Name == y.Name) && ((x.Value as List<string>).SequenceEqual(y.Value as List<string>));
	//		} else return false;
	//	}

	//	/// <summary>比較兩個 AceVisionRecipe 是否不同。以 <see cref="VaribleType"/>, <seealso cref="Name"/> 與 <seealso cref="Value"/> 做比較</summary>
	//	/// <param name="x">欲比較之物件</param>
	//	/// <param name="y">欲被比較之物件</param>
	//	/// <returns>(<see langword="true"/>)兩者不同 (<see langword="false"/>)兩者相等</returns>
	//	public static bool operator !=(AceVisionRecipe x, AceVisionRecipe y) {
	//		return !(x == y);
	//	}
	//	#endregion
	//}

	///// <summary>適用於 Beckhoff 之 Recipe 資訊</summary>
	//public class BeckhoffRecipe : ICtRecipe, IComparer<BeckhoffRecipe> {

	//	#region Fields
	//	private string mVar = string.Empty;
	//	private string mCmt = string.Empty;
	//	private int mDevIdx = 0;
	//	private object mVal = null;
	//	private SymbolType mSymType = SymbolType.INT;
	//	private Color mColor = SystemColors.Window;
	//	#endregion

	//	#region Properties
	//	/// <summary>於介面應用時代表此 Recipe 之顏色</summary>
	//	public Color BackgroundColor { get { return mColor; } }
	//	/// <summary>此資訊之註解</summary>
	//	public string Comment { get { return mCmt; } }
	//	/// <summary>對應裝置設備，為 Beckhoff</summary>
	//	public Devices Device { get { return Devices.Beckhoff; } }
	//	/// <summary>
	//	/// 對應相同裝置時之裝置索引 (適用於多台相同裝置)
	//	/// <para>例如同時有兩台 Beckhoff CX9020 於 L/UL 機台，1 為 Loader CX9020、2 為 Unloader CX9020，可用於區分此 Recipe 隸屬何台裝置</para>
	//	/// </summary>
	//	public int DeviceIndex { get { return mDevIdx; } }
	//	/// <summary>對應 Beckhoff 之變數名稱</summary>
	//	public string Name { get { return mVar; } }
	//	/// <summary>當前數值</summary>
	//	public object Value { get { return mVal; } }
	//	/// <summary>對應的變數類型</summary>
	//	public SymbolType VaribleType { get { return mSymType; } }
	//	#endregion

	//	#region Constructors
	//	/// <summary>建構 Beckhoff 外部數值 變數專用的 Recipe 資訊</summary>
	//	/// <param name="varName">對應 Beckhoff 之變數名稱</param>
	//	/// <param name="varType">對應的變數類型</param>
	//	/// <param name="comment">此資訊之註解</param>
	//	/// <param name="defVal">預設數值</param>
	//	/// <param name="devIdx">對應相同裝置時之裝置索引 (適用於多台相同裝置)</param>
	//	public BeckhoffRecipe(string varName, SymbolType varType, string comment, object defVal = null, int devIdx = 0) {
	//		mVar = varName;
	//		mCmt = comment;
	//		mSymType = varType;
	//		mVal = defVal;
	//		mDevIdx = devIdx;
	//	}

	//	/// <summary>建構 Beckhoff 外部數值 變數專用的 Recipe 資訊</summary>
	//	/// <param name="varName">對應 Beckhoff 之變數名稱</param>
	//	/// <param name="varType">對應的變數類型</param>
	//	/// <param name="comment">此資訊之註解</param>
	//	/// <param name="defVal">預設數值</param>
	//	/// <param name="devIdx">對應相同裝置時之裝置索引 (適用於多台相同裝置)</param>
	//	/// <param name="color">於介面應用時代表此 Recipe 之顏色</param>
	//	public BeckhoffRecipe(string varName, SymbolType varType, string comment, Color color, object defVal = null, int devIdx = 0) {
	//		mVar = varName;
	//		mCmt = comment;
	//		mSymType = varType;
	//		mVal = defVal;
	//		mDevIdx = devIdx;
	//		mColor = color;
	//	}

	//	/// <summary>使用 <see cref="XmlElmt"/> 建構 Beckhoff 外部數值 專用之 Recipe 資訊</summary>
	//	/// <param name="xmlData">具有資訊節點的 <see cref="XmlElmt"/></param>
	//	public BeckhoffRecipe(XmlElmt xmlData) {

	//			XmlAttr attr = xmlData.Attribute("Device");
	//			if (attr != null) {
	//				if (attr.Value != "BECKHOFF") throw new InvalidCastException("此節點並非 Beckhoff 外部數值 之參數");
	//			} else throw new ArgumentNullException("Device", "Device 節點為空，無法判斷類型");

	//			if (xmlData.Attribute("Name", out attr)) mVar = attr.Value;

	//			if (xmlData.Attribute("Comment", out attr)) mCmt = attr.Value;

	//			if (!xmlData.Attribute("DeviceIndex", out attr)) xmlData.Attribute("ListIndex", out attr);
	//			if (attr != null) mDevIdx = int.Parse(attr.Value);

	//			if (xmlData.Attribute("Type", out attr)) mSymType = (SymbolType)(int.Parse(attr.Value));

	//			if (!xmlData.Attribute("Color", out attr)) xmlData.Attribute("LineColor", out attr);
	//			if (attr != null) {
	//				if (attr.Value.StartsWith("#")) {
	//					int argb = CtConvert.ToInteger(attr.Value.Replace("#", ""), NumericFormats.Hexadecimal);
	//					mColor = Color.FromArgb(argb);
	//				} else mColor = Color.FromName(attr.Value);
	//				if (mColor.ToArgb() == 0) mColor = SystemColors.Window;
	//			}

	//			DecodeValue(xmlData.Value);
	//	}

	//	#endregion

	//	#region Private Operations

	//	private void DecodeValue(string data) {
	//		switch (mSymType) {
	//			case SymbolType.BOOL:
	//				mVal = bool.Parse(data);
	//				break;
	//			case SymbolType.STRING:
	//			case SymbolType.WSTRING:
	//			default:
	//				mVal = data.ToString();
	//				break;
	//			case SymbolType.SINT:
	//				mVal = sbyte.Parse(data);
	//				break;
	//			case SymbolType.INT:
	//				mVal = short.Parse(data);
	//				break;
	//			case SymbolType.DINT:
	//			case SymbolType.TIME:
	//				mVal = int.Parse(data);
	//				break;
	//			case SymbolType.LINT:
	//				mVal = long.Parse(data);
	//				break;
	//			case SymbolType.USINT:
	//				mVal = byte.Parse(data);
	//				break;
	//			case SymbolType.UINT:
	//				mVal = ushort.Parse(data);
	//				break;
	//			case SymbolType.UDINT:
	//				mVal = uint.Parse(data);
	//				break;
	//			case SymbolType.ULINT:
	//				mVal = ulong.Parse(data);
	//				break;
	//			case SymbolType.REAL:
	//				mVal = float.Parse(data);
	//				break;
	//			case SymbolType.LREAL:
	//				mVal = double.Parse(data);
	//				break;
	//		}
	//	}

	//	#endregion

	//	#region Public Operations

	//	/// <summary>將當前數值寫入設備</summary>
	//	/// <param name="bkf">欲寫入的 Beckhoff PLC</param>
	//	public void WriteValue(CtBeckhoff bkf) {
	//		if (bkf != null) {
	//			if (mVal != null) {
	//				bkf.SetValue(mVar, mVal);
	//			} else throw new ArgumentNullException("Value", "數值為空，請先設定數值");
	//		}
	//	}

	//	/// <summary>讀取當前的數值</summary>
	//	/// <param name="bkf">欲讀取的 Beckhoff PLC</param>
	//	public void ReadValue(CtBeckhoff bkf) {
	//		if (bkf != null) {
	//			bkf.GetValue(mVar, out mVal);
	//		}
	//	}
	//	#endregion

	//	#region Inheritance Implements
	//	int IComparable.CompareTo(object obj) {
	//		if (obj is BeckhoffRecipe) return mVar.CompareTo((obj as BeckhoffRecipe).Name);    //如果 obj 是 null 就直接讓他自己跳 Exception 吧
	//		else throw new InvalidCastException(string.Format("無法比較。引數型態 {0} 非 BeckhoffRecipe 可比較之型態", obj.GetType()));
	//	}

	//	int IComparer<BeckhoffRecipe>.Compare(BeckhoffRecipe x, BeckhoffRecipe y) {
	//		return x.Name.CompareTo(y.Name);
	//	}

	//	/// <summary>取得數值所相對應的字串</summary>
	//	/// <returns>數值相對應字串</returns>
	//	public string EncodeValue() {
	//		string val = string.Empty;
	//		if (mVal != null) {
	//			switch (mSymType) {
	//				case SymbolType.BOOL:
	//				case SymbolType.STRING:
	//				case SymbolType.WSTRING:
	//				case SymbolType.SINT:
	//				case SymbolType.INT:
	//				case SymbolType.DINT:
	//				case SymbolType.LINT:
	//				case SymbolType.USINT:
	//				case SymbolType.UINT:
	//				case SymbolType.UDINT:
	//				case SymbolType.ULINT:
	//				case SymbolType.TIME:
	//				default:
	//					val = mVal.ToString();
	//					break;
	//				case SymbolType.REAL:
	//				case SymbolType.LREAL:
	//					val = ((double)mVal).ToString("F3");
	//					break;
	//			}
	//		}
	//		return val;
	//	}

	//	/// <summary>取得此 Recipe 所對應的 XML 節點資料</summary>
	//	/// <param name="nodeName">欲建立的節點名稱</param>
	//	/// <returns>節點資料</returns>
	//	public XmlElmt CreateXmlData(string nodeName) {
	//		return new XmlElmt(
	//			nodeName,
	//			EncodeValue(),
	//			new XmlAttr("Name", mVar),
	//			new XmlAttr("Comment", mCmt),
	//			new XmlAttr("Device", "BECKHOFF"),
	//			new XmlAttr("DeviceIndex", mDevIdx.ToString()),
	//			new XmlAttr("Type", ((int)mSymType).ToString()),
	//			new XmlAttr("Color", mColor.IsNamedColor ? mColor.Name : "#" + CtConvert.ToHex(mColor.ToArgb()))
	//		);
	//	}

	//	/// <summary>更改對應此 Recipe 之註解</summary>
	//	/// <param name="cmt">欲套用的新註解</param>
	//	public void SetComment(string cmt) { mCmt = cmt; }

	//	/// <summary>更改當前 Recipe 所儲存的數值</summary>
	//	/// <param name="val">欲套用的新數值</param>
	//	public void SetValue(string val) { DecodeValue(val); }
	//	#endregion

	//	#region Overrides
	//	/// <summary>將 BeckhoffRecipe 轉以 <see cref="string"/> 描述</summary>
	//	/// <returns>BeckhoffRecipe 之描述字串</returns>
	//	public override string ToString() {
	//		return string.Format("Bkf, {0}, {1}, {2}", mVar, mSymType.ToString(), EncodeValue());
	//	}

	//	/// <summary>比較兩個 BeckhoffRecipe 是否相等。以 <see cref="VaribleType"/>, <seealso cref="Name"/> 與 <seealso cref="Value"/> 為準</summary>
	//	/// <param name="obj">欲比較的物件</param>
	//	/// <returns>(<see langword="true"/>)兩者相等 (<see langword="false"/>)兩者不同</returns>
	//	public override bool Equals(object obj) {
	//		if (obj != null && obj is BeckhoffRecipe) {
	//			if (ReferenceEquals(this, obj)) return true;
	//			BeckhoffRecipe aceVp = obj as BeckhoffRecipe;
	//			return (mSymType == aceVp.VaribleType) && (mVar == aceVp.Name) && (mVal == aceVp.Value);
	//		} else return false;
	//	}

	//	/// <summary>取得此物件之雜湊碼</summary>
	//	/// <returns>雜湊碼</returns>
	//	public override int GetHashCode() {
	//		return (int)mSymType ^ mVar.GetHashCode() ^ mVal.GetHashCode();
	//	}

	//	/// <summary>比較兩個 BeckhoffRecipe 是否相等。以 <see cref="VaribleType"/>, <seealso cref="Name"/> 與 <seealso cref="Value"/> 做比較</summary>
	//	/// <param name="x">欲比較之物件</param>
	//	/// <param name="y">欲被比較之物件</param>
	//	/// <returns>(<see langword="true"/>)兩者相等 (<see langword="false"/>)兩者不同</returns>
	//	public static bool operator ==(BeckhoffRecipe x, BeckhoffRecipe y) {
	//		if ((object)x != null && (object)y != null) {
	//			if (ReferenceEquals(x, y)) return true;
	//			return (x.VaribleType == y.VaribleType) && (x.Name == y.Name) && (x.Value == y.Value);
	//		} else return false;
	//	}

	//	/// <summary>比較兩個 BeckhoffRecipe 是否不同。以 <see cref="VaribleType"/>, <seealso cref="Name"/> 與 <seealso cref="Value"/> 做比較</summary>
	//	/// <param name="x">欲比較之物件</param>
	//	/// <param name="y">欲被比較之物件</param>
	//	/// <returns>(<see langword="true"/>)兩者不同 (<see langword="false"/>)兩者相等</returns>
	//	public static bool operator !=(BeckhoffRecipe x, BeckhoffRecipe y) {
	//		return !(x == y);
	//	}
	//	#endregion
	//}

	///// <summary>適用於 Delta PLC 之 Recipe 資訊</summary>
	//public class DeltaPlcRecipe : ICtRecipe, IComparer<DeltaPlcRecipe> {

	//	#region Fields
	//	private string mVar = string.Empty;
	//	private string mCmt = string.Empty;
	//	private int mDevIdx = 0;
	//	private object mVal = null;
	//	private Color mColor = SystemColors.Window;
	//	#endregion

	//	#region Properties
	//	/// <summary>於介面應用時代表此 Recipe 之顏色</summary>
	//	public Color BackgroundColor { get { return mColor; } }
	//	/// <summary>此資訊之註解</summary>
	//	public string Comment { get { return mCmt; } }
	//	/// <summary>對應裝置設備，為 Delta PLC</summary>
	//	public Devices Device { get { return Devices.DELTA; } }
	//	/// <summary>
	//	/// 對應相同裝置時之裝置索引 (適用於多台相同裝置)
	//	/// <para>例如同時有兩台 Delta SE 於 L/UL 機台，1 為 Loader、2 為 Unloader，可用於區分此 Recipe 隸屬何台裝置</para>
	//	/// </summary>
	//	public int DeviceIndex { get { return mDevIdx; } }
	//	/// <summary>對應 Delta PLC 之變數名稱</summary>
	//	public string Name { get { return mVar; } }
	//	/// <summary>當前數值</summary>
	//	public object Value { get { return mVal; } }
	//	#endregion

	//	#region Constructors
	//	/// <summary>建構 Delta PLC 外部數值 變數專用的 Recipe 資訊</summary>
	//	/// <param name="varName">對應 Delta PLC 之變數名稱</param>
	//	/// <param name="comment">此資訊之註解</param>
	//	/// <param name="defVal">預設數值</param>
	//	/// <param name="devIdx">對應相同裝置時之裝置索引 (適用於多台相同裝置)</param>
	//	public DeltaPlcRecipe(string varName, string comment, object defVal = null, int devIdx = 0) {
	//		mVar = varName;
	//		mCmt = comment;
	//		mVal = defVal;
	//		mDevIdx = devIdx;
	//	}

	//	/// <summary>建構 Delta PLC 外部數值 變數專用的 Recipe 資訊</summary>
	//	/// <param name="varName">對應 Delta PLC 之變數名稱</param>
	//	/// <param name="comment">此資訊之註解</param>
	//	/// <param name="defVal">預設數值</param>
	//	/// <param name="devIdx">對應相同裝置時之裝置索引 (適用於多台相同裝置)</param>
	//	/// <param name="color">於介面應用時代表此 Recipe 之顏色</param>
	//	public DeltaPlcRecipe(string varName, string comment, Color color, object defVal = null, int devIdx = 0) {
	//		mVar = varName;
	//		mCmt = comment;
	//		mVal = defVal;
	//		mDevIdx = devIdx;
	//		mColor = color;
	//	}

	//	/// <summary>使用 <see cref="XmlElmt"/> 建構 Delta PLC 外部數值 專用之 Recipe 資訊</summary>
	//	/// <param name="xmlData">具有資訊節點的 <see cref="XmlElmt"/></param>
	//	public DeltaPlcRecipe(XmlElmt xmlData) {

	//			XmlAttr attr = xmlData.Attribute("Device");
	//			if (attr != null) {
	//				if (attr.Value != "DELTA_PLC") throw new InvalidCastException("此節點並非 Delta PLC 外部數值 之參數");
	//			} else throw new ArgumentNullException("Device", "Device 節點為空，無法判斷類型");

	//			if (xmlData.Attribute("Name", out attr)) mVar = attr.Value;

	//			if (xmlData.Attribute("Comment", out attr)) mCmt = attr.Value;

	//			if (!xmlData.Attribute("DeviceIndex", out attr)) xmlData.Attribute("ListIndex", out attr);
	//			if (attr != null) mDevIdx = int.Parse(attr.Value);

	//			if (!xmlData.Attribute("Color", out attr)) xmlData.Attribute("LineColor", out attr);
	//			if (attr != null) {
	//				if (attr.Value.StartsWith("#")) {
	//					int argb = CtConvert.ToInteger(attr.Value.Replace("#", ""), NumericFormats.Hexadecimal);
	//					mColor = Color.FromArgb(argb);
	//				} else mColor = Color.FromName(attr.Value);
	//				if (mColor.ToArgb() == 0) mColor = SystemColors.Window;
	//			}

	//			DecodeValue(xmlData.Value);
	//	}

	//	#endregion

	//	#region Private Operations

	//	private void DecodeValue(string data) {
	//		if (!string.IsNullOrEmpty(data)) {
	//			if (mVar.ToUpper().StartsWith("M") || mVar.ToUpper().StartsWith("X") || mVar.ToUpper().StartsWith("Y")) {
	//				mVal = bool.Parse(data);
	//			} else mVal = uint.Parse(data);
	//		}
	//	}

	//	#endregion

	//	#region Public Operations

	//	/// <summary>將當前數值寫入設備</summary>
	//	/// <param name="plc">欲寫入的 Delta PLC</param>
	//	public void WriteValue(CtDelta_PLC plc) {
	//		if (plc != null) {
	//			if (mVal != null) {
	//				if (mVar.ToUpper().StartsWith("M") || mVar.ToUpper().StartsWith("X") || mVar.ToUpper().StartsWith("Y"))
	//					plc.SetValue(mVar, (bool)mVal);
	//				else plc.SetValue(mVar, (uint)mVal);
	//			} else throw new ArgumentNullException("Value", "數值為空，請先設定數值");
	//		}
	//	}

	//	/// <summary>將當前數值寫入設備</summary>
	//	/// <param name="plc">欲寫入的 Delta PLC</param>
	//	/// <param name="commNum">連線埠號</param>
	//	public void WriteValue(CtDelta_PLC plc, byte commNum) {
	//		if (plc != null) {
	//			if (mVal != null) {
	//				if (mVar.ToUpper().StartsWith("M") || mVar.ToUpper().StartsWith("X") || mVar.ToUpper().StartsWith("Y"))
	//					plc.SetValue(commNum, mVar, (bool)mVal);
	//				else plc.SetValue(commNum, mVar, (uint)mVal);
	//			} else throw new ArgumentNullException("Value", "數值為空，請先設定數值");
	//		}
	//	}

	//	/// <summary>將當前數值寫入設備</summary>
	//	/// <param name="plc">欲寫入的 Delta PLC</param>
	//	public void ReadValue(CtDelta_PLC plc) {
	//		if (plc != null) {
	//			if (mVar.ToUpper().StartsWith("M") || mVar.ToUpper().StartsWith("X") || mVar.ToUpper().StartsWith("Y")) {
	//				bool bolTemp;
	//				plc.GetValue(mVar, out bolTemp);
	//				mVal = bolTemp;
	//			} else {
	//				uint uintTemp;
	//				plc.GetValue(mVar, out uintTemp);
	//				mVal = uintTemp;
	//			}
	//		}
	//	}

	//	/// <summary>將當前數值寫入設備</summary>
	//	/// <param name="plc">欲寫入的 Delta PLC</param>
	//	/// <param name="commNum">連線埠號</param>
	//	public void ReadValue(CtDelta_PLC plc, byte commNum) {
	//		if (plc != null) {
	//			if (mVar.ToUpper().StartsWith("M") || mVar.ToUpper().StartsWith("X") || mVar.ToUpper().StartsWith("Y")) {
	//				bool bolTemp;
	//				plc.GetValue(commNum, mVar, out bolTemp);
	//				mVal = bolTemp;
	//			} else {
	//				uint uintTemp;
	//				plc.GetValue(commNum, mVar, out uintTemp);
	//				mVal = uintTemp;
	//			}
	//		}
	//	}

	//	#endregion

	//	#region Inheritance Implements
	//	int IComparable.CompareTo(object obj) {
	//		if (obj is DeltaPlcRecipe) return mVar.CompareTo((obj as DeltaPlcRecipe).Name);    //如果 obj 是 null 就直接讓他自己跳 Exception 吧
	//		else throw new InvalidCastException(string.Format("無法比較。引數型態 {0} 非 DeltaPlcRecipe 可比較之型態", obj.GetType()));
	//	}

	//	int IComparer<DeltaPlcRecipe>.Compare(DeltaPlcRecipe x, DeltaPlcRecipe y) {
	//		return x.Name.CompareTo(y.Name);
	//	}

	//	/// <summary>取得數值所相對應的字串</summary>
	//	/// <returns>數值相對應字串</returns>
	//	public string EncodeValue() {
	//		string val = string.Empty;
	//		if (mVal != null) val = mVal.ToString();
	//		return val;
	//	}

	//	/// <summary>取得此 Recipe 所對應的 XML 節點資料</summary>
	//	/// <param name="nodeName">欲建立的節點名稱</param>
	//	/// <returns>節點資料</returns>
	//	public XmlElmt CreateXmlData(string nodeName) {
	//		return new XmlElmt(
	//			nodeName,
	//			EncodeValue(),
	//			new XmlAttr("Name", mVar),
	//			new XmlAttr("Comment", mCmt),
	//			new XmlAttr("Device", "DELTA_PLC"),
	//			new XmlAttr("DeviceIndex", mDevIdx.ToString()),
	//			new XmlAttr("Color", mColor.IsNamedColor ? mColor.Name : "#" + CtConvert.ToHex(mColor.ToArgb()))
	//		);
	//	}

	//	/// <summary>更改對應此 Recipe 之註解</summary>
	//	/// <param name="cmt">欲套用的新註解</param>
	//	public void SetComment(string cmt) { mCmt = cmt; }

	//	/// <summary>更改當前 Recipe 所儲存的數值</summary>
	//	/// <param name="val">欲套用的新數值</param>
	//	public void SetValue(string val) { DecodeValue(val); }
	//	#endregion

	//	#region Overrides
	//	/// <summary>將 DeltaPlcRecipe 轉以 <see cref="string"/> 描述</summary>
	//	/// <returns>DeltaPlcRecipe 之描述字串</returns>
	//	public override string ToString() {
	//		return string.Format("DltPlc, {0}, {1}", mVar, EncodeValue());
	//	}

	//	/// <summary>比較兩個 DeltaPlcRecipe 是否相等。以 <seealso cref="Name"/> 與 <seealso cref="Value"/> 為準</summary>
	//	/// <param name="obj">欲比較的物件</param>
	//	/// <returns>(<see langword="true"/>)兩者相等 (<see langword="false"/>)兩者不同</returns>
	//	public override bool Equals(object obj) {
	//		if (obj != null && obj is DeltaPlcRecipe) {
	//			if (ReferenceEquals(this, obj)) return true;
	//			DeltaPlcRecipe aceVp = obj as DeltaPlcRecipe;
	//			return (mVar == aceVp.Name) && (mVal == aceVp.Value);
	//		} else return false;
	//	}

	//	/// <summary>取得此物件之雜湊碼</summary>
	//	/// <returns>雜湊碼</returns>
	//	public override int GetHashCode() {
	//		return mVar.GetHashCode() ^ mVal.GetHashCode();
	//	}

	//	/// <summary>比較兩個 DeltaPlcRecipe 是否相等。以 <seealso cref="Name"/> 與 <seealso cref="Value"/> 做比較</summary>
	//	/// <param name="x">欲比較之物件</param>
	//	/// <param name="y">欲被比較之物件</param>
	//	/// <returns>(<see langword="true"/>)兩者相等 (<see langword="false"/>)兩者不同</returns>
	//	public static bool operator ==(DeltaPlcRecipe x, DeltaPlcRecipe y) {
	//		if ((object)x != null && (object)y != null) {
	//			if (ReferenceEquals(x, y)) return true;
	//			return (x.Name == y.Name) && (x.Value == y.Value);
	//		} else return false;
	//	}

	//	/// <summary>比較兩個 DeltaPlcRecipe 是否不同。以 <seealso cref="Name"/> 與 <seealso cref="Value"/> 做比較</summary>
	//	/// <param name="x">欲比較之物件</param>
	//	/// <param name="y">欲被比較之物件</param>
	//	/// <returns>(<see langword="true"/>)兩者不同 (<see langword="false"/>)兩者相等</returns>
	//	public static bool operator !=(DeltaPlcRecipe x, DeltaPlcRecipe y) {
	//		return !(x == y);
	//	}
	//	#endregion
	//}
}
