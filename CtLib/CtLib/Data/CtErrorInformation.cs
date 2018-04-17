using System;
using System.Collections.Generic;

using CtLib.Module.XML;

namespace CtLib.Library {

    /// <summary>
    /// 錯誤相關訊息
    /// <para>內含時間、代碼、解決方法等。亦可用於錯誤發報</para>
    /// </summary>
    public class CtErrorInfo : IXmlOperable {

		#region Fields
		private DateTime mTime = DateTime.Now;
		private Devices mDev = Devices.CAMPro;
		private int mErrorCode = 0;
		private string mErrorInfo = string.Empty;
		private string mErrorSol = string.Empty;
		private string mErrorTitle = string.Empty;
		#endregion

		#region Properties
		/// <summary>事件發生之時間點</summary>
		public string Time { get { return mTime.ToString("HH:mm:ss.ff"); } }
		/// <summary>設備</summary>
		public Devices Device { get { return mDev; } }
		/// <summary>錯誤代碼</summary>
		public int ErrorCode { get { return mErrorCode; } }
		/// <summary>錯誤資訊</summary>
		public string Information { get { return mErrorInfo; } }
		/// <summary>解除方法</summary>
		public string Solution { get { return mErrorSol; } }
		/// <summary>錯誤訊息之標題</summary>
		public string Title { get { return mErrorTitle; } }
		#endregion

		#region Constructors
		/// <summary>建立帶有預設值的警報資訊</summary>
		/// <param name="device">設備</param>
		/// <param name="errCode">錯誤代碼</param>
		/// <param name="errInfo">錯誤資訊</param>
		/// <param name="errSol">解除方法</param>
		public CtErrorInfo(Devices device, int errCode, string errInfo, string errSol = "") {
			mTime = DateTime.Now;
			mDev = device;
			mErrorCode = errCode;
			mErrorInfo = errInfo;
			mErrorSol = errSol;
		}

		/// <summary>建立帶有預設值的警報資訊 (含標題)</summary>
		/// <param name="device">設備</param>
		/// <param name="errCode">錯誤代碼</param>
		/// <param name="errInfo">錯誤資訊</param>
		/// <param name="errSol">解除方法</param>
		/// <param name="errTitle">錯誤訊息標題</param>
		public CtErrorInfo(Devices device, int errCode, string errTitle, string errInfo, string errSol = "") {
			mTime = DateTime.Now;
			mDev = device;
			mErrorCode = errCode;
			mErrorInfo = errInfo;
			mErrorSol = errSol;
			mErrorTitle = errTitle;
		}

		/// <summary>使用 <see cref="XmlElmt"/> 進行建構</summary>
		/// <param name="data">含有所需資訊的 <see cref="IXmlData"/></param>
		public CtErrorInfo(XmlElmt data) {
			data.Elements().ForEach(
				param => {
					switch (param.Attribute("Link").Value) {
						case "Device":
							mDev = (Devices)Enum.Parse(typeof(Devices), param.Value);
							break;
						case "ErrorCode":
							mErrorCode = int.Parse(param.Value);
							break;
						case "Information":
							mErrorInfo = param.Value;
							break;
						case "Solution":
							mErrorSol = param.Value;
							break;
						default:
							throw new InvalidCastException("無法解析的 XML 節點");
					}
				}
			);
		}
		#endregion

		#region Public Operations
		/// <summary>取得此錯誤發生的時間點</summary>
		/// <returns>時間</returns>
		public DateTime GetErrorTime() { return mTime; }
		#endregion

		#region Overrides
		/// <summary>取得此錯誤之描述字串</summary>
		/// <returns>描述字串</returns>
		public override string ToString() {
			return string.Format("{0},{1},{2}", mDev.ToString(), mErrorCode.ToString(), mErrorTitle);
		}

		/// <summary>取得此錯誤之湊雜碼</summary>
		/// <returns>湊雜碼</returns>
		public override int GetHashCode() {
			return (int)mDev ^ mErrorCode ^ mErrorTitle.GetHashCode() ^ mErrorInfo.GetHashCode();
		}

		/// <summary>比較兩個物件是否相同，以 <see cref="Device"/>, <seealso cref="Time"/>, <seealso cref="ErrorCode"/> 與 <seealso cref="Title"/> 為準</summary>
		/// <param name="obj">欲比較之物件</param>
		/// <returns>(<see langword="true"/>)兩者相同  (<see langword="false"/>)兩者不相同</returns>
		public override bool Equals(object obj) {
			if (obj != null && obj is CtErrorInfo) {
				if ((object)this == null) return false;
				if (ReferenceEquals(this, obj)) return true;
				CtErrorInfo compare = obj as CtErrorInfo;
				return (mDev == compare.Device) && (mErrorCode == compare.ErrorCode) && (mErrorTitle == compare.Title) && (this.Time == compare.Time);
			} else return false;
		}

		/// <summary>產生此物件相對應的 <see cref="XmlElmt"/></summary>
		/// <param name="nodeName">欲產生的節點名稱</param>
		/// <returns>對應的 <see cref="XmlElmt"/></returns>
		public IXmlData CreateXmlNode(string nodeName) {
			return new XmlElmt(
				nodeName,
				new XmlElmt("param", mDev.ToString(), new XmlAttr("Link", "Device")),
				new XmlElmt("param", mErrorCode.ToString(), new XmlAttr("Link", "ErrorCode")),
				new XmlElmt("param", mErrorInfo.ToString(), new XmlAttr("Link", "Information")),
				new XmlElmt("param", mErrorSol.ToString(), new XmlAttr("Link", "Solution"))
			);
		}

		/// <summary>比較兩個物件是否相同，以 <see cref="Device"/>, <seealso cref="Time"/>, <seealso cref="ErrorCode"/> 與 <seealso cref="Title"/> 為準</summary>
		/// <param name="a">欲比較之物件</param>
		/// <param name="b">欲被比較之物件</param>
		/// <returns>(<see langword="true"/>)兩者相同  (<see langword="false"/>)兩者不相同</returns>
		public static bool operator ==(CtErrorInfo a, CtErrorInfo b) {
			if ((object)a == null || (object)b == null) return false;
			if (ReferenceEquals(a, b)) return true;
			return (a.Device == b.Device) && (a.ErrorCode == b.ErrorCode) && (a.Title == b.Title) && (a.Time == b.Time);
		}

		/// <summary>比較兩個物件是否不同，以 <see cref="Device"/>, <seealso cref="Time"/>, <seealso cref="ErrorCode"/> 與 <seealso cref="Title"/> 為準</summary>
		/// <param name="a">欲比較之物件</param>
		/// <param name="b">欲被比較之物件</param>
		/// <returns>(<see langword="true"/>)兩者不相同  (<see langword="false"/>)兩者相同</returns>
		public static bool operator !=(CtErrorInfo a, CtErrorInfo b) {
			return !(a == b);
		} 
		#endregion
	}
}
