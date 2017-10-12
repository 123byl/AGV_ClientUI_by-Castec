namespace CtLib.Module.Adept {
	partial class CtAceLightSetting {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtAceLightSetting));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
			this.dgvChannel = new System.Windows.Forms.DataGridView();
			this.colCh = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colStyle = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colCmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dgvDimmer = new System.Windows.Forms.DataGridView();
			this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colBrand = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colPort = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.lbDimmerName = new System.Windows.Forms.Label();
			this.lbTValTip = new System.Windows.Forms.Label();
			this.btnLeave = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnEdit = new System.Windows.Forms.Button();
			this.btnExit = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.dgvChannel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvDimmer)).BeginInit();
			this.SuspendLayout();
			// 
			// dgvChannel
			// 
			this.dgvChannel.AllowUserToAddRows = false;
			this.dgvChannel.AllowUserToDeleteRows = false;
			this.dgvChannel.AllowUserToResizeColumns = false;
			this.dgvChannel.AllowUserToResizeRows = false;
			resources.ApplyResources(this.dgvChannel, "dgvChannel");
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvChannel.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvChannel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvChannel.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCh,
            this.colStyle,
            this.colColor,
            this.colValue,
            this.colCmt});
			dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle5.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvChannel.DefaultCellStyle = dataGridViewCellStyle5;
			this.dgvChannel.MultiSelect = false;
			this.dgvChannel.Name = "dgvChannel";
			this.dgvChannel.RowHeadersVisible = false;
			this.dgvChannel.RowTemplate.Height = 24;
			this.dgvChannel.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLight_CellEndEdit);
			// 
			// colCh
			// 
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colCh.DefaultCellStyle = dataGridViewCellStyle2;
			resources.ApplyResources(this.colCh, "colCh");
			this.colCh.Name = "colCh";
			this.colCh.ReadOnly = true;
			this.colCh.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// colStyle
			// 
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colStyle.DefaultCellStyle = dataGridViewCellStyle3;
			resources.ApplyResources(this.colStyle, "colStyle");
			this.colStyle.Name = "colStyle";
			this.colStyle.ReadOnly = true;
			this.colStyle.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.colStyle.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// colColor
			// 
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle4.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colColor.DefaultCellStyle = dataGridViewCellStyle4;
			resources.ApplyResources(this.colColor, "colColor");
			this.colColor.Name = "colColor";
			this.colColor.ReadOnly = true;
			this.colColor.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.colColor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// colValue
			// 
			resources.ApplyResources(this.colValue, "colValue");
			this.colValue.Name = "colValue";
			this.colValue.ReadOnly = true;
			this.colValue.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.colValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
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
			this.dgvDimmer.AllowUserToAddRows = false;
			this.dgvDimmer.AllowUserToDeleteRows = false;
			this.dgvDimmer.AllowUserToResizeColumns = false;
			this.dgvDimmer.AllowUserToResizeRows = false;
			resources.ApplyResources(this.dgvDimmer, "dgvDimmer");
			dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle6.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvDimmer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
			this.dgvDimmer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvDimmer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colBrand,
            this.colPort});
			dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle10.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvDimmer.DefaultCellStyle = dataGridViewCellStyle10;
			this.dgvDimmer.MultiSelect = false;
			this.dgvDimmer.Name = "dgvDimmer";
			this.dgvDimmer.RowHeadersVisible = false;
			this.dgvDimmer.RowTemplate.Height = 24;
			this.dgvDimmer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDimmer_CellDoubleClick);
			// 
			// colName
			// 
			this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle7.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colName.DefaultCellStyle = dataGridViewCellStyle7;
			resources.ApplyResources(this.colName, "colName");
			this.colName.Name = "colName";
			this.colName.ReadOnly = true;
			this.colName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// colBrand
			// 
			dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle8.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colBrand.DefaultCellStyle = dataGridViewCellStyle8;
			resources.ApplyResources(this.colBrand, "colBrand");
			this.colBrand.Name = "colBrand";
			this.colBrand.ReadOnly = true;
			// 
			// colPort
			// 
			dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle9.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colPort.DefaultCellStyle = dataGridViewCellStyle9;
			resources.ApplyResources(this.colPort, "colPort");
			this.colPort.Name = "colPort";
			this.colPort.ReadOnly = true;
			this.colPort.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// lbDimmerName
			// 
			resources.ApplyResources(this.lbDimmerName, "lbDimmerName");
			this.lbDimmerName.Name = "lbDimmerName";
			// 
			// lbTValTip
			// 
			resources.ApplyResources(this.lbTValTip, "lbTValTip");
			this.lbTValTip.ForeColor = System.Drawing.Color.Red;
			this.lbTValTip.Name = "lbTValTip";
			// 
			// btnLeave
			// 
			this.btnLeave.BackgroundImage = global::CtLib.Properties.Resources.Exit;
			resources.ApplyResources(this.btnLeave, "btnLeave");
			this.btnLeave.FlatAppearance.BorderSize = 0;
			this.btnLeave.Name = "btnLeave";
			this.btnLeave.UseVisualStyleBackColor = true;
			this.btnLeave.Click += new System.EventHandler(this.btnLeave_Click);
			// 
			// btnSave
			// 
			this.btnSave.BackgroundImage = global::CtLib.Properties.Resources.Save_2;
			resources.ApplyResources(this.btnSave, "btnSave");
			this.btnSave.FlatAppearance.BorderSize = 0;
			this.btnSave.Name = "btnSave";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnEdit
			// 
			this.btnEdit.BackgroundImage = global::CtLib.Properties.Resources.Edit;
			resources.ApplyResources(this.btnEdit, "btnEdit");
			this.btnEdit.FlatAppearance.BorderSize = 0;
			this.btnEdit.Name = "btnEdit";
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
			this.btnExit.UseVisualStyleBackColor = true;
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// CtAceLightSetting
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ControlBox = false;
			this.Controls.Add(this.lbTValTip);
			this.Controls.Add(this.lbDimmerName);
			this.Controls.Add(this.dgvDimmer);
			this.Controls.Add(this.btnLeave);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.dgvChannel);
			this.Controls.Add(this.btnExit);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CtAceLightSetting";
			this.ShowInTaskbar = false;
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.dgvChannel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvDimmer)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.DataGridView dgvChannel;
		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnLeave;
		private System.Windows.Forms.DataGridView dgvDimmer;
		private System.Windows.Forms.Label lbDimmerName;
		private System.Windows.Forms.Label lbTValTip;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.DataGridViewTextBoxColumn colName;
		private System.Windows.Forms.DataGridViewTextBoxColumn colBrand;
		private System.Windows.Forms.DataGridViewTextBoxColumn colPort;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCh;
		private System.Windows.Forms.DataGridViewTextBoxColumn colStyle;
		private System.Windows.Forms.DataGridViewTextBoxColumn colColor;
		private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCmt;
	}
}