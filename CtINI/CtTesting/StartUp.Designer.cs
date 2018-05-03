namespace INITesting {
    partial class StartUp {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent() {
            this.btnIni = new System.Windows.Forms.Button();
            this.btnSetting = new System.Windows.Forms.Button();
            this.btnParamGenarator = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnIni
            // 
            this.btnIni.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnIni.Location = new System.Drawing.Point(12, 12);
            this.btnIni.Name = "btnIni";
            this.btnIni.Size = new System.Drawing.Size(74, 40);
            this.btnIni.TabIndex = 0;
            this.btnIni.Text = "INI";
            this.btnIni.UseVisualStyleBackColor = true;
            this.btnIni.Click += new System.EventHandler(this.btnIni_Click);
            // 
            // btnSetting
            // 
            this.btnSetting.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSetting.Location = new System.Drawing.Point(12, 58);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Size = new System.Drawing.Size(128, 40);
            this.btnSetting.TabIndex = 1;
            this.btnSetting.Text = "Properties Setting";
            this.btnSetting.UseVisualStyleBackColor = true;
            this.btnSetting.Click += new System.EventHandler(this.btnSetting_Click);
            // 
            // btnParamGenarator
            // 
            this.btnParamGenarator.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnParamGenarator.Location = new System.Drawing.Point(12, 104);
            this.btnParamGenarator.Name = "btnParamGenarator";
            this.btnParamGenarator.Size = new System.Drawing.Size(192, 40);
            this.btnParamGenarator.TabIndex = 2;
            this.btnParamGenarator.Text = "Param Genarator";
            this.btnParamGenarator.UseVisualStyleBackColor = true;
            this.btnParamGenarator.Click += new System.EventHandler(this.btnParamGenarator_Click);
            // 
            // StartUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.btnParamGenarator);
            this.Controls.Add(this.btnSetting);
            this.Controls.Add(this.btnIni);
            this.Name = "StartUp";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnIni;
        private System.Windows.Forms.Button btnSetting;
        private System.Windows.Forms.Button btnParamGenarator;
    }
}

