namespace CtLib.Forms.TestPlatform {
	partial class Test_AsyncPipe {
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
		/// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
		/// 修改這個方法的內容。
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Test_AsyncPipe));
			this.gbReceive = new System.Windows.Forms.GroupBox();
			this.lsbxMsg = new System.Windows.Forms.ListBox();
			this.gbSend = new System.Windows.Forms.GroupBox();
			this.chkAutoClose = new System.Windows.Forms.CheckBox();
			this.cbEndLine = new System.Windows.Forms.ComboBox();
			this.btnSend = new System.Windows.Forms.Button();
			this.txtSend = new System.Windows.Forms.TextBox();
			this.cbMode = new System.Windows.Forms.ComboBox();
			this.txtPipe = new System.Windows.Forms.TextBox();
			this.txtServer = new System.Windows.Forms.TextBox();
			this.btnConnect = new System.Windows.Forms.Button();
			this.pbStt = new System.Windows.Forms.PictureBox();
			this.lbPipe = new System.Windows.Forms.Label();
			this.lbSrv = new System.Windows.Forms.Label();
			this.cbDataType = new System.Windows.Forms.ComboBox();
			this.cntxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miClrMsg = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.miSaveLog = new System.Windows.Forms.ToolStripMenuItem();
			this.chkEnter = new System.Windows.Forms.CheckBox();
			this.gbReceive.SuspendLayout();
			this.gbSend.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbStt)).BeginInit();
			this.cntxMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbReceive
			// 
			this.gbReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gbReceive.Controls.Add(this.lsbxMsg);
			this.gbReceive.Font = new System.Drawing.Font("Candara", 9F);
			this.gbReceive.Location = new System.Drawing.Point(12, 150);
			this.gbReceive.Name = "gbReceive";
			this.gbReceive.Size = new System.Drawing.Size(554, 407);
			this.gbReceive.TabIndex = 11;
			this.gbReceive.TabStop = false;
			this.gbReceive.Text = "Reception";
			// 
			// lsbxMsg
			// 
			this.lsbxMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lsbxMsg.BackColor = System.Drawing.SystemColors.Control;
			this.lsbxMsg.ContextMenuStrip = this.cntxMenu;
			this.lsbxMsg.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lsbxMsg.FormattingEnabled = true;
			this.lsbxMsg.ItemHeight = 14;
			this.lsbxMsg.Location = new System.Drawing.Point(12, 21);
			this.lsbxMsg.Name = "lsbxMsg";
			this.lsbxMsg.Size = new System.Drawing.Size(531, 368);
			this.lsbxMsg.TabIndex = 0;
			// 
			// gbSend
			// 
			this.gbSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gbSend.Controls.Add(this.chkEnter);
			this.gbSend.Controls.Add(this.chkAutoClose);
			this.gbSend.Controls.Add(this.cbEndLine);
			this.gbSend.Controls.Add(this.btnSend);
			this.gbSend.Controls.Add(this.txtSend);
			this.gbSend.Enabled = false;
			this.gbSend.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbSend.Location = new System.Drawing.Point(12, 73);
			this.gbSend.Name = "gbSend";
			this.gbSend.Size = new System.Drawing.Size(554, 71);
			this.gbSend.TabIndex = 10;
			this.gbSend.TabStop = false;
			this.gbSend.Text = "Transmission";
			// 
			// chkAutoClose
			// 
			this.chkAutoClose.AutoSize = true;
			this.chkAutoClose.Location = new System.Drawing.Point(469, 47);
			this.chkAutoClose.Name = "chkAutoClose";
			this.chkAutoClose.Size = new System.Drawing.Size(81, 18);
			this.chkAutoClose.TabIndex = 5;
			this.chkAutoClose.Text = "Auto Close";
			this.chkAutoClose.UseVisualStyleBackColor = true;
			// 
			// cbEndLine
			// 
			this.cbEndLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbEndLine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbEndLine.FormattingEnabled = true;
			this.cbEndLine.Items.AddRange(new object[] {
            "None",
            "CrLf",
            "Cr",
            "Lf"});
			this.cbEndLine.Location = new System.Drawing.Point(398, 21);
			this.cbEndLine.Name = "cbEndLine";
			this.cbEndLine.Size = new System.Drawing.Size(68, 22);
			this.cbEndLine.TabIndex = 4;
			this.cbEndLine.SelectedIndexChanged += new System.EventHandler(this.cbEndLine_SelectedIndexChanged);
			// 
			// btnSend
			// 
			this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSend.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSend.Location = new System.Drawing.Point(472, 21);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(75, 22);
			this.btnSend.TabIndex = 3;
			this.btnSend.Text = "Send";
			this.btnSend.UseVisualStyleBackColor = true;
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			// 
			// txtSend
			// 
			this.txtSend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSend.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSend.ImeMode = System.Windows.Forms.ImeMode.Disable;
			this.txtSend.Location = new System.Drawing.Point(12, 21);
			this.txtSend.Name = "txtSend";
			this.txtSend.Size = new System.Drawing.Size(382, 22);
			this.txtSend.TabIndex = 2;
			this.txtSend.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSend_KeyPress);
			// 
			// cbMode
			// 
			this.cbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbMode.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cbMode.FormattingEnabled = true;
			this.cbMode.Items.AddRange(new object[] {
            "Client",
            "Server"});
			this.cbMode.Location = new System.Drawing.Point(89, 20);
			this.cbMode.Name = "cbMode";
			this.cbMode.Size = new System.Drawing.Size(89, 22);
			this.cbMode.TabIndex = 12;
			this.cbMode.SelectedIndexChanged += new System.EventHandler(this.cbMode_SelectedIndexChanged);
			// 
			// txtPipe
			// 
			this.txtPipe.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtPipe.Location = new System.Drawing.Point(219, 20);
			this.txtPipe.Name = "txtPipe";
			this.txtPipe.Size = new System.Drawing.Size(77, 22);
			this.txtPipe.TabIndex = 13;
			this.txtPipe.Text = "TestPipe";
			this.txtPipe.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtServer
			// 
			this.txtServer.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtServer.Location = new System.Drawing.Point(355, 20);
			this.txtServer.Name = "txtServer";
			this.txtServer.Size = new System.Drawing.Size(69, 22);
			this.txtServer.TabIndex = 14;
			this.txtServer.Text = ".";
			this.txtServer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// btnConnect
			// 
			this.btnConnect.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnConnect.Location = new System.Drawing.Point(430, 19);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(87, 23);
			this.btnConnect.TabIndex = 15;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// pbStt
			// 
			this.pbStt.Image = global::CtLib.Properties.Resources.Grey_Ball;
			this.pbStt.Location = new System.Drawing.Point(525, 14);
			this.pbStt.Name = "pbStt";
			this.pbStt.Size = new System.Drawing.Size(30, 30);
			this.pbStt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pbStt.TabIndex = 16;
			this.pbStt.TabStop = false;
			// 
			// lbPipe
			// 
			this.lbPipe.AutoSize = true;
			this.lbPipe.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbPipe.Location = new System.Drawing.Point(183, 23);
			this.lbPipe.Name = "lbPipe";
			this.lbPipe.Size = new System.Drawing.Size(35, 14);
			this.lbPipe.TabIndex = 17;
			this.lbPipe.Text = "Pipe";
			// 
			// lbSrv
			// 
			this.lbSrv.AutoSize = true;
			this.lbSrv.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbSrv.Location = new System.Drawing.Point(305, 23);
			this.lbSrv.Name = "lbSrv";
			this.lbSrv.Size = new System.Drawing.Size(49, 14);
			this.lbSrv.TabIndex = 18;
			this.lbSrv.Text = "Server";
			// 
			// cbDataType
			// 
			this.cbDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbDataType.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cbDataType.FormattingEnabled = true;
			this.cbDataType.Items.AddRange(new object[] {
            "String",
            "Byte"});
			this.cbDataType.Location = new System.Drawing.Point(15, 20);
			this.cbDataType.Name = "cbDataType";
			this.cbDataType.Size = new System.Drawing.Size(68, 22);
			this.cbDataType.TabIndex = 20;
			this.cbDataType.SelectedIndexChanged += new System.EventHandler(this.cbDataType_SelectedIndexChanged);
			// 
			// cntxMenu
			// 
			this.cntxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miClrMsg,
            this.toolStripSeparator1,
            this.miSaveLog});
			this.cntxMenu.Name = "cntxMenu";
			this.cntxMenu.Size = new System.Drawing.Size(158, 54);
			// 
			// miClrMsg
			// 
			this.miClrMsg.Name = "miClrMsg";
			this.miClrMsg.Size = new System.Drawing.Size(157, 22);
			this.miClrMsg.Text = "Clear Message";
			this.miClrMsg.Click += new System.EventHandler(this.miClrMsg_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(154, 6);
			// 
			// miSaveLog
			// 
			this.miSaveLog.Name = "miSaveLog";
			this.miSaveLog.Size = new System.Drawing.Size(157, 22);
			this.miSaveLog.Text = "Save As Log";
			this.miSaveLog.Click += new System.EventHandler(this.miSaveLog_Click);
			// 
			// chkEnter
			// 
			this.chkEnter.AutoSize = true;
			this.chkEnter.Checked = true;
			this.chkEnter.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkEnter.Location = new System.Drawing.Point(12, 49);
			this.chkEnter.Name = "chkEnter";
			this.chkEnter.Size = new System.Drawing.Size(183, 18);
			this.chkEnter.TabIndex = 21;
			this.chkEnter.Text = "Send data with ENTER pressed";
			this.chkEnter.UseVisualStyleBackColor = true;
			this.chkEnter.CheckStateChanged += new System.EventHandler(this.chkEnter_CheckStateChanged);
			// 
			// Test_AsyncPipe
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(578, 569);
			this.Controls.Add(this.cbDataType);
			this.Controls.Add(this.lbSrv);
			this.Controls.Add(this.lbPipe);
			this.Controls.Add(this.pbStt);
			this.Controls.Add(this.btnConnect);
			this.Controls.Add(this.txtServer);
			this.Controls.Add(this.txtPipe);
			this.Controls.Add(this.cbMode);
			this.Controls.Add(this.gbReceive);
			this.Controls.Add(this.gbSend);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Test_AsyncPipe";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Test Platform of Pipe (Async)";
			this.gbReceive.ResumeLayout(false);
			this.gbSend.ResumeLayout(false);
			this.gbSend.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbStt)).EndInit();
			this.cntxMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox gbReceive;
		private System.Windows.Forms.GroupBox gbSend;
		private System.Windows.Forms.Button btnSend;
		private System.Windows.Forms.TextBox txtSend;
		private System.Windows.Forms.ComboBox cbMode;
		private System.Windows.Forms.TextBox txtPipe;
		private System.Windows.Forms.TextBox txtServer;
		private System.Windows.Forms.Button btnConnect;
		private System.Windows.Forms.PictureBox pbStt;
		private System.Windows.Forms.Label lbPipe;
		private System.Windows.Forms.Label lbSrv;
		private System.Windows.Forms.ComboBox cbEndLine;
		private System.Windows.Forms.ComboBox cbDataType;
		private System.Windows.Forms.CheckBox chkAutoClose;
		private System.Windows.Forms.ListBox lsbxMsg;
		private System.Windows.Forms.ContextMenuStrip cntxMenu;
		private System.Windows.Forms.ToolStripMenuItem miClrMsg;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem miSaveLog;
		private System.Windows.Forms.CheckBox chkEnter;
	}
}

