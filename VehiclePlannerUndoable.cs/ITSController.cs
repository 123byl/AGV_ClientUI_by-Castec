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
	}

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

		private bool mConnectStatus = false;

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

		public int LaserID { get => mLaserID; set => mLaserID = value; }
		public int PathID { get => mPathID; set => mPathID = value; }
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
				var status = AutoReportStatus(isAutoReport);
				var path = AutoReportPath(isAutoReport);
				IsAutoReport = (laser?.Count ?? 0) > 0;
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
						var ori = RequestOriFile(oriName);
						if (ori != null)
						{
							System.Windows.Forms.SaveFileDialog saveOri = new System.Windows.Forms.SaveFileDialog() { InitialDirectory = @"D:\MapInfo\Client" };
							if (saveOri.ShowDialog() == System.Windows.Forms.DialogResult.OK)
							{
								if (ori.SaveAs(saveOri.FileName))
								{
									success = true;
									OnConsoleMessage($"Planner - {ori.Name} download completed");
								}
								else
								{
									success = false;
									OnConsoleMessage($"Planner - {ori.Name} failed to save");
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
				FileInfo file = new FileInfo();
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
		#endregion Function - Public Methods

		#region Function - Private Methods

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
		protected override void ClientConnect(string ip, int port) => mClient.Connect(ip, port);

		/// <summary>
		/// 停止与伺服端连线
		/// </summary>
		protected override void ClientStop() => mClient.Disconnect();


		protected override string RequestOriList()
		{
			var oriList = (Send(new RequestOriList(null)) as RequestOriList).Response;
			return oriList != null ? string.Join(",", oriList) : null;
		}

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
				Status = response as AGVStatus;
			}
			else if (response is AGVPath2D)
			{
				var path = response as AGVPath2D;
				GLCMD.CMD.SaftyEditMultiGeometry<IPair>(mPathID, true, (line) =>
				{
					line.Clear();
					line.AddRangeIfNotNull(Point2DToPairCollection(path.Points));
				});
			}
			else if (response is AGVLaser)
			{
				var laser = response as AGVLaser;
				GLCMD.CMD.SaftyEditMultiGeometry<IPair>(mLaserID, true, (point) =>
				  {
					  point.Clear();
					  point.AddRange(Point2DToPairCollection(laser.Points));
				  }
				);
			}
		}

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
			throw new NotImplementedException();
		}

		protected override BaseListReturn ImpRequestMapList()
		{
			RequestMapList Info = Send(new RequestMapList(null)) as RequestMapList;
			return new ConvertRequestMapList(Info);
		}

		protected override BaseBoolReturn UploadMapToAGV(string mapPath)
		{
			FileInfo Data = new FileInfo(mapPath);
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

		public override void StartScan(bool scan)
		{
			try
			{
				BaseBoolReturn isScanning = null;
				if (mIsScanning != scan)
				{
					if (scan)
					{//開始掃描
						if (mStatus?.Description == EDescription.Idle)
						{
							string oriName = string.Empty;
							if (InputBox.Invoke(out oriName, "MAP Name", "Set Map File Name"))
							{
								isScanning = SetScanningOriFileName(oriName);
							}
							if (isScanning.Requited && isScanning.Value)
							{
								OnConsoleMessage($"iTS - The new ori name is {oriName}.ori");
							}
						}
						else
						{
							OnConsoleMessage($"The iTS is now in {mStatus?.Description}, can't start scanning");
						}
					}
					else
					{//停止掃描
						if (true || mStatus?.Description == EDescription.Map)
						{
							isScanning = StopScanning();
						}
						else
						{
							OnConsoleMessage($"The iTS is now in {mStatus?.Description}, can't stop scanning");
						}
					}
					if (isScanning != null)
					{
						IsScanning = isScanning.Value;
					}
				}
			}
			catch (Exception ex)
			{
				OnConsoleMessage("Error:" + ex.Message);
			}
		}

		/// <summary>
		/// 要求自動回傳雷射資料
		/// </summary>
		/// <param name="on"></param>
		/// <returns></returns>
		protected List<Point2D> AutoReportLaser(bool on)
		{
			AutoReportLaser Info = (AutoReportLaser)Send(new AutoReportLaser(on));
			List<Point2D> laser = Info?.Response.Points;
			return laser;
		}

		protected AGVStatus AutoReportStatus(bool on)
		{
			AutoReportStatus Info = (AutoReportStatus)Send(new AutoReportStatus(on));
			AGVStatus status = Info?.Response;
			return status;
		}

		protected List<Point2D> AutoReportPath(bool on)
		{
			AutoReportPath Info = (AutoReportPath)Send(new AutoReportPath(on));
			List<Point2D> path = Info.Response.Points;
			return path;
		}

		protected BaseFileReturn GetParameter()
		{
			GetIni Info = Send(new GetIni(null)) as GetIni;
			return new ConvertGetIni(Info);
		}

		protected BaseBoolReturn SetParameter(FileInfo Ini)
		{
			SetIni Info = Send(new SetIni(Ini)) as SetIni;
			return new ConvertSetIni(Info);
		}

		/// <summary>
		/// 要求AGV設定新位置
		/// </summary>
		/// <param name="oldPosition">舊座標</param>
		/// <param name="newPosition">新座標</param>
		public void SetPosition(Vector2D Vector)
		{
			SetPosition Info = Send(new SetPosition(Vector)) as SetPosition;
			bool success = Info.Response;
			if (success == true)
			{
				GLCMD.CMD.AddAGV(1, Vector.Start.X, Vector.Start.Y, Vector.Angle);
				//OnConsoleMessage($"iTS - The position are now at {position}");
			}
			GLCMD.CMD.AddAGV(1, Vector.Start.X, Vector.Start.Y, Vector.Angle);
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

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="oriName"></param>
		/// <returns></returns>
		protected FileInfo RequestOriFile(string oriName)
		{
			var oriFile = (Send(new GetOri(oriName)) as GetOri).Response;
			return oriFile;
		}

		#endregion Function - Private Methods

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
				/*-- 查詢是否有等待該封包 --*/
				ReceiveDataCheck(product);
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
					var status = AutoReportStatus(true);
					var laser = AutoReportLaser(true);
					var path = AutoReportPath(true);
					IsAutoReport = true;
					OnBalloonTip("Connecting", "Server IP = " + e.RemoteInfo.IP);
					break;
				case AsyncSocket.EConnectStatus.Disconnect:
					ConnectStatus = false;
					GLCMD.CMD.DeleteAGV(1);
					GLCMD.CMD.SaftyEditMultiGeometry<IPair>(mLaserID, true, (point) => { point.Clear(); });
					GLCMD.CMD.SaftyEditMultiGeometry<IPair>(mPathID, true, (line) => { line.Clear(); });
					this.Status = new AGVStatus();
					OnBalloonTip("Disconnect", "Server IP = " + e.RemoteInfo.IP);
					break;
			}
		}

		#endregion Function - Events

	}
}
