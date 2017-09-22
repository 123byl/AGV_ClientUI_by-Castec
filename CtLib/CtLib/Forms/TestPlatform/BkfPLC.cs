using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Beckhoff;
using CtLib.Module.Ultity;

namespace CtLib.Forms.TestPlatform
{
    /// <summary>[測試介面] Beckhoff PLC</summary>
    public partial class Test_BkfPLC : Form
    {

        #region Version

        /// <summary>Test_BkfPLC 版本訊息</summary>
        public static readonly CtVersion @Version = new CtVersion(1, 0, 0, "2014/09/12", "Ahern Kuo");

        /*---------- Version Note ----------
         * 
         * 1.0.0  <Ahern> [2014/09/12]
         *      + 完成初版介面
         *      
         *----------------------------------*/

        #endregion

        #region Declaration - Member

        /// <summary>Reference of Beckhoff object</summary>
        private CtBeckhoff rBkf;

        #endregion

        #region Function - Constructor
        /// <summary>開啟測試介面</summary>
        public Test_BkfPLC()
        {
            InitializeComponent();
        }
        #endregion

        #region Function - Method
        private void EnableControl(Control ctrl, bool enable)
        {
            if (ctrl.HasChildren)
            {
                foreach (Control subCtrl in ctrl.Controls)
                {
                    EnableControl(subCtrl, enable);
                }
            }
            else
            {
                ctrl.Enabled = enable;
            }
        }

        private void ShowMessage(string info)
        {
            List<string> lstTemp = new List<string> { DateTime.Now.ToString("HH:mm:ss.ff"), info };
            CtInvoke.DataGridViewAddRow(dgvInfo, lstTemp);
        }

        private void ShowMessage(string info, Color bgColor)
        {
            List<string> lstTemp = new List<string> { DateTime.Now.ToString("HH:mm:ss.ff"), info };
            CtInvoke.DataGridViewAddRow(dgvInfo, lstTemp, bgColor);
        }
        #endregion

        #region Function - CtBeckhoff Events

        void rBkf_OnSymbolChanged(object sender, CtBeckhoff.SymbolEventArgs e)
        {
            throw new NotImplementedException();
        }

        void rBkf_OnBoolEventChanged(object sender, CtBeckhoff.BoolEventArgs e)
        {
            throw new NotImplementedException();
        }

        void rBkf_OnMessage(object sender, CtBeckhoff.MessageEventArgs e)
        {
            /* According the message type, choose the different background color */
            Color bgColor = Color.Black;
            switch (e.Type)
            {
                case -1:
                    bgColor = Color.Red;
                    break;
                case 0:
                    bgColor = Color.White;
                    break;
                case 1:
                    bgColor = Color.Yellow;
                    break;
            }

            /* Combine the message of Title and Content */
            string strTemp = "[" + e.Title + "] - " + e.Message;

            /* Request show message function to display on DataGridView */
            ShowMessage(strTemp, bgColor);
        }
        #endregion

        private void btnConnect_Click(object sender, EventArgs e)
        {
            rBkf = new CtBeckhoff();
            if (rBkf != null)
            {
                rBkf.OnBoolEventChanged += rBkf_OnBoolEventChanged;
                rBkf.OnSymbolChanged += rBkf_OnSymbolChanged;
                rBkf.OnMessage += rBkf_OnMessage;
            }

            Stat stt = rBkf.Connect(txtNetID.Text, CtConvert.CInt(txtPort.Text));
            if (stt == Stat.SUCCESS)
            {
                /* Save the current NetID and port into application */
                Properties.Settings.Default.Save();

                foreach (Control ctrl in this.Controls)
                {
                    EnableControl(ctrl, true);
                }

                CtInvoke.TextBoxEnable(txtNetID, false);
                CtInvoke.TextBoxEnable(txtPort, false);
            }
        }

        private void btnVarRead_Click(object sender, EventArgs e)
        {
            if (txtVarName.Text != "")
            {
                object objTemp = null;
                rBkf.GetValue(txtVarName.Text, out objTemp);

                if (objTemp != null)
                {
                    CtInvoke.TextBoxText(txtVarVal, CtConvert.CStr(objTemp));
                }
            }
        }

        private void btnVarWrite_Click(object sender, EventArgs e)
        {
            if (txtVarVal.Text != "")
            {
                rBkf.SetValue(txtVarName.Text, txtVarVal.Text);
            }
        }

        private void btnVarAdMoni_Click(object sender, EventArgs e)
        {
            if (CtConvert.CBool(btnVarAdMoni.Tag))
            {
                rBkf.RemoveNotification(txtVarName.Text);
                CtInvoke.ButtonTag(btnVarAdMoni, false);
                CtInvoke.ButtonText(btnVarAdMoni, "加入監控");
            }
            else
            {
                if (txtVarName.Text != "")
                {
                    rBkf.AddNotification(txtVarName.Text);
                    CtInvoke.ButtonTag(btnVarAdMoni, true);
                    CtInvoke.ButtonText(btnVarAdMoni, "停止監控");
                }
            }
        }

        private void txtVarName_TextChanged(object sender, EventArgs e)
        {
            if (rBkf != null)
            {
                if (rBkf.IsMonitoring(txtVarName.Text))
                {
                    CtInvoke.ButtonTag(btnVarAdMoni, true);
                    CtInvoke.ButtonText(btnVarAdMoni, "停止監控");
                }
                else
                {
                    CtInvoke.ButtonTag(btnVarAdMoni, false);
                    CtInvoke.ButtonText(btnVarAdMoni, "加入監控");
                }
            }
        }

        private void btnSttSet_Click(object sender, EventArgs e)
        {
            rBkf.SetAdsStatus((CtBeckhoff.AdsStatus)cbSttList.SelectedIndex);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<object> lstObj;
            rBkf.GetValue(txtAryName.Text, out lstObj);
            if (lstObj != null)
            {
                List<string> lstStr = lstObj.ConvertAll(new Converter<object, string>(val => CtConvert.CStr(val)));
                foreach (string str in lstStr)
                {
                    CtInvoke.DataGridViewAddRow(dgvData, str);
                }
            }
        }

        private void btnAryWrite_Click(object sender, EventArgs e)
        {
            if (dgvData.Rows.Count > 0)
            {
                List<string> lstStr = new List<string>();
                foreach (DataGridViewRow row in dgvData.Rows)
                {
                    lstStr.Add(CtConvert.CStr(row.Cells[0].Value));
                }

                for (int i = 0; i < lstStr.Count; i++)
                {
                    rBkf.SetValue(txtAryName.Text + "[" + CtConvert.CStr(i) + "]", lstStr[i]);
                }
            }
        }


    }
}
