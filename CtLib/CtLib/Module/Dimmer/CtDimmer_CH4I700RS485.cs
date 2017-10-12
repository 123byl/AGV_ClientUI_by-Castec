using System;
using System.Collections.Generic;
using System.Linq;

using CtLib.Library;
using CtLib.Module.SerialPort;
using CtLib.Module.Utility;

namespace CtLib.Module.Dimmer {

	/// <summary>調光器模組，荃達(弘積) 4 通道、RS-485</summary>
	public class CtDimmer_CH4I700RS485 : ICtDimmer, ICtVersion {

		#region Version

		/// <summary>CtDimmer_CH4I700RS485 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0	Ahern	[2016/07/01]
		///     + 建立基礎模組
		///     
		/// 1.0.1	Ahern	[2016/07/02]
		///		+ Send Delay 因應傳輸速度較慢所導致的資料較慢
		///		\ 已上機測試完成
		///		
		/// 1.0.2	Ahern	[2016/09/26]
		///		\ SetLight(IEnumerable(Of int)) 補上延遲，不然有機會沒切到
		///     
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(1, 0, 2, "2016/09/26", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Serial Port Default Values
		/// <summary>預設鮑率(BaudRate)</summary>
		private static readonly int DEFAULT_BAUDRATE = 19200;
		/// <summary>預設交握方式</summary>
		private static readonly CtSerial.Handshake DEFAULT_HANDSHAKE = CtSerial.Handshake.None;
		/// <summary>預設同位元</summary>
		private static readonly CtSerial.Parity DEFAULT_PARITY = CtSerial.Parity.None;
		/// <summary>預設停止位元</summary>
		private static readonly CtSerial.StopBits DEFAULT_STOPBIT = CtSerial.StopBits.One;
		/// <summary>預設資料位元</summary>
		private static readonly int DEFAULT_DATABIT = 8;
		#endregion

		#region Declaration - Decode Abstract Class

		/// <summary>提供解碼訊息的抽象類別</summary>
		private abstract class DimmerData {
			/// <summary>此解碼的狀態碼</summary>
			public Stat Status { get; protected set; }
			/// <summary>訊息建立時間</summary>
			public DateTime Time { get; protected set; }
			/// <summary>收到的原始資料</summary>
			protected List<byte> ReceviedData;
		}

		/// <summary>讀取單一頻道數值之回傳訊息</summary>
		private class SingleLight : DimmerData {
			public Channels Channel { get; private set; }
			/// <summary>當前數值</summary>
			public int Light { get; private set; }
			/// <summary>以回傳訊息建構此類別並儲存數值</summary>
			/// <param name="data">回傳訊息</param>
			public SingleLight(List<byte> data) {
				Time = DateTime.Now;
				ReceviedData = data.ToList();
				if (data != null && data.Count == 8) {
					/* 03 06 55 AA 01 00 C8 D1 */
					Channel = (Channels)(data[4] - 1);
					Light = data[5] * 256 + data[6];
					Status = Stat.SUCCESS;
				} else Status = Stat.ER_COM_RCIV;
			}
		}

		/// <summary>讀取溫度之回傳訊息</summary>
		private class TemperatureData : DimmerData {
			/// <summary>當前調光器溫度</summary>
			public int? Temperature { get; private set; }
			/// <summary>以回傳訊息建構此類別並儲存數值</summary>
			/// <param name="data">回傳訊息</param>
			public TemperatureData(List<byte> data) {
				Time = DateTime.Now;
				ReceviedData = data.ToList();
				if (data != null && data.Count == 7) {
					Temperature = data[5];      //e.g. { 0x87 0xAA 0x55 0xDB 00 "TT" Chk }
					Status = Stat.SUCCESS;
				} else Status = Stat.ER_COM_RCIV;
			}
		}

		/// <summary>讀取調光器版本之回傳訊息</summary>
		private class VersionData : DimmerData {
			/// <summary>調光器版本</summary>
			public string Version { get; private set; }
			/// <summary>以回傳訊息建構此類別並儲存數值</summary>
			/// <param name="data">回傳訊息</param>
			public VersionData(List<byte> data) {
				Time = DateTime.Now;
				ReceviedData = data.ToList();
				if (data != null && data.Count > 0) {
					Version = "";
					for (byte idx = 2; idx < data.Count; idx++) {
						Version += CtConvert.ASCII(data[idx]);
					}
					Status = Stat.SUCCESS;
				} else Status = Stat.ER_COM_RCIV;
			}
		}

		/// <summary>讀取調光器版本之回傳訊息</summary>
		private class LockData : DimmerData {
			/// <summary>調光器版本</summary>
			public bool Lock { get; private set; }
			/// <summary>以回傳訊息建構此類別並儲存數值</summary>
			/// <param name="data">回傳訊息</param>
			public LockData(List<byte> data) {
				Time = DateTime.Now;
				ReceviedData = data.ToList();
				if (data != null && data.Count > 0) {
					if (data[5] == 1 && data[6] == 0) Lock = true;
					else if (data[5] == 0 && data[6] == 0) Lock = true;
					else throw new ArgumentException("無法解析的命令");
					Status = Stat.SUCCESS;
				} else Status = Stat.ER_COM_RCIV;
			}
		}

		/// <summary>收到不可解析或訊息錯誤的回傳訊息</summary>
		private class Error : DimmerData {
			/// <summary>建構錯誤訊息，並記錄原始資料</summary>
			/// <param name="data">原始回傳訊息</param>
			public Error(List<byte> data) {
				Time = DateTime.Now;
				ReceviedData = data.ToList();
				Status = Stat.ER_COM_RCIV;
			}
		}
		#endregion

		#region Declaration - Fields
		/// <summary>串列埠控制。不啟用事件接收資料，以手動方式接收</summary>
		private CtSerial mSerial = new CtSerial(false, TransDataFormats.EnumerableOfByte);
        /// <summary>傳送指令後的等待時間</summary>
        /// <remarks>因有裝 RS485 轉 RS232 轉換器，故通訊會有 Delay，如太快收會導致資料不完全</remarks>
        private int mSendDelay = 1;
		#endregion

		#region Declaration - Properties
		/// <summary>取得目前串列埠(SerialPort)開啟狀態。 (<see langword="true"/>)開啟 (<see langword="false"/>)關閉</summary>
		public bool IsConnected { get { return mSerial.IsOpen; } }
		#endregion

		#region Function - Constructors
		/// <summary>建構調光器模組</summary>
		public CtDimmer_CH4I700RS485() {

		}
		#endregion

		#region Function - Disposable
		/// <summary>中斷與調光器之連線，並釋放相關資源</summary>
		public void Dispose() {
			try {
				Dispose(true);
				GC.SuppressFinalize(this);
			} catch (ObjectDisposedException ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
		}

		/// <summary>中斷與調光器之連線，並釋放相關資源</summary>
		/// <param name="isDisposing">是否為第一次釋放</param>
		protected virtual void Dispose(bool isDisposing) {
			try {
				if (isDisposing) mSerial.Close();
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
		}
		#endregion

		#region Function - Methods
		/// <summary>處理接收到的資料</summary>
		/// <param name="rxData">透過 RS-232 接收到的資料</param>
		private DimmerData Decode(List<byte> rxData) {
			DimmerData dimmerData = null;
			try {
				/* 以功能碼做為區別 */
				switch (rxData[0]) {
					case 0x01:
						dimmerData = new VersionData(rxData);
						break;
					case 0x03:
						dimmerData = new SingleLight(rxData);
						break;
					case 0x05:
						dimmerData = new LockData(rxData);
						break;
				}
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_COM_RCIV, ex);
				dimmerData = new Error(rxData);
			}

			return dimmerData;
		}

		/// <summary>Calculate checksum and return a list of fully data</summary>
		/// <param name="buf">Byte value that want to calculate. e.g. "0xFF, 0x98, 0xAE, 0xDA"</param>
		/// <returns>List of byte, the data that calculated</returns>
		private List<byte> CheckSum(params byte[] buf) {
			List<byte> chkSum = buf.ToList();
			byte chkValue = 0;
			chkSum.ForEach(val => chkValue += val); //Using Linq to do operate.
			chkSum.Add(chkValue);
			return chkSum;
		}

		/// <summary>Check the input data's checksum is pass or not</summary>
		/// <param name="buf">Byte value that want to check. e.g. "0xFF, 0x98, 0xAE, 0xDA"</param>
		/// <returns>(<see langword="true"/>)Pass (<see langword="false"/>)Invalid</returns>
		private bool CheckSum(List<byte> buf) {
			byte chkValue = 0;
			buf.Take(buf.Count - 1).ForEach(val => chkValue += val); //Using Linq to do operate.
			return chkValue == buf.Last();
		}

		/// <summary>透過 RS-232 傳送資料，並等待其回傳訊息</summary>
		/// <param name="data">欲傳送的資料</param>
		/// <returns>Status Code</returns>
		private Stat SendData(List<byte> data) {
			Stat stt = Stat.SUCCESS;
			try {
				mSerial.ClearBuffer();
				mSerial.Send(data);     //Writing data to serial port buffer
				CtTimer.Delay(mSendDelay);//Delay. If with fast delay, it may without change of dimmer. (Buffer failed ??)

			} catch (Exception ex) {
				if (stt == Stat.SUCCESS) stt = Stat.ER_COM_SEND;
				CtStatus.Report(Stat.ER_COM_SEND, ex);
			}
			return stt;
		}

		/// <summary>透過 RS-232 傳送資料，並等待其回傳訊息</summary>
		/// <param name="data">欲傳送的資料</param>
		/// <param name="rxData">接收到的資料</param>
		/// <returns>Status Code</returns>
		private Stat SendData(List<byte> data, out DimmerData rxData) {
			Stat stt = Stat.SUCCESS;
			DimmerData dimmerData = null;
			try {
				mSerial.ClearBuffer();
				mSerial.Send(data);     //Writing data to serial port buffer
				CtTimer.Delay(mSendDelay);//Delay. If with fast delay, it may without change of dimmer. (Buffer failed ??)

				if (!WaitResponse(out dimmerData)) stt = Stat.ER_COM_RTMO;  //Waiting response from dimmer

			} catch (Exception ex) {
				if (stt == Stat.SUCCESS) stt = Stat.ER_COM_SEND;
				CtStatus.Report(Stat.ER_COM_SEND, ex);
			}
			rxData = dimmerData;
			return stt;
		}

		/// <summary>接收 Buffer 裡的資料並分析之</summary>
		/// <param name="rxData">接收到的資料</param>
		/// <returns>(<see langword="true"/>)成功收到可解析資料 (<see langword="false"/>)無回應或不可解析之資料</returns>
		private bool WaitResponse(out DimmerData rxData) {
			bool result = false;
			List<byte> comData;

			mSerial.Receive(out comData);               //手動從 Buffer 接收資料
														//Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.ff] ") + string.Join(", ", comData));
			DimmerData dimmerData = Decode(comData);    //解碼

			result = (dimmerData is Error) ? false : true;
			rxData = dimmerData;

			return result;
		}
		#endregion

		#region Function - Connections
		/// <summary>以使用者介面方式供人員手動連線</summary>
		/// <returns>Status Code</returns>
		public Stat Connect() {
			Stat stt = Stat.SUCCESS;
			try {
				mSerial.Open();
			} catch (Exception ex) {
				if (stt == Stat.SUCCESS) stt = Stat.ER_COM_OPEN;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		/// <summary>已指定的串列埠編號及預設參數進行連線</summary>
		/// <param name="comPort">串列埠編號，如 "COM3"</param>
		/// <returns>Status Code</returns>
		public Stat Connect(string comPort) {
			Stat stt = Stat.SUCCESS;
			try {
				mSerial.Open(comPort, DEFAULT_BAUDRATE, DEFAULT_DATABIT, DEFAULT_STOPBIT, DEFAULT_PARITY, DEFAULT_HANDSHAKE);
			} catch (Exception ex) {
				if (stt == Stat.SUCCESS) stt = Stat.ER_COM_OPEN;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		/// <summary>中斷連線</summary>
		/// <returns>Status Code</returns>
		public Stat Disconnect() {
			Stat stt = Stat.SUCCESS;
			try {
				mSerial.Close();
			} catch (Exception ex) {
				if (stt == Stat.SUCCESS) stt = Stat.ER_COM_OPEN;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}
		#endregion

		#region Function - Lights
		/// <summary>取得當前特定組別之電流數值</summary>
		/// <param name="channel">調光器組別(通道、頻道)</param>
		/// <param name="value">當前數值</param>
		/// <returns>Status Code</returns>
		public Stat GetLight(Channels channel, out LightValue value) {
			LightValue lightValue = null;

			List<byte> data = CheckSum(0x03, 0x06, 0x55, 0xAA, (byte)(channel + 1), 0, 0);

			mSendDelay = 16;
			DimmerData rxData;
			SingleLight sngLight;
			Stat stt = SendData(data, out rxData);
			sngLight = rxData as SingleLight;
			if (stt == Stat.SUCCESS && sngLight != null)
				lightValue = new LightValue(sngLight.Channel, Modes.Constant, sngLight.Light);

			value = lightValue;
			return stt;
		}

		/// <summary>取得當前所有的電流數值</summary>
		/// <param name="value">當前四組電流數值</param>
		/// <returns>Status Code</returns>
		public Stat GetLight(out List<LightValue> value) {
			Stat stt = Stat.SUCCESS;
			List<byte> data = new List<byte>();
			DimmerData rxData;
			SingleLight sngLight;
			List<LightValue> lights = new List<LightValue>();
			mSendDelay = 20;

			for (byte idx = 1; idx < 5; idx++) {
				data.Clear();
				data = CheckSum(0x03, 0x06, 0x55, 0xAA, idx, 0, 0);

				stt = SendData(data, out rxData);
				sngLight = rxData as SingleLight;
				if (stt == Stat.SUCCESS && sngLight != null)
					lights.Add(new LightValue(sngLight.Channel, Modes.Constant, sngLight.Light));
				else break;
			}

			value = lights;
			return stt;
		}

		/// <summary>設定全部電流數值為相同數值</summary>
		/// <param name="lightValue">電流數值，單位為 毫安培(mA)</param>
		/// <returns>(<see langword="true"/>)設定成功 (<see langword="false"/>)設定失敗</returns>
		public bool SetLight(int lightValue) {
			byte M = (byte)((lightValue / 256) << 4);   //It's two parts of this byte. 0x(value MSB)(channel mode). Default "mode" is (0)constant.
			byte C = (byte)(lightValue % 256);          //The LSB of value

			List<byte> data = CheckSum(0x02, 0x06, 0x55, 0xAA, 0x00, M, C); //As manual, calculate checksum and retuen a list of byte
																			//Trace.WriteLine(string.Join(", ", data));	//Shows byte data if necessary
			mSendDelay = 8;
			Stat stt = SendData(data);

			return stt == Stat.SUCCESS;
		}

		/// <summary>設定單一組別之電流數值</summary>
		/// <param name="channel">調光器組別(通道、頻道)</param>
		/// <param name="lightValue">電流數值，單位為 毫安培(mA)</param>
		/// <returns>(<see langword="true"/>)設定成功 (<see langword="false"/>)設定失敗</returns>
		public bool SetLight(Channels channel, int lightValue) {
			if (lightValue <= -1) return false;

			byte M = (byte)((lightValue / 256) << 4);   //It's two parts of this byte. 0x(value MSB)(channel mode). Default "mode" is (0)constant.
			byte C = (byte)(lightValue % 256);          //The LSB of value

			List<byte> data = CheckSum(0x02, 0x06, 0x55, 0xAA, (byte)(channel + 1), M, C);  //As manual, calculate checksum and retuen a list of byte
																							//Trace.WriteLine(string.Join(", ", data));	//Shows byte data if necessary
			mSendDelay = 8;
			DimmerData rxData;
			SingleLight sngLight;
			Stat stt = SendData(data, out rxData);
			sngLight = rxData as SingleLight;

			return stt == Stat.SUCCESS && sngLight != null;
		}

		/// <summary>設定所有組別之電流數值</summary>
		/// <param name="ch1Value">第 1 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch2Value">第 2 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch3Value">第 3 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch4Value">第 4 組數值，單位為 毫安培(mA)</param>
		/// <returns>(<see langword="true"/>)設定成功 (<see langword="false"/>)設定失敗</returns>
		public bool SetLight(int ch1Value, int ch2Value, int ch3Value, int ch4Value) {
			bool result = true;
			int[] chValue = new int[] { ch1Value, ch2Value, ch3Value, ch4Value };
			mSendDelay = 8;
			for (byte idx = 0; idx < 4; idx++) {
				result = SetLight((Channels)idx, chValue[idx]);
				CtTimer.Delay(10);
			}
			return result;
		}

		/// <summary>設定所有組別之電流數值，第 5 通道為使用獨立設定</summary>
		/// <param name="ch1Value">第 1 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch2Value">第 2 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch3Value">第 3 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch4Value">第 4 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch5Value">第 5 組數值，單位為 毫安培(mA)</param>
		/// <returns>(<see langword="true"/>)設定成功 (<see langword="false"/>)設定失敗</returns>
		public bool SetLight(int ch1Value, int ch2Value, int ch3Value, int ch4Value, int ch5Value) {
			throw new NotSupportedException("4CH-DI700-RS485 不支援此命令");
		}

		/// <summary>設定所有組別之電流數值</summary>
		/// <param name="values">數值集合</param>
		/// <returns>(<see langword="true"/>)設定成功 (<see langword="false"/>)設定失敗</returns>
		public bool SetLight(IEnumerable<int> values) {
			int count = values.Count();
			if (values.All(val => val > -1)) {
				if (count == 2) {
					return SetLight(values.ElementAt(0), values.ElementAt(1), 0, 0);
				} else if (count == 4) {
					return SetLight(values.ElementAt(0), values.ElementAt(1), values.ElementAt(2), values.ElementAt(3));
				} else if (count == 5) {
					return SetLight(values.ElementAt(0), values.ElementAt(1), values.ElementAt(2), values.ElementAt(3), values.ElementAt(4));
				} else return false;
			} else {
				bool ret = true;
				for (int idx = 0; idx < count; idx++) {
					if (values.ElementAt(idx) > -1) {
						ret &= SetLight((Channels)idx, values.ElementAt(idx));
						CtTimer.Delay(10);
					}
				}
				return ret;
			}
		}
		#endregion

		#region Function - Periphery
		/// <summary>取得調光器版本訊息</summary>
		/// <param name="version">版本字串</param>
		/// <returns>Status Code</returns>
		public Stat GetDimmerVersion(out string version) {
			string ver = "";

			List<byte> data = CheckSum(0x01, 0x06, 0x55, 0xAA, 0x01, 0x00, 0x00);

			mSendDelay = 10;
			DimmerData rxData;
			VersionData verData;
			Stat stt = SendData(data, out rxData);
			verData = rxData as VersionData;
			if (stt == Stat.SUCCESS && verData != null)
				ver = verData.Version;

			version = ver;
			return stt;
		}

		/// <summary>鎖定特定組別，使其無法使用實體旋鈕進行更改</summary>
		/// <param name="channel">欲鎖定的組別(通道、頻道)</param>
		/// <param name="lockStt">(<see langword="true"/>)鎖定  (<see langword="false"/>)解鎖</param>
		/// <returns>(<see langword="true"/>)設定成功  (<see langword="false"/>)設定失敗</returns>
		public bool Lock(Channels channel, bool lockStt) {
			byte lockVal = (byte)(lockStt ? 0x01 : 0x00);
			List<byte> data = CheckSum(0x04, 0x06, 0x55, 0xAA, (byte)(channel + 1), lockVal, 0x00);
			
			mSendDelay = 20;
			DimmerData rxData;
			Stat stt = SendData(data, out rxData);

			return stt == Stat.SUCCESS && rxData is LockData;
		}

		/// <summary>鎖定所有組別，使其無法使用實體旋鈕進行更改</summary>
		/// <param name="lockStt">(<see langword="true"/>)鎖定  (<see langword="false"/>)解鎖</param>
		/// <returns>(<see langword="true"/>)設定成功  (<see langword="false"/>)設定失敗</returns>
		public bool LockAll(bool lockStt) {
			Stat stt = Stat.SUCCESS;
			byte lockVal = (byte)(lockStt ? 0x01 : 0x00);
			mSendDelay = 20;

			for (byte idx = 1; idx < 5; idx++) {
				List<byte> data = CheckSum(0x04, 0x06, 0x55, 0xAA, idx, lockVal, 0x00);

				DimmerData rxData;
				stt = SendData(data, out rxData);
				if (stt != Stat.SUCCESS || !(rxData is LockData)) {
					stt = Stat.ER_COM_RCIV;
					break;
				}
				CtTimer.Delay(10);
			}

			return stt == Stat.SUCCESS;
		}

		/// <summary>取得特定組別之鎖定狀態</summary>
		/// <param name="channel">欲取得鎖定的組別(通道、頻道)</param>
		/// <param name="state">此通道當前鎖定狀態 (<see langword="true"/>)鎖定  (<see langword="false"/>)解鎖</param>
		/// <returns>(<see langword="true"/>)取得成功  (<see langword="false"/>)取得失敗</returns>
		public bool Lock(Channels channel, out bool state) {
			List<byte> data = CheckSum(0x05, 0x06, 0x55, 0xAA, (byte)(channel + 1), 0x00, 0x00);

			mSendDelay = 20;
			DimmerData rxData;
			LockData lockData;
			Stat stt = SendData(data, out rxData);
			lockData = rxData as LockData;
			if (stt == Stat.SUCCESS && lockData != null) {
				state = lockData.Lock;
				return true;
			} else {
				state = false;
				return false;
			}
		}

		/// <summary>取得特定組別之鎖定狀態</summary>
		/// <param name="state">此通道當前鎖定狀態 (<see langword="true"/>)鎖定  (<see langword="false"/>)解鎖</param>
		/// <returns>(<see langword="true"/>)取得成功  (<see langword="false"/>)取得失敗</returns>
		public bool Lock(out List<bool> state) {
			List<bool> result = new List<bool>();

			mSendDelay = 20;
			DimmerData rxData;
			LockData lockData;
			Stat stt = Stat.SUCCESS;
			for (byte idx = 1; idx < 5; idx++) {
				List<byte> data = CheckSum(0x05, 0x06, 0x55, 0xAA, idx, 0x00, 0x00);

				stt = SendData(data, out rxData);
				lockData = rxData as LockData;
				if (stt == Stat.SUCCESS && lockData != null) {
					result.Add(lockData.Lock);
				} else {
					break;
				}
			}

			state = result;
			return stt == Stat.SUCCESS;
		}
		#endregion
	}
}
