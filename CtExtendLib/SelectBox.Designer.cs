namespace CtExtendLib
{
	partial class SelectBox
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.dgvSelect = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.dgvSelect)).BeginInit();
			this.SuspendLayout();
			// 
			// dgvSelect
			// 
			this.dgvSelect.AllowUserToAddRows = false;
			this.dgvSelect.AllowUserToDeleteRows = false;
			this.dgvSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dgvSelect.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvSelect.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
			this.dgvSelect.BackgroundColor = System.Drawing.SystemColors.ControlLight;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvSelect.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.dgvSelect.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSelect.Location = new System.Drawing.Point(14, 15);
			this.dgvSelect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.dgvSelect.Name = "dgvSelect";
			this.dgvSelect.ReadOnly = true;
			this.dgvSelect.RowTemplate.Height = 27;
			this.dgvSelect.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.dgvSelect.Size = new System.Drawing.Size(455, 203);
			this.dgvSelect.TabIndex = 0;
			// 
			// SelectBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(482, 234);
			this.Controls.Add(this.dgvSelect);
			this.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(500, 281);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(500, 281);
			this.Name = "SelectBox";
			this.Text = "SelectBox";
			((System.ComponentModel.ISupportInitialize)(this.dgvSelect)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dgvSelect;
	}
}