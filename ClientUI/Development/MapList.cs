using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientUI
{
    public partial class MapList : Form
    {

        public string strMapList = "";
        public MapList(string MapList)
        {
            InitializeComponent();
            btnOK.DialogResult = DialogResult.OK;
            btnCancel.DialogResult = DialogResult.Cancel;
            string[] strRemoveEndPoint = MapList.Split(',');
            foreach (string x in strRemoveEndPoint)
            {
                lstbMap.Items.Add(x);
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            strMapList = lstbMap.SelectedItem.ToString();
        }
    }
}
