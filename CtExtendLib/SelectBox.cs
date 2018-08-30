using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtExtendLib
{
	public partial class SelectBox : Form
	{
	
		public DataRow SelectRow { get; private set; }
		private DataTable _table;
		private const string _selectHead = "Click Button";
		private string _highlight = null;
		public static SelectBox Show(string title, DataTable table)
		{
			return new SelectBox(title, table,"Go To");
		}
		public SelectBox()
		{
			InitializeComponent();
			dgvSelect.Paint += Dgv_Paint;
			dgvSelect.SelectionChanged += Dgv_SelectionChanged;
		}

		public SelectBox(string title, DataTable table,string btnText = "Click",string highlight = null) : this()
		{
			Text = title;
			_table = table;
			dgvSelect.AutoGenerateColumns = false;
			foreach (DataColumn col in table.Columns)
			{
				DataGridViewTextBoxColumn txtCol = new DataGridViewTextBoxColumn();
				txtCol.Name = col.ColumnName;
				txtCol.HeaderText = col.ColumnName;
				txtCol.DataPropertyName = col.ColumnName;
				txtCol.ReadOnly = true;
				txtCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			
				dgvSelect.Columns.Add(txtCol);
			}
			DataGridViewButtonColumn btnCol = new DataGridViewButtonColumn();
			btnCol.Text = btnText;
			btnCol.UseColumnTextForButtonValue = true;
			btnCol.FlatStyle = FlatStyle.Flat;
			btnCol.Name = "Select";
			btnCol.HeaderText = _selectHead;
			dgvSelect.Columns.Add(btnCol);
			dgvSelect.CellContentClick += Dgv_CellContentClick;
			dgvSelect.DataSource = table;
			_highlight = highlight;
		}

		private void Dgv_Paint(object sender ,EventArgs e )
		{
			if (_highlight != null)
			{
				for (int i = 0; i < dgvSelect.RowCount; i++)
				{
					if (dgvSelect.Rows[i].Cells[0].Value.ToString() == _highlight)
					{
						dgvSelect.Rows[i].Cells[0].Style.BackColor= Color.LightGreen;
					}
				}
			}
		}

		private void Dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (dgvSelect.Columns[e.ColumnIndex].HeaderText == _selectHead && e.RowIndex !=- 1)
			{
				SelectRow = _table.NewRow();
				for (int i = 0; i <= dgvSelect.ColumnCount - 2; i++)
				{
					var name = dgvSelect.Columns[i].HeaderText;
					SelectRow[name] = dgvSelect.Rows[e.RowIndex].Cells[name].Value;
				}
				this.Close();
			}
		}
		private void Dgv_SelectionChanged(object sender ,EventArgs e )
		{
			dgvSelect.ClearSelection();
		}

	}
}
