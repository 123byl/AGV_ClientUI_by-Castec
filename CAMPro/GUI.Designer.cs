namespace CAMPro
{
    partial class GUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUI));
            MapGL.CastecMapUI.Pos pos1 = new MapGL.CastecMapUI.Pos();
            this.tbpGoalList = new System.Windows.Forms.TabPage();
            this.btnPower = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnErase = new System.Windows.Forms.Button();
            this.lbAGVStatus = new System.Windows.Forms.Label();
            this.cmbGoalList = new System.Windows.Forms.ComboBox();
            this.btnSaveMap = new System.Windows.Forms.Button();
            this.btnRunAll = new System.Windows.Forms.Button();
            this.btnPath = new System.Windows.Forms.Button();
            this.btnGoGoal = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.txtAddPtheta = new System.Windows.Forms.TextBox();
            this.txtAddPy = new System.Windows.Forms.TextBox();
            this.txtAddPx = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.dgvGoalPoint = new System.Windows.Forms.DataGridView();
            this.cSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cTheta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cArrive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnNewPoint = new System.Windows.Forms.Button();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.tbpBasic = new System.Windows.Forms.TabPage();
            this.gbModeChange = new System.Windows.Forms.GroupBox();
            this.btnIdleMode = new System.Windows.Forms.Button();
            this.btnWorkMode = new System.Windows.Forms.Button();
            this.btnMapMode = new System.Windows.Forms.Button();
            this.btnSetIP = new System.Windows.Forms.Button();
            this.btnResetThread = new System.Windows.Forms.Button();
            this.btnGetMap = new System.Windows.Forms.Button();
            this.btnSendMap = new System.Windows.Forms.Button();
            this.btnCursorMode = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnPosConfirm = new System.Windows.Forms.Button();
            this.btnSetCar = new System.Windows.Forms.Button();
            this.btnSimplify = new System.Windows.Forms.Button();
            this.btnCorrectMap = new System.Windows.Forms.Button();
            this.btnDLOri = new System.Windows.Forms.Button();
            this.btnLoadMap = new System.Windows.Forms.Button();
            this.btnLoadOri = new System.Windows.Forms.Button();
            this.btnGetCarStatus = new System.Windows.Forms.Button();
            this.btnClrMap = new System.Windows.Forms.Button();
            this.btnGetLaser = new System.Windows.Forms.Button();
            this.gpbBattery = new System.Windows.Forms.GroupBox();
            this.pBarPowerPercent = new System.Windows.Forms.ProgressBar();
            this.gpbShift = new System.Windows.Forms.GroupBox();
            this.btnSetVelo = new System.Windows.Forms.Button();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.lbVelocity = new System.Windows.Forms.Label();
            this.txtVelocity = new System.Windows.Forms.TextBox();
            this.btnServoOnOff = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.tabcSetting = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tbMsg = new System.Windows.Forms.TextBox();
            this.btnPannel = new System.Windows.Forms.Button();
            this.MapUI1 = new MapGL.CastecMapUI();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.lbConnectGoal = new System.Windows.Forms.Label();
            this.tbpGoalList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGoalPoint)).BeginInit();
            this.tbpBasic.SuspendLayout();
            this.gbModeChange.SuspendLayout();
            this.gpbBattery.SuspendLayout();
            this.gpbShift.SuspendLayout();
            this.tabcSetting.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbpGoalList
            // 
            this.tbpGoalList.Controls.Add(this.btnPower);
            this.tbpGoalList.Controls.Add(this.btnStop);
            this.tbpGoalList.Controls.Add(this.btnErase);
            this.tbpGoalList.Controls.Add(this.lbAGVStatus);
            this.tbpGoalList.Controls.Add(this.cmbGoalList);
            this.tbpGoalList.Controls.Add(this.btnSaveMap);
            this.tbpGoalList.Controls.Add(this.btnRunAll);
            this.tbpGoalList.Controls.Add(this.btnPath);
            this.tbpGoalList.Controls.Add(this.btnGoGoal);
            this.tbpGoalList.Controls.Add(this.label14);
            this.tbpGoalList.Controls.Add(this.txtAddPtheta);
            this.tbpGoalList.Controls.Add(this.txtAddPy);
            this.tbpGoalList.Controls.Add(this.txtAddPx);
            this.tbpGoalList.Controls.Add(this.label15);
            this.tbpGoalList.Controls.Add(this.label16);
            this.tbpGoalList.Controls.Add(this.dgvGoalPoint);
            this.tbpGoalList.Controls.Add(this.btnNewPoint);
            this.tbpGoalList.Controls.Add(this.btnDeleteAll);
            this.tbpGoalList.Controls.Add(this.btnDelete);
            this.tbpGoalList.Location = new System.Drawing.Point(4, 28);
            this.tbpGoalList.Name = "tbpGoalList";
            this.tbpGoalList.Padding = new System.Windows.Forms.Padding(3);
            this.tbpGoalList.Size = new System.Drawing.Size(543, 639);
            this.tbpGoalList.TabIndex = 1;
            this.tbpGoalList.Text = "Setting Goal List";
            this.tbpGoalList.UseVisualStyleBackColor = true;
            // 
            // btnPower
            // 
            this.btnPower.Location = new System.Drawing.Point(343, 338);
            this.btnPower.Name = "btnPower";
            this.btnPower.Size = new System.Drawing.Size(65, 65);
            this.btnPower.TabIndex = 48;
            this.btnPower.Text = "Power Area";
            this.btnPower.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(272, 338);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(65, 65);
            this.btnStop.TabIndex = 49;
            this.btnStop.Text = "Stop Area";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // btnErase
            // 
            this.btnErase.Location = new System.Drawing.Point(201, 338);
            this.btnErase.Name = "btnErase";
            this.btnErase.Size = new System.Drawing.Size(65, 65);
            this.btnErase.TabIndex = 50;
            this.btnErase.Text = "Erase Tool";
            this.btnErase.UseVisualStyleBackColor = true;
            // 
            // lbAGVStatus
            // 
            this.lbAGVStatus.AutoSize = true;
            this.lbAGVStatus.BackColor = System.Drawing.Color.Black;
            this.lbAGVStatus.ForeColor = System.Drawing.Color.Yellow;
            this.lbAGVStatus.Location = new System.Drawing.Point(92, 574);
            this.lbAGVStatus.Name = "lbAGVStatus";
            this.lbAGVStatus.Size = new System.Drawing.Size(57, 19);
            this.lbAGVStatus.TabIndex = 47;
            this.lbAGVStatus.Text = "Status : ";
            // 
            // cmbGoalList
            // 
            this.cmbGoalList.FormattingEnabled = true;
            this.cmbGoalList.Location = new System.Drawing.Point(414, 24);
            this.cmbGoalList.Name = "cmbGoalList";
            this.cmbGoalList.Size = new System.Drawing.Size(83, 27);
            this.cmbGoalList.TabIndex = 46;
            // 
            // btnSaveMap
            // 
            this.btnSaveMap.Location = new System.Drawing.Point(414, 338);
            this.btnSaveMap.Name = "btnSaveMap";
            this.btnSaveMap.Size = new System.Drawing.Size(120, 50);
            this.btnSaveMap.TabIndex = 45;
            this.btnSaveMap.Text = "Save Goal List";
            this.btnSaveMap.UseVisualStyleBackColor = true;
            this.btnSaveMap.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnRunAll
            // 
            this.btnRunAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRunAll.Location = new System.Drawing.Point(414, 170);
            this.btnRunAll.Name = "btnRunAll";
            this.btnRunAll.Size = new System.Drawing.Size(120, 50);
            this.btnRunAll.TabIndex = 44;
            this.btnRunAll.Text = "Run All";
            this.btnRunAll.UseVisualStyleBackColor = true;
            this.btnRunAll.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnPath
            // 
            this.btnPath.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPath.Location = new System.Drawing.Point(414, 58);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(120, 50);
            this.btnPath.TabIndex = 43;
            this.btnPath.Text = "Path";
            this.btnPath.UseVisualStyleBackColor = true;
            this.btnPath.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnGoGoal
            // 
            this.btnGoGoal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGoGoal.Location = new System.Drawing.Point(414, 114);
            this.btnGoGoal.Name = "btnGoGoal";
            this.btnGoGoal.Size = new System.Drawing.Size(120, 50);
            this.btnGoGoal.TabIndex = 43;
            this.btnGoGoal.Text = "Run";
            this.btnGoGoal.UseVisualStyleBackColor = true;
            this.btnGoGoal.Click += new System.EventHandler(this.Button_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(362, 2);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(17, 19);
            this.label14.TabIndex = 40;
            this.label14.Text = "θ";
            // 
            // txtAddPtheta
            // 
            this.txtAddPtheta.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddPtheta.Location = new System.Drawing.Point(336, 25);
            this.txtAddPtheta.Name = "txtAddPtheta";
            this.txtAddPtheta.Size = new System.Drawing.Size(72, 26);
            this.txtAddPtheta.TabIndex = 37;
            this.txtAddPtheta.Text = "0";
            this.txtAddPtheta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtAddPy
            // 
            this.txtAddPy.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddPy.Location = new System.Drawing.Point(247, 25);
            this.txtAddPy.Name = "txtAddPy";
            this.txtAddPy.Size = new System.Drawing.Size(72, 26);
            this.txtAddPy.TabIndex = 38;
            this.txtAddPy.Text = "0";
            this.txtAddPy.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtAddPx
            // 
            this.txtAddPx.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddPx.Location = new System.Drawing.Point(158, 25);
            this.txtAddPx.Name = "txtAddPx";
            this.txtAddPx.Size = new System.Drawing.Size(72, 26);
            this.txtAddPx.TabIndex = 39;
            this.txtAddPx.Text = "0";
            this.txtAddPx.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(276, 2);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(16, 19);
            this.label15.TabIndex = 41;
            this.label15.Text = "y";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(187, 2);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(17, 19);
            this.label16.TabIndex = 42;
            this.label16.Text = "x";
            // 
            // dgvGoalPoint
            // 
            this.dgvGoalPoint.AllowUserToAddRows = false;
            this.dgvGoalPoint.AllowUserToResizeRows = false;
            this.dgvGoalPoint.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGoalPoint.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cSelect,
            this.cX,
            this.cY,
            this.cTheta,
            this.cArrive});
            this.dgvGoalPoint.Location = new System.Drawing.Point(9, 58);
            this.dgvGoalPoint.Name = "dgvGoalPoint";
            this.dgvGoalPoint.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
            this.dgvGoalPoint.RowTemplate.Height = 24;
            this.dgvGoalPoint.Size = new System.Drawing.Size(399, 274);
            this.dgvGoalPoint.TabIndex = 33;
            // 
            // cSelect
            // 
            this.cSelect.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cSelect.FillWeight = 80F;
            this.cSelect.HeaderText = "Select";
            this.cSelect.Name = "cSelect";
            // 
            // cX
            // 
            this.cX.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cX.DataPropertyName = "double";
            this.cX.FillWeight = 80F;
            this.cX.HeaderText = "X";
            this.cX.Name = "cX";
            this.cX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cY
            // 
            this.cY.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cY.DataPropertyName = "double";
            this.cY.FillWeight = 80F;
            this.cY.HeaderText = "Y";
            this.cY.Name = "cY";
            this.cY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cTheta
            // 
            this.cTheta.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cTheta.DataPropertyName = "double";
            this.cTheta.FillWeight = 80F;
            this.cTheta.HeaderText = "Theta";
            this.cTheta.Name = "cTheta";
            this.cTheta.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cArrive
            // 
            this.cArrive.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cArrive.FillWeight = 80F;
            this.cArrive.HeaderText = "Note";
            this.cArrive.Name = "cArrive";
            this.cArrive.ReadOnly = true;
            this.cArrive.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btnNewPoint
            // 
            this.btnNewPoint.Location = new System.Drawing.Point(15, 1);
            this.btnNewPoint.Name = "btnNewPoint";
            this.btnNewPoint.Size = new System.Drawing.Size(120, 50);
            this.btnNewPoint.TabIndex = 34;
            this.btnNewPoint.Text = "Add New Point";
            this.btnNewPoint.UseVisualStyleBackColor = true;
            this.btnNewPoint.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.Location = new System.Drawing.Point(414, 282);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(120, 50);
            this.btnDeleteAll.TabIndex = 35;
            this.btnDeleteAll.Text = "Delete All";
            this.btnDeleteAll.UseVisualStyleBackColor = true;
            this.btnDeleteAll.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(414, 226);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(120, 50);
            this.btnDelete.TabIndex = 36;
            this.btnDelete.Text = "Delete Selection";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.Button_Click);
            // 
            // tbpBasic
            // 
            this.tbpBasic.Controls.Add(this.gbModeChange);
            this.tbpBasic.Controls.Add(this.btnSetIP);
            this.tbpBasic.Controls.Add(this.btnResetThread);
            this.tbpBasic.Controls.Add(this.btnGetMap);
            this.tbpBasic.Controls.Add(this.btnSendMap);
            this.tbpBasic.Controls.Add(this.btnCursorMode);
            this.tbpBasic.Controls.Add(this.btnConnect);
            this.tbpBasic.Controls.Add(this.btnPosConfirm);
            this.tbpBasic.Controls.Add(this.btnSetCar);
            this.tbpBasic.Controls.Add(this.btnSimplify);
            this.tbpBasic.Controls.Add(this.btnCorrectMap);
            this.tbpBasic.Controls.Add(this.btnDLOri);
            this.tbpBasic.Controls.Add(this.btnLoadMap);
            this.tbpBasic.Controls.Add(this.btnLoadOri);
            this.tbpBasic.Controls.Add(this.btnGetCarStatus);
            this.tbpBasic.Controls.Add(this.btnClrMap);
            this.tbpBasic.Controls.Add(this.btnGetLaser);
            this.tbpBasic.Controls.Add(this.gpbBattery);
            this.tbpBasic.Controls.Add(this.gpbShift);
            this.tbpBasic.Location = new System.Drawing.Point(4, 28);
            this.tbpBasic.Name = "tbpBasic";
            this.tbpBasic.Padding = new System.Windows.Forms.Padding(3);
            this.tbpBasic.Size = new System.Drawing.Size(543, 639);
            this.tbpBasic.TabIndex = 0;
            this.tbpBasic.Text = "Testing Panel";
            this.tbpBasic.UseVisualStyleBackColor = true;
            // 
            // gbModeChange
            // 
            this.gbModeChange.Controls.Add(this.btnIdleMode);
            this.gbModeChange.Controls.Add(this.btnWorkMode);
            this.gbModeChange.Controls.Add(this.btnMapMode);
            this.gbModeChange.Location = new System.Drawing.Point(8, 324);
            this.gbModeChange.Name = "gbModeChange";
            this.gbModeChange.Size = new System.Drawing.Size(217, 99);
            this.gbModeChange.TabIndex = 32;
            this.gbModeChange.TabStop = false;
            this.gbModeChange.Text = "Server Mode";
            // 
            // btnIdleMode
            // 
            this.btnIdleMode.Location = new System.Drawing.Point(148, 25);
            this.btnIdleMode.Name = "btnIdleMode";
            this.btnIdleMode.Size = new System.Drawing.Size(65, 63);
            this.btnIdleMode.TabIndex = 23;
            this.btnIdleMode.Text = "Idle Mode";
            this.btnIdleMode.UseVisualStyleBackColor = true;
            this.btnIdleMode.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnWorkMode
            // 
            this.btnWorkMode.Location = new System.Drawing.Point(77, 25);
            this.btnWorkMode.Name = "btnWorkMode";
            this.btnWorkMode.Size = new System.Drawing.Size(65, 63);
            this.btnWorkMode.TabIndex = 23;
            this.btnWorkMode.Text = "Work Mode";
            this.btnWorkMode.UseVisualStyleBackColor = true;
            this.btnWorkMode.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnMapMode
            // 
            this.btnMapMode.Location = new System.Drawing.Point(6, 25);
            this.btnMapMode.Name = "btnMapMode";
            this.btnMapMode.Size = new System.Drawing.Size(65, 63);
            this.btnMapMode.TabIndex = 23;
            this.btnMapMode.Text = "Map Mode";
            this.btnMapMode.UseVisualStyleBackColor = true;
            this.btnMapMode.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnSetIP
            // 
            this.btnSetIP.BackColor = System.Drawing.Color.DimGray;
            this.btnSetIP.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetIP.ForeColor = System.Drawing.Color.Yellow;
            this.btnSetIP.Location = new System.Drawing.Point(8, 546);
            this.btnSetIP.Name = "btnSetIP";
            this.btnSetIP.Size = new System.Drawing.Size(156, 53);
            this.btnSetIP.TabIndex = 31;
            this.btnSetIP.Text = "Set IP";
            this.btnSetIP.UseVisualStyleBackColor = false;
            this.btnSetIP.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnResetThread
            // 
            this.btnResetThread.BackColor = System.Drawing.Color.DimGray;
            this.btnResetThread.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResetThread.ForeColor = System.Drawing.Color.Yellow;
            this.btnResetThread.Location = new System.Drawing.Point(8, 487);
            this.btnResetThread.Name = "btnResetThread";
            this.btnResetThread.Size = new System.Drawing.Size(156, 53);
            this.btnResetThread.TabIndex = 31;
            this.btnResetThread.Text = "Reset Server";
            this.btnResetThread.UseVisualStyleBackColor = false;
            this.btnResetThread.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnGetMap
            // 
            this.btnGetMap.Location = new System.Drawing.Point(450, 522);
            this.btnGetMap.Name = "btnGetMap";
            this.btnGetMap.Size = new System.Drawing.Size(65, 65);
            this.btnGetMap.TabIndex = 30;
            this.btnGetMap.Text = "Get Map";
            this.btnGetMap.UseVisualStyleBackColor = true;
            this.btnGetMap.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnSendMap
            // 
            this.btnSendMap.Location = new System.Drawing.Point(379, 522);
            this.btnSendMap.Name = "btnSendMap";
            this.btnSendMap.Size = new System.Drawing.Size(65, 65);
            this.btnSendMap.TabIndex = 29;
            this.btnSendMap.Text = "Send Map";
            this.btnSendMap.UseVisualStyleBackColor = true;
            this.btnSendMap.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnCursorMode
            // 
            this.btnCursorMode.Location = new System.Drawing.Point(308, 522);
            this.btnCursorMode.Name = "btnCursorMode";
            this.btnCursorMode.Size = new System.Drawing.Size(65, 65);
            this.btnCursorMode.TabIndex = 25;
            this.btnCursorMode.Text = "Cursor Mode";
            this.btnCursorMode.UseVisualStyleBackColor = true;
            this.btnCursorMode.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Image = global::CAMPro.Properties.Resources.Disconnect;
            this.btnConnect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConnect.Location = new System.Drawing.Point(8, 429);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(156, 52);
            this.btnConnect.TabIndex = 24;
            this.btnConnect.Text = "Connect AGV";
            this.btnConnect.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnPosConfirm
            // 
            this.btnPosConfirm.Location = new System.Drawing.Point(379, 311);
            this.btnPosConfirm.Name = "btnPosConfirm";
            this.btnPosConfirm.Size = new System.Drawing.Size(65, 65);
            this.btnPosConfirm.TabIndex = 22;
            this.btnPosConfirm.Text = "confirm car";
            this.btnPosConfirm.UseVisualStyleBackColor = true;
            this.btnPosConfirm.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnSetCar
            // 
            this.btnSetCar.Location = new System.Drawing.Point(308, 311);
            this.btnSetCar.Name = "btnSetCar";
            this.btnSetCar.Size = new System.Drawing.Size(65, 65);
            this.btnSetCar.TabIndex = 22;
            this.btnSetCar.Text = "Set Car";
            this.btnSetCar.UseVisualStyleBackColor = true;
            this.btnSetCar.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnSimplify
            // 
            this.btnSimplify.Location = new System.Drawing.Point(450, 450);
            this.btnSimplify.Name = "btnSimplify";
            this.btnSimplify.Size = new System.Drawing.Size(65, 65);
            this.btnSimplify.TabIndex = 21;
            this.btnSimplify.Text = "Simplify Ori";
            this.btnSimplify.UseVisualStyleBackColor = true;
            this.btnSimplify.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnCorrectMap
            // 
            this.btnCorrectMap.Location = new System.Drawing.Point(379, 451);
            this.btnCorrectMap.Name = "btnCorrectMap";
            this.btnCorrectMap.Size = new System.Drawing.Size(65, 65);
            this.btnCorrectMap.TabIndex = 21;
            this.btnCorrectMap.Text = "Correct Ori";
            this.btnCorrectMap.UseVisualStyleBackColor = true;
            this.btnCorrectMap.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnDLOri
            // 
            this.btnDLOri.Location = new System.Drawing.Point(308, 98);
            this.btnDLOri.Name = "btnDLOri";
            this.btnDLOri.Size = new System.Drawing.Size(65, 65);
            this.btnDLOri.TabIndex = 20;
            this.btnDLOri.Text = "Get Ori";
            this.btnDLOri.UseVisualStyleBackColor = true;
            this.btnDLOri.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnLoadMap
            // 
            this.btnLoadMap.Location = new System.Drawing.Point(379, 169);
            this.btnLoadMap.Name = "btnLoadMap";
            this.btnLoadMap.Size = new System.Drawing.Size(65, 65);
            this.btnLoadMap.TabIndex = 20;
            this.btnLoadMap.Text = "Load Map";
            this.btnLoadMap.UseVisualStyleBackColor = true;
            this.btnLoadMap.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnLoadOri
            // 
            this.btnLoadOri.Location = new System.Drawing.Point(308, 451);
            this.btnLoadOri.Name = "btnLoadOri";
            this.btnLoadOri.Size = new System.Drawing.Size(65, 65);
            this.btnLoadOri.TabIndex = 20;
            this.btnLoadOri.Text = "Load Ori";
            this.btnLoadOri.UseVisualStyleBackColor = true;
            this.btnLoadOri.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnGetCarStatus
            // 
            this.btnGetCarStatus.Location = new System.Drawing.Point(450, 99);
            this.btnGetCarStatus.Name = "btnGetCarStatus";
            this.btnGetCarStatus.Size = new System.Drawing.Size(65, 65);
            this.btnGetCarStatus.TabIndex = 19;
            this.btnGetCarStatus.Text = "Car";
            this.btnGetCarStatus.UseVisualStyleBackColor = true;
            this.btnGetCarStatus.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnClrMap
            // 
            this.btnClrMap.Location = new System.Drawing.Point(308, 169);
            this.btnClrMap.Name = "btnClrMap";
            this.btnClrMap.Size = new System.Drawing.Size(65, 65);
            this.btnClrMap.TabIndex = 18;
            this.btnClrMap.Text = "Clear Map";
            this.btnClrMap.UseVisualStyleBackColor = true;
            this.btnClrMap.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnGetLaser
            // 
            this.btnGetLaser.Location = new System.Drawing.Point(379, 98);
            this.btnGetLaser.Name = "btnGetLaser";
            this.btnGetLaser.Size = new System.Drawing.Size(65, 65);
            this.btnGetLaser.TabIndex = 16;
            this.btnGetLaser.Text = "Get Laser";
            this.btnGetLaser.UseVisualStyleBackColor = true;
            this.btnGetLaser.Click += new System.EventHandler(this.Button_Click);
            // 
            // gpbBattery
            // 
            this.gpbBattery.Controls.Add(this.pBarPowerPercent);
            this.gpbBattery.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpbBattery.Location = new System.Drawing.Point(8, 6);
            this.gpbBattery.Name = "gpbBattery";
            this.gpbBattery.Size = new System.Drawing.Size(515, 77);
            this.gpbBattery.TabIndex = 15;
            this.gpbBattery.TabStop = false;
            this.gpbBattery.Text = "Battery (%)";
            // 
            // pBarPowerPercent
            // 
            this.pBarPowerPercent.Location = new System.Drawing.Point(6, 23);
            this.pBarPowerPercent.Name = "pBarPowerPercent";
            this.pBarPowerPercent.Size = new System.Drawing.Size(503, 38);
            this.pBarPowerPercent.TabIndex = 0;
            // 
            // gpbShift
            // 
            this.gpbShift.Controls.Add(this.btnSetVelo);
            this.gpbShift.Controls.Add(this.btnStartStop);
            this.gpbShift.Controls.Add(this.lbVelocity);
            this.gpbShift.Controls.Add(this.txtVelocity);
            this.gpbShift.Controls.Add(this.btnServoOnOff);
            this.gpbShift.Controls.Add(this.btnUp);
            this.gpbShift.Controls.Add(this.btnRight);
            this.gpbShift.Controls.Add(this.btnLeft);
            this.gpbShift.Controls.Add(this.btnDown);
            this.gpbShift.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpbShift.Location = new System.Drawing.Point(8, 89);
            this.gpbShift.Name = "gpbShift";
            this.gpbShift.Size = new System.Drawing.Size(294, 229);
            this.gpbShift.TabIndex = 14;
            this.gpbShift.TabStop = false;
            this.gpbShift.Text = "Velocity";
            // 
            // btnSetVelo
            // 
            this.btnSetVelo.Location = new System.Drawing.Point(212, 27);
            this.btnSetVelo.Name = "btnSetVelo";
            this.btnSetVelo.Size = new System.Drawing.Size(50, 28);
            this.btnSetVelo.TabIndex = 11;
            this.btnSetVelo.Text = "Set";
            this.btnSetVelo.UseVisualStyleBackColor = true;
            this.btnSetVelo.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnStartStop
            // 
            this.btnStartStop.BackColor = System.Drawing.SystemColors.Control;
            this.btnStartStop.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStartStop.Image")));
            this.btnStartStop.Location = new System.Drawing.Point(212, 78);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(50, 50);
            this.btnStartStop.TabIndex = 10;
            this.btnStartStop.Tag = "Stop";
            this.btnStartStop.UseVisualStyleBackColor = false;
            this.btnStartStop.Click += new System.EventHandler(this.Button_Click);
            // 
            // lbVelocity
            // 
            this.lbVelocity.AutoSize = true;
            this.lbVelocity.BackColor = System.Drawing.Color.Transparent;
            this.lbVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVelocity.Location = new System.Drawing.Point(11, 33);
            this.lbVelocity.Name = "lbVelocity";
            this.lbVelocity.Size = new System.Drawing.Size(129, 19);
            this.lbVelocity.TabIndex = 1;
            this.lbVelocity.Text = "Movement Velocity:";
            // 
            // txtVelocity
            // 
            this.txtVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVelocity.Location = new System.Drawing.Point(144, 29);
            this.txtVelocity.Name = "txtVelocity";
            this.txtVelocity.Size = new System.Drawing.Size(51, 26);
            this.txtVelocity.TabIndex = 0;
            this.txtVelocity.Text = "500";
            this.txtVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnServoOnOff
            // 
            this.btnServoOnOff.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnServoOnOff.Location = new System.Drawing.Point(33, 75);
            this.btnServoOnOff.Name = "btnServoOnOff";
            this.btnServoOnOff.Size = new System.Drawing.Size(50, 50);
            this.btnServoOnOff.TabIndex = 1;
            this.btnServoOnOff.Text = "OFF";
            this.btnServoOnOff.UseVisualStyleBackColor = true;
            this.btnServoOnOff.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnUp
            // 
            this.btnUp.Font = new System.Drawing.Font("新細明體", 20F);
            this.btnUp.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
            this.btnUp.Location = new System.Drawing.Point(123, 75);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(50, 50);
            this.btnUp.TabIndex = 1;
            this.btnUp.Tag = "1";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.Button_Click);
            this.btnUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_MouseDown);
            this.btnUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_MouseUp);
            // 
            // btnRight
            // 
            this.btnRight.Font = new System.Drawing.Font("新細明體", 20F);
            this.btnRight.Image = ((System.Drawing.Image)(resources.GetObject("btnRight.Image")));
            this.btnRight.Location = new System.Drawing.Point(212, 154);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(50, 50);
            this.btnRight.TabIndex = 1;
            this.btnRight.Tag = "4";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.Button_Click);
            this.btnRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_MouseDown);
            this.btnRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_MouseUp);
            // 
            // btnLeft
            // 
            this.btnLeft.Font = new System.Drawing.Font("新細明體", 20F);
            this.btnLeft.Image = ((System.Drawing.Image)(resources.GetObject("btnLeft.Image")));
            this.btnLeft.Location = new System.Drawing.Point(33, 154);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(50, 50);
            this.btnLeft.TabIndex = 1;
            this.btnLeft.Tag = "3";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.Button_Click);
            this.btnLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_MouseDown);
            this.btnLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_MouseUp);
            // 
            // btnDown
            // 
            this.btnDown.Font = new System.Drawing.Font("新細明體", 20F);
            this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
            this.btnDown.Location = new System.Drawing.Point(123, 154);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(50, 50);
            this.btnDown.TabIndex = 1;
            this.btnDown.Tag = "2";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.Button_Click);
            this.btnDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_MouseDown);
            this.btnDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_MouseUp);
            // 
            // tabcSetting
            // 
            this.tabcSetting.AllowDrop = true;
            this.tabcSetting.Controls.Add(this.tbpBasic);
            this.tabcSetting.Controls.Add(this.tbpGoalList);
            this.tabcSetting.Controls.Add(this.tabPage1);
            this.tabcSetting.Dock = System.Windows.Forms.DockStyle.Left;
            this.tabcSetting.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.tabcSetting.Location = new System.Drawing.Point(0, 0);
            this.tabcSetting.Margin = new System.Windows.Forms.Padding(0);
            this.tabcSetting.Name = "tabcSetting";
            this.tabcSetting.SelectedIndex = 0;
            this.tabcSetting.Size = new System.Drawing.Size(551, 671);
            this.tabcSetting.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbMsg);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(543, 639);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Console";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tbMsg
            // 
            this.tbMsg.Location = new System.Drawing.Point(3, 6);
            this.tbMsg.Multiline = true;
            this.tbMsg.Name = "tbMsg";
            this.tbMsg.ReadOnly = true;
            this.tbMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbMsg.Size = new System.Drawing.Size(534, 397);
            this.tbMsg.TabIndex = 0;
            // 
            // btnPannel
            // 
            this.btnPannel.AutoSize = true;
            this.btnPannel.BackColor = System.Drawing.Color.Silver;
            this.btnPannel.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnPannel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPannel.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPannel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnPannel.Location = new System.Drawing.Point(520, -3);
            this.btnPannel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.btnPannel.Name = "btnPannel";
            this.btnPannel.Size = new System.Drawing.Size(30, 30);
            this.btnPannel.TabIndex = 0;
            this.btnPannel.Text = "<";
            this.btnPannel.UseVisualStyleBackColor = false;
            this.btnPannel.Click += new System.EventHandler(this.Button_Click);
            // 
            // MapUI1
            // 
            this.MapUI1.AllowDrop = true;
            this.MapUI1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MapUI1.AutoSize = true;
            this.MapUI1.BackColor = System.Drawing.SystemColors.Control;
            this.MapUI1.CarShape = MapGL.CastecMapUI.Shape.Rectangle;
            this.MapUI1.ColorAxisX = System.Drawing.Color.Red;
            this.MapUI1.ColorAxisY = System.Drawing.Color.Green;
            this.MapUI1.ColorBackground = System.Drawing.Color.White;
            this.MapUI1.ColorCarIcon = System.Drawing.Color.Blue;
            this.MapUI1.ColorGoalIcon = System.Drawing.Color.Lime;
            this.MapUI1.ColorGrid = System.Drawing.Color.Silver;
            this.MapUI1.ColorObstacle = System.Drawing.Color.Maroon;
            this.MapUI1.ColorPowerIcon = System.Drawing.Color.Orange;
            this.MapUI1.ColorTextPoint = System.Drawing.Color.Yellow;
            this.MapUI1.CountTotalCar = ((uint)(0u));
            this.MapUI1.CountTotalPower = ((uint)(1u));
            this.MapUI1.EnableAxis = false;
            this.MapUI1.EnableCar = true;
            this.MapUI1.EnableGoal = true;
            this.MapUI1.EnableGrid = true;
            this.MapUI1.EnableMouseLocatation = true;
            this.MapUI1.EnablePower = false;
            this.MapUI1.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.MapUI1.FontText = new System.Drawing.Font("Arial", 12F);
            this.MapUI1.GoalShape = MapGL.CastecMapUI.Shape.Rectangle;
            this.MapUI1.Location = new System.Drawing.Point(551, 0);
            this.MapUI1.Margin = new System.Windows.Forms.Padding(0);
            this.MapUI1.MaxPos = new System.Drawing.Point(2000, 2000);
            this.MapUI1.MinPos = new System.Drawing.Point(-2000, -2000);
            this.MapUI1.Name = "MapUI1";
            pos1.id = ((uint)(0u));
            pos1.name = null;
            pos1.theta = 0D;
            pos1.x = 0D;
            pos1.y = 0D;
            this.MapUI1.PosCar = pos1;
            this.MapUI1.Resolution = 1;
            this.MapUI1.Size = new System.Drawing.Size(660, 1024);
            this.MapUI1.SizeCar = new System.Drawing.Size(700, 560);
            this.MapUI1.SizeGoal = new System.Drawing.Size(750, 610);
            this.MapUI1.SizeGrid = 1000;
            this.MapUI1.TabIndex = 0;
            this.MapUI1.Zoom = 7.5D;
            this.MapUI1.MouseSelectObj += new MapGL.CastecMapUI.DelMouseSelectObj(this.MapUI1_MouseSelectObj);
            this.MapUI1.MouseClickRealPos += new MapGL.CastecMapUI.DelMouseClickRealPos(this.MapUI1_MouseClickRealPos);
            this.MapUI1.MouseSelectRange += new MapGL.CastecMapUI.DelMouseSelectRange(this.MapUI1_MouseSelectRange);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // lbConnectGoal
            // 
            this.lbConnectGoal.AutoSize = true;
            this.lbConnectGoal.BackColor = System.Drawing.Color.Silver;
            this.lbConnectGoal.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbConnectGoal.ForeColor = System.Drawing.Color.Yellow;
            this.lbConnectGoal.Location = new System.Drawing.Point(554, 0);
            this.lbConnectGoal.Name = "lbConnectGoal";
            this.lbConnectGoal.Size = new System.Drawing.Size(81, 30);
            this.lbConnectGoal.TabIndex = 33;
            this.lbConnectGoal.Text = "label1";
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(1207, 671);
            this.Controls.Add(this.lbConnectGoal);
            this.Controls.Add(this.btnPannel);
            this.Controls.Add(this.tabcSetting);
            this.Controls.Add(this.MapUI1);
            this.Name = "GUI";
            this.Text = "CASTEC - Monitoring and Control System";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GUI_FormClosed);
            this.Load += new System.EventHandler(this.GUI_Load);
            this.tbpGoalList.ResumeLayout(false);
            this.tbpGoalList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGoalPoint)).EndInit();
            this.tbpBasic.ResumeLayout(false);
            this.gbModeChange.ResumeLayout(false);
            this.gpbBattery.ResumeLayout(false);
            this.gpbShift.ResumeLayout(false);
            this.gpbShift.PerformLayout();
            this.tabcSetting.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MapGL.CastecMapUI MapUI1;
        private System.Windows.Forms.TabPage tbpGoalList;
        private System.Windows.Forms.Button btnRunAll;
        private System.Windows.Forms.Button btnGoGoal;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtAddPtheta;
        private System.Windows.Forms.TextBox txtAddPy;
        private System.Windows.Forms.TextBox txtAddPx;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.DataGridView dgvGoalPoint;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn cX;
        private System.Windows.Forms.DataGridViewTextBoxColumn cY;
        private System.Windows.Forms.DataGridViewTextBoxColumn cTheta;
        private System.Windows.Forms.DataGridViewTextBoxColumn cArrive;
        private System.Windows.Forms.Button btnNewPoint;
        private System.Windows.Forms.Button btnDeleteAll;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TabPage tbpBasic;
        private System.Windows.Forms.GroupBox gpbBattery;
        private System.Windows.Forms.ProgressBar pBarPowerPercent;
        private System.Windows.Forms.GroupBox gpbShift;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Label lbVelocity;
        private System.Windows.Forms.TextBox txtVelocity;
        private System.Windows.Forms.Button btnServoOnOff;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.TabControl tabcSetting;
        private System.Windows.Forms.Button btnPannel;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox tbMsg;
        private System.Windows.Forms.Button btnGetLaser;
        private System.Windows.Forms.Button btnClrMap;
        private System.Windows.Forms.Button btnGetCarStatus;
        private System.Windows.Forms.Button btnSetVelo;
        private System.Windows.Forms.Button btnLoadOri;
        private System.Windows.Forms.Button btnDLOri;
        private System.Windows.Forms.Button btnCorrectMap;
        private System.Windows.Forms.Button btnSimplify;
        private System.Windows.Forms.Button btnSetCar;
        private System.Windows.Forms.Button btnPosConfirm;
        private System.Windows.Forms.Button btnIdleMode;
        private System.Windows.Forms.Button btnWorkMode;
        private System.Windows.Forms.Button btnMapMode;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnCursorMode;
        private System.Windows.Forms.Button btnSaveMap;
        private System.Windows.Forms.Button btnSendMap;
        private System.Windows.Forms.Button btnGetMap;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button btnLoadMap;
        private System.Windows.Forms.ComboBox cmbGoalList;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.Label lbAGVStatus;
        private System.Windows.Forms.Button btnResetThread;
        private System.Windows.Forms.Button btnPower;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnErase;
        private System.Windows.Forms.GroupBox gbModeChange;
        private System.Windows.Forms.Button btnSetIP;
        private System.Windows.Forms.Label lbConnectGoal;
    }
}

