using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.ORiN2;

namespace CtLib.Forms.TestPlatform {
    /// <summary>DENSO ORiN2 Tester</summary>
    public partial class Test_Denso : Form {

        private CtORiN2 mORiN2 = new CtORiN2();

        /// <summary>ORiN2 Constructor</summary>
        public Test_Denso() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            if (CtConvert.CBool(button1.Tag)) {
                mORiN2.Disconnect();
                CtInvoke.ButtonTag(button1, false);
            } else {
                mORiN2.Connect(txtIP.Text);
                CtInvoke.ButtonTag(button1, true);
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            object temp = mORiN2.GetValue(textBox1.Text);
            MessageBox.Show(CtLib.Library.CtConvert.CStr(temp));
        }

        private void button3_Click(object sender, EventArgs e) {
            bool temp = mORiN2.GetIO(textBox2.Text);
            MessageBox.Show(temp.ToString());
        }

        private void button4_Click(object sender, EventArgs e) {
            List<float> temp = textBox4.Text.Split(CtConst.CHR_SEPERATOR, StringSplitOptions.RemoveEmptyEntries).ToList().ConvertAll(val => float.Parse(val));
            mORiN2.SetValue(textBox3.Text, temp);
        }

        private void button5_Click(object sender, EventArgs e) {
            mORiN2.SetIO(textBox6.Text, CtConvert.CBool(textBox5.Text));
        }

        private void button6_Click(object sender, EventArgs e) {
            mORiN2.ExecuteTask(textBox7.Text);
        }

        private void button7_Click(object sender, EventArgs e) {
            List<string> temp = mORiN2.GetTasks();
            MessageBox.Show(string.Join(", ", temp.ToArray()));
        }

        private void button8_Click(object sender, EventArgs e) {
            mORiN2.TakeArm();
        }
    }
}
