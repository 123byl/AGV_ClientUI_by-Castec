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

namespace VehiclePlannerAGVBase {

    public partial class VehiclePlanner : VehiclePlannerUI {
        
        /// <summary>
        /// 地圖插入控制器
        /// </summary>
        private CtDockContainer mMapInsert = new CtMapInsert();

        /// <summary>
        /// MapGL子視窗
        /// </summary>
        private IMapGL MapGL {
            get {
                return mDockContent.ContainsKey(miMapGL) ? mDockContent[miMapGL] as IMapGL : null;
            }
        }

        protected override IScene IMapCtrl {
            get {
                return MapGL?.Ctrl;
            }
        }


        public VehiclePlanner():base(null) {
            InitializeComponent();
        }

        protected override void AddMapGL() {
            mDockContent.Add(miMapGL, new MapGL(DockState.Document));
        }

        protected override void SetEvents() {
            base.SetEvents();
            IMapCtrl.GLClickEvent += IMapCtrl_GLClickEvent;
            IMapCtrl.DragTowerPairEvent += IMapCtrl_DragTowerPairEvent;
            IMapCtrl.GLMoveUp += IMapCtrl_GLMoveUp;

        }
        
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

        protected override void LoadICtDockContainer() {
            base.LoadICtDockContainer();
            mMapInsert.AssignmentDockPanel(dockPanel);
        }


    }

    public interface IMapGL : IBaseMapGL {
        /// <summary>
        /// 地圖中心點
        /// </summary>
        IPair MapCenter { get; set; }

        IScene Ctrl { get; }
    }
}
