using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Web.Services.Protocols;

using CtLib.Module.Utility;

using Staubli.Robotics;
using Staubli.Robotics.Soap.Proxies.ServerV0;
using Staubli.Robotics.Soap.Proxies.ServerV1;
using Staubli.Robotics.Soap.Proxies.ServerV2;
using Staubli.Robotics.Soap.Proxies.ServerV3;

namespace CtLib.Module.Staubli {

    /// <summary>Stäubli CS8 手臂控制器相關操作。使用 SOAP。</summary>
    /// <remarks>
    ///		目前尚缺以下部分之實作，尚未找到對應的方法...
    ///			1. Execute Task
    ///			2. Set Variable Value
    ///			
    ///		以下為注意事項
    ///			1. 變數與定義均藏在 Application 裡，即是 .pjx 與 .dtx 兩檔案
    ///			2. I/O 部分需要完整路徑，像是 BasicIO-1\%Q0 不可打 %Q0
    ///			3. CS8 提供 Web Service 與純 SOAP 操作
    ///				3-1. Web Serive 部分，Jackson 有拿到 WSDL，但其整體反應稍比純 SOAP 慢，故仍採 SOAP
    ///				3-2. 純 SOAP 部分有 Simon 提供的 DLL，Jackson 也有拿到原始碼，但都需要設定 App.config 採用 WCF 架構，所以都沒差...
    ///			4. 承上，若有 Exception 都是噴 SoapException (Win32Exception 而非 .Net Exception)，故須做轉換
    /// </remarks>
    public class CtStaubliCS8 : ICtVersion, IDisposable {

        #region Version
        /// <summary>CtStaubli 版本資訊</summary>
        /// <remarks><code>
        ///		0.0.0	Jackson	[2017/05/25]
        ///			+ Initial Pilot
        ///			
        ///		0.0.1	Jackson [2017/05/26]
        ///			+ Application
        ///			+ I/O
        ///			+ Task
        ///			
        ///		0.0.2	Jackson [2017/05/31]
        ///			\ I/O 讀取方式，其 Path 須為完整路徑
        ///			\ createBy 實際上為 Application 路徑
        ///			
        ///		0.0.3	Jackson	[2017/06/28]
        ///			+ VAL3 Variable 讀取(尚不支援寫入)
        ///			\ Disconnect 若尚未連線則不噴 Exception
        ///			\ 所有內容套用 ConvertSoapException
        /// 
        /// </code></remarks>
        public CtVersion Version { get { return new CtVersion(0, 0, 3, "2017/06/28", "Jackson"); } }
        #endregion

        #region Declaration - Fields
        /// <summary>儲存連線後的資訊</summary>
        private int? mSessionId = null;
        private string mAddr = string.Empty;
        private double mSoapVer = 0;

        private CS8ServerV0PortTypeClient mV0;
        private CS8ServerV1PortTypeClient mV1;
        private CS8ServerV2PortTypeClient mV2;
        private CS8ServerV3PortTypeClient mV3;

        private Robot[] mRobots;
        #endregion

        #region Declaration - Properties
        /// <summary>取得當前是否有登入 CS8 控制器</summary>
        public bool IsLogin { get { return mSessionId.HasValue; } }
        #endregion

        #region Function - Constructors
        /// <summary>初始化 Stäubli CS8</summary>
        /// <param name="ip">網際網路位址</param>
        /// <param name="port">埠號</param>
        public CtStaubliCS8(string ip, int port) {
            mAddr = $"http://{ip}:{port}";
            mV0 = new CS8ServerV0PortTypeClient();
            mV0.Endpoint.Address = new EndpointAddress(mAddr);
        }
        #endregion

        #region Function - IDisposable Implements
        /// <summary>關閉連線並釋放資源</summary>
        public virtual void Dispose() {
            if (IsLogin) Disconnect();
        }
        #endregion

        #region Function - Private Methods
        /// <summary>尋找並連接 V1~V3 伺服器</summary>
        /// <param name="ver">SOAP 版本</param>
        private void CreateServers(double ver) {
            try {
                if (ver >= 1.1 && mV0.findServer("/CS8ServerV1")) {
                    mV1 = new CS8ServerV1PortTypeClient();
                    mV1.Endpoint.Address = new EndpointAddress($"{mAddr}/CS8ServerV1");
                }

                if (ver >= 1.2 && mV0.findServer("/CS8ServerV2")) {
                    mV2 = new CS8ServerV2PortTypeClient();
                    mV2.Endpoint.Address = new EndpointAddress($"{mAddr}/CS8ServerV2");
                }

                if (ver >= 1.2 && mV0.findServer("/CS8ServerV3")) {
                    mV3 = new CS8ServerV3PortTypeClient();
                    mV3.Endpoint.Address = new EndpointAddress($"{mAddr}/CS8ServerV3");
                }
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>將 <see cref="SoapException"/> 轉換為對應訊息的 <see cref="Exception"/></summary>
        /// <param name="ex">SOAP 例外狀況</param>
        /// <returns>對應的例外狀況</returns>
        private Exception ConvertSoapException(SoapException ex) {
            Exception newEx = null;
            if (ex.Detail != null) {
                switch (ex.Detail.InnerText) {
                    case "INVALID-SESSION-ID-CODE":
                        newEx = new InvalidOperationException("Session has expired");
                        break;
                    case "READ-ACCESS-ERROR-CODE":
                        newEx = new AddressAccessDeniedException("User doesn't have read access");
                        break;
                    case "WRITE-ACCESS-ERROR-CODE":
                        newEx = new AddressAccessDeniedException("User doesn't have write access");
                        break;
                    case "SET-POS-POWER-ON-CODE":
                        newEx = new InvalidOperationException("Unable to send a joint position to the controller because the power is ON");
                        break;
                    case "CLIENT-ALREADY-CONNECTED":
                        newEx = new InvalidOperationException("A debugger is already connected on the controller");
                        break;
                    case "CLIENT-COMMUNICATION-ERROR":
                        newEx = new InvalidOperationException("Watchdog between the controller and the debugger");
                        break;
                    case "APPLICATION-NOT-FOUND":
                        newEx = new ArgumentException("Application not found on the controller");
                        break;
                    case "PROGRAM-NOT-FOUND":
                        newEx = new ArgumentException("Program not found on the controller (not loaded in the memory)");
                        break;
                    case "TASK-NOT-FOUND":
                        newEx = new ArgumentException("Task not found on the controller");
                        break;
                    case "STACK-FRAME-NOT-FOUND":
                        newEx = new ArgumentException("StackFrame not found on the controller");
                        break;
                    case "TASK-ALREADY-LOCKED":
                        newEx = new InvalidOperationException("Task is already locked by the debugger on the teach pendant. A task cannot be accessed from SOAP and from the teach pendant simultaneously.");
                        break;
                    case "PROGRAM-LINE--NOT-FOUND":
                        newEx = new InvalidOperationException("Program line doesn‟t exist");
                        break;
                    case "SIN-RETURN-CODE-NOK":
                        newEx = new InvalidOperationException("SIN return code. (Internal use)");
                        break;
                    case "MISMATCHED-CODE":
                        newEx = new InvalidOperationException("The SoapProgramLine provided is different from the one in the memory of the controller");
                        break;
                    case "IOWRITE-ACCESS-ERROR-WORKING-MODE":
                        newEx = new InvalidOperationException("IO write disabled: make sure that the working mode is “Manual” or that the IO is simulated");
                        break;
                    case "IOWRITE-ACCESS-ERROR-VALIDATION":
                        newEx = new InvalidOperationException("Fail to write into the IO. Press the deadman or park the teach to enable it");
                        break;
                    case "IOWRITE-ACCESS-ERROR-CODE":
                        newEx = new AddressAccessDeniedException("Current profile doesn't have IO WRITE ACCESS");
                        break;
                    default:
                        newEx = new Exception(ex.Detail.InnerText);
                        break;
                }
            } else {
                newEx = ex;
            }
            return newEx;
        }
        #endregion

        #region Function - Public Operations
        /// <summary>測試 CS8 控制器是否存在網域內</summary>
        /// <returns>(True)可供連線 (False)找不到該控制器</returns>
        public bool Ping() {
            try {
                var oriMsg = "CASTEC Library";
                var pingMsg = oriMsg;
                mV0.ping(ref pingMsg);
                return oriMsg.Equals(pingMsg);
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>嘗試連接並登入至 CS8 控制器</summary>
        /// <param name="usr">登入帳號</param>
        /// <param name="pwd">登入密碼</param>
        public void Connect(string usr, string pwd) {
            try {
                var soapVer = mV0.getSoapServerVersion("me", "0");
                if (double.TryParse(soapVer.version, out mSoapVer)) {
                    CreateServers(mSoapVer);
                }
                int sid = int.MinValue;
                mV0.login(usr, pwd, out sid);
                if (sid <= 0) throw new Exception("Invalid user name or password");
                mSessionId = sid;

                mRobots = mV0.getRobots(sid);
                Console.WriteLine(string.Join("\r\n", mRobots.Select(rob => $"Arm: {rob.arm}")));
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>中斷連線並登出</summary>
        public void Disconnect() {
            if (mSessionId.HasValue) {
                try {
                    mV0.logout(mSessionId.Value);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            mSessionId = null;
            //mV0 = null; 不處理，因為其 Endpoint 已經設定過，不再重複
            mV1 = null;
            mV2 = null;
            mV3 = null;
        }
        #endregion

        #region Function - Robotics
        /// <summary>取得當前 CS8 控制器所連接的所有手臂</summary>
        /// <returns>手臂集合</returns>
        public List<StaubliRobot> GetRobots() {
            try {
                var robots = mV0.getRobots(mSessionId.Value);
                return robots.Select(robot => new StaubliRobot(robot)).ToList();
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>取得當前的 Joint 數值</summary>
        /// <param name="robNum">手臂編號</param>
        /// <returns>各個 Joint 數值</returns>
        public double[] GetCurrentJoint(int robNum) {
            try {
                var jnt = mV0.getRobotJointPos(mSessionId.Value, robNum);
                return Array.ConvertAll(jnt, j => j * Tools.RAD_TO_DEG);
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>取得當前的 Cartesian 座標</summary>
        /// <param name="robNum">手臂編號</param>
        /// <returns>依序為 x, y, z, rx, ry, rz</returns>
        public double[] GetCurrentPoint(int robNum) {
            try {
                CartesianPos point;
                mV0.getRobotJntCartPos(mSessionId.Value, robNum, new CartesianPos(), new CartesianPos(), out point);
                if (point == null) throw new Exception("Get robot cartesian point failed");
                return new double[] { point.x, point.y, point.z, point.rx, point.ry, point.rz };
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>設定機器手臂電源致能狀態</summary>
        /// <param name="pwr">致能狀態</param>
        public void SetPower(bool pwr) {
            try {
                var retCode = mV3.setPower(mSessionId.Value, pwr);
                switch (retCode) {
                    case PowerReturnCode.POWERCHANGEWHILEROBOTNOTSTOPPED:
                        throw new InvalidOperationException("Robot is not stopped");

                    case PowerReturnCode.POWERENABLETIMEOUT:
                        throw new TimeoutException("Enable power timeout");

                    case PowerReturnCode.POWERDISABLETIMEOUT:
                        throw new TimeoutException("Disable power timeout");

                    case PowerReturnCode.POWERCHANGEONLYINREMOTEMODE:
                        throw new Exception("Robot is not in remote mode");

                    default:
                        break;
                }
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }
        #endregion

        #region Function - I/Os
        /// <summary>取得 I/O 狀態</summary>
        /// <param name="ioName">I/O 位址，如 "BasicIO-1\%Q0"</param>
        /// <returns>I/O 狀態</returns>
        public IoState GetIO(string ioName) {
            try {
                var ioStt = mV2.readIos(mSessionId.Value, new string[] { ioName }, true);
                if (ioStt == null || ioStt.Length < 1) throw new Exception($"Read I/O \"{ioName}\" failed");
                return new IoState(ioName, ioStt[0]);
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>取得 I/O 狀態</summary>
        /// <param name="ioPath">I/O 位址</param>
        /// <returns>I/O 狀態</returns>
        public List<IoState> GetIOs(params string[] ioPath) {
            try {
                var ioStt = mV2.readIos(mSessionId.Value, ioPath, true);
                if (ioStt == null || ioStt.Length < 1) throw new Exception($"Read I/O \"{string.Join(",", ioPath)}\" failed");
                var retIO = new List<IoState>();
                for (int idx = 0; idx < ioPath.Length; idx++) {
                    retIO.Add(new IoState(ioPath[idx], ioStt[idx]));
                }
                return retIO;
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>取得所有 I/O 資訊</summary>
        /// <returns>I/O 狀態</returns>
        public List<PhysicalIO> GetIOs() {
            try {
                var mime = mV2.getAllPhysicalIos(mSessionId.Value);
                List<PhysicalIO> retIO = null;
                using (var memStream = new MemoryStream(mime.data)) {
                    var doc = XDocument.Load(memStream);
                    if (doc.Root.HasElements) {
                        retIO = doc.Root.Elements().Select(elmt => new PhysicalIO(elmt)).ToList();
                    }
                    //var xs = new System.Xml.XmlWriterSettings() { Indent = true, IndentChars = "\t" };
                    //using (var xw = System.Xml.XmlWriter.Create(@"D:\io.xml", xs))
                    //{
                    //    doc.Save(xw);
                    //}
                }
                return retIO;
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>設定 I/O 狀態</summary>
        /// <param name="ioPath">I/O 位址</param>
        /// <param name="stt">(True)ON (False)OFF</param>
        public void SetIO(string ioPath, bool stt) {
            try {
                var rsp = mV2.writeIos(mSessionId.Value, new string[] { ioPath }, new double?[] { (stt ? 1 : 0) });
                if (!rsp[0].found) {
                    if (!rsp[0].found) throw new Exception($"I/O \"{ioPath}\" not found");
                } else if (!rsp[0].success) {
                    throw new Exception($"Setting I/O \"{ioPath}\" failed");
                }
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>設定 I/O 狀態</summary>
        /// <param name="ioStt">I/O 編號與其對應狀態</param>
        public void SetIO(Dictionary<string, bool> ioStt) {
            try {
                var ioStr = ioStt.Keys.ToArray();
                var ioVal = ioStt.Values.Select(io => (double?)(io ? 1D : 0D)).ToArray();
                var rsps = mV2.writeIos(mSessionId.Value, ioStr, ioVal);
                if (rsps.Any(rsp => !rsp.success || !rsp.found)) {
                    var notFound = new List<string>();
                    var fail = new List<string>();
                    for (int idx = 0; idx < rsps.Length; idx++) {
                        var rsp = rsps[idx];
                        if (!rsp.found) {
                            notFound.Add(ioStt.ElementAt(idx).Key);
                        } else if (!rsp.success) {
                            fail.Add(ioStt.ElementAt(idx).Key);
                        }
                    }
                    if (notFound.Count > 0 && fail.Count > 0) {
                        throw new Exception($"I/O \"{string.Join(",", notFound)}\" are not found. Setting I/O \"{string.Join(",", fail)}\" failed");
                    } else if (notFound.Count > 0) {
                        throw new Exception($"I/O \"{string.Join(",", notFound)}\" are not found");
                    } else if (fail.Count > 0) {
                        throw new Exception($"Setting I/O \"{string.Join(",", fail)}\" failed");
                    }
                }
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }
        #endregion

        #region Function - Tasks
        /// <summary>取得當前已載入的應用程式清單</summary>
        /// <returns>應用程式清單</returns>
        public List<VAL3Application> GetApplications() {
            try {
                var apps = mV1.getApplications(mSessionId.Value);
                return apps.Select(app => new VAL3Application(app)).ToList();
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>取得當前已載入控制器的所有任務名稱</summary>
        /// <returns>任務名稱集合</returns>
        public List<VAL3Task> GetTasks() {
            try {
                var tsks = mV2.getTasks(mSessionId.Value);
                return tsks.Select(tsk => new VAL3Task(tsk)).ToList();
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>取得當前的任務狀態</summary>
        /// <param name="name">任務名稱</param>
        /// <returns>狀態</returns>
        public TaskState GetTaskState(string name) {
            try {
                var tsks = mV2.getTasks(mSessionId.Value);
                var tsk = tsks.FirstOrDefault(t => string.Equals(name, t.name));
                if (tsk == null) throw new ArgumentException("Invalid task name");
                return (TaskState)tsk.state;
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>執行 VAL3 程式</summary>
        /// <param name="prjName">專案名稱</param>
        /// <param name="tskName">程式名稱</param>
        /// <param name="appPath">Application 位址</param>
        public void TaskExecute(string prjName, string appPath, string tskName) {
            try {
                string retStr;
                var retCode = mV2.execVal3(mSessionId.Value, prjName, tskName, appPath, 0, string.Empty, out retStr);
                switch (retCode) {
                    case execVal3ReturnCode.EXVCOMPILATIONERROR:
                        throw new Exception("Compilation error");

                    case execVal3ReturnCode.EXVRUNTIMEERROR:
                        throw new Exception("Runtime error");

                    default:
                        break;
                }
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>暫停當前執行的任務</summary>
        /// <param name="tskName">任務名稱</param>
        /// <param name="appPath">Application 路徑，如 "Disk://TX90_Demo/TX90_Demo.pjx"</param>
        public void TaskSuspend(string appPath, string tskName) {
            try {
                mV2.taskSuspend(mSessionId.Value, tskName, appPath);
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>恢復已暫停的任務，讓其繼續運行</summary>
        /// <param name="tskName">程式名稱</param>
        /// <param name="appPath">Application 路徑，如 "Disk://TX90_Demo/TX90_Demo.pjx"</param>
        public void TaskResume(string appPath, string tskName) {
            try {
                mV2.taskResume(mSessionId.Value, tskName, appPath);
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>從當前的任務清單中刪除指定的任務</summary>
        /// <param name="tskName">任務名稱</param>
        /// <param name="appPath">Application 路徑，如 "Disk://TX90_Demo/TX90_Demo.pjx"</param>
        public void TaskKill(string appPath, string tskName) {
            try {
                mV2.taskKill(mSessionId.Value, tskName, appPath);
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }
        #endregion

        #region Function - Variables

        /// <summary>取得指定 Application 內，所有的 VAL3 全域變數及其數值</summary>
        /// <param name="appName">欲查詢的 Application 名稱，請注意大小寫。如 "Disk://Main/Main.pjx" (亦可簡化輸入 "Main" 即可)</param>
        /// <returns>變數資訊</returns>
        public List<VAL3Variable> GetVariables(string appName) {
            try {
                var apps = mV1.getApplications(mSessionId.Value);
                var pjxName = appName.EndsWith(".pjx") ? appName : appName + ".pjx";
                var app = apps.FirstOrDefault(val3App => val3App.name.EndsWith(pjxName));
                if (app == null) throw new ArgumentNullException("appName", "Can not find application that specified from parameter");
                var appData = mV1.getApplicationDatas(mSessionId.Value, app.name);
                List<VAL3Variable> val3Var = null;
                using (var memStream = new MemoryStream(appData)) {
                    var doc = XDocument.Load(memStream);

                    /* 因 .dtx 含有 namespace，查詢時都要加上 namespace */
                    var nsMang = new XmlNamespaceManager(new NameTable());
                    var nsName = string.Empty;
                    foreach (var attr in doc.Root.Attributes()) {
                        nsName = attr.Name.LocalName;
                        nsMang.AddNamespace("xmlns".Equals(nsName) ? "def" : nsName, attr.Value);
                    }

                    /* 用 XPath 的方式直接搜尋對應的 dtx 節點 */
                    var data = doc.XPathSelectElements($"def:Database/def:Datas/def:Data", nsMang);

                    /* 轉轉 */
                    val3Var = data.Select(elmt => new VAL3Variable(elmt)).ToList();
                }
                return val3Var;
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }

        /// <summary>取得當前 VAL3 全域變數數值</summary>
        /// <param name="appName">該變數所隸屬的 Application 名稱，請注意大小寫。如 "Disk://Main/Main.pjx" (亦可簡化輸入 "Main" 即可)</param>
        /// <param name="varName">欲查詢的變數名稱</param>
        /// <returns>變數資訊</returns>
        public VAL3Variable GetVariable(string appName, string varName) {
            try {
                var apps = mV1.getApplications(mSessionId.Value);
                var pjxName = appName.EndsWith(".pjx") ? appName : appName + ".pjx";
                var app = apps.FirstOrDefault(val3App => val3App.name.EndsWith(pjxName));
                if (app == null) throw new ArgumentNullException("appName", "Can not find application that specified from parameter");
                var appData = mV1.getApplicationDatas(mSessionId.Value, app.name);
                VAL3Variable val3Var = null;
                using (var memStream = new MemoryStream(appData)) {
                    var doc = XDocument.Load(memStream);

                    /* 因 .dtx 含有 namespace，查詢時都要加上 namespace */
                    var nsMang = new XmlNamespaceManager(new NameTable());
                    var nsName = string.Empty;
                    foreach (var attr in doc.Root.Attributes()) {
                        nsName = attr.Name.LocalName;
                        nsMang.AddNamespace("xmlns".Equals(nsName) ? "def" : nsName, attr.Value);
                    }

                    /* 用 XPath 的方式直接搜尋對應的 dtx 節點 */
                    var data = doc.XPathSelectElement($"def:Database/def:Datas/def:Data[@name='{varName}']", nsMang);

                    /* 轉轉 */
                    val3Var = new VAL3Variable(data);
                }
                return val3Var;
            } catch (SoapException soapEx) {
                throw ConvertSoapException(soapEx);
            }
        }
        #endregion
    }
}
