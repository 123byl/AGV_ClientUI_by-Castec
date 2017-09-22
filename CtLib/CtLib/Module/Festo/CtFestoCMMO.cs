using CtLib.Library;
using CtLib.Module.Ultity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace CtLib.Module.FESTO
{
    /// <summary>
    /// FESTO CMMO 相關控制
    /// <para>由於 FESTO 馬達與控制器差異較大，故使用前請先確認是否適用</para>
    /// </summary>
    public class CtFestoCMMO
    {
        #region Version

        /// <summary>
        /// 1.0.0  Bruce [2015/05/14]
        ///      + CtFestoCMMO
        /// </summary>
        /// 
        public static readonly CtVersion @Version = new CtVersion(1, 0, 0, "2015/05/14", "Bruce Lee");

        #endregion

        #region Declaration - Enumeration

        /// <summary>連線方式</summary>
        public enum eHigherOrderControl : byte
        {
            /// <summary>I/O</summary>
            IO = 0x00,
            /// <summary>FestoControlTool</summary>
            FCT = 0x01,
            /// <summary>ControlViaEthernet</summary>
            CVE = 0x02,
            /// <summary>WebServer</summary>
            WEB = 0x03
        }

        /// <summary>操作模式</summary>
        public enum eNominalOperatingMode : sbyte
        {
            /// <summary>NO_OPERATION_MODE_SELECTED</summary>
            NO_OPERATION_MODE_SELECTED = 0,
            /// <summary>POSITIONING_MODE</summary>
            POSITIONING_MODE = 1,
            /// <summary>SPEED_MODE</summary>
            SPEED_MODE = 3,
            /// <summary>FORCE_MODE</summary>
            FORCE_MODE = 4,
            /// <summary>HOMING_MODE</summary>
            HOMING_MODE = 6,
            /// <summary>JOG_POSITIVE</summary>
            JOG_POSITIVE = -3,
            /// <summary>JOG_NEGATIVE</summary>
            JOG_NEGATIVE = -4
        }

        /// <summary>度量衡</summary>
        public enum eUnitOfMeasurementConversionFactor : byte
        {
            /// <summary>UNDEFINED 未定義</summary>
            UNDEFINED = 0,
            /// <summary>METRE 米</summary>
            METRE = 1,
            /// <summary>INCH 英吋</summary>
            INCH = 2,
            /// <summary>REVOLUTIONS 公轉</summary>
            REVOLUTIONS = 3,
            /// <summary>DEGREE 角度</summary>
            DEGREE = 4,
        }

        #endregion

        #region Declaration - Definitions

        /// <summary>FESTO CMMO IP Address</summary>
        public static readonly string IP = "192.168.178.1";
        /// <summary>FESTO CMMO Port</summary>
        public static readonly string PORT = "49700";
        /// <summary>FESTO CMMO 連線方式</summary>
        public static readonly eHigherOrderControl ControlViaEthernet = eHigherOrderControl.CVE;

        #endregion

        #region Declaration - Properties

        /// <summary>
        /// 取得是否連線狀態
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return mConnectStatus;
            }
        }

        /// <summary>
        /// 取得是否Enable狀態
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return mEnableStatus;
            }
        }

        /// <summary>
        /// 取得是否結束狀態
        /// </summary>
        public bool IsTerminated
        {
            get
            {
                return mTerminateStatus;
            }
        }

        /// <summary>
        /// 取得StatusWord
        /// </summary>
        public long StatusWord
        {
            get
            {
                mStatusWord.Read(ref mStream);
                return mStatusWord.mValue;
            }
        }

        /// <summary>
        /// 設定或設定ControlWord
        /// </summary>
        public long ControlWord
        {
            set
            {
                mControlWord.Send(value, ref mStream);
            }
            get
            {
                mControlWord.Read(ref mStream);
                return mControlWord.mValue;
            }
        }

        /// <summary>
        /// 取得或設定控制權模式
        /// </summary>
        public eHigherOrderControl HigherOrderControl
        {
            set
            {
                mHigherOrderControl.Send(CtConvert.CLong(value), ref mStream);
            }
            get
            {
                mHigherOrderControl.Read(ref mStream);
                return (eHigherOrderControl)Enum.Parse(typeof(eHigherOrderControl), mHigherOrderControl.mValue.ToString());
            }
        }

        /// <summary>
        /// 取得或設定單步動作類別
        /// </summary>
        public long WriteSingleStepType
        {
            set
            {
                mWriteSingleStepType.Send(value, ref mStream);
            }
            get
            {
                mWriteSingleStepType.Read(ref mStream);
                return mWriteSingleStepType.mValue;
            }
        }

        /// <summary>
        /// 取得或設定單步動作位置
        /// </summary>
        public double WriteSingleStepPosition
        {
            set
            {
                mWriteSingleStepPosition.Send(SetMeasurementUnitData(value, "POSITION"), ref mStream);
            }
            get
            {
                mWriteSingleStepPosition.Read(ref mStream);
                return GetMeasurementUnitData(mWriteSingleStepPosition.mValue, "POSITION");
            }
        }

        /// <summary>
        /// 取得或設定單步動作速度
        /// </summary>
        public double WriteSingleStepVelocity
        {
            set
            {
                mWriteSingleStepVelocity.Send(SetMeasurementUnitData(value, "VELOCITY"), ref mStream);
            }
            get
            {
                mWriteSingleStepVelocity.Read(ref mStream);
                return GetMeasurementUnitData(mWriteSingleStepVelocity.mValue, "VELOCITY");
            }
        }

        ///// <summary>
        ///// 取得或設定單步動作加速度
        ///// </summary>
        //public double WriteDefaultAcceleration
        //{
        //    set
        //    {
        //        mWriteSingleStepAcceleration.Send(SetMeasurementUnitData(value, "ACCELEREATION"), ref mStream);
        //    }
        //    get
        //    {
        //        mWriteSingleStepAcceleration.Read(ref mStream);
        //        return GetMeasurementUnitData(mWriteSingleStepAcceleration.value, "ACCELEREATION");
        //    }
        //}

        /// <summary>
        /// 取得或設定預設動作速度
        /// </summary>
        public double DefaultVelocity
        {
            set
            {
                mDefaultVelocity.Send(SetMeasurementUnitData(value, "VELOCITY"), ref mStream);
            }
            get
            {
                mDefaultVelocity.Read(ref mStream);
                return GetMeasurementUnitData(mDefaultVelocity.mValue, "VELOCITY");
            }
        }

        /// <summary>
        /// 取得或設定預設動作速度
        /// </summary>
        public double DefaultAcceleration
        {
            set
            {
                mDefaultAcceleration.Send(SetMeasurementUnitData(value, "ACCELERATION"), ref mStream);
            }
            get
            {
                mDefaultAcceleration.Read(ref mStream);
                return GetMeasurementUnitData(mDefaultAcceleration.mValue, "ACCELERATION");
            }
        }

        /// <summary>
        /// 取得或設定記錄表內的記錄所引
        /// </summary>
        public long RecordNumberPreselection
        {
            set
            {
                mRecordNumberPreselection.Send(value, ref mStream);
            }
            get
            {
                mRecordNumberPreselection.Read(ref mStream);
                return mRecordNumberPreselection.mValue;
            }
        }

        /// <summary>
        /// 取得目前目標位置
        /// </summary>
        public double ActualPosition
        {
            get
            {
                mActualPosition.Read(ref mStream);
                return GetMeasurementUnitData(mActualPosition.mValue, "POSITION");
            }
        }

        /// <summary>
        /// 取得實際速度
        /// </summary>
        public double ActualVelocity
        {
            get
            {
                mActualVelocity.Read(ref mStream);
                return GetMeasurementUnitData(mActualVelocity.mValue, "ACCELERATION");
            }
        }

        /// <summary>
        /// 取得實際電流
        /// </summary>
        public double ActualCurrent
        {
            get
            {
                mActualCurrent.Read(ref mStream);
                return CtConvert.CDbl(mActualCurrent.mValue) / 1000;
            }
        }

        /// <summary>
        /// 取得實際扭力
        /// </summary>
        public double ActualForce
        {
            get
            {
                mActualForce.Read(ref mStream);
                return GetMeasurementUnitData(mActualForce.mValue, "FORCE");
            }
        }

        /// <summary>
        /// 取得設定位置
        /// </summary>
        public double SetPointPosition
        {
            get
            {
                mSetPointPosition.Read(ref mStream);
                return GetMeasurementUnitData(mSetPointPosition.mValue, "POSITION");
            }
        }

        /// <summary>
        /// 取得或設定操作模式
        /// </summary>
        public eNominalOperatingMode NominalOperatingMode
        {
            set
            {
                mNominalOperatingMode.Send(CtConvert.CInt(value), ref mStream);
            }
            get
            {
                mNominalOperatingMode.Read(ref mStream);
                return (eNominalOperatingMode)Enum.Parse(typeof(eNominalOperatingMode), mNominalOperatingMode.mValue.ToString());
            }
        }

        /// <summary>
        /// 取得目前記錄所引
        /// </summary>
        public long CurrentRecordNumber
        {
            get
            {
                mCurrentRecordNumber.Read(ref mStream);
                return mCurrentRecordNumber.mValue;
            }
        }

        /// <summary>
        /// 取得錯誤資訊
        /// </summary>
        public long ErrorWithTopPriorty
        {
            get
            {
                mErrorWithTopPriorty.Read(ref mStream);
                return mErrorWithTopPriorty.mValue;
            }
        }

        /// <summary>
        /// 取得警告資訊
        /// </summary>
        public long WarningWithTopPriorty
        {
            get
            {
                mWarningWithTopPriorty.Read(ref mStream);
                return mWarningWithTopPriorty.mValue;
            }
        }

        /// <summary>
        /// 取得Enable狀態的位元
        /// </summary>
        public long EnableStatusBitField
        {
            get
            {
                mEnableStatusBitField.Read(ref mStream);
                return mEnableStatusBitField.mValue;
            }
        }

        /// <summary>
        /// 取得或設定量測單位
        /// </summary>
        public eUnitOfMeasurementConversionFactor UnitOfMeasurementConversionFactor
        {
            set
            {
                mMesaurementUnit = value;
                mUnitOfMeasurementConversionFactor.Send(CtConvert.CInt(value), ref mStream);
            }
            get
            {
                mUnitOfMeasurementConversionFactor.Read(ref mStream);
                mMesaurementUnit = (eUnitOfMeasurementConversionFactor)Enum.Parse(typeof(eUnitOfMeasurementConversionFactor), mUnitOfMeasurementConversionFactor.mValue.ToString());
                return mMesaurementUnit;
            }
        }

        #endregion

        #region Declaration - Members

        private NetworkStream mStream;
        private TcpClient mClient;
        private int mControlWordValue;
        private bool mConnectStatus = false;
        private bool mEnableStatus = false;
        private bool mTerminateStatus = true;
        private eUnitOfMeasurementConversionFactor mMesaurementUnit;

        private SVE_Object mStatusWord = new SVE_Object(1, 0, SVE_Message.DataTypes.dtUINT32);
        private SVE_Object mControlWord = new SVE_Object(2, 0, SVE_Message.DataTypes.dtUINT32);
        private SVE_Object mHigherOrderControl = new SVE_Object(3, 0, SVE_Message.DataTypes.dtUINT08);

        //SubIndex 65 , 針對單步動作
        private SVE_Object mWriteSingleStepType = new SVE_Object(5, 65, SVE_Message.DataTypes.dtUINT08);
        private SVE_Object mWriteSingleStepPosition = new SVE_Object(6, 65, SVE_Message.DataTypes.dtSINT32);
        private SVE_Object mWriteSingleStepVelocity = new SVE_Object(7, 65, SVE_Message.DataTypes.dtSINT32);
        //private SVE_Object mWriteSingleStepAcceleration = new SVE_Object(9, 65, SVE_Message.DataTypes.dtSINT32);

        private SVE_Object mRecordNumberPreselection = new SVE_Object(31, 0, SVE_Message.DataTypes.dtUINT08);
        private SVE_Object mActualPosition = new SVE_Object(56, 0, SVE_Message.DataTypes.dtSINT32);
        private SVE_Object mActualVelocity = new SVE_Object(57, 0, SVE_Message.DataTypes.dtSINT32);
        private SVE_Object mActualCurrent = new SVE_Object(58, 0, SVE_Message.DataTypes.dtSINT32);
        private SVE_Object mActualForce = new SVE_Object(59, 0, SVE_Message.DataTypes.dtSINT16);
        private SVE_Object mSetPointPosition = new SVE_Object(60, 0, SVE_Message.DataTypes.dtSINT32);
        private SVE_Object mStoreData = new SVE_Object(107, 0, SVE_Message.DataTypes.dtUINT32);
        private SVE_Object mNominalOperatingMode = new SVE_Object(120, 0, SVE_Message.DataTypes.dtSINT08);
        private SVE_Object mCurrentRecordNumber = new SVE_Object(141, 0, SVE_Message.DataTypes.dtUINT08);
        private SVE_Object mErrorWithTopPriorty = new SVE_Object(191, 0, SVE_Message.DataTypes.dtUINT16);
        private SVE_Object mWarningWithTopPriorty = new SVE_Object(213, 0, SVE_Message.DataTypes.dtUINT16);
        private SVE_Object mUnitOfMeasurementConversionFactor = new SVE_Object(218, 0, SVE_Message.DataTypes.dtUINT08);
        private SVE_Object mEnableStatusBitField = new SVE_Object(358, 0, SVE_Message.DataTypes.dtUINT08);

        private SVE_Object mDefaultVelocity = new SVE_Object(414, 0, SVE_Message.DataTypes.dtSINT32);
        private SVE_Object mDefaultAcceleration = new SVE_Object(415, 65, SVE_Message.DataTypes.dtSINT32);

        #endregion

        #region Functions

        /// <summary>
        /// 建立連線 Terminate = false , Connect = true
        /// </summary>
        /// <param name="ip">機器IP</param>
        /// <param name="port">機器Port</param>
        public void Connection(string ip, string port)
        {
            try
            {
                mClient = new TcpClient(ip, CtConvert.CInt(port));
                mStream = mClient.GetStream();
                mConnectStatus = true;
                mTerminateStatus = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 關閉連線 Terminate = true , Connect = false
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                if (mClient != null)
                    mClient.Close();
                mClient = null;

                mTerminateStatus = true;
                mConnectStatus = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Enable機器
        /// </summary>
        public void Enable()
        {
            try
            {
                //Switch on disabled -> Ready to switch on ,EO=0 QS=1 EV=1 SO=0 (0110)
                SetControlWordBit("EO", 0);
                SetControlWordBit("QS", 1);
                SetControlWordBit("EV", 1);
                SetControlWordBit("SO", 0);
                ControlWord = mControlWordValue;

                //Ready to switch on -> Switch on , EO=0 QS=1 EV=1 SO=1 (0111)
                SetControlWordBit("SO", 1);
                ControlWord = mControlWordValue;

                //Switch on -> Operation enabled , EO=1 QS=1 EV=1 SO=1 (1111)
                SetControlWordBit("EO", 1);
                ControlWord = mControlWordValue;

                mEnableStatus = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Disable機器
        /// </summary>
        public void Disable()
        {
            try
            {
                //Error -> Switch on disabled ,EO=0 QS=0 EV=0 SO=0 (0000)
                SetControlWordBit("EO", 0);
                SetControlWordBit("QS", 0);
                SetControlWordBit("EV", 0);
                SetControlWordBit("SO", 0);
                ControlWord = mControlWordValue;

                mEnableStatus = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 單步動作
        /// </summary>
        /// <param name="Velocity">速度</param>
        /// <param name="Position">距離</param>
        public void SingleStep(double Velocity, double Position)
        {
            try
            {
                //Positioning Mode (1) - Write Position and Volecity Start
                NominalOperatingMode = eNominalOperatingMode.POSITIONING_MODE;

                WriteSingleStepType = 17;
                WriteSingleStepPosition = Position;
                WriteSingleStepVelocity = Velocity;

                RecordNumberPreselection = 65;

                ActionStart();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 持續漫步動作
        /// </summary>
        /// <param name="IsPositive">正轉 IsPositive = true , 反轉 IsPositive = false</param>
        public void JogStart(bool IsPositive)
        {
            try
            {
                if (IsPositive)
                {
                    //Jog Positive Mode (-3)
                    NominalOperatingMode = eNominalOperatingMode.JOG_POSITIVE;
                }
                else
                {
                    //Jog Positive Mode (-4)
                    NominalOperatingMode = eNominalOperatingMode.JOG_NEGATIVE;
                }

                ActionStart();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 停止漫步動作
        /// </summary>
        public void JogStop()
        {
            try
            {
                ActionStop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 歸位動作
        /// </summary>
        public void Homing()
        {
            try
            {
                //Homing Mode (6)
                NominalOperatingMode = eNominalOperatingMode.HOMING_MODE;

                ActionStart();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 錯誤發生，重置動作
        /// </summary>
        public void ErrorReset()
        {
            try
            {
                //Error Reset
                //Error -> Operation enabled , FR=1 PSON=1 EO=1 QS=1 EV=1 SO=1 (11001111)
                SetControlWordBit("FR", 0);
                ControlWord = mControlWordValue;

                SetControlWordBit("FR", 1);
                SetControlWordBit("PSON", 1);
                SetControlWordBit("EO", 1);
                SetControlWordBit("QS", 1);
                SetControlWordBit("EV", 1);
                SetControlWordBit("SO", 1);
                ControlWord = mControlWordValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 停止任何動作
        /// </summary>
        public void StopMotion()
        {
            try
            {
                //Operation enabled -> Quick stop active , FR=0 QS=0 EV=1
                SetControlWordBit("FR", 0);
                SetControlWordBit("QS", 0);
                SetControlWordBit("EV", 1);
                ControlWord = mControlWordValue;

                //Quick stop active -> Switch on disabled , FR=0 QS=0 EV=1
                SetControlWordBit("FR", 0);
                SetControlWordBit("EV", 0);
                ControlWord = mControlWordValue;

                Enable();

                //// Quick stop active -> Operation enabled , FR=0 EO=1 QS=1 EV=1 SO=1
                //SetControlWordBit("FR", 0);
                //SetControlWordBit("EO", 1);
                //SetControlWordBit("QS", 1);
                //SetControlWordBit("EV", 1);
                //SetControlWordBit("SO", 1);
                //ControlWord = mControlWordValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 取得單一列記錄表內的記錄
        /// </summary>
        /// <param name="index">記錄索引</param>
        /// <returns></returns>
        public List<string> GetRecordTableRow(ushort index)
        {
            try
            {
                string Type = GetRecordType(index);
                double PositionX = GetRecordPositionX(index);
                double PositionY = GetRecordPositionY(index);
                double Velocity = GetRecordVelocity(index);
                double Acceleration = GetRecordAcceleration(index);
                double Force = GetRecordForce(index);

                List<string> Row = new List<string>{index.ToString(), Type, PositionX.ToString("F3"),PositionY.ToString("F3"),
                Velocity.ToString("F3"), Acceleration.ToString("F3"), Force.ToString("F3")};

                return Row;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 執行單一列記錄表內的記錄
        /// </summary>
        /// <param name="index">記錄索引</param>
        public void StartRecordTableRow(int index)
        {
            try
            {
                //Positioning Mode (1) - Select Record Start
                NominalOperatingMode = eNominalOperatingMode.POSITIONING_MODE;
                RecordNumberPreselection = index;

                ActionStart();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 寫入單一列記錄表內的記錄
        /// </summary>
        /// <param name="index">記錄所引</param>
        /// <param name="Type">動作類別</param>
        /// <param name="PositionX">動作位置</param>
        /// <param name="PositionY">動作位置</param>
        /// <param name="Velocity">動作速度</param>
        /// <param name="Acceleration">動作加速度</param>
        /// <param name="Force">Force</param>

        public void SetRecordTableRow(ushort index, int Type, double PositionX, double PositionY, double Velocity, double Acceleration, double Force)
        {
            try
            {
                SVE_Object WriteType = new SVE_Object(5, index, SVE_Message.DataTypes.dtUINT08);
                SVE_Object WritePositionX = new SVE_Object(6, index, SVE_Message.DataTypes.dtSINT32);
                SVE_Object WritePositionY = new SVE_Object(302, index, SVE_Message.DataTypes.dtSINT32);
                SVE_Object WriteVelocity = new SVE_Object(7, index, SVE_Message.DataTypes.dtSINT32);
                SVE_Object WriteAcceleration = new SVE_Object(9, index, SVE_Message.DataTypes.dtSINT32);
                SVE_Object WriteForce = new SVE_Object(292, index, SVE_Message.DataTypes.dtUINT16);
                SVE_Object WriteMaxFollowingError = new SVE_Object(14, index, SVE_Message.DataTypes.dtSINT32);

                WriteType.Send(Type, ref mStream);
                WritePositionX.Send(SetMeasurementUnitData(PositionX, "POSITION"), ref mStream);
                WritePositionY.Send(SetMeasurementUnitData(PositionY, "POSITION"), ref mStream);
                WriteVelocity.Send(SetMeasurementUnitData(Velocity, "VELOCITY"), ref mStream);
                WriteAcceleration.Send(SetMeasurementUnitData(Acceleration, "ACCELERATION"), ref mStream);
                WriteForce.Send(SetMeasurementUnitData(Force, "FORCE"), ref mStream);

                //Hidden Parameter Max Following Error
                WriteMaxFollowingError.Send(SetMeasurementUnitData(5, "POSITION"), ref mStream);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 取得單一列記錄的動作類別
        /// </summary>
        /// <param name="index">記錄索引</param>
        /// <returns></returns>
        private string GetRecordType(ushort index)
        {
            try
            {
                SVE_Object TempSVE = new SVE_Object(5, index, SVE_Message.DataTypes.dtUINT08);
                TempSVE.Read(ref mStream);
                string Type;

                switch (TempSVE.mValue)
                {
                    case 0:
                        Type = "Inactive";
                        break;
                    case 16:
                        Type = "PA";
                        break;
                    case 17:
                        Type = "PRN";
                        break;
                    default:
                        Type = "";
                        break;
                }

                return Type;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }

        /// <summary>
        /// 取得單一列記錄的動作位置X
        /// </summary>
        /// <param name="index">記錄索引</param>
        /// <returns></returns>
        private double GetRecordPositionX(ushort index)
        {
            try
            {
                SVE_Object TempSVE = new SVE_Object(6, index, SVE_Message.DataTypes.dtSINT32);
                TempSVE.Read(ref mStream);
                double Position = GetMeasurementUnitData(TempSVE.mValue, "POSITION");

                return Position;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 取得單一列記錄的動作位置Y
        /// </summary>
        /// <param name="index">記錄索引</param>
        /// <returns></returns>
        private double GetRecordPositionY(ushort index)
        {
            try
            {
                SVE_Object TempSVE = new SVE_Object(302, index, SVE_Message.DataTypes.dtSINT32);
                TempSVE.Read(ref mStream);
                double Position = GetMeasurementUnitData(TempSVE.mValue, "POSITION");

                return Position;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 取得單一列記錄的動作速度
        /// </summary>
        /// <param name="index">記錄索引</param>
        /// <returns></returns>
        private double GetRecordVelocity(ushort index)
        {
            try
            {
                SVE_Object TempSVE = new SVE_Object(7, index, SVE_Message.DataTypes.dtSINT32);
                TempSVE.Read(ref mStream);
                double Velocity = GetMeasurementUnitData(TempSVE.mValue, "VELOCITY");

                return Velocity;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 取得單一列記錄的動作加速度
        /// </summary>
        /// <param name="index">記錄索引</param>
        /// <returns></returns>
        private double GetRecordAcceleration(ushort index)
        {
            try
            {
                SVE_Object TempSVE = new SVE_Object(9, index, SVE_Message.DataTypes.dtSINT32);
                TempSVE.Read(ref mStream);
                double Acceleration = GetMeasurementUnitData(TempSVE.mValue, "ACCELERATION");

                return Acceleration;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 取得單一列記錄的扭力
        /// </summary>
        /// <param name="index">記錄索引</param>
        /// <returns></returns>
        private double GetRecordForce(ushort index)
        {
            try
            {
                SVE_Object TempSVE = new SVE_Object(292, index, SVE_Message.DataTypes.dtSINT32);
                TempSVE.Read(ref mStream);
                double Force = GetMeasurementUnitData(TempSVE.mValue, "FORCE");

                return Force;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 設定ControlWord單一位元
        /// </summary>
        /// <param name="abbreviation">位元名稱</param>
        /// <param name="value">位元值</param>
        private void SetControlWordBit(string abbreviation, int value)
        {
            int mask;
            switch (abbreviation)
            {
                case "SO":
                    mask = 1 << 0;
                    break;
                case "EV":
                    mask = 1 << 1;
                    break;
                case "QS":
                    mask = 1 << 2;
                    break;
                case "EO":
                    mask = 1 << 3;
                    break;
                case "ST":
                    mask = 1 << 4;
                    break;
                case "PSON":
                    mask = 1 << 6;
                    break;
                case "FR":
                    mask = 1 << 7;
                    break;
                case "STP":
                    mask = 1 << 8;
                    break;
                default:
                    mask = 0;
                    break;
            }

            if (value == 1)
            {
                mControlWordValue |= mask;
            }
            else
            {
                mControlWordValue &= ~mask;
            }
        }

        /// <summary>
        /// 動作開始
        /// </summary>
        private void ActionStart()
        {
            //ST=0
            SetControlWordBit("ST", 0);
            ControlWord = mControlWordValue;

            //ST=1
            SetControlWordBit("ST", 1);
            ControlWord = mControlWordValue;
        }

        /// <summary>
        /// 動作結束
        /// </summary>
        private void ActionStop()
        {
            //ST=0
            SetControlWordBit("ST", 0);
            ControlWord = mControlWordValue;
        }

        /// <summary>
        /// 儲存資料進Device
        /// </summary>
        public void StoreDataToDevice()
        {
            mStoreData.Send(0xFFFFFFFF, ref mStream);
        }

        /// <summary>
        /// 寫入轉換單位的資料
        /// </summary>
        /// <param name="Value">Value</param>
        /// <param name="Type">POSITION , VELOCITY , ACCELEREATION</param>
        private long SetMeasurementUnitData(double Value, string Type)
        {
            long Result = 0;
            switch (mMesaurementUnit)
            {
                case (eUnitOfMeasurementConversionFactor.METRE):
                    switch (Type)
                    {
                        case "POSITION":
                            Result = CtConvert.CInt(Value * 1000);
                            break;
                        case "VELOCITY":
                            Result = CtConvert.CInt(Value * 1000);
                            break;
                        case "ACCELERATION":
                            Result = CtConvert.CInt(Value * 1000000);
                            break;
                        case "FORCE":
                            Result = CtConvert.CInt(Value / 100 * 1000);
                            break;
                        default:
                            break;
                    }
                    break;

                case (eUnitOfMeasurementConversionFactor.INCH):
                    switch (Type)
                    {
                        case "POSITION":
                            Result = CtConvert.CInt(Value * 1000 * 2.54);
                            break;
                        case "VELOCITY":
                            Result = CtConvert.CInt(Value * 1000);
                            break;
                        case "ACCELERATION":
                            Result = CtConvert.CInt(Value * 1000000);
                            break;
                        case "FORCE":
                            Result = CtConvert.CInt(Value / 100 * 1000);
                            break;
                        default:
                            break;
                    }
                    break;

                case (eUnitOfMeasurementConversionFactor.REVOLUTIONS):
                    switch (Type)
                    {
                        case "POSITION":
                            Result = CtConvert.CInt(Value * 100000);
                            break;
                        case "VELOCITY":
                            Result = CtConvert.CInt(Value / 60 * 100000);
                            break;
                        case "ACCELERATION":
                            Result = CtConvert.CInt(Value / 60 * 100000);
                            break;
                        case "FORCE":
                            Result = CtConvert.CInt(Value / 100 * 1000);
                            break;
                        default:
                            break;
                    }
                    break;

                case (eUnitOfMeasurementConversionFactor.DEGREE):
                    switch (Type)
                    {
                        case "POSITION":
                            Result = CtConvert.CInt(Value * 100000 / 360);
                            break;
                        case "VELOCITY":
                            Result = CtConvert.CInt(Value / 60 * 100000);
                            break;
                        case "ACCELERATION":
                            Result = CtConvert.CInt(Value / 60 * 100000);
                            break;
                        case "FORCE":
                            Result = CtConvert.CInt(Value / 100 * 1000);
                            break;
                        default:
                            break;
                    }
                    break;

                case (eUnitOfMeasurementConversionFactor.UNDEFINED):
                default:
                    break;
            }
            return Result;
        }

        /// <summary>
        /// 讀取轉換單位的資料
        /// </summary>
        /// <param name="Value">Value</param>
        /// <param name="Type">POSITION , VELOCITY , ACCELEREATION</param>
        private double GetMeasurementUnitData(long Value, string Type)
        {
            double Result = 0;
            switch (mMesaurementUnit)
            {
                case (eUnitOfMeasurementConversionFactor.METRE):
                    switch (Type)
                    {
                        case "POSITION":
                            Result = CtConvert.CDbl(Value) / 1000;
                            break;
                        case "VELOCITY":
                            Result = CtConvert.CDbl(Value) / 1000;
                            break;
                        case "ACCELERATION":
                            Result = CtConvert.CDbl(Value) / 1000000;
                            break;
                        case "FORCE":
                            Result = CtConvert.CDbl(Value) / 1000 * 100;
                            break;
                        default:
                            break;
                    }
                    Result = Math.Round(Result, 2);
                    break;

                case (eUnitOfMeasurementConversionFactor.INCH):
                    switch (Type)
                    {
                        case "POSITION":
                            Result = CtConvert.CDbl(Value) / 1000 / 2.54;
                            break;
                        case "VELOCITY":
                            Result = CtConvert.CDbl(Value) / 1000;
                            break;
                        case "ACCELERATION":
                            Result = CtConvert.CDbl(Value) / 1000000;
                            break;
                        case "FORCE":
                            Result = CtConvert.CDbl(Value) / 1000 * 100;
                            break;
                        default:
                            break;
                    }
                    Result = Math.Round(Result, 2);
                    break;

                case (eUnitOfMeasurementConversionFactor.REVOLUTIONS):
                    switch (Type)
                    {
                        case "POSITION":
                            Result = CtConvert.CDbl(Value) / 100000;
                            break;
                        case "VELOCITY":
                            Result = CtConvert.CDbl(Value) * 60 / 100000;
                            break;
                        case "ACCELERATION":
                            Result = CtConvert.CDbl(Value) * 60 / 100000;
                            break;
                        case "FORCE":
                            Result = CtConvert.CDbl(Value) / 1000 * 100;
                            break;
                        default:
                            break;
                    }
                    Result = Math.Round(Result, 3);
                    break;

                case (eUnitOfMeasurementConversionFactor.DEGREE):
                    switch (Type)
                    {
                        case "POSITION":
                            Result = CtConvert.CDbl(Value) / 100000 * 360;
                            break;
                        case "VELOCITY":
                            Result = CtConvert.CDbl(Value) * 60 / 100000;
                            break;
                        case "ACCELERATION":
                            Result = CtConvert.CDbl(Value) * 60 / 100000;
                            break;
                        case "FORCE":
                            Result = CtConvert.CDbl(Value) / 1000 * 100;
                            break;
                        default:
                            break;
                    }
                    Result = Math.Round(Result, 3);
                    break;

                case (eUnitOfMeasurementConversionFactor.UNDEFINED):
                default:
                    break;
            }
            return Result;
        }

        #endregion
    }
}
