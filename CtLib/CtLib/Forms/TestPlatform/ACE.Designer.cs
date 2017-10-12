namespace CtLib.Forms.TestPlatform {
    partial class Test_ACE {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnConnect = new System.Windows.Forms.Button();
            this.chkCtrl = new System.Windows.Forms.CheckBox();
            this.dgvInfo = new System.Windows.Forms.DataGridView();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colInfo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lbSpeed = new System.Windows.Forms.Label();
            this.txtSpeed = new System.Windows.Forms.TextBox();
            this.lbPower = new System.Windows.Forms.Label();
            this.imgPower = new System.Windows.Forms.PictureBox();
            this.gbVar = new System.Windows.Forms.GroupBox();
            this.lbVarValue = new System.Windows.Forms.Label();
            this.lbVarName = new System.Windows.Forms.Label();
            this.btnVarWrite = new System.Windows.Forms.Button();
            this.btnVarRead = new System.Windows.Forms.Button();
            this.txtVarVal = new System.Windows.Forms.TextBox();
            this.txtVarName = new System.Windows.Forms.TextBox();
            this.gbTask = new System.Windows.Forms.GroupBox();
            this.lbProgIdx = new System.Windows.Forms.Label();
            this.cbProgTask = new System.Windows.Forms.ComboBox();
            this.btnProgMoni = new System.Windows.Forms.Button();
            this.btnProgKill = new System.Windows.Forms.Button();
            this.btnProgProc = new System.Windows.Forms.Button();
            this.lbProgName = new System.Windows.Forms.Label();
            this.btnProgExe = new System.Windows.Forms.Button();
            this.txtProgName = new System.Windows.Forms.TextBox();
            this.gbModel = new System.Windows.Forms.GroupBox();
            this.lbLocPath = new System.Windows.Forms.Label();
            this.txtLocPath = new System.Windows.Forms.TextBox();
            this.lbModelPath = new System.Windows.Forms.Label();
            this.btnModelCrt = new System.Windows.Forms.Button();
            this.txtModelPath = new System.Windows.Forms.TextBox();
            this.lbVisPath = new System.Windows.Forms.Label();
            this.btnVision = new System.Windows.Forms.Button();
            this.txtVisionPath = new System.Windows.Forms.TextBox();
            this.gbAwp = new System.Windows.Forms.GroupBox();
            this.btnAwpZero = new System.Windows.Forms.Button();
            this.btnAwpSaveAs = new System.Windows.Forms.Button();
            this.btnAwpSave = new System.Windows.Forms.Button();
            this.lbCtrl = new System.Windows.Forms.Label();
            this.txtCtrl = new System.Windows.Forms.TextBox();
            this.txtRobot = new System.Windows.Forms.TextBox();
            this.lbRobot = new System.Windows.Forms.Label();
            this.btnShowVision = new System.Windows.Forms.Button();
            this.gbIO = new System.Windows.Forms.GroupBox();
            this.btnRdIO = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.txtIO = new System.Windows.Forms.TextBox();
            this.btnPendant = new System.Windows.Forms.Button();
            this.btnCusCmd = new System.Windows.Forms.Button();
            this.btnExConnect = new System.Windows.Forms.Button();
            this.btnVisBud = new System.Windows.Forms.Button();
            this.tabVision = new System.Windows.Forms.TabControl();
            this.pageSingle = new System.Windows.Forms.TabPage();
            this.visionWindow = new CtLib.Module.Adept.CtAceSingleVision();
            this.pageWindow = new System.Windows.Forms.TabPage();
            this.visWindow = new CtLib.Module.Adept.CtAceVisionWindow();
            this.gbMove = new System.Windows.Forms.GroupBox();
            this.txtMoRob = new System.Windows.Forms.TextBox();
            this.lbRob = new System.Windows.Forms.Label();
            this.btnMove = new System.Windows.Forms.Button();
            this.btnRdHere = new System.Windows.Forms.Button();
            this.txtRoll = new System.Windows.Forms.TextBox();
            this.txtYaw = new System.Windows.Forms.TextBox();
            this.txtY = new System.Windows.Forms.TextBox();
            this.txtPitch = new System.Windows.Forms.TextBox();
            this.txtZ = new System.Windows.Forms.TextBox();
            this.txtX = new System.Windows.Forms.TextBox();
            this.lbRoll = new System.Windows.Forms.Label();
            this.lbPitch = new System.Windows.Forms.Label();
            this.lbYaw = new System.Windows.Forms.Label();
            this.lbZ = new System.Windows.Forms.Label();
            this.lbY = new System.Windows.Forms.Label();
            this.lbX = new System.Windows.Forms.Label();
            this.btnMonitor = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPower)).BeginInit();
            this.gbVar.SuspendLayout();
            this.gbTask.SuspendLayout();
            this.gbModel.SuspendLayout();
            this.gbAwp.SuspendLayout();
            this.gbIO.SuspendLayout();
            this.tabVision.SuspendLayout();
            this.pageSingle.SuspendLayout();
            this.pageWindow.SuspendLayout();
            this.gbMove.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnConnect.Location = new System.Drawing.Point(28, 54);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(168, 32);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "連線至 Adept ACE";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // chkCtrl
            // 
            this.chkCtrl.AutoSize = true;
            this.chkCtrl.Checked = true;
            this.chkCtrl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCtrl.Location = new System.Drawing.Point(28, 26);
            this.chkCtrl.Margin = new System.Windows.Forms.Padding(4);
            this.chkCtrl.Name = "chkCtrl";
            this.chkCtrl.Size = new System.Drawing.Size(177, 23);
            this.chkCtrl.TabIndex = 1;
            this.chkCtrl.Text = "含有 SmartController";
            this.chkCtrl.UseVisualStyleBackColor = true;
            // 
            // dgvInfo
            // 
            this.dgvInfo.AllowUserToAddRows = false;
            this.dgvInfo.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
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
            this.dgvInfo.Location = new System.Drawing.Point(0, 603);
            this.dgvInfo.Margin = new System.Windows.Forms.Padding(4);
            this.dgvInfo.Name = "dgvInfo";
            this.dgvInfo.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvInfo.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvInfo.RowHeadersVisible = false;
            this.dgvInfo.RowTemplate.Height = 24;
            this.dgvInfo.Size = new System.Drawing.Size(1497, 146);
            this.dgvInfo.TabIndex = 2;
            // 
            // colTime
            // 
            this.colTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colTime.DefaultCellStyle = dataGridViewCellStyle2;
            this.colTime.HeaderText = "時間";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            this.colTime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTime.ToolTipText = "訊息發出時間";
            this.colTime.Width = 150;
            // 
            // colInfo
            // 
            this.colInfo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.colInfo.DefaultCellStyle = dataGridViewCellStyle3;
            this.colInfo.HeaderText = "訊息";
            this.colInfo.Name = "colInfo";
            this.colInfo.ReadOnly = true;
            this.colInfo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // lbSpeed
            // 
            this.lbSpeed.AutoSize = true;
            this.lbSpeed.Enabled = false;
            this.lbSpeed.Location = new System.Drawing.Point(31, 178);
            this.lbSpeed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbSpeed.Name = "lbSpeed";
            this.lbSpeed.Size = new System.Drawing.Size(113, 19);
            this.lbSpeed.TabIndex = 3;
            this.lbSpeed.Text = "Monitor Speed";
            // 
            // txtSpeed
            // 
            this.txtSpeed.Enabled = false;
            this.txtSpeed.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSpeed.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtSpeed.Location = new System.Drawing.Point(152, 174);
            this.txtSpeed.Margin = new System.Windows.Forms.Padding(4);
            this.txtSpeed.Name = "txtSpeed";
            this.txtSpeed.Size = new System.Drawing.Size(75, 27);
            this.txtSpeed.TabIndex = 4;
            this.txtSpeed.Text = "0";
            this.txtSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtSpeed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSpeed_KeyPress);
            // 
            // lbPower
            // 
            this.lbPower.AutoSize = true;
            this.lbPower.Enabled = false;
            this.lbPower.Location = new System.Drawing.Point(31, 141);
            this.lbPower.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbPower.Name = "lbPower";
            this.lbPower.Size = new System.Drawing.Size(99, 19);
            this.lbPower.TabIndex = 5;
            this.lbPower.Text = "Robot Power";
            // 
            // imgPower
            // 
            this.imgPower.Cursor = System.Windows.Forms.Cursors.Hand;
            this.imgPower.Enabled = false;
            this.imgPower.Image = global::CtLib.Properties.Resources.Grey_Ball;
            this.imgPower.Location = new System.Drawing.Point(152, 133);
            this.imgPower.Margin = new System.Windows.Forms.Padding(4);
            this.imgPower.Name = "imgPower";
            this.imgPower.Size = new System.Drawing.Size(30, 30);
            this.imgPower.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgPower.TabIndex = 6;
            this.imgPower.TabStop = false;
            this.imgPower.Click += new System.EventHandler(this.imgPower_Click);
            // 
            // gbVar
            // 
            this.gbVar.Controls.Add(this.lbVarValue);
            this.gbVar.Controls.Add(this.lbVarName);
            this.gbVar.Controls.Add(this.btnVarWrite);
            this.gbVar.Controls.Add(this.btnVarRead);
            this.gbVar.Controls.Add(this.txtVarVal);
            this.gbVar.Controls.Add(this.txtVarName);
            this.gbVar.Location = new System.Drawing.Point(240, 15);
            this.gbVar.Margin = new System.Windows.Forms.Padding(4);
            this.gbVar.Name = "gbVar";
            this.gbVar.Padding = new System.Windows.Forms.Padding(4);
            this.gbVar.Size = new System.Drawing.Size(339, 161);
            this.gbVar.TabIndex = 7;
            this.gbVar.TabStop = false;
            this.gbVar.Text = "變數控制";
            // 
            // lbVarValue
            // 
            this.lbVarValue.AutoSize = true;
            this.lbVarValue.Enabled = false;
            this.lbVarValue.Location = new System.Drawing.Point(26, 96);
            this.lbVarValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbVarValue.Name = "lbVarValue";
            this.lbVarValue.Size = new System.Drawing.Size(42, 19);
            this.lbVarValue.TabIndex = 7;
            this.lbVarValue.Text = "數值:";
            // 
            // lbVarName
            // 
            this.lbVarName.AutoSize = true;
            this.lbVarName.Enabled = false;
            this.lbVarName.Location = new System.Drawing.Point(26, 33);
            this.lbVarName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbVarName.Name = "lbVarName";
            this.lbVarName.Size = new System.Drawing.Size(72, 19);
            this.lbVarName.TabIndex = 6;
            this.lbVarName.Text = "變數名稱:";
            // 
            // btnVarWrite
            // 
            this.btnVarWrite.Enabled = false;
            this.btnVarWrite.Location = new System.Drawing.Point(231, 74);
            this.btnVarWrite.Margin = new System.Windows.Forms.Padding(4);
            this.btnVarWrite.Name = "btnVarWrite";
            this.btnVarWrite.Size = new System.Drawing.Size(100, 29);
            this.btnVarWrite.TabIndex = 3;
            this.btnVarWrite.Text = "寫入";
            this.btnVarWrite.UseVisualStyleBackColor = true;
            this.btnVarWrite.Click += new System.EventHandler(this.btnVarWrite_Click);
            // 
            // btnVarRead
            // 
            this.btnVarRead.Enabled = false;
            this.btnVarRead.Location = new System.Drawing.Point(231, 42);
            this.btnVarRead.Margin = new System.Windows.Forms.Padding(4);
            this.btnVarRead.Name = "btnVarRead";
            this.btnVarRead.Size = new System.Drawing.Size(100, 29);
            this.btnVarRead.TabIndex = 2;
            this.btnVarRead.Text = "讀取";
            this.btnVarRead.UseVisualStyleBackColor = true;
            this.btnVarRead.Click += new System.EventHandler(this.btnVarRead_Click);
            // 
            // txtVarVal
            // 
            this.txtVarVal.Enabled = false;
            this.txtVarVal.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVarVal.Location = new System.Drawing.Point(26, 118);
            this.txtVarVal.Margin = new System.Windows.Forms.Padding(4);
            this.txtVarVal.Name = "txtVarVal";
            this.txtVarVal.Size = new System.Drawing.Size(304, 27);
            this.txtVarVal.TabIndex = 1;
            // 
            // txtVarName
            // 
            this.txtVarName.Enabled = false;
            this.txtVarName.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVarName.Location = new System.Drawing.Point(26, 55);
            this.txtVarName.Margin = new System.Windows.Forms.Padding(4);
            this.txtVarName.Name = "txtVarName";
            this.txtVarName.Size = new System.Drawing.Size(189, 27);
            this.txtVarName.TabIndex = 0;
            // 
            // gbTask
            // 
            this.gbTask.Controls.Add(this.lbProgIdx);
            this.gbTask.Controls.Add(this.cbProgTask);
            this.gbTask.Controls.Add(this.btnProgMoni);
            this.gbTask.Controls.Add(this.btnProgKill);
            this.gbTask.Controls.Add(this.btnProgProc);
            this.gbTask.Controls.Add(this.lbProgName);
            this.gbTask.Controls.Add(this.btnProgExe);
            this.gbTask.Controls.Add(this.txtProgName);
            this.gbTask.Location = new System.Drawing.Point(240, 184);
            this.gbTask.Margin = new System.Windows.Forms.Padding(4);
            this.gbTask.Name = "gbTask";
            this.gbTask.Padding = new System.Windows.Forms.Padding(4);
            this.gbTask.Size = new System.Drawing.Size(339, 121);
            this.gbTask.TabIndex = 8;
            this.gbTask.TabStop = false;
            this.gbTask.Text = "Task";
            // 
            // lbProgIdx
            // 
            this.lbProgIdx.AutoSize = true;
            this.lbProgIdx.Enabled = false;
            this.lbProgIdx.Location = new System.Drawing.Point(15, 31);
            this.lbProgIdx.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbProgIdx.Name = "lbProgIdx";
            this.lbProgIdx.Size = new System.Drawing.Size(43, 19);
            this.lbProgIdx.TabIndex = 14;
            this.lbProgIdx.Text = "Task:";
            // 
            // cbProgTask
            // 
            this.cbProgTask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProgTask.Enabled = false;
            this.cbProgTask.FormattingEnabled = true;
            this.cbProgTask.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24"});
            this.cbProgTask.Location = new System.Drawing.Point(19, 51);
            this.cbProgTask.Margin = new System.Windows.Forms.Padding(4);
            this.cbProgTask.Name = "cbProgTask";
            this.cbProgTask.Size = new System.Drawing.Size(49, 27);
            this.cbProgTask.TabIndex = 13;
            this.cbProgTask.SelectedValueChanged += new System.EventHandler(this.cbProgTask_SelectedValueChanged);
            // 
            // btnProgMoni
            // 
            this.btnProgMoni.Enabled = false;
            this.btnProgMoni.Location = new System.Drawing.Point(231, 48);
            this.btnProgMoni.Margin = new System.Windows.Forms.Padding(4);
            this.btnProgMoni.Name = "btnProgMoni";
            this.btnProgMoni.Size = new System.Drawing.Size(100, 29);
            this.btnProgMoni.TabIndex = 12;
            this.btnProgMoni.Text = "加入監控";
            this.btnProgMoni.UseVisualStyleBackColor = true;
            this.btnProgMoni.Click += new System.EventHandler(this.btnProgMoni_Click);
            // 
            // btnProgKill
            // 
            this.btnProgKill.Enabled = false;
            this.btnProgKill.Location = new System.Drawing.Point(231, 82);
            this.btnProgKill.Margin = new System.Windows.Forms.Padding(4);
            this.btnProgKill.Name = "btnProgKill";
            this.btnProgKill.Size = new System.Drawing.Size(100, 29);
            this.btnProgKill.TabIndex = 11;
            this.btnProgKill.Text = "移除";
            this.btnProgKill.UseVisualStyleBackColor = true;
            this.btnProgKill.Click += new System.EventHandler(this.btnProgKill_Click);
            // 
            // btnProgProc
            // 
            this.btnProgProc.Enabled = false;
            this.btnProgProc.Location = new System.Drawing.Point(125, 82);
            this.btnProgProc.Margin = new System.Windows.Forms.Padding(4);
            this.btnProgProc.Name = "btnProgProc";
            this.btnProgProc.Size = new System.Drawing.Size(100, 29);
            this.btnProgProc.TabIndex = 10;
            this.btnProgProc.Text = "下一步";
            this.btnProgProc.UseVisualStyleBackColor = true;
            this.btnProgProc.Click += new System.EventHandler(this.btnProgProc_Click);
            // 
            // lbProgName
            // 
            this.lbProgName.AutoSize = true;
            this.lbProgName.Enabled = false;
            this.lbProgName.Location = new System.Drawing.Point(75, 30);
            this.lbProgName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbProgName.Name = "lbProgName";
            this.lbProgName.Size = new System.Drawing.Size(67, 19);
            this.lbProgName.TabIndex = 9;
            this.lbProgName.Text = "V+ 名稱:";
            // 
            // btnProgExe
            // 
            this.btnProgExe.Enabled = false;
            this.btnProgExe.Location = new System.Drawing.Point(17, 82);
            this.btnProgExe.Margin = new System.Windows.Forms.Padding(4);
            this.btnProgExe.Name = "btnProgExe";
            this.btnProgExe.Size = new System.Drawing.Size(100, 29);
            this.btnProgExe.TabIndex = 8;
            this.btnProgExe.Text = "執行";
            this.btnProgExe.UseVisualStyleBackColor = true;
            this.btnProgExe.Click += new System.EventHandler(this.btnProgExe_Click);
            // 
            // txtProgName
            // 
            this.txtProgName.Enabled = false;
            this.txtProgName.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProgName.Location = new System.Drawing.Point(77, 49);
            this.txtProgName.Margin = new System.Windows.Forms.Padding(4);
            this.txtProgName.Name = "txtProgName";
            this.txtProgName.Size = new System.Drawing.Size(147, 27);
            this.txtProgName.TabIndex = 7;
            this.txtProgName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // gbModel
            // 
            this.gbModel.Controls.Add(this.lbLocPath);
            this.gbModel.Controls.Add(this.txtLocPath);
            this.gbModel.Controls.Add(this.lbModelPath);
            this.gbModel.Controls.Add(this.btnModelCrt);
            this.gbModel.Controls.Add(this.txtModelPath);
            this.gbModel.Location = new System.Drawing.Point(240, 312);
            this.gbModel.Margin = new System.Windows.Forms.Padding(4);
            this.gbModel.Name = "gbModel";
            this.gbModel.Padding = new System.Windows.Forms.Padding(4);
            this.gbModel.Size = new System.Drawing.Size(339, 171);
            this.gbModel.TabIndex = 9;
            this.gbModel.TabStop = false;
            this.gbModel.Text = "Model Editor";
            // 
            // lbLocPath
            // 
            this.lbLocPath.AutoSize = true;
            this.lbLocPath.Enabled = false;
            this.lbLocPath.Location = new System.Drawing.Point(9, 24);
            this.lbLocPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbLocPath.Name = "lbLocPath";
            this.lbLocPath.Size = new System.Drawing.Size(98, 19);
            this.lbLocPath.TabIndex = 11;
            this.lbLocPath.Text = "Locator 路徑:";
            // 
            // txtLocPath
            // 
            this.txtLocPath.Enabled = false;
            this.txtLocPath.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLocPath.Location = new System.Drawing.Point(8, 46);
            this.txtLocPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtLocPath.Name = "txtLocPath";
            this.txtLocPath.Size = new System.Drawing.Size(321, 27);
            this.txtLocPath.TabIndex = 10;
            // 
            // lbModelPath
            // 
            this.lbModelPath.AutoSize = true;
            this.lbModelPath.Enabled = false;
            this.lbModelPath.Location = new System.Drawing.Point(9, 78);
            this.lbModelPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbModelPath.Name = "lbModelPath";
            this.lbModelPath.Size = new System.Drawing.Size(91, 19);
            this.lbModelPath.TabIndex = 9;
            this.lbModelPath.Text = "Model 路徑:";
            // 
            // btnModelCrt
            // 
            this.btnModelCrt.Enabled = false;
            this.btnModelCrt.Location = new System.Drawing.Point(231, 133);
            this.btnModelCrt.Margin = new System.Windows.Forms.Padding(4);
            this.btnModelCrt.Name = "btnModelCrt";
            this.btnModelCrt.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnModelCrt.Size = new System.Drawing.Size(100, 29);
            this.btnModelCrt.TabIndex = 8;
            this.btnModelCrt.Text = "編輯";
            this.btnModelCrt.UseVisualStyleBackColor = true;
            this.btnModelCrt.Click += new System.EventHandler(this.btnModelCrt_Click);
            // 
            // txtModelPath
            // 
            this.txtModelPath.Enabled = false;
            this.txtModelPath.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtModelPath.Location = new System.Drawing.Point(8, 100);
            this.txtModelPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtModelPath.Name = "txtModelPath";
            this.txtModelPath.Size = new System.Drawing.Size(321, 27);
            this.txtModelPath.TabIndex = 7;
            // 
            // lbVisPath
            // 
            this.lbVisPath.AutoSize = true;
            this.lbVisPath.Enabled = false;
            this.lbVisPath.Location = new System.Drawing.Point(7, 10);
            this.lbVisPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbVisPath.Name = "lbVisPath";
            this.lbVisPath.Size = new System.Drawing.Size(76, 19);
            this.lbVisPath.TabIndex = 12;
            this.lbVisPath.Text = "Tool 路徑:";
            // 
            // btnVision
            // 
            this.btnVision.Enabled = false;
            this.btnVision.Location = new System.Drawing.Point(483, 5);
            this.btnVision.Margin = new System.Windows.Forms.Padding(4);
            this.btnVision.Name = "btnVision";
            this.btnVision.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnVision.Size = new System.Drawing.Size(100, 29);
            this.btnVision.TabIndex = 11;
            this.btnVision.Text = "開啟";
            this.btnVision.UseVisualStyleBackColor = true;
            this.btnVision.Click += new System.EventHandler(this.btnVision_Click);
            // 
            // txtVisionPath
            // 
            this.txtVisionPath.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.txtVisionPath.Enabled = false;
            this.txtVisionPath.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVisionPath.Location = new System.Drawing.Point(87, 6);
            this.txtVisionPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtVisionPath.Name = "txtVisionPath";
            this.txtVisionPath.Size = new System.Drawing.Size(388, 27);
            this.txtVisionPath.TabIndex = 10;
            // 
            // gbAwp
            // 
            this.gbAwp.Controls.Add(this.btnAwpZero);
            this.gbAwp.Controls.Add(this.btnAwpSaveAs);
            this.gbAwp.Controls.Add(this.btnAwpSave);
            this.gbAwp.Location = new System.Drawing.Point(240, 498);
            this.gbAwp.Margin = new System.Windows.Forms.Padding(4);
            this.gbAwp.Name = "gbAwp";
            this.gbAwp.Padding = new System.Windows.Forms.Padding(4);
            this.gbAwp.Size = new System.Drawing.Size(339, 88);
            this.gbAwp.TabIndex = 11;
            this.gbAwp.TabStop = false;
            this.gbAwp.Text = "Workspace";
            // 
            // btnAwpZero
            // 
            this.btnAwpZero.Enabled = false;
            this.btnAwpZero.Location = new System.Drawing.Point(231, 38);
            this.btnAwpZero.Margin = new System.Windows.Forms.Padding(4);
            this.btnAwpZero.Name = "btnAwpZero";
            this.btnAwpZero.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnAwpZero.Size = new System.Drawing.Size(100, 29);
            this.btnAwpZero.TabIndex = 11;
            this.btnAwpZero.Text = "ZeroMemory";
            this.btnAwpZero.UseVisualStyleBackColor = true;
            this.btnAwpZero.Click += new System.EventHandler(this.btnAwpZero_Click);
            // 
            // btnAwpSaveAs
            // 
            this.btnAwpSaveAs.Enabled = false;
            this.btnAwpSaveAs.Location = new System.Drawing.Point(125, 38);
            this.btnAwpSaveAs.Margin = new System.Windows.Forms.Padding(4);
            this.btnAwpSaveAs.Name = "btnAwpSaveAs";
            this.btnAwpSaveAs.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnAwpSaveAs.Size = new System.Drawing.Size(100, 29);
            this.btnAwpSaveAs.TabIndex = 10;
            this.btnAwpSaveAs.Text = "另存新檔";
            this.btnAwpSaveAs.UseVisualStyleBackColor = true;
            this.btnAwpSaveAs.Click += new System.EventHandler(this.btnAwpSaveAs_Click);
            // 
            // btnAwpSave
            // 
            this.btnAwpSave.Enabled = false;
            this.btnAwpSave.Location = new System.Drawing.Point(19, 38);
            this.btnAwpSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnAwpSave.Name = "btnAwpSave";
            this.btnAwpSave.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnAwpSave.Size = new System.Drawing.Size(100, 29);
            this.btnAwpSave.TabIndex = 9;
            this.btnAwpSave.Text = "存檔";
            this.btnAwpSave.UseVisualStyleBackColor = true;
            this.btnAwpSave.Click += new System.EventHandler(this.btnAwpSave_Click);
            // 
            // lbCtrl
            // 
            this.lbCtrl.AutoSize = true;
            this.lbCtrl.Enabled = false;
            this.lbCtrl.Location = new System.Drawing.Point(32, 239);
            this.lbCtrl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbCtrl.Name = "lbCtrl";
            this.lbCtrl.Size = new System.Drawing.Size(102, 19);
            this.lbCtrl.TabIndex = 12;
            this.lbCtrl.Text = "已連結控制器:";
            // 
            // txtCtrl
            // 
            this.txtCtrl.Enabled = false;
            this.txtCtrl.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCtrl.Location = new System.Drawing.Point(35, 258);
            this.txtCtrl.Margin = new System.Windows.Forms.Padding(4);
            this.txtCtrl.Name = "txtCtrl";
            this.txtCtrl.ReadOnly = true;
            this.txtCtrl.Size = new System.Drawing.Size(192, 27);
            this.txtCtrl.TabIndex = 13;
            // 
            // txtRobot
            // 
            this.txtRobot.Enabled = false;
            this.txtRobot.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRobot.Location = new System.Drawing.Point(33, 322);
            this.txtRobot.Margin = new System.Windows.Forms.Padding(4);
            this.txtRobot.Name = "txtRobot";
            this.txtRobot.ReadOnly = true;
            this.txtRobot.Size = new System.Drawing.Size(194, 27);
            this.txtRobot.TabIndex = 15;
            // 
            // lbRobot
            // 
            this.lbRobot.AutoSize = true;
            this.lbRobot.Enabled = false;
            this.lbRobot.Location = new System.Drawing.Point(31, 304);
            this.lbRobot.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbRobot.Name = "lbRobot";
            this.lbRobot.Size = new System.Drawing.Size(99, 19);
            this.lbRobot.TabIndex = 14;
            this.lbRobot.Text = "已連結Robot:";
            // 
            // btnShowVision
            // 
            this.btnShowVision.Enabled = false;
            this.btnShowVision.Location = new System.Drawing.Point(486, 3);
            this.btnShowVision.Margin = new System.Windows.Forms.Padding(4);
            this.btnShowVision.Name = "btnShowVision";
            this.btnShowVision.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnShowVision.Size = new System.Drawing.Size(100, 29);
            this.btnShowVision.TabIndex = 18;
            this.btnShowVision.Text = "顯示";
            this.btnShowVision.UseVisualStyleBackColor = true;
            this.btnShowVision.Click += new System.EventHandler(this.btnShowVision_Click);
            // 
            // gbIO
            // 
            this.gbIO.Controls.Add(this.btnRdIO);
            this.gbIO.Controls.Add(this.btnClose);
            this.gbIO.Controls.Add(this.btnOpen);
            this.gbIO.Controls.Add(this.txtIO);
            this.gbIO.Location = new System.Drawing.Point(586, 15);
            this.gbIO.Margin = new System.Windows.Forms.Padding(4);
            this.gbIO.Name = "gbIO";
            this.gbIO.Padding = new System.Windows.Forms.Padding(4);
            this.gbIO.Size = new System.Drawing.Size(299, 103);
            this.gbIO.TabIndex = 18;
            this.gbIO.TabStop = false;
            this.gbIO.Text = "I/O 控制";
            // 
            // btnRdIO
            // 
            this.btnRdIO.Enabled = false;
            this.btnRdIO.Location = new System.Drawing.Point(191, 61);
            this.btnRdIO.Margin = new System.Windows.Forms.Padding(4);
            this.btnRdIO.Name = "btnRdIO";
            this.btnRdIO.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnRdIO.Size = new System.Drawing.Size(47, 29);
            this.btnRdIO.TabIndex = 19;
            this.btnRdIO.Text = "讀";
            this.btnRdIO.UseVisualStyleBackColor = true;
            this.btnRdIO.Click += new System.EventHandler(this.btnRdIO_Click);
            // 
            // btnClose
            // 
            this.btnClose.Enabled = false;
            this.btnClose.Location = new System.Drawing.Point(222, 24);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnClose.Size = new System.Drawing.Size(47, 29);
            this.btnClose.TabIndex = 18;
            this.btnClose.Text = "OFF";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Enabled = false;
            this.btnOpen.Location = new System.Drawing.Point(167, 24);
            this.btnOpen.Margin = new System.Windows.Forms.Padding(4);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnOpen.Size = new System.Drawing.Size(47, 29);
            this.btnOpen.TabIndex = 17;
            this.btnOpen.Text = "ON";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // txtIO
            // 
            this.txtIO.Enabled = false;
            this.txtIO.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIO.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtIO.Location = new System.Drawing.Point(35, 44);
            this.txtIO.Margin = new System.Windows.Forms.Padding(4);
            this.txtIO.MaxLength = 3;
            this.txtIO.Name = "txtIO";
            this.txtIO.Size = new System.Drawing.Size(104, 27);
            this.txtIO.TabIndex = 16;
            this.txtIO.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnPendant
            // 
            this.btnPendant.Enabled = false;
            this.btnPendant.Location = new System.Drawing.Point(191, 155);
            this.btnPendant.Margin = new System.Windows.Forms.Padding(4);
            this.btnPendant.Name = "btnPendant";
            this.btnPendant.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnPendant.Size = new System.Drawing.Size(77, 29);
            this.btnPendant.TabIndex = 21;
            this.btnPendant.Text = "Pendant";
            this.btnPendant.UseVisualStyleBackColor = true;
            this.btnPendant.Click += new System.EventHandler(this.btnPendant_Click);
            // 
            // btnCusCmd
            // 
            this.btnCusCmd.Enabled = false;
            this.btnCusCmd.Location = new System.Drawing.Point(36, 535);
            this.btnCusCmd.Margin = new System.Windows.Forms.Padding(4);
            this.btnCusCmd.Name = "btnCusCmd";
            this.btnCusCmd.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnCusCmd.Size = new System.Drawing.Size(134, 32);
            this.btnCusCmd.TabIndex = 22;
            this.btnCusCmd.Text = "Send String";
            this.btnCusCmd.UseVisualStyleBackColor = true;
            this.btnCusCmd.Click += new System.EventHandler(this.btnCusCmd_Click);
            // 
            // btnExConnect
            // 
            this.btnExConnect.Enabled = false;
            this.btnExConnect.Location = new System.Drawing.Point(28, 89);
            this.btnExConnect.Margin = new System.Windows.Forms.Padding(4);
            this.btnExConnect.Name = "btnExConnect";
            this.btnExConnect.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnExConnect.Size = new System.Drawing.Size(168, 29);
            this.btnExConnect.TabIndex = 23;
            this.btnExConnect.Text = "Controller State";
            this.btnExConnect.UseVisualStyleBackColor = true;
            this.btnExConnect.Click += new System.EventHandler(this.btnExConnect_Click);
            // 
            // btnVisBud
            // 
            this.btnVisBud.Enabled = false;
            this.btnVisBud.Location = new System.Drawing.Point(36, 498);
            this.btnVisBud.Margin = new System.Windows.Forms.Padding(4);
            this.btnVisBud.Name = "btnVisBud";
            this.btnVisBud.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnVisBud.Size = new System.Drawing.Size(134, 32);
            this.btnVisBud.TabIndex = 24;
            this.btnVisBud.Text = "Vision Builder";
            this.btnVisBud.UseVisualStyleBackColor = true;
            this.btnVisBud.Click += new System.EventHandler(this.btnVisBud_Click);
            // 
            // tabVision
            // 
            this.tabVision.Controls.Add(this.pageSingle);
            this.tabVision.Controls.Add(this.pageWindow);
            this.tabVision.Location = new System.Drawing.Point(892, 15);
            this.tabVision.Name = "tabVision";
            this.tabVision.SelectedIndex = 0;
            this.tabVision.Size = new System.Drawing.Size(597, 571);
            this.tabVision.TabIndex = 25;
            // 
            // pageSingle
            // 
            this.pageSingle.Controls.Add(this.visionWindow);
            this.pageSingle.Controls.Add(this.lbVisPath);
            this.pageSingle.Controls.Add(this.txtVisionPath);
            this.pageSingle.Controls.Add(this.btnVision);
            this.pageSingle.Location = new System.Drawing.Point(4, 28);
            this.pageSingle.Name = "pageSingle";
            this.pageSingle.Padding = new System.Windows.Forms.Padding(3);
            this.pageSingle.Size = new System.Drawing.Size(589, 539);
            this.pageSingle.TabIndex = 0;
            this.pageSingle.Text = "Single";
            this.pageSingle.UseVisualStyleBackColor = true;
            // 
            // visionWindow
            // 
            this.visionWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.visionWindow.Location = new System.Drawing.Point(0, 38);
            this.visionWindow.Margin = new System.Windows.Forms.Padding(10, 16, 10, 16);
            this.visionWindow.Name = "visionWindow";
            this.visionWindow.Size = new System.Drawing.Size(589, 483);
            this.visionWindow.TabIndex = 0;
            // 
            // pageWindow
            // 
            this.pageWindow.Controls.Add(this.btnShowVision);
            this.pageWindow.Controls.Add(this.visWindow);
            this.pageWindow.Location = new System.Drawing.Point(4, 25);
            this.pageWindow.Name = "pageWindow";
            this.pageWindow.Padding = new System.Windows.Forms.Padding(3);
            this.pageWindow.Size = new System.Drawing.Size(589, 542);
            this.pageWindow.TabIndex = 1;
            this.pageWindow.Text = "Vision Window";
            this.pageWindow.UseVisualStyleBackColor = true;
            // 
            // visWindow
            // 
            this.visWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.visWindow.BackColor = System.Drawing.Color.Transparent;
            this.visWindow.Location = new System.Drawing.Point(0, 34);
            this.visWindow.Margin = new System.Windows.Forms.Padding(10, 16, 10, 16);
            this.visWindow.Name = "visWindow";
            this.visWindow.Size = new System.Drawing.Size(589, 497);
            this.visWindow.TabIndex = 16;
            // 
            // gbMove
            // 
            this.gbMove.Controls.Add(this.txtMoRob);
            this.gbMove.Controls.Add(this.lbRob);
            this.gbMove.Controls.Add(this.btnMove);
            this.gbMove.Controls.Add(this.btnRdHere);
            this.gbMove.Controls.Add(this.txtRoll);
            this.gbMove.Controls.Add(this.txtYaw);
            this.gbMove.Controls.Add(this.txtY);
            this.gbMove.Controls.Add(this.btnPendant);
            this.gbMove.Controls.Add(this.txtPitch);
            this.gbMove.Controls.Add(this.txtZ);
            this.gbMove.Controls.Add(this.txtX);
            this.gbMove.Controls.Add(this.lbRoll);
            this.gbMove.Controls.Add(this.lbPitch);
            this.gbMove.Controls.Add(this.lbYaw);
            this.gbMove.Controls.Add(this.lbZ);
            this.gbMove.Controls.Add(this.lbY);
            this.gbMove.Controls.Add(this.lbX);
            this.gbMove.Location = new System.Drawing.Point(586, 125);
            this.gbMove.Name = "gbMove";
            this.gbMove.Size = new System.Drawing.Size(300, 201);
            this.gbMove.TabIndex = 26;
            this.gbMove.TabStop = false;
            this.gbMove.Text = "Motion";
            // 
            // txtMoRob
            // 
            this.txtMoRob.Enabled = false;
            this.txtMoRob.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMoRob.Location = new System.Drawing.Point(140, 120);
            this.txtMoRob.Margin = new System.Windows.Forms.Padding(4);
            this.txtMoRob.Name = "txtMoRob";
            this.txtMoRob.Size = new System.Drawing.Size(37, 27);
            this.txtMoRob.TabIndex = 23;
            this.txtMoRob.Text = "1";
            this.txtMoRob.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbRob
            // 
            this.lbRob.AutoSize = true;
            this.lbRob.Location = new System.Drawing.Point(59, 125);
            this.lbRob.Name = "lbRob";
            this.lbRob.Size = new System.Drawing.Size(79, 19);
            this.lbRob.TabIndex = 22;
            this.lbRob.Text = "Robot No.";
            // 
            // btnMove
            // 
            this.btnMove.Enabled = false;
            this.btnMove.Location = new System.Drawing.Point(191, 118);
            this.btnMove.Margin = new System.Windows.Forms.Padding(4);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(77, 29);
            this.btnMove.TabIndex = 13;
            this.btnMove.Text = "移動";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // btnRdHere
            // 
            this.btnRdHere.Enabled = false;
            this.btnRdHere.Location = new System.Drawing.Point(62, 155);
            this.btnRdHere.Margin = new System.Windows.Forms.Padding(4);
            this.btnRdHere.Name = "btnRdHere";
            this.btnRdHere.Size = new System.Drawing.Size(115, 29);
            this.btnRdHere.TabIndex = 12;
            this.btnRdHere.Text = "讀取當前座標";
            this.btnRdHere.UseVisualStyleBackColor = true;
            this.btnRdHere.Click += new System.EventHandler(this.btnRdHere_Click);
            // 
            // txtRoll
            // 
            this.txtRoll.Enabled = false;
            this.txtRoll.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRoll.Location = new System.Drawing.Point(191, 81);
            this.txtRoll.Margin = new System.Windows.Forms.Padding(4);
            this.txtRoll.Name = "txtRoll";
            this.txtRoll.Size = new System.Drawing.Size(77, 27);
            this.txtRoll.TabIndex = 11;
            this.txtRoll.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtYaw
            // 
            this.txtYaw.Enabled = false;
            this.txtYaw.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtYaw.Location = new System.Drawing.Point(191, 51);
            this.txtYaw.Margin = new System.Windows.Forms.Padding(4);
            this.txtYaw.Name = "txtYaw";
            this.txtYaw.Size = new System.Drawing.Size(77, 27);
            this.txtYaw.TabIndex = 10;
            this.txtYaw.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtY
            // 
            this.txtY.Enabled = false;
            this.txtY.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtY.Location = new System.Drawing.Point(191, 21);
            this.txtY.Margin = new System.Windows.Forms.Padding(4);
            this.txtY.Name = "txtY";
            this.txtY.Size = new System.Drawing.Size(77, 27);
            this.txtY.TabIndex = 9;
            this.txtY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtPitch
            // 
            this.txtPitch.Enabled = false;
            this.txtPitch.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPitch.Location = new System.Drawing.Point(62, 81);
            this.txtPitch.Margin = new System.Windows.Forms.Padding(4);
            this.txtPitch.Name = "txtPitch";
            this.txtPitch.Size = new System.Drawing.Size(77, 27);
            this.txtPitch.TabIndex = 8;
            this.txtPitch.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtZ
            // 
            this.txtZ.Enabled = false;
            this.txtZ.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtZ.Location = new System.Drawing.Point(62, 51);
            this.txtZ.Margin = new System.Windows.Forms.Padding(4);
            this.txtZ.Name = "txtZ";
            this.txtZ.Size = new System.Drawing.Size(77, 27);
            this.txtZ.TabIndex = 7;
            this.txtZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtX
            // 
            this.txtX.Enabled = false;
            this.txtX.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtX.Location = new System.Drawing.Point(62, 21);
            this.txtX.Margin = new System.Windows.Forms.Padding(4);
            this.txtX.Name = "txtX";
            this.txtX.Size = new System.Drawing.Size(77, 27);
            this.txtX.TabIndex = 6;
            this.txtX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbRoll
            // 
            this.lbRoll.AutoSize = true;
            this.lbRoll.Location = new System.Drawing.Point(155, 88);
            this.lbRoll.Name = "lbRoll";
            this.lbRoll.Size = new System.Drawing.Size(36, 19);
            this.lbRoll.TabIndex = 5;
            this.lbRoll.Text = "Roll";
            // 
            // lbPitch
            // 
            this.lbPitch.AutoSize = true;
            this.lbPitch.Location = new System.Drawing.Point(21, 88);
            this.lbPitch.Name = "lbPitch";
            this.lbPitch.Size = new System.Drawing.Size(43, 19);
            this.lbPitch.TabIndex = 4;
            this.lbPitch.Text = "Pitch";
            // 
            // lbYaw
            // 
            this.lbYaw.AutoSize = true;
            this.lbYaw.Location = new System.Drawing.Point(155, 55);
            this.lbYaw.Name = "lbYaw";
            this.lbYaw.Size = new System.Drawing.Size(38, 19);
            this.lbYaw.TabIndex = 3;
            this.lbYaw.Text = "Yaw";
            // 
            // lbZ
            // 
            this.lbZ.AutoSize = true;
            this.lbZ.Location = new System.Drawing.Point(21, 55);
            this.lbZ.Name = "lbZ";
            this.lbZ.Size = new System.Drawing.Size(18, 19);
            this.lbZ.TabIndex = 2;
            this.lbZ.Text = "Z";
            // 
            // lbY
            // 
            this.lbY.AutoSize = true;
            this.lbY.Location = new System.Drawing.Point(155, 27);
            this.lbY.Name = "lbY";
            this.lbY.Size = new System.Drawing.Size(18, 19);
            this.lbY.TabIndex = 1;
            this.lbY.Text = "Y";
            // 
            // lbX
            // 
            this.lbX.AutoSize = true;
            this.lbX.Location = new System.Drawing.Point(21, 27);
            this.lbX.Name = "lbX";
            this.lbX.Size = new System.Drawing.Size(19, 19);
            this.lbX.TabIndex = 0;
            this.lbX.Text = "X";
            // 
            // btnMonitor
            // 
            this.btnMonitor.Enabled = false;
            this.btnMonitor.Location = new System.Drawing.Point(778, 317);
            this.btnMonitor.Margin = new System.Windows.Forms.Padding(4);
            this.btnMonitor.Name = "btnMonitor";
            this.btnMonitor.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnMonitor.Size = new System.Drawing.Size(77, 29);
            this.btnMonitor.TabIndex = 27;
            this.btnMonitor.Text = "Monitor";
            this.btnMonitor.UseVisualStyleBackColor = true;
            this.btnMonitor.Click += new System.EventHandler(this.btnMonitor_Click);
            // 
            // Test_ACE
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1497, 749);
            this.Controls.Add(this.btnMonitor);
            this.Controls.Add(this.gbMove);
            this.Controls.Add(this.tabVision);
            this.Controls.Add(this.btnVisBud);
            this.Controls.Add(this.btnExConnect);
            this.Controls.Add(this.btnCusCmd);
            this.Controls.Add(this.gbIO);
            this.Controls.Add(this.txtRobot);
            this.Controls.Add(this.lbRobot);
            this.Controls.Add(this.txtCtrl);
            this.Controls.Add(this.lbCtrl);
            this.Controls.Add(this.gbAwp);
            this.Controls.Add(this.gbModel);
            this.Controls.Add(this.gbTask);
            this.Controls.Add(this.gbVar);
            this.Controls.Add(this.imgPower);
            this.Controls.Add(this.lbPower);
            this.Controls.Add(this.txtSpeed);
            this.Controls.Add(this.lbSpeed);
            this.Controls.Add(this.dgvInfo);
            this.Controls.Add(this.chkCtrl);
            this.Controls.Add(this.btnConnect);
            this.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Test_ACE";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Adept ACE Test Platform";
            this.Load += new System.EventHandler(this.Test_ACE_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPower)).EndInit();
            this.gbVar.ResumeLayout(false);
            this.gbVar.PerformLayout();
            this.gbTask.ResumeLayout(false);
            this.gbTask.PerformLayout();
            this.gbModel.ResumeLayout(false);
            this.gbModel.PerformLayout();
            this.gbAwp.ResumeLayout(false);
            this.gbIO.ResumeLayout(false);
            this.gbIO.PerformLayout();
            this.tabVision.ResumeLayout(false);
            this.pageSingle.ResumeLayout(false);
            this.pageSingle.PerformLayout();
            this.pageWindow.ResumeLayout(false);
            this.gbMove.ResumeLayout(false);
            this.gbMove.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.CheckBox chkCtrl;
        private System.Windows.Forms.DataGridView dgvInfo;
        private System.Windows.Forms.Label lbSpeed;
        private System.Windows.Forms.TextBox txtSpeed;
        private System.Windows.Forms.Label lbPower;
        private System.Windows.Forms.PictureBox imgPower;
        private System.Windows.Forms.GroupBox gbVar;
        private System.Windows.Forms.Label lbVarValue;
        private System.Windows.Forms.Label lbVarName;
        private System.Windows.Forms.Button btnVarWrite;
        private System.Windows.Forms.Button btnVarRead;
        private System.Windows.Forms.TextBox txtVarVal;
        private System.Windows.Forms.TextBox txtVarName;
        private System.Windows.Forms.GroupBox gbTask;
        private System.Windows.Forms.Button btnProgMoni;
        private System.Windows.Forms.Button btnProgKill;
        private System.Windows.Forms.Button btnProgProc;
        private System.Windows.Forms.Label lbProgName;
        private System.Windows.Forms.Button btnProgExe;
        private System.Windows.Forms.TextBox txtProgName;
        private System.Windows.Forms.Label lbProgIdx;
        private System.Windows.Forms.ComboBox cbProgTask;
        private System.Windows.Forms.GroupBox gbModel;
        private System.Windows.Forms.Label lbModelPath;
        private System.Windows.Forms.Button btnModelCrt;
        private System.Windows.Forms.TextBox txtModelPath;
        private System.Windows.Forms.Label lbLocPath;
        private System.Windows.Forms.TextBox txtLocPath;
        private System.Windows.Forms.Label lbVisPath;
        private System.Windows.Forms.Button btnVision;
        private System.Windows.Forms.TextBox txtVisionPath;
        private Module.Adept.CtAceSingleVision visionWindow;
        private System.Windows.Forms.GroupBox gbAwp;
        private System.Windows.Forms.Button btnAwpZero;
        private System.Windows.Forms.Button btnAwpSaveAs;
        private System.Windows.Forms.Button btnAwpSave;
        private System.Windows.Forms.Label lbCtrl;
        private System.Windows.Forms.TextBox txtCtrl;
        private System.Windows.Forms.TextBox txtRobot;
        private System.Windows.Forms.Label lbRobot;
        private Module.Adept.CtAceVisionWindow visWindow;
        private System.Windows.Forms.Button btnShowVision;
        private System.Windows.Forms.GroupBox gbIO;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TextBox txtIO;
        private System.Windows.Forms.Button btnPendant;
        private System.Windows.Forms.Button btnCusCmd;
		private System.Windows.Forms.Button btnExConnect;
		private System.Windows.Forms.Button btnVisBud;
		private System.Windows.Forms.TabControl tabVision;
		private System.Windows.Forms.TabPage pageSingle;
		private System.Windows.Forms.TabPage pageWindow;
		private System.Windows.Forms.GroupBox gbMove;
		private System.Windows.Forms.Label lbRoll;
		private System.Windows.Forms.Label lbPitch;
		private System.Windows.Forms.Label lbYaw;
		private System.Windows.Forms.Label lbZ;
		private System.Windows.Forms.Label lbY;
		private System.Windows.Forms.Label lbX;
		private System.Windows.Forms.TextBox txtRoll;
		private System.Windows.Forms.TextBox txtYaw;
		private System.Windows.Forms.TextBox txtY;
		private System.Windows.Forms.TextBox txtPitch;
		private System.Windows.Forms.TextBox txtZ;
		private System.Windows.Forms.TextBox txtX;
		private System.Windows.Forms.Button btnMove;
		private System.Windows.Forms.Button btnRdHere;
		private System.Windows.Forms.Button btnRdIO;
		private System.Windows.Forms.TextBox txtMoRob;
		private System.Windows.Forms.Label lbRob;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
		private System.Windows.Forms.DataGridViewTextBoxColumn colInfo;
        private System.Windows.Forms.Button btnMonitor;
    }
}