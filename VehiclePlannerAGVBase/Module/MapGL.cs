using Geometry;
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
using VehiclePlanner.Module.Implement;
using WeifenLuo.WinFormsUI.Docking;
using VehiclePlanner.Core;

namespace VehiclePlannerAGVBase {
    public partial class MapGL : BaseMapGL ,IMapGL{
        
        /// <summary>
        /// MapGL
        /// </summary>
        /// <remarks>
        /// 本來是想在設計介面加入的
        /// 但是在顯示/隱藏的時候會神奇的反向
        /// 只好用程式碼加入的方式
        /// </remarks>
        private GLUserControl mAGVBaseMapGL = new GLUserControl();

        /// <summary>
        /// 地圖中心點
        /// </summary>
        private IPair mMapCenter = FactoryMode.Factory.Pair();

        /// <summary>
        /// 獲得地圖控制器控制
        /// </summary>
        public IScene Ctrl { get { return mAGVBaseMapGL.BaseCtrl; } }

        /// <summary>
        /// 地圖焦點
        /// </summary>
        public IPair MapCenter {
            get => mMapCenter;
            set {
                if (mMapCenter != value) {
                    mMapCenter = value;
                    Ctrl.Focus(mMapCenter.X, mMapCenter.Y);
                }
            }
        }

        protected MapGL():base() { }

        public MapGL(CtVehiclePlanner_Ctrl refUI, DockState defState = DockState.Float):base(refUI,defState) {
            InitializeComponent();
            ///控制項會自動Dock.Fill
            ///預設控制項位置在0,0
            ///會有一部分殘影，因此將控制項移至中心位置
            mAGVBaseMapGL.Location = new Point(0, 0);
            mAGVBaseMapGL.Dock = DockStyle.Fill;
            pnlShow.Controls.Add(mAGVBaseMapGL);
        }

        public void Bindings(IVehiclePlanner source) {
            base.Bindings(source);
            /*-- 地圖中心點 --*/
            this.DataBindings.Add(nameof(MapCenter), source, nameof(source.MapCenter), true, DataSourceUpdateMode.OnPropertyChanged, MapCenter);

        }
        
    }
}
