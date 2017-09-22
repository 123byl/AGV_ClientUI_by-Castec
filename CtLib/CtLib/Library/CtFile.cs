using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using CtLib.Module.Ultity;

namespace CtLib.Library {

    /// <summary>
    /// 路徑與檔案處理相關
    /// <para>包含了檢查檔案或路徑是否存在、取得檔名或是搜尋特定資料夾內檔案等功能</para>
    /// </summary>
    public static class CtFile {

        #region Version

        /// <summary>CtFile 版本訊息</summary>
        /// <remarks><code>
        /// 1.0.0  Ahern [2014/07/12]
        ///     + BackSlash 從舊 CtLib 轉移至此
        ///     + 新增常用之建立路徑、取得檔名等功能
        ///     
        /// 1.0.1  Ahern [2014/07/14]
        ///     + 新增ReadFile、WriteFile，並使用StreamReader/Writer達成
        ///     
        /// 1.0.2  Ahern [2014/08/31]
        ///     \ 修正CreatePath之路徑應在最後加上反斜線，避免IsDirectoryExist判斷錯誤
        ///     
        /// 1.1.0  Ahern [2014/09/10]
        ///     + CopyFile
        ///     + DeleteFile
        ///     
        /// 1.2.0  Ahern [2014/12/10]
        ///     + GetFileNames
        ///     + GetFolderNames
        ///     + GetFileInformation
        ///     
        /// 1.2.1  Ahern [2015/04/23]
        ///     \ GetFileNames 增添時間選項
        ///     \ GetFolderNames 增添時間選項
        ///     
        /// 1.3.0  Ahern [2015/05/08]
        ///     + Ini 檔案相關操作
        ///     
        /// 1.4.0  Ahern [2015/05/22]
        ///     \ ReadFile 與 WriteFile 改以 using 實作
        ///     + 插入式 WriteFile
        ///     
        /// 1.4.1  Ahern [2015/05/25]
        ///     \ string[] 與 List&lt;string&gt; 改以 IEnumerable&lt;string&gt;
        ///     
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 4, 1, "2015/05/25", "Ahern Kuo");

        #endregion

        #region Declaration - Definitions
        /// <summary>The maximum length of array buffer that using for getting ini value</summary>
        private static readonly int INI_MAXIMUM_LENGTH = 2048;
        #endregion

        #region Declaration - Support Class

        /// <summary>檔案相關訊息</summary>
        public class FileInformation {
            /// <summary>檔案建立日期</summary>
            public DateTime CreateTime { get; internal set; }
            /// <summary>最後編輯日期</summary>
            public DateTime LastEditTime { get; internal set; }
            /// <summary>上次存取日期</summary>
            public DateTime LastAccessTime { get; internal set; }
            /// <summary>檔案大小</summary>
            public long FileSize { get; internal set; }

            /// <summary>建立一新的檔案訊息</summary>
            /// <param name="create">檔案建立日期</param>
            /// <param name="edit">最後編輯日期</param>
            /// <param name="access">上次存取日期</param>
            /// <param name="size">檔案大小</param>
            public FileInformation(DateTime create, DateTime edit, DateTime access, long size) {
                CreateTime = create;
                LastEditTime = edit;
                LastAccessTime = access;
                FileSize = size;
            }
        }

        #endregion

        #region Declaration - Windows API
        /// <summary>寫入 INI 資料</summary>
        /// <param name="section">Section</param>
        /// <param name="key">Key</param>
        /// <param name="val">Value</param>
        /// <param name="filePath">檔案路徑</param>
        /// <returns>(0)寫入失敗  (其他)成功</returns>
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(
            string section,
            string key,
            string val,
            string filePath
        );

        /// <summary>讀取 INI 資料</summary>
        /// <param name="section">Section</param>
        /// <param name="key">Key</param>
        /// <param name="def">Default Value (When cannot find the section and key requested, return this value)</param>
        /// <param name="retVal">Returned Value from specified section and key</param>
        /// <param name="size">Buffer size of the receiving object</param>
        /// <param name="filePath">File full path. like @"D:\abc.ini"</param>
        /// <returns></returns>
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(
            string section,
            string key,
            string def,
            StringBuilder retVal,
            int size,
            string filePath
        );
        #endregion

        #region Functions - Core
        /// <summary>強制字串以反斜線結尾，如已存在則不做更動</summary>
        /// <param name="str">欲檢查之字串</param>
        /// <returns>檢查完畢字串</returns>
        public static string BackSlash(string str) {
            string strTemp = str;
            if (!strTemp.EndsWith("\\")) strTemp += "\\";
            return strTemp;
        }

        /// <summary>建立資料夾路徑，如該路徑已存在則不做任何動作</summary>
        /// <param name="path">路徑字串</param>
        public static void CreatePath(string path) {
            string strDir = BackSlash(GetPath(path));   //避免餵進來的包含其他資訊，所以先取得完成資料夾路徑名稱
            if (!IsDirectoryExist(strDir))              //檢查路徑是否存在
                Directory.CreateDirectory(strDir);      //如不存在則建立之
        }

        /// <summary>建立空白檔案，如該檔案已存在則依 overWrite 引數決定是否覆寫</summary>
        /// <param name="path">路徑與檔名字串。如 @"D:\Config\Profuct.txt"</param>
        /// <param name="overWrite">是否覆寫? (True)覆寫  (False)不覆寫</param>
        public static void CreateFile(string path, bool overWrite = false) {
            if (!IsFileExist(path)) {   //如果檔案不存在，建立路徑並建立檔案
                CreatePath(path);
                File.WriteAllText(path, "", Encoding.UTF8);
            } else if (overWrite) {     //如果存在，則依照overwrite去決定要不要改成空白檔案
                File.WriteAllText(path, "", Encoding.UTF8);
            }
        }

        /// <summary>檢查檔案是否存在</summary>
        /// <param name="path">檔案路徑</param>
        /// <returns>是否存在</returns>
        public static bool IsFileExist(string path) {
            return File.Exists(path);
        }

        /// <summary>檢查路徑是否存在，請勿帶入含有檔案名稱之字串</summary>
        /// <param name="path">資料夾路徑</param>
        /// <returns>是否存在</returns>
        public static bool IsDirectoryExist(string path) {
            return Directory.Exists(GetPath(BackSlash(path)));
        }

        /// <summary>取得路徑完整字串</summary>
        /// <param name="path">欲取得路徑字串之字串</param>
        /// <returns>資料夾路徑</returns>
        public static string GetPath(string path) {
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// 取得完整路徑字串中的檔案名稱
        /// <para>如 @"D:\DataFile.txt" 將得到 "DataFile"</para>
        /// </summary>
        /// <param name="path">欲取得檔案名稱之字串</param>
        /// <param name="extension">是否包含副檔名? (True)包含副檔名  (False)不含副檔名</param>
        /// <returns>檔案名稱</returns>
        public static string GetFileName(string path, bool extension = true) {
            return (extension) ? Path.GetFileName(path) : Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>取得特定路徑裡之所有名稱檔案</summary>
        /// <param name="path">欲搜尋之路徑</param>
        /// <param name="subFolder">是否搜尋子資料夾? (True)搜尋所有資料夾  (False)只回傳該層資料夾</param>
        /// <param name="extension">回傳路徑是否包含副檔名? (True)"D:\TempFile.txt" (False)"TempFile"</param>
        /// <returns>所有符合之檔案路徑</returns>
        public static List<string> GetFileNames(string path, bool subFolder = true, bool extension = false) {
            List<string> strFiles = Directory.GetFiles(path, "*.*", ((subFolder) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                                    .Select(val => extension ? val : GetFileName(val, false))
                                    .ToList();
            return strFiles;
        }

        /// <summary>取得特定路徑裡之所有名稱檔案，且附帶檔案建立時間條件</summary>
        /// <param name="path">欲搜尋之路徑</param>
        /// <param name="BKForeAfter">建立時間點? (True)於 buildTime (含)之後  (False)於 buildTime 之前</param>
        /// <param name="buildTime">欲比較的建立時間點</param>
        /// <param name="subFolder">是否搜尋子資料夾? (True)搜尋所有資料夾  (False)只回傳該層資料夾</param>
        /// <param name="extension">回傳路徑是否包含副檔名</param>
        /// <returns>所有符合之檔案路徑</returns>
        public static List<string> GetFileNames(string path, DateTime buildTime, bool BKForeAfter = false, bool subFolder = true, bool extension = false) {
            List<string> strTemp = new List<string>();

            List<string> strFiles = Directory.GetFiles(path, "*.*", ((subFolder) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).ToList();

            if (strFiles.Count > 0) {
                strFiles.ForEach(
                    val => {
                        FileInfo info = new FileInfo(val);
                        if (BKForeAfter && info.CreationTime >= buildTime) strTemp.Add(GetFileName(val, extension));
                        else if (!BKForeAfter && info.CreationTime < buildTime) strTemp.Add(GetFileName(val, extension));
                    }
                );
            }

            return strTemp;
        }

        /// <summary>取得特定路徑裡之所有名稱檔案，且附帶檔案建立時間條件</summary>
        /// <param name="path">欲搜尋之路徑</param>
        /// <param name="buildTime">欲比較的起始時間點</param>
        /// <param name="lastTime">欲比較的結束時間點</param>
        /// <param name="subFolder">是否搜尋子資料夾? (True)搜尋所有資料夾  (False)只回傳該層資料夾</param>
        /// <param name="extension">回傳路徑是否包含副檔名</param>
        /// <returns>所有符合之檔案路徑</returns>
        public static List<string> GetFileNames(string path, DateTime buildTime, DateTime lastTime, bool subFolder = true, bool extension = false) {
            List<string> strTemp = new List<string>();

            List<string> strFiles = Directory.GetFiles(path, "*.*", ((subFolder) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).ToList();

            if (strFiles.Count > 0) {
                strFiles.ForEach(
                    val => {
                        FileInfo info = new FileInfo(val);
                        if (buildTime <= info.CreationTime && info.CreationTime <= lastTime) strTemp.Add(GetFileName(val, extension));
                    }
                );
            }

            return strTemp;
        }

        /// <summary>取得特定路徑裡符合搜尋樣式之所有名稱檔案</summary>
        /// <param name="path">欲搜尋之路徑</param>
        /// <param name="pattern">搜尋樣式。如 "*.*"、"*.txt"、"test.*"、"test.txt" 等等</param>
        /// <param name="subFolder">是否搜尋子資料夾? (True)搜尋所有資料夾  (False)只回傳該層資料夾</param>
        /// <param name="extension">回傳路徑是否包含副檔名</param>
        /// <returns>所有符合之檔案路徑</returns>
        public static List<string> GetFileNames(string path, string pattern, bool subFolder = true, bool extension = false) {
            List<string> strTemp = new List<string>();

            List<string> strFiles = Directory.GetFiles(path, pattern, ((subFolder) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).ToList();

            if (strFiles.Count > 0) {
                if (!extension) strFiles.ForEach(val => strTemp.Add(GetFileName(val, false)));
                else strTemp.AddRange(strFiles);
            }

            return strTemp;
        }

        /// <summary>取得特定路徑裡符合搜尋樣式之所有名稱檔案，且附帶檔案建立時間條件</summary>
        /// <param name="path">欲搜尋之路徑</param>
        /// <param name="pattern">搜尋樣式。如 "*.*"、"*.txt"、"test.*"、"test.txt" 等等</param>
        /// <param name="BKForeAfter">建立時間點? (True)於 buildTime (含)之後  (False)於 buildTime 之前</param>
        /// <param name="buildTime">欲比較的建立時間點</param>
        /// <param name="subFolder">是否搜尋子資料夾? (True)搜尋所有資料夾  (False)只回傳該層資料夾</param>
        /// <param name="extension">回傳路徑是否包含副檔名</param>
        /// <returns>所有符合之檔案路徑</returns>
        public static List<string> GetFileNames(string path, string pattern, DateTime buildTime, bool BKForeAfter = false, bool subFolder = true, bool extension = false) {
            List<string> strTemp = new List<string>();

            List<string> strFiles = Directory.GetFiles(path, pattern, ((subFolder) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).ToList();

            if (strFiles.Count > 0) {
                strFiles.ForEach(
                    val => {
                        FileInfo info = new FileInfo(val);
                        if (BKForeAfter && info.CreationTime >= buildTime) strTemp.Add(GetFileName(val, extension));
                        else if (!BKForeAfter && info.CreationTime < buildTime) strTemp.Add(GetFileName(val, extension));
                    }
                );
            }

            return strTemp;
        }

        /// <summary>取得特定路徑裡符合搜尋樣式之所有名稱檔案，且附帶檔案建立時間條件</summary>
        /// <param name="path">欲搜尋之路徑</param>
        /// <param name="buildTime">欲比較的起始時間點</param>
        /// <param name="pattern">搜尋樣式。如 "*.*"、"*.txt"、"test.*"、"test.txt" 等等</param>
        /// <param name="lastTime">欲比較的結束時間點</param>
        /// <param name="subFolder">是否搜尋子資料夾? (True)搜尋所有資料夾  (False)只回傳該層資料夾</param>
        /// <param name="extension">回傳路徑是否包含副檔名</param>
        /// <returns>所有符合之檔案路徑</returns>
        public static List<string> GetFileNames(string path, string pattern, DateTime buildTime, DateTime lastTime, bool subFolder = true, bool extension = false) {
            List<string> strTemp = new List<string>();

            List<string> strFiles = Directory.GetFiles(path, pattern, ((subFolder) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).ToList();

            if (strFiles.Count > 0) {
                strFiles.ForEach(
                    val => {
                        FileInfo info = new FileInfo(val);
                        if (buildTime <= info.CreationTime && info.CreationTime <= lastTime) strTemp.Add(GetFileName(val, extension));
                    }
                );
            }

            return strTemp;
        }

        /// <summary>取得檔案相關訊息，如建立時間等</summary>
        /// <param name="path">欲取得訊息之檔案路徑</param>
        /// <returns>檔案相關訊息 <see cref="FileInformation"/></returns>
        /// <remarks>檔案相關訊息來自 <see cref="FileInfo"/></remarks>
        public static FileInformation GetFileInformation(string path) {
            FileInformation info = null;

            FileInfo fileInfo = new FileInfo(path);
            info = new FileInformation(fileInfo.CreationTime, fileInfo.LastWriteTime, fileInfo.LastAccessTime, fileInfo.Length);

            return info;
        }

        /// <summary>取得特定路徑裡之所有資料夾名稱</summary>
        /// <param name="path">欲搜尋之路徑</param>
        /// <param name="subFolder">是否搜尋子資料夾? (True)搜尋所有資料夾  (False)只回傳該層資料夾</param>
        /// <returns>所有符合之資料夾路徑</returns>
        public static List<string> GetFolderNames(string path, bool subFolder = true) {
            List<string> strFiles = Directory.GetDirectories(path, "*", ((subFolder) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).ToList();
            return (strFiles.Count > 0) ? strFiles : null;
        }

        /// <summary>取得特定路徑裡之所有資料夾名稱，且附帶資料夾建立時間條件</summary>
        /// <param name="path">欲搜尋之路徑</param>
        /// <param name="BKForeAfter">建立時間點? (True)於 buildTime (含)之後  (False)於 buildTime 之前</param>
        /// <param name="buildTime">欲比較的建立時間點</param>
        /// <param name="subFolder">是否搜尋子資料夾? (True)搜尋所有資料夾  (False)只回傳該層資料夾</param>
        /// <returns>所有符合之資料夾路徑</returns>
        public static List<string> GetFolderNames(string path, DateTime buildTime, bool BKForeAfter = false, bool subFolder = true) {
            List<string> strTemp = new List<string>();

            List<string> strFiles = Directory.GetDirectories(path, "*.*", ((subFolder) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).ToList();

            if (strFiles.Count > 0) {
                strFiles.ForEach(
                    val => {
                        DirectoryInfo info = new DirectoryInfo(val);
                        if (BKForeAfter && info.CreationTime >= buildTime) strTemp.Add(val);
                        else if (!BKForeAfter && info.CreationTime < buildTime) strTemp.Add(val);
                    }
                );
            }

            return strTemp;
        }

        /// <summary>取得特定路徑裡之所有資料夾名稱，且附帶資料夾建立時間條件</summary>
        /// <param name="path">欲搜尋之路徑</param>
        /// <param name="buildTime">欲比較的起始時間點</param>
        /// <param name="lastTime">欲比較的結束時間點</param>
        /// <param name="subFolder">是否搜尋子資料夾? (True)搜尋所有資料夾  (False)只回傳該層資料夾</param>
        /// <returns>所有符合之資料夾路徑</returns>
        public static List<string> GetFolderNames(string path, DateTime buildTime, DateTime lastTime, bool subFolder = true) {
            List<string> strTemp = new List<string>();

            List<string> strFiles = Directory.GetFiles(path, "*.*", ((subFolder) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).ToList();

            if (strFiles.Count > 0) {
                strFiles.ForEach(
                    val => {
                        DirectoryInfo info = new DirectoryInfo(val);
                        if (buildTime <= info.CreationTime && info.CreationTime <= lastTime) strTemp.Add(val);
                    }
                );
            }

            return strTemp;
        }

        /// <summary>取得特定路徑裡符合搜尋樣式之所有資料夾名稱</summary>
        /// <param name="path">欲搜尋之路徑</param>
        /// <param name="pattern">搜尋樣式。如 "*"、"test*" 等等</param>
        /// <param name="subFolder">是否搜尋子資料夾? True:搜尋所有資料夾 False:只回傳該層資料夾</param>
        /// <returns>所有符合之資料夾路徑</returns>
        public static List<string> GetFolderNames(string path, string pattern, bool subFolder = true) {
            List<string> strFiles = Directory.GetDirectories(path, pattern, ((subFolder) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).ToList();
            return (strFiles.Count > 0) ? strFiles : null;
        }

        /// <summary>取得特定路徑裡符合搜尋樣式之所有資料夾名稱，且附帶資料夾建立時間條件</summary>
        /// <param name="path">欲搜尋之路徑</param>
        /// <param name="pattern">搜尋樣式。如 "*"、"test*" 等等</param>
        /// <param name="BKForeAfter">建立時間點? (True)於 buildTime (含)之後  (False)於 buildTime 之前</param>
        /// <param name="buildTime">欲比較的建立時間點</param>
        /// <param name="subFolder">是否搜尋子資料夾? (True)搜尋所有資料夾  (False)只回傳該層資料夾</param>
        /// <returns>所有符合之資料夾路徑</returns>
        public static List<string> GetFolderNames(string path, string pattern, DateTime buildTime, bool BKForeAfter = false, bool subFolder = true) {
            List<string> strTemp = new List<string>();

            List<string> strFiles = Directory.GetDirectories(path, pattern, ((subFolder) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).ToList();

            if (strFiles.Count > 0) {
                strFiles.ForEach(
                    val => {
                        DirectoryInfo info = new DirectoryInfo(val);
                        if (BKForeAfter && info.CreationTime >= buildTime) strTemp.Add(val);
                        else if (!BKForeAfter && info.CreationTime < buildTime) strTemp.Add(val);
                    }
                );
            }

            return strTemp;
        }

        /// <summary>取得特定路徑裡符合搜尋樣式之所有資料夾名稱，且附帶資料夾建立時間條件</summary>
        /// <param name="path">欲搜尋之路徑</param>
        /// <param name="pattern">搜尋樣式。如 "*"、"test*" 等等</param>
        /// <param name="buildTime">欲比較的起始時間點</param>
        /// <param name="lastTime">欲比較的結束時間點</param>
        /// <param name="subFolder">是否搜尋子資料夾? (True)搜尋所有資料夾  (False)只回傳該層資料夾</param>
        /// <returns>所有符合之資料夾路徑</returns>
        public static List<string> GetFolderNames(string path, string pattern, DateTime buildTime, DateTime lastTime, bool subFolder = true) {
            List<string> strTemp = new List<string>();

            List<string> strFiles = Directory.GetFiles(path, pattern, ((subFolder) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).ToList();

            if (strFiles.Count > 0) {
                strFiles.ForEach(
                    val => {
                        DirectoryInfo info = new DirectoryInfo(val);
                        if (buildTime <= info.CreationTime && info.CreationTime <= lastTime) strTemp.Add(val);
                    }
                );
            }

            return strTemp.ToList();
        }

        /// <summary>讀取檔案並回傳該文件內容集合</summary>
        /// <param name="path">檔案路徑，如 @"D:\CASTEC.txt"</param>
        /// <returns>該文件String集合</returns>
        public static List<string> ReadFile(string path) {
            List<string> lstTemp = new List<string>();  /* 用於儲存該文件每一列 */

            /*-- 建立 StreamReader 並開啟檔案 (使用UTF8格式) --*/
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8)) {
                /*-- 如果Buffer內還有文字沒抓，就把他抓出來 (一次一行) --*/
                while (sr.Peek() > 0) lstTemp.Add(sr.ReadLine());
            }

            return lstTemp;
        }

        /// <summary>讀取檔案並回傳該文件字串集合</summary>
        /// <param name="path">檔案路徑，如 @"D:\CASTEC.txt"</param>
        /// <param name="content">欲儲存String集合之物件</param>
        public static void ReadFile(string path, out List<string> content) {
            List<string> lstTemp = new List<string>();

            /*-- 建立 StreamReader 並開啟檔案 (使用UTF8格式) --*/
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8)) {
                /*-- 如果Buffer內還有文字沒抓，就把他抓出來 (一次一行) --*/
                while (sr.Peek() > 0) lstTemp.Add(sr.ReadLine());
            }

            content = lstTemp;
        }

        /// <summary>將字串物件寫入特定檔案</summary>
        /// <param name="path">檔案路徑，如 @"D:\CASTEC.txt"</param>
        /// <param name="content">欲寫入之String集合物件</param>
        /// <param name="append">是否附加於文件後面? (True)繼續往後附加上去   (False)覆寫原始文件</param>
        public static void WriteFile(string path, IEnumerable<string> content, bool append = false) {
            /*-- 建立 StreamWriter 並開啟檔案 (使用UTF8格式) --*/
            using (StreamWriter sw = new StreamWriter(path, append, Encoding.UTF8)) {
                /*-- 如果Buffer內還有文字沒抓，就把他抓出來 (一次一行) --*/
                foreach (string val in content) sw.WriteLine(val);
            }
        }

        /// <summary>將字串物件寫入特定檔案</summary>
        /// <param name="path">檔案路徑，如 @"D:\CASTEC.txt"</param>
        /// <param name="content">欲寫入之字串</param>
        /// <param name="append">是否附加於文件後面? (True)繼續往後附加上去   (False)覆寫原始文件</param>
        public static void WriteFile(string path, string content, bool append = false) {
            /*-- 建立 StreamWriter 並開啟檔案 (使用UTF8格式) --*/
            using (StreamWriter sw = new StreamWriter(path, append, Encoding.UTF8)) {
                /*-- 寫入檔案 --*/
                sw.WriteLine(content);
            }
        }

        /// <summary>將字串物件插入特定檔案</summary>
        /// <param name="path">檔案路徑，如 @"D:\CASTEC.txt"</param>
        /// <param name="content">欲寫入之字串</param>
        /// <param name="insertIdx">欲插入的位置，索引從 0 開始算起</param>
        public static void WriteFile(string path, int insertIdx, string content) {
            List<string> strOri;
            ReadFile(path, out strOri);
            strOri.Insert(insertIdx, content);
            WriteFile(path, strOri, false);
        }

        /// <summary>將字串物件插入特定檔案</summary>
        /// <param name="path">檔案路徑，如 @"D:\CASTEC.txt"</param>
        /// <param name="content">欲寫入之字串</param>
        /// <param name="insertIdx">欲插入的位置，索引從 0 開始算起</param>
        public static void WriteFile(string path, int insertIdx, IEnumerable<string> content) {
            List<string> strOri;
            ReadFile(path, out strOri);
            strOri.InsertRange(insertIdx, content);
            WriteFile(path, strOri, false);
        }

        /// <summary>複製檔案至另一路徑</summary>
        /// <param name="oriPath">欲複製之原始檔案路徑。如 @"D:\ori.awp"</param>
        /// <param name="destPath">欲存放之目的地路徑。如 @"D:\Backup\dest.awp"</param>
        /// <param name="overWrite">如目的地已有相同名稱之檔案，是否覆蓋? (True)覆蓋  (False)放棄並發Exception</param>
        public static void CopyFile(string oriPath, string destPath, bool overWrite = true) {
            File.Copy(oriPath, destPath, overWrite);
        }

        /// <summary>刪除特定路徑之檔案</summary>
        /// <param name="path">欲刪除之檔案路徑</param>
        public static void DeleteFile(string path) {
            File.Delete(path);
        }

        /// <summary>刪除特定路徑之檔案</summary>
        /// <param name="path">欲刪除之檔案路徑</param>
        /// <param name="forceContents">是否強制刪除所有資料</param>
        public static void DeleteDirectory(string path, bool forceContents) {
            Directory.Delete(path, forceContents);
        }

        /// <summary>刪除特定路徑裡建立於特定時間之前的檔案</summary>
        /// <param name="path">欲刪除之檔案路徑</param>
        /// <param name="time">欲比較的時間點，若檔案建立時間比這時間早則刪除之。(僅小於，非小於等於)</param>
        /// <param name="subFolder">是否搜尋子目錄</param>
        public static void DeleteFile(string path, DateTime time, bool subFolder = true) {
            if (!IsDirectoryExist(path)) return;
            List<string> fileNames = GetFileNames(path, time, false, subFolder, true);
            if (fileNames != null && fileNames.Count > 0)
                fileNames.ForEach(val => File.Delete(val));
        }

        /// <summary>
        /// 刪除特定路徑裡建立於特定時間之前的資料夾
        /// <para>可將 foreceContents 設為 false 即檢查 path 所帶入的資料夾是否符合時間點並刪除之</para>
        /// </summary>
        /// <param name="path">欲刪除之檔案路徑</param>
        /// <param name="time">欲比較的時間點，若檔案建立時間比這時間早則刪除之。(僅小於，非小於等於)</param>
        /// <param name="forceContents">是否搜尋子目錄？如要檢查 path 引數之時間，此選項可設為 false 即直接檢查該資料夾</param>
        public static void DeleteDirectory(string path, DateTime time, bool forceContents = true) {
            if (!IsDirectoryExist(path)) return;
            if (!forceContents) {
                /*-- 因為要檢查子資料夾的時間，所以要一個一個去找 --*/
                List<string> folderNames = GetFolderNames(path, time, false, forceContents);
                if (folderNames != null && folderNames.Count > 0)
                    folderNames.ForEach(val => Directory.Delete(val, true));
            } else {
                DirectoryInfo info = new DirectoryInfo(path);
                if (info.CreationTime < time) Directory.Delete(path, true);
            }
        }

        /// <summary>刪除特定路徑裡建立於特定時間之前的檔案</summary>
        /// <param name="path">欲刪除之檔案路徑</param>
        /// <param name="buildTime">欲比較的起始時間點</param>
        /// <param name="lastTime">欲比較的結束時間點</param>
        /// <param name="subFolder">是否搜尋子目錄</param>
        public static void DeleteFile(string path, DateTime buildTime, DateTime lastTime, bool subFolder = true) {
            if (!IsDirectoryExist(path)) return;
            List<string> fileNames = GetFileNames(path, buildTime, lastTime, subFolder, true);
            if (fileNames != null && fileNames.Count > 0)
                fileNames.ForEach(val => File.Delete(val));
        }

        /// <summary>
        /// 刪除特定路徑裡建立於特定時間之前的資料夾
        /// <para>可將 foreceContents 設為 false 即檢查 path 所帶入的資料夾是否符合時間點並刪除之</para>
        /// </summary>
        /// <param name="path">欲刪除之檔案路徑</param>
        /// <param name="buildTime">欲比較的起始時間點</param>
        /// <param name="lastTime">欲比較的結束時間點</param>
        /// <param name="forceContents">是否搜尋子目錄？如要檢查 path 引數之時間，此選項可設為 false 即直接檢查該資料夾</param>
        public static void DeleteDirectory(string path, DateTime buildTime, DateTime lastTime, bool forceContents = true) {
            if (!IsDirectoryExist(path)) return;
            if (!forceContents) {
                List<string> folderNames = GetFolderNames(path, buildTime, lastTime, forceContents);
                if (folderNames != null && folderNames.Count > 0)
                    folderNames.ForEach(val => Directory.Delete(val, true));
            } else {
                DirectoryInfo info = new DirectoryInfo(path);
                if (buildTime <= info.CreationTime && info.CreationTime <= lastTime) Directory.Delete(path, true);
            }
        }

        /// <summary>讀取 INI 檔案之特定資料</summary>
        /// <param name="filePath">檔案路徑。如 @"D:\data.ini"</param>
        /// <param name="section">Section 名稱</param>
        /// <param name="key">Key 名稱</param>
        /// <param name="value">讀取到的數值</param>
        public static void IniReadString(string filePath, string section, string key, out string value) {
            StringBuilder sb = new StringBuilder(INI_MAXIMUM_LENGTH);
            GetPrivateProfileString(section, key, "", sb, INI_MAXIMUM_LENGTH, filePath);
            value = sb.ToString();
        }

        /// <summary>讀取 INI 檔案之特定資料</summary>
        /// <param name="filePath">檔案路徑。如 @"D:\data.ini"</param>
        /// <param name="section">Section 名稱</param>
        /// <param name="key">Key 名稱</param>
        /// <param name="defVal">預設值。如果沒有讀到資料，將會以此數值作為回傳</param>
        /// <param name="value">讀取到的數值</param>
        public static void IniReadString(string filePath, string section, string key, string defVal, out string value) {
            StringBuilder sb = new StringBuilder(INI_MAXIMUM_LENGTH);
            GetPrivateProfileString(section, key, defVal, sb, INI_MAXIMUM_LENGTH, filePath);
            value = sb.ToString();
        }

        /// <summary>寫入特定資料至 INI 檔案</summary>
        /// <param name="filePath">檔案路徑。如 @"D:\data.ini"</param>
        /// <param name="section">Section 名稱</param>
        /// <param name="key">Key 名稱</param>
        /// <param name="value">欲寫入的數值</param>
        /// <returns>是否寫入成功</returns>
        public static bool IniWriteString(string filePath, string section, string key, object value) {
            long result = WritePrivateProfileString(section, key, value.ToString(), filePath);
            return Convert.ToBoolean(result);
        }
        #endregion
    }
}
