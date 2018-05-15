using GLStyle;
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

namespace VehiclePlannerUndoable.cs {

    /// <summary>
    /// 以可重做MapGL進行實作之MapGL介面
    /// </summary>
    public partial class MapGL : BaseMapGL ,IMapGL{

        private GLUICtrl mMapGL = new GLUICtrl();

        /// <summary>
        /// 地圖控制項
        /// </summary>
        public GLUICtrl MapControl { get => mMapGL; }

        protected MapGL() {
            InitializeComponent();

        }
        public MapGL(DockState defState = DockState.Float) : base(defState) {
            InitializeComponent();

            // 載入設定檔
            StyleManager.LoadStyle("Style.ini");
            
            mMapGL.Location = new Point(0, 0);
            mMapGL.Dock = DockStyle.Fill;
            pnlShow.Controls.Add(mMapGL);
        }
    }
}
