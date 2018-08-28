namespace CtExtendLib
{
	partial class LoadingForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pgbLoad = new System.Windows.Forms.ProgressBar();
			this.lblLoad = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// pgbLoad
			// 
			this.pgbLoad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pgbLoad.Location = new System.Drawing.Point(13, 81);
			this.pgbLoad.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.pgbLoad.Name = "pgbLoad";
			this.pgbLoad.Size = new System.Drawing.Size(276, 38);
			this.pgbLoad.TabIndex = 0;
			this.pgbLoad.Value = 100;
			// 
			// lblLoad
			// 
			this.lblLoad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblLoad.Location = new System.Drawing.Point(13, 25);
			this.lblLoad.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblLoad.Name = "lblLoad";
			this.lblLoad.Size = new System.Drawing.Size(276, 25);
			this.lblLoad.TabIndex = 1;
			this.lblLoad.Text = "讀取剩餘時間";
			this.lblLoad.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LoadingForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(302, 133);
			this.ControlBox = false;
			this.Controls.Add(this.lblLoad);
			this.Controls.Add(this.pgbLoad);
			this.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(320, 180);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(320, 180);
			this.Name = "LoadingForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "LoadingForm";
			this.TopMost = true;
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ProgressBar pgbLoad;
		private System.Windows.Forms.Label lblLoad;
	}
}