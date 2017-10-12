namespace CtLib.Forms.TestPlatform {
    partial class Keyence_DL_RS1A {
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
			this.picCntStt = new System.Windows.Forms.PictureBox();
			this.gbGetSngVal = new System.Windows.Forms.GroupBox();
			this.btnGetSngVal = new System.Windows.Forms.Button();
			this.txtSngVal = new System.Windows.Forms.TextBox();
			this.lbSngVal = new System.Windows.Forms.Label();
			this.lbSngID = new System.Windows.Forms.Label();
			this.nudSngID = new System.Windows.Forms.NumericUpDown();
			this.gbGetAllVal = new System.Windows.Forms.GroupBox();
			this.btnGetAllVal = new System.Windows.Forms.Button();
			this.txtAllVal = new System.Windows.Forms.TextBox();
			this.lbAllVal = new System.Windows.Forms.Label();
			this.gbFunc = new System.Windows.Forms.GroupBox();
			this.btnZero = new System.Windows.Forms.Button();
			this.lsbxMsg = new System.Windows.Forms.ListBox();
			((System.ComponentModel.ISupportInitialize)(this.picCntStt)).BeginInit();
			this.gbGetSngVal.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudSngID)).BeginInit();
			this.gbGetAllVal.SuspendLayout();
			this.gbFunc.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnConnect
			// 
			this.btnConnect.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnConnect.Location = new System.Drawing.Point(16, 19);
			this.btnConnect.Margin = new System.Windows.Forms.Padding(2);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(116, 42);
			this.btnConnect.TabIndex = 0;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// picCntStt
			// 
			this.picCntStt.Image = global::CtLib.Properties.Resources.Grey_Ball;
			this.picCntStt.Location = new System.Drawing.Point(137, 26);
			this.picCntStt.Margin = new System.Windows.Forms.Padding(2);
			this.picCntStt.Name = "picCntStt";
			this.picCntStt.Size = new System.Drawing.Size(26, 30);
			this.picCntStt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.picCntStt.TabIndex = 1;
			this.picCntStt.TabStop = false;
			// 
			// gbGetSngVal
			// 
			this.gbGetSngVal.Controls.Add(this.btnGetSngVal);
			this.gbGetSngVal.Controls.Add(this.txtSngVal);
			this.gbGetSngVal.Controls.Add(this.lbSngVal);
			this.gbGetSngVal.Controls.Add(this.lbSngID);
			this.gbGetSngVal.Controls.Add(this.nudSngID);
			this.gbGetSngVal.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbGetSngVal.Location = new System.Drawing.Point(16, 86);
			this.gbGetSngVal.Margin = new System.Windows.Forms.Padding(2);
			this.gbGetSngVal.Name = "gbGetSngVal";
			this.gbGetSngVal.Padding = new System.Windows.Forms.Padding(2);
			this.gbGetSngVal.Size = new System.Drawing.Size(237, 90);
			this.gbGetSngVal.TabIndex = 2;
			this.gbGetSngVal.TabStop = false;
			this.gbGetSngVal.Text = "Get Single Value";
			// 
			// btnGetSngVal
			// 
			this.btnGetSngVal.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnGetSngVal.Location = new System.Drawing.Point(76, 42);
			this.btnGetSngVal.Margin = new System.Windows.Forms.Padding(2);
			this.btnGetSngVal.Name = "btnGetSngVal";
			this.btnGetSngVal.Size = new System.Drawing.Size(56, 25);
			this.btnGetSngVal.TabIndex = 6;
			this.btnGetSngVal.Text = "Get";
			this.btnGetSngVal.UseVisualStyleBackColor = true;
			this.btnGetSngVal.Click += new System.EventHandler(this.btnGetSngVal_Click);
			// 
			// txtSngVal
			// 
			this.txtSngVal.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSngVal.Location = new System.Drawing.Point(143, 43);
			this.txtSngVal.Margin = new System.Windows.Forms.Padding(2);
			this.txtSngVal.Name = "txtSngVal";
			this.txtSngVal.Size = new System.Drawing.Size(76, 26);
			this.txtSngVal.TabIndex = 3;
			this.txtSngVal.Text = "0";
			this.txtSngVal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbSngVal
			// 
			this.lbSngVal.AutoSize = true;
			this.lbSngVal.Location = new System.Drawing.Point(163, 25);
			this.lbSngVal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lbSngVal.Name = "lbSngVal";
			this.lbSngVal.Size = new System.Drawing.Size(41, 16);
			this.lbSngVal.TabIndex = 5;
			this.lbSngVal.Text = "Value";
			// 
			// lbSngID
			// 
			this.lbSngID.AutoSize = true;
			this.lbSngID.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbSngID.Location = new System.Drawing.Point(32, 25);
			this.lbSngID.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lbSngID.Name = "lbSngID";
			this.lbSngID.Size = new System.Drawing.Size(22, 16);
			this.lbSngID.TabIndex = 4;
			this.lbSngID.Text = "ID";
			// 
			// nudSngID
			// 
			this.nudSngID.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nudSngID.Location = new System.Drawing.Point(19, 43);
			this.nudSngID.Margin = new System.Windows.Forms.Padding(2);
			this.nudSngID.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this.nudSngID.Name = "nudSngID";
			this.nudSngID.Size = new System.Drawing.Size(45, 26);
			this.nudSngID.TabIndex = 3;
			this.nudSngID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.nudSngID.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// gbGetAllVal
			// 
			this.gbGetAllVal.Controls.Add(this.btnGetAllVal);
			this.gbGetAllVal.Controls.Add(this.txtAllVal);
			this.gbGetAllVal.Controls.Add(this.lbAllVal);
			this.gbGetAllVal.Font = new System.Drawing.Font("Cambria", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gbGetAllVal.Location = new System.Drawing.Point(16, 180);
			this.gbGetAllVal.Margin = new System.Windows.Forms.Padding(2);
			this.gbGetAllVal.Name = "gbGetAllVal";
			this.gbGetAllVal.Padding = new System.Windows.Forms.Padding(2);
			this.gbGetAllVal.Size = new System.Drawing.Size(237, 90);
			this.gbGetAllVal.TabIndex = 3;
			this.gbGetAllVal.TabStop = false;
			this.gbGetAllVal.Text = "Get All Values";
			// 
			// btnGetAllVal
			// 
			this.btnGetAllVal.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnGetAllVal.Location = new System.Drawing.Point(19, 42);
			this.btnGetAllVal.Margin = new System.Windows.Forms.Padding(2);
			this.btnGetAllVal.Name = "btnGetAllVal";
			this.btnGetAllVal.Size = new System.Drawing.Size(56, 25);
			this.btnGetAllVal.TabIndex = 6;
			this.btnGetAllVal.Text = "Get";
			this.btnGetAllVal.UseVisualStyleBackColor = true;
			this.btnGetAllVal.Click += new System.EventHandler(this.btnGetAllVal_Click);
			// 
			// txtAllVal
			// 
			this.txtAllVal.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtAllVal.Location = new System.Drawing.Point(80, 43);
			this.txtAllVal.Margin = new System.Windows.Forms.Padding(2);
			this.txtAllVal.Name = "txtAllVal";
			this.txtAllVal.Size = new System.Drawing.Size(140, 26);
			this.txtAllVal.TabIndex = 3;
			this.txtAllVal.Text = "0";
			this.txtAllVal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// lbAllVal
			// 
			this.lbAllVal.AutoSize = true;
			this.lbAllVal.Location = new System.Drawing.Point(130, 25);
			this.lbAllVal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lbAllVal.Name = "lbAllVal";
			this.lbAllVal.Size = new System.Drawing.Size(41, 16);
			this.lbAllVal.TabIndex = 5;
			this.lbAllVal.Text = "Value";
			// 
			// gbFunc
			// 
			this.gbFunc.Controls.Add(this.btnZero);
			this.gbFunc.Font = new System.Drawing.Font("Cambria", 10.2F);
			this.gbFunc.Location = new System.Drawing.Point(258, 86);
			this.gbFunc.Margin = new System.Windows.Forms.Padding(2);
			this.gbFunc.Name = "gbFunc";
			this.gbFunc.Padding = new System.Windows.Forms.Padding(2);
			this.gbFunc.Size = new System.Drawing.Size(237, 90);
			this.gbFunc.TabIndex = 4;
			this.gbFunc.TabStop = false;
			this.gbFunc.Text = "Functions";
			// 
			// btnZero
			// 
			this.btnZero.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnZero.Location = new System.Drawing.Point(64, 37);
			this.btnZero.Margin = new System.Windows.Forms.Padding(2);
			this.btnZero.Name = "btnZero";
			this.btnZero.Size = new System.Drawing.Size(124, 30);
			this.btnZero.TabIndex = 7;
			this.btnZero.Text = "Set to zero";
			this.btnZero.UseVisualStyleBackColor = true;
			this.btnZero.Click += new System.EventHandler(this.btnZero_Click);
			// 
			// lsbxMsg
			// 
			this.lsbxMsg.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lsbxMsg.Font = new System.Drawing.Font("新細明體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.lsbxMsg.FormattingEnabled = true;
			this.lsbxMsg.ItemHeight = 14;
			this.lsbxMsg.Location = new System.Drawing.Point(0, 312);
			this.lsbxMsg.Margin = new System.Windows.Forms.Padding(2);
			this.lsbxMsg.Name = "lsbxMsg";
			this.lsbxMsg.Size = new System.Drawing.Size(523, 130);
			this.lsbxMsg.TabIndex = 5;
			// 
			// Keyence_DL_RS1A
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(523, 442);
			this.Controls.Add(this.lsbxMsg);
			this.Controls.Add(this.gbFunc);
			this.Controls.Add(this.gbGetAllVal);
			this.Controls.Add(this.gbGetSngVal);
			this.Controls.Add(this.picCntStt);
			this.Controls.Add(this.btnConnect);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(2);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Keyence_DL_RS1A";
			this.Text = "Keyence DL-RS1A";
			((System.ComponentModel.ISupportInitialize)(this.picCntStt)).EndInit();
			this.gbGetSngVal.ResumeLayout(false);
			this.gbGetSngVal.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudSngID)).EndInit();
			this.gbGetAllVal.ResumeLayout(false);
			this.gbGetAllVal.PerformLayout();
			this.gbFunc.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.PictureBox picCntStt;
        private System.Windows.Forms.GroupBox gbGetSngVal;
        private System.Windows.Forms.TextBox txtSngVal;
        private System.Windows.Forms.Label lbSngVal;
        private System.Windows.Forms.Label lbSngID;
        private System.Windows.Forms.NumericUpDown nudSngID;
        private System.Windows.Forms.Button btnGetSngVal;
        private System.Windows.Forms.GroupBox gbGetAllVal;
        private System.Windows.Forms.Button btnGetAllVal;
        private System.Windows.Forms.TextBox txtAllVal;
        private System.Windows.Forms.Label lbAllVal;
        private System.Windows.Forms.GroupBox gbFunc;
        private System.Windows.Forms.Button btnZero;
        private System.Windows.Forms.ListBox lsbxMsg;
    }
}