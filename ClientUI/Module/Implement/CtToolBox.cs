﻿using CtDockSuit;
using CtOutLookBar.Public;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VehiclePlanner.Partial.VehiclePlannerUI;
using WeifenLuo.WinFormsUI.Docking;
using static VehiclePlanner.Partial.VehiclePlannerUI.Events.GoalSettingEvents;

namespace VehiclePlanner.Module.Implement {

    public partial class CtToolBox : CtDockContainer {

        #region Declaration - Events

        public event DelSwitchCursor SwitchCursor;

        #endregion Declaration - Evetns

        #region Function - Constructors

        /// <summary>
        /// 共用建構方法
        /// </summary>
        public CtToolBox(DockState defState = DockState.Float)
            : base(defState)
        {
            InitializeComponent();
            FixedSize = new Size(200, 711);
            //outlookBar2.Dock = DockStyle.Fill;
            IOutlookCategory mapTool =  outlookBar2.AddCategory("Map Tool");
            foreach(CursorMode mode in Enum.GetValues(typeof(CursorMode))) {
                IClickSender sender = mapTool.AddItem(mode, $@"Icon\{mode}.png");
                if (sender != null) {
                    sender.Click += OutlookItem_OnClick;
                }
            }
            IOutlookCategory otherTool = outlookBar2.AddCategory("Other Tool");
            foreach (CursorMode mode in Enum.GetValues(typeof(CursorMode))) {
                otherTool.AddItem(mode.ToString()).Click += OutlookItem_OnClick;
            }
            mapTool.BackColor = Color.LightSlateGray;
            otherTool.BackColor = Color.LightGreen;
            mapTool.RowCount = 1;
            outlookBar2.SelectCategory(0);
            outlookBar2.Dock = DockStyle.Fill;
            outlookBar2.BackColor = Color.Black;
        }

        #endregion Function - Consturctors

        #region Funciotn - Events

        private void OutlookItem_OnClick(object sender,EventArgs e) {
            Control ctrl = sender as Control;
            IOutlookItem item = ctrl?.Tag as IOutlookItem;
            if (Enum.IsDefined(typeof(CursorMode), item.EnumIdx)) {
                CursorMode mode = (CursorMode)item.EnumIdx;
                Console.WriteLine((CursorMode)item.EnumIdx);
                SwitchCursor?.Invoke(mode);

                this.DockState = DockState.DockRight;
                this.DockState = DockState.DockRightAutoHide;
            }
        }

        #endregion Function - Events

    }

}
