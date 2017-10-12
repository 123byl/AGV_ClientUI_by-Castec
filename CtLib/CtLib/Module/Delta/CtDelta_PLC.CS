using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

using CtLib.Library;
using CtLib.Module.SerialPort;
using CtLib.Module.Utility;

namespace CtLib.Module.Delta {

	/// <summary>
	/// 適用於台達電子(Delta)可程式化邏輯控制器(Programmable Logic Controller, PLC)之應用
	/// <para>目前採用 DMT Dynamic Link Library</para>
	/// </summary>
	public class CtDelta_PLC : IDisposable, ICtVersion {

		#region Version

		/// <summary>CtDeltaPLC 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 0.0.0  William [2012/12/02]
		///     + DeltaPLC.vb
		/// 
		/// 0.0.1  Ahern [2015/03/31]
		///     + CtDeltaPLC (Definition/DLL Functions)
		///     
		/// 0.0.2  Johnson [2015/04/02]
		///     + InsertDll
		///     + Eumerations
		///     
		/// 0.0.3  Ahern [2015/04/05]
		///     - Enumerations
		///     - ReadFunction
		///     - WriteFunction
		///     + GetValue
		///     + SetValue
		///     
		/// 0.0.4  Ahern [2015/11/12]
		///     + Tag
		///     
		/// 1.0.0  Ahern [2016/01/18]
		///		\ 改以連線號為主，因應多台同時連線問題
		/// 
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(1, 0, 0, "2016/01/18", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Definitions
		/// <summary>預設的 Delta PLC Product Name</summary>
		private static readonly string DMT_PRODUCT_NAME = "DVP";
		/// <summary>預設的 SerialPort Modbus 模式  (1)ASCII (2)RTU</summary>
		private static readonly int DMT_SERIAL_MODE = 2;
		#endregion

		#region Declaration - Enumeration
		/// <summary>與 Delta PLC 之連接方式</summary>
		public enum ConnectorType : byte {
			/// <summary>SerialPort 連線，以 Modbus RTU 為主</summary>
			/// <remarks>DMT 可以選擇 Modbus ASCII 或 Modbus RTU。雖 ASCII 比較好理解，但 ASCII 需要多一層文字轉資料的轉換，故這邊以 RTU 為主</remarks>
			Serial = 0,
			/// <summary>TCP/IP 連線，以 Modbus TCP 為格式</summary>
			TCP = 1
		}
		#endregion

		#region Declaration - Extern Dlls

		#region Common Dll Import
		/// <summary>當想要將 DMT.dll 置於特定路徑下時需要引入此函式</summary>
		/// <param name="path">DMT.dll 所在路徑，如 @"C:\Program Files (x86)\Delta Industrial Automation\Library\DMT 2.2\Library\C#\DMT.dll"</param>
		/// <returns>指向 DMT.dll 之 Handle</returns>
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr LoadLibrary(string path);

		/// <summary>釋放 DMT.dll 所載入的資源</summary>
		/// <param name="handle">使用 <see cref="LoadLibrary"/> 所取得之 Handle</param>
		/// <returns>是否成功? (<see langword="true"/>)成功  (<see langword="false"/>)失敗</returns>
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern bool FreeLibrary(IntPtr handle);
		#endregion

		#region TCP Socket
		/// <summary>關閉網路通訊</summary>
		/// <param name="commNum">使用 <see cref="OpenModbusTCPSocket"/> 所取得之 Handle</param>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern void CloseSocket(int commNum);

		/// <summary>
		/// 在進行MODBUS TCP網路通訊時，最多可建立32條Socket通訊連線
		/// <para>每條Socket需指派一個連線識別碼 (連線識別碼可使用數值範圍為0~255)</para>
		/// <para>連線識別碼供數據存取時使用 (即在Request/Response時指明由哪條Socket通訊連線進行傳送/接收)</para>
		/// <para>不同的Socket不得使用相同的連線識別碼，唯有在使用CloseSocket關閉某條Socket連線後</para>
		/// <para>該Socket所使用到的連線識別碼才可再次指派給不同的Socket使用</para>
		/// </summary>
		/// <param name="connNum">連線識別碼 (支援多條連線)</param>
		/// <param name="ipAddr">裝置IP位址</param>
		/// <returns>(-1)網路通訊開啟失敗 (Else)網路通訊開啟成功</returns>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern int OpenModbusTCPSocket(int connNum, int ipAddr);

		/// <summary>獲得網路通訊最新的錯誤訊息</summary>
		/// <returns>錯誤代碼</returns>
		/// <remarks>
		///  0    沒有錯誤
		/// <para>01    無效的網路錯誤碼</para>
		/// <para>02    獲得的遠端網路錯誤</para>
		/// <para>03    網路無法建立</para>
		/// <para>04    緩衝區溢位</para>
		/// <para>05    網路無法連接</para>
		/// <para>06    網路已被斷線</para>
		/// <para>07    檔案長度不符預期長度</para>
		/// <para>08    檔案修改時間和日期不符</para>
		/// <para>09    檔案系統發生錯誤</para>
		/// <para>10    獲得的網路選項錯誤</para>
		/// <para>11    無法解析名稱</para>
		/// <para>12    初始化錯誤</para>
		/// <para>13    監聽錯誤</para>
		/// <para>14    獲得的對等名稱錯誤</para>
		/// <para>15    未知的協定請求</para>
		/// <para>16    數據接收錯誤</para>
		/// <para>17    通訊逾時</para>
		/// <para>18    未知的服務請求</para>
		/// <para>19    不正確的網路選項設定</para>
		/// <para>20    獲得的網路名稱錯誤</para>
		/// <para>21    未知的網路型態請求</para>
		/// <para>22    數據傳送錯誤</para>
		/// </remarks>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern int GetLastSocketErr();

		/// <summary>清除網路通訊的錯誤訊息</summary>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern void ResetSocketErr();

		/// <summary>檢查數據是否收到 (毫秒)</summary>
		/// <param name="time">毫秒</param>
		/// <returns>(0)數據未收到或網路通訊錯誤  (1)數據已收到</returns>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		static extern int ReadSelect(int time);
		#endregion

		#region Serial RS-232
		/// <summary>
		/// 在進行MODBUS序列通訊時，序列埠號即為連線識別碼
		/// <para>連線識別碼供數據存取時使用 (即在Request/Response時指明由哪個序列埠進行傳送/接收)</para>
		/// <para>最多可同時開啟32個序列埠 (32條序列通訊連線)</para>
		/// </summary>
		/// <param name="connNum">序列埠號，如： COM"1" (支援多條連線)</param>
		/// <param name="baudRate">鮑率，如： 9600</param>
		/// <param name="dataLength">資料長度，如：7</param>
		/// <param name="parity">同位元，如：'E'</param>
		/// <param name="stopBits">停止位元，如：1</param>
		/// <param name="modbusMode">MODBUS模式  (1)ASCII  (2)RTU</param>
		/// <returns>(-1)序列埠開啟失敗 (Else)序列埠開啟成功</returns>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern int OpenModbusSerial(int connNum, int baudRate, int dataLength, char parity, int stopBits, int modbusMode);

		/// <summary>關閉序列埠通訊</summary>
		/// <param name="connNum">使用 <see cref="OpenModbusSerial"/> 所取得之開啟之 connNum</param>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern void CloseSerial(int connNum);

		/// <summary></summary>
		/// <returns></returns>
		/// <remarks>
		/// 0   沒有錯誤
		/// <para>1   無效的串列錯誤碼</para>
		/// <para>2   無效的鮑率</para>
		/// <para>3   緩衝區錯誤</para>
		/// <para>4   無效的資料位元</para>
		/// <para>5   無效的流量控制</para>
		/// <para>6   無法初始串列埠</para>
		/// <para>7   無效的初始參數</para>
		/// <para>8   無法開啟串列埠</para>
		/// <para>9   無效的同位元</para>
		/// <para>10  數據接收錯誤</para>
		/// <para>11  無效的停止位元</para>
		/// <para>12  數據傳送錯誤</para>
		/// </remarks>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern int GetLastSerialErr();

		/// <summary>清除串列通訊的錯誤訊息</summary>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern void ResetSerialErr();

		#endregion

		#region Modbus
		/// <summary>計算台達電子通訊產品裝置元件所對應的MODBUS位址，同時檢驗存取筆數是否超過元件支援範圍</summary>
		/// <param name="series">台達電子通訊產品系列名稱,目前支援兩個產品系列: "DVP"與 "RTU"</param>
		/// <param name="device">台達電子通訊產品裝置元件名稱,如: "Y0"、"M73"、"D1367"</param>
		/// <param name="count">筆數,檢查從device元件開始存取qty筆是否超出產品裝置元件支援範圍</param>
		/// <returns>(-2)裝置元件存取筆數超出支援範圍  (-1)品系列名稱或裝置元件名稱不正確  (>0)裝置元件所對應的MODBUS位址</returns>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern int DevToAddrW(string series, string device, int count);

		/// <summary>[FC01] 從指定MODBUS位址(Coil)讀取指定筆數資料</summary>
		/// <param name="comm_type">通訊種類  (0)序列埠  (1)乙太網路</param>
		/// <param name="conn_num">序列埠編號或連線識別碼</param>
		/// <param name="slave_addr">站號</param>
		/// <param name="dev_addr">MODBUS位址 (可由DevToAddrA函式計算得到)</param>
		/// <param name="qty">讀取筆數,支援範圍1~2000</param>
		/// <param name="data_r">讀取到的數據  (0)OFF  (1)ON</param>
		/// <param name="req">MODBUS Request字串</param>
		/// <param name="res">MODBUS Response字串</param>
		/// <returns>(-1)資料存取失敗  (qty)資料存取成功</returns>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern int ReadCoilsW(int comm_type, int conn_num, int slave_addr, int dev_addr, int qty, UInt32[] data_r, StringBuilder req, StringBuilder res);

		/// <summary>[FC02] 從指定MODBUS位址(Discrete Input)讀取指定筆數資料</summary>
		/// <param name="comm_type">通訊種類  (0)序列埠  (1)乙太網路</param>
		/// <param name="conn_num">序列埠編號或連線識別碼</param>
		/// <param name="slave_addr">站號</param>
		/// <param name="dev_addr">MODBUS位址 (可由DevToAddrA函式計算得到)</param>
		/// <param name="qty">讀取筆數,支援範圍1~2000</param>
		/// <param name="data_r">讀取到的數據  (0)OFF  (1)ON</param>
		/// <param name="req">MODBUS Request字串</param>
		/// <param name="res">MODBUS Response字串</param>
		/// <returns>(-1)資料存取失敗  (qty)資料存取成功</returns>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern int ReadInputsW(int comm_type, int conn_num, int slave_addr, int dev_addr, int qty, UInt32[] data_r, StringBuilder req, StringBuilder res);

		/// <summary>[FC03] 從指定MODBUS位址(Holding Register)讀取指定筆數資料</summary>
		/// <param name="comm_type">通訊種類  (0)序列埠  (1)乙太網路</param>
		/// <param name="conn_num">序列埠編號或連線識別碼</param>
		/// <param name="slave_addr">站號</param>
		/// <param name="dev_addr">MODBUS位址 (可由DevToAddrA函式計算得到)</param>
		/// <param name="qty">讀取筆數,支援範圍1~125</param>
		/// <param name="data_r">讀取到的數據</param>
		/// <param name="req">MODBUS Request字串</param>
		/// <param name="res">MODBUS Response字串</param>
		/// <returns>(-1)資料存取失敗  (qty)資料存取成功</returns>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern int ReadHoldRegsW(int comm_type, int conn_num, int slave_addr, int dev_addr, int qty, UInt32[] data_r, StringBuilder req, StringBuilder res);

		/// <summary>[FC03] 從指定MODBUS位址(Holding Register 32-Bit)讀取指定筆數資料</summary>
		/// <param name="comm_type">通訊種類  (0)序列埠  (1)乙太網路</param>
		/// <param name="conn_num">序列埠編號或連線識別碼</param>
		/// <param name="slave_addr">站號</param>
		/// <param name="dev_addr">MODBUS位址 (可由DevToAddrA函式計算得到)</param>
		/// <param name="qty">讀取筆數,支援範圍1~50</param>
		/// <param name="data_r">讀取到的數據</param>
		/// <param name="req">MODBUS Request字串</param>
		/// <param name="res">MODBUS Response字串</param>
		/// <returns>(-1)資料存取失敗  (qty)資料存取成功</returns>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern int ReadHoldRegs32W(int comm_type, int conn_num, int slave_addr, int dev_addr, int qty, UInt32[] data_r, StringBuilder req, StringBuilder res);

		/// <summary>[FC04] 從指定MODBUS位址(Input Register)讀取指定筆數資料</summary>
		/// <param name="comm_type">通訊種類  (0)序列埠  (1)乙太網路</param>
		/// <param name="conn_num">序列埠編號或連線識別碼</param>
		/// <param name="slave_addr">站號</param>
		/// <param name="dev_addr">MODBUS位址 (可由DevToAddrA函式計算得到)</param>
		/// <param name="qty">讀取筆數,支援範圍1~125</param>
		/// <param name="data_r">讀取到的數據</param>
		/// <param name="req">MODBUS Request字串</param>
		/// <param name="res">MODBUS Response字串</param>
		/// <returns>(-1)資料存取失敗  (qty)資料存取成功</returns>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern int ReadInputRegsW(int comm_type, int conn_num, int slave_addr, int dev_addr, int qty, UInt32[] data_r, StringBuilder req, StringBuilder res);

		/// <summary>[FC05] 寫入數據到指定MODBUS位址(Coil)當中</summary>
		/// <param name="comm_type">通訊種類  (0)序列埠  (1)乙太網路</param>
		/// <param name="conn_num">序列埠編號或連線識別碼</param>
		/// <param name="slave_addr">站號</param>
		/// <param name="dev_addr">MODBUS位址 (可由DevToAddrA函式計算得到)</param>
		/// <param name="data_w">欲寫入之數據  (0)OFF (非0)ON</param>
		/// <param name="req">MODBUS Request字串</param>
		/// <param name="res">MODBUS Response字串</param>
		/// <returns>(-1)資料存取失敗  (1)資料存取成功</returns>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern int WriteSingleCoilW(int comm_type, int conn_num, int slave_addr, int dev_addr, UInt32 data_w, StringBuilder req, StringBuilder res);

		/// <summary>[FC06] 寫入數據到指定MODBUS位址(Holding Register)當中</summary>
		/// <param name="comm_type">通訊種類  (0)序列埠  (1)乙太網路</param>
		/// <param name="conn_num">序列埠編號或連線識別碼</param>
		/// <param name="slave_addr">站號</param>
		/// <param name="dev_addr">MODBUS位址 (可由DevToAddrA函式計算得到)</param>
		/// <param name="data_w">欲寫入之數據, 支援範圍0~65535 (超過則以65535計)</param>
		/// <param name="req">MODBUS Request字串</param>
		/// <param name="res">MODBUS Response字串</param>
		/// <returns>(-1)資料存取失敗  (1)資料存取成功</returns>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern int WriteSingleRegW(int comm_type, int conn_num, int slave_addr, int dev_addr, UInt32 data_w, StringBuilder req, StringBuilder res);

		/// <summary>[FC06] 寫入數據到指定MODBUS位址(Holding Register 32-Bit)當中</summary>
		/// <param name="comm_type">通訊種類  (0)序列埠  (1)乙太網路</param>
		/// <param name="conn_num">序列埠編號或連線識別碼</param>
		/// <param name="slave_addr">站號</param>
		/// <param name="dev_addr">MODBUS位址 (可由DevToAddrA函式計算得到)</param>
		/// <param name="data_w">欲寫入之數據, 支援範圍0~4294967295</param>
		/// <param name="req">MODBUS Request字串</param>
		/// <param name="res">MODBUS Response字串</param>
		/// <returns>(-1)資料存取失敗  (1)資料存取成功</returns>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern int WriteSingleReg32W(int comm_type, int conn_num, int slave_addr, int dev_addr, UInt32 data_w, StringBuilder req, StringBuilder res);

		/// <summary>[FC15] 從指定MODBUS位址(Coil)開始寫入指定筆數資料</summary>
		/// <param name="comm_type">通訊種類  (0)序列埠  (1)乙太網路</param>
		/// <param name="conn_num">序列埠編號或連線識別碼</param>
		/// <param name="slave_addr">站號</param>
		/// <param name="dev_addr">MODBUS位址 (可由DevToAddrA函式計算得到)</param>
		/// <param name="qty">讀取筆數,支援範圍1~2000</param>
		/// <param name="data_w">欲寫入之數據, 支援範圍0~65535 (超過則以65535計)</param>
		/// <param name="req">MODBUS Request字串</param>
		/// <param name="res">MODBUS Response字串</param>
		/// <returns>(-1)資料存取失敗  (qty)資料存取成功</returns>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern int WriteMultiCoilsW(int comm_type, int conn_num, int slave_addr, int dev_addr, int qty, UInt32[] data_w, StringBuilder req, StringBuilder res);

		/// <summary>[FC16] 從指定MODBUS位址(Holding Register)開始寫入指定筆數資料</summary>
		/// <param name="comm_type">通訊種類  (0)序列埠  (1)乙太網路</param>
		/// <param name="conn_num">序列埠編號或連線識別碼</param>
		/// <param name="slave_addr">站號</param>
		/// <param name="dev_addr">MODBUS位址 (可由DevToAddrA函式計算得到)</param>
		/// <param name="qty">讀取筆數,支援範圍1~2000</param>
		/// <param name="data_w">欲寫入的數據,  支援範圍0~65535 (超過則以65535計)</param>
		/// <param name="req">MODBUS Request字串</param>
		/// <param name="res">MODBUS Response字串</param>
		/// <returns>(-1)資料存取失敗  (qty)資料存取成功</returns>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern int WriteMultiRegsW(int comm_type, int conn_num, int slave_addr, int dev_addr, int qty, UInt32[] data_w, StringBuilder req, StringBuilder res);

		/// <summary>[FC16] 從指定MODBUS位址(Holding Register 32-Bit)開始寫入指定筆數資料</summary>
		/// <param name="comm_type">通訊種類  (0)序列埠  (1)乙太網路</param>
		/// <param name="conn_num">序列埠編號或連線識別碼</param>
		/// <param name="slave_addr">站號</param>
		/// <param name="dev_addr">MODBUS位址 (可由DevToAddrA函式計算得到)</param>
		/// <param name="qty">讀取筆數,支援範圍1~2000</param>
		/// <param name="data_w">欲寫入的數據,  支援範圍0~4294967295</param>
		/// <param name="req">MODBUS Request字串</param>
		/// <param name="res">MODBUS Response字串</param>
		/// <returns>(-1)資料存取失敗  (qty)資料存取成功</returns>
		[DllImport("DMT.dll", CharSet = CharSet.Auto)]
		private static extern int WriteMultiRegs32W(int comm_type, int conn_num, int slave_addr, int dev_addr, int qty, UInt32[] data_w, StringBuilder req, StringBuilder res);

		#endregion


		#endregion

		#region Declaration - Fields
		private byte mSerialNum = 1;
		private IntPtr mDllHandle;
		private object mTag;
		private Dictionary<string, byte> mCommDataColl = new Dictionary<string, byte>();  //因應可有 32 條 Socket
		private byte mSocketNum = 0;  //回傳 Socket number，如果連線時該 IP 已有，則 +1 並回傳
		#endregion

		#region Declaration - Properties
		/// <summary>與 Delta PLC 之連線方式，由 Connect (+2 overload) 決定方式</summary>
		public ConnectorType ConnType { get; private set; }
		/// <summary>取得或設定連線裝置之站號</summary>
		public int SlaveNo { get; set; }
		/// <summary>取得當前連線裝置的 IP 位址或連接埠及其配置連線編號。TKey 為連線編號，TValue 為 IP 位址</summary>
		public Dictionary<string, byte> ConnectionNumber { get { return mCommDataColl.ToDictionary(kvp => kvp.Key, kvp => kvp.Value); } }
		/// <summary>取得或設定物件，其包含控制項相關資料。</summary>
		public object Tag { get { return mTag; } set { mTag = value; } }
		#endregion

		#region Function - Connection

		/// <summary>使用 TCP 連線</summary>
		/// <param name="ipAddr">目標 Delta PLC IP 位址</param>
		/// <param name="commNum">此次所分配到的連線編號，於讀取或寫如時均須帶入該裝置連線編號</param>
		/// <returns>(<see langword="true"/>)成功  (<see langword="false"/>)失敗</returns>
		public bool Connect(string ipAddr, out byte commNum) {
			ConnType = ConnectorType.TCP;
			byte comm = byte.MinValue;
			if (mCommDataColl.ContainsKey(ipAddr)) comm = mCommDataColl[ipAddr];
			else {
				for (byte num = byte.MinValue; num <= byte.MaxValue; num++) {
					if (!mCommDataColl.Values.Contains(num)) {
						comm = num;
						mSocketNum = num; //mSocketNum 只記最後一筆
						break;
					}
					if (num == byte.MaxValue) throw new IndexOutOfRangeException("已無可用的連線");
				}
			}

			int addr = BitConverter.ToInt32(IPAddress.Parse(ipAddr).GetAddressBytes(), 0);
			int ret = OpenModbusTCPSocket(comm, addr);

			if (ret != -1 && !mCommDataColl.ContainsKey(ipAddr)) {
				mCommDataColl.Add(ipAddr, comm);
			}

			commNum = comm;
			return (ret == -1 ? false : true);
		}

		/// <summary>使用序列埠連線</summary>
		/// <param name="portName">連接埠，如 "COM1"</param>
		/// <param name="commNum">此次所分配到的連線編號，於讀取或寫如時均須帶入該裝置連線編號</param>
		/// <param name="baudRate">鮑率，如 115200</param>
		/// <param name="dataBits">資料位元數，如 8</param>
		/// <param name="stopBit">停止位元，如 StopBit.ONE</param>
		/// <param name="parity">同位檢查元，如 Parity.None</param>
		/// <returns>(<see langword="true"/>)成功  (<see langword="false"/>)失敗</returns>
		public bool Connect(string portName, out byte commNum, int baudRate = 9600, int dataBits = 8,
			CtSerial.StopBits stopBit = CtSerial.StopBits.One, CtSerial.Parity parity = CtSerial.Parity.None) {
			ConnType = ConnectorType.Serial;
			mSerialNum = byte.Parse(portName.ToLower().Replace("com", ""));

			if (mCommDataColl.Values.Contains(mSerialNum)) mCommDataColl[portName.ToUpper()] = mSerialNum;
			else mCommDataColl.Add(portName.ToUpper(), mSerialNum);

			char par = ' ';
			switch (parity) {
				case CtSerial.Parity.None:
					par = 'N';
					break;
				case CtSerial.Parity.Odd:
					par = 'O';
					break;
				case CtSerial.Parity.Even:
					par = 'E';
					break;
				case CtSerial.Parity.Mark:
					par = 'M';
					break;
				case CtSerial.Parity.Space:
					par = 'S';
					break;
			}
			int ret = OpenModbusSerial(mSerialNum, baudRate, dataBits, par, (int)stopBit, DMT_SERIAL_MODE);

			commNum = mSerialNum;
			return ret == -1 ? false : true;
		}

		/// <summary>中斷連線</summary>
		public void Disconnect() {
			if (ConnType == ConnectorType.TCP) {
				if (mCommDataColl.Values.Contains(mSocketNum)) {
					CloseSocket(mSocketNum);
					string ip = mCommDataColl.FirstOrDefault(kvp => kvp.Value == mSocketNum).Key;
					mCommDataColl.Remove(ip);
				} else throw new InvalidOperationException("錯誤的連線代號");
			} else {
				if (mCommDataColl.Values.Contains(mSerialNum)) {
					CloseSerial(mSerialNum);
					string serial = mCommDataColl.FirstOrDefault(kvp => kvp.Value == mSerialNum).Key;
					mCommDataColl.Remove(serial);
				} else throw new InvalidOperationException("錯誤的連線代號");
			}
		}

		/// <summary>中斷連線</summary>
		/// <param name="commNum">指定要中斷連線的連線代號</param>
		public void Disconnect(byte commNum) {
			if (mCommDataColl.Values.Contains(commNum)) {
				if (ConnType == ConnectorType.TCP) CloseSocket(commNum);
				else CloseSerial(commNum);
				string commKey = mCommDataColl.FirstOrDefault(kvp => kvp.Value == commNum).Key;
				mCommDataColl.Remove(commKey);
			} else throw new InvalidOperationException("錯誤的連線代號");
		}
		#endregion

		#region Function - Constructor
		/// <summary>建立 CtDeltaPLC</summary>
		public CtDelta_PLC() {
			SlaveNo = 0;
			LoadDll("\\DMT.dll");
		}

		#endregion

		#region Function - Disposable
		/// <summary>中斷與 Delta PLC 之連線，並釋放相關資源</summary>
		public void Dispose() {
			try {
				Dispose(true);
				GC.SuppressFinalize(this);
			} catch (ObjectDisposedException ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
		}

		/// <summary>中斷與 Delta PLC 之連線，並釋放相關資源</summary>
		/// <param name="isDisposing">是否為第一次釋放</param>
		protected virtual void Dispose(bool isDisposing) {
			try {
				if (isDisposing) {
					foreach (KeyValuePair<string,byte> comm in mCommDataColl) {
						Disconnect(comm.Value);
					}
				}
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			} finally {
				if (isDisposing) FreeLibrary(mDllHandle);
			}
		}
		#endregion

		#region Function - Method

		/// <summary>載入 DLL 檔案</summary>
		/// <param name="dllPath">
		/// DMT.dll 路徑
		/// <para>欲載入 CtLib 目錄之 DLL，請輸入如 "DMT.dll"</para>
		/// <para>若為外部資料夾請輸入完整路徑，如 @"C:\Program Files\Delta\Library\C#\DMT.dll"</para>
		/// </param>
		private void LoadDll(string dllPath) {
			string path = dllPath;

			if (!CtFile.IsFileExist(dllPath)) {
				path = Environment.CurrentDirectory;

				/*-- 檢查反斜線 --*/
				if (!dllPath.StartsWith(@"\")) path += @"\";
				path += dllPath;    // obtain the relative path where the DMT.dll resides
			}

			/*-- 載入 DLL --*/
			mDllHandle = LoadLibrary(path);  // explicitly link to DMT.dll
		}

		private void ConvertDataToBool(int count, List<uint> data, out List<bool> bolVal) {
			byte size = sizeof(uint) * 8;
			byte maxCount = 0;
			uint valTemp = 0;
			List<bool> bRet = new List<bool>();
			for (int idx = 0; idx < data.Count; idx++) {
				valTemp = data[idx];
				if (count <= size) maxCount = (byte)count;
				else maxCount = (byte)(((idx * size) < count) ? size : count % size);
				for (byte bit = 0; bit < maxCount; bit++) {
					bRet.Add(((valTemp & 0x01) == 1) ? true : false);
					valTemp >>= 1;
				}
			}
			bolVal = bRet;
		}

		private void ConvertBoolToData(List<bool> bolVal, out List<uint> bytVal) {
			List<uint> valTemp = new List<uint>();
			byte size = sizeof(uint) * 8;
			uint uintTemp = 0;

			byte count = 0;
			for (int idx = bolVal.Count; idx > 0; idx--) {
				uintTemp <<= 1;
				if (bolVal[idx - 1])
					uintTemp++;
				count++;
				if (count == size || idx == bolVal.Count) {
					valTemp.Add(uintTemp);
					uintTemp = 0;
					count = 0;
				}
			}
			valTemp.Reverse();
			bytVal = valTemp;
		}
		#endregion

		#region Function - Core
		/// <summary>取得最新發生的錯誤</summary>
		/// <returns>
		/// Serial -
		/// <para>00  沒有錯誤</para>
		/// <para>01  無效的串列錯誤碼</para>
		/// <para>02  無效的鮑率</para>
		/// <para>03  緩衝區錯誤</para>
		/// <para>04  無效的資料位元</para>
		/// <para>05  無效的流量控制</para>
		/// <para>06  無法初始串列埠</para>
		/// <para>07  無效的初始參數</para>
		/// <para>08  無法開啟串列埠</para>
		/// <para>09  無效的同位元</para>
		/// <para>10  數據接收錯誤</para>
		/// <para>11  無效的停止位元</para>
		/// <para>12  數據傳送錯誤</para>
		/// <para> </para>
		/// <para>Socket - </para>
		/// <para>00  沒有錯誤</para>
		/// <para>01  無效的網路錯誤碼</para>
		/// <para>02  獲得的遠端網路錯誤</para>
		/// <para>03  網路無法建立</para>
		/// <para>04  緩衝區溢位</para>
		/// <para>05  網路無法連接</para>
		/// <para>06  網路已被斷線</para>
		/// <para>07  檔案長度不符預期長度</para>
		/// <para>08  檔案修改時間和日期不符</para>
		/// <para>09  檔案系統發生錯誤</para>
		/// <para>10  獲得的網路選項錯誤</para>
		/// <para>11  無法解析名稱</para>
		/// <para>12  初始化錯誤</para>
		/// <para>13  監聽錯誤</para>
		/// <para>14  獲得的對等名稱錯誤</para>
		/// <para>15  未知的協定請求</para>
		/// <para>16  數據接收錯誤</para>
		/// <para>17  通訊逾時</para>
		/// <para>18  未知的服務請求</para>
		/// <para>19  不正確的網路選項設定</para>
		/// <para>20  獲得的網路名稱錯誤</para>
		/// <para>21  未知的網路型態請求</para>
		/// <para>22  數據傳送錯誤</para>
		/// </returns>
		public int GetLastError() {
			if (ConnType == ConnectorType.Serial) return GetLastSerialErr();
			else return GetLastSocketErr();
		}

		#region Get Value
		/// <summary>取得暫存器(Register)數值，對應 Ladder 應為 "D"、"T"</summary>
		/// <param name="name">欲取得的元件名稱，如 "D200"</param>
		/// <param name="data">取得的數值</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(string name, out uint data) {
			return GetValue(SlaveNo, (ConnType == ConnectorType.Serial ? mSerialNum : mSocketNum), name, out data);
		}

		/// <summary>取得暫存器(Register)數值，對應 Ladder 應為 "D"、"T"</summary>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲取得的元件名稱，如 "D200"</param>
		/// <param name="data">取得的數值</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(byte commNum, string name, out uint data) {
			return GetValue(SlaveNo, commNum, name, out data);
		}

		/// <summary>取得暫存器(Register)數值，對應 Ladder 應為 "D"、"T"</summary>
		/// <param name="slaveNo">目標裝置站號，及 Device Number 或 Slave Number</param>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲取得的元件名稱，如 "D200"</param>
		/// <param name="data">取得的數值</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(int slaveNo, byte commNum, string name, out uint data) {
			int ret = 0;
			int addr = DevToAddrW(DMT_PRODUCT_NAME, name, 1);
			uint[] tempData = new uint[] { 0 };

			StringBuilder req = new StringBuilder(1024);
			StringBuilder res = new StringBuilder(1024);

			if (addr < 0) {
				ret = addr;
				if (addr == -1) throw (new Exception("產品系列名稱或裝置元件名稱不正確"));
				else throw (new Exception("裝置元件存取筆數超出支援範圍"));
			} else {
				ret = ReadHoldRegsW((int)ConnType, commNum, slaveNo, addr, 1, tempData, req, res);
			}

			data = (tempData == null) ? 0 : tempData[0];
			return ret;
		}

		/// <summary>取得暫存器(Register)數值，對應 Ladder 應為 "D"、"T"</summary>
		/// <param name="name">多個欲取得的元件名稱，如 "D200、D201、D202"</param>
		/// <param name="data">取得的數值</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(List<string> name, out List<uint> data) {
			return GetValue(SlaveNo, (ConnType == ConnectorType.Serial ? mSerialNum : mSocketNum), name, out data);
		}

		/// <summary>取得暫存器(Register)數值，對應 Ladder 應為 "D"、"T"</summary>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">多個欲取得的元件名稱，如 "D200、D201、D202"</param>
		/// <param name="data">取得的數值</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(byte commNum, List<string> name, out List<uint> data) {
			return GetValue(SlaveNo, commNum, name, out data);
		}

		/// <summary>取得暫存器(Register)數值，對應 Ladder 應為 "D"、"T"</summary>
		/// <param name="slaveNo">目標裝置站號，及 Device Number 或 Slave Number</param>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">多個欲取得的元件名稱，如 "D200、D201、D202"</param>
		/// <param name="data">取得的數值</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(int slaveNo, byte commNum, List<string> name, out List<uint> data) {
			int ret = 0;
			int addr = -2;

			List<uint> tempResult = new List<uint>();
			StringBuilder req = new StringBuilder(1024);
			StringBuilder res = new StringBuilder(1024);

			foreach (string varName in name) {
				uint[] tempData = new uint[] { 0 };
				addr = DevToAddrW(DMT_PRODUCT_NAME, varName, 1);
				if (addr < 0) {
					ret = addr;
					if (addr == -1) throw (new Exception("產品系列名稱或裝置元件名稱不正確"));
					else throw (new Exception("裝置元件存取筆數超出支援範圍"));
				} else {
					ret += ReadHoldRegsW((int)ConnType, commNum, slaveNo, addr, 1, tempData, req, res);
				}
				if (tempData != null) tempResult.AddRange(tempData);
			}

			data = tempResult;
			return ret;
		}

		/// <summary>取得連續數值，可對應所有 Ladder 元件</summary>
		/// <param name="name">欲取得的連續起始元件名稱。如從 "X0" 讀取 8 個，則此處輸入 "X0"</param>
		/// <param name="count">欲讀取的數量。如從 "X0" 讀取 8 個，則此處輸入 8</param>
		/// <param name="data">取得的數值</param>
		/// <returns>(-2)數量錯誤 (-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(string name, int count, out List<uint> data) {
			return GetValue(SlaveNo, (ConnType == ConnectorType.Serial ? mSerialNum : mSocketNum), name, count, out data);
		}

		/// <summary>取得連續數值，可對應所有 Ladder 元件</summary>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲取得的連續起始元件名稱。如從 "X0" 讀取 8 個，則此處輸入 "X0"</param>
		/// <param name="count">欲讀取的數量。如從 "X0" 讀取 8 個，則此處輸入 8</param>
		/// <param name="data">取得的數值</param>
		/// <returns>(-2)數量錯誤 (-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(byte commNum, string name, int count, out List<uint> data) {
			return GetValue(SlaveNo, commNum, name, count, out data);
		}

		/// <summary>取得連續數值，可對應所有 Ladder 元件</summary>
		/// <param name="slaveNo">目標裝置站號，及 Device Number 或 Slave Number</param>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲取得的連續起始元件名稱。如從 "X0" 讀取 8 個，則此處輸入 "X0"</param>
		/// <param name="count">欲讀取的數量。如從 "X0" 讀取 8 個，則此處輸入 8</param>
		/// <param name="data">取得的數值</param>
		/// <returns>(-2)數量錯誤 (-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(int slaveNo, byte commNum, string name, int count, out List<uint> data) {
			int ret = 0;
			int addr = DevToAddrW(DMT_PRODUCT_NAME, name, count);
			uint[] tempData = new uint[] { 0 };

			StringBuilder req = new StringBuilder(1024);
			StringBuilder res = new StringBuilder(1024);

			if (addr < 0) {
				ret = addr;
				if (addr == -1) throw (new Exception("產品系列名稱或裝置元件名稱不正確"));
				else throw (new Exception("裝置元件存取筆數超出支援範圍"));
			} else {
				if (name.ToLower().Contains("x"))
					ret = ReadInputsW((int)ConnType, commNum, slaveNo, addr, count, tempData, req, res);
				else if (name.ToLower().Contains("y") || name.ToLower().Contains("m"))
					ret = ReadCoilsW((int)ConnType, commNum, slaveNo, addr, count, tempData, req, res);
				else ret = ReadHoldRegsW((int)ConnType, commNum, slaveNo, addr, count, tempData, req, res);
				//小弟推測 D 應該適用 ReadHoldRegsW，而 T 則不確定能讀取啥，屆時還請協助補上！還有 ReadHoldRegs32W 也請協助測試
			}

			data = (tempData == null) ? null : tempData.ToList();
			return ret;
		}

		/// <summary>取得 Bit 數值，對應 Ladder 應為 "X"、"Y"、"M"</summary>
		/// <param name="name">欲取得的元件名稱。如 "Y0"</param>
		/// <param name="data">取得的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(string name, out bool data) {
			return GetValue(SlaveNo, (ConnType == ConnectorType.Serial ? mSerialNum : mSocketNum), name, out data);
		}

		/// <summary>取得 Bit 數值，對應 Ladder 應為 "X"、"Y"、"M"</summary>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲取得的元件名稱。如 "Y0"</param>
		/// <param name="data">取得的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(byte commNum, string name, out bool data) {
			return GetValue(SlaveNo, commNum, name, out data);
		}

		/// <summary>取得 Bit 數值，對應 Ladder 應為 "X"、"Y"、"M"</summary>
		/// <param name="slaveNo">目標裝置站號，及 Device Number 或 Slave Number</param>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲取得的元件名稱。如 "Y0"</param>
		/// <param name="data">取得的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(int slaveNo, byte commNum, string name, out bool data) {
			int ret = 0;
			int addr = DevToAddrW(DMT_PRODUCT_NAME, name, 1);
			uint[] tempData = new uint[] { 0 };

			StringBuilder req = new StringBuilder(1024);
			StringBuilder res = new StringBuilder(1024);

			if (addr < 0) {
				ret = addr;
				if (addr == -1) throw (new Exception("產品系列名稱或裝置元件名稱不正確"));
				else throw (new Exception("裝置元件存取筆數超出支援範圍"));
			} else {
				if (name.ToLower().Contains("x"))
					ret = ReadInputsW((int)ConnType, commNum, slaveNo, addr, 1, tempData, req, res);
				else if (name.ToLower().Contains("y") || name.ToLower().Contains("m"))
					ret = ReadCoilsW((int)ConnType, commNum, slaveNo, addr, 1, tempData, req, res);
			}

			data = (tempData == null) ? false : CtConvert.CBool(tempData[0]);
			return ret;
		}

		/// <summary>取得多個 Bit 數值，對應 Ladder 應為 "X"、"Y"、"M"</summary>
		/// <param name="name">欲取得的元件名稱。如 "Y0"、"X0"、"M0"</param>
		/// <param name="data">取得的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(List<string> name, out List<bool> data) {
			return GetValue(SlaveNo, (ConnType == ConnectorType.Serial ? mSerialNum : mSocketNum), name, out data);
		}

		/// <summary>取得多個 Bit 數值，對應 Ladder 應為 "X"、"Y"、"M"</summary>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲取得的元件名稱。如 "Y0"、"X0"、"M0"</param>
		/// <param name="data">取得的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(byte commNum, List<string> name, out List<bool> data) {
			return GetValue(SlaveNo, commNum, name, out data);
		}

		/// <summary>取得多個 Bit 數值，對應 Ladder 應為 "X"、"Y"、"M"</summary>
		/// <param name="slaveNo">目標裝置站號，及 Device Number 或 Slave Number</param>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲取得的元件名稱。如 "Y0"、"X0"、"M0"</param>
		/// <param name="data">取得的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(int slaveNo, byte commNum, List<string> name, out List<bool> data) {
			int ret = 0;
			int addr = -2;

			List<uint> tempResult = new List<uint>();
			StringBuilder req = new StringBuilder(1024);
			StringBuilder res = new StringBuilder(1024);

			foreach (string varName in name) {
				uint[] tempData = new uint[] { 0 };
				addr = DevToAddrW(DMT_PRODUCT_NAME, varName, 1);
				if (addr < 0) {
					ret = addr;
					if (addr == -1) throw (new Exception("產品系列名稱或裝置元件名稱不正確"));
					else throw (new Exception("裝置元件存取筆數超出支援範圍"));
				} else {
					if (varName.ToLower().Contains("x"))
						ret += ReadInputsW((int)ConnType, commNum, slaveNo, addr, 1, tempData, req, res);
					else if (varName.ToLower().Contains("y") || varName.ToLower().Contains("m"))
						ret += ReadCoilsW((int)ConnType, commNum, slaveNo, addr, 1, tempData, req, res);
				}
				if (tempData != null) tempResult.AddRange(tempData);
			}

			data = tempResult.ConvertAll(val => CtConvert.CBool(val));
			return ret;
		}

		/// <summary>取得連續多個 Bit 數值，對應 Ladder 應為 "X"、"Y"、"M"</summary>
		/// <param name="name">欲連續取得的起始元件名稱。如 "M0" 開始讀取 9 個，則此處帶入 "M0"</param>
		/// <param name="count">連續讀取的數量。如 "M0" 開始讀取 9 個，則此處帶入 9</param>
		/// <param name="data">取得的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(string name, int count, out List<bool> data) {
			return GetValue(SlaveNo, (ConnType == ConnectorType.Serial ? mSerialNum : mSocketNum), name, count, out data);
		}

		/// <summary>取得連續多個 Bit 數值，對應 Ladder 應為 "X"、"Y"、"M"</summary>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲連續取得的起始元件名稱。如 "M0" 開始讀取 9 個，則此處帶入 "M0"</param>
		/// <param name="count">連續讀取的數量。如 "M0" 開始讀取 9 個，則此處帶入 9</param>
		/// <param name="data">取得的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(byte commNum, string name, int count, out List<bool> data) {
			return GetValue(SlaveNo, commNum, name, count, out data);
		}

		/// <summary>取得連續多個 Bit 數值，對應 Ladder 應為 "X"、"Y"、"M"</summary>
		/// <param name="slaveNo">目標裝置站號，及 Device Number 或 Slave Number</param>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲連續取得的起始元件名稱。如 "M0" 開始讀取 9 個，則此處帶入 "M0"</param>
		/// <param name="count">連續讀取的數量。如 "M0" 開始讀取 9 個，則此處帶入 9</param>
		/// <param name="data">取得的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功讀取的筆數</returns>
		public int GetValue(int slaveNo, byte commNum, string name, int count, out List<bool> data) {
			int ret = 0;
			int addr = DevToAddrW(DMT_PRODUCT_NAME, name, count);
			uint[] tempData = new uint[] { 0 };

			StringBuilder req = new StringBuilder(1024);
			StringBuilder res = new StringBuilder(1024);

			if (addr < 0) {
				ret = addr;
				if (addr == -1) throw (new Exception("產品系列名稱或裝置元件名稱不正確"));
				else throw (new Exception("裝置元件存取筆數超出支援範圍"));
			} else {
				if (name.ToLower().Contains("x"))
					ret = ReadInputsW((int)ConnType, commNum, slaveNo, addr, count, tempData, req, res);
				else if (name.ToLower().Contains("y") || name.ToLower().Contains("m"))
					ret = ReadCoilsW((int)ConnType, commNum, slaveNo, addr, count, tempData, req, res);
			}

			/*-- 不確定回傳型態為何，如果是每筆資料丟到 uint[] 裡，請試試下面的方法  || M0,3 => uint[] { 0, 1, 0} 這種 --*/
			data = (tempData == null) ? null : tempData.ToList().ConvertAll(val => CtConvert.CBool(val));

			/*-- 不確定回傳型態為何，如果是以 Bit 丟到 uint 裡，請試試下面的方法  || M0,3 => uint[] { 2 }  //010(Binary) = 2(Decimal) --*/
			//List<bool> tempResult;
			//ConvertDataToBool(count, tempData.ToList(),out tempResult);
			//data = tempResult;

			return ret;
		}
		#endregion

		#region Set Value
		/// <summary>設定單一 Bit，對應 Ladder 應為 "Y"、"M"</summary>
		/// <param name="name">欲更改數值之元件名稱。如 "Y3"</param>
		/// <param name="stt">欲更改的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(string name, bool stt) {
			return SetValue(SlaveNo, (ConnType == ConnectorType.Serial ? mSerialNum : mSocketNum), name, stt);
		}

		/// <summary>設定單一 Bit，對應 Ladder 應為 "Y"、"M"</summary>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲更改數值之元件名稱。如 "Y3"</param>
		/// <param name="stt">欲更改的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(byte commNum, string name, bool stt) {
			return SetValue(SlaveNo, commNum, name, stt);
		}

		/// <summary>設定單一 Bit，對應 Ladder 應為 "Y"、"M"</summary>
		/// <param name="slaveNo">目標裝置站號，及 Device Number 或 Slave Number</param>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲更改數值之元件名稱。如 "Y3"</param>
		/// <param name="stt">欲更改的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(int slaveNo, byte commNum, string name, bool stt) {
			int ret = 0;
			int addr = DevToAddrW(DMT_PRODUCT_NAME, name, 1);

			StringBuilder req = new StringBuilder(1024);
			StringBuilder res = new StringBuilder(1024);

			if (addr < 0) {
				ret = addr;
				if (addr == -1) throw (new Exception("產品系列名稱或裝置元件名稱不正確"));
				else throw (new Exception("裝置元件存取筆數超出支援範圍"));
			} else {
				ret = WriteSingleCoilW((int)ConnType, commNum, slaveNo, addr, (uint)((stt) ? 1 : 0), req, res);
			}
			return ret;
		}

		/// <summary>設定單一暫存器(Register)，對應 Ladder 應為 "D"、"T"</summary>
		/// <param name="name">欲更改數值之元件名稱。如 "D200"</param>
		/// <param name="value">欲更改的數值</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(string name, uint value) {
			return SetValue(SlaveNo, (ConnType == ConnectorType.Serial ? mSerialNum : mSocketNum), name, value);
		}

		/// <summary>設定單一暫存器(Register)，對應 Ladder 應為 "D"、"T"</summary>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲更改數值之元件名稱。如 "D200"</param>
		/// <param name="value">欲更改的數值</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(byte commNum, string name, uint value) {
			return SetValue(SlaveNo, commNum, name, value);
		}

		/// <summary>設定單一暫存器(Register)，對應 Ladder 應為 "D"、"T"</summary>
		/// <param name="slaveNo">目標裝置站號，及 Device Number 或 Slave Number</param>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲更改數值之元件名稱。如 "D200"</param>
		/// <param name="value">欲更改的數值</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(int slaveNo, byte commNum, string name, uint value) {
			int ret = 0;
			int addr = DevToAddrW(DMT_PRODUCT_NAME, name, 1);

			StringBuilder req = new StringBuilder(1024);
			StringBuilder res = new StringBuilder(1024);

			if (addr < 0) {
				ret = addr;
				if (addr == -1) throw (new Exception("產品系列名稱或裝置元件名稱不正確"));
				else throw (new Exception("裝置元件存取筆數超出支援範圍"));
			} else {
				ret = WriteSingleRegW((int)ConnType, commNum, slaveNo, addr, value, req, res);
			}
			return ret;
		}

		/// <summary>設定多個 Bit，對應 Ladder 應為 "Y"、"M"</summary>
		/// <param name="name">欲更改數值之元件名稱。如 "Y5"、"M6"</param>
		/// <param name="stt">欲更改的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(List<string> name, List<bool> stt) {
			return SetValue(SlaveNo, (ConnType == ConnectorType.Serial ? mSerialNum : mSocketNum), name, stt);
		}

		/// <summary>設定多個 Bit，對應 Ladder 應為 "Y"、"M"</summary>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲更改數值之元件名稱。如 "Y5"、"M6"</param>
		/// <param name="stt">欲更改的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(byte commNum, List<string> name, List<bool> stt) {
			return SetValue(SlaveNo, commNum, name, stt);
		}

		/// <summary>設定多個 Bit，對應 Ladder 應為 "Y"、"M"</summary>
		/// <param name="slaveNo">目標裝置站號，及 Device Number 或 Slave Number</param>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲更改數值之元件名稱。如 "Y5"、"M6"</param>
		/// <param name="stt">欲更改的狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(int slaveNo, byte commNum, List<string> name, List<bool> stt) {
			int ret = 0;
			int addr = -2;

			StringBuilder req = new StringBuilder(1024);
			StringBuilder res = new StringBuilder(1024);

			for (int idx = 0; idx < name.Count; idx++) {
				addr = DevToAddrW(DMT_PRODUCT_NAME, name[idx], 1);
				if (addr < 0) {
					ret = addr;
					if (addr == -1) throw (new Exception("產品系列名稱或裝置元件名稱不正確"));
					else throw (new Exception("裝置元件存取筆數超出支援範圍"));
				} else {
					ret += WriteSingleCoilW((int)ConnType, commNum, slaveNo, addr, (uint)((stt[idx]) ? 1 : 0), req, res);
				}
			}
			return ret;
		}

		/// <summary>設定多個暫存器(Register)，對應 Ladder 應為 "D"、"T"</summary>
		/// <param name="name">欲更改數值之元件名稱。如 "D200"、"D201"</param>
		/// <param name="value">欲更改的數值</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(List<string> name, List<uint> value) {
			return SetValue(SlaveNo, (ConnType == ConnectorType.Serial ? mSerialNum : mSocketNum), name, value);
		}

		/// <summary>設定多個暫存器(Register)，對應 Ladder 應為 "D"、"T"</summary>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲更改數值之元件名稱。如 "D200"、"D201"</param>
		/// <param name="value">欲更改的數值</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(byte commNum, List<string> name, List<uint> value) {
			return SetValue(SlaveNo, commNum, name, value);
		}

		/// <summary>設定多個暫存器(Register)，對應 Ladder 應為 "D"、"T"</summary>
		/// <param name="slaveNo">目標裝置站號，及 Device Number 或 Slave Number</param>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲更改數值之元件名稱。如 "D200"、"D201"</param>
		/// <param name="value">欲更改的數值</param>
		/// <returns>(-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(int slaveNo, byte commNum, List<string> name, List<uint> value) {
			int ret = 0;
			int addr = -2;

			StringBuilder req = new StringBuilder(1024);
			StringBuilder res = new StringBuilder(1024);

			for (int idx = 0; idx < name.Count; idx++) {
				addr = DevToAddrW(DMT_PRODUCT_NAME, name[idx], 1);
				if (addr < 0) {
					ret = addr;
					if (addr == -1) throw (new Exception("產品系列名稱或裝置元件名稱不正確"));
					else throw (new Exception("裝置元件存取筆數超出支援範圍"));
				} else {
					ret += WriteSingleRegW((int)ConnType, commNum, slaveNo, addr, value[idx], req, res);
				}
			}
			return ret;
		}

		/// <summary>設定連續多個 Bit，對應 Ladder 應為 "Y"、"M"</summary>
		/// <param name="name">欲更改連續數值之起始元件名稱。如 "Y5" 開始改寫 7 個 Bit，則此處帶入 "Y5"</param>
		/// <param name="stt">對應各元件之狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-2)筆數錯誤  (-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(string name, List<bool> stt) {
			return SetValue(SlaveNo, (ConnType == ConnectorType.Serial ? mSerialNum : mSocketNum), name, stt);
		}

		/// <summary>設定連續多個 Bit，對應 Ladder 應為 "Y"、"M"</summary>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲更改連續數值之起始元件名稱。如 "Y5" 開始改寫 7 個 Bit，則此處帶入 "Y5"</param>
		/// <param name="stt">對應各元件之狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-2)筆數錯誤  (-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(byte commNum, string name, List<bool> stt) {
			return SetValue(SlaveNo, commNum, name, stt);
		}

		/// <summary>設定連續多個 Bit，對應 Ladder 應為 "Y"、"M"</summary>
		/// <param name="slaveNo">目標裝置站號，及 Device Number 或 Slave Number</param>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲更改連續數值之起始元件名稱。如 "Y5" 開始改寫 7 個 Bit，則此處帶入 "Y5"</param>
		/// <param name="stt">對應各元件之狀態  (<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
		/// <returns>(-2)筆數錯誤  (-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(int slaveNo, byte commNum, string name, List<bool> stt) {
			int ret = 0;
			int addr = DevToAddrW(DMT_PRODUCT_NAME, name, stt.Count);

			StringBuilder req = new StringBuilder(1024);
			StringBuilder res = new StringBuilder(1024);

			if (addr < 0) {
				ret = addr;
				if (addr == -1) throw (new Exception("產品系列名稱或裝置元件名稱不正確"));
				else throw (new Exception("裝置元件存取筆數超出支援範圍"));
			} else {
				List<uint> tempValue;
				ConvertBoolToData(stt, out tempValue);
				ret = WriteMultiCoilsW((int)ConnType, commNum, slaveNo, addr, tempValue.Count, tempValue.ToArray(), req, res);
			}
			return ret;
		}

		/// <summary>設定連續多個暫存器(Register)，對應 Ladder 應為 "D"、"T"</summary>
		/// <param name="name">欲更改連續數值之起始之元件名稱。如 "D200" 開始改寫 12 個 Register，則此處帶入 "D200"</param>
		/// <param name="value">對應各元件之數值</param>
		/// <returns>(-2)筆數錯誤  (-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(string name, List<uint> value) {
			return SetValue(SlaveNo, (ConnType == ConnectorType.Serial ? mSerialNum : mSocketNum), name, value);
		}

		/// <summary>設定連續多個暫存器(Register)，對應 Ladder 應為 "D"、"T"</summary>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="name">欲更改連續數值之起始之元件名稱。如 "D200" 開始改寫 12 個 Register，則此處帶入 "D200"</param>
		/// <param name="value">對應各元件之數值</param>
		/// <returns>(-2)筆數錯誤  (-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(byte commNum, string name, List<uint> value) {
			return SetValue(SlaveNo, commNum, name, value);
		}

		/// <summary>設定連續多個暫存器(Register)，對應 Ladder 應為 "D"、"T"</summary>
		/// <param name="commNum">目標連線編號，於 Connect 時所取得的 <see cref="byte"/> 編號</param>
		/// <param name="slaveNo">目標裝置站號，及 Device Number 或 Slave Number</param>
		/// <param name="name">欲更改連續數值之起始之元件名稱。如 "D200" 開始改寫 12 個 Register，則此處帶入 "D200"</param>
		/// <param name="value">對應各元件之數值</param>
		/// <returns>(-2)筆數錯誤  (-1)元件名稱不正確  (Else)成功寫入的筆數</returns>
		public int SetValue(int slaveNo, byte commNum, string name, List<uint> value) {
			int ret = 0;
			int addr = DevToAddrW(DMT_PRODUCT_NAME, name, value.Count);

			StringBuilder req = new StringBuilder(1024);
			StringBuilder res = new StringBuilder(1024);

			if (addr < 0) {
				ret = addr;
				if (addr == -1) throw (new Exception("產品系列名稱或裝置元件名稱不正確"));
				else throw (new Exception("裝置元件存取筆數超出支援範圍"));
			} else {
				ret = WriteMultiRegsW((int)ConnType, commNum, slaveNo, addr, value.Count, value.ToArray(), req, res);
			}
			return ret;
		}
		#endregion
		#endregion
	}
}
