using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CtLib.Forms {
    /// <summary>
    /// CtLib 啟動器
    /// <para>藉由簡易的按鈕提供使用者選擇測試介面</para>
    /// <para>請使用<see cref="Start"/>方法進行啟動</para>
    /// </summary>
    public partial class Launcher : Form {

        private Dictionary<string, Type> DEFAULT_FORM_LIST = new Dictionary<string, Type>{
			{ "System Info", typeof(TestPlatform.SystemInfo)},
			{ "Serial", typeof(TestPlatform.Test_Serial)},
			{ "Socket", typeof(TestPlatform.Test_Socket)},
			{ "Pipe", typeof(TestPlatform.Test_AsyncPipe)},
			{ "Adept ACE", typeof(TestPlatform.Test_ACE)},
			{ "Beckhoff PLC", typeof(TestPlatform.Test_BkfPLC)},
			{ "Wago I/O", typeof(TestPlatform.Test_WagoIO)},
			{ "Universal Robots", typeof(TestPlatform.Test_UniversalRobots)},
			{ "Modbus RTU", typeof(TestPlatform.Test_ModbusRTU)},
			{ "XML Editor", typeof(TestPlatform.XML_Editor)},
			{ "DENSO ORiN2", typeof(TestPlatform.Test_Denso)},
			{ "FESTO CMMO", typeof(TestPlatform.Test_FestoCMMO)},
			{ "Delta ASDA-A2", typeof(TestPlatform.Test_Delta_ASDA_A2)},
			{ "Delta SE", typeof(TestPlatform.Delta_SE)},
			{ "Oriental BLE", typeof(TestPlatform.Oriental_BLE_FLEX)},
			{ "IAI PCON", typeof(TestPlatform.IAI_PCON)},
			{ "Dimmer (荃達)", typeof(TestPlatform.DimmerCtrl)},
			{ "Dimmer (5CH RS232)", typeof(TestPlatform.Dimmer_CH5I700RS232)},
			{ "Dimmer (4CH RS485)", typeof(TestPlatform.Dimmer_CH4I700RS485)},
			{ "Keyence DL-RS1A", typeof(TestPlatform.Keyence_DL_RS1A)},
			{ "繁簡轉換", typeof(TestPlatform.ChineseTranslator)},
			{ "Console Monitor", typeof(TestPlatform.ProcessMonitor)},
			{ "ICT", typeof(Module.PCB.ICT.ICTReader)},
			{ "Stäubli", typeof(TestPlatform.Test_Staubli)}
		};

        private static readonly int DEFAULT_ROW_COUNT = 6;

        private static readonly Point DEFAULT_BUTTON_LOC = new Point(60, 30);
        private static readonly Size DEFAULT_BUTTON_SIZE = new Size(250, 65);
        private static readonly int DEFAULT_BUTTON_GAP = 5;
        private static readonly Font DEFAULT_BUTTON_FONT = new Font("Century Gothic", 12);

        private Form mFormChoosed;

        /// <summary>
        /// 建構啟動器，並且建立 DEFAULT_FORM_LIST 之對應按鈕
        /// <para>請使用<see cref="Start"/>方法進行啟動</para>
        /// </summary>
        public Launcher() {
            InitializeComponent();

            CreateButtons();
        }

        private void Launcher_Paint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;
            Brush brush = new LinearGradientBrush(DisplayRectangle, Color.DimGray, Color.Black, 45);
            g.FillRectangle(brush, 0, 0, Width, Height);
        }

        private void CreateButtons() {

            int index = 0;
            foreach (KeyValuePair<string, Type> item in DEFAULT_FORM_LIST) {
                Button btn = new Button();
                btn.Text = item.Key;
                btn.Size = DEFAULT_BUTTON_SIZE;
                btn.Left = DEFAULT_BUTTON_LOC.X + (index / DEFAULT_ROW_COUNT * (DEFAULT_BUTTON_SIZE.Width + DEFAULT_BUTTON_GAP));
                btn.Top = DEFAULT_BUTTON_LOC.Y + (index % DEFAULT_ROW_COUNT) * (DEFAULT_BUTTON_SIZE.Height + DEFAULT_BUTTON_GAP);
                btn.Font = DEFAULT_BUTTON_FONT;
                btn.FlatAppearance.BorderColor = Color.DimGray;
                btn.FlatAppearance.MouseDownBackColor = Color.DeepSkyBlue;
                btn.FlatAppearance.MouseOverBackColor = Color.LightSkyBlue;
                btn.FlatStyle = FlatStyle.Flat;
                btn.Parent = this;
                btn.Click += btn_Click;
                btn.Show();
                index++;
            }
            Width = (int)Math.Ceiling((double)index / DEFAULT_ROW_COUNT) * (DEFAULT_BUTTON_SIZE.Width + DEFAULT_BUTTON_GAP) + DEFAULT_BUTTON_LOC.X * 2;
            Height = (DEFAULT_ROW_COUNT + 1) * (DEFAULT_BUTTON_SIZE.Height + DEFAULT_BUTTON_GAP) + DEFAULT_BUTTON_LOC.Y;
        }

        private void btn_Click(object sender, EventArgs e) {
			Type frm = DEFAULT_FORM_LIST[((sender as Button).Text)];
			mFormChoosed = Activator.CreateInstance(frm) as Form;

			DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>顯示啟動器，並且回傳使用者所選擇之視窗</summary>
        /// <param name="form">使用者所選擇之視窗</param>
        /// <returns>視窗操作結果 (OK)使用者選擇一介面 (Cancel)使用者選擇關閉視窗，不載入任何介面</returns>
        public DialogResult Start(out Form form) {
            DialogResult result = ShowDialog();
            form = mFormChoosed;
            return result;
        }
    }
}
