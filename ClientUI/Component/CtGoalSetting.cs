using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using static ClientUI.Events.GoalSettingEvents;
using MapProcessing;
using CtLib.Library;
using GLCore;
using Geometry;

namespace ClientUI
{
    /// <summary>
    /// Goal點設定介面
    /// </summary>
    public partial class CtGoalSetting : CtDockContent, IIGoalSetting
    {

        #region Declaration - Fields

        private readonly object mKey = new object();

        /// <summary>
        /// 當下車子的位置
        /// </summary>
        private CartesianPos mCurrentCar = new CartesianPos();

        #endregion Declaration - Fiedls

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
            : base(defState)
        {
            InitializeComponent();
            FixedSize = new Size(776, 860);
        }

        #endregion Funciton - Construcotrs
        
        #region Implement - IIGoalSetting

        /// <summary>
        /// 加入 Goal 點
        /// </summary>
        public event DelAddNewGoal AddNewGoalEvent;

        /// <summary>
        /// 清除所有目標點
        /// </summary>
        public event DelClearGoals ClearGoalsEvent;
        
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
        /// 取得所有Goal點名稱
        /// </summary>
        public event DelGetGoalNames GetGoalNames;
        
        public event DelCharging Charging;
        
        public event Events.TestingEvents.DelClearMap ClearMap;
        
        /// <summary>
        /// 當下車子的位置
        /// </summary>
        public CartesianPos CurrentCar { get { return mCurrentCar; } set { if (value != null) mCurrentCar = value; } }
        
        /// <summary>
        /// 目標點個數
        /// </summary>
        public int GoalCount { get { lock (mKey) return dgvGoalPoint.Rows.Count; } }

        /// <summary>
        /// 加入 Goal 點
        /// </summary>
        public void AddGoal(CartesianPosInfo goal)
        {
            lock (mKey)
            {
                int index = FindIndexByID(goal.id);
                if (!cmbGoalList.Items.Contains(goal.name)) {
                    cmbGoalList.InvokeIfNecessary(() => cmbGoalList.Items.Add(goal.name));
                }

                if (index == -1)
                {
                    dgvGoalPoint.InvokeIfNecessary(
                        () => dgvGoalPoint.Rows.Add(new object[] { new CheckBox().Checked = false, goal.id, goal.name, goal.x, goal.y, goal.theta.ToString("F2") }));
                }
                else
                {
                    dgvGoalPoint.InvokeIfNecessary(
                        () =>
                        {
                            if ((uint)dgvGoalPoint[IDColumn, index].Value != goal.id) dgvGoalPoint[IDColumn, index].Value = goal.id;
                            if ((string)dgvGoalPoint[NameColumn, index].Value != goal.name) dgvGoalPoint[NameColumn, index].Value = goal.name;
                            if ((double)dgvGoalPoint[XColumn, index].Value != goal.x) dgvGoalPoint[XColumn, index].Value = goal.x;
                            if ((double)dgvGoalPoint[YColumn, index].Value != goal.y) dgvGoalPoint[YColumn, index].Value = goal.y;
                            if ((string)dgvGoalPoint[TowardColumn, index].Value != goal.theta.ToString("F2")) dgvGoalPoint[TowardColumn, index].Value = goal.theta.ToString("F2");
                        });
                }
            }
        }

        /// <summary>
        /// 加入 Goal 點
        /// </summary>
        public void AddPower(CartesianPosInfo power) {
            lock (mKey) {
                int index = FindIndexByID(power.id);
                if (index == -1) {
                    dgvGoalPoint.InvokeIfNecessary(
                        () => dgvGoalPoint.Rows.Add(new object[] { new CheckBox().Checked = false, power.id, power.name, power.x, power.y, power.theta.ToString("F2") }));
                } else {
                    dgvGoalPoint.InvokeIfNecessary(
                        () => {
                            if ((uint)dgvGoalPoint[IDColumn, index].Value != power.id) dgvGoalPoint[IDColumn, index].Value = power.id;
                            if ((string)dgvGoalPoint[NameColumn, index].Value != power.name) dgvGoalPoint[NameColumn, index].Value = power.name;
                            if ((double)dgvGoalPoint[XColumn, index].Value != power.x) dgvGoalPoint[XColumn, index].Value = power.x;
                            if ((double)dgvGoalPoint[YColumn, index].Value != power.y) dgvGoalPoint[YColumn, index].Value = power.y;
                            if ((string)dgvGoalPoint[TowardColumn, index].Value != power.theta.ToString("F2")) dgvGoalPoint[TowardColumn, index].Value = power.theta.ToString("F2");
                        });
                }
            }
        }

        /// <summary>
        /// 移除目前 Goal 點並加入新的 goal 點
        /// </summary>
        public void ClearAndAddGoals(IEnumerable<CartesianPosInfo> goals)
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
                cmbGoalList.InvokeIfNecessary(() => {
                    cmbGoalList.Items.Clear();
                    cmbGoalList.SelectedIndex = ListBox.NoMatches;
                });
            }
        }

        /// <summary>
        /// 根據 ID 移除 Goal 點
        /// </summary>
        public void DeleteGoal(uint ID)
        {
            lock (mKey)
            {
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
        public void DeleteGoals(IEnumerable<uint> ID)
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
        public int FindIndexByID(uint ID)
        {
            lock (mKey)
            {
                for (int row = 0; row < dgvGoalPoint.Rows.Count; ++row)
                {
                    if ((uint)(dgvGoalPoint[IDColumn, row].Value) == ID) return row;
                }
                return -1;
            }
        }

        /// <summary>
        /// 根據 ID 查詢 Goal 點
        /// </summary>
        public CartesianPosInfo GetGoalByID(uint ID)
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
        public CartesianPosInfo GetGoalByIndex(int row)
        {
            lock (mKey)
            {
                if (row < 0 || row >= GoalCount) return null;

                uint id = 0;
                string name = string.Empty;
                int x = 0;
                int y = 0;
                double toward = 0.0;
                dgvGoalPoint.InvokeIfNecessary(() =>
                {
                    id = (uint)dgvGoalPoint[IDColumn, row].Value;
                    name = (string)dgvGoalPoint[NameColumn, row].Value;
                    x = Convert.ToInt32(dgvGoalPoint[XColumn, row].Value);
                    y = Convert.ToInt32(dgvGoalPoint[YColumn, row].Value);
                    toward = Convert.ToDouble(dgvGoalPoint[TowardColumn, row].Value);
                });
                return new CartesianPosInfo(x, y, toward, name, id);
            }
        }

        /// <summary>
        /// 獲得所有 Goal 點資訊
        /// </summary>
        public List<CartesianPosInfo> GetGoals()
        {
            lock (mKey)
            {
                List<CartesianPosInfo> list = new List<CartesianPosInfo>();
                for (int row = 0; row < GoalCount; ++row)
                {
                    uint id = 0;
                    string name = string.Empty;
                    double x=0d,y=0d, toward = 0.0;
                    dgvGoalPoint.InvokeIfNecessary(() =>
                    {
                        string type = dgvGoalPoint[TowardColumn, row].Value.GetType().Name;
                        id = Convert.ToUInt32(dgvGoalPoint[IDColumn, row].Value);
                        name = (string)dgvGoalPoint[NameColumn, row].Value;
                        x = Convert.ToDouble(dgvGoalPoint[XColumn, row].Value);
                        y = Convert.ToDouble(dgvGoalPoint[YColumn, row].Value);
                        toward = Convert.ToDouble(dgvGoalPoint[TowardColumn, row].Value.ToString());
                    });
                    if (id != 0) list.Add(new CartesianPosInfo(x, y, toward, name, id));
                }
                return list;
            }
        }

        /// <summary>
        /// 獲得所有被選取的 Goal 點資訊
        /// </summary>
        public List<CartesianPosInfo> GetSelectedGoals()
        {
            lock (mKey)
            {
                List<CartesianPosInfo> list = new List<CartesianPosInfo>();
                for (int row = 0; row < GoalCount; ++row)
                {
                    uint id = 0;
                    string name = string.Empty;
                    double x = 0;
                    double y = 0;
                    double toward = 0.0;
                    dgvGoalPoint.InvokeIfNecessary(() =>
                    {
                        if ((bool)dgvGoalPoint[SelectColumn, row].Value)
                        {
                            id = (uint)dgvGoalPoint[IDColumn, row].Value;
                            name = (string)dgvGoalPoint[NameColumn, row].Value;
                            x = Convert.ToDouble(dgvGoalPoint[XColumn, row].Value);
                            y = Convert.ToDouble(dgvGoalPoint[YColumn, row].Value);
                            toward = Convert.ToDouble(dgvGoalPoint[TowardColumn, row].Value);
                        }
                    });
                    if (id != 0) list.Add(new CartesianPosInfo(x, y, toward, name, id));
                }
                return list;
            }
        }
        
        /// <summary>
        /// 設定真實座標
        /// </summary>
        public void SetCurrentRealPos(CartesianPos realPos) {
            lock (mKey) {
                txtAddPx.InvokeIfNecessary(() => {
                    if (txtAddPx.Text != realPos.x.ToString()) txtAddPx.Text = realPos.x.ToString();
                });
                txtAddPy.InvokeIfNecessary(() => {
                    if (txtAddPy.Text != realPos.y.ToString()) txtAddPy.Text = realPos.y.ToString();
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

        /// <summary>
        /// 解鎖與路徑相關操作
        /// </summary>
        /// <param name="enb"></param>
        public void EnableGo(bool enb = true) {
            CtInvoke.ControlEnabled(btnPath, enb);
            CtInvoke.ControlEnabled(btnRunAll, enb);
            CtInvoke.ControlEnabled(btnRun, enb);
            CtInvoke.ControlEnabled(btnCharging, enb);
        }

        public void RefreshSingle() {
            Dictionary<uint, int> uidMapping = new Dictionary<uint, int>();
            for (int idx = 0; idx < dgvGoalPoint.RowCount; idx++) {
                uidMapping.Add(Convert.ToUInt32(dgvGoalPoint[IDColumn, idx].Value), idx);
            }
            Database.GoalGM.SaftyForLoop((uid, goal) => {
                UpdataSingle(uid, goal, uidMapping);
                uidMapping.Remove(uid);
            });
            Database.PowerGM.SaftyForLoop((uid, power) => {
                UpdataSingle(uid, power, uidMapping);
                uidMapping.Remove(uid);
            });

            foreach (uint uid in uidMapping.Keys) {
                DeleteGoal(uid);
            }

        }

        #endregion IIGoalSetting

        #region UI Event

        #region Button

        private void btnGetGoalList_Click(object sender, EventArgs e)
        {
            GetGoalNames.Invoke();
        }
        
        private void btnCurrPos_Click(object sender, EventArgs e)
        {
            uint id = Database.ID.GenerateID();
            CartesianPosInfo goal = new CartesianPosInfo(CurrentCar.x, CurrentCar.y, CurrentCar.theta, "Goal" + id, id);
            AddNewGoalEvent?.Invoke();
        }

        private void btnGetMap_Click(object sender, EventArgs e)
        {
            LoadMapFromAGVEvent?.Invoke();
        }

        private void btnLoadMap_Click(object sender, EventArgs e)
        {
            LoadMapEvent?.Invoke();
        }
        
        private void btnPath_Click(object sender, EventArgs e)
        {
            CartesianPosInfo goal = GetGoalByIndex(cmbGoalList.SelectedIndex);
            if (goal != null) {
                int index = Database.GoalGM.IndexOf(goal.id);
                if (index >= 0) {
                    FindPathEvent?.Invoke(goal, index);
                }
            }
        }

        private void btnSendMap_Click(object sender, EventArgs e)
        {
            SendMapToAGVEvent?.Invoke();
        }

        private void btnGoGoal_Click(object sender, EventArgs e)
        {
            lock (mKey)
            {
                CartesianPosInfo goal = GetGoalByIndex(cmbGoalList.SelectedIndex);
                if (goal != null) {
                    int index = Database.GoalGM.IndexOf(goal.id);
                    if (index >= 0) {
                        RunGoalEvent?.Invoke(goal, index);
                    }
                }
            }
        }

        private void btnRunAll_Click(object sender, EventArgs e)
        {
            List<CartesianPosInfo> goal = new List<CartesianPosInfo>();
            lock (mKey)
            {
                goal = GetGoals();
            }
            if (goal.Count != 0) RunLoopEvent?.Invoke(goal);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            List<CartesianPosInfo> goal = new List<CartesianPosInfo>();
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

        private void btnCharging_Click(object sender, EventArgs e) {
            CartesianPosInfo power = GetGoalByIndex(cmbGoalList.SelectedIndex);
            if (power != null) {
                int index = Database.PowerGM.IndexOf(power.id);
                if (index >= 0) {
                    Charging?.Invoke(power, index);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e) {
            ClearMap?.Invoke();
        }

        #endregion Button
        
        #endregion UI Event

        #region Functin - Public Methods

        #endregion Funtion - Public Methods

        #region Fucnction - Private Methods

        /// <summary>
        /// 載入標示物
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <param name="goal"></param>
        private void LoadSingle<T>(uint uid, ISingle<T> goal) where T : ITowardPair {
            int index = FindIndexByID(uid);
            if (!cmbGoalList.Items.Contains(goal.Name)) {
                cmbGoalList.InvokeIfNecessary(() => cmbGoalList.Items.Add(goal.Name));
            }

            if (index == -1) {
                dgvGoalPoint.InvokeIfNecessary(
                    () => dgvGoalPoint.Rows.Add(new object[] { new CheckBox().Checked = false, uid, goal.Name, goal.Data.Position.X, goal.Data.Position.Y, goal.Data.Toward.Theta.ToString("F2") }));
            } else {
                dgvGoalPoint.InvokeIfNecessary(
                    () => {
                        if ((uint)dgvGoalPoint[IDColumn, index].Value != uid) dgvGoalPoint[IDColumn, index].Value = uid;
                        if ((string)dgvGoalPoint[NameColumn, index].Value != goal.Name) dgvGoalPoint[NameColumn, index].Value = goal.Name;
                        if ((double)dgvGoalPoint[XColumn, index].Value != goal.Data.Position.X) dgvGoalPoint[XColumn, index].Value = goal.Data.Position.X;
                        if ((double)dgvGoalPoint[YColumn, index].Value != goal.Data.Position.X) dgvGoalPoint[YColumn, index].Value = goal.Data.Position.Y;
                        if ((string)dgvGoalPoint[TowardColumn, index].Value != goal.Data.Toward.Theta.ToString("F2")) dgvGoalPoint[TowardColumn, index].Value = goal.Data.Toward.Theta.ToString("F2");
                    });
            }
        }

        #endregion Function - Private Methods

        private void btnRefresh_Click(object sender, EventArgs e) {
            RefreshSingle();
        }

        private void UpdataSingle(uint uid,ISingle<ITowardPair> single,Dictionary<uint,int> mapping) {
            double x = single.Data.Position.X;
            double y = single.Data.Position.Y;
            double theta = single.Data.Toward.Theta;
            if (mapping.ContainsKey(uid)) {
                int index = mapping[uid];
                dgvGoalPoint.InvokeIfNecessary(
                    () => {
                        if ((uint)dgvGoalPoint[IDColumn, index].Value != uid) dgvGoalPoint[IDColumn, index].Value = uid;
                        if ((string)dgvGoalPoint[NameColumn, index].Value != single.Name) dgvGoalPoint[NameColumn, index].Value = single.Name;
                        if ((double)dgvGoalPoint[XColumn, index].Value != x) dgvGoalPoint[XColumn, index].Value = x;
                        if ((double)dgvGoalPoint[YColumn, index].Value != y) dgvGoalPoint[YColumn, index].Value = y;
                        if ((string)dgvGoalPoint[TowardColumn, index].Value != theta.ToString("F2")) dgvGoalPoint[TowardColumn, index].Value = theta.ToString("F2");
                    });
            } else {
                dgvGoalPoint.InvokeIfNecessary(
                () => dgvGoalPoint.Rows.Add(new object[] { new CheckBox().Checked = false, uid, single.Name, x, y, theta.ToString("F2") }));
                cmbGoalList.InvokeIfNecessary(() => {
                    cmbGoalList.Items.Add(single.Name);
                });
            }
        }
    }

    /// <summary>
    /// 鼠標模式
    /// </summary>
    public enum CursorMode {
        /// <summary>
        /// 選擇模式
        /// </summary>
        Select,
        /// <summary>
        /// 新增Goal點模式
        /// </summary>
        Goal,
        /// <summary>
        /// 新增充電站模式
        /// </summary>
        Power,
        /// <summary>
        /// 拖曳模式
        /// </summary>
        Drag,
        /// <summary>
        /// 畫筆模式
        /// </summary>
        Pen,
        /// <summary>
        /// 橡皮擦模式
        /// </summary>
        Eraser,
        /// <summary>
        /// 地圖插入模式
        /// </summary>
        Insert,
        /// <summary>
        /// 禁止區模式
        /// </summary>
        ForbiddenArea
    }
}
