using System;
using System.Collections.Generic;

using CtLib.Library;
using CtLib.Module.SerialPort;
using CtLib.Module.Utility;

namespace CtLib.Module.Keyence {

    /// <summary>
    /// Keynce DL-RS1A
    /// <para>Laser Sensor RS-232 Communication Box</para>
    /// </summary>
    public class CtKeyence_DL_RS1A : IDisposable, ICtVersion {

        #region Version

        /// <summary>Keyence DL-RS1A 版本訊息</summary>
        /// <remarks><code language="C#">
        /// 0.0.0  Ahern [2015/10/20]
        ///      + 建立基礎模組
        /// </code></remarks>
        public CtVersion Version { get { return new CtVersion(0, 0, 0, "2015/10/20", "Ahern Kuo"); } }

        #endregion

        #region Declaration - Support Class
        /// <summary>DL-RS1A 命令類型</summary>
        protected enum CmdType : byte {
            /// <summary>Read Single Amplifier</summary>
            SR,
            /// <summary>Read All Amplifiers</summary>
            M0,
            /// <summary>Read I/O Status</summary>
            MS,
            /// <summary>Write Settings To Single Amplifier</summary>
            SW,
            /// <summary>Write Settings To All Amplifiers</summary>
            AW,
            /// <summary>Error</summary>
            ER
        }

        /// <summary>通訊資料介面</summary>
        protected interface ICommData {
            /// <summary>取得狀態代碼</summary>
            Stat Status { get; }
            /// <summary>取得命令類型</summary>
            CmdType CommandType { get; }
            /// <summary>取得原始通訊訊息</summary>
            string GetReceivedData();
        }

        /// <summary>使用於讀取數值後回傳的通訊資料基底</summary>
        protected abstract class ReadBase : ICommData {
            /// <summary>狀態代碼</summary>
            protected Stat mStt = Stat.ER_COM_RCIV;
            /// <summary>命令類型</summary>
            protected CmdType mCmdType = CmdType.ER;
            /// <summary>數值</summary>
            protected float mValue = 0;
            /// <summary>原始訊息</summary>
            protected string mRxData = "";

            /// <summary>取得狀態代碼</summary>
            public Stat Status { get { return mStt; } }
            /// <summary>取得命令類型</summary>
            public CmdType CommandType { get { return mCmdType; } }
            /// <summary>取得數值</summary>
            public float Value { get { return mValue; } }
            /// <summary>取得原始訊息</summary>
            public string GetReceivedData() { return mRxData; }
        }

        /// <summary>使用於寫入數值後回傳的通訊資料基底</summary>
        protected abstract class WriteBase : ICommData {
            /// <summary>狀態代碼</summary>
            protected Stat mStt = Stat.ER_COM_RCIV;
            /// <summary>命令類型</summary>
            protected CmdType mCmdType = CmdType.SW;
            /// <summary>資料類型</summary>
            protected int mDataNo = 0;
            /// <summary>原始資料</summary>
            protected string mRxData = "";

            /// <summary>取得狀態代碼</summary>
            public Stat Status { get { return mStt; } }
            /// <summary>取的命令類型</summary>
            public CmdType CommandType { get { return mCmdType; } }
            /// <summary>取得資料類型</summary>
            public int DataNo { get { return mDataNo; } }
            /// <summary>取得原始資料</summary>
            public string GetReceivedData() { return mRxData; }
        }

        /// <summary>解析讀取單一資料</summary>
        private class CommRead : ReadBase {
            /// <summary>ID</summary>
            private byte mID = 0;
            /// <summary>資料類型</summary>
            private int mDataNo = 0;

            /// <summary>取得通訊模組編號</summary>
            public byte ID { get { return mID; } }
            /// <summary>取得資料類型</summary>
            public int DataNo { get { return mDataNo; } }

            /// <summary>建構解析資料</summary>
            /// <param name="id">通訊模組編號</param>
            /// <param name="dataNo">資料類型</param>
            /// <param name="data">數值</param>
            public CommRead(byte id, int dataNo, float data) {
                mCmdType = CmdType.SR;
                mID = id;
                mDataNo = dataNo;
                mValue = data;
                mStt = Stat.SUCCESS;
            }

            /// <summary>以通訊訊息解析資料</summary>
            /// <param name="rxData">通訊收到的訊息</param>
            public CommRead(string rxData) {
                mRxData = rxData;
                mCmdType = CmdType.SR;

                string[] strSplit = rxData.Trim().Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries);
                if (strSplit != null && strSplit.Length == 4) {
                    mID = byte.Parse(strSplit[1]);
                    mDataNo = int.Parse(strSplit[2]);
                    mValue = float.Parse(strSplit[3].Replace("+", ""));
                    mStt = Stat.SUCCESS;
                } else mStt = Stat.ER_COM_RCIV;
            }
        }

        /// <summary>解析錯誤資料</summary>
        private class CommError : ReadBase {
            /// <summary>發生錯誤的命令類型</summary>
            private CmdType mOriCmdType = CmdType.SR;

            /// <summary>取得發生錯誤的命令之類型</summary>
            public CmdType RequestedCommandType { get { return mOriCmdType; } }

            /// <summary>建構解析資料</summary>
            /// <param name="reqCmdType">發生錯誤的命令類型</param>
            /// <param name="errCode">錯誤代碼</param>
            public CommError(CmdType reqCmdType, float errCode) {
                mCmdType = CmdType.ER;
                mOriCmdType = reqCmdType;
                mValue = errCode;
                mStt = Stat.SUCCESS;
            }

            /// <summary>以通訊訊息解析資料</summary>
            /// <param name="rxData">通訊收到的訊息</param>
            public CommError(string rxData) {
                mRxData = rxData;
                mCmdType = CmdType.ER;

                string[] strSplit = rxData.Trim().Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries);
                if (strSplit != null && strSplit.Length == 3) {
                    mOriCmdType = (CmdType)Enum.Parse(typeof(CmdType), strSplit[1], true);
                    mValue = float.Parse(strSplit[2]);
                    mStt = Stat.ER_COM_RCIV;
                } else mStt = Stat.SUCCESS;
            }
        }

        /// <summary>解析寫入單一資料</summary>
        private class CommWrite : WriteBase {
            /// <summary>通訊模組編號</summary>
            private byte mID = 0;

            /// <summary>取得通訊模組編號</summary>
            public byte ID { get { return mID; } }

            /// <summary>建構解析資料</summary>
            /// <param name="id">通訊模組編號</param>
            /// <param name="dataNo">資料類型</param>
            public CommWrite(byte id, int dataNo) {
                mCmdType = CmdType.SW;
                mID = id;
                mDataNo = dataNo;
                mStt = Stat.SUCCESS;
            }

            /// <summary>以通訊訊息解析資料</summary>
            /// <param name="rxData">通訊收到的訊息</param>
            public CommWrite(string rxData) {
                mRxData = rxData;
                mCmdType = CmdType.SW;

                string[] strSplit = rxData.Trim().Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries);
                if (strSplit != null && strSplit.Length == 3) {
                    mID = byte.Parse(strSplit[1]);
                    mDataNo = int.Parse(strSplit[2]);
                    mStt = Stat.SUCCESS;
                } else mStt = Stat.ER_COM_RCIV;
            }
        }

        /// <summary>解析寫入全部放大器數值</summary>
        private class CommWriteAll : WriteBase {

            /// <summary>建構解析資料</summary>
            /// <param name="dataNo">資料類型</param>
            public CommWriteAll(int dataNo) {
                mCmdType = CmdType.AW;
                mDataNo = dataNo;
                mStt = Stat.SUCCESS;
            }

            /// <summary>以通訊訊息解析資料</summary>
            /// <param name="rxData">通訊收到的訊息</param>
            public CommWriteAll(string rxData) {
                mRxData = rxData;
                mCmdType = CmdType.AW;

                string[] strSplit = rxData.Trim().Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries);
                if (strSplit != null && strSplit.Length == 2) {
                    mDataNo = int.Parse(strSplit[1]);
                    mStt = Stat.SUCCESS;
                } else mStt = Stat.ER_COM_RCIV;
            }
        }

        /// <summary>解析讀取全部資料</summary>
        private class CommReadAll : ICommData {
            /// <summary>狀態代碼</summary>
            protected Stat mStt = Stat.ER_COM_RCIV;
            /// <summary>原始資校</summary>
            protected string mRxData = "";
            /// <summary>收到的數值</summary>
            protected List<float> mValue = new List<float>();

            /// <summary>取得狀態代碼</summary>
            public Stat Status { get { return mStt; } }
            /// <summary>取得命令類型</summary>
            public CmdType CommandType { get { return CmdType.M0; } }
            /// <summary>取得解析後的資料</summary>
            public List<float> Value { get { return mValue; } }

            /// <summary>建構解析資料</summary>
            /// <param name="data">數值</param>
            public CommReadAll(params float[] data) {
                mValue.AddRange(data);
                mStt = Stat.SUCCESS;
            }

            /// <summary>以通訊訊息解析資料</summary>
            /// <param name="rxData">通訊收到的訊息</param>
            public CommReadAll(string rxData) {
                mRxData = rxData;
                string[] strSplit = rxData.Trim().Split(CtConst.CHR_COMMA, StringSplitOptions.RemoveEmptyEntries);
                if (strSplit != null && strSplit.Length > 0) {
                    for (int idx = 1; idx < strSplit.Length; idx++) {
                        mValue.Add(float.Parse(strSplit[idx].Replace("+", "")));
                    }
                    mStt = Stat.SUCCESS;
                } else mStt = Stat.ER_COM_RCIV;

            }

            /// <summary>取得原始通訊訊息</summary>
            public string GetReceivedData() { return mRxData; }
        }
        #endregion

        #region Declaration - Definitions
        private static readonly CtSerial.Handshake DEFAULT_HANDSHAKE = CtSerial.Handshake.None;
        private static readonly CtSerial.Parity DEFAULT_PARITY = CtSerial.Parity.None;
        private static readonly CtSerial.StopBits DEFAULT_STOPBIT = CtSerial.StopBits.One;
        private static readonly int DEFAULT_BAUDRATE = 38400;
        private static readonly int DEFAULT_DATABIT = 8;
        #endregion

        #region Declaration - Fields
        private CtSerial mSerial = new CtSerial(false, TransDataFormats.String, EndChar.CrLf);
        #endregion

        #region Declaration - Properties
        /// <summary>取得是否已開啟通訊埠</summary>
        public bool IsConnected { get { return mSerial.IsOpen; } }
        #endregion

        #region Declaration - Disposable
        /// <summary>解除相關資源</summary>
        public void Dispose() {
            try {
                mSerial.Close();
                GC.SuppressFinalize(this);
            } catch (ObjectDisposedException ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
        }
        #endregion

        #region Function - Connections
        /// <summary>透過串列埠設定介面供使用者選擇連線</summary>
        /// <returns>Status Code</returns>
        public Stat Connect() {
            Stat stt = Stat.SUCCESS;
            try {
                mSerial.Open();
            } catch (Exception ex) {
                stt = Stat.ER_COM_OPEN;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }

        /// <summary>指定特定的連接埠並嘗試連線</summary>
        /// <param name="portName">串列埠編號，如 "COM6"</param>
        /// <returns>Status Code</returns>
        public Stat Connect(string portName) {
            Stat stt = Stat.SUCCESS;
            try {
                mSerial.Open(portName, DEFAULT_BAUDRATE, DEFAULT_DATABIT, DEFAULT_STOPBIT, DEFAULT_PARITY, DEFAULT_HANDSHAKE);
            } catch (Exception ex) {
                stt = Stat.ER_COM_OPEN;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }

        /// <summary>中斷連線</summary>
        public void Disconnect() {
            mSerial.Close();
        }
        #endregion

        #region Function - Private Methods
        /// <summary>傳送資料</summary>
        /// <param name="data">欲寫入 Buffer 的字串</param>
        /// <returns>Status Code</returns>
        private Stat SendData(string data) {
            Stat stt = Stat.SUCCESS;
            try {
                mSerial.ClearBuffer();
                mSerial.Send(data, EndChar.CrLf);
            } catch (Exception ex) {
                stt = Stat.ER_COM_SEND;
                CtStatus.Report(stt, ex);
            }
            return stt;
        }

        /// <summary>解碼訊息，並回傳相對應的解析資料</summary>
        /// <param name="rxData">透過通訊接收到的訊息</param>
        /// <returns>解析資料</returns>
        private ICommData Decode(string rxData) {
            ICommData comData;
            if (rxData == string.Empty) throw new ArgumentNullException("Receviced an empty data");
            if (rxData.StartsWith("SR")) {
                comData = new CommRead(rxData);
            } else if (rxData.StartsWith("M0")) {
                comData = new CommReadAll(rxData);
            } else if (rxData.StartsWith("SW")) {
                comData = new CommWrite(rxData);
            } else if (rxData.StartsWith("AW")) {
                comData = new CommWriteAll(rxData);
            } else if (rxData.StartsWith("ER")) {
                comData = new CommError(rxData);
            } else throw new System.IO.InvalidDataException("Non-supported command : " + rxData);
            return comData;
        }

        /// <summary>等待回傳資料</summary>
        /// <returns>收到的資料(已解析)</returns>
        private ICommData WaitResponse() {
            string strRxData;
            mSerial.Receive(out strRxData);
            return Decode(strRxData);
        }
        #endregion

        #region Function - Core
        /// <summary>讀取當前特定放大器之 R.V. 數值</summary>
        /// <param name="id">放大器編號</param>
        /// <returns>R.V. 數值</returns>
        public float GetValue(byte id = 0) {
            float fltResult = float.MinValue;

            string cmd = "SR," + id.ToString("00") + ",038";
            Stat stt = SendData(cmd);
            ICommData rxData = WaitResponse();
            if (rxData is CommRead) {
                CommRead obj = rxData as CommRead;
                if (obj.Status == Stat.SUCCESS) fltResult = obj.Value;
                else throw new Exception("Decode data failed. Origin data is \"" + obj.GetReceivedData() + "\"");
            } else if (rxData is CommError) {
                throw new Exception("Device return an error, error code is " + (rxData as CommError).Value.ToString());
            }

            return fltResult;
        }

        /// <summary>取得所有放大器的 R.V. 數值</summary>
        /// <returns>數值集合</returns>
        public List<float> GetAllValue() {
            List<float> values = null;

            string cmd = "M0";
            Stat stt = SendData(cmd);

            ICommData rxData = WaitResponse();
            if (rxData is CommReadAll) {
                CommReadAll obj = rxData as CommReadAll;
                if (obj.Status == Stat.SUCCESS) values = obj.Value;
                else throw new Exception("Decode data failed. Origin data is \"" + obj.GetReceivedData() + "\"");
            } else if (rxData is CommError) {
                throw new Exception("Device return an error, error code is " + (rxData as CommError).Value.ToString());
            }

            return values;
        }

        /// <summary>將目前高度設為 0</summary>
        /// <returns>Status Code</returns>
        public Stat SetZero() {
            string cmd = "AW,001,0";
            Stat stt = SendData(cmd);
            CommWriteAll rxData = WaitResponse() as CommWriteAll;
            if (rxData != null && rxData.Status == Stat.SUCCESS) {
                cmd = "AW,001,1";
                stt = SendData(cmd);
                rxData = WaitResponse() as CommWriteAll;
                stt = rxData.Status;
            }
            return stt;
        }
        #endregion
    }
}
