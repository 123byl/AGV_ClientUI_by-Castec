using VehiclePlanner.Partial.VehiclePlannerUI;

namespace VehiclePlanner.Module.Implement {
    partial class BaseGoalSetting {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseGoalSetting));
            this.btnSaveGoal = new System.Windows.Forms.Button();
            this.cmbGoalList = new System.Windows.Forms.ComboBox();
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
            this.ctGroupBox2 = new VehiclePlanner.Partial.VehiclePlannerUI.CtGroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.grbMap = new VehiclePlanner.Partial.VehiclePlannerUI.CtGroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnLoadMap = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnGetMap = new System.Windows.Forms.Button();
            this.btnSendMap = new System.Windows.Forms.Button();
            this.ctGroupBox1 = new VehiclePlanner.Partial.VehiclePlannerUI.CtGroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnPath = new System.Windows.Forms.Button();
            this.btnCharging = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnRunAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGoalPoint)).BeginInit();
            this.ctGroupBox2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.grbMap.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.ctGroupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSaveGoal
            // 
            this.btnSaveGoal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSaveGoal.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSaveGoal.Image = global::VehiclePlanner.Properties.Resources.Save;
            this.btnSaveGoal.Location = new System.Drawing.Point(4, 417);
            this.btnSaveGoal.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveGoal.Name = "btnSaveGoal";
            this.btnSaveGoal.Size = new System.Drawing.Size(170, 71);
            this.btnSaveGoal.TabIndex = 58;
            this.btnSaveGoal.Text = "Save Goal";
            this.btnSaveGoal.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSaveGoal.UseVisualStyleBackColor = true;
            this.btnSaveGoal.Click += new System.EventHandler(this.btnSaveGoal_Click);
            // 
            // cmbGoalList
            // 
            this.cmbGoalList.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cmbGoalList.FormattingEnabled = true;
            this.cmbGoalList.Location = new System.Drawing.Point(565, 115);
            this.cmbGoalList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbGoalList.Name = "cmbGoalList";
            this.cmbGoalList.Size = new System.Drawing.Size(160, 33);
            this.cmbGoalList.TabIndex = 57;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(328, 155);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(20, 22);
            this.label14.TabIndex = 52;
            this.label14.Text = "θ";
            // 
            // txtAddPtheta
            // 
            this.txtAddPtheta.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtAddPtheta.Location = new System.Drawing.Point(293, 184);
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
            this.txtAddPy.Location = new System.Drawing.Point(173, 184);
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
            this.txtAddPx.Location = new System.Drawing.Point(56, 184);
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
            this.label15.Location = new System.Drawing.Point(213, 155);
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
            this.label16.Location = new System.Drawing.Point(93, 155);
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
            this.dgvGoalPoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGoalPoint.Location = new System.Drawing.Point(4, 4);
            this.dgvGoalPoint.Margin = new System.Windows.Forms.Padding(4);
            this.dgvGoalPoint.Name = "dgvGoalPoint";
            this.dgvGoalPoint.RowHeadersVisible = false;
            this.dgvGoalPoint.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
            this.dgvGoalPoint.RowTemplate.Height = 24;
            this.dgvGoalPoint.Size = new System.Drawing.Size(543, 516);
            this.dgvGoalPoint.TabIndex = 45;
            // 
            // cSelect
            // 
            this.cSelect.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cSelect.HeaderText = "Select";
            this.cSelect.Name = "cSelect";
            this.cSelect.Width = 73;
            // 
            // cID
            // 
            this.cID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cID.HeaderText = "ID";
            this.cID.Name = "cID";
            this.cID.Width = 62;
            // 
            // cName
            // 
            this.cName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cName.HeaderText = "Name";
            this.cName.Name = "cName";
            this.cName.Width = 98;
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
            this.btnDeleteAll.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDeleteAll.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnDeleteAll.Image = global::VehiclePlanner.Properties.Resources.Clear;
            this.btnDeleteAll.Location = new System.Drawing.Point(4, 287);
            this.btnDeleteAll.Margin = new System.Windows.Forms.Padding(4);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(170, 71);
            this.btnDeleteAll.TabIndex = 47;
            this.btnDeleteAll.Text = "Delete All";
            this.btnDeleteAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDeleteAll.UseVisualStyleBackColor = true;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDelete.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnDelete.Image = global::VehiclePlanner.Properties.Resources.Delete;
            this.btnDelete.Location = new System.Drawing.Point(4, 158);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(4);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(170, 71);
            this.btnDelete.TabIndex = 48;
            this.btnDelete.Text = "Delete";
            this.btnDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCurrPos
            // 
            this.btnCurrPos.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCurrPos.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCurrPos.Image = global::VehiclePlanner.Properties.Resources.AddCurrent;
            this.btnCurrPos.Location = new System.Drawing.Point(4, 29);
            this.btnCurrPos.Margin = new System.Windows.Forms.Padding(4);
            this.btnCurrPos.Name = "btnCurrPos";
            this.btnCurrPos.Size = new System.Drawing.Size(170, 71);
            this.btnCurrPos.TabIndex = 46;
            this.btnCurrPos.Text = "Add ";
            this.btnCurrPos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCurrPos.UseVisualStyleBackColor = true;
            this.btnCurrPos.Click += new System.EventHandler(this.btnCurrPos_Click);
            // 
            // btnGetGoalList
            // 
            this.btnGetGoalList.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetGoalList.Location = new System.Drawing.Point(695, 726);
            this.btnGetGoalList.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetGoalList.Name = "btnGetGoalList";
            this.btnGetGoalList.Size = new System.Drawing.Size(160, 62);
            this.btnGetGoalList.TabIndex = 64;
            this.btnGetGoalList.Text = "Get Goal List";
            this.btnGetGoalList.UseVisualStyleBackColor = true;
            this.btnGetGoalList.Visible = false;
            this.btnGetGoalList.Click += new System.EventHandler(this.btnGetGoalList_Click);
            // 
            // ctGroupBox2
            // 
            this.ctGroupBox2.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ctGroupBox2.Controls.Add(this.tableLayoutPanel3);
            this.ctGroupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctGroupBox2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ctGroupBox2.Location = new System.Drawing.Point(3, 225);
            this.ctGroupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ctGroupBox2.Name = "ctGroupBox2";
            this.ctGroupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ctGroupBox2.Size = new System.Drawing.Size(741, 555);
            this.ctGroupBox2.TabIndex = 69;
            this.ctGroupBox2.TabStop = false;
            this.ctGroupBox2.Text = "Goal edit";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.Controls.Add(this.dgvGoalPoint, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 29);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(735, 524);
            this.tableLayoutPanel3.TabIndex = 67;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.btnCurrPos, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnDelete, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.btnDeleteAll, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.btnSaveGoal, 0, 3);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(554, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 4;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(178, 518);
            this.tableLayoutPanel4.TabIndex = 70;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.grbMap, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.ctGroupBox1, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.ctGroupBox2, 0, 2);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 3;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 111F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 122F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(747, 782);
            this.tableLayoutPanel5.TabIndex = 68;
            // 
            // grbMap
            // 
            this.grbMap.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grbMap.Controls.Add(this.tableLayoutPanel1);
            this.grbMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grbMap.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.grbMap.Location = new System.Drawing.Point(3, 2);
            this.grbMap.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grbMap.Name = "grbMap";
            this.grbMap.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grbMap.Size = new System.Drawing.Size(741, 107);
            this.grbMap.TabIndex = 64;
            this.grbMap.TabStop = false;
            this.grbMap.Text = "Map";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.btnLoadMap, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnClear, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnGetMap, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSendMap, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 29);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(735, 76);
            this.tableLayoutPanel1.TabIndex = 67;
            // 
            // btnLoadMap
            // 
            this.btnLoadMap.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnLoadMap.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnLoadMap.Image = global::VehiclePlanner.Properties.Resources.Folder_files;
            this.btnLoadMap.Location = new System.Drawing.Point(4, 4);
            this.btnLoadMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoadMap.Name = "btnLoadMap";
            this.btnLoadMap.Size = new System.Drawing.Size(175, 68);
            this.btnLoadMap.TabIndex = 61;
            this.btnLoadMap.Text = "Load Map";
            this.btnLoadMap.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLoadMap.UseVisualStyleBackColor = true;
            this.btnLoadMap.Click += new System.EventHandler(this.btnLoadMap_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnClear.Image = global::VehiclePlanner.Properties.Resources.Eraser_S;
            this.btnClear.Location = new System.Drawing.Point(553, 4);
            this.btnClear.Margin = new System.Windows.Forms.Padding(4);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(178, 68);
            this.btnClear.TabIndex = 66;
            this.btnClear.Text = "Clear Map";
            this.btnClear.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnGetMap
            // 
            this.btnGetMap.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnGetMap.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetMap.Image = global::VehiclePlanner.Properties.Resources.Download;
            this.btnGetMap.Location = new System.Drawing.Point(187, 4);
            this.btnGetMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetMap.Name = "btnGetMap";
            this.btnGetMap.Size = new System.Drawing.Size(175, 68);
            this.btnGetMap.TabIndex = 62;
            this.btnGetMap.Text = "Get Map";
            this.btnGetMap.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGetMap.UseVisualStyleBackColor = true;
            this.btnGetMap.Click += new System.EventHandler(this.btnGetMap_Click);
            // 
            // btnSendMap
            // 
            this.btnSendMap.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSendMap.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSendMap.Image = global::VehiclePlanner.Properties.Resources.Upload;
            this.btnSendMap.Location = new System.Drawing.Point(370, 4);
            this.btnSendMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnSendMap.Name = "btnSendMap";
            this.btnSendMap.Size = new System.Drawing.Size(175, 68);
            this.btnSendMap.TabIndex = 60;
            this.btnSendMap.Text = "Send Map";
            this.btnSendMap.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSendMap.UseVisualStyleBackColor = true;
            this.btnSendMap.Click += new System.EventHandler(this.btnSendMap_Click);
            // 
            // ctGroupBox1
            // 
            this.ctGroupBox1.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ctGroupBox1.Controls.Add(this.tableLayoutPanel2);
            this.ctGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctGroupBox1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ctGroupBox1.Location = new System.Drawing.Point(3, 113);
            this.ctGroupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ctGroupBox1.Name = "ctGroupBox1";
            this.ctGroupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ctGroupBox1.Size = new System.Drawing.Size(741, 108);
            this.ctGroupBox1.TabIndex = 69;
            this.ctGroupBox1.TabStop = false;
            this.ctGroupBox1.Text = "Action";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Controls.Add(this.btnPath, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnCharging, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnRun, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnRunAll, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 29);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(735, 77);
            this.tableLayoutPanel2.TabIndex = 67;
            // 
            // btnPath
            // 
            this.btnPath.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPath.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnPath.Image = global::VehiclePlanner.Properties.Resources.Path;
            this.btnPath.Location = new System.Drawing.Point(4, 4);
            this.btnPath.Margin = new System.Windows.Forms.Padding(4);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(175, 69);
            this.btnPath.TabIndex = 59;
            this.btnPath.Text = "Path";
            this.btnPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPath.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPath.UseVisualStyleBackColor = true;
            this.btnPath.Click += new System.EventHandler(this.btnPath_Click);
            // 
            // btnCharging
            // 
            this.btnCharging.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCharging.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCharging.Image = global::VehiclePlanner.Properties.Resources.Warning;
            this.btnCharging.Location = new System.Drawing.Point(553, 4);
            this.btnCharging.Margin = new System.Windows.Forms.Padding(4);
            this.btnCharging.Name = "btnCharging";
            this.btnCharging.Size = new System.Drawing.Size(178, 69);
            this.btnCharging.TabIndex = 67;
            this.btnCharging.Text = "Charging";
            this.btnCharging.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCharging.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCharging.UseVisualStyleBackColor = true;
            this.btnCharging.Click += new System.EventHandler(this.btnCharging_Click);
            // 
            // btnRun
            // 
            this.btnRun.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRun.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRun.Image = global::VehiclePlanner.Properties.Resources.play;
            this.btnRun.Location = new System.Drawing.Point(187, 4);
            this.btnRun.Margin = new System.Windows.Forms.Padding(4);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(175, 69);
            this.btnRun.TabIndex = 55;
            this.btnRun.Text = "Run";
            this.btnRun.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRun.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnGoGoal_Click);
            // 
            // btnRunAll
            // 
            this.btnRunAll.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRunAll.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRunAll.Image = global::VehiclePlanner.Properties.Resources.Refresh_2;
            this.btnRunAll.Location = new System.Drawing.Point(370, 4);
            this.btnRunAll.Margin = new System.Windows.Forms.Padding(4);
            this.btnRunAll.Name = "btnRunAll";
            this.btnRunAll.Size = new System.Drawing.Size(175, 69);
            this.btnRunAll.TabIndex = 56;
            this.btnRunAll.Text = "Run All";
            this.btnRunAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRunAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRunAll.UseVisualStyleBackColor = true;
            this.btnRunAll.Click += new System.EventHandler(this.btnRunAll_Click);
            // 
            // CtGoalSetting
            // 
            this.AutoHidePortion = 206D;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 776);
            this.Controls.Add(this.tableLayoutPanel5);
            this.Controls.Add(this.btnGetGoalList);
            this.Controls.Add(this.cmbGoalList);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtAddPtheta);
            this.Controls.Add(this.txtAddPy);
            this.Controls.Add(this.txtAddPx);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label16);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "CtGoalSetting";
            this.Text = "GoalSetting";
            ((System.ComponentModel.ISupportInitialize)(this.dgvGoalPoint)).EndInit();
            this.ctGroupBox2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.grbMap.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ctGroupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtAddPtheta;
        protected System.Windows.Forms.TextBox txtAddPy;
        protected System.Windows.Forms.TextBox txtAddPx;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        protected System.Windows.Forms.DataGridView dgvGoalPoint;
        private System.Windows.Forms.Button btnDeleteAll;
        private System.Windows.Forms.Button btnDelete;
        protected System.Windows.Forms.ComboBox cmbGoalList;
        private System.Windows.Forms.Button btnSaveGoal;
        private System.Windows.Forms.Button btnCurrPos;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn cID;
        private System.Windows.Forms.DataGridViewTextBoxColumn cName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cX;
        private System.Windows.Forms.DataGridViewTextBoxColumn cY;
        private System.Windows.Forms.DataGridViewTextBoxColumn cToward;
        private System.Windows.Forms.Button btnGetGoalList;
        private CtGroupBox ctGroupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private CtGroupBox grbMap;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnLoadMap;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnGetMap;
        private System.Windows.Forms.Button btnSendMap;
        private CtGroupBox ctGroupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.Button btnCharging;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnRunAll;
    }
}