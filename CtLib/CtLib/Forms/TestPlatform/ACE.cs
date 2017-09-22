using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Adept;

using Ace.Adept.Server.Controls;
using CtLib.Module.Ultity;

namespace CtLib.Forms.TestPlatform {
    /// <summary>Test Platform of CtACE</summary>
    public partial class Test_ACE : Form {

        #region Version

        /// <summary>Test_ACE 版本訊息</summary>
        public static readonly CtVersion @Version = new CtVersion(1, 0, 1, "2014/09/19", "Ahern Kuo");

        /*---------- Version Note ----------
         * 
         * 1.0.0  <Ahern> [2014/09/12]
         *      + 完成初版介面
         *      
         * 1.0.1  <Ahern> [2014/09/19]
         *      + CtAceVisionWindow
         *      
         *----------------------------------*/

        #endregion

        #region Declaration - Members
        /// <summary>Reference of CtAce</summary>
        private CtAce rAce;
        #endregion

        #region Function - Constructor

        /// <summary>Constructor, Initial the form component and objects</summary>
        public Test_ACE() {
            InitializeComponent();
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
                ctrl.Enabled = enable;
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
        void rAce_OnMessage(object sender, CtAce.MessageEventArgs e) {
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
        void rAce_NotifyEventOccur(object sender, CtAce.NotifyEventArgs e) {
            /* You cna do something for each event, just show a messsage to DataGridView here */
            switch (e.Events) {
                case CtAce.NotifyEvents.WORKSPACE_LOAD:
                    ShowMessage("Workspace載入");
                    break;
                case CtAce.NotifyEvents.WORKSPACE_SAVE:
                    ShowMessage("Workspace已儲存");
                    break;
                case CtAce.NotifyEvents.WORKSPACE_UNLOAD:
                    ShowMessage("Workspace卸載");
                    break;
                case CtAce.NotifyEvents.PROGRAM_MODIFIED:
                    ShowMessage("有程式受到變更");
                    break;
                case CtAce.NotifyEvents.ACE_SHUTDOWN:
                    ShowMessage("Adept ACE 已關閉");
                    break;
                default:
                    break;
            }
        }

        /// <summary>Handle CtAce OnNumericEventChanged event</summary>
        void rAce_NumericEventOccur(object sender, CtAce.NumericEventArgs e) {
            /* show the message */
            switch (e.Events) {
                case CtAce.NumericEvents.SPEED_CHANGED:
                    ShowMessage("速度變更為: " + CtConvert.CStr(e.Value));
                    CtInvoke.TextBoxText(txtSpeed, CtConvert.CStr(e.Value));
                    break;
                default:
                    break;
            }
        }

        /// <summary>Handle CtAce OnBoolEventChanged event</summary>
        void rAce_BoolEventOccur(object sender, CtAce.BoolEventArgs e) {
            /* According the state to change item */
            switch (e.Events) {
                case CtAce.BoolEvents.POWER_CHANGED:
                    if (e.Value) {
                        CtInvoke.PictureBoxTag(imgPower, true);
                        CtInvoke.PictureBoxImage(imgPower, Properties.Resources.Green_Ball);
                        ShowMessage("電源開啟");
                    } else {
                        CtInvoke.PictureBoxTag(imgPower, false);
                        CtInvoke.PictureBoxImage(imgPower, Properties.Resources.Grey_Ball);
                        ShowMessage("電源已關閉");
                    }
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

                foreach (Control ctrl in this.Controls) {
                    EnableControl(ctrl, false);
                }

                CtInvoke.CheckBoxEnable(chkCtrl, true);
                CtInvoke.ButtonEnable(btnConnect, true);
                CtInvoke.ButtonText(btnConnect, "連線");
                CtInvoke.ButtonTag(btnConnect, false);
            } else {

                CtAce.ControllerType ctrlType = (chkCtrl.Checked) ? CtAce.ControllerType.WITH_SMARTCONTROLLER : CtAce.ControllerType.WITHOUT_SMARTCONTROLLER;
                rAce = new CtAce();

                if (rAce != null) {
                    rAce.EnableMessage = true;
                    rAce.OnBoolEventChanged += rAce_BoolEventOccur;
                    rAce.OnNumericEventChanged += rAce_NumericEventOccur;
                    rAce.OnNotifyEventChanged += rAce_NotifyEventOccur;
                    rAce.OnTaskChanged += rAce_TaskEventOccur;
                    rAce.OnMessage += rAce_OnMessage;
                }


                rAce.Connect(ctrlType);

                if (rAce.IsConnected) {

                    rAce.RequestPower();
                    rAce.RequestSpeed();

                    foreach (Control ctrl in this.Controls) {
                        EnableControl(ctrl, true);
                    }

                    CtInvoke.CheckBoxEnable(chkCtrl, false);
                    CtInvoke.ButtonEnable(btnConnect, true);
                    CtInvoke.ButtonText(btnConnect, "中斷連線");
                    CtInvoke.ButtonTag(btnConnect, true);

                    List<string> lstCtrl = rAce.Controllers;
                    if (lstCtrl != null && lstCtrl.Count > 0) CtInvoke.TextBoxText(txtCtrl, lstCtrl);
                    List<string> lstRob = rAce.Robots;
                    if (lstRob != null && lstRob.Count > 0) CtInvoke.TextBoxText(txtRobot, lstRob);
                } else {
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
                CtInvoke.TextBoxForeColor(txtSpeed, Color.Red);
                CtInvoke.TextBoxFont(txtSpeed, new Font("Consolas", 10, FontStyle.Bold));
            } else {
                if (CtConvert.CInt(txtSpeed.Text) < 101) {
                    CtInvoke.TextBoxForeColor(txtSpeed, Color.Black);
                    CtInvoke.TextBoxFont(txtSpeed, new Font("Consolas", 10, FontStyle.Regular));
                    if (rAce != null)
                        rAce.SetSpeed(CtConvert.CInt(txtSpeed.Text));
                }
            }
        }

        private void btnVarRead_Click(object sender, EventArgs e) {
            object objTemp;
            rAce.Variable.GetValue(txtVarName.Text, out objTemp);
            if (objTemp != null) {
                string strTemp = CtConvert.CStr(objTemp);
                CtInvoke.TextBoxText(txtVarVal, strTemp);
            }
        }

        private void Test_ACE_Load(object sender, EventArgs e) {
            //List<double> aa = new List<double> { -999.95, 5.9, 1.0, 0.5 };
            //MessageBox.Show(CtConvert.CStr(aa.Max()));
            //List<string> num = new List<string> { "-999.999", "3", "5", "7", "9" };
            ////double max_val = num.Max(val => Convert.ToDouble(val));
            //List<double> num2 = num.ConvertAll(new Converter<string,double>(val => Convert.ToDouble(val)));
            //double max_val = num2.Min();
            //MessageBox.Show(max_val.ToString());


            CtInvoke.ComboBoxSelectedIndex(cbVisTool, 1);
            //CtInvoke.ComboBoxSelectedIndex(cbProgTask, 1);
        }

        private void btnVarWrite_Click(object sender, EventArgs e) {
            if (txtVarVal.Text != "") {
                CtAce.VPlusVariableType varType;
                rAce.Variable.GetVariabeType(txtVarName.Text, out varType);

                object objTemp = null;
                switch (varType) {
                    case CtAce.VPlusVariableType.REAL:
                        objTemp = CtConvert.CFloat(txtVarVal.Text);
                        break;
                    case CtAce.VPlusVariableType.STRING:
                        objTemp = txtVarVal.Text;
                        break;
                    case CtAce.VPlusVariableType.LOCATION:
                        List<string> strSplit = txtVarVal.Text.Split(CtConst.CHR_DELIMITERS, StringSplitOptions.RemoveEmptyEntries).ToList();
                        List<double> dblTemp = strSplit.ConvertAll(new Converter<string, double>(val => Convert.ToDouble(val)));
                        objTemp = dblTemp;
                        break;
                    case CtAce.VPlusVariableType.PRECISION_POINT:
                        List<string> strSplitPP = txtVarVal.Text.Split(CtConst.CHR_DELIMITERS, StringSplitOptions.RemoveEmptyEntries).ToList();
                        List<double> dblTempPP = strSplitPP.ConvertAll(new Converter<string, double>(val => Convert.ToDouble(val)));
                        objTemp = dblTempPP;
                        break;
                }
                if (objTemp != null) {
                    rAce.Variable.SetValue(txtVarName.Text, objTemp);
                }
            }
        }

        private void btnProgExe_Click(object sender, EventArgs e) {
            if (CtConvert.CBool(btnProgExe.Tag)) {
                rAce.Tasks.TaskAbort(cbProgTask.SelectedIndex);
                CtInvoke.ButtonTag(btnProgExe, false);
                CtInvoke.ButtonText(btnProgExe, "執行");
            } else {
                rAce.Tasks.TaskExecute(txtProgName.Text, cbProgTask.SelectedIndex);
                CtInvoke.ButtonTag(btnProgExe, true);
                CtInvoke.ButtonText(btnProgExe, "停止/暫停");
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
                CtInvoke.ButtonTag(btnProgMoni, false);
                CtInvoke.ButtonText(btnProgMoni, "加入監控");
            } else {
                rAce.Tasks.AddMonitorTask(cbProgTask.SelectedIndex);
                CtInvoke.ButtonTag(btnProgMoni, true);
                CtInvoke.ButtonText(btnProgMoni, "停止監控");
            }
        }

        private void cbProgTask_SelectedValueChanged(object sender, EventArgs e) {
            if (rAce.Tasks.IsMoniting(cbProgTask.SelectedIndex)) {
                CtInvoke.ButtonTag(btnProgMoni, true);
                CtInvoke.ButtonText(btnProgMoni, "停止監控");
            } else {
                CtInvoke.ButtonTag(btnProgMoni, false);
                CtInvoke.ButtonText(btnProgMoni, "加入監控");
            }

            if (rAce.Tasks.IsTaskExist(cbProgTask.SelectedIndex)) {
                CtInvoke.ButtonTag(btnProgExe, true);
                CtInvoke.ButtonText(btnProgExe, "停止/暫停");
            } else {
                CtInvoke.ButtonTag(btnProgExe, false);
                CtInvoke.ButtonText(btnProgExe, "執行");
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
            if (txtVisionPath.Text != null) {
                CtAceSingleVision.ToolType toolType = CtAceSingleVision.ToolType.VirtualCamera;
                switch (cbVisTool.SelectedIndex) {
                    case 0:
                        toolType = CtAceSingleVision.ToolType.Blob;
                        break;
                    case 1:
                        toolType = CtAceSingleVision.ToolType.VirtualCamera;
                        break;
                    case 2:
                        toolType = CtAceSingleVision.ToolType.CSharpCustomTool;
                        break;
                    case 3:
                        toolType = CtAceSingleVision.ToolType.ImageProcess;
                        break;
                    case 4:
                        toolType = CtAceSingleVision.ToolType.Locator;
                        break;
                }
                visionWindow.Connect(toolType, rAce, txtVisionPath.Text);
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

        #endregion

        private void btnShowVision_Click(object sender, EventArgs e) {
            visWindow.Connect(rAce);
        }

        private void btnOpen_Click(object sender, EventArgs e) {
            int io = -1;
            if (int.TryParse(txtIO.Text.Replace("-", ""), out io)) {
                rAce.IO.SetIO(io);
            }
        }

        private void btnClose_Click(object sender, EventArgs e) {
            int io = -1;
            if (int.TryParse(txtIO.Text.Replace("-", ""), out io)) {
                rAce.IO.SetIO(io * -1);
                //MessageBox.Show(rAce.IO.GetIO(io).ToString());
            }
        }

        private void btn90P_Click(object sender, EventArgs e) {
            rAce.Motion.MoveDistance(1, CtAce.Axis.ROLL, 90);
        }

        private void btn90N_Click(object sender, EventArgs e) {
            rAce.Motion.MoveDistance(1, CtAce.Axis.ROLL, -90);
        }

        private void btnPendant_Click(object sender, EventArgs e) {
            rAce.Pendant(true);
        }

        private void btnCusCmd_Click(object sender, EventArgs e) {
            CtInput input = new CtInput();
            string temp = "";
            input.Start(CtInput.InputStyle.TEXT, "自訂命令", "請輸入命令", out temp);
            //if (temp != "") rAce.Motion.SendString(temp);
        }

    }
}
