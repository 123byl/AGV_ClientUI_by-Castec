namespace ClientUI {
    partial class SocketTest {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SocketTest));
            this.grbServer = new ClientUI.CtGroupBox();
            this.btnListen = new System.Windows.Forms.Button();
            this.lbPort = new System.Windows.Forms.Label();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.grbClient = new ClientUI.CtGroupBox();
            this.lbIP = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.btnReconnect = new System.Windows.Forms.Button();
            this.lbClientPort = new System.Windows.Forms.Label();
            this.txtRemotePort = new System.Windows.Forms.TextBox();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnConnectAgv = new System.Windows.Forms.Button();
            this.btnSetVelo = new System.Windows.Forms.Button();
            this.lbVelocity = new System.Windows.Forms.Label();
            this.txtVelocity = new System.Windows.Forms.TextBox();
            this.btnServoOnOff = new System.Windows.Forms.Button();
            this.gpbShift = new ClientUI.CtGroupBox();
            this.btnLeft = new System.Windows.Forms.Button();
            this.grbMap = new ClientUI.CtGroupBox();
            this.btnGetMap = new System.Windows.Forms.Button();
            this.btnSendMap = new System.Windows.Forms.Button();
            this.btnGetOri = new System.Windows.Forms.Button();
            this.btnPosConfirm = new System.Windows.Forms.Button();
            this.btnGetCarStatus = new System.Windows.Forms.Button();
            this.grbInfo = new ClientUI.CtGroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTheta = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtY = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGetLaser = new System.Windows.Forms.Button();
            this.txtX = new System.Windows.Forms.TextBox();
            this.grbMode = new ClientUI.CtGroupBox();
            this.btnIdleMode = new System.Windows.Forms.Button();
            this.btnWorkMode = new System.Windows.Forms.Button();
            this.btnMapMode = new System.Windows.Forms.Button();
            this.grbServer.SuspendLayout();
            this.grbClient.SuspendLayout();
            this.gpbShift.SuspendLayout();
            this.grbMap.SuspendLayout();
            this.grbInfo.SuspendLayout();
            this.grbMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbServer
            // 
            this.grbServer.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grbServer.Controls.Add(this.btnListen);
            this.grbServer.Controls.Add(this.lbPort);
            this.grbServer.Controls.Add(this.txtServerPort);
            this.grbServer.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.grbServer.Location = new System.Drawing.Point(12, 12);
            this.grbServer.Name = "grbServer";
            this.grbServer.Size = new System.Drawing.Size(370, 92);
            this.grbServer.TabIndex = 0;
            this.grbServer.TabStop = false;
            this.grbServer.Text = "Server";
            // 
            // btnListen
            // 
            this.btnListen.Location = new System.Drawing.Point(218, 33);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(117, 38);
            this.btnListen.TabIndex = 2;
            this.btnListen.Text = "Listen";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
            // 
            // lbPort
            // 
            this.lbPort.AutoSize = true;
            this.lbPort.Location = new System.Drawing.Point(35, 36);
            this.lbPort.Name = "lbPort";
            this.lbPort.Size = new System.Drawing.Size(71, 25);
            this.lbPort.TabIndex = 1;
            this.lbPort.Text = "Port：";
            // 
            // txtServerPort
            // 
            this.txtServerPort.Location = new System.Drawing.Point(112, 33);
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.Size = new System.Drawing.Size(100, 34);
            this.txtServerPort.TabIndex = 0;
            this.txtServerPort.Text = "400";
            // 
            // grbClient
            // 
            this.grbClient.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grbClient.Controls.Add(this.lbIP);
            this.grbClient.Controls.Add(this.txtIP);
            this.grbClient.Controls.Add(this.btnReconnect);
            this.grbClient.Controls.Add(this.lbClientPort);
            this.grbClient.Controls.Add(this.txtRemotePort);
            this.grbClient.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.grbClient.Location = new System.Drawing.Point(12, 110);
            this.grbClient.Name = "grbClient";
            this.grbClient.Size = new System.Drawing.Size(581, 92);
            this.grbClient.TabIndex = 1;
            this.grbClient.TabStop = false;
            this.grbClient.Text = "Client";
            // 
            // lbIP
            // 
            this.lbIP.AutoSize = true;
            this.lbIP.Location = new System.Drawing.Point(27, 40);
            this.lbIP.Name = "lbIP";
            this.lbIP.Size = new System.Drawing.Size(124, 25);
            this.lbIP.TabIndex = 4;
            this.lbIP.Text = "RemoteIP：";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(157, 37);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(100, 34);
            this.txtIP.TabIndex = 3;
            this.txtIP.Text = "127.0.0.1";
            // 
            // btnReconnect
            // 
            this.btnReconnect.Location = new System.Drawing.Point(447, 33);
            this.btnReconnect.Name = "btnReconnect";
            this.btnReconnect.Size = new System.Drawing.Size(117, 38);
            this.btnReconnect.TabIndex = 2;
            this.btnReconnect.Text = "Connect";
            this.btnReconnect.UseVisualStyleBackColor = true;
            this.btnReconnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lbClientPort
            // 
            this.lbClientPort.AutoSize = true;
            this.lbClientPort.Location = new System.Drawing.Point(264, 40);
            this.lbClientPort.Name = "lbClientPort";
            this.lbClientPort.Size = new System.Drawing.Size(71, 25);
            this.lbClientPort.TabIndex = 1;
            this.lbClientPort.Text = "Port：";
            // 
            // txtRemotePort
            // 
            this.txtRemotePort.Location = new System.Drawing.Point(341, 37);
            this.txtRemotePort.Name = "txtRemotePort";
            this.txtRemotePort.Size = new System.Drawing.Size(100, 34);
            this.txtRemotePort.TabIndex = 0;
            this.txtRemotePort.Text = "400";
            // 
            // txtConsole
            // 
            this.txtConsole.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtConsole.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtConsole.Location = new System.Drawing.Point(0, 613);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtConsole.Size = new System.Drawing.Size(722, 235);
            this.txtConsole.TabIndex = 4;
            this.txtConsole.WordWrap = false;
            // 
            // btnStartStop
            // 
            this.btnStartStop.BackColor = System.Drawing.SystemColors.Control;
            this.btnStartStop.Enabled = false;
            this.btnStartStop.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStartStop.Image")));
            this.btnStartStop.Location = new System.Drawing.Point(254, 93);
            this.btnStartStop.Margin = new System.Windows.Forms.Padding(4);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(67, 62);
            this.btnStartStop.TabIndex = 10;
            this.btnStartStop.Tag = "Stop";
            this.btnStartStop.UseVisualStyleBackColor = false;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // btnUp
            // 
            this.btnUp.Enabled = false;
            this.btnUp.Font = new System.Drawing.Font("新細明體", 20F);
            this.btnUp.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
            this.btnUp.Location = new System.Drawing.Point(143, 93);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(67, 62);
            this.btnUp.TabIndex = 1;
            this.btnUp.Tag = "0";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnRight
            // 
            this.btnRight.Enabled = false;
            this.btnRight.Font = new System.Drawing.Font("新細明體", 20F);
            this.btnRight.Image = ((System.Drawing.Image)(resources.GetObject("btnRight.Image")));
            this.btnRight.Location = new System.Drawing.Point(254, 172);
            this.btnRight.Margin = new System.Windows.Forms.Padding(4);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(67, 62);
            this.btnRight.TabIndex = 1;
            this.btnRight.Tag = "2";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnDown
            // 
            this.btnDown.Enabled = false;
            this.btnDown.Font = new System.Drawing.Font("新細明體", 20F);
            this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
            this.btnDown.Location = new System.Drawing.Point(143, 172);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(67, 62);
            this.btnDown.TabIndex = 1;
            this.btnDown.Tag = "1";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnConnectAgv
            // 
            this.btnConnectAgv.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnConnectAgv.Image = global::ClientUI.Properties.Resources.Disconnect;
            this.btnConnectAgv.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConnectAgv.Location = new System.Drawing.Point(435, 32);
            this.btnConnectAgv.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnectAgv.Name = "btnConnectAgv";
            this.btnConnectAgv.Size = new System.Drawing.Size(231, 65);
            this.btnConnectAgv.TabIndex = 67;
            this.btnConnectAgv.Text = "Connect AGV";
            this.btnConnectAgv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnConnectAgv.UseVisualStyleBackColor = true;
            this.btnConnectAgv.Click += new System.EventHandler(this.btnConnectAgv_Click);
            // 
            // btnSetVelo
            // 
            this.btnSetVelo.Enabled = false;
            this.btnSetVelo.Location = new System.Drawing.Point(265, 35);
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
            this.txtVelocity.Enabled = false;
            this.txtVelocity.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVelocity.Location = new System.Drawing.Point(192, 38);
            this.txtVelocity.Margin = new System.Windows.Forms.Padding(4);
            this.txtVelocity.Name = "txtVelocity";
            this.txtVelocity.Size = new System.Drawing.Size(67, 30);
            this.txtVelocity.TabIndex = 0;
            this.txtVelocity.Text = "500";
            this.txtVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnServoOnOff
            // 
            this.btnServoOnOff.BackColor = System.Drawing.Color.Green;
            this.btnServoOnOff.Enabled = false;
            this.btnServoOnOff.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnServoOnOff.ForeColor = System.Drawing.Color.White;
            this.btnServoOnOff.Location = new System.Drawing.Point(32, 93);
            this.btnServoOnOff.Margin = new System.Windows.Forms.Padding(4);
            this.btnServoOnOff.Name = "btnServoOnOff";
            this.btnServoOnOff.Size = new System.Drawing.Size(67, 62);
            this.btnServoOnOff.TabIndex = 1;
            this.btnServoOnOff.Text = "ON";
            this.btnServoOnOff.UseVisualStyleBackColor = false;
            this.btnServoOnOff.Click += new System.EventHandler(this.btnServoOnOff_Click);
            // 
            // gpbShift
            // 
            this.gpbShift.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.gpbShift.Controls.Add(this.btnSetVelo);
            this.gpbShift.Controls.Add(this.btnStartStop);
            this.gpbShift.Controls.Add(this.lbVelocity);
            this.gpbShift.Controls.Add(this.txtVelocity);
            this.gpbShift.Controls.Add(this.btnServoOnOff);
            this.gpbShift.Controls.Add(this.btnUp);
            this.gpbShift.Controls.Add(this.btnRight);
            this.gpbShift.Controls.Add(this.btnLeft);
            this.gpbShift.Controls.Add(this.btnDown);
            this.gpbShift.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpbShift.Location = new System.Drawing.Point(13, 209);
            this.gpbShift.Margin = new System.Windows.Forms.Padding(4);
            this.gpbShift.Name = "gpbShift";
            this.gpbShift.Padding = new System.Windows.Forms.Padding(4);
            this.gpbShift.Size = new System.Drawing.Size(340, 250);
            this.gpbShift.TabIndex = 64;
            this.gpbShift.TabStop = false;
            this.gpbShift.Text = "Velocity";
            // 
            // btnLeft
            // 
            this.btnLeft.Enabled = false;
            this.btnLeft.Font = new System.Drawing.Font("新細明體", 20F);
            this.btnLeft.Image = ((System.Drawing.Image)(resources.GetObject("btnLeft.Image")));
            this.btnLeft.Location = new System.Drawing.Point(32, 172);
            this.btnLeft.Margin = new System.Windows.Forms.Padding(4);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(67, 62);
            this.btnLeft.TabIndex = 1;
            this.btnLeft.Tag = "3";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // grbMap
            // 
            this.grbMap.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grbMap.Controls.Add(this.btnGetMap);
            this.grbMap.Controls.Add(this.btnSendMap);
            this.grbMap.Controls.Add(this.btnGetOri);
            this.grbMap.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.grbMap.Location = new System.Drawing.Point(360, 208);
            this.grbMap.Name = "grbMap";
            this.grbMap.Size = new System.Drawing.Size(341, 140);
            this.grbMap.TabIndex = 73;
            this.grbMap.TabStop = false;
            this.grbMap.Text = "Map";
            // 
            // btnGetMap
            // 
            this.btnGetMap.Enabled = false;
            this.btnGetMap.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetMap.Location = new System.Drawing.Point(125, 34);
            this.btnGetMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetMap.Name = "btnGetMap";
            this.btnGetMap.Size = new System.Drawing.Size(90, 85);
            this.btnGetMap.TabIndex = 62;
            this.btnGetMap.Text = "Get Map";
            this.btnGetMap.UseVisualStyleBackColor = true;
            this.btnGetMap.Click += new System.EventHandler(this.btnGetMap_Click);
            // 
            // btnSendMap
            // 
            this.btnSendMap.Enabled = false;
            this.btnSendMap.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSendMap.Location = new System.Drawing.Point(243, 34);
            this.btnSendMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnSendMap.Name = "btnSendMap";
            this.btnSendMap.Size = new System.Drawing.Size(90, 85);
            this.btnSendMap.TabIndex = 60;
            this.btnSendMap.Text = "Send Map";
            this.btnSendMap.UseVisualStyleBackColor = true;
            this.btnSendMap.Click += new System.EventHandler(this.btnSendMap_Click);
            // 
            // btnGetOri
            // 
            this.btnGetOri.Enabled = false;
            this.btnGetOri.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetOri.Location = new System.Drawing.Point(7, 34);
            this.btnGetOri.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetOri.Name = "btnGetOri";
            this.btnGetOri.Size = new System.Drawing.Size(90, 85);
            this.btnGetOri.TabIndex = 65;
            this.btnGetOri.Text = "Get Ori";
            this.btnGetOri.UseVisualStyleBackColor = true;
            this.btnGetOri.Click += new System.EventHandler(this.btnGetOri_Click);
            // 
            // btnPosConfirm
            // 
            this.btnPosConfirm.Enabled = false;
            this.btnPosConfirm.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnPosConfirm.Location = new System.Drawing.Point(243, 97);
            this.btnPosConfirm.Margin = new System.Windows.Forms.Padding(4);
            this.btnPosConfirm.Name = "btnPosConfirm";
            this.btnPosConfirm.Size = new System.Drawing.Size(90, 85);
            this.btnPosConfirm.TabIndex = 70;
            this.btnPosConfirm.Text = "confirm car";
            this.btnPosConfirm.UseVisualStyleBackColor = true;
            this.btnPosConfirm.Click += new System.EventHandler(this.btnPosConfirm_Click);
            // 
            // btnGetCarStatus
            // 
            this.btnGetCarStatus.Enabled = false;
            this.btnGetCarStatus.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetCarStatus.Location = new System.Drawing.Point(125, 97);
            this.btnGetCarStatus.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetCarStatus.Name = "btnGetCarStatus";
            this.btnGetCarStatus.Size = new System.Drawing.Size(90, 85);
            this.btnGetCarStatus.TabIndex = 44;
            this.btnGetCarStatus.Text = "Car";
            this.btnGetCarStatus.UseVisualStyleBackColor = true;
            this.btnGetCarStatus.Click += new System.EventHandler(this.btnGetCarStatus_Click);
            // 
            // grbInfo
            // 
            this.grbInfo.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grbInfo.Controls.Add(this.label3);
            this.grbInfo.Controls.Add(this.txtTheta);
            this.grbInfo.Controls.Add(this.label2);
            this.grbInfo.Controls.Add(this.txtY);
            this.grbInfo.Controls.Add(this.label1);
            this.grbInfo.Controls.Add(this.btnGetLaser);
            this.grbInfo.Controls.Add(this.txtX);
            this.grbInfo.Controls.Add(this.btnGetCarStatus);
            this.grbInfo.Controls.Add(this.btnPosConfirm);
            this.grbInfo.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.grbInfo.Location = new System.Drawing.Point(360, 354);
            this.grbInfo.Name = "grbInfo";
            this.grbInfo.Size = new System.Drawing.Size(341, 216);
            this.grbInfo.TabIndex = 63;
            this.grbInfo.TabStop = false;
            this.grbInfo.Text = "Information";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(197, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 25);
            this.label3.TabIndex = 80;
            this.label3.Text = "Theta";
            // 
            // txtTheta
            // 
            this.txtTheta.Location = new System.Drawing.Point(262, 37);
            this.txtTheta.Name = "txtTheta";
            this.txtTheta.Size = new System.Drawing.Size(54, 34);
            this.txtTheta.TabIndex = 79;
            this.txtTheta.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(105, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 25);
            this.label2.TabIndex = 78;
            this.label2.Text = "Y";
            // 
            // txtY
            // 
            this.txtY.Location = new System.Drawing.Point(134, 36);
            this.txtY.Name = "txtY";
            this.txtY.Size = new System.Drawing.Size(54, 34);
            this.txtY.TabIndex = 77;
            this.txtY.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 25);
            this.label1.TabIndex = 76;
            this.label1.Text = "X";
            // 
            // btnGetLaser
            // 
            this.btnGetLaser.Enabled = false;
            this.btnGetLaser.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetLaser.Location = new System.Drawing.Point(7, 97);
            this.btnGetLaser.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetLaser.Name = "btnGetLaser";
            this.btnGetLaser.Size = new System.Drawing.Size(90, 85);
            this.btnGetLaser.TabIndex = 43;
            this.btnGetLaser.Text = "Get Laser";
            this.btnGetLaser.UseVisualStyleBackColor = true;
            this.btnGetLaser.Click += new System.EventHandler(this.btnGetLaser_Click);
            // 
            // txtX
            // 
            this.txtX.Location = new System.Drawing.Point(44, 36);
            this.txtX.Name = "txtX";
            this.txtX.Size = new System.Drawing.Size(54, 34);
            this.txtX.TabIndex = 75;
            this.txtX.Text = "0";
            // 
            // grbMode
            // 
            this.grbMode.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grbMode.Controls.Add(this.btnIdleMode);
            this.grbMode.Controls.Add(this.btnWorkMode);
            this.grbMode.Controls.Add(this.btnMapMode);
            this.grbMode.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.grbMode.Location = new System.Drawing.Point(12, 466);
            this.grbMode.Name = "grbMode";
            this.grbMode.Size = new System.Drawing.Size(341, 138);
            this.grbMode.TabIndex = 74;
            this.grbMode.TabStop = false;
            this.grbMode.Text = "Mode";
            // 
            // btnIdleMode
            // 
            this.btnIdleMode.Enabled = false;
            this.btnIdleMode.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnIdleMode.Location = new System.Drawing.Point(243, 34);
            this.btnIdleMode.Margin = new System.Windows.Forms.Padding(4);
            this.btnIdleMode.Name = "btnIdleMode";
            this.btnIdleMode.Size = new System.Drawing.Size(90, 85);
            this.btnIdleMode.TabIndex = 58;
            this.btnIdleMode.Text = "Idle Mode";
            this.btnIdleMode.UseVisualStyleBackColor = true;
            this.btnIdleMode.Click += new System.EventHandler(this.btnIdleMode_Click);
            // 
            // btnWorkMode
            // 
            this.btnWorkMode.Enabled = false;
            this.btnWorkMode.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnWorkMode.Location = new System.Drawing.Point(125, 34);
            this.btnWorkMode.Margin = new System.Windows.Forms.Padding(4);
            this.btnWorkMode.Name = "btnWorkMode";
            this.btnWorkMode.Size = new System.Drawing.Size(90, 85);
            this.btnWorkMode.TabIndex = 57;
            this.btnWorkMode.Text = "Work Mode";
            this.btnWorkMode.UseVisualStyleBackColor = true;
            this.btnWorkMode.Click += new System.EventHandler(this.btnWorkMode_Click);
            // 
            // btnMapMode
            // 
            this.btnMapMode.Enabled = false;
            this.btnMapMode.Font = new System.Drawing.Font("微軟正黑體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnMapMode.Location = new System.Drawing.Point(7, 34);
            this.btnMapMode.Margin = new System.Windows.Forms.Padding(4);
            this.btnMapMode.Name = "btnMapMode";
            this.btnMapMode.Size = new System.Drawing.Size(90, 85);
            this.btnMapMode.TabIndex = 56;
            this.btnMapMode.Text = "Map Mode";
            this.btnMapMode.UseVisualStyleBackColor = true;
            this.btnMapMode.Click += new System.EventHandler(this.btnMapMode_Click);
            // 
            // SocketTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 848);
            this.Controls.Add(this.grbMode);
            this.Controls.Add(this.btnConnectAgv);
            this.Controls.Add(this.gpbShift);
            this.Controls.Add(this.grbMap);
            this.Controls.Add(this.grbInfo);
            this.Controls.Add(this.txtConsole);
            this.Controls.Add(this.grbClient);
            this.Controls.Add(this.grbServer);
            this.Name = "SocketTest";
            this.Text = "SocketTest";
            this.Load += new System.EventHandler(this.SocketTest_Load);
            this.grbServer.ResumeLayout(false);
            this.grbServer.PerformLayout();
            this.grbClient.ResumeLayout(false);
            this.grbClient.PerformLayout();
            this.gpbShift.ResumeLayout(false);
            this.gpbShift.PerformLayout();
            this.grbMap.ResumeLayout(false);
            this.grbInfo.ResumeLayout(false);
            this.grbInfo.PerformLayout();
            this.grbMode.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CtGroupBox grbServer;
        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.Label lbPort;
        private System.Windows.Forms.TextBox txtServerPort;
        private CtGroupBox grbClient;
        private System.Windows.Forms.Button btnReconnect;
        private System.Windows.Forms.Label lbClientPort;
        private System.Windows.Forms.TextBox txtRemotePort;
        private System.Windows.Forms.Label lbIP;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnConnectAgv;
        private System.Windows.Forms.Button btnSetVelo;
        private System.Windows.Forms.Label lbVelocity;
        private System.Windows.Forms.TextBox txtVelocity;
        private System.Windows.Forms.Button btnServoOnOff;
        private CtGroupBox gpbShift;
        private System.Windows.Forms.Button btnLeft;
        private CtGroupBox grbMap;
        private System.Windows.Forms.Button btnGetMap;
        private System.Windows.Forms.Button btnSendMap;
        private System.Windows.Forms.Button btnPosConfirm;
        private System.Windows.Forms.Button btnGetOri;
        private System.Windows.Forms.Button btnGetCarStatus;
        private CtGroupBox grbInfo;
        private System.Windows.Forms.Button btnGetLaser;
        private CtGroupBox grbMode;
        private System.Windows.Forms.Button btnIdleMode;
        private System.Windows.Forms.Button btnWorkMode;
        private System.Windows.Forms.Button btnMapMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTheta;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtY;
    }
}