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
using System.Xml.Serialization;
using System.Xml;
using VehiclePlanner.Core;

namespace INITesting
{

	/// <summary>
	/// 參數編輯器介面
	/// </summary>
	public partial class CtrlParamEditor : Form, IDataDisplay<IParamEditor>, IDataDisplay<IParamCollection>
	{

		#region Declaration - Fields

		private IParamEditor mEditor = Factory.Param.Editor();

		/// <summary>
		/// 檔案開啟對話視窗
		/// </summary>
		private OpenFileDialog mOdlg = new OpenFileDialog();

		/// <summary>
		/// 檔案儲存對話視窗
		/// </summary>
		private SaveFileDialog mSdlg = new SaveFileDialog();

		private ICellStyles mCellStyle = new CtCellStyles();

		private RtfConvert mRgConvert = new RtfConvert();

		private RtfConvert mRcConvert = new RtfConvert();

		private RtfConvert mMrConvert = new RtfConvert();

		private RtfConvert mMcConvert = new RtfConvert();

		private string mHighlight = null;

		public event EventHandler UplaodParameter;

		public event EventHandler DownloadParameter;
		#endregion Declaration - Fields

		#region Declaration - Properties

		#endregion Declaration - Properties

		#region Function - Constructors

		public CtrlParamEditor()
		{
			InitializeComponent();

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
		private void dgvProperties_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (dgvProperties.Columns[e.ColumnIndex].HeaderText == nameof(IParamColumn.Value))
			{
				mEditor.SelectedColumnName = e.ColumnIndex != -1  ? dgvProperties.Columns[e.ColumnIndex].HeaderText : "";
				mEditor.SelectedRow = e.RowIndex;
				if (e.Button == MouseButtons.Right)
				{
					if (mEditor.ShowOption != CmsOption.None)
					{
						cmsDGV.Show(Cursor.Position);
					}
				}
			}
		}

		/// <summary>
		/// 開啟右鍵選單(增加新參數設定)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dgvProperties_MouseClick(object sender, MouseEventArgs e)
		{
		}

		/// <summary>
		/// 虛擬填充
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dgvProperties_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			string columnName = dgvProperties.Columns[e.ColumnIndex].HeaderText;
			if (e.RowIndex >= mEditor.ParamCollection.RowCount)
			{
				return;
			}
			try
			{
				/*-- 取得欄位資料 --*/
				IParamColumn prop = mEditor.ParamCollection[e.RowIndex] as IParamColumn;
				string v = prop.Default;
				/*-- 取得欄位值 --*/
				string val = prop[columnName].ToString();
				EmColumn emColumn = columnName.ToEnumColumn();

				if ((prop.IlleaglColumn() & emColumn) != EmColumn.None)
				{
					e.Value = mRcConvert.ToRTF("Required cell", mCellStyle.RequiredCell, false);//必填欄位樣式
				}
				else if (prop.ModifiedColumn != EmColumn.None)
				{
					if ((prop.ModifiedColumn & emColumn) != EmColumn.None)
					{
						e.Value = mMcConvert.ToRTF(val, mCellStyle.ModifiedCell);
					}
					else
					{
						e.Value = mMrConvert.ToRTF(val, mCellStyle.ModifiedRow);
					}
				}
				else
				{
					if (e.Value == null || e.Value.ToString() == mRgConvert.ToRTF(val, mCellStyle.Regular))
					{
						e.Value = mRgConvert.ToRTF(val, mCellStyle.Regular);
					}
					else
					{

					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// 儲存格啟用驗證
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dgvProperties_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			RichTextBox rtb = new RichTextBox();
			rtb.Rtf = e.FormattedValue.ToString();
			string columnName = dgvProperties.Columns[e.ColumnIndex].HeaderText;

			if (dgvProperties.Rows[e.RowIndex].Cells[e.ColumnIndex].Value!=null && rtb.Text !=  (mEditor as ParamEditor).DataSource[e.RowIndex][columnName].ToString() )
			{
				(mEditor as ParamEditor).mCommandManager.Edit(rtb.Text, e.RowIndex, dgvProperties.Columns[e.ColumnIndex].HeaderText);
			}
		}

		#endregion DataGridView

		#region ToolStripMenuItem

		/// <summary>
		/// 參數編輯
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miEdit_Click(object sender, EventArgs e)
		{
			var columnName = mEditor.SelectedColumnName;
			/*-- 取得目前選取的資料列 --*/
			IParamColumn prop = mEditor.ParamCollection[mEditor.SelectedRow] as IParamColumn;
			/*-- 使用者輸入 --*/
			mEditor.Edit(columnName);
		}

		/// <summary>
		/// 參數刪除
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miDelete_Click(object sender, EventArgs e)
		{
			mEditor.Remove();
		}

		/// <summary>
		/// 參數加入
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miAdd_Click(object sender, EventArgs e)
		{
			mEditor.Insert();
		}

		#endregion ToolStripMenuItem

		#region Button

		/// <summary>
		/// 參數範本輸出
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnTest_Click(object sender, EventArgs e)
		{
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

		/// <summary>
		/// 參數讀取
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnRead_Click(object sender, EventArgs e)
		{
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

		#region IParamCollection

		/// <summary>
		/// 參數變更事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ParamCollection_DataChanged(object sender, EventArgs e)
		{
			dgvProperties.Refresh();
		}

		#endregion IParamCollection

		#region ToolStrip

		#endregion ToolStrip

		/// <summary>
		/// 開啟檔案
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbOpen_Click(object sender, EventArgs e)
		{
			OpenFile();
		}

		/// <summary>
		/// 儲存檔案
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbSave_Click(object sender, EventArgs e)
		{
			SaveFile();
		}

		/// <summary>
		/// 復原
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbUndo_Click(object sender, EventArgs e)
		{
			Undo();
		}

		/// <summary>
		/// 重做
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbRedo_Click(object sender, EventArgs e)
		{
			Redo();
		}

		/// <summary>
		/// 參數過濾
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbFilter_Click(object sender, EventArgs e)
		{
			Filter();
		}

		/// <summary>
		/// 參數標記
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbHighlight_Click(object sender, EventArgs e)
		{
			Highlight();
		}
		private void tsbDownload_Click(object sender, EventArgs e)
		{
			DownloadParameter?.Invoke(sender, e);
		}

		private void tsbUpload_Click(object sender, EventArgs e)
		{
			UplaodParameter?.Invoke(sender, e);
		}

		#endregion Function - Events

		#region  Function - Private Methods

		/// <summary>
		/// 使用者文字輸入方法委派
		/// </summary>
		/// <param name="result"></param>
		/// <param name="title"></param>
		/// <param name="describe"></param>
		/// <param name="defValue"></param>
		/// <returns></returns>
		private bool InputText(out string result, string title, string describe, string defValue = "")
		{
			return Stat.SUCCESS == CtInput.Text(out result, title, describe, defValue);
		}

		/// <summary>
		/// 使用者ComboBox輸入方法委派
		/// </summary>
		/// <param name="result"></param>
		/// <param name="title"></param>
		/// <param name="describe"></param>
		/// <param name="itemList"></param>
		/// <param name="defValue"></param>
		/// <param name="allowEdit"></param>
		/// <returns></returns>
		private bool ComboBoxList(out string result, string title, string describe, IEnumerable<string> itemList, string defValue = "", bool allowEdit = false)
		{
			return Stat.SUCCESS == CtInput.ComboBoxList(out result, title, describe, itemList, defValue, allowEdit);
		}

		private Color ComplementrayColor(Color color)
		{
			if (color == Color.Empty)
			{
				return Color.Black;
			}
			else
			{
				return Color.FromArgb(color.A, 255 - color.R, 255 - color.G, 255 - color.B);
			}
		}

		/// <summary>
		/// 部署<see cref="DataGridView"/>
		/// </summary>
		/// <param name="dgv"></param>
		private void DeployDGV(DataGridView dgv)
		{
			//int width = dgv.RowHeadersWidth + 3;
			//foreach (DataGridViewColumn col in dgv.Columns) width += col.Width;
			//dgv.Width = width;
			/*-- 關閉欄位名稱自動產生 --*/
			dgv.AutoGenerateColumns = false;
			/*-- 開啟虛擬填充模式 --*/
			dgv.VirtualMode = true;
			/*-- 鎖住直接編輯功能 --*/
			//dgv.ReadOnly = true;
			/*-- 啟用雙緩衝 --*/
			Type dgvType = dgv.GetType();
			PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
			pi.SetValue(dgv, true, null);
			/*-- 虛擬填充事件委派 --*/
			dgv.CellValueNeeded += dgvProperties_CellValueNeeded;
			/*-- 儲存格滑鼠點擊事件委派 --*/
			dgv.CellMouseClick += dgvProperties_CellMouseClick;
			/*-- 滑鼠點擊事件委派 --*/
			dgv.MouseClick += dgvProperties_MouseClick;
			/*--Cell啟用驗證時--*/
			dgv.CellValidating += dgvProperties_CellValidating;
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
			foreach (DataGridViewColumn column in dgv.Columns)
			{
				Console.WriteLine(column.HeaderText);
			}
			int valWidth = 80;
			dgv.Columns[nameof(IParamColumn.Name)].Width = 230;
			dgv.Columns[nameof(IParamColumn.Name)].ReadOnly = true;
			dgv.Columns[nameof(IParamColumn.Value)].Width = valWidth;
			dgv.Columns[nameof(IParamColumn.Type)].Width = 80;
			dgv.Columns[nameof(IParamColumn.Type)].ReadOnly = true;
			dgv.Columns[nameof(IParamColumn.Description)].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			dgv.Columns[nameof(IParamColumn.Description)].ReadOnly = true;
			dgv.Columns[nameof(IParamColumn.Max)].Width = valWidth;
			dgv.Columns[nameof(IParamColumn.Max)].ReadOnly = true;
			dgv.Columns[nameof(IParamColumn.Min)].Width = valWidth;
			dgv.Columns[nameof(IParamColumn.Max)].ReadOnly = true;
			dgv.Columns[nameof(IParamColumn.Default)].Width = valWidth;
			dgv.Columns[nameof(IParamColumn.Default)].ReadOnly = true;
		}

		/// <summary>
		/// 復原
		/// </summary>
		protected void Undo()
		{
			mEditor.Undo();
		}

		/// <summary>
		/// 重做
		/// </summary>
		protected void Redo()
		{
			mEditor.Redo();
		}

		/// <summary>
		/// 參數過濾
		/// </summary>
		protected void Filter()
		{
			mEditor.Filter();
		}

		/// <summary>
		/// 參數標記
		/// </summary>
		protected void Highlight()
		{
			Image img = null;
			if (mHighlight != mEditor.ParamCollection.KeyWord)
			{
				mHighlight = mEditor.ParamCollection.KeyWord;
				img = Properties.Resources.Unhighlight;
			}
			else
			{
				mHighlight = string.Empty;
				img = Properties.Resources.Highlight;
			}
			mMrConvert.KeyWord = mHighlight;
			mMcConvert.KeyWord = mHighlight;
			mRcConvert.KeyWord = mHighlight;
			mRgConvert.KeyWord = mHighlight;
			dgvProperties.Refresh();
			CtInvoke.ToolStripItemImage(tsbHighlight, img);
		}

		/// <summary>
		/// 開啟檔案
		/// </summary>
		protected void OpenFile()
		{
			/*-- 設定要開啟得檔案類型 --*/
			mOdlg.Filter = "Ini File|*.ini";
			mOdlg.Title = "Select a Ini File";
			if (mOdlg.ShowDialog() == DialogResult.OK)
			{
				/*-- 讀取Ini檔 --*/
				mEditor.ReadINI(mOdlg.FileName);
			}
		}

		/// <summary>
		/// 儲存檔案
		/// </summary>
		protected void SaveFile()
		{
			if (mEditor.UndoCount > 0)
			{
				string path = mEditor.IniPath;
				if (string.IsNullOrEmpty(path))
				{
					mSdlg.Filter = "Ini File|*.ini";
					mSdlg.Title = "Select save path";
					mSdlg.FileName = "iTS_Setting.ini";
					if (mSdlg.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(mSdlg.FileName))
					{
						return;
					}
					path = mSdlg.FileName;
				}
				mEditor.SaveToINI(path);
			}
		}

		/// <summary>
		/// 快捷鍵偵測
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Z | Keys.Control:
					Undo();
					break;
				case Keys.Z | Keys.Control | Keys.Shift:
					Redo();
					break;
				case Keys.O | Keys.Control:
					OpenFile();
					break;
				case Keys.S | Keys.Control:
					SaveFile();
					break;
				case Keys.F | Keys.Control:
					Filter();
					break;
				case Keys.L | Keys.Control:
					Highlight();
					break;
				case Keys.C:
					Console.WriteLine("ParamEdit");
					break;
				default:
					return base.ProcessCmdKey(ref msg, keyData);
			}
			return true;
		}

		#endregion Function - Private Methods

		public enum TestEM
		{
			one = 2,
			two = 3,
			three = 4
		}

		#region Implement - IDataDisplay

		/// <summary>
		/// <see cref="IParamEditor"/>資料綁定
		/// </summary>
		/// <param name="source">資料源</param>
		public void Bindings(IParamEditor source)
		{
			if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);
			try
			{
				/*-- Add選項顯示 --*/
				miAdd.DataBindings.ExAdd(nameof(miAdd.Visible), source, nameof(source.ShowOption), (sender, e) =>
				{
					e.Value = ((CmsOption)e.Value).ShowOption(CmsOption.Add);
				}, mEditor.ShowOption.ShowOption(CmsOption.Add));
				/*-- Delete選項 --*/
				miDelete.DataBindings.ExAdd(nameof(miDelete.Visible), source, nameof(source.ShowOption), (sender, e) =>
				{
					e.Value = ((CmsOption)e.Value).ShowOption(CmsOption.Delete);
				}, source.ShowOption.ShowOption(CmsOption.Delete));
				/*-- Edit選項 --*/
				miEdit.DataBindings.ExAdd(nameof(miEdit.Visible), source, nameof(source.ShowOption), (sneder, e) =>
				{
					e.Value = ((CmsOption)e.Value).ShowOption(CmsOption.Edit);
				}, source.ShowOption.ShowOption(CmsOption.Edit));
				/*-- Add Disable --*/
				miAdd.DataBindings.ExAdd(nameof(miAdd.Enabled), source, nameof(source.DisableOption), (sneder, e) =>
				{
					e.Value = ((CmsOption)e.Value).DisableOption(CmsOption.Add);
				}, source.DisableOption.DisableOption(CmsOption.Delete));
				/*-- Delete Disable --*/
				miDelete.DataBindings.ExAdd(nameof(miDelete.Enabled), source, nameof(source.DisableOption), (sender, e) =>
				{
					e.Value = ((CmsOption)e.Value).DisableOption(CmsOption.Delete);
				}, source.DisableOption.DisableOption(CmsOption.Delete));
				/*-- Edit Disable --*/
				miEdit.DataBindings.ExAdd(nameof(miEdit.Enabled), source, nameof(source.DisableOption), (sender, e) =>
				{
					e.Value = ((CmsOption)e.Value).DisableOption(CmsOption.Edit);
				}, source.DisableOption.DisableOption(CmsOption.Edit));
				/*-- 可撤銷次數 --*/
				tsbUndo.DataBindings.ExAdd(nameof(tsbUndo.Enabled), source, nameof(source.UndoCount), (sender, e) =>
				{
					e.Value = (int)e.Value > 0;
				}, source.UndoCount > 0);
				tsbSave.DataBindings.ExAdd(nameof(tsbSave.Enabled), source, nameof(source.UndoCount), (sender, e) =>
				{
					e.Value = (int)e.Value > 0;
				}, source.UndoCount > 0);
				/*-- 可重做次數 --*/
				tsbRedo.DataBindings.ExAdd(nameof(tsbRedo.Enabled), source, nameof(source.RedoCount), (sender, e) =>
				{
					e.Value = (int)e.Value > 0;
				}, source.RedoCount > 0);
				/*-- INI檔路徑 --*/
				tslbPath.DataBindings.ExAdd(nameof(tslbPath.Text), source, nameof(source.IniPath), (sneder, e) =>
				{
					e.Value = $"Path：{e.Value}";
				}, source.IniPath);
			}
			catch (Exception ex)
			{
				CtStatus.Report(Stat.ER_SYSTEM, ex, true);
			}
		}

		/// <summary>
		/// <see cref="IParamCollection"/>資料綁定
		/// </summary>
		/// <param name="source"></param>
		public void Bindings(IParamCollection source)
		{
			if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);
			try
			{
				/*-- 資料筆數 --*/
				dgvProperties.DataBindings.ExAdd(nameof(dgvProperties.RowCount), source, nameof(source.RowCount));
				tslbCount.DataBindings.ExAdd(nameof(tslbCount.Text), source, nameof(source.RowCount), (sneder, e) =>
				{
					e.Value = $"Count：{(int)e.Value}";
				}, (int)source.RowCount);
				/*-- 是否已過濾資料 --*/
				tsbFilter.DataBindings.ExAdd(nameof(tsbFilter.Image), source, nameof(source.IsFilterMode), (sender, e) =>
				{
					e.Value = (bool)e.Value ? Properties.Resources.Unfilter : Properties.Resources.Filter;
				}, source.IsFilterMode ? Properties.Resources.Unfilter : Properties.Resources.Filter);
				/*-- 過濾關鍵字 --*/
				tstKeyWord.DataBindings.ExAdd(nameof(tstKeyWord.Text), source, nameof(source.KeyWord));
			}
			catch (Exception ex)
			{
				CtStatus.Report(Stat.ER_SYSTEM, ex, true);
			}
		}

		#endregion Implenent - IDataDisplay

		private void CtrlParamEditor_Load(object sender, EventArgs e)
		{
		}

		private void tstKeyWord_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control)
			{
				e.Handled = true;
			}
		}

		private void CtrlParamEditor_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control)
			{
				switch (e.KeyCode)
				{
					case Keys.Z:
						if (e.Shift)
						{
							Redo();
						}
						else
						{
							Undo();
						}
						break;
					case Keys.F:
						Filter();
						break;
					case Keys.L:
						Highlight();
						break;
					case Keys.O:
						OpenFile();
						break;
					case Keys.S:
						SaveFile();
						break;
				}
			}
		}
	}

	internal class TestData
	{
		public object Value { get; internal set; }
	}

	public static class Extenstion
	{

		public static bool ShowOption(this CmsOption option, CmsOption show)
		{
			return (option & show) == show;
		}

		public static bool DisableOption(this CmsOption option, CmsOption disable)
		{
			return (option & disable) != disable;
		}
	}

	[Serializable]
	public class CloneClass
	{
		public int Value { get; set; } = 0;
		public CloneClass(int value) { Value = value; }
		public object Clone()
		{
			object clone = null;
			using (var memory = new MemoryStream())
			{
				IFormatter formatter = new BinaryFormatter();
				formatter.Serialize(memory, this);
				memory.Seek(0, SeekOrigin.Begin);
				clone = formatter.Deserialize(memory);
			}
			return clone;
		}
	}

}
