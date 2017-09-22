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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.label1 = new System.Windows.Forms.Label();
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
            this.gbVision = new System.Windows.Forms.GroupBox();
            this.cbVisTool = new System.Windows.Forms.ComboBox();
            this.lbVisPath = new System.Windows.Forms.Label();
            this.btnVision = new System.Windows.Forms.Button();
            this.txtVisionPath = new System.Windows.Forms.TextBox();
            this.visionWindow = new CtLib.Module.Adept.CtAceSingleVision();
            this.gbAwp = new System.Windows.Forms.GroupBox();
            this.btnAwpZero = new System.Windows.Forms.Button();
            this.btnAwpSaveAs = new System.Windows.Forms.Button();
            this.btnAwpSave = new System.Windows.Forms.Button();
            this.lbCtrl = new System.Windows.Forms.Label();
            this.txtCtrl = new System.Windows.Forms.TextBox();
            this.txtRobot = new System.Windows.Forms.TextBox();
            this.lbRobot = new System.Windows.Forms.Label();
            this.gbVisionWindow = new System.Windows.Forms.GroupBox();
            this.btnShowVision = new System.Windows.Forms.Button();
            this.visWindow = new CtLib.Module.Adept.CtAceVisionWindow();
            this.gbIO = new System.Windows.Forms.GroupBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.txtIO = new System.Windows.Forms.TextBox();
            this.btn90P = new System.Windows.Forms.Button();
            this.btn90N = new System.Windows.Forms.Button();
            this.btnPendant = new System.Windows.Forms.Button();
            this.btnCusCmd = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPower)).BeginInit();
            this.gbVar.SuspendLayout();
            this.gbTask.SuspendLayout();
            this.gbModel.SuspendLayout();
            this.gbVision.SuspendLayout();
            this.gbAwp.SuspendLayout();
            this.gbVisionWindow.SuspendLayout();
            this.gbIO.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnConnect.Location = new System.Drawing.Point(21, 43);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(126, 36);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "連線";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // chkCtrl
            // 
            this.chkCtrl.AutoSize = true;
            this.chkCtrl.Checked = true;
            this.chkCtrl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCtrl.Location = new System.Drawing.Point(21, 21);
            this.chkCtrl.Name = "chkCtrl";
            this.chkCtrl.Size = new System.Drawing.Size(126, 16);
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
            dataGridViewCellStyle1.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvInfo.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTime,
            this.colInfo});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvInfo.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgvInfo.Location = new System.Drawing.Point(0, 482);
            this.dgvInfo.Name = "dgvInfo";
            this.dgvInfo.ReadOnly = true;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvInfo.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvInfo.RowHeadersVisible = false;
            this.dgvInfo.RowTemplate.Height = 24;
            this.dgvInfo.Size = new System.Drawing.Size(1284, 117);
            this.dgvInfo.TabIndex = 2;
            // 
            // colTime
            // 
            this.colTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Candara", 10F);
            this.colTime.DefaultCellStyle = dataGridViewCellStyle2;
            this.colTime.HeaderText = "時間";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            this.colTime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTime.ToolTipText = "訊息發出時間";
            // 
            // colInfo
            // 
            this.colInfo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
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
            this.lbSpeed.Location = new System.Drawing.Point(23, 147);
            this.lbSpeed.Name = "lbSpeed";
            this.lbSpeed.Size = new System.Drawing.Size(74, 12);
            this.lbSpeed.TabIndex = 3;
            this.lbSpeed.Text = "Monitor Speed";
            // 
            // txtSpeed
            // 
            this.txtSpeed.Enabled = false;
            this.txtSpeed.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSpeed.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtSpeed.Location = new System.Drawing.Point(102, 142);
            this.txtSpeed.Name = "txtSpeed";
            this.txtSpeed.Size = new System.Drawing.Size(69, 23);
            this.txtSpeed.TabIndex = 4;
            this.txtSpeed.Text = "0";
            this.txtSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtSpeed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSpeed_KeyPress);
            // 
            // lbPower
            // 
            this.lbPower.AutoSize = true;
            this.lbPower.Enabled = false;
            this.lbPower.Location = new System.Drawing.Point(23, 113);
            this.lbPower.Name = "lbPower";
            this.lbPower.Size = new System.Drawing.Size(34, 12);
            this.lbPower.TabIndex = 5;
            this.lbPower.Text = "Power";
            // 
            // imgPower
            // 
            this.imgPower.Cursor = System.Windows.Forms.Cursors.Hand;
            this.imgPower.Enabled = false;
            this.imgPower.Image = global::CtLib.Properties.Resources.Grey_Ball;
            this.imgPower.Location = new System.Drawing.Point(119, 102);
            this.imgPower.Name = "imgPower";
            this.imgPower.Size = new System.Drawing.Size(34, 34);
            this.imgPower.TabIndex = 6;
            this.imgPower.TabStop = false;
            this.imgPower.Click += new System.EventHandler(this.imgPower_Click);
            // 
            // gbVar
            // 
            this.gbVar.Controls.Add(this.label1);
            this.gbVar.Controls.Add(this.lbVarName);
            this.gbVar.Controls.Add(this.btnVarWrite);
            this.gbVar.Controls.Add(this.btnVarRead);
            this.gbVar.Controls.Add(this.txtVarVal);
            this.gbVar.Controls.Add(this.txtVarName);
            this.gbVar.Location = new System.Drawing.Point(180, 12);
            this.gbVar.Name = "gbVar";
            this.gbVar.Size = new System.Drawing.Size(254, 129);
            this.gbVar.TabIndex = 7;
            this.gbVar.TabStop = false;
            this.gbVar.Text = "變數控制";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(20, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "數值:";
            // 
            // lbVarName
            // 
            this.lbVarName.AutoSize = true;
            this.lbVarName.Enabled = false;
            this.lbVarName.Location = new System.Drawing.Point(20, 30);
            this.lbVarName.Name = "lbVarName";
            this.lbVarName.Size = new System.Drawing.Size(56, 12);
            this.lbVarName.TabIndex = 6;
            this.lbVarName.Text = "變數名稱:";
            // 
            // btnVarWrite
            // 
            this.btnVarWrite.Enabled = false;
            this.btnVarWrite.Location = new System.Drawing.Point(173, 59);
            this.btnVarWrite.Name = "btnVarWrite";
            this.btnVarWrite.Size = new System.Drawing.Size(75, 23);
            this.btnVarWrite.TabIndex = 3;
            this.btnVarWrite.Text = "寫入";
            this.btnVarWrite.UseVisualStyleBackColor = true;
            this.btnVarWrite.Click += new System.EventHandler(this.btnVarWrite_Click);
            // 
            // btnVarRead
            // 
            this.btnVarRead.Enabled = false;
            this.btnVarRead.Location = new System.Drawing.Point(173, 34);
            this.btnVarRead.Name = "btnVarRead";
            this.btnVarRead.Size = new System.Drawing.Size(75, 23);
            this.btnVarRead.TabIndex = 2;
            this.btnVarRead.Text = "讀取";
            this.btnVarRead.UseVisualStyleBackColor = true;
            this.btnVarRead.Click += new System.EventHandler(this.btnVarRead_Click);
            // 
            // txtVarVal
            // 
            this.txtVarVal.Enabled = false;
            this.txtVarVal.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVarVal.Location = new System.Drawing.Point(19, 94);
            this.txtVarVal.Name = "txtVarVal";
            this.txtVarVal.Size = new System.Drawing.Size(229, 23);
            this.txtVarVal.TabIndex = 1;
            // 
            // txtVarName
            // 
            this.txtVarName.Enabled = false;
            this.txtVarName.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVarName.Location = new System.Drawing.Point(19, 44);
            this.txtVarName.Name = "txtVarName";
            this.txtVarName.Size = new System.Drawing.Size(143, 23);
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
            this.gbTask.Location = new System.Drawing.Point(180, 147);
            this.gbTask.Name = "gbTask";
            this.gbTask.Size = new System.Drawing.Size(254, 97);
            this.gbTask.TabIndex = 8;
            this.gbTask.TabStop = false;
            this.gbTask.Text = "Task";
            // 
            // lbProgIdx
            // 
            this.lbProgIdx.AutoSize = true;
            this.lbProgIdx.Enabled = false;
            this.lbProgIdx.Location = new System.Drawing.Point(11, 25);
            this.lbProgIdx.Name = "lbProgIdx";
            this.lbProgIdx.Size = new System.Drawing.Size(30, 12);
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
            this.cbProgTask.Location = new System.Drawing.Point(14, 41);
            this.cbProgTask.Name = "cbProgTask";
            this.cbProgTask.Size = new System.Drawing.Size(38, 20);
            this.cbProgTask.TabIndex = 13;
            this.cbProgTask.SelectedValueChanged += new System.EventHandler(this.cbProgTask_SelectedValueChanged);
            // 
            // btnProgMoni
            // 
            this.btnProgMoni.Enabled = false;
            this.btnProgMoni.Location = new System.Drawing.Point(173, 38);
            this.btnProgMoni.Name = "btnProgMoni";
            this.btnProgMoni.Size = new System.Drawing.Size(75, 23);
            this.btnProgMoni.TabIndex = 12;
            this.btnProgMoni.Text = "加入監控";
            this.btnProgMoni.UseVisualStyleBackColor = true;
            this.btnProgMoni.Click += new System.EventHandler(this.btnProgMoni_Click);
            // 
            // btnProgKill
            // 
            this.btnProgKill.Enabled = false;
            this.btnProgKill.Location = new System.Drawing.Point(173, 66);
            this.btnProgKill.Name = "btnProgKill";
            this.btnProgKill.Size = new System.Drawing.Size(75, 23);
            this.btnProgKill.TabIndex = 11;
            this.btnProgKill.Text = "移除";
            this.btnProgKill.UseVisualStyleBackColor = true;
            this.btnProgKill.Click += new System.EventHandler(this.btnProgKill_Click);
            // 
            // btnProgProc
            // 
            this.btnProgProc.Enabled = false;
            this.btnProgProc.Location = new System.Drawing.Point(94, 66);
            this.btnProgProc.Name = "btnProgProc";
            this.btnProgProc.Size = new System.Drawing.Size(75, 23);
            this.btnProgProc.TabIndex = 10;
            this.btnProgProc.Text = "下一步";
            this.btnProgProc.UseVisualStyleBackColor = true;
            this.btnProgProc.Click += new System.EventHandler(this.btnProgProc_Click);
            // 
            // lbProgName
            // 
            this.lbProgName.AutoSize = true;
            this.lbProgName.Enabled = false;
            this.lbProgName.Location = new System.Drawing.Point(56, 24);
            this.lbProgName.Name = "lbProgName";
            this.lbProgName.Size = new System.Drawing.Size(49, 12);
            this.lbProgName.TabIndex = 9;
            this.lbProgName.Text = "V+ 名稱:";
            // 
            // btnProgExe
            // 
            this.btnProgExe.Enabled = false;
            this.btnProgExe.Location = new System.Drawing.Point(13, 66);
            this.btnProgExe.Name = "btnProgExe";
            this.btnProgExe.Size = new System.Drawing.Size(75, 23);
            this.btnProgExe.TabIndex = 8;
            this.btnProgExe.Text = "執行";
            this.btnProgExe.UseVisualStyleBackColor = true;
            this.btnProgExe.Click += new System.EventHandler(this.btnProgExe_Click);
            // 
            // txtProgName
            // 
            this.txtProgName.Enabled = false;
            this.txtProgName.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProgName.Location = new System.Drawing.Point(58, 39);
            this.txtProgName.Name = "txtProgName";
            this.txtProgName.Size = new System.Drawing.Size(111, 23);
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
            this.gbModel.Location = new System.Drawing.Point(180, 250);
            this.gbModel.Name = "gbModel";
            this.gbModel.Size = new System.Drawing.Size(254, 137);
            this.gbModel.TabIndex = 9;
            this.gbModel.TabStop = false;
            this.gbModel.Text = "Model Editor";
            // 
            // lbLocPath
            // 
            this.lbLocPath.AutoSize = true;
            this.lbLocPath.Enabled = false;
            this.lbLocPath.Location = new System.Drawing.Point(7, 19);
            this.lbLocPath.Name = "lbLocPath";
            this.lbLocPath.Size = new System.Drawing.Size(71, 12);
            this.lbLocPath.TabIndex = 11;
            this.lbLocPath.Text = "Locator 路徑:";
            // 
            // txtLocPath
            // 
            this.txtLocPath.Enabled = false;
            this.txtLocPath.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLocPath.Location = new System.Drawing.Point(6, 33);
            this.txtLocPath.Name = "txtLocPath";
            this.txtLocPath.Size = new System.Drawing.Size(242, 23);
            this.txtLocPath.TabIndex = 10;
            // 
            // lbModelPath
            // 
            this.lbModelPath.AutoSize = true;
            this.lbModelPath.Enabled = false;
            this.lbModelPath.Location = new System.Drawing.Point(7, 62);
            this.lbModelPath.Name = "lbModelPath";
            this.lbModelPath.Size = new System.Drawing.Size(65, 12);
            this.lbModelPath.TabIndex = 9;
            this.lbModelPath.Text = "Model 路徑:";
            // 
            // btnModelCrt
            // 
            this.btnModelCrt.Enabled = false;
            this.btnModelCrt.Location = new System.Drawing.Point(173, 105);
            this.btnModelCrt.Name = "btnModelCrt";
            this.btnModelCrt.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnModelCrt.Size = new System.Drawing.Size(75, 23);
            this.btnModelCrt.TabIndex = 8;
            this.btnModelCrt.Text = "編輯";
            this.btnModelCrt.UseVisualStyleBackColor = true;
            this.btnModelCrt.Click += new System.EventHandler(this.btnModelCrt_Click);
            // 
            // txtModelPath
            // 
            this.txtModelPath.Enabled = false;
            this.txtModelPath.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtModelPath.Location = new System.Drawing.Point(6, 76);
            this.txtModelPath.Name = "txtModelPath";
            this.txtModelPath.Size = new System.Drawing.Size(242, 23);
            this.txtModelPath.TabIndex = 7;
            // 
            // gbVision
            // 
            this.gbVision.Controls.Add(this.cbVisTool);
            this.gbVision.Controls.Add(this.lbVisPath);
            this.gbVision.Controls.Add(this.btnVision);
            this.gbVision.Controls.Add(this.txtVisionPath);
            this.gbVision.Controls.Add(this.visionWindow);
            this.gbVision.Location = new System.Drawing.Point(440, 12);
            this.gbVision.Name = "gbVision";
            this.gbVision.Size = new System.Drawing.Size(445, 456);
            this.gbVision.TabIndex = 10;
            this.gbVision.TabStop = false;
            this.gbVision.Text = "Single Vision";
            // 
            // cbVisTool
            // 
            this.cbVisTool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVisTool.Enabled = false;
            this.cbVisTool.FormattingEnabled = true;
            this.cbVisTool.Items.AddRange(new object[] {
            "Blob",
            "Camera",
            "CVT",
            "ImageProcess",
            "Locator"});
            this.cbVisTool.Location = new System.Drawing.Point(278, 14);
            this.cbVisTool.Name = "cbVisTool";
            this.cbVisTool.Size = new System.Drawing.Size(80, 20);
            this.cbVisTool.TabIndex = 14;
            // 
            // lbVisPath
            // 
            this.lbVisPath.AutoSize = true;
            this.lbVisPath.Enabled = false;
            this.lbVisPath.Location = new System.Drawing.Point(7, 17);
            this.lbVisPath.Name = "lbVisPath";
            this.lbVisPath.Size = new System.Drawing.Size(57, 12);
            this.lbVisPath.TabIndex = 12;
            this.lbVisPath.Text = "Tool 路徑:";
            // 
            // btnVision
            // 
            this.btnVision.Enabled = false;
            this.btnVision.Location = new System.Drawing.Point(364, 12);
            this.btnVision.Name = "btnVision";
            this.btnVision.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnVision.Size = new System.Drawing.Size(75, 23);
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
            this.txtVisionPath.Location = new System.Drawing.Point(78, 12);
            this.txtVisionPath.Name = "txtVisionPath";
            this.txtVisionPath.Size = new System.Drawing.Size(194, 23);
            this.txtVisionPath.TabIndex = 10;
            // 
            // visionWindow
            // 
            this.visionWindow.Location = new System.Drawing.Point(6, 41);
            this.visionWindow.Name = "visionWindow";
            this.visionWindow.OPTION_AUTOCLEAR = false;
            this.visionWindow.Size = new System.Drawing.Size(434, 409);
            this.visionWindow.TabIndex = 0;
            // 
            // gbAwp
            // 
            this.gbAwp.Controls.Add(this.btnAwpZero);
            this.gbAwp.Controls.Add(this.btnAwpSaveAs);
            this.gbAwp.Controls.Add(this.btnAwpSave);
            this.gbAwp.Location = new System.Drawing.Point(180, 398);
            this.gbAwp.Name = "gbAwp";
            this.gbAwp.Size = new System.Drawing.Size(254, 70);
            this.gbAwp.TabIndex = 11;
            this.gbAwp.TabStop = false;
            this.gbAwp.Text = "Workspace";
            // 
            // btnAwpZero
            // 
            this.btnAwpZero.Enabled = false;
            this.btnAwpZero.Location = new System.Drawing.Point(173, 30);
            this.btnAwpZero.Name = "btnAwpZero";
            this.btnAwpZero.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnAwpZero.Size = new System.Drawing.Size(75, 23);
            this.btnAwpZero.TabIndex = 11;
            this.btnAwpZero.Text = "ZeroMemory";
            this.btnAwpZero.UseVisualStyleBackColor = true;
            this.btnAwpZero.Click += new System.EventHandler(this.btnAwpZero_Click);
            // 
            // btnAwpSaveAs
            // 
            this.btnAwpSaveAs.Enabled = false;
            this.btnAwpSaveAs.Location = new System.Drawing.Point(94, 30);
            this.btnAwpSaveAs.Name = "btnAwpSaveAs";
            this.btnAwpSaveAs.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnAwpSaveAs.Size = new System.Drawing.Size(75, 23);
            this.btnAwpSaveAs.TabIndex = 10;
            this.btnAwpSaveAs.Text = "另存新檔";
            this.btnAwpSaveAs.UseVisualStyleBackColor = true;
            this.btnAwpSaveAs.Click += new System.EventHandler(this.btnAwpSaveAs_Click);
            // 
            // btnAwpSave
            // 
            this.btnAwpSave.Enabled = false;
            this.btnAwpSave.Location = new System.Drawing.Point(14, 30);
            this.btnAwpSave.Name = "btnAwpSave";
            this.btnAwpSave.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnAwpSave.Size = new System.Drawing.Size(75, 23);
            this.btnAwpSave.TabIndex = 9;
            this.btnAwpSave.Text = "存檔";
            this.btnAwpSave.UseVisualStyleBackColor = true;
            this.btnAwpSave.Click += new System.EventHandler(this.btnAwpSave_Click);
            // 
            // lbCtrl
            // 
            this.lbCtrl.AutoSize = true;
            this.lbCtrl.Enabled = false;
            this.lbCtrl.Location = new System.Drawing.Point(24, 191);
            this.lbCtrl.Name = "lbCtrl";
            this.lbCtrl.Size = new System.Drawing.Size(80, 12);
            this.lbCtrl.TabIndex = 12;
            this.lbCtrl.Text = "已連結控制器:";
            // 
            // txtCtrl
            // 
            this.txtCtrl.Enabled = false;
            this.txtCtrl.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCtrl.Location = new System.Drawing.Point(26, 206);
            this.txtCtrl.Name = "txtCtrl";
            this.txtCtrl.Size = new System.Drawing.Size(145, 23);
            this.txtCtrl.TabIndex = 13;
            // 
            // txtRobot
            // 
            this.txtRobot.Enabled = false;
            this.txtRobot.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRobot.Location = new System.Drawing.Point(25, 258);
            this.txtRobot.Name = "txtRobot";
            this.txtRobot.Size = new System.Drawing.Size(145, 23);
            this.txtRobot.TabIndex = 15;
            // 
            // lbRobot
            // 
            this.lbRobot.AutoSize = true;
            this.lbRobot.Enabled = false;
            this.lbRobot.Location = new System.Drawing.Point(23, 243);
            this.lbRobot.Name = "lbRobot";
            this.lbRobot.Size = new System.Drawing.Size(73, 12);
            this.lbRobot.TabIndex = 14;
            this.lbRobot.Text = "已連結Robot:";
            // 
            // gbVisionWindow
            // 
            this.gbVisionWindow.Controls.Add(this.btnShowVision);
            this.gbVisionWindow.Controls.Add(this.visWindow);
            this.gbVisionWindow.Location = new System.Drawing.Point(891, 12);
            this.gbVisionWindow.Name = "gbVisionWindow";
            this.gbVisionWindow.Size = new System.Drawing.Size(534, 455);
            this.gbVisionWindow.TabIndex = 17;
            this.gbVisionWindow.TabStop = false;
            this.gbVisionWindow.Text = "Vision Window";
            // 
            // btnShowVision
            // 
            this.btnShowVision.Enabled = false;
            this.btnShowVision.Location = new System.Drawing.Point(456, 9);
            this.btnShowVision.Name = "btnShowVision";
            this.btnShowVision.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnShowVision.Size = new System.Drawing.Size(75, 23);
            this.btnShowVision.TabIndex = 18;
            this.btnShowVision.Text = "顯示";
            this.btnShowVision.UseVisualStyleBackColor = true;
            this.btnShowVision.Click += new System.EventHandler(this.btnShowVision_Click);
            // 
            // visWindow
            // 
            this.visWindow.BackColor = System.Drawing.SystemColors.Control;
            this.visWindow.Location = new System.Drawing.Point(2, 30);
            this.visWindow.Name = "visWindow";
            this.visWindow.Size = new System.Drawing.Size(530, 422);
            this.visWindow.TabIndex = 16;
            // 
            // gbIO
            // 
            this.gbIO.Controls.Add(this.btnClose);
            this.gbIO.Controls.Add(this.btnOpen);
            this.gbIO.Controls.Add(this.txtIO);
            this.gbIO.Location = new System.Drawing.Point(13, 398);
            this.gbIO.Name = "gbIO";
            this.gbIO.Size = new System.Drawing.Size(158, 69);
            this.gbIO.TabIndex = 18;
            this.gbIO.TabStop = false;
            this.gbIO.Text = "I/O 控制";
            // 
            // btnClose
            // 
            this.btnClose.Enabled = false;
            this.btnClose.Location = new System.Drawing.Point(114, 39);
            this.btnClose.Name = "btnClose";
            this.btnClose.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnClose.Size = new System.Drawing.Size(35, 23);
            this.btnClose.TabIndex = 18;
            this.btnClose.Text = "關";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Enabled = false;
            this.btnOpen.Location = new System.Drawing.Point(114, 13);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnOpen.Size = new System.Drawing.Size(35, 23);
            this.btnOpen.TabIndex = 17;
            this.btnOpen.Text = "開";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // txtIO
            // 
            this.txtIO.Enabled = false;
            this.txtIO.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIO.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtIO.Location = new System.Drawing.Point(18, 28);
            this.txtIO.MaxLength = 3;
            this.txtIO.Name = "txtIO";
            this.txtIO.Size = new System.Drawing.Size(79, 23);
            this.txtIO.TabIndex = 16;
            this.txtIO.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btn90P
            // 
            this.btn90P.Enabled = false;
            this.btn90P.Location = new System.Drawing.Point(21, 326);
            this.btn90P.Name = "btn90P";
            this.btn90P.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btn90P.Size = new System.Drawing.Size(75, 23);
            this.btn90P.TabIndex = 19;
            this.btn90P.Text = "CW";
            this.btn90P.UseVisualStyleBackColor = true;
            this.btn90P.Click += new System.EventHandler(this.btn90P_Click);
            // 
            // btn90N
            // 
            this.btn90N.Enabled = false;
            this.btn90N.Location = new System.Drawing.Point(21, 355);
            this.btn90N.Name = "btn90N";
            this.btn90N.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btn90N.Size = new System.Drawing.Size(75, 23);
            this.btn90N.TabIndex = 20;
            this.btn90N.Text = "CCW";
            this.btn90N.UseVisualStyleBackColor = true;
            this.btn90N.Click += new System.EventHandler(this.btn90N_Click);
            // 
            // btnPendant
            // 
            this.btnPendant.Enabled = false;
            this.btnPendant.Location = new System.Drawing.Point(21, 301);
            this.btnPendant.Name = "btnPendant";
            this.btnPendant.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnPendant.Size = new System.Drawing.Size(75, 23);
            this.btnPendant.TabIndex = 21;
            this.btnPendant.Text = "Pendant";
            this.btnPendant.UseVisualStyleBackColor = true;
            this.btnPendant.Click += new System.EventHandler(this.btnPendant_Click);
            // 
            // btnCusCmd
            // 
            this.btnCusCmd.Enabled = false;
            this.btnCusCmd.Location = new System.Drawing.Point(99, 355);
            this.btnCusCmd.Name = "btnCusCmd";
            this.btnCusCmd.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnCusCmd.Size = new System.Drawing.Size(75, 23);
            this.btnCusCmd.TabIndex = 22;
            this.btnCusCmd.Text = "Send String";
            this.btnCusCmd.UseVisualStyleBackColor = true;
            this.btnCusCmd.Click += new System.EventHandler(this.btnCusCmd_Click);
            // 
            // Test_ACE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 599);
            this.Controls.Add(this.btnCusCmd);
            this.Controls.Add(this.btnPendant);
            this.Controls.Add(this.btn90N);
            this.Controls.Add(this.btn90P);
            this.Controls.Add(this.gbIO);
            this.Controls.Add(this.gbVisionWindow);
            this.Controls.Add(this.txtRobot);
            this.Controls.Add(this.lbRobot);
            this.Controls.Add(this.txtCtrl);
            this.Controls.Add(this.lbCtrl);
            this.Controls.Add(this.gbAwp);
            this.Controls.Add(this.gbVision);
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
            this.Name = "Test_ACE";
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
            this.gbVision.ResumeLayout(false);
            this.gbVision.PerformLayout();
            this.gbAwp.ResumeLayout(false);
            this.gbVisionWindow.ResumeLayout(false);
            this.gbIO.ResumeLayout(false);
            this.gbIO.PerformLayout();
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
        private System.Windows.Forms.Label label1;
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
        private System.Windows.Forms.GroupBox gbVision;
        private System.Windows.Forms.Label lbVisPath;
        private System.Windows.Forms.Button btnVision;
        private System.Windows.Forms.TextBox txtVisionPath;
        private Module.Adept.CtAceSingleVision visionWindow;
        private System.Windows.Forms.ComboBox cbVisTool;
        private System.Windows.Forms.GroupBox gbAwp;
        private System.Windows.Forms.Button btnAwpZero;
        private System.Windows.Forms.Button btnAwpSaveAs;
        private System.Windows.Forms.Button btnAwpSave;
        private System.Windows.Forms.Label lbCtrl;
        private System.Windows.Forms.TextBox txtCtrl;
        private System.Windows.Forms.TextBox txtRobot;
        private System.Windows.Forms.Label lbRobot;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colInfo;
        private Module.Adept.CtAceVisionWindow visWindow;
        private System.Windows.Forms.GroupBox gbVisionWindow;
        private System.Windows.Forms.Button btnShowVision;
        private System.Windows.Forms.GroupBox gbIO;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TextBox txtIO;
        private System.Windows.Forms.Button btn90P;
        private System.Windows.Forms.Button btn90N;
        private System.Windows.Forms.Button btnPendant;
        private System.Windows.Forms.Button btnCusCmd;
    }
}