using CtDockSuit;
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
using VehiclePlanner;
using VehiclePlanner.Forms;
using VehiclePlanner.Module.Interface;
using VehiclePlanner.Partial.VehiclePlannerUI;
using WeifenLuo.WinFormsUI.Docking;
using static VehiclePlanner.Partial.VehiclePlannerUI.Events.GoalSettingEvents;
using VehiclePlanner.Module.Implement;

namespace VehiclePlannerAGVBase {

    public partial class CtVehiclePlanner_Ctrl : BaseVehiclePlanner_Ctrl {

        #region Declaration - Fields

        /// <summary>
        /// Car Position 設定位置
        /// </summary>
        private IPair mNewPos = null;

        /// <summary>
        /// 地圖插入控制器
        /// </summary>
        private CtDockContainer mMapInsert = new CtMapInsert();

        #endregion Declaration - Fields
        
        #region Declaration - Properties

        /// <summary>
        /// MapGL子視窗
        /// </summary>
        private IMapGL MapGL 
            => mDockContent.ContainsKey(miMapGL) ? mDockContent[miMapGL] as IMapGL : null;
            
        private IGoalSetting mGoalSetting 
            => mDockContent.ContainsKey(miGoalSetting) ? mDockContent[miGoalSetting] as IGoalSetting : null;

        protected  IScene IMapCtrl {
            get {
                return MapGL?.Ctrl;
            }
        }

        private IVehiclePlanner rVehiclePlanner = null; 

        #endregion Declaration - Properties

        #region Funciton - Constructors

        public CtVehiclePlanner_Ctrl(IVehiclePlanner vehiclePlanner):base(vehiclePlanner) {
            InitializeComponent();
            rVehiclePlanner = vehiclePlanner;
            SetEvents();
        }

        #endregion Funciton - Constructors

        #region Funciton - Private Methods

        protected override BaseMapGL GetMapGL(DockState dockState) {
            return new MapGL(dockState);
        }

        protected override BaseGoalSetting GetGoalSetting(DockState dockState) {
            return new GoalSetting(dockState);
        }

        private void SetEvents() {

            mGoalSetting.ClearGoalsEvent += rVehiclePlanner.ClearMarker;
            mGoalSetting.DeleteSingleEvent += rVehiclePlanner.DeleteMarker;
            mGoalSetting.RunLoopEvent += IGoalSetting_RunLoopEvent;

            IMapCtrl.GLClickEvent += IMapCtrl_GLClickEvent;
            IMapCtrl.DragTowerPairEvent += IMapCtrl_DragTowerPairEvent;
            IMapCtrl.GLMoveUp += IMapCtrl_GLMoveUp;
        }


        protected override void LoadICtDockContainer() {
            base.LoadICtDockContainer();
            mMapInsert.AssignmentDockPanel(dockPanel);
        }

        protected override void MarkerChanged() {
            mGoalSetting.ReloadSingle();
        }

        #endregion Funciton - Private Methods

        #region Function - Events


        #region IMapGL事件連結

        protected void IMapCtrl_GLClickEvent(object sender, GLMouseEventArgs e) {
            if (mIsSetting) {
                if (mNewPos == null) {
                    mNewPos = e.Position;
                } else {
                    OnConsoleMessage($"NewPos{mNewPos.ToString()}");
                    Task.Run(() => {
                        rVehiclePlanner.Controller.SetPosition(e.Position, mNewPos);
                        mNewPos = null;
                        mIsSetting = false;
                    });
                }
            }
            //顯示滑鼠點擊的座標
            mGoalSetting.UpdateNowPosition(e.Position);
        }

        protected void IMapCtrl_DragTowerPairEvent(object sender, TowerPairEventArgs e) {
            mGoalSetting.ReloadSingle();
        }

        /// <summary>
        /// MapGL滑鼠放開事件處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void IMapCtrl_GLMoveUp(object sender, GLMouseEventArgs e) {
            switch (mCursorMode) {
                case CursorMode.Goal:
                case CursorMode.Power:
                    mGoalSetting.ReloadSingle();
                    mCursorMode = CursorMode.Select;
                    break;
            }
        }

        #endregion IMapGL事件連結


        #region IGoalSetting 事件連結

        private void IGoalSetting_RunLoopEvent(IEnumerable<IGoal> goal) {
            //int goalCount = goal?.Count() ?? -1;
            //if (goalCount > 0) {
            //    mSimilarityFlow.CheckFlag("Run all", () => {
            //        OnConsoleMessage("[AGV Start Moving...]");
            //        foreach (var item in goal) {
            //            OnConsoleMessage("[AGV Move To] - {0}", item.ToString());
            //            OnConsoleMessage("[AGV Arrived] - {0}", item.ToString());
            //        }
            //        OnConsoleMessage("[AGV Move Finished]");
            //    });
            //}else {
            //    CtMsgBox.Show(mHandle,"No target","尚未選取Goal點，無法進行Run all",MsgBoxBtn.OK,MsgBoxStyle.Information);
            //}
        }

        #endregion IGoalSetting 事件連結


        /// <summary>
        /// 工具箱切換工具事件
        /// </summary>
        /// <param name="mode"></param>
        protected override void ToolBox_SwitchCursor(CursorMode mode) {
            mCursorMode = mode;
            switch (mode) {
                case CursorMode.Drag:
                    IMapCtrl.SetDragMode();
                    break;

                case CursorMode.Goal:
                    IMapCtrl.SetAddMode(FactoryMode.Factory.Goal($"Goal{Database.GoalGM.Count}"));
                    break;

                case CursorMode.Power:
                    IMapCtrl.SetAddMode(FactoryMode.Factory.Power($"Power{Database.PowerGM.Count}"));
                    break;

                case CursorMode.Select:
                    IMapCtrl.SetSelectMode();
                    break;

                case CursorMode.Pen:
                    IMapCtrl.SetPenMode();
                    break;

                case CursorMode.Eraser:
                    IMapCtrl.SetEraserMode(500);
                    break;

                case CursorMode.Insert:
                    OpenFileDialog old = new OpenFileDialog() {
                        Filter = ".Map|*.map"
                    };
                    if (old.ShowDialog() == DialogResult.OK) {
                        IMapCtrl.SetInsertMapMode(old.FileName, mMapInsert as IMouseInsertPanel);
                    }
                    break;

                case CursorMode.ForbiddenArea:
                    IMapCtrl.SetAddMode(FactoryMode.Factory.ForbiddenArea("ForbiddenArea"));
                    break;

                default:
                    throw new ArgumentException($"未定義{mode}模式");
            }
        }

        #endregion Function - Events

    }

    public interface IMapGL : IBaseMapGL {
        /// <summary>
        /// 地圖中心點
        /// </summary>
        IPair MapCenter { get; set; }

        IScene Ctrl { get; }
    }

    public interface IGoalSetting : IBaseGoalSetting {
        /// <summary>
        /// 按照順序移動全部
        /// </summary>
        event DelRunLoop RunLoopEvent;

        /// <summary>
        /// 設定真實座標
        /// </summary>
        void UpdateNowPosition(IPair realPos);

        /// <summary>
        /// 重新載入標示物
        /// </summary>
        void ReloadSingle();

    }


    /// <summary>
    /// 按照順序移動全部
    /// </summary>
    public delegate void DelRunLoop(IEnumerable<IGoal> goal);

    /// <summary>
    /// 更新 Goal 點
    /// </summary>
    public delegate void DelUpdateGoal(IGoal newGoal);

}
