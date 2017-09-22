namespace CtLib.Forms.TestPlatform {
    partial class Test_Serial {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Test_Serial));
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtSend = new System.Windows.Forms.TextBox();
            this.txtReceive = new System.Windows.Forms.TextBox();
            this.cbPorts = new System.Windows.Forms.ComboBox();
            this.pbConnectStt = new System.Windows.Forms.PictureBox();
            this.gbSend = new System.Windows.Forms.GroupBox();
            this.cbNewLine = new System.Windows.Forms.ComboBox();
            this.gbReceive = new System.Windows.Forms.GroupBox();
            this.cbDataType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbConnectStt)).BeginInit();
            this.gbSend.SuspendLayout();
            this.gbReceive.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOpen
            // 
            this.btnOpen.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpen.Location = new System.Drawing.Point(202, 22);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Enabled = false;
            this.btnSend.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSend.Location = new System.Drawing.Point(359, 21);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtSend
            // 
            this.txtSend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSend.Enabled = false;
            this.txtSend.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSend.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtSend.Location = new System.Drawing.Point(12, 21);
            this.txtSend.Name = "txtSend";
            this.txtSend.Size = new System.Drawing.Size(272, 22);
            this.txtSend.TabIndex = 2;
            // 
            // txtReceive
            // 
            this.txtReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReceive.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReceive.Location = new System.Drawing.Point(10, 24);
            this.txtReceive.Multiline = true;
            this.txtReceive.Name = "txtReceive";
            this.txtReceive.ReadOnly = true;
            this.txtReceive.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReceive.Size = new System.Drawing.Size(420, 358);
            this.txtReceive.TabIndex = 4;
            // 
            // cbPorts
            // 
            this.cbPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPorts.Enabled = false;
            this.cbPorts.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbPorts.FormattingEnabled = true;
            this.cbPorts.Items.AddRange(new object[] {
            "N/A"});
            this.cbPorts.Location = new System.Drawing.Point(115, 22);
            this.cbPorts.Name = "cbPorts";
            this.cbPorts.Size = new System.Drawing.Size(81, 22);
            this.cbPorts.TabIndex = 6;
            // 
            // pbConnectStt
            // 
            this.pbConnectStt.Image = global::CtLib.Properties.Resources.Grey_Ball;
            this.pbConnectStt.Location = new System.Drawing.Point(285, 18);
            this.pbConnectStt.Name = "pbConnectStt";
            this.pbConnectStt.Size = new System.Drawing.Size(30, 30);
            this.pbConnectStt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbConnectStt.TabIndex = 7;
            this.pbConnectStt.TabStop = false;
            // 
            // gbSend
            // 
            this.gbSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSend.Controls.Add(this.cbNewLine);
            this.gbSend.Controls.Add(this.btnSend);
            this.gbSend.Controls.Add(this.txtSend);
            this.gbSend.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbSend.Location = new System.Drawing.Point(18, 65);
            this.gbSend.Name = "gbSend";
            this.gbSend.Size = new System.Drawing.Size(441, 56);
            this.gbSend.TabIndex = 8;
            this.gbSend.TabStop = false;
            this.gbSend.Text = "Transmission";
            // 
            // cbNewLine
            // 
            this.cbNewLine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNewLine.Enabled = false;
            this.cbNewLine.FormattingEnabled = true;
            this.cbNewLine.Items.AddRange(new object[] {
            "None",
            "CrLf",
            "Cr",
            "Lf"});
            this.cbNewLine.Location = new System.Drawing.Point(290, 21);
            this.cbNewLine.Name = "cbNewLine";
            this.cbNewLine.Size = new System.Drawing.Size(64, 22);
            this.cbNewLine.TabIndex = 4;
            this.cbNewLine.SelectedIndexChanged += new System.EventHandler(this.cbNewLine_SelectedIndexChanged);
            // 
            // gbReceive
            // 
            this.gbReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbReceive.Controls.Add(this.txtReceive);
            this.gbReceive.Font = new System.Drawing.Font("Candara", 9F);
            this.gbReceive.Location = new System.Drawing.Point(18, 127);
            this.gbReceive.Name = "gbReceive";
            this.gbReceive.Size = new System.Drawing.Size(439, 392);
            this.gbReceive.TabIndex = 9;
            this.gbReceive.TabStop = false;
            this.gbReceive.Text = "Reception";
            // 
            // cbDataType
            // 
            this.cbDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDataType.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbDataType.FormattingEnabled = true;
            this.cbDataType.Items.AddRange(new object[] {
            "String",
            "Byte"});
            this.cbDataType.Location = new System.Drawing.Point(28, 22);
            this.cbDataType.Name = "cbDataType";
            this.cbDataType.Size = new System.Drawing.Size(81, 22);
            this.cbDataType.TabIndex = 10;
            this.cbDataType.SelectedIndexChanged += new System.EventHandler(this.cbDataType_SelectedIndexChanged);
            // 
            // Test_Serial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 531);
            this.Controls.Add(this.cbDataType);
            this.Controls.Add(this.gbReceive);
            this.Controls.Add(this.gbSend);
            this.Controls.Add(this.pbConnectStt);
            this.Controls.Add(this.cbPorts);
            this.Controls.Add(this.btnOpen);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Test_Serial";
            this.Text = "Test Platform of Serial";
            this.Load += new System.EventHandler(this.Serial_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbConnectStt)).EndInit();
            this.gbSend.ResumeLayout(false);
            this.gbSend.PerformLayout();
            this.gbReceive.ResumeLayout(false);
            this.gbReceive.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtSend;
        private System.Windows.Forms.TextBox txtReceive;
        private System.Windows.Forms.ComboBox cbPorts;
        private System.Windows.Forms.PictureBox pbConnectStt;
        private System.Windows.Forms.GroupBox gbSend;
        private System.Windows.Forms.GroupBox gbReceive;
        private System.Windows.Forms.ComboBox cbNewLine;
        private System.Windows.Forms.ComboBox cbDataType;
    }
}