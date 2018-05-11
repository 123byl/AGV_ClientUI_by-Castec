using CtLib.Library;
using Geometry;
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
using VehiclePlanner.Module.Implement;
using WeifenLuo.WinFormsUI.Docking;
using static VehiclePlanner.Partial.VehiclePlannerUI.Events.GoalSettingEvents;

namespace VehiclePlannerAGVBase {
    public partial class GoalSetting :BaseGoalSetting,IGoalSetting {

        #region Declaration - Events

        /// <summary>
        /// 按照順序移動全部
        /// </summary>
        public event DelRunLoop RunLoopEvent;

        #endregion Declaration - Events

        #region Funciton - Constructors

        /// <summary>
        /// 共用建構方法
        /// </summary>
        /// <param name="goalsetting">GoalSetting方法實作物件參考</param>
        /// <param name="main">主介面參考</param>
        /// <param name="defState">預設停靠方式</param>
        public GoalSetting(DockState defState = DockState.Float)
            : base(defState) {
            InitializeComponent();
        }

        #endregion Funciotn - Constructors

        #region Funciotn - Public Methods

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
        /// 重新載入標示物
        /// </summary>
        public void ReloadSingle() {
            lock (mKey) {
                ClearGoal();
                Database.GoalGM.SaftyForLoop(LoadSingle);
                Database.PowerGM.SaftyForLoop(LoadSingle);
            }
        }

        #endregion Funciotn - Public Methods

        #region Funciton - Private Methods

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

        #endregion Function - Private Methdos

        #region Function - Events

        protected override void btnRunAll_Click(object sender, EventArgs e) {
            lock (mKey) {
                RunLoopEvent?.BeginInvoke(GetGoals(), null, null);
            }
        }

        #endregion Funcitn - Events
    }
}
