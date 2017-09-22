using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel;

using CtLib.Library;
using CtLib.Forms;

using Ace.Adept.Client;
using Ace.Adept.Server.Controls;
using Ace.Adept.Server.Desktop;
using Ace.Adept.Server.Desktop.Connection;
using Ace.Adept.Server.Motion;
using Ace.Adept.Server.Motion.Robots;
using Ace.Core.Client;
using Ace.Core.Server;
using Ace.Core.Server.Event;
using Ace.Core.Server.Program;
using Ace.Core.Util;
using Ace.HSVision.Client;
using Ace.HSVision.Client.Wizard;
using Ace.HSVision.Server.Control;
using Ace.HSVision.Server.Integration;
using Ace.HSVision.Server.Integration.ImageSources.Emulation;
using Ace.HSVision.Server.Tools;
using Ace.Process.Client;
using CtLib.Module.Ultity;

namespace CtLib.Module.Adept {
    /// <summary>
    /// Adept ACE 相關應用
    /// <para>Client 模式為以已開啟之 Adept ACE 為主要連線對象，所有項目以當前載入的 Workspace 為準</para>
    /// <para>Server 模式以 AceServer.exe 為主，需指定 Workspacce 檔案並於開啟後載入</para>
    /// <para>目前採用模組化方式，Variable/IO/Motion/Vision 請依各類別去操作</para>
    /// </summary>
    /// <example>
    /// 1. Client Mode
    /// <code>
    /// CtAce mAce = new CtAce(false);  //Don't raise message event
    /// AddEventHandler();              //Add CtAce events, like OnBoolEventChanged, OnNumericEventChanged and so on
    /// Stat stt = mAce.Connect(CtAce.ControllerType.WITH_SMARTCONTROLLER); //Connect with SmartController, you should set properties as you need
    /// </code>
    /// 2. Workspace Mode
    /// <code>
    /// CtAce mAce = new CtAce(false);  //Don't raise message event
    /// AddEventHandler();              //Add CtAce events, like OnBoolEventChanged, OnNumericEventChanged and so on
    /// mAce.ClientMode = false;        //Select to Server Mode
    /// mAce.WorkspacePath = @"D:\CASTEC\Project\Workspace\Demo.awp";       //Set the workspace path
    /// Stat stt = mAce.Connect(CtAce.ControllerType.WITH_SMARTCONTROLLER); //Connect with SmartController, you should set properties as you need
    /// </code>
    /// 3. Simply Operations (If you need more detail operations, please see each chapter)
    /// <code>
    /// /*-- Here is a REAL V+ variable reading/getting, you can change it as you need --*/
    /// float var1;
    /// mAce.Variable.GetValue("var1", out var1);   //Get "var1" real value as a float
    /// mAce.Variable.SetValue("var2", 3);          //Set "var2" value to 3
    /// 
    /// /*-- If target is NumericVariabe ot StringVariable (Not V+ object) --*/
    /// float num1;
    /// mAce.Variable.GetValue(@"/Vairable/Numeric1", out num1, CtAce.VariableType.NUMERIC_VARIABLE);   //Get "Numeric1" value
    /// mAce.Variable.SetValue(@"/Vairable/Numeric2", 9, CtAce.VariableType.NUMERIC_VARIABLE);          //Set "Numeric2" value
    /// 
    /// /*-- I/O operation --*/
    /// mAce.IO.SetIO(97);      //Set output "97" to ON
    /// mAce.IO.SetIO(-98);     //Set output "98" to OFF
    /// 
    /// bool io1 = mAce.IO.GetIO(1002);                             //Get input "1002" status and assign to io1 boolean variable
    /// List&lt;bool&gt; ioList = mAce.IO.GetIO(2001, 2002, 2003);  //Get multiple input and assign to List variable io2
    /// 
    /// /*-- Vision --*/
    /// float score;
    /// string visPath = @"/Vision/Demo/CVT1";
    /// CtAce.VisionToolType toolType = CtAce.VisionToolType.CSHARP_VISION_TOOL;
    /// mAce.Vision.ExecuteVisionTool(visPath, toolType, true, out score);  //Execute CVT (take new picture) and get the highest Matched Qulity Score 
    /// 
    /// /*-- Motion --*/
    /// mACE.Motion.MoveDistance(1, CtAce.Axis.ROLL, 90F);  //Let Robot1's theta (Joint4) do rotation with 90 degrees.
    /// </code>
    /// </example>
    public class CtAce : IDisposable {

        #region Version

        /// <summary>CtAce 版本訊息</summary>
        /// <remarks><code>
        /// 0.0.0  William [2012/07/03]
        ///     + CtAceServer
        /// 
        /// 1.0.0  Ahern [2014/08/31]
        ///     + 序列化，現可使用ACE本身的Event
        ///     + 使用EventHandler建立本身事件，可讓後續介面得知相關事件
        ///     \ 使用IAceServer.Root.Filter方式取代原有遞迴尋找內部SmartController/Robot方式
        ///     + 完成常用Function
        ///      
        /// 1.0.1  Ahern [2014/09/09]
        ///     - 序列化。發現僅將CtLib.exe搬移至Ace\Bin資料夾即可，不需施作序列化
        ///     + Vision相關應用，但目前無法正確獲取影像 (連AceDemo也不行)
        ///     + IDisposable，在Release時能先解除相關事件並釋放資源
        ///      
        /// 1.0.2  Ahern [2014/09/10]
        ///     + SaveWorkspace方法
        ///      
        /// 1.0.3  Ahern [2014/09/12]
        ///     \ 修改相關事件名稱
        ///     + OnMessage事件，以利將訊息拋出去
        ///     + Connection事件
        ///      
        /// 1.0.4  Ahern [2014/09/19]
        ///     + RaiseVisionWindow
        ///     + AddVisionHandle
        ///     + RemoveVisionHadle
        ///      
        /// 1.0.5  Ahern [2014/09/22]
        ///     \ 開啟電源之倒數視窗，改以CtProgress取代CtProgress_Ctrl
        ///      
        /// 1.1.0  Ahern [2014/10/19]
        ///     + ExecuteVisionTool
        ///      
        /// 1.1.1  Ahern [2014/10/28]
        ///     \ 統一Enumeration大小寫
        ///      
        /// 1.1.2  Ahern [2014/11/01]
        ///     \ ExecuteVisionTool之Execute改用False
        ///      
        /// 1.1.3  Ahern [2014/11/03]
        ///     + AddLocatorModel
        ///     + RemoveLocatorModel
        ///     + CreateLocatorModel
        ///      
        /// 1.1.4  Ahern [2014/11/04]
        ///     \ CreateLocatorModel
        ///     \ 原GetAceObject重新命名為FindObject，並新增兩組Overload (回傳IAceObjectCollection物件與路徑)
        ///     + DeleteObject，用於刪除物件
        ///     + GetLocatorModelNames，用於取得特定KeyWord之Model路徑
        ///     + GetCurrentModelNames，用於取得Locator內目前的Model路徑
        ///     \ ExecuteVisionTool之Execute改回True，不知為何有時無法取得新影像，切回true即可
        ///      
        /// 1.1.5  Ahern [2014/11/05]
        ///     + ExecuteVisionTool增加回傳 Match Quality (Locator) 或是 Area (Blob)
        ///      
        /// 1.1.6  Ahern [2014/11/06]
        ///     + EmulationCameraImageAdd
        ///     + EmulationCameraImageRemove
        ///      
        /// 1.1.7  Ahern [2014/11/07]
        ///     \ 原 VariableType 更改為 CtAceVariable.VPlusVariableType
        ///     + VariableType，區分為三種:V+/Numeric/String Variable
        ///      
        /// 1.1.8  Ahern [2014/11/08]
        ///     \ 修正部分註解
        ///      
        /// 1.2.0  Ahern [2014/11/26]
        ///     \ 將 I/O 獨立至 CtAceIO
        ///     \ 將 Variable 獨立至 CtAceVariable
        ///     \ 將 Task 獨立至 CtAceTask
        ///      
        /// 1.2.1  Ahern [2014/12/17]
        ///     \ ExceptionHandle 改採 StackTrace 模式
        ///      
        /// 1.2.2  Ahern [2015/02/06]
        ///     \ SetPower 修改 CtProgress 訊息與新增引數用於不要顯示進度條
        ///      
        /// 1.2.3  Ahern [2015/03/01]
        ///     - Try-Catch
        ///      
        /// 1.2.4  Ahern [2015/05/12]
        ///     \ Motion 加入 InRange 判斷
        ///      
        /// 1.3.0  Ahern [2015/05/22]
        ///     \ Server 模式，LoadWorkspace 後需要 Delay，否則會「无法进入通信渠道」
        ///     + EmulationMode
        ///     X 嘗試於 LoadLocalWorkspace 加入 Progress 事件，但會跳出非序列化
        ///     
        /// 1.3.1  Ahern [2015/05/27]
        ///     \ Task 部分增加保護確認
        /// 
        /// 1.3.2  Ahern [2015/06/01]
        ///     + CtAcePendant
        ///     
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 3, 2, "2015/06/01", "Ahern Kuo");

        #endregion

        #region Support Class
        /// <summary>Adept Controller與Robot集合</summary>
        private class AceObjects {
            public List<IAdeptController> Controllers = new List<IAdeptController>();
            public List<object> Robots = new List<object>();
        }

        /// <summary>Boolean事件參數</summary>
        public class BoolEventArgs : EventArgs {
            /// <summary>事件類別</summary>
            public BoolEvents Events { get; set; }
            /// <summary>數值</summary>
            public bool Value { get; set; }
            /// <summary>建立一個Boolean事件參數</summary>
            public BoolEventArgs(BoolEvents boolEvent, bool boolValue) {
                Events = boolEvent;
                Value = boolValue;
            }
        }

        /// <summary>數值變化事件參數</summary>
        public class NumericEventArgs : EventArgs {
            /// <summary>事件類別</summary>
            public NumericEvents Events { get; set; }
            /// <summary>數值</summary>
            public object Value { get; set; }
            /// <summary>建立一個數值變化的事件參數</summary>
            public NumericEventArgs(NumericEvents numEvent, object numValue) {
                Events = numEvent;
                Value = numValue;
            }
        }

        /// <summary>通知事件參數</summary>
        public class NotifyEventArgs : EventArgs {
            /// <summary>事件類別</summary>
            public NotifyEvents Events { get; set; }
            /// <summary>建立一個通知事件參數</summary>
            public NotifyEventArgs(NotifyEvents notifyEvent) {
                Events = notifyEvent;
            }
        }

        /// <summary>訊息事件參數</summary>
        public class MessageEventArgs : EventArgs {
            /// <summary>
            /// 訊息類型
            /// <para>-1: Alarm/Exception Message</para>
            /// <para>0: Normal Message</para>
            /// <para>1: Warning Message</para>
            /// </summary>
            public sbyte @Type { get; set; }

            /// <summary>訊息標題</summary>
            public string Title { get; set; }

            /// <summary>訊息內容</summary>
            public string Message { get; set; }

            /// <summary>建構一新的訊息事件參數</summary>
            /// <param name="msgType">
            /// 訊息類型
            /// <para>-1: Alarm/Exception Message</para>
            /// <para>0: Normal Message</para>
            /// <para>1: Warning Message</para>
            /// </param>
            /// <param name="msgTitle">訊息標題</param>
            /// <param name="msgContent">訊息內容</param>
            public MessageEventArgs(sbyte msgType, string msgTitle, string msgContent) {
                Type = msgType;
                Title = msgTitle;
                Message = msgContent;
            }
        }

        #endregion

        #region Declaration - Enumeration

        /// <summary>Boolean事件集合</summary>
        public enum BoolEvents : byte {
            /// <summary>電源狀態變更</summary>
            POWER_CHANGED,
            /// <summary>連線狀態變更</summary>
            CONNECTION
        }

        /// <summary>數值變化事件集合</summary>
        public enum NumericEvents : byte {
            /// <summary>
            /// Monitor Speed 變更
            /// <para>事件的數值型態為 int</para>
            /// </summary>
            SPEED_CHANGED,
            /// <summary>
            /// Task狀態發生變動
            /// <para>事件的數值為 object[] { (string)TaskName, (TaskState)CurrentTaskState }</para>
            /// </summary>
            TASK_STATE_CHANGED,
        }

        /// <summary>通知事件集合</summary>
        public enum NotifyEvents : byte {
            /// <summary>Workspace已載入</summary>
            WORKSPACE_LOAD,
            /// <summary>Workspace已完成儲存</summary>
            WORKSPACE_SAVE,
            /// <summary>Workspace已卸載，或是Adept ACE關閉</summary>
            WORKSPACE_UNLOAD,
            /// <summary>程式已被編輯</summary>
            PROGRAM_MODIFIED,
            /// <summary>ACE軟體關閉</summary>
            ACE_SHUTDOWN
        }

        /// <summary>
        /// 專案是否有存在SmartController
        /// <para>此將影響連線方式及相關物件建立方式</para>
        /// </summary>
        public enum ControllerType : byte {
            /// <summary>含有 SmartController</summary>
            WITH_SMARTCONTROLLER = 0,
            /// <summary>不含 SmartController</summary>
            WITHOUT_SMARTCONTROLLER = 1
        }

        /// <summary>Adept Industrial Robot Series</summary>
        public enum RobotType : byte {
            /// <summary>Adept Python Linear Modules</summary>
            PYTHON = 1,
            /// <summary>Custom X-Y tables</summary>
            LINEAR_MODULE = 2,
            /// <summary>Adept iCobra SCARA Robots (Without SmartController, using RS-232)</summary>
            I_COBRA = 3,
            /// <summary>Adept sCobra SCARA Robots (With SmartController)</summary>
            S_COBRA = 4,
            /// <summary>Adept Quattro Parallel Robots</summary>
            QUATTRO = 5,
            /// <summary>Adept Viper Six-Axis Robots</summary>
            VIPER = 6
        }

        /// <summary>座標軸</summary>
        /// <remarks>0-Base</remarks>
        public enum Axis : byte {
            /// <summary>X</summary>
            X = 0,
            /// <summary>Y</summary>
            Y = 1,
            /// <summary>Z</summary>
            Z = 2,
            /// <summary>Yaw 或稱 rX</summary>
            YAW = 3,
            /// <summary>Pitch 或稱 rY</summary>
            PITCH = 4,
            /// <summary>Roll 或稱 rZ 、 Theta</summary>
            ROLL = 5
        }

        /// <summary>Joint 列舉</summary>
        /// <remarks>0-Base</remarks>
        public enum Joint : byte {
            /// <summary>第一軸</summary>
            JOINT_1 = 0,
            /// <summary>第二軸</summary>
            JOINT_2 = 1,
            /// <summary>第三軸</summary>
            JOINT_3 = 2,
            /// <summary>第四軸</summary>
            JOINT_4 = 3,
            /// <summary>第五軸</summary>
            JOINT_5 = 4,
            /// <summary>第六軸</summary>
            JOINT_6 = 5
        }

        /// <summary>路徑模式</summary>
        public enum MotionMode : byte {
            /// <summary>直線模式。以兩點間最短距離來移動</summary>
            STRIGHT_LINE = 0,
            /// <summary>Joint內插。以 Robot Joint 最短距離來移動</summary>
            JOINTS_INTERPOLATED = 1
        }

        /// <summary>移動到點模式</summary>
        public enum MoveEndMode : byte {
            /// <summary>
            /// 滑順到點
            /// <para>若於開始減速前設定下一個命令，會以滑順的方式往下一點前進</para>
            /// <para>無須設置百分比</para>
            /// </summary>
            BLEND = 1,
            /// <summary>完整到點</summary>
            NONULL = 0,
            /// <summary>
            /// 粗略百分比
            /// <para>粗略到達設置的百分比後(精準度較低)，往下一點前進</para>
            /// <para>請設置百分比，預設為 80%</para>
            /// </summary>
            SETTLE_COARSE = 4,
            /// <summary>
            /// 精準百分比
            /// <para>到達設置的百分比後(較高精準度)，往下一點前進</para>
            /// <para>請設置百分比，預設為 80%</para>
            /// </summary>
            SETTLE_FINE = 2
        }

        /// <summary>速度模式</summary>
        public enum SpeedMode : byte {
            /// <summary>
            /// 百分比
            /// <para>適用於stright-line與joints-interpolated</para>
            /// </summary>
            PERCENT = 0,
            /// <summary>
            /// 毫米/每秒 (MMPS)
            /// <para>僅適用於stright-line</para>
            /// </summary>
            MILLIMETER_PER_SECOND = 1,
            /// <summary>
            /// 英吋/每秒 (IPS)
            /// <para>僅適用於stright-line</para>
            /// </summary>
            INCHES_PER_SECOND = 2
        }

        /// <summary>Jog 模式</summary>
        public enum JogMode : byte {
            /// <summary>Manual mode without selection</summary>
            NONE = 0,
            /// <summary>Free-joint mode</summary>
            FREE = 1,
            /// <summary>Individual joint control</summary>
            JOINT = 2,
            /// <summary>World coordinates control</summary>
            WORLD = 3,
            /// <summary>Tool coordinates control</summary>
            TOOL = 4,
            /// <summary>Computer control enabled</summary>
            COMP = 5,
            /// <summary>Jogging toward a joint location</summary>
            JOG_TO_JOINT_LOCATION = 6,
            /// <summary>Jogging toward a world location</summary>
            JOG_TO_WORLD_LOCATION = 7
        }

        /// <summary>Adept ACE Task 執行狀態</summary>
        public enum TaskState : sbyte {
            /// <summary>閒置，該Task尚未開始執行</summary>
            IDLE = 0,
            /// <summary>執行中</summary>
            EXECUTING = 4,
            /// <summary>發生例外而停止</summary>
            EXCEPTION = 2,
            /// <summary>暫停，但沒有發生任何例外情況</summary>
            PAUSED = 5,
            /// <summary>中斷或取消，同時會觸發例外狀況</summary>
            ABORT = 3,
            /// <summary>執行完畢</summary>
            COMPLETE = 1,
            /// <summary>違法事項</summary>
            INVALID = -1
        }

        /// <summary>Adept ACE V+ 變數類型</summary>
        public enum VPlusVariableType : byte {
            /// <summary>實數，對應於float或double</summary>
            REAL = 0,
            /// <summary>字串</summary>
            @STRING = 1,
            /// <summary>世界座標，對應於Transform3D</summary>
            LOCATION = 2,
            /// <summary>Joint，對應於PrecisionPoint</summary>
            PRECISION_POINT = 3
        }

        /// <summary>Adept ACE 變數種類</summary>
        public enum VariableType : byte {
            /// <summary>V+ 變數，如 Real、Location、PrecisionPoint、String等</summary>
            VPLUS_VARIABLE = 0,
            /// <summary>Numeric Variable，即 IVariableNumeric，僅 float 數值</summary>
            NUMERIC_VARIABLE = 1,
            /// <summary>String Variable，即 IVariableString，僅 String 數值</summary>
            STRING_VARIABLE = 2
        }

        /// <summary>支援的 Vision Tool 類型</summary>
        public enum VisionToolType : byte {
            /// <summary>攝影機</summary>
            VISION_SOURCE = 0,
            /// <summary>Locator Tool</summary>
            LOCATOR = 1,
            /// <summary>Custom Vision Tool (CVT)</summary>
            CSHARP_VISION_TOOL = 2,
            /// <summary>Blob Analyzer Tool</summary>
            BLOB_ANALYZER = 3,
            /// <summary>ImageProcessing Tool</summary>
            IMAGE_PROCESSING = 4
        }
        #endregion

        #region Declaration - Definitions

        /// <summary>預設的程式延遲(毫秒)</summary>
        private static readonly int PROGRAM_DELAY = 100;

        /// <summary>Adept AceServer 執行檔名稱，用於開啟或強制關閉服務</summary>
        private static readonly string ACE_ACESERVER_NAME = "aceserver";
        /// <summary>Adept ACE 執行檔路徑</summary>
        private static readonly string ACE_APPLICATION_PATH = @"C:\Program Files\Adept Technology\Adept ACE\bin\Ace.exe";

        /// <summary>Adept ACE Server 名稱</summary>
        private static readonly string ACE_REMOTING_NAME = "ace";
        /// <summary>Adept ACE Server IP</summary>
        private static readonly string ACE_SERVER_IP = "localhost";
        /// <summary>Adept ACE Server Port</summary>
        private static readonly int ACE_SERVER_PORT = 43434;

        /// <summary>整體速度之最低數值，用於限制整體速度</summary>
        private static readonly int ACE_MINSPEED = 0;
        /// <summary>整體速度之可接受最大數值，用於限制整體速度</summary>
        private static readonly int ACE_MAXSPEED = 100;

        #endregion

        #region Declaration - Properties

        /// <summary>取得 ACE Server 是否為模擬環境</summary>
        public bool EmulationMode { get; internal set; }

        /// <summary>取得目前是否已與AceServer連線</summary>
        public bool IsConnected {
            get {
                return (mVpLink != null) ? mVpLink.IsOnline : false;
            }
        }

        /// <summary>取得目前HighPower狀態</summary>
        public bool IsPower {
            get {
                bool bolPwr = false;
                if ((!mSmartCtrl) && (mAceObj.Robots.Count > 0)) {
                    bolPwr = (mAceObj.Robots[0] as iCobra).HighPower;
                } else if (mICtrl != null) {
                    bolPwr = mICtrl.HighPower;
                }
                return bolPwr;
            }
        }

        /// <summary>取得目前ACE介面上之整體速度 (MonitorSpeed)</summary>
        public int Speed {
            get {
                int intSpd = -1;
                if ((!mSmartCtrl) && (mAceObj.Robots.Count > 0)) {
                    intSpd = (mAceObj.Robots[0] as iCobra).MonitorSpeed;
                } else if (mICtrl != null) {
                    intSpd = mICtrl.MonitorSpeed;
                }
                return intSpd;
            }
        }

        /// <summary>取得或設定與ACE連線模式。 True:Client  False:Server</summary>
        public bool ClientMode {
            get { return mClientMode; }
            set { mClientMode = value; }
        }

        /// <summary>取得或設定遠端名稱，用於與AceServer連線</summary>
        public string RemotingName {
            get { return mACE_Name; }
            set { mACE_Name = value; }
        }

        /// <summary>取得或設定AceServer之IP Address</summary>
        public string ServerIP {
            get { return mACE_IP; }
            set { mACE_IP = value; }
        }

        /// <summary>取得或設定AceServer之連線埠</summary>
        public int ServerPort {
            get { return mACE_Port; }
            set { mACE_Port = value; }
        }

        /// <summary>取得或設定Workspace路徑，如 @"D:\Workspace\E1404_1-1-4.awp"</summary>
        [DefaultValue(@"D:\Workspace\CASTEC.awp")]
        public string WorkspacePath { get; set; }

        /// <summary>取得或設定訊息發報至Event。如開啟將可透過OnMessage事件取得Exception或是相關訊息</summary>
        [DefaultValue(true)]
        public bool EnableMessage { get; set; }

        /// <summary>取得當前存在於AceServer中之SmartController</summary>
        public List<string> Controllers {
            get {
                List<string> lstCtrl = new List<string>();
                if (mAceObj.Controllers.Count > 0) {
                    for (int i = 0; i < mAceObj.Controllers.Count; i++) {
                        lstCtrl.Add(mAceObj.Controllers[i].FullPath);
                    }
                } else
                    lstCtrl = null;
                return lstCtrl;
            }
        }

        /// <summary>取得當前存在於AceServer中之IAdeptRobot</summary>
        public List<string> Robots {
            get {
                List<string> lstCtrl = new List<string>();
                if (mAceObj.Robots.Count > 0) {
                    for (int i = 0; i < mAceObj.Robots.Count; i++) {
                        if (mSmartCtrl)
                            lstCtrl.Add((mAceObj.Robots[i] as IAdeptRobot).FullPath);
                        else
                            lstCtrl.Add((mAceObj.Robots[i] as iCobra).FullPath);
                    }
                } else
                    lstCtrl = null;
                return lstCtrl;
            }
        }

        /// <summary>I/O 相關控制模組</summary>
        public CtAceIO IO { get { return mIO; } }

        /// <summary>變數控制模組</summary>
        public CtAceVariable Variable { get { return mVariables; } }

        /// <summary>Task相關控制模組</summary>
        public CtAceTask Tasks { get { return mTask; } }

        /// <summary>Robot移動相關控制模組</summary>
        public CtAceMotion Motion { get { return mMotion; } }

        /// <summary>VisionEvents及影像處理控制模組</summary>
        public CtAceVision Vision { get { return mVision; } }

        #endregion

        #region Declaration - Members
        /*-- ACE Socket Setting --*/
        /// <summary>Adept ACE Server IP</summary>
        private string mACE_Name = ACE_REMOTING_NAME;
        /// <summary>Adept ACE Server IP</summary>
        private string mACE_IP = ACE_SERVER_IP;
        /// <summary>Adept ACE Server Port</summary>
        private int mACE_Port = ACE_SERVER_PORT;

        /*-- ACE Objects --*/
        /// <summary>Adept ACE IAceClient Interface</summary>
        private IAceClient mIClient;
        /// <summary>Adept ACE IAceServer Interface</summary>
        private IAceServer mIServer;
        /// <summary>Adept ACE IAdeptController Interface</summary>
        private IAdeptController mICtrl;
        /// <summary>Adept ACE IVpLink Interface</summary>
        private IVpLink mVpLink;
        /// <summary>ACE 相關物件集合</summary>
        private AceObjects mAceObj = new AceObjects();
        /// <summary>目前Task Cotrol裡最大Task編號。如CX共有0~24(25個)，則將回傳24</summary>
        private int mMaxTaskNum = -1;
        /// <summary>用於初始化 Adept ACE GUI</summary>
        private IAdeptGuiPlugin mIGuiPlug;

        /*-- ACE Handler --*/
        ///// <summary>ACE Application EventHandler, using for IAdeptController Events</summary>
        //RemoteApplicationEventHandler mAppHdl;
        /// <summary>ACE AceObject EventHandler, using for moniting IAdeptController Properties Modifiy and Dispose Event</summary>
        RemoteAceObjectEventHandler mObjHdl;
        /// <summary>ACE Task EventHandler, using for moniting a Task state chagned</summary>
        RemoteTaskEventHandler mTskHdl;

        /*-- Flags --*/
        /// <summary>[Flag] 用於確認ACE是否開啟，連線後改為False，如遇到第一次IAdeptController.Dispose則改為True</summary>
        /// <remarks>由於Dispose事件會發好幾次，故利用此Flag去卡住，不要連續發Event出去</remarks>
        private bool mAceDispose = true;

        /*-- Others --*/
        /// <summary>是否為Client端? True:Client False:Server</summary>
        private bool mClientMode = true;
        /// <summary>是否含有SmartController</summary>
        private bool mSmartCtrl = true;
        /// <summary>顯示Vision畫面之清單，如為空則全部顯示</summary>
        private List<string> mVisionFreeze = new List<string>();

        /*-- Sub Modules --*/
        /// <summary>[Module] I/O Control</summary>
        private CtAceIO mIO;
        /// <summary>[Module] Variable Controls</summary>
        private CtAceVariable mVariables;
        /// <summary>[Module] Task Controls</summary>
        private CtAceTask mTask;
        /// <summary>[Module] Motion Controls</summary>
        private CtAceMotion mMotion;
        /// <summary>[Module] Vision Controls</summary>
        private CtAceVision mVision;
        #endregion

        #region Declaration - Events
        /// <summary>發生Boolean值改變事件</summary>
        public event EventHandler<BoolEventArgs> OnBoolEventChanged;
        /// <summary>發生特定數值改變事件</summary>
        public event EventHandler<NumericEventArgs> OnNumericEventChanged;
        /// <summary>發生需通知外部之事件</summary>
        public event EventHandler<NotifyEventArgs> OnNotifyEventChanged;
        /// <summary>發生Task通知事件</summary>
        public event EventHandler<CtAceTask.TaskEventArgs> OnTaskChanged;
        /// <summary>發生顯示訊息事件</summary>
        public event EventHandler<MessageEventArgs> OnMessage;

        /// <summary>觸發Boolean改變事件</summary>
        /// <param name="e">Boolean事件參數</param>
        protected virtual void OnBoolEventOccur(BoolEventArgs e) {
            EventHandler<BoolEventArgs> handler = OnBoolEventChanged;
            if (handler != null) handler(this, e);
        }

        /// <summary>觸發數值改變事件</summary>
        /// <param name="e">數值事件參數></param>
        protected virtual void OnNumericEventOccur(NumericEventArgs e) {
            EventHandler<NumericEventArgs> handler = OnNumericEventChanged;
            if (handler != null) handler(this, e);
        }

        /// <summary>觸發通知事件</summary>
        /// <param name="e">通知事件之參數</param>
        protected virtual void OnNotifyEventOccur(NotifyEventArgs e) {
            EventHandler<NotifyEventArgs> handler = OnNotifyEventChanged;
            if (handler != null) handler(this, e);
        }

        /// <summary>觸發Task狀態變更事件</summary>
        /// <param name="e">Task事件之參數</param>
        protected virtual void OnTaskEventOccur(CtAceTask.TaskEventArgs e) {
            EventHandler<CtAceTask.TaskEventArgs> handler = OnTaskChanged;
            if (handler != null) handler(this, e);
        }

        /// <summary>觸發UpdateMessage事件</summary>
        /// <param name="e">Message事件參數</param>
        protected virtual void OnMessageUpdate(MessageEventArgs e) {
            EventHandler<MessageEventArgs> handler = OnMessage;
            if (handler != null) handler(this, e);
        }

        #endregion

        #region Function - Constructor
        /// <summary>建立空白的CtAce元件</summary>
        /// <param name="enbMsg">是否透過OnMessage事件取得Exception或是相關訊息</param>
        public CtAce(bool enbMsg = true) {
            EnableMessage = enbMsg;
        }

        /// <summary>建立後進行所有元件之連線，並尋找控制器與手臂</summary>
        /// <param name="ctrl">是否含有SmartController。此將影響連線與建立物件方式</param>
        /// <param name="enbMsg">是否透過OnMessage事件取得Exception或是相關訊息</param>
        public CtAce(ControllerType ctrl, bool enbMsg = true) {
            EnableMessage = enbMsg;
            Connect(ctrl);
        }

        ///// <summary>用於序列化之建構元</summary>
        //protected CtAce(SerializationInfo info, StreamingContext context)
        //    : base(info, context) {

        //    mIClient = (IAceClient) info.GetValue("mIClient", typeof(IAceClient));
        //    mIServer = (IAceServer) info.GetValue("mIAceIAceServer", typeof(IAceServer));
        //    mICtrl = (IAdeptController) info.GetValue("mICtrl", typeof(IAdeptController));
        //    mVpLink = (IVpLink) info.GetValue("mVpLink", typeof(IVpLink));

        //    mACE_Name = info.GetString("mACE_Name");
        //    mACE_IP = info.GetString("mACE_IP");
        //    mACE_Port = info.GetInt32("mACE_Port");
        //    mMaxTaskNum = info.GetInt32("mMaxTaskNum");

        //    mAceObj = (AceObjects) info.GetValue("mAceObj", typeof(AceObjects));

        //    mFirstInitial = info.GetBoolean("mFirstInitial");
        //    mAceDispose = info.GetBoolean("mAceDispose");
        //    mClientMode = info.GetBoolean("mClientMode");
        //    mRobotType = (RobotType) info.GetValue("mRobotType", typeof(RobotType));
        //    mMonitorTask = (List<TaskInfo>) info.GetValue("mMonitorTask", typeof(List<TaskInfo>));
        //    mMonitorThread = (Thread) info.GetValue("mMonitorThread", typeof(Thread));
        //    mIVisPlug = (IVisionPlugin) info.GetValue("mIVisPlug",typeof(IVisionPlugin));
        //    mIGuiPlug = (IAdeptGuiPlugin) info.GetValue("mIGuiPlug",typeof(IAdeptGuiPlugin));
        //    mVisSrvHdl = (VisionServerEventHandler) info.GetValue("mVisSrvHdl",typeof(VisionServerEventHandler));
        //}

        ///// <summary>複寫AceObject序列化之資料建立</summary>
        //public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        //    base.GetObjectData(info, context);

        //    info.AddValue("mIClient", mIClient);
        //    info.AddValue("mIServer", mIServer);
        //    info.AddValue("mICtrl", mICtrl);
        //    info.AddValue("mVpLink", mVpLink);
        //    info.AddValue("mACE_Name", mACE_Name);
        //    info.AddValue("mACE_IP", mACE_IP);
        //    info.AddValue("mACE_Port", mACE_Port);
        //    info.AddValue("mMaxTaskNum", mMaxTaskNum);
        //    info.AddValue("mAceObj", mAceObj);
        //    info.AddValue("mFirstInitial", mFirstInitial);
        //    info.AddValue("mAceDispose", mAceDispose);
        //    info.AddValue("mClientMode", mClientMode);
        //    info.AddValue("mRobotType", mRobotType);
        //    info.AddValue("mMonitorTask", mMonitorTask);
        //    info.AddValue("mMonitorThread", mMonitorThread);
        //    info.AddValue("mIVisPlug",mIVisPlug);
        //    info.AddValue("mIGuiPlug", mIGuiPlug);
        //    info.AddValue("mVisSrvHdl",mVisSrvHdl);
        //}

        #endregion

        #region Function - Method

        /// <summary>集中發報Exception</summary>
        /// <param name="stt">Status</param>
        /// <param name="title">Title</param>
        /// <param name="msg">Message Content</param>
        private void ExceptionHandle(Stat stt, string title, string msg) {
            if (EnableMessage) {
                sbyte msgTyp = 0;
                if ((int)stt == 0) {
                    msgTyp = 0;
                } else if ((int)stt < 0) {
                    msgTyp = -1;
                } else {
                    msgTyp = 1;
                }
                OnMessageUpdate(new MessageEventArgs(msgTyp, title, msg));
            }
            CtStatus.Report(stt, title, msg);
        }

        /// <summary>集中發報Exception</summary>
        /// <param name="stt">Status Code</param>
        /// <param name="ex">Exception</param>
        private void ExceptionHandle(Stat stt, Exception ex) {
            string method = "";
            CtStatus.Report(stt, ex, out method);
            if (EnableMessage) {
                sbyte msgTyp = 0;
                if ((int)stt == 0) {
                    msgTyp = 0;
                } else if ((int)stt < 0) {
                    msgTyp = -1;
                } else {
                    msgTyp = 1;
                }
                OnMessageUpdate(new MessageEventArgs(msgTyp, method, ex.Message));
            }

        }

        /// <summary>印出 IAceObject 裡面的名稱以及型態</summary>
        /// <param name="item">欲掃描的 IAceObject</param>
        /// <param name="dumpTemp">要儲存的 List(Of string) 回傳格式為 "Name || Type"</param>
        private void DumpObject(IAceObject item, ref List<string> dumpTemp) {
            if (item.GetType().ToString().Contains("AceObjectCollection")) {
                foreach (IAceObject subItem in (item as IAceObjectCollection).ToArray()) {
                    DumpObject(subItem, ref dumpTemp);
                }
            } else {
                dumpTemp.Add(item.Name + " || " + item.GetType().ToString());
            }
        }

        /// <summary>連線，並傳遞已建立之IAceServer/IAceClient元件</summary>
        /// <param name="iServer">回傳已建立之IAceServer元件</param>
        /// <param name="iClient">回傳已建立之IAceClient元件</param>
        /// <param name="client">是否為Client端模式 (True)Client  (False)Server</param>
        /// <returns>Status Code</returns>
        /// <remarks>如果有遇到系統於 AceClient (@825) 卡住，請於 App.config 檢查是否有 useLegacyV2RuntimeActivationPolicy="true"</remarks>
        private Stat Connect(out IAceServer iServer, out IAceClient iClient, bool client = true) {
            Stat stt = Stat.SUCCESS;
            IAceServer aceServer = null;
            IAceClient aceClient = null;

            /*-- 定義Port --*/
            int intRemotingPort = (client) ? 0 : mACE_Port;
            int intServerPort = (client) ? RemotingUtil.DefaultRemotingPort : mACE_Port;

            /*-- 如果是Server模式，則關閉ACE並開啟Server --*/
            if (!client) {
                try {
                    CtApplication.KillService(ACE_ACESERVER_NAME);
                } catch (Exception) {
                    /*-- 不確定 Ace.exe 會不會有跳 Service 出來，如果沒有會跳錯誤，所以用 try-catch 包住 --*/
                }
                Thread.Sleep(PROGRAM_DELAY);
                CtApplication.ExecuteProcess(ACE_APPLICATION_PATH, "server");
                Thread.Sleep(1000);
            }

            /*-- 初始化ACE子系統 --*/
            RemotingUtil.InitializeRemotingSubsystem(true, intRemotingPort);

            /*-- 建立IAceServer與IAceClient --*/
            aceServer = (IAceServer)RemotingUtil.GetRemoteServerObject(typeof(IAceServer), mACE_Name, mACE_IP, intServerPort);

            /*-- 建立 IAceClient，如果此處會卡很久，請檢查 App.config 是否有加入 useLegacyV2RuntimeActivationPolicy="true" --*/
            aceClient = new AceClient(aceServer);

            /*-- 初始化所有Plugins，方便後續Jog Pendant等方便建立 --*/
            aceClient.InitializePlugins(null);

            /*-- 如果均成功建立，將Dispose的Flag改為False表示ACE活著 --*/
            if ((aceServer != null) && (aceClient != null))
                mAceDispose = false;

            Thread.Sleep(500);

            iServer = aceServer;
            iClient = aceClient;
            return stt;
        }

        /// <summary>載入Workspace</summary>
        /// <param name="path">awp檔案路徑，如 @"D:\Workspace\E1404_1-1-4.awp"</param>
        /// <param name="waitTime">等待逾時時間，如超過將自動停止並回報錯誤。預設為 5 分鐘 (5*60*1000 = 300000ms)</param>
        /// <returns>Status Code</returns>
        private Stat LoadWorkspace(string path, int waitTime = 300000) {
            Stat stt = Stat.SUCCESS;
            try {
                if (path.Length < 1) {
                    stt = Stat.ER_SYS_ILFLPH;
                    throw (new Exception("檔案路徑錯誤，請檢查路徑是否正確！"));
                }

                Stopwatch sw = new Stopwatch();

                Thread tLoadWorkspace = CtThread.CreateThread("CtAce_LoadWorkspace", tsk_LoadWorkspace);
                tLoadWorkspace.Start(path);

                sw.Start();
                do {
                    Application.DoEvents();
                    Thread.Sleep(PROGRAM_DELAY);
                } while ((sw.ElapsedMilliseconds < waitTime) && (tLoadWorkspace.IsAlive));
                sw.Stop();
                CtThread.KillThread(ref tLoadWorkspace);

                if (sw.ElapsedMilliseconds >= waitTime) {
                    stt = Stat.ER_SYS_FILACC;
                    throw (new Exception("等待載入 Workspace 過久，請檢查相關設定"));
                }

            } catch (Exception ex) {
                if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
                ExceptionHandle(stt, ex);
            }
            return stt;
        }

        /// <summary>取得並分析IAceServer內部物件，如有分析到Controller或是Robot則加入集合</summary>
        /// <param name="aceServer">IAceServer物件</param>
        /// <returns>Status Code</returns>
        private Stat LinkAceObj(IAceServer aceServer) {
            Stat stt = Stat.SUCCESS;
            try {
                /*-- 搜尋IAceServer裡的物件，並過濾出IAdeptController (→SmartController)物件並加入集合 --*/
                foreach (IAdeptController ctrl in aceServer.Root.Filter(new ObjectTypeFilter(typeof(IAdeptController)), true)) {
                    mAceObj.Controllers.Add(ctrl);
                }

                /*-- 搜尋IAceServer裡的物件，並過濾出Robot物件並加入集合 --*/
                if (!mSmartCtrl) {
                    foreach (iCobra robot in aceServer.Root.Filter(new ObjectTypeFilter(typeof(iCobra)), true)) {
                        mAceObj.Robots.Add(robot);
                    }

                    /*-- 以下為遞迴尋找 aceServer 之內部物件，並將之儲存檔案之示範 --*/
                    //List<string> strNames = new List<string>();
                    //foreach (IAceObject item in aceServer.Root.ToArray()) {
                    //    DumpObject(item, ref strNames);
                    //}
                    //System.IO.File.WriteAllLines(@"D:\AceServerRoot.log", strNames.ToArray());

                } else {
                    foreach (IAdeptRobot robot in aceServer.Root.Filter(new ObjectTypeFilter(typeof(IAdeptRobot)), true)) {
                        mAceObj.Robots.Add(robot);
                    }
                }
            } catch (Exception ex) {
                stt = Stat.ER_SYSTEM;
                ExceptionHandle(stt, ex);
            }
            return stt;
        }

        /// <summary>取得目前IAceClient。此方法並無確保IAceClient已連上並建立</summary>
        /// <returns>當前的IAceClient</returns>
        public IAceClient GetClient() {
            return mIClient;
        }

        /// <summary>取得目前IAceServer。此方法並無確保IAceServer已連上並建立</summary>
        /// <returns>當前的IAceServer</returns>
        public IAceServer GetServer() {
            return mIServer;
        }

        /// <summary>取得ACE內部物件(IAceServer.Root)</summary>
        /// <param name="path">欲取得物件之路徑</param>
        /// <returns>回傳之路徑</returns>
        public object FindObject(string path) {
            object objTemp = null;
            if ((path != "") && (mIServer != null)) objTemp = mIServer.Root[path];
            return objTemp;
        }

        /// <summary>
        /// 儲存Workspace
        /// <para>此方法以當前儲存路徑為主，如要更改路徑請用SaveWorkspaceAs()</para>
        /// </summary>
        /// <returns>Status Code</returns>
        public Stat SaveWorkspace() {
            Stat stt = Stat.SUCCESS;
            try {
                bool bolResult = mIClient.SaveWorkspace(null);
                if (!bolResult)
                    stt = Stat.WN_ACE_USRCNC;
            } catch (Exception ex) {
                if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
                ExceptionHandle(stt, ex);
            }
            return stt;
        }

        /// <summary>
        /// Workspace另存新檔
        /// <para>此方法將跳出路徑視窗，並讓使用者選擇儲存路徑與檔名</para>
        /// </summary>
        /// <returns>Status Code</returns>
        public Stat SaveWorkspaceAs() {
            Stat stt = Stat.SUCCESS;
            try {
                bool bolResult = mIClient.SaveWorkspaceAs(null);
                if (!bolResult)
                    stt = Stat.WN_ACE_USRCNC;
            } catch (Exception ex) {
                if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
                ExceptionHandle(stt, ex);
            }
            return stt;
        }

        /// <summary>
        /// Workspace另存新檔
        /// <para>此方法將直接指定儲存Workspace至特定路徑</para>
        /// </summary>
        /// <param name="path">指定存檔路徑</param>
        /// <param name="overWrite">如檔案已經存在是否覆蓋？ True:覆蓋 False:放棄並發Exception</param>
        /// <returns>Status Code</returns>
        public Stat SaveWorkspaceAs(string path, bool overWrite = true) {
            Stat stt = Stat.SUCCESS;
            try {
                string strPath = mIServer.SaveLocalWorkspace(path, null);
                CtFile.CopyFile(strPath, path, overWrite);
                if (stt == Stat.SUCCESS)
                    CtFile.DeleteFile(strPath);
            } catch (Exception ex) {
                if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
                ExceptionHandle(stt, ex);
            }
            return stt;
        }

        /// <summary>取得當前MonitorSpeed為多少，並透過Event的方式回傳</summary>
        public void RequestSpeed() {
            int intSpd = -1;
            if ((!mSmartCtrl) && (mAceObj.Robots.Count > 0)) {
                intSpd = (mAceObj.Robots[0] as iCobra).MonitorSpeed;
            } else if (mICtrl != null) {
                intSpd = mICtrl.MonitorSpeed;
            }
            OnNumericEventOccur(new NumericEventArgs(NumericEvents.SPEED_CHANGED, intSpd));
        }

        /// <summary>取得當前HighPower狀態，並透過Event的方式回傳</summary>
        public void RequestPower() {
            bool bolPwr = false;
            if ((!mSmartCtrl) && (mAceObj.Robots.Count > 0)) {
                bolPwr = (mAceObj.Robots[0] as iCobra).HighPower;
            } else if (mICtrl != null) {
                bolPwr = mICtrl.HighPower;
            }
            OnBoolEventOccur(new BoolEventArgs(BoolEvents.POWER_CHANGED, bolPwr));
        }

        /// <summary>清除SmartController裡之變數、程式</summary>
        /// <returns>Status Code</returns>
        public Stat ZeroMemory() {
            Stat stt = Stat.SUCCESS;
            try {
                mICtrl.ZeroMemory(null);
            } catch (Exception ex) {
                stt = Stat.ER_SYSTEM;
                ExceptionHandle(stt, ex);
            }
            return stt;
        }

        /// <summary>從已連線之 AceServer 尋找特定路徑裡之物件，將回傳該路徑下所有物件</summary>
        /// <param name="path">欲取的物件集合之路徑</param>
        /// <param name="obj">該路徑下之物件集合</param>
        /// <returns>Status Code</returns>
        public Stat FindObject(string path, out List<object> obj) {
            Stat stt = Stat.SUCCESS;
            List<object> objTemp = new List<object>();
            try {
                if ((path != "") && (mIServer != null)) {
                    foreach (var item in (mIServer.Root[path] as IAceObjectCollection).ToArray()) {
                        objTemp.Add(item);
                    }
                }
            } catch (Exception ex) {
                if (stt == Stat.SUCCESS) stt = Stat.WN_ACE_ACCFAIL;
                ExceptionHandle(stt, ex);
            }
            obj = objTemp;
            return stt;
        }

        /// <summary>從已連線之 AceServer 尋找特定路徑裡之物件，將回傳該路徑下所有物件名稱</summary>
        /// <param name="path">欲取的物件名稱之路徑</param>
        /// <param name="obj">該路徑下之物件名稱集合</param>
        /// <returns></returns>
        public Stat FindObject(string path, out List<string> obj) {
            Stat stt = Stat.SUCCESS;
            List<string> objTemp = new List<string>();
            try {
                if ((path != "") && (mIServer != null)) {
                    foreach (var item in (mIServer.Root[path] as IAceObjectCollection).ToArray()) {
                        objTemp.Add(item.FullPath);
                    }
                }
            } catch (Exception ex) {
                if (stt == Stat.SUCCESS) stt = Stat.WN_ACE_ACCFAIL;
                ExceptionHandle(stt, ex);
            }
            obj = objTemp;
            return stt;
        }

        /// <summary>從已連線之 AceServer 尋找特定類型，且名稱具有特定關鍵字之物件</summary>
        /// <param name="keyWord">搜尋名稱之關鍵字</param>
        /// <param name="type">欲搜尋的類型。如 typeof(IAdeptController)</param>
        /// <param name="obj">物件路徑集合</param>
        /// <returns>Status Code</returns>
        public Stat FindObject(string keyWord, Type type, out List<string> obj) {
            Stat stt = Stat.SUCCESS;
            List<string> objTemp = new List<string>();
            try {
                if ((keyWord != "") && (mIServer != null)) {
                    /*-- 搜尋IAceServer裡的物件，並過濾出特定型態之物件，檢查是否含有關鍵字... --*/
                    foreach (var item in mIServer.Root.Filter(new ObjectTypeFilter(type), true)) {
                        if (item.FullPath.Contains(keyWord))
                            objTemp.Add(item.FullPath);
                    }
                }
            } catch (Exception ex) {
                if (stt == Stat.SUCCESS) stt = Stat.WN_ACE_ACCFAIL;
                ExceptionHandle(stt, ex);
            }
            obj = objTemp;
            return stt;
        }

        /// <summary>刪除 AceServer 裡之特定路徑物件</summary>
        /// <param name="path">欲刪除之物件路徑</param>
        /// <returns>Status Code</returns>
        public Stat DeleteObject(string path) {
            Stat stt = Stat.SUCCESS;
            try {
                if ((path != "") && (mIServer != null)) {
                    IAceObject obj = FindObject(path) as IAceObject;
                    if (obj != null) {
                        mIServer.Root.Remove(obj);
                        obj.Dispose();
                    }
                } else stt = Stat.ER_SYS_NOFILE;
            } catch (Exception ex) {
                if (stt == Stat.SUCCESS) stt = Stat.WN_ACE_ACCFAIL;
                ExceptionHandle(stt, ex);
            }
            return stt;
        }
        #endregion

        #region Function - Threads

        /// <summary>載入Workspace檔案之執行緒，等待載入完成隨後關閉</summary>
        /// <param name="path">檔案路徑</param>
        private void tsk_LoadWorkspace(object path) {
            try {
                mIServer.Clear();
                mIServer.LoadLocalWorkspace(CtConvert.CStr(path), EmulationMode);
            } catch (Exception ex) {
                ExceptionHandle(Stat.ER_SYSTEM, ex);
            }
        }

        #endregion

        #region Function - Core

        /// <summary>加入相關元件之事件
        /// <para>(True) 不初始化Plugin，適用於VS開啟時可避開當掉問題</para>
        /// <para>(False) 初始化Plugin，後續可方便建立Pendant或Vision等相關視窗，但須脫離VS方可成功初始化</para>
        /// </summary>
        /// <returns>Status Code</returns>
        private Stat AddEventHandler() {
            Stat stt = Stat.SUCCESS;
            try {

                /*-- 目前尚未發現有任何拋出此事件的，暫時不加 --*/
                //mAppHdl = new RemoteApplicationEventHandler(mICtrl);
                //mAppHdl.ApplicationEventReceived += new EventHandler<NumericChangeEventArgs>(rx_ApplicationEventReceived);

                if (!mSmartCtrl) mObjHdl = new RemoteAceObjectEventHandler(mIClient, mAceObj.Robots[0] as iCobra);
                else mObjHdl = new RemoteAceObjectEventHandler(mIClient, mICtrl);

                mObjHdl.ObjectPropertyModified += new EventHandler<PropertyModifiedEventArgs>(rx_ObjectPropertyModified);
                mObjHdl.ObjectDisposing += new EventHandler<EventArgs>(rx_ObjectDisposing);

                /*-- 添加IAceClient相關事件，將會發在CtLib --*/
                /*-- 因為如用EventHandler會需要使用Static修飾詞，但許多物件並非Static，故改採直接用delegate --*/
                /*-- PS. IAceServer事件會發在Adept ACE介面中 --*/
                /*-- 2014/09/08 取消 Inherit AceObject 後可以使用獨立的Function了 --*/
                mIClient.WorkspaceLoad += new EventHandler<EventArgs>(mIClient_WorkspaceLoad);
                mIClient.WorkspaceSaved += new EventHandler<WorkspaceSaveEventArgs>(mIClient_WorkspaceSaved);
                mIClient.WorkspaceUnload += new EventHandler<EventArgs>(mIClient_WorkspaceUnload);

                /*-- 初始化GUI --*/
                mIGuiPlug = mIClient.ClientPropertyManager[typeof(IAdeptGuiPlugin).Name] as IAdeptGuiPlugin;

                Thread.Sleep(500);

            } catch (Exception ex) {
                stt = Stat.ER_SYSTEM;
                ExceptionHandle(stt, ex);
            }
            return stt;
        }

        /// <summary>移除相關元件之事件</summary>
        private void RemoveEventHandler() {
            try {
                if (mObjHdl != null) {
                    mObjHdl.ObjectPropertyModified -= new EventHandler<PropertyModifiedEventArgs>(rx_ObjectPropertyModified);
                    mObjHdl.ObjectDisposing -= new EventHandler<EventArgs>(rx_ObjectDisposing);
                }

                if (mIClient != null) {
                    mIClient.WorkspaceLoad -= new EventHandler<EventArgs>(mIClient_WorkspaceLoad);
                    mIClient.WorkspaceSaved -= new EventHandler<WorkspaceSaveEventArgs>(mIClient_WorkspaceSaved);
                    mIClient.WorkspaceUnload -= new EventHandler<EventArgs>(mIClient_WorkspaceUnload);
                }

                mTask.OnTaskChanged -= mTask_OnTaskChanged;
            } catch (Exception ex) {
                ExceptionHandle(Stat.ER_SYSTEM, ex);
            }
        }

        /// <summary>關閉與 Adept ACE 之連線並釋放資源</summary>
        public void Dispose() {
            try {
                Dispose(true);
                GC.SuppressFinalize(this);
            } catch (ObjectDisposedException ex) {
                ExceptionHandle(Stat.ER_SYSTEM, ex);
            }
        }

        /// <summary>關閉與 Adept ACE 之連線並釋放資源</summary>
        /// <param name="isDisposing">是否第一次釋放</param>
        protected virtual void Dispose(bool isDisposing) {
            try {
                if (isDisposing) {
                    /*-- 移除相關事件 --*/
                    RemoveEventHandler();
                    /*-- Disconnect --*/
                    Disconnect();
                }

                if (!ClientMode)
                    CtApplication.KillProcess("Ace");

            } catch (Exception ex) {
                ExceptionHandle(Stat.ER_SYSTEM, ex);
            }
        }

        /// <summary>中斷與Adept ACE Server之連結，並將相關元件清除為null</summary>
        /// <returns>Status Code</returns>
        public Stat Disconnect() {
            Stat stt = Stat.SUCCESS;
            try {
                /*-- IAceClient --*/
                if (mIClient != null) {
                    mIClient.DisposePlugins();
                    mIClient.Dispose();
                    mIClient = null;
                }

                /*-- IAceServer --*/
                if (mIServer != null)
                    mIServer = null;

                /*-- IAdeptController --*/
                if (mICtrl != null)
                    mICtrl = null;

                /*-- IVpLink --*/
                if (mVpLink != null)
                    mVpLink = null;

                /*-- ObjectEventHandler of IAdeptController --*/
                if (mObjHdl != null) {
                    mObjHdl.Dispose();
                    mObjHdl = null;
                }

                /*-- TaskEventHandler --*/
                if (mTskHdl != null) {
                    mTskHdl.Dispose();
                    mTskHdl = null;
                }

                /*-- 更改Flag表示已經Dispose --*/
                mAceDispose = true;

                /*-- 發布Event --*/
                OnBoolEventOccur(new BoolEventArgs(BoolEvents.CONNECTION, false));
            } catch (Exception ex) {
                stt = Stat.ER_SYSTEM;
                ExceptionHandle(stt, ex);
            }
            return stt;
        }

        /// <summary>建立所有元件之連線，並尋找控制器與手臂</summary>
        /// <param name="ctrl">是否含有SmartController。此將影響連線與建立物件方式</param>
        /// <param name="emulate">[Server Only] 是否使用 Emulation 模式</param>
        /// <returns>Status Code</returns>
        /// <remarks>如果有遇到系統於 AceClient (@825) 卡住，請於 App.config 檢查是否有加入 useLegacyV2RuntimeActivationPolicy="true"</remarks>
        public Stat Connect(ControllerType ctrl, bool emulate = false) {
            Stat stt = Stat.SUCCESS;

            EmulationMode = emulate;

            /*-- 如果是 ClientMode，檢查是否有開啟 Ace.exe 了 --*/
            if (mClientMode) {
                bool procExist;
                CtApplication.IsProcessExist("Ace", out procExist);
                if (!procExist) throw (new Exception("選擇以 Client 方式連線，但尚未開啟 Adept ACE。"));
            }

            /*-- 是否有SmartController --*/
            switch (ctrl) {
                case ControllerType.WITH_SMARTCONTROLLER:
                    mSmartCtrl = true;
                    break;
                case ControllerType.WITHOUT_SMARTCONTROLLER:
                    mSmartCtrl = false;
                    break;
            }

            /*-- 如果是重新連線，把原有東西給Dispose掉 --*/
            if ((mIServer != null) || (mIClient != null))
                Disconnect();

            /*-- 建立IAceServer與IAceClient供後續使用 --*/
            Connect(out mIServer, out mIClient, mClientMode);
            if (mIServer == null || mIClient == null) {
                stt = Stat.ER3_ACE_CONT;
                Dispose();
                throw (new Exception("IAceServer與IAceClient建立失敗"));
            }

            /*-- 載入Workspace --*/
            if (!mClientMode) {
                mIServer.EmulationMode = EmulationMode;
                stt = LoadWorkspace(WorkspacePath);
                if (stt != Stat.SUCCESS) {
                    stt = Stat.ER3_ACE_CONT;
                    throw (new Exception("IAceServer與IAceClient建立失敗"));
                } else Thread.Sleep(350);  /* Load Workspace 之後，需要 Delay 一下，試過最少要 2.5秒，但有時候 3 秒也會掛掉，視情況再增減吧!! */
            }

            /*-- 分析IAceServer，將各元件拉至mAceObj，同時可以抓Controller/Robot/Variable等等 --*/
            stt = LinkAceObj(mIServer);
            if (stt != Stat.SUCCESS) {
                stt = Stat.ER3_ACE_CONT;
                throw (new Exception("AceServer物件分析失敗"));
            }

            /*-- 如果正確連結至SmartController，建立其IVpLink --*/
            if (!mSmartCtrl) {
                mVpLink = (mAceObj.Robots[0] as iCobra).Link;
                mMaxTaskNum = (mAceObj.Robots[0] as iCobra).MaxTaskNumber;
            } else {
                if (mAceObj.Controllers.Count > 0) {
                    mICtrl = mAceObj.Controllers[0];
                    mVpLink = mICtrl.Link;

                    mMaxTaskNum = mICtrl.MaxTaskNumber - mVpLink.Status().ToList().FindAll(data => data.MainProgram == ("sv.q_mgr") || data.MainProgram == ("a.ace_srvr")).Count;

                } else {
                    stt = Stat.ER3_ACE_CONT;
                    throw (new Exception("無法正確建立與SmartController之連結"));
                }
            }

            /*-- 建立相關Event --*/
            stt = AddEventHandler();
            if (stt != Stat.SUCCESS) {
                stt = Stat.ER3_ACE_CONT;
                throw (new Exception("ACE相關事件無法正確加入"));
            }

            /*-- 連結子模組 --*/
            mIO = new CtAceIO(mSmartCtrl, (mSmartCtrl) ? mICtrl : mAceObj.Robots[0]);
            mVariables = new CtAceVariable(mVpLink, mICtrl, mIServer);
            mTask = new CtAceTask(mVpLink, mMaxTaskNum);
            mTask.OnTaskChanged += mTask_OnTaskChanged;
            mMotion = new CtAceMotion(mVpLink, mIServer, (mSmartCtrl) ? mICtrl : mAceObj.Robots[0]);
            mVision = new CtAceVision(mIClient, mIServer);

            /*-- 發布Event --*/
            OnBoolEventOccur(new BoolEventArgs(BoolEvents.CONNECTION, true));

            return stt;
        }

        void mTask_OnTaskChanged(object sender, CtAceTask.TaskEventArgs e) {
            OnTaskEventOccur(e);
        }

        /// <summary>設定 HighPower 狀態。如欲送電，則會出現介面並等待按鈕按下</summary>
        /// <param name="power">(True)送電  (False)關閉電源</param>
        /// <param name="prog">是否顯示進度條？</param>
        /// <returns>Status Code</returns>
        /// <remarks>因 Johnson 於 2015/02/26 提出某些電腦在開關電時跳出的 CtProgress 造成莫名 Exception 導致介面崩潰，加入 prog 方便使用</remarks>
        public Stat SetPower(bool power, bool prog = true) {
            Stat stt = Stat.SUCCESS;
            CtProgress frmProg = null;

            try {

                if (prog) {
                    if (power) {
                        frmProg = new CtProgress(
                            CtProgress.Style.COUNTDOWN,
                            "HighPower",
                            "請於10秒內按下面板電源鈕 / Press \"HighPower\" button at front panel in 10 sec.",
                            10F
                        );
                    } else {
                        frmProg = new CtProgress(
                            "HighPower",
                            "請等待電源解除 / Waiting disable power ..."
                        );
                    }
                }

                if (!mSmartCtrl) {
                    if (mAceObj.Robots.Count > 0) {
                        (mAceObj.Robots[0] as iCobra).Calibrate();
                        (mAceObj.Robots[0] as iCobra).HighPower = power;
                    } else
                        stt = Stat.ER4_SYS_ACECNT;
                } else {
                    if (mICtrl != null) {
                        mICtrl.Calibrate();
                        mICtrl.HighPower = power;
                    } else
                        stt = Stat.ER4_SYS_ACECNT;
                }
            } catch (AceException aceEx) {
                if (stt == Stat.SUCCESS) stt = Stat.WN_ACE_PWTMOUT;
                ExceptionHandle(stt, aceEx);
            } catch (Exception ex) {
                if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
                ExceptionHandle(stt, ex);
            } finally {
                if (frmProg != null) frmProg.Close();
            }
            return stt;
        }

        /// <summary>變更整體速度(MonitorSpeed)</summary>
        /// <param name="speed">欲變更之速度</param>
        /// <returns>Status Code</returns>
        public Stat SetSpeed(int speed) {
            Stat stt = Stat.SUCCESS;
            try {
                if (speed < ACE_MINSPEED)
                    speed = 0;
                if (speed > ACE_MAXSPEED)
                    speed = 100;

                if (!mSmartCtrl) {
                    if (mAceObj.Robots.Count > 0)
                        (mAceObj.Robots[0] as iCobra).MonitorSpeed = speed;
                    else
                        stt = Stat.ER4_SYS_ACECNT;
                } else {
                    if (mICtrl != null)
                        mICtrl.MonitorSpeed = speed;
                    else
                        stt = Stat.ER4_SYS_ACECNT;
                }
            } catch (Exception ex) {
                if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
                ExceptionHandle(stt, ex);
            }
            return stt;
        }

        /// <summary>Jog Pendant</summary>
        /// <param name="ctStyle">是否使用 CASTEC Style Pendant? (True)CASTEC Style  (False)Adept Origin</param>
        /// <returns>Status Code</returns>
        public Stat Pendant(bool ctStyle = false) {
            Stat stt = Stat.SUCCESS;
            try {
                if (ctStyle) {
                    CtAcePendant pendant = new CtAcePendant(this);
                    pendant.ShowDialog();
                    pendant.Dispose();
                } else {
                    ControlPanelManager panelMag = new ControlPanelManager();
                    panelMag.LaunchControlForm(null, mIClient, (mSmartCtrl ? mICtrl.GetRobot(1) : mAceObj.Robots[0] as IAdeptRobot), true);
                    //do {
                    //    Thread.Sleep(10);
                    //    Application.DoEvents();
                    //} while (panelMag.GetControlForm() != null);
                }

            } catch (Exception ex) {
                if (stt == Stat.SUCCESS) stt = Stat.ER_SYSTEM;
                ExceptionHandle(stt, ex);
            }
            return stt;
        }

        /// <summary>取得手臂所含有的 Joint 數量</summary>
        /// <param name="robotNum">欲取得的手臂編號</param>
        /// <returns>Joint 數量</returns>
        public int GetJointCount(int robotNum) {
            int jointCount = -1;
            if (mAceObj.Robots.Count >= robotNum)
                jointCount = (mAceObj.Robots[robotNum - 1] as IAdeptRobot).JointCount;
            return jointCount;
        }
        #endregion

        #region Function - Events

        #region Workspace Events
        private void mIClient_WorkspaceLoad(object sender, EventArgs e) {
            OnNotifyEventOccur(new NotifyEventArgs(NotifyEvents.WORKSPACE_LOAD));
        }

        private void mIClient_WorkspaceSaved(object sender, WorkspaceSaveEventArgs e) {
            OnNotifyEventOccur(new NotifyEventArgs(NotifyEvents.WORKSPACE_SAVE));
        }

        private void mIClient_WorkspaceUnload(object sender, EventArgs e) {
            OnNotifyEventOccur(new NotifyEventArgs(NotifyEvents.WORKSPACE_UNLOAD));
        }
        #endregion

        #region Application Events
        void rx_ApplicationEventReceived(object sender, NumericChangeEventArgs e) {
            CtMsgBox.Show("Application Event", e.Value.ToString());
        }
        #endregion

        #region ObjectProperty Events

        void rx_ObjectDisposing(object sender, EventArgs e) {
            if (!mAceDispose) {
                mAceDispose = true;
                OnNotifyEventOccur(new NotifyEventArgs(NotifyEvents.ACE_SHUTDOWN));
            }
        }

        void rx_ObjectPropertyModified(object sender, PropertyModifiedEventArgs e) {
            try {
                switch (e.PropertyName) {
                    case "HighPower":
                        OnBoolEventOccur(new BoolEventArgs(BoolEvents.POWER_CHANGED, mICtrl.HighPower));
                        break;

                    case "MonitorSpeed":
                        OnNumericEventOccur(new NumericEventArgs(NumericEvents.SPEED_CHANGED, mICtrl.MonitorSpeed));
                        break;

                    case "Line":
                        OnNotifyEventOccur(new NotifyEventArgs(NotifyEvents.PROGRAM_MODIFIED));
                        break;
                }
            } catch (Exception ex) {
                ExceptionHandle(Stat.ER_SYSTEM, ex);
            }
        }
        #endregion

        #region Task Events
        void rx_TaskStateChanged(object sender, TaskStateChangeEventArgs e) {
            try {
                object[] objTask = new object[] { e.Program.TaskName, (TaskState)e.State };
                OnNumericEventOccur(new NumericEventArgs(NumericEvents.TASK_STATE_CHANGED, objTask));

            } catch (Exception ex) {
                ExceptionHandle(Stat.ER_SYSTEM, ex);
            }
        }
        #endregion

        #endregion

    }
}
