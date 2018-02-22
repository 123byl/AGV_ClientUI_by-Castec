using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace VehiclePlanner {
    static class Program {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /*-- 抓取未用 Try-Catch-Finally 包住的 Exception --*/
            /* 設定如果遇到未包住的 Exception 則處理 Exception */
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            /* UI 執行緒類的錯誤 */
            Application.ThreadException += Application_ThreadException;
            /* 並非處理 UI 的錯誤 */
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.SetCompatibleTextRenderingDefault(false);
            CtVehiclePlanner planner = new CtVehiclePlanner();
            Application.Run(new VehiclePlannerUI(planner));

        }

        /// <summary>非 UI 執行緒錯誤的處理方法</summary>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            Exception ex = e.ExceptionObject as Exception;
            Console.WriteLine(ex.Message);
            
            //if (ex != null) CtStatus.Report(Stat.ER_SYSTEM, ex, true);
            //else CtStatus.Report(Stat.ER_SYSTEM, "UnhandledNonThreadEx", e.ToString());
        }

        /// <summary>UI 錯誤的處理方法</summary>
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
            if (e.Exception != null)
                Console.WriteLine(e.Exception.Message);
            //    CtStatus.Report(Stat.ER_SYSTEM, e.Exception, true);
            //else CtStatus.Report(Stat.ER_SYSTEM, "UnhandledThreadEx", e.ToString());
        }
    }
}
