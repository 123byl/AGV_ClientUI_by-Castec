using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using CtLib.Module.Ultity;

namespace CtLib.Library {
    /// <summary>
    /// 記錄檔相關
    /// <para>存檔路徑部分，將在每次進入副程式時讀取 Resource 之路徑 (Settings.settings)</para>
    /// <para>方法採用 Queue，由一個 執行緒(<see cref="Thread"/>) 來持續檢查 Queue，有需要寫入時將進行寫入動作</para>
    /// </summary>
    /// <remarks>
    /// 目前 Log 分類
    /// <para>1. TraceLog - 用於操作紀錄，如 "設備開始"、"設備停止" 等相關訊息</para>
    /// <para>2. ErrorLog - 用於設備錯誤，如 "Sensor無法感應到"、"馬達 Servo 失敗" 等訊息</para>
    /// <para>3. ReportLog - 自訂義的紀錄，如 "產量紀錄"。 為承襲傳統，CtStatus.Report 也使用此紀錄</para>
    /// <para>..........</para>
    /// <para>由於時下還有些許系統是跑 Windows XP 或 Adept ACE 3.3 版本，故無法使用 System.Threading.Tasks</para>
    /// <para>未來若已全部升級，可更換成 ThreadPool 方式提升效能，避免 Thread 持續空等</para>
    /// </remarks>
    public static class CtLog {

        #region Version

        /// <summary>CtLog 版本訊息</summary>
        /// <remarks><code>
        /// 1.0.0  Ahern [2014/07/15]
        ///     + 將讀寫Log之常用Function建立至此
        ///     
        /// 1.0.1  Ahern [2015/02/11]
        ///     \ Log 路徑改採 CtDefaultPath 以讀取 Settings.settings 之路徑
        ///     
        /// 1.1.0  Ahern [2015/04/23]
        ///     + SystemEventLog
        ///     
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 1, 0, "2015/04/23", "Ahern Kuo");

        #endregion

        #region Declaration - Members

        /// <summary>[Thread] 儲存記錄檔</summary>
        private static Thread tSaveLog;
        /// <summary>[Queue] 先進先出之記錄檔佇列</summary>
        private static Queue<LogMessage> qSaveLog;

        #endregion

        #region Declaration - Properties
        /// <summary>取得剩餘未記錄的訊息筆數</summary>
        public static int QueueCount { get { return qSaveLog.Count; } } 
        #endregion

        #region Declaration - Definitions

        /// <summary>記錄檔訊息，含路徑、訊息與時間點</summary>
        private class LogMessage {
            /// <summary>該訊息時間點</summary>
            public DateTime LogTime;
            /// <summary>檔案路徑</summary>
            public string FilePath = "";
            /// <summary>紀錄訊息</summary>
            public string InfoText = "";
            /// <summary>建構元，建立含路徑、時間與訊息之類別</summary>
            /// <param name="time">該訊息時間點</param>
            /// <param name="path">檔案路徑</param>
            /// <param name="text">紀錄文字</param>
            public LogMessage(DateTime time, string path, string text) {
                LogTime = time;
                FilePath = path;
                InfoText = text;
            }
        }

        #endregion

        #region Functions - Methods
        /// <summary>寫入單行至特定檔案</summary>
        /// <param name="path">檔案路徑</param>
        /// <param name="info">紀錄文字訊息</param>
        /// <param name="newline">是否在結尾添加換行符號</param>
        private static void WriteLine(string path, string info, bool newline = true) {
            /*-- 確認檔案是否已存在，如不存在則建立之 --*/
            CtFile.CreateFile(path);

            /*-- 檢查當前訊息是否以換行符號結尾，如沒有且newline為True則在結尾加入換行 --*/
            string strInfo = info;
            if (newline && !strInfo.EndsWith(Environment.NewLine))
                strInfo += Environment.NewLine; //

            /*-- 將訊息寫入檔案 --*/
            File.AppendAllText(path, strInfo, Encoding.UTF8);
        }

        /// <summary>儲存記錄檔</summary>
        /// <param name="msg">欲儲存之訊息類別</param>
        private static void SaveLog(LogMessage msg) {
            string strInfo = "";
            /*-- 組合時間點與訊息 --*/
            strInfo = "[" + msg.LogTime.ToString("HH:mm:ss.fff") + "] " + msg.InfoText.Replace("\r\n", " ");

            /*-- 寫入檔案 --*/
            WriteLine(msg.FilePath, strInfo);
        }

        /// <summary>建立記錄檔類別，並加入佇列</summary>
        /// <param name="path">檔案路徑</param>
        /// <param name="msg">訊息內容</param>
        /// <remarks>時間點將在此加入，但路徑與訊息則須在上一層設定好。或許這邊可以再討論看看是否合宜</remarks>
        private static void SaveLog(string path, string msg) {
            /*-- 如果Queue尚未建立，建立之 --*/
            if (qSaveLog == null) qSaveLog = new Queue<LogMessage>();

            /*-- 如果Scan Thread尚未建立，建立之 --*/
            if (tSaveLog == null || !tSaveLog.IsAlive) {
                CtThread.CreateThread(ref tSaveLog, "CtLib_SaveLog", tsk_LogQueue);
            }

            /*-- 加入佇列 --*/
            lock (qSaveLog) {
                qSaveLog.Enqueue(new LogMessage(DateTime.Now, path, msg));
            }
        }

        /// <summary>建立記錄檔類別，並加入佇列</summary>
        /// <param name="path">檔案路徑</param>
        /// <param name="msg">訊息內容</param>
        /// <param name="time">訊息時間</param>
        private static void SaveLog(string path, string msg, DateTime time) {
            /*-- 如果Queue尚未建立，建立之 --*/
            if (qSaveLog == null) qSaveLog = new Queue<LogMessage>();

            /*-- 如果Scan Thread尚未建立，建立之 --*/
            if (tSaveLog == null || !tSaveLog.IsAlive) {
                CtThread.CreateThread(ref tSaveLog, "CtLib_SaveLog", tsk_LogQueue);
            }

            /*-- 加入佇列 --*/
            lock (qSaveLog) {
                qSaveLog.Enqueue(new LogMessage(time, path, msg));
            }
        }

        #endregion

        #region Functions - Threads & Queue

        /// <summary>檢查並儲存記錄檔之執行緒</summary>
        private static void tsk_LogQueue() {
            /* 卡住迴圈，直至Thread關閉 */
            do {
                try {
                    /*-- 如果佇列裡有東西則取出最前面的一個並儲存之 --*/
                    if (qSaveLog.Count > 0)
                        SaveLog(qSaveLog.Dequeue());
                } catch (Exception) {
                } finally {
                    Thread.Sleep(100);  //延遲0.1秒
                }
            } while (tSaveLog.IsAlive);
        }

        #endregion

        #region Functions - Core

        /// <summary>添加一般訊息、操作步驟、通知等紀錄檔</summary>
        /// <param name="title">訊息標題</param>
        /// <param name="msg">訊息內容</param>
        /// <param name="filename">檔案名稱，不含路徑。如 "Trace.log"</param>
        public static void TraceLog(string title, string msg, string filename = CtConst.FILE_TRACELOG) {
            string strPath = CtDefaultPath.GetPath(SystemPath.LOG);
            string strMsg = "";

            strPath += CtFile.BackSlash(DateTime.Now.ToString("yyyyMMdd")) + filename;
            strMsg = "[" + title.Trim() + "] - " + msg;
            SaveLog(strPath, strMsg);
        }

        /// <summary>添加警報、錯誤等紀錄檔</summary>
        /// <param name="errDevice">發生錯誤之裝置</param>
        /// <param name="title">訊息標題</param>
        /// <param name="errCode">錯誤代碼</param>
        /// <param name="msg">訊息內容</param>
        /// <param name="filename">檔案名稱，不含路徑。如 "Alarm.log"</param>
        public static void ErrorLog(string errDevice, int errCode, string title, string msg, string filename = CtConst.FILE_ALARMLOG) {
            string strPath = CtDefaultPath.GetPath(SystemPath.LOG);
            string strMsg = "";

            strPath += CtFile.BackSlash(DateTime.Now.ToString("yyyyMMdd")) + filename;
            strMsg = "{" + errDevice.Trim() + "} (" + CtConvert.CStr(errCode) + ") : [" + title + "] - " + msg;
            SaveLog(strPath, strMsg);
        }

        /// <summary>添加警報、錯誤等紀錄檔</summary>
        /// <param name="time">錯誤發生時間</param>
        /// <param name="errDevice">發生錯誤之裝置</param>
        /// <param name="title">訊息標題</param>
        /// <param name="errCode">錯誤代碼</param>
        /// <param name="msg">訊息內容</param>
        /// <param name="filename">檔案名稱，不含路徑。如 "Alarm.log"</param>
        public static void ErrorLog(DateTime time, string errDevice, int errCode, string title, string msg, string filename = CtConst.FILE_ALARMLOG) {
            string strPath = CtDefaultPath.GetPath(SystemPath.LOG);
            string strMsg = "";

            strPath += CtFile.BackSlash(DateTime.Now.ToString("yyyyMMdd")) + filename;
            strMsg = "{" + errDevice.Trim() + "} (" + CtConvert.CStr(errCode) + ") : [" + title + "] - " + msg;
            SaveLog(strPath, strMsg, time);
        }

        /// <summary>添加自訂義記錄檔</summary>
        /// <param name="msg">訊息內容</param>
        /// <param name="filename">檔案名稱，不含路徑。如 "Report.log"</param>
        public static void ReportLog(string msg, string filename = CtConst.FILE_REPORTLOG) {
            string strPath = CtDefaultPath.GetPath(SystemPath.LOG);

            strPath += DateTime.Now.ToString("yyyyMMdd") + "\\" + filename;
            SaveLog(strPath, msg);
        }

        /// <summary>
        /// 儲存相關訊息至 Windows 事件檢視器裡
        /// <para>我方類別: 事件檢視器\應用程式及服務紀錄檔\CASTEC</para>
        /// </summary>
        /// <param name="source">來源名稱，即程式名稱</param>
        /// <param name="message">欲儲存的訊息</param>
        /// <param name="logType">儲存的紀錄檔類型。請參考 <see cref="EventLogType"/></param>
        /// <param name="eventID">[選項] 此事件 ID 代號</param>
        public static void SystemEventLog(string source, string message, EventLogType logType, int eventID = 0) {
            if (!EventLog.SourceExists(source)) EventLog.CreateEventSource(source, "CASTEC");
            EventLog log = new EventLog("CASTEC");
            log.Source = source;
            log.WriteEntry(message, (EventLogEntryType)logType, eventID);
        }
        #endregion
    }
}
