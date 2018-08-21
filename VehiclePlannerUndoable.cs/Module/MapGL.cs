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
using WeifenLuo.WinFormsUI.Docking;


namespace VehiclePlannerUndoable.cs
{

	/// <summary>
	/// 以可重做MapGL進行實作之MapGL介面
	/// </summary>
	public partial class MapGL : BaseMapGL, IMapGL
	{

		private GLUICtrl mMapGL = new GLUICtrl();

		private CtVehiclePlanner_Ctrl ParrentUI { get => rUI as CtVehiclePlanner_Ctrl; }

		/// <summary>
		/// 地圖控制項
		/// </summary>
		public GLUICtrl MapControl { get => mMapGL; }

		protected MapGL()
		{
			InitializeComponent();
			tsbGetLaser.Visible = false;
		}
		public MapGL(CtVehiclePlanner_Ctrl refUI, DockState defState = DockState.Float) : base(refUI, defState)
		{
			InitializeComponent();

			// 載入設定檔
			StyleManager.LoadStyle("Style.ini");
			tsbGetLaser.Visible = false;
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
			});
		}

		protected override void tsbSetCar_Click(object sender, EventArgs e)
		{
			ParrentUI.ITest_SettingCarPos();
			tsbLocalization.Checked = !tsbLocalization.Checked;
			if (tsbLocalization.Checked)
			{
				tsbAutoReport.Enabled = false;
				tsbChangeMap.Enabled = false;
				tsbClearMap.Enabled = false;
				tsbConfirm.Enabled = false;
				tsbConnect.Enabled = false;
				tsbController.Enabled = false;
				tsbGetLaser.Enabled = false;
				tsbGetMap.Enabled = false;
				tsbInsertMap.Enabled = false;
				tsbOpenFile.Enabled = false;
				tsbSave.Enabled = false;
				tsbScan.Enabled = false;
				tsbSendMap.Enabled = false;
			}
			else if(!tsbLocalization.Checked && ParrentUI.rVehiclePlanner.Controller.ConnectStatus)
			{
				tsbAutoReport.Enabled = true;
				tsbChangeMap.Enabled = true;
				tsbClearMap.Enabled = true;
				tsbConfirm.Enabled = true;
				tsbConnect.Enabled = true;
				tsbController.Enabled = true;
				tsbGetLaser.Enabled = true;
				tsbGetMap.Enabled = true;
				tsbInsertMap.Enabled = true;
				tsbOpenFile.Enabled = true;
				tsbSave.Enabled = true;
				tsbScan.Enabled = true;
				tsbSendMap.Enabled = true;
				ParrentUI.CancelLocalize();
			}
			else
			{
				tsbClearMap.Enabled = true;
				tsbConnect.Enabled = true;
				tsbInsertMap.Enabled = true;
				tsbOpenFile.Enabled = true;
				tsbSave.Enabled = true;
				ParrentUI.CancelLocalize();
			}
		}
	}
}
