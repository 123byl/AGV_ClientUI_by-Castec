namespace CtLib.Forms.TestPlatform
{
    partial class Test_FestoCMMO
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
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
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lbActualCurrent = new System.Windows.Forms.Label();
            this.btnJog1 = new System.Windows.Forms.Button();
            this.btnJog0 = new System.Windows.Forms.Button();
            this.lbMasterControl = new System.Windows.Forms.Label();
            this.textBoxMasterControl = new System.Windows.Forms.TextBox();
            this.btnEnable = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.lbActualTorque = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lbTargetPosition = new System.Windows.Forms.Label();
            this.lbTargetPositionUnit = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbxMeasurementUnit = new System.Windows.Forms.ComboBox();
            this.lbMeasurementUnit = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbOperationMode = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lbSWbit15 = new System.Windows.Forms.Label();
            this.lbWarningMsg = new System.Windows.Forms.Label();
            this.lbErrorMsg = new System.Windows.Forms.Label();
            this.lbSWbit7 = new System.Windows.Forms.Label();
            this.lbSWbit3 = new System.Windows.Forms.Label();
            this.lbSWbit10 = new System.Windows.Forms.Label();
            this.lbSWbit1 = new System.Windows.Forms.Label();
            this.lbSTObit = new System.Windows.Forms.Label();
            this.lbSWbit2 = new System.Windows.Forms.Label();
            this.lbActualVelocityUnit = new System.Windows.Forms.Label();
            this.lbActualVelocity = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbActualPositionUnit = new System.Windows.Forms.Label();
            this.lbActualPosition = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnHoming = new System.Windows.Forms.Button();
            this.textBoxIncrement = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnStep1 = new System.Windows.Forms.Button();
            this.lbVelocityUnit = new System.Windows.Forms.Label();
            this.textBoxVelocity = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.lbIncrementUnit = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.btnStep0 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataGridViewRecordTable = new System.Windows.Forms.DataGridView();
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.TargetX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TargetY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Velocity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Acceleration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Force = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnGetRecordTable = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnSaveRecordTable = new System.Windows.Forms.Button();
            this.btnStopMotion = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRecordTable)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Port :";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(49, 18);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(100, 22);
            this.textBoxIP.TabIndex = 2;
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(49, 43);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(100, 22);
            this.textBoxPort.TabIndex = 3;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(165, 18);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(135, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "Motor Current :";
            // 
            // lbActualCurrent
            // 
            this.lbActualCurrent.AutoSize = true;
            this.lbActualCurrent.Location = new System.Drawing.Point(233, 18);
            this.lbActualCurrent.Name = "lbActualCurrent";
            this.lbActualCurrent.Size = new System.Drawing.Size(0, 12);
            this.lbActualCurrent.TabIndex = 10;
            // 
            // btnJog1
            // 
            this.btnJog1.Enabled = false;
            this.btnJog1.Location = new System.Drawing.Point(433, 177);
            this.btnJog1.Name = "btnJog1";
            this.btnJog1.Size = new System.Drawing.Size(75, 23);
            this.btnJog1.TabIndex = 11;
            this.btnJog1.Text = "Jog+ >>";
            this.btnJog1.UseVisualStyleBackColor = true;
            this.btnJog1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnJog1_MouseDown);
            this.btnJog1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnJog1_MouseUp);
            // 
            // btnJog0
            // 
            this.btnJog0.Enabled = false;
            this.btnJog0.Location = new System.Drawing.Point(355, 177);
            this.btnJog0.Name = "btnJog0";
            this.btnJog0.Size = new System.Drawing.Size(75, 23);
            this.btnJog0.TabIndex = 12;
            this.btnJog0.Text = "Jog- <<";
            this.btnJog0.UseVisualStyleBackColor = true;
            this.btnJog0.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnJog0_MouseDown);
            this.btnJog0.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnJog0_MouseUp);
            // 
            // lbMasterControl
            // 
            this.lbMasterControl.AutoSize = true;
            this.lbMasterControl.Enabled = false;
            this.lbMasterControl.Location = new System.Drawing.Point(15, 23);
            this.lbMasterControl.Name = "lbMasterControl";
            this.lbMasterControl.Size = new System.Drawing.Size(78, 12);
            this.lbMasterControl.TabIndex = 13;
            this.lbMasterControl.Text = "MasterControl :";
            // 
            // textBoxMasterControl
            // 
            this.textBoxMasterControl.Enabled = false;
            this.textBoxMasterControl.Location = new System.Drawing.Point(98, 18);
            this.textBoxMasterControl.Name = "textBoxMasterControl";
            this.textBoxMasterControl.Size = new System.Drawing.Size(31, 22);
            this.textBoxMasterControl.TabIndex = 14;
            // 
            // btnEnable
            // 
            this.btnEnable.Enabled = false;
            this.btnEnable.Location = new System.Drawing.Point(145, 18);
            this.btnEnable.Name = "btnEnable";
            this.btnEnable.Size = new System.Drawing.Size(60, 22);
            this.btnEnable.TabIndex = 15;
            this.btnEnable.Text = "Enable";
            this.btnEnable.UseVisualStyleBackColor = true;
            this.btnEnable.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(135, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "Actual torque :";
            // 
            // lbActualTorque
            // 
            this.lbActualTorque.AutoSize = true;
            this.lbActualTorque.Location = new System.Drawing.Point(233, 42);
            this.lbActualTorque.Name = "lbActualTorque";
            this.lbActualTorque.Size = new System.Drawing.Size(0, 12);
            this.lbActualTorque.TabIndex = 17;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(280, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(13, 12);
            this.label6.TabIndex = 18;
            this.label6.Text = "A";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(280, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 12);
            this.label7.TabIndex = 19;
            this.label7.Text = "%";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(135, 90);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 12);
            this.label8.TabIndex = 20;
            this.label8.Text = "Target Position :";
            // 
            // lbTargetPosition
            // 
            this.lbTargetPosition.AutoSize = true;
            this.lbTargetPosition.Location = new System.Drawing.Point(234, 90);
            this.lbTargetPosition.Name = "lbTargetPosition";
            this.lbTargetPosition.Size = new System.Drawing.Size(0, 12);
            this.lbTargetPosition.TabIndex = 21;
            // 
            // lbTargetPositionUnit
            // 
            this.lbTargetPositionUnit.AutoSize = true;
            this.lbTargetPositionUnit.Location = new System.Drawing.Point(280, 90);
            this.lbTargetPositionUnit.Name = "lbTargetPositionUnit";
            this.lbTargetPositionUnit.Size = new System.Drawing.Size(0, 12);
            this.lbTargetPositionUnit.TabIndex = 22;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxIP);
            this.groupBox1.Controls.Add(this.textBoxPort);
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Location = new System.Drawing.Point(27, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(321, 74);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Comunication";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbxMeasurementUnit);
            this.groupBox2.Controls.Add(this.lbMeasurementUnit);
            this.groupBox2.Controls.Add(this.lbMasterControl);
            this.groupBox2.Controls.Add(this.textBoxMasterControl);
            this.groupBox2.Controls.Add(this.btnEnable);
            this.groupBox2.Location = new System.Drawing.Point(354, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(312, 74);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Device Control";
            // 
            // cbxMeasurementUnit
            // 
            this.cbxMeasurementUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxMeasurementUnit.Enabled = false;
            this.cbxMeasurementUnit.FormattingEnabled = true;
            this.cbxMeasurementUnit.Location = new System.Drawing.Point(145, 43);
            this.cbxMeasurementUnit.Name = "cbxMeasurementUnit";
            this.cbxMeasurementUnit.Size = new System.Drawing.Size(121, 20);
            this.cbxMeasurementUnit.TabIndex = 17;
            this.cbxMeasurementUnit.SelectedIndexChanged += new System.EventHandler(this.cbxMeasurementUnit_SelectedIndexChanged);
            // 
            // lbMeasurementUnit
            // 
            this.lbMeasurementUnit.AutoSize = true;
            this.lbMeasurementUnit.Enabled = false;
            this.lbMeasurementUnit.Location = new System.Drawing.Point(15, 46);
            this.lbMeasurementUnit.Name = "lbMeasurementUnit";
            this.lbMeasurementUnit.Size = new System.Drawing.Size(96, 12);
            this.lbMeasurementUnit.TabIndex = 16;
            this.lbMeasurementUnit.Text = "Measurement Unit :";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbOperationMode);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.lbSWbit15);
            this.groupBox3.Controls.Add(this.lbWarningMsg);
            this.groupBox3.Controls.Add(this.lbErrorMsg);
            this.groupBox3.Controls.Add(this.lbSWbit7);
            this.groupBox3.Controls.Add(this.lbSWbit3);
            this.groupBox3.Controls.Add(this.lbSWbit10);
            this.groupBox3.Controls.Add(this.lbSWbit1);
            this.groupBox3.Controls.Add(this.lbSTObit);
            this.groupBox3.Controls.Add(this.lbSWbit2);
            this.groupBox3.Controls.Add(this.lbActualVelocityUnit);
            this.groupBox3.Controls.Add(this.lbActualVelocity);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.lbActualPositionUnit);
            this.groupBox3.Controls.Add(this.lbActualPosition);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.lbActualCurrent);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.lbTargetPositionUnit);
            this.groupBox3.Controls.Add(this.lbActualTorque);
            this.groupBox3.Controls.Add(this.lbTargetPosition);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(28, 92);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(320, 187);
            this.groupBox3.TabIndex = 25;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Dynamic Data";
            // 
            // lbOperationMode
            // 
            this.lbOperationMode.AutoSize = true;
            this.lbOperationMode.Location = new System.Drawing.Point(233, 157);
            this.lbOperationMode.Name = "lbOperationMode";
            this.lbOperationMode.Size = new System.Drawing.Size(0, 12);
            this.lbOperationMode.TabIndex = 43;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(135, 157);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(87, 12);
            this.label17.TabIndex = 42;
            this.label17.Text = "Operation Mode :";
            // 
            // lbSWbit15
            // 
            this.lbSWbit15.Image = global::CtLib.Properties.Resources.Grey_Ball;
            this.lbSWbit15.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbSWbit15.Location = new System.Drawing.Point(11, 112);
            this.lbSWbit15.Name = "lbSWbit15";
            this.lbSWbit15.Size = new System.Drawing.Size(90, 12);
            this.lbSWbit15.TabIndex = 41;
            this.lbSWbit15.Text = "Homing Valid";
            this.lbSWbit15.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbWarningMsg
            // 
            this.lbWarningMsg.AutoSize = true;
            this.lbWarningMsg.Location = new System.Drawing.Point(101, 157);
            this.lbWarningMsg.Name = "lbWarningMsg";
            this.lbWarningMsg.Size = new System.Drawing.Size(0, 12);
            this.lbWarningMsg.TabIndex = 40;
            // 
            // lbErrorMsg
            // 
            this.lbErrorMsg.AutoSize = true;
            this.lbErrorMsg.Location = new System.Drawing.Point(101, 135);
            this.lbErrorMsg.Name = "lbErrorMsg";
            this.lbErrorMsg.Size = new System.Drawing.Size(0, 12);
            this.lbErrorMsg.TabIndex = 39;
            // 
            // lbSWbit7
            // 
            this.lbSWbit7.Image = global::CtLib.Properties.Resources.Grey_Ball;
            this.lbSWbit7.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbSWbit7.Location = new System.Drawing.Point(11, 157);
            this.lbSWbit7.Name = "lbSWbit7";
            this.lbSWbit7.Size = new System.Drawing.Size(64, 12);
            this.lbSWbit7.TabIndex = 38;
            this.lbSWbit7.Text = "Warning";
            this.lbSWbit7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbSWbit3
            // 
            this.lbSWbit3.Image = global::CtLib.Properties.Resources.Grey_Ball;
            this.lbSWbit3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbSWbit3.Location = new System.Drawing.Point(11, 135);
            this.lbSWbit3.Name = "lbSWbit3";
            this.lbSWbit3.Size = new System.Drawing.Size(49, 12);
            this.lbSWbit3.TabIndex = 37;
            this.lbSWbit3.Text = "Error";
            this.lbSWbit3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbSWbit10
            // 
            this.lbSWbit10.Image = global::CtLib.Properties.Resources.Grey_Ball;
            this.lbSWbit10.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbSWbit10.Location = new System.Drawing.Point(11, 89);
            this.lbSWbit10.Name = "lbSWbit10";
            this.lbSWbit10.Size = new System.Drawing.Size(42, 12);
            this.lbSWbit10.TabIndex = 36;
            this.lbSWbit10.Text = "MC";
            this.lbSWbit10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbSWbit1
            // 
            this.lbSWbit1.Image = global::CtLib.Properties.Resources.Grey_Ball;
            this.lbSWbit1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbSWbit1.Location = new System.Drawing.Point(11, 65);
            this.lbSWbit1.Name = "lbSWbit1";
            this.lbSWbit1.Size = new System.Drawing.Size(54, 12);
            this.lbSWbit1.TabIndex = 35;
            this.lbSWbit1.Text = "Ready";
            this.lbSWbit1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbSTObit
            // 
            this.lbSTObit.Image = global::CtLib.Properties.Resources.Grey_Ball;
            this.lbSTObit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbSTObit.Location = new System.Drawing.Point(11, 42);
            this.lbSTObit.Name = "lbSTObit";
            this.lbSTObit.Size = new System.Drawing.Size(45, 12);
            this.lbSTObit.TabIndex = 34;
            this.lbSTObit.Text = "STO";
            this.lbSTObit.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbSWbit2
            // 
            this.lbSWbit2.Image = global::CtLib.Properties.Resources.Grey_Ball;
            this.lbSWbit2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbSWbit2.Location = new System.Drawing.Point(11, 18);
            this.lbSWbit2.Name = "lbSWbit2";
            this.lbSWbit2.Size = new System.Drawing.Size(55, 12);
            this.lbSWbit2.TabIndex = 33;
            this.lbSWbit2.Text = "Enable";
            this.lbSWbit2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbActualVelocityUnit
            // 
            this.lbActualVelocityUnit.AutoSize = true;
            this.lbActualVelocityUnit.Location = new System.Drawing.Point(280, 65);
            this.lbActualVelocityUnit.Name = "lbActualVelocityUnit";
            this.lbActualVelocityUnit.Size = new System.Drawing.Size(0, 12);
            this.lbActualVelocityUnit.TabIndex = 32;
            // 
            // lbActualVelocity
            // 
            this.lbActualVelocity.AutoSize = true;
            this.lbActualVelocity.Location = new System.Drawing.Point(234, 65);
            this.lbActualVelocity.Name = "lbActualVelocity";
            this.lbActualVelocity.Size = new System.Drawing.Size(0, 12);
            this.lbActualVelocity.TabIndex = 31;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(135, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 12);
            this.label4.TabIndex = 30;
            this.label4.Text = "Actual Velocity :";
            // 
            // lbActualPositionUnit
            // 
            this.lbActualPositionUnit.AutoSize = true;
            this.lbActualPositionUnit.Location = new System.Drawing.Point(280, 112);
            this.lbActualPositionUnit.Name = "lbActualPositionUnit";
            this.lbActualPositionUnit.Size = new System.Drawing.Size(0, 12);
            this.lbActualPositionUnit.TabIndex = 29;
            // 
            // lbActualPosition
            // 
            this.lbActualPosition.AutoSize = true;
            this.lbActualPosition.Location = new System.Drawing.Point(234, 111);
            this.lbActualPosition.Name = "lbActualPosition";
            this.lbActualPosition.Size = new System.Drawing.Size(0, 12);
            this.lbActualPosition.TabIndex = 28;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(135, 112);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(81, 12);
            this.label9.TabIndex = 27;
            this.label9.Text = "Actual Position :";
            // 
            // btnHoming
            // 
            this.btnHoming.Enabled = false;
            this.btnHoming.Location = new System.Drawing.Point(514, 177);
            this.btnHoming.Name = "btnHoming";
            this.btnHoming.Size = new System.Drawing.Size(74, 23);
            this.btnHoming.TabIndex = 26;
            this.btnHoming.Text = "Homing";
            this.btnHoming.UseVisualStyleBackColor = true;
            this.btnHoming.Click += new System.EventHandler(this.btnHoming_Click);
            // 
            // textBoxIncrement
            // 
            this.textBoxIncrement.Enabled = false;
            this.textBoxIncrement.Location = new System.Drawing.Point(97, 18);
            this.textBoxIncrement.Name = "textBoxIncrement";
            this.textBoxIncrement.Size = new System.Drawing.Size(69, 22);
            this.textBoxIncrement.TabIndex = 27;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnStep1);
            this.groupBox4.Controls.Add(this.lbVelocityUnit);
            this.groupBox4.Controls.Add(this.textBoxVelocity);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.lbIncrementUnit);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.textBoxIncrement);
            this.groupBox4.Controls.Add(this.btnStep0);
            this.groupBox4.Location = new System.Drawing.Point(355, 92);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(311, 74);
            this.groupBox4.TabIndex = 29;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Movement Data";
            // 
            // btnStep1
            // 
            this.btnStep1.Enabled = false;
            this.btnStep1.Location = new System.Drawing.Point(215, 44);
            this.btnStep1.Name = "btnStep1";
            this.btnStep1.Size = new System.Drawing.Size(75, 23);
            this.btnStep1.TabIndex = 36;
            this.btnStep1.Text = "Step+ >";
            this.btnStep1.UseVisualStyleBackColor = true;
            this.btnStep1.Click += new System.EventHandler(this.btnStep1_Click);
            // 
            // lbVelocityUnit
            // 
            this.lbVelocityUnit.AutoSize = true;
            this.lbVelocityUnit.Location = new System.Drawing.Point(172, 49);
            this.lbVelocityUnit.Name = "lbVelocityUnit";
            this.lbVelocityUnit.Size = new System.Drawing.Size(0, 12);
            this.lbVelocityUnit.TabIndex = 33;
            // 
            // textBoxVelocity
            // 
            this.textBoxVelocity.Enabled = false;
            this.textBoxVelocity.Location = new System.Drawing.Point(97, 46);
            this.textBoxVelocity.Name = "textBoxVelocity";
            this.textBoxVelocity.Size = new System.Drawing.Size(69, 22);
            this.textBoxVelocity.TabIndex = 35;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(14, 49);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(50, 12);
            this.label15.TabIndex = 34;
            this.label15.Text = "Velocity :";
            // 
            // lbIncrementUnit
            // 
            this.lbIncrementUnit.AutoSize = true;
            this.lbIncrementUnit.Location = new System.Drawing.Point(172, 21);
            this.lbIncrementUnit.Name = "lbIncrementUnit";
            this.lbIncrementUnit.Size = new System.Drawing.Size(0, 12);
            this.lbIncrementUnit.TabIndex = 33;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 21);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 12);
            this.label11.TabIndex = 16;
            this.label11.Text = "Increment :";
            // 
            // btnStep0
            // 
            this.btnStep0.Enabled = false;
            this.btnStep0.Location = new System.Drawing.Point(215, 16);
            this.btnStep0.Name = "btnStep0";
            this.btnStep0.Size = new System.Drawing.Size(75, 23);
            this.btnStep0.TabIndex = 30;
            this.btnStep0.Text = "Step- <";
            this.btnStep0.UseVisualStyleBackColor = true;
            this.btnStep0.Click += new System.EventHandler(this.btnStep0_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(32, 294);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(515, 201);
            this.tabControl1.TabIndex = 31;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridViewRecordTable);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(507, 175);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Record Table";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridViewRecordTable
            // 
            this.dataGridViewRecordTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRecordTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.No,
            this.Type,
            this.TargetX,
            this.TargetY,
            this.Velocity,
            this.Acceleration,
            this.Force});
            this.dataGridViewRecordTable.Location = new System.Drawing.Point(5, 6);
            this.dataGridViewRecordTable.Name = "dataGridViewRecordTable";
            this.dataGridViewRecordTable.RowTemplate.Height = 24;
            this.dataGridViewRecordTable.Size = new System.Drawing.Size(494, 163);
            this.dataGridViewRecordTable.TabIndex = 0;
            this.dataGridViewRecordTable.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRecordTable_CellClick);
            // 
            // No
            // 
            this.No.Frozen = true;
            this.No.HeaderText = "No";
            this.No.Name = "No";
            this.No.Width = 30;
            // 
            // Type
            // 
            this.Type.Frozen = true;
            this.Type.HeaderText = "Type";
            this.Type.Items.AddRange(new object[] {
            "Inactive",
            "PA",
            "PRN"});
            this.Type.Name = "Type";
            this.Type.Width = 65;
            // 
            // TargetX
            // 
            this.TargetX.Frozen = true;
            this.TargetX.HeaderText = "TargetX";
            this.TargetX.Name = "TargetX";
            this.TargetX.Width = 65;
            // 
            // TargetY
            // 
            this.TargetY.Frozen = true;
            this.TargetY.HeaderText = "TargetY";
            this.TargetY.Name = "TargetY";
            this.TargetY.ReadOnly = true;
            this.TargetY.Visible = false;
            this.TargetY.Width = 65;
            // 
            // Velocity
            // 
            this.Velocity.Frozen = true;
            this.Velocity.HeaderText = "Velocity";
            this.Velocity.Name = "Velocity";
            this.Velocity.Width = 65;
            // 
            // Acceleration
            // 
            this.Acceleration.Frozen = true;
            this.Acceleration.HeaderText = "Acceleration";
            this.Acceleration.Name = "Acceleration";
            this.Acceleration.Width = 70;
            // 
            // Force
            // 
            this.Force.HeaderText = "Force";
            this.Force.Name = "Force";
            this.Force.Width = 70;
            // 
            // btnGetRecordTable
            // 
            this.btnGetRecordTable.Enabled = false;
            this.btnGetRecordTable.Location = new System.Drawing.Point(570, 316);
            this.btnGetRecordTable.Name = "btnGetRecordTable";
            this.btnGetRecordTable.Size = new System.Drawing.Size(96, 23);
            this.btnGetRecordTable.TabIndex = 32;
            this.btnGetRecordTable.Text = "Get Record Table";
            this.btnGetRecordTable.UseVisualStyleBackColor = true;
            this.btnGetRecordTable.Click += new System.EventHandler(this.btnGetRecordTable_Click);
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(570, 345);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(96, 23);
            this.btnStart.TabIndex = 33;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnReset
            // 
            this.btnReset.Enabled = false;
            this.btnReset.Location = new System.Drawing.Point(355, 206);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(153, 23);
            this.btnReset.TabIndex = 34;
            this.btnReset.Text = "Acknowledge error";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnSaveRecordTable
            // 
            this.btnSaveRecordTable.Enabled = false;
            this.btnSaveRecordTable.Location = new System.Drawing.Point(570, 450);
            this.btnSaveRecordTable.Name = "btnSaveRecordTable";
            this.btnSaveRecordTable.Size = new System.Drawing.Size(96, 35);
            this.btnSaveRecordTable.TabIndex = 35;
            this.btnSaveRecordTable.Text = "Save Record Table";
            this.btnSaveRecordTable.UseVisualStyleBackColor = true;
            this.btnSaveRecordTable.Click += new System.EventHandler(this.btnSaveRecordTable_Click);
            // 
            // btnStopMotion
            // 
            this.btnStopMotion.Enabled = false;
            this.btnStopMotion.Location = new System.Drawing.Point(514, 206);
            this.btnStopMotion.Name = "btnStopMotion";
            this.btnStopMotion.Size = new System.Drawing.Size(152, 23);
            this.btnStopMotion.TabIndex = 36;
            this.btnStopMotion.Text = "Stop Motion";
            this.btnStopMotion.UseVisualStyleBackColor = true;
            this.btnStopMotion.Click += new System.EventHandler(this.btnStopMotion_Click);
            // 
            // Test_FestoCMMO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 524);
            this.Controls.Add(this.btnStopMotion);
            this.Controls.Add(this.btnSaveRecordTable);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnGetRecordTable);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.btnHoming);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnJog0);
            this.Controls.Add(this.btnJog1);
            this.Name = "Test_FestoCMMO";
            this.Text = "FestoCmmo";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRecordTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbActualCurrent;
        private System.Windows.Forms.Button btnJog1;
        private System.Windows.Forms.Button btnJog0;
        private System.Windows.Forms.Label lbMasterControl;
        private System.Windows.Forms.TextBox textBoxMasterControl;
        private System.Windows.Forms.Button btnEnable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbActualTorque;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lbTargetPosition;
        private System.Windows.Forms.Label lbTargetPositionUnit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnHoming;
        private System.Windows.Forms.Label lbActualPositionUnit;
        private System.Windows.Forms.Label lbActualPosition;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxIncrement;
        private System.Windows.Forms.Label lbActualVelocityUnit;
        private System.Windows.Forms.Label lbActualVelocity;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lbVelocityUnit;
        private System.Windows.Forms.TextBox textBoxVelocity;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label lbIncrementUnit;
        private System.Windows.Forms.Button btnStep0;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label lbSWbit7;
        private System.Windows.Forms.Label lbSWbit3;
        private System.Windows.Forms.Label lbSWbit10;
        private System.Windows.Forms.Label lbSWbit1;
        private System.Windows.Forms.Label lbSTObit;
        private System.Windows.Forms.Label lbSWbit2;
        private System.Windows.Forms.Label lbErrorMsg;
        private System.Windows.Forms.Label lbWarningMsg;
        private System.Windows.Forms.Label lbSWbit15;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lbOperationMode;
        private System.Windows.Forms.DataGridView dataGridViewRecordTable;
        private System.Windows.Forms.Button btnGetRecordTable;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnSaveRecordTable;
        private System.Windows.Forms.Button btnStep1;
        private System.Windows.Forms.Button btnStopMotion;
        private System.Windows.Forms.ComboBox cbxMeasurementUnit;
        private System.Windows.Forms.Label lbMeasurementUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn No;
        private System.Windows.Forms.DataGridViewComboBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn TargetX;
        private System.Windows.Forms.DataGridViewTextBoxColumn TargetY;
        private System.Windows.Forms.DataGridViewTextBoxColumn Velocity;
        private System.Windows.Forms.DataGridViewTextBoxColumn Acceleration;
        private System.Windows.Forms.DataGridViewTextBoxColumn Force;
    }
}

