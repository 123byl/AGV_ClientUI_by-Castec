using CtDockSuit;
using CtLib.Library;
using Geometry;
using GLCore;
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

namespace VehiclePlanner.Module.Implement {

    /// <summary>
    /// Goal點設定介面
    /// </summary>
    public partial class CtGoalSetting : CtDockContainer, IGoalSetting {

        #region Declaration - Fields

        private readonly object mKey = new object();

        #endregion Declaration - Fields

        #region Declaration - Const

        private const int IDColumn = 1;
        private const int NameColumn = 2;
        private const int SelectColumn = 0;
        private const int TowardColumn = 5;
        private const int XColumn = 3;
        private const int YColumn = 4;

        #endregion Declaration - Const

        #region Funciton - Construcotrs

        /// <summary>
        /// 共用建構方法
        /// </summary>
        /// <param name="goalsetting">GoalSetting方法實作物件參考</param>
        /// <param name="main">主介面參考</param>
        /// <param name="defState">預設停靠方式</param>
        public CtGoalSetting(DockState defState = DockState.Float)
            : base(defState) {
            InitializeComponent();
            FixedSize = new Size(776, 860);
        }

        #endregion Funciton - Construcotrs

        #region Implement - IIGoalSetting

        /// <summary>
        /// 加入 Goal 點
        /// </summary>
        public event DelAddCurrentGoal AddCurrentGoalEvent;

        /// <summary>
        /// 清除所有目標點
        /// </summary>
        public event DelClearGoals ClearGoalsEvent;

        /// <summary>
        /// 刪除
        /// </summary>
        public event DelDeleteSingle DeleteSingleEvent;

        /// <summary>
        /// 尋找路徑
        /// </summary>
        public event DelFindPath FindPathEvent;

        /// <summary>
        /// 載入地圖
        /// </summary>
        public event DelLoadMap LoadMapEvent;

        /// <summary>
        /// 從 AGV 下載地圖
        /// </summary>
        public event DelLoadMapFromAGV LoadMapFromAGVEvent;

        /// <summary>
        /// 移動
        /// </summary>
        public event DelRunGoal RunGoalEvent;

        /// <summary>
        /// 按照順序移動全部
        /// </summary>
        public event DelRunLoop RunLoopEvent;

        /// <summary>
        /// 儲存
        /// </summary>
        public event DelSaveGoal SaveGoalEvent;

        /// <summary>
        /// 上傳地圖
        /// </summary>
        public event DelSendMapToAGV SendMapToAGVEvent;

        /// <summary>
        /// 取得所有Goal點名稱
        /// </summary>
        public event DelGetGoalNames GetGoalNames;

        /// <summary>
        /// 充電
        /// </summary>
        public event DelCharging Charging;

        public event Events.TestingEvents.DelClearMap ClearMap;

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
        /// 更新現在位置
        /// </summary>
        public void UpdateNowPosition(IPair nowPisition) {
            lock (mKey) {
                txtAddPx.InvokeIfNecessary(() => {
                    if (txtAddPx.Text != nowPisition.X.ToString()) txtAddPx.Text = nowPisition.X.ToString();
                    if (txtAddPy.Text != nowPisition.Y.ToString()) txtAddPy.Text = nowPisition.Y.ToString();
                });
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
        /// 重新載入標示物
        /// </summary>
        public void ReloadSingle() {
            lock (mKey) {
                ClearGoal();
                Database.GoalGM.SaftyForLoop(LoadSingle);
                Database.PowerGM.SaftyForLoop(LoadSingle);
            }
        }

        #endregion Implement - IIGoalSetting

        #region UI Event

        #region Button

        private void btnGetGoalList_Click(object sender, EventArgs e) {
            Task.Run(() => {
                GetGoalNames.Invoke();
            });
        }

        private void btnCurrPos_Click(object sender, EventArgs e) {
            lock (mKey) {
                AddCurrentGoalEvent?.Invoke();
            }
        }

        private void btnGetMap_Click(object sender, EventArgs e) {
            lock (mKey) {
                Task.Run(() => LoadMapFromAGVEvent?.Invoke());
            }
        }

        private void btnLoadMap_Click(object sender, EventArgs e) {
            lock (mKey) {
                LoadMapEvent?.Invoke();
            }
        }

        private void btnPath_Click(object sender, EventArgs e) {
            Task.Run(() => {
                string goalName = cmbGoalList.InvokeIfNecessary(() => cmbGoalList.Text);
                FindPathEvent?.Invoke(goalName);
            });
        }

        private void btnSendMap_Click(object sender, EventArgs e) {
            lock (mKey) {
                SendMapToAGVEvent?.Invoke();
            }
        }

        private void btnGoGoal_Click(object sender, EventArgs e) {
            lock (mKey) {
                Task.Run(() => {
                    string goalName = cmbGoalList.InvokeIfNecessary(() => cmbGoalList.Text);
                    RunGoalEvent?.Invoke(goalName);
                });
            }
        }

        private void btnRunAll_Click(object sender, EventArgs e) {
            lock (mKey) {
                RunLoopEvent?.BeginInvoke(GetGoals(), null, null);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e) {
            lock (mKey) {
                DeleteSingleEvent?.Invoke(GetSelectedSingleID());
            }
        }

        private void btnDeleteAll_Click(object sender, EventArgs e) {
            lock (mKey) {
                ClearGoalsEvent?.Invoke();
            }
        }

        private void btnSaveGoal_Click(object sender, EventArgs e) {
            lock (mKey) {
                SaveGoalEvent?.Invoke();
            }
        }

        private void btnCharging_Click(object sender, EventArgs e) {
            lock (mKey) {
                Task.Run(() => {
                    string powerName = cmbGoalList.InvokeIfNecessary(() => cmbGoalList.Text);
                    Charging?.Invoke(powerName);
                });
            }
        }

        private void btnClear_Click(object sender, EventArgs e) {
            lock (mKey) {
                ClearMap?.Invoke();
            }
        }

        #endregion Button

        #endregion UI Event



        #region Fucnction - Private Methods

        /// <summary>
        /// 獲得所有 Goal 點資訊
        /// </summary>
        private List<IGoal> GetGoals() {
            lock (mKey) {
                var list = new List<IGoal>();
                dgvGoalPoint.InvokeIfNecessary(() => {
                    for (int row = 0; row < GoalCount; ++row) {
                        uint id = 0;
                        id = Convert.ToUInt32(dgvGoalPoint[IDColumn, row].Value);
                        if (Database.GoalGM.ContainsID(id)) list.Add(Database.GoalGM[id]);
                    }
                });
                return list;
            }
        }

        /// <summary>
        /// 獲得所有被選取的 Goal 點ID
        /// </summary>
        private List<uint> GetSelectedSingleID() {
            lock (mKey) {
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
        }

        /// <summary>
        /// 用 ID 尋找 Goal 點所在的引索位置
        /// </summary>
        private int FindIndexByID(uint ID) {
            lock (mKey) {
                for (int row = 0; row < dgvGoalPoint.Rows.Count; ++row) {
                    if ((uint)(dgvGoalPoint[IDColumn, row].Value) == ID) return row;
                }
                return -1;
            }
        }

        /// <summary>
        /// 載入標示物
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <param name="goal"></param>
        private void LoadSingle<T>(uint uid, ISingle<T> goal) where T : ITowardPair {
            int index = FindIndexByID(uid);
            if (!cmbGoalList.Items.Contains(goal.Name)) {
                cmbGoalList.BeginInvokeIfNecessary(() => cmbGoalList.Items.Add(goal.Name));
            }

            if (index == -1) {
                dgvGoalPoint.BeginInvokeIfNecessary(
                    () => dgvGoalPoint.Rows.Add(new object[] { new CheckBox().Checked = false, uid, goal.Name, goal.Data.Position.X, goal.Data.Position.Y, goal.Data.Toward.Theta.ToString("F2") }));
            } else {
                dgvGoalPoint.BeginInvokeIfNecessary(
                    () => {
                        if ((uint)dgvGoalPoint[IDColumn, index].Value != uid) dgvGoalPoint[IDColumn, index].Value = uid;
                        if ((string)dgvGoalPoint[NameColumn, index].Value != goal.Name) dgvGoalPoint[NameColumn, index].Value = goal.Name;
                        if ((double)dgvGoalPoint[XColumn, index].Value != goal.Data.Position.X) dgvGoalPoint[XColumn, index].Value = goal.Data.Position.X;
                        if ((double)dgvGoalPoint[YColumn, index].Value != goal.Data.Position.Y) dgvGoalPoint[YColumn, index].Value = goal.Data.Position.Y;
                        if ((string)dgvGoalPoint[TowardColumn, index].Value != goal.Data.Toward.Theta.ToString("F2")) dgvGoalPoint[TowardColumn, index].Value = goal.Data.Toward.Theta.ToString("F2");
                    });
            }
        }

        private void UpdataSingle(uint uid, ISingle<ITowardPair> single, Dictionary<uint, int> mapping) {
            double x = single.Data.Position.X;
            double y = single.Data.Position.Y;
            double theta = single.Data.Toward.Theta;
            if (mapping.ContainsKey(uid)) {
                int index = mapping[uid];
                dgvGoalPoint.BeginInvokeIfNecessary(
                    () => {
                        DataGridViewRow row = dgvGoalPoint.Rows[index];
                        if ((uint)row.Cells[IDColumn].Value != uid) row.Cells[IDColumn].Value = uid;
                        if ((string)row.Cells[NameColumn].Value != single.Name) row.Cells[NameColumn].Value = single.Name;
                        if ((double)row.Cells[XColumn].Value != x) row.Cells[XColumn].Value = x;
                        if ((double)row.Cells[YColumn].Value != y) row.Cells[YColumn].Value = y;
                        if ((string)row.Cells[TowardColumn].Value != theta.ToString("F2")) row.Cells[TowardColumn].Value = theta.ToString("F2");
                    });
            } else {
                dgvGoalPoint.BeginInvokeIfNecessary(
                () => dgvGoalPoint.Rows.Add(new object[] { new CheckBox().Checked = false, uid, single.Name, x, y, theta.ToString("F2") }));
                cmbGoalList.BeginInvokeIfNecessary(() => {
                    cmbGoalList.Items.Add(single.Name);
                });
            }
        }

        #endregion Fucnction - Private Methods

        #region Implement - IDataDisplay<ICtVehiclePlanner>

        /// <summary>
        /// 資料綁定
        /// </summary>
        /// <param name="source">資料來源</param>
        public void Bindings(ICtVehiclePlanner source) {
            if (source.DelInvoke == null) source.DelInvoke = invk => this.InvokeIfNecessary(invk);
        }

        #endregion Implement - IDataDisplay<ICtVehiclePlanner>

        private void grbMap_Enter(object sender, EventArgs e) {

        }
    }
}