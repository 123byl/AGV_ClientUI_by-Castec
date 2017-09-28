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
using CtLib.Module.Adept;
using CtLib.Module.Beckhoff;
using CtLib.Module.Ultity;

namespace CtLib.Forms {

    /// <summary>
    /// Recipe 介面
    /// <para>請使用 "Start" 方法來開起此視窗</para>
    /// <para>目前如果是要儲存 Adept ACE 相關路徑，如 <see cref="Ace.HSVision.Server.Tools.ILocatorTool"/> 所使用之 <seealso cref="Ace.HSVision.Server.Tools.ILocatorModel"/> 等路徑，請將類別設為 <see cref="Devices.CAMPro"/> 而非 <see cref="Devices.ADEPT_ACE"/></para>
    /// </summary>
    public partial class CtRecipe : Form {

        #region Version

        /// <summary>CtRecipe 版本訊息</summary>
        /// <remarks><code>
        /// 1.0.0  Ahern [2015/02/05]
        ///     + 從 CAMPro 搬移至 CtLib
        ///     
        /// 1.1.0  Ahern [2015/05/26]
        ///     \ 寫入設備鈕，限定存檔後才可儲存
        ///     \ 權限 ReadOnly 相反
        ///     + RecipeInfo，Start 後將最後載入的 Recipe 資訊丟出去，並加上 Stt 判斷是否有寫入設備
        /// 
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 1, 0, "2015/05/26", "Ahern Kuo");

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

        #region Declaration - Members

        /// <summary>[Ref] 已建立之CtAce</summary>
        private List<CtAce> rAce = new List<CtAce>();
        /// <summary>[Ref] 已建立之CtBeckhoff</summary>
        private List<CtBeckhoff> rBkf = new List<CtBeckhoff>();

        /// <summary>處理XML相關事項</summary>
        private CtXML mXML = new CtXML();

        /// <summary>原始Recipe資料，即於Opcode階段所引入之Recipe</summary>
        private List<CtRecipeData> mEmptyRecipe;
        /// <summary>當前載入之Recipe資料</summary>
        private List<CtRecipeData> mRecipe = new List<CtRecipeData>();
        /// <summary>暫存當前所載入的 Recipe 資訊</summary>
        private RecipeInfo mCurrInfo;

        /// <summary>是否有被更改過</summary>
        private bool mChanged = false;
        /// <summary>紀錄是否有載入設備</summary>
        private bool mLoaded = false;
        /// <summary>Recipe ID 是否僅能輸入數字</summary>
        private bool mNumOnly = false;

        /// <summary>儲存點取載入時之檔案名稱</summary>
        private string mOriName = "";
        /// <summary>Recipe 之 檔案路徑</summary>
        private string mFileFolder = CtDefaultPath.GetPath(SystemPath.RECIPE);

        #endregion

        #region Function - Constructor
        /// <summary>建立 CtRecipe，如要啟動請使用 "Start" 方法</summary>
        /// <param name="emptyRecipe">空的 Recipe Data，用於建立新 Recipe 或是初始化時</param>
        public CtRecipe(List<CtRecipeData> emptyRecipe) {
            InitializeComponent();

            mEmptyRecipe = emptyRecipe.ToList();
        }
        #endregion

        #region Declaration - Suport Class
        /// <summary>儲存 Recipe 相關資訊</summary>
        public struct RecipeInfo {
            /// <summary>Recipe 存檔時間</summary>
            public DateTime BuildTime;
            /// <summary>寫入設備之時間</summary>
            public DateTime LoadTime;
            /// <summary>此 Recipe 名稱</summary>
            public string Name;
            /// <summary>此 Recipe 註解</summary>
            public string Comment;
            /// <summary>此 Recipe 完整檔案名稱</summary>
            public string Path;
        }
        #endregion

        #region Function - Core
        /// <summary>將已建立的 CtAce 加入至欄位中</summary>
        /// <param name="ace">CtAce 集合</param>
        public void AssignAce(params CtAce[] ace) {
            if (rAce == null) rAce = new List<CtAce>();
            rAce.AddRange(ace);
        }

        /// <summary>將已建立的 CtBeckhoff 加入至欄位中</summary>
        /// <param name="bkf">CtBeckhoff 集合</param>
        public void AssignBeckhoff(params CtBeckhoff[] bkf) {
            if (rBkf == null) rBkf = new List<CtBeckhoff>();
            rBkf.AddRange(bkf);
        }

        /// <summary>
        /// 開啟 Recipe 視窗。請先確認已使用 AssignACE / AssignBeckhoff 等方法加入相關物件
        /// <para>如果帶有 currentRecipe 之引數，則會自動更新當前項目，然後先填好名稱(未存檔)，方便後續操作</para>
        /// </summary>
        /// <param name="lv">登入者的權限等級，目前僅有 Administrator 可以直接修改數值</param>
        /// <param name="recipe">欲儲存與修改的 Recipe Data</param>
        /// <param name="currentRecipe">當前的 Recipe 名稱，只用於開啟時是否先行更新與顯示而已。保持空字串則不自動更新與填入</param>
        /// <remarks>目前限定 Administrator 才可以直接修改數值，其餘權限只能看不能改</remarks>
        public void Start(CtLogin.AccessLevel lv, ref List<CtRecipeData> recipe, string currentRecipe = "") {
            dgvRecipe.Columns[1].ReadOnly = (lv == CtLogin.AccessLevel.ADMINISTRATOR) ? false : true;
            SearchRecipe();

            if (currentRecipe != "") {
                CtInvoke.ButtonVisible(btnRenew, true);
                CtInvoke.DataGridViewClear(dgvRecipe);

                CtInvoke.TextBoxText(txtComment, "");
                CtInvoke.TextBoxText(txtID, "");

                List<string> strDGV = new List<string>();
                mRecipe.Clear();
                foreach (CtRecipeData item in mEmptyRecipe) {
                    strDGV.Clear();
                    strDGV.Add(item.Name);
                    strDGV.Add("");
                    strDGV.Add(item.Comment);
                    CtInvoke.DataGridViewAddRow(dgvRecipe, strDGV, false, false);

                    mRecipe.Add(item);
                }
                GetCurrentValue(mRecipe);
                CtInvoke.TextBoxText(txtID, currentRecipe);
            }

            this.ShowDialog();
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
        /// <remarks>目前限定 Administrator 才可以直接修改數值，其餘權限只能看不能改</remarks>
        public Stat Start(CtLogin.AccessLevel lv, ref List<CtRecipeData> recipe, out RecipeInfo recipeInfo, string currentRecipe = "") {
            dgvRecipe.Columns[1].ReadOnly = (lv == CtLogin.AccessLevel.ADMINISTRATOR) ? false : true;
            SearchRecipe();

            if (currentRecipe != "") {
                CtInvoke.ButtonVisible(btnRenew, true);
                CtInvoke.DataGridViewClear(dgvRecipe);

                CtInvoke.TextBoxText(txtComment, "");
                CtInvoke.TextBoxText(txtID, "");

                List<string> strDGV = new List<string>();
                mRecipe.Clear();
                foreach (CtRecipeData item in mEmptyRecipe) {
                    strDGV.Clear();
                    strDGV.Add(item.Name);
                    strDGV.Add("");
                    strDGV.Add(item.Comment);
                    CtInvoke.DataGridViewAddRow(dgvRecipe, strDGV, false, false);

                    mRecipe.Add(item);
                }
                GetCurrentValue(mRecipe);
                CtInvoke.TextBoxText(txtID, currentRecipe);
            }

            this.ShowDialog();
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
        public static Stat LoadRecipe(string recipeName, List<CtAce> ace = null, List<CtBeckhoff> bkf = null) {
            Stat stt = Stat.SUCCESS;
            string path = CtFile.BackSlash(CtDefaultPath.GetPath(SystemPath.RECIPE)) + RECIPE_EXTENSION.Replace("*", recipeName);
            string strTemp = "";
            try {
                if (CtFile.IsFileExist(path)) {

                    List<CtRecipeData> recipe = new List<CtRecipeData>();

                    /*-- 載入XML --*/
                    CtXML xml = new CtXML(path);

                    /*-- 註解 --*/
                    strTemp = xml.GetInnerText("CASTEC_RECIPE/Comment");

                    /*-- 抓取內容 --*/
                    List<CtXML.XmlData> data = xml.GetAllValue("CASTEC_RECIPE/Data");
                    if (data.Count < 1) {
                        stt = Stat.ER_SYS_ILLVAL;
                        throw (new Exception("Recipe檔案有誤"));
                    }

                    /*-- 將Recipe顯示到DataGridView上 --*/
                    foreach (CtXML.XmlData xmlData in data) {
                        if (CtXML.FindAttribute(xmlData.Attributes, "Device").Value.ToUpper() == "ACE") {
                            recipe.Add(
                                new CtRecipeData(
                                    Devices.ADEPT_ACE,
                                    xmlData.Name,
                                    xmlData.InnerText,
                                    (CtAce.VPlusVariableType)(CtConvert.CByte(CtXML.FindAttribute(xmlData.Attributes, "Type").Value)),
                                    CtXML.FindAttribute(xmlData.Attributes, "Comment").Value
                                )
                            );
                        } else if (CtXML.FindAttribute(xmlData.Attributes, "Device").Value.ToUpper() == "BECKHOFF") {
                            recipe.Add(
                                new CtRecipeData(
                                    Devices.BECKHOFF_PLC,
                                    xmlData.Name,
                                    xmlData.InnerText,
                                    (CtBeckhoff.SymbolType)(CtConvert.CByte(CtXML.FindAttribute(xmlData.Attributes, "Type").Value)),
                                    CtXML.FindAttribute(xmlData.Attributes, "Comment").Value
                                )
                            );
                        } else if (CtXML.FindAttribute(xmlData.Attributes, "Device").Value.ToUpper() == "CAMPRO") {
                            recipe.Add(
                                new CtRecipeData(
                                    Devices.CAMPro,
                                    xmlData.Name,
                                    xmlData.InnerText,
                                    CtXML.FindAttribute(xmlData.Attributes, "Comment").Value
                                )
                            );
                        }
                    }

                    foreach (CtRecipeData item in recipe) {
                        if (item.Source == Devices.ADEPT_ACE) {

                            switch (item.AceVarType) {
                                case CtAce.VPlusVariableType.REAL:
                                    ace[item.DeviceIndex].Variable.SetValue(item.Name, CtConvert.CFloat(item.Value));
                                    break;

                                case CtAce.VPlusVariableType.STRING:
                                    ace[item.DeviceIndex].Variable.SetValue(item.Name, CtConvert.CStr(item.Value));
                                    break;

                                case CtAce.VPlusVariableType.LOCATION:
                                    strTemp = item.Value.ToString();
                                    List<string> strSplit = strTemp.Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    if (strSplit.Count == 6) {
                                        List<double> dblSplit = strSplit.ConvertAll(val => CtConvert.CDbl(val));
                                        ace[item.DeviceIndex].Variable.SetValue(item.Name, dblSplit, CtAce.VPlusVariableType.LOCATION);
                                    }
                                    break;

                                case CtAce.VPlusVariableType.PRECISION_POINT:
                                    string strTempPP = item.Value.ToString();
                                    List<string> strSplitPP = strTempPP.Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    if ((strSplitPP.Count == 6) || (strSplitPP.Count == 4)) {
                                        List<double> dblSplit = strSplitPP.ConvertAll(val => CtConvert.CDbl(val));
                                        ace[item.DeviceIndex].Variable.SetValue(item.Name, dblSplit, CtAce.VPlusVariableType.PRECISION_POINT);
                                    }
                                    break;
                            }

                        } else if (item.Source == Devices.BECKHOFF_PLC) {
                            bkf[item.DeviceIndex].SetValue(item.Name, item.Value);
                        } else if (item.Source == Devices.CAMPro) {
                            ace[item.DeviceIndex].Vision.RemoveLocatorModel(item.Name);
                            ace[item.DeviceIndex].Vision.AddLocatorModel(item.Name, item.Value.ToString());
                        }
                    }

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
        #endregion

        #region Function - Methods

        /// <summary>建立一個新的XML檔案</summary>
        /// <param name="path">欲儲存新檔之路徑</param>
        private void CreateXML(string path) {
            List<string> strDoc = new List<string> {
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
                "<CASTEC_RECIPE>",
                "    <Comment> </Comment>",
                "    <Data>",
                "    <!-- ACE : (0)Real (1)String (2)Location (3)PrecisionPoint -->",
	            "    <!-- BECKHOFF : (2)INT (3)DINT (4)REAL (5)LREAL (16)SINT (17)USINT (18)UINT (19)UDINT (20)LINT (21)ULINT (30)STRING (33)BOOL (40)TIME (99)ARRAY_STRUC -->",
                "    <!-- CAMPro : VisionToolPath -->",
                "    </Data>",
                "</CASTEC_RECIPE>"
            };
            CtFile.WriteFile(path, strDoc);
        }

        /// <summary>搜尋所有Recipe，並顯示在ListBox裡</summary>
        private void SearchRecipe() {
            listFile.Items.Clear();
            foreach (string file in Directory.GetFileSystemEntries(mFileFolder, RECIPE_EXTENSION)) {
                listFile.Items.Add(CtFile.GetFileName(file, false));
            }
        }

        /// <summary>載入Recipe檔案，並顯示在DataGridView上</summary>
        /// <param name="fileName">Recipe檔案名稱，不含路徑與附檔名。如: D1403_001</param>
        /// <returns>Status Code</returns>
        private Stat LoadRecipe(string fileName) {
            Stat stt = Stat.SUCCESS;
            string path = CtFile.BackSlash(mFileFolder) + RECIPE_EXTENSION.Replace("*", fileName);
            string strTemp = "";
            List<string> strDGV = new List<string>();
            try {
                if (CtFile.IsFileExist(path)) {
                    mCurrInfo = new RecipeInfo();
                    mCurrInfo.Path = path;
                    mCurrInfo.BuildTime = CtFile.GetFileInformation(path).LastEditTime;

                    mOriName = fileName;

                    mRecipe.Clear();
                    CtInvoke.DataGridViewClear(dgvRecipe);

                    /*-- 更新ID --*/
                    CtInvoke.TextBoxText(txtID, fileName);
                    mCurrInfo.Name = fileName;

                    /*-- 載入XML --*/
                    mXML.Load(path);

                    /*-- 註解 --*/
                    strTemp = mXML.GetInnerText("CASTEC_RECIPE/Comment");
                    if (strTemp != "") {
                        CtInvoke.TextBoxText(txtComment, strTemp);
                        mCurrInfo.Comment = strTemp;
                    }

                    /*-- 抓取內容 --*/
                    List<CtXML.XmlData> data = mXML.GetAllValue("CASTEC_RECIPE/Data");
                    if (data.Count < 1) {
                        stt = Stat.ER_SYS_ILLVAL;
                        throw (new Exception("Recipe檔案有誤"));
                    }

                    /*-- 將Recipe顯示到DataGridView上 --*/
                    foreach (CtXML.XmlData xmlData in data) {
                        strDGV.Clear();
                        strDGV.Add(xmlData.Name);
                        strDGV.Add(xmlData.InnerText);
                        strDGV.Add(CtXML.FindAttribute(xmlData.Attributes, "Comment").Value);
                        CtInvoke.DataGridViewAddRow(dgvRecipe, strDGV, false, false);

                        if (CtXML.FindAttribute(xmlData.Attributes, "Device").Value.ToUpper() == "ACE") {
                            mRecipe.Add(
                                new CtRecipeData(
                                    Devices.ADEPT_ACE,
                                    xmlData.Name,
                                    xmlData.InnerText,
                                    (CtAce.VPlusVariableType)(CtConvert.CByte(CtXML.FindAttribute(xmlData.Attributes, "Type").Value)),
                                    CtXML.FindAttribute(xmlData.Attributes, "Comment").Value
                                )
                            );
                        } else if (CtXML.FindAttribute(xmlData.Attributes, "Device").Value.ToUpper() == "BECKHOFF") {
                            mRecipe.Add(
                                new CtRecipeData(
                                    Devices.BECKHOFF_PLC,
                                    xmlData.Name,
                                    xmlData.InnerText,
                                    (CtBeckhoff.SymbolType)(CtConvert.CByte(CtXML.FindAttribute(xmlData.Attributes, "Type").Value)),
                                    CtXML.FindAttribute(xmlData.Attributes, "Comment").Value
                                )
                            );
                        } else if (CtXML.FindAttribute(xmlData.Attributes, "Device").Value.ToUpper() == "CAMPRO") {
                            mRecipe.Add(
                                new CtRecipeData(
                                    Devices.CAMPro,
                                    xmlData.Name,
                                    xmlData.InnerText,
                                    CtXML.FindAttribute(xmlData.Attributes, "Comment").Value
                                )
                            );
                        }
                    }

                    /*-- 剛剛載入完且未修改，可以直接寫入設備 --*/
                    CtInvoke.ButtonEnable(btnDownload, true);

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
        private CtRecipeData FindRecipe(string name) {
            return mRecipe.Find(data => data.Name == name);
        }

        /// <summary>存檔至檔案</summary>
        /// <returns>Status Code</returns>
        private Stat SaveToXML() {
            Stat stt = Stat.SUCCESS;
            string strName = "";
            string strValue = "";
            string strComment = "";
            string strNode = "";
            byte idx = 0;
            CtProgress prog = new CtProgress("存檔", "存檔中，請稍後...", false);
            try {

                /*-- 檢查 ID 是不是有輸入 --*/
                if (txtID.Text == "") {
                    stt = Stat.ER_SYS_INVARG;
                    throw (new Exception("請先設定對應的Recipe ID"));
                }

                /*-- 檢查是不是已經存在此檔案，如果有就跳視窗詢問是否覆蓋 --*/
                string path = "";
                MsgBoxButton mbResult = MsgBoxButton.YES;

                if ((mOriName != "") && (txtID.Text != mOriName)) {
                    mbResult = CtMsgBox.Show("更改檔名", "是否更改檔名?" + CtConst.NewLine + "重新命名請按「是」" + CtConst.NewLine + "另存新檔請按「否」", MsgBoxButton.YES_NO, MsgBoxStyle.QUESTION);
                    if (mbResult == MsgBoxButton.YES) {
                        path = CtFile.BackSlash(mFileFolder) + RECIPE_EXTENSION.Replace("*", mOriName);
                        CtFile.DeleteFile(path);
                    }
                    mbResult = MsgBoxButton.YES;
                }

                path = CtFile.BackSlash(mFileFolder) + RECIPE_EXTENSION.Replace("*", txtID.Text);
                if (CtFile.IsFileExist(path)) {
                    mbResult = CtMsgBox.Show("覆寫", "此檔案已存在，是否覆寫?", MsgBoxButton.YES_NO, MsgBoxStyle.QUESTION);
                }

                if (mbResult == MsgBoxButton.YES) {

                    mCurrInfo = new RecipeInfo();
                    mCurrInfo.BuildTime = DateTime.Now;
                    mCurrInfo.Name = txtID.Text;
                    mCurrInfo.Comment = txtComment.Text;
                    mCurrInfo.Path = path;

                    prog.Start();

                    /*-- 開新檔案，覆蓋掉原始檔案 --*/
                    CreateXML(path);

                    mXML.Load(path);
                    mXML.SetInnerText("CASTEC_RECIPE/Comment", (txtComment.Text == "") ? DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff") : txtComment.Text);
                    foreach (DataGridViewRow item in dgvRecipe.Rows) {
                        strName = item.Cells[0].Value.ToString();
                        strValue = item.Cells[1].Value.ToString();
                        strComment = item.Cells[2].Value.ToString();
                        strNode = "CASTEC_RECIPE/Data/Recipe" + idx.ToString();
                        CtRecipeData data = FindRecipe(strName);

                        /*-- 該項目名稱 --*/
                        mXML.SetAttributes(strNode, "Name", strName);

                        /*-- 該項目註解 --*/
                        mXML.SetAttributes(strNode, "Comment", strComment);

                        /*-- 來源裝置 --*/
                        mXML.SetAttributes(strNode, "Device", (data.Source == Devices.ADEPT_ACE) ? "ACE" : (data.Source == Devices.BECKHOFF_PLC) ? "BECKHOFF" : "CAMPro");

                        /*-- 如有多重裝置，則如於 List<CtBeckhoff> 的第幾台設備 --*/
                        mXML.SetAttributes(strNode, "ListIndex", data.DeviceIndex.ToString());

                        /*-- 該項目之類型，目前 ACE、Beckhoff 都是變數型態，CAMPro 則用來表示 ACE 之 Vision Tool 路徑 --*/
                        byte varType = 0;
                        if (data.Source == Devices.ADEPT_ACE) varType = CtConvert.CByte(data.AceVarType);
                        else if (data.Source == Devices.BECKHOFF_PLC) varType = CtConvert.CByte(data.BkfVarType);
                        else if (data.Source == Devices.CAMPro) varType = 0;
                        mXML.SetAttributes(strNode, "Type", varType.ToString());

                        /*-- 寫入數值至 InnerText --*/
                        mXML.SetInnerText(strNode, strValue);

                        /*-- 如果東西有改變，那就更新吧！ --*/
                        if (data.Value.ToString() != strValue) data.Value = strValue;
                        if (data.Comment != strComment) data.Comment = strComment;

                        idx++;
                    }

                    /*-- 限制一定要存檔後才可以寫入設備 --*/
                    CtInvoke.ButtonEnable(btnDownload, true);

                    /*-- 更新完就重新掃 Recipe，因為可能有新增或移除檔案 --*/
                    SearchRecipe();
                    mChanged = false;
                }
            } catch (Exception ex) {
                if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, ex);
            } finally {
                prog.Close();
            }
            return stt;
        }

        /// <summary>取得當前數值，並更新於DataGridView上</summary>
        /// <param name="data">RecipeData之集合</param>
        private void GetCurrentValue(List<CtRecipeData> data) {
            CtProgress prog = new CtProgress("更新資料", "更新中，請稍後...", true);
            try {
                foreach (CtRecipeData item in data) {
                    if (item.Source == Devices.ADEPT_ACE) {
                        switch (item.AceVarType) {
                            case CtAce.VPlusVariableType.REAL:
                                float sngTemp;
                                rAce[item.DeviceIndex].Variable.GetValue(item.Name, out sngTemp);
                                item.Value = CtConvert.CStr(sngTemp);
                                break;

                            case CtAce.VPlusVariableType.STRING:
                                string strTemp;
                                rAce[item.DeviceIndex].Variable.GetValue(item.Name, out strTemp);
                                item.Value = strTemp;
                                break;

                            case CtAce.VPlusVariableType.LOCATION:
                                List<double> dblTemp;
                                rAce[item.DeviceIndex].Variable.GetValue(item.Name, out dblTemp, CtAce.VPlusVariableType.LOCATION);
                                string strLoc = "";
                                foreach (double dbl in dblTemp) {
                                    strLoc += dbl.ToString("###0.0##") + " ";
                                }
                                item.Value = strLoc.Trim().Replace(" ", ",");
                                break;

                            case CtAce.VPlusVariableType.PRECISION_POINT:
                                List<double> dblPP;
                                rAce[item.DeviceIndex].Variable.GetValue(item.Name, out dblPP, CtAce.VPlusVariableType.PRECISION_POINT);
                                string strPP = "";
                                foreach (double dbl in dblPP) {
                                    strPP += dbl.ToString("###0.0##") + " ";
                                }
                                item.Value = strPP.Trim().Replace(" ", ",");
                                break;
                        }
                    } else if (item.Source == Devices.BECKHOFF_PLC) {
                        object objTemp;
                        rBkf[item.DeviceIndex].GetValue(item.Name, out objTemp);
                        item.Value = objTemp;
                    } else if (item.Source == Devices.CAMPro) {
                        List<string> strModel = rAce[item.DeviceIndex].Vision.GetCurrentModelNames(item.Name);
                        if ((strModel != null) && (strModel.Count > 0)) {
                            item.Value = "";
                            for (byte idx = 0; idx < strModel.Count; idx++) {
                                item.Value += strModel[idx];
                                if (idx < strModel.Count - 1) item.Value += ",";
                            }
                        }
                    }
                }

                /*-- 更新至DataGridView --*/
                UpdateDataGridView(data);

                /*-- 限制一定要存檔後才可以寫入設備 --*/
                mLoaded = false;
                CtInvoke.ButtonEnable(btnDownload, false);
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            } finally {
                prog.Close();
            }
        }

        /// <summary>搜尋DataGridView裡之特定列</summary>
        /// <param name="name">Recipe名稱</param>
        /// <returns>最相符之列，如沒有則回傳null</returns>
        private DataGridViewRow FindRow(string name) {
            DataGridViewRow row = null;
            foreach (DataGridViewRow item in dgvRecipe.Rows) {
                if (item.Cells[0].Value.ToString() == name) {
                    row = item;
                    break;
                }
            }
            return row;
        }

        /// <summary>更新DataGridView之資料</summary>
        /// <param name="data">欲更新之RecipeData集合</param>
        private void UpdateDataGridView(List<CtRecipeData> data) {
            foreach (CtRecipeData item in data) {
                FindRow(item.Name).Cells[1].Value = item.Value.ToString();
            }
        }

        /// <summary>依照RecipeData寫入設備</summary>
        private Stat WrtieToEquipment() {
            Stat stt = Stat.SUCCESS;
            int step = 0;
            CtProgress prog = new CtProgress(ProgBarStyle.Percent, "寫入參數", "正在寫入...", mRecipe.Count, true);
            try {
                mCurrInfo.LoadTime = DateTime.Now;
                foreach (CtRecipeData item in mRecipe) {
                    step++;
                    prog.UpdateStep(step);
                    if (item.Source == Devices.ADEPT_ACE) {

                        switch (item.AceVarType) {
                            case CtAce.VPlusVariableType.REAL:
                                rAce[item.DeviceIndex].Variable.SetValue(item.Name, CtConvert.CFloat(item.Value));
                                break;

                            case CtAce.VPlusVariableType.STRING:
                                rAce[item.DeviceIndex].Variable.SetValue(item.Name, CtConvert.CStr(item.Value));
                                break;

                            case CtAce.VPlusVariableType.LOCATION:
                                string strTemp = item.Value.ToString();
                                List<string> strSplit = strTemp.Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries).ToList();
                                if (strSplit.Count == 6) {
                                    List<double> dblSplit = strSplit.ConvertAll(data => CtConvert.CDbl(data));
                                    rAce[item.DeviceIndex].Variable.SetValue(item.Name, dblSplit, CtAce.VPlusVariableType.LOCATION);
                                }
                                break;

                            case CtAce.VPlusVariableType.PRECISION_POINT:
                                string strTempPP = item.Value.ToString();
                                List<string> strSplitPP = strTempPP.Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries).ToList();
                                if ((strSplitPP.Count == 6) || (strSplitPP.Count == 4)) {
                                    List<double> dblSplit = strSplitPP.ConvertAll(data => CtConvert.CDbl(data));
                                    rAce[item.DeviceIndex].Variable.SetValue(item.Name, dblSplit, CtAce.VPlusVariableType.PRECISION_POINT);
                                }
                                break;
                        }

                    } else if (item.Source == Devices.BECKHOFF_PLC) {
                        rBkf[item.DeviceIndex].SetValue(item.Name, item.Value);
                    } else if (item.Source == Devices.CAMPro) {
                        rAce[item.DeviceIndex].Vision.RemoveLocatorModel(item.Name);
                        rAce[item.DeviceIndex].Vision.AddLocatorModel(item.Name, item.Value.ToString());
                    }
                }
                Thread.Sleep(500);
            } catch (Exception ex) {
                if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
                CtStatus.Report(stt, ex);
            } finally {
                prog.Close();
                if (stt == Stat.SUCCESS) {
                    mLoaded = true;
                    CtMsgBox.Show("寫入設備", "各參數已成功寫入設備！", MsgBoxButton.OK, MsgBoxStyle.INFORMATION);
                }
            }
            return stt;
        }

        private void LoadEmptyRecipe() {
            /*-- 建立空 Recipe，讓 "更新" 可按 --*/
            CtInvoke.ButtonVisible(btnRenew, true);

            /*-- 清除相關資料 --*/
            mOriName = "";
            mLoaded = false;
            CtInvoke.DataGridViewClear(dgvRecipe);
            CtInvoke.TextBoxText(txtComment, "");
            CtInvoke.TextBoxText(txtID, "");

            /*-- 限制一定要存檔後才可以寫入設備 --*/
            CtInvoke.ButtonEnable(btnDownload, false);

            /*-- 建立 DataGridView --*/
            List<string> strDGV = new List<string>();
            mRecipe.Clear();
            foreach (CtRecipeData item in mEmptyRecipe) {
                strDGV.Clear();
                strDGV.Add(item.Name);
                strDGV.Add("");
                strDGV.Add(item.Comment);
                CtInvoke.DataGridViewAddRow(dgvRecipe, strDGV, false, false);

                mRecipe.Add(item);
            }
        }
        #endregion

        #region Function - Interface Events

        /// <summary>ListBox點擊，檢查有沒有更新，然後載入點擊之檔案</summary>
        private void listFile_DoubleClick(object sender, EventArgs e) {
            if (listFile.SelectedIndex > -1) {
                MsgBoxButton mbResult = MsgBoxButton.NO;
                if (mChanged) {
                    mbResult = CtMsgBox.Show("檔案變更", "更改尚未儲存，放棄並離開？", MsgBoxButton.YES_NO, MsgBoxStyle.QUESTION);
                }

                if ((!mChanged) || (mbResult == MsgBoxButton.YES)) {
                    LoadRecipe(listFile.SelectedItem.ToString());
                    mChanged = false;
                }
            }
        }

        /// <summary>刪除</summary>
        private void btnDelete_Click(object sender, EventArgs e) {
            string path = CtFile.BackSlash(mFileFolder) + RECIPE_EXTENSION.Replace("*", txtID.Text);
            MsgBoxButton mbResult = MsgBoxButton.YES;
            if (CtFile.IsFileExist(path)) {
                mbResult = CtMsgBox.Show("刪除", "確定要刪除？", MsgBoxButton.YES_NO, MsgBoxStyle.QUESTION);
            }

            if (mbResult == MsgBoxButton.YES) {
                CtFile.DeleteFile(path);
                SearchRecipe();
                CtMsgBox.Show("刪除", "已刪除檔案");
            }
        }

        /// <summary>更新現在的數值，並更新在DataGridView</summary>
        private void btnRenew_Click(object sender, EventArgs e) {
            GetCurrentValue(mRecipe);
        }

        /// <summary>寫入設備，檢查有無更新並詢問後，依照RecipeData寫入</summary>
        private void btnDownload_Click(object sender, EventArgs e) {
            if (mChanged) {
                if (CtMsgBox.Show("存檔", "尚未儲存檔案，是否先存檔？", MsgBoxButton.YES_NO, MsgBoxStyle.QUESTION) == MsgBoxButton.YES) {
                    SaveToXML();
                }
            }

            WrtieToEquipment();
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
            mChanged = true;

            /*-- 限制一定要存檔後才可以寫入設備 --*/
            CtInvoke.ButtonEnable(btnDownload, false);
        }

        /// <summary>建立新的Recipe，藉由Origion Recipe更新於介面，但不讀取數值</summary>
        private void btnCreate_Click(object sender, EventArgs e) {
            LoadEmptyRecipe();
        }

        /// <summary>存檔</summary>
        private void btnSave_Click(object sender, EventArgs e) {
            SaveToXML();
        }

        private void btnExit_Click(object sender, EventArgs e) {
            if (mLoaded) DialogResult = System.Windows.Forms.DialogResult.OK;
            else DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
        #endregion

    }
}
