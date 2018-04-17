using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtLib.Module.XML {

	/// <summary>提供可轉換成 <see cref="XmlElmt"/> 之物件，請額外實作「建構元(<see cref="XmlElmt"/>)」之建構元方法以供呼叫</summary>
	internal interface IXmlOperable {

		#region Public Operations
		/// <summary>產生此物件相對應的 <see cref="IXmlData"/></summary>
		/// <param name="nodeName">欲產生的節點名稱</param>
		/// <returns>對應的 <see cref="IXmlData"/></returns>
		IXmlData CreateXmlNode(string nodeName);
		#endregion

	}
}
