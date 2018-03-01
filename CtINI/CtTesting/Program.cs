using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtTesting {
    static class Program {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main() {
            string str = "mEditor.ParamCollection.FindVal<string>(\"LaserHostIP\", val => device.LaserHostIp = val);";
            Regex reg = new Regex(@"<(?<type>.*?)>\(""(?<name>.*?)""");
            foreach (Match match in reg.Matches(str)) {
                var type = match.Groups["type"].Value;
                var name = match.Groups["name"].Value;
                Console.Write(match.ToString());
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new StartUp());
        }
    }
}
