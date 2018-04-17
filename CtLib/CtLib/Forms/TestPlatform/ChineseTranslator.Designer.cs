namespace CtLib.Forms.TestPlatform {
	partial class ChineseTranslator {
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
			this.txtOri = new System.Windows.Forms.TextBox();
			this.txtDest = new System.Windows.Forms.TextBox();
			this.rdbtnCNTW = new System.Windows.Forms.RadioButton();
			this.rdbtnTWCN = new System.Windows.Forms.RadioButton();
			this.btnTrans = new System.Windows.Forms.Button();
			this.lbOri = new System.Windows.Forms.Label();
			this.lbDest = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtOri
			// 
			this.txtOri.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.txtOri.ImeMode = System.Windows.Forms.ImeMode.On;
			this.txtOri.Location = new System.Drawing.Point(46, 32);
			this.txtOri.Multiline = true;
			this.txtOri.Name = "txtOri";
			this.txtOri.Size = new System.Drawing.Size(347, 123);
			this.txtOri.TabIndex = 0;
			// 
			// txtDest
			// 
			this.txtDest.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.txtDest.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.txtDest.Location = new System.Drawing.Point(46, 202);
			this.txtDest.Multiline = true;
			this.txtDest.Name = "txtDest";
			this.txtDest.ReadOnly = true;
			this.txtDest.Size = new System.Drawing.Size(347, 123);
			this.txtDest.TabIndex = 1;
			// 
			// rdbtnCNTW
			// 
			this.rdbtnCNTW.AutoSize = true;
			this.rdbtnCNTW.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.rdbtnCNTW.Location = new System.Drawing.Point(445, 140);
			this.rdbtnCNTW.Name = "rdbtnCNTW";
			this.rdbtnCNTW.Size = new System.Drawing.Size(115, 24);
			this.rdbtnCNTW.TabIndex = 2;
			this.rdbtnCNTW.TabStop = true;
			this.rdbtnCNTW.Text = "簡體 轉 繁體";
			this.rdbtnCNTW.UseVisualStyleBackColor = true;
			this.rdbtnCNTW.Click += new System.EventHandler(this.rdbtnCNTW_Click);
			// 
			// rdbtnTWCN
			// 
			this.rdbtnTWCN.AutoSize = true;
			this.rdbtnTWCN.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.rdbtnTWCN.Location = new System.Drawing.Point(445, 110);
			this.rdbtnTWCN.Name = "rdbtnTWCN";
			this.rdbtnTWCN.Size = new System.Drawing.Size(115, 24);
			this.rdbtnTWCN.TabIndex = 3;
			this.rdbtnTWCN.TabStop = true;
			this.rdbtnTWCN.Text = "繁體 轉 簡體";
			this.rdbtnTWCN.UseVisualStyleBackColor = true;
			this.rdbtnTWCN.Click += new System.EventHandler(this.rdbtnTWCN_Click);
			// 
			// btnTrans
			// 
			this.btnTrans.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnTrans.Location = new System.Drawing.Point(446, 187);
			this.btnTrans.Name = "btnTrans";
			this.btnTrans.Size = new System.Drawing.Size(114, 45);
			this.btnTrans.TabIndex = 4;
			this.btnTrans.Text = "翻譯";
			this.btnTrans.UseVisualStyleBackColor = true;
			this.btnTrans.Click += new System.EventHandler(this.btnTrans_Click);
			// 
			// lbOri
			// 
			this.lbOri.AutoSize = true;
			this.lbOri.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.lbOri.Location = new System.Drawing.Point(42, 9);
			this.lbOri.Name = "lbOri";
			this.lbOri.Size = new System.Drawing.Size(105, 20);
			this.lbOri.TabIndex = 5;
			this.lbOri.Text = "請輸入原文：";
			// 
			// lbDest
			// 
			this.lbDest.AutoSize = true;
			this.lbDest.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.lbDest.Location = new System.Drawing.Point(42, 179);
			this.lbDest.Name = "lbDest";
			this.lbDest.Size = new System.Drawing.Size(105, 20);
			this.lbDest.TabIndex = 6;
			this.lbDest.Text = "轉換後文字：";
			// 
			// ChineseTranslator
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(617, 377);
			this.Controls.Add(this.lbDest);
			this.Controls.Add(this.lbOri);
			this.Controls.Add(this.btnTrans);
			this.Controls.Add(this.rdbtnTWCN);
			this.Controls.Add(this.rdbtnCNTW);
			this.Controls.Add(this.txtDest);
			this.Controls.Add(this.txtOri);
			this.Name = "ChineseTranslator";
			this.Text = "繁簡轉換器";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtOri;
		private System.Windows.Forms.TextBox txtDest;
		private System.Windows.Forms.RadioButton rdbtnCNTW;
		private System.Windows.Forms.RadioButton rdbtnTWCN;
		private System.Windows.Forms.Button btnTrans;
		private System.Windows.Forms.Label lbOri;
		private System.Windows.Forms.Label lbDest;
	}
}