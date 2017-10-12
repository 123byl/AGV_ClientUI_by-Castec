namespace CtLib.Module.Adept {
	partial class CtAceDimmerSetting {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtAceDimmerSetting));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
			this.dgvChannel = new System.Windows.Forms.DataGridView();
			this.colCh = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.colStyle = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.colColor = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.colCmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dgvDimmer = new System.Windows.Forms.DataGridView();
			this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colBrand = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colPort = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.lbDimmerName = new System.Windows.Forms.Label();
			this.btnExport = new System.Windows.Forms.Button();
			this.btnImport = new System.Windows.Forms.Button();
			this.btnLeaveDim = new System.Windows.Forms.Button();
			this.btnSaveDim = new System.Windows.Forms.Button();
			this.btnEditDim = new System.Windows.Forms.Button();
			this.btnDeleteDim = new System.Windows.Forms.Button();
			this.btnAddDim = new System.Windows.Forms.Button();
			this.btnLeave = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnEdit = new System.Windows.Forms.Button();
			this.btnExit = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.dgvChannel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvDimmer)).BeginInit();
			this.SuspendLayout();
			// 
			// dgvChannel
			// 
			resources.ApplyResources(this.dgvChannel, "dgvChannel");
			this.dgvChannel.AllowUserToAddRows = false;
			this.dgvChannel.AllowUserToDeleteRows = false;
			this.dgvChannel.AllowUserToResizeColumns = false;
			this.dgvChannel.AllowUserToResizeRows = false;
			dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle11.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvChannel.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
			this.dgvChannel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvChannel.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCh,
            this.colStyle,
            this.colColor,
            this.colCmt});
			dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle15.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvChannel.DefaultCellStyle = dataGridViewCellStyle15;
			this.dgvChannel.MultiSelect = false;
			this.dgvChannel.Name = "dgvChannel";
			this.dgvChannel.RowHeadersVisible = false;
			this.dgvChannel.RowTemplate.Height = 24;
			this.toolTip.SetToolTip(this.dgvChannel, resources.GetString("dgvChannel.ToolTip"));
			this.dgvChannel.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLight_CellEndEdit);
			// 
			// colCh
			// 
			dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle12.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colCh.DefaultCellStyle = dataGridViewCellStyle12;
			this.colCh.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
			resources.ApplyResources(this.colCh, "colCh");
			this.colCh.Name = "colCh";
			this.colCh.ReadOnly = true;
			this.colCh.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.colCh.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// colStyle
			// 
			dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle13.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colStyle.DefaultCellStyle = dataGridViewCellStyle13;
			this.colStyle.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
			resources.ApplyResources(this.colStyle, "colStyle");
			this.colStyle.Name = "colStyle";
			this.colStyle.ReadOnly = true;
			this.colStyle.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// colColor
			// 
			dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle14.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colColor.DefaultCellStyle = dataGridViewCellStyle14;
			this.colColor.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
			resources.ApplyResources(this.colColor, "colColor");
			this.colColor.Name = "colColor";
			this.colColor.ReadOnly = true;
			this.colColor.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// colCmt
			// 
			this.colCmt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(this.colCmt, "colCmt");
			this.colCmt.Name = "colCmt";
			this.colCmt.ReadOnly = true;
			this.colCmt.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.colCmt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// dgvDimmer
			// 
			resources.ApplyResources(this.dgvDimmer, "dgvDimmer");
			this.dgvDimmer.AllowUserToAddRows = false;
			this.dgvDimmer.AllowUserToDeleteRows = false;
			this.dgvDimmer.AllowUserToResizeColumns = false;
			this.dgvDimmer.AllowUserToResizeRows = false;
			dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle16.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle16.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle16.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle16.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle16.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvDimmer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle16;
			this.dgvDimmer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvDimmer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colBrand,
            this.colPort});
			dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle20.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle20.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle20.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle20.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle20.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle20.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvDimmer.DefaultCellStyle = dataGridViewCellStyle20;
			this.dgvDimmer.MultiSelect = false;
			this.dgvDimmer.Name = "dgvDimmer";
			this.dgvDimmer.RowHeadersVisible = false;
			this.dgvDimmer.RowTemplate.Height = 24;
			this.toolTip.SetToolTip(this.dgvDimmer, resources.GetString("dgvDimmer.ToolTip"));
			this.dgvDimmer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDimmer_CellDoubleClick);
			// 
			// colName
			// 
			this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle17.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colName.DefaultCellStyle = dataGridViewCellStyle17;
			resources.ApplyResources(this.colName, "colName");
			this.colName.Name = "colName";
			this.colName.ReadOnly = true;
			this.colName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// colBrand
			// 
			dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle18.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colBrand.DefaultCellStyle = dataGridViewCellStyle18;
			resources.ApplyResources(this.colBrand, "colBrand");
			this.colBrand.Name = "colBrand";
			this.colBrand.ReadOnly = true;
			// 
			// colPort
			// 
			dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle19.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colPort.DefaultCellStyle = dataGridViewCellStyle19;
			resources.ApplyResources(this.colPort, "colPort");
			this.colPort.Name = "colPort";
			this.colPort.ReadOnly = true;
			this.colPort.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// lbDimmerName
			// 
			resources.ApplyResources(this.lbDimmerName, "lbDimmerName");
			this.lbDimmerName.Name = "lbDimmerName";
			this.toolTip.SetToolTip(this.lbDimmerName, resources.GetString("lbDimmerName.ToolTip"));
			// 
			// btnExport
			// 
			resources.ApplyResources(this.btnExport, "btnExport");
			this.btnExport.BackgroundImage = global::CtLib.Properties.Resources.Eexport;
			this.btnExport.FlatAppearance.BorderSize = 0;
			this.btnExport.Name = "btnExport";
			this.toolTip.SetToolTip(this.btnExport, resources.GetString("btnExport.ToolTip"));
			this.btnExport.UseVisualStyleBackColor = true;
			this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
			// 
			// btnImport
			// 
			resources.ApplyResources(this.btnImport, "btnImport");
			this.btnImport.BackgroundImage = global::CtLib.Properties.Resources.Import;
			this.btnImport.FlatAppearance.BorderSize = 0;
			this.btnImport.Name = "btnImport";
			this.toolTip.SetToolTip(this.btnImport, resources.GetString("btnImport.ToolTip"));
			this.btnImport.UseVisualStyleBackColor = true;
			this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
			// 
			// btnLeaveDim
			// 
			resources.ApplyResources(this.btnLeaveDim, "btnLeaveDim");
			this.btnLeaveDim.BackgroundImage = global::CtLib.Properties.Resources.Exit;
			this.btnLeaveDim.FlatAppearance.BorderSize = 0;
			this.btnLeaveDim.Name = "btnLeaveDim";
			this.toolTip.SetToolTip(this.btnLeaveDim, resources.GetString("btnLeaveDim.ToolTip"));
			this.btnLeaveDim.UseVisualStyleBackColor = true;
			this.btnLeaveDim.Click += new System.EventHandler(this.btnLeaveDim_Click);
			// 
			// btnSaveDim
			// 
			resources.ApplyResources(this.btnSaveDim, "btnSaveDim");
			this.btnSaveDim.BackgroundImage = global::CtLib.Properties.Resources.Save_2;
			this.btnSaveDim.FlatAppearance.BorderSize = 0;
			this.btnSaveDim.Name = "btnSaveDim";
			this.toolTip.SetToolTip(this.btnSaveDim, resources.GetString("btnSaveDim.ToolTip"));
			this.btnSaveDim.UseVisualStyleBackColor = true;
			this.btnSaveDim.Click += new System.EventHandler(this.btnSaveDim_Click);
			// 
			// btnEditDim
			// 
			resources.ApplyResources(this.btnEditDim, "btnEditDim");
			this.btnEditDim.BackgroundImage = global::CtLib.Properties.Resources.Edit;
			this.btnEditDim.FlatAppearance.BorderSize = 0;
			this.btnEditDim.Name = "btnEditDim";
			this.toolTip.SetToolTip(this.btnEditDim, resources.GetString("btnEditDim.ToolTip"));
			this.btnEditDim.UseVisualStyleBackColor = true;
			this.btnEditDim.Click += new System.EventHandler(this.btnEditDim_Click);
			// 
			// btnDeleteDim
			// 
			resources.ApplyResources(this.btnDeleteDim, "btnDeleteDim");
			this.btnDeleteDim.BackgroundImage = global::CtLib.Properties.Resources.Delete;
			this.btnDeleteDim.FlatAppearance.BorderSize = 0;
			this.btnDeleteDim.Name = "btnDeleteDim";
			this.toolTip.SetToolTip(this.btnDeleteDim, resources.GetString("btnDeleteDim.ToolTip"));
			this.btnDeleteDim.UseVisualStyleBackColor = true;
			this.btnDeleteDim.Click += new System.EventHandler(this.btnDeleteDim_Click);
			// 
			// btnAddDim
			// 
			resources.ApplyResources(this.btnAddDim, "btnAddDim");
			this.btnAddDim.BackgroundImage = global::CtLib.Properties.Resources.Add;
			this.btnAddDim.FlatAppearance.BorderSize = 0;
			this.btnAddDim.Name = "btnAddDim";
			this.toolTip.SetToolTip(this.btnAddDim, resources.GetString("btnAddDim.ToolTip"));
			this.btnAddDim.UseVisualStyleBackColor = true;
			this.btnAddDim.Click += new System.EventHandler(this.btnAddDim_Click);
			// 
			// btnLeave
			// 
			resources.ApplyResources(this.btnLeave, "btnLeave");
			this.btnLeave.BackgroundImage = global::CtLib.Properties.Resources.Exit;
			this.btnLeave.FlatAppearance.BorderSize = 0;
			this.btnLeave.Name = "btnLeave";
			this.toolTip.SetToolTip(this.btnLeave, resources.GetString("btnLeave.ToolTip"));
			this.btnLeave.UseVisualStyleBackColor = true;
			this.btnLeave.Click += new System.EventHandler(this.btnLeave_Click);
			// 
			// btnSave
			// 
			resources.ApplyResources(this.btnSave, "btnSave");
			this.btnSave.BackgroundImage = global::CtLib.Properties.Resources.Save_2;
			this.btnSave.FlatAppearance.BorderSize = 0;
			this.btnSave.Name = "btnSave";
			this.toolTip.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnDelete
			// 
			resources.ApplyResources(this.btnDelete, "btnDelete");
			this.btnDelete.BackgroundImage = global::CtLib.Properties.Resources.Delete;
			this.btnDelete.FlatAppearance.BorderSize = 0;
			this.btnDelete.Name = "btnDelete";
			this.toolTip.SetToolTip(this.btnDelete, resources.GetString("btnDelete.ToolTip"));
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnAdd
			// 
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.BackgroundImage = global::CtLib.Properties.Resources.Add;
			this.btnAdd.FlatAppearance.BorderSize = 0;
			this.btnAdd.Name = "btnAdd";
			this.toolTip.SetToolTip(this.btnAdd, resources.GetString("btnAdd.ToolTip"));
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnEdit
			// 
			resources.ApplyResources(this.btnEdit, "btnEdit");
			this.btnEdit.BackgroundImage = global::CtLib.Properties.Resources.Edit;
			this.btnEdit.FlatAppearance.BorderSize = 0;
			this.btnEdit.Name = "btnEdit";
			this.toolTip.SetToolTip(this.btnEdit, resources.GetString("btnEdit.ToolTip"));
			this.btnEdit.UseVisualStyleBackColor = true;
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// btnExit
			// 
			resources.ApplyResources(this.btnExit, "btnExit");
			this.btnExit.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
			this.btnExit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnExit.Image = global::CtLib.Properties.Resources.Exit;
			this.btnExit.Name = "btnExit";
			this.toolTip.SetToolTip(this.btnExit, resources.GetString("btnExit.ToolTip"));
			this.btnExit.UseVisualStyleBackColor = true;
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// CtAceDimmerSetting
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ControlBox = false;
			this.Controls.Add(this.btnExport);
			this.Controls.Add(this.btnImport);
			this.Controls.Add(this.btnLeaveDim);
			this.Controls.Add(this.btnSaveDim);
			this.Controls.Add(this.btnEditDim);
			this.Controls.Add(this.lbDimmerName);
			this.Controls.Add(this.dgvDimmer);
			this.Controls.Add(this.btnDeleteDim);
			this.Controls.Add(this.btnAddDim);
			this.Controls.Add(this.btnLeave);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.dgvChannel);
			this.Controls.Add(this.btnExit);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CtAceDimmerSetting";
			this.ShowInTaskbar = false;
			this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
			this.TopMost = true;
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.CtAceDimmerSetting_Paint);
			((System.ComponentModel.ISupportInitialize)(this.dgvChannel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvDimmer)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.DataGridView dgvChannel;
		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnLeave;
		private System.Windows.Forms.Button btnDeleteDim;
		private System.Windows.Forms.Button btnAddDim;
		private System.Windows.Forms.DataGridView dgvDimmer;
		private System.Windows.Forms.Label lbDimmerName;
		private System.Windows.Forms.Button btnLeaveDim;
		private System.Windows.Forms.Button btnSaveDim;
		private System.Windows.Forms.Button btnEditDim;
		private System.Windows.Forms.DataGridViewTextBoxColumn colName;
		private System.Windows.Forms.DataGridViewTextBoxColumn colBrand;
		private System.Windows.Forms.DataGridViewTextBoxColumn colPort;
		private System.Windows.Forms.Button btnImport;
		private System.Windows.Forms.Button btnExport;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.DataGridViewComboBoxColumn colCh;
		private System.Windows.Forms.DataGridViewComboBoxColumn colStyle;
		private System.Windows.Forms.DataGridViewComboBoxColumn colColor;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCmt;
	}
}