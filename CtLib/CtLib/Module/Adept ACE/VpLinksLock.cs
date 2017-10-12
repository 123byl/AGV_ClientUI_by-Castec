using System;
using System.Collections.Generic;
using System.Linq;

using Ace.Adept.Server.Controls;
using Ace.Adept.Server.Desktop.Connection;

namespace CtLib.Module.Adept {

	/// <summary>儲存 <see cref="IVpLinkedObject"/> 之 V+ 連結器集合，並提供 lock 服務</summary>
	[Serializable]
	internal sealed class VpObjects : IDisposable {

		#region Fields
		/// <summary>用於 lock 之物件</summary>
		private object mLock = int.MaxValue;
		/// <summary>儲存 VpLink 之集合</summary>
		private Dictionary<int, IVpLinkedObject> mVpLinks;
		#endregion

		#region Properteis
		public Dictionary<int, IVpLinkedObject> VpLinkers { get { return mVpLinks; } }
		public int ObjectCount { get { return mVpLinks.Count; } }
		#endregion

		#region Constructors
		/// <summary>建構 V+ 連結器儲存封包</summary>
		/// <param name="links">V+ 連結器集合</param>
		public VpObjects(IEnumerable<IVpLinkedObject> links) {
			int idx = 0;
			mVpLinks = links.ToDictionary(obj => idx++, obj => obj);
		}
		#endregion

		#region IDisposable Implements
		public void Dispose() {
			if (mVpLinks != null) {
				mVpLinks.Clear();
				mVpLinks = null;
			}
		}
		#endregion

		#region Public Operations
		/// <summary>取得特定編號的 <see cref="IVpLinkedObject"/>，但不進行 lock 動作！</summary>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		/// <returns><see cref="IVpLinkedObject"/></returns>
		public IVpLinkedObject Obj(int vpNum = 0) {
			return mVpLinks[vpNum];
		}

		/// <summary>進行 <see cref="IVpLinkedObject"/> 相關動作，並禁止其他人存取</summary>
		/// <param name="action">欲執行的工作內容</param>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		public void Obj(Action<IVpLinkedObject> action, int vpNum = 0) {
			IVpLinkedObject vpLink = mVpLinks[vpNum];
			lock (mLock) {
				action(vpLink);
			}
		}

		/// <summary>進行 <see cref="IVpLinkedObject"/> 相關動作並取得相關物件或數值，同時禁止其他人存取</summary>
		/// <typeparam name="TRet">工作內容所回傳的型態</typeparam>
		/// <param name="action">欲執行的工作內容</param>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		/// <returns>工作內容所回傳的物件</returns>
		public TRet Obj<TRet>(Func<IVpLinkedObject, TRet> action, int vpNum = 0) {
			TRet obj = default(TRet);
			IVpLinkedObject vpLink = mVpLinks[vpNum];
			lock (mLock) {
				obj = action(vpLink);
			}
			return obj;
		}

		/// <summary>取得特定編號的 <see cref="IVpLink"/>，但不進行 lock 動作！</summary>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		/// <returns><see cref="IVpLink"/></returns>
		public IVpLink Link(int vpNum = 0) {
			return mVpLinks[vpNum].Link;
		}

		/// <summary>進行 <see cref="IVpLink"/> 相關動作，並禁止其他人存取</summary>
		/// <param name="action">欲執行的工作內容</param>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		public void Link(Action<IVpLink> action, int vpNum = 0) {
			IVpLink vpLink = mVpLinks[vpNum].Link;
			lock (mLock) {
				action(vpLink);
			}
		}

		/// <summary>進行 <see cref="IVpLink"/> 相關動作並取得相關物件或數值，同時禁止其他人存取</summary>
		/// <typeparam name="TRet">工作內容所回傳的型態</typeparam>
		/// <param name="action">欲執行的工作內容</param>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		/// <returns>工作內容所回傳的物件</returns>
		public TRet Link<TRet>(Func<IVpLink, TRet> action, int vpNum = 0) {
			IVpLink vpLink = mVpLinks[vpNum].Link;
			lock (mLock) {
				return action(vpLink);
			}
		}

		/// <summary>取得當前 Task 狀態集合，存取期間禁止其他人存取</summary>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		/// <returns>Task 狀態集合</returns>
		public VPStatus[] Status(int vpNum = 0) {
			lock (mLock) {
				return mVpLinks[vpNum].Link.Status();
			}
		}

		/// <summary>進行 <see cref="IVPlusMemory"/> 相關動作，並禁止其他人存取</summary>
		/// <param name="action">欲執行的工作內容</param>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		public void Mem(Action<IVPlusMemory> action, int vpNum = 0) {
			IVPlusMemory memory = mVpLinks[vpNum].Memory;
			lock (mLock) {
				action(memory);
			}
		}

		/// <summary>進行 <see cref="IVPlusMemory"/> 相關動作並取得相關物件或數值，同時禁止其他人存取</summary>
		/// <typeparam name="TRet">工作內容所回傳的型態</typeparam>
		/// <param name="action">欲執行的工作內容</param>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		/// <returns>工作內容所回傳的物件</returns>
		public TRet Mem<TRet>(Func<IVPlusMemory, TRet> action, int vpNum = 0) {
			IVPlusMemory memory = mVpLinks[vpNum].Memory;
			lock (mLock) {
				return action(memory);
			}
		}

		/// <summary>進行 <see cref="IVPlusGlobalVariableCollection"/> 相關動作，並禁止其他人存取</summary>
		/// <param name="action">欲執行的工作內容</param>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		public void Var(Action<IVPlusGlobalVariableCollection> action, int vpNum = 0) {
			IVPlusGlobalVariableCollection varColl = mVpLinks[vpNum].Memory.Variables;
			lock (mLock) {
				action(varColl);
			}
		}

		/// <summary>進行 <see cref="IVPlusGlobalVariableCollection"/> 相關動作並取得相關物件或數值，同時禁止其他人存取</summary>
		/// <typeparam name="TRet">工作內容所回傳的型態</typeparam>
		/// <param name="action">欲執行的工作內容</param>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		/// <returns>工作內容所回傳的物件</returns>
		public TRet Var<TRet>(Func<IVPlusGlobalVariableCollection, TRet> action, int vpNum = 0) {
			IVPlusGlobalVariableCollection varColl = mVpLinks[vpNum].Memory.Variables;
			lock (mLock) {
				return action(varColl);
			}
		}

		/// <summary>取得特定 Task 狀態碼，存取時禁止其他人存取</summary>
		/// <param name="tskNum">欲取得的 Task 編號</param>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		/// <returns>Task 狀態碼</returns>
		public int TaskStt(int tskNum, int vpNum = 0) {
			lock (mLock) {
				return mVpLinks[0].Link.Task(tskNum, 1);
			}
		}

		#endregion
	}
}
