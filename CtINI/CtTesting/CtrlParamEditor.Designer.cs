using CtBind;
using System;

namespace CtTesting {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.btnSave = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.btnAll = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRestoreDefault = new System.Windows.Forms.Button();
            this.btnWrite = new System.Windows.Forms.Button();
            this.btnNewRow = new System.Windows.Forms.Button();
            this.lbRegularFont = new System.Windows.Forms.Label();
            this.lbHighlightFont = new System.Windows.Forms.Label();
            this.lbRgFore = new System.Windows.Forms.Label();
            this.lbRgBack = new System.Windows.Forms.Label();
            this.lbHlBack = new System.Windows.Forms.Label();
            this.lbHlFore = new System.Windows.Forms.Label();
            this.txtKeyWord = new System.Windows.Forms.TextBox();
            this.btnHighlight = new System.Windows.Forms.Button();
            this.btnRead = new System.Windows.Forms.Button();
            this.lbRowCount = new System.Windows.Forms.Label();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnRedo = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProperties)).BeginInit();
            this.cmsDGV.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvProperties
            // 
            this.dgvProperties.AllowUserToAddRows = false;
            this.dgvProperties.AllowUserToOrderColumns = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvProperties.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProperties.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colType,
            this.colValue,
            this.colDescription,
            this.colMax,
            this.colMin,
            this.colDefault});
            this.dgvProperties.Location = new System.Drawing.Point(12, 190);
            this.dgvProperties.Name = "dgvProperties";
            dataGridViewCellStyle4.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dgvProperties.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvProperties.RowTemplate.Height = 27;
            this.dgvProperties.Size = new System.Drawing.Size(1171, 479);
            this.dgvProperties.TabIndex = 0;
            // 
            // colName
            // 
            this.colName.HeaderText = "Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
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
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSave.Location = new System.Drawing.Point(135, 111);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(112, 43);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnOpen.Location = new System.Drawing.Point(17, 111);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(112, 43);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnFilter.Location = new System.Drawing.Point(17, 62);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(121, 43);
            this.btnFilter.TabIndex = 3;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // btnAll
            // 
            this.btnAll.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btnAll.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnAll.Location = new System.Drawing.Point(253, 111);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(112, 43);
            this.btnAll.TabIndex = 4;
            this.btnAll.Text = "All";
            this.btnAll.UseVisualStyleBackColor = true;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnClear.Location = new System.Drawing.Point(371, 112);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(112, 43);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRestoreDefault
            // 
            this.btnRestoreDefault.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRestoreDefault.Location = new System.Drawing.Point(489, 112);
            this.btnRestoreDefault.Name = "btnRestoreDefault";
            this.btnRestoreDefault.Size = new System.Drawing.Size(187, 43);
            this.btnRestoreDefault.TabIndex = 6;
            this.btnRestoreDefault.Text = "Restore default";
            this.btnRestoreDefault.UseVisualStyleBackColor = true;
            this.btnRestoreDefault.Click += new System.EventHandler(this.btnRestoreDefault_Click);
            // 
            // btnWrite
            // 
            this.btnWrite.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnWrite.Location = new System.Drawing.Point(12, 675);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(134, 43);
            this.btnWrite.TabIndex = 7;
            this.btnWrite.Text = "WriteTest";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnNewRow
            // 
            this.btnNewRow.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnNewRow.Location = new System.Drawing.Point(682, 111);
            this.btnNewRow.Name = "btnNewRow";
            this.btnNewRow.Size = new System.Drawing.Size(112, 43);
            this.btnNewRow.TabIndex = 8;
            this.btnNewRow.Text = "New row";
            this.btnNewRow.UseVisualStyleBackColor = true;
            this.btnNewRow.Click += new System.EventHandler(this.btnNewRow_Click);
            // 
            // lbRegularFont
            // 
            this.lbRegularFont.AutoSize = true;
            this.lbRegularFont.Location = new System.Drawing.Point(70, 9);
            this.lbRegularFont.Name = "lbRegularFont";
            this.lbRegularFont.Size = new System.Drawing.Size(41, 15);
            this.lbRegularFont.TabIndex = 9;
            this.lbRegularFont.Tag = "一般文字";
            this.lbRegularFont.Text = "label1";
            this.lbRegularFont.DoubleClick += new System.EventHandler(this.lbRegularFont_DoubleClick);
            // 
            // lbHighlightFont
            // 
            this.lbHighlightFont.AutoSize = true;
            this.lbHighlightFont.Location = new System.Drawing.Point(655, 9);
            this.lbHighlightFont.Name = "lbHighlightFont";
            this.lbHighlightFont.Size = new System.Drawing.Size(41, 15);
            this.lbHighlightFont.TabIndex = 11;
            this.lbHighlightFont.Tag = "標記文字";
            this.lbHighlightFont.Text = "label1";
            this.lbHighlightFont.DoubleClick += new System.EventHandler(this.lbHightlightFont_OnDoubleClick);
            // 
            // lbRgFore
            // 
            this.lbRgFore.AutoSize = true;
            this.lbRgFore.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbRgFore.Location = new System.Drawing.Point(12, 9);
            this.lbRgFore.Name = "lbRgFore";
            this.lbRgFore.Size = new System.Drawing.Size(52, 25);
            this.lbRgFore.TabIndex = 12;
            this.lbRgFore.Text = "填色";
            this.lbRgFore.DoubleClick += new System.EventHandler(this.lbRgFore_DoubleClick);
            // 
            // lbRgBack
            // 
            this.lbRgBack.AutoSize = true;
            this.lbRgBack.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbRgBack.Location = new System.Drawing.Point(12, 34);
            this.lbRgBack.Name = "lbRgBack";
            this.lbRgBack.Size = new System.Drawing.Size(52, 25);
            this.lbRgBack.TabIndex = 13;
            this.lbRgBack.Text = "底色";
            this.lbRgBack.DoubleClick += new System.EventHandler(this.lbRgBack_DoubleClick);
            // 
            // lbHlBack
            // 
            this.lbHlBack.AutoSize = true;
            this.lbHlBack.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbHlBack.Location = new System.Drawing.Point(597, 34);
            this.lbHlBack.Name = "lbHlBack";
            this.lbHlBack.Size = new System.Drawing.Size(52, 25);
            this.lbHlBack.TabIndex = 15;
            this.lbHlBack.Text = "底色";
            this.lbHlBack.DoubleClick += new System.EventHandler(this.lbHlBack_DoubleClick);
            // 
            // lbHlFore
            // 
            this.lbHlFore.AutoSize = true;
            this.lbHlFore.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbHlFore.Location = new System.Drawing.Point(597, 9);
            this.lbHlFore.Name = "lbHlFore";
            this.lbHlFore.Size = new System.Drawing.Size(52, 25);
            this.lbHlFore.TabIndex = 14;
            this.lbHlFore.Text = "填色";
            this.lbHlFore.DoubleClick += new System.EventHandler(this.lbHlFore_DoubleClick);
            // 
            // txtKeyWord
            // 
            this.txtKeyWord.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtKeyWord.Location = new System.Drawing.Point(271, 68);
            this.txtKeyWord.Name = "txtKeyWord";
            this.txtKeyWord.Size = new System.Drawing.Size(260, 34);
            this.txtKeyWord.TabIndex = 16;
            // 
            // btnHighlight
            // 
            this.btnHighlight.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnHighlight.Location = new System.Drawing.Point(144, 62);
            this.btnHighlight.Name = "btnHighlight";
            this.btnHighlight.Size = new System.Drawing.Size(121, 43);
            this.btnHighlight.TabIndex = 17;
            this.btnHighlight.Text = "Hightlight";
            this.btnHighlight.UseVisualStyleBackColor = true;
            this.btnHighlight.Click += new System.EventHandler(this.btnHighlight_Click);
            // 
            // btnRead
            // 
            this.btnRead.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRead.Location = new System.Drawing.Point(152, 675);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(134, 43);
            this.btnRead.TabIndex = 18;
            this.btnRead.Text = "ReadTest";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // lbRowCount
            // 
            this.lbRowCount.AutoSize = true;
            this.lbRowCount.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbRowCount.Location = new System.Drawing.Point(1077, 672);
            this.lbRowCount.Name = "lbRowCount";
            this.lbRowCount.Size = new System.Drawing.Size(69, 25);
            this.lbRowCount.TabIndex = 19;
            this.lbRowCount.Tag = "一般文字";
            this.lbRowCount.Text = "label1";
            // 
            // btnUndo
            // 
            this.btnUndo.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnUndo.Location = new System.Drawing.Point(845, 112);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(112, 43);
            this.btnUndo.TabIndex = 20;
            this.btnUndo.Text = "Undo";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnRedo
            // 
            this.btnRedo.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRedo.Location = new System.Drawing.Point(963, 111);
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(112, 43);
            this.btnRedo.TabIndex = 21;
            this.btnRedo.Text = "Redo";
            this.btnRedo.UseVisualStyleBackColor = true;
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // CtrlParamEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1238, 730);
            this.Controls.Add(this.btnRedo);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.lbRowCount);
            this.Controls.Add(this.btnRead);
            this.Controls.Add(this.btnHighlight);
            this.Controls.Add(this.txtKeyWord);
            this.Controls.Add(this.lbHlBack);
            this.Controls.Add(this.lbHlFore);
            this.Controls.Add(this.lbRgBack);
            this.Controls.Add(this.lbRgFore);
            this.Controls.Add(this.lbHighlightFont);
            this.Controls.Add(this.lbRegularFont);
            this.Controls.Add(this.dgvProperties);
            this.Controls.Add(this.btnNewRow);
            this.Controls.Add(this.btnWrite);
            this.Controls.Add(this.btnRestoreDefault);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnAll);
            this.Controls.Add(this.btnFilter);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnSave);
            this.Name = "CtrlParamEditor";
            this.Text = "CtrlPropertiesSetting";
            ((System.ComponentModel.ISupportInitialize)(this.dgvProperties)).EndInit();
            this.cmsDGV.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvProperties;
        private System.Windows.Forms.ContextMenuStrip cmsDGV;
        private Bindable.ToolStripMenuItem miAdd;
        private Bindable.ToolStripMenuItem miDelete;
        private Bindable.ToolStripMenuItem miEdit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnRestoreDefault;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.Button btnNewRow;
        private System.Windows.Forms.Label lbRegularFont;
        private System.Windows.Forms.Label lbHighlightFont;
        private System.Windows.Forms.Label lbRgFore;
        private System.Windows.Forms.Label lbRgBack;
        private System.Windows.Forms.Label lbHlBack;
        private System.Windows.Forms.Label lbHlFore;
        private System.Windows.Forms.TextBox txtKeyWord;
        private System.Windows.Forms.Button btnHighlight;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMax;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDefault;
        private System.Windows.Forms.Label lbRowCount;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnRedo;
    }
}