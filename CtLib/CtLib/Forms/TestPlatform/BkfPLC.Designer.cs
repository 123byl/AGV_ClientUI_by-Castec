namespace CtLib.Forms.TestPlatform {
    partial class Test_BkfPLC {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if ( disposing && ( components != null ) ) {
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			this.lbNetID = new System.Windows.Forms.Label();
			this.lbPort = new System.Windows.Forms.Label();
			this.btnConnect = new System.Windows.Forms.Button();
			this.dgvInfo = new System.Windows.Forms.DataGridView();
			this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colInfo = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.gbVariable = new System.Windows.Forms.GroupBox();
			this.btnVarAdMoni = new System.Windows.Forms.Button();
			this.btnVarWrite = new System.Windows.Forms.Button();
			this.btnVarRead = new System.Windows.Forms.Button();
			this.txtVarVal = new System.Windows.Forms.TextBox();
			this.lbVarVal = new System.Windows.Forms.Label();
			this.txtVarName = new System.Windows.Forms.TextBox();
			this.lbVarName = new System.Windows.Forms.Label();
			this.gbState = new System.Windows.Forms.GroupBox();
			this.btnSttSet = new System.Windows.Forms.Button();
			this.cbSttList = new System.Windows.Forms.ComboBox();
			this.txtStt = new System.Windows.Forms.TextBox();
			this.lbStt = new System.Windows.Forms.Label();
			this.dgvData = new System.Windows.Forms.DataGridView();
			this.colData = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.txtPort = new System.Windows.Forms.TextBox();
			this.txtNetID = new System.Windows.Forms.TextBox();
			this.gbAry = new System.Windows.Forms.GroupBox();
			this.btnAryWrite = new System.Windows.Forms.Button();
			this.btnAryRead = new System.Windows.Forms.Button();
			this.txtAryName = new System.Windows.Forms.TextBox();
			this.lbAryName = new System.Windows.Forms.Label();
			this.btnSttGet = new System.Windows.Forms.Button();
			this.lbAryOfs = new System.Windows.Forms.Label();
			this.nudAryOfs = new System.Windows.Forms.NumericUpDown();
			this.gbNotify = new System.Windows.Forms.GroupBox();
			this.dgvNotf = new System.Windows.Forms.DataGridView();
			this.colNotfName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colNotfVal = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.btnNotfAdd = new System.Windows.Forms.Button();
			this.btnNotfDel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dgvInfo)).BeginInit();
			this.gbVariable.SuspendLayout();
			this.gbState.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
			this.gbAry.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudAryOfs)).BeginInit();
			this.gbNotify.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvNotf)).BeginInit();
			this.SuspendLayout();
			// 
			// lbNetID
			// 
			this.lbNetID.AutoSize = true;
			this.lbNetID.Location = new System.Drawing.Point(12, 36);
			this.lbNetID.Name = "lbNetID";
			this.lbNetID.Size = new System.Drawing.Size(33, 12);
			this.lbNetID.TabIndex = 0;
			this.lbNetID.Text = "NetID";
			// 
			// lbPort
			// 
			this.lbPort.AutoSize = true;
			this.lbPort.Location = new System.Drawing.Point(12, 65);
			this.lbPort.Name = "lbPort";
			this.lbPort.Size = new System.Drawing.Size(24, 12);
			this.lbPort.TabIndex = 2;
			this.lbPort.Text = "Port";
			// 
			// btnConnect
			// 
			this.btnConnect.Location = new System.Drawing.Point(124, 60);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(75, 23);
			this.btnConnect.TabIndex = 4;
			this.btnConnect.Text = "連線";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// dgvInfo
			// 
			this.dgvInfo.AllowUserToAddRows = false;
			this.dgvInfo.AllowUserToDeleteRows = false;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvInfo.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTime,
            this.colInfo});
			this.dgvInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.dgvInfo.Location = new System.Drawing.Point(0, 409);
			this.dgvInfo.Name = "dgvInfo";
			this.dgvInfo.ReadOnly = true;
			this.dgvInfo.RowHeadersVisible = false;
			this.dgvInfo.RowTemplate.Height = 24;
			this.dgvInfo.Size = new System.Drawing.Size(911, 107);
			this.dgvInfo.TabIndex = 5;
			// 
			// colTime
			// 
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.colTime.DefaultCellStyle = dataGridViewCellStyle2;
			this.colTime.HeaderText = "時間";
			this.colTime.Name = "colTime";
			this.colTime.ReadOnly = true;
			this.colTime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.colTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.colTime.ToolTipText = "訊息時間";
			// 
			// colInfo
			// 
			this.colInfo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			this.colInfo.DefaultCellStyle = dataGridViewCellStyle3;
			this.colInfo.HeaderText = "訊息";
			this.colInfo.Name = "colInfo";
			this.colInfo.ReadOnly = true;
			this.colInfo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.colInfo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// gbVariable
			// 
			this.gbVariable.Controls.Add(this.btnVarAdMoni);
			this.gbVariable.Controls.Add(this.btnVarWrite);
			this.gbVariable.Controls.Add(this.btnVarRead);
			this.gbVariable.Controls.Add(this.txtVarVal);
			this.gbVariable.Controls.Add(this.lbVarVal);
			this.gbVariable.Controls.Add(this.txtVarName);
			this.gbVariable.Controls.Add(this.lbVarName);
			this.gbVariable.Location = new System.Drawing.Point(14, 248);
			this.gbVariable.Name = "gbVariable";
			this.gbVariable.Size = new System.Drawing.Size(266, 151);
			this.gbVariable.TabIndex = 6;
			this.gbVariable.TabStop = false;
			this.gbVariable.Text = "非陣列變數控制";
			// 
			// btnVarAdMoni
			// 
			this.btnVarAdMoni.Location = new System.Drawing.Point(172, 30);
			this.btnVarAdMoni.Name = "btnVarAdMoni";
			this.btnVarAdMoni.Size = new System.Drawing.Size(75, 23);
			this.btnVarAdMoni.TabIndex = 7;
			this.btnVarAdMoni.Text = "加入監控";
			this.btnVarAdMoni.UseVisualStyleBackColor = true;
			this.btnVarAdMoni.Click += new System.EventHandler(this.btnVarAdMoni_Click);
			// 
			// btnVarWrite
			// 
			this.btnVarWrite.Location = new System.Drawing.Point(172, 110);
			this.btnVarWrite.Name = "btnVarWrite";
			this.btnVarWrite.Size = new System.Drawing.Size(75, 23);
			this.btnVarWrite.TabIndex = 6;
			this.btnVarWrite.Text = "寫入";
			this.btnVarWrite.UseVisualStyleBackColor = true;
			this.btnVarWrite.Click += new System.EventHandler(this.btnVarWrite_Click);
			// 
			// btnVarRead
			// 
			this.btnVarRead.Location = new System.Drawing.Point(172, 56);
			this.btnVarRead.Name = "btnVarRead";
			this.btnVarRead.Size = new System.Drawing.Size(75, 23);
			this.btnVarRead.TabIndex = 5;
			this.btnVarRead.Text = "讀取";
			this.btnVarRead.UseVisualStyleBackColor = true;
			this.btnVarRead.Click += new System.EventHandler(this.btnVarRead_Click);
			// 
			// txtVarVal
			// 
			this.txtVarVal.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtVarVal.Location = new System.Drawing.Point(12, 110);
			this.txtVarVal.Name = "txtVarVal";
			this.txtVarVal.Size = new System.Drawing.Size(154, 23);
			this.txtVarVal.TabIndex = 3;
			this.txtVarVal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbVarVal
			// 
			this.lbVarVal.AutoSize = true;
			this.lbVarVal.Location = new System.Drawing.Point(10, 95);
			this.lbVarVal.Name = "lbVarVal";
			this.lbVarVal.Size = new System.Drawing.Size(32, 12);
			this.lbVarVal.TabIndex = 2;
			this.lbVarVal.Text = "數值:";
			// 
			// txtVarName
			// 
			this.txtVarName.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtVarName.Location = new System.Drawing.Point(12, 56);
			this.txtVarName.Name = "txtVarName";
			this.txtVarName.Size = new System.Drawing.Size(154, 23);
			this.txtVarName.TabIndex = 1;
			this.txtVarName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.txtVarName.TextChanged += new System.EventHandler(this.txtVarName_TextChanged);
			// 
			// lbVarName
			// 
			this.lbVarName.AutoSize = true;
			this.lbVarName.Location = new System.Drawing.Point(10, 41);
			this.lbVarName.Name = "lbVarName";
			this.lbVarName.Size = new System.Drawing.Size(56, 12);
			this.lbVarName.TabIndex = 0;
			this.lbVarName.Text = "變數名稱:";
			// 
			// gbState
			// 
			this.gbState.Controls.Add(this.btnSttGet);
			this.gbState.Controls.Add(this.btnSttSet);
			this.gbState.Controls.Add(this.cbSttList);
			this.gbState.Controls.Add(this.txtStt);
			this.gbState.Controls.Add(this.lbStt);
			this.gbState.Location = new System.Drawing.Point(14, 107);
			this.gbState.Name = "gbState";
			this.gbState.Size = new System.Drawing.Size(266, 135);
			this.gbState.TabIndex = 7;
			this.gbState.TabStop = false;
			this.gbState.Text = "PLC 狀態";
			// 
			// btnSttSet
			// 
			this.btnSttSet.Location = new System.Drawing.Point(172, 75);
			this.btnSttSet.Name = "btnSttSet";
			this.btnSttSet.Size = new System.Drawing.Size(75, 23);
			this.btnSttSet.TabIndex = 6;
			this.btnSttSet.Text = "設定";
			this.btnSttSet.UseVisualStyleBackColor = true;
			this.btnSttSet.Click += new System.EventHandler(this.btnSttSet_Click);
			// 
			// cbSttList
			// 
			this.cbSttList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSttList.FormattingEnabled = true;
			this.cbSttList.Items.AddRange(new object[] {
            "Invalid",
            "IDLE",
            "RESET",
            "INIT",
            "START",
            "RUN",
            "STOP",
            "SAVECFG",
            "LOADCFG",
            "POWERFAILURE",
            "POWERGOOD",
            "ERROR",
            "SHUTDOWN",
            "SUSPEND",
            "RESUME",
            "CONFIG",
            "RECONFIG",
            "MAXSTATES"});
			this.cbSttList.Location = new System.Drawing.Point(12, 78);
			this.cbSttList.Name = "cbSttList";
			this.cbSttList.Size = new System.Drawing.Size(154, 20);
			this.cbSttList.TabIndex = 4;
			// 
			// txtStt
			// 
			this.txtStt.Enabled = false;
			this.txtStt.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtStt.Location = new System.Drawing.Point(72, 38);
			this.txtStt.Name = "txtStt";
			this.txtStt.ReadOnly = true;
			this.txtStt.Size = new System.Drawing.Size(94, 23);
			this.txtStt.TabIndex = 3;
			this.txtStt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbStt
			// 
			this.lbStt.AutoSize = true;
			this.lbStt.Location = new System.Drawing.Point(10, 43);
			this.lbStt.Name = "lbStt";
			this.lbStt.Size = new System.Drawing.Size(56, 12);
			this.lbStt.TabIndex = 2;
			this.lbStt.Text = "當前狀態:";
			// 
			// dgvData
			// 
			this.dgvData.AllowUserToAddRows = false;
			this.dgvData.AllowUserToDeleteRows = false;
			this.dgvData.AllowUserToResizeRows = false;
			this.dgvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle4.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
			this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colData});
			this.dgvData.Location = new System.Drawing.Point(6, 69);
			this.dgvData.Name = "dgvData";
			this.dgvData.RowHeadersVisible = false;
			this.dgvData.RowTemplate.Height = 24;
			this.dgvData.Size = new System.Drawing.Size(322, 312);
			this.dgvData.TabIndex = 8;
			// 
			// colData
			// 
			this.colData.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colData.HeaderText = "資料";
			this.colData.Name = "colData";
			this.colData.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.colData.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// txtPort
			// 
			this.txtPort.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtPort.Location = new System.Drawing.Point(51, 60);
			this.txtPort.Name = "txtPort";
			this.txtPort.Size = new System.Drawing.Size(65, 23);
			this.txtPort.TabIndex = 3;
			this.txtPort.Text = "801";
			this.txtPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtNetID
			// 
			this.txtNetID.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtNetID.Location = new System.Drawing.Point(51, 31);
			this.txtNetID.Name = "txtNetID";
			this.txtNetID.Size = new System.Drawing.Size(148, 23);
			this.txtNetID.TabIndex = 1;
			this.txtNetID.Text = "5.8.174.117.1.1";
			this.txtNetID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// gbAry
			// 
			this.gbAry.Controls.Add(this.nudAryOfs);
			this.gbAry.Controls.Add(this.lbAryOfs);
			this.gbAry.Controls.Add(this.btnAryWrite);
			this.gbAry.Controls.Add(this.btnAryRead);
			this.gbAry.Controls.Add(this.txtAryName);
			this.gbAry.Controls.Add(this.lbAryName);
			this.gbAry.Controls.Add(this.dgvData);
			this.gbAry.Location = new System.Drawing.Point(286, 12);
			this.gbAry.Name = "gbAry";
			this.gbAry.Size = new System.Drawing.Size(334, 387);
			this.gbAry.TabIndex = 9;
			this.gbAry.TabStop = false;
			this.gbAry.Text = "陣列變數讀寫";
			// 
			// btnAryWrite
			// 
			this.btnAryWrite.Location = new System.Drawing.Point(253, 14);
			this.btnAryWrite.Name = "btnAryWrite";
			this.btnAryWrite.Size = new System.Drawing.Size(75, 23);
			this.btnAryWrite.TabIndex = 12;
			this.btnAryWrite.Text = "寫入";
			this.btnAryWrite.UseVisualStyleBackColor = true;
			this.btnAryWrite.Click += new System.EventHandler(this.btnAryWrite_Click);
			// 
			// btnAryRead
			// 
			this.btnAryRead.Location = new System.Drawing.Point(178, 14);
			this.btnAryRead.Name = "btnAryRead";
			this.btnAryRead.Size = new System.Drawing.Size(75, 23);
			this.btnAryRead.TabIndex = 11;
			this.btnAryRead.Text = "讀取";
			this.btnAryRead.UseVisualStyleBackColor = true;
			this.btnAryRead.Click += new System.EventHandler(this.button2_Click);
			// 
			// txtAryName
			// 
			this.txtAryName.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtAryName.Location = new System.Drawing.Point(61, 14);
			this.txtAryName.Name = "txtAryName";
			this.txtAryName.Size = new System.Drawing.Size(116, 23);
			this.txtAryName.TabIndex = 10;
			this.txtAryName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbAryName
			// 
			this.lbAryName.AutoSize = true;
			this.lbAryName.Location = new System.Drawing.Point(6, 19);
			this.lbAryName.Name = "lbAryName";
			this.lbAryName.Size = new System.Drawing.Size(53, 12);
			this.lbAryName.TabIndex = 9;
			this.lbAryName.Text = "變數名稱";
			// 
			// btnSttGet
			// 
			this.btnSttGet.Location = new System.Drawing.Point(172, 38);
			this.btnSttGet.Name = "btnSttGet";
			this.btnSttGet.Size = new System.Drawing.Size(75, 23);
			this.btnSttGet.TabIndex = 7;
			this.btnSttGet.Text = "讀取";
			this.btnSttGet.UseVisualStyleBackColor = true;
			this.btnSttGet.Click += new System.EventHandler(this.btnSttGet_Click);
			// 
			// lbAryOfs
			// 
			this.lbAryOfs.AutoSize = true;
			this.lbAryOfs.Location = new System.Drawing.Point(6, 45);
			this.lbAryOfs.Name = "lbAryOfs";
			this.lbAryOfs.Size = new System.Drawing.Size(53, 12);
			this.lbAryOfs.TabIndex = 13;
			this.lbAryOfs.Text = "陣列起始";
			// 
			// nudAryOfs
			// 
			this.nudAryOfs.Location = new System.Drawing.Point(61, 41);
			this.nudAryOfs.Name = "nudAryOfs";
			this.nudAryOfs.Size = new System.Drawing.Size(44, 22);
			this.nudAryOfs.TabIndex = 14;
			this.nudAryOfs.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.nudAryOfs.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// gbNotify
			// 
			this.gbNotify.Controls.Add(this.btnNotfDel);
			this.gbNotify.Controls.Add(this.btnNotfAdd);
			this.gbNotify.Controls.Add(this.dgvNotf);
			this.gbNotify.Location = new System.Drawing.Point(626, 12);
			this.gbNotify.Name = "gbNotify";
			this.gbNotify.Size = new System.Drawing.Size(279, 387);
			this.gbNotify.TabIndex = 10;
			this.gbNotify.TabStop = false;
			this.gbNotify.Text = "Device Notification";
			// 
			// dgvNotf
			// 
			this.dgvNotf.AllowUserToAddRows = false;
			this.dgvNotf.AllowUserToDeleteRows = false;
			this.dgvNotf.AllowUserToResizeColumns = false;
			this.dgvNotf.AllowUserToResizeRows = false;
			dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle5.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvNotf.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
			this.dgvNotf.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvNotf.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNotfName,
            this.colNotfVal});
			this.dgvNotf.Location = new System.Drawing.Point(6, 69);
			this.dgvNotf.Name = "dgvNotf";
			this.dgvNotf.ReadOnly = true;
			this.dgvNotf.RowHeadersVisible = false;
			this.dgvNotf.RowTemplate.Height = 24;
			this.dgvNotf.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvNotf.Size = new System.Drawing.Size(267, 312);
			this.dgvNotf.TabIndex = 0;
			// 
			// colNotfName
			// 
			this.colNotfName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colNotfName.HeaderText = "變數名稱";
			this.colNotfName.Name = "colNotfName";
			this.colNotfName.ReadOnly = true;
			this.colNotfName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// colNotfVal
			// 
			this.colNotfVal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colNotfVal.HeaderText = "當前數值";
			this.colNotfVal.Name = "colNotfVal";
			this.colNotfVal.ReadOnly = true;
			this.colNotfVal.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// btnNotfAdd
			// 
			this.btnNotfAdd.Location = new System.Drawing.Point(117, 24);
			this.btnNotfAdd.Name = "btnNotfAdd";
			this.btnNotfAdd.Size = new System.Drawing.Size(75, 23);
			this.btnNotfAdd.TabIndex = 12;
			this.btnNotfAdd.Text = "加入";
			this.btnNotfAdd.UseVisualStyleBackColor = true;
			this.btnNotfAdd.Click += new System.EventHandler(this.btnNotfAdd_Click);
			// 
			// btnNotfDel
			// 
			this.btnNotfDel.Location = new System.Drawing.Point(198, 24);
			this.btnNotfDel.Name = "btnNotfDel";
			this.btnNotfDel.Size = new System.Drawing.Size(75, 23);
			this.btnNotfDel.TabIndex = 13;
			this.btnNotfDel.Text = "移除";
			this.btnNotfDel.UseVisualStyleBackColor = true;
			this.btnNotfDel.Click += new System.EventHandler(this.btnNotfDel_Click);
			// 
			// Test_BkfPLC
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(911, 516);
			this.Controls.Add(this.gbNotify);
			this.Controls.Add(this.gbAry);
			this.Controls.Add(this.gbState);
			this.Controls.Add(this.gbVariable);
			this.Controls.Add(this.dgvInfo);
			this.Controls.Add(this.btnConnect);
			this.Controls.Add(this.txtPort);
			this.Controls.Add(this.lbPort);
			this.Controls.Add(this.txtNetID);
			this.Controls.Add(this.lbNetID);
			this.Name = "Test_BkfPLC";
			this.Text = "Test Platform of Beckhoff PLC";
			((System.ComponentModel.ISupportInitialize)(this.dgvInfo)).EndInit();
			this.gbVariable.ResumeLayout(false);
			this.gbVariable.PerformLayout();
			this.gbState.ResumeLayout(false);
			this.gbState.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
			this.gbAry.ResumeLayout(false);
			this.gbAry.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudAryOfs)).EndInit();
			this.gbNotify.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvNotf)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbNetID;
        private System.Windows.Forms.TextBox txtNetID;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lbPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.DataGridView dgvInfo;
        private System.Windows.Forms.GroupBox gbVariable;
        private System.Windows.Forms.Button btnVarAdMoni;
        private System.Windows.Forms.Button btnVarWrite;
        private System.Windows.Forms.Button btnVarRead;
        private System.Windows.Forms.TextBox txtVarVal;
        private System.Windows.Forms.Label lbVarVal;
        private System.Windows.Forms.TextBox txtVarName;
        private System.Windows.Forms.Label lbVarName;
        private System.Windows.Forms.GroupBox gbState;
        private System.Windows.Forms.TextBox txtStt;
        private System.Windows.Forms.Label lbStt;
        private System.Windows.Forms.Button btnSttSet;
        private System.Windows.Forms.ComboBox cbSttList;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colInfo;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.GroupBox gbAry;
        private System.Windows.Forms.Button btnAryWrite;
        private System.Windows.Forms.Button btnAryRead;
        private System.Windows.Forms.TextBox txtAryName;
        private System.Windows.Forms.Label lbAryName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colData;
		private System.Windows.Forms.Button btnSttGet;
		private System.Windows.Forms.NumericUpDown nudAryOfs;
		private System.Windows.Forms.Label lbAryOfs;
		private System.Windows.Forms.GroupBox gbNotify;
		private System.Windows.Forms.Button btnNotfDel;
		private System.Windows.Forms.Button btnNotfAdd;
		private System.Windows.Forms.DataGridView dgvNotf;
		private System.Windows.Forms.DataGridViewTextBoxColumn colNotfName;
		private System.Windows.Forms.DataGridViewTextBoxColumn colNotfVal;
	}
}