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
using VehiclePlanner.Core;
using SerialCommunicationData;
using CtBind;
using CtLib.Module.Utility;

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
        
        /// <summary>
        /// VehicleConsole模擬物件
        /// </summary>
        private FakeVehicleConsole mVC = null;

        private Bindable.ToolStripMenuItem miToolBox = null;

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

        public CtVehiclePlanner_Ctrl(IVehiclePlanner vehiclePlanner):base() {
            InitializeComponent();
            /*-- 取得底層物件參考 --*/
            rVehiclePlanner = vehiclePlanner;

            /*-- 由於無法在設計師模式新增ToolStripMenuItem物件，只好以程式碼方式新增 --*/
            miToolBox = new Bindable.ToolStripMenuItem() { Text = "ToolBox" };
            miView.DropDownItems.Add(miToolBox);

            /*-- 系統初始化 --*/
            Initial(vehiclePlanner);

            /*-- 介面資料綁定 --*/
            Bindings(vehiclePlanner);
            Bindings(rVehiclePlanner.Controller);

            /*-- 模擬網路廣播接收物件 --*/
            mVC = new FakeVehicleConsole(!DesignMode);
        }

        #endregion Funciton - Constructors

        #region Function - Public Metnhods

        public void Bindings(IVehiclePlanner source) {
            if (source == null) return;
            Bindings<IVehiclePlanner>(source);
            /*-- 使用者變更 --*/
            string dataMember = nameof(source.UserData);
            miToolBox.DataBindings.ExAdd(nameof(miToolBox.Enabled), source, dataMember, (sender, e) => {
                e.Value = (e.Value as UserData).Authority(miToolBox);
            },source.UserData.Authority(miToolBox));
        }

        public void Bindings(IITSController source) {
            Bindings<IITSController>(source);
            /*-- iTS資訊 --*/
            string dataMember = nameof(source.Status);
            tsprgBattery.ProgressBar.DataBindings.Add(nameof(ProgressBar.Value), source, dataMember).Format += (sender, e) => {
                e.Value = (e.Value as IStatus).Battery;
            };
            tslbBattery.DataBindings.ExAdd(nameof(tslbBattery.Text), source, dataMember, (sender, e) => {
                e.Value = $"{(e.Value as IStatus).Battery}%";
            });
            tslbStatus.DataBindings.ExAdd(nameof(tslbStatus.Text), source, dataMember, (sender, e) => {
                e.Value = (e.Value as IStatus).Description.ToString();
            });
        }

        #endregion Funciton - Public Methods

        #region Funciton - Private Methods

        protected override BaseMapGL GetMapGL(DockState dockState) {
            return new MapGL(this,dockState);
        }

        protected override BaseGoalSetting GetGoalSetting(DockState dockState) {
            return new GoalSetting(this,dockState);
        }

        protected override void SetEvents() {
            base.SetEvents();
            mGoalSetting.ClearGoalsEvent += rVehiclePlanner.ClearMarker;
            mGoalSetting.DeleteSingleEvent += rVehiclePlanner.DeleteMarker;
            mGoalSetting.RunLoopEvent += IGoalSetting_RunLoopEvent;

            IMapCtrl.GLClickEvent += IMapCtrl_GLClickEvent;
            IMapCtrl.DragTowerPairEvent += IMapCtrl_DragTowerPairEvent;
            IMapCtrl.GLMoveUp += IMapCtrl_GLMoveUp;

            (mDockContent[miToolBox] as CtToolBox).SwitchCursor += ToolBox_SwitchCursor;
        }
        
        protected override void LoadICtDockContainer() {
            AddSubForm(miToolBox, new CtToolBox(this,DockState.DockRightAutoHide));
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

}
