namespace CtLib.Forms.TestPlatform {
    partial class IAI_PCON {
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.btnConnect = new System.Windows.Forms.Button();
			this.lbDevID = new System.Windows.Forms.Label();
			this.nudDevID = new System.Windows.Forms.NumericUpDown();
			this.lsbxMsg = new System.Windows.Forms.ListBox();
			this.picConStt = new System.Windows.Forms.PictureBox();
			this.gbStt = new System.Windows.Forms.GroupBox();
			this.btnSetBreak = new System.Windows.Forms.Button();
			this.picBreak = new System.Windows.Forms.PictureBox();
			this.btnSetServo = new System.Windows.Forms.Button();
			this.picServo = new System.Windows.Forms.PictureBox();
			this.gbHome = new System.Windows.Forms.GroupBox();
			this.button1 = new System.Windows.Forms.Button();
			this.gbJog = new System.Windows.Forms.GroupBox();
			this.chkJog = new System.Windows.Forms.CheckBox();
			this.btnJogP = new System.Windows.Forms.Button();
			this.btnJogR = new System.Windows.Forms.Button();
			this.gbAbsMove = new System.Windows.Forms.GroupBox();
			this.btnAbsMove = new System.Windows.Forms.Button();
			this.txtTarSpd = new System.Windows.Forms.TextBox();
			this.txtTarPos = new System.Windows.Forms.TextBox();
			this.lbTarSpd = new System.Windows.Forms.Label();
			this.lbTarPos = new System.Windows.Forms.Label();
			this.gbPathMove = new System.Windows.Forms.GroupBox();
			this.btnPathMove = new System.Windows.Forms.Button();
			this.txtPath = new System.Windows.Forms.TextBox();
			this.lbPathIdx = new System.Windows.Forms.Label();
			this.gbTeach = new System.Windows.Forms.GroupBox();
			this.btnTeach = new System.Windows.Forms.Button();
			this.txtTeach = new System.Windows.Forms.TextBox();
			this.lbTeach = new System.Windows.Forms.Label();
			this.gbMotorStt = new System.Windows.Forms.GroupBox();
			this.btnGetState = new System.Windows.Forms.Button();
			this.lbCurSpd = new System.Windows.Forms.Label();
			this.txtCurSpd = new System.Windows.Forms.TextBox();
			this.lbCurPos = new System.Windows.Forms.Label();
			this.txtCurPos = new System.Windows.Forms.TextBox();
			this.lbEncErr = new System.Windows.Forms.Label();
			this.lbFatalErr = new System.Windows.Forms.Label();
			this.lbLightErr = new System.Windows.Forms.Label();
			this.picEncErr = new System.Windows.Forms.PictureBox();
			this.picFatalErr = new System.Windows.Forms.PictureBox();
			this.picLightErr = new System.Windows.Forms.PictureBox();
			this.nudPathStart = new System.Windows.Forms.NumericUpDown();
			this.nudPathEnd = new System.Windows.Forms.NumericUpDown();
			this.lbTo = new System.Windows.Forms.Label();
			this.btnGetPath = new System.Windows.Forms.Button();
			this.gbPath = new System.Windows.Forms.GroupBox();
			this.btnSetPath = new System.Windows.Forms.Button();
			this.dgvPos = new System.Windows.Forms.DataGridView();
			this.colIdx = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colPos = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colPosBand = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colSpeed = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colIndvP = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colIndvM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colAccel = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colDecel = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colPush = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colLoad = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colCtrl = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.btnCleanAlarm = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.nudDevID)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picConStt)).BeginInit();
			this.gbStt.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picBreak)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picServo)).BeginInit();
			this.gbHome.SuspendLayout();
			this.gbJog.SuspendLayout();
			this.gbAbsMove.SuspendLayout();
			this.gbPathMove.SuspendLayout();
			this.gbTeach.SuspendLayout();
			this.gbMotorStt.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picEncErr)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picFatalErr)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picLightErr)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPathStart)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPathEnd)).BeginInit();
			this.gbPath.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvPos)).BeginInit();
			this.SuspendLayout();
			// 
			// btnConnect
			// 
			this.btnConnect.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnConnect.Location = new System.Drawing.Point(111, 24);
			this.btnConnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(144, 54);
			this.btnConnect.TabIndex = 0;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// lbDevID
			// 
			this.lbDevID.AutoSize = true;
			this.lbDevID.Location = new System.Drawing.Point(31, 24);
			this.lbDevID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lbDevID.Name = "lbDevID";
			this.lbDevID.Size = new System.Drawing.Size(65, 15);
			this.lbDevID.TabIndex = 1;
			this.lbDevID.Text = "Device ID";
			// 
			// nudDevID
			// 
			this.nudDevID.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nudDevID.Location = new System.Drawing.Point(29, 48);
			this.nudDevID.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.nudDevID.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
			this.nudDevID.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudDevID.Name = "nudDevID";
			this.nudDevID.Size = new System.Drawing.Size(73, 27);
			this.nudDevID.TabIndex = 2;
			this.nudDevID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.nudDevID.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// lsbxMsg
			// 
			this.lsbxMsg.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lsbxMsg.FormattingEnabled = true;
			this.lsbxMsg.ItemHeight = 15;
			this.lsbxMsg.Location = new System.Drawing.Point(0, 727);
			this.lsbxMsg.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.lsbxMsg.Name = "lsbxMsg";
			this.lsbxMsg.Size = new System.Drawing.Size(936, 229);
			this.lsbxMsg.TabIndex = 3;
			// 
			// picConStt
			// 
			this.picConStt.Image = global::CtLib.Properties.Resources.Grey_Ball;
			this.picConStt.Location = new System.Drawing.Point(263, 29);
			this.picConStt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.picConStt.Name = "picConStt";
			this.picConStt.Size = new System.Drawing.Size(47, 44);
			this.picConStt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picConStt.TabIndex = 4;
			this.picConStt.TabStop = false;
			// 
			// gbStt
			// 
			this.gbStt.Controls.Add(this.btnSetBreak);
			this.gbStt.Controls.Add(this.picBreak);
			this.gbStt.Controls.Add(this.btnSetServo);
			this.gbStt.Controls.Add(this.picServo);
			this.gbStt.Location = new System.Drawing.Point(29, 104);
			this.gbStt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbStt.Name = "gbStt";
			this.gbStt.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbStt.Size = new System.Drawing.Size(161, 156);
			this.gbStt.TabIndex = 5;
			this.gbStt.TabStop = false;
			this.gbStt.Text = "System Status";
			// 
			// btnSetBreak
			// 
			this.btnSetBreak.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSetBreak.Location = new System.Drawing.Point(60, 91);
			this.btnSetBreak.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnSetBreak.Name = "btnSetBreak";
			this.btnSetBreak.Size = new System.Drawing.Size(84, 35);
			this.btnSetBreak.TabIndex = 9;
			this.btnSetBreak.Text = "Break";
			this.btnSetBreak.UseVisualStyleBackColor = true;
			this.btnSetBreak.Click += new System.EventHandler(this.btnSetBreak_Click);
			// 
			// picBreak
			// 
			this.picBreak.Image = global::CtLib.Properties.Resources.Grey_Ball;
			this.picBreak.Location = new System.Drawing.Point(15, 91);
			this.picBreak.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.picBreak.Name = "picBreak";
			this.picBreak.Size = new System.Drawing.Size(37, 35);
			this.picBreak.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picBreak.TabIndex = 8;
			this.picBreak.TabStop = false;
			// 
			// btnSetServo
			// 
			this.btnSetServo.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSetServo.Location = new System.Drawing.Point(60, 49);
			this.btnSetServo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnSetServo.Name = "btnSetServo";
			this.btnSetServo.Size = new System.Drawing.Size(84, 35);
			this.btnSetServo.TabIndex = 7;
			this.btnSetServo.Text = "Servo";
			this.btnSetServo.UseVisualStyleBackColor = true;
			this.btnSetServo.Click += new System.EventHandler(this.btnSetServo_Click);
			// 
			// picServo
			// 
			this.picServo.Image = global::CtLib.Properties.Resources.Grey_Ball;
			this.picServo.Location = new System.Drawing.Point(15, 49);
			this.picServo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.picServo.Name = "picServo";
			this.picServo.Size = new System.Drawing.Size(37, 35);
			this.picServo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picServo.TabIndex = 6;
			this.picServo.TabStop = false;
			// 
			// gbHome
			// 
			this.gbHome.Controls.Add(this.button1);
			this.gbHome.Location = new System.Drawing.Point(199, 104);
			this.gbHome.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbHome.Name = "gbHome";
			this.gbHome.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbHome.Size = new System.Drawing.Size(111, 156);
			this.gbHome.TabIndex = 6;
			this.gbHome.TabStop = false;
			this.gbHome.Text = "Functions";
			// 
			// button1
			// 
			this.button1.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button1.Location = new System.Drawing.Point(13, 69);
			this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(80, 39);
			this.button1.TabIndex = 7;
			this.button1.Text = "Home";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// gbJog
			// 
			this.gbJog.Controls.Add(this.chkJog);
			this.gbJog.Controls.Add(this.btnJogP);
			this.gbJog.Controls.Add(this.btnJogR);
			this.gbJog.Location = new System.Drawing.Point(29, 268);
			this.gbJog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbJog.Name = "gbJog";
			this.gbJog.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbJog.Size = new System.Drawing.Size(280, 101);
			this.gbJog.TabIndex = 7;
			this.gbJog.TabStop = false;
			this.gbJog.Text = "Jog";
			// 
			// chkJog
			// 
			this.chkJog.AutoSize = true;
			this.chkJog.Location = new System.Drawing.Point(40, 71);
			this.chkJog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.chkJog.Name = "chkJog";
			this.chkJog.Size = new System.Drawing.Size(65, 19);
			this.chkJog.TabIndex = 8;
			this.chkJog.Text = "吋動?";
			this.chkJog.UseVisualStyleBackColor = true;
			this.chkJog.CheckedChanged += new System.EventHandler(this.chkJog_CheckedChanged);
			// 
			// btnJogP
			// 
			this.btnJogP.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnJogP.Location = new System.Drawing.Point(145, 25);
			this.btnJogP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnJogP.Name = "btnJogP";
			this.btnJogP.Size = new System.Drawing.Size(80, 39);
			this.btnJogP.TabIndex = 9;
			this.btnJogP.Text = "Jog +";
			this.btnJogP.UseVisualStyleBackColor = true;
			this.btnJogP.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnJogP_MouseDown);
			this.btnJogP.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnJogP_MouseUp);
			// 
			// btnJogR
			// 
			this.btnJogR.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnJogR.Location = new System.Drawing.Point(40, 25);
			this.btnJogR.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnJogR.Name = "btnJogR";
			this.btnJogR.Size = new System.Drawing.Size(80, 39);
			this.btnJogR.TabIndex = 8;
			this.btnJogR.Text = "Jog -";
			this.btnJogR.UseVisualStyleBackColor = true;
			this.btnJogR.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnJogR_MouseDown);
			this.btnJogR.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnJogR_MouseUp);
			// 
			// gbAbsMove
			// 
			this.gbAbsMove.Controls.Add(this.btnAbsMove);
			this.gbAbsMove.Controls.Add(this.txtTarSpd);
			this.gbAbsMove.Controls.Add(this.txtTarPos);
			this.gbAbsMove.Controls.Add(this.lbTarSpd);
			this.gbAbsMove.Controls.Add(this.lbTarPos);
			this.gbAbsMove.Location = new System.Drawing.Point(333, 104);
			this.gbAbsMove.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbAbsMove.Name = "gbAbsMove";
			this.gbAbsMove.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbAbsMove.Size = new System.Drawing.Size(316, 101);
			this.gbAbsMove.TabIndex = 8;
			this.gbAbsMove.TabStop = false;
			this.gbAbsMove.Text = "Absolutly Move";
			// 
			// btnAbsMove
			// 
			this.btnAbsMove.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnAbsMove.Location = new System.Drawing.Point(231, 34);
			this.btnAbsMove.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnAbsMove.Name = "btnAbsMove";
			this.btnAbsMove.Size = new System.Drawing.Size(60, 50);
			this.btnAbsMove.TabIndex = 6;
			this.btnAbsMove.Text = "動";
			this.btnAbsMove.UseVisualStyleBackColor = true;
			this.btnAbsMove.Click += new System.EventHandler(this.btnAbsMove_Click);
			// 
			// txtTarSpd
			// 
			this.txtTarSpd.Location = new System.Drawing.Point(107, 62);
			this.txtTarSpd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtTarSpd.Name = "txtTarSpd";
			this.txtTarSpd.Size = new System.Drawing.Size(112, 25);
			this.txtTarSpd.TabIndex = 5;
			this.txtTarSpd.Text = "5";
			this.txtTarSpd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtTarPos
			// 
			this.txtTarPos.Location = new System.Drawing.Point(107, 28);
			this.txtTarPos.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtTarPos.Name = "txtTarPos";
			this.txtTarPos.Size = new System.Drawing.Size(112, 25);
			this.txtTarPos.TabIndex = 4;
			this.txtTarPos.Text = "5";
			this.txtTarPos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbTarSpd
			// 
			this.lbTarSpd.AutoSize = true;
			this.lbTarSpd.Location = new System.Drawing.Point(28, 69);
			this.lbTarSpd.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lbTarSpd.Name = "lbTarSpd";
			this.lbTarSpd.Size = new System.Drawing.Size(69, 15);
			this.lbTarSpd.TabIndex = 3;
			this.lbTarSpd.Text = "速        度";
			// 
			// lbTarPos
			// 
			this.lbTarPos.AutoSize = true;
			this.lbTarPos.Location = new System.Drawing.Point(28, 34);
			this.lbTarPos.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lbTarPos.Name = "lbTarPos";
			this.lbTarPos.Size = new System.Drawing.Size(67, 15);
			this.lbTarPos.TabIndex = 2;
			this.lbTarPos.Text = "目標位置";
			// 
			// gbPathMove
			// 
			this.gbPathMove.Controls.Add(this.btnPathMove);
			this.gbPathMove.Controls.Add(this.txtPath);
			this.gbPathMove.Controls.Add(this.lbPathIdx);
			this.gbPathMove.Location = new System.Drawing.Point(333, 214);
			this.gbPathMove.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbPathMove.Name = "gbPathMove";
			this.gbPathMove.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbPathMove.Size = new System.Drawing.Size(316, 74);
			this.gbPathMove.TabIndex = 9;
			this.gbPathMove.TabStop = false;
			this.gbPathMove.Text = "Path Move";
			// 
			// btnPathMove
			// 
			this.btnPathMove.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnPathMove.Location = new System.Drawing.Point(231, 25);
			this.btnPathMove.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnPathMove.Name = "btnPathMove";
			this.btnPathMove.Size = new System.Drawing.Size(60, 28);
			this.btnPathMove.TabIndex = 6;
			this.btnPathMove.Text = "動";
			this.btnPathMove.UseVisualStyleBackColor = true;
			this.btnPathMove.Click += new System.EventHandler(this.btnPathMove_Click);
			// 
			// txtPath
			// 
			this.txtPath.Location = new System.Drawing.Point(107, 25);
			this.txtPath.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtPath.Name = "txtPath";
			this.txtPath.Size = new System.Drawing.Size(112, 25);
			this.txtPath.TabIndex = 4;
			this.txtPath.Text = "5";
			this.txtPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbPathIdx
			// 
			this.lbPathIdx.AutoSize = true;
			this.lbPathIdx.Location = new System.Drawing.Point(28, 31);
			this.lbPathIdx.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lbPathIdx.Name = "lbPathIdx";
			this.lbPathIdx.Size = new System.Drawing.Size(67, 15);
			this.lbPathIdx.TabIndex = 2;
			this.lbPathIdx.Text = "路徑編號";
			// 
			// gbTeach
			// 
			this.gbTeach.Controls.Add(this.btnTeach);
			this.gbTeach.Controls.Add(this.txtTeach);
			this.gbTeach.Controls.Add(this.lbTeach);
			this.gbTeach.Location = new System.Drawing.Point(333, 295);
			this.gbTeach.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbTeach.Name = "gbTeach";
			this.gbTeach.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbTeach.Size = new System.Drawing.Size(316, 74);
			this.gbTeach.TabIndex = 10;
			this.gbTeach.TabStop = false;
			this.gbTeach.Text = "Teach";
			// 
			// btnTeach
			// 
			this.btnTeach.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnTeach.Location = new System.Drawing.Point(231, 28);
			this.btnTeach.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnTeach.Name = "btnTeach";
			this.btnTeach.Size = new System.Drawing.Size(60, 28);
			this.btnTeach.TabIndex = 6;
			this.btnTeach.Text = "踢";
			this.btnTeach.UseVisualStyleBackColor = true;
			this.btnTeach.Click += new System.EventHandler(this.btnTeach_Click);
			// 
			// txtTeach
			// 
			this.txtTeach.Location = new System.Drawing.Point(107, 28);
			this.txtTeach.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtTeach.Name = "txtTeach";
			this.txtTeach.Size = new System.Drawing.Size(112, 25);
			this.txtTeach.TabIndex = 4;
			this.txtTeach.Text = "5";
			this.txtTeach.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbTeach
			// 
			this.lbTeach.AutoSize = true;
			this.lbTeach.Location = new System.Drawing.Point(28, 34);
			this.lbTeach.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lbTeach.Name = "lbTeach";
			this.lbTeach.Size = new System.Drawing.Size(67, 15);
			this.lbTeach.TabIndex = 2;
			this.lbTeach.Text = "教導編號";
			// 
			// gbMotorStt
			// 
			this.gbMotorStt.Controls.Add(this.btnGetState);
			this.gbMotorStt.Controls.Add(this.lbCurSpd);
			this.gbMotorStt.Controls.Add(this.txtCurSpd);
			this.gbMotorStt.Controls.Add(this.lbCurPos);
			this.gbMotorStt.Controls.Add(this.txtCurPos);
			this.gbMotorStt.Controls.Add(this.lbEncErr);
			this.gbMotorStt.Controls.Add(this.lbFatalErr);
			this.gbMotorStt.Controls.Add(this.lbLightErr);
			this.gbMotorStt.Controls.Add(this.picEncErr);
			this.gbMotorStt.Controls.Add(this.picFatalErr);
			this.gbMotorStt.Controls.Add(this.picLightErr);
			this.gbMotorStt.Location = new System.Drawing.Point(671, 104);
			this.gbMotorStt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbMotorStt.Name = "gbMotorStt";
			this.gbMotorStt.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbMotorStt.Size = new System.Drawing.Size(244, 265);
			this.gbMotorStt.TabIndex = 11;
			this.gbMotorStt.TabStop = false;
			this.gbMotorStt.Text = "Motor Status";
			// 
			// btnGetState
			// 
			this.btnGetState.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnGetState.Location = new System.Drawing.Point(28, 206);
			this.btnGetState.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnGetState.Name = "btnGetState";
			this.btnGetState.Size = new System.Drawing.Size(131, 40);
			this.btnGetState.TabIndex = 19;
			this.btnGetState.Text = "Get Status";
			this.btnGetState.UseVisualStyleBackColor = true;
			this.btnGetState.Click += new System.EventHandler(this.btnGetState_Click);
			// 
			// lbCurSpd
			// 
			this.lbCurSpd.AutoSize = true;
			this.lbCurSpd.Location = new System.Drawing.Point(25, 175);
			this.lbCurSpd.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lbCurSpd.Name = "lbCurSpd";
			this.lbCurSpd.Size = new System.Drawing.Size(67, 15);
			this.lbCurSpd.TabIndex = 18;
			this.lbCurSpd.Text = "目前速度";
			// 
			// txtCurSpd
			// 
			this.txtCurSpd.Enabled = false;
			this.txtCurSpd.Location = new System.Drawing.Point(103, 169);
			this.txtCurSpd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtCurSpd.Name = "txtCurSpd";
			this.txtCurSpd.Size = new System.Drawing.Size(112, 25);
			this.txtCurSpd.TabIndex = 17;
			this.txtCurSpd.Text = "5";
			this.txtCurSpd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbCurPos
			// 
			this.lbCurPos.AutoSize = true;
			this.lbCurPos.Location = new System.Drawing.Point(25, 140);
			this.lbCurPos.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lbCurPos.Name = "lbCurPos";
			this.lbCurPos.Size = new System.Drawing.Size(67, 15);
			this.lbCurPos.TabIndex = 16;
			this.lbCurPos.Text = "目前位置";
			// 
			// txtCurPos
			// 
			this.txtCurPos.Enabled = false;
			this.txtCurPos.Location = new System.Drawing.Point(103, 134);
			this.txtCurPos.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtCurPos.Name = "txtCurPos";
			this.txtCurPos.Size = new System.Drawing.Size(112, 25);
			this.txtCurPos.TabIndex = 15;
			this.txtCurPos.Text = "5";
			this.txtCurPos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbEncErr
			// 
			this.lbEncErr.AutoSize = true;
			this.lbEncErr.Location = new System.Drawing.Point(63, 99);
			this.lbEncErr.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lbEncErr.Name = "lbEncErr";
			this.lbEncErr.Size = new System.Drawing.Size(89, 15);
			this.lbEncErr.TabIndex = 14;
			this.lbEncErr.Text = "Encoder Error";
			// 
			// lbFatalErr
			// 
			this.lbFatalErr.AutoSize = true;
			this.lbFatalErr.Location = new System.Drawing.Point(63, 66);
			this.lbFatalErr.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lbFatalErr.Name = "lbFatalErr";
			this.lbFatalErr.Size = new System.Drawing.Size(70, 15);
			this.lbFatalErr.TabIndex = 13;
			this.lbFatalErr.Text = "Fatal Error";
			// 
			// lbLightErr
			// 
			this.lbLightErr.AutoSize = true;
			this.lbLightErr.Location = new System.Drawing.Point(63, 34);
			this.lbLightErr.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lbLightErr.Name = "lbLightErr";
			this.lbLightErr.Size = new System.Drawing.Size(73, 15);
			this.lbLightErr.TabIndex = 12;
			this.lbLightErr.Text = "Light Error";
			// 
			// picEncErr
			// 
			this.picEncErr.Image = global::CtLib.Properties.Resources.Grey_Ball;
			this.picEncErr.Location = new System.Drawing.Point(28, 91);
			this.picEncErr.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.picEncErr.Name = "picEncErr";
			this.picEncErr.Size = new System.Drawing.Size(27, 25);
			this.picEncErr.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picEncErr.TabIndex = 11;
			this.picEncErr.TabStop = false;
			// 
			// picFatalErr
			// 
			this.picFatalErr.Image = global::CtLib.Properties.Resources.Grey_Ball;
			this.picFatalErr.Location = new System.Drawing.Point(28, 59);
			this.picFatalErr.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.picFatalErr.Name = "picFatalErr";
			this.picFatalErr.Size = new System.Drawing.Size(27, 25);
			this.picFatalErr.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picFatalErr.TabIndex = 10;
			this.picFatalErr.TabStop = false;
			// 
			// picLightErr
			// 
			this.picLightErr.Image = global::CtLib.Properties.Resources.Grey_Ball;
			this.picLightErr.Location = new System.Drawing.Point(28, 28);
			this.picLightErr.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.picLightErr.Name = "picLightErr";
			this.picLightErr.Size = new System.Drawing.Size(27, 25);
			this.picLightErr.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picLightErr.TabIndex = 9;
			this.picLightErr.TabStop = false;
			// 
			// nudPathStart
			// 
			this.nudPathStart.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nudPathStart.Location = new System.Drawing.Point(21, 26);
			this.nudPathStart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.nudPathStart.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.nudPathStart.Name = "nudPathStart";
			this.nudPathStart.Size = new System.Drawing.Size(73, 27);
			this.nudPathStart.TabIndex = 12;
			this.nudPathStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// nudPathEnd
			// 
			this.nudPathEnd.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nudPathEnd.Location = new System.Drawing.Point(129, 26);
			this.nudPathEnd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.nudPathEnd.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.nudPathEnd.Name = "nudPathEnd";
			this.nudPathEnd.Size = new System.Drawing.Size(73, 27);
			this.nudPathEnd.TabIndex = 13;
			this.nudPathEnd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.nudPathEnd.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
			// 
			// lbTo
			// 
			this.lbTo.AutoSize = true;
			this.lbTo.Location = new System.Drawing.Point(107, 32);
			this.lbTo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lbTo.Name = "lbTo";
			this.lbTo.Size = new System.Drawing.Size(15, 15);
			this.lbTo.TabIndex = 15;
			this.lbTo.Text = "~";
			// 
			// btnGetPath
			// 
			this.btnGetPath.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnGetPath.Location = new System.Drawing.Point(216, 20);
			this.btnGetPath.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnGetPath.Name = "btnGetPath";
			this.btnGetPath.Size = new System.Drawing.Size(131, 40);
			this.btnGetPath.TabIndex = 20;
			this.btnGetPath.Text = "Get Path";
			this.btnGetPath.UseVisualStyleBackColor = true;
			this.btnGetPath.Click += new System.EventHandler(this.btnGetPath_Click);
			// 
			// gbPath
			// 
			this.gbPath.Controls.Add(this.btnSetPath);
			this.gbPath.Controls.Add(this.dgvPos);
			this.gbPath.Controls.Add(this.nudPathStart);
			this.gbPath.Controls.Add(this.btnGetPath);
			this.gbPath.Controls.Add(this.nudPathEnd);
			this.gbPath.Controls.Add(this.lbTo);
			this.gbPath.Location = new System.Drawing.Point(29, 376);
			this.gbPath.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbPath.Name = "gbPath";
			this.gbPath.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbPath.Size = new System.Drawing.Size(885, 342);
			this.gbPath.TabIndex = 21;
			this.gbPath.TabStop = false;
			this.gbPath.Text = "Path";
			// 
			// btnSetPath
			// 
			this.btnSetPath.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnSetPath.Location = new System.Drawing.Point(355, 19);
			this.btnSetPath.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnSetPath.Name = "btnSetPath";
			this.btnSetPath.Size = new System.Drawing.Size(131, 40);
			this.btnSetPath.TabIndex = 47;
			this.btnSetPath.Text = "Set Path";
			this.btnSetPath.UseVisualStyleBackColor = true;
			this.btnSetPath.Click += new System.EventHandler(this.btnSetPath_Click);
			// 
			// dgvPos
			// 
			this.dgvPos.AllowUserToAddRows = false;
			this.dgvPos.AllowUserToDeleteRows = false;
			this.dgvPos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvPos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvPos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvPos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIdx,
            this.colPos,
            this.colPosBand,
            this.colSpeed,
            this.colIndvP,
            this.colIndvM,
            this.colAccel,
            this.colDecel,
            this.colPush,
            this.colLoad,
            this.colCtrl});
			this.dgvPos.Location = new System.Drawing.Point(21, 68);
			this.dgvPos.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.dgvPos.Name = "dgvPos";
			this.dgvPos.RowHeadersVisible = false;
			this.dgvPos.RowTemplate.Height = 24;
			this.dgvPos.Size = new System.Drawing.Size(836, 268);
			this.dgvPos.TabIndex = 46;
			// 
			// colIdx
			// 
			this.colIdx.HeaderText = "Index";
			this.colIdx.Name = "colIdx";
			this.colIdx.ReadOnly = true;
			this.colIdx.Width = 50;
			// 
			// colPos
			// 
			this.colPos.HeaderText = "Position (mm)";
			this.colPos.Name = "colPos";
			this.colPos.Width = 50;
			// 
			// colPosBand
			// 
			this.colPosBand.HeaderText = "Positioning Band (mm)";
			this.colPosBand.Name = "colPosBand";
			this.colPosBand.Width = 60;
			// 
			// colSpeed
			// 
			this.colSpeed.HeaderText = "Speed (mm/s)";
			this.colSpeed.Name = "colSpeed";
			this.colSpeed.Width = 50;
			// 
			// colIndvP
			// 
			this.colIndvP.HeaderText = "Indv Zone Bound + (mm)";
			this.colIndvP.Name = "colIndvP";
			this.colIndvP.Width = 80;
			// 
			// colIndvM
			// 
			this.colIndvM.HeaderText = "Indv Zone Bound - (mm)";
			this.colIndvM.Name = "colIndvM";
			this.colIndvM.Width = 80;
			// 
			// colAccel
			// 
			this.colAccel.HeaderText = "Accel (G)";
			this.colAccel.Name = "colAccel";
			this.colAccel.Width = 50;
			// 
			// colDecel
			// 
			this.colDecel.HeaderText = "Decel (G)";
			this.colDecel.Name = "colDecel";
			this.colDecel.Width = 50;
			// 
			// colPush
			// 
			this.colPush.HeaderText = "Push Current (%)";
			this.colPush.Name = "colPush";
			this.colPush.Width = 50;
			// 
			// colLoad
			// 
			this.colLoad.HeaderText = "Load Current (%)";
			this.colLoad.Name = "colLoad";
			this.colLoad.Width = 50;
			// 
			// colCtrl
			// 
			this.colCtrl.HeaderText = "Control Flag";
			this.colCtrl.Name = "colCtrl";
			this.colCtrl.Width = 50;
			// 
			// btnCleanAlarm
			// 
			this.btnCleanAlarm.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnCleanAlarm.Location = new System.Drawing.Point(784, 39);
			this.btnCleanAlarm.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnCleanAlarm.Name = "btnCleanAlarm";
			this.btnCleanAlarm.Size = new System.Drawing.Size(131, 40);
			this.btnCleanAlarm.TabIndex = 20;
			this.btnCleanAlarm.Text = "Clean Alarm";
			this.btnCleanAlarm.UseVisualStyleBackColor = true;
			this.btnCleanAlarm.Click += new System.EventHandler(this.btnCleanAlarm_Click);
			// 
			// IAI_PCON
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(936, 956);
			this.Controls.Add(this.btnCleanAlarm);
			this.Controls.Add(this.gbPath);
			this.Controls.Add(this.gbMotorStt);
			this.Controls.Add(this.gbTeach);
			this.Controls.Add(this.gbPathMove);
			this.Controls.Add(this.gbAbsMove);
			this.Controls.Add(this.gbJog);
			this.Controls.Add(this.gbHome);
			this.Controls.Add(this.gbStt);
			this.Controls.Add(this.picConStt);
			this.Controls.Add(this.lsbxMsg);
			this.Controls.Add(this.nudDevID);
			this.Controls.Add(this.lbDevID);
			this.Controls.Add(this.btnConnect);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "IAI_PCON";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "IAI_PCON";
			((System.ComponentModel.ISupportInitialize)(this.nudDevID)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picConStt)).EndInit();
			this.gbStt.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picBreak)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picServo)).EndInit();
			this.gbHome.ResumeLayout(false);
			this.gbJog.ResumeLayout(false);
			this.gbJog.PerformLayout();
			this.gbAbsMove.ResumeLayout(false);
			this.gbAbsMove.PerformLayout();
			this.gbPathMove.ResumeLayout(false);
			this.gbPathMove.PerformLayout();
			this.gbTeach.ResumeLayout(false);
			this.gbTeach.PerformLayout();
			this.gbMotorStt.ResumeLayout(false);
			this.gbMotorStt.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.picEncErr)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picFatalErr)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picLightErr)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPathStart)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPathEnd)).EndInit();
			this.gbPath.ResumeLayout(false);
			this.gbPath.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvPos)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lbDevID;
        private System.Windows.Forms.NumericUpDown nudDevID;
        private System.Windows.Forms.ListBox lsbxMsg;
        private System.Windows.Forms.PictureBox picConStt;
        private System.Windows.Forms.GroupBox gbStt;
        private System.Windows.Forms.Button btnSetBreak;
        private System.Windows.Forms.PictureBox picBreak;
        private System.Windows.Forms.Button btnSetServo;
        private System.Windows.Forms.PictureBox picServo;
        private System.Windows.Forms.GroupBox gbHome;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox gbJog;
        private System.Windows.Forms.CheckBox chkJog;
        private System.Windows.Forms.Button btnJogP;
        private System.Windows.Forms.Button btnJogR;
        private System.Windows.Forms.GroupBox gbAbsMove;
        private System.Windows.Forms.Label lbTarSpd;
        private System.Windows.Forms.Label lbTarPos;
        private System.Windows.Forms.TextBox txtTarPos;
        private System.Windows.Forms.TextBox txtTarSpd;
        private System.Windows.Forms.Button btnAbsMove;
        private System.Windows.Forms.GroupBox gbPathMove;
        private System.Windows.Forms.Button btnPathMove;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label lbPathIdx;
        private System.Windows.Forms.GroupBox gbTeach;
        private System.Windows.Forms.Button btnTeach;
        private System.Windows.Forms.TextBox txtTeach;
        private System.Windows.Forms.Label lbTeach;
        private System.Windows.Forms.GroupBox gbMotorStt;
        private System.Windows.Forms.Label lbEncErr;
        private System.Windows.Forms.Label lbFatalErr;
        private System.Windows.Forms.Label lbLightErr;
        private System.Windows.Forms.PictureBox picEncErr;
        private System.Windows.Forms.PictureBox picFatalErr;
        private System.Windows.Forms.PictureBox picLightErr;
        private System.Windows.Forms.Label lbCurPos;
        private System.Windows.Forms.TextBox txtCurPos;
        private System.Windows.Forms.Label lbCurSpd;
        private System.Windows.Forms.TextBox txtCurSpd;
        private System.Windows.Forms.Button btnGetState;
        private System.Windows.Forms.NumericUpDown nudPathStart;
        private System.Windows.Forms.NumericUpDown nudPathEnd;
        private System.Windows.Forms.Label lbTo;
        private System.Windows.Forms.Button btnGetPath;
        private System.Windows.Forms.GroupBox gbPath;
        private System.Windows.Forms.DataGridView dgvPos;
        private System.Windows.Forms.Button btnCleanAlarm;
        private System.Windows.Forms.Button btnSetPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIdx;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPosBand;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSpeed;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIndvP;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIndvM;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAccel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDecel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPush;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLoad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCtrl;
    }
}