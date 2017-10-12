using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Adept;
using CtLib.Module.Utility;

namespace CtLib.Forms.TestPlatform {
    /// <summary>Test Platform of CtACE</summary>
	[Serializable]
    public partial class Test_ACE : Form, ICtVersion {

        #region Version

        /// <summary>Test_ACE 版本訊息</summary>
        public CtVersion Version { get { return new CtVersion(1, 1, 0, "2016/10/18", "Ahern Kuo"); } }

        /*---------- Version Note ----------
         * 
         * 1.0.0	Ahern	[2014/09/12]
         *		+ 完成初版介面
         *      
         * 1.0.1	Ahern	[2014/09/19]
         *		+ CtAceVisionWindow
		 *      
		 * 1.1.0	Ahern	[2016/10/18]
		 *		+ Motion
		 *		\ 調整版面
         *      
         *----------------------------------*/

        #endregion

        #region Declaration - Fields
        /// <summary>Reference of CtAce</summary>
        private CtAce rAce;
        ///<summary>手臂位置監看物件</summary>
        private CtRobotMonitor monitor = null;
        #endregion

        #region Function - Constructor

        /// <summary>Constructor, Initial the form component and objects</summary>
        public Test_ACE() {
            InitializeComponent();

			tabVision.SelectedIndex = 1;
        }
        #endregion

        #region Function - Method

        /// <summary>Set enable of Control and its sub-control</summary>
        /// <param name="ctrl">The Control object that want to setting</param>
        /// <param name="enable">Enabled</param>
        private void EnableControl(Control ctrl, bool enable) {
            if (ctrl.HasChildren) {
                foreach (Control subCtrl in ctrl.Controls) {
                    EnableControl(subCtrl, enable);
                }
            } else {
                CtInvoke.ControlEnabled(ctrl,enable);
            }
        }

        /// <summary>Handle the message that showing to the DataGridView</summary>
        /// <param name="info">message that want to display</param>
        private void ShowMessage(string info) {
            List<string> strTemp = new List<string> { System.DateTime.Now.ToString("HH:mm:ss.ff"), info };
            CtInvoke.DataGridViewAddRow(dgvInfo, strTemp);
        }

        /// <summary>Handle the message that showing to the DataGridView</summary>
        /// <param name="info">message that want to display</param>
        /// <param name="bgColor">Line Color</param>
        private void ShowMessage(string info, Color bgColor) {
            List<string> strTemp = new List<string> { System.DateTime.Now.ToString("HH:mm:ss.ff"), info };
            CtInvoke.DataGridViewAddRow(dgvInfo, strTemp, bgColor);
        }

        #endregion

        #region Function - CtAce Events

        /// <summary>Handle CtAce OnMessage event, display the message that send from CtAce object</summary>
        void rAce_OnMessage(object sender, AceMessageEventArgs e) {
            /* According the message type, choose the different background color */
            Color bgColor = Color.Black;
            switch (e.Type) {
                case -1:
                    bgColor = Color.Red;
                    break;
                case 0:
                    bgColor = Color.White;
                    break;
                case 1:
                    bgColor = Color.Yellow;
                    break;
            }

            /* Combine the message of Title and Content */
            string strTemp = "[" + e.Title + "] - " + e.Message;

            /* Request show message function to display on DataGridView */
            ShowMessage(strTemp, bgColor);
        }

        /// <summary>Handle CtAce event, Task status changed</summary>
        void rAce_TaskEventOccur(object sender, CtAceTask.TaskEventArgs e) {
            /* You can do something when state changed, just show a message to DataGridView here  */
            ShowMessage("Task" + e.TaskNumber + ": " + e.TaskStatus.ToString());
        }

        /// <summary>Handle CtACe OnNotifyEventChanged event</summary>
        void rAce_NotifyEventOccur(object sender, AceNotifyEventArgs e) {
            /* You cna do something for each event, just show a messsage to DataGridView here */
            switch (e.Events) {
                case AceNotifyEvents.WorkspaceLoaded:
                    ShowMessage("Workspace載入");
                    break;
                case AceNotifyEvents.WorkspaceSaved:
                    ShowMessage("Workspace已儲存");
                    break;
                case AceNotifyEvents.WorkspaceUnloaded:
                    ShowMessage("Workspace卸載");
                    break;
                case AceNotifyEvents.ProgramModified:
                    ShowMessage("有程式受到變更");
                    break;
                case AceNotifyEvents.AceShutdown:
                    ShowMessage("Adept ACE 已關閉");
                    break;
                default:
                    break;
            }
        }

        /// <summary>Handle CtAce OnNumericEventChanged event</summary>
        void rAce_NumericEventOccur(object sender, AceNumericEventArgs e) {
            /* show the message */
            switch (e.Events) {
                case AceNumericEvents.SpeedChanged:
                    ShowMessage("速度變更為: " + CtConvert.CStr(e.Value));
                    CtInvoke.ControlText(txtSpeed, CtConvert.CStr(e.Value));
                    break;
				case AceNumericEvents.Contents:
					ShowMessage("物件變更: " + e.Value.ToString());
					break;
                default:
                    break;
            }
        }

        /// <summary>Handle CtAce OnBoolEventChanged event</summary>
        void rAce_BoolEventOccur(object sender, AceBoolEventArgs e) {
            /* According the state to change item */
            switch (e.Events) {
                case AceBoolEvents.PowerChanged:
                    if (e.Value) {
                        CtInvoke.ControlTag(imgPower, true);
                        CtInvoke.PictureBoxImage(imgPower, Properties.Resources.Green_Ball);
                        ShowMessage("電源開啟");
                    } else {
                        CtInvoke.ControlTag(imgPower, false);
                        CtInvoke.PictureBoxImage(imgPower, Properties.Resources.Grey_Ball);
                        ShowMessage("電源已關閉");
                    }
                    break;
				case AceBoolEvents.Calibration:
					if (e.Value) {
						ShowMessage("已進行校正");
					} else {
						ShowMessage("校正失效");
					}
					break;
                case AceBoolEvents.Connection:
                    if (e.Value) {
                        rAce.RequestPower();
                        rAce.RequestSpeed();

                        foreach (Control ctrl in this.Controls) {
                            EnableControl(ctrl, true);
                        }

                        CtInvoke.ControlEnabled(chkCtrl, false);
                        CtInvoke.ControlEnabled(btnConnect, true);
                        CtInvoke.ControlText(btnConnect, "中斷連線");
                        CtInvoke.ControlTag(btnConnect, true);

                        List<string> lstCtrl = rAce.VpLinks;
                        if (lstCtrl != null && lstCtrl.Count > 0) CtInvoke.ControlText(txtCtrl, lstCtrl);
                        List<string> lstRob = rAce.Robots;
                        if (lstRob != null && lstRob.Count > 0) CtInvoke.ControlText(txtRobot, lstRob);
                    } else {
                        foreach (Control ctrl in this.Controls) {
                            EnableControl(ctrl, false);
                        }

                        CtInvoke.ControlEnabled(chkCtrl, true);
                        CtInvoke.ControlEnabled(btnConnect, true);
                        CtInvoke.ControlText(btnConnect, "連線");
                        CtInvoke.ControlTag(btnConnect, false);
                        visionWindow.Disconnect();
                        visWindow.Disconnect();
                    }

                    break;
				default:
					break;
            }
        }

        #endregion

        #region Function - Form Events

        private void btnConnect_Click(object sender, EventArgs e) {
            if (CtConvert.CBool(btnConnect.Tag)) {
                if (rAce != null) {
                    rAce.Dispose();
                    rAce = null;
                }

            } else {
                ControllerType ctrlType = (chkCtrl.Checked) ? ControllerType.SmartController : ControllerType.Embedded;
                rAce = new CtAce();

                if (rAce != null) {
                    rAce.EnableMessage = true;
                    rAce.OnBoolEventChanged += rAce_BoolEventOccur;
                    rAce.OnNumericEventChanged += rAce_NumericEventOccur;
                    rAce.OnNotifyEventChanged += rAce_NotifyEventOccur;
                    rAce.OnTaskChanged += rAce_TaskEventOccur;
                    rAce.OnMessage += rAce_OnMessage;
                }

				//rAce.ClientMode = false;
				//rAce.WorkspacePath = @"D:\Codes\Testing Code\ACE_Empty\empty.awp";
                rAce.Connect(ctrlType, true);

                if (!rAce.IsVpConnected()) {
                    ShowMessage("連線失敗");
                }
            }
        }

        private void txtSpeed_TextChanged(object sender, EventArgs e) {
            rAce.SetSpeed(CtConvert.CInt(txtSpeed.Text));
        }

        private void imgPower_Click(object sender, EventArgs e) {
            if (CtConvert.CBool(imgPower.Tag)) {
                rAce.SetPower(false);
            } else {
                rAce.SetPower(true);
            }
        }

        private void txtSpeed_KeyPress(object sender, KeyPressEventArgs e) {
            if ((e.KeyChar != '\n') && (e.KeyChar != '\r')) {
                CtInvoke.ControlForeColor(txtSpeed, Color.Red);
                CtInvoke.ControlFont(txtSpeed, new Font("Consolas", 10, FontStyle.Bold));
            } else {
                if (CtConvert.CInt(txtSpeed.Text) < 101) {
                    CtInvoke.ControlForeColor(txtSpeed, Color.Black);
                    CtInvoke.ControlFont(txtSpeed, new Font("Consolas", 10, FontStyle.Regular));
                    if (rAce != null)
                        rAce.SetSpeed(CtConvert.CInt(txtSpeed.Text));
                }
            }
        }

        private void btnVarRead_Click(object sender, EventArgs e) {
			try {
				object objTemp;
				rAce.Variable.GetUndefVpValue(txtVarName.Text, out objTemp);
				if (objTemp != null) {
					string strTemp = CtConvert.CStr(objTemp);
					CtInvoke.ControlText(txtVarVal, strTemp);
				}
			} catch (Exception ex) {
				ShowMessage(ex.Message, Color.Red);
			}
        }

        private void Test_ACE_Load(object sender, EventArgs e) {

        }

        private void btnVarWrite_Click(object sender, EventArgs e) {
			try {
				if (txtVarVal.Text != "") {
					VPlusVariableType varType;
					rAce.Variable.GetVariabeType(txtVarName.Text, out varType);

					object objTemp = null;
					switch (varType) {
						case VPlusVariableType.Real:
							objTemp = CtConvert.CFloat(txtVarVal.Text);
							break;
						case VPlusVariableType.String:
							objTemp = txtVarVal.Text;
							break;
						case VPlusVariableType.Location:
							List<string> strSplit = txtVarVal.Text.Split(CtConst.CHR_DELIMITERS, StringSplitOptions.RemoveEmptyEntries).ToList();
							List<double> dblTemp = strSplit.ConvertAll(val => Convert.ToDouble(val));
							objTemp = dblTemp;
							break;
						case VPlusVariableType.PrecisionPoint:
							List<string> strSplitPP = txtVarVal.Text.Split(CtConst.CHR_DELIMITERS, StringSplitOptions.RemoveEmptyEntries).ToList();
							List<double> dblTempPP = strSplitPP.ConvertAll(val => Convert.ToDouble(val));
							objTemp = dblTempPP;
							break;
					}
					if (objTemp != null) {
						rAce.Variable.SetUndefVpValue(txtVarName.Text, objTemp);
					}
				}
			} catch (Exception ex) {
				ShowMessage(ex.Message, Color.Red);
			}
        }

        private void btnProgExe_Click(object sender, EventArgs e) {
            if (CtConvert.CBool(btnProgExe.Tag)) {
                rAce.Tasks.TaskAbort(cbProgTask.SelectedIndex);
                CtInvoke.ControlTag(btnProgExe, false);
                CtInvoke.ControlText(btnProgExe, "執行");
            } else {
                rAce.Tasks.TaskExecute(txtProgName.Text, cbProgTask.SelectedIndex);
                CtInvoke.ControlTag(btnProgExe, true);
                CtInvoke.ControlText(btnProgExe, "停止/暫停");
            }

        }

        private void btnProgProc_Click(object sender, EventArgs e) {
            rAce.Tasks.TaskProceed(cbProgTask.SelectedIndex);
        }

        private void btnProgKill_Click(object sender, EventArgs e) {
            rAce.Tasks.TaskKill(cbProgTask.SelectedIndex);
        }

        private void btnProgMoni_Click(object sender, EventArgs e) {
            if (CtConvert.CBool(btnProgMoni.Tag)) {
                rAce.Tasks.RemoveMonitorTask(cbProgTask.SelectedIndex);
                CtInvoke.ControlTag(btnProgMoni, false);
                CtInvoke.ControlText(btnProgMoni, "加入監控");
            } else {
                rAce.Tasks.AddMonitorTask(cbProgTask.SelectedIndex);
                CtInvoke.ControlTag(btnProgMoni, true);
                CtInvoke.ControlText(btnProgMoni, "停止監控");
            }
        }

        private void cbProgTask_SelectedValueChanged(object sender, EventArgs e) {
            if (rAce.Tasks.IsMoniting(cbProgTask.SelectedIndex)) {
                CtInvoke.ControlTag(btnProgMoni, true);
                CtInvoke.ControlText(btnProgMoni, "停止監控");
            } else {
                CtInvoke.ControlTag(btnProgMoni, false);
                CtInvoke.ControlText(btnProgMoni, "加入監控");
            }

            if (rAce.Tasks.IsTaskExist(cbProgTask.SelectedIndex)) {
                CtInvoke.ControlTag(btnProgExe, true);
                CtInvoke.ControlText(btnProgExe, "停止/暫停");
            } else {
                CtInvoke.ControlTag(btnProgExe, false);
                CtInvoke.ControlText(btnProgExe, "執行");
            }
        }

        private void btnModelCrt_Click(object sender, EventArgs e) {
            if ((txtModelPath.Text != null) && (txtLocPath.Text != null)) {
                CtAceModelEditor form = new CtAceModelEditor();
                form.Connect(rAce, txtLocPath.Text, txtModelPath.Text);
                form.Close();
                form.Dispose();
            }
        }

        private void btnVision_Click(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(txtVisionPath.Text)) {
				visionWindow.Disconnect();
                visionWindow.Connect(rAce, txtVisionPath.Text);
            }
        }

        private void btnAwpSave_Click(object sender, EventArgs e) {
            rAce.SaveWorkspace();
        }

        private void btnAwpSaveAs_Click(object sender, EventArgs e) {
            rAce.SaveWorkspaceAs();
        }

        private void btnAwpZero_Click(object sender, EventArgs e) {
            rAce.ZeroMemory();
        }

        private void btnShowVision_Click(object sender, EventArgs e) {
			visWindow.Disconnect();
            visWindow.Connect(rAce);
        }

        private void btnOpen_Click(object sender, EventArgs e) {
            int io = -1;
            if (int.TryParse(txtIO.Text.Replace("-", ""), out io)) {
				try {
					rAce.IO.SetIO(io);
				} catch (Exception ex) {
					ShowMessage(ex.Message, Color.Red);
				}
            }
        }

        private void btnClose_Click(object sender, EventArgs e) {
            int io = -1;
            if (int.TryParse(txtIO.Text.Replace("-", ""), out io)) {
				try {
					rAce.IO.SetIO(io * -1);
				} catch (Exception ex) {
					ShowMessage(ex.Message, Color.Red);
				}
            }
        }

        private void btn90P_Click(object sender, EventArgs e) {
            rAce.Motion.MoveDistance(1, Axis.Roll, 90);
        }

        private void btn90N_Click(object sender, EventArgs e) {
            rAce.Motion.MoveDistance(1, Axis.Roll, -90);
        }

        private void btnPendant_Click(object sender, EventArgs e) {
            rAce.Pendant(true);
        }

        private void btnCusCmd_Click(object sender, EventArgs e) {
            string temp;
			CtInput.Text(out temp, "自訂命令", "請輸入命令");
            //if (temp != "") rAce.Motion.SendString(temp);
        }

		private void btnExConnect_Click(object sender, EventArgs e) {
			rAce.ConnectWithController(!rAce.IsVpConnected());
		}

		private void btnVisBud_Click(object sender, EventArgs e) {
			using (CtAceVisionBuilder_Ctrl builder = new CtAceVisionBuilder_Ctrl(rAce)) {
				this.Hide();
				builder.ShowDialog();
				this.Show();
			}
		}

		private void btnRdIO_Click(object sender, EventArgs e) {
			int io;
			if (int.TryParse(txtIO.Text?.Replace("-", ""), out io)) {
				bool result = rAce.IO.GetIO(io);
				ShowMessage($"I/O ({io}) {(result? "ON" : "OFF")}", Color.LightSkyBlue);
			}
		}

		private void btnRdHere_Click(object sender, EventArgs e) {
			int robotNum;
			if (int.TryParse(txtMoRob.Text, out robotNum)) {
				List<double> location;
				try {
					rAce.Variable.GetHere(robotNum, out location);
					CtInvoke.ControlText(txtX, location[0].ToString("F2"));
					CtInvoke.ControlText(txtY, location[1].ToString("F2"));
					CtInvoke.ControlText(txtZ, location[2].ToString("F2"));
					CtInvoke.ControlText(txtYaw, location[3].ToString("F2"));
					CtInvoke.ControlText(txtPitch, location[4].ToString("F2"));
					CtInvoke.ControlText(txtRoll, location[5].ToString("F2"));
				} catch (Exception ex) {
					ShowMessage(ex.Message, Color.Red);
				}
			}
		}

		private void btnMove_Click(object sender, EventArgs e) {
			int robotNum;
			if (int.TryParse(txtMoRob.Text, out robotNum)) {
				List<double> location = new List<double>();
				try {
					location.Add(double.Parse(CtInvoke.ControlText(txtX)));
					location.Add(double.Parse(CtInvoke.ControlText(txtY)));
					location.Add(double.Parse(CtInvoke.ControlText(txtZ)));
					location.Add(double.Parse(CtInvoke.ControlText(txtYaw)));
					location.Add(double.Parse(CtInvoke.ControlText(txtPitch)));
					location.Add(double.Parse(CtInvoke.ControlText(txtRoll)));
					rAce.Motion.MoveToLocation(robotNum, location);
					rAce.Motion.WaitMoveDone(robotNum);
				} catch (Exception ex) {
					ShowMessage(ex.Message, Color.Red);
				}
			}
		}
        
        private void btnMonitor_Click(object sender, EventArgs e) {
            try {
                if (monitor == null) monitor = new CtRobotMonitor(rAce);
                monitor.Show();
            } catch(Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex, true);
            }

        }

        #endregion


    }
}
