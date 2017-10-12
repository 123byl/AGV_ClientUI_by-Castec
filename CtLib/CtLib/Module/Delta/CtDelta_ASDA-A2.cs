using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CtLib.Forms;
using CtLib.Library;
using CtLib.Module.Modbus;
using CtLib.Module.SerialPort;
using CtLib.Module.Utility;

namespace CtLib.Module.Delta {

	/// <summary>
	/// Delta Servo Deriver (ASDA-A2) 模組，採用 ModbusRTU 模式(RS-232/RS-485均可)
	/// 目前提供簡易資訊查詢與馬達控制，尚待持續補充
	/// </summary>
	public class CtDelta_ASDA_A2 : IDisposable, ICtVersion {

		#region Version

		/// <summary>CtDelta_ASDA-A2 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 0.0.0  Ahern [2015/08/13]
		///     + 從 DeltaMotor 測試專案搬移至此
		///     
		/// 1.0.0  Ahern [2015/08/26]
		///     - INH，讀取 Servo 狀態改以 DO 輸出
		/// 
		/// 1.0.1  Ahern [2015/11/18]
		///     + 執行動作前檢查馬達狀態
		///     + WaitMoveDone 如果馬達不動則跳出
		///     
		/// 1.0.2  Ahern [2017/05/18]
		///		+ P4-06 SDO
		///		+ P4-07 SDI
		/// 
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(1, 0, 2, "2017/05/18", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Definitions
		/// <summary>狀態暫存器之存放編號。設定時將直接從此表格抓取相對應暫存器</summary>
		private static readonly Dictionary<MonitorVaries, MonitorRegisters> DEFAULT_STATUS_MONITOR = new Dictionary<MonitorVaries, MonitorRegisters> {
			{ MonitorVaries.FeedbackEncoderPUU, MonitorRegisters.Register1 },
			{ MonitorVaries.FeedbackEncoderPulse, MonitorRegisters.Register1 },
			{ MonitorVaries.PositionPUU, MonitorRegisters.Register1 },
			{ MonitorVaries.POSITION_PULSE, MonitorRegisters.Register1 },
			{ MonitorVaries.FeedbackSpeed, MonitorRegisters.Register2 },
			{ MonitorVaries.AverageOutputTorque, MonitorRegisters.Register3 },
			{ MonitorVaries.OutputTorquePeak, MonitorRegisters.Register3 }
		};
		#endregion

		#region Declaration - Support Class
		/// <summary>[P0-46] 驅動器數位輸出(DO)訊號狀態</summary>
		public class DigitalOutputSignal {
			/// <summary>取得伺服是否備妥</summary>
			public bool ServoReady { get; private set; }
			/// <summary>取得伺服是否啟動</summary>
			public bool ServoON { get; private set; }
			/// <summary>取得零速度是否檢出</summary>
			public bool ZeroSpeed { get; private set; }
			/// <summary>取得是否到達目標速度</summary>
			public bool SpeedReached { get; private set; }
			/// <summary>取得是否到達目標位置</summary>
			public bool PositionReached { get; private set; }
			/// <summary>取得扭矩是否限制中</summary>
			public bool TorqueLimited { get; private set; }
			/// <summary>取得是否伺服警示中</summary>
			public bool Alarm { get; private set; }
			/// <summary>取得電磁煞車控制是否輸出</summary>
			public bool Breaker { get; private set; }
			/// <summary>取得原點復歸是否完成</summary>
			public bool Home { get; private set; }
			/// <summary>取得是否馬達過負載預警中</summary>
			public bool Overload { get; private set; }
			/// <summary>取得是否伺服警告中</summary>
			public bool Warning { get; private set; }
			/// <summary>建立驅動器數位輸出(DO)訊號狀態</summary>
			/// <param name="data">通訊資料</param>
			public DigitalOutputSignal(ushort data) {
				SignalAnalysis(data);
			}
			/// <summary>建立驅動器數位輸出(DO)訊號狀態</summary>
			/// <param name="data">通訊資料</param>
			public DigitalOutputSignal(IEnumerable<byte> data) {
				if (data.Count() == 2) {
					SignalAnalysis((ushort) (data.ElementAt(0) * 256 + data.ElementAt(1)));
				}
			}
			/// <summary>依照手冊將各 bit 狀態對應到輸出狀態</summary>
			/// <param name="data">通訊資料</param>
			private void SignalAnalysis(ushort data) {
				ServoReady = (data & 0x0001) == 0x0001 ? true : false;
				ServoON = (data & 0x0002) == 0x0002 ? true : false;
				ZeroSpeed = (data & 0x0004) == 0x0004 ? true : false;
				SpeedReached = (data & 0x0008) == 0x0008 ? true : false;
				PositionReached = (data & 0x0010) == 0x0010 ? true : false;
				TorqueLimited = (data & 0x0020) == 0x0020 ? true : false;
				Alarm = (data & 0x0040) == 0x0040 ? true : false;
				Breaker = (data & 0x0080) == 0x0080 ? true : false;
				Home = (data & 0x0100) == 0x0100 ? true : false;
				Overload = (data & 0x0200) == 0x0200 ? true : false;
				Warning = (data & 0x0400) == 0x0400 ? true : false;
			}
		}
		#endregion

		#region Declaration - Enumerations
		/// <summary>吋動(Jog)移動方向</summary>
		public enum JogDirection : byte {
			/// <summary>往前(正方向)</summary>
			Forward,
			/// <summary>往後(負方向)</summary>
			Backward
		}

		/// <summary>馬達位置模式</summary>
		/// <remarks>對應 <see cref="MonitorVaries"/>，此處獨立僅方便操作</remarks>
		public enum Positions : byte {
			/// <summary>編碼器(Encoder)數值，單位為「使用者單位」(Pulse of User Unit, PUU)</summary>
			Encoder_PUU = 0x00,
			/// <summary>編碼器(Encoder)數值，單位為「脈波」(Pulse)</summary>
			Encoder_Pulse = 0x03,
			/// <summary>馬達迴授位置，單位為「使用者單位」(Pulse of User Unit, PUU)</summary>
			Position_PUU = 0x01,
			/// <summary>馬達迴授位置，單位為「脈波」(Pulse)</summary>
			Position_Pulse = 0x04
		}

		/// <summary>馬達位置單位</summary>
		public enum Units : byte {
			/// <summary>每分鐘轉速。 r/min = RPM = Rotation/Minute</summary>
			RotationPerMinute = 0x00,
			/// <summary>每秒脈波數。 p/s = PPS = Pulse/Second</summary>
			PulsePerSecond = 0x40
		}

		/// <summary>[P0-02][7.2.1] 狀態監視變數</summary>
		public enum MonitorVaries : byte {
			/// <summary>馬達編碼器迴授的位置座標(電子齒輪比之後)。單位為「使用者單位」(Pulse of User Unit, PUU)</summary>
			FeedbackEncoderPUU = 0x00,
			/// <summary>位置命令的目前座標(電子齒輪比之後)，相當於上位機發送的命令脈波數。單位為「使用者單位」(Pulse of User Unit, PUU)</summary>
			PositionPUU = 0x01,
			/// <summary>控制命令位置(<see cref="MonitorVaries.PositionPUU"/>)與編碼器迴授位置(<see cref="MonitorVaries.FeedbackEncoderPUU"/>)誤差數。單位為「使用者單位」(Pulse of User Unit, PUU)</summary>
			ErrorPUU = 0x02,
			/// <summary>馬達編碼器迴授的位置脈波數。單位為「脈波」(Pulse)，預設為範圍為 [128 萬  Pulse/rev]</summary>
			FeedbackEncoderPulse = 0x03,
			/// <summary>位置命令的目前座標(電子齒輪比之後)。單位為「脈波」(Pulse)</summary>
			POSITION_PULSE = 0x04,
			/// <summary>控制命令脈波(<see cref="MonitorVaries.POSITION_PULSE"/>)與編碼器迴授脈波(<see cref="MonitorVaries.FeedbackEncoderPulse"/>)誤差數。單位為「脈波」(Pulse)</summary>
			ErrorPulse = 0x05,
			/// <summary>馬達目前轉速，有經過低通濾波器，數值較穩定。單位為「0.1 * 每分鐘轉次」(0.1 r/min, 0.1RPM)</summary>
			FeedbackSpeed = 0x07,
			/// <summary>由類比通道輸入的扭力命令。單位為「0.01 * 伏特」(0.01 Volt, 0.01V)</summary>
			AnalogyInputTorque = 0x0A,
			/// <summary>整合的扭力命令，來源可能是類比、暫存器或速度迴路所產生。單位為「百分比值」(%)</summary>
			InputTorquePercent = 0x0B,
			/// <summary>驅動器輸出的平均轉矩(負載比率)。單位為「百分比值」(%)</summary>
			AverageOutputTorque = 0x0C,
			/// <summary>驅動器輸出的峰值轉矩(負載比率)。單位為「百分比值」(%)</summary>
			OutputTorquePeak = 0x0D,
			/// <summary>IGBT 溫度。單位為「攝氏」(°C)</summary>
			IGBTTemperature = 0x0F,
			/// <summary>馬達位置與 Z 相的偏移量，範圍為 -5000 ~ +5000。與 Z 相重疊處其值為 0，數值愈大偏移愈多。單位為「脈波」(Pulse)</summary>
			ZOffset = 0x12,
			/// <summary>絕對型編碼器之電池電壓。單位為「0.1 * 伏特」(0.1 Volt, 0.1V)，例如：若顯示 36，表示電池電壓為 3.6V</summary>
			BatteryVoltage = 0x26,
			/// <summary>馬達目前實際電流。單位為「0.01 * 安培」(0.01 Ampere, 0.01A)</summary>
			FeedbackAmpere = 0x37,
			/// <summary>錯誤的狀態變數。僅供內部使用</summary>
			Invalid = 0xFF
		}

		/// <summary>狀態監控暫存器</summary>
		/// <remarks>此列舉用於限定使用者不要超出範圍。另搭配 <see cref="MonitorVaries"/> 可讀取其數值</remarks>
		public enum MonitorRegisters : byte {
			/// <summary>[P0-17] 狀態監控暫存器 #1</summary>
			/// <remarks>設定後，對應數值暫存器為 P0-09</remarks>
			Register1 = 0,
			/// <summary>[P0-18] 狀態監控暫存器 #2</summary>
			/// <remarks>設定後，對應數值暫存器為 P0-10</remarks>
			Register2 = 1,
			/// <summary>[P0-19] 狀態監控暫存器 #3</summary>
			/// <remarks>設定後，對應數值暫存器為 P0-11</remarks>
			Register3 = 2,
			/// <summary>[P0-20] 狀態監控暫存器 #4</summary>
			/// <remarks>設定後，對應數值暫存器為 P0-12</remarks>
			Register4 = 3,
			/// <summary>[P0-21] 狀態監控暫存器 #5</summary>
			/// <remarks>設定後，對應數值暫存器為 P0-13</remarks>
			Register5 = 4,
		}

		/// <summary>[P5-04] 原點復歸時，正負極限觸發後之處理動作</summary>
		public enum HomeLimit : byte {
			/// <summary>發報異常</summary>
			ThrowException = 0,
			/// <summary>方向反轉</summary>
			InverseDirection = 1
		}

		/// <summary>[P5-04] 尋找 Z 相訊號之設定</summary>
		public enum HomeZSignal : byte {
			/// <summary>返回找 Z</summary>
			Backward = 0,
			/// <summary>不返回找 Z (往前找 Z)</summary>
			Forward = 1,
			/// <summary>一律不找 Z</summary>
			DoNotFindZ = 2
		}

		/// <summary>[P5-04] 復歸方式</summary>
		public enum HomeMode : byte {
			/// <summary>正轉方向原點復歸，PL 作為復歸原點</summary>
			PL_CW = 0,
			/// <summary>反轉方向原點復歸，NL 作為復歸原點</summary>
			NL_CCW = 1,
			/// <summary>正轉方向原點復歸，等待 ORG 訊號由 OFF → ON (Raising Edge-Triggered) 作為復歸原點</summary>
			ORG_R_TRIG_CW = 2,
			/// <summary>反轉方向原點復歸，等待 ORG 訊號由 OFF → ON (Raising Edge-Triggered) 作為復歸原點</summary>
			ORG_R_TRIG_CCW = 3,
			/// <summary>正轉直接尋找 Z 脈波作為復歸原點</summary>
			Z_PULSE_CW = 4,
			/// <summary>反轉直接尋找 Z 脈波作為復歸原點</summary>
			Z_PULSE_CCW = 5,
			/// <summary>正轉方向原點復歸，等待 ORG 訊號由 ON → OFF (Falling Edge-Triggered) 作為復歸原點</summary>
			ORG_F_TRIG_CW = 6,
			/// <summary>反轉方向原點復歸，等待 ORG 訊號由 ON → OFF (Falling Edge-Triggered) 作為復歸原點</summary>
			ORG_F_TRIG_CCW = 7,
			/// <summary>直接定義目前位置為原點</summary>
			CURRENT_POSITION = 8,
			/// <summary>正轉方向找碰撞點當作原點</summary>
			COL_CW = 9,
			/// <summary>反轉方向找碰撞點當作原點</summary>
			COL_CCW = 10
		}

		/// <summary>Cmd_E 位置命令終點</summary>
		public enum MotionCommand : ushort {
			/// <summary>位置命令終點，直接指定位置</summary>
			Absolute = 0x00,
			/// <summary>位置命令終點由上一次命令終點(監視變數 40h)，加上指定的增加量。與 Relative 相似</summary>
			Incremental = 0x80,
			/// <summary>位置命令終點由目前位置回授(監視變數 00h)，加上指定的增加量。與 Incremental 相似</summary>
			Relative = 0x40,
			/// <summary>位置命令終點由 CAP 抓取位置(監視變數 2Bh)，加上指定的增加量</summary>
			CAP = 0xC0
		}
		#endregion

		#region Declaration - Fields
		/// <summary>ModbusRTU 相關處理</summary>
		private CtModbus_RTU mModbusRTU = new CtModbus_RTU();

		/// <summary>局號</summary>
		private byte mDevID = 0;
		/// <summary>[P2-30] 輔助機能數值(暫存用)</summary>
		private byte mINH = 5;
		/// <summary>暫存目前狀態監控暫存器之監視變數，更改時如已相同則不進行修改動作</summary>
		private Dictionary<MonitorRegisters, MonitorVaries> mMonitorRegStt = new Dictionary<MonitorRegisters, MonitorVaries> {
			{MonitorRegisters.Register1, MonitorVaries.Invalid },
			{MonitorRegisters.Register2, MonitorVaries.Invalid },
			{MonitorRegisters.Register3, MonitorVaries.Invalid },
			{MonitorRegisters.Register4, MonitorVaries.Invalid },
			{MonitorRegisters.Register5, MonitorVaries.Invalid }
		};
		#endregion

		#region Declaration - Properties
		/// <summary>取得目前是否已連線至 Delta ASDA-A2</summary>
		public bool IsConnected { get { return mModbusRTU.IsOpen; } }
		/// <summary>取得或設定串列埠鮑率(Baud Rate)</summary>
		public int BaudRate { get; set; }
		/// <summary>取得或設定資料位元數</summary>
		public byte DataBits { get; set; }
		/// <summary>取得或設定串列埠交握方式</summary>
		public CtSerial.Handshake HandShake { get; set; }
		/// <summary>取得或設定同位元方式</summary>
		public CtSerial.Parity Parity { get; set; }
		/// <summary>取得或設定停止位元數</summary>
		public CtSerial.StopBits StopBits { get; set; }
		/// <summary>取得或設定該 ASDA-A2 局號</summary>
		public byte DeviceID { get { return mDevID; } set { mDevID = value; } }
		/// <summary>取得當前電腦所安裝的串列埠名稱</summary>
		public List<string> SerialPorts { get { return CtSerial.GetPortNames(); } }

		#endregion

		#region Declaration - Events

		/// <summary>CtDelta_ASDA_A2 事件</summary>
		public event EventHandler<ASDA_A2_EventArgs> On_ASDA_A2_Events;

		/// <summary>觸發 ASDA_A2 事件</summary>
		/// <param name="events">CtDelta_ASDA_A2 事件</param>
		/// <param name="value">事件所帶的參數</param>
		protected virtual void RaiseEvents(ASDA_A2_Events events, object value) {
			EventHandler<ASDA_A2_EventArgs> handler = On_ASDA_A2_Events;
			if (handler != null) handler(this, new ASDA_A2_EventArgs(events, value));
		}
		#endregion

		#region Function - Constructors
		/// <summary>建立帶有預設值之 ASDA-A2 物件</summary>
		public CtDelta_ASDA_A2() {
			DeviceID = 1;
			BaudRate = 115200;
			DataBits = 8;
			HandShake = CtSerial.Handshake.None;
			Parity = CtSerial.Parity.None;
			StopBits = CtSerial.StopBits.Two;
			mModbusRTU.OnSerialEvents += mModbusRTU_OnSerialEvents;
		}

		/// <summary>建立 ASDA-A2 物件，並直接指定其局號</summary>
		/// <param name="deviceID">[P3-00] ASDA-A2 局號</param>
		public CtDelta_ASDA_A2(byte deviceID) {
			DeviceID = deviceID;
			BaudRate = 115200;
			DataBits = 8;
			HandShake = CtSerial.Handshake.None;
			Parity = CtSerial.Parity.None;
			StopBits = CtSerial.StopBits.Two;
			mModbusRTU.OnSerialEvents += mModbusRTU_OnSerialEvents;
		}

		/// <summary>建立 ASDA-A2 物件，並指定其串列埠設定</summary>
		/// <param name="baudRate">傳輸速度，鮑率(BaudRate)</param>
		/// <param name="dataBit">資料位元數</param>
		/// <param name="hsk">硬體交握方式</param>
		/// <param name="parity">同位元檢查</param>
		/// <param name="stopBit">停止位元</param>
		public CtDelta_ASDA_A2(int baudRate, byte dataBit, CtSerial.Handshake hsk, CtSerial.Parity parity, CtSerial.StopBits stopBit) {
			DeviceID = 1;
			BaudRate = baudRate;
			DataBits = dataBit;
			HandShake = hsk;
			Parity = parity;
			StopBits = stopBit;
			mModbusRTU.OnSerialEvents += mModbusRTU_OnSerialEvents;
		}

		/// <summary>建立 ASDA-A2 物件，並指定其串列埠設定與局號</summary>
		/// <param name="deviceID">[P3-00] ASDA-A2 局號</param>
		/// <param name="baudRate">傳輸速度，鮑率(BaudRate)</param>
		/// <param name="dataBit">資料位元數</param>
		/// <param name="hsk">硬體交握方式</param>
		/// <param name="parity">同位元檢查</param>
		/// <param name="stopBit">停止位元</param>
		public CtDelta_ASDA_A2(byte deviceID, int baudRate, byte dataBit, CtSerial.Handshake hsk, CtSerial.Parity parity, CtSerial.StopBits stopBit) {
			DeviceID = deviceID;
			BaudRate = baudRate;
			DataBits = dataBit;
			HandShake = hsk;
			Parity = parity;
			StopBits = stopBit;
			mModbusRTU.OnSerialEvents += mModbusRTU_OnSerialEvents;
		}

		/// <summary>建立 ASDA-A2 物件，並帶入已建立的 <see cref="CtModbus_RTU"/></summary>
		/// <param name="rtu">已建立的 <see cref="CtModbus_RTU"/></param>
		public CtDelta_ASDA_A2(CtModbus_RTU rtu) {
			mModbusRTU = rtu;
			mModbusRTU.OnSerialEvents += mModbusRTU_OnSerialEvents;
		}

		/// <summary>建立 ASDA-A2 物件，指定其局號並帶入已建立的 <see cref="CtModbus_RTU"/></summary>
		/// <param name="deviceID">[P3-00] ASDA-A2 局號</param>
		/// <param name="rtu">已建立的 <see cref="CtModbus_RTU"/></param>
		public CtDelta_ASDA_A2(byte deviceID, CtModbus_RTU rtu) {
			DeviceID = deviceID;
			mModbusRTU = rtu;
			mModbusRTU.OnSerialEvents += mModbusRTU_OnSerialEvents;
		}
		#endregion

		#region Function - Disposable
		/// <summary>中斷與 Delta ASDA-A2 之連線，並釋放相關資源</summary>
		public void Dispose() {
			try {
				Dispose(true);
				GC.SuppressFinalize(this);
			} catch (ObjectDisposedException ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
		}

		/// <summary>中斷與 Delta ASDA-A2 之連線，並釋放相關資源</summary>
		/// <param name="isDisposing">是否為第一次釋放</param>
		protected virtual void Dispose(bool isDisposing) {
			try {
				if (isDisposing) {
					if (mModbusRTU != null) {
						mModbusRTU.Close();
						mModbusRTU.OnSerialEvents -= mModbusRTU_OnSerialEvents;
					}
				}
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
		}
		#endregion

		#region Function - ModbusRTU Events
		private void mModbusRTU_OnSerialEvents(object sender, SerialEventArgs e) {
			switch (e.Event) {
				case SerialPortEvents.Connection:
					CtThread.AddWorkItem(obj => RaiseEvents(ASDA_A2_Events.Connection, CtConvert.CBool(e.Value)));
					break;
				case SerialPortEvents.DataReceived:
					//ModbusRTU 裡面已經覆寫此事件，基本上是不會發出任何東西...
					break;
				case SerialPortEvents.Error:
					if (e.Value is string)
						CtThread.AddWorkItem(obj => RaiseEvents(ASDA_A2_Events.CommunicationError, e.Value as string));
					else
						CtThread.AddWorkItem(obj => RaiseEvents(ASDA_A2_Events.DeviceError, e.Value as List<byte>));
					break;
			}
		}
		#endregion

		#region Function - Methods
		/// <summary>統一檢查並發報狀態</summary>
		/// <param name="stt">欲檢查的 Status Code</param>
		/// <param name="value">通訊所收到的數值</param>
		private void CheckReturnValue(Stat stt, List<byte> value) {
			if (stt == Stat.SUCCESS) return;
			else if (stt == Stat.ER3_MB_SLVERR) {
				if (value != null) RaiseEvents(ASDA_A2_Events.DeviceError, value.ToList());
				else RaiseEvents(ASDA_A2_Events.CommunicationError, "Error responded from slave device, but null argument");
			} else if (stt == Stat.ER3_MB_COMTMO) {
				RaiseEvents(ASDA_A2_Events.CommunicationError, "Waiting response timeout");
			} else {
				RaiseEvents(ASDA_A2_Events.CommunicationError, "Received unrecognized data");
			}
		}

		/// <summary>[P2-30] 備份輔助機能數值</summary>
		/// <remarks>
		/// 因使用通訊並更改面板數值時須改為 5，但 0/1 才可得知 Servo 狀態。作此備份，於更改面板數值後複寫回正確數值
		/// 20150826 - 發現 P0-46 抓取 DO 狀態可以讀取到 Servo 狀態，故此方法將不再使用，但保留不刪除
		/// </remarks>
		private void BackupINHOnly() {
			Stat stt = Stat.SUCCESS;
			byte count = 0;
			List<byte> result = null;

			do {
				stt = mModbusRTU.ReadHoldingRegister(mDevID, 0x023C, 1, out result);
				CheckReturnValue(stt, result);
				if (result != null) mINH = result[1];
				else count++;
			} while (result == null && count < 4);

			if (result == null) throw (new System.IO.IOException("SerialPort opened, but no response from it"));
		}

		/// <summary>[P2-30] 備份輔助機能數值，備份後設定為 5 表示不寫入 EEPROM</summary>
		/// <remarks>
		/// 因使用通訊並更改面板數值時須改為 5，但 0/1 才可得知 Servo 狀態。作此備份，於更改面板數值後複寫回正確數值
		/// 20150826 - 發現 P0-46 抓取 DO 狀態可以讀取到 Servo 狀態，故此方法將不再使用，但保留不刪除
		/// </remarks>
		private void BackupINH() {
			List<byte> result;

			Stat stt = mModbusRTU.ReadHoldingRegister(mDevID, 0x023C, 1, out result);
			CheckReturnValue(stt, result);
			if (result != null && result[1] != 5) mINH = result[1];
			mModbusRTU.WriteSingleRegister(mDevID, 0x023C, 5, out result);

		}

		/// <summary>[P2-30] 還原備份的輔助機能數值</summary>
		/// <remarks>20150826 - 發現 P0-46 抓取 DO 狀態可以讀取到 Servo 狀態，故此方法將不再使用，但保留不刪除</remarks>
		private void RestoreINH() {
			List<byte> result;
			mModbusRTU.WriteSingleRegister(mDevID, 0x023C, mINH, out result);

		}

		/// <summary>[P0-17+] 設定狀態監控暫存器</summary>
		/// <param name="monitorVar">欲設定的狀態，將對應於 <see cref="DEFAULT_STATUS_MONITOR"/> 設定其暫存器數值</param>
		/// <returns>Status Code</returns>
		private Stat SetMonitorVaries(MonitorVaries monitorVar) {
			Stat stt = Stat.SUCCESS;

			/*-- 確認引數為可處理範圍 --*/
			if (monitorVar == MonitorVaries.Invalid) stt = Stat.ER_SYS_INVARG;

			/*-- 如果紀錄暫存器之數值，與欲更改的狀態不同，進行更改！反之則不做更改 --*/
			else if (mMonitorRegStt[DEFAULT_STATUS_MONITOR[monitorVar]] != monitorVar) {

				/* 狀態暫存器從 0x0022 開始， #1 = 0x0022、0x0023, #2 = 0x0024、0x0025 依此類推 */
				ushort addr = (ushort) (0x0022 + (byte) DEFAULT_STATUS_MONITOR[monitorVar] * 2);

				/* 寫入設定 */
				List<byte> value;   //接受回傳用
				stt = mModbusRTU.WriteSingleRegister(mDevID, addr, (ushort) monitorVar, out value);

				/* 紀錄當前設定 */
				mMonitorRegStt[DEFAULT_STATUS_MONITOR[monitorVar]] = monitorVar;
			}

			return stt;
		}

		/// <summary>[P0-09+] 取得 <see cref="DEFAULT_STATUS_MONITOR"/> 所相對應的暫存器位址(Address)</summary>
		/// <param name="monitorVar">欲取得位址的狀態監控變數</param>
		/// <returns>相對應的暫存器位址</returns>
		private ushort GetMonitorVariesAddress(MonitorVaries monitorVar) {
			return (ushort) (0x0012 + (byte) DEFAULT_STATUS_MONITOR[monitorVar] * 2);
		}

		/// <summary>檢查 HOME 訊號，如尚未復歸則跳 Exception</summary>
		private void CheckHome() {
			DigitalOutputSignal signal = null;

			/*-- 讀取數值 --*/
			List<byte> result;
			Stat stt = mModbusRTU.ReadHoldingRegister(mDevID, 0x005C, 1, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			/*-- 讀取成功則進行解碼 --*/
			if (stt == Stat.SUCCESS) signal = new DigitalOutputSignal(result);

			/*-- 檢查是否有 Home 了 --*/
			if (signal != null && !signal.Home) throw new InvalidOperationException("馬達尚未進行原點復歸，無法進行動作");
		}

		/// <summary>檢查 READY 訊號，如尚未備妥則跳 Exception</summary>
		private void CheckReady() {
			DigitalOutputSignal signal = null;

			/*-- 讀取數值 --*/
			List<byte> result;
			Stat stt = mModbusRTU.ReadHoldingRegister(mDevID, 0x005C, 1, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			/*-- 讀取成功則進行解碼 --*/
			if (stt == Stat.SUCCESS) signal = new DigitalOutputSignal(result);

			/*-- 檢查是否有 Home 了 --*/
			if (signal != null && !signal.ServoReady) throw new InvalidOperationException("伺服系統上未備妥(Ready)，無法進行動作");
		}

		/// <summary>檢查 SERVO 訊號，如尚未致能則跳 Exception</summary>
		private void CheckServo() {
			DigitalOutputSignal signal = null;

			/*-- 讀取數值 --*/
			List<byte> result;
			Stat stt = mModbusRTU.ReadHoldingRegister(mDevID, 0x005C, 1, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			/*-- 讀取成功則進行解碼 --*/
			if (stt == Stat.SUCCESS) signal = new DigitalOutputSignal(result);

			/*-- 檢查是否有 Home 了 --*/
			if (signal != null && !signal.ServoON) throw new InvalidOperationException("伺服尚未致能(Servo ON)，無法進行動作");
		}

		/// <summary>檢查 READY、HOME、SERVO 訊號，如果任一未致能則跳 Exception</summary>
		private void CheckStatus() {
			DigitalOutputSignal signal = null;

			/*-- 讀取數值 --*/
			List<byte> result;
			Stat stt = mModbusRTU.ReadHoldingRegister(mDevID, 0x005C, 1, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			/*-- 讀取成功則進行解碼 --*/
			if (stt == Stat.SUCCESS) signal = new DigitalOutputSignal(result);

			/*-- 檢查是否有 Home 了 --*/
			if (signal != null) {
				if (!signal.ServoReady) throw new InvalidOperationException("伺服系統上未備妥(Ready)，無法進行動作。請先進行初始化");
				if (!signal.Home) throw new InvalidOperationException("馬達尚未進行原點復歸，無法進行動作。請先施作原點復歸");
				if (!signal.ServoON) throw new InvalidOperationException("伺服尚未致能(Servo ON)，無法進行動作。請先致能馬達");
			} else throw new ArgumentNullException("DigitalOutputSignal", "通訊解碼失敗");
		}

		/// <summary>檢查 ASDA-A2 之 P1-01 控制模式是否為 PR。僅跳出對話視窗提醒</summary>
		public void CheckPR() {

			/*-- 讀取狀態 --*/
			List<byte> result;
			Stat stt = mModbusRTU.ReadHoldingRegister(mDevID, 0x0102, 1, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			/*-- 讀取成功則進行解碼，該暫存器為 16bit --*/
			if (stt == Stat.SUCCESS) {
				if ((result[0] * 256 + result[1]) != 0x01) CtMsgBox.Show("PR-Mode", "當前 ASDA-A2 並非為 PR 模式，部分功能無法施作\r\n請檢查 [P1-01] 數值");
			} else throw new ArgumentNullException("PR-Mode", "通訊失敗");

		}

		#endregion

		#region Function - Connections

		/// <summary>嘗試連接至 ASDA-A2</summary>
		/// <param name="checkPR">連線後是否檢查 ASDA-A2 當前模式為 PR，亦可用於驗證連線是否成功</param>
		/// <returns>Status Code</returns>
		public Stat Connect(bool checkPR = false) {
			Stat stt = Stat.SUCCESS;
			try {
				mModbusRTU.Open();
				if (checkPR) CheckPR();
			} catch (Exception ex) {
				stt = Stat.ER_COM_OPEN;
				RaiseEvents(ASDA_A2_Events.CommunicationError, ex.Message);
			}
			return stt;
		}

		/// <summary>嘗試連接至 ASDA-A2，並指定其串列埠編號</summary>
		/// <param name="comPort">串列埠編號，如 "COM3"</param>
		/// <param name="checkPR">連線後是否檢查 ASDA-A2 當前模式為 PR，亦可用於驗證連線是否成功</param>
		/// <returns>Status Code</returns>
		public Stat Connect(string comPort, bool checkPR = false) {
			Stat stt = Stat.SUCCESS;
			try {
				mModbusRTU.Open(comPort, BaudRate, DataBits, StopBits, Parity, HandShake);
				if (checkPR) CheckPR();
			} catch (Exception ex) {
				stt = Stat.ER_COM_OPEN;
				RaiseEvents(ASDA_A2_Events.CommunicationError, ex.Message);
			}
			return stt;
		}

		/// <summary>嘗試連接至 ASDA-A2，使用預設局號並設定串列埠屬性</summary>
		/// <param name="comPort">串列埠編號，如 "COM3"</param>
		/// <param name="baudRate">傳輸速度，鮑率(BaudRate)</param>
		/// <param name="dataBit">資料位元數</param>
		/// <param name="hsk">硬體交握方式</param>
		/// <param name="parity">同位元檢查</param>
		/// <param name="stopBit">停止位元</param>
		/// <param name="checkPR">連線後是否檢查 ASDA-A2 當前模式為 PR，亦可用於驗證連線是否成功</param>
		/// <returns>Status Code</returns>
		public Stat Connect(string comPort, int baudRate, byte dataBit, CtSerial.Handshake hsk, CtSerial.Parity parity, CtSerial.StopBits stopBit, bool checkPR = false) {
			Stat stt = Stat.SUCCESS;
			try {
				mModbusRTU.Open(comPort, baudRate, dataBit, stopBit, parity, hsk);
				if (checkPR) CheckPR();
			} catch (Exception ex) {
				stt = Stat.ER_COM_OPEN;
				RaiseEvents(ASDA_A2_Events.CommunicationError, ex.Message);
			}
			return stt;
		}

		/// <summary>嘗試連接至 ASDA-A2，使用指定的局號與串列埠編號</summary>
		/// <param name="deviceID">指定的局號，如 0x7F</param>
		/// <param name="comPort">指定的串列埠編號，如 "COM3"</param>
		/// <param name="checkPR">連線後是否檢查 ASDA-A2 當前模式為 PR，亦可用於驗證連線是否成功</param>
		/// <returns>Status Code</returns>
		public Stat Connect(byte deviceID, string comPort, bool checkPR = false) {
			Stat stt = Stat.SUCCESS;
			mModbusRTU.DeviceID = deviceID;
			try {
				mModbusRTU.Open(comPort, BaudRate, DataBits, StopBits, Parity, HandShake);
				if (checkPR) CheckPR();
			} catch (Exception ex) {
				stt = Stat.ER_COM_OPEN;
				RaiseEvents(ASDA_A2_Events.CommunicationError, ex.Message);
			}
			return stt;
		}

		/// <summary>嘗試連接至 ASDA-A2，使用指定的局號與串列埠屬性</summary>
		/// <param name="deviceID">指定的局號，如 0x7F</param>
		/// <param name="comPort">指定的串列埠編號，如 "COM3"</param>
		/// <param name="baudRate">傳輸速度，鮑率(BaudRate)</param>
		/// <param name="dataBit">資料位元數</param>
		/// <param name="hsk">硬體交握方式</param>
		/// <param name="parity">同位元檢查</param>
		/// <param name="stopBit">停止位元</param>
		/// <param name="checkPR">連線後是否檢查 ASDA-A2 當前模式為 PR，亦可用於驗證連線是否成功</param>
		/// <returns>Status Code</returns>
		public Stat Connect(byte deviceID, string comPort, int baudRate, byte dataBit, CtSerial.Handshake hsk, CtSerial.Parity parity, CtSerial.StopBits stopBit, bool checkPR = false) {
			Stat stt = Stat.SUCCESS;
			mModbusRTU.DeviceID = deviceID;
			try {
				mModbusRTU.Open(comPort, baudRate, dataBit, stopBit, parity, hsk);
				if (checkPR) CheckPR();
			} catch (Exception ex) {
				stt = Stat.ER_COM_OPEN;
				RaiseEvents(ASDA_A2_Events.CommunicationError, ex.Message);
			}
			return stt;
		}

		/// <summary>中斷與 ASDA-A2 之連線</summary>
		/// <returns>Status Code</returns>
		public Stat Disconnect() {
			Stat stt = Stat.SUCCESS;
			try {
				mModbusRTU.Close();
			} catch (Exception ex) {
				stt = Stat.ER_COM_OPEN;
				RaiseEvents(ASDA_A2_Events.CommunicationError, ex.Message);
			}
			return stt;
		}
		#endregion

		#region Function - ASDA-A2

		#region Setting
		/// <summary>[P2-30] 更改伺服(Servo)狀態</summary>
		/// <param name="onOff">欲更改的狀態。 (<see langword="true"/>)Servo-ON  (<see langword="false"/>)Servo-OFF</param>
		/// <returns>Status Code</returns>
		public Stat Servo(bool onOff) {
			/*-- 寫入 Servo --*/
			List<byte> result;
			Stat stt = mModbusRTU.WriteSingleRegister(mDevID, 0x023C, (ushort) (onOff ? 1 : 0), out result);

			/*-- 檢查回傳值，但不直接離開。因為要將輔助機能寫回 5 --*/
			CheckReturnValue(stt, result);

			/*-- 將輔助機能設定為 5，通訊更改面板資料時不儲存至 EEPROM --*/
			//stt = mModbusRTU.WriteSingleRegister(mDevID, 0x023C, 5, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			return stt;
		}

		/// <summary>[P0-01] 使用通訊方式清除錯誤(Alarm)</summary>
		/// <returns>Status Code</returns>
		public Stat CleanError() {
			/*-- 清除錯誤暫存器 --*/
			List<byte> result;
			Stat stt = mModbusRTU.WriteSingleRegister(mDevID, 0x0002, 0, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			return stt;
		}

		/// <summary>[P5-20+] 設定加減速時間，單位為毫秒(Millisecond, ms)。共 16 組，從 #0 至 #15</summary>
		/// <param name="index">欲設定的加減速編號。從 0 至 15</param>
		/// <param name="accel">欲設定的加減速時間，單位為毫秒(Millisecond, ms)</param>
		/// <returns>Status Code</returns>
		public Stat SetRampTime(byte index, ushort accel) {
			Stat stt = Stat.SUCCESS;

			/*-- 檢查要更改的編號是否在範圍內 --*/
			if (index < 16) {

				/*-- 計算暫存器位址。 #0 = 0x0528、0x0529; #1 = 0x0530、0x0531 依此類推 --*/
				ushort addr = (ushort) (0x0528 + index * 2); //加減速設定從 P5-20 (0x0528) 開始，兩個一組。從 0 開始

				/*-- 寫入設定 --*/
				List<byte> result;
				stt = mModbusRTU.WriteSingleRegister(mDevID, addr, accel, out result);

				/*-- 檢查回傳值 --*/
				CheckReturnValue(stt, result);

			} else stt = Stat.ER_SYS_INVARG;

			return stt;
		}

		/// <summary>[P5-60+] 設定內部目標速度，單位為 RPM。 共 16 組，從 #0 至 #15</summary>
		/// <param name="index">欲設定的速度編號。從 0 至 15</param>
		/// <param name="speed">欲設定的速度，單位為 RPM</param>
		/// <returns>Status Code</returns>
		public Stat SetSpeed(byte index, ushort speed) {
			Stat stt = Stat.SUCCESS;

			/*-- 檢查要更改的編號是否在範圍內 --*/
			if (index < 16) {

				/*-- 計算暫存器位址。 #0 = 0x0578、0x0579; #1 = 0x0580、0x0581 依此類推 --*/
				ushort addr = (ushort) (0x0578 + index * 2); //計算位址，加減速設定從 P5-60 (0x0578) 開始，兩個一組。從 0 開始

				/*-- 寫入設定 --*/
				List<byte> result;
				stt = mModbusRTU.WriteSingleRegister(mDevID, addr, (ushort) (speed / 0.1), out result);  //使用通訊時，速度單位為 0.1 RPM，為方便使用者設定，於此處做計算即可

				/*-- 檢查回傳值 --*/
				CheckReturnValue(stt, result);

			} else stt = Stat.ER_SYS_INVARG;

			return stt;
		}

		/// <summary>[P5-40+] 設定位置到達後的延遲時間，單位為毫秒(Millisecond, ms)。共 16 組，從 #0 至 #15</summary>
		/// <param name="index">欲設定的延遲時間編號。從 0 至 15</param>
		/// <param name="delayTime">欲設定的延遲時間，單位為毫秒(Millisecond, ms)</param>
		/// <returns>Status Code</returns>
		public Stat SetDelay(byte index, ushort delayTime) {
			Stat stt = Stat.SUCCESS;

			/*-- 檢查要更改的編號是否在範圍內 --*/
			if (index < 16) {

				/*-- 計算暫存器位址。 #0 = 0x0550、0x0551; #1 = 0x0552、0x0553 依此類推 --*/
				ushort addr = (ushort) (0x0550 + index * 2); //計算位址，加減速設定從 P5-60 (0x0578) 開始，兩個一組。從 0 開始

				/*-- 寫入設定 --*/
				List<byte> result;
				stt = mModbusRTU.WriteSingleRegister(mDevID, addr, delayTime, out result);

				/*-- 檢查回傳值 --*/
				CheckReturnValue(stt, result);

			} else stt = Stat.ER_SYS_INVARG;

			return stt;
		}

		/// <summary>[P6-02+][7.10] 設定路徑資料，適用於絕對座標、增量</summary>
		/// <param name="index">欲設定的路徑編號。從 1 至 63</param>
		/// <param name="position">座標(PUU)或增量數值</param>
		/// <param name="accelNo">欲使用的加速時間編號，從 0 至 15。對應於 <see cref="SetRampTime(byte, ushort)"/></param>
		/// <param name="decelNo">欲使用的減速時間編號，從 0 至 15。對應於 <see cref="SetRampTime(byte, ushort)"/></param>
		/// <param name="speedNo">欲使用的速度編號，從 0 至 15。對應於 <see cref="SetSpeed(byte, ushort)"/></param>
		/// <param name="delayNo">欲使用的到點後延遲時間編號，從 0 至 15。對應於 <see cref="SetDelay(byte, ushort)"/></param>
		/// <param name="interrupt">是否插斷前一路徑？  (<see langword="true"/>)立即停止執行中的路徑  (<see langword="false"/>)等待前一路徑完成</param>
		/// <param name="overlap">完成後是否接續下一路徑？  (<see langword="true"/>)完成後自動載入下一路徑 (<see langword="false"/>)完成後停止</param>
		/// <param name="cmd">移動模式</param>
		/// <returns>Status Code</returns>
		public Stat SetPath(byte index, int position, ushort accelNo, ushort decelNo, ushort speedNo, ushort delayNo, bool interrupt = true, bool overlap = false, MotionCommand cmd = MotionCommand.Absolute) {
			Stat stt = Stat.SUCCESS;

			/*-- 檢查索引編號是否於合理範圍內 --*/
			if (0 < index && index < 64) {

				ushort addr = (ushort) (0x0602 + index * 2); //index 從 1 ~ 63 (手冊定義)，但 Speed、RampTime、Delay 等是從0開始喔!!
				ushort param_L = (ushort) (0x0002 + (interrupt ? 0x0010 : 0x0000) + (overlap ? 0x0020 : 0x0000) + (ushort) cmd + (accelNo << 8) + (decelNo << 12));
				ushort param_H = (ushort) (speedNo + (delayNo << 4));
				ushort pos_L = (ushort) (position & 0x0000FFFF);
				ushort pos_H = (ushort) (position >> 16 & 0x0000FFFF);

				/* 寫入數值 */
				List<byte> result;
				stt = mModbusRTU.WriteMultiRegisters(mDevID, addr, new List<ushort> { param_L, param_H, pos_L, pos_H }, out result);

				/* 檢查回傳值 */
				CheckReturnValue(stt, result);

			} else stt = Stat.ER_SYS_INVIDX;

			return stt;
		}

		/// <summary>[P6-02+][7.10] 設定路徑資料，適用於定速巡航</summary>
		/// <param name="index">欲設定的路徑編號。從 1 至 63</param>
		/// <param name="speed">巡航速度</param>
		/// <param name="accelNo">欲使用的加速時間編號，從 0 至 15。對應於 <see cref="SetRampTime(byte, ushort)"/></param>
		/// <param name="decelNo">欲使用的減速時間編號，從 0 至 15。對應於 <see cref="SetRampTime(byte, ushort)"/></param>
		/// <param name="delayNo">欲使用的到點後延遲時間編號，從 0 至 15。對應於 <see cref="SetDelay(byte, ushort)"/></param>
		/// <param name="interrupt">是否插斷前一路徑？  (<see langword="true"/>)立即停止執行中的路徑  (<see langword="false"/>)等待前一路徑完成</param>
		/// <param name="autoLoad">到達等速區後，是否自動載入下一路徑？  (<see langword="true"/>)自動載入  (<see langword="false"/>)單純巡航</param>
		/// <param name="unit">速度單位，RPM 或 PPS</param>
		/// <returns>Status Code</returns>
		public Stat SetPath(byte index, int speed, ushort accelNo, ushort decelNo, ushort delayNo, bool interrupt = true, bool autoLoad = false, Units unit = Units.RotationPerMinute) {
			Stat stt = Stat.SUCCESS;

			/*-- 檢查索引編號是否於合理範圍內 --*/
			if (0 < index && index < 64) {

				if (unit == Units.RotationPerMinute) speed = (int) (speed / 0.1);
				ushort addr = (ushort) (0x0602 + index * 2); //index 從 1 ~ 63 (手冊定義)，但 RampTime、Delay 等是從0開始喔!!
				ushort param_L = (ushort) (0x0001 + (interrupt ? 0x0010 : 0x0000) + (autoLoad ? 0x0020 : 0x0000) + (ushort) unit + (accelNo << 8) + (decelNo << 12));
				ushort param_H = (ushort) (delayNo << 4);
				ushort pos_L = (ushort) (speed & 0x0000FFFF);
				ushort pos_H = (ushort) (speed >> 16 & 0x0000FFFF);

				/* 寫入數值 */
				List<byte> result;
				stt = mModbusRTU.WriteMultiRegisters(mDevID, addr, new List<ushort> { param_L, param_H, pos_L, pos_H }, out result);

				/* 檢查回傳值 */
				CheckReturnValue(stt, result);

			} else stt = Stat.ER_SYS_INVIDX;

			return stt;
		}

		/// <summary>[P4-06] 更改數位輸出狀態</summary>
		/// <param name="ioNum">DO 編號</param>
		/// <param name="stt">ON/OFF</param>
		/// <returns>Status Code</returns>
		public Stat SetSDO(int ioNum, bool stt) {
			var curDo = GetSDO();
			curDo[ioNum] = stt;
			return SetSDO(curDo);
		}

		/// <summary>[P4-06] 更改數位輸出狀態</summary>
		/// <param name="ioStt">DO 狀態集合</param>
		/// <returns>Status Code</returns>
		public Stat SetSDO(List<bool> ioStt) {
			Stat stt = Stat.SUCCESS;

			/* 將 List<bool> 轉成 List<short> */
			List<ushort> ioData;
			CtConvert.ToNumericSequence(ioStt, out ioData);
			ioData.Reverse();	//高低位元對調

			/* 寫入數值 */
			List<byte> result;
			stt = mModbusRTU.WriteMultiRegisters(mDevID, 0x040C, ioData, out result);

			/* 檢查回傳值 */
			CheckReturnValue(stt, result);

			return stt;
		}

		#endregion

		#region Reading

		/// <summary>[P0-46] 取得驅動器數位輸出(DO)訊號狀態</summary>
		/// <returns>訊號狀態。  (null)讀取失敗  (HasValue)當前的訊號狀態</returns>
		public DigitalOutputSignal GetDriverDOState() {
			DigitalOutputSignal signal = null;

			/*-- 讀取數值 --*/
			List<byte> result;
			Stat stt = mModbusRTU.ReadHoldingRegister(mDevID, 0x005C, 1, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			/*-- 讀取成功則進行解碼 --*/
			if (stt == Stat.SUCCESS) signal = new DigitalOutputSignal(result);

			return signal;
		}

		/// <summary>[P0-17+][P0-09+] 讀取當前馬達回授速度，單位為 RPM</summary>
		/// <returns>(null)讀取失敗  (HasValue)回饋的速度數值</returns>
		public double? GetVelocity() {
			double? dblSpeed = null;
			MonitorVaries monVar = MonitorVaries.FeedbackSpeed;

			/*-- 設定狀態監控暫存器 2 的顯示內容為馬達轉速  (r/min) --*/
			Stat stt = SetMonitorVaries(monVar);
			if (stt != Stat.SUCCESS) return dblSpeed;

			/*-- 從狀態監控暫存器 2 讀取當前數值 --*/
			List<byte> result;
			stt = mModbusRTU.ReadHoldingRegister(mDevID, GetMonitorVariesAddress(monVar), 2, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			/*-- 讀取成功則進行解碼 --*/
			if (stt == Stat.SUCCESS) {
				int intTemp = result[0] * 256 + result[1];  //將兩個 byte 組成一個 int， [0]H [1]L
				if (intTemp > 30000) intTemp = 0 - ((result[0] ^ 0xFF) * 256 + (result[1] ^ 0xFF)); //如果超過 30000 表示是負值
				dblSpeed = intTemp * 0.1;
			}

			return dblSpeed;
		}

		/// <summary>[P0-17+][P0-09+] 讀取當前馬達回授的平均轉矩，單位為 % (內部力矩的百分比)</summary>
		/// <returns>(null)讀取失敗  (HasValue)回饋的力矩</returns>
		public int? GetTorque() {
			int? intLoad = null;
			MonitorVaries monVar = MonitorVaries.AverageOutputTorque;

			/*-- 設定狀態監控暫存器 3 的顯示內容為平均轉矩 --*/
			Stat stt = SetMonitorVaries(monVar);

			/*-- 從狀態監控暫存器 3 讀取當前數值 --*/
			List<byte> result;
			stt = mModbusRTU.ReadHoldingRegister(mDevID, GetMonitorVariesAddress(monVar), 2, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			/*-- 讀取成功則進行解碼 --*/
			if (stt == Stat.SUCCESS)
				intLoad = result[0] * 256 + result[1];  //將兩個 byte 組成一個 int， [0]H [1]L

			return intLoad;
		}

		/// <summary>[P0-17+][P0-09+] 取得當前馬達回授的位置</summary>
		/// <param name="posMode">取得回授的類型</param>
		/// <returns>(null)讀取失敗  (HasValue)回饋的數據</returns>
		public int? GetPosition(Positions posMode = Positions.Position_PUU) {
			int? intSpeed = null;

			/*-- 設定狀態監控暫存器 1 的顯示內容 --*/
			Stat stt = SetMonitorVaries((MonitorVaries) posMode);

			/*-- 從狀態監控暫存器 1 讀取當前數值 --*/
			List<byte> result;
			stt = mModbusRTU.ReadHoldingRegister(mDevID, GetMonitorVariesAddress((MonitorVaries) posMode), 2, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			/*-- 讀取成功則進行解碼，數值為 32bit --*/
			if (stt == Stat.SUCCESS) {
				intSpeed = result[2] * 256 + result[3]; //先組成 16bit
				intSpeed <<= 16;    //往高位元位移
				intSpeed += result[0] * 256 + result[1];    //再組成低位元的 16bit
			}

			return intSpeed;
		}

		/// <summary>[P0-46] 取得當前伺服致能狀態(Servo)</summary>
		/// <returns>(null)讀取失敗  (HasValue)(<see langword="true"/>)Servo ON  (<see langword="false"/>)Servo OFF</returns>
		public bool? GetServo() {
			bool? servo = null;

			/*-- 使用輔助機能讀取狀態 --*/
			//List<byte> result;
			//Stat stt = mModbusRTU.ReadHoldingRegister(0x023C, 1, out result);

			/*-- 檢查回傳值 --*/
			//CheckReturnValue(stt, result);

			/*-- 讀取成功則進行解碼，數值於低位元(也就是[1]) --*/
			//if (stt == Stat.SUCCESS)
			//    servo = result[1] == 0 ? false : true;

			/*-- 使用 DO 讀取狀態 --*/
			DigitalOutputSignal signal = GetDriverDOState();

			/*-- 讀取成功則進行轉換 --*/
			if (signal != null) servo = signal.ServoON;

			return servo;
		}

		/// <summary>[P0-01] 取得面板錯誤代碼</summary>
		/// <returns>(null)讀取失敗  (HasValue)錯誤代碼</returns>
		public ushort? GetErrorCode() {
			ushort? error = null;

			/*-- 讀取狀態 --*/
			List<byte> result;
			Stat stt = mModbusRTU.ReadHoldingRegister(mDevID, 0x0002, 1, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			/*-- 讀取成功則進行解碼，該暫存器為 16bit --*/
			if (stt == Stat.SUCCESS)
				error = (ushort) (result[0] * 256 + result[1]);

			return error;
		}

		/// <summary>[P4-06] 取得軟體數位輸出狀態</summary>
		/// <returns>軟體輸出狀態</returns>
		public List<bool> GetSDO() {

			/*-- 讀取狀態 --*/
			List<byte> result;
			Stat stt = mModbusRTU.ReadHoldingRegister(mDevID, 0x040C, 2, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			/*-- 讀取成功則進行解碼，該暫存器為 16bit --*/
			if (stt == Stat.SUCCESS) {
				List<bool> sdo = new List<bool>();
				if (result.Count <= 2) result.Reverse();	//高低位元對調，方便轉成 bool
				else {
					List<byte> tempResult = new List<byte>();
					for (int shiftIdx = 0; shiftIdx < result.Count; shiftIdx+=2) {
						tempResult.Add(result[shiftIdx + 1]);
						tempResult.Add(result[shiftIdx]);
					}
					result.Clear();
					result = tempResult;
				}
				result.ForEach(
					data => {
						byte mask = 1;
						for (int bitIdx = 0; bitIdx < 8; bitIdx++) {
							sdo.Add((data & mask) > 0);
							mask <<= 1;
						}
					}
				);
				return sdo;
			} else return null;
		}

		/// <summary>[P4-07] 取得軟體數位輸入狀態</summary>
		/// <returns>數位輸入狀態</returns>
		public List<bool> GetDI() {

			/*-- 讀取狀態 --*/
			List<byte> result;
			Stat stt = mModbusRTU.ReadHoldingRegister(mDevID, 0x040E, 2, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			/*-- 讀取成功則進行解碼，該暫存器為 16bit --*/
			if (stt == Stat.SUCCESS) {
				List<bool> sdo = new List<bool>();
				if (result.Count <= 2) result.Reverse();    //高低位元對調，方便轉成 bool
				else {
					List<byte> tempResult = new List<byte>();
					for (int shiftIdx = 0; shiftIdx < result.Count; shiftIdx += 2) {
						tempResult.Add(result[shiftIdx + 1]);
						tempResult.Add(result[shiftIdx]);
					}
					result.Clear();
					result = tempResult;
				}
				result.ForEach(
					data => {
						byte mask = 1;
						for (int bitIdx = 0; bitIdx < 8; bitIdx++) {
							sdo.Add((data & mask) > 0);
							mask <<= 1;
						}
					}
				);
				return sdo;
			} else return null;
		}
		#endregion

		#region Motion

		/// <summary>[P5-07] 等待路徑執行完畢</summary>
		/// <returns>Status Code</returns>
		public Stat WaitMoveDone() {
			ushort addr = 0x050E;

			/*-- 讀取觸發暫存器數值 --*/
			List<byte> result;
			Stat stt = mModbusRTU.ReadHoldingRegister(mDevID, addr, 1, out result);

			/*-- 檢查回傳值，如果數值設定失敗則直接離開 --*/
			CheckReturnValue(stt, result);
			if (stt != Stat.SUCCESS) return stt;

			ushort currVal = (ushort) (result[0] * 256 + result[1]);
			ushort waitVal = (ushort) (currVal % 10000 + 20000); //執行中 (10000 + 路徑編號)，完成時會變成 (20000 + 路徑編號)


			byte readCount = 0;
			ushort lastVal = currVal;
			do {
				/*-- 讀取觸發暫存器數值 --*/
				stt = mModbusRTU.ReadHoldingRegister(mDevID, addr, 1, out result);

				/*-- 檢查回傳值 --*/
				CheckReturnValue(stt, result);

				/*-- 如果成功收值，計算其數值 --*/
				if (stt == Stat.SUCCESS)
					currVal = (ushort) (result[0] * 256 + result[1]);

				/*-- 防止馬達根本沒有動 --*/
				/* 如果前一次跟這次不同，重置；反之加一 */
				if (currVal == lastVal) readCount++;
				else {
					lastVal = currVal;
					readCount = 0;
				}
				/* 如果10次都沒反應，那就表示 GG，跳開迴圈！ */
				if (readCount > 9) {
					stt = Stat.ER3_DELTA_COMTMO;
					break;
				}

				CtTimer.Delay(100);
			} while (currVal < waitVal);

			return stt;
		}

		/// <summary>[P4-05] 吋動</summary>
		/// <param name="dir">吋動方向</param>
		/// <param name="speed">移動時速度</param>
		/// <returns>Status Code</returns>
		public Stat JogStart(JogDirection dir, ushort speed = 100) {
			ushort addr = 0x040A;

			/*-- 檢查是否 Servo ON --*/
			CheckServo();

			/*-- 更改速度 --*/
			List<byte> result;
			Stat stt = mModbusRTU.WriteSingleRegister(mDevID, addr, speed, out result);

			/*-- 檢查回傳值，如果數值設定失敗則直接離開 --*/
			CheckReturnValue(stt, result);
			if (stt != Stat.SUCCESS) return stt;

			/*-- 轉轉轉 --*/
			if (dir == JogDirection.Forward) {
				stt = mModbusRTU.WriteSingleRegister(mDevID, addr, 4999, out result);
			} else stt = mModbusRTU.WriteSingleRegister(mDevID, addr, 4998, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			return stt;
		}

		/// <summary>[P4-05] 停止吋動</summary>
		/// <returns>Status Code</returns>
		public Stat JogStop() {
			Stat stt = Stat.SUCCESS;
			List<byte> result;

			/*-- 停止轉轉轉 --*/
			stt = mModbusRTU.WriteSingleRegister(mDevID, 0x040A, 0, out result);

			return stt;
		}

		/// <summary>
		/// 原點復歸，含所有可設定之參數
		/// <para>原點位置、加減速編號、延遲編號、第一與第二段找尋速度、極限模式、Z相找尋、復歸模式、下一路徑</para>
		/// </summary>
		/// <param name="oriPos">原點定義值。原點所在的座標值，原點的座標不一定是 0，此功能係作為座標系統的橫移使用</param>
		/// <param name="accelNo">復歸中所使用的加速時間編號，從 0 至 15。 對應 <see cref="SetRampTime(byte, ushort)"/></param>
		/// <param name="decelNo">復歸中所使用的減速時間編號，從 0 至 15。 對應 <see cref="SetRampTime(byte, ushort)"/></param>
		/// <param name="delayNo">復歸中所使用的到位延遲時間編號，從 0 至 15。 對應 <see cref="SetDelay(byte, ushort)"/></param>
		/// <param name="highSpeed">第一段找尋 ORG 觸發的速度。 ORG 為實體 I/O 訊號，如無使用此訊號則可忽略此數值</param>
		/// <param name="lowSpeed">第二段找尋 Z 相脈波的速度</param>
		/// <param name="limit">極限碰撞設定</param>
		/// <param name="zSig">Z 相找尋模式設定</param>
		/// <param name="mode">復歸模式設定</param>
		/// <param name="nextPath">完成後是否接續路徑。 (0)完成後停止 (1~63)欲執行的PATH路徑</param>
		/// <param name="waitDone">等待歸位完成</param>
		/// <returns>Status Code</returns>
		public Stat Home(int oriPos,
							ushort accelNo, ushort decelNo, ushort delayNo,
							ushort highSpeed, ushort lowSpeed,
							HomeLimit limit, HomeZSignal zSig, HomeMode mode,
							byte nextPath = 0, bool waitDone = true) {

			/*-- 檢查是否 Servo ON --*/
			CheckServo();

			/*-- P6-00 寫入歸 HOME 的 PATH --*/
			List<byte> result;
			ushort oriDef_L = (ushort) ((decelNo << 12) + (accelNo << 8) + nextPath);
			ushort oriDef_H = (ushort) (decelNo << 4);
			Stat stt = mModbusRTU.WriteMultiRegisters(mDevID, 0x0600, new List<ushort> { oriDef_L, oriDef_H }, out result);

			/*-- 檢查回傳值，如果數值設定失敗則直接離開 --*/
			CheckReturnValue(stt, result);
			if (stt != Stat.SUCCESS) return stt;

			/*-- P6-01 寫入原點數值為 0 --*/
			ushort oriPos_L = (ushort) (oriPos & 0x0000FFFF);
			ushort oriPos_H = (ushort) (oriPos >> 16 & 0x0000FFFF);
			stt = mModbusRTU.WriteMultiRegisters(mDevID, 0x0602, new List<ushort> { oriPos_L, oriPos_H }, out result);

			/*-- 檢查回傳值，如果數值設定失敗則直接離開 --*/
			CheckReturnValue(stt, result);
			if (stt != Stat.SUCCESS) return stt;

			/*-- P5-05 更改第一段高速 --*/
			stt = mModbusRTU.WriteSingleRegister(mDevID, 0x050A, (ushort) (highSpeed / 0.1), out result);

			/*-- 檢查回傳值，如果數值設定失敗則直接離開 --*/
			CheckReturnValue(stt, result);
			if (stt != Stat.SUCCESS) return stt;

			/*-- P5-06 更改第二段慢速找 Z --*/
			stt = mModbusRTU.WriteSingleRegister(mDevID, 0x050C, (ushort) (lowSpeed / 0.1), out result);

			/*-- 檢查回傳值，如果數值設定失敗則直接離開 --*/
			CheckReturnValue(stt, result);
			if (stt != Stat.SUCCESS) return stt;

			/*-- P5-04 更改找原點方式 --*/
			ushort value = (ushort) (((ushort) limit << 8) + ((ushort) zSig << 4) + (byte) mode);
			stt = mModbusRTU.WriteSingleRegister(mDevID, 0x0508, value, out result);

			/*-- 檢查回傳值，如果數值設定失敗則直接離開 --*/
			CheckReturnValue(stt, result);
			if (stt != Stat.SUCCESS) return stt;

			/*-- P5-07 觸發，開始進行復歸 --*/
			stt = mModbusRTU.WriteSingleRegister(mDevID, 0x050E, 0, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			/*-- 等待完成 --*/
			if (waitDone) {
				DigitalOutputSignal signal = null;
				CtTimer.WaitTimeout(
					TimeSpan.FromMinutes(3),
					token => {
						do {
							try {
								signal = GetDriverDOState();
								if (signal != null && signal.Home) token.WorkDone();
								else Task.Delay(100).Wait();
							} catch (Exception ex) {
								CtStatus.Report(Stat.ER3_DELTA_COMTMO, ex);
							}
						} while (!token.IsDone);
					}
				);
			}

			return stt;
		}

		/// <summary>
		/// 原點復歸，自訂第一段尋找 ORG 觸發之速度
		/// <para>採用預設加減速與延遲時間，並復歸至數值 0 後即完成復歸</para>
		/// </summary>
		/// <param name="speed">第一段高速復歸速度</param>
		/// <param name="limit">極限碰撞設定</param>
		/// <param name="zSig">Z 相找尋模式設定</param>
		/// <param name="mode">復歸模式設定</param>
		/// <param name="waitDone">等待歸位完成</param>
		/// <returns>Status Code</returns>
		public Stat Home(ushort speed, HomeLimit limit, HomeZSignal zSig, HomeMode mode, bool waitDone = true) {

			/*-- 設定預設加減速，採用手冊所示之初值 200 --*/
			Stat stt = SetRampTime(0, 200);

			/*-- 如果設定失敗則直接離開 --*/
			if (stt != Stat.SUCCESS) return stt;

			/*-- 設定預設加到位延遲，採用手冊所示之初值 0，表示到位後不進行等待 --*/
			stt = SetDelay(0, 0);

			/*-- 如果設定失敗則直接離開 --*/
			if (stt != Stat.SUCCESS) return stt;

			/*-- 執行原點復歸 --*/
			return Home(0, 0, 0, 0, speed, 5, limit, zSig, mode, 0, waitDone);
		}

		/// <summary>
		/// 原點復歸，自訂第一段尋找 ORG 觸發之速度，於完成後接續路徑
		/// <para>採用預設加減速與延遲時間，並復歸至數值 0 後即自動載入指定路徑</para>
		/// </summary>
		/// <param name="speed">第一段高速復歸速度</param>
		/// <param name="nextPath">完成復歸後欲自動載入的路徑，從 1 至 63</param>
		/// <param name="limit">極限碰撞設定</param>
		/// <param name="zSig">Z 相找尋模式設定</param>
		/// <param name="mode">復歸模式設定</param>
		/// <param name="waitDone">等待歸位完成</param>
		/// <returns>Status Code</returns>
		public Stat Home(ushort speed, byte nextPath, HomeLimit limit, HomeZSignal zSig, HomeMode mode, bool waitDone = true) {

			/*-- 設定預設加減速，採用手冊所示之初值 200 --*/
			Stat stt = SetRampTime(0, 200);

			/*-- 如果設定失敗則直接離開 --*/
			if (stt != Stat.SUCCESS) return stt;

			/*-- 設定預設加到位延遲，採用手冊所示之初值 0，表示到位後不進行等待 --*/
			stt = SetDelay(0, 0);

			/*-- 如果設定失敗則直接離開 --*/
			if (stt != Stat.SUCCESS) return stt;

			/*-- 執行原點復歸 --*/
			return Home(0, 0, 0, 0, speed, 50, limit, zSig, mode, nextPath, waitDone);
		}

		/// <summary>
		/// 原點復歸，自訂尋找 ORG 與 Z 相觸發之速度
		/// <para>採用預設加減速與延遲時間，並復歸至數值 0 後即完成復歸</para>
		/// </summary>
		/// <param name="highSpeed">第一段尋找 ORG 觸發的復歸速度</param>
		/// <param name="lowSpeed">第二段尋找 Z 相脈波的復歸速度</param>
		/// <param name="limit">極限碰撞設定</param>
		/// <param name="zSig">Z 相找尋模式設定</param>
		/// <param name="mode">復歸模式設定</param>
		/// <param name="waitDone">等待歸位完成</param>
		/// <returns>Status Code</returns>
		public Stat Home(ushort lowSpeed, ushort highSpeed, HomeLimit limit, HomeZSignal zSig, HomeMode mode, bool waitDone = true) {

			/*-- 設定預設加減速，採用手冊所示之初值 200 --*/
			Stat stt = SetRampTime(0, 200);

			/*-- 如果設定失敗則直接離開 --*/
			if (stt != Stat.SUCCESS) return stt;

			/*-- 設定預設加到位延遲，採用手冊所示之初值 0，表示到位後不進行等待 --*/
			stt = SetDelay(0, 0);

			/*-- 如果設定失敗則直接離開 --*/
			if (stt != Stat.SUCCESS) return stt;

			/*-- 執行原點復歸 --*/
			return Home(0, 0, 0, 0, highSpeed, lowSpeed, limit, zSig, mode, 0, waitDone);
		}

		/// <summary>
		/// 原點復歸，自訂尋找 ORG 與 Z 相觸發之速度，於完成後接續路徑
		/// <para>採用預設加減速與延遲時間，並復歸至數值 0 後即自動載入指定路徑</para>
		/// </summary>
		/// <param name="highSpeed">第一段尋找 ORG 觸發的復歸速度</param>
		/// <param name="lowSpeed">第二段尋找 Z 相脈波的復歸速度</param>
		/// <param name="nextPath">完成復歸後欲自動載入的路徑，從 1 至 63</param>
		/// <param name="limit">極限碰撞設定</param>
		/// <param name="zSig">Z 相找尋模式設定</param>
		/// <param name="mode">復歸模式設定</param>
		/// <param name="waitDone">等待歸位完成</param>
		/// <returns>Status Code</returns>
		public Stat Home(ushort lowSpeed, ushort highSpeed, byte nextPath, HomeLimit limit, HomeZSignal zSig, HomeMode mode, bool waitDone = true) {

			/*-- 設定預設加減速，採用手冊所示之初值 200 --*/
			Stat stt = SetRampTime(0, 200);

			/*-- 如果設定失敗則直接離開 --*/
			if (stt != Stat.SUCCESS) return stt;

			/*-- 設定預設加到位延遲，採用手冊所示之初值 0，表示到位後不進行等待 --*/
			stt = SetDelay(0, 0);

			/*-- 如果設定失敗則直接離開 --*/
			if (stt != Stat.SUCCESS) return stt;

			/*-- 執行原點復歸 --*/
			return Home(0, 0, 0, 0, highSpeed, lowSpeed, limit, zSig, mode, nextPath, waitDone);
		}

		/// <summary>[P6-02+] 執行定速巡航</summary>
		/// <param name="speed">巡航速度</param>
		/// <param name="accelNo">欲使用的加速時間編號，從 0 至 15。對應於 <see cref="SetRampTime(byte, ushort)"/></param>
		/// <param name="decelNo">欲使用的減速時間編號，從 0 至 15。對應於 <see cref="SetRampTime(byte, ushort)"/></param>
		/// <param name="delayNo">欲使用的到點後延遲時間編號，從 0 至 15。對應於 <see cref="SetDelay(byte, ushort)"/></param>
		/// <param name="interrupt">是否插斷前一路徑？  (<see langword="true"/>)立即停止執行中的路徑  (<see langword="false"/>)等待前一路徑完成</param>
		/// <param name="autoLoad">到達等速區後，是否自動載入下一路徑？  (<see langword="true"/>)自動載入  (<see langword="false"/>)單純巡航</param>
		/// <param name="unit">速度單位，RPM 或 PPS</param>
		/// <returns>Status Code</returns>
		public Stat Cruise(int speed, ushort accelNo, ushort decelNo, ushort delayNo, bool interrupt = true, bool autoLoad = false, Units unit = Units.RotationPerMinute) {

			/*-- 檢查是否 Servo ON --*/
			CheckServo();

			/*-- 設定參數 --*/
			if (unit == Units.RotationPerMinute) speed = (int) (speed / 0.1);  //單位為 0.1RPM 或 PPS
			ushort param_L = (ushort) (0x0001 + (interrupt ? 0x0010 : 0x0000) + (autoLoad ? 0x0020 : 0x0000) + (ushort) unit + (accelNo << 8) + (decelNo << 12));
			ushort param_H = (ushort) (delayNo << 4);
			ushort pos_L = (ushort) (speed & 0x0000FFFF);
			ushort pos_H = (ushort) (speed >> 16 & 0x0000FFFF);

			/*-- 寫入路徑資料，定義32Bit、資料32Bit --*/
			List<byte> result;
			Stat stt = mModbusRTU.WriteMultiRegisters(mDevID, 0x0604, new List<ushort> { param_L, param_H, pos_L, pos_H }, out result);

			/*-- 檢查回傳值，如果設定失敗則直接離開 --*/
			CheckReturnValue(stt, result);
			if (stt != Stat.SUCCESS) return stt;

			/*-- P5-07 觸發，開始動作 --*/
			stt = mModbusRTU.WriteSingleRegister(mDevID, 0x050E, 1, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			return stt;
		}

		/// <summary>[P6-02+] 移動至絕對座標、移動特定距離</summary>
		/// <param name="position">座標(PUU)或增量數值</param>
		/// <param name="accelNo">欲使用的加速時間編號，從 0 至 15。對應於 <see cref="SetRampTime(byte, ushort)"/></param>
		/// <param name="decelNo">欲使用的減速時間編號，從 0 至 15。對應於 <see cref="SetRampTime(byte, ushort)"/></param>
		/// <param name="speedNo">欲使用的速度編號，從 0 至 15。對應於 <see cref="SetSpeed(byte, ushort)"/></param>
		/// <param name="delayNo">欲使用的到點後延遲時間編號，從 0 至 15。對應於 <see cref="SetDelay(byte, ushort)"/></param>
		/// <param name="interrupt">是否插斷前一路徑？  (<see langword="true"/>)立即停止執行中的路徑  (<see langword="false"/>)等待前一路徑完成</param>
		/// <param name="overlap">完成後是否接續下一路徑？  (<see langword="true"/>)完成後自動載入下一路徑 (<see langword="false"/>)完成後停止</param>
		/// <param name="cmd">移動模式</param>
		/// <returns>Status Code</returns>
		/// <remarks>Position 定位控制僅限 PUU</remarks>
		public Stat MoveTo(int position, ushort accelNo, ushort decelNo, ushort speedNo, ushort delayNo, bool interrupt = true, bool overlap = false, MotionCommand cmd = MotionCommand.Absolute) {

			/*-- 檢查是否可以進行動作 --*/
			CheckStatus();

			/*-- 設定參數 --*/
			ushort param_L = (ushort) (0x0002 + (interrupt ? 0x0010 : 0x0000) + (overlap ? 0x0020 : 0x0000) + (ushort) cmd + (accelNo << 8) + (decelNo << 12));
			ushort param_H = (ushort) (speedNo + (delayNo << 4));
			ushort pos_L = (ushort) (position & 0x0000FFFF);
			ushort pos_H = (ushort) (position >> 16 & 0x0000FFFF);

			/*-- 寫入路徑資料，定義32Bit、資料32Bit --*/
			List<byte> result;
			Stat stt = mModbusRTU.WriteMultiRegisters(mDevID, 0x0604, new List<ushort> { param_L, param_H, pos_L, pos_H }, out result);

			/*-- 檢查回傳值，如果設定失敗則直接離開 --*/
			CheckReturnValue(stt, result);
			if (stt != Stat.SUCCESS) return stt;

			/*-- P5-07 觸發，開始動作 --*/
			stt = mModbusRTU.WriteSingleRegister(mDevID, 0x050E, 1, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			return stt;
		}

		/// <summary>[P5-07] 執行已設定好之路徑。對應 <see cref="SetPath(byte, int, ushort, ushort, ushort, bool, bool, Units)"/> 或 <seealso cref="SetPath(byte, int, ushort, ushort, ushort, ushort, bool, bool, MotionCommand)"/></summary>
		/// <param name="index">欲執行的路徑編號，從 1 至 63。</param>
		/// <returns>Status Code</returns>
		public Stat ExecutePath(ushort index) {
			Stat stt = Stat.SUCCESS;

			/*-- 檢查是否可以進行動作 --*/
			CheckStatus();

			/*-- 確認編號索引在合理範圍內 --*/
			if (0 < index && index < 64) {  //index 從 1 ~ 63 (手冊定義)，0 是原點復歸，但 Speed、RampTime、Delay 等是從 0 開始喔!!
				List<byte> result;

				/*-- P5-07 觸發，開始動作 --*/
				stt = mModbusRTU.WriteSingleRegister(mDevID, 0x050E, index, out result);    //index 從 1 開始

				/*-- 檢查回傳值 --*/
				CheckReturnValue(stt, result);

			} else stt = Stat.ER_SYS_INVIDX;

			return stt;
		}

		/// <summary>[P5-07] 停止當前執行中的動作</summary>
		/// <returns>Status Code</returns>
		public Stat MotionStop() {
			/*-- 停止轉轉轉 --*/
			List<byte> result;
			Stat stt = mModbusRTU.WriteSingleRegister(mDevID, 0x050E, 1000, out result);

			/*-- 檢查回傳值 --*/
			CheckReturnValue(stt, result);

			return stt;
		}
		#endregion

		#endregion

	}
}
