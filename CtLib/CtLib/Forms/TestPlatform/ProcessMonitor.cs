using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Utility;

namespace CtLib.Forms.TestPlatform {
	/// <summary>一個簡單的視窗供監看應用程式之輸出資料</summary>
	public partial class ProcessMonitor : Form {

		#region Version

		/// <summary>ProcessMonitor 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 
		/// 1.0.0  Ahern [2016/03/18]
		///     + 建構基本 UI 與 Function
		///     
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(1, 0, 0, "2016/03/18", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Fields
		private Process mProc;
		private Thread mScan;
		#endregion

		#region Declaration - Fields
		private string mAppPath = string.Empty;
		private string mAppArg = string.Empty;
		#endregion

		#region Function - Constructors
		/// <summary>建構監控視窗，於啟動後由使用者選擇應用程式</summary>
		public ProcessMonitor() {
			InitializeComponent();
		}

		/// <summary>建構監控視窗，並指定特定應用程式路徑</summary>
		/// <param name="appPath">應用程式路徑，如 @"D:\CASTEC\Project\CAMPro\CAMPro.exe"</param>
		public ProcessMonitor(string appPath) {
			InitializeComponent();

			mAppPath = appPath;
		}

		/// <summary>建構監控視窗，並指定特定應用程式路徑與啟動參數</summary>
		/// <param name="appPath">應用程式路徑，如 @"D:\CASTEC\Project\CAMPro\CAMPro.exe"</param>
		/// <param name="args">啟動參數，如 "culture=en"</param>
		public ProcessMonitor(string appPath, string args) {
			InitializeComponent();

			mAppPath = appPath;
			mAppArg = args;
		}
		#endregion

		#region Function - Private Methods
		private void StartMonitor() {
			if (string.IsNullOrEmpty(mAppPath)) {
				OpenFileDialog dialog = new OpenFileDialog();
				dialog.Filter = "應用程式 | *.exe";
				dialog.InitialDirectory = Application.StartupPath;
				if (dialog.ShowDialog() == DialogResult.OK) {
					mAppPath = dialog.FileName;
					CtInput.Text(out mAppArg, "參數設定", "請輸入啟動附加參數，若無附加參數可保持空白");
				}
			}

			if (!string.IsNullOrEmpty(mAppPath)) {
				if (Process.GetProcesses().Any(proc => proc.StartInfo.FileName == mAppPath)) {
					IEnumerable<Process> existProc = Process.GetProcesses().Where(proc => proc.StartInfo.FileName == mAppPath);
					foreach (Process proc in existProc) {
						proc.Close();
						proc.WaitForExit(3000);
					}
				}

				ProcessStartInfo startInfo = new ProcessStartInfo(mAppPath, mAppArg) {
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				};

				mProc = new Process() {
					StartInfo = startInfo,
					EnableRaisingEvents = true
				};

				mProc.ErrorDataReceived += mProc_ErrorDataReceived;
				mProc.OutputDataReceived += mProc_OutputDataReceived;

				mProc.Start();

				mProc.BeginErrorReadLine();
				mProc.BeginOutputReadLine();

				CtThread.CreateThread(ref mScan, "ProcessMonitor", tsk_WaitProcess);
			}
		}

		private void AddItem(string msg) {
			listBox1.InvokeIfNecessary(
				() => {
					int itemCount = listBox1.Items.Count;
					if (itemCount > 200) listBox1.Items.RemoveAt(itemCount - 1);
					listBox1.Items.Insert(0, msg);
				}
			);
		}
		#endregion

		#region Function - Threads
		private void tsk_WaitProcess() {
			mProc.WaitForExit();
			string msg = string.Format("[{0}] Target had shutdown, exit code is {1}", DateTime.Now.ToString("HH:mm:ss.fff"), mProc.ExitCode);
			mProc.ErrorDataReceived -= mProc_ErrorDataReceived;
			mProc.OutputDataReceived -= mProc_OutputDataReceived;
			AddItem(msg);
		}
		#endregion

		#region Function - Interface Events
		private void mProc_ErrorDataReceived(object sender, DataReceivedEventArgs e) {
			if (!string.IsNullOrEmpty(e.Data)) {
				string msg = string.Format("[{0}][Error] - {1}", DateTime.Now.ToString("HH:mm:ss.fff"), e.Data);
				AddItem(msg);
			}
		}

		private void mProc_OutputDataReceived(object sender, DataReceivedEventArgs e) {
			if (!string.IsNullOrEmpty(e.Data)) {
				string msg = string.Format("[{0}][Data] - {1}", DateTime.Now.ToString("HH:mm:ss.fff"), e.Data);
				AddItem(msg);
			}
		}

		private void ProcessMonitor_Shown(object sender, EventArgs e) {
			Application.DoEvents();
			StartMonitor();
		}

		private void clearAllMessageToolStripMenuItem_Click(object sender, EventArgs e) {
			CtInvoke.ListBoxClear(listBox1);
		}

		#endregion
	}
}
