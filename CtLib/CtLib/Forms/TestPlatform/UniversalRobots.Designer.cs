namespace CtLib.Forms.TestPlatform {
    partial class Test_UniversalRobots {
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
            this.txtIP = new System.Windows.Forms.TextBox();
            this.lbIP = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.picConnect = new System.Windows.Forms.PictureBox();
            this.cbCommand = new System.Windows.Forms.ComboBox();
            this.lbCmd = new System.Windows.Forms.Label();
            this.lbPara = new System.Windows.Forms.Label();
            this.txtParameter = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.lsbxInfo = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.picConnect)).BeginInit();
            this.SuspendLayout();
            // 
            // txtIP
            // 
            this.txtIP.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIP.Location = new System.Drawing.Point(40, 18);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(124, 23);
            this.txtIP.TabIndex = 0;
            this.txtIP.Text = "192.168.0.154";
            this.txtIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbIP
            // 
            this.lbIP.AutoSize = true;
            this.lbIP.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbIP.Location = new System.Drawing.Point(19, 20);
            this.lbIP.Name = "lbIP";
            this.lbIP.Size = new System.Drawing.Size(19, 17);
            this.lbIP.TabIndex = 1;
            this.lbIP.Text = "IP";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(176, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Port";
            // 
            // txtPort
            // 
            this.txtPort.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPort.Location = new System.Drawing.Point(210, 18);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(62, 23);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "2999";
            this.txtPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.Location = new System.Drawing.Point(289, 6);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(115, 44);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // picConnect
            // 
            this.picConnect.Image = global::CtLib.Properties.Resources.Grey_Ball;
            this.picConnect.Location = new System.Drawing.Point(410, 12);
            this.picConnect.Name = "picConnect";
            this.picConnect.Size = new System.Drawing.Size(34, 33);
            this.picConnect.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picConnect.TabIndex = 5;
            this.picConnect.TabStop = false;
            // 
            // cbCommand
            // 
            this.cbCommand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCommand.Enabled = false;
            this.cbCommand.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbCommand.FormattingEnabled = true;
            this.cbCommand.Location = new System.Drawing.Point(100, 81);
            this.cbCommand.Name = "cbCommand";
            this.cbCommand.Size = new System.Drawing.Size(110, 23);
            this.cbCommand.TabIndex = 6;
            // 
            // lbCmd
            // 
            this.lbCmd.AutoSize = true;
            this.lbCmd.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCmd.Location = new System.Drawing.Point(19, 83);
            this.lbCmd.Name = "lbCmd";
            this.lbCmd.Size = new System.Drawing.Size(80, 17);
            this.lbCmd.TabIndex = 7;
            this.lbCmd.Text = "Command";
            // 
            // lbPara
            // 
            this.lbPara.AutoSize = true;
            this.lbPara.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPara.Location = new System.Drawing.Point(19, 112);
            this.lbPara.Name = "lbPara";
            this.lbPara.Size = new System.Drawing.Size(76, 17);
            this.lbPara.TabIndex = 9;
            this.lbPara.Text = "Parameter";
            // 
            // txtParameter
            // 
            this.txtParameter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtParameter.Enabled = false;
            this.txtParameter.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtParameter.Location = new System.Drawing.Point(100, 110);
            this.txtParameter.Name = "txtParameter";
            this.txtParameter.Size = new System.Drawing.Size(304, 23);
            this.txtParameter.TabIndex = 8;
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Enabled = false;
            this.btnSend.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSend.Location = new System.Drawing.Point(326, 74);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(78, 35);
            this.btnSend.TabIndex = 10;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lsbxInfo
            // 
            this.lsbxInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lsbxInfo.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lsbxInfo.FormattingEnabled = true;
            this.lsbxInfo.ItemHeight = 15;
            this.lsbxInfo.Location = new System.Drawing.Point(0, 168);
            this.lsbxInfo.Name = "lsbxInfo";
            this.lsbxInfo.Size = new System.Drawing.Size(466, 349);
            this.lsbxInfo.TabIndex = 11;
            // 
            // Test_UniversalRobots
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 517);
            this.Controls.Add(this.lsbxInfo);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.lbPara);
            this.Controls.Add(this.txtParameter);
            this.Controls.Add(this.lbCmd);
            this.Controls.Add(this.cbCommand);
            this.Controls.Add(this.picConnect);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.lbIP);
            this.Controls.Add(this.txtIP);
            this.Name = "Test_UniversalRobots";
            this.Text = "Test Platform for Universal Robots";
            ((System.ComponentModel.ISupportInitialize)(this.picConnect)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label lbIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.PictureBox picConnect;
        private System.Windows.Forms.ComboBox cbCommand;
        private System.Windows.Forms.Label lbCmd;
        private System.Windows.Forms.Label lbPara;
        private System.Windows.Forms.TextBox txtParameter;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ListBox lsbxInfo;
    }
}