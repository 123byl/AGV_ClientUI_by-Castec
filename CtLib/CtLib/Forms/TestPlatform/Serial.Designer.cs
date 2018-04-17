namespace CtLib.Forms.TestPlatform {
    partial class Test_Serial {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Test_Serial));
			this.btnOpen = new System.Windows.Forms.Button();
			this.btnSend = new System.Windows.Forms.Button();
			this.txtSend = new System.Windows.Forms.TextBox();
			this.cbPorts = new System.Windows.Forms.ComboBox();
			this.pbConnectStt = new System.Windows.Forms.PictureBox();
			this.gbSend = new System.Windows.Forms.GroupBox();
			this.cbNewLine = new System.Windows.Forms.ComboBox();
			this.gbReceive = new System.Windows.Forms.GroupBox();
			this.lsbxMsg = new System.Windows.Forms.ListBox();
			this.cntxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miClrMsg = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.miSaveLog = new System.Windows.Forms.ToolStripMenuItem();
			this.cbDataType = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.pbConnectStt)).BeginInit();
			this.gbSend.SuspendLayout();
			this.gbReceive.SuspendLayout();
			this.cntxMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOpen
			// 
			this.btnOpen.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnOpen.Location = new System.Drawing.Point(269, 28);
			this.btnOpen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnOpen.Name = "btnOpen";
			this.btnOpen.Size = new System.Drawing.Size(100, 29);
			this.btnOpen.TabIndex = 1;
			this.btnOpen.Text = "Open";
			this.btnOpen.UseVisualStyleBackColor = true;
			this.btnOpen.Click += new System.EventHandler(this.button1_Click);
			// 
			// btnSend
			// 
			this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSend.Enabled = false;
			this.btnSend.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSend.Location = new System.Drawing.Point(479, 26);
			this.btnSend.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(100, 29);
			this.btnSend.TabIndex = 3;
			this.btnSend.Text = "Send";
			this.btnSend.UseVisualStyleBackColor = true;
			this.btnSend.Click += new System.EventHandler(this.button2_Click);
			// 
			// txtSend
			// 
			this.txtSend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSend.Enabled = false;
			this.txtSend.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSend.ImeMode = System.Windows.Forms.ImeMode.Disable;
			this.txtSend.Location = new System.Drawing.Point(16, 26);
			this.txtSend.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtSend.Name = "txtSend";
			this.txtSend.Size = new System.Drawing.Size(367, 25);
			this.txtSend.TabIndex = 2;
			// 
			// cbPorts
			// 
			this.cbPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPorts.Enabled = false;
			this.cbPorts.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cbPorts.FormattingEnabled = true;
			this.cbPorts.Items.AddRange(new object[] {
            "N/A"});
			this.cbPorts.Location = new System.Drawing.Point(153, 28);
			this.cbPorts.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.cbPorts.Name = "cbPorts";
			this.cbPorts.Size = new System.Drawing.Size(107, 26);
			this.cbPorts.TabIndex = 6;
			// 
			// pbConnectStt
			// 
			this.pbConnectStt.Image = global::CtLib.Properties.Resources.Grey_Ball;
			this.pbConnectStt.Location = new System.Drawing.Point(380, 22);
			this.pbConnectStt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.pbConnectStt.Name = "pbConnectStt";
			this.pbConnectStt.Size = new System.Drawing.Size(40, 38);
			this.pbConnectStt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pbConnectStt.TabIndex = 7;
			this.pbConnectStt.TabStop = false;
			// 
			// gbSend
			// 
			this.gbSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gbSend.Controls.Add(this.cbNewLine);
			this.gbSend.Controls.Add(this.btnSend);
			this.gbSend.Controls.Add(this.txtSend);
			this.gbSend.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbSend.Location = new System.Drawing.Point(24, 81);
			this.gbSend.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbSend.Name = "gbSend";
			this.gbSend.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbSend.Size = new System.Drawing.Size(593, 70);
			this.gbSend.TabIndex = 8;
			this.gbSend.TabStop = false;
			this.gbSend.Text = "Transmission";
			// 
			// cbNewLine
			// 
			this.cbNewLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbNewLine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbNewLine.Enabled = false;
			this.cbNewLine.FormattingEnabled = true;
			this.cbNewLine.Items.AddRange(new object[] {
            "None",
            "CrLf",
            "Cr",
            "Lf"});
			this.cbNewLine.Location = new System.Drawing.Point(387, 26);
			this.cbNewLine.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.cbNewLine.Name = "cbNewLine";
			this.cbNewLine.Size = new System.Drawing.Size(84, 26);
			this.cbNewLine.TabIndex = 4;
			this.cbNewLine.SelectedIndexChanged += new System.EventHandler(this.cbNewLine_SelectedIndexChanged);
			// 
			// gbReceive
			// 
			this.gbReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gbReceive.Controls.Add(this.lsbxMsg);
			this.gbReceive.Font = new System.Drawing.Font("Candara", 9F);
			this.gbReceive.Location = new System.Drawing.Point(24, 159);
			this.gbReceive.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbReceive.Name = "gbReceive";
			this.gbReceive.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbReceive.Size = new System.Drawing.Size(591, 500);
			this.gbReceive.TabIndex = 9;
			this.gbReceive.TabStop = false;
			this.gbReceive.Text = "Reception";
			// 
			// lsbxMsg
			// 
			this.lsbxMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lsbxMsg.ContextMenuStrip = this.cntxMenu;
			this.lsbxMsg.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lsbxMsg.FormattingEnabled = true;
			this.lsbxMsg.ItemHeight = 18;
			this.lsbxMsg.Location = new System.Drawing.Point(13, 26);
			this.lsbxMsg.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.lsbxMsg.Name = "lsbxMsg";
			this.lsbxMsg.Size = new System.Drawing.Size(564, 454);
			this.lsbxMsg.TabIndex = 0;
			// 
			// cntxMenu
			// 
			this.cntxMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cntxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miClrMsg,
            this.toolStripSeparator1,
            this.miSaveLog});
			this.cntxMenu.Name = "cntxMenu";
			this.cntxMenu.Size = new System.Drawing.Size(181, 58);
			// 
			// miClrMsg
			// 
			this.miClrMsg.Name = "miClrMsg";
			this.miClrMsg.Size = new System.Drawing.Size(180, 24);
			this.miClrMsg.Text = "Clear Message";
			this.miClrMsg.Click += new System.EventHandler(this.miClrMsg_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
			// 
			// miSaveLog
			// 
			this.miSaveLog.Name = "miSaveLog";
			this.miSaveLog.Size = new System.Drawing.Size(180, 24);
			this.miSaveLog.Text = "Save As Log";
			this.miSaveLog.Click += new System.EventHandler(this.miSaveLog_Click);
			// 
			// cbDataType
			// 
			this.cbDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbDataType.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cbDataType.FormattingEnabled = true;
			this.cbDataType.Items.AddRange(new object[] {
            "String",
            "Byte"});
			this.cbDataType.Location = new System.Drawing.Point(37, 28);
			this.cbDataType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.cbDataType.Name = "cbDataType";
			this.cbDataType.Size = new System.Drawing.Size(107, 26);
			this.cbDataType.TabIndex = 10;
			this.cbDataType.SelectedIndexChanged += new System.EventHandler(this.cbDataType_SelectedIndexChanged);
			// 
			// Test_Serial
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(633, 674);
			this.Controls.Add(this.cbDataType);
			this.Controls.Add(this.gbReceive);
			this.Controls.Add(this.gbSend);
			this.Controls.Add(this.pbConnectStt);
			this.Controls.Add(this.cbPorts);
			this.Controls.Add(this.btnOpen);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Test_Serial";
			this.Text = "Test Platform of Serial";
			this.Load += new System.EventHandler(this.Serial_Load);
			((System.ComponentModel.ISupportInitialize)(this.pbConnectStt)).EndInit();
			this.gbSend.ResumeLayout(false);
			this.gbSend.PerformLayout();
			this.gbReceive.ResumeLayout(false);
			this.cntxMenu.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtSend;
        private System.Windows.Forms.ComboBox cbPorts;
        private System.Windows.Forms.PictureBox pbConnectStt;
        private System.Windows.Forms.GroupBox gbSend;
        private System.Windows.Forms.GroupBox gbReceive;
        private System.Windows.Forms.ComboBox cbNewLine;
        private System.Windows.Forms.ComboBox cbDataType;
		private System.Windows.Forms.ListBox lsbxMsg;
		private System.Windows.Forms.ContextMenuStrip cntxMenu;
		private System.Windows.Forms.ToolStripMenuItem miClrMsg;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem miSaveLog;
	}
}