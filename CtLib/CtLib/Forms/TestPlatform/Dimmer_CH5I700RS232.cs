using System;
using System.Collections.Generic;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Dimmer;

namespace CtLib.Forms.TestPlatform {

    /// <summary>調光器控制</summary>
    public partial class Dimmer_CH5I700RS232 : Form {

        private CtDimmer_CH5I700RS232 mDimmer = new CtDimmer_CH5I700RS232();
        private Dictionary<byte, NumericUpDown> mLightValues = new Dictionary<byte, NumericUpDown>();

        /// <summary>建構調光器控制介面</summary>
        public Dimmer_CH5I700RS232() {
            InitializeComponent();

            CollectNumUpDown();
        }

        private void CollectNumUpDown() {
            mLightValues.Add(1, nudCh1);
            mLightValues.Add(2, nudCh2);
            mLightValues.Add(3, nudCh3);
            mLightValues.Add(4, nudCh4);
            mLightValues.Add(5, nudCh5);
        }

        private void SetUIEnable(bool enabled) {
            CtInvoke.ControlEnabled(gbCH1, enabled);
            CtInvoke.ControlEnabled(gbCH2, enabled);
            CtInvoke.ControlEnabled(gbCH3, enabled);
            CtInvoke.ControlEnabled(gbCH4, enabled);
            CtInvoke.ControlEnabled(gbCH5, enabled);
            CtInvoke.ControlEnabled(btnSetAll, enabled);
            CtInvoke.ControlEnabled(btnON, enabled);
            CtInvoke.ControlEnabled(btnOFF, enabled);
            CtInvoke.ControlVisible(lbVersion, enabled);
            CtInvoke.ControlText(btnConnect, enabled ? "Disconnect" : "Connect");
        }

        private void btnConnect_Click(object sender, EventArgs e) {
            if (mDimmer.IsConnected) {
                mDimmer.Disconnect();
                SetUIEnable(false);
            } else if (mDimmer.Connect() == Stat.SUCCESS) {
                SetUIEnable(true);
            }

            CtInvoke.PictureBoxImage(picConStt, mDimmer.IsConnected ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
        }

        private void btnSetAll_Click(object sender, EventArgs e) {
            if (mDimmer.SetLight((int)nudCh1.Value, (int)nudCh2.Value, (int)nudCh3.Value, (int)nudCh4.Value, (int)nudCh5.Value))
                CtMsgBox.Show("設定數值", "設定所有通道成功！");
            else CtMsgBox.Show("設定數值", "設定所有通道失敗 :(", MsgBoxBtn.OK, MsgBoxStyle.Error);
        }

        private void btnON_Click(object sender, EventArgs e) {
            mDimmer.LightSwitcher(true);
        }

        private void btnOFF_Click(object sender, EventArgs e) {
            mDimmer.LightSwitcher(false);
        }
    }
}
