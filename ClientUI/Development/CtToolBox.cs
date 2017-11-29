using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilityLibrary.WinControls;

namespace ClientUI.Development {
    public partial class CtToolBox : Form {
        private OutlookBar outlookBar1 = null;  
        public CtToolBox() {
            InitializeComponent();
            outlookBar1 = new OutlookBar();
            
            #region 销售管理
            OutlookBarBand outlookShortcutsBand = new OutlookBarBand("销售管理");
            outlookShortcutsBand.SmallImageList = this.imageList1;
            outlookShortcutsBand.LargeImageList = this.imageList1;
            outlookShortcutsBand.Items.Add(new OutlookBarItem("订单管理", 0));

            outlookShortcutsBand.Items.Add(new OutlookBarItem("订单管理", 0));

            outlookShortcutsBand.Items.Add(new OutlookBarItem("订单管理", 0));

            outlookShortcutsBand.Items.Add(new OutlookBarItem("订单管理", 0));

            outlookShortcutsBand.Items.Add(new OutlookBarItem("订单管理", 0));

            outlookShortcutsBand.Items.Add(new OutlookBarItem("订单管理", 0));

            outlookShortcutsBand.Items.Add(new OutlookBarItem("订单管理", 0));
            outlookShortcutsBand.Background = SystemColors.AppWorkspace;
            outlookShortcutsBand.TextColor = Color.White;
            outlookBar1.Bands.Add(outlookShortcutsBand);
            outlookBar1.Dock = DockStyle.Fill;
            #endregion
            panel1.Controls.Add(outlookBar1);
        }
    }
}
