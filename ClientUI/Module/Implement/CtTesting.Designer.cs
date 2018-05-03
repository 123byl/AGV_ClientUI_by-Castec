using VehiclePlanner.Partial.VehiclePlannerUI;

namespace VehiclePlanner.Module.Implement {
    partial class CtTesting {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtTesting));
            this.btnPosConfirm = new System.Windows.Forms.Button();
            this.btnSetCar = new System.Windows.Forms.Button();
            this.btnSimplyOri = new System.Windows.Forms.Button();
            this.btnGetOri = new System.Windows.Forms.Button();
            this.btnLoadOri = new System.Windows.Forms.Button();
            this.btnClrMap = new System.Windows.Forms.Button();
            this.btnGetMap = new System.Windows.Forms.Button();
            this.btnLoadMap = new System.Windows.Forms.Button();
            this.btnSendMap = new System.Windows.Forms.Button();
            this.btnGetLaser = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnFind = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.grbConnect = new VehiclePlanner.Partial.VehiclePlannerUI.CtGroupBox();
            this.lbHostIP = new System.Windows.Forms.Label();
            this.cboHostIP = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.gpbMapBuild = new VehiclePlanner.Partial.VehiclePlannerUI.CtGroupBox();
            this.btnGetCarStatus = new System.Windows.Forms.Button();
            this.btnMotionController = new System.Windows.Forms.Button();
            this.btnSetVelo = new System.Windows.Forms.Button();
            this.btnScan = new System.Windows.Forms.Button();
            this.lbVelocity = new System.Windows.Forms.Label();
            this.txtVelocity = new System.Windows.Forms.TextBox();
            this.btnServoOnOff = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.grbConnect.SuspendLayout();
            this.gpbMapBuild.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPosConfirm
            // 
            this.btnPosConfirm.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPosConfirm.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnPosConfirm.Image = global::VehiclePlanner.Properties.Resources.Zoom;
            this.btnPosConfirm.Location = new System.Drawing.Point(247, 338);
            this.btnPosConfirm.Margin = new System.Windows.Forms.Padding(4);
            this.btnPosConfirm.Name = "btnPosConfirm";
            this.btnPosConfirm.Size = new System.Drawing.Size(204, 84);
            this.btnPosConfirm.TabIndex = 49;
            this.btnPosConfirm.Text = "confirm car";
            this.btnPosConfirm.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPosConfirm.UseVisualStyleBackColor = true;
            this.btnPosConfirm.Click += new System.EventHandler(this.btnPosConfirm_Click);
            // 
            // btnSetCar
            // 
            this.btnSetCar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSetCar.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSetCar.Image = global::VehiclePlanner.Properties.Resources.Hand;
            this.btnSetCar.Location = new System.Drawing.Point(14, 338);
            this.btnSetCar.Margin = new System.Windows.Forms.Padding(4);
            this.btnSetCar.Name = "btnSetCar";
            this.btnSetCar.Size = new System.Drawing.Size(204, 84);
            this.btnSetCar.TabIndex = 48;
            this.btnSetCar.Text = "Set Car";
            this.btnSetCar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSetCar.UseVisualStyleBackColor = true;
            this.btnSetCar.Click += new System.EventHandler(this.btnSetCar_Click);
            // 
            // btnSimplyOri
            // 
            this.btnSimplyOri.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSimplyOri.Location = new System.Drawing.Point(605, 644);
            this.btnSimplyOri.Margin = new System.Windows.Forms.Padding(4);
            this.btnSimplyOri.Name = "btnSimplyOri";
            this.btnSimplyOri.Size = new System.Drawing.Size(90, 85);
            this.btnSimplyOri.TabIndex = 47;
            this.btnSimplyOri.Text = "Simplify Ori";
            this.btnSimplyOri.UseVisualStyleBackColor = true;
            this.btnSimplyOri.Visible = false;
            this.btnSimplyOri.Click += new System.EventHandler(this.btnSimplyOri_Click);
            // 
            // btnGetOri
            // 
            this.btnGetOri.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnGetOri.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetOri.Image = global::VehiclePlanner.Properties.Resources.Download;
            this.btnGetOri.Location = new System.Drawing.Point(14, 186);
            this.btnGetOri.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetOri.Name = "btnGetOri";
            this.btnGetOri.Size = new System.Drawing.Size(204, 84);
            this.btnGetOri.TabIndex = 43;
            this.btnGetOri.Text = "Get Ori";
            this.btnGetOri.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGetOri.UseVisualStyleBackColor = true;
            this.btnGetOri.Click += new System.EventHandler(this.btnGetOri_Click);
            // 
            // btnLoadOri
            // 
            this.btnLoadOri.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnLoadOri.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnLoadOri.Image = global::VehiclePlanner.Properties.Resources.Folder_files;
            this.btnLoadOri.Location = new System.Drawing.Point(14, 34);
            this.btnLoadOri.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoadOri.Name = "btnLoadOri";
            this.btnLoadOri.Size = new System.Drawing.Size(204, 84);
            this.btnLoadOri.TabIndex = 44;
            this.btnLoadOri.Text = "Load Ori";
            this.btnLoadOri.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoadOri.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLoadOri.UseVisualStyleBackColor = true;
            this.btnLoadOri.Click += new System.EventHandler(this.btnLoadOri_Click);
            // 
            // btnClrMap
            // 
            this.btnClrMap.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClrMap.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnClrMap.Image = global::VehiclePlanner.Properties.Resources.Eraser_S;
            this.btnClrMap.Location = new System.Drawing.Point(481, 34);
            this.btnClrMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnClrMap.Name = "btnClrMap";
            this.btnClrMap.Size = new System.Drawing.Size(204, 84);
            this.btnClrMap.TabIndex = 41;
            this.btnClrMap.Text = "Clear Map";
            this.btnClrMap.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnClrMap.UseVisualStyleBackColor = true;
            this.btnClrMap.Click += new System.EventHandler(this.btnClrMap_Click);
            // 
            // btnGetMap
            // 
            this.btnGetMap.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnGetMap.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetMap.Image = global::VehiclePlanner.Properties.Resources.Download;
            this.btnGetMap.Location = new System.Drawing.Point(247, 186);
            this.btnGetMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetMap.Name = "btnGetMap";
            this.btnGetMap.Size = new System.Drawing.Size(204, 84);
            this.btnGetMap.TabIndex = 62;
            this.btnGetMap.Text = "Get Map";
            this.btnGetMap.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGetMap.UseVisualStyleBackColor = true;
            this.btnGetMap.Click += new System.EventHandler(this.btnGetMap_Click);
            // 
            // btnLoadMap
            // 
            this.btnLoadMap.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnLoadMap.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnLoadMap.Image = global::VehiclePlanner.Properties.Resources.Folder_files;
            this.btnLoadMap.Location = new System.Drawing.Point(247, 34);
            this.btnLoadMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoadMap.Name = "btnLoadMap";
            this.btnLoadMap.Size = new System.Drawing.Size(204, 84);
            this.btnLoadMap.TabIndex = 61;
            this.btnLoadMap.Text = "Load Map";
            this.btnLoadMap.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLoadMap.UseVisualStyleBackColor = true;
            this.btnLoadMap.Click += new System.EventHandler(this.btnLoadMap_Click);
            // 
            // btnSendMap
            // 
            this.btnSendMap.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSendMap.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSendMap.Image = global::VehiclePlanner.Properties.Resources.Upload;
            this.btnSendMap.Location = new System.Drawing.Point(481, 186);
            this.btnSendMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnSendMap.Name = "btnSendMap";
            this.btnSendMap.Size = new System.Drawing.Size(204, 84);
            this.btnSendMap.TabIndex = 60;
            this.btnSendMap.Text = "Send Map";
            this.btnSendMap.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSendMap.UseVisualStyleBackColor = true;
            this.btnSendMap.Click += new System.EventHandler(this.btnSendMap_Click);
            // 
            // btnGetLaser
            // 
            this.btnGetLaser.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnGetLaser.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetLaser.Image = global::VehiclePlanner.Properties.Resources.Laser;
            this.btnGetLaser.Location = new System.Drawing.Point(481, 338);
            this.btnGetLaser.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetLaser.Name = "btnGetLaser";
            this.btnGetLaser.Size = new System.Drawing.Size(204, 84);
            this.btnGetLaser.TabIndex = 43;
            this.btnGetLaser.Text = "Get Laser";
            this.btnGetLaser.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGetLaser.UseVisualStyleBackColor = true;
            this.btnGetLaser.Click += new System.EventHandler(this.btnGetLaser_Click);
            // 
            // btnFind
            // 
            this.btnFind.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnFind.Image = ((System.Drawing.Image)(resources.GetObject("btnFind.Image")));
            this.btnFind.Location = new System.Drawing.Point(524, 54);
            this.btnFind.Margin = new System.Windows.Forms.Padding(4);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(44, 39);
            this.btnFind.TabIndex = 69;
            this.toolTip1.SetToolTip(this.btnFind, "Search iTS");
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.grbConnect, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gpbMapBuild, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 109F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 194F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(706, 766);
            this.tableLayoutPanel1.TabIndex = 66;
            // 
            // grbConnect
            // 
            this.grbConnect.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grbConnect.Controls.Add(this.btnFind);
            this.grbConnect.Controls.Add(this.lbHostIP);
            this.grbConnect.Controls.Add(this.cboHostIP);
            this.grbConnect.Controls.Add(this.btnConnect);
            this.grbConnect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grbConnect.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.grbConnect.Location = new System.Drawing.Point(3, 3);
            this.grbConnect.Name = "grbConnect";
            this.grbConnect.Size = new System.Drawing.Size(700, 103);
            this.grbConnect.TabIndex = 13;
            this.grbConnect.TabStop = false;
            this.grbConnect.Text = "iTS Connect";
            // 
            // lbHostIP
            // 
            this.lbHostIP.AutoSize = true;
            this.lbHostIP.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbHostIP.Location = new System.Drawing.Point(9, 48);
            this.lbHostIP.Name = "lbHostIP";
            this.lbHostIP.Size = new System.Drawing.Size(79, 25);
            this.lbHostIP.TabIndex = 68;
            this.lbHostIP.Text = "Host IP";
            // 
            // cboHostIP
            // 
            this.cboHostIP.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cboHostIP.FormattingEnabled = true;
            this.cboHostIP.Location = new System.Drawing.Point(94, 45);
            this.cboHostIP.Name = "cboHostIP";
            this.cboHostIP.Size = new System.Drawing.Size(184, 33);
            this.cboHostIP.TabIndex = 67;
            this.cboHostIP.Text = "192.168.50.152";
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnConnect.Image = global::VehiclePlanner.Properties.Resources.Disconnect;
            this.btnConnect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConnect.Location = new System.Drawing.Point(285, 28);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(231, 65);
            this.btnConnect.TabIndex = 66;
            this.btnConnect.Text = "Connect AGV";
            this.btnConnect.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // gpbMapBuild
            // 
            this.gpbMapBuild.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.gpbMapBuild.Controls.Add(this.btnGetCarStatus);
            this.gpbMapBuild.Controls.Add(this.btnMotionController);
            this.gpbMapBuild.Controls.Add(this.btnSetVelo);
            this.gpbMapBuild.Controls.Add(this.btnScan);
            this.gpbMapBuild.Controls.Add(this.lbVelocity);
            this.gpbMapBuild.Controls.Add(this.txtVelocity);
            this.gpbMapBuild.Controls.Add(this.btnServoOnOff);
            this.gpbMapBuild.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpbMapBuild.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpbMapBuild.Location = new System.Drawing.Point(4, 113);
            this.gpbMapBuild.Margin = new System.Windows.Forms.Padding(4);
            this.gpbMapBuild.Name = "gpbMapBuild";
            this.gpbMapBuild.Padding = new System.Windows.Forms.Padding(4);
            this.gpbMapBuild.Size = new System.Drawing.Size(698, 186);
            this.gpbMapBuild.TabIndex = 38;
            this.gpbMapBuild.TabStop = false;
            this.gpbMapBuild.Text = "Map Build";
            // 
            // btnGetCarStatus
            // 
            this.btnGetCarStatus.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetCarStatus.Image = global::VehiclePlanner.Properties.Resources.Radar;
            this.btnGetCarStatus.Location = new System.Drawing.Point(480, 109);
            this.btnGetCarStatus.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetCarStatus.Name = "btnGetCarStatus";
            this.btnGetCarStatus.Size = new System.Drawing.Size(204, 69);
            this.btnGetCarStatus.TabIndex = 44;
            this.btnGetCarStatus.Text = "Car";
            this.btnGetCarStatus.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGetCarStatus.UseVisualStyleBackColor = true;
            this.btnGetCarStatus.Click += new System.EventHandler(this.btnGetCarStatus_Click);
            // 
            // btnMotionController
            // 
            this.btnMotionController.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnMotionController.Image = global::VehiclePlanner.Properties.Resources.Controller;
            this.btnMotionController.Location = new System.Drawing.Point(108, 31);
            this.btnMotionController.Margin = new System.Windows.Forms.Padding(4);
            this.btnMotionController.Name = "btnMotionController";
            this.btnMotionController.Size = new System.Drawing.Size(342, 67);
            this.btnMotionController.TabIndex = 65;
            this.btnMotionController.Text = "Motion Controller";
            this.btnMotionController.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnMotionController.UseVisualStyleBackColor = true;
            this.btnMotionController.Click += new System.EventHandler(this.btnMotionController_Click);
            // 
            // btnSetVelo
            // 
            this.btnSetVelo.Location = new System.Drawing.Point(293, 122);
            this.btnSetVelo.Margin = new System.Windows.Forms.Padding(4);
            this.btnSetVelo.Name = "btnSetVelo";
            this.btnSetVelo.Size = new System.Drawing.Size(67, 35);
            this.btnSetVelo.TabIndex = 11;
            this.btnSetVelo.Text = "Set";
            this.btnSetVelo.UseVisualStyleBackColor = true;
            this.btnSetVelo.Click += new System.EventHandler(this.btnSetVelo_Click);
            // 
            // btnScan
            // 
            this.btnScan.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnScan.Image = global::VehiclePlanner.Properties.Resources.Scan;
            this.btnScan.Location = new System.Drawing.Point(480, 31);
            this.btnScan.Margin = new System.Windows.Forms.Padding(4);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(204, 69);
            this.btnScan.TabIndex = 61;
            this.btnScan.Text = "Scan";
            this.btnScan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // lbVelocity
            // 
            this.lbVelocity.AutoSize = true;
            this.lbVelocity.BackColor = System.Drawing.Color.Transparent;
            this.lbVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVelocity.Location = new System.Drawing.Point(41, 128);
            this.lbVelocity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbVelocity.Name = "lbVelocity";
            this.lbVelocity.Size = new System.Drawing.Size(169, 22);
            this.lbVelocity.TabIndex = 1;
            this.lbVelocity.Text = "Movement Velocity:";
            // 
            // txtVelocity
            // 
            this.txtVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVelocity.Location = new System.Drawing.Point(218, 125);
            this.txtVelocity.Margin = new System.Windows.Forms.Padding(4);
            this.txtVelocity.Name = "txtVelocity";
            this.txtVelocity.Size = new System.Drawing.Size(67, 30);
            this.txtVelocity.TabIndex = 0;
            this.txtVelocity.Text = "500";
            this.txtVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnServoOnOff
            // 
            this.btnServoOnOff.BackColor = System.Drawing.Color.Red;
            this.btnServoOnOff.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnServoOnOff.ForeColor = System.Drawing.Color.White;
            this.btnServoOnOff.Location = new System.Drawing.Point(33, 31);
            this.btnServoOnOff.Margin = new System.Windows.Forms.Padding(4);
            this.btnServoOnOff.Name = "btnServoOnOff";
            this.btnServoOnOff.Size = new System.Drawing.Size(67, 67);
            this.btnServoOnOff.TabIndex = 1;
            this.btnServoOnOff.Text = "OFF";
            this.btnServoOnOff.UseVisualStyleBackColor = false;
            this.btnServoOnOff.Click += new System.EventHandler(this.btnServoOnOff_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Controls.Add(this.btnLoadOri, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnGetLaser, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.btnLoadMap, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnSendMap, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnClrMap, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnPosConfirm, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.btnGetMap, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnSetCar, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.btnGetOri, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 306);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(700, 457);
            this.tableLayoutPanel2.TabIndex = 67;
            // 
            // CtTesting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 767);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnSimplyOri);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CtTesting";
            this.Text = "Testing";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.grbConnect.ResumeLayout(false);
            this.grbConnect.PerformLayout();
            this.gpbMapBuild.ResumeLayout(false);
            this.gpbMapBuild.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGetOri;
        private System.Windows.Forms.Button btnLoadOri;
        private System.Windows.Forms.Button btnClrMap;
        private System.Windows.Forms.Button btnSimplyOri;
        private System.Windows.Forms.Button btnSetCar;
        private System.Windows.Forms.Button btnPosConfirm;
        private System.Windows.Forms.Button btnGetLaser;
        private System.Windows.Forms.Button btnGetMap;
        private System.Windows.Forms.Button btnLoadMap;
        private System.Windows.Forms.Button btnSendMap;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private CtGroupBox grbConnect;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Label lbHostIP;
        private System.Windows.Forms.ComboBox cboHostIP;
        private System.Windows.Forms.Button btnConnect;
        private CtGroupBox gpbMapBuild;
        private System.Windows.Forms.Button btnGetCarStatus;
        private System.Windows.Forms.Button btnMotionController;
        private System.Windows.Forms.Button btnSetVelo;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.Label lbVelocity;
        private System.Windows.Forms.TextBox txtVelocity;
        private System.Windows.Forms.Button btnServoOnOff;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}