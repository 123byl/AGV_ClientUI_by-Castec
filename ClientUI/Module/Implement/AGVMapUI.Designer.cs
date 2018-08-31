
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
			this.tsbInsertMap = new CtBind.Bindable.ToolStripButton();
			this.tsbConnect = new CtBind.Bindable.ToolStripButton();
			this.tsbGetMap = new CtBind.Bindable.ToolStripButton();
			this.tsbSendMap = new CtBind.Bindable.ToolStripButton();
			this.tsbChangeMap = new CtBind.Bindable.ToolStripButton();
			this.tsbScan = new CtBind.Bindable.ToolStripButton();
			this.tsbController = new CtBind.Bindable.ToolStripButton();
			this.tsbAutoReport = new CtBind.Bindable.ToolStripButton();
			this.tsbLocalization = new CtBind.Bindable.ToolStripButton();
			this.tsbConfirm = new CtBind.Bindable.ToolStripButton();
			this.tsbMove = new CtBind.Bindable.ToolStripButton();
			this.tsbGetLaser = new CtBind.Bindable.ToolStripButton();
			this.tsbFocus = new CtBind.Bindable.ToolStripButton();
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
			this.pnlHide.Location = new System.Drawing.Point(0, 52);
			this.pnlHide.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
			this.pnlHide.Name = "pnlHide";
			this.pnlHide.Size = new System.Drawing.Size(751, 531);
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
            this.tsbInsertMap,
            this.tsbConnect,
            this.tsbGetMap,
            this.tsbSendMap,
            this.tsbChangeMap,
            this.tsbScan,
            this.tsbController,
            this.tsbAutoReport,
            this.tsbLocalization,
            this.tsbConfirm,
            this.tsbMove,
            this.tsbFocus,
            this.tsbGetLaser});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip1.Size = new System.Drawing.Size(751, 50);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// tsbOpenFile
			// 
			this.tsbOpenFile.AutoSize = false;
			this.tsbOpenFile.Font = new System.Drawing.Font("Microsoft JhengHei UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.tsbOpenFile.Image = global::VehiclePlanner.Properties.Resources.Folder_files;
			this.tsbOpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbOpenFile.Name = "tsbOpenFile";
			this.tsbOpenFile.Size = new System.Drawing.Size(60, 40);
			this.tsbOpenFile.Text = "Open";
			this.tsbOpenFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsbOpenFile.ToolTipText = "OpenFile";
			this.tsbOpenFile.Click += new System.EventHandler(this.tsbOpenFile_Click);
			// 
			// tsbSave
			// 
			this.tsbSave.AutoSize = false;
			this.tsbSave.Font = new System.Drawing.Font("Microsoft JhengHei UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.tsbSave.Image = global::VehiclePlanner.Properties.Resources.Save;
			this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSave.Name = "tsbSave";
			this.tsbSave.Size = new System.Drawing.Size(60, 40);
			this.tsbSave.Text = "Save";
			this.tsbSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsbSave.ToolTipText = "Save";
			this.tsbSave.Click += new System.EventHandler(this.tsbSave_Click);
			// 
			// tsbClearMap
			// 
			this.tsbClearMap.AutoSize = false;
			this.tsbClearMap.Font = new System.Drawing.Font("Microsoft JhengHei UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.tsbClearMap.Image = global::VehiclePlanner.Properties.Resources.Eraser;
			this.tsbClearMap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbClearMap.Name = "tsbClearMap";
			this.tsbClearMap.Size = new System.Drawing.Size(60, 40);
			this.tsbClearMap.Text = "Clear";
			this.tsbClearMap.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsbClearMap.ToolTipText = "Clear Map";
			this.tsbClearMap.Click += new System.EventHandler(this.tsbClearMap_Click);
			// 
			// tsbInsertMap
			// 
			this.tsbInsertMap.AutoSize = false;
			this.tsbInsertMap.Font = new System.Drawing.Font("Microsoft JhengHei UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.tsbInsertMap.Image = ((System.Drawing.Image)(resources.GetObject("tsbInsertMap.Image")));
			this.tsbInsertMap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbInsertMap.Name = "tsbInsertMap";
			this.tsbInsertMap.Size = new System.Drawing.Size(60, 40);
			this.tsbInsertMap.Text = "Insert";
			this.tsbInsertMap.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsbInsertMap.ToolTipText = "Insert Map";
			this.tsbInsertMap.Click += new System.EventHandler(this.tsbInsertMap_Click);
			// 
			// tsbConnect
			// 
			this.tsbConnect.AutoSize = false;
			this.tsbConnect.Font = new System.Drawing.Font("Microsoft JhengHei UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.tsbConnect.Image = global::VehiclePlanner.Properties.Resources.Disconnect;
			this.tsbConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbConnect.Name = "tsbConnect";
			this.tsbConnect.Size = new System.Drawing.Size(60, 40);
			this.tsbConnect.Text = "Connect";
			this.tsbConnect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsbConnect.ToolTipText = "Connect to iTS";
			this.tsbConnect.Click += new System.EventHandler(this.tsbConnect_Click);
			// 
			// tsbGetMap
			// 
			this.tsbGetMap.AutoSize = false;
			this.tsbGetMap.Font = new System.Drawing.Font("Microsoft JhengHei UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.tsbGetMap.Image = global::VehiclePlanner.Properties.Resources.Download;
			this.tsbGetMap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbGetMap.Name = "tsbGetMap";
			this.tsbGetMap.Size = new System.Drawing.Size(60, 40);
			this.tsbGetMap.Text = "Download";
			this.tsbGetMap.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsbGetMap.ToolTipText = "Download map";
			this.tsbGetMap.Click += new System.EventHandler(this.tsbGetMap_Click);
			// 
			// tsbSendMap
			// 
			this.tsbSendMap.AutoSize = false;
			this.tsbSendMap.Font = new System.Drawing.Font("Microsoft JhengHei UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.tsbSendMap.Image = global::VehiclePlanner.Properties.Resources.Upload;
			this.tsbSendMap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSendMap.Name = "tsbSendMap";
			this.tsbSendMap.Size = new System.Drawing.Size(60, 40);
			this.tsbSendMap.Text = "Upload";
			this.tsbSendMap.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsbSendMap.ToolTipText = "Upload map";
			this.tsbSendMap.Click += new System.EventHandler(this.tsbSendMap_Click);
			// 
			// tsbChangeMap
			// 
			this.tsbChangeMap.AutoSize = false;
			this.tsbChangeMap.Font = new System.Drawing.Font("Microsoft JhengHei UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.tsbChangeMap.Image = global::VehiclePlanner.Properties.Resources.Change;
			this.tsbChangeMap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbChangeMap.Name = "tsbChangeMap";
			this.tsbChangeMap.Size = new System.Drawing.Size(60, 40);
			this.tsbChangeMap.Text = "Chage";
			this.tsbChangeMap.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsbChangeMap.ToolTipText = "Change map";
			this.tsbChangeMap.Click += new System.EventHandler(this.tsbChangeMap_Click);
			// 
			// tsbScan
			// 
			this.tsbScan.AutoSize = false;
			this.tsbScan.Font = new System.Drawing.Font("Microsoft JhengHei UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.tsbScan.Image = global::VehiclePlanner.Properties.Resources.NewScan;
			this.tsbScan.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbScan.Name = "tsbScan";
			this.tsbScan.Size = new System.Drawing.Size(60, 40);
			this.tsbScan.Text = "Scan";
			this.tsbScan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsbScan.ToolTipText = "Map Scan";
			this.tsbScan.Click += new System.EventHandler(this.tsbScan_Click);
			// 
			// tsbController
			// 
			this.tsbController.AutoSize = false;
			this.tsbController.Font = new System.Drawing.Font("Microsoft JhengHei UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.tsbController.Image = global::VehiclePlanner.Properties.Resources.NewController;
			this.tsbController.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbController.Name = "tsbController";
			this.tsbController.Size = new System.Drawing.Size(60, 40);
			this.tsbController.Text = "Control";
			this.tsbController.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsbController.ToolTipText = "Controller";
			this.tsbController.Click += new System.EventHandler(this.tsbController_Click);
			// 
			// tsbAutoReport
			// 
			this.tsbAutoReport.AutoSize = false;
			this.tsbAutoReport.Font = new System.Drawing.Font("Microsoft JhengHei UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.tsbAutoReport.Image = global::VehiclePlanner.Properties.Resources.AutoResponse;
			this.tsbAutoReport.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbAutoReport.Name = "tsbAutoReport";
			this.tsbAutoReport.Size = new System.Drawing.Size(60, 40);
			this.tsbAutoReport.Text = "Auto";
			this.tsbAutoReport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsbAutoReport.ToolTipText = "Auto Report";
			this.tsbAutoReport.Click += new System.EventHandler(this.tsbCar_Click);
			// 
			// tsbLocalization
			// 
			this.tsbLocalization.AutoSize = false;
			this.tsbLocalization.BackColor = System.Drawing.SystemColors.Window;
			this.tsbLocalization.Font = new System.Drawing.Font("Microsoft JhengHei UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.tsbLocalization.Image = global::VehiclePlanner.Properties.Resources.Localize;
			this.tsbLocalization.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbLocalization.Name = "tsbLocalization";
			this.tsbLocalization.Size = new System.Drawing.Size(60, 40);
			this.tsbLocalization.Text = "Localize";
			this.tsbLocalization.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsbLocalization.ToolTipText = "Localization";
			this.tsbLocalization.Click += new System.EventHandler(this.tsbSetCar_Click);
			// 
			// tsbConfirm
			// 
			this.tsbConfirm.AutoSize = false;
			this.tsbConfirm.Font = new System.Drawing.Font("Microsoft JhengHei UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.tsbConfirm.Image = global::VehiclePlanner.Properties.Resources.NewConfirm;
			this.tsbConfirm.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbConfirm.Name = "tsbConfirm";
			this.tsbConfirm.Size = new System.Drawing.Size(60, 40);
			this.tsbConfirm.Text = "Confirm";
			this.tsbConfirm.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsbConfirm.ToolTipText = "Confirm";
			this.tsbConfirm.Click += new System.EventHandler(this.tsbConfirm_Click);
			// 
			// tsbMove
			// 
			this.tsbMove.AutoSize = false;
			this.tsbMove.Font = new System.Drawing.Font("Microsoft JhengHei UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.tsbMove.Image = global::VehiclePlanner.Properties.Resources.Movement;
			this.tsbMove.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbMove.Name = "tsbMove";
			this.tsbMove.Size = new System.Drawing.Size(60, 40);
			this.tsbMove.Text = "Move";
			this.tsbMove.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
			this.tsbMove.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsbMove.Click += new System.EventHandler(this.tsbMove_Click);
			// 
			// tsbGetLaser
			// 
			this.tsbGetLaser.AutoSize = false;
			this.tsbGetLaser.Font = new System.Drawing.Font("Microsoft JhengHei UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.tsbGetLaser.Image = global::VehiclePlanner.Properties.Resources.Laser;
			this.tsbGetLaser.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbGetLaser.Name = "tsbGetLaser";
			this.tsbGetLaser.Size = new System.Drawing.Size(60, 40);
			this.tsbGetLaser.Text = "Laser";
			this.tsbGetLaser.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsbGetLaser.ToolTipText = "Get Laser";
			this.tsbGetLaser.Click += new System.EventHandler(this.tsbGetLaser_Click);
			// 
			// tsbFocus
			// 
			this.tsbFocus.AutoSize = false;
			this.tsbFocus.Font = new System.Drawing.Font("Microsoft JhengHei UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.tsbFocus.Image = global::VehiclePlanner.Properties.Resources.Focus;
			this.tsbFocus.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbFocus.Name = "tsbFocus";
			this.tsbFocus.Size = new System.Drawing.Size(60, 40);
			this.tsbFocus.Text = "Focus";
			this.tsbFocus.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.tsbFocus.Click += new System.EventHandler(this.tsbFocus_Click);
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
		protected Bindable.ToolStripButton tsbLocalization;
		protected Bindable.ToolStripButton tsbScan;
		protected Bindable.ToolStripButton tsbController;
		protected Bindable.ToolStripButton tsbAutoReport;
		protected Bindable.ToolStripButton tsbConfirm;
		protected Bindable.ToolStripButton tsbGetLaser;
		protected Bindable.ToolStripButton tsbGetMap;
		protected Bindable.ToolStripButton tsbSendMap;
		protected Bindable.ToolStripButton tsbChangeMap;
		protected Bindable.ToolStripButton tsbOpenFile;
		protected Bindable.ToolStripButton tsbClearMap;
		protected Bindable.ToolStripButton tsbConnect;
		protected Bindable.ToolStripButton tsbInsertMap;
		protected Bindable.ToolStripButton tsbSave;
		protected Bindable.ToolStripButton tsbMove;
		protected Bindable.ToolStripButton tsbFocus;
	}
}