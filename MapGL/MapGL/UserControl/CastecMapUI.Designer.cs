namespace MapGL
{
    partial class CastecMapUI
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MapPanel = new System.Windows.Forms.Panel();
            this.MapDisplayer = new MapGL.OpenGLControl();
            this.MapPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapDisplayer)).BeginInit();
            this.SuspendLayout();
            // 
            // MapPanel
            // 
            this.MapPanel.AutoSize = true;
            this.MapPanel.Controls.Add(this.MapDisplayer);
            this.MapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MapPanel.Location = new System.Drawing.Point(0, 0);
            this.MapPanel.Margin = new System.Windows.Forms.Padding(0);
            this.MapPanel.Name = "MapPanel";
            this.MapPanel.Size = new System.Drawing.Size(200, 200);
            this.MapPanel.TabIndex = 0;
            // 
            // MapDisplayer
            // 
            this.MapDisplayer.AutoSize = true;
            this.MapDisplayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MapDisplayer.DrawFPS = false;
            this.MapDisplayer.Location = new System.Drawing.Point(0, 0);
            this.MapDisplayer.Margin = new System.Windows.Forms.Padding(0);
            this.MapDisplayer.Name = "MapDisplayer";
            this.MapDisplayer.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            this.MapDisplayer.RenderContextType = SharpGL.RenderContextType.DIBSection;
            this.MapDisplayer.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            this.MapDisplayer.Size = new System.Drawing.Size(200, 200);
            this.MapDisplayer.TabIndex = 0;
            this.MapDisplayer.GDIDraw += new SharpGL.RenderEventHandler(this.MapDisplayer_GDIDraw);
            this.MapDisplayer.Load += new System.EventHandler(this.MapDisplayer_Resize);
            this.MapDisplayer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MapDisplayer_MouseClick);
            this.MapDisplayer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MapDisplayer_MouseDown);
            this.MapDisplayer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MapDisplayer_MouseMove);
            this.MapDisplayer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MapDisplayer_MouseUp);
            this.MapDisplayer.Resize += new System.EventHandler(this.MapDisplayer_Resize);
            // 
            // CastecMapUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.MapPanel);
            this.Name = "CastecMapUI";
            this.Size = new System.Drawing.Size(200, 200);
            this.Load += new System.EventHandler(this.CastecMapUI_Load);
            this.MapPanel.ResumeLayout(false);
            this.MapPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapDisplayer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel MapPanel;
        private OpenGLControl MapDisplayer;
    }
}
