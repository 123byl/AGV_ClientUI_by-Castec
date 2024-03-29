﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace VehiclePlanner.Forms {

    public partial class MapList : Form {
        public string strMapList = "";

        public MapList(string MapList) : this(MapList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
        }

        public MapList(IEnumerable<string> mapList) {
            InitializeComponent();
            TopLevel = true;
            TopMost = true;
            btnOK.DialogResult = DialogResult.OK;
            btnCancel.DialogResult = DialogResult.Cancel;
            foreach (string x in mapList) {
                lstbMap.Items.Add(x);
            }
            this.TopMost = true;
        }

        private void btnOK_Click(object sender, EventArgs e) {
            strMapList = lstbMap.SelectedItem.ToString();
        }
    }
}