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
        protected BaseGoalSetting():base() {
            InitializeComponent();
        }

        /// <summary>
        /// 共用建構方法
        /// </summary>
        /// <param name="goalsetting">GoalSetting方法實作物件參考</param>
        /// <param name="main">主介面參考</param>
        /// <param name="defState">預設停靠方式</param>
        public BaseGoalSetting(BaseVehiclePlanner_Ctrl refUI, DockState defState = DockState.Float)
            : base(refUI,defState) {
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

        protected virtual void btnRunAll_Click(object sender, EventArgs e) {
            
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
        protected int FindIndexByID(uint ID) {
            lock (mKey) {
                for (int row = 0; row < dgvGoalPoint.Rows.Count; ++row) {
                    if ((uint)(dgvGoalPoint[IDColumn, row].Value) == ID) return row;
                }
                return -1;
            }
        }

        public override bool IsVisiable(AccessLevel lv) {
            return lv > AccessLevel.None;
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