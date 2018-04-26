using CtINI;
using CtINI.Testing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Generic.Dictionary<int, string>;
using CtLib.Forms;
using CtLib.Library;
using CtParamEditor.Comm;
using CtBind;
using CtParamEditor.Core.Internal;
using CtParamEditor.Core.Internal.Component.FIeldEditor;
using CtParamEditor.Core.Internal.Component;
using DataGridViewRichTextBox;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using CtParamEditor.Core;

namespace CtTesting {

    /// <summary>
    /// 參數編輯器介面
    /// </summary>
    public partial class CtrlParamEditor : Form ,IDataDisplay<IParamEditor>,IDataDisplay<IParamCollection>{

        #region Declaration - Fields

        private IParamEditor mEditor = Factory.Param.Editor();
        
        /// <summary>
        /// 檔案開啟對話視窗
        /// </summary>
        private OpenFileDialog mOdlg = new OpenFileDialog();

        private ICellStyles mCellStyle = new CtCellStyles();

        private RtfConvert mRgConvert = new RtfConvert();

        private RtfConvert mRcConvert = new RtfConvert();

        private RtfConvert mMrConvert = new RtfConvert();

        private RtfConvert mMcConvert = new RtfConvert();

        #endregion Declaration - Fields
        
        #region Function - Constructors

        public CtrlParamEditor() {
            InitializeComponent();

            CloneClass c1 = new CloneClass(10);
            CloneClass c2 = c1.Clone() as CloneClass;
            /*-- DataGridView寬度預設 --*/
            DeployDGV(dgvProperties);
            /*-- 使用者輸入方法委派 --*/
            mEditor.InputText = InputText;
            mEditor.ComboBoxList = ComboBoxList;
            /*-- 事件委派 --*/
            mEditor.ParamCollection.DataChanged += ParamCollection_DataChanged;
            /*-- 資料綁定 --*/
            Bindings(mEditor);
            Bindings(mEditor.ParamCollection);
        }

        #endregion Function - Constructors

        #region Function - Evnets

        #region DataGridView

        /// <summary>
        /// 以儲存格為對象開啟右鍵選單
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvProperties_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            mEditor.SelectedColumnName = e.ColumnIndex != -1 ? dgvProperties.Columns[e.ColumnIndex].HeaderText : "";
            mEditor.SelectedRow = e.RowIndex;
            if (e.Button == MouseButtons.Right) {
                if (mEditor.ShowOption != CmsOption.None) {
                    cmsDGV.Show(Cursor.Position);
                }
            }
        }
        
        /// <summary>
        /// 開啟右鍵選單(增加新參數設定)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvProperties_MouseClick(object sender, MouseEventArgs e) {
        }

        /// <summary>
        /// 虛擬填充
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvProperties_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            string columnName = dgvProperties.Columns[e.ColumnIndex].HeaderText;
            if (e.RowIndex >= mEditor.ParamCollection.RowCount) {
                return;
            }
            try {
                /*-- 取得欄位資料 --*/
                IParam prop = mEditor.ParamCollection[e.RowIndex]as IParam;
                string v = prop.Default;
                /*-- 取得欄位值 --*/
                string val = prop.GetParamValue(columnName);
                EmColumn emColumn = columnName.ToEnumColumn();

                if ((prop.IlleaglColumn() & emColumn) != EmColumn.None) {
                    e.Value = mRcConvert.ToRTF("Required cell", mCellStyle.RequiredCell, false);//必填欄位樣式
                }else if (prop.ModifiedColumn() != EmColumn.None) {
                    if ((prop.ModifiedColumn() & emColumn) != EmColumn.None) {
                        e.Value = mMcConvert.ToRTF(val, mCellStyle.ModifiedCell);
                    } else {
                        e.Value = mMrConvert.ToRTF(val, mCellStyle.ModifiedRow);
                    }
                } else {
                    
                    e.Value = mRgConvert.ToRTF(val, mCellStyle.Regular);
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion DataGridView

        #region ToolStripMenuItem

        private void miEdit_Click(object sender, EventArgs e) {
            var columnName = mEditor.SelectedColumnName;
            /*-- 取得目前選取的資料列 --*/
            IParam prop = mEditor.ParamCollection[mEditor.SelectedRow] as IParam;
            /*-- 使用者輸入 --*/
            mEditor.Edit(columnName);
        }

        private void miDelete_Click(object sender, EventArgs e) {
            mEditor.Remove();
        }

        private void miAdd_Click(object sender, EventArgs e) {
            mEditor.Insert();
        }

        #endregion ToolStripMenuItem

        #region Button

        /// <summary>
        /// 儲存參數設定檔
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e) {
            mEditor.SaveToINI();
        }

        /// <summary>
        /// 過濾欄位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFilter_Click(object sender, EventArgs e) {
            mEditor.Filter(txtKeyWord.Text);
        }

        /// <summary>
        /// 顯示全部欄位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAll_Click(object sender, EventArgs e) {
            mEditor.CloseFilter();
        }

        /// <summary>
        /// 開啟參數設定檔
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpen_Click(object sender, EventArgs e) {
            //if (mEditor.GridView == null) mEditor.GridView = dgvProperties;
            /*-- 設定要開啟得檔案類型 --*/
            mOdlg.Filter = "Ini File|*.ini";
            mOdlg.Title = "Select a Ini File";
            if (mOdlg.ShowDialog() == DialogResult.OK) {
                /*-- 讀取Ini檔 --*/
                mEditor.ReadINI(mOdlg.FileName);
            }
        }
        
        /// <summary>
        /// 清除DGV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e) {
            mEditor.Clear();
        }

        /// <summary>
        /// 恢復預設
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRestoreDefault_Click(object sender, EventArgs e) {
            mEditor.RestoreDefault();
        }

        /// <summary>
        /// 標記
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHighlight_Click(object sender, EventArgs e) {
            mEditor.Highlight(txtKeyWord.Text);
            dgvProperties.Refresh();
        }
        
        /// <summary>
        /// 參數範本輸出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click(object sender, EventArgs e) {
            //參數設定樣本輸出
            //if (mEditor.GridView != null) mEditor.GridView = null;
            mEditor.Clear();
            mEditor.WriteParam("IntName", 20, "IntDescription", 20);
            mEditor.WriteParam("BoolName", false, "BoolDescription", false);
            mEditor.WriteParam("FloatName", 20f, "FloatDescription", 20f);
            mEditor.WriteParam("StringName", "value", "StringDescription", "default");
            mEditor.WriteParam("EnumName", TestEM.one, "EnumDescription", TestEM.three);

            mEditor.WriteParam("MaxSetting", 20, "MaxSetting").SetMaximun(100);
            mEditor.WriteParam("MinSetting", 20, "MinSetting").SetMinimun(0);
            mEditor.WriteParam("RangeSetting", 20, "RangeSetting").SetRange(0, 100);
            mEditor.SaveToINI(@"D:\Test1123.ini");
        }

        private void btnNewRow_Click(object sender, EventArgs e) {

        }

        /// <summary>
        /// 撤銷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUndo_Click(object sender, EventArgs e) {
            mEditor.Undo();
        }

        /// <summary>
        /// 重做
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRedo_Click(object sender, EventArgs e) {
            mEditor.Redo();
        }

        /// <summary>
        /// 參數讀取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRead_Click(object sender, EventArgs e) {
            //if (mEditor.GridView != null) mEditor.GridView = null;
            int iVal = 0;
            float fVal = 0f;
            bool bVal = false;
            string sVal = string.Empty;
            TestEM emVal = TestEM.two;
            TestData data = new TestData();
            mEditor.Clear();
            mEditor.ReadINI(@"D:\Test1122.ini");
            mEditor.ParamCollection.FindVal<int>("IntName", val => data.Value = val);
            mEditor.ParamCollection.FindVal("IntName", ref iVal);
            mEditor.ParamCollection.FindVal("FloatName", ref fVal);
            mEditor.ParamCollection.FindVal("BoolName", ref bVal);
            mEditor.ParamCollection.FindVal("StringName", ref sVal);
            mEditor.ParamCollection.FindVal("EnumName", ref emVal);
        }

        #endregion Button

        #region Label

        private void lbHightlightFont_OnDoubleClick(object sender, EventArgs e) {

        }

        private void lbRegularFont_DoubleClick(object sender, EventArgs e) {

        }

        private void lbRgFore_DoubleClick(object sender, EventArgs e) {

        }

        private void lbRgBack_DoubleClick(object sender, EventArgs e) {

        }
        private void lbHlFore_DoubleClick(object sender, EventArgs e) {

        }

        private void lbHlBack_DoubleClick(object sender, EventArgs e) {

        }

        #endregion Label

        #region IParamCollection

        private void ParamCollection_DataChanged(object sender, EventArgs e) {
            dgvProperties.Refresh();
        }

        #endregion IParamCollection

        #endregion Function - Events

        #region  Function - Private Methods

        private bool InputText(out string result, string title, string describe, string defValue = "") {
            return Stat.SUCCESS == CtInput.Text(out result, title, describe, defValue);
        }

        private bool ComboBoxList(out string result, string title, string describe, IEnumerable<string> itemList, string defValue = "", bool allowEdit = false) {
            return Stat.SUCCESS == CtInput.ComboBoxList(out result, title, describe, itemList,defValue, allowEdit);
        }
        
        private Color ComplementrayColor(Color color) {
            if (color == Color.Empty) {
                return Color.Black;
            }else {
                return Color.FromArgb(color.A, 255 - color.R, 255 - color.G, 255 - color.B);
            }
        }


        /// <summary>
        /// 部署<see cref="DataGridView"/>
        /// </summary>
        /// <param name="dgv"></param>
        private void DeployDGV(DataGridView dgv) {
            int width = dgv.RowHeadersWidth + 3;
            foreach (DataGridViewColumn col in dgv.Columns) width += col.Width;
            dgv.Width = width;
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
            dgv.CellValueNeeded += DgvProperties_CellValueNeeded;
            /*-- 儲存格滑鼠點擊事件委派 --*/
            dgv.CellMouseClick += dgvProperties_CellMouseClick;
            /*-- 滑鼠點擊事件委派 --*/
            dgv.MouseClick += dgvProperties_MouseClick;
            /*-- 配置欄位標題 --*/
            DataGridViewRichTextBox.Factory FTY = new DataGridViewRichTextBox.Factory();
            List<DataGridViewColumn> cols = new List<DataGridViewColumn>() {
                FTY.GetRichTextColumn(nameof(IParamColumn.Name)),
                FTY.GetRichTextColumn(nameof(IParamColumn.Type)),
                FTY.GetRichTextColumn(nameof(IParamColumn.Value)),
                FTY.GetRichTextColumn(nameof(IParamColumn.Description)),
                FTY.GetRichTextColumn(nameof(IParamColumn.Max)),
                FTY.GetRichTextColumn(nameof(IParamColumn.Min)),
                FTY.GetRichTextColumn(nameof(IParamColumn.Default)),
            };
            dgv.Columns.Clear();
            dgv.Columns.AddRange(cols.ToArray());
        }


        #endregion Function - Private Methods

        public enum TestEM {
            one = 2,
            two = 3,
            three = 4
        }

        #region Implement - IDataDisplay

        /// <summary>
        /// <see cref="IParamEditor"/>資料綁定
        /// </summary>
        /// <param name="source">資料源</param>
        public void Bindings(IParamEditor source) {
            if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);

            /*-- Add選項顯示 --*/
            miAdd.DataBindings.ExAdd(nameof(miAdd.Visible), source, nameof(source.ShowOption),(sender,e) => {
                e.Value = ((CmsOption)e.Value).ShowOption(CmsOption.Add);
            },mEditor.ShowOption.ShowOption(CmsOption.Add));
            /*-- Delete選項 --*/
            miDelete.DataBindings.ExAdd(nameof(miDelete.Visible), source, nameof(source.ShowOption),(sender,e) => {
                e.Value = ((CmsOption)e.Value).ShowOption(CmsOption.Delete);
            },source.ShowOption.ShowOption(CmsOption.Delete));
            /*-- Edit選項 --*/
            miEdit.DataBindings.ExAdd(nameof(miEdit.Visible), source, nameof(source.ShowOption), (sneder, e) => {
                e.Value = ((CmsOption)e.Value).ShowOption(CmsOption.Edit);
            }, source.ShowOption.ShowOption(CmsOption.Edit));
            /*-- Add Disable --*/
            miAdd.DataBindings.ExAdd(nameof(miAdd.Enabled), source, nameof(source.DisableOption), (sneder, e) => {
                e.Value = ((CmsOption)e.Value).DisableOption(CmsOption.Add);
            },source.DisableOption.DisableOption(CmsOption.Delete));
            /*-- Delete Disable --*/
            miDelete.DataBindings.ExAdd(nameof(miDelete.Enabled), source, nameof(source.DisableOption), (sender, e) => {
                e.Value = ((CmsOption)e.Value).DisableOption(CmsOption.Delete);
            },source.DisableOption.DisableOption(CmsOption.Delete));
            /*-- Edit Disable --*/
            miEdit.DataBindings.ExAdd(nameof(miEdit.Enabled), source, nameof(source.DisableOption), (sender, e) => {
                e.Value = ((CmsOption)e.Value).DisableOption(CmsOption.Edit);
            },source.DisableOption.DisableOption(CmsOption.Edit));
            /*-- 可撤銷次數 --*/
            btnUndo.DataBindings.ExAdd(nameof(btnUndo.Enabled), source, nameof(source.UndoCount),(snemder,e)=> {
                e.Value = (int)e.Value > 0;
            });
            /*-- 可重做次數 --*/
            btnRedo.DataBindings.ExAdd(nameof(btnRedo.Enabled), source, nameof(source.RedoCount), (sneder, e) => {
                e.Value = (int)e.Value > 0;
            });

        }

        /// <summary>
        /// <see cref="IParamCollection"/>資料綁定
        /// </summary>
        /// <param name="source"></param>
        public void Bindings(IParamCollection source) {
            if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);

            /*-- 資料筆數 --*/
            dgvProperties.DataBindings.ExAdd(nameof(dgvProperties.RowCount), source, nameof(source.RowCount));
            lbRowCount.DataBindings.ExAdd(nameof(lbRowCount.Text), source, nameof(source.RowCount),(sender,e) => {
                e.Value = $"Row Count:{e.Value}";
            });
        }

        #endregion Implenent - IDataDisplay

    }

    internal class TestData {
        public object Value { get; internal set; }
    }

    public static class Extenstion {
        
        public static bool ShowOption(this CmsOption option, CmsOption show) {
            return (option & show) == show;
        }

        public static bool DisableOption(this CmsOption option,CmsOption disable) {
            return (option & disable) != disable;
        }
    }

    [Serializable]
    public class CloneClass {
        public int Value { get; set; } = 0;
        public CloneClass(int value) { Value = value; }
        public object Clone() {
            object clone = null;
            using (var memory = new MemoryStream()) {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memory, this);
                memory.Seek(0, SeekOrigin.Begin);
                clone = formatter.Deserialize(memory);
            }
            return clone;
        }
    }

}
