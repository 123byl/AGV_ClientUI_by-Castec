using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Ace.AdeptSight.Server;
using Ace.Core.Server;
using Ace.HSVision.Server.Integration.Tools;
using Ace.HSVision.Server.Tools;
using Ace.HSVision.Server.Parameters;

using CtLib.Forms;
using CtLib.Library;
using CtLib.Module.Dimmer;
using CtLib.Module.Utility;
using CtLib.Module.XML;
using Ace.Adept.Server.Desktop.Connection;

namespace CtLib.Module.Adept {

	#region Declaration - Enumerations
	/// <summary>影像結果回傳類型</summary>
	public enum VisionResultType {
		/// <summary>將影像的結果做平均</summary>
		Average,
		/// <summary>取得結果為影像分割區域中的編號</summary>
		TableSlot
	}

	/// <summary>影像評斷工具類型</summary>
	public enum VisionJudgeType {
		/// <summary>距離判斷</summary>
		Distance,
		/// <summary>角度差判斷</summary>
		AngleMeasure,
		/// <summary>角度旋轉</summary>
		AngleWhirling
	}

	/// <summary>周邊工具類型</summary>
	public enum PeripheryType {
		/// <summary>燈光(調光器)控制</summary>
		LightControl,
		/// <summary>攝影機參數</summary>
		CameraParameter
	}

	/// <summary>註解樣式</summary>
	public enum Comment {
		/// <summary>雙斜線「//」</summary>
		Normal,
		/// <summary>區段帶線「/*--  --*/」</summary>
		StartDash,
		/// <summary>區段「/* */」</summary>
		Start
	}

	/// <summary>數值編輯類型</summary>
	public enum EditType {
		/// <summary>數字</summary>
		Value,
		/// <summary>布林</summary>
		Bool,
		/// <summary>列舉</summary>
		Enum,
		/// <summary>可供影像來源之選取清單</summary>
		ImageSource
	}

	/// <summary>無結果時所相對應之動作</summary>
	public enum NoResultAction {
		/// <summary>不判斷是否有結果</summary>
		DO_NOTHING = 0,
		/// <summary>中斷當前程序，並離開回傳空結果</summary>
		EXIT_SCRIPT = 1,
		/// <summary>視為 NG</summary>
		NG = 2
	}

	/// <summary>影像結果排序方向</summary>
	public enum SortDirection {
		/// <summary>未定義</summary>
		Undefined = 0,
		/// <summary>從上方至下方</summary>
		TopToBottom = 1,
		/// <summary>從下方至上方</summary>
		BottomToTop = 2,
		/// <summary>從左至右</summary>
		LeftToRight = 4,
		/// <summary>從右至左</summary>
		RightToLeft = 8
	}

    #endregion

    #region Declaration - Interfaces

    #region Properties

    /// <summary>提供屬性及其數值之介面</summary>
    public interface IPropertable {
        /// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
        /// <param name="langMap">各國語系之對應清單</param>
        /// <returns>對應的屬性檢視</returns>
        List<PropertyView> CreateDataSource(Dictionary<string, string> langMap);
    }

    /// <summary>指出是否已變更之介面</summary>
    public interface IEditable {
        /// <summary>取得或設定物件是否已被修改</summary>
        bool IsModified { get; set; }
        /// <summary>取得或設定物件是否已被編譯過</summary>
        /// <remarks>(<see langword="true"/>)不需要再重新 Compile CVT  (<see langword="false"/>)需要重新 Compile</remarks>
        bool IsCompiled { get; set; }
    }

    /// <summary>表示可進行工具複製的工具包</summary>
    public interface ICopyable {
        /// <summary>取得此工具是否可被複製</summary>
        bool IsCopyable { get; }
    }

    /// <summary>提供產生與儲存 XML 相關資料介面</summary>
    public interface IXmlSavable {
        /// <summary>產生物件的 XML 相關資料描述</summary>
        /// <param name="nodeName">此物件之 XML 節點名稱</param>
        /// <returns>XML 節點</returns>
        XmlElmt CreateXmlData(string nodeName);
    }

    #endregion Properties

    #region Result

    /// <summary>提供執行結果之介面，並可供顯示於 <see cref="DataGridView"/> 上</summary>
    public interface IResultable {
        /// <summary>取得適用於顯示結果的 <see cref="DataTable.TableName"/></summary>
        string ResultTableName { get; }

        /// <summary>建立當前影像工具的執行結果清單。如欲隱藏特定欄位，請透過 <see cref="DataGridView.Columns"/> 進行操作</summary>
        /// <returns>對應的 執行結果清單</returns>
        DataTable CreateDataTable();

        /// <summary>取得結果清單的欄位名稱(<see cref="DataColumn.ColumnName"/>)與標題(<see cref="DataColumn.Caption"/>)對應表</summary>
        /// <returns>清單欄位與標題對應表</returns>
        Dictionary<string, string> GetResultColumnNames();

        /// <summary>建立結果清單的預設欄位名稱與預設啟用狀態之 XML 資料</summary>
        /// <returns>預設的欄位與致能 XML 資料</returns>
        ResultableTable GetDefaultResultColumns();
    }

    /// <summary>可做為最後回傳 <see cref="VisionTransform"/> 之角度定義工具</summary>
    public interface IResultOfAngle {
        /// <summary>取得或設定此影像工具是否作為最後回傳的角度定義</summary>
        bool IsAngleReturner { get; set; }

        ///<summary>產生角度CVT語法</summary>
        ///<param name="code">程式碼主體</param>
        void GenerateAngleCVT(List<string> code);
    }

    ///<summary>提供弧線結果之介面</summary>
    public interface IResultOfArc : IResultOfPoint, IResultable {

    }

    ///<summary>提供座標點結果之介面</summary>
    public interface IResultOfPoint : IResultable {

    }

    ///<summary>提供線段結果結果之介面</summary>
    public interface IResultOfLine : IResultOfTransform, IResultOfAngle {

    }

    ///<summary>提供Offset結果之介面</summary>
    public interface IResultOfTransform : IResultOfPoint, IResultOfAngle {

    }

    #endregion Result

    #region Tool Pack

    /// <summary>提供可應用於 CASTEC 影像專案之影像工具</summary>
    public interface IVisionProjectable : IPropertable, IEditable, IXmlSavable, IDisposable {
        /// <summary>取得此工具的識別碼</summary>
        long ID { get; }
        /// <summary>取得 Relatived 的識別碼</summary>
        long? InputLinkID { get; }
        /// <summary>取得此工具的 <see cref="TreeView"/> 節點</summary>
        TreeNode Node { get; }
        /// <summary>取得於 <see cref="TreeView"/> 的節點之層數(從左至右，0 開始)</summary>
        int NodeLevel { get; }
        /// <summary>取得於 <see cref="TreeView"/> 的節點於層內的深度(從上至下，0 開始)</summary>
        int NodeIndex { get; }
        /// <summary>取得此工具的註解</summary>
        string Comment { get; }
        /// <summary>取得或設定物件，其包含相關資料</summary>
        object Tag { get; set; }
        /// <summary>更新 <see cref="TreeNode"/> 相關資訊，<see cref="TreeNode.Level"/>、<see cref="TreeNode.Index"/> 等</summary>
        void UpdateTreeNodeInformation();
        /// <summary>檢查移除的物件是否與此工具有關聯，如有關聯則取消之</summary>
        /// <param name="tool">移除的物件</param>
        void ConfirmRemovedLink(IVisionProjectable tool);
    }

    /// <summary>影像工具包</summary>
    public interface IVisionToolPack : IVisionProjectable, ICopyable {

        #region Properties
        /// <summary>取得於 ACE 內之資料夾</summary>
        IAceObjectCollection AceFold { get; }
        /// <summary>取得無結果可獲取時之動作</summary>
        NoResultAction ResultUnavailable { get; }
        /// <summary>取得主要的 VisionTool</summary>
        IVisionToolBase Tool { get; }
        /// <summary>取得於 ACE 內之物件名稱</summary>
        string ToolName { get; }
        /// <summary>取得於 ACE 內之完整路徑</summary>
        string ToolPath { get; }
        /// <summary>取得主要 VisionTool 之類型</summary>
        Type ToolType { get; }
        /// <summary>取得自動創建的 C# 變數名稱</summary>
        string VariableName { get; }
        /// <summary>取得已設為 Relative 的 VisionTool</summary>
        IVisionToolBase ParentTool { get; }
        /// <summary>取得是否需要復原 Relative 物件</summary>
        bool RecoverInputRequired { get; }
        /// <summary>取得是否回傳 ROI 中心而非結果集合</summary>
        bool ReturnRoiCenter { get; }
        #endregion

        #region Methods
        /// <summary>更換 <see cref="TreeNode"/></summary>
        /// <param name="node">欲更換的節點</param>
        void AssignTreeNode(TreeNode node);

        /// <summary>更改資料夾名稱</summary>
        /// <param name="name">欲更改的名稱</param>
        void RenameFolder(string name);

        /// <summary>產生可適用於 <see cref="ICvtExecutor"/> 內的建構程式碼</summary>
        /// <param name="cvt"><see cref="ICSharpCustomTool"/> 變數名稱</param>
        /// <param name="ng">不合格的 ROI 顏色</param>
        /// <returns>建構程式</returns>
        List<string> GenerateExecutionConstruct(string cvt, string ng);

        /// <summary>如有設定 Relative，檢查並還原設定</summary>
        void RecoverRelativeSetting();

        /// <summary>如有設定 Relative，檢查並還原來源 <see cref="IVisionToolPack"/></summary>
        /// <param name="toolColl">影像工具集合</param>
        void RecoverRelativePack(IEnumerable<IVisionToolPack> toolColl);

        /// <summary>更新變數名稱</summary>
        /// <param name="idx">欲更改的索引</param>
        void RefreshVariable(int idx);

        #endregion
    }

    ///<summary>計算影像工具包</summary>
    public interface ICalculatedToolPack {
        ///<summary>是否已載入參考影像工具</summary>
        bool IsLoadRef { get; }
        ///<summary>載入參考影像工具</summary>
        void LoadReference(List<IVisionToolPack> tools);
    }

    /// <summary>合格/不合格評斷工具</summary>
    public interface IVisionJudgement : IVisionProjectable, ICopyable {
        #region Methods
        /// <summary>產生此工具的 CVT 程式碼</summary>
        /// <param name="passVar">切換 PASS 或 NG 的區域變數名稱</param>
        /// <returns>程式碼集合，一個索引對應一列</returns>
        List<string> GenerateCode(string passVar);
        #endregion
    }
    
    /// <summary>僅回傳判斷角度而非完整程式片段(結果計算)</summary>
    public interface IDynamicAngle {
        /// <summary>取得工具是否為固定角度而非進行角度補償</summary>
        bool FixedAngle { get; }
        /// <summary>取得工具是否需要回傳動態結果</summary>
        bool DynamicResultable { get; }
        /// <summary>取得參考的影像工具是否存在</summary>
        bool IsToolExist { get; }
    }

    /// <summary>適用於 CVT 回傳結果計算用之介面</summary>
    public interface IVisionResult : IVisionProjectable {

        #region Properties
        /// <summary>取得或設定是否要使用額外的 <see cref="IVisionTool"/> 結果作為旋轉角</summary>
        /// <remarks>如 Locator 只取 X、Y，而 Theta 則由 LineFinder 決定</remarks>
        bool CalculateTheta { get; set; }
        #endregion

        #region Methods
        /// <summary>產生此工具的 CVT 程式碼</summary>
        /// <param name="retVar">回傳用的區域變數</param>
        /// <returns>程式碼集合，一個索引對應一列</returns>
        List<string> GenerateCode(string retVar);
        /// <summary>添加影像工具包，視為計算用之工具</summary>
        /// <param name="tool">欲計算的影像工具包</param>
        void AddVisionTool(IVisionToolPack tool);
        /// <summary>移除影像工具包</summary>
        /// <param name="tool">欲移除的影像工具包</param>
        void RemoveVisionTool(IVisionToolPack tool);
        /// <summary>清除所有已列入計算的影像工具包</summary>
        void ClearVisionTool();
        /// <summary>指定要計算旋轉角的影像工具包</summary>
        /// <param name="tool">欲指定的影像工具包</param>
        void AssignThetaVisionTool(IResultOfAngle tool);
        #endregion
    }

    /// <summary>周邊工具，如調光、攝影機參數等</summary>
    public interface IPeripheryUtility : IVisionProjectable, ICopyable {

        #region Methods
        /// <summary>產生可適用於 <see cref="ICvtExecutor"/> 內的建構程式碼</summary>
        /// <returns>建構程式</returns>
        List<string> GenerateExecutionConstruct();
        #endregion

    }

    #endregion Tool Pack
    
    #endregion

    #region Declaration - Vision Tools
    /// <summary>影像工具包基底</summary>
    public abstract class VisionToolPackBase : IVisionToolPack {

		#region Fields
		/// <summary>主要的 VisionTool</summary>
		protected IVisionToolBase mTool;
		/// <summary>Relatived 之 Vision Tool</summary>
		protected IVisionTool mInputLinkTool;
		/// <summary>Relatived 之 <see cref="IVisionToolPack"/></summary>
		protected IVisionToolPack mInputLinkPack;
		/// <summary>此 Vision Tool 所相對應的類型</summary>
		protected Type mToolType;
		/// <summary>對應的變數名稱</summary>
		protected string mVarName = string.Empty;
		/// <summary>所建立的 TreeView 節點</summary>
		protected TreeNode mNode;
		/// <summary>Vision Tool 之 ACE 資料夾</summary>
		protected IAceObjectCollection mFold;
		/// <summary>無結果時的動作</summary>
		protected NoResultAction mIvdAct = NoResultAction.NG;
		/// <summary>所建立的 TreeView 節點為第幾層</summary>
		protected int mNodeLv = -1;
		/// <summary>所建立的 TreeView 節點深度</summary>
		protected int mNodeIdx = -1;
		/// <summary>註解</summary>
		protected object mTag = null;
		/// <summary>註解</summary>
		protected string mCmt = string.Empty;
		/// <summary>當前的 Offset</summary>
		protected VisionTransform mOfs = new VisionTransform(0, 0, 0);
		/// <summary>備份的 Offset</summary>
		protected VisionTransform mOfsShd = new VisionTransform(0, 0, 0);
		/// <summary>識別碼</summary>
		protected long mID = -1;
		/// <summary>Relatived 之 <see cref="IVisionToolPack"/> 之 ID</summary>
		protected string mInputLinkPackID = string.Empty;
		/// <summary>於 <see cref="TreeNode.Parent"/> 之 <see cref="IVisionProjectable.ID"/></summary>
		protected long? mInputLinkParentNodeID = null;
		/// <summary>是否回傳 ROI 中心而非找到的結果</summary>
		protected bool mRoiCenter = false;
		#endregion

		#region Properties
		/// <summary>取得主要的 VisionTool</summary>
		public IVisionToolBase Tool { get { return mTool; } }
		/// <summary>取得已設為 Relative 的 VisionTool</summary>
		public IVisionToolBase ParentTool { get { return mInputLinkTool; } }
		/// <summary>取得於 ACE 內之物件名稱</summary>
		public string ToolName { get { return mTool?.Name; } }
		/// <summary>取得於 ACE 內之完整路徑</summary>
		public string ToolPath { get { return mTool?.FullPath; } }
		/// <summary>取得自動創建的 C# 變數名稱</summary>
		public string VariableName { get { return mVarName; } }
		/// <summary>取得主要 VisionTool 之類型</summary>
		public Type ToolType { get { return mToolType; } }
		/// <summary>取得樹狀節點</summary>
		public TreeNode Node { get { return mNode; } }
		/// <summary>取得於 ACE 內之資料夾</summary>
		public IAceObjectCollection AceFold { get { return mFold; } }
		/// <summary>取得無結果可獲取時之動作</summary>
		public NoResultAction ResultUnavailable { get { return mIvdAct; } }
		/// <summary>取得於 <see cref="TreeView"/> 的節點之層數(從左至右，0 開始)</summary>
		public int NodeLevel { get { return mNodeLv; } }
		/// <summary>取得於 <see cref="TreeView"/> 的節點於層內的深度(從上至下，0 開始)</summary>
		public int NodeIndex { get { return mNodeIdx; } }
		/// <summary>取得或設定此項目之相關數值</summary>
		public object Tag { get { return mTag; } set { mTag = value; } }
		/// <summary>取得此工具的文字註解</summary>
		public string Comment { get { return mCmt; } }
		/// <summary>取得或設定工具是否被修改</summary>
		public bool IsModified { get; set; } = false;
		/// <summary>取得或設定工具是否已被編譯</summary>
		/// <remarks>(<see langword="true"/>)不需要再重新 Compile CVT  (<see langword="false"/>)需要重新 Compile</remarks>
		public bool IsCompiled { get; set; } = true;
		/// <summary>取得此工具的識別碼</summary>
		public long ID { get { return mID; } }
		/// <summary>取得是否需要復原 Relative 物件</summary>
		public bool RecoverInputRequired { get { return mInputLinkPack != null && (mTool as IVisionTool).GetInputLinks()[0].Reference == null; } }
		/// <summary>取得 Relatived 的識別碼</summary>
		public long? InputLinkID { get { return mInputLinkParentNodeID; } }
		/// <summary>取得是否回傳 ROI 中心而非結果集合</summary>
		public bool ReturnRoiCenter { get { return mRoiCenter; } }
		/// <summary>取得此工具是否可被複製</summary>
		public bool IsCopyable { get { return true; } }
		#endregion

		#region Factories
		/// <summary>建立影像工具包於指定的 Ace 資料夾，附帶建立 <see cref="TreeNode"/></summary>
		/// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
		/// <param name="toolType">欲建立的影像工具類型</param>
		/// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
		/// <returns>影像工具包</returns>
		public static IVisionToolPack Factory(
			List<IVisionToolPack> toolColl,
			VisionToolType toolType,
			IAceObjectCollection toolFold,
			TreeNode mainNode,
			Dictionary<string, string> langMap
		) {
			IVisionToolPack pack = null;
			switch (toolType) {
				case VisionToolType.Locator:
					pack = new LocatorToolPack(toolColl, toolFold, mainNode);
					break;
				case VisionToolType.CustomVisionTool:
					pack = new CustomVisionToolPack(toolColl, toolFold, mainNode);
					break;
				case VisionToolType.BlobAnalyzer:
					pack = new BlobAnalyzerPack(toolColl, toolFold, mainNode);
					break;
				case VisionToolType.EdgeLocator:
					pack = new EdgeLocatorPack(toolColl, toolFold, mainNode);
					break;
				case VisionToolType.LineFinder:
					pack = new LineFinderPack(toolColl, toolFold, mainNode);
					break;
				case VisionToolType.ImageProcessing:
					pack = new ImageProcessingPack(toolColl, toolFold, mainNode);
					break;
				case VisionToolType.ArcFinder:
					pack = new ArcFinderPack(toolColl, toolFold, mainNode);
					break;
				case VisionToolType.PointFinder:
					pack = new PointFinderPack(toolColl, toolFold, mainNode);
					break;
                case VisionToolType.CalculatedLine:
                    pack = new CalculatedLine(toolColl, toolFold, mainNode);
                    break;
                case VisionToolType.CalculatedPoint:
                    pack = new CalculatedPoint(toolColl, toolFold, mainNode);
                    break;
                case VisionToolType.CalculatedArc:
                    pack = new CalculatedArc(toolColl, toolFold, mainNode);
                    break;
                case VisionToolType.CalculatedFrame:
                    pack = new CalculatedFrame(toolColl, toolFold, mainNode);
                    break;
                case VisionToolType.SubCVT:
                    pack = new SubCustomVisionToolPack(toolColl, toolFold, mainNode);
                    break;
				default:
					throw new NotSupportedException(langMap["VisToolNotSup"]);
			}
			return pack;
		}

		/// <summary>建構單純的影像工具包，不含 <seealso cref="TreeNode"/> 等父節點</summary>
		/// <param name="toolType">影像工具類型</param>
		/// <param name="source">指定的影像來源</param>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <param name="fold">欲存放此工具的 Ace 資料夾</param>
		/// <returns>影像工具包</returns>
		public static IVisionToolPack Factory(
			VisionToolType toolType,
			IVisionImageSource source,
			IAceObjectCollection fold,
			Dictionary<string, string> langMap
		) {
			IVisionToolPack pack = null;
			switch (toolType) {
				case VisionToolType.Locator:
					pack = new LocatorToolPack(source, fold);
					break;
				case VisionToolType.CustomVisionTool:
					pack = new CustomVisionToolPack(source, fold);
					break;
				case VisionToolType.BlobAnalyzer:
					pack = new BlobAnalyzerPack(source, fold);
					break;
				case VisionToolType.EdgeLocator:
					pack = new EdgeLocatorPack(source, fold);
					break;
				case VisionToolType.LineFinder:
					pack = new LineFinderPack(source, fold);
					break;
				case VisionToolType.ImageProcessing:
					pack = new ImageProcessingPack(source, fold);
					break;
				case VisionToolType.ArcFinder:
					pack = new ArcFinderPack(source, fold);
					break;
				case VisionToolType.PointFinder:
					pack = new PointFinderPack(source, fold);
					break;
                case VisionToolType.CalculatedPoint:
                    pack = new CalculatedPoint(source, fold);
                    break;
                case VisionToolType.CalculatedLine:
                    pack = new CalculatedLine(source, fold);
                    break;
                case VisionToolType.CalculatedArc:
                    pack = new CalculatedArc(source, fold);
                    break;
                case VisionToolType.CalculatedFrame:
                    pack = new CalculatedFrame(source, fold);
                    break;
				default:
					throw new NotSupportedException(langMap["VisToolNotSup"]);
			}
			return pack;
		}

		/// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
		/// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
		/// <param name="aceSrv">已連線的 ACE Server 端物件</param>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <returns>影像工具包</returns>
		/// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
		/// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
		public static IVisionToolPack Factory(XmlElmt xmlData, IAceServer aceSrv, Dictionary<string, string> langMap) {
			IVisionToolPack pack = null;

			XmlAttr attr = xmlData.Attribute("Type");
			if (attr != null) {
				switch (attr.Value) {
					case "ICSharpCustomTool":
                        
						pack = xmlData.Name == "VisionPack_00" ? 
                            new CustomVisionToolPack(langMap, xmlData, aceSrv) as IVisionToolPack:
                            new SubCustomVisionToolPack(langMap,xmlData,aceSrv) as IVisionToolPack;
						break;
					case "ILocatorTool":
						pack = new LocatorToolPack(langMap, xmlData, aceSrv);
						break;
					case "ILocatorModel":
						pack = new LocatorModelPack(langMap, xmlData, aceSrv);
						break;
					case "IBlobAnalyzerTool":
						pack = new BlobAnalyzerPack(langMap, xmlData, aceSrv);
						break;
					case "ILineFinderTool":
						pack = new LineFinderPack(langMap, xmlData, aceSrv);
						break;
					case "IEdgeLocatorTool":
						pack = new EdgeLocatorPack(langMap, xmlData, aceSrv);
						break;
					case "IImageProcessingTool":
						pack = new ImageProcessingPack(langMap, xmlData, aceSrv);
						break;
					case "IArcFinderTool":
						pack = new ArcFinderPack(langMap, xmlData, aceSrv);
						break;
					case "IPointFinderTool":
						pack = new PointFinderPack(langMap, xmlData, aceSrv);
						break;
                    case "ICalculatedPointTool":
                        pack = new CalculatedPoint(langMap, xmlData, aceSrv);
                        break;
                    case "ICalculatedLineTool":
                        pack = new CalculatedLine(langMap, xmlData, aceSrv);
                        break;
                    case "ICalculatedArcTool":
                        pack = new CalculatedArc(langMap, xmlData, aceSrv);
                        break;
                    case "ICalculatedFrameTool":
                        pack = new CalculatedFrame(langMap, xmlData, aceSrv);
                        break;
					default:
						throw new InvalidCastException(langMap["VisToolNotSup"]);
				}
			}
			return pack;
		}

		/// <summary>建立 <see cref="ILocatorModel"/> 之工具包，請勿加入 IVisionToolPack 集合物件，避免偵測時失誤</summary>
		/// <param name="source">指定的影像來源</param>
		/// <param name="toolFold">指定存放的 Ace 資料夾</param>
		/// <param name="mainNode">存放含有 <see cref="ILocatorTool"/> 的 <seealso cref="IVisionToolPack"/> 之 <seealso cref="TreeNode"/></param>
		/// <returns>影像工具包</returns>
		public static IVisionToolPack Factory(IVisionImageSource source, IAceObjectCollection toolFold, TreeNode mainNode) {
			return new LocatorModelPack(source, toolFold, mainNode);
		}
		#endregion

		#region Constructor Implements
		/// <summary>空白建構子</summary>
		internal VisionToolPackBase() {
			//Nothing to do ...
		}

		/// <summary>建立影像工具包於指定的 Ace 資料夾，附帶建立 <see cref="TreeNode"/></summary>
		/// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
		/// <param name="toolType">欲建立的影像工具類型</param>
		/// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
		/// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
		internal VisionToolPackBase(List<IVisionToolPack> toolColl, VisionToolType toolType, IAceObjectCollection toolFold, TreeNode mainNode) {
			int idx = toolColl.Count + 1;
			mID = DateTime.Now.ToBinary();

			/* 如果 TreeNode 有父節點，看看父節點是不是影像工具包，是的話就抓出作為 Relative Tool */
			IVisionImageSource imgSrc = null;
			if (mainNode?.Tag != null) {
				IVisionToolPack pack = mainNode.Tag as IVisionToolPack;
				if (pack != null) {
					IVisionTool tool = pack.Tool as IVisionTool;
					if (tool != null) {
						imgSrc = tool.ImageSource;
						if (pack.ToolType != typeof(ICSharpCustomTool)) {
							mInputLinkPack = pack;
							mInputLinkTool = pack.Tool as IVisionTool;
						}
					} else {
						imgSrc = pack.Tool as IVisionImageSource;

						if (imgSrc != null) {
							/* 因 IVisionImageSource 沒辦法取得 Input Linked，故從 TreeNode 下手 */
							TreeNode tempNode = mainNode;
							do {
								if (tempNode.Parent != null) {
									tempNode = tempNode.Parent;
									IVisionToolPack tempPack = tempNode.Tag as IVisionToolPack;
									if (tempPack != null && !(tempPack.Tool is IVisionImageSource) && !(tempPack is CustomVisionToolPack)) {
										mInputLinkPack = tempPack;
										mInputLinkTool = tempPack.Tool as IVisionTool;
										break;
									}
								} else break;
							} while (true);
						}
					}
				}
			}

			/* 新增工具&資料夾 */
			mToolType = GetToolType(toolType);  //因 ACE 如果直接 typeof(Tool) 會得到非 Interface 的類型，故這邊多做一個取類型的
			mFold = toolFold.AddCollection(idx.ToString("00")); //建立新資料夾並放新增的工具進去
			mTool = mFold.AddObjectOfType(mToolType, GetToolString(toolType)) as IVisionToolBase;   //新增工具

			IVisionTool iTool = mTool as IVisionTool;
			if (iTool != null) {
				iTool.ImageSource = imgSrc; //影像來源
				iTool.GetInputLinks()[0].Reference = mInputLinkTool;   //設定 Relative Tool 
			} else if (mToolType == typeof(IImageProcessingTool)) {
				IImageProcessingTool procTool = mTool as IImageProcessingTool;
				procTool.ImageSource = imgSrc;
				var inputLink = procTool.GetInputLinks();
				if (inputLink != null && inputLink.Length > 0) inputLink[0].Reference = mInputLinkTool;   //設定 Relative Tool 
			}

            /* 建立變數名稱 */
            idx = toolColl.Count(val => val.ToolType == mToolType);
            mVarName = GetToolVariable(idx, toolType);

            /* 建立 TreeNode */
            string name = string.Format("[{0:D2}] {1}", idx, GetToolString(toolType));
			mNode = mainNode.Nodes.Add(name);
			mNode.Tag = this;
			mNodeLv = mNode.Level;
			mNodeIdx = mNode.Index;

            /* 設定 ShowGraphics */
            InitialTool();

			/* 剛剛建構，故有修改 */
			IsModified = true;
		}

		/// <summary>從現有的影像工具進行複製</summary>
		/// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
		/// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
		/// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
		/// <param name="copyPack">欲複製的來源</param>
		internal VisionToolPackBase(List<IVisionToolPack> toolColl, IVisionToolPack copyPack, IAceObjectCollection toolFold, TreeNode mainNode) {
			int idx = toolColl.Count + 1;
			mID = DateTime.Now.ToBinary();

			/* 如果 TreeNode 有父節點，看看父節點是不是影像工具包，是的話就抓出作為 Relative Tool */
			IVisionImageSource imgSrc = null;
			if (mainNode?.Tag != null) {
				IVisionToolPack pack = mainNode.Tag as IVisionToolPack;
				if (pack != null) {
					IVisionTool tool = pack.Tool as IVisionTool;
					if (tool != null) {
						imgSrc = tool.ImageSource;
						if (pack.ToolType != typeof(ICSharpCustomTool)) {
							mInputLinkPack = pack;
							mInputLinkTool = pack.Tool as IVisionTool;
						}
					} else {
						imgSrc = pack.Tool as IVisionImageSource;
					}
				}
			}

			/* 新增工具&資料夾 */
			mToolType = copyPack.ToolType;  //從複製的過來
			mTool = copyPack.Tool.Clone() as IVisionToolBase;
			mFold = toolFold.AddCollection(idx.ToString("00")); //建立新資料夾並放新增的工具進去
			mFold.Add(mTool);

			IVisionTool iTool = mTool as IVisionTool;
			if (iTool != null) {
				iTool.ImageSource = imgSrc; //影像來源
				iTool.GetInputLinks()[0].Reference = mInputLinkTool;   //設定 Relative Tool 
			} else if (mToolType == typeof(IImageProcessingTool)) {
				IImageProcessingTool procTool = mTool as IImageProcessingTool;
				procTool.ImageSource = imgSrc;
				var inputLink = procTool.GetInputLinks();
				if (inputLink != null && inputLink.Length > 0) inputLink[0].Reference = mInputLinkTool;   //設定 Relative Tool 
			}

			/* 建立變數名稱 */
			idx = toolColl.Count(val => val.ToolType == mToolType);
			mVarName = GetToolVariable(idx, mToolType);

			/* 建立 TreeNode */
			string name = string.Format("[{0:D2}] {1}", idx, GetToolString(mToolType));
			mNode = mainNode.Nodes.Add(name);
			mNode.Tag = this;
			mNodeLv = mNode.Level;
			mNodeIdx = mNode.Index;

			/* 讀取設定 */
			mIvdAct = copyPack.ResultUnavailable;
			mRoiCenter = copyPack.ReturnRoiCenter;
			mCmt = copyPack.Comment;

			/* 設定 ShowGraphics */
			InitialTool();

			/* 剛剛建構，故有修改 */
			IsModified = true;
		}

		/// <summary>建構單純的影像工具包，不含 <seealso cref="TreeNode"/> 等父節點</summary>
		/// <param name="toolType">影像工具類型</param>
		/// <param name="source">指定的影像來源</param>
		/// <param name="fold">欲存放此工具的 Ace 資料夾</param>
		internal VisionToolPackBase(VisionToolType toolType, IVisionImageSource source, IAceObjectCollection fold) {
			mID = DateTime.Now.ToBinary();

			/* 建立 Vision tool */
			mToolType = GetToolType(toolType);
			mTool = fold.AddObjectOfType(mToolType, GetToolString(toolType)) as IVisionToolBase;
			IVisionTool tool = mTool as IVisionTool;
			if (tool != null) {
				tool.ImageSource = source;
			} else if (toolType == VisionToolType.ImageProcessing) {
				(mTool as IImageProcessingTool).ImageSource = source;
			}

			/* 因沒有辦法檢視有多少工具，直接信任採縮寫... 如未來有衝突，請從外部排除 */
			mVarName = GetToolVariable(-1, toolType);

			/* 設定 ShowGraphics */
			InitialTool();

			/* 剛剛建構，故有修改 */
			IsModified = true;
		}

		/// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
		/// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
		/// <param name="aceSrv">已連線的 ACE Server 端物件</param>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
		/// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
		internal VisionToolPackBase(XmlElmt xmlData, IAceServer aceSrv, Dictionary<string, string> langMap) {
			/* 拆出 Type */
			string toolType = xmlData.Attribute("Type").Value;
			mToolType = GetToolType(toolType, langMap);

			/* 取得 ID */
			mID = long.Parse(xmlData.Attribute("ID").Value);

			/* 如果是 CVT，則會有 Tag 資訊 */
			XmlAttr attr;
			if (xmlData.Attribute("Sequence", out attr)) {
				if (mTag == null) mTag = new object[] { 0, "00000" };
				int seq = int.Parse(attr.Value);
				(mTag as object[])[0] = seq > 0 ? seq : 0;
			}

			if (xmlData.Attribute("PartNo", out attr)) {
				if (mTag == null) mTag = new object[] { 0, "00000" };
				(mTag as object[])[1] = attr.Value;
			}

			/* 抓 Tool 路徑 */
			string toolPath = xmlData.Element("Path")?.Value;

			if (!string.IsNullOrEmpty(toolPath)) {
				IAceObject toolObj = aceSrv.Root[toolPath];
				if (toolObj != null) {
					mTool = toolObj as IVisionToolBase;
					mFold = mTool.ParentCollection;
					IVisionTool tool = mTool as IVisionTool;
					if (tool != null) {
						if (tool.GetInputLinks().Length > 0) {
							mInputLinkTool = tool.GetInputLinks()[0].Reference;
						}
					}

				} else throw new ArgumentNullException("toolObj", langMap["VisToolNull"]);
			} else throw new ArgumentNullException("toolPath", langMap["PathNull"]);

			/* 抓 & 暫存 Relative Pack */
			XmlElmt childData;
			if (xmlData.Element("InputLink", out childData)) {
				if (!string.IsNullOrEmpty(childData.Value))
					mInputLinkPackID = childData.Value;
			}

			/* 抓住解 */
			mCmt = xmlData.Element("Comment")?.Value;

			/* 抓變數名稱 */
			mVarName = xmlData.Element("Variable")?.Value;

            /* 無結果回應 */
            if (xmlData.Element("ResultUnavailable", out childData)) {
                mIvdAct = (NoResultAction)Enum.Parse(typeof(NoResultAction), childData.Value, true);
            }

            /* 樹節點 */
            if (xmlData.Element("Node", out childData)) {
				/* 建立 TreeNode */
				if (!string.IsNullOrEmpty(childData.Value)) {
					mNode = new TreeNode(childData.Value);
					mNode.Tag = this;
				} else throw new ArgumentNullException("Node", langMap["TreeNodeNull"]);

				/* 去抓 TreeNode 是第幾層與父節點名稱 */
				if (childData.HasAttribute) {
					//抓節點父層的 ID
					attr = childData.Attribute("ParentID");
					if (attr != null && !string.IsNullOrEmpty(attr.Value))
						mInputLinkParentNodeID = long.Parse(attr.Value);

					//抓第幾層
					attr = childData.Attribute("Level");
					if (attr != null) mNodeLv = int.Parse(attr.Value);
					else throw new ArgumentNullException("NodeLvAttr", langMap["TreeLvNull"]);

					//抓第深度
					attr = childData.Attribute("Index");
					if (attr != null) mNodeIdx = int.Parse(attr.Value);
					else throw new ArgumentNullException("NodeLvAttr", langMap["TreeLvNull"]);

				} else throw new ArgumentNullException("NodeAttr", langMap["TreeNodeNull"]);
			}

			/* Offset */
			if (xmlData.Element("/Offset", out childData)) {
				var ofs = childData.Value.Split(CtConst.CHR_COMMA).Select(str => double.Parse(str.Trim()));
				mOfs = new VisionTransform(ofs.ElementAt(0), ofs.ElementAt(1), ofs.ElementAt(2));
			}

			/* ROI Center */
			if (xmlData.Element("/RoiCenter", out childData)) {
				mRoiCenter = bool.Parse(childData.Value);
			}
		}
		#endregion

		#region Private Methods
		/// <summary>遞迴找出 <see cref="TreeNode"/> 及其子節點的 <see cref="TreeNode.Tag"/> 為 T 之物件</summary>
		/// <typeparam name="T">欲尋找的類型</typeparam>
		/// <param name="topNode">欲開始尋找的 <see cref="TreeNode"/></param>
		/// <param name="tarColl">存放符合條件的物件</param>
		private void FilterTagFromNode<T>(TreeNode topNode, List<T> tarColl) {
			if (topNode.Tag is T) tarColl.Add((T)topNode.Tag);
			if (topNode.Nodes.Count > 0) {
				foreach (TreeNode subNode in topNode.Nodes) {
					FilterTagFromNode(subNode, tarColl);
				}
			}
		}
		#endregion

		#region Protected Methods

		/// <summary>初始化影像工具</summary>
		/// <remarks>目前僅設定 <see cref="IVisionToolBase.ShowResultsGraphics"/>，未來有相關的東西都可以加入</remarks>
		protected void InitialTool() {
			/* 顯示結果 */
			mTool.ShowResultsGraphics = true;

			/* 如果是 Blob，額外顯示為 Marker + Blob 區域 */
			if (mToolType == typeof(IBlobAnalyzerTool))
				(mTool as IBlobAnalyzerTool).ResultsDisplay = BlobAnalyzerResultMode.MarkerAndBlobImage;
		}

		/// <summary>從 <see cref="string"/> 轉換為相對應的 <seealso cref="Type"/></summary>
		/// <param name="toolType">含有類型名稱的字串，如 "ILocatorTool"</param>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <returns>相對應的類型</returns>
		/// <exception cref="InvalidCastException">目前不支援的影像工具類型</exception>
		protected Type GetToolType(string toolType, Dictionary<string, string> langMap) {
			Type rspType = typeof(void);
			switch (toolType) {
				case "ICSharpCustomTool":
					rspType = typeof(ICSharpCustomTool);
					break;
				case "ILocatorTool":
					rspType = typeof(ILocatorTool);
					break;
				case "ILocatorModel":
					rspType = typeof(ILocatorModel);
					break;
				case "IBlobAnalyzerTool":
					rspType = typeof(IBlobAnalyzerTool);
					break;
				case "ILineFinderTool":
					rspType = typeof(ILineFinderTool);
					break;
				case "IEdgeLocatorTool":
					rspType = typeof(IEdgeLocatorTool);
					break;
				case "IImageProcessingTool":
					rspType = typeof(IImageProcessingTool);
					break;
				case "IArcFinderTool":
					rspType = typeof(IArcFinderTool);
					break;
				case "IPointFinderTool":
					rspType = typeof(IPointFinderTool);
                    break;
                case "ICalculatedPointTool":
                    rspType = typeof(ICalculatedPointTool);
                    break;
                case "ICalculatedLineTool":
                    rspType = typeof(ICalculatedLineTool);
					break;
                case "ICalculatedArcTool":
                    rspType = typeof(ICalculatedArcTool);
                    break;
                case "ICalculatedFrameTool":
                    rspType = typeof(ICalculatedFrameTool);
                    break;
				default:
					throw new InvalidCastException(langMap["VisToolNotSup"]);
			}
			return rspType;
		}

		/// <summary>將 <see cref="VisionToolType"/> 轉換為相對應的 <seealso cref="IVisionTool"/></summary>
		/// <param name="toolType">欲取得新類型的 <see cref="VisionToolType"/></param>
		/// <returns>相對應的 <seealso cref="IVisionTool"/> 類別</returns>
		/// <remarks>以 Locator 為例，如果使用 loc.GetType() 會得到 LocatorTool 而非 ILocatorTool，故這邊做額外的轉換</remarks>
		protected Type GetToolType(VisionToolType toolType) {
			Type type = null;
			switch (toolType) {
				case VisionToolType.VisionSource:
					type = typeof(IVisionImageSource);
					break;
				case VisionToolType.Locator:
					type = typeof(ILocatorTool);
					break;
				case VisionToolType.CustomVisionTool:
                case VisionToolType.SubCVT:
					type = typeof(ICSharpCustomTool);
					break;
				case VisionToolType.BlobAnalyzer:
					type = typeof(IBlobAnalyzerTool);
					break;
				case VisionToolType.ImageProcessing:
					type = typeof(IImageProcessingTool);
					break;
				case VisionToolType.LocatorModel:
					type = typeof(ILocatorModel);
					break;
				case VisionToolType.EdgeLocator:
					type = typeof(IEdgeLocatorTool);
					break;
				case VisionToolType.LineFinder:
					type = typeof(ILineFinderTool);
					break;
				case VisionToolType.ArcFinder:
					type = typeof(IArcFinderTool);
					break;
				case VisionToolType.PointFinder:
					type = typeof(IPointFinderTool);
					break;
                case VisionToolType.CalculatedPoint:
                    type = typeof(ICalculatedPointTool);
                    break;
                case VisionToolType.CalculatedLine:
                    type = typeof(ICalculatedLineTool);
                    break;
                case VisionToolType.CalculatedArc:
                    type = typeof(ICalculatedArcTool);
                    break;
                case VisionToolType.CalculatedFrame:
                    type = typeof(ICalculatedFrameTool);
                    break;
                default:
                    throw new ArgumentException("未定義影像工具類型");
			}
			return type;
		}

		/// <summary>取得 <see cref="IVisionTool"/> 的自訂名稱</summary>
		/// <param name="toolType">欲取的名稱的影像工具類型</param>
		/// <returns>相對應的自訂名稱</returns>
		protected string GetToolString(VisionToolType toolType) {
			string name = string.Empty;
			switch (toolType) {
				case VisionToolType.Locator:
					name = "Locator";
					break;
				case VisionToolType.CustomVisionTool:
					name = "Custom Vision Tool";
					break;
				case VisionToolType.BlobAnalyzer:
					name = "Blob Analyzer";
					break;
				case VisionToolType.ImageProcessing:
					name = "Image Processing";
					break;
				case VisionToolType.LocatorModel:
					name = "Locator Model";
					break;
				case VisionToolType.EdgeLocator:
					name = "Edge Locator";
					break;
				case VisionToolType.LineFinder:
					name = "Line Finder";
					break;
				case VisionToolType.ArcFinder:
					name = "Arc Finder";
					break;
				case VisionToolType.PointFinder:
					name = "Point Finder";
					break;
                case VisionToolType.CalculatedPoint:
                    name = "Calculated Point";
                    break;
                case VisionToolType.CalculatedLine:
                    name = "Calculated Line";
                    break;
                case VisionToolType.CalculatedArc:
                    name = "Calculated Arc";
                    break;
                case VisionToolType.CalculatedFrame:
                    name = "Calculated Frame";
                    break;
				default:
					name = "Tool";
					break;
			}
			return name;
		}

		/// <summary>取得 <see cref="IVisionTool"/> 的自訂名稱</summary>
		/// <param name="toolType">欲取的名稱的影像工具類型</param>
		/// <returns>相對應的自訂名稱</returns>
		protected string GetToolString(Type toolType) {
			string name = string.Empty;
			switch (toolType.Name) {
				case "ILocatorTool":
					name = "Locator";
					break;
				case "ICSharpCustomTool":
					name = "Custom Vision Tool";
					break;
				case "IBlobAnalyzerTool":
					name = "Blob Analyzer";
					break;
				case "IImageProcessingTool":
					name = "Image Processing";
					break;
				case "ILocatorModel":
					name = "Locator Model";
					break;
				case "IEdgeLocatorTool":
					name = "Edge Locator";
					break;
				case "ILineFinderTool":
					name = "Line Finder";
					break;
				case "IArcFinderTool":
					name = "Arc Finder";
					break;
				case "IPointFinderTool":
					name = "Point Finder";
					break;
                case "ICalculatedPoint":
                    name = "Calculated Point";
                    break;
                case "ICalculatedLine":
                    name = "Calculated Line";
                    break;
                case "ICalculatedArc":
                    name = "Calculated Arc";
                    break;
                case "ICalculatedFrame":
                    name = "Calculated Frame";
                    break;
				default:
					name = "Tool";
					break;
			}
			return name;
		}

		/// <summary>取得相對應 <see cref="IVisionTool"/> 的變數名稱</summary>
		/// <param name="idx">變數索引，此物件是否曾經重複過。如 locator_0、locator_1 等。帶入 -1 表示忽略</param>
		/// <param name="toolType">欲取得物件變數名稱的類型</param>
		/// <returns>相對應類型的變數名稱</returns>
		protected string GetToolVariable(int idx, VisionToolType toolType) {
			string initStr = string.Empty;
			switch (toolType) {
				case VisionToolType.VisionSource:
					initStr = "camera";
					break;
				case VisionToolType.Locator:
					initStr = "locator";
					break;
				case VisionToolType.CustomVisionTool:
					initStr = "cvt";
					break;
				case VisionToolType.BlobAnalyzer:
					initStr = "blob";
					break;
				case VisionToolType.ImageProcessing:
					initStr = "imgProc";
					break;
				case VisionToolType.EdgeLocator:
					initStr = "edge";
					break;
				case VisionToolType.LineFinder:
					initStr = "lineFind";
					break;
				case VisionToolType.ArcFinder:
					initStr = "arcFind";
					break;
				case VisionToolType.PointFinder:
					initStr = "pointFind";
					break;
                case VisionToolType.CalculatedPoint:
                    initStr = "calPoint";
                    break;
                case VisionToolType.CalculatedLine:
                    initStr = "calLine";
                    break;
                case VisionToolType.CalculatedArc:
                    initStr = "calArc";
                    break;
                case VisionToolType.CalculatedFrame:
                    initStr = "calFrame";
                    break;
				default:
					break;
			}
			return idx > -1 ? string.Format("{0}_{1}", initStr, idx.ToString()) : initStr;  //負數就直接回傳縮寫吧，外面自行負責
		}

		/// <summary>取得相對應 <see cref="IVisionTool"/> 的變數名稱</summary>
		/// <param name="idx">變數索引，此物件是否曾經重複過。如 locator_0、locator_1 等。帶入 -1 表示忽略</param>
		/// <param name="toolType">欲取得物件變數名稱的類型</param>
		/// <returns>相對應類型的變數名稱</returns>
		protected string GetToolVariable(int idx, Type toolType) {
			string initStr = string.Empty;
			switch (toolType.Name) {
				case "IVisionImageSource":
					initStr = "camera";
					break;
				case "ILocatorTool":
					initStr = "locator";
					break;
				case "ICSharpCustomTool":
					initStr = "cvt";
					break;
				case "IBlobAnalyzerTool":
					initStr = "blob";
					break;
				case "IImageProcessingTool":
					initStr = "imgProc";
					break;
				case "IEdgeLocatorTool":
					initStr = "edge";
					break;
				case "ILineFinderTool":
					initStr = "lineFind";
					break;
				case "IArcFinderTool":
					initStr = "arcFind";
					break;
				case "IPointFinderTool":
					initStr = "pointFind";
                    break;
                case "ICalculatedPoint":
                    initStr = "calPoint";
                    break;
                case "ICalculatedLine":
                    initStr = "calLine";
					break;
                case "ICalculatedArc":
                    initStr = "calArc";
                    break;
                case "ICalculatedFrame":
                    initStr = "calFrame";
                    break;
				default:
					break;
			}
			return idx > -1 ? string.Format("{0}_{1}", initStr, idx.ToString()) : initStr;  //負數就直接回傳縮寫吧，外面自行負責
		}

		/// <summary>建立兩欄的 <see cref="DataGridViewRow"/>，並填入第一格的數值(名稱)</summary>
		/// <param name="dgv">欲顯示欄位的 <see cref="DataGridView"/></param>
		/// <param name="name">欲顯示欄位的名稱字串</param>
		/// <param name="tip">此列所欲顯示的提示字串</param>
		/// <returns>列</returns>
		protected DataGridViewRow CreateNewRow(DataGridView dgv, string name, string tip = "") {
			DataGridViewRow row = new DataGridViewRow();
			row.CreateCells(dgv, name);
			row.Tag = tip;
			return row;
		}

		/// <summary>數字型態的輸入器，僅供 <see cref="int"/>, <seealso cref="short"/>, <seealso cref="float"/> 與 <seealso cref="double"/></summary>
		/// <typeparam name="T">實質型態數值，如 <see cref="int"/>, <see cref="double"/> 等</typeparam>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <param name="curVal">當前數值</param>
		/// <param name="group">共需多少筆資料</param>
		/// <param name="minVal">最小數值</param>
		/// <param name="maxVal">最大數值</param>
		/// <param name="newVal">回傳使用者輸入的新數值</param>
		/// <returns>是否有完成輸入  (<see langword="false"/>)使用者取消 (<see langword="true"/>)使用者輸入合法數值</returns>
		protected bool ValueEditor<T>(Dictionary<string, string> langMap, string curVal, int group, T minVal, T maxVal, out List<T> newVal) where T : struct {
			bool verify = true;
			List<T> outVal = new List<T>(); //接收 ICtInput 回傳

			string inVal;
			verify = CtInput.Text(out inVal, langMap["NormalEditTitle"], string.Format(langMap["ValEditDesc"].Replace(@"\r\n", "\r\n"), group.ToString(), minVal, maxVal), curVal) == Stat.SUCCESS;

			/* 如果使用者按下確認鍵才進行數值驗證 */
			if (verify) {
				string[] split = inVal.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
				if (split.Length == group) {
					if (typeof(T) == typeof(int)) {
						int intMin = Convert.ToInt32(minVal);
						int intMax = Convert.ToInt32(maxVal);
						verify = split.All(val => { int temp; return int.TryParse(val.Trim(), out temp); });
						if (verify) {
							outVal = split.ToList().ConvertAll(val => (T)(object)int.Parse(val.Trim()));
							verify = outVal.All(val => { int temp = Convert.ToInt32(val); return intMin <= temp && temp <= intMax; });
							if (!verify) CtMsgBox.Show(langMap["ValEditInv"], langMap["ValEditOutOfRng"], MsgBoxBtn.OK, MsgBoxStyle.Error);
						} else CtMsgBox.Show(langMap["ValEditInv"], langMap["ValEditInt"], MsgBoxBtn.OK, MsgBoxStyle.Error);
					} else if (typeof(T) == typeof(short)) {
						short sorMin = Convert.ToInt16(minVal);
						short sorMax = Convert.ToInt16(maxVal);
						verify = split.All(val => { short temp; return short.TryParse(val.Trim(), out temp); });
						if (verify) {
							outVal = split.ToList().ConvertAll(val => (T)(object)short.Parse(val.Trim()));
							verify = outVal.All(val => { int temp = Convert.ToInt32(val); return sorMin <= temp && temp <= sorMax; });
							if (!verify) CtMsgBox.Show(langMap["ValEditInv"], langMap["ValEditOutOfRng"], MsgBoxBtn.OK, MsgBoxStyle.Error);
						} else CtMsgBox.Show(langMap["ValEditInv"], langMap["ValEditInt"], MsgBoxBtn.OK, MsgBoxStyle.Error);
					} else if (typeof(T) == typeof(float)) {
						float sngMin = Convert.ToSingle(minVal);
						float sngMax = Convert.ToSingle(maxVal);
						verify = split.All(val => { float temp; return float.TryParse(val.Trim(), out temp); });
						if (verify) {
							outVal = split.ToList().ConvertAll(val => (T)(object)float.Parse(val.Trim()));
							verify = outVal.All(val => { float temp = Convert.ToSingle(val); return sngMin <= temp && temp <= sngMax; });
							if (!verify) CtMsgBox.Show(langMap["ValEditInv"], langMap["ValEditOutOfRng"], MsgBoxBtn.OK, MsgBoxStyle.Error);
						} else CtMsgBox.Show(langMap["ValEditInv"], langMap["ValEditReal"], MsgBoxBtn.OK, MsgBoxStyle.Error);
					} else if (typeof(T) == typeof(double)) {
						double dblMin = Convert.ToDouble(minVal);
						double dblMax = Convert.ToDouble(maxVal);
						verify = split.All(val => { double temp; return double.TryParse(val.Trim(), out temp); });
						if (verify) {
							outVal = split.ToList().ConvertAll(val => (T)(object)double.Parse(val.Trim()));
							verify = outVal.All(val => { double temp = Convert.ToDouble(val); return dblMin <= temp && temp <= dblMax; });
							if (!verify) CtMsgBox.Show(langMap["ValEditInv"], langMap["ValEditOutOfRng"], MsgBoxBtn.OK, MsgBoxStyle.Error);
						} else CtMsgBox.Show(langMap["ValEditInv"], langMap["ValEditReal"], MsgBoxBtn.OK, MsgBoxStyle.Error);
					}
				} else {
					verify = false;
					CtMsgBox.Show(langMap["ValEditInv"], langMap["ValEditGrp"], MsgBoxBtn.OK, MsgBoxStyle.Error);
				}
			}

			newVal = outVal;
			return verify;
		}

		/// <summary>布林 true、false 選擇視窗</summary>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <param name="curVal">當前數值</param>
		/// <param name="newVal">使用者回傳的新數值</param>
		/// <returns>是否有完成輸入  (<see langword="false"/>)使用者取消 (<see langword="true"/>)使用者輸入合法數值</returns>
		protected bool BoolEditor(Dictionary<string, string> langMap, bool curVal, out bool newVal) {
			bool verify = false;
			bool outVal = curVal;

			string inVal;
			var valMap = new Dictionary<bool, string> { { true, langMap["ValBoolTrue"] }, { false, langMap["ValBoolFalse"] } };
			verify = CtInput.ComboBoxList(out inVal, langMap["NormalEditTitle"], langMap["NormalEditDesc"], valMap.Values, valMap[curVal]) == Stat.SUCCESS;

			if (verify) {
				var pair = valMap.FirstOrDefault(kvp => kvp.Value.Equals(inVal));
				outVal = pair.Key;
			}

			newVal = outVal;
			return verify;
		}

		/// <summary>列舉選擇視窗</summary>
		/// <typeparam name="T">列舉</typeparam>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <param name="curVal">當前數值</param>
		/// <param name="newVal">使用者回傳的新數值</param>
		/// <returns>是否有完成輸入  (<see langword="false"/>)使用者取消 (<see langword="true"/>)使用者輸入合法數值</returns>
		protected bool EnumEditor<T>(Dictionary<string, string> langMap, string curVal, out T newVal) where T : struct {
			bool verify = false;
			T outEnum = (T)Enum.Parse(typeof(T), curVal);

			var enumMap = Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(e => e, e => langMap[e.GetFullName()]);
            string inVal = string.Empty;

            //string curStr = langMap[curVal]; curVal已經是當前語系對應字串，不須再找一次 by Jay 2017/06/09
            string curStr = curVal;
            verify = CtInput.ComboBoxList(out inVal, langMap["NormalEditTitle"], langMap["NormalEditDesc"], enumMap.Values, curStr) == Stat.SUCCESS;

			if (verify) {
				var pair = enumMap.FirstOrDefault(kvp => kvp.Value.Equals(inVal));
				outEnum = pair.Key;
			}

			newVal = outEnum;
			return verify;
		}

		/// <summary>影像來源選擇視窗</summary>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <param name="curVal">當前來源</param>
		/// <param name="newPath">使用者回傳的新數值</param>
		/// <param name="showNone">是否顯示 "None" 選項</param>
		/// <returns>是否有完成輸入  (<see langword="false"/>)使用者取消 (<see langword="true"/>)使用者輸入合法數值</returns>
		protected bool ImageSourceEditor(Dictionary<string, string> langMap, string curVal, out string newPath, bool showNone = false) {
			bool verify = false;
			string outVal = string.IsNullOrEmpty(curVal) ? "None" : (curVal.ToLower() == "null" ? "None" : curVal);

			/*-- 列出可供選擇的來源 --*/
			/* ACE 內的僅列出 CCD */
			List<string> srcList = mTool.AceServer.Root.FilterType(typeof(IVisionImageVirtualCamera)).Select(src => src.FullPath).ToList();
			if (showNone) srcList.Insert(0, "None");
			/* 找出專案內有那些 ImageSource */
			var packColl = FilterTagFromNode<IVisionToolPack>()
							.FindAll(pack => pack.Tool is IVisionImageSource && !(pack.Tool is ICSharpCustomTool))
							.ToDictionary(pack => pack.Node.Text, pack => pack);
			/* 如果有專案內的，加入清單 */
			if (packColl != null && packColl.Count > 0) srcList.AddRange(packColl.Keys.ToList());

			/*-- 檢查當前來源是否有在專案清單內 --*/
			var chkKvp = packColl.FirstOrDefault(kvp => kvp.Value.Tool.FullPath == curVal);
			if (chkKvp.Value != null) outVal = chkKvp.Key;

			/*-- 顯示對話視窗 --*/
			string inVal;
			verify = CtInput.ComboBoxList(out inVal, langMap["NormalEditTitle"], langMap["NormalEditDesc"], srcList, outVal) == Stat.SUCCESS;

			/*-- 如果是按下確定，儲存之 --*/
			if (verify) {
				/* 如果再專案清單內，轉成對應的路徑 */
				if (packColl.ContainsKey(inVal)) outVal = packColl[inVal].Tool.FullPath;
				else outVal = inVal;
			}

			newPath = outVal;
			return verify;
		}

		/// <summary>沒有結果時之動作選擇視窗 "Do Nothing", "Exit Script", "As NG Product"</summary>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <param name="curVal">當前數值</param>
		/// <param name="act">使用者回傳的新數值</param>
		/// <returns>是否有完成輸入  (<see langword="false"/>)使用者取消 (<see langword="true"/>)使用者輸入合法數值</returns>
		protected bool NoResultEditor(Dictionary<string, string> langMap, NoResultAction curVal, out NoResultAction act) {
			bool verify = false;
			NoResultAction outVal = curVal;

			var nameList = Enum.GetValues(typeof(NoResultAction)).Cast<NoResultAction>().ToDictionary(e => e, e => langMap[e.GetFullName()]);

			string inVal, curStr = langMap[curVal.GetFullName()];
			verify = CtInput.ComboBoxList(out inVal, langMap["NormalEditTitle"], langMap["NormalEditDesc"], nameList.Values, curStr) == Stat.SUCCESS;

			if (verify) {
				outVal = nameList.FirstOrDefault(kvp => kvp.Value.Equals(inVal)).Key;
			}

			act = outVal;
			return verify;
		}

		/// <summary>尋找樹狀的最頂端節點</summary>
		/// <returns>尋找到的節點</returns>
		protected TreeNode FindTopNode() {
			TreeNode tempNode = mNode;
			do {
				if (tempNode.Parent == null) break;
				else tempNode = tempNode.Parent;
			} while (true);
			return tempNode;
		}

		/// <summary>利用遞迴方式尋找 <see cref="TreeNode.Tag"/> 是否有符合的 T 類型物件</summary>
		/// <typeparam name="T">欲尋找的物件類型</typeparam>
		/// <returns>尋找到的結果集合</returns>
		protected List<T> FilterTagFromNode<T>() {
			TreeNode topNode = FindTopNode();
			List<T> tempColl = new List<T>();
			FilterTagFromNode(topNode, tempColl);
			return tempColl;
		}

		/// <summary>取得工具對應的<see cref="Ace.HSVision.Client.ImageDisplay.MarkerColor"/> 顏色字串</summary>
		/// <returns>顏色字串</returns>
		protected string GetMarkerColor() {
			string color = string.Empty;

            //將使用到CustomColor的程式碼改為從變數取得全名字串，而非寫死
            //未來即使更改CustomColor的變數名稱可也快速的知道哪裡忘了改
            //再透過取代的方式將所有錯誤的變數名稱修正
            //by Jay 2017/06/09
            switch (mToolType.Name) {
                case "ICSharpCustomTool":
                    //by Jay 2017/06/28
                    //預設顏色為藍色
                    //有Relatived Tool 則以Relatived Tool顏色為主
                    color = mInputLinkTool == null ? "MarkerColor.Blue" : (mInputLinkPack as VisionToolPackBase).GetMarkerColor();
                    break;
                case "ILocatorTool":
                    color = $"(MarkerColor){CustomColor.LIGHT_BLUE.GetFullName()}";
                    break;
                case "IBlobAnalyzerTool":
                    color = $"(MarkerColor){CustomColor.WINE_RED.GetFullName()}";
                    break;
                case "ILineFinderTool":
                    color = $"(MarkerColor){CustomColor.OLIVE.GetFullName()}";
                    break;
                case "IEdgeLocatorTool":
                    color = "MarkerColor.Orange";
                    break;
                case "IArcFinderTool":
                    color = $"(MarkerColor){CustomColor.TEAL.GetFullName()}";
                    break;
                case "IPointFinderTool":
                    color = "MarkerColor.Cyan";
                    break;
                default:
                    color = $"(MarkerColor){CustomColor.PURPLE.GetFullName()}";
                    break;
            }
            return color;
		}

		/// <summary>取得 <see cref="bool"/> 之多國語系字串</summary>
		/// <param name="langMap">多國語系對應清單</param>
		/// <param name="tar">欲轉換之布林值</param>
		/// <returns>多國語系</returns>
		protected string GetBoolString(Dictionary<string, string> langMap, bool tar) {
			return tar ? langMap["ValBoolTrue"] : langMap["ValBoolFalse"];
		}

        ///<summary>產生角度CVT程式碼</summary>
        ///<param name="code">程式碼主體</param>
        ///<returns>是否以產生CVT程式碼</returns>
        protected void generateAngleCVT(List<string> code) {
            code.Add($"\tVisionTransform pos = {VariableName}.GetTransformResults()[0];");
        }
        
        #endregion

        #region IVisionToolPack Implements
        /// <summary>更換 <see cref="TreeNode"/></summary>
        /// <param name="node">欲更換的節點</param>
        public void AssignTreeNode(TreeNode node) {
			mNode = node;
			mNodeLv = mNode.Level;
			mNodeIdx = mNode.Index;
		}

		/// <summary>更改資料夾名稱</summary>
		/// <param name="name">欲更改的名稱</param>
		public void RenameFolder(string name) {
			if (!string.IsNullOrEmpty(name)) {
				mFold.Name = name;
				mFold.CheckInternalReferences();
			}
		}

		/// <summary>產生可適用於 <see cref="ICvtExecutor"/> 內的建構程式碼</summary>
		/// <param name="cvt"><see cref="ICSharpCustomTool"/> 變數名稱</param>
		/// <param name="ng">不合格的 ROI 顏色</param>
		/// <returns>建構程式</returns>
		public List<string> GenerateExecutionConstruct(string cvt, string ng) {
			List<string> code = new List<string>();
			string color = GetMarkerColor();
			/* 檢查當前是否有 Relative */
			if (mInputLinkPack != null && !(mInputLinkPack is CustomVisionToolPack) && !(mTool is IVisionImageSource) && !(this is ICalculatedToolPack)) {
				/* 暫存 Offset 數值 */
				string ofs = string.Empty;

				/* 有的話抓出 Offset */
				IArcRoiTool arcTool = mTool as IArcRoiTool;
				if (arcTool != null) {
					ofs = $"{arcTool.SearchRegion.Center.X:F3}, {arcTool.SearchRegion.Center.Y:F3}, {arcTool.SearchRegion.Rotation:F3}";
				} else {
                    //增加SubCVT Ofs，預設為0,0,0 by Jay 2017/06/28
					var ofsVT = (this is SubCustomVisionToolPack) ?
                        new VisionTransform(0, 0, 0):
                        mTool.GetType().GetProperty("Offset")?.GetValue(mTool) as VisionTransform;
					if (ofsVT != null) ofs = $"{ofsVT.X:F3}, {ofsVT.Y:F3}, {ofsVT.Degrees:F3}";
					else ofs = "null, null, null";
				}

                /* 組合 */
                code.Add($"new VisionExecutor({mVarName}, {mInputLinkPack.VariableName}, {ofs}, {cvt}, {GetMarkerColor()}, {ng}, {mIvdAct.GetFullName()})");//改為回傳enum變數全名
            } else
                /* 如果沒有 Relative 就直接回傳 null 吧 */
                code.Add($"new VisionExecutor({mVarName}, null, null, null, null, {cvt}, {GetMarkerColor()}, {ng},  {mIvdAct.GetFullName()})");//改為回傳enum變數全名 by Jay 2016/06/09
            return code;
		}

		/// <summary>如有設定 Relative，檢查並還原之</summary>
		public void RecoverRelativeSetting() {
			if (mInputLinkPack == null) return;

			IVisionTool tool = mTool as IVisionTool;
			if (tool != null && mInputLinkPack != null) {
				InputParameterLink inputs = tool.GetInputLinks()[0];
				if (inputs.Reference == null) {
					inputs.Reference = mInputLinkTool ?? (mInputLinkPack.Tool as IVisionTool);

					IArcRoiTool arcTool = tool as IArcRoiTool;
					if (arcTool != null) {
						VisionArc oriSR = arcTool.SearchRegion;
						VisionArc newSR = new VisionArc(mOfs.X, mOfs.Y, oriSR.Radius, oriSR.Thickness, mOfs.Degrees, oriSR.Opening);
						arcTool.SearchRegion = newSR;
					} else {
						if (!(this is ICalculatedToolPack) ) mTool.GetType().GetProperty("Offset").SetValue(tool, mOfs);
					}
				}
			}
		}

		/// <summary>如有設定 Relative，檢查並還原來源 <see cref="IVisionToolPack"/></summary>
		/// <param name="toolColl">影像工具集合</param>
		public void RecoverRelativePack(IEnumerable<IVisionToolPack> toolColl) {
			if (!string.IsNullOrEmpty(mInputLinkPackID)) {
				long tarID = long.Parse(mInputLinkPackID);
				var parentPack = toolColl.FirstOrDefault(obj => obj.ID == tarID);
				if (parentPack != null) {
					mInputLinkPack = parentPack;
					mInputLinkTool = parentPack.Tool as IVisionTool;
				}
			}
		}

		/// <summary>更新 <see cref="TreeNode"/> 相關資訊，<see cref="TreeNode.Level"/>、<see cref="TreeNode.Index"/> 等</summary>
		public void UpdateTreeNodeInformation() {
			mNodeIdx = mNode.Index;
			mNodeLv = mNode.Level;
		}

		/// <summary>更新變數名稱</summary>
		/// <param name="idx">欲更改的索引</param>
		public void RefreshVariable(int idx) {
			mVarName = GetToolVariable(idx, mToolType);
		}

		/// <summary>檢查移除的物件是否與此工具有關聯，如有關聯則取消之</summary>
		/// <param name="tool">移除的物件</param>
		public void ConfirmRemovedLink(IVisionProjectable tool) {
			if (mInputLinkPack != null && mInputLinkPack.ID == tool.ID) {
				mInputLinkPack = null;
				mInputLinkTool = null;
			}
		}
        
		#endregion

		#region IPropertable Ulities
		#region Image Source
		/// <summary>取得 <see cref="IVisionImageSource"/> 之完整路徑</summary>
		/// <returns>路徑</returns>
		private string GetImageSourceFullpath() {
			var imgSrc = mTool.GetType().GetProperty("ImageSource")?.GetValue(mTool) as IVisionImageSource;
			return imgSrc?.FullPath ?? string.Empty;
		}

		/// <summary>建立 <see cref="IVisionImageSource"/> 之 <see cref="PropertyView"/></summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns><see cref="PropertyView"/></returns>
		protected PropertyView CreateImageSourceView(Dictionary<string, string> langMap) {
			return new PropertyView(
				langMap["PropImgSrc"],
				AccessLevel.None,
				langMap["TipImgSrc"],
				"Image Source",
				GetImageSourceFullpath(),
				() => {
					string imgSrc, oriSrc = GetImageSourceFullpath();
					if (ImageSourceEditor(langMap, oriSrc, out imgSrc)) {
						if (string.IsNullOrEmpty(imgSrc) || "None".Equals(imgSrc)) {
							mTool.GetType().GetProperty("ImageSource").SetValue(mTool, null);
						} else {
							IVisionImageSource src = mTool.AceServer.Root[imgSrc] as IVisionImageSource;
							mTool.GetType().GetProperty("ImageSource").SetValue(mTool, src);
						}
						this.IsModified = true;
					} else imgSrc = oriSrc; //避免 return imgSrc 把原本的東西洗掉，所以 re-assign 一次
					return imgSrc;
				}
			);
		}
		#endregion

		#region Comments
		/// <summary>建立註解之 <see cref="PropertyView"/></summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns><see cref="PropertyView"/></returns>
		protected PropertyView CreateCommentView(Dictionary<string, string> langMap) {
			return new PropertyView(
				langMap["PropCmt"],
				AccessLevel.None,
				langMap["TipCmt"],
				"Comments",
				mCmt,
				() => {
					string cmt = string.Empty;
					if (CtInput.Text(out cmt, langMap["Cmt"], langMap["CmtEnt"], mCmt) == Stat.SUCCESS) {
						mCmt = cmt;
						this.IsModified = true;
					} else cmt = mCmt;
					return cmt;
				}
			);
		}
		#endregion

		#region Result Available
		/// <summary>建立無結果動作之 <see cref="PropertyView"/></summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns><see cref="PropertyView"/></returns>
		protected PropertyView CreateResultAvailableView(Dictionary<string, string> langMap) {
			return new PropertyView(
				langMap["PropRetVia"],
				AccessLevel.None,
				langMap["TipRetNotVia"],
				"Results Not Available",
				langMap[mIvdAct.GetFullName()],
				() => {
					NoResultAction act = mIvdAct;
					string actStr = langMap[mIvdAct.GetFullName()];
					if (NoResultEditor(langMap, mIvdAct, out act)) {
						mIvdAct = act;
						actStr = langMap[mIvdAct.GetFullName()];
						this.IsModified = true;
						this.IsCompiled = false;
					}
					return actStr;
				}
			);
		}
		#endregion

		#region Show Results Graphics
		/// <summary>建立顯示結果圖像之 <see cref="PropertyView"/></summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns><see cref="PropertyView"/></returns>
		protected PropertyView CreateShowResultView(Dictionary<string, string> langMap) {
			return new PropertyView(
				langMap["PropShowRet"],
				AccessLevel.None,
				langMap["TipShowGraph"],
				"Show Results Graphics",
				GetBoolString(langMap, mTool.ShowResultsGraphics),
				() => {
					bool showGraphEnb = mTool.ShowResultsGraphics;
					if (BoolEditor(langMap, showGraphEnb, out showGraphEnb)) {
						mTool.ShowResultsGraphics = showGraphEnb;
						this.IsModified = true;
					}
					return GetBoolString(langMap, showGraphEnb);
				}
			);
		}
		#endregion

		#region Return ROI Center
		/// <summary>建立回傳 ROI 中心之 <see cref="PropertyView"/></summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns><see cref="PropertyView"/></returns>
		protected PropertyView CreateRoiView(Dictionary<string, string> langMap) {
			return new PropertyView(
				langMap["PropRoiCent"],
				AccessLevel.None,
				langMap["TipRoiCent"],
				"Return ROI Center",
				GetBoolString(langMap, mRoiCenter),
				() => {
					bool roiCent;
					if (BoolEditor(langMap, mRoiCenter, out roiCent)) {
						mRoiCenter = roiCent;
						this.IsModified = true;
						this.IsCompiled = false;
					}
					return GetBoolString(langMap, mRoiCenter);
				}
			);
		}
		#endregion

		#endregion

		#region IPropertable Implements
		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public abstract List<PropertyView> CreateDataSource(Dictionary<string, string> langMap);
		#endregion

		#region IXmlSavable Implements
		/// <summary>產生物件的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		public abstract XmlElmt CreateXmlData(string nodeName);

		/// <summary>提供基礎的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		protected XmlElmt GenerateXmlData(string nodeName) {
			/* 檢查一下 Node 資訊 */
			if (mNodeLv == -1 && mNode.Level != -1) mNodeLv = mNode.Level;
			if (mNodeIdx == -1 && mNode.Index != -1) mNodeIdx = mNode.Index;

			List<XmlAttr> attrColl = new List<XmlAttr>();

            //將Type屬性改為透過Virtual Method GetTypeAttr取得
            //一般屬性為AceToolName
            //自訂義工具類型則覆寫GetTypeAttr
            //attrColl.Add(new XmlAttr("Type", mToolType?.Name ?? string.Empty));
            attrColl.Add(new XmlAttr("Type", mToolType?.Name ?? string.Empty));

            attrColl.Add(new XmlAttr("ID", mID.ToString()));
			if (mToolType == typeof(ICSharpCustomTool) && mTag != null) {
				object[] tag = mTag as object[];
				attrColl.Add(new XmlAttr("Sequence", tag[0].ToString()));
				attrColl.Add(new XmlAttr("PartNo", tag[1].ToString()));
			}
			List<XmlElmt> dataColl = new List<XmlElmt>();
			dataColl.Add(new XmlElmt("Comment", mCmt));
			dataColl.Add(new XmlElmt("InputLink", mInputLinkPack?.ID.ToString() ?? string.Empty));
			dataColl.Add(
				new XmlElmt(
					"Node",
					mNode?.Text ?? string.Empty,
					new XmlAttr("ParentID", (mNode.Parent?.Tag as IVisionProjectable)?.ID.ToString() ?? string.Empty),
					new XmlAttr("Level", mNodeLv.ToString()),
					new XmlAttr("Index", mNodeIdx.ToString())
				)
			);
			dataColl.Add(new XmlElmt("Path", mTool?.FullPath ?? string.Empty));
			dataColl.Add(new XmlElmt("ResultUnavailable", mIvdAct.ToString()));
			dataColl.Add(new XmlElmt("Variable", mVarName));
			dataColl.Add(new XmlElmt("Offset", $"{mOfs.X:F3}, {mOfs.Y:F3}, {mOfs.Degrees:F3}"));
			dataColl.Add(new XmlElmt("RoiCenter", mRoiCenter.ToString()));

			return new XmlElmt(
				nodeName,
				attrColl,
				dataColl
			);
		}

        ///<summary>產生XMLType屬性內容</summary>
        protected virtual string GetTypeAttr() {
            return mToolType?.Name ?? string.Empty;
        }

        #endregion

        #region IDisposable Implements
        /// <summary>指出是否已經釋放過資源</summary>
        protected bool mDisposed = false;

		/// <summary>釋放此影像工具資源</summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>釋放此影像工具資源之內容</summary>
		protected virtual void Dispose(bool disposing) {
			if (!mDisposed && disposing) {
				mDisposed = true;
				mFold = null;
				mTool = null;
				mInputLinkTool = null;
				mInputLinkPack = null;
				mToolType = null;
				mNode = null;
				mTag = null;
				mOfs = null;
				mOfsShd = null;
				mInputLinkPackID = null;
				mInputLinkParentNodeID = null;
			}
		}

		/// <summary>解構子</summary>
		~VisionToolPackBase() {
			Console.WriteLine("Disposing : " + mID.ToString());
			Dispose(true);
		}
		#endregion
	}

	/// <summary>CVT 工具包</summary>
	public class CustomVisionToolPack : VisionToolPackBase, IResultable {

		#region Fields
		private string mPreLightVar = string.Empty;
		private bool mPreLightEnb = false;
		private string mSaveOkVar = string.Empty;
		private string mCali = string.Empty;
        private TreeNodeMeth mTreeNodeMeth = new TreeNodeMeth();
		#endregion

		#region Properties
		/// <summary>取得當前設定的僅切換調光器數值之 V+ 變數。如果不啟用請保持為空</summary>
		public string PreLightVariable { get { return mPreLightVar; } }
		/// <summary>取得是否啟用僅切換調光器之功能</summary>
		public bool IsPreLight { get { return mPreLightEnb; } }
		/// <summary>取得是否需要儲存合格的圖片</summary>
		public bool IsSaveOkImage { get { return !string.IsNullOrEmpty(mSaveOkVar); } }
		/// <summary>取得儲存合格圖片的 V+ 變數。如不啟用請保持為空</summary>
		public string SaveOkImageVariable { get { return mSaveOkVar; } }
		/// <summary>取得 Calibration 物件路徑</summary>
		public string Calibration { get { return mCali; } }
		#endregion

		#region Constructors
		/// <summary>建立影像工具包於指定的 Ace 資料夾，附帶建立 <see cref="TreeNode"/></summary>
		/// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
		/// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
		/// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
		public CustomVisionToolPack(
			List<IVisionToolPack> toolColl,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base(toolColl, VisionToolType.CustomVisionTool, toolFold, mainNode) {

		}

		/// <summary>從現有的影像工具進行複製</summary>
		/// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
		/// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
		/// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
		/// <param name="copyPack">欲複製的來源</param>
		public CustomVisionToolPack(
			List<IVisionToolPack> toolColl,
			IVisionToolPack copyPack,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base(toolColl, copyPack, toolFold, mainNode) {

		}

		/// <summary>建構單純的影像工具包，不含 <seealso cref="TreeNode"/> 等父節點</summary>
		/// <param name="source">指定的影像來源</param>
		/// <param name="fold">欲存放此工具的 Ace 資料夾</param>
		public CustomVisionToolPack(
			IVisionImageSource source,
			IAceObjectCollection fold
		) : base(VisionToolType.CustomVisionTool, source, fold) {

		}

		/// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
		/// <param name="aceSrv">已連線的 ACE Server 端物件</param>
		/// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
		/// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
		public CustomVisionToolPack(
			Dictionary<string, string> langMap,
			XmlElmt xmlData,
			IAceServer aceSrv
		) : base(xmlData, aceSrv, langMap) {

			/* 抓 PreLight */
			mPreLightVar = xmlData.Element("PreLight/VpGlobVar")?.Value;
			mPreLightEnb = bool.Parse(xmlData.Element("PreLight/Enabled")?.Value ?? "false");

			mSaveOkVar = xmlData.Element("SaveOkImg")?.Value;

			mCali = xmlData.Element("Calibration")?.Value;
		}

		/// <summary>建立 <see cref="ILocatorModel"/> 之工具包，請勿加入 IVisionToolPack 集合物件，避免偵測時失誤</summary>
		/// <param name="source">指定的影像來源</param>
		/// <param name="toolFold">指定存放的 Ace 資料夾</param>
		/// <param name="mainNode">存放含有 <see cref="ILocatorTool"/> 的 <seealso cref="IVisionToolPack"/> 之 <seealso cref="TreeNode"/></param>
		public CustomVisionToolPack(
			IVisionImageSource source,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base() {

			/* 直接指定建立 ILocatorModel */
			mToolType = GetToolType(VisionToolType.LocatorModel);
			mFold = toolFold;
			mTool = mFold.AddObjectOfType(mToolType, GetToolString(VisionToolType.LocatorModel)) as IVisionTool;

			IVisionTool tool = mTool as IVisionTool;
			if (tool != null) tool.ImageSource = source;
			else if (mToolType == typeof(IImageProcessingTool)) (mTool as IImageProcessingTool).ImageSource = source;

			/* Model 不須變數名稱 */
			mVarName = string.Empty;

			/* 建立 TreeNode */
			mNode = mainNode.Nodes.Add(GetToolString(VisionToolType.LocatorModel));
			mNode.Tag = this;
			mNodeLv = mNode.Level;

			/* 確認是否父節點有影像工具包 */
			if (mainNode.Tag != null) {
				IVisionToolPack pack = mainNode.Tag as IVisionToolPack;
				if (pack != null) mInputLinkTool = pack.Tool as IVisionTool;
			}

			/* 設定 ShowGraphics */
			InitialTool();

		}
		#endregion

		#region Private Methods
		/// <summary>搜尋當前 <see cref="TreeView"/> 裡有多少的 <see cref="IVisionToolPack"/></summary>
		/// <param name="node">當前使用的節點</param>
		/// <param name="nodeColl">回傳搜尋到的 <see cref="IVisionToolPack"/></param>
		private void SearchVisionToolPack(TreeNode node, ref List<IVisionToolPack> nodeColl) {
			/* 找到的就先加入集合 */
			if (node.Tag is IVisionToolPack) {
				IVisionToolPack pack = node.Tag as IVisionToolPack;
				if (pack.ToolType != typeof(ILocatorModel) && pack.ToolType != typeof(ICSharpCustomTool)) nodeColl.Add(pack);
			}

			/* 繼續往下找 */
			if (node.Nodes.Count > 0) {
				foreach (TreeNode subNode in node.Nodes) {
					mTreeNodeMeth.SearchVisionToolPack(subNode, ref nodeColl);
				}
			}
		}

		/// <summary>檢查特定的 V+ 變數是否存在</summary>
		/// <param name="name">欲檢查的 V+ 變數名稱</param>
		/// <returns>(<see langword="true"/>)存在 (<see langword="false"/>)不存在</returns>
		private bool CheckRealVp(string name) {
			IVpLinkedObject ctrl = mTool.AceServer.Root.FilterType(typeof(IVpLinkedObject), true)?[0] as IVpLinkedObject;
			if (ctrl != null) {
				var vpVar = ctrl.Memory.Variables.Variables.FirstOrDefault(val => val.Name == name);
				return (vpVar != null && vpVar.VariableType == Ace.Adept.Server.Controls.VPlusGlobalVariableType.Real);
			} else return false;
		}

		/// <summary>建立 Real 變數</summary>
		/// <param name="name">欲建立的 V+ 變數名稱</param>
		private void CreateRealVp(string name) {
			IVpLinkedObject ctrl = mTool.AceServer.Root.FilterType(typeof(IVpLinkedObject), true)?[0] as IVpLinkedObject;
			if (ctrl != null) {
				ctrl.Memory.Variables.AddVariable(name, 0F);
			}
		}
		#endregion

		#region IPropertable Implements

		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public override List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
			if (mTag == null) mTag = new object[] { 0, "00000" };
			var propList = new List<PropertyView>();

			propList.Add(CreateImageSourceView(langMap));
			propList.Add(
				new PropertyView(
					langMap["PropAdptSeq"],
					AccessLevel.None,
					langMap["TipSeq"],
					"AdeptSight Sequence",
					(mTag as object[])[0].ToString(),
					() => {
						List<int> seq;
						object[] tag = mTag as object[];
						if (ValueEditor(langMap, tag[0].ToString(), 1, 0, 32767, out seq)) {
							tag[0] = seq[0];
							this.IsModified = true;
						}
						return tag[0].ToString();
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropCali"],
					AccessLevel.None,
					langMap["TipCali"],
					"Calibration",
					mCali,
					() => {
						var caliNames = mTool.AceServer.Root
										.FilterType(typeof(IAdeptSightCameraCalibration), true)
										.Select(obj => obj.FullPath)
										.ToList();
						caliNames.Insert(0, "None");
						string cali = string.Empty;
						string curCali = caliNames.Contains(mCali) ? mCali : string.Empty;
						if (CtInput.ComboBoxList(out cali, langMap["CaliTit"], langMap["CaliMsg"], caliNames, curCali) == Stat.SUCCESS) {
							mCali = "None".Equals(cali) ? string.Empty : cali;
							this.IsModified = true;
						}
						return mCali;
					},
					() => (int)(mTag as object[])[0] > 0
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPreLightEnb"],
					AccessLevel.None,
					langMap["TipPreLightEnb"],
					"Pre-Light Enabled",
					GetBoolString(langMap, mPreLightEnb),
					() => {
						bool preEnb;
						if (BoolEditor(langMap, mPreLightEnb, out preEnb)) {
							mPreLightEnb = preEnb;
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return GetBoolString(langMap, mPreLightEnb);
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPreLightVar"],
					AccessLevel.None,
					langMap["TipPreLightVar"],
					"Pre-Light Variable",
					mPreLightVar,
					() => {
						string preLight;
						if (CtInput.Text(out preLight, langMap["PreLightTit"], langMap["PreLightMsg"], mPreLightVar) == Stat.SUCCESS) {
							if (!string.IsNullOrEmpty(preLight)) {
								if (!CheckRealVp(preLight)) {
									CtMsgBox.Show(langMap["NoVpVarTit"], langMap["NoVpVarMsg"], MsgBoxBtn.OK, MsgBoxStyle.Error);
									MsgBoxBtn btn = CtMsgBox.Show(langMap["NoVpVarTit"], langMap["NoVpVarCrt"], MsgBoxBtn.YesNo, MsgBoxStyle.Warning);
									if (btn == MsgBoxBtn.Yes) {
										CreateRealVp(preLight);
										mPreLightVar = preLight;
									}
								} else mPreLightVar = preLight;
							} else mPreLightVar = string.Empty;
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mPreLightVar;
					},
					() => mPreLightEnb
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropSaveOkVar"],
					AccessLevel.None,
					langMap["TipSaveOkVar"],
					"Save OK Image",
					mSaveOkVar,
					() => {
						string saveOK = string.Empty;
						if (CtInput.Text(out saveOK, langMap["SaveOkTit"], langMap["SaveOkMsg"], mSaveOkVar) == Stat.SUCCESS) {
							if (!string.IsNullOrEmpty(saveOK)) {
								if (!CheckRealVp(saveOK)) {
									CtMsgBox.Show(langMap["NoVpVarTit"], langMap["NoVpVarMsg"], MsgBoxBtn.OK, MsgBoxStyle.Error);
									MsgBoxBtn btn = CtMsgBox.Show(langMap["NoVpVarTit"], langMap["NoVpVarCrt"], MsgBoxBtn.YesNo, MsgBoxStyle.Warning);
									if (btn == MsgBoxBtn.Yes) {
										CreateRealVp(saveOK);
										mSaveOkVar = saveOK;
									}
								} else mSaveOkVar = saveOK;
							} else mSaveOkVar = string.Empty;
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mSaveOkVar;
					}
				)
			);

			propList.Add(CreateShowResultView(langMap));

			propList.Add(
				new PropertyView(
					langMap["PropPartNo"],
					AccessLevel.None,
					langMap["TipPartNo"],
					"Component Code / Part Number",
					(mTag as object[])[1].ToString(),
					() => {
						string partNo = string.Empty;
						object[] tag = mTag as object[];
						if (CtInput.Text(out partNo, langMap["PartNo"], langMap["PartNoEnt"], tag[1].ToString()) == Stat.SUCCESS) {
							tag[1] = partNo;
							this.IsModified = true;
						}
						return tag[1].ToString();
					}
				)
			);

			propList.Add(CreateCommentView(langMap));

			return propList;
		}
		#endregion

		#region IResultable Implements
		/// <summary>取得適用於顯示結果的 <see cref="DataTable.TableName"/></summary>
		public string ResultTableName { get { return "CustomVisionTool"; } }

		/// <summary>建立當前影像工具的執行結果清單。如欲隱藏特定欄位，請透過 <see cref="DataGridView.Columns"/> 進行操作</summary>
		/// <returns>對應的 執行結果清單</returns>
		public DataTable CreateDataTable() {
			DataTable dt = new DataTable(ResultTableName);
			DataColumn[] columns = GetResultColumnNames().Select(kvp => new DataColumn(kvp.Key) { Caption = kvp.Value }).ToArray();
			dt.Columns.AddRange(columns);

			ICSharpCustomTool tool = mTool as ICSharpCustomTool;
			if (tool.Results.Length > 0) {
				int idx = 1;
				List<VisionTransformResult> results = tool.Results.ToList();
				results.ForEach(
					result => {
						dt.Rows.Add(
							new string[] {
								(idx++).ToString(),
								result.Transform.X.ToString("F3"),
								result.Transform.Y.ToString("F3"),
								result.Transform.Degrees.ToString("F3"),
								result.Transform.Name ?? string.Empty
							}
						);
					}
				);
			}

			return dt;
		}

		/// <summary>取得結果清單的欄位名稱(<see cref="DataColumn.ColumnName"/>)與標題(<see cref="DataColumn.Caption"/>)對應表</summary>
		/// <returns>清單欄位與標題對應表</returns>
		public Dictionary<string, string> GetResultColumnNames() {
			return new Dictionary<string, string> {
				{ "inst" , "Instance" },
				{ "posX" , "Position X" },
				{ "posY" , "Position Y" },
				{ "angle" , "Angle" },
				{ "name" , "Name" }
			};
		}

		/// <summary>建立結果清單的預設欄位名稱與預設啟用狀態</summary>
		/// <returns>預設的欄位與致能</returns>
		public ResultableTable GetDefaultResultColumns() {
			List<ResultableColumn> columns = new List<ResultableColumn> {
				new ResultableColumn("inst", "Instance", true),
				new ResultableColumn("posX", "Position X", true),
				new ResultableColumn("posY", "Position Y", true),
				new ResultableColumn("angle", "Angle", true),
				new ResultableColumn("name", "Name", true)
			};
			return new ResultableTable(ResultTableName, columns);
		}
		#endregion

		#region IXmlSavable Implements
		/// <summary>產生物件的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		public override XmlElmt CreateXmlData(string nodeName) {
			XmlElmt xmlData = GenerateXmlData(nodeName);
			xmlData.Add(
				new XmlElmt(
					"PreLight",
					new XmlElmt("Enabled", mPreLightEnb.ToString()),
					new XmlElmt("VpGlobVar", mPreLightVar)
				),
				new XmlElmt("SaveOkImg", mSaveOkVar ?? string.Empty),
				new XmlElmt("Calibration", mCali ?? string.Empty)
			);
			return xmlData;
		}
		#endregion
	}

    ///<summary>子CVT 工具包</summary>
    public class SubCustomVisionToolPack : VisionToolPackBase,IResultable {
        
        #region Constructors
        /// <summary>建立影像工具包於指定的 Ace 資料夾，附帶建立 <see cref="TreeNode"/></summary>
        /// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
        /// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
        /// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
        public SubCustomVisionToolPack(
            List<IVisionToolPack> toolColl,
            IAceObjectCollection toolFold,
            TreeNode mainNode
        ) : base(toolColl, VisionToolType.SubCVT, toolFold, mainNode) {

        }

        /// <summary>從現有的影像工具進行複製</summary>
        /// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
        /// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
        /// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
        /// <param name="copyPack">欲複製的來源</param>
        public SubCustomVisionToolPack(
            List<IVisionToolPack> toolColl,
            IVisionToolPack copyPack,
            IAceObjectCollection toolFold,
            TreeNode mainNode
        ) : base(toolColl, copyPack, toolFold, mainNode) {

        }

        /// <summary>建構單純的影像工具包，不含 <seealso cref="TreeNode"/> 等父節點</summary>
        /// <param name="source">指定的影像來源</param>
        /// <param name="fold">欲存放此工具的 Ace 資料夾</param>
        public SubCustomVisionToolPack(
            IVisionImageSource source,
            IAceObjectCollection fold
        ) : base(VisionToolType.CustomVisionTool, source, fold) {

        }

        /// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
        /// <param name="langMap">多國語系之對應清單</param>
        /// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
        /// <param name="aceSrv">已連線的 ACE Server 端物件</param>
        /// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
        /// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
        public SubCustomVisionToolPack(
            Dictionary<string, string> langMap,
            XmlElmt xmlData,
            IAceServer aceSrv
        ) : base(xmlData, aceSrv, langMap) {            
        }

        
        /// <summary>建立 <see cref="ILocatorModel"/> 之工具包，請勿加入 IVisionToolPack 集合物件，避免偵測時失誤</summary>
        /// <param name="source">指定的影像來源</param>
        /// <param name="toolFold">指定存放的 Ace 資料夾</param>
        /// <param name="mainNode">存放含有 <see cref="ILocatorTool"/> 的 <seealso cref="IVisionToolPack"/> 之 <seealso cref="TreeNode"/></param>
        public SubCustomVisionToolPack(
            IVisionImageSource source,
            IAceObjectCollection toolFold,
            TreeNode mainNode
        ) : base() {

            /* 直接指定建立 ILocatorModel */
            mToolType = GetToolType(VisionToolType.LocatorModel);
            mFold = toolFold;
            mTool = mFold.AddObjectOfType(mToolType, GetToolString(VisionToolType.LocatorModel)) as IVisionTool;

            IVisionTool tool = mTool as IVisionTool;
            if (tool != null) tool.ImageSource = source;
            else if (mToolType == typeof(IImageProcessingTool)) (mTool as IImageProcessingTool).ImageSource = source;

            /* Model 不須變數名稱 */
            mVarName = string.Empty;

            /* 建立 TreeNode */
            mNode = mainNode.Nodes.Add(GetToolString(VisionToolType.LocatorModel));
            mNode.Tag = this;
            mNodeLv = mNode.Level;

            /* 確認是否父節點有影像工具包 */
            if (mainNode.Tag != null) {
                IVisionToolPack pack = mainNode.Tag as IVisionToolPack;
                if (pack != null) mInputLinkTool = pack.Tool as IVisionTool;
            }

            /* 設定 ShowGraphics */
            InitialTool();

        }

        #endregion

        #region IPropertable Implement

        /// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
        /// <param name="langMap">各國語系之對應清單</param>
        /// <returns>對應的屬性檢視</returns>
        public override List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
            return new List<PropertyView>();
        }

        #endregion IPropertable Implement

        #region IXmlSavable Implement 

        /// <summary>產生物件的 XML 相關資料描述</summary>
        /// <param name="nodeName">此物件之 XML 節點名稱</param>
        /// <returns>XML 節點</returns>
        public override XmlElmt CreateXmlData(string nodeName) {
            return new XmlElmt("SubCVT");
        }

        #endregion IXmlSavable Implement

        #region IResultable Implement

        /// <summary>建立當前影像工具的執行結果清單。如欲隱藏特定欄位，請透過 <see cref="DataGridView.Columns"/> 進行操作</summary>
        /// <returns>對應的 執行結果清單</returns>
        public DataTable CreateDataTable() {
            return new DataTable();
        }

        /// <summary>取得結果清單的欄位名稱(<see cref="DataColumn.ColumnName"/>)與標題(<see cref="DataColumn.Caption"/>)對應表</summary>
        /// <returns>清單欄位與標題對應表</returns>
        public Dictionary<string, string> GetResultColumnNames() {
            return new Dictionary<string, string>();
        }


        /// <summary>建立結果清單的預設欄位名稱與預設啟用狀態</summary>
        /// <returns>預設的欄位與致能</returns>
        public ResultableTable GetDefaultResultColumns() {
            return new ResultableTable(ResultTableName,new List<ResultableColumn>());
        }

        /// <summary>取得適用於顯示結果的 <see cref="DataTable.TableName"/></summary>
        public string ResultTableName {
            get {
                return "SubCVT Result";
            }
        }

        #endregion IResultable Implement
    }

    /// <summary>Locator 工具包</summary>
    public class LocatorToolPack : VisionToolPackBase, IResultOfTransform {

		#region Fields
		private bool mJustCreated = false;
		#endregion

		#region IResultOfAngle Implements
		/// <summary>取得或設定此影像工具是否作為最後回傳的角度定義</summary>
		public bool IsAngleReturner { get; set; }

        ///<summary>產生角度CVT語法</summary>
        ///<param name="code">程式碼主體</param>
        public void GenerateAngleCVT(List<string> code) {
            base.generateAngleCVT(code);
        }

        #endregion

        #region Constructors
        /// <summary>建立影像工具包於指定的 Ace 資料夾，附帶建立 <see cref="TreeNode"/></summary>
        /// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
        /// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
        /// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
        public LocatorToolPack(
			List<IVisionToolPack> toolColl,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base(toolColl, VisionToolType.Locator, toolFold, mainNode) {

			mJustCreated = true;
			(mTool as ILocatorTool).ResultsDisplay = LocatorResultMode.MarkerAndModel;

		}

		/// <summary>從現有的影像工具進行複製</summary>
		/// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
		/// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
		/// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
		/// <param name="copyPack">欲複製的來源</param>
		public LocatorToolPack(
			List<IVisionToolPack> toolColl,
			IVisionToolPack copyPack,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base(toolColl, copyPack, toolFold, mainNode) {

			mJustCreated = true;
			(mTool as ILocatorTool).ResultsDisplay = LocatorResultMode.MarkerAndModel;

		}

		/// <summary>建構單純的影像工具包，不含 <seealso cref="TreeNode"/> 等父節點</summary>
		/// <param name="source">指定的影像來源</param>
		/// <param name="fold">欲存放此工具的 Ace 資料夾</param>
		public LocatorToolPack(
			IVisionImageSource source,
			IAceObjectCollection fold
		) : base(VisionToolType.Locator, source, fold) {

			mJustCreated = true;
			(mTool as ILocatorTool).ResultsDisplay = LocatorResultMode.MarkerAndModel;

		}

		/// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
		/// <param name="aceSrv">已連線的 ACE Server 端物件</param>
		/// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
		/// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
		public LocatorToolPack(
			Dictionary<string, string> langMap,
			XmlElmt xmlData,
			IAceServer aceSrv
		) : base(xmlData, aceSrv, langMap) {

			mJustCreated = false;
			(mTool as ILocatorTool).ResultsDisplay = LocatorResultMode.MarkerAndModel;

		}

		#endregion

		#region IPropertable Implements		

		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public override List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
			var propList = new List<PropertyView>();

			var locator = mTool as ILocatorTool;
			var tol = locator.ConformityTolerance;
			if (mJustCreated) {
				mJustCreated = false;
				tol.UseDefault = false;
				tol.Tolerance = tol.DefaultTolerance;
			}

			var confTol = string.Format(langMap["TipConfTol"], tol.DefaultTolerance.ToString());

			NominalParameterConfiguration degConfig = locator.NominalRotation;
			var nomRot = string.Format("{0:F3}, {1:F3}", degConfig.Minimum, degConfig.Maximum);

			NominalParameterConfiguration sfConfig = locator.NominalScaleFactor;
			var nomSF = string.Format("{0:F3}, {1:F3}", sfConfig.Minimum, sfConfig.Maximum);

			mOfs = new VisionTransform(locator.Offset);
			if (mOfsShd != mOfs) {
				this.IsCompiled = false;
				mOfsShd = new VisionTransform(mOfs);
			}

			propList.Add(
				new PropertyView(
					langMap["PropConfTol"],
					AccessLevel.None,
					confTol,
					"Conformity Tolerance",
					locator.ConformityTolerance.Tolerance.ToString(),
					() => {
						List<float> tolVal;
						LocatorConformityToleranceConfiguration tolConf = locator.ConformityTolerance;
						string tolStr = tolConf.Tolerance.ToString();
						if (ValueEditor(langMap, tolStr, 1, tolConf.MinimumTolerance, tolConf.MaximumTolerance, out tolVal)) {
							tolConf.UseDefault = false;
							tolConf.Tolerance = tolVal[0];
							tolStr = tolVal[0].ToString();
							this.IsModified = true;
						}
						return tolStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropInsCnt"],
					AccessLevel.None,
					langMap["TipInsCnt"],
					"Maximum Instance Count",
					locator.MaximumInstanceCount.Count.ToString(),
					() => {
						List<int> instCount;
						string countStr = locator.MaximumInstanceCount.Count.ToString();
						if (ValueEditor(langMap, countStr, 1, 1, 2000, out instCount)) {
							locator.MaximumInstanceCount.Count = instCount[0];
							countStr = instCount[0].ToString();
							this.IsModified = true;
						}
						return countStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropNomRot"],
					AccessLevel.None,
					langMap["TipNomRot"],
					"Nominal Rotaion",
					nomRot,
					() => {
						List<double> deg;
						var nomRotStr = string.Format("{0:F3}, {1:F3}", degConfig.Minimum, degConfig.Maximum);
						if (ValueEditor(langMap, nomRotStr, 2, -180, 180, out deg)) {
							degConfig.Minimum = (float)deg[0];
							degConfig.Maximum = (float)deg[1];
							nomRotStr = string.Format("{0:F3}, {1:F3}", degConfig.Minimum, degConfig.Maximum);
							this.IsModified = true;
						}
						return nomRotStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropNomSF"],
					AccessLevel.None,
					langMap["TipNomSF"],
					"Nominal Scale Factor",
					nomSF,
					() => {
						List<double> deg;
						var nomSfStr = string.Format("{0:F3}, {1:F3}", sfConfig.Minimum, sfConfig.Maximum);
						if (ValueEditor(langMap, nomSfStr, 2, -180, 180, out deg)) {
							sfConfig.Minimum = (float)deg[0];
							sfConfig.Maximum = (float)deg[1];
							nomSfStr = string.Format("{0:F3}, {1:F3}", sfConfig.Minimum, sfConfig.Maximum);
							this.IsModified = true;
						}
						return nomSfStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropModPa"],
					AccessLevel.None,
					langMap["TipModPa"],
					"Minimum Model Percentage",
					locator.MinimumModelPercentage.ToString(),
					() => {
						List<float> modPerc;
						var percStr = locator.MinimumModelPercentage.ToString();
						if (ValueEditor(langMap, percStr, 1, 0, 100, out modPerc)) {
							locator.MinimumModelPercentage = modPerc[0];
							percStr = modPerc[0].ToString();
							this.IsModified = true;
						}
						return percStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropOfs"],
					AccessLevel.None,
					langMap["TipOfs"],
					"Offset",
					locator.Offset.ToString(),
					() => {
						List<double> ofsVal;
						var ofsStr = locator.Offset.ToString();
						if (ValueEditor(langMap, ofsStr, 3, double.MinValue, double.MaxValue, out ofsVal)) {
							VisionTransform ofsVT = new VisionTransform(ofsVal[0], ofsVal[1], ofsVal[2]);
							locator.Offset = ofsVT;
							ofsStr = locator.Offset.ToString();
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return ofsStr;
					}
				)
			);
			propList.Add(
				new PropertyView(
					langMap["PropRscReg"],
					AccessLevel.None,
					langMap["TipSchReg"],
					"Search Region / Region of Interest",
					locator.SearchRegion.ToString(),
					() => {
						List<double> srchRec;
						string roiStr = locator.SearchRegion.ToString();
						if (ValueEditor(langMap, roiStr, 2, double.MinValue, double.MaxValue, out srchRec)) {
							VisionRectangularSearchRegion roiSR = new VisionRectangularSearchRegion(srchRec[0], srchRec[1]);
							locator.SearchRegion = roiSR;
							roiStr = locator.SearchRegion.ToString();
							this.IsModified = true;
						}
						return roiStr;
					}
				)
			);

			propList.Add(CreateImageSourceView(langMap));
			propList.Add(CreateResultAvailableView(langMap));
			propList.Add(CreateShowResultView(langMap));
			propList.Add(CreateRoiView(langMap));
			propList.Add(CreateCommentView(langMap));

			return propList;
		}
		#endregion

		#region IResultable Implements
		/// <summary>取得適用於顯示結果的 <see cref="DataTable.TableName"/></summary>
		public string ResultTableName { get { return "Locator"; } }

		/// <summary>建立當前影像工具的執行結果清單。如欲隱藏特定欄位，請透過 <see cref="DataGridView.Columns"/> 進行操作</summary>
		/// <returns>對應的 執行結果清單</returns>
		public DataTable CreateDataTable() {
			DataTable dt = new DataTable(ResultTableName);
			DataColumn[] columns = GetResultColumnNames().Select(kvp => new DataColumn(kvp.Key) { Caption = kvp.Value }).ToArray();
			dt.Columns.AddRange(columns);

			ILocatorTool tool = mTool as ILocatorTool;
			if (tool.ResultsAvailable) {
				int idx = 1;
				List<LocatorInstance> results = tool.Results.ToList();
				results.ForEach(
					result => {
						dt.Rows.Add(
							new string[] {
								(idx++).ToString(),
								result.ModelName,
								(result.MatchQuality * 100).ToString("F2"),
								result.Position.X.ToString("F3"),
								result.Position.Y.ToString("F3"),
								result.Position.Degrees.ToString("F3"),
								result.ScaleFactor.ToString("F3"),
								(result.ClearQuality * 100).ToString("F2"),
								(result.FitQuality * 100).ToString("F2"),
								result.GroupIndex.ToString(),
								result.Model.ToString(),
								result.Symmetry.ToString(),
								result.Time.ToString("F2")
							}
						);
					}
				);
			}

			return dt;
		}

		/// <summary>取得結果清單的欄位名稱(<see cref="DataColumn.ColumnName"/>)與標題(<see cref="DataColumn.Caption"/>)對應表</summary>
		/// <returns>清單欄位與標題對應表</returns>
		public Dictionary<string, string> GetResultColumnNames() {
			return new Dictionary<string, string> {
				{ "inst" , "Instance" },
				{ "model" , "Model Name" },
				{ "machQty" , "Match Quality" },
				{ "posX" , "Position X" },
				{ "posY" , "Position Y" },
				{ "angle" , "Angle" },
				{ "sf" , "Scale Factor" },
				{ "clrQty" , "Clear Quality" },
				{ "fitQty" , "Fit Quality" },
				{ "fpg" , "Frame/Group" },
				{ "modelIdx" , "Model Index" },
				{ "symt" , "Symmetry" },
				{ "time" , "Time" }
			};
		}

		/// <summary>建立結果清單的預設欄位名稱與預設啟用狀態</summary>
		/// <returns>預設的欄位與致能</returns>
		public ResultableTable GetDefaultResultColumns() {
			List<ResultableColumn> columns = new List<ResultableColumn> {
				new ResultableColumn("inst", "Instance", true),
				new ResultableColumn("model", "Model Name", true),
				new ResultableColumn("machQty", "Match Quality", true),
				new ResultableColumn("posX", "Position X", true),
				new ResultableColumn("posY", "Position Y", true),
				new ResultableColumn("angle", "Angle", true),
				new ResultableColumn("sf", "Scale Factor", true),
				new ResultableColumn("clrQty", "Clear Quality", false),
				new ResultableColumn("fitQty", "Fit Quality", false),
				new ResultableColumn("fpg", "Frame/Group", false),
				new ResultableColumn("modelIdx", "Model Index", false),
				new ResultableColumn("symt", "Symmetry", false),
				new ResultableColumn("time", "Time", false)
			};

			return new ResultableTable(ResultTableName, columns);
		}
		#endregion

		#region IXmlSavable Implements
		/// <summary>產生物件的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		public override XmlElmt CreateXmlData(string nodeName) {
			return GenerateXmlData(nodeName);
		}
		#endregion
	}

	/// <summary>Locator Model 工具包</summary>
	public class LocatorModelPack : VisionToolPackBase {

		#region Constructors
		/// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
		/// <param name="aceSrv">已連線的 ACE Server 端物件</param>
		/// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
		/// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
		public LocatorModelPack(
			Dictionary<string, string> langMap,
			XmlElmt xmlData,
			IAceServer aceSrv
		) : base(xmlData, aceSrv, langMap) {

		}

		/// <summary>建立 <see cref="ILocatorModel"/> 之工具包，請勿加入 IVisionToolPack 集合物件，避免偵測時失誤</summary>
		/// <param name="source">指定的影像來源</param>
		/// <param name="toolFold">指定存放的 Ace 資料夾</param>
		/// <param name="mainNode">存放含有 <see cref="ILocatorTool"/> 的 <seealso cref="IVisionToolPack"/> 之 <seealso cref="TreeNode"/></param>
		public LocatorModelPack(
			IVisionImageSource source,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base() {

			/* 直接指定建立 ILocatorModel */
			mToolType = GetToolType(VisionToolType.LocatorModel);
			mFold = toolFold;
			int idx = mFold.FilterType(typeof(ILocatorModel)).Count;
			mTool = mFold.AddObjectOfType(mToolType, string.Format("{0} {1}", GetToolString(VisionToolType.LocatorModel), idx)) as IVisionToolBase;
			(mTool as ILocatorModel).ImageSource = source;

			/* Model 不須變數名稱 */
			mVarName = string.Empty;

			/* 建立 TreeNode */
			mNode = mainNode.Nodes.Add(GetToolString(VisionToolType.LocatorModel));
			mNode.Tag = this;
			mNodeLv = mNode.Level;

			/* 確認是否父節點有影像工具包 */
			if (mainNode.Tag != null) {
				IVisionToolPack pack = mainNode.Tag as IVisionToolPack;
				if (pack != null) mInputLinkTool = pack.Tool as IVisionTool;
			}

			/* 設定 ShowGraphics */
			InitialTool();

			/* 剛剛建構，故有修改 */
			IsModified = true;
		}

		/// <summary>複製 <see cref="ILocatorModel"/> 之工具包</summary>
		/// <param name="toolFold">指定存放的 Ace 資料夾</param>
		/// <param name="mainNode">存放含有 <see cref="ILocatorTool"/> 的 <seealso cref="IVisionToolPack"/> 之 <seealso cref="TreeNode"/></param>
		/// <param name="copyPack">欲複製的來源</param>
		public LocatorModelPack(
			IVisionToolPack copyPack,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base() {

			/* 直接指定建立 ILocatorModel */
			mToolType = GetToolType(VisionToolType.LocatorModel);
			mFold = toolFold;
			int idx = mFold.FilterType(typeof(ILocatorModel)).Count;
			mTool = copyPack.Tool.Clone() as IVisionToolBase;
			mTool.Name = string.Format("{0} {1}", GetToolString(VisionToolType.LocatorModel), idx);
			mFold.Add(mTool);

			/* Model 不須變數名稱 */
			mVarName = string.Empty;

			/* 建立 TreeNode */
			mNode = mainNode.Nodes.Add(GetToolString(VisionToolType.LocatorModel));
			mNode.Tag = this;
			mNodeLv = mNode.Level;

			/* 確認是否父節點有影像工具包 */
			if (mainNode.Tag != null) {
				IVisionToolPack pack = mainNode.Tag as IVisionToolPack;
				if (pack != null) mInputLinkTool = pack.Tool as IVisionTool;
			}

			/* 設定 ShowGraphics */
			InitialTool();

			/* 剛剛建構，故有修改 */
			IsModified = true;
		}
		#endregion

		#region IPropertable Implements

		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public override List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
			var propList = new List<PropertyView> {
				CreateImageSourceView(langMap),
				CreateCommentView(langMap)
			};

			return propList;
		}
		#endregion

		#region IXmlSavable Implements
		/// <summary>產生物件的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		public override XmlElmt CreateXmlData(string nodeName) {
			return GenerateXmlData(nodeName);
		}
		#endregion
	}

	/// <summary>Blob Analyzer 工具包</summary>
	public class BlobAnalyzerPack : VisionToolPackBase, IResultOfTransform {

		#region IResultOfAngle Implements
		
        /// <summary>取得或設定此影像工具是否作為最後回傳的角度定義</summary>
		public bool IsAngleReturner { get; set; }

        ///<summary>產生角度CVT語法</summary>
        ///<param name="code">程式碼主體</param>
        public void GenerateAngleCVT(List<string> code) {
            base.generateAngleCVT(code);
        }

        #endregion

        #region Constructors
        /// <summary>建立影像工具包於指定的 Ace 資料夾，附帶建立 <see cref="TreeNode"/></summary>
        /// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
        /// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
        /// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
        public BlobAnalyzerPack(
			List<IVisionToolPack> toolColl,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base(toolColl, VisionToolType.BlobAnalyzer, toolFold, mainNode) {

			(mTool as IBlobAnalyzerTool).ResultsDisplay = BlobAnalyzerResultMode.MarkerAndBlobImage;

		}

		/// <summary>從現有的影像工具進行複製</summary>
		/// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
		/// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
		/// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
		/// <param name="copyPack">欲複製的來源</param>
		public BlobAnalyzerPack(
			List<IVisionToolPack> toolColl,
			IVisionToolPack copyPack,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base(toolColl, copyPack, toolFold, mainNode) {

			(mTool as IBlobAnalyzerTool).ResultsDisplay = BlobAnalyzerResultMode.MarkerAndBlobImage;

		}

		/// <summary>建構單純的影像工具包，不含 <seealso cref="TreeNode"/> 等父節點</summary>
		/// <param name="source">指定的影像來源</param>
		/// <param name="fold">欲存放此工具的 Ace 資料夾</param>
		public BlobAnalyzerPack(
			IVisionImageSource source,
			IAceObjectCollection fold
		) : base(VisionToolType.BlobAnalyzer, source, fold) {

			(mTool as IBlobAnalyzerTool).ResultsDisplay = BlobAnalyzerResultMode.MarkerAndBlobImage;

		}

		/// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
		/// <param name="aceSrv">已連線的 ACE Server 端物件</param>
		/// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
		/// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
		public BlobAnalyzerPack(
			Dictionary<string, string> langMap,
			XmlElmt xmlData,
			IAceServer aceSrv
		) : base(xmlData, aceSrv, langMap) {

			(mTool as IBlobAnalyzerTool).ResultsDisplay = BlobAnalyzerResultMode.MarkerAndBlobImage;

		}

		#endregion

		#region IPropertable Implements

		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public override List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
			var propList = new List<PropertyView>();

			var blob = mTool as IBlobAnalyzerTool;

			var segParm = blob.SegmentationParameters;
			var thrStr = $"{segParm.Threshold1}, {segParm.Threshold2}";
			if (segParm.SegmentationMode > BlobSegmentationMode.Dark)
				thrStr += $", {segParm.Threshold3}, {segParm.Threshold4}";

			var ordStr = blob.Sorting.Ascending ? "Ascending" : "Descending";

			mOfs = new VisionTransform(blob.Offset);
			if (mOfsShd != mOfs) {
				this.IsCompiled = false;
				mOfsShd = new VisionTransform(mOfs);
			}

			propList.Add(
				new PropertyView(
					langMap["PropBlobAngl"],
					AccessLevel.None,
					langMap["TipBlobAngl"],
					"Calculate Blob Angle",
					GetBoolString(langMap, blob.CalculateBlobAngle),
					() => {
						bool blobAngle = blob.CalculateBlobAngle;
						if (BoolEditor(langMap, blobAngle, out blobAngle)) {
							blob.CalculateBlobAngle = blobAngle;
							this.IsModified = true;
						}
						return GetBoolString(langMap, blob.CalculateBlobAngle);
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropHolFil"],
					AccessLevel.None,
					langMap["TipHolFil"],
					"Hole Filling Enabled",
					GetBoolString(langMap, blob.HoleFillingEnabled),
					() => {
						bool blobHole = blob.HoleFillingEnabled;
						if (BoolEditor(langMap, blobHole, out blobHole)) {
							blob.HoleFillingEnabled = blobHole;
							this.IsModified = true;
						}
						return GetBoolString(langMap, blob.HoleFillingEnabled);
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropBlobOptMod"],
					AccessLevel.None,
					langMap["TipBlobOptMod"],
					"Optimization Mode",
					langMap[blob.OptimizationMode.GetFullName()],
					() => {
						BlobAnalyzerOptimizationMode blobMode = BlobAnalyzerOptimizationMode.Speed;
						string optStr = blob.OptimizationMode.ToString();
						if (EnumEditor(langMap, optStr, out blobMode)) {
							blob.OptimizationMode = blobMode;
							optStr = blob.OptimizationMode.ToString();
							this.IsModified = true;
						}
						return optStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropClipBlob"],
					AccessLevel.None,
					langMap["TipClipBlob"],
					"Allow Clipped Blobs",
					GetBoolString(langMap, blob.AllowClippedBlobs),
					() => {
						bool blobClip = blob.AllowClippedBlobs;
						if (BoolEditor(langMap, blobClip, out blobClip)) {
							blob.AllowClippedBlobs = blobClip;
							this.IsModified = true;
						}
						return GetBoolString(langMap, blob.AllowClippedBlobs);
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropBlobCnt"],
					AccessLevel.None,
					langMap["TipBlobCnt"],
					"Maximum Blob Count",
					blob.MaximumBlobCount.ToString(),
					() => {
						List<int> blobCount;
						string countStr = blob.MaximumBlobCount.ToString();
						if (ValueEditor(langMap, countStr, 1, 1, int.MaxValue, out blobCount)) {
							blob.MaximumBlobCount = blobCount[0];
							countStr = blob.MaximumBlobCount.ToString();
							this.IsModified = true;
						}
						return countStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropBlobSeg"],
					AccessLevel.None,
					langMap["TipBlobSeg"],
					"Segmentation Mode",
					langMap[blob.SegmentationParameters.SegmentationMode.GetFullName()],
					() => {
						BlobSegmentationMode blobSeg = BlobSegmentationMode.Dark;
						string segStr = blob.SegmentationParameters.SegmentationMode.ToString();
						if (EnumEditor(langMap, segStr, out blobSeg)) {
							var segPara = blob.SegmentationParameters;
							segPara.SegmentationMode = blobSeg;
							if (blobSeg > BlobSegmentationMode.Outside) {
								segPara.Threshold1 = 0;
								segPara.Threshold2 = 0;
								segPara.Threshold3 = 0;
								segPara.Threshold4 = 0;
							}
							segStr = blob.SegmentationParameters.SegmentationMode.ToString();
							this.IsModified = true;
						}
						return segStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropMinArea"],
					AccessLevel.None,
					langMap["TipMinArea"],
					"Minimum Blob Area",
					blob.SegmentationParameters.MinimumBlobArea.ToString("F3"),
					() => {
						List<float> blobAreaMin;
						string minStr = blob.SegmentationParameters.MinimumBlobArea.ToString("F3");
						if (ValueEditor(langMap, minStr, 1, 0, 32767, out blobAreaMin)) {
							blob.SegmentationParameters.MinimumBlobArea = blobAreaMin[0];
							minStr = blob.SegmentationParameters.MinimumBlobArea.ToString("F3");
							this.IsModified = true;
						}
						return minStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropMaxArea"],
					AccessLevel.None,
					langMap["TipMaxArea"],
					"Maximum Blob Area",
					blob.SegmentationParameters.MaximumBlobArea.ToString("F3"),
					() => {
						List<float> blobAreaMax;
						string maxStr = blob.SegmentationParameters.MaximumBlobArea.ToString("F3");
						if (ValueEditor(langMap, maxStr, 1, 0, 32767, out blobAreaMax)) {
							blob.SegmentationParameters.MaximumBlobArea = blobAreaMax[0];
							maxStr = blob.SegmentationParameters.MaximumBlobArea.ToString("F3");
							this.IsModified = true;
						}
						return maxStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropBlobThr"],
					AccessLevel.None,
					langMap["TipBlobThr"],
					"Segmentation Thresholds",
					thrStr,
					() => {
						List<short> blobThr;
						var seg = blob.SegmentationParameters;
						var segThrCnt = (seg.SegmentationMode <= BlobSegmentationMode.Dark) ? 2 : 4;
						var segThrStr = $"{seg.Threshold1}, {seg.Threshold2}";
						if (segThrCnt > 2) segThrStr += $", {seg.Threshold3}, {seg.Threshold4}";
						if (seg.SegmentationMode > BlobSegmentationMode.Outside) {
							CtMsgBox.Show(langMap["InvSegTit"], langMap["InvSegMsg"], MsgBoxBtn.OK, MsgBoxStyle.Error);
						} else if (ValueEditor(langMap, segThrStr, segThrCnt, (short)0, (short)255, out blobThr)) {
							if (segThrCnt == 2) {
								seg.Threshold1 = blobThr[0];
								seg.Threshold2 = blobThr[1];
								seg.Threshold3 = 0;
								seg.Threshold4 = 0;
								segThrStr = $"{seg.Threshold1}, {seg.Threshold2}";
							} else {
								seg.Threshold1 = blobThr[0];
								seg.Threshold2 = blobThr[1];
								seg.Threshold3 = blobThr[2];
								seg.Threshold4 = blobThr[3];
								segThrStr = $"{seg.Threshold1}, {seg.Threshold2}, {seg.Threshold3}, {seg.Threshold4}";
							}
							this.IsModified = true;
						}
						return segThrStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropBlobSortEnb"],
					AccessLevel.None,
					langMap["TipBlobSortEnb"],
					"Sorting Enabled",
					GetBoolString(langMap, blob.Sorting.Enabled),
					() => {
						bool blobSort = blob.Sorting.Enabled;
						if (BoolEditor(langMap, blobSort, out blobSort)) {
							blob.Sorting.Enabled = blobSort;
							this.IsModified = true;
						}
						return GetBoolString(langMap, blob.Sorting.Enabled);
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropBlobSortOrd"],
					AccessLevel.None,
					langMap["TipBlobSortOrd"],
					"Sorting Ascending/Descending",
					ordStr,
					() => {
						BlobSortConfiguration ordConf = blob.Sorting;
						string sortOrdStr;
						string ordCurStr = ordConf.Ascending ? langMap["Blob.Ascending"] : langMap["Blob.Descending"];
						if (CtInput.ComboBoxList(out sortOrdStr, langMap["BlobOrdTit"], langMap["BlobOrdEnt"], new List<string> { langMap["Blob.Ascending"], langMap["Blob.Descending"] }, ordCurStr) == Stat.SUCCESS) {
							ordConf.Ascending = ordStr == langMap["Blob.Ascending"];
							ordCurStr = ordConf.Ascending ? langMap["Blob.Ascending"] : langMap["Blob.Descending"];
							this.IsModified = true;
						}
						return ordCurStr;
					},
					() => blob.Sorting.Enabled
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropBlobSortMode"],
					AccessLevel.None,
					langMap["TipBlobSortMode"],
					"Blob Sort Method",
					langMap[blob.Sorting.Sort.GetFullName()],
					() => {
						BlobSortConfiguration propConf = blob.Sorting;
						BlobSortMethod sorEnum;
						string propStr = propConf.Sort.ToString();
						if (EnumEditor(langMap, propStr, out sorEnum)) {
							propConf.Sort = sorEnum;
							propStr = propConf.Sort.ToString();
							this.IsModified = true;
						}
						return propStr;
					},
					() => blob.Sorting.Enabled
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropOfs"],
					AccessLevel.None,
					langMap["TipOfs"],
					"Offset",
					blob.Offset.ToString(),
					() => {
						List<double> ofs;
						string ofsStr = blob.Offset.ToString();
						if (ValueEditor(langMap, ofsStr, 3, double.MinValue, double.MaxValue, out ofs)) {
							VisionTransform ofsVT = new VisionTransform(ofs[0], ofs[1], ofs[2]);
							blob.Offset = ofsVT;
							ofsStr = blob.Offset.ToString();
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return ofsStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropRscReg"],
					AccessLevel.None,
					langMap["TipSchReg"],
					"Search Region / Region of Interest",
					blob.SearchRegion.ToString(),
					() => {
						List<double> srchRec;
						string recStr = blob.SearchRegion.ToString();
						if (ValueEditor(langMap, recStr, 2, double.MinValue, double.MaxValue, out srchRec)) {
							VisionRectangularSearchRegion roiSR = new VisionRectangularSearchRegion(srchRec[0], srchRec[1]);
							blob.SearchRegion = roiSR;
							recStr = blob.SearchRegion.ToString();
							this.IsModified = true;
						}
						return recStr;
					}
				)
			);

			propList.Add(CreateImageSourceView(langMap));
			propList.Add(CreateShowResultView(langMap));
			propList.Add(CreateResultAvailableView(langMap));
			propList.Add(CreateRoiView(langMap));
			propList.Add(CreateCommentView(langMap));

			return propList;
		}
		#endregion

		#region IResultable Implements
		/// <summary>取得適用於顯示結果的 <see cref="DataTable.TableName"/></summary>
		public string ResultTableName { get { return "BlobAnalyzer"; } }

		/// <summary>建立當前影像工具的執行結果清單。如欲隱藏特定欄位，請透過 <see cref="DataGridView.Columns"/> 進行操作</summary>
		/// <returns>對應的 執行結果清單</returns>
		public DataTable CreateDataTable() {
			DataTable dt = new DataTable(ResultTableName);
			DataColumn[] columns = GetResultColumnNames().Select(kvp => new DataColumn(kvp.Key) { Caption = kvp.Value }).ToArray();
			dt.Columns.AddRange(columns);

			IBlobAnalyzerTool tool = mTool as IBlobAnalyzerTool;
			if (tool.ResultsAvailable) {
				int idx = 1;
				List<BlobResult> results = tool.Results.ToList();
				results.ForEach(
					result => {
						dt.Rows.Add(
							new string[] {
								(idx++).ToString(),
								result.Position.X.ToString("F3"),
								result.Position.Y.ToString("F3"),
								result.Position.Degrees.ToString("F3"),
								result.Area.ToString("F3"),
								result.BoundingBoxTop.ToString("F2"),
								result.BoundingBoxBottom.ToString("F2"),
								result.BoundingBoxLeft.ToString("F2"),
								result.BoundingBoxRight.ToString("F2"),
								result.BoundingBoxHeight.ToString("F2"),
								result.BoundingBoxWidth.ToString("F2"),
								result.BoundingBoxCenterX.ToString("F2"),
								result.BoundingBoxCenterY.ToString("F2"),
								result.BoundingBoxRotation.ToString("F2"),
								result.ConvexPerimeter.ToString("F2"),
								result.Elongation.ToString("F2"),
								result.ExtentTop.ToString("F2"),
								result.ExtentBottom.ToString("F2"),
								result.ExtentLeft.ToString("F2"),
								result.ExtentRight.ToString("F2"),
								result.BlobGroup.ToString(),
								result.GreyLevelMaximum.ToString(),
								result.GreyLevelMean.ToString(),
								result.GreyLevelMinimum.ToString(),
								result.GreyLevelRange.ToString(),
								result.GreyLevelStdDev.ToString("F2"),
								result.HoleCount.ToString(),
								result.InertiaMaximum.ToString("F2"),
								result.InertiaMinimum.ToString("F2"),
								result.InertiaXAxis.ToString("F2"),
								result.InertiaYAxis.ToString("F2"),
								result.IntrinsicBoundingBoxTop.ToString("F2"),
								result.IntrinsicBoundingBoxBottom.ToString("F2"),
								result.IntrinsicBoundingBoxLeft.ToString("F2"),
								result.IntrinsicBoundingBoxRight.ToString("F2"),
								result.IntrinsicBoundingBoxHeight.ToString("F2"),
								result.IntrinsicBoundingBoxWidth.ToString("F2"),
								result.IntrinsicBoundingBoxCenterX.ToString("F2"),
								result.IntrinsicBoundingBoxCenterY.ToString("F2"),
								result.IntrinsicBoundingBoxRotation.ToString("F2"),
								result.IntrinsicExtentTop.ToString("F2"),
								result.IntrinsicExtentBottom.ToString("F2"),
								result.IntrinsicExtentLeft.ToString("F2"),
								result.IntrinsicExtentRight.ToString("F2"),
								result.RawPerimeter.ToString("F2"),
								result.Roundness.ToString("F2")
							}
						);
					}
				);
			}

			return dt;
		}

		/// <summary>取得結果清單的欄位名稱(<see cref="DataColumn.ColumnName"/>)與標題(<see cref="DataColumn.Caption"/>)對應表</summary>
		/// <returns>清單欄位與標題對應表</returns>
		public Dictionary<string, string> GetResultColumnNames() {
			return new Dictionary<string, string> {
				{ "inst" , "Instance" },
				{ "posX" , "Position X" },
				{ "posY" , "Position Y" },
				{ "angle" , "Angle" },
				{ "area", "Area" },
				{ "bondTop", "Bounding Box Top" },
				{ "bondBtm", "Bounding Box Bottom" },
				{ "bondLeft", "Bounding Box Left" },
				{ "bondRight", "Bounding Box Right" },
				{ "bondHeight", "Bounding Box Height" },
				{ "bondWidth", "Bounding Box Width" },
				{ "bondX", "Bounding Box Center X" },
				{ "bondY", "Bounding Box Center Y" },
				{ "bondRotate", "Bounding box Rotation" },
				{ "cnvx", "Convex Perimeter" },
				{ "elong", "Elongation" },
				{ "extTop", "Extent Top" },
				{ "extBtm", "Extent Bottom" },
				{ "extLeft", "Extent Left" },
				{ "extRight", "Extent Right" },
				{ "fpg", "Frame/Group" },
				{ "glvMax", "Grey Level Maximum" },
				{ "glvMean", "Grey Level Mean" },
				{ "glvMin", "Grey Level Minimum" },
				{ "glvRng", "Grey Level Range" },
				{ "glvStdDev", "Grey Level Std Dev" },
				{ "holeCount", "Hole Count" },
				{ "intMax", "Inertia Maximum" },
				{ "intMin", "Inertia Mimimum" },
				{ "intX", "Inertia X Axis" },
				{ "intY", "Inertia Y Axis" },
				{ "intBondTop", "Intrinsic Bounding Box Top" },
				{ "intBondBtm", "Intrinsic Bounding Box Bottom" },
				{ "intBondL", "Intrinsic Bounding Box Left" },
				{ "intBondR", "Intrinsic Bounding Box Right" },
				{ "intBondH", "Intrinsic Bounding Box Height" },
				{ "intBondW", "Intrinsic Bounding Box Width" },
				{ "intBondX", "Intrinsic Bounding Box Center X" },
				{ "intBondY", "Intrinsic Bounding Box Center Y" },
				{ "intBondRot", "Intrinsic Bounding Box Rotation" },
				{ "intExtTop", "Intrinsic Extent Top" },
				{ "intExtBtm", "Intrinsic Extent Bottom" },
				{ "intExtL", "Intrinsic Extent Left" },
				{ "intExtR", "Intrinsic Extent Right" },
				{ "raw", "Raw Perimeter" },
				{ "round", "Roundness" }
			};
		}

		/// <summary>建立結果清單的預設欄位名稱與預設啟用狀態</summary>
		/// <returns>預設的欄位與致能</returns>
		public ResultableTable GetDefaultResultColumns() {
			List<ResultableColumn> columns = new List<ResultableColumn> {
				new ResultableColumn("inst", "Instance", true),
				new ResultableColumn("posX", "Position X", true),
				new ResultableColumn("posY", "Position Y", true),
				new ResultableColumn("angle", "Angle", true),
				new ResultableColumn("area", "Area", true),
				new ResultableColumn("bondTop", "Bounding Box Top", false),
				new ResultableColumn("bondBtm", "Bounding Box Bottom", false),
				new ResultableColumn("bondLeft", "Bounding Box Left", false),
				new ResultableColumn("bondRight", "Bounding Box Right", false),
				new ResultableColumn("bondHeight", "Bounding Box Height", false),
				new ResultableColumn("bondWidth", "Bounding Box Width", false),
				new ResultableColumn("bondX", "Bounding Box Center X", false),
				new ResultableColumn("bondY", "Bounding Box Center Y", false),
				new ResultableColumn("bondRotate", "Bounding box Rotation", false),
				new ResultableColumn("cnvx", "Convex Perimeter", false),
				new ResultableColumn("elong", "Elongation", false),
				new ResultableColumn("extTop", "Extent Top", false),
				new ResultableColumn("extBtm", "Extent Bottom", false),
				new ResultableColumn("extLeft", "Extent Left", false),
				new ResultableColumn("extRight", "Extent Right", false),
				new ResultableColumn("fpg", "Frame/Group", false),
				new ResultableColumn("glvMax", "Grey Level Maximum", false),
				new ResultableColumn("glvMean", "Grey Level Mean", false),
				new ResultableColumn("glvMin", "Grey Level Minimum", false),
				new ResultableColumn("glvRng", "Grey Level Range", false),
				new ResultableColumn("glvStdDev", "Grey Level Std Dev", false),
				new ResultableColumn("holeCount", "Hole Count", false),
				new ResultableColumn("intMax", "Inertia Maximum", false),
				new ResultableColumn("intMin", "Inertia Mimimum", false),
				new ResultableColumn("intX", "Inertia X Axis", false),
				new ResultableColumn("intY", "Inertia Y Axis", false),
				new ResultableColumn("intBondTop", "Intrinsic Bounding Box Top", false),
				new ResultableColumn("intBondBtm", "Intrinsic Bounding Box Bottom", false),
				new ResultableColumn("intBondL", "Intrinsic Bounding Box Left", false),
				new ResultableColumn("intBondR", "Intrinsic Bounding Box Right", false),
				new ResultableColumn("intBondH", "Intrinsic Bounding Box Height", false),
				new ResultableColumn("intBondW", "Intrinsic Bounding Box Width", false),
				new ResultableColumn("intBondX", "Intrinsic Bounding Box Center X", false),
				new ResultableColumn("intBondY", "Intrinsic Bounding Box Center Y", false),
				new ResultableColumn("intBondRot", "Intrinsic Bounding Box Rotation", false),
				new ResultableColumn("intExtTop", "Intrinsic Extent Top", false),
				new ResultableColumn("intExtBtm", "Intrinsic Extent Bottom", false),
				new ResultableColumn("intExtL", "Intrinsic Extent Left", false),
				new ResultableColumn("intExtR", "Intrinsic Extent Right", false),
				new ResultableColumn("raw", "Raw Perimeter", false),
				new ResultableColumn("round", "Roundness", false)
			};

			return new ResultableTable(ResultTableName, columns);
		}
		#endregion

		#region IXmlSavable Implements
		/// <summary>產生物件的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		public override XmlElmt CreateXmlData(string nodeName) {
			return GenerateXmlData(nodeName);
		}
		#endregion
	}

	/// <summary>Edge Locator 工具包</summary>
	public class EdgeLocatorPack : VisionToolPackBase, IResultOfTransform {

		#region Constructors
		/// <summary>建立影像工具包於指定的 Ace 資料夾，附帶建立 <see cref="TreeNode"/></summary>
		/// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
		/// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
		/// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
		public EdgeLocatorPack(
			List<IVisionToolPack> toolColl,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base(toolColl, VisionToolType.EdgeLocator, toolFold, mainNode) {

		}

		/// <summary>從現有的影像工具進行複製</summary>
		/// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
		/// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
		/// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
		/// <param name="copyPack">欲複製的來源</param>
		public EdgeLocatorPack(
			List<IVisionToolPack> toolColl,
			IVisionToolPack copyPack,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base(toolColl, copyPack, toolFold, mainNode) {

		}

		/// <summary>建構單純的影像工具包，不含 <seealso cref="TreeNode"/> 等父節點</summary>
		/// <param name="source">指定的影像來源</param>
		/// <param name="fold">欲存放此工具的 Ace 資料夾</param>
		public EdgeLocatorPack(
			IVisionImageSource source,
			IAceObjectCollection fold
		) : base(VisionToolType.EdgeLocator, source, fold) {

		}

		/// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
		/// <param name="aceSrv">已連線的 ACE Server 端物件</param>
		/// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
		/// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
		public EdgeLocatorPack(
			Dictionary<string, string> langMap,
			XmlElmt xmlData,
			IAceServer aceSrv
		) : base(xmlData, aceSrv, langMap) {

		}

		#endregion

		#region IPropertable Implements

		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public override List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
			List<PropertyView> propList = new List<PropertyView>();

			var edge = mTool as IEdgeLocatorTool;

			mOfs = new VisionTransform(edge.Offset);
			if (mOfsShd != mOfs) {
				this.IsCompiled = false;
				mOfsShd = new VisionTransform(mOfs);
			}

			propList.Add(
				new PropertyView(
					langMap["PropEdgeHalf"],
					AccessLevel.None,
					langMap["TipEdgeHalf"],
					"Filter Half Width",
					edge.FilterHalfWidth.ToString(),
					() => {
						List<int> filtWidth;
						string fwStr = edge.FilterHalfWidth.ToString();
						if (ValueEditor(langMap, fwStr, 1, 0, 25, out filtWidth)) {
							edge.FilterHalfWidth = filtWidth[0];
							fwStr = edge.FilterHalfWidth.ToString();
							this.IsModified = true;
						}
						return fwStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropEdgeFilt"],
					AccessLevel.None,
					langMap["TipEdgeFilt"],
					"Filter Mode",
					langMap[edge.FilterMode.GetFullName()],
					() => {
						EdgeLocatorFilterMode filtMode = EdgeLocatorFilterMode.FirstEdge;
						string fmStr = edge.FilterMode.ToString();
						if (EnumEditor(langMap, fmStr, out filtMode)) {
							edge.FilterMode = filtMode;
							fmStr = edge.FilterMode.ToString();
							this.IsModified = true;
						}
						return fmStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropEdgeMag"],
					AccessLevel.None,
					langMap["TipEdgeMag"],
					"Magnitude Threshold",
					edge.MagnitudeThreshold.ToString(),
					() => {
						List<int> magThr;
						string magStr = edge.MagnitudeThreshold.ToString();
						if (ValueEditor(langMap, magStr, 1, 0, 255, out magThr)) {
							edge.MagnitudeThreshold = magThr[0];
							magStr = edge.MagnitudeThreshold.ToString();
							this.IsModified = true;
						}
						return magStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropEdgePola"],
					AccessLevel.None,
					langMap["TipEdgePola"],
					"Polarity Mode",
					langMap[edge.SearchParameters.PolarityMode.GetFullName()],
					() => {
						EdgeLocatorPolarityMode pol = EdgeLocatorPolarityMode.Either;
						string polStr = edge.SearchParameters.PolarityMode.ToString();
						if (EnumEditor(langMap, polStr, out pol)) {
							EdgeLocatorSearchParameters para = edge.SearchParameters;
							para.PolarityMode = pol;
							edge.SearchParameters = para;
							polStr = edge.SearchParameters.PolarityMode.ToString();
							this.IsModified = true;
						}
						return polStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropEdgeConst"],
					AccessLevel.None,
					langMap["TipEdgeConst"],
					"Constraints",
					langMap[edge.SearchParameters.Constraints.GetFullName()],
					() => {
						EdgeLocatorConstraint cons = EdgeLocatorConstraint.None;
						string conStr = edge.SearchParameters.Constraints.ToString();
						if (EnumEditor(langMap, conStr, out cons)) {
							EdgeLocatorSearchParameters para = edge.SearchParameters;
							para.Constraints = cons;
							edge.SearchParameters = para;
							conStr = edge.SearchParameters.Constraints.ToString();
							this.IsModified = true;
						}
						return conStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropEdgeScor"],
					AccessLevel.None,
					langMap["TipEdgeScor"],
					"Score Threshold",
					edge.SearchParameters.ScoreThreshold.ToString("F2"),
					() => {
						List<float> scThr;
						string stStr = edge.SearchParameters.ScoreThreshold.ToString("F2");
						if (ValueEditor(langMap, stStr, 1, 0F, 1F, out scThr)) {
							EdgeLocatorSearchParameters para = edge.SearchParameters;
							para.ScoreThreshold = scThr[0];
							edge.SearchParameters = para;
							stStr = edge.SearchParameters.ScoreThreshold.ToString("F2");
							this.IsModified = true;
						}
						return stStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropOfs"],
					AccessLevel.None,
					langMap["TipOfs"],
					"Offset",
					edge.Offset.ToString(),
					() => {
						List<double> ofs;
						string ofsStr = edge.Offset.ToString();
						if (ValueEditor(langMap, ofsStr, 3, double.MinValue, double.MaxValue, out ofs)) {
							VisionTransform ofsVT = new VisionTransform(ofs[0], ofs[1], ofs[2]);
							edge.Offset = ofsVT;
							ofsStr = edge.Offset.ToString();
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return ofsStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropRscReg"],
					AccessLevel.None,
					langMap["TipSchReg"],
					"Search Region / Region of Interest",
					edge.SearchRegion.ToString(),
					() => {
						List<double> srchRec;
						string recStr = edge.SearchRegion.ToString();
						if (ValueEditor(langMap, recStr, 2, double.MinValue, double.MaxValue, out srchRec)) {
							VisionRectangularSearchRegion roiSR = new VisionRectangularSearchRegion(srchRec[0], srchRec[1]);
							edge.SearchRegion = roiSR;
							recStr = edge.SearchRegion.ToString();
							this.IsModified = true;
						}
						return recStr;
					}
				)
			);

			propList.Add(CreateImageSourceView(langMap));
			propList.Add(CreateShowResultView(langMap));
			propList.Add(CreateResultAvailableView(langMap));
			propList.Add(CreateRoiView(langMap));
			propList.Add(CreateCommentView(langMap));

			return propList;
		}
		#endregion

		#region IResultable Implements
		/// <summary>取得適用於顯示結果的 <see cref="DataTable.TableName"/></summary>
		public string ResultTableName { get { return "EdgeLocator"; } }

		/// <summary>建立當前影像工具的執行結果清單。如欲隱藏特定欄位，請透過 <see cref="DataGridView.Columns"/> 進行操作</summary>
		/// <returns>對應的 執行結果清單</returns>
		public DataTable CreateDataTable() {
			DataTable dt = new DataTable(ResultTableName);
			DataColumn[] columns = GetResultColumnNames().Select(kvp => new DataColumn(kvp.Key) { Caption = kvp.Value }).ToArray();
			dt.Columns.AddRange(columns);

			IEdgeLocatorTool tool = mTool as IEdgeLocatorTool;
			if (tool.ResultsAvailable) {
				int idx = 1;
				List<Edge> results = tool.Results.ToList();
				results.ForEach(
					result => {
						dt.Rows.Add(
							new string[] {
								(idx++).ToString(),
								result.EdgePosition.X.ToString("F3"),
								result.EdgePosition.Y.ToString("F3"),
								result.EdgePosition.Degrees.ToString("F3"),
								result.Score.ToString("F2"),
								result.EdgeGroup.ToString(),
								result.Magnitude.ToString("F2"),
								result.MagnitudeScore.ToString("F2"),
								result.PositionScore.ToString("F2"),
								result.ProjectionAverage.ToString("F2"),
								result.ProjectionMagnitude.ToString("F2")
							}
						);
					}
				);
			}

			return dt;
		}

		/// <summary>取得結果清單的欄位名稱(<see cref="DataColumn.ColumnName"/>)與標題(<see cref="DataColumn.Caption"/>)對應表</summary>
		/// <returns>清單欄位與標題對應表</returns>
		public Dictionary<string, string> GetResultColumnNames() {
			return new Dictionary<string, string> {
				{ "inst" , "Instance" },
				{ "posX" , "Edge X" },
				{ "posY" , "Edge Y" },
				{ "angle" , "Angle" },
				{ "score", "Edge Score" },
				{ "fpg", "Frame/Group" },
				{ "magt", "Magnitude" },
				{ "msgtScore", "Magnitude Score" },
				{ "posScore", "Position Score" },
				{ "projAvg", "Projection Average" },
				{ "projMagt", "Projection Magnitude" }
			};
		}

		/// <summary>建立結果清單的預設欄位名稱與預設啟用狀態</summary>
		/// <returns>預設的欄位與致能</returns>
		public ResultableTable GetDefaultResultColumns() {
			List<ResultableColumn> columns = new List<ResultableColumn> {
				new ResultableColumn("inst", "Instance", true),
				new ResultableColumn("posX", "Edge X", true),
				new ResultableColumn("posY", "Edge Y", true),
				new ResultableColumn("angle", "Angle", true),
				new ResultableColumn("score", "Edge Score", true),
				new ResultableColumn("fpg", "Frame/Group", false),
				new ResultableColumn("magt", "Magnitude", false),
				new ResultableColumn("msgtScore", "Magnitude Score", false),
				new ResultableColumn("posScore", "Position Score", false),
				new ResultableColumn("projAvg", "Projection Average", false),
				new ResultableColumn("projMagt", "Projection Magnitude", false)
			};

			return new ResultableTable(ResultTableName, columns);
		}
        #endregion

        #region IResultOfAngle Implements
        /// <summary>取得或設定此影像工具是否作為最後回傳的角度定義</summary>
        public bool IsAngleReturner { get; set; }

        ///<summary>產生角度CVT語法</summary>
        ///<param name="code">程式碼主體</param>
        public void GenerateAngleCVT(List<string> code) {
            base.generateAngleCVT(code);
        }

        #endregion

        #region IXmlSavable Implements
        /// <summary>產生物件的 XML 相關資料描述</summary>
        /// <param name="nodeName">此物件之 XML 節點名稱</param>
        /// <returns>XML 節點</returns>
        public override XmlElmt CreateXmlData(string nodeName) {
			return GenerateXmlData(nodeName);
		}
		#endregion
	}

	/// <summary>Line Finder 工具包</summary>
	public class LineFinderPack : VisionToolPackBase, IResultOfLine {

		#region IResultOfAngle Implements
		/// <summary>取得或設定此影像工具是否作為最後回傳的角度定義</summary>
		public bool IsAngleReturner { get; set; }

        ///<summary>產生角度CVT語法</summary>
        ///<param name="code">程式碼主體</param>
        public void GenerateAngleCVT(List<string> code) {
            base.generateAngleCVT(code);
            code.Add($"pos.Degrees = {VariableName}.Results[0].Line.Degrees;");
        }

        #endregion

        #region Constructors
        /// <summary>建立影像工具包於指定的 Ace 資料夾，附帶建立 <see cref="TreeNode"/></summary>
        /// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
        /// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
        /// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
        public LineFinderPack(
			List<IVisionToolPack> toolColl,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base(toolColl, VisionToolType.LineFinder, toolFold, mainNode) {

		}

		/// <summary>從現有的影像工具進行複製</summary>
		/// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
		/// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
		/// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
		/// <param name="copyPack">欲複製的來源</param>
		public LineFinderPack(
			List<IVisionToolPack> toolColl,
			IVisionToolPack copyPack,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base(toolColl, copyPack, toolFold, mainNode) {

		}

		/// <summary>建構單純的影像工具包，不含 <seealso cref="TreeNode"/> 等父節點</summary>
		/// <param name="source">指定的影像來源</param>
		/// <param name="fold">欲存放此工具的 Ace 資料夾</param>
		public LineFinderPack(
			IVisionImageSource source,
			IAceObjectCollection fold
		) : base(VisionToolType.LineFinder, source, fold) {

		}

		/// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
		/// <param name="aceSrv">已連線的 ACE Server 端物件</param>
		/// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
		/// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
		public LineFinderPack(
			Dictionary<string, string> langMap,
			XmlElmt xmlData,
			IAceServer aceSrv
		) : base(xmlData, aceSrv, langMap) {

		}

		#endregion

		#region IPropertable Implements

		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public override List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
			List<PropertyView> propList = new List<PropertyView>();

			var lineFind = mTool as ILineFinderTool;

			mOfs = new VisionTransform(lineFind.Offset);
			if (mOfsShd != mOfs) {
				this.IsCompiled = false;
				mOfsShd = new VisionTransform(mOfs);
			}

			propList.Add(
				new PropertyView(
					langMap["PropLineAnglDev"],
					AccessLevel.None,
					langMap["TipLineAnglDev"],
					"Maximum Angle Deviation",
					lineFind.MaximumAngleDeviation.ToString("F3"),
					() => {
						List<float> maxAngDev;
						string angStr = lineFind.MaximumAngleDeviation.ToString("F3");
						if (ValueEditor(langMap, angStr, 1, 0, 20, out maxAngDev)) {
							lineFind.MaximumAngleDeviation = maxAngDev[0];
							angStr = lineFind.MaximumAngleDeviation.ToString("F3");
							this.IsModified = true;
						}
						return angStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropLinePosLv"],
					AccessLevel.None,
					langMap["TipLinePosLv"],
					"Positioning Level",
					lineFind.PositioningLevel.ToString(),
					() => {
						List<int> posLv;
						string posStr = lineFind.PositioningLevel.ToString();
						if (ValueEditor(langMap, posStr, 1, 10, 100, out posLv)) {
							lineFind.PositioningLevel = posLv[0];
							posStr = lineFind.PositioningLevel.ToString();
							this.IsModified = true;
						}
						return posStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropLineSmpLv"],
					AccessLevel.None,
					langMap["TipLineSmpLv"],
					"SubSampling Level",
					lineFind.SubSamplingLevel.ToString(),
					() => {
						List<int> subLv;
						string sampStr = lineFind.SubSamplingLevel.ToString();
						if (ValueEditor(langMap, sampStr, 1, 1, 8, out subLv)) {
							lineFind.SubSamplingLevel = subLv[0];
							sampStr = lineFind.SubSamplingLevel.ToString();
							this.IsModified = true;
						}
						return sampStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropLinePerc"],
					AccessLevel.None,
					langMap["TipLinePerc"],
					"Minimum Line Percentage",
					lineFind.MinimumLinePercentage.ToString("F3"),
					() => {
						List<float> linePercent;
						string percStr = lineFind.MinimumLinePercentage.ToString("F3");
						if (ValueEditor(langMap, percStr, 1, 0F, 100F, out linePercent)) {
							lineFind.MinimumLinePercentage = linePercent[0];
							percStr = lineFind.MinimumLinePercentage.ToString("F3");
							this.IsModified = true;
						}
						return percStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropLinePola"],
					AccessLevel.None,
					langMap["TipLinePola"],
					"Polarity Mode",
					langMap[lineFind.PolarityMode.GetFullName()],
					() => {
						LineFinderPolarity polarity;
						string polStr = lineFind.PolarityMode.ToString();
						if (EnumEditor(langMap, polStr, out polarity)) {
							lineFind.PolarityMode = polarity;
							polStr = lineFind.PolarityMode.ToString();
							this.IsModified = true;
						}
						return polStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropLineSchMode"],
					AccessLevel.None,
					langMap["TipLineSchMode"],
					"Search Mode",
					langMap[lineFind.SearchMode.GetFullName()],
					() => {
						LineFinderSearchMode srcMode;
						string srcStr = lineFind.SearchMode.ToString();
						if (EnumEditor(langMap, srcStr, out srcMode)) {
							lineFind.SearchMode = srcMode;
							srcStr = lineFind.SearchMode.ToString();
							this.IsModified = true;
						}
						return srcStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropLineGuid"],
					AccessLevel.None,
					langMap["TipLineGuid"],
					"Guideline Offset",
					lineFind.GuidelineOffset.ToString("F3"),
					() => {
						List<double> guide;
						string guidStr = lineFind.GuidelineOffset.ToString("F3");
						double width = lineFind.SearchRegion.Width / 2;
						if (ValueEditor(langMap, guidStr, 1, -width, width, out guide)) {
							lineFind.GuidelineOffset = guide[0];
							guidStr = lineFind.GuidelineOffset.ToString("F3");
							this.IsModified = true;
						}
						return guidStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropOfs"],
					AccessLevel.None,
					langMap["TipOfs"],
					"Offset",
					lineFind.Offset.ToString(),
					() => {
						List<double> ofs;
						string ofsStr = lineFind.Offset.ToString();
						if (ValueEditor(langMap, ofsStr, 3, double.MinValue, double.MaxValue, out ofs)) {
							VisionTransform ofsVT = new VisionTransform(ofs[0], ofs[1], ofs[2]);
							lineFind.Offset = ofsVT;
							ofsStr = lineFind.Offset.ToString();
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return ofsStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropRscReg"],
					AccessLevel.None,
					langMap["TipSchReg"],
					"Search Region / Region of Interest",
					lineFind.SearchRegion.ToString(),
					() => {
						List<double> srchRec;
						string recStr = lineFind.SearchRegion.ToString();
						if (ValueEditor(langMap, recStr, 2, double.MinValue, double.MaxValue, out srchRec)) {
							VisionRectangularSearchRegion roiSR = new VisionRectangularSearchRegion(srchRec[0], srchRec[1]);
							lineFind.SearchRegion = roiSR;
							recStr = lineFind.SearchRegion.ToString();
							this.IsModified = true;
						}
						return recStr;
					}
				)
			);

			propList.Add(CreateImageSourceView(langMap));
			propList.Add(CreateShowResultView(langMap));
			propList.Add(CreateResultAvailableView(langMap));
			propList.Add(CreateRoiView(langMap));
			propList.Add(CreateCommentView(langMap));

			return propList;
		}
		#endregion

		#region IResultable Implements
		/// <summary>取得適用於顯示結果的 <see cref="DataTable.TableName"/></summary>
		public string ResultTableName { get { return "LineFinder"; } }

		/// <summary>建立當前影像工具的執行結果清單。如欲隱藏特定欄位，請透過 <see cref="DataGridView.Columns"/> 進行操作</summary>
		/// <returns>對應的 執行結果清單</returns>
		public DataTable CreateDataTable() {
			DataTable dt = new DataTable(ResultTableName);
			DataColumn[] columns = GetResultColumnNames().Select(kvp => new DataColumn(kvp.Key) { Caption = kvp.Value }).ToArray();
			dt.Columns.AddRange(columns);

			ILineFinderTool tool = mTool as ILineFinderTool;
			if (tool.ResultsAvailable) {
				int idx = 1;
				List<LineFinderResults> results = tool.Results.ToList();
				results.ForEach(
					result => {
						dt.Rows.Add(
							new string[] {
								(idx++).ToString(),
								result.StartPoint.X.ToString("F3"),
								result.StartPoint.Y.ToString("F3"),
								result.EndPoint.X.ToString("F3"),
								result.EndPoint.Y.ToString("F3"),
								result.Line.CenterPoint.X.ToString("F3"),
								result.Line.CenterPoint.Y.ToString("F3"),
								result.Line.Degrees.ToString("F3"),
								result.AverageContrast.ToString("F2"),
								(result.FitQuality * 100).ToString("F2"),
								result.GroupIndex.ToString(),
								(result.MatchQuality * 100).ToString("F2")
							}
						);
					}
				);
			}

			return dt;
		}

		/// <summary>取得結果清單的欄位名稱(<see cref="DataColumn.ColumnName"/>)與標題(<see cref="DataColumn.Caption"/>)對應表</summary>
		/// <returns>清單欄位與標題對應表</returns>
		public Dictionary<string, string> GetResultColumnNames() {
			return new Dictionary<string, string> {
				{ "inst" , "Instance" },
				{ "srtX", "Start X" },
				{ "srtY", "Start Y" },
				{ "endX", "End X" },
				{ "endY", "End Y" },
				{ "cntX", "Center X" },
				{ "cntY", "Center Y" },
				{ "angle", "Angle" },
				{ "avgCont", "Average Contrast" },
				{ "fitQty", "Fit Quality" },
				{ "fpg", "Frame/Group" },
				{ "matchQty", "Match Quality" }
			};
		}

		/// <summary>建立結果清單的預設欄位名稱與預設啟用狀態</summary>
		/// <returns>預設的欄位與致能</returns>
		public ResultableTable GetDefaultResultColumns() {
			List<ResultableColumn> columns = new List<ResultableColumn> {
				new ResultableColumn("inst", "Instance", true),
				new ResultableColumn("srtX", "Start X", true),
				new ResultableColumn("srtY", "Start Y", true),
				new ResultableColumn("endX", "End X", true),
				new ResultableColumn("endY", "End Y", true),
				new ResultableColumn("cntX", "Center X", false),
				new ResultableColumn("cntY", "Center Y", false),
				new ResultableColumn("angle", "Angle", true),
				new ResultableColumn("avgCont", "Average Contrast", false),
				new ResultableColumn("fitQty", "Fit Quality", false),
				new ResultableColumn("fpg", "Frame/Group", false),
				new ResultableColumn("matchQty", "Match Quality", false)
			};

			return new ResultableTable(ResultTableName, columns);
		}
		#endregion

		#region IXmlSavable Implements
		/// <summary>產生物件的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		public override XmlElmt CreateXmlData(string nodeName) {
			return GenerateXmlData(nodeName);
		}
		#endregion
	}

	/// <summary>Arc Finder 工具包</summary>
	public class ArcFinderPack : VisionToolPackBase, IResultOfArc ,IResultOfAngle{

		#region IResultOfAngle Implements
		/// <summary>取得或設定此影像工具是否作為最後回傳的角度定義</summary>
		public bool IsAngleReturner { get; set; }

        ///<summary>產生角度CVT語法</summary>
        ///<param name="code">程式碼主體</param>
        public void GenerateAngleCVT(List<string> code) {
            base.generateAngleCVT(code);
            code.Add($"pos.Degrees = {VariableName}.Results[0].Arc.Rotation");
        }

        #endregion

        #region Constructors
        /// <summary>建立影像工具包於指定的 Ace 資料夾，附帶建立 <see cref="TreeNode"/></summary>
        /// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
        /// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
        /// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
        public ArcFinderPack(
			List<IVisionToolPack> toolColl,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base(toolColl, VisionToolType.ArcFinder, toolFold, mainNode) {

		}

		/// <summary>從現有的影像工具進行複製</summary>
		/// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
		/// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
		/// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
		/// <param name="copyPack">欲複製的來源</param>
		public ArcFinderPack(
			List<IVisionToolPack> toolColl,
			IVisionToolPack copyPack,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base(toolColl, copyPack, toolFold, mainNode) {

		}

		/// <summary>建構單純的影像工具包，不含 <seealso cref="TreeNode"/> 等父節點</summary>
		/// <param name="source">指定的影像來源</param>
		/// <param name="fold">欲存放此工具的 Ace 資料夾</param>
		public ArcFinderPack(
			IVisionImageSource source,
			IAceObjectCollection fold
		) : base(VisionToolType.ArcFinder, source, fold) {

		}

		/// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
		/// <param name="aceSrv">已連線的 ACE Server 端物件</param>
		/// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
		/// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
		public ArcFinderPack(
			Dictionary<string, string> langMap,
			XmlElmt xmlData,
			IAceServer aceSrv
		) : base(xmlData, aceSrv, langMap) {

		}

		#endregion

		#region IPropertable Implements

		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public override List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
			List<PropertyView> propList = new List<PropertyView>();

			var arcFind = mTool as IArcFinderTool;

			mOfs = new VisionTransform(arcFind.SearchRegion.Center);
			mOfs.Degrees = arcFind.SearchRegion.Rotation;
            if (mOfsShd != mOfs) {
                this.IsCompiled = false;
                mOfsShd = new VisionTransform(mOfs);
            }
            propList.Add(
				new PropertyView(
					langMap["PropArcEnc"],
					AccessLevel.None,
					langMap["TipArcEnc"],
					"Arc Must Be Totally Enclosed",
					GetBoolString(langMap, arcFind.ArcMustBeTotallyEnclosed),
					() => {
						bool enclose = arcFind.ArcMustBeTotallyEnclosed;
						if (BoolEditor(langMap, enclose, out enclose)) {
							arcFind.ArcMustBeTotallyEnclosed = enclose;
							this.IsModified = true;
						}
						return GetBoolString(langMap, arcFind.ArcMustBeTotallyEnclosed);
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropArcAnglDev"],
					AccessLevel.None,
					langMap["TipArcAnglDev"],
					"Maximum Angle Deviation",
					arcFind.MaximumAngleDeviation.ToString("F3"),
					() => {
						List<float> maxAngDev;
						string angStr = arcFind.MaximumAngleDeviation.ToString("F3");
						if (ValueEditor(langMap, angStr, 1, 0, 20, out maxAngDev)) {
							arcFind.MaximumAngleDeviation = maxAngDev[0];
							angStr = arcFind.MaximumAngleDeviation.ToString("F3");
							this.IsModified = true;
						}
						return angStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropArcPosLv"],
					AccessLevel.None,
					langMap["TipArcPosLv"],
					"Positioning Level",
					arcFind.PositioningLevel.ToString(),
					() => {
						List<int> posLv;
						string posStr = arcFind.PositioningLevel.ToString();
						if (ValueEditor(langMap, posStr, 1, 10, 100, out posLv)) {
							arcFind.PositioningLevel = posLv[0];
							posStr = arcFind.PositioningLevel.ToString();
							this.IsModified = true;
						}
						return posStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropArcSmpLv"],
					AccessLevel.None,
					langMap["TipArcSmpLv"],
					"SubSampling Level",
					arcFind.SubSamplingLevel.ToString(),
					() => {
						List<int> subLv;
						string sampStr = arcFind.SubSamplingLevel.ToString();
						if (ValueEditor(langMap, sampStr, 1, 1, 8, out subLv)) {
							arcFind.SubSamplingLevel = subLv[0];
							sampStr = arcFind.SubSamplingLevel.ToString();
							this.IsModified = true;
						}
						return sampStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropArcFit"],
					AccessLevel.None,
					langMap["TipArcFit"],
					"Fit Mode",
					langMap[arcFind.FitMode.GetFullName()],
					() => {
						ArcFinderFitMode fitMode;
						string fmStr = arcFind.FitMode.ToString();
						if (EnumEditor(langMap, fmStr, out fitMode)) {
							arcFind.FitMode = fitMode;
							fmStr = arcFind.FitMode.ToString();
							this.IsModified = true;
						}
						return fmStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropArcPerc"],
					AccessLevel.None,
					langMap["TipArcPerc"],
					"Minimum Arc Percentage",
					arcFind.MinimumArcPercentage.ToString("F3"),
					() => {
						List<float> linePercent;
						string percStr = arcFind.MinimumArcPercentage.ToString("F3");
						if (ValueEditor(langMap, percStr, 1, 0F, 100F, out linePercent)) {
							arcFind.MinimumArcPercentage = linePercent[0];
							percStr = arcFind.MinimumArcPercentage.ToString("F3");
							this.IsModified = true;
						}
						return percStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropArcPola"],
					AccessLevel.None,
					langMap["TipArcPola"],
					"Polarity Mode",
					langMap[arcFind.PolarityMode.GetFullName()],
					() => {
						ArcFinderPolarityMode polarity;
						string polStr = arcFind.PolarityMode.ToString();
						if (EnumEditor(langMap, polStr, out polarity)) {
							arcFind.PolarityMode = polarity;
							polStr = arcFind.PolarityMode.ToString();
							this.IsModified = true;
						}
						return polStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropArcSchMode"],
					AccessLevel.None,
					langMap["TipArcSchMode"],
					"Search Mode",
					langMap[arcFind.SearchMode.GetFullName()],
					() => {
						ArcFinderSearchMode srcMode;
						string srcStr = arcFind.SearchMode.ToString();
						if (EnumEditor(langMap, srcStr, out srcMode)) {
							arcFind.SearchMode = srcMode;
							srcStr = arcFind.SearchMode.ToString();
							this.IsModified = true;
						}
						return srcStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropLineGuid"],
					AccessLevel.None,
					langMap["TipLineGuid"],
					"Guideline Offset",
					arcFind.GuidelineOffset.ToString("F3"),
					() => {
						List<double> guide;
						string guidStr = arcFind.GuidelineOffset.ToString("F3");
						double width = arcFind.SearchRegion.Thickness / 2;
						if (ValueEditor(langMap, guidStr, 1, -width, width, out guide)) {
							arcFind.GuidelineOffset = guide[0];
							guidStr = arcFind.GuidelineOffset.ToString("F3");
							this.IsModified = true;
						}
						return guidStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropArcSchReg"],
					AccessLevel.None,
					langMap["TipArcSchReg"],
					"Search Region / Region of Interest",
					arcFind.SearchRegion.ToString(),
					() => {
						List<double> srchArc;
						string recStr = arcFind.SearchRegion.ToString();
						if (ValueEditor(langMap, recStr, 6, double.MinValue, double.MaxValue, out srchArc)) {
							VisionArc arc = new VisionArc(srchArc[0], srchArc[1], srchArc[2], srchArc[3], srchArc[4], srchArc[5]);
							arcFind.SearchRegion = arc;
							recStr = arcFind.SearchRegion.ToString();
							this.IsModified = true;
						}
						return recStr;
					}
				)
			);

			propList.Add(CreateImageSourceView(langMap));
			propList.Add(CreateShowResultView(langMap));
			propList.Add(CreateResultAvailableView(langMap));
			propList.Add(CreateRoiView(langMap));
			propList.Add(CreateCommentView(langMap));

			return propList;
		}
		#endregion

		#region IResultable Implements
		/// <summary>取得適用於顯示結果的 <see cref="DataTable.TableName"/></summary>
		public string ResultTableName { get { return "ArcFinder"; } }

		/// <summary>建立當前影像工具的執行結果清單。如欲隱藏特定欄位，請透過 <see cref="DataGridView.Columns"/> 進行操作</summary>
		/// <returns>對應的 執行結果清單</returns>
		public DataTable CreateDataTable() {
			DataTable dt = new DataTable(ResultTableName);
			DataColumn[] columns = GetResultColumnNames().Select(kvp => new DataColumn(kvp.Key) { Caption = kvp.Value }).ToArray();
			dt.Columns.AddRange(columns);

			IArcFinderTool tool = mTool as IArcFinderTool;
			if (tool.ResultsAvailable) {
				int idx = 1;
				List<ArcFinderResults> results = tool.Results.ToList();
				results.ForEach(
					result => {
						dt.Rows.Add(
							new string[] {
								(idx++).ToString(),
								result.Arc.Center.X.ToString("F3"),
								result.Arc.Center.Y.ToString("F3"),
								result.Arc.Opening.ToString("F3"),
								result.Arc.Rotation.ToString("F3"),
								result.Radius.ToString("F2"),
								result.AverageContrast.ToString("F2"),
								(result.FitQuality * 100).ToString("F2"),
								(result.MatchQuality * 100).ToString("F2"),
								result.GroupIndex.ToString()
							}
						);
					}
				);
			}

			return dt;
		}

		/// <summary>取得結果清單的欄位名稱(<see cref="DataColumn.ColumnName"/>)與標題(<see cref="DataColumn.Caption"/>)對應表</summary>
		/// <returns>清單欄位與標題對應表</returns>
		public Dictionary<string, string> GetResultColumnNames() {
			return new Dictionary<string, string> {
				{ "inst" , "Instance" },
				{ "arcX", "Arc X" },
				{ "arcY", "Arc Y" },
				{ "open", "Opening" },
				{ "rotate", "Rotation" },
				{ "rad", "Radius" },
				{ "avgCont", "Average Contrast" },
				{ "fitQty", "Fit Quality" },
				{ "matchQty", "Match Quality" },
				{ "fpg", "Frame/Group" }
			};
		}

		/// <summary>建立結果清單的預設欄位名稱與預設啟用狀態</summary>
		/// <returns>預設的欄位與致能</returns>
		public ResultableTable GetDefaultResultColumns() {
			List<ResultableColumn> columns = new List<ResultableColumn> {
				new ResultableColumn("inst", "Instance", true),
				new ResultableColumn("arcX", "Arc X", true),
				new ResultableColumn("arcY", "Arc Y", true),
				new ResultableColumn("open", "Opening", false),
				new ResultableColumn("rotate", "Rotation", false),
				new ResultableColumn("rad", "Radius", true),
				new ResultableColumn("avgCont", "Average Contrast", true),
				new ResultableColumn("fitQty", "Fit Quality", true),
				new ResultableColumn("matchQty", "Match Quality", true),
				new ResultableColumn("fpg", "Frame/Group", false)
			};

			return new ResultableTable(ResultTableName, columns);
		}
		#endregion

		#region IXmlSavable Implements
		/// <summary>產生物件的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		public override XmlElmt CreateXmlData(string nodeName) {
			return GenerateXmlData(nodeName);
		}
		#endregion
	}

	/// <summary>Arc Finder 工具包</summary>
	public class PointFinderPack : VisionToolPackBase, IResultOfPoint {

        #region Constructors
        /// <summary>建立影像工具包於指定的 Ace 資料夾，附帶建立 <see cref="TreeNode"/></summary>
        /// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
        /// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
        /// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
        public PointFinderPack(
			List<IVisionToolPack> toolColl,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base(toolColl, VisionToolType.PointFinder, toolFold, mainNode) {

		}

		/// <summary>從現有的影像工具進行複製</summary>
		/// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
		/// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
		/// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
		/// <param name="copyPack">欲複製的來源</param>
		public PointFinderPack(
			List<IVisionToolPack> toolColl,
			IVisionToolPack copyPack,
			IAceObjectCollection toolFold,
			TreeNode mainNode
		) : base(toolColl, copyPack, toolFold, mainNode) {

		}

		/// <summary>建構單純的影像工具包，不含 <seealso cref="TreeNode"/> 等父節點</summary>
		/// <param name="source">指定的影像來源</param>
		/// <param name="fold">欲存放此工具的 Ace 資料夾</param>
		public PointFinderPack(
			IVisionImageSource source,
			IAceObjectCollection fold
		) : base(VisionToolType.PointFinder, source, fold) {

		}

		/// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
		/// <param name="aceSrv">已連線的 ACE Server 端物件</param>
		/// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
		/// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
		public PointFinderPack(
			Dictionary<string, string> langMap,
			XmlElmt xmlData,
			IAceServer aceSrv
		) : base(xmlData, aceSrv, langMap) {

		}

		#endregion

		#region IPropertable Implements

		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public override List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
			List<PropertyView> propList = new List<PropertyView>();

			var poFind = mTool as IPointFinderTool;

			mOfs = new VisionTransform(poFind.Offset);
			if (mOfsShd != mOfs) {
				this.IsCompiled = false;
				mOfsShd = new VisionTransform(mOfs);
			}

			propList.Add(
				new PropertyView(
					langMap["PropPotPosLv"],
					AccessLevel.None,
					langMap["TipPotPosLv"],
					"Positioning Level",
					poFind.PositioningLevel.ToString(),
					() => {
						List<int> posLv;
						string posStr = poFind.PositioningLevel.ToString();
						if (ValueEditor(langMap, posStr, 1, 10, 100, out posLv)) {
							poFind.PositioningLevel = posLv[0];
							posStr = poFind.PositioningLevel.ToString();
							this.IsModified = true;
						}
						return posStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPotSmpLv"],
					AccessLevel.None,
					langMap["TipPotSmpLv"],
					"SubSampling Level",
					poFind.SubSamplingLevel.ToString(),
					() => {
						List<int> subLv;
						string sampStr = poFind.SubSamplingLevel.ToString();
						if (ValueEditor(langMap, sampStr, 1, 1, 8, out subLv)) {
							poFind.SubSamplingLevel = subLv[0];
							sampStr = poFind.SubSamplingLevel.ToString();
							this.IsModified = true;
						}
						return sampStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPotPola"],
					AccessLevel.None,
					langMap["TipPotPola"],
					"Polarity Mode",
					langMap[poFind.PolarityMode.GetFullName()],
					() => {
						PointFinderPolarity polarity;
						string polStr = poFind.PolarityMode.ToString();
						if (EnumEditor(langMap, polStr, out polarity)) {
							poFind.PolarityMode = polarity;
							polStr = poFind.PolarityMode.ToString();
							this.IsModified = true;
						}
						return polStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPotSchMode"],
					AccessLevel.None,
					langMap["TipPotSchMode"],
					"Search Mode",
					langMap[poFind.SearchMode.GetFullName()],
					() => {
						PointFinderSearchMode srcMode;
						string srcStr = poFind.SearchMode.ToString();
						if (EnumEditor(langMap, srcStr, out srcMode)) {
							poFind.SearchMode = srcMode;
							srcStr = poFind.SearchMode.ToString();
							this.IsModified = true;
						}
						return srcStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropLineGuid"],
					AccessLevel.None,
					langMap["TipLineGuid"],
					"Guideline Offset",
					poFind.GuidelineOffset.ToString("F3"),
					() => {
						List<double> guide;
						string guidStr = poFind.GuidelineOffset.ToString("F3");
						double width = poFind.SearchRegion.Width / 2;
						if (ValueEditor(langMap, guidStr, 1, -width, width, out guide)) {
							poFind.GuidelineOffset = guide[0];
							guidStr = poFind.GuidelineOffset.ToString("F3");
							this.IsModified = true;
						}
						return guidStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropOfs"],
					AccessLevel.None,
					langMap["TipOfs"],
					"Offset",
					poFind.Offset.ToString(),
					() => {
						List<double> ofs;
						string ofsStr = poFind.Offset.ToString();
						if (ValueEditor(langMap, ofsStr, 3, double.MinValue, double.MaxValue, out ofs)) {
							VisionTransform ofsVT = new VisionTransform(ofs[0], ofs[1], ofs[2]);
							poFind.Offset = ofsVT;
							ofsStr = poFind.Offset.ToString();
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return ofsStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropRscReg"],
					AccessLevel.None,
					langMap["TipSchReg"],
					"Search Region / Region of Interest",
					poFind.SearchRegion.ToString(),
					() => {
						List<double> srchRec;
						string recStr = poFind.SearchRegion.ToString();
						if (ValueEditor(langMap, recStr, 2, double.MinValue, double.MaxValue, out srchRec)) {
							VisionRectangularSearchRegion roiSR = new VisionRectangularSearchRegion(srchRec[0], srchRec[1]);
							poFind.SearchRegion = roiSR;
							recStr = poFind.SearchRegion.ToString();
							this.IsModified = true;
						}
						return recStr;
					}
				)
			);

			propList.Add(CreateImageSourceView(langMap));
			propList.Add(CreateShowResultView(langMap));
			propList.Add(CreateResultAvailableView(langMap));
			propList.Add(CreateRoiView(langMap));
			propList.Add(CreateCommentView(langMap));

			return propList;
		}
		#endregion

		#region IResultable Implements
		/// <summary>取得適用於顯示結果的 <see cref="DataTable.TableName"/></summary>
		public string ResultTableName { get { return "PointFinder"; } }

		/// <summary>建立當前影像工具的執行結果清單。如欲隱藏特定欄位，請透過 <see cref="DataGridView.Columns"/> 進行操作</summary>
		/// <returns>對應的 執行結果清單</returns>
		public DataTable CreateDataTable() {
			DataTable dt = new DataTable(ResultTableName);
			DataColumn[] columns = GetResultColumnNames().Select(kvp => new DataColumn(kvp.Key) { Caption = kvp.Value }).ToArray();
			dt.Columns.AddRange(columns);

			IPointFinderTool tool = mTool as IPointFinderTool;
			if (tool.ResultsAvailable) {
				int idx = 1;
				List<PointFinderResults> results = tool.Results.ToList();
				results.ForEach(
					result => {
						dt.Rows.Add(
							new string[] {
								(idx++).ToString(),
								result.Point.X.ToString("F3"),
								result.Point.Y.ToString("F3"),
								result.AverageContrast.ToString("F2"),
								result.GroupIndex.ToString()
							}
						);
					}
				);
			}

			return dt;
		}

		/// <summary>取得結果清單的欄位名稱(<see cref="DataColumn.ColumnName"/>)與標題(<see cref="DataColumn.Caption"/>)對應表</summary>
		/// <returns>清單欄位與標題對應表</returns>
		public Dictionary<string, string> GetResultColumnNames() {
			return new Dictionary<string, string> {
				{ "inst" , "Instance" },
				{ "posX", "Position X" },
				{ "posY", "Position Y" },
				{ "avgCont", "Average Contrast" },
				{ "fpg", "Frame/Group" }
			};
		}

		/// <summary>建立結果清單的預設欄位名稱與預設啟用狀態</summary>
		/// <returns>預設的欄位與致能</returns>
		public ResultableTable GetDefaultResultColumns() {
			List<ResultableColumn> columns = new List<ResultableColumn> {
				new ResultableColumn("inst", "Instance", true),
				new ResultableColumn("posX", "Position X", true),
				new ResultableColumn("posY", "Position Y", true),
				new ResultableColumn("avgCont", "Average Contrast", true),
				new ResultableColumn("fpg", "Frame/Group", false)
			};

			return new ResultableTable(ResultTableName, columns);
		}
		#endregion

		#region IXmlSavable Implements
		/// <summary>產生物件的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		public override XmlElmt CreateXmlData(string nodeName) {
            return GenerateXmlData(nodeName);
		}
		#endregion
	}

	/// <summary>Line Finder 工具包</summary>
	public class ImageProcessingPack : VisionToolPackBase {

		#region Constructors
		/// <summary>建立影像工具包於指定的 Ace 資料夾，附帶建立 <see cref="TreeNode"/></summary>
		/// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
		/// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
		/// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
		public ImageProcessingPack(
			List<IVisionToolPack> toolColl, 
			IAceObjectCollection toolFold, 
			TreeNode mainNode
		) : base(toolColl, VisionToolType.ImageProcessing, toolFold, mainNode) {
			
		}

		/// <summary>從現有的影像工具進行複製</summary>
		/// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
		/// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
		/// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
		/// <param name="copyPack">欲複製的來源</param>
		public ImageProcessingPack(
			List<IVisionToolPack> toolColl, 
			IVisionToolPack copyPack, 
			IAceObjectCollection toolFold, 
			TreeNode mainNode
		) : base(toolColl, copyPack, toolFold, mainNode) {
			
		}

		/// <summary>建構單純的影像工具包，不含 <seealso cref="TreeNode"/> 等父節點</summary>
		/// <param name="source">指定的影像來源</param>
		/// <param name="fold">欲存放此工具的 Ace 資料夾</param>
		public ImageProcessingPack(
			IVisionImageSource source, 
			IAceObjectCollection fold
		) : base(VisionToolType.ImageProcessing, source, fold) {
			
		}

		/// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
		/// <param name="aceSrv">已連線的 ACE Server 端物件</param>
		/// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
		/// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
		public ImageProcessingPack(
			Dictionary<string, string> langMap, 
			XmlElmt xmlData, 
			IAceServer aceSrv
		) : base(xmlData, aceSrv, langMap) {
			
		}

		#endregion

		#region Private Methods
		private void SetRowVisible(IEnumerable<DataGridViewRow> rows) {
			IImageProcessingTool imgProc = mTool as IImageProcessingTool;
			ImageProcessingOperation op = imgProc.Operation;
			bool visible = false;
			string rowName = string.Empty;
			foreach (DataGridViewRow row in rows) {
				rowName = row.Cells[0].Value.ToString();
				switch (rowName) {
					case "Arithmetic Clipping Mode":
					case "Arithmetic Constant":
					case "Arithmetic Scale":
						visible = (op <= ImageProcessingOperation.ArithmeticDarkest && imgProc.ImageOperandSource == null);
						break;
					case "Assignment Constant":
						visible = (op == ImageProcessingOperation.AssignmentInitialization);
						break;
					case "Filtering Clipping Mode":
					case "Filtering Kernel Size":
					case "Filtering Scale":
						visible = (ImageProcessingOperation.FilteringAverage <= op && op <= ImageProcessingOperation.FilteringMedian);
						break;
					case "Histogram Threshold":
						visible = (op == ImageProcessingOperation.HistogramLightThreshold || op == ImageProcessingOperation.HistogramDarkThreshold);
						break;
					case "Logical Constant":
						visible = (ImageProcessingOperation.LogicalAnd <= op && op <= ImageProcessingOperation.LogicalNOr);
						break;
					case "Morphological Neighborhood Size":
						visible = (ImageProcessingOperation.MorphologicalDilate <= op && op <= ImageProcessingOperation.MorphologicalOpen);
						break;
					case "Transform Flag":
						visible = (ImageProcessingOperation.TransformFFT <= op && op <= ImageProcessingOperation.TransformDCT);
						break;
					case "Operand Image":
						visible = (ImageProcessingOperation.ArithmeticAddition <= op && op <= ImageProcessingOperation.ArithmeticDivision);
						break;
					case "Operation":
					case "Image Source":
					case "Comment":
					default:
						visible = true;
						break;
				}
				row.Visible = visible;
			}
		}
		#endregion

		#region IPropertable Implements

		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public override List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
			List<PropertyView> propList = new List<PropertyView>();

			var imgProc = mTool as IImageProcessingTool;
			var procOp = imgProc.Operation;

			propList.Add(
				new PropertyView(
					langMap["PropPrcOp"],
					AccessLevel.None,
					langMap["TipPrcOp"],
					"Operation",
					langMap[procOp.GetFullName()],
					() => {
						ImageProcessingOperation op;
						string opStr = imgProc.Operation.ToString();
						if (EnumEditor(langMap, opStr, out op)) {
							imgProc.Operation = op;
							opStr = imgProc.Operation.ToString();
							this.IsModified = true;
						}
						return opStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPrcAthClip"],
					AccessLevel.None,
					langMap["TipPrcAthClip"],
					"Arithmetic Clipping Mode",
					langMap[imgProc.ArithmeticClippingMode.GetFullName()],
					() => {
						ImageProcessingClippingMode clipMode;
						string clipStr = imgProc.ArithmeticClippingMode.ToString();
						if (EnumEditor(langMap, clipStr, out clipMode)) {
							imgProc.ArithmeticClippingMode = clipMode;
							clipStr = imgProc.ArithmeticClippingMode.ToString();
							this.IsModified = true;
						}
						return clipStr;
					},
					() => procOp <= ImageProcessingOperation.ArithmeticDarkest && imgProc.ImageOperandSource == null
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPrcAthConst"],
					AccessLevel.None,
					langMap["TipPrcAthConst"],
					"Arithmetic Constant",
					imgProc.ArithmeticConstant.ToString(),
					() => {
						List<int> arConst;
						string conStr = imgProc.ArithmeticConstant.ToString();
						if (ValueEditor(langMap, conStr, 1, 1, 256, out arConst)) {
							imgProc.ArithmeticConstant = arConst[0];
							conStr = imgProc.ArithmeticConstant.ToString();
							this.IsModified = true;
						}
						return conStr;
					},
					() => procOp <= ImageProcessingOperation.ArithmeticDarkest && imgProc.ImageOperandSource == null
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPrcAthScale"],
					AccessLevel.None,
					langMap["TipPrcAthScale"],
					"Arithmetic Scale",
					imgProc.ArithmeticScale.ToString("F2"),
					() => {
						List<int> arScale;
						string scStr = imgProc.ArithmeticScale.ToString("F2");
						if (ValueEditor(langMap, scStr, 1, 1, 100, out arScale)) {
							imgProc.ArithmeticScale = arScale[0];
							scStr = imgProc.ArithmeticScale.ToString("F2");
							this.IsModified = true;
						}
						return scStr;
					},
					() => procOp <= ImageProcessingOperation.ArithmeticDarkest && imgProc.ImageOperandSource == null
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPrcAsmConst"],
					AccessLevel.None,
					langMap["TipPrcAsmConst"],
					"Assignment Constant",
					imgProc.AssignmentConstant.ToString(),
					() => {
						List<int> asConst;
						string conStr = imgProc.AssignmentConstant.ToString();
						if (ValueEditor(langMap, conStr, 1, -10000, 10000, out asConst)) {
							imgProc.AssignmentConstant = asConst[0];
							conStr = imgProc.AssignmentConstant.ToString();
							this.IsModified = true;
						}
						return conStr;
					},
					() => procOp == ImageProcessingOperation.AssignmentInitialization
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPrcFiltClip"],
					AccessLevel.None,
					langMap["TipPrcFiltClip"],
					"Filtering Clipping Mode",
					langMap[imgProc.FilteringClippingMode.GetFullName()],
					() => {
						ImageProcessingClippingMode filtClipMode;
						string clipStr = imgProc.FilteringClippingMode.ToString();
						if (EnumEditor(langMap, clipStr, out filtClipMode)) {
							imgProc.FilteringClippingMode = filtClipMode;
							clipStr = imgProc.FilteringClippingMode.ToString();
							this.IsModified = true;
						}
						return clipStr;
					},
					() => ImageProcessingOperation.FilteringAverage <= procOp && procOp <= ImageProcessingOperation.FilteringMedian
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPrcFiltKern"],
					AccessLevel.None,
					langMap["TipPrcFiltKern"],
					"Filtering Kernel Size",
					langMap[imgProc.FilteringKernelSize.GetFullName()],
					() => {
						KernelSize knSize;
						string kerStr = imgProc.FilteringKernelSize.ToString();
						if (EnumEditor(langMap, kerStr, out knSize)) {
							imgProc.FilteringKernelSize = knSize;
							kerStr = imgProc.FilteringKernelSize.ToString();
							this.IsModified = true;
						}
						return kerStr;
					},
					() => ImageProcessingOperation.FilteringAverage <= procOp && procOp <= ImageProcessingOperation.FilteringMedian
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPrcFiltScale"],
					AccessLevel.None,
					langMap["TipPrcFiltScale"],
					"Filtering Scale",
					imgProc.FilteringScale.ToString("F2"),
					() => {
						List<float> filtScale;
						string scStr = imgProc.FilteringScale.ToString("F2");
						if (ValueEditor(langMap, scStr, 1, 0, 10000, out filtScale)) {
							imgProc.FilteringScale = filtScale[0];
							scStr = imgProc.FilteringScale.ToString("F2");
							this.IsModified = true;
						}
						return scStr;
					},
					() => ImageProcessingOperation.FilteringAverage <= procOp && procOp <= ImageProcessingOperation.FilteringMedian
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPrcHisThr"],
					AccessLevel.None,
					langMap["TipPrcHisThr"],
					"Histogram Threshold",
					imgProc.HistogramThreshold.ToString(),
					() => {
						List<int> hisThr;
						string thrStr = imgProc.HistogramThreshold.ToString();
						if (ValueEditor(langMap, thrStr, 1, 0, 255, out hisThr)) {
							imgProc.HistogramThreshold = hisThr[0];
							thrStr = imgProc.HistogramThreshold.ToString();
							this.IsModified = true;
						}
						return thrStr;
					},
					() => procOp == ImageProcessingOperation.HistogramLightThreshold || procOp == ImageProcessingOperation.HistogramDarkThreshold
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPrcLogiConst"],
					AccessLevel.None,
					langMap["TipPrcLogiConst"],
					"Logical Constant",
					imgProc.LogicalConstant.ToString(),
					() => {
						List<int> logConst;
						string conStr = imgProc.LogicalConstant.ToString();
						if (ValueEditor(langMap, conStr, 1, -10000, 10000, out logConst)) {
							imgProc.LogicalConstant = logConst[0];
							conStr = imgProc.LogicalConstant.ToString();
							this.IsModified = true;
						}
						return conStr;
					},
					() => ImageProcessingOperation.LogicalAnd <= procOp && procOp <= ImageProcessingOperation.LogicalNOr
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPrcMorpNbh"],
					AccessLevel.None,
					langMap["TipPrcMorpNbh"],
					"Morphological Neighborhood Size",
					imgProc.MorphologicalNeighborhoodSize.ToString(),
					null,
					() => ImageProcessingOperation.MorphologicalDilate <= procOp && procOp <= ImageProcessingOperation.MorphologicalOpen
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPrcTran"],
					AccessLevel.None,
					langMap["TipPrcTran"],
					"Transform Flags",
					langMap[imgProc.TransformFlags.GetFullName()],
					() => {
						ImageProcessingTransformFlag transFlag;
						string trnsStr = imgProc.TransformFlags.ToString();
						if (EnumEditor(langMap, trnsStr, out transFlag)) {
							imgProc.TransformFlags = transFlag;
							trnsStr = imgProc.TransformFlags.ToString();
							this.IsModified = true;
						}
						return trnsStr;
					},
					() => ImageProcessingOperation.TransformFFT <= procOp && procOp <= ImageProcessingOperation.TransformDCT
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropPrcOpImg"],
					AccessLevel.None,
					langMap["TipPrcOpImg"],
					"Image Operand Source",
					imgProc.ImageOperandSource?.FullPath ?? "None",
					() => {
						string curOpImg = imgProc.ImageOperandSource?.FullPath ?? string.Empty;
						string tarOpImg;
						if (ImageSourceEditor(langMap, curOpImg, out tarOpImg, true)) {
							if (!string.IsNullOrEmpty(tarOpImg) && !("None".Equals(tarOpImg))) {
								var newTool = mTool.AceServer.Root[tarOpImg] as IVisionImageSource;
								if (newTool != null) imgProc.ImageOperandSource = newTool;
								curOpImg = imgProc.ImageOperandSource?.FullPath ?? string.Empty;
							} else {
								imgProc.ImageOperandSource = null;
								curOpImg = "None";
							}
							this.IsModified = true;
						}
						return curOpImg;
					},
					() => ImageProcessingOperation.ArithmeticAddition <= procOp && procOp <= ImageProcessingOperation.ArithmeticDivision
				)
			);

			propList.Add(CreateImageSourceView(langMap));
			propList.Add(CreateCommentView(langMap));

			return propList;
		}
		#endregion

		#region IXmlSavable Implements
		/// <summary>產生物件的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		public override XmlElmt CreateXmlData(string nodeName) {
			return GenerateXmlData(nodeName);
		}
		#endregion
	}

    ///<summary>計算影像工具包基底</summary>
    public abstract class CalculatedToolBase :VisionToolPackBase, ICalculatedToolPack {
       
        #region Declaration Fields

        ///<summary>載入參考工具方法</summary>
        protected Action<List<IVisionToolPack>> mLoadRef = null;
        ///<summary>參考工具對照表</summary>
        protected  Dictionary<string, IVisionToolPack> mRef = null;
        ///<summary>TreeNode函式庫</summary>
        protected TreeNodeMeth mTreeNodeMeth = new TreeNodeMeth();

        #endregion Declaratikon Fields

        #region Function - Constructors
        
        /// <summary>建立影像工具包於指定的 Ace 資料夾，附帶建立 <see cref="TreeNode"/></summary>
		/// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
		/// <param name="toolType">欲建立的影像工具類型</param>
		/// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
		/// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
		public CalculatedToolBase(
            List<IVisionToolPack> toolColl,
            VisionToolType toolType,
            IAceObjectCollection toolFold,
            TreeNode mainNode)
            : base(toolColl, toolType, toolFold, mainNode) {
            mRef = GetRefTool();
        }

        /// <summary>從現有的影像工具進行複製</summary>
		/// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
		/// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
		/// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
		/// <param name="copyPack">欲複製的來源</param>
		public CalculatedToolBase(
            List<IVisionToolPack> toolColl,
            IVisionToolPack copyPack,
            IAceObjectCollection toolFold,
            TreeNode mainNode
        ) : base(toolColl, copyPack, toolFold, mainNode) {
            mRef = GetRefTool();
        }

        /// <summary>建構單純的影像工具包，不含 <seealso cref="TreeNode"/> 等父節點</summary>
		/// <param name="toolType">影像工具類型</param>
		/// <param name="source">指定的影像來源</param>
		/// <param name="fold">欲存放此工具的 Ace 資料夾</param>
		public CalculatedToolBase(
            VisionToolType toolType,
            IVisionImageSource source, 
            IAceObjectCollection fold
        ) : base(VisionToolType.CalculatedPoint, source, fold) {
            mRef = GetRefTool();
        }

        /// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
		/// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
		/// <param name="aceSrv">已連線的 ACE Server 端物件</param>
		/// <param name="langMap">多國語系之對應清單</param>
		/// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
		/// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
		public CalculatedToolBase(
            XmlElmt xmlData,
            IAceServer aceSrv, 
            Dictionary<string, string> langMap
        ) : base(xmlData, aceSrv, langMap) {
            mRef = GetRefTool();
        }

        #endregion Funciton - Constructors

        #region Funciton - Protected Methods

        ///<summary>取得參考工具定義</summary>
        protected abstract Dictionary<string, IVisionToolPack> GetRefTool();

        ///<summary>取得影像專案中所有影像工具清單</summary>
        ///<param name="predicate">過濾條件</param>
        ///<returns>所有影像工具清單</returns>
        protected List<IVisionToolPack> GetToolList(Func<IVisionToolPack, bool> predicate = null) {
            List<IVisionToolPack> toolList = new List<IVisionToolPack>();
            mTreeNodeMeth.SearchVisionToolPack(mNode.Parent, ref toolList);
            if (predicate != null) toolList = toolList.FindAll(t => predicate(t));
            return toolList;
        }

        ///<summary>設定參考影像工具</summary>
        ///<param name="xmlData">Xml資料</param>
        ///<param name="elmtName">影像工具名稱</param>
        ///<param name="tools">已載入影像工具集合</param>
        ///<param name="setRef">參考設定方法</param>
        protected void SetRef(XmlElmt xmlData, string elmtName, List<IVisionToolPack> tools, Action<IVisionTool> setRef) {
            XmlElmt childData = null;
            if (xmlData.Element(elmtName, out childData)) {
                if (!string.IsNullOrEmpty(childData.Value)) {
                    mRef[elmtName] = tools.Find(t => t.VariableName == childData.Value);
                    setRef(mRef[elmtName]?.Tool as IVisionTool);
                }
            }

        }
        
        #endregion Function - Protected Methods

        #region ICalculatedTool Implements

        ///<summary>是否已載入參考影像工具</summary>
        public bool IsLoadRef { get; protected set; } = true;

        ///<summary>載入參考影像工具</summary>
        ///<param name="tools">已載入的影像工具集合</param>
        public void LoadReference(List<IVisionToolPack> tools) {
            mLoadRef(tools);
        }

        #endregion ICalculatedTool Implements

    }

    ///<summary>求交點 工具包</summary>
    public class CalculatedPoint : CalculatedToolBase, IResultOfPoint, ICalculatedToolPack {

        #region Constructors
        /// <summary>建立影像工具包於指定的 Ace 資料夾，附帶建立 <see cref="TreeNode"/></summary>
        /// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
        /// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
        /// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
        public CalculatedPoint(
            List<IVisionToolPack> toolColl,
            IAceObjectCollection toolFold,
            TreeNode mainNode
        ) : base(toolColl, VisionToolType.CalculatedPoint, toolFold, mainNode) {

        }

        /// <summary>從現有的影像工具進行複製</summary>
        /// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
        /// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
        /// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
        /// <param name="copyPack">欲複製的來源</param>
        public CalculatedPoint(
            List<IVisionToolPack> toolColl,
            IVisionToolPack copyPack,
            IAceObjectCollection toolFold,
            TreeNode mainNode
        ) : base(toolColl, copyPack, toolFold, mainNode) {

        }

        /// <summary>建構單純的影像工具包，不含 <seealso cref="TreeNode"/> 等父節點</summary>
        /// <param name="source">指定的影像來源</param>
        /// <param name="fold">欲存放此工具的 Ace 資料夾</param>
        public CalculatedPoint(
            IVisionImageSource source,
            IAceObjectCollection fold
        ) : base(VisionToolType.CalculatedPoint, source, fold) {

        }

        /// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
        /// <param name="langMap">多國語系之對應清單</param>
        /// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
        /// <param name="aceSrv">已連線的 ACE Server 端物件</param>
        /// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
        /// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
        public CalculatedPoint(
            Dictionary<string, string> langMap,
            XmlElmt xmlData,
            IAceServer aceSrv
        ) : base(xmlData, aceSrv, langMap) {
            XmlElmt childData = null;
            var callPoint = mTool as ICalculatedPointTool;
            if (xmlData.Element("Mode", out childData)) {
                callPoint.Mode = (CalculatedPointToolMode)Enum.Parse(typeof(CalculatedPointToolMode), childData.Value);
            }

            IsLoadRef = false;
            if (xmlData.Element("Ofs", out childData)) {
                double[] val = childData.Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .ConvertAll(v => Convert.ToDouble(v));
                callPoint.Offset = new VisionPoint(val[0], val[1]);
            }
            mLoadRef = tools => {
                SetRef(xmlData, "Point1", tools, t => callPoint.Point1.Reference = t);
                SetRef(xmlData, "Point2", tools, t => callPoint.Point2.Reference = t);
                SetRef(xmlData, "Line1", tools, t => callPoint.Line1.Reference = t);
                SetRef(xmlData, "Line2", tools, t => callPoint.Line2.Reference = t);
                SetRef(xmlData, "Arc1", tools, t => callPoint.Arc1.Reference = t);
                SetRef(xmlData, "Arc2", tools, t => callPoint.Arc2.Reference = t);
                IsLoadRef = true;
                mLoadRef = null;
            };
            
        }

        #endregion

        #region IResultable Implements
        /// <summary>取得適用於顯示結果的 <see cref="DataTable.TableName"/></summary>
        public string ResultTableName { get { return "CalculatedPoint"; } }

        /// <summary>建立當前影像工具的執行結果清單。如欲隱藏特定欄位，請透過 <see cref="DataGridView.Columns"/> 進行操作</summary>
        /// <returns>對應的 執行結果清單</returns>
        public DataTable CreateDataTable() {
            DataTable dt = new DataTable(ResultTableName);
            DataColumn[] columns = GetResultColumnNames().Select(kvp => new DataColumn(kvp.Key) { Caption = kvp.Value }).ToArray();
            dt.Columns.AddRange(columns);

            ICalculatedPointTool tool = mTool as ICalculatedPointTool;
            if (tool.ResultsAvailable) {
                int idx = 1;
                List<VisionPointResult> results = tool.Results.ToList();
                results.ForEach(
                    result => {
                        dt.Rows.Add(
                            new string[] {
                                (idx++).ToString(),
                                result.Point.X.ToString("F3"),
                                result.Point.Y.ToString("F3")
                            }
                        );
                    }
                );
            }

            return dt;
        }

        /// <summary>取得結果清單的欄位名稱(<see cref="DataColumn.ColumnName"/>)與標題(<see cref="DataColumn.Caption"/>)對應表</summary>
        /// <returns>清單欄位與標題對應表</returns>
        public Dictionary<string, string> GetResultColumnNames() {
            return new Dictionary<string, string> {
                { "inst" , "Instance" },
                { "posX", "Position X" },
                { "posY", "Position Y" }
            };
        }

        /// <summary>建立結果清單的預設欄位名稱與預設啟用狀態</summary>
        /// <returns>預設的欄位與致能</returns>
        public ResultableTable GetDefaultResultColumns() {
            List<ResultableColumn> columns = new List<ResultableColumn> {
                new ResultableColumn("inst", "Instance", true),
                new ResultableColumn("posX", "Position X", true),
                new ResultableColumn("posY", "Position Y", true)
            };

            return new ResultableTable(ResultTableName, columns);
        }
        #endregion

        #region IPropertable Implements

        /// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
        /// <param name="langMap">各國語系之對應清單</param>
        /// <returns>對應的屬性檢視</returns>
        public override List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
            Stat stt = Stat.SUCCESS;
            var propList = new List<PropertyView>();

            var calPoint = mTool as ICalculatedPointTool;

            mOfs = new VisionTransform(calPoint.Offset);
            if (mOfsShd != mOfs) {
                this.IsCompiled = false;
                mOfsShd = new VisionTransform(mOfs);
            }

            propList.Add(
                new PropertyView(
                    langMap["PropCalPointMod"],
                    AccessLevel.None,
                    langMap["TipCalPointMod"],
                    "CalculatedPointTool Mode",
                    langMap[calPoint.Mode.GetFullName()],
                    () => {
                        CalculatedPointToolMode excuMode = CalculatedPointToolMode.Midpoint;
                        string optStr = calPoint.Mode.ToString();
                        if (EnumEditor(langMap, optStr, out excuMode)) {
                            calPoint.Mode = excuMode;
                            optStr = calPoint.Mode.ToString();
                            this.IsModified = true;
                        }
                        return optStr;
                    }
                )
            );

            propList.Add(
                new PropertyView(
                    langMap["PropPoint1"],
                    AccessLevel.None,
                    langMap["TipPoint1"],
                    "Referenced Tool",
                    calPoint.Point1.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfPoint);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calPoint.Point1.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcPointEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["Point1"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calPoint.Point1.Reference = mRef["Point1"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calPoint.Point1.Reference?.ToString() ?? string.Empty;
                    },
                    () => {
                        bool vis = false;
                        switch (calPoint.Mode) {
                            case CalculatedPointToolMode.Midpoint:
                            case CalculatedPointToolMode.PointOnLineClosestToPoint:
                            case CalculatedPointToolMode.PointOnArcClosestToPoint:
                                vis = true;
                                break;
                        }
                        return vis;
                    }
                )
            );

            propList.Add(
                new PropertyView(
                    langMap["PropPoint2"],
                    AccessLevel.None,
                    langMap["TipPoint2"],
                    "Referenced Tool",
                    calPoint.Point2.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfPoint);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calPoint.Point2.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcPointEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["Point2"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calPoint.Point2.Reference = mRef["Point2"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calPoint.Point2.Reference?.ToString() ?? string.Empty;
                    },
                    () => {
                        bool vis = false;
                        switch (calPoint.Mode) {
                            case CalculatedPointToolMode.Midpoint:
                                vis = true;
                                break;
                        }
                        return vis;
                    }
                )
            );

            propList.Add(
                new PropertyView(
                    langMap["PropLine1"],
                    AccessLevel.None,
                    langMap["TipLine1"],
                    "Referenced Tool",
                    calPoint.Line1.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfLine);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calPoint.Line1.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcPointEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["Line1"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calPoint.Line1.Reference = mRef["Line1"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calPoint.Line1.Reference?.ToString() ?? string.Empty;
                    },
                    () => {
                        bool vis = false;
                        switch (calPoint.Mode) {
                            case CalculatedPointToolMode.LineArcIntersection:
                            case CalculatedPointToolMode.LineLineIntersection:
                            case CalculatedPointToolMode.PointOnLineClosestToPoint:
                                vis = true;
                                break;
                        }
                        return vis;
                    }
                )
            );

            propList.Add(
                new PropertyView(
                    langMap["PropLine2"],
                    AccessLevel.None,
                    langMap["TipLine2"],
                    "Referenced Tool",
                    calPoint.Line2.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfLine);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calPoint.Line2.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcPointEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["Line2"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calPoint.Line2.Reference = mRef["Line2"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calPoint.Line2.Reference?.ToString() ?? string.Empty;
                    },
                    () => {
                        bool vis = false;
                        switch (calPoint.Mode) {
                            case CalculatedPointToolMode.LineLineIntersection:
                                vis = true;
                                break;
                        }
                        return vis;
                    }
                )
            );

            propList.Add(
                new PropertyView(
                    langMap["PropArc1"],
                    AccessLevel.None,
                    langMap["TipArc1"],
                    "Referenced Tool",
                    calPoint.Arc1.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfArc);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calPoint.Arc1.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcArcEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["Arc1"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calPoint.Arc1.Reference = mRef["Arc1"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calPoint.Arc1.Reference?.ToString() ?? string.Empty;
                    },
                    () => {
                        bool vis = false;
                        switch (calPoint.Mode) {
                            case CalculatedPointToolMode.ArcArcIntersection:
                            case CalculatedPointToolMode.LineArcIntersection:
                            case CalculatedPointToolMode.PointOnArcClosestToPoint:
                                vis = true;
                                break;
                        }
                        return vis;
                    }
                )
            );

            propList.Add(
                new PropertyView(
                    langMap["PropArc2"],
                    AccessLevel.None,
                    langMap["TipArc2"],
                    "Referenced Tool",
                    calPoint.Arc2.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfArc);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calPoint.Arc2.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcArcEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["Arc2"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calPoint.Arc2.Reference = mRef["Arc2"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calPoint.Arc2.Reference?.ToString() ?? string.Empty;
                    },
                    () => {
                        bool vis = false;
                        switch (calPoint.Mode) {
                            case CalculatedPointToolMode.ArcArcIntersection:
                                vis = true;
                                break;
                        }
                        return vis;
                    }
                )
            );

            propList.Add(
             new PropertyView(
                    langMap["PropPositionOfs"],
                    AccessLevel.None,
                    langMap["TipPositionOfs"],
                    "Offset",
                    mOfs.ToString(),
                    () => {
                        List<double> ofs;
                        string ofsStr = mOfs.ToString();
                        if (ValueEditor(langMap, ofsStr, 2, double.MinValue, double.MaxValue, out ofs)) {
                            VisionPoint ofsVP = new VisionPoint(ofs[0], ofs[1]);
                            calPoint.Offset = ofsVP;
                            ofsStr = ofsVP.ToString();
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return ofsStr;
                    }
                )
          );

            propList.Add(CreateImageSourceView(langMap));
            propList.Add(CreateShowResultView(langMap));
            propList.Add(CreateCommentView(langMap));

            return propList;
        }
        #endregion 

        #region IXmlSavable Implements
        /// <summary>產生物件的 XML 相關資料描述</summary>
        /// <param name="nodeName">此物件之 XML 節點名稱</param>
        /// <returns>XML 節點</returns>
        public override XmlElmt CreateXmlData(string nodeName) {
            var calPoint = mTool as ICalculatedPointTool;
            XmlElmt elmt = GenerateXmlData(nodeName);
            elmt.Add(
                new XmlElmt("Mode", calPoint.Mode.ToString()),
                new XmlElmt("Ofs", calPoint.Offset.ToString())
            );

            mRef.ForEach(kvp => {
                if (kvp.Value != null) {
                    elmt.Add(new XmlElmt(kvp.Key, kvp.Value.VariableName));
                }
            });
            return elmt;
        }
        #endregion
        
        #region CalculatedToolBase Implements

        ///<summary>取得參考工具定義</summary>
        protected override Dictionary<string, IVisionToolPack> GetRefTool() {
            return new Dictionary<string, IVisionToolPack>() {
                { "Point1",null},
                { "Point2",null},
                { "Line1",null},
                { "Line2", null},
                { "Arc1",null},
                { "Arc2",null}
            };
        }

        #endregion CalculatedToolBase Implements
    }

    ///<summary>求线段 工具包</summary>
    public class CalculatedLine : CalculatedToolBase, IResultOfLine {

        #region Constructors
        /// <summary>建立影像工具包於指定的 Ace 資料夾，附帶建立 <see cref="TreeNode"/></summary>
        /// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
        /// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
        /// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
        public CalculatedLine(
            List<IVisionToolPack> toolColl,
            IAceObjectCollection toolFold,
            TreeNode mainNode
        ) : base(toolColl, VisionToolType.CalculatedLine, toolFold, mainNode) {
            
        }

        /// <summary>從現有的影像工具進行複製</summary>
        /// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
        /// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
        /// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
        /// <param name="copyPack">欲複製的來源</param>
        public CalculatedLine(
            List<IVisionToolPack> toolColl,
            IVisionToolPack copyPack,
            IAceObjectCollection toolFold,
            TreeNode mainNode
        ) : base(toolColl, copyPack, toolFold, mainNode) {

        }

        /// <summary>建構單純的影像工具包，不含 <seealso cref="TreeNode"/> 等父節點</summary>
        /// <param name="source">指定的影像來源</param>
        /// <param name="fold">欲存放此工具的 Ace 資料夾</param>
        public CalculatedLine(
            IVisionImageSource source,
            IAceObjectCollection fold
        ) : base(VisionToolType.CalculatedLine, source, fold) {

        }

        /// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
        /// <param name="langMap">多國語系之對應清單</param>
        /// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
        /// <param name="aceSrv">已連線的 ACE Server 端物件</param>
        /// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
        /// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
        public CalculatedLine(
            Dictionary<string, string> langMap,
            XmlElmt xmlData,
            IAceServer aceSrv
        ) : base(xmlData, aceSrv, langMap) {
            XmlElmt childData = null;
            var callPoint = mTool as ICalculatedLineTool;
            if (xmlData.Element("Mode", out childData)) {
                callPoint.Mode = (CalculatedLineToolMode)Enum.Parse(typeof(CalculatedLineToolMode), childData.Value);
            }

            IsLoadRef = false;
            if (xmlData.Element("Ofs", out childData)) {
                double[] val = childData.Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .ConvertAll(v => Convert.ToDouble(v));
                callPoint.Offset = new VisionPoint(val[0], val[1]);
            }
            mLoadRef = tools => {
                SetRef(xmlData, "Point1", tools, t => callPoint.Point1.Reference = t);
                SetRef(xmlData, "Point2", tools, t => callPoint.Point2.Reference = t);
                SetRef(xmlData, "Line1", tools, t => callPoint.Line1.Reference = t);
                IsLoadRef = true;
                mLoadRef = null;
            };

        }

        #endregion
        
        #region IResultable Implements
        /// <summary>取得適用於顯示結果的 <see cref="DataTable.TableName"/></summary>
        public string ResultTableName { get { return "CalculatedLine"; } }

        /// <summary>建立當前影像工具的執行結果清單。如欲隱藏特定欄位，請透過 <see cref="DataGridView.Columns"/> 進行操作</summary>
        /// <returns>對應的 執行結果清單</returns>
        public DataTable CreateDataTable() {
            DataTable dt = new DataTable(ResultTableName);
            DataColumn[] columns = GetResultColumnNames().Select(kvp => new DataColumn(kvp.Key) { Caption = kvp.Value }).ToArray();
            dt.Columns.AddRange(columns);

            ICalculatedLineTool tool = mTool as ICalculatedLineTool;
            if (tool.ResultsAvailable) {
                int idx = 1;
                List<VisionLineResult> results = tool.Results.ToList();
                results.ForEach(
                    result => {
                        dt.Rows.Add(
                            new string[] {
                                (idx++).ToString(),
                                result.StartPoint.X.ToString("F3"),
                                result.StartPoint.Y.ToString("F3"),
                                result.EndPoint.X.ToString("F3"),
                                result.EndPoint.Y.ToString("F3"),
                                result.Line.Degrees.ToString("F3")
                            }
                        );
                    }
                );
            }

            return dt;
        }

        /// <summary>取得結果清單的欄位名稱(<see cref="DataColumn.ColumnName"/>)與標題(<see cref="DataColumn.Caption"/>)對應表</summary>
        /// <returns>清單欄位與標題對應表</returns>
        public Dictionary<string, string> GetResultColumnNames() {
            return new Dictionary<string, string> {
                { "inst" , "Instance" },
                { "startX", "Start X" },
                { "startY", "Start Y" },
                { "endX", "End X" },
                { "endY", "End Y" },
                { "angle", "Angle" }
            };
        }

        /// <summary>建立結果清單的預設欄位名稱與預設啟用狀態</summary>
        /// <returns>預設的欄位與致能</returns>
        public ResultableTable GetDefaultResultColumns() {
            List<ResultableColumn> columns = new List<ResultableColumn> {
                new ResultableColumn("inst", "Instance", true),
                new ResultableColumn("startX", "Start X", true),
                new ResultableColumn("startY", "Start Y", true),
                new ResultableColumn("endX", "End X", true),
                new ResultableColumn("endY", "End Y", true),
                new ResultableColumn("angle", "Angle", true)
            };

            return new ResultableTable(ResultTableName, columns);
        }
        #endregion

        #region IResultOfAngle Implements

        /// <summary>取得或設定此影像工具是否作為最後回傳的角度定義</summary>
        public bool IsAngleReturner { get; set; }


        ///<summary>產生角度CVT語法</summary>
        ///<param name="code">程式碼主體</param>
        public void GenerateAngleCVT(List<string> code) {
            base.generateAngleCVT(code);
            code.Add($"pos.Degrees = {VariableName}.Results[0].Line.Degrees");
        }

        #endregion IResultOfAngle Implements

        #region IPropertable Implements

        /// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
        /// <param name="langMap">各國語系之對應清單</param>
        /// <returns>對應的屬性檢視</returns>
        public override List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
            Stat stt = Stat.SUCCESS;
            var propList = new List<PropertyView>();

            var calLine = mTool as ICalculatedLineTool;

            mOfs = new VisionTransform(calLine.Offset);
            if (mOfsShd != mOfs) {
                this.IsCompiled = false;
                mOfsShd = new VisionTransform(mOfs);
            }

            propList.Add(
                new PropertyView(
                    langMap["PropCalLineMod"],
                    AccessLevel.None,
                    langMap["TipCalLineMod"],
                    "CalculatedLineTool Mode",
                    langMap[calLine.Mode.GetFullName()],
                    () => {
                        CalculatedLineToolMode excuMode = CalculatedLineToolMode.TwoPoint;
                        string optStr = calLine.Mode.ToString();
                        if (EnumEditor(langMap, optStr, out excuMode)) {
                            calLine.Mode = excuMode;
                            optStr = calLine.Mode.ToString();
                            this.IsModified = true;
                        }
                        return optStr;
                    }
                )
            );

            propList.Add(
                new PropertyView(
                    langMap["PropPoint1"],
                    AccessLevel.None,
                    langMap["TipPoint1"],
                    "Referenced Tool",
                    calLine.Point1.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfPoint);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calLine.Point1.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcLineEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["Point1"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calLine.Point1.Reference = mRef["Point1"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calLine.Point1.Reference?.ToString() ?? string.Empty;
                    },
                    () => {return true;}
                )
            );

            propList.Add(
                new PropertyView(
                    langMap["PropPoint2"],
                    AccessLevel.None,
                    langMap["TipPoint2"],
                    "Referenced Tool",
                    calLine.Point2.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfPoint);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calLine.Point2.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcLineEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["Point2"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calLine.Point2.Reference = mRef["Point2"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calLine.Point2.Reference?.ToString() ?? string.Empty;
                    },
                    () => {return calLine.Mode == CalculatedLineToolMode.TwoPoint;}
                )
            );

            propList.Add(
                new PropertyView(
                    langMap["PropLine1"],
                    AccessLevel.None,
                    langMap["TipLine1"],
                    "Referenced Tool",
                    calLine.Line1.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfLine);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calLine.Line1.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcLineEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["Line1"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calLine.Line1.Reference = mRef["Line1"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calLine.Line1.Reference?.ToString() ?? string.Empty;
                    },
                    () => {return calLine.Mode == CalculatedLineToolMode.PerpendicularLine;}
                )
            );
            
            propList.Add(
             new PropertyView(
                    langMap["PropPositionOfs"],
                    AccessLevel.None,
                    langMap["TipPositionOfs"],
                    "Offset",
                    mOfs.ToString(),
                    () => {
                        List<double> ofs;
                        string ofsStr = mOfs.ToString();
                        if (ValueEditor(langMap, ofsStr, 2, double.MinValue, double.MaxValue, out ofs)) {
                            VisionPoint ofsVP = new VisionPoint(ofs[0], ofs[1]);
                            calLine.Offset = ofsVP;
                            ofsStr = ofsVP.ToString();
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return ofsStr;
                    }
                )
          );

            propList.Add(CreateImageSourceView(langMap));
            propList.Add(CreateShowResultView(langMap));
            propList.Add(CreateCommentView(langMap));

            return propList;
        }
        #endregion 

        #region IXmlSavable Implements
        /// <summary>產生物件的 XML 相關資料描述</summary>
        /// <param name="nodeName">此物件之 XML 節點名稱</param>
        /// <returns>XML 節點</returns>
        public override XmlElmt CreateXmlData(string nodeName) {
            var calLine = mTool as ICalculatedLineTool;
            XmlElmt elmt = GenerateXmlData(nodeName);
            elmt.Add(
                new XmlElmt("Mode", calLine.Mode.ToString()),
                new XmlElmt("Ofs", calLine.Offset.ToString())
            );

            mRef.ForEach(kvp => {
                if (kvp.Value != null) {
                    elmt.Add(new XmlElmt(kvp.Key, kvp.Value.VariableName));
                }
            });
            return elmt;
        }
        #endregion
        
        #region CalculatedToolBase Implements

        ///<summary>取得參考工具定義</summary>
        protected override Dictionary<string, IVisionToolPack> GetRefTool() {
            return new Dictionary<string, IVisionToolPack>() {
                { "Point1",null},
                { "Point2",null},
                { "Line1",null}
            };
        }

        #endregion CalculatedToolBase Implements
    }

    ///<summary>求弧線 工具包</summary>
    public class CalculatedArc :CalculatedToolBase,IResultOfArc   {

        #region Constructors

        /// <summary>建立影像工具包於指定的 Ace 資料夾，附帶建立 <see cref="TreeNode"/></summary>
        /// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
        /// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
        /// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
        public CalculatedArc(
            List<IVisionToolPack> toolColl,
            IAceObjectCollection toolFold,
            TreeNode mainNode
        ) : base(toolColl, VisionToolType.CalculatedArc, toolFold, mainNode) {

        }

        /// <summary>從現有的影像工具進行複製</summary>
        /// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
        /// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
        /// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
        /// <param name="copyPack">欲複製的來源</param>
        public CalculatedArc(
            List<IVisionToolPack> toolColl,
            IVisionToolPack copyPack,
            IAceObjectCollection toolFold,
            TreeNode mainNode
        ) : base(toolColl, copyPack, toolFold, mainNode) {

        }

        /// <summary>建構單純的影像工具包，不含 <seealso cref="TreeNode"/> 等父節點</summary>
        /// <param name="source">指定的影像來源</param>
        /// <param name="fold">欲存放此工具的 Ace 資料夾</param>
        public CalculatedArc(
            IVisionImageSource source,
            IAceObjectCollection fold
        ) : base(VisionToolType.CalculatedArc, source, fold) {

        }

        /// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
        /// <param name="langMap">多國語系之對應清單</param>
        /// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
        /// <param name="aceSrv">已連線的 ACE Server 端物件</param>
        /// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
        /// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
        public CalculatedArc(
            Dictionary<string, string> langMap,
            XmlElmt xmlData,
            IAceServer aceSrv
        ) : base(xmlData, aceSrv, langMap) {
            XmlElmt childData = null;
            var callPoint = mTool as ICalculatedArcTool;
            if (xmlData.Element("Mode", out childData)) {
                callPoint.Mode = (CalculatedArcToolMode)Enum.Parse(typeof(CalculatedArcToolMode), childData.Value);
            }

            IsLoadRef = false;
            if (xmlData.Element("Ofs", out childData)) {
                double[] val = childData.Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .ConvertAll(v => Convert.ToDouble(v));
                callPoint.Offset = new VisionPoint(val[0], val[1]);
            }
            mLoadRef = tools => {
                SetRef(xmlData, "CenterArcPoint", tools, t => callPoint.CenterPoint.Reference = t);
                SetRef(xmlData, "Point1", tools, t => callPoint.ArcPoint1.Reference = t);
                SetRef(xmlData, "Point2", tools, t => callPoint.ArcPoint2.Reference = t);
                SetRef(xmlData, "Point3", tools, t => callPoint.ArcPoint3.Reference = t);
                IsLoadRef = true;
                mLoadRef = null;
            };

        }

        #endregion

        #region IResultOfAngle Implements
        /// <summary>取得或設定此影像工具是否作為最後回傳的角度定義</summary>
        public bool IsAngleReturner { get; set; }

        ///<summary>產生角度CVT語法</summary>
        ///<param name="code">程式碼主體</param>
        public void GenerateAngleCVT(List<string> code) {
            base.generateAngleCVT(code);
            code.Add($"pos.Degrees = {VariableName}.Results[0].Arc.Rotation");
        }

        #endregion
        
        #region IResultable Implements
        /// <summary>取得適用於顯示結果的 <see cref="DataTable.TableName"/></summary>
        public string ResultTableName { get { return "CalculatedArc"; } }

        /// <summary>建立當前影像工具的執行結果清單。如欲隱藏特定欄位，請透過 <see cref="DataGridView.Columns"/> 進行操作</summary>
        /// <returns>對應的 執行結果清單</returns>
        public DataTable CreateDataTable() {
            DataTable dt = new DataTable(ResultTableName);
            DataColumn[] columns = GetResultColumnNames().Select(kvp => new DataColumn(kvp.Key) { Caption = kvp.Value }).ToArray();
            dt.Columns.AddRange(columns);

            ICalculatedArcTool tool = mTool as ICalculatedArcTool;
            if (tool.Results.Any()) {
                int idx = 1;
                List<VisionArcResult> results = tool.Results.ToList();
                results.ForEach(
                    result => {
                        dt.Rows.Add(
                            new string[] {
                                (idx++).ToString(),
                                result.CenterPoint.X.ToString("F3"),
                                result.CenterPoint.Y.ToString("F3"),
                                result.Arc.Radius.ToString("F3")
                            }
                        );
                    }
                );
            }

            return dt;
        }

        /// <summary>取得結果清單的欄位名稱(<see cref="DataColumn.ColumnName"/>)與標題(<see cref="DataColumn.Caption"/>)對應表</summary>
        /// <returns>清單欄位與標題對應表</returns>
        public Dictionary<string, string> GetResultColumnNames() {
            return new Dictionary<string, string> {
                { "inst" , "Instance" },
                { "arcX", "Arc X" },
                { "arcY", "Arc Y" },
                { "radius","Radius"}
            };
        }

        /// <summary>建立結果清單的預設欄位名稱與預設啟用狀態</summary>
        /// <returns>預設的欄位與致能</returns>
        public ResultableTable GetDefaultResultColumns() {
            List<ResultableColumn> columns = new List<ResultableColumn> {
                new ResultableColumn("inst", "Instance", true),
                new ResultableColumn("arcX", "Arc X", true),
                new ResultableColumn("arcY", "Arc Y", true),
                new ResultableColumn("radius","Radius", true)
            };

            return new ResultableTable(ResultTableName, columns);
        }
        #endregion

        #region IPropertable Implements

        /// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
        /// <param name="langMap">各國語系之對應清單</param>
        /// <returns>對應的屬性檢視</returns>
        public override List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
            Stat stt = Stat.SUCCESS;
            var propList = new List<PropertyView>();

            var calPoint = mTool as ICalculatedArcTool;

            mOfs = new VisionTransform(calPoint.Offset);
            if (mOfsShd != mOfs) {
                this.IsCompiled = false;
                mOfsShd = new VisionTransform(mOfs);
            }

            propList.Add(
                new PropertyView(
                    langMap["PropCalArcMod"],
                    AccessLevel.None,
                    langMap["TipCalArcMod"],
                    "CalculatedArcTool Mode",
                    langMap[calPoint.Mode.GetFullName()],
                    () => {
                        CalculatedArcToolMode excuMode = CalculatedArcToolMode.CenterPointAndPointOnArc;
                        string optStr = calPoint.Mode.ToString();
                        if (EnumEditor(langMap, optStr, out excuMode)) {
                            calPoint.Mode = excuMode;
                            optStr = calPoint.Mode.ToString();
                            this.IsModified = true;
                        }
                        return optStr;
                    }
                )
            );

            propList.Add(
                new PropertyView(
                    langMap["PropCenterArcPoint"],
                    AccessLevel.None,
                    langMap["TipCenterArcPoint"],
                    "Referenced Tool",
                    calPoint.CenterPoint.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfPoint);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calPoint.CenterPoint.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcPointEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["CenterArcPoint"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calPoint.CenterPoint.Reference = mRef["CenterArcPoint"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calPoint.CenterPoint.Reference?.ToString() ?? string.Empty;
                    },
                    () => calPoint.Mode == CalculatedArcToolMode.CenterPointAndPointOnArc
                )
            );


            propList.Add(
                new PropertyView(
                    langMap["PropPoint1"],
                    AccessLevel.None,
                    langMap["TipPoint1"],
                    "Referenced Tool",
                    calPoint.ArcPoint1.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfPoint);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calPoint.ArcPoint1.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcPointEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["Point1"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calPoint.ArcPoint1.Reference = mRef["Point1"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calPoint.ArcPoint1.Reference?.ToString() ?? string.Empty;
                    },
                    () => true
                )
            );


            propList.Add(
                new PropertyView(
                    langMap["PropPoint2"],
                    AccessLevel.None,
                    langMap["TipPoint2"],
                    "Referenced Tool",
                    calPoint.ArcPoint2.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfPoint);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calPoint.ArcPoint2.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcPointEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["Point2"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calPoint.ArcPoint2.Reference = mRef["Point2"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calPoint.ArcPoint2.Reference?.ToString() ?? string.Empty;
                    },
                    () => calPoint.Mode == CalculatedArcToolMode.ThreePointsOnArc
                )
            );


            propList.Add(
                new PropertyView(
                    langMap["PropPoint3"],
                    AccessLevel.None,
                    langMap["TipPoint3"],
                    "Referenced Tool",
                    calPoint.ArcPoint3.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfPoint);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calPoint.ArcPoint3.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcPointEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["Point3"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calPoint.ArcPoint3.Reference = mRef["Point3"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calPoint.ArcPoint3.Reference?.ToString() ?? string.Empty;
                    },
                    () => calPoint.Mode == CalculatedArcToolMode.ThreePointsOnArc
                )
            );


            propList.Add(
             new PropertyView(
                    langMap["PropPositionOfs"],
                    AccessLevel.None,
                    langMap["TipPositionOfs"],
                    "Offset",
                    mOfs.ToString(),
                    () => {
                        List<double> ofs;
                        string ofsStr = mOfs.ToString();
                        if (ValueEditor(langMap, ofsStr, 2, double.MinValue, double.MaxValue, out ofs)) {
                            VisionPoint ofsVP = new VisionPoint(ofs[0], ofs[1]);
                            calPoint.Offset = ofsVP;
                            ofsStr = ofsVP.ToString();
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return ofsStr;
                    }
                )
          );

            propList.Add(CreateImageSourceView(langMap));
            propList.Add(CreateShowResultView(langMap));
            propList.Add(CreateCommentView(langMap));

            return propList;
        }
        #endregion 

        #region IXmlSavable Implements
        /// <summary>產生物件的 XML 相關資料描述</summary>
        /// <param name="nodeName">此物件之 XML 節點名稱</param>
        /// <returns>XML 節點</returns>
        public override XmlElmt CreateXmlData(string nodeName) {
            var calPoint = mTool as ICalculatedArcTool;
            XmlElmt elmt = GenerateXmlData(nodeName);
            elmt.Add(
                new XmlElmt("Mode", calPoint.Mode.ToString()),
                new XmlElmt("Ofs", calPoint.Offset.ToString())
            );

            mRef.ForEach(kvp => {
                if (kvp.Value != null) {
                    elmt.Add(new XmlElmt(kvp.Key, kvp.Value.VariableName));
                }
            });
            return elmt;
        }
        #endregion

        #region CalculatedToolBase Implements

        ///<summary>取得參考工具定義</summary>
        protected override Dictionary<string, IVisionToolPack> GetRefTool() {
            return new Dictionary<string, IVisionToolPack>() {
                { "CenterArcPoint",null},
                { "Point1",null},
                { "Point2",null},
                { "Point3", null}
            };
        }

        #endregion CalculatedToolBase Implements
    }

    ///<summary>求座標點 工具包</summary>
    public class CalculatedFrame:CalculatedToolBase,IResultOfTransform {
        #region Constructors

        /// <summary>建立影像工具包於指定的 Ace 資料夾，附帶建立 <see cref="TreeNode"/></summary>
        /// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
        /// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
        /// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
        public CalculatedFrame(
            List<IVisionToolPack> toolColl,
            IAceObjectCollection toolFold,
            TreeNode mainNode
        ) : base(toolColl, VisionToolType.CalculatedFrame, toolFold, mainNode) {

        }

        /// <summary>從現有的影像工具進行複製</summary>
        /// <param name="toolColl">當前已有的影像包集合，用於建立名稱與物件時，判斷當前有多少相同工具</param>
        /// <param name="toolFold">存放此影像工具的 Ace 資料夾</param>
        /// <param name="mainNode">欲建立 <see cref="TreeNode"/> 的 <see cref="TreeView"/> 的主節點</param>
        /// <param name="copyPack">欲複製的來源</param>
        public CalculatedFrame(
            List<IVisionToolPack> toolColl,
            IVisionToolPack copyPack,
            IAceObjectCollection toolFold,
            TreeNode mainNode
        ) : base(toolColl, copyPack, toolFold, mainNode) {

        }

        /// <summary>建構單純的影像工具包，不含 <seealso cref="TreeNode"/> 等父節點</summary>
        /// <param name="source">指定的影像來源</param>
        /// <param name="fold">欲存放此工具的 Ace 資料夾</param>
        public CalculatedFrame(
            IVisionImageSource source,
            IAceObjectCollection fold
        ) : base(VisionToolType.CalculatedFrame, source, fold) {

        }

        /// <summary>使用 <see cref="IXmlData"/> 建構此影像工具包</summary>
        /// <param name="langMap">多國語系之對應清單</param>
        /// <param name="xmlData">含有影像工具資訊的 <see cref="IXmlData"/></param>
        /// <param name="aceSrv">已連線的 ACE Server 端物件</param>
        /// <exception cref="InvalidCastException">無法解析的 <see cref="IXmlData"/></exception>
        /// <exception cref="ArgumentNullException">無法解析的字串或無相對應的 <see cref="IVisionTool"/></exception>
        public CalculatedFrame(
            Dictionary<string, string> langMap,
            XmlElmt xmlData,
            IAceServer aceSrv
        ) : base(xmlData, aceSrv, langMap) {
            XmlElmt childData = null;
            var callPoint = mTool as ICalculatedFrameTool;
            if (xmlData.Element("Mode", out childData)) {
                callPoint.Mode = (CalculatedFrameToolMode)Enum.Parse(typeof(CalculatedFrameToolMode), childData.Value);
            }

            IsLoadRef = false;
            if (xmlData.Element("Ofs", out childData)) {
                double[] val = childData.Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .ConvertAll(v => Convert.ToDouble(v));
                callPoint.Offset = new VisionTransform(val[0], val[1],val[2]);
            }
            mLoadRef = tools => {
                SetRef(xmlData, "XLine", tools, t => callPoint.XLine.Reference = t);
                SetRef(xmlData, "YLine", tools, t => callPoint.YLine.Reference = t);
                SetRef(xmlData, "OriginPoint", tools, t => callPoint.OriginPoint.Reference = t);
                SetRef(xmlData, "PostiveXPoint", tools, t => callPoint.PositiveXPoint.Reference = t);
                SetRef(xmlData, "OriginTransform", tools, t => callPoint.OriginTransform.Reference = t);
                SetRef(xmlData, "PostiveXPoint", tools, t => callPoint.PositiveXPoint.Reference = t);
                IsLoadRef = true;
                mLoadRef = null;
            };

        }

        #endregion

        #region IResultable Implements
        /// <summary>取得適用於顯示結果的 <see cref="DataTable.TableName"/></summary>
        public string ResultTableName { get { return "CalculatedFrame"; } }

        /// <summary>建立當前影像工具的執行結果清單。如欲隱藏特定欄位，請透過 <see cref="DataGridView.Columns"/> 進行操作</summary>
        /// <returns>對應的 執行結果清單</returns>
        public DataTable CreateDataTable() {
            DataTable dt = new DataTable(ResultTableName);
            DataColumn[] columns = GetResultColumnNames().Select(kvp => new DataColumn(kvp.Key) { Caption = kvp.Value }).ToArray();
            dt.Columns.AddRange(columns);

            ICalculatedFrameTool tool = mTool as ICalculatedFrameTool;
            if (tool.ResultsAvailable) {
                int idx = 1;
                List<VisionTransformResult> results = tool.Results.ToList();
                results.ForEach(
                    result => {
                        dt.Rows.Add(
                            new string[] {
                                (idx++).ToString(),
                                result.Transform.X.ToString("F3"),
                                result.Transform.Y.ToString("F3"),
                                result.Transform.Degrees.ToString("F3")
                            }
                        );
                    }
                );
            }

            return dt;
        }

        /// <summary>取得結果清單的欄位名稱(<see cref="DataColumn.ColumnName"/>)與標題(<see cref="DataColumn.Caption"/>)對應表</summary>
        /// <returns>清單欄位與標題對應表</returns>
        public Dictionary<string, string> GetResultColumnNames() {
            return new Dictionary<string, string> {
                { "inst" , "Instance" },
                { "posX", "Position X" },
                { "posY", "Position Y" },
                { "angle","Angle"}
            };
        }

        /// <summary>建立結果清單的預設欄位名稱與預設啟用狀態</summary>
        /// <returns>預設的欄位與致能</returns>
        public ResultableTable GetDefaultResultColumns() {
            List<ResultableColumn> columns = new List<ResultableColumn> {
                new ResultableColumn("inst", "Instance", true),
                new ResultableColumn("posX", "Position X", true),
                new ResultableColumn("posY", "Position Y", true),
                new ResultableColumn("angle", "Angle", true)
            };

            return new ResultableTable(ResultTableName, columns);
        }
        #endregion

        #region IResultOfAngle Implements
        /// <summary>取得或設定此影像工具是否作為最後回傳的角度定義</summary>
        public bool IsAngleReturner { get; set; }

        ///<summary>產生角度CVT語法</summary>
        ///<param name="code">程式碼主體</param>
        public void GenerateAngleCVT(List<string> code) {
            base.generateAngleCVT(code);
        }

        #endregion

        #region IPropertable Implements

        /// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
        /// <param name="langMap">各國語系之對應清單</param>
        /// <returns>對應的屬性檢視</returns>
        public override List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
            Stat stt = Stat.SUCCESS;
            var propList = new List<PropertyView>();

            var calPoint = mTool as ICalculatedFrameTool;

            mOfs = new VisionTransform(calPoint.Offset);
            if (mOfsShd != mOfs) {
                this.IsCompiled = false;
                mOfsShd = new VisionTransform(mOfs);
            }

            propList.Add(
                new PropertyView(
                    langMap["PropCalFrameMod"],
                    AccessLevel.None,
                    langMap["TipCalFrameMod"],
                    "CalculatedFrameTool Mode",
                    langMap[calPoint.Mode.GetFullName()],
                    () => {
                        CalculatedFrameToolMode excuMode = CalculatedFrameToolMode.Fixed;
                        string optStr = calPoint.Mode.ToString();
                        if (EnumEditor(langMap, optStr, out excuMode)) {
                            calPoint.Mode = excuMode;
                            optStr = calPoint.Mode.ToString();
                            this.IsModified = true;
                        }
                        return optStr;
                    }
                )
            );

            propList.Add(
                new PropertyView(
                    langMap["PropXLine"],
                    AccessLevel.None,
                    langMap["TipXLine"],
                    "Referenced Tool",
                    calPoint.XLine.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfLine);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calPoint.XLine.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcLineEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["XLine"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calPoint.XLine.Reference = mRef["XLine"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calPoint.XLine.Reference?.ToString() ?? string.Empty;
                    },
                    () => {
                        bool ret = false;
                        switch (calPoint.Mode) {
                            case CalculatedFrameToolMode.OnePointAndOneLine:
                            case CalculatedFrameToolMode.TwoLines:
                                ret = true;
                                break;
                        }
                        return ret;
                    }
                )
            );


            propList.Add(
                new PropertyView(
                    langMap["PropYLine"],
                    AccessLevel.None,
                    langMap["TipYLine"],
                    "Referenced Tool",
                    calPoint.YLine.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfLine);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calPoint.YLine.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcLineEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["YLine"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calPoint.YLine.Reference = mRef["YLine"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calPoint.YLine.Reference?.ToString() ?? string.Empty;
                    },
                    () => calPoint.Mode == CalculatedFrameToolMode.TwoLines
                )
            );


            propList.Add(
                new PropertyView(
                    langMap["PropOriPoint"],
                    AccessLevel.None,
                    langMap["TipOriPoint"],
                    "Referenced Tool",
                    calPoint.OriginPoint.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfPoint);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calPoint.OriginPoint.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcPointEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["OriPoint"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calPoint.OriginPoint.Reference = mRef["OriPoint"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calPoint.OriginPoint.Reference?.ToString() ?? string.Empty;
                    },
                    () => {
                        bool ret = false;
                        switch (calPoint.Mode) {
                            case CalculatedFrameToolMode.OnePointAndOneLine:
                            case CalculatedFrameToolMode.TwoLines:
                            case CalculatedFrameToolMode.TwoPoints:
                            case CalculatedFrameToolMode.OnePoint:
                                ret = true;
                                break;
                        }
                        return ret;
                    }
                )
            );


            propList.Add(
                new PropertyView(
                    langMap["PropPositiveXPoint"],
                    AccessLevel.None,
                    langMap["TipPositiveXPoint"],
                    "Referenced Tool",
                    calPoint.PositiveXPoint.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfPoint);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calPoint.PositiveXPoint.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcPointEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["PositiveXPoint"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calPoint.PositiveXPoint.Reference = mRef["PositiveXPoint"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calPoint.PositiveXPoint.Reference?.ToString() ?? string.Empty;
                    },
                    () => calPoint.Mode == CalculatedFrameToolMode.TwoPoints
                )
            );

            propList.Add(
                new PropertyView(
                    langMap["PropOriTrans"],
                    AccessLevel.None,
                    langMap["TipOriTrans"],
                    "Referenced Tool",
                    calPoint.OriginTransform.Reference?.ToString() ?? string.Empty,
                    () => {
                        List<IVisionToolPack> toolList = GetToolList(tool => tool is IResultOfTransform);
                        List<string> chkColl;
                        string curTool = toolList.Find(tool => (tool.Tool as IVisionTool) == calPoint.OriginTransform.Reference)?.Node?.Text ?? string.Empty;
                        stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcFrameEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
                        if (stt == Stat.SUCCESS) {
                            mRef["OriTrans"] = toolList.Find(tool => tool.Node.Text == chkColl[0]);
                            calPoint.OriginTransform.Reference = mRef["OriTrans"].Tool as IVisionTool;
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return calPoint.OriginTransform.Reference?.ToString() ?? string.Empty;
                    },
                    () =>  calPoint.Mode == CalculatedFrameToolMode.OneFrame
                )
            );


            propList.Add(
             new PropertyView(
                    langMap["PropTransformOfs"],
                    AccessLevel.None,
                    langMap["TipTransformOfs"],
                    "Offset",
                    mOfs.ToString(),
                    () => {
                        List<double> ofs;
                        string ofsStr = mOfs.ToString();
                        if (ValueEditor(langMap, ofsStr, 3, double.MinValue, double.MaxValue, out ofs)) {
                            VisionTransform ofsVP = new VisionTransform(ofs[0], ofs[1],ofs[2]);
                            calPoint.Offset = ofsVP;
                            ofsStr = ofsVP.ToString();
                            this.IsModified = true;
                            this.IsCompiled = false;
                        }
                        return ofsStr;
                    }
                )
          );

            propList.Add(CreateImageSourceView(langMap));
            propList.Add(CreateShowResultView(langMap));
            propList.Add(CreateCommentView(langMap));

            return propList;
        }
        #endregion 

        #region IXmlSavable Implements
        /// <summary>產生物件的 XML 相關資料描述</summary>
        /// <param name="nodeName">此物件之 XML 節點名稱</param>
        /// <returns>XML 節點</returns>
        public override XmlElmt CreateXmlData(string nodeName) {
            var calPoint = mTool as ICalculatedFrameTool;
            XmlElmt elmt = GenerateXmlData(nodeName);
            elmt.Add(
                new XmlElmt("Mode", calPoint.Mode.ToString()),
                new XmlElmt("Ofs", calPoint.Offset.ToString())
            );

            mRef.ForEach(kvp => {
                if (kvp.Value != null) {
                    elmt.Add(new XmlElmt(kvp.Key, kvp.Value.VariableName));
                }
            });
            return elmt;
        }
        #endregion

        #region CalculatedToolBase Implements

        ///<summary>取得參考工具定義</summary>
        protected override Dictionary<string, IVisionToolPack> GetRefTool() {
            return new Dictionary<string, IVisionToolPack>() {
                { "XLine",null},
                { "YLine",null},
                { "OriPoint",null},
                { "OriTrans", null},
                { "PositiveXPoint",null }
            };
        }

        #endregion CalculatedToolBase Implements
    }

    #endregion

    #region Declaration - Results

    /// <summary>將多個影像結果進行平均計算並回傳</summary>
    /// <remarks>如將四個 Locator 的結果取中心點</remarks>
    public class ResultAverage : IVisionResult {

		#region Fields
		/// <summary>此結果工具的 <see cref="TreeView"/> 節點</summary>
		private TreeNode mTreeNode;
		/// <summary>暫存旋轉角之工具</summary>
		private IResultOfAngle mThetaTool;
		/// <summary>計算平均的影像工具集合</summary>
		private List<IVisionToolPack> mToolColl = new List<IVisionToolPack>();
		/// <summary>註解</summary>
		private string mCmt = string.Empty;
		/// <summary>是否要使用特定的影像工具來決定旋轉角</summary>
		private bool mCalcTheta = false;
		/// <summary>於 <see cref="TreeView"/> 的節點之層數(從左至右，0 開始)</summary>
		private int mTreeNodeLv = -1;
		/// <summary>於 <see cref="TreeView"/> 的節點於同層內的深度(從上至下，0 開始)</summary>
		private int mTreeNodeIdx = -1;
		/// <summary>識別碼</summary>
		private long mID = -1;
		/// <summary>Parent Node 的識別碼</summary>
		private long? mInputLinkID = null;
		#endregion

		#region Properties
		/// <summary>取得此工具的識別碼</summary>
		public long ID { get { return mID; } }
		/// <summary>取得或設定是否要使用特定的影像工具來決定旋轉角</summary>
		public bool CalculateTheta { get { return mCalcTheta; } set { mCalcTheta = value; } }
		/// <summary>取得此結果工具的 <see cref="TreeView"/> 節點</summary>
		public TreeNode Node { get { return mTreeNode; } }
		/// <summary>取得於 <see cref="TreeView"/> 的節點之層數(從左至右，0 開始)</summary>
		public int NodeLevel { get { return mTreeNodeLv; } }
		/// <summary>取得於 <see cref="TreeView"/> 的節點於同層內的深度(從上至下，0 開始)</summary>
		public int NodeIndex { get { return mTreeNodeIdx; } }
		/// <summary>取得此工具的註解</summary>
		public string Comment { get { return mCmt; } }
		/// <summary>取得或設定工具是否被修改</summary>
		public bool IsModified { get; set; } = false;
		/// <summary>取得或設定工具是否已被編譯</summary>
		/// <remarks>(<see langword="true"/>)不需要再重新 Compile CVT  (<see langword="false"/>)需要重新 Compile</remarks>
		public bool IsCompiled { get; set; } = true;
		/// <summary>取得或設定物件，其包含相關資料</summary>
		public object Tag { get; set; } = null;
		/// <summary>取得 Relatived 的識別碼</summary>
		public long? InputLinkID { get { return mInputLinkID; } }
		#endregion

		#region Constructors
		/// <summary>建構平均結果</summary>
		/// <param name="mainNode">欲存放此平均結果的 <see cref="TreeView"/> 父節點</param>
		public ResultAverage(TreeNode mainNode) {
			mID = DateTime.Now.ToBinary();
			mTreeNode = mainNode.Nodes.Add("Average Result");
			mTreeNode.Tag = this;
			mTreeNodeLv = mTreeNode.Level;
			mTreeNodeIdx = mTreeNode.Index;
			IsModified = true;
		}

		/// <summary>使用 <see cref="IXmlData"/> 建構計算平均之結果工具</summary>
		/// <param name="xmlData">含有 <see cref="IVisionResult"/> 訊息之 <see cref="IXmlData"/></param>
		/// <param name="aceSrv">已連線的 ACE Server 端物件</param>
		/// <param name="toolColl">含有 <see cref="IResultOfAngle"/> 與 <see cref="IVisionToolPack"/> 之集合，供尋找參考</param>
		public ResultAverage(XmlElmt xmlData, IAceServer aceSrv, IEnumerable<IVisionProjectable> toolColl) {
			/* 拆出 Type */
			string toolType = xmlData.Attribute("Type").Value;
			if (toolType != "ResultAverage") throw new InvalidCastException(GetMultiLangText("VisToolNull"));

			/* 取得 ID */
			mID = long.Parse(xmlData.Attribute("ID").Value);

			/* 樹節點 */
			XmlElmt childData;
			if (xmlData.Element("Node", out childData)) {
				if (!string.IsNullOrEmpty(childData.Value)) {
					mTreeNode = new TreeNode(childData.Value);
					mTreeNode.Tag = this;

					XmlAttr attr;
					if (childData.Attribute("ParentID", out attr))
						mInputLinkID = string.IsNullOrEmpty(attr.Value) ? null : (long?)long.Parse(attr.Value);
					if (childData.Attribute("Level", out attr)) mTreeNodeLv = int.Parse(attr.Value);
					if (childData.Attribute("Index", out attr)) mTreeNodeIdx = int.Parse(attr.Value);
				} else throw new ArgumentNullException("Node", GetMultiLangText("PathNull"));
			}

			/* 抓註解 */
			mCmt = xmlData.Element("Comment")?.Value;

			/* 是否有額外旋轉角 */
			mCalcTheta = bool.Parse(xmlData.Element("ThetaSetting/Enabled")?.Value ?? "false");


			/* 是否有旋轉角工具 */
			childData = xmlData.Element("ThetaSetting/ToolPack");
			if (childData != null && !string.IsNullOrEmpty(childData.Value)) {
				long tarID = long.Parse(childData.Value);
				var thetaTool = toolColl.FirstOrDefault(obj => obj.ID == tarID);
				mThetaTool = thetaTool as IResultOfAngle;
			}

			/* 抓 tool 囉 */
			if (xmlData.Element("Tools", out childData)) {
				childData.Elements().ForEach(
					nod => {
						long tarID = long.Parse(nod.Value);
						var tempPack = toolColl.FirstOrDefault(obj => obj.ID == tarID);
						if (tempPack != null) mToolColl.Add(tempPack as IVisionToolPack);
					}
				);
			}
		}
		#endregion

		#region Private Methods
		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		private string GetMultiLangText(string key) {
			return CtLanguage.GetMultiLangXmlText("Language.xml", key)[key];
		}

		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		private Dictionary<string, string> GetMultiLangText(params string[] key) {
			return CtLanguage.GetMultiLangXmlText("Language.xml", key);
		}

		/// <summary>建立新的一行 <see cref="DataGridViewRow"/></summary>
		/// <param name="dgv">欲建立的 <see cref="DataGridView"/></param>
		/// <param name="name">此欄位所表態的屬性名稱</param>
		/// <param name="tip">欲顯示於提示區域之提示文字</param>
		/// <returns>新的一列</returns>
		private DataGridViewRow CreateNewRow(DataGridView dgv, string name, string tip = "") {
			DataGridViewRow row = new DataGridViewRow();
			row.CreateCells(dgv, name);
			row.Tag = tip;
			return row;
		}

		/// <summary>搜尋當前 <see cref="TreeView"/> 裡有多少的 <see cref="IVisionToolPack"/></summary>
		/// <param name="node">當前使用的節點</param>
		/// <param name="nodeColl">回傳搜尋到的 <see cref="IVisionToolPack"/></param>
		private void SearchVisionToolPack(TreeNode node, ref List<IVisionToolPack> nodeColl) {
			/* 找到的就先加入集合 */
			if (node.Tag is IVisionToolPack) {
				IVisionToolPack pack = node.Tag as IVisionToolPack;
				if (pack.ToolType != typeof(ILocatorModel) && pack.ToolType != typeof(ICSharpCustomTool)) nodeColl.Add(pack);
			}

			/* 繼續往下找 */
			if (node.Nodes.Count > 0) {
				foreach (TreeNode subNode in node.Nodes) {
					SearchVisionToolPack(subNode, ref nodeColl);
				}
			}
		}

		/// <summary>搜尋當前 <see cref="TreeView"/> 裡有多少的 <see cref="IResultOfAngle"/></summary>
		/// <param name="node">當前使用的節點</param>
		/// <param name="nodeColl">回傳搜尋到的 <see cref="IResultOfAngle"/></param>
		private void SearchAngleReturner(TreeNode node, ref List<IResultOfAngle> nodeColl) {
			/* 找到的就先加入集合 */
			IResultOfAngle obj = node.Tag as IResultOfAngle;
			if (obj != null) nodeColl.Add(obj);

			/* 繼續往下找 */
			if (node.Nodes.Count > 0) {
				foreach (TreeNode subNode in node.Nodes) {
					SearchAngleReturner(subNode, ref nodeColl);
				}
			}
		}

		/// <summary>產生影像工具的累加程式碼</summary>
		/// <param name="visPack">要進行累加的影像工具包</param>
		/// <param name="varName">於 CVT 內，累加的 <see cref="VisionTransform"/> 變數名稱</param>
		/// <param name="countName">於 CVT 內，累加總共有多少 <see cref="VisionTransform"/> 變數名稱</param>
		/// <returns>對應的 CVT 程式碼</returns>
		private string GenerateCodeByTool(IVisionToolPack visPack, string varName, string countName) {
			string code = string.Empty;
			if (visPack.ReturnRoiCenter && visPack.Tool is IArcRoiTool) {
				code = string.Format(
					"\t{0}.NumericAdd({1}.SearchRegion.Center, {1}.SearchRegion.Rotation); {2}++;",
					varName,
					visPack.VariableName,
					countName
				);

			} else if (visPack.ReturnRoiCenter) {
				code = $"\t{varName}.NumericAdd({visPack.VariableName}.Offset); {countName}++;";
			} else {
				code = string.Format(
						"\tArray.ForEach({0}.GetTransformResults(), (result) => {{ {1}.NumericAdd(result); {2}++; }});",
						visPack.VariableName,
						varName,
						countName
					);
			}
			return code;
		}

		/// <summary>遞迴尋找 <see cref="TreeNode.Text"/> 為特定的字串</summary>
		/// <param name="node">欲尋找的 <see cref="TreeNode"/></param>
		/// <param name="nodeText">欲尋找的文字</param>
		/// <returns>相符的  TreeNode</returns>
		private TreeNode NodeRecursive(TreeNode node, string nodeText) {
			TreeNode found = null;
			if (node.Text == nodeText) found = node;
			else if (node.Nodes.Count > 0) {
				foreach (TreeNode item in node.Nodes) {
					found = NodeRecursive(item, nodeText);
					if (found != null) break;
				}
			}
			return found;
		}

		/// <summary>搜尋 <see cref="TreeNode"/> 符合特定文字的節點，並回傳其 <see cref="IVisionToolPack"/></summary>
		/// <param name="key">欲搜尋 <see cref="TreeNode"/> 文字</param>
		/// <param name="node">欲搜尋的 <see cref="TreeNode"/></param>
		/// <returns>符合的 <see cref="IVisionToolPack"/></returns>
		private IVisionToolPack SearchVisionToolPack(string key, TreeNode node) {
			TreeNode tempNode = NodeRecursive(node, key);
			IVisionToolPack resultPack = null;
			if (tempNode != null) {
				resultPack = tempNode.Tag as IVisionToolPack;    //如果 tag 不是 visiontoolpack 也沒差，同樣回傳 null
			}   //else 不用做，反正都是回傳 null
			return resultPack;
		}

		/// <summary>搜尋 <see cref="TreeNode"/> 符合特定文字的節點，並回傳其 <see cref="IResultOfAngle"/></summary>
		/// <param name="key">欲搜尋 <see cref="TreeNode"/> 文字</param>
		/// <param name="node">欲搜尋的 <see cref="TreeNode"/></param>
		/// <returns>符合的 <see cref="IResultOfAngle"/></returns>
		private IResultOfAngle SearchAngleReturner(string key, TreeNode node) {
			TreeNode tempNode = NodeRecursive(node, key);
			IResultOfAngle resultPack = null;
			if (tempNode != null) {
				resultPack = tempNode.Tag as IResultOfAngle;    //如果 tag 不是 visiontoolpack 也沒差，同樣回傳 null
			}   //else 不用做，反正都是回傳 null
			return resultPack;
		}
		#endregion

		#region IVisionResult Implements
		/// <summary>添加要平均結果的影像工具包</summary>
		/// <param name="tool">欲加入計算的影像工具包</param>
		public void AddVisionTool(IVisionToolPack tool) { mToolColl.Add(tool); }
		/// <summary>移除已在計算清單內的影像工具包</summary>
		/// <param name="tool">欲移除的影像工具包</param>
		public void RemoveVisionTool(IVisionToolPack tool) { mToolColl.RemoveAll(val => val.ToolPath == tool.ToolPath); }
		/// <summary>清除所有要計算的影像工具包</summary>
		public void ClearVisionTool() { mToolColl.Clear(); }
		/// <summary>指定要當作旋轉角依據的影像工具包</summary>
		/// <param name="tool">指定的影像工具包</param>
		public void AssignThetaVisionTool(IResultOfAngle tool) {
			if (mThetaTool != null) mThetaTool.IsAngleReturner = false;
			mThetaTool = tool;
		}
		/// <summary>產生此計算平均的 CVT 程式碼</summary>
		/// <param name="retVar">回傳用的區域變數</param>
		/// <returns>CVT 程式碼，一索引對應一行</returns>
		public List<string> GenerateCode(string retVar) {
			List<string> code = new List<string>();
			if (mToolColl.Count <= 0) return code;

			/* 確認每個 tool 是否都有結果 */
			code.Add("/*-- Results Average --*/");
			code.Add(string.Format("if ({0}) {{", string.Join(" && ", mToolColl.ConvertAll(tool => string.Format("{0}.ResultsAvailable", tool.VariableName)))));
			/* 宣告累計用的 VisionTransform */
			code.Add("\tVisionTransform retVT = new VisionTransform();\t//Tempoary result");
			/* 根據每個 tool 去產生累加的程式 */
			code.Add("\tint retCount = 0;\t//Count how many tools will be calculated");
			code.Add(string.Empty);
			code.Add("\t/* Sum all result */");
			mToolColl.ForEach(tool => code.Add(GenerateCodeByTool(tool, "retVT", "retCount")));
			/* 做平均 */
			code.Add(string.Empty);
			code.Add("\t/* Average results */");
			code.Add("\tretVT.NumericDivide(retCount);");
			/* 如果有指定角度，將角度改為那個影像工具的結果 */
			if (mCalcTheta && mThetaTool != null) {
				code.Add(string.Empty);
				code.Add("\t/* Change rotation of average result */");

				if (!(mThetaTool as IDynamicAngle)?.FixedAngle ?? false) {
					code.Add("\tretVT.Degrees += resultAngle;");
				} else if (mThetaTool is IVisionJudgement) {
					code.Add("\tretVT.Degrees = resultAngle;");
				} else if (mThetaTool is IVisionToolPack) {
					IVisionToolPack pack = mThetaTool as IVisionToolPack;
					if (pack.ReturnRoiCenter && pack.Tool is IArcRoiTool) {
						code.Add($"\tretVT.Degrees = {pack.VariableName}.SearchRegion.Rotation;");
					} else if (pack.ReturnRoiCenter) {
						code.Add($"\tretVT.Degrees = {pack.VariableName}.Offset.Degrees;");
					} else {
						code.Add(
							string.Format(
								"\tif ({0}.ResultsAvailable) retVT.Degrees = {0}.GetTransformResults()[0].Degrees;",
								pack.VariableName
							)
						);
					}
				}
			}

			/* 回傳最後結果 */
			code.Add(string.Empty);
			code.Add("\t/* Assign results */");
			code.Add($"\t{retVar} = new VisionTransform[] {{ retVT }};");
			/* 如果沒有結果，直接回傳 null */
			code.Add("} else return null;");
			return code;
		}

		/// <summary>更新 <see cref="TreeNode"/> 相關資訊，<see cref="TreeNode.Level"/>、<see cref="TreeNode.Index"/> 等</summary>
		public void UpdateTreeNodeInformation() {
			mTreeNodeIdx = mTreeNode.Index;
			mTreeNodeLv = mTreeNode.Level;
		}

		/// <summary>檢查移除的物件是否與此工具有關聯，如有關聯則取消之</summary>
		/// <param name="tool">移除的物件</param>
		public void ConfirmRemovedLink(IVisionProjectable tool) {
			var rmTool = mToolColl.FindAll(pack => pack.ID == tool.ID);
			if (rmTool != null && rmTool.Count > 0) rmTool.ForEach(pack => mToolColl.Remove(pack));
			rmTool.Clear();
		}
		#endregion

		#region IPropertable Implements

		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
			List<PropertyView> propList = new List<PropertyView>();

			var curToolNames = mToolColl.ConvertAll(tool => tool.Node.Text);
			var toolStr = string.Join("; ", curToolNames);

			propList.Add(
				new PropertyView(
					langMap["PropAvgTool"],
					AccessLevel.None,
					langMap["TipAvgTool"],
					"Tool",
					toolStr,
					() => {
						/* 目前平均結果就只有讓使用者選那些 tool 要平均，所以直接硬幹吧~ */
						List<IVisionToolPack> toolList = new List<IVisionToolPack>();
						SearchVisionToolPack(mTreeNode.Parent, ref toolList);
						List<string> curList = mToolColl.ConvertAll(tool => tool.Node.Text);
						List<string> chkColl;
						Stat stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcToolEnt"], toolList.ConvertAll(tool => tool.Node.Text), curList);
						if (stt == Stat.SUCCESS) {
							mToolColl.Clear();
							mToolColl = chkColl.ConvertAll(chk => toolList.Find(tool => tool.Node.Text == chk)); //直接拿搜尋到的 tool pack 來做
							this.IsModified = true;
							this.IsCompiled = false;
						}

						curList = mToolColl.ConvertAll(tool => tool.Node.Text);
						return string.Join("; ", curList);
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropAvgAngEnb"],
					AccessLevel.None,
					langMap["TipAvgAngEnb"],
					"Angle Tool Enabled",
					mCalcTheta ? langMap["ValBoolTrue"] : langMap["ValBoolFalse"],
					() => {
						string enb = mCalcTheta ? langMap["ValBoolTrue"] : langMap["ValBoolFalse"];
						Stat stt = CtInput.ComboBoxList(out enb, langMap["NormalEditTitle"], langMap["NormalEditDesc"], new List<string> { langMap["ValBoolTrue"], langMap["ValBoolFalse"] }, enb);
						if (stt == Stat.SUCCESS) {
							mCalcTheta = langMap["ValBoolTrue"].Equals(enb);
							if (mThetaTool != null) mThetaTool.IsAngleReturner = mCalcTheta;
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mCalcTheta ? langMap["ValBoolTrue"] : langMap["ValBoolFalse"];
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropAvgAng"],
					AccessLevel.None,
					langMap["TipAvgAng"],
					"Angle Tool",
					(mThetaTool as IVisionProjectable)?.Node?.Text ?? string.Empty,
					() => {
						/* 目前平均結果就只有讓使用者選那些 tool 要平均，所以直接硬幹吧~ */
						List<IResultOfAngle> toolList = new List<IResultOfAngle>();
						SearchAngleReturner(mTreeNode.Parent, ref toolList);
						List<string> toolNames = toolList.ConvertAll(tool => (tool as IVisionProjectable)?.Node?.Text ?? string.Empty);
						toolNames.Insert(0, "None");
						string chkColl;
						Dictionary<string, string> slcMsg = GetMultiLangText("SlcTool", "SlcToolEnt");
						Stat stt = CtInput.ComboBoxList(out chkColl, slcMsg["SlcTool"], slcMsg["SlcToolEnt"], toolNames, (mThetaTool as IVisionProjectable)?.Node?.Text ?? string.Empty);
						if (stt == Stat.SUCCESS) {
							if (chkColl == "None") {
								if (mThetaTool != null) mThetaTool.IsAngleReturner = false;
								mThetaTool = null;
							} else {
								if (mThetaTool != null) mThetaTool.IsAngleReturner = false;
								mThetaTool = toolList.Find(tool => (tool as IVisionProjectable).Node.Text == chkColl); //直接拿搜尋到的 tool pack 來做
								mThetaTool.IsAngleReturner = true;
							}
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return (mThetaTool as IVisionProjectable)?.Node?.Text ?? string.Empty;
					},
					() => mCalcTheta
				)
			);

            propList.Add(
				new PropertyView(
					langMap["PropCmt"],
					AccessLevel.None,
					langMap["TipCmt"],
					"Comments",
					mCmt,
					() => {
						string cmt;
						if (CtInput.Text(out cmt, langMap["Cmt"], langMap["CmtEnt"], mCmt) == Stat.SUCCESS) {
							mCmt = cmt;
							this.IsModified = true;
						}
						return mCmt;
					}
				)
			);

			return propList;
		}
		#endregion

		#region IXmlSavable Implements
		/// <summary>產生物件的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		public XmlElmt CreateXmlData(string nodeName) {
			List<XmlAttr> attrColl = new List<XmlAttr>();
			attrColl.Add(new XmlAttr("Type", "ResultAverage"));
			attrColl.Add(new XmlAttr("ID", mID.ToString()));
			List<XmlElmt> dataColl = new List<XmlElmt>();
			dataColl.Add(new XmlElmt("Comment", mCmt));
			dataColl.Add(
				new XmlElmt(
					"Node",
					mTreeNode.Text,
					new XmlAttr("ParentID", (mTreeNode.Parent?.Tag as IVisionProjectable)?.ID.ToString() ?? string.Empty),
					new XmlAttr("Level", mTreeNodeLv.ToString()),
					new XmlAttr("Index", mTreeNodeIdx.ToString())
				)
			);
			dataColl.Add(
				new XmlElmt(
					"ThetaSetting",
					string.Empty,
					new XmlElmt("Enabled", CalculateTheta.ToString()),
					new XmlElmt("ToolPack", (mThetaTool as IVisionProjectable)?.ID.ToString() ?? string.Empty)
				)
			);
			if (mToolColl.Count > 0) {
				int idx = 0;
				dataColl.Add(
					new XmlElmt(
						"Tools",
						mToolColl.ConvertAll(
							tool => new XmlElmt(
								string.Format("TOOL{0:D2}", idx++),
								tool.ID.ToString()
							)
						)
					)
				);
			} else dataColl.Add(new XmlElmt("Tools", string.Empty));
			return new XmlElmt(
				nodeName,
				attrColl,
				dataColl
			);
		}
		#endregion

		#region IDisposable Implements
		/// <summary>指出是否已經釋放過資源</summary>
		protected bool mDisposed = false;

		/// <summary>釋放此影像工具資源</summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>釋放此影像工具資源之內容</summary>
		protected virtual void Dispose(bool disposing) {
			if (!mDisposed && disposing) {
				mTreeNode = null;
				mThetaTool = null;
				mToolColl.Clear();
				mToolColl = null;
				mInputLinkID = null;
				mCmt = null;
			}
		}

		/// <summary>解構子</summary>
		~ResultAverage() {
			Console.WriteLine("Disposing : " + mID.ToString());
			Dispose(true);
		}
		#endregion
	}

	/// <summary>將指定的區域切割成一塊一塊，並回傳結果編號</summary>
	/// <remarks>如用於 Tray 盤，指出此結果為第 n 個</remarks>
	public class ResultTable : IVisionResult {

		#region Fields
		/// <summary>此結果工具的 <see cref="TreeView"/> 節點</summary>
		private TreeNode mTreeNode;
		/// <summary>暫存旋轉角之工具</summary>
		private IResultOfAngle mThetaTool;
		/// <summary>欲取得結果的影像工具</summary>
		private IVisionToolPack mRefTool;
		/// <summary>註解</summary>
		private string mCmt = string.Empty;
		/// <summary>分割區域之左上角位置</summary>
		private PointF mLeftTop = PointF.Empty;
		/// <summary>分割區域之右下角位置</summary>
		private PointF mRightBottom = PointF.Empty;
		/// <summary>此區域共有幾列</summary>
		private int mRowCount = -1;
		/// <summary>此區域共有幾行</summary>
		private int mColCount = -1;
		/// <summary>是否要使用特定的影像工具來決定旋轉角</summary>
		private bool mCalcTheta = false;
		/// <summary>欲回傳結果為第幾顆之 V+ 變數</summary>
		private string mIdxVar = string.Empty;
		/// <summary>結果排序方向</summary>
		private SortDirection mSortDir = SortDirection.LeftToRight | SortDirection.TopToBottom;
		/// <summary>於 <see cref="TreeView"/> 的節點之層數(從左至右，0 開始)</summary>
		private int mTreeNodeLv = -1;
		/// <summary>於 <see cref="TreeView"/> 的節點於同層內的深度(從上至下，0 開始)</summary>
		private int mTreeNodeIdx = -1;
		/// <summary>識別碼</summary>
		private long mID = -1;
		/// <summary>Parent Node 的識別碼</summary>
		private long? mInputLinkID = null;
		#endregion

		#region Properties
		/// <summary>取得此工具的識別碼</summary>
		public long ID { get { return mID; } }
		/// <summary>取得此結果工具的 <see cref="TreeView"/> 節點</summary>
		public TreeNode Node { get { return mTreeNode; } }
		/// <summary>取得於 <see cref="TreeView"/> 的節點之層數(從左至右，0 開始)</summary>
		public int NodeLevel { get { return mTreeNodeLv; } }
		/// <summary>取得於 <see cref="TreeView"/> 的節點於同層內的深度(從上至下，0 開始)</summary>
		public int NodeIndex { get { return mTreeNodeIdx; } }
		/// <summary>取得此工具的註解</summary>
		public string Comment { get { return mCmt; } }
		/// <summary>取得或設定工具是否被修改</summary>
		public bool IsModified { get; set; } = false;
		/// <summary>取得或設定工具是否已被編譯</summary>
		/// <remarks>(<see langword="true"/>)不需要再重新 Compile CVT  (<see langword="false"/>)需要重新 Compile</remarks>
		public bool IsCompiled { get; set; } = true;
		/// <summary>取得或設定物件，其包含相關資料</summary>
		public object Tag { get; set; } = null;
		/// <summary>取得或設定是否要使用額外的 <see cref="IVisionTool"/> 結果作為旋轉角</summary>
		/// <remarks>如 Locator 只取 X、Y，而 Theta 則由 LineFinder 決定</remarks>
		public bool CalculateTheta { get { return mCalcTheta; } set { mCalcTheta = value; } }
		/// <summary>取得當前參考的影像工具</summary>
		public IVisionToolPack ReferenceTool { get { return mRefTool; } }
		/// <summary>取得 Relatived 的識別碼</summary>
		public long? InputLinkID { get { return mInputLinkID; } }
		#endregion

		#region Constructors
		/// <summary>建構表格分割位置</summary>
		/// <param name="mainNode">欲存放此表格分割位置的 <see cref="TreeView"/> 父節點</param>
		public ResultTable(TreeNode mainNode) {
			mID = DateTime.Now.ToBinary();
			mTreeNode = mainNode.Nodes.Add("Table Slot");
			mTreeNode.Tag = this;
			mTreeNodeLv = mTreeNode.Level;
			mTreeNodeIdx = mTreeNode.Index;
			IsModified = true;
		}

		/// <summary>使用 <see cref="IXmlData"/> 建構計算表格分割位置</summary>
		/// <param name="xmlData">含有 <see cref="IVisionResult"/> 訊息之 <see cref="IXmlData"/></param>
		/// <param name="aceSrv">已連線的 ACE Server 端物件</param>
		/// <param name="toolColl">含有 <see cref="IResultOfAngle"/> 與 <see cref="IVisionToolPack"/> 之集合，供尋找參考</param>
		public ResultTable(XmlElmt xmlData, IAceServer aceSrv, IEnumerable<IVisionProjectable> toolColl) {
			/* 拆出 Type */
			string toolType = xmlData.Attribute("Type").Value;
			if (toolType != "ResultTable") throw new InvalidCastException(GetMultiLangText("VisToolNull"));

			/* 取得 ID */
			mID = long.Parse(xmlData.Attribute("ID").Value);

			/* 樹節點 */
			XmlElmt childData;
			if (xmlData.Element("Node", out childData)) {
				if (!string.IsNullOrEmpty(childData.Value)) {
					mTreeNode = new TreeNode(childData.Value);
					mTreeNode.Tag = this;

					XmlAttr attr;
					if (childData.Attribute("ParentID", out attr))
						mInputLinkID = string.IsNullOrEmpty(attr.Value) ? null : (long?)long.Parse(attr.Value);
					if (childData.Attribute("Level", out attr)) mTreeNodeLv = int.Parse(attr.Value);
					if (childData.Attribute("Index", out attr)) mTreeNodeIdx = int.Parse(attr.Value);
				} else throw new ArgumentNullException("Node", GetMultiLangText("PathNull"));
			}

			/* 抓註解 */
			mCmt = xmlData.Element("Comment")?.Value;

			/* 是否有額外旋轉角 */
			mCalcTheta = bool.Parse(xmlData.Element("ThetaSetting/Enabled")?.Value ?? "false");

			/* 是否有旋轉角工具 */
			childData = xmlData.Element("ThetaSetting/ToolPack");
			if (childData != null && !string.IsNullOrEmpty(childData.Value)) {
				long tarID = long.Parse(childData.Value);
				var thetaTool = toolColl.FirstOrDefault(obj => obj.ID == tarID);
				if (thetaTool != null) mThetaTool = thetaTool as IResultOfAngle;
			}

			/* 抓 tool 囉 */
			childData = xmlData.Element("RefTool");
			if (childData != null && !string.IsNullOrEmpty(childData.Value)) {
				long tarID = long.Parse(childData.Value);
				var tempPack = toolColl.FirstOrDefault(obj => obj.ID == tarID);
				if (tempPack != null) mRefTool = tempPack as IVisionToolPack;
			}

			/* 抓左上角 */
			childData = xmlData.Element("LeftTop");
			if (childData != null && !string.IsNullOrEmpty(childData.Value)) {
				string[] split = childData.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				mLeftTop.X = float.Parse(split[0]);
				mLeftTop.Y = float.Parse(split[1]);
			}

			/* 抓右下角 */
			childData = xmlData.Element("RightBottom");
			if (childData != null && !string.IsNullOrEmpty(childData.Value)) {
				string[] split = childData.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				mRightBottom.X = float.Parse(split[0]);
				mRightBottom.Y = float.Parse(split[1]);
			}

			/* 抓幾列 */
			childData = xmlData.Element("RowCount");
			if (childData != null && !string.IsNullOrEmpty(childData.Value)) {
				mRowCount = int.Parse(childData.Value);
			}

			/* 抓幾行 */
			childData = xmlData.Element("ColumnCount");
			if (childData != null && !string.IsNullOrEmpty(childData.Value)) {
				mColCount = int.Parse(childData.Value);
			}

			/* 抓 V+ 變數 */
			childData = xmlData.Element("VpIdexVariable");
			if (childData != null) mIdxVar = childData.Value;

			/* 抓排序方向 */
			childData = xmlData.Element("SortDirection");
			if (childData != null && !string.IsNullOrEmpty(childData.Value)) {
				mSortDir = (SortDirection)int.Parse(childData.Value);
			}
		}
		#endregion

		#region Private Methods
		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		private string GetMultiLangText(string key) {
			return CtLanguage.GetMultiLangXmlText("Language.xml", key)[key];
		}

		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		private Dictionary<string, string> GetMultiLangText(params string[] key) {
			return CtLanguage.GetMultiLangXmlText("Language.xml", key);
		}

		/// <summary>建立新的一行 <see cref="DataGridViewRow"/></summary>
		/// <param name="dgv">欲建立的 <see cref="DataGridView"/></param>
		/// <param name="name">此欄位所表態的屬性名稱</param>
		/// <param name="tip">欲顯示於提示區域之提示文字</param>
		/// <returns>新的一列</returns>
		private DataGridViewRow CreateNewRow(DataGridView dgv, string name, string tip = "") {
			DataGridViewRow row = new DataGridViewRow();
			row.CreateCells(dgv, name);
			row.Tag = tip;
			return row;
		}

		/// <summary>搜尋當前 <see cref="TreeView"/> 裡有多少的 <see cref="IVisionToolPack"/></summary>
		/// <param name="node">當前使用的節點</param>
		/// <param name="nodeColl">回傳搜尋到的 <see cref="IVisionToolPack"/></param>
		private void SearchVisionToolPack(TreeNode node, ref List<IVisionToolPack> nodeColl) {
			/* 找到的就先加入集合 */
			if (node.Tag is IVisionToolPack) {
				IVisionToolPack pack = node.Tag as IVisionToolPack;
				if (pack.ToolType != typeof(ILocatorModel) && pack.ToolType != typeof(ICSharpCustomTool)) nodeColl.Add(pack);
			}

			/* 繼續往下找 */
			if (node.Nodes.Count > 0) {
				foreach (TreeNode subNode in node.Nodes) {
					SearchVisionToolPack(subNode, ref nodeColl);
				}
			}
		}

		/// <summary>搜尋當前 <see cref="TreeView"/> 裡有多少的 <see cref="IResultOfAngle"/></summary>
		/// <param name="node">當前使用的節點</param>
		/// <param name="nodeColl">回傳搜尋到的 <see cref="IResultOfAngle"/></param>
		private void SearchAngleReturner(TreeNode node, ref List<IResultOfAngle> nodeColl) {
			/* 找到的就先加入集合 */
			IResultOfAngle obj = node.Tag as IResultOfAngle;
			if (obj != null) nodeColl.Add(obj);

			/* 繼續往下找 */
			if (node.Nodes.Count > 0) {
				foreach (TreeNode subNode in node.Nodes) {
					SearchAngleReturner(subNode, ref nodeColl);
				}
			}
		}

		/// <summary>遞迴尋找 <see cref="TreeNode.Text"/> 為特定的字串</summary>
		/// <param name="node">欲尋找的 <see cref="TreeNode"/></param>
		/// <param name="nodeText">欲尋找的文字</param>
		/// <returns>相符的  TreeNode</returns>
		private TreeNode NodeRecursive(TreeNode node, string nodeText) {
			TreeNode found = null;
			if (node.Text == nodeText) found = node;
			else if (node.Nodes.Count > 0) {
				foreach (TreeNode item in node.Nodes) {
					found = NodeRecursive(item, nodeText);
					if (found != null) break;
				}
			}
			return found;
		}

		/// <summary>搜尋 <see cref="TreeNode"/> 符合特定文字的節點，並回傳其 <see cref="IVisionToolPack"/></summary>
		/// <param name="key">欲搜尋 <see cref="TreeNode"/> 文字</param>
		/// <param name="node">欲搜尋的 <see cref="TreeNode"/></param>
		/// <returns>符合的 <see cref="IVisionToolPack"/></returns>
		private IVisionToolPack SearchVisionToolPack(string key, TreeNode node) {
			TreeNode tempNode = NodeRecursive(node, key);
			IVisionToolPack resultPack = null;
			if (tempNode != null) {
				resultPack = tempNode.Tag as IVisionToolPack;    //如果 tag 不是 visiontoolpack 也沒差，同樣回傳 null
			}   //else 不用做，反正都是回傳 null
			return resultPack;
		}

		/// <summary>搜尋 <see cref="TreeNode"/> 符合特定文字的節點，並回傳其 <see cref="IResultOfAngle"/></summary>
		/// <param name="key">欲搜尋 <see cref="TreeNode"/> 文字</param>
		/// <param name="node">欲搜尋的 <see cref="TreeNode"/></param>
		/// <returns>符合的 <see cref="IResultOfAngle"/></returns>
		private IResultOfAngle SearchAngleReturner(string key, TreeNode node) {
			TreeNode tempNode = NodeRecursive(node, key);
			IResultOfAngle resultPack = null;
			if (tempNode != null) {
				resultPack = tempNode.Tag as IResultOfAngle;    //如果 tag 不是 visiontoolpack 也沒差，同樣回傳 null
			}   //else 不用做，反正都是回傳 null
			return resultPack;
		}
		#endregion

		#region IVisionResult Implements
		/// <summary>添加要平均結果的影像工具包</summary>
		/// <param name="tool">欲加入計算的影像工具包</param>
		public void AddVisionTool(IVisionToolPack tool) { mRefTool = tool; }
		/// <summary>移除已在計算清單內的影像工具包</summary>
		/// <param name="tool">欲移除的影像工具包</param>
		public void RemoveVisionTool(IVisionToolPack tool) { if (tool == mRefTool) mRefTool = null; }
		/// <summary>清除所有要計算的影像工具包</summary>
		public void ClearVisionTool() { mRefTool = null; }
		/// <summary>指定要當作旋轉角依據的影像工具包</summary>
		/// <param name="tool">指定的影像工具包</param>
		public void AssignThetaVisionTool(IResultOfAngle tool) {
			if (mThetaTool != null) mThetaTool.IsAngleReturner = false;
			mThetaTool = tool;
		}
		/// <summary>產生此計算索引的 CVT 程式碼</summary>
		/// <param name="retVar">回傳用的區域變數</param>
		/// <returns>CVT 程式碼，一元素對應一行</returns>
		public List<string> GenerateCode(string retVar) {
			List<string> code = new List<string>();
			if (mRefTool == null) return code;

			code.Add("/*-- Calculate first object in table slot and its index --*/");
			code.Add($"if ({mRefTool.VariableName}.ResultsAvailable) {{");
			code.Add("\tint slotIdx = 1;\t//Counting of table slots");
			code.Add("\tMarkerColor slotColor = (MarkerColor)0xBEBEBE;\t//Color of drawing table slot");
			code.Add(string.Empty);
			code.Add("/* Calculate table region and split as slots */");
			code.Add($"\tCtTableRegion tabRegion = new CtTableRegion({mLeftTop.X}, {mLeftTop.Y}, {mRightBottom.X}, {mRightBottom.Y}, {mRowCount}, {mColCount});");
			code.Add("/* Get index of first result in table slots */");
			code.Add($"\tvar result = tabRegion.GetIndex({mRefTool.VariableName}.GetTransformResults(), (SortDirection){mSortDir.ToString()});");
			code.Add("/* Draw table slots */");
			code.Add("\tList<VisionRectangle> tabRect = tabRegion.Tables;");
			code.Add("\ttabRect.ForEach(");
			code.Add("\t\trect => {");
			code.Add("\t\t\tcvt.OverlayMarkers.AddLabelMarker(rect.CenterPoint.VisionTransform(), (slotIdx++).ToString()).Color = slotColor;");
			code.Add("\t\t\tcvt.OverlayMarkers.AddRectangleMarker(rect).Color = slotColor;");
			code.Add("\t\t}");
			code.Add("\t);");
			if (!string.IsNullOrEmpty(mIdxVar)) {
				code.Add(string.Empty);
				code.Add("\t/* Write first object index to V+ variable */");
				code.Add($"\tctrl.Link.SetR(\"{mIdxVar}\", result.Value + 1);");
			}
			code.Add(string.Empty);
			code.Add("\t/* Assign results */");
			if (!mCalcTheta || mThetaTool == null) code.Add($"\t{retVar} = new VisionTransform[] {{ result.Key }};");
			else {
				if (mThetaTool is IDynamicAngle) {
					IDynamicAngle pack = mThetaTool as IDynamicAngle;
					if (pack.FixedAngle) code.Add("\tVisionTransform retVT = new VisionTransform(result.Key.X, result.Key.Y, resultAngle);");
					else code.Add("\tVisionTransform retVT = new VisionTransform(result.Key.X, result.Key.Y, result.Key.Degrees + resultAngle);");
					code.Add($"\t{retVar} = new VisionTransform[] {{ retVT }};");
				} else if (mThetaTool is IVisionJudgement) {
					IVisionJudgement pack = mThetaTool as IVisionJudgement;
					code.Add("\tVisionTransform retVT = new VisionTransform(result.Key.X, result.Key.Y, resultAngle);");
					code.Add($"\t{retVar} = new VisionTransform[] {{ retVT }};");
				} else if (mThetaTool is IVisionToolPack) {
					IVisionToolPack pack = mThetaTool as IVisionToolPack;
					if (pack.ReturnRoiCenter && pack.Tool is IArcRoiTool) {
						code.Add(
							string.Format(
								"\t{0} = new VisionTransform[] {{ new VisionTransform({1}.SearchRegion.Center.X, {1}.SearchRegion.Center.Y, {1}.SearchRegion.Rotation) }};",
								retVar,
								pack.VariableName
							)
						);
					} else if (pack.ReturnRoiCenter) {
						code.Add($"\t{retVar} = new VisionTransform[] {{ {pack.VariableName}.Offset }};");
					} else {
						code.Add(string.Format("\tif ({0}.ResultsAvailable) {{", pack.VariableName));
						code.Add(
							string.Format(
								"\t\tVisionTransform retVT = new VisionTransform(result.Key.X, result.Key.Y, {0}.GetTransformResults()[0].Degrees);",
								pack.VariableName
							)
						);
						code.Add($"\t\t{retVar} = new VisionTransform[] {{ retVT }};");
						code.Add("\t} else return null;");
					}
				}
			}
			code.Add("} else {");
			if (!string.IsNullOrEmpty(mIdxVar)) {
				code.Add("\t/* Write unavailable value to V+ variable */");
				code.Add($"\tctrl.Link.SetR(\"{mIdxVar}\", -1);");
			}
			code.Add("\treturn null;");
			code.Add("}");
			return code;
		}

		/// <summary>更新 <see cref="TreeNode"/> 相關資訊，<see cref="TreeNode.Level"/>、<see cref="TreeNode.Index"/> 等</summary>
		public void UpdateTreeNodeInformation() {
			mTreeNodeIdx = mTreeNode.Index;
			mTreeNodeLv = mTreeNode.Level;
		}

		/// <summary>檢查移除的物件是否與此工具有關聯，如有關聯則取消之</summary>
		/// <param name="tool">移除的物件</param>
		public void ConfirmRemovedLink(IVisionProjectable tool) {
			if (mRefTool != null && mRefTool.ID == tool.ID) {
				mRefTool = null;
			}
		}
		#endregion

		#region IPropertable Implements

		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
			List<PropertyView> propList = new List<PropertyView>();

			Stat stt = Stat.SUCCESS;
			List<string> sort = new List<string>();
			if ((mSortDir & SortDirection.BottomToTop) == SortDirection.BottomToTop) sort.Add("Bottom → Top");
			if ((mSortDir & SortDirection.LeftToRight) == SortDirection.LeftToRight) sort.Add("Left → Right");
			if ((mSortDir & SortDirection.RightToLeft) == SortDirection.RightToLeft) sort.Add("Right → Left");
			if ((mSortDir & SortDirection.TopToBottom) == SortDirection.TopToBottom) sort.Add("Top → Bottom");
			var sortStr = string.Join(", ", sort);

			propList.Add(
				new PropertyView(
					langMap["PropTabTool"],
					AccessLevel.None,
					langMap["TipAvgTool"],
					"Referenced Tool",
					mRefTool?.Node?.Text ?? string.Empty,
					() => {
						/* 目前平均結果就只有讓使用者選那些 tool 要平均，所以直接硬幹吧~ */
						List<IVisionToolPack> toolList = new List<IVisionToolPack>();
						SearchVisionToolPack(mTreeNode.Parent, ref toolList);
						string curTool = mRefTool?.Node?.Text ?? string.Empty;
						List<string> refTool;
						stt = CtInput.CheckList(out refTool, langMap["SlcTool"], langMap["SlcToolEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
						if (stt == Stat.SUCCESS) {
							mRefTool = toolList.Find(chk => chk.Node.Text == refTool[0]);

							IVisionTool tool = mRefTool.Tool as IVisionTool;
							VisionRectangle rect = tool?.GetRectangularSearchRegion();
							if (rect != null) {
								mLeftTop.X = (float)rect.TopLeftPoint.X;
								mLeftTop.Y = (float)rect.TopLeftPoint.Y;
								mRightBottom.X = (float)rect.BottomRightPoint.X;
								mRightBottom.Y = (float)rect.BottomRightPoint.Y;
							} else {
								mLeftTop.X = 0; mLeftTop.Y = 0;
								mRightBottom.X = 0; mRightBottom.Y = 0;
							}

							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mRefTool?.Node?.Text ?? string.Empty;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropAvgAngEnb"],
					AccessLevel.None,
					langMap["TipAvgAngEnb"],
					"Angle Tool Enabled",
					mCalcTheta ? langMap["ValBoolTrue"] : langMap["ValBoolFalse"],
					() => {
						string enb = mCalcTheta ? langMap["ValBoolTrue"] : langMap["ValBoolFalse"];
						stt = CtInput.ComboBoxList(out enb, langMap["NormalEditTitle"], langMap["NormalEditDesc"], new List<string> { langMap["ValBoolTrue"], langMap["ValBoolFalse"] }, enb);
						if (stt == Stat.SUCCESS) {
							mCalcTheta = langMap["ValBoolTrue"].Equals(enb);
							if (mThetaTool != null) mThetaTool.IsAngleReturner = mCalcTheta;
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mCalcTheta ? langMap["ValBoolTrue"] : langMap["ValBoolFalse"];
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropAvgAng"],
					AccessLevel.None,
					langMap["TipAvgAng"],
					"Angle Tool",
					(mThetaTool as IVisionProjectable)?.Node?.Text ?? string.Empty,
					() => {
						/* 目前平均結果就只有讓使用者選那些 tool 要平均，所以直接硬幹吧~ */
						List<IResultOfAngle> roa = new List<IResultOfAngle>();
						SearchAngleReturner(mTreeNode.Parent, ref roa);
						List<string> toolNames = roa.ConvertAll(tool => (tool as IVisionProjectable)?.Node?.Text ?? string.Empty);
						toolNames.Insert(0, "None");
						string chkColl;
						stt = CtInput.ComboBoxList(out chkColl, langMap["SlcTool"], langMap["SlcToolEnt"], toolNames, (mThetaTool as IVisionProjectable)?.Node?.Text ?? string.Empty);
						if (stt == Stat.SUCCESS) {
							if ("None".Equals(chkColl)) {
								if (mThetaTool != null) {
									mThetaTool.IsAngleReturner = false;
								}
								mThetaTool = null;
							} else {
								if (mThetaTool != null) {
									mThetaTool.IsAngleReturner = false;
								}
								mThetaTool = roa.Find(tool => (tool as IVisionProjectable).Node.Text == chkColl); //直接拿搜尋到的 tool pack 來做
								mThetaTool.IsAngleReturner = true;
							}
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return (mThetaTool as IVisionProjectable)?.Node?.Text ?? string.Empty;
					},
					() => mCalcTheta
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropTabSortDir"],
					AccessLevel.None,
					langMap["TipTabSortDir"],
					"Sorting Direction",
					sortStr,
					() => {
						string retStr = string.Empty;
						Dictionary<string, string> ordMsg = GetMultiLangText("OrdTit", "OrdEnt", "OrdInv");
						List<string> ordStr = new List<string> { "Top → Bottom", "Bottom → Top", "Left → Right", "Right → Left" };
						List<string> curStr = new List<string>();
						if ((mSortDir & SortDirection.BottomToTop) == SortDirection.BottomToTop) curStr.Add("Bottom → Top");
						if ((mSortDir & SortDirection.LeftToRight) == SortDirection.LeftToRight) curStr.Add("Left → Right");
						if ((mSortDir & SortDirection.RightToLeft) == SortDirection.RightToLeft) curStr.Add("Right → Left");
						if ((mSortDir & SortDirection.TopToBottom) == SortDirection.TopToBottom) curStr.Add("Top → Bottom");
						List<string> ordRet;
						stt = CtInput.CheckList(out ordRet, ordMsg["OrdTit"], ordMsg["OrdEnt"], ordStr, curStr);
						if (stt == Stat.SUCCESS) {
							SortDirection sortDir = SortDirection.Undefined;
							if ((ordRet.Contains("Top → Bottom") && ordRet.Contains("Bottom → Top")) || (ordRet.Contains("Left → Right") && ordRet.Contains("Right → Left"))) {
								CtMsgBox.Show(ordMsg["OrdTit"], ordMsg["OrdInv"]);
								retStr = string.Join(", ", curStr);
							} else {
								if (ordRet.Contains("Top → Bottom")) sortDir |= SortDirection.TopToBottom;
								if (ordRet.Contains("Bottom → Top")) sortDir |= SortDirection.BottomToTop;
								if (ordRet.Contains("Left → Right")) sortDir |= SortDirection.LeftToRight;
								if (ordRet.Contains("Right → Left")) sortDir |= SortDirection.RightToLeft;
								mSortDir = sortDir;
								retStr = string.Join(", ", ordRet);
								this.IsModified = true;
								this.IsCompiled = false;
							}
						} else retStr = string.Join(", ", curStr);
						return retStr;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropTabLt"],
					AccessLevel.None,
					langMap["TipTabLt"],
					"Left-Top Coordinate",
					$"{mLeftTop.X}, {mLeftTop.Y}",
					() => {
						string curLt = $"{mLeftTop.X}, {mLeftTop.Y}";
						string ltStr;
						string retStr = curLt;
						stt = CtInput.Text(out ltStr, langMap["LftTopTit"], langMap["LftTopEnt"], curLt);
						if (stt == Stat.SUCCESS && !string.IsNullOrEmpty(ltStr)) {
							string[] split = ltStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							if (split.Length == 2) {
								float tempX, tempY;
								if (float.TryParse(split[0], out tempX) && float.TryParse(split[1], out tempY)) {
									mLeftTop.X = tempX;
									mLeftTop.Y = tempY;
									this.IsModified = true;
									this.IsCompiled = false;
								} else {
									CtMsgBox.Show(langMap["LftTopTit"], langMap["LftTopInv"]);
								}
							} else {
								CtMsgBox.Show(langMap["LftTopTit"], langMap["LftTopInv"]);
							}
						}
						return $"{mLeftTop.X}, {mLeftTop.Y}";
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropTabRb"],
					AccessLevel.None,
					langMap["TipTabRb"],
					"Bottom-Right Coordinate",
					$"{mRightBottom.X}, {mRightBottom.Y}",
					() => {
						string curRb = $"{mRightBottom.X}, {mRightBottom.Y}";
						string rbStr;
						stt = CtInput.Text(out rbStr, langMap["RitBtmTit"], langMap["RitBtmEnt"], curRb);
						if (stt == Stat.SUCCESS && !string.IsNullOrEmpty(rbStr)) {
							string[] split = rbStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							if (split.Length == 2) {
								float tempX, tempY;
								if (float.TryParse(split[0], out tempX) && float.TryParse(split[1], out tempY)) {
									mRightBottom.X = tempX;
									mRightBottom.Y = tempY;
									this.IsModified = true;
									this.IsCompiled = false;
								} else {
									CtMsgBox.Show(langMap["RitBtmTit"], langMap["RitBtmInv"]);
								}
							} else {
								CtMsgBox.Show(langMap["RitBtmTit"], langMap["RitBtmInv"]);
							}
						}
						return $"{mRightBottom.X}, {mRightBottom.Y}";
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropTabRow"],
					AccessLevel.None,
					langMap["TipTabRow"],
					"Row Count",
					mRowCount.ToString(),
					() => {
						string curRow = mRowCount.ToString();
						string rowStr;
						stt = CtInput.Text(out rowStr, langMap["TabRowTit"], langMap["TabRowEnt"], curRow);
						if (stt == Stat.SUCCESS) {
							int rowCount;
							if (int.TryParse(rowStr, out rowCount)) {
								mRowCount = rowCount;
								this.IsModified = true;
								this.IsCompiled = false;
							} else {
								CtMsgBox.Show(langMap["TabRowTit"], langMap["TabRowInv"]);
							}
						}
						return mRowCount.ToString();
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropTabCol"],
					AccessLevel.None,
					langMap["TipTabCol"],
					"Column Count",
					mColCount.ToString(),
					() => {
						string curCol = mColCount.ToString();
						string colStr;
						stt = CtInput.Text(out colStr, langMap["TabColTit"], langMap["TabColEnt"], curCol);
						if (stt == Stat.SUCCESS) {
							int colCount;
							if (int.TryParse(colStr, out colCount)) {
								mColCount = colCount;
								this.IsModified = true;
								this.IsCompiled = false;
							} else {
								CtMsgBox.Show(langMap["TabColTit"], langMap["TabColInv"]);
							}
						}
						return mColCount.ToString();
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropTabVp"],
					AccessLevel.None,
					langMap["TipTabVp"],
					"Output V+ Variable",
					mIdxVar,
					() => {
						string vpVar;
						if (CtInput.Text(out vpVar, langMap["TabVpTit"], langMap["TabVpEnt"], mIdxVar) == Stat.SUCCESS) {
							mIdxVar = vpVar;
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mIdxVar;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropCmt"],
					AccessLevel.None,
					langMap["TipCmt"],
					"Comments",
					mCmt,
					() => {
						string cmt;
						if (CtInput.Text(out cmt, langMap["Cmt"], langMap["CmtEnt"], mCmt) == Stat.SUCCESS) {
							mCmt = cmt;
							this.IsModified = true;
						}
						return mCmt;
					}
				)
			);

			return propList;
		}
		#endregion

		#region IXmlSavable Implements
		/// <summary>產生物件的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		public XmlElmt CreateXmlData(string nodeName) {
			List<XmlAttr> attrColl = new List<XmlAttr>();
			attrColl.Add(new XmlAttr("Type", "ResultTable"));
			attrColl.Add(new XmlAttr("ID", mID.ToString()));
			List<XmlElmt> dataColl = new List<XmlElmt>();
			dataColl.Add(new XmlElmt("Comment", mCmt));
			dataColl.Add(
				new XmlElmt(
					"Node",
					mTreeNode.Text,
					new XmlAttr("ParentID", (mTreeNode.Parent?.Tag as IVisionProjectable)?.ID.ToString() ?? string.Empty),
					new XmlAttr("Level", mTreeNodeLv.ToString()),
					new XmlAttr("Index", mTreeNodeIdx.ToString())
				)
			);
			dataColl.Add(
				new XmlElmt(
					"ThetaSetting",
					new XmlElmt("Enabled", CalculateTheta.ToString()),
					new XmlElmt("ToolPack", (mThetaTool as IVisionProjectable)?.ID.ToString() ?? string.Empty)
				)
			);
			dataColl.Add(new XmlElmt("RefTool", mRefTool?.ID.ToString() ?? string.Empty));
			dataColl.Add(new XmlElmt("LeftTop", $"{mLeftTop.X}, {mLeftTop.Y}"));
			dataColl.Add(new XmlElmt("RightBottom", $"{mRightBottom.X}, {mRightBottom.Y}"));
			dataColl.Add(new XmlElmt("RowCount", mRowCount.ToString()));
			dataColl.Add(new XmlElmt("ColumnCount", mColCount.ToString()));
			dataColl.Add(new XmlElmt("VpIdexVariable", mIdxVar));
			dataColl.Add(new XmlElmt("SortDirection", ((int)mSortDir).ToString()));

			return new XmlElmt(
				nodeName,
				attrColl,
				dataColl
			);
		}
		#endregion

		#region IDisposable Implements
		/// <summary>指出是否已經釋放過資源</summary>
		protected bool mDisposed = false;

		/// <summary>釋放此影像工具資源</summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>釋放此影像工具資源之內容</summary>
		protected virtual void Dispose(bool disposing) {
			if (!mDisposed && disposing) {
				mTreeNode = null;
				mThetaTool = null;
				mRefTool = null;
				mIdxVar = null;
				mInputLinkID = null;
				mLeftTop = PointF.Empty;
				mRightBottom = PointF.Empty;
			}
		}

		/// <summary>解構子</summary>
		~ResultTable() {
			Console.WriteLine("Disposing : " + mID.ToString());
			Dispose(true);
		}
		#endregion
	}

	#endregion

	#region Declaration - Judgement

	/// <summary>距離評斷工具。量測兩影像工具之結果距離，並回報是否合格</summary>
	public class DistanceJudge : IVisionJudgement {

		#region Fields
		private double mDist = 0;
		private double mTol = 0;
		private TreeNode mTreeNode;
		private IVisionToolPack mTool1;
		private IVisionToolPack mTool2;
		private string mCmt = string.Empty;
		private bool mAsNg = true;
		private int mTreeNodeLv = -1;
		private int mTreeNodeIdx = -1;
		private long mID = -1;
		private long? mInputLinkID = null;
		#endregion

		#region Properties
		/// <summary>取得此工具的識別碼</summary>
		public long ID { get { return mID; } }
		/// <summary>取得此評斷工具於 <see cref="TreeView"/> 之節點</summary>
		public TreeNode Node { get { return mTreeNode; } }
		/// <summary>取得於 <see cref="TreeView"/> 的節點之層數(從左至右，0 開始)</summary>
		public int NodeLevel { get { return mTreeNodeLv; } }
		/// <summary>取得於 <see cref="TreeView"/> 的節點於同層內的深度(從上至下，0 開始)</summary>
		public int NodeIndex { get { return mTreeNodeIdx; } }
		/// <summary>取得當前量測的數值</summary>
		public double Distance { get { return mDist; } }
		/// <summary>取得此評斷工具所允許的距離誤差</summary>
		public double Tolerance { get { return mTol; } }
		/// <summary>取得此工具的文字註解</summary>
		public string Comment { get { return mCmt; } }
		/// <summary>取得或設定工具是否被修改</summary>
		public bool IsModified { get; set; } = false;
		/// <summary>取得或設定工具是否已被編譯</summary>
		/// <remarks>(<see langword="true"/>)不需要再重新 Compile CVT  (<see langword="false"/>)需要重新 Compile</remarks>
		public bool IsCompiled { get; set; } = true;
		/// <summary>取得是否需判定為 NG 產品</summary>
		public bool JudgeAsNG { get { return mAsNg; } }
		/// <summary>取得或設定物件，其包含相關資料</summary>
		public object Tag { get; set; } = null;
		/// <summary>取得 Relatived 的識別碼</summary>
		public long? InputLinkID { get { return mInputLinkID; } }
		/// <summary>取得起點工具</summary>
		public IVisionToolPack Tool1 { get { return mTool1; } }
		/// <summary>取得終點工具</summary>
		public IVisionToolPack Tool2 { get { return mTool2; } }
		/// <summary>取得此工具是否可被複製</summary>
		public bool IsCopyable { get { return true; } }
		#endregion

		#region Constructors
		/// <summary>建構新的距離評斷工具</summary>
		/// <param name="mainNode">欲存放此工具的父節點</param>
		public DistanceJudge(TreeNode mainNode) {
			mID = DateTime.Now.ToBinary();
			mTreeNode = mainNode.Nodes.Add("Distance Measure");
			mTreeNode.Tag = this;
			mTreeNodeLv = mTreeNode.Level;
			mTreeNodeIdx = mTreeNode.Index;
			IsModified = true;
		}

		/// <summary>複製距離評斷工具</summary>
		/// <param name="mainNode">欲存放此工具的父節點</param>
		/// <param name="copyTool">欲複製的來源</param>
		public DistanceJudge(TreeNode mainNode, DistanceJudge copyTool) {
			mID = DateTime.Now.ToBinary();
			mTreeNode = mainNode.Nodes.Add("Distance Measure");
			mTreeNode.Tag = this;
			mTreeNodeLv = mTreeNode.Level;
			mTreeNodeIdx = mTreeNode.Index;
			IsModified = true;

			mDist = copyTool.Distance;
			mCmt = copyTool.Comment;
			mTol = copyTool.Tolerance;
			mTool1 = copyTool.Tool1;
			mTool2 = copyTool.Tool2;
			mAsNg = copyTool.JudgeAsNG;
			Tag = copyTool.Tag;
		}

		/// <summary>透過 <see cref="IXmlData"/> 建構距離評斷工具</summary>
		/// <param name="xmlData">含有評斷工具資料的 XML 節點</param>
		/// <param name="aceSrc">已連線的 ACE Server</param>
		/// <param name="toolColl">含有 <see cref="IVisionToolPack"/> 之集合，供尋找參考</param>
		public DistanceJudge(XmlElmt xmlData, IAceServer aceSrc, IEnumerable<IVisionToolPack> toolColl) {
			/* 拆出 Type */
			string toolType = xmlData.Attribute("Type").Value;
			if (toolType != "DistanceJudge") throw new InvalidCastException(GetMultiLangText("VisToolNotSup"));

			/* 取得 ID */
			mID = long.Parse(xmlData.Attribute("ID").Value);

			/* 樹節點 */
			XmlElmt childData;
			if (xmlData.Element("Node", out childData)) {
				if (!string.IsNullOrEmpty(childData.Value)) {
					mTreeNode = new TreeNode(childData.Value);
					mTreeNode.Tag = this;

					XmlAttr attr;
					if (childData.Attribute("ParentID", out attr))
						mInputLinkID = string.IsNullOrEmpty(attr.Value) ? null : (long?)long.Parse(attr.Value);
					if (childData.Attribute("Level", out attr)) mTreeNodeLv = int.Parse(attr.Value);
					if (childData.Attribute("Index", out attr)) mTreeNodeIdx = int.Parse(attr.Value);
				} else throw new ArgumentNullException("Node", GetMultiLangText("PathNull"));
			}

			/* 抓註解 */
			mCmt = xmlData.Element("Comment")?.Value;

			/* 抓距離 */
			mAsNg = bool.Parse(xmlData.Element("AsNG")?.Value ?? "false");

			/* 抓距離 */
			mDist = double.Parse(xmlData.Element("Distance")?.Value ?? "0");

			/* 抓誤差 */
			mTol = double.Parse(xmlData.Element("Tolerance")?.Value ?? "0");

			/* 抓 tool 囉 */
			childData = xmlData.Element("Tool1");
			if (childData != null && !string.IsNullOrEmpty(childData.Value)) {
				long tarID = long.Parse(childData.Value);
				IVisionToolPack tempPack = toolColl.FirstOrDefault(obj => obj.ID == tarID);
				if (tempPack != null) mTool1 = tempPack;
			}

			/* 抓 tool 囉 */
			childData = xmlData.Element("Tool2");
			if (childData != null && !string.IsNullOrEmpty(childData.Value)) {
				long tarID = long.Parse(childData.Value);
				IVisionToolPack tempPack = toolColl.FirstOrDefault(obj => obj.ID == tarID);
				if (tempPack != null) mTool2 = tempPack;
			}
		}
		#endregion

		#region Private Function
		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		private string GetMultiLangText(string key) {
			return CtLanguage.GetMultiLangXmlText("Language.xml", key)[key];
		}

		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		private Dictionary<string, string> GetMultiLangText(params string[] key) {
			return CtLanguage.GetMultiLangXmlText("Language.xml", key);
		}

		/// <summary>建立一行新的 <see cref="DataGridViewRow"/></summary>
		/// <param name="dgv">欲顯示此欄位的 <see cref="DataGridView"/></param>
		/// <param name="name">於第一欄顯示屬性名稱之字串</param>
		/// <param name="tip">欲顯示於提示視窗的提示文字</param>
		/// <returns>已建立的欄位</returns>
		private DataGridViewRow CreateNewRow(DataGridView dgv, string name, string tip = "") {
			DataGridViewRow row = new DataGridViewRow();
			row.CreateCells(dgv, name);
			row.Tag = tip;
			return row;
		}

		/// <summary>搜尋 <see cref="TreeView"/> 內的影像工具包</summary>
		/// <param name="node">要開始往下搜尋的 <see cref="TreeView"/> 節點</param>
		/// <param name="nodeColl">集合</param>
		private void SearchVisionToolPack(TreeNode node, ref List<IVisionToolPack> nodeColl) {
			/* 判斷是不是 tool pack，是的話就直接加入即和吧~ */
			if (node.Tag is IVisionToolPack) {
				IVisionToolPack pack = node.Tag as IVisionToolPack;
				//排除 model 和 cvt，這沒辦法當結果的來源 (cvt是因為目前僅允許一份，人為操作當然可以用...)
				if (pack.ToolType != typeof(ILocatorModel) && pack.ToolType != typeof(ICSharpCustomTool)) nodeColl.Add(pack);
			}

			/* 如果還有東西就往下做吧 */
			if (node.Nodes.Count > 0) {
				foreach (TreeNode subNode in node.Nodes) {
					SearchVisionToolPack(subNode, ref nodeColl);
				}
			}
		}

		/// <summary>遞迴尋找 <see cref="TreeNode.Text"/> 為特定的字串</summary>
		/// <param name="node">欲尋找的 <see cref="TreeNode"/></param>
		/// <param name="nodeText">欲尋找的文字</param>
		/// <returns>相符的  TreeNode</returns>
		private TreeNode NodeRecursive(TreeNode node, string nodeText) {
			TreeNode found = null;
			if (node.Text == nodeText) found = node;
			else if (node.Nodes.Count > 0) {
				foreach (TreeNode item in node.Nodes) {
					found = NodeRecursive(item, nodeText);
					if (found != null) break;
				}
			}
			return found;
		}

		/// <summary>搜尋 <see cref="TreeNode"/> 符合特定文字的節點，並回傳其 <see cref="IVisionToolPack"/></summary>
		/// <param name="key">欲搜尋 <see cref="TreeNode"/> 文字</param>
		/// <param name="node">欲搜尋的 <see cref="TreeNode"/></param>
		/// <returns>符合的 <see cref="IVisionToolPack"/></returns>
		private IVisionToolPack SearchVisionToolPack(string key, TreeNode node) {
			TreeNode tempNode = NodeRecursive(node, key);
			IVisionToolPack resultPack = null;
			if (tempNode != null) {
				resultPack = tempNode.Tag as IVisionToolPack;    //如果 tag 不是 visiontoolpack 也沒差，同樣回傳 null
			}   //else 不用做，反正都是回傳 null
			return resultPack;
		}

		private void SetRowVisible(IEnumerable<DataGridViewRow> rows) {
			string propName = string.Empty;
			foreach (DataGridViewRow row in rows) {
				propName = row.Cells[0].Value.ToString();
				switch (propName) {
					case "Distance":
					case "Tolerance":
						row.Visible = mAsNg;
						break;
					default:
						row.Visible = true;
						break;
				}
			}
		}
		#endregion

		#region IVisionJudgement Implements
		/// <summary>指定計算用的第一組影像工具包</summary>
		/// <param name="tool">欲指定的影像工具包</param>
		public void AssignVisionTool_1(IVisionToolPack tool) {
			mTool1 = tool;
		}
		/// <summary>指定計算用的第二組影像工具包</summary>
		/// <param name="tool">欲指定的影像工具包</param>
		public void AssignVisionTool_2(IVisionToolPack tool) {
			mTool2 = tool;
		}

		/// <summary>取得此影像工具之對應的 CVT 程式碼</summary>
		/// <param name="passVar">切換 PASS 或 NG 的區域變數名稱</param>
		/// <returns>CVT 程式碼，一索引對應一行</returns>
		public List<string> GenerateCode(string passVar) {
			List<string> code = new List<string>();
			/* 確認 tool 都有結果 */
			if (mTool1 == null || mTool2 == null) return code;
			code.Add($"/*-- Distance measure and judge bewteen {mTool1.VariableName} and {mTool2.VariableName} --*/");
			code.Add(string.Format("if ({0}.ResultsAvailable && {1}.ResultsAvailable) {{", mTool1.VariableName, mTool2.VariableName));
			code.Add(string.Empty);
			code.Add("\t/* Get two target positions */");
			/* 取得結果 */
			if (mTool1.ReturnRoiCenter && mTool1.Tool is IArcRoiTool) {
				code.Add(
					string.Format(
						"\tVisionTransform pos1 = new VisionTransform({0}.SearchRegion.Center.X, {0}.SearchRegion.Center.Y, {0}.SearchRegion.Rotation);",
						mTool1.VariableName
					)
				);
			} else if (mTool1.ReturnRoiCenter) {
				code.Add($"\tVisionTransform pos1 = {mTool1.VariableName}.Offset;");
			} else {
				code.Add($"\tVisionTransform pos1 = {mTool1.VariableName}.GetTransformResults()[0];");
			}

			if (mTool2.ReturnRoiCenter && mTool2.Tool is IArcRoiTool) {
				code.Add(
					string.Format(
						"\tVisionTransform pos2 = new VisionTransform({0}.SearchRegion.Center.X, {0}.SearchRegion.Center.Y, {0}.SearchRegion.Rotation);",
						mTool2.VariableName
					)
				);
			} else if (mTool2.ReturnRoiCenter) {
				code.Add($"\tVisionTransform pos2 = {mTool2.VariableName}.Offset;");
			} else {
				code.Add($"\tVisionTransform pos2 = {mTool2.VariableName}.GetTransformResults()[0];");
			}

			/* 直接使用 Transform3D 計算平面距離 */
			code.Add(string.Empty);
			code.Add($"\t/* Calculate distance from {mTool1.VariableName} to {mTool2.VariableName} */");
			code.Add("\tdouble dist = pos1.Transform3D.DistanceFrom(pos2.Transform3D);");
			code.Add(string.Format("\tTrace.WriteLine(string.Format(\"Distance between \\\"{0}\\\" and \\\"{1}\\\" is {{0:F3}}\", dist));", mTool1.VariableName, mTool2.VariableName));
			if (mAsNg) {
				code.Add(string.Empty);
				code.Add($"\t/* Check distance is PASS or NG */");
				/* 判斷是否在合格範圍內，合格則畫 PASS 框 */
				code.Add("\tVisionLine line = new VisionLine(pos1, pos2);");
				code.Add("\tdouble angle = line.Degrees < 0 ? line.Degrees + 180 : line.Degrees;");
                #region 修改 by Jay 2017/08/24
                //code.Add("\tVisionTransform distLbLoc = new VisionTransform(line.CenterPoint.X, line.CenterPoint.Y + (angle < 45 || angle > 135 ? angle * 0.01 : 0), 0);");
                //不明白為什麼Y要加上(angle < 45 || angle > 135 ? angle * 0.01 : 0)
                //angle <= 135都沒事
                //一但超過之後，量測結果Y軸位置就飛的世界高
                //暫時乾脆統一給0就好 
                #endregion 修改 by Jay 2017/08/24
                code.Add("\tVisionTransform distLbLoc = new VisionTransform(line.CenterPoint.X, line.CenterPoint.Y , 0);");
                code.Add(string.Format("\tif ({0} < dist || dist < {1}) {{", (mDist + mTol).ToString("F3"), (mDist - mTol).ToString("F3")));
				code.Add("\t\tcvt.OverlayMarkers.AddLineMarker(pos1, pos2).Color = clrRoiFail;");
				code.Add("\t\tcvt.OverlayMarkers.AddLabelMarker(distLbLoc, dist.ToString(\"F2\") + \"mm\").Color = clrRoiFail;");
				code.Add($"\t\t{passVar} = false;");
				/* 如果不合格，畫 NG 框 */
				code.Add("\t} else {");
				code.Add("\t\tMarkerColor distLbClr = (MarkerColor)0xFF80FF;");
				code.Add("\t\tcvt.OverlayMarkers.AddLineMarker(pos1, pos2).Color = distLbClr;");
				code.Add("\t\tcvt.OverlayMarkers.AddLabelMarker(distLbLoc, dist.ToString(\"F2\") + \"mm\").Color = distLbClr;");
				code.Add("\t}");
			} else {
				code.Add(string.Empty);
				code.Add("\t/* Measure distance and draw results */");
				code.Add("\tVisionLine line = new VisionLine(pos1, pos2);");
				code.Add("\tdouble angle = line.Degrees < 0 ? line.Degrees + 180 : line.Degrees;");
				code.Add("\tMarkerColor distLbClr = (MarkerColor)0xFF80FF;");
				code.Add("\tVisionTransform distLbLoc = new VisionTransform(line.CenterPoint.X, line.CenterPoint.Y + (angle < 45 || angle > 135 ? angle * 0.01 : 0), 0);");
				code.Add("\tcvt.OverlayMarkers.AddLineMarker(pos1, pos2).Color = distLbClr;");
				code.Add("\tcvt.OverlayMarkers.AddLabelMarker(distLbLoc, dist.ToString(\"F2\") + \"mm\").Color = distLbClr;");
			}
			code.Add("}");
			code.Add(string.Empty);
			return code;
		}

		/// <summary>更新 <see cref="TreeNode"/> 相關資訊，<see cref="TreeNode.Level"/>、<see cref="TreeNode.Index"/> 等</summary>
		public void UpdateTreeNodeInformation() {
			mTreeNodeIdx = mTreeNode.Index;
			mTreeNodeLv = mTreeNode.Level;
		}

		/// <summary>檢查移除的物件是否與此工具有關聯，如有關聯則取消之</summary>
		/// <param name="tool">移除的物件</param>
		public void ConfirmRemovedLink(IVisionProjectable tool) {
			if (mTool1 != null && mTool1.ID == tool.ID) mTool1 = null;
			if (mTool2 != null && mTool2.ID == tool.ID) mTool2 = null;
		}
		#endregion

		#region IPropertable Implements

		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
			List<PropertyView> propList = new List<PropertyView>();

			Stat stt = Stat.SUCCESS;

			propList.Add(
				new PropertyView(
					langMap["PropDistNg"],
					AccessLevel.None,
					langMap["TipDistNg"],
					"NG Judging Enabled",
					mAsNg ? langMap["ValBoolTrue"] : langMap["ValBoolFalse"],
					() => {
						string curStr = mAsNg ? langMap["ValBoolTrue"] : langMap["ValBoolFalse"];
						string ng = string.Empty;
						stt = CtInput.ComboBoxList(out ng, langMap["DistNgTitle"], langMap["DistNgEnt"], new List<string> { langMap["ValBoolTrue"], langMap["ValBoolFalse"] }, curStr);
						if (stt == Stat.SUCCESS) {
							mAsNg = langMap["ValBoolTrue"].Equals(ng);
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mAsNg ? langMap["ValBoolTrue"] : langMap["ValBoolFalse"];
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropDist"],
					AccessLevel.None,
					langMap["TipDist"],
					"Distance",
					mDist.ToString("F3"),
					() => {
						string disStr = string.Empty;
						stt = CtInput.Text(out disStr, langMap["NormalEditTitle"], langMap["DisEnt"], mDist.ToString("F3"));
						if (stt == Stat.SUCCESS) {
							if (!double.TryParse(disStr, out mDist)) {
								CtMsgBox.Show(langMap["NormalEditTitle"], langMap["ValEditReal"], MsgBoxBtn.OK, MsgBoxStyle.Error);
							} else {
								this.IsModified = true;
								this.IsCompiled = false;
							}
						}
						return mDist.ToString("F3");
					},
					() => mAsNg
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropDistTol"],
					AccessLevel.None,
					langMap["TipDistTol"],
					"Tolerance",
					mTol.ToString("F3"),
					() => {
						string tolStr = string.Empty;
						stt = CtInput.Text(out tolStr, langMap["NormalEditTitle"], "Please enter the tolerance of distance", mTol.ToString("F3"));
						if (stt == Stat.SUCCESS) {
							if (!double.TryParse(tolStr, out mTol)) {
								CtMsgBox.Show(langMap["NormalEditTitle"], langMap["ValEditReal"], MsgBoxBtn.OK, MsgBoxStyle.Error);
							} else {
								this.IsModified = true;
								this.IsCompiled = false;
							}
						}
						return mTol.ToString("F3");
					},
					() => mAsNg
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropDistTol1"],
					AccessLevel.None,
					langMap["TipDistTol1"],
					"Referenced Tool - 1",
					mTool1?.Node.Text ?? string.Empty,
					() => {
						List<IVisionToolPack> toolList = new List<IVisionToolPack>();
						SearchVisionToolPack(mTreeNode.Parent, ref toolList);
						List<string> chkColl;
						string curTool = mTool1?.Node.Text ?? string.Empty;
						stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcDistEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
						if (stt == Stat.SUCCESS) {
							mTool1 = toolList.Find(tool => tool.Node.Text == chkColl[0]);
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mTool1?.Node.Text ?? string.Empty;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropDistTol2"],
					AccessLevel.None,
					langMap["TipDistTol2"],
					"Referenced Tool - 2",
					mTool2?.Node.Text ?? string.Empty,
					() => {
						List<IVisionToolPack> toolList2 = new List<IVisionToolPack>();
						SearchVisionToolPack(mTreeNode.Parent, ref toolList2);
						List<string> chkColl2;
						string curTool2 = mTool2?.Node.Text ?? string.Empty;
						stt = CtInput.CheckList(out chkColl2, langMap["SlcTool"], langMap["SlcDistEnt"], toolList2.ConvertAll(tool => tool.Node.Text), curTool2, true);
						if (stt == Stat.SUCCESS) {
							mTool2 = toolList2.Find(tool => tool.Node.Text == chkColl2[0]);
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mTool2?.Node.Text ?? string.Empty;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropCmt"],
					AccessLevel.None,
					langMap["TipCmt"],
					"Comments",
					mCmt,
					() => {
						string cmt;
						if (CtInput.Text(out cmt, langMap["Cmt"], langMap["CmtEnt"], mCmt) == Stat.SUCCESS) {
							mCmt = cmt;
							this.IsModified = true;
						}
						return mCmt;
					}
				)
			);

			return propList;
		}

		#endregion

		#region IXmlSavable Implements
		/// <summary>產生物件的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		public XmlElmt CreateXmlData(string nodeName) {
			List<XmlAttr> attrColl = new List<XmlAttr>();
			attrColl.Add(new XmlAttr("Type", "DistanceJudge"));
			attrColl.Add(new XmlAttr("ID", mID.ToString()));
			List<XmlElmt> dataColl = new List<XmlElmt>();
			dataColl.Add(new XmlElmt("Comment", mCmt));
			dataColl.Add(
				new XmlElmt(
					"Node",
					mTreeNode.Text,
					new XmlAttr("ParentID", (mTreeNode.Parent?.Tag as IVisionProjectable)?.ID.ToString() ?? string.Empty),
					new XmlAttr("Level", mTreeNodeLv.ToString()),
					new XmlAttr("Index", mTreeNodeIdx.ToString())
				)
			);
			dataColl.Add(new XmlElmt("AsNG", mAsNg.ToString()));
			dataColl.Add(new XmlElmt("Distance", mDist.ToString("F3")));
			dataColl.Add(new XmlElmt("Tolerance", mTol.ToString("F3")));
			dataColl.Add(new XmlElmt("Tool1", mTool1?.ID.ToString() ?? string.Empty));
			dataColl.Add(new XmlElmt("Tool2", mTool2?.ID.ToString() ?? string.Empty));
			return new XmlElmt(
				nodeName,
				attrColl,
				dataColl
			);
		}
		#endregion

		#region IDisposable Implements
		/// <summary>指出是否已經釋放過資源</summary>
		protected bool mDisposed = false;

		/// <summary>釋放此影像工具資源</summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>釋放此影像工具資源之內容</summary>
		protected virtual void Dispose(bool disposing) {
			if (!mDisposed && disposing) {
				mTreeNode = null;
				mTool1 = null;
				mTool2 = null;
				mCmt = null;
				mInputLinkID = null;
			}
		}

		/// <summary>解構子</summary>
		~DistanceJudge() {
			Console.WriteLine("Disposing : " + mID.ToString());
			Dispose(true);
		}
		#endregion
	}

	/// <summary>角度評斷工具。量測兩影像工具之結果夾角，並回報是否合格</summary>
	public class ThetaJudge : IVisionJudgement, IResultOfAngle {

		#region Fields
		private double mTheta = 0;
		private double mTol = 0;
		private TreeNode mTreeNode;
		private IVisionToolPack mTool1;
		private IVisionToolPack mTool2;
		private string mCmt = string.Empty;
		private bool mAsNg = true;
		private double mThetaOfs = 0D;
		private int mTreeNodeLv = -1;
		private int mTreeNodeIdx = -1;
		private long mID = -1;
		private long? mInputLinkID = null;
		#endregion

		#region Properties
		/// <summary>取得此工具的識別碼</summary>
		public long ID { get { return mID; } }
		/// <summary>取得此評斷工具於 <see cref="TreeView"/> 之節點</summary>
		public TreeNode Node { get { return mTreeNode; } }
		/// <summary>取得於 <see cref="TreeView"/> 的節點之層數(從左至右，0 開始)</summary>
		public int NodeLevel { get { return mTreeNodeLv; } }
		/// <summary>取得於 <see cref="TreeView"/> 的節點於同層內的深度(從上至下，0 開始)</summary>
		public int NodeIndex { get { return mTreeNodeIdx; } }
		/// <summary>取得當前量測的數值</summary>
		public double Theta { get { return mTheta; } }
		/// <summary>取得此評斷工具所允許的距離誤差</summary>
		public double Tolerance { get { return mTol; } }
		/// <summary>取得此工具的文字註解</summary>
		public string Comment { get { return mCmt; } }
		/// <summary>取得或設定工具是否被修改</summary>
		public bool IsModified { get; set; } = false;
		/// <summary>取得或設定工具是否已被編譯</summary>
		/// <remarks>(<see langword="true"/>)不需要再重新 Compile CVT  (<see langword="false"/>)需要重新 Compile</remarks>
		public bool IsCompiled { get; set; } = true;
		/// <summary>取得是否需判定為 NG 產品</summary>
		public bool JudgeAsNG { get { return mAsNg; } }
		/// <summary>取得量測後數值所需補償的角度，用於讓向量轉向特定方向。  最終角度 = Theta + ThetaOffset</summary>
		public double ThetaOffset { get { return mThetaOfs; } }
		/// <summary>取得或設定物件，其包含相關資料</summary>
		public object Tag { get; set; } = null;
		/// <summary>取得或設定此影像工具是否作為最後回傳的角度定義</summary>
		public bool IsAngleReturner { get; set; }
		/// <summary>取得 Relatived 的識別碼</summary>
		public long? InputLinkID { get { return mInputLinkID; } }
		/// <summary>取得起點工具</summary>
		public IVisionToolPack Tool1 { get { return mTool1; } }
		/// <summary>取得終點工具</summary>
		public IVisionToolPack Tool2 { get { return mTool2; } }
		/// <summary>取得此工具是否可被複製</summary>
		public bool IsCopyable { get { return true; } }
		#endregion

		#region Constructors
		/// <summary>建構新的角度差評斷工具</summary>
		/// <param name="mainNode">欲存放此工具的父節點</param>
		public ThetaJudge(TreeNode mainNode) {
			mID = DateTime.Now.ToBinary();
			mTreeNode = mainNode.Nodes.Add("Angle Measure");
			mTreeNode.Tag = this;
			mTreeNodeLv = mTreeNode.Level;
			mTreeNodeIdx = mTreeNode.Index;
			IsModified = true;
		}

		/// <summary>複製角度差評斷工具</summary>
		/// <param name="mainNode">欲存放此工具的父節點</param>
		/// <param name="copyTool">欲複製的來源</param>
		public ThetaJudge(TreeNode mainNode, ThetaJudge copyTool) {
			mID = DateTime.Now.ToBinary();
			mTreeNode = mainNode.Nodes.Add("Angle Measure");
			mTreeNode.Tag = this;
			mTreeNodeLv = mTreeNode.Level;
			mTreeNodeIdx = mTreeNode.Index;
			IsModified = true;

			mTheta = copyTool.Theta;
			mTol = copyTool.Tolerance;
			mTool1 = copyTool.Tool1;
			mTool2 = copyTool.Tool2;
			mCmt = copyTool.Comment;
			mAsNg = copyTool.JudgeAsNG;
			mThetaOfs = copyTool.ThetaOffset;
			Tag = copyTool.Tag;
		}

		/// <summary>透過 <see cref="IXmlData"/> 建構角度差評斷工具</summary>
		/// <param name="xmlData">含有工具資料的 XML 節點</param>
		/// <param name="aceSrc">已連線的 ACE Server</param>
		/// <param name="toolColl">含有 <see cref="IVisionToolPack"/> 之集合，供尋找參考</param>
		public ThetaJudge(XmlElmt xmlData, IAceServer aceSrc, IEnumerable<IVisionToolPack> toolColl) {
			/* 拆出 Type */
			string toolType = xmlData.Attribute("Type").Value;
			if (toolType != "ThetaJudge") throw new InvalidCastException(GetMultiLangText("VisToolNotSup"));

			/* 取得 ID */
			mID = long.Parse(xmlData.Attribute("ID").Value);

			/* 樹節點 */
			XmlElmt childData;
			if (xmlData.Element("Node", out childData)) {
				if (!string.IsNullOrEmpty(childData.Value)) {
					mTreeNode = new TreeNode(childData.Value);
					mTreeNode.Tag = this;

					XmlAttr attr;
					if (childData.Attribute("ParentID", out attr))
						mInputLinkID = string.IsNullOrEmpty(attr.Value) ? null : (long?)long.Parse(attr.Value);
					if (childData.Attribute("Level", out attr)) mTreeNodeLv = int.Parse(attr.Value);
					if (childData.Attribute("Index", out attr)) mTreeNodeIdx = int.Parse(attr.Value);
				} else throw new ArgumentNullException("Node", GetMultiLangText("PathNull"));
			}

			/* 抓註解 */
			mCmt = xmlData.Element("Comment")?.Value;

			/* 抓 NG */
			mAsNg = bool.Parse(xmlData.Element("AsNG")?.Value ?? "false");

			/* 抓角度 */
			mTheta = double.Parse(xmlData.Element("Theta")?.Value ?? "0");

			/* 抓角度補償 */
			mThetaOfs = double.Parse(xmlData.Element("ThetaOffset")?.Value ?? "0");

			/* 抓誤差 */
			mTol = double.Parse(xmlData.Element("Tolerance")?.Value ?? "0");

			/* 抓 tool 囉 */
			childData = xmlData.Element("Tool1");
			if (childData != null && !string.IsNullOrEmpty(childData.Value)) {
				long tarID = long.Parse(childData.Value);
				IVisionToolPack tempPack = toolColl.FirstOrDefault(obj => obj.ID == tarID);
				if (tempPack != null) mTool1 = tempPack;
			}

			/* 抓 tool 囉 */
			childData = xmlData.Element("Tool2");
			if (childData != null && !string.IsNullOrEmpty(childData.Value)) {
				long tarID = long.Parse(childData.Value);
				IVisionToolPack tempPack = toolColl.FirstOrDefault(obj => obj.ID == tarID);
				if (tempPack != null) mTool2 = tempPack;
			}

			/* 抓是不是角度回傳工具 */
			IsAngleReturner = bool.Parse(xmlData.Element("AngleReturner")?.Value ?? "false");
		}
		#endregion

		#region Private Function
		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		private string GetMultiLangText(string key) {
			return CtLanguage.GetMultiLangXmlText("Language.xml", key)[key];
		}

		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		private Dictionary<string, string> GetMultiLangText(params string[] key) {
			return CtLanguage.GetMultiLangXmlText("Language.xml", key);
		}

		/// <summary>建立一行新的 <see cref="DataGridViewRow"/></summary>
		/// <param name="dgv">欲顯示此欄位的 <see cref="DataGridView"/></param>
		/// <param name="name">於第一欄顯示屬性名稱之字串</param>
		/// <param name="tip">欲顯示於提示視窗的提示文字</param>
		/// <returns>已建立的欄位</returns>
		private DataGridViewRow CreateNewRow(DataGridView dgv, string name, string tip = "") {
			DataGridViewRow row = new DataGridViewRow();
			row.CreateCells(dgv, name);
			row.Tag = tip;
			return row;
		}

		/// <summary>搜尋 <see cref="TreeView"/> 內的影像工具包</summary>
		/// <param name="node">要開始往下搜尋的 <see cref="TreeView"/> 節點</param>
		/// <param name="nodeColl">集合</param>
		private void SearchVisionToolPack(TreeNode node, ref List<IVisionToolPack> nodeColl) {
			/* 判斷是不是 tool pack，是的話就直接加入即和吧~ */
			if (node.Tag is IVisionToolPack) {
				IVisionToolPack pack = node.Tag as IVisionToolPack;
				//排除 model 和 cvt，這沒辦法當結果的來源 (cvt是因為目前僅允許一份，人為操作當然可以用...)
				if (pack.ToolType != typeof(ILocatorModel) && pack.ToolType != typeof(ICSharpCustomTool)) nodeColl.Add(pack);
			}

			/* 如果還有東西就往下做吧 */
			if (node.Nodes.Count > 0) {
				foreach (TreeNode subNode in node.Nodes) {
					SearchVisionToolPack(subNode, ref nodeColl);
				}
			}
		}

		/// <summary>遞迴尋找 <see cref="TreeNode.Text"/> 為特定的字串</summary>
		/// <param name="node">欲尋找的 <see cref="TreeNode"/></param>
		/// <param name="nodeText">欲尋找的文字</param>
		/// <returns>相符的  TreeNode</returns>
		private TreeNode NodeRecursive(TreeNode node, string nodeText) {
			TreeNode found = null;
			if (node.Text == nodeText) found = node;
			else if (node.Nodes.Count > 0) {
				foreach (TreeNode item in node.Nodes) {
					found = NodeRecursive(item, nodeText);
					if (found != null) break;
				}
			}
			return found;
		}

		/// <summary>搜尋 <see cref="TreeNode"/> 符合特定文字的節點，並回傳其 <see cref="IVisionToolPack"/></summary>
		/// <param name="key">欲搜尋 <see cref="TreeNode"/> 文字</param>
		/// <param name="node">欲搜尋的 <see cref="TreeNode"/></param>
		/// <returns>符合的 <see cref="IVisionToolPack"/></returns>
		private IVisionToolPack SearchVisionToolPack(string key, TreeNode node) {
			TreeNode tempNode = NodeRecursive(node, key);
			IVisionToolPack resultPack = null;
			if (tempNode != null) {
				resultPack = tempNode.Tag as IVisionToolPack;    //如果 tag 不是 visiontoolpack 也沒差，同樣回傳 null
			}   //else 不用做，反正都是回傳 null
			return resultPack;
		}

		private void SetRowVisible(IEnumerable<DataGridViewRow> rows) {
			string propName = string.Empty;
			foreach (DataGridViewRow row in rows) {
				propName = row.Cells[0].Value.ToString();
				switch (propName) {
					case "Angle":
					case "Tolerance":
						row.Visible = mAsNg;
						break;
					default:
						row.Visible = true;
						break;
				}
			}
		}
		#endregion

		#region IVisionJudgement Implements
		/// <summary>指定計算用的第一組影像工具包</summary>
		/// <param name="tool">欲指定的影像工具包</param>
		public void AssignVisionTool_1(IVisionToolPack tool) {
			mTool1 = tool;
		}
		/// <summary>指定計算用的第二組影像工具包</summary>
		/// <param name="tool">欲指定的影像工具包</param>
		public void AssignVisionTool_2(IVisionToolPack tool) {
			mTool2 = tool;
		}

		/// <summary>取得此影像工具之對應的 CVT 程式碼</summary>
		/// <param name="passVar">切換 PASS 或 NG 的區域變數名稱</param>
		/// <returns>CVT 程式碼，一索引對應一行</returns>
		public List<string> GenerateCode(string passVar) {
			List<string> code = new List<string>();
			if (mTool1 == null || mTool2 == null) return code;

			/* 如果是角度回傳者，提供 resultAngle 給回傳工具 */
			code.Add($"/*-- Calculate angle between {mTool1.VariableName} and {mTool2.VariableName} --*/");
			if (IsAngleReturner) code.Add("double resultAngle = 0D;");
			/* 確認 tool 都有結果 */
			code.Add(string.Format("if ({0}.ResultsAvailable && {1}.ResultsAvailable) {{", mTool1.VariableName, mTool2.VariableName));
			code.Add(string.Empty);
			code.Add("\t/* Get two target positions */");
			/* 取得結果 */
			if (mTool1.ReturnRoiCenter && mTool1.Tool is IArcRoiTool) {
				code.Add(
					string.Format(
						"\tVisionTransform pos1 = new VisionTransform({0}.SearchRegion.Center.X, {0}.SearchRegion.Center.Y, {0}.SearchRegion.Rotation);",
						mTool1.VariableName
					)
				);
			} else if (mTool1.ReturnRoiCenter) {
				code.Add($"\tVisionTransform pos1 = {mTool1.VariableName}.Offset;");
			} else {
				code.Add($"\tVisionTransform pos1 = {mTool1.VariableName}.GetTransformResults()[0];");
			}

			if (mTool2.ReturnRoiCenter && mTool2.Tool is IArcRoiTool) {
				code.Add(
					string.Format(
						"\tVisionTransform pos2 = new VisionTransform({0}.SearchRegion.Center.X, {0}.SearchRegion.Center.Y, {0}.SearchRegion.Rotation);",
						mTool2.VariableName
					)
				);
			} else if (mTool2.ReturnRoiCenter) {
				code.Add($"\tVisionTransform pos2 = {mTool2.VariableName}.Offset;");
			} else {
				code.Add($"\tVisionTransform pos2 = {mTool2.VariableName}.GetTransformResults()[0];");
			}

			/* 直接使用 Transform3D 計算平面距離 */
			code.Add(string.Empty);
			code.Add("\t/* Calculate angle of two locations */ ");
			code.Add("\tdouble angle = Math.Atan2(pos2.Y - pos1.Y, pos2.X - pos1.X) / Math.PI * 180;");
			/*if (mThetaOfs > 0) */code.Add(string.Format("\tangle += {0}D;", mThetaOfs));
			code.Add(string.Format("\tTrace.WriteLine(string.Format(\"Angle between \\\"{0}\\\" and \\\"{1}\\\" is {{0:F3}}\", angle));", mTool1.VariableName, mTool2.VariableName));
			if (mAsNg) {
				code.Add(string.Empty);
				code.Add("\t/* Check angle is PASS or NG */");
				code.Add("\tdouble angLbX = (pos1.X + pos2.X) / 2, angLbY = (pos1.Y + pos2.Y) / 2;");
				code.Add("\tVisionTransform angLbLoc = new VisionTransform(angLbX, angLbY + (angle < 45 || angle > 135 ? angle * 0.01 : 0) + 0.2, 0);");
				/* 判斷是否在合格範圍內，合格則畫 PASS 框 */
				code.Add(string.Format("\tif ({0} < angle || angle < {1}) {{", (mTheta + mTol).ToString("F3"), (mTheta - mTol).ToString("F3")));
				code.Add("\t\tcvt.OverlayMarkers.AddLineMarker(pos1, pos2).Color = clrRoiFail;");
				code.Add("\t\tcvt.OverlayMarkers.AddLabelMarker(angLbLoc, angle.ToString(\"F2\") + \"°\").Color = clrRoiFail;");
				code.Add($"\t\t{passVar} = false;");
				/* 如果不合格，畫 NG 框 */
				code.Add("\t} else {");
				code.Add("\t\tMarkerColor angLbClr = (MarkerColor)0x3473B1;");
				code.Add("\t\tcvt.OverlayMarkers.AddLineMarker(pos1, pos2).Color = angLbClr;");
				code.Add("\t\tcvt.OverlayMarkers.AddLabelMarker(angLbLoc, angle.ToString(\"F2\") + \"°\").Color = angLbClr;");
				if (IsAngleReturner) code.Add("\t\tresultAngle = angle;");
				code.Add("\t}");
				code.Add($"}} else {passVar} = false;");
			} else {
				code.Add(string.Empty);
				code.Add("\t/* Measure angle */");
				code.Add("\tdouble angLbX = (pos1.X + pos2.X) / 2, angLbY = (pos1.Y + pos2.Y) / 2;");
				code.Add("\tMarkerColor angLbClr = (MarkerColor)0x3473B1;");
				code.Add("\tVisionTransform angLbLoc = new VisionTransform(angLbX, angLbY + (angle < 45 || angle > 135 ? angle * 0.01 : 0) + 0.2, 0);");
				code.Add("\tcvt.OverlayMarkers.AddLineMarker(pos1, pos2).Color = angLbClr;");
				code.Add("\tcvt.OverlayMarkers.AddLabelMarker(angLbLoc, angle.ToString(\"F2\") + \"°\").Color = angLbClr;");
				if (IsAngleReturner) code.Add("\tresultAngle = angle;");
				code.Add("}");
			}

			code.Add(string.Empty);
			return code;
		}

		/// <summary>更新 <see cref="TreeNode"/> 相關資訊，<see cref="TreeNode.Level"/>、<see cref="TreeNode.Index"/> 等</summary>
		public void UpdateTreeNodeInformation() {
			mTreeNodeIdx = mTreeNode.Index;
			mTreeNodeLv = mTreeNode.Level;
		}

		/// <summary>檢查移除的物件是否與此工具有關聯，如有關聯則取消之</summary>
		/// <param name="tool">移除的物件</param>
		public void ConfirmRemovedLink(IVisionProjectable tool) {
			if (mTool1 != null && mTool1.ID == tool.ID) mTool1 = null;
			if (mTool2 != null && mTool2.ID == tool.ID) mTool2 = null;
		}
		#endregion

		#region IPropertable Implements

		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
			List<PropertyView> propList = new List<PropertyView>();

			Stat stt = Stat.SUCCESS;

			propList.Add(
				new PropertyView(
					langMap["PropThetaNg"],
					AccessLevel.None,
					langMap["TipThetaNg"],
					"NG Judging Enabled",
					mAsNg ? langMap["ValBoolTrue"] : langMap["ValBoolFalse"],
					() => {
						string curStr = mAsNg ? langMap["ValBoolTrue"] : langMap["ValBoolFalse"];
						string ng = string.Empty;
						stt = CtInput.ComboBoxList(out ng, langMap["ThetaNgTitle"], langMap["ThetaNgEnt"], new List<string> { langMap["ValBoolTrue"], langMap["ValBoolFalse"] }, curStr);
						if (stt == Stat.SUCCESS) {
							mAsNg = langMap["ValBoolTrue"].Equals(ng);
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mAsNg ? langMap["ValBoolTrue"] : langMap["ValBoolFalse"];
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropTheta"],
					AccessLevel.None,
					langMap["TipTheta"],
					"Theta / Rotation / Degree",
					mTheta.ToString("F3"),
					() => {
						string disStr = string.Empty;
						stt = CtInput.Text(out disStr, langMap["NormalEditTitle"], langMap["ThetaEnt"], mTheta.ToString("F3"));
						if (stt == Stat.SUCCESS) {
							if (!double.TryParse(disStr, out mTheta)) {
								CtMsgBox.Show(langMap["NormalEditTitle"], langMap["ValEditReal"], MsgBoxBtn.OK, MsgBoxStyle.Error);
							} else {
								this.IsModified = true;
								this.IsCompiled = false;
							}
						}
						return mTheta.ToString("F3");
					},
					() => mAsNg
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropOfs"],
					AccessLevel.None,
					langMap["TipThetaOfs"],
					"Offset",
					mThetaOfs.ToString("F3"),
					() => {
						string ofsStr = string.Empty;
						stt = CtInput.ComboBoxList(out ofsStr, langMap["NormalEditTitle"], langMap["ThetaOfsEnt"], new List<string> { "0", "90", "180", "-90" }, mThetaOfs.ToString(), true);
						if (stt == Stat.SUCCESS) {
							if (!double.TryParse(ofsStr, out mThetaOfs)) {
								CtMsgBox.Show(langMap["NormalEditTitle"], langMap["ValEditReal"], MsgBoxBtn.OK, MsgBoxStyle.Error);
							} else {
								this.IsModified = true;
								this.IsCompiled = false;
							}
						}
						return mTheta.ToString("F3");
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropDistTol"],
					AccessLevel.None,
					langMap["TipDistTol"],
					"Tolerance",
					mTol.ToString("F3"),
					() => {
						string tolStr = string.Empty;
						Dictionary<string, string> tolMsg = GetMultiLangText("NormalEditTitle", "ThetaTolEnt", "ValEditReal");
						stt = CtInput.Text(out tolStr, tolMsg["NormalEditTitle"], tolMsg["ThetaTolEnt"], mTol.ToString("F3"));
						if (stt == Stat.SUCCESS) {
							if (!double.TryParse(tolStr, out mTol)) {
								CtMsgBox.Show(tolMsg["NormalEditTitle"], tolMsg["ValEditReal"], MsgBoxBtn.OK, MsgBoxStyle.Error);
							} else {
								this.IsModified = true;
								this.IsCompiled = false;
							}
						}
						return mTol.ToString("F3");
					},
					() => mAsNg
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropDistTol1"],
					AccessLevel.None,
					langMap["TipDistTol1"],
					"Referenced Tool - 1",
					mTool1?.Node.Text ?? string.Empty,
					() => {
						List<IVisionToolPack> toolList = new List<IVisionToolPack>();
						SearchVisionToolPack(mTreeNode.Parent, ref toolList);
						List<string> chkColl;
						string curTool = mTool1?.Node.Text ?? string.Empty;
						stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcDistEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
						if (stt == Stat.SUCCESS) {
							mTool1 = toolList.Find(tool => tool.Node.Text == chkColl[0]);
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mTool1?.Node.Text ?? string.Empty;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropDistTol2"],
					AccessLevel.None,
					langMap["TipDistTol2"],
					"Referenced Tool - 2",
					mTool2?.Node.Text ?? string.Empty,
					() => {
						List<IVisionToolPack> toolList2 = new List<IVisionToolPack>();
						SearchVisionToolPack(mTreeNode.Parent, ref toolList2);
						List<string> chkColl2;
						string curTool2 = mTool2?.Node.Text ?? string.Empty;
						stt = CtInput.CheckList(out chkColl2, langMap["SlcTool"], langMap["SlcDistEnt"], toolList2.ConvertAll(tool => tool.Node.Text), curTool2, true);
						if (stt == Stat.SUCCESS) {
							mTool2 = toolList2.Find(tool => tool.Node.Text == chkColl2[0]);
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mTool2?.Node.Text ?? string.Empty;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropCmt"],
					AccessLevel.None,
					langMap["TipCmt"],
					"Comments",
					mCmt,
					() => {
						string cmt;
						if (CtInput.Text(out cmt, langMap["Cmt"], langMap["CmtEnt"], mCmt) == Stat.SUCCESS) {
							mCmt = cmt;
							this.IsModified = true;
						}
						return mCmt;
					}
				)
			);

			return propList;
		}
		#endregion

		#region IXmlSavable Implements
		/// <summary>產生物件的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		public XmlElmt CreateXmlData(string nodeName) {
			List<XmlAttr> attrColl = new List<XmlAttr>();
			attrColl.Add(new XmlAttr("Type", "ThetaJudge"));
			attrColl.Add(new XmlAttr("ID", mID.ToString()));
			List<XmlElmt> dataColl = new List<XmlElmt>();
			dataColl.Add(new XmlElmt("Comment", mCmt));
			dataColl.Add(
				new XmlElmt(
					"Node",
					mTreeNode.Text,
					new XmlAttr("ParentID", (mTreeNode.Parent?.Tag as IVisionProjectable)?.ID.ToString() ?? string.Empty),
					new XmlAttr("Level", mTreeNodeLv.ToString()),
					new XmlAttr("Index", mTreeNodeIdx.ToString())
				)
			);
			dataColl.Add(new XmlElmt("AsNG", mAsNg.ToString()));
			dataColl.Add(new XmlElmt("AngleReturner", IsAngleReturner.ToString()));
			dataColl.Add(new XmlElmt("Theta", mTheta.ToString("F3")));
			dataColl.Add(new XmlElmt("ThetaOffset", mThetaOfs.ToString("F3")));
			dataColl.Add(new XmlElmt("Tolerance", mTol.ToString("F3")));
			dataColl.Add(new XmlElmt("Tool1", mTool1?.ID.ToString() ?? string.Empty));
			dataColl.Add(new XmlElmt("Tool2", mTool2?.ID.ToString() ?? string.Empty));
			return new XmlElmt(
				nodeName,
				attrColl,
				dataColl
			);
		}
		#endregion

		#region IDisposable Implements
		/// <summary>指出是否已經釋放過資源</summary>
		protected bool mDisposed = false;

		/// <summary>釋放此影像工具資源</summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>釋放此影像工具資源之內容</summary>
		protected virtual void Dispose(bool disposing) {
			if (!mDisposed && disposing) {
				mTreeNode = null;
				mTool1 = null;
				mTool2 = null;
				mInputLinkID = null;
				mCmt = null;
			}
		}
        
        /// <summary>解構子</summary>
        ~ThetaJudge() {
			Console.WriteLine("Disposing : " + mID.ToString());
			Dispose(true);
		}
        #endregion

        ///<summary><see cref="ThetaJudge"/>不提供該方法</summary>
        public void GenerateAngleCVT(List<string> code) {
            throw new NotImplementedException();
        }
    }

	/// <summary>旋轉單一影像結果之角度</summary>
	public class ThetaWhirling : IVisionJudgement, IResultOfAngle, IDynamicAngle {

		#region Fields
		private TreeNode mTreeNode;
		private IVisionToolPack mTool;
		private string mCmt = string.Empty;
		private double mThetaOfs = 0D;
		private bool mFixed = false;
		private int mTreeNodeLv = -1;
		private int mTreeNodeIdx = -1;
		private long mID = -1;
		private long? mInputLinkID = null;
		#endregion

		#region Properties
		/// <summary>取得此工具的識別碼</summary>
		public long ID { get { return mID; } }
		/// <summary>取得此評斷工具於 <see cref="TreeView"/> 之節點</summary>
		public TreeNode Node { get { return mTreeNode; } }
		/// <summary>取得於 <see cref="TreeView"/> 的節點之層數(從左至右，0 開始)</summary>
		public int NodeLevel { get { return mTreeNodeLv; } }
		/// <summary>取得於 <see cref="TreeView"/> 的節點於同層內的深度(從上至下，0 開始)</summary>
		public int NodeIndex { get { return mTreeNodeIdx; } }
		/// <summary>取得此工具的文字註解</summary>
		public string Comment { get { return mCmt; } }
		/// <summary>取得或設定工具是否被修改</summary>
		public bool IsModified { get; set; } = false;
		/// <summary>取得或設定工具是否已被編譯</summary>
		/// <remarks>(<see langword="true"/>)不需要再重新 Compile CVT  (<see langword="false"/>)需要重新 Compile</remarks>
		public bool IsCompiled { get; set; } = true;
		/// <summary>取得量測後數值所需補償的角度，用於讓向量轉向特定方向。  最終角度 = Theta + ThetaOffset</summary>
		public double ThetaOffset { get { return mThetaOfs; } }
		/// <summary>取得或設定物件，其包含相關資料</summary>
		public object Tag { get; set; } = null;
		/// <summary>取得或設定此影像工具是否作為最後回傳的角度定義</summary>
		public bool IsAngleReturner { get; set; }
		/// <summary>取得工具是否為固定角度而非進行角度補償</summary>
		public bool FixedAngle { get { return mFixed; } }
		/// <summary>取得工具是否需要回傳動態結果</summary>
		/// <remarks>需要建立完整的程式，但卻不是 IVisionResult 的角度來源，表示要把某個 Tool 的所有結果給旋轉</remarks>
		public bool DynamicResultable { get { return !IsAngleReturner && mTool != null; } }
		/// <summary>取得參考的影像工具是否存在</summary>
		public bool IsToolExist { get { return mTool != null; } }
		/// <summary>取得 Relatived 的識別碼</summary>
		public long? InputLinkID { get { return mInputLinkID; } }
		/// <summary>取得工具</summary>
		public IVisionToolPack Tool { get { return mTool; } }
		/// <summary>取得此工具是否可被複製</summary>
		public bool IsCopyable { get { return true; } }
		#endregion

		#region Constructors
		/// <summary>建構新的角度旋轉工具</summary>
		/// <param name="mainNode">欲存放此工具的父節點</param>
		public ThetaWhirling(TreeNode mainNode) {
			mID = DateTime.Now.ToBinary();
			mTreeNode = mainNode.Nodes.Add("Angle Whirling");
			mTreeNode.Tag = this;
			mTreeNodeLv = mTreeNode.Level;
			mTreeNodeIdx = mTreeNode.Index;
			IsModified = true;
		}

		/// <summary>複製角度旋轉工具</summary>
		/// <param name="mainNode">欲存放此工具的父節點</param>
		/// <param name="copyTool">欲複製的來源</param>
		public ThetaWhirling(TreeNode mainNode, ThetaWhirling copyTool) {
			mID = DateTime.Now.ToBinary();
			mTreeNode = mainNode.Nodes.Add("Angle Whirling");
			mTreeNode.Tag = this;
			mTreeNodeLv = mTreeNode.Level;
			mTreeNodeIdx = mTreeNode.Index;
			IsModified = true;

			mTool = copyTool.Tool;
			mCmt = copyTool.Comment;
			mThetaOfs = copyTool.ThetaOffset;
			mFixed = copyTool.FixedAngle;
			Tag = copyTool.Tag;
		}

		/// <summary>透過 <see cref="IXmlData"/> 建構角度旋轉工具</summary>
		/// <param name="xmlData">含有工具資料的 XML 節點</param>
		/// <param name="aceSrc">已連線的 ACE Server</param>
		/// <param name="toolColl">含有 <see cref="IVisionToolPack"/> 之集合，供尋找參考</param>
		public ThetaWhirling(XmlElmt xmlData, IAceServer aceSrc, IEnumerable<IVisionToolPack> toolColl) {
			/* 拆出 Type */
			string toolType = xmlData.Attribute("Type").Value;
			if (toolType != "ThetaWhirling") throw new InvalidCastException(GetMultiLangText("VisToolNotSup"));

			/* 取得 ID */
			mID = long.Parse(xmlData.Attribute("ID").Value);

			/* 樹節點 */
			XmlElmt childData;
			if (xmlData.Element("Node", out childData)) {
				if (!string.IsNullOrEmpty(childData.Value)) {
					mTreeNode = new TreeNode(childData.Value);
					mTreeNode.Tag = this;

					XmlAttr attr;
					if (childData.Attribute("ParentID", out attr))
						mInputLinkID = string.IsNullOrEmpty(attr.Value) ? null : (long?)long.Parse(attr.Value);
					if (childData.Attribute("Level", out attr)) mTreeNodeLv = int.Parse(attr.Value);
					if (childData.Attribute("Index", out attr)) mTreeNodeIdx = int.Parse(attr.Value);
				} else throw new ArgumentNullException("Node", GetMultiLangText("PathNull"));
			}

			/* 抓註解 */
			mCmt = xmlData.Element("Comment")?.Value;

			/* 抓角度補償 */
			mThetaOfs = double.Parse(xmlData.Element("ThetaOffset")?.Value ?? "0");

			/* 抓 tool 囉 */
			childData = xmlData.Element("Tool");
			if (childData != null && !string.IsNullOrEmpty(childData.Value)) {
				long tarID = long.Parse(childData.Value);
				IVisionToolPack tempPack = toolColl.FirstOrDefault(obj => obj.ID == tarID);
				if (tempPack != null) mTool = tempPack;
			}

			/* 抓是不是角度回傳工具 */
			IsAngleReturner = bool.Parse(xmlData.Element("AngleReturner")?.Value ?? "false");

			/* 抓是不是角度回傳工具 */
			mFixed = bool.Parse(xmlData.Element("Fixed")?.Value ?? "false");
		}
		#endregion

		#region Private Function
		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		private string GetMultiLangText(string key) {
			return CtLanguage.GetMultiLangXmlText("Language.xml", key)[key];
		}

		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		private Dictionary<string, string> GetMultiLangText(params string[] key) {
			return CtLanguage.GetMultiLangXmlText("Language.xml", key);
		}

		/// <summary>建立一行新的 <see cref="DataGridViewRow"/></summary>
		/// <param name="dgv">欲顯示此欄位的 <see cref="DataGridView"/></param>
		/// <param name="name">於第一欄顯示屬性名稱之字串</param>
		/// <param name="tip">欲顯示於提示視窗的提示文字</param>
		/// <returns>已建立的欄位</returns>
		private DataGridViewRow CreateNewRow(DataGridView dgv, string name, string tip = "") {
			DataGridViewRow row = new DataGridViewRow();
			row.CreateCells(dgv, name);
			row.Tag = tip;
			return row;
		}

		/// <summary>搜尋 <see cref="TreeView"/> 內的影像工具包</summary>
		/// <param name="node">要開始往下搜尋的 <see cref="TreeView"/> 節點</param>
		/// <param name="nodeColl">集合</param>
		private void SearchVisionToolPack(TreeNode node, ref List<IVisionToolPack> nodeColl) {
			/* 判斷是不是 tool pack，是的話就直接加入即和吧~ */
			if (node.Tag is IVisionToolPack) {
				IVisionToolPack pack = node.Tag as IVisionToolPack;
				//排除 model 和 cvt，這沒辦法當結果的來源 (cvt是因為目前僅允許一份，人為操作當然可以用...)
				if (pack.ToolType != typeof(ILocatorModel) && pack.ToolType != typeof(ICSharpCustomTool)) nodeColl.Add(pack);
			}

			/* 如果還有東西就往下做吧 */
			if (node.Nodes.Count > 0) {
				foreach (TreeNode subNode in node.Nodes) {
					SearchVisionToolPack(subNode, ref nodeColl);
				}
			}
		}

		/// <summary>遞迴尋找 <see cref="TreeNode.Text"/> 為特定的字串</summary>
		/// <param name="node">欲尋找的 <see cref="TreeNode"/></param>
		/// <param name="nodeText">欲尋找的文字</param>
		/// <returns>相符的 TreeNode</returns>
		private TreeNode NodeRecursive(TreeNode node, string nodeText) {
			TreeNode found = null;
			if (node.Text == nodeText) found = node;
			else if (node.Nodes.Count > 0) {
				foreach (TreeNode item in node.Nodes) {
					found = NodeRecursive(item, nodeText);
					if (found != null) break;
				}
			}
			return found;
		}

		/// <summary>搜尋 <see cref="TreeNode"/> 符合特定文字的節點，並回傳其 <see cref="IVisionToolPack"/></summary>
		/// <param name="key">欲搜尋 <see cref="TreeNode"/> 文字</param>
		/// <param name="node">欲搜尋的 <see cref="TreeNode"/></param>
		/// <returns>符合的 <see cref="IVisionToolPack"/></returns>
		private IVisionToolPack SearchVisionToolPack(string key, TreeNode node) {
			TreeNode tempNode = NodeRecursive(node, key);
			IVisionToolPack resultPack = null;
			if (tempNode != null) {
				resultPack = tempNode.Tag as IVisionToolPack;    //如果 tag 不是 visiontoolpack 也沒差，同樣回傳 null
			}   //else 不用做，反正都是回傳 null
			return resultPack;
		}

		#endregion

		#region IVisionJudgement Implements
		/// <summary>指定計算用的第一組影像工具包</summary>
		/// <param name="tool">欲指定的影像工具包</param>
		public void AssignVisionTool(IVisionToolPack tool) {
			mTool = tool;
		}

		/// <summary>取得此影像工具之對應的 CVT 程式碼</summary>
		/// <param name="passVar">切換 PASS 或 NG 的區域變數名稱</param>
		/// <returns>CVT 程式碼，一索引對應一行</returns>
		public List<string> GenerateCode(string passVar) {
			List<string> code = new List<string>();

			if (mTool != null) code.Add($"/*-- Whirling the results of {mTool.VariableName} --*/");
			else code.Add($"/*-- Whirling the results --*/");

			/*-- 是角度回傳者，且有設定要旋轉的工具、非固定角，取得 mTool 的角度然後設定至 resultAngle 供 ResultTool 使用 --*/
			if (IsAngleReturner && mTool != null && !mFixed) {
				/* 如果是角度回傳者，提供 resultAngle 給回傳工具 */
				code.Add("double resultAngle = 0D;");
				/* 確認 tool 都有結果 */
				code.Add(string.Format("if ({0}.ResultsAvailable) {{", mTool.VariableName));
				/* 取得結果 */
				code.Add("\t/* Get vision result from specified tool */");
				if (mTool.ReturnRoiCenter && mTool.Tool is IArcRoiTool) {
					code.Add(
						string.Format(
							"\tVisionTransform pos = new VisionTransform({0}.SearchRegion.Center.X, {0}.SearchRegion.Center.Y, {0}.SearchRegion.Rotation);",
							mTool.VariableName
						)
					);
				} else if (mTool.ReturnRoiCenter) {
					code.Add($"\tVisionTransform pos = {mTool.VariableName}.Offset;");
				} else {
                    (mTool as IResultOfAngle).GenerateAngleCVT(code);
					//code.Add($"\tVisionTransform pos = {mTool.VariableName}.GetTransformResults()[0];");
				}
				/* 直接使用 Transform3D 計算平面距離 */
				code.Add(string.Empty);
				code.Add("\t/* Whirling angle */");
				code.Add(string.Format("\tresultAngle = pos.Degrees + {0}D;", mThetaOfs));
				code.Add("\tTrace.WriteLine(string.Format(\"Rotated angle from {0:F3} to {1:F3}\", pos.Degrees, resultAngle));");
				/* 畫出角度數值 */
				code.Add(string.Empty);
				code.Add("\t/* Draw whirled angle */");
				code.Add("\tMarkerColor angClr = (MarkerColor)0xC0A75A;");
				code.Add("\tVisionTransform angLbLoc = new VisionTransform(pos.X + 0.5, pos.Y + (resultAngle < 45 || resultAngle > 135 ? resultAngle * 0.01 : 0) + 0.4, 0);");
				code.Add("\tcvt.OverlayMarkers.AddAxesMarker(pos, 1.5, 1.5).Color = angClr;");
				code.Add("\tcvt.OverlayMarkers.AddLabelMarker(angLbLoc, resultAngle.ToString(\"F2\") + \"°\").Color = angClr;");
				code.Add("}");
			} else if (!IsAngleReturner && mTool != null) {
				code.Add("/* Rotate results from specified tool */");
				/*-- 如果不是回傳者，但卻有設定工具，表示要旋轉工具的結果... 旋轉後丟到 dynamicResults --*/
				code.Add("VisionTransform[] dynamicResults = null;");
				if (mTool.ReturnRoiCenter && mTool.Tool is IArcRoiTool) {
					code.Add(
						string.Format(
							"\tVisionTransform whirlVT = new VisionTransform({0}.SearchRegion.Center.X, {0}.SearchRegion.Center.Y, {0}.SearchRegion.Rotation);",
							mTool.VariableName
						)
					);
					if (mFixed) code.Add($"\twhirlVT.Degrees = {mThetaOfs:F3};");
					else code.Add($"\twhirlVT.Degrees += {mThetaOfs:F3};");
					code.Add("\tdynamicResults = new VisionTransform[] { whirlVT };");
				} else if (mTool.ReturnRoiCenter) {
					code.Add($"\tVisionTransform whirlVT = {mTool.VariableName}.Offset;");
					if (mFixed) code.Add($"\twhirlVT.Degrees = {mThetaOfs:F3};");
					else code.Add($"\twhirlVT.Degrees += {mThetaOfs:F3};");
					code.Add("\tdynamicResults = new VisionTransform[] { whirlVT };");
				} else {
					code.Add(string.Format("if ({0}.ResultsAvailable) {{", mTool.VariableName));
					/* 取得所有 Transform */
					code.Add(string.Format("\tdynamicResults = {0}.GetTransformResults();", mTool.VariableName));
					/* 用 foreach 去改角度 */
					code.Add("\tforeach(var whirlVT in dynamicResults) {");
					if (mFixed) code.Add(string.Format("\t\twhirlVT.Degrees = {0:F3};", mThetaOfs));
					else code.Add(string.Format("\t\twhirlVT.Degrees += {0:F3};", mThetaOfs));
					code.Add("\t}");
					code.Add("}");
				}

				/* 畫出角度數值 */
				code.Add(string.Empty);
				code.Add("/* Draw result axes */");
				code.Add("MarkerColor angClr = (MarkerColor)0xC0A75A;");
				code.Add("foreach(var whirlVT in dynamicResults) {");
				code.Add("\tVisionTransform angLbLoc = new VisionTransform(whirlVT.X + 0.5, whirlVT.Y + (whirlVT.Degrees < 45 || whirlVT.Degrees > 135 ? whirlVT.Degrees * 0.01 : 0) + 0.4, 0);");
				code.Add("\tcvt.OverlayMarkers.AddAxesMarker(whirlVT, 1.5, 1.5).Color = angClr;");
				code.Add("\tcvt.OverlayMarkers.AddLabelMarker(angLbLoc, whirlVT.Degrees.ToString(\"F2\") + \"°\").Color = angClr;");
				code.Add("}");
			} else if (IsAngleReturner) {
				code.Add("/* Provide whirling angle for result tools */");
				/* 如果有設為回傳者，但沒有設定 mTool，表示要旋轉 ResultTool。或為回傳者，但不是固定角。不論是啥，都是直接印出角度即可 */
				code.Add(string.Format("double resultAngle = {0}D;", mThetaOfs));
			}

			code.Add(string.Empty);
			return code;
		}

		/// <summary>更新 <see cref="TreeNode"/> 相關資訊，<see cref="TreeNode.Level"/>、<see cref="TreeNode.Index"/> 等</summary>
		public void UpdateTreeNodeInformation() {
			mTreeNodeIdx = mTreeNode.Index;
			mTreeNodeLv = mTreeNode.Level;
		}

		/// <summary>檢查移除的物件是否與此工具有關聯，如有關聯則取消之</summary>
		/// <param name="tool">移除的物件</param>
		public void ConfirmRemovedLink(IVisionProjectable tool) {
			if (mTool != null && mTool.ID == tool.ID) mTool = null;
		}
		#endregion

		#region IPropertable Implements

		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
			List<PropertyView> propList = new List<PropertyView>();

			Stat stt = Stat.SUCCESS;

			propList.Add(
				new PropertyView(
					langMap["PropWhrilTol"],
					AccessLevel.None,
					langMap["TipWhrilTol"],
					"Referenced Tool",
					mTool?.Node.Text ?? string.Empty,
					() => {
						List<IVisionToolPack> toolList = new List<IVisionToolPack>();
						SearchVisionToolPack(mTreeNode.Parent, ref toolList);
						List<string> chkColl;
						string curTool = mTool?.Node.Text ?? string.Empty;
						stt = CtInput.CheckList(out chkColl, langMap["SlcTool"], langMap["SlcDistEnt"], toolList.ConvertAll(tool => tool.Node.Text), curTool, true);
						if (stt == Stat.SUCCESS) {
							if (chkColl.Count > 0) {
								mTool = toolList.Find(tool => tool.Node.Text == chkColl[0]);
							} else mTool = null;
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mTool?.Node.Text ?? string.Empty;
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropThetaFix"],
					AccessLevel.None,
					langMap["TipThetaFix"],
					"Fixed Angle",
					mFixed ? langMap["ValBoolTrue"] : langMap["ValBoolFalse"],
					() => {
						string curStr = mFixed ? langMap["ValBoolTrue"] : langMap["ValBoolFalse"];
						string inVal;
						if (CtInput.ComboBoxList(out inVal, langMap["NormalEditTitle"], langMap["NormalEditDesc"], new List<string> { langMap["ValBoolTrue"], langMap["ValBoolFalse"] }, curStr) == Stat.SUCCESS) {
							mFixed = langMap["ValBoolTrue"].Equals(inVal);
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mFixed ? langMap["ValBoolTrue"] : langMap["ValBoolFalse"];
					}
				)
			);

			propList.Add(
				new PropertyView(
                    langMap["PropAngOfs"],
                    AccessLevel.None,
                    langMap["TipThetaOfs"],
					"Offset",
					mThetaOfs.ToString("F3"),
					() => {
						string ofsStr = string.Empty;
						stt = CtInput.ComboBoxList(out ofsStr, langMap["NormalEditTitle"], langMap["ThetaOfsEnt"], new List<string> { "0", "90", "180", "-90" }, mThetaOfs.ToString(), true);
						if (stt == Stat.SUCCESS) {
							if (!double.TryParse(ofsStr, out mThetaOfs)) {
								CtMsgBox.Show(langMap["NormalEditTitle"], langMap["ValEditReal"], MsgBoxBtn.OK, MsgBoxStyle.Error);
							}
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mThetaOfs.ToString("F3");
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropCmt"],
					AccessLevel.None,
					langMap["TipCmt"],
					"Comments",
					mCmt,
					() => {
						string cmt;
						if (CtInput.Text(out cmt, langMap["Cmt"], langMap["CmtEnt"], mCmt) == Stat.SUCCESS) {
							mCmt = cmt;
							this.IsModified = true;
						}
						return mCmt;
					}
				)
			);

			return propList;
		}

		#endregion

		#region IXmlSavable Implements
		/// <summary>產生物件的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		public XmlElmt CreateXmlData(string nodeName) {
			List<XmlAttr> attrColl = new List<XmlAttr>();
			attrColl.Add(new XmlAttr("Type", "ThetaWhirling"));
			attrColl.Add(new XmlAttr("ID", mID.ToString()));
			List<XmlElmt> dataColl = new List<XmlElmt>();
			dataColl.Add(new XmlElmt("Comment", mCmt));
			dataColl.Add(
				new XmlElmt(
					"Node",
					mTreeNode.Text,
					new XmlAttr("ParentID", (mTreeNode.Parent?.Tag as IVisionProjectable)?.ID.ToString() ?? string.Empty),
					new XmlAttr("Level", mTreeNodeLv.ToString()),
					new XmlAttr("Index", mTreeNodeIdx.ToString())
				)
			);
			dataColl.Add(new XmlElmt("AngleReturner", IsAngleReturner.ToString()));
			dataColl.Add(new XmlElmt("ThetaOffset", mThetaOfs.ToString("F3")));
			dataColl.Add(new XmlElmt("Tool", mTool?.ID.ToString() ?? string.Empty));
			dataColl.Add(new XmlElmt("Fixed", mFixed.ToString()));
			return new XmlElmt(
				nodeName,
				attrColl,
				dataColl
			);
		}
		#endregion

		#region IDisposable Implements
		/// <summary>指出是否已經釋放過資源</summary>
		protected bool mDisposed = false;

		/// <summary>釋放此影像工具資源</summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>釋放此影像工具資源之內容</summary>
		protected virtual void Dispose(bool disposing) {
			if (!mDisposed && disposing) {
				mTreeNode = null;
				mTool = null;
				mCmt = null;
				mInputLinkID = null;
			}
		}

		/// <summary>解構子</summary>
		~ThetaWhirling() {
			Console.WriteLine("Disposing : " + mID.ToString());
			Dispose(true);
		}
        #endregion

        ///<summary><see cref="ThetaWhirling"/>不提供該方法</summary>
        public void GenerateAngleCVT(List<string> code) {
            throw new NotImplementedException();
        }
    }

	#endregion

	#region Declaration - Utility Classes

	/// <summary>燈光控制，調光器參數與切換</summary>
	public class LightCtrlPack : IPeripheryUtility {

		#region Fields
		/// <summary>於 <see cref="TreeView"/> 之節點</summary>
		private TreeNode mNode;
		/// <summary>註解</summary>
		private string mCmt = string.Empty;
		/// <summary>相關資料</summary>
		private object mTag;
		/// <summary>調光器數值</summary>
		private List<DimmerPack> mDimColl = new List<DimmerPack>();
		/// <summary>於 <see cref="TreeView"/> 的節點之層數(從左至右，0 開始)</summary>
		private int mNodeLv = -1;
		/// <summary>於 <see cref="TreeView"/> 的節點於同層內的深度(從上至下，0 開始)</summary>
		private int mNodeIdx = -1;
		/// <summary>識別碼</summary>
		private long mID = -1;
		/// <summary>Parent Node 的識別碼</summary>
		private long? mInputLinkID = null;
		#endregion

		#region Properties
		/// <summary>取得此工具的識別碼</summary>
		public long ID { get { return mID; } }
		/// <summary>取得或設定物件是否已被修改</summary>
		public bool IsModified { get; set; } = false;
		/// <summary>取得或設定物件是否已被編譯過</summary>
		/// <remarks>(<see langword="true"/>)不需要再重新 Compile CVT  (<see langword="false"/>)需要重新 Compile</remarks>
		public bool IsCompiled { get; set; } = false;
		/// <summary>取得此工具的 <see cref="TreeView"/> 節點</summary>
		public TreeNode Node { get { return mNode; } }
		/// <summary>取得於 <see cref="TreeView"/> 的節點之層數(從左至右，0 開始)</summary>
		public int NodeLevel { get { return mNodeLv; } }
		/// <summary>取得於 <see cref="TreeView"/> 的節點於同層內的深度(從上至下，0 開始)</summary>
		public int NodeIndex { get { return mNodeIdx; } }
		/// <summary>取得此工具的註解</summary>
		public string Comment { get { return mCmt; } }
		/// <summary>取得或設定物件，其包含相關資料</summary>
		public object Tag { get { return mTag; } set { mTag = value; } }
		/// <summary>取得 Relatived 的識別碼</summary>
		public long? InputLinkID { get { return mInputLinkID; } }
		/// <summary>取得當前的調光數值</summary>
		public List<DimmerPack> Dimmers { get { return mDimColl.ConvertAll(dim => dim.Clone()); } }
		/// <summary>取得此工具是否可被複製</summary>
		public bool IsCopyable { get { return true; } }
		#endregion

		#region Constructors
		/// <summary>建構新的調光器調整工具</summary>
		/// <param name="mainNode">欲存放此工具的父節點</param>
		/// <param name="dimColl">所有的調光器模組</param>
		public LightCtrlPack(TreeNode mainNode, IEnumerable<DimmerPack> dimColl = null) {
			mID = DateTime.Now.ToBinary();
			mNode = mainNode.Nodes.Add("Light Control");
			mDimColl = dimColl.Select(dim => dim.Clone()).ToList();
			mNode.Tag = this;
			mNodeLv = mNode.Level;
			mNodeIdx = mNode.Index;
			IsModified = true;
		}

		/// <summary>複製調光器調整工具</summary>
		/// <param name="mainNode">欲存放此工具的父節點</param>
		/// <param name="copyPack">欲複製的來源</param>
		public LightCtrlPack(TreeNode mainNode, LightCtrlPack copyPack) {
			mID = DateTime.Now.ToBinary();
			mNode = mainNode.Nodes.Add("Light Control");
			mNode.Tag = this;
			mNodeLv = mNode.Level;
			mNodeIdx = mNode.Index;
			IsModified = true;

			mCmt = copyPack.Comment;
			mDimColl.AddRange(copyPack.Dimmers);
			mTag = copyPack.Tag;
		}

		/// <summary>透過 <see cref="IXmlData"/> 建構調光器調整工具</summary>
		/// <param name="xmlData">含有工具資料的 XML 節點</param>
		/// <param name="dimColl">所有的調光器模組</param>
		public LightCtrlPack(XmlElmt xmlData, IEnumerable<DimmerPack> dimColl = null) {
			/* 拆出 Type */
			string toolType = xmlData.Attribute("Type").Value;
			if (toolType != "LightCtrl") throw new InvalidCastException(GetMultiLangText("VisToolNotSup"));

			/* 取得 ID */
			mID = long.Parse(xmlData.Attribute("ID").Value);

			/* 樹節點 */
			XmlElmt childData;
			if (xmlData.Element("Node", out childData)) {
				if (!string.IsNullOrEmpty(childData.Value)) {
					mNode = new TreeNode(childData.Value);
					mNode.Tag = this;

					XmlAttr attr;
					if (childData.Attribute("ParentID", out attr))
						mInputLinkID = string.IsNullOrEmpty(attr.Value) ? null : (long?)long.Parse(attr.Value);
					if (childData.Attribute("Level", out attr)) mNodeLv = int.Parse(attr.Value);
					if (childData.Attribute("Index", out attr)) mNodeIdx = int.Parse(attr.Value);
				} else throw new ArgumentNullException("Node", GetMultiLangText("PathNull"));
			}

			/* 抓註解 */
			mCmt = xmlData.Element("Comment").Value;

			/* 調光器 */
			if (xmlData.Element("Lights", out childData) && dimColl != null) {
				foreach (var dim in dimColl) {
					DimmerPack pack = dim.Clone();
					pack.ResetChannelValue();
					mDimColl.Add(pack);
				}

				childData.Elements().ForEach(
					ixml => {
						string comPort = ixml.Attribute("Port").Value;

						XmlAttr attr = ixml.Attribute("Channel");
						Channels ch = (Channels)Enum.Parse(typeof(Channels), attr.Value.Replace("_", string.Empty), true);

						DimmerPack pack = mDimColl.Find(val => val.Port == comPort);
						if (pack != null)
							pack.SetChannelValue(ch, int.Parse(ixml.Value));
					}
				);
			}
		}
		#endregion

		#region Private Utilities
		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		private string GetMultiLangText(string key) {
			return CtLanguage.GetMultiLangXmlText("Language.xml", key)[key];
		}

		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		private Dictionary<string, string> GetMultiLangText(params string[] key) {
			return CtLanguage.GetMultiLangXmlText("Language.xml", key);
		}

		/// <summary>建立一行新的 <see cref="DataGridViewRow"/></summary>
		/// <param name="dgv">欲顯示此欄位的 <see cref="DataGridView"/></param>
		/// <param name="name">於第一欄顯示屬性名稱之字串</param>
		/// <param name="tip">欲顯示於提示視窗的提示文字</param>
		/// <returns>已建立的欄位</returns>
		private DataGridViewRow CreateNewRow(DataGridView dgv, string name, string tip = "") {
			DataGridViewRow row = new DataGridViewRow();
			row.CreateCells(dgv, name);
			row.Tag = tip;
			return row;
		}
		#endregion

		#region Public Operations
		/// <summary>更新調光器選項</summary>
		/// <param name="dimColl">調光器集合</param>
		public void UpdateDimmer(List<DimmerPack> dimColl) {
			List<DimmerPack> copied = new List<DimmerPack>();
			foreach (DimmerPack pack in dimColl) {
				DimmerPack temp = pack.Clone();
				DimmerPack oriPack = mDimColl.Find(dim => dim.Brand == pack.Brand && dim.Name == pack.Name && dim.Port == pack.Port);
				if (oriPack != null) temp.OverwriteChannelValue(oriPack.Channels);
				copied.Add(temp);
			}
			mDimColl.Clear();
			mDimColl.AddRange(copied);
		}

		/// <summary>嘗試切換至當前設定的調光器數值</summary>
		public void TrySetDimmer() {
			if (mDimColl.Count > 0) {
				mDimColl.ForEach(dim => dim.TrySwitchLight());
			}
		}
		#endregion

		#region IPropertable Implements

		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
			List<PropertyView> propList = new List<PropertyView>();

			List<int> dimValColl = new List<int>();
			foreach (var pack in mDimColl) {
				List<DimmerChannel> valColl = pack.Channels.FindAll(val => val.CurrentValue > -1);
				dimValColl.AddRange(valColl.ConvertAll(val => val.CurrentValue));
			}
			var dimStr = dimValColl.Count > 0 ? string.Join(", ", dimValColl) : "-1";

			propList.Add(
				new PropertyView(
					langMap["PropDim"],
					AccessLevel.None,
					langMap["TipDim"],
					"Dimmer Values",
					dimStr,
					() => {
						UILanguage lang = CtLanguage.GetUiLangByCult();
						using (CtAceLightSetting frm = new CtAceLightSetting(lang, mDimColl)) {
							frm.ShowDialog();
							this.IsModified = true;
							this.IsCompiled = false;
						}

						List<int> retVal = new List<int>();
						foreach (var pack in mDimColl) {
							List<DimmerChannel> valColl = pack.Channels.FindAll(val => val.CurrentValue > -1);
							retVal.AddRange(valColl.ConvertAll(val => val.CurrentValue));
						}
						return retVal.Count > 0 ? string.Join(", ", retVal) : "-1";
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropCmt"],
					AccessLevel.None,
					langMap["TipCmt"],
					"Comments",
					mCmt,
					() => {
						string cmt;
						if (CtInput.Text(out cmt, langMap["Cmt"], langMap["CmtEnt"], mCmt) == Stat.SUCCESS) {
							mCmt = cmt;
							this.IsModified = true;
						}
						return mCmt;
					}
				)
			);

			return propList;
		}
		#endregion

		#region IXmlSavable Implements
		/// <summary>產生物件的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		public XmlElmt CreateXmlData(string nodeName) {
			List<XmlAttr> attrColl = new List<XmlAttr>();
			attrColl.Add(new XmlAttr("Type", "LightCtrl"));
			attrColl.Add(new XmlAttr("ID", mID.ToString()));

			List<XmlElmt> dataColl = new List<XmlElmt>();
			dataColl.Add(new XmlElmt("Comment", mCmt));
			dataColl.Add(
				new XmlElmt(
					"Node",
					mNode.Text,
					new XmlAttr("ParentID", (mNode.Parent?.Tag as IVisionProjectable)?.ID.ToString() ?? string.Empty),
					new XmlAttr("Level", mNodeLv.ToString()),
					new XmlAttr("Index", mNodeIdx.ToString())
				)
			);

			int idx = 0;
			List<XmlElmt> dimColl = new List<XmlElmt>();
			foreach (var pack in mDimColl) {
				List<DimmerChannel> valuedColl = pack.Channels.FindAll(val => val.CurrentValue > -1);
				dimColl.AddRange(valuedColl.ConvertAll(val => val.CreateValueXml(string.Format("Dimmer_{0:D2}", idx++), pack.Name)));
			}

			dataColl.Add(new XmlElmt("Lights", string.Empty, dimColl));

			return new XmlElmt(
				nodeName,
				attrColl,
				dataColl
			);
		}

		#endregion

		#region IPeripheryUtility Implements
		/// <summary>產生可適用於 <see cref="ICvtExecutor"/> 內的建構程式碼</summary>
		/// <returns>建構程式</returns>
		public List<string> GenerateExecutionConstruct() {
			List<DimmerPack> pack = mDimColl.FindAll(pair => pair.Channels.Exists(ch => ch.CurrentValue > -1));
			return pack.ConvertAll(dim => dim.CreateExecObjInsStr());
		}

		/// <summary>更新 <see cref="TreeNode"/> 相關資訊，<see cref="TreeNode.Level"/>、<see cref="TreeNode.Index"/> 等</summary>
		public void UpdateTreeNodeInformation() {
			mNodeIdx = mNode.Index;
			mNodeLv = mNode.Level;
		}

		/// <summary>檢查移除的物件是否與此工具有關聯，如有關聯則取消之</summary>
		/// <param name="tool">移除的物件</param>
		public void ConfirmRemovedLink(IVisionProjectable tool) {

		}
		#endregion

		#region IDisposable Implements
		/// <summary>指出是否已經釋放過資源</summary>
		protected bool mDisposed = false;

		/// <summary>釋放此影像工具資源</summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>釋放此影像工具資源之內容</summary>
		protected virtual void Dispose(bool disposing) {
			if (!mDisposed && disposing) {
				mNode = null;
				mCmt = null;
				mTag = null;
				mDimColl.Clear();
				mDimColl = null;
				mInputLinkID = null;
			}
		}

		/// <summary>解構子</summary>
		~LightCtrlPack() {
			Console.WriteLine("Disposing : " + mID.ToString());
			Dispose(true);
		}
		#endregion
	}

	/// <summary>攝影機參數切換</summary>
	public class CamParaPack : IPeripheryUtility {
		#region Fields
		/// <summary>於 <see cref="TreeView"/> 之節點</summary>
		private TreeNode mNode;
		/// <summary>註解</summary>
		private string mCmt = string.Empty;
		/// <summary>相關資料</summary>
		private object mTag;
		/// <summary>目標曝光值</summary>
		private int mExp = -1;
		/// <summary>目標增益值</summary>
		private int mGain = -1;
		/// <summary>於 <see cref="TreeView"/> 的節點之層數(從左至右，0 開始)</summary>
		private int mNodeLv = -1;
		/// <summary>於 <see cref="TreeView"/> 的節點於同層內的深度(從上至下，0 開始)</summary>
		private int mNodeIdx = -1;
		/// <summary>識別碼</summary>
		private long mID = -1;
		/// <summary>Parent Node 的識別碼</summary>
		private long? mInputLinkID = null;
		#endregion

		#region Properties
		/// <summary>取得此工具的識別碼</summary>
		public long ID { get { return mID; } }
		/// <summary>取得或設定物件是否已被修改</summary>
		public bool IsModified { get; set; } = false;
		/// <summary>取得或設定物件是否已被編譯過</summary>
		/// <remarks>(<see langword="true"/>)不需要再重新 Compile CVT  (<see langword="false"/>)需要重新 Compile</remarks>
		public bool IsCompiled { get; set; } = false;
		/// <summary>取得此工具的 <see cref="TreeView"/> 節點</summary>
		public TreeNode Node { get { return mNode; } }
		/// <summary>取得此工具的註解</summary>
		public string Comment { get { return mCmt; } }
		/// <summary>取得或設定物件，其包含相關資料</summary>
		public object Tag { get { return mTag; } set { mTag = value; } }
		/// <summary>取得於 <see cref="TreeView"/> 的節點之層數(從左至右，0 開始)</summary>
		public int NodeLevel { get { return mNodeLv; } }
		/// <summary>取得於 <see cref="TreeView"/> 的節點於同層內的深度(從上至下，0 開始)</summary>
		public int NodeIndex { get { return mNodeIdx; } }
		/// <summary>取得 Relatived 的識別碼</summary>
		public long? InputLinkID { get { return mInputLinkID; } }
		/// <summary>取得目標曝光值</summary>
		public int Exposure { get { return mExp; } }
		/// <summary>取得目標增益值</summary>
		public int Gain { get { return mGain; } }
		/// <summary>取得此工具是否可被複製</summary>
		public bool IsCopyable { get { return true; } }
		#endregion

		#region Constructors
		/// <summary>建構新的攝影機參數調整工具</summary>
		/// <param name="mainNode">欲存放此工具的父節點</param>
		public CamParaPack(TreeNode mainNode) {
			mID = DateTime.Now.ToBinary();
			mNode = mainNode.Nodes.Add("Camera Parameter");
			mNode.Tag = this;
			mNodeLv = mNode.Level;
			mNodeIdx = mNode.Index;
			IsModified = true;
		}

		/// <summary>複製攝影機參數調整工具</summary>
		/// <param name="mainNode">欲存放此工具的父節點</param>
		/// <param name="copyPack">欲複製的來源</param>
		public CamParaPack(TreeNode mainNode, CamParaPack copyPack) {
			mID = DateTime.Now.ToBinary();
			mNode = mainNode.Nodes.Add("Camera Parameter");
			mNode.Tag = this;
			mNodeLv = mNode.Level;
			mNodeIdx = mNode.Index;
			IsModified = true;

			mCmt = copyPack.Comment;
			mExp = copyPack.Exposure;
			mGain = copyPack.Gain;
			mTag = copyPack.Tag;
		}

		/// <summary>透過 <see cref="IXmlData"/> 建構攝影機參數調整工具</summary>
		/// <param name="xmlData">含有工具資料的 XML 節點</param>
		public CamParaPack(XmlElmt xmlData) {
			/* 拆出 Type */
			string toolType = xmlData.Attribute("Type").Value;
			if (toolType != "CameraPara") throw new InvalidCastException(GetMultiLangText("VisToolNotSup"));

			/* 取得 ID */
			mID = long.Parse(xmlData.Attribute("ID").Value);

			/* 樹節點 */
			XmlElmt childData;
			if (xmlData.Element("Node", out childData)) {
				if (!string.IsNullOrEmpty(childData.Value)) {
					mNode = new TreeNode(childData.Value);
					mNode.Tag = this;

					XmlAttr attr;
					if (childData.Attribute("ParentID", out attr))
						mInputLinkID = string.IsNullOrEmpty(attr.Value) ? null : (long?)long.Parse(attr.Value);
					if (childData.Attribute("Level", out attr)) mNodeLv = int.Parse(attr.Value);
					if (childData.Attribute("Index", out attr)) mNodeIdx = int.Parse(attr.Value);
				} else throw new ArgumentNullException("Node", GetMultiLangText("PathNull"));
			}

			/* 抓註解 */
			mCmt = xmlData.Element("Comment").Value;

			/* 抓曝光 */
			mExp = int.Parse(xmlData.Element("Exposure")?.Value ?? "0");

			/* 抓增益 */
			mGain = int.Parse(xmlData.Element("Gain")?.Value ?? "0");
		}
		#endregion

		#region Private Utilities
		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		private string GetMultiLangText(string key) {
			return CtLanguage.GetMultiLangXmlText("Language.xml", key)[key];
		}

		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		private Dictionary<string, string> GetMultiLangText(params string[] key) {
			return CtLanguage.GetMultiLangXmlText("Language.xml", key);
		}

		/// <summary>建立一行新的 <see cref="DataGridViewRow"/></summary>
		/// <param name="dgv">欲顯示此欄位的 <see cref="DataGridView"/></param>
		/// <param name="name">於第一欄顯示屬性名稱之字串</param>
		/// <param name="tip">欲顯示於提示視窗的提示文字</param>
		/// <returns>已建立的欄位</returns>
		private DataGridViewRow CreateNewRow(DataGridView dgv, string name, string tip = "") {
			DataGridViewRow row = new DataGridViewRow();
			row.CreateCells(dgv, name);
			row.Tag = tip;
			return row;
		}

		/// <summary>數字型態的輸入器，僅供 <see cref="int"/>, <seealso cref="short"/>, <seealso cref="float"/> 與 <seealso cref="double"/></summary>
		/// <typeparam name="T">實質型態數值，如 <see cref="int"/>, <see cref="double"/> 等</typeparam>
		/// <param name="curVal">當前數值</param>
		/// <param name="group">共需多少筆資料</param>
		/// <param name="minVal">最小數值</param>
		/// <param name="maxVal">最大數值</param>
		/// <param name="newVal">回傳使用者輸入的新數值</param>
		/// <returns>是否有完成輸入  (<see langword="false"/>)使用者取消 (<see langword="true"/>)使用者輸入合法數值</returns>
		protected bool ValueEditor<T>(string curVal, int group, T minVal, T maxVal, out List<T> newVal) where T : struct {
			bool verify = true;
			List<T> outVal = new List<T>(); //接收 ICtInput 回傳

			Dictionary<string, string> inMsg = GetMultiLangText("NormalEditTitle", "ValEditDesc", "ValEditInv", "ValEditOutOfRng", "ValEditInt", "ValEditReal", "ValEditGrp");

			string inVal;
			verify = CtInput.Text(out inVal, inMsg["NormalEditTitle"], string.Format(inMsg["ValEditDesc"].Replace(@"\r\n", "\r\n"), group.ToString(), minVal, maxVal), curVal) == Stat.SUCCESS;

			/* 如果使用者按下確認鍵才進行數值驗證 */
			if (verify) {
				string[] split = inVal.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
				if (split.Length == group) {
					if (typeof(T) == typeof(int)) {
						int intMin = Convert.ToInt32(minVal);
						int intMax = Convert.ToInt32(maxVal);
						verify = split.All(val => { int temp; return int.TryParse(val.Trim(), out temp); });
						if (verify) {
							outVal = split.ToList().ConvertAll(val => (T)(object)int.Parse(val.Trim()));
							verify = outVal.All(val => { int temp = Convert.ToInt32(val); return intMin <= temp && temp <= intMax; });
							if (!verify) CtMsgBox.Show(inMsg["ValEditInv"], inMsg["ValEditOutOfRng"], MsgBoxBtn.OK, MsgBoxStyle.Error);
						} else CtMsgBox.Show(inMsg["ValEditInv"], inMsg["ValEditInt"], MsgBoxBtn.OK, MsgBoxStyle.Error);
					} else if (typeof(T) == typeof(short)) {
						short sorMin = Convert.ToInt16(minVal);
						short sorMax = Convert.ToInt16(maxVal);
						verify = split.All(val => { short temp; return short.TryParse(val.Trim(), out temp); });
						if (verify) {
							outVal = split.ToList().ConvertAll(val => (T)(object)short.Parse(val.Trim()));
							verify = outVal.All(val => { int temp = Convert.ToInt32(val); return sorMin <= temp && temp <= sorMax; });
							if (!verify) CtMsgBox.Show(inMsg["ValEditInv"], inMsg["ValEditOutOfRng"], MsgBoxBtn.OK, MsgBoxStyle.Error);
						} else CtMsgBox.Show(inMsg["ValEditInv"], inMsg["ValEditInt"], MsgBoxBtn.OK, MsgBoxStyle.Error);
					} else if (typeof(T) == typeof(float)) {
						float sngMin = Convert.ToSingle(minVal);
						float sngMax = Convert.ToSingle(maxVal);
						verify = split.All(val => { float temp; return float.TryParse(val.Trim(), out temp); });
						if (verify) {
							outVal = split.ToList().ConvertAll(val => (T)(object)float.Parse(val.Trim()));
							verify = outVal.All(val => { float temp = Convert.ToSingle(val); return sngMin <= temp && temp <= sngMax; });
							if (!verify) CtMsgBox.Show(inMsg["ValEditInv"], inMsg["ValEditOutOfRng"], MsgBoxBtn.OK, MsgBoxStyle.Error);
						} else CtMsgBox.Show(inMsg["ValEditInv"], inMsg["ValEditReal"], MsgBoxBtn.OK, MsgBoxStyle.Error);
					} else if (typeof(T) == typeof(double)) {
						double dblMin = Convert.ToDouble(minVal);
						double dblMax = Convert.ToDouble(maxVal);
						verify = split.All(val => { double temp; return double.TryParse(val.Trim(), out temp); });
						if (verify) {
							outVal = split.ToList().ConvertAll(val => (T)(object)double.Parse(val.Trim()));
							verify = outVal.All(val => { double temp = Convert.ToDouble(val); return dblMin <= temp && temp <= dblMax; });
							if (!verify) CtMsgBox.Show(inMsg["ValEditInv"], inMsg["ValEditOutOfRng"], MsgBoxBtn.OK, MsgBoxStyle.Error);
						} else CtMsgBox.Show(inMsg["ValEditInv"], inMsg["ValEditReal"], MsgBoxBtn.OK, MsgBoxStyle.Error);
					}
				} else {
					verify = false;
					CtMsgBox.Show(inMsg["ValEditInv"], inMsg["ValEditGrp"], MsgBoxBtn.OK, MsgBoxStyle.Error);
				}
			}

			newVal = outVal;
			return verify;
		}
		#endregion

		#region Public Operations
		/// <summary>嘗試切換目標攝影機</summary>
		public void TrySetParameter() {
			if (mExp > -1 || mGain > -1) {
				/* 動態抓，這樣就不用去紀錄 Parent 是誰 */
				IVisionToolPack toolPack = mNode.Parent.Tag as IVisionToolPack;
				if (toolPack != null) {
					CtCameraParam para = new CtCameraParam(toolPack.Tool, mExp, mGain);
					para.TrySetParam();
				}
			}
		}
		#endregion

		#region IPropertable Implements

		/// <summary>取得此 <see cref="IVisionTool"/> 可供顯示於 <see cref="DataGridView"/> 之資料，建立後會自動填入當前數值</summary>
		/// <param name="langMap">各國語系之對應清單</param>
		/// <returns>對應的屬性檢視</returns>
		public List<PropertyView> CreateDataSource(Dictionary<string, string> langMap) {
			List<PropertyView> propList = new List<PropertyView>();

			propList.Add(
				new PropertyView(
					langMap["PropExp"],
					AccessLevel.None,
					langMap["TipExp"],
					"Exposure",
					mExp.ToString(),
					() => {
						List<int> exp;
						if (ValueEditor(mExp.ToString(), 1, -1, 1000000, out exp)) {
							mExp = exp[0];
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mExp.ToString();
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropGain"],
					AccessLevel.None,
					langMap["TipGain"],
					"Gain",
					mGain.ToString(),
					() => {
						List<int> gain;
						if (ValueEditor(mGain.ToString(), 1, -1, 63, out gain)) {
							mGain = gain[0];
							this.IsModified = true;
							this.IsCompiled = false;
						}
						return mGain.ToString();
					}
				)
			);

			propList.Add(
				new PropertyView(
					langMap["PropCmt"],
					AccessLevel.None,
					langMap["TipCmt"],
					"Comments",
					mCmt,
					() => {
						string cmt;
						if (CtInput.Text(out cmt, langMap["Cmt"], langMap["CmtEnt"], mCmt) == Stat.SUCCESS) {
							mCmt = cmt;
							this.IsModified = true;
						}
						return mCmt;
					}
				)
			);

			return propList;
		}
		#endregion

		#region IXmlSavable Implements
		/// <summary>產生物件的 XML 相關資料描述</summary>
		/// <param name="nodeName">此物件之 XML 節點名稱</param>
		/// <returns>XML 節點</returns>
		public XmlElmt CreateXmlData(string nodeName) {
			List<XmlAttr> attrColl = new List<XmlAttr>();
			attrColl.Add(new XmlAttr("Type", "CameraPara"));
			attrColl.Add(new XmlAttr("ID", mID.ToString()));

			List<XmlElmt> dataColl = new List<XmlElmt>();
			dataColl.Add(new XmlElmt("Comment", mCmt));
			dataColl.Add(
				new XmlElmt(
					"Node",
					mNode.Text,
					new XmlAttr("ParentID", (mNode.Parent?.Tag as IVisionProjectable)?.ID.ToString() ?? string.Empty),
					new XmlAttr("Level", mNodeLv.ToString()),
					new XmlAttr("Index", mNodeIdx.ToString())
				)
			);
			dataColl.Add(new XmlElmt("Exposure", mExp.ToString()));
			dataColl.Add(new XmlElmt("Gain", mGain.ToString()));

			return new XmlElmt(
				nodeName,
				attrColl,
				dataColl
			);
		}

		#endregion

		#region IPeripheryUtility Implements
		/// <summary>產生可適用於 <see cref="ICvtExecutor"/> 內的建構程式碼</summary>
		/// <returns>建構程式</returns>
		public List<string> GenerateExecutionConstruct() {
			return new List<string> { $"new CameraParameterExecutor(cvt, {mExp}, {mGain})" };
		}

		/// <summary>更新 <see cref="TreeNode"/> 相關資訊，<see cref="TreeNode.Level"/>、<see cref="TreeNode.Index"/> 等</summary>
		public void UpdateTreeNodeInformation() {
			mNodeIdx = mNode.Index;
			mNodeLv = mNode.Level;
		}

		/// <summary>檢查移除的物件是否與此工具有關聯，如有關聯則取消之</summary>
		/// <param name="tool">移除的物件</param>
		public void ConfirmRemovedLink(IVisionProjectable tool) {

		}
		#endregion

		#region IDisposable Implements
		/// <summary>指出是否已經釋放過資源</summary>
		protected bool mDisposed = false;

		/// <summary>釋放此影像工具資源</summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>釋放此影像工具資源之內容</summary>
		protected virtual void Dispose(bool disposing) {
			if (!mDisposed && disposing) {
				mNode = null;
				mCmt = null;
				mTag = null;
				mInputLinkID = null;
			}
		}

		/// <summary>解構子</summary>
		~CamParaPack() {
			Console.WriteLine("Disposing : " + mID.ToString());
			Dispose(true);
		}
		#endregion
	}

    #endregion

    #region Declaration - Lib

    ///<summary>TreeNode函示庫</summary>
    public class TreeNodeMeth {
        
        /// <summary>搜尋 <see cref="TreeView"/> 內的影像工具包</summary>
        /// <param name="node">要開始往下搜尋的 <see cref="TreeView"/> 節點</param>
        /// <param name="nodeColl">集合</param>
        public void SearchVisionToolPack(TreeNode node, ref List<IVisionToolPack> nodeColl) {
            /* 判斷是不是 tool pack，是的話就直接加入即和吧~ */
            if (node.Tag is IVisionToolPack) {
                IVisionToolPack pack = node.Tag as IVisionToolPack;
                //排除 model 和 cvt，這沒辦法當結果的來源 (cvt是因為目前僅允許一份，人為操作當然可以用...)
                if (pack.ToolType != typeof(ILocatorModel) && pack.ToolType != typeof(ICSharpCustomTool)) nodeColl.Add(pack);
            }

            /* 如果還有東西就往下做吧 */
            if (node.Nodes.Count > 0) {
                foreach (TreeNode subNode in node.Nodes) {
                    SearchVisionToolPack(subNode, ref nodeColl);
                }
            }
        }

        /// <summary>遞迴尋找 <see cref="TreeNode.Text"/> 為特定的字串</summary>
		/// <param name="node">欲尋找的 <see cref="TreeNode"/></param>
		/// <param name="nodeText">欲尋找的文字</param>
		/// <returns>相符的  TreeNode</returns>
		public TreeNode NodeRecursive(TreeNode node, string nodeText) {
            TreeNode found = null;
            if (node.Text == nodeText) found = node;
            else if (node.Nodes.Count > 0) {
                foreach (TreeNode item in node.Nodes) {
                    found = NodeRecursive(item, nodeText);
                    if (found != null) break;
                }
            }
            return found;
        }

        /// <summary>搜尋 <see cref="TreeNode"/> 符合特定文字的節點，並回傳其 <see cref="IVisionToolPack"/></summary>
		/// <param name="key">欲搜尋 <see cref="TreeNode"/> 文字</param>
		/// <param name="node">欲搜尋的 <see cref="TreeNode"/></param>
		/// <returns>符合的 <see cref="IVisionToolPack"/></returns>
		public IVisionToolPack SearchVisionToolPack(string key, TreeNode node) {
            TreeNode tempNode = NodeRecursive(node, key);
            IVisionToolPack resultPack = null;
            if (tempNode != null) {
                resultPack = tempNode.Tag as IVisionToolPack;    //如果 tag 不是 visiontoolpack 也沒差，同樣回傳 null
            }   //else 不用做，反正都是回傳 null
            return resultPack;
        }

    }

    ///<summary>屬性編譯器</summary>
    internal class PropEditer {
        ///<summary>座標點結果之影像工具參考類型</summary>
        private static List<Type> mPointRef = new List<Type>() {
            typeof(PointFinderPack),
            typeof(CalculatedPoint)
        };
        ///<summary>線段結果之影像工具參考類型</summary>
        private static List<Type> mLineRef = new List<Type>() {
            typeof(LineFinderPack),
            typeof(CalculatedLine)
        };
        ///<summary>弧線結果之影像工具參考類型</summary>
        private static List<Type> mArcRef = new List<Type>() {
            typeof(ArcFinderPack)
        };


        public static Func<string> GetEditer(string prop, Func<List<IVisionToolPack>> toolList) {
            throw new NotImplementedException();
        }

        public static bool IsPointResult(IVisionToolPack tool) {
            return mPointRef.Any(t => t == tool.GetType());
        }

        public static bool IsLineResult(IVisionToolPack tool) {
            return mLineRef.Any(t => t == tool.GetType());
        }

        public static bool IsArcResult(IVisionToolPack tool) {
            return mArcRef.Any(t => t == tool.GetType());
        }

    }

    #endregion Declaration - Lib
}
