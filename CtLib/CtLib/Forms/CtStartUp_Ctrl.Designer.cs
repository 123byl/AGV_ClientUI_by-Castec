namespace CtLib.Forms {
    internal partial class CtStartUp_Ctrl {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtStartUp_Ctrl));
            this.progProcess = new System.Windows.Forms.ProgressBar();
            this.lbInfo = new System.Windows.Forms.Label();
            this.lbProc = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progProcess
            // 
            this.progProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progProcess.Location = new System.Drawing.Point(208, 202);
            this.progProcess.Name = "progProcess";
            this.progProcess.Size = new System.Drawing.Size(241, 21);
            this.progProcess.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progProcess.TabIndex = 0;
            // 
            // lbInfo
            // 
            this.lbInfo.BackColor = System.Drawing.Color.White;
            this.lbInfo.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbInfo.Location = new System.Drawing.Point(205, 173);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(284, 17);
            this.lbInfo.TabIndex = 1;
            this.lbInfo.Text = "Information";
            this.lbInfo.UseWaitCursor = true;
            // 
            // lbProc
            // 
            this.lbProc.AutoSize = true;
            this.lbProc.BackColor = System.Drawing.Color.White;
            this.lbProc.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProc.Location = new System.Drawing.Point(455, 203);
            this.lbProc.Name = "lbProc";
            this.lbProc.Size = new System.Drawing.Size(35, 19);
            this.lbProc.TabIndex = 2;
            this.lbProc.Text = "99%";
            this.lbProc.UseWaitCursor = true;
            // 
            // CtStartUp_Ctrl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = global::CtLib.Properties.Resources.StartupLogo;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(501, 251);
            this.ControlBox = false;
            this.Controls.Add(this.lbProc);
            this.Controls.Add(this.lbInfo);
            this.Controls.Add(this.progProcess);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CtStartUp_Ctrl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CAMPro Starting...";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CtStartUp_Ctrl_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CtStartUp_Ctrl_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progProcess;
        private System.Windows.Forms.Label lbInfo;
        private System.Windows.Forms.Label lbProc;
    }
}