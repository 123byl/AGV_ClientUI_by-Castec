namespace CtLib.Module.PCB.ICT {
	/// <summary>適用於 Cadence Allegro 之 In-Circuit Test 檔案(*.val)</summary>
	partial class ICTReader {
		/// <summary>
		/// 設計工具所需的變數。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清除任何使用中的資源。
		/// </summary>
		/// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form 設計工具產生的程式碼

		/// <summary>
		/// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
		/// 這個方法的內容。
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ICTReader));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
			this.txtPartNo = new System.Windows.Forms.TextBox();
			this.btnSearch = new System.Windows.Forms.Button();
			this.dgvSearch = new System.Windows.Forms.DataGridView();
			this.colRef = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colSym = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colPin = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dgvTarget = new System.Windows.Forms.DataGridView();
			this.colTarSym = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colTarPin = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.lbSrcPool = new System.Windows.Forms.Label();
			this.lbTarPool = new System.Windows.Forms.Label();
			this.lbTip = new System.Windows.Forms.Label();
			this.lbCur = new System.Windows.Forms.Label();
			this.lbUnit = new System.Windows.Forms.Label();
			this.lbComma = new System.Windows.Forms.Label();
			this.lbRPattern = new System.Windows.Forms.Label();
			this.lbMilX = new System.Windows.Forms.Label();
			this.lbMmX = new System.Windows.Forms.Label();
			this.lbMilY = new System.Windows.Forms.Label();
			this.lbMmY = new System.Windows.Forms.Label();
			this.btnOpenFile = new System.Windows.Forms.Button();
			this.btnExit = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnRemoveAll = new System.Windows.Forms.Button();
			this.btnAddAll = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dgvSearch)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvTarget)).BeginInit();
			this.SuspendLayout();
			// 
			// txtPartNo
			// 
			resources.ApplyResources(this.txtPartNo, "txtPartNo");
			this.txtPartNo.Name = "txtPartNo";
			// 
			// btnSearch
			// 
			resources.ApplyResources(this.btnSearch, "btnSearch");
			this.btnSearch.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
			this.btnSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.UseVisualStyleBackColor = true;
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// dgvSearch
			// 
			resources.ApplyResources(this.dgvSearch, "dgvSearch");
			this.dgvSearch.AllowUserToAddRows = false;
			this.dgvSearch.AllowUserToDeleteRows = false;
			this.dgvSearch.AllowUserToResizeColumns = false;
			this.dgvSearch.AllowUserToResizeRows = false;
			dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle8.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvSearch.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
			this.dgvSearch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSearch.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRef,
            this.colSym,
            this.colPin});
			this.dgvSearch.Name = "dgvSearch";
			this.dgvSearch.ReadOnly = true;
			this.dgvSearch.RowHeadersVisible = false;
			this.dgvSearch.RowTemplate.Height = 24;
			this.dgvSearch.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			// 
			// colRef
			// 
			dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle9.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colRef.DefaultCellStyle = dataGridViewCellStyle9;
			resources.ApplyResources(this.colRef, "colRef");
			this.colRef.Name = "colRef";
			this.colRef.ReadOnly = true;
			this.colRef.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.colRef.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// colSym
			// 
			this.colSym.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle10.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colSym.DefaultCellStyle = dataGridViewCellStyle10;
			resources.ApplyResources(this.colSym, "colSym");
			this.colSym.Name = "colSym";
			this.colSym.ReadOnly = true;
			this.colSym.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// colPin
			// 
			dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle11.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colPin.DefaultCellStyle = dataGridViewCellStyle11;
			resources.ApplyResources(this.colPin, "colPin");
			this.colPin.Name = "colPin";
			this.colPin.ReadOnly = true;
			// 
			// dgvTarget
			// 
			resources.ApplyResources(this.dgvTarget, "dgvTarget");
			this.dgvTarget.AllowUserToAddRows = false;
			this.dgvTarget.AllowUserToDeleteRows = false;
			this.dgvTarget.AllowUserToResizeColumns = false;
			this.dgvTarget.AllowUserToResizeRows = false;
			dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle12.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvTarget.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle12;
			this.dgvTarget.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvTarget.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTarSym,
            this.colTarPin});
			this.dgvTarget.MultiSelect = false;
			this.dgvTarget.Name = "dgvTarget";
			this.dgvTarget.ReadOnly = true;
			this.dgvTarget.RowHeadersVisible = false;
			this.dgvTarget.RowTemplate.Height = 24;
			this.dgvTarget.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			// 
			// colTarSym
			// 
			this.colTarSym.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle13.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colTarSym.DefaultCellStyle = dataGridViewCellStyle13;
			resources.ApplyResources(this.colTarSym, "colTarSym");
			this.colTarSym.Name = "colTarSym";
			this.colTarSym.ReadOnly = true;
			this.colTarSym.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// colTarPin
			// 
			dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle14.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colTarPin.DefaultCellStyle = dataGridViewCellStyle14;
			resources.ApplyResources(this.colTarPin, "colTarPin");
			this.colTarPin.Name = "colTarPin";
			this.colTarPin.ReadOnly = true;
			// 
			// lbSrcPool
			// 
			resources.ApplyResources(this.lbSrcPool, "lbSrcPool");
			this.lbSrcPool.Name = "lbSrcPool";
			// 
			// lbTarPool
			// 
			resources.ApplyResources(this.lbTarPool, "lbTarPool");
			this.lbTarPool.Name = "lbTarPool";
			// 
			// lbTip
			// 
			resources.ApplyResources(this.lbTip, "lbTip");
			this.lbTip.ForeColor = System.Drawing.Color.Red;
			this.lbTip.Name = "lbTip";
			// 
			// lbCur
			// 
			resources.ApplyResources(this.lbCur, "lbCur");
			this.lbCur.ForeColor = System.Drawing.Color.DarkGray;
			this.lbCur.Name = "lbCur";
			// 
			// lbUnit
			// 
			resources.ApplyResources(this.lbUnit, "lbUnit");
			this.lbUnit.ForeColor = System.Drawing.Color.DarkGray;
			this.lbUnit.Name = "lbUnit";
			// 
			// lbComma
			// 
			resources.ApplyResources(this.lbComma, "lbComma");
			this.lbComma.ForeColor = System.Drawing.Color.DarkGray;
			this.lbComma.Name = "lbComma";
			// 
			// lbRPattern
			// 
			resources.ApplyResources(this.lbRPattern, "lbRPattern");
			this.lbRPattern.ForeColor = System.Drawing.Color.DarkGray;
			this.lbRPattern.Name = "lbRPattern";
			// 
			// lbMilX
			// 
			resources.ApplyResources(this.lbMilX, "lbMilX");
			this.lbMilX.ForeColor = System.Drawing.Color.DarkGray;
			this.lbMilX.Name = "lbMilX";
			// 
			// lbMmX
			// 
			resources.ApplyResources(this.lbMmX, "lbMmX");
			this.lbMmX.ForeColor = System.Drawing.Color.DarkGray;
			this.lbMmX.Name = "lbMmX";
			// 
			// lbMilY
			// 
			resources.ApplyResources(this.lbMilY, "lbMilY");
			this.lbMilY.ForeColor = System.Drawing.Color.DarkGray;
			this.lbMilY.Name = "lbMilY";
			// 
			// lbMmY
			// 
			resources.ApplyResources(this.lbMmY, "lbMmY");
			this.lbMmY.ForeColor = System.Drawing.Color.DarkGray;
			this.lbMmY.Name = "lbMmY";
			// 
			// btnOpenFile
			// 
			resources.ApplyResources(this.btnOpenFile, "btnOpenFile");
			this.btnOpenFile.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
			this.btnOpenFile.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnOpenFile.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnOpenFile.Name = "btnOpenFile";
			this.btnOpenFile.UseVisualStyleBackColor = true;
			this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
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
			// btnSave
			// 
			resources.ApplyResources(this.btnSave, "btnSave");
			this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
			this.btnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnSave.Image = global::CtLib.Properties.Resources.Save_2;
			this.btnSave.Name = "btnSave";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnRemoveAll
			// 
			resources.ApplyResources(this.btnRemoveAll, "btnRemoveAll");
			this.btnRemoveAll.BackgroundImage = global::CtLib.Properties.Resources.Arrow_LeftDouble_4;
			this.btnRemoveAll.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
			this.btnRemoveAll.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnRemoveAll.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnRemoveAll.Name = "btnRemoveAll";
			this.btnRemoveAll.UseVisualStyleBackColor = true;
			this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
			// 
			// btnAddAll
			// 
			resources.ApplyResources(this.btnAddAll, "btnAddAll");
			this.btnAddAll.BackgroundImage = global::CtLib.Properties.Resources.Arrow_RightDouble_4;
			this.btnAddAll.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
			this.btnAddAll.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnAddAll.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnAddAll.Name = "btnAddAll";
			this.btnAddAll.UseVisualStyleBackColor = true;
			this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
			// 
			// btnRemove
			// 
			resources.ApplyResources(this.btnRemove, "btnRemove");
			this.btnRemove.BackgroundImage = global::CtLib.Properties.Resources.Arrow_Left_3;
			this.btnRemove.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
			this.btnRemove.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnRemove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.UseVisualStyleBackColor = true;
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnAdd
			// 
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.BackgroundImage = global::CtLib.Properties.Resources.Arrow_Right_3;
			this.btnAdd.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
			this.btnAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// ICTReader
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.lbMmY);
			this.Controls.Add(this.lbMilY);
			this.Controls.Add(this.lbMmX);
			this.Controls.Add(this.lbMilX);
			this.Controls.Add(this.lbRPattern);
			this.Controls.Add(this.lbComma);
			this.Controls.Add(this.lbUnit);
			this.Controls.Add(this.lbCur);
			this.Controls.Add(this.btnExit);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.lbTip);
			this.Controls.Add(this.lbTarPool);
			this.Controls.Add(this.lbSrcPool);
			this.Controls.Add(this.btnRemoveAll);
			this.Controls.Add(this.btnAddAll);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.dgvTarget);
			this.Controls.Add(this.dgvSearch);
			this.Controls.Add(this.btnSearch);
			this.Controls.Add(this.txtPartNo);
			this.Controls.Add(this.btnOpenFile);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "ICTReader";
			this.TopMost = true;
			this.Shown += new System.EventHandler(this.ICTReader_Shown);
			((System.ComponentModel.ISupportInitialize)(this.dgvSearch)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvTarget)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox txtPartNo;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.DataGridView dgvSearch;
		private System.Windows.Forms.DataGridView dgvTarget;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnAddAll;
		private System.Windows.Forms.Button btnRemoveAll;
		private System.Windows.Forms.Label lbSrcPool;
		private System.Windows.Forms.Label lbTarPool;
		private System.Windows.Forms.Label lbTip;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.Label lbCur;
		private System.Windows.Forms.Label lbUnit;
		private System.Windows.Forms.Label lbComma;
		private System.Windows.Forms.Label lbRPattern;
		private System.Windows.Forms.Label lbMilX;
		private System.Windows.Forms.Label lbMmX;
		private System.Windows.Forms.Label lbMilY;
		private System.Windows.Forms.Label lbMmY;
		private System.Windows.Forms.Button btnOpenFile;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTarSym;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTarPin;
		private System.Windows.Forms.DataGridViewTextBoxColumn colRef;
		private System.Windows.Forms.DataGridViewTextBoxColumn colSym;
		private System.Windows.Forms.DataGridViewTextBoxColumn colPin;
	}
}

