namespace ClientUI {
    partial class CtGoalSetting {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtGoalSetting));
            this.grbMap = new ClientUI.CtGroupBox();
            this.btnGetMap = new System.Windows.Forms.Button();
            this.btnLoadMap = new System.Windows.Forms.Button();
            this.btnSendMap = new System.Windows.Forms.Button();
            this.btnPath = new System.Windows.Forms.Button();
            this.btnSaveGoal = new System.Windows.Forms.Button();
            this.cmbGoalList = new System.Windows.Forms.ComboBox();
            this.btnRunAll = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.txtAddPtheta = new System.Windows.Forms.TextBox();
            this.txtAddPy = new System.Windows.Forms.TextBox();
            this.txtAddPx = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.dgvGoalPoint = new System.Windows.Forms.DataGridView();
            this.cSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cToward = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnCurrPos = new System.Windows.Forms.Button();
            this.btnGetGoalList = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnCharging = new System.Windows.Forms.Button();
            this.grbMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGoalPoint)).BeginInit();
            this.SuspendLayout();
            // 
            // grbMap
            // 
            this.grbMap.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grbMap.Controls.Add(this.btnGetMap);
            this.grbMap.Controls.Add(this.btnLoadMap);
            this.grbMap.Controls.Add(this.btnSendMap);
            this.grbMap.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.grbMap.Location = new System.Drawing.Point(25, 6);
            this.grbMap.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grbMap.Name = "grbMap";
            this.grbMap.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grbMap.Size = new System.Drawing.Size(323, 140);
            this.grbMap.TabIndex = 63;
            this.grbMap.TabStop = false;
            this.grbMap.Text = "Map";
            // 
            // btnGetMap
            // 
            this.btnGetMap.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetMap.Location = new System.Drawing.Point(115, 34);
            this.btnGetMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetMap.Name = "btnGetMap";
            this.btnGetMap.Size = new System.Drawing.Size(91, 85);
            this.btnGetMap.TabIndex = 62;
            this.btnGetMap.Text = "Get Map";
            this.btnGetMap.UseVisualStyleBackColor = true;
            this.btnGetMap.Click += new System.EventHandler(this.btnGetMap_Click);
            // 
            // btnLoadMap
            // 
            this.btnLoadMap.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnLoadMap.Location = new System.Drawing.Point(17, 34);
            this.btnLoadMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoadMap.Name = "btnLoadMap";
            this.btnLoadMap.Size = new System.Drawing.Size(91, 85);
            this.btnLoadMap.TabIndex = 61;
            this.btnLoadMap.Text = "Load Map";
            this.btnLoadMap.UseVisualStyleBackColor = true;
            this.btnLoadMap.Click += new System.EventHandler(this.btnLoadMap_Click);
            // 
            // btnSendMap
            // 
            this.btnSendMap.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSendMap.Location = new System.Drawing.Point(213, 34);
            this.btnSendMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnSendMap.Name = "btnSendMap";
            this.btnSendMap.Size = new System.Drawing.Size(91, 85);
            this.btnSendMap.TabIndex = 60;
            this.btnSendMap.Text = "Send Map";
            this.btnSendMap.UseVisualStyleBackColor = true;
            this.btnSendMap.Click += new System.EventHandler(this.btnSendMap_Click);
            // 
            // btnPath
            // 
            this.btnPath.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnPath.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPath.Location = new System.Drawing.Point(565, 222);
            this.btnPath.Margin = new System.Windows.Forms.Padding(4);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(160, 62);
            this.btnPath.TabIndex = 59;
            this.btnPath.Text = "Path";
            this.btnPath.UseVisualStyleBackColor = true;
            this.btnPath.Click += new System.EventHandler(this.btnPath_Click);
            // 
            // btnSaveGoal
            // 
            this.btnSaveGoal.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSaveGoal.Location = new System.Drawing.Point(565, 572);
            this.btnSaveGoal.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveGoal.Name = "btnSaveGoal";
            this.btnSaveGoal.Size = new System.Drawing.Size(160, 62);
            this.btnSaveGoal.TabIndex = 58;
            this.btnSaveGoal.Text = "Save Point List";
            this.btnSaveGoal.UseVisualStyleBackColor = true;
            this.btnSaveGoal.Click += new System.EventHandler(this.btnSaveGoal_Click);
            // 
            // cmbGoalList
            // 
            this.cmbGoalList.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cmbGoalList.FormattingEnabled = true;
            this.cmbGoalList.Location = new System.Drawing.Point(565, 181);
            this.cmbGoalList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbGoalList.Name = "cmbGoalList";
            this.cmbGoalList.Size = new System.Drawing.Size(160, 33);
            this.cmbGoalList.TabIndex = 57;
            // 
            // btnRunAll
            // 
            this.btnRunAll.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRunAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRunAll.Location = new System.Drawing.Point(565, 362);
            this.btnRunAll.Margin = new System.Windows.Forms.Padding(4);
            this.btnRunAll.Name = "btnRunAll";
            this.btnRunAll.Size = new System.Drawing.Size(160, 62);
            this.btnRunAll.TabIndex = 56;
            this.btnRunAll.Text = "Run All";
            this.btnRunAll.UseVisualStyleBackColor = true;
            this.btnRunAll.Click += new System.EventHandler(this.btnRunAll_Click);
            // 
            // btnRun
            // 
            this.btnRun.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRun.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRun.Location = new System.Drawing.Point(565, 292);
            this.btnRun.Margin = new System.Windows.Forms.Padding(4);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(160, 62);
            this.btnRun.TabIndex = 55;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnGoGoal_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(655, 40);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(20, 22);
            this.label14.TabIndex = 52;
            this.label14.Text = "θ";
            // 
            // txtAddPtheta
            // 
            this.txtAddPtheta.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtAddPtheta.Location = new System.Drawing.Point(620, 69);
            this.txtAddPtheta.Margin = new System.Windows.Forms.Padding(4);
            this.txtAddPtheta.Name = "txtAddPtheta";
            this.txtAddPtheta.Size = new System.Drawing.Size(95, 34);
            this.txtAddPtheta.TabIndex = 49;
            this.txtAddPtheta.Text = "0";
            this.txtAddPtheta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtAddPy
            // 
            this.txtAddPy.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtAddPy.Location = new System.Drawing.Point(500, 69);
            this.txtAddPy.Margin = new System.Windows.Forms.Padding(4);
            this.txtAddPy.Name = "txtAddPy";
            this.txtAddPy.Size = new System.Drawing.Size(95, 34);
            this.txtAddPy.TabIndex = 50;
            this.txtAddPy.Text = "0";
            this.txtAddPy.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtAddPx
            // 
            this.txtAddPx.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtAddPx.Location = new System.Drawing.Point(383, 69);
            this.txtAddPx.Margin = new System.Windows.Forms.Padding(4);
            this.txtAddPx.Name = "txtAddPx";
            this.txtAddPx.Size = new System.Drawing.Size(95, 34);
            this.txtAddPx.TabIndex = 51;
            this.txtAddPx.Text = "0";
            this.txtAddPx.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(540, 40);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(19, 22);
            this.label15.TabIndex = 53;
            this.label15.Text = "y";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(420, 40);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(19, 22);
            this.label16.TabIndex = 54;
            this.label16.Text = "x";
            // 
            // dgvGoalPoint
            // 
            this.dgvGoalPoint.AllowUserToAddRows = false;
            this.dgvGoalPoint.AllowUserToResizeRows = false;
            this.dgvGoalPoint.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dgvGoalPoint.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGoalPoint.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cSelect,
            this.cID,
            this.cName,
            this.cX,
            this.cY,
            this.cToward});
            this.dgvGoalPoint.Location = new System.Drawing.Point(25, 152);
            this.dgvGoalPoint.Margin = new System.Windows.Forms.Padding(4);
            this.dgvGoalPoint.Name = "dgvGoalPoint";
            this.dgvGoalPoint.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
            this.dgvGoalPoint.RowTemplate.Height = 24;
            this.dgvGoalPoint.Size = new System.Drawing.Size(532, 442);
            this.dgvGoalPoint.TabIndex = 45;
            // 
            // cSelect
            // 
            this.cSelect.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cSelect.HeaderText = "Select";
            this.cSelect.Name = "cSelect";
            this.cSelect.Width = 47;
            // 
            // cID
            // 
            this.cID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cID.HeaderText = "ID";
            this.cID.Name = "cID";
            this.cID.Width = 51;
            // 
            // cName
            // 
            this.cName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cName.HeaderText = "Name";
            this.cName.Name = "cName";
            this.cName.Width = 69;
            // 
            // cX
            // 
            this.cX.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cX.DataPropertyName = "double";
            this.cX.HeaderText = "X";
            this.cX.Name = "cX";
            this.cX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cY
            // 
            this.cY.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cY.DataPropertyName = "double";
            this.cY.HeaderText = "Y";
            this.cY.Name = "cY";
            this.cY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cToward
            // 
            this.cToward.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cToward.DataPropertyName = "double";
            this.cToward.HeaderText = "Toward";
            this.cToward.Name = "cToward";
            this.cToward.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnDeleteAll.Location = new System.Drawing.Point(565, 502);
            this.btnDeleteAll.Margin = new System.Windows.Forms.Padding(4);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(160, 62);
            this.btnDeleteAll.TabIndex = 47;
            this.btnDeleteAll.Text = "Delete All";
            this.btnDeleteAll.UseVisualStyleBackColor = true;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnDelete.Location = new System.Drawing.Point(565, 432);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(4);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(160, 62);
            this.btnDelete.TabIndex = 48;
            this.btnDelete.Text = "Delete Selection";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCurrPos
            // 
            this.btnCurrPos.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCurrPos.Location = new System.Drawing.Point(244, 712);
            this.btnCurrPos.Margin = new System.Windows.Forms.Padding(4);
            this.btnCurrPos.Name = "btnCurrPos";
            this.btnCurrPos.Size = new System.Drawing.Size(145, 62);
            this.btnCurrPos.TabIndex = 46;
            this.btnCurrPos.Text = "Add Current Point";
            this.btnCurrPos.UseVisualStyleBackColor = true;
            this.btnCurrPos.Click += new System.EventHandler(this.btnCurrPos_Click);
            // 
            // btnGetGoalList
            // 
            this.btnGetGoalList.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetGoalList.Location = new System.Drawing.Point(565, 642);
            this.btnGetGoalList.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetGoalList.Name = "btnGetGoalList";
            this.btnGetGoalList.Size = new System.Drawing.Size(160, 62);
            this.btnGetGoalList.TabIndex = 64;
            this.btnGetGoalList.Text = "Get Goal List";
            this.btnGetGoalList.UseVisualStyleBackColor = true;
            this.btnGetGoalList.Click += new System.EventHandler(this.btnGetGoalList_Click);
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnClear.Location = new System.Drawing.Point(565, 712);
            this.btnClear.Margin = new System.Windows.Forms.Padding(4);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(160, 62);
            this.btnClear.TabIndex = 66;
            this.btnClear.Text = "Clear Map";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCharging
            // 
            this.btnCharging.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCharging.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCharging.Location = new System.Drawing.Point(397, 712);
            this.btnCharging.Margin = new System.Windows.Forms.Padding(4);
            this.btnCharging.Name = "btnCharging";
            this.btnCharging.Size = new System.Drawing.Size(160, 62);
            this.btnCharging.TabIndex = 67;
            this.btnCharging.Text = "Charging";
            this.btnCharging.UseVisualStyleBackColor = true;
            this.btnCharging.Click += new System.EventHandler(this.btnCharging_Click);
            // 
            // CtGoalSetting
            // 
            this.AutoHidePortion = 206D;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 776);
            this.Controls.Add(this.btnCharging);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnGetGoalList);
            this.Controls.Add(this.grbMap);
            this.Controls.Add(this.btnPath);
            this.Controls.Add(this.btnSaveGoal);
            this.Controls.Add(this.cmbGoalList);
            this.Controls.Add(this.btnRunAll);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtAddPtheta);
            this.Controls.Add(this.txtAddPy);
            this.Controls.Add(this.txtAddPx);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.dgvGoalPoint);
            this.Controls.Add(this.btnCurrPos);
            this.Controls.Add(this.btnDeleteAll);
            this.Controls.Add(this.btnDelete);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "CtGoalSetting";
            this.Text = "GoalSetting";
            this.grbMap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGoalPoint)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRunAll;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtAddPtheta;
        private System.Windows.Forms.TextBox txtAddPy;
        private System.Windows.Forms.TextBox txtAddPx;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.DataGridView dgvGoalPoint;
        private System.Windows.Forms.Button btnDeleteAll;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ComboBox cmbGoalList;
        private System.Windows.Forms.Button btnSaveGoal;
        private System.Windows.Forms.Button btnPath;
        private CtGroupBox grbMap;
        private System.Windows.Forms.Button btnGetMap;
        private System.Windows.Forms.Button btnLoadMap;
        private System.Windows.Forms.Button btnSendMap;
        private System.Windows.Forms.Button btnCurrPos;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn cID;
        private System.Windows.Forms.DataGridViewTextBoxColumn cName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cX;
        private System.Windows.Forms.DataGridViewTextBoxColumn cY;
        private System.Windows.Forms.DataGridViewTextBoxColumn cToward;
        private System.Windows.Forms.Button btnGetGoalList;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnCharging;
    }
}