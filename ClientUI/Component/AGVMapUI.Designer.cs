using GLUI;

namespace VehiclePlanner {
    partial class AGVMapUI {
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
            this.uiControl = new GLUI.GLUserControl();
            this.pnlShow.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlShow
            // 
            this.pnlShow.Controls.Add(this.pnlHide);
            this.pnlShow.Controls.Add(this.uiControl);
            this.pnlShow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlShow.Location = new System.Drawing.Point(0, 0);
            this.pnlShow.Name = "pnlShow";
            this.pnlShow.Size = new System.Drawing.Size(751, 583);
            this.pnlShow.TabIndex = 0;
            // 
            // pnlHide
            // 
            this.pnlHide.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHide.Location = new System.Drawing.Point(0, 0);
            this.pnlHide.Name = "pnlHide";
            this.pnlHide.Size = new System.Drawing.Size(751, 583);
            this.pnlHide.TabIndex = 1;
            this.pnlHide.Visible = false;
            // 
            // uiControl
            // 
            this.uiControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uiControl.Location = new System.Drawing.Point(12, 12);
            this.uiControl.Name = "uiControl";
            this.uiControl.Size = new System.Drawing.Size(727, 559);
            this.uiControl.TabIndex = 1;
            // 
            // AGVMapUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 583);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.pnlShow);
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            this.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Name = "AGVMapUI";
            this.Text = "iTS Map";
            this.pnlShow.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlShow;
        private GLUserControl uiControl;
        private System.Windows.Forms.Panel pnlHide;
    }
}