using CtBind;
using System;

namespace INITesting {
    partial class CtrlParamEditor {
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtrlParamEditor));
			this.dgvProperties = new System.Windows.Forms.DataGridView();
			this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colDefault = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.cmsDGV = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miAdd = new CtBind.Bindable.ToolStripMenuItem();
			this.miDelete = new CtBind.Bindable.ToolStripMenuItem();
			this.miEdit = new CtBind.Bindable.ToolStripMenuItem();
			this.btnWrite = new System.Windows.Forms.Button();
			this.btnRead = new System.Windows.Forms.Button();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.tslbPath = new CtBind.Bindable.ToolStripStatusLabel();
			this.toolStripStatusLabel2 = new CtBind.Bindable.ToolStripStatusLabel();
			this.tslbCount = new CtBind.Bindable.ToolStripStatusLabel();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.tstKeyWord = new CtBind.Bindable.ToolStripTextBox();
			this.tsbOpen = new CtBind.Bindable.ToolStripButton();
			this.tsbSave = new CtBind.Bindable.ToolStripButton();
			this.tsbDownload = new CtBind.Bindable.ToolStripButton();
			this.tsbUndo = new CtBind.Bindable.ToolStripButton();
			this.tsbRedo = new CtBind.Bindable.ToolStripButton();
			this.tsbFilter = new CtBind.Bindable.ToolStripButton();
			this.tsbHighlight = new CtBind.Bindable.ToolStripButton();
			this.tsbUpload = new CtBind.Bindable.ToolStripButton();
			((System.ComponentModel.ISupportInitialize)(this.dgvProperties)).BeginInit();
			this.cmsDGV.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// dgvProperties
			// 
			this.dgvProperties.AllowUserToAddRows = false;
			this.dgvProperties.AllowUserToOrderColumns = true;
			this.dgvProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle5.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvProperties.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
			this.dgvProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvProperties.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colType,
            this.colValue,
            this.colDescription,
            this.colMax,
            this.colMin,
            this.colDefault});
			this.dgvProperties.Location = new System.Drawing.Point(0, 46);
			this.dgvProperties.Margin = new System.Windows.Forms.Padding(1);
			this.dgvProperties.Name = "dgvProperties";
			dataGridViewCellStyle6.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.dgvProperties.RowsDefaultCellStyle = dataGridViewCellStyle6;
			this.dgvProperties.RowTemplate.Height = 27;
			this.dgvProperties.Size = new System.Drawing.Size(1238, 658);
			this.dgvProperties.TabIndex = 0;
			// 
			// colName
			// 
			this.colName.HeaderText = "Name";
			this.colName.Name = "colName";
			this.colName.ReadOnly = true;
			this.colName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.colName.Width = 250;
			// 
			// colType
			// 
			this.colType.HeaderText = "Type";
			this.colType.Name = "colType";
			this.colType.ReadOnly = true;
			this.colType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// colValue
			// 
			this.colValue.HeaderText = "Value";
			this.colValue.Name = "colValue";
			this.colValue.ReadOnly = true;
			this.colValue.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// colDescription
			// 
			this.colDescription.HeaderText = "Description";
			this.colDescription.Name = "colDescription";
			this.colDescription.ReadOnly = true;
			this.colDescription.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// colMax
			// 
			this.colMax.HeaderText = "Max";
			this.colMax.Name = "colMax";
			this.colMax.ReadOnly = true;
			this.colMax.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// colMin
			// 
			this.colMin.HeaderText = "Min";
			this.colMin.Name = "colMin";
			this.colMin.ReadOnly = true;
			this.colMin.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// colDefault
			// 
			this.colDefault.HeaderText = "Default";
			this.colDefault.Name = "colDefault";
			this.colDefault.ReadOnly = true;
			this.colDefault.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// cmsDGV
			// 
			this.cmsDGV.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cmsDGV.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAdd,
            this.miDelete,
            this.miEdit});
			this.cmsDGV.Name = "cmsDGV";
			this.cmsDGV.Size = new System.Drawing.Size(123, 76);
			// 
			// miAdd
			// 
			this.miAdd.Name = "miAdd";
			this.miAdd.Size = new System.Drawing.Size(122, 24);
			this.miAdd.Text = "Add";
			this.miAdd.Click += new System.EventHandler(this.miAdd_Click);
			// 
			// miDelete
			// 
			this.miDelete.Name = "miDelete";
			this.miDelete.Size = new System.Drawing.Size(122, 24);
			this.miDelete.Text = "Delete";
			this.miDelete.Click += new System.EventHandler(this.miDelete_Click);
			// 
			// miEdit
			// 
			this.miEdit.Name = "miEdit";
			this.miEdit.Size = new System.Drawing.Size(122, 24);
			this.miEdit.Text = "Edit";
			this.miEdit.Click += new System.EventHandler(this.miEdit_Click);
			// 
			// btnWrite
			// 
			this.btnWrite.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnWrite.Location = new System.Drawing.Point(37, 622);
			this.btnWrite.Name = "btnWrite";
			this.btnWrite.Size = new System.Drawing.Size(134, 43);
			this.btnWrite.TabIndex = 7;
			this.btnWrite.Text = "WriteTest";
			this.btnWrite.UseVisualStyleBackColor = true;
			this.btnWrite.Visible = false;
			this.btnWrite.Click += new System.EventHandler(this.btnTest_Click);
			// 
			// btnRead
			// 
			this.btnRead.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnRead.Location = new System.Drawing.Point(177, 622);
			this.btnRead.Name = "btnRead";
			this.btnRead.Size = new System.Drawing.Size(134, 43);
			this.btnRead.TabIndex = 18;
			this.btnRead.Text = "ReadTest";
			this.btnRead.UseVisualStyleBackColor = true;
			this.btnRead.Visible = false;
			this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslbPath,
            this.toolStripStatusLabel2,
            this.tslbCount,
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 706);
			this.statusStrip1.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(1238, 24);
			this.statusStrip1.TabIndex = 22;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// tslbPath
			// 
			this.tslbPath.Name = "tslbPath";
			this.tslbPath.Size = new System.Drawing.Size(55, 19);
			this.tslbPath.Text = "Path：";
			// 
			// toolStripStatusLabel2
			// 
			this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
			this.toolStripStatusLabel2.Size = new System.Drawing.Size(1069, 19);
			this.toolStripStatusLabel2.Spring = true;
			// 
			// tslbCount
			// 
			this.tslbCount.Name = "tslbCount";
			this.tslbCount.Size = new System.Drawing.Size(75, 19);
			this.tslbCount.Text = "Count：0";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(24, 19);
			this.toolStripStatusLabel1.Text = "　";
			// 
			// toolStrip1
			// 
			this.toolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.toolStrip1.AutoSize = false;
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbOpen,
            this.tsbSave,
            this.tsbDownload,
            this.tsbUpload,
            this.tsbUndo,
            this.tsbRedo,
            this.tsbFilter,
            this.tsbHighlight,
            this.tstKeyWord});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
			this.toolStrip1.Size = new System.Drawing.Size(1238, 44);
			this.toolStrip1.TabIndex = 23;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// tstKeyWord
			// 
			this.tstKeyWord.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tstKeyWord.Name = "tstKeyWord";
			this.tstKeyWord.Size = new System.Drawing.Size(200, 44);
			this.tstKeyWord.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tstKeyWord_KeyDown);
			// 
			// tsbOpen
			// 
			this.tsbOpen.AutoSize = false;
			this.tsbOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbOpen.Image = ((System.Drawing.Image)(resources.GetObject("tsbOpen.Image")));
			this.tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbOpen.Name = "tsbOpen";
			this.tsbOpen.Size = new System.Drawing.Size(32, 32);
			this.tsbOpen.Text = "toolStripButton1";
			this.tsbOpen.ToolTipText = "Open(Ctrl + O)";
			this.tsbOpen.Click += new System.EventHandler(this.tsbOpen_Click);
			// 
			// tsbSave
			// 
			this.tsbSave.AutoSize = false;
			this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbSave.Image")));
			this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSave.Name = "tsbSave";
			this.tsbSave.Size = new System.Drawing.Size(32, 32);
			this.tsbSave.Text = "toolStripButton1";
			this.tsbSave.ToolTipText = "Save(Ctrl + S)";
			this.tsbSave.Click += new System.EventHandler(this.tsbSave_Click);
			// 
			// tsbDownload
			// 
			this.tsbDownload.AutoSize = false;
			this.tsbDownload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbDownload.Image = global::INITesting.Properties.Resources.Download;
			this.tsbDownload.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbDownload.Name = "tsbDownload";
			this.tsbDownload.Size = new System.Drawing.Size(32, 30);
			this.tsbDownload.Text = "toolStripButton1";
			this.tsbDownload.Click += new System.EventHandler(this.tsbDownload_Click);
			// 
			// tsbUndo
			// 
			this.tsbUndo.AutoSize = false;
			this.tsbUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbUndo.Image = global::INITesting.Properties.Resources.Undo1;
			this.tsbUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbUndo.Name = "tsbUndo";
			this.tsbUndo.Size = new System.Drawing.Size(32, 32);
			this.tsbUndo.Text = "toolStripButton1";
			this.tsbUndo.ToolTipText = "Undo(Ctrl + Z)";
			this.tsbUndo.Click += new System.EventHandler(this.tsbUndo_Click);
			// 
			// tsbRedo
			// 
			this.tsbRedo.AutoSize = false;
			this.tsbRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbRedo.Image = global::INITesting.Properties.Resources.Redo2;
			this.tsbRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbRedo.Name = "tsbRedo";
			this.tsbRedo.Size = new System.Drawing.Size(32, 32);
			this.tsbRedo.Text = "toolStripButton2";
			this.tsbRedo.ToolTipText = "Redo(Ctrl + Shifht + Z)";
			this.tsbRedo.Click += new System.EventHandler(this.tsbRedo_Click);
			// 
			// tsbFilter
			// 
			this.tsbFilter.AutoSize = false;
			this.tsbFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbFilter.Image = global::INITesting.Properties.Resources.Filter;
			this.tsbFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbFilter.Name = "tsbFilter";
			this.tsbFilter.Size = new System.Drawing.Size(32, 32);
			this.tsbFilter.Text = "toolStripButton1";
			this.tsbFilter.ToolTipText = "Filter(Ctrl + F)";
			this.tsbFilter.Click += new System.EventHandler(this.tsbFilter_Click);
			// 
			// tsbHighlight
			// 
			this.tsbHighlight.AutoSize = false;
			this.tsbHighlight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbHighlight.Image = global::INITesting.Properties.Resources.Highlight;
			this.tsbHighlight.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbHighlight.Name = "tsbHighlight";
			this.tsbHighlight.Size = new System.Drawing.Size(32, 32);
			this.tsbHighlight.Text = "toolStripButton2";
			this.tsbHighlight.ToolTipText = "Highlight(Ctrl + L)";
			this.tsbHighlight.Click += new System.EventHandler(this.tsbHighlight_Click);
			// 
			// tsbUpload
			// 
			this.tsbUpload.AutoSize = false;
			this.tsbUpload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbUpload.Image = global::INITesting.Properties.Resources.Upload;
			this.tsbUpload.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbUpload.Name = "tsbUpload";
			this.tsbUpload.Size = new System.Drawing.Size(40, 40);
			this.tsbUpload.Text = "toolStripButton2";
			this.tsbUpload.Click += new System.EventHandler(this.tsbUpload_Click);
			// 
			// CtrlParamEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1238, 730);
			this.Controls.Add(this.dgvProperties);
			this.Controls.Add(this.btnRead);
			this.Controls.Add(this.btnWrite);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.toolStrip1);
			this.Name = "CtrlParamEditor";
			this.Text = "CtrlPropertiesSetting";
			this.Load += new System.EventHandler(this.CtrlParamEditor_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CtrlParamEditor_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.dgvProperties)).EndInit();
			this.cmsDGV.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvProperties;
        private System.Windows.Forms.ContextMenuStrip cmsDGV;
        private Bindable.ToolStripMenuItem miAdd;
        private Bindable.ToolStripMenuItem miDelete;
        private Bindable.ToolStripMenuItem miEdit;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private Bindable.ToolStripStatusLabel tslbPath;
        private Bindable.ToolStripStatusLabel toolStripStatusLabel2;
        private Bindable.ToolStripStatusLabel tslbCount;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private Bindable.ToolStripButton tsbUndo;
        private Bindable.ToolStripButton tsbRedo;
        private Bindable.ToolStripButton tsbOpen;
        private Bindable.ToolStripButton tsbSave;
        private Bindable.ToolStripButton tsbFilter;
        private Bindable.ToolStripButton tsbHighlight;
        private Bindable.ToolStripTextBox tstKeyWord;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMax;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDefault;
		private Bindable.ToolStripButton tsbDownload;
		private Bindable.ToolStripButton tsbUpload;
	}
}