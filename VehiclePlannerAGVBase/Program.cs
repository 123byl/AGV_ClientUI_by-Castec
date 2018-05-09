using CtLib.Forms;
using CtLib.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VehiclePlannerAGVBase {
    static class Program {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main() {

            /*-- 抓取未用 Try-Catch-Finally 包住的 Exception --*/
            /* 設定如果遇到未包住的 Exception 則處理 Exception */
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            /* UI 執行緒類的錯誤 */
            Application.ThreadException += Application_ThreadException;
            /* 並非處理 UI 的錯誤 */
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            bool isNotDuplicateApplication = false;
            using (Mutex mutex = new Mutex(true, Application.ProductName, out isNotDuplicateApplication)) {
                //判斷是否重複，不重複則執行應用程式，重複則關閉應用程式
                if (isNotDuplicateApplication) {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    IVehiclePlanner mVehiclePlanner = new VehiclePlanner();
                    Application.Run(new CtVehiclePlanner_Ctrl(mVehiclePlanner));
                } else {
                    CtMsgBox.Show("重复开启", "程序已经在运行中", MsgBoxBtn.OK, MsgBoxStyle.Error);
                    Application.Exit();
                }
            }
        }


        /// <summary>非 UI 執行緒錯誤的處理方法</summary>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null) CtStatus.Report(Stat.ER_SYSTEM, ex);
            else CtStatus.Report(Stat.ER_SYSTEM, "UnhandledNonThreadEx", e.ToString());
        }

        /// <summary>UI 錯誤的處理方法</summary>
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
            if (e.Exception != null) CtStatus.Report(Stat.ER_SYSTEM, e.Exception);
            else CtStatus.Report(Stat.ER_SYSTEM, "UnhandledThreadEx", e.ToString());
        }
    }
}
