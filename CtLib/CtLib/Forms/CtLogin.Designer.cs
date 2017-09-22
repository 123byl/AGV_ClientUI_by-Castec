namespace CtLib.Forms {
    partial class CtLogin {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if ( disposing && ( components != null ) ) {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtLogin));
            this.lbAccount = new System.Windows.Forms.Label();
            this.lbPWD = new System.Windows.Forms.Label();
            this.txtAccount = new System.Windows.Forms.TextBox();
            this.txtPWD = new System.Windows.Forms.TextBox();
            this.lbError = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // lbAccount
            // 
            resources.ApplyResources(this.lbAccount, "lbAccount");
            this.lbAccount.Name = "lbAccount";
            // 
            // lbPWD
            // 
            resources.ApplyResources(this.lbPWD, "lbPWD");
            this.lbPWD.Name = "lbPWD";
            // 
            // txtAccount
            // 
            resources.ApplyResources(this.txtAccount, "txtAccount");
            this.txtAccount.Name = "txtAccount";
            // 
            // txtPWD
            // 
            resources.ApplyResources(this.txtPWD, "txtPWD");
            this.txtPWD.Name = "txtPWD";
            this.txtPWD.UseSystemPasswordChar = true;
            // 
            // lbError
            // 
            resources.ApplyResources(this.lbError, "lbError");
            this.lbError.ForeColor = System.Drawing.Color.Red;
            this.lbError.Name = "lbError";
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
            this.btnOK.Image = global::CtLib.Properties.Resources.Check_S;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
            this.btnCancel.Image = global::CtLib.Properties.Resources.Stop;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pbLogo
            // 
            resources.ApplyResources(this.pbLogo, "pbLogo");
            this.pbLogo.Image = global::CtLib.Properties.Resources.CASTEC_Logo_Vertical;
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.TabStop = false;
            // 
            // CtLogin
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ControlBox = false;
            this.Controls.Add(this.lbError);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtPWD);
            this.Controls.Add(this.txtAccount);
            this.Controls.Add(this.lbPWD);
            this.Controls.Add(this.lbAccount);
            this.Controls.Add(this.pbLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CtLogin";
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbLogo;
        private System.Windows.Forms.Label lbAccount;
        private System.Windows.Forms.Label lbPWD;
        private System.Windows.Forms.TextBox txtAccount;
        private System.Windows.Forms.TextBox txtPWD;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lbError;
    }
}