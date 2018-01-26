using WeifenLuo.WinFormsUI.Docking;

namespace VehiclePlanner {
    partial class VehiclePlannerUI {
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
                /*-- 遍歷所有CtDockContent物件 --*/
                foreach (var kvp in mDockContent) {
                    DockContent dokContent = kvp.Value as DockContent;
                    /*-- 註銷DockStateChanged事件訂閱 --*/
                    dokContent.DockStateChanged -= Value_DockStateChanged;
                    /*-- 釋放CtDockContent物件 --*/
                    dokContent.Dispose();
                }
                mDockContent.Clear();
                mDockContent = null;

                mNotifyIcon?.Dispose();
                mNotifyIcon = null;

                mMenuItems?.Dispose();
                mMenuItems = null;

                components.Dispose();

                mKeyboardHook.KeyUpEvent -= mKeyboardHook_KeyUpEvent;
                mKeyboardHook.Stop();

            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            WeifenLuo.WinFormsUI.Docking.DockPanelSkin dockPanelSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPanelSkin();
            WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin autoHideStripSkin1 = new WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient1 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin dockPaneStripSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient dockPaneStripGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient2 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient2 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient3 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient dockPaneStripToolWindowGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient4 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient5 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient3 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient6 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient7 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VehiclePlannerUI));
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tslbAccessLv = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslbInterval = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslbUserName = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslbSpring = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslbHostIP = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslbStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslbBattery = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsprgBattery = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.systemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miLogin = new System.Windows.Forms.ToolStripMenuItem();
            this.miUserManager = new System.Windows.Forms.ToolStripMenuItem();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miMapGL = new System.Windows.Forms.ToolStripMenuItem();
            this.miTesting = new System.Windows.Forms.ToolStripMenuItem();
            this.miGoalSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.miConsole = new System.Windows.Forms.ToolStripMenuItem();
            this.miToolBox = new System.Windows.Forms.ToolStripMenuItem();
            this.miMapInsert = new System.Windows.Forms.ToolStripMenuItem();
            this.miMotionController = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.miAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miBypass = new System.Windows.Forms.ToolStripMenuItem();
            this.miBypassSocket = new System.Windows.Forms.ToolStripMenuItem();
            this.miLoadFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miServer = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dockPanel
            // 
            this.dockPanel.ActiveAutoHideContent = null;
            this.dockPanel.BackColor = System.Drawing.Color.Transparent;
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.DockBackColor = System.Drawing.SystemColors.Control;
            this.dockPanel.DockLeftPortion = 0.1D;
            this.dockPanel.DockRightPortion = 0.1D;
            this.dockPanel.Location = new System.Drawing.Point(0, 0);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size(1082, 663);
            dockPanelGradient1.EndColor = System.Drawing.SystemColors.ControlLight;
            dockPanelGradient1.StartColor = System.Drawing.SystemColors.ControlLight;
            autoHideStripSkin1.DockStripGradient = dockPanelGradient1;
            tabGradient1.EndColor = System.Drawing.SystemColors.Control;
            tabGradient1.StartColor = System.Drawing.SystemColors.Control;
            tabGradient1.TextColor = System.Drawing.SystemColors.ControlDarkDark;
            autoHideStripSkin1.TabGradient = tabGradient1;
            dockPanelSkin1.AutoHideStripSkin = autoHideStripSkin1;
            tabGradient2.EndColor = System.Drawing.SystemColors.ControlLightLight;
            tabGradient2.StartColor = System.Drawing.SystemColors.ControlLightLight;
            tabGradient2.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripGradient1.ActiveTabGradient = tabGradient2;
            dockPanelGradient2.EndColor = System.Drawing.SystemColors.Control;
            dockPanelGradient2.StartColor = System.Drawing.SystemColors.Control;
            dockPaneStripGradient1.DockStripGradient = dockPanelGradient2;
            tabGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
            tabGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
            tabGradient3.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripGradient1.InactiveTabGradient = tabGradient3;
            dockPaneStripSkin1.DocumentGradient = dockPaneStripGradient1;
            tabGradient4.EndColor = System.Drawing.SystemColors.ActiveCaption;
            tabGradient4.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            tabGradient4.StartColor = System.Drawing.SystemColors.GradientActiveCaption;
            tabGradient4.TextColor = System.Drawing.SystemColors.ActiveCaptionText;
            dockPaneStripToolWindowGradient1.ActiveCaptionGradient = tabGradient4;
            tabGradient5.EndColor = System.Drawing.SystemColors.Control;
            tabGradient5.StartColor = System.Drawing.SystemColors.Control;
            tabGradient5.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripToolWindowGradient1.ActiveTabGradient = tabGradient5;
            dockPanelGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
            dockPanelGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
            dockPaneStripToolWindowGradient1.DockStripGradient = dockPanelGradient3;
            tabGradient6.EndColor = System.Drawing.SystemColors.GradientInactiveCaption;
            tabGradient6.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            tabGradient6.StartColor = System.Drawing.SystemColors.GradientInactiveCaption;
            tabGradient6.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripToolWindowGradient1.InactiveCaptionGradient = tabGradient6;
            tabGradient7.EndColor = System.Drawing.Color.Transparent;
            tabGradient7.StartColor = System.Drawing.Color.Transparent;
            tabGradient7.TextColor = System.Drawing.SystemColors.ControlDarkDark;
            dockPaneStripToolWindowGradient1.InactiveTabGradient = tabGradient7;
            dockPaneStripSkin1.ToolWindowGradient = dockPaneStripToolWindowGradient1;
            dockPanelSkin1.DockPaneStripSkin = dockPaneStripSkin1;
            this.dockPanel.Skin = dockPanelSkin1;
            this.dockPanel.TabIndex = 3;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslbAccessLv,
            this.tslbInterval,
            this.tslbUserName,
            this.tslbSpring,
            this.tslbHostIP,
            this.tslbStatus,
            this.tslbBattery,
            this.tsprgBattery,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 635);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1082, 28);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tslbAccessLv
            // 
            this.tslbAccessLv.Name = "tslbAccessLv";
            this.tslbAccessLv.Size = new System.Drawing.Size(53, 23);
            this.tslbAccessLv.Text = "NONE";
            // 
            // tslbInterval
            // 
            this.tslbInterval.Name = "tslbInterval";
            this.tslbInterval.Size = new System.Drawing.Size(24, 23);
            this.tslbInterval.Text = "　";
            // 
            // tslbUserName
            // 
            this.tslbUserName.Name = "tslbUserName";
            this.tslbUserName.Size = new System.Drawing.Size(90, 23);
            this.tslbUserName.Text = "No Account";
            // 
            // tslbSpring
            // 
            this.tslbSpring.Name = "tslbSpring";
            this.tslbSpring.Size = new System.Drawing.Size(499, 23);
            this.tslbSpring.Spring = true;
            this.tslbSpring.Text = "　";
            // 
            // tslbHostIP
            // 
            this.tslbHostIP.Name = "tslbHostIP";
            this.tslbHostIP.Size = new System.Drawing.Size(108, 23);
            this.tslbHostIP.Text = "192.168.31.84";
            // 
            // tslbStatus
            // 
            this.tslbStatus.BackColor = System.Drawing.Color.Black;
            this.tslbStatus.ForeColor = System.Drawing.Color.Yellow;
            this.tslbStatus.Name = "tslbStatus";
            this.tslbStatus.Size = new System.Drawing.Size(60, 23);
            this.tslbStatus.Text = "OffLine";
            // 
            // tslbBattery
            // 
            this.tslbBattery.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tslbBattery.Name = "tslbBattery";
            this.tslbBattery.Size = new System.Drawing.Size(107, 23);
            this.tslbBattery.Tag = "Battery ({0}%)";
            this.tslbBattery.Text = "Battery (50%)";
            // 
            // tsprgBattery
            // 
            this.tsprgBattery.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsprgBattery.Name = "tsprgBattery";
            this.tsprgBattery.Size = new System.Drawing.Size(100, 22);
            this.tsprgBattery.Value = 50;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(24, 23);
            this.toolStripStatusLabel1.Text = "　";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.systemToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.miHelp,
            this.miBypass});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1082, 27);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // systemToolStripMenuItem
            // 
            this.systemToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miLogin,
            this.miUserManager,
            this.miExit});
            this.systemToolStripMenuItem.Name = "systemToolStripMenuItem";
            this.systemToolStripMenuItem.Size = new System.Drawing.Size(72, 23);
            this.systemToolStripMenuItem.Text = "System";
            // 
            // miLogin
            // 
            this.miLogin.Name = "miLogin";
            this.miLogin.Size = new System.Drawing.Size(183, 26);
            this.miLogin.Tag = "Logout";
            this.miLogin.Text = "Login";
            this.miLogin.Click += new System.EventHandler(this.miLogin_Click);
            // 
            // miUserManager
            // 
            this.miUserManager.Name = "miUserManager";
            this.miUserManager.Size = new System.Drawing.Size(183, 26);
            this.miUserManager.Text = "User manager";
            this.miUserManager.Visible = false;
            this.miUserManager.Click += new System.EventHandler(this.miUserManager_Click);
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(183, 26);
            this.miExit.Text = "Exit";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miMapGL,
            this.miTesting,
            this.miGoalSetting,
            this.miConsole,
            this.miToolBox,
            this.miMapInsert,
            this.miMotionController});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(55, 23);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // miMapGL
            // 
            this.miMapGL.Name = "miMapGL";
            this.miMapGL.Size = new System.Drawing.Size(208, 26);
            this.miMapGL.Text = "MapGL";
            // 
            // miTesting
            // 
            this.miTesting.Name = "miTesting";
            this.miTesting.Size = new System.Drawing.Size(208, 26);
            this.miTesting.Text = "Testing";
            // 
            // miGoalSetting
            // 
            this.miGoalSetting.Name = "miGoalSetting";
            this.miGoalSetting.Size = new System.Drawing.Size(208, 26);
            this.miGoalSetting.Text = "Goal Setting";
            // 
            // miConsole
            // 
            this.miConsole.Name = "miConsole";
            this.miConsole.Size = new System.Drawing.Size(208, 26);
            this.miConsole.Text = "Console";
            // 
            // miToolBox
            // 
            this.miToolBox.Name = "miToolBox";
            this.miToolBox.Size = new System.Drawing.Size(208, 26);
            this.miToolBox.Text = "ToolBox";
            // 
            // miMapInsert
            // 
            this.miMapInsert.Name = "miMapInsert";
            this.miMapInsert.Size = new System.Drawing.Size(208, 26);
            this.miMapInsert.Text = "MapInsert";
            this.miMapInsert.Visible = false;
            // 
            // miMotionController
            // 
            this.miMotionController.Name = "miMotionController";
            this.miMotionController.Size = new System.Drawing.Size(208, 26);
            this.miMotionController.Text = "Motion Controller";
            this.miMotionController.Click += new System.EventHandler(this.miMotionController_Click);
            // 
            // miHelp
            // 
            this.miHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAbout,
            this.testToolStripMenuItem});
            this.miHelp.Name = "miHelp";
            this.miHelp.Size = new System.Drawing.Size(53, 23);
            this.miHelp.Text = "Help";
            // 
            // miAbout
            // 
            this.miAbout.Name = "miAbout";
            this.miAbout.Size = new System.Drawing.Size(126, 26);
            this.miAbout.Text = "About";
            this.miAbout.Click += new System.EventHandler(this.miAbout_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(126, 26);
            this.testToolStripMenuItem.Text = "Test";
            // 
            // miBypass
            // 
            this.miBypass.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miBypassSocket,
            this.miLoadFile,
            this.miServer});
            this.miBypass.Name = "miBypass";
            this.miBypass.Size = new System.Drawing.Size(69, 23);
            this.miBypass.Text = "Bypass";
            this.miBypass.Visible = false;
            // 
            // miBypassSocket
            // 
            this.miBypassSocket.Name = "miBypassSocket";
            this.miBypassSocket.Size = new System.Drawing.Size(143, 26);
            this.miBypassSocket.Text = "Socket";
            this.miBypassSocket.Click += new System.EventHandler(this.miBypassSocket_Click);
            // 
            // miLoadFile
            // 
            this.miLoadFile.Name = "miLoadFile";
            this.miLoadFile.Size = new System.Drawing.Size(143, 26);
            this.miLoadFile.Text = "LoadFile";
            this.miLoadFile.Click += new System.EventHandler(this.miLoadFile_Click);
            // 
            // miServer
            // 
            this.miServer.Name = "miServer";
            this.miServer.Size = new System.Drawing.Size(143, 26);
            this.miServer.Text = "Server";
            // 
            // VehiclePlannerUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1082, 663);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.dockPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "VehiclePlannerUI";
            this.Text = "CASTEC - Vehicle planner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientUI_FormClosing);
            this.Load += new System.EventHandler(this.ClientUI_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        protected System.Windows.Forms.ToolStripStatusLabel tslbBattery;
        protected System.Windows.Forms.ToolStripProgressBar tsprgBattery;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miMapGL;
        private System.Windows.Forms.ToolStripMenuItem miTesting;
        private System.Windows.Forms.ToolStripMenuItem miGoalSetting;
        private System.Windows.Forms.ToolStripMenuItem miConsole;
        private System.Windows.Forms.ToolStripMenuItem systemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miLogin;
        private System.Windows.Forms.ToolStripMenuItem miExit;
        private System.Windows.Forms.ToolStripMenuItem miHelp;
        private System.Windows.Forms.ToolStripMenuItem miAbout;
        private System.Windows.Forms.ToolStripMenuItem miUserManager;
        private System.Windows.Forms.ToolStripStatusLabel tslbAccessLv;
        private System.Windows.Forms.ToolStripStatusLabel tslbUserName;
        private System.Windows.Forms.ToolStripStatusLabel tslbSpring;
        private System.Windows.Forms.ToolStripStatusLabel tslbInterval;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        protected System.Windows.Forms.ToolStripStatusLabel tslbStatus;
        private System.Windows.Forms.ToolStripStatusLabel tslbHostIP;
        private System.Windows.Forms.ToolStripMenuItem miBypass;
        private System.Windows.Forms.ToolStripMenuItem miBypassSocket;
        private System.Windows.Forms.ToolStripMenuItem miLoadFile;
        private System.Windows.Forms.ToolStripMenuItem miServer;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miToolBox;
        private System.Windows.Forms.ToolStripMenuItem miMapInsert;
        private System.Windows.Forms.ToolStripMenuItem miMotionController;
    }
}