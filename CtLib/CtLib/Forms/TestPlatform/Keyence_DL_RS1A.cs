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
using CtLib.Module.Keyence;

namespace CtLib.Forms.TestPlatform {
    /// <summary>Keyence DL-RS1A TestPlatform</summary>
    public partial class Keyence_DL_RS1A : Form {

        private CtKeyence_DL_RS1A mDlRs1a = new CtKeyence_DL_RS1A();

        /// <summary>建構 DL-RS1A 測試介面</summary>
        public Keyence_DL_RS1A() {
            InitializeComponent();
        }

        private void ExceptionHandler(Exception ex) {
            CtInvoke.ListBoxInsert(lsbxMsg, 0, DateTime.Now.ToString("[HH:mm:ss.ff] ") + ex.Message);
        }

        private void btnConnect_Click(object sender, EventArgs e) {
            if (mDlRs1a.IsConnected) mDlRs1a.Disconnect();
            else mDlRs1a.Connect();

            CtInvoke.PictureBoxImage(picCntStt, mDlRs1a.IsConnected ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
        }

        private void btnGetSngVal_Click(object sender, EventArgs e) {
            if (mDlRs1a.IsConnected) {
                try {
                    float data = mDlRs1a.GetValue((byte)nudSngID.Value);
                    CtInvoke.ControlText(txtSngVal, data.ToString());
                } catch (Exception ex) {
                    ExceptionHandler(ex);
                }
            }
        }

        private void btnGetAllVal_Click(object sender, EventArgs e) {
            if (mDlRs1a.IsConnected) {
                try {
                    List<float> data = mDlRs1a.GetAllValue();
                    CtInvoke.ControlText(txtAllVal, string.Join(", ", data.ConvertAll(val => val.ToString())));
                } catch (Exception ex) {
                    ExceptionHandler(ex);
                }
            }
        }

        private void btnZero_Click(object sender, EventArgs e) {
            if (mDlRs1a.IsConnected) {
                try {
                    mDlRs1a.SetZero();
                } catch (Exception ex) {
                    ExceptionHandler(ex);
                }
            }
        }
	}
}
