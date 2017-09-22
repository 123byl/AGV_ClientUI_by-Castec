namespace CtLib.Forms.TestPlatform {
    partial class XML_Editor {
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.檔案ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.開啟舊檔ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iOXMLFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.開新檔案ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iODataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView = new System.Windows.Forms.TreeView();
            this.button1 = new System.Windows.Forms.Button();
            this.ctxmAdd = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.adeptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.beckhoffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wagoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.menuStrip.SuspendLayout();
            this.ctxmAdd.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.檔案ToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(473, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // 檔案ToolStripMenuItem
            // 
            this.檔案ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.開啟舊檔ToolStripMenuItem,
            this.開新檔案ToolStripMenuItem});
            this.檔案ToolStripMenuItem.Name = "檔案ToolStripMenuItem";
            this.檔案ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.檔案ToolStripMenuItem.Text = "檔案";
            // 
            // 開啟舊檔ToolStripMenuItem
            // 
            this.開啟舊檔ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iOXMLFileToolStripMenuItem});
            this.開啟舊檔ToolStripMenuItem.Name = "開啟舊檔ToolStripMenuItem";
            this.開啟舊檔ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.開啟舊檔ToolStripMenuItem.Text = "開啟舊檔";
            // 
            // iOXMLFileToolStripMenuItem
            // 
            this.iOXMLFileToolStripMenuItem.Name = "iOXMLFileToolStripMenuItem";
            this.iOXMLFileToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.iOXMLFileToolStripMenuItem.Text = "IO XML File";
            this.iOXMLFileToolStripMenuItem.Click += new System.EventHandler(this.iOXMLFileToolStripMenuItem_Click);
            // 
            // 開新檔案ToolStripMenuItem
            // 
            this.開新檔案ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iODataToolStripMenuItem});
            this.開新檔案ToolStripMenuItem.Name = "開新檔案ToolStripMenuItem";
            this.開新檔案ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.開新檔案ToolStripMenuItem.Text = "開新檔案";
            // 
            // iODataToolStripMenuItem
            // 
            this.iODataToolStripMenuItem.Name = "iODataToolStripMenuItem";
            this.iODataToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.iODataToolStripMenuItem.Text = "IO XML File";
            this.iODataToolStripMenuItem.Click += new System.EventHandler(this.iODataToolStripMenuItem_Click);
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView.Location = new System.Drawing.Point(0, 24);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(345, 522);
            this.treeView.TabIndex = 1;
            this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseDoubleClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(361, 41);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 45);
            this.button1.TabIndex = 2;
            this.button1.Text = "Add";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ctxmAdd
            // 
            this.ctxmAdd.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.adeptToolStripMenuItem,
            this.beckhoffToolStripMenuItem,
            this.wagoToolStripMenuItem,
            this.toolStripSeparator1});
            this.ctxmAdd.Name = "ctxmAdd";
            this.ctxmAdd.Size = new System.Drawing.Size(124, 76);
            // 
            // adeptToolStripMenuItem
            // 
            this.adeptToolStripMenuItem.Name = "adeptToolStripMenuItem";
            this.adeptToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.adeptToolStripMenuItem.Text = "Adept";
            this.adeptToolStripMenuItem.Click += new System.EventHandler(this.adeptToolStripMenuItem_Click);
            // 
            // beckhoffToolStripMenuItem
            // 
            this.beckhoffToolStripMenuItem.Name = "beckhoffToolStripMenuItem";
            this.beckhoffToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.beckhoffToolStripMenuItem.Text = "Beckhoff";
            this.beckhoffToolStripMenuItem.Click += new System.EventHandler(this.beckhoffToolStripMenuItem_Click);
            // 
            // wagoToolStripMenuItem
            // 
            this.wagoToolStripMenuItem.Name = "wagoToolStripMenuItem";
            this.wagoToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.wagoToolStripMenuItem.Text = "Wago";
            this.wagoToolStripMenuItem.Click += new System.EventHandler(this.wagoToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(120, 6);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(361, 108);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(84, 45);
            this.button2.TabIndex = 4;
            this.button2.Text = "Delete";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(361, 475);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(84, 45);
            this.button3.TabIndex = 5;
            this.button3.Text = "Save";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(361, 172);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(84, 45);
            this.button4.TabIndex = 6;
            this.button4.Text = "Copy";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(361, 414);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(84, 45);
            this.button5.TabIndex = 7;
            this.button5.Text = "Dump Enum";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(361, 392);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(108, 16);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "Auto Enum Value";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // XML_Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 546);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "XML_Editor";
            this.Text = "XML Editor";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ctxmAdd.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem 檔案ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 開啟舊檔ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 開新檔案ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iODataToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip ctxmAdd;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ToolStripMenuItem adeptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem beckhoffToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wagoToolStripMenuItem;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ToolStripMenuItem iOXMLFileToolStripMenuItem;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}