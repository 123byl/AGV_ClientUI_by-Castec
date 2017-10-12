using System;
using System.Collections.Generic;
using System.Linq;

using Ace.Adept.Server.Motion;

using CtLib.Library;

namespace CtLib.Module.Adept {

	/// <summary>
	/// Adept Robot 與 SmartController 之 I/O 控制
	/// <para>此部分提供多種讀取與寫入I/O方法</para>
	/// </summary>
	[Serializable]
	public sealed class CtAceIO {

		#region Declaration - Fields
		/// <summary>儲存 V+ 連結器，並提供 lock 服務</summary>
		private VpObjects mVpObj;
		/// <summary>Thread-Safe Lock Object</summary>
		private object mLockObj = int.MinValue;
		#endregion

		#region Function - Constructors
		/// <summary>建立 Adept I/O 相關控制</summary>
		/// <param name="links">V+ 連結器</param>
		internal CtAceIO(VpObjects links) {
			mVpObj = links;
		}
		#endregion

		#region Function - SmartController & Generic I/O

		/// <summary>
		/// 更改當前I/O狀態。
		/// 正整數表示開啟該I/O，負整數表關閉。如 2001 = 開啟2001; -2014 = 關閉2014
		/// </summary>
		/// <param name="io">
		/// 欲更改之I/O編號。
		/// 可帶入陣列，如 int[] IOs = new int[] {2001, -2002, -2003, 2004}
		/// 也可採用逗號分隔，如 SetIO(2001, -2002, -2003, 2004)
		/// </param>
		/// <example><code language="C#">
		/// CtAce mAce = new CtAce();
		/// 
		/// List&lt;int&gt; ioList = new List&lt;int&gt; { 43, -44, -97, 98 };
		/// mAce.IO.SetIO(ioList.ToArray());    //設定 43(ON) 44(OFF) 97(OFF) 98(ON)
		/// mAce.IO.SetIO(2001, -2002);         //設定 2001(ON) 2002(OFF)
		/// </code></example>
		public void SetIO(params int[] io) {
			mVpObj.Obj(obj => obj.SetDigitalIO(io));
		}

		/// <summary>
		/// 更改當前I/O狀態。以布林(Boolean)表示ON/OFF狀態
		/// </summary>
		/// <param name="io">欲更改之I/O編號</param>
		/// <param name="stt">(<see langword="true"/>)ON   (<see langword="false"/>)OFF</param>
		/// <example><code language="C#">
		/// CtAce mAce = new CtAce();
		/// 
		/// mAce.IO.SetIO(97, false);   //設定 IO 編號 97 為 OFF
		/// mAce.IO.SetIO(43, true);    //設定 IO 編號 43 為 ON
		/// </code></example>
		public void SetIO(int io, bool stt) {
			int ioStt = io * (stt ? 1 : -1);
			mVpObj.Obj(obj => obj.SetDigitalIO(ioStt));
		}

		/// <summary>
		/// 更改多個I/O狀態。以布林(Boolean)集合表示ON/OFF狀態
		/// </summary>
		/// <param name="ioList">欲更改之I/O編號集合</param>
		/// <param name="ioStt">相對應 ioList 順序之狀態  (<see langword="true"/>)ON   (<see langword="false"/>)OFF</param>
		/// <example><code language="C#">
		/// CtAce mAce = new CtAce();
		/// 
		/// List&lt;int&gt; ioList = new List&lt;int&gt; { 2001, 2002, 2003, 2004, 2005 };
		/// List&lt;bool&gt; ioStt = new List&lt;bool&gt; { false, true, true, false, false };
		/// mAce.IO.SetIO(ioList, ioStt);   //設定 2001(OFF) 2002(ON) 2003(ON) 2004(OFF) 2005(OFF)
		/// </code></example>
		public void SetIO(List<int> ioList, List<bool> ioStt) {
			if (ioList.Count == ioStt.Count) {
				int[] sig = ioList.Zip(ioStt, (io, stt) => io * (stt ? 1 : -1)).ToArray();
				mVpObj.Obj(obj => obj.SetDigitalIO(sig));
			}
		}

		/// <summary>
		/// 更改多個I/O狀態。以 <see cref="Dictionary&lt;TKey,TValue&gt;"/> 表示 I/O 及其欲更改之狀態
		/// </summary>
		/// <param name="ioDict">欲更改之 I/O 編號及其對應狀態</param>
		/// <example><code language="C#">
		/// CtAce mAce = new CtAce();
		/// 
		/// Dictionary&lt;int, bool&gt; ioList = new Dictionary&lt;int, bool&gt; { {2001, true}, {2002, true}, {2003, false} };
		/// mAce.IO.SetIO(ioList);  //設定 2001(ON) 2002(ON) 2003(OFF)
		/// </code></example>
		public void SetIO(Dictionary<int, bool> ioDict) {
			int[] sig = ioDict.Select(kvp => kvp.Key * (kvp.Value ? 1 : -1)).ToArray();
			mVpObj.Obj(obj => obj.SetDigitalIO(sig));
		}

		/// <summary>取得當前單一I/O狀態</summary>
		/// <param name="io">欲查詢之I/O編號</param>
		/// <param name="stt">回傳之當前 On(<see langword="true"/>) / Off(<see langword="false"/>) 狀態</param>
		/// <example><code language="C#">
		/// CtAce mAce = new CtAce();
		/// bool stt;
		/// mAce.IO.GetIO(97, out stt); //將 I/O 編號 97 之狀態存放至 stt 變數裡
		/// </code></example>
		public void GetIO(int io, out bool stt) {
			bool sig = false;
			sig = mVpObj.Obj(obj => obj.GetDigitalIO(io));
			stt = sig;
		}

		/// <summary>取得當前單一I/O狀態，並直接回傳該布林值(Boolean)</summary>
		/// <param name="io">欲查詢之I/O編號</param>
		/// <returns>回傳該布林值(Boolean)。 (<see langword="true"/>)ON; (<see langword="false"/>)OFF</returns>
		/// /// <example><code language="C#">
		/// CtAce mAce = new CtAce();
		/// bool stt = mAce.IO.GetIO(97); //將 I/O 編號 97 之狀態存放至 stt 變數裡
		/// </code></example>
		public bool GetIO(int io) {
			bool stt = false;
			stt = mVpObj.Obj(obj => obj.GetDigitalIO(io));
			return stt;
		}

		/// <summary>取得當前多個I/O狀態，並直接回傳該布林值(Boolean)陣列</summary>
		/// <param name="io">欲查詢之I/O編號陣列，如 int[] IOs = new int[] {94, 96, 105, 106}</param>
		/// <returns>回傳該布林值(Boolean)陣列。 (<see langword="true"/>)ON; (<see langword="false"/>)OFF</returns>
		/// <example><code language="C#">
		/// CtAce mAce = new CtAce();
		/// List&lt;bool&gt; ioStt = mAce.IO.GetIO(94, 96, 105, 106);
		/// </code></example>
		public List<bool> GetIO(params int[] io) {
			List<bool> sttColl = null;
			sttColl = mVpObj.Obj(obj => io.ConvertToList(sig => obj.GetDigitalIO(sig)));
			return sttColl;
		}

		/// <summary>取得當前多個I/O狀態</summary>
		/// <param name="io">欲查詢之I/O編號陣列，如 int[] IOs = new int[] {94, 96, 105, 106}</param>
		/// <param name="value">回傳之當前 On(<see langword="true"/>) / Off(<see langword="false"/>) List集合</param>
		/// <example><code language="C#">
		/// CtAce mAce = new CtAce();
		/// 
		/// int[] ioList = new int[] { 94, 96, 105, 106 };
		/// List&lt;bool&gt; ioStt;
		/// mAce.IO.GetIO(ioList, out ioStt);
		/// </code></example>
		public void GetIO(int[] io, out List<bool> value) {
			List<bool> sttColl = null;
			sttColl = mVpObj.Obj(obj => io.ConvertToList(sig => obj.GetDigitalIO(sig)));
			value = sttColl;
		}
		#endregion

		#region Function - Specified Robot I/O

		/// <summary>
		/// 更改特定機器手臂當前I/O狀態。
		/// 正整數表示開啟該I/O，負整數表關閉。如 2001 = 開啟2001; -2014 = 關閉2014
		/// </summary>
		/// <param name="robNum">指定的 <see cref="IAdeptRobot"/> 編號，預設從 1 開始</param>
		/// <param name="io">
		/// 欲更改之I/O編號。
		/// 可帶入陣列，如 int[] IOs = new int[] {2001, -2002, -2003, 2004}
		/// 也可採用逗號分隔，如 SetIO(2001, -2002, -2003, 2004)
		/// </param>
		/// <example><code language="C#">
		/// CtAce mAce = new CtAce();
		/// 
		/// List&lt;int&gt; ioList = new List&lt;int&gt; { 43, -44, -97, 98 };
		/// mAce.IO.SetRobotIO(ioList.ToArray());    //設定 43(ON) 44(OFF) 97(OFF) 98(ON)
		/// mAce.IO.SetRobotIO(2001, -2002);         //設定 2001(ON) 2002(OFF)
		/// </code></example>
		public void SetRobotIO(int robNum, params int[] io) {
			mVpObj.Obj(obj => obj.SetRobotDigitalIO(robNum, io));
		}

		/// <summary>
		/// 更改特定機器手臂當前I/O狀態。以布林(Boolean)表示ON/OFF狀態
		/// </summary>
		/// <param name="io">欲更改之I/O編號</param>
		/// <param name="robNum">指定的 <see cref="IAdeptRobot"/> 編號，預設從 1 開始</param>
		/// <param name="stt">(<see langword="true"/>)ON   (<see langword="false"/>)OFF</param>
		/// <example><code language="C#">
		/// CtAce mAce = new CtAce();
		/// 
		/// mAce.IO.SetRobotIO(97, false);   //設定 IO 編號 97 為 OFF
		/// mAce.IO.SetRobotIO(43, true);    //設定 IO 編號 43 為 ON
		/// </code></example>
		public void SetRobotIO(int robNum, int io, bool stt) {
			int ioStt = io * (stt ? 1 : -1);
			mVpObj.Obj(obj => obj.SetRobotDigitalIO(robNum, ioStt));
		}

		/// <summary>
		/// 更改特定機器手臂多個I/O狀態。以布林(Boolean)集合表示ON/OFF狀態
		/// </summary>
		/// <param name="robNum">指定的 <see cref="IAdeptRobot"/> 編號，預設從 1 開始</param>
		/// <param name="ioList">欲更改之I/O編號集合</param>
		/// <param name="ioStt">相對應 ioList 順序之狀態  (<see langword="true"/>)ON   (<see langword="false"/>)OFF</param>
		/// <example><code language="C#">
		/// CtAce mAce = new CtAce();
		/// 
		/// List&lt;int&gt; ioList = new List&lt;int&gt; { 2001, 2002, 2003, 2004, 2005 };
		/// List&lt;bool&gt; ioStt = new List&lt;bool&gt; { false, true, true, false, false };
		/// mAce.IO.SetRobotIO(ioList, ioStt);   //設定 2001(OFF) 2002(ON) 2003(ON) 2004(OFF) 2005(OFF)
		/// </code></example>
		public void SetRobotIO(int robNum, List<int> ioList, List<bool> ioStt) {
			if (ioList.Count == ioStt.Count) {
				int[] sig = ioList.Zip(ioStt, (io, stt) => io * (stt ? 1 : -1)).ToArray();
				mVpObj.Obj(obj => obj.SetRobotDigitalIO(robNum, sig));
			}
		}

		/// <summary>
		/// 更改特定機器手臂多個I/O狀態。以 <see cref="Dictionary&lt;TKey,TValue&gt;"/> 表示 I/O 及其欲更改之狀態
		/// </summary>
		/// <param name="robNum">指定的 <see cref="IAdeptRobot"/> 編號，預設從 1 開始</param>
		/// <param name="ioDict">欲更改之 I/O 編號及其對應狀態</param>
		/// <example><code language="C#">
		/// CtAce mAce = new CtAce();
		/// 
		/// Dictionary&lt;int, bool&gt; ioList = new Dictionary&lt;int, bool&gt; { {2001, true}, {2002, true}, {2003, false} };
		/// mAce.IO.SetRobotIO(ioList);  //設定 2001(ON) 2002(ON) 2003(OFF)
		/// </code></example>
		public void SetRobotIO(int robNum, Dictionary<int, bool> ioDict) {
			int[] sig = ioDict.Select(kvp => kvp.Key * (kvp.Value ? 1 : -1)).ToArray();
			mVpObj.Obj(obj => obj.SetRobotDigitalIO(robNum, sig));
		}

		/// <summary>取得特定機器手臂當前單一I/O狀態</summary>
		/// <param name="robNum">指定的 <see cref="IAdeptRobot"/> 編號，預設從 1 開始</param>
		/// <param name="io">欲查詢之I/O編號</param>
		/// <param name="value">回傳之當前 On(<see langword="true"/>) / Off(<see langword="false"/>) 狀態</param>
		/// <example><code language="C#">
		/// CtAce mAce = new CtAce();
		/// bool stt;
		/// mAce.IO.SetRobotIO(97, out stt); //將 I/O 編號 97 之狀態存放至 stt 變數裡
		/// </code></example>
		public void GetRobotIO(int robNum, int io, out bool value) {
			value = mVpObj.Obj(obj => obj.GetRobotDigitalIO(robNum, io));
		}

		/// <summary>取得特定機器手臂當前單一I/O狀態，並直接回傳該布林值(Boolean)</summary>
		/// <param name="robNum">指定的 <see cref="IAdeptRobot"/> 編號，預設從 1 開始</param>
		/// <param name="io">欲查詢之I/O編號</param>
		/// <returns>回傳該布林值(Boolean)。 (<see langword="true"/>)ON; (<see langword="false"/>)OFF</returns>
		/// /// <example><code language="C#">
		/// CtAce mAce = new CtAce();
		/// bool stt = mAce.IO.GetRobotIO(97); //將 I/O 編號 97 之狀態存放至 stt 變數裡
		/// </code></example>
		public bool GetRobotIO(int robNum, int io) {
			return mVpObj.Obj(obj => obj.GetRobotDigitalIO(robNum, io));
		}

		/// <summary>取得特定機器手臂當前多個I/O狀態，並直接回傳該布林值(Boolean)陣列</summary>
		/// <param name="robNum">指定的 <see cref="IAdeptRobot"/> 編號，預設從 1 開始</param>
		/// <param name="io">欲查詢之I/O編號陣列，如 int[] IOs = new int[] {94, 96, 105, 106}</param>
		/// <returns>回傳該布林值(Boolean)陣列。 (<see langword="true"/>)ON; (<see langword="false"/>)OFF</returns>
		/// <example><code language="C#">
		/// CtAce mAce = new CtAce();
		/// List&lt;bool&gt; ioStt = mAce.IO.GetRobotIO(94, 96, 105, 106);
		/// </code></example>
		public List<bool> GetRobotIO(int robNum, params int[] io) {
			List<bool> sttColl = null;
			sttColl = mVpObj.Obj(obj => io.ConvertToList(sig => obj.GetRobotDigitalIO(robNum, sig)));
			return sttColl;
		}

		/// <summary>取得特定機器手臂當前多個I/O狀態</summary>
		/// <param name="robNum">指定的 <see cref="IAdeptRobot"/> 編號，預設從 1 開始</param>
		/// <param name="io">欲查詢之I/O編號陣列，如 int[] IOs = new int[] {94, 96, 105, 106}</param>
		/// <param name="value">回傳之當前 On(<see langword="true"/>) / Off(<see langword="false"/>) List集合</param>
		/// <example><code language="C#">
		/// CtAce mAce = new CtAce();
		/// 
		/// int[] ioList = new int[] { 94, 96, 105, 106 };
		/// List&lt;bool&gt; ioStt;
		/// mAce.IO.GetRobotIO(ioList, out ioStt);
		/// </code></example>
		public void GetRobotIO(int robNum, int[] io, out List<bool> value) {
			List<bool> sttColl = null;
			sttColl = mVpObj.Obj(obj => io.ConvertToList(sig => obj.GetRobotDigitalIO(robNum, sig)));
			value = sttColl;
		}
		#endregion
	}
}
