using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Delta;
using CtLib.Module.Utility;

namespace CtLib.Forms.TestPlatform {
    /// <summary>Delta ASDA-A2 Test Platform</summary>
    public partial class Test_Delta_ASDA_A2 : Form {

        private CtDelta_ASDA_A2 mASDA = new CtDelta_ASDA_A2();

        /// <summary>Constructors of Delta ASDA-A2 Test Platform</summary>
        public Test_Delta_ASDA_A2() {
            InitializeComponent();

            mASDA.On_ASDA_A2_Events += mASDA_On_ASDA_A2_Events;

            cbPositionUnit.SelectedIndex = 2;
            cbCruiseUnit.SelectedIndex = 1;
        }

        private void mASDA_On_ASDA_A2_Events(object sender, ASDA_A2_EventArgs e) {
            switch (e.Event) {
                case ASDA_A2_Events.Connection:
                    if (CtConvert.CBool(e.Value)) {
                        CtInvoke.PictureBoxImage(pbOpen, Properties.Resources.Green_Ball);
                        CtInvoke.ControlText(btnOpen, "Disconnect");
                        ShowMessage("Connected");
                    } else {
                        CtInvoke.PictureBoxImage(pbOpen, Properties.Resources.Grey_Ball);
                        CtInvoke.ControlText(btnOpen, "Connect");
                        ShowMessage("Disconnected");
                    }
                    break;
                case ASDA_A2_Events.DeviceError:
                    ShowMessage("[Device Error] " + string.Join(" ", e.Value as List<byte>));
                    break;
                case ASDA_A2_Events.CommunicationError:
                    ShowMessage("[Communication Error] " + e.Value as string);
                    break;
                default:
                    break;
            }
        }

        private void ShowMessage(string msg) {
            string temp = DateTime.Now.ToString("[HH:mm:ss.ff] ") + msg;
            CtInvoke.ListBoxInsert(lsbxMsg, 0, temp);
        }

        private void CheckServo() {
            bool? servo = mASDA.GetServo();
            if (servo.HasValue)
                CtInvoke.PictureBoxImage(pbServo, servo.Value ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
        }

        private void btnOpen_Click(object sender, EventArgs e) {
            if (mASDA.IsConnected) mASDA.Disconnect();
            else {
                byte deviceID = byte.Parse(txtDevID.Text);
                mASDA.DeviceID = deviceID;

                mASDA.Connect();
                CheckServo();
            }
        }

        private void btnSpeed_Click(object sender, EventArgs e) {
            double? speed = mASDA.GetVelocity();
            if (speed.HasValue) CtInvoke.ControlText(txtSpeed, speed.ToString());
            else CtInvoke.ControlText(txtSpeed, "0");
        }

        private void btnServo_Click(object sender, EventArgs e) {
            bool? servo = mASDA.GetServo();
            if (servo.HasValue) {
                CtTimer.Delay(10);
                mASDA.Servo(!servo.Value);
                CtTimer.Delay(10);
                CheckServo();
            }

        }

        private void JogStart(object sender, MouseEventArgs e) {
            CtDelta_ASDA_A2.JogDirection jogDIr = (CtDelta_ASDA_A2.JogDirection)Enum.Parse(typeof(CtDelta_ASDA_A2.JogDirection), (sender as Button).Tag.ToString());
            mASDA.JogStart(jogDIr, ushort.Parse(txtJogSpeed.Text));
        }

        private void JogStop(object sender, MouseEventArgs e) {
            mASDA.JogStop();
        }

        private void Test_Delta_ASDA_A2_FormClosing(object sender, FormClosingEventArgs e) {
            mASDA.Dispose();
        }

        private void btnGetPosition_Click(object sender, EventArgs e) {
            int? value = mASDA.GetPosition((CtDelta_ASDA_A2.Positions)Enum.Parse(typeof(CtDelta_ASDA_A2.Positions), cbPositionUnit.Text));
            if (value.HasValue) CtInvoke.ControlText(txtPosition, value.ToString());
            else CtInvoke.ControlText(txtPosition, "?");
        }

        private void btnGetError_Click(object sender, EventArgs e) {
            ushort? value = mASDA.GetErrorCode();
            if (value.HasValue) CtInvoke.ControlText(txtError, value.ToString());
            else CtInvoke.ControlText(txtError, "?");
        }

        private void btnHome_Click(object sender, EventArgs e) {
            mASDA.Home(1,(ushort)60, CtDelta_ASDA_A2.HomeLimit.InverseDirection, CtDelta_ASDA_A2.HomeZSignal.Backward, CtDelta_ASDA_A2.HomeMode.ORG_R_TRIG_CCW);
            MessageBox.Show("Home Done!");
        }

        private void btnGO_Click(object sender, EventArgs e) {
            mASDA.SetRampTime(1, (ushort)nudAccel.Value);
            mASDA.SetRampTime(2, (ushort)nudDecel.Value);
            mASDA.SetSpeed(1, (ushort)nudSpeed.Value);
            mASDA.MoveTo(int.Parse(txtTarget.Text), 1, 2, 1, 0, true, false, CtDelta_ASDA_A2.MotionCommand.Absolute);
            mASDA.WaitMoveDone();
            MessageBox.Show("Motion Done!");
        }

        private void btnStop_Click(object sender, EventArgs e) {
            mASDA.MotionStop();
        }

        private void btnCruise_Click(object sender, EventArgs e) {
            mASDA.SetRampTime(3, (ushort)nudAccel_Cruise.Value);
            mASDA.SetRampTime(4, (ushort)nudDecel_Cruise.Value);
            CtDelta_ASDA_A2.Units unit = cbCruiseUnit.SelectedIndex == 0 ? CtDelta_ASDA_A2.Units.PulsePerSecond : CtDelta_ASDA_A2.Units.RotationPerMinute;
            mASDA.Cruise(int.Parse(txtSpeed_Cruise.Text), 3, 4, 0, true, false, unit);
        }

        private void btnGetDoStt_Click(object sender, EventArgs e) {
            CtDelta_ASDA_A2.DigitalOutputSignal signal = mASDA.GetDriverDOState();
            if (signal != null) {
                Image imgGreen = Properties.Resources.Green_Ball;
                Image imgGray = Properties.Resources.Grey_Ball;
                CtInvoke.PictureBoxImage(picSRDY, signal.ServoReady ? imgGreen : imgGray);
                CtInvoke.PictureBoxImage(picSON, signal.ServoON ? imgGreen : imgGray);
                CtInvoke.PictureBoxImage(picZSPD, signal.ZeroSpeed ? imgGreen : imgGray);
                CtInvoke.PictureBoxImage(picTSPD, signal.SpeedReached ? imgGreen : imgGray);
                CtInvoke.PictureBoxImage(picTPOS, signal.PositionReached ? imgGreen : imgGray);
                CtInvoke.PictureBoxImage(picTQL, signal.TorqueLimited ? imgGreen : imgGray);
                CtInvoke.PictureBoxImage(picALRM, signal.Alarm ? imgGreen : imgGray);
                CtInvoke.PictureBoxImage(picBRKR, signal.Breaker ? imgGreen : imgGray);
                CtInvoke.PictureBoxImage(picHOME, signal.Home ? imgGreen : imgGray);
                CtInvoke.PictureBoxImage(picOLW, signal.Overload ? imgGreen : imgGray);
                CtInvoke.PictureBoxImage(picWARN, signal.Warning ? imgGreen : imgGray); 
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            mASDA.CleanError();
        }

		private void btnRelat_Click(object sender, EventArgs e) {
			mASDA.SetRampTime(1, (ushort)nudAccel.Value);
			mASDA.SetRampTime(2, (ushort)nudDecel.Value);
			mASDA.SetSpeed(1, (ushort)nudSpeed.Value);
			mASDA.MoveTo(int.Parse(txtTarget.Text), 1, 2, 1, 0, true, false, CtDelta_ASDA_A2.MotionCommand.Relative);
			mASDA.WaitMoveDone();
			MessageBox.Show("Motion Done!");
		}

		private void btnGetSDO_Click(object sender, EventArgs e) {
			var doStt = mASDA.GetSDO();
			var idx = 0;
			var doStr = doStt.Select(
				o => {
					idx++;
					return $"[{idx}] {(o? "ON" : "OFF")}";
				}
			);
			MessageBox.Show(string.Join("\r\n", doStr));
		}

		private void btnGetDI_Click(object sender, EventArgs e) {
			var diStt = mASDA.GetDI();
			var idx = 0;
			var diStr = diStt.Select(
				o => {
					idx++;
					return $"[{idx}] {(o ? "ON" : "OFF")}";
				}
			);
			MessageBox.Show(string.Join("\r\n", diStr));
		}

		private void btnSetSDO_Click(object sender, EventArgs e) {
			string str;
			CtInput.Text(out str, "Software Digital Output", "請輸入編號與狀態，如 \"3, True\" 表示 DO #4 設定為 High");
			if (!string.IsNullOrEmpty(str)) {
				var split = str.Split(',').Select(s => s.Trim());
				if (split.Count() == 2) {
					int ioNum = int.Parse(split.ElementAt(0));
					bool stt = bool.Parse(split.ElementAt(1));
					mASDA.SetSDO(ioNum, stt);
				}
			}
		}
	}
}
