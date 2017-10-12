namespace CtLib.Forms.TestPlatform {
    partial class DimmerCtrl {
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
			this.btnConnect = new System.Windows.Forms.Button();
			this.picConStt = new System.Windows.Forms.PictureBox();
			this.gbCH1 = new System.Windows.Forms.GroupBox();
			this.chkCh1Lock = new System.Windows.Forms.CheckBox();
			this.btnCh1Get = new System.Windows.Forms.Button();
			this.btnCh1Set = new System.Windows.Forms.Button();
			this.nudCh1 = new System.Windows.Forms.NumericUpDown();
			this.gbCH2 = new System.Windows.Forms.GroupBox();
			this.chkCh2Lock = new System.Windows.Forms.CheckBox();
			this.btnCh2Get = new System.Windows.Forms.Button();
			this.btnCh2Set = new System.Windows.Forms.Button();
			this.nudCh2 = new System.Windows.Forms.NumericUpDown();
			this.gbCH4 = new System.Windows.Forms.GroupBox();
			this.chkCh4Lock = new System.Windows.Forms.CheckBox();
			this.btnCh4Get = new System.Windows.Forms.Button();
			this.btnCh4Set = new System.Windows.Forms.Button();
			this.nudCh4 = new System.Windows.Forms.NumericUpDown();
			this.gbCH3 = new System.Windows.Forms.GroupBox();
			this.chkCh3Lock = new System.Windows.Forms.CheckBox();
			this.btnCh3Get = new System.Windows.Forms.Button();
			this.btnCh3Set = new System.Windows.Forms.Button();
			this.nudCh3 = new System.Windows.Forms.NumericUpDown();
			this.btnSetAll = new System.Windows.Forms.Button();
			this.btnGetAll = new System.Windows.Forms.Button();
			this.lbVersion = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.picConStt)).BeginInit();
			this.gbCH1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCh1)).BeginInit();
			this.gbCH2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCh2)).BeginInit();
			this.gbCH4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCh4)).BeginInit();
			this.gbCH3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCh3)).BeginInit();
			this.SuspendLayout();
			// 
			// btnConnect
			// 
			this.btnConnect.Font = new System.Drawing.Font("Cambria", 12F);
			this.btnConnect.Location = new System.Drawing.Point(26, 24);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(112, 46);
			this.btnConnect.TabIndex = 0;
			this.btnConnect.Text = "Connnect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// picConStt
			// 
			this.picConStt.Image = global::CtLib.Properties.Resources.Grey_Ball;
			this.picConStt.Location = new System.Drawing.Point(146, 32);
			this.picConStt.Name = "picConStt";
			this.picConStt.Size = new System.Drawing.Size(30, 30);
			this.picConStt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.picConStt.TabIndex = 1;
			this.picConStt.TabStop = false;
			// 
			// gbCH1
			// 
			this.gbCH1.Controls.Add(this.chkCh1Lock);
			this.gbCH1.Controls.Add(this.btnCh1Get);
			this.gbCH1.Controls.Add(this.btnCh1Set);
			this.gbCH1.Controls.Add(this.nudCh1);
			this.gbCH1.Enabled = false;
			this.gbCH1.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbCH1.Location = new System.Drawing.Point(26, 88);
			this.gbCH1.Name = "gbCH1";
			this.gbCH1.Size = new System.Drawing.Size(195, 116);
			this.gbCH1.TabIndex = 2;
			this.gbCH1.TabStop = false;
			this.gbCH1.Text = "Channel 1";
			// 
			// chkCh1Lock
			// 
			this.chkCh1Lock.AutoSize = true;
			this.chkCh1Lock.Location = new System.Drawing.Point(18, 74);
			this.chkCh1Lock.Name = "chkCh1Lock";
			this.chkCh1Lock.Size = new System.Drawing.Size(61, 23);
			this.chkCh1Lock.TabIndex = 3;
			this.chkCh1Lock.Tag = "0";
			this.chkCh1Lock.Text = "Lock";
			this.chkCh1Lock.UseVisualStyleBackColor = true;
			this.chkCh1Lock.CheckedChanged += new System.EventHandler(this.LockLights);
			// 
			// btnCh1Get
			// 
			this.btnCh1Get.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCh1Get.Location = new System.Drawing.Point(101, 64);
			this.btnCh1Get.Name = "btnCh1Get";
			this.btnCh1Get.Size = new System.Drawing.Size(75, 33);
			this.btnCh1Get.TabIndex = 2;
			this.btnCh1Get.Tag = "0";
			this.btnCh1Get.Text = "Get";
			this.btnCh1Get.UseVisualStyleBackColor = true;
			this.btnCh1Get.Click += new System.EventHandler(this.GetLights);
			// 
			// btnCh1Set
			// 
			this.btnCh1Set.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCh1Set.Location = new System.Drawing.Point(101, 25);
			this.btnCh1Set.Name = "btnCh1Set";
			this.btnCh1Set.Size = new System.Drawing.Size(75, 33);
			this.btnCh1Set.TabIndex = 1;
			this.btnCh1Set.Tag = "0";
			this.btnCh1Set.Text = "Set";
			this.btnCh1Set.UseVisualStyleBackColor = true;
			this.btnCh1Set.Click += new System.EventHandler(this.SetLights);
			// 
			// nudCh1
			// 
			this.nudCh1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nudCh1.Location = new System.Drawing.Point(18, 41);
			this.nudCh1.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
			this.nudCh1.Name = "nudCh1";
			this.nudCh1.Size = new System.Drawing.Size(67, 26);
			this.nudCh1.TabIndex = 0;
			this.nudCh1.Tag = "0";
			this.nudCh1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// gbCH2
			// 
			this.gbCH2.Controls.Add(this.chkCh2Lock);
			this.gbCH2.Controls.Add(this.btnCh2Get);
			this.gbCH2.Controls.Add(this.btnCh2Set);
			this.gbCH2.Controls.Add(this.nudCh2);
			this.gbCH2.Enabled = false;
			this.gbCH2.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbCH2.Location = new System.Drawing.Point(243, 88);
			this.gbCH2.Name = "gbCH2";
			this.gbCH2.Size = new System.Drawing.Size(195, 116);
			this.gbCH2.TabIndex = 3;
			this.gbCH2.TabStop = false;
			this.gbCH2.Text = "Channel 2";
			// 
			// chkCh2Lock
			// 
			this.chkCh2Lock.AutoSize = true;
			this.chkCh2Lock.Location = new System.Drawing.Point(18, 74);
			this.chkCh2Lock.Name = "chkCh2Lock";
			this.chkCh2Lock.Size = new System.Drawing.Size(61, 23);
			this.chkCh2Lock.TabIndex = 3;
			this.chkCh2Lock.Tag = "1";
			this.chkCh2Lock.Text = "Lock";
			this.chkCh2Lock.UseVisualStyleBackColor = true;
			this.chkCh2Lock.CheckedChanged += new System.EventHandler(this.LockLights);
			// 
			// btnCh2Get
			// 
			this.btnCh2Get.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCh2Get.Location = new System.Drawing.Point(101, 64);
			this.btnCh2Get.Name = "btnCh2Get";
			this.btnCh2Get.Size = new System.Drawing.Size(75, 33);
			this.btnCh2Get.TabIndex = 2;
			this.btnCh2Get.Tag = "1";
			this.btnCh2Get.Text = "Get";
			this.btnCh2Get.UseVisualStyleBackColor = true;
			this.btnCh2Get.Click += new System.EventHandler(this.GetLights);
			// 
			// btnCh2Set
			// 
			this.btnCh2Set.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCh2Set.Location = new System.Drawing.Point(101, 25);
			this.btnCh2Set.Name = "btnCh2Set";
			this.btnCh2Set.Size = new System.Drawing.Size(75, 33);
			this.btnCh2Set.TabIndex = 1;
			this.btnCh2Set.Tag = "1";
			this.btnCh2Set.Text = "Set";
			this.btnCh2Set.UseVisualStyleBackColor = true;
			this.btnCh2Set.Click += new System.EventHandler(this.SetLights);
			// 
			// nudCh2
			// 
			this.nudCh2.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nudCh2.Location = new System.Drawing.Point(18, 41);
			this.nudCh2.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
			this.nudCh2.Name = "nudCh2";
			this.nudCh2.Size = new System.Drawing.Size(67, 26);
			this.nudCh2.TabIndex = 0;
			this.nudCh2.Tag = "1";
			this.nudCh2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// gbCH4
			// 
			this.gbCH4.Controls.Add(this.chkCh4Lock);
			this.gbCH4.Controls.Add(this.btnCh4Get);
			this.gbCH4.Controls.Add(this.btnCh4Set);
			this.gbCH4.Controls.Add(this.nudCh4);
			this.gbCH4.Enabled = false;
			this.gbCH4.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbCH4.Location = new System.Drawing.Point(675, 88);
			this.gbCH4.Name = "gbCH4";
			this.gbCH4.Size = new System.Drawing.Size(195, 116);
			this.gbCH4.TabIndex = 5;
			this.gbCH4.TabStop = false;
			this.gbCH4.Text = "Channel 4";
			// 
			// chkCh4Lock
			// 
			this.chkCh4Lock.AutoSize = true;
			this.chkCh4Lock.Location = new System.Drawing.Point(18, 74);
			this.chkCh4Lock.Name = "chkCh4Lock";
			this.chkCh4Lock.Size = new System.Drawing.Size(61, 23);
			this.chkCh4Lock.TabIndex = 3;
			this.chkCh4Lock.Tag = "3";
			this.chkCh4Lock.Text = "Lock";
			this.chkCh4Lock.UseVisualStyleBackColor = true;
			this.chkCh4Lock.CheckedChanged += new System.EventHandler(this.LockLights);
			// 
			// btnCh4Get
			// 
			this.btnCh4Get.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCh4Get.Location = new System.Drawing.Point(101, 64);
			this.btnCh4Get.Name = "btnCh4Get";
			this.btnCh4Get.Size = new System.Drawing.Size(75, 33);
			this.btnCh4Get.TabIndex = 2;
			this.btnCh4Get.Tag = "3";
			this.btnCh4Get.Text = "Get";
			this.btnCh4Get.UseVisualStyleBackColor = true;
			this.btnCh4Get.Click += new System.EventHandler(this.GetLights);
			// 
			// btnCh4Set
			// 
			this.btnCh4Set.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCh4Set.Location = new System.Drawing.Point(101, 25);
			this.btnCh4Set.Name = "btnCh4Set";
			this.btnCh4Set.Size = new System.Drawing.Size(75, 33);
			this.btnCh4Set.TabIndex = 1;
			this.btnCh4Set.Tag = "3";
			this.btnCh4Set.Text = "Set";
			this.btnCh4Set.UseVisualStyleBackColor = true;
			this.btnCh4Set.Click += new System.EventHandler(this.SetLights);
			// 
			// nudCh4
			// 
			this.nudCh4.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nudCh4.Location = new System.Drawing.Point(18, 41);
			this.nudCh4.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
			this.nudCh4.Name = "nudCh4";
			this.nudCh4.Size = new System.Drawing.Size(67, 26);
			this.nudCh4.TabIndex = 0;
			this.nudCh4.Tag = "3";
			this.nudCh4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// gbCH3
			// 
			this.gbCH3.Controls.Add(this.chkCh3Lock);
			this.gbCH3.Controls.Add(this.btnCh3Get);
			this.gbCH3.Controls.Add(this.btnCh3Set);
			this.gbCH3.Controls.Add(this.nudCh3);
			this.gbCH3.Enabled = false;
			this.gbCH3.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbCH3.Location = new System.Drawing.Point(458, 88);
			this.gbCH3.Name = "gbCH3";
			this.gbCH3.Size = new System.Drawing.Size(195, 116);
			this.gbCH3.TabIndex = 4;
			this.gbCH3.TabStop = false;
			this.gbCH3.Text = "Channel 3";
			// 
			// chkCh3Lock
			// 
			this.chkCh3Lock.AutoSize = true;
			this.chkCh3Lock.Location = new System.Drawing.Point(18, 74);
			this.chkCh3Lock.Name = "chkCh3Lock";
			this.chkCh3Lock.Size = new System.Drawing.Size(61, 23);
			this.chkCh3Lock.TabIndex = 3;
			this.chkCh3Lock.Tag = "2";
			this.chkCh3Lock.Text = "Lock";
			this.chkCh3Lock.UseVisualStyleBackColor = true;
			this.chkCh3Lock.CheckedChanged += new System.EventHandler(this.LockLights);
			// 
			// btnCh3Get
			// 
			this.btnCh3Get.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCh3Get.Location = new System.Drawing.Point(101, 64);
			this.btnCh3Get.Name = "btnCh3Get";
			this.btnCh3Get.Size = new System.Drawing.Size(75, 33);
			this.btnCh3Get.TabIndex = 2;
			this.btnCh3Get.Tag = "2";
			this.btnCh3Get.Text = "Get";
			this.btnCh3Get.UseVisualStyleBackColor = true;
			this.btnCh3Get.Click += new System.EventHandler(this.GetLights);
			// 
			// btnCh3Set
			// 
			this.btnCh3Set.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCh3Set.Location = new System.Drawing.Point(101, 25);
			this.btnCh3Set.Name = "btnCh3Set";
			this.btnCh3Set.Size = new System.Drawing.Size(75, 33);
			this.btnCh3Set.TabIndex = 1;
			this.btnCh3Set.Tag = "2";
			this.btnCh3Set.Text = "Set";
			this.btnCh3Set.UseVisualStyleBackColor = true;
			this.btnCh3Set.Click += new System.EventHandler(this.SetLights);
			// 
			// nudCh3
			// 
			this.nudCh3.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nudCh3.Location = new System.Drawing.Point(18, 41);
			this.nudCh3.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
			this.nudCh3.Name = "nudCh3";
			this.nudCh3.Size = new System.Drawing.Size(67, 26);
			this.nudCh3.TabIndex = 0;
			this.nudCh3.Tag = "2";
			this.nudCh3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// btnSetAll
			// 
			this.btnSetAll.Enabled = false;
			this.btnSetAll.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSetAll.Location = new System.Drawing.Point(688, 224);
			this.btnSetAll.Name = "btnSetAll";
			this.btnSetAll.Size = new System.Drawing.Size(88, 33);
			this.btnSetAll.TabIndex = 6;
			this.btnSetAll.Tag = "-1";
			this.btnSetAll.Text = "Set All";
			this.btnSetAll.UseVisualStyleBackColor = true;
			this.btnSetAll.Click += new System.EventHandler(this.btnSetAll_Click);
			// 
			// btnGetAll
			// 
			this.btnGetAll.Enabled = false;
			this.btnGetAll.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnGetAll.Location = new System.Drawing.Point(782, 224);
			this.btnGetAll.Name = "btnGetAll";
			this.btnGetAll.Size = new System.Drawing.Size(88, 33);
			this.btnGetAll.TabIndex = 7;
			this.btnGetAll.Tag = "-1";
			this.btnGetAll.Text = "Get All";
			this.btnGetAll.UseVisualStyleBackColor = true;
			this.btnGetAll.Click += new System.EventHandler(this.btnGetAll_Click);
			// 
			// lbVersion
			// 
			this.lbVersion.Font = new System.Drawing.Font("Cambria", 12F);
			this.lbVersion.Location = new System.Drawing.Point(188, 32);
			this.lbVersion.Name = "lbVersion";
			this.lbVersion.Size = new System.Drawing.Size(465, 30);
			this.lbVersion.TabIndex = 8;
			this.lbVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbVersion.Visible = false;
			// 
			// DimmerCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(901, 297);
			this.Controls.Add(this.lbVersion);
			this.Controls.Add(this.btnGetAll);
			this.Controls.Add(this.btnSetAll);
			this.Controls.Add(this.gbCH4);
			this.Controls.Add(this.gbCH3);
			this.Controls.Add(this.gbCH2);
			this.Controls.Add(this.gbCH1);
			this.Controls.Add(this.picConStt);
			this.Controls.Add(this.btnConnect);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DimmerCtrl";
			this.Text = "調光器";
			((System.ComponentModel.ISupportInitialize)(this.picConStt)).EndInit();
			this.gbCH1.ResumeLayout(false);
			this.gbCH1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCh1)).EndInit();
			this.gbCH2.ResumeLayout(false);
			this.gbCH2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCh2)).EndInit();
			this.gbCH4.ResumeLayout(false);
			this.gbCH4.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCh4)).EndInit();
			this.gbCH3.ResumeLayout(false);
			this.gbCH3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCh3)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.PictureBox picConStt;
        private System.Windows.Forms.GroupBox gbCH1;
        private System.Windows.Forms.CheckBox chkCh1Lock;
        private System.Windows.Forms.Button btnCh1Get;
        private System.Windows.Forms.Button btnCh1Set;
        private System.Windows.Forms.NumericUpDown nudCh1;
        private System.Windows.Forms.GroupBox gbCH2;
        private System.Windows.Forms.CheckBox chkCh2Lock;
        private System.Windows.Forms.Button btnCh2Get;
        private System.Windows.Forms.Button btnCh2Set;
        private System.Windows.Forms.NumericUpDown nudCh2;
        private System.Windows.Forms.GroupBox gbCH4;
        private System.Windows.Forms.CheckBox chkCh4Lock;
        private System.Windows.Forms.Button btnCh4Get;
        private System.Windows.Forms.Button btnCh4Set;
        private System.Windows.Forms.NumericUpDown nudCh4;
        private System.Windows.Forms.GroupBox gbCH3;
        private System.Windows.Forms.CheckBox chkCh3Lock;
        private System.Windows.Forms.Button btnCh3Get;
        private System.Windows.Forms.Button btnCh3Set;
        private System.Windows.Forms.NumericUpDown nudCh3;
        private System.Windows.Forms.Button btnSetAll;
        private System.Windows.Forms.Button btnGetAll;
        private System.Windows.Forms.Label lbVersion;
    }
}