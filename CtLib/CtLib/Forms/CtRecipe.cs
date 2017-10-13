using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using System.Windows.Forms;

using CtLib.Library;
//using CtLib.Module.Adept;
//using CtLib.Module.Beckhoff;
using CtLib.Module.Utility;
using CtLib.Module.XML;

namespace CtLib.Forms {

	/// <summary>
	/// Recipe 介面
	/// <para>請使用 "Start" 方法來開起此視窗</para>
	/// </summary>
	public partial class CtRecipe : Form {

		#region Version

		/// <summary>CtRecipe 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0  Ahern [2015/02/05]
		///     + 從 CAMPro 搬移至 CtLib
		///     
		/// 1.1.0  Ahern [2015/05/26]
		///     \ 寫入設備鈕，限定存檔後才可儲存
		///     \ 權限 ReadOnly 相反
		///     + RecipeInfo，Start 後將最後載入的 Recipe 資訊丟出去，並加上 Stt 判斷是否有寫入設備
		/// 
		/// 1.1.1  Jackson [2016/02/19]
		///		\ Color.Empty 改以 SystemColors.Window 顯示
		/// 
		/// 1.1.2  Ahern [2016/02/24]
		///		+ 僅工程師以上等級可修改數值
		///		\ 如果讀取失敗，則 mRecipe 會被清空無法搜尋，改以 mEmptyRecipe 搜尋
		/// 
		/// 1.1.3  Ahern [2016/03/29]
		///		\ 僅管理員顯示變數名稱
		///		\ 僅工程師以上顯示儲存按鈕
		/// 
		/// 1.2.0  Ahern [2016/03/30]
		///		\ 改以新 ICtRecipe 實作
		///		
		/// 1.3.0  Ahern [2016/03/31]
		///		\ 更改介面樣式，以 UiMode 與編輯鈕進行操作
		///		
		/// 1.3.1  Ahern [2017/02/07]
		///		\ 修正載入時的 XML 錯誤
		/// 
		/// </code></remarks>
		public static readonly CtVersion @Version = new CtVersion(1, 3, 1, "2017/02/07", "Ahern Kuo");

		#endregion

		#region Declaration - Enumerations
		private enum UiMode {
			None = 0,
			EditingDefault = 1,
			EditingNew = 2,
			Saved = 3
		}
		#endregion

		#region Declaration - Definitions

		/// <summary>Recipe之附檔名</summary>
		public static readonly string RECIPE_EXTENSION = @"*.xml";

		#endregion

		#region Declaration - Properties
		/// <summary>限制 Recipe ID 僅能輸入數字</summary>
		public bool IDNumericOnly {
			get { return mNumOnly; }
			set {
				if (value && !mNumOnly) {
					txtID.KeyPress += txtID_KeyPress;
					mNumOnly = true;
				} else if (mNumOnly) {
					txtID.KeyPress -= txtID_KeyPress;
					mNumOnly = false;
				}
			}
		}
		#endregion

		#region Declaration - Fields

		/// <summary>[Ref] 已建立之CtAce</summary>
		//private List<CtAce> rAce = new List<CtAce>();
		///// <summary>[Ref] 已建立之CtBeckhoff</summary>
		//private List<CtBeckhoff> rBkf = new List<CtBeckhoff>();

		/// <summary>處理XML相關事項</summary>
		private CtXML mXML = new CtXML();

		/// <summary>當前介面模式</summary>
		private UiMode mUiMode = UiMode.None;
		/// <summary>當前使用者權限</summary>
		private AccessLevel mUserLv = AccessLevel.None;

		/// <summary>原始Recipe資料，即於Opcode階段所引入之Recipe</summary>
		private List<ICtRecipe> mEmptyRecipe;
		/// <summary>當前載入之Recipe資料</summary>
		private List<ICtRecipe> mRecipe = new List<ICtRecipe>();
		/// <summary>暫存當前所載入的 Recipe 資訊</summary>
		private RecipeInfo mCurrInfo;

		/// <summary>是否有被更改過</summary>
		private bool mChanged = false;
		/// <summary>紀錄是否有寫入設備</summary>
		private bool mWrtieToEquip = false;
		/// <summary>Recipe ID 是否僅能輸入數字</summary>
		private bool mNumOnly = false;
		/// <summary>是否有載入 Recipe 至畫面上</summary>
		private bool mRecipeLoaded = false;

		/// <summary>儲存點取載入時之檔案名稱</summary>
		private string mOriName = "";
		/// <summary>Recipe 之 檔案路徑</summary>
		private string mFileFolder = CtDefaultPath.GetPath(SystemPath.Recipe);

		private Dictionary<UILanguage, string> mNewStr = new Dictionary<UILanguage, string> {
			{ UILanguage.English, "<Add New>" }, { UILanguage.SimplifiedChinese, "<建立新檔>" }, { UILanguage.TraditionalChinese, "<建立新檔>" }
		};

		#endregion

		#region Function - Constructor
		/// <summary>建立 CtRecipe，如要啟動請使用 "Start" 方法</summary>
		/// <param name="emptyRecipe">空的 Recipe Data，用於建立新 Recipe 或是初始化時</param>
		public CtRecipe(List<ICtRecipe> emptyRecipe) {
			InitializeComponent();

			mEmptyRecipe = emptyRecipe.ToList();
		}
		#endregion

		#region Function - Core
		///// <summary>將已建立的 CtAce 加入至欄位中</summary>
		///// <param name="ace">CtAce 集合</param>
		//public void AssignAce(params CtAce[] ace) {
		//	if (rAce == null) rAce = new List<CtAce>();
		//	rAce.AddRange(ace);
		//}

		///// <summary>將已建立的 CtBeckhoff 加入至欄位中</summary>
		///// <param name="bkf">CtBeckhoff 集合</param>
		//public void AssignBeckhoff(params CtBeckhoff[] bkf) {
		//	if (rBkf == null) rBkf = new List<CtBeckhoff>();
		//	rBkf.AddRange(bkf);
		//}

		/// <summary>
		/// 開啟 Recipe 視窗。請先確認已使用 AssignACE / AssignBeckhoff 等方法加入相關物件
		/// <para>如果帶有 currentRecipe 之引數，則會自動更新當前項目，然後先填好名稱(未存檔)，方便後續操作</para>
		/// </summary>
		/// <param name="lv">登入者的權限等級，目前僅有 Administrator 可以直接修改數值</param>
		/// <param name="recipe">欲儲存與修改的 Recipe Data</param>
		/// <param name="currentRecipe">當前的 Recipe 名稱，只用於開啟時是否先行更新與顯示而已。保持空字串則不自動更新與填入</param>
		/// <remarks>目前限定工程師以上才可以直接修改數值，其餘權限只能看不能改</remarks>
		public void Start(AccessLevel lv, ref List<ICtRecipe> recipe, string currentRecipe = "") {
			mUserLv = lv;
			tsk_UiMode(UiMode.None);
			dgvRecipe.BeginInvokeIfNecessary(
				() => {
					colValue.ReadOnly = (lv < AccessLevel.Engineer);
					colName.Visible = (lv == AccessLevel.Administrator);
				}
			);
			SearchRecipe();

			if (currentRecipe != "") LoadRecipe(currentRecipe);

			if (!Visible) this.ShowDialog();
			recipe = mRecipe.ToList();
		}

		/// <summary>
		/// 開啟 Recipe 視窗。請先確認已使用 AssignACE / AssignBeckhoff 等方法加入相關物件
		/// <para>如果帶有 currentRecipe 之引數，則會自動更新當前項目，然後先填好名稱(未存檔)，方便後續操作</para>
		/// </summary>
		/// <param name="lv">登入者的權限等級，目前僅有 Administrator 可以直接修改數值</param>
		/// <param name="recipe">欲儲存與修改的 Recipe Data</param>
		/// <param name="currentRecipe">當前的 Recipe 名稱，只用於開啟時是否先行更新與顯示而已。保持空字串則不自動更新與填入</param>
		/// <param name="recipeInfo">最後存取的 Recipe 相關資訊</param>
		/// <returns>Status Code</returns>
		/// <remarks>目前限定工程師以上才可以直接修改數值，其餘權限只能看不能改</remarks>
		public Stat Start(AccessLevel lv, ref List<ICtRecipe> recipe, out RecipeInfo recipeInfo, string currentRecipe = "") {
			mUserLv = lv;
			tsk_UiMode(UiMode.None);
			dgvRecipe.BeginInvokeIfNecessary(
				() => {
					colValue.ReadOnly = (lv < AccessLevel.Engineer);
					colName.Visible = (lv == AccessLevel.Administrator);
				}
			);
			SearchRecipe();

			if (currentRecipe != "") LoadRecipe(currentRecipe);

			if (!Visible) this.ShowDialog();

			recipeInfo = mCurrInfo;
			recipe = mRecipe.ToList();

			if (DialogResult == DialogResult.OK) return Stat.SUCCESS;
			else return Stat.WN_SYS_USRCNC;
		}

		/// <summary>直接從檔案載入 Recipe 並寫入設備</summary>
		/// <param name="recipeName">Recipe 名稱，不含路徑與附檔名。如 "Recipe01"</param>
		/// <param name="ace">此設備所含有之 Adept ACE 裝置</param>
		/// <param name="bkf">此設備所含有之 Beckhoff PLC 裝置</param>
		/// <returns>Status Code</returns>
		//public static Stat LoadRecipe(string recipeName, List<CtAce> ace = null, List<CtBeckhoff> bkf = null) {
		//	Stat stt = Stat.SUCCESS;
		//	string path = CtFile.BackSlash(CtDefaultPath.GetPath(SystemPath.Recipe)) + RECIPE_EXTENSION.Replace("*", recipeName);
		//	string strTemp = "";
		//	try {
		//		if (CtFile.IsFileExist(path)) {

		//			List<ICtRecipe> recipe = new List<ICtRecipe>();

		//			/*-- 載入XML --*/
		//			XmlElmt ctRcp = CtXML.Load(path);

		//			/*-- 註解 --*/
		//			strTemp = ctRcp.Element("Comment")?.Value;

		//			/*-- 抓取內容 --*/
		//			List<XmlElmt> data = ctRcp.Elements("Data");
		//			if (data.Count < 1) {
		//				stt = Stat.ER_SYS_ILLVAL;
		//				throw (new Exception("Recipe檔案有誤"));
		//			}

		//			XmlAttr attr;
		//			foreach (XmlElmt item in data) {
		//				if (item.Attribute("Device", out attr)) {
		//					switch (attr.Value) {
		//						case "ACE":
		//							recipe.Add(new AceVPlusRecipe(item));
		//							break;
		//						case "ACE_NUM":
		//							recipe.Add(new AceNumRecipe(item));
		//							break;
		//						case "ACE_VIS":
		//						case "CAMPro":
		//							recipe.Add(new AceVisionRecipe(item));
		//							break;
		//						case "BECKHOFF":
		//							recipe.Add(new BeckhoffRecipe(item));
		//							break;
		//						case "DELTA_PLC":
		//							recipe.Add(new DeltaPlcRecipe(item));
		//							break;
		//						default:
		//							break;
		//					}
		//				}
		//			}

		//			foreach (ICtRecipe item in recipe) {
		//				if (item is AceVPlusRecipe) {
		//					AceVPlusRecipe rcp = item as AceVPlusRecipe;
		//					rcp.WriteValue(ace[rcp.DeviceIndex]);
		//				} else if (item is AceNumRecipe) {
		//					AceNumRecipe rcp = item as AceNumRecipe;
		//					rcp.WriteValue(ace[rcp.DeviceIndex]);
		//				} else if (item is AceVisionRecipe) {
		//					AceVisionRecipe rcp = item as AceVisionRecipe;
		//					rcp.WriteValue(ace[rcp.DeviceIndex]);
		//				} else if (item is BeckhoffRecipe) {
		//					BeckhoffRecipe rcp = item as BeckhoffRecipe;
		//					rcp.WriteValue(bkf[rcp.DeviceIndex]);
		//				} else if (item is DeltaPlcRecipe) {
		//					DeltaPlcRecipe rcp = item as DeltaPlcRecipe;
		//				}
		//			}

		//		} else {
		//			stt = Stat.ER_SYS_NOFILE;
		//			throw (new Exception("找無對應檔案，請檢查檔案是否正確"));
		//		}

		//	} catch (Exception ex) {
		//		if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
		//		CtStatus.Report(stt, ex);
		//	}
		//	return stt;
		//}
		
        #endregion

		#region Function - Methods

		private void tsk_UiMode(UiMode mode) {
			mUiMode = mode;
			dgvRecipe.BeginInvokeIfNecessary(() => colValue.ReadOnly = !(mUserLv > AccessLevel.Operator && (mode == UiMode.EditingDefault || mode == UiMode.EditingNew)));
			CtInvoke.ControlVisible(btnEdit, mUserLv > AccessLevel.Operator && mRecipeLoaded && mode == UiMode.None);
			CtInvoke.ControlVisible(btnEditFinish, mode != UiMode.None);
			CtInvoke.ControlVisible(btnRenew, mode > UiMode.None);
			CtInvoke.ControlVisible(btnSave, mode >= UiMode.EditingDefault);
			CtInvoke.ControlVisible(btnDelete, mode == UiMode.EditingDefault || mode == UiMode.Saved);
			CtInvoke.ControlVisible(btnDownload, mRecipeLoaded && mode == UiMode.None);
			CtInvoke.ControlEnabled(txtID, mode == UiMode.EditingDefault || mode == UiMode.EditingNew);
			CtInvoke.ControlEnabled(txtComment, mode == UiMode.EditingDefault || mode == UiMode.EditingNew);
		}


		/// <summary>搜尋所有Recipe，並顯示在ListBox裡</summary>
		private void SearchRecipe() {
			listFile.InvokeIfNecessary(
				() => {
					listFile.Items.Clear();
					if (mUserLv > AccessLevel.Operator) {
						string title = string.Empty;
						if (Thread.CurrentThread.CurrentUICulture.Name.Contains("en")) title = mNewStr[UILanguage.English];
						else title = mNewStr[UILanguage.TraditionalChinese];
						listFile.Items.Add(title);
					}
					foreach (string file in Directory.GetFileSystemEntries(mFileFolder, RECIPE_EXTENSION)) {
						listFile.Items.Add(CtFile.GetFileName(file, false));
					}
				}
			);
		}

		/// <summary>載入Recipe檔案，並顯示在DataGridView上</summary>
		/// <param name="fileName">Recipe檔案名稱，不含路徑與附檔名。如: D1403_001</param>
		/// <returns>Status Code</returns>
		private Stat LoadRecipe(string fileName) {
			Stat stt = Stat.SUCCESS;
			string path = CtFile.BackSlash(mFileFolder) + RECIPE_EXTENSION.Replace("*", fileName);
			string cmt = string.Empty;
			List<string> strDGV = new List<string>();
			try {
				mRecipeLoaded = false;

				if (CtFile.IsFileExist(path)) {

					mOriName = fileName;

					mRecipe.Clear();

					/*-- 更新ID --*/
					CtInvoke.ControlText(txtID, fileName);

					/*-- 載入XML --*/
					XmlElmt rcpXml = CtXML.Load(path);

					/*-- 註解 --*/
					cmt = rcpXml.Element("Comment")?.Value;
					if (!string.IsNullOrEmpty(cmt)) {
						CtInvoke.ControlText(txtComment, cmt);
					}

					/*-- Recipe Info --*/
					mCurrInfo = new RecipeInfo(fileName, cmt, path);

					/*-- 抓取內容 --*/
					List<XmlElmt> data = rcpXml.Element("Data").Elements();
					if (data == null || data.Count == 0) {
						stt = Stat.ER_SYS_ILLVAL;
						throw new ArgumentNullException("IXmlData", "節點數量錯誤，可能為非法或不完全的檔案");
					}

					XmlAttr attr;
					foreach (XmlElmt item in data) {
						if (item.Attribute("Device", out attr)) {
							switch (attr.Value) {
								//case "ACE":
								//	mRecipe.Add(new AceVPlusRecipe(item));
								//	break;
								//case "ACE_NUM":
								//	mRecipe.Add(new AceNumRecipe(item));
								//	break;
								//case "ACE_VIS":
								//	mRecipe.Add(new AceVisionRecipe(item));
								//	break;
								//case "BECKHOFF":
								//	mRecipe.Add(new BeckhoffRecipe(item));
								//	break;
								//case "DELTA_PLC":
								//	mRecipe.Add(new DeltaPlcRecipe(item));
								//	break;
								default:
									break;
							}
						}
					}

					List<DataGridViewRow> rowColl = mRecipe.ConvertAll(
						rcp => {
							DataGridViewRow row = new DataGridViewRow();
							row.CreateCells(dgvRecipe);
							row.Cells[0].Value = rcp.Name;
							row.Cells[1].Value = rcp.EncodeValue();
							row.Cells[2].Value = rcp.Comment;
							if (!rcp.BackgroundColor.IsEmpty) row.DefaultCellStyle.BackColor = rcp.BackgroundColor;
							else row.DefaultCellStyle.BackColor = SystemColors.Window;
							return row;
						}
					);

					dgvRecipe.InvokeIfNecessary(
						() => {
							dgvRecipe.Rows.Clear();
							dgvRecipe.Rows.AddRange(rowColl.ToArray());
						}
					);

					mRecipeLoaded = true;
				} else {
					stt = Stat.ER_SYS_NOFILE;
					throw (new Exception("找無對應檔案，請檢查檔案是否正確"));
				}

			} catch (Exception ex) {
				if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		/// <summary>搜尋相對應的Recipe</summary>
		/// <param name="name">Recipe名稱</param>
		/// <returns>最相符的RecipeData</returns>
		private ICtRecipe FindRecipe(string name) {
			return mRecipe.Find(data => data.Name == name) ?? mEmptyRecipe.Find(data => data.Name == name);
		}

		/// <summary>存檔至檔案</summary>
		/// <returns>Status Code</returns>
		private Stat SaveToXML() {
			Stat stt = Stat.SUCCESS;
			CtProgress prog = new CtProgress("存檔", "存檔中，請稍後...", false);
			try {

				/*-- 檢查 ID 是不是有輸入 --*/
				if (string.IsNullOrEmpty(txtID.Text)) {
					stt = Stat.ER_SYS_INVARG;
					throw new OperationCanceledException("請先設定對應的 Recipe 名稱");
				}

				/*-- 檢查是不是所有數值都有了 --*/
				if (!dgvRecipe.Rows.Cast<DataGridViewRow>().All(val => val.Cells[1].Value != null && !string.IsNullOrEmpty(val.Cells[1].Value.ToString()))) {
					stt = Stat.ER_SYS_INVARG;
					throw new OperationCanceledException("尚有數值為空，無法進行存檔");
				}

				/*-- 檢查是不是已經存在此檔案，如果有就跳視窗詢問是否覆蓋 --*/
				string path = string.Empty;
				MsgBoxBtn mbResult = MsgBoxBtn.Yes;

				if ((mOriName != "") && (txtID.Text != mOriName)) {
					mbResult = CtMsgBox.Show("更改檔名", "是否更改檔名？\r\n\r\n(是)更改檔名\r\n(否)另存新檔", MsgBoxBtn.YesNo, MsgBoxStyle.Question);
					if (mbResult == MsgBoxBtn.Yes) {
						path = CtFile.BackSlash(mFileFolder) + RECIPE_EXTENSION.Replace("*", mOriName);
						CtFile.DeleteFile(path);
					}
					mbResult = MsgBoxBtn.Yes;
				}

				path = CtFile.BackSlash(mFileFolder) + RECIPE_EXTENSION.Replace("*", txtID.Text);
				if (CtFile.IsFileExist(path)) {
					mbResult = CtMsgBox.Show("覆寫", "此檔案已存在，是否覆寫？\r\n\r\n(是)覆蓋原始檔案\r\n(否)取消存檔", MsgBoxBtn.YesNo, MsgBoxStyle.Question);
				}

				if (mbResult == MsgBoxBtn.Yes) {

					mCurrInfo = new RecipeInfo(txtID.Text, txtComment.Text, path);

					prog.Start();

					/*-- 開新檔案，覆蓋掉原始檔案 --*/
					int idx = 0;
					List<IXmlData> rcpColl = new List<IXmlData>();
					dgvRecipe.InvokeIfNecessary(
						() => {
							foreach (DataGridViewRow row in dgvRecipe.Rows) {
								ICtRecipe rcp = FindRecipe(row.Cells[0].Value.ToString());
								if (rcp != null) {
									rcp.SetValue(row.Cells[1].Value.ToString());
									rcpColl.Add(rcp.CreateXmlData(string.Format("RECIPE{0:D3}", idx++)));
								}
							}
						}
					);

					XmlElmt root = new XmlElmt(
						"CASTEC_RECIPE",
						string.Empty,
						new XmlElmt("Comment", (string.IsNullOrEmpty(txtComment.Text) ? DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff") : txtComment.Text)),
						new XmlElmt("Data", string.Empty, rcpColl)
					);

					CtXML.Save(root, path);

					/*-- 更新完就重新掃 Recipe，因為可能有新增或移除檔案 --*/
					SearchRecipe();
					mChanged = false;
				}
			} catch (Exception ex) {
				if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex, true);
			} finally {
				prog.Close();
			}
			return stt;
		}

		/// <summary>取得當前數值，並更新於DataGridView上</summary>
		/// <param name="data">RecipeData之集合</param>
		private void GetCurrentValue(List<ICtRecipe> data) {
			CtProgress prog = new CtProgress("更新資料", "更新中，請稍後...", true);
			try {
				List<DataGridViewRow> rowColl = new List<DataGridViewRow>();
				foreach (ICtRecipe item in data) {
					//if (item is AceVPlusRecipe) {
					//	AceVPlusRecipe rcp = item as AceVPlusRecipe;
					//	rcp.ReadValue(rAce[rcp.DeviceIndex]);
					//} else if (item is AceNumRecipe) {
					//	AceNumRecipe rcp = item as AceNumRecipe;
					//	rcp.ReadValue(rAce[rcp.DeviceIndex]);
					//} else if (item is AceVisionRecipe) {
					//	AceVisionRecipe rcp = item as AceVisionRecipe;
					//	rcp.ReadValue(rAce[rcp.DeviceIndex]);
					//} else if (item is BeckhoffRecipe) {
					//	BeckhoffRecipe rcp = item as BeckhoffRecipe;
					//	rcp.ReadValue(rBkf[rcp.DeviceIndex]);
					//}

					DataGridViewRow row = new DataGridViewRow();
					row.CreateCells(dgvRecipe);
					row.Cells[0].Value = item.Name;
					row.Cells[1].Value = item.EncodeValue();
					row.Cells[2].Value = item.Comment;
					if (!item.BackgroundColor.IsEmpty) row.DefaultCellStyle.BackColor = item.BackgroundColor;
					else row.DefaultCellStyle.BackColor = SystemColors.Window;
					rowColl.Add(row);
				}

				/*-- 更新至DataGridView --*/
				dgvRecipe.InvokeIfNecessary(
					() => {
						dgvRecipe.Rows.Clear();
						dgvRecipe.Rows.AddRange(rowColl.ToArray());
					}
				);

				/*-- 限制一定要存檔後才可以寫入設備 --*/
				mWrtieToEquip = false;

			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			} finally {
				prog.Close();
			}
		}

		/// <summary>依照RecipeData寫入設備</summary>
		private Stat WrtieToEquipment() {
			Stat stt = Stat.SUCCESS;
			int step = 0;

			if (!mRecipe.All(rcp => rcp.Value != null && !string.IsNullOrEmpty(rcp.Value.ToString()))) {
				CtMsgBox.Show("空白的資料", "尚有資料為空，請檢查後再進行載入！");
				return Stat.ER_SYS_INVARG;
			}

			CtProgress prog = new CtProgress(ProgBarStyle.Percent, "寫入參數", "正在寫入...", mRecipe.Count, true);
			try {
				if (mCurrInfo == null) mCurrInfo = new RecipeInfo(txtID.Text, txtComment.Text);
				else mCurrInfo.SetLoadTime();

				foreach (ICtRecipe item in mRecipe) {
					//if (item is AceVPlusRecipe) {
					//	AceVPlusRecipe rcp = item as AceVPlusRecipe;
					//	rcp.WriteValue(rAce[rcp.DeviceIndex]);
					//} else if (item is AceNumRecipe) {
					//	AceNumRecipe rcp = item as AceNumRecipe;
					//	rcp.WriteValue(rAce[rcp.DeviceIndex]);
					//} else if (item is AceVisionRecipe) {
					//	AceVisionRecipe rcp = item as AceVisionRecipe;
					//	rcp.WriteValue(rAce[rcp.DeviceIndex]);
					//} else if (item is BeckhoffRecipe) {
					//	BeckhoffRecipe rcp = item as BeckhoffRecipe;
					//	rcp.WriteValue(rBkf[rcp.DeviceIndex]);
					//}

					prog.UpdateStep(++step);
				}
			} catch (Exception ex) {
				if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			} finally {
				prog.Close();
				if (stt == Stat.SUCCESS) {
					mWrtieToEquip = true;
					CtMsgBox.Show("寫入設備", "各參數已成功寫入設備！", MsgBoxBtn.OK, MsgBoxStyle.Information);
				}
			}
			return stt;
		}

		private void LoadEmptyRecipe() {
			/*-- 建立空 Recipe，讓 "更新" 可按 --*/
			CtInvoke.ControlVisible(btnRenew, true);

			/*-- 清除相關資料 --*/
			mOriName = "";
			mWrtieToEquip = false;
			CtInvoke.DataGridViewClear(dgvRecipe);
			CtInvoke.ControlText(txtComment, string.Empty);
			CtInvoke.ControlText(txtID, string.Empty);

			/*-- 建立 DataGridView --*/
			List<DataGridViewRow> rowColl = new List<DataGridViewRow>();
			mRecipe.Clear();
			foreach (ICtRecipe item in mEmptyRecipe) {
				DataGridViewRow row = new DataGridViewRow();
				row.CreateCells(dgvRecipe);
				row.Cells[0].Value = item.Name;
				row.Cells[1].Value = string.Empty;
				row.Cells[2].Value = item.Comment;
				if (!item.BackgroundColor.IsEmpty) row.DefaultCellStyle.BackColor = item.BackgroundColor;
				else row.DefaultCellStyle.BackColor = SystemColors.Window;
				rowColl.Add(row);
				mRecipe.Add(item);
			}

			dgvRecipe.BeginInvokeIfNecessary(
				() => {
					dgvRecipe.Rows.Clear();
					dgvRecipe.Rows.AddRange(rowColl.ToArray());
				}
			);
		}
		#endregion

		#region Function - Interface Events

		/// <summary>ListBox點擊，檢查有沒有更新，然後載入點擊之檔案</summary>
		private void listFile_DoubleClick(object sender, EventArgs e) {
			if (listFile.SelectedIndex > -1) {
				MsgBoxBtn mbResult = MsgBoxBtn.No;
				if (mChanged) {
					mbResult = CtMsgBox.Show("檔案變更", "更改尚未儲存，放棄並離開？", MsgBoxBtn.YesNo, MsgBoxStyle.Question);
				}

				if ((!mChanged) || (mbResult == MsgBoxBtn.Yes)) {
					if (listFile.SelectedIndex == 0 && mNewStr.ContainsValue(listFile.SelectedItem.ToString())) {
						LoadEmptyRecipe();
						GetCurrentValue(mRecipe);
						tsk_UiMode(UiMode.EditingNew);
					} else {
						LoadRecipe(listFile.SelectedItem.ToString());
						mChanged = false;
						tsk_UiMode(UiMode.None);
					}
				}
			}
		}

		/// <summary>刪除</summary>
		private void btnDelete_Click(object sender, EventArgs e) {
			string path = CtFile.BackSlash(mFileFolder) + RECIPE_EXTENSION.Replace("*", txtID.Text);
			MsgBoxBtn mbResult = MsgBoxBtn.Yes;
			if (CtFile.IsFileExist(path)) {
				mbResult = CtMsgBox.Show("刪除", "確定要刪除 " + txtID.Text + " ？", MsgBoxBtn.YesNo, MsgBoxStyle.Question);
			}

			if (mbResult == MsgBoxBtn.Yes) {
				CtFile.DeleteFile(path);
				SearchRecipe();
				CtMsgBox.Show("刪除", "已刪除檔案");
			}

			CtInvoke.DataGridViewClear(dgvRecipe);
			tsk_UiMode(UiMode.None);
		}

		/// <summary>更新現在的數值，並更新在DataGridView</summary>
		private void btnRenew_Click(object sender, EventArgs e) {
			GetCurrentValue(mRecipe);
		}

		/// <summary>寫入設備，檢查有無更新並詢問後，依照RecipeData寫入</summary>
		private void btnDownload_Click(object sender, EventArgs e) {
			if (mChanged) {
				if (CtMsgBox.Show("存檔", "尚未儲存檔案，是否先存檔？", MsgBoxBtn.YesNo, MsgBoxStyle.Question) == MsgBoxBtn.Yes) {
					SaveToXML();
				}
			}

			WrtieToEquipment();
			tsk_UiMode(UiMode.None);
		}

		/// <summary>限制只能輸入數字</summary>
		/// <remarks>目前是沒有啟用，如果有需要請把 property NumericOnly = true</remarks>
		private void txtID_KeyPress(object sender, KeyPressEventArgs e) {
			if ((char.IsDigit(e.KeyChar)) || (char.IsControl(e.KeyChar)))
				e.Handled = false;
			else
				e.Handled = true;
		}

		/// <summary>Cell編輯事件，將Flag設為True表示有更改</summary>
		private void dgvRecipe_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e) {
			if (mUserLv < AccessLevel.Engineer) {
				e.Cancel = true;
			} else {
				mChanged = true;
			}
		}

		/// <summary>存檔</summary>
		private void btnSave_Click(object sender, EventArgs e) {
			SaveToXML();
			tsk_UiMode(UiMode.None);
		}

		private void btnExit_Click(object sender, EventArgs e) {
			MsgBoxBtn btn = MsgBoxBtn.No;
			if (mChanged) {
				btn = CtMsgBox.Show("離開", "尚有資料未儲存，是否要自動儲存？\r\n\r\n(是)儲存後離開\r\n(否)放棄資料，直接離開\r\n(取消)不離開視窗", MsgBoxBtn.YesNo | MsgBoxBtn.Cancel, MsgBoxStyle.Question);
			}

			if (btn == MsgBoxBtn.Cancel) return;
			if (btn == MsgBoxBtn.Yes) btnSave.PerformClick();

			if (mWrtieToEquip) DialogResult = DialogResult.OK;
			else DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void CtRecipe_VisibleChanged(object sender, EventArgs e) {
			if (Visible && this.Parent != null) {
				CtInvoke.ControlVisible(btnExit, false);
			}
		}

		private void btnEdit_Click(object sender, EventArgs e) {
			tsk_UiMode(UiMode.EditingDefault);
		}

		private void btnEditFinish_Click(object sender, EventArgs e) {
			MsgBoxBtn btn = MsgBoxBtn.No;
			if (mChanged) {
				btn = CtMsgBox.Show("離開編輯", "尚未儲存檔案，是否要儲存修改資料後離開？\r\n(是)存檔後再離開\r\n(否)捨棄資料並離開\r\n(取消)取消離開動作", MsgBoxBtn.YesNo | MsgBoxBtn.Cancel, MsgBoxStyle.Question);
			}

			if (btn == MsgBoxBtn.Yes) {
				btnSave.PerformClick();
				tsk_UiMode(UiMode.None);
			} else if (btn == MsgBoxBtn.No)
				tsk_UiMode(UiMode.None);
		}

		#endregion
	}
}
