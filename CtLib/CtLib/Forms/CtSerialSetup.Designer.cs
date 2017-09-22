namespace CtLib.Forms {
    partial class CtSerialSetup {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtSerialSetup));
            this.lbCom = new System.Windows.Forms.Label();
            this.cbCom = new System.Windows.Forms.ComboBox();
            this.cbBaudRate = new System.Windows.Forms.ComboBox();
            this.lbBaudRate = new System.Windows.Forms.Label();
            this.gbData = new System.Windows.Forms.GroupBox();
            this.rdbData9 = new System.Windows.Forms.RadioButton();
            this.rdbData8 = new System.Windows.Forms.RadioButton();
            this.rdbData7 = new System.Windows.Forms.RadioButton();
            this.gbStop = new System.Windows.Forms.GroupBox();
            this.rdbStop2 = new System.Windows.Forms.RadioButton();
            this.rdbStop15 = new System.Windows.Forms.RadioButton();
            this.rdbStop1 = new System.Windows.Forms.RadioButton();
            this.rdbStopNone = new System.Windows.Forms.RadioButton();
            this.gbParity = new System.Windows.Forms.GroupBox();
            this.rdbParitySpace = new System.Windows.Forms.RadioButton();
            this.rdbParityMark = new System.Windows.Forms.RadioButton();
            this.rdbParityEven = new System.Windows.Forms.RadioButton();
            this.rdbParityOdd = new System.Windows.Forms.RadioButton();
            this.rdbParityNone = new System.Windows.Forms.RadioButton();
            this.gbHandshake = new System.Windows.Forms.GroupBox();
            this.rdbHskRTSX = new System.Windows.Forms.RadioButton();
            this.rdbHskRTS = new System.Windows.Forms.RadioButton();
            this.rdbHskX = new System.Windows.Forms.RadioButton();
            this.rdbHskNone = new System.Windows.Forms.RadioButton();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.gbData.SuspendLayout();
            this.gbStop.SuspendLayout();
            this.gbParity.SuspendLayout();
            this.gbHandshake.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbCom
            // 
            resources.ApplyResources(this.lbCom, "lbCom");
            this.lbCom.Name = "lbCom";
            // 
            // cbCom
            // 
            resources.ApplyResources(this.cbCom, "cbCom");
            this.cbCom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCom.FormattingEnabled = true;
            this.cbCom.Name = "cbCom";
            // 
            // cbBaudRate
            // 
            resources.ApplyResources(this.cbBaudRate, "cbBaudRate");
            this.cbBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBaudRate.FormattingEnabled = true;
            this.cbBaudRate.Items.AddRange(new object[] {
            resources.GetString("cbBaudRate.Items"),
            resources.GetString("cbBaudRate.Items1"),
            resources.GetString("cbBaudRate.Items2"),
            resources.GetString("cbBaudRate.Items3"),
            resources.GetString("cbBaudRate.Items4"),
            resources.GetString("cbBaudRate.Items5"),
            resources.GetString("cbBaudRate.Items6"),
            resources.GetString("cbBaudRate.Items7")});
            this.cbBaudRate.Name = "cbBaudRate";
            this.cbBaudRate.SelectedIndexChanged += new System.EventHandler(this.cbBaudRate_SelectedIndexChanged);
            // 
            // lbBaudRate
            // 
            resources.ApplyResources(this.lbBaudRate, "lbBaudRate");
            this.lbBaudRate.Name = "lbBaudRate";
            // 
            // gbData
            // 
            resources.ApplyResources(this.gbData, "gbData");
            this.gbData.Controls.Add(this.rdbData9);
            this.gbData.Controls.Add(this.rdbData8);
            this.gbData.Controls.Add(this.rdbData7);
            this.gbData.Name = "gbData";
            this.gbData.TabStop = false;
            // 
            // rdbData9
            // 
            resources.ApplyResources(this.rdbData9, "rdbData9");
            this.rdbData9.Name = "rdbData9";
            this.rdbData9.UseVisualStyleBackColor = true;
            this.rdbData9.CheckedChanged += new System.EventHandler(this.rdbData9_CheckedChanged);
            // 
            // rdbData8
            // 
            resources.ApplyResources(this.rdbData8, "rdbData8");
            this.rdbData8.Name = "rdbData8";
            this.rdbData8.UseVisualStyleBackColor = true;
            this.rdbData8.CheckedChanged += new System.EventHandler(this.rdbData8_CheckedChanged);
            // 
            // rdbData7
            // 
            resources.ApplyResources(this.rdbData7, "rdbData7");
            this.rdbData7.Name = "rdbData7";
            this.rdbData7.UseVisualStyleBackColor = true;
            this.rdbData7.CheckedChanged += new System.EventHandler(this.rdbData7_CheckedChanged);
            // 
            // gbStop
            // 
            resources.ApplyResources(this.gbStop, "gbStop");
            this.gbStop.Controls.Add(this.rdbStop2);
            this.gbStop.Controls.Add(this.rdbStop15);
            this.gbStop.Controls.Add(this.rdbStop1);
            this.gbStop.Controls.Add(this.rdbStopNone);
            this.gbStop.Name = "gbStop";
            this.gbStop.TabStop = false;
            // 
            // rdbStop2
            // 
            resources.ApplyResources(this.rdbStop2, "rdbStop2");
            this.rdbStop2.Name = "rdbStop2";
            this.rdbStop2.UseVisualStyleBackColor = true;
            this.rdbStop2.CheckedChanged += new System.EventHandler(this.rdbStop2_CheckedChanged);
            // 
            // rdbStop15
            // 
            resources.ApplyResources(this.rdbStop15, "rdbStop15");
            this.rdbStop15.Name = "rdbStop15";
            this.rdbStop15.UseVisualStyleBackColor = true;
            this.rdbStop15.CheckedChanged += new System.EventHandler(this.rdbStop15_CheckedChanged);
            // 
            // rdbStop1
            // 
            resources.ApplyResources(this.rdbStop1, "rdbStop1");
            this.rdbStop1.Name = "rdbStop1";
            this.rdbStop1.UseVisualStyleBackColor = true;
            this.rdbStop1.CheckedChanged += new System.EventHandler(this.rdbStop1_CheckedChanged);
            // 
            // rdbStopNone
            // 
            resources.ApplyResources(this.rdbStopNone, "rdbStopNone");
            this.rdbStopNone.Name = "rdbStopNone";
            this.rdbStopNone.UseVisualStyleBackColor = true;
            this.rdbStopNone.CheckedChanged += new System.EventHandler(this.rdbStopNone_CheckedChanged);
            // 
            // gbParity
            // 
            resources.ApplyResources(this.gbParity, "gbParity");
            this.gbParity.Controls.Add(this.rdbParitySpace);
            this.gbParity.Controls.Add(this.rdbParityMark);
            this.gbParity.Controls.Add(this.rdbParityEven);
            this.gbParity.Controls.Add(this.rdbParityOdd);
            this.gbParity.Controls.Add(this.rdbParityNone);
            this.gbParity.Name = "gbParity";
            this.gbParity.TabStop = false;
            // 
            // rdbParitySpace
            // 
            resources.ApplyResources(this.rdbParitySpace, "rdbParitySpace");
            this.rdbParitySpace.Name = "rdbParitySpace";
            this.rdbParitySpace.UseVisualStyleBackColor = true;
            this.rdbParitySpace.CheckedChanged += new System.EventHandler(this.rdbParitySpace_CheckedChanged);
            // 
            // rdbParityMark
            // 
            resources.ApplyResources(this.rdbParityMark, "rdbParityMark");
            this.rdbParityMark.Name = "rdbParityMark";
            this.rdbParityMark.UseVisualStyleBackColor = true;
            this.rdbParityMark.CheckedChanged += new System.EventHandler(this.rdbParityMark_CheckedChanged);
            // 
            // rdbParityEven
            // 
            resources.ApplyResources(this.rdbParityEven, "rdbParityEven");
            this.rdbParityEven.Name = "rdbParityEven";
            this.rdbParityEven.UseVisualStyleBackColor = true;
            this.rdbParityEven.CheckedChanged += new System.EventHandler(this.rdbParityEven_CheckedChanged);
            // 
            // rdbParityOdd
            // 
            resources.ApplyResources(this.rdbParityOdd, "rdbParityOdd");
            this.rdbParityOdd.Name = "rdbParityOdd";
            this.rdbParityOdd.UseVisualStyleBackColor = true;
            this.rdbParityOdd.CheckedChanged += new System.EventHandler(this.rdbParityOdd_CheckedChanged);
            // 
            // rdbParityNone
            // 
            resources.ApplyResources(this.rdbParityNone, "rdbParityNone");
            this.rdbParityNone.Name = "rdbParityNone";
            this.rdbParityNone.UseVisualStyleBackColor = true;
            this.rdbParityNone.CheckedChanged += new System.EventHandler(this.rdbParityNone_CheckedChanged);
            // 
            // gbHandshake
            // 
            resources.ApplyResources(this.gbHandshake, "gbHandshake");
            this.gbHandshake.Controls.Add(this.rdbHskRTSX);
            this.gbHandshake.Controls.Add(this.rdbHskRTS);
            this.gbHandshake.Controls.Add(this.rdbHskX);
            this.gbHandshake.Controls.Add(this.rdbHskNone);
            this.gbHandshake.Name = "gbHandshake";
            this.gbHandshake.TabStop = false;
            // 
            // rdbHskRTSX
            // 
            resources.ApplyResources(this.rdbHskRTSX, "rdbHskRTSX");
            this.rdbHskRTSX.Name = "rdbHskRTSX";
            this.rdbHskRTSX.UseVisualStyleBackColor = true;
            this.rdbHskRTSX.CheckedChanged += new System.EventHandler(this.rdbHskRTSX_CheckedChanged);
            // 
            // rdbHskRTS
            // 
            resources.ApplyResources(this.rdbHskRTS, "rdbHskRTS");
            this.rdbHskRTS.Name = "rdbHskRTS";
            this.rdbHskRTS.UseVisualStyleBackColor = true;
            this.rdbHskRTS.CheckedChanged += new System.EventHandler(this.rdbHskRTS_CheckedChanged);
            // 
            // rdbHskX
            // 
            resources.ApplyResources(this.rdbHskX, "rdbHskX");
            this.rdbHskX.Name = "rdbHskX";
            this.rdbHskX.UseVisualStyleBackColor = true;
            this.rdbHskX.CheckedChanged += new System.EventHandler(this.rdbHskX_CheckedChanged);
            // 
            // rdbHskNone
            // 
            resources.ApplyResources(this.rdbHskNone, "rdbHskNone");
            this.rdbHskNone.Name = "rdbHskNone";
            this.rdbHskNone.UseVisualStyleBackColor = true;
            this.rdbHskNone.CheckedChanged += new System.EventHandler(this.rdbHskNone_CheckedChanged);
            // 
            // btnConnect
            // 
            resources.ApplyResources(this.btnConnect, "btnConnect");
            this.btnConnect.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnConnect.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnConnect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
            this.btnConnect.Image = global::CtLib.Properties.Resources.Check_S;
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnExit
            // 
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnExit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
            this.btnExit.Image = global::CtLib.Properties.Resources.Exit;
            this.btnExit.Name = "btnExit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // CtSerialSetup
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ControlBox = false;
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.gbHandshake);
            this.Controls.Add(this.gbParity);
            this.Controls.Add(this.gbStop);
            this.Controls.Add(this.gbData);
            this.Controls.Add(this.cbBaudRate);
            this.Controls.Add(this.lbBaudRate);
            this.Controls.Add(this.cbCom);
            this.Controls.Add(this.lbCom);
            this.Name = "CtSerialSetup";
            this.gbData.ResumeLayout(false);
            this.gbData.PerformLayout();
            this.gbStop.ResumeLayout(false);
            this.gbStop.PerformLayout();
            this.gbParity.ResumeLayout(false);
            this.gbParity.PerformLayout();
            this.gbHandshake.ResumeLayout(false);
            this.gbHandshake.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbCom;
        private System.Windows.Forms.ComboBox cbCom;
        private System.Windows.Forms.ComboBox cbBaudRate;
        private System.Windows.Forms.Label lbBaudRate;
        private System.Windows.Forms.GroupBox gbData;
        private System.Windows.Forms.RadioButton rdbData8;
        private System.Windows.Forms.RadioButton rdbData7;
        private System.Windows.Forms.GroupBox gbStop;
        private System.Windows.Forms.RadioButton rdbStop2;
        private System.Windows.Forms.RadioButton rdbStop15;
        private System.Windows.Forms.RadioButton rdbStop1;
        private System.Windows.Forms.GroupBox gbParity;
        private System.Windows.Forms.RadioButton rdbParityEven;
        private System.Windows.Forms.RadioButton rdbParityOdd;
        private System.Windows.Forms.RadioButton rdbParityNone;
        private System.Windows.Forms.GroupBox gbHandshake;
        private System.Windows.Forms.RadioButton rdbHskRTSX;
        private System.Windows.Forms.RadioButton rdbHskRTS;
        private System.Windows.Forms.RadioButton rdbHskX;
        private System.Windows.Forms.RadioButton rdbHskNone;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.RadioButton rdbStopNone;
        private System.Windows.Forms.RadioButton rdbData9;
        private System.Windows.Forms.RadioButton rdbParitySpace;
        private System.Windows.Forms.RadioButton rdbParityMark;
    }
}