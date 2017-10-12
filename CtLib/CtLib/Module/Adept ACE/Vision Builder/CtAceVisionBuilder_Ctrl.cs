using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows.Forms;

using CtLib.Forms;
using CtLib.Library;
using CtLib.Module.Utility;
using System.Reflection;

namespace CtLib.Module.Adept {

	/// <summary>[建立中] Adept ACE CustomVisionTool Code Generator</summary>
	public partial class CtAceVisionBuilder_Ctrl : Form {

		#region Version

		/// <summary>CtAceVisionBuilder 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 
		///		0.0.0	Ahern	[2016/03/31]
		///			+ 使用 CSharpScriptBuilder 建構基本樣式
		///     
		///		0.0.1	Ahern	[2016/04/01]
		///			+ Vision Display Kit，採用 VisionToolRendererBase 抓取 VisionServer 之渲染
		///			+ CVT、Locator 之 DataGridView 項目，含讀取與寫入
		///		
		///		0.0.2	Ahern	[2016/04/06]
		///			+ 建立 Locator 時順便建出 Model
		///			+ 如果 DGV.Cell[0] 為 Image Source 則改跳視窗供選擇來源
		///		
		///		0.0.3	Ahern	[2016/04/11]
		///			+ Blob Analyzer
		///			+ ValueEditor、BoolEditor、EnumEditor、ImageSourceEditor 等輸入或選擇視窗
		///			\ 修改數值均以跳視窗供選擇或輸入
		///			\ 新建 Tool 時如果以現存 Tool 為準，將 Relative 設定之
		///		
		///		0.0.4	Ahern	[2016/04/29]
		///			- CSharpScriptBuilder
		///			+ CreateUsing
		///			+ CreateFunction
		///		
		///		0.0.5	Ahern	[2016/05/03]
		///			+ IVisionJudgement
		///		
		///		0.1.0	Ahern	[2016/05/25]
		///			\ 分離成 CtAceVisionBuilder 與 _Ctrl
		///			+ 點擊進入編輯
		///			
		///		0.1.1	Ahern	[2016/06/01]
		///			\ 調整 ContextMenu
		///			
		///		0.1.2	Ahern	[2016/06/29]
		///			+ 新增刪除時對 TreeView 進行排序
		///			
		///		0.1.3	Ahern	[2016/07/07]
		///			+ 調光器設定介面之多語系
		///			
		///		0.1.4	Ahern	[2016/07/27]
		///			+ 調光器獨立儲存 XML 設定
		///			
		///		0.1.5	Ahern	[2016/08/02]
		///			+ 調光器設定註解
		///			
		///		0.1.6	Ahern	[2016/08/12]
		///			+ 匯出與匯入專案後提示訊息
		///			
		///		0.2.0	Ahern	[2016/08/31]
		///			+ ToolStrip - Run、Continue、Stop
		///			
		///		0.3.0	Ahern	[2016/09/06]
		///			+ 顯示結果相關欄位
		/// 
		///		0.3.1	Ahern	[2016/09/23]
		///			+ 如滑鼠點擊 CVT，顯示 ReCompile
		///			+ Model 編輯鈕
		/// 
		///		0.3.2	Ahern	[2016/09/26]
		///			\ lbHelp 轉為 txtHelp 並適時套用 ScrollBar
		///			\ 開放 Border 為 Sizable
		///			\ 如編輯 Judgement 或 Resultable 則隱藏 Run 等等鈕
		///			
		///		0.4.0	Ahern	[2016/09/30]
		///			+ Node 上下移動
		///			
		///		0.5.0	Ahern	[2016/10/07]
		///			+ Copy / Paste
		///			
		///		0.6.0	Ahern	[2016/10/13]
		///			+ About
		///			
		///		0.6.1	Ahern	[2016/10/17]
		///			\ 新建專案前詢問是否清空
		///			\ 如是 CVT 不顯示移除工具
		///			
		///		0.7.0	Ahern	[2016/10/19]
		///			+ Icon
		///			+ Ctrl-S 進行存檔
		///			
		///		0.7.1	Ahern	[2017/02/02]
		///			\ GetMultiLang 改為參考 CtVisionBuilder
		///			\ Edit 時的 CreateDgvRow 改由 CtVisionBuilder 實作
		///			
		///		0.8.0	Ahern	[2017/02/11]
		///			\ 工具的屬性顯示改由 BindingList + PropertyView 顯示
		///			\ OnPropertiesCreated 顯示完後才循環檢視每一筆資料是否需要顯示 (PropertyView.CheckRowVisible)
		///			\ 編輯數值後(DoubleClick 觸發)，循環檢視每一筆資料是否需要顯示 (PropertyView.CheckRowVisible)
		///			\ ShowHelp 多個 if 判斷是否為空，為空則直接寫入不必再判斷 ScrollBar (已經顯示就算了吧)
		///			\ Edit 一開始清空註解及工具屬性 (原本最後才清空)
		///			\ 切換語系後，通知 mVisBud 重新載入語系資源並載入 CtProgress 訊息
		///			\ 為因應 OnResultsCreated 事件，ResultTable 改為新增工具、匯入 CTVP 後跟著檢查，原為顯示時才檢查(mVisBud 不知道是否需要 GetDefaultColumns)
		///			+ CtProgrss 以避免使用者空等
		///			+ Edit 時顯示 mProg，並於 OnPropertiesCreated 時關閉
		///			+ 接受 OnToolModifying 事件，顯示 mProg
		///			+ 登入登出選項
		///			+ dgvParam 之 ToolTipNeeded 事件，顯示對應 PropertyView.ToolTip 內容
		///			+ 新增工具之多國語系
		///			+ 帶有 AccessLevel 之建構子，並切換登入按鈕
		///			+ ResultTable 之 Lock，避免上一筆資料尚未顯示完畢下一筆就進來，導致 ColumnName 對不上(被洗掉了)
		///			- GetMultiLangText，均參考 mVisBud
		///			
		///		0.8.1	Ahern	[2017/02/13]
		///			\ Edit 時先清空 dgvResult
		///			\ AssignResultTable 等待 DataSource 變更完畢，並添加 dgvResult 的 DataBindingComplete 事件
		///			
		///		0.8.2	Ahern	[2017/05/23]
		///			\ Edit 添加 try-catch 避免意外
		///			\ 雙擊後的 ReRenderer 改把當前物件塞進去判斷是否重新渲染或僅刷新屬性
		///     
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(0, 8, 2, "2017/05/23", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Fields
		/// <summary>影像建置器核心</summary>
		private CtAceVisionBuilderOri mVisBud;

		/// <summary>CVT 之 <see cref="TreeNode"/></summary>
		private TreeNode mTreNodCvt;
		/// <summary>當前滑鼠點擊的 <see cref="TreeNode"/></summary>
		private TreeNode mTreNodSelect;

		/// <summary>語言切換控制</summary>
		private CtLanguage mLang;
		/// <summary>當前使用者的權限等級，用於判斷是否要顯示特定選項</summary>
		private AccessLevel mUsrLv = AccessLevel.None;

		/// <summary>連續執行之執行緒</summary>
		private Thread mThrContinue;
		/// <summary>指出是否連續執行中</summary>
		private volatile bool mFlag_Continue = false;

		/// <summary>結果欄位設定檔案路徑</summary>
		/// <remarks>Result Columns Setting File Path</remarks>
		private string mRetColSetPath = string.Empty;
		/// <summary>已載入的結果欄位設定</summary>
		private List<ResultableTable> mRetTabs;
		/// <summary>指出 <see cref="dgvResult"/> 是否已完成資料繫結</summary>
		private volatile bool mFlag_RetTblCmpl = false;

		/// <summary>當前按下「複製」的物件</summary>
		private ICopyable mCopyObj;

		/// <summary>連結至 <see cref="DataGridView.DataSource"/> 之清單</summary>
		private BindingList<PropertyView> mBindView = new BindingList<PropertyView>();
		/// <summary>用於顯示 <see cref="DataGridView"/> 時的同步鎖</summary>
		private object mDgvLock = int.MaxValue;

		/// <summary>顯示載入 Vision Tool 時的 Loading 進度條</summary>
		private CtProgress mProg;
		#endregion

		#region Function - Constructors
		/// <summary>建構影像編輯器</summary>
		/// <param name="ace">已進行連線之 <see cref="CtAce"/> 物件</param>
		public CtAceVisionBuilder_Ctrl(CtAce ace) {
			InitializeComponent();

			/* 顯示登入選項 */
			mUsrLv = AccessLevel.None;
			toolStripSeparator12.Visible = true;
			miSignIn.Visible = true;
			miSignOut.Visible = false;

			/* 初始化建置器 */
			InitializeBuilder(ace);
            subCVTToolStripMenuItem.Visible = false;
		}

		/// <summary>建構影像編輯器</summary>
		/// <param name="ace">已進行連線之 <see cref="CtAce"/> 物件</param>
		/// <param name="lv">當前的使用者權限等級</param>
		public CtAceVisionBuilder_Ctrl(CtAce ace, AccessLevel lv) {
			InitializeComponent();

			/* 隱藏登入選項 */
			mUsrLv = lv;
			toolStripSeparator12.Visible = false;
			miSignIn.Visible = false;
			miSignOut.Visible = false;

			/* 初始化建置器 */
			InitializeBuilder(ace);
		}

		/// <summary>初始化影像編輯器</summary>
		/// <param name="ace">已進行連線之 <see cref="CtAce"/> 物件</param>
		private void InitializeBuilder(CtAce ace) {
			/* 取得結果欄位設定檔並載入之 */
			mRetColSetPath = Environment.CurrentDirectory + @"\ResultColumnsSetting.xml";
			if (CtFile.IsFileExist(mRetColSetPath)) {
				XDocument doc = XDocument.Load(mRetColSetPath);
				if (doc.Root.HasElements) {
					mRetTabs = doc.Root.Elements().Select(node => new ResultableTable(node)).ToList();
				}
			}
			if (mRetTabs == null) mRetTabs = new List<ResultableTable>();

			/* 建立核心並添加事件 */
			mVisBud = new CtAceVisionBuilder(ace, this.Handle, pnVision);
			mVisBud.OnAssignTreeNode += mVisBud_OnAssignTreeNode;
			mVisBud.OnPropertiesCreated += mVisBud_OnPropCret;
			mVisBud.OnPropertiesUpdated += mVisBud_OnPropUpd;
			mVisBud.OnDisableVisionWindow += mVisBud_OnDisVisWind;
			mVisBud.OnExecuteVisionTool += mVisBud_OnExecVisTool;
			mVisBud.OnResultsUpdated += mVisBud_OnRetUpd;
			mVisBud.OnResultsCreated += mVisBud_OnRetCret;
			mVisBud.OnToolModifying += mVisBud_OnToolModify;

			/* 建立語言控制模組 */
			mLang = new CtLanguage(this);

			/* 連結 DataGridView 與 BindingList */
			colProp.DataPropertyName = "Property";
			colValue.DataPropertyName = "Content";
			dgvParam.AutoGenerateColumns = false;
			dgvParam.DataSource = mBindView;

			/* 初始化 CtProgess */
			InitializeLoadingProgress();
		}
		#endregion

		#region Function - Vision Builder Events
		/// <summary>[事件處理] 觸發需要更新 <see cref="dgvResult"/> 內容</summary>
		private void mVisBud_OnRetUpd() {
			var pack = mTreNodSelect?.Tag as IResultable;
			if (pack != null) {
				Task.Run(
					() => {
						Task.Delay(100).Wait(); //稍微等一下
						task_UpdateResultColumns(pack, false);
					}
				);
			}
		}

		/// <summary>[事件處理] 如內部無法進行渲染時，嘗試從外部重下 Execute</summary>
		private void mVisBud_OnExecVisTool() {
			(mTreNodSelect.Tag as IVisionToolPack).Tool.Execute(true);
            mProg.Close();//用於關閉圖像處裡工具
            Console.WriteLine("OnExecVisTool-mProg.Close");
        }

		/// <summary>[事件處理] 顯示 <see cref="IVisionResult"/> 或 <see cref="IVisionJudgement"/> 時隱藏 Vision Window</summary>
		private void mVisBud_OnDisVisWind() {
			pnVision.BeginInvokeIfNecessary(
				() =>
					pnVision.Controls.Cast<Control>().ForEach(ctrl => ctrl.Dispose())
			);
		}

		/// <summary>[事件處理] 當建立新工具或有修改時，告知需要更新 <see cref="dgvParam"/> 內容</summary>
		private void mVisBud_OnPropUpd() {
			if (mTreNodSelect?.Tag != null) {
				IPropertable dgvUpd = mTreNodSelect.Tag as IPropertable;
				var views = mVisBud.CreateDataGridViewRows(dgvUpd);
				dgvParam.BeginInvokeIfNecessary(
					() => {
						mBindView.Clear();
						views.ForEach(view => mBindView.Add(view));
						CheckProperyViewVisible();
					}
				);
			}
		}

		/// <summary>[事件處理] 編輯對象切換，將傳出的 <see cref="DataGridViewRow"/> 顯示到 <see cref="dgvParam"/> 上</summary>
		/// <param name="rows"></param>
		private void mVisBud_OnPropCret(List<PropertyView> rows) {
			dgvParam.InvokeIfNecessary(
				() => {
					mBindView.Clear();
					rows.ForEach(row => mBindView.Add(row));
					CheckProperyViewVisible();
				}
			);
			mProg.Close();
            Console.WriteLine("OnPropCret-mProg.Close");
		}

		/// <summary>[事件處理] 新增工具後，將傳出 <see cref="TreeNode"/> 加入樹狀</summary>
		/// <param name="main">要被添加的父節點</param>
		/// <param name="node">欲添加的新節點</param>
		private void mVisBud_OnAssignTreeNode(TreeNode main, TreeNode node) {
			if (main == null) { //如傳入 null，表示清空整個 TreeView
				treeTool.BeginInvokeIfNecessary(
					() => {
						treeTool.Nodes.Clear();
						treeTool.Nodes.Add(node);
					}
				);
				mTreNodCvt = node;
			} else {
				treeTool.BeginInvokeIfNecessary(
					() =>
						main.Nodes.Add(node)
				);
			}

			/* 檢查此工具是否可有結果，如果為可結果則檢查並建立預設欄位 */
			EnsureResultTable(node.Tag as IResultable);
		}

		/// <summary>[事件處理] 結果更新，顯示至 <see cref="dgvResult"/></summary>
		/// <param name="dt">更新後的 <see cref="DataTable"/></param>
		/// <remarks>此由 BeginInvoke 過來的，直接處理即可</remarks>
		private void mVisBud_OnRetCret(DataTable dt) {
			AssignResultTable(dt);
		}

		/// <summary>[事件處理] 建置器正在重新抓取相關數值及結果</summary>
		private void mVisBud_OnToolModify() {
			mProg.Start();
            Console.WriteLine("OnToolModify-mProg.Start");
		}
		#endregion

		#region Function - Private Methods
		/// <summary>變更介面語言</summary>
		/// <param name="lang">欲變更之語系</param>
		private void ChangeLanguage(UILanguage lang) {
			mVisBud.LanguageChanged(lang);
			mLang.ChangeUI(lang);
			InitializeLoadingProgress();
			miEdit.PerformClick();
		}

		/// <summary>顯示訊息至 <see cref="txtHelp"/> 上，並計算長度決定是否需顯示 ScrollBar</summary>
		/// <param name="msg">欲顯示的訊息</param>
		private void ShowHelp(string msg) {
			txtHelp.BeginInvokeIfNecessary(
				() => {
					if (string.IsNullOrEmpty(msg)) {
						txtHelp.Text = string.Empty;
					} else {
						/* 量測文字大小 */
						var size = TextRenderer.MeasureText(
										msg,
										txtHelp.Font,
										new Size(txtHelp.Width, int.MaxValue),
										TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl
									);

						/* 如果太長了，顯示 ScrollBar */
						if (size.Height > txtHelp.Height) txtHelp.ScrollBars = ScrollBars.Vertical;
						else txtHelp.ScrollBars = ScrollBars.None;

						/* 顯示內容 */
						txtHelp.Text = msg;
					}
				}
			);
		}

		/// <summary>建立具有多國語系的 <see cref="CtProgress"/></summary>
		private void InitializeLoadingProgress() {
			/* 先關閉當前的 CtProgress */
			if (mProg != null) mProg.Dispose();

			/* 使用多語系載入 */
			var msg = mVisBud.GetMultiLangText("ProgTit", "ProgCnt");
			mProg = new CtProgress(msg["ProgTit"], msg["ProgCnt"], false);
		}

		/// <summary>使用遞迴的方式往下尋找具有特定 <see cref="TreeNode.Text"/> 的目標</summary>
		/// <param name="node">遞迴起始節點</param>
		/// <param name="parentNodeName">欲尋找的目標 <see cref="TreeNode.Text"/></param>
		/// <returns>目標節點</returns>
		private TreeNode NodeRecursive(TreeNode node, string parentNodeName) {
			TreeNode found = null;
			if (node.Text == parentNodeName) found = node;
			else if (node.Nodes.Count > 0) {
				foreach (TreeNode item in node.Nodes) {
					found = NodeRecursive(item, parentNodeName);
					if (found != null) break;
				}
			}
			return found;
		}

		/// <summary>尋找整個 <see cref="TreeView"/> 中是否含有特定 <see cref="TreeNode.Text"/> 之節點</summary>
		/// <param name="parentNodeName">欲搜尋的文字</param>
		/// <returns>目標節點</returns>
		private TreeNode FindParentNode(string parentNodeName) {
			TreeNode found = null;
			foreach (TreeNode item in treeTool.Nodes) {
				found = NodeRecursive(item, parentNodeName);
				if (found != null) break;
			}
			return found;
		}

		/// <summary>從 <see cref="TreeNode"/> 子節點中遞迴尋找所有的 <see cref="IXmlSavable"/> 物件並加入集合</summary>
		/// <param name="treNod">欲尋找的起始節點</param>
		/// <param name="xmlColl">存放找到的 <see cref="IXmlSavable"/> 集合</param>
		private void SearchToolPack(TreeNode treNod, ref List<IXmlSavable> xmlColl) {
			IXmlSavable xmlSave = treNod.Tag as IXmlSavable;
			if (xmlSave != null) xmlColl.Add(xmlSave);

			if (treNod.Nodes.Count > 0) {
				foreach (TreeNode subNod in treNod.Nodes) {
					SearchToolPack(subNod, ref xmlColl);
				}
			}
		}

		/// <summary>使用 <see cref="TreeNode.Text"/> 來排序子節點</summary>
		/// <param name="node">起始節點</param>
		/// <remarks>後因為確保執行順序，改以 Move Up/Down 讓使用者手動決定而不自動排序</remarks>
		private void NodeSort(TreeNode node) {
			/*-- 複製 --*/
			List<TreeNode> nodColl = node.Nodes.Cast<TreeNode>().ToList();

			if (nodColl != null && nodColl.Any()) {
				/* 全部移除 */
				nodColl.ForEach(nod => node.Nodes.Remove(nod));

				/* 找 Vision Tool 並用 Text 排序 */
				IEnumerable<TreeNode> tool = nodColl.FindAll(nod => nod.Tag is IVisionToolPack).OrderBy(nod => nod.Text);
				tool.ForEach(nod => node.Nodes.Add(nod));

				/* 找 Judge Tool 並用 Text 排序 */
				tool = nodColl.Where(nod => nod.Tag is IVisionJudgement).OrderBy(nod => nod.Text);
				tool.ForEach(nod => node.Nodes.Add(nod));

				/* 找 Result Tool 並用 Text 排序 */
				tool = nodColl.Where(nod => nod.Tag is IVisionResult).OrderBy(nod => nod.Text);
				tool.ForEach(nod => node.Nodes.Add(nod));
			}

			/*-- 如果還有子節點也跟著整理吧! --*/
			foreach (TreeNode nod in node.Nodes) {
				if (nod.Nodes.Count > 0) NodeSort(nod);
			}
		}

		/// <summary>清空 Vision Window、<see cref="TreeView"/>、<see cref="DataGridView"/> 與提示欄位</summary>
		private void ClearInfo() {
			if (mVisBud != null) mVisBud.InitialVisionDisplay(null);
			CtInvoke.DataGridViewClear(dgvParam);
			ShowHelp(string.Empty);
			treeTool.BeginInvokeIfNecessary(() => treeTool.Nodes.Clear());
		}

		/// <summary>儲存選擇的結果欄位</summary>
		private void SaveResultColumns() {
			List<XElement> elmts = mRetTabs.ConvertAll(tab => tab.CreateElement());
			XElement root = new XElement("ResultColumns", elmts);
			XDocument doc = new XDocument(new XDeclaration("1.0", "UTF-8", string.Empty), root);
			doc.SaveWithIndent(mRetColSetPath);
		}

		/// <summary>根據工具的可 "執行" 狀態來切換 <see cref="ToolStripMenuItem"/> 與 <see cref="TabPage"/> 的顯示或隱藏</summary>
		/// <param name="enb">(<see langword="true"/>)可執行，顯示頁面  (<see langword="false"/>)不可執行，如 <see cref="IVisionResult"/>，隱藏頁面</param>
		private void SwitchExecutionEnabled(bool enb) {
			toolStrip.BeginInvokeIfNecessary(
				() => {
					tsRun.Enabled = enb;
					tsContinue.Enabled = enb;
					tsResult.Enabled = enb;
				}
			);

			tabInfo.BeginInvokeIfNecessary(
				() => {
					try {
						if (enb && !tabInfo.TabPages.Contains(pageResult)) tabInfo.TabPages.Add(pageResult);
						else if (!enb) tabInfo.TabPages.Remove(pageResult);
					} catch (Exception ex) {
						Console.WriteLine(ex.Message);
					}
				}
			);
		}

		/// <summary>於新增工具後，將 <see cref="TreeNode"/> 指定為當前選擇的節點</summary>
		/// <param name="node">使用者選取的節點</param>
		/// <param name="edit">是否要觸發 "編輯與執行" 功能</param>
		private void SelectTreeNode(TreeNode node, bool edit = false) {
			treeTool.InvokeIfNecessary(
				() => {
					treeTool.SelectedNode = node;
					mTreNodSelect = node;
				}
			);

			//觸發編輯
			if (edit) miEdit_Click(null, null);
		}

		/// <summary>透過 Shell 方式搜尋 Adept ACE 的檔案位置</summary>
		/// <returns>Ace.exe 路徑</returns>
		private string WhereAce() {
			/* 如直接用 Enviroment 去抓 %programfiles%，疑似因為 CtLib 是 x86 的關係會抓到 (x86) 的，故採兩段式搜尋~ */
			string sysDrive = Environment.ExpandEnvironmentVariables("%SystemDrive%");
			string progFilePath = string.Format("{0}Program Files", CtFile.BackSlash(sysDrive));
			string outMsg, errMsg;
			CtApplication.CmdProcess("WHERE /R \"" + progFilePath + "\" Ace.exe", out outMsg, out errMsg);
			if (string.IsNullOrEmpty(errMsg)) return outMsg.Trim();
			else {
				progFilePath = string.Format("{0}Program Files (x86)", CtFile.BackSlash(sysDrive));
				CtApplication.CmdProcess("WHERE /R \"" + progFilePath + "\" Ace.exe", out outMsg, out errMsg);
				if (string.IsNullOrEmpty(errMsg)) return outMsg.Trim();
				else return string.Empty;
			}
		}

		/// <summary>透過 Shell 方式搜尋 Basler Pylon 的檔案位置</summary>
		/// <returns>PylonViewerApp.exe 路徑</returns>
		private string WhereBasler() {
			/* 如直接用 Enviroment 去抓 %programfiles%，疑似因為 CtLib 是 x86 的關係會抓到 (x86) 的，故採兩段式搜尋~ */
			string sysDrive = Environment.ExpandEnvironmentVariables("%SystemDrive%");
			string progFilePath = string.Format("{0}Program Files", CtFile.BackSlash(sysDrive));
			string outMsg, errMsg;
			CtApplication.CmdProcess("WHERE /R \"" + progFilePath + "\" PylonViewerApp.exe", out outMsg, out errMsg);
			if (string.IsNullOrEmpty(errMsg)) return outMsg.Split(CtConst.CHR_CRLF)[0].Trim();
			else {
				progFilePath = string.Format("{0}Program Files (x86)", CtFile.BackSlash(sysDrive));
				CtApplication.CmdProcess("WHERE /R \"" + progFilePath + "\" PylonViewerApp.exe", out outMsg, out errMsg);
				if (string.IsNullOrEmpty(errMsg)) return outMsg.Split(CtConst.CHR_CRLF)[0].Trim();
				else return string.Empty;
			}
		}

		/// <summary>使用遞迴的方式列出 <see cref="TreeNode.Nodes"/> 所有的 <see cref="TreeNode.Text"/> 與其層數、深度</summary>
		/// <param name="nodeColl">起始節點</param>
		/// <param name="nodeStr">欲儲存 <see cref="TreeNode.Text"/> 之集合</param>
		private void GetAllNodeString(TreeNodeCollection nodeColl, List<string> nodeStr) {
			foreach (TreeNode node in nodeColl) {
				nodeStr.Add($"{node.Text},\tLevel = {node.Level},\tIndex = {node.Index}");
				if (node.Nodes.Count > 0) GetAllNodeString(node.Nodes, nodeStr);
			}
		}

		/// <summary>顯示結果至 <see cref="dgvResult"/></summary>
		/// <param name="dt">欲顯示的表單</param>
		private void AssignResultTable(DataTable dt) {
			if (dt == null) {
				dgvResult.InvokeIfNecessary(() => dgvResult.DataSource = null);
			} else if (!string.IsNullOrEmpty(dt.TableName)) {
				lock (mDgvLock) {
					/* 尋找結果欄位設定，如果還沒有設定，取得預設欄位 */
					ResultableTable retTab = mRetTabs.Find(tab => tab.TableName == dt.TableName);
                    //取得預設欄位 by Jay 2017/06/09
                    //懷疑是Arc建構時有東西有缺，將以下註解調進行測試 by Jay 2017/06/12
                    //if (retTab == null) {
                    //    IResultable tool = mTreNodSelect.Tag as IResultable;
                    //    retTab = tool.GetDefaultResultColumns();
                    //    mRetTabs.Add(retTab);
                    //}

                    /* 設定 DataTable 至 DataGridView */
                    dgvResult.InvokeIfNecessary(
						() => {
							int columnWidth = 0;
							string columnName = string.Empty;

							/* 設定資料 */
							dgvResult.DataSource = null;
							dgvResult.DataSource = dt;

							/* 等待 DataGridView 完成 DataSource 之欄位等等變更 */
							do {
								Thread.Sleep(1);
							} while (!mFlag_RetTblCmpl);

							/* 設定資料 */
							foreach (DataGridViewColumn dgvCol in dgvResult.Columns) {
							//因為是用 DataSource，所以這裡不要用 Header Text ~ 容易出事 XD
							columnName = dgvCol.DataPropertyName;
							//抓 XML 裡面看要不要顯示
							dgvCol.Visible = retTab.Columns[columnName].Visible;
							//DataGridView 設定的 Caption 屬性不會顯示
							dgvCol.HeaderText = dt.Columns[columnName].Caption;
							//改預設欄位寬度，之後要怎樣拉是他的事
							dgvCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
								columnWidth = dgvCol.Width + 10;    //留白
							dgvCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
								dgvCol.Width = columnWidth;
							}
						}
					);
				}
			}
		}

		/// <summary>檢查結果欄位表，如果尚未建立則建立預設值</summary>
		/// <param name="obj">欲檢查之工具</param>
		private void EnsureResultTable(IResultable obj) {
			if (obj != null) {
				/* 尋找結果欄位設定，如果還沒有設定，取得預設欄位 */
				if (!mRetTabs.Exists(tab => obj.ResultTableName.Equals(tab.TableName))) {
					mRetTabs.Add(obj.GetDefaultResultColumns());
				}
			}
		}

		/// <summary>使用遞迴的方式檢查所有的 <see cref="TreeNode"/> 是否都有載入結果欄位表</summary>
		/// <param name="node">起始的節點</param>
		private void EnsureResultTableRecursive(TreeNode node) {
			EnsureResultTable(node.Tag as IResultable);
			if (node.Nodes.Count > 0) {
				foreach (TreeNode subNode in node.Nodes) {
					EnsureResultTableRecursive(subNode);
				}
			}
		}

		/// <summary>循環檢查 <see cref="dgvParam"/> 內的資料是否要顯示，請於 Invoke 內使用</summary>
		private void CheckProperyViewVisible() {
			int rowCount = dgvParam.Rows.Count;
			for (int i = 0; i < rowCount; i++) {
				dgvParam.Rows[i].Visible = mBindView[i].CheckRowVisible(mUsrLv);
			}
		}
		#endregion

		#region Function - Interface Events
		/// <summary>使用者按下 <see cref="TreeView"/> 上的任意節點</summary>
		private void treeTool_MouseDown(object sender, MouseEventArgs e) {
			/* 僅抓取 "右鍵" */
			if (e.Button == MouseButtons.Right) {
				/* 取得 TreeNode */
				mTreNodSelect = treeTool.GetNodeAt(e.Location);
				if (mTreNodSelect != null) {
					treeTool.SelectedNode = mTreNodSelect;
					/* 取的顯示項目的 Flags */
					IVisionToolPack pack = mTreNodSelect.Tag as IVisionToolPack;
					bool isVis = pack != null && !(pack is LocatorModelPack);
					bool isCvt = pack?.ToolType.Name == "ICSharpCustomTool";
                    bool isMainCvt = pack is CustomVisionToolPack;
					/* 顯示 ContextMenu */
					cntxMenu.BeginInvokeIfNecessary(
						() => {
							miAdd.Visible = isVis;
							miAceUI.Visible = isVis;
							miModel.Visible = pack?.ToolType.Name == "ILocatorTool";
							miCvtReComp.Visible = isCvt;
							miRemove.Visible = !isMainCvt;
							miPaste.Enabled = !object.ReferenceEquals(mCopyObj, null);
							toolStripSeparator7.Visible = isCvt;
							cntxMenu.Show(treeTool, e.Location);
						}
					);
				}
			}
		}

		/// <summary>新增 ILocatorTool</summary>
		private void miLocator_Click(object sender, EventArgs e) {
			IVisionToolPack locator = mVisBud.GenerateTool(VisionToolType.Locator, dgvParam, mTreNodSelect);
			mVisBud.GenerateLocatorModel(locator);
			SelectTreeNode(locator.Node);
			EnsureResultTable(locator as IResultable);
		}

		/// <summary>使用 Omron|Adept ACE 精靈進行編輯</summary>
		private void miAceUI_Click(object sender, EventArgs e) {
			mVisBud.ShowVisionConfigurationWindow(this, mTreNodSelect);
		}

		/// <summary>編輯與執行</summary>
		private void miEdit_Click(object sender, EventArgs e) {

			/* 如有需要可顯示 TreeView 裡面的 TreeNode.Text 與其層數、深度 */
			//List<string> nodeStr = new List<string>();
			//GetAllNodeString(treeTool.Nodes, nodeStr);
			//MessageBox.Show(string.Join("\r\n", nodeStr));

			try {
				//因畫面更新時不想讓使用者按，所以下面就不額外開 Task 囉~
				if (mTreNodSelect?.Tag != null) {

					/* 清空 DataGridView 與 Hint */
					ShowHelp(string.Empty);
					dgvParam.InvokeIfNecessary(() => mBindView.Clear());
					dgvResult.InvokeIfNecessary(() => dgvResult.DataSource = null);

					/* 若為影像類的，啟動 Loading 進度條... 純數值的就不用了 */
					if (mTreNodSelect.Tag is IVisionToolPack) {
                        mProg.Start();
                        Console.WriteLine("Edit-mProg.Start");
                    }

                    /* 顯示工具與其屬性 */
                    bool executable = mVisBud.DisplayTool(mTreNodSelect.Tag as IVisionProjectable, mUsrLv);

					/* 顯示 "執行" */
					SwitchExecutionEnabled(executable);

					/* 如果是 Model，顯示 "編輯模型" */
					CtInvoke.ToolStripItemVisible(tsModel, mTreNodSelect.Tag is LocatorModelPack);

					/* 如果不是屬性頁，切回去屬性頁 */
					if (CtInvoke.TabControlSelectedIndex(tabInfo) != 0)
						CtInvoke.TabControlSelectedIndex(tabInfo, 0);
				}
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
		}

		/// <summary>離開</summary>
		private void miExit_Click(object sender, EventArgs e) {
			MsgBoxBtn btn = MsgBoxBtn.Yes;
			if (mVisBud.IsModified) {
				Dictionary<string, string> msg = mVisBud.GetMultiLangText("ExitTit", "ExitMsg");
				btn = CtMsgBox.Show(msg["ExitTit"], msg["ExitMsg"], MsgBoxBtn.YesNo | MsgBoxBtn.Cancel, MsgBoxStyle.Question, -1);
				if (btn == MsgBoxBtn.Yes) {
					/* 如果使用者要先儲存，先儲存 */
					mVisBud.SaveVisionProject();
				} else if (btn == MsgBoxBtn.No) btn = MsgBoxBtn.Yes;
			}
			/* 離開 */
			if (btn == MsgBoxBtn.Yes) CtInvoke.FormClose(this);
		}

		/// <summary>新增 Vision 專案</summary>
		private void miNew_Click(object sender, EventArgs e) {
			if (mVisBud.IsModified || !mVisBud.IsCompiled) {
				Dictionary<string, string> msg = mVisBud.GetMultiLangText("ExitTit", "ImportMsg");
				MsgBoxBtn btn = CtMsgBox.Show(msg["ExitTit"], msg["ImportMsg"], MsgBoxBtn.YesNo, MsgBoxStyle.Question, -1);
				if (btn == MsgBoxBtn.No) return;
			}

			/* 清除 DataGridView */
			CtInvoke.DataGridViewClear(dgvParam);
			/* 初始化 ACE Root */
			mVisBud.InitialRoot();
		}

		/// <summary>重新 Compile</summary>
		private void miExpCVT_Click(object sender, EventArgs e) {
			/*-- 確保 Input Links 有正確回來 --*/
			mVisBud.EnsureRelativeLink();

			/*-- Compile --*/
			mVisBud.GenerateScript();
			mVisBud.GenerateCVT(mTreNodCvt.Tag as IVisionToolPack);
		}

		/// <summary>DataGridView 滑鼠雙擊</summary>
		private void dgvParam_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
			if (e.ColumnIndex == 1 && e.RowIndex > -1) {
				if (mTreNodSelect != null && mTreNodSelect.Tag is IPropertable) {
					/* 取得點取的 PropertyView，並呼叫數值修改 */
					var prop = mBindView[e.RowIndex];
					prop.ExecuteEdit();

					/* 檢查有沒有要顯示或隱藏的欄位 */
					dgvParam.InvokeIfNecessary(() => CheckProperyViewVisible());

					/* 如果有修改，還沒 Compile 就詢問吧~ */
					if (!mVisBud.IsCompiled) mVisBud.RequestReCompile();

					/* 重新渲染 */
					//mVisBud.ReRenderer(mTreNodSelect.Tag as IPropertable);
                    //略過Main CVT屬性修改，避免進度條無法關閉 by Jay 2017/06/09
                    if (mTreNodSelect.Text != "Main CVT") mVisBud.ReRenderer(mTreNodSelect.Tag as IPropertable);
                }
			}
		}

		/// <summary>新增 Blob</summary>
		private void miBlob_Click(object sender, EventArgs e) {
			var pack = mVisBud.GenerateTool(VisionToolType.BlobAnalyzer, dgvParam, mTreNodSelect);
			SelectTreeNode(pack.Node);
			EnsureResultTable(pack as IResultable);
		}

		/// <summary>新增 Average Result</summary>
		private void miToolRetAvg_Click(object sender, EventArgs e) {
			var pack = mVisBud.GenerateVisionResult(VisionResultType.Average, mTreNodCvt);
			SelectTreeNode(pack.Node, true);
        }

		/// <summary>新增 Distance Measure</summary>
		private void miDistance_Click(object sender, EventArgs e) {
			var pack = mVisBud.GenerateVisionJudge(VisionJudgeType.Distance, mTreNodCvt);
			SelectTreeNode(pack.Node, true);
		}

		/// <summary>儲存 CTVP 專案</summary>
		private void miSave_Click(object sender, EventArgs e) {
			if (treeTool.Nodes.Count > 0)
				mVisBud.SaveVisionProject();
		}

		/// <summary>匯入 CTVP</summary>
		private void miImport_Click(object sender, EventArgs e) {
			try {
				/* 如果有尚未儲存或未 Compile，詢問要不要繼續 */
				if (mVisBud.IsModified || !mVisBud.IsCompiled) {
					Dictionary<string, string> msg = mVisBud.GetMultiLangText("ExitTit", "ImportMsg");
					MsgBoxBtn btn = CtMsgBox.Show(msg["ExitTit"], msg["ImportMsg"], MsgBoxBtn.YesNo, MsgBoxStyle.Question, -1);
					if (btn == MsgBoxBtn.No) return;
				}

				/* 清除所有欄位 */
				ClearInfo();

				/* 開啟 Dialog 並讓使用者選取 */
				using (OpenFileDialog dialog = new OpenFileDialog()) {
					dialog.Filter = mVisBud.GetMultiLangText("FileExt");
					dialog.InitialDirectory = @"D:\CASTEC\Recipe\";
					if (dialog.ShowDialog() == DialogResult.OK) {
						/* 如果有選擇檔案，載入之 */
						mVisBud.ImportVisionProject(dialog.FileName);
						/* 遞迴載入 ResultTable */
						EnsureResultTableRecursive(mTreNodCvt);
					}
				}
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex, true);
			}
		}

		/// <summary>調光器設定介面</summary>
		private void miDimmer_Click(object sender, EventArgs e) {
			mVisBud.ShowDimmerSetting(this);
		}

		/// <summary>移除影像工具</summary>
		private void miRemove_Click(object sender, EventArgs e) {
			if (mTreNodSelect != null) {
				try {
					/* 詢問是否確定刪除 */
					string title = mVisBud.GetMultiLangText("DelRelaTit");
					string msg = mVisBud.GetMultiLangText("DelRelaMsg");
					MsgBoxBtn btn = CtMsgBox.Show(title, msg, MsgBoxBtn.YesNo, MsgBoxStyle.Warning, -1);
					if (btn == MsgBoxBtn.No) throw new OperationCanceledException();

					/* 確認複製品 */
					if (mCopyObj != null && mCopyObj == mTreNodSelect.Tag) mCopyObj = null;
					/* 移除 Tool */
					mVisBud.RemoveVisionTool(mTreNodSelect);
					/* 移除 TreeNode */
					treeTool.Nodes.Remove(mTreNodSelect);

                    /* 解除鎖定 */
                    ToolStripMenuItem miTool = null;
                    if (mTreNodSelect.Tag is ThetaWhirling) miTool = miAngleWhirl;
                    if (miTool != null) {
                        cntxMenu.BeginInvokeIfNecessary(() => {
                            miTool.Enabled = true;
                        });
                    }

					mTreNodSelect = null;
				} catch (Exception) {
					/* 如果使用者取消會直接噴出來，不抓！ */
				}
			}
		}

		/// <summary>雙擊 TreeNode，進入編輯</summary>
		private void treeTool_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				mTreNodSelect = e.Node;
				miEdit_Click(null, null);
			}
		}

		/// <summary>新增 Edge Locator</summary>
		private void miEdge_Click(object sender, EventArgs e) {
			var pack = mVisBud.GenerateTool(VisionToolType.EdgeLocator, dgvParam, mTreNodSelect);
			SelectTreeNode(pack.Node);
			EnsureResultTable(pack as IResultable);
		}

		/// <summary>新增 Line Finder</summary>
		private void miLine_Click(object sender, EventArgs e) {
			var pack = mVisBud.GenerateTool(VisionToolType.LineFinder, dgvParam, mTreNodSelect);
			SelectTreeNode(pack.Node);
			EnsureResultTable(pack as IResultable);
		}

		/// <summary>新增 Image Processing</summary>
		private void miImgProc_Click(object sender, EventArgs e) {
			var pack = mVisBud.GenerateTool(VisionToolType.ImageProcessing, dgvParam, mTreNodSelect);
			SelectTreeNode(pack.Node);
		}

		/// <summary>新增 Arc Finder</summary>
		private void miArc_Click(object sender, EventArgs e) {
			var pack = mVisBud.GenerateTool(VisionToolType.ArcFinder, dgvParam, mTreNodSelect);
			SelectTreeNode(pack.Node);
            EnsureResultTable(pack as IResultable);//不確定是否為遺漏 by Jay 2017/06/12
        }

		/// <summary>新增 Point Finder</summary>
		private void miPoint_Click(object sender, EventArgs e) {
			var pack = mVisBud.GenerateTool(VisionToolType.PointFinder, dgvParam, mTreNodSelect);
			SelectTreeNode(pack.Node);
			EnsureResultTable(pack as IResultable);
		}

        ///<summary>新增 求點工具</summary>
        private void miCalPoint_Click(object sender, EventArgs e) {
            var pack = mVisBud.GenerateTool(VisionToolType.CalculatedPoint, dgvParam, mTreNodSelect);
            SelectTreeNode(pack.Node);
            EnsureResultTable(pack as IResultable);
        }

        ///<summary>新增 求線段工具</summary>
        private void miCalLine_Click(object sender, EventArgs e) {
            var pack = mVisBud.GenerateTool(VisionToolType.CalculatedLine, dgvParam, mTreNodSelect);
            SelectTreeNode(pack.Node);
            EnsureResultTable(pack as IResultable);
        }

        ///<summary>新增 求圓工具</summary>
        private void miCalArc_Click(object sender, EventArgs e) {
            var pack = mVisBud.GenerateTool(VisionToolType.CalculatedArc, dgvParam, mTreNodSelect);
            SelectTreeNode(pack.Node);
            EnsureResultTable(pack as IResultable);
        }

        ///<summary>新增 求位置工具</summary>
        private void miCalFrame_Click(object sender, EventArgs e) {
            var pack = mVisBud.GenerateTool(VisionToolType.CalculatedFrame, dgvParam, mTreNodSelect);
            SelectTreeNode(pack.Node);
            EnsureResultTable(pack as IResultable);
        }

        /// <summary>新增 Locator Model</summary>
        private void miModel_Click(object sender, EventArgs e) {
			IVisionToolPack locPack = mTreNodSelect.Tag as IVisionToolPack;
			if (locPack != null) {
				var model = mVisBud.GenerateLocatorModel(locPack);
				SelectTreeNode(model.Node);
			}
		}

		/// <summary>切換英文</summary>
		private void miLangEn_Click(object sender, EventArgs e) {
			ChangeLanguage(UILanguage.English);
		}

		/// <summary>切換繁中</summary>
		private void miLangTW_Click(object sender, EventArgs e) {
			ChangeLanguage(UILanguage.TraditionalChinese);
		}

		/// <summary>切換簡中</summary>
		private void miLangCN_Click(object sender, EventArgs e) {
			ChangeLanguage(UILanguage.SimplifiedChinese);
		}

		/// <summary><see cref="DataGridView"/> 點擊 <see cref="DataGridViewCell"/>，顯示提示</summary>
		private void dgvParam_CellClick(object sender, DataGridViewCellEventArgs e) {
			if (e.RowIndex > -1) {
				var prop = mBindView[e.RowIndex];
				ShowHelp(prop.Hint);
			}
		}

		/// <summary>新增 Angle Measure</summary>
		private void miAngle_Click(object sender, EventArgs e) {
			var pack = mVisBud.GenerateVisionJudge(VisionJudgeType.AngleMeasure, mTreNodCvt);
			SelectTreeNode(pack.Node, true);
		}

		/// <summary>導出 ACE 資料夾</summary>
		private void niOutputProj_Click(object sender, EventArgs e) {

			/* 確保 Relative 有放回去 */
			mVisBud.EnsureRelativeLink();

			/* 如果還沒 Compile，詢問之 */
			if (!mVisBud.IsCompiled) {
				Dictionary<string, string> msg = mVisBud.GetMultiLangText("ReCompTit", "ReCompMsg");
				MsgBoxBtn btn = CtMsgBox.Show(msg["ReCompTit"], msg["ReCompMsg"], MsgBoxBtn.YesNo | MsgBoxBtn.Cancel, MsgBoxStyle.Question, -1);
				if (btn == MsgBoxBtn.Yes) {
					mVisBud.GenerateScript();
					mVisBud.GenerateCVT(mTreNodCvt.Tag as IVisionToolPack);
				} else if (btn == MsgBoxBtn.Cancel) return;
			}

			/* 如果還沒儲存 CTVP，詢問之 */
			if (mVisBud.IsModified) {
				Dictionary<string, string> msg = mVisBud.GetMultiLangText("OutputTit", "OutputMsg");
				MsgBoxBtn btn = CtMsgBox.Show(msg["OutputTit"], msg["OutputMsg"].Replace(@"\r\n", "\r\n"), MsgBoxBtn.YesNo | MsgBoxBtn.Cancel, MsgBoxStyle.Question, -1);
				if (btn == MsgBoxBtn.Yes) {
					mVisBud.SaveVisionProject();
				} else if (btn == MsgBoxBtn.Cancel) return;
			}

			/* 嘗試移動到 ACE 資料夾，成功的話再清空所有欄位 */
			if (mVisBud.MoveFolder()) ClearInfo();
		}

		/// <summary>新增 Table Slot</summary>
		private void mTableSlot_Click(object sender, EventArgs e) {
			var pack = mVisBud.GenerateVisionResult(VisionResultType.TableSlot, mTreNodCvt);
			SelectTreeNode(pack.Node, true);
		}

		/// <summary>執行</summary>
		private void tsRun_Click(object sender, EventArgs e) {
			IVisionToolPack pack = mTreNodSelect?.Tag as IVisionToolPack;
			if (pack != null) {
				mVisBud.ReRenderer();   //ReRenderer 裡面有 Execute
			}
		}

		/// <summary>連續執行</summary>
		private void tsContinue_Click(object sender, EventArgs e) {
			IVisionToolPack pack = mTreNodSelect?.Tag as IVisionToolPack;
			if (pack != null) {
				CtInvoke.ToolStripItemEnable(tsContinue, false);
				CtInvoke.ToolStripItemEnable(tsStop, true);
				CtInvoke.ControlEnabled(treeTool, false);

				/* 等待 Thread 關閉 */
				if (mThrContinue != null && mThrContinue.IsAlive) {
					mFlag_Continue = false;
					do {
						CtTimer.Delay(100);
					} while (mThrContinue.IsAlive);
				}

				mFlag_Continue = true;

				/* 開跑 */
				mThrContinue = new Thread(task_ContinueRun);
				mThrContinue.IsBackground = true;
				mThrContinue.Name = "VisBuild_Continue";
				mThrContinue.Start();
			}
		}

		/// <summary>停止連續執行</summary>
		private void tsStop_Click(object sender, EventArgs e) {
			mFlag_Continue = false;

			CtInvoke.ToolStripItemEnable(tsContinue, true);
			CtInvoke.ToolStripItemEnable(tsStop, false);
			CtInvoke.ControlEnabled(treeTool, true);
		}

		/// <summary>顯示結果</summary>
		private void tsResult_Click(object sender, EventArgs e) {
			var pack = mTreNodSelect?.Tag as IResultable;
			if (pack != null) {
				Task.Run(() => task_UpdateResultColumns(pack));
			}
		}

		/// <summary>調整結果欄位</summary>
		private void btnRetAdj_Click(object sender, EventArgs e) {
			IResultable tool = mTreNodSelect.Tag as IResultable;
			if (tool != null) {
				/* 檢查是否已有紀錄，沒有則取得預設的欄位 */
				ResultableTable retTab = mRetTabs.Find(tab => tab.TableName == tool.ResultTableName);
				if (retTab == null) {
					retTab = tool.GetDefaultResultColumns();
					mRetTabs.Add(retTab);
				}

				/* 顯示欄位至 CtInput 讓使用者選擇 */
				var names = retTab.Columns.Select(kvp => kvp.Value.Caption);
				var enb = retTab.Columns.Where(kvp => kvp.Value.Visible).Select(kvp => kvp.Value.Caption);
				var msg = mVisBud.GetMultiLangText("AdjRetTit", "AdjRetMsg");
				List<string> result;
				Stat stt = CtInput.CheckList(out result, msg["AdjRetTit"], msg["AdjRetMsg"], names, enb);
				if (stt == Stat.SUCCESS) {
					/* 儲存之 */
					retTab.Columns.ForEach(kvp => kvp.Value.Visible = result.Contains(kvp.Value.Caption));
					SaveResultColumns();
					Task.Run(() => task_UpdateResultColumns(tool));
				}
			}
		}

		/// <summary>切換 TabControl</summary>
		private void tabInfo_SelectedIndexChanged(object sender, EventArgs e) {
			int page = CtInvoke.TabControlSelectedIndex(tabInfo);
			CtInvoke.ControlVisible(btnRetAdj, page == 1);
			ShowHelp(string.Empty);
		}

		/// <summary>新增 Angle Whirling</summary>
		private void miAngleWhirl_Click(object sender, EventArgs e) {
			var pack = mVisBud.GenerateVisionJudge(VisionJudgeType.AngleWhirling, mTreNodCvt);
			SelectTreeNode(pack.Node, true);
            cntxMenu.BeginInvokeIfNecessary(() => (sender as ToolStripMenuItem).Enabled = false);
		}

		/// <summary>編輯 Locator Model</summary>
		private void tsModel_Click(object sender, EventArgs e) {
			if (mTreNodSelect?.Tag != null) {
				LocatorModelPack model = mTreNodSelect.Tag as LocatorModelPack;
				if (model != null)
					mVisBud.ShowModelTeacher(this, model);
			}
		}

		/// <summary>新增 LightCtrlPack</summary>
		private void miLightCtrl_Click(object sender, EventArgs e) {
			var pack = mVisBud.GeneratePeriphery(PeripheryType.LightControl, mTreNodSelect);
			SelectTreeNode(pack.Node, true);
		}

		/// <summary>新增 CamParamPack</summary>
		private void miCamPara_Click(object sender, EventArgs e) {
			var pack = mVisBud.GeneratePeriphery(PeripheryType.CameraParameter, mTreNodSelect);
			SelectTreeNode(pack.Node, true);
		}

		/// <summary>往上移一格</summary>
		private void miNodeUp_Click(object sender, EventArgs e) {
			if (mTreNodSelect != null && mTreNodSelect.Index > 0) {
				TreeNode parNode = mTreNodSelect.Parent;
				if (parNode != null) {
					int oriIdx = mTreNodSelect.Index;
					parNode.Nodes.Remove(mTreNodSelect);
					parNode.Nodes.Insert(oriIdx - 1, mTreNodSelect);
					treeTool.InvokeIfNecessary(() => treeTool.SelectedNode = mTreNodSelect);
					mVisBud.UpdateTreeNodeInfo();
				}
			}
		}

		/// <summary>往下移一格</summary>
		private void miNodeDown_Click(object sender, EventArgs e) {
			if (mTreNodSelect != null) {
				TreeNode parNode = mTreNodSelect.Parent;
				if (parNode != null) {
					int oriIdx = mTreNodSelect.Index;
					parNode.Nodes.Remove(mTreNodSelect);
					parNode.Nodes.Insert(oriIdx + 1, mTreNodSelect);
					treeTool.InvokeIfNecessary(() => treeTool.SelectedNode = mTreNodSelect);
					mVisBud.UpdateTreeNodeInfo();
				}
			}
		}

		/// <summary>複製選取的項目</summary>
		private void miCopy_Click(object sender, EventArgs e) {
			ICopyable copyObj = mTreNodSelect.Tag as ICopyable;
			if (copyObj != null && !(copyObj is CustomVisionToolPack)) {
				mCopyObj = copyObj;
			}
		}

		/// <summary>貼上選取項目</summary>
		private void miPaste_Click(object sender, EventArgs e) {

			/* 如果是複製 Model，但不是貼上到 Locator，告知違法 */
			if (mCopyObj is LocatorModelPack && !(mTreNodSelect.Tag is LocatorToolPack)) {
				var msg = mVisBud.GetMultiLangText("CanNotCopyTit", "CanNotCopyMsg");
				CtMsgBox.Show(msg["CanNotCopyTit"], msg["CanNotCopyMsg"], MsgBoxBtn.OK, MsgBoxStyle.Error, -1);
				return;
			}

			/* 尋找貼上這個項目的父層含有 IVisionToolPack 的節點，避免加到 IVisionResult 等 */
			TreeNode node = mTreNodSelect;
			IVisionToolPack visPack = null;
			do {
				visPack = node.Tag as IVisionToolPack;
				if (visPack != null) break;
				else if (node.Parent != null) {
					node = node.Parent;
				} else break;
			} while (true);

			/* 如果是 Locator，連當前的 Model 也一起複製；不是就直接複製吧~ */
			if (mCopyObj != null && visPack != null) {
				if (mCopyObj is LocatorModelPack) {
					mVisBud.CopyModel(mCopyObj as LocatorModelPack, visPack as LocatorToolPack);
				} else {
					mVisBud.CopyFactory(mCopyObj, visPack.Node);
				}
			}

			mCopyObj = null;
		}

		/// <summary>關於 CAMPro</summary>
		private void miAbout_Click(object sender, EventArgs e) {
			using (CtAbout frm = new CtAbout()) {
				/*-- 取得 ACE 路徑 --*/
				string acePath = WhereAce();
				/*-- 準備暫存 HexSight 路徑 --*/
				string hxPath = string.Empty;
				/*-- 如果找不到 ACE 就抓 CtLib 內的 Ace.Adept.dll --*/
				if (string.IsNullOrEmpty(acePath)) acePath = "Ace.Adept.dll";
				/*-- 如果有找到 ACE，順便抓 HexSight 路徑 --*/
				else hxPath = CtFile.GetDirectoryName(acePath) + @"\HexSight\Controls\HexsightNet.dll";
				/*-- 抓 ACE 版本 --*/
				string aceVer = Assembly.LoadFrom(acePath).GetName().Version.ToString();
				/*-- 抓 HexSight 版本 --*/
				string hxVer = string.IsNullOrEmpty(hxPath) ? "4.0" : FileVersion.Load(hxPath).Version;
				/*-- 抓 Basler 版本 --*/
				string bslrPath = WhereBasler();
				string bslrVer = string.IsNullOrEmpty(bslrPath) ? "4.0" : FileVersion.Load(bslrPath).Version;

				Dictionary<string, string> info = new Dictionary<string, string> {
					{ "UI", Version.FullString },
					{ "Adept ACE", aceVer },
					{ "Basler", bslrVer },
					{ "HexSight", hxVer }
				};

				frm.Start(
					mVisBud,
					mVisBud.Version,
					info,
					"About Vision Builder",
					"影像建置器 (Vision Builder)",
					"Copyright © 2005~2016 CASTEC, Inc.",
					"此影像建置器需搭配 Omron|Adept ACE 軟體(以下簡稱 ACE)之使用。"
					+ "影像來源為 ACE 之複本，相關工具亦為 ACE 所使用之 HexSight 影像工具實際連結"
					+ "操作時請避免影像建置器與 ACE 交互使用以避免誤動作！"
					+ "開啟此建置器時會讓 ACE 效能低落，請於使用後關閉此程式。"
				);
			}
		}

		/// <summary>偵測鍵盤按下的動作，抓到 Ctrl+S 就跳儲存 CTVP</summary>
		private void CtAceVisionBuilder_Ctrl_KeyDown(object sender, KeyEventArgs e) {
			if (e.Control && e.KeyCode == Keys.S) {
				miSave_Click(miSave, null);
			}
		}

		/// <summary>介面關閉，Dispose 資源</summary>
		private void CtAceVisionBuilder_Ctrl_FormClosing(object sender, FormClosingEventArgs e) {
			if (mVisBud != null) mVisBud.Dispose();
		}

		/// <summary>滑鼠停留於 <see cref="dgvParam"/> 時，顯示 <see cref="PropertyView.ToolTip"/></summary>
		private void dgvParam_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e) {
			if (e.ColumnIndex == 0 && e.RowIndex > -1) {
				var view = mBindView[e.RowIndex];
				e.ToolTipText = view.ToolTip;
			}
		}

		/// <summary>使用者登入</summary>
		private void miSignIn_Click(object sender, EventArgs e) {
			using (CtLogin logIn = new CtLogin()) {
				UserData usr;
				if (logIn.Start(out usr) == Stat.SUCCESS) {
					mUsrLv = usr.Level;
					CheckProperyViewVisible();
					menuStrip.BeginInvokeIfNecessary(
						() => {
							miSignIn.Visible = false;
							miSignOut.Visible = true;
						}
					);
				}
			}
		}

		/// <summary>使用者登出</summary>
		private void miSignOut_Click(object sender, EventArgs e) {
			mUsrLv = AccessLevel.None;
			CheckProperyViewVisible();
			menuStrip.BeginInvokeIfNecessary(
				() => {
					miSignIn.Visible = true;
					miSignOut.Visible = false;
				}
			);
		}

		/// <summary>當 <see cref="dgvResult"/> 的 DataSource 完成後，發出訊號以告知完成</summary>
		private void dgvResult_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e) {
			mFlag_RetTblCmpl = true;
		}
		#endregion

		#region Function - Thread Process
		/// <summary>[執行緒處理] 連續執行</summary>
		private void task_ContinueRun() {
			IVisionToolPack pack = mTreNodSelect?.Tag as IVisionToolPack;
			if (pack == null) return;
			while (mFlag_Continue) {
				try {
					/* ReRenderer 內有 Execute 會重跑 */
					mVisBud.ReRenderer(false);
					/* Delayyyyy... */
					CtTimer.Delay(500);
				} catch (Exception ex) {
					Console.WriteLine(ex.Message);
				}
			}
		}

		/// <summary>[執行緒處理] 更新結果欄位內容</summary>
		/// <param name="tool">欲顯示結果的 <see cref="IResultable"/> 工具</param>
		/// <param name="swTab">是否自動切換至 <see cref="pageResult"/> 頁面</param>
		private void task_UpdateResultColumns(IResultable tool, bool swTab = true) {
			/* 取得 DataTable */
			DataTable dt = tool.CreateDataTable();

			/* 顯示至 DataGridView */
			AssignResultTable(dt);

			/* 如果要切換 pageResult，切吧~ */
			if (swTab) {
				int page = CtInvoke.TabControlSelectedIndex(tabInfo);
				if (page != 1) CtInvoke.TabControlSelectedIndex(tabInfo, 1);
			}
		}


        #endregion

        private void subCVTToolStripMenuItem_Click(object sender, EventArgs e) {
            var pack = mVisBud.GenerateTool(VisionToolType.SubCVT, dgvParam, mTreNodSelect);
            SelectTreeNode(pack.Node);
            EnsureResultTable(pack as IResultable);
        }
    }
}
