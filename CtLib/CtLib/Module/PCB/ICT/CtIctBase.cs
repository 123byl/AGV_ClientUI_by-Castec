using System;
using System.Collections.Generic;
using System.Linq;

namespace CtLib.Module.PCB.ICT {

	/// <summary>ICT 符號類型</summary>
	public enum SymbolTypes {
		/// <summary>封裝符號</summary>
		Package
	}

	/// <summary>ICT 圖形</summary>
	public enum GraphicTypes {
		/// <summary>弧形</summary>
		Arc,
		/// <summary>直線</summary>
		Line,
		/// <summary>矩形</summary>
		Rectangle,
		/// <summary>文字</summary>
		Text
	}

	/// <summary>ICT 基底類別，提供基本的 ICT 功能</summary>
	public abstract class IctBase {

		#region Fields
		/// <summary>使用「!」分割後的 ICT 字串</summary>
		protected IEnumerable<string> mCntx;
		#endregion

		#region Methods
		/// <summary>依照 ICT 字串產生相對應的 ICT 物件</summary>
		/// <param name="data">欲分析的 ICT 字串</param>
		/// <returns>相對應的 ICT 物件</returns>
		public static IctBase Factory(string data) {
			IctBase ict = null;
			IEnumerable<string> cntx = data.Split(new char[] { '!' }).Select(str => str.Trim());
			if (cntx.ElementAt(0) == "S") {
				switch (cntx.Count()) {
					case 4:
						ict = new IctClassDef(cntx);
						break;
					case 7:
						ict = new IctVia(cntx);
						break;
					case 8:
						ict = new IctNet(cntx);
						break;
					case 12:
						ict = new IctSymbol(cntx);
						break;
					case 17:
						GraphicTypes gph;
						if (Enum.TryParse(cntx.ElementAt(1), out gph)) ict = new IctGraphic(cntx);
						else if (Enum.TryParse(cntx.ElementAt(3), out gph)) ict = new IctClass(cntx);
						else ict = new IctRefDes(cntx);
						break;
					case 19:
						ict = new IctSubClass(cntx);
						break;
					case 28:
						ict = new IctPad(cntx);
						break;
					default:
						break;
				}
			}
			return ict;
		}
		#endregion

		#region Overrides
		/// <summary>取得此 ICT 物件之文字描述</summary>
		/// <returns>文字描述</returns>
		public override string ToString() {
			if (mCntx != null) return string.Join("!", mCntx);
			else return string.Empty;
		}
		#endregion
	}

	/// <summary>ICT Reference Designator</summary>
	public class IctRefDes : IctBase {

		#region Properties
		/// <summary>REFDES</summary>
		public string RefDes { get { return mCntx.ElementAt(1); } }
		/// <summary>COMP_CLASS</summary>
		public string CompClass { get { return mCntx.ElementAt(2); } }
		/// <summary>COMP_PART_NUMBER</summary>
		public int? CompPartNo { get { return string.IsNullOrEmpty(mCntx.ElementAt(3)) ? null as int? : int.Parse(mCntx.ElementAt(3)); } }
		/// <summary>COM_HEIGHT</summary>
		public double? CompHeight { get { return string.IsNullOrEmpty(mCntx.ElementAt(4)) ? null as double? : double.Parse(mCntx.ElementAt(4)); } }
		/// <summary>COMP_INSERTION_CODE</summary>
		public string CompInsertCode { get { return mCntx.ElementAt(5); } }
		/// <summary>COMP_DEVICE_LABEL</summary>
		public string CompDeviceLabel { get { return mCntx.ElementAt(6); } }
		/// <summary>SYM_TYPE</summary>
		public SymbolTypes SymbolType { get { return (SymbolTypes)Enum.Parse(typeof(SymbolTypes), mCntx.ElementAt(7)); } }
		/// <summary>SYM_NAME</summary>
		public string SymbolName { get { return mCntx.ElementAt(8); } }
		/// <summary>SYM_MIRROR</summary>
		public bool SymbolMirror { get { return mCntx.ElementAt(9) == "NO" ? false : true; } }
		/// <summary>SYM_ROTATE</summary>
		public double SymbolRotate { get { return double.Parse(mCntx.ElementAt(10)); } }
		/// <summary>SYM_X</summary>
		public double SymbolX { get { return double.Parse(mCntx.ElementAt(11)); } }
		/// <summary>SYM_Y</summary>
		public double SymbolY { get { return double.Parse(mCntx.ElementAt(12)); } }
		/// <summary>COMP_VALUE</summary>
		public string CompValue { get { return mCntx.ElementAt(13); } }
		/// <summary>COMP_TOL</summary>
		public double? CompTolerance { get { return string.IsNullOrEmpty(mCntx.ElementAt(14)) ? null as double? : double.Parse(mCntx.ElementAt(14)); } }
		/// <summary>COMP_VOLTAGE</summary>
		public double? CompVoltage { get { return string.IsNullOrEmpty(mCntx.ElementAt(15)) ? null as double? : double.Parse(mCntx.ElementAt(15)); } }
		#endregion

		#region Constructors
		/// <summary>建構 Reference Designator</summary>
		/// <param name="data">ICT 字串</param>
		public IctRefDes(string data) {
			mCntx = data.Split(new char[] { '!' }).Select(str => str?.Trim());
			if (mCntx.Count() == 16) {
				if (mCntx.ElementAt(0) != "S") throw new InvalidCastException("Couldn't parse comments or file descriptions");
			}
		}

		/// <summary>使用已分割之 ICT 文字進行建構</summary>
		/// <param name="split">已分割之 ICT 字串集合</param>
		public IctRefDes(IEnumerable<string> split) {
			mCntx = split;
		}
		#endregion
	}

	/// <summary>ICT Graphics</summary>
	public class IctGraphic : IctBase {

		#region Properties
		/// <summary>GRAPHIC_DATA_NAME</summary>
		public GraphicTypes GraphicType { get { return (GraphicTypes)Enum.Parse(typeof(GraphicTypes), mCntx.ElementAt(1)); } }
		/// <summary>GRAPHIC_DATA_NUMBER</summary>
		public int? DataNo { get { return string.IsNullOrEmpty(mCntx.ElementAt(1)) ? null as int? : int.Parse(mCntx.ElementAt(2)); } }
		/// <summary>RECORD_TAG</summary>
		public string RecordTag { get { return mCntx.ElementAt(3); } }
		/// <summary>GRAPHIC_DATA_1</summary>
		public string Data_1 { get { return mCntx.ElementAt(4); } }
		/// <summary>GRAPHIC_DATA_2</summary>
		public string Data_2 { get { return mCntx.ElementAt(5); } }
		/// <summary>GRAPHIC_DATA_3</summary>
		public string Data_3 { get { return mCntx.ElementAt(6); } }
		/// <summary>GRAPHIC_DATA_4</summary>
		public string Data_4 { get { return mCntx.ElementAt(7); } }
		/// <summary>GRAPHIC_DATA_5</summary>
		public string Data_5 { get { return mCntx.ElementAt(8); } }
		/// <summary>GRAPHIC_DATA_6</summary>
		public string Data_6 { get { return mCntx.ElementAt(9); } }
		/// <summary>GRAPHIC_DATA_7</summary>
		public string Data_7 { get { return mCntx.ElementAt(10); } }
		/// <summary>GRAPHIC_DATA_8</summary>
		public string Data_8 { get { return mCntx.ElementAt(11); } }
		/// <summary>GRAPHIC_DATA_9</summary>
		public string Data_9 { get { return mCntx.ElementAt(12); } }
		/// <summary>SUBCLASS</summary>
		public string Subclass { get { return mCntx.ElementAt(13); } }
		/// <summary>SYM_NAME</summary>
		public string SymbolName { get { return mCntx.ElementAt(14); } }
		/// <summary>REFDES</summary>
		public string RefDes { get { return mCntx.ElementAt(15); } }
		#endregion

		#region Constructors
		/// <summary>使用 ICT 字串進行建構</summary>
		/// <param name="data">ICT 文字</param>
		public IctGraphic(string data) {
			mCntx = data.Split(new char[] { '!' }).Select(str => str.Trim());
			if (mCntx.Count() == 16) {
				if (mCntx.ElementAt(0) != "S") throw new InvalidCastException("Couldn't parse comments or file descriptions");
			}
		}

		/// <summary>使用已分割之 ICT 文字進行建構</summary>
		/// <param name="split">已分割之 ICT 字串集合</param>
		public IctGraphic(IEnumerable<string> split) {
			mCntx = split;
		}
		#endregion
	}

	/// <summary>ICT Symbol</summary>
	public class IctSymbol : IctBase {

		#region Properties
		/// <summary>SYM_NAME</summary>
		public string SymbolName { get { return mCntx.ElementAt(1); } }
		/// <summary>SYM_MIRROR</summary>
		public bool SymbolMirror { get { return bool.Parse(mCntx.ElementAt(2)); } }
		/// <summary>PIN_NAME</summary>
		public string PinName { get { return mCntx.ElementAt(3); } }
		/// <summary>PIN_NUMBER</summary>
		public int PinNumber { get { return int.Parse(mCntx.ElementAt(4)); } }
		/// <summary>PIN_X</summary>
		public double PinX { get { return double.Parse(mCntx.ElementAt(5)); } }
		/// <summary>PIN_Y</summary>
		public double PinY { get { return double.Parse(mCntx.ElementAt(6)); } }
		/// <summary>PAD_STACK_NAME</summary>
		public string PadStackName { get { return mCntx.ElementAt(7); } }
		/// <summary>REFDES</summary>
		public string RefDes { get { return mCntx.ElementAt(8); } }
		/// <summary>PIN_ROTATION</summary>
		public double PinRotation { get { return double.Parse(mCntx.ElementAt(9)); } }
		/// <summary>TEST_POINT</summary>
		public string TestPoint { get { return mCntx.ElementAt(10); } }
		#endregion

		#region Constructors
		/// <summary>使用 ICT 字串進行建構</summary>
		/// <param name="data">ICT 文字</param>
		public IctSymbol(string data) {
			mCntx = data.Split(new char[] { '!' }).Select(str => str.Trim());
			if (mCntx.Count() == 11) {
				if (mCntx.ElementAt(0) != "S") throw new InvalidCastException("Couldn't parse comments or file descriptions");
			}
		}

		/// <summary>使用已分割之 ICT 文字進行建構</summary>
		/// <param name="split">已分割之 ICT 字串集合</param>
		public IctSymbol(IEnumerable<string> split) {
			mCntx = split;
		}
		#endregion
	}

	/// <summary>ICT Net</summary>
	public class IctNet : IctBase {

		#region Properties
		/// <summary>NET_NAME</summary>
		public string NetName { get { return mCntx.ElementAt(1); } }
		/// <summary>REFDES</summary>
		public string RefDes { get { return mCntx.ElementAt(2); } }
		/// <summary>PIN_NUMBER</summary>
		public int PinNumber { get { return int.Parse(mCntx.ElementAt(3)); } }
		/// <summary>PIN_NAME</summary>
		public string PinName { get { return mCntx.ElementAt(4); } }
		/// <summary>PIN_GROUND</summary>
		public string PinGound { get { return mCntx.ElementAt(5); } }
		/// <summary>PIN_POWER</summary>
		public string PinPower { get { return mCntx.ElementAt(6); } }
		#endregion

		#region Constructors
		/// <summary>使用 ICT 字串進行建構</summary>
		/// <param name="data">ICT 文字</param>
		public IctNet(string data) {
			mCntx = data.Split(new char[] { '!' }).Select(str => str.Trim());
			if (mCntx.Count() == 7) {
				if (mCntx.ElementAt(0) != "S") throw new InvalidCastException("Couldn't parse comments or file descriptions");
			}
		}

		/// <summary>使用已分割之 ICT 文字進行建構</summary>
		/// <param name="split">已分割之 ICT 字串集合</param>
		public IctNet(IEnumerable<string> split) {
			mCntx = split;
		}
		#endregion
	}

	/// <summary>ICT Class and SubClass Definition</summary>
	public class IctClassDef : IctBase {

		#region Properties
		/// <summary>CLASS</summary>
		public string Class { get { return mCntx.ElementAt(0); } }
		/// <summary>SUBCLASS</summary>
		public string SubClass { get { return mCntx.ElementAt(1); } }
		#endregion

		#region Constructors
		/// <summary>使用 ICT 字串進行建構</summary>
		/// <param name="data">ICT 文字</param>
		public IctClassDef(string data) {
			mCntx = data.Split(new char[] { '!' }).Select(str => str.Trim());
			if (mCntx.Count() == 3) {
				if (mCntx.ElementAt(0) != "S") throw new InvalidCastException("Couldn't parse comments or file descriptions");
			}
		}

		/// <summary>使用已分割之 ICT 文字進行建構</summary>
		/// <param name="split">已分割之 ICT 字串集合</param>
		public IctClassDef(IEnumerable<string> split) {
			mCntx = split;
		}
		#endregion
	}

	/// <summary>ICT Pad</summary>
	public class IctPad : IctBase {

		#region Properties
		/// <summary>PAD_NAME</summary>
		public string PadName { get { return mCntx.ElementAt(1); } }
		/// <summary>REC_NUMBER</summary>
		public int RecNumber { get { return int.Parse(mCntx.ElementAt(2)); } }
		/// <summary>LAYER</summary>
		public string Layer { get { return mCntx.ElementAt(3); } }
		/// <summary>FIXFLAG</summary>
		public string FixFlag { get { return mCntx.ElementAt(4); } }
		/// <summary>VIAFLAG</summary>
		public string ViaFlag { get { return mCntx.ElementAt(5); } }
		/// <summary>PADSHPE1</summary>
		public string PadShpae { get { return mCntx.ElementAt(6); } }
		/// <summary>PADWIDTH</summary>
		public double PadWidth { get { return double.Parse(mCntx.ElementAt(7)); } }
		/// <summary>PADHGHT</summary>
		public double PadHeight { get { return double.Parse(mCntx.ElementAt(8)); } }
		/// <summary>PADXOFF</summary>
		public double PadXOffset { get { return double.Parse(mCntx.ElementAt(9)); } }
		/// <summary>PADYOFF</summary>
		public double PadYOffset { get { return double.Parse(mCntx.ElementAt(10)); } }
		/// <summary>PADFLASH</summary>
		public string PadFlash { get { return mCntx.ElementAt(11); } }
		/// <summary>PADSHAPENAME</summary>
		public string PadShapeName { get { return mCntx.ElementAt(12); } }
		/// <summary>TRELSHAPE1</summary>
		public string TrelShpae { get { return mCntx.ElementAt(13); } }
		/// <summary>TRELWIDTH</summary>
		public double TrelWidth { get { return double.Parse(mCntx.ElementAt(14)); } }
		/// <summary>TRELHGHT</summary>
		public double TrelHeight { get { return double.Parse(mCntx.ElementAt(15)); } }
		/// <summary>TRELXOFF</summary>
		public double TrelXOffset { get { return double.Parse(mCntx.ElementAt(16)); } }
		/// <summary>TRELYOFF</summary>
		public double TrelYOffset { get { return double.Parse(mCntx.ElementAt(17)); } }
		/// <summary>TRELFLASH</summary>
		public string TrelFlash { get { return mCntx.ElementAt(18); } }
		/// <summary>TRELSHAPENAME</summary>
		public string TrelShapeName { get { return mCntx.ElementAt(19); } }
		/// <summary>APADSHAPE1</summary>
		public string APadShpae { get { return mCntx.ElementAt(20); } }
		/// <summary>APADWIDTH</summary>
		public double APadWidth { get { return double.Parse(mCntx.ElementAt(21)); } }
		/// <summary>APADHGHT</summary>
		public double APadHeight { get { return double.Parse(mCntx.ElementAt(22)); } }
		/// <summary>APADXOFF</summary>
		public double APadXOffset { get { return double.Parse(mCntx.ElementAt(23)); } }
		/// <summary>APADYOFF</summary>
		public double APadYOffset { get { return double.Parse(mCntx.ElementAt(24)); } }
		/// <summary>APADFLASH</summary>
		public string APadFlash { get { return mCntx.ElementAt(25); } }
		/// <summary>APADSHAPENAME</summary>
		public string APadShapeName { get { return mCntx.ElementAt(26); } }
		#endregion

		#region Constructors
		/// <summary>使用 ICT 字串進行建構</summary>
		/// <param name="data">ICT 文字</param>
		public IctPad(string data) {
			mCntx = data.Split(new char[] { '!' }).Select(str => str.Trim());
			if (mCntx.Count() == 27) {
				if (mCntx.ElementAt(0) != "S") throw new InvalidCastException("Couldn't parse comments or file descriptions");
			}
		}

		/// <summary>使用已分割之 ICT 文字進行建構</summary>
		/// <param name="split">已分割之 ICT 字串集合</param>
		public IctPad(IEnumerable<string> split) {
			mCntx = split;
		}
		#endregion
	}

	/// <summary>ICT Via</summary>
	public class IctVia : IctBase {

		#region Properties
		/// <summary>VIA_X</summary>
		public double ViaX { get { return double.Parse(mCntx.ElementAt(1)); } }
		/// <summary>VIA_Y</summary>
		public double ViaY { get { return double.Parse(mCntx.ElementAt(2)); } }
		/// <summary>PAD_STACK_NAME</summary>
		public string PadStackName { get { return mCntx.ElementAt(3); } }
		/// <summary>NET_NAME</summary>
		public string NetName { get { return mCntx.ElementAt(4); } }
		/// <summary>TEST_POINT</summary>
		public string TestPoint { get { return mCntx.ElementAt(5); } }
		#endregion

		#region Constructors
		/// <summary>使用 ICT 字串進行建構</summary>
		/// <param name="data">ICT 文字</param>
		public IctVia(string data) {
			mCntx = data.Split(new char[] { '!' }).Select(str => str.Trim());
			if (mCntx.Count() == 6) {
				if (mCntx.ElementAt(0) != "S") throw new InvalidCastException("Couldn't parse comments or file descriptions");
			}
		}

		/// <summary>使用已分割之 ICT 文字進行建構</summary>
		/// <param name="split">已分割之 ICT 字串集合</param>
		public IctVia(IEnumerable<string> split) {
			mCntx = split;
		}
		#endregion
	}

	/// <summary>ICT Class Information</summary>
	public class IctClass : IctBase {

		#region Properteis
		/// <summary>CLASS</summary>
		public string Class { get { return mCntx.ElementAt(1); } }
		/// <summary>SUBCLASS</summary>
		public string SubClass { get { return mCntx.ElementAt(2); } }
		/// <summary>GRAPHIC_DATA_NAME</summary>
		public GraphicTypes GraphicData { get { return (GraphicTypes)Enum.Parse(typeof(GraphicTypes), mCntx.ElementAt(3)); } }
		/// <summary>GRAPHIC_DATA_NUMBER</summary>
		public int GraphicDataNumber { get { return int.Parse(mCntx.ElementAt(4)); } }
		/// <summary>RECORD_TAG</summary>
		public string RecordTag { get { return mCntx.ElementAt(5); } }
		/// <summary>GRAPHIC_DATA_1</summary>
		public string GraphicData_1 { get { return mCntx.ElementAt(6); } }
		/// <summary>GRAPHIC_DATA_2</summary>
		public string GraphicData_2 { get { return mCntx.ElementAt(7); } }
		/// <summary>GRAPHIC_DATA_3</summary>
		public string GraphicData_3 { get { return mCntx.ElementAt(8); } }
		/// <summary>GRAPHIC_DATA_4</summary>
		public string GraphicData_4 { get { return mCntx.ElementAt(9); } }
		/// <summary>GRAPHIC_DATA_5</summary>
		public string GraphicData_5 { get { return mCntx.ElementAt(10); } }
		/// <summary>GRAPHIC_DATA_6</summary>
		public string GraphicData_6 { get { return mCntx.ElementAt(11); } }
		/// <summary>GRAPHIC_DATA_7</summary>
		public string GraphicData_7 { get { return mCntx.ElementAt(12); } }
		/// <summary>GRAPHIC_DATA_8</summary>
		public string GraphicData_8 { get { return mCntx.ElementAt(13); } }
		/// <summary>GRAPHIC_DATA_9</summary>
		public string GraphicData_9 { get { return mCntx.ElementAt(14); } }
		/// <summary>NET_NAME</summary>
		public string NetName { get { return mCntx.ElementAt(15); } }
		#endregion

		#region Constructors
		/// <summary>使用 ICT 字串進行建構</summary>
		/// <param name="data">ICT 文字</param>
		public IctClass(string data) {
			mCntx = data.Split(new char[] { '!' }).Select(str => str.Trim());
			if (mCntx.Count() == 16) {
				if (mCntx.ElementAt(0) != "S") throw new InvalidCastException("Couldn't parse comments or file descriptions");
			}
		}

		/// <summary>使用已分割之 ICT 文字進行建構</summary>
		/// <param name="split">已分割之 ICT 字串集合</param>
		public IctClass(IEnumerable<string> split) {
			mCntx = split;
		}
		#endregion
	}

	/// <summary>ICT SubClass</summary>
	public class IctSubClass : IctBase {

		#region Properteis
		/// <summary>SUBCLASS</summary>
		public string SubClass { get { return mCntx.ElementAt(1); } }
		/// <summary>PAD_SHAPE_NAME</summary>
		public string PadShapeName { get { return mCntx.ElementAt(2); } }
		/// <summary>GRAPHIC_DATA_NAME</summary>
		public GraphicTypes GraphicData { get { return (GraphicTypes)Enum.Parse(typeof(GraphicTypes), mCntx.ElementAt(3)); } }
		/// <summary>GRAPHIC_DATA_NUMBER</summary>
		public int GraphicDataNumber { get { return int.Parse(mCntx.ElementAt(4)); } }
		/// <summary>RECORD_TAG</summary>
		public string RecordTag { get { return mCntx.ElementAt(5); } }
		/// <summary>GRAPHIC_DATA_1</summary>
		public string GraphicData_1 { get { return mCntx.ElementAt(6); } }
		/// <summary>GRAPHIC_DATA_2</summary>
		public string GraphicData_2 { get { return mCntx.ElementAt(7); } }
		/// <summary>GRAPHIC_DATA_3</summary>
		public string GraphicData_3 { get { return mCntx.ElementAt(8); } }
		/// <summary>GRAPHIC_DATA_4</summary>
		public string GraphicData_4 { get { return mCntx.ElementAt(9); } }
		/// <summary>GRAPHIC_DATA_5</summary>
		public string GraphicData_5 { get { return mCntx.ElementAt(10); } }
		/// <summary>GRAPHIC_DATA_6</summary>
		public string GraphicData_6 { get { return mCntx.ElementAt(11); } }
		/// <summary>GRAPHIC_DATA_7</summary>
		public string GraphicData_7 { get { return mCntx.ElementAt(12); } }
		/// <summary>GRAPHIC_DATA_8</summary>
		public string GraphicData_8 { get { return mCntx.ElementAt(13); } }
		/// <summary>GRAPHIC_DATA_9</summary>
		public string GraphicData_9 { get { return mCntx.ElementAt(14); } }
		/// <summary>PAD_STACK_NAME</summary>
		public string PadStackName { get { return mCntx.ElementAt(15); } }
		/// <summary>REFDES</summary>
		public string RefDes { get { return mCntx.ElementAt(16); } }
		/// <summary>PIN_NUMBER</summary>
		public int PinNumber { get { return int.Parse(mCntx.ElementAt(17)); } }
		#endregion

		#region Constructors
		/// <summary>使用 ICT 字串進行建構</summary>
		/// <param name="data">ICT 文字</param>
		public IctSubClass(string data) {
			mCntx = data.Split(new char[] { '!' }).Select(str => str.Trim());
			if (mCntx.Count() == 18) {
				if (mCntx.ElementAt(0) != "S") throw new InvalidCastException("Couldn't parse comments or file descriptions");
			}
		}

		/// <summary>使用已分割之 ICT 文字進行建構</summary>
		/// <param name="split">已分割之 ICT 字串集合</param>
		public IctSubClass(IEnumerable<string> split) {
			mCntx = split;
		}
		#endregion
	}
}
