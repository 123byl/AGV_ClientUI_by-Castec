using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CtLib.Forms;
using CtLib.Library;
using CtLib.Module.UniversalRobot;

namespace CtLib.Forms.TestPlatform {
    /// <summary>Test Platform for Universal Robots</summary>
    public partial class Test_UniversalRobots : Form {

        private CtUrServer mUR = new CtUrServer();

        /// <summary>Construct a new Universal Robots platform</summary>
        public Test_UniversalRobots() {
            InitializeComponent();

            GetCommands();
            mUR.OnUrEvents += mUR_OnUrEvents;
        }

        private void ShowMessage(string msg) {
            string temp = DateTime.Now.ToString("[HH:mm:ss.ff] ") + msg;
            CtInvoke.ListBoxInsert(lsbxInfo, 0, temp);
        }

        void mUR_OnUrEvents(object sender, CtUrServer.UrEventArgs e) {
            switch (e.Event) {
                case CtUrServer.UrEvents.Connection:
                    if (CtConvert.CBool(e.Value)) {
                        ShowMessage("已與 UR 連線。 IP = " + mUR.UR_IP + "  Port = " + mUR.UR_Port.ToString());
                        CtInvoke.ControlEnabled(btnSend, true);
                        CtInvoke.ControlText(btnConnect, "Disconnect");
                        CtInvoke.ControlEnabled(cbCommand, true);
                        CtInvoke.ControlEnabled(txtParameter, true);
                    } else {
                        ShowMessage("已與 UR 中斷連線");
                        CtInvoke.ControlEnabled(btnSend, false);
                        CtInvoke.ControlText(btnConnect, "Connect");
                        CtInvoke.ControlEnabled(cbCommand, false);
                        CtInvoke.ControlEnabled(txtParameter, false);
                    }
                    break;
                case CtUrServer.UrEvents.RunState:
                    ShowMessage("Running State = " + e.Value.ToString());
                    break;
                case CtUrServer.UrEvents.Mode:
                    ShowMessage("Mode State = " + e.Value.ToString());
                    break;
                case CtUrServer.UrEvents.ProgramState:
                    ShowMessage("Program State = " + e.Value.ToString());
                    break;
                case CtUrServer.UrEvents.LoadedProgram:
                    ShowMessage("Load Program = " + e.Value.ToString());
                    break;
                case CtUrServer.UrEvents.Shutdown:
                    ShowMessage("UR Shutdown");
                    break;
                case CtUrServer.UrEvents.Exception:
                    ShowMessage("[Exception] " + e.Value.ToString());
                    break;
                default:
                    break;
            }
        }

        private void GetCommands() {
            List<CtUrServer.URCommand> cmd = Enum.GetValues(typeof(CtUrServer.URCommand)).Cast<CtUrServer.URCommand>().ToList();
            foreach (CtUrServer.URCommand item in cmd) {
                CtInvoke.ComboBoxAdd(cbCommand, item.ToString());
            }
        }

        private void btnConnect_Click(object sender, EventArgs e) {
            if (mUR.IsConnected) {
                mUR.Disconnect();
            } else {
                mUR.Connect(txtIP.Text, CtConvert.CInt(txtPort.Text));
            }
        }

        private void btnSend_Click(object sender, EventArgs e) {
            CtUrServer.URCommand cmd = (CtUrServer.URCommand)Enum.Parse(typeof(CtUrServer.URCommand), cbCommand.Text, true);
            mUR.SendCommand(cmd, txtParameter.Text);
            CtInvoke.ControlText(txtParameter, "");
        }
    }
}
