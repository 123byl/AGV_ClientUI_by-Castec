using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VehiclePlanner.Module;
using CtLib.Module.Modbus;
using CtLib.Module.Utility;
using VehiclePlanner;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using CtExtendLib;
using System.Reflection;
using System.Diagnostics;

namespace CtItsParameter
{
	public partial class ItsParameterCtrl : AuthorityDockContainer
	{
		#region Declaration - Fields
		private ParameterEditor _parameterEditor;
		private string _currentPath;
		#endregion

		#region Declaration - Properties
		private OperateRecorder Recorder => OperateRecorder.Record;
		private string DesktopPath => System.Environment.SpecialFolder.Desktop.ToString();
		#endregion


		#region Function- Constructors
		public ItsParameterCtrl() : base()
		{
			InitializeComponent();

			DgvInitialize();
		}

		public ItsParameterCtrl(BaseVehiclePlanner_Ctrl refUI, DockState defState) : base(refUI, defState)
		{
			InitializeComponent();
			DgvInitialize();
		}
		#endregion

		#region Function - Override
		public override bool IsVisiable(AccessLevel lv)
		{
			return lv > AccessLevel.None;
		}

		#endregion

		#region Function - Public

		#endregion

		#region Function - Private
		private void OpenFile(string path = null)
		{
			if (path == null)
			{
				OpenFileDialog ctrl = new OpenFileDialog() { Filter = "Ini File(*.ini)|*.ini", Title = "Select Ini File", InitialDirectory = DesktopPath };
				if (ctrl.ShowDialog() == DialogResult.OK)
				{
					path = ctrl.FileName;
					if (File.Exists(path))
					{
						_parameterEditor.Read(path);
						_currentPath = path;
					};
				}
			}
			else if (File.Exists(path))
			{
				_parameterEditor.Read(path);
				_currentPath = path;
			}
		}
		private void DgvInitialize()
		{
			_parameterEditor = new ParameterEditor();
			_parameterEditor.LoadFinish += ParameterEditor_LoadFinish;
			_parameterEditor.ParameterChange += ParameterEditor_ParameterChange;
			dgvParameter.AllowUserToResizeRows = false;
			dgvParameter.AutoGenerateColumns = false;
			dgvParameter.VirtualMode = true;
			//dgvParameter.DoubleMode(true);
			Type dgvType = dgvParameter.GetType();
			PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
			pi.SetValue(dgvParameter, true, null);
			dgvParameter.ShowCellToolTips = false;
			dgvParameter.CellValueNeeded += dgvParameter_CellValueNeeded;
			dgvParameter.CellValidating += dgvParameter_CellValidating;
			//dgvParameter.DataSource = _parameterEditor.ParameterTable;
			dgvParameter.Columns[nameof(colName)].DataPropertyName = ParameterEditor.NameWord;
			dgvParameter.Columns[nameof(colType)].DataPropertyName = ParameterEditor.TypeWord;
			dgvParameter.Columns[nameof(colValue)].DataPropertyName = ParameterEditor.ValueWord;
			dgvParameter.Columns[nameof(colDefault)].DataPropertyName = ParameterEditor.DefaultWord;
			dgvParameter.Columns[nameof(colMin)].DataPropertyName = ParameterEditor.MaxWord;
			dgvParameter.Columns[nameof(colMax)].DataPropertyName = ParameterEditor.MinWord;
			dgvParameter.Columns[nameof(colDescription)].DataPropertyName = ParameterEditor.DescriptionWord;
		}
		private void ParameterEditor_LoadFinish(object sender, EventArgs e)
		{
			dgvParameter.RowCount = _parameterEditor.ParameterTable.Rows.Count;
		}
		private void ParameterEditor_ParameterChange(object sender , ParameterHeaderEventArgs e)
		{
			tsbUndo.Enabled = e.UndoCount > 0 ? true : false;
			tsbRedo.Enabled = e.RedoCount > 0 ? true : false;
			tsbSaveFile.Enabled = e.IsLoad ? true : false;
		}

		private void dgvParameter_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			if (dgvParameter.Columns[e.ColumnIndex].HeaderText == ParameterEditor.ValueWord && e.FormattedValue != null)
			{
				var name = dgvParameter.Rows[e.RowIndex].Cells[nameof(colName)].Value.ToString().RtfToString();
				var type = dgvParameter.Rows[e.RowIndex].Cells[nameof(colType)].Value.ToString().RtfToString();
				string value;
				if (e.FormattedValue.ToString().StartsWith(@"{\rtf")) value = new RichTextBox() { Rtf = e.FormattedValue.ToString() }.Text;
				else value = e.FormattedValue.ToString();
				_parameterEditor.Edit(name, type, value);
			}
		}
		private void dgvParameter_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			if (dgvParameter.RowCount != _parameterEditor.ParameterTable.Rows.Count)
			{
				if (dgvParameter.RowCount < _parameterEditor.ParameterTable.Rows.Count) dgvParameter.RowCount = _parameterEditor.ParameterTable.Rows.Count;
				else if (dgvParameter.RowCount > _parameterEditor.ParameterTable.Rows.Count) return;
			}
			e.Value = _parameterEditor.ParameterTable.DefaultView.ToTable().Rows[e.RowIndex][dgvParameter.Columns[e.ColumnIndex].HeaderText].ToString();
			stopwatch.Stop();
			Console.WriteLine(stopwatch.Elapsed);
		}
		#endregion

		#region Function - Events
		private void tsbOpenFile_Click(object sender, EventArgs e)
		{
			_parameterEditor.Clear();
			OpenFile();
		}

		private void tsbSaveFile_Click(object sender, EventArgs e)
		{
			SaveFileDialog ctrl = new SaveFileDialog() { Filter = "Ini File(*.ini)|*.ini", Title = "Save Ini File", InitialDirectory = DesktopPath };
			if (ctrl.ShowDialog() == DialogResult.OK)
			{
				_parameterEditor.Save(ctrl.FileName);
				dgvParameter.Refresh();
				dgvParameter.RowCount = 0;
				OpenFile(ctrl.FileName);
			}
		}


		private void tsbDownload_Click(object sender, EventArgs e)
		{
			RefUI.DownloadParameter();
		}

		private void tsbUpload_Click(object sender, EventArgs e)
		{
			RefUI.UploadParameter();
		}
		#endregion

		private void tsbUndo_Click(object sender, EventArgs e)
		{
			_parameterEditor.Undo();
			dgvParameter.Refresh();
		}

		private void tsbRedo_Click(object sender, EventArgs e)
		{
			_parameterEditor.Redo();
			dgvParameter.Refresh();
		}
	}
}
