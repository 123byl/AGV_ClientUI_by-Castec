using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using static VehiclePlanner.Events.GoalSettingEvents;
using CtLib.Library;
using GLCore;
using Geometry;
using CtLib.Forms;

namespace VehiclePlanner
{
    /// <summary>
    /// Goal點設定介面
    /// </summary>
    public partial class CtGoalSetting : CtDockContent, IIGoalSetting
    {

        #region Declaration - Fields

        private readonly object mKey = new object();
       
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
        public IGoal GetGoalByID(uint ID)
        {
            lock (mKey)
            {
                int row = FindIndexByID(ID);
                return GetSingleByIndex<IGoal>(row);
            }
        }

        /// <summary>
        /// 根據表單的列編號查詢 Goal
        /// </summary>
        public T GetSingleByIndex<T>(int row)where T:ISingle<ITowardPair>
        {
            lock (mKey)
            {
                T single = default(T);
                if (row >= 0 && row < GoalCount) {
                    uint id = 0;
                    dgvGoalPoint.InvokeIfNecessary(() =>
                    {
                        id = (uint)dgvGoalPoint[IDColumn, row].Value;
                    });
                    if (Database.GoalGM.ContainsID(id)) {
                        if (!Database.GoalGM.ContainsID(id)) throw new Exception($"GoalGM中不存在{id}");
                        single = (T)Database.GoalGM[id];
                    }else if (Database.PowerGM.ContainsID(id)) {
                        if (!Database.PowerGM.ContainsID(id)) throw new Exception($"PowerGM中不存在{id}");
                        single = (T)Database.PowerGM[id];
                    }else {
                        throw new Exception($"未知的標記形態{typeof(T).Name}");
                    }
                }
                return single;
            }
        }
        
        public uint GetSelectedID() {
            lock (mKey) {
                uint id = uint.MaxValue;
                int row = cmbGoalList.SelectedIndex;
                if (row >= 0 && row < GoalCount) {                    
                    dgvGoalPoint.InvokeIfNecessary(() => {
                        id = (uint)dgvGoalPoint[IDColumn, row].Value;
                    });
                }
                return id;
            }
        }

        /// <summary>
        /// 獲得所有 Goal 點資訊
        /// </summary>
        private List<IGoal> GetGoals()
        {
            lock (mKey)
            {
                var list = new List<IGoal>();
                for (int row = 0; row < GoalCount; ++row)
                {
                    uint id = 0;
                    dgvGoalPoint.InvokeIfNecessary(() =>
                    {
                        id = Convert.ToUInt32(dgvGoalPoint[IDColumn, row].Value);
                    });
                    if (Database.GoalGM.ContainsID(id)) list.Add(Database.GoalGM[id]);
                }
                return list;
            }
        }

        /// <summary>
        /// 獲得所有被選取的 Goal 點資訊
        /// </summary>
        private List<uint> GetSelectedSingleID()
        {
            lock (mKey)
            {
                var list = new List<uint>();
                for (int row = 0; row < GoalCount; ++row)
                {
                    dgvGoalPoint.InvokeIfNecessary(() =>
                    {
                        bool isSelected = (bool)dgvGoalPoint[SelectColumn, row].Value;
                        if (isSelected)
                        {
                            uint id = (uint)dgvGoalPoint[IDColumn, row].Value;
                            list.Add(id);
                        }
                    });
                }
                return list;
            }
        }
        
        /// <summary>
        /// 設定真實座標
        /// </summary>
        public void SetCurrentRealPos(IPair realPos) {
            lock (mKey) {
                txtAddPx.InvokeIfNecessary(() => {
                    if (txtAddPx.Text != realPos.X.ToString()) txtAddPx.Text = realPos.X.ToString();
                });
                txtAddPy.InvokeIfNecessary(() => {
                    if (txtAddPy.Text != realPos.Y.ToString()) txtAddPy.Text = realPos.Y.ToString();
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
            //CtInvoke.ControlEnabled(btnPath, enb);
            //CtInvoke.ControlEnabled(btnRunAll, enb);
            //CtInvoke.ControlEnabled(btnRun, enb);
            //CtInvoke.ControlEnabled(btnCharging, enb);
        }

        #endregion IIGoalSetting

        #region UI Event

        #region Button

        private void btnGetGoalList_Click(object sender, EventArgs e)
        {
            lock (mKey) {
                GetGoalNames.Invoke();
            }
        }
        
        private void btnCurrPos_Click(object sender, EventArgs e)
        {
            lock (mKey) {
                AddNewGoalEvent?.Invoke(Database.AGVGM[0].Data);
            }
        }

        private void btnGetMap_Click(object sender, EventArgs e)
        {
            lock (mKey) {
                LoadMapFromAGVEvent?.BeginInvoke(null,null);
            }
        }

        private void btnLoadMap_Click(object sender, EventArgs e)
        {
            lock (mKey) {
                LoadMapEvent?.Invoke();
            }
        }
        
        private void btnPath_Click(object sender, EventArgs e)
        {
            lock (mKey) {
                var goal = GetSingleByIndex<IGoal>(cmbGoalList.SelectedIndex);
                uint id = GetSelectedID();
                int index = Database.GoalGM.IndexOf(id);
                FindPathEvent?.BeginInvoke(goal, index,null,null);
            }
        }

        private void btnSendMap_Click(object sender, EventArgs e)
        {
            lock (mKey) {
                SendMapToAGVEvent?.Invoke();
            }
        }

        private void btnGoGoal_Click(object sender, EventArgs e)
        {
            lock (mKey)
            {
                var goal = GetSingleByIndex<IGoal>(cmbGoalList.SelectedIndex);
                uint id = GetSelectedID();
                int index = Database.GoalGM.IndexOf(id);
                RunGoalEvent?.BeginInvoke(goal, index,null,null);
            }
        }

        private void btnRunAll_Click(object sender, EventArgs e)
        {
            lock (mKey)
            {
                RunLoopEvent?.BeginInvoke(GetGoals(),null,null);
            } 
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            lock (mKey)
            {
                DeleteSingleEvent?.Invoke(GetSelectedSingleID());
            }
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            lock (mKey) {
                ClearGoalsEvent?.Invoke();
            }
        }

        private void btnSaveGoal_Click(object sender, EventArgs e)
        {
            lock (mKey) {
                SaveGoalEvent?.Invoke();
            }
        }

        private void btnCharging_Click(object sender, EventArgs e) {
            lock (mKey) {
                var power = GetSingleByIndex<IPower>(cmbGoalList.SelectedIndex);
                uint id = GetSelectedID();
                int index = Database.PowerGM.IndexOf(id);
                Charging?.BeginInvoke(power, index,null,null);
            }
        }

        private void btnClear_Click(object sender, EventArgs e) {
            lock (mKey) {
                ClearMap?.Invoke();
            }
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

        #endregion Function - Private Methods

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
