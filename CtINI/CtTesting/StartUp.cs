using CtINI.Testing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtTesting {
    public partial class StartUp : Form {

        private enum TestEnum {
            one,
            two
        }

        CtrlIni mIni = null;
        CtrlParamEditor mSetting = null;
        CtrlParamGenarator mGenarator = null;

        public StartUp() {
            InitializeComponent();
            Test();
        }

        private void Test() {
            //int executeCount = 1000000;
            //Type classType = typeof(TestClass1);
            //Stopwatch sw = new Stopwatch();
            //sw.Restart();
            //for (int i = 0;i < executeCount; i++) {
            //    TestClass1 TestClass1 = new TestClass1();
            //}
            //Console.WriteLine($"直接創建{executeCount}次花費{sw.ElapsedMilliseconds:F2}(ms)");

            //sw.Restart();
            //for (int i = 0; i < executeCount; i++) {
            //    TestClass1 TestClass1 = (TestClass1)Activator.CreateInstance(classType);
            //}
            //Console.WriteLine($"Activator創建{executeCount}次花費{sw.ElapsedMilliseconds:F2}(ms)");

            //sw.Restart();
            //for (int i = 0; i < executeCount; i++) {
            //    TestClass1 TestClass1 = (TestClass1)classType.GetConstructors()[0].Invoke(null);
            //}
            //Console.WriteLine($"GetConstructors創建{executeCount}次花費{sw.ElapsedMilliseconds:F2}(ms)");

        }

        private bool IsEnum(object val) {
            return val is Enum;
        }

        private void btnIni_Click(object sender, EventArgs e) {
            if (mIni?.IsDisposed ?? true) mIni = new CtrlIni();
            mIni.Show();
        }

        private void btnSetting_Click(object sender, EventArgs e) {
            if (mSetting?.IsDisposed ?? true) mSetting = new CtrlParamEditor();
            mSetting.Show();
        }

        private void btnParamGenarator_Click(object sender, EventArgs e) {
            if (mGenarator?.IsDisposed ?? true) mGenarator = new CtrlParamGenarator();
            mGenarator.Show();
        }
    }

    public class TestClass1 {
        public delegate void DelDoSomething();
        public void DoSomething() {

        }
    }

}
