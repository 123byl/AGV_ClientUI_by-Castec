using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

using CtLib.Forms;
using CtLib.Forms.TestPlatform;
using CtLib.Library;

namespace CtLib {
	static class Program {
		#region Entries
		/// <summary>
		/// 應用程式的主要進入點。
		/// </summary>
		[STAThread]
		static void Main() {
			/*-- 抓取未用 Try-Catch-Finally 包住的 Exception --*/
			#region Exception Handler
			/* 設定如果遇到未包住的 Exception 則處理 Exception */
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			/* UI 執行緒類的錯誤 */
			Application.ThreadException += Application_ThreadException;
			/* 並非處理 UI 的錯誤 */
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			#endregion

			/*-- Windows Form 樣式設定 --*/
			#region Styles
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			#endregion

			/*-- 檢查是否帶有啟動參數 --*/
			#region Argument Examination
			string[] args = Environment.GetCommandLineArgs();
			if (1.Equals(args.Length)) {
				#region Normal StartUp
				/* 檢查是否是用 Visual Studio 開啟，如不是則請使用者輸入驗證碼 */
				Stat verified = ProgramVerification();

				/* 視窗啟動器 */
				if (verified == Stat.SUCCESS) {
					Form form;
					Launcher launcher = new Launcher();
					if (launcher.Start(out form) == DialogResult.OK)
						Application.Run(form);
				} else if (verified == Stat.ER_SYS_INVSET) {
					MessageBox.Show(
						"驗證錯誤\r\nInvalid verification code",
						"Verification Failed",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
				}
				#endregion
			} else {
				#region StartUp With Arguments
				switch (args[1].ToLower()) {
					/* Console Monitor */
					case "/consolemonitor":
						string[] splited = args[2].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

						ProcessMonitor pm;
						if (1.Equals(splited.Length)) pm = new ProcessMonitor(splited[0]);
						else pm = new ProcessMonitor(splited[0], splited[1]);

						Application.Run(pm);
						break;

					/* Socket */
					case "socket":
						Application.Run(new Test_Socket());
						break;

					/* Serial */
					case "serial":
						Application.Run(new Test_Serial());
						break;

					/* 詢問有哪些參數 */
					case "/?":
						string describe = ArgumentsDescription();
						MessageBox.Show(
							describe,
							"CtLib Information",
							MessageBoxButtons.OK,
							MessageBoxIcon.Information
						);
						break;

					/* 未解析的參數 */
					default:
						MessageBox.Show(
							"啟動參數錯誤\r\nInvalid arguments",
							"Start Failed",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error
						);
						break;
				}
				#endregion
			}
			#endregion
		} 
		#endregion

		#region Exception Process
		/// <summary>非 UI 執行緒錯誤的處理方法</summary>
		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			Exception ex = e.ExceptionObject as Exception;
			if (ex != null) CtStatus.Report(Stat.ER_SYSTEM, ex);
			else CtStatus.Report(Stat.ER_SYSTEM, "UnhandledNonThreadEx", e.ToString());
		}

		/// <summary>UI 錯誤的處理方法</summary>
		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
			if (e.Exception != null) CtStatus.Report(Stat.ER_SYSTEM, e.Exception);
			else CtStatus.Report(Stat.ER_SYSTEM, "UnhandledThreadEx", e.ToString());
		}
		#endregion

		#region StartUp Process
		/// <summary>CtLib 啟動驗證，若非由 VS 啟動則須輸入正確密碼</summary>
		/// <returns>(<see langword="true"/>)驗證成功  (<see langword="false"/>)密碼輸入錯誤</returns>
		static Stat ProgramVerification() {
			if (!Debugger.IsAttached) {
				string password;
				Stat stt = CtInput.Password(out password, "Verification", "請輸入驗證碼\r\nPlease enter verification code", true);
				if (stt == Stat.SUCCESS) {
					CtCrypto.Encrypt(CryptoMode.AES256, ref password);
					if (!"td1+CY1spGpYHBsye0VoEty8UA0rw7g9bQERA7XaKLM=".Equals(password)) {
						stt = Stat.ER_SYS_INVSET;
					}
				}
				return stt;
			} else return Stat.SUCCESS;
		}

		/// <summary>取得 /? 時所顯示的參數介紹</summary>
		/// <returns>參數介紹</returns>
		static string ArgumentsDescription() {
			StringBuilder strBuld = new StringBuilder();

			/*-- Console Monitor --*/
			strBuld.AppendLine("[ConsoleMonitor]");
			strBuld.AppendLine("    說明");
			strBuld.AppendLine("         .Net 程式監控，獲取其相關串流資訊。例如：Console.WriteLine、Exception");
			strBuld.AppendLine("    使用方法");
			strBuld.AppendLine("         CtLib.exe /ConsoleMonitor File,Args");
			strBuld.AppendLine("    參數說明");
			strBuld.AppendLine("         File = 欲監控的程式檔案");
			strBuld.AppendLine("         Args = 啟動該程式的參數");
			strBuld.AppendLine("    範例");
			strBuld.AppendLine("         CtLib.exe /ConsoleMonitor Ace.exe,-culture=en");
			strBuld.AppendLine();
			strBuld.AppendLine("[Socket]");
			strBuld.AppendLine("    說明");
			strBuld.AppendLine("         開啟 Socket 測試介面，提供 TCP 與 UDP 通訊應用");
			strBuld.AppendLine("    使用方法");
			strBuld.AppendLine("         CtLib.exe /Socket");
			strBuld.AppendLine();
			strBuld.AppendLine("[Serial]");
			strBuld.AppendLine("    說明");
			strBuld.AppendLine("         開啟串列埠測試介面");
			strBuld.AppendLine("    使用方法");
			strBuld.AppendLine("         CtLib.exe /Socket");

			return strBuld.ToString();
		}
		#endregion
	}
}
