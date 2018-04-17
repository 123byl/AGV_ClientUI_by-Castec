using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

using CtLib.Module.Utility;

namespace CtLib.Library {

	#region Function - Support Structure
	/// <summary>
	/// 系統效能資訊
	/// <para>包含記憶體、分頁、執行緒數量、程序數量等</para>
	/// </summary>
	/// <remarks>Reference: https://msdn.microsoft.com/en-us/library/windows/desktop/ms684824%28v=vs.85%29.aspx </remarks>
	[StructLayout(LayoutKind.Sequential)]
	public struct PerformanceInfo {
		/// <summary>The size of this structure, in bytes</summary>
		public int Size;
		/// <summary>
		/// The number of pages currently committed by the system.
		/// <para>Note that committing pages (using VirtualAlloc with MEM_COMMIT) changes this value immediately;</para>
		/// <para>however, the physical memory is not charged until the pages are accessed</para>
		/// </summary>
		public IntPtr CommitTotal;
		/// <summary>
		/// The current maximum number of pages that can be committed by the system without extending the paging file(s).
		/// <para>This number can change if memory is added or deleted, or if pagefiles have grown, shrunk, or been added.</para>
		/// <para>If the paging file can be extended, this is a soft limit.</para>
		/// </summary>
		public IntPtr CommitLimit;
		/// <summary>The maximum number of pages that were simultaneously in the committed state since the last system reboot.</summary>
		public IntPtr CommitPeak;
		/// <summary>The amount of actual physical memory, in pages.</summary>
		public IntPtr PhysicalTotal;
		/// <summary>
		/// The amount of physical memory currently available, in pages.
		/// <para>This is the amount of physical memory that can be immediately reused without having to write its contents to disk first.</para>
		/// <para> It is the sum of the size of the standby, free, and zero lists.</para>
		/// </summary>
		public IntPtr PhysicalAvailable;
		/// <summary>The amount of system cache memory, in pages. This is the size of the standby list plus the system working set.</summary>
		public IntPtr SystemCache;
		/// <summary>The sum of the memory currently in the paged and nonpaged kernel pools, in pages.</summary>
		public IntPtr KernelTotal;
		/// <summary>The memory currently in the paged kernel pool, in pages.</summary>
		public IntPtr KernelPaged;
		/// <summary>The memory currently in the nonpaged kernel pool, in pages.</summary>
		public IntPtr KernelNonPaged;
		/// <summary>The size of a page, in bytes.</summary>
		public IntPtr PageSize;
		/// <summary>The current number of open handles.</summary>
		public int HandlesCount;
		/// <summary>The current number of processes.</summary>
		public int ProcessCount;
		/// <summary>The current number of threads.</summary>
		public int ThreadCount;
	}
	#endregion

	#region Function - Support Class
	/// <summary>BIOS (Basic Input/Output System) 資訊</summary>
	public struct BiosInfo {
		/// <summary>名稱</summary>
		public string Name { get; set; }
		/// <summary>軟/韌體版本</summary>
		public string Version { get; set; }
		/// <summary>序號</summary>
		public string SerialNumber { get; set; }
	}

	/// <summary>CPU (Central Processing Unit) 資訊</summary>
	public struct CpuInfo {
		/// <summary>Processor ID</summary>
		public string ID { get; set; }
		/// <summary>序號</summary>
		public string SerialNumber { get; set; }
		/// <summary>軟/韌體版本</summary>
		public string Version { get; set; }
		/// <summary>型號名稱</summary>
		public string Name { get; set; }
		/// <summary>製造商</summary>
		public string Manufacturer { get; set; }
	}

	/// <summary>主機板資訊</summary>
	public struct MotherBoardInfo {
		/// <summary>序號</summary>
		public string SerialNumber { get; set; }
		/// <summary>製造商</summary>
		public string Manufacturer { get; set; }
		/// <summary>型號名稱</summary>
		public string Product { get; set; }
	}

	/// <summary>邏輯磁碟 (LogicalDisk) 資訊</summary>
	public struct LogicalDiskInfo {
		/// <summary>名稱</summary>
		public string Name { get; set; }
		/// <summary>磁碟大小 (Byte)</summary>
		public long Size { get; set; }
		/// <summary>可用空間 (Byte)</summary>
		public long FreeSpace { get; set; }
		/// <summary>裝置識別碼 (Device ID)</summary>
		public string DeviceID { get; set; }
		/// <summary>標籤名稱</summary>
		public string VolumeName { get; set; }
		/// <summary>系統名稱</summary>
		public string SystemName { get; set; }
		/// <summary>磁碟序號</summary>
		public string VolumeSerialNumber { get; set; }
	}

	/// <summary>實體磁碟 (PhysicalDisk) 資訊</summary>
	public struct PhysicalDiskInfo {
		/// <summary>型號</summary>
		public string Model { get; set; }
		/// <summary>名稱</summary>
		public string Name { get; set; }
		/// <summary>磁碟序號</summary>
		public string SerialNumber { get; set; }
		/// <summary>製造商</summary>
		public string Manufacturer { get; set; }
		/// <summary>介面類型</summary>
		public string InterfaceType { get; set; }
	}

	/// <summary>作業系統 (Operating System) 資訊</summary>
	public struct OsInfo {
		/// <summary>電腦名稱</summary>
		public string Name { get; set; }
		/// <summary>系統根目錄</summary>
		public string WindowsDirectory { get; set; }
		/// <summary>系統名稱</summary>
		public string Caption { get; set; }
		/// <summary>製造商</summary>
		public string Manufacturer { get; set; }
		/// <summary>版本</summary>
		public string Version { get; set; }
	}
	#endregion

	/// <summary>應用程式相關，如關閉服務、關閉外部程式等</summary>
	public static class CtApplication {

		#region Version

		/// <summary>CtApplication 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0  Ahern [2014/07/23]
		///     + 新增常用功能
		///     
		/// 1.0.1  Ahern [2014/09/12]
		///     + IsAppExist
		///     
		/// 1.1.0  Ahern [2015/03/21]
		///     + GetCPUUsage
		///     + GetFreeMemorySize
		///     + GetPerformance &amp; PerformanceInfo
		///     + GetLogicalDiskInfo &amp; LogicalDiskInfo
		///     + GetLogicalDiskSerial
		///     + GetPhysicalDiskInfo &amp; PhysicalDiskInfo
		///     + GetPhysicalDiskSerial
		///     + GetBIOSInfo
		///     + GetCPUInfo
		///     + GetMBInfo
		///     + GetOSInfo
		///     
		/// 1.1.1  Ahern [2015/05/25]
		///     \ Support Class 改以 struct 存放實值型態物件
		///     
		/// 1.1.2  Ahern [2016/04/15]
		///		\ CmdProcess 移除不必要的等待與引數
		/// 
		/// </code></remarks>
		public static CtVersion Version { get { return new CtVersion(1, 1, 2, "2016/04/15", "Ahern Kuo"); } }

		#endregion

		#region Function - Extern
		[DllImport("psapi.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetPerformanceInfo([Out] out PerformanceInfo PerformanceInfo, [In] int Size);
		#endregion

		#region Function - Methods
		/// <summary>執行「Windows 命令提示字元 (DOS/CMD/WindowsShell)」相關命令，不觀察其訊息與結果</summary>
		/// <param name="arg">欲執行之命令</param>
		/// <param name="wait">是否等待其執行結束才返回主程式</param>
		/// <example>
		/// 透過 cmd 開啟第三方程式
		/// <code language="C#">CmdProcess(@"START /WAIT D:\ABC.exe", true);</code>
		/// </example>
		private static void CmdProcess(string arg, bool wait = false) {
			/*-- 建立執行程序 --*/
			Process proc = Process.Start("CMD.EXE", "/C " + arg);

			/*-- 等待結束 --*/
			if (wait) proc.WaitForExit();
		}

		/// <summary>執行「Windows 命令提示字元 (DOS/CMD/WindowsShell)」相關命令，並等待其結果</summary>
		/// <param name="arg">欲執行之命令</param>
		/// <param name="msg">由 CMD 回傳的訊息</param>
		/// <param name="error">由 CMD 回傳的錯誤訊息</param>
		/// <example>
		/// 透過 cmd 執行重開機
		/// <code language="C#">
		/// string outMsg = string.Empty, errMsg = string.Empty;
		/// CmdProcess("SHUTDOWN /R /T 30", out outMsg, out errMsg);
		/// </code>
		/// </example>
		public static void CmdProcess(string arg, out string msg, out string error) {
			/*-- 建立執行程序 --*/
			ProcessStartInfo info = new ProcessStartInfo("CMD.EXE", "/C " + arg) {
				UseShellExecute = false,
				RedirectStandardError = true,
				RedirectStandardInput = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
			};

			/*-- 開始執行 --*/
			Process proc = Process.Start(info);

			/*-- 取得 Error 訊息 --*/
			StringBuilder sb = new StringBuilder();
			using (StreamReader sr = proc.StandardError) {
				while (sr.Peek() > 0) sb.AppendLine(sr.ReadLine()); //如果 Stream 還活著，會卡在 Peek 裡
			}
			error = sb.ToString();

			/*-- 取得一般訊息 --*/
			sb.Clear();
			using (StreamReader sr = proc.StandardOutput) {
				while (sr.Peek() > 0) sb.AppendLine(sr.ReadLine());
			}
			msg = sb.ToString();
		}

		#endregion

		#region Function - Core
		/// <summary>關閉服務</summary>
		/// <param name="service">欲關閉之服務名稱</param>
		/// <remarks>
		/// 此處方法透過 <see cref="CmdProcess(string, bool)"/> 方法來達成
		/// </remarks>
		/// <example><code language="C#">
		/// Stat stt = KillService("AceServer");
		/// </code></example>
		public static void KillService(string service) {
			string strCMD = "NET STOP " + service.Trim();
			CmdProcess(strCMD);
		}

		/// <summary>執行外部程式，可選擇是否等待其結束</summary>
		/// <param name="app">欲執行之程式名稱，請視情況帶入路徑。如需帶入參數請於第二引數帶入</param>
		/// <param name="arg">參數，如有需要會在開啟時帶入</param>
		/// <param name="wait">是否等待結束</param>
		/// <example><code language="C#">
		/// ExecuteApplication(@"D:\CASTEC\Project\CAMPro\CAMPro.exe", "monitor=true", true);
		/// </code></example>
		public static void ExecuteApplication(string app, string arg = "", bool wait = false) {
			string strCMD = string.Format(@"START {0} /MIN ""{1}"" {2}", (wait ? "/WAIT" : ""), app, arg.Trim());
			CmdProcess(strCMD, wait);
		}

		/// <summary>執行外部程式。使用 Process 方法</summary>
		/// <param name="app">欲執行之程式名稱，請視情況帶入路徑。如需帶入參數請於第二引數帶入</param>
		/// <param name="arg">參數，如有需要會在開啟時帶入</param>
		/// <param name="showWind">是否顯示視窗  (<see langword="true"/>)顯示視窗 (<see langword="false"/>)隱藏視窗</param>
		/// <returns>已建立的<see cref="Process"/></returns>
		/// <example><code language="C#">
		/// ExecuteProcess("explorer", @"D:\Software");
		/// </code></example>
		public static Process ExecuteProcess(string app, string arg = "", bool showWind = true) {
			ProcessStartInfo info = new ProcessStartInfo() {
				FileName = app,
				Arguments = arg,
				UseShellExecute = false,
				WindowStyle = showWind ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
			};
			return Process.Start(info);
		}

		/// <summary>
		/// 關閉外部執行程式
		/// <para>使用 CMD 方法，另須帶入完整程式名稱，如 "Ace.exe"</para>
		/// </summary>
		/// <param name="app">欲關閉之程式名稱，請帶入完整處理程序名稱，如 "Ace.exe"</param>
		/// <remarks>
		/// 此方法採用 TASKKILL 命令達成，須帶入完整程式名稱
		/// </remarks>
		/// <example><code language="C#">
		/// KillApplication("Ace.exe");
		/// </code></example>
		public static void KillApplication(string app) {
			string strCMD = "TASKKILL /F /IM \"" + app.Trim() + "\" /T";
			CmdProcess(strCMD);
		}

		/// <summary>
		/// 關閉外部處理程序
		/// <para>使用 Process 方法，帶入程式名稱，如 "Ace.exe" 則帶入 "Ace"</para>
		/// </summary>
		/// <param name="appName">欲關閉之程式名稱，請帶入程序名稱，如 "Ace.exe" 則帶入 "Ace"</param>
		/// <example><code language="C#">
		/// KillApplication("Ace");
		/// </code></example>
		public static void KillProcess(string appName) {
			foreach (Process proc in Process.GetProcessesByName(appName)) {
				if (proc.ProcessName.Contains(appName)) {
					proc.Kill();
					break;
				}
			}
		}

		/// <summary>
		/// 檢查當前的執行程序中是否存在特定程式
		/// <para>名稱搜尋採關鍵字，如 "AceServer.exe" 帶入 "Ace"、"Server" 即會回傳 True</para>
		/// </summary>
		/// <param name="appName">欲確認之程式名稱</param>
		/// <returns>是否正在執行 (<see langword="true"/>)此程式正在執行中 (<see langword="false"/>)不存在於當前執行程式列表中</returns>
		/// <example><code language="C#">
		/// bool bExist = IsProcessExist("Ace");
		/// </code></example>
		public static bool IsProcessExist(string appName) {
			return Process.GetProcesses().Any(proc => proc.ProcessName.Contains(appName));
		}

		/// <summary>
		/// 取得前一次計數的 CPU 使用率 (%)
		/// <para>每次間隔需 500ms 以上，且第一次數值為 0，表示開始計數</para>
		/// <para>如需抓取 CPU 使用率建議看 Example 並整合至專案中</para>
		/// </summary>
		/// <returns>前一次計數使用率 (%)</returns>
		/// <example><code language="C#">
		/// float usage = 0;
		/// PerformanceCounter cpuUsage = new PerformanceCounter("Processor", "% Processor Time", "_Total");
		/// do {
		///     usage = cpuUsage.NextValue();   //usage 即是取得的 CPU 使用率
		///     CtTimer.Delay(1000);     //每次需間隔 500ms 以上
		/// } while (true);
		/// </code></example>
		public static float GetCPUUsage() {
			PerformanceCounter usage = new PerformanceCounter("Processor", "% Processor Time", "_Total");
			return usage.NextValue();
		}

		/// <summary>
		/// 取得特定程序的 CPU 使用率 (%)
		/// <para>每次間隔需 500ms 以上，且第一次數值為 0，表示開始計數</para>
		/// <para>如需抓取 CPU 使用率建議看 Example 並整合至專案中</para>
		/// </summary>
		/// <param name="procName">欲監控的程序名稱</param>
		/// <returns>前一次計數使用率 (%)</returns>
		/// <example><code language="C#">
		/// string procName = "Ace";    //欲監控的程序名稱
		/// float usage = 0;
		/// PerformanceCounter cpuUsage = new PerformanceCounter("Process", "% Processor Time", procName);
		/// do {
		///     usage = cpuUsage.NextValue() / Environment.ProcessorCount;   //usage 即是取得的 CPU 使用率 (要除以總 CPU 核心數才是正確數值)
		///     CtTimer.Delay(1000);     //每次需間隔 500ms 以上
		/// } while (true);
		/// </code></example>
		public static float GetCPUUsage(string procName) {
			Process proc = Process.GetProcessesByName(procName)[0];
			if (proc != null) {
				PerformanceCounter usage = new PerformanceCounter("Process", "% Processor Time", proc.ProcessName);
				return usage.NextValue() / Environment.ProcessorCount;
			} else return -1;
		}

		/// <summary>
		/// 取得特定的 CPU 使用率 (%)
		/// <para>每次間隔需 500ms 以上，且第一次數值為 0，表示開始計數</para>
		/// <para>如需抓取 CPU 使用率建議看 Example 並整合至專案中</para>
		/// </summary>
		/// <param name="cpuUsage">欲取得數值的<see cref="PerformanceCounter"/></param>
		/// <returns>前一次計數使用率 (%)</returns>
		/// <example><code language="C#">
		/// float usage = 0;
		/// string procName = "Ace";    //欲監控的程序名稱
		/// PerformanceCounter cpuUsage;
		/// CtApplication.GetCPUUsage(procName, out cpuUsage);    //取得 PerformanceCount
		/// do {
		///     usage = CtApplication.GetCPUUsage(cpuUsage) / Environment.ProcessorCount;   //usage 即是取得的 CPU 使用率 (要除以總 CPU 核心數才是正確數值)
		///     CtTimer.Delay(1000);     //每次需間隔 500ms 以上
		/// } while (true);
		/// </code></example>
		public static float GetCPUUsage(PerformanceCounter cpuUsage) {
			return cpuUsage.NextValue();
		}

		/// <summary>
		/// 取得 CPU 總體效能之描述物件 <see cref="PerformanceCounter"/>
		/// <para>每次間隔需 500ms 以上，且第一次數值為 0，表示開始計數</para>
		/// <para>如需抓取 CPU 使用率建議看 Example 並整合至專案中</para>
		/// </summary>
		/// <param name="cpuUsage">欲存放的<see cref="PerformanceCounter"/></param>
		/// <example><code language="C#">
		/// float usage = 0;
		/// PerformanceCounter cpuUsage;
		/// CtApplication.GetCPUUsage(out cpuUsage);    //取得 PerformanceCount
		/// do {
		///     usage = CtApplication.GetCPUUsage(cpuUsage);    //usage 即是取得的總體 CPU 使用率
		///     CtTimer.Delay(1000);                             //每次需間隔 500ms 以上
		/// } while (true);
		/// </code></example>
		public static void GetCPUUsage(out PerformanceCounter cpuUsage) {
			cpuUsage = new PerformanceCounter("Processor", "% Processor Time", "_Total");
		}

		/// <summary>
		/// 取得特定程序的 CPU 效能描述物件 <see cref="PerformanceCounter"/>
		/// <para>每次間隔需 500ms 以上，且第一次數值為 0，表示開始計數</para>
		/// <para>如需抓取 CPU 使用率建議看 Example 並整合至專案中</para>
		/// </summary>
		/// <param name="procName">欲監控的程序名稱</param>
		/// <param name="cpuUsage">欲存放的<see cref="PerformanceCounter"/></param>
		/// <returns>前一次計數使用率 (%)</returns>
		/// <example><code language="C#">
		/// float usage = 0;
		/// string procName = "Ace";    //欲監控的程序名稱
		/// PerformanceCounter cpuUsage;
		/// CtApplication.GetCPUUsage(procName, out cpuUsage);    //取得 PerformanceCount
		/// do {
		///     usage = CtApplication.GetCPUUsage(cpuUsage) / Environment.ProcessorCount;   //usage 即是取得的 CPU 使用率 (要除以總 CPU 核心數才是正確數值)
		///     CtTimer.Delay(1000);     //每次需間隔 500ms 以上
		/// } while (true);
		/// </code></example>
		public static void GetCPUUsage(string procName, out PerformanceCounter cpuUsage) {
			Process proc = Process.GetProcessesByName(procName)[0];
			if (proc != null) {
				cpuUsage = new PerformanceCounter("Process", "% Processor Time", proc.ProcessName);
			} else cpuUsage = null;
		}

		/// <summary>
		/// 取得當前可用的記憶體空間大小
		/// <para>如需抓取可用的記憶體空間建議看 Example 並整合至專案中</para>
		/// </summary>
		/// <returns>當前可用的記憶體大小 (MB)</returns>
		/// <example><code language="C#">
		/// float availSize = 0;
		/// PerformanceCounter ramUsage = new PerformanceCounter("Memory", "Available MBytes");
		/// do {
		///     availSize = ramUsage.NextValue();   //availSize 即是可用的記憶體大小 (MB)
		///     CtTimer.Delay(100);                  //稍微 Delay 一下
		/// } while (true);
		/// </code></example>
		public static float GetFreeMemorySize() {
			PerformanceCounter ramUsage = new PerformanceCounter("Memory", "Available MBytes");
			return ramUsage.NextValue();
		}

		/// <summary>
		/// 取得當前系統效能資訊
		/// <para>包含記憶體、分頁、程序數、執行緒數等資訊。請參考 <see cref="PerformanceInfo"/></para>
		/// </summary>
		/// <param name="info">效能資訊</param>
		/// <example><code language="C#">
		/// PerformanceInfo pmInfo;
		/// CtApplication.GetPerformance(out pmInfo);
		/// 
		/// string temp = "";
		/// temp += "CommitLimit = " + pmInfo.CommitLimit.ToInt64().ToString() + CtConst.NewLine;
		/// temp += "CommitPeak = " + pmInfo.CommitPeak.ToInt64().ToString() + CtConst.NewLine;
		/// temp += "CommitTotal = " + pmInfo.CommitTotal.ToInt64().ToString() + CtConst.NewLine;
		/// temp += "KernelNonPaged = " + pmInfo.KernelNonPaged.ToInt64().ToString() + CtConst.NewLine;
		/// temp += "KernelPaged = " + pmInfo.KernelPaged.ToInt64().ToString() + CtConst.NewLine;
		/// temp += "KernelTotal = " + pmInfo.KernelTotal.ToInt64().ToString() + CtConst.NewLine;
		/// temp += "PageSize = " + pmInfo.PageSize.ToInt64().ToString() + CtConst.NewLine;
		/// temp += "PhysicalAvailable = " + pmInfo.PhysicalAvailable.ToInt64().ToString() + CtConst.NewLine;
		/// temp += "PhysicalTotal = " + pmInfo.PhysicalTotal.ToInt64().ToString() + CtConst.NewLine;
		/// temp += "HandlesCount = " + pmInfo.HandlesCount.ToString() + CtConst.NewLine;
		/// temp += "ProcessCount = " + pmInfo.ProcessCount.ToString() + CtConst.NewLine;
		/// temp += "ThreadCount = " + pmInfo.ThreadCount.ToString() + CtConst.NewLine;
		/// temp += "SystemCache = " + pmInfo.SystemCache.ToString();
		/// MessageBox.Show(temp);
		/// </code></example>
		public static void GetPerformance(out PerformanceInfo info) {
			PerformanceInfo pi = new PerformanceInfo();
			GetPerformanceInfo(out pi, Marshal.SizeOf(pi));
			info = pi;
		}

		/// <summary>
		/// 取得邏輯磁碟相關資訊
		/// <para>因此部分已有系統的 Class，如需使用請自行參考 Example 並整合至專案裡</para>
		/// </summary>
		/// <param name="info">硬碟資訊集合</param>
		/// <example><code language="C#">
		/// foreach (DriveInfo item in DriveInfo.GetDrives()) {
		///      if (item.IsReady) { /* 先檢查此磁碟能否存取。像是抓到沒有光碟的光碟機，此屬性會是 false */
		///         long availFreeSpace = item.AvailableFreeSpace;  //可用空間，與 TotalFreeSpace 一樣 (Unit: bytes)
		///         long totalFreeSapce = item.TotalFreeSpace;      //可用空間，與 AvailableFreeSpace 一樣 (Unit: bytes)
		///         long diskSize = item.TotalSize;     //整個磁碟大小 (Unit: bytes)
		///
		///         string diskName = item.Name;        //磁碟名稱 (e.g. "C:\")
		///         string format = item.DriveFormat;   //FAT32 or NTFS
		///         string volLabel = item.VolumeLabel; //磁碟區標籤 (e.g. "本機磁碟")
		///
		///         DriveType type = item.DriveType;    //磁碟類型 (CDRom、Fixed、Unknown、Network、NoRootDirectory、Ram、Removable 或 Unknown)
		///
		///         DirectoryInfo rootDir = item.RootDirectory;     //根目錄 (e.g. "C:\")
		///     }
		/// }
		/// </code></example>
		public static void GetLogicalDiskInfo(out List<DriveInfo> info) {
			info = DriveInfo.GetDrives().ToList();
		}

		/// <summary>取得邏輯磁碟 (Logical Disk) 相關資訊</summary>
		/// <param name="info">當前電腦可取得之邏輯磁碟資訊集合</param>
		/// <example><code language="C#">
		/// List&lt;LogicalDiskInfo&gt; lgInfo;
		/// CtApplication.GetLogicalDiskInfo(out lgInfo);
		/// 
		/// foreach(LogicalDiskInfo info in lgInfo) {
		///     string temp = "";
		///     temp += "Name = " + info.Name + CtConst.NewLine;
		///     temp += "SystemName = " + info.SystemName + CtConst.NewLine;
		///     temp += "VolumeName = " + info.VolumeName + CtConst.NewLine;
		///     temp += "VolumeSerialNumber = " + info.VolumeSerialNumber + CtConst.NewLine;
		///     temp += "Size = " + info.Size.ToString() + CtConst.NewLine;
		///     temp += "FreeSpace = " + info.FreeSpace.ToString();
		///     MessageBox.Show(temp);
		/// }
		/// </code></example>
		public static void GetLogicalDiskInfo(out List<LogicalDiskInfo> info) {
			List<LogicalDiskInfo> temp = new List<LogicalDiskInfo>();
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk");
			foreach (ManagementObject item in searcher.Get()) {
				try {
					LogicalDiskInfo logDisk = new LogicalDiskInfo();
					logDisk.DeviceID = item["DeviceID"].ToString();
					logDisk.FreeSpace = CtConvert.CLong(item["FreeSpace"]);
					logDisk.Name = item["Name"].ToString();
					logDisk.Size = CtConvert.CLong(item["Size"]);
					logDisk.SystemName = item["SystemName"].ToString();
					logDisk.VolumeName = item["VolumeName"].ToString();
					logDisk.VolumeSerialNumber = item["VolumeSerialNumber"].ToString();
					temp.Add(logDisk);
				} catch (Exception) {
					/*-- 有可能會抓到光碟機或虛擬機而導致跳 Exception --*/
				}
			}
			info = temp;
		}

		/// <summary>取得電腦上所有邏輯磁碟 (Logical Disk) 之序號</summary>
		/// <param name="info">&lt;名稱, 序號&gt;</param>
		/// <example><code language="C#">
		/// Dictionary&lt;string, string&gt; logicDisk;
		/// CtApplication.GetLogicalDiskSerial(out logicDisk);
		/// foreach (KeyValuePair&lt;string, string&gt; item in logicDisk) {
		///     CtInvoke.ListBoxAdd(listBox1, "[" + item.Key + "] " + item.Value); //將資訊顯示到 ListBox 裡，格式為 "[名稱] 序號"
		/// }
		/// </code></example>
		public static void GetLogicalDiskSerial(out Dictionary<string, string> info) {
			Dictionary<string, string> dict = new Dictionary<string, string>();
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk");
			foreach (ManagementObject item in searcher.Get()) {
				try {
					dict.Add(item["Name"].ToString(), item["VolumeSerialNumber"].ToString());
				} catch (Exception) {
					/*-- 有可能會抓到光碟機或虛擬機而導致跳 Exception --*/
				}
			}
			info = dict;
		}

		/// <summary>取的特定邏輯磁碟 (Logical Disk) 序號 (VolumeSerialNumber)</summary>
		/// <param name="diskName">欲取得的磁碟機名稱。格式如 "C:" "D:"</param>
		/// <returns>取得的磁碟序號</returns>
		/// <example><code language="C#">
		/// MessageBox.Show(CtApplication.GetLogicalDiskSerial("D:"));  //取得 D 槽序號
		/// </code></example>
		public static string GetLogicalDiskSerial(string diskName) {
			string strTemp = "";
			ManagementObject obj = new ManagementObject("Win32_LogicalDisk.DeviceID=\"" + diskName + "\"");
			if (obj != null) strTemp = obj.GetPropertyValue("VolumeSerialNumber").ToString();
			return strTemp;
		}

		/// <summary>取得實體磁碟 (Physical Disk) 相關資訊</summary>
		/// <param name="info">當前電腦可取得之實體磁碟資訊集合</param>
		/// <example><code language="C#">
		/// List&lt;PhysicalDiskInfo&gt; phyInfo;
		/// CtApplication.GetPhysicalDiskInfo(out phyIndo);
		/// 
		/// foreach(PhysicalDiskInfo info in phyInfo) {
		///     string temp = "";
		///     temp += "Name = " + info.Name + CtConst.NewLine;
		///     temp += "Model = " + info.Model + CtConst.NewLine;
		///     temp += "Manufacturer = " + info.Manufacturer + CtConst.NewLine;
		///     temp += "InterfaceType = " + info.InterfaceType + CtConst.NewLine;
		///     temp += "SerialNumber = " + info.SerialNumber;
		///     MessageBox.Show(temp);
		/// }
		/// </code></example>
		public static void GetPhysicalDiskInfo(out List<PhysicalDiskInfo> info) {
			List<PhysicalDiskInfo> temp = new List<PhysicalDiskInfo>();
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
			foreach (ManagementObject item in searcher.Get()) {
				try {
					PhysicalDiskInfo phyDisk = new PhysicalDiskInfo();
					phyDisk.Name = item["Name"].ToString();
					phyDisk.SerialNumber = item["SerialNumber"].ToString().Trim();
					phyDisk.Model = item["Model"].ToString();
					phyDisk.Manufacturer = item["Manufacturer"].ToString();
					phyDisk.InterfaceType = item["InterfaceType"].ToString();
					temp.Add(phyDisk);
				} catch (Exception) {
					/*-- 有可能會抓到光碟機或虛擬機而導致跳 Exception --*/
				}
			}
			info = temp;
		}

		/// <summary>取得電腦上所有實體磁碟 (Physical Disk) 序號</summary>
		/// <param name="info">&lt;磁碟代號, 序號&gt;</param>
		/// <example><code language="C#">
		/// Dictionary&lt;string, string&gt; physDisk;
		/// CtApplication.GetPhysicalDiskSerial(out physDisk);
		/// foreach (KeyValuePair&lt;string, string&gt; item in physDisk) {
		///     CtInvoke.ListBoxAdd(listBox1, "[" + item.Key + "] " + item.Value);  //將資訊顯示到 ListBox 裡，格式為 "[代號] 序號"
		/// }
		/// </code></example>
		public static void GetPhysicalDiskSerial(out Dictionary<string, string> info) {
			Dictionary<string, string> dict = new Dictionary<string, string>();
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
			foreach (ManagementObject item in searcher.Get()) {
				try {
					dict.Add(item["Tag"].ToString().Replace("\\\\.\\PHYSICALDRIVE", "").Trim(), item["SerialNumber"].ToString().Trim());
				} catch (Exception) {
					/*-- 有可能會抓到光碟機或虛擬機而導致跳 Exception --*/
				}
			}
			info = dict;
		}

		/// <summary>取的特定實體磁碟機 (Physical Disk) 序號</summary>
		/// <param name="driveNum">欲取得的磁碟機ID，如 "0"</param>
		/// <returns>取得的磁碟序號</returns>
		/// <example><code language="C#">
		/// MessageBox.Show(CtApplication.GetPhysicalDiskSerial(0));  //取得編號 0 之磁碟機序號
		/// </code></example>
		public static string GetPhysicalDiskSerial(byte driveNum) {
			string strTemp = "";
			ManagementObject obj = new ManagementObject("Win32_PhysicalMedia.Tag=\"\\\\\\\\.\\\\PHYSICALDRIVE" + driveNum.ToString() + "\"");
			if (obj != null) strTemp = obj.GetPropertyValue("SerialNumber").ToString();
			return strTemp;
		}

		/// <summary>取得 BIOS (Basic Input/Output System) 資訊</summary>
		/// <param name="info">BIOS 資訊</param>
		/// <example><code language="C#">
		/// BiosInfo bios;
		/// CtApplication.GetBIOSInfo(out bios);
		/// 
		/// string temp = "";
		/// temp += "Name = " + bios.Name + CtConst.NewLine;
		/// temp += "Version = " + bios.Version + CtConst.NewLine;
		/// temp += "SerialNumber = " + bios.SerialNumber;
		/// MessageBox.Show(temp);
		/// </code></example>
		public static void GetBIOSInfo(out BiosInfo info) {
			BiosInfo bios = new BiosInfo();
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
			foreach (ManagementObject item in searcher.Get()) {
				bios.Name = item["Name"].ToString();
				bios.SerialNumber = item["SerialNumber"].ToString().Trim();
				bios.Version = item["Version"].ToString();
			}
			info = bios;
		}

		/// <summary>取得 CPU (Central Processing Unit) 資訊</summary>
		/// <param name="info">CPU 資訊</param>
		/// <example><code language="C#">
		/// CpuInfo cpu;
		/// CtApplication.GetCPUInfo(out cpu);
		/// 
		/// string temp = "";
		/// temp += "ID = " + cpu.ID + CtConst.NewLine;
		/// temp += "Name = " + cpu.Name + CtConst.NewLine;
		/// temp += "Manufacturer = " + cpu.Manufacturer + CtConst.NewLine;
		/// temp += "SerialNumber = " + cpu.SerialNumber + CtConst.NewLine;
		/// temp += "Version = " + cpu.Version;
		/// MessageBox.Show(temp);
		/// </code></example>
		public static void GetCPUInfo(out CpuInfo info) {
			CpuInfo cpu = new CpuInfo();
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
			foreach (ManagementObject item in searcher.Get()) {
				cpu.Name = item["Name"].ToString();
				cpu.SerialNumber = item["ProcessorId"].ToString();
				cpu.Version = item["Version"].ToString();
				cpu.Manufacturer = item["Manufacturer"].ToString();
			}
			info = cpu;
		}

		/// <summary>取得主機板資訊</summary>
		/// <param name="info">主機板資訊</param>
		/// <example><code language="C#">
		/// MotherBoardInfo mb;
		/// CtApplication.GetMBInfo(out mb);
		/// 
		/// string temp = "";
		/// temp += "Product = " + mb.Product + CtConst.NewLine;
		/// temp += "Manufacturer = " + mb.Manufacturer + CtConst.NewLine;
		/// temp += "SerialNumber = " + mb.SerialNumber;
		/// MessageBox.Show(temp);
		/// </code></example>
		public static void GetMBInfo(out MotherBoardInfo info) {
			MotherBoardInfo mb = new MotherBoardInfo();
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
			foreach (ManagementObject item in searcher.Get()) {
				mb.SerialNumber = item["SerialNumber"].ToString();
				mb.Manufacturer = item["Manufacturer"].ToString();
				mb.Product = item["Product"].ToString();
			}
			info = mb;
		}

		/// <summary>取得主機板資訊</summary>
		/// <param name="info">主機板資訊</param>
		/// <example><code language="C#">
		/// MotherBoardInfo mb;
		/// CtApplication.GetMBInfo(out mb);
		/// 
		/// string temp = "";
		/// temp += "Product = " + mb.Product + CtConst.NewLine;
		/// temp += "Manufacturer = " + mb.Manufacturer + CtConst.NewLine;
		/// temp += "SerialNumber = " + mb.SerialNumber;
		/// MessageBox.Show(temp);
		/// </code></example>
		public static void GetOSInfo(out OsInfo info) {
			OsInfo os = new OsInfo();
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
			foreach (ManagementObject item in searcher.Get()) {
				os.Name = item["csname"].ToString();
				os.WindowsDirectory = item["WindowsDirectory"].ToString();
				os.Caption = item["Caption"].ToString();
				os.Manufacturer = item["Manufacturer"].ToString();
				os.Version = item["Version"].ToString();
			}
			info = os;
		}
		#endregion
	}

}
