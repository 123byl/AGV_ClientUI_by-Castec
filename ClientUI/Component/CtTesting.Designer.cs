﻿namespace VehiclePlanner {
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
            this.btnMotionController = new System.Windows.Forms.Button();
            this.lbHostIP = new System.Windows.Forms.Label();
            this.cboHostIP = new System.Windows.Forms.ComboBox();
            this.btnScan = new System.Windows.Forms.Button();
            this.btnPosConfirm = new System.Windows.Forms.Button();
            this.btnSetCar = new System.Windows.Forms.Button();
            this.btnSimplyOri = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnGetOri = new System.Windows.Forms.Button();
            this.btnLoadOri = new System.Windows.Forms.Button();
            this.btnClrMap = new System.Windows.Forms.Button();
            this.grbMap = new VehiclePlanner.CtGroupBox();
            this.btnGetMap = new System.Windows.Forms.Button();
            this.btnLoadMap = new System.Windows.Forms.Button();
            this.btnSendMap = new System.Windows.Forms.Button();
            this.grbInfo = new VehiclePlanner.CtGroupBox();
            this.btnGetLaser = new System.Windows.Forms.Button();
            this.btnGetCarStatus = new System.Windows.Forms.Button();
            this.gpbShift = new VehiclePlanner.CtGroupBox();
            this.btnSetVelo = new System.Windows.Forms.Button();
            this.lbVelocity = new System.Windows.Forms.Label();
            this.txtVelocity = new System.Windows.Forms.TextBox();
            this.btnServoOnOff = new System.Windows.Forms.Button();
            this.btnFind = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.grbMap.SuspendLayout();
            this.grbInfo.SuspendLayout();
            this.gpbShift.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnMotionController
            // 
            this.btnMotionController.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnMotionController.Location = new System.Drawing.Point(106, 74);
            this.btnMotionController.Margin = new System.Windows.Forms.Padding(4);
            this.btnMotionController.Name = "btnMotionController";
            this.btnMotionController.Size = new System.Drawing.Size(123, 85);
            this.btnMotionController.TabIndex = 65;
            this.btnMotionController.Text = "Motion Controller";
            this.btnMotionController.UseVisualStyleBackColor = true;
            this.btnMotionController.Click += new System.EventHandler(this.btnMotionController_Click);
            // 
            // lbHostIP
            // 
            this.lbHostIP.AutoSize = true;
            this.lbHostIP.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbHostIP.Location = new System.Drawing.Point(12, 29);
            this.lbHostIP.Name = "lbHostIP";
            this.lbHostIP.Size = new System.Drawing.Size(79, 25);
            this.lbHostIP.TabIndex = 64;
            this.lbHostIP.Text = "Host IP";
            // 
            // cboHostIP
            // 
            this.cboHostIP.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cboHostIP.FormattingEnabled = true;
            this.cboHostIP.Location = new System.Drawing.Point(11, 57);
            this.cboHostIP.Name = "cboHostIP";
            this.cboHostIP.Size = new System.Drawing.Size(184, 33);
            this.cboHostIP.TabIndex = 63;
            this.cboHostIP.Text = "192.168.50.152";
            // 
            // btnScan
            // 
            this.btnScan.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnScan.Location = new System.Drawing.Point(460, 24);
            this.btnScan.Margin = new System.Windows.Forms.Padding(4);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(105, 65);
            this.btnScan.TabIndex = 61;
            this.btnScan.Text = "Scan";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // btnPosConfirm
            // 
            this.btnPosConfirm.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnPosConfirm.Location = new System.Drawing.Point(586, 24);
            this.btnPosConfirm.Margin = new System.Windows.Forms.Padding(4);
            this.btnPosConfirm.Name = "btnPosConfirm";
            this.btnPosConfirm.Size = new System.Drawing.Size(109, 65);
            this.btnPosConfirm.TabIndex = 49;
            this.btnPosConfirm.Text = "confirm car";
            this.btnPosConfirm.UseVisualStyleBackColor = true;
            this.btnPosConfirm.Click += new System.EventHandler(this.btnPosConfirm_Click);
            // 
            // btnSetCar
            // 
            this.btnSetCar.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSetCar.Location = new System.Drawing.Point(503, 402);
            this.btnSetCar.Margin = new System.Windows.Forms.Padding(4);
            this.btnSetCar.Name = "btnSetCar";
            this.btnSetCar.Size = new System.Drawing.Size(90, 85);
            this.btnSetCar.TabIndex = 48;
            this.btnSetCar.Text = "Set Car";
            this.btnSetCar.UseVisualStyleBackColor = true;
            this.btnSetCar.Click += new System.EventHandler(this.btnSetCar_Click);
            // 
            // btnSimplyOri
            // 
            this.btnSimplyOri.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSimplyOri.Location = new System.Drawing.Point(240, 287);
            this.btnSimplyOri.Margin = new System.Windows.Forms.Padding(4);
            this.btnSimplyOri.Name = "btnSimplyOri";
            this.btnSimplyOri.Size = new System.Drawing.Size(90, 85);
            this.btnSimplyOri.TabIndex = 47;
            this.btnSimplyOri.Text = "Simplify Ori";
            this.btnSimplyOri.UseVisualStyleBackColor = true;
            this.btnSimplyOri.Click += new System.EventHandler(this.btnSimplyOri_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnConnect.Image = global::VehiclePlanner.Properties.Resources.Disconnect;
            this.btnConnect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConnect.Location = new System.Drawing.Point(202, 24);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(231, 65);
            this.btnConnect.TabIndex = 45;
            this.btnConnect.Text = "Connect AGV";
            this.btnConnect.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnGetOri
            // 
            this.btnGetOri.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetOri.Location = new System.Drawing.Point(126, 287);
            this.btnGetOri.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetOri.Name = "btnGetOri";
            this.btnGetOri.Size = new System.Drawing.Size(90, 85);
            this.btnGetOri.TabIndex = 43;
            this.btnGetOri.Text = "Get Ori";
            this.btnGetOri.UseVisualStyleBackColor = true;
            this.btnGetOri.Click += new System.EventHandler(this.btnGetOri_Click);
            // 
            // btnLoadOri
            // 
            this.btnLoadOri.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnLoadOri.Location = new System.Drawing.Point(30, 287);
            this.btnLoadOri.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoadOri.Name = "btnLoadOri";
            this.btnLoadOri.Size = new System.Drawing.Size(90, 85);
            this.btnLoadOri.TabIndex = 44;
            this.btnLoadOri.Text = "Load Ori";
            this.btnLoadOri.UseVisualStyleBackColor = true;
            this.btnLoadOri.Click += new System.EventHandler(this.btnLoadOri_Click);
            // 
            // btnClrMap
            // 
            this.btnClrMap.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnClrMap.Location = new System.Drawing.Point(601, 402);
            this.btnClrMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnClrMap.Name = "btnClrMap";
            this.btnClrMap.Size = new System.Drawing.Size(90, 85);
            this.btnClrMap.TabIndex = 41;
            this.btnClrMap.Text = "Clear Map";
            this.btnClrMap.UseVisualStyleBackColor = true;
            this.btnClrMap.Click += new System.EventHandler(this.btnClrMap_Click);
            // 
            // grbMap
            // 
            this.grbMap.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grbMap.Controls.Add(this.btnGetMap);
            this.grbMap.Controls.Add(this.btnLoadMap);
            this.grbMap.Controls.Add(this.btnSendMap);
            this.grbMap.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.grbMap.Location = new System.Drawing.Point(368, 255);
            this.grbMap.Name = "grbMap";
            this.grbMap.Size = new System.Drawing.Size(323, 140);
            this.grbMap.TabIndex = 62;
            this.grbMap.TabStop = false;
            this.grbMap.Text = "Map";
            // 
            // btnGetMap
            // 
            this.btnGetMap.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetMap.Location = new System.Drawing.Point(115, 34);
            this.btnGetMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetMap.Name = "btnGetMap";
            this.btnGetMap.Size = new System.Drawing.Size(90, 85);
            this.btnGetMap.TabIndex = 62;
            this.btnGetMap.Text = "Get Map";
            this.btnGetMap.UseVisualStyleBackColor = true;
            this.btnGetMap.Click += new System.EventHandler(this.btnGetMap_Click);
            // 
            // btnLoadMap
            // 
            this.btnLoadMap.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnLoadMap.Location = new System.Drawing.Point(17, 34);
            this.btnLoadMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoadMap.Name = "btnLoadMap";
            this.btnLoadMap.Size = new System.Drawing.Size(90, 85);
            this.btnLoadMap.TabIndex = 61;
            this.btnLoadMap.Text = "Load Map";
            this.btnLoadMap.UseVisualStyleBackColor = true;
            this.btnLoadMap.Click += new System.EventHandler(this.btnLoadMap_Click);
            // 
            // btnSendMap
            // 
            this.btnSendMap.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSendMap.Location = new System.Drawing.Point(213, 34);
            this.btnSendMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnSendMap.Name = "btnSendMap";
            this.btnSendMap.Size = new System.Drawing.Size(90, 85);
            this.btnSendMap.TabIndex = 60;
            this.btnSendMap.Text = "Send Map";
            this.btnSendMap.UseVisualStyleBackColor = true;
            this.btnSendMap.Click += new System.EventHandler(this.btnSendMap_Click);
            // 
            // grbInfo
            // 
            this.grbInfo.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grbInfo.Controls.Add(this.btnGetLaser);
            this.grbInfo.Controls.Add(this.btnGetCarStatus);
            this.grbInfo.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.grbInfo.Location = new System.Drawing.Point(368, 107);
            this.grbInfo.Name = "grbInfo";
            this.grbInfo.Size = new System.Drawing.Size(222, 142);
            this.grbInfo.TabIndex = 12;
            this.grbInfo.TabStop = false;
            this.grbInfo.Text = "Information";
            // 
            // btnGetLaser
            // 
            this.btnGetLaser.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetLaser.Location = new System.Drawing.Point(17, 36);
            this.btnGetLaser.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetLaser.Name = "btnGetLaser";
            this.btnGetLaser.Size = new System.Drawing.Size(90, 85);
            this.btnGetLaser.TabIndex = 43;
            this.btnGetLaser.Text = "Get Laser";
            this.btnGetLaser.UseVisualStyleBackColor = true;
            this.btnGetLaser.Click += new System.EventHandler(this.btnGetLaser_Click);
            // 
            // btnGetCarStatus
            // 
            this.btnGetCarStatus.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetCarStatus.Location = new System.Drawing.Point(115, 36);
            this.btnGetCarStatus.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetCarStatus.Name = "btnGetCarStatus";
            this.btnGetCarStatus.Size = new System.Drawing.Size(90, 85);
            this.btnGetCarStatus.TabIndex = 44;
            this.btnGetCarStatus.Text = "Car";
            this.btnGetCarStatus.UseVisualStyleBackColor = true;
            this.btnGetCarStatus.Click += new System.EventHandler(this.btnGetCarStatus_Click);
            // 
            // gpbShift
            // 
            this.gpbShift.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.gpbShift.Controls.Add(this.btnMotionController);
            this.gpbShift.Controls.Add(this.btnSetVelo);
            this.gpbShift.Controls.Add(this.lbVelocity);
            this.gpbShift.Controls.Add(this.txtVelocity);
            this.gpbShift.Controls.Add(this.btnServoOnOff);
            this.gpbShift.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpbShift.Location = new System.Drawing.Point(11, 107);
            this.gpbShift.Margin = new System.Windows.Forms.Padding(4);
            this.gpbShift.Name = "gpbShift";
            this.gpbShift.Padding = new System.Windows.Forms.Padding(4);
            this.gpbShift.Size = new System.Drawing.Size(350, 172);
            this.gpbShift.TabIndex = 37;
            this.gpbShift.TabStop = false;
            this.gpbShift.Text = "Velocity";
            // 
            // btnSetVelo
            // 
            this.btnSetVelo.Location = new System.Drawing.Point(267, 35);
            this.btnSetVelo.Margin = new System.Windows.Forms.Padding(4);
            this.btnSetVelo.Name = "btnSetVelo";
            this.btnSetVelo.Size = new System.Drawing.Size(67, 35);
            this.btnSetVelo.TabIndex = 11;
            this.btnSetVelo.Text = "Set";
            this.btnSetVelo.UseVisualStyleBackColor = true;
            this.btnSetVelo.Click += new System.EventHandler(this.btnSetVelo_Click);
            // 
            // lbVelocity
            // 
            this.lbVelocity.AutoSize = true;
            this.lbVelocity.BackColor = System.Drawing.Color.Transparent;
            this.lbVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVelocity.Location = new System.Drawing.Point(15, 41);
            this.lbVelocity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbVelocity.Name = "lbVelocity";
            this.lbVelocity.Size = new System.Drawing.Size(169, 22);
            this.lbVelocity.TabIndex = 1;
            this.lbVelocity.Text = "Movement Velocity:";
            // 
            // txtVelocity
            // 
            this.txtVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVelocity.Location = new System.Drawing.Point(192, 36);
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
            this.btnServoOnOff.Location = new System.Drawing.Point(19, 80);
            this.btnServoOnOff.Margin = new System.Windows.Forms.Padding(4);
            this.btnServoOnOff.Name = "btnServoOnOff";
            this.btnServoOnOff.Size = new System.Drawing.Size(67, 62);
            this.btnServoOnOff.TabIndex = 1;
            this.btnServoOnOff.Text = "OFF";
            this.btnServoOnOff.UseVisualStyleBackColor = false;
            this.btnServoOnOff.Click += new System.EventHandler(this.btnServoOnOff_Click);
            // 
            // btnFind
            // 
            this.btnFind.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnFind.Image = ((System.Drawing.Image)(resources.GetObject("btnFind.Image")));
            this.btnFind.Location = new System.Drawing.Point(150, 10);
            this.btnFind.Margin = new System.Windows.Forms.Padding(4);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(44, 39);
            this.btnFind.TabIndex = 65;
            this.toolTip1.SetToolTip(this.btnFind, "Search iTS");
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // CtTesting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 767);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.lbHostIP);
            this.Controls.Add(this.cboHostIP);
            this.Controls.Add(this.grbMap);
            this.Controls.Add(this.grbInfo);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.btnPosConfirm);
            this.Controls.Add(this.btnSetCar);
            this.Controls.Add(this.btnSimplyOri);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnGetOri);
            this.Controls.Add(this.btnLoadOri);
            this.Controls.Add(this.btnClrMap);
            this.Controls.Add(this.gpbShift);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CtTesting";
            this.Text = "Testing";
            this.grbMap.ResumeLayout(false);
            this.grbInfo.ResumeLayout(false);
            this.gpbShift.ResumeLayout(false);
            this.gpbShift.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGetOri;
        private System.Windows.Forms.Button btnLoadOri;
        private System.Windows.Forms.Button btnClrMap;
        private System.Windows.Forms.Button btnSetVelo;
        private System.Windows.Forms.Label lbVelocity;
        private System.Windows.Forms.TextBox txtVelocity;
        private System.Windows.Forms.Button btnServoOnOff;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnSimplyOri;
        private System.Windows.Forms.Button btnSetCar;
        private System.Windows.Forms.Button btnPosConfirm;
        private CtGroupBox gpbShift;
        private System.Windows.Forms.Button btnScan;
        private CtGroupBox grbInfo;
        private System.Windows.Forms.Button btnGetCarStatus;
        private System.Windows.Forms.Button btnGetLaser;
        private CtGroupBox grbMap;
        private System.Windows.Forms.Button btnGetMap;
        private System.Windows.Forms.Button btnLoadMap;
        private System.Windows.Forms.Button btnSendMap;
        private System.Windows.Forms.ComboBox cboHostIP;
        private System.Windows.Forms.Label lbHostIP;
        private System.Windows.Forms.Button btnMotionController;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}