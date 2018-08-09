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
		public static SelectBox Show(string title, DataTable table)
		{
			return new SelectBox(title, table,"Go To");
		}
		public SelectBox()
		{
			InitializeComponent();
		}

		public SelectBox(string title, DataTable table,string btnText = "Click") : this()
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
		}
		private void Dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (dgvSelect.Columns[e.ColumnIndex].HeaderText == _selectHead)
			{
				SelectRow = _table.NewRow();
				for (int i = 0; i < dgvSelect.ColumnCount - 2; i++)
				{
					var name = dgvSelect.Columns[i].HeaderText;
					SelectRow[name] = dgvSelect.Rows[e.RowIndex].Cells[name].Value;
				}
				this.Close();
			}
		}

	}
}
