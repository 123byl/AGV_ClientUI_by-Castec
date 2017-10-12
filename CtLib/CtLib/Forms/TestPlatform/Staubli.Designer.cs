namespace CtLib.Forms.TestPlatform {
    partial class Test_Staubli {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Test_Staubli));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.misProg = new System.Windows.Forms.ToolStripMenuItem();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.lblIP = new System.Windows.Forms.Label();
            this.grpNet = new System.Windows.Forms.GroupBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.txtUsr = new System.Windows.Forms.TextBox();
            this.lblUsr = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.dgvMsg = new System.Windows.Forms.DataGridView();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMsg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            this.btnCurJoint = new System.Windows.Forms.Button();
            this.grpLoc = new System.Windows.Forms.GroupBox();
            this.btnCurPoint = new System.Windows.Forms.Button();
            this.grpPwr = new System.Windows.Forms.GroupBox();
            this.rdoPwrOff = new System.Windows.Forms.RadioButton();
            this.rdoPwrOn = new System.Windows.Forms.RadioButton();
            this.btnPwr = new System.Windows.Forms.Button();
            this.grpIO = new System.Windows.Forms.GroupBox();
            this.txtIO = new System.Windows.Forms.TextBox();
            this.btnAllIo = new System.Windows.Forms.Button();
            this.rdoIoOff = new System.Windows.Forms.RadioButton();
            this.rdoIoOn = new System.Windows.Forms.RadioButton();
            this.lblIO = new System.Windows.Forms.Label();
            this.btnIoWrite = new System.Windows.Forms.Button();
            this.btnIoRead = new System.Windows.Forms.Button();
            this.grpTask = new System.Windows.Forms.GroupBox();
            this.btnKill = new System.Windows.Forms.Button();
            this.btnResume = new System.Windows.Forms.Button();
            this.btnSusp = new System.Windows.Forms.Button();
            this.cboTsk = new System.Windows.Forms.ComboBox();
            this.cboApp = new System.Windows.Forms.ComboBox();
            this.btnTask = new System.Windows.Forms.Button();
            this.btnApp = new System.Windows.Forms.Button();
            this.btnVarGet = new System.Windows.Forms.Button();
            this.grpVar = new System.Windows.Forms.GroupBox();
            this.txtVarName = new System.Windows.Forms.TextBox();
            this.cboVarApp = new System.Windows.Forms.ComboBox();
            this.btnVarGetAll = new System.Windows.Forms.Button();
            this.menuStrip.SuspendLayout();
            this.grpNet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMsg)).BeginInit();
            this.grpLoc.SuspendLayout();
            this.grpPwr.SuspendLayout();
            this.grpIO.SuspendLayout();
            this.grpTask.SuspendLayout();
            this.grpVar.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.misProg});
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowMove);
            // 
            // misProg
            // 
            this.misProg.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miExit});
            this.misProg.Name = "misProg";
            resources.ApplyResources(this.misProg, "misProg");
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            resources.ApplyResources(this.miExit, "miExit");
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // lblIP
            // 
            resources.ApplyResources(this.lblIP, "lblIP");
            this.lblIP.Name = "lblIP";
            // 
            // grpNet
            // 
            resources.ApplyResources(this.grpNet, "grpNet");
            this.grpNet.Controls.Add(this.btnLogin);
            this.grpNet.Controls.Add(this.txtUsr);
            this.grpNet.Controls.Add(this.lblUsr);
            this.grpNet.Controls.Add(this.txtPort);
            this.grpNet.Controls.Add(this.lblPort);
            this.grpNet.Controls.Add(this.txtIP);
            this.grpNet.Controls.Add(this.lblIP);
            this.grpNet.Name = "grpNet";
            this.grpNet.TabStop = false;
            // 
            // btnLogin
            // 
            resources.ApplyResources(this.btnLogin, "btnLogin");
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // txtUsr
            // 
            resources.ApplyResources(this.txtUsr, "txtUsr");
            this.txtUsr.Name = "txtUsr";
            // 
            // lblUsr
            // 
            resources.ApplyResources(this.lblUsr, "lblUsr");
            this.lblUsr.Name = "lblUsr";
            // 
            // txtPort
            // 
            resources.ApplyResources(this.txtPort, "txtPort");
            this.txtPort.Name = "txtPort";
            // 
            // lblPort
            // 
            resources.ApplyResources(this.lblPort, "lblPort");
            this.lblPort.Name = "lblPort";
            // 
            // txtIP
            // 
            resources.ApplyResources(this.txtIP, "txtIP");
            this.txtIP.Name = "txtIP";
            // 
            // dgvMsg
            // 
            this.dgvMsg.AllowUserToAddRows = false;
            this.dgvMsg.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.dgvMsg, "dgvMsg");
            this.dgvMsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvMsg.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMsg.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvMsg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMsg.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTime,
            this.colType,
            this.colMsg});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvMsg.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvMsg.Name = "dgvMsg";
            this.dgvMsg.ReadOnly = true;
            this.dgvMsg.RowHeadersVisible = false;
            this.dgvMsg.RowTemplate.Height = 27;
            // 
            // colTime
            // 
            resources.ApplyResources(this.colTime, "colTime");
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            this.colTime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colType
            // 
            resources.ApplyResources(this.colType, "colType");
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            this.colType.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colMsg
            // 
            this.colMsg.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.colMsg.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.colMsg, "colMsg");
            this.colMsg.Name = "colMsg";
            this.colMsg.ReadOnly = true;
            this.colMsg.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colMsg.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCurJoint
            // 
            resources.ApplyResources(this.btnCurJoint, "btnCurJoint");
            this.btnCurJoint.Name = "btnCurJoint";
            this.btnCurJoint.UseVisualStyleBackColor = true;
            this.btnCurJoint.Click += new System.EventHandler(this.btnCurJoint_Click);
            // 
            // grpLoc
            // 
            resources.ApplyResources(this.grpLoc, "grpLoc");
            this.grpLoc.Controls.Add(this.btnCurPoint);
            this.grpLoc.Controls.Add(this.btnCurJoint);
            this.grpLoc.Name = "grpLoc";
            this.grpLoc.TabStop = false;
            // 
            // btnCurPoint
            // 
            resources.ApplyResources(this.btnCurPoint, "btnCurPoint");
            this.btnCurPoint.Name = "btnCurPoint";
            this.btnCurPoint.UseVisualStyleBackColor = true;
            this.btnCurPoint.Click += new System.EventHandler(this.btnCurPoint_Click);
            // 
            // grpPwr
            // 
            resources.ApplyResources(this.grpPwr, "grpPwr");
            this.grpPwr.Controls.Add(this.rdoPwrOff);
            this.grpPwr.Controls.Add(this.rdoPwrOn);
            this.grpPwr.Controls.Add(this.btnPwr);
            this.grpPwr.Name = "grpPwr";
            this.grpPwr.TabStop = false;
            // 
            // rdoPwrOff
            // 
            resources.ApplyResources(this.rdoPwrOff, "rdoPwrOff");
            this.rdoPwrOff.Name = "rdoPwrOff";
            this.rdoPwrOff.UseVisualStyleBackColor = true;
            // 
            // rdoPwrOn
            // 
            resources.ApplyResources(this.rdoPwrOn, "rdoPwrOn");
            this.rdoPwrOn.Checked = true;
            this.rdoPwrOn.Name = "rdoPwrOn";
            this.rdoPwrOn.TabStop = true;
            this.rdoPwrOn.UseVisualStyleBackColor = true;
            // 
            // btnPwr
            // 
            resources.ApplyResources(this.btnPwr, "btnPwr");
            this.btnPwr.Name = "btnPwr";
            this.btnPwr.UseVisualStyleBackColor = true;
            this.btnPwr.Click += new System.EventHandler(this.btnPwr_Click);
            // 
            // grpIO
            // 
            resources.ApplyResources(this.grpIO, "grpIO");
            this.grpIO.Controls.Add(this.txtIO);
            this.grpIO.Controls.Add(this.btnAllIo);
            this.grpIO.Controls.Add(this.rdoIoOff);
            this.grpIO.Controls.Add(this.rdoIoOn);
            this.grpIO.Controls.Add(this.lblIO);
            this.grpIO.Controls.Add(this.btnIoWrite);
            this.grpIO.Controls.Add(this.btnIoRead);
            this.grpIO.Name = "grpIO";
            this.grpIO.TabStop = false;
            // 
            // txtIO
            // 
            resources.ApplyResources(this.txtIO, "txtIO");
            this.txtIO.Name = "txtIO";
            // 
            // btnAllIo
            // 
            resources.ApplyResources(this.btnAllIo, "btnAllIo");
            this.btnAllIo.Name = "btnAllIo";
            this.btnAllIo.UseVisualStyleBackColor = true;
            this.btnAllIo.Click += new System.EventHandler(this.btnAllIo_Click);
            // 
            // rdoIoOff
            // 
            resources.ApplyResources(this.rdoIoOff, "rdoIoOff");
            this.rdoIoOff.Name = "rdoIoOff";
            this.rdoIoOff.UseVisualStyleBackColor = true;
            // 
            // rdoIoOn
            // 
            resources.ApplyResources(this.rdoIoOn, "rdoIoOn");
            this.rdoIoOn.Checked = true;
            this.rdoIoOn.Name = "rdoIoOn";
            this.rdoIoOn.TabStop = true;
            this.rdoIoOn.UseVisualStyleBackColor = true;
            // 
            // lblIO
            // 
            resources.ApplyResources(this.lblIO, "lblIO");
            this.lblIO.Name = "lblIO";
            // 
            // btnIoWrite
            // 
            resources.ApplyResources(this.btnIoWrite, "btnIoWrite");
            this.btnIoWrite.Name = "btnIoWrite";
            this.btnIoWrite.UseVisualStyleBackColor = true;
            this.btnIoWrite.Click += new System.EventHandler(this.btnIoWrite_Click);
            // 
            // btnIoRead
            // 
            resources.ApplyResources(this.btnIoRead, "btnIoRead");
            this.btnIoRead.Name = "btnIoRead";
            this.btnIoRead.UseVisualStyleBackColor = true;
            this.btnIoRead.Click += new System.EventHandler(this.btnIoRead_Click);
            // 
            // grpTask
            // 
            resources.ApplyResources(this.grpTask, "grpTask");
            this.grpTask.Controls.Add(this.btnKill);
            this.grpTask.Controls.Add(this.btnResume);
            this.grpTask.Controls.Add(this.btnSusp);
            this.grpTask.Controls.Add(this.cboTsk);
            this.grpTask.Controls.Add(this.cboApp);
            this.grpTask.Controls.Add(this.btnTask);
            this.grpTask.Controls.Add(this.btnApp);
            this.grpTask.Name = "grpTask";
            this.grpTask.TabStop = false;
            // 
            // btnKill
            // 
            resources.ApplyResources(this.btnKill, "btnKill");
            this.btnKill.Name = "btnKill";
            this.btnKill.UseVisualStyleBackColor = true;
            this.btnKill.Click += new System.EventHandler(this.btnKill_Click);
            // 
            // btnResume
            // 
            resources.ApplyResources(this.btnResume, "btnResume");
            this.btnResume.Name = "btnResume";
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
            // 
            // btnSusp
            // 
            resources.ApplyResources(this.btnSusp, "btnSusp");
            this.btnSusp.Name = "btnSusp";
            this.btnSusp.UseVisualStyleBackColor = true;
            this.btnSusp.Click += new System.EventHandler(this.btnSusp_Click);
            // 
            // cboTsk
            // 
            this.cboTsk.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTsk.FormattingEnabled = true;
            resources.ApplyResources(this.cboTsk, "cboTsk");
            this.cboTsk.Name = "cboTsk";
            // 
            // cboApp
            // 
            this.cboApp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboApp.FormattingEnabled = true;
            resources.ApplyResources(this.cboApp, "cboApp");
            this.cboApp.Name = "cboApp";
            // 
            // btnTask
            // 
            resources.ApplyResources(this.btnTask, "btnTask");
            this.btnTask.Name = "btnTask";
            this.btnTask.UseVisualStyleBackColor = true;
            this.btnTask.Click += new System.EventHandler(this.btnTask_Click);
            // 
            // btnApp
            // 
            resources.ApplyResources(this.btnApp, "btnApp");
            this.btnApp.Name = "btnApp";
            this.btnApp.UseVisualStyleBackColor = true;
            this.btnApp.Click += new System.EventHandler(this.btnApp_Click);
            // 
            // btnVarGet
            // 
            resources.ApplyResources(this.btnVarGet, "btnVarGet");
            this.btnVarGet.Name = "btnVarGet";
            this.btnVarGet.UseVisualStyleBackColor = true;
            this.btnVarGet.Click += new System.EventHandler(this.btnVarGet_Click);
            // 
            // grpVar
            // 
            resources.ApplyResources(this.grpVar, "grpVar");
            this.grpVar.Controls.Add(this.btnVarGetAll);
            this.grpVar.Controls.Add(this.txtVarName);
            this.grpVar.Controls.Add(this.btnVarGet);
            this.grpVar.Controls.Add(this.cboVarApp);
            this.grpVar.Name = "grpVar";
            this.grpVar.TabStop = false;
            // 
            // txtVarName
            // 
            resources.ApplyResources(this.txtVarName, "txtVarName");
            this.txtVarName.Name = "txtVarName";
            // 
            // cboVarApp
            // 
            this.cboVarApp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVarApp.FormattingEnabled = true;
            resources.ApplyResources(this.cboVarApp, "cboVarApp");
            this.cboVarApp.Name = "cboVarApp";
            // 
            // btnVarGetAll
            // 
            resources.ApplyResources(this.btnVarGetAll, "btnVarGetAll");
            this.btnVarGetAll.Name = "btnVarGetAll";
            this.btnVarGetAll.UseVisualStyleBackColor = true;
            this.btnVarGetAll.Click += new System.EventHandler(this.btnVarGetAll_Click);
            // 
            // Test_Staubli
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpVar);
            this.Controls.Add(this.grpTask);
            this.Controls.Add(this.grpIO);
            this.Controls.Add(this.grpPwr);
            this.Controls.Add(this.grpLoc);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dgvMsg);
            this.Controls.Add(this.grpNet);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Test_Staubli";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormPaint);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.grpNet.ResumeLayout(false);
            this.grpNet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMsg)).EndInit();
            this.grpLoc.ResumeLayout(false);
            this.grpPwr.ResumeLayout(false);
            this.grpPwr.PerformLayout();
            this.grpIO.ResumeLayout(false);
            this.grpIO.PerformLayout();
            this.grpTask.ResumeLayout(false);
            this.grpVar.ResumeLayout(false);
            this.grpVar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem misProg;
        private System.Windows.Forms.ToolStripMenuItem miExit;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.GroupBox grpNet;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtUsr;
        private System.Windows.Forms.Label lblUsr;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.DataGridView dgvMsg;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnCurJoint;
        private System.Windows.Forms.GroupBox grpLoc;
        private System.Windows.Forms.Button btnCurPoint;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMsg;
        private System.Windows.Forms.GroupBox grpPwr;
        private System.Windows.Forms.Button btnPwr;
        private System.Windows.Forms.RadioButton rdoPwrOff;
        private System.Windows.Forms.RadioButton rdoPwrOn;
        private System.Windows.Forms.GroupBox grpIO;
        private System.Windows.Forms.Button btnIoWrite;
        private System.Windows.Forms.Button btnIoRead;
        private System.Windows.Forms.Label lblIO;
        private System.Windows.Forms.RadioButton rdoIoOff;
        private System.Windows.Forms.RadioButton rdoIoOn;
        private System.Windows.Forms.GroupBox grpTask;
        private System.Windows.Forms.Button btnApp;
        private System.Windows.Forms.Button btnAllIo;
        private System.Windows.Forms.Button btnTask;
        private System.Windows.Forms.ComboBox cboTsk;
        private System.Windows.Forms.ComboBox cboApp;
        private System.Windows.Forms.Button btnKill;
        private System.Windows.Forms.Button btnResume;
        private System.Windows.Forms.Button btnSusp;
        private System.Windows.Forms.Button btnVarGet;
        private System.Windows.Forms.TextBox txtIO;
        private System.Windows.Forms.GroupBox grpVar;
        private System.Windows.Forms.TextBox txtVarName;
        private System.Windows.Forms.ComboBox cboVarApp;
        private System.Windows.Forms.Button btnVarGetAll;
    }
}