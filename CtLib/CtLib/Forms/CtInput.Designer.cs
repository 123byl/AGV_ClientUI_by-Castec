namespace CtLib.Forms {
    partial class CtInput {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtInput));
            this.lbInfo = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.cbItem = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // lbInfo
            // 
            resources.ApplyResources(this.lbInfo, "lbInfo");
            this.lbInfo.Name = "lbInfo";
            // 
            // txtInput
            // 
            resources.ApplyResources(this.txtInput, "txtInput");
            this.txtInput.Name = "txtInput";
            this.txtInput.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            // 
            // cbItem
            // 
            resources.ApplyResources(this.cbItem, "cbItem");
            this.cbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbItem.FormattingEnabled = true;
            this.cbItem.Name = "cbItem";
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
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
            // CtInput
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ControlBox = false;
            this.Controls.Add(this.cbItem);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.lbInfo);
            this.Controls.Add(this.pbLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CtInput";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbLogo;
        private System.Windows.Forms.Label lbInfo;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cbItem;
    }
}