using System;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Delta;

namespace CtLib.Forms.TestPlatform {
	public partial class Delta_SE : Form {

		private CtDelta_PLC mDMT = new CtDelta_PLC();
		private byte mComNo = 0;

		/// <summary>建構 Delta SE 測試介面</summary>
		public Delta_SE() {
			InitializeComponent();
			cbMVal.SelectedIndex = 0;
			cbYVal.SelectedIndex = 0;
		}

		private void ShowMessage(string msg) {
			string item = string.Format("[{0}] {1}", DateTime.Now.ToString("HH:mm:ss.fff"), msg);
			lsbxMsg.BeginInvokeIfNecessary(() => lsbxMsg.Items.Insert(0, item));
		}

		private void ChangeControl(bool stt) {
			btnConnect.BeginInvokeIfNecessary(() => btnConnect.Visible = !stt);
			btnDisconnect.BeginInvokeIfNecessary(() => btnDisconnect.Visible = stt);
			txtIP.BeginInvokeIfNecessary(() => txtIP.Enabled = !stt);
			gbX.BeginInvokeIfNecessary(() => gbX.Enabled = stt);
			gbY.BeginInvokeIfNecessary(() => gbY.Enabled = stt);
			gbM.BeginInvokeIfNecessary(() => gbM.Enabled = stt);
			gbD.BeginInvokeIfNecessary(() => gbD.Enabled = stt);
		}

		private void btnConnect_Click(object sender, EventArgs e) {
			string ip = txtIP.InvokeIfNecessary(() => txtIP.Text);

			if (mDMT.Connect(ip, out mComNo)) {
				ShowMessage("已與 " + ip + " 連線");
				ChangeControl(true);
			}
		}

		private void btnDisconnect_Click(object sender, EventArgs e) {
			mDMT.Disconnect(mComNo);
			ShowMessage("已中斷連線");
			ChangeControl(false);
		}

		private void btnXRead_Click(object sender, EventArgs e) {
			int no = nudXId.InvokeIfNecessary(() => (int)nudXId.Value);
			string tar = string.Format("X{0}", no);
			bool stt;
			if (mDMT.GetValue(mComNo, tar, out stt) > -1) {
				ShowMessage(string.Format("讀取 {0}，取得結果為 {1}", tar, (stt ? "On" : "Off")));
				txtXVal.BeginInvokeIfNecessary(() => txtXVal.Text = stt ? "On" : "Off");
			} else ShowMessage(string.Format("讀取 {0} 失敗", tar));
		}

		private void btnYRead_Click(object sender, EventArgs e) {
			int no = nudYId.InvokeIfNecessary(() => (int)nudYId.Value);
			string tar = string.Format("Y{0}", no);
			bool stt;
			if (mDMT.GetValue(mComNo, tar, out stt) > -1) {
				ShowMessage(string.Format("讀取 {0}，取得結果為 {1}", tar, (stt ? "On" : "Off")));
				cbYVal.BeginInvokeIfNecessary(() => cbYVal.SelectedIndex = stt ? 0 : 1);
			} else ShowMessage(string.Format("讀取 {0} 失敗", tar));
		}

		private void btnYWrite_Click(object sender, EventArgs e) {
			int no = nudYId.InvokeIfNecessary(() => (int)nudYId.Value);
			string tar = string.Format("Y{0}", no);
			bool stt = cbYVal.InvokeIfNecessary(() => cbYVal.SelectedIndex) == 0 ? true : false;
			if (mDMT.SetValue(mComNo, tar, stt) > -1) {
				ShowMessage(string.Format("成功寫入 {0} 為 {1}", tar, (stt ? "On" : "Off")));
			} else ShowMessage(string.Format("寫入 {0} 失敗", tar));
		}

		private void btnMRead_Click(object sender, EventArgs e) {
			int no = nudMId.InvokeIfNecessary(() => (int)nudMId.Value);
			string tar = string.Format("M{0}", no);
			bool stt;
			if (mDMT.GetValue(mComNo, tar, out stt) > -1) {
				ShowMessage(string.Format("讀取 {0}，取得結果為 {1}", tar, (stt ? "On" : "Off")));
				cbMVal.BeginInvokeIfNecessary(() => cbMVal.SelectedIndex = stt ? 0 : 1);
			} else ShowMessage(string.Format("讀取 {0} 失敗", tar));
		}

		private void btnMWrite_Click(object sender, EventArgs e) {
			int no = nudMId.InvokeIfNecessary(() => (int)nudMId.Value);
			string tar = string.Format("M{0}", no);
			bool stt = cbMVal.InvokeIfNecessary(() => cbMVal.SelectedIndex) == 0 ? true : false;
			if (mDMT.SetValue(mComNo, tar, stt) > -1) {
				ShowMessage(string.Format("成功寫入 {0} 為 {1}", tar, (stt ? "On" : "Off")));
			} else ShowMessage(string.Format("寫入 {0} 失敗", tar));
		}

		private void btnDRead_Click(object sender, EventArgs e) {
			int no = nudDId.InvokeIfNecessary(() => (int)nudDId.Value);
			string tar = string.Format("D{0}", no);
			uint val;
			if (mDMT.GetValue(mComNo, tar, out val) > -1) {
				ShowMessage(string.Format("讀取 {0}，取得結果為 {1}", tar, val));
				txtDVal.BeginInvokeIfNecessary(() => txtDVal.Text = val.ToString());
			} else ShowMessage(string.Format("讀取 {0} 失敗", tar));
		}

		private void btnDWrite_Click(object sender, EventArgs e) {
			int no = nudDId.InvokeIfNecessary(() => (int)nudDId.Value);
			string tar = string.Format("D{0}", no);
			uint val = uint.Parse(txtDVal.InvokeIfNecessary(() => txtDVal.Text));
			if (mDMT.SetValue(mComNo, tar, val) > -1) {
				ShowMessage(string.Format("成功寫入 {0} 為 {1}", tar, val));
			} else ShowMessage(string.Format("寫入 {0} 失敗", tar));
		}
	}
}
