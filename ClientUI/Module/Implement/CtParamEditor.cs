using CtDockSuit;
using INITesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VehiclePlanner.Core;
using WeifenLuo.WinFormsUI.Docking;
using CtLib.Module.Utility;

namespace VehiclePlanner.Module.Implement {

    /// <summary>
    /// 參數編輯器子視窗
    /// </summary>
    public partial class ParamEditor : AuthorityDockContainer {

        #region Declaration - Fields

        /// <summary>
        /// 參數編輯器
        /// </summary>
        private CtrlParamEditor mEditor = null;

        #endregion Declaration - Fields

        #region Funciotn - Constructors

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public ParamEditor(BaseVehiclePlanner_Ctrl refUI, DockState defState = DockState.Float)
            : base(refUI,defState) {
            InitializeComponent();
            mEditor = new CtrlParamEditor() {
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None
            };
			mEditor.UplaodParameter += Editor_UplaodParameter;
			mEditor.DownloadParameter += Editor_DownloadParameter;
            this.Controls.Add(mEditor);
            mEditor.Show();
            FixedSize = new Size(718, 814);
        }

		private void Editor_UplaodParameter(object sender, EventArgs e)
		{
			RefUI.UploadParameter();
		}

		private void Editor_DownloadParameter(object sender, EventArgs e)
		{
			RefUI.DownloadParameter();
		}

		#endregion Funciotn - Consturctors

		#region Function - Public Mehotds

		public override bool IsVisiable(AccessLevel lv) {
            return lv > AccessLevel.Operator;
        }

        #endregion Funciotn - Public Methods
    }
}
