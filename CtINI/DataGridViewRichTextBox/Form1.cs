using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DataGridViewRichTextBox
{
    public partial class Form1 : Form
    {
		DataTable _table = new DataTable();

        public Form1()
        {
            InitializeComponent();

			_table.Columns.Add("col1");
			_table.Columns.Add("col2");

			DataRow row = _table.NewRow();
			row[0] = "Datagridview and richtextbox for bold substring in C#";
			row[1] = "Datagridview and richtextbox for bold substring in C#";
			_table.Rows.Add(row);
			_table.AcceptChanges();
            this.dataGridView1.AutoGenerateColumns = false;
			this.dataGridView1.DataSource = _table;
            string rtf = GetRtf("", "");
            richTextBox1.Rtf = rtf;
            row[0] = rtf;
            row[1] = rtf;
            //string str = "微軟正黑體";
            //byte[] data = Encoding.UTF8.GetBytes(str);
            //int page = Encoding.UTF8.CodePage;
            //string strData = new CtEncoding(page).ToHexs(str);
            //string dd = data.ToHex();
            ////'b4\'fa\'b8\'d5
            //bool isEqual = strData == dd;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.CurrentCell != null )
            {
                //this.dataGridView1.CurrentCell.Value = this.richTextBox1.Rtf;
                this.dataGridView1.Rows[0].Cells[1].Value = this.richTextBox1.Rtf;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.Filter = "RTF file(*.rtf)|*.rtf";

            if (f.ShowDialog() == DialogResult.OK)
            {
                this.richTextBox1.LoadFile(f.FileName);
            }
        }

		private void btnHighlight_Click(object sender, EventArgs e)
		{
			foreach (DataRow row in _table.Rows)
			{
				SetTextBoldByColumn(row, "col1", this.txtSearch.Text);
				SetTextBoldByColumn(row, "col2", this.txtSearch.Text);
            }
            
		}

		private void SetTextBoldByColumn(DataRow row, string colName, string boldText)
		{
			string plainText;
			if (row.HasVersion(DataRowVersion.Original))
				plainText = row[colName, DataRowVersion.Original].ToString();
			else
				plainText = row[colName].ToString();
			row[colName] = GetRtf(plainText, this.txtSearch.Text);
		}

		public string GetRtf(string originalText, string boldText)
		{
            string rtf ="";
            Font rgFont = new Font("微軟正黑體", 12);
            Font mdFont = new Font("微軟正黑體", 12, FontStyle.Italic);
            RtfConvert regularStyle = new RtfConvert();
            regularStyle.Regular = new ExFont(rgFont,Color.Black,Color.Empty);
            regularStyle.Highlight = new ExFont(new Font(rgFont, FontStyle.Bold),Color.Red,Color.Yellow);
            RtfConvert modifiedStyle = new RtfConvert();
            modifiedStyle.Regular = new ExFont(mdFont, Color.Black, Color.Empty);
            modifiedStyle.Highlight = new ExFont(new Font(mdFont,FontStyle.Bold), Color.Red, Color.Yellow); 
            rtf = modifiedStyle.ToRTF("微軟正黑體");
            return rtf;
        }
          
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            //string v = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
            
        }
        
    }
}