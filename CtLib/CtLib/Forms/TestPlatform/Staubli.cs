using CtLib.Library;
using CtLib.Module.Staubli;
using CtLib.Module.Utility;
using CtLib.Module.Utility.Drawing;
using CtLib.Module.Utility.Renderer;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtLib.Forms.TestPlatform {
    /// <summary>Stäubli CS8 測試介面</summary>
    public partial class Test_Staubli : Form {

        #region Declaration - Supported Class
        private enum MsgType {
            Normal,
            Exception,
            Data
        }

        private class Msg {
            public string Time { get; }
            public string Type { get; }
            public string Message { get; }
            public Color RowColor { get; }

            public Msg(MsgType mt, string msg) {
                Time = DateTime.Now.ToString("HH:mm:ss.fff");
                Type = mt.ToString();
                Message = msg;
                RowColor = ToColor(mt);
            }

            public Msg(DateTime time, MsgType mt, string msg) {
                Time = time.ToString("HH:mm:ss.fff");
                Type = mt.ToString();
                Message = msg;
                RowColor = ToColor(mt);
            }

            private Color ToColor(MsgType mt) {
                Color clr = Color.Black;
                switch (mt) {
                    case MsgType.Normal:
                        clr = Color.FromArgb(210, 210, 210);
                        break;
                    case MsgType.Exception:
                        clr = Color.Red;
                        break;
                    case MsgType.Data:
                        clr = Color.LightSkyBlue;
                        break;
                    default:
                        break;
                }
                return clr;
            }
        }
        #endregion

        #region Declaration - Fields
        private CtStaubliCS8 mCS8;
        private BindingList<Msg> mMsgPool = new BindingList<Msg>();
        private Dictionary<string, bool> mMouseIn = new Dictionary<string, bool>();
        private Dictionary<string, Dictionary<UILanguage, string>> mCult = new Dictionary<string, Dictionary<UILanguage, string>> {
            { "login",
                new Dictionary<UILanguage, string> {
                    {UILanguage.English, "LogIn" },
                    {UILanguage.SimplifiedChinese, "登入" },
                    {UILanguage.TraditionalChinese, "登入" }
                }
            },
            { "logout",
                new Dictionary<UILanguage, string> {
                    {UILanguage.English, "LogOut" },
                    {UILanguage.SimplifiedChinese, "注销" },
                    {UILanguage.TraditionalChinese, "登出" }
                }
            },
            { "pwdTit",
                new Dictionary<UILanguage, string> {
                    {UILanguage.English, "Password" },
                    {UILanguage.SimplifiedChinese, "输入密码" },
                    {UILanguage.TraditionalChinese, "輸入密碼" }
                }
            },
            { "pwdCnt",
                new Dictionary<UILanguage, string> {
                    {UILanguage.English, "Please enter the password of \"{0}\"" },
                    {UILanguage.SimplifiedChinese, "请输入 \"{0}\" 的登入密码" },
                    {UILanguage.TraditionalChinese, "請輸入 \"{0}\" 的登入密碼" }
                }
            },
            { "tskInv",
                new Dictionary<UILanguage, string> {
                    {UILanguage.English, "Please load task first and click \"Get Tasks\" on left side" },
                    {UILanguage.SimplifiedChinese, "请先加载任务，并按下左方 \"取得任务\"" },
                    {UILanguage.TraditionalChinese, "請先載入任務，並按下左方 \"取得任務\"" }
                }
            },
            { "appInv",
                new Dictionary<UILanguage, string> {
                    {UILanguage.English, "Please get application first. Click \"Get Apps\" on \"Task\" groupbox" },
                    {UILanguage.SimplifiedChinese, "请先读取所有项目，按下上方 \"Task\" 群组中的 \"取得 App\"" },
                    {UILanguage.TraditionalChinese, "請先讀取所有專案，按下上方 \"Task\" 群組中的 \"取得 App\"" }
                }
            },
            { "execTit",
                new Dictionary<UILanguage, string> {
                    {UILanguage.English, "Execute VAL3 Task" },
                    {UILanguage.SimplifiedChinese, "执行 VAL3 任务" },
                    {UILanguage.TraditionalChinese, "執行 VAL3 任務" }
                }
            },
            { "execDesc",
                new Dictionary<UILanguage, string> {
                    {UILanguage.English, "Please enter \"Project Name, VAL3 Task\". E.g. \"proj1,start,Disk://Demo/Demo.pjx\"" },
                    {UILanguage.SimplifiedChinese, "请输入 \"项目名称,VAL3 程序名称\"。例如 \"proj1,start,Disk://Demo/Demo.pjx\" (不含 \" 号)" },
                    {UILanguage.TraditionalChinese, "請輸入 \"專案名稱,VAL3 程式名稱\"。例如 \"proj1,start,Disk://Demo/Demo.pjx\" (不含 \" 號)" }
                }
            }

        };
        #endregion

        #region Function - Constructors
        /// <summary>初始化測試介面</summary>
        public Test_Staubli() {
            InitializeComponent();

            txtIP.Text = Staubli.Default.IP;
            txtPort.Text = Staubli.Default.Port;
            txtUsr.Text = Staubli.Default.Account;

            InitializeControls();
            InitializeMouseState();
            SetStyles();
        }
        #endregion

        #region Function - Styles
        private void InitializeControls() {
            /* DataGridView */
            dgvMsg.AutoGenerateColumns = false;
            colTime.DataPropertyName = "Time";
            colType.DataPropertyName = "Type";
            colMsg.DataPropertyName = "Message";
            dgvMsg.DataSource = mMsgPool;
        }

        private void FindControls(Dictionary<Type, List<Control>> dic, Control ctrl) {
            if (!dic.ContainsKey(ctrl.GetType())) dic.Add(ctrl.GetType(), new List<Control> { ctrl });
            else dic[ctrl.GetType()].Add(ctrl);

            if (ctrl.HasChildren) {
                foreach (Control item in ctrl.Controls) {
                    FindControls(dic, item);
                }
            }
        }

        private void InitializeMouseState() {
            /* Search all controls */
            var ctrls = new Dictionary<Type, List<Control>>();
            foreach (Control ctrl in this.Controls) {
                FindControls(ctrls, ctrl);
            }

            foreach (var kvp in ctrls) {
                kvp.Value.ForEach(
                    ctrl => {
                        if (!string.IsNullOrEmpty(ctrl.Name))
                            mMouseIn.Add(ctrl.Name, false);
                    }
                );
            }
        }

        private void SetStyles() {
            var frmBgClr = Color.FromArgb(30, 30, 30);
            var ctrlBgClr = Color.FromArgb(45, 45, 45);
            var txtClr = Color.FromArgb(220, 220, 220);

            /* Search all controls */
            var ctrls = new Dictionary<Type, List<Control>>();
            foreach (Control ctrl in this.Controls) {
                FindControls(ctrls, ctrl);
            }

            /* Form */
            this.BackColor = frmBgClr;
            this.ForeColor = txtClr;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            /* GroupBox */
            if (ctrls.ContainsKey(typeof(GroupBox)))
                ctrls[typeof(GroupBox)].ForEach(
                    ctrl => (ctrl as GroupBox).Paint += GroupBoxPaint
                );

            /* Button */
            if (ctrls.ContainsKey(typeof(Button)))
                ctrls[typeof(Button)].ForEach(
                    ctrl => {
                        var btn = ctrl as Button;
                        btn.Paint += ButtonPaint;
                        btn.MouseEnter += MouseIn;
                        btn.MouseLeave += MouseOut;
                    }
                );

            /* TextBox */
            if (ctrls.ContainsKey(typeof(TextBox)))
                ctrls[typeof(TextBox)].ForEach(
                    ctrl => {
                        var txt = ctrl as TextBox;
                        txt.BorderStyle = BorderStyle.None;
                        txt.BackColor = ctrlBgClr;
                        txt.ForeColor = txtClr;
                    }
                );

            /* ListBox */
            if (ctrls.ContainsKey(typeof(ListBox)))
                ctrls[typeof(ListBox)].ForEach(
                    ctrl => {
                        var lst = ctrl as ListBox;
                        lst.BorderStyle = BorderStyle.None;
                        lst.BackColor = frmBgClr;
                        lst.ForeColor = txtClr;
                    }
                );

            /* ComboBox */
            if (ctrls.ContainsKey(typeof(ComboBox)))
                ctrls[typeof(ComboBox)].ForEach(
                    ctrl => {
                        var cb = ctrl as ComboBox;
                        cb.BackColor = ctrlBgClr;
                        cb.ForeColor = txtClr;
                    }
                );

            /* Labels */
            if (ctrls.ContainsKey(typeof(Label)))
                ctrls[typeof(Label)].ForEach(
                    ctrl => {
                        var lbl = ctrl as Label;
                        lbl.ForeColor = txtClr;
                    }
                );

            /* DataGridView */
            if (ctrls.ContainsKey(typeof(DataGridView)))
                ctrls[typeof(DataGridView)].ForEach(
                    ctrl => {
                        var dgv = ctrl as DataGridView;
                        dgv.BackgroundColor = frmBgClr;
                        dgv.ColumnHeadersDefaultCellStyle.BackColor = ctrlBgClr;
                        dgv.ColumnHeadersDefaultCellStyle.ForeColor = txtClr;
                        dgv.EnableHeadersVisualStyles = false;
                        dgv.DefaultCellStyle.BackColor = frmBgClr;
                        dgv.DefaultCellStyle.ForeColor = txtClr;
                    }
                );

            /* MenuStrip */
            menuStrip.Renderer = new ToolStripDeepStyleRenderer();
        }

        private void GroupBoxPaint(object sender, PaintEventArgs e) {
            var bgClr = Color.FromArgb(30, 30, 30);
            var bdClr = Color.FromArgb(70, 70, 70);
            var txtClr = Color.FromArgb(220, 220, 220);
            var grp = sender as GroupBox;
            var g = e.Graphics;

            //取得標題大小
            var txtSz = g.MeasureString(grp.Text, grp.Font);
            //計算邊框位置
            var bdRect = e.ClipRectangle;
            var halfTxtY = Math.Ceiling(txtSz.Height / 2);
            bdRect.Y += (int)halfTxtY;
            bdRect.Height -= (int)halfTxtY;
            var path = CtDraw.GetRoundRectanglePath(bdRect);

            //畫畫
            g.Clear(bgClr);
            g.DrawPath(bdClr.GetPen(1), path);
            g.FillRectangle(bgClr.GetSolidBrush(), 8, 0, txtSz.Width + 2, txtSz.Height);
            g.DrawString(grp.Text, grp.Font, txtClr.GetSolidBrush(), 10, 0);
        }

        private void ButtonPaint(object sender, PaintEventArgs e) {
            var bgClr = Color.FromArgb(45, 45, 45);
            var bdClr = Color.FromArgb(70, 70, 70);
            var txtClr = Color.FromArgb(220, 220, 220);
            var disClr = Color.FromArgb(30, 30, 30);
            var btn = sender as Button;
            var g = e.Graphics;

            //取得文字座標
            var txtSz = g.MeasureString(btn.Text, btn.Font);
            var txtPt = new PointF(
                (e.ClipRectangle.Width - txtSz.Width) / 2,
                (e.ClipRectangle.Height - txtSz.Height) / 2
            );

            //畫畫
            g.Clear(mMouseIn[btn.Name] ? bdClr : bgClr);
            var brsh = (btn.Enabled) ? txtClr.GetSolidBrush() : disClr.GetSolidBrush();
            g.DrawString(btn.Text, btn.Font, brsh, txtPt);
        }

        private void MouseIn(object sender, EventArgs e) {
            var ctrl = sender as Control;
            mMouseIn[ctrl.Name] = true;
        }

        private void MouseOut(object sender, EventArgs e) {
            var ctrl = sender as Control;
            mMouseIn[ctrl.Name] = false;
        }

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        private void WindowMove(object sender, MouseEventArgs e) {
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        /// <summary>處理 Windows 訊息，當滑鼠指向右下角時，顯示 Moving 並允許改變大小</summary>
        /// <param name="m">Windows 訊息</param>
        protected override void WndProc(ref Message m) {
            if (0x84.Equals(m.Msg)) {
                var lparam = m.LParam.ToInt32();
                var pos = new Point(lparam & 0xFFFF, lparam >> 16);
                pos = this.PointToClient(pos);
                if (pos.X >= this.ClientSize.Width - 10 && pos.Y >= this.ClientSize.Height - 10) {
                    m.Result = (IntPtr)17;
                    return;
                }
            }
            base.WndProc(ref m);
        }

        private void FormPaint(object sender, PaintEventArgs e) {
            var pen = Color.FromArgb(0, 122, 204).GetPen(1);
            e.Graphics.DrawRectangle(pen, e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
        }
        #endregion

        #region Function - Private Methods
        private string GetCultMsg(string keyWord) {
            var lang = CtLanguage.GetUiLangByCult();
            return mCult[keyWord][lang];
        }

        private void ShowMessage(MsgType type, string msg) {
            var obj = new Msg(type, msg);
            dgvMsg.InvokeIfNecessary(
                () => {
                    mMsgPool.Insert(0, obj);
                    dgvMsg.Rows[0].Cells[2].Style.ForeColor = obj.RowColor;
                }
            );
        }

        private void LoginChanged(bool log) {
            if (log) {
                CtInvoke.ControlEnabled(txtIP, false);
                CtInvoke.ControlEnabled(txtPort, false);
                CtInvoke.ControlEnabled(txtUsr, false);
                CtInvoke.ControlEnabled(grpLoc, true);
                CtInvoke.ControlEnabled(grpPwr, true);
                CtInvoke.ControlEnabled(grpIO, true);
                CtInvoke.ControlEnabled(grpTask, true);
                CtInvoke.ControlEnabled(grpVar, true);
                CtInvoke.ControlText(btnLogin, GetCultMsg("logout"));
            } else {
                CtInvoke.ControlEnabled(txtIP, true);
                CtInvoke.ControlEnabled(txtPort, true);
                CtInvoke.ControlEnabled(txtUsr, true);
                CtInvoke.ControlEnabled(grpLoc, false);
                CtInvoke.ControlEnabled(grpPwr, false);
                CtInvoke.ControlEnabled(grpIO, false);
                CtInvoke.ControlEnabled(grpTask, false);
                CtInvoke.ControlEnabled(grpVar, false);
                CtInvoke.ControlText(btnLogin, GetCultMsg("login"));
            }
        }
        #endregion

        private void miExit_Click(object sender, EventArgs e) {
            try {
                if (mCS8?.IsLogin ?? false)
                    mCS8?.Disconnect();
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            CtInvoke.FormClose(this);
        }

        private void btnLogin_Click(object sender, EventArgs e) {
            try {
                if (mCS8 == null || !mCS8.IsLogin) {
                    var ip = CtInvoke.ControlText(txtIP);
                    var port = CtInvoke.ControlText(txtPort);
                    mCS8 = new CtStaubliCS8(ip, int.Parse(port));
                    string pwd = string.Empty;
                    string usr = CtInvoke.ControlText(txtUsr);
                    Stat stt = CtInput.Password(out pwd, GetCultMsg("pwdTit"), string.Format(GetCultMsg("pwdCnt"), usr));
                    if (stt == Stat.SUCCESS) {
                        mCS8.Connect(usr, pwd);
                    }
                    LoginChanged(true);
                    Staubli.Default.IP = ip;
                    Staubli.Default.Port = port;
                    Staubli.Default.Account = usr;
                    Staubli.Default.Save();
                    ShowMessage(MsgType.Normal, "Login to Stäubli CS8");
                } else {
                    mCS8.Disconnect();
                    mCS8 = null;
                    LoginChanged(false);
                    ShowMessage(MsgType.Normal, "Logout to Stäubli CS8");
                }
            } catch (Exception ex) {
                ShowMessage(MsgType.Exception, ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            try {
                var ret = string.Empty;
                var inStt = CtInput.Text(out ret, GetCultMsg("execTit"), GetCultMsg("execDesc"));
                if (inStt == Stat.SUCCESS) {
                    var split = ret.Split(',', ' ').Select(str => str.Trim());
                    if (split.Count() != 3) throw new ArgumentException("Invalid format");
                    var prj = split.ElementAt(0);
                    var tsk = split.ElementAt(1);
                    var app = split.ElementAt(2);
                    mCS8.TaskExecute(prj, app, tsk);
                    ShowMessage(MsgType.Normal, $"Task \"{tsk}\" executing");
                }
            } catch (Exception ex) {
                ShowMessage(MsgType.Exception, ex.Message);
            }
        }

        private void btnCurJoint_Click(object sender, EventArgs e) {
            try {
                var robs = mCS8.GetRobots();
                Console.WriteLine(string.Join("\r\n", robs));
                var idx = 0;
                foreach (var rob in robs) {
                    var jnt = mCS8.GetCurrentJoint(idx);
                    ShowMessage(MsgType.Data, $"Arm: {rob.Model}, Joints: {string.Join(" ", jnt.ConvertAll(j => j.ToString("F2")))}");
                    idx++;
                }
            } catch (Exception ex) {
                ShowMessage(MsgType.Exception, ex.Message);
            }
        }

        private void btnCurPoint_Click(object sender, EventArgs e) {
            try {
                var robs = mCS8.GetRobots();
                Console.WriteLine(string.Join("\r\n", robs));
                var idx = 0;
                foreach (var rob in robs) {
                    var wld = mCS8.GetCurrentPoint(idx);
                    ShowMessage(MsgType.Data, $"Arm: {rob.Model}, Joints: {string.Join(" ", wld.ConvertAll(j => j.ToString("F2")))}");
                    idx++;
                }
            } catch (Exception ex) {
                ShowMessage(MsgType.Exception, ex.Message);
            }
        }

        private void btnPwr_Click(object sender, EventArgs e) {
            bool pwr = CtInvoke.RadioButtonChecked(rdoPwrOn);
            try {
                mCS8.SetPower(pwr);
                ShowMessage(MsgType.Normal, $"Power {(pwr ? "enabled" : "disabled")}");
            } catch (Exception ex) {
                ShowMessage(MsgType.Exception, ex.Message);
            }
        }

        private void btnIoRead_Click(object sender, EventArgs e) {
            string io = CtInvoke.ControlText(txtIO);
            try {
                var ioStt = mCS8.GetIO(io);
                ShowMessage(MsgType.Data, ioStt.ToString());
            } catch (Exception ex) {
                ShowMessage(MsgType.Exception, ex.Message);
            }
        }

        private void btnIoWrite_Click(object sender, EventArgs e) {
            string io = CtInvoke.ControlText(txtIO);
            bool stt = CtInvoke.RadioButtonChecked(rdoIoOn);
            try {
                mCS8.SetIO(io, stt);
                ShowMessage(MsgType.Data, $"I/O \"{io}\" has been set to {(stt ? "ON" : "OFF")}");
            } catch (Exception ex) {
                ShowMessage(MsgType.Exception, ex.Message);
            }
        }

        private void btnAllIo_Click(object sender, EventArgs e) {
            try {
                var io = mCS8.GetIOs();
                io.ForEach(i => ShowMessage(MsgType.Data, i.ToString()));
            } catch (Exception ex) {
                ShowMessage(MsgType.Exception, ex.Message);
            }
        }

        private void btnApp_Click(object sender, EventArgs e) {
            try {
                var apps = mCS8.GetApplications();
                CtInvoke.ComboBoxClear(cboApp);
                CtInvoke.ComboBoxClear(cboVarApp);
                apps.ForEach(
                    a => {
                        CtInvoke.ComboBoxAdd(cboApp, a.Name);
                        CtInvoke.ComboBoxAdd(cboVarApp, a.Name);
                        ShowMessage(MsgType.Data, a.ToString());
                    }
                );
            } catch (Exception ex) {
                ShowMessage(MsgType.Exception, ex.Message);
            }
        }

        private void btnTask_Click(object sender, EventArgs e) {
            try {
                var tsks = mCS8.GetTasks();
                CtInvoke.ComboBoxClear(cboTsk);
                tsks.ForEach(
                    t => {
                        CtInvoke.ComboBoxAdd(cboTsk, t.Name);
                        ShowMessage(MsgType.Data, t.ToString());
                    }
                );
            } catch (Exception ex) {
                ShowMessage(MsgType.Exception, ex.Message);
            }
        }

        private void btnSusp_Click(object sender, EventArgs e) {
            var app = CtInvoke.ControlText(cboApp);
            var tsk = CtInvoke.ControlText(cboTsk);
            if (!string.IsNullOrEmpty(tsk)) {
                try {
                    mCS8.TaskSuspend(app, tsk);
                    var stt = mCS8.GetTaskState(tsk);
                    ShowMessage(MsgType.Data, $"Task:{tsk}, State:{stt}");
                } catch (Exception ex) {
                    ShowMessage(MsgType.Exception, ex.Message);
                }
            } else ShowMessage(MsgType.Exception, GetCultMsg("tskInv"));
        }

        private void btnResume_Click(object sender, EventArgs e) {
            var app = CtInvoke.ControlText(cboApp);
            var tsk = CtInvoke.ControlText(cboTsk);
            if (!string.IsNullOrEmpty(tsk)) {
                try {
                    mCS8.TaskResume(app, tsk);
                    var stt = mCS8.GetTaskState(tsk);
                    ShowMessage(MsgType.Data, $"Task:{tsk}, State:{stt}");
                } catch (Exception ex) {
                    ShowMessage(MsgType.Exception, ex.Message);
                }
            } else ShowMessage(MsgType.Exception, GetCultMsg("tskInv"));
        }

        private void btnKill_Click(object sender, EventArgs e) {
            var app = CtInvoke.ControlText(cboApp);
            var tsk = CtInvoke.ControlText(cboTsk);
            if (!string.IsNullOrEmpty(tsk)) {
                try {
                    mCS8.TaskKill(app, tsk);
                    ShowMessage(MsgType.Data, $"Task \"{tsk}\" killed");
                } catch (Exception ex) {
                    ShowMessage(MsgType.Exception, ex.Message);
                }
            } else ShowMessage(MsgType.Exception, GetCultMsg("tskInv"));
        }

        private void btnVarGet_Click(object sender, EventArgs e) {
            var appName = CtInvoke.ControlText(cboVarApp);
            var varName = CtInvoke.ControlText(txtVarName);
            if (!string.IsNullOrEmpty(appName)) {
                var val3Var = mCS8.GetVariable(appName, varName);
                var split = val3Var.ToString().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Reverse();
                foreach (var str in split) {
                    ShowMessage(MsgType.Data, str.Replace("\t", "    "));
                }
            } else ShowMessage(MsgType.Exception, GetCultMsg("appInv"));
        }

        private void btnVarGetAll_Click(object sender, EventArgs e) {
            var appName = CtInvoke.ControlText(cboVarApp);
            if (!string.IsNullOrEmpty(appName)) {
                var val3Var = mCS8.GetVariables(appName);
                val3Var.ForEach(
                    val => {
                        var split = val.ToString().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Reverse();
                        foreach (var str in split) {
                            ShowMessage(MsgType.Data, str.Replace("\t", "    "));
                        }
                    }
                );
            } else ShowMessage(MsgType.Exception, GetCultMsg("appInv"));
        }
    }
}
