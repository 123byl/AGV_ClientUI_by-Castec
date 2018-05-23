
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
            this.pnlShow = new System.Windows.Forms.Panel();
            this.pnlHide = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbOpenFile = new Bindable.ToolStripButton();
            this.tsbClearMap = new Bindable.ToolStripButton();
            this.tsbConnect = new Bindable.ToolStripButton();
            this.tsbGetMap = new Bindable.ToolStripButton();
            this.tsbScan = new Bindable.ToolStripButton();
            this.tsbController = new Bindable.ToolStripButton();
            this.tsbCar = new Bindable.ToolStripButton();
            this.tsbSetCar = new Bindable.ToolStripButton();
            this.tsbConfirm = new Bindable.ToolStripButton();
            this.tsbGetLaser = new Bindable.ToolStripButton();
            this.tsbSendMap = new Bindable.ToolStripButton();
            this.pnlShow.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlShow
            // 
            this.pnlShow.Controls.Add(this.pnlHide);
            this.pnlShow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlShow.Location = new System.Drawing.Point(0, 27);
            this.pnlShow.Name = "pnlShow";
            this.pnlShow.Size = new System.Drawing.Size(751, 556);
            this.pnlShow.TabIndex = 0;
            // 
            // pnlHide
            // 
            this.pnlHide.BackColor = System.Drawing.SystemColors.Control;
            this.pnlHide.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHide.Location = new System.Drawing.Point(0, 0);
            this.pnlHide.Name = "pnlHide";
            this.pnlHide.Size = new System.Drawing.Size(751, 556);
            this.pnlHide.TabIndex = 1;
            this.pnlHide.Visible = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbOpenFile,
            this.tsbClearMap,
            this.tsbConnect,
            this.tsbGetMap,
            this.tsbSendMap,
            this.tsbScan,
            this.tsbController,
            this.tsbCar,
            this.tsbSetCar,
            this.tsbConfirm,
            this.tsbGetLaser});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(751, 27);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbOpenFile
            // 
            this.tsbOpenFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpenFile.Image = global::VehiclePlanner.Properties.Resources.Folder_files;
            this.tsbOpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpenFile.Name = "tsbOpenFile";
            this.tsbOpenFile.Size = new System.Drawing.Size(24, 24);
            this.tsbOpenFile.Text = "toolStripButton1";
            this.tsbOpenFile.ToolTipText = "OpenFile(Ctrl + O)";
            this.tsbOpenFile.Click += new System.EventHandler(this.tsbOpenFile_Click);
            // 
            // tsbClearMap
            // 
            this.tsbClearMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClearMap.Image = global::VehiclePlanner.Properties.Resources.Eraser;
            this.tsbClearMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClearMap.Name = "tsbClearMap";
            this.tsbClearMap.Size = new System.Drawing.Size(24, 24);
            this.tsbClearMap.Text = "toolStripButton2";
            this.tsbClearMap.ToolTipText = "Clear Map(Ctrl + E)";
            this.tsbClearMap.Click += new System.EventHandler(this.tsbClearMap_Click);
            // 
            // tsbConnect
            // 
            this.tsbConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConnect.Image = global::VehiclePlanner.Properties.Resources.Disconnect;
            this.tsbConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConnect.Name = "tsbConnect";
            this.tsbConnect.Size = new System.Drawing.Size(24, 24);
            this.tsbConnect.Text = "toolStripButton1";
            this.tsbConnect.ToolTipText = "Connect to iTS(Ctrl + C)";
            this.tsbConnect.Click += new System.EventHandler(this.tsbConnect_Click);
            // 
            // tsbGetMap
            // 
            this.tsbGetMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGetMap.Image = global::VehiclePlanner.Properties.Resources.Download;
            this.tsbGetMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGetMap.Name = "tsbGetMap";
            this.tsbGetMap.Size = new System.Drawing.Size(24, 24);
            this.tsbGetMap.Text = "toolStripButton2";
            this.tsbGetMap.ToolTipText = "Download map(Ctrl + D)";
            this.tsbGetMap.Click += new System.EventHandler(this.tsbGetMap_Click);
            // 
            // tsbScan
            // 
            this.tsbScan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbScan.Image = global::VehiclePlanner.Properties.Resources.Scan;
            this.tsbScan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbScan.Name = "tsbScan";
            this.tsbScan.Size = new System.Drawing.Size(24, 24);
            this.tsbScan.Text = "toolStripButton3";
            this.tsbScan.ToolTipText = "Map Scan(Ctrl + S)";
            this.tsbScan.Click += new System.EventHandler(this.tsbScan_Click);
            // 
            // tsbController
            // 
            this.tsbController.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbController.Image = global::VehiclePlanner.Properties.Resources.Controller;
            this.tsbController.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbController.Name = "tsbController";
            this.tsbController.Size = new System.Drawing.Size(24, 24);
            this.tsbController.Text = "toolStripButton4";
            this.tsbController.ToolTipText = "Controller(Ctrl + M)";
            this.tsbController.Click += new System.EventHandler(this.tsbController_Click);
            // 
            // tsbCar
            // 
            this.tsbCar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCar.Image = global::VehiclePlanner.Properties.Resources.Radar;
            this.tsbCar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCar.Name = "tsbCar";
            this.tsbCar.Size = new System.Drawing.Size(24, 24);
            this.tsbCar.Text = "toolStripButton5";
            this.tsbCar.ToolTipText = "Car(Ctrl + G)";
            this.tsbCar.Click += new System.EventHandler(this.tsbCar_Click);
            // 
            // tsbSetCar
            // 
            this.tsbSetCar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSetCar.Image = global::VehiclePlanner.Properties.Resources.Hand;
            this.tsbSetCar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSetCar.Name = "tsbSetCar";
            this.tsbSetCar.Size = new System.Drawing.Size(24, 24);
            this.tsbSetCar.Text = "Set Car";
            this.tsbSetCar.ToolTipText = "Set Car(Ctrl + P)";
            this.tsbSetCar.Click += new System.EventHandler(this.tsbSetCar_Click);
            // 
            // tsbConfirm
            // 
            this.tsbConfirm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConfirm.Image = global::VehiclePlanner.Properties.Resources.Zoom;
            this.tsbConfirm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConfirm.Name = "tsbConfirm";
            this.tsbConfirm.Size = new System.Drawing.Size(24, 24);
            this.tsbConfirm.Text = "toolStripButton7";
            this.tsbConfirm.ToolTipText = "Confirm(Ctrl + F)";
            this.tsbConfirm.Click += new System.EventHandler(this.tsbConfirm_Click);
            // 
            // tsbGetLaser
            // 
            this.tsbGetLaser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGetLaser.Image = global::VehiclePlanner.Properties.Resources.Laser;
            this.tsbGetLaser.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGetLaser.Name = "tsbGetLaser";
            this.tsbGetLaser.Size = new System.Drawing.Size(24, 24);
            this.tsbGetLaser.Text = "toolStripButton8";
            this.tsbGetLaser.ToolTipText = "Get Laser(Ctrl + L)";
            this.tsbGetLaser.Click += new System.EventHandler(this.tsbGetLaser_Click);
            // 
            // tsbSendMap
            // 
            this.tsbSendMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSendMap.Image = global::VehiclePlanner.Properties.Resources.Upload;
            this.tsbSendMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSendMap.Name = "tsbSendMap";
            this.tsbSendMap.Size = new System.Drawing.Size(24, 24);
            this.tsbSendMap.Text = "toolStripButton3";
            this.tsbSendMap.ToolTipText = "Upload map(Ctrl + U)";
            this.tsbSendMap.Click += new System.EventHandler(this.tsbSendMap_Click);
            // 
            // BaseMapGL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 583);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.pnlShow);
            this.Controls.Add(this.toolStrip1);
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            this.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Name = "BaseMapGL";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Unknown;
            this.Text = "iTS Map";
            this.pnlShow.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Panel pnlShow;
        protected System.Windows.Forms.Panel pnlHide;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private Bindable.ToolStripButton tsbOpenFile;
        private Bindable.ToolStripButton tsbClearMap;
        private Bindable.ToolStripButton tsbScan;
        private Bindable.ToolStripButton tsbController;
        private Bindable.ToolStripButton tsbCar;
        private Bindable.ToolStripButton tsbSetCar;
        private Bindable.ToolStripButton tsbConfirm;
        private Bindable.ToolStripButton tsbGetLaser;
        private Bindable.ToolStripButton tsbConnect;
        private Bindable.ToolStripButton tsbGetMap;
        private Bindable.ToolStripButton tsbSendMap;
    }
}