using CtINI;
using CtParamEditor.Comm;
using CtParamEditor.Core.Internal;
using CtParamEditor.Core.Internal.Component;
using CtParamEditor.Core.Internal.Component.FIeldEditor;
using DataGridViewRichTextBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtParamEditor.Core
{
    
    /// <summary>
    /// 參數編輯器
    /// </summary>
    public class ParamEditor: IParamEditor {

        #region Declaration - Fields

        /// <summary>
        /// 檔案儲存對話視窗
        /// </summary>
        //private SaveFileDialog mSdlg = new SaveFileDialog();
        
        /// <summary>
        /// 右鍵選單
        /// </summary>
        private ContextMenuStrip mCMS = new ContextMenuStrip();

        /// <summary>
        /// 新增參數選項
        /// </summary>
        private ToolStripItem miAdd = null;

        /// <summary>
        /// 編輯欄位選項
        /// </summary>
        private ToolStripItem miEdit = null;

        /// <summary>
        /// 刪除參數選項
        /// </summary>
        private ToolStripItem miDelete = null;

        /// <summary>
        /// INI檔案路徑
        /// </summary>
        private string mIniPath = null;

        /// <summary>
        /// <see cref="DataGridView"/>物件參考
        /// </summary>
        private DataGridView rDgv = null;

        /// <summary>
        /// 被點選的DataGridCell列數
        /// </summary>
        private int mIdxRow = -1;

        /// <summary>
        /// 被點選的DataGridCell行數
        /// </summary>
        private int mIdxCol = -1;

        private RtfConvert mRgConvert = new RtfConvert();

        private RtfConvert mRcConvert = new RtfConvert();

        private RtfConvert mMrConvert = new RtfConvert();

        private RtfConvert mMcConvert = new RtfConvert();

        private Action<MethodInvoker> mDelinvoke = null;

        /// <summary>
        /// 要顯示的選項
        /// </summary>
        private CmsOption mShowOption = CmsOption.None;

        /// <summary>
        /// 要鎖住的選項
        /// </summary>
        private CmsOption mDisableOption = CmsOption.None;

        #endregion Declaration - Fields

        #region Declaration - Properties

        /// <summary>
        /// 被選取的行索引
        /// </summary>
        public int SelectedColumn {
            get => mIdxCol;
            set {
                if (mIdxCol != value) {
                    mIdxCol = value;
                    DecidedShow(SelectedColumn, SelectedRow);
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 被選取的列索引
        /// </summary>
        public int SelectedRow {
            get => mIdxRow;
            set {
                if (mIdxRow != value) {
                    mIdxRow = value;
                    DecidedShow(SelectedColumn, SelectedRow);
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 儲存格樣式管理
        /// </summary>
        public ICellStyles CellStyles { get; } = new CtCellStyles();

        /// <summary>
        /// 顯示資料用的<see cref="DataGridView"/>
        /// </summary>
        public DataGridView GridView {
            get { return rDgv; }
            set {
                rDgv = value;
                if (rDgv != null) {
                    DeployDGV(rDgv);
                }
            }
        }

        public IParamCollection ParamCollection { get { return DataSource; } }

        /// <summary>
        /// 文字輸入視窗方法委派
        /// </summary>
        public Input.Text InputText {
            get {
                return Field.InputText;
            }
            set {
                if (value != null) Field.InputText = value;
            }
        }

        /// <summary>
        /// 下拉選單輸入視窗方法委派
        /// </summary>
        public Input.ComboBox ComboBoxList {
            get {
                return Field.ComboBoxList;
            }
            set {
                if (value != null) Field.ComboBoxList = value;
            }
        }

        /// <summary>
        /// 列舉定義管理器
        /// </summary>
        private CtEnumData EnumData { get; set; } = new CtEnumData();

        /// <summary>
        /// 參數資料
        /// </summary>
        private CtDgvDataSource DataSource { get; set; } = new CtDgvDataSource();

        /// <summary>
        /// 非法欄位紀錄
        /// </summary>
        private CtIllegalField IllegalField { get; set; } = new CtIllegalField();

        /// <summary>
        /// 被修改欄位紀錄
        /// </summary>
        private CtModifiedField ModifiedField { get; set; } = new CtModifiedField();

        /// <summary>
        /// 欄位編輯器
        /// </summary>
        private CtFieldEditor Field { get; } = new CtFieldEditor();

        /// <summary>
        /// Invoke方法委派
        /// </summary>
        public Action<MethodInvoker> DelInvoke { get => mDelinvoke; set {
                if (mDelinvoke != value && value != null) {
                    mDelinvoke = value;
                }
            }
        }

        /// <summary>
        /// 要顯示的選項
        /// </summary>
        public CmsOption ShowOption {
            get => mShowOption;
            set {
                if (mShowOption != value) {
                    mShowOption = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 要鎖住的選項
        /// </summary>
        public CmsOption DisableOption {
            get => mDisableOption;
            set {
                if (mDisableOption != value) {
                    mDisableOption = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Declaration - Properteis

        #region Declaration - Enum

        

        #endregion Declaration - Enum

        #region Declaration - Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Declaration - Events

        #region Function - Constructors

        internal ParamEditor() {
            /*-- 產生右鍵選單選項 --*/
            CreateOption();

            /*-- 分配委派 --*/
            AssignDelegation();

            /*-- 分配字型樣式 --*/
            AssignFontStyle();

        }

        #endregion Funciton - Constructors

        #region Function - Public Methods

        /// <summary>
        /// 標記關鍵字
        /// </summary>
        /// <param name="keyWord"></param>
        public void Highlight(string keyWord) {
            mRgConvert.KeyWord = keyWord;
            mMrConvert.KeyWord = keyWord;
            mMcConvert.KeyWord = keyWord;
        }

        /// <summary>
        /// 將含有
        /// </summary>
        /// <param name="keyWord"></param>
        public void Filter(string keyWord) {
            DataSource.Filter(keyWord);
        }

        public void CloseFilter() {
            DataSource.CloseFilter();
        }

        /// <summary>
        /// 讀取INI檔
        /// </summary>
        /// <param name="fileName"></param>
        public void ReadINI(string fileName) {
            /*-- 讀取INI資料 --*/
            List<IniStruct> inis = CtINI.CtINI.ReadValues(fileName);
            /*-- 讀取Enum定義 --*/
            EnumData.ReadINI(inis);
            /*-- 讀取參數定義 --*/
            DataSource.ReadINI(inis);
            /*-- 記錄INI檔路徑 --*/
            mIniPath = fileName;
        }

        /// <summary>
        /// 在指定列數插入新參數
        /// </summary>
        /// <param name="idxRow"></param>
        public void Insert(int idxRow) {
            idxRow = idxRow != -1 ? idxRow : DataSource.RowCount();
            DataSource.Insert(idxRow);
        }

        /// <summary>
        /// 在選定列插入新行
        /// </summary>
        public void Insert() {
            Insert(SelectedRow);
        }

        /// <summary>
        /// 移除指定列數的參數
        /// </summary>
        /// <param name="idxRow"></param>
        public void Remove(int idxRow) {
            DataSource.Remove(idxRow);
        }

        /// <summary>
        /// 移除選定列
        /// </summary>
        public void Remove(){
            Remove(SelectedRow);
        }

        public void Edit() {
            //IParam prop = DataSource[mIdxRow] as IParam;
            //string rtnVal = string.Empty;
            //string oriVal = cell.Value?.ToString();
            //List<string> Types = EnumData.Data.Keys.ToList();
            //if (Field.Edit(mIdxCol, prop) && rDgv != null) {
            //    DataGridViewRow row = rDgv.Rows[mIdxRow];
            //    cell.Style.ForeColor = Color.Red;
            //    cell.Selected = false;
            //    rDgv.Refresh();
            //}
        }

        /// <summary>
        /// 將參數寫入INI檔
        /// </summary>
        public void SaveToINI(string path = null) {
            if (IllegalField.Count() > 0) {
                /*-- 反白未填入的儲存格 --*/
                if (rDgv != null) {
                    IParamColumn prop = IllegalField[0].Key;
                    int idx = DataSource.IndexOf(prop);
                    rDgv[PropField.Idx.Name, idx].Selected = true;
                }
                throw new Exception("尚有必填資料未輸入");
            } else {
                string savePath = string.IsNullOrEmpty(path) ? mIniPath : path;
                StringBuilder iniContent = new StringBuilder();
                ///Note: Section定義不可相同
                ///     WriteINI方法並不會檢查已寫入的Section名稱
                ///     若是有重複Section名稱可能在讀取的時候會有問題
                ///     因此要自行確保沒有相同的Section名稱
                /*-- 寫入Enum類型定義 --*/
                EnumData.WriteINI(iniContent);
                /*-- 寫入參數定義 --*/
                DataSource.WriteINI(iniContent);

                /*-- 寫入INI檔 --*/
                File.WriteAllText(savePath, iniContent.ToString(), Encoding.UTF8);

                /*-- 將修改的儲存格樣式復原 --*/
                #region 資料繫結 
                /////資料繫結需逐儲存格將樣式復原
                //if (ModifiedField.Data.Count() > 0 && rDgv != null) {
                //    foreach(var kvp in ModifiedField.Data) {
                //        int idxRow = DataSource.Data.IndexOf(kvp.Key);
                //        rDgv.Rows[idxRow].DefaultCellStyle.Font = RegularFont;
                //        foreach(int idxCol in kvp.Value) {
                //            rDgv[idxCol, idxRow].Style.ForeColor = Color.Black;
                //        }
                //    }
                //}
                #endregion 資料繫結需逐儲存格將樣式復原

                #region 虛擬填充

                ///由於虛擬填充是顯示時設定顯示樣式
                ///清除特殊樣式欄位紀錄即可
                ModifiedField.Clear();
                rDgv?.Refresh();

                #endregion   

            }
        }
        
        /// <summary>
        /// 清除所有資料
        /// </summary>
        public void Clear() {
            ///為避免DataSource清除後虛擬填充事件觸發撈不到資料
            ///必須先把觸發顯示的資料筆數清除
            if (rDgv != null) rDgv.RowCount = 0;
            DataSource.Clear();
            ModifiedField.Clear();
            EnumData.Data.Clear();
            IllegalField.Clear();
        }

        /// <summary>
        /// 恢復預設值
        /// </summary>
        public void RestoreDefault() {
            DataSource.Foreach(prop => {
                if (prop.Default != null) {
                    prop.SetValue(prop.Default.ToString(), PropField.Idx.Value);
                }
            });
            rDgv?.Refresh();
        }

        //public void WriteParam<T>(string name, T val, string description, T def) {
        //    if (DataSource.Contains(name)) throw new ArgumentException($"已有{name}參數名稱定義");

        //    DataSource.WriteParam(name, val, description, def);
        //}
        
        #endregion Function - Public Methods

        #region Function - Events

        #region DataGridView

        /// <summary>
        /// 虛擬填充儲存格資料要求事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rDgv_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            if (e.RowIndex >= DataSource.RowCount()) {
                return;
            }
            /*-- 取得欄位資料 --*/
            IParam prop = DataSource[e.RowIndex] as IParam;
            string v = prop.Default;
            /*-- 取得欄位值 --*/
            string val = prop.GetParamValue(e.ColumnIndex);
            /*-- 必填欄位檢查 --*/
            if (IllegalField.Contains(prop, e.ColumnIndex)) {
                e.Value = mRcConvert.ToRTF("Required cell", CellStyles.RequiredCell, false);//必填欄位樣式
            } else if (ModifiedField.ContainsRow((prop))) {
                //已編輯儲存格樣式
                if (ModifiedField.ContainsColumn(prop, e.ColumnIndex)) {
                    e.Value = mMcConvert.ToRTF(val, CellStyles.ModifiedCell);
                    //已編輯欄位樣式
                } else {
                    e.Value = mMrConvert.ToRTF(val, CellStyles.ModifiedRow);
                }
                /*-- 一般欄位 --*/
            } else {
                e.Value = mRgConvert.ToRTF(val, CellStyles.Regular);
            }
        }
        
        /// <summary>
        /// 開啟右鍵選單(增加新參數設定)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rDgv_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right && !mCMS.Visible) {
                mCMS.Show(Cursor.Position);
                if (!miAdd.Visible) miAdd.Visible = true;
                if (miEdit.Visible) miEdit.Visible = false;
                if (miDelete.Visible) miDelete.Visible = false;
                mIdxRow = rDgv.RowCount;
            }
        }

        #endregion DataGridView

        #region ToolStripItem

        /// <summary>
        /// 新增參數
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miAdd_Click(object sender, EventArgs e) {
            Insert(mIdxRow);
        }

        /// <summary>
        /// 刪除參數
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miDelete_Click(object sender, EventArgs e) {
            Remove(mIdxRow);
        }

        /// <summary>
        /// 編輯欄位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miEdit_Click(object sender, EventArgs e) {
            DataGridViewCell cell = rDgv[mIdxCol, mIdxRow];
            if (DataSource == null) throw new Exception("繫結資料源為null");
            if (DataSource.RowCount() < mIdxRow + 1) throw new Exception("超出資料範圍");
            IParam prop = DataSource[mIdxRow] as IParam;
            string rtnVal = string.Empty;
            string oriVal = cell.Value?.ToString();
            List<string> Types = EnumData.Data.Keys.ToList();
            if (Field.Edit(mIdxCol, prop) && rDgv != null) {
                DataGridViewRow row = rDgv.Rows[mIdxRow];
                cell.Style.ForeColor = Color.Red;
                cell.Selected = false;
                rDgv.Refresh();
            }
        }

        #endregion ToolStripItem

        #endregion Function - Events

        #region Function-  Private Methods

        /// <summary>
        /// 分配字型樣式
        /// </summary>
        private void AssignFontStyle() {
            mRgConvert.Regular = CellStyles.Regular;
            mRcConvert.Regular = CellStyles.RequiredCell;
            mMcConvert.Regular = CellStyles.ModifiedCell;
            mMrConvert.Regular = CellStyles.ModifiedRow;

            mRgConvert.Highlight = CellStyles.Highlight;
            mMcConvert.Highlight = CellStyles.Highlight;
            mMrConvert.Highlight = CellStyles.Highlight;
        }

        /// <summary>
        /// 部署<see cref="DataGridView"/>
        /// </summary>
        /// <param name="dgv"></param>
        private void DeployDGV(DataGridView dgv) {
            /*-- 關閉欄位名稱自動產生 --*/
            dgv.AutoGenerateColumns = false;
            /*-- 開啟虛擬填充模式 --*/
            dgv.VirtualMode = true;
            /*-- 鎖住直接編輯功能 --*/
            dgv.ReadOnly = true;
            /*-- 啟用雙緩衝 --*/
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, true, null);
            /*-- 虛擬填充事件委派 --*/
            dgv.CellValueNeeded += rDgv_CellValueNeeded;
            /*-- 儲存格滑鼠點擊事件委派 --*/
            //dgv.CellMouseClick += rDgv_CellMouseClick;
            /*-- 滑鼠點擊事件委派 --*/
            //dgv.MouseClick += rDgv_MouseClick;
            /*-- 配置欄位標題 --*/
            DataGridViewRichTextBox.Factory FTY = new DataGridViewRichTextBox.Factory();
            List<DataGridViewColumn> cols = new List<DataGridViewColumn>() {
                FTY.GetRichTextColumn(PropField.Str.Name),
                FTY.GetRichTextColumn(PropField.Str.ValType),
                FTY.GetRichTextColumn(PropField.Str.Value),
                FTY.GetRichTextColumn(PropField.Str.Description),
                FTY.GetRichTextColumn(PropField.Str.Max),
                FTY.GetRichTextColumn(PropField.Str.Min),
                FTY.GetRichTextColumn(PropField.Str.Default),
            };
            dgv.Columns.Clear();
            dgv.Columns.AddRange(cols.ToArray());
        }

        /// <summary>
        /// 將參數設定寫入<see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="data"></param>
        private void WriteProperty(StringBuilder sb, IParam data) {
            sb.AppendLine($"[{data.Name}]");
            sb.AppendLine($"Type={data.Type}");
            sb.AppendLine($"Value={data.Value}");
            sb.AppendLine($"Description={data.Description}");
            if (data.Max != null) sb.AppendLine($"Max={data.Max}");
            if (data.Min != null) sb.AppendLine($"Min={data.Min}");
            if (data.Default != null) sb.AppendLine($"Default={data.Default}");
            sb.AppendLine();
        }

        ///// <summary>
        ///// 鎖住右鍵選單選項
        ///// </summary>
        ///// <param name="option"></param>
        //private void DisableOption(CmsOption option) {
        //    bool isAdd = ((int)option & (int)CmsOption.Add) == (int)CmsOption.Add;
        //    bool isEdit = ((int)option & (int)CmsOption.Edit) == (int)CmsOption.Edit;
        //    bool isDelete = ((int)option & (int)CmsOption.Delete) == (int)CmsOption.Delete;
        //    if (miAdd.Enabled == isAdd) miAdd.Enabled = !isAdd;
        //    if (miEdit.Enabled == isEdit) miEdit.Enabled = !isEdit;
        //    if (miDelete.Enabled == isDelete) miDelete.Enabled = !isDelete;
        //}

        ///// <summary>
        ///// 顯示右鍵選單選項
        ///// </summary>
        ///// <param name="option"></param>
        //private void ShowOption(CmsOption option) {
        //    bool isAdd = ((int)option & (int)CmsOption.Add) == (int)CmsOption.Add;
        //    bool isEdit = ((int)option & (int)CmsOption.Edit) == (int)CmsOption.Edit;
        //    bool isDelete = ((int)option & (int)CmsOption.Delete) == (int)CmsOption.Delete;
        //    if (miAdd.Visible != isAdd) miAdd.Visible = isAdd;
        //    if (miEdit.Visible != isEdit) miEdit.Visible = isEdit;
        //    if (miDelete.Visible != isDelete) miDelete.Visible = isDelete;
        //}

        /// <summary>
        /// 產生右鍵選單選項
        /// </summary>
        private void CreateOption() {
            miAdd = mCMS.Items.Add("Add");
            miEdit = mCMS.Items.Add("Edit");
            miDelete = mCMS.Items.Add("Delete"); 
        }

        /// <summary>
        /// 分配委派
        /// </summary>
        private void AssignDelegation() {
            /*-- 修改欄位紀錄方法委派 --*/
            Field.AddModifiedField = ModifiedField.Add;
            /*-- 非法欄位紀錄方法委派 --*/
            Field.RecordIllegalField = IllegalField.Add;
            /*-- 非法欄位紀錄註銷方法委派 --*/
            Field.RemoveIllegalField = IllegalField.Remove;

            Field.GetItems = EnumData.GetItems;
            /*-- 非法欄位紀錄方法委派 --*/
            DataSource.AddIllegal = IllegalField.Add;
            /*-- 非法欄位移除方法委派 --*/
            DataSource.RemoveIllegalRecord = IllegalField.Remove;

            DataSource.ContainType = EnumData.ContainType;

            DataSource.ContainItem = EnumData.ContainItem;

            DataSource.UpdateRowCount = UpdateRowCount;

            DataSource.ReadEnum = EnumData.ReadEnum;

            Field.ContainItem = EnumData.ContainItem;

            Field.ContainType = EnumData.ContainType;

            Field.GetTypes = EnumData.GetTypes;

            /*-- 右鍵選單選項事件委派 --*/
            miAdd.Click += miAdd_Click;
            miEdit.Click += miEdit_Click;
            miDelete.Click += miDelete_Click;
        }

        /// <summary>
        /// 更新要顯示的資料筆數
        /// </summary>
        /// <param name="rowCount"></param>
        private void UpdateRowCount(int rowCount) {
            if (rDgv != null) rDgv.RowCount = rowCount;
        }

        /// <summary>
        /// 依照欄位索引決定右鍵選單樣式
        /// </summary>
        /// <param name="e"></param>
        /// <param name="show"></param>
        /// <param name="disable"></param>
        private void DecidedShow(DataGridViewCellMouseEventArgs e, out CmsOption show, out CmsOption disable) {
            int idxCol = e.ColumnIndex;
            disable = CmsOption.None;
            show = CmsOption.Edit;
            switch (idxCol) {
                case PropField.Idx.Name:
                case PropField.Idx.Description:
                case PropField.Idx.ValType:
                    break;
                case PropField.Idx.Value:
                case PropField.Idx.Max:
                case PropField.Idx.Min:
                case PropField.Idx.Default:
                    IParam prop = DataSource[e.RowIndex] as IParam;
                    if (string.IsNullOrEmpty(prop.Type)) {
                        disable = CmsOption.Edit;
                    } else {
                        if (idxCol == PropField.Idx.Max || idxCol == PropField.Idx.Min) {
                            Type type = prop.GetParamType();
                            Type[] interfaces = type.GetInterfaces();
                            if (prop.RangeDefinable) { 
//                            if (prop.Type != typeof(int).Name && prop.Type != typeof(float).Name) {
                                disable = CmsOption.Edit;
                            }
                        }
                    }
                    break;
                default:
                    show = CmsOption.Add | CmsOption.Delete;
                    break;
            }
        }

        private void DecidedShow(int idxCol,int idxRow) {
            if (idxRow == -1) {
                ShowOption = CmsOption.Add;
            } else {

                switch (idxCol) {
                    case PropField.Idx.Name:
                    case PropField.Idx.Description:
                    case PropField.Idx.ValType:
                        ShowOption = CmsOption.Edit;
                        break;
                    case PropField.Idx.Value:
                    case PropField.Idx.Max:
                    case PropField.Idx.Min:
                    case PropField.Idx.Default:
                        ShowOption = CmsOption.Edit;
                        IParam prop = DataSource[idxRow] as IParam;
                        if (string.IsNullOrEmpty(prop.Type)) {
                            DisableOption = CmsOption.Edit;
                        } else {
                            if (idxCol == PropField.Idx.Max || idxCol == PropField.Idx.Min) {
                                if (prop.RangeDefinable) {
                                    DisableOption = CmsOption.Edit;
                                }
                            }
                        }
                        break;
                    default:
                        ShowOption = CmsOption.Add | CmsOption.Delete;
                        break;
                }
            }
        }

        /// <summary>
        /// 屬性變更發報
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") {
            DelInvoke?.Invoke(() => PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName)));
        }

        public IParamValue<T> WriteParam<T>(string name, T val, string description) {
            return WriteParam<T>(name, val, description, default(T));
        }

        public IParamValue<T> WriteParam<T>(string name, T val, string description, T def) {
            if (DataSource.Contains(name)) throw new ArgumentException($"已有{name}參數名稱定義");
            return DataSource.WriteParam(name, val, description, def);
        }

        #endregion Funciton - Private Methods

    }

}
