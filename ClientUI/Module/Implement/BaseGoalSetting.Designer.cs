using CtBind;
using VehiclePlanner.Partial.VehiclePlannerUI;

namespace VehiclePlanner.Module.Implement {
    partial class BaseGoalSetting {
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseGoalSetting));
			this.cmbGoalList = new System.Windows.Forms.ComboBox();
			this.label14 = new System.Windows.Forms.Label();
			this.txtAddPtheta = new System.Windows.Forms.TextBox();
			this.txtAddPy = new System.Windows.Forms.TextBox();
			this.txtAddPx = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.btnGetGoalList = new System.Windows.Forms.Button();
			this.dgvGoalPoint = new CtBind.Inheritable.DataGridView();
			this.cSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.cID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.cName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.cX = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.cY = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.cToward = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.tsbAddNow = new System.Windows.Forms.ToolStripButton();
			this.tsbDelete = new System.Windows.Forms.ToolStripButton();
			this.tsbSave = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsbRun = new System.Windows.Forms.ToolStripButton();
			this.tsbRunAll = new System.Windows.Forms.ToolStripButton();
			this.tsbStop = new CtBind.Bindable.ToolStripButton();
			this.tsbCharging = new System.Windows.Forms.ToolStripButton();
			this.tsbPath = new System.Windows.Forms.ToolStripButton();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miPath = new System.Windows.Forms.ToolStripMenuItem();
			this.miRun = new System.Windows.Forms.ToolStripMenuItem();
			this.miCharging = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.miDelete = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.dgvGoalPoint)).BeginInit();
			this.toolStrip1.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmbGoalList
			// 
			this.cmbGoalList.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.cmbGoalList.FormattingEnabled = true;
			this.cmbGoalList.Location = new System.Drawing.Point(565, 115);
			this.cmbGoalList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.cmbGoalList.Name = "cmbGoalList";
			this.cmbGoalList.Size = new System.Drawing.Size(160, 33);
			this.cmbGoalList.TabIndex = 57;
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label14.Location = new System.Drawing.Point(328, 155);
			this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(20, 22);
			this.label14.TabIndex = 52;
			this.label14.Text = "θ";
			// 
			// txtAddPtheta
			// 
			this.txtAddPtheta.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.txtAddPtheta.Location = new System.Drawing.Point(293, 184);
			this.txtAddPtheta.Margin = new System.Windows.Forms.Padding(4);
			this.txtAddPtheta.Name = "txtAddPtheta";
			this.txtAddPtheta.Size = new System.Drawing.Size(95, 34);
			this.txtAddPtheta.TabIndex = 49;
			this.txtAddPtheta.Text = "0";
			this.txtAddPtheta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// txtAddPy
			// 
			this.txtAddPy.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.txtAddPy.Location = new System.Drawing.Point(173, 184);
			this.txtAddPy.Margin = new System.Windows.Forms.Padding(4);
			this.txtAddPy.Name = "txtAddPy";
			this.txtAddPy.Size = new System.Drawing.Size(95, 34);
			this.txtAddPy.TabIndex = 50;
			this.txtAddPy.Text = "0";
			this.txtAddPy.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// txtAddPx
			// 
			this.txtAddPx.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.txtAddPx.Location = new System.Drawing.Point(56, 184);
			this.txtAddPx.Margin = new System.Windows.Forms.Padding(4);
			this.txtAddPx.Name = "txtAddPx";
			this.txtAddPx.Size = new System.Drawing.Size(95, 34);
			this.txtAddPx.TabIndex = 51;
			this.txtAddPx.Text = "0";
			this.txtAddPx.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label15.Location = new System.Drawing.Point(213, 155);
			this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(19, 22);
			this.label15.TabIndex = 53;
			this.label15.Text = "y";
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label16.Location = new System.Drawing.Point(93, 155);
			this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(19, 22);
			this.label16.TabIndex = 54;
			this.label16.Text = "x";
			// 
			// btnGetGoalList
			// 
			this.btnGetGoalList.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.btnGetGoalList.Location = new System.Drawing.Point(695, 726);
			this.btnGetGoalList.Margin = new System.Windows.Forms.Padding(4);
			this.btnGetGoalList.Name = "btnGetGoalList";
			this.btnGetGoalList.Size = new System.Drawing.Size(160, 62);
			this.btnGetGoalList.TabIndex = 64;
			this.btnGetGoalList.Text = "Get Goal List";
			this.btnGetGoalList.UseVisualStyleBackColor = true;
			this.btnGetGoalList.Visible = false;
			this.btnGetGoalList.Click += new System.EventHandler(this.btnGetGoalList_Click);
			// 
			// dgvGoalPoint
			// 
			this.dgvGoalPoint.AllowUserToAddRows = false;
			this.dgvGoalPoint.AllowUserToResizeRows = false;
			this.dgvGoalPoint.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
			this.dgvGoalPoint.BackgroundColor = System.Drawing.SystemColors.ControlLight;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvGoalPoint.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvGoalPoint.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvGoalPoint.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cSelect,
            this.cID,
            this.cName,
            this.cX,
            this.cY,
            this.cToward});
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvGoalPoint.DefaultCellStyle = dataGridViewCellStyle2;
			this.dgvGoalPoint.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvGoalPoint.Location = new System.Drawing.Point(0, 27);
			this.dgvGoalPoint.Margin = new System.Windows.Forms.Padding(4);
			this.dgvGoalPoint.Name = "dgvGoalPoint";
			this.dgvGoalPoint.RowHeadersVisible = false;
			this.dgvGoalPoint.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
			this.dgvGoalPoint.RowTemplate.Height = 24;
			this.dgvGoalPoint.Size = new System.Drawing.Size(752, 749);
			this.dgvGoalPoint.TabIndex = 46;
			this.dgvGoalPoint.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvGoalPoint_CellMouseClick);
			// 
			// cSelect
			// 
			this.cSelect.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.cSelect.HeaderText = "Select";
			this.cSelect.Name = "cSelect";
			this.cSelect.Width = 73;
			// 
			// cID
			// 
			this.cID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.cID.HeaderText = "ID";
			this.cID.Name = "cID";
			this.cID.Width = 62;
			// 
			// cName
			// 
			this.cName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.cName.HeaderText = "Name";
			this.cName.Name = "cName";
			this.cName.Width = 98;
			// 
			// cX
			// 
			this.cX.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.cX.DataPropertyName = "double";
			this.cX.HeaderText = "X";
			this.cX.Name = "cX";
			this.cX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// cY
			// 
			this.cY.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.cY.DataPropertyName = "double";
			this.cY.HeaderText = "Y";
			this.cY.Name = "cY";
			this.cY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// cToward
			// 
			this.cToward.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.cToward.DataPropertyName = "double";
			this.cToward.HeaderText = "Toward";
			this.cToward.Name = "cToward";
			this.cToward.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// toolStrip1
			// 
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbAddNow,
            this.tsbDelete,
            this.tsbSave,
            this.toolStripSeparator1,
            this.tsbRun,
            this.tsbRunAll,
            this.tsbStop,
            this.tsbCharging,
            this.tsbPath});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(752, 27);
			this.toolStrip1.TabIndex = 69;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// tsbAddNow
			// 
			this.tsbAddNow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbAddNow.Image = global::VehiclePlanner.Properties.Resources.Add;
			this.tsbAddNow.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbAddNow.Name = "tsbAddNow";
			this.tsbAddNow.Size = new System.Drawing.Size(24, 24);
			this.tsbAddNow.Text = "Add now";
			this.tsbAddNow.ToolTipText = "Add now(Ctrl + N)";
			this.tsbAddNow.Click += new System.EventHandler(this.tsbAddNow_Click);
			// 
			// tsbDelete
			// 
			this.tsbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbDelete.Image = global::VehiclePlanner.Properties.Resources.Delete;
			this.tsbDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbDelete.Name = "tsbDelete";
			this.tsbDelete.Size = new System.Drawing.Size(24, 24);
			this.tsbDelete.Text = "Delete";
			this.tsbDelete.ToolTipText = "Delete(Ctrll + D)";
			this.tsbDelete.Click += new System.EventHandler(this.tsbDelete_Click);
			// 
			// tsbSave
			// 
			this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSave.Image = global::VehiclePlanner.Properties.Resources.Save;
			this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSave.Name = "tsbSave";
			this.tsbSave.Size = new System.Drawing.Size(24, 24);
			this.tsbSave.Text = "Save";
			this.tsbSave.ToolTipText = "Save(Ctrl + S)";
			this.tsbSave.Click += new System.EventHandler(this.tsbSave_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
			// 
			// tsbRun
			// 
			this.tsbRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbRun.Image = global::VehiclePlanner.Properties.Resources.play;
			this.tsbRun.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbRun.Name = "tsbRun";
			this.tsbRun.Size = new System.Drawing.Size(24, 24);
			this.tsbRun.Text = "Run";
			this.tsbRun.ToolTipText = "Run(Ctrl + R)";
			this.tsbRun.Click += new System.EventHandler(this.tsbRun_Click);
			// 
			// tsbRunAll
			// 
			this.tsbRunAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbRunAll.Image = global::VehiclePlanner.Properties.Resources.Cycle;
			this.tsbRunAll.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbRunAll.Name = "tsbRunAll";
			this.tsbRunAll.Size = new System.Drawing.Size(24, 24);
			this.tsbRunAll.Text = "RunAll";
			this.tsbRunAll.ToolTipText = "RunAll(Ctrl + I)";
			this.tsbRunAll.Click += new System.EventHandler(this.tsbRunAll_Click);
			// 
			// tsbStop
			// 
			this.tsbStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbStop.Image = global::VehiclePlanner.Properties.Resources.NewStop;
			this.tsbStop.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbStop.Name = "tsbStop";
			this.tsbStop.Size = new System.Drawing.Size(24, 24);
			this.tsbStop.Text = "toolStripButton1";
			this.tsbStop.ToolTipText = "Stop";
			this.tsbStop.Click += new System.EventHandler(this.tsbStop_Click);
			// 
			// tsbCharging
			// 
			this.tsbCharging.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbCharging.Image = global::VehiclePlanner.Properties.Resources.Charge;
			this.tsbCharging.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbCharging.Name = "tsbCharging";
			this.tsbCharging.Size = new System.Drawing.Size(24, 24);
			this.tsbCharging.Text = "Charging";
			this.tsbCharging.ToolTipText = "Charging(Ctrl + C)";
			this.tsbCharging.Click += new System.EventHandler(this.tsbCharging_Click);
			// 
			// tsbPath
			// 
			this.tsbPath.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbPath.Image = global::VehiclePlanner.Properties.Resources.Route;
			this.tsbPath.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbPath.Name = "tsbPath";
			this.tsbPath.Size = new System.Drawing.Size(24, 24);
			this.tsbPath.Text = "Path";
			this.tsbPath.ToolTipText = "Path(Ctrl + P)";
			this.tsbPath.Click += new System.EventHandler(this.tsbPath_Click);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miPath,
            this.miRun,
            this.miCharging,
            this.toolStripSeparator2,
            this.miDelete});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(145, 106);
			// 
			// miPath
			// 
			this.miPath.Name = "miPath";
			this.miPath.Size = new System.Drawing.Size(144, 24);
			this.miPath.Text = "Path";
			this.miPath.Click += new System.EventHandler(this.miPath_Click);
			// 
			// miRun
			// 
			this.miRun.Name = "miRun";
			this.miRun.Size = new System.Drawing.Size(144, 24);
			this.miRun.Text = "Run";
			this.miRun.Click += new System.EventHandler(this.miRun_Click);
			// 
			// miCharging
			// 
			this.miCharging.Name = "miCharging";
			this.miCharging.Size = new System.Drawing.Size(144, 24);
			this.miCharging.Text = "Charging";
			this.miCharging.Click += new System.EventHandler(this.miCharging_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(141, 6);
			// 
			// miDelete
			// 
			this.miDelete.Name = "miDelete";
			this.miDelete.Size = new System.Drawing.Size(144, 24);
			this.miDelete.Text = "Delete";
			this.miDelete.Click += new System.EventHandler(this.miDelete_Click);
			// 
			// BaseGoalSetting
			// 
			this.AutoHidePortion = 206D;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(752, 776);
			this.Controls.Add(this.dgvGoalPoint);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.btnGetGoalList);
			this.Controls.Add(this.cmbGoalList);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.txtAddPtheta);
			this.Controls.Add(this.txtAddPy);
			this.Controls.Add(this.txtAddPx);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.label16);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
			this.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "BaseGoalSetting";
			this.Text = "GoalSetting";
			((System.ComponentModel.ISupportInitialize)(this.dgvGoalPoint)).EndInit();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtAddPtheta;
        protected System.Windows.Forms.TextBox txtAddPy;
        protected System.Windows.Forms.TextBox txtAddPx;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        protected System.Windows.Forms.ComboBox cmbGoalList;
        private System.Windows.Forms.Button btnGetGoalList;
        protected Inheritable.DataGridView dgvGoalPoint;
        protected System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbAddNow;
        private System.Windows.Forms.ToolStripButton tsbDelete;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbPath;
        private System.Windows.Forms.ToolStripButton tsbRun;
        private System.Windows.Forms.ToolStripButton tsbRunAll;
        private System.Windows.Forms.ToolStripButton tsbCharging;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem miPath;
        private System.Windows.Forms.ToolStripMenuItem miRun;
        private System.Windows.Forms.ToolStripMenuItem miCharging;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem miDelete;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn cID;
        private System.Windows.Forms.DataGridViewTextBoxColumn cName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cX;
        private System.Windows.Forms.DataGridViewTextBoxColumn cY;
        private System.Windows.Forms.DataGridViewTextBoxColumn cToward;
		private Bindable.ToolStripButton tsbStop;
	}
}