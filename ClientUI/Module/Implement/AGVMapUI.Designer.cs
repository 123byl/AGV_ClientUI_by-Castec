﻿
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
            this.tsbOpenFile = new System.Windows.Forms.ToolStripButton();
            this.tsbClearMap = new System.Windows.Forms.ToolStripButton();
            this.tsbScan = new System.Windows.Forms.ToolStripButton();
            this.tsbController = new System.Windows.Forms.ToolStripButton();
            this.tsbCar = new System.Windows.Forms.ToolStripButton();
            this.tsbSetCar = new System.Windows.Forms.ToolStripButton();
            this.tsbConfirm = new System.Windows.Forms.ToolStripButton();
            this.tsbGetLaser = new System.Windows.Forms.ToolStripButton();
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
            this.tsbOpenFile.ToolTipText = "OpenFile";
            // 
            // tsbClearMap
            // 
            this.tsbClearMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClearMap.Image = global::VehiclePlanner.Properties.Resources.Eraser;
            this.tsbClearMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClearMap.Name = "tsbClearMap";
            this.tsbClearMap.Size = new System.Drawing.Size(24, 24);
            this.tsbClearMap.Text = "toolStripButton2";
            this.tsbClearMap.ToolTipText = "Clear Map";
            // 
            // tsbScan
            // 
            this.tsbScan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbScan.Image = global::VehiclePlanner.Properties.Resources.Scan;
            this.tsbScan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbScan.Name = "tsbScan";
            this.tsbScan.Size = new System.Drawing.Size(24, 24);
            this.tsbScan.Text = "toolStripButton3";
            // 
            // tsbController
            // 
            this.tsbController.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbController.Image = global::VehiclePlanner.Properties.Resources.Controller;
            this.tsbController.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbController.Name = "tsbController";
            this.tsbController.Size = new System.Drawing.Size(24, 24);
            this.tsbController.Text = "toolStripButton4";
            this.tsbController.ToolTipText = "Controller";
            // 
            // tsbCar
            // 
            this.tsbCar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCar.Image = global::VehiclePlanner.Properties.Resources.Radar;
            this.tsbCar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCar.Name = "tsbCar";
            this.tsbCar.Size = new System.Drawing.Size(24, 24);
            this.tsbCar.Text = "toolStripButton5";
            this.tsbCar.ToolTipText = "Car";
            // 
            // tsbSetCar
            // 
            this.tsbSetCar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSetCar.Image = global::VehiclePlanner.Properties.Resources.Hand;
            this.tsbSetCar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSetCar.Name = "tsbSetCar";
            this.tsbSetCar.Size = new System.Drawing.Size(24, 24);
            this.tsbSetCar.Text = "Set Car";
            // 
            // tsbConfirm
            // 
            this.tsbConfirm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConfirm.Image = global::VehiclePlanner.Properties.Resources.Zoom;
            this.tsbConfirm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConfirm.Name = "tsbConfirm";
            this.tsbConfirm.Size = new System.Drawing.Size(24, 24);
            this.tsbConfirm.Text = "toolStripButton7";
            this.tsbConfirm.ToolTipText = "Confirm";
            // 
            // tsbGetLaser
            // 
            this.tsbGetLaser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGetLaser.Image = global::VehiclePlanner.Properties.Resources.Laser;
            this.tsbGetLaser.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGetLaser.Name = "tsbGetLaser";
            this.tsbGetLaser.Size = new System.Drawing.Size(24, 24);
            this.tsbGetLaser.Text = "toolStripButton8";
            this.tsbGetLaser.ToolTipText = "Get Laser";
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
        private System.Windows.Forms.ToolStripButton tsbOpenFile;
        private System.Windows.Forms.ToolStripButton tsbClearMap;
        private System.Windows.Forms.ToolStripButton tsbScan;
        private System.Windows.Forms.ToolStripButton tsbController;
        private System.Windows.Forms.ToolStripButton tsbCar;
        private System.Windows.Forms.ToolStripButton tsbSetCar;
        private System.Windows.Forms.ToolStripButton tsbConfirm;
        private System.Windows.Forms.ToolStripButton tsbGetLaser;
    }
}