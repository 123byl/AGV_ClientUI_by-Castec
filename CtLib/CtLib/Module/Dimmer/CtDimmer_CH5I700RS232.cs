using System;
using System.Collections.Generic;
using System.Linq;

using CtLib.Library;
using CtLib.Module.SerialPort;
using CtLib.Module.Utility;

namespace CtLib.Module.Dimmer {

	/// <summary>調光器模組，荃達(弘積) 5 通道、RS-232</summary>
	public class CtDimmer_CH5I700RS232 : ICtDimmer, ICtVersion {

		#region Version

		/// <summary>CtDimmer 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 0.0.0  Ahern [2015/10/21]
		///     + 建立基礎模組
		///     
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(0, 0, 0, "2015/10/21", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Serial Port Default Values
		/// <summary>預設鮑率(BaudRate)</summary>
		private static readonly int DEFAULT_BAUDRATE = 9600;
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

		/// <summary>設定數值之回傳訊息</summary>
		private class SetPoint : DimmerData {
			/// <summary>設定成功與否</summary>
			public bool? Result { get; private set; }
			/// <summary>以回傳訊息建構此類別並儲存數值</summary>
			/// <param name="data">回傳訊息</param>
			public SetPoint(List<byte> data) {
				Time = DateTime.Now;
				ReceviedData = data.ToList();
				if (data != null) {
					if (data.Count == 6) {  //數量6，表示是全部設定或是重置
						Result = data[4] == 0x01 ? true : false;    //e.g. { 0x86 0xAA 0x55 0xDD "0x01" Chk }
						Status = Stat.SUCCESS;
					} else if (data.Count == 7) {   //數量7，表示為單一通道之設定回傳
						Result = data[5] == 0x01 ? true : false;    //e.g. { 0x87 0xAA 0x55 0xDC CH "0x01" Chk }
						Status = Stat.SUCCESS;
					} else Status = Stat.ER_COM_RCIV;
				} else Status = Stat.ER_COM_RCIV;
			}
		}

		/// <summary>讀取單一頻道數值之回傳訊息</summary>
		private class SingleLight : DimmerData {
			/// <summary>當前數值</summary>
			public LightValue Light { get; private set; }
			/// <summary>以回傳訊息建構此類別並儲存數值</summary>
			/// <param name="data">回傳訊息</param>
			public SingleLight(List<byte> data) {
				Time = DateTime.Now;
				ReceviedData = data.ToList();
				if (data != null && data.Count == 8) {
					Light = new LightValue(data.GetRange(4, 4));    //e.g. { 0x89 0xAA 0x55 0xDE CH M C P Chk }
					Status = Stat.SUCCESS;
				} else Status = Stat.ER_COM_RCIV;
			}
		}

		/// <summary>讀取所有頻道數值之回傳訊息</summary>
		private class AllLights : DimmerData {
			/// <summary>數值</summary>
			public List<LightValue> LightValue { get; private set; }
			/// <summary>以回傳訊息建構此類別並儲存數值</summary>
			/// <param name="data">回傳訊息</param>
			public AllLights(List<byte> data) {
				Time = DateTime.Now;
				ReceviedData = data.ToList();
				if (data != null && data.Count == 17) {
					Channels ch = Channels.Channel1;
					for (byte idx = 4; idx < 16; idx += 3) {
						LightValue lightValue = new LightValue(data.GetRange(idx, 3));  //直接將片段丟進去建構，並用內建的解碼直接解析
						lightValue.Channel = ch++;      //讀取全部數值並不會帶有 CH 資料，故須自行寫入
						LightValue.Add(lightValue);
					}
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
					for (byte idx = 4; idx <= data[1] + 1; idx++) {
						Version += CtConvert.ASCII(data[idx]);
					}
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
		private CtSerial mSerial = new CtSerial(false, TransDataFormats.String);
		#endregion

		#region Declaration - Properties
		/// <summary>取得目前串列埠(SerialPort)開啟狀態。 (<see langword="true"/>)開啟 (<see langword="false"/>)關閉</summary>
		public bool IsConnected { get { return mSerial.IsOpen; } }
		#endregion

		#region Function - Constructors
		/// <summary>建構調光器模組</summary>
		public CtDimmer_CH5I700RS232() {

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
				switch (rxData[3]) {
					case 0x01:
						dimmerData = new VersionData(rxData);
						break;
					case 0xDF:
						dimmerData = new AllLights(rxData);
						break;
					case 0xDE:
						dimmerData = new SingleLight(rxData);
						break;
					case 0xDB:
						dimmerData = new TemperatureData(rxData);
						break;
					case 0xDD:
					case 0xDC:
					case 0xD8:
						dimmerData = new SetPoint(rxData);
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
										//Thread.Sleep(1);        //Delay. If with fast delay, it may without change of dimmer. (Buffer failed ??)

				if (!WaitResponse(out dimmerData)) stt = Stat.ER_COM_RTMO;  //Waiting response from dimmer

			} catch (Exception ex) {
				if (stt == Stat.SUCCESS) stt = Stat.ER_COM_SEND;
				CtStatus.Report(Stat.ER_COM_SEND, ex);
			}
			rxData = dimmerData;
			return stt;
		}

		/// <summary>透過 RS-232 傳送資料</summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		private Stat SendData(string cmd) {
			Stat stt = Stat.SUCCESS;
			try {
				mSerial.ClearBuffer();
				mSerial.Send(cmd);
			} catch (Exception ex) {
				if (stt == Stat.SUCCESS) stt = Stat.ER_COM_SEND;
				CtStatus.Report(Stat.ER_COM_SEND, ex);
			}
			return stt;
		}

		/// <summary>將調光器數值換算成 byte 資料</summary>
		/// <param name="value">調光器數值</param>
		/// <param name="p">保留字元</param>
		/// <returns>byte 資料</returns>
		/// <remarks>目前無使用。由於 Call Function 會再浪費 CPU Ticks，所以目前採用直接計算；另 CheckSum 已設計成 params 方式，就不再做額外的變動了</remarks>
		private List<byte> CalculateLightValue(int value, byte p = 0) {
			List<byte> lightValue = new List<byte>();
			lightValue.Add((byte)((value / 256) << 4));     //It's two parts of this byte. 0x(value MSB)(channel mode). Default "mode" is (0)constant.
			lightValue.Add((byte)(value % 256));            //The LSB of value
			lightValue.Add(p);                              //Useless byte
			return lightValue;
		}

		/// <summary>接收 Buffer 裡的資料並分析之</summary>
		/// <param name="rxData">接收到的資料</param>
		/// <returns>(<see langword="true"/>)成功收到可解析資料 (<see langword="false"/>)無回應或不可解析之資料</returns>
		private bool WaitResponse(out DimmerData rxData) {
			bool result = false;
			List<byte> comData;

			mSerial.Receive(out comData);               //手動從 Buffer 接收資料
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

		/// <summary>[不支援] 取得單一通道數值，當前版本不支援讀取功能</summary>
		/// <param name="channel">欲取得數值的通道</param>
		/// <param name="value">取得的數值</param>
		/// <returns>Status Code</returns>
		public Stat GetLight(Channels channel, out LightValue value) {
			value = null;
			return Stat.ER_SYS_ILLCMD;
		}

		/// <summary>[不支援] 取得所有通道數值，當前版本不支援讀取功能</summary>
		/// <param name="value">當前四組電流數值</param>
		/// <returns>Status Code</returns>
		public Stat GetLight(out List<LightValue> value) {
			value = null;
			return Stat.ER_SYS_ILLCMD;
		}

		/// <summary>[不支援] 設定單一通道數值，當前版本不支援單一設定功能</summary>
		/// <param name="channel">調光器組別(通道、頻道)</param>
		/// <param name="lightValue">電流數值，單位為 毫安培(mA)</param>
		/// <returns>(<see langword="true"/>)設定成功 (<see langword="false"/>)設定失敗</returns>
		public bool SetLight(Channels channel, int lightValue) {
			return false;
		}

		/// <summary>設定所有組別之電流數值</summary>
		/// <param name="ch1Value">第 1 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch2Value">第 2 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch3Value">第 3 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch4Value">第 4 組數值，單位為 毫安培(mA)</param>
		/// <returns>(<see langword="true"/>)設定成功 (<see langword="false"/>)設定失敗</returns>
		public bool SetLight(int ch1Value, int ch2Value, int ch3Value, int ch4Value) {
			string cmd = "%1L"
							+ ch1Value.ToString() + " "
							+ ch2Value.ToString() + " "
							+ ch3Value.ToString() + " "
							+ ch4Value.ToString() + " 0";

			Stat stt = SendData(cmd);

			CtTimer.Delay(20);

			cmd = "%1O";
			stt = SendData(cmd);

			return true;
		}

		/// <summary>設定所有組別之電流數值</summary>
		/// <param name="ch1Value">第 1 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch2Value">第 2 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch3Value">第 3 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch4Value">第 4 組數值，單位為 毫安培(mA)</param>
		/// <param name="ch5Value">第 5 組數值，單位為 毫安培(mA)</param>
		/// <returns>(<see langword="true"/>)設定成功 (<see langword="false"/>)設定失敗</returns>
		public bool SetLight(int ch1Value, int ch2Value, int ch3Value, int ch4Value, int ch5Value) {
			string cmd = "%1L"
							+ ch1Value.ToString() + " "
							+ ch2Value.ToString() + " "
							+ ch3Value.ToString() + " "
							+ ch4Value.ToString() + " "
							+ ch5Value.ToString();

			Stat stt = SendData(cmd);

			CtTimer.Delay(20);

			cmd = "%1O";
			stt = SendData(cmd);

			return true;
		}

		/// <summary>設定所有組別之電流數值</summary>
		/// <param name="values">數值集合</param>
		/// <returns>(<see langword="true"/>)設定成功 (<see langword="false"/>)設定失敗</returns>
		public bool SetLight(IEnumerable<int> values) {
			if (values.All(val => val > -1)) {
				if (values.Count() == 4)
					return SetLight(values.ElementAt(0), values.ElementAt(1), values.ElementAt(2), values.ElementAt(3));
				if (values.Count() == 5)
					return SetLight(values.ElementAt(0), values.ElementAt(1), values.ElementAt(2), values.ElementAt(3), values.ElementAt(4));
				else return false;
			} else {
				bool ret = true;
				for (int idx = 0; idx < values.Count(); idx++) {
					if (values.ElementAt(idx) > -1) {
						ret &= SetLight((Channels)idx, values.ElementAt(idx));
					}
				}
				return ret;
			}
		}

		/// <summary>調光器開關</summary>
		/// <param name="onOff">(<see langword="true"/>)開 (<see langword="false"/>)關</param>
		/// <returns>(<see langword="true"/>)設定成功 (<see langword="false"/>)設定失敗</returns>
		/// <remarks>此型號僅能全部開或全部關...</remarks>
		public bool LightSwitcher(bool onOff) {
			string cmd = onOff ? "%1O" : "%1X";
			Stat stt = SendData(cmd);
			return true;
		}
		#endregion
	}
}
