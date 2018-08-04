using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VehiclePlanner.Module;
using CtLib.Module.Modbus;
using CtLib.Module.Utility;
using VehiclePlanner;
using WeifenLuo.WinFormsUI.Docking;

namespace CtItsParameter
{
	public partial class ItsParameterCtrl : AuthorityDockContainer
	{
		public ItsParameterCtrl():base()
		{
			InitializeComponent();
		}

		public ItsParameterCtrl(BaseVehiclePlanner_Ctrl refUI, DockState defState):base(refUI,defState)
		{
			InitializeComponent();
		}

		public override bool IsVisiable(AccessLevel lv)
		{
			return lv > AccessLevel.None;
		}
	}
}
