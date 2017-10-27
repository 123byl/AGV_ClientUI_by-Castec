using GLUI;

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
            this.uiControl = new UIControl();
            ((System.ComponentModel.ISupportInitialize)(this.uiControl)).BeginInit();
            this.SuspendLayout();
            // 
            // uiControl
            // 
            this.uiControl.AllowEdit = true;
            this.uiControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uiControl.DrawFPS = false;
            this.uiControl.Location = new System.Drawing.Point(13, 12);
            this.uiControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.uiControl.Name = "uiControl";
            this.uiControl.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            this.uiControl.RenderContextType = SharpGL.RenderContextType.DIBSection;
            this.uiControl.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            this.uiControl.ShowAxis = true;
            this.uiControl.ShowFPS = false;
            this.uiControl.ShowGrid = true;
            this.uiControl.ShowNames = true;
            this.uiControl.Size = new System.Drawing.Size(256, 229);
            this.uiControl.TabIndex = 0;
            // 
            // AGVMapUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.uiControl);
            this.Name = "AGVMapUI";
            this.Text = "AGVMapUI";
            ((System.ComponentModel.ISupportInitialize)(this.uiControl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UIControl uiControl;
    }
}