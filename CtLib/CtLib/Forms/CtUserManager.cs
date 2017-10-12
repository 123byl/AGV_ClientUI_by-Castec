using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Utility;

namespace CtLib.Forms {
	/// <summary>
	/// 使用者管理視窗
	/// <para>此視窗可用於建立、刪除、更改相關使用者的帳號、密碼與等級</para>
	/// </summary>
	/// <example>
	/// 此視窗僅用於修改檔案，不須回傳任何東西，故直接開啟即可
	/// <code language="C#">
	/// UserData usrData = new UserData("Demo", "0000", AccessLevel.Engineer);  //假設已有現存資料，可直接套用不須再 new
	/// CtUserManager usrMag = new CtUserManager(usrData);
	/// usrMag.ShowDialog();    //由使用者進行操作
	/// usrMag.Dispose();
	/// </code></example>
	public partial class CtUserManager : Form, ICtVersion {

		#region Version

		/// <summary>CtUserManager 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 1.0.0  Ahern [2014/09/13]
		///     + 完成基礎介面
		///     
		/// 1.0.1  Ahern [2015/02/11]
		///     + 刪除帳號時如有登入者，將訊息寫至 Report
		///     
		/// 1.1.0  Ahern [2015/05/14]
		///     + 區分新增與編輯狀態
		///     + 啟動時檢查當前登入者是否為工程師或友上人員
		///     
		/// 1.2.0  Ahern [2015/12/08]
		///		\ 使用者權限為 byte 全部
		///		\ 按鈕文字改以 Dictionary 方式表達
		///		+ 當前語系，供變更按鈕文字時使用
		///		+ ComboBox 權限表達方式改為 "Level 99" 方式，除原本的 Engineer、Operator 之外
		///		+ 使用其他權限時使用 CtInput 供使用者輸入
		/// 
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(1, 2, 0, "2015/12/08", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Fields

		/// <summary>是否要更新底下資訊。如在編輯/新增，則設為False</summary>
		private bool mAllowUpdate = true;
		/// <summary>儲存當前載入的使用者資料</summary>
		private List<UserData> mUserData = new List<UserData>();
		/// <summary>當前開啟者的資料</summary>
		private UserData mCurrUser;
		/// <summary>當前的語系</summary>
		/// <remarks>於 Jimmy 的 Raytheon 專案發現，介面開啟後有可能因 Task 或 Event 導致 Cultrue 不同(不同條 Thread)，故用此取代</remarks>
		private UILanguage mCulture = UILanguage.TraditionalChinese;
		/// <summary>權限等級描述文字</summary>
		private Dictionary<AccessLevel, Dictionary<UILanguage, string>> mDefLvStr = new Dictionary<AccessLevel, Dictionary<UILanguage, string>> {
			{ AccessLevel.Administrator, new Dictionary<UILanguage, string> { { UILanguage.English, "Administrator" }, { UILanguage.SimplifiedChinese, "管理员" }, { UILanguage.TraditionalChinese, "管理員" } } },
			{ AccessLevel.Engineer, new Dictionary<UILanguage, string> { { UILanguage.English, "Engineer" }, { UILanguage.SimplifiedChinese, "工程师" }, { UILanguage.TraditionalChinese, "工程師" } } },
			{ AccessLevel.None, new Dictionary<UILanguage, string> { { UILanguage.English, "N/A" }, { UILanguage.SimplifiedChinese, "无" }, { UILanguage.TraditionalChinese, "無" } } },
			{ AccessLevel.Operator, new Dictionary<UILanguage, string> { { UILanguage.English, "Operator" }, { UILanguage.SimplifiedChinese, "操作员" }, { UILanguage.TraditionalChinese, "作業員" } } },
		};
		/// <summary>編輯按鈕文字</summary>
		private Dictionary<UILanguage, string> mDefEditStr = new Dictionary<UILanguage, string> { { UILanguage.English, "Edit" }, { UILanguage.SimplifiedChinese, "编辑" }, { UILanguage.TraditionalChinese, "編輯" } };
		/// <summary>刪除按鈕文字</summary>
		private Dictionary<UILanguage, string> mDefDeleteStr = new Dictionary<UILanguage, string> { { UILanguage.English, "Delete" }, { UILanguage.SimplifiedChinese, "删除" }, { UILanguage.TraditionalChinese, "刪除" } };
		/// <summary>存檔按鈕文字</summary>
		private Dictionary<UILanguage, string> mDefSaveStr = new Dictionary<UILanguage, string> { { UILanguage.English, "Save" }, { UILanguage.SimplifiedChinese, "存档" }, { UILanguage.TraditionalChinese, "存檔" } };
		/// <summary>取消按鈕文字</summary>
		private Dictionary<UILanguage, string> mDefCancelStr = new Dictionary<UILanguage, string> { { UILanguage.English, "Cancel" }, { UILanguage.SimplifiedChinese, "取消" }, { UILanguage.TraditionalChinese, "取消" } };
		#endregion

		#region Function - Method

		/// <summary>取得權限等級文字</summary>
		/// <param name="level">層級</param>
		/// <returns>權限等級字串</returns>
		private string GetLevelString(AccessLevel level) {
			string strTemp = string.Empty;
			if (Enum.IsDefined(typeof(AccessLevel), level))
				strTemp = mDefLvStr[level][mCulture];
			else
				strTemp = string.Format("Level {0}", (byte)level);
			return strTemp;
		}

		/// <summary>根據文字取得相對應的權限層級</summary>
		/// <param name="lvStr">層級文字</param>
		/// <returns>權限層級</returns>
		private AccessLevel GetAccessLevel(string lvStr) {
			bool found = false;
			AccessLevel level = AccessLevel.None;

			foreach (KeyValuePair<AccessLevel, Dictionary<UILanguage, string>> kvp in mDefLvStr) {
				foreach (KeyValuePair<UILanguage, string> item in kvp.Value) {
					if (lvStr == item.Value) {
						found = true;
						level = kvp.Key;
						break;
					}
				}
				if (found) break;
			}

			if (!found && lvStr.StartsWith("Level "))
				level = (AccessLevel)byte.Parse(lvStr.Replace("Level ", string.Empty));

			return level;
		}

		/// <summary>取得使用者資料檔，並將內部帳密資料輸出</summary>
		/// <param name="userData">已存檔之使用者資料檔</param>
		/// <returns>Status Code</returns>
		/// <remarks>原本想直接把CtLogin變成Static就可以直接使用，但是感覺... 不妥?! 所以還是重新複製一次吧!</remarks>
		public Stat GetUserDataFile(out List<UserData> userData) {
			Stat stt = Stat.SUCCESS;
			List<UserData> tempData = new List<UserData>();
			try {
				/*-- 組合路徑，預設於Config資料夾裡 --*/
				string strPath = CtDefaultPath.GetPath(SystemPath.UserManagement);

				/*-- 檢查是否有該檔案 --*/
				if (CtFile.IsFileExist(strPath)) {
					/* 讀取檔案內容 */
					List<string> strFile = CtFile.ReadFile(strPath);

					/* 分析檔案內容 */
					string strDecode = string.Empty;
					foreach (string strDoc in strFile) {

						/*__ 內容使用AES 256加密，故取得檔案後先解密 __*/
						CtCrypto.Decrypt(CryptoMode.AES256, strDoc, out strDecode);

						/*__ 如果解密成功，則分析之 __*/
						if (stt == Stat.SUCCESS) {

							//使用「=」分段解密後字串
							string[] strSplit = strDecode.Split(CtConst.CHR_EQUAL, StringSplitOptions.RemoveEmptyEntries);

							//照理說應該會有五段資料(帳、密、等級、日期、建造者)，如果不對表示資料有誤
							if (strSplit.Length == 5) {
								//如果資料正確，將資料存進去List裡
								tempData.Add(
									new UserData(
										strSplit[(int)UserDataSequence.Account],
										strSplit[(int)UserDataSequence.Password],
										(AccessLevel)(CtConvert.CByte(strSplit[(int)UserDataSequence.AccessLevel])),
										DateTime.Parse(strSplit[(int)UserDataSequence.BuiltDate]),
										strSplit[(int)UserDataSequence.Creator]
									)
								);
							} else {
								//此等級較嚴重，故需throw exception
								stt = Stat.ER_SYS_INVARG;
								throw (new Exception("使用者資料被更改！無法載入"));
							}
						} else {
							/*__ 由於有可能只是空檔案，所以這裡沒有throw excpetion __*/
							stt = Stat.ER_SYS_NOFILE;
						}
					}
				} else
					/*-- 由於有可能使用者還沒有建過任何帳號，所以這邊只有改Stt，沒有throw exception --*/
					stt = Stat.ER_SYS_NOFILE;

			} catch (Exception ex) {
				if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			userData = tempData;
			return stt;
		}

		/// <summary>載入使用者資料檔，並將之顯示在DataGridView上</summary>
		/// <returns>Status Code</returns>
		private Stat LoadUserData() {
			Stat stt = Stat.SUCCESS;
			try {
				/*-- 清空DataGridView與UserData，避免完成後重複輸出 --*/
				mUserData.Clear();
				CtInvoke.DataGridViewClear(dgvData);

				/*-- 載入檔案 --*/
				stt = GetUserDataFile(out mUserData);


				/*-- 如果取得成功，將之顯示在DataGridView上 --*/
				if ((stt == Stat.SUCCESS) && (mUserData != null)) {
					foreach (UserData data in mUserData) {
						CtInvoke.DataGridViewAddRow(
							dgvData,
							new List<string> {
								data.Account,
								GetLevelString(data.Level),
								data.BuiltTime.ToString("yyyy/MM/dd HH:mm"),
								data.Creator
							},
							false,
							false
						);
						CheckComboBoxLevel(data.Level, false);
					}
					/* 將 DataGridView 切到第一列 */
					if (mUserData.Count > 0) {
						dgvData.Rows[0].Selected = true;
					}
				}
			} catch (Exception ex) {
				if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		/// <summary>
		/// 變更介面狀態
		/// <para>0 = 預設 ; 1 = 點擊後 ; 2 = 新增 ; 3 = 編輯中</para>
		/// </summary>
		/// <param name="stt">樣式選項。0 = 預設 ; 1 = 點擊後 ; 2 = 新增 ; 3 = 編輯中</param>
		private void UpdateControl(byte stt) {
			switch (stt) {
				case 0: /* 預設 */
					mAllowUpdate = true;
					CtInvoke.ControlEnabled(dgvData, true);
					CtInvoke.ControlText(txtAccount, string.Empty);
					CtInvoke.ControlVisible(lbAccount, true);
					CtInvoke.ControlVisible(lbAccLv, true);
					CtInvoke.ControlVisible(lbPWD1, false);
					CtInvoke.ControlVisible(lbPWD2, false);
					CtInvoke.ControlEnabled(txtAccount, false);
					CtInvoke.ControlEnabled(cbAccLv, false);
					CtInvoke.ControlVisible(txtPWD1, false);
					CtInvoke.ControlVisible(txtPWD2, false);
					CtInvoke.ControlEnabled(btnAdd, true);
					CtInvoke.ControlEnabled(btnEdit, false);
					CtInvoke.ControlEnabled(btnDelete, false);
					CtInvoke.ControlText(btnEdit, mDefEditStr[mCulture]);
					CtInvoke.ButtonImage(btnEdit, Properties.Resources.Edit);
					CtInvoke.ControlTag(btnEdit, "Edit");
					CtInvoke.ControlText(btnDelete, mDefDeleteStr[mCulture]);
					CtInvoke.ControlTag(btnDelete, "Delete");
					break;

				case 1: /* 顯示點擊的訊息 */
					mAllowUpdate = true;
					CtInvoke.ControlEnabled(dgvData, true);
					CtInvoke.ControlVisible(lbAccount, true);
					CtInvoke.ControlVisible(lbAccLv, true);
					CtInvoke.ControlVisible(lbPWD1, false);
					CtInvoke.ControlVisible(lbPWD2, false);
					CtInvoke.ControlEnabled(txtAccount, false);
					CtInvoke.ControlEnabled(cbAccLv, false);
					CtInvoke.ControlVisible(txtPWD1, false);
					CtInvoke.ControlVisible(txtPWD2, false);
					CtInvoke.ControlEnabled(btnAdd, true);
					CtInvoke.ControlEnabled(btnEdit, true);
					CtInvoke.ControlEnabled(btnDelete, true);
					CtInvoke.ControlText(btnEdit, mDefEditStr[mCulture]);
					CtInvoke.ButtonImage(btnEdit, Properties.Resources.Edit);
					CtInvoke.ControlTag(btnEdit, "Edit");
					CtInvoke.ControlText(btnDelete, mDefDeleteStr[mCulture]);
					CtInvoke.ControlTag(btnDelete, "Delete");
					break;

				case 2: /* 新增中 */
					mAllowUpdate = false;
					CtInvoke.ControlEnabled(dgvData, false);
					CtInvoke.ControlText(txtPWD1, string.Empty);
					CtInvoke.ControlText(txtPWD2, string.Empty);
					CtInvoke.ControlVisible(lbAccount, true);
					CtInvoke.ControlVisible(lbAccLv, true);
					CtInvoke.ControlVisible(lbPWD1, true);
					CtInvoke.ControlVisible(lbPWD2, true);
					CtInvoke.ControlEnabled(txtAccount, true);
					CtInvoke.ControlEnabled(cbAccLv, true);
					CtInvoke.ControlVisible(txtPWD1, true);
					CtInvoke.ControlVisible(txtPWD2, true);
					CtInvoke.ControlEnabled(btnAdd, false);
					CtInvoke.ControlEnabled(btnEdit, true);
					CtInvoke.ControlEnabled(btnDelete, true);
					CtInvoke.ControlText(btnEdit, mDefSaveStr[mCulture]);
					CtInvoke.ButtonImage(btnEdit, Properties.Resources.Save_2);
					CtInvoke.ControlTag(btnEdit, "Save");
					CtInvoke.ControlText(btnDelete, mDefCancelStr[mCulture]);
					CtInvoke.ControlTag(btnDelete, "Cancel");
					break;

				case 3: /* 編輯中 */
					mAllowUpdate = false;
					CtInvoke.ControlEnabled(dgvData, false);
					CtInvoke.ControlText(txtPWD1, string.Empty);
					CtInvoke.ControlText(txtPWD2, string.Empty);
					CtInvoke.ControlVisible(lbAccount, true);
					CtInvoke.ControlVisible(lbAccLv, true);
					CtInvoke.ControlVisible(lbPWD1, true);
					CtInvoke.ControlVisible(lbPWD2, true);
					CtInvoke.ControlEnabled(txtAccount, false);
					CtInvoke.ControlEnabled(cbAccLv, true);
					CtInvoke.ControlVisible(txtPWD1, true);
					CtInvoke.ControlVisible(txtPWD2, true);
					CtInvoke.ControlEnabled(btnAdd, false);
					CtInvoke.ControlEnabled(btnEdit, true);
					CtInvoke.ControlEnabled(btnDelete, true);
					CtInvoke.ControlText(btnEdit, mDefSaveStr[mCulture]);
					CtInvoke.ButtonImage(btnEdit, Properties.Resources.Save_2);
					CtInvoke.ControlTag(btnEdit, "Save");
					CtInvoke.ControlText(btnDelete, mDefCancelStr[mCulture]);
					CtInvoke.ControlTag(btnDelete, "Cancel");
					break;
			}
		}

		/// <summary>切換 ComboBox 顯示的層級，如果是未在清單上的，會自動新增</summary>
		/// <param name="lv">欲切換的層級</param>
		/// <param name="switcher">完成後是否強制切換 ComboBox 到此項目</param>
		private void CheckComboBoxLevel(AccessLevel lv, bool switcher = true) {
			if (!Enum.IsDefined(typeof(AccessLevel), lv)) {
				string lvStr = string.Format("Level {0}", ((byte)lv).ToString());
				if (!cbAccLv.Items.Contains(lvStr)) CtInvoke.ComboBoxAdd(cbAccLv, lvStr);
				if (switcher) CtInvoke.ControlText(cbAccLv, lvStr);
			} else if (switcher) {
				if (lv == AccessLevel.Engineer) CtInvoke.ControlText(cbAccLv, "Engineer");
				else CtInvoke.ControlText(cbAccLv, "Operator");
			}
		}

		/// <summary>儲存UserData至使用者檔案</summary>
		/// <param name="usrData">欲儲存之使用者資料</param>
		/// <returns>Status Code</returns>
		private Stat SaveUserData(List<UserData> usrData) {
			Stat stt = Stat.SUCCESS;
			try {
				if (usrData != null) {
					/*-- 用於儲存要寫入檔案之字串 --*/
					List<string> strData = new List<string>();
					/*-- 用於儲存依順序排入之資料 --*/
					string[] strCombine = new string[5];
					/*-- 用於組合strComnine之所有字串 --*/
					string strTemp = string.Empty;
					/*-- 讀取資料 --*/
					foreach (UserData data in usrData) {
						/*-- 依照順序寫至strCombine --*/
						strCombine[(byte)UserDataSequence.Account] = data.Account;
						strCombine[(byte)UserDataSequence.Password] = data.Password;
						strCombine[(byte)UserDataSequence.AccessLevel] = CtConvert.CStr((byte)data.Level);
						strCombine[(byte)UserDataSequence.BuiltDate] = data.BuiltTime.ToString("yyyy/MM/dd HH:mm");
						strCombine[(byte)UserDataSequence.Creator] = data.Creator;

						/*-- 組合字串 --*/
						strTemp = string.Join("=", strCombine);

						/*-- 加密 --*/
						CtCrypto.Encrypt(CryptoMode.AES256, ref strTemp);

						/*-- 加入至欲寫入檔案之集合 --*/
						strData.Add(strTemp);
					}
					/*-- 寫入檔案 --*/
					CtFile.WriteFile(CtDefaultPath.GetPath(SystemPath.UserManagement), strData);
					LoadUserData();
				}
			} catch (Exception ex) {
				if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
				CtStatus.Report(stt, ex);
			} finally {
				/*-- 不管怎樣，都恢復預設介面 --*/
				UpdateControl(0);
			}
			return stt;
		}

		#endregion

		#region Function - Constructors
		/// <summary>建立使用者管理介面，帶入CtProject以獲取當前相關系統路徑</summary>
		/// <param name="lang">欲顯示的語系</param>
		public CtUserManager(UILanguage lang) {
			InitializeComponent();

			mCulture = lang;
		}

		/// <summary>
		/// 建立使用者管理介面
		/// <para>帶入CtProject以獲取當前相關系統路徑</para>
		/// <para>另如目前有登入，帶入使用者以便在操作時紀錄</para>
		/// </summary>
		/// <param name="user">當前登入之使用者，用於新增或刪除帳號時紀錄是誰刪除的</param>
		/// <param name="lang">欲顯示的語系</param>
		public CtUserManager(UserData user, UILanguage lang) {
			InitializeComponent();

			mCurrUser = user;
			mCulture = lang;

			if (mCurrUser.Level <= AccessLevel.Operator) {
				CtInvoke.ControlVisible(btnAdd, false);
				CtInvoke.ControlVisible(btnEdit, false);
				CtInvoke.ControlVisible(btnDelete, false);
			}
		}
		#endregion

		#region Function - Interface Event

		/// <summary>使用者點擊某一列，更新資訊於下方TextBox等</summary>
		private void dgvData_RowEnter(object sender, DataGridViewCellEventArgs e) {
			/*-- 確認當前是否可以更新 --*/
			if (mAllowUpdate && dgvData.SelectedRows.Count > 0) {

				DataGridViewRow row = dgvData.SelectedRows[0];

				/*-- 顯示帳號 --*/
				CtInvoke.ControlText(txtAccount, row.Cells[0].Value.ToString());

				/*-- 顯示層級 --*/
				CheckComboBoxLevel(GetAccessLevel(row.Cells[1].Value.ToString()));

				/*-- 顯示模式 --*/
				UpdateControl(1);
			}
		}

		/// <summary>新增使用者，將TextBox清空讓使用者輸入</summary>
		private void btnAdd_Click(object sender, EventArgs e) {
			/*-- 清空相關資訊 --*/
			CtInvoke.ControlText(txtAccount, string.Empty);
			CtInvoke.ControlText(txtPWD1, string.Empty);
			CtInvoke.ControlText(txtPWD2, string.Empty);
			CtInvoke.ComboBoxSelectedIndex(cbAccLv, 1);

			/*-- 更新介面 --*/
			UpdateControl(2);
		}

		/// <summary>刪除或取消編輯</summary>
		private void btnDelete_Click(object sender, EventArgs e) {

			if (CtConvert.CStr(btnDelete.Tag) == "Cancel") {
				/*-- 取消編輯，將資料清空 --*/
				CtInvoke.ControlText(txtAccount, string.Empty);
				/*-- 介面恢復預設 --*/
				UpdateControl(0);
			} else if (dgvData.SelectedRows[0] != null) {
				/*-- 刪除，取得帳號 --*/
				string usrAccount = dgvData.SelectedRows[0].Cells[0].Value.ToString();
				/*-- 詢問是否刪除 --*/
				if (CtMsgBox.Show("刪除", "是否刪除 " + usrAccount + "?", MsgBoxBtn.YesNo, MsgBoxStyle.Warning) == MsgBoxBtn.Yes) {
					/*-- 確認刪除則找到相對應的mUserData，並將之移除 --*/
					UserData usrData = mUserData.Find(usr => usr.Account == usrAccount);
					if (usrData != null) {
						mUserData.Remove(usrData);
						CtStatus.Report(Stat.SUCCESS, "Account", string.Format("帳號 {0} 已經被 {1} 刪除", usrAccount, (mCurrUser == null ? "Unknown" : mCurrUser.Account)));
					}
				}
				/*-- 動作後儲存檔案並重新載入 --*/
				SaveUserData(mUserData);
			}
		}

		/// <summary>離開</summary>
		private void btnExit_Click(object sender, EventArgs e) {
			this.Close();
		}

		/// <summary>編輯或儲存</summary>
		private void btnEdit_Click(object sender, EventArgs e) {
			try {
				/*-- 介面變更為編輯模式 --*/
				if (CtConvert.CStr(btnEdit.Tag) == "Edit") {
					mAllowUpdate = false;
					UpdateControl(3);
				} else {
					string account = txtAccount.Text;
					string pwd1 = txtPWD1.Text;
					string pwd2 = txtPWD2.Text;
					AccessLevel lv = GetAccessLevel(cbAccLv.Text);

					/*-- 儲存 --*/
					if (pwd1.Equals(pwd2)) {
						UserData usrData = mUserData.Find(usr => usr.Account == account);
						if (usrData != null) {
							if (!string.IsNullOrEmpty(pwd1)) usrData.Password = pwd1;
							usrData.Level = lv;
						} else if (!string.IsNullOrEmpty(pwd1)) {
							mUserData.Add(
								new UserData(
									account,
									pwd1,
									lv,
									DateTime.Now,
									(mCurrUser != null) ? mCurrUser.Account : "Unknown"
								)
							);
						} else {
							CtMsgBox.Show(
								"錯誤",
								"密碼請勿為空!",
								MsgBoxBtn.OK,
								MsgBoxStyle.Error
							);
						}

						/*-- 動作後儲存並重新載入檔案 --*/
						SaveUserData(mUserData);

					} else if (string.IsNullOrEmpty(pwd1) || string.IsNullOrEmpty(pwd2)) {
						CtMsgBox.Show(
							"錯誤",
							"密碼不可為空!" + Environment.NewLine + "請重新輸入",
							MsgBoxBtn.OK,
							MsgBoxStyle.Error
						);
						CtInvoke.ControlText(txtPWD1, string.Empty);
						CtInvoke.ControlText(txtPWD2, string.Empty);

					} else {
						CtMsgBox.Show(
							"錯誤",
							"兩格密碼不相同!" + Environment.NewLine + "請重新輸入",
							MsgBoxBtn.OK,
							MsgBoxStyle.Error
						);
						CtInvoke.ControlText(txtPWD1, string.Empty);
						CtInvoke.ControlText(txtPWD2, string.Empty);
					}

				}
			} catch (Exception ex) {
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
		}

		/// <summary>介面載入</summary>
		private void CtUserManager_Load(object sender, EventArgs e) {
			/*-- 載入檔案 --*/
			LoadUserData();
			/*-- 變更介面為預設 --*/
			UpdateControl(0);
		}

		private void cbAccLv_SelectedIndexChanged(object sender, EventArgs e) {
			if (cbAccLv.SelectedIndex == 2) {
				string result = string.Empty;
				List<string> list = new List<string>();
				for (int idx = 3; idx < byte.MaxValue; idx++) {
					list.Add(string.Format("Level {0}", idx.ToString()));
				}
				if (CtInput.ComboBoxList(out result, "自訂等級", "請選擇自訂等級", list) == Stat.SUCCESS) {
					CheckComboBoxLevel(GetAccessLevel(result));
				}
			}
		}

		#endregion
	}
}
