namespace ClientUI.Component {
    partial class CtToolBox {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtToolBox));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.outlookBar2 = new CtOutLookBar.Public.OutlookBar();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Goal.bmp");
            this.imageList1.Images.SetKeyName(1, "Power.bmp");
            this.imageList1.Images.SetKeyName(2, "AGV.bmp");
            this.imageList1.Images.SetKeyName(3, "Parking.bmp");
            this.imageList1.Images.SetKeyName(4, "NarrowLine.bmp");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.outlookBar2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(167, 664);
            this.panel1.TabIndex = 0;
            // 
            // outlookBar2
            // 
            this.outlookBar2.BackColor = System.Drawing.Color.Black;
            this.outlookBar2.ButtonHeight = 25;
            this.outlookBar2.Location = new System.Drawing.Point(0, 0);
            this.outlookBar2.Name = "outlookBar2";
            this.outlookBar2.SelectedBand = 0;
            this.outlookBar2.Size = new System.Drawing.Size(207, 664);
            this.outlookBar2.TabIndex = 1;
            // 
            // CtToolBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(167, 664);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CtToolBox";
            this.Text = "CtToolBox";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel panel1;
        private CtOutLookBar.Public.OutlookBar outlookBar2;
    }
}