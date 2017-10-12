using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Ace.AdeptSight.Server;
using Ace.Core.Client;
using Ace.Core.Server;
using Ace.HSVision.Client;
using Ace.HSVision.Client.ImageDisplay;
using Ace.HSVision.Client.Renderers;
using Ace.HSVision.Client.Wizard;
using Ace.HSVision.Server.Control;
using Ace.HSVision.Server.Tools;

using CtLib.Forms;
using CtLib.Library;
using CtLib.Module.Dimmer;
using CtLib.Module.Utility;
using CtLib.Module.XML;

using Ace.Adept.Server.Desktop.Connection;
using Ace.HSVision.Server.Parameters;
using CtLib.Module.Adept.Extension;
using Ace.Adept.Server.Motion.Robots;
namespace CtLib.Module.Adept {

    /// <summary>
    /// 測試用影像建置器核心 by jay 2017/08/17
    /// </summary>
    /// <remarks>
    /// 用於產生CVT程式碼時增加NgReason應用
    /// </remarks>
    public class CtAceVisionBuilder : CtAceVisionBuilderOri {

        #region Funciton - Constructors

        /// <summary>建構影像程序編輯器</summary>
        /// <param name="ace">已連線的 ACE 相關物件</param>
        /// <param name="hndl">主視窗之 Handle，用於 <see cref="CtMsgBox"/> 之顯示視窗使用</param>
        /// <param name="dispCtrl">欲擺放 Vision Window 之控制項</param>
        public CtAceVisionBuilder(CtAce ace, IntPtr hndl, Control dispCtrl) : base(ace, hndl, dispCtrl) {
        }

        #endregion Function - Constructors

        #region Function - Private Methods

        /// <summary>加入 CVT Main 程式裡的 <see cref="IVisionToolBase.Execute(bool)"/> 、flags 與相關變數設定</summary>
        /// <param name="cvt"><see cref="ICSharpCustomTool"/> 變數名稱</param>
        /// <param name="clrNg"><see cref="IVisionTool.ResultsAvailable"/> = false 時所畫的 ROI 顏色</param>
        /// <param name="exec">判斷 <see cref="IVisionToolBase.Execute(bool)"/> 之變數</param>
        /// <param name="exit">判斷是否有任一 <see cref="NoResultAction.EXIT_SCRIPT"/> 觸發之變數，觸發後後續則不執行</param>
        /// <param name="pass">判斷 NG 或 PASS 之變數</param>
        protected override void AddVisionExecution(string cvt, string clrNg, string exec, string exit, string pass) {
            base.AddVisionExecution(cvt, clrNg, exec, exit, pass);
            mMainScript.Add("Trace.WriteLine(VisionExecutor.NgReason);");
        }

        #endregion Function - Private Methods

    }

    /// <summary>影像建置器核心，提供 CVT 建置、工具與影像顯示核心</summary>
    public class CtAceVisionBuilderOri : IDisposable {

		#region Version

		/// <summary>CtAceVisionBuilder 版本訊息</summary>
		/// <remarks><code>
		/// 
		///		0.0.0	Ahern	[2016/05/25]
		///			+ 從 CtAceVisionBuilder_Ctrl 分離
		///     
		///		0.0.1	Ahern	[2016/05/30]
		///			+ IXmlSavable
		///			+ Edge Locator
		///			+ Line Finder
		///			
		///		0.0.2	Ahern	[2016/05/31]
		///			+ Image Processing
		///			+ Arc Finder
		///			\ IVisionTool 改以 IVisionToolBase 取代
		///			
		///		0.0.3	Ahern	[2016/06/01]
		///			+ Point Finder
		///			\ DrawResult、DrawRoi 之 MarkerColor
		///			
		///		0.1.0	Ahern	[2016/06/16]
		///			+ CtEmbdResx
		///			\ 相關提示文字以多語系顯示
		///			
		///		0.1.1	Ahern	[2016/06/24]
		///			+ DistanceJudge 加上 NG 判斷
		///			
		///		0.1.2	Ahern	[2016/06/27]
		///			+ ThetaJudge
		///			
		///		0.1.3	Ahern	[2016/06/28]
		///			\ ResultAverage 可用 ThetaJudge
		///			
		///		0.1.4	Ahern	[2016/07/18]
		///			+ 各 Tool 之曝光值與增益值
		///			
		///		0.1.5	Ahern	[2016/07/19]
		///			\ 各 Tool 之修改數值範圍提示
		///			
		///		0.1.6	Ahern	[2016/07/28]
		///			+ 預先切換調光器模式
		///			
		///		0.1.7	Ahern	[2016/07/29]
		///			+ ResultTable
		///			
		///		0.1.8	Ahern	[2016/08/01]
		///			\ GetDependentTool() 改抓 .Last()
		///			
		///		0.1.9	Ahern	[2016/08/02]
		///			+ 預先切燈時也切曝光值
		///			\ Sequence 若為 -1 則不產生物件
		///			
		///		0.2.0	Ahern	[2016/08/12]
		///			\ 輸出 ACE 資料夾時改用移動，避免 Relative 不見
		///			\ 輸出 ACE 資料夾前的清除舊有資料夾名稱修正，避免把其他東西也刪了
		///			\ 清除 AceObject 時確實清除並 null
		///			\ Tool 執行後的 NG 判斷加上 BeginDrawShape
		///			
		///		0.3.0	Ahern	[2016/08/31]
		///			+ Show Results Graphics
		///			+ Run/Continuous/Stop
		///			- DrawRoi Exception，找不到時不要畫就好
		///			
		///		0.3.1	Ahern	[2016/09/06]
		///			+ 顯示結果(含欄位選擇)
		///			\ Blob 現可以依照 Segmentation 決定 Threshold 組數
		///			+ mVisPainted 抓取 mImgDisp.Paint 事件，表示 Control 已進入繪製階段
		///			
		///		0.3.2	Ahern	[2016/09/12]
		///			+ ThetaWhirling、IDynamicAngle
		///			\ GenerateCVT 後，補上所有工具的 IsCompiled = true
		///			\ ExportVisionPorject 後，補上 mVisCvt 之 IsModified = false
		///			\ ResultAverage、ResultTable 加上 IDynamicAngle 判斷
		///			
		///		0.3.3	Ahern	[2016/09/13]
		///			\ mVisPainted 改為抓取 IVisionPlugin 之事件
		///			\ Average 平均數值改除以多少個 VisionTransform 而非 Tool 數量
		///			\ 新增工具後 mTreNodSelect 會正確指向新增的 Tool
		///			
		///		0.4.0	Ahern	[2016/09/23]
		///			\ PropertyModified 新增判斷 AceBoolEventArg 加以讓畫面重拍
		///			\ Model 現不直接開啟精靈，改由 ToolStrip 手動開啟
		///			\ InitialRoot 補上 ModelColl.Clear()
		///			\ RemoveTool 時額外檢查 Model，並從 ModelColl 移除
		///			\ RemoveTool 時如果有其他 Relative 到此工具，一併移除
		///			\ Sequence 現改為 0 不產生，並修正註解
		///			\ ImageProcessing 之 Threshold 顯示時機
		///			\ Locator 於建立時強制寫入預設的 ConformityTolerance
		///			\ Locator 之 InstanceCount 範圍為 1 ~ 2000
		///			\ Locator 之 ScaleFactor 範圍為 0.1 ~ 10
		///			\ Exposure 與 Gain 現在可正確設定至 -1
		///			\ CtCameraParam 修正如果 ImageSource 是 ImageProcessing 會造成誤判
		///			\ 匯至 ACE 時否、取消按鈕與對應動作
		///			\ VisionToolBase 添加 IsGeneralProperty 與 IsGeneralValue 以讓各 Tool 共用
		///			\ 補上判斷 Tool 的 IsCompiled
		///			\ 如滑鼠選擇 CVT，顯示 Compile 選項
		///			- ImageProcessing 現不給切 Dimmer/Exposure/Gain，由 Tool 自行設定後再由 Relative 執行
		///			+ CVT 現可由 V+ 變數決定是否儲存 OK 圖
		///			
		///		0.4.1	Ahern	[2016/09/26]
		///			\ 修正調光器設定離開按鈕動作
		///			\ 修正調光器彈跳視窗內容多語系
		///			\ ThetaWhirling 產生程式碼修改，會搭配 mTool 與 AsRetuener 去切換
		///			\ Model 開啟時不要 Execute
		///			+ 區分並顯示 Model Renderer
		///			
		///		0.5.0	Ahern	[2016/09/30]
		///			\ 重寫 CVT，取消 Relative
		///			\ 修正載入 PointFinder 問題
		///			\ 修正複寫 Result 時 TreeNode 沒有刪除的問題
		///			\ 沒有工具時 CVT 回傳 null
		///			+ ICvtExecutor 用於 CVT 內執行動作，如是 Vision Tool 會取消其 InputLink 並用乘的方式計算 Offset
		///			+ 每個 Tool 都具有 ID，用於載入 CTVP 時尋找參考
		///			+ 儲存 CTVP 時紀錄 Node 之 Level 與 Index，並於載入後使用之進行排序
		///			+ IVisionToolPack 會紀錄 InputLink 相關資訊，並於編輯前檢查是否需要補上 Relative
		/// 
		///		0.5.1	Ahern	[2016/10/04]
		///			\ 於編輯前進行切燈切參數
		///			\ 修改 InputLinkPack 參考時機，避免參考到 CVT
		///			\ 新增儲存 InputLinkID 以於載入 CTVP 時方便塞 Node
		///			+ 新增 Return ROI Center 相關方法
		///			
		///		0.6.0	Ahern	[2016/10/07]
		///			\ 修正 AddJudgement 尚有 flag 錯誤問題
		///			\ Judgement 添加各項目註解，比較不會混在一起
		///			\ 修正切攝影機時的安全轉型錯誤問題
		///			+ 常用的 CVT 變數名稱拉至全域
		///			+ 工具複製
		///			
		///		0.6.1	Ahern	[2016/10/13]
		///			\ 修正判斷 PreLight 時有可能為 Null 情況
		///			\ ImageSourceEditor 改以搜尋 ACE CCD 與專案內 ImageSource
		///			+ DimmerChannel 添加註解選項
		///			
		///		0.6.2	Ahern	[2016/10/17]
		///			\ 修正 IVisionImageSource 不應取得 Offset 問題
		///			\ VisionExecutor 執行後將 rePic = false
		///			\ VisionExecutor 如是 IVisionImageSource 則僅將 rePic = true
		///			\ RemoveTool 因應 Relative 可能因為執行後沒有還原，改以 TreeNode 直接刪
		///			
		///		0.6.3	Ahern	[2016/10/18]
		///			\ IArcRoiTool.SearchRegion 需直接取代不可用 .Center.X 方式更改
		///			\ 區分各種 Tool 的 OverlayMarker 顏色，僅 NG 時共用
		///			\ 於 InitialRoot 前檢查是否有一個(含)以上的 IVisionImageSource
		///			\ DistanceJudge、ThetaMeasure 顯示數值至結果中心
		///			- ImageSourceEditor.None
		///			- ThetaWhirling.GenerateFullCode
		///			
		///		0.6.4	Ahern	[2016/10/19]
		///			\ Re-Compile 補上 EnsureRelativeLink
		///			
		///		0.6.5	Ahern	[2016/10/20]
		///			\ 貼上時補上 mInvAct、mRoiCent 與 mCmt
		///			
		///		0.6.6	Ahern	[2016/10/21]
		///			\ Execute 補上 try-catch 以防止 ACE 的 System.OutOfMemory 例外
		///			
		///		0.6.7	Ahern	[2016/10/22]
		///			\ MoveFolder 加上關閉顯示結果
		///			
		///		0.6.8	Ahern	[2016/10/24]
		///			\ VisionClasses 補上 IDisposable
		///			
		///		0.6.9	Ahern	[2016/10/26]
		///			\ Blob Count 開到 int.Maximum
		///			\ CVT 的 Part No. 改為 Component Code
		///			
		///		0.6.10	Ahern	[2016/10/28]
		///			\ MoveFolder 確保 Sequence 的 VisionTool
		///			
		///		0.6.11	Ahern	[2016/10/30]
		///			+ Image Processing 補上 Operand Image
		///			\ Image Processing 修正 Assigment 選項於錯的 Operation 顯示
		///			\ Locator 與 Blob 於建構時使用預設的結果顯示方式
		///			\ 修正 Judgement 後多一段 return null 問題
		///			\ Match Quality 等改以百分比顯示
		/// 
		///		0.6.12	Ahern	[2016/11/09]
		///			\ 取消 mVisPlugin.Dispose
		///
		///		0.6.13	Ahern	[2016/11/15]
		///			\ 修正 Tool 移除後相關參考工具於開啟時的 Null Exception
		///			\ 修正 Tool 移除後，相同工具的變數名稱重複問題，於移除後重新確認變數名稱
		///			
		///		0.6.14	Ahern	[2016/12/06]
		///			+ CVT 新增 Calibration 選項
		///			
		///		0.6.15	Ahern	[2016/12/16]
		///			\ 執行 mRendBase.__EditMode 時改用 InvokeIfNecessary 執行
		///			
		///		0.6.16	Ahern	[2016/12/20]
		///			\ 修正 ArcFinder 之 Min Arc Percent 無法編輯問題
		///			
		///		0.6.17	Ahern	[2016/12/22]
		///			+ 編譯日期、*.ctvp 檔案路徑額外寫入至 INote
		/// 
		///		0.7.0	Ahern	[2017/02/02]
		///			+ mLangMap，一次載入所有的 Language.xml
		///			+ LanguageChanged 以重新載入 Language.xml
		///			\ GetMultiLang 改直接參考 mLangMap
		///			\ IDataGridViewUpdatable 添加 Dictionary 選項，改由其直接讀取語系資訊
		///			
		///		0.8.0	Ahern	[2017/02/11]
		///			\ IDataGridUpdatable 改名為 IPropertable
		///			\ IDataGridResultable 改名為 IResultable
		///			\ OnDgvRowCreated 改名為 OnPropertiesCreated，並改用 PropertyView 發布
		///			\ OnDgvResultUpdated 改名為 OnResultsUpdated
		///			\ Edit、ReCompile 改回此類別處理，不再由 Ctrl 分開處理
		///			\ DisplayTool(原 Ctrl 之 Edit 按鈕) 完成後直接發布屬性欄位及結果表，以利 ProgressBar 顯示與關閉
		///			\ 修正數個 Raise-- 方法名稱，改由簡寫取代
		///			+ OnResultsCreated 事件
		///			+ OnToolModifying 事件
		///			+ Renderer_PropertyModified 觸發 OnToolModifying
		///			+ 屬性顯示部分改由 PropertyView 實作
		///			+ 所有的 Tool 均已設置 PropertyView 之多國語系、編輯動作、是否顯示與權限，但尚未翻譯完畢
		///			+ 可設定之 Enum 或 bool 選項改為多語系，但尚未翻譯完畢
		///			
		///		0.8.1	Ahern	[2017/02/12]
		///			\ VisionToolPack 改採 base 之建構子，取代原本呼叫 NewInstance 方法
		///			
		///		0.8.2	Ahern	[2017/02/13]
		///			\ DisplayTool 若為 Model 則回傳 false
		///			
		///		0.8.3	Ahern	[2017/05/23]
		///			\ 新增 ReRenderer 多載，判斷該物件需重新渲染或僅需刷新屬性
		///			\ Renderer_PropertyModified 僅於有畫面顯示時才顯示 Loading 條
		///			\ InitialVisionDisplay 為 null 時把 mRendBase 清空
		///			\ Locator 之 Max Instance Count 修改後的數值，修正 .ToString() 丟錯人
		///     0.8.4   Jay     [2017/06/16]
        ///         + 加入CalculatedPoint與CalculatedLine工具
        ///         \ 修改部分進度條不停的問題
        ///     0.8.5   Jay     [2017/06/27]
        ///         + 加入Calculated Arc與Calculated Frame工具 
        ///     0.8.6   Jay     [2017/08/30]
        ///         / NG圖資訊增加
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(0, 8, 6, "2017/08/30", "Jay chang"); } }

		#endregion

		/* 
		 * Generate	= 建立相關物件
		 * Create	= 產生並回傳程式碼
		 * Add		= 產生程式碼並直接加入大集合
		 * 
		 */

		#region Declaration - Definitions
		/// <summary>使用於 CVT 內部的區域變數對應表</summary>
		private static readonly Dictionary<string, string> mCvtVarMap = new Dictionary<string, string> {
			{ "CVT", "cvt" },
			{ "IVpLinkedObject", "ctrl" },
			{ "CvtResult", "results" },
			{ "ColorPass", "clrRoiPass" },
			{ "ColorNg", "clrRoiFail" },
			{ "ColorAxis", "clrAxis" },
			{ "ExecFlag", "flag_Exec" },
			{ "ExitFlag", "flag_Exit" },
			{ "PassFlag", "flag_Pass" }
		};
		#endregion

		#region Declaration - Fields
		/// <summary>[Ref] Adept ACE 關聯物件</summary>
		private CtAce rAce;
		/// <summary>[Ref] rAce 所連接的 <see cref="IAceServer"/></summary>
		private IAceServer rIServer;
		/// <summary>[Ref] rAce 所連接的 <see cref="IAceClient"/></summary>
		private IAceClient rIClient;

		/// <summary>主 UI 之 Handle</summary>
		private IntPtr rUiHndl;
		/// <summary>欲顯示影像視窗的控制項</summary>
		private Control rDispCtrl;

		/// <summary>於 <see cref="IAceServer.Root"/> 裡的 Vision Builder 主資料夾</summary>
		private IAceObjectCollection mAceFoldMain;
		/// <summary>於 Vision Builder 主資料夾裡，存放 <see cref="IVisionTool"/> 之資料夾</summary>
		private IAceObjectCollection mAceFoldTool;
		///// <summary>用於產生 CVT Script 程式碼</summary>
		///// <remarks>改用 List&lt;string&gt; 自行製作程式碼</remarks>
		//private CSharpScriptBuilder mCvtBuilder;
		/// <summary>此 Vision Builder 主要輸出的 CVT</summary>
		private ICSharpCustomTool mCvt;
		/// <summary>當前於 <see cref="IAceClient"/> 所建立的影像關聯插件，影像均藉由此插件進行傳導</summary>
		private IVisionPlugin mVisPlugin;
		/// <summary>Adept ACE 渲染基底</summary>
		private VisionToolRendererBase mRendBase;
		/// <summary>影像顯示控制項，由 <see cref="IImageDisplayControl"/> 所轉型</summary>
		private Control mImgDisp;
		/// <summary>CVT 之結果計算工具</summary>
		private IVisionResult mVisResult;
		/// <summary>影像工具包集合</summary>
		private List<IVisionToolPack> mVisToolPackColl = new List<IVisionToolPack>();
        ///<summary>計算工具包集合</summary>
        private List<ICalculatedToolPack> mCalToolPackColl = new List<ICalculatedToolPack>();
		/// <summary>評斷工具集合</summary>
		private List<IVisionJudgement> mVisJudgeColl = new List<IVisionJudgement>();
		/// <summary><see cref="ILocatorModel"/> 集合</summary>
		private List<IVisionToolPack> mVisModelColl = new List<IVisionToolPack>();
		/// <summary><see cref="IPeripheryUtility"/> 集合</summary>
		private List<IPeripheryUtility> mPrphColl = new List<IPeripheryUtility>();
		/// <summary>CVT Main 程式碼</summary>
		protected List<string> mMainScript = new List<string>();
		///// <summary>暫存 <see cref="DataGridView.CellBeginEdit"/> 的數值，如 <seealso cref="DataGridView.CellEndEdit"/> 判定違法數值則使用此數值進行還原</summary>
		///// <remarks>改以 UpdateProperty 供使用者選擇數值，故此物件目前無實際作用</remarks>
		//private object mPreCellVal;
		/// <summary>調光器集合</summary>
		private List<DimmerPack> mDimColl = new List<DimmerPack>();
		/// <summary>CVT 之工具包</summary>
		private IVisionToolPack mVisCvt;

		/// <summary>當前開啟或存檔的 CTVP 名稱，會附加訊息於 <see cref="INote"/> 上</summary>
		private string mCtvpName = string.Empty;
		/// <summary>觸發 <see cref="GenerateScript"/> 之時間點，會附加時間戳記於 <see cref="INote"/> 上</summary>
		private DateTime mGenTime = DateTime.Now;

		/// <summary>[Flag] 當前是否需要顯示 Vision Window。例如，<see cref="IVisionResult"/> 與 <see cref="IVisionJudgement"/> 等或許不須顯示畫面</summary>
		private bool mDisplayVision = true;
		/// <summary>[Flag] 是否已觸發 <see cref="Control.Paint"/> 事件</summary>
		private volatile bool mVisPainted = false;

		/// <summary>Language.xml 之對應表</summary>
		private Dictionary<string, string> mLangMap;
		#endregion

		#region Declaration - Properties
		/// <summary>取得是否有任何工具修改過</summary>
		public bool IsModified {
			get {
				if (mVisCvt != null && mVisCvt.IsModified) return true;
				else if (mVisToolPackColl.Any(pack => pack.IsModified)) return true;
				else if (mVisJudgeColl.Any(judge => judge.IsModified)) return true;
				else if (mVisResult != null && mVisResult.IsModified) return true;
				else if (mPrphColl.Any(prph => prph.IsModified)) return true;
				else return false;
			}
		}

		/// <summary>取得當前工具是否均有編譯</summary>
		public bool IsCompiled {
			get {
				if (mVisCvt != null && !mVisCvt.IsCompiled) return false;
				else if (mVisToolPackColl.Any(pack => !pack.IsCompiled)) return false;
				else if (mVisJudgeColl.Any(judge => !judge.IsCompiled)) return false;
				else if (mVisResult != null && !mVisResult.IsCompiled) return false;
				else if (mPrphColl.Any(prph => !prph.IsCompiled)) return false;
				else return true;
			}
		}

		/// <summary>取得當前是否有需要進行 InputLink (Relative) 的恢復</summary>
		public bool IsInputRecoverRequire {
			get {
				return mVisToolPackColl.Any(pack => pack.RecoverInputRequired);
			}
		}
		#endregion

		#region Declaration - Events
		/// <summary>[委派] 新增 <see cref="TreeNode"/> 節點至 <see cref="TreeView"/></summary>
		/// <param name="main">父節點，若為 null 表示存放於根節點</param>
		/// <param name="node">欲附加的新節點</param>
		public delegate void AssignTreeNode(TreeNode main, TreeNode node);
		/// <summary>[委派] 更換所有 <see cref="DataGridView.Rows"/> 欄位</summary>
		/// <param name="rows">欲更換的資料欄位</param>
		public delegate void PropertiesCreated(List<PropertyView> rows);
		/// <summary>[委派] 提醒需更新當前 <see cref="DataGridView.Rows"/> 欄位數值</summary>
		public delegate void PropertiesUpdated();
		/// <summary>[委派] 關閉影像視窗</summary>
		public delegate void DisableVisionWindow();
		/// <summary>[委派] 執行 <see cref="IVisionToolBase.Execute()"/> 以進行更新</summary>
		public delegate void ExecuteVisionTool();
		/// <summary>[委派] 提醒需更新當前 <see cref="DataGridView.Rows"/> 結果數值</summary>
		public delegate void ResultsUpdated();
		/// <summary>[委派] <see cref="IResultable"/> 之 <see cref="DataTable"/> 已建立</summary>
		/// <param name="dt">新建立之 <see cref="DataTable"/></param>
		public delegate void ResultsCreated(DataTable dt);
		/// <summary>[委派] 由 UI 觸發修改狀態，建置器正在重新抓取相關數值及結果</summary>
		public delegate void ToolModifying();

		/// <summary>新增 <see cref="TreeNode"/> 節點至 <see cref="TreeView"/></summary>
		public event AssignTreeNode OnAssignTreeNode;
		/// <summary>更換所有 <see cref="DataGridView.Rows"/> 欄位</summary>
		public event PropertiesCreated OnPropertiesCreated;
		/// <summary>
		/// 提醒需更新當前 <see cref="DataGridView.Rows"/> 欄位數值
		/// <para>請調用 <see cref="CreateDataGridViewRows(IPropertable)"/> 後重新顯示至 <see cref="DataGridView"/> 上</para>
		/// </summary>
		public event PropertiesUpdated OnPropertiesUpdated;
		/// <summary>關閉影像視窗</summary>
		public event DisableVisionWindow OnDisableVisionWindow;
		/// <summary>執行 <see cref="IVisionToolBase.Execute()"/> 以進行更新</summary>
		public event ExecuteVisionTool OnExecuteVisionTool;
		/// <summary>
		/// 提醒需更新當前 <see cref="DataGridView.Rows"/> 結果數值
		/// <para>請再實作 <see cref="IResultable.CreateDataTable"/></para>
		/// </summary>
		public event ResultsUpdated OnResultsUpdated;
		/// <summary>
		/// <see cref="IResultable"/> 之 <see cref="DataTable"/> 已建立
		/// <para>請將此 <see cref="DataTable"/> 顯示至 <see cref="DataGridView"/> 上</para>
		/// </summary>
		public event ResultsCreated OnResultsCreated;
		/// <summary>由 UI 觸發修改狀態，建置器正在重新抓取相關數值及結果</summary>
		public event ToolModifying OnToolModifying;

		/// <summary>觸發新增 <see cref="TreeNode"/> 節點至 <see cref="TreeView"/> 事件</summary>
		/// <param name="main">父節點，若為 null 表示存放於根節點</param>
		/// <param name="node">欲附加的新節點</param>
		protected virtual void RaiseAssignTreeNode(TreeNode main, TreeNode node) {
			if (OnAssignTreeNode != null)
				OnAssignTreeNode(main, node);
		}
		/// <summary>觸發更換所有 <see cref="DataGridView.Rows"/> 欄位事件</summary>
		/// <param name="rows">欲更換的資料欄位</param>
		protected virtual void RaisePropCret(List<PropertyView> rows) {
			if (OnPropertiesCreated != null)
				OnPropertiesCreated.BeginInvoke(rows, null, null);
		}
		/// <summary>觸發提醒需更新當前 <see cref="DataGridView.Rows"/> 欄位數值事件</summary>
		protected virtual void RaisePropUpd() {
			if (OnPropertiesUpdated != null)
				OnPropertiesUpdated.BeginInvoke(null, null);
		}
		/// <summary>觸發關閉影像視窗事件</summary>
		protected virtual void RaiseDisVisWind() {
			if (OnDisableVisionWindow != null)
				OnDisableVisionWindow();
		}
		/// <summary>觸發執行 <see cref="IVisionToolBase.Execute()"/> 以進行更新事件</summary>
		protected virtual void RaiseExecVisTool() {
			if (OnExecuteVisionTool != null)
				OnExecuteVisionTool();
		}
		/// <summary>觸發提醒需更新當前 <see cref="DataGridView.Rows"/> 結果數值事件</summary>
		protected virtual void RaiseResultUpd() {
			if (OnResultsUpdated != null)
				OnResultsUpdated.Invoke();
		}
		/// <summary>觸發更換當前 <see cref="DataGridView.Rows"/> 結果數值事件</summary>
		protected virtual void RaiseResultCret(DataTable dt) {
			if (OnResultsCreated != null)
				OnResultsCreated.BeginInvoke(dt, null, null);
		}
		/// <summary>觸發建置器正在重新抓取相關數值及結果</summary>
		protected virtual void RaiseModify() {
			if (OnToolModifying != null)
				OnToolModifying.BeginInvoke(null, null);
		}
		#endregion

		#region Function - Constructors
		/// <summary>建構影像程序編輯器</summary>
		/// <param name="ace">已連線的 ACE 相關物件</param>
		/// <param name="hndl">主視窗之 Handle，用於 <see cref="CtMsgBox"/> 之顯示視窗使用</param>
		/// <param name="dispCtrl">欲擺放 Vision Window 之控制項</param>
		public CtAceVisionBuilderOri(CtAce ace, IntPtr hndl, Control dispCtrl) {
			rAce = ace;
			rIClient = ace.GetClient();
			rIServer = ace.GetServer();

			rUiHndl = hndl;
			rDispCtrl = dispCtrl;

			mLangMap = CtLanguage.GetAllLangXmlText<string>("Language.xml");
		}
		#endregion

		#region Function - IDisposable Implements
		/// <summary>指出是否已觸發過 <see cref="Dispose()"/></summary>
		private bool mDisposed = false;

		/// <summary>釋放資源</summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>釋放資源內容</summary>
		/// <param name="disposing">是否由觸發釋放</param>
		private void Dispose(bool disposing) {
			if (!mDisposed && disposing) {
				mDisposed = true;
				rUiHndl = IntPtr.Zero;
				mAceFoldMain = null;
				mAceFoldTool = null;
				mCvt = null;

				/*-- 若 ACE 已經關閉，則這些 proxy 物件會找不到 server 通知關閉訊息而跳錯誤 --*/
				try {
					//mVisPlugin?.Dispose();	//因為 Vision Plugin 別人還要用，甚至是下次重開要用，所以不能 Dispose
					mRendBase?.Dispose();
					mImgDisp?.Dispose();
				} catch (Exception) {
					/* Dispose 的錯誤就不理他囉 */
				}

				mVisCvt?.Dispose();
				mVisCvt = null;

				mVisJudgeColl.ForEach(pack => pack.Dispose());
				mVisJudgeColl.Clear();
				mVisJudgeColl = null;

				mVisModelColl.ForEach(pack => pack.Dispose());
				mVisModelColl.Clear();
				mVisModelColl = null;

				mVisResult?.Dispose();
				mVisResult = null;

				mVisToolPackColl.ForEach(pack => pack.Dispose());
				mVisToolPackColl.Clear();
				mVisToolPackColl = null;

				mPrphColl.ForEach(pack => pack.Dispose());
				mPrphColl.Clear();
				mPrphColl = null;

				mMainScript.Clear();
				mMainScript = null;

				mDimColl.Clear();
				mDimColl = null;
			}
		}

		/// <summary>解構子</summary>
		~CtAceVisionBuilderOri() {
			Console.WriteLine("Dispose : CtAceVisionBuilder");
			Dispose(true);
		}
		#endregion

		#region Function - Ultility
		/// <summary>判斷 XML 節點是否為 <see cref="IVisionToolPack"/></summary>
		/// <param name="xmlData">欲判斷的 XML 節點</param>
		/// <returns>(<see langword="true"/>)影像工具 (<see langword="false"/>)其他節點</returns>
		private bool IsVisionToolPack(XmlElmt xmlData) {
			bool result = false;
			if (xmlData.HasAttribute) {
				string toolType = xmlData.Attribute("Type")?.Value;
				switch (toolType) {
					case "ICSharpCustomTool":
					case "ILocatorTool":
					case "ILocatorModel":
					case "IBlobAnalyzerTool":
					case "ILineFinderTool":
					case "IEdgeLocatorTool":
					case "IImageProcessingTool":
					case "IArcFinderTool":
					case "IPointFinderTool":
                    case "ICalculatedPointTool":
                    case "ICalculatedLineTool":
                    case "ICalculatedArcTool":
                    case "ICalculatedFrameTool":
						result = true;
						break;
					default:
						result = false;
						break;
				}
			}
			return result;
		}

		/// <summary>判斷 XML 節點是否為 <see cref="IVisionJudgement"/></summary>
		/// <param name="xmlData">欲判斷的 XML 節點</param>
		/// <returns>(<see langword="true"/>)評斷工具 (<see langword="false"/>)其他節點</returns>
		private bool IsVisionJudgement(XmlElmt xmlData) {
			bool result = false;
			if (xmlData.HasAttribute) {
				string toolType = xmlData.Attribute("Type").Value;
				switch (toolType) {
					case "DistanceJudge":
					case "ThetaJudge":
					case "ThetaWhirling":
						result = true;
						break;
					default:
						break;
				}
			}
			return result;
		}

		/// <summary>判斷 XML 節點是否為 <see cref="IVisionResult"/></summary>
		/// <param name="xmlData">欲判斷的 XML 節點</param>
		/// <returns>(<see langword="true"/>)結果工具 (<see langword="false"/>)其他節點</returns>
		private bool IsVisionResult(XmlElmt xmlData) {
			bool result = false;
			if (xmlData.HasAttribute) {
				string toolType = xmlData.Attribute("Type").Value;
				switch (toolType) {
					case "ResultAverage":
					case "ResultTable":
						result = true;
						break;
					default:
						break;
				}
			}
			return result;
		}

		/// <summary>判斷 XML 節點是否為 <see cref="IPeripheryUtility"/></summary>
		/// <param name="xmlData">欲判斷的 XML 節點</param>
		/// <returns>(<see langword="true"/>)周邊工具 (<see langword="false"/>)其他節點</returns>
		private bool IsPeriphery(XmlElmt xmlData) {
			bool result = false;
			if (xmlData.HasAttribute) {
				string toolType = xmlData.Attribute("Type").Value;
				switch (toolType) {
					case "LightCtrl":
					case "CameraPara":
						result = true;
						break;
					default:
						break;
				}
			}
			return result;
		}

		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		/// <remarks>目前均沒有判斷 mLangMap.ContainsKey()，因為正常來說欄位都要先檢查過才會 Release</remarks>
		public string GetMultiLangText(string key) {
			return mLangMap[key];
		}

		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="keys">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		/// <remarks>目前均沒有判斷 mLangMap.ContainsKey()，因為正常來說欄位都要先檢查過才會 Release</remarks>
		public Dictionary<string, string> GetMultiLangText(params string[] keys) {
			return keys
				.ToDictionary(
					key => key,
					key => mLangMap[key]
				);
		}

		/// <summary>刪除並釋放 Omron|Adept ACE 相關 <see cref="IAceObject"/> 物件</summary>
		/// <param name="aceObj">欲釋放之物件</param>
		private void DisposeObject(IAceObject aceObj) {
			IAceObjectCollection fold = aceObj as IAceObjectCollection;
			if (fold != null) { //資料夾，遞迴去釋放每個物件
				IAceObject[] objs = fold.ToArray();
				foreach (IAceObject item in objs) { //遞迴
					DisposeObject(item);
				}

				//子物件都釋放完後，換自己釋放掉
				IAceObjectCollection parFold = fold.ParentCollection;
				if (parFold != null) parFold.Remove(fold);
				fold.Dispose();
				if (parFold != null) parFold.CheckInternalReferences();

			} else {    //物件，直接釋放
				fold = aceObj.ParentCollection;
				if (fold != null) fold.Remove(aceObj);
				aceObj.Dispose();
				if (fold != null) fold.CheckInternalReferences();
			}
		}

		/// <summary>觸發 <see cref="IVisionToolBase.Execute(bool)"/> 並等待 <see cref="IImageDisplayControl"/> 畫完畫面</summary>
		/// <remarks>mVisPainted 是抓 <see cref="IVisionPlugin"/> 的 State == Idle 事件，尚未釐清是否真的是畫完... 但目前看起來很接近了 XD</remarks>
		private void WaitVisionWindowIdle(IVisionToolBase tool, bool exec) {
			/* Model 如果執行，Model 模型會不見... */
			if (tool is ILocatorModel) return;
			/* 清除 Flag，等下要等待 Vision Window 畫完 */
			mVisPainted = false;
			/* 拍拍 */
			tool.Execute(exec);
			/* 等 Vision Window */
			CtTimer.WaitTimeout(
				TimeSpan.FromSeconds(3),
				token => {
					while (!token.IsDone) {
						if (mVisPainted) token.WorkDone();
						else {
							Task.Delay(10).Wait();
							Application.DoEvents();
						}
					}
				}
			);
		}

		/// <summary>使用遞迴的方式尋找 <see cref="TreeNode.Nodes"/> 裡所有的 <see cref="IPropertable"/> 物件</summary>
		/// <param name="nodeColl">欲尋找的 <see cref="TreeNode.Nodes"/></param>
		/// <param name="updColl">存放找到的 <see cref="IPropertable"/> 集合</param>
		private void RecursiveNodeTool(TreeNodeCollection nodeColl, List<IPropertable> updColl) {
			foreach (TreeNode node in nodeColl) {
				IPropertable updObj = node.Tag as IPropertable;
				if (updObj != null && !(updObj is LocatorModelPack)) {
					updColl.Add(updObj);
				}

				if (node.Nodes.Count > 0) RecursiveNodeTool(node.Nodes, updColl);
			}
		}

		/// <summary>還原 <see cref="IVisionProjectable"/> 的 <see cref="TreeNode"/> 連結</summary>
		/// <param name="toolColl">欲還原連結的 <see cref="IVisionProjectable"/> 集合</param>
		private void RecoverTreeNode(IEnumerable<IVisionProjectable> toolColl) {
			/* 排序 */
			var sortColl = toolColl.OrderBy(obj => obj.NodeLevel).ThenBy(obj => obj.NodeIndex);

			/* 從小到大，根據儲存的 InputLinkID 去抓去特定的 */
			foreach (var obj in sortColl) {
				if (obj is CustomVisionToolPack) continue;
				if (obj.InputLinkID.HasValue) {
					var tarObj = toolColl.FirstOrDefault(tool => tool.ID == obj.InputLinkID.Value);
					tarObj.Node.Nodes.Add(obj.Node);
				}
			}
		}

		/// <summary>找出此 <see cref="TreeNode"/> 的 <see cref="IVisionToolPack"/> 父層</summary>
		/// <param name="selectedNode">欲檢查 InputLinked 的節點</param>
		/// <returns>含有 <see cref="IVisionToolPack"/> 的父層</returns>
		private TreeNode EnsureTreeNode(TreeNode selectedNode) {
			TreeNode tempNode = selectedNode;
			do {
				if (tempNode.Tag is IVisionToolPack && !(tempNode.Tag is LocatorModelPack)) break;
				else if (tempNode.Parent != null) {
					tempNode = tempNode.Parent;
				} else break;
			} while (true);
			return tempNode;
		}
		#endregion

		#region Function - Object Initializations
		/// <summary>於 <see cref="IAceServer.Root"/> 建立影像編輯器之臨時資料夾</summary>
		public void InitialRoot() {
			/* 先確認有一個以上的 IVisionImageSource */
			if (rIServer.Root.FilterType(typeof(IVisionImageSource)).Count <= 0) {
				string title = GetMultiLangText("NoImgSrcTit");
				string msg = GetMultiLangText("NoImgSrcMsg");
				CtMsgBox.Show(rUiHndl, title, msg, MsgBoxBtn.OK, MsgBoxStyle.Error, -1);
				return;
			}

			/* 將 ToolPack 清空 */
			mVisCvt = null;
			mVisToolPackColl.Clear();
			mVisJudgeColl.Clear();
			mVisModelColl.Clear();
			mPrphColl.Clear();
			mVisResult = null;

			/* 如已存在則先刪除 */
			rIServer.Root.CheckInternalReferences();
			IAceObject obj = rIServer.Root["/VisionBuilder"];
			if (obj != null) {
				CtAceFile.DeleteAceObject(obj);
			}

			/* 建立資料夾 */
			mAceFoldMain = rIServer.Root.AddCollection("VisionBuilder");
			mAceFoldTool = mAceFoldMain.AddCollection("Tools");

			/* 搜尋當前 IAceServer.Root 裡所有的攝影機 */
			IEnumerable<IVisionImageVirtualCamera> cameraColl = GetCamera();

			/* 建立 CVT */
			mVisCvt = VisionToolPackBase.Factory(
				VisionToolType.CustomVisionTool,
				(cameraColl.Count() > 0 ? cameraColl.ElementAt(0) : null),
				mAceFoldMain,
				mLangMap
			);
			mVisCvt.Tag = new object[] { 0, "000000" };
			mCvt = mVisCvt.Tool as ICSharpCustomTool;

			/* 建立 TreeView 的主節點 */
			TreeNode nod = new TreeNode("Main CVT");
			nod.Tag = mVisCvt;
			RaiseAssignTreeNode(null, nod);

			/* TreeNode 塞回去 */
			mVisCvt.AssignTreeNode(nod);

			/* 顯示 CVT */
			InitialVisionDisplay(mVisCvt);
		}

		/// <summary>初始化 GUI 上的影像顯示模組</summary>
		/// <param name="toolPack">欲顯示畫面的影像工具</param>
		public void InitialVisionDisplay(IVisionToolPack toolPack) {
			if (toolPack != null) { //如果有影像來源，顯示之
				mDisplayVision = true;
				if (mVisPlugin == null) GenerateVisionPlugin(); //建立 Vision Plugin
				GenerateDisplayControl(toolPack.Tool);   //建立 IDisplayControl，即 Vision 視窗
				GenerateRenderer(toolPack.Tool);         //取得當前影像來源之渲染，方可顯示綠框等物件
			} else {    //沒有影像來源，將所有的隱藏掉
				mDisplayVision = false;
				RaiseDisVisWind();
				//若當前有 Renderer，把它清掉，避免誤動作
				if (mRendBase != null) {
					mRendBase.Dispose();
					mRendBase = null;
				}
				//rDispCtrl.BeginInvokeIfNecessary(() => rDispCtrl.Controls.Cast<Control>().ForEach(ctrl => ctrl.Dispose()));
			}
		}

		/// <summary>重新執行拍照並啟用綠框</summary>
		/// <param name="toolPack">欲重新拍照的影像工具</param>
		public void RePicture(IVisionToolPack toolPack) {
			if (toolPack != null) {
				mDisplayVision = true;
				GenerateRenderer(toolPack.Tool);
			}
		}
		#endregion

		#region Function - Object Property Operations
		/// <summary>搜尋 <see cref="IAceServer.Root"/> 內所有的攝影機</summary>
		/// <returns>攝影機集合</returns>
		private IEnumerable<IVisionImageVirtualCamera> GetCamera() {
			return rIServer.Root.FilterType(typeof(IVisionImageVirtualCamera)).Cast<IVisionImageVirtualCamera>();
		}
		#endregion

		#region Function - Object Generations
		/// <summary>建立 CVT</summary>
		/// <param name="cvtPack">可供產生 <see cref="ICSharpCustomTool"/> 的 <see cref="CustomVisionToolPack"/> 物件</param>
		public void GenerateCVT(IVisionToolPack cvtPack) {
			List<string> code = new List<string>();
			/* using namespace 區段 */
			code.AddRange(CreateUsing());
			/* CVT 物件宣告區段，沒辦法改... 改了 CVT 就不會有回傳惹 */
			code.Add("namespace Ace.Custom {");
			code.Add("\tpublic class Program {");
			code.Add("\t\tpublic AceServer ace;");
			code.Add("\t\tpublic IEnumerable<VisionTransform> Main() {");
			/* 加入 CVT Main 程式碼，並進行 Tab 位移  */
			code.AddRange(mMainScript.ConvertAll(prog => string.Format("\t\t\t{0}", prog)));
			code.Add("\t\t}");
			code.Add(string.Empty);
			/* 加入輔助用方法 */
			//code.AddRange(CreateFunction());    //已於方法內部進行 \t 編排，改以 Extension 方式
			code.Add("\t}");
			code.Add("}");

			/* 將程式碼寫入 CVT 內 */
			mCvt.Text = string.Join(Environment.NewLine, code);//mCvtBuilder.ScriptText;

			/* 執行 Compile */
			mCvt.Compile();

			/* 檢查並建立 AdeptSight Sequence */
			IAdeptSightSequence seq = mAceFoldMain["Execution Sequence"] as IAdeptSightSequence;
			int seqNo = (int)(cvtPack.Tag as object[])[0];
			if (seqNo > 0) {    //大於 0 表示有要建立 Sequence
				if (seq == null) seq = mAceFoldMain.AddObjectOfType(typeof(IAdeptSightSequence), "Execution Sequence") as IAdeptSightSequence;
				seq.SequenceNumber = seqNo;
				seq.VisionTool = cvtPack.Tool as IVisionTool;

				/* 如果有設定 Calibration，設定之 */
				if (!string.IsNullOrEmpty((cvtPack as CustomVisionToolPack).Calibration)) {
					var caliObj = rAce.FindObject((cvtPack as CustomVisionToolPack).Calibration) as IAdeptSightCameraCalibration;
					seq.DefaultCalibration = caliObj;
				} else seq.DefaultCalibration = null;
			} else if (seq != null) {   //小於等於 0 則不用建立，但如果現在有 Sequence，刪除之!
				mAceFoldMain.Remove(seq);
				seq.Dispose();
			}

			/* 寫入註記 */
			string noteTxt = $"Compile Time\t{mGenTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}\r\nCTVP File\t{(string.IsNullOrEmpty(mCtvpName) ? "(None)" : mCtvpName)}";
			INote note = mAceFoldMain["/Note"] as INote;
			if (note == null) note = mAceFoldMain.AddObjectOfType(typeof(INote), "Note") as INote;
			note.Text = noteTxt;

			/* 告知所有的工具已經 Compile 過了 */
			mVisCvt.IsCompiled = true;
			mVisToolPackColl.ForEach(tool => tool.IsCompiled = true);
			mVisJudgeColl.ForEach(tool => tool.IsCompiled = true);
			mPrphColl.ForEach(prph => prph.IsCompiled = true);
			if (mVisResult != null) mVisResult.IsCompiled = true;

			/* 告知已經有改變，屆時提醒使用者要存檔 */
			mVisCvt.IsModified = true;
		}

		/// <summary>建立影像插件 <see cref="IVisionPlugin"/> 供 GUI 使用</summary>
		private void GenerateVisionPlugin() {
			/*-- 建立渲染，抓取 Tool 執行結果並開啟編輯模式(需重複開啟) --*/
			mVisPlugin = rIClient.ClientPropertyManager[typeof(IVisionPlugin).Name] as IVisionPlugin;
			/*-- 添加事件，擷取 Vision 是否已經顯示完(Idle = 沒事 = 顯示完了)，但不代表畫面畫完囉! 但是是最接近的了... --*/
			mVisPlugin.ServerStateChanged += (sender, e) => mVisPainted = e.NewStatus == VisionServerState.Idle;
		}

		/// <summary>建立影像顯示控制項</summary>
		/// <param name="tool">要顯示的影像來源</param>
		private void GenerateDisplayControl(IVisionToolBase tool) {
			/*-- 如果已經有東西，先將現有的殺掉 --*/
			if (mImgDisp != null) {
				RaiseDisVisWind();
				mImgDisp.InvokeIfNecessary(
					() => {
						mImgDisp.Dispose();
						mImgDisp = null;
					}
				);
			}

			/*-- 在 IAceClient 建立 DisplayControl --*/
			IImageDisplayControl imgCtrl = rIClient.CreateObject(typeof(IImageDisplayControl)) as IImageDisplayControl;

			/*-- 嘗試取得 ImageSource --*/
			IVisionImageSource src = tool as IVisionImageSource;
			if (src == null) src = tool.GetDependentTools()?.Last() as IVisionImageSource;

			/*-- 設定 DisplayControl --*/
			imgCtrl.Client = rIClient;
			imgCtrl.Buffer = src?.Buffer;
			imgCtrl.AutomaticRendering = true;
			imgCtrl.RulersVisible = true;
			imgCtrl.ScrollBarsVisible = true;
			imgCtrl.ToolBarVisible = true;
			imgCtrl.StatusBarVisible = true;
			imgCtrl.ExecutionTimeVisible = true;
			imgCtrl.AutoClearGraphics = true;

			/*-- 轉為 Windows 之 Control --*/
			mImgDisp = imgCtrl as Control;
			mImgDisp.Name = "Vision";
			mImgDisp.Parent = rDispCtrl;
			mImgDisp.Dock = DockStyle.Fill;
			mImgDisp.Visible = false;   //直接下 Show() 沒有反應...
			mImgDisp.Visible = true;
		}

        private static string NgSave( IiCobra iCobra, ICSharpCustomTool CVT, ICSharpCustomTool NgPictureName, int low_ng_count) {
            string ProductName = "/test1_B_000000_0";//形體名稱
            string ng_picture_path = iCobra.Link.ListS("$ng_pho_dir_path") + ProductName + "/PointFinder1";
            string now_DataTime = DateTime.Now.ToString("yyyyMMdd-HHmmss-");//日期
            
            iCobra.Link.SetS("$ng_picture_path", ng_picture_path);
            NgPictureName.Execute(false);
            CtDrawOverlayMarkers.BeginDrawShape(CVT, ng_picture_path + "/" + now_DataTime + low_ng_count + ".jpg");
            return ng_picture_path;
        }

        //public void GetCVTs(IAceObjectCollection coll ,ref List<string> paths) {
        //    IEnumerable<IAceObject> ary = coll.ToArray();
        //    IEnumerable<ICSharpCustomTool> cvt = ary.Where(v => v is ICSharpCustomTool).Select(t => t as ICSharpCustomTool);
        //    IEnumerable<IAceObjectCollection> sub = ary.Where(v => v is IAceObjectCollection).Select(t => t as IAceObjectCollection);
        //    if (cvt.Any()) foreach (var v in cvt) paths.Add(v.ParentCollection.FullPath);
        //    if (sub.Any()) foreach (var v in sub) GetCVTs(v, ref paths);
        //}
        
        /// <summary>建立影像控制項的渲染，即顯示綠框、Overlay 等</summary>
        /// <param name="tool">要顯示的影像工具</param>
        private void GenerateRenderer(IVisionToolBase tool) {
			/*-- 如果現在有渲染，先把事件和畫在上面的刪除 --*/
			if (mRendBase != null) {
				mRendBase.PropertyModified -= Renderer_PropertyModified;
				mRendBase.ClearToolExecutionGraphics();
				mRendBase.Dispose();
				mRendBase = null;
			}

			/*-- 建立渲染 --*/
			mRendBase = VisionToolRendererBase.CreateRenderer(mVisPlugin, tool, mImgDisp as IImageDisplayControl);
			if (mRendBase != null) {
				mRendBase.PropertyModified += Renderer_PropertyModified;
			}

			try {
				/*-- 重拍一下，讓畫面顯示出來 --*/
				WaitVisionWindowIdle(tool, true);
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER3_ACE, ex, true);
			}

			/*-- 如果渲染沒問題，嘗試開啟綠框(有可能會壞掉!像是) --*/
			if (mRendBase != null) {
				try {
					/*-- 綠框需要重新 OFF → ON 才行 --*/
					mImgDisp.InvokeIfNecessary(
						() => mRendBase.DisableEditMode()
					);

					/*-- 如果是 Model... 則畫 Model 圖案，且不給使用者拉綠框以免出事 @_@ --*/
					if (mRendBase is ILocatorModelRenderer) {
						ILocatorModelRenderer modelRend = mRendBase as ILocatorModelRenderer;
						modelRend.DirectRender(10); //不知道裡面參數有啥差... 改了都沒反應
					}

					/*-- 讓使用者可以去拉綠框 --*/
					mImgDisp.InvokeIfNecessary(
						() => mRendBase.EnableEditMode(mImgDisp, true)
					);
				} catch (Exception ex) {
					Console.Write(ex.ToString());
				}
			}
		}

		/// <summary>建立影像工具包，並自動進入編輯此工具</summary>
		/// <param name="toolType">欲建立的影像工具類型</param>
		/// <param name="dgv">存放可編輯的 <see cref="DataGridView"/> 控制項</param>
		/// <param name="treNod">欲存放此節點的樹狀父節點</param>
		/// <returns>影像工具包</returns>
		public IVisionToolPack GenerateTool(VisionToolType toolType, DataGridView dgv, TreeNode treNod) {
			TreeNode tarNode = EnsureTreeNode(treNod);
			IVisionToolPack pack = VisionToolPackBase.Factory(
				mVisToolPackColl,
				toolType,
				mAceFoldTool,
				tarNode,
				mLangMap
			);
			if (pack != null) {
				/*-- 先加集合 --*/
				mVisToolPackColl.Add(pack);
				/*-- 顯示 Vision Window --*/
				InitialVisionDisplay(pack);
				/*-- 觸發需要顯示屬性頁面 --*/
				RaisePropCret(pack.CreateDataSource(mLangMap));
				/*-- 展開樹狀節點 --*/
				if (!treNod.IsExpanded) treNod.Expand();
				/*-- 告知需要 Compiler --*/
				if (mVisCvt != null) mVisCvt.IsCompiled = false;
			}
			return pack;
		}

		/// <summary>建立 <see cref="ILocatorModel"/> 的影像工具包</summary>
		/// <param name="locatorPack">此 <see cref="ILocatorModel"/> 要依附的 <see cref="ILocatorTool"/> 工具包</param>
		/// <returns>Model 專用的影像工具包</returns>
		public IVisionToolPack GenerateLocatorModel(IVisionToolPack locatorPack) {
			IVisionToolPack modelPack = null;
			if (locatorPack != null) {
				modelPack = VisionToolPackBase.Factory((locatorPack.Tool as IVisionTool).ImageSource, locatorPack.AceFold, locatorPack.Node);
				(locatorPack.Tool as ILocatorTool).AddModel(modelPack.Tool as ILocatorModel);
				mVisModelColl.Add(modelPack);
			}
			return modelPack;
		}

		/// <summary>建立影像結果回傳工具包</summary>
		/// <param name="type">要建立的影像結果回傳工具樣式</param>
		/// <param name="mainNod">欲存放此樹狀節點的父節點</param>
		/// <returns>影像結果回傳工具</returns>
		public IVisionResult GenerateVisionResult(VisionResultType type, TreeNode mainNod) {
			MsgBoxBtn btn = MsgBoxBtn.Yes;

			/*-- 詢問 --*/
			if (mVisResult != null) {
				string title = GetMultiLangText("RetExistTit").Replace(@"\r\n", "\r\n");
				string msg = GetMultiLangText("RetExistMsg").Replace(@"\r\n", "\r\n");
				btn = CtMsgBox.Show(
						rUiHndl,
						title,
						msg,
						MsgBoxBtn.YesNo,
						MsgBoxStyle.Question,
						-1
					);
			}

			if (btn == MsgBoxBtn.Yes) {
				/*-- 如果現在有東西，也同意取代，那就刪了現在的吧! --*/
				if (mVisResult != null) {
					mVisResult.Node.Parent.Nodes.Remove(mVisResult.Node);
					mVisResult = null;
				}

				/*-- 確保目標 Target 不是 Model --*/
				TreeNode tarNode = EnsureTreeNode(mainNod);

				/*-- 依照類型建立之 --*/
				switch (type) {
					case VisionResultType.Average:
						mVisResult = new ResultAverage(tarNode);
						break;
					case VisionResultType.TableSlot:
						mVisResult = new ResultTable(tarNode);
						break;
					default:
						throw new InvalidEnumArgumentException("VisionResultType", (int)type, typeof(VisionResultType));
				}
				if (mVisCvt != null) mVisCvt.IsCompiled = false;
				if (mVisResult != null && !mainNod.IsExpanded) mainNod.Expand();
			}
			return mVisResult;
		}

		/// <summary>建立影像評斷工具包</summary>
		/// <param name="type">要建立的評斷工具樣式</param>
		/// <param name="mainNod">欲存放此樹狀節點的父節點</param>
		/// <returns>影像評斷工具</returns>
		public IVisionJudgement GenerateVisionJudge(VisionJudgeType type, TreeNode mainNod) {
			IVisionJudgement tool = null;

			/*-- 確保目標 Target 不是 Model --*/
			TreeNode tarNode = EnsureTreeNode(mainNod);

			switch (type) {
				case VisionJudgeType.Distance:
					tool = new DistanceJudge(tarNode);
					break;
				case VisionJudgeType.AngleMeasure:
					tool = new ThetaJudge(tarNode);
					break;
				case VisionJudgeType.AngleWhirling:
					tool = new ThetaWhirling(tarNode);
					break;
				default:
					throw new InvalidEnumArgumentException("VisionJudgeType", (int)type, typeof(VisionJudgeType));
			}
			mVisJudgeColl.Add(tool);
			if (mVisCvt != null) mVisCvt.IsCompiled = false;
			if (tool != null && !mainNod.IsExpanded) mainNod.Expand();
			return tool;
		}

		/// <summary>建立周邊工具包</summary>
		/// <param name="type">要建立的周邊工具樣式</param>
		/// <param name="mainNod">欲存放此樹狀節點的父節點</param>
		/// <returns>影像周邊工具</returns>
		public IPeripheryUtility GeneratePeriphery(PeripheryType type, TreeNode mainNod) {
			IPeripheryUtility tool = null;

			/*-- 確保目標 Target 不是 Model --*/
			TreeNode tarNode = EnsureTreeNode(mainNod);

			switch (type) {
				case PeripheryType.LightControl:
					tool = new LightCtrlPack(tarNode, mDimColl);
					break;
				case PeripheryType.CameraParameter:
					tool = new CamParaPack(tarNode);
					break;
				default:
					throw new InvalidEnumArgumentException("PeripheryType", (int)type, typeof(PeripheryType));
			}
			mPrphColl.Add(tool);
			if (mVisCvt != null) mVisCvt.IsCompiled = false;
			if (tool != null && !mainNod.IsExpanded) mainNod.Expand();
			return tool;
		}

		/// <summary>製作複製工具包，並放置到指定的 <see cref="TreeNode"/></summary>
		/// <param name="obj">欲複製的工具包</param>
		/// <param name="mainNode">目標節點</param>
		/// <returns>複製後的工具包</returns>
		public ICopyable CopyFactory(ICopyable obj, TreeNode mainNode) {
			ICopyable tarObj = null;
			if (obj is IVisionToolPack) {
				IVisionToolPack oriPack = obj as IVisionToolPack;
				switch (oriPack.ToolType.Name) {
					case "ILocatorTool":
						tarObj = new LocatorToolPack(mVisToolPackColl, oriPack, mAceFoldTool, mainNode);

						/* 找出原本 locator 裡面的 model */
						LocatorToolPack oriLocator = oriPack as LocatorToolPack;
						var models = oriLocator.Node.Nodes
										.Cast<TreeNode>()
										.Where(nod => nod.Tag is LocatorModelPack)
										.Select(nod => nod.Tag as LocatorModelPack);

						/* 將 locator 裡面的 model 先清空 */
						LocatorToolPack newLocator = tarObj as LocatorToolPack;
						ILocatorTool newLocatorTool = newLocator.Tool as ILocatorTool;
						foreach (var item in newLocatorTool.Models) {
							newLocatorTool.RemoveModel(item);
						}

						/* 複製過去 */
						var newModels = models.Select(mod => CopyModel(mod, newLocator)).Cast<IVisionToolPack>();
						mVisModelColl.AddRange(newModels);

						break;
					case "IBlobAnalyzerTool":
						tarObj = new BlobAnalyzerPack(mVisToolPackColl, oriPack, mAceFoldTool, mainNode);
						break;
					case "IImageProcessingTool":
						tarObj = new ImageProcessingPack(mVisToolPackColl, oriPack, mAceFoldTool, mainNode);
						break;
					case "IEdgeLocatorTool":
						tarObj = new EdgeLocatorPack(mVisToolPackColl, oriPack, mAceFoldTool, mainNode);
						break;
					case "ILineFinderTool":
						tarObj = new LineFinderPack(mVisToolPackColl, oriPack, mAceFoldTool, mainNode);
						break;
					case "IArcFinderTool":
						tarObj = new ArcFinderPack(mVisToolPackColl, oriPack, mAceFoldTool, mainNode);
						break;
					case "IPointFinderTool":
						tarObj = new PointFinderPack(mVisToolPackColl, oriPack, mAceFoldTool, mainNode);
						break;
					default:
						break;
				}
				if (tarObj != null) mVisToolPackColl.Add(tarObj as IVisionToolPack);

			} else if (obj is ThetaJudge) {

				tarObj = new ThetaJudge(mainNode, obj as ThetaJudge);
				if (tarObj != null) mVisJudgeColl.Add(tarObj as IVisionJudgement);

			} else if (obj is ThetaWhirling) {

				tarObj = new ThetaWhirling(mainNode, obj as ThetaWhirling);
				if (tarObj != null) mVisJudgeColl.Add(tarObj as IVisionJudgement);

			} else if (obj is DistanceJudge) {

				tarObj = new DistanceJudge(mainNode, obj as DistanceJudge);
				if (tarObj != null) mVisJudgeColl.Add(tarObj as IVisionJudgement);

			} else if (obj is LightCtrlPack) {

				tarObj = new LightCtrlPack(mainNode, obj as LightCtrlPack);
				if (tarObj != null) mPrphColl.Add(tarObj as IPeripheryUtility);

			} else if (obj is CamParaPack) {

				tarObj = new CamParaPack(mainNode, obj as CamParaPack);
				if (tarObj != null) mPrphColl.Add(tarObj as IPeripheryUtility);

			}
			return tarObj;
		}

		/// <summary>複製 <see cref="ILocatorModel"/>，並添加到 <see cref="ILocatorTool.Models"/></summary>
		/// <param name="model">欲複製的工具包</param>
		/// <param name="locator">要放置的目標</param>
		/// <returns>新複製品</returns>
		public ICopyable CopyModel(LocatorModelPack model, LocatorToolPack locator) {
			/* 建立 Model */
			LocatorModelPack newModel = new LocatorModelPack(model, locator.AceFold, locator.Node);
			/* 把這個 Model 加進 Locator 裡 */
			(locator.Tool as ILocatorTool).AddModel(newModel.Tool as ILocatorModel);
			return newModel;
		}

		/// <summary>使用 XML 節點建立評斷工具包</summary>
		/// <param name="xmlData">含有工具資訊之 XML 節點</param>
		/// <param name="toolColl">影像工具集合，用於抓取參考工具</param>
		/// <returns>對應的評斷工具包，如不存在則回傳 null</returns>
		private IVisionJudgement VisionJudgementFactory(XmlElmt xmlData, IEnumerable<IVisionToolPack> toolColl) {
			if (xmlData == null) return null;
			IVisionJudgement outObj = null;
			string vpType = xmlData.Attribute("Type")?.Value ?? string.Empty;

			switch (vpType) {
				case "DistanceJudge":
					outObj = new DistanceJudge(xmlData, rIServer, toolColl);
					break;
				case "ThetaJudge":
					outObj = new ThetaJudge(xmlData, rIServer, toolColl);
					break;
				case "ThetaWhirling":
					outObj = new ThetaWhirling(xmlData, rIServer, toolColl);
					break;
				default:
					throw new InvalidCastException("不支援的評斷工具 : " + vpType);
			}
			return outObj;
		}

		/// <summary>使用 XML 節點建立結果工具包</summary>
		/// <param name="xmlData">含有工具資訊之 XML 節點</param>
		/// <param name="toolColl">影像工具集合，用於抓取參考工具</param>
		/// <returns>對應的結果工具包，如不存在則回傳 null</returns>
		private IVisionResult VisionResultFactory(XmlElmt xmlData, IEnumerable<IVisionProjectable> toolColl) {
			if (xmlData == null) return null;
			IVisionResult outObj = null;
			string vpType = xmlData.Attribute("Type")?.Value ?? string.Empty;

			switch (vpType) {
				case "ResultAverage":
					outObj = new ResultAverage(xmlData, rIServer, toolColl);
					break;
				case "ResultTable":
					outObj = new ResultTable(xmlData, rIServer, toolColl);
					break;
				default:
					throw new InvalidCastException("不支援的結果回傳工具 : " + vpType);
			}
			return outObj;
		}

		/// <summary>使用 XML 節點建立周邊工具包</summary>
		/// <param name="xmlData">含有工具資訊之 XML 節點</param>
		/// <returns>對應的周邊工具包，如不存在則回傳 null</returns>
		private IPeripheryUtility PeripheryFactory(XmlElmt xmlData) {
			if (xmlData == null) return null;
			IPeripheryUtility outObj = null;
			string vpType = xmlData.Attribute("Type")?.Value ?? string.Empty;

			switch (vpType) {
				case "LightCtrl":
					outObj = new LightCtrlPack(xmlData, mDimColl);
					break;
				case "CameraPara":
					outObj = new CamParaPack(xmlData);
					break;
				default:
					throw new InvalidCastException("不支援的結果回傳工具 : " + vpType);
			}
			return outObj;
		}

		#endregion

		#region Function - Code Generations
		/// <summary>建立 CVT 程式碼內容</summary>
		public void GenerateScript() {
			mGenTime = DateTime.Now;
			AddStartOfScript();
			AddToolDeclare(mCvtVarMap["CVT"], mCvtVarMap["IVpLinkedObject"]);
			AddPreLightDeclare();
			AddVisionExecution(
				mCvtVarMap["CVT"],
				mCvtVarMap["ColorNg"],
				mCvtVarMap["ExecFlag"], mCvtVarMap["ExitFlag"], mCvtVarMap["PassFlag"]
			);
			AddNgCheck(mCvtVarMap["PassFlag"]);
			AddJudgement(mCvtVarMap["PassFlag"]);
			if (mVisJudgeColl.Count > 0) AddNgCheck(mCvtVarMap["PassFlag"]);
			AddResult(mCvtVarMap["CvtResult"]);
			AddSaveOkImage(mCvtVarMap["CvtResult"]);
			AddEndOfScript(mCvtVarMap["CvtResult"]);
		}

		/// <summary>加入顯示訊息於下方 Trace 欄位之程式碼</summary>
		/// <param name="msg">欲顯示的訊息</param>
		private string CreateTraceMessage(string msg) {
			return string.Format("Trace.WriteLine(\"{0}\");", msg);
		}

		/// <summary>加入顯示訊息於下方 Trace 欄位之程式碼</summary>
		/// <param name="msg">欲顯示的訊息</param>
		/// <param name="condition">於 Trace.WriteLineIf 之條件</param>
		private string CreateTraceMessage(string condition, string msg) {
			return string.Format("Trace.WriteLineIf({0}, \"{1}\");", condition, msg);
		}

		/// <summary>加入程式碼註解</summary>
		/// <param name="msg">註解訊息內容</param>
		/// <param name="cmt">註解樣式</param>
		private string CreateComment(string msg, Comment cmt) {
			string format = string.Empty;
			switch (cmt) {
				case Comment.Normal:
				default:
					format = "//{0}";
					break;
				case Comment.StartDash:
					format = "/*-- {0} --*/";
					break;
				case Comment.Start:
					format = "/* {0} */";
					break;
			}
			return string.Format(format, msg);
		}

		/// <summary>加入 CVT 上方 using namesapce 區段</summary>
		/// <returns>using namespace 區段</returns>
		private List<string> CreateUsing() {
			List<string> code = new List<string>();
			code.Add(string.Empty);
			code.Add("#region - Using Namespaces -");
			code.Add(string.Empty);
			code.Add("using Ace.Adept.Server.Desktop.Connection;");
			code.Add("using Ace.Core.Client;");
			code.Add("using Ace.Core.Server;");
			code.Add("using Ace.HSVision.Client.ImageDisplay;");
			code.Add("using Ace.HSVision.Server.Integration.Tools;");
			code.Add("using Ace.HSVision.Server.Parameters;");
			code.Add("using Ace.HSVision.Server.Tools;");
			code.Add(string.Empty);
			code.Add("using System;");
			code.Add("using System.Collections.Generic;");
			code.Add("using System.Diagnostics;");
			code.Add("using System.IO;");
			code.Add("using System.Linq;");
			code.Add("using System.Text;");
			code.Add(string.Empty);
			code.Add("using CtLib.Module.Adept;");
			code.Add("using CtLib.Module.Adept.Extension;");    //VisionTransform 和 Transform3D 的擴充
			code.Add("using CtLib.Module.Dimmer;");
			code.Add(string.Empty);
			code.Add("#endregion");
			code.Add(string.Empty);
			return code;
		}

		/// <summary>加入輔助的方法，如繪出 ROI 等</summary>
		/// <returns>方法區段</returns>
		/// <remarks>目前採用 using CtLib 的方式，避免落落長又重複</remarks>
		private List<string> CreateFunction() {
			List<string> code = new List<string>();
			code.Add("\t\t#region - Supported Functions -");
			code.Add(string.Empty);
			code.Add("\t\t/// <summary>Draw vision tool's SearchRegion on OverlayMarker</summary>");
			code.Add("\t\t/// <param name=\"tool\">The vision tool which defined SearchRegion</param>");
			code.Add("\t\t/// <param name=\"cvt\">OverlayMarker owner, main script image source to paint</param>");
			code.Add("\t\t/// <param name=\"color\">SearchRegion color</param>");
			code.Add("\t\tprivate void DrawRoi(IVisionTool tool, ICSharpCustomTool cvt, MarkerColor color) {");
			code.Add("\t\t\t/*-- Descriptor receiver --*/");
			code.Add("\t\t\tMarkerDescriptor descriptor = null;");
			code.Add(string.Empty);
			code.Add("\t\t\t/*-- Check vision tool type and draw SearchRegion on CVT --*/");
			code.Add("\t\t\tif (tool is IRectangularRoiTool) {  //IVisionTool using rectangle region");
			code.Add("\t\t\t\tIRectangularRoiTool rctRoi = tool as IRectangularRoiTool;");
			code.Add("\t\t\t\tif (rctRoi.Origins.Length > 0) {");
			code.Add("\t\t\t\t\tdescriptor = cvt.OverlayMarkers.AddRectangleMarker(");
			code.Add("\t\t\t\t\t\tnew VisionRectangle(");
			code.Add("\t\t\t\t\t\t\trctRoi.Origins[0],");
			code.Add("\t\t\t\t\t\t\trctRoi.SearchRegion");
			code.Add("\t\t\t\t\t\t)");
			code.Add("\t\t\t\t\t);");
			code.Add("\t\t\t\t} else Trace.WriteLine(\"Vision tool \" + tool.FullPath + \" have been crashed\");");
			code.Add("\t\t\t} else if (tool is ILineRoiTool) {  //IVisionTool using line region");
			code.Add("\t\t\t\tILineRoiTool lineRoi = tool as ILineRoiTool;");
			code.Add("\t\t\t\tif (lineRoi.Origins.Length > 0) {");
			code.Add("\t\t\t\t\tdescriptor = cvt.OverlayMarkers.AddRectangleMarker(");
			code.Add("\t\t\t\t\t\tnew VisionRectangle(");
			code.Add("\t\t\t\t\t\t\tlineRoi.Origins[0],");
			code.Add("\t\t\t\t\t\t\tlineRoi.SearchRegion");
			code.Add("\t\t\t\t\t\t)");
			code.Add("\t\t\t\t\t);");
			code.Add("\t\t\t\t} else Trace.WriteLine(\"Vision tool \" + tool.FullPath + \" have been crashed\");");
			code.Add("\t\t\t} else if (tool is IArcRoiTool) {   //IVisionTool using arc region");
			code.Add("\t\t\t\tIArcRoiTool arcRoi = tool as IArcRoiTool;");
			code.Add("\t\t\t\tVisionArc arc = arcRoi.SearchRegion;");
			code.Add("\t\t\t\tif (arcRoi.Origins.Length > 0) {");
			code.Add("\t\t\t\t\tdescriptor = cvt.OverlayMarkers.AddArcMarker(");
			code.Add("\t\t\t\t\t\tarcRoi.Origins[0].X,");
			code.Add("\t\t\t\t\t\tarcRoi.Origins[0].Y,");
			code.Add("\t\t\t\t\t\tarc.Radius,");
			code.Add("\t\t\t\t\t\tarc.Thickness,");
			code.Add("\t\t\t\t\t\tarc.Rotation,");
			code.Add("\t\t\t\t\t\tarc.Opening");
			code.Add("\t\t\t\t\t);");
			code.Add("\t\t\t\t} else Trace.WriteLine(\"Vision tool \" + tool.FullPath + \" have been crashed\");");
			code.Add("\t\t\t}");
			code.Add(string.Empty);
			code.Add("\t\t\t/*-- Set descriptor's color and pen width if exist --*/");
			code.Add("\t\t\tif (descriptor != null) {");
			code.Add("\t\t\t\tdescriptor.Color = color;");
			code.Add("\t\t\t\tdescriptor.PenWidth = MarkerPenWidth.Thin;");
			code.Add("\t\t\t}");
			code.Add("\t\t}");
			code.Add(string.Empty);
			code.Add("\t\t/// <summary>Draw vision tool's result on OverlayMarker</summary>");
			code.Add("\t\t/// <param name=\"tool\">The vision tool which to draw results</param>");
			code.Add("\t\t/// <param name=\"cvt\">OverlayMarker owner, main script image source to paint</param>");
			code.Add("\t\t/// <param name=\"clrPass\">The color of results are available</param>");
			code.Add("\t\t/// <param name=\"clrNg\">The color which ng presented</param>");
			code.Add("\t\tprivate void DrawResult(IVisionTool tool, ICSharpCustomTool cvt, MarkerColor clrPass, MarkerColor clrNg) {");
			code.Add("\t\t\t/*-- If there have results, draw ROI with passed color --*/");
			code.Add("\t\t\tif (tool.ResultsAvailable) {");
			code.Add("\t\t\t\t/* Define style variables */");
			code.Add("\t\t\t\tdouble length = 1.5;");
			code.Add("\t\t\t\tMarkerColor resultColor = MarkerColor.Magenta;");
			code.Add("\t\t\t\tMarkerPenWidth resultWidth = MarkerPenWidth.Thick;");
			code.Add(string.Empty);
			code.Add("\t\t\t\t/* Draw ROI */");
			code.Add("\t\t\t\tDrawRoi(tool, cvt, clrPass);");
			code.Add(string.Empty);
			code.Add("\t\t\t\t/* Do action with specifed tool type */");
			code.Add("\t\t\t\tif (tool is ILineFinderTool) {");
			code.Add("\t\t\t\t\tILineFinderTool lineTool = tool as ILineFinderTool;");
			code.Add("\t\t\t\t\tArray.ForEach(");
			code.Add("\t\t\t\t\t\tlineTool.Results,");
			code.Add("\t\t\t\t\t\tresult => {");
			code.Add("\t\t\t\t\t\t\tMarkerDescriptor descriptor = cvt.OverlayMarkers.AddLineMarker(");
			code.Add("\t\t\t\t\t\t\t\tresult.StartPoint,");
			code.Add("\t\t\t\t\t\t\t\tresult.EndPoint");
			code.Add("\t\t\t\t\t\t\t);");
			code.Add(string.Empty);
			code.Add("\t\t\t\t\t\t\tdescriptor.Color = resultColor;");
			code.Add("\t\t\t\t\t\t\tdescriptor.PenWidth = resultWidth;");
			code.Add("\t\t\t\t\t\t}");
			code.Add("\t\t\t\t\t);");
			code.Add("\t\t\t\t} else if (tool is IArcFinderTool) {");
			code.Add("\t\t\t\t\tIArcFinderTool arcFinder = tool as IArcFinderTool;");
			code.Add("\t\t\t\t\tArray.ForEach(");
			code.Add("\t\t\t\t\t\tarcFinder.Results,");
			code.Add("\t\t\t\t\t\tresult => {");
			code.Add("\t\t\t\t\t\t\tMarkerDescriptor descriptor = cvt.OverlayMarkers.AddArcMarker(");
			code.Add("\t\t\t\t\t\t\t\tresult.Center.X,");
			code.Add("\t\t\t\t\t\t\t\tresult.Center.Y,");
			code.Add("\t\t\t\t\t\t\t\tresult.Arc.Radius,");
			code.Add("\t\t\t\t\t\t\t\tresult.Arc.Thickness,");
			code.Add("\t\t\t\t\t\t\t\tresult.Arc.Rotation,");
			code.Add("\t\t\t\t\t\t\t\tresult.Arc.Opening");
			code.Add("\t\t\t\t\t\t\t);");
			code.Add(string.Empty);
			code.Add("\t\t\t\t\t\t\tdescriptor.Color = resultColor;");
			code.Add("\t\t\t\t\t\t\tdescriptor.PenWidth = resultWidth;");
			code.Add("\t\t\t\t\t\t}");
			code.Add("\t\t\t\t\t);");
			code.Add("\t\t\t\t} else {");
			code.Add("\t\t\t\t\tVisionTransform[] results = tool.GetTransformResults();");
			code.Add("\t\t\t\t\tArray.ForEach(");
			code.Add("\t\t\t\t\t\tresults,");
			code.Add("\t\t\t\t\t\tvisTrans => {");
			code.Add("\t\t\t\t\t\t\tMarkerDescriptor descriptor = cvt.OverlayMarkers.AddAxesMarker(");
			code.Add("\t\t\t\t\t\t\t\tvisTrans,");
			code.Add("\t\t\t\t\t\t\t\tlength,");
			code.Add("\t\t\t\t\t\t\t\tlength");
			code.Add("\t\t\t\t\t\t\t);");
			code.Add(string.Empty);
			code.Add("\t\t\t\t\t\t\tdescriptor.Color = resultColor;");
			code.Add("\t\t\t\t\t\t\tdescriptor.PenWidth = resultWidth;");
			code.Add("\t\t\t\t\t\t}");
			code.Add("\t\t\t\t\t);");
			code.Add("\t\t\t\t}");
			code.Add(string.Empty);
			code.Add("\t\t\t\t/*-- If there without results, draw ROI with ng color --*/");
			code.Add("\t\t\t} else DrawRoi(tool, cvt, clrNg);");
			code.Add("\t\t}");
			code.Add(string.Empty);
			code.Add("\t\t#endregion");
			return code;
		}

		/// <summary>加入 CVT Main 程式裡的起始區段</summary>
		private void AddStartOfScript() {
			mMainScript.Clear();
			mMainScript.Add(string.Empty);
			mMainScript.Add("/*--------------------------------------------------------*/");
			mMainScript.Add("/*--    This script was generated by Vision Builder     --*/");
			mMainScript.Add("/*--                             © 2016 CASTEC Inc.     --*/");
			mMainScript.Add("/*--------------------------------------------------------*/");
			mMainScript.Add(string.Empty);
			mMainScript.Add(CreateTraceMessage("Script Starting"));
			mMainScript.Add(string.Empty);
		}

		/// <summary>加入 CVT Main 程式裡的結果計算區段</summary>
		/// <param name="retVar">回傳用的區域變數</param>
		private void AddResult(string retVar) {
			mMainScript.Add(string.Empty);
			mMainScript.Add(CreateComment("Calculate results", Comment.StartDash));
			if (mVisResult != null) {
				/* 有 IVisionResult，將結果 Assign 過去 */
				mMainScript.Add($"IEnumerable<VisionTransform> {retVar} = null;");
				mMainScript.AddRange(mVisResult.GenerateCode(retVar));
			} else if (mVisJudgeColl.Exists(judge => { var tool = judge as IDynamicAngle; return tool != null && tool.DynamicResultable && tool.IsToolExist; })) {
				/* 有 IDynamicAngle，且指定 Tool 但沒有 IVisionResult 指定作為回傳，表示要處理 Tool 的結果，Assign 到 dynamicResults */
				mMainScript.Add($"IEnumerable<VisionTransform> {retVar} = dynamicResults;");
			} else if (mVisToolPackColl.Exists(pack => !(pack.Tool is IVisionImageSource))) {
				/* 如果沒有 IDynamicAngle 也沒有 IVisionResult，但有一個以上的 IVisionToolPack 且不為 Source，則拉出該 Tool 的結果 */
				IVisionToolPack tool = mVisToolPackColl.FindLast(pack => !(pack.Tool is IVisionImageSource));
                
                if (tool.ReturnRoiCenter && tool.Tool is IArcRoiTool)   //回傳 ROI 而非 result
                    mMainScript.Add($"IEnumerable<VisionTransform> {retVar} = new VisionTransform[] {{ new VisionTransform({tool.VariableName}.SearchRegion.Center) }};");
                else if (tool.ReturnRoiCenter)  //回傳 ROI 而非 result
                    mMainScript.Add($"IEnumerable<VisionTransform> {retVar} = new VisionTransform[] {{ {tool.VariableName}.Offset }};");
                else
                    mMainScript.Add($"IEnumerable<VisionTransform> {retVar} = {tool.VariableName}.GetTransformResults();");
            } else {
				/* 都沒有東西... */
				mMainScript.Add($"IEnumerable<VisionTransform> {retVar} = null;");
			}
		}

		/// <summary>加入 CVT Main 程式裡的回傳結果區段</summary>
		/// <param name="retVar">回傳用的區域變數</param>
		private void AddEndOfScript(string retVar) {
			mMainScript.Add(string.Empty);
			mMainScript.Add(CreateComment("All tools are passed and result available. Ready to return", Comment.StartDash));
			mMainScript.Add(CreateTraceMessage("Script End"));
			mMainScript.Add($"return {retVar};");
		}

		/// <summary>加入 CVT Main 程式裡的 <see cref="IVisionTool"/> 宣告</summary>
		/// <param name="cvt"><see cref="ICSharpCustomTool"/> 變數名稱</param>
		/// <param name="ctrl"><see cref="Ace.Adept.Server.Desktop.Connection.IVpLinkedObject"/> 變數名稱</param>
		private void AddToolDeclare(string cvt, string ctrl) {
			mMainScript.Add(CreateComment("Ace object declarations", Comment.StartDash));
			/* CVT */
			mMainScript.Add($"ICSharpCustomTool {cvt} = ace.Root[\"{mCvt.FullPath}\"] as ICSharpCustomTool;");
			/* EX/CX/iCobra 等 */
			mMainScript.Add($"IVpLinkedObject {ctrl} = ace.Root[\"{rAce.VpLinks[0]}\"] as IVpLinkedObject;");
			mVisToolPackColl.ForEach(
				tool => {
					string val = string.Format("{0} {1} = ace.Root[\"{2}\"] as {0};", tool.ToolType.Name, tool.VariableName, tool.ToolPath);
					Console.WriteLine(val);
					mMainScript.Add(val);
				}
			);
        }

		/// <summary>加入 CVT Main 程式裡的 <see cref="IVisionToolBase.Execute(bool)"/> 、flags 與相關變數設定</summary>
		/// <param name="cvt"><see cref="ICSharpCustomTool"/> 變數名稱</param>
		/// <param name="clrNg"><see cref="IVisionTool.ResultsAvailable"/> = false 時所畫的 ROI 顏色</param>
		/// <param name="exec">判斷 <see cref="IVisionToolBase.Execute(bool)"/> 之變數</param>
		/// <param name="exit">判斷是否有任一 <see cref="NoResultAction.EXIT_SCRIPT"/> 觸發之變數，觸發後後續則不執行</param>
		/// <param name="pass">判斷 NG 或 PASS 之變數</param>
		protected virtual void AddVisionExecution(string cvt, string clrNg, string exec, string exit, string pass) {
			var cvtTool = mVisCvt as CustomVisionToolPack;
			bool preLight = cvtTool.IsPreLight && !string.IsNullOrEmpty(cvtTool.PreLightVariable);

			/*-- MarkerColor 宣告 --*/
			mMainScript.Add(string.Empty);
			mMainScript.Add(CreateComment("Color style", Comment.StartDash));
			mMainScript.Add($"MarkerColor {clrNg} = MarkerColor.Red;");

			/*-- Flag 宣告 --*/
			mMainScript.Add(string.Empty);
			mMainScript.Add(CreateComment("Execution and judgement flags", Comment.StartDash));
			mMainScript.Add($"bool {exec} = false;\t//Point to use what execution mode");
			mMainScript.Add($"bool {exit} = false;\t//Is any EXIT_SCRIPT and result unavailable");
			mMainScript.Add($"bool {pass} = true;\t//Point to this object is (true)Pass (false)NG");
			mMainScript.Add(string.Empty);

			/*-- 用遞迴的方式，依照順序拉出所有工具 --*/
			List<IPropertable> updColl = new List<IPropertable>();
			RecursiveNodeTool(mVisCvt.Node.Nodes, updColl);

			/*-- 如果有啟用 Pre-Light，移除第一個燈與CCD --*/
			if (preLight) {
				var firstLight = updColl.Find(updObj => updObj is LightCtrlPack) as IVisionProjectable;
				var firstCam = updColl.Find(updObj => updObj is CamParaPack) as IVisionProjectable;
				var firstTool = updColl.Find(updObj => updObj is IVisionToolPack) as IVisionProjectable;

				var lightWt = firstLight == null ? int.MaxValue : firstLight.Node.Level * 10000 + firstLight.Node.Index;
				var camWt = firstCam == null ? int.MaxValue : firstCam.Node.Level * 10000 + firstCam.Node.Index;
				var toolWt = firstTool == null ? int.MaxValue : firstTool.Node.Level * 10000 + firstTool.Node.Index;

				if (lightWt < toolWt) updColl.Remove(firstLight);
				if (camWt < toolWt) updColl.Remove(firstCam);
			}

			/*-- 如果除了 Pre-Light 外，還有其他工具則建立 Executor --*/
			if (updColl.Count > 0) {
				/*-- 建構 --*/
				mMainScript.Add(CreateComment("Construct executable object collection with sorted sequence", Comment.StartDash));
				mMainScript.Add("List<ICvtExecutor> execColl = new List<ICvtExecutor> {");
				List<string> constColl = new List<string>();
				updColl.ForEach(
					updObj => {
						if (updObj is IVisionToolPack) {
							IVisionToolPack vis = updObj as IVisionToolPack;
							constColl.AddRange(vis.GenerateExecutionConstruct(cvt, clrNg));
						} else if (updObj is IPeripheryUtility)
							constColl.AddRange((updObj as IPeripheryUtility).GenerateExecutionConstruct());
					}
				);
				mMainScript.Add("\t" + string.Join(",\r\n\t\t\t\t", constColl).Replace("{0}", "cvt"));
				mMainScript.Add("};");
				mMainScript.Add(string.Empty);

				/*-- 執行 --*/
				mMainScript.Add(CreateComment("Execute each object in collection", Comment.StartDash));
				mMainScript.Add("execColl.ForEach(");
				mMainScript.Add("\texecObj => ");
				mMainScript.Add($"\t\t{pass} &= execObj.DoAction(ref {exec}, ref {exit})");
				mMainScript.Add(");");

                
			}
		}

		/// <summary>加入評斷工具之執行與判斷</summary>
		private void AddJudgement(string passVar) {
			if (mVisJudgeColl.Count > 0) {
				mMainScript.Add(string.Empty);
				mMainScript.Add(CreateComment("Judgements and measurements", Comment.StartDash));
				mVisJudgeColl.ForEach(tool => mMainScript.AddRange(tool.GenerateCode(passVar)));
			}
		}

		/// <summary>檢查並加入預先切換調光器</summary>
		private void AddPreLightDeclare() {
			CustomVisionToolPack cvt = mVisCvt as CustomVisionToolPack;
			if (cvt.IsPreLight) {
				if (string.IsNullOrEmpty(cvt.PreLightVariable)) return;

				/* 依照 TreeView 的順序拉出所有物件 */
				List<IPropertable> updColl = new List<IPropertable>();
				RecursiveNodeTool(mVisCvt.Node.Nodes, updColl);

				/* 沒有可以切的就先跳 */
				LightCtrlPack lightTool = updColl.Find(prph => prph is LightCtrlPack) as LightCtrlPack;
				CamParaPack camTool = updColl.Find(prph => prph is CamParaPack) as CamParaPack;
				IVisionToolPack firstTool = updColl.Find(prph => prph is IVisionToolPack) as IVisionToolPack;
				if (lightTool == null && camTool == null) return;

				/* 確保起碼有一個以上是在 Tool 執行之前，如果第一個 Tool 之前沒東西，表示不需要切... */
				int lightWt = lightTool == null ? int.MaxValue : lightTool.Node.Level * 10000 + lightTool.Node.Index;
				int camWt = camTool == null ? int.MaxValue : camTool.Node.Level * 10000 + camTool.Node.Index;
				int toolWt = firstTool == null ? int.MaxValue : firstTool.Node.Level * 10000 + firstTool.Node.Index;
				if (toolWt <= lightWt && toolWt <= camWt) return;   //Tool 均在 Camera、Light 之前，表示不用 Pre-Light 了(要直接拍照處理)

				/* 把可以的丟進去集合裡 */
				List<IPeripheryUtility> prphColl = new List<IPeripheryUtility>();
				if (lightWt < toolWt) prphColl.Add(lightTool);
				if (camWt < toolWt) prphColl.Add(camTool);

				/* 生出對應的建構子 */
				List<string> tarConst = new List<string>();
				prphColl.ForEach(prph => tarConst.AddRange(prph.GenerateExecutionConstruct()));

				/* 產生扣 */
				mMainScript.Add(string.Empty);
				mMainScript.Add(CreateComment("Check whether in pre-light mode", Comment.StartDash));
				mMainScript.Add($"bool preLight = Convert.ToBoolean(ctrl.Link.ListR(\"{cvt.PreLightVariable}\"));");
				mMainScript.Add("if (preLight) {");
				mMainScript.Add("\t" + CreateComment("Declare pre-light construct", Comment.Start));
				mMainScript.Add("\tList<ICvtExecutor> preExec = new List<ICvtExecutor> {");
				mMainScript.Add("\t\t" + string.Join(",\r\n\t\t\t\t\t", tarConst));
				mMainScript.Add("\t};");
				mMainScript.Add(string.Empty);
				mMainScript.Add("\t" + CreateComment("Switch light and camera parameters", Comment.Start));
				mMainScript.Add("\tpreExec.ForEach(execObj => execObj.DoAction());");
				mMainScript.Add("\treturn null;");
				mMainScript.Add("}");
			}
		}

		/// <summary>檢查是否需要儲存合格圖片</summary>
		/// <param name="retVar">回傳用的區域變數</param>
		private void AddSaveOkImage(string retVar) {
			CustomVisionToolPack cvt = mVisCvt as CustomVisionToolPack;
			if (cvt.IsSaveOkImage) {
				mMainScript.Add(string.Empty);
				mMainScript.Add(CreateComment("Check save passed image or not", Comment.StartDash));
				mMainScript.Add($"bool savePassImg = Convert.ToBoolean(ctrl.Link.ListR(\"{cvt.SaveOkImageVariable}\"));");
				mMainScript.Add($"if (savePassImg && {retVar} != null) {{");
				mMainScript.Add("\t" + CreateComment("Draw final results", Comment.Start));
				mMainScript.Add($"\tforeach (VisionTransform finRet in {retVar}) {{");
				mMainScript.Add("\t\tcvt.OverlayMarkers.AddAxesMarker(finRet, 1, 1).Color = MarkerColor.Magenta;");
				mMainScript.Add("\t}");
				mMainScript.Add("\t" + CreateComment("Saving image to file", Comment.Start));
				mMainScript.Add("\tCtDrawOverlayMarkers.BeginDrawShape(");
				mMainScript.Add("\t\tcvt,");
				mMainScript.Add("\t\tstring.Format(");
				mMainScript.Add($"\t\t\t@\"D:\\CASTEC\\Log\\{{0}}\\Component Pass\\{(cvt.Tag as object[])[1]}\\{{1}}.png\",");
				mMainScript.Add("\t\t\tDateTime.Now.ToString(\"yyyyMMdd\"),");
				mMainScript.Add("\t\t\tDateTime.Now.ToString(\"HHmmssff\")");
				mMainScript.Add("\t\t)");
				mMainScript.Add("\t);");
				mMainScript.Add("}");
			}
		}

		/// <summary>添加 NG 檢查，如 NG 則進行存圖</summary>
		/// <param name="passVar">判斷 NG 與否的變數</param>
		private void AddNgCheck(string passVar) {
			/*-- 如果有 NG 就存圖離開 --*/
			mMainScript.Add(string.Empty);
			mMainScript.Add(CreateComment("Check target is PASS or NG after tools are executed", Comment.StartDash));
			mMainScript.Add($"if (!{passVar}) {{");
			mMainScript.Add("\tCtDrawOverlayMarkers.BeginDrawShape(");
			mMainScript.Add("\t\tcvt,");
			mMainScript.Add("\t\tstring.Format(");
			mMainScript.Add($"\t\t\t@\"D:\\CASTEC\\Log\\{{0}}\\Component NG\\{(mVisCvt.Tag as object[])[1]}\\{{1}}.png\",");
			mMainScript.Add("\t\t\tDateTime.Now.ToString(\"yyyyMMdd\"),");
			mMainScript.Add("\t\t\tDateTime.Now.ToString(\"HHmmssff\")");
			mMainScript.Add("\t\t)");
			mMainScript.Add("\t);");
			mMainScript.Add("\treturn null;");
			mMainScript.Add("}");
		}
		#endregion

		#region Function - Events
		/// <summary>影像工具屬性變更，重新進行渲染</summary>
		private void Renderer_PropertyModified(object sender, EventArgs e) {
			/* 通知正在重新讀取修改後數值 */
			Console.WriteLine("Modified by UI");

			/* 判斷是否含有可判斷的 Execute() 旗標
			 * 由 PropertyModified 事件觸發時(來自綠框拖曳)，下 true 會導致畫面來不及刷新而瘋狂跳 Exception
			 * 目前僅有手動按下 Run 與 Continue 才下 true --*/
			bool exec = (e as AceBoolEventArgs)?.Value ?? false;

			if (mDisplayVision) {
				RaiseModify();
				if (mRendBase != null) {
					mVisPainted = false;
					/*-- 清除現在的畫面 --*/
					mRendBase.DisableEditMode();    //綠框需要 OFF → ON
					mRendBase.ClearToolExecutionGraphics();
					mRendBase.ImageDisplay.ClearAllGraphics();

					/*-- 抓取結果並畫出來。如果 Relative 的 Tool 沒有結果，則這邊會 GG (正常的，父母都不存在~ 兒子怎麼可能冒出來) --*/
					try {
						/*-- 重拍 --*/
						WaitVisionWindowIdle(mRendBase.Tool, exec);
						/*-- 取得執行結果 --*/
						mRendBase.CaptureExecutionResults();
						mRendBase.RenderExecutionResults();
						/*-- 啟用綠框 --*/
						mRendBase.EnableEditMode(mImgDisp, true);
					} catch (Exception ex) {
						CtStatus.Report(Stat.ER3_ACE, ex, true);
					}
				} else if (rDispCtrl != null) {
					/*-- 如果渲染物件 GG，但是 Vision Window 有顯示，表示這個工具沒有辦法做渲染，直接由外部重新 Execute --*/
					/*-- 因下面會更新 DataGridView 與 Result，所以這邊不採 BeginInvoke --*/
					RaiseExecVisTool();
				}
			}

			/* 抓取是哪個 VisionToolPack 後，重新顯示其屬性和結果 */
			var toolPath = mRendBase?.Tool?.FullPath ?? string.Empty;
			if (!string.IsNullOrEmpty(toolPath)) {
				var pack = mVisToolPackColl.Find(tool => toolPath.Equals(tool.ToolPath));
				if (pack != null) {
					/* 顯示屬性 */
					var prop = pack.CreateDataSource(mLangMap);
					RaisePropCret(prop);

					/* 顯示結果 */
					if (pack is IResultable) {
						var dt = (pack as IResultable).CreateDataTable();
						RaiseResultCret(dt);
					}
				}
			}
		}
		#endregion

		#region Function - Public Operations
		/// <summary>呼叫 Ormon ACE 內建的影像編輯工具視窗</summary>
		/// <param name="owner">欲用於 <see cref="Form.Show(IWin32Window)"/> 之 Handle</param>
		/// <param name="treNod">欲存放此節點的父節點</param>
		public void ShowVisionConfigurationWindow(Form owner, TreeNode treNod) {
			if (treNod != null && treNod.Tag != null) {
				VisionToolTeachUtil.ConfigureVisionTool(owner, rIClient, (treNod.Tag as IVisionToolPack).Tool as IVisionTool);
			}
		}

		/// <summary>呼叫 <see cref="ILocatorModel"/> 之 Ormon ACE 編輯工具</summary>
		/// <param name="owner">欲用於 <see cref="Form.Show(IWin32Window)"/> 之 Handle</param>
		/// <param name="tool">含有 <see cref="ILocatorModel"/> 之影像工具</param>
		public void ShowModelTeacher(Form owner, IVisionToolPack tool) {
			if (tool.ToolType != typeof(ILocatorModel)) return;
			ILocatorModel model = tool.Tool as ILocatorModel;
			ILocatorTool locator = (tool.Node.Parent.Tag as IVisionToolPack).Tool as ILocatorTool;
			IEnumerable<string> oriModList = locator.Models.Select(val => val.FullPath);
			VisionToolTeachUtil.TeachLocatorModel(owner, rIClient, locator, ref model);
			foreach (var item in locator.Models) {
				locator.RemoveModel(item);
			}
			foreach (var item in oriModList) {
				ILocatorModel tarMod = rIServer.Root[item] as ILocatorModel;
				locator.AddModel(tarMod);
			}
		}

		/// <summary>顯示調光器設定頁面</summary>
		/// <param name="owner">欲用於 <see cref="Form.Show(IWin32Window)"/> 之 Handle</param>
		public void ShowDimmerSetting(Form owner) {
			using (CtAceDimmerSetting form = new CtAceDimmerSetting(UILanguage.TraditionalChinese, mDimColl)) {
				form.ShowDialog(owner);
				mPrphColl.ForEach(
					prph => {
						LightCtrlPack pack = prph as LightCtrlPack;
						if (pack != null) pack.UpdateDimmer(mDimColl);
					}
				);
			}
		}

		/// <summary>重新進行渲染並觸發更新 <see cref="DataGridView.Rows"/></summary>
		/// <param name="async">是否使用非同步的方式進行渲染動作</param>
		public void ReRenderer(bool async = true) {
			AceBoolEventArgs arg = new AceBoolEventArgs(AceBoolEvents.VisionExecution, true);
			if (async) CtThread.AddTask(() => Renderer_PropertyModified(null, arg));
			else Renderer_PropertyModified(null, arg);
		}

		/// <summary>依照當前顯示畫面與否，進行渲染與觸發更新 DataGridView 等動作</summary>
		/// <param name="obj">當前處理物件</param>
		/// <param name="async">是否使用非同步的方式進行渲染動作</param>
		public void ReRenderer(IPropertable obj, bool async = true) {
			/* 如果當前是有畫面的，可能為影像類或是特定 Resultable，重新渲染 */
			if (mDisplayVision) {
				AceBoolEventArgs arg = new AceBoolEventArgs(AceBoolEvents.VisionExecution, true);
				if (async) CtThread.AddTask(() => Renderer_PropertyModified(null, arg));
				else Renderer_PropertyModified(null, arg);
			} else {
				/* 沒有畫面，可能是燈光或計算，僅刷新屬性 */
				var prop = obj.CreateDataSource(mLangMap);
				RaisePropCret(prop);
			}
		}

		/// <summary>儲存目前的影像專案，提供對話視窗供使用者選擇檔案</summary>
		public void SaveVisionProject() {
			using (SaveFileDialog dialog = new SaveFileDialog()) {
				dialog.Filter = GetMultiLangText("FileExt");
				dialog.InitialDirectory = @"D:\CASTEC\Recipe\";
				if (dialog.ShowDialog() == DialogResult.OK) {

					/* 確保 Relative 都有放回去 */
					EnsureRelativeLink();

					/* 用遞迴把 TreeNode 的 Tag 抓出來，如果是 IXmlSavable 才抓~   */
					List<IXmlSavable> xmlColl = new List<IXmlSavable>();
					if (mVisCvt != null) xmlColl.Add(mVisCvt);
					if (mVisToolPackColl.Count > 0) xmlColl.AddRange(mVisToolPackColl);
					if (mVisModelColl.Count > 0) xmlColl.AddRange(mVisModelColl);
					if (mVisJudgeColl.Count > 0) xmlColl.AddRange(mVisJudgeColl);
					if (mVisResult != null) xmlColl.Add(mVisResult);
					if (mPrphColl.Count > 0) xmlColl.AddRange(mPrphColl);

					/* 儲存檔案 */
					ExportVisionPorject(dialog.FileName, xmlColl);

					/* 告知匯出完成 */
					string tit = GetMultiLangText("ExpProjTit");
					string msg = GetMultiLangText("ExpProjOk");
					CtMsgBox.Show(rUiHndl, tit, msg, MsgBoxBtn.OK, MsgBoxStyle.Information, -1);
				}
			}
		}

		/// <summary>將當前的影像專案匯出至本機</summary>
		/// <param name="vpFileName">欲匯出的檔案名稱</param>
		/// <param name="xmlColl">所有的可儲存物件集合</param>
		public void ExportVisionPorject(string vpFileName, List<IXmlSavable> xmlColl) {
			try {
				/* 儲存名稱 */
				mCtvpName = vpFileName;

				/* 取得相關路徑 */
				string tempDir = Path.GetTempPath();
				string fileName = CtFile.GetFileName(vpFileName, false);
				string xmlPath = string.Format("{0}{1}.xml", CtFile.BackSlash(tempDir), fileName);
				string awpPath = string.Format("{0}{1}.awp", CtFile.BackSlash(tempDir), fileName);

				/* 是否有抓到東西 */
				if (xmlColl.Count > 0) {

					/* 產生 XML 節點 */
					int idx = 0, dimIdx = 0;
					List<XmlElmt> xmlVis = xmlColl.ConvertAll(tool => tool.CreateXmlData(string.Format("VisionPack_{0:D2}", idx++)));
					List<XmlElmt> xmlDim = mDimColl.ConvertAll(dim => dim.CreateXmlData(string.Format("Dimmer_{0:D2}", dimIdx++)));

					XmlElmt xmlFile = new XmlElmt(
						"VisionBuilder",
						string.Empty,
						new XmlElmt("Dimmers", xmlDim),
						new XmlElmt("VisionTools", xmlVis)
					);

					/* 儲存檔案 */
					xmlFile.Save(xmlPath);

					/* 將 ACE 資料夾匯出 */
					CtAceFile.ExportFile(mAceFoldMain, awpPath);

					/* 組合 7-Zip 命令 */
					string cmd7z = string.Format(
						"-t7z a \"{0}\" \"{1}\" \"{2}\" -mx=9 -y",
						vpFileName,
						xmlPath,
						awpPath
					);

					/* 使用 7-Zip 將 XML 與 AWP 壓縮在一起 */
					Process proc = CtApplication.ExecuteProcess("7z.exe", cmd7z, false);
					proc.WaitForExit();

					/* 將暫存區裡的檔案刪除 */
					CtFile.DeleteFile(xmlPath);
					CtFile.DeleteFile(awpPath);

					/* 將所有的 IsModifed 清除 */
					mVisCvt.IsModified = false;
					mVisToolPackColl.ForEach(pack => pack.IsModified = false);
					mVisJudgeColl.ForEach(judge => judge.IsModified = false);
					mPrphColl.ForEach(prph => prph.IsModified = false);
					if (mVisResult != null) mVisResult.IsModified = false;
				}
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex, true);
			}
		}

		/// <summary>從本機檔案匯入影像專案至 Ormon ACE，CVT 之 <see cref="TreeNode"/> 會以 <see cref="OnAssignTreeNode"/> 發布出去</summary>
		/// <param name="vpFileName">本機檔案名稱，如 @"D:\20991231.ctvp"</param>
		public void ImportVisionProject(string vpFileName) {
			/* 儲存名稱 */
			mCtvpName = vpFileName;

			/* 取得相關路徑 */
			string tempDir = Path.GetTempPath();
			string fileName = CtFile.GetFileName(vpFileName, false);
			string xmlPath = string.Format("{0}{1}.xml", CtFile.BackSlash(tempDir), fileName);
			string awpPath = string.Format("{0}{1}.awp", CtFile.BackSlash(tempDir), fileName);

			/* 組合 7-Zip 命令 */
			string cmd7z = string.Format(
				"x \"{0}\" -o\"{1}\" -y",
				vpFileName,
				tempDir
			);

			/* 使用 7-Zip 將 XML 與 AWP 解壓縮在一起 */
			Process proc = CtApplication.ExecuteProcess("7z.exe", cmd7z, false);
			proc.WaitForExit();

			/* 建立 Ace 資料夾並載入檔案 */
			rIServer.Root.CheckInternalReferences();
			mAceFoldMain = rIServer.Root["/VisionBuilder"] as IAceObjectCollection;
			if (mAceFoldMain != null) {
				try {
					DisposeObject(mAceFoldMain);
				} catch (Exception ex) {
					CtStatus.Report(Stat.ER3_ACE, ex);
				}
				mAceFoldMain = null;
				rIServer.Root.CheckInternalReferences();
			}
			mAceFoldMain = rIServer.Root.AddCollection("VisionBuilder");
			rIServer.Root.CheckInternalReferences();

			CtAceFile.ImportFile(rIServer, "/VisionBuilder", awpPath);

			/* 清除集合 */
			mVisCvt = null;
			mVisToolPackColl.Clear();
            mCalToolPackColl.Clear();
			mVisJudgeColl.Clear();
			mVisModelColl.Clear();
			mPrphColl.Clear();
			mVisResult = null;

			/* 讀取資料夾 */
			mAceFoldTool = rIServer.Root["/VisionBuilder/Tools"] as IAceObjectCollection;
			if (mAceFoldTool == null) mAceFoldMain.AddCollection("Tools");

			/* 載入 XML */
			XmlElmt ctvp = CtXML.Load(xmlPath);

			/* 抓出調光器 */
			mDimColl.Clear();
			List<XmlElmt> xmlDim = ctvp.Element("Dimmers").Elements();
			xmlDim.ForEach(xml => mDimColl.Add(new DimmerPack(xml)));

			/* 抓出 Vision Tools */
			List<XmlElmt> xmlVis = ctvp.Element("VisionTools").Elements();  //會取得 /VisionBuilder 底下的工具包

			/* 先抓出 IVisionToolPack ... 否則 IVisionResult/IVisionJudgement 需要 TreeNode 會 GG */
			List<IVisionToolPack> visPackColl = xmlVis
												.FindAll(xml => IsVisionToolPack(xml))
												.ConvertAll(xml => VisionToolPackBase.Factory(xml, rIServer, mLangMap));
			if (visPackColl != null && visPackColl.Count > 0) {
				/* 要先抓出 CVT，並建立 TreeNode 等 */
				IVisionToolPack cvtPack = visPackColl.Find(tool => tool.ToolType == typeof(ICSharpCustomTool));
				if (cvtPack != null) {
					mCvt = cvtPack.Tool as ICSharpCustomTool;
					RaiseAssignTreeNode(null, cvtPack.Node);    //將 mTreNodCvt 加入 TreeView
                    visPackColl.RemoveAll(tool => tool == mCvt);

                    
					/* 將 IVisionToolPack 加到集合理 */
					mVisToolPackColl.AddRange(visPackColl.FindAll(tool => tool.ToolType != typeof(ICSharpCustomTool) && tool.ToolType != typeof(ILocatorModel)));
					mVisToolPackColl.ForEach(pack => pack.RecoverRelativePack(mVisToolPackColl));
					mVisToolPackColl.ForEach(pack => pack.RecoverRelativeSetting());

                    /* 將ICalculatedToolPack 加到集合裡 */
                    mCalToolPackColl.AddRange(visPackColl.FindAll(tool => tool is ICalculatedToolPack).Cast<ICalculatedToolPack>());
                    mCalToolPackColl.ForEach(pack => (pack as ICalculatedToolPack).LoadReference(mVisToolPackColl));

					/* 將 ILocatorModel 加到集合理 */
					mVisModelColl.AddRange(visPackColl.FindAll(tool => tool.ToolType == typeof(ILocatorModel)));

					/* 抓出 IVisionJudgement */
					mVisJudgeColl.AddRange(xmlVis.FindAll(xml => IsVisionJudgement(xml)).ConvertAll(xml => VisionJudgementFactory(xml, mVisToolPackColl)));

					/* 抓出 IVisionResult */
					List<XmlElmt> retColl = xmlVis.FindAll(xml => IsVisionResult(xml));
					if (retColl != null && retColl.Count == 1) {
						var projColl = mVisToolPackColl.Cast<IVisionProjectable>().Concat(mVisJudgeColl);
						mVisResult = VisionResultFactory(retColl[0], projColl);
					}

					/* 抓出 IPeriphery */
					mPrphColl.AddRange(xmlVis.FindAll(xml => IsPeriphery(xml)).ConvertAll(xml => PeripheryFactory(xml)));

					/* 排序 TreeNode */
					IEnumerable<IVisionProjectable> totColl = null;
					if (mVisResult != null) {
						totColl = mVisToolPackColl.Cast<IVisionProjectable>()
									.Concat(mVisModelColl)
									.Concat(mVisJudgeColl)
									.Concat(mPrphColl)
									.Concat(new List<IVisionProjectable> { cvtPack })
									.Concat(new List<IVisionProjectable> { mVisResult });
					} else {
						totColl = mVisToolPackColl.Cast<IVisionProjectable>()
									.Concat(mVisModelColl)
									.Concat(mVisJudgeColl)
									.Concat(mPrphColl)
									.Concat(new List<IVisionProjectable> { cvtPack });
					}

					RecoverTreeNode(totColl);

					/* 指定 CVT */
					mVisCvt = cvtPack;

					/* 重新 Compiler */
					GenerateScript();
					GenerateCVT(cvtPack);

					/* 告知匯入成功 */
					string tit = GetMultiLangText("ImpProjTit");
					string msg = GetMultiLangText("ImpProjOk");
					CtMsgBox.Show(rUiHndl, tit, msg, MsgBoxBtn.OK, MsgBoxStyle.Information, -1);
				} else {
					/* 告知找不到 CVT */
					string tit = GetMultiLangText("ImpProjTit");
					string msg = GetMultiLangText("ImpProjNoCvt");
					CtMsgBox.Show(rUiHndl, tit, msg, MsgBoxBtn.OK, MsgBoxStyle.Information, -1);
				}
			} else {
				/* 告知匯入失敗 */
				string tit = GetMultiLangText("ImpProjTit");
				string msg = GetMultiLangText("ImpProjUndef");
				CtMsgBox.Show(rUiHndl, tit, msg, MsgBoxBtn.OK, MsgBoxStyle.Information, -1);
			}
		}

		/// <summary>移除特定的影像工具</summary>
		/// <param name="treNod">欲移除的樹狀節點</param>
		public void RemoveVisionTool(TreeNode treNod) {
			/*-- 先把所有有參考到這個元件的給清除關聯 --*/
			IVisionProjectable proj = treNod.Tag as IVisionProjectable;
			mVisToolPackColl.ForEach(pack => pack.ConfirmRemovedLink(proj));
			mVisJudgeColl.ForEach(pack => pack.ConfirmRemovedLink(proj));
			mVisResult?.ConfirmRemovedLink(proj);
			if (treNod.Tag is LocatorModelPack) {
				//Model 要額外移除，隸屬於不同的集合
				LocatorModelPack model = treNod.Tag as LocatorModelPack;
				mVisModelColl.Remove(model);
				CtAceFile.DeleteAceObject(model.Tool);
			} else if (treNod.Tag is IVisionToolPack) {
				//取得相關資訊
				IVisionToolPack pack = treNod.Tag as IVisionToolPack;
				Type oriType = pack.ToolType;
				int oriIdx = int.Parse(pack.AceFold.Name);

				//如果有子節點，刪除之！
				if (treNod.Nodes.Count > 0) {
					foreach (TreeNode childNode in treNod.Nodes) {
						RemoveVisionTool(childNode);
					}
				}

				//刪除 ACE 資料夾(含工具)
				CtAceFile.DeleteAceObject(pack.AceFold);
				mAceFoldTool.CheckInternalReferences();

				//從集合中移除
				mVisToolPackColl.Remove(pack);
				pack = null;

				//更換其他的工具名稱
				int tempIdx = 0;
				foreach (var item in mVisToolPackColl) {
					tempIdx = int.Parse(item.AceFold.Name);
					if (tempIdx > oriIdx) {
						item.RenameFolder((tempIdx - 1).ToString("D2"));
						if (item.ToolType == oriType) {
							string[] txtSplit = item.Node.Text.Split(CtConst.CHR_BRACKET, StringSplitOptions.RemoveEmptyEntries);
							tempIdx = int.Parse(txtSplit[0]);
							string newTxt = string.Format("[{0:D2}] {1}", (tempIdx - 1), txtSplit[1].Trim());
							item.Node.Text = newTxt;
							item.RefreshVariable(tempIdx - 1);
						}
					}
				}
			} else if (treNod.Tag is IVisionJudgement) {
				IVisionJudgement judTool = treNod.Tag as IVisionJudgement;
				mVisJudgeColl.Remove(judTool);
			} else if (treNod.Tag is IVisionResult) {
				mVisResult.AssignThetaVisionTool(null);
				mVisResult = null;
			} else if (treNod.Tag is IPeripheryUtility) {
				IPeripheryUtility perTool = treNod.Tag as IPeripheryUtility;
				mPrphColl.Remove(perTool);
			}
		}

		/// <summary>將目前的 VisionBuilder 資料夾搬移至特定位置，由對話視窗供使用者選定</summary>
		/// <returns>(<see langword="true"/>)成功移動至資料夾  (<see langword="false"/>)移動失敗</returns>
		public bool MoveFolder() {
			/* 詢問是否存檔 */
			if (!this.IsCompiled) {
				MsgBoxBtn btn = CtMsgBox.Show(
					rUiHndl,
					GetMultiLangText("ReCompTit"),
					GetMultiLangText("ReCompMsg"),
					MsgBoxBtn.YesNo | MsgBoxBtn.Cancel,
					MsgBoxStyle.Question,
					-1
				);
				if (btn == MsgBoxBtn.Yes) {
					GenerateScript();
					GenerateCVT(mVisCvt);
				} else if (btn == MsgBoxBtn.Cancel) return false;
			}

			/* 預設清單 */
			List<string> defLoc = new List<string> {
				"/Vision/Tray1",
				"/Vision/Tray2",
				"/Vision/VOF1",
				"/Vision/VOF2",
				"/Vision/PCB",
				"/Vision/Trolley Target",
			};

			/* 供使用者選擇要輸出的資料夾 */
			string tarLoc;
			Stat stt = CtInput.ComboBoxList(out tarLoc, "Output target project", "Please enter destination folder", defLoc, "/Vision/Vof1", true);
			if (stt == Stat.SUCCESS && !string.IsNullOrEmpty(tarLoc)) {
				string[] split = tarLoc.Split(CtConst.CHR_PATH, StringSplitOptions.RemoveEmptyEntries);
				if (split != null && split.Length > 0) {

					/* 檢查並取得目標路徑資料夾 */
					object[] cvtTag = mVisCvt.Tag as object[];
					IAceObjectCollection tarFold = rIServer.Root[tarLoc + "/" + cvtTag[1].ToString()] as IAceObjectCollection;
					if (tarFold != null) DisposeObject(tarFold);    //輸入路徑的資料夾，如果目前有東西就先刪掉

					tarFold = rIServer.Root[tarLoc + "/VisionBuilder"] as IAceObjectCollection;
					if (tarFold != null) DisposeObject(tarFold);    //準備要移過去的資料夾，先用 "VisionBuilder" 之後在 Rename

					/* 確認要移過去的資料夾是否存在，不存在就建立一個 */
					IAceObjectCollection fold = rIServer.Root[split[0]] as IAceObjectCollection;
					if (fold == null) fold = rIServer.Root.AddCollection(split[0]);
					if (split.Length > 1) {
						for (int idx = 1; idx < split.Length; idx++) {
							IAceObjectCollection subFold = fold.ToArray().FirstOrDefault(val => val.Name == split[idx]) as IAceObjectCollection;
							if (subFold == null) subFold = fold.AddCollection(split[idx]);
							fold = subFold;
						}
					}

					if (fold != null) {
						/* 把 ShowResultsGraphics 通通關了，避免浪費效能 */
						mVisCvt.Tool.ShowResultsGraphics = false;
						mVisToolPackColl.ForEach(pack => pack.Tool.ShowResultsGraphics = false);

						/* 先把 /VisionBuilder 從 Root 移除，然後加到目標 fold */
						IAceObjectCollection parFold = mAceFoldMain.ParentCollection;
						parFold.Remove(mAceFoldMain);
						fold.Add(mAceFoldMain);

						/* 重新命名資料夾 */
						mAceFoldMain.Name = cvtTag[1].ToString();

						/* 重新 Compile CVT，讓裡面的路徑重新抓取 */
						GenerateScript();
						GenerateCVT(mVisCvt);

						/* 確認 Sequence */
						if ((int)cvtTag[0] > 0) {
							var objColl = mAceFoldMain.FilterType(typeof(IAdeptSightSequence));
							if (objColl != null && objColl.Count > 0) {
								var seq = objColl[0] as IAdeptSightSequence;
								seq.VisionTool = mVisCvt.Tool as IVisionTool;
							}
						}

						/* 告知存檔成功 */
						string tit = GetMultiLangText("OutputTit");
						string msg = GetMultiLangText("OutputOK");
						CtMsgBox.Show(rUiHndl, tit, msg, MsgBoxBtn.OK, MsgBoxStyle.Information, -1);

						/* 清空資訊 */
						mVisCvt = null;
						mVisResult = null;
						mVisJudgeColl.Clear();
						mVisModelColl.Clear();
						mVisToolPackColl.Clear();
						mPrphColl.Clear();

						return true;
					} else {
						/* 告知存檔失敗 */
						string tit = GetMultiLangText("OutputTit");
						string msg = GetMultiLangText("OutputFail");
						CtMsgBox.Show(rUiHndl, tit, msg, MsgBoxBtn.OK, MsgBoxStyle.Information, -1);

						return false;
					}
				} else return false;
			} else return false;
		}

		/// <summary>檢查並修正 Relative 參考</summary>
		public void EnsureRelativeLink() {
			mVisToolPackColl.ForEach(pack => pack.RecoverRelativeSetting());
		}

		/// <summary>更新所有工具的節點資訊</summary>
		public void UpdateTreeNodeInfo() {
			var toolColl = mVisToolPackColl.Cast<IVisionProjectable>()
							.Concat(mVisModelColl)
							.Concat(mVisJudgeColl)
							.Concat(mPrphColl);

			toolColl.ForEach(obj => obj.UpdateTreeNodeInformation());

			if (mVisResult != null) mVisResult.UpdateTreeNodeInformation();

			if (mVisCvt != null) {
				mVisCvt.IsCompiled = false;
				mVisCvt.IsModified = true;
			}
		}

		/// <summary>取得並切換此工具之前最近的 <see cref="LightCtrlPack"/> 與 <see cref="CamParaPack"/></summary>
		/// <param name="obj">欲檢查的工具</param>
		public void SwitchLightAndCamera(IVisionProjectable obj) {
			if (obj == null) return;

			/* 如果是 LightCtrlPack，直接切 */
			LightCtrlPack lightPack = obj as LightCtrlPack;
			if (lightPack != null) {
				lightPack.TrySetDimmer();
				return;
			}

			/* 如果是 CamParamPack，直接切 */
			CamParaPack camPack = obj as CamParaPack;
			if (camPack != null) {
				camPack.TrySetParameter();
				return;
			}

			/* 如果是 Tool 之類的，往上去找最近的 LightCtrlPack 與 CamParamPack 並切換 */
			if (mPrphColl.Count > 0) {
				var projColl = mPrphColl.Cast<IVisionProjectable>()
								.Concat(new List<IVisionProjectable> { obj })
								.OrderBy(proj => proj.NodeLevel)    //依照 TreeNode 去排序
								.ThenBy(proj => proj.NodeIndex);

				/* 從上往下找，直到該 Tool 停止，記錄到的 LightCtrlPack 與 CamParaPack 就是距離最近的 */
				foreach (var item in projColl) {
					if (item.ID == obj.ID) break;
					else if (item is LightCtrlPack) lightPack = item as LightCtrlPack;
					else if (item is CamParaPack) camPack = item as CamParaPack;
				}

				/* 如果有東西，切切~ */
				if (lightPack != null) lightPack.TrySetDimmer();
				if (camPack != null) camPack.TrySetParameter();
			}
		}

		/// <summary>語系已變更，重新載入 Language.xml</summary>
		/// <param name="lang">當前切換的語系</param>
		public void LanguageChanged(UILanguage lang) {
			mLangMap = CtLanguage.GetAllLangXmlText<string>("Language.xml", lang);
		}

		/// <summary>建立屬性欄位，可供 <see cref="DataGridView"/> 重新顯示內容時使用</summary>
		/// <param name="dgvUpd">欲顯示內容之 <see cref="IPropertable"/> 物件</param>
		public List<PropertyView> CreateDataGridViewRows(IPropertable dgvUpd) {
			return dgvUpd.CreateDataSource(mLangMap);
		}

		/// <summary>顯示工具與其屬性、結果，並回傳是否可以執行</summary>
		/// <param name="obj">欲顯示的工具</param>
		/// <param name="usrLv">使用者權限</param>
		/// <returns>(<see langword="true"/>)可執行之工具  (<see langword="false"/>)不可執行</returns>
		public bool DisplayTool(IVisionProjectable obj, AccessLevel usrLv) {
			bool executable = false;

			/* 檢查 Relative，沒加回來的就把他加回來吧 */
			EnsureRelativeLink();

			/* 檢查是否需要切燈 */
			SwitchLightAndCamera(obj);

			if (obj is IVisionToolPack) {
				/* 如果是 IVisionToolPack，顯示畫面 */
				IVisionToolPack toolPack = obj as IVisionToolPack;

				//查看是否需要重新 Compile
				if (toolPack is CustomVisionToolPack && (!this.IsCompiled)) {
					Dictionary<string, string> msg = GetMultiLangText("ReCompTit", "ReCompMsg");
					MsgBoxBtn btn = CtMsgBox.Show(msg["ReCompTit"], msg["ReCompMsg"], MsgBoxBtn.YesNo, MsgBoxStyle.Question, -1);
					if (btn == MsgBoxBtn.Yes) {
						GenerateScript();
						GenerateCVT(toolPack);
					}
				}

				//顯示 Vision Window
				InitialVisionDisplay(toolPack);

				//告知可以顯示屬性與
				executable = !(obj is LocatorModelPack);
			} else if (obj is ResultTable) {
				ResultTable retTab = obj as ResultTable;
				//如果有參考工具且可以顯示之
				if (retTab.ReferenceTool != null) InitialVisionDisplay(retTab.ReferenceTool);
				//切換不可 "編輯與執行"
				executable = false;
			} else {
				//隱藏畫面
				InitialVisionDisplay(null);
				//切換不可 "編輯與執行"
				executable = false;
			}

			/* 顯示屬性，因進來時 Ctrl 已經先清空 DataGridView (為了美觀)，所以這邊不必再丟 null 了 */
			if (obj is IPropertable) {
				var prop = (obj as IPropertable).CreateDataSource(mLangMap);
				RaisePropCret(prop);
			}

			/* 顯示結果 */
			if (obj is IResultable) {
				var dt = (obj as IResultable).CreateDataTable();
				RaiseResultCret(dt);
			} else {
				RaiseResultCret(null);
			}

			return executable;
		}

		/// <summary>進行重新編譯 CVT 之動作</summary>
		public void RequestReCompile() {
			MsgBoxBtn btn = CtMsgBox.Show(mLangMap["ReCompTit"], mLangMap["ReCompMsg"], MsgBoxBtn.YesNo, MsgBoxStyle.Question, -1);
			if (btn == MsgBoxBtn.Yes) {
				/* 檢查 Relative，沒加回來的就把他加回來吧 */
				EnsureRelativeLink();
				/* Compile */
				GenerateScript();
				GenerateCVT(mVisCvt);
			}
		}
		#endregion
	}
}
