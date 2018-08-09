using CtDockSuit;
using CtLib.Library;
//using Geometry;
//using GLCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using VehiclePlanner.Core;
using VehiclePlanner.Module.Interface;
using VehiclePlanner.Partial.VehiclePlannerUI;
using WeifenLuo.WinFormsUI.Docking;
using static VehiclePlanner.Partial.VehiclePlannerUI.Events.GoalSettingEvents;
using CtLib.Module.Utility;

namespace VehiclePlanner.Module.Implement {

	/// <summary>
	/// Goal點設定介面
	/// </summary>
	public partial class BaseGoalSetting : AuthorityDockContainer, IBaseGoalSetting {

		#region Declaration - Fields

		protected readonly object mKey = new object();

		/// <summary>
		/// 右鍵選單所點的列索引
		/// </summary>
		protected int mSelectedRowIdx = -1;

		/// <summary>
		/// 是否全選
		/// </summary>
		protected bool mSelectAll = false;


		#endregion Declaration - Fields

		#region Declaration - Const

		protected const int IDColumn = 1;
		protected const int NameColumn = 2;
		protected const int SelectColumn = 0;
		protected const int TowardColumn = 5;
		protected const int XColumn = 3;
		protected const int YColumn = 4;

		#endregion Declaration - Const

		#region Funciton - Construcotrs

		/// <summary>
		/// 給介面設計師使用的建構式，拿掉後繼承該類的衍生類將無法顯示介面設計
		/// </summary>
		protected BaseGoalSetting() : base() {
			InitializeComponent();
			dgvGoalPoint.CellValueChanged += dgvGoalPoint_CellValueChanged;

		}

		/// <summary>
		/// 共用建構方法
		/// </summary>
		/// <param name="goalsetting">GoalSetting方法實作物件參考</param>
		/// <param name="main">主介面參考</param>
		/// <param name="defState">預設停靠方式</param>
		public BaseGoalSetting(BaseVehiclePlanner_Ctrl refUI, DockState defState = DockState.Float)
			: base(refUI, defState) {
			InitializeComponent();
			dgvGoalPoint.CellValueChanged += dgvGoalPoint_CellValueChanged;
			FixedSize = new Size(776, 860);
		}

		protected virtual string GetGoalName()
		{
			string goalName = cmbGoalList.InvokeIfNecessary(() => cmbGoalList.Text);
			return goalName;
		}

		protected virtual string GetChargeName()
		{
			return cmbGoalList.InvokeIfNecessary(() => cmbGoalList.Text);
		}

		protected virtual List<string> GetGeneralGoals()
		{
			throw new NotImplementedException();
		}

		#endregion Funciton - Construcotrs

		#region Implement - IIGoalSetting

		/// <summary>
		/// 目標點個數
		/// </summary>
		public int GoalCount {
			get {
				lock (mKey) {
					return dgvGoalPoint.Rows.Count;
				}
			}
		}

		/// <summary>
		/// 移除所有 Goal 點
		/// </summary>
		public void ClearGoal() {
			lock (mKey) {
				dgvGoalPoint.InvokeIfNecessary(() => dgvGoalPoint.Rows.Clear());
				cmbGoalList.InvokeIfNecessary(() => {
					cmbGoalList.Items.Clear();
					cmbGoalList.SelectedIndex = ListBox.NoMatches;
				});
			}
		}

		/// <summary>
		/// 根據 ID 移除 Goal 點
		/// </summary>
		public void DeleteGoal(uint ID) {
			lock (mKey) {
				int row = FindIndexByID(ID);
				if (row != -1) {
					dgvGoalPoint.InvokeIfNecessary(() => dgvGoalPoint.Rows.RemoveAt(row));
					cmbGoalList.InvokeIfNecessary(() => {
						if (cmbGoalList.SelectedIndex == row) {
							cmbGoalList.SelectedIndex = ListBox.NoMatches;
						}
						cmbGoalList.Items.RemoveAt(row);
					});
				}
			}
		}

		/// <summary>
		/// 根據 ID 移除 Goal 點
		/// </summary>
		public void DeleteGoals(IEnumerable<uint> ID) {
			lock (mKey) {
				foreach (var id in ID) {
					DeleteGoal(id);
				}
			}
		}

		/// <summary>
		/// 設定表單選擇項目
		/// </summary>
		public void SetSelectItem(uint id) {
			lock (mKey) {
				dgvGoalPoint.InvokeIfNecessary(() => {
					for (int row = 0; row < dgvGoalPoint.RowCount; row++) {
						if ((uint)dgvGoalPoint[IDColumn, row].Value == id) {
							dgvGoalPoint.Rows[row].Selected = true;
						} else {
							dgvGoalPoint.Rows[row].Selected = false;
						}
					}
				});
			}
		}

		/// <summary>
		/// 回傳該權限是否具有控制權
		/// </summary>
		/// <param name="lv">使用者權限</param>
		/// <returns>是否具有控制權</returns>
		public override bool IsVisiable(AccessLevel lv) {
			return lv > AccessLevel.None;
		}

		#endregion Implement - IIGoalSetting

		#region UI Event

		#region GoalList

		/// <summary>
		/// Goal點清單滑鼠點擊事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void dgvGoalPoint_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
			if (e.RowIndex > -1) {
				switch (e.Button) {
					case MouseButtons.Left://全選功能
						if (e.RowIndex == -1 && dgvGoalPoint.Columns[e.ColumnIndex].Name == "cSelect") {
							mSelectAll = !mSelectAll;
							this.InvokeIfNecessary(() => {
								foreach (DataGridViewRow row in dgvGoalPoint.Rows) {
									row.Cells["cSelect"].Value = mSelectAll;
									Console.WriteLine(row.Cells["cName"].Value);
								}
								dgvGoalPoint.EndEdit();
							});
						}
						break;
					case MouseButtons.Right://右鍵選單
						mSelectedRowIdx = e.RowIndex;
						contextMenuStrip1.Show(Cursor.Position);
						break;
				}
			}
		}
		protected virtual void dgvGoalPoint_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
			Console.WriteLine("Test");
			uint ID = (uint) dgvGoalPoint.Rows[e.RowIndex].Cells[1].Value;
			var colName = dgvGoalPoint.Columns[e.ColumnIndex].HeaderText;
			var value = dgvGoalPoint.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
			rUI.UpdateValue(ID,colName,value );
		}
		#endregion GoalList

		#region Button

		private void btnGetGoalList_Click(object sender, EventArgs e) {
            Task.Run(() => {
                rUI.GetGoalName();
            });
        }
        
        #endregion Button

        #region ToolStripButton

        /// <summary>
        /// 將當前位置設為Goal點
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbAddNow_Click(object sender, EventArgs e) {
            rUI.AddNow();
        }

        /// <summary>
        /// 刪除選擇的Goal點
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbDelete_Click(object sender, EventArgs e) {
            dgvGoalPoint.EndEdit();
            rUI.Delete(GetSelectedSingleID());
        }

        /// <summary>
        /// 將當前設定寫入Map檔
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbSave_Click(object sender, EventArgs e) {
            rUI.SaveMap();
        }

        /// <summary>
        /// 路徑規劃
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbPath_Click(object sender, EventArgs e) {
            string goalName = cmbGoalList.InvokeIfNecessary(() => cmbGoalList.Text);
            rUI.FindPath(goalName);
        }

        /// <summary>
        /// 跑向指定Goal點
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbRun_Click(object sender, EventArgs e) {
			//string goalName = cmbGoalList.InvokeIfNecessary(() => cmbGoalList.Text);
			string goalName = GetGoalName();
			if (!string.IsNullOrEmpty(goalName))
			{
				rUI.Run(goalName);
			}
        }

        private void tsbRunAll_Click(object sender, EventArgs e) {
			//GetSelectedSingleID();
			rUI.RunLoop(GetGeneralGoals());
        }

		private void tsbStop_Click(object sender, EventArgs e)
		{
			rUI.StopRunLoop();
		}

		/// <summary>
		/// 進行充電
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbCharging_Click(object sender, EventArgs e) {
            lock (mKey) {
				string powerName = GetChargeName();
                rUI.Charging(powerName);
            }
        }


        #endregion ToolStripButton

        #region MenuItem

        /// <summary>
        /// 搜尋路徑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miPath_Click(object sender, EventArgs e) {
            string name = ToSingleName(mSelectedRowIdx);
            rUI.FindPath(name);
        }

        /// <summary>
        /// 移動至該Goal點
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miRun_Click(object sender, EventArgs e) {
            string name = ToSingleName(mSelectedRowIdx);
            rUI.Run(name);
        }

        /// <summary>
        /// 至該充電站進行充電
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miCharging_Click(object sender, EventArgs e) {
            string name = ToSingleName(mSelectedRowIdx);
            rUI.Charging(name);
        }

        /// <summary>
        /// 刪除該Goal點
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miDelete_Click(object sender, EventArgs e) {
            uint id = ToSingleID(mSelectedRowIdx);
            List<uint> ids = new List<uint>();
            ids.Add(id);
            rUI.Delete(ids);
        }

        #endregion MenuItem

        #endregion UI Event

        #region Fucnction - Private Methods

        /// <summary>
        /// 獲得所有被選取的 Goal 點ID
        /// </summary>
        protected virtual List<uint> GetSelectedSingleID() {
            var list = new List<uint>();
            dgvGoalPoint.InvokeIfNecessary(() => {
                for (int row = 0; row < GoalCount; ++row) {
                    bool isSelected = (bool)dgvGoalPoint[SelectColumn, row].Value;
                    if (isSelected) {
                        uint id = (uint)dgvGoalPoint[IDColumn, row].Value;
                        list.Add(id);
                    }
                }
            });
            return list;
        }

        /// <summary>
        /// 回傳所有Goal點
        /// </summary>
        /// <returns></returns>
        private List<uint> GetAllGoal() {
            lock (mKey) {
                var list = new List<uint>();
                dgvGoalPoint.InvokeIfNecessary(() => {
                    for (int row = 0; row < GoalCount; row++) {
                        uint id = (uint)dgvGoalPoint[IDColumn, row].Value;
                        list.Add(id);
                    }
                });
                return list;
            }
        }

        /// <summary>
        /// 用 ID 尋找 Goal 點所在的引索位置
        /// </summary>
        protected int FindIndexByID(uint ID) {
            lock (mKey) {
                for (int row = 0; row < dgvGoalPoint.Rows.Count; ++row) {
                    if ((uint)(dgvGoalPoint[IDColumn, row].Value) == ID) return row;
                }
                return -1;
            }
        }

        /// <summary>
        /// 快捷鍵
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            bool ret = true;
            switch (keyData) {
                case Keys.N | Keys.Control:
                    rUI.AddNow();
                    break;
                case Keys.D | Keys.Control:
                    dgvGoalPoint.EndEdit();
                    rUI.Delete(GetSelectedSingleID());
                    break;
                case Keys.S | Keys.Control:
                    rUI.SaveMap();
                    break;
                case Keys.P | Keys.Control:
                    //string goalName = cmbGoalList.InvokeIfNecessary(() => cmbGoalList.Text);
                    //rUI.FindPath(goalName);
                    Console.WriteLine("Path");
                    break;
                case Keys.R | Keys.Control:
                    //rUI.Run();
                    Console.WriteLine("Run");
                    break;
                case Keys.I | Keys.Control:
                    //RunAll
                    Console.WriteLine("RunAll");
                    break;
                case Keys.C | Keys.Control:
                    //Charging
                    Console.WriteLine("Charging");
                    break;
                default:
                    ret = base.ProcessCmdKey(ref msg, keyData);
                    break;
            }
            return ret;
        }

        /// <summary>
        /// 取得對應標記物索引的標記物名稱
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <returns></returns>
        protected string ToSingleName(int rowIdx) {
            string name = string.Empty;
            this.InvokeIfNecessary(() => {
                name = dgvGoalPoint["cName", rowIdx].Value.ToString();
            });
            return name;
        }

        /// <summary>
        /// 取得對應標記物索引的標記物ID
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <returns></returns>
        protected uint ToSingleID(int rowIdx) {
            uint id = 0;
            this.InvokeIfNecessary(() => {
                id = (uint)dgvGoalPoint["cID", rowIdx].Value;
            });
            return id;
        }

        #endregion Fucnction - Private Methods

        #region Implement - IDataDisplay<ICtVehiclePlanner>

        /// <summary>
        /// 資料綁定
        /// </summary>
        /// <param name="source">資料來源</param>
        public void Bindings(IBaseVehiclePlanner source) {
            if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);
        }

		#endregion Implement - IDataDisplay<ICtVehiclePlanner>

	}

}