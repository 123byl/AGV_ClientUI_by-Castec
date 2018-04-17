using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

using CtLib.Module.Utility;

namespace CtLib.Library {

	#region Declaration - Enumerations

	/// <summary>儲存於 FTP 站台上的檔案類型</summary>
	public enum FtpObjectType {
		/// <summary>此物件為檔案</summary>
		File = 1,
		/// <summary>此物件為資料夾</summary>
		Directory = 2
	}

	#endregion

	#region Function - Support Class
	/// <summary>嘗試是否能存取遠端裝置的結果</summary>
	public class PingResult {

		#region Fields
		private IPAddress mAddr = null;
		private byte[] mBuf = null;
		private string mBufStr = string.Empty;
		private long mTime = 0;
		private PingStatus mStt = PingStatus.Unknown;
		#endregion

		#region Properties
		/// <summary>取得目標遠端裝置的位址</summary>
		public IPAddress TargetAddress { get { return mAddr; } }
		/// <summary>取得嘗試存取遠端裝置的回傳緩衝資料</summary>
		public byte[] ReturnedBuffer { get { return mBuf.ToArray(); } }
		/// <summary>取得嘗試存取遠端裝置的回傳緩衝資料字串</summary>
		public string ReturnedBufferString { get { return Encoding.UTF8.GetString(mBuf); } }
		/// <summary>取得此次嘗試存取遠端裝置所消耗的時間</summary>
		public TimeSpan Time { get { return TimeSpan.FromMilliseconds(mTime); } }
		/// <summary>取得此次嘗試存取遠端裝置的狀態</summary>
		public PingStatus PingState { get { return mStt; } }
		#endregion

		#region Constructors
		/// <summary>建構自訂義的存取遠端裝置的結果</summary>
		/// <param name="ip">目標遠端裝置的位址，如 "192.168.0.1"</param>
		/// <param name="retBuf">存取遠端裝置的回傳緩衝資料</param>
		/// <param name="time">存取遠端裝置所消耗的時間(毫秒)</param>
		/// <param name="stt">存取遠端裝置的狀態</param>
		public PingResult(string ip, byte[] retBuf, long time, PingStatus stt) {
			mAddr = IPAddress.Parse(ip);
			mBuf = retBuf.ToArray();
			mTime = time;
			mStt = stt;
		}

		/// <summary>藉由系統 <see cref="PingReply"/> 建構存取遠端裝置的結果</summary>
		/// <param name="reply">由 Ping.Send / Ping.AsyncSend 之回傳結果</param>
		public PingResult(PingReply reply) {
			mStt = (PingStatus)reply.Status;
			if (mStt == PingStatus.Success) {
				mAddr = reply.Address;
				mBuf = reply.Buffer;
				mTime = reply.RoundtripTime;
			}
		}

		/// <summary>藉由系統 <see cref="PingReply"/> 建構存取遠端裝置的結果，並帶入預設位址與逾時時間</summary>
		/// <param name="reply">由 Ping.Send / Ping.AsyncSend 之回傳結果</param>
		/// <param name="defaultIP">當存取失敗時所帶入的目標裝置地址</param>
		/// <param name="timeOut">發生存取失敗時所設定的逾時時間</param>
		public PingResult(PingReply reply, string defaultIP, int timeOut = 5000) {
			mStt = (PingStatus)reply.Status;
			if (mStt == PingStatus.Success) {
				mAddr = reply.Address;
				mBuf = reply.Buffer;
				mTime = reply.RoundtripTime;
			} else {
				mAddr = IPAddress.Parse(defaultIP);
				mTime = timeOut;
			}
		}
		#endregion

		#region Overrides
		/// <summary>取得此遠端裝置存取結果的文字描述</summary>
		/// <returns>存取結果的文字描述</returns>
		public override string ToString() {
			if (mStt == PingStatus.Success) return string.Format("Ping {0} at {1} with {2} ms.", mStt.ToString().ToLower(), mAddr.ToString(), mTime.ToString());
			else return string.Format("Ping failed, {0} replied.", mStt.ToString().ToLower());
		}
		#endregion
	}

	/// <summary>儲存於 FTP 站台上的物件資訊</summary>
	public class FtpObject {

		#region Fields
		private string mAcsLv = string.Empty;
		private string mType = string.Empty;
		private string mOwner = string.Empty;
		private string mGroup = string.Empty;
		private string mFileSize = string.Empty;
		private string mMonth = string.Empty;
		private string mDate = string.Empty;
		private string mTime = string.Empty;
		private string mFileName = string.Empty;
		#endregion

		#region Properties
		/// <summary>取得此物件的權限資訊</summary>
		public string AccessInformation { get { return mAcsLv; } }
		/// <summary>取得此物件的檔案類型</summary>
		public FtpObjectType ObjectType { get { return mType == "1" ? FtpObjectType.File : FtpObjectType.Directory; } }
		/// <summary>取得此物件的擁有者</summary>
		public string Owner { get { return mOwner; } }
		/// <summary>取得此物件之使用者群組</summary>
		public string Group { get { return mGroup; } }
		/// <summary>取得此物件之大小，單位為「位元組(Bytes)」</summary>
		public long FileSize { get { return long.Parse(mFileSize); } }
		/// <summary>取得此物件之時間戳</summary>
		public DateTime TimeStamp { get { return DateTime.Parse(string.Format("{0}/{1}/{2} {3}", mMonth, mDate, DateTime.Now.Year, mTime)); } }
		/// <summary>取得此物件的名稱</summary>
		public string FileName { get { return mFileName; } }
		#endregion

		#region Constructors
		/// <summary>根據 FTP 所收到的訊息來建構物件訊息</summary>
		/// <param name="data">收到的訊息</param>
		public FtpObject(string data) {
			string[] split = data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			mAcsLv = split[0];
			mType = split[1];
			mOwner = split[2];
			mGroup = split[3];
			mFileSize = split[4];
			mMonth = split[5];
			mDate = split[6];
			mTime = split[7];
			mFileName = split[8];
		}
		#endregion

		#region Overrides
		/// <summary>取得此物件的文字描述</summary>
		/// <returns>物件的文字描述</returns>
		public override string ToString() {
			return string.Format("{0}, {1}, {2}, {3}, {4}", mFileName, ObjectType.ToString(), mFileSize, mOwner, TimeStamp.ToString("MM/dd HH:mm"));
		} 
		#endregion
	}

	/// <summary>提供 FTP 進度更新用的事件參數</summary>
	public class FtpProgressChangedEventArgs : EventArgs {

		#region Fields
		private long mCurBytes = 0;
		private long mTotBytes = 0;
		private string mName = string.Empty;
		#endregion

		#region Properties
		/// <summary>取得此事件所更新的檔案名稱</summary>
		public string FileName { get { return mName; } }
		/// <summary>取得或設定當前已處理的位元組(Bytes)</summary>
		public long BytesProcessed { get { return mCurBytes; } set { mCurBytes = value; } }
		/// <summary>取得總檔案大小</summary>
		public long TotalBytesToProcess { get { return mTotBytes; } }
		/// <summary>取的當前的百分比</summary>
		public double Percent { get { return ((double)mCurBytes / mTotBytes * 100); } }
		#endregion

		#region Constructors
		/// <summary>建構 FTP 物件之上傳或下載進度更新事件參數</summary>
		/// <param name="fileName">檔案名稱</param>
		/// <param name="totalBytes">檔案大小，單位為位元組(Bytes)</param>
		public FtpProgressChangedEventArgs(string fileName, long totalBytes) {
			mName = fileName;
			mTotBytes = totalBytes;
		}
		#endregion

		#region Overrides
		/// <summary>取得此事件之文字描述</summary>
		/// <returns>文字描述</returns>
		public override string ToString() {
			return string.Format("{0}, {1:F1}% ({2}/{3})", mName, Percent, BytesProcessed, TotalBytesToProcess);
		} 
		#endregion
	}
	#endregion

	#region Function - Network Operations
	/// <summary>網路相關操作</summary>
	public static class CtNetwork {

		#region Version
		/// <summary>CtNetwork 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0	Ahern	[2015/12/08]
		///     + Ping 相關操作
		///     
		/// 1.1.0	Ahern	[2016/04/22]
		///		+ FTP 相關操作
		///     
		/// </code></remarks>
		public static CtVersion Version { get { return new CtVersion(1, 1, 0, "2016/04/22", "Ahern Kuo"); } }
		#endregion

		/// <summary>查詢是否有可以使用的網路連線，未完成</summary>
		/// <returns>(<see langword="true"/>)有網路可使用  (<see langword="false"/>)沒有網路可使用</returns>
		public static bool IsAnyNetworkAvailable() {
			bool available = false;
			if (NetworkInterface.GetIsNetworkAvailable()) {
				NetworkInterface[] niColl = NetworkInterface.GetAllNetworkInterfaces();
				foreach (NetworkInterface ni in niColl) {
					if (ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback && ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel) {
						IPInterfaceProperties iip = ni.GetIPProperties();
						if (iip != null && iip.UnicastAddresses != null && iip.UnicastAddresses.Count > 0)
							available = true;
						else
							available = false;
					}
				}
			}
			return available;
		}

		#region Ping Operations
		/// <summary>預設的逾時時間設定</summary>
		private static readonly int DEFAULT_TIMEOUT = 1000;

		/// <summary>嘗試存取遠端電腦，並回傳結果</summary>
		/// <param name="ipOrHostName">遠端裝置 IP 或主機名稱。如 "192.168.0.1"、"www.google.com.tw"</param>
		/// <returns>嘗試存取結果</returns>
		public static PingResult Ping(string ipOrHostName) {
			Ping ping = new Ping();
			PingReply reply = ping.Send(ipOrHostName);
			return new PingResult(reply, ipOrHostName);
		}

		/// <summary>嘗試存取遠端電腦，並回傳結果</summary>
		/// <param name="ipOrHostName">遠端裝置 IP 或主機名稱。如 "192.168.0.1"、"www.google.com.tw"</param>
		/// <param name="pingText">嘗試存取時欲送至遠方主機的字串</param>
		/// <returns>嘗試存取結果</returns>
		public static PingResult Ping(string ipOrHostName, string pingText) {
			Ping ping = new Ping();
			byte[] buf = Encoding.UTF8.GetBytes(pingText);
			PingReply reply = ping.Send(ipOrHostName, DEFAULT_TIMEOUT, buf);
			return new PingResult(reply, ipOrHostName);
		}

		/// <summary>嘗試存取遠端電腦，並回傳結果</summary>
		/// <param name="ipOrHostName">遠端裝置 IP 或主機名稱。如 "192.168.0.1"、"www.google.com.tw"</param>
		/// <param name="timeout">嘗試存取的逾時時間</param>
		/// <returns>嘗試存取結果</returns>
		public static PingResult Ping(string ipOrHostName, int timeout) {
			Ping ping = new Ping();
			PingReply reply = ping.Send(ipOrHostName, timeout);
			return new PingResult(reply, ipOrHostName, timeout);
		}

		/// <summary>嘗試存取遠端電腦，並回傳結果</summary>
		/// <param name="ipOrHostName">遠端裝置 IP 或主機名稱。如 "192.168.0.1"、"www.google.com.tw"</param>
		/// <param name="timeout">嘗試存取的逾時時間</param>
		/// <param name="pingText">嘗試存取時欲送至遠方主機的字串</param>
		/// <returns>嘗試存取結果</returns>
		public static PingResult Ping(string ipOrHostName, int timeout, string pingText) {
			Ping ping = new Ping();
			byte[] buf = Encoding.UTF8.GetBytes(pingText);
			PingReply reply = ping.Send(ipOrHostName, timeout, buf);
			return new PingResult(reply, ipOrHostName, timeout);
		}

		/// <summary>嘗試存取遠端電腦，並回傳結果</summary>
		/// <param name="ip">遠端裝置 IP 位址。如 "192.168.0.1"</param>
		/// <returns>嘗試存取結果</returns>
		public static PingResult Ping(IPAddress ip) {
			Ping ping = new Ping();
			PingReply reply = ping.Send(ip);
			return new PingResult(reply, ip.ToString());
		}

		/// <summary>嘗試存取遠端電腦，並回傳結果</summary>
		/// <param name="ip">遠端裝置 IP 位址。如 "192.168.0.1"</param>
		/// <param name="pingText">嘗試存取時欲送至遠方主機的字串</param>
		/// <returns>嘗試存取結果</returns>
		public static PingResult Ping(IPAddress ip, string pingText) {
			Ping ping = new Ping();
			byte[] buf = Encoding.UTF8.GetBytes(pingText);
			PingReply reply = ping.Send(ip, DEFAULT_TIMEOUT, buf);
			return new PingResult(reply, ip.ToString());
		}

		/// <summary>嘗試存取遠端電腦，並回傳結果</summary>
		/// <param name="ip">遠端裝置 IP 位址。如 "192.168.0.1"</param>
		/// <param name="timeout">嘗試存取的逾時時間</param>
		/// <returns>嘗試存取結果</returns>
		public static PingResult Ping(IPAddress ip, int timeout) {
			Ping ping = new Ping();
			PingReply reply = ping.Send(ip, timeout);
			return new PingResult(reply, ip.ToString(), timeout);
		}

		/// <summary>嘗試存取遠端電腦，並回傳結果</summary>
		/// <param name="ip">遠端裝置 IP 位址。如 "192.168.0.1"</param>
		/// <param name="timeout">嘗試存取的逾時時間</param>
		/// <param name="pingText">嘗試存取時欲送至遠方主機的字串</param>
		/// <returns>嘗試存取結果</returns>
		public static PingResult Ping(IPAddress ip, int timeout, string pingText) {
			Ping ping = new Ping();
			byte[] buf = Encoding.UTF8.GetBytes(pingText);
			PingReply reply = ping.Send(ip, timeout, buf);
			return new PingResult(reply, ip.ToString(), timeout);
		}
		#endregion

		#region FTP Operations

		/// <summary>透過 FTP 下載檔案至本機電腦</summary>
		/// <param name="addr">目標 FTP 檔案，含完整檔名。 如 @"ftp://ftp.server/File/Data.txt"</param>
		/// <param name="localFile">本機存檔路徑，含完整檔名。 如 @"D:\Data.txt"</param>
		/// <param name="account">FTP 登入的帳號</param>
		/// <param name="password">登入密碼</param>
		public static void DownloadFile(string addr, string localFile, string account, string password) {
			/*-- 連線至 FTP 站台 --*/
			FtpWebRequest ftpSite = WebRequest.Create(addr) as FtpWebRequest;

			/*-- 設定帳號密碼 --*/
			ftpSite.Credentials = new NetworkCredential(account, password);

			/*-- 設定模式為下載檔案 --*/
			ftpSite.Method = WebRequestMethods.Ftp.DownloadFile;

			/*-- 使用已設定的 ftpSite 取得當前 FTP 的回應 --*/
			FtpWebResponse ftpRsp = ftpSite.GetResponse() as FtpWebResponse;

			/*-- 如果 FTP 表示已開啟檔案，則可以開始進行下載 --*/
			if (ftpRsp.StatusCode == FtpStatusCode.OpeningData) {

				/*-- 如果是直接塞進 byte[] 有可能因為檔案過大而記憶體爆炸，故這邊一小搓一小搓的抓，抓完就寫檔案 (似乎未來可以做傳續?) --*/
				int length = 40960, byte2Read = 0;
				byte[] buffer = new byte[length];
				using (FileStream fs = new FileStream(localFile, FileMode.Create)) {
					using (Stream stream = ftpRsp.GetResponseStream()) {
						byte2Read = stream.Read(buffer, 0, length);
						while (byte2Read > 0) {
							fs.Write(buffer, 0, byte2Read);
							byte2Read = stream.Read(buffer, 0, length);
						}
					}
				}
			}

			Console.WriteLine("Download Complete, status {0}", ftpRsp.StatusDescription);

			ftpRsp.Close();
		}

		/// <summary>透過 FTP 下載檔案至本機電腦</summary>
		/// <param name="addr">目標 FTP 檔案，含完整檔名。 如 @"ftp://ftp.server/File/Data.txt"</param>
		/// <param name="localFile">本機存檔路徑，含完整檔名。 如 @"D:\Data.txt"</param>
		/// <param name="account">FTP 登入的帳號</param>
		/// <param name="password">登入密碼</param>
		/// <param name="handler">接收更新進度之委派</param>
		public static void DownloadFile(string addr, string localFile, string account, string password, EventHandler<FtpProgressChangedEventArgs> handler) {
			/*-- 連線至 FTP 站台 --*/
			FtpWebRequest ftpSite = WebRequest.Create(addr) as FtpWebRequest;

			/*-- 設定帳號密碼 --*/
			ftpSite.Credentials = new NetworkCredential(account, password);

			/*-- 設定模式為讀取檔案資訊 --*/
			ftpSite.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

			/*-- 使用已設定的 ftpSite 取得當前 FTP 的回應 --*/
			FtpWebResponse ftpRsp = ftpSite.GetResponse() as FtpWebResponse;

			/*-- 如果 FTP 表示現在可以進行存取，那就開抓吧 --*/
			List<FtpObject> objList = new List<FtpObject>();
			if (ftpRsp.StatusCode == FtpStatusCode.OpeningData) {
				using (StreamReader rspStm = new StreamReader(ftpRsp.GetResponseStream())) {
					/*-- 一口氣讀到尾巴 --*/
					string content = rspStm.ReadToEnd();
					List<string> split = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
					objList = split.ConvertAll(val => new FtpObject(val));
				}
			}

			/*-- 取得檔案大小 --*/
			long contentSize = -1;
			if (objList.Count == 1) contentSize = objList[0].FileSize;
			else contentSize = objList.Find(obj => addr.Contains(obj.FileName)).FileSize;

			/*-- 連線至 FTP 站台 --*/
			ftpSite = WebRequest.Create(addr) as FtpWebRequest;

			/*-- 設定帳號密碼 --*/
			ftpSite.Credentials = new NetworkCredential(account, password);

			/*-- 設定模式為下載檔案 --*/
			ftpSite.Method = WebRequestMethods.Ftp.DownloadFile;

			/*-- 使用 ASCII 下載檔案 --*/
			ftpSite.UseBinary = false;

			/*-- 使用已設定的 ftpSite 取得當前 FTP 的回應 --*/
			ftpRsp = ftpSite.GetResponse() as FtpWebResponse;

			/*-- 如果 FTP 表示已開啟檔案，則可以開始進行下載 --*/
			if (ftpRsp.StatusCode == FtpStatusCode.OpeningData) {

				/*-- 建立可發佈之事件參數 --*/
				FtpProgressChangedEventArgs e = new FtpProgressChangedEventArgs(Path.GetFileName(addr), contentSize);

				/*-- 如果是直接塞進 byte[] 有可能因為檔案過大而記憶體爆炸，故這邊一小搓一小搓的抓，抓完就寫檔案 (似乎未來可以做傳續?) --*/
				int length = 40960, byte2Read = 0;
				byte[] buffer = new byte[length];
				using (FileStream fs = new FileStream(localFile, FileMode.Create)) {
					using (Stream stream = ftpRsp.GetResponseStream()) {
						byte2Read = stream.Read(buffer, 0, length);
						while (byte2Read > 0) {
							fs.Write(buffer, 0, byte2Read);
							e.BytesProcessed += byte2Read;
							byte2Read = stream.Read(buffer, 0, length);
							handler(ftpSite, e);
						}
					}
				}
			}

			Console.WriteLine("Download Complete, status {0}", ftpRsp.StatusDescription);

			ftpRsp.Close();
		}

		/// <summary>透過 FTP 將本機電腦檔案上傳至遠端站台</summary>
		/// <param name="localFile">欲上傳的本機檔案，含完整檔名。 如 @"D:\Data.txt"</param>
		/// <param name="addr">目標 FTP 路徑，含完整檔名。 如 @"ftp://ftp.server/File/Data.txt"</param>
		/// <param name="account">FTP 登入的帳號</param>
		/// <param name="password">登入密碼</param>
		public static void UploadFile(string localFile, string addr, string account, string password) {
			/*-- 連線至 FTP 站台 --*/
			FtpWebRequest ftpSite = WebRequest.Create(addr) as FtpWebRequest;

			/*-- 設定模式為上傳檔案 --*/
			ftpSite.Method = WebRequestMethods.Ftp.UploadFile;

			/*-- 設定帳號密碼 --*/
			ftpSite.Credentials = new NetworkCredential(account, password);

			/*-- 使用已設定的 ftpSite 取得當前 FTP 的回應 --*/
			FtpWebResponse ftpRsp = ftpSite.GetResponse() as FtpWebResponse;

			/*-- 如果 FTP 表示檔案沒有在使用，開始上傳檔案 --*/
			if (ftpRsp.StatusCode == FtpStatusCode.ClosingData) {
				/*-- 如果是直接塞進 byte[] 有可能因為檔案過大而記憶體爆炸，故這邊一小搓一小搓的讀，讀完就寫入 FTP (似乎未來可以做傳續?) --*/
				int length = 40960, byte2Write = 0;
				byte[] buffer = new byte[length];
				using (FileStream filStm = new FileStream(localFile, FileMode.Open)) {
					using (Stream reqStm = ftpSite.GetRequestStream()) {
						byte2Write = filStm.Read(buffer, 0, length);
						while (byte2Write > 0) {
							reqStm.Write(buffer, 0, byte2Write);
							byte2Write = filStm.Read(buffer, 0, length);
						}
					}
				}
			}

			Console.WriteLine("Upload File Complete, status {0}", ftpRsp.StatusDescription);

			ftpRsp.Close();
		}

		/// <summary>透過 FTP 將本機電腦檔案上傳至遠端站台</summary>
		/// <param name="localFile">欲上傳的本機檔案，含完整檔名。 如 @"D:\Data.txt"</param>
		/// <param name="addr">目標 FTP 路徑，含完整檔名。 如 @"ftp://ftp.server/File/Data.txt"</param>
		/// <param name="account">FTP 登入的帳號</param>
		/// <param name="password">登入密碼</param>
		/// <param name="handler">接收更新進度之委派</param>
		public static void UploadFile(string localFile, string addr, string account, string password, EventHandler<FtpProgressChangedEventArgs> handler) {
			/*-- 連線至 FTP 站台 --*/
			FtpWebRequest ftpSite = WebRequest.Create(addr) as FtpWebRequest;

			/*-- 設定模式為上傳檔案 --*/
			ftpSite.Method = WebRequestMethods.Ftp.UploadFile;

			/*-- 設定帳號密碼 --*/
			ftpSite.Credentials = new NetworkCredential(account, password);

			/*-- 使用已設定的 ftpSite 取得當前 FTP 的回應 --*/
			FtpWebResponse ftpRsp = ftpSite.GetResponse() as FtpWebResponse;

			/*-- 如果 FTP 表示檔案沒有在使用，開始上傳檔案 --*/
			if (ftpRsp.StatusCode == FtpStatusCode.ClosingData) {
				/*-- 如果是直接塞進 byte[] 有可能因為檔案過大而記憶體爆炸，故這邊一小搓一小搓的讀，讀完就寫入 FTP (似乎未來可以做傳續?) --*/
				int length = 40960, byte2Write = 0;
				byte[] buffer = new byte[length];
				using (FileStream filStm = new FileStream(localFile, FileMode.Open)) {

					/*-- 建立可發佈之事件參數 --*/
					FtpProgressChangedEventArgs e = new FtpProgressChangedEventArgs(Path.GetFileName(localFile), filStm.Length);

					using (Stream reqStm = ftpSite.GetRequestStream()) {
						byte2Write = filStm.Read(buffer, 0, length);
						while (byte2Write > 0) {
							reqStm.Write(buffer, 0, byte2Write);
							e.BytesProcessed += byte2Write;
							byte2Write = filStm.Read(buffer, 0, length);
							handler(ftpSite, e);
						}
					}
				}
			}

			Console.WriteLine("Upload File Complete, status {0}", ftpRsp.StatusDescription);

			ftpRsp.Close();
		}

		/// <summary>透過 FTP 刪除遠端站台之檔案</summary>
		/// <param name="addr">欲刪除的遠端站台檔案路徑。 如 @"ftp://ftp.server/File/Data.txt"</param>
		/// <param name="account">FTP 登入的帳號</param>
		/// <param name="password">登入密碼</param>
		public static void DeleteFile(string addr, string account, string password) {
			/*-- 連線至 FTP 站台 --*/
			FtpWebRequest ftpSite = WebRequest.Create(addr) as FtpWebRequest;

			/*-- 設定模式為刪除檔案 --*/
			ftpSite.Method = WebRequestMethods.Ftp.DeleteFile;

			/*-- 設定帳號密碼 --*/
			ftpSite.Credentials = new NetworkCredential(account, password);

			/*-- 使用已設定的 ftpSite 進行操作，並等待 FTP 回應 --*/
			FtpWebResponse ftpRsp = ftpSite.GetResponse() as FtpWebResponse;

			Console.WriteLine("Delete File Complete, status {0}", ftpRsp.StatusDescription);

			ftpRsp.Close();
		}

		/// <summary>透過 FTP 於遠端站台新增資料夾</summary>
		/// <param name="addr">欲新增資料夾的遠端站台路徑。 如欲新增 "Log_2020" 資料夾，請帶入 @"ftp://ftp.server/File/Log_2020"</param>
		/// <param name="account">FTP 登入的帳號</param>
		/// <param name="password">登入密碼</param>
		public static void MakeDirectory(string addr, string account, string password) {
			/*-- 連線至 FTP 站台 --*/
			FtpWebRequest ftpSite = WebRequest.Create(addr) as FtpWebRequest;

			/*-- 設定模式為建立目錄 --*/
			ftpSite.Method = WebRequestMethods.Ftp.MakeDirectory;

			/*-- 設定帳號密碼 --*/
			ftpSite.Credentials = new NetworkCredential(account, password);

			/*-- 使用已設定的 ftpSite 進行操作，並等待 FTP 回應 --*/
			FtpWebResponse ftpRsp = ftpSite.GetResponse() as FtpWebResponse;

			Console.WriteLine("Make Directory Complete, status {0}", ftpRsp.StatusDescription);

			ftpRsp.Close();
		}

		/// <summary>透過 FTP 於遠端站台刪除資料夾</summary>
		/// <param name="addr">欲刪除資料夾的遠端站台路徑。 如欲刪除 "Log_2020" 資料夾，請帶入 @"ftp://ftp.server/File/Log_2020"</param>
		/// <param name="account">FTP 登入的帳號</param>
		/// <param name="password">登入密碼</param>
		public static void DeleteDirectory(string addr, string account, string password) {
			/*-- 連線至 FTP 站台 --*/
			FtpWebRequest ftpSite = WebRequest.Create(addr) as FtpWebRequest;

			/*-- 設定模式為刪除目錄 --*/
			ftpSite.Method = WebRequestMethods.Ftp.RemoveDirectory;

			/*-- 設定帳號密碼 --*/
			ftpSite.Credentials = new NetworkCredential(account, password);

			/*-- 使用已設定的 ftpSite 進行操作，並等待 FTP 回應 --*/
			FtpWebResponse ftpRsp = ftpSite.GetResponse() as FtpWebResponse;

			Console.WriteLine("Remove Directory Complete, status {0}", ftpRsp.StatusDescription);

			ftpRsp.Close();
		}

		/// <summary>列出 FTP 站台特定資料夾或檔案的資訊</summary>
		/// <param name="addr">欲獲取資訊的檔案或資料夾，如是資料夾將回傳資料夾內各物件的訊息。 如 @"ftp://ftp.server/File/Log_2020"</param>
		/// <param name="list">接收解析完成的物件資料集合</param>
		/// <param name="account">FTP 登入的帳號</param>
		/// <param name="password">登入密碼</param>
		public static void ListDirectory(string addr, out List<FtpObject> list, string account, string password) {
			/*-- 連線至 FTP 站台 --*/
			FtpWebRequest request = WebRequest.Create(addr) as FtpWebRequest;

			/*-- 設定模式為列出檔案 --*/
			request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

			/*-- 設定帳號密碼 --*/
			request.Credentials = new NetworkCredential(account, password);

			/*-- 使用已設定的 ftpSite 取得當前 FTP 的回應 --*/
			FtpWebResponse ftpRsp = request.GetResponse() as FtpWebResponse;

			/*-- 如果 FTP 表示現在可以進行存取，那就開抓吧 --*/
			List<FtpObject> objList = new List<FtpObject>();
			if (ftpRsp.StatusCode == FtpStatusCode.OpeningData) {
				using (StreamReader rspStm = new StreamReader(ftpRsp.GetResponseStream())) {
					/*-- 一口氣讀到尾巴 --*/
					string content = rspStm.ReadToEnd();
					List<string> split = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
					objList = split.ConvertAll(val => new FtpObject(val));
				}
			}

			Console.WriteLine("Directory List Complete, status {0}", ftpRsp.StatusDescription);

			ftpRsp.Close();

			list = objList;
		}
		#endregion
	}
	#endregion

}
