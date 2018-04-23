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

namespace CtTesting {

    public partial class CtrlParamEditor : Form ,IDataDisplay<IParamEditor>{

        #region Declaration - Fields

        private IParamEditor mEditor = Factory.Param.Editor();

        private CtFieldEditor mFieldEditor = new CtFieldEditor();

        /// <summary>
        /// 檔案開啟對話視窗
        /// </summary>
        private OpenFileDialog mOdlg = new OpenFileDialog();

        #endregion Declaration - Fields

        #region Declaration - Enum

        #endregion Declaration - Enum

        #region Function - Constructors

        public CtrlParamEditor() {
            InitializeComponent();
            /*-- DataGridView寬度預設 --*/
            int width = dgvProperties.RowHeadersWidth +3;
            foreach (DataGridViewColumn col in dgvProperties.Columns) width += col.Width;
            dgvProperties.Width = width;
            mEditor.GridView = dgvProperties;
            mFieldEditor.InputText = InputText;
            mFieldEditor.ComboBoxList = ComboBoxList;

            dgvProperties.CellMouseClick += dgvProperties_CellMouseClick;
            dgvProperties.CellMouseDoubleClick += dgvProperties_CellMouseDoubleClick;
            dgvProperties.MouseClick += dgvProperties_MouseClick;

            Bindings(mEditor);
        }

        #endregion Function - Constructors

        #region Function - Evnets

        #region DataGridView

        private void dgvProperties_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            //dgvProperties.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "Modified";
           
        }

        /// <summary>
        /// 以儲存格為對象開啟右鍵選單
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvProperties_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            mEditor.SelectedColumn = e.ColumnIndex;
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

        #endregion DataGridView

        #region ToolStripMenuItem

        private void miEdit_Click(object sender, EventArgs e) {
            /*-- 取得目前選取的資料列 --*/
            IParam prop = mEditor.ParamCollection[mEditor.SelectedRow] as IParam;
            /*-- 使用者輸入 --*/
            if (mFieldEditor.Edit(mEditor.SelectedColumn, prop,out string returnValue)) {
                /*-- 寫入使用者輸入 --*/
                mEditor.SetValue(returnValue);
                dgvProperties.InvokeIfNecessary(() => {
                    dgvProperties.Refresh();
                });
            }
        }

        private void miDelete_Click(object sender, EventArgs e) {
            mEditor.Remove();
            dgvProperties.Refresh();
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

        private void btnHighlight_Click(object sender, EventArgs e) {
            mEditor.Highlight(txtKeyWord.Text);
            dgvProperties.Refresh();
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

        #endregion Function - Private Methods
        public enum TestEM {
            one = 2,
            two = 3,
            three = 4
        }

        /// <summary>
        /// 參數範本輸出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click(object sender, EventArgs e) {
            //參數設定樣本輸出
            if (mEditor.GridView != null)mEditor.GridView = null;
            mEditor.Clear();
            mEditor.WriteParam("IntName",20,"IntDescription",20);
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
        /// 參數讀取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRead_Click(object sender, EventArgs e) {
            if (mEditor.GridView != null) mEditor.GridView = null;
            int iVal = 0;
            float fVal = 0f;
            bool bVal = false;
            string sVal = string.Empty;
            TestEM emVal = TestEM.two;
            TestData data = new TestData();
            mEditor.Clear();
            mEditor.ReadINI(@"D:\Test1122.ini");
            mEditor.ParamCollection.FindVal<int>("IntName",val => data.Value = val);
            mEditor.ParamCollection.FindVal("IntName", ref iVal);
            mEditor.ParamCollection.FindVal("FloatName", ref fVal);
            mEditor.ParamCollection.FindVal("BoolName", ref bVal);
            mEditor.ParamCollection.FindVal("StringName", ref sVal);
            mEditor.ParamCollection.FindVal("EnumName", ref emVal);
        }
        public class TestData {
            public int Value { get; set; } = 0;
        }

        #region Implement - IDataDisplay<IParamEditor>

        /// <summary>
        /// 資料綁定
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
        }

        #endregion Implenent - IDataDisplay<IParamEditor>


    }

    public static class Extenstion {
        
        public static bool ShowOption(this CmsOption option, CmsOption show) {
            return (option & show) == show;
        }

        public static bool DisableOption(this CmsOption option,CmsOption disable) {
            return (option & disable) != disable;
        }
    }

}
