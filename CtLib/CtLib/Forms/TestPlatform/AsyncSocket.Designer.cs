namespace CtLib.Forms.TestPlatform
{
    partial class Test_AsyncSockets
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Test_AsyncSockets));
            this.gbReceive = new System.Windows.Forms.GroupBox();
            this.txtReceive = new System.Windows.Forms.TextBox();
            this.gbSend = new System.Windows.Forms.GroupBox();
            this.cbEndLine = new System.Windows.Forms.ComboBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtSend = new System.Windows.Forms.TextBox();
            this.cbMode = new System.Windows.Forms.ComboBox();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.pbStt = new System.Windows.Forms.PictureBox();
            this.lbIP = new System.Windows.Forms.Label();
            this.lbPort = new System.Windows.Forms.Label();
            this.lbSrvCount = new System.Windows.Forms.Label();
            this.cbDataType = new System.Windows.Forms.ComboBox();
            this.gbReceive.SuspendLayout();
            this.gbSend.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbStt)).BeginInit();
            this.SuspendLayout();
            // 
            // gbReceive
            // 
            this.gbReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbReceive.Controls.Add(this.txtReceive);
            this.gbReceive.Font = new System.Drawing.Font("Candara", 9F);
            this.gbReceive.Location = new System.Drawing.Point(12, 135);
            this.gbReceive.Name = "gbReceive";
            this.gbReceive.Size = new System.Drawing.Size(551, 422);
            this.gbReceive.TabIndex = 11;
            this.gbReceive.TabStop = false;
            this.gbReceive.Text = "Reception";
            // 
            // txtReceive
            // 
            this.txtReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReceive.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReceive.Location = new System.Drawing.Point(16, 21);
            this.txtReceive.Multiline = true;
            this.txtReceive.Name = "txtReceive";
            this.txtReceive.ReadOnly = true;
            this.txtReceive.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReceive.Size = new System.Drawing.Size(535, 383);
            this.txtReceive.TabIndex = 4;
            // 
            // gbSend
            // 
            this.gbSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSend.Controls.Add(this.cbEndLine);
            this.gbSend.Controls.Add(this.btnSend);
            this.gbSend.Controls.Add(this.txtSend);
            this.gbSend.Font = new System.Drawing.Font("Candara", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbSend.Location = new System.Drawing.Point(12, 73);
            this.gbSend.Name = "gbSend";
            this.gbSend.Size = new System.Drawing.Size(551, 56);
            this.gbSend.TabIndex = 10;
            this.gbSend.TabStop = false;
            this.gbSend.Text = "Transmission";
            // 
            // cbEndLine
            // 
            this.cbEndLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbEndLine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEndLine.Enabled = false;
            this.cbEndLine.FormattingEnabled = true;
            this.cbEndLine.Items.AddRange(new object[] {
            "None",
            "CrLf",
            "Cr",
            "Lf"});
            this.cbEndLine.Location = new System.Drawing.Point(395, 21);
            this.cbEndLine.Name = "cbEndLine";
            this.cbEndLine.Size = new System.Drawing.Size(68, 22);
            this.cbEndLine.TabIndex = 4;
            this.cbEndLine.SelectedIndexChanged += new System.EventHandler(this.cbEndLine_SelectedIndexChanged);
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Enabled = false;
            this.btnSend.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSend.Location = new System.Drawing.Point(469, 21);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
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
            this.txtSend.Size = new System.Drawing.Size(379, 22);
            this.txtSend.TabIndex = 2;
            // 
            // cbMode
            // 
            this.cbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMode.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbMode.FormattingEnabled = true;
            this.cbMode.Items.AddRange(new object[] {
            "Client",
            "Server"});
            this.cbMode.Location = new System.Drawing.Point(89, 20);
            this.cbMode.Name = "cbMode";
            this.cbMode.Size = new System.Drawing.Size(89, 22);
            this.cbMode.TabIndex = 12;
            this.cbMode.SelectedIndexChanged += new System.EventHandler(this.cbMode_SelectedIndexChanged);
            // 
            // txtIP
            // 
            this.txtIP.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIP.Location = new System.Drawing.Point(209, 20);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(116, 22);
            this.txtIP.TabIndex = 13;
            this.txtIP.Text = "127.0.0.1";
            this.txtIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtPort
            // 
            this.txtPort.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPort.Location = new System.Drawing.Point(372, 20);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(60, 22);
            this.txtPort.TabIndex = 14;
            this.txtPort.Text = "2909";
            this.txtPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.Location = new System.Drawing.Point(443, 20);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(87, 23);
            this.btnConnect.TabIndex = 15;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // pbStt
            // 
            this.pbStt.Image = global::CtLib.Properties.Resources.Grey_Ball;
            this.pbStt.Location = new System.Drawing.Point(536, 15);
            this.pbStt.Name = "pbStt";
            this.pbStt.Size = new System.Drawing.Size(30, 30);
            this.pbStt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbStt.TabIndex = 16;
            this.pbStt.TabStop = false;
            // 
            // lbIP
            // 
            this.lbIP.AutoSize = true;
            this.lbIP.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbIP.Location = new System.Drawing.Point(185, 23);
            this.lbIP.Name = "lbIP";
            this.lbIP.Size = new System.Drawing.Size(21, 14);
            this.lbIP.TabIndex = 17;
            this.lbIP.Text = "IP";
            // 
            // lbPort
            // 
            this.lbPort.AutoSize = true;
            this.lbPort.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPort.Location = new System.Drawing.Point(334, 23);
            this.lbPort.Name = "lbPort";
            this.lbPort.Size = new System.Drawing.Size(35, 14);
            this.lbPort.TabIndex = 18;
            this.lbPort.Text = "Port";
            // 
            // lbSrvCount
            // 
            this.lbSrvCount.AutoSize = true;
            this.lbSrvCount.Font = new System.Drawing.Font("Consolas", 9F);
            this.lbSrvCount.Location = new System.Drawing.Point(442, 48);
            this.lbSrvCount.Name = "lbSrvCount";
            this.lbSrvCount.Size = new System.Drawing.Size(112, 14);
            this.lbSrvCount.TabIndex = 19;
            this.lbSrvCount.Text = "Client Count: 0";
            this.lbSrvCount.Visible = false;
            // 
            // cbDataType
            // 
            this.cbDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDataType.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbDataType.FormattingEnabled = true;
            this.cbDataType.Items.AddRange(new object[] {
            "String",
            "Byte"});
            this.cbDataType.Location = new System.Drawing.Point(15, 20);
            this.cbDataType.Name = "cbDataType";
            this.cbDataType.Size = new System.Drawing.Size(68, 22);
            this.cbDataType.TabIndex = 20;
            this.cbDataType.SelectedIndexChanged += new System.EventHandler(this.cbDataType_SelectedIndexChanged);
            // 
            // Test_AsyncSockets
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(575, 569);
            this.Controls.Add(this.cbDataType);
            this.Controls.Add(this.lbSrvCount);
            this.Controls.Add(this.lbPort);
            this.Controls.Add(this.lbIP);
            this.Controls.Add(this.pbStt);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.cbMode);
            this.Controls.Add(this.gbReceive);
            this.Controls.Add(this.gbSend);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Test_AsyncSockets";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test Platform of Socket (Async)";
            this.gbReceive.ResumeLayout(false);
            this.gbReceive.PerformLayout();
            this.gbSend.ResumeLayout(false);
            this.gbSend.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbStt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbReceive;
        private System.Windows.Forms.TextBox txtReceive;
        private System.Windows.Forms.GroupBox gbSend;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtSend;
        private System.Windows.Forms.ComboBox cbMode;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.PictureBox pbStt;
        private System.Windows.Forms.Label lbIP;
        private System.Windows.Forms.Label lbPort;
        private System.Windows.Forms.Label lbSrvCount;
        private System.Windows.Forms.ComboBox cbEndLine;
        private System.Windows.Forms.ComboBox cbDataType;

    }
}

