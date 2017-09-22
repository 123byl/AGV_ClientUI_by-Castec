using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CtLib.Forms;
using CtLib.Library;

using ORiN2.interop.CAO;
using CtLib.Module.Ultity;

namespace CtLib.Module.ORiN2 {

    /// <summary>
    /// ORiN2
    /// <para>此為聯盟協議，包含 DENSO、FANUC 等</para>
    /// </summary>
    public class CtORiN2 {

        #region Version

        /// <summary>CtORiN2 版本相關訊息</summary>
        /// <remarks><code>
        /// 0.0.0  Ahern [2015/05/14]
        ///     + 建立 CtORiN2
        ///     
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(0, 0, 0, "2015/05/14", "Ahern Kuo");

        #endregion

        #region Declaration - Definitions
        private static readonly string CAO_PROVIDER = "CaoProv.DENSO.RC.X";
        private static readonly int CAO_ROBOT_START_INDEX = 0;
        #endregion

        #region Declaration - Enumerations

        /// <summary>ORiN2 變數類型</summary>
        public enum VariableType : byte {
            /// <summary>I/O</summary>
            IO,
            /// <summary>Point</summary>
            POINT,
            /// <summary>Joint</summary>
            JOINT
        }
        #endregion

        #region Declaration - Members
        private CaoEngine mIEngine;
        private CaoController mICtrl;
        private CaoWorkspace mIWrks;

        private string mCaoProv = CAO_PROVIDER;

        private Dictionary<string, CaoTask> mCaoTasks = new Dictionary<string, CaoTask>();
        private Dictionary<int, CaoRobot> mCaoRobots = new Dictionary<int, CaoRobot>();
        private Dictionary<string, CaoVariable> mCaoVars = new Dictionary<string, CaoVariable>();
        #endregion

        #region Declaration - Properties
        /// <summary>取得或設定 ORiN2 CaoProvider</summary>
        public string CaoProvider { get { return mCaoProv; } set { mCaoProv = value; } }
        /// <summary>取得或設定此控制器上設有多少手臂</summary>
        /// <remarks>請於 Connect 前設定好此項目，否則建立的數量將會有誤</remarks>
        public int RobotCount { get; set; }
        #endregion

        #region Function - Constructors
        /// <summary>ORiN2 Constructor</summary>
        public CtORiN2() {

        }
        #endregion

        #region Function - Methods

        #endregion

        #region Function - Core
        /// <summary>透過 CaoEngine 連線至 Controller 與 Robot</summary>
        /// <param name="ip">目標 Controller 網際網路位址(IP Address)</param>
        /// <param name="robotCount">Robot 數量</param>
        public void Connect(string ip, int robotCount = 1) {
            /* --------------------------------------------
             * As RC8 manual, the sequence of connection is
             *  1. CaoEngine
             *  2. CaoWorkspace
             *  3. CaoController
             * -------------------------------------------- */
            mIEngine = new CaoEngine();
            if (mIEngine.Workspaces.Count > 0) mIWrks = mIEngine.Workspaces.Item(0);    //When creating CaoEngine, it will create new CaoWorkspace automatically
            else mIWrks = mIEngine.AddWorkspace("NewWrks", "");
            mICtrl = mIWrks.AddController("RC7M", mCaoProv, Environment.UserDomainName, "Server=" + ip);  //Connect to Controller 

            RobotCount = robotCount;
            for (int idx = CAO_ROBOT_START_INDEX; idx < CAO_ROBOT_START_INDEX + robotCount; idx++) {
                CaoRobot rob = mICtrl.AddRobot("Robot" + idx.ToString(), "ID=" + idx.ToString());
                mCaoRobots.Add(idx, rob);
            }
        }

        /// <summary>中斷連線</summary>
        public void Disconnect() {
            mCaoRobots.Clear();
            mICtrl.Variables.Clear();
            mICtrl.Tasks.Clear();
            mICtrl.Robots.Clear();
            mIWrks.Controllers.Remove(mICtrl.Index);
            mICtrl = null;
            mIEngine.Workspaces.Remove(mIWrks.Index);
            mIWrks = null;
            mIEngine = null;

        }

        /// <summary>取得變數數值。如 "P13"</summary>
        /// <param name="varName">變數名稱。如 "P7"、"J3"</param>
        /// <returns>取得的數值，請自行做型態轉換!</returns>
        public object GetValue(string varName) {
            CaoVariable caoVar;

            if (mCaoVars.ContainsKey(varName)) caoVar = mCaoVars[varName];
            else {
                caoVar = mICtrl.AddVariable(varName);
                mCaoVars.Add(varName, caoVar);
            }

            return caoVar.Value;
        }

        /// <summary>設定變數數值，型態為 float</summary>
        /// <param name="varName">變數名稱</param>
        /// <param name="value">欲寫入之數值</param>
        public void SetValue(string varName, float value) {
            CaoVariable caoVar;
            if (mCaoVars.ContainsKey(varName)) caoVar = mCaoVars[varName];
            else {
                caoVar = mICtrl.AddVariable(varName);
                mCaoVars.Add(varName, caoVar);
            }

            caoVar.Value = value;
        }

        /// <summary>設定變數數值，型態為 Point 或 Joint</summary>
        /// <param name="varName">變數名稱</param>
        /// <param name="value">欲寫入變數之數值。Point 長度為 7 (含姿態定義)，Joint 長度為 6</param>
        /// <param name="varType">變數類型，Point 或 Joint</param>
        public void SetValue(string varName, List<float> value, VariableType varType = VariableType.POINT) {
            if (!varName.StartsWith("P") && !varName.StartsWith("J")) {
                int verify = -1;
                if (int.TryParse(varName, out verify))
                    varName = (varType == VariableType.POINT ? "P" : "J") + varName;
                else
                    throw (new Exception("The name \"" + varName + "\" is invalid for access I/O"));
            }
            if (varType == VariableType.POINT) {
                //if (value.Count == 6) value.Add(-1);
                //else if (value.Count != 7) throw (new Exception("Invalid value for setting Point location. Pass-in value are \"" + string.Join(", ", value) + "\""));
            } else {
                if (value.Count == 6) {
                    value.Add(0);
                    value.Add(0);
                } else if (value.Count != 8) throw (new Exception("Invalid value for setting Joint variable. Pass-in value are \"" + string.Join(", ", value.ConvertAll(val => val.ToString("0.0##")).ToArray()) + "\""));
            }


            CaoVariable caoVar;
            if (mCaoVars.ContainsKey(varName)) caoVar = mCaoVars[varName];
            else {
                caoVar = mICtrl.AddVariable(varName);
                mCaoVars.Add(varName, caoVar);
            }

            caoVar.Value = value.ToArray();
            //Console.WriteLine(CtConvert.CStr(temp));
        }

        /// <summary>取得 I/O 狀態</summary>
        /// <param name="ioName">欲取得的 I/O。如 "IO64"、"73"</param>
        /// <returns>(True)ON  (False)OFF</returns>
        public bool GetIO(string ioName) {
            if (!ioName.StartsWith("IO")) {
                int verify = -1;
                if (int.TryParse(ioName, out verify))
                    ioName = "IO" + ioName;
                else
                    throw (new Exception("The name \"" + ioName + "\" is invalid for access I/O"));
            }

            CaoVariable caoVar;
            if (mCaoVars.ContainsKey(ioName)) caoVar = mCaoVars[ioName];
            else {
                caoVar = mICtrl.AddVariable(ioName);
                mCaoVars.Add(ioName, caoVar);
            }

            return CtConvert.CBool(caoVar.Value);
        }

        /// <summary>設定 I/O 狀態</summary>
        /// <param name="ioName">欲設定的 I/O。如 "IO64"、"69"</param>
        /// <param name="value">欲更改之狀態</param>
        public void SetIO(string ioName, bool value) {
            if (!ioName.StartsWith("IO")) {
                int verify = -1;
                if (int.TryParse(ioName, out verify))
                    ioName = "IO" + ioName;
                else
                    throw (new Exception("The name \"" + ioName + "\" is invalid for access I/O"));
            }

            CaoVariable caoVar;
            if (mCaoVars.ContainsKey(ioName)) caoVar = mCaoVars[ioName];
            else {
                caoVar = mICtrl.AddVariable(ioName);
                mCaoVars.Add(ioName, caoVar);
            }

            caoVar.Value = value ? 1 : 0;
        }

        /// <summary>設定 I/O 狀態</summary>
        /// <param name="ioNum">欲設定的 I/O。如 "93"</param>
        /// <param name="value">欲更改之狀態</param>
        public void SetIO(int ioNum, bool value) {
            CaoVariable caoVar;
            if (mCaoVars.ContainsKey("IO" + ioNum)) caoVar = mCaoVars["IO" + ioNum];
            else {
                caoVar = mICtrl.AddVariable("IO*");
                caoVar.ID = ioNum;
                mCaoVars.Add("IO" + ioNum, caoVar);
            }

            caoVar.Value = value ? 1 : 0;
        }

        /// <summary>執行 Task</summary>
        /// <param name="prgName">程式名稱，如 "IO_Test"</param>
        /// <remarks>採用關鍵字搜尋</remarks>
        public void ExecuteTask(string prgName) {
            //重 Task 清單搜尋所有 Program 並比對
            string fullName = (mICtrl.TaskNames as object[]).Cast<string>().First(val => val.Contains(prgName));

            CaoTask caoTask;
            if (mCaoTasks.ContainsKey(fullName)) caoTask = mCaoTasks[fullName];
            else {
                caoTask = mICtrl.AddTask(fullName);
                mCaoTasks.Add(fullName, caoTask);
            }

            caoTask.Start(2, "");
        }

        /// <summary>取消 Task</summary>
        /// <param name="prgName">欲取消的 Task 名稱</param>
        public void AbortTask(string prgName) {
            //重 Task 清單搜尋所有 Program 並比對
            string fullName = (mICtrl.TaskNames as object[]).Cast<string>().First(val => val.Contains(prgName));

            if (mCaoTasks.ContainsKey(fullName)) {
                mCaoTasks[fullName].Stop(4);
            } else {
                //此段程式確定沒有用!! 有時間再修
                if (mICtrl.Tasks.Count > 0) {
                    List<CaoTask> aa = mICtrl.Tasks.Cast<CaoTask>().ToList();
                    CaoTask caoTask = aa.Find(tsk => tsk.Name.Contains(prgName));
                    if (caoTask != null)
                        caoTask.Stop(4);
                    else throw (new Exception("The task name \"" + prgName + "\" is not exists"));
                }
                throw (new Exception("There are not exists any executing tasks"));
            }
        }

        /// <summary>取得所有 Task 名稱</summary>
        /// <returns></returns>
        public List<string> GetTasks() {
            object[] temp = mICtrl.TaskNames as object[];
            return temp.Cast<string>().ToList();
        }

        /// <summary>
        /// Test of Attach()
        /// </summary>
        public void TakeArm() {
            mICtrl.Execute("PutAutoMode", 1);
            mCaoRobots[0].Execute("TakeArm");
        }
        #endregion

    }
}
