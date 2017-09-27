namespace ClientUI.Development
{
    partial class AGVMapUI
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
            this.mapUI = new AGVMap.MapUI();
            ((System.ComponentModel.ISupportInitialize)(this.mapUI)).BeginInit();
            this.SuspendLayout();
            // 
            // mapUI
            // 
            this.mapUI.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapUI.DrawFPS = true;
            this.mapUI.Location = new System.Drawing.Point(14, 13);
            this.mapUI.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.mapUI.Name = "mapUI";
            this.mapUI.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            this.mapUI.RenderContextType = SharpGL.RenderContextType.DIBSection;
            this.mapUI.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            this.mapUI.Size = new System.Drawing.Size(254, 227);
            this.mapUI.TabIndex = 0;
            // 
            // AGVMapUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.mapUI);
            this.Name = "AGVMapUI";
            this.Text = "AGVMapUI";
            ((System.ComponentModel.ISupportInitialize)(this.mapUI)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AGVMap.MapUI mapUI;
    }
}