namespace VehiclePlannerUndoable.cs {
    partial class GoalSetting {
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
            this.cboSingleType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGoalPoint)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvGoalPoint
            // 
            this.dgvGoalPoint.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.dgvGoalPoint.RowTemplate.Height = 24;
            this.dgvGoalPoint.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGoalPoint_CellValueChanged);
            this.dgvGoalPoint.DoubleClick += new System.EventHandler(this.dgvGoalPoint_DoubleClick);
            // 
            // cboSingleType
            // 
            this.cboSingleType.Dock = System.Windows.Forms.DockStyle.Top;
            this.cboSingleType.Location = new System.Drawing.Point(0, 0);
            this.cboSingleType.Name = "cboSingleType";
            this.cboSingleType.Size = new System.Drawing.Size(545, 23);
            this.cboSingleType.TabIndex = 47;
            // 
            // GoalSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 776);
            this.Name = "GoalSetting";
            ((System.ComponentModel.ISupportInitialize)(this.dgvGoalPoint)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        protected System.Windows.Forms.ComboBox cboSingleType;

        #endregion
    }
}