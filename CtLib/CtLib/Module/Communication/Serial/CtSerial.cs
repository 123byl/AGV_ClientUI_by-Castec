﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

using CtLib.Forms;
using CtLib.Library;
using CtLib.Module.Utility;

namespace CtLib.Module.SerialPort {
	/// <summary>
	/// 串列埠(<see cref="SerialPort"/>)相關控制
	/// <para>可應用於 RS-232、RS-485、USB、UART 等串列通訊</para>
	/// <para>如是以 STRING 為主，目前採用 ASCII 加解碼，故請勿傳送 Unicode 之資料！</para>
	/// </summary>
	/// <example>
	/// 以下為連線方法
	/// <code language="C#">
	/// CtSerial mSerial = new CtSerial();                  //建立 CtSerial，使用 string 做為資料傳送格式(不含結尾符號)，並使用事件將收到的資料拋出來
	/// mSerial.OnSerialEvents += mSerial_OnSerialEvents;
	/// 
	/// mSerial.Open();                                                                     //此方法會開啟視窗並由使用者設定所需參數
	/// //mSerial.Open("COM1", 115200, 8, StopBits.One, Parity.None, Handshake.None, 1000); //使用參數直接開啟 SerialPort
	/// 
	/// mSerial.Send("Hello", EndChar.CrLf);                            //送出 "Hello\r\n" 字串至目標裝置. 請依情況改變 EndChar
	/// mSerial.Send(new List&lt;byte&gt; { 0x01, 0x01, 0x0A, 0xF1 });  /送出以 Byte 為主的資料, 如果有結尾符號請「自行」加入至 List 中 (一般來說，Byte 傳送格式很少須結尾符號)
	/// </code>
	/// 如果不想透過事件接收資料，想要由程式自行控制，須於建構時設定(不開放其他時機是怕事件亂掉而崩潰)，如以下步驟
	/// <code language="C#">
	/// CtSerial mSerial = new CtSerial(false);     //設定不透過事件接收資料
	/// //... do operations
	/// 
	/// string returnedValue;
	/// mSerial.Receive(out returnedValue);         //於需要接收時執行此方法即可，亦可用 List&lt;byte&gt; 以接收 byte 資料
	/// </code>
	/// 如需設定自訂義結尾符號，以下為基本步驟
	/// <code language="C#">
	/// mSerial.EndOfLineSymbol = EndChar.Custom;   //設定使用自定義結尾
	/// mSerial.CustomEndOfLine = "*";              //自訂義的結尾符號為 "*"
	/// 
	/// string temp;
	/// mSerial.Receive(out temp);  //如果目標傳送 "Hello*", 你則會收到 "Hello" (不含結尾符號 "*")!!
	/// </code>
	/// </example>
	public class CtSerial : IDisposable, ICtVersion {

		#region Version

		/// <summary>CtSerial 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 0.0.0  Chi Sha [2007/12/10]
		///     + CtSerial
		///     
		/// 1.0.0  Ahern [2014/11/19]
		///     + Translate from VB
		///     
		/// 1.0.1  Ahern [2014/12/15]
		///     \ 註解全面轉回繁體中文，避免詞不達意
		///     
		/// 1.0.2  Ahern [2015/02/17]
		///     \ mCom 改為 protected 以讓外部繼承
		///     \ 部分 Function 加上 virtual 修飾詞，以讓外部 Override
		///     
		/// 1.0.3  Ahern [2015/03/15]
		///     + EndOfLineSymbol
		///     + CustomEndOfLine
		///     
		/// 1.0.4  Ahern [2015/04/30]
		///     \ DataFormat 改為 private set
		///     
		/// 1.0.5  Ahern [2016/11/29]
		///		\ 修改兩個 Receive 的 ReadByte() 部分，添加延遲與採用 do-while 先收再判斷 Buffer
		///		+ ReadTimeout 與 WriteTimeout
		///     
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(1, 0, 5, "2016/11/29", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Events

		/// <summary>CtSerial 串列埠事件</summary>
		public event EventHandler<SerialEventArgs> OnSerialEvents;

		/// <summary>觸發串列埠事件</summary>
		/// <param name="e">欲發佈之串列埠事件參數</param>
		protected virtual void RaiseEvents(SerialEventArgs e) {
			if (EnableReceiveEvent) {
				EventHandler<SerialEventArgs> handler = OnSerialEvents;
				if (handler != null)
					handler(this, e);
			}
		}
		#endregion

		#region Declaration - Disposable
		/// <summary>中斷與 SerialPort 之連線，並釋放相關資源</summary>
		public void Dispose() {
			try {
				Dispose(true);
				GC.SuppressFinalize(this);
			} catch (ObjectDisposedException ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
		}

		/// <summary>中斷與 SerialPort 之連線，並釋放相關資源</summary>
		/// <param name="isDisposing">是否為第一次釋放</param>
		protected virtual void Dispose(bool isDisposing) {
			try {
				if (isDisposing) Close();
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
		}
		#endregion

		#region Declaration - Enumeration
		/// <summary>串列埠(SerialPort)之同位檢查位元(<see cref="System.IO.Ports.Parity"/>)</summary>
		public enum Parity : byte {
			/// <summary>無同位檢查位元(不啟用)</summary>
			None = 0,
			/// <summary>保持位元集之計數總和為奇數</summary>
			Odd = 1,
			/// <summary>保持位元集之計數總和為偶數</summary>
			Even = 2,
			/// <summary>同位檢查位元永遠送1</summary>
			Mark = 3,
			/// <summary>同位檢查位元永遠送0</summary>
			Space = 4
		}

		/// <summary>串列埠(SerialPort)之停止位元(<see cref="System.IO.Ports.StopBits"/>)</summary>
		public enum StopBits : byte {
			/// <summary>不使用停止位元</summary>
			None = 0,
			/// <summary>使用一個停止位元</summary>
			One = 1,
			/// <summary>使用兩個停止位元</summary>
			Two = 2,
			/// <summary>使用1.5個停止位元</summary>
			OnePointFive = 3
		}

		/// <summary>串列埠(SerialPort)之交握控制協議(<see cref="System.IO.Ports.Handshake"/>)</summary>
		public enum Handshake : byte {
			/// <summary>不使用任何交握控制</summary>
			None = 0,
			/// <summary>使用軟體控制。XON開始傳送資料，XOFF停止傳輸</summary>
			XON_XOFF = 1,
			/// <summary>簡稱RTS，即使用硬體流量控制(Flow Control)。 True 表示緩衝區未滿可傳輸資料，反之則不可傳輸</summary>
			RequestToSend = 2,
			/// <summary>同時啟用RTS與XON/XOFF</summary>
			RequestToSend_XON_XOFF = 3
		}
		#endregion

		#region Declaration - Properties

		/// <summary>取得目前串列埠(SerialPort)開啟狀態。 (<see langword="true"/>)開啟 (<see langword="false"/>)關閉</summary>
		public bool IsOpen {
			get {
				return (mCom != null) ? mCom.IsOpen : false;
			}
		}

		/// <summary>
		/// 取得是否啟用事件來接收資料
		/// <para>為避免出問題，此部分請於 CtSerial 建構時帶入</para>
		/// </summary>
		/// <remarks>如果想要自行觸發 Receive 方法來接收資料，請將此屬性設為 false</remarks>
		public bool EnableReceiveEvent { get; private set; }

		/// <summary>取得此串列埠之資料格式</summary>
		public TransDataFormats DataFormat { get; protected set; }

		/// <summary>取得當前已開啟之串列埠名稱。如尚未開啟則回傳 "N/A"</summary>
		public string PortName {
			get {
				string strName = "N/A";
				if (mCom != null) {
					if (mCom.IsOpen) strName = mCom.PortName;
				}
				return strName;
			}
		}

		/// <summary>取得當前已開啟之串列埠停止位元設定。如尚未開啟則回傳 NONE</summary>
		public StopBits StopBit {
			get {
				StopBits sb = StopBits.None;
				if (mCom != null) {
					if (mCom.IsOpen) sb = (StopBits)mCom.StopBits;
				}
				return sb;
			}
		}

		/// <summary>取得當前已開啟之串列埠同位檢查位元設定。如尚未開啟則回傳 NONE</summary>
		public Parity ParityCheck {
			get {
				Parity pa = Parity.None;
				if (mCom != null) {
					if (mCom.IsOpen) pa = (Parity)mCom.Parity;
				}
				return pa;
			}
		}

		/// <summary>取得當前已開啟之串列埠交握控制協議。如尚未開啟則回傳 NONE</summary>
		public Handshake HandShake {
			get {
				Handshake hsk = Handshake.None;
				if (mCom != null) {
					if (mCom.IsOpen) hsk = (Handshake)mCom.Handshake;
				}
				return hsk;
			}
		}

		/// <summary>取得當前已開啟之串列埠資料位元。如尚未開啟則回傳 -1</summary>
		public int DataBits {
			get {
				int bits = -1;
				if (mCom != null) {
					if (mCom.IsOpen) bits = mCom.DataBits;
				}
				return bits;
			}
		}

		/// <summary>取得或設定讀寫資料時之結尾符號</summary>
		public EndChar EndOfLineSymbol { get; set; }

		/// <summary>取得或設定自訂義之結尾符號</summary>
		/// <example><code language="C#">
		/// mSerial.EndOfLineSymbol = EndChar.Custom;   //設定使用自定義結尾
		/// mSerial.CustomEndOfLine = "]";              //自訂義的結尾符號為 "]"
		/// 
		/// string temp;
		/// mSerial.Receive(out temp);  //If target send "[ABC]", you will receive "[ABC" (without last end char)!!
		/// </code></example>
		public string CustomEndOfLine {
			get {
				if (DataFormat == TransDataFormats.String) return mEOLStr;
				else return Encoding.ASCII.GetString(mEOLByte);
			}
			set {
				mEOLStr = value;
				mEOLByte = Encoding.ASCII.GetBytes(value);
			}
		}

		/// <summary>取得或設定是否訂閱發布傳送資料之事件。如訂閱，將於傳送資料後發布相關事件</summary>
		public bool SubscribeSendEvent { get; set; } = false;

		/// <summary>取得或設定讀取作業未完成時，發生逾時之前的毫秒數</summary>
		public int ReadTimeout {
			get { return mCom?.ReadTimeout ?? -1; }
			set { if (mCom != null) mCom.ReadTimeout = value; }
		}

		/// <summary>取得或設定寫入作業未完成時，發生逾時之前的毫秒數</summary>
		public int WriteTimeout {
			get { return mCom?.WriteTimeout ?? -1; }
			set { if (mCom != null) mCom.WriteTimeout = value; }
		}
		#endregion

		#region Declaration - Fields
		/// <summary>SerialPort 物件，用於控制串列埠</summary>
		protected System.IO.Ports.SerialPort mCom;

		/// <summary>結尾符號字串</summary>
		/// <remarks>搭配 EndOfLineSymbol == EndChar.Custom</remarks>
		protected string mEOLStr = "";
		/// <summary>相對應結尾符號之byte</summary>
		/// <remarks>搭配 EndOfLineSymbol == EndChar.Custom</remarks>
		protected byte[] mEOLByte;
		#endregion

		#region Function - Constructor

		/// <summary>
		/// 建立 CtSerial 物件，並會自動發報接收到資料等事件
		/// <para>另請自行設定相關 Port、BaudRate 等等</para>
		/// </summary>
		/// <param name="dataType">此串列埠所傳輸的資料格式</param>
		public CtSerial(TransDataFormats dataType) {
			EnableReceiveEvent = true;
			DataFormat = dataType;
			EndOfLineSymbol = EndChar.None;
		}

		/// <summary>
		/// 建立 CtSerial 物件，但可自行決定是否發報接收到資料等事件與結尾符號
		/// <para>另請自行設定相關 Port、BaudRate 等等</para>
		/// </summary>
		/// <param name="enableEvent">
		/// 是否發報接收資料事件通知?
		/// <para>(<see langword="true"/>)開啟，由事件發報接收到的資料</para>
		/// <para>(<see langword="false"/>)關閉此功能，手動透過 <see cref="Receive(out List{byte})"/> 進行接收</para>
		/// </param>
		/// <param name="dataType">此串列埠所傳輸的資料格式</param>
		/// <param name="endOfLineSymbol">讀取或寫入資料時之結尾符號</param>
		public CtSerial(bool enableEvent = true, TransDataFormats dataType = TransDataFormats.String, EndChar endOfLineSymbol = EndChar.None) {
			EnableReceiveEvent = enableEvent;
			DataFormat = dataType;
			EndOfLineSymbol = endOfLineSymbol;
		}

		/// <summary>建立 CtSerial 物件，並「直接」設定、開啟串列埠</summary>
		/// <param name="portName">串列埠名稱(Port Name), e.g. "COM1"</param>
		/// <param name="baudRate">鮑率(BaudRate)，每秒所傳輸的位元(bits/s), e.g. 9600</param>
		/// <param name="dataBits">資料位元數(Data Bits), 預設為 8 位元</param>
		/// <param name="stopBit">停止位元</param>
		/// <param name="parity">同位檢查位元</param>
		/// <param name="handshake">交握協議</param>
		/// <param name="timeout">讀取/寫入逾時時間</param>
		/// <param name="enableEvent">
		/// 是否發報接收資料事件通知?
		/// <para>(<see langword="true"/>)開啟，由事件發報接收到的資料</para>
		/// <para>(<see langword="false"/>)關閉此功能，手動透過 <see cref="Receive(out List{byte})"/> 進行接收</para>
		/// </param>
		/// <param name="dataType">此串列埠所傳輸的資料格式</param>
		/// <param name="endOfLineSymbol">讀取或寫入資料時之結尾符號</param>
		public CtSerial(string portName,
						int baudRate = 9600,
						int dataBits = 8,
						StopBits stopBit = StopBits.One,
						Parity parity = Parity.None,
						Handshake handshake = Handshake.None,
						int timeout = 1000,
						bool enableEvent = true,
						TransDataFormats dataType = TransDataFormats.String,
						EndChar endOfLineSymbol = EndChar.None) {
			EnableReceiveEvent = enableEvent;
			DataFormat = dataType;
			Open(portName, baudRate, dataBits, stopBit, parity, handshake, timeout);
			EndOfLineSymbol = endOfLineSymbol;
		}
		#endregion

		#region Function - Methods
		/// <summary>搜尋此電腦上之所有已裝設之串列埠，並回傳名稱集合</summary>
		/// <returns>名稱集合，格式如 "COM1"</returns>
		public static List<string> GetPortNames() {
			List<string> strPort = new List<string>();
			try {
				strPort = System.IO.Ports.SerialPort.GetPortNames().ToList();
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
			return strPort.ToList();
		}

		/// <summary>Byte陣列中是否含有目前設定的結尾符號</summary>
		/// <param name="data">欲檢查的資料</param>
		/// <returns>(<see langword="true"/>)含有結尾符號  (<see langword="false"/>)不含結尾符號</returns>
		private bool IsContainEndOfLine(List<byte> data) {
			bool result = false;

			if (EndOfLineSymbol == EndChar.CrLf) result = data.Contains(0x0D) & data.Contains(0x0A);
			else if (EndOfLineSymbol == EndChar.Cr) result = data.Contains(0x0D);
			else if (EndOfLineSymbol == EndChar.Lf) result = data.Contains(0x0A);
			else if (EndOfLineSymbol == EndChar.Custom) {
				result = true;
				foreach (byte item in mEOLByte) {
					if (!data.Contains(item)) {
						result = false;
						break;
					}
				}
			}

			return result;
		}
		#endregion

		#region Function - Core

		/// <summary>開啟串列埠</summary>
		/// <param name="portName">串列埠名稱, e.g."COM1"</param>
		/// <param name="baudRate">鮑率(BaudRate)，即每秒傳輸速度(bits/s), e.g. 9600</param>
		/// <param name="dataBits">資料位元數(Data Bits), 一般為 8 位元</param>
		/// <param name="stopBit">停止位元(StopBits)</param>
		/// <param name="parity">同位檢查位元(Parity)</param>
		/// <param name="handshake">交握控制(Handshake)</param>
		/// <param name="timeout">讀取/寫入之逾時時間(Timeout)</param>
		public virtual void Open(string portName,
						 int baudRate = 9600,
						 int dataBits = 8,
						 StopBits stopBit = StopBits.One,
						 Parity parity = Parity.None,
						 Handshake handshake = Handshake.None,
						 int timeout = 1000) {

			/*-- Check the com name is correct ? --*/
			int intPort = -1;
			string strPort = portName.ToUpper();
			if (!strPort.Contains("COM") || (!int.TryParse(strPort.Replace("COM", ""), out intPort))) {
				throw (new Exception("Invalid com name"));
			}

			/*-- If mCom ever been used, dispose it --*/
			if (mCom != null) {
				if (mCom.IsOpen) mCom.Close();
			} else mCom = new System.IO.Ports.SerialPort();

			/*-- Setting Enviroments --*/
			mCom.PortName = strPort;
			mCom.BaudRate = baudRate;
			mCom.DataBits = dataBits;
			mCom.Parity = (System.IO.Ports.Parity)parity;
			mCom.Handshake = (System.IO.Ports.Handshake)handshake;
			mCom.StopBits = (System.IO.Ports.StopBits)stopBit;
			mCom.ReadTimeout = timeout;
			mCom.WriteTimeout = timeout;

			/*-- Open the com port --*/
			mCom.Open();

			mCom.DiscardInBuffer();
			mCom.DiscardOutBuffer();

			/*-- If open successfully, raise an event --*/
			if (mCom.IsOpen)
				RaiseEvents(new SerialEventArgs(SerialPortEvents.Connection, true));

			/*-- If enable events, add events --*/
			if (EnableReceiveEvent) mCom.DataReceived += mCom_DataReceived;
		}

		/// <summary>開啟串列埠，並由介面選擇相關設定</summary>
		public virtual void Open() {

			/*-- If mCom ever been used, dispose it --*/
			if (mCom != null) {
				if (mCom.IsOpen) mCom.Close();
			} else mCom = new System.IO.Ports.SerialPort();

			CtSerialSetup setup = new CtSerialSetup();
			setup.Start(ref mCom);

			mCom.DiscardInBuffer();
			mCom.DiscardOutBuffer();

			/*-- If open successfully, raise an event --*/
			if (mCom.IsOpen)
				RaiseEvents(new SerialEventArgs(SerialPortEvents.Connection, true));

			setup.Dispose();

			/*-- If enable events, add events --*/
			if (EnableReceiveEvent) mCom.DataReceived += mCom_DataReceived;
		}

		/// <summary>關閉串列埠</summary>
		public void Close() {
			if (mCom != null) {
				if (mCom.IsOpen) mCom.Close();

				if (!mCom.IsOpen) {
					RaiseEvents(new SerialEventArgs(SerialPortEvents.Connection, false));
				}

				if (EnableReceiveEvent) mCom.DataReceived -= mCom_DataReceived;
			}
		}

		/// <summary>傳送資料至目標緩衝區 (資料格式為 string)</summary>
		/// <param name="data">欲傳送之資料(string)</param>
		/// <param name="endOfLine">是否自動在結尾添加換行符號("\r\n")? 如對應格式無須結尾，或其他如 "\n"，請設為 false 並自行添加!!</param>
		public virtual void Send(string data, EndChar endOfLine = EndChar.None) {

			/*-- Check mCom state --*/
			if (!mCom.IsOpen) throw (new Exception("Please open serial port first"));

			/*-- If it need new line, add it! --*/
			string strData = data;
			switch (endOfLine) {
				case EndChar.Cr:
					strData += "\r";
					break;
				case EndChar.Lf:
					strData += "\n";
					break;
				case EndChar.CrLf:
					strData += "\r\n";
					break;
				case EndChar.Custom:
					strData += mEOLStr;
					break;
			}
			/*-- Convert string to byte[] with each char --*/
			//byte[] temp = new byte[strData.Length];
			//for (int idx = 0; idx < strData.Length; idx++) {
			//    temp[idx] = Convert.ToByte(Convert.ToInt16(strData[idx]));
			//}

			byte[] temp = Encoding.ASCII.GetBytes(strData);

			/*-- Write the byte[] into buffer --*/
			mCom.Write(temp, 0, temp.Length);

			if (SubscribeSendEvent) {
				if (DataFormat == TransDataFormats.String)
					RaiseEvents(new SerialEventArgs(SerialPortEvents.DataSend, strData));
				else
					RaiseEvents(new SerialEventArgs(SerialPortEvents.DataSend, temp));
			}
		}

		/// <summary>從緩衝區收取資料，將之轉為 string 格式並回傳</summary>
		/// <param name="data">已收取的資料</param>
		public virtual void Receive(out string data) {

			string strTemp = "";

			if (EndOfLineSymbol == EndChar.None) {
				/*-- Using a byte to receive data from buffer --*/
				/*-- 'Cause GetString function need byte[], so create a byte[] but length = 1 --*/
				//byte[] bytTemp = new byte[1];
				//do {
				//    bytTemp[0] = (byte)mCom.ReadByte();             //Read a byte from starter of buffer stream, it will sub BytesToRead with 1
				//    strTemp += Encoding.ASCII.GetString(bytTemp); //Convert the byte that received to string with Default encoding
				//} while (mCom.BytesToRead > 0);     //Do until there are nothing in buffer

				List<byte> lstTemp = new List<byte>();
				do {
					lstTemp.Add((byte)mCom.ReadByte());
					CtTimer.Delay(TimeSpan.FromMilliseconds(0.5));
				} while (mCom.BytesToRead > 0);

				strTemp = Encoding.ASCII.GetString(lstTemp.ToArray());
			} else if (EndOfLineSymbol == EndChar.Custom) {
				strTemp = mCom.ReadTo(CustomEndOfLine);
			} else {
				strTemp = mCom.ReadTo(CtConst.END_OF_CHAR[EndOfLineSymbol]);
			}

			data = strTemp;
		}

		/// <summary>傳送資料至目標緩衝區 (格式為 List&lt;byte&gt;)</summary>
		/// <param name="data">Byte 資料集合</param>
		public virtual void Send(List<byte> data) {

			/*-- Check mCom state --*/
			if (!mCom.IsOpen) throw (new Exception("Please open serial port first"));

			/*-- Convert string to byte[] with each char --*/
			byte[] temp = data.ToArray();

			/*-- Write the byte[] into buffer --*/
			mCom.Write(temp, 0, temp.Length);

			if (SubscribeSendEvent) {
				if (DataFormat == TransDataFormats.String)
					RaiseEvents(new SerialEventArgs(SerialPortEvents.DataSend, Encoding.ASCII.GetString(temp)));
				else
					RaiseEvents(new SerialEventArgs(SerialPortEvents.DataSend, temp));
			}
		}

		/// <summary>從緩衝區收取資料，並直接回傳收到的 byte 集合</summary>
		/// <param name="data">已收取之 byte 資料集合</param>
		public virtual void Receive(out List<byte> data) {
			List<byte> bytTemp = new List<byte>();

			if (EndOfLineSymbol == EndChar.None) {

				/*-- Read each byte from start to end of stream --*/
				do {	//Do until there are nothing in buffer
					bytTemp.Add((byte)mCom.ReadByte());
					CtTimer.Delay(TimeSpan.FromMilliseconds(0.5));
				} while (mCom.BytesToRead > 0);

			} else {
				//目前先保留 Stopwatch 監控時間之用，如果有遇到一直收不到結尾的再打開讓他跳 Timeout
				//Stopwatch sw = new Stopwatch();
				//sw.Start();
				do {
					byte temp = (byte)mCom.ReadByte();
					bytTemp.Add(temp);
					if (IsContainEndOfLine(bytTemp)) break;
				} while (true /*&& sw.ElapsedMilliseconds < mCom.ReadTimeout*/);
				//sw.Stop();
				//if (sw.ElapsedMilliseconds >= mCom.ReadTimeout) throw (new TimeoutException);
			}

			data = bytTemp.ToList();
		}

		/// <summary>清空串列埠之輸入/輸出緩衝區</summary>
		public virtual void ClearBuffer() {
			mCom.DiscardInBuffer();
			mCom.DiscardOutBuffer();
		}
		#endregion

		#region Function - Events
		/// <summary>SerialPort 觸發之資料收取事件</summary>
		protected virtual void mCom_DataReceived(object sender, SerialDataReceivedEventArgs e) {
			try {
				/*-- If enable events, do the operations --*/
				if (EnableReceiveEvent) {
					/*- Status Code --*/
					Stat stt = Stat.SUCCESS;

					CtTimer.Delay(1);

					/*-- If the data type is string --*/
					if (DataFormat == TransDataFormats.String) {
						string strTemp = "";
						Receive(out strTemp);

						if ((stt == Stat.SUCCESS) && (strTemp != "")) {
							/*-- Raise an event --*/
							RaiseEvents(new SerialEventArgs(SerialPortEvents.DataReceived, strTemp));
						}

						/*-- If the data type is byte[] --*/
					} else {
						List<byte> bytTemp;
						Receive(out bytTemp);
						if ((stt == Stat.SUCCESS) && (bytTemp != null)) {
							/*-- Raise an event --*/
							RaiseEvents(new SerialEventArgs(SerialPortEvents.DataReceived, bytTemp));
						}
					}
				}
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
		}
		#endregion
	}
}
