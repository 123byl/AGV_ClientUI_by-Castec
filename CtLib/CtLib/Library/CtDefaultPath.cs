using System.IO;

using CtLib.Module.Utility;

namespace CtLib.Library {

	/// <summary>
	/// 取得當前設定的路徑
	/// <para></para>
	/// <para>主資料夾 @"D:\CASTEC"</para>
	/// <para>  ├ Config  相關設定檔</para>
	/// <para>  ├   ├ *.opc      專案 Opcode</para>
	/// <para>  ├   ├ *.xml      專案相關設定檔</para>
	/// <para>  ├   └ UserManage 使用者帳號密碼設定檔</para>
	/// <para>  ├ Log     紀錄檔</para>
	/// <para>  ├ Project 專案程式</para>
	/// <para>  └ Recipe  程式配方或參數</para>
	/// </summary>
	public static class CtDefaultPath {

		#region Version

		/// <summary>CtDefaultPath 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0  Ahern [2015/02/02]
		///     + GetDirectoryName
		///     
		/// 1.0.1  Ahern [2015/02/06]
		///     + SetPath
		///     + CheckDirectory
		///     + DeleteDirectory
		/// </code></remarks>
		public static CtVersion Version { get { return new CtVersion(1, 0, 1, "2015/02/06", "Ahern Kuo"); } }

		#endregion

		#region Function - Core
		/// <summary>取得當前的相關專案路徑 (結尾已含反斜線)</summary>
		/// <param name="pathType">欲取得的路徑。如 Config 資料夾、Log 資料夾等</param>
		/// <returns>完整的路徑字串。如 @"D:\CASTEC\Config\"</returns>
		public static string GetPath(SystemPath pathType) {
			string strTemp = CtFile.BackSlash(Properties.Settings.Default.PATH_MAIN);
			switch (pathType) {
				case SystemPath.Configuration:
					strTemp += CtFile.BackSlash(Properties.Settings.Default.FOLD_CONFIG);
					break;
				case SystemPath.Log:
					strTemp += CtFile.BackSlash(Properties.Settings.Default.FOLD_LOG);
					break;
				case SystemPath.Recipe:
					strTemp += CtFile.BackSlash(Properties.Settings.Default.FOLD_RECIPE);
					break;
				case SystemPath.Project:
					strTemp += CtFile.BackSlash(Properties.Settings.Default.FOLD_PROJECT);
					break;
				case SystemPath.UserManagement:
					strTemp += CtFile.BackSlash(Properties.Settings.Default.FOLD_CONFIG);
					strTemp += Properties.Settings.Default.FILE_USERMANAGE;
					break;
				default:
					break;
			}
			return strTemp;
		}

		/// <summary>設定 CtLib 之預設路徑。範例請看 Example</summary>
		/// <param name="pathType">欲取得的路徑。如 Config 資料夾、Log 資料夾等</param>
		/// <param name="keyWord">欲更改的名稱。如 "D:\New CASTEC\"</param>
		/// <example><code language="C#">
		/// CtDefaultPath.SetPath(SystemPath.MainDirectory, @"D:\CASTEC\");  //更改 CtLib 主目錄
		/// CtDefaultPath.SetPath(SystemPath.Configuration, "Config");               //設定檔 Config 目錄名稱(僅名稱不含路徑)
		/// CtDefaultPath.SetPath(SystemPath.Log, "Log");                     //紀錄檔 Log 存放目錄名稱(僅名稱不含路徑)
		/// CtDefaultPath.SetPath(SystemPath.Recipe, "Recipe");               //參數檔 Recipe 目錄名稱(僅名稱不含路徑)
		/// CtDefaultPath.SetPath(SystemPath.Project, "Project");             //執行專案 Project 目錄名稱(僅名稱不含路徑)
		/// CtDefaultPath.SetPath(SystemPath.UserManagement, "UserManage.bin"); //使用者紀錄檔檔名(名稱含附檔名，不含路徑)
		/// </code></example>
		public static void SetPath(SystemPath pathType, string keyWord) {
			DeleteDirectory(pathType);
			switch (pathType) {
				case SystemPath.MainDirectory:
					Properties.Settings.Default.PATH_MAIN = keyWord;
					break;
				case SystemPath.Configuration:
					Properties.Settings.Default.FOLD_CONFIG = keyWord;
					break;
				case SystemPath.Log:
					Properties.Settings.Default.FOLD_LOG = keyWord;
					break;
				case SystemPath.Recipe:
					Properties.Settings.Default.FOLD_RECIPE = keyWord;
					break;
				case SystemPath.Project:
					Properties.Settings.Default.FOLD_PROJECT = keyWord;
					break;
				case SystemPath.UserManagement:
					Properties.Settings.Default.FILE_USERMANAGE = keyWord;
					break;
			}
			Properties.Settings.Default.Save();
			CheckDirectory();
		}

		/// <summary>刪除特定路徑之資料夾。如有東西則不刪除，空的資料夾則刪除之</summary>
		/// <param name="pathType">欲刪除的路徑。如 Config 資料夾、Log 資料夾等</param>
		private static void DeleteDirectory(SystemPath pathType) {
			if (CtFile.IsDirectoryExist(GetPath(pathType))) {
				DirectoryInfo info = new DirectoryInfo(GetPath(pathType));
				if (info.GetFiles().Length == 0 && info.GetDirectories().Length == 0) {
					CtFile.DeleteDirectory(GetPath(pathType), true);
				}
			}
		}

		/// <summary>檢查 CtLib 相關路徑是否存在，如果不存在則建立之</summary>
		public static void CheckDirectory() {
			if (!CtFile.IsDirectoryExist(GetPath(SystemPath.MainDirectory))) CtFile.CreateDirectory(GetPath(SystemPath.MainDirectory));
			if (!CtFile.IsDirectoryExist(GetPath(SystemPath.Configuration))) CtFile.CreateDirectory(GetPath(SystemPath.Configuration));
			if (!CtFile.IsDirectoryExist(GetPath(SystemPath.Log))) CtFile.CreateDirectory(GetPath(SystemPath.Log));
			if (!CtFile.IsDirectoryExist(GetPath(SystemPath.Project))) CtFile.CreateDirectory(GetPath(SystemPath.Project));
			if (!CtFile.IsDirectoryExist(GetPath(SystemPath.Recipe))) CtFile.CreateDirectory(GetPath(SystemPath.Recipe));
			if (!CtFile.IsFileExist(GetPath(SystemPath.UserManagement))) CtFile.CreateFile(GetPath(SystemPath.UserManagement));
		}
		#endregion
	}
}
