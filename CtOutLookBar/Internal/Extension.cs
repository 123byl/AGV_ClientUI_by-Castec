using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtOutLookBar.Internal {
    /// <summary>
    /// 擴充方法
    /// </summary>
    internal static class Extension {

        public static Point Add(this Point p1, Point p2) {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static void InvokeIfNecessary(this Control ctrl, MethodInvoker action) {
            if (ctrl.InvokeRequired) ctrl.Invoke(action);
            else action();
        }

    }

}
