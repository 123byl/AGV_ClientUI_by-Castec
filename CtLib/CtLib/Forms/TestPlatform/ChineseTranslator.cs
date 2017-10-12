using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CtLib.Library;

namespace CtLib.Forms.TestPlatform {
	/// <summary>一個簡易的介面供繁簡文字轉換，僅字對字轉，無法轉換語意</summary>
	public partial class ChineseTranslator : Form {
		/// <summary>建構繁簡轉換器</summary>
		public ChineseTranslator() {
			InitializeComponent();

			CtInvoke.RadioButtonChecked(rdbtnTWCN, true);
		}

		private void btnTrans_Click(object sender, EventArgs e) {
			string oriStr = txtOri.Text;
			string destStr = string.Empty;
			if (CtInvoke.RadioButtonChecked(rdbtnCNTW))
				destStr = CtConvert.ToTraditional(oriStr);
			else destStr = CtConvert.ToSimplified(oriStr);
			CtInvoke.ControlText(txtDest, destStr);
		}

		private void rdbtnTWCN_Click(object sender, EventArgs e) {
			CtInvoke.RadioButtonChecked(rdbtnTWCN, true);
			CtInvoke.RadioButtonChecked(rdbtnCNTW, false);
		}

		private void rdbtnCNTW_Click(object sender, EventArgs e) {
			CtInvoke.RadioButtonChecked(rdbtnTWCN, false);
			CtInvoke.RadioButtonChecked(rdbtnCNTW, true);
		}
	}
}
