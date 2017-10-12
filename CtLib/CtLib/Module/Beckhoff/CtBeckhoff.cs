using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CtLib.Forms;
using CtLib.Library;
using CtLib.Module.Utility;

using TwinCAT.Ads;

namespace CtLib.Module.Beckhoff {

	#region Declaration - Enumeration

	/// <summary>
	/// Beckhoff PLC 內部資料型態
	/// <para>簡易參數:</para>
	/// <para>SINT/USINT = sbyte/byte   |   INT/UINT = short/ushort</para>
	/// <para>DINT/UDINT = int/uint     |   LINT/ULINT = long/ulong</para>
	/// <para>REAL/LREAL = float/double</para>
	/// </summary>
	public enum SymbolType : sbyte {
		/// <summary>Unknown type</summary>
		UNKNOWN = -1,
		/// <summary>Boolean</summary>
		BOOL = 33,
		/// <summary>String</summary>
		STRING = 30,
		/// <summary>Wide String</summary>
		WSTRING = 31,
		/// <summary>Signed Byte</summary>
		SINT = 16,
		/// <summary>Int16/Short Integer</summary>
		INT = 2,
		/// <summary>Int32/Integer</summary>
		DINT = 3,
		/// <summary>Int64/Long</summary>
		LINT = 20,
		/// <summary>Byte</summary>
		USINT = 17,
		/// <summary>UInt16/UShort</summary>
		UINT = 18,
		/// <summary>UInt32/UInteger</summary>
		UDINT = 19,
		/// <summary>UInt64/ULong</summary>
		ULINT = 21,
		/// <summary>Single/Float</summary>
		REAL = 4,
		/// <summary>Double</summary>
		LREAL = 5,
		/// <summary>Time</summary>
		TIME = 40,
		/// <summary>Array or Structure</summary>
		ARRAY_STRUC = 99
	}

	/// <summary>Beckhoff PLC 狀態</summary>
	public enum AdsStatus : byte {
		/// <summary>Invalided state</summary>
		Invalid = 0,
		/// <summary>Idles state</summary>
		Idle = 1,
		/// <summary>Reset state</summary>
		Reset = 2,
		/// <summary>Initialized</summary>
		Init = 3,
		/// <summary>Started</summary>
		Start = 4,
		/// <summary>Running</summary>
		Run = 5,
		/// <summary>Stopped</summary>
		Stop = 6,
		/// <summary>Saved Configuration</summary>
		SaveCfg = 7,
		/// <summary>Load Configuration</summary>
		LoadCfg = 8,
		/// <summary>Power Failure</summary>
		PowerFailure = 9,
		/// <summary>Power Good</summary>
		PowerGood = 10,
		/// <summary>Error state</summary>
		Error = 11,
		/// <summary>Shutting down</summary>
		Shutdown = 12,
		/// <summary>Suspended</summary>
		Suspend = 13,
		/// <summary>Resumed</summary>
		Resume = 14,
		/// <summary>System is in config mode</summary>
		Config = 15,
		/// <summary>System should restart in config mode</summary>
		Reconfig = 16,
		/// <summary>Max State</summary>
		MaxStates = 17
	}

	/// <summary>Beckhoff PLC Device State</summary>
	public enum DeviceStatus : short {
		/// <summary>Init State</summary>
		Init = 0x01,
		/// <summary>Pre-Operational State</summary>
		PreOP = 0x02,
		/// <summary>Bootstrap State</summary>
		BootStrap = 0x03,
		/// <summary>Safe-Operational State</summary>
		SaveOP = 0x04,
		/// <summary>Operational State</summary>
		OP = 0x08,
		/// <summary>Statemachine error in the EtherCAT slave</summary>
		Error = 0x10,
		/// <summary>Invalid VendorId, Product Code, RevisionsNo or SerialNo</summary>
		InvalidVPRS = 0x20,
		/// <summary>Error occured while sending initialization commands</summary>
		InitCmdError = 0x40,
	}

	#endregion

	/// <summary>集合相關使用於 Beckhoff PLC 之應用</summary>
	/// <example><code language="C#">
	/// /*-- Create object --*/
	/// CtBeckhoff mBkf = new CtBeckhoff();
	/// AddEventHandler();                              //Add relative events
	/// 
	/// /*-- Connect --*/
	/// mBkf.Connect("5.24.12.31.8.1.1", 801);
	/// 
	/// /*-- Simply Operations, get and set values --*/
	/// bool var1;
	/// mBkf.GetValue("MAIN.pF_Variable1", out var1);   //Get the value and assign to var1
	/// mBkf.SetValue("MAIN.pF_Variable2", false);      //Set "pF_Variable2" with "false"
	/// </code></example>
	public class CtBeckhoff : IDisposable, ICtVersion {

		#region Version

		/// <summary>CtBeckhoff 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 0.0.0  William [2012/07/20]
		///      + CtBeckhoff
		/// 
		/// 1.0.0  Ahern [2014/09/10]
		///      + 連線/中斷連線
		///      + GetValue/SetValue，並捨棄舊有BinaryWriter方式(經測試Writer方式速度較慢)
		///      + IDisposable，在Release時能更完善的釋放資源
		///      + AdsNotification與SymbolChanged事件，現在註冊後收到相關事件將由CtBeckhoff發報
		///      
		/// 1.0.1  Ahern [2014/09/15]
		///      + 若NetID不正確，跳出對話視窗讓使用者輸入
		///      
		/// 1.0.2  Ahern [2014/12/17]
		///      \ ExceptionHandle 改採 StackTrace 方法
		///      
		/// 1.0.3  Ahern [2015/03/01]
		///      - Try-Catach
		///      
		/// 1.0.4  Ahern [2015/03/02]
		///     + GetValue of string size
		///     + GetValue of get List(Of byte) of AdsStream
		///     - 使用實質型態之 GetValue
		///     + 使用泛型之 GetValue
		///     
		///	1.1.0  Ahern [2015/08/01]
		///		\ 套用新版的 ITcAdsSymbol5 來進行變數讀取
		///		
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(1, 1, 0, "2016/08/01", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Definitions

		/// <summary>預設NetID，用於連線至Beckhoff PLC</summary>
		private const string BECKHOFF_NETID = "";
		/// <summary>預設Port，用於連線至Beckhoff PLC</summary>
		private const int BECKHOFF_PORT = 801;
		/// <summary>預設Notification之CycleTime</summary>
		private const int BECKHOFF_NOTIFY_CYCLE = 20;
		/// <summary>預設Notification之MaxDelay</summary>
		private const int BECKHOFF_NOTIFY_DELAY = 150;
		#endregion

		#region Declaration - Properties

		/// <summary>取得目前連線狀態</summary>
		public bool IsConnected {
			get { return (mAdsClnt == null) ? false : mAdsClnt.IsConnected; }
		}

		/// <summary>取得或設定當前PLC之NetID，用於連線至Beckhoff PLC</summary>
		[DefaultValue("123")]
		public string NetID { get; set; }
		/// <summary>取得或設定當前 ADS Device Port，用於連線至Beckhoff PLC</summary>
		[DefaultValue(BECKHOFF_PORT)]
		public int AdsPort { get; set; }
		/// <summary>取得或設定發生錯誤時是否將錯誤訊息以事件方式發報</summary>
		[DefaultValue(true)]
		public bool EnableMessage { get; set; }
		#endregion

		#region Declaration - Fields

		/// <summary>透過TwinCAT.Ads連結至Beckhoff PLC以同步進行相關動作</summary>
		private TcAdsClient mAdsClnt;
		/// <summary>儲存DeviceNotification之變數名稱(TKey:string)與其Handle值(TValue:int)</summary>
		private Dictionary<string, int> mNotifyHdl = new Dictionary<string, int>();
		/// <summary>[Flag] 目前是否有DeviceNotification</summary>
		private bool mNotified = false;
		#endregion

		#region Declaration - Events

		/// <summary>發生Boolean值改變事件</summary>
		public event EventHandler<BeckhoffBoolEventArgs> OnBoolEventChanged;
		/// <summary>Beckhoff Symbol(Variable) 數值變更事件</summary>
		public event EventHandler<BeckhoffSymbolEventArgs> OnSymbolChanged;
		/// <summary>發報訊息至外部</summary>
		public event EventHandler<BeckhoffMessageEventArgs> OnMessage;

		/// <summary>觸發Boolean改變事件</summary>
		/// <param name="e">Boolean事件參數</param>
		protected virtual void OnBoolEventOccur(BeckhoffBoolEventArgs e) {
			EventHandler<BeckhoffBoolEventArgs> handler = OnBoolEventChanged;
			if (handler != null)
				handler(this, e);
		}

		/// <summary>觸發SymbolChanged事件</summary>
		/// <param name="e">SymbolChanged事件參數</param>
		protected virtual void OnSymbolEventChanged(BeckhoffSymbolEventArgs e) {
			EventHandler<BeckhoffSymbolEventArgs> handler = OnSymbolChanged;
			if (handler != null)
				handler(this, e);
		}

		/// <summary>觸發訊息更新事件</summary>
		/// <param name="e">Message事件參數</param>
		protected virtual void OnMessageUpdate(BeckhoffMessageEventArgs e) {
			EventHandler<BeckhoffMessageEventArgs> handler = OnMessage;
			if (handler != null)
				handler(this, e);
		}
		#endregion

		#region Function - Core

		/// <summary>建構一個全新的Beckhoff Ads物件</summary>
		public CtBeckhoff() {
			EnableMessage = true;
		}

		/// <summary>建構一個全新的Beckhoff Ads物件</summary>
		/// <param name="enbMsg">是否啟用錯誤訊息事件，於 <see cref="Exception"/> 發生時將以事件方式通知</param>
		public CtBeckhoff(bool enbMsg) {
			EnableMessage = enbMsg;
		}

		/// <summary>建立一個新Beckhoff Ads元件，並直接施行連線</summary>
		/// <param name="netID">Beckhoff PLC 之 NetID</param>
		/// <param name="adsPort">Beckhoff PLC 之 Ads Port</param>
		/// <param name="autoConnect">是否自動連線?  (<see langword="true"/>)完成設定後自動進行連線 (<see langword="false"/>)完成後不施行任何動作</param>
		/// <param name="enbMsg">是否自動回報相關錯誤狀態 <see cref="EnableMessage"/></param>
		public CtBeckhoff(string netID, int adsPort, bool autoConnect = true, bool enbMsg = true) {
			EnableMessage = enbMsg;
			NetID = netID;
			AdsPort = adsPort;
			if (autoConnect) Connect(netID, adsPort);
		}

		/// <summary>中斷與Beckhoff Ads連線，並釋放相關資源</summary>
		public void Dispose() {
			try {
				Dispose(true);
				GC.SuppressFinalize(this);
			} catch (ObjectDisposedException ex) {
				ExceptionHandle(Stat.ER_SYSTEM, ex);
			}
		}

		/// <summary>連線至Beckhoff PLC</summary>
		/// <param name="netID">Beckhoff PLC 之 NetID</param>
		/// <param name="adsPort">Beckhoff PLC 之 Ads Port</param>
		/// <returns>Status Code</returns>
		public Stat Connect(string netID, int adsPort = BECKHOFF_PORT) {
			Stat stt = Stat.SUCCESS;
			try {
				/*-- 檢查NetID是否實際存在 --*/
				if (netID == "") {
					stt = Stat.ER_SYS_INVARG;
					throw (new Exception("錯誤或為空的 NetID"));
				}

				/*-- 檢查NetID是不是正確，不正確則跳出對話視窗讓使用者輸入 --*/
				string strID = netID;
				do {
					if (strID.Split(CtConst.CHR_PERIOD, StringSplitOptions.RemoveEmptyEntries).Length == 6) {
						break;
					} else {
						if (CtInput.Text(out strID, "Beckhoff Ads NetID", "請輸入 Beckhoff PLC 之 NetID") != Stat.SUCCESS) {
							stt = Stat.ER4_BKF;
							throw (new Exception("使用者拒絕輸入 NetID"));
						}
					}
				} while (true);

				/*-- 如果已經建立，則先Dispose掉 --*/
				if (mAdsClnt != null) {
					mAdsClnt.Dispose();
					mAdsClnt = null;
				}

				/*-- 建立TcAdsClient --*/
				mAdsClnt = new TcAdsClient();

				/*-- 如果建不出來，發Alarm --*/
				if (mAdsClnt == null) {
					stt = Stat.ER4_BKF;
					throw (new Exception("無法建立 TcAdsClient"));
				}

				/*-- 嘗試連線 --*/
				mAdsClnt.Connect(strID, adsPort);
				if (mAdsClnt.IsConnected) {
					NetID = strID;  //AdsPort不知為何，連線後仍然為0，故不做複寫的動作
					OnBoolEventOccur(new BeckhoffBoolEventArgs(BeckhoffBoolEvents.Connection, true));   //如果連線成功，觸發Event
				} else
					stt = Stat.ER3_BKF_CONT;

			} catch (Exception ex) {
				if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
				ExceptionHandle(stt, ex);
			}
			return stt;
		}

		/// <summary>中斷與Beckhoff PLC之連線</summary>
		/// <returns>Status Code</returns>
		public Stat Disconnect() {
			Stat stt = Stat.SUCCESS;
			try {
				/*-- 如連線仍存在，Dispose掉 --*/
				if ((mAdsClnt != null) && (mAdsClnt.IsConnected)) {
					mAdsClnt.Dispose();
					mAdsClnt = null;
				}

				/*-- 觸發Event --*/
				OnBoolEventOccur(new BeckhoffBoolEventArgs(BeckhoffBoolEvents.Connection, false));
			} catch (Exception ex) {
				stt = Stat.ER_SYSTEM;
				ExceptionHandle(stt, ex);
			}
			return stt;
		}

		/// <summary>實作Dispose</summary>
		/// <param name="isDisposing"></param>
		protected virtual void Dispose(bool isDisposing) {
			try {
				/*-- 如有目前還有DeviceNotification，取消事件 --*/
				if (mNotified) {
					foreach (KeyValuePair<string, int> pair in mNotifyHdl) {
						mAdsClnt.DeleteDeviceNotification(pair.Value);
					}
					mNotifyHdl.Clear();
					mAdsClnt.AdsNotificationEx -= mAdsClnt_AdsNotificationEx;
				}

				/*-- Disconnect --*/
				Disconnect();
			} catch (Exception ex) {
				ExceptionHandle(Stat.ER_SYSTEM, ex);
			}
		}
		#endregion

		#region Function - Method

		/// <summary>控管Exception，統一發報管理</summary>
		/// <param name="stt">Status</param>
		/// <param name="title">Title</param>
		/// <param name="content">Message Content</param>
		private void ExceptionHandle(Stat stt, string title, string content) {
			/*-- 如果要發報訊息事件 --*/
			if (EnableMessage) {
				/*-- 依照Stt分類 --*/
				sbyte msgType = 0;
				if ((int)stt == 0) {
					msgType = 0;
				} else if ((int)stt < 0) {
					msgType = -1;
				} else {
					msgType = 1;
				}

				/*-- 發報 --*/
				OnMessageUpdate(new BeckhoffMessageEventArgs(msgType, title, content));
			}

			/*-- 寫入Log --*/
			CtStatus.Report(stt, title, content);
		}

		private void ExceptionHandle(Stat stt, Exception ex) {
			/*-- 寫入Log --*/
			string method = "";
			CtStatus.Report(stt, ex, out method);

			/*-- 如果要發報訊息事件 --*/
			if (EnableMessage) {
				/*-- 依照Stt分類 --*/
				sbyte msgType = 0;
				if ((int)stt == 0) {
					msgType = 0;
				} else if ((int)stt < 0) {
					msgType = -1;
				} else {
					msgType = 1;
				}

				/*-- 發報 --*/
				OnMessageUpdate(new BeckhoffMessageEventArgs(msgType, method, ex.Message));
			}
		}

		#region Symbol Process
		/// <summary>
		/// 取得符號數值
		/// <para>此方法「不可」應用於陣列(Array)或結構(Structure)上！</para>
		/// </summary>
		/// <param name="symName">欲取得之符號名稱</param>
		/// <returns>當前於Ads內之數值</returns>
		private object ReadSymbol(string symName) {
			object objTemp = null;
			/*-- 尋找並建立SymbolInfo --*/
			ITcAdsSymbol5 symInfo = mAdsClnt.ReadSymbolInfo(symName) as ITcAdsSymbol5;

			/*-- 如有找到該變數則取得其值 --*/
			if (symInfo != null) {
				/*-- 確認是否為陣列，如是陣列或結構不應使用此Function --*/
				if ((symInfo.DataTypeId != AdsDatatypeId.ADST_BIGTYPE) && (symInfo.DataTypeId != AdsDatatypeId.ADST_MAXTYPES) && (!symInfo.TypeName.Contains("ARRAY")))
					objTemp = mAdsClnt.ReadSymbol(symInfo);
				else
					throw (new Exception("變數" + symName + "應用函式錯誤，此方法無法支援陣列"));
			} else {
				throw (new Exception("變數" + symName + "存取失敗，請檢查名稱是否正確！"));
			}

			return objTemp;
		}

		/// <summary>
		/// 取得符號數值
		/// 此方法「不可」應用於陣列(Array)或結構(Structure)上！
		/// </summary>
		/// <param name="symName">欲取得之符號名稱</param>
		/// <param name="symType">欲取得之符號型態。如 typeof(int), typeof(bool) 等</param>
		/// <param name="symLength">如欲取得字串，請指定字串最大長度</param>
		/// <returns>當前於Ads內之數值</returns>
		private object ReadSymbol(string symName, Type symType, int symLength = 81) {
			object objTemp = null;
			/*-- 取得該變數的Handle --*/
			int varHdl = mAdsClnt.CreateVariableHandle(symName);

			/*-- 如果是字串則依照最大長度去抓取字串 --*/
			if (symType.Name == typeof(string).Name)
				objTemp = mAdsClnt.ReadAny(varHdl, symType, new int[] { symLength });

			/*-- 如非字串則直接抓取並回傳 --*/
			else
				objTemp = mAdsClnt.ReadAny(varHdl, symType);

			/*-- 取完後將之Handle刪掉 --*/
			mAdsClnt.DeleteVariableHandle(varHdl);

			return objTemp;
		}

		/// <summary>
		/// 取得符號數值
		/// 此方法可應用於陣列(Array)或結構(Structure)上。
		/// </summary>
		/// <param name="symInfo">欲取得之符號</param>
		/// <returns>AdsBinaryReader</returns>
		private AdsBinaryReader ReadSymbol(ITcAdsSymbol symInfo) {
			AdsStream dataStream = null;
			AdsBinaryReader bnrRead = null;

			/*-- 讀取Stream並建立BinaryReader --*/
			dataStream = new AdsStream(symInfo.Size);
			bnrRead = new AdsBinaryReader(dataStream);

			/*-- 取得該變數的Handle --*/
			int varHdl = mAdsClnt.CreateVariableHandle(symInfo.Name);
			mAdsClnt.Read(varHdl, dataStream);

			/*-- 取完後將之Handle刪掉 --*/
			mAdsClnt.DeleteVariableHandle(varHdl);

			return bnrRead;
		}

		/// <summary>取得變數(Symbol/Variable)之型態</summary>
		/// <param name="varName">欲取得之變數名稱</param>
		/// <returns>此符號類型，若沒有對應變數會以 <see cref="Exception"/> 方式跳出</returns>
		public SymbolType GetSymbolType(string varName) {
			SymbolType dataType = SymbolType.UNKNOWN;
			ITcAdsSymbol5 symInfo = mAdsClnt.ReadSymbolInfo(varName) as ITcAdsSymbol5;

			if (symInfo != null) {
				switch (symInfo.DataTypeId) {
					case AdsDatatypeId.ADST_BIGTYPE:
						dataType = SymbolType.ARRAY_STRUC;
						break;
					case AdsDatatypeId.ADST_BIT:
						dataType = SymbolType.BOOL;
						break;
					case AdsDatatypeId.ADST_INT16:
						dataType = SymbolType.INT;
						break;
					case AdsDatatypeId.ADST_INT32:
						dataType = SymbolType.DINT;
						break;
					case AdsDatatypeId.ADST_INT64:
						dataType = SymbolType.LINT;
						break;
					case AdsDatatypeId.ADST_INT8:
						dataType = SymbolType.SINT;
						break;
					case AdsDatatypeId.ADST_MAXTYPES:
						dataType = SymbolType.ARRAY_STRUC;
						break;
					case AdsDatatypeId.ADST_REAL32:
						dataType = SymbolType.REAL;
						break;
					case AdsDatatypeId.ADST_REAL64:
						dataType = SymbolType.LREAL;
						break;
					case AdsDatatypeId.ADST_STRING:
						dataType = SymbolType.STRING;
						break;
					case AdsDatatypeId.ADST_UINT16:
						dataType = SymbolType.UINT;
						break;
					case AdsDatatypeId.ADST_UINT32:
						dataType = SymbolType.UDINT;
						break;
					case AdsDatatypeId.ADST_UINT64:
						dataType = SymbolType.ULINT;
						break;
					case AdsDatatypeId.ADST_UINT8:
						dataType = SymbolType.USINT;
						break;
					default:
						dataType = SymbolType.UNKNOWN;
						throw (new Exception("變數" + varName + "為無法判別的" + Enum.GetName(typeof(AdsDatatypeId), symInfo.DataTypeId) + "資料型態"));
				}
			}

			return dataType;
		}

		/// <summary>回傳須補滿多少個記憶體才能為 4byte 的倍數。 如 19 回傳 1 (補至20)、13 回傳 3(補至16)</summary>
		/// <param name="typeList">欲計算的型態列表</param>
		/// <param name="strSize">如有 string，請輸入 string 代表的</param>
		/// <returns>尚須補滿的記憶體大小</returns>
		private int CalculateFillMemerySize(List<SymbolType> typeList, List<int> strSize) {
			int strIdx = 0;
			int temp = 0;
			foreach (SymbolType item in typeList) {
				switch (item) {
					case SymbolType.UNKNOWN:
						temp += 0;
						break;
					case SymbolType.BOOL:
						temp += 1;
						break;
					case SymbolType.STRING:
						temp += strSize[strIdx];
						if (strIdx < strSize.Count - 1) strIdx++;
						break;
					case SymbolType.SINT:
						temp += 1;
						break;
					case SymbolType.INT:
						temp += 2;
						break;
					case SymbolType.DINT:
						temp += 4;
						break;
					case SymbolType.LINT:
						temp += 8;
						break;
					case SymbolType.USINT:
						temp += 1;
						break;
					case SymbolType.UINT:
						temp += 2;
						break;
					case SymbolType.UDINT:
						temp += 4;
						break;
					case SymbolType.ULINT:
						temp += 8;
						break;
					case SymbolType.REAL:
						temp += 4;
						break;
					case SymbolType.LREAL:
						temp += 8;
						break;
					case SymbolType.TIME:
						temp += 2;
						break;
					default:
						throw (new Exception("無法計算的類型"));
				}
			}

			/*-- 往上補滿至 4byte 倍數所需數量 --*/
			if (temp % 4 != 0) temp = 4 - (temp % 4);
			else temp = 0;

			return temp;
		}
		#endregion

		#region Get Values

		///// <summary>
		///// 取得當前 Beckhoff PLC 內部單一變數資料
		///// <para>此方法僅能使用於「非」陣列(Array)/結構(Structure)之變數！</para>
		///// <para>或可用於特定索引(Index)之陣列元素，如 Variable[5]</para>
		///// </summary>
		///// <param name="varName">欲取得數值之變數名稱</param>
		///// <param name="value">回傳之當前數值</param>
		//public void GetValue(string varName, out object value) {
		//    value = ReadSymbol(varName);
		//}

		///// <summary>
		///// 取得當前 Beckhoff PLC 內部單一變數資料，並直接轉型為布林值(Boolean,bool)
		///// <para>此方法僅能使用於「非」陣列(Array)/結構(Structure)之變數！</para>
		///// <para>或可用於特定索引(Index)之陣列元素，如 Variable[5]</para>
		///// </summary>
		///// <param name="varName">欲取得數值之變數名稱</param>
		///// <param name="value">回傳之當前數值</param>
		//public void GetValue(string varName, out bool value) {
		//    value = CtConvert.CBool(ReadSymbol(varName));
		//}

		///// <summary>
		///// 取得當前 Beckhoff PLC 內部單一變數資料，並直接轉型為8位元帶符號整數(Int8,sbyte)
		///// <para>此方法僅能使用於「非」陣列(Array)/結構(Structure)之變數！</para>
		///// <para>或可用於特定索引(Index)之陣列元素，如 Variable[5]</para>
		///// </summary>
		///// <param name="varName">欲取得數值之變數名稱</param>
		///// <param name="value">回傳之當前數值</param>
		//public void GetValue(string varName, out sbyte value) {
		//    value = CtConvert.CSByte(ReadSymbol(varName));
		//}

		///// <summary>
		///// 取得當前 Beckhoff PLC 內部單一變數資料，並直接轉型為16位元帶符號整數(Int16,short)
		///// <para>此方法僅能使用於「非」陣列(Array)/結構(Structure)之變數！</para>
		///// <para>或可用於特定索引(Index)之陣列元素，如 Variable[5]</para>
		///// </summary>
		///// <param name="varName">欲取得數值之變數名稱</param>
		///// <param name="value">回傳之當前數值</param>
		//public void GetValue(string varName, out short value) {
		//    value = CtConvert.CShort(ReadSymbol(varName));
		//}

		///// <summary>
		///// 取得當前 Beckhoff PLC 內部單一變數資料，並直接轉型為32位元帶符號整數(Int32,int)
		///// <para>此方法僅能使用於「非」陣列(Array)/結構(Structure)之變數！</para>
		///// <para>或可用於特定索引(Index)之陣列元素，如 Variable[5]</para>
		///// </summary>
		///// <param name="varName">欲取得數值之變數名稱</param>
		///// <param name="value">回傳之當前數值</param>
		//public void GetValue(string varName, out int value) {
		//    value = CtConvert.CInt(ReadSymbol(varName));
		//}

		///// <summary>
		///// 取得當前 Beckhoff PLC 內部單一變數資料，並直接轉型為64位元帶符號整數(Int64,long)
		///// <para>此方法僅能使用於「非」陣列(Array)/結構(Structure)之變數！</para>
		///// <para>或可用於特定索引(Index)之陣列元素，如 Variable[5]</para>
		///// </summary>
		///// <param name="varName">欲取得數值之變數名稱</param>
		///// <param name="value">回傳之當前數值</param>
		//public void GetValue(string varName, out long value) {
		//    value = CtConvert.CLong(ReadSymbol(varName));
		//}

		///// <summary>
		///// 取得當前 Beckhoff PLC 內部單一變數資料，並直接轉型為8位元正整數(UInt8,byte)
		///// <para>此方法僅能使用於「非」陣列(Array)/結構(Structure)之變數！</para>
		///// <para>或可用於特定索引(Index)之陣列元素，如 Variable[5]</para>
		///// </summary>
		///// <param name="varName">欲取得數值之變數名稱</param>
		///// <param name="value">回傳之當前數值</param>
		//public void GetValue(string varName, out byte value) {
		//    value = CtConvert.CByte(ReadSymbol(varName));
		//}

		///// <summary>
		///// 取得當前 Beckhoff PLC 內部單一變數資料，並直接轉型為16位元正整數(UInt16,ushort)
		///// <para>此方法僅能使用於「非」陣列(Array)/結構(Structure)之變數！</para>
		///// <para>或可用於特定索引(Index)之陣列元素，如 Variable[5]</para>
		///// </summary>
		///// <param name="varName">欲取得數值之變數名稱</param>
		///// <param name="value">回傳之當前數值</param>
		//public void GetValue(string varName, out ushort value) {
		//    value = CtConvert.CUShort(ReadSymbol(varName));
		//}

		///// <summary>
		///// 取得當前 Beckhoff PLC 內部單一變數資料，並直接轉型為32位元正整數(UInt32,uint)
		///// <para>此方法僅能使用於「非」陣列(Array)/結構(Structure)之變數！</para>
		///// <para>或可用於特定索引(Index)之陣列元素，如 Variable[5]</para>
		///// </summary>
		///// <param name="varName">欲取得數值之變數名稱</param>
		///// <param name="value">回傳之當前數值</param>
		//public void GetValue(string varName, out uint value) {
		//    value = CtConvert.CUInt(ReadSymbol(varName));
		//}

		///// <summary>
		///// 取得當前 Beckhoff PLC 內部單一變數資料，並直接轉型為64位元正整數(UInt64,ulong)
		///// <para>此方法僅能使用於「非」陣列(Array)/結構(Structure)之變數！</para>
		///// <para>或可用於特定索引(Index)之陣列元素，如 Variable[5]</para>
		///// </summary>
		///// <param name="varName">欲取得數值之變數名稱</param>
		///// <param name="value">回傳之當前數值</param>
		//public void GetValue(string varName, out ulong value) {
		//    value = CtConvert.CULong(ReadSymbol(varName));
		//}

		///// <summary>
		///// 取得當前 Beckhoff PLC 內部單一變數資料，並直接轉型為32位元單精度浮點數(float,single)
		///// <para>此方法僅能使用於「非」陣列(Array)/結構(Structure)之變數！</para>
		///// <para>或可用於特定索引(Index)之陣列元素，如 Variable[5]</para>
		///// </summary>
		///// <param name="varName">欲取得數值之變數名稱</param>
		///// <param name="value">回傳之當前數值</param>
		//public void GetValue(string varName, out float value) {
		//    value = CtConvert.CFloat(ReadSymbol(varName));
		//}

		///// <summary>
		///// 取得當前 Beckhoff PLC 內部單一變數資料，並直接轉型為64位元倍精度浮點數(double)
		///// <para>此方法僅能使用於「非」陣列(Array)/結構(Structure)之變數！</para>
		///// <para>或可用於特定索引(Index)之陣列元素，如 Variable[5]</para>
		///// </summary>
		///// <param name="varName">欲取得數值之變數名稱</param>
		///// <param name="value">回傳之當前數值</param>
		//public void GetValue(string varName, out double value) {
		//    value = CtConvert.CDbl(ReadSymbol(varName));
		//}

		///// <summary>
		///// 取得當前 Beckhoff PLC 內部單一變數資料，並直接轉型為字串(String)
		///// <para>此方法僅能使用於「非」陣列(Array)/結構(Structure)之變數！</para>
		///// <para>或可用於特定索引(Index)之陣列元素，如 Variable[5]</para>
		///// </summary>
		///// <param name="varName">欲取得數值之變數名稱</param>
		///// <param name="value">回傳之當前數值</param>
		//public void GetValue(string varName, out string value) {
		//    value = ReadSymbol(varName).ToString();
		//}

		/// <summary>
		/// 取得當前 Beckhoff PLC 內部單一變數資料，並直接轉型為特定型態(以引數為主)
		/// <para>此方法僅能使用於「非」陣列(Array)/結構(Structure)之變數！</para>
		/// <para>或可用於特定索引(Index)之陣列元素，如 Variable[5]</para>
		/// </summary>
		/// <typeparam name="T">欲將收到的資料(object)轉換成特定的型態</typeparam>
		/// <param name="varName">欲取得數值之變數名稱</param>
		/// <param name="value">回傳之當前數值</param>
		public void GetValue<T>(string varName, out T value) {
			//value = (T)ReadSymbol(varName);
			value = (T)CtConvert.CType(ReadSymbol(varName), typeof(T));
		}

		/// <summary>
		/// 取得當前 Beckhoff PLC 內部單一變數資料，並直接轉型為字串(String)
		/// <para>此方法僅能使用於「非」陣列(Array)/結構(Structure)之變數！</para>
		/// <para>或可用於特定索引(Index)之陣列元素，如 Variable[5]</para>
		/// </summary>
		/// <param name="varName">欲取得數值之變數名稱</param>
		/// <param name="value">回傳之當前數值</param>
		/// <param name="strSize">PLC 內指定的 String 長度</param>
		public void GetValue(string varName, out string value, int strSize) {
			object objTemp = null;

			/*-- 尋找並建立SymbolInfo --*/
			ITcAdsSymbol5 symInfo = mAdsClnt.ReadSymbolInfo(varName) as ITcAdsSymbol5;

			int hdl = mAdsClnt.CreateVariableHandle(varName);

			/*-- 如有找到該變數則取得其值 --*/
			if (symInfo != null) {
				/*-- 確認是否為陣列，如是陣列或結構不應使用此Function --*/
				if ((symInfo.DataTypeId != AdsDatatypeId.ADST_BIGTYPE) && (symInfo.DataTypeId != AdsDatatypeId.ADST_MAXTYPES) && (!symInfo.TypeName.Contains("ARRAY")))
					objTemp = mAdsClnt.ReadAny(hdl, typeof(string), new int[] { strSize });
				else
					throw (new Exception("變數 " + varName + " 應用函式錯誤，此方法無法支援陣列"));
			} else {
				throw (new Exception("變數 " + varName + " 存取失敗，請檢查名稱是否正確！"));
			}

			mAdsClnt.DeleteVariableHandle(hdl);

			value = objTemp.ToString();
		}

		/// <summary>
		/// 取得當前 Beckhoff PLC 內部單一變數資料，並直接回傳
		/// 
		/// 此方法僅能使用於「非」陣列(Array)/結構(Structure)之變數！
		/// 或可用於特定索引(Index)之陣列元素，如 Variable[5]
		/// </summary>
		/// <param name="varName">欲取得數值之變數名稱</param>
		/// <returns>從 Ads 系統取得的對應此名稱之物件</returns>
		public object GetValue(string varName) {
			return ReadSymbol(varName);
		}

		/// <summary>取得當前 Beckhoff PLC 內部一陣列(Array)變數資料</summary>
		/// <param name="varName">欲取得數值之變數名稱</param>
		/// <param name="value">回傳之當前數值</param>
		public void GetValue(string varName, out List<object> value) {
			List<object> lstTemp = new List<object>();

			/*-- 取得SymbolInfo --*/
			ITcAdsSymbol5 symInfo = mAdsClnt.ReadSymbolInfo(varName) as ITcAdsSymbol5;

			/*-- 確認是否有取得正確資訊 --*/
			if (symInfo == null) throw (new Exception("無法查詢到 " + varName + "。請檢查變數名稱是否正確"));

			/*-- 抓取對應的Stream --*/
			AdsBinaryReader bnrRdr = ReadSymbol(symInfo);

			/*-- 依照不同種類的型態抓取資料 --*/
			do {
				switch (symInfo.DataTypeId) {
					case AdsDatatypeId.ADST_BIT:
						lstTemp.Add(bnrRdr.ReadBoolean());
						break;
					case AdsDatatypeId.ADST_INT16:
						lstTemp.Add(bnrRdr.ReadInt16());
						break;
					case AdsDatatypeId.ADST_INT32:
						lstTemp.Add(bnrRdr.ReadInt32());
						break;
					case AdsDatatypeId.ADST_INT64:
						lstTemp.Add(bnrRdr.ReadInt64());
						break;
					case AdsDatatypeId.ADST_INT8:
						lstTemp.Add(bnrRdr.ReadSByte());
						break;
					case AdsDatatypeId.ADST_REAL32:
						lstTemp.Add(bnrRdr.ReadSingle());
						break;
					case AdsDatatypeId.ADST_REAL64:
						lstTemp.Add(bnrRdr.ReadDouble());
						break;
					case AdsDatatypeId.ADST_STRING:
						string strTemp = bnrRdr.ReadString();
						if (strTemp != "")
							lstTemp.Add(strTemp);
						break;
					case AdsDatatypeId.ADST_UINT16:
						lstTemp.Add(bnrRdr.ReadUInt16());
						break;
					case AdsDatatypeId.ADST_UINT32:
						lstTemp.Add(bnrRdr.ReadUInt32());
						break;
					case AdsDatatypeId.ADST_UINT64:
						lstTemp.Add(bnrRdr.ReadUInt64());
						break;
					case AdsDatatypeId.ADST_UINT8:
						lstTemp.Add(bnrRdr.ReadByte());
						break;
					default:
						/* TIME 類型屬於 BigType類型，在此另外判斷 */
						if (symInfo.TypeName.ToUpper().Contains("TIME"))
							lstTemp.Add(bnrRdr.ReadInt32());    //TIME使用Int32讀取
						else
							throw (new Exception("變數" + varName + "為無法判別的" + Enum.GetName(typeof(AdsDatatypeId), symInfo.DataTypeId) + "資料型態，請檢查該型態並Debug"));

						break;
				}
			} while (bnrRdr.BaseStream.Position < symInfo.Size);

			/*-- 關閉元件 --*/
			bnrRdr.Close();
			bnrRdr.Dispose();

			value = lstTemp;
		}

		/// <summary>取得當前 Beckhoff PLC 內部一陣列(Array)變數資料</summary>
		/// <typeparam name="T">可對應 Ads 變數的型態。如 INT 對應 <see langword="short"/></typeparam>
		/// <param name="varName">欲取得數值之變數名稱</param>
		/// <param name="value">回傳之當前數值</param>
		public void GetValue<T>(string varName, out List<T> value) {
			List<T> lstTemp = new List<T>();

			/*-- 取得SymbolInfo --*/
			ITcAdsSymbol5 symInfo = mAdsClnt.ReadSymbolInfo(varName) as ITcAdsSymbol5;

			/*-- 確認是否有取得正確資訊 --*/
			if (symInfo == null) throw (new Exception("無法查詢到 " + varName + "。請檢查變數名稱是否正確"));

			/*-- 抓取對應的Stream --*/
			AdsBinaryReader bnrRdr = ReadSymbol(symInfo);

			/*-- 依照不同種類的型態抓取資料 --*/
			do {
				object obj;
				switch (symInfo.DataTypeId) {
					case AdsDatatypeId.ADST_BIT:
						obj = bnrRdr.ReadBoolean();
						break;
					case AdsDatatypeId.ADST_INT16:
						obj = bnrRdr.ReadInt16();
						break;
					case AdsDatatypeId.ADST_INT32:
						obj = bnrRdr.ReadInt32();
						break;
					case AdsDatatypeId.ADST_INT64:
						obj = bnrRdr.ReadInt64();
						break;
					case AdsDatatypeId.ADST_INT8:
						obj = bnrRdr.ReadSByte();
						break;
					case AdsDatatypeId.ADST_REAL32:
						obj = bnrRdr.ReadSingle();
						break;
					case AdsDatatypeId.ADST_REAL64:
						obj = bnrRdr.ReadDouble();
						break;
					case AdsDatatypeId.ADST_STRING:
						obj = bnrRdr.ReadString();
						break;
					case AdsDatatypeId.ADST_UINT16:
						obj = bnrRdr.ReadUInt16();
						break;
					case AdsDatatypeId.ADST_UINT32:
						obj = bnrRdr.ReadUInt32();
						break;
					case AdsDatatypeId.ADST_UINT64:
						obj = bnrRdr.ReadUInt64();
						break;
					case AdsDatatypeId.ADST_UINT8:
						obj = bnrRdr.ReadByte();
						break;
					default:
						/* TIME 類型屬於 BigType類型，在此另外判斷 */
						if (symInfo.TypeName.ToUpper().Contains("TIME"))
							obj = bnrRdr.ReadInt32();   //TIME使用Int32讀取
						else
							throw (new Exception("變數" + varName + "為無法判別的" + Enum.GetName(typeof(AdsDatatypeId), symInfo.DataTypeId) + "資料型態，請檢查該型態並Debug"));

						break;
				}
				lstTemp.Add((T)obj);
			} while (bnrRdr.BaseStream.Position < symInfo.Size);

			/*-- 關閉元件 --*/
			bnrRdr.Close();
			bnrRdr.Dispose();

			value = lstTemp;
		}

		/// <summary>取得當前 Beckhoff PLC 內部一結構(Structure)變數資料</summary>
		/// <param name="varName">欲取得數值之變數名稱</param>
		/// <param name="dataType">結構中的型態。如 test[0][0]:INT test[0][1]:BOOL test[0][3]:REAL，則帶入 List(Of SymbolType) typ = new List(Of SymbolType) {INT,BOOL,REAL}</param>
		/// <param name="value">取得回傳之當前數值</param>
		public void GetValue(string varName, List<SymbolType> dataType, out List<object> value) {
			List<object> lstTemp = new List<object>();

			if (dataType.Count > 0) {
				/*-- 取得SymbolInfo --*/
				ITcAdsSymbol symInfo = mAdsClnt.ReadSymbolInfo(varName);

				/*-- 確認是否有取得正確資訊 --*/
				if (symInfo == null) throw (new Exception("無法查詢到 " + varName + "。請檢查變數名稱是否正確"));

				/*-- 計算總共有多少記憶體位置 --*/
				int fillSize = CalculateFillMemerySize(dataType, new List<int> { 81 });

				/*-- 抓取對應的Stream --*/
				AdsBinaryReader bnrRdr = ReadSymbol(symInfo);

				do {
					for (int i = 0; i < dataType.Count; i++) {
						switch (dataType[i]) {
							case SymbolType.BOOL:
								lstTemp.Add(bnrRdr.ReadBoolean());
								break;
							case SymbolType.STRING:
								string strTemp = bnrRdr.ReadString();
								if (strTemp != "")
									lstTemp.Add(strTemp);
								break;
							case SymbolType.SINT:
								lstTemp.Add(bnrRdr.ReadSByte());
								break;
							case SymbolType.INT:
								lstTemp.Add(bnrRdr.ReadInt16());
								break;
							case SymbolType.DINT:
								lstTemp.Add(bnrRdr.ReadInt32());
								break;
							case SymbolType.LINT:
								lstTemp.Add(bnrRdr.ReadInt64());
								break;
							case SymbolType.USINT:
								lstTemp.Add(bnrRdr.ReadByte());
								break;
							case SymbolType.UINT:
								lstTemp.Add(bnrRdr.ReadUInt16());
								break;
							case SymbolType.UDINT:
								lstTemp.Add(bnrRdr.ReadUInt32());
								break;
							case SymbolType.ULINT:
								lstTemp.Add(bnrRdr.ReadUInt64());
								break;
							case SymbolType.REAL:
								lstTemp.Add(bnrRdr.ReadSingle());
								break;
							case SymbolType.LREAL:
								lstTemp.Add(bnrRdr.ReadDouble());
								break;
							case SymbolType.TIME:
								lstTemp.Add(bnrRdr.ReadInt32());
								break;
							default:
								throw (new Exception("變數" + varName + "為無法判別的" + Enum.GetName(typeof(SymbolType), dataType[i]) + "資料型態，請檢查該型態並Debug"));
						}
					}

					if (fillSize > 0) {
						for (int fill = 0; fill < fillSize; fill++) {
							bnrRdr.ReadByte();
						}
					}

				} while (bnrRdr.BaseStream.Position < symInfo.Size);

				/*-- 關閉元件 --*/
				bnrRdr.Close();
				bnrRdr.Dispose();
			}

			value = lstTemp;
		}

		/// <summary>將特定變數之資料串流撈取出來，以 List(Of byte) 方式回傳</summary>
		/// <param name="varName">欲讀取的變數名稱</param>
		/// <param name="stream">從 Ads System 讀取到的資料</param>
		public void GetValue(string varName, out List<byte> stream) {
			/*-- 取得SymbolInfo --*/
			ITcAdsSymbol symInfo = mAdsClnt.ReadSymbolInfo(varName);

			/*-- 確認是否有取得正確資訊 --*/
			if (symInfo == null) throw (new Exception("無法查詢到 " + varName + "。請檢查變數名稱是否正確"));

			/*-- 抓取對應的Stream --*/
			AdsBinaryReader bnrRdr = ReadSymbol(symInfo);

			/*-- 將 Stream 裡的 byte[] 抓出來 --*/
			byte[] bytTemp = new byte[bnrRdr.BaseStream.Length];
			int ret = bnrRdr.BaseStream.Read(bytTemp, 0, symInfo.Size);
			if (ret != 0) throw (new Exception("讀取到的 Stream Size 與 ITcAdsSymbol Size 不同"));

			stream = bytTemp.ToList();
		}

		/// <summary>
		/// 取得當前 Beckhoff PLC 內部一結構(Structure)變數資料，帶入型態並回傳
		/// <para>僅能用於讀取單一結構，無法用於讀取陣列結構！</para>
		/// </summary>
		/// <param name="varName">欲讀取的變數名稱</param>
		/// <param name="structType">符合 PLC STRUCT 之 Structure</param>
		/// <returns>回傳的 Structure，請自己轉換型態</returns>
		/// <example>
		/// 以下範例複製自 Beckhoff Information System
		/// <code language="C#">
		/// TYPE TSimpleStruct :
		/// STRUCT
		/// 	lrealVal: LREAL := 1.23;
		/// 	dintVal1: DINT := 120000;
		/// END_STRUCT
		/// END_TYPE
		/// 
		/// TYPE TComplexStruct :
		/// STRUCT
		/// 	intVal : INT:=1200;
		/// 	dintArr: ARRAY[0..3] OF DINT:= 1,2,3,4;
		/// 	boolVal: BOOL := FALSE;
		/// 	byteVal: BYTE:=10;
		/// 	stringVal : STRING(5) := 'hallo';
		/// 	simpleStruct1: TSimpleStruct;
		/// END_STRUCT
		/// END_TYPE
		/// </code>
		/// <code language="C#">
		/// [StructLayout(LayoutKind.Sequential, Pack=1)]   //Pack = (TwinCAT2) 1 , (TwinCAT3) 0
		/// public class SimpleStruct {
		///     public double lrealVal;
		///     public int dintVal1;
		/// }
		/// 
		/// [StructLayout(LayoutKind.Sequential, Pack=1)]   //Pack = (TwinCAT2) 1 , (TwinCAT3) 0
		/// public class ComplexStruct
		/// {
		///     public short intVal;
		///     //specifies how .NET should marshal the array
		///     //SizeConst specifies the number of elements the array has.
		///     [MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
		///     public int[] dintArr = new int[4];
		///     [MarshalAs(UnmanagedType.I1)]
		///     public bool boolVal;	public byte byteVal;
		///     //specifies how .NET should marshal the string
		///     //SizeConst specifies the number of characters the string has.
		///     //'(inclusive the terminating null ).
		///     [MarshalAs(UnmanagedType.ByValTStr, SizeConst=6)]
		///     public string stringVal = "";
		///     public SimpleStruct simpleStruct1 = new SimpleStruct();
		/// }
		/// </code>
		/// 於 CAMPro 之應用如下
		/// <code language="C#">
		/// CtBeckhoff mBkf;
		/// ComplexStruct structObj = (ComplexStruct)mBkf.GetValue("MAIN.ComplexStruct1", typeof(ComplexStruct));
		/// </code></example>
		public object GetValue(string varName, Type structType) {
			/*-- 讀取 Handle --*/
			int hdl = mAdsClnt.CreateVariableHandle(varName);

			/*-- 抓資料 --*/
			object obj = mAdsClnt.ReadAny(hdl, structType);

			/*-- 刪除 Handle --*/
			mAdsClnt.DeleteVariableHandle(hdl);

			return obj;
		}

		/// <summary>
		/// 取得當前 Beckhoff PLC 內部一陣列或結構(Structure)變數資料，帶入型態並回傳
		/// <para>可用於讀取陣列結構，請於 arrayCount 引數帶入陣列數量</para>
		/// </summary>
		/// <param name="varName">欲讀取的變數名稱</param>
		/// <param name="structType">符合 PLC STRUCT 之 Structure</param>
		/// <param name="arrayCount">陣列數量</param>
		/// <returns>回傳的 Structure，請自己轉換型態</returns>
		/// <example>
		/// 以下範例複製自 Beckhoff Information System
		/// <code language="C#">
		/// TYPE TSimpleStruct :
		/// STRUCT
		/// 	lrealVal: LREAL := 1.23;
		/// 	dintVal1: DINT := 120000;
		/// END_STRUCT
		/// END_TYPE
		/// 
		/// TYPE TComplexStruct :
		/// STRUCT
		/// 	intVal : INT:=1200;
		/// 	dintArr: ARRAY[0..3] OF DINT:= 1,2,3,4;
		/// 	boolVal: BOOL := FALSE;
		/// 	byteVal: BYTE:=10;
		/// 	stringVal : STRING(5) := 'hallo';
		/// 	simpleStruct1: TSimpleStruct;
		/// END_STRUCT
		/// END_TYPE
		/// 
		/// VAR PERSISTENT
		///     ComplexStructArray : ARRAY [1..10] OF TComplexStruct;
		/// END_VAR
		/// </code>
		/// <code language="C#">
		/// [StructLayout(LayoutKind.Sequential, Pack=1)]   //Pack = (TwinCAT2) 1 , (TwinCAT3) 0
		/// public class SimpleStruct {
		///     public double lrealVal;
		///     public int dintVal1;
		/// }
		/// 
		/// [StructLayout(LayoutKind.Sequential, Pack=1)]   //Pack = (TwinCAT2) 1 , (TwinCAT3) 0
		/// public class ComplexStruct
		/// {
		///     public short intVal;
		///     //specifies how .NET should marshal the array
		///     //SizeConst specifies the number of elements the array has.
		///     [MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
		///     public int[] dintArr = new int[4];
		///     [MarshalAs(UnmanagedType.I1)]
		///     public bool boolVal;	public byte byteVal;
		///     //specifies how .NET should marshal the string
		///     //SizeConst specifies the number of characters the string has.
		///     //'(inclusive the terminating null ).
		///     [MarshalAs(UnmanagedType.ByValTStr, SizeConst=6)]
		///     public string stringVal = "";
		///     public SimpleStruct simpleStruct1 = new SimpleStruct();
		/// }
		/// </code>
		/// 於 CAMPro 之應用如下
		/// <code language="C#">
		/// CtBeckhoff mBkf;
		/// ComplexStruct[] structObj = mBkf.GetValue("MAIN.ComplexStructArray", typeof(ComplexStruct[]), 10) as ComplexStruct[];
		/// </code></example>
		public object GetValue(string varName, Type structType, int arrayCount) {
			object obj = null;

			/*-- 讀取 Handle --*/
			int hdl = mAdsClnt.CreateVariableHandle(varName);

			try {
				/*-- 抓資料 --*/
				obj = mAdsClnt.ReadAny(hdl, structType, new int[] { arrayCount });
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}

			/*-- 刪除 Handle --*/
			mAdsClnt.DeleteVariableHandle(hdl);

			return obj;
		}
		#endregion

		#region Set Value

		/// <summary>寫入數值至指定的變數</summary>
		/// <param name="varName">欲寫入的變數名稱</param>
		/// <param name="value">欲寫入的數值</param>
		public void SetValue(string varName, object value) {

			/*-- 取得SymbolInfo --*/
			ITcAdsSymbol5 symInfo = mAdsClnt.ReadSymbolInfo(varName) as ITcAdsSymbol5;

			/*-- 確認是否有取得正確資訊 --*/
			if (symInfo == null) throw (new Exception("無法查詢到 " + varName + "。請檢查變數名稱是否正確"));

			/*-- 依照目標變數型態將value轉為相對應型態，以便到時寫入AdsClient --*/
			object objTemp = null;
			switch (symInfo.DataTypeId) {
				case AdsDatatypeId.ADST_BIT:
					objTemp = CtConvert.CBool(value);
					break;
				case AdsDatatypeId.ADST_INT16:
					objTemp = CtConvert.CShort(value);
					break;
				case AdsDatatypeId.ADST_INT32:
					objTemp = CtConvert.CInt(value);
					break;
				case AdsDatatypeId.ADST_INT64:
					objTemp = CtConvert.CLong(value);
					break;
				case AdsDatatypeId.ADST_INT8:
					objTemp = CtConvert.CSByte(value);
					break;
				case AdsDatatypeId.ADST_REAL32:
					objTemp = CtConvert.CFloat(value);
					break;
				case AdsDatatypeId.ADST_REAL64:
					objTemp = CtConvert.CDbl(value);
					break;
				case AdsDatatypeId.ADST_STRING:
					objTemp = CtConvert.CStr(value);
					break;
				case AdsDatatypeId.ADST_UINT16:
					objTemp = CtConvert.CUShort(value);
					break;
				case AdsDatatypeId.ADST_UINT32:
					objTemp = CtConvert.CUInt(value);
					break;
				case AdsDatatypeId.ADST_UINT64:
					objTemp = CtConvert.CULong(value);
					break;
				case AdsDatatypeId.ADST_UINT8:
					objTemp = CtConvert.CByte(value);
					break;
				default:
					throw (new Exception("變數" + varName + "為目前不支援的" + Enum.GetName(typeof(AdsDatatypeId), symInfo.DataTypeId) + "資料型態"));
			}

			/*-- 利用AdsClient寫入PLC --*/
			mAdsClnt.WriteSymbol(symInfo, objTemp);
		}

		#endregion

		#region Notifications

		/// <summary>查詢特定變數是否在DeviceNotification中</summary>
		/// <param name="varName">變數名稱</param>
		/// <returns>是否存在於監控名單內</returns>
		public bool IsMonitoring(string varName) {
			bool bolTemp = false;
			foreach (KeyValuePair<string, int> pair in mNotifyHdl) {
				if (pair.Key == varName) {
					bolTemp = true;
					break;
				}
			}
			return bolTemp;
		}

		/// <summary>註冊DeviceNotification，並指定AdsClient的AdsNotificationEx事件</summary>
		/// <param name="varName">變數名稱</param>
		/// <param name="dataType">該變數於Beckhoff PLC內部資料型態</param>
		private void RegisterNotification(string varName, Type dataType) {
			/*-- 透過AdsClient註冊變數，並取回Handle值 --*/
			int varHdl = mAdsClnt.AddDeviceNotificationEx(
							varName,
							AdsTransMode.OnChange,
							BECKHOFF_NOTIFY_CYCLE,
							BECKHOFF_NOTIFY_DELAY,
							varName,
							dataType
						);

			/*-- 將取回的Handle值丟到List裡，以方便存取 --*/
			mNotifyHdl.Add(varName, varHdl);

			/*-- 如果尚未註冊事件則註冊之 --*/
			if (!mNotified) {
				mAdsClnt.AdsNotificationEx += mAdsClnt_AdsNotificationEx;
				mNotified = true;
			}
		}

		/// <summary>註冊變數至Beckhoff的Nitification，該變數有變化時將會拋出SymbolChanged事件</summary>
		/// <param name="varName">欲註冊之變數名稱</param>
		/// <param name="dataType">該變數於Beckhoff PLC之內部型態</param>
		public void AddNotification(string varName, SymbolType dataType) {

			/*-- 建立Type物件以存放SymbolType相對應的型態 --*/
			Type typTemp = null;

			/*-- 依照SymbolType去轉換型態 --*/
			switch (dataType) {
				case SymbolType.BOOL:
					typTemp = typeof(bool);
					break;
				case SymbolType.STRING:
					typTemp = typeof(string);
					break;
				case SymbolType.SINT:
					typTemp = typeof(sbyte);
					break;
				case SymbolType.INT:
					typTemp = typeof(short);
					break;
				case SymbolType.DINT:
					typTemp = typeof(int);
					break;
				case SymbolType.LINT:
					typTemp = typeof(long);
					break;
				case SymbolType.USINT:
					typTemp = typeof(byte);
					break;
				case SymbolType.UINT:
					typTemp = typeof(ushort);
					break;
				case SymbolType.UDINT:
					typTemp = typeof(uint);
					break;
				case SymbolType.ULINT:
					typTemp = typeof(ulong);
					break;
				case SymbolType.REAL:
					typTemp = typeof(float);
					break;
				case SymbolType.LREAL:
					typTemp = typeof(double);
					break;
				case SymbolType.TIME:
					typTemp = typeof(int);
					break;
				default:
					throw (new Exception("變數" + varName + "為無法採用的變數型態 " + Enum.GetName(typeof(SymbolType), dataType)));
			}

			/*-- 透過AdsClient註冊變數 --*/
			RegisterNotification(varName, typTemp);
		}

		/// <summary>註冊變數至 Beckhoff 的 DeviceNotificationEx，該變數有變化時將會拋出 SymbolChanged 事件</summary>
		/// <param name="varName">欲註冊之變數名稱</param>
		/// <param name="dataType">該變數於Beckhoff PLC之內部型態，請自行轉為相對應於Beckhoff PLC之Windows型態</param>
		public void AddNotification(string varName, Type dataType) {
			/*-- 透過AdsClient註冊變數 --*/
			RegisterNotification(varName, dataType);
		}

		/// <summary>註冊變數至Beckhoff的Nitification，該變數有變化時將會拋出SymbolChanged事件</summary>
		/// <param name="varName">欲註冊之變數名稱</param>
		public void AddNotification(string varName) {
			SymbolType dataType = GetSymbolType(varName);

			if (dataType != SymbolType.UNKNOWN) {
				/*-- 透過AdsClient註冊變數 --*/
				AddNotification(varName, dataType);
			}
		}

		/// <summary>註銷於Beckhoff PLC內部之Notification Handle</summary>
		/// <param name="varName">欲註銷之變數名稱</param>
		public void RemoveNotification(string varName) {

			/*-- 準備獲取Handle之值，預設為int之最小值-2147483648 --*/
			int varHdl = int.MinValue;

			/*-- 搜尋Dictionary裡之對應變數名稱之Handle，並將之Assign出來 --*/
			foreach (KeyValuePair<string, int> temp in mNotifyHdl) {
				if (temp.Key == varName) {
					varHdl = temp.Value;
					break;
				}
			}

			/*-- 如果沒有找到，表示變數名稱有錯，發報Alarm --*/
			if (varHdl == int.MinValue) throw (new Exception("錯誤的變數名稱" + varName + "，請檢查並Debug"));

			/*-- 如果有找到Handle，透過AdsClient將之刪除 --*/
			mAdsClnt.DeleteDeviceNotification(varHdl);

			/*-- 如果到這裡都沒有跳Exception，表示順利刪除，將Dictionary裡之資料刪除 --*/
			mNotifyHdl.Remove(varName);

			/*-- 如果都沒有Notification了，移除事件 --*/
			if (mNotifyHdl.Count < 1) {
				mNotified = false;
				mAdsClnt.AdsNotificationEx -= mAdsClnt_AdsNotificationEx;
			}
		}

		#endregion

		#region Status

		/// <summary>
		/// 取得當前Beckhoff Ads狀態
		/// <para>即系統當前執行狀態</para>
		/// </summary>
		/// <returns>當前狀態</returns>
		public AdsStatus GetAdsStatus() {
			AdsStatus plcStt = AdsStatus.Run;
			StateInfo sttInfo = mAdsClnt.ReadState();
			plcStt = (AdsStatus)sttInfo.AdsState;
			return plcStt;
		}

		/// <summary>
		/// 取得當前Beckhoff Ads狀態
		/// <para>即系統當前執行狀態</para>
		/// </summary>
		/// <param name="adsStt">當前狀態</param>
		public void GetAdsStatus(out AdsStatus adsStt) {
			AdsStatus plcStt = AdsStatus.Run;
			StateInfo sttInfo = mAdsClnt.ReadState();
			plcStt = (AdsStatus)sttInfo.AdsState;
			adsStt = plcStt;
		}

		/// <summary>
		/// 取得當前Beckhoff Ads狀態
		/// <para>即系統當前執行狀態</para>
		/// </summary>
		/// <returns>當前狀態</returns>
		public DeviceStatus GetDeviceStatus() {
			DeviceStatus devStt = DeviceStatus.OP;
			StateInfo sttInfo = mAdsClnt.ReadState();
			devStt = (DeviceStatus)sttInfo.DeviceState;
			return devStt;
		}

		/// <summary>
		/// 取得當前Beckhoff Ads狀態
		/// <para>即系統當前執行狀態</para>
		/// </summary>
		/// <param name="devStt">當前狀態</param>
		public void GetDeviceStatus(out DeviceStatus devStt) {
			DeviceStatus devTemp = DeviceStatus.OP;
			StateInfo sttInfo = mAdsClnt.ReadState();
			devTemp = (DeviceStatus)sttInfo.DeviceState;
			devStt = devTemp;
		}

		/// <summary>設定Beckhoff Ads狀態</summary>
		/// <param name="adsStt">預變更之狀態</param>
		public void SetAdsStatus(AdsStatus adsStt) {
			StateInfo sttInfo = new StateInfo((AdsState)adsStt, mAdsClnt.ReadState().DeviceState);
			mAdsClnt.WriteControl(sttInfo);
		}

		/// <summary>
		/// 取得當前Beckhoff Ads狀態
		/// <para>即系統當前執行狀態</para>
		/// </summary>
		/// <param name="devStt">當前狀態</param>
		public void SetDeviceStatus(DeviceStatus devStt) {
			StateInfo sttInfo = new StateInfo(AdsState.Stop, (short)15);
			mAdsClnt.WriteControl(sttInfo);
		}
		#endregion

		#endregion

		#region Function - Events

		/// <summary>接收到DeviceNotification事件，將接收到的參數發報出去</summary>
		/// <param name="sender">Ads物件</param>
		/// <param name="e">AdsNotificationEx參數</param>
		void mAdsClnt_AdsNotificationEx(object sender, AdsNotificationExEventArgs e) {
			OnSymbolEventChanged(new BeckhoffSymbolEventArgs(e.UserData.ToString(), e.Value));
		}

		#endregion

	}
}
