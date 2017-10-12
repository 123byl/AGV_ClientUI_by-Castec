using System;
using System.Collections.Generic;
using System.Linq;

using CtLib.Library;
using CtLib.Module.Modbus;
using CtLib.Module.SerialPort;
using CtLib.Module.Utility;

namespace CtLib.Module.IAI {

    /// <summary>IAI PCON C/CG/CY/PL/PO/SE 系列之通訊，採用 Modbus RTU</summary>
    public class CtIAI_PCON : IDisposable, ICtVersion {

        #region Version

        /// <summary>CtIAI_PCON 版本訊息</summary>
        /// <remarks><code language="C#">
        /// 0.0.0  Ahern [2015/09/03]
        ///     + 建立基礎模組
        ///     
        /// </code></remarks>
        public CtVersion Version { get { return new CtVersion(0, 0, 0, "2015/09/03", "Ahern Kuo"); } }

        #endregion

        #region Declaration - Enumerations

        /// <summary>Jog 方向</summary>
        public enum JogDirection : ushort {
            /// <summary>正轉</summary>
            Forward = 0x0416,
            /// <summary>反轉</summary>
            Reverse = 0x0417
        }

        /// <summary>座標</summary>
        public enum Coordinate : byte {
            /// <summary>絕對座標</summary>
            Absolute = 0x00,
            /// <summary>增量座標</summary>
            Increment = 0x08
        }

        /// <summary>推壓模式</summary>
        public enum PushMode : byte {
            /// <summary>到達目標位置後開始往前推壓</summary>
            AfterReached = 0x00,
            /// <summary>在目標位置前減去兩倍定位距離後，開始推壓直至到達目標點位</summary>
            BeforeReached = 0x04
        }

        /// <summary>運動模式</summary>
        public enum MotionMode : byte {
            /// <summary>一般到點</summary>
            Normal = 0x00,
            /// <summary>推壓</summary>
            Push = 0x02
        }

        #endregion

        #region Declaration - Support Class
        /// <summary>軸的狀態</summary>
        public class AxisStatus {
            /// <summary>急停狀態   (<see langword="false"/>)正常 (<see langword="true"/>)急停中</summary>
            public bool EmergencyStop { get; private set; }
            /// <summary>安全速度限制狀態   (<see langword="false"/>)非限制 (<see langword="true"/>)安全限制</summary>
            public bool SafeSpeed { get; private set; }
            /// <summary>控制器預備狀態   (<see langword="false"/>)Not Ready (<see langword="true"/>)Ready</summary>
            public bool Ready { get; private set; }
            /// <summary>伺服ON/OFF狀態   (<see langword="false"/>)Servo Off (<see langword="true"/>)Servo ON</summary>
            public bool Servo { get; private set; }
            /// <summary>推壓動作無作用   (<see langword="false"/>)正常 (<see langword="true"/>)推壓動作無作用</summary>
            public bool PushFlow { get; private set; }
            /// <summary>嚴重錯誤   (<see langword="false"/>)無錯誤 (<see langword="true"/>)需冷開機</summary>
            public bool FatalAlarm { get; private set; }
            /// <summary>輕微錯誤   (<see langword="false"/>)無錯誤 (<see langword="true"/>)一般錯誤</summary>
            public bool LightAlarm { get; private set; }
            /// <summary>絕對編碼器(Encoder)錯誤   (<see langword="false"/>)正常 (<see langword="true"/>)Encoder錯誤</summary>
            public bool EncoderAlarm { get; private set; }
            /// <summary>剎車解除狀態   (<see langword="false"/>)剎車鎖定 (<see langword="true"/>)剎車解除</summary>
            public bool Break { get; private set; }
            /// <summary>暫停動作狀態   (<see langword="false"/>)正常 (<see langword="true"/>)暫停動作中</summary>
            public bool Pause { get; private set; }
            /// <summary>歸零動作完成狀態   (<see langword="false"/>)驅動軸未歸零 (<see langword="true"/>)驅動軸已歸零</summary>
            public bool Home { get; private set; }
            /// <summary>指定點動作完成狀態   (<see langword="false"/>)指定點動作未完 (<see langword="true"/>)指定動作完成</summary>
            public bool MoveEnd { get; private set; }

            /// <summary>建立一全新的 AxisStatus，請自行設定數字</summary>
            public AxisStatus() {
                EmergencyStop = false;
                SafeSpeed = false;
                Ready = false;
                Servo = false;
                PushFlow = false;
                FatalAlarm = false;
                LightAlarm = false;
                EncoderAlarm = false;
                Break = false;
                Pause = false;
                Home = false;
                MoveEnd = false;
            }

            /// <summary>藉由解碼後的字串解析各狀態，並將結果放至屬性(Propery)中</summary>
            /// <param name="data">解碼後的字串，請補滿長度為16!!</param>
            public AxisStatus(string data) {
                /*-- 9005 回傳長度為 16 --*/
                if (data.Length == 16) {
                    EmergencyStop = (data[0] == '0') ? false : true;
                    SafeSpeed = (data[1] == '0') ? false : true;
                    Ready = (data[2] == '0') ? false : true;
                    Servo = (data[3] == '0') ? false : true;
                    PushFlow = (data[4] == '0') ? false : true;
                    FatalAlarm = (data[5] == '0') ? false : true;
                    LightAlarm = (data[6] == '0') ? false : true;
                    EncoderAlarm = (data[7] == '0') ? false : true;
                    Break = (data[8] == '0') ? false : true;
                    Pause = (data[10] == '0') ? false : true;
                    Home = (data[11] == '0') ? false : true;
                    MoveEnd = (data[12] == '0') ? false : true;
                }
            }

            /// <summary>藉由解碼後的字串解析各狀態，並將結果放至屬性(Propery)中</summary>
            /// <param name="data">收到的數值</param>
            private void DecodeData(ushort data) {
                EmergencyStop = (data & 0x8000) == 0x8000 ? true : false;
                SafeSpeed = (data & 0x4000) == 0x4000 ? true : false;
                Ready = (data & 0x2000) == 0x2000 ? true : false;
                Servo = (data & 0x1000) == 0x1000 ? true : false;
                PushFlow = (data & 0x0800) == 0x0800 ? true : false;
                FatalAlarm = (data & 0x0400) == 0x0400 ? true : false;
                LightAlarm = (data & 0x0200) == 0x0200 ? true : false;
                EncoderAlarm = (data & 0x0100) == 0x0100 ? true : false;
                Break = (data & 0x0080) == 0x0080 ? true : false;
                Pause = (data & 0x0020) == 0x0020 ? true : false;
                Home = (data & 0x0010) == 0x0010 ? true : false;
                MoveEnd = (data & 0x0008) == 0x0008 ? true : false;
            }

            /// <summary>建構軸狀態，藉由解碼後的資料解析各狀態</summary>
            /// <param name="data">收到的數值</param>
            public AxisStatus(ushort data) {
                DecodeData(data);
            }

        }

        /// <summary>馬達狀態</summary>
        public class MotionStatus {
            /// <summary>負載判斷    (<see langword="false"/>)正常 (<see langword="true"/>)負載狀態(只對PCON-CF)</summary>
            public bool Load { get; private set; }
            /// <summary>推力狀態    (<see langword="false"/>)正常 (<see langword="true"/>)當推力達到馬達電流限制狀態</summary>
            public bool PushForce { get; private set; }
            /// <summary>教導模式狀態    (<see langword="false"/>)正常 (<see langword="true"/>)教導模式狀態</summary>
            public bool Teach { get; private set; }
            /// <summary>數據寫入狀態    (<see langword="false"/>)正常 (<see langword="true"/>)數據寫入EEPROM中</summary>
            public bool DataLoading { get; private set; }
            /// <summary>往前慢跑中    (<see langword="false"/>)沒事 (<see langword="true"/>)Jog+</summary>
            public bool Jog_Forward { get; private set; }
            /// <summary>往後慢跑中    (<see langword="false"/>)沒事 (<see langword="true"/>)Jog-</summary>
            public bool Jog_Back { get; private set; }
            /// <summary>位置7到達    (<see langword="false"/>)Servo Off  (<see langword="true"/>)電磁閥模式時，到達指定位置。Servo ON 指定為 true</summary>
            public bool Pos7End { get; private set; }
            /// <summary>位置6到達    (<see langword="false"/>)Servo Off  (<see langword="true"/>)電磁閥模式時，到達指定位置。Servo ON 指定為 true</summary>
            public bool Pos6End { get; private set; }
            /// <summary>位置5到達    (<see langword="false"/>)Servo Off  (<see langword="true"/>)電磁閥模式時，到達指定位置。Servo ON 指定為 true</summary>
            public bool Pos5End { get; private set; }
            /// <summary>位置4到達    (<see langword="false"/>)Servo Off  (<see langword="true"/>)電磁閥模式時，到達指定位置。Servo ON 指定為 true</summary>
            public bool Pos4End { get; private set; }
            /// <summary>位置3到達    (<see langword="false"/>)Servo Off  (<see langword="true"/>)電磁閥模式時，到達指定位置。Servo ON 指定為 true</summary>
            public bool Pos3End { get; private set; }
            /// <summary>位置2到達    (<see langword="false"/>)Servo Off  (<see langword="true"/>)電磁閥模式時，到達指定位置。Servo ON 指定為 true</summary>
            public bool Pos2End { get; private set; }
            /// <summary>位置1到達    (<see langword="false"/>)Servo Off  (<see langword="true"/>)電磁閥模式時，到達指定位置。Servo ON 指定為 true</summary>
            public bool Pos1End { get; private set; }
            /// <summary>位置0到達    (<see langword="false"/>)Servo Off  (<see langword="true"/>)電磁閥模式時，到達指定位置。Servo ON 指定為 true</summary>
            public bool Pos0End { get; private set; }

            /// <summary>建立一全新的 MotionStatus，請自行設定數字</summary>
            public MotionStatus() {
                Load = false;
                PushForce = false;
                Teach = false;
                DataLoading = false;
                Jog_Forward = false;
                Jog_Back = false;
                Pos7End = false;
                Pos6End = false;
                Pos5End = false;
                Pos4End = false;
                Pos3End = false;
                Pos2End = false;
                Pos1End = false;
                Pos0End = false;
            }

            /// <summary>藉由解碼後的字串解析各狀態，並將結果放至屬性(Propery)中</summary>
            /// <param name="data">解碼後的字串，請補滿長度為16!!</param>
            public MotionStatus(string data) {
                if (data.Length == 16) {
                    Load = (data[2] == '0') ? false : true;
                    PushForce = (data[3] == '0') ? false : true;
                    Teach = (data[4] == '0') ? false : true;
                    DataLoading = (data[5] == '0') ? false : true;
                    Jog_Forward = (data[6] == '0') ? false : true;
                    Jog_Back = (data[7] == '0') ? false : true;
                    Pos7End = (data[8] == '0') ? false : true;
                    Pos6End = (data[9] == '0') ? false : true;
                    Pos5End = (data[10] == '0') ? false : true;
                    Pos4End = (data[11] == '0') ? false : true;
                    Pos3End = (data[12] == '0') ? false : true;
                    Pos2End = (data[13] == '0') ? false : true;
                    Pos1End = (data[14] == '0') ? false : true;
                    Pos0End = (data[15] == '0') ? false : true;
                }
            }

            private void DecodeData(ushort data) {
                Load = (data & 0x2000) == 0x2000 ? true : false;
                PushForce = (data & 0x1000) == 0x1000 ? true : false;
                Teach = (data & 0x0800) == 0x0800 ? true : false;
                DataLoading = (data & 0x0400) == 0x0400 ? true : false;
                Jog_Forward = (data & 0x0200) == 0x0200 ? true : false;
                Jog_Back = (data & 0x0100) == 0x0100 ? true : false;
                Pos7End = (data & 0x0080) == 0x0080 ? true : false;
                Pos6End = (data & 0x0040) == 0x0040 ? true : false;
                Pos5End = (data & 0x0020) == 0x0020 ? true : false;
                Pos4End = (data & 0x0010) == 0x0010 ? true : false;
                Pos3End = (data & 0x0008) == 0x0008 ? true : false;
                Pos2End = (data & 0x0004) == 0x0004 ? true : false;
                Pos1End = (data & 0x0002) == 0x0002 ? true : false;
                Pos0End = (data & 0x0001) == 0x0001 ? true : false;
            }

            /// <summary>建構運動狀態</summary>
            /// <param name="data">收到的數值</param>
            public MotionStatus(ushort data) {
                DecodeData(data);
            }

            /// <summary>建構運動狀態</summary>
            /// <param name="data">收到的數值</param>
            public MotionStatus(List<byte> data) {
                if (data.Count == 2)
                    DecodeData((ushort)((data[0] << 8) + data[1]));
            }

        }

        /// <summary>當前動作狀態</summary>
        public class RunningStatus {
            /// <summary>急停輸入狀態    (<see langword="false"/>)急停輸入OFF (<see langword="true"/>)急停輸入ON</summary>
            public bool EmergencyStop { get; private set; }
            /// <summary>馬達電壓過低狀態    (<see langword="false"/>)正常 (<see langword="true"/>)馬達電壓過低狀態</summary>
            public bool MotorLowVolt { get; private set; }
            /// <summary>操作狀態    (<see langword="false"/>)自動模式 (<see langword="true"/>)手動模式</summary>
            public bool OperationMode { get; private set; }
            /// <summary>歸零動作    (<see langword="false"/>)正常 (<see langword="true"/>)歸零動作執行中</summary>
            public bool Homing { get; private set; }
            /// <summary>推壓動作    (<see langword="false"/>)正常 (<see langword="true"/>)以推壓動作運動時</summary>
            public bool Pushing { get; private set; }
            /// <summary>編碼器(Encoder)狀態    (<see langword="false"/>)PCON/ERC2/ACON在開機後為0，SCON常False (<see langword="true"/>)PCON/ERC2/ACON在開機後Servo On後轉為1</summary>
            public bool EncoderStatus { get; private set; }
            /// <summary>ModBus通訊中的PIO控制狀態    (<see langword="false"/>)可使用PIO (<see langword="true"/>)不可使用PIO</summary>
            public bool ModBusPIO { get; private set; }
            /// <summary>移動狀態    (<see langword="false"/>)驅動軸停止狀態 (<see langword="true"/>)驅動軸運動中</summary>
            public bool Moving { get; private set; }

            /// <summary>建立一全新的 RnningStatus，請自行設定數字</summary>
            public RunningStatus() {
                EmergencyStop = false;
                MotorLowVolt = false;
                OperationMode = false;
                Homing = false;
                Pushing = false;
                EncoderStatus = false;
                ModBusPIO = false;
                Moving = false;
            }

            /// <summary>藉由解碼後的字串解析各狀態，並將結果放至屬性(Propery)中</summary>
            /// <param name="data">解碼後的字串，請補滿長度為16!!</param>
            public RunningStatus(string data) {
                if (data.Length == 16) {
                    EmergencyStop = (data[0] == '0') ? false : true;
                    MotorLowVolt = (data[1] == '0') ? false : true;
                    OperationMode = (data[2] == '0') ? false : true;
                    Homing = (data[4] == '0') ? false : true;
                    Pushing = (data[5] == '0') ? false : true;
                    EncoderStatus = (data[6] == '0') ? false : true;
                    ModBusPIO = (data[7] == '0') ? false : true;
                    Moving = (data[10] == '0') ? false : true;
                }
            }

            private void DecodeData(ushort data) {
                EmergencyStop = (data & 0x8000) == 0x8000 ? true : false;
                MotorLowVolt = (data & 0x4000) == 0x4000 ? true : false;
                OperationMode = (data & 0x2000) == 0x2000 ? true : false;
                Homing = (data & 0x0800) == 0x0800 ? true : false;
                Pushing = (data & 0x0400) == 0x0400 ? true : false;
                EncoderStatus = (data & 0x0200) == 0x0200 ? true : false;
                ModBusPIO = (data & 0x0100) == 0x0100 ? true : false;
                Moving = (data & 0x0020) == 0x0020 ? true : false;
            }

            /// <summary>建構執行狀態</summary>
            /// <param name="data">收到的數值</param>
            public RunningStatus(ushort data) {
                DecodeData(data);
            }

            /// <summary>建構執行狀態</summary>
            /// <param name="data">收到的數值</param>
            public RunningStatus(List<byte> data) {
                if (data.Count == 2)
                    DecodeData((ushort)((data[0] << 8) + data[1]));
            }
        }

        /// <summary>系統狀態</summary>
        public class SystemStatus {
            /// <summary>自動Servo Off    (<see langword="false"/>)正常 (<see langword="true"/>)有設定自動Servo OFF並執行自動Servo OFF中</summary>
            public bool AutoServoOff { get; private set; }
            /// <summary>使用EEPROM記憶體    (<see langword="false"/>)不使用 (<see langword="true"/>)使用</summary>
            public bool EEPROM { get; private set; }
            /// <summary>操作狀態    (<see langword="false"/>)自動模式 (<see langword="true"/>)手動模式</summary>
            public bool OperationMode { get; private set; }
            /// <summary>歸零動作完成狀態    (<see langword="false"/>)驅動軸未有歸零 (<see langword="true"/>)驅動軸已曾歸零</summary>
            public bool Homed { get; private set; }
            /// <summary>Servo ON/OFF 狀態    (<see langword="false"/>)Servo OFF (<see langword="true"/>)Servo ON</summary>
            public bool Servo { get; private set; }
            /// <summary>伺服命令狀態    (<see langword="false"/>)Servo OFF 命令輸入中 (<see langword="true"/>)Servo ON 命令輸入中</summary>
            public bool Servoing { get; private set; }
            /// <summary>馬達電源狀態    (<see langword="false"/>)馬達電源關閉 (<see langword="true"/>)正常</summary>
            public bool Power { get; private set; }

            /// <summary>建立一全新的 MotionStatus，請自行設定數字</summary>
            public SystemStatus() {
                AutoServoOff = false;
                EEPROM = false;
                OperationMode = false;
                Homed = false;
                Servo = false;
                Servoing = false;
                Power = false;
            }

            /// <summary>藉由解碼後的字串解析各狀態，並將結果放至屬性(Propery)中</summary>
            /// <param name="data">解碼後的字串，請補滿長度為32!!</param>
            public SystemStatus(string data) {
                if (data.Length == 32) {
                    AutoServoOff = (data[14] == '0') ? false : true;
                    EEPROM = (data[15] == '0') ? false : true;
                    OperationMode = (data[27] == '0') ? false : true;
                    Homed = (data[28] == '0') ? false : true;
                    Servo = (data[29] == '0') ? false : true;
                    Servoing = (data[30] == '0') ? false : true;
                    Power = (data[31] == '0') ? false : true;
                }
            }

            private void DecodeData(int data) {
                AutoServoOff = (data & 0x20000) == 0x20000 ? true : false;
                EEPROM = (data & 0x10000) == 0x10000 ? true : false;
                OperationMode = (data & 0x10) == 0x10 ? true : false;
                Homed = (data & 0x08) == 0x08 ? true : false;
                Servo = (data & 0x04) == 0x04 ? true : false;
                Servoing = (data & 0x02) == 0x02 ? true : false;
                Power = (data & 0x01) == 0x01 ? true : false;
            }

            /// <summary>藉由解碼後的字串解析各狀態，並將結果放至屬性(Propery)中</summary>
            /// <param name="data">含有各資訊之數值</param>
            public SystemStatus(int data) {
                DecodeData(data);
            }

            /// <summary>藉由解碼後的字串解析各狀態，並將結果放至屬性(Propery)中</summary>
            /// <param name="data">未解碼之資料</param>
            public SystemStatus(List<byte> data) {
                DecodeData((data[0] << 24) + (data[1] << 16) + (data[2] << 8) + data[3]);
            }
        }

        /// <summary>座標相關資料</summary>
        public class PathData {
            /// <summary>該點位編號</summary>
            public int No { get; private set; }
            /// <summary>Target position</summary>
            public float Position { get; private set; }
            /// <summary>Positioning band</summary>
            public float PositioningBand { get; private set; }
            /// <summary>Speed command</summary>
            public float Speed { get; private set; }
            /// <summary>Individual zone boundary +</summary>
            public float IdvZoneBound_P { get; private set; }
            /// <summary>Individual zone boundary -</summary>
            public float IdvZoneBound_M { get; private set; }
            /// <summary>Acceleration command</summary>
            public float Accel { get; private set; }
            /// <summary>Deceleration command</summary>
            public float Decel { get; private set; }
            /// <summary>Push-current limiting value</summary>
            public float PushCurrent { get; private set; }
            /// <summary>Load current threshold</summary>
            public float LoadCurrent { get; private set; }
            /// <summary>Control flag specification</summary>
            public int CtrlFlag { get; private set; }

            private void DecodeData(List<byte> data) {
                if (data.Count == 30) {
                    Position = ((data[0] << 24) + (data[1] << 16) + (data[2] << 8) + data[3]) * 0.01F;
                    PositioningBand = ((data[4] << 24) + (data[5] << 16) + (data[6] << 8) + data[7]) * 0.01F;
                    Speed = ((data[8] << 24) + (data[9] << 16) + (data[10] << 8) + data[11]) * 0.01F;
                    IdvZoneBound_P = ((data[12] << 24) + (data[13] << 16) + (data[14] << 8) + data[15]) * 0.01F;
                    IdvZoneBound_M = ((data[16] << 24) + (data[17] << 16) + (data[18] << 8) + data[19]) * 0.01F;
                    Accel = ((data[20] << 8) + data[21]) * 0.01F;
                    Decel = ((data[22] << 8) + data[23]) * 0.01F;
                    PushCurrent = ((data[24] << 8) + data[25]) * 0.01F;
                    LoadCurrent = ((data[26] << 8) + data[27]) * 0.01F;
                    CtrlFlag = (data[28] << 8) + data[29];
                }
            }

            /// <summary>座標相關資料</summary>
            /// <param name="no">該點位編號</param>
            /// <param name="data">欲分析的資料</param>
            public PathData(int no, List<byte> data) {
                No = no;
                DecodeData(data);
            }
            /// <summary>座標相關資料</summary>
            /// <param name="pos">Target position</param>
            /// <param name="band">Positioning band</param>
            /// <param name="speed">Speed command</param>
            /// <param name="idvZonBod_p">Individual zone boundary +</param>
            /// <param name="idvZonBod_m">Individual zone boundary -</param>
            /// <param name="accel">Acceleration command</param>
            /// <param name="decel">Deceleration command</param>
            /// <param name="push">Push-current limiting value</param>
            /// <param name="load">Load current threshold</param>
            /// <param name="ctrlFlag">Control flag specification</param>
            public PathData(float pos, float band, float speed, float idvZonBod_p, float idvZonBod_m, float accel, float decel, float push, float load, int ctrlFlag) {
                No = 0;
                Position = pos;
                PositioningBand = band;
                Speed = speed;
                IdvZoneBound_P = idvZonBod_p;
                IdvZoneBound_M = idvZonBod_m;
                Accel = accel;
                Decel = decel;
                PushCurrent = push;
                LoadCurrent = load;
                CtrlFlag = ctrlFlag;
            }
            /// <summary>座標相關資料</summary>
            /// <param name="no">該點位編號</param>
            /// <param name="pos">Target position</param>
            /// <param name="band">Positioning band</param>
            /// <param name="speed">Speed command</param>
            /// <param name="idvZonBod_p">Individual zone boundary +</param>
            /// <param name="idvZonBod_m">Individual zone boundary -</param>
            /// <param name="accel">Acceleration command</param>
            /// <param name="decel">Deceleration command</param>
            /// <param name="push">Push-current limiting value</param>
            /// <param name="load">Load current threshold</param>
            /// <param name="ctrlFlag">Control flag specification</param>
            public PathData(int no, float pos, float band, float speed, float idvZonBod_p, float idvZonBod_m, float accel, float decel, float push, float load, int ctrlFlag) {
                No = no;
                Position = pos;
                PositioningBand = band;
                Speed = speed;
                IdvZoneBound_P = idvZonBod_p;
                IdvZoneBound_M = idvZonBod_m;
                Accel = accel;
                Decel = decel;
                PushCurrent = push;
                LoadCurrent = load;
                CtrlFlag = ctrlFlag;
            }
        }
        #endregion

        #region Declaration - Fields

        /// <summary>ModbusRTU 相關處理</summary>
        private CtModbus_RTU mModbusRTU = new CtModbus_RTU();

        #endregion

        #region Declaration - Events

        /// <summary>IAI PCON 事件</summary>
        public event EventHandler<PconEventArgs> OnIAIEvents;

        /// <summary>觸發 IAI PCON 事件</summary>
        /// <param name="events">IAI PCON 事件</param>
        /// <param name="value">事件所帶的參數</param>
        protected virtual void RaiseEvents(PconEvents events, object value) {
            EventHandler<PconEventArgs> handler = OnIAIEvents;
            if (handler != null) handler(this, new PconEventArgs(events, value));
        }
        #endregion

        #region Declaration - Properties

        /// <summary>取得目前是否已連線至 IAI PCON</summary>
        public bool IsConnected { get { return mModbusRTU.IsOpen; } }
        /// <summary>取得或設定串列埠鮑率(Baud Rate)</summary>
        public int BaudRate { get; set; }
        /// <summary>取得或設定資料位元數</summary>
        public byte DataBits { get; set; }
        /// <summary>取得或設定串列埠交握方式</summary>
        public CtSerial.Handshake HandShake { get; set; }
        /// <summary>取得或設定同位元方式</summary>
        public CtSerial.Parity Parity { get; set; }
        /// <summary>取得或設定停止位元數</summary>
        public CtSerial.StopBits StopBits { get; set; }
        /// <summary>取得或設定該 IAI PCON 局號。 數值為局號旋鈕加一，如旋鈕為 0，則局號為 1</summary>
        public byte DeviceID { get { return mModbusRTU.DeviceID; } set { mModbusRTU.DeviceID = value; } }
        /// <summary>取得當前電腦所安裝的串列埠名稱</summary>
        public List<string> SerialPorts { get { return CtSerial.GetPortNames(); } }

        #endregion

        #region Function - Disposable
        /// <summary>中斷與 IAI PCON 之連線，並釋放相關資源</summary>
        public void Dispose() {
            try {
                Dispose(true);
                GC.SuppressFinalize(this);
            } catch (ObjectDisposedException ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
        }

        /// <summary>中斷與 IAI PCON 之連線，並釋放相關資源</summary>
        /// <param name="isDisposing">是否為第一次釋放</param>
        protected virtual void Dispose(bool isDisposing) {
            try {
                if (isDisposing) {
                    mModbusRTU.Close();
                    mModbusRTU.OnSerialEvents -= mModbusRTU_OnSerialEvents;
                }
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
        }
        #endregion

        #region Function - Constructors

        /// <summary>建構 IAI PCON 控制模組</summary>
        public CtIAI_PCON() {
            mModbusRTU.OnSerialEvents += mModbusRTU_OnSerialEvents;
        }

        #endregion

        #region Function - Modbus RTU Events

        private void mModbusRTU_OnSerialEvents(object sender, SerialPort.SerialEventArgs e) {
            switch (e.Event) {
                case SerialPortEvents.Connection:
                    CtThread.AddWorkItem(obj => RaiseEvents(PconEvents.Connection, CtConvert.CBool(e.Value)));
                    break;
                case SerialPortEvents.DataReceived:
                    //ModbusRTU 裡面已經覆寫此事件，基本上是不會發出任何東西...
                    break;
                case SerialPortEvents.Error:
                    if (e.Value is string)
                        CtThread.AddWorkItem(obj => RaiseEvents(PconEvents.CommunicationError, e.Value as string));
                    else
                        CtThread.AddWorkItem(obj => RaiseEvents(PconEvents.DeviceError, e.Value as List<byte>));
                    break;
            }
        }

        #endregion

        #region Function - Connections

        /// <summary>嘗試連接至 IAI PCON</summary>
        /// <returns>Status Code</returns>
        public Stat Connect() {
            Stat stt = Stat.SUCCESS;
            try {
                mModbusRTU.Open();
            } catch (Exception ex) {
                stt = Stat.ER_COM_OPEN;
                RaiseEvents(PconEvents.CommunicationError, ex.Message);
            }
            return stt;
        }

        /// <summary>嘗試連接至 IAI PCON，並指定其串列埠編號</summary>
        /// <param name="comPort">串列埠編號，如 "COM3"</param>
        /// <returns>Status Code</returns>
        public Stat Connect(string comPort) {
            Stat stt = Stat.SUCCESS;
            try {
                mModbusRTU.Open(comPort, BaudRate, DataBits, StopBits, Parity, HandShake);
            } catch (Exception ex) {
                stt = Stat.ER_COM_OPEN;
                RaiseEvents(PconEvents.CommunicationError, ex.Message);
            }
            return stt;
        }

        /// <summary>嘗試連接至 IAI PCON，使用預設局號並設定串列埠屬性</summary>
        /// <param name="comPort">串列埠編號，如 "COM3"</param>
        /// <param name="baudRate">傳輸速度，鮑率(BaudRate)</param>
        /// <param name="dataBit">資料位元數</param>
        /// <param name="hsk">硬體交握方式</param>
        /// <param name="parity">同位元檢查</param>
        /// <param name="stopBit">停止位元</param>
        /// <returns>Status Code</returns>
        public Stat Connect(string comPort, int baudRate, byte dataBit, CtSerial.Handshake hsk, CtSerial.Parity parity, CtSerial.StopBits stopBit) {
            Stat stt = Stat.SUCCESS;
            try {
                mModbusRTU.Open(comPort, baudRate, dataBit, stopBit, parity, hsk);
            } catch (Exception ex) {
                stt = Stat.ER_COM_OPEN;
                RaiseEvents(PconEvents.CommunicationError, ex.Message);
            }
            return stt;
        }

        /// <summary>嘗試連接至 IAI PCON，使用指定的局號與串列埠編號</summary>
        /// <param name="deviceID">指定的局號，如 0x7F</param>
        /// <param name="comPort">指定的串列埠編號，如 "COM3"</param>
        /// <returns>Status Code</returns>
        public Stat Connect(byte deviceID, string comPort) {
            Stat stt = Stat.SUCCESS;
            mModbusRTU.DeviceID = deviceID;
            try {
                mModbusRTU.Open(comPort, BaudRate, DataBits, StopBits, Parity, HandShake);
            } catch (Exception ex) {
                stt = Stat.ER_COM_OPEN;
                RaiseEvents(PconEvents.CommunicationError, ex.Message);
            }
            return stt;
        }

        /// <summary>嘗試連接至 IAI PCON，使用指定的局號與串列埠屬性</summary>
        /// <param name="deviceID">指定的局號，如 0x7F</param>
        /// <param name="comPort">指定的串列埠編號，如 "COM3"</param>
        /// <param name="baudRate">傳輸速度，鮑率(BaudRate)</param>
        /// <param name="dataBit">資料位元數</param>
        /// <param name="hsk">硬體交握方式</param>
        /// <param name="parity">同位元檢查</param>
        /// <param name="stopBit">停止位元</param>
        /// <returns>Status Code</returns>
        public Stat Connect(byte deviceID, string comPort, int baudRate, byte dataBit, CtSerial.Handshake hsk, CtSerial.Parity parity, CtSerial.StopBits stopBit) {
            Stat stt = Stat.SUCCESS;
            mModbusRTU.DeviceID = deviceID;
            try {
                mModbusRTU.Open(comPort, baudRate, dataBit, stopBit, parity, hsk);
            } catch (Exception ex) {
                stt = Stat.ER_COM_OPEN;
                RaiseEvents(PconEvents.CommunicationError, ex.Message);
            }
            return stt;
        }

        /// <summary>中斷與 IAI PCON 之連線</summary>
        /// <returns>Status Code</returns>
        public Stat Disconnect() {
            Stat stt = Stat.SUCCESS;
            try {
                mModbusRTU.Close();
            } catch (Exception ex) {
                stt = Stat.ER_COM_OPEN;
                RaiseEvents(PconEvents.CommunicationError, ex.Message);
            }
            return stt;
        }
        #endregion

        #region Function - Methods

        /// <summary>統一檢查並發報狀態</summary>
        /// <param name="stt">欲檢查的 Status Code</param>
        /// <param name="value">通訊所收到的數值</param>
        /// <returns>修正後的 Status Code</returns>
        private Stat CheckReturnValue(Stat stt, List<byte> value) {
            Stat retStt = stt;
            if (stt == Stat.SUCCESS) {
                if (value != null) return retStt;
                else {
                    retStt = Stat.ER3_MB_UNDATA;
                    RaiseEvents(PconEvents.CommunicationError, "Communicate successfull, but received unrecognized data");
                }
            } else if (stt == Stat.ER3_MB_SLVERR) {
                if (value != null) RaiseEvents(PconEvents.DeviceError, value.ToList());
                else {
                    retStt = Stat.ER3_MB_UNDATA;
                    RaiseEvents(PconEvents.CommunicationError, "Error responded from slave device, but null argument");
                }
            } else if (stt == Stat.ER3_MB_COMTMO) {
                RaiseEvents(PconEvents.CommunicationError, "Waiting response timeout");
            } else {
                RaiseEvents(PconEvents.CommunicationError, "Received unrecognized data");
            }
            return retStt;
        }

        /// <summary>[FC03] 讀取暫存器(Register)數值，可為輸入或輸出之暫存器(Input, Output, InOut)</summary>
        /// <param name="addr">Register Address</param>
        /// <param name="regCount">連續讀取的暫存器數量。如 4000~4007 則輸入 8</param>
        /// <param name="value">收到回傳的數值</param>
        /// <returns>Status Code</returns>
        /// <remarks>以 Wago IO 為例，一個 Register 都是 16bit，所以回傳的資訊以 ushort (uint16) 為主</remarks>
        private Stat ReadHoldingRegister(ushort addr, ushort regCount, out List<byte> value) {
            //mSW.Restart();
            Stat stt = mModbusRTU.ReadHoldingRegister(addr, regCount, out value);
            //mSW.Stop();
            //Console.WriteLine("ReadHoldingRegister >> " + mSW.ElapsedMilliseconds.ToString() + " ms");
            return CheckReturnValue(stt, value);
        }

        /// <summary>[FC03] 讀取暫存器(Register)數值，並直接以 ushort 回傳。可為輸入或輸出之暫存器(Input, Output, InOut)</summary>
        /// <param name="addr">Register Address</param>
        /// <param name="regCount">連續讀取的暫存器數量。如 4000~4007 則輸入 8</param>
        /// <param name="result">(Null)讀取失敗 (HasValue)收到的數值</param>
        /// <returns>Status Code</returns>
        private Stat ReadHoldingRegister(ushort addr, ushort regCount, out ushort result) {
            ushort tempVal = ushort.MinValue;
            List<byte> value;
            Stat stt = mModbusRTU.ReadHoldingRegister(addr, regCount, out value);
            CheckReturnValue(stt, value);

            if (stt == Stat.SUCCESS)
                tempVal = (ushort)((value[0] << 8) + value[1]);

            result = tempVal;
            return stt;
        }

        /// <summary>[FC03] 讀取暫存器(Register)數值，並直接以 int 回傳。可為輸入或輸出之暫存器(Input, Output, InOut)</summary>
        /// <param name="addr">Register Address</param>
        /// <param name="regCount">連續讀取的暫存器數量。如 4000~4007 則輸入 8</param>
        /// <param name="result">(Null)讀取失敗 (HasValue)收到的數值</param>
        /// <returns>Status Code</returns>
        private Stat ReadHoldingRegister(ushort addr, ushort regCount, out int result) {
            int tempVal = int.MinValue;
            List<byte> value;
            Stat stt = mModbusRTU.ReadHoldingRegister(addr, regCount, out value);
            CheckReturnValue(stt, value);

            if (stt == Stat.SUCCESS)
                tempVal = (value[0] << 24) + (value[1] << 16) + (value[2] << 8) + value[3];

            result = tempVal;
            return stt;
        }

        /// <summary>[FC05] 設定單一 Output (Bit)</summary>
        /// <param name="addr">IO 位址。 如 0x0D03</param>
        /// <param name="value">欲變更的狀態</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        private Stat WriteSingleCoil(ushort addr, bool value, out List<byte> result) {
            //mSW.Restart();
            Stat stt = mModbusRTU.WriteSingleCoil(addr, value, out result);
            //mSW.Stop();
            //Console.WriteLine("ReadHoldingRegister >> " + mSW.ElapsedMilliseconds.ToString() + " ms");
            return CheckReturnValue(stt, result);
        }

        /// <summary>[FC06] 設定單一 Register (通常為 16bit)</summary>
        /// <param name="addr">Register 位址。 如 0x04D3</param>
        /// <param name="value">欲寫入的數值</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        private Stat WriteSingleRegister(ushort addr, ushort value, out List<byte> result) {
            //mSW.Restart();
            Stat stt = mModbusRTU.WriteSingleRegister(addr, value, out result);
            //mSW.Stop();
            //Console.WriteLine("ReadHoldingRegister >> " + mSW.ElapsedMilliseconds.ToString() + " ms");
            return CheckReturnValue(stt, result);
        }

        /// <summary>[FC16] 設定連續多個 Register</summary>
        /// <param name="addr">起始 Register 位址。 如 0x9000</param>
        /// <param name="value">欲寫入的數值</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        private Stat WriteMultiRegisters(ushort addr, List<ushort> value, out List<byte> result) {
            //mSW.Restart();
            Stat stt = mModbusRTU.WriteMultiRegisters(addr, value, out result);
            //mSW.Stop();
            //Console.WriteLine("ReadHoldingRegister >> " + mSW.ElapsedMilliseconds.ToString() + " ms");
            return CheckReturnValue(stt, result);
        }

        #endregion

        #region Function - Driver Settings

        /// <summary>選擇路徑編號</summary>
        /// <param name="pathNo">路徑編號</param>
        /// <returns>Status Code</returns>
        public Stat SelectPath(ushort pathNo) {
            List<byte> result;
            return WriteSingleRegister(0x0D03, pathNo, out result);
        }

        /// <summary>設定路徑資料</summary>
        /// <param name="pathNo">路徑編號</param>
        /// <param name="position">目標點位或增量數值</param>
        /// <param name="precision">精準度、容許誤差</param>
        /// <param name="speed">速度</param>
        /// <param name="regionP">正區域</param>
        /// <param name="regionN">負區域</param>
        /// <param name="accel">加速度</param>
        /// <param name="decel">減速度</param>
        /// <param name="pushForce">推力</param>
        /// <param name="load">容許推力</param>
        /// <param name="mvMethod">移動方式</param>
        /// <param name="psMethod">推壓方式</param>
        /// <param name="corMethod">增量或絕對座標</param>
        /// <returns>Status Code</returns>
        public Stat SetPath(byte pathNo, float position, float precision, float speed, float regionP, float regionN, float accel, float decel, float pushForce, float load, MotionMode mvMethod, PushMode psMethod, Coordinate corMethod) {
            ushort addr = (ushort)(0x1000 + pathNo * 16);

            ushort mode = (ushort)((byte)mvMethod + (byte)psMethod + (byte)corMethod);

            int pos = (int)(position / 0.01);
            ushort pos_L = (ushort)(pos & 0x0000FFFF);
            ushort pos_H = (ushort)(pos >> 16 & 0x0000FFFF);

            int pcs = (int)(precision / 0.01);
            ushort pcs_L = (ushort)(pcs & 0x0000FFFF);
            ushort pcs_H = (ushort)(pcs >> 16 & 0x0000FFFF);

            int spd = (int)(speed / 0.01);
            ushort spd_L = (ushort)(spd & 0x0000FFFF);
            ushort spd_H = (ushort)(spd >> 16 & 0x0000FFFF);

            int rgp = (int)(regionP / 0.01);
            ushort rgp_L = (ushort)(rgp & 0x0000FFFF);
            ushort rgp_H = (ushort)(rgp >> 16 & 0x0000FFFF);

            int rgn = (int)(regionN / 0.01);
            ushort rgn_L = (ushort)(rgn & 0x0000FFFF);
            ushort rgn_H = (ushort)(rgn >> 16 & 0x0000FFFF);

            ushort acl = (ushort)(accel / 0.01);
            ushort dcl = (ushort)(decel / 0.01);
            ushort psf = (ushort)(pushForce / 0.01);
            ushort lod = (ushort)(load / 0.01);

            List<byte> result;
            return WriteMultiRegisters(
                        addr,
                        new List<ushort> { pos_H, pos_L, pcs_H, pcs_L, spd_H, spd_L, rgp_H, rgp_L, rgn_H, rgn_L, acl, dcl, psf, lod, mode },
                        out result
                    );
        }

		/// <summary>設定手動移動模式</summary>
		/// <param name="mode">(<see langword="true"/>)吋動  (<see langword="false"/>)Jog</param>
		/// <returns>Status Code</returns>
        public Stat SetJogMode(bool mode) {
            List<byte> result;
            return WriteSingleCoil(0x0411, mode, out result);
        }

        #endregion

        #region Function - Driver State
		/// <summary>取得當前馬達位置</summary>
		/// <returns>馬達位置。<see langword="null"/> 表示讀取失敗</returns>
        public float? GetPosition() {
            float? value = null;
            int result;
            Stat stt = ReadHoldingRegister(0x9000, 2, out result);

            if (stt == Stat.SUCCESS) value = result * 0.01F;

            return value;
        }

		/// <summary>取得特定路徑編號的設定內容</summary>
		/// <param name="pathNo">欲取得的路徑編號</param>
		/// <returns>路徑設定資料</returns>
        public PathData GetPath(int pathNo) {
            PathData data = null;

            List<byte> result;
            Stat stt = ReadHoldingRegister((ushort)(0x1000 + pathNo * 16), 15, out result);

            if (stt == Stat.SUCCESS) data = new PathData(pathNo, result);

            return data;
        }

		/// <summary>取得特定範圍的路徑編號之設定內容</summary>
		/// <param name="startIndex">欲取得設定資料的起始編號。如要取得 No.4 ~ No.16 則此處帶入 4</param>
		/// <param name="count">欲取得設定資料的數量。如要取得 No.4 ~ No.16 則此處帶入 13</param>
		/// <returns>路徑設定資料集合</returns>
		public List<PathData> GetPath(byte startIndex, byte count) {
            List<PathData> data = new List<PathData>();

            for (int idx = startIndex; idx < startIndex + count; idx++) {
                PathData tempData = GetPath(idx);
                if (tempData != null) data.Add(tempData);
            }

            return data;
        }

		/// <summary>取得錯誤代碼</summary>
		/// <returns>錯誤代碼。<see langword="null"/> 表示讀取失敗</returns>
        public ushort? GetError() {
            ushort? result = null;
            ushort value = 0;
            Stat stt = ReadHoldingRegister(0x9002, 1, out value);
            if (stt == Stat.SUCCESS) result = value;
            return result;
        }

		/// <summary>取得軸(馬達)的狀態</summary>
		/// <returns>軸狀態</returns>
        public AxisStatus GetAxisStatus() {
            ushort value = 0;
            Stat stt = ReadHoldingRegister(0x9005, 1, out value);

            AxisStatus axStt = null;
            if (stt == Stat.SUCCESS) axStt = new AxisStatus(value);
            return axStt;
        }

		/// <summary>取得馬達移動狀態</summary>
		/// <returns>移動狀態</returns>
        public MotionStatus GetMotionStatus() {
            ushort value = 0;
            Stat stt = ReadHoldingRegister(0x9006, 1, out value);

            MotionStatus moStt = null;
            if (stt == Stat.SUCCESS) moStt = new MotionStatus(value);
            return moStt;
        }

		/// <summary>取得馬達執行狀態</summary>
		/// <returns>執行狀態</returns>
        public RunningStatus GetRunningStatus() {
            ushort value = 0;
            Stat stt = ReadHoldingRegister(0x9007, 1, out value);

            RunningStatus ruStt = null;
            if (stt == Stat.SUCCESS) ruStt = new RunningStatus(value);
            return ruStt;
        }

		/// <summary>取得 PCON 控制器當前狀態</summary>
		/// <returns>控制器狀態</returns>
        public SystemStatus GetSystemStatus() {
            int value = 0;
            Stat stt = ReadHoldingRegister(0x9008, 2, out value);

            SystemStatus sysStt = null;
            if (stt == Stat.SUCCESS) sysStt = new SystemStatus(value);
            return sysStt;
        }

		/// <summary>取得當前速度</summary>
		/// <returns>速度。<see langword="null"/> 表示讀取失敗</returns>
        public float? GetSpeed() {
            float? value = null;
            int result;
            Stat stt = ReadHoldingRegister(0x900A, 2, out result);

            if (stt == Stat.SUCCESS) value = result * 0.01F;

            return value;
        }

		/// <summary>取得電流值</summary>
		/// <returns>電流數值。<see langword="null"/> 表示讀取失敗</returns>
		public int? GetCurrent() {
            int? value = null;
            int result;
            Stat stt = ReadHoldingRegister(0x900C, 2, out result);

            if (stt == Stat.SUCCESS) value = result;

            return value;
        }

		/// <summary>取得脈衝壓差</summary>
		/// <returns>數值。<see langword="null"/> 表示讀取失敗</returns>
		public int? GetPulseDiff() {
            int? value = null;
            int result;
            Stat stt = ReadHoldingRegister(0x900E, 2, out result);

            if (stt == Stat.SUCCESS) value = result;

            return value;
        }

		/// <summary>取得開機時間</summary>
		/// <returns>已開機時間。<see langword="null"/> 表示讀取失敗</returns>
		public int? GetTime() {
            int? value = null;
            int result;
            Stat stt = ReadHoldingRegister(0x9010, 2, out result);

            if (stt == Stat.SUCCESS) value = result / 60000;

            return value;
        }

		/// <summary>取得手動移動模式</summary>
		/// <returns>(<see langword="true"/>)吋動  (<see langword="false"/>)連續移動 Jog</returns>
        public bool? GetJogMode() {
            bool? value = null;
            ushort result;
            Stat stt = ReadHoldingRegister(0x0411, 1, out result);

            if (stt == Stat.SUCCESS) value = result > 0 ? true : false;

            return value;
        }

        #endregion

        #region Function - Actions
		/// <summary>進行伺服動作</summary>
		/// <param name="OnOff">(<see langword="true"/>)Servo ON  (<see langword="false"/>)Servo OFF</param>
		/// <returns>Status Code</returns>
        public Stat Servo(bool OnOff) {
            List<byte> result;
            return WriteSingleCoil(0x0403, OnOff, out result);
        }

		/// <summary>釋放或咬住剎車</summary>
		/// <param name="OnOff">(<see langword="true"/>)剎車咬住  (<see langword="false"/>)剎車釋放掉</param>
		/// <returns>Status Code</returns>
		public Stat Breaker(bool OnOff) {
            List<byte> result;
            return WriteSingleCoil(0x0408, OnOff, out result);
        }

		/// <summary>手動移動</summary>
		/// <param name="dir">移動方向</param>
		/// <param name="OnOff">(<see langword="true"/>)開始進行移動  (<see langword="false"/>停止移動</param>
		/// <returns>Status Code</returns>
        public Stat Jog(JogDirection dir, bool OnOff) {
            List<byte> result;
            return WriteSingleCoil((ushort)dir, OnOff, out result);
        }

		/// <summary>控制器正常化(類似 Reset)</summary>
		/// <returns>Status Code</returns>
        public Stat Normalize() {
            List<byte> result;
            return WriteSingleCoil(0x040B, false, out result);
        }

		/// <summary>馬達復歸、尋找原點</summary>
		/// <returns>Status Code</returns>
        public Stat Home() {
            List<byte> result;

            Stat stt = Normalize();
            if (stt != Stat.SUCCESS) return stt;

            stt = WriteSingleCoil(0x040B, true, out result);
            if (stt != Stat.SUCCESS) return stt;

            stt = Normalize();

            return stt;
        }

		/// <summary>移動至特定位置</summary>
		/// <param name="position">欲前往的座標或移動量</param>
		/// <param name="precision">精準度、容許誤差</param>
		/// <param name="speed">平均速度</param>
		/// <param name="accel">加速</param>
		/// <param name="pushForce">推力大小</param>
		/// <param name="mvMethod">移動方式</param>
		/// <param name="psMethod">推壓方式</param>
		/// <param name="corMethod">增量或絕對座標</param>
		/// <returns>Status Code</returns>
        public Stat MoveTo(float position, float precision, int speed, float accel, int pushForce, MotionMode mvMethod, PushMode psMethod, Coordinate corMethod) {
            ushort mode = (ushort)((byte)mvMethod + (byte)psMethod + (byte)corMethod);

            int pos = (int)(position / 0.01);
            ushort pos_L = (ushort)(pos & 0x0000FFFF);
            ushort pos_H = (ushort)(pos >> 16 & 0x0000FFFF);

            int pcs = (int)(precision / 0.01);
            ushort pcs_L = (ushort)(pcs & 0x0000FFFF);
            ushort pcs_H = (ushort)(pcs >> 16 & 0x0000FFFF);

            int spd = (int)(speed / 0.01);
            ushort spd_L = (ushort)(spd & 0x0000FFFF);
            ushort spd_H = (ushort)(spd >> 16 & 0x0000FFFF);

            ushort acl = (ushort)(accel / 0.01);
            ushort psf = (ushort)(pushForce / 0.01);

            List<byte> result;
            return WriteMultiRegisters(
                        0x9900,
                        new List<ushort> { pos_H, pos_L, pcs_H, pcs_L, spd_H, spd_L, acl, psf, mode },
                        out result
                    );
        }

		/// <summary>移動至路徑編號所設定的位置，並使用其路徑設定內容進行移動</summary>
		/// <param name="index">欲使用的路徑編號</param>
		/// <returns>Status Code</returns>
        public Stat MoveTo(ushort index) {
            Stat stt = SelectPath(index);
            List<byte> result;
            if (stt == Stat.SUCCESS) stt = WriteSingleCoil(0x040C, true, out result);
            return stt;
        }

		/// <summary>電磁閥移動</summary>
		/// <param name="index">電磁閥路徑編號</param>
		/// <returns>Status Code</returns>
        public Stat SolenoidMove(ushort index) {
            List<byte> result;
            Stat stt = Normalize();
            if (stt == Stat.SUCCESS)
                stt = WriteSingleCoil((ushort)(0x0418 + index), true, out result);

            return stt;
        }

		/// <summary>教導路徑組，記錄下當前位置</summary>
		/// <param name="index">欲教導的路徑編號</param>
		/// <returns>Status Code</returns>
        public Stat Teach(ushort index) {
            Stat stt = SelectPath(index);
            List<byte> result;
            if (stt == Stat.SUCCESS) stt = WriteSingleCoil(0x0415, true, out result);
            return stt;
        }

		/// <summary>清除錯誤。此功能僅能清除軟體錯誤！如遇硬體錯誤請查詢官方手冊之疑難排解</summary>
		/// <returns>Status Code</returns>
        public Stat CleanAlarm() {
            List<byte> result;
            Stat stt = WriteSingleCoil(0x0407, true, out result);
            stt = WriteSingleCoil(0x0407, false, out result);
            return stt;
        }

        #endregion
    }
}
