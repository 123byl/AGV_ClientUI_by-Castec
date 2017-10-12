namespace CtLib.Forms.TestPlatform
{
    partial class Test_Socket
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Test_Socket));
			this.gbReceive = new System.Windows.Forms.GroupBox();
			this.lsbxMsg = new System.Windows.Forms.ListBox();
			this.cntxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miClrMsg = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.misCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.miSaveLog = new System.Windows.Forms.ToolStripMenuItem();
			this.gbSend = new System.Windows.Forms.GroupBox();
			this.lbUdpPort = new System.Windows.Forms.Label();
			this.lbUdpIp = new System.Windows.Forms.Label();
			this.txtUdpPort = new System.Windows.Forms.TextBox();
			this.txtUdpIp = new System.Windows.Forms.TextBox();
			this.cbEndLine = new System.Windows.Forms.ComboBox();
			this.btnReceive = new System.Windows.Forms.Button();
			this.btnSend = new System.Windows.Forms.Button();
			this.txtSend = new System.Windows.Forms.TextBox();
			this.cbMode = new System.Windows.Forms.ComboBox();
			this.txtIP = new System.Windows.Forms.TextBox();
			this.txtPort = new System.Windows.Forms.TextBox();
			this.btnConnect = new System.Windows.Forms.Button();
			this.lbIP = new System.Windows.Forms.Label();
			this.lbPort = new System.Windows.Forms.Label();
			this.lbSrvCount = new System.Windows.Forms.Label();
			this.cbDataType = new System.Windows.Forms.ComboBox();
			this.gbSetting = new System.Windows.Forms.GroupBox();
			this.pbStt = new System.Windows.Forms.PictureBox();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.miProg = new System.Windows.Forms.ToolStripMenuItem();
			this.misExit = new System.Windows.Forms.ToolStripMenuItem();
			this.miOpt = new System.Windows.Forms.ToolStripMenuItem();
			this.misProt = new System.Windows.Forms.ToolStripMenuItem();
			this.misProtList = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.misEnc = new System.Windows.Forms.ToolStripMenuItem();
			this.misEncList = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.misAutoClose = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.misEnter = new System.Windows.Forms.ToolStripMenuItem();
			this.misAutoClear = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.misShowSend = new System.Windows.Forms.ToolStripMenuItem();
			this.misManRx = new System.Windows.Forms.ToolStripMenuItem();
			this.gbReceive.SuspendLayout();
			this.cntxMenu.SuspendLayout();
			this.gbSend.SuspendLayout();
			this.gbSetting.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbStt)).BeginInit();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbReceive
			// 
			resources.ApplyResources(this.gbReceive, "gbReceive");
			this.gbReceive.Controls.Add(this.lsbxMsg);
			this.gbReceive.Name = "gbReceive";
			this.gbReceive.TabStop = false;
			this.gbReceive.Paint += new System.Windows.Forms.PaintEventHandler(this.GroupBoxPaint);
			// 
			// lsbxMsg
			// 
			resources.ApplyResources(this.lsbxMsg, "lsbxMsg");
			this.lsbxMsg.BackColor = System.Drawing.SystemColors.Control;
			this.lsbxMsg.ContextMenuStrip = this.cntxMenu;
			this.lsbxMsg.FormattingEnabled = true;
			this.lsbxMsg.Name = "lsbxMsg";
			// 
			// cntxMenu
			// 
			resources.ApplyResources(this.cntxMenu, "cntxMenu");
			this.cntxMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cntxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miClrMsg,
            this.toolStripSeparator1,
            this.misCopy,
            this.miSaveLog});
			this.cntxMenu.Name = "cntxMenu";
			// 
			// miClrMsg
			// 
			resources.ApplyResources(this.miClrMsg, "miClrMsg");
			this.miClrMsg.Name = "miClrMsg";
			this.miClrMsg.Click += new System.EventHandler(this.miClrMsg_Click);
			// 
			// toolStripSeparator1
			// 
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			// 
			// misCopy
			// 
			resources.ApplyResources(this.misCopy, "misCopy");
			this.misCopy.Name = "misCopy";
			this.misCopy.Click += new System.EventHandler(this.misCopy_Click);
			// 
			// miSaveLog
			// 
			resources.ApplyResources(this.miSaveLog, "miSaveLog");
			this.miSaveLog.Name = "miSaveLog";
			this.miSaveLog.Click += new System.EventHandler(this.miSaveLog_Click);
			// 
			// gbSend
			// 
			resources.ApplyResources(this.gbSend, "gbSend");
			this.gbSend.Controls.Add(this.lbUdpPort);
			this.gbSend.Controls.Add(this.lbUdpIp);
			this.gbSend.Controls.Add(this.txtUdpPort);
			this.gbSend.Controls.Add(this.txtUdpIp);
			this.gbSend.Controls.Add(this.cbEndLine);
			this.gbSend.Controls.Add(this.btnReceive);
			this.gbSend.Controls.Add(this.btnSend);
			this.gbSend.Controls.Add(this.txtSend);
			this.gbSend.Name = "gbSend";
			this.gbSend.TabStop = false;
			this.gbSend.Paint += new System.Windows.Forms.PaintEventHandler(this.GroupBoxPaint);
			// 
			// lbUdpPort
			// 
			resources.ApplyResources(this.lbUdpPort, "lbUdpPort");
			this.lbUdpPort.Name = "lbUdpPort";
			// 
			// lbUdpIp
			// 
			resources.ApplyResources(this.lbUdpIp, "lbUdpIp");
			this.lbUdpIp.Name = "lbUdpIp";
			// 
			// txtUdpPort
			// 
			resources.ApplyResources(this.txtUdpPort, "txtUdpPort");
			this.txtUdpPort.Name = "txtUdpPort";
			// 
			// txtUdpIp
			// 
			resources.ApplyResources(this.txtUdpIp, "txtUdpIp");
			this.txtUdpIp.Name = "txtUdpIp";
			// 
			// cbEndLine
			// 
			resources.ApplyResources(this.cbEndLine, "cbEndLine");
			this.cbEndLine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbEndLine.FormattingEnabled = true;
			this.cbEndLine.Items.AddRange(new object[] {
            resources.GetString("cbEndLine.Items"),
            resources.GetString("cbEndLine.Items1"),
            resources.GetString("cbEndLine.Items2"),
            resources.GetString("cbEndLine.Items3")});
			this.cbEndLine.Name = "cbEndLine";
			this.cbEndLine.SelectedIndexChanged += new System.EventHandler(this.cbEndLine_SelectedIndexChanged);
			// 
			// btnReceive
			// 
			resources.ApplyResources(this.btnReceive, "btnReceive");
			this.btnReceive.Name = "btnReceive";
			this.btnReceive.UseVisualStyleBackColor = true;
			this.btnReceive.Click += new System.EventHandler(this.btnReceive_Click);
			this.btnReceive.Paint += new System.Windows.Forms.PaintEventHandler(this.ButtonPaint);
			this.btnReceive.MouseEnter += new System.EventHandler(this.MouseIn);
			this.btnReceive.MouseLeave += new System.EventHandler(this.MouseOut);
			// 
			// btnSend
			// 
			resources.ApplyResources(this.btnSend, "btnSend");
			this.btnSend.Name = "btnSend";
			this.btnSend.UseVisualStyleBackColor = true;
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			this.btnSend.Paint += new System.Windows.Forms.PaintEventHandler(this.ButtonPaint);
			this.btnSend.MouseEnter += new System.EventHandler(this.MouseIn);
			this.btnSend.MouseLeave += new System.EventHandler(this.MouseOut);
			// 
			// txtSend
			// 
			resources.ApplyResources(this.txtSend, "txtSend");
			this.txtSend.Name = "txtSend";
			this.txtSend.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSend_KeyPress);
			// 
			// cbMode
			// 
			resources.ApplyResources(this.cbMode, "cbMode");
			this.cbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbMode.FormattingEnabled = true;
			this.cbMode.Items.AddRange(new object[] {
            resources.GetString("cbMode.Items"),
            resources.GetString("cbMode.Items1")});
			this.cbMode.Name = "cbMode";
			this.cbMode.SelectedIndexChanged += new System.EventHandler(this.cbMode_SelectedIndexChanged);
			// 
			// txtIP
			// 
			resources.ApplyResources(this.txtIP, "txtIP");
			this.txtIP.Name = "txtIP";
			// 
			// txtPort
			// 
			resources.ApplyResources(this.txtPort, "txtPort");
			this.txtPort.Name = "txtPort";
			// 
			// btnConnect
			// 
			resources.ApplyResources(this.btnConnect, "btnConnect");
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			this.btnConnect.Paint += new System.Windows.Forms.PaintEventHandler(this.ButtonPaint);
			this.btnConnect.MouseEnter += new System.EventHandler(this.MouseIn);
			this.btnConnect.MouseLeave += new System.EventHandler(this.MouseOut);
			// 
			// lbIP
			// 
			resources.ApplyResources(this.lbIP, "lbIP");
			this.lbIP.Name = "lbIP";
			// 
			// lbPort
			// 
			resources.ApplyResources(this.lbPort, "lbPort");
			this.lbPort.Name = "lbPort";
			// 
			// lbSrvCount
			// 
			resources.ApplyResources(this.lbSrvCount, "lbSrvCount");
			this.lbSrvCount.Name = "lbSrvCount";
			// 
			// cbDataType
			// 
			resources.ApplyResources(this.cbDataType, "cbDataType");
			this.cbDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbDataType.FormattingEnabled = true;
			this.cbDataType.Items.AddRange(new object[] {
            resources.GetString("cbDataType.Items"),
            resources.GetString("cbDataType.Items1")});
			this.cbDataType.Name = "cbDataType";
			this.cbDataType.SelectedIndexChanged += new System.EventHandler(this.cbDataType_SelectedIndexChanged);
			// 
			// gbSetting
			// 
			resources.ApplyResources(this.gbSetting, "gbSetting");
			this.gbSetting.Controls.Add(this.cbDataType);
			this.gbSetting.Controls.Add(this.lbSrvCount);
			this.gbSetting.Controls.Add(this.lbPort);
			this.gbSetting.Controls.Add(this.lbIP);
			this.gbSetting.Controls.Add(this.pbStt);
			this.gbSetting.Controls.Add(this.btnConnect);
			this.gbSetting.Controls.Add(this.txtPort);
			this.gbSetting.Controls.Add(this.txtIP);
			this.gbSetting.Controls.Add(this.cbMode);
			this.gbSetting.Name = "gbSetting";
			this.gbSetting.TabStop = false;
			this.gbSetting.Paint += new System.Windows.Forms.PaintEventHandler(this.GroupBoxPaint);
			// 
			// pbStt
			// 
			resources.ApplyResources(this.pbStt, "pbStt");
			this.pbStt.Image = global::CtLib.Properties.Resources.Grey_Ball;
			this.pbStt.Name = "pbStt";
			this.pbStt.TabStop = false;
			// 
			// menuStrip
			// 
			resources.ApplyResources(this.menuStrip, "menuStrip");
			this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miProg,
            this.miOpt});
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.MouseDown += new System.Windows.Forms.MouseEventHandler(this.menuStrip_MouseDown);
			// 
			// miProg
			// 
			resources.ApplyResources(this.miProg, "miProg");
			this.miProg.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.misExit});
			this.miProg.Name = "miProg";
			// 
			// misExit
			// 
			resources.ApplyResources(this.misExit, "misExit");
			this.misExit.Name = "misExit";
			this.misExit.Click += new System.EventHandler(this.misExit_Click);
			// 
			// miOpt
			// 
			resources.ApplyResources(this.miOpt, "miOpt");
			this.miOpt.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.misProt,
            this.toolStripSeparator5,
            this.misEnc,
            this.toolStripSeparator2,
            this.misAutoClose,
            this.toolStripSeparator3,
            this.misEnter,
            this.misAutoClear,
            this.toolStripSeparator4,
            this.misShowSend,
            this.misManRx});
			this.miOpt.Name = "miOpt";
			// 
			// misProt
			// 
			resources.ApplyResources(this.misProt, "misProt");
			this.misProt.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.misProtList});
			this.misProt.Name = "misProt";
			// 
			// misProtList
			// 
			resources.ApplyResources(this.misProtList, "misProtList");
			this.misProtList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.misProtList.Items.AddRange(new object[] {
            resources.GetString("misProtList.Items"),
            resources.GetString("misProtList.Items1")});
			this.misProtList.Name = "misProtList";
			this.misProtList.DropDownClosed += new System.EventHandler(this.misCbProtocol_DropDownClosed);
			// 
			// toolStripSeparator5
			// 
			resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			// 
			// misEnc
			// 
			resources.ApplyResources(this.misEnc, "misEnc");
			this.misEnc.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.misEncList});
			this.misEnc.Name = "misEnc";
			// 
			// misEncList
			// 
			resources.ApplyResources(this.misEncList, "misEncList");
			this.misEncList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.misEncList.Items.AddRange(new object[] {
            resources.GetString("misEncList.Items"),
            resources.GetString("misEncList.Items1"),
            resources.GetString("misEncList.Items2"),
            resources.GetString("misEncList.Items3"),
            resources.GetString("misEncList.Items4"),
            resources.GetString("misEncList.Items5")});
			this.misEncList.Name = "misEncList";
			this.misEncList.DropDownClosed += new System.EventHandler(this.misEncList_DropDownClosed);
			// 
			// toolStripSeparator2
			// 
			resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			// 
			// misAutoClose
			// 
			resources.ApplyResources(this.misAutoClose, "misAutoClose");
			this.misAutoClose.Name = "misAutoClose";
			this.misAutoClose.Click += new System.EventHandler(this.misAutoClose_Click);
			// 
			// toolStripSeparator3
			// 
			resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			// 
			// misEnter
			// 
			resources.ApplyResources(this.misEnter, "misEnter");
			this.misEnter.Name = "misEnter";
			this.misEnter.Click += new System.EventHandler(this.misEnter_Click);
			// 
			// misAutoClear
			// 
			resources.ApplyResources(this.misAutoClear, "misAutoClear");
			this.misAutoClear.Name = "misAutoClear";
			this.misAutoClear.Click += new System.EventHandler(this.misAutoClear_Click);
			// 
			// toolStripSeparator4
			// 
			resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			// 
			// misShowSend
			// 
			resources.ApplyResources(this.misShowSend, "misShowSend");
			this.misShowSend.Name = "misShowSend";
			this.misShowSend.Click += new System.EventHandler(this.misShowSend_Click);
			// 
			// misManRx
			// 
			resources.ApplyResources(this.misManRx, "misManRx");
			this.misManRx.Name = "misManRx";
			this.misManRx.Click += new System.EventHandler(this.misManRx_Click);
			// 
			// Test_Socket
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.gbSend);
			this.Controls.Add(this.menuStrip);
			this.Controls.Add(this.gbSetting);
			this.Controls.Add(this.gbReceive);
			this.MainMenuStrip = this.menuStrip;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Test_Socket";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Test_Socket_Paint);
			this.gbReceive.ResumeLayout(false);
			this.cntxMenu.ResumeLayout(false);
			this.gbSend.ResumeLayout(false);
			this.gbSend.PerformLayout();
			this.gbSetting.ResumeLayout(false);
			this.gbSetting.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbStt)).EndInit();
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbReceive;
        private System.Windows.Forms.GroupBox gbSend;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtSend;
        private System.Windows.Forms.ComboBox cbMode;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.PictureBox pbStt;
        private System.Windows.Forms.Label lbIP;
        private System.Windows.Forms.Label lbPort;
        private System.Windows.Forms.Label lbSrvCount;
        private System.Windows.Forms.ComboBox cbEndLine;
        private System.Windows.Forms.ComboBox cbDataType;
		private System.Windows.Forms.ListBox lsbxMsg;
		private System.Windows.Forms.ContextMenuStrip cntxMenu;
		private System.Windows.Forms.ToolStripMenuItem miClrMsg;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem miSaveLog;
		private System.Windows.Forms.GroupBox gbSetting;
		private System.Windows.Forms.Button btnReceive;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem miOpt;
		private System.Windows.Forms.ToolStripMenuItem misEnc;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem misEnter;
		private System.Windows.Forms.ToolStripMenuItem misAutoClear;
		private System.Windows.Forms.ToolStripMenuItem misAutoClose;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripComboBox misEncList;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem misShowSend;
		private System.Windows.Forms.ToolStripMenuItem misManRx;
		private System.Windows.Forms.ToolStripMenuItem miProg;
		private System.Windows.Forms.ToolStripMenuItem misExit;
		private System.Windows.Forms.ToolStripMenuItem misProt;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripComboBox misProtList;
		private System.Windows.Forms.Label lbUdpPort;
		private System.Windows.Forms.Label lbUdpIp;
		private System.Windows.Forms.TextBox txtUdpPort;
		private System.Windows.Forms.TextBox txtUdpIp;
		private System.Windows.Forms.ToolStripMenuItem misCopy;
	}
}

