using CtBind;

namespace VehiclePlanner.Forms {
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnServoOnOff = new System.Windows.Forms.Button();
            this.picRightTurn = new System.Windows.Forms.PictureBox();
            this.picLeftTurn = new System.Windows.Forms.PictureBox();
            this.picBack = new System.Windows.Forms.PictureBox();
            this.picForward = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tslVelocity = new System.Windows.Forms.ToolStripLabel();
            this.tstVelocity = new CtBind.Bindable.ToolStripTextBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRightTurn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLeftTurn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picForward)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnServoOnOff);
            this.panel1.Controls.Add(this.picRightTurn);
            this.panel1.Controls.Add(this.picLeftTurn);
            this.panel1.Controls.Add(this.picBack);
            this.panel1.Controls.Add(this.picForward);
            this.panel1.Location = new System.Drawing.Point(3, 38);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 124);
            this.panel1.TabIndex = 6;
            // 
            // btnServoOnOff
            // 
            this.btnServoOnOff.BackColor = System.Drawing.Color.Red;
            this.btnServoOnOff.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnServoOnOff.ForeColor = System.Drawing.Color.White;
            this.btnServoOnOff.Location = new System.Drawing.Point(19, 7);
            this.btnServoOnOff.Margin = new System.Windows.Forms.Padding(4);
            this.btnServoOnOff.Name = "btnServoOnOff";
            this.btnServoOnOff.Size = new System.Drawing.Size(50, 50);
            this.btnServoOnOff.TabIndex = 10;
            this.btnServoOnOff.Text = "OFF";
            this.btnServoOnOff.UseVisualStyleBackColor = false;
            this.btnServoOnOff.Click += new System.EventHandler(this.btnServoOnOff_Click);
            // 
            // picRightTurn
            // 
            this.picRightTurn.Image = global::VehiclePlanner.Properties.Resources.Arrow_RotationR;
            this.picRightTurn.Location = new System.Drawing.Point(131, 64);
            this.picRightTurn.Name = "picRightTurn";
            this.picRightTurn.Size = new System.Drawing.Size(50, 50);
            this.picRightTurn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picRightTurn.TabIndex = 9;
            this.picRightTurn.TabStop = false;
            this.picRightTurn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Motion_MouseDown);
            this.picRightTurn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Motion_MouseUp);
            // 
            // picLeftTurn
            // 
            this.picLeftTurn.Image = global::VehiclePlanner.Properties.Resources.Arrow_RotationL;
            this.picLeftTurn.Location = new System.Drawing.Point(19, 64);
            this.picLeftTurn.Name = "picLeftTurn";
            this.picLeftTurn.Size = new System.Drawing.Size(50, 50);
            this.picLeftTurn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLeftTurn.TabIndex = 8;
            this.picLeftTurn.TabStop = false;
            this.picLeftTurn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Motion_MouseDown);
            this.picLeftTurn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Motion_MouseUp);
            // 
            // picBack
            // 
            this.picBack.Image = global::VehiclePlanner.Properties.Resources.Arrow_Down;
            this.picBack.Location = new System.Drawing.Point(75, 64);
            this.picBack.Name = "picBack";
            this.picBack.Size = new System.Drawing.Size(50, 50);
            this.picBack.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBack.TabIndex = 7;
            this.picBack.TabStop = false;
            this.picBack.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Motion_MouseDown);
            this.picBack.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Motion_MouseUp);
            // 
            // picForward
            // 
            this.picForward.Image = global::VehiclePlanner.Properties.Resources.Arrow_Up;
            this.picForward.Location = new System.Drawing.Point(75, 8);
            this.picForward.Name = "picForward";
            this.picForward.Size = new System.Drawing.Size(50, 50);
            this.picForward.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picForward.TabIndex = 6;
            this.picForward.TabStop = false;
            this.picForward.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Motion_MouseDown);
            this.picForward.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Motion_MouseUp);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslVelocity,
            this.tstVelocity});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(209, 27);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tslVelocity
            // 
            this.tslVelocity.Name = "tslVelocity";
            this.tslVelocity.Size = new System.Drawing.Size(64, 24);
            this.tslVelocity.Text = "Velocity";
            // 
            // tstVelocity
            // 
            this.tstVelocity.Name = "tstVelocity";
            this.tstVelocity.Size = new System.Drawing.Size(80, 27);
            this.tstVelocity.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tstVelocity_KeyPress);
            this.tstVelocity.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tstVelocity_KeyUp);
            // 
            // CtMotionController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(209, 163);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CtMotionController";
            this.Text = "Motion Controller";
            this.TopMost = true;
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picRightTurn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLeftTurn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picForward)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox picRightTurn;
        private System.Windows.Forms.PictureBox picLeftTurn;
        private System.Windows.Forms.PictureBox picBack;
        private System.Windows.Forms.PictureBox picForward;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel tslVelocity;
        private Bindable.ToolStripTextBox tstVelocity;
        private System.Windows.Forms.Button btnServoOnOff;
    }
}