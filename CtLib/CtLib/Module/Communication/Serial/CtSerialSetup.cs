using System;
using System.IO.Ports;
using System.Windows.Forms;

using CtLib.Forms;
using CtLib.Library;
using CtLib.Module.Utility;

namespace CtLib.Module.SerialPort {
    /// <summary>
    /// 提供一個簡易的 SerialPort 設定介面
    /// <para>包含了 埠號(Port), 鮑率(BaudRate), 資料位元數(Bit) 以及其他相關設定</para>
    /// <para>請使用 "Start" 方法來啟動視窗！</para>
    /// </summary>
    /// <example>
    /// 以下示範如何開啟並回傳 SerialPort
    /// <code language="C#">
    /// CtSerial serial = new CtSerial();
    /// CtSerialSetup setupForm = new CtSerialSetup();
    /// setupForm.Start(ref serial);    //等待使用者設定完畢後，將自動獲得開啟連線之 CtSerial (即此處得 serial)
    /// setupForm.Dispose();
    /// </code>
    /// </example>
    public partial class CtSerialSetup : Form, ICtVersion {

        #region Version

        /// <summary>CtSerialSetup 版本訊息</summary>
        /// <remarks><code language="C#">
        /// 0.0.0  Chi Sha [2007/02/10]
        ///     + 建立基礎模組
        /// 
        /// 1.0.0  Ahern [2015/01/03]
        ///     + 轉換至 C#
        /// </code></remarks>
        public CtVersion Version { get { return new CtVersion(1, 0, 0, "2015/01/03", "Ahern Kuo"); } }

        #endregion

        #region Declaration - Fields
        private CtSerial rSerial;
        private System.IO.Ports.SerialPort mCom;
        private int mBaudRate = 9600;
        private int mDataBit = 8;
        private CtSerial.Handshake mHsk = CtSerial.Handshake.None;
        private CtSerial.Parity mParity = CtSerial.Parity.None;
        private CtSerial.StopBits mStopBit = CtSerial.StopBits.None;
        private bool mType = false;
        #endregion

        #region Function - Constructors
        /// <summary>Initialize the interface, please using "Start" function to execute</summary>
        public CtSerialSetup() {
            InitializeComponent();
        }
        #endregion

        #region Function - Core
        /// <summary>
        /// Execute and show the form for user setup
        /// <para>It will load available port at start-up and assign to ComboBox</para>
        /// </summary>
        /// <param name="serial">Reference of CtSerial</param>
        /// <returns>Status Code</returns>
        public Stat Start(ref CtSerial serial) {
            Stat stt = Stat.SUCCESS;
            mType = false;
            rSerial = serial;
            if (rSerial == null) rSerial = new CtSerial();

            CtInvoke.ComboBoxClear(cbCom);
            foreach (string item in CtSerial.GetPortNames()) {
                CtInvoke.ComboBoxAdd(cbCom, item);
            }

            if (cbCom.Items.Count < 1) CtMsgBox.Show("SerialPort", "There are not exist any available COM Port");
            else {
                CtInvoke.ComboBoxSelectedIndex(cbCom, 0);
                CtInvoke.ComboBoxSelectedIndex(cbBaudRate, 3);
                CtInvoke.RadioButtonChecked(rdbData8, true);
                CtInvoke.RadioButtonChecked(rdbStop1, true);
                CtInvoke.RadioButtonChecked(rdbParityNone, true);
                CtInvoke.RadioButtonChecked(rdbHskNone, true);

                this.ShowDialog();
            }
            
            if (DialogResult != DialogResult.OK) stt = Stat.WN_SYS_USRCNC;
            return stt;
        }

        /// <summary>Execute and show the form for user setup, it will expend with SerialPort object</summary>
        /// <param name="com">SerialPort object</param>
        /// <returns>Status Code</returns>
        public Stat Start(ref System.IO.Ports.SerialPort com) {
            Stat stt = Stat.SUCCESS;
            mType = true;
            mCom = com;
            if (mCom == null) mCom = new System.IO.Ports.SerialPort();

            CtInvoke.ComboBoxClear(cbCom);
            foreach (string item in System.IO.Ports.SerialPort.GetPortNames()) {
                CtInvoke.ComboBoxAdd(cbCom, item);
            }

            if (cbCom.Items.Count < 1) CtMsgBox.Show("SerialPort", "There are not exist any available COM Port");
            else {
                CtInvoke.ComboBoxSelectedIndex(cbCom, 0);
                CtInvoke.ComboBoxSelectedIndex(cbBaudRate, 3);
                CtInvoke.RadioButtonChecked(rdbData8, true);
                CtInvoke.RadioButtonChecked(rdbStop1, true);
                CtInvoke.RadioButtonChecked(rdbParityNone, true);
                CtInvoke.RadioButtonChecked(rdbHskNone, true);

                this.ShowDialog();
            }
            return stt;
        }
        #endregion

        #region Function - Interface Events
        private void rdbData7_CheckedChanged(object sender, EventArgs e) {
            mDataBit = 7;
        }

        private void rdbData8_CheckedChanged(object sender, EventArgs e) {
            mDataBit = 8;
        }

        private void rdbData9_CheckedChanged(object sender, EventArgs e) {
            mDataBit = 9;
        }

        private void rdbStopNone_CheckedChanged(object sender, EventArgs e) {
            mStopBit = CtSerial.StopBits.None;
        }

        private void rdbStop1_CheckedChanged(object sender, EventArgs e) {
            mStopBit = CtSerial.StopBits.One;
        }

        private void rdbStop15_CheckedChanged(object sender, EventArgs e) {
            mStopBit = CtSerial.StopBits.OnePointFive;
        }

        private void rdbStop2_CheckedChanged(object sender, EventArgs e) {
            mStopBit = CtSerial.StopBits.Two;
        }

        private void rdbParityNone_CheckedChanged(object sender, EventArgs e) {
            mParity = CtSerial.Parity.None;
        }

        private void rdbParityOdd_CheckedChanged(object sender, EventArgs e) {
            mParity = CtSerial.Parity.Odd;
        }

        private void rdbParityEven_CheckedChanged(object sender, EventArgs e) {
            mParity = CtSerial.Parity.Even;
        }

        private void rdbParityMark_CheckedChanged(object sender, EventArgs e) {
            mParity = CtSerial.Parity.Mark;
        }

        private void rdbParitySpace_CheckedChanged(object sender, EventArgs e) {
            mParity = CtSerial.Parity.Space;
        }

        private void rdbHskNone_CheckedChanged(object sender, EventArgs e) {
            mHsk = CtSerial.Handshake.None;
        }

        private void rdbHskX_CheckedChanged(object sender, EventArgs e) {
            mHsk = CtSerial.Handshake.XON_XOFF;
        }

        private void rdbHskRTS_CheckedChanged(object sender, EventArgs e) {
            mHsk = CtSerial.Handshake.RequestToSend;
        }

        private void rdbHskRTSX_CheckedChanged(object sender, EventArgs e) {
            mHsk = CtSerial.Handshake.RequestToSend_XON_XOFF;
        }

        private void btnConnect_Click(object sender, EventArgs e) {
            try {
                if (mType) {
                    /*-- Setting Enviroments --*/
                    mCom.PortName = cbCom.SelectedItem.ToString();
                    mCom.BaudRate = mBaudRate;
                    mCom.DataBits = mDataBit;
                    mCom.Parity = (System.IO.Ports.Parity)mParity;
                    mCom.Handshake = (System.IO.Ports.Handshake)mHsk;
                    mCom.StopBits = (System.IO.Ports.StopBits)mStopBit;
                    mCom.ReadTimeout = 1000;
                    mCom.WriteTimeout = 1000;

                    /*-- Open the com port --*/
                    mCom.Open();

                    if (mCom.IsOpen) {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                    }
                } else {
                    rSerial.Open(cbCom.SelectedItem.ToString(), mBaudRate, mDataBit, mStopBit, mParity, mHsk);
                    if (rSerial.IsOpen) {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                    }
                }
            } catch (Exception) {

                throw;
            }
        }

        private void btnExit_Click(object sender, EventArgs e) {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
        #endregion

        private void cbBaudRate_SelectedIndexChanged(object sender, EventArgs e) {
            mBaudRate = CtConvert.CInt(cbBaudRate.Text);
        }
    }
}
