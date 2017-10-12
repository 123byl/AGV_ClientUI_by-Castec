using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Forms;
using CtLib.Module.Net;
using CtLib.Module.Utility;

using Ace.Adept.Client;
using Ace.Adept.Server.Controls;
using Ace.Adept.Server.Desktop.Connection;
using Ace.Adept.Server.Motion;
using Ace.Adept.Server.Motion.Robots;
using Ace.Core.Client;
using Ace.Core.Server;
using Ace.Core.Server.Event;
using Ace.Core.Server.Motion;
using Ace.Core.Server.Program;
using Ace.Core.Util;
namespace CtLib.Module.Adept {

	/// <summary>
	/// Adept ACE 相關應用
	/// <para>Client 模式為以已開啟之 Adept ACE 為主要連線對象，所有項目以當前載入的 Workspace 為準</para>
	/// <para>Server 模式以 AceServer.exe 為主，需指定 Workspacce 檔案並於開啟後載入</para>
	/// <para>目前採用模組化方式，Variable/IO/Motion/Vision 請依各類別去操作</para>
	/// </summary>
	/// <example>
	/// 1. Client Mode
	/// <code language="C#">
	/// CtAce mAce = new CtAce(false);  //Don't raise message event
	/// AddEventHandler();              //Add CtAce events, like OnBoolEventChanged, OnNumericEventChanged and so on
	/// Stat stt = mAce.Connect(ControllerType.SmartController); //Connect with SmartController, you should set properties as you need
	/// </code>
	/// 2. Workspace Mode
	/// <code language="C#">
	/// CtAce mAce = new CtAce(false);  //Don't raise message event
	/// AddEventHandler();              //Add CtAce events, like OnBoolEventChanged, OnNumericEventChanged and so on
	/// mAce.ClientMode = false;        //Select to Server Mode
	/// mAce.WorkspacePath = @"D:\CASTEC\Project\Workspace\Demo.awp";       //Set the workspace path
	/// Stat stt = mAce.Connect(ControllerType.SmartController); //Connect with SmartController, you should set properties as you need
	/// </code>
	/// 3. Simply Operations (If you need more detail operations, please see each chapter)
	/// <code language="C#">
	/// /*-- Here is a REAL V+ variable reading/getting, you can change it as you need --*/
	/// float var1;
	/// mAce.Variable.GetValue("var1", out var1);   //Get "var1" real value as a float
	/// mAce.Variable.SetValue("var2", 3);          //Set "var2" value to 3
	/// 
	/// /*-- If target is NumericVariabe ot StringVariable (Not V+ object) --*/
	/// float num1;
	/// mAce.Variable.GetValue(@"/Vairable/Numeric1", out num1, VariableType.Numeric);   //Get "Numeric1" value
	/// mAce.Variable.SetValue(@"/Vairable/Numeric2", 9, VariableType.Numeric);          //Set "Numeric2" value
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
	/// VisionToolType toolType = VisionToolType.CustomVisionTool;
	/// mAce.Vision.ExecuteVisionTool(visPath, toolType, true, out score);  //Execute CVT (take new picture) and get the highest Matched Qulity Score 
	/// 
	/// /*-- Motion --*/
	/// mACE.Motion.MoveDistance(1, Axis.Roll, 90F);  //Let Robot1's theta (Joint4) do rotation with 90 degrees.
	/// </code>
	/// </example>
	[Serializable]
	public class CtAce : IDisposable, ICtVersion {

		#region Version

		/// <summary>CtAce 版本訊息</summary>
		/// <remarks><code language="C#">
		/// 0.0.0	William	[2012/07/03]
		///     + CtAceServer
		/// 
		/// 1.0.0	Ahern	[2014/08/31]
		///     + 序列化，現可使用ACE本身的Event
		///     + 使用EventHandler建立本身事件，可讓後續介面得知相關事件
		///     \ 使用IAceServer.Root.Filter方式取代原有遞迴尋找內部SmartController/Robot方式
		///     + 完成常用Function
		///      
		/// 1.0.1	Ahern	[2014/09/09]
		///     - 序列化。發現僅將CtLib.exe搬移至Ace\Bin資料夾即可，不需施作序列化
		///     + Vision相關應用，但目前無法正確獲取影像 (連AceDemo也不行)
		///     + IDisposable，在Release時能先解除相關事件並釋放資源
		///      
		/// 1.0.2	Ahern	[2014/09/10]
		///     + SaveWorkspace方法
		///      
		/// 1.0.3	Ahern	[2014/09/12]
		///     \ 修改相關事件名稱
		///     + OnMessage事件，以利將訊息拋出去
		///     + Connection事件
		///      
		/// 1.0.4	Ahern	[2014/09/19]
		///     + RaiseVisionWindow
		///     + AddVisionHandle
		///     + RemoveVisionHadle
		///      
		/// 1.0.5	Ahern	[2014/09/22]
		///     \ 開啟電源之倒數視窗，改以CtProgress取代CtProgress_Ctrl
		///      
		/// 1.1.0	Ahern	[2014/10/19]
		///     + ExecuteVisionTool
		///      
		/// 1.1.1	Ahern	[2014/10/28]
		///     \ 統一Enumeration大小寫
		///      
		/// 1.1.2	Ahern	[2014/11/01]
		///     \ ExecuteVisionTool之Execute改用False
		///      
		/// 1.1.3	Ahern	[2014/11/03]
		///     + AddLocatorModel
		///     + RemoveLocatorModel
		///     + CreateLocatorModel
		///      
		/// 1.1.4	Ahern	[2014/11/04]
		///     \ CreateLocatorModel
		///     \ 原GetAceObject重新命名為FindObject，並新增兩組Overload (回傳IAceObjectCollection物件與路徑)
		///     + DeleteObject，用於刪除物件
		///     + GetLocatorModelNames，用於取得特定KeyWord之Model路徑
		///     + GetCurrentModelNames，用於取得Locator內目前的Model路徑
		///     \ ExecuteVisionTool之Execute改回True，不知為何有時無法取得新影像，切回true即可
		///      
		/// 1.1.5	Ahern	[2014/11/05]
		///     + ExecuteVisionTool增加回傳 Match Quality (Locator) 或是 Area (Blob)
		///      
		/// 1.1.6	Ahern	[2014/11/06]
		///     + EmulationCameraImageAdd
		///     + EmulationCameraImageRemove
		///      
		/// 1.1.7	Ahern	[2014/11/07]
		///     \ 原 VariableType 更改為 CtAceVariable.VPlusVariableType
		///     + VariableType，區分為三種:V+/Numeric/String Variable
		///      
		/// 1.1.8	Ahern	[2014/11/08]
		///     \ 修正部分註解
		///      
		/// 1.2.0	Ahern	[2014/11/26]
		///     \ 將 I/O 獨立至 CtAceIO
		///     \ 將 Variable 獨立至 CtAceVariable
		///     \ 將 Task 獨立至 CtAceTask
		///      
		/// 1.2.1	Ahern	[2014/12/17]
		///     \ ExceptionHandle 改採 StackTrace 模式
		///      
		/// 1.2.2	Ahern	[2015/02/06]
		///     \ SetPower 修改 CtProgress 訊息與新增引數用於不要顯示進度條
		///      
		/// 1.2.3	Ahern	[2015/03/01]
		///     - Try-Catch
		///      
		/// 1.2.4	Ahern	[2015/05/12]
		///     \ Motion 加入 InRange 判斷
		///      
		/// 1.3.0	Ahern	[2015/05/22]
		///     \ Server 模式，LoadWorkspace 後需要 Delay，否則會「无法进入通信渠道」
		///     + EmulationMode
		///     X 嘗試於 LoadLocalWorkspace 加入 Progress 事件，但會跳出非序列化
		///     
		/// 1.3.1	Ahern	[2015/05/27]
		///     \ Task 部分增加保護確認
		/// 
		/// 1.4.0	Ahern	[2015/06/01]
		///     + CtAcePendant
		///     
		/// 1.4.1	Ahern	[2016/01/14]
		///		+ GetCurrentPosture
		///		+ Get/Set RobotDigitalIO
		///		
		/// 1.5.0	Ahern	[2016/03/21]
		///		\ 將 Event 獨立
		///		
		/// 1.5.1	Ahern	[2016/03/28]
		///		+ Smart Controller / iCobra 連線狀態
		///		
		/// 1.5.2	Ahern	[2016/06/28]
		///		+ Task.IsAnyTaskExecuting
		///		
		/// 1.6.0	Ahern	[2016/07/17]
		///		\ 改以 VpObjects 取代原本 mICtrl、miCobra，並全部採以 Lock
		///		\ 部分方法可帶入 VpLinkObject 索引，先預留未來可能有多控制器情況
		///		+ IAceServer 相關事件及 PipeStream 交握，但目前無法取消訂閱，先預留
		///		
		/// 1.6.1	Ahern	[2016/07/19]
		///		- 移除大部分的 try-catch，讓外面包就好
		///		\ WhereAce 補上搜尋 x86 資料夾之部分
		///		
		/// 1.6.2	Ahern	[2016/09/26]
		///		\ Motion 取得座標改以 VpLinkedObject 為準
		///		
		/// 1.6.3	Ahern	[2016/10/18]
		///		+ Motion.Detach
		///		
		/// 1.6.4	Ahern	[2016/12/07]
		///		+ IsAceConnected
		///		
		/// 1.7.0	Ahern	[2016/12/14]
		///		+ CtAceScript，提供 C# 與其他相關腳本控制
		///		
		/// 1.7.1	Ahern	[2017/02/10]
		///		\ 於讀取 Task 數量前添加判斷 Controller 是否有連線避免 Exception
		///     
		/// </code></remarks>
		public CtVersion Version { get { return new CtVersion(1, 7, 1, "2017/02/10", "Ahern Kuo"); } }

		#endregion

		#region Declaration - Definitions

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

		/// <summary><see cref="CtAsyncPipe"/> 之管道名稱</summary>
		private static readonly string PIPE_NAME = "CtAce";
		#endregion

		#region Declaration - Private  Enumerations
		private enum ServerEvents {
			AwpSaved = 0,
			AwpShutdown = 1,
			AwpUnload = 2,
			Calibration = 3,
			Contents = 4,
			EStop = 5,
			MonitorSpeed = 6,
			Power = 7,
			AwpLoad = 8,
			Progress = 9
		}
		#endregion

		#region Declaration - Properties

		/// <summary>取得 ACE Server 是否為模擬環境</summary>
		public bool EmulationMode { get; private set; }

		/// <summary>取得或設定與ACE連線模式 (<see langword="true"/>)Client  (<see langword="false"/>)Server</summary>
		public bool ClientMode {
			get { return mClientMode; }
			set { mClientMode = value; }
		}

		/// <summary>取得或設定遠端名稱，用於與 AceServer 連線</summary>
		public string RemotingName {
			get { return mACE_Name; }
			set { mACE_Name = value; }
		}

		/// <summary>取得或設定 AceServer 之IP Address</summary>
		public string ServerIP {
			get { return mACE_IP; }
			set { mACE_IP = value; }
		}

		/// <summary>取得或設定 AceServer 之連線埠</summary>
		public int ServerPort {
			get { return mACE_Port; }
			set { mACE_Port = value; }
		}

		/// <summary>取得或設定Workspace路徑，如 @"D:\Workspace\E1404_1-1-4.awp"</summary>
		public string WorkspacePath { get; set; } = @"D:\Workspace\CASTEC.awp";

		/// <summary>取得或設定訊息發報至Event。如開啟將可透過OnMessage事件取得Exception或是相關訊息</summary>
		[DefaultValue(true)]
		public bool EnableMessage { get; set; }

		/// <summary>取得當前所有的與 V+ 連結之物件路徑，可能為 <see cref="IAdeptController"/> 或 <seealso cref="iCobra"/></summary>
		public List<string> VpLinks { get { return mVpObj?.VpLinkers.Select(vpLink => vpLink.Value.FullPath).ToList(); } }
       
		/// <summary>取得當前所有的 <see cref="IAdeptRobot"/> 完整路徑</summary>
		public List<string> Robots { get { return mRobots?.Select(rob => rob.Value.FullPath).ToList(); } }

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

		/// <summary>腳本、程序相關控制模組</summary>
		public CtAceScript Script { get { return mScript; } }

		/// <summary>取得當前 <see cref="CtAce"/> 是否已連接上 Omron|Adept ACE，不論 Client 或 Server</summary>
		public bool IsAceConnected { get; private set; } = false;

        /// <summary>是否含有SmartController</summary>	
        public bool IsSmartControl { get { return mSmartCtrl; } }
        #endregion

		#region Declaration - Fields
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
		/// <summary>V+ 連結器集合，並提供 lock 服務</summary>
		private VpObjects mVpObj;
		/// <summary>Adept 含有智慧控制器之機器手臂集合，如 sCobra 800、Quattro 600 等</summary>
		private Dictionary<int, IRobot> mRobots = new Dictionary<int, IRobot>();
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

		/*-- Communications --*/
		///// <summary>接收從 Adept ACE Server 端所發送的事件通知</summary>
		//private CtAsyncPipe mPipSrv;

		/*-- Flags --*/
		/// <summary>[Flag] 用於確認ACE是否開啟，連線後改為False，如遇到第一次IAdeptController.Dispose則改為True</summary>
		/// <remarks>由於Dispose事件會發好幾次，故利用此Flag去卡住，不要連續發Event出去</remarks>
		private bool mAceDispose = true;

		/*-- Progres events --*/
		private bool mProgExit = false;
		private bool mProgChange = false;
		private int mProgPercent = 0;
		private string mProgMsg = string.Empty;

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
		/// <summary>[Module] Script Controls</summary>
		private CtAceScript mScript;
        ///<summary>[Module]Robot Monitor</summary>
        //private CtRobotMonitor mRobotMonitor = null;

		/*-- Lock Objects --*/
		private object mLockVpLink = int.MinValue;
		#endregion

		#region Declaration - Events
		/// <summary>發生Boolean值改變事件</summary>
		public event EventHandler<AceBoolEventArgs> OnBoolEventChanged;
		/// <summary>發生特定數值改變事件</summary>
		public event EventHandler<AceNumericEventArgs> OnNumericEventChanged;
		/// <summary>發生需通知外部之事件</summary>
		public event EventHandler<AceNotifyEventArgs> OnNotifyEventChanged;
		/// <summary>發生Task通知事件</summary>
		public event EventHandler<CtAceTask.TaskEventArgs> OnTaskChanged;
		/// <summary>發生顯示訊息事件</summary>
		public event EventHandler<AceMessageEventArgs> OnMessage;

		/// <summary>觸發Boolean改變事件</summary>
		/// <param name="ev">事件</param>
		/// <param name="value">數值</param>
		protected virtual void OnBoolEventOccur(AceBoolEvents ev, bool value) {
			EventHandler<AceBoolEventArgs> handler = OnBoolEventChanged;
			if (handler != null) handler.BeginInvoke(this, new AceBoolEventArgs(ev, value), null, null);
		}

		/// <summary>觸發數值改變事件</summary>
		/// <param name="ev">事件</param>
		/// <param name="value">數值</param>
		protected virtual void OnNumericEventOccur(AceNumericEvents ev, object value) {
			EventHandler<AceNumericEventArgs> handler = OnNumericEventChanged;
			if (handler != null) handler.BeginInvoke(this, new AceNumericEventArgs(ev, value), null, null);
		}

		/// <summary>觸發通知事件</summary>
		/// <param name="ev">事件</param>
		protected virtual void OnNotifyEventOccur(AceNotifyEvents ev) {
			EventHandler<AceNotifyEventArgs> handler = OnNotifyEventChanged;
			if (handler != null) handler.BeginInvoke(this, new AceNotifyEventArgs(ev), null, null);
		}

		/// <summary>觸發Task狀態變更事件</summary>
		/// <param name="e">Task事件之參數</param>
		protected virtual void OnTaskEventOccur(CtAceTask.TaskEventArgs e) {
			EventHandler<CtAceTask.TaskEventArgs> handler = OnTaskChanged;
			if (handler != null) handler.BeginInvoke(this, e, null, null);
		}

		/// <summary>觸發UpdateMessage事件</summary>
		/// <param name="er">
		/// 訊息類型
		/// <para>-1: Alarm/Exception Message</para>
		/// <para>0: Normal Message</para>
		/// <para>1: Warning Message</para>
		/// </param>
		/// <param name="title">訊息標題</param>
		/// <param name="msg">訊息內容</param>
		protected virtual void OnMessageUpdate(sbyte er, string title, string msg) {
			EventHandler<AceMessageEventArgs> handler = OnMessage;
			if (handler != null) handler.BeginInvoke(this, new AceMessageEventArgs(er, title, msg), null, null);
		}

		#endregion

		#region Function - Constructor

		/// <summary>建立空白的CtAce元件</summary>
		public CtAce() {
			EnableMessage = true;
			//mPipSrv = new CtAsyncPipe(TransDataFormats.EnumerableOfByte);
			//mPipSrv.OnPipeEvents += rx_SrvPipeEvents;
			//mPipSrv.ServerListen(PIPE_NAME);
		}

		/// <summary>建立空白的CtAce元件</summary>
		/// <param name="enbMsg">是否透過OnMessage事件取得Exception或是相關訊息</param>
		public CtAce(bool enbMsg) {
			EnableMessage = enbMsg;

			//mPipSrv = new CtAsyncPipe(TransDataFormats.EnumerableOfByte);
			//mPipSrv.OnPipeEvents += rx_SrvPipeEvents;
			//mPipSrv.ServerListen(PIPE_NAME);
		}

		/// <summary>建立後進行所有元件之連線，並尋找控制器與手臂</summary>
		/// <param name="ctrl">是否含有SmartController。此將影響連線與建立物件方式</param>
		/// <param name="enbMsg">是否透過OnMessage事件取得Exception或是相關訊息</param>
		public CtAce(ControllerType ctrl, bool enbMsg = true) {
			EnableMessage = enbMsg;

			//mPipSrv = new CtAsyncPipe(TransDataFormats.EnumerableOfByte);
			//mPipSrv.OnPipeEvents += rx_SrvPipeEvents;
			//mPipSrv.ServerListen(PIPE_NAME);

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
				OnMessageUpdate(msgTyp, title, msg);
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
				OnMessageUpdate(msgTyp, method, ex.Message);
			}

		}

		/// <summary>印出 <see cref="IAceObject"/> 裡面的名稱以及型態</summary>
		/// <param name="item">欲掃描的 IAceObject</param>
		/// <param name="dumpTemp">要儲存的 List(Of string) 回傳格式為 "Name || Type"</param>
		private void DumpObject(IAceObject item, ref List<string> dumpTemp) {
			IAceObjectCollection fold = item as IAceObjectCollection;
			if (fold != null) {
				foreach (IAceObject subItem in fold.ToArray()) {
					DumpObject(subItem, ref dumpTemp);
				}
			} else {
				dumpTemp.Add(item.Name + " || " + item.GetType().ToString());
			}
		}

		/// <summary>加入相關元件之事件
		/// <para>(<see langword="true"/>) 不初始化Plugin，適用於VS開啟時可避開當掉問題</para>
		/// <para>(<see langword="false"/>) 初始化Plugin，後續可方便建立Pendant或Vision等相關視窗，但須脫離VS方可成功初始化</para>
		/// </summary>
		private void AddEventHandler() {
			mObjHdl = new RemoteAceObjectEventHandler(mIClient, mVpObj.Obj() as IAceObject);
			mObjHdl.ObjectPropertyModified += rx_ObjectPropertyModified;
			//mObjHdl.ObjectDisposing += rx_ObjectDisposing;	//由 mIClient.BeforeShutdown 取代

			mIClient.BeforeShutdown += AceClient_BeforeShutdown;
			mIClient.WorkspaceLoad += AceClient_WorkspaceLoad;
			mIClient.WorkspaceSaved += AceClient_WorkspaceSaved;
			mIClient.WorkspaceUnload += AceClient_WorkspaceUnload;
			//mIServer.Shutdown += AceSrv_Shutdown;
			//mIServer.WorkspaceLoad += AceSrv_WorkspaceLoad;
			//mIServer.WorkspaceSaved += AceSrv_WorkspaceSaved;
			//mIServer.WorkspaceUnload += AceSrv_WorkspaceUnload;
			//mIServer.Root.ContentsChanged += mIServerRoot_ContentsChanged;

			//IVpLinkedObject link = mVpObj.Obj();
			//link.CalibrationStateChanged += VpLinkObj_CalibrationStateChanged;
			//link.EStopStateChanged += VpLinkObj_EStopStateChanged;
			//link.MonitorSpeedChanged += VpLinkObj_MonitorSpeedChanged;
			//link.NodeStateChanged += VpLinkObj_NodeStateChanged;
			//link.PowerStateChanged += VpLinkObj_PowerStateChanged;

			/*-- 初始化GUI --*/
			mIGuiPlug = mIClient.ClientPropertyManager[typeof(IAdeptGuiPlugin).Name] as IAdeptGuiPlugin;

			CtTimer.Delay(500);
		}
        
        /// <summary>移除相關元件之事件</summary>
        private void RemoveEventHandler() {
			try {
				if (mObjHdl != null) {
					mObjHdl.ObjectPropertyModified -= rx_ObjectPropertyModified;
					mObjHdl.ObjectDisposing -= rx_ObjectDisposing;  //由 mIClient.BeforeShutdown 取代
				}

				if (mIClient != null) {
					mIClient.BeforeShutdown -= AceClient_BeforeShutdown;
					mIClient.WorkspaceLoad -= AceClient_WorkspaceLoad;
					mIClient.WorkspaceSaved -= AceClient_WorkspaceSaved;
					mIClient.WorkspaceUnload -= AceClient_WorkspaceUnload;
				}

				//if (mIServer != null) {
				//	mIServer.Shutdown -= AceSrv_Shutdown;
				//	mIServer.WorkspaceLoad -= AceSrv_WorkspaceLoad;
				//	mIServer.WorkspaceSaved -= AceSrv_WorkspaceSaved;
				//	mIServer.WorkspaceUnload -= AceSrv_WorkspaceUnload;
				//	mIServer.Root.ContentsChanged -= mIServerRoot_ContentsChanged;
				//}

				//if (mVpObj != null && mVpObj.ObjectCount > 0) {
				//	IVpLinkedObject link = mVpObj.Obj();
				//	link.CalibrationStateChanged -= VpLinkObj_CalibrationStateChanged;
				//	link.EStopStateChanged -= VpLinkObj_EStopStateChanged;
				//	link.MonitorSpeedChanged -= VpLinkObj_MonitorSpeedChanged;
				//	link.NodeStateChanged -= VpLinkObj_NodeStateChanged;
				//	link.PowerStateChanged -= VpLinkObj_PowerStateChanged;
				//}

				mTask.OnTaskChanged -= mTask_OnTaskChanged;
			} catch (Exception ex) {
				ExceptionHandle(Stat.ER_SYSTEM, ex);
			}
		}

		/// <summary>連線，並傳遞已建立之IAceServer/IAceClient元件</summary>
		/// <param name="iServer">回傳已建立之IAceServer元件</param>
		/// <param name="iClient">回傳已建立之IAceClient元件</param>
		/// <param name="client">是否為Client端模式 (<see langword="true"/>)Client  (<see langword="false"/>)Server</param>
		/// <remarks>如果有遇到系統於 AceClient (@825) 卡住，請於 App.config 檢查是否有 useLegacyV2RuntimeActivationPolicy="true"</remarks>
		private void Connect(out IAceServer iServer, out IAceClient iClient, bool client = true) {
			IAceServer aceServer = null;
			IAceClient aceClient = null;

			/*-- 定義Port --*/
			int intRemotingPort = (client) ? 0 : mACE_Port;
			int intServerPort = (client) ? RemotingUtil.DefaultRemotingPort : mACE_Port;

			/*-- 如果是Server模式，則關閉ACE並開啟Server --*/
			//if (!client) {
			//	try {
			//		CtApplication.KillService(ACE_ACESERVER_NAME);
			//	} catch (Exception) {
			//		/*-- 不確定 Ace.exe 會不會有跳 Service 出來，如果沒有會跳錯誤，所以用 try-catch 包住 --*/
			//	}
			//	CtTimer.Delay(PROGRAM_DELAY);
			//	CtApplication.ExecuteProcess(ACE_APPLICATION_PATH, "server");
			//	CtTimer.Delay(1000);
			//}

			/*-- 初始化ACE子系統 --*/
			RemotingUtil.InitializeRemotingSubsystem(true, intRemotingPort);

			/*-- 建立IAceServer與IAceClient --*/
			aceServer = (IAceServer)RemotingUtil.GetRemoteServerObject(typeof(IAceServer), mACE_Name, mACE_IP, intServerPort);

			/*-- 建立 IAceClient，如果此處會卡很久，請檢查 App.config 是否有加入 useLegacyV2RuntimeActivationPolicy="true" --*/
			aceClient = new AceClient(aceServer);

			/*-- 初始化所有Plugins，方便後續Jog Pendant等方便建立 --*/
			aceClient.InitializePlugins(null);

			/*-- 如果均成功建立，將Dispose的Flag改為False表示ACE活著 --*/
			if ((aceServer != null) && (aceClient != null)) {
				mAceDispose = false;
				IsAceConnected = true;
			}

			CtTimer.Delay(500);

			iServer = aceServer;
			iClient = aceClient;
		}

		/// <summary>取得並分析 <see cref="IAceServer"/> 內部物件，如有分析到 <see cref="IAdeptController"/> 或是 <seealso cref="IAdeptRobot"/> 則加入集合</summary>
		/// <param name="aceServer">Adept ACE Server 物件</param>
		private void LinkAceObj(IAceServer aceServer) {
			/*-- 搜尋 IVpLinkedObject，可能為 SmartController 或 iCobra --*/
			IEnumerable<IVpLinkedObject> linkColl = aceServer.Root.FilterType(typeof(IVpLinkedObject), true).Cast<IVpLinkedObject>();
			if (linkColl != null) {
				mVpObj = new VpObjects(linkColl);
			}

            /*- 若是有呼叫過Disconnect方法則mRobots為null，在這裡重新實例化 -*/
            if (mRobots == null) mRobots = new Dictionary<int, IRobot>();
			
            /*-- 搜尋 Robot --*/
			if (mSmartCtrl) {
                IAdeptRobot robott = (IAdeptRobot)aceServer.Root.FilterType(typeof(IAdeptRobot), true)[0];

                foreach (IAdeptRobot robot in aceServer.Root.FilterType(typeof(IAdeptRobot), true)) {
					mRobots.Add(robot.RobotNumber, robot);
				}
			} else {
				int idx = 1;
				foreach (IRobot robot in aceServer.Root.FilterType(typeof(IRobot), true)) {
					mRobots.Add(idx, robot);
					idx++;
				}
			}
		}

		/// <summary>取得屬於此 <see cref="CtAce"/> 的 <see cref="IAceClient"/>。此方法並無確保 <see cref="IAceClient"/> 已連上並建立</summary>
		/// <returns>當前的 <see cref="IAceClient"/></returns>
		public IAceClient GetClient() {
			return mIClient;
		}

		/// <summary>取得屬於此 <see cref="CtAce"/> 的 <see cref="IAceServer"/>。此方法並無確保 <see cref="IAceServer"/> 已連上並建立</summary>
		/// <returns>當前的 <see cref="IAceServer"/></returns>
		public IAceServer GetServer() {
			return mIServer;
		}

        ///<summary>取得VpLine物件</summary>
        internal VpObjects GetVpLink() {
            return mVpObj;
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
		public void SaveWorkspace() {
			lock (mIClient) {
				bool bolResult = mIClient.SaveWorkspace(null);
				if (!bolResult)
					throw new Exception("Saving workspace failed");
			}
		}

		/// <summary>
		/// Workspace另存新檔
		/// <para>此方法將跳出路徑視窗，並讓使用者選擇儲存路徑與檔名</para>
		/// </summary>
		public void SaveWorkspaceAs() {
			lock (mIClient) {
				bool bolResult = mIClient.SaveWorkspaceAs(null);
				if (!bolResult)
					throw new Exception("Saving workspace failed");
			}
		}

		/// <summary>
		/// Workspace另存新檔
		/// <para>此方法將直接指定儲存Workspace至特定路徑</para>
		/// </summary>
		/// <param name="path">指定存檔路徑</param>
		/// <param name="overWrite">如檔案已經存在是否覆蓋？ True:覆蓋 False:放棄並發Exception</param>
		public void SaveWorkspaceAs(string path, bool overWrite = true) {
			lock (mIServer) {
				string strPath = mIServer.SaveLocalWorkspace(path, null);
				CtFile.CopyFile(strPath, path, overWrite);
				CtFile.DeleteFile(strPath);
			}
		}

		/// <summary>取得當前整體速度，並透過數值事件回傳</summary>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		public void RequestSpeed(int vpNum = 0) {
			int spd = mVpObj.Obj(link => link.MonitorSpeed, vpNum);
			OnNumericEventOccur(AceNumericEvents.SpeedChanged, spd);
		}

		/// <summary>取得當前電源狀態，並透過布林事件回傳</summary>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		public void RequestPower(int vpNum = 0) {
			bool pwr = mVpObj.Obj(link => link.HighPower, vpNum);
			OnBoolEventOccur(AceBoolEvents.PowerChanged, pwr);
		}

		/// <summary>重設 V+ 系統裡所有的 V+ 全域變數與 V+ 程式</summary>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		/// <returns>Status Code</returns>
		public void ZeroMemory(int vpNum = 0) {
			System.Threading.Tasks.Task.Run((Action)tsk_ZeroMemory);
			mVpObj.Obj(link => link.ZeroMemory(null /*VpLinkObj_Progress*/), vpNum);
			mProgExit = true;
		}

		/// <summary>從已連線之 AceServer 尋找特定路徑裡之物件，將回傳該路徑下所有物件</summary>
		/// <param name="path">欲取的物件集合之路徑</param>
		/// <param name="obj">該路徑下之物件集合</param>
		public void FindObject(string path, out List<object> obj) {
			List<object> objTemp = new List<object>();
			if ((path != "") && (mIServer != null)) {
				foreach (var item in (mIServer.Root[path] as IAceObjectCollection).ToArray()) {
					objTemp.Add(item);
				}
			}
			obj = objTemp;
		}

		/// <summary>從已連線之 AceServer 尋找特定路徑裡之物件，將回傳該路徑下所有物件名稱</summary>
		/// <param name="path">欲取的物件名稱之路徑</param>
		/// <param name="obj">該路徑下之物件名稱集合</param>
		public void FindObject(string path, out List<string> obj) {
			List<string> objTemp = new List<string>();
			if ((path != "") && (mIServer != null)) {
				foreach (var item in (mIServer.Root[path] as IAceObjectCollection).ToArray()) {
					objTemp.Add(item.FullPath);
				}
			}
			obj = objTemp;
		}

		/// <summary>從已連線之 AceServer 尋找特定類型，且名稱具有特定關鍵字之物件</summary>
		/// <param name="keyWord">搜尋名稱之關鍵字</param>
		/// <param name="type">欲搜尋的類型。如 typeof(IAdeptController)</param>
		/// <param name="obj">物件路徑集合</param>
		public void FindObject(string keyWord, Type type, out List<string> obj) {
			List<string> objTemp = new List<string>();
			if ((keyWord != "") && (mIServer != null)) {
				/*-- 搜尋IAceServer裡的物件，並過濾出特定型態之物件，檢查是否含有關鍵字... --*/
				foreach (var item in mIServer.Root.Filter(new ObjectTypeFilter(type), true)) {
					if (item.FullPath.Contains(keyWord))
						objTemp.Add(item.FullPath);
				}
			}
			obj = objTemp;
		}

		/// <summary>刪除 AceServer 裡之特定路徑物件</summary>
		/// <param name="path">欲刪除之物件路徑</param>
		/// <returns>Status Code</returns>
		public void DeleteObject(string path) {
			if ((path != "") && (mIServer != null)) {
				IAceObject obj = FindObject(path) as IAceObject;
				if (obj != null) {
					IAceObjectCollection parent = obj.ParentCollection;
					if (parent != null) parent.Remove(obj);
					obj.Dispose();
				}
			} else throw new ArgumentNullException("Path", "Path can not be null");
		}

		/// <summary>等待 Ormon ACE 軟體開啟</summary>
		/// <param name="time">等待逾時時間，單位為毫秒(Milliseconds, ms)</param>
		/// <returns>是否有成功開啟？  (<see langword="true"/>)已成功開啟 (<see langword="false"/>)開啟失敗</returns>
		private bool WaitAceStartUp(int time) {
			return !CtTimer.WaitTimeout(
				time,
				token => {
					do {
						Process[] procColl = Process.GetProcessesByName("Ace");
						if (procColl != null && procColl.Length > 0) {
							string title = procColl[0].MainWindowTitle.Trim();
							if (title.Length > 1) {
								token.WorkDone();
								Console.WriteLine(title);
							}
						}
						CtTimer.Delay(100);
					} while (!token.IsDone);
				}
			);
		}

		/// <summary>透過 Shell 方式搜尋 Adept ACE 的檔案位置</summary>
		/// <returns>Ace.exe 路徑</returns>
		private string WhereAce() {
			/* 如直接用 Enviroment 去抓 %programfiles%，疑似因為 CtLib 是 x86 的關係會抓到 (x86) 的，故採兩段式搜尋~ */
			string sysDrive = Environment.ExpandEnvironmentVariables("%SystemDrive%");
			string progFilePath = string.Format("{0}Program Files", CtFile.BackSlash(sysDrive));
			string outMsg, errMsg;
			CtApplication.CmdProcess("WHERE /R \"" + progFilePath + "\" Ace.exe", out outMsg, out errMsg);
			if (string.IsNullOrEmpty(errMsg)) return outMsg.Trim();
			else {
				progFilePath = string.Format("{0}Program Files (x86)", CtFile.BackSlash(sysDrive));
				CtApplication.CmdProcess("WHERE /R \"" + progFilePath + "\" Ace.exe", out outMsg, out errMsg);
				if (string.IsNullOrEmpty(errMsg)) return outMsg.Trim();
				else return string.Empty;
			}
		}

		/// <summary>透過 PipeStream 傳送資料至 Client 端</summary>
		private void SendDataToClient(byte[] data) {
			using (NamedPipeClientStream pipStrm = new NamedPipeClientStream(".", PIPE_NAME, PipeDirection.Out)) {
				try {
					pipStrm.Connect(100);
					pipStrm.Write(data, 0, data.Length);
				} catch (Exception) {
					/* 不知為何 -= 不能取消訂閱事件，故 CtAce 關掉後 ACE 仍會進來這裡，只好使用 try-catch 硬包囉！ */
				}
			}
		}

		/// <summary>解析 PipeStream 接收到的資料</summary>
		/// <param name="rxData">從 PipeStream 收到的資料</param>
		/// <param name="time">收到資料之時間點</param>
		private void PipeDecode(List<byte> rxData, DateTime time) {
			if (rxData != null && rxData.Count > 0) {
				ServerEvents srvEv = (ServerEvents)rxData[0];
				if (!Enum.IsDefined(typeof(ServerEvents), srvEv)) return;
				switch (srvEv) {
					case ServerEvents.AwpSaved:
						byte[] wask = rxData.GetRange(1, rxData.Count - 1).ToArray();
						string awp = Encoding.UTF8.GetString(wask);
						Console.WriteLine(awp);
						OnNotifyEventOccur(AceNotifyEvents.WorkspaceSaved);
						break;
					case ServerEvents.AwpShutdown:
						OnNotifyEventOccur(AceNotifyEvents.AceShutdown);
						break;
					case ServerEvents.AwpLoad:
						OnNotifyEventOccur(AceNotifyEvents.WorkspaceLoaded);
						break;
					case ServerEvents.AwpUnload:
						OnNotifyEventOccur(AceNotifyEvents.WorkspaceUnloaded);
						break;
					case ServerEvents.Calibration:
						OnBoolEventOccur(AceBoolEvents.Calibration, CtConvert.CBool(rxData[1]));
						break;
					case ServerEvents.Contents:
						byte[] cntx = rxData.GetRange(1, rxData.Count - 1).ToArray();
						string name = Encoding.UTF8.GetString(cntx);
						OnNumericEventOccur(AceNumericEvents.Contents, name);
						break;
					case ServerEvents.EStop:
						OnBoolEventOccur(AceBoolEvents.Calibration, CtConvert.CBool(rxData[1]));
						break;
					case ServerEvents.MonitorSpeed:
						OnNumericEventOccur(AceNumericEvents.SpeedChanged, rxData[1]);
						break;
					case ServerEvents.Power:
						OnBoolEventOccur(AceBoolEvents.PowerChanged, CtConvert.CBool(rxData[1]));
						break;
					case ServerEvents.Progress:
						mProgPercent = rxData[1];
						byte[] msgData = rxData.GetRange(2, rxData.Count - 2).ToArray();
						mProgMsg = Encoding.UTF8.GetString(msgData);
						mProgChange = true;
						//Console.WriteLine($"{mProgMsg} : {mProgPercent}%");
						break;
					default:
						break;
				}
			}
		}
		#endregion

		#region Function - IDisposable Implements
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

		/// <summary>關閉與 Adept ACE 之連線並釋放資源</summary>
		public void Dispose() {
			try {
				Dispose(true);
				GC.SuppressFinalize(this);
			} catch (ObjectDisposedException ex) {
				ExceptionHandle(Stat.ER_SYSTEM, ex);
			}
		}
		#endregion

		#region Function - Connections
		/// <summary>中斷與Adept ACE Server之連結，並將相關元件清除為null</summary>
		/// <returns>Status Code</returns>
		public void Disconnect() {
			try {
				/*-- VpLinks --*/
				if (mVpObj != null) {
					mVpObj.Dispose();
					mVpObj = null;
				}

				/*-- Robots --*/
				if (mRobots != null)
					mRobots = null;

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

				/*-- IAceClient --*/
				if (mIClient != null) {
					mIClient.DisposePlugins();
					mIClient.Dispose();
					mIClient = null;
				}

				/*-- IAceServer --*/
				if (mIServer != null)
					mIServer = null;

				/*-- 更改Flag表示已經Dispose --*/
				mAceDispose = true;
				IsAceConnected = false;

				/*-- 發布Event --*/
				OnBoolEventOccur(AceBoolEvents.Connection, false);
			} catch (Exception ex) {
				ExceptionHandle(Stat.ER_SYSTEM, ex);
			}
		}

		/// <summary>建立所有元件之連線，並尋找控制器與手臂</summary>
		/// <param name="ctrl">是否含有SmartController。此將影響連線與建立物件方式</param>
		/// <param name="emulate">[Server Only] 是否使用 Emulation 模式</param>
		/// <remarks>如果有遇到系統於 AceClient (@825) 卡住，請於 App.config 檢查是否有加入 useLegacyV2RuntimeActivationPolicy="true"</remarks>
		public void Connect(ControllerType ctrl, bool emulate = false) {
			Connect(ctrl, WorkspacePath, emulate);
		}

		/// <summary>建立所有元件之連線，並尋找控制器與手臂</summary>
		/// <param name="ctrl">是否含有SmartController。此將影響連線與建立物件方式</param>
		/// <param name="awpPath">如尚未開啟 Adept ACE，欲自動載入的 Workspace</param>
		/// <param name="emulate">[Server Only] 是否使用 Emulation 模式</param>
		/// <remarks>如果有遇到系統於 AceClient (@825) 卡住，請於 App.config 檢查是否有加入 useLegacyV2RuntimeActivationPolicy="true"</remarks>
		public void Connect(ControllerType ctrl, string awpPath, bool emulate = false) {
			WorkspacePath = awpPath;
			EmulationMode = emulate;

			/*-- 如果是 ClientMode，檢查是否有開啟 Ace.exe 了 --*/
			if (mClientMode) {
				bool procExist = CtApplication.IsProcessExist("Ace");
				if (!procExist) {
					if (!CtFile.IsFileExist(awpPath)) throw new FileLoadException("The workspace is not exist.");
					string format = "culture=en {0}=\"{1}\"";
					string arg = string.Empty;
					if (emulate) arg = string.Format(format, "openfileemul", awpPath);
					else arg = string.Format(format, "datafile", awpPath);
					string acePath = WhereAce();
					if (CtFile.IsFileExist(acePath)) {
						CtApplication.ExecuteProcess(acePath, arg);
						if (!WaitAceStartUp(180000)) {
							throw new TimeoutException("Waiting Adept ACE start-up timeout");
						}
					} else throw new ArgumentNullException("AcePath", "Can not find Ace.exe program");
				}
			} else {
				CtApplication.KillProcess("Ace");   //把舊的給砍了
				CtApplication.ExecuteProcess(WhereAce(), "culture=en server");
				CtTimer.Delay(100); //等待 ACE Server 跑起來
			}

			/*-- 是否有SmartController --*/
			switch (ctrl) {
				case ControllerType.SmartController:
					mSmartCtrl = true;
					break;
				case ControllerType.Embedded:
					mSmartCtrl = false;
					break;
			}

			/*-- 如果是重新連線，把原有東西給Dispose掉 --*/
			if ((mIServer != null) || (mIClient != null))
				Disconnect();

			/*-- 建立IAceServer與IAceClient供後續使用 --*/
			Connect(out mIServer, out mIClient, mClientMode);
			if (mIServer == null || mIClient == null) {
				Dispose();
				throw (new Exception("IAceServer與IAceClient建立失敗"));
			}

			/*-- 載入Workspace --*/
			if (!mClientMode) {
				mIServer.EmulationMode = EmulationMode;
				mProgChange = false;
				mProgExit = false;
				System.Threading.Tasks.Task.Run((Action)tsk_LoadAwpProg);
				bool tmo = CtTimer.WaitTimeout(
					TimeSpan.FromMinutes(3),
					(token, path) => {
						mIServer.Clear();
						mIServer.LoadLocalWorkspace(path[0].ToString(), null /*VpLinkObj_Progress*/, emulate);
						token.WorkDone();
					},
					awpPath
				);
				mProgExit = true;
				if (tmo) throw (new Exception("等待載入 Workspace 過久，請檢查相關設定"));
			} else if (string.IsNullOrEmpty(mIServer.LoadedWorkspacePath)) {
				/* 以下方法可正確載入 workspace，但是 ACE 的 Getting Start 視窗會卡住阿... 先拋例外~ */
				//mProgExit = false;
				//System.Threading.Tasks.Task.Run((Action)tsk_LoadAwpProg);
				//mIServer.LoadLocalWorkspace(awpPath, VpLinkObj_Progress, emulate);
				//mProgExit = true;

				throw new InvalidOperationException("Adept ACE does not load any workspace");
			}

			/*-- 分析IAceServer，將各元件拉至mAceObj，同時可以抓Controller/Robot/Variable等等 --*/
			LinkAceObj(mIServer);

			/*-- 確認並等待 Controller 有連線 --*/
			CtTimer.Delay(500);
			if (!mVpObj.Obj(obj => obj.IsAlive)) {
				bool conCtrl = false;
				if (mClientMode) {
					conCtrl = CtMsgBox.Show(
								"Controller Connection",
								"It is not connect to controller, trying to connect?",
								MsgBoxBtn.YesNo,
								MsgBoxStyle.Question
							) == MsgBoxBtn.Yes;
				} else conCtrl = true;
				if (conCtrl) ConnectWithController(true);
			}

			/*-- 取得 Task 之最大編號 --*/
			if (mVpObj.Obj(obj => obj.IsAlive)) {
				int maxTask = mVpObj.Obj(link => link.MaxTaskNumber);
				int srvTask = mVpObj.Link(link => link.Status()).Count(vpStt => vpStt.MainProgram == ("sv.q_mgr") || vpStt.MainProgram == ("a.ace_srvr"));
				mMaxTaskNum = maxTask - srvTask;
			}

			/*-- 建立相關Event --*/
			AddEventHandler();

			/*-- 連結子模組 --*/
			mIO = new CtAceIO(mVpObj);
			mVariables = new CtAceVariable(mIServer, mVpObj);
			mTask = new CtAceTask(mVpObj, mMaxTaskNum);
			mTask.OnTaskChanged += mTask_OnTaskChanged;
			mMotion = new CtAceMotion(mIServer, mSmartCtrl, mVpObj, mRobots);
			mVision = new CtAceVision(mIClient, mIServer);
			mScript = new CtAceScript(mIServer, mVpObj);

			/*-- 發布Event --*/
			OnBoolEventOccur(AceBoolEvents.Connection, true);
		}

		/// <summary>嘗試更改 V+ 連結器與 V+ 系統連線狀態</summary>
		/// <param name="connect">欲寫入的連線狀態</param>
		/// <param name="vpNum">V+ 連結器之編號，從 0 開始</param>
		public void ConnectWithController(bool connect, int vpNum = 0) {
			System.Threading.Tasks.Task.Run((Action)tsk_CtrlConnect);
			mVpObj.Obj(link => link.ChangeConnectionState(connect, null /*VpLinkObj_Progress*/), vpNum);
			mProgExit = true;

			/*-- 如果有成功連線，取得 Task 之最大編號 --*/
			if (mVpObj.Obj(obj => obj.IsAlive)) {
				int maxTask = mVpObj.Obj(link => link.MaxTaskNumber);
				int srvTask = mVpObj.Link(link => link.Status()).Count(vpStt => vpStt.MainProgram == ("sv.q_mgr") || vpStt.MainProgram == ("a.ace_srvr"));
				mMaxTaskNum = maxTask - srvTask;
			}
		}

		#endregion

		#region Function - Vp+ State
		/// <summary>取得當前是否已與 V+ 系統連結</summary>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		/// <returns>(<see langword="true"/>)已連接 V+ 系統  (<see langword="false"/>)尚未連線</returns>
		public bool IsVpConnected(int vpNum = 0) {
			return mVpObj?.Obj(link => link.IsAlive, vpNum) ?? false;
		}

		/// <summary>取得 V+ 連結器之電源狀態</summary>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		/// <returns>(<see langword="true"/>)HightPower  (<see langword="false"/>)LowPower</returns>
		public bool IsHighPower(int vpNum = 0) {
			return mVpObj?.Obj(link => link.HighPower, vpNum) ?? false;
		}

		/// <summary>取得整體速度(Monitor Speed)</summary>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		/// <returns>(-1)尚未連線  (Other)整體速度，0 ~ 100、單位為 %</returns>
		public int MonitorSpeed(int vpNum = 0) {
			return mVpObj?.Obj(link => link.MonitorSpeed, vpNum) ?? -1;
		}
		#endregion

		#region Function - Public Operations

		/// <summary>設定 HighPower 狀態。如欲送電，則會出現介面並等待按鈕按下</summary>
		/// <param name="power">(<see langword="true"/>)送電  (<see langword="false"/>)關閉電源</param>
		/// <param name="prog">是否顯示進度條？</param>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		/// <remarks>因 Johnson 於 2015/02/26 提出某些電腦在開關電時跳出的 CtProgress 造成莫名 Exception 導致介面崩潰，加入 prog 方便使用</remarks>
		public void SetPower(bool power, bool prog = true, int vpNum = 0) {
			CtProgress frmProg = null;
			if (prog) {
				if (power) {
					frmProg = new CtProgress(
						ProgBarStyle.Countdown,
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

			if (mVpObj != null) {
				mVpObj.Obj(
					obj => {
						if (power) {
							obj.Calibrate();
							obj.HighPower = true;
						} else obj.HighPower = false;
					},
					vpNum
				);
			} else {
				if (frmProg != null) frmProg.Close();
				throw new ArgumentNullException("Can not find controller connection. Please connecte fist.");
			}

			if (frmProg != null) frmProg.Close();
		}

		/// <summary>變更整體速度(MonitorSpeed)</summary>
		/// <param name="speed">欲變更之速度</param>
		/// <param name="vpNum">V+ 連結器編號，從 0 開始</param>
		public void SetSpeed(int speed, int vpNum = 0) {
			if (speed < ACE_MINSPEED)
				speed = ACE_MINSPEED;
			else if (speed > ACE_MAXSPEED)
				speed = ACE_MAXSPEED;

			if (mVpObj != null) {
				mVpObj.Obj(obj => obj.MonitorSpeed = speed, vpNum);
			} else throw new ArgumentNullException("Can not find controller connection. Please connecte fist.");
		}

		/// <summary>Jog Pendant</summary>
		/// <param name="ctStyle">是否使用 CASTEC Style Pendant? (<see langword="true"/>)CASTEC Style  (<see langword="false"/>)Adept Origin</param>
		public void Pendant(bool ctStyle = false) {
			if (ctStyle) {
				CtAcePendant pendant = new CtAcePendant(this);
				pendant.ShowDialog();
				pendant.Dispose();
			} else {
				ControlPanelManager panelMag = new ControlPanelManager();
				panelMag.LaunchControlForm(null, mIClient, mRobots.First().Value, true);
				do {
					CtTimer.Delay(10);
					Application.DoEvents();
				} while (panelMag.GetControlForm() != null);
			}
		}

		/// <summary>取得手臂所含有的 Joint 數量</summary>
		/// <param name="robotNum">欲取得的手臂編號</param>
		/// <returns>Joint 數量</returns>
		public int GetJointCount(int robotNum) {
			return mRobots[robotNum].JointCount;
		}

		#endregion

		#region Function - Events

		#region Client Events
		private void AceClient_WorkspaceUnload(object sender, EventArgs e) {
			IsAceConnected = false;
			OnNotifyEventOccur(AceNotifyEvents.WorkspaceUnloaded);
		}

		private void AceClient_WorkspaceSaved(object sender, WorkspaceSaveEventArgs e) {
			OnNotifyEventOccur(AceNotifyEvents.WorkspaceSaved);
		}

		private void AceClient_WorkspaceLoad(object sender, EventArgs e) {
			OnNotifyEventOccur(AceNotifyEvents.WorkspaceLoaded);
		}

		private void AceClient_BeforeShutdown(object sender, EventArgs e) {
			IsAceConnected = false;
			OnNotifyEventOccur(AceNotifyEvents.AceShutdown);
		}
		#endregion

		#region Server Events
		private void VpLinkObj_PowerStateChanged(object sender, BooleanStateChangeEventArgs e) {
			DateTime time = DateTime.Now;
			byte[] data = new byte[] { (byte)ServerEvents.Power, (byte)(e.State ? 1 : 0) };
			SendDataToClient(data);
		}

		private void VpLinkObj_NodeStateChanged(object sender, BooleanStateChangeEventArgs e) {
			bool isDispose = (bool)sender.GetType().GetProperty("IsBeingDisposed").GetValue(sender);
			if (!isDispose) {
				byte[] data = new byte[] { (byte)ServerEvents.EStop, (byte)(e.State ? 1 : 0) };
				SendDataToClient(data);
			}
		}

		private void VpLinkObj_MonitorSpeedChanged(object sender, NumericChangeEventArgs e) {
			DateTime time = DateTime.Now;
			byte[] data = new byte[] { (byte)ServerEvents.MonitorSpeed, (byte)e.Value };
			SendDataToClient(data);
		}

		private void VpLinkObj_EStopStateChanged(object sender, BooleanStateChangeEventArgs e) {
			byte[] data = new byte[] { (byte)ServerEvents.EStop, (byte)(e.State ? 1 : 0) };
			SendDataToClient(data);
		}

		private void VpLinkObj_CalibrationStateChanged(object sender, BooleanStateChangeEventArgs e) {
			byte[] data = new byte[] { (byte)ServerEvents.Calibration, (byte)(e.State ? 1 : 0) };
			SendDataToClient(data);
		}

		private void mIServerRoot_ContentsChanged(object sender, ContentsChangeEventArgs e) {
			IAceObjectCollection root = sender as IAceObjectCollection;
			if (!root.IsBeingDisposed && !e.Target.IsBeingDisposed) {
				byte[] name = Encoding.UTF8.GetBytes(e.Name);
				List<byte> data = new List<byte> { (byte)ServerEvents.Contents };
				data.AddRange(name);
				SendDataToClient(data.ToArray());
			}
		}

		private void AceSrv_WorkspaceUnload(object sender, EventArgs e) {
			byte[] data = new byte[] { (byte)ServerEvents.AwpUnload };
			SendDataToClient(data);
		}

		private void AceSrv_WorkspaceSaved(object sender, WorkspaceSaveEventArgs e) {
			byte[] name = Encoding.UTF8.GetBytes(e.Path);
			List<byte> data = new List<byte> { (byte)ServerEvents.AwpSaved };
			data.AddRange(name);
			SendDataToClient(data.ToArray());
		}

		private void AceSrv_WorkspaceLoad(object sender, WorkspaceLoadEventArgs e) {
			byte[] name = Encoding.UTF8.GetBytes(e.Path);
			List<byte> data = new List<byte> { (byte)ServerEvents.AwpLoad };
			data.AddRange(name);
			SendDataToClient(data.ToArray());
		}

		private void AceSrv_Shutdown(object sender, EventArgs e) {
			byte[] data = new byte[] { (byte)ServerEvents.AwpShutdown };
			SendDataToClient(data);
		}
		#endregion

		#region ObjectProperty Events

		void rx_ObjectDisposing(object sender, EventArgs e) {
			if (!mAceDispose) {
				mAceDispose = true;
				OnNotifyEventOccur(AceNotifyEvents.AceShutdown);
			}
		}

		void rx_ObjectPropertyModified(object sender, PropertyModifiedEventArgs e) {
			try {
				switch (e.PropertyName) {
					case "HighPower":
						OnBoolEventOccur(AceBoolEvents.PowerChanged, mVpObj.Obj(obj => obj.HighPower));
						break;

					case "MonitorSpeed":
						OnNumericEventOccur(AceNumericEvents.SpeedChanged, mVpObj.Obj(obj => obj.MonitorSpeed));
						break;

					case "Line":
						OnNotifyEventOccur(AceNotifyEvents.ProgramModified);
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
				OnNumericEventOccur(AceNumericEvents.TaskStateChanged, objTask);
			} catch (Exception ex) {
				ExceptionHandle(Stat.ER_SYSTEM, ex);
			}
		}

		void mTask_OnTaskChanged(object sender, CtAceTask.TaskEventArgs e) {
			OnTaskEventOccur(e);
		}
		#endregion

		#region PipeStream Events
		private void rx_SrvPipeEvents(object sender, PipeEventArgs e) {
			if (e.Event == PipeEvents.DataReceived) {
				PipeDecode(e.Value as List<byte>, DateTime.Now);
			}
		}
		#endregion

		#region Progress
		private void VpLinkObj_Progress(object sender, ProgressEventArgs e) {
			byte[] msg = Encoding.UTF8.GetBytes(e.UserState.ToString());
			List<byte> data = new List<byte> { (byte)ServerEvents.Progress, (byte)e.ProgressPercentage };
			data.AddRange(msg);
			SendDataToClient(data.ToArray());
		}

		private void tsk_ZeroMemory() {
			mProgExit = false;
			CtProgress prog = new CtProgress(ProgBarStyle.Percent, "Zero Memory", "Cleaning V+ variables and initialize with controller", 100);
			while (!mProgExit) {
				if (mProgChange) {
					mProgChange = false;
					prog.UpdateStep(mProgPercent, mProgMsg);
				}
				CtTimer.Delay(10);
			}
			prog.Close();
		}

		private void tsk_CtrlConnect() {
			mProgExit = false;
			CtProgress prog = new CtProgress(ProgBarStyle.Percent, "Controller State", "Waiting for controller...", 100);
			while (!mProgExit) {
				if (mProgChange) {
					mProgChange = false;
					prog.UpdateStep(mProgPercent, mProgMsg);
				}
				CtTimer.Delay(10);
			}
			prog.Close();
		}

		private void tsk_LoadAwpProg() {
			mProgExit = false;
			CtProgress prog = new CtProgress(ProgBarStyle.Percent, "Load Workspace", "Loading workspace to ACE", 100);
			while (!mProgExit) {
				if (mProgChange) {
					mProgChange = false;
					prog.UpdateStep(mProgPercent, mProgMsg);
				}
				CtTimer.Delay(10);
			}
			prog.Close();
		}
		#endregion

		#endregion

	}
}
