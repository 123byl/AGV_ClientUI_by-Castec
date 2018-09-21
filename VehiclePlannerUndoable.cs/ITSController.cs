using Geometry;
using Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehiclePlanner.Core;
using SerialData;
using SerialCommunicationData;
using GLCore;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using AsyncSocket;
using System.Threading;
using CtExtendLib;

namespace VehiclePlannerUndoable.cs
{

	/// <summary>
	/// 以Undoable實作之ITS控制器介面
	/// </summary>
	public interface IITSController_Undoable : IBaseITSController
	{
		AGVStatus Status { get; }
		void SetPosition(Vector2D Vector);

		int LaserID { get; set; }
		int PathID { get; set; }

		bool ConnectStatus { get; set; }

		void RequireIni();

		void UploadIni();

		event EventHandler ShowMotionController;

		event EventHandler CloseMotionController;

		event LoadMapEventHandler OpenMap;

		event ConnectStatusChangedEvent ConnectStatusChanged;

		event ChargeChangeHandler ChargeChange;

		event EventHandler OnFocus;
	}
	public delegate void ChargeChangeHandler(bool value);

	public delegate void LoadMapEventHandler(string path);

	/// <summary>
	/// ITS控制器
	/// </summary>
	public class ITSController : BaseiTSController<Serializable, Serializable>, IITSController_Undoable
	{

		#region Declaration - Fields

		/// <summary>
		/// 序列传输用户端
		/// </summary>
		private SerialClient mClient = new SerialClient();
		/// <summary>
		/// iTS狀態
		/// </summary>
		private AGVStatus mStatus = new AGVStatus();
		/// <summary>
		/// Path 繪圖物件ID
		/// </summary>
		private int mPathID = -1;
		/// <summary>
		///  Laser繪圖物件ID
		/// </summary>
		private int mLaserID = -1;
		/// <summary>
		/// 掃描地圖暫存名稱
		/// </summary>
		private string _oriName = null;
		/// <summary>
		/// 連線狀態
		/// </summary>
		private bool mConnectStatus = false;
		/// <summary>
		/// 開啟Controller
		/// </summary>
		public event EventHandler ShowMotionController;
		/// <summary>
		/// 關閉Controller
		/// </summary>
		public event EventHandler CloseMotionController;
		/// <summary>
		/// 開啟地圖檔
		/// </summary>
		public event LoadMapEventHandler OpenMap;
		/// <summary>
		/// 連線狀態改變
		/// </summary>
		public event ConnectStatusChangedEvent ConnectStatusChanged;
		/// <summary>
		/// 充電狀態改變
		/// </summary>
		public event ChargeChangeHandler ChargeChange;
		/// <summary>
		/// 聚焦
		/// </summary>
		public event EventHandler OnFocus;
		#endregion Declaration - Fields

		#region Declaration - Properties
		/// <summary>
		/// iTS狀態
		/// </summary>
		public AGVStatus Status
		{
			get { return mStatus; }
			set
			{
				if (value != null && mStatus != value)
				{
					mStatus = value;
					GLCMD.CMD.AddAGV(1, mStatus.Name, mStatus.X, mStatus.Y, mStatus.Toward);
					OnPropertyChanged();
					if (IsFocus) OnFocus?.Invoke(this, EventArgs.Empty);
				}
			}

		}
		/// <summary>
		/// 是否已连线
		/// </summary>
		public override bool IsConnected
		{
			get
			{
				OnPropertyChanged();
				return mClient.ConnectStatus == AsyncSocket.EConnectStatus.Connect;
			}
		}
		/// <summary>
		/// 雷射繪圖ID
		/// </summary>
		public int LaserID { get => mLaserID; set => mLaserID = value; }
		/// <summary>
		/// 路徑繪圖ID
		/// </summary>
		public int PathID { get => mPathID; set => mPathID = value; }
		/// <summary>
		/// 連線狀態
		/// </summary>
		public bool ConnectStatus
		{
			get => mConnectStatus; set
			{
				mConnectStatus = value;
				OnPropertyChanged();
			}
		}
		#endregion Declaration - Properties

		#region Function - Public Methods

		public ITSController() : base()
		{
			mPathID = GLCMD.CMD.AddMultiStripLine("Path", null);
			mLaserID = GLCMD.CMD.AddMultiPair("Laser", null);
		}

		public override void AutoReport(bool auto)
		{
			try
			{
				bool isAutoReport = auto;
				var laser = AutoReportLaser(isAutoReport);
				//var status = AutoReportStatus(isAutoReport);
				var path = AutoReportPath(isAutoReport);
				if (laser || path)
				{
					OnBalloonTip("AutoReport", $"AutoReport {(auto ? "Open" : "Close")}");
					IsAutoReport = isAutoReport;
					if (!isAutoReport)
					{
						GLCMD.CMD.SaftyEditMultiGeometry<IPair>(mPathID, true, (list) => { list.Clear(); });
						GLCMD.CMD.SaftyEditMultiGeometry<IPair>(mLaserID, true, (list) => { list.Clear(); });
					}
				}
			}
			catch (Exception ex)
			{
				OnConsoleMessage(ex.Message);
			}
		}

		public override void GetOri()
		{
			bool? success = null;
			string oriName = null;
			try
			{
				string oriList = RequestOriList();
				if (!string.IsNullOrEmpty(oriList))
				{
					oriName = SelectFile(oriList);
					if (!string.IsNullOrEmpty(oriName))
					{
						BaseFileReturn ori = RequestOriFile(oriName);
						if (ori != null)
						{
							System.Windows.Forms.SaveFileDialog saveOri = new System.Windows.Forms.SaveFileDialog() { InitialDirectory = @"D:\MapInfo\Client" };
							if (saveOri.ShowDialog() == System.Windows.Forms.DialogResult.OK)
							{
								if (ori.SaveAs(saveOri.FileName))
								{
									success = true;
									OnConsoleMessage($"Planner - {ori.FileName} download completed");
								}
								else
								{
									success = false;
									OnConsoleMessage($"Planner - {ori.FileName} failed to save");
								}
							}

						}
					}
				}
			}
			catch (Exception ex)
			{
				OnConsoleMessage(ex.Message);
			}
			finally
			{
				if (success != null)
				{
					OnBalloonTip("Donwload", $"{oriName}.ori is downloaded {(success == true ? "successfully" : "failed")} ");
				}
			}
		}

		public void RequireIni()
		{
			BaseFileReturn Info = GetParameter();
			bool success = Info.Requited;
			if (success)
			{
				System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog() { InitialDirectory = Environment.SpecialFolder.Desktop.ToString() };
				System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.None;
				DelInvoke?.Invoke(() => result = dialog.ShowDialog());
				success = Info.SaveAs(dialog.FileName);
				if (success)
				{
					OnConsoleMessage($"Planner - Ini download completed");
				}
				else
				{
					OnConsoleMessage($"Planner - Ini failed to save");
				}
			}
			else
			{
				OnConsoleMessage($"Planner - Ini failed to download");
			}
			OnBalloonTip("Dowload", $"Ini download {(success ? "successfully" : "failed")}");
		}

		public void UploadIni()
		{
			string IniDir = Environment.SpecialFolder.Desktop.ToString();
			string filter = "Ini file(*.ini)|*.ini|All file(*.*)|*.*";
			System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.None;
			System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog()
			{
				InitialDirectory = IniDir,
				Filter = filter
			};
			DelInvoke?.Invoke(() => result = dialog.ShowDialog());
			bool success = result == System.Windows.Forms.DialogResult.OK;
			if (success)
			{
				SerialData.FileInfo file = new SerialData.FileInfo();
				success = file.Load(dialog.FileName);
				if (success)
				{
					OnConsoleMessage($"Planner - Ini upload completed");
				}
				else
				{
					OnConsoleMessage($"Planner - Ini failed to load");
				}
			}
			OnBalloonTip("Dowload", $"Ini upload {(success ? "successfully" : "failed")}");
		}
		protected override void LoadMap(string path)
		{
			OpenMap?.Invoke(path);
		}
		#endregion Function - Public Methods

		#region Function - Override Methods

		/// <summary>
		/// 用户端初始化
		/// </summary>
		protected override void ClientInitial()
		{
			if (mClient != null)
			{
				try
				{
					mClient.Dispose();
				}
				catch (Exception)
				{

				}
			}
			mClient = new SerialClient();
			mClient.ConnectStatusChangedEvent += MClient_ConnectStatusChangedEvent;
			mClient.ReceivedSerialDataEvent += MClient_ReceivedSerialDataEvent;
		}

		/// <summary>
		/// 连线至伺服端
		/// </summary>
		/// <param name="ip">伺服端IP</param>
		/// <param name="port">伺服端埠号</param>
		protected override void ClientConnect(string ip, int port)
		{
			mClient.Connect(ip, port);
		}

		/// <summary>
		/// 停止与伺服端连线
		/// </summary>
		protected override void ClientStop() => mClient.Disconnect();

		/// <summary>
		/// 传送命令
		/// </summary>
		/// <param name="packet">命令物件</param>
		/// <returns>是否传送成功</returns>
		protected override bool ClientSend(Serializable packet)
		{
			bool success = true;
			try
			{
				mClient.Send(packet);
			}
			catch (Exception)
			{
				success = false;
			}
			return success;
		}

		/// <summary>
		/// 检测是否等待中的命令
		/// </summary>
		/// <param name="v">等待的命令</param>
		/// <param name="packet">要发送的命令</param>
		/// <returns>是否等待中命令</returns>
		protected override bool IsWaitingCmd(CtTaskCompletionSource<Serializable, Serializable> v, Serializable packet) => v.WaitCmd.TxID == packet.TxID;

		/// <summary>
		/// 检测是否等待中的回复
		/// </summary>
		/// <param name="v">等待的命令</param>
		/// <param name="response">收到的回复</param>
		/// <returns>是否是等待中的命令</returns>
		protected override bool IsWaitingResponse(CtTaskCompletionSource<Serializable, Serializable> v, Serializable response) => v.WaitCmd.TxID == response.TxID;

		/// <summary>
		/// 产生命令标题
		/// </summary>
		/// <param name="packet">命令物件</param>
		/// <returns>命令标题</returns>
		protected override string CmdTitle(Serializable packet) => $"{packet.ToString()}({packet.TxID}):";


		protected override void DoReceiveAction(Serializable response)
		{
			if (response is AGVStatus)
			{
				if (Status.Description != (response as AGVStatus).Description && (response as AGVStatus).Description == EDescription.Charge) ChargeChange?.Invoke(true);
				Status = response as AGVStatus;
			}
			else if (response is AGVPath2D path)
			{
				GLCMD.CMD.SaftyEditMultiGeometry<IPair>(mPathID, true, (line) =>
				{
					line.Clear();
					line.AddRangeIfNotNull(Point2DToPairCollection(path.Points));
				});
			}
			else if (response is AGVLaser laser)
			{
				GLCMD.CMD.SaftyEditMultiGeometry<IPair>(mLaserID, true, (point) =>
				  {
					  point.Clear();
					  point.AddRange(Point2DToPairCollection(laser.Points));
				  }
				);
			}
			else if (response is ConnectInfomation info)
			{
				if (info.IsConnect)
				{
					DelInvoke.Invoke(() => MessageBox.Show($"有其他裝置 {info.IpPort}，嘗試連入ITS", "連線警告"));
				}
				else
				{
					DelInvoke.Invoke(() => MessageBox.Show($"目前由 {info.IpPort} 正在使用，如要使用請先停止他人使用", "連線警告"));
				}
			}
		}

		#region Command & Response
		protected override BaseRequeLaser ImpRequestLaser()
		{
			RequestLaser Info = Send(new RequestLaser(null)) as RequestLaser;
			return new ConvertRequestLaser(Info);
		}

		protected override BaseDoPositionComfirm ImpDoPositionComfirm()
		{
			DoPositionComfirm Info = Send(new DoPositionComfirm(null)) as DoPositionComfirm;
			return new ConvertDoPositionComfirm(Info);
		}

		protected override BaseGoTo ImpGoTo(string goalName)
		{
			GoTo Info = Send(new GoTo(goalName)) as GoTo;
			return new ConvertGoTo(Info);
		}

		protected override BaseRequestPath ImpRequestPath(string goalName)
		{
			RequestPath Info = Send(new RequestPath(goalName)) as RequestPath;
			return new ConvertRequestPath(Info);

		}

		protected override BaseRequestGoalList ImpRequestGoalList()
		{
			RequestGoalList Info = Send(new RequestGoalList(null)) as RequestGoalList;
			return new ConvertRequestGoalList(Info);
		}

		public override BaseFileReturn RequestMapFile(string mapName)
		{
			GetMap Info = Send(new GetMap(mapName)) as GetMap;
			return new RequestMapFile(Info);
		}

		protected override BaseBoolReturn ImpSetServoMode(bool servoOn)
		{
			SetServoMode Info = Send(new SetServoMode(servoOn)) as SetServoMode;
			return new ConvertSetServoMode(Info);
		}

		protected override BaseBoolReturn ImpSetWorkVelocity(int velocity)
		{
			SetWorkVelocity Info = Send(new SetWorkVelocity(velocity)) as SetWorkVelocity;
			return new ConvertSetWorkVelocity(Info);
		}

		protected override BaseListReturn ImpRequestMapList()
		{
			RequestMapList Info = Send(new RequestMapList(null)) as RequestMapList;
			return new ConvertRequestMapList(Info);
		}

		protected override BaseBoolReturn UploadMapToAGV(string mapPath)
		{
			SerialData.FileInfo Data = new SerialData.FileInfo(mapPath);
			UploadMapToAGV Info = Send(new UploadMapToAGV(Data)) as UploadMapToAGV;
			return new ConvertUploadMapToAGV(Info);
		}

		protected override BaseBoolReturn ChangeMap(string mapName)
		{
			ChangeMap Info = Send(new ChangeMap(mapName)) as ChangeMap;
			return new ConvertChangeMap(Info);
		}

		protected override BaseBoolReturn StartManualControl(bool start)
		{
			StartManualControl Info = Send(new StartManualControl(start)) as StartManualControl;
			return new ConvertStartManualControl(Info);
		}

		protected override BaseBoolReturn StopScanning()
		{
			StopScanning Info = Send(new StopScanning(null)) as StopScanning;
			return new ConvertStopScanning(Info);
		}

		protected override BaseSetScanningOriFileName SetScanningOriFileName(string oriName)
		{
			SetScanningOriFileName Info = Send(new SetScanningOriFileName(oriName)) as SetScanningOriFileName;
			return new ConvertSetScanningOriFileName(Info);
		}

		protected override BaseBoolReturn SetManualVelocity(int leftVelocity, int rightVelocity)
		{
			SetManualVelocity Info = Send(new SetManualVelocity(new Velocity(rightVelocity, leftVelocity))) as SetManualVelocity;
			return new ConvertSetManualVelocity(Info);
		}

		/// <summary>
		/// 要求自動回傳雷射資料
		/// </summary>
		/// <param name="on"></param>
		/// <returns></returns>
		protected bool AutoReportLaser(bool on)
		{
			AutoReportLaser Info = (AutoReportLaser)Send(new AutoReportLaser(on));
			bool response = (Info != null) ? Info.Response : false;
			return response;
		}

		protected bool AutoReportStatus(bool on)
		{
			AutoReportStatus Info = (AutoReportStatus)Send(new AutoReportStatus(on));
			bool response = (Info != null) ? Info.Response : false;
			return response;
		}

		protected bool AutoReportPath(bool on)
		{
			AutoReportPath Info = (AutoReportPath)Send(new AutoReportPath(on));
			bool response = (Info != null) ? Info.Response : false;
			return response;
		}

		protected BaseFileReturn GetParameter()
		{
			GetIni Info = Send(new GetIni(null)) as GetIni;
			return new ConvertGetIni(Info);
		}

		protected BaseBoolReturn SetParameter(SerialData.FileInfo Ini)
		{
			SetIni Info = Send(new SetIni(Ini)) as SetIni;
			return new ConvertSetIni(Info);
		}

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="oriName"></param>
		/// <returns></returns>
		protected BaseFileReturn RequestOriFile(string oriName)
		{
			GetOri Info = Send(new GetOri(oriName)) as GetOri;
			return new RequestOriFile(Info);
		}
		protected override string RequestOriList()
		{
			var oriList = (Send(new RequestOriList(null)) as RequestOriList).Response;
			return oriList != null ? string.Join(",", oriList) : null;
		}

		#endregion
		public override void StartScan(bool scan)
		{
			try
			{
				BaseBoolReturn scanReturn = null;
				if (IsScanning != scan)
				{
					if (scan)
					{//開始掃描
						if (mStatus?.Description == EDescription.Idle)
						{
							string oriName = string.Empty;
							if (InputBox.Invoke(out oriName, "MAP Name", "Set Map File Name"))
							{
								scanReturn = SetScanningOriFileName(oriName);
							}
							if (scanReturn.Requited && scanReturn.Value)
							{
								ShowMotionController?.Invoke(this, EventArgs.Empty);
								OnBalloonTip("Scan Map", "Start Scan Map");
								OnConsoleMessage($"iTS - The new ori name is {oriName}.ori");
								_oriName = oriName;
							}
						}
						else
						{
							OnConsoleMessage($"The iTS is now in {mStatus?.Description}, can't start scanning");
						}
					}
					else
					{//停止掃描
					 //IsScanning = false;
						if (true || mStatus?.Description == EDescription.Map)
						{
							CloseMotionController?.Invoke(this, EventArgs.Empty);
							scanReturn = StopScanning();
							OnBalloonTip("Scan Map", "Close Scan Map");
							LoadingForm load = new LoadingForm();
							DelInvoke.Invoke(() => load.Start("Wait Load Map", 3));

							BaseFileReturn ori = RequestOriFile(_oriName);
							if (ori != null)
							{
								FolderBrowserDialog pathOri = new FolderBrowserDialog() { SelectedPath = @"D:\" };
								DialogResult Ans = DialogResult.None;
								DelInvoke?.Invoke(() => Ans = pathOri.ShowDialog());
								if (Ans == DialogResult.OK)
								{
									string directory = pathOri.SelectedPath;
									if (ori.SaveAs(directory))
									{
										OnConsoleMessage($"Planner - {ori.FileName} download completed");
										string path = directory + $@"\{ori.FileName}";
										OriToMap(path);
										OnBalloonTip("File convert", "Ori convert to Map completed");
										path = path.Replace(".ori", ".map");
										LoadMap(path);
									}
									else
									{
										OnConsoleMessage($"Planner - {ori.FileName} failed to save ");
									}
								}
							}
							_oriName = null;
						}
						else
						{
							OnConsoleMessage($"The iTS is now in {mStatus?.Description}, can't stop scanning");
						}
					}
					if (scanReturn != null)
					{
						IsScanning = scanReturn.Value;
					}
				}
			}
			catch (Exception ex)
			{
				OnConsoleMessage("Error:" + ex.Message);
			}
		}

		public void OriToMap(string path)
		{
			int? maxX, maxY, minX, minY;
			maxX = null; maxY = null; minX = null; minY = null;
			var lines = File.ReadAllLines(path);
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Obstacle Points");

			foreach (string line in lines)
			{
				var data = line.Split(new char[] { ',' });
				for (int i = 3; i <= data.Length - 2; i += 2)
				{
					try
					{
						int x = Convert.ToInt32(Convert.ToDouble(data[i]));
						int y = Convert.ToInt32(Convert.ToDouble(data[i + 1]));
						sb.AppendLine($"{x.ToString()},{y.ToString()}");
						if (maxX == null) maxX = x; else if (maxX < x) maxX = x;
						if (maxY == null) maxY = y; else if (maxY < y) maxY = y;
						if (minX == null) minX = x; else if (minX > x) minX = x;
						if (minY == null) minY = y; else if (minY > y) minY = y;
					}
					catch (Exception ex)
					{
						OnConsoleMessage(ex.Message);
					}
				}
			}
			sb.AppendLine($"Minimum Position:{minX},{minY}");
			sb.AppendLine($"Maximum Position:{maxX},{maxY}");
			path = path.Replace("ori", "map");
			File.WriteAllText(path, sb.ToString());
		}


		/// <summary>
		/// 要求AGV設定新位置
		/// </summary>
		/// <param name="oldPosition">舊座標</param>
		/// <param name="newPosition">新座標</param>
		public void SetPosition(Vector2D Vector)
		{
			SetPosition Info = Send(new SetPosition(Vector)) as SetPosition;
			bool? success = Info?.Response;
			if (success == true)
			{
				//GLCMD.CMD.AddAGV(1, Vector.Start.X, Vector.Start.Y, Vector.Angle);
				//OnConsoleMessage($"iTS - The position are now at {position}");
			}
		}

		/// <summary>
		/// 將 <see cref="AGVPath"/> 轉為 <see cref="IPair"/> 集合
		/// </summary>
		private IEnumerable<IPair> PathToPairCollection(AGVPath path)
		{
			for (int ii = 0; ii < path.PathX.Count; ii++)
			{
				yield return new Pair(path.PathX[ii], path.PathY[ii]);
			}
		}

		private IEnumerable<IPair> Point2DToPairCollection(List<Point2D> points)
		{
			for (int i = 0; i < points.Count; i++)
			{
				yield return new Pair(points[i].X, points[i].Y);
			}

		}

		protected override BaseBoolReturn RequireStopAGV()
		{
			Stop Info = Send(new Stop(null)) as Stop;
			return new ConvertStop(Info);
		}

		protected override BaseBoolReturn RequireUncharge()
		{
			Uncharge Info = Send(new Uncharge(null)) as Uncharge;
			return new ConvertUncharge(Info);
		}

		public override void StopAGV()
		{
			RequireStopAGV();
		}

		public override void Uncharge()
		{
			RequireUncharge();
		}

		public override void Focus(bool isFocus)
		{
			IsFocus = isFocus;
		}

		#endregion Function - Override Methods

		#region Function - Events

		/// <summary>
		/// 资料接收事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MClient_ReceivedSerialDataEvent(object sender, ReceivedSerialDataEventArgs e)
		{
			if (e.Data is Serializable)
			{
				var product = e.Data as Serializable;
				//if (product is BaseSerialCommand cmd) Console.WriteLine($"{cmd.Type.ToString()}");
				/*-- 查詢是否有等待該封包 --*/
				Task.Run(() => ReceiveDataCheck(product));
			}
		}

		/// <summary>
		/// 连线状态变更事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MClient_ConnectStatusChangedEvent(object sender, AsyncSocket.ConnectStatusChangedEventArgs e)
		{
			switch (e.ConnectStatus)
			{
				case AsyncSocket.EConnectStatus.Connect:
					ConnectStatus = true;
					PathID = GLCMD.CMD.AddMultiStripLine("Path", null);
					LaserID = GLCMD.CMD.AddMultiPair("Laser", null);
					var status = AutoReportStatus(true);
					var laser = AutoReportLaser(true);
					var path = AutoReportPath(true);
					IsAutoReport = true;
					OnBalloonTip("Connecting", "Server IP = " + e.RemoteInfo.IP);
					break;
				case AsyncSocket.EConnectStatus.Disconnect:
					ConnectStatus = false;
					this.Status = new AGVStatus();
					GLCMD.CMD.Initial();
					IsFocus = false;
					OnBalloonTip("Disconnect", "Server IP = " + e.RemoteInfo.IP);
					break;
			}
			ConnectStatusChanged?.Invoke(sender, e);
		}
		#endregion Function - Events

	}
}
