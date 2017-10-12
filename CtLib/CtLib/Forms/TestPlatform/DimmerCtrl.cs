using System;
using System.Collections.Generic;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Dimmer;

namespace CtLib.Forms.TestPlatform {

    /// <summary>調光器控制</summary>
    public partial class DimmerCtrl : Form {

        private CtDimmer mDimmer = new CtDimmer();
        private Dictionary<Channels, NumericUpDown> mLightValues = new Dictionary<Channels, NumericUpDown>();

        /// <summary>建構調光器控制介面</summary>
        public DimmerCtrl() {
            InitializeComponent();

            CollectNumUpDown();
        }

        private void CollectNumUpDown() {
            mLightValues.Add(Channels.Channel1, nudCh1);
            mLightValues.Add(Channels.Channel2, nudCh2);
            mLightValues.Add(Channels.Channel3, nudCh3);
            mLightValues.Add(Channels.Channel4, nudCh4);
        }

        private void SetUIEnable(bool enabled) {
            CtInvoke.ControlEnabled(gbCH1, enabled);
            CtInvoke.ControlEnabled(gbCH2, enabled);
            CtInvoke.ControlEnabled(gbCH3, enabled);
            CtInvoke.ControlEnabled(gbCH4, enabled);
            CtInvoke.ControlEnabled(btnSetAll, enabled);
            CtInvoke.ControlEnabled(btnGetAll, enabled);
            CtInvoke.ControlVisible(lbVersion, enabled);
            CtInvoke.ControlText(btnConnect, enabled ? "Disconnect" : "Connect");
        }

        private void btnConnect_Click(object sender, EventArgs e) {
            if (mDimmer.IsConnected) {
                mDimmer.Disconnect();
                SetUIEnable(false);
            } else if (mDimmer.Connect() == Stat.SUCCESS) {
                string ver = "";
				if (mDimmer.GetDimmerVersion(out ver) == Stat.SUCCESS) {
					CtInvoke.ControlText(lbVersion, ver);
					SetUIEnable(true);
				}
            }

            CtInvoke.PictureBoxImage(picConStt, mDimmer.IsConnected ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
        }

        private void SetLights(object sender, EventArgs e) {
            Channels ch = (Channels)CtConvert.CByte((sender as Button).Tag);
            if (mDimmer.SetLight(ch, (int)mLightValues[ch].Value)) CtMsgBox.Show("設定數值", "設定 " + ch.ToString() + "成功！");
            else CtMsgBox.Show("設定數值", "設定 " + ch.ToString() + "失敗 :(", MsgBoxBtn.OK, MsgBoxStyle.Error);
        }

        private void GetLights(object sender, EventArgs e) {
            Channels ch = (Channels)CtConvert.CByte((sender as Button).Tag);
            LightValue value;
            if (mDimmer.GetLight(ch, out value) == Stat.SUCCESS && value.Value.HasValue) {
                CtInvoke.NumericUpDownValue(mLightValues[ch], value.Value.Value);
            } else CtMsgBox.Show("取得數值", "取得 " + ch.ToString() + "失敗 :(", MsgBoxBtn.OK, MsgBoxStyle.Error);
        }

        private void btnSetAll_Click(object sender, EventArgs e) {
            if (mDimmer.SetLight((int)nudCh1.Value, (int)nudCh2.Value, (int)nudCh3.Value, (int)nudCh4.Value))
                CtMsgBox.Show("設定數值", "設定所有通道成功！");
            else CtMsgBox.Show("設定數值", "設定所有通道失敗 :(", MsgBoxBtn.OK, MsgBoxStyle.Error);
        }

        private void btnGetAll_Click(object sender, EventArgs e) {
            List<LightValue> values;
            if (mDimmer.GetLight(out values) == Stat.SUCCESS) {
                for (byte idx = 0; idx < values.Count; idx++) {
                    CtInvoke.NumericUpDownValue(mLightValues[(Channels)idx], values[idx].Value.Value);
                }
            } else CtMsgBox.Show("取得數值", "取得所有通道數值失敗 :(", MsgBoxBtn.OK, MsgBoxStyle.Error);
        }

        private void LockLights(object sender, EventArgs e) {
            CheckBox chk = sender as CheckBox;
            Channels ch = (Channels)CtConvert.CByte(chk.Tag);
            if (mDimmer.Lock(ch, chk.Checked)) CtMsgBox.Show("鎖定", "鎖定 " + ch.ToString() + "成功！");
            else CtMsgBox.Show("鎖定", "鎖定 " + ch.ToString() + "失敗 :(", MsgBoxBtn.OK, MsgBoxStyle.Error);
        }
    }
}
