
using CtBind;

namespace VehiclePlanner.Module.Implement {
    partial class BaseMapGL {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseMapGL));
			this.pnlShow = new System.Windows.Forms.Panel();
			this.pnlHide = new System.Windows.Forms.Panel();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.tsbOpenFile = new CtBind.Bindable.ToolStripButton();
			this.tsbSave = new CtBind.Bindable.ToolStripButton();
			this.tsbClearMap = new CtBind.Bindable.ToolStripButton();
			this.tsbConnect = new CtBind.Bindable.ToolStripButton();
			this.tsbGetMap = new CtBind.Bindable.ToolStripButton();
			this.tsbSendMap = new CtBind.Bindable.ToolStripButton();
			this.tsbChangeMap = new CtBind.Bindable.ToolStripButton();
			this.tsbScan = new CtBind.Bindable.ToolStripButton();
			this.tsbController = new CtBind.Bindable.ToolStripButton();
			this.tsbAutoReport = new CtBind.Bindable.ToolStripButton();
			this.tsbLocalization = new CtBind.Bindable.ToolStripButton();
			this.tsbConfirm = new CtBind.Bindable.ToolStripButton();
			this.tsbGetLaser = new CtBind.Bindable.ToolStripButton();
			this.tsbInsertMap = new CtBind.Bindable.ToolStripButton();
			this.pnlShow.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlShow
			// 
			this.pnlShow.Controls.Add(this.pnlHide);
			this.pnlShow.Controls.Add(this.toolStrip1);
			this.pnlShow.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlShow.Location = new System.Drawing.Point(0, 0);
			this.pnlShow.Name = "pnlShow";
			this.pnlShow.Size = new System.Drawing.Size(751, 583);
			this.pnlShow.TabIndex = 0;
			// 
			// pnlHide
			// 
			this.pnlHide.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pnlHide.BackColor = System.Drawing.SystemColors.Control;
			this.pnlHide.Location = new System.Drawing.Point(0, 42);
			this.pnlHide.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
			this.pnlHide.Name = "pnlHide";
			this.pnlHide.Size = new System.Drawing.Size(751, 541);
			this.pnlHide.TabIndex = 1;
			this.pnlHide.Visible = false;
			// 
			// toolStrip1
			// 
			this.toolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.toolStrip1.AutoSize = false;
			this.toolStrip1.BackColor = System.Drawing.SystemColors.Window;
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbOpenFile,
            this.tsbSave,
            this.tsbClearMap,
            this.tsbConnect,
            this.tsbGetMap,
            this.tsbSendMap,
            this.tsbChangeMap,
            this.tsbScan,
            this.tsbController,
            this.tsbAutoReport,
            this.tsbLocalization,
            this.tsbConfirm,
            this.tsbGetLaser,
            this.tsbInsertMap});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
			this.toolStrip1.Size = new System.Drawing.Size(751, 44);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// tsbOpenFile
			// 
			this.tsbOpenFile.AutoSize = false;
			this.tsbOpenFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbOpenFile.Image = global::VehiclePlanner.Properties.Resources.Folder_files;
			this.tsbOpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbOpenFile.Name = "tsbOpenFile";
			this.tsbOpenFile.Size = new System.Drawing.Size(32, 32);
			this.tsbOpenFile.Text = "toolStripButton1";
			this.tsbOpenFile.ToolTipText = "OpenFile";
			this.tsbOpenFile.Click += new System.EventHandler(this.tsbOpenFile_Click);
			// 
			// tsbSave
			// 
			this.tsbSave.AutoSize = false;
			this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSave.Image = global::VehiclePlanner.Properties.Resources.Save;
			this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSave.Name = "tsbSave";
			this.tsbSave.Size = new System.Drawing.Size(41, 41);
			this.tsbSave.Text = "toolStripButton1";
			this.tsbSave.ToolTipText = "Save";
			this.tsbSave.Click += new System.EventHandler(this.tsbSave_Click);
			// 
			// tsbClearMap
			// 
			this.tsbClearMap.AutoSize = false;
			this.tsbClearMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbClearMap.Image = global::VehiclePlanner.Properties.Resources.Eraser;
			this.tsbClearMap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbClearMap.Name = "tsbClearMap";
			this.tsbClearMap.Size = new System.Drawing.Size(32, 32);
			this.tsbClearMap.Text = "toolStripButton2";
			this.tsbClearMap.ToolTipText = "Clear Map";
			this.tsbClearMap.Click += new System.EventHandler(this.tsbClearMap_Click);
			// 
			// tsbConnect
			// 
			this.tsbConnect.AutoSize = false;
			this.tsbConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbConnect.Image = global::VehiclePlanner.Properties.Resources.Disconnect;
			this.tsbConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbConnect.Name = "tsbConnect";
			this.tsbConnect.Size = new System.Drawing.Size(32, 32);
			this.tsbConnect.Text = "toolStripButton1";
			this.tsbConnect.ToolTipText = "Connect to iTS";
			this.tsbConnect.Click += new System.EventHandler(this.tsbConnect_Click);
			// 
			// tsbGetMap
			// 
			this.tsbGetMap.AutoSize = false;
			this.tsbGetMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbGetMap.Image = global::VehiclePlanner.Properties.Resources.Download;
			this.tsbGetMap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbGetMap.Name = "tsbGetMap";
			this.tsbGetMap.Size = new System.Drawing.Size(32, 32);
			this.tsbGetMap.Text = "toolStripButton2";
			this.tsbGetMap.ToolTipText = "Download map";
			this.tsbGetMap.Click += new System.EventHandler(this.tsbGetMap_Click);
			// 
			// tsbSendMap
			// 
			this.tsbSendMap.AutoSize = false;
			this.tsbSendMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSendMap.Image = global::VehiclePlanner.Properties.Resources.Upload;
			this.tsbSendMap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSendMap.Name = "tsbSendMap";
			this.tsbSendMap.Size = new System.Drawing.Size(32, 32);
			this.tsbSendMap.Text = "toolStripButton3";
			this.tsbSendMap.ToolTipText = "Upload map";
			this.tsbSendMap.Click += new System.EventHandler(this.tsbSendMap_Click);
			// 
			// tsbChangeMap
			// 
			this.tsbChangeMap.AutoSize = false;
			this.tsbChangeMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbChangeMap.Image = global::VehiclePlanner.Properties.Resources.Change;
			this.tsbChangeMap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbChangeMap.Name = "tsbChangeMap";
			this.tsbChangeMap.Size = new System.Drawing.Size(32, 32);
			this.tsbChangeMap.Text = "toolStripButton1";
			this.tsbChangeMap.ToolTipText = "Change map";
			this.tsbChangeMap.Click += new System.EventHandler(this.tsbChangeMap_Click);
			// 
			// tsbScan
			// 
			this.tsbScan.AutoSize = false;
			this.tsbScan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbScan.Image = global::VehiclePlanner.Properties.Resources.NewScan;
			this.tsbScan.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbScan.Name = "tsbScan";
			this.tsbScan.Size = new System.Drawing.Size(32, 32);
			this.tsbScan.Text = "toolStripButton3";
			this.tsbScan.ToolTipText = "Map Scan";
			this.tsbScan.Click += new System.EventHandler(this.tsbScan_Click);
			// 
			// tsbController
			// 
			this.tsbController.AutoSize = false;
			this.tsbController.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbController.Image = global::VehiclePlanner.Properties.Resources.NewController;
			this.tsbController.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbController.Name = "tsbController";
			this.tsbController.Size = new System.Drawing.Size(32, 32);
			this.tsbController.Text = "toolStripButton4";
			this.tsbController.ToolTipText = "Controller";
			this.tsbController.Click += new System.EventHandler(this.tsbController_Click);
			// 
			// tsbAutoReport
			// 
			this.tsbAutoReport.AutoSize = false;
			this.tsbAutoReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbAutoReport.Image = global::VehiclePlanner.Properties.Resources.AutoResponse;
			this.tsbAutoReport.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbAutoReport.Name = "tsbAutoReport";
			this.tsbAutoReport.Size = new System.Drawing.Size(32, 32);
			this.tsbAutoReport.Text = "toolStripButton5";
			this.tsbAutoReport.ToolTipText = "Auto Report";
			this.tsbAutoReport.Click += new System.EventHandler(this.tsbCar_Click);
			// 
			// tsbLocalization
			// 
			this.tsbLocalization.AutoSize = false;
			this.tsbLocalization.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbLocalization.Image = global::VehiclePlanner.Properties.Resources.Localize;
			this.tsbLocalization.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbLocalization.Name = "tsbLocalization";
			this.tsbLocalization.Size = new System.Drawing.Size(32, 32);
			this.tsbLocalization.Text = "Set Car";
			this.tsbLocalization.ToolTipText = "Localization";
			this.tsbLocalization.Click += new System.EventHandler(this.tsbSetCar_Click);
			// 
			// tsbConfirm
			// 
			this.tsbConfirm.AutoSize = false;
			this.tsbConfirm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbConfirm.Image = global::VehiclePlanner.Properties.Resources.NewConfirm;
			this.tsbConfirm.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbConfirm.Name = "tsbConfirm";
			this.tsbConfirm.Size = new System.Drawing.Size(32, 32);
			this.tsbConfirm.Text = "toolStripButton7";
			this.tsbConfirm.ToolTipText = "Confirm";
			this.tsbConfirm.Click += new System.EventHandler(this.tsbConfirm_Click);
			// 
			// tsbGetLaser
			// 
			this.tsbGetLaser.AutoSize = false;
			this.tsbGetLaser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbGetLaser.Image = global::VehiclePlanner.Properties.Resources.Laser;
			this.tsbGetLaser.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbGetLaser.Name = "tsbGetLaser";
			this.tsbGetLaser.Size = new System.Drawing.Size(32, 32);
			this.tsbGetLaser.Text = "toolStripButton8";
			this.tsbGetLaser.ToolTipText = "Get Laser";
			this.tsbGetLaser.Click += new System.EventHandler(this.tsbGetLaser_Click);
			// 
			// tsbInsertMap
			// 
			this.tsbInsertMap.AutoSize = false;
			this.tsbInsertMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbInsertMap.Image = ((System.Drawing.Image)(resources.GetObject("tsbInsertMap.Image")));
			this.tsbInsertMap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbInsertMap.Name = "tsbInsertMap";
			this.tsbInsertMap.Size = new System.Drawing.Size(32, 32);
			this.tsbInsertMap.Text = "toolStripButton1";
			this.tsbInsertMap.ToolTipText = "Insert Map";
			this.tsbInsertMap.Click += new System.EventHandler(this.tsbInsertMap_Click);
			// 
			// BaseMapGL
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(751, 583);
			this.CloseButton = false;
			this.CloseButtonVisible = false;
			this.Controls.Add(this.pnlShow);
			this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
			this.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.Name = "BaseMapGL";
			this.Text = "iTS Map";
			this.pnlShow.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.Panel pnlShow;
        protected System.Windows.Forms.Panel pnlHide;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private Bindable.ToolStripButton tsbOpenFile;
        private Bindable.ToolStripButton tsbClearMap;
        private Bindable.ToolStripButton tsbScan;
        private Bindable.ToolStripButton tsbController;
        private Bindable.ToolStripButton tsbAutoReport;
        private Bindable.ToolStripButton tsbConfirm;
        private Bindable.ToolStripButton tsbGetLaser;
        private Bindable.ToolStripButton tsbConnect;
        private Bindable.ToolStripButton tsbGetMap;
        private Bindable.ToolStripButton tsbSendMap;
		private Bindable.ToolStripButton tsbInsertMap;
		private Bindable.ToolStripButton tsbChangeMap;
		protected Bindable.ToolStripButton tsbLocalization;
		private Bindable.ToolStripButton tsbSave;
	}
}