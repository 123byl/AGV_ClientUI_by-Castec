using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using CtLib.Library;
using CtLib.Module.Utility;

using Ace.Adept.Server.Desktop.Connection;

namespace CtLib.Module.Adept {

	#region Declaration - Support Class
	/// <summary>Task相關資訊</summary>
	[Serializable]
	public class TaskInfo {
		/// <summary>V+程式名稱</summary>
		public string ProgName { get; set; }
		/// <summary>Task Control 之 Task 編號</summary>
		public int TaskNum { get; set; }
		/// <summary>該Task狀態</summary>
		public TaskState TaskStt { get; set; }
		/// <summary>新建一個Task資訊</summary>
		public TaskInfo() {
			ProgName = "";
			TaskNum = -1;
			TaskStt = TaskState.Invalid;
		}
		/// <summary>新建一個Task資訊</summary>
		/// <param name="name">V+程式名稱</param>
		/// <param name="num">Task Control 之 Task 編號</param>
		/// <param name="stt">Task狀態</param>
		public TaskInfo(string name, int num, TaskState stt) {
			ProgName = name;
			TaskNum = num;
			TaskStt = stt;
		}
	}
	#endregion

	/// <summary>
	/// Adept ACE Task 相關控制
	/// <para>包含執行(Execute)、停止(Abort)、下一步(Proceed)、清除(Kill)等，另有 Task 監控與事件之發佈</para>
	/// <para>此部分為 Task Control 之 Task，與 Process Manager 、 Windows Task 不同！</para>
	/// </summary>
	[Serializable]
	public sealed class CtAceTask {

		#region Declaration - Fields
		/// <summary>最大的 Task 號碼。如 Task 0 ~ Task 25 (共 26 個)，則此值為 25</summary>
		private int mMaxTaskNum = -1;
		/// <summary>V+ 相關物件連結，用於控制 Task</summary>
		private VpObjects mVpObj;
		/// <summary>紀錄有多少Task需要去監控</summary>
		private List<TaskInfo> mMonitorTask = new List<TaskInfo>();
		/// <summary>[Thread] 監控Task變化之執行緒</summary>
		[NonSerialized]
		private Thread mMonitorThread;
		#endregion

		#region Declaration - Events
		/// <summary>發生Task通知事件</summary>
		public event EventHandler<TaskEventArgs> OnTaskChanged;

		/// <summary>Task事件參數</summary>
		public class TaskEventArgs : EventArgs {
			/// <summary>於Task Control裡的Task編號</summary>
			public int TaskNumber { get; set; }
			/// <summary>Main V+ Program 名稱</summary>
			public string ProgramName { get; set; }
			/// <summary>目前狀態</summary>
			public TaskState TaskStatus { get; set; }
			/// <summary>建立一個空的Task事件參數</summary>
			public TaskEventArgs() {
				TaskNumber = -1;
				ProgramName = "N/A";
				TaskStatus = TaskState.Invalid;
			}
			/// <summary>建立一個由Task資訊所組成的Task事件參數</summary>
			/// <param name="tskInfo">Task相關資訊物件</param>
			public TaskEventArgs(TaskInfo tskInfo) {
				TaskNumber = tskInfo.TaskNum;
				ProgramName = tskInfo.ProgName;
				TaskStatus = tskInfo.TaskStt;
			}
			/// <summary>建立一個由相關資訊所組成的Task事件參數</summary>
			/// <param name="tskNum">於Task Control裡的Task編號</param>
			/// <param name="tskName">Main V+ Program 名稱</param>
			/// <param name="tskStt">目前狀態</param>
			public TaskEventArgs(int tskNum, string tskName, TaskState tskStt) {
				TaskNumber = tskNum;
				ProgramName = tskName;
				TaskStatus = tskStt;
			}
		}

		/// <summary>觸發Task狀態變更事件</summary>
		/// <param name="e">Task事件之參數</param>
		private void OnTaskEventOccur(TaskEventArgs e) {
			EventHandler<TaskEventArgs> handler = OnTaskChanged;
			if (handler != null)
				handler(this, e);
		}
		#endregion

		#region Function - Status
		/// <summary>檢查該 Task 是否存在於 Task Control 清單中 (不管現在狀態)</summary>
		/// <param name="tskName">欲查詢之 V+ 程式名稱</param>
		/// <returns>是否存在。(<see langword="true"/>)存在  (<see langword="false"/>)不存在於 Task Control 裡</returns>
		public bool IsTaskExist(string tskName) {
			return mVpObj?.Status().Any(vpStt => vpStt.MainProgram == tskName) ?? false;
		}

		/// <summary>檢查該 Task 是否存在 於Task Control 清單中 (不管現在狀態)</summary>
		/// <param name="tskNum">欲查詢之 Task 編號</param>
		/// <returns>是否存在。(<see langword="true"/>)存在  (<see langword="false"/>)不存在於Task Control裡</returns>
		public bool IsTaskExist(int tskNum) {
			return mVpObj?.Status().Any(vpStt => vpStt.Task == tskNum) ?? false;
		}

		/// <summary>檢查該Task是否存在於Task Status Control裡，如存在則回傳當前執行狀態</summary>
		/// <param name="tskName">欲查詢之V+程式名稱</param>
		/// <param name="isExecuting">目前是否執行中</param>
		/// <returns>是否存在。(<see langword="true"/>)存在，但無要求現在狀態!  (<see langword="false"/>)不存在於Task Control裡</returns>
		public bool IsTaskExist(string tskName, out bool isExecuting) {
			bool bolTemp = false;
			bool bolExec = false;

			VPStatus vpStt = mVpObj.Status().FirstOrDefault(val => val.MainProgram == tskName);
			if (vpStt != null) {
				bolTemp = true;
				bolExec = vpStt.Running;
			}

			isExecuting = bolExec;
			return bolTemp;
		}

		/// <summary>檢查該Task是否存在於Task Status Control裡，如存在則回傳當前執行狀態</summary>
		/// <param name="tskNum">欲查詢之Task編號</param>
		/// <param name="isExecuting">目前是否執行中</param>
		/// <returns>是否存在。(<see langword="true"/>)存在，但無要求現在狀態!  (<see langword="false"/>)不存在於Task Control裡</returns>
		public bool IsTaskExist(int tskNum, out bool isExecuting) {
			bool bolTemp = false;
			bool bolExec = false;

			VPStatus vpStt = mVpObj.Status().FirstOrDefault(val => val.Task == tskNum);
			if (vpStt != null) {
				bolTemp = true;
				bolExec = vpStt.Running;
			}

			isExecuting = bolExec;
			return bolTemp;
		}

		/// <summary>檢查該Task是否監控中</summary>
		/// <param name="tskName">欲查詢之V+程式名稱</param>
		/// <returns>是否存在。(<see langword="true"/>)正在監控  (<see langword="false"/>)尚未在監控名單內</returns>
		public bool IsMoniting(string tskName) {
			return mMonitorTask.Exists(tsk => tsk.ProgName == tskName);
		}

		/// <summary>檢查該Task是否監控中</summary>
		/// <param name="tskNum">欲查詢之Task編號</param>
		/// <returns>是否存在。(<see langword="true"/>)正在監控  (<see langword="false"/>)尚未在監控名單內</returns>
		public bool IsMoniting(int tskNum) {
			return mMonitorTask.Exists(tsk => tsk.TaskNum == tskNum);
		}

		/// <summary>檢查當前是否有任一個 Task 處於執行狀態</summary>
		/// <returns>(<see langword="true"/>)尚有 Task 執行中  (<see langword="false"/>)全部已停止或未啟用</returns>
		public bool IsAnyTaskExecuting() {
			return mVpObj.Status().Any(val => val.Task <= mMaxTaskNum && val.Running);
		}
		#endregion

		#region Function - Constructors
		/// <summary>建立一 Task 相關控制物件</summary>
		/// <param name="links">V+ 相關物件連結，用於控制 Task</param>
		/// <param name="maxTaskNum">最大的 Task 數量</param>
		internal CtAceTask(VpObjects links, int maxTaskNum) {
			mVpObj = links;
			mMaxTaskNum = maxTaskNum;
		}
		#endregion

		#region Function - Methods
		/// <summary>利用V+名稱尋找當前執行中的Task是否有相符項目，如有則回傳對應TaskInfo</summary>
		/// <param name="tskName">欲搜尋之程式名稱</param>
		/// <returns>如有相符Task則回傳對應TaskInfo，否則回傳全空的</returns>
		private TaskInfo FindTask(string tskName) {
			VPStatus vpTask = mVpObj.Status().FirstOrDefault(vpStt => vpStt.MainProgram == tskName);
			if (vpTask != null) {
				int tskStt = mVpObj.TaskStt(vpTask.Task);
				return new TaskInfo(vpTask.MainProgram, vpTask.Task, (TaskState)tskStt);
			} else return null;
		}

		/// <summary>利用Task編號尋找當前Task Control裡是否有相符項目，如有則回傳對應TaskInfo</summary>
		/// <param name="tskNum">欲搜尋之Task索引</param>
		/// <returns>如有相符Task則回傳對應TaskInfo，否則回傳全空的</returns>
		private TaskInfo FindTask(int tskNum) {
			VPStatus vpTask = mVpObj.Status().FirstOrDefault(vpStt => vpStt.Task == tskNum);
			if (vpTask != null) {
				int tskStt = mVpObj.TaskStt(tskNum);
				return new TaskInfo(vpTask.MainProgram, vpTask.Task, (TaskState)tskStt);
			} else return null;
		}

		/// <summary>等待Task不再是Executing</summary>
		/// <param name="tskNum">欲監控的Task編號</param>
		/// <param name="waitTime">逾時時間</param>
		private bool WaitTaskBreak(int tskNum, TimeSpan waitTime) {
			bool tmo = false;
			try {
				IVpLink vpLink = mVpObj.Link();
				int exec = (int)TaskState.Executing;
				int tskStt = vpLink.Task(tskNum, 1);

				tmo = CtTimer.WaitTimeout(
					waitTime,
					token => {
						do {
							tskStt = vpLink.Task(tskNum, 1);
							if (tskStt != exec) token.WorkDone();
							else CtTimer.Delay(10);
						} while (!token.IsDone);
					}
				);
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
			return !tmo;
		}

		/// <summary>等待Task轉為Executing</summary>
		/// <param name="tskNum">欲監控的Task編號</param>
		/// <param name="waitTime">逾時時間</param>
		private bool WaitTaskRunning(int tskNum, TimeSpan waitTime) {
			bool tmo = false;
			try {
				IVpLink vpLink = mVpObj.Link();
				int abort = (int)TaskState.Abort;
				int pause = (int)TaskState.Paused;
				int tskStt = vpLink.Task(tskNum, 1);

				tmo = CtTimer.WaitTimeout(
					waitTime,
					token => {
						do {
							tskStt = vpLink.Task(tskNum, 1);
							if (tskStt != abort && tskStt != pause) token.WorkDone();
							else CtTimer.Delay(10);
						} while (!token.IsDone);
					}
				);
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
			return !tmo;
		}

		/// <summary>監控一個Task，並在其發生變化時拋出事件通知狀態</summary>
		/// <param name="tskName">Task名稱</param>
		public void AddMonitorTask(string tskName) {
			TaskInfo tskInfo = FindTask(tskName);
			if ((tskInfo.TaskNum > 0) && (!mMonitorTask.Contains(tskInfo)))
				mMonitorTask.Add(tskInfo);

			if (mMonitorThread == null) {
				CtThread.CreateThread(ref mMonitorThread, "CtAce_MonitorTask", tsk_MonitorTask);
			} else if (!mMonitorThread.IsAlive) {
				CtThread.KillThread(ref mMonitorThread);
				CtThread.CreateThread(ref mMonitorThread, "CtAce_MonitorTask", tsk_MonitorTask);
			}
		}

		/// <summary>監控一個Task，並在其發生變化時拋出事件通知狀態</summary>
		/// <param name="tskNum">於 Task Control 裡的 Task Index</param>
		public void AddMonitorTask(int tskNum) {
			TaskInfo tskInfo = FindTask(tskNum);
			if ((tskInfo.TaskNum > 0) && (!mMonitorTask.Contains(tskInfo)))
				mMonitorTask.Add(tskInfo);

			if (mMonitorThread == null) {
				CtThread.CreateThread(ref mMonitorThread, "CtAce_MonitorTask", tsk_MonitorTask);
			} else if (!mMonitorThread.IsAlive) {
				CtThread.KillThread(ref mMonitorThread);
				CtThread.CreateThread(ref mMonitorThread, "CtAce_MonitorTask", tsk_MonitorTask);
			}
		}

		/// <summary>移除一個監控中的Task</summary>
		/// <param name="tskName">Task名稱</param>
		public void RemoveMonitorTask(string tskName) {
			for (int i = 0; i < mMonitorTask.Count; i++) {
				if (mMonitorTask[i].ProgName == tskName) {
					mMonitorTask.RemoveAt(i);
					break;
				}
			}
		}

		/// <summary>移除一個監控中的Task</summary>
		/// <param name="tskNum">於 Task Control 裡的 Task Index</param>
		public void RemoveMonitorTask(int tskNum) {
			for (int i = 0; i < mMonitorTask.Count; i++) {
				if (mMonitorTask[i].TaskNum == tskNum) {
					mMonitorTask.RemoveAt(i);
					break;
				}
			}

			/*-- 目前在Thread裡面直接判斷是否有 >0，如果沒有就直接關閉了 --*/
			//if (mMonitorTask.Count < 1) {
			//    CtThread.KillThread(ref mMonitorThread);
			//}
		}
		#endregion

		#region Function - Task
		/// <summary>取得特定編號之Task當前狀態</summary>
		/// <param name="tskNum">於Task Control裡的編號</param>
		/// <returns>當前Task狀態</returns>
		public TaskState GetTaskState(int tskNum) {
			int tskStt = mVpObj.TaskStt(tskNum);
			if (tskStt <= -1) throw (new Exception("取得 Task 狀態錯誤。代碼: " + tskStt.ToString()));
			return (TaskState)tskStt;
		}

		/// <summary>取得特定V+名稱之Task當前狀態</summary>
		/// <param name="tskName">欲取得狀態之V+ Main Program 名稱</param>
		/// <returns>當前Task狀態</returns>
		public TaskState GetTaskState(string tskName) {
			VPStatus vpStt = mVpObj.Status().FirstOrDefault(tsk => tsk.MainProgram == tskName);
			if (vpStt == null) throw (new Exception("無法取得的 Task 狀態。可能為違法的名稱"));
			return (TaskState)mVpObj.TaskStt(vpStt.Task);
		}

		/// <summary>查詢特定編號之 Task 是否存在，如存在則傳回當前狀態</summary>
		/// <param name="tskNum">於 Task Control 裡的編號</param>
		/// <param name="tskStt">取得的當前狀態</param>
		/// <returns>(<see langword="true"/>)成功取得Task  (<see langword="false"/>)無法取得Task</returns>
		public bool GetTaskState(int tskNum, out TaskState tskStt) {
			VPStatus task = mVpObj.Status().FirstOrDefault(vpStt => vpStt.Task == tskNum);
			if (task == null) {
				tskStt = TaskState.Invalid;
				return false;
			} else {
				tskStt = (TaskState)mVpObj.TaskStt(tskNum);
				return true;
			}
		}

		/// <summary>查詢特定 V+ 名稱之 Task 是否存在，如存在則傳回當前狀態</summary>
		/// <param name="tskName">欲取得狀態之 V+ Main Program 名稱</param>
		/// <param name="tskStt">取得的當前狀態</param>
		/// <returns>(<see langword="true"/>)成功取得Task  (<see langword="false"/>)無法取得Task</returns>
		public bool GetTaskState(string tskName, out TaskState tskStt) {
			VPStatus task = mVpObj.Status().FirstOrDefault(vpStt => vpStt.MainProgram == tskName);
			if (task == null) {
				tskStt = TaskState.Invalid;
				return false;
			} else {
				tskStt = (TaskState)mVpObj.TaskStt(task.Task);
				return true;
			}
		}

		/// <summary>於特定Task編號執行一個V+程式</summary>
		/// <param name="tskName">欲執行之V+程式名稱。如 "initial.r1"</param>
		/// <param name="tskNum">欲施放的Task編號</param>
		/// <param name="tskTmo">等待執行的逾時(Timeout)時間(毫秒，ms)。如果設定小於或等於0之數字，則表示忽略逾時選項</param>
		/// <param name="wait">是否等待該Task狀態變為 "Executing" 時才返回原程式</param>
		public void TaskExecute(string tskName, int tskNum, int tskTmo = 0, bool wait = false) {
			int intRet = mVpObj.Link(link => link.Execute(tskName, tskNum, tskTmo));
			if (intRet < 0) throw (new Exception("執行 Task 錯誤。代碼: " + intRet.ToString()));
			if (wait) WaitTaskRunning(tskNum, TimeSpan.FromMilliseconds(tskTmo));
		}

		/// <summary>將特定編號之Task繼續執行(下一步)</summary>
		/// <param name="tskNum">欲下一步的Task編號</param>
		/// <param name="wait">是否等待該Task狀態變為 "Executing" 時才返回原程式</param>
		public void TaskProceed(int tskNum, bool wait = false) {
			mVpObj.Link(link => link.Proceed(tskNum));
			if (wait) WaitTaskRunning(tskNum, TimeSpan.FromMinutes(1));
		}

		/// <summary>將特定V+程式名稱之Task繼續執行(下一步)</summary>
		/// <param name="tskName">欲下一步的V+程式名稱</param>
		/// <param name="wait">是否等待該Task狀態變為 "Executing" 時才返回原程式</param>
		public void TaskProceed(string tskName, bool wait = false) {
			VPStatus task = mVpObj.Status().FirstOrDefault(vpStt => vpStt.MainProgram == tskName);
			if (task != null) {
				mVpObj.Link(link => link.Proceed(task.Task));
				if (wait) WaitTaskRunning(task.Task, TimeSpan.FromMinutes(1));
			} else throw (new Exception("無法取得的 Task 狀態。可能為違法的名稱"));
		}

		/// <summary>將目前所有存在於Task Control之Task繼續動作(下一步)</summary>
		/// <param name="wait">是否等待每一個Task狀態變為 "Executing" 時才返回原程式。如開啟此功能，請小心Task狀態要能繼續，如是Halt/Error出來的可能導致卡死</param>
		/// <remarks>不確定如果繼續後馬上發Error或是停掉等可不可以正確離開WaitTaskRunning</remarks>
		public void TaskProceedAll(bool wait = false) {
			foreach (VPStatus vpStt in mVpObj.Status()) {
				if (vpStt.Task <= mMaxTaskNum && !vpStt.Running) {
					mVpObj.Link(link => link.Proceed(vpStt.Task));
					if (wait) WaitTaskRunning(vpStt.Task, TimeSpan.FromSeconds(3));
				}
			}

		}

		/// <summary>中斷特定編號之Task</summary>
		/// <param name="tskNum">欲中斷之Task編號</param>
		/// <param name="wait">是否等待該Task狀態不再是 "Executing" 時才返回原程式</param>
		public void TaskAbort(int tskNum, bool wait = false) {
			mVpObj.Link(link => link.Abort(tskNum));
			if (wait) WaitTaskBreak(tskNum, TimeSpan.FromMinutes(1));
		}

		/// <summary>中斷特定V+程式名稱之Task</summary>
		/// <param name="tskName">欲中斷之V+程式名稱</param>
		/// <param name="wait">是否等待該Task狀態不再是 "Executing" 時才返回原程式</param>
		public void TaskAbort(string tskName, bool wait = false) {
			VPStatus task = mVpObj.Status().FirstOrDefault(vpStt => vpStt.MainProgram == tskName);
			if (task != null) {
				mVpObj.Link(link => link.Abort(task.Task));
				if (wait) WaitTaskBreak(task.Task, TimeSpan.FromSeconds(10));
			} else throw (new Exception("無法取得的 Task 狀態。可能為違法的名稱"));
		}

		/// <summary>中斷所有活動中的Task</summary>
		/// <param name="wait">是否等待每一個Task狀態不再是 "Executing" 時才返回原程式</param>
		public void TaskAbortAll(bool wait = false) {
			foreach (VPStatus vpStt in mVpObj.Status()) {
				if ((vpStt.Task <= mMaxTaskNum) && (vpStt.Running)) {
					mVpObj.Link(link => link.Abort(vpStt.Task));
					if (wait) WaitTaskBreak(vpStt.Task, TimeSpan.FromSeconds(10));
				}
			}
		}

		/// <summary>清除特定編號之Task。如該Task仍在Executing狀態，則會等待中斷後才清除</summary>
		/// <param name="tskNum">欲清除之Task編號</param>
		public void TaskKill(int tskNum) {
			if (mVpObj.TaskStt(tskNum) == 1) {
				mVpObj.Link(link => link.Abort(tskNum));
				if (!WaitTaskBreak(tskNum, TimeSpan.FromSeconds(10)))
					throw new TimeoutException("等待 Task 停止過久。Task 編號 : " + tskNum);
			}

			mVpObj.Link(link => link.Kill(tskNum));
		}

		/// <summary>清除特定V+程式名稱之Task。如該Task仍在Executing狀態，則會等待中斷後才清除</summary>
		/// <param name="tskName">欲清除之V+程式名稱</param>
		public void TaskKill(string tskName) {
			VPStatus task = mVpObj.Status().FirstOrDefault(vpStt => vpStt.MainProgram == tskName);
			if (task == null) throw (new Exception("無法取得的 Task 狀態。可能為違法的名稱"));
			if (task.Running) {
				mVpObj.Link(link => link.Abort(task.Task));
				if (!WaitTaskBreak(task.Task, TimeSpan.FromSeconds(10)))
					throw new TimeoutException("等待 Task 停止過久。Task 編號 : " + task.Task);
			}
			mVpObj.Link(link => link.Kill(task.Task));
		}

		/// <summary>清除所有Task Control裡的Task。此將會等待各Task中斷後才清除</summary>
		public void TaskKillAll() {
			foreach (VPStatus vpStt in mVpObj.Status()) {
				if (vpStt.Task <= mMaxTaskNum) {
					if (vpStt.Running) {
						mVpObj.Link(link => link.Abort(vpStt.Task));
						if (!WaitTaskBreak(vpStt.Task, TimeSpan.FromSeconds(10)))
							throw new TimeoutException("等待 Task 停止過久。Task 編號 : " + vpStt.Task);
					}
					mVpObj.Link(link => link.Kill(vpStt.Task));
				}
			}
		}
		#endregion

		#region Function - Thread
		/// <summary>[Thread] 監控Task之執行緒，當有變化時會拋出Event</summary>
		private void tsk_MonitorTask() {
			int intIdx = 0;
			TaskState tskStt = TaskState.Invalid;
			try {
				do {
					tskStt = GetTaskState(mMonitorTask[intIdx].TaskNum);
					if (tskStt != mMonitorTask[intIdx].TaskStt) {
						mMonitorTask[intIdx].TaskStt = tskStt;
						//Raise an event
						OnTaskEventOccur(new TaskEventArgs(mMonitorTask[intIdx]));
					}
					intIdx = (intIdx < mMonitorTask.Count - 1) ? intIdx + 1 : 0;
					CtTimer.Delay(10);
				} while (mMonitorTask.Count > 0);
			} catch (ThreadAbortException) {
			} catch (ThreadInterruptedException) {
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
		}
		#endregion
	}
}
