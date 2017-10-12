namespace CtLib.Forms.TestPlatform {
    partial class Test_WagoIO {
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
			this.txtIP = new System.Windows.Forms.TextBox();
			this.txtPort = new System.Windows.Forms.TextBox();
			this.btnConnect = new System.Windows.Forms.Button();
			this.picConnect = new System.Windows.Forms.PictureBox();
			this.btnGetCoil = new System.Windows.Forms.Button();
			this.txtGetMutiAddr = new System.Windows.Forms.TextBox();
			this.richInfo = new System.Windows.Forms.RichTextBox();
			this.gbGet = new System.Windows.Forms.GroupBox();
			this.lbMultiCount = new System.Windows.Forms.Label();
			this.lbMultiNum = new System.Windows.Forms.Label();
			this.txtGetMultiCount = new System.Windows.Forms.TextBox();
			this.btnGetRegs = new System.Windows.Forms.Button();
			this.btnGetCoils = new System.Windows.Forms.Button();
			this.btnGetReg = new System.Windows.Forms.Button();
			this.btnSetRegs = new System.Windows.Forms.Button();
			this.btnSetReg = new System.Windows.Forms.Button();
			this.btnSetCoils = new System.Windows.Forms.Button();
			this.btnSetCoil = new System.Windows.Forms.Button();
			this.btnRun = new System.Windows.Forms.Button();
			this.gbGetSig = new System.Windows.Forms.GroupBox();
			this.lbSigNum = new System.Windows.Forms.Label();
			this.txtGetSigAddr = new System.Windows.Forms.TextBox();
			this.gbSetSigIO = new System.Windows.Forms.GroupBox();
			this.cbSetSig = new System.Windows.Forms.ComboBox();
			this.lbSetSigIOStt = new System.Windows.Forms.Label();
			this.lbSetSigIOAddr = new System.Windows.Forms.Label();
			this.txtSetSigIOAddr = new System.Windows.Forms.TextBox();
			this.gbSetSigReg = new System.Windows.Forms.GroupBox();
			this.txtSetSigRegVal = new System.Windows.Forms.TextBox();
			this.lbSetSigRegVal = new System.Windows.Forms.Label();
			this.lbSetSigRegAddr = new System.Windows.Forms.Label();
			this.txtSetSigRegAddr = new System.Windows.Forms.TextBox();
			this.gbSetMulti = new System.Windows.Forms.GroupBox();
			this.txtSetMultiVal = new System.Windows.Forms.TextBox();
			this.lbSetMultiVal = new System.Windows.Forms.Label();
			this.lbSetMultiAddr = new System.Windows.Forms.Label();
			this.txtSetMultiAddr = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.picConnect)).BeginInit();
			this.gbGet.SuspendLayout();
			this.gbGetSig.SuspendLayout();
			this.gbSetSigIO.SuspendLayout();
			this.gbSetSigReg.SuspendLayout();
			this.gbSetMulti.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtIP
			// 
			this.txtIP.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtIP.Location = new System.Drawing.Point(33, 31);
			this.txtIP.Name = "txtIP";
			this.txtIP.Size = new System.Drawing.Size(137, 23);
			this.txtIP.TabIndex = 0;
			this.txtIP.Text = "192.168.1.1";
			this.txtIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtPort
			// 
			this.txtPort.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtPort.Location = new System.Drawing.Point(176, 31);
			this.txtPort.Name = "txtPort";
			this.txtPort.Size = new System.Drawing.Size(70, 23);
			this.txtPort.TabIndex = 1;
			this.txtPort.Text = "502";
			this.txtPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// btnConnect
			// 
			this.btnConnect.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnConnect.Location = new System.Drawing.Point(258, 31);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(82, 23);
			this.btnConnect.TabIndex = 2;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// picConnect
			// 
			this.picConnect.Image = global::CtLib.Properties.Resources.Grey_Ball;
			this.picConnect.Location = new System.Drawing.Point(349, 28);
			this.picConnect.Name = "picConnect";
			this.picConnect.Size = new System.Drawing.Size(28, 28);
			this.picConnect.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.picConnect.TabIndex = 3;
			this.picConnect.TabStop = false;
			// 
			// btnGetCoil
			// 
			this.btnGetCoil.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnGetCoil.Location = new System.Drawing.Point(200, 28);
			this.btnGetCoil.Name = "btnGetCoil";
			this.btnGetCoil.Size = new System.Drawing.Size(107, 23);
			this.btnGetCoil.TabIndex = 4;
			this.btnGetCoil.Text = "Get Coil";
			this.btnGetCoil.UseVisualStyleBackColor = true;
			this.btnGetCoil.Click += new System.EventHandler(this.btnGetCoil_Click);
			// 
			// txtGetMutiAddr
			// 
			this.txtGetMutiAddr.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtGetMutiAddr.Location = new System.Drawing.Point(11, 51);
			this.txtGetMutiAddr.Name = "txtGetMutiAddr";
			this.txtGetMutiAddr.Size = new System.Drawing.Size(95, 23);
			this.txtGetMutiAddr.TabIndex = 6;
			this.txtGetMutiAddr.Text = "512";
			this.txtGetMutiAddr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// richInfo
			// 
			this.richInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.richInfo.Location = new System.Drawing.Point(0, 486);
			this.richInfo.Name = "richInfo";
			this.richInfo.Size = new System.Drawing.Size(784, 313);
			this.richInfo.TabIndex = 8;
			this.richInfo.Text = "";
			// 
			// gbGet
			// 
			this.gbGet.Controls.Add(this.lbMultiCount);
			this.gbGet.Controls.Add(this.lbMultiNum);
			this.gbGet.Controls.Add(this.txtGetMultiCount);
			this.gbGet.Controls.Add(this.btnGetRegs);
			this.gbGet.Controls.Add(this.txtGetMutiAddr);
			this.gbGet.Controls.Add(this.btnGetCoils);
			this.gbGet.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbGet.Location = new System.Drawing.Point(33, 182);
			this.gbGet.Name = "gbGet";
			this.gbGet.Size = new System.Drawing.Size(344, 140);
			this.gbGet.TabIndex = 9;
			this.gbGet.TabStop = false;
			this.gbGet.Text = "Get Multi IOs/Registers";
			// 
			// lbMultiCount
			// 
			this.lbMultiCount.AutoSize = true;
			this.lbMultiCount.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbMultiCount.Location = new System.Drawing.Point(8, 80);
			this.lbMultiCount.Name = "lbMultiCount";
			this.lbMultiCount.Size = new System.Drawing.Size(218, 16);
			this.lbMultiCount.TabIndex = 16;
			this.lbMultiCount.Text = "Continuous IOs/Registers Count";
			// 
			// lbMultiNum
			// 
			this.lbMultiNum.AutoSize = true;
			this.lbMultiNum.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbMultiNum.Location = new System.Drawing.Point(8, 32);
			this.lbMultiNum.Name = "lbMultiNum";
			this.lbMultiNum.Size = new System.Drawing.Size(214, 16);
			this.lbMultiNum.TabIndex = 15;
			this.lbMultiNum.Text = "I/O or Register Starter Address";
			// 
			// txtGetMultiCount
			// 
			this.txtGetMultiCount.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtGetMultiCount.Location = new System.Drawing.Point(11, 98);
			this.txtGetMultiCount.Name = "txtGetMultiCount";
			this.txtGetMultiCount.Size = new System.Drawing.Size(95, 23);
			this.txtGetMultiCount.TabIndex = 10;
			this.txtGetMultiCount.Text = "1";
			this.txtGetMultiCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// btnGetRegs
			// 
			this.btnGetRegs.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnGetRegs.Location = new System.Drawing.Point(228, 83);
			this.btnGetRegs.Name = "btnGetRegs";
			this.btnGetRegs.Size = new System.Drawing.Size(107, 23);
			this.btnGetRegs.TabIndex = 9;
			this.btnGetRegs.Text = "Get Registers";
			this.btnGetRegs.UseVisualStyleBackColor = true;
			this.btnGetRegs.Click += new System.EventHandler(this.btnGetRegs_Click);
			// 
			// btnGetCoils
			// 
			this.btnGetCoils.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnGetCoils.Location = new System.Drawing.Point(228, 54);
			this.btnGetCoils.Name = "btnGetCoils";
			this.btnGetCoils.Size = new System.Drawing.Size(107, 23);
			this.btnGetCoils.TabIndex = 8;
			this.btnGetCoils.Text = "Get Coils";
			this.btnGetCoils.UseVisualStyleBackColor = true;
			this.btnGetCoils.Click += new System.EventHandler(this.btnGetCoils_Click);
			// 
			// btnGetReg
			// 
			this.btnGetReg.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnGetReg.Location = new System.Drawing.Point(200, 57);
			this.btnGetReg.Name = "btnGetReg";
			this.btnGetReg.Size = new System.Drawing.Size(107, 23);
			this.btnGetReg.TabIndex = 7;
			this.btnGetReg.Text = "Get Register";
			this.btnGetReg.UseVisualStyleBackColor = true;
			this.btnGetReg.Click += new System.EventHandler(this.btnGetReg_Click);
			// 
			// btnSetRegs
			// 
			this.btnSetRegs.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSetRegs.Location = new System.Drawing.Point(213, 86);
			this.btnSetRegs.Name = "btnSetRegs";
			this.btnSetRegs.Size = new System.Drawing.Size(107, 23);
			this.btnSetRegs.TabIndex = 9;
			this.btnSetRegs.Text = "Set Registers";
			this.btnSetRegs.UseVisualStyleBackColor = true;
			this.btnSetRegs.Click += new System.EventHandler(this.btnSetRegs_Click);
			// 
			// btnSetReg
			// 
			this.btnSetReg.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSetReg.Location = new System.Drawing.Point(213, 72);
			this.btnSetReg.Name = "btnSetReg";
			this.btnSetReg.Size = new System.Drawing.Size(107, 23);
			this.btnSetReg.TabIndex = 7;
			this.btnSetReg.Text = "Set Register";
			this.btnSetReg.UseVisualStyleBackColor = true;
			this.btnSetReg.Click += new System.EventHandler(this.btnSetReg_Click);
			// 
			// btnSetCoils
			// 
			this.btnSetCoils.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSetCoils.Location = new System.Drawing.Point(213, 57);
			this.btnSetCoils.Name = "btnSetCoils";
			this.btnSetCoils.Size = new System.Drawing.Size(107, 23);
			this.btnSetCoils.TabIndex = 8;
			this.btnSetCoils.Text = "Set Coils";
			this.btnSetCoils.UseVisualStyleBackColor = true;
			this.btnSetCoils.Click += new System.EventHandler(this.btnSetCoils_Click);
			// 
			// btnSetCoil
			// 
			this.btnSetCoil.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSetCoil.Location = new System.Drawing.Point(213, 48);
			this.btnSetCoil.Name = "btnSetCoil";
			this.btnSetCoil.Size = new System.Drawing.Size(107, 23);
			this.btnSetCoil.TabIndex = 4;
			this.btnSetCoil.Text = "Set Coil";
			this.btnSetCoil.UseVisualStyleBackColor = true;
			this.btnSetCoil.Click += new System.EventHandler(this.btnSetCoil_Click);
			// 
			// btnRun
			// 
			this.btnRun.Location = new System.Drawing.Point(33, 352);
			this.btnRun.Name = "btnRun";
			this.btnRun.Size = new System.Drawing.Size(81, 23);
			this.btnRun.TabIndex = 11;
			this.btnRun.Text = "Bomb!";
			this.btnRun.UseVisualStyleBackColor = true;
			this.btnRun.Visible = false;
			this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
			// 
			// gbGetSig
			// 
			this.gbGetSig.Controls.Add(this.lbSigNum);
			this.gbGetSig.Controls.Add(this.txtGetSigAddr);
			this.gbGetSig.Controls.Add(this.btnGetReg);
			this.gbGetSig.Controls.Add(this.btnGetCoil);
			this.gbGetSig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbGetSig.Location = new System.Drawing.Point(33, 81);
			this.gbGetSig.Name = "gbGetSig";
			this.gbGetSig.Size = new System.Drawing.Size(344, 95);
			this.gbGetSig.TabIndex = 12;
			this.gbGetSig.TabStop = false;
			this.gbGetSig.Text = "Get Single IO/Register";
			// 
			// lbSigNum
			// 
			this.lbSigNum.AutoSize = true;
			this.lbSigNum.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbSigNum.Location = new System.Drawing.Point(8, 31);
			this.lbSigNum.Name = "lbSigNum";
			this.lbSigNum.Size = new System.Drawing.Size(162, 16);
			this.lbSigNum.TabIndex = 14;
			this.lbSigNum.Text = "I/O or Register Address";
			// 
			// txtGetSigAddr
			// 
			this.txtGetSigAddr.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtGetSigAddr.Location = new System.Drawing.Point(11, 49);
			this.txtGetSigAddr.Name = "txtGetSigAddr";
			this.txtGetSigAddr.Size = new System.Drawing.Size(95, 23);
			this.txtGetSigAddr.TabIndex = 13;
			this.txtGetSigAddr.Text = "0";
			this.txtGetSigAddr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// gbSetSigIO
			// 
			this.gbSetSigIO.Controls.Add(this.cbSetSig);
			this.gbSetSigIO.Controls.Add(this.lbSetSigIOStt);
			this.gbSetSigIO.Controls.Add(this.lbSetSigIOAddr);
			this.gbSetSigIO.Controls.Add(this.txtSetSigIOAddr);
			this.gbSetSigIO.Controls.Add(this.btnSetCoil);
			this.gbSetSigIO.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbSetSigIO.Location = new System.Drawing.Point(414, 81);
			this.gbSetSigIO.Name = "gbSetSigIO";
			this.gbSetSigIO.Size = new System.Drawing.Size(344, 95);
			this.gbSetSigIO.TabIndex = 13;
			this.gbSetSigIO.TabStop = false;
			this.gbSetSigIO.Text = "Set Single IO";
			// 
			// cbSetSig
			// 
			this.cbSetSig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSetSig.FormattingEnabled = true;
			this.cbSetSig.Items.AddRange(new object[] {
            "OFF",
            "ON"});
			this.cbSetSig.Location = new System.Drawing.Point(125, 48);
			this.cbSetSig.Name = "cbSetSig";
			this.cbSetSig.Size = new System.Drawing.Size(62, 24);
			this.cbSetSig.TabIndex = 21;
			// 
			// lbSetSigIOStt
			// 
			this.lbSetSigIOStt.AutoSize = true;
			this.lbSetSigIOStt.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbSetSigIOStt.Location = new System.Drawing.Point(124, 29);
			this.lbSetSigIOStt.Name = "lbSetSigIOStt";
			this.lbSetSigIOStt.Size = new System.Drawing.Size(52, 16);
			this.lbSetSigIOStt.TabIndex = 20;
			this.lbSetSigIOStt.Text = "Status";
			// 
			// lbSetSigIOAddr
			// 
			this.lbSetSigIOAddr.AutoSize = true;
			this.lbSetSigIOAddr.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbSetSigIOAddr.Location = new System.Drawing.Point(21, 29);
			this.lbSetSigIOAddr.Name = "lbSetSigIOAddr";
			this.lbSetSigIOAddr.Size = new System.Drawing.Size(86, 16);
			this.lbSetSigIOAddr.TabIndex = 19;
			this.lbSetSigIOAddr.Text = "I/O Address";
			// 
			// txtSetSigIOAddr
			// 
			this.txtSetSigIOAddr.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSetSigIOAddr.Location = new System.Drawing.Point(24, 48);
			this.txtSetSigIOAddr.Name = "txtSetSigIOAddr";
			this.txtSetSigIOAddr.Size = new System.Drawing.Size(95, 23);
			this.txtSetSigIOAddr.TabIndex = 17;
			this.txtSetSigIOAddr.Text = "512";
			this.txtSetSigIOAddr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// gbSetSigReg
			// 
			this.gbSetSigReg.Controls.Add(this.txtSetSigRegVal);
			this.gbSetSigReg.Controls.Add(this.lbSetSigRegVal);
			this.gbSetSigReg.Controls.Add(this.btnSetReg);
			this.gbSetSigReg.Controls.Add(this.lbSetSigRegAddr);
			this.gbSetSigReg.Controls.Add(this.txtSetSigRegAddr);
			this.gbSetSigReg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbSetSigReg.Location = new System.Drawing.Point(414, 182);
			this.gbSetSigReg.Name = "gbSetSigReg";
			this.gbSetSigReg.Size = new System.Drawing.Size(344, 140);
			this.gbSetSigReg.TabIndex = 14;
			this.gbSetSigReg.TabStop = false;
			this.gbSetSigReg.Text = "Set Single Register";
			// 
			// txtSetSigRegVal
			// 
			this.txtSetSigRegVal.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSetSigRegVal.Location = new System.Drawing.Point(24, 100);
			this.txtSetSigRegVal.Name = "txtSetSigRegVal";
			this.txtSetSigRegVal.Size = new System.Drawing.Size(95, 23);
			this.txtSetSigRegVal.TabIndex = 21;
			this.txtSetSigRegVal.Text = "65535";
			this.txtSetSigRegVal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbSetSigRegVal
			// 
			this.lbSetSigRegVal.AutoSize = true;
			this.lbSetSigRegVal.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbSetSigRegVal.Location = new System.Drawing.Point(21, 82);
			this.lbSetSigRegVal.Name = "lbSetSigRegVal";
			this.lbSetSigRegVal.Size = new System.Drawing.Size(106, 16);
			this.lbSetSigRegVal.TabIndex = 20;
			this.lbSetSigRegVal.Text = "Data (Decimal)";
			// 
			// lbSetSigRegAddr
			// 
			this.lbSetSigRegAddr.AutoSize = true;
			this.lbSetSigRegAddr.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbSetSigRegAddr.Location = new System.Drawing.Point(21, 32);
			this.lbSetSigRegAddr.Name = "lbSetSigRegAddr";
			this.lbSetSigRegAddr.Size = new System.Drawing.Size(118, 16);
			this.lbSetSigRegAddr.TabIndex = 19;
			this.lbSetSigRegAddr.Text = "Register Address";
			// 
			// txtSetSigRegAddr
			// 
			this.txtSetSigRegAddr.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSetSigRegAddr.Location = new System.Drawing.Point(24, 51);
			this.txtSetSigRegAddr.Name = "txtSetSigRegAddr";
			this.txtSetSigRegAddr.Size = new System.Drawing.Size(95, 23);
			this.txtSetSigRegAddr.TabIndex = 17;
			this.txtSetSigRegAddr.Text = "512";
			this.txtSetSigRegAddr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// gbSetMulti
			// 
			this.gbSetMulti.Controls.Add(this.txtSetMultiVal);
			this.gbSetMulti.Controls.Add(this.btnSetRegs);
			this.gbSetMulti.Controls.Add(this.lbSetMultiVal);
			this.gbSetMulti.Controls.Add(this.lbSetMultiAddr);
			this.gbSetMulti.Controls.Add(this.btnSetCoils);
			this.gbSetMulti.Controls.Add(this.txtSetMultiAddr);
			this.gbSetMulti.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbSetMulti.Location = new System.Drawing.Point(414, 330);
			this.gbSetMulti.Name = "gbSetMulti";
			this.gbSetMulti.Size = new System.Drawing.Size(344, 140);
			this.gbSetMulti.TabIndex = 15;
			this.gbSetMulti.TabStop = false;
			this.gbSetMulti.Text = "Set Multi I/Os/Registers";
			// 
			// txtSetMultiVal
			// 
			this.txtSetMultiVal.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSetMultiVal.Location = new System.Drawing.Point(24, 95);
			this.txtSetMultiVal.Name = "txtSetMultiVal";
			this.txtSetMultiVal.Size = new System.Drawing.Size(163, 23);
			this.txtSetMultiVal.TabIndex = 21;
			this.txtSetMultiVal.Text = "0,1,1,0";
			this.txtSetMultiVal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbSetMultiVal
			// 
			this.lbSetMultiVal.AutoSize = true;
			this.lbSetMultiVal.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbSetMultiVal.Location = new System.Drawing.Point(21, 77);
			this.lbSetMultiVal.Name = "lbSetMultiVal";
			this.lbSetMultiVal.Size = new System.Drawing.Size(191, 16);
			this.lbSetMultiVal.TabIndex = 20;
			this.lbSetMultiVal.Text = "Data (split by \",\") (Decimal)";
			// 
			// lbSetMultiAddr
			// 
			this.lbSetMultiAddr.AutoSize = true;
			this.lbSetMultiAddr.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbSetMultiAddr.Location = new System.Drawing.Point(21, 27);
			this.lbSetMultiAddr.Name = "lbSetMultiAddr";
			this.lbSetMultiAddr.Size = new System.Drawing.Size(214, 16);
			this.lbSetMultiAddr.TabIndex = 19;
			this.lbSetMultiAddr.Text = "I/O or Register Starter Address";
			// 
			// txtSetMultiAddr
			// 
			this.txtSetMultiAddr.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSetMultiAddr.Location = new System.Drawing.Point(24, 46);
			this.txtSetMultiAddr.Name = "txtSetMultiAddr";
			this.txtSetMultiAddr.Size = new System.Drawing.Size(95, 23);
			this.txtSetMultiAddr.TabIndex = 17;
			this.txtSetMultiAddr.Text = "512";
			this.txtSetMultiAddr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Test_WagoIO
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 799);
			this.Controls.Add(this.gbSetMulti);
			this.Controls.Add(this.gbSetSigReg);
			this.Controls.Add(this.gbSetSigIO);
			this.Controls.Add(this.gbGetSig);
			this.Controls.Add(this.btnRun);
			this.Controls.Add(this.gbGet);
			this.Controls.Add(this.richInfo);
			this.Controls.Add(this.picConnect);
			this.Controls.Add(this.btnConnect);
			this.Controls.Add(this.txtPort);
			this.Controls.Add(this.txtIP);
			this.Name = "Test_WagoIO";
			this.Text = "WagoIO";
			((System.ComponentModel.ISupportInitialize)(this.picConnect)).EndInit();
			this.gbGet.ResumeLayout(false);
			this.gbGet.PerformLayout();
			this.gbGetSig.ResumeLayout(false);
			this.gbGetSig.PerformLayout();
			this.gbSetSigIO.ResumeLayout(false);
			this.gbSetSigIO.PerformLayout();
			this.gbSetSigReg.ResumeLayout(false);
			this.gbSetSigReg.PerformLayout();
			this.gbSetMulti.ResumeLayout(false);
			this.gbSetMulti.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.PictureBox picConnect;
        private System.Windows.Forms.Button btnGetCoil;
        private System.Windows.Forms.TextBox txtGetMutiAddr;
        private System.Windows.Forms.RichTextBox richInfo;
        private System.Windows.Forms.GroupBox gbGet;
        private System.Windows.Forms.TextBox txtGetMultiCount;
        private System.Windows.Forms.Button btnGetRegs;
        private System.Windows.Forms.Button btnGetReg;
        private System.Windows.Forms.Button btnGetCoils;
        private System.Windows.Forms.Button btnSetRegs;
        private System.Windows.Forms.Button btnSetReg;
        private System.Windows.Forms.Button btnSetCoils;
        private System.Windows.Forms.Button btnSetCoil;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.GroupBox gbGetSig;
        private System.Windows.Forms.Label lbSigNum;
        private System.Windows.Forms.TextBox txtGetSigAddr;
        private System.Windows.Forms.Label lbMultiCount;
        private System.Windows.Forms.Label lbMultiNum;
        private System.Windows.Forms.GroupBox gbSetSigIO;
        private System.Windows.Forms.Label lbSetSigIOAddr;
        private System.Windows.Forms.TextBox txtSetSigIOAddr;
        private System.Windows.Forms.Label lbSetSigIOStt;
        private System.Windows.Forms.ComboBox cbSetSig;
        private System.Windows.Forms.GroupBox gbSetSigReg;
        private System.Windows.Forms.Label lbSetSigRegVal;
        private System.Windows.Forms.Label lbSetSigRegAddr;
        private System.Windows.Forms.TextBox txtSetSigRegAddr;
        private System.Windows.Forms.TextBox txtSetSigRegVal;
        private System.Windows.Forms.GroupBox gbSetMulti;
        private System.Windows.Forms.TextBox txtSetMultiVal;
        private System.Windows.Forms.Label lbSetMultiVal;
        private System.Windows.Forms.Label lbSetMultiAddr;
        private System.Windows.Forms.TextBox txtSetMultiAddr;
    }
}