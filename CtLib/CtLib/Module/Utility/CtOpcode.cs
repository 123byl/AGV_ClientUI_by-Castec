using System;
using System.Collections.Generic;
using System.Linq;

using CtLib.Library;

namespace CtLib.Module.Utility {

	#region Declaration - Enumerations

	/// <summary>各類 Opcode 範圍</summary>
	public enum OpcodeRange : int {

		/*-- System --*/
		/// <summary>Opcode 於 System 的基數</summary>
		SYSTEM_BASE = 0,
		/// <summary>Opcode 於 System 的範圍結尾</summary>
		SYSTEM_RANGE = 999,

		/*-- CAMPro --*/
		/// <summary>Opcode 於 CAMPro 的基數</summary>
		CAMPRO_BASE = 1000,
		/// <summary>Opcode 於 CAMPro 的範圍結尾</summary>
		CAMPRO_RANGE = 1999,

		/*-- Mobile Robot --*/
		/// <summary>Opcode 於 Mobile 的基數</summary>
		MOBILE_BASE = 2000,
		/// <summary>Opcode 於 Mobile 的範圍結尾</summary>
		MOBILE_RANGE = 2999,

		/*-- WAGO --*/
		/// <summary>Opcode 於 WAGO module 的基數</summary>
		WAGO_IO_BASE = 11000,
		/// <summary>Opcode 於 WAGO module 的範圍結尾</summary>
		WAGO_IO_RANGE = 11999,

		/*-- Beckhoff --*/
		/// <summary>Opcode 於 Beckhoff module 的基數</summary>
		BECKHOFF_BASE = 12000,
		/// <summary>Opcode 於 Beckhoff module 的範圍結尾</summary>
		BECKHOFF_RANGE = 12999,

		/*-- ARCL --*/
		/// <summary>Opcode 於 ARCL module 的基數</summary>
		ARCL_BASE = 21000,
		/// <summary>Opcode 於 ARCL module 的範圍結尾</summary>
		ARCL_RANGE = 21999,

		/*-- ACE --*/
		/// <summary>Opcode 於 Ace module 的基數</summary>
		ACE_BASE = 22000,
		/// <summary>Opcode 於 Ace module 的範圍結尾</summary>
		ACE_RANGE = 22999,

		/*-- Vision --*/
		/// <summary>Opcode 於 Vision module 的基數</summary>
		VISION_BASE = 30000,
		/// <summary>Opcode 於 Vision module 的範圍結尾</summary>
		VISION_RANGE = 30999,

		/*-- Dimmer --*/
		/// <summary>Opcode 於 調光器模組 的基數</summary>
		DIMMER_BASE = 31000,
		/// <summary>Opcode 於 調光器模組 的範圍結尾</summary>
		DIMMER_RANGE = 31999,

		/*-- Stäubli --*/
		/// <summary>Opcode 於 Stäubli Module 的基數</summary>
		STAUBLI_BASE = 40000,
		/// <summary>Opcode 於 Stäubli Module 的範圍結尾</summary>
		STAUBLI_RANGE = 40999,

		/*-- Delta --*/
		/// <summary>Opcode 於 Delta Module 的基數</summary>
		DELTA_BASE = 50000,
		/// <summary>Opcode 於 Delta Module 的範圍結尾</summary>
		DELTA_RANGE = 50999,

		/*-- Other --*/
		/// <summary>Opcode 於 MySQL module 的基數</summary>
		MYSQL_BASE = 60000,
		/// <summary>Opcode 於 MySQL module 的範圍結尾</summary>
		MYSQL_RANGE = 60099,
		/// <summary>Opcode 於 SECS/GEM module 的基數</summary>
		CIM_BASE = 60100,
		/// <summary>Opcode 於 SECS/GEM module 的範圍結尾</summary>
		CIM_RANGE = 60199,
		/// <summary>Opcode 於 Brillian RFID module 的基數</summary>
		BRILLIAN_RFID_BASE = 60300,
		/// <summary>Opcode 於 Brillian RFID module 的範圍結尾</summary>
		BRILLIAN_RFID_RANGE = 60399,

		/*-- Universal Robots --*/
		/// <summary>Opcode 於 Universal Robots 的基數</summary>
		UR_BASE = 70000,
		/// <summary>Opcode 於 Universal Robots 的範圍結尾</summary>
		UR_RANGE = 70999
	}

	#endregion

	#region Declaration - Support Class

	/// <summary>Opcode 資料</summary>
	public class OpcodeData : IComparable<OpcodeData>, IComparer<OpcodeData> {

		#region Declaration - Fields

		/// <summary>.opc文件中之行號</summary>
		public int DocumentLine { get; set; }
		/// <summary>.opc文件中之原始字串</summary>
		public string DocumentText { get; set; }
		/// <summary>OpCode號碼</summary>
		public int Opcode { get; set; }
		/// <summary>索引值</summary>
		public int Index { get; set; }
		/// <summary>註解</summary>
		public string Comment { get; set; }
		/// <summary>參數集合</summary>
		public List<string> Argument { get; set; }

		#endregion

		#region Functions - Core
		/// <summary>建構元，建立全空的訊息類別</summary>
		public OpcodeData() {
			DocumentLine = -1;
			DocumentText = string.Empty;
			Opcode = 0;
			Comment = string.Empty;
			Argument = new List<string>();
		}
		/// <summary>建構元，建立包含所有訊息之類別</summary>
		/// <param name="docLine">.opc文件中之行號</param>
		/// <param name="docText">.opc文件中之原始字串</param>
		/// <param name="code">OpCode號碼</param>
		/// <param name="idx">索引值</param>
		/// <param name="cmt">註解</param>
		/// <param name="arg">參數集合</param>
		public OpcodeData(int docLine, string docText, int code, int idx, string cmt, List<string> arg) {
			DocumentLine = docLine;
			DocumentText = docText;
			Opcode = code;
			Index = idx;
			Comment = cmt;
			Argument = arg;
		}
		/// <summary>建構元，建立必要訊息之類別</summary>
		/// <param name="code">OpCode號碼</param>
		/// <param name="idx">索引值</param>
		/// <param name="cmt">註解</param>
		/// <param name="arg">參數集合</param>
		public OpcodeData(int code, int idx, string cmt, List<string> arg) {
			Opcode = code;
			Index = idx;
			Comment = cmt;
			Argument = arg;
			DocumentLine = -1;
			DocumentText = string.Empty;
		}

		#endregion

		#region Function - Clone
		/// <summary>取的此 <see cref="OpcodeData"/> 複本</summary>
		/// <returns><see cref="OpcodeData"/></returns>
		public OpcodeData Clone() {
			return new OpcodeData(DocumentLine, DocumentText, Opcode, Index, Comment, Argument.ToList());
		}
		#endregion

		#region Function - Overrides
		/// <summary>取得此 Opcode 的文字描述</summary>
		/// <returns>文字描述</returns>
		public override string ToString() {
			return string.Format("{0},{1},[ {2} ] = {3}", Opcode, Index, Comment, string.Join(",", Argument));
		}

		/// <summary>比較兩者 <see cref="OpcodeData"/> 並指出是否相同</summary>
		/// <param name="obj">欲比較的物件</param>
		/// <returns>(<see langword="true"/>)兩者相同  (<see langword="false"/>)兩者不同</returns>
		public override bool Equals(object obj) {
			OpcodeData opc = obj as OpcodeData;
			if (opc == null || (object)this == null) return false;
			return opc.Opcode == Opcode && opc.Index == Index && opc.Comment == Comment && opc.Argument.SequenceEqual(Argument);
		}

		/// <summary>取得此 <see cref="OpcodeData"/> 之湊雜碼</summary>
		/// <returns>湊雜碼</returns>
		public override int GetHashCode() {
			return Opcode ^ Index ^ Comment.GetHashCode() ^ Argument.GetHashCode();
		}

		/// <summary>比較兩者 <see cref="OpcodeData"/> 並指出是否相同</summary>
		/// <param name="a">欲比較的物件</param>
		/// <param name="b">被比較的物件</param>
		/// <returns>(<see langword="true"/>)兩者相同  (<see langword="false"/>)兩者不同</returns>
		public static bool operator ==(OpcodeData a, OpcodeData b) {
			if ((object)a == null && (object)b == null) return true;
			else if ((object)a == null || (object)b == null) return false;
			else return a.Opcode == b.Opcode && a.Index == b.Index && a.Comment == b.Comment && a.Argument.SequenceEqual(b.Argument);
		}

		/// <summary>比較兩者 <see cref="OpcodeData"/> 並指出是否不同</summary>
		/// <param name="a">欲比較的物件</param>
		/// <param name="b">被比較的物件</param>
		/// <returns>(<see langword="true"/>)兩者不同  (<see langword="false"/>)兩者相同</returns>
		public static bool operator !=(OpcodeData a, OpcodeData b) {
			return !(a == b);
		}
		#endregion

		#region Function - ICompareable & IComparer Implements
		/// <summary>比較兩個 <see cref="OpcodeData"/> 之大小，以 Opcode 與 Index 為比較基準</summary>
		/// <param name="other">欲比較的 <see cref="OpcodeData"/></param>
		/// <returns>(0)兩者同大 (-1)對方比較大 (1)自己比較大</returns>
		public int CompareTo(OpcodeData other) {
			int compare = Opcode.CompareTo(other.Opcode);
			if (compare == 0) compare = Index.CompareTo(other.Index);
			return compare;
		}

		/// <summary>比較兩個 <see cref="OpcodeData"/> 之大小，以 Opcode 與 Index 為比較基準</summary>
		/// <param name="x">欲比較的 <see cref="OpcodeData"/></param>
		/// <param name="y">被比較的 <see cref="OpcodeData"/></param>
		/// <returns>(0)兩者同大 (-1)x比較大 (1)y比較大</returns>
		public int Compare(OpcodeData x, OpcodeData y) {
			int compare = x.Opcode.CompareTo(y.Opcode);
			if (compare == 0) compare = x.Index.CompareTo(y.Index);
			return compare;
		}

		#endregion
	}

	#endregion

	/// <summary>
	/// Opcode 相關操作
	/// <para>包含了 讀取 *.opc、儲存 *.opc 等等操作</para>
	/// </summary>
	/// <example>
	/// VB.Net 版本之 CtOpcode 是採用讓其他程式繼承之方式
	/// <para>但思考後覺得沒有必要使用繼承，且考慮到不只一處會使用，故將此設計以物件方式實作</para>
	/// <para>以下為基本的使用方式</para>
	/// <code language="C#">
	/// CtOpcode opc = new CtOpcode();
	/// 
	/// /*-- 載入 Opcode --*/
	/// Stat stt = opc.LoadOpcode(@"D:\CASTEC\Config\Demo.opc");    //載入檔案並儲存到物件裡
	/// AllocateOpcode(opc.OpcodeCollection);                       //假設 AllocateOpcode 為分析各 Opcode 之副程式
	/// 
	/// /*-- 更新 Opcode (含寫入檔案) --*/
	/// opc.SaveOpcode(12000, "New Value");                         //假設 12000 已存在會更新數值，如果尚未存在則會在最接近 Opcode 的地方插入新的數值
	/// </code>
	/// 如是要刪除 Opcode，目前仍採直接修改檔案的方式
	/// </example>
	public class CtOpcode : ICtVersion {

		#region Version

		/// <summary>CtOpcode 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 0.0.0  William [2012/05/18]
		///     + CtOpcode
		///     
		/// 1.0.0  Ahern [2014/07/18]
		///     + 從舊版CtLib搬移
		///     
		/// 1.0.1  Ahern [2014/09/15]
		///     + SaveOpcode加入params，較有彈性
		///     
		/// 1.0.2  Ahern [2014/09/19]
		///     + 利用Collection更新並儲存之SaveOpcode
		///     
		/// 1.0.3  Ahern [2014/10/16]
		///     \ 修正參數分割方式，避免參數帶有相同符號而失敗
		///     
		/// 1.0.4  Ahern [2014/10/28]
		///     \ 將OpcodeRange與OpcodeData拉到CtOpcode底下
		///     
		/// 1.0.5  Ahern [2015/02/11]
		///     \ 將預設路徑改以 Resource 之路徑為主
		///     \ 搜尋等程式改以 Lambda 方法施作    
		/// 
		/// 1.0.6  Ahern [2015/02/25]
		///     \ OpcodeCollection 改以 Property 實作
		/// 
		/// 1.0.7  Ahern [2015/02/27]
		///     \ 修正 Argument 改以搜尋第一個 '=' 位置，避免參數中若有等號也會被消掉
		///     
		/// 1.0.8  Ahern [2015/03/03]
		///     \ 根據 Johnson 要求，將 Argument 一定要有參數之限制解除
		///     
		/// 1.0.9  Ahern [2015/05/25]
		///     \ 精簡 LoadOpcode
		///     
		/// 1.1.0  Ahern [2016/07/10]
		///		\ 刪除不必要的方法
		///		\ SaveOpcode 加上 Index 判斷與搜尋
		/// 
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(1, 1, 0, "2016/07/10", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Properties

		/// <summary>取得或設定 Opcode 檔案路徑</summary>
		public string FilePath {
			get { return mPath; }
			set { mPath = value; }
		}

		/// <summary>取得目前已載入的 Opcode</summary>
		public List<OpcodeData> OpcodeCollection { get { return mOpcCollect.ConvertAll(opc => opc.Clone()); } }
		#endregion

		#region Declaration - Fields

		/// <summary>Opcode 檔案路徑</summary>
		private string mPath = CtDefaultPath.GetPath(SystemPath.Configuration) + "CASTEC.opc";
		/// <summary>Opcode 集合。已載入的Opcode會儲存一份在此變數中(需開啟此功能)</summary>
		private List<OpcodeData> mOpcCollect;

		#endregion

		#region Functions - Methods

		/// <summary>組合 Opcode 訊息字串，包含編號、索引、註解與參數</summary>
		/// <param name="opcCode">Opcode 編號</param>
		/// <param name="idx">Opcode 索引</param>
		/// <param name="cmt">該項目註解</param>
		/// <param name="arg">參數集合</param>
		/// <returns>已組合完成之 Opcode 字串</returns>
		private string CombineOpcodeString(int opcCode, int idx, string cmt, List<string> arg) {
			return string.Format(
				"{0},{1},[ {2} ] = {3}",
				opcCode.ToString(),
				idx.ToString(),
				cmt,
				string.Join(",", arg)
			);
		}

		/// <summary>尋找 Opcode 原始文件之該 Opcode 編號索引，如無符合項目將回傳(-1)</summary>
		/// <param name="oriCntx">Opcode文件字串集合</param>
		/// <param name="opcCode">Opcode 編號</param>
		/// <param name="idx">Opcode 索引</param>
		/// <returns>找尋到符合的 Opcode 編號索引，如無符合項目將回傳(-1)</returns>
		private int FindIndex(List<string> oriCntx, int opcCode, int idx) {
			string msg = string.Format("{0},{1}", opcCode.ToString(), idx.ToString());
			/* 已測試過，如果找不到東西，則會回傳 -1，不會有 Exception */
			return oriCntx.FindIndex(val => val.StartsWith(msg));
		}

		/// <summary>尋找 mOpcCollect，並回傳符合 Opcode 編號的索引(Index)</summary>
		/// <param name="opcCode">欲尋找之 Opcode 編號</param>
		/// <param name="idx">Opcode 索引</param>
		/// <returns>最符合的 Opcode 編號索引</returns>
		private int FindIndex(int opcCode, int idx) {
			/* 如果找不到東西，則會回傳 -1 */
			return mOpcCollect.FindIndex(dlgOpc => dlgOpc.Opcode == opcCode && dlgOpc.Index == idx); ;
		}

		/// <summary>
		/// 尋找原始文件字串新增 Opcode 之適合位置(索引Index)
		/// <para>插入於大於編號的第一個位置，如 "1001"，則插入於 "1000" 的下一行</para>
		/// </summary>
		/// <param name="oriCntx">原始文件字串</param>
		/// <param name="opcCode">欲尋找之 Opcode</param>
		/// <param name="idx">Opcode 索引</param>
		/// <returns>最符合的插入的編號索引</returns>
		private int FindInsertIndex(List<string> oriCntx, int opcCode, int idx) {
			int intIdx = -1;
			if (mOpcCollect.Count > 0) {
				/*-- 檢查是否有相同 Opcode，但索引不同的位置 --*/
				List<OpcodeData> existOpc = mOpcCollect.FindAll(opc => opc.Opcode == opcCode);
				if (existOpc != null && existOpc.Count > 0) {	//此為新增 Index
					intIdx = existOpc.OrderBy(opc => opc.Index).Last().DocumentLine + 1;
				} else {	//新增 Opcode
					intIdx = mOpcCollect.FindLast(opc => opcCode > opc.Opcode).DocumentLine + 1;
				}
			}

			return intIdx;
		}

		/// <summary>於已載入之 <see cref="mOpcCollect"/> 搜尋相對應的 <see cref="OpcodeData.Opcode"/>。如該值不存在，則回傳 null</summary>
		/// <param name="opcCode">欲查詢之 Opcode 編號</param>
		/// <returns>第一個相符的 OpcodeData，如不存在則回傳 null</returns>
		private OpcodeData FindOpcodeData(int opcCode) {
			return mOpcCollect.Find(data => data.Opcode == opcCode);
		}

		/// <summary>於已載入之 <see cref="mOpcCollect"/> 搜尋相對應的 <see cref="OpcodeData.Opcode"/>。如該值不存在，則回傳 null</summary>
		/// <param name="opcCode">欲查詢之Opcode編號</param>
		/// <param name="idx">欲查詢的索引</param>
		/// <returns>第一個相符的 OpcodeData，如不存在則回傳 null</returns>
		private OpcodeData FindOpcodeData(int opcCode, int idx) {
			return mOpcCollect.Find(data => data.Opcode == opcCode && data.Index == idx);
		}

		/// <summary>分析並拆解Opcode字串，並塞入OpcodeData物件</summary>
		/// <param name="oriCntx">原始Opcode字串</param>
		/// <param name="lineNo">原文件列號</param>
		/// <param name="opc">欲儲存之Opcode物件</param>
		private void AnalyzeOpcodeString(string oriCntx, int lineNo, out OpcodeData opc) {
			OpcodeData opcTemp = null;		/* 暫存拆解完成之Opcode */
			string[] splitComa;				/* 字串利用 ',' 拆解之String陣列 */
			string[] splitCmt;				/* 字串利用 '[' ']' '=' 拆解之String陣列 */
			string[] splitArg;				/* 註解後半段利用 ',' 拆解之String陣列 */
			string combArg = string.Empty;	/* Combine Argument */
			string cmt = string.Empty;		/* 註解 */
			string arg = string.Empty;		/* 用於計算後半段之參數，避免後面參數也含有 '=' 而導致參數錯誤 */
			int code = 0;					/* 暫存TryParse回傳之Opcode編號 */
			int idx = 0;					/* 暫存TryParse回傳之Index */

			/*-- 如果是//開頭表示註解 --*/
			if ((!oriCntx.StartsWith("//")) && (oriCntx.Trim().Length > 0)) {
				splitComa = oriCntx.Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries);
				if (splitComa.Length > 0) {

					/*-- 先拆逗號，確認有Opcode後再往下拆 --*/
					if (int.TryParse(splitComa[0], out code)) {
						/*-- 有Opcode後拆Index --*/
						if (int.TryParse(splitComa[1], out idx)) {
							/*-- 如果後面還有字串，把它組合起來 --*/
							combArg = string.Empty;
							if (splitComa.Length > 3)
								combArg += string.Join(",", splitComa, 2, splitComa.Length - 2);
							else
								combArg = splitComa[2];

							/*-- 將後續字串用 " [ ] = " 拆開 --*/
							splitCmt = combArg.Split(CtConst.CHR_OPCODE, StringSplitOptions.RemoveEmptyEntries);
							if (splitCmt.Length < 2)
								throw new Exception(CtFile.GetFileName(mPath) + " 第 " + CtConvert.CStr(lineNo) + " 列之註解與參數分割失敗");

							/*-- 抽取註解出來 --*/
							cmt = splitCmt[0].Trim();

							/*-- 將參數部分利用逗號拆開 --*/
							arg = combArg.Remove(0, combArg.IndexOf("[") + 1);
							arg = arg.Remove(0, arg.IndexOf("]") + 1);
							arg = arg.Remove(0, arg.IndexOf("=") + 1).Trim();
							splitArg = arg.Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries);

							/*-- 將資訊加入List中 --*/
							opcTemp = new OpcodeData(lineNo, oriCntx, code, idx, cmt, splitArg.ToList());

						} else {
							throw new Exception(CtFile.GetFileName(mPath) + " 第 " + CtConvert.CStr(lineNo) + " 列之 Index 無法轉換為 int");
						}
					} else {
						throw new Exception(CtFile.GetFileName(mPath) + " 第 " + CtConvert.CStr(lineNo) + " 列之 Opcode 無法轉換為 int");
					}
				}
			}

			opc = opcTemp;
		}

		/// <summary>載入特定路徑之Opcode檔案，並丟出該OpcodeData集合</summary>
		/// <param name="path">Opcode 檔案路徑，如 @"D:\CASTEC\Config\CASTEC.opc"</param>
		/// <param name="opcode">讀取完畢之OpcodeData集合</param>
		private void LoadOpcode(string path, out List<OpcodeData> opcode) {
			OpcodeData opcTemp;
			List<OpcodeData> opc = new List<OpcodeData>();

			/*-- 載入檔案並拆解字串 --*/
			List<string> strLines = CtFile.ReadFile(path);

			/*-- 將每一列都去拆解並將回傳的OpcodeData存至mOpcCollect --*/
			int index = 0;
			strLines.ForEach(
				val => {
					/*-- 拆解字串，如果成功則將OpcodeData儲存 --*/
					AnalyzeOpcodeString(val, index, out opcTemp);
					if (opcTemp != null) opc.Add(opcTemp);
					index++;
				}
			);

			opcode = opc;
		}
		#endregion

		#region Functions - Core

		/// <summary>載入 <see cref="FilePath"/> 之檔案至記憶體中(<seealso cref="mOpcCollect"/>)</summary>
		public void LoadOpcode() {
			LoadOpcode(mPath, out mOpcCollect);
		}

		/// <summary>載入指定檔案至記憶體中(<seealso cref="mOpcCollect"/>)</summary>
		/// <param name="path">Opcode 檔案路徑，如 @"D:\CASTEC\Config\CASTEC.opc"</param>
		public void LoadOpcode(string path) {
			/*-- 回存指定路徑 --*/
			mPath = path;

			/*-- 載入之 --*/
			LoadOpcode(path, out mOpcCollect);
		}

		/// <summary>新增或更新 OpcodeData，並寫入 FilePath 檔案</summary>
		/// <param name="opc">欲儲存之 OpcodeData 物件</param>
		public void SaveOpcode(OpcodeData opc) {
			/*-- 利用已建立之Function達成 --*/
			SaveOpcode(mPath, opc);
		}

		/// <summary>新增或更新 Opcode 相關資訊至 FilePath 檔案</summary>
		/// <param name="opcode">Opcode 編號</param>
		/// <param name="index">Opcode 索引</param>
		/// <param name="comment">註解</param>
		/// <param name="arg">參數集合</param>
		public void SaveOpcode(int opcode, int index, string comment, params string[] arg) {
			/*-- 檢查是否已有紀錄 --*/
			OpcodeData opc = FindOpcodeData(opcode, index);
			if (opc == null) {
				opc = new OpcodeData(opcode, index, comment, arg.ToList());
			} else {
				opc.Comment = comment;
				opc.Argument.Clear();
				opc.Argument = arg.ToList();
			}

			/*-- 利用現有Function達成 --*/
			SaveOpcode(mPath, new OpcodeData(opcode, index, comment, arg.ToList()));
		}

		/// <summary>利用舊有 mOpcCollect 更新參數 Opcode 至 FilePath 檔案</summary>
		/// <param name="opcode">Opcode 編號</param>
		/// <param name="arg">參數集合</param>
		public void SaveOpcode(int opcode, params string[] arg) {
			OpcodeData opc = FindOpcodeData(opcode);

			if (opc != null) {
				opc.Argument.Clear();
				opc.Argument = arg.ToList();
			} else {
				opc = new OpcodeData(opcode, 0, string.Empty, arg.ToList());
			}

			SaveOpcode(mPath, opc);
		}

		/// <summary>利用舊有 mOpcCollect 更新參數 Opcode 至 FilePath 檔案</summary>
		/// <param name="opcode">Opcode 編號</param>
		/// <param name="idx">Opcode 索引</param>
		/// <param name="arg">參數集合</param>
		public void SaveOpcode(int opcode, int idx, params string[] arg) {
			OpcodeData opc = FindOpcodeData(opcode, idx);

			if (opc != null) {
				opc.Argument.Clear();
				opc.Argument = arg.ToList();
			} else {
				opc = new OpcodeData(opcode, idx, string.Empty, arg.ToList());
			}

			SaveOpcode(mPath, opc);
		}

		/// <summary>新增或更新 Opcode 相關資訊至特定 Opcode 檔案</summary>
		/// <param name="path">特定的 Opcode 檔案路徑</param>
		/// <param name="opcode">Opcode 編號</param>
		/// <param name="index">Opcode 索引</param>
		/// <param name="comment">註解</param>
		/// <param name="arg">參數集合</param>
		public void SaveOpcode(string path, int opcode, int index, string comment, params string[] arg) {
			/*-- 利用現有Function達成 --*/
			SaveOpcode(path, new OpcodeData(opcode, index, comment, arg.ToList()));
		}

		/// <summary>新增或更新 OpcodeData，並寫入特定 Opcode 檔案</summary>
		/// <param name="path">特定的 Opcode 檔案路徑</param>
		/// <param name="opc">欲儲存之 OpcodeData 物件</param>
		public void SaveOpcode(string path, OpcodeData opc) {
			bool inserted = false;
			int intLineNum = -1, intColIdx = -1;
			string strMsg = string.Empty;
			List<string> strDocument = new List<string>();

			/*-- 載入Opcode檔案 --*/
			mPath = path;
			strDocument = CtFile.ReadFile(path);

			/*-- 抓取Index(文件列減1)，如果已經有指定(大於 -1)則直接寫入該行，否則用搜尋的 --*/
			intLineNum = (opc.DocumentLine > -1) ? opc.DocumentLine : FindIndex(strDocument, opc.Opcode, opc.Index);

			/*-- 抓取全域Opcode index，並取代資料。如果沒有則會回傳 -1 --*/
			intColIdx = FindIndex(opc.Opcode, opc.Index);

			/*-- 組合Opcode字串 --*/
			strMsg = CombineOpcodeString(opc.Opcode, opc.Index, (((opc.Comment == string.Empty) && (intColIdx > -1)) ? mOpcCollect[intColIdx].Comment : opc.Comment), opc.Argument);

			/*-- 覆蓋文字檔 --*/
			if ((intLineNum < 0) && mOpcCollect.Count > 0) {
				intLineNum = FindInsertIndex(strDocument, opc.Opcode, opc.Index);
				strDocument.Insert(intLineNum, strMsg);
				inserted = true;
			} else if (intLineNum < 0) {
				strDocument.Add(strMsg);
			} else {
				strDocument[intLineNum] = strMsg;
			}

			/*-- 覆蓋全域Opcode，但如果之前選擇不儲存至全域，那就跳過 --*/
			if ((intColIdx < 0) && (mOpcCollect.Count > 0)) {
				mOpcCollect.Add(opc);
				intColIdx = mOpcCollect.Count - 1;
			} else if (intColIdx > -1) {
				mOpcCollect[intColIdx].DocumentLine = intLineNum;
				mOpcCollect[intColIdx].Index = opc.Index;
				mOpcCollect[intColIdx].Argument = opc.Argument;
				mOpcCollect[intColIdx].DocumentText = strMsg;
				if (opc.Comment != string.Empty)
					mOpcCollect[intColIdx].Comment = opc.Comment;
			}

			/*-- 如果有插入，則把在這之後的行數加一 --*/
			if (inserted) {
				mOpcCollect.ForEach(
					val => {
						if (val.DocumentLine > intLineNum) {
							val.DocumentLine++;
						}
					}
				);
			}

			/*-- 寫入檔案 --*/
			CtFile.WriteFile(path, strDocument);
		}

		#endregion
	}
}
