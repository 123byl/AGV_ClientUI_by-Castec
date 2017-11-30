namespace ClientUI.Component {
    partial class CtMotionController {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtMotionController));
            this.btnSetVelo = new System.Windows.Forms.Button();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.lbVelocity = new System.Windows.Forms.Label();
            this.txtVelocity = new System.Windows.Forms.TextBox();
            this.btnServoOnOff = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSetVelo
            // 
            this.btnSetVelo.Enabled = false;
            this.btnSetVelo.Location = new System.Drawing.Point(270, 17);
            this.btnSetVelo.Margin = new System.Windows.Forms.Padding(4);
            this.btnSetVelo.Name = "btnSetVelo";
            this.btnSetVelo.Size = new System.Drawing.Size(67, 35);
            this.btnSetVelo.TabIndex = 20;
            this.btnSetVelo.Text = "Set";
            this.btnSetVelo.UseVisualStyleBackColor = true;
            // 
            // btnStartStop
            // 
            this.btnStartStop.BackColor = System.Drawing.SystemColors.Control;
            this.btnStartStop.Enabled = false;
            this.btnStartStop.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStartStop.Image")));
            this.btnStartStop.Location = new System.Drawing.Point(257, 75);
            this.btnStartStop.Margin = new System.Windows.Forms.Padding(4);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(67, 62);
            this.btnStartStop.TabIndex = 19;
            this.btnStartStop.Tag = "Stop";
            this.btnStartStop.UseVisualStyleBackColor = false;
            // 
            // lbVelocity
            // 
            this.lbVelocity.AutoSize = true;
            this.lbVelocity.BackColor = System.Drawing.Color.Transparent;
            this.lbVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVelocity.Location = new System.Drawing.Point(18, 23);
            this.lbVelocity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbVelocity.Name = "lbVelocity";
            this.lbVelocity.Size = new System.Drawing.Size(169, 22);
            this.lbVelocity.TabIndex = 13;
            this.lbVelocity.Text = "Movement Velocity:";
            // 
            // txtVelocity
            // 
            this.txtVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVelocity.Location = new System.Drawing.Point(195, 18);
            this.txtVelocity.Margin = new System.Windows.Forms.Padding(4);
            this.txtVelocity.Name = "txtVelocity";
            this.txtVelocity.Size = new System.Drawing.Size(67, 30);
            this.txtVelocity.TabIndex = 12;
            this.txtVelocity.Text = "500";
            this.txtVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnServoOnOff
            // 
            this.btnServoOnOff.BackColor = System.Drawing.Color.Red;
            this.btnServoOnOff.Enabled = false;
            this.btnServoOnOff.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnServoOnOff.ForeColor = System.Drawing.Color.White;
            this.btnServoOnOff.Location = new System.Drawing.Point(35, 75);
            this.btnServoOnOff.Margin = new System.Windows.Forms.Padding(4);
            this.btnServoOnOff.Name = "btnServoOnOff";
            this.btnServoOnOff.Size = new System.Drawing.Size(67, 62);
            this.btnServoOnOff.TabIndex = 14;
            this.btnServoOnOff.Text = "OFF";
            this.btnServoOnOff.UseVisualStyleBackColor = false;
            // 
            // btnUp
            // 
            this.btnUp.Enabled = false;
            this.btnUp.Font = new System.Drawing.Font("新細明體", 20F);
            this.btnUp.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
            this.btnUp.Location = new System.Drawing.Point(146, 75);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(67, 62);
            this.btnUp.TabIndex = 15;
            this.btnUp.Tag = "38";
            this.btnUp.UseVisualStyleBackColor = true;
            // 
            // btnRight
            // 
            this.btnRight.Enabled = false;
            this.btnRight.Font = new System.Drawing.Font("新細明體", 20F);
            this.btnRight.Image = ((System.Drawing.Image)(resources.GetObject("btnRight.Image")));
            this.btnRight.Location = new System.Drawing.Point(257, 154);
            this.btnRight.Margin = new System.Windows.Forms.Padding(4);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(67, 62);
            this.btnRight.TabIndex = 16;
            this.btnRight.Tag = "39";
            this.btnRight.UseVisualStyleBackColor = true;
            // 
            // btnLeft
            // 
            this.btnLeft.Enabled = false;
            this.btnLeft.Font = new System.Drawing.Font("新細明體", 20F);
            this.btnLeft.Image = ((System.Drawing.Image)(resources.GetObject("btnLeft.Image")));
            this.btnLeft.Location = new System.Drawing.Point(35, 154);
            this.btnLeft.Margin = new System.Windows.Forms.Padding(4);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(67, 62);
            this.btnLeft.TabIndex = 17;
            this.btnLeft.Tag = "37";
            this.btnLeft.UseVisualStyleBackColor = true;
            // 
            // btnDown
            // 
            this.btnDown.Enabled = false;
            this.btnDown.Font = new System.Drawing.Font("新細明體", 20F);
            this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
            this.btnDown.Location = new System.Drawing.Point(146, 154);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(67, 62);
            this.btnDown.TabIndex = 18;
            this.btnDown.Tag = "40";
            this.btnDown.UseVisualStyleBackColor = true;
            // 
            // CtMotionController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 235);
            this.Controls.Add(this.btnSetVelo);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.lbVelocity);
            this.Controls.Add(this.txtVelocity);
            this.Controls.Add(this.btnServoOnOff);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnDown);
            this.Name = "CtMotionController";
            this.Text = "CtMotionController";
            this.Click += new System.EventHandler(this.CtMotionController_Click);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CtMotionController_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSetVelo;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Label lbVelocity;
        private System.Windows.Forms.TextBox txtVelocity;
        private System.Windows.Forms.Button btnServoOnOff;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnDown;
    }
}