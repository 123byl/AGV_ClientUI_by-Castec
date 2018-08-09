
namespace CtItsParameter
{
	partial class ItsParameterCtrl
	{
		/// <summary>
		/// 設計工具所需的變數。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清除任何使用中的資源。
		/// </summary>
		/// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form 設計工具產生的程式碼

		/// <summary>
		/// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
		/// 這個方法的內容。
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItsParameterCtrl));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.tsbOpenFile = new CtBind.Bindable.ToolStripButton();
			this.tsbSaveFile = new CtBind.Bindable.ToolStripButton();
			this.tsbDownload = new CtBind.Bindable.ToolStripButton();
			this.tsbUpLoad = new CtBind.Bindable.ToolStripButton();
			this.tsbUndo = new System.Windows.Forms.ToolStripButton();
			this.tsbRedo = new System.Windows.Forms.ToolStripButton();
			this.dgvParameter = new System.Windows.Forms.DataGridView();
			this.dataGridViewRichTextBoxColumn1 = new CtExtendLib.DataGridViewRichTextBoxColumn();
			this.dataGridViewRichTextBoxColumn2 = new CtExtendLib.DataGridViewRichTextBoxColumn();
			this.dataGridViewRichTextBoxColumn3 = new CtExtendLib.DataGridViewRichTextBoxColumn();
			this.dataGridViewRichTextBoxColumn4 = new CtExtendLib.DataGridViewRichTextBoxColumn();
			this.dataGridViewRichTextBoxColumn5 = new CtExtendLib.DataGridViewRichTextBoxColumn();
			this.dataGridViewRichTextBoxColumn6 = new CtExtendLib.DataGridViewRichTextBoxColumn();
			this.dataGridViewRichTextBoxColumn7 = new CtExtendLib.DataGridViewRichTextBoxColumn();
			this.colName = new CtExtendLib.DataGridViewRichTextBoxColumn();
			this.colType = new CtExtendLib.DataGridViewRichTextBoxColumn();
			this.colValue = new CtExtendLib.DataGridViewRichTextBoxColumn();
			this.colDefault = new CtExtendLib.DataGridViewRichTextBoxColumn();
			this.colMin = new CtExtendLib.DataGridViewRichTextBoxColumn();
			this.colMax = new CtExtendLib.DataGridViewRichTextBoxColumn();
			this.colDescription = new CtExtendLib.DataGridViewRichTextBoxColumn();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvParameter)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.toolStrip1.AutoSize = false;
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbOpenFile,
            this.tsbSaveFile,
            this.tsbDownload,
            this.tsbUpLoad,
            this.tsbUndo,
            this.tsbRedo});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(1108, 44);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// tsbOpenFile
			// 
			this.tsbOpenFile.AutoSize = false;
			this.tsbOpenFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbOpenFile.Image = global::CtItsParameter.Properties.Resources.OpenFile;
			this.tsbOpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbOpenFile.Name = "tsbOpenFile";
			this.tsbOpenFile.Size = new System.Drawing.Size(32, 32);
			this.tsbOpenFile.Text = "toolStripButton1";
			this.tsbOpenFile.ToolTipText = "Open";
			this.tsbOpenFile.Click += new System.EventHandler(this.tsbOpenFile_Click);
			// 
			// tsbSaveFile
			// 
			this.tsbSaveFile.AutoSize = false;
			this.tsbSaveFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSaveFile.Image = ((System.Drawing.Image)(resources.GetObject("tsbSaveFile.Image")));
			this.tsbSaveFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSaveFile.Name = "tsbSaveFile";
			this.tsbSaveFile.Size = new System.Drawing.Size(32, 32);
			this.tsbSaveFile.Text = "toolStripButton4";
			this.tsbSaveFile.ToolTipText = "Save";
			this.tsbSaveFile.Click += new System.EventHandler(this.tsbSaveFile_Click);
			// 
			// tsbDownload
			// 
			this.tsbDownload.AutoSize = false;
			this.tsbDownload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbDownload.Image = global::CtItsParameter.Properties.Resources.Download;
			this.tsbDownload.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbDownload.Name = "tsbDownload";
			this.tsbDownload.Size = new System.Drawing.Size(32, 32);
			this.tsbDownload.Text = "toolStripButton2";
			this.tsbDownload.ToolTipText = "Download";
			this.tsbDownload.Click += new System.EventHandler(this.tsbDownload_Click);
			// 
			// tsbUpLoad
			// 
			this.tsbUpLoad.AutoSize = false;
			this.tsbUpLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbUpLoad.Image = global::CtItsParameter.Properties.Resources.Upload;
			this.tsbUpLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbUpLoad.Name = "tsbUpLoad";
			this.tsbUpLoad.Size = new System.Drawing.Size(32, 32);
			this.tsbUpLoad.Text = "toolStripButton3";
			this.tsbUpLoad.ToolTipText = "Upload";
			this.tsbUpLoad.Click += new System.EventHandler(this.tsbUpload_Click);
			// 
			// tsbUndo
			// 
			this.tsbUndo.AutoSize = false;
			this.tsbUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbUndo.Image = global::CtItsParameter.Properties.Resources.Undo;
			this.tsbUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbUndo.Name = "tsbUndo";
			this.tsbUndo.Size = new System.Drawing.Size(32, 32);
			this.tsbUndo.Text = "toolStripButton5";
			this.tsbUndo.ToolTipText = "Undo";
			this.tsbUndo.Click += new System.EventHandler(this.tsbUndo_Click);
			// 
			// tsbRedo
			// 
			this.tsbRedo.AutoSize = false;
			this.tsbRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbRedo.Image = global::CtItsParameter.Properties.Resources.Redo;
			this.tsbRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbRedo.Name = "tsbRedo";
			this.tsbRedo.Size = new System.Drawing.Size(32, 32);
			this.tsbRedo.Text = "toolStripButton6";
			this.tsbRedo.ToolTipText = "Redo";
			this.tsbRedo.Click += new System.EventHandler(this.tsbRedo_Click);
			// 
			// dgvParameter
			// 
			this.dgvParameter.AllowUserToAddRows = false;
			this.dgvParameter.AllowUserToDeleteRows = false;
			this.dgvParameter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvParameter.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvParameter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvParameter.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colType,
            this.colValue,
            this.colDefault,
            this.colMin,
            this.colMax,
            this.colDescription});
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvParameter.DefaultCellStyle = dataGridViewCellStyle2;
			this.dgvParameter.Location = new System.Drawing.Point(10, 45);
			this.dgvParameter.Margin = new System.Windows.Forms.Padding(1);
			this.dgvParameter.Name = "dgvParameter";
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvParameter.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.dgvParameter.RowHeadersVisible = false;
			this.dgvParameter.RowTemplate.Height = 27;
			this.dgvParameter.Size = new System.Drawing.Size(1088, 737);
			this.dgvParameter.TabIndex = 1;
			// 
			// dataGridViewRichTextBoxColumn1
			// 
			this.dataGridViewRichTextBoxColumn1.HeaderText = "Name";
			this.dataGridViewRichTextBoxColumn1.Name = "dataGridViewRichTextBoxColumn1";
			this.dataGridViewRichTextBoxColumn1.ReadOnly = true;
			this.dataGridViewRichTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridViewRichTextBoxColumn1.Width = 250;
			// 
			// dataGridViewRichTextBoxColumn2
			// 
			this.dataGridViewRichTextBoxColumn2.HeaderText = "Type";
			this.dataGridViewRichTextBoxColumn2.Name = "dataGridViewRichTextBoxColumn2";
			this.dataGridViewRichTextBoxColumn2.ReadOnly = true;
			this.dataGridViewRichTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// dataGridViewRichTextBoxColumn3
			// 
			this.dataGridViewRichTextBoxColumn3.HeaderText = "Value";
			this.dataGridViewRichTextBoxColumn3.Name = "dataGridViewRichTextBoxColumn3";
			this.dataGridViewRichTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridViewRichTextBoxColumn3.Width = 150;
			// 
			// dataGridViewRichTextBoxColumn4
			// 
			this.dataGridViewRichTextBoxColumn4.HeaderText = "Default";
			this.dataGridViewRichTextBoxColumn4.Name = "dataGridViewRichTextBoxColumn4";
			this.dataGridViewRichTextBoxColumn4.ReadOnly = true;
			this.dataGridViewRichTextBoxColumn4.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridViewRichTextBoxColumn4.Width = 150;
			// 
			// dataGridViewRichTextBoxColumn5
			// 
			this.dataGridViewRichTextBoxColumn5.HeaderText = "Min";
			this.dataGridViewRichTextBoxColumn5.Name = "dataGridViewRichTextBoxColumn5";
			this.dataGridViewRichTextBoxColumn5.ReadOnly = true;
			this.dataGridViewRichTextBoxColumn5.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// dataGridViewRichTextBoxColumn6
			// 
			this.dataGridViewRichTextBoxColumn6.HeaderText = "Max";
			this.dataGridViewRichTextBoxColumn6.Name = "dataGridViewRichTextBoxColumn6";
			this.dataGridViewRichTextBoxColumn6.ReadOnly = true;
			this.dataGridViewRichTextBoxColumn6.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// dataGridViewRichTextBoxColumn7
			// 
			this.dataGridViewRichTextBoxColumn7.HeaderText = "Description";
			this.dataGridViewRichTextBoxColumn7.Name = "dataGridViewRichTextBoxColumn7";
			this.dataGridViewRichTextBoxColumn7.ReadOnly = true;
			this.dataGridViewRichTextBoxColumn7.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridViewRichTextBoxColumn7.Width = 400;
			// 
			// colName
			// 
			this.colName.HeaderText = "Name";
			this.colName.Name = "colName";
			this.colName.ReadOnly = true;
			this.colName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
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
			this.colValue.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.colValue.Width = 150;
			// 
			// colDefault
			// 
			this.colDefault.HeaderText = "Default";
			this.colDefault.Name = "colDefault";
			this.colDefault.ReadOnly = true;
			this.colDefault.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.colDefault.Width = 150;
			// 
			// colMin
			// 
			this.colMin.HeaderText = "Min";
			this.colMin.Name = "colMin";
			this.colMin.ReadOnly = true;
			this.colMin.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// colMax
			// 
			this.colMax.HeaderText = "Max";
			this.colMax.Name = "colMax";
			this.colMax.ReadOnly = true;
			this.colMax.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// colDescription
			// 
			this.colDescription.HeaderText = "Description";
			this.colDescription.Name = "colDescription";
			this.colDescription.ReadOnly = true;
			this.colDescription.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.colDescription.Width = 400;
			// 
			// ItsParameterCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1108, 792);
			this.Controls.Add(this.dgvParameter);
			this.Controls.Add(this.toolStrip1);
			this.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.Name = "ItsParameterCtrl";
			this.Text = "Parameter";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvParameter)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private CtBind.Bindable.ToolStripButton tsbOpenFile;
		private CtBind.Bindable.ToolStripButton tsbDownload;
		private CtBind.Bindable.ToolStripButton tsbUpLoad;
		private CtBind.Bindable.ToolStripButton tsbSaveFile;
		private System.Windows.Forms.ToolStripButton tsbUndo;
		private System.Windows.Forms.ToolStripButton tsbRedo;
		private System.Windows.Forms.DataGridView dgvParameter;
		private CtExtendLib.DataGridViewRichTextBoxColumn colName;
		private CtExtendLib.DataGridViewRichTextBoxColumn colType;
		private CtExtendLib.DataGridViewRichTextBoxColumn colValue;
		private CtExtendLib.DataGridViewRichTextBoxColumn colDefault;
		private CtExtendLib.DataGridViewRichTextBoxColumn colMin;
		private CtExtendLib.DataGridViewRichTextBoxColumn colMax;
		private CtExtendLib.DataGridViewRichTextBoxColumn colDescription;
		private CtExtendLib.DataGridViewRichTextBoxColumn dataGridViewRichTextBoxColumn1;
		private CtExtendLib.DataGridViewRichTextBoxColumn dataGridViewRichTextBoxColumn2;
		private CtExtendLib.DataGridViewRichTextBoxColumn dataGridViewRichTextBoxColumn3;
		private CtExtendLib.DataGridViewRichTextBoxColumn dataGridViewRichTextBoxColumn4;
		private CtExtendLib.DataGridViewRichTextBoxColumn dataGridViewRichTextBoxColumn5;
		private CtExtendLib.DataGridViewRichTextBoxColumn dataGridViewRichTextBoxColumn6;
		private CtExtendLib.DataGridViewRichTextBoxColumn dataGridViewRichTextBoxColumn7;
	}
}

