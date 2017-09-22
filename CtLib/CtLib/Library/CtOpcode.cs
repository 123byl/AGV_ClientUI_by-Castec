using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CtLib.Library;
using CtLib.Module.Ultity;

namespace CtLib.Library {

    /// <summary>
    /// Opcode 相關操作
    /// <para>包含了 讀取 *.opc、儲存 *.opc 等等操作</para>
    /// </summary>
    /// <example>
    /// VB.Net 版本之 CtOpcode 是採用讓其他程式繼承之方式
    /// <para>但思考後覺得沒有必要使用繼承，且考慮到不只一處會使用，故將此設計以物件方式實作</para>
    /// <para>以下為基本的使用方式</para>
    /// <code>
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
    public class CtOpcode {

        #region Version

        /// <summary>CtOpcode 版本訊息</summary>
        /// <remarks><code>
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
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 0, 9, "2015/05/25", "Ahern Kuo");

        #endregion

        #region Declaration - Support Class

        /// <summary>Opcode資料存放結構</summary>
        public class OpcodeData {

            #region Declaration - Members

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
                DocumentText = "";
                Opcode = 0;
                Comment = "";
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
            }

            #endregion
        }

        #endregion

        #region Declaration - Properties

        /// <summary>取得或設定Opcode檔案路徑</summary>
        public string OPCODE_PATH {
            get { return mPath; }
            set { mPath = value; }
        }

        /// <summary>取得目前已載入的 Opcode</summary>
        public List<OpcodeData> OpcodeCollection { get { return mOpcCollect; } }
        #endregion

        #region Declaration - Members

        /// <summary>Opcode檔案路徑</summary>
        private string mPath = CtDefaultPath.GetPath(SystemPath.CONFIG) + "CASTEC.opc";
        /// <summary>Opcode集合陣列。已載入的Opcode會儲存一份在此變數中(需開啟此功能)</summary>
        private List<OpcodeData> mOpcCollect;

        #endregion

        #region Functions - Methods

        /// <summary>組合Opcode訊息字串，包含編號、索引、註解與參數</summary>
        /// <param name="opcCode">Opcode 編號</param>
        /// <param name="opcIdx">Opcode 索引</param>
        /// <param name="opcCmt">該項目註解</param>
        /// <param name="opcArg">參數集合</param>
        /// <returns>已組合完成之Opcode字串</returns>
        private string CombineOpcodeString(int opcCode, int opcIdx, string opcCmt, List<string> opcArg) {
            string strMsg = "";

            strMsg = CtConvert.CStr(opcCode) + "," + CtConvert.CStr(opcIdx) + ",";
            strMsg += "[ " + opcCmt + " ] = ";
            strMsg += string.Join(",", opcArg.ToArray());

            return strMsg;
        }

        /// <summary>尋找Opcode原始文件之該Opcode編號索引，如無符合項目將回傳(-1)</summary>
        /// <param name="str">Opcode文件字串集合</param>
        /// <param name="opcCode">Opcode 編號</param>
        /// <returns>找尋到符合的Opcode編號索引，如無符合項目將回傳(-1)</returns>
        private int FindIndex(List<string> str, int opcCode) {
            /* 已測試過，如果找不到東西，則會回傳 -1，不會有 Exception */
            return str.FindIndex(val => val.Contains(CtConvert.CStr(opcCode)));
        }

        /// <summary>尋找mOpcCollect，並回傳符合Opcode編號的索引(Index)</summary>
        /// <param name="opcCode">欲尋找之 Opcode 編號</param>
        /// <returns>最符合的Opcode編號索引</returns>
        private int FindIndex(int opcCode) {
            /* 已測試過，如果找不到東西，則會回傳 -1，不會有 Exception */
            return mOpcCollect.FindIndex(dlgOpc => dlgOpc.Opcode == opcCode); ;
        }

        /// <summary>
        /// 尋找原始文件字串新增Opcode之適合位置(索引Index)
        /// 插入於比該編號還大之位置(前一格)
        /// </summary>
        /// <param name="str">原始文件字串</param>
        /// <param name="opcCode">欲尋找之Opcode</param>
        /// <returns>最符合的插入的編號索引</returns>
        private int FindInsertIndex(List<string> str, int opcCode) {
            int intIdx = -1;
            if (mOpcCollect.Count > 0) {
                intIdx = mOpcCollect.FindIndex(val => val.Opcode > opcCode);
                if (intIdx > -1) intIdx = FindIndex(str, mOpcCollect[intIdx].Opcode);
            }

            return intIdx;
        }

        /// <summary>於已載入之 mOpcCollect 搜尋相對應 Opcode 編號之 OpcodeData。如該值不存在，則回傳 null</summary>
        /// <param name="opcCode">欲查詢之Opcode編號</param>
        /// <returns>第一個相符的 OpcodeData，如不存在則回傳 null</returns>
        private OpcodeData FindOpcodeData(int opcCode) {
            OpcodeData opc = mOpcCollect.Find(data => data.Opcode == opcCode);
            return opc;
        }

        /// <summary>分析並拆解Opcode字串，並塞入OpcodeData物件</summary>
        /// <param name="str">原始Opcode字串</param>
        /// <param name="lineNum">原文件列號</param>
        /// <param name="opc">欲儲存之Opcode物件</param>
        private void AnalyzeOpcodeString(string str, int lineNum, out OpcodeData opc) {
            OpcodeData opcTemp = null;      /* 暫存拆解完成之Opcode */
            string[] strCOMMA;              /* 字串利用 ',' 拆解之String陣列 */
            string[] strCmtArg;             /* 字串利用 '[' ']' '=' 拆解之String陣列 */
            string[] strArg;                /* 註解後半段利用 ',' 拆解之String陣列 */
            string strCombArg = "";         /* Combine Argument */
            string strComment = "";         /* 註解 */
            string strCalcArg = "";         /* 用於計算後半段之參數，避免後面參數也含有 '=' 而導致參數錯誤 */
            int intOpc = 0;                 /* 暫存TryParse回傳之Opcode編號 */
            int intIdx = 0;                 /* 暫存TryParse回傳之Index */

            /*-- 如果是//開頭表示註解 --*/
            if ((!str.StartsWith("//")) && (str.Trim().Length > 0)) {
                strCOMMA = str.Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries);
                if (strCOMMA.Length > 0) {

                    /*-- 先拆逗號，確認有Opcode後再往下拆 --*/
                    if (int.TryParse(strCOMMA[0], out intOpc)) {
                        /*-- 有Opcode後拆Index --*/
                        if (int.TryParse(strCOMMA[1], out intIdx)) {
                            /*-- 如果後面還有字串，把它組合起來 --*/
                            strCombArg = "";
                            if (strCOMMA.Length > 3)
                                strCombArg += string.Join(",", strCOMMA, 2, strCOMMA.Length - 2);
                            else
                                strCombArg = strCOMMA[2];

                            /*-- 將後續字串用 " [ ] = " 拆開 --*/
                            strCmtArg = strCombArg.Split(CtConst.CHR_OPCODE, StringSplitOptions.RemoveEmptyEntries);
                            if (strCmtArg.Length < 2)
                                throw new Exception(CtFile.GetFileName(mPath) + " 第 " + CtConvert.CStr(lineNum) + " 列之註解與參數分割失敗");

                            /*-- 抽取註解出來 --*/
                            strComment = strCmtArg[0].Trim();

                            /*-- 將參數部分利用逗號拆開 --*/
                            strCalcArg = strCombArg.Remove(0, strCombArg.IndexOf("[") + 1);
                            strCalcArg = strCalcArg.Remove(0, strCalcArg.IndexOf("]") + 1);
                            strCalcArg = strCalcArg.Remove(0, strCalcArg.IndexOf("=") + 1).Trim();
                            strArg = strCalcArg.Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries);

                            /*-- 將資訊加入List中 --*/
                            opcTemp = SetOpcode(CtConvert.CInt(lineNum), str, intOpc, intIdx, strComment, strArg.ToList());

                        } else {
                            throw new Exception(CtFile.GetFileName(mPath) + " 第 " + CtConvert.CStr(lineNum) + " 列之 Index 無法轉換為 int");
                        }
                    } else {
                        throw new Exception(CtFile.GetFileName(mPath) + " 第 " + CtConvert.CStr(lineNum) + " 列之 Opcode 無法轉換為 int");
                    }
                }
            }

            opc = opcTemp;
        }

        #endregion

        #region Functions - Core

        /// <summary>寫入Opcode相關資訊至OpcodeData變數</summary>
        /// <param name="opc">欲寫入之 OpcodeData 物件</param>
        /// <param name="docLine">原始文件列號</param>
        /// <param name="docText">原始文件文字</param>
        /// <param name="code">Opcode 編號</param>
        /// <param name="idx">Index</param>
        /// <param name="cmt">註解</param>
        /// <param name="arg">參數集合</param>
        public void SetOpcode(ref OpcodeData opc, int docLine, string docText, int code, int idx, string cmt, List<string> arg) {
            opc.DocumentLine = docLine;
            opc.DocumentText = docText;
            opc.Opcode = code;
            opc.Index = idx;
            opc.Comment = cmt;
            opc.Argument = arg;
        }

        /// <summary>回傳已寫入相關設定訊息之OpcodeData物件</summary>
        /// <param name="docLine">原始文件列號</param>
        /// <param name="docText">原始文件文字</param>
        /// <param name="code">Opcode 編號</param>
        /// <param name="idx">Index</param>
        /// <param name="cmt">註解</param>
        /// <param name="arg">參數集合</param>
        /// <returns>已設定好之OpcodeData物件</returns>
        public OpcodeData SetOpcode(int docLine, string docText, int code, int idx, string cmt, List<string> arg) {
            OpcodeData opcTemp = new OpcodeData();

            opcTemp.DocumentLine = docLine;
            opcTemp.DocumentText = docText;
            opcTemp.Opcode = code;
            opcTemp.Index = idx;
            opcTemp.Comment = cmt;
            opcTemp.Argument = arg;

            return opcTemp;
        }

        /// <summary>載入 OPCODE_PATH 之檔案至記憶體中(mOpcCollect)</summary>
        public void LoadOpcode() {
            LoadOpcode(mPath, out mOpcCollect);
        }

        /// <summary>載入指定路徑的Opcode檔案至記憶體中(mOpcCollect)</summary>
        /// <param name="path">Opcode 檔案路徑，如 @"D:\CASTEC\Config\CASTEC.opc"</param>
        public void LoadOpcode(string path) {
            /*-- 回存指定路徑 --*/
            mPath = path;

            /*-- 載入之 --*/
            LoadOpcode(path, out mOpcCollect);
        }

        /// <summary>載入 OPCODE_PATH 之檔案，並丟出該OpcodeData集合</summary>
        /// <param name="opcode">讀取完畢之OpcodeData集合</param>
        /// <param name="memory">是否將解析完之資料多存一份在mOpcCollect裡? (True)儲存，回傳集合並加入mOpcCollect   (False)不儲存，僅回傳物件</param>
        public void LoadOpcode(out List<OpcodeData> opcode, bool memory = true) {
            List<OpcodeData> opc;

            /*-- 利用已建立Function直接執行 --*/
            /*-- 但是回傳物件因不確定若直接把opcode丟進去會不會造成記憶體使用不良，故仍另外宣告，完成後再另外丟出 --*/
            LoadOpcode(mPath, out opc);
            if (memory) mOpcCollect = opc.ToList();

            opcode = opc;
        }

        /// <summary>載入 OPCODE_PATH 之檔案，並丟出該OpcodeData集合</summary>
        /// <param name="path">Opcode 檔案路徑，如 @"D:\CASTEC\Config\CASTEC.opc"</param>
        /// <param name="opcode">讀取完畢之OpcodeData集合</param>
        /// <param name="memory">是否將解析完之資料多存一份在mOpcCollect裡? (True)儲存，回傳集合並加入mOpcCollect   (False)不儲存，僅回傳物件</param>
        public void LoadOpcode(string path, out List<OpcodeData> opcode, bool memory = true) {
            List<OpcodeData> opc;

            /*-- 利用已建立Function直接執行 --*/
            /*-- 但是回傳物件因不確定若直接把opcode丟進去會不會造成記憶體使用不良，故仍另外宣告，完成後再另外丟出 --*/
            LoadOpcode(path, out opc);
            if (memory) {
                mPath = path;
                mOpcCollect = opc.ToList();
            }

            opcode = opc;
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

        /// <summary>新增或更新 OpcodeData，並寫入 OPCODE_PATH 檔案</summary>
        /// <param name="opc">欲儲存之 OpcodeData 物件</param>
        public void SaveOpcode(OpcodeData opc) {
            /*-- 利用已建立之Function達成 --*/
            SaveOpcode(mPath, opc);
        }

        /// <summary>新增或更新 Opcode 相關資訊至 OPCODE_PATH 檔案</summary>
        /// <param name="opcode">Opcode 編號</param>
        /// <param name="index">Opcode 索引</param>
        /// <param name="comment">註解</param>
        /// <param name="arg">參數集合</param>
        public void SaveOpcode(int opcode, int index, string comment, params string[] arg) {
            /*-- 利用現有Function達成 --*/
            SaveOpcode(mPath, new OpcodeData(opcode, index, comment, arg.ToList()));
        }

        /// <summary>利用舊有 mOpcCollect 更新參數 Opcode 至 OPCODE_PATH 檔案</summary>
        /// <param name="opcode">Opcode 編號</param>
        /// <param name="arg">參數集合</param>
        public void SaveOpcode(int opcode, params string[] arg) {
            OpcodeData opc = FindOpcodeData(opcode);

            if (opc != null) {
                opc.Argument.Clear();
                opc.Argument = arg.ToList();
                SaveOpcode(mPath, opc);
            }
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
            int intLineNum = -1, intColIdx = -1;
            string strMsg = "";
            List<string> strDocument = new List<string>();

            /*-- 載入Opcode檔案 --*/
            mPath = path;
            strDocument = CtFile.ReadFile(path);

            /*-- 抓取Index(文件列減1)，如果已經有指定(大於 -1)則直接寫入該行，否則用搜尋的 --*/
            intLineNum = (opc.DocumentLine > -1) ? opc.DocumentLine : FindIndex(strDocument, opc.Opcode);

            /*-- 抓取全域Opcode index，並取代資料。如果沒有則會回傳 -1 --*/
            intColIdx = FindIndex(opc.Opcode);

            /*-- 組合Opcode字串 --*/
            strMsg = CombineOpcodeString(opc.Opcode, opc.Index, (((opc.Comment == "") && (intColIdx > -1)) ? mOpcCollect[intColIdx].Comment : opc.Comment), opc.Argument);

            /*-- 覆蓋文字檔 --*/
            if ((intLineNum < 0) && mOpcCollect.Count > 0) {
                strDocument.Insert(FindInsertIndex(strDocument, opc.Opcode), strMsg);
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
                if (opc.Comment != "")
                    mOpcCollect[intColIdx].Comment = opc.Comment;
            }

            /*-- 寫入檔案 --*/
            CtFile.WriteFile(path, strDocument);
        }

        /// <summary>更新 mOpcCollect 。此副程式不會寫入檔案！</summary>
        /// <param name="opcode">Opcode 編號</param>
        /// <param name="index">Opcode 索引</param>
        /// <param name="comment">註解</param>
        /// <param name="arg">參數集合</param>
        public void UpdatemOpcCollect(int opcode, int index, string comment, List<string> arg) {
            /*-- 利用現有Function達成此功能 --*/
            UpdatemOpcCollect(new OpcodeData(opcode, index, comment, arg));
        }

        /// <summary>裡用 OpcodeData 更新至 mOpcCollect 裡</summary>
        /// <param name="opc">欲儲存之 OpcodeData</param>
        public void UpdatemOpcCollect(OpcodeData opc) {
            int intColIdx = -1;

            /*-- 抓取全域Opcode index，並取代資料。如果沒有則會回傳 -1 --*/
            intColIdx = FindIndex(opc.Opcode);

            /*-- 覆蓋全域Opcode，但如果之前選擇不儲存至全域，那就跳過 --*/
            if ((intColIdx < 0) && (mOpcCollect.Count > 0)) {
                mOpcCollect.Add(opc);
                intColIdx = mOpcCollect.Count - 1;
            } else if (intColIdx > -1) {
                mOpcCollect[intColIdx].Index = opc.Index;
                mOpcCollect[intColIdx].Argument = opc.Argument;
                if (opc.Comment != "")
                    mOpcCollect[intColIdx].Comment = opc.Comment;
                if (opc.DocumentLine > 0)
                    mOpcCollect[intColIdx].DocumentLine = opc.DocumentLine;
                if (opc.DocumentText != "")
                    mOpcCollect[intColIdx].DocumentText = opc.DocumentText;
            }
        }

        #endregion
    }
}
