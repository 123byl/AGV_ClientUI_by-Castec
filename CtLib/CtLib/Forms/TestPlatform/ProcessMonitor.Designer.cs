namespace CtLib.Forms.TestPlatform {
	/// <summary>一個簡單的視窗供監看應用程式之輸出資料</summary>
	partial class ProcessMonitor {
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
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.clearAllMessageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.ContextMenuStrip = this.contextMenuStrip;
			this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 19;
			this.listBox1.Location = new System.Drawing.Point(0, 0);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(642, 550);
			this.listBox1.TabIndex = 0;
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearAllMessageToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(175, 48);
			// 
			// clearAllMessageToolStripMenuItem
			// 
			this.clearAllMessageToolStripMenuItem.Name = "clearAllMessageToolStripMenuItem";
			this.clearAllMessageToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.clearAllMessageToolStripMenuItem.Text = "Clear All Message";
			this.clearAllMessageToolStripMenuItem.Click += new System.EventHandler(this.clearAllMessageToolStripMenuItem_Click);
			// 
			// ProcessMonitor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(642, 550);
			this.Controls.Add(this.listBox1);
			this.Name = "ProcessMonitor";
			this.Text = "ProcessMonitor";
			this.Shown += new System.EventHandler(this.ProcessMonitor_Shown);
			this.contextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem clearAllMessageToolStripMenuItem;
	}
}