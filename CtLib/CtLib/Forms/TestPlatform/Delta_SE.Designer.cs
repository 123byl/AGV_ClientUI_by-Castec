namespace CtLib.Forms.TestPlatform {
	/// <summary>提供 Delta PLC (DVP/SE) 等型號之簡易控制</summary>
	partial class Delta_SE {
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
			this.lbIP = new System.Windows.Forms.Label();
			this.txtIP = new System.Windows.Forms.TextBox();
			this.btnConnect = new System.Windows.Forms.Button();
			this.btnDisconnect = new System.Windows.Forms.Button();
			this.lsbxMsg = new System.Windows.Forms.ListBox();
			this.lbXId = new System.Windows.Forms.Label();
			this.nudXId = new System.Windows.Forms.NumericUpDown();
			this.lbXVal = new System.Windows.Forms.Label();
			this.gbX = new System.Windows.Forms.GroupBox();
			this.btnXRead = new System.Windows.Forms.Button();
			this.txtXVal = new System.Windows.Forms.TextBox();
			this.gbD = new System.Windows.Forms.GroupBox();
			this.btnDWrite = new System.Windows.Forms.Button();
			this.btnDRead = new System.Windows.Forms.Button();
			this.txtDVal = new System.Windows.Forms.TextBox();
			this.lbDVal = new System.Windows.Forms.Label();
			this.nudDId = new System.Windows.Forms.NumericUpDown();
			this.lbDId = new System.Windows.Forms.Label();
			this.gbY = new System.Windows.Forms.GroupBox();
			this.cbYVal = new System.Windows.Forms.ComboBox();
			this.btnYWrite = new System.Windows.Forms.Button();
			this.btnYRead = new System.Windows.Forms.Button();
			this.lbYVal = new System.Windows.Forms.Label();
			this.nudYId = new System.Windows.Forms.NumericUpDown();
			this.lbYId = new System.Windows.Forms.Label();
			this.gbM = new System.Windows.Forms.GroupBox();
			this.cbMVal = new System.Windows.Forms.ComboBox();
			this.btnMWrite = new System.Windows.Forms.Button();
			this.btnMRead = new System.Windows.Forms.Button();
			this.lbMVal = new System.Windows.Forms.Label();
			this.nudMId = new System.Windows.Forms.NumericUpDown();
			this.lbMId = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.nudXId)).BeginInit();
			this.gbX.SuspendLayout();
			this.gbD.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudDId)).BeginInit();
			this.gbY.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudYId)).BeginInit();
			this.gbM.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMId)).BeginInit();
			this.SuspendLayout();
			// 
			// lbIP
			// 
			this.lbIP.AutoSize = true;
			this.lbIP.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbIP.Location = new System.Drawing.Point(21, 20);
			this.lbIP.Name = "lbIP";
			this.lbIP.Size = new System.Drawing.Size(86, 18);
			this.lbIP.TabIndex = 0;
			this.lbIP.Text = "IP Address";
			// 
			// txtIP
			// 
			this.txtIP.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtIP.Location = new System.Drawing.Point(113, 18);
			this.txtIP.Name = "txtIP";
			this.txtIP.Size = new System.Drawing.Size(177, 25);
			this.txtIP.TabIndex = 1;
			this.txtIP.Text = "192.168.10.121";
			this.txtIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// btnConnect
			// 
			this.btnConnect.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
			this.btnConnect.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnConnect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnConnect.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnConnect.Location = new System.Drawing.Point(307, 11);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(82, 41);
			this.btnConnect.TabIndex = 2;
			this.btnConnect.Text = "連線";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// btnDisconnect
			// 
			this.btnDisconnect.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
			this.btnDisconnect.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnDisconnect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnDisconnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDisconnect.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnDisconnect.Location = new System.Drawing.Point(395, 11);
			this.btnDisconnect.Name = "btnDisconnect";
			this.btnDisconnect.Size = new System.Drawing.Size(82, 41);
			this.btnDisconnect.TabIndex = 3;
			this.btnDisconnect.Text = "中斷連線";
			this.btnDisconnect.UseVisualStyleBackColor = true;
			this.btnDisconnect.Visible = false;
			this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
			// 
			// lsbxMsg
			// 
			this.lsbxMsg.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lsbxMsg.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.lsbxMsg.FormattingEnabled = true;
			this.lsbxMsg.ItemHeight = 17;
			this.lsbxMsg.Location = new System.Drawing.Point(0, 358);
			this.lsbxMsg.Name = "lsbxMsg";
			this.lsbxMsg.Size = new System.Drawing.Size(690, 123);
			this.lsbxMsg.TabIndex = 4;
			// 
			// lbXId
			// 
			this.lbXId.AutoSize = true;
			this.lbXId.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbXId.Location = new System.Drawing.Point(40, 37);
			this.lbXId.Name = "lbXId";
			this.lbXId.Size = new System.Drawing.Size(39, 19);
			this.lbXId.TabIndex = 5;
			this.lbXId.Text = "編號";
			// 
			// nudXId
			// 
			this.nudXId.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nudXId.Location = new System.Drawing.Point(16, 59);
			this.nudXId.Name = "nudXId";
			this.nudXId.Size = new System.Drawing.Size(91, 25);
			this.nudXId.TabIndex = 6;
			this.nudXId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbXVal
			// 
			this.lbXVal.AutoSize = true;
			this.lbXVal.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbXVal.Location = new System.Drawing.Point(144, 36);
			this.lbXVal.Name = "lbXVal";
			this.lbXVal.Size = new System.Drawing.Size(39, 19);
			this.lbXVal.TabIndex = 9;
			this.lbXVal.Text = "數值";
			// 
			// gbX
			// 
			this.gbX.Controls.Add(this.btnXRead);
			this.gbX.Controls.Add(this.txtXVal);
			this.gbX.Controls.Add(this.lbXVal);
			this.gbX.Controls.Add(this.nudXId);
			this.gbX.Controls.Add(this.lbXId);
			this.gbX.Enabled = false;
			this.gbX.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.gbX.Location = new System.Drawing.Point(12, 75);
			this.gbX.Name = "gbX";
			this.gbX.Size = new System.Drawing.Size(328, 130);
			this.gbX.TabIndex = 11;
			this.gbX.TabStop = false;
			this.gbX.Text = "輸入 :: X";
			// 
			// btnXRead
			// 
			this.btnXRead.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
			this.btnXRead.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnXRead.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnXRead.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnXRead.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnXRead.Location = new System.Drawing.Point(224, 55);
			this.btnXRead.Name = "btnXRead";
			this.btnXRead.Size = new System.Drawing.Size(82, 30);
			this.btnXRead.TabIndex = 12;
			this.btnXRead.Text = "讀取";
			this.btnXRead.UseVisualStyleBackColor = true;
			this.btnXRead.Click += new System.EventHandler(this.btnXRead_Click);
			// 
			// txtXVal
			// 
			this.txtXVal.Enabled = false;
			this.txtXVal.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtXVal.Location = new System.Drawing.Point(113, 59);
			this.txtXVal.Name = "txtXVal";
			this.txtXVal.Size = new System.Drawing.Size(105, 25);
			this.txtXVal.TabIndex = 10;
			this.txtXVal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// gbD
			// 
			this.gbD.Controls.Add(this.btnDWrite);
			this.gbD.Controls.Add(this.btnDRead);
			this.gbD.Controls.Add(this.txtDVal);
			this.gbD.Controls.Add(this.lbDVal);
			this.gbD.Controls.Add(this.nudDId);
			this.gbD.Controls.Add(this.lbDId);
			this.gbD.Enabled = false;
			this.gbD.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.gbD.Location = new System.Drawing.Point(346, 211);
			this.gbD.Name = "gbD";
			this.gbD.Size = new System.Drawing.Size(328, 130);
			this.gbD.TabIndex = 12;
			this.gbD.TabStop = false;
			this.gbD.Text = "暫存器 :: D";
			// 
			// btnDWrite
			// 
			this.btnDWrite.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
			this.btnDWrite.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnDWrite.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnDWrite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDWrite.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnDWrite.Location = new System.Drawing.Point(224, 73);
			this.btnDWrite.Name = "btnDWrite";
			this.btnDWrite.Size = new System.Drawing.Size(82, 30);
			this.btnDWrite.TabIndex = 13;
			this.btnDWrite.Text = "寫入";
			this.btnDWrite.UseVisualStyleBackColor = true;
			this.btnDWrite.Click += new System.EventHandler(this.btnDWrite_Click);
			// 
			// btnDRead
			// 
			this.btnDRead.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
			this.btnDRead.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnDRead.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnDRead.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDRead.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnDRead.Location = new System.Drawing.Point(224, 40);
			this.btnDRead.Name = "btnDRead";
			this.btnDRead.Size = new System.Drawing.Size(82, 30);
			this.btnDRead.TabIndex = 12;
			this.btnDRead.Text = "讀取";
			this.btnDRead.UseVisualStyleBackColor = true;
			this.btnDRead.Click += new System.EventHandler(this.btnDRead_Click);
			// 
			// txtDVal
			// 
			this.txtDVal.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtDVal.Location = new System.Drawing.Point(113, 59);
			this.txtDVal.Name = "txtDVal";
			this.txtDVal.Size = new System.Drawing.Size(105, 25);
			this.txtDVal.TabIndex = 10;
			this.txtDVal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbDVal
			// 
			this.lbDVal.AutoSize = true;
			this.lbDVal.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbDVal.Location = new System.Drawing.Point(144, 36);
			this.lbDVal.Name = "lbDVal";
			this.lbDVal.Size = new System.Drawing.Size(39, 19);
			this.lbDVal.TabIndex = 9;
			this.lbDVal.Text = "數值";
			// 
			// nudDId
			// 
			this.nudDId.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nudDId.Location = new System.Drawing.Point(16, 59);
			this.nudDId.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
			this.nudDId.Name = "nudDId";
			this.nudDId.Size = new System.Drawing.Size(91, 25);
			this.nudDId.TabIndex = 6;
			this.nudDId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbDId
			// 
			this.lbDId.AutoSize = true;
			this.lbDId.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbDId.Location = new System.Drawing.Point(40, 37);
			this.lbDId.Name = "lbDId";
			this.lbDId.Size = new System.Drawing.Size(39, 19);
			this.lbDId.TabIndex = 5;
			this.lbDId.Text = "編號";
			// 
			// gbY
			// 
			this.gbY.Controls.Add(this.cbYVal);
			this.gbY.Controls.Add(this.btnYWrite);
			this.gbY.Controls.Add(this.btnYRead);
			this.gbY.Controls.Add(this.lbYVal);
			this.gbY.Controls.Add(this.nudYId);
			this.gbY.Controls.Add(this.lbYId);
			this.gbY.Enabled = false;
			this.gbY.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.gbY.Location = new System.Drawing.Point(12, 211);
			this.gbY.Name = "gbY";
			this.gbY.Size = new System.Drawing.Size(328, 130);
			this.gbY.TabIndex = 13;
			this.gbY.TabStop = false;
			this.gbY.Text = "輸出 :: Y";
			// 
			// cbYVal
			// 
			this.cbYVal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbYVal.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cbYVal.FormattingEnabled = true;
			this.cbYVal.Items.AddRange(new object[] {
            "On",
            "Off"});
			this.cbYVal.Location = new System.Drawing.Point(113, 58);
			this.cbYVal.Name = "cbYVal";
			this.cbYVal.Size = new System.Drawing.Size(105, 26);
			this.cbYVal.TabIndex = 14;
			// 
			// btnYWrite
			// 
			this.btnYWrite.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
			this.btnYWrite.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnYWrite.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnYWrite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnYWrite.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnYWrite.Location = new System.Drawing.Point(224, 72);
			this.btnYWrite.Name = "btnYWrite";
			this.btnYWrite.Size = new System.Drawing.Size(82, 30);
			this.btnYWrite.TabIndex = 14;
			this.btnYWrite.Text = "寫入";
			this.btnYWrite.UseVisualStyleBackColor = true;
			this.btnYWrite.Click += new System.EventHandler(this.btnYWrite_Click);
			// 
			// btnYRead
			// 
			this.btnYRead.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
			this.btnYRead.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnYRead.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnYRead.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnYRead.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnYRead.Location = new System.Drawing.Point(224, 39);
			this.btnYRead.Name = "btnYRead";
			this.btnYRead.Size = new System.Drawing.Size(82, 30);
			this.btnYRead.TabIndex = 12;
			this.btnYRead.Text = "讀取";
			this.btnYRead.UseVisualStyleBackColor = true;
			this.btnYRead.Click += new System.EventHandler(this.btnYRead_Click);
			// 
			// lbYVal
			// 
			this.lbYVal.AutoSize = true;
			this.lbYVal.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbYVal.Location = new System.Drawing.Point(144, 36);
			this.lbYVal.Name = "lbYVal";
			this.lbYVal.Size = new System.Drawing.Size(39, 19);
			this.lbYVal.TabIndex = 9;
			this.lbYVal.Text = "數值";
			// 
			// nudYId
			// 
			this.nudYId.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nudYId.Location = new System.Drawing.Point(16, 59);
			this.nudYId.Name = "nudYId";
			this.nudYId.Size = new System.Drawing.Size(91, 25);
			this.nudYId.TabIndex = 6;
			this.nudYId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbYId
			// 
			this.lbYId.AutoSize = true;
			this.lbYId.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbYId.Location = new System.Drawing.Point(40, 37);
			this.lbYId.Name = "lbYId";
			this.lbYId.Size = new System.Drawing.Size(39, 19);
			this.lbYId.TabIndex = 5;
			this.lbYId.Text = "編號";
			// 
			// gbM
			// 
			this.gbM.Controls.Add(this.cbMVal);
			this.gbM.Controls.Add(this.btnMWrite);
			this.gbM.Controls.Add(this.btnMRead);
			this.gbM.Controls.Add(this.lbMVal);
			this.gbM.Controls.Add(this.nudMId);
			this.gbM.Controls.Add(this.lbMId);
			this.gbM.Enabled = false;
			this.gbM.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.gbM.Location = new System.Drawing.Point(346, 75);
			this.gbM.Name = "gbM";
			this.gbM.Size = new System.Drawing.Size(328, 130);
			this.gbM.TabIndex = 14;
			this.gbM.TabStop = false;
			this.gbM.Text = "輔助繼電器 :: M";
			// 
			// cbMVal
			// 
			this.cbMVal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbMVal.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cbMVal.FormattingEnabled = true;
			this.cbMVal.Items.AddRange(new object[] {
            "On",
            "Off"});
			this.cbMVal.Location = new System.Drawing.Point(113, 58);
			this.cbMVal.Name = "cbMVal";
			this.cbMVal.Size = new System.Drawing.Size(105, 26);
			this.cbMVal.TabIndex = 14;
			// 
			// btnMWrite
			// 
			this.btnMWrite.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
			this.btnMWrite.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnMWrite.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnMWrite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMWrite.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnMWrite.Location = new System.Drawing.Point(224, 72);
			this.btnMWrite.Name = "btnMWrite";
			this.btnMWrite.Size = new System.Drawing.Size(82, 30);
			this.btnMWrite.TabIndex = 14;
			this.btnMWrite.Text = "寫入";
			this.btnMWrite.UseVisualStyleBackColor = true;
			this.btnMWrite.Click += new System.EventHandler(this.btnMWrite_Click);
			// 
			// btnMRead
			// 
			this.btnMRead.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
			this.btnMRead.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
			this.btnMRead.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
			this.btnMRead.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMRead.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnMRead.Location = new System.Drawing.Point(224, 39);
			this.btnMRead.Name = "btnMRead";
			this.btnMRead.Size = new System.Drawing.Size(82, 30);
			this.btnMRead.TabIndex = 12;
			this.btnMRead.Text = "讀取";
			this.btnMRead.UseVisualStyleBackColor = true;
			this.btnMRead.Click += new System.EventHandler(this.btnMRead_Click);
			// 
			// lbMVal
			// 
			this.lbMVal.AutoSize = true;
			this.lbMVal.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbMVal.Location = new System.Drawing.Point(144, 36);
			this.lbMVal.Name = "lbMVal";
			this.lbMVal.Size = new System.Drawing.Size(39, 19);
			this.lbMVal.TabIndex = 9;
			this.lbMVal.Text = "數值";
			// 
			// nudMId
			// 
			this.nudMId.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nudMId.Location = new System.Drawing.Point(16, 59);
			this.nudMId.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
			this.nudMId.Name = "nudMId";
			this.nudMId.Size = new System.Drawing.Size(91, 25);
			this.nudMId.TabIndex = 6;
			this.nudMId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbMId
			// 
			this.lbMId.AutoSize = true;
			this.lbMId.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbMId.Location = new System.Drawing.Point(40, 37);
			this.lbMId.Name = "lbMId";
			this.lbMId.Size = new System.Drawing.Size(39, 19);
			this.lbMId.TabIndex = 5;
			this.lbMId.Text = "編號";
			// 
			// Delta_SE
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(690, 481);
			this.Controls.Add(this.gbM);
			this.Controls.Add(this.gbY);
			this.Controls.Add(this.gbD);
			this.Controls.Add(this.gbX);
			this.Controls.Add(this.lsbxMsg);
			this.Controls.Add(this.btnDisconnect);
			this.Controls.Add(this.btnConnect);
			this.Controls.Add(this.txtIP);
			this.Controls.Add(this.lbIP);
			this.Name = "Delta_SE";
			this.Text = "Delta SE Control";
			((System.ComponentModel.ISupportInitialize)(this.nudXId)).EndInit();
			this.gbX.ResumeLayout(false);
			this.gbX.PerformLayout();
			this.gbD.ResumeLayout(false);
			this.gbD.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudDId)).EndInit();
			this.gbY.ResumeLayout(false);
			this.gbY.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudYId)).EndInit();
			this.gbM.ResumeLayout(false);
			this.gbM.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMId)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lbIP;
		private System.Windows.Forms.TextBox txtIP;
		private System.Windows.Forms.Button btnConnect;
		private System.Windows.Forms.Button btnDisconnect;
		private System.Windows.Forms.ListBox lsbxMsg;
		private System.Windows.Forms.Label lbXId;
		private System.Windows.Forms.NumericUpDown nudXId;
		private System.Windows.Forms.Label lbXVal;
		private System.Windows.Forms.GroupBox gbX;
		private System.Windows.Forms.Button btnXRead;
		private System.Windows.Forms.TextBox txtXVal;
		private System.Windows.Forms.GroupBox gbD;
		private System.Windows.Forms.Button btnDWrite;
		private System.Windows.Forms.Button btnDRead;
		private System.Windows.Forms.TextBox txtDVal;
		private System.Windows.Forms.Label lbDVal;
		private System.Windows.Forms.NumericUpDown nudDId;
		private System.Windows.Forms.Label lbDId;
		private System.Windows.Forms.GroupBox gbY;
		private System.Windows.Forms.ComboBox cbYVal;
		private System.Windows.Forms.Button btnYWrite;
		private System.Windows.Forms.Button btnYRead;
		private System.Windows.Forms.Label lbYVal;
		private System.Windows.Forms.NumericUpDown nudYId;
		private System.Windows.Forms.Label lbYId;
		private System.Windows.Forms.GroupBox gbM;
		private System.Windows.Forms.ComboBox cbMVal;
		private System.Windows.Forms.Button btnMWrite;
		private System.Windows.Forms.Button btnMRead;
		private System.Windows.Forms.Label lbMVal;
		private System.Windows.Forms.NumericUpDown nudMId;
		private System.Windows.Forms.Label lbMId;
	}
}

