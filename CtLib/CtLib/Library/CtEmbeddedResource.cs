using System.IO;
using System.Reflection;
using System.Linq;

namespace CtLib.Library {

	/// <summary>提供讀取內嵌資源(Embedded Resource)之相關操作</summary>
	public static class CtEmbdResx {

		#region Version

		/*-----------------------------------
		 *	1.0.0	Ahern	[2016/06/16]
		 *		+ 建立基礎模組與方法
		 *		
		 *----------------------------------- */

		#endregion

		#region Embedded Resource
		/// <summary>取得 CtLib 之內嵌資源</summary>
		/// <param name="name">資源名稱，如 "Language.xml"</param>
		/// <returns>該資源之串流</returns>
		public static Stream GetEmbdResx(string name) {
			Assembly asm = Assembly.GetExecutingAssembly();
			string resxName = asm.GetManifestResourceNames().Where(val => val.Contains(name))?.First();
			return asm.GetManifestResourceStream(resxName);
		}

		/// <summary>取得 CtLib 之內嵌資源</summary>
		/// <param name="asm">欲搜尋資源的組件</param>
		/// <param name="name">資源名稱，如 "Language.xml"</param>
		/// <returns>該資源之串流</returns>
		public static Stream GetEmbdResx(Assembly asm, string name) {
			string resxName = asm.GetManifestResourceNames().Where(val => val.Contains(name))?.First();
			return asm.GetManifestResourceStream(resxName);
		}
		#endregion
	}
}
