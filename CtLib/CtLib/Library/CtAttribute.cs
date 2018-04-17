using System;

namespace CtLib.Library {

	/// <summary>描述文化語系之關鍵詞</summary>
	public class CultureKey : Attribute {

		#region Fields
		/// <summary>關鍵詞</summary>
		private string mKeyWord = string.Empty;
		#endregion

		#region Properties
		/// <summary>取得關鍵詞</summary>
		public string KeyWord { get { return mKeyWord; } }
		#endregion

		#region Constructors
		/// <summary>建立文化語系之關鍵詞</summary>
		/// <param name="keyWord">關鍵詞</param>
		public CultureKey(string keyWord) {
			mKeyWord = keyWord;
		}
		#endregion
	}
}
