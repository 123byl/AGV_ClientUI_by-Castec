using AGVMap;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using static ClientUI.Events.GoalSettingEvents;
using MapProcessing;
using CtLib.Library;

namespace ClientUI
{
    /// <summary>
    /// Goal點設定介面
    /// </summary>
    public partial class CtGoalSetting : CtDockContent, IIGoalSetting
    {
        public const int IDColumn = 1;
        public const int NameColumn = 2;
        public const int SelectColumn = 0;
        public const int TowardColumn = 5;
        public const int XColumn = 3;
        public const int YColumn = 4;
        private readonly object mKey = new object();

        #region Funciton - Construcotrs

        /// <summary>
        /// 共用建構方法
        /// </summary>
        /// <param name="goalsetting">GoalSetting方法實作物件參考</param>
        /// <param name="main">主介面參考</param>
        /// <param name="defState">預設停靠方式</param>
        public CtGoalSetting(DockState defState = DockState.Float)
            : base(defState)
        {
            InitializeComponent();
            FixedSize = new Size(776, 860);
        }

        #endregion Funciton - Construcotrs
        /// <summary>
        /// 設定表單選擇項目
        /// </summary>
        public void SetSelectItem(int id)
        {
            lock (mKey)
            {
                dgvGoalPoint.InvokeIfNecessary(() =>
                {
                    for (int row = 0; row < dgvGoalPoint.RowCount; row++)
                    {
                        if ((int)dgvGoalPoint[IDColumn, row].Value == id)
                        {
                            dgvGoalPoint.Rows[row].Selected = true;
                        }
                        else
                        {
                            dgvGoalPoint.Rows[row].Selected = false;
                        }
                    }
                });
            }
        }
        #region IIGoalSetting
        /// <summary>
        /// 設定真實座標
        /// </summary>
        public void SetCurrentRealPos(IPair realPos)
        {
            lock (mKey)
            {
                txtAddPx.InvokeIfNecessary(() =>
                {
                    if (txtAddPx.Text != realPos.X.ToString()) txtAddPx.Text = realPos.X.ToString();
                });
                txtAddPy.InvokeIfNecessary(() =>
                {
                    if (txtAddPy.Text != realPos.Y.ToString()) txtAddPy.Text = realPos.Y.ToString();
                });
            }
        }
        /// <summary>
        /// 當下車子的位置
        /// </summary>
        private TowardPos mCurrentCar = new TowardPos();

        /// <summary>
        /// 加入 Goal 點
        /// </summary>
        public event DelAddNewGoal AddNewGoalEvent;

        /// <summary>
        /// 刪除
        /// </summary>
        public event DelDeleteGoals DeleteGoalsEvent;

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
        /// 當下車子的位置
        /// </summary>
        public TowardPos CurrentCar { get { return mCurrentCar; } set { if (value != null) mCurrentCar = value; } }

        /// <summary>
        /// 目標點個數
        /// </summary>
        public int GoalCount { get { lock (mKey) return dgvGoalPoint.Rows.Count; } }

        /// <summary>
        /// 加入 Goal 點
        /// </summary>
        public void AddGoal(Info goal)
        {
            lock (mKey)
            {
                int index = FindIndexByID(goal.ID);
                if (index == -1)
                {
                    dgvGoalPoint.InvokeIfNecessary(
                        () => dgvGoalPoint.Rows.Add(new object[] { new CheckBox().Checked = false, goal.ID, goal.Name, goal.X, goal.Y, goal.Toward.ToStr() }));
                }
                else
                {
                    dgvGoalPoint.InvokeIfNecessary(
                        () =>
                        {
                            if ((int)dgvGoalPoint[IDColumn, index].Value != goal.ID) dgvGoalPoint[IDColumn, index].Value = goal.ID;
                            if ((string)dgvGoalPoint[NameColumn, index].Value != goal.Name) dgvGoalPoint[NameColumn, index].Value = goal.Name;
                            if ((int)dgvGoalPoint[XColumn, index].Value != goal.X) dgvGoalPoint[XColumn, index].Value = goal.X;
                            if ((int)dgvGoalPoint[YColumn, index].Value != goal.Y) dgvGoalPoint[YColumn, index].Value = goal.Y;
                            if ((string)dgvGoalPoint[TowardColumn, index].Value != goal.Toward.ToStr()) dgvGoalPoint[TowardColumn, index].Value = goal.Toward.ToStr();
                        });
                }
            }
        }

        /// <summary>
        /// 移除目前 Goal 點並加入新的 goal 點
        /// </summary>
        public void ClearAndAddGoals(IEnumerable<Info> goals)
        {
            lock (mKey)
            {
                ClearGoal();
                foreach (var goal in goals)
                {
                    AddGoal(goal);
                }
            }
        }

        /// <summary>
        /// 移除所有 Goal 點
        /// </summary>
        public void ClearGoal()
        {
            lock (mKey)
            {
                dgvGoalPoint.InvokeIfNecessary(() => dgvGoalPoint.Rows.Clear());
            }
        }

        /// <summary>
        /// 根據 ID 移除 Goal 點
        /// </summary>
        public void DeleteGoal(int ID)
        {
            lock (mKey)
            {
                int row = FindIndexByID(ID);
                if (row != -1) dgvGoalPoint.InvokeIfNecessary(() => dgvGoalPoint.Rows.RemoveAt(row));
            }
        }

        /// <summary>
        /// 根據 ID 移除 Goal 點
        /// </summary>
        public void DeleteGoals(IEnumerable<int> ID)
        {
            lock (mKey)
            {
                foreach (var id in ID)
                {
                    DeleteGoal(id);
                }
            }
        }

        /// <summary>
        /// 用 ID 尋找 Goal 點所在的引索位置
        /// </summary>
        public int FindIndexByID(int ID)
        {
            lock (mKey)
            {
                for (int row = 0; row < dgvGoalPoint.Rows.Count; ++row)
                {
                    if ((int)(dgvGoalPoint[IDColumn, row].Value) == ID) return row;
                }
                return -1;
            }
        }

        /// <summary>
        /// 根據 ID 查詢 Goal 點
        /// </summary>
        public Info GetGoalByID(int ID)
        {
            lock (mKey)
            {
                int row = FindIndexByID(ID);
                return GetGoalByIndex(row);
            }
        }

        /// <summary>
        /// 根據表單的列編號查詢 Goal
        /// </summary>
        public Info GetGoalByIndex(int row)
        {
            lock (mKey)
            {
                if (row < 0 || row >= GoalCount) return default(Info);

                int id = 0;
                string name = string.Empty;
                int x = 0;
                int y = 0;
                double toward = 0.0;
                dgvGoalPoint.InvokeIfNecessary(() =>
                {
                    id = (int)dgvGoalPoint[IDColumn, row].Value;
                    name = (string)dgvGoalPoint[NameColumn, row].Value;
                    x = (int)dgvGoalPoint[XColumn, row].Value;
                    y = (int)dgvGoalPoint[YColumn, row].Value;
                    toward = (double)dgvGoalPoint[TowardColumn, row].Value;
                });
                return new Info(id, name, x, y, toward);
            }
        }

        /// <summary>
        /// 獲得所有 Goal 點資訊
        /// </summary>
        public List<Info> GetGoals()
        {
            lock (mKey)
            {
                List<Info> list = new List<Info>();
                for (int row = 0; row < GoalCount; ++row)
                {
                    int id = 0;
                    string name = string.Empty;
                    int x = 0;
                    int y = 0;
                    double toward = 0.0;
                    dgvGoalPoint.InvokeIfNecessary(() =>
                    {
                        id = (int)dgvGoalPoint[IDColumn, row].Value;
                        name = (string)dgvGoalPoint[NameColumn, row].Value;
                        x = (int)dgvGoalPoint[XColumn, row].Value;
                        y = (int)dgvGoalPoint[YColumn, row].Value;
                        toward = (double)dgvGoalPoint[TowardColumn, row].Value;
                    });
                    if (id != 0) list.Add(new Info(id, name, x, y, toward));
                }
                return list;
            }
        }
        /// <summary>
        /// 獲得所有被選取的 Goal 點資訊
        /// </summary>
        public List<Info> GetSelectedGoals()
        {
            lock (mKey)
            {
                List<Info> list = new List<Info>();
                for (int row = 0; row < GoalCount; ++row)
                {
                    int id = 0;
                    string name = string.Empty;
                    int x = 0;
                    int y = 0;
                    double toward = 0.0;
                    dgvGoalPoint.InvokeIfNecessary(() =>
                    {
                        if ((bool)dgvGoalPoint[SelectColumn, row].Value)
                        {
                            id = (int)dgvGoalPoint[IDColumn, row].Value;
                            name = (string)dgvGoalPoint[NameColumn, row].Value;
                            x = (int)dgvGoalPoint[XColumn, row].Value;
                            y = (int)dgvGoalPoint[YColumn, row].Value;
                            toward = double.Parse((string)dgvGoalPoint[TowardColumn, row].Value);
                        }
                    });
                    if (id != 0) list.Add(new Info(id, name, x, y, toward));
                }
                return list;
            }
        }
        /// <summary>
        /// 清除所有目標點
        /// </summary>
        public event DelClearGoals ClearGoalsEvent;

        #endregion IIGoalSetting

        #region UI Event

        private void btnCurrPos_Click(object sender, EventArgs e)
        {
            int id = Factory.CreatID.NewID;
            Info goal = new Info(id, "Goal" + id, CurrentCar);
            AddNewGoalEvent?.Invoke(goal);
        }

        private void btnGetMap_Click(object sender, EventArgs e)
        {
            LoadMapFromAGVEvent?.Invoke();
        }

        private void btnLoadMap_Click(object sender, EventArgs e)
        {
            LoadMapEvent?.Invoke();
        }

        private void btnNewPoint_Click(object sender, EventArgs e)
        {
            int id = Factory.CreatID.NewID;
            int x; int.TryParse(txtAddPx.Text, out x);
            int y; int.TryParse(txtAddPy.Text, out y);
            double toward; double.TryParse(txtAddPtheta.Text, out toward);
            Info goal = new Info(id, "Goal" + id, x, y, toward);
            AddNewGoalEvent?.Invoke(goal);
        }

        private void btnPath_Click(object sender, EventArgs e)
        {

            Info goal = GetGoalByIndex(cmbGoalList.SelectedIndex);
            if (goal.ID != 0) FindPathEvent?.Invoke(goal, cmbGoalList.SelectedIndex);
        }

        private void btnSendMap_Click(object sender, EventArgs e)
        {
            SendMapToAGVEvent?.Invoke();
        }

        private void btnGoGoal_Click(object sender, EventArgs e)
        {
            Info goal = default(Info);
            lock (mKey)
            {
                goal = GetGoalByIndex(cmbGoalList.SelectedIndex);
            }
            if (goal.ID != 0) RunGoalEvent?.Invoke(goal, cmbGoalList.SelectedIndex);
        }
        private void btnRunAll_Click(object sender, EventArgs e)
        {
            List<Info> goal = new List<Info>();
            lock (mKey)
            {
                goal = GetGoals();
            }
            if (goal.Count != 0) RunLoopEvent?.Invoke(goal);
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            List<Info> goal = new List<Info>();
            lock (mKey)
            {
                goal = GetSelectedGoals();
            }
            if (goal.Count != 0) DeleteGoalsEvent?.Invoke(goal);
        }
        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            ClearGoalsEvent?.Invoke();
        }

        private void btnSaveGoal_Click(object sender, EventArgs e)
        {
            SaveGoalEvent?.Invoke();
        }

        /// <summary>
        /// 地圖載入
        /// </summary>
        /// <param name="goals"></param>
        public void LoadGoals(List<Info> goals)
        {
            lock (mKey)
            {
                ClearGoal();
                foreach (var item in goals)
                {
                    AddGoal(item);
                }
            }
        }
        #endregion UI Event
    }
}
