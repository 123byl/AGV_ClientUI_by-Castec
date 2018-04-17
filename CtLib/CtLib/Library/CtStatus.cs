using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using CtLib.Forms;
using CtLib.Module.Utility;

namespace CtLib.Library {

	#region  Status Code
	/// <summary>Status Code</summary>
	public enum Stat {
		/// <summary>System Normal Status</summary>
		SUCCESS = 0,

		/* System Information. ( 0 ~ 9999 ) */
		/// <summary>System Information Raised Base</summary>
		CONST_IF_BASE = 1,
		/// <summary>System Information Raised Range</summary>
		CONST_IF_RANGE = 9999,

		/// <summary>[Info] Starting a thread</summary>
		IF_TSKSTR = 100,
		/// <summary>[Info] Thread aborted</summary>
		IF_TSKABT = 101,
		/// <summary>[Info] General information</summary>
		IF_GENERL = 102,
		/// <summary>[Info] User Handle information</summary>
		IF_USERHANDLE = 103,
		/// <summary>[Info] Test Point for Debug</summary>
		IF_TESTPOINT = 104,
		/// <summary>[Info] Function check pass</summary>
		IF_CHECKPASS = 105,
		/// <summary>[Info] Function check fail</summary>
		IF_CHECKFAIL = 106,

		/* Warnings ( 10000 ~ 19999 ) */
		/// <summary>Warning Raised Base</summary>
		CONST_WN_BASE = 10000,
		/// <summary>Warning Raised Range</summary>
		CONST_WN_RANGE = 19999,

		/*-- System Warnings. ( 10000 ~ 10099 ) --*/
		/// <summary>[Warning: System] System Warning</summary>
		WN_SYS = 10001,
		/// <summary>[Warning: System] Operation skipped</summary>
		WN_SYS_OPSKIP = 10002,
		/// <summary>[Warning: System] Operation timeout warning</summary>
		WN_SYS_TIMOUT = 10003,
		/// <summary>[Warning: System] Data not found</summary>
		WN_SYS_NOTFND = 10004,
		/// <summary>[Warning: System] No matched pattern</summary>
		WN_SYS_NOMTCH = 10005,
		/// <summary>[Warning: System] Motion profile setup error</summary>
		WN_SYS_MPFERR = 10006,
		/// <summary>[Warning: System] System in manual mode operation</summary>
		WN_SYS_MANMOD = 10007,
		/// <summary>[Warning: System] Invalid equipment state</summary>
		WN_SYS_ILESTT = 10008,
		/// <summary>[Warning: System] Recipe file does not existed</summary>
		WN_SYS_RCPNET = 10009,
		/// <summary>[Warning: System] Recipe validate failed</summary>
		WN_SYS_RCPVAF = 10010,
		/// <summary>[Warning: System] User cancel an operation</summary>
		WN_SYS_USRCNC = 10011,

		/*-- OpCode Warnings ( 11000 ~ 11099 ) --*/
		/// <summary>[Warning: Opcode] No corresponding opcode</summary>
		WN_OPC_NOOPC = 11001,
		/// <summary>[Warning: Opcode] No opcode found</summary>
		WN_OPC_NOCOD = 11002,

		/*-- Beckhoff ( 11500 ~ 11999 ) --*/
		/// <summary>[Warning: Beckhoff] Warning occur</summary>
		WN_BKF = 11501,
		/// <summary>[Warning: Beckhoff] TwinCAT symbol dose not find</summary>
		WN_BKF_NOSYMLO = 11502,
		/// <summary>[Warning: Beckhoff] Illegal mode</summary>
		WN_BKF_ILLRUNM = 11503,

		/*-- Adept ACE/Robot ( 12000 ~ 12499 ) --*/
		/// <summary>[Warning: ACE] Warning occur</summary>
		WN_ACE = 12001,
		/// <summary>[Warning: ACE] Highpower timeout</summary>
		WN_ACE_PWTMOUT = 12002,
		/// <summary>[Warning: ACE] Access variable or program fail</summary>
		WN_ACE_ACCFAIL = 12003,
		/// <summary>[Warning: ACE] User cancel an operation</summary>
		WN_ACE_USRCNC = 12004,
		/// <summary>[Warning: ACE] Axis out of range</summary>
		WN_ACE_OUTRNG = 12005,
		/// <summary>[Warning: ACE] Invalid Orientation</summary>
		WN_ACE_ORIENT = 12006,

		/*-- Adept Mobile Robot ( 13000 ~ 13499 ) --*/
		/// <summary>[Warning: MR] Warning occur</summary>
		WN_MR_WARNING = 13001,
		/// <summary>[Warning: MR] Power level (Battery) too low</summary>
		WN_MR_PWLOWLV = 13002,

		/*-- Stäubli ( 13500 ~ 13999 ) --*/
		/// <summary>[Warning: Stäubli] Warning occur</summary>
		WN_STBL = 13501,

		/*-- Delta PLC ( 14000 ~ 14499 ) --*/
		/// <summary>[Warning: Delta] Warning occur</summary>
		WN_DELTA = 14001,

		/*-- Wago I/O Module ( 14500 ~ 14999 ) --*/
		/// <summary>[Warning: Wago] Warning occur</summary>
		WN_WGO = 14501,

		/* System Errors ( -10000 ~ -19999 ) */
		/// <summary>System Error Raised Base</summary>
		CONST_ER_BASE = -10000,
		/// <summary>System Error Raised Range</summary>
		CONST_ER_RANGE = -19999,


		/*-- System error ( -10000 ~ -10499 ) --*/
		/// <summary>[System] System error</summary>
		ER_SYSTEM = -10001,
		/// <summary>[System] Illegal directory</summary>
		ER_SYS_ILLDIR = -10002,
		/// <summary>[System] Illegal file name</summary>
		ER_SYS_ILLFIL = -10003,
		/// <summary>[System] File access error</summary>
		ER_SYS_FILACC = -10004,
		/// <summary>[System] File does not exist</summary>
		ER_SYS_NOFILE = -10005,
		/// <summary>[System] Invalid file path</summary>
		ER_SYS_ILFLPH = -10006,
		/// <summary>[System] Invalid setup</summary>
		ER_SYS_INVSET = -10007,
		/// <summary>[System] Duplicate name</summary>
		ER_SYS_NCOLSN = -10008,
		/// <summary>[System] Invalid index</summary>
		ER_SYS_INVIDX = -10009,
		/// <summary>[System] Parameter list index error</summary>
		ER_SYS_PRMIDX = -10010,
		/// <summary>[System] Opcode access error</summary>
		ER_SYS_OPCACC = -10011,
		/// <summary>[System] No config data</summary>
		ER_SYS_NOCONF = -10012,
		/// <summary>[System] Operation timeout error</summary>
		ER_SYS_TIMOUT = -10013,
		/// <summary>[System] Illegal value</summary>
		ER_SYS_ILLVAL = -10014,
		/// <summary>[System] Command not found</summary>
		ER_SYS_NOCOMD = -10015,
		/// <summary>[System] Illegal command</summary>
		ER_SYS_ILLCMD = -10016,
		/// <summary>[System] Too many arguments</summary>
		ER_SYS_MNYARG = -10017,
		/// <summary>[System] Too few arguments</summary>
		ER_SYS_FEWARG = -10018,
		/// <summary>[System] Invalid # of arguments</summary>
		ER_SYS_WRNARG = -10019,
		/// <summary>[System] Invalid argument</summary>
		ER_SYS_INVARG = -10020,
		/// <summary>[System] Kill process in O/S process fail</summary>
		ER_SYS_KILLPR = -10021,
		/// <summary>[System] Construction class fail</summary>
		ER_SYS_CLSCON = -10022,
		/// <summary>[System] Disconstruction class fail</summary>
		ER_SYS_CLSDIC = -10023,
		/// <summary>[System] Without opcode data</summary>
		ER_SYS_OPCDAT = -10034,
		/// <summary>[System] Directory is exitst</summary>
		ER_SYS_NODIRE = -10035,
		/// <summary>[System] System initial fail</summary>
		ER_SYS_INITIL = -10036,
		/// <summary>[System] User trying to sign-in, but with a invalid account</summary>
		ER_SYS_USRACN = -10037,
		/// <summary>[System] User trying to sign-in, but with a invalid password</summary>
		ER_SYS_USRPWD = -10038,

		/*-- Serial error ( -10500 ~ -10999 ) --*/
		/// <summary>[System: Serial] Seral port open fail</summary>
		ER_COM_OPEN = -10501,
		/// <summary>[System: Serial] Sending data to buffer failed</summary>
		ER_COM_SEND = -10502,
		/// <summary>[System: Serial] Receving data from buffer failed</summary>
		ER_COM_RCIV = -10503,
		/// <summary>[System: Serial] Waiting response timeout</summary>
		ER_COM_RTMO = -10504,

		/* Light error  ( -20000 ~ -29999 ) */
		/// <summary>Light Error Raised Base</summary>
		CONST_ER2_BASE = -20000,
		/// <summary>Light Error Raised Range</summary>
		CONST_ER2_RANGE = -29999,

		/*-- Beckhoff light alarm ( -20000 ~ -20499 ) --*/
		/// <summary>[LightErr: Beckhoff] Beckhoff light alarm occur</summary>
		ER2_BKF = -20001,

		/*-- Adept light alarm ( -20500 ~ -20999 ) --*/
		/// <summary>[LightErr: ACE] ACE or Robot light alarm occur</summary>
		ER2_ACE = -20501,

		/*-- Stäubli light alarm ( -21000 ~ -21499 ) --*/
		/// <summary>[LightErr: Stäubli] Stäubli light alarm occur</summary>
		ER2_STBL = -21001,

		/*-- Wago I/O light alarm ( -21500 ~ -21999 ) --*/
		/// <summary>[LightErr: Wago] Wago I/O module light alarm occur</summary>
		ER2_WGO = -21501,

		/*-- Delta PLC light alarm ( -22000 ~ -22499 ) --*/
		/// <summary>[LightErr: Delta] Delta PLC light alarm occur</summary>
		ER2_DELTA = -22001,


		/* Normal error. ( -30000 ~ -39999 ) */
		/// <summary>Normal Error Raised Base</summary>
		CONST_ER3_BASE = -30000,
		/// <summary>Normal Error Raised Range</summary>
		CONST_ER3_RANGE = -39999,

		/*-- Beckhoff normal alarm ( -30000 ~ -30999 ) --*/
		/// <summary>[NormalErr: Beckhoff] Normal alarm occur</summary>
		ER3_BKF = -30001,
		/// <summary>[NormalErr: Beckhoff] Connect fail</summary>
		ER3_BKF_CONT = -30002,

		/*-- Adept normal alarm ( -31000 ~ -31999 ) --*/
		/// <summary>[NormalErr: ACE] Normal alarm occur</summary>
		ER3_ACE = -31001,
		/// <summary>[NormalErr: ACE] Robot is not in COMP mode</summary>
		ER3_ACE_COMP = -31002,
		/// <summary>[NormalErr: ACE] Connect fail</summary>
		ER3_ACE_CONT = -31003,
		/// <summary>[NormalErr: ACE] Task Access Failed</summary>
		ER3_ACE_TSKACC = -31004,
		/// <summary>[NormalErr: ACE] Task doesn't exist</summary>
		ER3_ACE_TSKNON = -31005,

		/*-- Adept Mobile Robot alarm ( -32000 ~ -32999 ) --*/
		/// <summary>[NormalErr: MR] Normal alarm occur</summary>
		ER3_MR = -32001,
		/// <summary>[NormalErr: MR] Invalid cargo</summary>
		ER3_MR_INVCAR = -32002,
		/// <summary>[NormalErr: MR] Connect lost</summary>
		ER3_MR_MORLOS = -32003,
		/// <summary>[NormalErr: MR] Invalid Station</summary>
		ER3_MR_INVSTA = -32004,
		/// <summary>[NormalErr: MR] Route is block</summary>
		ER3_MR_ROUBLK = -32005,
		/// <summary>[NormalErr: MR] Mobile breakdown</summary>
		ER3_MR_BRKDON = -32006,
		/// <summary>[NormalErr: MR] Reply timeout</summary>
		ER3_MR_RPYTMO = -32007,
		/// <summary>[NormalErr: MR] Read RFID tag failed</summary>
		ER3_MR_RFIDRF = -32008,
		/// <summary>[NormalErr: MR] Message reply timeout</summary>
		ER3_MR_MESTMO = -32009,

		/*-- Winsock error ( -33000 ~ -33999 ) --*/
		/// <summary>[NormalErr: Winsock] Invalid port type</summary>
		ER3_WSK_INVPRT = -33001,
		/// <summary>[NormalErr: Winsock] Communication error</summary>
		ER3_WSK_COMERR = -33002,
		/// <summary>[NormalErr: Winsock] Communication port not selected</summary>
		ER3_WSK_NOPORT = -33003,
		/// <summary>[NormalErr: Winsock] Port configuration error</summary>
		ER3_WSK_PRTCFG = -33004,
		/// <summary>[NormalErr: Winsock] Communication timeout</summary>
		ER3_WSK_COMTMO = -33005,
		/// <summary>[NormalErr: Winsock] Unrecognized IP address</summary>
		ER3_WSK_WRNIPA = -33006,
		/// <summary>[NormalErr: Winsock] Cannot connect Ethernet port</summary>
		ER3_WSK_ETHCON = -33007,
		/// <summary>[NormalErr: Winsock] TCP socket listen failed</summary>
		ER3_WSK_TCPLST = -33008,

		/*-- Communication Protocol Error ( -34000 ~ -34999 ) --*/
		/// <summary>[NormalErr: CommProtocol] Unknow Message</summary >
		ER3_CP_UNKMSG = -34001,
		/// <summary>[NormalErr: CommProtocol] Mismatch format</summary >
		ER3_CP_MISFMT = -34002,
		/// <summary>[NormalErr: CommProtocol] Unknow Version</summary >
		ER3_CP_MSGVER = -34003,
		/// <summary>[NormalErr: CommProtocol] Unknow Destination</summary >
		ER3_CP_MSGDET = -34004,
		/// <summary>[NormalErr: CommProtocol] Unknow Source</summary >
		ER3_CP_MSGSRC = -34005,
		/// <summary>[NormalErr: CommProtocol] Dismatch Time</summary >
		ER3_CP_DMTIME = -34006,
		/// <summary>[NormalErr: CommProtocol] Timeout</summary >
		ER3_CP_TIMEOT = -34007,
		/// <summary>[NormalErr: CommProtocol] Command Error</summary >
		ER3_CP_CMDERR = -34008,

		/*-- Stäubli normal alarm ( -35000 ~ -35999 ) --*/
		/// <summary>[NormalErr: Stäubli] Normal alarm occur</summary>
		ER3_STBL = -35001,
		/// <summary>[NormalErr: Stäubli] Invalid Location</summary>
		ER3_STBL_INVLOC = -35002,
		/// <summary>[NormalErr: Stäubli] Reply timeout</summary>
		ER3_STBL_RPYTMO = -35003,

		/*-- Wago I/O Module normal alarm ( -36000 ~ -36999) --*/
		/// <summary>[NormalErr: Wago] Connect with device failed</summary>
		ER3_WGO_COMERR = -36000,
		/// <summary>[NormalErr: Wago] Read data from Wago I/O or Registers failed</summary>
		ER3_WGO_READIO = -36001,
		/// <summary>[NormalErr: Wago] Wrtie value to Wago I/O or Registers failed</summary>
		ER3_WGO_WRITIO = -36002,
		/// <summary>[NormalErr: Wago] Wago device shows alarm</summary>
		ER3_WGO_SYSERR = -36003,

		/*-- Modbus Exceptions ( -37000 ~ -37999 ) --*/
		/// <summary>[NormalErr: Modbus] Master or Slave device report an exception</summary>
		ER3_MB_SLVERR = -37000,
		/// <summary>[NormalErr: Modbus] SerialPort error</summary>
		ER3_MB_COMERR = -37001,
		/// <summary>[NormalErr: Modbus] Can't anlysis received data</summary>
		ER3_MB_UNDATA = -37002,
		/// <summary>[NormalErr: Modbus] Communication Timeout</summary>
		ER3_MB_COMTMO = -37003,

		/*-- Delta PLC Exceptions ( -38000 ~ -38999 ) --*/
		/// <summary>[NormalErr: Delta] Normal error occurred</summary>
		ER3_DELTA = -38000,
		/// <summary>[NormalErr: Delta] Communication error</summary>
		ER3_DELTA_COMERR = -38001,
		/// <summary>[NormalErr: Delta] Connecting/Requesting/Resposing Timeout</summary>
		ER3_DELTA_COMTMO = -38002,
		/// <summary>[NormalErr: Delta] Access variable failed</summary>
		ER3_DELTA_ACCFAIL = -38003,
        
        /*-- Mitsubishi PLC Exceptions ( -39000 ~ -38999 ) --*/
        /// <summary>[NormalErr: Mitsubishi] Normal error occurred</summary>
        ER3_MITSUBISHI = -39000,
        /// <summary>[NormalErr: Mitsubishi] Communication error</summary>
        ER3_MITSUBISHI_COMERR = -39001,
        /// <summary>[NormalErr: Mitsubishi] Connecting/Requesting/Resposing Timeout</summary>
        ER3_MITSUBISHI_COMTMO = -39002,
        /// <summary>[NormalErr: Mitsubishi] Access variable failed</summary>
        ER3_MITSUBISHI_ACCFAIL = -39003,

        /* Fatal error.  ( -40000 ~ -49999 ) */
        /// <summary>Fatal Error Raised Base</summary>
        CONST_ER4_BASE = -40000,
        /// <summary>[FatalErr: System] System doesn't create connection with CPM30</summary>
        ER4_SYS_CPM30CNT = -40004,
        /// <summary>Fatal Error Raised Range</summary>
        CONST_ER4_RANGE = -49999,

		/*-- System program Fatal error ( -40000 ~ -40999 ) --*/
		/// <summary>[FatalErr: System] System E-STOP or Shutdown</summary>
		ER4_SYS_ESTOP = -40001,
		/// <summary>[FatalErr: System] System doesn't create connection with Adept ACE and robots, or ACE already closed.</summary>
		ER4_SYS_ACECNT = -4002,
		/// <summary>[FatalErr: System] System doesn't create connection with Beckhoff PLCs</summary>
		ER4_SYS_BKFCNT = -4003,

		/*-- Beckhoff fatal alarm ( -41000 ~ -41999 ) --*/
		/// <summary>[FatalErr: Beckhoff] Fatal alarm occur</summary>
		ER4_BKF = -41001,
		/// <summary>[FatalErr: Beckhoff] Initial failed</summary>
		ER4_BKF_INIT = -41002,
		/// <summary>[FatalErr: Beckhoff] E-Stop trigger</summary>
		ER4_BKF_ESTOP = -41003,

		/*-- Adept fatal alarm ( -42000 ~ -42999 ) --*/
		/// <summary>[FatalErr: ACE] Fatal alarm occur</summary>
		ER4_ACE = -42001,
		/// <summary>[FatalErr: ACE] Initial failed</summary>
		ER4_ACE_INIT = -42002,
		/// <summary>[FatalErr: ACE] E-Stop trigger</summary>
		ER4_ACE_ESTOP = -42003,
		/// <summary>[FatalErr: ACE] Axis out of range</summary>
		ER4_ACE_OUTRNG = -42004,
		/// <summary>[FatalErr: ACE] Robot Collision</summary>
		ER4_ACE_COLISN = -42005,
		/// <summary>[FatalErr: ACE] Invalid Orientation</summary>
		ER4_ACE_ORIENT = -42006,

		/*-- Adept Mobile Robot fatal alarm ( -43000 ~ -43999 ) --*/
		/// <summary>[FatalErr: MR] Fatal alarm occur</summary>
		ER4_MR = -43001,
		/// <summary>[FatalErr: MR] E-Stop trigger</summary>
		ER4_MR_ESTOP = -43002,

		/*-- Stäubli fatal alarm ( -44000 ~ -44999 ) --*/
		/// <summary>[FatalErr: Stäubli] Fatal alarm occur</summary>
		ER4_STBL = -44001,
		/// <summary>[FatalErr: Stäubli] Initial failed</summary >
		ER4_STBL_INIT = -44002,
		/// <summary>[FatalErr: Stäubli] E-stop trigger</summary>
		ER4_STBL_ESTOP = -44003
	}
	#endregion

	/// <summary>狀態相關回報、紀錄</summary>
	public static class CtStatus {

		#region Version

		/// <summary>CtStatus 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 0.0.0  Chi Sha [2007/02/10]
		///     + Status
		///     
		/// 0.1.0  William [2012/05/09]
		///     \ Status
		///     
		/// 1.0.0  Ahern [2014/07/16]
		///     + 從舊版CtLib搬移
		///     
		/// 1.1.0  Ahern [2015/04/23]
		///     + GetMsgBoxIcon
		///     + GetStackTraceString
		///     + GetEventLogType
		///     \ Report，加入 EventLogType
		///     
		/// 1.1.1  Ahern [2015/05/14]
		///     \ Excpetion 如果為當行宣告，會沒有 StackTrace
		///     
		/// 1.1.2  Ahern [2015/05/31]
		///     \ 改用 CtMsgBox 顯示錯誤
		///     
		/// 1.1.3  Ahern [2015/11/18]
		///     \ 修正字串組合效能
		///     
		/// 1.1.4  Ahern [2016/03/25]
		///		+ Handle Overload
		///		
		/// 1.1.5  Ahern [2016/05/11]
		///		\ 調整 dialog 引數順序
		///		
		/// 1.1.6  Ahern [2016/11/24]
		///		\ 寫入系統事件時，改以 Assembly.FullName 作為來源
		/// 
		/// </code></remarks>
		public static CtVersion Version { get { return new CtVersion(1, 1, 5, "2016/05/11", "Ahern Kuo"); } }

		#endregion

		#region Functions - Method
		/// <summary>取得錯誤代碼層級字串</summary>
		/// <param name="code">錯誤代碼</param>
		/// <returns>層級字串</returns>
		private static string GetMsgLevel(Stat code) {
			string strLv = "Unclassified";

			/* 因為C#裡不像VB.net可以在Select-Case裡用"To"表示一段Range，所以以下均採用If-Else-If */
			if (code == Stat.SUCCESS)
				strLv = "Normal Msg";
			else if ((Stat.CONST_IF_BASE < code) && (code < Stat.CONST_IF_RANGE))
				strLv = "System Info";
			else if ((Stat.CONST_WN_BASE < code) && (code < Stat.CONST_WN_RANGE))
				strLv = "Warning";
			else if ((Stat.CONST_ER_RANGE < code) && (code < Stat.CONST_ER_BASE))
				strLv = "System Error";
			else if ((Stat.CONST_ER2_RANGE < code) && (code < Stat.CONST_ER2_BASE))
				strLv = "Light Error";
			else if ((Stat.CONST_ER3_RANGE < code) && (code < Stat.CONST_ER3_BASE))
				strLv = "Normal Error";
			else if ((Stat.CONST_ER4_RANGE < code) && (code < Stat.CONST_ER4_BASE))
				strLv = "Fatal Error";

			return strLv;
		}

		/// <summary>取得文字對話盒之樣式</summary>
		/// <param name="stt">原始狀態</param>
		/// <returns>相對應的對話盒樣式</returns>
		private static MsgBoxStyle GetMsgBoxIcon(Stat stt) {
			MsgBoxStyle msgicon = MsgBoxStyle.None;

			if (stt == Stat.SUCCESS)
				msgicon = MsgBoxStyle.Information;
			else if ((Stat.CONST_IF_BASE < stt) && (stt < Stat.CONST_IF_RANGE))
				msgicon = MsgBoxStyle.Information;
			else if ((Stat.CONST_WN_BASE < stt) && (stt < Stat.CONST_WN_RANGE))
				msgicon = MsgBoxStyle.Warning;
			else if ((Stat.CONST_ER_RANGE < stt) && (stt < Stat.CONST_ER_BASE))
				msgicon = MsgBoxStyle.Error;
			else if ((Stat.CONST_ER2_RANGE < stt) && (stt < Stat.CONST_ER2_BASE))
				msgicon = MsgBoxStyle.Error;
			else if ((Stat.CONST_ER3_RANGE < stt) && (stt < Stat.CONST_ER3_BASE))
				msgicon = MsgBoxStyle.Error;
			else if ((Stat.CONST_ER4_RANGE < stt) && (stt < Stat.CONST_ER4_BASE))
				msgicon = MsgBoxStyle.Error;

			return msgicon;
		}

		/// <summary>取得 Exception 來源之方法(副程式)名稱</summary>
		/// <param name="ex">欲查詢之 Exception</param>
		/// <returns>最符合的名稱，如沒有則回傳空字串</returns>
		public static string GetMethodName(Exception ex) {
			string strReturn = "";
			StackTrace st = new StackTrace(ex, true);
			List<StackFrame> stFrames = st.GetFrames().ToList().FindAll(data => data.GetFileLineNumber() > 0);
			if (stFrames.Count > 0) strReturn = stFrames.Last().GetMethod().Name;
			return strReturn;
		}

		/// <summary>從 <see cref="StackTrace"/> 中獲取具有行號之方法名稱集合字串</summary>
		/// <param name="stackTrace">欲取得資訊之 StackTrace</param>
		/// <returns>具有行號之方法名稱集合字串</returns>
		private static string GetStackTraceString(StackTrace stackTrace) {
			string strTemp = "InLineException";
			if (stackTrace.FrameCount > 0) {
				List<StackFrame> stFrames = stackTrace.GetFrames().ToList().FindAll(data => data.GetFileLineNumber() > 0);
				if (stFrames != null && stFrames.Count > 0) {
					strTemp = string.Join(
								" | ",
								stFrames.ConvertAll(
									data =>
										string.Format("{0}({1}:{2})", data.GetMethod().Name, CtFile.GetFileName(data.GetFileName(), false), data.GetFileLineNumber().ToString())
								)
							);
				}
			}
			return strTemp;
		}

		/// <summary>從 <see cref="StackTrace"/> 中獲取具有行號之方法名稱集合字串</summary>
		/// <param name="stackTrace">欲取得資訊之 StackTrace</param>
		/// <param name="methodName">擷取到的主方法(副程式)名稱</param>
		/// <returns>具有行號之方法名稱集合字串</returns>
		private static string GetStackTraceString(StackTrace stackTrace, out string methodName) {
			string strTemp = "";
			string strTitle = "InLineException";
			if (stackTrace.FrameCount > 0) {
				List<StackFrame> stFrames = stackTrace.GetFrames().ToList().FindAll(data => data.GetFileLineNumber() > 0);
				if (stFrames != null && stFrames.Count > 0) {
					strTitle = stFrames.Last().GetMethod().Name;
					strTemp = string.Join(
								" | ",
								stFrames.ConvertAll(
									data =>
										string.Format("{0}({1}:{2})", data.GetMethod().Name, CtFile.GetFileName(data.GetFileName(), false), data.GetFileLineNumber().ToString())
								)
							);
				}
			}
			methodName = strTitle;
			return strTemp;
		}

		/// <summary>藉由 Stat 取得相對應的 EventLogType</summary>
		/// <param name="stt">原始狀態</param>
		/// <returns>EventLogType</returns>
		private static EventLogType GetEventLogType(Stat stt) {
			EventLogType logType = EventLogType.Error;
			if (stt == Stat.SUCCESS) logType = EventLogType.Information;
			else if ((Stat.CONST_WN_BASE < stt) && (stt < Stat.CONST_WN_RANGE)) logType = EventLogType.Warning;
			else if (Enum.GetName(typeof(Stat), stt).Contains("_INV") || Enum.GetName(typeof(Stat), stt).Contains("_IL")
					|| stt == Stat.ER_SYS_USRACN || stt == Stat.ER_SYS_USRPWD)
				logType = EventLogType.FailureAudit;
			else logType = EventLogType.Error;
			return logType;
		}

		/// <summary>回傳檔案名稱(不含副檔名)</summary>
		/// <param name="path">欲判斷的路徑</param>
		/// <returns>檔案名稱</returns>
		private static string GetFileNameWithourExtension(this string path) {
			return System.IO.Path.GetFileNameWithoutExtension(path);
		}
		#endregion

		#region Functions - Core

		/// <summary>寫入狀態至記錄檔裡，如有需要可建立對話視窗以提醒使用者</summary>
		/// <param name="stt">Stat</param>
		/// <param name="caption">標題</param>
		/// <param name="msg">說明文字</param>
		/// <param name="eventID">此訊息所附帶之 ID</param>
		/// <param name="filename">檔案名稱。如 "Report.log"，需建立子資料夾可直接在字串中加入如 @"Logs\Error.log" (前方不再帶反斜)</param>
		/// <param name="dialog">是否彈出對話視窗以提醒使用者</param>
		public static void Report(Stat stt, string caption, string msg, bool dialog = false, ushort eventID = 0, string filename = CtConst.FILE_REPORTLOG) {
			string strLv = "";
			string strMsg = "";
			string strTrace = "";
			string strAsm = "";
			try {
				strLv = GetMsgLevel(stt).Trim();

				msg = msg.Replace(Environment.NewLine, "").Replace("\r", "").Replace("\n", " ");

				StackTrace st = new StackTrace(true);
				strTrace = GetStackTraceString(st);

				strMsg = string.Format("<{0}> ({1}:{2}) [{3}] {4} {{ {5} }}", strLv, ((int)stt).ToString(), stt.ToString(), caption, msg, strTrace);

				CtLog.ReportLog(strMsg, filename);

				strAsm = System.Reflection.Assembly.GetCallingAssembly()?.ManifestModule.Name ?? caption;
				EventLogType logType = GetEventLogType(stt);
				CtLog.SystemEventLog(strAsm, string.Format("MethodInfo = {0}\r\nMessage = {1}", strTrace, msg), logType, eventID);
				if (dialog) {
					MsgBoxStyle msgicon = GetMsgBoxIcon(stt);
					CtMsgBox.Show(strLv, msg.Trim(), MsgBoxBtn.OK, msgicon);
				}
			} catch (Exception ex) {
				Report(Stat.ER_SYSTEM, ex);
			}
		}

		/// <summary>寫入狀態至記錄檔裡，並顯示對話視窗以提醒使用者。對話視窗將鎖定指定視窗，直到關閉對話視窗</summary>
		/// <param name="stt">Stat</param>
		/// <param name="caption">標題</param>
		/// <param name="msg">說明文字</param>
		/// <param name="eventID">此訊息所附帶之 ID</param>
		/// <param name="filename">檔案名稱。如 "Report.log"，需建立子資料夾可直接在字串中加入如 @"Logs\Error.log" (前方不再帶反斜)</param>
		/// <param name="handle">欲附加對話視窗的 <see cref="System.Windows.Forms.Control.Handle"/></param>
		public static void Report(Stat stt, string caption, string msg, IntPtr handle, ushort eventID = 0, string filename = CtConst.FILE_REPORTLOG) {
			string strLv = "";
			string strMsg = "";
			string strTrace = "";
			string strAsm = "";
			try {
				strLv = GetMsgLevel(stt).Trim();

				msg = msg.Replace(Environment.NewLine, "").Replace("\r", "").Replace("\n", " ");

				StackTrace st = new StackTrace(true);
				strTrace = GetStackTraceString(st);

				strMsg = string.Format("<{0}> ({1}:{2}) [{3}] {4} {{ {5} }}", strLv, ((int)stt).ToString(), stt.ToString(), caption, msg, strTrace);

				CtLog.ReportLog(strMsg, filename);

				strAsm = System.Reflection.Assembly.GetCallingAssembly()?.ManifestModule.Name ?? caption;
				EventLogType logType = GetEventLogType(stt);
				CtLog.SystemEventLog(strAsm, string.Format("MethodInfo = {0}\r\nMessage = {1}", strTrace, msg), logType, eventID);

				if (handle != IntPtr.Zero) {
					MsgBoxStyle msgicon = GetMsgBoxIcon(stt);
					CtMsgBox.Show(handle, strLv, msg.Trim(), MsgBoxBtn.OK, msgicon); 
				}

			} catch (Exception ex) {
				Report(Stat.ER_SYSTEM, ex);
			}
		}

		/// <summary>寫入例外狀態(<see cref="Exception"/>)至記錄檔裡，如有需要可建立對話視窗以提醒使用者</summary>
		/// <param name="stt">Status Code</param>
		/// <param name="ex">欲紀錄之 Exception</param>
		/// <param name="filename">檔案名稱。如 "Report.log"，需建立子資料夾可直接在字串中加入如 @"Logs\Error.log" (前方不再帶反斜)</param>
		/// <param name="method">回傳由 Exception 取得的觸發錯誤方法名稱</param>
		/// <param name="eventID">此訊息所附帶之 ID</param>
		/// <param name="dialog">是否彈出對話視窗以提醒使用者</param>
		public static void Report(Stat stt, Exception ex, out string method, bool dialog = false, ushort eventID = 0, string filename = CtConst.FILE_REPORTLOG) {
			string strLv = "";
			string strTitle = "";
			string strMsg = "";
			string strTrace = "";
			string strInnerMsg = "";
			string strAsm = "";
			try {
				strLv = GetMsgLevel(stt).Trim();

				StackTrace st = new StackTrace(ex, true);
				strTrace = GetStackTraceString(st, out strTitle);
				strInnerMsg = ex.Message.Replace(Environment.NewLine, " ").Replace("\r", " ").Replace("\n", " ");

				strMsg = string.Format("<{0}> ({1}:{2}) [{3}] {4} {{ {5} }}", strLv, ((int)stt).ToString(), stt.ToString(), strTitle, strInnerMsg, strTrace);

				CtLog.ReportLog(strMsg, filename);

				EventLogType logType = GetEventLogType(stt);
				strInnerMsg = (ex.InnerException == null) ? ex.Message : ex.InnerException.Message;
				strAsm = System.Reflection.Assembly.GetCallingAssembly()?.ManifestModule.Name ?? strTitle;
				CtLog.SystemEventLog(strAsm, string.Format("MethodInfo = {0}\r\nMessage = {1}\r\nStackTrace = {2}", strTrace, strInnerMsg, ex.StackTrace), logType, eventID);

				if (dialog) {
					MsgBoxStyle msgicon = GetMsgBoxIcon(stt);
					CtMsgBox.Show(strLv, strInnerMsg, MsgBoxBtn.OK, msgicon);
				}
			} catch (Exception newEx) {
				Report(Stat.ER_SYSTEM, newEx);
			}
			method = strTitle;
		}

		/// <summary>寫入例外狀態(<see cref="Exception"/>)至記錄檔裡，如有需要可建立對話視窗以提醒使用者。對話視窗將鎖定指定視窗，直到關閉對話視窗</summary>
		/// <param name="stt">Status Code</param>
		/// <param name="ex">欲紀錄之 Exception</param>
		/// <param name="filename">檔案名稱。如 "Report.log"，需建立子資料夾可直接在字串中加入如 @"Logs\Error.log" (前方不再帶反斜)</param>
		/// <param name="method">回傳由 Exception 取得的觸發錯誤方法名稱</param>
		/// <param name="eventID">此訊息所附帶之 ID</param>
		/// <param name="handle">欲附加對話視窗的 <see cref="System.Windows.Forms.Control.Handle"/></param>
		public static void Report(Stat stt, Exception ex, out string method, IntPtr handle, ushort eventID = 0, string filename = CtConst.FILE_REPORTLOG) {
			string strLv = "";
			string strTitle = "";
			string strMsg = "";
			string strTrace = "";
			string strInnerMsg = "";
			string strAsm = "";
			try {
				strLv = GetMsgLevel(stt).Trim();

				StackTrace st = new StackTrace(ex, true);
				strTrace = GetStackTraceString(st, out strTitle);
				strInnerMsg = ex.Message.Replace(Environment.NewLine, " ").Replace("\r", " ").Replace("\n", " ");

				strMsg = string.Format("<{0}> ({1}:{2}) [{3}] {4} {{ {5} }}", strLv, ((int)stt).ToString(), stt.ToString(), strTitle, strInnerMsg, strTrace);

				CtLog.ReportLog(strMsg, filename);
				
				EventLogType logType = GetEventLogType(stt);
				strInnerMsg = (ex.InnerException == null) ? ex.Message : ex.InnerException.Message;
				strAsm = System.Reflection.Assembly.GetCallingAssembly()?.ManifestModule.Name ?? strTitle;
				CtLog.SystemEventLog(strAsm, string.Format("MethodInfo = {0}\r\nMessage = {1}\r\nStackTrace = {2}", strTrace, strInnerMsg, ex.StackTrace), logType, eventID);

				if (handle != IntPtr.Zero) {
					MsgBoxStyle msgicon = GetMsgBoxIcon(stt);
					CtMsgBox.Show(handle, strLv, strInnerMsg, MsgBoxBtn.OK, msgicon);
				}
			} catch (Exception) {
				Report(Stat.ER_SYSTEM, ex);
			}
			method = strTitle;
		}

		/// <summary>寫入例外狀態(<see cref="Exception"/>)至記錄檔裡，如有需要可建立對話視窗以提醒使用者</summary>
		/// <param name="stt">Status Code</param>
		/// <param name="ex">欲紀錄之 Exception</param>
		/// <param name="filename">檔案名稱。如 "Report.log"，需建立子資料夾可直接在字串中加入如 @"Logs\Error.log" (前方不再帶反斜)</param>
		/// <param name="dialog">是否彈出對話視窗以提醒使用者</param>
		/// <param name="eventID">此訊息所附帶之 ID</param>
		public static void Report(Stat stt, Exception ex, bool dialog = false, ushort eventID = 0, string filename = CtConst.FILE_REPORTLOG) {
			string method = "";
			Report(stt, ex, out method, dialog, eventID, filename);
		}

		/// <summary>寫入例外狀態(<see cref="Exception"/>)至記錄檔裡，並顯示對話視窗以提醒使用者。對話視窗將鎖定指定視窗，直到關閉對話視窗</summary>
		/// <param name="stt">Status Code</param>
		/// <param name="ex">欲紀錄之 Exception</param>
		/// <param name="filename">檔案名稱。如 "Report.log"，需建立子資料夾可直接在字串中加入如 @"Logs\Error.log" (前方不再帶反斜)</param>
		/// <param name="handle">欲附加對話視窗的 <see cref="System.Windows.Forms.Control.Handle"/></param>
		/// <param name="eventID">此訊息所附帶之 ID</param>
		public static void Report(Stat stt, Exception ex, IntPtr handle, ushort eventID = 0, string filename = CtConst.FILE_REPORTLOG) {
			string method = "";
			Report(stt, ex, out method, handle, eventID, filename);
		}

		#endregion
	}
}
