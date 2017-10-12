using System;
using System.IO;
using System.Linq;

using Ace.Core.Server;

namespace CtLib.Module.Adept {

	/// <summary>Adept ACE 檔案相關操作，如將 ACE 物件匯出、匯入</summary>
	public static class CtAceFile {

		#region Saving Files
		/// <summary>將特定的 ACE 資料夾或物件匯出成本機檔案</summary>
		/// <param name="tarObj">欲儲存的資料夾或檔案</param>
		/// <param name="fileName">欲儲存的檔案名稱，如 @"D:\cvt.awp"</param>
		/// <example><code language="C#">
		/// IAceObject visionFolder = ace.Root["/Program/Vision"];	//可為資料夾或檔案
		/// CtAceFile.ExportFile(visionFolder, @"D:\CVT_Backup\now.awp");
		/// </code></example>
		public static void ExportFile(IAceObject tarObj, string fileName) {
			using (Stream stream = new FileStream(fileName, FileMode.Create)) {
				tarObj.SaveToZip(stream, false);
			}
		}

		/// <summary>將特定的 ACE 資料夾或物件匯出成本機檔案</summary>
		/// <param name="ace">當前的 <see cref="IAceServer"/>，於 CVT 或 C# 即為預設的全域變數 "ace"</param>
		/// <param name="objPath">欲儲存的資料夾或檔案路徑，如 "/Program/Vision"</param>
		/// <param name="fileName">欲儲存的檔案名稱，如 @"D:\cvt.awp"</param>
		/// <example><code language="C#">
		/// IAceObjectCollection visionFolder = ace.Root["/Program/Vision"] as IAceObjectCollection;
		/// CtAceFile.ExportFile(ace, "/Program/Vision", @"D:\CVT_Backup\now.awp");
		/// </code></example>
		public static void ExportFile(IAceServer ace, string objPath, string fileName) {
			IAceObject obj = ace.Root[objPath];
			if (obj == null) throw new FileNotFoundException("Could not find Adept ACE object", objPath);
			using (Stream stream = new FileStream(fileName, FileMode.Create)) {
				obj.SaveToZip(stream, false);
			}
		}
		#endregion

		#region Import Files
		/// <summary>匯入本機檔案至特定的 ACE 資料夾內。如路徑不存在則會自動建立目錄</summary>
		/// <param name="ace">當前的 <see cref="IAceServer"/>，於 CVT 或 C# 即為預設的全域變數 "ace"</param>
		/// <param name="tarFold">欲匯入的資料夾路徑，如 "/Program/Vision"。若為此參數為空則匯入至根目錄</param>
		/// <param name="fileName">欲匯入的本機檔案，如 @"D:\cvt_backup.awp"</param>
		/// <example><code language="C#">
		/// string file = @"D:\cvt_backup.awp";
		/// CtAceFile.ImportFile(ace, "/Program/Vision", @"D:\vision.awp");	//載入檔案至 /Program/Vision
		/// CtAceFile.ImportFile(ace, "", @"D:\empty.awp");	//載入檔案至根目錄
		/// </code></example>
		public static void ImportFile(IAceServer ace, string tarFold, string fileName) {
			IAceObjectCollection obj = ace.Root[tarFold] as IAceObjectCollection;
			using (Stream stream = new FileStream(fileName, FileMode.Open)) {
				if (obj == null) {
					string[] pathSplit = tarFold.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
					if (pathSplit != null && pathSplit.Length > 0) {
						IAceObjectCollection fold = ace.Root;
						string tempPath = string.Empty;
						foreach (string name in pathSplit) {
							if (fold == null) throw new ArgumentNullException("fold", "Creating folder failed");
							IAceObject item = fold.ToArray().FirstOrDefault(val => val.Name == name);
							if (item == null) item = fold.AddCollection(name);
							fold = item as IAceObjectCollection;
						}
						if (fold != null) {
							fold.LoadFromZip(stream);
							fold.CheckInternalReferences();
						}
					} else {
						ace.Root.LoadFromZip(stream);
						ace.Root.CheckInternalReferences();
					}
				} else {
					obj.LoadFromZip(stream);
					obj.CheckInternalReferences();
				}
			}
		}
		#endregion

		#region Delete Object

		/// <summary>刪除指定的 <see cref="IAceObject"/>，可為物件、資料夾等</summary>
		/// <param name="aceObj">欲刪除的物件</param>
		public static void DeleteAceObject(IAceObject aceObj) {
			IAceObjectCollection fold = aceObj as IAceObjectCollection;
			if (fold == null) {
				try {
					aceObj.Dispose();
				} catch (Exception ex) {
					Console.WriteLine(ex.ToString());
				}
			} else {
				IAceObject[] innerItems = fold.ToArray();
				foreach (IAceObject item in innerItems) {
					DeleteAceObject(item);
				}
				fold.CheckInternalReferences();
				try {
					fold.Dispose();
				} catch (Exception ex) {
					Console.WriteLine(ex.ToString());
				}
			}
		}

		#endregion
	}
}
