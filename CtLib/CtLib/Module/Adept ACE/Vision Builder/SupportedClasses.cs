using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Diagnostics;

using Ace.HSVision.Client.ImageDisplay;
using Ace.HSVision.Server.Parameters;
using Ace.HSVision.Server.Tools;

using CtLib.Library;
using CtLib.Module.Adept.Extension;
using CtLib.Module.Dimmer;
using CtLib.Module.Utility;

namespace CtLib.Module.Adept {

	/// <summary>使用於 <see cref="ICSharpCustomTool"/> 之可執行的物件</summary>
	public enum CvtExecType {
		/// <summary>影像工具 <see cref="IVisionToolBase"/></summary>
		VisionTool,
		/// <summary>調光切燈 <see cref="CtLightCtrlData"/></summary>
		LightControl,
		/// <summary>攝影機參數 <see cref="CtCameraParam"/></summary>
		CameraParameter
            
	}

	/// <summary>使用於 <see cref="System.Windows.Forms.DataGridView"/> 顯示影像相關屬性與其數值</summary>
	public sealed class PropertyView : IDisposable {

		#region Fields
		/// <summary>屬性名稱</summary>
		private string mProp = string.Empty;
		/// <summary>顯示數值</summary>
		private string mCnt = string.Empty;
		/// <summary>下方註解</summary>
		private string mHint = string.Empty;
		/// <summary>ToolTip 訊息</summary>
		private string mTip = string.Empty;
		/// <summary>編輯時動作</summary>
		private Func<string> mAct;
		/// <summary>此屬性是否顯示之判斷方法</summary>
		private Func<bool> mVisb;
		/// <summary>最低的使用者權限</summary>
		private AccessLevel mUsrLv;
		#endregion

		#region Properties
		/// <summary>取得此屬性之名稱</summary>
		public string Property { get { return mProp; } }
		/// <summary>取得此屬性之數值</summary>
		public string Content { get { return mCnt; } }
		/// <summary>取得此屬性之提示</summary>
		public string Hint { get { return mHint; } }
		/// <summary>取得當滑鼠移至此屬性時欲顯示的註解</summary>
		public string ToolTip { get { return mTip; } }
		/// <summary>取得此屬性所需的最低使用者權限等級</summary>
		public AccessLevel MinimumUserLevel { get { return mUsrLv; } }
		#endregion

		#region Constructors
		/// <summary>建立帶有數值的屬性檢視</summary>
		/// <param name="lv">最低的使用者權限，如使用者小於此權限則隱藏顯示此項目</param>
		/// <param name="prop">屬性之名稱</param>
		/// <param name="cnt">屬性之數值</param>
		/// <param name="hint">屬性之提示</param>
		/// <param name="tip">滑鼠移至此屬性時欲顯示的註解</param>
		/// <param name="act">於編輯時要做的動作。於 <see cref="ExecuteEdit"/> 會無條件寫入返回值，請注意處理!</param>
		/// <param name="visb">檢查此列為顯示或隱藏之條件</param>
		public PropertyView(string prop, AccessLevel lv, string hint, string tip, string cnt, Func<string> act = null, Func<bool> visb = null) {
			mProp = prop;
			mUsrLv = lv;
			mCnt = cnt;
			mHint = hint.Replace("\\r\\n", "\r\n");
			mTip = tip.Replace("\\r\\n", "\r\n");
			mAct = act;
			mVisb = visb;
		}
		#endregion

		#region Public Operations
		/// <summary>執行編輯內容</summary>
		public void ExecuteEdit() {
			if (mAct != null) {
				string newVal = mAct();
				mCnt = newVal;
			}
		}

		/// <summary>檢查此列是否需要顯示於 <see cref="System.Windows.Forms.DataGridView"/></summary>
		/// <returns>(<see langword="true"/>)需顯示 (<see langword="false"/>)隱藏</returns>
		public bool CheckRowVisible(AccessLevel usrLv) {
			if (usrLv < mUsrLv) return false;
			else if (mVisb != null) return mVisb();
			else return true;
		}
		#endregion

		#region IDisposable Implements
		/// <summary>指出是否已經釋放過資源</summary>
		private bool mDisposed = false;

		/// <summary>釋放此物件資源</summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>釋放此物件之資源之內容</summary>
		private void Dispose(bool disposing) {
			if (!mDisposed && disposing) {
				mDisposed = true;
				mProp = null;
				mCnt = null;
				mHint = null;
				mAct = null;
			}
		}

		/// <summary>解構子</summary>
		~PropertyView() {
			Dispose(true);
		}
		#endregion

		#region Overrides
		/// <summary>判斷兩個 <see cref="PropertyView"/> 是否相同</summary>
		/// <param name="obj">欲判斷之物件</param>
		/// <returns>(True)兩者相同  (False)兩者不同</returns>
		public override bool Equals(object obj) {
			if (object.ReferenceEquals(obj, null)) return false;
			var pv = obj as PropertyView;
			if (pv == null) return false;
			else return string.Equals(mProp, pv.Property)
						&& string.Equals(mCnt, pv.Content)
						&& string.Equals(mHint, pv.Hint)
						&& string.Equals(mTip, pv.ToolTip);
		}

		/// <summary>取得此物件之雜湊碼</summary>
		/// <returns>雜湊碼</returns>
		public override int GetHashCode() {
			return mProp.GetHashCode() ^ mCnt.GetHashCode() ^ mHint.GetHashCode() ^ mTip.GetHashCode();
		}

		/// <summary>取得此物件之文字描述</summary>
		/// <returns>文字描述</returns>
		public override string ToString() {
			return $"{mProp}, {mCnt}";
		}

		/// <summary>比較兩個 <see cref="PropertyView"/> 是否相同</summary>
		/// <param name="a">被比較之 <see cref="PropertyView"/></param>
		/// <param name="b">欲比較之 <see cref="PropertyView"/></param>
		/// <returns>(True)兩者相同  (False)兩者不同</returns>
		public static bool operator ==(PropertyView a, PropertyView b) {
			if (object.ReferenceEquals(a, null)) return object.ReferenceEquals(b, null);
			else return a.Equals(b);
		}

		/// <summary>比較兩個 <see cref="PropertyView"/> 是否不同</summary>
		/// <param name="a">被比較之 <see cref="PropertyView"/></param>
		/// <param name="b">欲比較之 <see cref="PropertyView"/></param>
		/// <returns>(True)兩者不同  (False)兩者相同</returns>
		public static bool operator !=(PropertyView a, PropertyView b) {
			return !(a == b);
		}
		#endregion
	}

	/// <summary>燈源控制數值</summary>
	public class CtLightCtrlData : IDisposable {
		#region Fields
		private DimmerBrand mBrand = DimmerBrand.QUAN_DA;
		private string mComPort = string.Empty;
		private List<int> mLightValue = new List<int>();
		#endregion

		#region Properties
		/// <summary>取得或設定調光器品牌</summary>
		public DimmerBrand Brand { get { return mBrand; } set { mBrand = value; } }
		/// <summary>取得串列埠</summary>
		public string Port { get { return mComPort; } }
		/// <summary>取得電流數值</summary>
		public List<int> LightValues { get { return mLightValue.ToList(); } }
		#endregion

		#region Constructors
		/// <summary>建構燈源數值資訊</summary>
		/// <param name="port">串列埠</param>
		/// <param name="values">數值集合</param>
		public CtLightCtrlData(string port, params int[] values) {
			mComPort = port;
			mLightValue = values.ToList();
		}

		/// <summary>建構特定品牌的數值資訊</summary>
		/// <param name="brand">調光器品牌</param>
		/// <param name="port">串列埠</param>
		/// <param name="values">數值集合</param>
		public CtLightCtrlData(DimmerBrand brand, string port, params int[] values) {
			mBrand = brand;
			mComPort = port;
			mLightValue = values.ToList();
		}
		#endregion

		#region Public Functions
		/// <summary>設定調光器數值</summary>
		/// <returns>(<see langword="true"/>)設定成功  (<see langword="false"/>)設定失敗</returns>
		public void SetAllLight() {
			using (ICtDimmer dimmer = CtDimmerBase.Factory(mBrand)) {
				if (dimmer.Connect(mComPort) == Stat.SUCCESS)
					dimmer.SetLight(mLightValue);
			}
		}

		/// <summary>設定調光器數值</summary>
		/// <returns>(<see langword="true"/>)設定成功  (<see langword="false"/>)設定失敗</returns>
		public bool TrySetAllLight() {
			bool ret = true;
			try {
				using (ICtDimmer dimmer = CtDimmerBase.Factory(mBrand)) {
					ret = dimmer.Connect(mComPort) == Stat.SUCCESS;
					if (ret) ret = dimmer.SetLight(mLightValue);
				}
			} catch (Exception ex) {
				ret = false;
				Console.WriteLine(ex.Message);
			}
			return ret;
		}
		#endregion

		#region IDisposable Implements
		/// <summary>釋放相關資源</summary>
		public void Dispose() {
			if (mLightValue != null) {
				mLightValue.Clear();
				mLightValue = null;
			}
		}
		#endregion
	}

	/// <summary>提供攝影機相關參數之更動</summary>
	public class CtCameraParam : IDisposable {

		#region Fields
		/// <summary>攝影機</summary>
		private IVisionImageVirtualCamera mCamera;
		/// <summary>曝光值</summary>
		private int mExp = -1;
		/// <summary>增益值</summary>
		private int mGain = -1;
		#endregion

		#region Properties
		/// <summary>取得參考的攝影機物件</summary>
		public IVisionImageVirtualCamera DependentCamera { get { return mCamera; } }
		#endregion

		#region Constructors
		/// <summary>建構攝影機相關參數</summary>
		/// <param name="tool">欲更動的工具</param>
		/// <param name="exp">曝光值</param>
		/// <param name="gain">增益值</param>
		public CtCameraParam(IVisionToolBase tool, int exp, int gain) {
			IVisionImageVirtualCamera camera = null;
			IVisionToolBase toolBase = tool;
			while (true) {
				camera = toolBase?.GetDependentTools()?.Last() as IVisionImageVirtualCamera;
				if (camera == null) {
					toolBase = toolBase?.GetDependentTools()?.Last();
					if (toolBase == null) break;
				} else break;
			}

			mCamera = camera;
			mExp = exp;
			mGain = gain;
		}

		/// <summary>建構攝影機相關參數</summary>
		/// <param name="camera">指定的攝影機</param>
		/// <param name="exp">曝光值</param>
		/// <param name="gain">增益值</param>
		public CtCameraParam(IVisionImageVirtualCamera camera, int exp, int gain) {
			mCamera = camera;
			mExp = exp;
			mGain = gain;
		}
		#endregion

		#region Public Operations
		/// <summary>將當前的設定寫入攝影機</summary>
		public void SetParam() {
			if (mCamera != null) {
				if (mExp > -1) mCamera.ActiveSettings.Exposure = mExp;
				if (mGain > -1) mCamera.ActiveSettings.Gain = mGain;
			}
		}

		/// <summary>嘗試設定攝影機曝光值與增益值</summary>
		/// <returns>(<see langword="true"/>)寫入成功  (Flase)寫入失敗</returns>
		public bool TrySetParam() {
			bool ret = false;
			if (mCamera != null) {
				try {
					if (mExp > -1) mCamera.ActiveSettings.Exposure = mExp;
					if (mGain > -1) mCamera.ActiveSettings.Gain = mGain;
					ret = true;
				} catch (Exception ex) {
					Console.WriteLine(ex.Message);
				}
			}
			return ret;
		}
		#endregion

		#region IDisposable Implements
		/// <summary>釋放相關資源</summary>
		public void Dispose() {
			mCamera = null;
		}
		#endregion
	}

	/// <summary>儲存結果清單欄位相關資訊</summary>
	public class ResultableColumn {

		#region Fields
		private string mColName = string.Empty;
		private string mCapName = string.Empty;
		private bool mVis = false;
		#endregion

		#region Properties
		/// <summary>取得欄位名稱，對應 <see cref="DataColumn.ColumnName"/></summary>
		public string ColumnName { get { return mColName; } }
		/// <summary>取得欄位標題，對應 <see cref="DataColumn.Caption"/></summary>
		public string Caption { get { return mCapName; } }
		/// <summary>取得或設定此欄位是否顯示</summary>
		public bool Visible { get { return mVis; } set { mVis = value; } }
		#endregion

		#region Constructors
		/// <summary>建構欄位資訊</summary>
		/// <param name="col">欄位名稱</param>
		/// <param name="cap">欄位標題</param>
		/// <param name="vis">是否顯示</param>
		public ResultableColumn(string col, string cap, bool vis) {
			mColName = col;
			mCapName = cap;
			mVis = vis;
		}

		/// <summary>使用 XML 資訊進行建構</summary>
		/// <param name="elmt">含有欄位資料的 XML 節點</param>
		public ResultableColumn(XElement elmt) {
			mColName = elmt.Name.LocalName;
			mCapName = elmt.Attribute("Caption")?.Value;
			mVis = bool.Parse(elmt.Value);
		}
		#endregion

		#region Public Operations
		/// <summary>產生可描述此物件的 XML 節點</summary>
		/// <returns>XML 節點</returns>
		public XElement CreateElement() {
			return new XElement(mColName, new XAttribute("Caption", mCapName), mVis.ToString());
		}
		#endregion
	}

	/// <summary>儲存結果清單相關資訊</summary>
	public class ResultableTable {

		#region Fields
		private string mTabName = string.Empty;
		private Dictionary<string, ResultableColumn> mCols;
		#endregion

		#region Properties
		/// <summary>取得對應於 <see cref="DataTable.TableName"/> 之清單名稱，通常為 <see cref="IResultable.ResultTableName"/></summary>
		public string TableName { get { return mTabName; } }
		/// <summary>取得此清單內的欄位資訊</summary>
		public Dictionary<string, ResultableColumn> Columns { get { return mCols; } }
		#endregion

		#region Constructors
		/// <summary>建構結果清單資訊</summary>
		/// <param name="name">清單名稱</param>
		/// <param name="cols">清單欄位集合</param>
		public ResultableTable(string name, IEnumerable<ResultableColumn> cols) {
			mTabName = name;
			mCols = cols.ToDictionary(col => col.ColumnName, col => col);
		}

		/// <summary>使用含有清單資訊的 XML 節點進行建構</summary>
		/// <param name="elmt">XML 節點</param>
		public ResultableTable(XElement elmt) {
			mTabName = elmt.Name.LocalName;
			mCols = elmt.Elements().ToDictionary(node => node.Name.LocalName, node => new ResultableColumn(node));
		}
		#endregion

		#region Public Operations
		/// <summary>產生可描述此物件的 XML 節點</summary>
		/// <returns>XML 節點</returns>
		public XElement CreateElement() {
			return new XElement(
				mTabName,
				mCols.Select(node => node.Value.CreateElement())
			);
		}
		#endregion
	}

	/// <summary>提供 <see cref="VisionTransform"/> 之排序</summary>
	public class TransformComparer : IComparer<VisionTransform> {

		#region Fields
		private bool mL2R = false;
		private bool mR2L = false;
		private bool mT2B = false;
		private bool mB2T = false;
		#endregion

		#region Constructors
		/// <summary>建構 <see cref="VisionTransform"/> 排序子</summary>
		/// <param name="dir">排序方向</param>
		public TransformComparer(SortDirection dir) {
			mL2R = (dir & SortDirection.LeftToRight) == SortDirection.LeftToRight;
			mR2L = (dir & SortDirection.RightToLeft) == SortDirection.RightToLeft;
			mT2B = (dir & SortDirection.TopToBottom) == SortDirection.TopToBottom;
			mB2T = (dir & SortDirection.BottomToTop) == SortDirection.BottomToTop;
		}
		#endregion

		#region IComparer Implements
		/// <summary>實作 <see cref="VisionTransform"/> 比較</summary>
		/// <param name="x">欲比較之 <see cref="VisionTransform"/></param>
		/// <param name="y">欲被比較之 <see cref="VisionTransform"/></param>
		/// <returns>(-1)x &lt; y   (0)x = y   (1)x &gt; y</returns>
		public int Compare(VisionTransform x, VisionTransform y) {
			int camp_x = x.X.CompareTo(y.X);
			int camp_y = x.Y.CompareTo(y.Y);

			if (mR2L) camp_x = camp_x * -1;
			if (mT2B) camp_y = camp_y * -1;

			if (camp_y == 0) return camp_x;
			else return camp_y;
		}
		#endregion
	}

	/// <summary>提供影像區域分割模組，可取得分割區域與判斷特定索引</summary>
	public class CtTableRegion {

		#region Fields
		private int mRowCount = -1;
		private int mColCount = -1;
		private VisionRectangle mRegion;
		private List<VisionRectangle> mTable;
		#endregion

		#region Properties
		/// <summary>取得當前所分割之影像區域。依序為 (1,1) ... (1,n), (2,1), (2,2) ...</summary>
		public List<VisionRectangle> Tables { get { return mTable?.ToList(); } }
		#endregion

		#region Constructors
		/// <summary>由 <see cref="PointF"/> 建構影像分割區域</summary>
		/// <param name="leftTop">分割區域左上角座標</param>
		/// <param name="rightBottom">分割區域右下角座標</param>
		/// <param name="row">共有幾列，橫列直行</param>
		/// <param name="col">共有幾行，橫列直行</param>
		public CtTableRegion(PointF leftTop, PointF rightBottom, int row, int col) {
			double width = Math.Abs(rightBottom.X - leftTop.X);
			double height = Math.Abs(rightBottom.Y - leftTop.Y);
			double x = leftTop.X + width / 2;   //VisionRectangle CenterPoint 在矩形中間
			double y = leftTop.Y - height / 2;  //VisionRectangle CenterPoint 在矩形中間
			mRegion = new VisionRectangle(x, y, height, width);

			mRowCount = row;
			mColCount = col;

			mTable = CreateTable(mRegion, row, col);
		}

		/// <summary>由相關 X、Y 座標建構影像分割區域</summary>
		/// <param name="lt_x">分割區域左上角之 X 座標</param>
		/// <param name="lt_y">分割區域左上角之 Y 座標</param>
		/// <param name="rb_x">分割區域右下角之 X 座標</param>
		/// <param name="rb_y">分割區域右下角之 Y 座標</param>
		/// <param name="row">共有幾列，橫列直行</param>
		/// <param name="col">共有幾行，橫列直行</param>
		public CtTableRegion(double lt_x, double lt_y, double rb_x, double rb_y, int row, int col) {
			double width = Math.Abs(rb_x - lt_x);
			double height = Math.Abs(rb_y - lt_y);
			double x = lt_x + width / 2;    //VisionRectangle CenterPoint 在矩形中間
			double y = lt_y - height / 2;   //VisionRectangle CenterPoint 在矩形中間
			mRegion = new VisionRectangle(x, y, height, width);

			mRowCount = row;
			mColCount = col;

			mTable = CreateTable(mRegion, row, col);
		}
		#endregion

		#region Private Methods
		/// <summary>將 <see cref="VisionRectangle"/> 分割成特定數量，但尚未進行排序</summary>
		/// <param name="region">欲分割之影像矩形</param>
		/// <param name="row">共有幾列，橫列直行</param>
		/// <param name="col">共有幾行，橫列直行</param>
		/// <returns>分割完成之矩形</returns>
		/// <remarks>
		/// ┌───────┐		┌─┬─┬─┬─┐
		/// │       │	=>	├─┼─┼─┼─┤	2 row 4 col
		/// └───────┘		└─┴─┴─┴─┘
		/// </remarks>
		private List<VisionRectangle> CreateTable(VisionRectangle region, int row, int col) {
			List<VisionRectangle> table = new List<VisionRectangle>();
			double width = Math.Abs(region.RightLine.CenterPoint.X - region.LeftLine.CenterPoint.X) / col;
			double height = Math.Abs(region.TopLine.CenterPoint.Y - region.BottomLine.CenterPoint.Y) / row;
			double base_x = region.TopLeftPoint.X + width / 2, x = base_x, y = region.TopLeftPoint.Y - height / 2;

			/* 位移，並建立 VisionRectangle */
			for (int rowIdx = 0; rowIdx < row; rowIdx++) {
				for (int colIdx = 0; colIdx < col; colIdx++) {
					table.Add(new VisionRectangle(x, y, height, width));
					x += width;
				}
				x = base_x;
				y -= height;
			}
			return table;
		}
		#endregion

		#region Public Operations
		/// <summary>取得 <see cref="VisionTransform"/> 所相對應的格子編號</summary>
		/// <param name="results"><see cref="Ace.HSVision.Server.Tools.IVisionTool"/> 之結果</param>
		/// <param name="dir">排序方向</param>
		/// <returns>最符合的結果。 (-1)表示不在格子內</returns>
		public int GetIndex(VisionTransform results, SortDirection dir) {
			bool r_2_l = (dir & SortDirection.RightToLeft) == SortDirection.RightToLeft;
			bool t_2_b = (dir & SortDirection.TopToBottom) == SortDirection.TopToBottom;

			/* 排序格子，預設是 Left→Right 與 Bottom→Top (Cartesian 座標) */
			mTable.Sort(
				(x, y) => {
					int camp_x = x.CenterPoint.X.CompareTo(y.CenterPoint.X);
					int camp_y = x.CenterPoint.Y.CompareTo(y.CenterPoint.Y);

					if (r_2_l) camp_x = camp_x * -1;    //與預設相反，乘以 -1
					if (t_2_b) camp_y = camp_y * -1;    //與預設相反，乘以 -1

					/* 以 Y 為優先比較，若相同再比 X */
					if (camp_y == 0) return camp_x;
					else return camp_y;
				}
			);

			return mTable.FindIndex(rect => rect.Contains(results));
		}

		/// <summary>取得相關 <see cref="VisionTransform"/> 所相對應的格子編號，並回傳第一個結果</summary>
		/// <param name="results"><see cref="Ace.HSVision.Server.Tools.IVisionTool"/> 之結果，用於比較並回傳第一個結果</param>
		/// <param name="dir">排序方向</param>
		/// <returns>第一個結果。 (-1)表示不在格子內</returns>
		public KeyValuePair<VisionTransform, int> GetIndex(IEnumerable<VisionTransform> results, SortDirection dir) {
			bool r_2_l = (dir & SortDirection.RightToLeft) == SortDirection.RightToLeft;
			bool t_2_b = (dir & SortDirection.TopToBottom) == SortDirection.TopToBottom;

			/* 排序格子，預設是 Left→Right 與 Bottom→Top (Cartesian 座標) */
			mTable.Sort(
				(x, y) => {
					int camp_x = x.CenterPoint.X.CompareTo(y.CenterPoint.X);
					int camp_y = x.CenterPoint.Y.CompareTo(y.CenterPoint.Y);

					if (r_2_l) camp_x = camp_x * -1;    //與預設相反，乘以 -1
					if (t_2_b) camp_y = camp_y * -1;    //與預設相反，乘以 -1

					/* 以 Y 為優先比較，若相同再比 X */
					if (camp_y == 0) return camp_x;
					else return camp_y;
				}
			);

			/* 取得 VisionTransform 對應的格子 */
			Dictionary<VisionTransform, int> idxMap = results
				.ToDictionary(
					result => result,
					result => mTable.FindIndex(rect => rect.Contains(result))
				);

			/* 搜尋格子索引最小的，並回傳 */
			KeyValuePair<VisionTransform, int> min = new KeyValuePair<VisionTransform, int>(null, int.MaxValue);
			idxMap.ForEach(
				kvp => {
					if (kvp.Value > -1 && kvp.Value < min.Value) {
						min = kvp;
					}
				}
			);

			return min.Key == null ? new KeyValuePair<VisionTransform, int>(null, -1) : min;
		}

		/// <summary>取得相關 <see cref="VisionTransform"/> 所相對應的格子編號，並回傳所有的對應編號</summary>
		/// <param name="results"><see cref="Ace.HSVision.Server.Tools.IVisionTool"/> 之結果，用於比較並回傳結果</param>
		/// <param name="dir">排序方向</param>
		/// <returns>所有的對應結果。 (-1)表示不在格子內</returns>
		public Dictionary<VisionTransform, int> GetMapping(IEnumerable<VisionTransform> results, SortDirection dir) {
			bool r_2_l = (dir & SortDirection.RightToLeft) == SortDirection.RightToLeft;
			bool t_2_b = (dir & SortDirection.TopToBottom) == SortDirection.TopToBottom;

			/* 排序格子，預設是 Left→Right 與 Bottom→Top (Cartesian 座標) */
			mTable.Sort(
				(x, y) => {
					int camp_x = x.CenterPoint.X.CompareTo(y.CenterPoint.X);
					int camp_y = x.CenterPoint.Y.CompareTo(y.CenterPoint.Y);

					if (r_2_l) camp_x = camp_x * -1;    //與預設相反，乘以 -1
					if (t_2_b) camp_y = camp_y * -1;    //與預設相反，乘以 -1

					/* 以 Y 為優先比較，若相同再比 X */
					if (camp_y == 0) return camp_x;
					else return camp_y;
				}
			);

			/* 取得 VisionTransform 對應的格子 */
			return results
					.ToDictionary(
						result => result,
						result => mTable.FindIndex(rect => rect.Contains(result))
					);
		}
		#endregion

	}

	/// <summary>使用於 <see cref="ICSharpCustomTool"/> 程式碼內表示可執行之動作</summary>
	public interface ICvtExecutor : IDisposable {

		#region Properties
		/// <summary>取得此動作類型</summary>
		CvtExecType ExecuteType { get; }
		#endregion

		#region Methods
		/// <summary>執行不須參數之動作</summary>
		void DoAction();
		/// <summary>執行動作，並參考與修改是否需要重新取像旗標</summary>
		/// <param name="rePic">(可修改)是否重新取像</param>
		/// <param name="exitScript">(不修改)是否已執行失敗，不須再執行</param>
		/// <returns>(<see langword="true"/>)執行成功 (<see langword="false"/>)執行失敗，或為 NG</returns>
		bool DoAction(ref bool rePic, ref bool exitScript);
		#endregion
	}

    /// <summary>
    /// 測試用影像辨識執行器 by Jay 2017/08/17
    /// </summary>
    /// <remarks>
    /// 繼承自VisionExecutorOri(初始版本影像辨識執行器)
    /// DoAction方法重構
    /// 目標在NG的時候進行NG原因的紀錄
    /// </remarks>
    public class VisionExecutor : VisionExecutorOri {

        #region Declaration - Enum

        /// <summary>
        /// 計數器列舉
        /// </summary>
        private enum CountType {
            NG = 0,
            Locator = 1,
            Point = 2,
            Line = 3,
            Arc = 4,
            Blob = 5,
            Edge = 6,            
        }

        #endregion Declaration - Enum

        #region Properties
        /// <summary>
        /// 影像辨識NG原因記錄
        /// </summary>
        /// <remarks>
        /// 考慮到可能有複數原因導致辨識NG，紀錄格式範例如下
        /// $"{mTool.Name}辨識失敗,{mTool2.Name}辨識失敗,{}"
        /// </remarks>
        public static string NgReason { get; private set; } = "";

        #region DoAction Count

        /// <summary>
        /// 計數器
        /// </summary>
        private static Dictionary<CountType,int> mCount = new Dictionary<CountType, int>() {
            { CountType.Locator,0},
            { CountType.Point,0},
            { CountType.Arc,0 },
            { CountType.Blob,0 },
            { CountType.Edge,0},
            { CountType.Line,0 },
            { CountType.NG,0 }
        };

        #endregion DoActionCount
  
        #endregion Properties

        #region Function - Constructors

        /// <summary>建立 <see cref="IVisionToolBase"/> 影像相關工具的執行動作</summary>
        /// <param name="tool">欲執行的 <see cref="IVisionToolBase"/></param>
        /// <param name="input">Relative 的影像工具</param>
        /// <param name="act">無結果時的對應動作</param>
        /// <param name="x">含有 Relative 時的 Offset X 分量</param>
        /// <param name="y">含有 Relative 時的 Offset Y 分量</param>
        /// <param name="deg">含有 Relative 時的 Offset Degrees 分量</param>
        /// <param name="cvt">執行此動作的 <see cref="ICSharpCustomTool"/>，供繪製結果用</param>
        /// <param name="clrPass">合格之顏色</param>
        /// <param name="clrNg">不合格之顏色</param>
        public VisionExecutor(
            IVisionToolBase tool,
            IVisionTool input,
            double? x,
            double? y,
            double? deg,
            ICSharpCustomTool cvt,
            MarkerColor clrPass, 
            MarkerColor clrNg,
            NoResultAction act) : base(tool, input, x, y, deg, cvt, clrPass, clrNg, act) {
            NgReason = "";
            ClearCount();
        }

        #endregion Function - Constructors

        #region Function - Public Methods

        /// <summary>執行影像工具並畫出結果</summary>
        /// <param name="rePic">(不修改)是否需要重新取像，對應 <see cref="IVisionToolBase.Execute(bool)"/> 所使用的參數</param>
        /// <param name="exitScript">(可修改)是否已執行失敗。或設定 <see cref="NoResultAction.EXIT_SCRIPT"/> 且無結果時設定為 true</param>
        /// <returns>是否成功 (<see langword="true"/>)執行成功 (<see langword="false"/>)執行失敗或無回傳結果，即 NG</returns>
        public override bool DoAction(ref bool rePic, ref bool exitScript) {
            /* 如果不用執行了，直接離開 */
            if (exitScript) {
                return false;
            }

            /* 如果不是 IVisionTool，例如 ImageProcessing，只要改 rePic，下一個 Tool 自然會因為 ImageSource 而重拍 */
            if (!mIsTool) {
                rePic = true;
                return true;
            }

            /* 要回傳的結果狀態 */
            bool result = false;

            /* 如果有 relative 且他有結果就取得 Input Offset 並計算新的 Offset */
            if (mIsRelative) {
                /*-- relative工具沒結果則直接回傳 --*/
                if (!mInputLink.ResultsAvailable)return mNoResAct == NoResultAction.DO_NOTHING;

                /*-- 計算新Offset，並依照不同影像工具進行Offset設定 --*/
                var tarOfs = mInputLink.GetTransformResults()[0] * mOriOfs;
                if (mIsArc) {
                    IArcRoiTool arcTool = mTool as IArcRoiTool;
                    VisionArc oriSR = arcTool.SearchRegion;
                    VisionArc newSR = new VisionArc(tarOfs.X, tarOfs.Y, oriSR.Radius, oriSR.Thickness, tarOfs.Degrees, oriSR.Opening);
                    arcTool.SearchRegion = newSR;
                } else if (mOfsProp != null) {
                    mOfsProp.SetValue(mTool, tarOfs);
                }
            }
            
            /* 執行 Tool */
            mTool.Execute(rePic);

            /* 如果有成功跑完，畫出結果 */
            IVisionTool tool = mTool as IVisionTool;
            result = tool.ResultsAvailable;
            if (result) {
                tool.DrawResult(mCVT.OverlayMarkers, mClrPass, mClrNg, mClrPass);
                rePic = false;
            } else {
                if (mNoResAct == NoResultAction.EXIT_SCRIPT) exitScript = true;

                /*-- 產生Ng敘述 --*/
                CreateNgDescription(tool);
            }

            /*-- 是否判斷結果有無? --*/
            if (mNoResAct == NoResultAction.DO_NOTHING) result = true;

            /*-- 計數該影像工具執行次數 --*/
            SetCount(tool);

            return result;
        }

        #endregion Function - Public Methods

        #region Funciton - Private Mehtods

        /// <summary>
        /// 產生NG訊息
        /// </summary>
        private void CreateNgDescription(IVisionTool tool) {
            /*-- 取得參數 --*/
            int actionCount = GetCount(tool);//該影像工具已執行的次數
            string ngCount = $"({mCount[CountType.NG]:00})";//目前影像專案已Ng的次數
            string toolName = $"[{actionCount:00}]{GetToolName(tool)}";
            string ngDescription = $"{ngCount} {toolName} 辨识失败";

            /*-- 繪出ROI --*/
            tool.DrawRoi(mCVT.OverlayMarkers, mClrNg);

            /*-- 依照Ng數對影像工具進行編號標記 --*/
            tool.DrawComment(mCVT.OverlayMarkers, ngCount, mClrNg);

            /*-- 紀錄Ng原因敘述 --*/
            NgReason += $"{ngDescription}\r\n";
            Trace.WriteLine($"{ngDescription}");
            Console.WriteLine($"{ngDescription}");

            /*--Ng次數計數--*/
            mCount[CountType.NG]++;
        }

        /// <summary>
        /// 計數器清除
        /// </summary>
        private static void ClearCount() {
            try {
                
                foreach (var kvp in Enum.GetValues(typeof(CountType))) {
                    mCount[(CountType)kvp] = 0;
                }
            }catch(Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM,ex,true);
            }

        }

        /// <summary>
        /// 計數對應影像工具的DoAction計數器
        /// </summary>
        /// <param name="tool"></param>
        private void SetCount(IVisionTool tool) {
            mCount[ToEnum(tool)]++;
        }

        /// <summary>
        /// 取得對應影像工具執行次數
        /// </summary>
        /// <param name="tool">要取得的影像工具</param>
        /// <returns>執行次數</returns>
        private int GetCount(IVisionTool tool) {
            return mCount[ToEnum(tool)];
        }

        /// <summary>
        /// 取得影像工具對應的<see cref="CountType"/>
        /// </summary>
        /// <param name="tool">要對應之影像工具</param>
        /// <returns>與之對應的<see cref="CountType"/></returns>
        private CountType ToEnum(IVisionTool tool) {
            CountType type = CountType.NG;
            if (tool is ILocatorTool) {
                type = CountType.Locator;
            } else if (tool is IPointFinderTool) {
                type = CountType.Point;
            } else if (tool is ILineFinderTool) {
                type = CountType.Line;
            } else if (tool is IArcFinderTool) {
                type = CountType.Arc;
            } else if (tool is IBlobAnalyzerTool) {
                type = CountType.Blob;
            } else if (tool is IEdgeLocatorTool) {
                type = CountType.Edge;
            } else {
                throw new ArgumentException("未定義的影像工具類型");
            }
            return type;
        }

        /// <summary>取得 <see cref="IVisionTool"/> 的自訂名稱</summary>
        /// <param name="tool">欲取的名稱的影像工具</param>
        /// <returns>相對應的自訂名稱</returns>
		protected string GetToolName(IVisionTool tool) {
            Type toolType = tool.GetType();
            string name = string.Empty;
            switch (toolType.Name) {
                case "LocatorTool":
                    name = "Locator";
                    break;
                case "BlobAnalyzerTool":
                    name = "Blob Analyzer";
                    break;
                case "EdgeLocatorTool":
                    name = "Edge Locator";
                    break;
                case "LineFinderTool":
                    name = "Line Finder";
                    break;
                case "ArcFinderTool":
                    name = "Arc Finder";
                    break;
                case "PointFinderTool":
                    name = "Point Finder";
                    break;
                default:
                    throw new ArgumentException($"未定義影像工具類型{toolType.Name}");
  
            }
            return name;
        }

        #endregion Function - Private Methods
    }

    /// <summary>使用於 <see cref="ICSharpCustomTool"/> 內的 <see cref="IVisionToolBase"/> 動作執行器</summary>
    public class VisionExecutorOri : ICvtExecutor {

		#region Fields
        /// <summary>
        /// 影像辨識工具實例
        /// </summary>
		protected IVisionToolBase mTool;
        /// <summary>
        /// 參考之影像辨識工具實例
        /// </summary>
		protected IVisionTool mInputLink;
        /// <summary>
        /// 影像辨識工具CVT實例
        /// </summary>
		protected ICSharpCustomTool mCVT;
        /// <summary>
        /// <see cref="mTool"/>之Offset副本
        /// </summary>
		protected VisionTransform mOriOfs;
        /// <summary>
        /// 影像工具Offset屬性設置器
        /// </summary>
        /// <remarks>
        /// 以反射方式對影像工具的Offset屬性進行設定
        /// </remarks>
		protected PropertyInfo mOfsProp;
        /// <summary>
        /// <see cref="mTool"/>是否具有<see cref="IArcRoiTool"/>接口
        /// </summary>
		protected bool mIsArc = false;
        /// <summary>
        /// <see cref="mTool"/>是否具有<see cref="IVisionTool"/>接口
        /// </summary>
		protected bool mIsTool = true;
        /// <summary>
        /// <see cref="mTool"/>是否具有參考影像辨識工具
        /// </summary>
		protected bool mIsRelative = false;
        /// <summary>
        /// 無結果時的對應動作
        /// </summary>
		protected NoResultAction mNoResAct = NoResultAction.DO_NOTHING;
        /// <summary>
        /// 合格之顏色
        /// </summary>
        protected MarkerColor mClrPass;
        /// <summary>
        /// 不合格之顏色
        /// </summary>
		protected MarkerColor mClrNg;
		#endregion

		#region Properties
		/// <summary>取得此動作類型</summary>
		public CvtExecType ExecuteType { get { return CvtExecType.VisionTool; } }
        #endregion
        
        /// <summary>建立 <see cref="IVisionToolBase"/> 影像相關工具的執行動作</summary>
        /// <param name="tool">欲執行的 <see cref="IVisionToolBase"/></param>
        /// <param name="input">Relative 的影像工具</param>
        /// <param name="act">無結果時的對應動作</param>
        /// <param name="x">含有 Relative 時的 Offset X 分量</param>
        /// <param name="y">含有 Relative 時的 Offset Y 分量</param>
        /// <param name="deg">含有 Relative 時的 Offset Degrees 分量</param>
        /// <param name="cvt">執行此動作的 <see cref="ICSharpCustomTool"/>，供繪製結果用</param>
        /// <param name="clrPass">合格之顏色</param>
        /// <param name="clrNg">不合格之顏色</param>
        public VisionExecutorOri(
				IVisionToolBase tool, IVisionTool input,
				double? x, double? y, double? deg,
				ICSharpCustomTool cvt,
				MarkerColor clrPass, MarkerColor clrNg,
				NoResultAction act) {

			mNoResAct = act;
			mTool = tool;
			mCVT = cvt;
			mClrNg = clrNg;
			mClrPass = clrPass;
            
			/*-- 如果此工具是 IVisionTool，紀錄 Relative 資訊 --*/
			IVisionTool visTool = tool as IVisionTool;
			mIsTool = visTool != null;
			if (mIsTool) {
				/* 取得 Relative Parent */
				mInputLink = input;
				mIsRelative = mInputLink != null;

				/* 取得是否是 Arc 類型，如果不是則取得 Offset 屬性 */
				IArcRoiTool arcTool = visTool as IArcRoiTool;
				mIsArc = arcTool != null;
				if (!mIsArc) mOfsProp = visTool.GetType().GetProperty("Offset");

				/* 如果當前有 Relative 再去紀錄綠框 */
				if (mIsRelative && x.HasValue && y.HasValue && deg.HasValue) {
					mOriOfs = new VisionTransform(x.Value, y.Value, deg.Value);

					/* 將 Relative 取消 */
					visTool.GetInputLinks()[0].Reference = null;
				}
			}
		}

        #region ICvtExecutor Implements
        /// <summary>(不支援)執行不須參數之動作</summary>
        public void DoAction() {
			throw new NotSupportedException("影像執行器不支援無參數動作");
		}

		/// <summary>執行影像工具並畫出結果</summary>
		/// <param name="rePic">(不修改)是否需要重新取像，對應 <see cref="IVisionToolBase.Execute(bool)"/> 所使用的參數</param>
		/// <param name="exitScript">(可修改)是否已執行失敗。或設定 <see cref="NoResultAction.EXIT_SCRIPT"/> 且無結果時設定為 true</param>
		/// <returns>是否成功 (<see langword="true"/>)執行成功 (<see langword="false"/>)執行失敗或無回傳結果，即 NG</returns>
		public virtual bool DoAction(ref bool rePic, ref bool exitScript) {
			/* 如果不用執行了，直接離開 */
			if (exitScript) {
				return false;
			}

			/* 如果不是 IVisionTool，例如 ImageProcessing，只要改 rePic，下一個 Tool 自然會因為 ImageSource 而重拍 */
			if (!mIsTool) {
				rePic = true;
				return true;
			}

			/* 要回傳的結果狀態 */
			bool result = false;

			/* 如果有 relative 且他有結果就取得 Input Offset 並計算新的 Offset */
			if (mIsRelative && mInputLink.ResultsAvailable) {
				var tarOfs = mInputLink.GetTransformResults()[0] * mOriOfs;
				if (mIsArc) {
					IArcRoiTool arcTool = mTool as IArcRoiTool;
					VisionArc oriSR = arcTool.SearchRegion;
					VisionArc newSR = new VisionArc(tarOfs.X, tarOfs.Y, oriSR.Radius, oriSR.Thickness, tarOfs.Degrees, oriSR.Opening);
					arcTool.SearchRegion = newSR;
				} else if (mOfsProp != null) {
					mOfsProp.SetValue(mTool, tarOfs);
				}
			} else if (mIsRelative) return mNoResAct == NoResultAction.DO_NOTHING;

			/* 執行 Tool */
			result = mTool.Execute(rePic);

			/* 如果有成功跑完，畫出結果 */
			IVisionTool tool = mTool as IVisionTool;
			result = tool.ResultsAvailable;
			if (!result && mNoResAct == NoResultAction.EXIT_SCRIPT) exitScript = true;
			if (result) {
				tool.DrawResult(mCVT.OverlayMarkers, mClrPass, mClrNg, mClrPass);
				rePic = false;
			} else tool.DrawRoi(mCVT.OverlayMarkers, mClrNg);

			if (mNoResAct == NoResultAction.DO_NOTHING) result = true;

            if (result == false) {
                Trace.WriteLine($"{tool.FullPath} Result NG");
                Console.WriteLine($"{tool.FullPath} Result NG");
            }

            return result;
		}

		#endregion

		#region IDisposable Implement
		/// <summary>釋放相關資源</summary>
		public void Dispose() {
			mTool = null;
			mInputLink = null;
			mCVT = null;
			mOriOfs = null;
			mOfsProp = null;
		}
		#endregion
	}

	/// <summary>使用於 <see cref="ICSharpCustomTool"/> 內的調光器數值設定執行器</summary>
	public class LightExecutor : ICvtExecutor {

		#region Fields
		private CtLightCtrlData mLight;
		#endregion

		#region Properties
		/// <summary>取得此動作類型</summary>
		public CvtExecType ExecuteType { get { return CvtExecType.LightControl; } }
		#endregion

		#region Constructor
		/// <summary>建立調光器控制動作</summary>
		/// <param name="brand">調光器品牌</param>
		/// <param name="port">串列埠</param>
		/// <param name="value">燈光數值</param>
		public LightExecutor(DimmerBrand brand, string port, params int[] value) {
			mLight = new CtLightCtrlData(brand, port, value);
		}
		#endregion

		#region ICvtExecutor Implement
		/// <summary>嘗試切換調光器</summary>
		public void DoAction() {
			if (mLight != null) mLight.TrySetAllLight();
		}

		/// <summary>切換調光器，並修改參數為需要重新取像</summary>
		/// <param name="rePic">(必修改)是否重新取像</param>
		/// <param name="exitScript">(不修改)是否已執行失敗，不須再執行</param>
		/// <returns>是否成功 (<see langword="true"/>)執行成功 (<see langword="false"/>)執行失敗或無回傳結果，即 NG</returns>
		/// <remarks>目前為了模擬測試，忽略 result，直接回傳 true</remarks>
		public bool DoAction(ref bool rePic, ref bool exitScript) {
			if (exitScript) return false;
			if (mLight != null) mLight.TrySetAllLight();
			rePic = true;
			return true;
		}
		#endregion

		#region IDisposable Implement
		/// <summary>釋放相關資源</summary>
		public void Dispose() {
			if (mLight != null) {
				mLight.Dispose();
				mLight = null;
			}
		}
		#endregion
	}

	/// <summary>使用於 <see cref="ICSharpCustomTool"/> 內的攝影機曝光、增益值設定執行器</summary>
	public class CameraParameterExecutor : ICvtExecutor {

		#region Fields
		private CtCameraParam mCamera;
		#endregion

		#region Properties
		/// <summary>取得此動作類型</summary>
		public CvtExecType ExecuteType { get { return CvtExecType.CameraParameter; } }
		#endregion

		#region Constructor
		/// <summary>建立攝影機參數調整動作</summary>
		/// <param name="tool">影像工具，於建立後自動找尋對應的攝影機</param>
		/// <param name="exp">曝光值，(-1)表示不切換</param>
		/// <param name="gain">增益值，(-1)表示不切換</param>
		public CameraParameterExecutor(IVisionToolBase tool, int exp, int gain) {
			mCamera = new CtCameraParam(tool, exp, gain);
		}
		#endregion

		#region ICvtExecutor Implement
		/// <summary>嘗試更改攝影機之曝光值與增益值</summary>
		public void DoAction() {
			if (mCamera != null) mCamera.TrySetParam();
		}

		/// <summary>更改攝影機之曝光值與增益值，並修改參數為需要重新取像</summary>
		/// <param name="rePic">(必修改)是否重新取像</param>
		/// <param name="exitScript">(不修改)是否已執行失敗，不須再執行</param>
		/// <returns>是否成功 (<see langword="true"/>)執行成功 (<see langword="false"/>)執行失敗或無回傳結果，即 NG</returns>
		public bool DoAction(ref bool rePic, ref bool exitScript) {
			if (exitScript) return false;
			bool result = false;
			if (mCamera != null) result = mCamera.TrySetParam();
			rePic = true;
			return result;
		}
		#endregion

		#region IDisposable Implement
		/// <summary>釋放相關資源</summary>
		public void Dispose() {
			if (mCamera != null) {
				mCamera.Dispose();
				mCamera = null;
			}
		}
		#endregion
	}
}
