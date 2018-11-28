using CtBind;
using CtLib.Library;
using Geometry;
using GLCore;
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
using CtExtendLib;
using VehiclePlannerUndoable.cs.Properties;

namespace VehiclePlannerUndoable.cs
{

	/// <summary>
	/// 以UndoableMapGL實作之GoalSetting介面
	/// </summary>
	public partial class GoalSetting : BaseGoalSetting, IGoalSetting
	{

		#region Declaration - Fields

		/// <summary>
		/// MapGL控制項參考
		/// </summary>
		private GLUICtrl rMapGL = null;

		/// <summary>
		/// 標示物資訊物件
		/// </summary>
		private ISingleInfo mSingleInfo = null;

		/// <summary>
		/// 目前選取的欄位指標
		/// </summary>
		private int mCurrentIndex = -1;
		#endregion Declaration - Fields

		#region Declaration - Properties
		private CtVehiclePlanner_Ctrl parentUI { get => (rUI as CtVehiclePlanner_Ctrl); }

		/// <summary>
		/// MapGL控制項參考
		/// </summary>
		public GLUICtrl RefMapControl
		{
			get
			{
				return rMapGL;
			}
			set
			{
				if (value != null)
				{
					rMapGL = value;
				}
			}
		}

		#endregion Declaration - Properties

		#region Funciotn - Constructors

		/// <summary>
		/// 共用建構方法
		/// </summary>
		public GoalSetting(CtVehiclePlanner_Ctrl refUI, DockState defState = DockState.Float)
			: base(refUI, defState)
		{
			InitializeComponent();

			/*-- 由於無法於介面設計師新增控制項，只好用程式碼新增 --*/
			tsbPath.Visible = false;
			ConnectButtonEnable(false);
			dgvGoalPoint.Columns.Clear();
			this.Controls.Remove(this.dgvGoalPoint);
			this.Controls.Add(this.cboSingleType);
			this.Controls.Add(dgvGoalPoint);
			this.toolStrip1.BringToFront();
			this.cboSingleType.BringToFront();
			this.dgvGoalPoint.BringToFront();
			Font font = new Font("微軟正黑體", 10, FontStyle.Regular);
			dgvGoalPoint.DefaultCellStyle.Font = font;
			dgvGoalPoint.ColumnHeadersDefaultCellStyle.Font = font;
			dgvGoalPoint.BringToFront();
			cboSingleType.Items.Add(nameof(SinglePairInfo));
			cboSingleType.Items.Add(nameof(SingleTowardPairInfo));
			cboSingleType.Items.Add(nameof(SingleLineInfo));
			cboSingleType.Items.Add(nameof(SingleAreaInfo));
			cboSingleType.Text = "Please select Info";
			cboSingleType.SelectedValueChanged += cboSingleType_SelectedValueChanged;
			cboSingleType.TextChanged += cboSingleType_TextChanged;
			dgvGoalPoint.SelectionChanged += dgvGoalPoint_SelectionChanged;
		}

		/// <summary>
		/// 取得選取的Goal點名稱
		/// </summary>
		/// <returns></returns>
		protected override string GetGoalName()
		{
			string goalname = null;
			var list = GLCMD.CMD.SingleTowerPairInfo.Where(data => data.StyleName == "General");
			if (list != null)
			{
				DataTable table = new DataTable();
				table.Columns.Add(nameof(ISingleTowardPairInfo.ID));
				table.Columns.Add(nameof(ISingleTowardPairInfo.Name));
				foreach (ISingleTowardPairInfo item in list)
				{
					DataRow row = table.NewRow();
					row[nameof(ISingleTowardPairInfo.ID)] = item.ID;
					row[nameof(ISingleTowardPairInfo.Name)] = item.Name;
					table.Rows.Add(row);
				}
				var ctl = SelectBox.Show("Select Goal", table);
				ctl.ShowDialog();
				goalname = ctl.SelectRow?[nameof(ISingleTowardPairInfo.Name)].ToString();
				ctl.Dispose();
			}
			return goalname;
		}
		protected override string GetChargeName()
		{
			string goal = null;
			var list = GLCMD.CMD.SingleTowerPairInfo.Where(data => data.StyleName == "ChargingDocking");
			if (list != null)
			{
				DataTable table = new DataTable();
				table.Columns.Add(nameof(ISingleTowardPairInfo.ID));
				table.Columns.Add(nameof(ISingleTowardPairInfo.Name));
				foreach (ISingleTowardPairInfo item in list)
				{
					DataRow row = table.NewRow();
					row[nameof(ISingleTowardPairInfo.ID)] = item.ID;
					row[nameof(ISingleTowardPairInfo.Name)] = item.Name;
					table.Rows.Add(row);
				}
				var ctl = SelectBox.Show("Select Charge Goal", table);
				ctl.ShowDialog();
				goal = ctl.SelectRow?[nameof(ISingleTowardPairInfo.Name)].ToString();
				ctl.Dispose();
			}
			return goal;
		}

		/// <summary>
		/// 取得所有General Goal
		/// </summary>
		/// <returns></returns>
		protected override List<string> GetGeneralGoals()
		{
			List<string> goals = null;
			//if (cboSingleType.Text == nameof(SingleTowardPairInfo))
			//{
			goals = GLCMD.CMD.SingleTowerPairInfo.Where(data => data.StyleName == "General").Select(data => data.Name).ToList<string>();
			//(from DataGridViewRow row in dgvGoalPoint.Rows
			//					  where row.Cells[nameof(SingleTowardPairInfo.StyleName)].Value.ToString() == "General"
			//					  select row.Cells[nameof(SingleTowardPairInfo.Name)].Value.ToString()).ToList<string>();
			//}
			return goals;
		}

		protected override List<uint> GetSelectedSingleID()
		{
			List<uint> list = null;
			if (mCurrentIndex != -1)
			{
				list = new List<uint>();
				list.Add(Convert.ToUInt32(dgvGoalPoint.Rows[mCurrentIndex].Cells[nameof(SingleTowardPairInfo.ID)].Value.ToString()));
				mCurrentIndex = -1;
			}
			return list;
		}

		protected override void tsbCharging_Click(object sender, EventArgs e)
		{
			if (parentUI.IsCharge)
			{
				Task.Run(() => parentUI.rVehiclePlanner.Controller.Uncharge());
				//ChargeButtonImage(true);
			}
			else
			{
				base.tsbCharging_Click(sender, e);
			}
		}
		internal void InvokeIfNecessaryDgv(MethodInvoker act)
		{
			dgvGoalPoint.InvokeIfNecessary(act);
		}
		#endregion Funciton - Constructors

		#region Functino - Events

		/// <summary>
		/// 重新綁定資料來源
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cboSingleType_SelectedValueChanged(object sender, EventArgs e)
		{
			mCurrentIndex = -1;
			if (sender is ComboBox cbo)
			{
				mSingleInfo = GetSingleInfo(cbo.Text, dgvGoalPoint);
			}
		}

		private void cboSingleType_TextChanged(object sender, EventArgs e)
		{
			mCurrentIndex = -1;
			if (sender is ComboBox cbo)
			{
				mSingleInfo = GetSingleInfo(cbo.Text, dgvGoalPoint);
			}
		}

		/// <summary>
		/// 移動視角
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dgvGoalPoint_DoubleClick(object sender, EventArgs e)
		{
			if (sender is DataGridView dgv)
			{
				int rowIndex = dgv.CurrentRow?.Index ?? -1;
				if (rowIndex >= 0 && rowIndex < dgv.RowCount)
				{
					/*-- 計算中心點 --*/
					var center = mSingleInfo.GetCenter(rowIndex);
					/*-- 將畫面移至中心點 --*/
					rMapGL?.Focus(center.X, center.Y);
				}
			}
		}
		protected override void dgvGoalPoint_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			mSingleInfo.RefreshSingle(e.ColumnIndex, e.RowIndex);
			//switch (cboSingleType.Text){
			//	case nameof(SinglePairInfo):
			//		break;
			//	case nameof(SingleTowardPairInfo):
			//		UpdateSingleTowardPairInfo(sender, e);
			//		break;
			//	case nameof(SingleLineInfo):
			//		break;
			//	case nameof(SingleAreaInfo):
			//		break;
			//	default:
			//		break;
			//}

		}

		private void UpdateSingleTowardPairInfo(object sender, DataGridViewCellEventArgs e)
		{
			int ID = (int)dgvGoalPoint.Rows[e.RowIndex].Cells[0].Value;
			string Name = (string)dgvGoalPoint.Rows[e.RowIndex].Cells[1].Value;
			string Type = (string)dgvGoalPoint.Rows[e.RowIndex].Cells[2].Value;
			double Toward = (double)dgvGoalPoint.Rows[e.RowIndex].Cells[3].Value;
			int X = (int)dgvGoalPoint.Rows[e.RowIndex].Cells[4].Value;
			int Y = (int)dgvGoalPoint.Rows[e.RowIndex].Cells[5].Value;
			switch (dgvGoalPoint.Columns[e.ColumnIndex].HeaderText)
			{
				case "Name":
					GLCMD.CMD.DoRename(ID, Name);
					break;
				case "Toward":

					GLCMD.CMD.DoMoveToward(ID, (int)(X + 1000 * Math.Cos(Toward * Math.PI / 180)), (int)(Y + 1000 * Math.Sin(Toward * Math.PI / 180)));
					break;
				case "X":
				case "Y":
					GLCMD.CMD.DoMoveCenter(ID, X, Y);
					break;
				default:
					break;
			}
		}

		protected virtual void dgvGoalPoint_SelectionChanged(object sender, EventArgs e)
		{
			if (dgvGoalPoint.CurrentRow != null)
			{
				mCurrentIndex = dgvGoalPoint.InvokeIfNecessary(() => dgvGoalPoint.CurrentRow.Index);
			}
		}

		///// <summary>
		///// 標示物參數編輯 
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		// void dgvGoalPoint_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
		//    if (sender is DataGridView dgv) {
		//        if (e.RowIndex >= 0 && e.RowIndex < dgv.RowCount &&
		//            e.ColumnIndex >= 0 && e.ColumnIndex < dgv.ColumnCount) {
		//            mSingleInfo.RefreshSingle(e.ColumnIndex, e.RowIndex);
		//        }
		//    }
		//}

		#endregion Function - Events

		#region Funciotn - Private Methods

		/// <summary>
		/// 依照標示物類型替換標示物資訊物件
		/// </summary>
		/// <param name="singleType">標示物類型名稱</param>
		/// <param name="dgv"></param>
		/// <returns>標示物資訊物件</returns>
		private ISingleInfo GetSingleInfo(string singleType, DataGridView dgv)
		{
			ISingleInfo singleInfo = null;
			switch (singleType)
			{
				case nameof(SinglePairInfo):
					singleInfo = new PairInfo(dgv);
					break;
				case nameof(SingleTowardPairInfo):
					singleInfo = new TowardPairInfo(dgv);
					break;
				case nameof(SingleLineInfo):
					singleInfo = new LineInfo(dgv);
					break;
				case nameof(SingleAreaInfo):
					singleInfo = new AreaInfo(dgv);
					break;
				default:
					throw new Exception($"未定義{singleType}類型資料");
			}
			return singleInfo;
		}

		public void ConnectButtonEnable(bool enable)
		{
			this.InvokeIfNecessary(() =>
			{
				tsbRun.Enabled = enable;
				tsbRunAll.Enabled = enable;
				tsbStop.Enabled = enable;
				tsbCharging.Enabled = enable;
			});
		}

		public void ChargeButtonImage(bool isCharge)
		{
			if (isCharge)
			{
				this.InvokeIfNecessary(() =>
				{
					tsbCharging.Image = Resources.Uncharge;
					tsbCharging.Text = "UnCharge";
				});
			}
			else
			{
				this.InvokeIfNecessary(() =>
				{
					tsbCharging.Image = Resources.Charge;
					tsbCharging.Text = "Charge";
				});
			}
		}

		#endregion Function - Private Methods

		#region Suppoer Class

		/// <summary>
		/// 標示物資訊
		/// </summary>
		internal interface ISingleInfo
		{
			/// <summary>
			/// 取得目標標示物中心座標
			/// </summary>
			/// <param name="rowIndex">目標標示物索引值</param>
			/// <returns>目標標示物中心點</returns>
			(int X, int Y) GetCenter(int rowIndex);

			/// <summary>
			/// 刷新標示物
			/// </summary>
			/// <param name="columnIndex">標示物行索引</param>
			/// <param name="rowIndex">標示物列索引</param>
			void RefreshSingle(int columnIndex, int rowIndex);
		}

		/// <summary>
		/// 標示物資訊基類
		/// </summary>
		/// <typeparam name="T"></typeparam>
		internal abstract class BaseSingleInfo<T> : ISingleInfo where T : IGLCore
		{

			#region Declaration - Fields

			/// <summary>
			/// 標示物資訊顯示表格
			/// </summary>
			protected DataGridView rDgv = null;

			/// <summary>
			/// ID欄位寬度
			/// </summary>
			protected readonly int mColumnWidthID = 30;

			/// <summary>
			/// 樣式欄位寬度
			/// </summary>
			protected readonly int mColumnWidthName = 200;

			/// <summary>
			/// 參數值欄位寬度
			/// </summary>
			protected readonly int mColumnValue = 60;

			#endregion Declaration - Fields

			#region Declaration - Properties

			/// <summary>
			/// ID欄位名稱
			/// </summary>
			protected abstract string ColumnID { get; }

			#endregion Declaratino - Protecties

			#region Function - Constructors

			public BaseSingleInfo(DataGridView dgv)
			{
				rDgv = dgv;
				rDgv.DataSource = new BindingSource(GetSource(), null);
				rDgv.Columns["ID"].ReadOnly = true;
				rDgv.Columns["StyleName"].ReadOnly = true;
				SetColumnWidth();
			}

			#endregion Function - Constructors

			#region Function - Public Methdos

			/// <summary>
			/// 取得目標標示物中心座標
			/// </summary>
			/// <param name="rowIndex">目標標示物行索引</param>
			/// <returns>目標標示物中心座標</returns>
			public abstract (int X, int Y) GetCenter(int rowIndex);

			/// <summary>
			/// 刷新標示物
			/// </summary>
			/// <param name="columnIndex">標示物行索引</param>
			/// <param name="rowIndex">標示物列索引</param>
			public void RefreshSingle(int columnIndex, int rowIndex)
			{
				if (columnIndex >= 0 && columnIndex < rDgv.ColumnCount &&
					rowIndex >= 0 && rowIndex < rDgv.RowCount
					)
				{
					string columnName = rDgv.Columns[columnIndex].Name;
					int targetID = (int)rDgv[ColumnID, rowIndex].Value;
					DataGridViewRow data = rDgv.Rows[rowIndex];
					RefreshSingle(rowIndex, columnName, targetID, data);
				}
			}

			#endregion Function - Public Methods

			#region Function - Private Methods

			/// <summary>
			/// 回傳標示物資料來源
			/// </summary>
			/// <returns>標示物資料來源</returns>
			protected abstract object GetSource();

			/// <summary>
			/// 刷新標示物
			/// </summary>
			/// <param name="rowIndex">標示物行索引</param>
			/// <param name="columnName">編輯的參數名稱</param>
			/// <param name="newValue">編輯後的值</param>
			protected virtual void RefreshSingle(int rowIndex, string columnName, int targetID, DataGridViewRow data)
			{
				throw new Exception($"未定義{columnName}欄位編輯方法，所以無法編輯");
			}

			/// <summary>
			/// 重新命名標示物
			/// </summary>
			/// <param name="targetID">目標標示物ID</param>
			/// <param name="newName">新名稱</param>
			protected virtual void Rename(int targetID, string newName)
			{
				GLCMD.CMD.DoRename(targetID, newName);
			}

			/// <summary>
			/// 取得儲存格值
			/// </summary>
			/// <param name="columnName">欄位名稱</param>
			/// <param name="rowIndex">列索引</param>
			/// <returns>儲存格值</returns>
			protected object GetCellValue(string columnName, int rowIndex)
			{
				return rDgv[columnName, rowIndex].Value;
			}

			/// <summary>
			/// 設定欄位寬度
			/// </summary>
			protected abstract void SetColumnWidth();

			#endregion Function - Private Methods

		}

		/// <summary>
		/// 點資訊
		/// </summary>
		internal class PairInfo : BaseSingleInfo<ISinglePairInfo>
		{

			/// <summary>
			/// ID欄位名稱
			/// </summary>
			protected override string ColumnID => nameof(ISinglePairInfo.ID);

			public PairInfo(DataGridView dgv) : base(dgv) { }

			/// <summary>
			/// 取得目標標示物中心座標
			/// </summary>
			/// <param name="rowIndex">目標標示物行索引</param>
			/// <returns>目標標示物中心座標</returns>
			public override (int X, int Y) GetCenter(int rowIndex)
			{
				int cX = (int)rDgv[nameof(ISinglePairInfo.X), rowIndex].Value;
				int cY = (int)rDgv[nameof(ISinglePairInfo.Y), rowIndex].Value;
				return (cX, cY);
			}

			/// <summary>
			/// 回傳標示物資料來源
			/// </summary>
			/// <returns>標示物資料來源</returns>
			protected override object GetSource()
			{
				return GLCMD.CMD.SinglePairInfo;
			}

			/// <summary>
			/// 刷新標示物
			/// </summary>
			/// <param name="rowIndex">標示物行索引</param>
			/// <param name="columnName">編輯的參數名稱</param>
			/// <param name="newValue">編輯後的值</param>
			protected override void RefreshSingle(int rowIndex, string columnName, int targetID, DataGridViewRow data)
			{
				string newValue = data.Cells[columnName].Value.ToString();
				switch (columnName)
				{
					case nameof(ISinglePairInfo.Name):
						Rename(targetID, newValue as string);
						break;
					case nameof(ISinglePairInfo.X):
						Move(targetID, int.Parse(newValue), int.Parse(data.Cells[nameof(ISinglePairInfo.Y)].Value.ToString()));
						break;
					case nameof(ISinglePairInfo.Y):
						Move(targetID, int.Parse(data.Cells[nameof(ISinglePairInfo.X)].Value.ToString()), int.Parse(newValue));
						break;
					default:
						break;
				}
			}

			/// <summary>
			/// 移動目標標示物到指定座標
			/// </summary>
			/// <param name="id">目標標示物ID</param>
			/// <param name="x">指定坐標X</param>
			/// <param name="y">指定坐標Y</param>
			protected virtual void Move(int id, int x, int y)
			{
				GLCMD.CMD.DoMoveCenter(id, x, y);
				GLCMD.CMD.MoveFinish();
			}

			/// <summary>
			/// 設定欄位寬度
			/// </summary>
			protected override void SetColumnWidth()
			{
				rDgv.Columns[nameof(ISinglePairInfo.ID)].Width = mColumnWidthID;
				rDgv.Columns[nameof(ISinglePairInfo.StyleName)].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
				rDgv.Columns[nameof(ISinglePairInfo.Name)].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				rDgv.Columns[nameof(ISinglePairInfo.X)].Width = mColumnValue;
				rDgv.Columns[nameof(ISinglePairInfo.Y)].Width = mColumnValue;
			}

		}

		/// <summary>
		/// 方向點資訊
		/// </summary>
		internal class TowardPairInfo : PairInfo
		{

			/// <summary>
			/// ID欄位名稱
			/// </summary>
			protected override string ColumnID => nameof(ISingleTowardPairInfo.ID);

			public TowardPairInfo(DataGridView dgv) : base(dgv) { }

			/// <summary>
			/// 刷新標示物
			/// </summary>
			/// <param name="rowIndex">標示物行索引</param>
			/// <param name="columnName">編輯的參數名稱</param>
			/// <param name="newValue">編輯後的值</param>
			protected override void RefreshSingle(int rowIndex, string columnName, int targetID, DataGridViewRow data)
			{
				string newValue = data.Cells[columnName].Value.ToString();
				switch (columnName)
				{
					case nameof(ISingleTowardPairInfo.Name):
						Rename(targetID, newValue as string);
						break;
					case nameof(ISingleTowardPairInfo.X):
						Move(targetID, int.Parse(newValue), int.Parse(data.Cells[nameof(ISingleTowardPairInfo.Y)].Value.ToString()));
						break;
					case nameof(ISingleTowardPairInfo.Y):
						Move(targetID, int.Parse(data.Cells[nameof(ISingleTowardPairInfo.X)].Value.ToString()), int.Parse(newValue));
						break;
					case nameof(ISingleTowardPairInfo.Toward):
						MoveToward(targetID, (int)data.Cells[nameof(ISingleTowardPairInfo.X)].Value, (int)data.Cells[nameof(ISingleTowardPairInfo.Y)].Value, double.Parse(newValue));
						break;
					default:
						break;
				}
			}

			/// <summary>
			/// 回傳標示物資料來源
			/// </summary>
			/// <returns>標示物資料來源</returns>
			protected override object GetSource()
			{
				return GLCMD.CMD.SingleTowerPairInfo;
			}

			/// <summary>
			/// 移動到指定角度
			/// </summary>
			/// <param name="sTheta">指定角度</param>
			/// <param name="rowIndex">目標標示物行索引</param>
			protected void MoveToward(int id, int x, int y, double angle)
			{
				double theta = angle * Math.PI / 180;
				int dx = (int)(x + 1000 * Math.Cos(theta));
				int dy = (int)(y + 1000 * Math.Sin(theta));
				GLCMD.CMD.DoMoveToward(id, dx, dy);
			}

			/// <summary>
			/// 設定欄位寬度
			/// </summary>
			protected override void SetColumnWidth()
			{
				base.SetColumnWidth();
				rDgv.Columns[nameof(ISingleTowardPairInfo.Toward)].Width = mColumnValue;
			}
		}

		/// <summary>
		/// 線資訊
		/// </summary>
		internal class LineInfo : BaseSingleInfo<ISingleLineInfo>
		{

			/// <summary>
			/// ID欄位名稱
			/// </summary>
			protected override string ColumnID => nameof(ISingleLineInfo.ID);

			public LineInfo(DataGridView dgv) : base(dgv) { }

			/// <summary>
			/// 取得目標標示物中心座標
			/// </summary>
			/// <param name="rowIndex">目標標示物行索引</param>
			/// <returns>目標標示物中心座標</returns>
			public override (int X, int Y) GetCenter(int rowIndex)
			{
				int x = (int)rDgv[nameof(ISingleLineInfo.X0), rowIndex].Value;
				int cX = (int)rDgv[nameof(ISingleLineInfo.X1), rowIndex].Value;
				int y = (int)rDgv[nameof(ISingleLineInfo.Y0), rowIndex].Value;
				int cY = (int)rDgv[nameof(ISingleLineInfo.Y1), rowIndex].Value;
				cX = (x + cX) / 2;
				cY = (y = cY) / 2;
				return (cX, cY);
			}

			/// <summary>
			/// 回傳標示物資料來源
			/// </summary>
			/// <returns>標示物資料來源</returns>
			protected override object GetSource()
			{
				return GLCMD.CMD.SingleLineInfo;
			}

			/// <summary>
			/// 刷新標示物
			/// </summary>
			/// <param name="rowIndex">標示物行索引</param>
			/// <param name="columnName">編輯的參數名稱</param>
			/// <param name="newValue">編輯後的值</param>
			protected override void RefreshSingle(int rowIndex, string columnName, int targetID, DataGridViewRow data)
			{
				string newValue = data.Cells[columnName].Value.ToString();
				switch (columnName)
				{
					case nameof(ISingleLineInfo.Name):
						Rename(targetID, newValue as string);
						break;
					case nameof(ISingleLineInfo.X0):
						MoveBegin(targetID, int.Parse(newValue), int.Parse(data.Cells[nameof(ISingleLineInfo.Y0)].Value.ToString()));
						break;
					case nameof(ISingleLineInfo.Y0):
						MoveBegin(targetID, int.Parse(data.Cells[nameof(ISingleLineInfo.X0)].Value.ToString()), int.Parse(newValue));
						break;
					case nameof(ISingleLineInfo.X1):
						MoveEnd(targetID, int.Parse(newValue), int.Parse(data.Cells[nameof(ISingleLineInfo.Y1)].Value.ToString()));
						break;
					case nameof(ISingleLineInfo.Y1):
						MoveEnd(targetID, int.Parse(data.Cells[nameof(ISingleLineInfo.X1)].Value.ToString()), int.Parse(newValue));
						break;
					default:
						break;
				}
			}

			/// <summary>
			/// 移動目標標示物起始位置
			/// </summary>
			/// <param name="id">目標標示物ID</param>
			/// <param name="x">起始座標X</param>
			/// <param name="y">起始座標Y</param>
			protected void MoveBegin(int id, int x, int y)
			{
				GLCMD.CMD.DoMoveBegin(id, x, y);
			}

			/// <summary>
			/// 移動目標標示物結束位置
			/// </summary>
			/// <param name="id">目標標示物ID</param>
			/// <param name="x">結束座標X</param>
			/// <param name="y">結束座標Y</param>
			protected void MoveEnd(int id, int x, int y)
			{
				GLCMD.CMD.DoMoveEnd(id, x, y);
			}

			/// <summary>
			/// 設定欄位寬度
			/// </summary>
			protected override void SetColumnWidth()
			{
				rDgv.Columns[nameof(ISingleLineInfo.ID)].Width = mColumnWidthID;
				rDgv.Columns[nameof(ISingleLineInfo.StyleName)].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
				rDgv.Columns[nameof(ISingleLineInfo.Name)].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				rDgv.Columns[nameof(ISingleLineInfo.X0)].Width = mColumnValue;
				rDgv.Columns[nameof(ISingleLineInfo.X1)].Width = mColumnValue;
				rDgv.Columns[nameof(ISingleLineInfo.Y0)].Width = mColumnValue;
				rDgv.Columns[nameof(ISingleLineInfo.Y1)].Width = mColumnValue;
			}

		}

		/// <summary>
		/// 面資訊
		/// </summary>
		internal class AreaInfo : BaseSingleInfo<ISingleAreaInfo>
		{

			/// <summary>
			/// ID欄位名稱
			/// </summary>
			protected override string ColumnID => nameof(ISingleAreaInfo.ID);

			public AreaInfo(DataGridView dgv) : base(dgv) { }

			/// <summary>
			/// 取得目標標示物中心座標
			/// </summary>
			/// <param name="rowIndex">目標標示物行索引</param>
			/// <returns>目標標示物中心座標</returns>
			public override (int X, int Y) GetCenter(int rowIndex)
			{
				int x = (int)rDgv[nameof(SingleAreaInfo.MinX), rowIndex].Value;
				int cX = (int)rDgv[nameof(SingleAreaInfo.MaxX), rowIndex].Value;
				int y = (int)rDgv[nameof(SingleAreaInfo.MinY), rowIndex].Value;
				int cY = (int)rDgv[nameof(SingleAreaInfo.MaxY), rowIndex].Value;
				cX = (x + cX) / 2;
				cY = (y = cY) / 2;
				return (cX, cY);
			}

			/// <summary>
			/// 回傳標示物資料來源
			/// </summary>
			/// <returns>標示物資料來源</returns>
			protected override object GetSource()
			{
				return GLCMD.CMD.SingleAreaInfo;
			}

			/// <summary>
			/// 刷新標示物
			/// </summary>
			/// <param name="rowIndex">標示物行索引</param>
			/// <param name="columnName">編輯的參數名稱</param>
			/// <param name="newValue">編輯後的值</param>
			protected override void RefreshSingle(int rowIndex, string columnName, int targetID, DataGridViewRow data)
			{
				string newValue = data.Cells[columnName].Value.ToString();
				switch (columnName)
				{
					case nameof(ISingleAreaInfo.Name):
						Rename(targetID, newValue);
						break;
					case nameof(ISingleAreaInfo.MaxX):
						MoveMax(targetID, int.Parse(newValue), int.Parse(data.Cells[nameof(ISingleAreaInfo.MaxY)].Value.ToString()));
						break;
					case nameof(ISingleAreaInfo.MaxY):
						MoveMax(targetID, int.Parse(data.Cells[nameof(ISingleAreaInfo.MaxX)].Value.ToString()), int.Parse(newValue));
						break;
					case nameof(ISingleAreaInfo.MinX):
						MoveMin(targetID, int.Parse(newValue), int.Parse(data.Cells[nameof(ISingleAreaInfo.MinY)].Value.ToString()));
						break;
					case nameof(ISingleAreaInfo.MinY):
						MoveMin(targetID, int.Parse(data.Cells[nameof(ISingleAreaInfo.MinX)].Value.ToString()), int.Parse(newValue));
						break;
					default:
						break;
				}
			}

			/// <summary>
			/// 移動目標標示物最大值座標
			/// </summary>
			/// <param name="id">目標標示物ID</param>
			/// <param name="x">最大值座標X</param>
			/// <param name="y">最大值座標Y</param>
			protected void MoveMax(int id, int x, int y)
			{
				GLCMD.CMD.DoMoveMax(id, x, y);
			}

			/// <summary>
			/// 移動目標標示物最小值座標
			/// </summary>
			/// <param name="id">目標標示物ID</param>
			/// <param name="x">最小值座標Y</param>
			/// <param name="y">最小值座標Y</param>
			protected void MoveMin(int id, int x, int y)
			{
				GLCMD.CMD.DoMoveMin(id, x, y);
			}

			/// <summary>
			/// 設定欄位寬度
			/// </summary>
			protected override void SetColumnWidth()
			{
				rDgv.Columns[nameof(ISingleAreaInfo.ID)].Width = mColumnWidthID;
				rDgv.Columns[nameof(ISingleAreaInfo.StyleName)].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
				rDgv.Columns[nameof(ISingleAreaInfo.Name)].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				rDgv.Columns[nameof(ISingleAreaInfo.MaxX)].Width = mColumnValue;
				rDgv.Columns[nameof(ISingleAreaInfo.MinX)].Width = mColumnValue;
				rDgv.Columns[nameof(ISingleAreaInfo.MaxY)].Width = mColumnValue;
				rDgv.Columns[nameof(ISingleAreaInfo.MinY)].Width = mColumnValue;
			}
		}

		#endregion Suppoer Class
	}

}
