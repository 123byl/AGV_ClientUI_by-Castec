namespace CtLib.Forms {
    partial class CtUserManager {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtUserManager));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAcc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCreator = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lbAccount = new System.Windows.Forms.Label();
            this.txtAccount = new System.Windows.Forms.TextBox();
            this.txtPWD1 = new System.Windows.Forms.TextBox();
            this.lbPWD1 = new System.Windows.Forms.Label();
            this.txtPWD2 = new System.Windows.Forms.TextBox();
            this.lbPWD2 = new System.Windows.Forms.Label();
            this.cbAccLv = new System.Windows.Forms.ComboBox();
            this.lbAccLv = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvData
            // 
            resources.ApplyResources(this.dgvData, "dgvData");
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AllowUserToDeleteRows = false;
            this.dgvData.AllowUserToResizeColumns = false;
            this.dgvData.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colAcc,
            this.colDate,
            this.colCreator});
            this.dgvData.Name = "dgvData";
            this.dgvData.ReadOnly = true;
            this.dgvData.RowHeadersVisible = false;
            this.dgvData.RowTemplate.Height = 24;
            this.dgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvData.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvData_RowEnter);
            // 
            // colName
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colName.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.colName, "colName");
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colAcc
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colAcc.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.colAcc, "colAcc");
            this.colAcc.Name = "colAcc";
            this.colAcc.ReadOnly = true;
            this.colAcc.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colAcc.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colDate
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colDate.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.colDate, "colDate");
            this.colDate.Name = "colDate";
            this.colDate.ReadOnly = true;
            this.colDate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colCreator
            // 
            this.colCreator.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colCreator.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.colCreator, "colCreator");
            this.colCreator.Name = "colCreator";
            this.colCreator.ReadOnly = true;
            this.colCreator.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colCreator.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // lbAccount
            // 
            resources.ApplyResources(this.lbAccount, "lbAccount");
            this.lbAccount.Name = "lbAccount";
            // 
            // txtAccount
            // 
            resources.ApplyResources(this.txtAccount, "txtAccount");
            this.txtAccount.Name = "txtAccount";
            // 
            // txtPWD1
            // 
            resources.ApplyResources(this.txtPWD1, "txtPWD1");
            this.txtPWD1.Name = "txtPWD1";
            this.txtPWD1.UseSystemPasswordChar = true;
            // 
            // lbPWD1
            // 
            resources.ApplyResources(this.lbPWD1, "lbPWD1");
            this.lbPWD1.Name = "lbPWD1";
            // 
            // txtPWD2
            // 
            resources.ApplyResources(this.txtPWD2, "txtPWD2");
            this.txtPWD2.Name = "txtPWD2";
            this.txtPWD2.UseSystemPasswordChar = true;
            // 
            // lbPWD2
            // 
            resources.ApplyResources(this.lbPWD2, "lbPWD2");
            this.lbPWD2.Name = "lbPWD2";
            // 
            // cbAccLv
            // 
            resources.ApplyResources(this.cbAccLv, "cbAccLv");
            this.cbAccLv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAccLv.FormattingEnabled = true;
            this.cbAccLv.Items.AddRange(new object[] {
            resources.GetString("cbAccLv.Items"),
            resources.GetString("cbAccLv.Items1")});
            this.cbAccLv.Name = "cbAccLv";
            // 
            // lbAccLv
            // 
            resources.ApplyResources(this.lbAccLv, "lbAccLv");
            this.lbAccLv.Name = "lbAccLv";
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
            this.btnAdd.Image = global::CtLib.Properties.Resources.UserAdd;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnEdit
            // 
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnEdit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnEdit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
            this.btnEdit.Image = global::CtLib.Properties.Resources.Edit;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnDelete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnDelete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
            this.btnDelete.Image = global::CtLib.Properties.Resources.Stop;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
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
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // CtUserManager
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ControlBox = false;
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lbAccLv);
            this.Controls.Add(this.cbAccLv);
            this.Controls.Add(this.txtPWD2);
            this.Controls.Add(this.lbPWD2);
            this.Controls.Add(this.txtPWD1);
            this.Controls.Add(this.lbPWD1);
            this.Controls.Add(this.txtAccount);
            this.Controls.Add(this.lbAccount);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.dgvData);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CtUserManager";
            this.Load += new System.EventHandler(this.CtUserManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lbAccount;
        private System.Windows.Forms.TextBox txtAccount;
        private System.Windows.Forms.TextBox txtPWD1;
        private System.Windows.Forms.Label lbPWD1;
        private System.Windows.Forms.TextBox txtPWD2;
        private System.Windows.Forms.Label lbPWD2;
        private System.Windows.Forms.ComboBox cbAccLv;
        private System.Windows.Forms.Label lbAccLv;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAcc;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCreator;
    }
}