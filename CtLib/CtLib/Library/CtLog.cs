using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using CtLib.Module.Utility;

namespace CtLib.Library {
	/// <summary>
	/// 記錄檔相關操作
	/// <para>存檔路徑部分，將在每次進入副程式時讀取 Resource 之路徑 (Settings.settings)</para>
	/// <para>方法採用 Queue，由一個 執行緒(<see cref="Thread"/>) 來持續檢查 Queue，有需要寫入時將進行寫入動作</para>
	/// </summary>
	/// <remarks>
	/// 目前 Log 分類
	/// <para>1. TraceLog - 用於操作紀錄，如 "設備開始"、"設備停止" 等相關訊息</para>
	/// <para>2. ErrorLog - 用於設備錯誤，如 "Sensor無法感應到"、"馬達 Servo 失敗" 等訊息</para>
	/// <para>3. ReportLog - 自訂義的紀錄，如 "產量紀錄"。 為承襲傳統，CtStatus.Report 也使用此紀錄</para>
	/// </remarks>
	public sealed class CtLog : IDisposable {

		#region Version

		/// <summary>CtLog 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0  Ahern [2014/07/15]
		///     + 將讀寫Log之常用Function建立至此
		///     
		/// 1.0.1  Ahern [2015/02/11]
		///     \ Log 路徑改採 CtDefaultPath 以讀取 Settings.settings 之路徑
		///     
		/// 1.1.0  Ahern [2015/04/23]
		///     + SystemEventLog
		/// 
		/// 1.1.1  Ahern [2015/11/18]
		///     \ 改以 string.Format 做字串組合
		///     \ Queue 改成 ConcurrentQueue
		///     
		/// 1.2.0  Ahern [2016/07/09]
		///		\ 改以 Public Instance + Private Static 作法，可套用 Dispose 與 ThreadExit 事件
		///		  讓 Thread 可完成所有 Queue 後再關閉，避免 Log 沒記錄到
		///		- 不必要的 SaveLog 與 WriteLine
		///		+ 採用 Dictionary 紀錄各檔，避免重複開檔造成硬碟負擔
		///		+ 採用 List 將 Queue 裡面資料先撈出來，避免後續 Queue 塞車
		///		+ 可選是否等待多筆後再進行寫入(預設關閉)
		///		
		/// 1.2.1  Ahern [2016/09/08]
		///		+ TraceLog 與 ReportLog 添加含有時間訊息之方法
		///		
		/// 1.2.2  Ahern [2016/10/13]
		///		\ 取消保持檔案開啟，改回寫入即關閉方式，方便人員查看
		///     
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(1, 2, 2, "2016/10/13", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Definitions
		/// <summary>記錄檔佇列監控執行緒每回合之延遲時間</summary>
		private static readonly int THREAD_DELAY_TIME = 100;
		/// <summary>記錄檔佇列監控執行緒於應用程式關閉前所等待的回合數，用於避免紀錄檔尚未加入完畢</summary>
		private static readonly int EXIT_DELAY_COUNT = 5;
		/// <summary>是否等待特定筆數後才進行寫入動作 (<see langword="true"/>)等待多於 <see cref="DELAY_WRITE_COUNT"/> 後才進行寫入  (<see langword="false"/>)直接寫入</summary>
		private static readonly bool DELAY_WRITE = false;
		/// <summary>等待筆數</summary>
		private static readonly int DELAY_WRITE_COUNT = 20;
		#endregion

		#region Declaration - Fields
		/// <summary>靜態處理物件</summary>
		private static CtLog mStaticInst;
		/// <summary>[Thread] 儲存記錄檔</summary>
		private Thread mThrQueMonitor;
		/// <summary>[Queue] 先進先出之記錄檔佇列</summary>
		private ConcurrentQueue<LogMessage> mLogQue;
		/// <summary>[Flag] 指出目前是否已進入處置階段，由 Dispose 或 Application.ThreadExit 觸發</summary>
		private bool mFlag_Dispose = false;
		#endregion

		#region Declaration - Properties
		/// <summary>取得剩餘未記錄的訊息筆數</summary>
		public int QueueCount { get { return mLogQue.Count; } }
		#endregion

		#region Declaration - Supported Classes

		/// <summary>記錄檔訊息，含路徑、訊息與時間點</summary>
		private class LogMessage {
			private DateTime mTime = DateTime.Now;
			private string mPath = string.Empty;
			private string mContent = string.Empty;

			/// <summary>取得該訊息時間點</summary>
			public DateTime LogTime { get { return mTime; } }
			/// <summary>取得檔案路徑</summary>
			public string FilePath { get { return mPath; } }
			/// <summary>取得紀錄訊息</summary>
			public string InfoText { get { return mContent; } }
			/// <summary>建構元，建立含路徑、時間與訊息之類別</summary>
			/// <param name="time">該訊息時間點</param>
			/// <param name="filePath">檔案路徑</param>
			/// <param name="text">紀錄文字</param>
			public LogMessage(DateTime time, string filePath, string text) {
				mTime = time;
				mPath = filePath;
				mContent = text;
			}
			/// <summary>建構元，建立含路徑、時間與訊息之類別</summary>
			/// <param name="filePath">檔案路徑</param>
			/// <param name="text">紀錄文字</param>
			public LogMessage(string filePath, string text) {
				mPath = filePath;
				mContent = text;
			}
			/// <summary>將 <see cref="LogMessage"/> 以 "[時間] 內容" 的方式呈現</summary>
			/// <returns>訊息內容</returns>
			public override string ToString() {
				return string.Format("[{0}] {1}", mTime.ToString("HH:mm:ss.fff"), mContent);
			}
		}

		#endregion

		#region Function - Constructors
		/// <summary>建構記錄檔紀錄器，產生佇列進行記錄檔寫入動作</summary>
		public CtLog() {
			/*-- 訂閱 Application 關閉事件 --*/
			Application.ThreadExit += OnThreadExit;

			/*-- 初始化變數 --*/
			mFlag_Dispose = false;
			mLogQue = new ConcurrentQueue<LogMessage>();

			/*-- 執行緒 --*/
			if (DELAY_WRITE)
				CtThread.CreateThread(ref mThrQueMonitor, "CtLog_QueueMonitor", tsk_LogQue_WaitCount, false);
			else
				CtThread.CreateThread(ref mThrQueMonitor, "CtLog_QueueMonitor", tsk_LogQue_Direct, false);
		}

		#endregion

		#region Function - Private Methods

		/// <summary>建立記錄檔類別，並加入佇列</summary>
		/// <param name="path">檔案路徑</param>
		/// <param name="msg">訊息內容</param>
		/// <param name="time">訊息時間</param>
		private static void SaveLog(string path, string msg, DateTime time) {
			/*-- 確認是由介面呼叫而非 DLL --*/
			if (AppDomain.CurrentDomain.IsDefaultAppDomain()) {

				/* 確保 Static Instance 有東西 */
				if (mStaticInst == null) mStaticInst = new CtLog();

				/* 加入佇列 */
				mStaticInst.Enqueue(path, msg, time);

			} else {    /*-- 若為 DLL 或其他參考所呼叫，則直接寫檔即可 --*/
				
				LogMessage log = new LogMessage(time, path, msg);
				try {
					CtFile.CreateDirectory(path);
					using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8)) {
						sw.WriteLine(log.ToString());
					}
				} catch (Exception ex) {
					StaticSpecialLog(ex, log);
				}

			}
		}

		/// <summary>組合記錄檔檔名為訂定的 CASTEC 資料夾路徑</summary>
		/// <param name="fileName">記錄檔檔名</param>
		/// <returns>符合 CASTEC 資料夾之路徑</returns>
		private static string GetFilePath(string fileName) {
			StringBuilder strPath = new StringBuilder(CtDefaultPath.GetPath(SystemPath.Log));
			strPath.Append(CtFile.BackSlash(DateTime.Now.ToString("yyyyMMdd")));
			strPath.Append(fileName);

			return strPath.ToString();
		}

		/// <summary>用於處理 mThrQueMonitor 之內部錯誤</summary>
		/// <param name="ex">例外狀況</param>
		private void SpecialLog(Exception ex) {
			string path = GetFilePath("LibCrash.log");
			using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8)) {
				sw.WriteLineAsync(ex.ToString());
			}
		}

		/// <summary>用於處理 mThrQueMonitor 之內部錯誤</summary>
		/// <param name="ex">例外狀況</param>
		/// <param name="log">原本欲附加的訊息</param>
		private void SpecialLog(Exception ex, LogMessage log) {
			string path = GetFilePath("LibCrash.log");
			using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8)) {
				sw.WriteLine($"----- Occurred At {DateTime.Now.ToString("HH:mm:ss.fff")} -----");
				sw.WriteLine($"Log Message : {log.ToString()}");
				sw.WriteLine(ex.ToString());
				sw.WriteLine(string.Empty);
			}
		}

		/// <summary>用於處理 mThrQueMonitor 之內部錯誤</summary>
		/// <param name="ex">例外狀況</param>
		/// <param name="log">原本欲附加的訊息</param>
		private static void StaticSpecialLog(Exception ex, LogMessage log) {
			string path = GetFilePath("LibCrash.log");
			using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8)) {
				sw.WriteLine($"----- Occurred At {DateTime.Now.ToString("HH:mm:ss.fff")} -----");
				sw.WriteLine($"Log Message : {log.ToString()}");
				sw.WriteLine(ex.ToString());
				sw.WriteLine(string.Empty);
			}
		}
		#endregion

		#region Function - Threads & Queue

		/// <summary>檢查並儲存記錄檔之執行緒，直接寫入</summary>
		private void tsk_LogQue_Direct() {
			int exitCount = 0;
			LogMessage logMsg = null;
			List<string> checkedPath = new List<string>();

			/* 卡住迴圈，直至 Dispose 被觸發關閉 */
			do {
				/*-- 檢查應用程式是否關閉 --*/
				if (mFlag_Dispose || Environment.HasShutdownStarted) {
					exitCount++;
					if (exitCount > EXIT_DELAY_COUNT) break;   //等待 0.5 秒，避免有些 Log 還來不及寫完
				}

				/*-- 檢查是否有記錄需要寫入 --*/
				if (!mLogQue.IsEmpty) {

					/*-- 如果佇列裡有東西則取出最前面的一個 --*/
					if (!mLogQue.TryDequeue(out logMsg)) continue;

					/*-- 如果有東西，進行儲存 --*/
					if (logMsg != null) {
						/* 如果還沒有檢查過路徑，檢查之並存起來 */
						if (!checkedPath.Contains(logMsg.FilePath)) {
							CtFile.CreateDirectory(logMsg.FilePath);
							checkedPath.Add(logMsg.FilePath);
						}
						/* 寫入檔案 */
						using (StreamWriter sw = new StreamWriter(logMsg.FilePath, true, Encoding.UTF8)) {
							sw.WriteLine(logMsg.ToString());
						}
					}
				}

				CtTimer.Delay(THREAD_DELAY_TIME);  //延遲0.1秒

			} while (true);
		}

		/// <summary>檢查並儲存記錄檔之執行緒，等待筆數後才寫入</summary>
		private void tsk_LogQue_WaitCount() {
			int exitCount = 0;
			LogMessage logMsg = null;
			Dictionary<string, StreamWriter> fileStrem = new Dictionary<string, StreamWriter>();
			Dictionary<string, List<LogMessage>> logColl = new Dictionary<string, List<LogMessage>>();

			/* 卡住迴圈，直至 Dispose 被觸發關閉 */
			do {
				/*-- 檢查應用程式是否關閉 --*/
				if (mFlag_Dispose || Environment.HasShutdownStarted) {
					exitCount++;
					if (exitCount > EXIT_DELAY_COUNT) break;   //等待 0.5 秒，避免有些 Log 還來不及寫完
				}

				/*-- 檢查是否有記錄需要寫入 --*/
				if (!mLogQue.IsEmpty) {

					/*-- 如果佇列裡有東西則取出最前面的一個並儲存之，直至取出所有的物件 --*/
					while (mLogQue.TryDequeue(out logMsg)) {
						if (logColl.ContainsKey(logMsg.FilePath)) {
							logColl[logMsg.FilePath].Add(logMsg);
						} else {
							logColl.Add(logMsg.FilePath, new List<LogMessage> { logMsg });
						}
					}

					/*-- 如果有東西，進行儲存 --*/
					logColl.ForEach(
						kvp => {
							if (kvp.Value.Count >= DELAY_WRITE_COUNT) {
								Console.WriteLine($"寫 {kvp.Key}");
								try {
									StreamWriter sw = null;
									if (fileStrem.ContainsKey(kvp.Key)) {
										sw = fileStrem[kvp.Key];
									} else {
										CtFile.CreateDirectory(kvp.Key);   //檢查該資料夾是否存在，如不存在則建立之
										sw = new StreamWriter(kvp.Key, true, Encoding.UTF8);
										fileStrem.Add(kvp.Key, sw);
									}
									kvp.Value.ForEach(log => sw.WriteLine(log.ToString()));
									kvp.Value.Clear();
								} catch (Exception ex) {
									SpecialLog(ex);
								}
							}
						}
					);
				}

				CtTimer.Delay(THREAD_DELAY_TIME);  //延遲0.1秒

			} while (true);

			/*-- 將所有未寫入的寫進去紀錄檔 --*/
			logColl.ForEach(
				kvp => {
					Console.WriteLine($"寫 {kvp.Key}");
					try {
						StreamWriter sw = null;
						if (fileStrem.ContainsKey(kvp.Key)) {
							sw = fileStrem[kvp.Key];
						} else {
							CtFile.CreateDirectory(kvp.Key);   //檢查該資料夾是否存在，如不存在則建立之
							sw = new StreamWriter(kvp.Key, true, Encoding.UTF8);
							fileStrem.Add(kvp.Key, sw);
						}
						kvp.Value.ForEach(log => sw.WriteLine(log.ToString()));
						kvp.Value.Clear();
					} catch (Exception ex) {
						SpecialLog(ex);
					}
				}
			);

			/*-- 應用程式進入關閉階段，釋放各個檔案的控制權 --*/
			if (fileStrem.Count > 0) {
				fileStrem.ForEach(
					kvp => {
						try {
							kvp.Value.Close();
						} catch (Exception ex) {
							SpecialLog(ex);
						}
					}
				);
			}
		}
		#endregion

		#region Function - Instance Operations
		/// <summary>添加記錄檔訊息至佇列，於下次佇列處理時進行寫入動作</summary>
		/// <param name="path">檔案路徑</param>
		/// <param name="msg">訊息內容</param>
		/// <param name="time">訊息時間</param>
		public void Enqueue(string path, string msg, DateTime time) {
			if (mLogQue != null)
				mLogQue.Enqueue(new LogMessage(time, path, msg));
		}
		#endregion

		#region Function - Log Operations

		/// <summary>添加一般訊息、操作步驟、通知等紀錄檔</summary>
		/// <param name="title">訊息標題</param>
		/// <param name="msg">訊息內容</param>
		/// <param name="filename">檔案名稱，不含路徑。如 "Trace.log"</param>
		public static void TraceLog(string title, string msg, string filename = CtConst.FILE_TRACELOG) {
			DateTime time = DateTime.Now;
			string strPath = GetFilePath(filename);
			string strMsg = string.Format("[{0}] - {1}", title.Trim(), msg);

			SaveLog(strPath, strMsg, time);
		}

		/// <summary>添加一般訊息、操作步驟、通知等紀錄檔</summary>
		/// <param name="time">此訊息所發生的時間點</param>
		/// <param name="title">訊息標題</param>
		/// <param name="msg">訊息內容</param>
		/// <param name="filename">檔案名稱，不含路徑。如 "Trace.log"</param>
		public static void TraceLog(DateTime time, string title, string msg, string filename = CtConst.FILE_TRACELOG) {
			string strPath = GetFilePath(filename);
			string strMsg = string.Format("[{0}] - {1}", title.Trim(), msg);

			SaveLog(strPath, strMsg, time);
		}

		/// <summary>添加警報、錯誤等紀錄檔</summary>
		/// <param name="errDevice">發生錯誤之裝置</param>
		/// <param name="title">訊息標題</param>
		/// <param name="errCode">錯誤代碼</param>
		/// <param name="msg">訊息內容</param>
		/// <param name="filename">檔案名稱，不含路徑。如 "Alarm.log"</param>
		public static void ErrorLog(string errDevice, int errCode, string title, string msg, string filename = CtConst.FILE_ALARMLOG) {
			DateTime time = DateTime.Now;
			string strPath = GetFilePath(filename);
			string strMsg = string.Format("{{{0}}} ({1}) : [{2}] - {3}", errDevice, errCode.ToString(), title, msg);

			SaveLog(strPath, strMsg, time);
		}

		/// <summary>添加警報、錯誤等紀錄檔</summary>
		/// <param name="time">錯誤發生時間</param>
		/// <param name="errDevice">發生錯誤之裝置</param>
		/// <param name="title">訊息標題</param>
		/// <param name="errCode">錯誤代碼</param>
		/// <param name="msg">訊息內容</param>
		/// <param name="filename">檔案名稱，不含路徑。如 "Alarm.log"</param>
		public static void ErrorLog(DateTime time, string errDevice, int errCode, string title, string msg, string filename = CtConst.FILE_ALARMLOG) {
			string strPath = GetFilePath(filename);
			string strMsg = string.Format("{{{0}}} ({1}) : [{2}] - {3}", errDevice, errCode.ToString(), title, msg);

			SaveLog(strPath, strMsg, time);
		}

		/// <summary>添加自訂義記錄檔</summary>
		/// <param name="msg">訊息內容</param>
		/// <param name="filename">檔案名稱，不含路徑。如 "Report.log"</param>
		public static void ReportLog(string msg, string filename = CtConst.FILE_REPORTLOG) {
			DateTime time = DateTime.Now;
			string strPath = GetFilePath(filename);
			SaveLog(strPath, msg, time);
		}

		/// <summary>添加自訂義記錄檔</summary>
		/// <param name="time">訊息發生時間</param>
		/// <param name="msg">訊息內容</param>
		/// <param name="filename">檔案名稱，不含路徑。如 "Report.log"</param>
		public static void ReportLog(DateTime time, string msg, string filename = CtConst.FILE_REPORTLOG) {
			string strPath = GetFilePath(filename);
			SaveLog(strPath, msg, time);
		}

		/// <summary>
		/// 儲存相關訊息至 Windows 事件檢視器裡
		/// <para>我方類別: 事件檢視器\應用程式及服務紀錄檔\CASTEC</para>
		/// </summary>
		/// <param name="source">來源名稱，即程式名稱</param>
		/// <param name="message">欲儲存的訊息</param>
		/// <param name="logType">儲存的紀錄檔類型。請參考 <see cref="EventLogType"/></param>
		/// <param name="eventID">[選項] 此事件 ID 代號</param>
		public static void SystemEventLog(string source, string message, EventLogType logType, ushort eventID = 0) {
			if (!EventLog.SourceExists(source)) EventLog.CreateEventSource(source, "CASTEC");
			EventLog log = new EventLog("CASTEC");
			log.Source = source;
			log.WriteEntry(message, (EventLogEntryType)logType, eventID);
		}
		#endregion

		#region Function - Application Operations
		/// <summary><see cref="Application.ThreadExit"/> 訂閱處理，更改旗標，執行緒完成現有紀錄檔後自動關閉</summary>
		/// <param name="sender"><see cref="Application"/></param>
		/// <param name="e">事件參數</param>
		private void OnThreadExit(object sender, EventArgs e) {
			mFlag_Dispose = true;
		}
		#endregion

		#region Function - IDisposable
		/// <summary>釋放 CtLog 相關資源，執行緒會於寫入所有紀錄後自動關閉</summary>
		public void Dispose() {
			mFlag_Dispose = true;
		}
		#endregion
	}
}
