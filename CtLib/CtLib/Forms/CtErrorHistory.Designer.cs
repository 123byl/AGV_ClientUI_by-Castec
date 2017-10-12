namespace CtLib.Forms {
    partial class CtErrorHistory {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtErrorHistory));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			this.tpickStart = new System.Windows.Forms.DateTimePicker();
			this.tpickEnd = new System.Windows.Forms.DateTimePicker();
			this.gbSelect = new System.Windows.Forms.GroupBox();
			this.lbTo = new System.Windows.Forms.Label();
			this.rdbTime_Custom = new System.Windows.Forms.RadioButton();
			this.rdbTime_Month = new System.Windows.Forms.RadioButton();
			this.rdbTime_Week = new System.Windows.Forms.RadioButton();
			this.rdbTime_Today = new System.Windows.Forms.RadioButton();
			this.dgvMsg = new System.Windows.Forms.DataGridView();
			this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colKind = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colTit = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colMsg = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.gbType = new System.Windows.Forms.GroupBox();
			this.cbLv = new System.Windows.Forms.ComboBox();
			this.lbLv = new System.Windows.Forms.Label();
			this.cbType = new System.Windows.Forms.ComboBox();
			this.lbType = new System.Windows.Forms.Label();
			this.btnExit = new System.Windows.Forms.Button();
			this.btnSearch = new System.Windows.Forms.Button();
			this.gbSelect.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvMsg)).BeginInit();
			this.gbType.SuspendLayout();
			this.SuspendLayout();
			// 
			// tpickStart
			// 
			resources.ApplyResources(this.tpickStart, "tpickStart");
			this.tpickStart.Name = "tpickStart";
			this.tpickStart.Value = new System.DateTime(2014, 9, 27, 0, 0, 0, 0);
			// 
			// tpickEnd
			// 
			resources.ApplyResources(this.tpickEnd, "tpickEnd");
			this.tpickEnd.Name = "tpickEnd";
			this.tpickEnd.Value = new System.DateTime(2014, 9, 27, 0, 0, 0, 0);
			// 
			// gbSelect
			// 
			resources.ApplyResources(this.gbSelect, "gbSelect");
			this.gbSelect.Controls.Add(this.lbTo);
			this.gbSelect.Controls.Add(this.rdbTime_Custom);
			this.gbSelect.Controls.Add(this.rdbTime_Month);
			this.gbSelect.Controls.Add(this.rdbTime_Week);
			this.gbSelect.Controls.Add(this.rdbTime_Today);
			this.gbSelect.Controls.Add(this.tpickStart);
			this.gbSelect.Controls.Add(this.tpickEnd);
			this.gbSelect.Name = "gbSelect";
			this.gbSelect.TabStop = false;
			// 
			// lbTo
			// 
			resources.ApplyResources(this.lbTo, "lbTo");
			this.lbTo.Name = "lbTo";
			// 
			// rdbTime_Custom
			// 
			resources.ApplyResources(this.rdbTime_Custom, "rdbTime_Custom");
			this.rdbTime_Custom.Name = "rdbTime_Custom";
			this.rdbTime_Custom.UseVisualStyleBackColor = true;
			this.rdbTime_Custom.CheckedChanged += new System.EventHandler(this.rdbTime_Custom_CheckedChanged);
			// 
			// rdbTime_Month
			// 
			resources.ApplyResources(this.rdbTime_Month, "rdbTime_Month");
			this.rdbTime_Month.Name = "rdbTime_Month";
			this.rdbTime_Month.UseVisualStyleBackColor = true;
			this.rdbTime_Month.CheckedChanged += new System.EventHandler(this.rdbTime_Month_CheckedChanged);
			// 
			// rdbTime_Week
			// 
			resources.ApplyResources(this.rdbTime_Week, "rdbTime_Week");
			this.rdbTime_Week.Name = "rdbTime_Week";
			this.rdbTime_Week.UseVisualStyleBackColor = true;
			this.rdbTime_Week.CheckedChanged += new System.EventHandler(this.rdbTime_Week_CheckedChanged);
			// 
			// rdbTime_Today
			// 
			resources.ApplyResources(this.rdbTime_Today, "rdbTime_Today");
			this.rdbTime_Today.Checked = true;
			this.rdbTime_Today.Name = "rdbTime_Today";
			this.rdbTime_Today.TabStop = true;
			this.rdbTime_Today.UseVisualStyleBackColor = true;
			this.rdbTime_Today.CheckedChanged += new System.EventHandler(this.rdbTime_Today_CheckedChanged);
			// 
			// dgvMsg
			// 
			resources.ApplyResources(this.dgvMsg, "dgvMsg");
			this.dgvMsg.AllowUserToAddRows = false;
			this.dgvMsg.AllowUserToDeleteRows = false;
			this.dgvMsg.AllowUserToResizeColumns = false;
			this.dgvMsg.AllowUserToResizeRows = false;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvMsg.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvMsg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvMsg.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTime,
            this.colKind,
            this.colType,
            this.colTit,
            this.colMsg});
			this.dgvMsg.Name = "dgvMsg";
			this.dgvMsg.ReadOnly = true;
			this.dgvMsg.RowHeadersVisible = false;
			this.dgvMsg.RowTemplate.Height = 24;
			this.dgvMsg.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			// 
			// colTime
			// 
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colTime.DefaultCellStyle = dataGridViewCellStyle2;
			resources.ApplyResources(this.colTime, "colTime");
			this.colTime.Name = "colTime";
			this.colTime.ReadOnly = true;
			this.colTime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.colTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// colKind
			// 
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colKind.DefaultCellStyle = dataGridViewCellStyle3;
			resources.ApplyResources(this.colKind, "colKind");
			this.colKind.Name = "colKind";
			this.colKind.ReadOnly = true;
			this.colKind.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.colKind.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// colType
			// 
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle4.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colType.DefaultCellStyle = dataGridViewCellStyle4;
			resources.ApplyResources(this.colType, "colType");
			this.colType.Name = "colType";
			this.colType.ReadOnly = true;
			this.colType.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.colType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// colTit
			// 
			dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle5.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colTit.DefaultCellStyle = dataGridViewCellStyle5;
			resources.ApplyResources(this.colTit, "colTit");
			this.colTit.Name = "colTit";
			this.colTit.ReadOnly = true;
			this.colTit.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.colTit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// colMsg
			// 
			this.colMsg.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle6.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.colMsg.DefaultCellStyle = dataGridViewCellStyle6;
			resources.ApplyResources(this.colMsg, "colMsg");
			this.colMsg.Name = "colMsg";
			this.colMsg.ReadOnly = true;
			this.colMsg.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.colMsg.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// gbType
			// 
			resources.ApplyResources(this.gbType, "gbType");
			this.gbType.Controls.Add(this.cbLv);
			this.gbType.Controls.Add(this.lbLv);
			this.gbType.Controls.Add(this.cbType);
			this.gbType.Controls.Add(this.lbType);
			this.gbType.Name = "gbType";
			this.gbType.TabStop = false;
			// 
			// cbLv
			// 
			resources.ApplyResources(this.cbLv, "cbLv");
			this.cbLv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbLv.FormattingEnabled = true;
			this.cbLv.Items.AddRange(new object[] {
            resources.GetString("cbLv.Items"),
            resources.GetString("cbLv.Items1"),
            resources.GetString("cbLv.Items2"),
            resources.GetString("cbLv.Items3"),
            resources.GetString("cbLv.Items4")});
			this.cbLv.Name = "cbLv";
			// 
			// lbLv
			// 
			resources.ApplyResources(this.lbLv, "lbLv");
			this.lbLv.Name = "lbLv";
			// 
			// cbType
			// 
			resources.ApplyResources(this.cbType, "cbType");
			this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbType.FormattingEnabled = true;
			this.cbType.Items.AddRange(new object[] {
            resources.GetString("cbType.Items"),
            resources.GetString("cbType.Items1"),
            resources.GetString("cbType.Items2"),
            resources.GetString("cbType.Items3")});
			this.cbType.Name = "cbType";
			this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
			// 
			// lbType
			// 
			resources.ApplyResources(this.lbType, "lbType");
			this.lbType.Name = "lbType";
			// 
			// btnExit
			// 
			resources.ApplyResources(this.btnExit, "btnExit");
			this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnExit.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
			this.btnExit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnExit.Image = global::CtLib.Properties.Resources.Exit;
			this.btnExit.Name = "btnExit";
			this.btnExit.UseVisualStyleBackColor = true;
			// 
			// btnSearch
			// 
			resources.ApplyResources(this.btnSearch, "btnSearch");
			this.btnSearch.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
			this.btnSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnSearch.Image = global::CtLib.Properties.Resources.Arrow_Down;
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.UseVisualStyleBackColor = true;
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// CtErrorHistory
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ControlBox = false;
			this.Controls.Add(this.btnExit);
			this.Controls.Add(this.btnSearch);
			this.Controls.Add(this.gbType);
			this.Controls.Add(this.dgvMsg);
			this.Controls.Add(this.gbSelect);
			this.Name = "CtErrorHistory";
			this.gbSelect.ResumeLayout(false);
			this.gbSelect.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvMsg)).EndInit();
			this.gbType.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker tpickStart;
        private System.Windows.Forms.DateTimePicker tpickEnd;
        private System.Windows.Forms.GroupBox gbSelect;
        private System.Windows.Forms.Label lbTo;
        private System.Windows.Forms.RadioButton rdbTime_Custom;
        private System.Windows.Forms.RadioButton rdbTime_Month;
        private System.Windows.Forms.RadioButton rdbTime_Week;
        private System.Windows.Forms.RadioButton rdbTime_Today;
        private System.Windows.Forms.DataGridView dgvMsg;
        private System.Windows.Forms.GroupBox gbType;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.Label lbType;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.ComboBox cbLv;
        private System.Windows.Forms.Label lbLv;
		private System.Windows.Forms.DataGridViewTextBoxColumn colMsg;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTit;
		private System.Windows.Forms.DataGridViewTextBoxColumn colType;
		private System.Windows.Forms.DataGridViewTextBoxColumn colKind;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
	}
}