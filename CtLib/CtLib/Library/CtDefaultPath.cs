using CtLib.Module.Ultity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

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
        /// <remarks><code>
        /// 1.0.0  Ahern [2015/02/02]
        ///     + GetPath
        ///     
        /// 1.0.1  Ahern [2015/02/06]
        ///     + SetPath
        ///     + CheckDirectory
        ///     + DeleteDirectory
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 0, 1, "2015/02/06", "Ahern Kuo");

        #endregion

        #region Function - Core
        /// <summary>取得當前的相關專案路徑 (結尾已含反斜線)</summary>
        /// <param name="pathType">欲取得的路徑。如 Config 資料夾、Log 資料夾等</param>
        /// <returns>完整的路徑字串。如 @"D:\CASTEC\Config\"</returns>
        public static string GetPath(SystemPath pathType) {
            string strTemp = CtFile.BackSlash(Properties.Settings.Default.PATH_MAIN);
            switch (pathType) {
                case SystemPath.CONFIG:
                    strTemp += CtFile.BackSlash(Properties.Settings.Default.FOLD_CONFIG);
                    break;
                case SystemPath.LOG:
                    strTemp += CtFile.BackSlash(Properties.Settings.Default.FOLD_LOG);
                    break;
                case SystemPath.RECIPE:
                    strTemp += CtFile.BackSlash(Properties.Settings.Default.FOLD_RECIPE);
                    break;
                case SystemPath.PROJECT:
                    strTemp += CtFile.BackSlash(Properties.Settings.Default.FOLD_PROJECT);
                    break;
                case SystemPath.USER_MANAGER:
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
        /// <example><code>
        /// CtDefaultPath.SetPath(SystemPath.MAIN_DIRECTORY, @"D:\CASTEC\");  //更改 CtLib 主目錄
        /// CtDefaultPath.SetPath(SystemPath.CONFIG, "Config");               //設定檔 Config 目錄名稱(僅名稱不含路徑)
        /// CtDefaultPath.SetPath(SystemPath.LOG, "Log");                     //紀錄檔 Log 存放目錄名稱(僅名稱不含路徑)
        /// CtDefaultPath.SetPath(SystemPath.RECIPE, "Recipe");               //參數檔 Recipe 目錄名稱(僅名稱不含路徑)
        /// CtDefaultPath.SetPath(SystemPath.PROJECT, "Project");             //執行專案 Project 目錄名稱(僅名稱不含路徑)
        /// CtDefaultPath.SetPath(SystemPath.USER_MANAGER, "UserManage.bin"); //使用者紀錄檔檔名(名稱含附檔名，不含路徑)
        /// </code></example>
        public static void SetPath(SystemPath pathType, string keyWord) {
            DeleteDirectory(pathType);
            switch (pathType) {
                case SystemPath.MAIN_DIRECTORY:
                    Properties.Settings.Default.PATH_MAIN = keyWord;
                    break;
                case SystemPath.CONFIG:
                    Properties.Settings.Default.FOLD_CONFIG = keyWord;
                    break;
                case SystemPath.LOG:
                    Properties.Settings.Default.FOLD_LOG = keyWord;
                    break;
                case SystemPath.RECIPE:
                    Properties.Settings.Default.FOLD_RECIPE = keyWord;
                    break;
                case SystemPath.PROJECT:
                    Properties.Settings.Default.FOLD_PROJECT = keyWord;
                    break;
                case SystemPath.USER_MANAGER:
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
            if (!CtFile.IsDirectoryExist(GetPath(SystemPath.MAIN_DIRECTORY))) CtFile.CreatePath(GetPath(SystemPath.MAIN_DIRECTORY));
            if (!CtFile.IsDirectoryExist(GetPath(SystemPath.CONFIG))) CtFile.CreatePath(GetPath(SystemPath.CONFIG));
            if (!CtFile.IsDirectoryExist(GetPath(SystemPath.LOG))) CtFile.CreatePath(GetPath(SystemPath.LOG));
            if (!CtFile.IsDirectoryExist(GetPath(SystemPath.PROJECT))) CtFile.CreatePath(GetPath(SystemPath.PROJECT));
            if (!CtFile.IsDirectoryExist(GetPath(SystemPath.RECIPE))) CtFile.CreatePath(GetPath(SystemPath.RECIPE));
            if (!CtFile.IsFileExist(GetPath(SystemPath.USER_MANAGER))) CtFile.CreateFile(GetPath(SystemPath.USER_MANAGER));
        } 
        #endregion
    }
}
