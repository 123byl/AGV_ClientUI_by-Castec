using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Ultity;

namespace CtLib.Forms {
    /// <summary>
    /// 使用者管理視窗
    /// <para>此視窗可用於建立、刪除、更改相關使用者的帳號、密碼與等級</para>
    /// </summary>
    /// <example>
    /// 此視窗僅用於修改檔案，不須回傳任何東西，故直接開啟即可
    /// <code>
    /// CtLogin.UserData usrData = new CtLogin.UserData("Demo", "0000", AccessLevel.ENGINEER);  //假設已有現存資料，可直接套用不須再 new
    /// CtUserManager usrMag = new CtUserManager(usrData);
    /// usrMag.ShowDialog();    //由使用者進行操作
    /// usrMag.Dispose();
    /// </code></example>
    public partial class CtUserManager : Form {

        #region Version

        /// <summary>CtUserManager 版本訊息</summary>
        /// <remarks><code>
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
        /// </code></remarks>
        public static CtVersion @Version = new CtVersion(1, 1, 0, "2015/05/14", "Ahern Kuo");

        #endregion

        #region Declaration - Members

        /// <summary>是否要更新底下資訊。如在編輯/新增，則設為False</summary>
        private bool mAllowUpdate = true;
        /// <summary>儲存當前載入的使用者資料</summary>
        private List<CtLogin.UserData> mUserData = new List<CtLogin.UserData>();
        /// <summary>當前開啟者的資料</summary>
        private CtLogin.UserData mCurrUser;

        #endregion

        #region Function - Method

        /// <summary>取得權限等級文字</summary>
        /// <param name="level">層級</param>
        /// <returns>權限等級字串</returns>
        private string GetLevelString(CtLogin.AccessLevel level) {
            string strTemp = "";
            switch (level) {
                case CtLogin.AccessLevel.ADMINISTRATOR:
                    strTemp = "Administrator";
                    break;
                case CtLogin.AccessLevel.ENGINEER:
                    strTemp = "Engineer";
                    break;
                case CtLogin.AccessLevel.OPERATOR:
                    strTemp = "Operator";
                    break;
                case CtLogin.AccessLevel.NONE:
                    strTemp = "N/A";
                    break;
            }
            return strTemp;
        }

        /// <summary>根據文字取得相對應的權限層級</summary>
        /// <param name="lvStr">層級文字</param>
        /// <returns>權限層級</returns>
        private CtLogin.AccessLevel GetLevel(string lvStr) {
            CtLogin.AccessLevel level = CtLogin.AccessLevel.NONE;
            switch (lvStr) {
                case "Engineer":
                    level = CtLogin.AccessLevel.ENGINEER;
                    break;
                case "Operator":
                    level = CtLogin.AccessLevel.OPERATOR;
                    break;
                default:
                    level = CtLogin.AccessLevel.NONE;
                    break;
            }
            return level;
        }

        /// <summary>取得使用者資料檔，並將內部帳密資料輸出</summary>
        /// <param name="userData">已存檔之使用者資料檔</param>
        /// <returns>Status Code</returns>
        /// <remarks>原本想直接把CtLogin變成Static就可以直接include，但是因為路徑都在CtProjects裡，mProject不應變成static，故於此重新複製一次</remarks>
        public Stat GetUserDataFile(out List<CtLogin.UserData> userData) {
            Stat stt = Stat.SUCCESS;
            List<CtLogin.UserData> tempData = new List<CtLogin.UserData>();
            try {
                /*-- 組合路徑，預設於Config資料夾裡 --*/
                string strPath = Properties.Settings.Default.PATH_MAIN + "\\" + Properties.Settings.Default.FOLD_CONFIG + "\\" + Properties.Settings.Default.FILE_USERMANAGE;

                /*-- 檢查是否有該檔案 --*/
                if (CtFile.IsFileExist(strPath)) {
                    /* 讀取檔案內容 */
                    List<string> strFile = CtFile.ReadFile(strPath);

                    /* 分析檔案內容 */
                    string strDecode = "";
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
                                    new CtLogin.UserData(
                                        strSplit[(int)CtLogin.UserDataSequence.ACCOUNT],
                                        strSplit[(int)CtLogin.UserDataSequence.PASSWORD],
                                        (CtLogin.AccessLevel)(CtConvert.CByte(strSplit[(int)CtLogin.UserDataSequence.ACCESS_LEVEL])),
                                        DateTime.Parse(strSplit[(int)CtLogin.UserDataSequence.BUILT_DATE]),
                                        strSplit[(int)CtLogin.UserDataSequence.CREATOR]
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
                CtStatus.Report(stt, "GetUserData", ex.Message);
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
                    foreach (CtLogin.UserData data in mUserData) {
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
                    }
                }
            } catch (Exception ex) {
                if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, "LoadUserData", ex.Message);
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
                    CtInvoke.LabelVisible(lbAccount, true);
                    CtInvoke.LabelVisible(lbAccLv, true);
                    CtInvoke.LabelVisible(lbPWD1, false);
                    CtInvoke.LabelVisible(lbPWD2, false);
                    CtInvoke.TextBoxEnable(txtAccount, false);
                    CtInvoke.ComboBoxEnable(cbAccLv, false);
                    CtInvoke.TextBoxVisible(txtPWD1, false);
                    CtInvoke.TextBoxVisible(txtPWD2, false);
                    CtInvoke.ButtonEnable(btnAdd, true);
                    CtInvoke.ButtonEnable(btnEdit, false);
                    CtInvoke.ButtonEnable(btnDelete, false);
                    CtInvoke.ButtonText(btnEdit, (Thread.CurrentThread.CurrentUICulture.Name == "zh-TW") ? "編輯" : "Edit");
                    CtInvoke.ButtonImage(btnEdit, Properties.Resources.Edit);
                    CtInvoke.ButtonTag(btnEdit, "Edit");
                    CtInvoke.ButtonText(btnDelete, (Thread.CurrentThread.CurrentUICulture.Name == "zh-TW") ? "刪除" : "Delete");
                    CtInvoke.ButtonTag(btnDelete, "Delete");
                    break;

                case 1: /* 顯示點擊的訊息 */
                    mAllowUpdate = true;
                    CtInvoke.LabelVisible(lbAccount, true);
                    CtInvoke.LabelVisible(lbAccLv, true);
                    CtInvoke.LabelVisible(lbPWD1, false);
                    CtInvoke.LabelVisible(lbPWD2, false);
                    CtInvoke.TextBoxEnable(txtAccount, false);
                    CtInvoke.ComboBoxEnable(cbAccLv, false);
                    CtInvoke.TextBoxVisible(txtPWD1, false);
                    CtInvoke.TextBoxVisible(txtPWD2, false);
                    CtInvoke.ButtonEnable(btnAdd, true);
                    CtInvoke.ButtonEnable(btnEdit, true);
                    CtInvoke.ButtonEnable(btnDelete, true);
                    CtInvoke.ButtonText(btnEdit, (Thread.CurrentThread.CurrentUICulture.Name == "zh-TW") ? "編輯" : "Edit");
                    CtInvoke.ButtonImage(btnEdit, Properties.Resources.Edit);
                    CtInvoke.ButtonTag(btnEdit, "Edit");
                    CtInvoke.ButtonText(btnDelete, (Thread.CurrentThread.CurrentUICulture.Name == "zh-TW") ? "刪除" : "Delete");
                    CtInvoke.ButtonTag(btnDelete, "Delete");
                    break;

                case 2: /* 新增中 */
                    mAllowUpdate = false;
                    CtInvoke.TextBoxText(txtPWD1, "");
                    CtInvoke.TextBoxText(txtPWD2, "");
                    CtInvoke.LabelVisible(lbAccount, true);
                    CtInvoke.LabelVisible(lbAccLv, true);
                    CtInvoke.LabelVisible(lbPWD1, true);
                    CtInvoke.LabelVisible(lbPWD2, true);
                    CtInvoke.TextBoxEnable(txtAccount, true);
                    CtInvoke.ComboBoxEnable(cbAccLv, true);
                    CtInvoke.TextBoxVisible(txtPWD1, true);
                    CtInvoke.TextBoxVisible(txtPWD2, true);
                    CtInvoke.ButtonEnable(btnAdd, false);
                    CtInvoke.ButtonEnable(btnEdit, true);
                    CtInvoke.ButtonEnable(btnDelete, true);
                    CtInvoke.ButtonText(btnEdit, (Thread.CurrentThread.CurrentUICulture.Name == "zh-TW") ? "存檔" : "Save");
                    CtInvoke.ButtonImage(btnEdit, Properties.Resources.Save_2);
                    CtInvoke.ButtonTag(btnEdit, "Save");
                    CtInvoke.ButtonText(btnDelete, (Thread.CurrentThread.CurrentUICulture.Name == "zh-TW") ? "取消" : "Cancel");
                    CtInvoke.ButtonTag(btnDelete, "Cancel");
                    break;

                case 3: /* 編輯中 */
                    mAllowUpdate = false;
                    CtInvoke.TextBoxText(txtPWD1, "");
                    CtInvoke.TextBoxText(txtPWD2, "");
                    CtInvoke.LabelVisible(lbAccount, true);
                    CtInvoke.LabelVisible(lbAccLv, true);
                    CtInvoke.LabelVisible(lbPWD1, true);
                    CtInvoke.LabelVisible(lbPWD2, true);
                    CtInvoke.TextBoxEnable(txtAccount, false);
                    CtInvoke.ComboBoxEnable(cbAccLv, true);
                    CtInvoke.TextBoxVisible(txtPWD1, true);
                    CtInvoke.TextBoxVisible(txtPWD2, true);
                    CtInvoke.ButtonEnable(btnAdd, false);
                    CtInvoke.ButtonEnable(btnEdit, true);
                    CtInvoke.ButtonEnable(btnDelete, true);
                    CtInvoke.ButtonText(btnEdit, (Thread.CurrentThread.CurrentUICulture.Name == "zh-TW") ? "存檔" : "Save");
                    CtInvoke.ButtonImage(btnEdit, Properties.Resources.Save_2);
                    CtInvoke.ButtonTag(btnEdit, "Save");
                    CtInvoke.ButtonText(btnDelete, (Thread.CurrentThread.CurrentUICulture.Name == "zh-TW") ? "取消" : "Cancel");
                    CtInvoke.ButtonTag(btnDelete, "Cancel");
                    break;
            }
        }

        /// <summary>儲存UserData至使用者檔案</summary>
        /// <param name="usrData">欲儲存之使用者資料</param>
        /// <returns>Status Code</returns>
        private Stat SaveUserData(List<CtLogin.UserData> usrData) {
            Stat stt = Stat.SUCCESS;
            try {
                if (usrData != null) {
                    /*-- 用於儲存要寫入檔案之字串 --*/
                    List<string> strData = new List<string>();
                    /*-- 用於儲存依順序排入之資料 --*/
                    string[] strCombine = new string[5];
                    /*-- 用於組合strComnine之所有字串 --*/
                    string strTemp = "";
                    /*-- 取得各資料順序 --*/
                    byte seqAccount = (byte)CtLogin.UserDataSequence.ACCOUNT;
                    byte seqPassword = (byte)CtLogin.UserDataSequence.PASSWORD;
                    byte seqAccessLv = (byte)CtLogin.UserDataSequence.ACCESS_LEVEL;
                    byte seqButDate = (byte)CtLogin.UserDataSequence.BUILT_DATE;
                    byte seqCreator = (byte)CtLogin.UserDataSequence.CREATOR;

                    /*-- 讀取資料 --*/
                    foreach (CtLogin.UserData data in usrData) {
                        /*-- 依照順序寫至strCombine --*/
                        strCombine[seqAccount] = data.Account;
                        strCombine[seqPassword] = data.Password;
                        strCombine[seqAccessLv] = CtConvert.CStr((byte)data.Level);
                        strCombine[seqButDate] = data.BuiltTime.ToString("yyyy/MM/dd HH:mm");
                        strCombine[seqCreator] = data.Creator;

                        /*-- 組合字串 --*/
                        strTemp = "";
                        for (int i = 0; i < strCombine.Length; i++) {
                            strTemp += (i < strCombine.Length - 1) ? strCombine[i] + "=" : strCombine[i];
                        }

                        /*-- 加密 --*/
                        CtCrypto.Encrypt(CryptoMode.AES256, ref strTemp);

                        /*-- 加入至欲寫入檔案之集合 --*/
                        strData.Add(strTemp);
                    }
                    /*-- 寫入檔案 --*/
                    CtFile.WriteFile(CtDefaultPath.GetPath(SystemPath.USER_MANAGER), strData);
                    LoadUserData();
                }
            } catch (Exception ex) {
                if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, "SaveUsrData", ex.Message);
            } finally {
                /*-- 不管怎樣，都恢復預設介面 --*/
                UpdateControl(0);
            }
            return stt;
        }

        #endregion

        #region Function - Constructors
        /// <summary>建立使用者管理介面，帶入CtProject以獲取當前相關系統路徑</summary>
        public CtUserManager() {
            InitializeComponent();
        }

        /// <summary>
        /// 建立使用者管理介面
        /// <para>帶入CtProject以獲取當前相關系統路徑</para>
        /// <para>另如目前有登入，帶入使用者以便在操作時紀錄</para>
        /// </summary>
        /// <param name="user">當前登入之使用者，用於新增或刪除帳號時紀錄是誰刪除的</param>
        public CtUserManager(CtLogin.UserData user) {
            InitializeComponent();

            mCurrUser = user;

            if (mCurrUser.Level >= CtLogin.AccessLevel.OPERATOR) {
                CtInvoke.ButtonVisible(btnAdd, false);
                CtInvoke.ButtonVisible(btnEdit, false);
                CtInvoke.ButtonVisible(btnDelete, false);
            }
        }
        #endregion

        #region Function - Interface Event

        /// <summary>使用者點擊某一列，更新資訊於下方TextBox等</summary>
        private void dgvData_RowEnter(object sender, DataGridViewCellEventArgs e) {
            /*-- 確認當前是否可以更新 --*/
            if (mAllowUpdate) {

                /*-- 顯示模式 --*/
                UpdateControl(1);

                /*-- 顯示帳號 --*/
                CtInvoke.TextBoxText(txtAccount, dgvData.Rows[e.RowIndex].Cells[0].Value.ToString());

                /*-- 顯示層級 --*/
                if (dgvData.Rows[e.RowIndex].Cells[1].Value.ToString() == GetLevelString(CtLogin.AccessLevel.ENGINEER))
                    CtInvoke.ComboBoxSelectedIndex(cbAccLv, 0);
                else
                    CtInvoke.ComboBoxSelectedIndex(cbAccLv, 1);
            }
        }

        /// <summary>新增使用者，將TextBox清空讓使用者輸入</summary>
        private void btnAdd_Click(object sender, EventArgs e) {
            /*-- 清空相關資訊 --*/
            CtInvoke.TextBoxText(txtAccount, "");
            CtInvoke.TextBoxText(txtPWD1, "");
            CtInvoke.TextBoxText(txtPWD2, "");
            CtInvoke.ComboBoxSelectedIndex(cbAccLv, 1);

            /*-- 更新介面 --*/
            UpdateControl(2);
        }

        /// <summary>刪除或取消編輯</summary>
        private void btnDelete_Click(object sender, EventArgs e) {

            if (CtConvert.CStr(btnDelete.Tag) == "Cancel") {
                /*-- 取消編輯，將資料清空 --*/
                CtInvoke.TextBoxText(txtAccount, "");
                /*-- 介面恢復預設 --*/
                UpdateControl(0);
            } else {
                /*-- 刪除，取得帳號 --*/
                string usrAccount = dgvData.Rows[dgvData.CurrentRow.Index].Cells[0].Value.ToString();
                /*-- 詢問是否刪除 --*/
                if (CtMsgBox.Show("刪除", "是否刪除 " + usrAccount + "?", MsgBoxButton.YES_NO, MsgBoxStyle.WARNING) == MsgBoxButton.YES) {
                    /*-- 確認刪除則找到相對應的mUserData，並將之移除 --*/
                    foreach (CtLogin.UserData data in mUserData) {
                        if (data.Account == usrAccount) {
                            mUserData.Remove(data);
                            break;
                        }
                    }
                    CtStatus.Report(Stat.SUCCESS, "Account", "帳號 " + usrAccount + " 已經被 " + ((mCurrUser == null) ? "Unknown User" : mCurrUser.Account) + " 刪除");
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
                    /*-- 儲存 --*/
                    if (txtPWD1.Text == txtPWD2.Text) {
                        /*-- 檢查現在輸入的帳號存不存在 --*/
                        bool isExist = false;
                        foreach (CtLogin.UserData data in mUserData) {
                            if (data.Account == txtAccount.Text) {
                                /* 如果存在，則更新之 */
                                if (txtPWD1.Text != "") data.Password = txtPWD1.Text;
                                data.Level = GetLevel(cbAccLv.Text);

                                /* 讓後面知道這邊已經更新 */
                                isExist = true;
                                break;
                            }
                        }

                        /*-- 如果前面沒有更新到表示新增 --*/
                        if (!isExist) {
                            /* 確認有輸入密碼 */
                            if (txtPWD1.Text != "") {
                                mUserData.Add(
                                    new CtLogin.UserData(
                                        txtAccount.Text,
                                        txtPWD1.Text,
                                        GetLevel(cbAccLv.Text),
                                        DateTime.Now,
                                        (mCurrUser != null) ? mCurrUser.Account : "Unknown"
                                    )
                                );
                            } else {
                                CtMsgBox.Show(
                                    "錯誤",
                                    "密碼請勿為空!",
                                    MsgBoxButton.OK,
                                    MsgBoxStyle.ERROR
                                );
                            }
                        }
                        /*-- 動作後儲存並重新載入檔案 --*/
                        SaveUserData(mUserData);

                    } else if (txtPWD1.Text == "" || txtPWD2.Text == "") {
                        CtMsgBox.Show(
                            "錯誤",
                            "密碼不可為空!" + Environment.NewLine + "請重新輸入",
                            MsgBoxButton.OK,
                            MsgBoxStyle.ERROR
                        );
                        CtInvoke.TextBoxText(txtPWD1, "");
                        CtInvoke.TextBoxText(txtPWD2, "");

                    } else {
                        CtMsgBox.Show(
                            "錯誤",
                            "兩格密碼不相同!" + Environment.NewLine + "請重新輸入",
                            MsgBoxButton.OK,
                            MsgBoxStyle.ERROR
                        );
                        CtInvoke.TextBoxText(txtPWD1, "");
                        CtInvoke.TextBoxText(txtPWD2, "");
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
        #endregion
    }
}
