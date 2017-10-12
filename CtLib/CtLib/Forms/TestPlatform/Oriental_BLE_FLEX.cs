using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Oriental;

namespace CtLib.Forms.TestPlatform {
    /// <summary>Test platform of Oriental Brushless Motor and Driver packages</summary>
    public partial class Oriental_BLE_FLEX : Form {

        private CtOriental_BLE mOriental = new CtOriental_BLE();
        private byte mDevID = 1;

        /// <summary>Constructor of Oriental Motors</summary>
        public Oriental_BLE_FLEX() {
            InitializeComponent();

            mOriental.OnOrientalEvents += mOriental_OnOrientalEvents;
            cbAIMode.SelectedIndex = 0;
            cbFunc.SelectedIndex = 0;
        }

        private void mOriental_OnOrientalEvents(object sender, BleEventArgs e) {
            switch (e.Event) {
                case BleEvents.Connection:
                    if (CtConvert.CBool(e.Value)) {
                        CtInvoke.PictureBoxImage(picConStt, Properties.Resources.Green_Ball);
                        CtInvoke.ControlText(btnConnect, "Disconnect");
                        ShowMessage("Connected");
                    } else {
                        CtInvoke.PictureBoxImage(picConStt, Properties.Resources.Grey_Ball);
                        CtInvoke.ControlText(btnConnect, "Connect");
                        ShowMessage("Disconnected");
                    }
                    break;
                case BleEvents.DeviceError:
                    ShowMessage("[Device Error] " + string.Join(" ", e.Value as List<byte>));
                    break;
                case BleEvents.CommunicationError:
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

        private void btnConnect_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) mOriental.Disconnect();
            else {
                mOriental.DeviceID = mDevID;
                if (mOriental.Connect() == Stat.SUCCESS) {
                    CtOriental_BLE.AnalogyInputMode? mode = mOriental.GetAnalogyInputMode();
                    cbAIMode.SelectedIndex = mode.HasValue? (int)mode.Value : 0;
                    
                    bool? delayStop = mOriental.GetStopMode();
                    chkStopMode.Checked = delayStop.HasValue? delayStop.Value : false;
                }
            }
        }

        private void nudDevID_ValueChanged(object sender, EventArgs e) {
            mDevID = (byte)nudDevID.Value;
        }

        private void btnGetAlarm_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                int? alrmCode = mOriental.GetAlarmCode();
                int? warnCode = mOriental.GetWarningCode();
                int? comCode = mOriental.GetCommunicateErrorCode();
                CtInvoke.ControlText(txtAlarm, alrmCode.HasValue ? alrmCode.Value.ToString() : "?");
                CtInvoke.ControlText(txtComAlarm, comCode.HasValue ? comCode.Value.ToString() : "?");
                CtInvoke.ControlText(txtWarn, warnCode.HasValue ? warnCode.Value.ToString() : "?");
            }
        }

        private void btnGetSpeed_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                double? speed = mOriental.GetMotorVelocity();
                CtInvoke.ControlText(txtSpeed, speed.HasValue ? speed.Value.ToString() : "?");
            }
        }

        private void btnGetBeltSpeed_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                double? speed = mOriental.GetBeltVelocity();
                CtInvoke.ControlText(txtBeltSpeed, speed.HasValue ? speed.Value.ToString() : "?");
            }
        }

        private void Oriental_BLE_FLEX_FormClosing(object sender, FormClosingEventArgs e) {
            if (mOriental != null && mOriental.IsConnected) mOriental.Disconnect();
        }

        private void btnGetLoad_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                int? torque = mOriental.GetLoad();
                CtInvoke.ControlText(txtLoad, torque.HasValue ? torque.Value.ToString() : "?");
            }
        }

        private void btnGetASpeed_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                int? speed = mOriental.GetAnalogyVelocity();
                CtInvoke.ControlText(txtAnalogSpeed, speed.HasValue ? speed.Value.ToString() : "?");
            }
        }

        private void btnGetATorq_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                int? torque = mOriental.GetAnalogyTorqueLimit();
                CtInvoke.ControlText(txtATorq, torque.HasValue ? torque.Value.ToString() : "?");
            }
        }

        private void btnGetAVolt_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                int? volt = mOriental.GetAnalogyVoltage();
                CtInvoke.ControlText(txtAVolt, volt.HasValue ? volt.Value.ToString() : "?");
            }
        }

        private void btnGetIO_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                CtOriental_BLE.IOState ioStt = mOriental.GetIOState();
                if (ioStt != null) {
                    Image imgGreen = Properties.Resources.Green_Ball;
                    Image imgGray = Properties.Resources.Grey_Ball;
                    CtInvoke.PictureBoxImage(picIn0, ioStt.IN_0 ? imgGreen : imgGray);
                    CtInvoke.PictureBoxImage(picIn1, ioStt.IN_1 ? imgGreen : imgGray);
                    CtInvoke.PictureBoxImage(picIn2, ioStt.IN_2 ? imgGreen : imgGray);
                    CtInvoke.PictureBoxImage(picIn3, ioStt.IN_3 ? imgGreen : imgGray);
                    CtInvoke.PictureBoxImage(picIn4, ioStt.IN_4 ? imgGreen : imgGray);
                    CtInvoke.PictureBoxImage(picIn5, ioStt.IN_5 ? imgGreen : imgGray);
                    CtInvoke.PictureBoxImage(picIn6, ioStt.IN_6 ? imgGreen : imgGray);
                    CtInvoke.PictureBoxImage(picOut0, ioStt.OUT_0 ? imgGreen : imgGray);
                    CtInvoke.PictureBoxImage(picOut1, ioStt.OUT_1 ? imgGreen : imgGray);
                }
            }
        }


        private void btnSetAIMode_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                CtOriental_BLE.AnalogyInputMode mode = (CtOriental_BLE.AnalogyInputMode)Enum.Parse(typeof(CtOriental_BLE.AnalogyInputMode), cbAIMode.Text);
                mOriental.SetAnalogyInputMode(mode);
            }
        }

        private void btnCleanAlarm_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                mOriental.CleanAlarm();
            }
        }

        private void btnGetSpdList_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                List<int> speed = mOriental.GetVelocity(0, 16);
                if (speed != null && speed.Count > 0) {
                    int index = 0;
                    List<string> dgvStr = new List<string>();
                    CtInvoke.DataGridViewClear(dgvSpeed);
                    foreach (int item in speed) {
                        dgvStr.Clear();
                        dgvStr.Add(index++.ToString());
                        dgvStr.Add(item.ToString());
                        CtInvoke.DataGridViewAddRow(dgvSpeed, dgvStr, false);
                    }
                }
            }
        }

        private void btnGetAccelList_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                List<int> speed = mOriental.GetAccel(0, 16);
                if (speed != null && speed.Count > 0) {
                    int index = 0;
                    List<string> dgvStr = new List<string>();
                    CtInvoke.DataGridViewClear(dgvAccel);
                    foreach (int item in speed) {
                        dgvStr.Clear();
                        dgvStr.Add(index++.ToString());
                        dgvStr.Add(item.ToString());
                        CtInvoke.DataGridViewAddRow(dgvAccel, dgvStr, false);
                    }
                }
            }
        }

        private void btnGetDecelList_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                List<int> speed = mOriental.GetDecel(0, 16);
                if (speed != null && speed.Count > 0) {
                    int index = 0;
                    List<string> dgvStr = new List<string>();
                    CtInvoke.DataGridViewClear(dgvDecel);
                    foreach (int item in speed) {
                        dgvStr.Clear();
                        dgvStr.Add(index++.ToString());
                        dgvStr.Add(item.ToString());
                        CtInvoke.DataGridViewAddRow(dgvDecel, dgvStr, false);
                    }
                }
            }
        }

        private void btnGetToqLimt_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                List<int> speed = mOriental.GetTorqueLimit(0, 16);
                if (speed != null && speed.Count > 0) {
                    int index = 0;
                    List<string> dgvStr = new List<string>();
                    CtInvoke.DataGridViewClear(dgvTorque);
                    foreach (int item in speed) {
                        dgvStr.Clear();
                        dgvStr.Add(index++.ToString());
                        dgvStr.Add(item.ToString());
                        CtInvoke.DataGridViewAddRow(dgvTorque, dgvStr, false);
                    }
                }
            }
        }

        private void btnSetSpeedList_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                List<ushort> value = new List<ushort>();
                foreach (DataGridViewRow item in dgvSpeed.Rows) {
                    value.Add(ushort.Parse(item.Cells[1].Value as string));
                }
                mOriental.SetVelocity(0, value);
            }
        }

        private void btnSetAccelList_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                List<ushort> value = new List<ushort>();
                foreach (DataGridViewRow item in dgvAccel.Rows) {
                    value.Add(ushort.Parse(item.Cells[1].Value as string));
                }
                mOriental.SetAccel(0, value);
            }
        }

        private void btnSetDecelList_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                List<ushort> value = new List<ushort>();
                foreach (DataGridViewRow item in dgvDecel.Rows) {
                    value.Add(ushort.Parse(item.Cells[1].Value as string));
                }
                mOriental.SetDecel(0, value);
            }
        }

        private void btnSetToqLimt_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                List<ushort> value = new List<ushort>();
                foreach (DataGridViewRow item in dgvTorque.Rows) {
                    value.Add(ushort.Parse(item.Cells[1].Value as string));
                }
                mOriental.SetTorqueLimit(0, value);
            }
        }

        private void btnJogFWD_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                mOriental.Forward();
            }
        }

        private void btnJogREV_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                mOriental.Reverse();
            }
        }

        private void btnJogStop_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                btnJogFWD.Click -= btnJogFWD_Click;
                mOriental.DeviceID = mDevID;
                mOriental.MotionStop();
                btnJogFWD.Click += btnJogFWD_Click;
            }
        }

        private void chkStopMode_CheckedChanged(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                mOriental.SetStopMode(chkStopMode.Checked);
            }
        }

        private void btnGetOMap_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                Dictionary<CtOriental_BLE.Func_OUT, CtOriental_BLE.Param_OUT> value = mOriental.GetFuncMap_Out();

                List<string> temp = new List<string>();
                foreach (KeyValuePair<CtOriental_BLE.Func_OUT, CtOriental_BLE.Param_OUT> item in value) {
                    temp.Add(item.Key.ToString() + " = " + item.Value.ToString());
                }
                MessageBox.Show(string.Join("\n", temp));
            }
        }

        private void btnGetIMap_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                Dictionary<CtOriental_BLE.Func_IN, CtOriental_BLE.Param_IN> value = mOriental.GetFuncMap_In();

                List<string> temp = new List<string>();
                foreach (KeyValuePair<CtOriental_BLE.Func_IN, CtOriental_BLE.Param_IN> item in value) {
                    temp.Add(item.Key.ToString() + " = " + item.Value.ToString());
                }
                MessageBox.Show(string.Join("\n", temp));
            }
        }

        private void btnGetNOMap_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                Dictionary<CtOriental_BLE.Func_NET_OUT, CtOriental_BLE.Param_NET_OUT> value = mOriental.GetFuncMap_NET_OUT();

                List<string> temp = new List<string>();
                foreach (KeyValuePair<CtOriental_BLE.Func_NET_OUT, CtOriental_BLE.Param_NET_OUT> item in value) {
                    temp.Add(item.Key.ToString() + " = " + item.Value.ToString());
                }
                MessageBox.Show(string.Join("\n", temp));
            }
        }

        private void btnGetNIMap_Click(object sender, EventArgs e) {
            if (mOriental.IsConnected) {
                mOriental.DeviceID = mDevID;
                Dictionary<CtOriental_BLE.Func_NET_IN, CtOriental_BLE.Param_NET_IN> value = mOriental.GetFuncMap_NET_IN();

                List<string> temp = new List<string>();
                foreach (KeyValuePair<CtOriental_BLE.Func_NET_IN, CtOriental_BLE.Param_NET_IN> item in value) {
                    temp.Add(item.Key.ToString() + " = " + item.Value.ToString());
                }
                MessageBox.Show(string.Join("\n", temp));
            }
        }

        private void cbFunc_SelectedIndexChanged(object sender, EventArgs e) {
            CtInvoke.ComboBoxClear(cbParam);
            CtInvoke.ComboBoxClear(cbFuncList);
            switch (cbFunc.SelectedIndex) {
                case 0:
                    CtInvoke.ComboBoxAdd(cbParam, Enum.GetValues(typeof(CtOriental_BLE.Param_IN)).Cast<CtOriental_BLE.Param_IN>());
                    CtInvoke.ComboBoxAdd(cbFuncList, Enum.GetValues(typeof(CtOriental_BLE.Func_IN)).Cast<CtOriental_BLE.Func_IN>());
                    break;
                case 1:
                    CtInvoke.ComboBoxAdd(cbParam, Enum.GetValues(typeof(CtOriental_BLE.Param_OUT)).Cast<CtOriental_BLE.Param_OUT>());
                    CtInvoke.ComboBoxAdd(cbFuncList, Enum.GetValues(typeof(CtOriental_BLE.Func_OUT)).Cast<CtOriental_BLE.Func_OUT>());
                    break;
                case 2:
                    CtInvoke.ComboBoxAdd(cbParam, Enum.GetValues(typeof(CtOriental_BLE.Param_NET_IN)).Cast<CtOriental_BLE.Param_NET_IN>());
                    CtInvoke.ComboBoxAdd(cbFuncList, Enum.GetValues(typeof(CtOriental_BLE.Func_NET_IN)).Cast<CtOriental_BLE.Func_NET_IN>());
                    break;
                case 3:
                    CtInvoke.ComboBoxAdd(cbParam, Enum.GetValues(typeof(CtOriental_BLE.Param_NET_OUT)).Cast<CtOriental_BLE.Param_NET_OUT>());
                    CtInvoke.ComboBoxAdd(cbFuncList, Enum.GetValues(typeof(CtOriental_BLE.Func_NET_OUT)).Cast<CtOriental_BLE.Func_NET_OUT>());
                    break;
                default:
                    break;
            }
            CtInvoke.ComboBoxSelectedIndex(cbParam, 0);
            CtInvoke.ComboBoxSelectedIndex(cbFuncList, 0);
        }

        private void btnSetIO_Click(object sender, EventArgs e) {
            switch (cbFunc.SelectedIndex) {
                case 0:
                    mOriental.SetFuncMap_IN(
                        (CtOriental_BLE.Func_IN)Enum.Parse(typeof(CtOriental_BLE.Func_IN), cbFuncList.Text),
                        (CtOriental_BLE.Param_IN)Enum.Parse(typeof(CtOriental_BLE.Param_IN), cbParam.Text)
                    );
                    break;
                case 1:
                    mOriental.SetFuncMap_OUT(
                        (CtOriental_BLE.Func_OUT)Enum.Parse(typeof(CtOriental_BLE.Func_OUT), cbFuncList.Text),
                        (CtOriental_BLE.Param_OUT)Enum.Parse(typeof(CtOriental_BLE.Param_OUT), cbParam.Text)
                    );
                    break;
                case 2:
                    mOriental.SetFuncMap_NET_IN(
                        (CtOriental_BLE.Func_NET_IN)Enum.Parse(typeof(CtOriental_BLE.Func_NET_IN), cbFuncList.Text),
                        (CtOriental_BLE.Param_NET_IN)Enum.Parse(typeof(CtOriental_BLE.Param_NET_IN), cbParam.Text)
                    );
                    break;
                case 3:
                    mOriental.SetFuncMap_NET_OUT(
                        (CtOriental_BLE.Func_NET_OUT)Enum.Parse(typeof(CtOriental_BLE.Func_NET_OUT), cbFuncList.Text),
                        (CtOriental_BLE.Param_NET_OUT)Enum.Parse(typeof(CtOriental_BLE.Param_NET_OUT), cbParam.Text)
                    );
                    break;
                default:
                    break;
            }
        }
    }
}
