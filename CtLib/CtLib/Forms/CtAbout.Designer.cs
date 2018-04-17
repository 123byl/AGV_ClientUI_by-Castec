namespace CtLib.Forms {
    partial class CtAbout {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtAbout));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			this.lbProduct = new System.Windows.Forms.Label();
			this.lbVersion = new System.Windows.Forms.Label();
			this.lbVersion_val = new System.Windows.Forms.Label();
			this.lbDate_val = new System.Windows.Forms.Label();
			this.lbDate = new System.Windows.Forms.Label();
			this.lbCopyright = new System.Windows.Forms.Label();
			this.txtDescription = new System.Windows.Forms.TextBox();
			this.pnModule = new System.Windows.Forms.Panel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pbLogo = new System.Windows.Forms.PictureBox();
			this.dgvModule = new System.Windows.Forms.DataGridView();
			this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colVer = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.pnModule.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvModule)).BeginInit();
			this.SuspendLayout();
			// 
			// lbProduct
			// 
			resources.ApplyResources(this.lbProduct, "lbProduct");
			this.lbProduct.Name = "lbProduct";
			// 
			// lbVersion
			// 
			resources.ApplyResources(this.lbVersion, "lbVersion");
			this.lbVersion.Name = "lbVersion";
			// 
			// lbVersion_val
			// 
			resources.ApplyResources(this.lbVersion_val, "lbVersion_val");
			this.lbVersion_val.Name = "lbVersion_val";
			// 
			// lbDate_val
			// 
			resources.ApplyResources(this.lbDate_val, "lbDate_val");
			this.lbDate_val.Name = "lbDate_val";
			// 
			// lbDate
			// 
			resources.ApplyResources(this.lbDate, "lbDate");
			this.lbDate.Name = "lbDate";
			// 
			// lbCopyright
			// 
			resources.ApplyResources(this.lbCopyright, "lbCopyright");
			this.lbCopyright.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lbCopyright.Name = "lbCopyright";
			// 
			// txtDescription
			// 
			resources.ApplyResources(this.txtDescription, "txtDescription");
			this.txtDescription.Name = "txtDescription";
			this.txtDescription.ReadOnly = true;
			this.txtDescription.TabStop = false;
			// 
			// pnModule
			// 
			this.pnModule.Controls.Add(this.pictureBox1);
			resources.ApplyResources(this.pnModule, "pnModule");
			this.pnModule.Name = "pnModule";
			// 
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = global::CtLib.Properties.Resources.Bar_Vertial;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// pbLogo
			// 
			resources.ApplyResources(this.pbLogo, "pbLogo");
			this.pbLogo.Image = global::CtLib.Properties.Resources.CASTEC_Logo_Vertical;
			this.pbLogo.Name = "pbLogo";
			this.pbLogo.TabStop = false;
			// 
			// dgvModule
			// 
			this.dgvModule.AllowUserToAddRows = false;
			this.dgvModule.AllowUserToDeleteRows = false;
			this.dgvModule.AllowUserToResizeColumns = false;
			this.dgvModule.AllowUserToResizeRows = false;
			resources.ApplyResources(this.dgvModule, "dgvModule");
			this.dgvModule.BackgroundColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvModule.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvModule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvModule.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colVer});
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle4.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvModule.DefaultCellStyle = dataGridViewCellStyle4;
			this.dgvModule.MultiSelect = false;
			this.dgvModule.Name = "dgvModule";
			this.dgvModule.ReadOnly = true;
			this.dgvModule.RowHeadersVisible = false;
			this.dgvModule.RowTemplate.Height = 27;
			this.dgvModule.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			// 
			// colName
			// 
			this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
			this.colName.DefaultCellStyle = dataGridViewCellStyle2;
			resources.ApplyResources(this.colName, "colName");
			this.colName.Name = "colName";
			this.colName.ReadOnly = true;
			this.colName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.colName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// colVer
			// 
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.colVer.DefaultCellStyle = dataGridViewCellStyle3;
			resources.ApplyResources(this.colVer, "colVer");
			this.colVer.Name = "colVer";
			this.colVer.ReadOnly = true;
			this.colVer.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.colVer.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// CtAbout
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.dgvModule);
			this.Controls.Add(this.pnModule);
			this.Controls.Add(this.txtDescription);
			this.Controls.Add(this.lbCopyright);
			this.Controls.Add(this.lbDate_val);
			this.Controls.Add(this.lbDate);
			this.Controls.Add(this.lbVersion_val);
			this.Controls.Add(this.lbVersion);
			this.Controls.Add(this.lbProduct);
			this.Controls.Add(this.pbLogo);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CtAbout";
			this.pnModule.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvModule)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbLogo;
        private System.Windows.Forms.Label lbProduct;
        private System.Windows.Forms.Label lbVersion;
        private System.Windows.Forms.Label lbVersion_val;
        private System.Windows.Forms.Label lbDate_val;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Label lbCopyright;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel pnModule;
        private System.Windows.Forms.DataGridView dgvModule;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVer;
    }
}