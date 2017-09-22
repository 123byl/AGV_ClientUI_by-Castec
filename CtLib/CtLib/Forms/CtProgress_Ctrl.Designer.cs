namespace CtLib.Forms {
    internal partial class CtProgress_Ctrl {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtProgress_Ctrl));
            this.progProcess = new System.Windows.Forms.ProgressBar();
            this.lbPercent = new System.Windows.Forms.Label();
            this.lbCaption = new System.Windows.Forms.Label();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // progProcess
            // 
            resources.ApplyResources(this.progProcess, "progProcess");
            this.progProcess.MarqueeAnimationSpeed = 5;
            this.progProcess.Name = "progProcess";
            this.progProcess.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // lbPercent
            // 
            resources.ApplyResources(this.lbPercent, "lbPercent");
            this.lbPercent.Name = "lbPercent";
            // 
            // lbCaption
            // 
            resources.ApplyResources(this.lbCaption, "lbCaption");
            this.lbCaption.Name = "lbCaption";
            this.lbCaption.UseMnemonic = false;
            // 
            // pbLogo
            // 
            resources.ApplyResources(this.pbLogo, "pbLogo");
            this.pbLogo.Image = global::CtLib.Properties.Resources.CASTEC_Logo_Vertical;
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.TabStop = false;
            // 
            // CtProgress_Ctrl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ControlBox = false;
            this.Controls.Add(this.pbLogo);
            this.Controls.Add(this.lbCaption);
            this.Controls.Add(this.lbPercent);
            this.Controls.Add(this.progProcess);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CtProgress_Ctrl";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbPercent;
        private System.Windows.Forms.Label lbCaption;
        private System.Windows.Forms.PictureBox pbLogo;
        private System.Windows.Forms.ProgressBar progProcess;
    }
}