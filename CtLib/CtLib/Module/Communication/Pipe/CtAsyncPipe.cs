using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;

using CtLib.Library;
using CtLib.Module.Utility;

namespace CtLib.Module.Net {

	/// <summary>
	/// 公開管道的 <see cref="Stream"/> 物件，使用具名管道實現
	/// <para>提供一對一的串流管道，可用於訊息傳遞或資料傳輸</para>
	/// <para>如需一對多或廣播，請參考 <see cref="Net.CtSocket"/> 與 UDP 相關類別</para>
	/// </summary>
	[Serializable]
	public class CtAsyncPipe {

		#region Version

		/// <summary>CtAsyncPipe 版本相關訊息</summary>
		/// <remarks><code language="C#">
		/// 0.0.0	Ahern	[2016/07/13]
		///     + 建構基礎模組，使用具名管道實作
		///     
		/// 1.0.0	Ahern	[2016/07/14]
		///     + 完成所有常用方法
		///     + 初步測試完成
		///     
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(1, 0, 0, "2016/07/14", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Definitions
		/// <summary>Client 嘗試連線至 Server 的逾時時間</summary>
		private static readonly int CONNECT_TIMEOUT = 3000;
		#endregion

		#region Declaration - Support Class

		/// <summary>用於非同步資料傳遞</summary>
		[Serializable]
		private class PipeObject {
			/// <summary>資料陣列長度</summary>
			public static int BufferSize { get; } = 1024;
			/// <summary><see cref="PipeStream"/> 物件</summary>
			public PipeStream WorkPipe { get; private set; }
			/// <summary>從 <see cref="PipeStream"/> 讀取到資料</summary>
			public byte[] Buffer { get; set; }
			/// <summary>暫存的 Buffer 資料</summary>
			/// <remarks>用於 Buffer 擷取有資料部分，另可用來儲存未完整收到訊息前所收到的資料</remarks>
			public List<byte> RemainBuffer { get; private set; }

			/// <summary>建構資料傳遞包裝</summary>
			/// <param name="stream"><see cref="PipeStream"/> 物件</param>
			public PipeObject(PipeStream stream) {
				WorkPipe = stream;
				Buffer = new byte[BufferSize];
				RemainBuffer = new List<byte>();
			}

			/// <summary>將 Buffer 轉移至 RemainBuffer 後端</summary>
			/// <param name="length">欲轉移的筆數</param>
			public void CaptureBuffer(int length) {
				RemainBuffer.AddRange(Buffer.Take(length));
			}
		}
		#endregion

		#region Declaration - Events
		/// <summary>提供 <see cref="CtAsyncPipe"/> 相關事件，如連線狀態、資料傳送接收等</summary>
		public event EventHandler<PipeEventArgs> OnPipeEvents;

		/// <summary>發布事件</summary>
		/// <param name="time">事件發生時間</param>
		/// <param name="ev">事件</param>
		/// <param name="value">附加數值</param>
		protected virtual void RaiseEvent(DateTime time, PipeEvents ev, object value) {
			if (OnPipeEvents != null) {
				OnPipeEvents.BeginInvoke(this, new PipeEventArgs(time, ev, value), null, null);
			}
		}
		#endregion

		#region Declaration - Fields
		/// <summary>
		/// 儲存已連線的 <see cref="PipeStream"/>
		/// <para>可能為 <see cref="NamedPipeServerStream"/> 或 <seealso cref="NamedPipeClientStream"/></para>
		/// </summary>
		private PipeStream mPipe;
		/// <summary>管道模式，Client 或 Server</summary>
		private PipeModes mMode = PipeModes.Client;
		/// <summary>[Client/Server] 管道名稱</summary>
		private string mPipeName = string.Empty;
		/// <summary>[Client] 目標主機名稱</summary>
		private string mSrvName = string.Empty;
		/// <summary>[Server] 指出當前是否有 Client 連線</summary>
		private bool mSrvCnt = false;
		/// <summary>[Server] 指出目前是否正在監聽，於 Disconnect 時設為 False 防止 Callback 自動連線</summary>
		private bool mSrvListen = false;
		/// <summary>相對應結尾符號之byte</summary>
		/// <remarks>搭配 EndOfLineSymbol == EndChar.Custom</remarks>
		private byte[] mEOLByte;
		#endregion

		#region Declaration - Properties
		/// <summary>取得當前管道雙方是否已連線</summary>
		public bool IsConnected { get { return mPipe?.IsConnected ?? false; } }
		/// <summary>取得事件回傳的資料格式</summary>
		public TransDataFormats DataFormat { get; private set; }
		/// <summary>取得或設定編碼。用於傳送、接收時 byte 和 string 之間之轉換</summary>
		public CodePages CodePage { get; set; }
		/// <summary>取得或設定讀寫資料時之結尾符號</summary>
		public EndChar EndOfLineSymbol { get; set; }
		/// <summary>取得或設定自訂義之結尾符號</summary>
		public string CustomEndOfLine {
			get { return Encoding.GetEncoding((int)CodePage).GetString(mEOLByte); }
			set { mEOLByte = Encoding.GetEncoding((int)CodePage).GetBytes(value); }
		}
		/// <summary>取得管道模式，請於建構時指定模式</summary>
		public PipeModes PipeMode { get { return mMode; } }
		/// <summary>取得是否訂閱資料送出事件，如有訂閱將於傳送資料時觸發此事件已告知傳送的資料。請於建構時指定</summary>
		public bool SubscribeSendEvent { get; private set; }
		#endregion

		#region Function - Constructors
		/// <summary>建構具名的 <seealso cref="PipeStream"/> 物件</summary>
		/// <param name="fmt">資料傳輸格式</param>
		/// <param name="txEvent">是否訂閱資料傳送事件，如有訂閱將於傳送資料時觸發此事件已告知傳送的資料</param>
		public CtAsyncPipe(TransDataFormats fmt, bool txEvent = false) {
			DataFormat = fmt;
			SubscribeSendEvent = txEvent;
		}
		#endregion

		#region Function - Private Methods
		/// <summary>檢查 Byte 陣列中是否含有目前設定的結尾符號</summary>
		/// <param name="data">欲檢查的資料</param>
		/// <returns>(<see langword="true"/>)含有結尾符號  (<see langword="false"/>)不含結尾符號</returns>
		private bool IsContainEndOfLine(List<byte> data) {
			bool result = false;

			if (EndOfLineSymbol == EndChar.None) result = true;
			else if (EndOfLineSymbol == EndChar.CrLf) result = data.Contains(0x0D) & data.Contains(0x0A);
			else if (EndOfLineSymbol == EndChar.Cr) result = data.Contains(0x0D);
			else if (EndOfLineSymbol == EndChar.Lf) result = data.Contains(0x0A);
			else if (EndOfLineSymbol == EndChar.Custom) {
				result = true;
				foreach (byte item in mEOLByte) {
					if (!data.Contains(item)) {
						result = false;
						break;
					}
				}
			}

			return result;
		}
		#endregion

		#region Function - Connections
		/// <summary>[Client] 嘗試連線至主機</summary>
		/// <param name="pipeName">管道名稱，相對應於 ServerListen 之引數</param>
		/// <param name="srvName">主機名稱，輸入 "." 表示本機電腦</param>
		public void ClientConnect(string pipeName, string srvName = ".") {
			/*-- 如果是重新連線，確保管道已消除 --*/
			if (mPipe != null && mPipe.IsConnected) mPipe.Dispose();

			/*-- 儲存相關參數至全域 --*/
			mMode = PipeModes.Client;
			mPipeName = pipeName;
			mSrvName = srvName;

			/*-- 建構 PipeStream，採用非同步模式 --*/
			mPipe = new NamedPipeClientStream(srvName, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

			/*-- 嘗試連線至 Server --*/
			(mPipe as NamedPipeClientStream).Connect(CONNECT_TIMEOUT);
			DateTime time = DateTime.Now;

			/*-- 開始接收資料 --*/
			PipeObject pipeObj = new PipeObject(mPipe);
			mPipe.BeginRead(pipeObj.Buffer, 0, PipeObject.BufferSize, ReceiveCallback, pipeObj);

			/*-- 發布連線事件 --*/
			RaiseEvent(time, PipeEvents.Connection, true);
		}

		/// <summary>[Server] 開始等候用戶端連線</summary>
		/// <param name="pipeName">欲開啟的管道名稱</param>
		public void ServerListen(string pipeName) {
			/*-- 如果是重新連線，確保管道已消除 --*/
			if (mPipe != null && mPipe.IsConnected) mPipe.Dispose();

			/*-- 儲存相關參數至全域 --*/
			mSrvCnt = false;
			mSrvListen = true;
			mMode = PipeModes.Server;
			mPipeName = pipeName;

			/*-- 建構 PipeStream，採用非同步模式 --*/
			mPipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

			/*-- 使用非同步方式，開始等候 Client 連線 --*/
			(mPipe as NamedPipeServerStream).BeginWaitForConnection(Server_AcceptCallback, mPipe);

			/*-- 發布等候事件 --*/
			RaiseEvent(DateTime.Now, PipeEvents.WaitForConnection, true);
		}

		/// <summary>[All] 中斷管道連線</summary>
		public void Disconnect() {
			DateTime time = DateTime.Now;
			try {
				mSrvListen = false;
				if (mMode == PipeModes.Server && mSrvCnt)
					(mPipe as NamedPipeServerStream).Disconnect();

				/*-- 解除 PipeStream --*/
				mPipe.Close();
				mPipe.Dispose();
			} catch (Exception) {
				/*-- 這邊就不處理囉 --*/
			} finally {
				/*-- 重設 & 發布連線事件 --*/
				mPipe = null;
				if (mMode == PipeModes.Client || mSrvCnt) RaiseEvent(time, PipeEvents.Connection, false);
				if (mMode == PipeModes.Server) RaiseEvent(time, PipeEvents.WaitForConnection, false);
			}
		}
		#endregion

		#region Function - Asynchronous Callback
		/// <summary>[Callback][Server] 用戶端已連線</summary>
		/// <param name="asyncResult">非同步傳遞資料</param>
		protected virtual void Server_AcceptCallback(IAsyncResult asyncResult) {
			/*-- 紀錄 & 取得相關資訊 --*/
			DateTime time = DateTime.Now;
			NamedPipeServerStream pipeSrv = asyncResult.AsyncState as NamedPipeServerStream;

			try {
				if (mSrvListen) {	//確保正在監聽中，避免斷線時會觸發存取已關閉通道之例外
					/*-- 表示結束非同步連線 --*/
					pipeSrv.EndWaitForConnection(asyncResult);

					/*-- 表示已連線 --*/
					mSrvCnt = true;

					/*-- 開始接收資料 --*/
					PipeObject pipeObj = new PipeObject(pipeSrv);
					pipeSrv.BeginRead(pipeObj.Buffer, 0, PipeObject.BufferSize, ReceiveCallback, pipeObj);

					/*-- 發布連線事件 --*/
					RaiseEvent(time, PipeEvents.Connection, true);
				}
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER3_WSK_COMERR, ex);
			}
		}

		/// <summary>[Callback][All] 從管道接收到資料</summary>
		/// <param name="asyncResult">非同步傳遞資料</param>
		protected virtual void ReceiveCallback(IAsyncResult asyncResult) {
			/*-- 紀錄 & 取得相關資訊 --*/
			DateTime time = DateTime.Now;
			PipeObject pipeObj = asyncResult.AsyncState as PipeObject;
			PipeStream pipeStrm = pipeObj.WorkPipe;

			bool isEnd = false; //紀錄此串命令是否已含有結尾符號
			int rxLength = 0;   //紀錄此次接收到的 byte 資料長度

			try {
				/*-- 取得此次共有多少筆資料，而資料內容已被塞入 PipeObject.Buffer 裡 --*/
				rxLength = pipeStrm.EndRead(asyncResult);
			} catch (ObjectDisposedException ex) {
				rxLength = 0;
				Console.WriteLine(ex.Message);
			}

			if (rxLength > 0) { /*-- 如果有收到資料 --*/

				/* 將取的的筆數塞入 RemainBuffer */
				pipeObj.CaptureBuffer(rxLength);

				/* 檢查是否已含有結尾符號 */
				isEnd = IsContainEndOfLine(pipeObj.RemainBuffer);

				if (isEnd) {    /* 如果是完整段落 */

					if (DataFormat == TransDataFormats.EnumerableOfByte) {
						RaiseEvent(time, PipeEvents.DataReceived, pipeObj.RemainBuffer.ToList());
					} else {
						string cntx = Encoding.GetEncoding((int)CodePage).GetString(pipeObj.RemainBuffer.ToArray());
						RaiseEvent(time, PipeEvents.DataReceived, cntx);
					}

					/*** 重複進行接收動作 ***/
					PipeObject newObj = new PipeObject(pipeStrm);
					pipeStrm.BeginRead(newObj.Buffer, 0, PipeObject.BufferSize, ReceiveCallback, newObj);

				} else {    /* 如果不是完整段落 */

					/*** 將當前的 PipeObject 塞回去，繼續收 ***/
					pipeStrm.BeginRead(pipeObj.Buffer, 0, PipeObject.BufferSize, ReceiveCallback, pipeObj);
				}
			} else if (mMode == PipeModes.Client) { /*-- 如果沒有資料，表示斷線 --*/

				/* Client >> 直接斷線吧！ */
				try {
					pipeStrm.Close();
					pipeStrm.Dispose();
				} catch (Exception) {
				} finally {
					RaiseEvent(time, PipeEvents.Connection, false);
					mPipe = null;
				}
			} else if (mMode == PipeModes.Server) { /*-- 如果沒有資料，表示斷線 --*/

				/* Server >> 重新等待連線 */
				mSrvCnt = false;
				RaiseEvent(time, PipeEvents.Connection, false);
				if (mSrvListen) {
					NamedPipeServerStream srvPipe = pipeStrm as NamedPipeServerStream;
					srvPipe.Disconnect();
					srvPipe.BeginWaitForConnection(Server_AcceptCallback, pipeStrm);
				}
			}
		}
		#endregion

		#region Function - Send Data
		/// <summary>
		/// 傳送資料至管道
		/// <para>此方法並不會自動添加結尾符號，請自行做好資料添加與確保</para>
		/// </summary>
		/// <param name="data">欲傳送的資料</param>
		public void Send(IEnumerable<byte> data) {
			if (mPipe != null && mPipe.IsConnected) {
				byte[] wrData = data.ToArray();
				mPipe.Write(wrData, 0, wrData.Length);
				if (SubscribeSendEvent) {
					RaiseEvent(DateTime.Now, PipeEvents.DataSend, data.ToList());
				}
			}
		}

		/// <summary>
		/// 使用非同步的方式，將資料傳送至管道
		/// <para>此方法並不會自動添加結尾符號，請自行做好資料添加與確保</para>
		/// </summary>
		/// <param name="data">欲傳送的資料</param>
		public void SendAsync(IEnumerable<byte> data) {
			if (mPipe != null && mPipe.IsConnected) {
				byte[] wrData = data.ToArray();
				mPipe.WriteAsync(wrData, 0, wrData.Length);
				if (SubscribeSendEvent) {
					RaiseEvent(DateTime.Now, PipeEvents.DataSend, data.ToList());
				}
			}
		}

		/// <summary>
		/// 傳送資料至管道
		/// <para>此方法並不會自動添加結尾符號，請自行做好資料添加與確保</para>
		/// </summary>
		/// <param name="data">欲傳送的資料</param>
		public void Send(string data) {
			if (mPipe != null && mPipe.IsConnected) {
				byte[] wrData = Encoding.GetEncoding((int)CodePage).GetBytes(data);
				mPipe.Write(wrData, 0, wrData.Length);
				if (SubscribeSendEvent) {
					RaiseEvent(DateTime.Now, PipeEvents.DataSend, data);
				}
			}
		}

		/// <summary>
		/// 使用非同步的方式，將資料傳送至管道
		/// <para>此方法並不會自動添加結尾符號，請自行做好資料添加與確保</para>
		/// </summary>
		/// <param name="data">欲傳送的資料</param>
		public void SendAsync(string data) {
			if (mPipe != null && mPipe.IsConnected) {
				byte[] wrData = Encoding.GetEncoding((int)CodePage).GetBytes(data);
				mPipe.WriteAsync(wrData, 0, wrData.Length);
				if (SubscribeSendEvent) {
					RaiseEvent(DateTime.Now, PipeEvents.DataSend, data);
				}
			}
		}
		#endregion
	}
}
