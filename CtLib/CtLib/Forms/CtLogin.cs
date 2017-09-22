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
    /// 使用者登入視窗
    /// <para>可讓使用者輸入帳號與密碼，並與 UserManage 檔案檢查是否存在該筆帳號與密碼是否正確</para>
    /// </summary>
    /// <example><code>
    /// UserData usr;
    /// CtLogin login = new CtLogin();
    /// 
    /// Stat stt = login.Start(out usr);
    /// MessageBox.Show("使用者: " + usr.Account + "  權限: " + Enum.GetName(typeof(AccessLevel), usr.Level));
    /// login.Dispose();
    /// </code></example>
    public partial class CtLogin : Form {

        #region Version

        /// <summary>CtLogin 版本訊息</summary>
        /// <remarks><code>
        /// 1.0.0  Ahern [2014/09/13]
        ///     + 完成基礎介面，建議使用Start方法來啟動並直接獲取帳號
        ///     
        /// 1.0.1  Ahern [2014/09/14]
        ///     \ 補上按下Enter將會觸發驗證 (使用 AcceptButton 屬性)
        /// </code></remarks>
        public static CtVersion @Version = new CtVersion(1, 0, 1, "2014/09/14", "Ahern Kuo");

        #endregion

        #region Declaration - Support Class

        /// <summary>
        /// 紀錄使用者相關資料，包含帳號、密碼、權限等資訊
        /// </summary>
        public class UserData {
            /// <summary>帳號</summary>
            public string Account { get; set; }
            /// <summary>密碼</summary>
            public string Password { get; set; }

            /// <summary>使用者等級</summary>
            public AccessLevel Level { get; set; }

            /// <summary>建立日期</summary>
            public DateTime BuiltTime { get; set; }

            /// <summary>由誰建立</summary>
            public string Creator { get; set; }

            /// <summary>建立一全空的資料，請手動指定屬性</summary>
            public UserData() {
                Account = "N/A";
                Password = "";
                Level = AccessLevel.OPERATOR;
            }

            /// <summary>建立一包含預設值之使用者資料</summary>
            public UserData(string usrAccount, string usrPassword, AccessLevel usrLevel) {
                Account = usrAccount;
                Password = usrPassword;
                Level = usrLevel;
                BuiltTime = DateTime.Now;
                Creator = LOGIN_ADMIN_ACCOUNT;
            }

            /// <summary>建立一包含預設值之使用者資料</summary>
            public UserData(string usrAccount, string usrPassword, AccessLevel usrLevel, DateTime usrTime, string usrBuilt) {
                Account = usrAccount;
                Password = usrPassword;
                Level = usrLevel;
                BuiltTime = usrTime;
                Creator = usrBuilt;
            }
        }

        #endregion

        #region Declaration - Enumerations

        /// <summary>使用者層級</summary>
        public enum AccessLevel : byte {
            /// <summary>CASTEC 管理員</summary>
            /// <remarks>包含軟體工程師、客服等</remarks>
            ADMINISTRATOR = 0,
            /// <summary>[客戶端] 工程師</summary>
            ENGINEER = 1,
            /// <summary>[客戶端] 操作員</summary>
            OPERATOR = 2,
            /// <summary>尚未登入，或是已登出</summary>
            NONE = 3
        }

        /// <summary>決定讀/寫檔案時之檔案內容順序</summary>
        /// <remarks>建議可以混放，這樣加上AES256加密比較不容易被破解</remarks>
        public enum UserDataSequence : byte {
            /// <summary>使用者資料檔 之 帳號名稱存放順序</summary>
            ACCOUNT = 2,
            /// <summary>使用者資料檔 之 密碼存放順序</summary>
            PASSWORD = 1,
            /// <summary>使用者資料檔 之 等級存放順序</summary>
            ACCESS_LEVEL = 4,
            /// <summary>使用者資料檔 之 建立日期存放順序</summary>
            BUILT_DATE = 0,
            /// <summary>使用者資料檔 之 建立者存放順序</summary>
            CREATOR = 3
        }
        #endregion

        #region Declaration - Definitions

        /// <summary>CASTEC 預設帳號</summary>
        private const string LOGIN_ADMIN_ACCOUNT = "CASTEC";
        /// <summary>CASTEC 預設帳號之密碼</summary>
        private const string LOGIN_ADMIN_PASSWORD = "27635744";

        #endregion

        #region Declaration - Members

        /// <summary>使用者資料</summary>
        /// <remarks>為了讓Start/Show/ShowDialog都可以彈性化，所以把最後的結果丟到全域</remarks>
        private UserData mUserData;
        /// <summary>取得登入狀態</summary>
        /// <remarks>為了讓Start/Show/ShowDialog都可以彈性化，所以把最後的結果丟到全域</remarks>
        private Stat mStt = Stat.SUCCESS;

        #endregion

        #region Function - Constructor

        /// <summary>
        /// 紀錄使用者相關資料
        /// <para>欲顯示並讓使用者操作建議用 "Start" 方法，可直接取得回傳值</para>
        /// <para>如自行使用 "Show"/"ShowDialog" 方法，請自行取得mStt與mUserData</para>
        /// </summary>
        public CtLogin() {
            InitializeComponent();
        }
        #endregion

        #region Function - Method

        /// <summary>
        /// 顯示CtLogin並在使用者完成操作後回傳使用者資料，外部請檢查Stt回傳值
        /// <para>使用者取消 = WN_SYS_USRCNC ; 成功登入 = SUCCESS ; 帳號錯誤 = ER_SYS_USRACN ; 密碼錯誤 = ER_SYS_USRPWD</para>
        /// </summary>
        /// <param name="usrData">如有成功登入，回傳正確帳號資料，否則回傳空的UserData(怕回傳null會出事)</param>
        /// <returns>
        /// 使用者取消 = WN_SYS_USRCNC ; 成功登入 = SUCCESS ; 帳號錯誤 = ER_SYS_USRACN ; 密碼錯誤 = ER_SYS_USRPWD
        /// <para>另有其他系統錯誤等</para>
        /// </returns>
        public Stat Start(out UserData usrData) {
            /*-- 顯示介面，並等待使用者操作完畢 --*/
            this.ShowDialog();

            /*-- 如果取消或是登入失敗，回傳一全空的資料 (因怕回傳null不知道會不會跳Exception)--*/
            if (mUserData == null) mUserData = new UserData();

            /*-- 回傳 --*/
            usrData = mUserData;
            return mStt;
        }

        /// <summary>取得使用者資料檔，並將內部帳密資料輸出</summary>
        /// <param name="userData">已存檔之使用者資料檔</param>
        /// <returns>Status Code</returns>
        public Stat GetUserDataFile(out List<UserData> userData) {
            Stat stt = Stat.SUCCESS;
            List<UserData> tempData = new List<UserData>();
            try {
                /*-- 組合路徑，預設於Config資料夾裡 --*/
                string strPath = CtDefaultPath.GetPath(SystemPath.USER_MANAGER);

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
                                    new UserData(
                                        strSplit[(int)UserDataSequence.ACCOUNT],
                                        strSplit[(int)UserDataSequence.PASSWORD],
                                        (AccessLevel)(CtConvert.CByte(strSplit[(int)UserDataSequence.ACCESS_LEVEL])),
                                        DateTime.Parse(strSplit[(int)UserDataSequence.BUILT_DATE]),
                                        strSplit[(int)UserDataSequence.CREATOR]
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

        /// <summary>比對帳號與密碼，如果有對應的資料則回傳，否則回傳Null</summary>
        /// <param name="usrAcount">欲比對之帳號</param>
        /// <param name="usrPassword">欲比對之密碼</param>
        /// <param name="usrData">回傳之使用者資料</param>
        /// <returns>Status Code. 無相符帳號 = ER_SYS_USRACN, 密碼錯誤 = ER_SYS_USRPWD</returns>
        private Stat GetUserData(string usrAcount, string usrPassword, out UserData usrData) {
            Stat stt = Stat.SUCCESS;
            UserData tempData = null;

            /*-- 檢查是不是CASTEC人員 --*/
            if (usrAcount == LOGIN_ADMIN_ACCOUNT) {
                if (usrPassword == LOGIN_ADMIN_PASSWORD) {
                    stt = Stat.SUCCESS;
                    tempData = new UserData(
                                    LOGIN_ADMIN_ACCOUNT,
                                    LOGIN_ADMIN_PASSWORD,
                                    AccessLevel.ADMINISTRATOR,
                                    DateTime.Parse("2005/03/10"),
                                    LOGIN_ADMIN_ACCOUNT
                               );
                } else {
                    stt = Stat.ER_SYS_USRPWD;
                }
            } else {

                /*-- 取得使用者資料檔 --*/
                List<UserData> lstTemp;
                stt = GetUserDataFile(out lstTemp);

                /*-- 搜尋檔案內是否相對應的 "帳號" --*/
                tempData = lstTemp.Find(delegate(UserData data) { return data.Account == usrAcount; });

                /*-- 如果有找到帳號 --*/
                if (tempData != null) {
                    /* 比對密碼，如果密碼錯誤則更改Stt，然後回傳null離開 */
                    if (tempData.Password != usrPassword) {
                        stt = Stat.ER_SYS_USRPWD;
                        tempData = null;
                    }
                } else {
                    /* 如果找不到帳號，回傳離開 */
                    stt = Stat.ER_SYS_USRACN;
                }

            }
            usrData = tempData;
            return stt;
        }

        #endregion

        #region Function - Interface Events

        /// <summary>按下確認後，檢查輸入的資料</summary>
        private void btnOK_Click(object sender, EventArgs e) {
            try {
                /*-- 獲取輸入的帳號與密碼，取得後把介面上的訊息刪掉 --*/
                string usrAccount = txtAccount.Text;
                string usrPassword = txtPWD.Text;
                CtInvoke.TextBoxText(txtAccount, "");
                CtInvoke.TextBoxText(txtPWD, "");

                /*-- 搜尋是否有相符的帳號 --*/
                UserData usrData;
                mStt = GetUserData(usrAccount, usrPassword, out usrData);

                /*-- 將結果丟到全域變數 --*/
                mUserData = usrData;

                /*-- 根據搜尋的結果做不同的動作 --*/
                if (mStt == Stat.ER_SYS_USRACN) {
                    /* 找不到帳號，顯示錯誤訊息，並且丟Exception寫到Log裡 */
                    if (Thread.CurrentThread.CurrentUICulture.Name == "zh-TW") {
                        CtInvoke.LabelText(lbError, "無相符帳號資料!!");
                        CtInvoke.LabelVisible(lbError, true);
                    } else {
                        CtInvoke.LabelText(lbError, "No match acoount");
                        CtInvoke.LabelVisible(lbError, true);
                    }
                    throw (new Exception("使用者 " + usrAccount + "嘗試登入，但找無相對應帳號"));
                } else if (mStt == Stat.ER_SYS_USRPWD) {
                    /* 密碼錯誤，顯示錯誤訊息，並且丟Exception寫到Log裡 */
                    if (Thread.CurrentThread.CurrentUICulture.Name == "zh-TW") {
                        CtInvoke.LabelText(lbError, "密碼錯誤!!");
                        CtInvoke.LabelVisible(lbError, true);
                    } else {
                        CtInvoke.LabelText(lbError, "Invalid password");
                        CtInvoke.LabelVisible(lbError, true);
                    }
                    throw (new Exception("使用者 " + usrAccount + "嘗試登入，但密碼錯誤"));
                } else if (mStt == Stat.SUCCESS) {
                    /* 如果成功登入，關閉視窗，並讓後續接手 */
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            } catch (Exception ex) {
                if (mStt == Stat.SUCCESS) mStt = Stat.ER_SYSTEM;
                CtStatus.Report(mStt, "UserSignIn", ex.Message);
            }
        }

        /// <summary>使用者按下取消</summary>
        private void btnCancel_Click(object sender, EventArgs e) {
            /*-- 更改全域Stt --*/
            mStt = Stat.WN_SYS_USRCNC;

            /*-- 關閉視窗 --*/
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion


    }
}
