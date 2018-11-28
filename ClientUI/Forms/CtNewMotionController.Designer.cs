namespace VehiclePlanner.Forms
{
	partial class CtNewMotionController
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
			this.gpbVelocity = new System.Windows.Forms.GroupBox();
			this.txtVelocity = new System.Windows.Forms.TextBox();
			this.btnServo = new System.Windows.Forms.Button();
			this.btnForward = new System.Windows.Forms.Button();
			this.btnBack = new System.Windows.Forms.Button();
			this.btnCW = new System.Windows.Forms.Button();
			this.btnCCW = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.gpbVelocity.SuspendLayout();
			this.SuspendLayout();
			// 
			// gpbVelocity
			// 
			this.gpbVelocity.Controls.Add(this.txtVelocity);
			this.gpbVelocity.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.gpbVelocity.Location = new System.Drawing.Point(22, 13);
			this.gpbVelocity.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.gpbVelocity.Name = "gpbVelocity";
			this.gpbVelocity.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.gpbVelocity.Size = new System.Drawing.Size(215, 70);
			this.gpbVelocity.TabIndex = 4;
			this.gpbVelocity.TabStop = false;
			this.gpbVelocity.Text = "Velocity";
			// 
			// txtVelocity
			// 
			this.txtVelocity.Location = new System.Drawing.Point(6, 28);
			this.txtVelocity.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtVelocity.Name = "txtVelocity";
			this.txtVelocity.ReadOnly = true;
			this.txtVelocity.Size = new System.Drawing.Size(201, 25);
			this.txtVelocity.TabIndex = 6;
			// 
			// btnServo
			// 
			this.btnServo.BackColor = System.Drawing.Color.Red;
			this.btnServo.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnServo.Location = new System.Drawing.Point(250, 13);
			this.btnServo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnServo.Name = "btnServo";
			this.btnServo.Size = new System.Drawing.Size(70, 70);
			this.btnServo.TabIndex = 5;
			this.btnServo.Text = "Servo Off";
			this.btnServo.UseVisualStyleBackColor = false;
			this.btnServo.Click += new System.EventHandler(this.btnServo_Click);
			// 
			// btnForward
			// 
			this.btnForward.BackgroundImage = global::VehiclePlanner.Properties.Resources.Up;
			this.btnForward.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnForward.Location = new System.Drawing.Point(120, 90);
			this.btnForward.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnForward.Name = "btnForward";
			this.btnForward.Size = new System.Drawing.Size(75, 75);
			this.btnForward.TabIndex = 0;
			this.btnForward.UseVisualStyleBackColor = true;
			// 
			// btnBack
			// 
			this.btnBack.BackgroundImage = global::VehiclePlanner.Properties.Resources.Down;
			this.btnBack.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnBack.Location = new System.Drawing.Point(120, 173);
			this.btnBack.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnBack.Name = "btnBack";
			this.btnBack.Size = new System.Drawing.Size(75, 75);
			this.btnBack.TabIndex = 1;
			this.btnBack.UseVisualStyleBackColor = true;
			// 
			// btnCW
			// 
			this.btnCW.BackgroundImage = global::VehiclePlanner.Properties.Resources.CW;
			this.btnCW.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnCW.Location = new System.Drawing.Point(201, 173);
			this.btnCW.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnCW.Name = "btnCW";
			this.btnCW.Size = new System.Drawing.Size(75, 75);
			this.btnCW.TabIndex = 2;
			this.btnCW.UseVisualStyleBackColor = true;
			// 
			// btnCCW
			// 
			this.btnCCW.BackgroundImage = global::VehiclePlanner.Properties.Resources.CCW;
			this.btnCCW.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnCCW.Location = new System.Drawing.Point(39, 173);
			this.btnCCW.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnCCW.Name = "btnCCW";
			this.btnCCW.Size = new System.Drawing.Size(75, 75);
			this.btnCCW.TabIndex = 3;
			this.btnCCW.UseVisualStyleBackColor = true;
			// 
			// btnStop
			// 
			this.btnStop.BackColor = System.Drawing.Color.Red;
			this.btnStop.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnStop.Location = new System.Drawing.Point(250, 90);
			this.btnStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(70, 70);
			this.btnStop.TabIndex = 4;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = false;
			this.btnStop.Click += new System.EventHandler(this.button1_Click);
			// 
			// CtNewMotionController
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(334, 271);
			this.Controls.Add(this.btnStop);
			this.Controls.Add(this.btnCCW);
			this.Controls.Add(this.btnCW);
			this.Controls.Add(this.btnBack);
			this.Controls.Add(this.btnForward);
			this.Controls.Add(this.btnServo);
			this.Controls.Add(this.gpbVelocity);
			this.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(350, 310);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(350, 310);
			this.Name = "CtNewMotionController";
			this.ShowIcon = false;
			this.Text = "Motion Controller";
			this.TopMost = true;
			this.gpbVelocity.ResumeLayout(false);
			this.gpbVelocity.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.GroupBox gpbVelocity;
		private System.Windows.Forms.TextBox txtVelocity;
		private System.Windows.Forms.Button btnServo;
		private System.Windows.Forms.Button btnForward;
		private System.Windows.Forms.Button btnBack;
		private System.Windows.Forms.Button btnCW;
		private System.Windows.Forms.Button btnCCW;
		private System.Windows.Forms.Button btnStop;
	}
}