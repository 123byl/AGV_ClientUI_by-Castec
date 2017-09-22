namespace CtLib.Forms {
    partial class CtRecipe {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtRecipe));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.listFile = new System.Windows.Forms.ListBox();
            this.lbID = new System.Windows.Forms.Label();
            this.lbComment = new System.Windows.Forms.Label();
            this.txtID = new System.Windows.Forms.TextBox();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.dgvRecipe = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colComment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRenew = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecipe)).BeginInit();
            this.SuspendLayout();
            // 
            // listFile
            // 
            resources.ApplyResources(this.listFile, "listFile");
            this.listFile.FormattingEnabled = true;
            this.listFile.Name = "listFile";
            this.listFile.Sorted = true;
            this.listFile.DoubleClick += new System.EventHandler(this.listFile_DoubleClick);
            // 
            // lbID
            // 
            resources.ApplyResources(this.lbID, "lbID");
            this.lbID.Name = "lbID";
            // 
            // lbComment
            // 
            resources.ApplyResources(this.lbComment, "lbComment");
            this.lbComment.Name = "lbComment";
            // 
            // txtID
            // 
            resources.ApplyResources(this.txtID, "txtID");
            this.txtID.Name = "txtID";
            // 
            // txtComment
            // 
            resources.ApplyResources(this.txtComment, "txtComment");
            this.txtComment.Name = "txtComment";
            // 
            // dgvRecipe
            // 
            this.dgvRecipe.AllowUserToAddRows = false;
            this.dgvRecipe.AllowUserToDeleteRows = false;
            this.dgvRecipe.AllowUserToResizeColumns = false;
            this.dgvRecipe.AllowUserToResizeRows = false;
            resources.ApplyResources(this.dgvRecipe, "dgvRecipe");
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRecipe.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvRecipe.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecipe.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colValue,
            this.colComment});
            this.dgvRecipe.Name = "dgvRecipe";
            this.dgvRecipe.RowHeadersVisible = false;
            this.dgvRecipe.RowTemplate.Height = 24;
            this.dgvRecipe.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvRecipe_CellBeginEdit);
            // 
            // colName
            // 
            resources.ApplyResources(this.colName, "colName");
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // colValue
            // 
            resources.ApplyResources(this.colValue, "colValue");
            this.colValue.Name = "colValue";
            this.colValue.ReadOnly = true;
            this.colValue.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // colComment
            // 
            this.colComment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colComment, "colComment");
            this.colComment.Name = "colComment";
            this.colComment.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // btnDownload
            // 
            resources.ApplyResources(this.btnDownload, "btnDownload");
            this.btnDownload.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnDownload.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnDownload.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
            this.btnDownload.Image = global::CtLib.Properties.Resources.Arrow_Down;
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnExit
            // 
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
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
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
            this.btnSave.Image = global::CtLib.Properties.Resources.Save_2;
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnDelete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnDelete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Image = global::CtLib.Properties.Resources.Delete;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnRenew
            // 
            this.btnRenew.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnRenew.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnRenew.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
            resources.ApplyResources(this.btnRenew, "btnRenew");
            this.btnRenew.Image = global::CtLib.Properties.Resources.Refresh_2;
            this.btnRenew.Name = "btnRenew";
            this.btnRenew.UseVisualStyleBackColor = true;
            this.btnRenew.Click += new System.EventHandler(this.btnRenew_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnCreate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnCreate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
            resources.ApplyResources(this.btnCreate, "btnCreate");
            this.btnCreate.Image = global::CtLib.Properties.Resources.Add_2;
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // CtRecipe
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvRecipe);
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.txtID);
            this.Controls.Add(this.lbComment);
            this.Controls.Add(this.lbID);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnRenew);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.listFile);
            this.Name = "CtRecipe";
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecipe)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listFile;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnRenew;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lbID;
        private System.Windows.Forms.Label lbComment;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.DataGridView dgvRecipe;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colComment;
    }
}