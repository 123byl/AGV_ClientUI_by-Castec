namespace CtLib.Module.Adept {
    partial class RobotMonitor {
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboMode = new System.Windows.Forms.ComboBox();
            this.lbDelVal6 = new System.Windows.Forms.Label();
            this.lbDelVal5 = new System.Windows.Forms.Label();
            this.lbDelVal4 = new System.Windows.Forms.Label();
            this.lbDelVal3 = new System.Windows.Forms.Label();
            this.lbDelVal2 = new System.Windows.Forms.Label();
            this.lbDelVal1 = new System.Windows.Forms.Label();
            this.lbSpend = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbLocVal6 = new System.Windows.Forms.Label();
            this.lbLocVal5 = new System.Windows.Forms.Label();
            this.lbLocVal4 = new System.Windows.Forms.Label();
            this.lbLocVal3 = new System.Windows.Forms.Label();
            this.lbLocVal2 = new System.Windows.Forms.Label();
            this.lbLocVal1 = new System.Windows.Forms.Label();
            this.lbLoc6 = new System.Windows.Forms.Label();
            this.lbLoc5 = new System.Windows.Forms.Label();
            this.lbLoc4 = new System.Windows.Forms.Label();
            this.lbLoc3 = new System.Windows.Forms.Label();
            this.lbLoc2 = new System.Windows.Forms.Label();
            this.lbLoc1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkFullPackage = new System.Windows.Forms.CheckBox();
            this.btnGet = new System.Windows.Forms.Button();
            this.btnMonitor = new System.Windows.Forms.Button();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.btnSocket = new System.Windows.Forms.Button();
            this.chkControl = new System.Windows.Forms.CheckBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.cboRole = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cboTransFormat = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cboMode);
            this.groupBox1.Controls.Add(this.lbDelVal6);
            this.groupBox1.Controls.Add(this.lbDelVal5);
            this.groupBox1.Controls.Add(this.lbDelVal4);
            this.groupBox1.Controls.Add(this.lbDelVal3);
            this.groupBox1.Controls.Add(this.lbDelVal2);
            this.groupBox1.Controls.Add(this.lbDelVal1);
            this.groupBox1.Controls.Add(this.lbSpend);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lbLocVal6);
            this.groupBox1.Controls.Add(this.lbLocVal5);
            this.groupBox1.Controls.Add(this.lbLocVal4);
            this.groupBox1.Controls.Add(this.lbLocVal3);
            this.groupBox1.Controls.Add(this.lbLocVal2);
            this.groupBox1.Controls.Add(this.lbLocVal1);
            this.groupBox1.Controls.Add(this.lbLoc6);
            this.groupBox1.Controls.Add(this.lbLoc5);
            this.groupBox1.Controls.Add(this.lbLoc4);
            this.groupBox1.Controls.Add(this.lbLoc3);
            this.groupBox1.Controls.Add(this.lbLoc2);
            this.groupBox1.Controls.Add(this.lbLoc1);
            this.groupBox1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox1.Location = new System.Drawing.Point(364, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(955, 320);
            this.groupBox1.TabIndex = 68;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Monitor";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(45, 262);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 28);
            this.label5.TabIndex = 87;
            this.label5.Tag = "Spend:";
            this.label5.Text = "Mode:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboMode
            // 
            this.cboMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMode.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cboMode.FormattingEnabled = true;
            this.cboMode.Items.AddRange(new object[] {
            "World",
            "Joint"});
            this.cboMode.Location = new System.Drawing.Point(128, 262);
            this.cboMode.Name = "cboMode";
            this.cboMode.Size = new System.Drawing.Size(148, 33);
            this.cboMode.TabIndex = 86;
            this.cboMode.SelectedIndexChanged += new System.EventHandler(this.cboMode_SelectedIndexChanged);
            // 
            // lbDelVal6
            // 
            this.lbDelVal6.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDelVal6.Location = new System.Drawing.Point(824, 128);
            this.lbDelVal6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbDelVal6.Name = "lbDelVal6";
            this.lbDelVal6.Size = new System.Drawing.Size(120, 31);
            this.lbDelVal6.TabIndex = 85;
            this.lbDelVal6.Text = "500.001";
            this.lbDelVal6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbDelVal5
            // 
            this.lbDelVal5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDelVal5.Location = new System.Drawing.Point(689, 128);
            this.lbDelVal5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbDelVal5.Name = "lbDelVal5";
            this.lbDelVal5.Size = new System.Drawing.Size(120, 31);
            this.lbDelVal5.TabIndex = 84;
            this.lbDelVal5.Text = "500.001";
            this.lbDelVal5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbDelVal4
            // 
            this.lbDelVal4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDelVal4.Location = new System.Drawing.Point(555, 128);
            this.lbDelVal4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbDelVal4.Name = "lbDelVal4";
            this.lbDelVal4.Size = new System.Drawing.Size(120, 31);
            this.lbDelVal4.TabIndex = 83;
            this.lbDelVal4.Text = "500.001";
            this.lbDelVal4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbDelVal3
            // 
            this.lbDelVal3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDelVal3.Location = new System.Drawing.Point(420, 128);
            this.lbDelVal3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbDelVal3.Name = "lbDelVal3";
            this.lbDelVal3.Size = new System.Drawing.Size(120, 31);
            this.lbDelVal3.TabIndex = 82;
            this.lbDelVal3.Text = "500.001";
            this.lbDelVal3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbDelVal2
            // 
            this.lbDelVal2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDelVal2.Location = new System.Drawing.Point(285, 128);
            this.lbDelVal2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbDelVal2.Name = "lbDelVal2";
            this.lbDelVal2.Size = new System.Drawing.Size(120, 31);
            this.lbDelVal2.TabIndex = 81;
            this.lbDelVal2.Text = "500.001";
            this.lbDelVal2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbDelVal1
            // 
            this.lbDelVal1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDelVal1.Location = new System.Drawing.Point(151, 128);
            this.lbDelVal1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbDelVal1.Name = "lbDelVal1";
            this.lbDelVal1.Size = new System.Drawing.Size(120, 31);
            this.lbDelVal1.TabIndex = 80;
            this.lbDelVal1.Text = "500.001";
            this.lbDelVal1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbSpend
            // 
            this.lbSpend.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSpend.Location = new System.Drawing.Point(45, 176);
            this.lbSpend.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbSpend.Name = "lbSpend";
            this.lbSpend.Size = new System.Drawing.Size(156, 31);
            this.lbSpend.TabIndex = 79;
            this.lbSpend.Tag = "Spend:";
            this.lbSpend.Text = "Spend:";
            this.lbSpend.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(23, 128);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 31);
            this.label2.TabIndex = 78;
            this.label2.Text = "Detal";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(23, 78);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 31);
            this.label1.TabIndex = 77;
            this.label1.Text = "Location";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLocVal6
            // 
            this.lbLocVal6.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLocVal6.Location = new System.Drawing.Point(824, 78);
            this.lbLocVal6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbLocVal6.Name = "lbLocVal6";
            this.lbLocVal6.Size = new System.Drawing.Size(120, 31);
            this.lbLocVal6.TabIndex = 76;
            this.lbLocVal6.Text = "500.001";
            this.lbLocVal6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLocVal5
            // 
            this.lbLocVal5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLocVal5.Location = new System.Drawing.Point(689, 78);
            this.lbLocVal5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbLocVal5.Name = "lbLocVal5";
            this.lbLocVal5.Size = new System.Drawing.Size(120, 31);
            this.lbLocVal5.TabIndex = 75;
            this.lbLocVal5.Text = "500.001";
            this.lbLocVal5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLocVal4
            // 
            this.lbLocVal4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLocVal4.Location = new System.Drawing.Point(555, 78);
            this.lbLocVal4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbLocVal4.Name = "lbLocVal4";
            this.lbLocVal4.Size = new System.Drawing.Size(120, 31);
            this.lbLocVal4.TabIndex = 74;
            this.lbLocVal4.Text = "500.001";
            this.lbLocVal4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLocVal3
            // 
            this.lbLocVal3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLocVal3.Location = new System.Drawing.Point(420, 78);
            this.lbLocVal3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbLocVal3.Name = "lbLocVal3";
            this.lbLocVal3.Size = new System.Drawing.Size(120, 31);
            this.lbLocVal3.TabIndex = 73;
            this.lbLocVal3.Text = "500.001";
            this.lbLocVal3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLocVal2
            // 
            this.lbLocVal2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLocVal2.Location = new System.Drawing.Point(285, 78);
            this.lbLocVal2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbLocVal2.Name = "lbLocVal2";
            this.lbLocVal2.Size = new System.Drawing.Size(120, 31);
            this.lbLocVal2.TabIndex = 72;
            this.lbLocVal2.Text = "500.001";
            this.lbLocVal2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLocVal1
            // 
            this.lbLocVal1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLocVal1.Location = new System.Drawing.Point(151, 78);
            this.lbLocVal1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbLocVal1.Name = "lbLocVal1";
            this.lbLocVal1.Size = new System.Drawing.Size(120, 31);
            this.lbLocVal1.TabIndex = 71;
            this.lbLocVal1.Text = "500.001";
            this.lbLocVal1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLoc6
            // 
            this.lbLoc6.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLoc6.Location = new System.Drawing.Point(824, 38);
            this.lbLoc6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbLoc6.Name = "lbLoc6";
            this.lbLoc6.Size = new System.Drawing.Size(120, 31);
            this.lbLoc6.TabIndex = 70;
            this.lbLoc6.Text = "Roll";
            this.lbLoc6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLoc5
            // 
            this.lbLoc5.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLoc5.Location = new System.Drawing.Point(689, 38);
            this.lbLoc5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbLoc5.Name = "lbLoc5";
            this.lbLoc5.Size = new System.Drawing.Size(120, 31);
            this.lbLoc5.TabIndex = 69;
            this.lbLoc5.Text = "Pitch";
            this.lbLoc5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLoc4
            // 
            this.lbLoc4.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLoc4.Location = new System.Drawing.Point(555, 38);
            this.lbLoc4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbLoc4.Name = "lbLoc4";
            this.lbLoc4.Size = new System.Drawing.Size(120, 31);
            this.lbLoc4.TabIndex = 68;
            this.lbLoc4.Text = "Yaw";
            this.lbLoc4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLoc3
            // 
            this.lbLoc3.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLoc3.Location = new System.Drawing.Point(420, 38);
            this.lbLoc3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbLoc3.Name = "lbLoc3";
            this.lbLoc3.Size = new System.Drawing.Size(120, 31);
            this.lbLoc3.TabIndex = 67;
            this.lbLoc3.Text = "Z";
            this.lbLoc3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLoc2
            // 
            this.lbLoc2.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLoc2.Location = new System.Drawing.Point(285, 38);
            this.lbLoc2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbLoc2.Name = "lbLoc2";
            this.lbLoc2.Size = new System.Drawing.Size(120, 31);
            this.lbLoc2.TabIndex = 66;
            this.lbLoc2.Text = "Y";
            this.lbLoc2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLoc1
            // 
            this.lbLoc1.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLoc1.Location = new System.Drawing.Point(151, 38);
            this.lbLoc1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbLoc1.Name = "lbLoc1";
            this.lbLoc1.Size = new System.Drawing.Size(120, 31);
            this.lbLoc1.TabIndex = 65;
            this.lbLoc1.Text = "X";
            this.lbLoc1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.cboTransFormat);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.cboRole);
            this.groupBox2.Controls.Add(this.chkFullPackage);
            this.groupBox2.Controls.Add(this.btnGet);
            this.groupBox2.Controls.Add(this.btnMonitor);
            this.groupBox2.Controls.Add(this.txtPort);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtIP);
            this.groupBox2.Controls.Add(this.btnSocket);
            this.groupBox2.Controls.Add(this.chkControl);
            this.groupBox2.Controls.Add(this.btnConnect);
            this.groupBox2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(346, 463);
            this.groupBox2.TabIndex = 69;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Control";
            // 
            // chkFullPackage
            // 
            this.chkFullPackage.AutoSize = true;
            this.chkFullPackage.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chkFullPackage.Location = new System.Drawing.Point(30, 425);
            this.chkFullPackage.Name = "chkFullPackage";
            this.chkFullPackage.Size = new System.Drawing.Size(152, 29);
            this.chkFullPackage.TabIndex = 78;
            this.chkFullPackage.Text = "Full package";
            this.chkFullPackage.UseVisualStyleBackColor = true;
            this.chkFullPackage.CheckedChanged += new System.EventHandler(this.chkFullPackage_CheckedChanged);
            // 
            // btnGet
            // 
            this.btnGet.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGet.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGet.Location = new System.Drawing.Point(239, 313);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(88, 63);
            this.btnGet.TabIndex = 77;
            this.btnGet.Text = "Get";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler(this.btnGet_Click);
            // 
            // btnMonitor
            // 
            this.btnMonitor.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnMonitor.Image = global::CtLib.Properties.Resources.Grey_Ball;
            this.btnMonitor.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMonitor.Location = new System.Drawing.Point(29, 313);
            this.btnMonitor.Name = "btnMonitor";
            this.btnMonitor.Size = new System.Drawing.Size(203, 63);
            this.btnMonitor.TabIndex = 70;
            this.btnMonitor.Text = "Start Monitor";
            this.btnMonitor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnMonitor.UseVisualStyleBackColor = true;
            this.btnMonitor.Click += new System.EventHandler(this.btnMonitor_Click);
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(239, 112);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(88, 34);
            this.txtPort.TabIndex = 74;
            this.txtPort.Text = "2909";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(176, 115);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 25);
            this.label4.TabIndex = 73;
            this.label4.Text = "Port:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(24, 115);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 25);
            this.label3.TabIndex = 72;
            this.label3.Text = "IP:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(66, 112);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(103, 34);
            this.txtIP.TabIndex = 70;
            this.txtIP.Text = "127.0.0.1";
            // 
            // btnSocket
            // 
            this.btnSocket.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSocket.Image = global::CtLib.Properties.Resources.Grey_Ball;
            this.btnSocket.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSocket.Location = new System.Drawing.Point(29, 160);
            this.btnSocket.Name = "btnSocket";
            this.btnSocket.Size = new System.Drawing.Size(298, 63);
            this.btnSocket.TabIndex = 71;
            this.btnSocket.Text = "Connect  to Server  ";
            this.btnSocket.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSocket.UseVisualStyleBackColor = true;
            this.btnSocket.Click += new System.EventHandler(this.btnSocket_Click);
            // 
            // chkControl
            // 
            this.chkControl.AutoSize = true;
            this.chkControl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chkControl.Location = new System.Drawing.Point(30, 390);
            this.chkControl.Name = "chkControl";
            this.chkControl.Size = new System.Drawing.Size(176, 29);
            this.chkControl.TabIndex = 70;
            this.chkControl.Text = "With Controller";
            this.chkControl.UseVisualStyleBackColor = true;
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnConnect.Image = global::CtLib.Properties.Resources.Grey_Ball;
            this.btnConnect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConnect.Location = new System.Drawing.Point(29, 235);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(298, 63);
            this.btnConnect.TabIndex = 68;
            this.btnConnect.Text = "Connect  To ACE  ";
            this.btnConnect.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // cboRole
            // 
            this.cboRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRole.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cboRole.FormattingEnabled = true;
            this.cboRole.Items.AddRange(new object[] {
            "Server",
            "Client"});
            this.cboRole.Location = new System.Drawing.Point(92, 34);
            this.cboRole.Name = "cboRole";
            this.cboRole.Size = new System.Drawing.Size(148, 33);
            this.cboRole.TabIndex = 87;
            this.cboRole.SelectedIndexChanged += new System.EventHandler(this.cboRole_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(20, 36);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 28);
            this.label6.TabIndex = 88;
            this.label6.Tag = "Spend:";
            this.label6.Text = "Role:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(20, 75);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(151, 28);
            this.label7.TabIndex = 90;
            this.label7.Tag = "Spend:";
            this.label7.Text = "Trans format:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboTransFormat
            // 
            this.cboTransFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTransFormat.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cboTransFormat.FormattingEnabled = true;
            this.cboTransFormat.Items.AddRange(new object[] {
            "Strign",
            "Byte"});
            this.cboTransFormat.Location = new System.Drawing.Point(178, 73);
            this.cboTransFormat.Name = "cboTransFormat";
            this.cboTransFormat.Size = new System.Drawing.Size(125, 33);
            this.cboTransFormat.TabIndex = 89;
            this.cboTransFormat.SelectedIndexChanged += new System.EventHandler(this.cboTransFormat_SelectedIndexChanged);
            // 
            // RobotMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1364, 499);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "RobotMonitor";
            this.Text = "RobotMonitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RobotMonitor_FormClosing);
            this.Load += new System.EventHandler(this.RobotMonitor_Load);
            this.Shown += new System.EventHandler(this.RobotMonitor_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbDelVal6;
        private System.Windows.Forms.Label lbDelVal5;
        private System.Windows.Forms.Label lbDelVal4;
        private System.Windows.Forms.Label lbDelVal3;
        private System.Windows.Forms.Label lbDelVal2;
        private System.Windows.Forms.Label lbDelVal1;
        private System.Windows.Forms.Label lbSpend;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbLocVal6;
        private System.Windows.Forms.Label lbLocVal5;
        private System.Windows.Forms.Label lbLocVal4;
        private System.Windows.Forms.Label lbLocVal3;
        private System.Windows.Forms.Label lbLocVal2;
        private System.Windows.Forms.Label lbLocVal1;
        private System.Windows.Forms.Label lbLoc6;
        private System.Windows.Forms.Label lbLoc5;
        private System.Windows.Forms.Label lbLoc4;
        private System.Windows.Forms.Label lbLoc3;
        private System.Windows.Forms.Label lbLoc2;
        private System.Windows.Forms.Label lbLoc1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkControl;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ComboBox cboMode;
        private System.Windows.Forms.Button btnSocket;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkFullPackage;
        private System.Windows.Forms.Button btnGet;
        private System.Windows.Forms.Button btnMonitor;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cboTransFormat;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboRole;
    }
}