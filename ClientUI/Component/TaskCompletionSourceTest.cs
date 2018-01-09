using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientUI.Component {
    public partial class TaskCompletionSourceTest : Form {
        private TaskCompletionSource<int> mTskSrc = null;
        bool isWaiting = false;
        public TaskCompletionSourceTest() {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e) {
            if (!isWaiting) {
                int tmo = 0;
                if (int.TryParse(txtTimeout.Text, out tmo)) {
                    Task.Run(() => {
                        isWaiting = true;
                        Show("Waiting");
                        mTskSrc = new TaskCompletionSource<int>();
                        var tsk = mTskSrc.Task;
                        try {
                            if (tsk.Wait(tmo)) {
                                Show("Response in time");
                            } else {
                                Show("Response timeout");
                            }
                        } catch {
                            Show("Exception");
                        }

                        if (tsk.IsCanceled) {
                            Show("Task canceled");
                        } else if (tsk.IsFaulted) {
                            Show("Task faulted");
                        } else if (tsk.IsCompleted) {
                            Show($"Task completed Result = {tsk.Result}");
                        }
                        isWaiting = false;
                    });
                }
            }
        }

        private void Show(string s) {
            Console.WriteLine(s);
        }

        private void btnResult_Click(object sender, EventArgs e) {
            if (isWaiting) {
                mTskSrc.SetResult(311);
                Show("Trigger SetResult");
            }
        }

        private void btnException_Click(object sender, EventArgs e) {
            if (isWaiting) {
                mTskSrc.SetException(new NullReferenceException());
                Show("Trigger SetExceptoin");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            if (isWaiting) {
                mTskSrc.SetCanceled();
                Show("Trigger SetCanceled");
            }
        }
    }
}
