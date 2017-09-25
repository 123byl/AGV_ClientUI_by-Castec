using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using MapProcessing;
using CtLib.Library;
namespace ClientUI {

    /// <summary>
    /// GoalSettinge功能接口
    /// </summary>
    public interface IGoalSetting {

        ///// <summary>
        ///// 是否與Server地圖同步
        ///// </summary>
        //bool IsAsync { get; set; }

        /// <summary>
        /// Goal點集合
        /// </summary>
        List<CartesianPos> Goals { get; }

        /// <summary>
        /// 當前Map檔路徑
        /// </summary>
        string CurMapPath { get; }

        /// <summary>
        /// 要新增的點位
        /// </summary>
        CarPos AddPos { get; set; }

        /// <summary>
        /// Goal Setting相關事件
        /// </summary>
        event GoalSettingEvent GoalSettingEvent;

        /// <summary>
        /// 新增Goal點
        /// </summary>
        /// <param name="goal">Goal點</param>
        void AddGoalPos(CarPos goal);

        /// <summary>
        /// 路徑規劃
        /// </summary>
        /// <param name="numGoal">目標Goal點編號</param>
        void PathPlan(int numGoal);

        /// <summary>
        /// 前往目標Goal點
        /// </summary>
        /// <param name="numGoal">目標Goal點編號</param>
        void Run(int numGoal);

        /// <summary>
        /// 清除所有Goal點
        /// </summary>
        void DeleteAllGoal();

        /// <summary>
        /// 清除指定索引Goal點
        /// </summary>
        /// <param name="">Goal點索引</param>
        void DeleteGoal(int index);

        /// <summary>
        /// 將所有Goal點寫入檔案
        /// </summary>
        void SaveGoals();

        /// <summary>
        /// 載入檔案
        /// </summary>
        /// <param name="type">要載入的檔案類型</param>
        void LoadFile(FileType type);

        /// <summary>
        /// 向AGV要求檔案
        /// </summary>
        /// <param name="type">檔案類型</param>
        void GetFile(FileType type);

        /// <summary>
        /// 傳送檔案
        /// </summary>
        void SendMap();
    }

    /// <summary>
    /// Goal點設定介面
    /// </summary>
    public partial class CtGoalSetting : CtDockContent<IGoalSetting> {

        #region Declaration - Fields

        /// <summary>
        /// 要新增的點位，rActFunc無參考時使用
        /// </summary>
        private CarPos mAddPos = null;

        /// <summary>
        /// 當前Map檔路徑，rActFunc無參考時使用
        /// </summary>
        private string mCurMapPath = string.Empty;

        /// <summary>
        /// Goal點集合，rActFunc無參考時使用
        /// </summary>
        private List<CartesianPos> mGoals = null;

        #endregion Declaration - Fields

        #region Declaration - Properties

        /// <summary>
        /// Goal點集合
        /// </summary>
        private List<CartesianPos> Goals {
            get {
                return rActFunc?.Goals ?? mGoals;
            }
        }

        /// <summary>
        /// 當前Map檔路徑
        /// </summary>
        private string CurMapPath { get { return rActFunc?.CurMapPath; } }

        /// <summary>
        /// 要新增的點位
        /// </summary>
        private CarPos AddPos {
            get {
                return rActFunc?.AddPos ?? mAddPos;
            }
            set {
                if (rActFunc != null) {
                    rActFunc.AddPos = value;
                } else {
                    mAddPos = value;
                }
            }
        }

        #endregion Declaration - Properties

        #region Funciton - Construcotrs

        public CtGoalSetting(DockState defState = DockState.Float) : this(null, null, defState) {

        }

        /// <summary>
        /// 傳入<see cref="IGoalSetting"/>參考進行建置
        /// </summary>
        /// <param name="goalSetting">GoalSetting方法實作物件參考</param>
        /// <param name="defState">預設停靠方式</param>
        public CtGoalSetting(IGoalSetting goalSetting, DockState defState = DockState.Float)
            : this(goalSetting, null, defState) {

        }

        /// <summary>
        /// 共用建構方法
        /// </summary>
        /// <param name="goalsetting">GoalSetting方法實作物件參考</param>
        /// <param name="main">主介面參考</param>
        /// <param name="defState">預設停靠方式</param>
        public CtGoalSetting(IGoalSetting goalsetting, AgvClientUI main, DockState defState = DockState.Float)
            : base(goalsetting, main, defState) {
            InitializeComponent();
            FixedSize = new Size(776, 860);
            if (rActFunc == null) {
                mAddPos = new CarPos();
            }
            this.Icon = null;
        }

        #endregion Funciton - Constructors

        #region Function - Events

        #region Button

        /// <summary>
        /// 新增Goal點
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewPoint_Click(object sender, EventArgs e) {
            /*-- 將Goal點資訊加入DataGridView --*/
            dgvGoalPoint.Rows.Add(new Object[] { new CheckBox().Checked = false, AddPos.x, AddPos.y, AddPos.theta, false });
            dgvGoalPoint.Rows[dgvGoalPoint.Rows.Count - 1].HeaderCell.Value = string.Format("{0}", dgvGoalPoint.Rows.Count);

            CtInvoke.ComboBoxAdd(cmbGoalList, AddPos.ToStr());

            /*-- 從底層觸發 --*/
            rActFunc.AddGoalPos(AddPos);

            ///*-- 從介面直接調用 --*/
            //rMain.AddGoalPos(AddPos);
        }

        #endregion Button

        private void rActFunc_OnGoalSettingEvent(object sender, GoalSettingEventArgs e) {
            switch (e.Type) {
                case GoalSettingEventType.LoadMap:
                    LoadMap(e.Value as List<CartesianPos>);
                    CtInvoke.ButtonEnable(btnSaveGoal, true);
                    break;
                case GoalSettingEventType.RefreshAddPos:
                    RefreshAddPos(e.Value as CarPos);
                    break;
                case GoalSettingEventType.Connect:
                    ChangedConnect((bool)e.Value);
                    break;
                case GoalSettingEventType.CurMapPath:
                    CtInvoke.ButtonEnable(btnSaveGoal, (bool)e.Value);
                    break;
            }
        }

        /// <summary>
        /// 依照連線狀態切換介面控制項狀態
        /// </summary>
        /// <param name="isConnected"></param>
        private void ChangedConnect(bool isConnected) {
            CtInvoke.ButtonEnable(btnGetMap, isConnected);
            CtInvoke.ButtonEnable(btnSendMap, isConnected);
        }

        /// <summary>
        /// 更新要新增的點位
        /// </summary>
        /// <param name="carPos"></param>
        private void RefreshAddPos(CarPos carPos) {
            CtInvoke.TextBoxText(txtAddPx, $"{carPos.x}");
            CtInvoke.TextBoxText(txtAddPy, $"{carPos.y}");
            CtInvoke.TextBoxText(txtAddPtheta, $"{carPos.theta}");
        }

        #endregion Function - Events

        #region Function - Public Methods



        #endregion Function - Public Methods

        #region Function - Private Methods

        /// <summary>
        /// 地圖載入
        /// </summary>
        /// <param name="goals"></param>
        private void LoadMap(List<CartesianPos> goals) {
            CtInvoke.DataGridViewClear(dgvGoalPoint);
            CtInvoke.ComboBoxClear(cmbGoalList);
            if (goals.Any()) {
                int idx = 1;
                foreach (CartesianPos goal in goals) {
                    CtInvoke.ComboBoxAdd(cmbGoalList, goal.ToStr());
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dgvGoalPoint);
                    row.Cells[0].Value = new CheckBox().Checked = false;
                    row.Cells[1].Value = goal.x;
                    row.Cells[2].Value = goal.y;
                    row.Cells[3].Value = goal.theta;
                    row.Cells[4].Value = false;
                    row.HeaderCell.Value = idx++.ToString();
                    dgvGoalPoint.InvokeIfNecessary(() => {
                        dgvGoalPoint.Rows.Add(row); 
                    });
                }
                CtInvoke.ComboBoxSelectedIndex(cmbGoalList, 0);
            }
        }

        /// <summary>
        /// 訂閱事件
        /// </summary>
        protected override void AddEvent() {
            if (rActFunc != null) {
                rActFunc.GoalSettingEvent += rActFunc_OnGoalSettingEvent;
            }

            TextChecker.Add(txtAddPx, str => AddPos.x = double.Parse(str));
            TextChecker.Add(txtAddPy, str => AddPos.y = double.Parse(str));
            TextChecker.Add(txtAddPtheta, str => AddPos.theta = double.Parse(str));

        }

        /// <summary>
        /// 取消訂閱事件
        /// </summary>
        protected override void RemoveEvent() {
            if (rActFunc != null) {
                rActFunc.GoalSettingEvent -= rActFunc_OnGoalSettingEvent;
            }
            TextChecker.Remove(txtAddPx, txtAddPy, txtAddPtheta);
        }

        #endregion Function - Private

        private void btnPath_Click(object sender, EventArgs e) {
            rActFunc.PathPlan(cmbGoalList.SelectedIndex);
        }

        private void btnGoGoal_Click(object sender, EventArgs e) {
            rActFunc.Run(cmbGoalList.SelectedIndex);
        }

        private void btnDelete_Click(object sender, EventArgs e) {
            int saveCount = 0;

            while (saveCount != dgvGoalPoint.Rows.Count) {
                DataGridViewRow row = dgvGoalPoint.Rows[saveCount];
                if ((bool)row.Cells[0].Value) {
                    dgvGoalPoint.Rows.Remove(row);
                    CtInvoke.ComboBoxRemove(cmbGoalList, saveCount);
                    rActFunc.DeleteGoal(saveCount);
                } else {
                    saveCount++;
                    row.HeaderCell.Value = $"{saveCount}";
                }
            }
        }

        /// <summary>
        /// 清除所有Goal點
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteAll_Click(object sender, EventArgs e) {
            CtInvoke.DataGridViewClear(dgvGoalPoint);
            CtInvoke.ComboBoxClear(cmbGoalList);
            rActFunc.DeleteAllGoal();

            rMain.DeleteAllGoal();

        }

        /// <summary>
        /// 儲存所有Goal點
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveGoal_Click(object sender, EventArgs e) {
            rActFunc.SaveGoals();
        }

        /// <summary>
        /// 載入Map檔
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadMap_Click(object sender, EventArgs e) {
            rActFunc.LoadFile(FileType.Map);
        }

        /// <summary>
        /// 從AGV要求檔案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetMap_Click(object sender, EventArgs e) {
            rActFunc.GetFile(FileType.Map);
        }

        private void btnSendMap_Click(object sender, EventArgs e) {
            rActFunc.SendMap();
        }
    }

    /// <summary>
    /// Goal Setting 事件類型
    /// </summary>
    public enum GoalSettingEventType {
        /// <summary>
        /// 載入地圖
        /// </summary>
        LoadMap,
        /// <summary>
        /// 更新要新增的點位
        /// </summary>
        RefreshAddPos,
        /// <summary>
        /// 連線狀態變更事件
        /// </summary>
        Connect,
        /// <summary>
        /// Map檔路徑變更
        /// </summary>
        CurMapPath

    }

    /// <summary>
    /// Goal Setting 事件參數
    /// </summary>
    public class GoalSettingEventArgs : EventArgs {
        /// <summary>
        /// 事件類型
        /// </summary>
        public GoalSettingEventType Type { get; }

        /// <summary>
        /// 傳遞參數
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// 一般建構方法
        /// </summary>
        /// <param name="type">事件類型</param>
        /// <param name="value">傳遞參數</param>
        public GoalSettingEventArgs(GoalSettingEventType type, object value = null) {
            this.Type = type;
            this.Value = value;
        }
    }

    /// <summary>
    /// Goal Setting 事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void GoalSettingEvent(object sender, GoalSettingEventArgs e);

}
