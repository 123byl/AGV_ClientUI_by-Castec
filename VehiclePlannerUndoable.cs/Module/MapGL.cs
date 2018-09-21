using CtLib.Library;
using GLCore;
using GLStyle;
using GLUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VehiclePlanner.Module.Implement;
using VehiclePlannerUndoable.cs.Properties;
using WeifenLuo.WinFormsUI.Docking;


namespace VehiclePlannerUndoable.cs
{

	/// <summary>
	/// 以可重做MapGL進行實作之MapGL介面
	/// </summary>
	public partial class MapGL : BaseMapGL, IMapGL
	{
		#region Declaration - Fields
		private enum EMotion
		{
			Location,
			Movement,
			Disconnect
		}

		/// <summary>
		/// Map sub UI
		/// </summary>
		private GLUICtrl mMapGL = new GLUICtrl();

		#endregion

		#region Declaration - Properties
		/// <summary>
		/// 父視窗
		/// </summary>
		private CtVehiclePlanner_Ctrl ParrentUI { get => rUI as CtVehiclePlanner_Ctrl; }

		/// <summary>
		/// 地圖控制項
		/// </summary>
		public GLUICtrl MapControl { get => mMapGL; }

		#endregion


		protected MapGL()
		{
			InitializeComponent();
			mMapGL.SetEditMode(true);
			tsbGetLaser.Visible = false;
		}


		public MapGL(CtVehiclePlanner_Ctrl refUI, DockState defState = DockState.Float) : base(refUI, defState)
		{
			InitializeComponent();

			// 載入設定檔
			mMapGL.SetEditMode(true);
			StyleManager.LoadStyle("Style.ini");
			tsbGetLaser.Visible = false;
			tsbMove.Visible = false;
			ConnectButtonEnable(false);
			mMapGL.Location = new Point(0, 0);
			mMapGL.Dock = DockStyle.Fill;
			pnlShow.Controls.Add(mMapGL);
		}

		public void ConnectButtonEnable(bool enable)
		{
			this.InvokeIfNecessary(() =>
			{
				tsbAutoReport.Enabled = enable;
				tsbChangeMap.Enabled = enable;
				tsbConfirm.Enabled = enable;
				tsbController.Enabled = enable;
				tsbGetLaser.Enabled = enable;
				tsbGetMap.Enabled = enable;
				tsbLocalization.Enabled = enable;
				if (!enable) tsbLocalization.Checked = enable;
				tsbScan.Enabled = enable;
				tsbSendMap.Enabled = enable;
				tsbMove.Enabled = enable;
				if (!enable) tsbMove.Checked = enable;
				tsbFocus.Enabled = enable;
				if (!enable) tsbFocus.Checked = enable;
				tsbConnect.Image = enable ? Resources.Connect : Resources.Disconnect;
			});
		}

		protected override void tsbSetCar_Click(object sender, EventArgs e)
		{
			MapControl.CancelSelect();
			ButtonControlEnable(!tsbLocalization.Checked, EMotion.Location);
			if (!tsbLocalization.Checked) ParrentUI.CancelLocalize();
			MapControl.SetEditMode(!tsbLocalization.Checked);
			ParrentUI.Locate();
		}

		protected override void tsbMove_Click(object sender, EventArgs e)
		{
			MapControl.CancelSelect();
			ButtonControlEnable(!tsbMove.Checked, EMotion.Movement);
			if (!tsbMove.Checked) ParrentUI.CancelMovement();
			MapControl.SetEditMode(!tsbMove.Checked);
			ParrentUI.MovePosition();
		}

		public void MovePositionFinish()
		{
			ButtonControlEnable(!tsbMove.Checked, EMotion.Movement);
			MapControl.SetEditMode(true);
		}

		protected override void tsbFocus_Click(object sender, EventArgs e)
		{
			base.tsbFocus_Click(sender, e);
			tsbFocus.Checked = ParrentUI.rVehiclePlanner.Controller.IsFocus;
		}

		private void ButtonControlEnable(bool enable, EMotion type)
		{
			//enable = ParrentUI.rVehiclePlanner.Controller.ConnectStatus ? enable : true;
			//type = ParrentUI.rVehiclePlanner.Controller.ConnectStatus ? type : EMotion.Disconnect;
			tsbOpenFile.Enabled = !enable;
			tsbSave.Enabled = !enable;
			tsbClearMap.Enabled = !enable;
			tsbConnect.Enabled = !enable;
			tsbInsertMap.Enabled = !enable;
			tsbGetMap.Enabled = !enable;
			tsbSendMap.Enabled = !enable;
			tsbChangeMap.Enabled = !enable;
			tsbScan.Enabled = !enable;
			tsbController.Enabled = !enable;
			tsbAutoReport.Enabled = !enable;
			tsbConfirm.Enabled = !enable;
			switch (type)
			{
				case EMotion.Location:
					tsbLocalization.Checked = enable;
					tsbMove.Enabled = enable ? false : !enable;
					break;
				case EMotion.Movement:
					tsbLocalization.Enabled = enable ? false : !enable;
					tsbMove.Checked = enable;
					break;
				case EMotion.Disconnect:
					tsbLocalization.Checked = enable;
					tsbMove.Checked = enable;
					break;
			}
		}
	}
}
