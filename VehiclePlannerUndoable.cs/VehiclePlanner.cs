﻿using GLCore;
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

namespace VehiclePlannerUndoable.cs
{

	/// <summary>
	/// 介面實作
	/// </summary>
	public partial class CtVehiclePlanner_Ctrl : BaseVehiclePlanner_Ctrl
	{
		#region Declaration - Field
		/// <summary>
		/// Car Position 設定位置
		/// </summary>
		private Point2D mNewPos = null;
		#endregion

		#region Declaration - Properties
		private IVehiclePlanner rVehiclePlanner = null;

		private IMapGL MapGL { get => mDockContent[miMapGL] as IMapGL; }

		private IGoalSetting GoalSetting { get => mDockContent[miGoalSetting] as IGoalSetting; }
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
			tsprgBattery.ProgressBar.DataBindings.ExAdd(nameof(ProgressBar.Value), source, dataMember, (sender, e) => { e.Value =(int)(e.Value as AGVStatus).Battery; });
			tslbBattery.DataBindings.ExAdd(nameof(tslbBattery.Text), source, dataMember, (sender, e) => { e.Value = $"{(e.Value as AGVStatus).Battery}%"; });
			tslbStatus.DataBindings.ExAdd(nameof(tslbStatus.Text), source, dataMember, (sender, e) => { e.Value = (e.Value as AGVStatus).Description.ToString(); });
		}
		#endregion

		#region Function - Private Methods

		protected override void Initial(IBaseVehiclePlanner vehiclePlanner)
		{
			base.Initial(vehiclePlanner);
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

		protected override void SetEvents()
		{
			base.SetEvents();
			MapGL.MapControl.GLClick += MapControl_GLClick;
		}

		protected override void MarkerChanged()
		{
		}

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
				if (mNewPos == null)
				{
					mNewPos = ToPoint2D(MapGL.MapControl.ScreenToGL(m.X, m.Y));
				}
				else
				{
					OnConsoleMessage($"NewPos{mNewPos.ToString()}");
					Vector2D V = new Vector2D(mNewPos, ToPoint2D(MapGL.MapControl.ScreenToGL(m.X, m.Y)));
					Task.Run(() =>
					{
						rVehiclePlanner.Controller.SetPosition(V);
						mNewPos = null;
						mIsSetting = false;
					});
				}
			}
		}
		#endregion
	}


	/// <summary>
	/// 底層介面定義
	/// </summary>
	public interface IVehiclePlanner : IBaseVehiclePlanner
	{
		new IITSController_Undoable Controller { get; }
	}

	/// <summary>
	/// 底層實作
	/// </summary>
	public class VehiclePlanner : BaseVehiclePlanner, IVehiclePlanner
	{
		#region Declaration - Field
		public new IITSController_Undoable Controller { get { return base.Controller as IITSController_Undoable; } }
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
		protected override void SaveMap(string path)
		{
			GLCMD.CMD.SaveMap(path);
		}
		public override void ClearMap()
		{
			GLCMD.CMD.Initial();
		}
		public override void LoadFile(FileType type, string fileName)
		{
			try
			{
				bool check = false;
				check = File.Exists(fileName);
				if (check)
				{
					GLCMD.CMD.LoadMap(fileName);

					/*載入地圖會清除原始資料，重新載入Laser Path繪製*/
					Controller.PathID = GLCMD.CMD.AddMultiStripLine("Path", null);
					Controller.LaserID = GLCMD.CMD.AddMultiPair("Laser", null);

					mCurMapPath = fileName;
					SetBalloonTip($"Load { type}", $"\'{fileName}\' is loaded");
					if (Controller.IsConnected && type == FileType.Map)
					{
						Controller.SendAndSetMap(fileName);
					}
				}
				else
				{
					OnErrorMessage("File data is wrong, can not read");
				}
			}
			catch(Exception ex)
			{
				OnErrorMessage(ex.Message);
			}
		}
		#endregion

		#region Funtion - Public Methods
		#endregion

		#region Function - Private Methods
		#endregion

		#region Function - Events
		#endregion

	}
}


