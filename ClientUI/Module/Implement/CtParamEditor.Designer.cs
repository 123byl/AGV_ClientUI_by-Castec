namespace VehiclePlanner.Module.Implement {
    partial class CtParamEditor {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtParamEditor));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tstKeyWord = new CtBind.Bindable.ToolStripTextBox();
            this.tsbFilter = new CtBind.Bindable.ToolStripButton();
            this.tsbRedo = new CtBind.Bindable.ToolStripButton();
            this.tsbUndo = new CtBind.Bindable.ToolStripButton();
            this.tsbSave = new CtBind.Bindable.ToolStripButton();
            this.tsbOpen = new CtBind.Bindable.ToolStripButton();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslbCount = new CtBind.Bindable.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new CtBind.Bindable.ToolStripStatusLabel();
            this.tslbPath = new CtBind.Bindable.ToolStripStatusLabel();
            this.tsbHighlight = new CtBind.Bindable.ToolStripButton();
            this.dgvProperties = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDefault = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnWrite = new System.Windows.Forms.Button();
            this.miDelete = new CtBind.Bindable.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.miAdd = new CtBind.Bindable.ToolStripMenuItem();
            this.cmsDGV = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miEdit = new CtBind.Bindable.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnRead = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProperties)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.cmsDGV.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tstKeyWord
            // 
            this.tstKeyWord.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tstKeyWord.Name = "tstKeyWord";
            this.tstKeyWord.Size = new System.Drawing.Size(200, 27);
            // 
            // tsbFilter
            // 
            this.tsbFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFilter.Image = global::VehiclePlanner.Properties.Resources.Filter;
            this.tsbFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFilter.Name = "tsbFilter";
            this.tsbFilter.Size = new System.Drawing.Size(24, 24);
            this.tsbFilter.Text = "toolStripButton1";
            this.tsbFilter.ToolTipText = "Filter(Ctrl + F)";
            // 
            // tsbRedo
            // 
            this.tsbRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRedo.Image = global::VehiclePlanner.Properties.Resources.Redo2;
            this.tsbRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRedo.Name = "tsbRedo";
            this.tsbRedo.Size = new System.Drawing.Size(24, 24);
            this.tsbRedo.Text = "toolStripButton2";
            this.tsbRedo.ToolTipText = "Redo(Ctrl + Shifht + Z)";
            // 
            // tsbUndo
            // 
            this.tsbUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbUndo.Image = global::VehiclePlanner.Properties.Resources.Undo1;
            this.tsbUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbUndo.Name = "tsbUndo";
            this.tsbUndo.Size = new System.Drawing.Size(24, 24);
            this.tsbUndo.Text = "toolStripButton1";
            this.tsbUndo.ToolTipText = "Undo(Ctrl + Z)";
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbSave.Image")));
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(24, 24);
            this.tsbSave.Text = "toolStripButton1";
            this.tsbSave.ToolTipText = "Save(Ctrl + S)";
            // 
            // tsbOpen
            // 
            this.tsbOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpen.Image = ((System.Drawing.Image)(resources.GetObject("tsbOpen.Image")));
            this.tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpen.Name = "tsbOpen";
            this.tsbOpen.Size = new System.Drawing.Size(24, 24);
            this.tsbOpen.Text = "toolStripButton1";
            this.tsbOpen.ToolTipText = "Open(Ctrl + O)";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(24, 19);
            this.toolStripStatusLabel1.Text = "　";
            // 
            // tslbCount
            // 
            this.tslbCount.Name = "tslbCount";
            this.tslbCount.Size = new System.Drawing.Size(75, 19);
            this.tslbCount.Text = "Count：0";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(844, 19);
            this.toolStripStatusLabel2.Spring = true;
            // 
            // tslbPath
            // 
            this.tslbPath.Name = "tslbPath";
            this.tslbPath.Size = new System.Drawing.Size(55, 19);
            this.tslbPath.Text = "Path：";
            // 
            // tsbHighlight
            // 
            this.tsbHighlight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbHighlight.Image = global::VehiclePlanner.Properties.Resources.Highlight;
            this.tsbHighlight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHighlight.Name = "tsbHighlight";
            this.tsbHighlight.Size = new System.Drawing.Size(24, 24);
            this.tsbHighlight.Text = "toolStripButton2";
            this.tsbHighlight.ToolTipText = "Highlight(Ctrl + L)";
            // 
            // dgvProperties
            // 
            this.dgvProperties.AllowUserToAddRows = false;
            this.dgvProperties.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvProperties.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProperties.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colType,
            this.colValue,
            this.colDescription,
            this.colMax,
            this.colMin,
            this.colDefault});
            this.dgvProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProperties.Location = new System.Drawing.Point(0, 27);
            this.dgvProperties.Name = "dgvProperties";
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dgvProperties.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvProperties.RowTemplate.Height = 27;
            this.dgvProperties.Size = new System.Drawing.Size(1013, 587);
            this.dgvProperties.TabIndex = 24;
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
            // btnWrite
            // 
            this.btnWrite.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnWrite.Location = new System.Drawing.Point(33, 569);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(134, 43);
            this.btnWrite.TabIndex = 25;
            this.btnWrite.Text = "WriteTest";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Visible = false;
            // 
            // miDelete
            // 
            this.miDelete.Name = "miDelete";
            this.miDelete.Size = new System.Drawing.Size(122, 24);
            this.miDelete.Text = "Delete";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslbPath,
            this.toolStripStatusLabel2,
            this.tslbCount,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 614);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1013, 24);
            this.statusStrip1.TabIndex = 27;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // miAdd
            // 
            this.miAdd.Name = "miAdd";
            this.miAdd.Size = new System.Drawing.Size(122, 24);
            this.miAdd.Text = "Add";
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
            // miEdit
            // 
            this.miEdit.Name = "miEdit";
            this.miEdit.Size = new System.Drawing.Size(122, 24);
            this.miEdit.Text = "Edit";
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbOpen,
            this.tsbSave,
            this.tsbUndo,
            this.tsbRedo,
            this.tsbFilter,
            this.tsbHighlight,
            this.tstKeyWord});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1013, 27);
            this.toolStrip1.TabIndex = 28;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnRead
            // 
            this.btnRead.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRead.Location = new System.Drawing.Point(173, 569);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(134, 43);
            this.btnRead.TabIndex = 26;
            this.btnRead.Text = "ReadTest";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Visible = false;
            // 
            // CtParamEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1013, 638);
            this.Controls.Add(this.dgvProperties);
            this.Controls.Add(this.btnWrite);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btnRead);
            this.Name = "CtParamEditor";
            this.Text = "CtParamEditor";
            ((System.ComponentModel.ISupportInitialize)(this.dgvProperties)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.cmsDGV.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CtBind.Bindable.ToolStripTextBox tstKeyWord;
        private CtBind.Bindable.ToolStripButton tsbFilter;
        private CtBind.Bindable.ToolStripButton tsbRedo;
        private CtBind.Bindable.ToolStripButton tsbUndo;
        private CtBind.Bindable.ToolStripButton tsbSave;
        private CtBind.Bindable.ToolStripButton tsbOpen;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private CtBind.Bindable.ToolStripStatusLabel tslbCount;
        private CtBind.Bindable.ToolStripStatusLabel toolStripStatusLabel2;
        private CtBind.Bindable.ToolStripStatusLabel tslbPath;
        private CtBind.Bindable.ToolStripButton tsbHighlight;
        private System.Windows.Forms.DataGridView dgvProperties;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMax;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDefault;
        private System.Windows.Forms.Button btnWrite;
        private CtBind.Bindable.ToolStripMenuItem miDelete;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private CtBind.Bindable.ToolStripMenuItem miAdd;
        private System.Windows.Forms.ContextMenuStrip cmsDGV;
        private CtBind.Bindable.ToolStripMenuItem miEdit;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Button btnRead;
    }
}