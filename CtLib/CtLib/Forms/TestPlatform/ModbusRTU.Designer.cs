namespace CtLib.Forms.TestPlatform {
    partial class Test_ModbusRTU {
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
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.lstBxMsg = new System.Windows.Forms.ListBox();
            this.cbFC = new System.Windows.Forms.ComboBox();
            this.txtAddr = new System.Windows.Forms.TextBox();
            this.lbDec = new System.Windows.Forms.Label();
            this.txtData = new System.Windows.Forms.TextBox();
            this.lbHex = new System.Windows.Forms.Label();
            this.lbHex2 = new System.Windows.Forms.Label();
            this.txtCRC = new System.Windows.Forms.TextBox();
            this.btnCRC = new System.Windows.Forms.Button();
            this.nudCount = new System.Windows.Forms.NumericUpDown();
            this.cbBolVal = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudCount)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(23, 25);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(90, 32);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(431, 198);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(85, 22);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lstBxMsg
            // 
            this.lstBxMsg.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lstBxMsg.FormattingEnabled = true;
            this.lstBxMsg.ItemHeight = 12;
            this.lstBxMsg.Location = new System.Drawing.Point(0, 284);
            this.lstBxMsg.Name = "lstBxMsg";
            this.lstBxMsg.Size = new System.Drawing.Size(555, 208);
            this.lstBxMsg.TabIndex = 2;
            // 
            // cbFC
            // 
            this.cbFC.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFC.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbFC.FormattingEnabled = true;
            this.cbFC.Items.AddRange(new object[] {
            "FC01",
            "FC02",
            "FC03",
            "FC04",
            "FC05",
            "FC06",
            "FC15",
            "FC16"});
            this.cbFC.Location = new System.Drawing.Point(23, 197);
            this.cbFC.Name = "cbFC";
            this.cbFC.Size = new System.Drawing.Size(77, 23);
            this.cbFC.TabIndex = 3;
            this.cbFC.SelectedIndexChanged += new System.EventHandler(this.cbFC_SelectedIndexChanged);
            // 
            // txtAddr
            // 
            this.txtAddr.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddr.Location = new System.Drawing.Point(106, 199);
            this.txtAddr.Name = "txtAddr";
            this.txtAddr.Size = new System.Drawing.Size(76, 22);
            this.txtAddr.TabIndex = 4;
            this.txtAddr.Text = "5";
            this.txtAddr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbDec
            // 
            this.lbDec.AutoSize = true;
            this.lbDec.Location = new System.Drawing.Point(165, 224);
            this.lbDec.Name = "lbDec";
            this.lbDec.Size = new System.Drawing.Size(17, 12);
            this.lbDec.TabIndex = 5;
            this.lbDec.Text = "10";
            // 
            // txtData
            // 
            this.txtData.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtData.Location = new System.Drawing.Point(269, 199);
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(147, 22);
            this.txtData.TabIndex = 6;
            this.txtData.Text = "0";
            this.txtData.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtData.TextChanged += new System.EventHandler(this.txtData_TextChanged);
            // 
            // lbHex
            // 
            this.lbHex.AutoSize = true;
            this.lbHex.Location = new System.Drawing.Point(399, 224);
            this.lbHex.Name = "lbHex";
            this.lbHex.Size = new System.Drawing.Size(17, 12);
            this.lbHex.TabIndex = 7;
            this.lbHex.Text = "10";
            // 
            // lbHex2
            // 
            this.lbHex2.AutoSize = true;
            this.lbHex2.Location = new System.Drawing.Point(200, 98);
            this.lbHex2.Name = "lbHex2";
            this.lbHex2.Size = new System.Drawing.Size(17, 12);
            this.lbHex2.TabIndex = 10;
            this.lbHex2.Text = "16";
            // 
            // txtCRC
            // 
            this.txtCRC.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCRC.Location = new System.Drawing.Point(23, 84);
            this.txtCRC.Name = "txtCRC";
            this.txtCRC.Size = new System.Drawing.Size(177, 22);
            this.txtCRC.TabIndex = 9;
            this.txtCRC.Text = "00 00 00 00";
            this.txtCRC.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnCRC
            // 
            this.btnCRC.Location = new System.Drawing.Point(223, 84);
            this.btnCRC.Name = "btnCRC";
            this.btnCRC.Size = new System.Drawing.Size(85, 22);
            this.btnCRC.TabIndex = 8;
            this.btnCRC.Text = "CRC";
            this.btnCRC.UseVisualStyleBackColor = true;
            this.btnCRC.Click += new System.EventHandler(this.btnCRC_Click);
            // 
            // nudCount
            // 
            this.nudCount.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudCount.Location = new System.Drawing.Point(188, 198);
            this.nudCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCount.Name = "nudCount";
            this.nudCount.Size = new System.Drawing.Size(75, 23);
            this.nudCount.TabIndex = 11;
            this.nudCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cbBolVal
            // 
            this.cbBolVal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBolVal.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbBolVal.FormattingEnabled = true;
            this.cbBolVal.Items.AddRange(new object[] {
            "OFF",
            "ON"});
            this.cbBolVal.Location = new System.Drawing.Point(269, 199);
            this.cbBolVal.Name = "cbBolVal";
            this.cbBolVal.Size = new System.Drawing.Size(87, 23);
            this.cbBolVal.TabIndex = 12;
            this.cbBolVal.Visible = false;
            // 
            // Test_ModbusRTU
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 492);
            this.Controls.Add(this.cbBolVal);
            this.Controls.Add(this.nudCount);
            this.Controls.Add(this.lbHex2);
            this.Controls.Add(this.txtCRC);
            this.Controls.Add(this.btnCRC);
            this.Controls.Add(this.lbHex);
            this.Controls.Add(this.txtData);
            this.Controls.Add(this.lbDec);
            this.Controls.Add(this.txtAddr);
            this.Controls.Add(this.cbFC);
            this.Controls.Add(this.lstBxMsg);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnConnect);
            this.Name = "Test_ModbusRTU";
            this.Text = "ModbusRTU";
            ((System.ComponentModel.ISupportInitialize)(this.nudCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ListBox lstBxMsg;
        private System.Windows.Forms.ComboBox cbFC;
        private System.Windows.Forms.TextBox txtAddr;
        private System.Windows.Forms.Label lbDec;
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.Label lbHex;
        private System.Windows.Forms.Label lbHex2;
        private System.Windows.Forms.TextBox txtCRC;
        private System.Windows.Forms.Button btnCRC;
        private System.Windows.Forms.NumericUpDown nudCount;
        private System.Windows.Forms.ComboBox cbBolVal;
    }
}