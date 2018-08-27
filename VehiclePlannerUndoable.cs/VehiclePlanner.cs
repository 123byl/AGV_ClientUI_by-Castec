using GLCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VehiclePlanner;
using VehiclePlanner.Core;
using VehiclePlanner.Module.Implement;
using WeifenLuo.WinFormsUI.Docking;
using SerialData;
using Geometry;
using CtBind;
using System.IO;
using System.Threading;
using VehiclePlanner.Module;
using CtItsParameter;
using CtLib.Library;
using CtExtendLib;
using AsyncSocket;
using VehiclePlannerUndoable.cs.Properties;

namespace VehiclePlannerUndoable.cs
{

	/// <summary>
	/// 介面實作
	/// </summary>
	public partial class CtVehiclePlanner_Ctrl : BaseVehiclePlanner_Ctrl
	{
		#region Declaration - Field
		///// <summary>
		///// Car Position 設定位置
		///// </summary>
		//private Point2D localizePosition = null;
		#endregion

		#region Declaration - Properties
		public IVehiclePlanner rVehiclePlanner = null;

		private IMapGL MapGL { get => mDockContent[miMapGL] as IMapGL; }

		private IGoalSetting GoalSetting { get => mDockContent[miGoalSetting] as IGoalSetting; }

		public bool IsCharge { get; set; } = false;
		#endregion

		#region Function - Constructors 
		public CtVehiclePlanner_Ctrl(IVehiclePlanner vehiclePlanner) : base()
		{
			InitializeComponent();

			rVehiclePlanner = vehiclePlanner;

			Initial(vehiclePlanner);

			Bindings(rVehiclePlanner);

			Bindings(rVehiclePlanner.Controller);

		}
		#endregion

		#region Function- Public Methods
		public void Bindings(IVehiclePlanner source)
		{
			Bindings<IVehiclePlanner>(source);
		}

		public void Bindings(IITSController_Undoable source)
		{
			Bindings<IITSController_Undoable>(source);
			string dataMember = nameof(source.Status);
			tsprgBattery.ProgressBar.DataBindings.ExAdd(nameof(ProgressBar.Value), source, dataMember, (sender, e) => { e.Value = (int)(e.Value as AGVStatus).Battery; });
			tslbBattery.DataBindings.ExAdd(nameof(tslbBattery.Text), source, dataMember, (sender, e) => { e.Value = $"{(e.Value as AGVStatus).Battery}%"; });
			tslbStatus.DataBindings.ExAdd(nameof(tslbStatus.Text), source, dataMember, (sender, e) => { e.Value = (e.Value as AGVStatus).Description.ToString(); });
			tslbConnect.DataBindings.ExAdd(nameof(tslbConnect.Image), source, nameof(source.ConnectStatus), (sender, e) => { e.Value = (bool)e.Value ? Properties.Resources.LED_L_Green : Properties.Resources.LED_L_Red; });
		}

		public void CancelLocalize()
		{
			//localizePosition = null;
			//GLCMD.CMD.DeleteAGV(2);
			GLCMD.CMD.CancelLocalize();
		}
		#endregion

		#region Function - Override Methods
		protected override void Initial(IBaseVehiclePlanner vehiclePlanner)
		{
			base.Initial(vehiclePlanner);
			base.WindowState = FormWindowState.Maximized;
			miLogin.Visible = false;
			miBypass.Visible = false;
			miUserManager.Visible = false;
			miTest.Visible = false;
			rVehiclePlanner.Controller.ShowMotionController += Controller_ShowMotionController;
			rVehiclePlanner.Controller.CloseMotionController += Controller_CloseMotionController;
			rVehiclePlanner.Controller.OpenMap += Controller_LoadMapEvent;
			rVehiclePlanner.Controller.ConnectStatusChanged += Controller_ConnectStatusChanged;
			rVehiclePlanner.Controller.ChargeChange += Controller_ChargeChange;
			GoalSetting.RefMapControl = MapGL.MapControl;
		}

		protected override BaseMapGL GetMapGL(DockState dockState)
		{
			return new MapGL(this, dockState);
		}

		protected override BaseGoalSetting GetGoalSetting(DockState dockState)
		{
			return new GoalSetting(this, dockState);
		}

		protected override AuthorityDockContainer GetParameterSetting(DockState dockState)
		{
			return new ItsParameterCtrl(this, dockState);
		}

		protected override void SetEvents()
		{
			base.SetEvents();
			MapGL.MapControl.GLClick += MapControl_GLClick;
		}

		protected override void MarkerChanged()
		{
		}
		public override void LoadFile(FileType type)
		{
			OpenFileDialog openMap = new OpenFileDialog()
			{
				InitialDirectory = rVehiclePlanner.DefMapDir,
				Filter = $"MAP|*.{type.ToString().ToLower()}|Ori|*.ori"
			};
			if (openMap.ShowDialog() == DialogResult.OK)
			{
				string path = openMap.FileName;
				if (Path.GetExtension(path) == ".ori")
				{
					(rVehiclePlanner.Controller as ITSController).OriToMap(path);
					path = path.Replace("ori", "map");
				}
				Task.Run(() =>
				{
					(GoalSetting as GoalSetting).InvokeIfNecessaryDgv(() => rVehiclePlanner.LoadFile(type, path));
					MapGL.MapControl.AdjustZoom();
					MapGL.MapControl.Focus(GLCMD.CMD.MapCenter.X, GLCMD.CMD.MapCenter.Y);

				});
			}
		}

		protected override void InsertMap()
		{
			MapGL.MapControl.JoinMap();
		}

		public override void DownloadParameter()
		{
			rVehiclePlanner.RequireIni();
		}

		public override void UploadParameter()
		{
			rVehiclePlanner.UploadIni();
		}

		protected override void RunLoop(List<string> goals)
		{
			rVehiclePlanner.RunLoop(goals);
		}

		protected override void StopRunLoop()
		{
			rVehiclePlanner.StopRunLoop();
			OnBalloonTip("Information", "Stop run loop");
		}

		protected override string SelectFile(string fileList)
		{
			string fileName = null;
			var fileNames = fileList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			DataTable table = new DataTable();
			table.Columns.Add("Name");
			foreach (string name in fileNames)
			{
				DataRow row = table.NewRow();
				row["Name"] = name;
				table.Rows.Add(row);
			}
			SelectBox selectBox = null;
			using (selectBox = new SelectBox("Select File", table, "Determine"))
			{
				var result = this.InvokeIfNecessary(() => selectBox.ShowDialog(this));
				fileName = selectBox.SelectRow?["Name"].ToString();
			}
			return fileName;
		}

		public override void ITest_SettingCarPos()
		{
			mIsSetting = !mIsSetting;
		}
		#endregion

		#region Function - Private Methods
		private Point2D ToPoint2D(IPair Point)
		{
			return new Point2D(Point.X, Point.Y);
		}
		#endregion

		#region Function - Events
		protected void MapControl_GLClick(object sender, EventArgs e)
		{
			MouseEventArgs m = (MouseEventArgs)e;
			if (mIsSetting)
			{
				if (!GLCMD.CMD.IsLocalize)
				{
					OnBalloonTip("Localization", "Set iTS Position");
					//localizePosition = ToPoint2D(MapGL.MapControl.ScreenToGL(m.X, m.Y));
					//GLCMD.CMD.AddAGV(2, "Localization", localizePosition.X, localizePosition.Y, 0);
					GLCMD.CMD.StartLocalize(MapGL.MapControl.ScreenToGL(m.X, m.Y));
				}
				else
				{
					OnBalloonTip("Localization", "Set iTS Vector");
					//OnConsoleMessage($"NewPos{localizePosition.ToString()}");
					var toword = GLCMD.CMD.FinishLocalize(MapGL.MapControl.ScreenToGL(m.X, m.Y));
					Vector2D V = new Vector2D(ToPoint2D(toword.Position), ToPoint2D(MapGL.MapControl.ScreenToGL(m.X, m.Y)));
					//GLCMD.CMD.AddAGV(2, "Localization",V.Start.X,V.Start.Y,V.Angle);
					Task.Run(() => rVehiclePlanner.Controller.SetPosition(V));
					//localizePosition = null;
					
					//GLCMD.CMD.DeleteAGV(2);

				}
			}
		}
		private void Controller_ShowMotionController(object sender, EventArgs e)
		{
			this.InvokeIfNecessary(this.ShowMotionController);
		}
		private void Controller_CloseMotionController(object sender, EventArgs e)
		{
			this.InvokeIfNecessary(this.CloseMotionController);
		}

		private void Controller_ConnectStatusChanged(object sender, ConnectStatusChangedEventArgs e)
		{
			var enable = e.ConnectStatus == EConnectStatus.Connect ? true : false;
			MapGL.ConnectButtonEnable(enable);
			GoalSetting.ConnectButtonEnable(enable);
			this.InvokeIfNecessary(() => tslbConnect.Image = enable ? Resources.LED_L_Green : Resources.LED_L_Red);
		}

		private void Controller_ChargeChange(bool value)
		{
			IsCharge = true;
			GoalSetting.ChargeButtonImage(IsCharge);
		}

		private void Controller_LoadMapEvent(string path)
		{
			GLCMD.CMD.Initial();
			GLCMD.CMD.LoadMap(path);
			MapGL.MapControl.AdjustZoom();
			MapGL.MapControl.Focus(GLCMD.CMD.MapCenter.X, GLCMD.CMD.MapCenter.Y);
			rVehiclePlanner.Controller.PathID = GLCMD.CMD.AddMultiStripLine("Path", null);
			rVehiclePlanner.Controller.LaserID = GLCMD.CMD.AddMultiPair("Laser", null);
		}
		#endregion
	}


	/// <summary>
	/// 底層介面定義
	/// </summary>
	public interface IVehiclePlanner : IBaseVehiclePlanner
	{
		new IITSController_Undoable Controller { get; }

		bool IsRunLoop { get; set; }
		/// <summary>
		/// 取得ITS Ini
		/// </summary>
		void RequireIni();
		/// <summary>
		/// 上傳ITS Ini
		/// </summary>
		void UploadIni();


		void RunLoop(List<string> goals);

		void StopRunLoop();
	}

	/// <summary>
	/// 底層實作
	/// </summary>
	public class VehiclePlanner : BaseVehiclePlanner, IVehiclePlanner
	{
		#region Declaration - Field
		public new IITSController_Undoable Controller { get { return base.Controller as IITSController_Undoable; } }

		public bool IsRunLoop { get; set; }
		#endregion

		#region Declaration - Properties
		#endregion

		#region Function - Constructors
		public VehiclePlanner() : base()
		{
			base.Controller = new ITSController();
		}

		#endregion

		#region Function - Override Methods
		public override void AddCurrentAsGoal()
		{
			var s = Controller.Status;
			if (Controller.ConnectStatus && (s.Description == EDescription.Arrived || s.Description == EDescription.Idle))
			{
				GLCMD.CMD.DoAddSingleTowardPair("General", s.X, s.Y, s.Toward);
			}
			else
			{
				SetBalloonTip("Set Goal Warnning", "Please check iTS status whether is Arrived or Idle");
				OnConsoleMessage("Set iTS current position to goal error,please check iTS status whether is Arrived or Idle");
			}
		}

		public override void DeleteMarker(IEnumerable<uint> markers)
		{
			if (markers != null && (markers as List<uint>).Count > 0)
			{
				foreach (uint index in markers)
					GLCMD.CMD.DoDelete(Convert.ToInt32(index));
			}
		}

		protected override void SaveMap(string path)
		{
			GLCMD.CMD.SaveMap(path);
		}
		public override void ClearMap()
		{
			GLCMD.CMD.Initial();
			Controller.PathID = GLCMD.CMD.AddMultiStripLine("Path", null);
			Controller.LaserID = GLCMD.CMD.AddMultiPair("Laser", null);
		}
		public override void LoadFile(FileType type, string fileName)
		{
			try
			{
				bool check = false;
				check = File.Exists(fileName);
				if (check)
				{
					GLCMD.CMD.Initial();
					GLCMD.CMD.LoadMap(fileName);

					/*載入地圖會清除原始資料，重新載入Laser Path繪製*/
					Controller.PathID = GLCMD.CMD.AddMultiStripLine("Path", null);
					Controller.LaserID = GLCMD.CMD.AddMultiPair("Laser", null);

					mCurMapPath = fileName;
					SetBalloonTip($"Load { type}", $"\'{fileName}\' is loaded");
					//if (Controller.IsConnected && type == FileType.Map)
					//{
					//	Controller.SendAndSetMap(fileName);
					//}
				}
				else
				{
					OnErrorMessage("File data is wrong, can not read");
				}
			}
			catch (Exception ex)
			{
				OnErrorMessage(ex.Message);
			}
		}
		#endregion

		#region Funtion - Public Methods
		public void RequireIni()
		{
			Controller.RequireIni();
		}
		public void UploadIni()
		{
			Controller.UploadIni();
		}

		public void RunLoop(List<string> goals)
		{
			if (IsRunLoop) IsRunLoop = false; Thread.Sleep(100);
			if (goals?.Count > 0)
			{
				IsRunLoop = true;
				SetBalloonTip("Information", "Start run loop");
				Task.Run(() =>
					{
						int i = 0;
						do
						{
							if (i >= goals.Count) i = 0;
							string goal = goals[i];
							if (Controller.Status.Description == EDescription.Idle || Controller.Status.Description == EDescription.Arrived)
							{
								Controller.GoTo(goal);
								i++;
								do
								{
									Thread.Sleep(1000);
								} while (Controller.Status.Description != EDescription.Running);
							}
							Thread.Sleep(50);
						} while (IsRunLoop);
					});
			}
		}

		public void StopRunLoop()
		{
			IsRunLoop = false;
			Task.Run(()=>Controller.StopAGV());
		}
		#endregion

		#region Function - Private Methods
		#endregion

		#region Function - Events
		#endregion

	}
}


