namespace VehiclePlanner.Component {
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
            this.picRightTurn = new System.Windows.Forms.PictureBox();
            this.picLeftTurn = new System.Windows.Forms.PictureBox();
            this.picBack = new System.Windows.Forms.PictureBox();
            this.picForward = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRightTurn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLeftTurn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picForward)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.picRightTurn);
            this.panel1.Controls.Add(this.picLeftTurn);
            this.panel1.Controls.Add(this.picBack);
            this.panel1.Controls.Add(this.picForward);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 124);
            this.panel1.TabIndex = 6;
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
            // CtMotionController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(225, 143);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CtMotionController";
            this.Text = "Motion Controller";
            this.TopMost = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CtMotionController_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CtCotionController_KeyUp);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picRightTurn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLeftTurn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picForward)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox picRightTurn;
        private System.Windows.Forms.PictureBox picLeftTurn;
        private System.Windows.Forms.PictureBox picBack;
        private System.Windows.Forms.PictureBox picForward;
    }
}