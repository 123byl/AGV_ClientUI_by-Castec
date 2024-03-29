﻿using CtDockSuit;
using CtLib.Library;
using System;
using System.Drawing;
using System.Windows.Forms;
using VehiclePlanner.Core;
using VehiclePlanner.Module.Interface;
using WeifenLuo.WinFormsUI.Docking;
using CtLib.Module.Utility; 
using System.Threading.Tasks;
using CtBind;

namespace VehiclePlanner.Module.Implement
{

	/// <summary>
	/// 地圖顯示基類
	/// </summary>
	public partial class BaseMapGL : AuthorityDockContainer, IBaseMapGL
	{

		#region Declaration  - Fields

		#endregion Declaration  - Fields

		#region Declaration - Properties

		public override DockState DockState
		{
			get
			{
				return pnlHide.Visible ? DockState.Hidden : DockState.Document;
			}

			set
			{
				pnlHide.Visible = value != DockState.Document;
				DockStateChanged?.Invoke(this, new EventArgs());
			}
		}

		#endregion Declaration - Properties

		#region Declaration - Events

		/// <summary>
		/// 停靠狀態變更事件
		/// </summary>
		public override event EventHandler DockStateChanged;

		#endregion Declaration - Events

		#region Function - Constructors

		/// <summary>
		/// 給介面設計師使用的建構式，拿掉後繼承該類的衍生類將無法顯示介面設計
		/// </summary>
		protected BaseMapGL() : base()
		{
			InitializeComponent();
		}

		/// <summary>
		/// 共用建構方法
		/// </summary>
		public BaseMapGL(BaseVehiclePlanner_Ctrl refUI, DockState defState = DockState.Float)
			: base(refUI, defState)
		{
			InitializeComponent();
			FixedSize = new Size(718, 814);
		}

		#endregion Function - Constructors

		#region Function - Events

		/// <summary>
		/// 載入地圖檔
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbOpenFile_Click(object sender, EventArgs e)
		{
			rUI.LoadMap();
		}

		/// <summary>
		/// 清除地圖
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbClearMap_Click(object sender, EventArgs e)
		{
			rUI.ClearMap();
		}

		/// <summary>
		/// 地圖掃描
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbScan_Click(object sender, EventArgs e)
		{
			rUI.StartScan();
		}

		/// <summary>
		/// 開啟移動控制器
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbController_Click(object sender, EventArgs e)
		{
			rUI.ShowMotionController();
		}

		/// <summary>
		/// 切換iTS狀態回傳開關
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbCar_Click(object sender, EventArgs e)
		{
			rUI.AutoReport();
		}

		/// <summary>
		/// 設定iTS當前位置
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void tsbSetCar_Click(object sender, EventArgs e)
		{
			rUI.Locate();
		}

		/// <summary>
		/// 微調iTS位置
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbConfirm_Click(object sender, EventArgs e)
		{
			rUI.Confirm();
		}

		/// <summary>
		/// 取得雷射資料，測試雷射用
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbGetLaser_Click(object sender, EventArgs e)
		{
			rUI.GetLaser();
		}


		/// <summary>
		/// 與iTS連線
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbConnect_Click(object sender, EventArgs e)
		{
			rUI.Connect();
		}

		/// <summary>
		/// 取得Map檔
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbGetMap_Click(object sender, EventArgs e)
		{
			rUI.DownloadMap();
		}

		/// <summary>
		/// 傳送Map檔
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbSendMap_Click(object sender, EventArgs e)
		{
			rUI.UploadMap();
		}

		/// <summary>
		/// 地圖匯入
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbInsertMap_Click(object sender, EventArgs e)
		{
			rUI.InsertMap();
		}

		private void tsbChangeMap_Click(object sender, EventArgs e)
		{
			rUI.ChangeMap();
		}

		private void tsbSave_Click(object sender, EventArgs e)
		{
			rUI.SaveMap();
		}

		protected virtual void tsbMove_Click(object sender, EventArgs e)
		{
			rUI.MovePosition();
		}

		protected virtual void tsbFocus_Click(object sender, EventArgs e)
		{
			rUI.SetFocus();
		}
		#endregion Function - Events

		#region Funciton - Public Methods

		public override bool IsVisiable(AccessLevel lv)
		{
			return lv > AccessLevel.None;
		}

		#endregion Funciton - Public Methods

		#region Function - Private Methods

		/// <summary>
		/// 顯示視窗
		/// </summary>
		protected override void ShowWindow()
		{
			base.ShowWindow();
			DockState = DockState.Document;
		}

		/// <summary>
		/// 隱藏視窗
		/// </summary>
		protected override void HideWindow()
		{
			DockState = DockState.Hidden;
		}

		/// <summary>
		/// 取消視窗關閉
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			e.Cancel = true;
		}

		/// <summary>
		/// 快捷鍵
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			bool ret = true;
			switch (keyData)
			{
				case Keys.O | Keys.Control:
					rUI.LoadMap();
					break;
				case Keys.E | Keys.Control:
					rUI.ClearMap();
					break;
				case Keys.C | Keys.Control:
					rUI.Connect();
					break;
				case Keys.D | Keys.Control:
					rUI.DownloadMap();
					break;
				case Keys.U | Keys.Control:
					rUI.UploadMap();
					break;
				case Keys.S | Keys.Control:
					rUI.SaveMap();
					break;
				case Keys.M | Keys.Control:
					rUI.ShowMotionController();
					break;
				case Keys.G | Keys.Control:
					rUI.AutoReport();
					break;
				case Keys.P | Keys.Control:
					rUI.Locate();
					break;
				case Keys.F | Keys.Control:
					rUI.Confirm();
					break;
				case Keys.L | Keys.Control:
					rUI.GetLaser();
					break;
				default:
					ret = base.ProcessCmdKey(ref msg, keyData);
					break;
			}
			return ret;
		}

		#endregion Function - Private Methods

		#region Implement - IMapGL

		#endregion Implement - IMapGL

		#region Implement - IDataDisplay<ICtVehiclePlanner>

		/// <summary>
		/// 資料綁定
		/// </summary>
		/// <param name="source"></param>
		public virtual void Bindings(IBaseITSController source)
		{
			if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);

			tsbConnect.DataBindings.ExAdd(nameof(tsbConnect.Image), source, nameof(source.IsConnected), (sender, e) =>
			{
				e.Value = (bool)e.Value ? Properties.Resources.Connect : Properties.Resources.Disconnect;
			});
			//tsbGetMap.DataBindings.ExAdd(nameof(tsbGetMap.Enabled), source, nameof(source.IsConnected), (sender, e) =>
			//{
			//	e.Value = (bool)e.Value ? true : false;
			//});
			//tsbSendMap.DataBindings.ExAdd(nameof(tsbSendMap.Enabled), source, nameof(source.IsConnected), (sender, e) =>
			//{
			//	e.Value = (bool)e.Value ? true : false;
			//});
			//tsbChangeMap.DataBindings.ExAdd(nameof(tsbChangeMap.Enabled), source, nameof(source.IsConnected), (sender, e) =>
			//{
			//	e.Value = (bool)e.Value ? true : false;
			//});
			//tsbScan.DataBindings.ExAdd(nameof(tsbScan.Enabled), source, nameof(source.IsConnected), (sender, e) =>
			//{
			//	e.Value = (bool)e.Value ? true : false;
			//});
			//tsbController.DataBindings.ExAdd(nameof(tsbController.Enabled), source, nameof(source.IsConnected), (sender, e) =>
			//{
			//	e.Value = (bool)e.Value ? true : false;
			//});
			//tsbAutoReport.DataBindings.ExAdd(nameof(tsbAutoReport.Enabled), source, nameof(source.IsConnected), (sender, e) =>
			//{
			//	e.Value = (bool)e.Value ? true : false;
			//});
			//tsbLocalization.DataBindings.ExAdd(nameof(tsbLocalization.Enabled), source, nameof(source.IsConnected), (sender, e) =>
			//{
			//	e.Value = (bool)e.Value ? true : false;
			//});
			//tsbConfirm.DataBindings.ExAdd(nameof(tsbConfirm.Enabled), source, nameof(source.IsConnected), (sender, e) =>
			//{
			//	e.Value = (bool)e.Value ? true : false;
			//});

		}

		/// <summary>
		/// 資料綁定
		/// </summary>
		/// <param name="source"></param>
		public virtual void Bindings(IBaseVehiclePlanner source)
		{
			if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);

		}


		#endregion Implement - IDataDisplay<ICtVehiclePlanner>
	}

}