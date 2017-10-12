using System;
using System.Collections.Generic;
using System.Linq;

using CtLib.Library;
using CtLib.Module.Modbus;
using CtLib.Module.SerialPort;
using CtLib.Module.Utility;

namespace CtLib.Module.Oriental {

    /// <summary>
    /// Oriental BLE 系列 FLEX RS-485 通訊型，採用 ModbusRTU 模式
    /// <para>目前提供簡易資訊查詢與馬達控制，尚待持續補充</para>
    /// </summary>
    /// <remarks>BLE 為「無刷馬達」(Brushless Motor)</remarks>
    public class CtOriental_BLE : IDisposable, ICtVersion {

        #region Version

        /// <summary>CtOriental_BLE_FLEX-A2 版本訊息</summary>
        /// <remarks><code language="C#">
        /// 0.0.0  Ahern [2015/08/27]
        ///     + 建立基礎模組
        ///     
        /// </code></remarks>
        public CtVersion Version { get { return new CtVersion(0, 0, 0, "2015/08/27", "Ahern Kuo"); } }

        #endregion

        #region Declaration - Support Class
        /// <summary>Oriental BLE FLEX I/O 狀態</summary>
        public class IOState {
            /// <summary>輸入(Input) 0</summary>
            public bool IN_0 { get; private set; }
            /// <summary>輸入(Input) 1</summary>
            public bool IN_1 { get; private set; }
            /// <summary>輸入(Input) 2</summary>
            public bool IN_2 { get; private set; }
            /// <summary>輸入(Input) 3</summary>
            public bool IN_3 { get; private set; }
            /// <summary>輸入(Input) 4</summary>
            public bool IN_4 { get; private set; }
            /// <summary>輸入(Input) 5</summary>
            public bool IN_5 { get; private set; }
            /// <summary>輸入(Input) 6</summary>
            public bool IN_6 { get; private set; }
            /// <summary>輸出(Output) 0</summary>
            public bool OUT_0 { get; private set; }
            /// <summary>輸出(Output) 1</summary>
            public bool OUT_1 { get; private set; }
            /// <summary>建構 I/O 狀態</summary>
            /// <param name="value">透過 Modbus RTU 讀取到的數值</param>
            public IOState(int value) {
                AnalysisIO(value);
            }
            /// <summary>建構 I/O 狀態</summary>
            /// <param name="value">透過 Modbus RTU 讀取到的數值</param>
            public IOState(IEnumerable<byte> value) {
                if (value.Count() == 4) {
                    int temp = value.ElementAt(0) * 256 + value.ElementAt(1);
                    temp <<= 16;
                    temp += value.ElementAt(2) * 256 + value.ElementAt(3);
                    AnalysisIO(temp);
                }

            }
            /// <summary>從數值分析各 I/O 資料</summary>
            /// <param name="io">欲分析的資料</param>
            private void AnalysisIO(int io) {
                IN_0 = (io & 0x00000001) == 0x00000001 ? true : false;
                IN_1 = (io & 0x00000002) == 0x00000002 ? true : false;
                IN_2 = (io & 0x00000004) == 0x00000004 ? true : false;
                IN_3 = (io & 0x00000008) == 0x00000008 ? true : false;
                IN_4 = (io & 0x00000010) == 0x00000010 ? true : false;
                IN_5 = (io & 0x00000020) == 0x00000020 ? true : false;
                IN_6 = (io & 0x00000040) == 0x00000040 ? true : false;
                OUT_0 = (io & 0x00010000) == 0x00010000 ? true : false;
                OUT_1 = (io & 0x00020000) == 0x00020000 ? true : false;
            }
        }
        #endregion

        #region Declaration - Enumerations

        /// <summary>例外代碼，顯示無法處理的原因</summary>
        public enum Exceptions : byte {
            /// <summary>[0x88] 不正確的功能</summary>
            /// <remarks>
            /// 由於功能代碼不正確，無法執行
            /// <para>1. 未對應的功能代碼</para>
            /// <para>2. 診斷(08h)的子功能代碼在 00h 以外</para>
            /// </remarks>
            InvalidFunction = 0x01,
            /// <summary>[0x88] 不正確的資料位址</summary>
            /// <remarks>
            /// 由於資料位址不正確，無法執行
            /// <para>1. 未對應的寄存器位址(0000h ~ 1FFFh 以外)</para>
            /// <para>2. 寄存器位址與寄存器數的和在 2000h 以上</para>
            /// </remarks>
            InvalidAddress = 0x02,
            /// <summary>[0x8C] 不正確資料</summary>
            /// <remarks>
            /// 由於資料不正確，無法執行
            /// <para>1. 寄存器數為 0 或 17 以上</para>
            /// <para>2. 位元數為寄存器數 x2 以外的值</para>
            /// <para>3. 資料長度在範圍外</para>
            /// </remarks>
            InvalidData = 0x03,
            /// <summary>[0x89][0x8A][0x8C][0x8D] 從屬裝置錯誤</summary>
            /// <remarks>
            /// 由於從屬裝置發生錯誤，因此無法執行
            /// <para>1. 與使用者 I/F 通訊中(89h)</para>
            /// <para>2. NV	記憶體處理中(8Ah)</para>
            /// <para>3. 參數設定範圍外(8Ch)</para>
            /// <para>4. 指令無法執行(8Dh)</para>
            /// </remarks>
            DeviceError = 0x04
        }

        /// <summary>類比輸入致能模式</summary>
        public enum AnalogyInputMode : byte {
            /// <summary>禁用類比輸入</summary>
            Disable = 0x00,
            /// <summary>啟用類比速度設定</summary>
            /// <remarks>僅能設定第 0 組速度時間</remarks>
            Velocity = 0x01,
            /// <summary>啟用類比轉矩限制設定</summary>
            /// <remarks>僅能設定第 0 組轉矩設定</remarks>
            TorqueLimit = 0x02
        }

        /// <summary>輸入(Input)功能編號</summary>
        public enum Func_IN : byte {
            /// <summary>輸入功能第 0 組</summary>
            IN_0 = 0,
            /// <summary>輸入功能第 1 組</summary>
            IN_1 = 1,
            /// <summary>輸入功能第 2 組</summary>
            IN_2 = 2,
            /// <summary>輸入功能第 3 組</summary>
            IN_3 = 3,
            /// <summary>輸入功能第 4 組</summary>
            IN_4 = 4,
            /// <summary>輸入功能第 5 組</summary>
            IN_5 = 5,
            /// <summary>輸入功能第 6 組</summary>
            IN_6 = 6
        }

        /// <summary>輸出(Output)功能編號</summary>
        public enum Func_OUT : byte {
            /// <summary>輸出功能第 0 組</summary>
            OUT_0 = 0,
            /// <summary>輸出功能第 1 組</summary>
            OUT_1 = 1
        }

        /// <summary>通訊用輸入(Net-Input)功能編號</summary>
        public enum Func_NET_IN : byte {
            /// <summary>網路通訊輸入功能第 0 組</summary>
            NET_IN_0 = 0,
            /// <summary>網路通訊輸入功能第 1 組</summary>
            NET_IN_1 = 1,
            /// <summary>網路通訊輸入功能第 2 組</summary>
            NET_IN_2 = 2,
            /// <summary>網路通訊輸入功能第 3 組</summary>
            NET_IN_3 = 3,
            /// <summary>網路通訊輸入功能第 4 組</summary>
            NET_IN_4 = 4,
            /// <summary>網路通訊輸入功能第 5 組</summary>
            NET_IN_5 = 5,
            /// <summary>網路通訊輸入功能第 6 組</summary>
            NET_IN_6 = 6,
            /// <summary>網路通訊輸入功能第 7 組</summary>
            NET_IN_7 = 7,
            /// <summary>網路通訊輸入功能第 8 組</summary>
            NET_IN_8 = 8,
            /// <summary>網路通訊輸入功能第 9 組</summary>
            NET_IN_9 = 9,
            /// <summary>網路通訊輸入功能第 10 組</summary>
            NET_IN_10 = 10,
            /// <summary>網路通訊輸入功能第 11 組</summary>
            NET_IN_11 = 11,
            /// <summary>網路通訊輸入功能第 12 組</summary>
            NET_IN_12 = 12,
            /// <summary>網路通訊輸入功能第 13 組</summary>
            NET_IN_13 = 13,
            /// <summary>網路通訊輸入功能第 14 組</summary>
            NET_IN_14 = 14,
            /// <summary>網路通訊輸入功能第 15 組</summary>
            NET_IN_15 = 15
        }

        /// <summary>通訊用輸出(Net-Output)功能編號</summary>
        public enum Func_NET_OUT : byte {
            /// <summary>網路通訊輸出功能第 0 組</summary>
            NET_OUT_0 = 0,
            /// <summary>網路通訊輸出功能第 1 組</summary>
            NET_OUT_1 = 1,
            /// <summary>網路通訊輸出功能第 2 組</summary>
            NET_OUT_2 = 2,
            /// <summary>網路通訊輸出功能第 3 組</summary>
            NET_OUT_3 = 3,
            /// <summary>網路通訊輸出功能第 4 組</summary>
            NET_OUT_4 = 4,
            /// <summary>網路通訊輸出功能第 5 組</summary>
            NET_OUT_5 = 5,
            /// <summary>網路通訊輸出功能第 6 組</summary>
            NET_OUT_6 = 6,
            /// <summary>網路通訊輸出功能第 7 組</summary>
            NET_OUT_7 = 7,
            /// <summary>網路通訊輸出功能第 8 組</summary>
            NET_OUT_8 = 8,
            /// <summary>網路通訊輸出功能第 9 組</summary>
            NET_OUT_9 = 9,
            /// <summary>網路通訊輸出功能第 10 組</summary>
            NET_OUT_10 = 10,
            /// <summary>網路通訊輸出功能第 11 組</summary>
            NET_OUT_11 = 11,
            /// <summary>網路通訊輸出功能第 12 組</summary>
            NET_OUT_12 = 12,
            /// <summary>網路通訊輸出功能第 13 組</summary>
            NET_OUT_13 = 13,
            /// <summary>網路通訊輸出功能第 14 組</summary>
            NET_OUT_14 = 14,
            /// <summary>網路通訊輸出功能第 15 組</summary>
            NET_OUT_15 = 15
        }

        /// <summary>輸入(Input)功能參數</summary>
        public enum Param_IN : byte {
            /// <summary>於不使用輸入端子時設定</summary>
            NO_USE = 0,
            /// <summary>馬達朝 正轉(Forward) 方向旋轉</summary>
            FWD = 1,
            /// <summary>馬達朝 反轉(Reverse) 方向旋轉</summary>
            REV = 2,
            /// <summary>選擇瞬間停止或減速停止</summary>
            STOP_MODE = 19,
            /// <summary>開放電磁剎車</summary>
            MB_FREE = 20,
            /// <summary>馬達停止 (常閉)</summary>
            EXT_ERROR = 21,
            /// <summary>馬達停止 (常閉)</summary>
            TH = 22,
            /// <summary>重設發生中的 Alarm。 下緣觸發(Falling-Triggered)</summary>
            ALARM_RESET = 24,
            /// <summary>解除 OPX-2A 和 MEXE02 的功能限制 (常閉)</summary>
            HMI = 27,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用</summary>
            R0 = 32,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用</summary>
            R1 = 33,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用</summary>
            R2 = 34,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用</summary>
            R3 = 35,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用</summary>
            R4 = 36,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用</summary>
            R5 = 37,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用</summary>
            R6 = 38,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用</summary>
            R7 = 39,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用</summary>
            R8 = 40,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用</summary>
            R9 = 41,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用</summary>
            R10 = 42,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用</summary>
            R11 = 43,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用</summary>
            R12 = 44,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用</summary>
            R13 = 45,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用</summary>
            R14 = 46,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用</summary>
            R15 = 47,
            /// <summary>使用 4 個位元選擇運轉資料 No.</summary>
            M0 = 48,
            /// <summary>使用 4 個位元選擇運轉資料 No.</summary>
            M1 = 49,
            /// <summary>使用 4 個位元選擇運轉資料 No.</summary>
            M2 = 50,
            /// <summary>使用 4 個位元選擇運轉資料 No.</summary>
            M3 = 51,
            /// <summary>使轉矩限制無效 (常閉)</summary>
            TL = 54
        }

        /// <summary>輸出(Output)功能參數</summary>
        public enum Param_OUT : byte {
            /// <summary>於不使用輸入端子時設定</summary>
            NO_USE = 0,
            /// <summary>輸出對 FWD 輸入的響應(Response)</summary>
            FWD_R = 1,
            /// <summary>輸出對 REV 輸入的響應(Response)</summary>
            REV_R = 2,
            /// <summary>輸出對 STOP-MODE 輸入的響應(Response)</summary>
            STOP_MODE_R = 19,
            /// <summary>輸出對 MB-FREE 輸入的響應(Response)</summary>
            MB_FREE_R = 20,
            /// <summary>輸出對 HMI 輸入的響應(Response)</summary>
            HMI_R = 27,
            /// <summary>輸出通用信號 R0 的狀態</summary>
            R0 = 32,
            /// <summary>輸出通用信號 R1 的狀態</summary>
            R1 = 33,
            /// <summary>輸出通用信號 R2 的狀態</summary>
            R2 = 34,
            /// <summary>輸出通用信號 R3 的狀態</summary>
            R3 = 35,
            /// <summary>輸出通用信號 R4 的狀態</summary>
            R4 = 36,
            /// <summary>輸出通用信號 R5 的狀態</summary>
            R5 = 37,
            /// <summary>輸出通用信號 R6 的狀態</summary>
            R6 = 38,
            /// <summary>輸出通用信號 R7 的狀態</summary>
            R7 = 39,
            /// <summary>輸出通用信號 R8 的狀態</summary>
            R8 = 40,
            /// <summary>輸出通用信號 R9 的狀態</summary>
            R9 = 41,
            /// <summary>輸出通用信號 R10 的狀態</summary>
            R10 = 42,
            /// <summary>輸出通用信號 R11 的狀態</summary>
            R11 = 43,
            /// <summary>輸出通用信號 R12 的狀態</summary>
            R12 = 44,
            /// <summary>輸出通用信號 R13 的狀態</summary>
            R13 = 45,
            /// <summary>輸出通用信號 R14 的狀態</summary>
            R14 = 46,
            /// <summary>輸出通用信號 R15 的狀態</summary>
            R15 = 47,
            /// <summary>輸出對 M0 輸入的響應(Response)</summary>
            M0_R = 48,
            /// <summary>輸出對 M1 輸入的響應(Response)</summary>
            M1_R = 49,
            /// <summary>輸出對 M2 輸入的響應(Response)</summary>
            M2_R = 50,
            /// <summary>輸出對 M3 輸入的響應(Response)</summary>
            M3_R = 51,
            /// <summary>輸出對 TL 輸入的響應</summary>
            TL_R = 54,
            /// <summary>Alarm 發生時輸出 (常閉)</summary>
            ALARM_OUT_1 = 65,
            /// <summary>Warning 發生時輸出</summary>
            WNG = 66,
            /// <summary>馬達運轉中輸出</summary>
            MOVE = 68,
            /// <summary>馬達轉矩達到轉矩限制值時輸出</summary>
            TLC = 71,
            /// <summary>馬達速度達到設定速度時輸出</summary>
            VA = 77,
            /// <summary>驅動器在內部處理狀態時輸出</summary>
            S_BUSY = 80,
            /// <summary>超過過負載 Warning 等級時輸出。發生過負載 Alarm 時輸出 (常閉)</summary>
            ALARM_OUT_2 = 81,
            /// <summary>輸出主電源的接通狀態</summary>
            MPS = 82,
            /// <summary>輸出馬達軸的旋轉方向</summary>
            DIR = 84,
            /// <summary>馬達軸每旋轉 1 圈輸出 30 脈波</summary>
            SPEED_OUT = 85
        }

        /// <summary>通訊網路輸入(Net-Input)功能參數</summary>
        public enum Param_NET_IN : byte {
            /// <summary>於不使用輸入端子時設定</summary>
            NO_USE = 0,
            /// <summary>馬達朝 正轉(Forward) 方向旋轉  (0)停止 (1)正轉</summary>
            FWD = 1,
            /// <summary>馬達朝 反轉(Reverse) 方向旋轉  (0)停止 (1)反轉</summary>
            REV = 2,
            /// <summary>選擇瞬間停止或減速停止  (0)瞬間停止 (1)減速停止</summary>
            STOP_MODE = 19,
            /// <summary>開放電磁剎車  (0)保持剎車 (1)放開剎車</summary>
            MB_FREE = 20,
            /// <summary>解除 OPX-2A 和 MEXE02 的功能限制 (常閉)  (0)限制功能 (1)解除限制</summary>
            HMI = 27,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用  (0)OFF (1)ON</summary>
            R0 = 32,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用  (0)OFF (1)ON</summary>
            R1 = 33,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用  (0)OFF (1)ON</summary>
            R2 = 34,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用  (0)OFF (1)ON</summary>
            R3 = 35,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用  (0)OFF (1)ON</summary>
            R4 = 36,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用  (0)OFF (1)ON</summary>
            R5 = 37,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用  (0)OFF (1)ON</summary>
            R6 = 38,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用  (0)OFF (1)ON</summary>
            R7 = 39,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用  (0)OFF (1)ON</summary>
            R8 = 40,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用  (0)OFF (1)ON</summary>
            R9 = 41,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用  (0)OFF (1)ON</summary>
            R10 = 42,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用  (0)OFF (1)ON</summary>
            R11 = 43,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用  (0)OFF (1)ON</summary>
            R12 = 44,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用  (0)OFF (1)ON</summary>
            R13 = 45,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用  (0)OFF (1)ON</summary>
            R14 = 46,
            /// <summary>通用信號，以 RS-485 通訊進行控制時使用  (0)OFF (1)ON</summary>
            R15 = 47,
            /// <summary>使用 4 個位元選擇運轉資料 No.</summary>
            M0 = 48,
            /// <summary>使用 4 個位元選擇運轉資料 No.</summary>
            M1 = 49,
            /// <summary>使用 4 個位元選擇運轉資料 No.</summary>
            M2 = 50,
            /// <summary>使用 4 個位元選擇運轉資料 No.</summary>
            M3 = 51,
            /// <summary>使轉矩限制無效 (常閉)  (0)轉矩限制無效 (1)轉矩限制有效</summary>
            TL = 54
        }

        /// <summary>通訊網路輸出(Net-Output)功能參數</summary>
        public enum Param_NET_OUT : byte {
            /// <summary>於不使用輸出端子時設定</summary>
            NO_USE = 0,
            /// <summary>輸出對 FWD 輸入的響應</summary>
            FWD_R = 1,
            /// <summary>輸出對 REV 輸入的響應</summary>
            REV_R = 2,
            /// <summary>輸出對 STOP-MODE 輸入的響應</summary>
            STOP_MODE_R = 19,
            /// <summary>輸出對 MB-FREE 輸入的響應</summary>
            MB_FREE_R = 20,
            /// <summary>輸出對 HMI 輸入的響應</summary>
            HMI_R = 27,
            /// <summary>輸出通用信號 R0 的狀態</summary>
            R0 = 32,
            /// <summary>輸出通用信號 R1 的狀態</summary>
            R1 = 33,
            /// <summary>輸出通用信號 R2 的狀態</summary>
            R2 = 34,
            /// <summary>輸出通用信號 R3 的狀態</summary>
            R3 = 35,
            /// <summary>輸出通用信號 R4 的狀態</summary>
            R4 = 36,
            /// <summary>輸出通用信號 R5 的狀態</summary>
            R5 = 37,
            /// <summary>輸出通用信號 R6 的狀態</summary>
            R6 = 38,
            /// <summary>輸出通用信號 R7 的狀態</summary>
            R7 = 39,
            /// <summary>輸出通用信號 R8 的狀態</summary>
            R8 = 40,
            /// <summary>輸出通用信號 R9 的狀態</summary>
            R9 = 41,
            /// <summary>輸出通用信號 R10 的狀態</summary>
            R10 = 42,
            /// <summary>輸出通用信號 R11 的狀態</summary>
            R11 = 43,
            /// <summary>輸出通用信號 R12 的狀態</summary>
            R12 = 44,
            /// <summary>輸出通用信號 R13 的狀態</summary>
            R13 = 45,
            /// <summary>輸出通用信號 R14 的狀態</summary>
            R14 = 46,
            /// <summary>輸出通用信號 R15 的狀態</summary>
            R15 = 47,
            /// <summary>輸出對 M0 輸入的響應</summary>
            M0_R = 48,
            /// <summary>輸出對 M1 輸入的響應</summary>
            M1_R = 49,
            /// <summary>輸出對 M2 輸入的響應</summary>
            M2_R = 50,
            /// <summary>輸出對 M3 輸入的響應</summary>
            M3_R = 51,
            /// <summary>輸出對 TL 輸入的響應</summary>
            TL_R = 54,
            /// <summary>Alarm 發生時輸出</summary>
            ALARM_OUT_1 = 65,
            /// <summary>Warning 發生時輸出</summary>
            WNG = 66,
            /// <summary>在馬達運轉中輸出</summary>
            MOVE = 68,
            /// <summary>馬達轉矩達到轉矩限制值時輸出</summary>
            TLC = 71,
            /// <summary>馬達速度達到設定速度時輸出</summary>
            VA = 77,
            /// <summary></summary>
            S_BUSY = 80,
            /// <summary></summary>
            ALARM_OUT_2 = 81,
            /// <summary></summary>
            MPS = 82,
            /// <summary></summary>
            DIR = 84,
        }

        #endregion

        #region Declaration - Fields
        /// <summary>ModbusRTU 相關處理</summary>
        private CtModbus_RTU mModbusRTU = new CtModbus_RTU();
        ///// <summary>效能計時用</summary>
        //private Stopwatch mSW = new Stopwatch();

        /// <summary>紀錄局號</summary>
        private byte mDevID = 0;
        #endregion

        #region Declaration - Properties
        /// <summary>取得目前是否已連線至 Oriental Motor</summary>
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
        /// <summary>取得或設定該 Oriental Motor 局號</summary>
        public byte DeviceID { get { return mDevID; } set { mDevID = value; } }
        /// <summary>取得當前電腦所安裝的串列埠名稱</summary>
        public List<string> SerialPorts { get { return CtSerial.GetPortNames(); } }

        #endregion

        #region Declaration - Events

        /// <summary>Oriental Motor 事件</summary>
        public event EventHandler<BleEventArgs> OnOrientalEvents;

        /// <summary>觸發 Oriental Motor 事件</summary>
        /// <param name="events">Oriental Motor 事件</param>
        /// <param name="value">事件所帶的參數</param>
        protected virtual void RaiseEvents(BleEvents events, object value) {
            EventHandler<BleEventArgs> handler = OnOrientalEvents;
            if (handler != null) handler(this, new BleEventArgs(events, value));
        }
        #endregion

        #region Function - Constructors
        /// <summary>建構 Oriental BLE FLEX 通訊控制</summary>
        public CtOriental_BLE() {
            mModbusRTU.OnSerialEvents += mModbusRTU_OnSerialEvents;
        }

        /// <summary>建構 Oriental BLE FLEX 通訊控制，以已建立的 <see cref="CtModbus_RTU"/> 為主</summary>
        /// <param name="rtu"></param>
        public CtOriental_BLE(CtModbus_RTU rtu) {
            mModbusRTU = rtu;
            mModbusRTU.OnSerialEvents += mModbusRTU_OnSerialEvents;
        }

        /// <summary>處理 Modbus RTU 事件</summary>
        private void mModbusRTU_OnSerialEvents(object sender, SerialPort.SerialEventArgs e) {
            switch (e.Event) {
                case SerialPortEvents.Connection:
                    CtThread.AddWorkItem(obj => RaiseEvents(BleEvents.Connection, CtConvert.CBool(e.Value)));
                    break;
                case SerialPortEvents.DataReceived:
                    //ModbusRTU 裡面已經覆寫此事件，基本上是不會發出任何東西...
                    break;
                case SerialPortEvents.Error:
                    if (e.Value is string)
                        CtThread.AddWorkItem(obj => RaiseEvents(BleEvents.CommunicationError, e.Value as string));
                    else
                        CtThread.AddWorkItem(obj => RaiseEvents(BleEvents.DeviceError, e.Value as List<byte>));
                    break;
            }
        }
        #endregion

        #region Function - Disposable
        /// <summary>中斷與 Oriental BLE FLEX 之連線，並釋放相關資源</summary>
        public void Dispose() {
            try {
                Dispose(true);
                GC.SuppressFinalize(this);
            } catch (ObjectDisposedException ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
        }

        /// <summary>中斷與 Oriental BLE FLEX 之連線，並釋放相關資源</summary>
        /// <param name="isDisposing">是否為第一次釋放</param>
        protected virtual void Dispose(bool isDisposing) {
            try {
                if (isDisposing) {
                    if (mModbusRTU != null) {
                        mModbusRTU.Close();
                        mModbusRTU.OnSerialEvents -= mModbusRTU_OnSerialEvents;
                    }
                }
            } catch (Exception ex) {
                CtStatus.Report(Stat.ER_SYSTEM, ex);
            }
        }
        #endregion

        #region Function - Connections

        /// <summary>嘗試連接至 Oriental Motor</summary>
        /// <returns>Status Code</returns>
        public Stat Connect() {
            Stat stt = Stat.SUCCESS;
            try {
                mModbusRTU.Open();
            } catch (Exception ex) {
                stt = Stat.ER_COM_OPEN;
                RaiseEvents(BleEvents.CommunicationError, ex.Message);
            }
            return stt;
        }

        /// <summary>嘗試連接至 Oriental Motor，並指定其串列埠編號</summary>
        /// <param name="comPort">串列埠編號，如 "COM3"</param>
        /// <returns>Status Code</returns>
        public Stat Connect(string comPort) {
            Stat stt = Stat.SUCCESS;
            try {
                mModbusRTU.Open(comPort, BaudRate, DataBits, StopBits, Parity, HandShake);
            } catch (Exception ex) {
                stt = Stat.ER_COM_OPEN;
                RaiseEvents(BleEvents.CommunicationError, ex.Message);
            }
            return stt;
        }

        /// <summary>嘗試連接至 Oriental Motor，使用預設局號並設定串列埠屬性</summary>
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
                RaiseEvents(BleEvents.CommunicationError, ex.Message);
            }
            return stt;
        }

        /// <summary>嘗試連接至 Oriental Motor，使用指定的局號與串列埠編號</summary>
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
                RaiseEvents(BleEvents.CommunicationError, ex.Message);
            }
            return stt;
        }

        /// <summary>嘗試連接至 Oriental Motor，使用指定的局號與串列埠屬性</summary>
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
                RaiseEvents(BleEvents.CommunicationError, ex.Message);
            }
            return stt;
        }

        /// <summary>中斷與 Oriental Motor 之連線</summary>
        /// <returns>Status Code</returns>
        public Stat Disconnect() {
            Stat stt = Stat.SUCCESS;
            try {
                mModbusRTU.Close();
            } catch (Exception ex) {
                stt = Stat.ER_COM_OPEN;
                RaiseEvents(BleEvents.CommunicationError, ex.Message);
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
                    RaiseEvents(BleEvents.CommunicationError, "Communicate successfull, but received unrecognized data");
                }
            } else if (stt == Stat.ER3_MB_SLVERR) {
                if (value != null) RaiseEvents(BleEvents.DeviceError, value.ToList());
                else {
                    retStt = Stat.ER3_MB_UNDATA;
                    RaiseEvents(BleEvents.CommunicationError, "Error responded from slave device, but null argument");
                }
            } else if (stt == Stat.ER3_MB_COMTMO) {
                RaiseEvents(BleEvents.CommunicationError, "Waiting response timeout");
            } else {
                RaiseEvents(BleEvents.CommunicationError, "Received unrecognized data");
            }
            return retStt;
        }

        /// <summary>[FC03] 讀取暫存器(Register)數值，可為輸入或輸出之暫存器(Input, Output, InOut)</summary>
        /// <param name="addr">Register Address</param>
        /// <param name="regCount">連續讀取的暫存器數量。如 4000~4007 則輸入 8</param>
        /// <param name="value">(<see langword="true"/>)ON  (<see langword="false"/>)OFF</param>
        /// <returns>Status Code</returns>
        /// <remarks>以 Wago IO 為例，一個 Register 都是 16bit，所以回傳的資訊以 ushort (uint16) 為主</remarks>
        private Stat ReadHoldingRegister(ushort addr, ushort regCount, out List<byte> value) {
            //mSW.Restart();
            Stat stt = mModbusRTU.ReadHoldingRegister(mDevID, addr, regCount, out value);
            //mSW.Stop();
            //Console.WriteLine("ReadHoldingRegister >> " + mSW.ElapsedMilliseconds.ToString() + " ms");
            return CheckReturnValue(stt, value);
        }

        /// <summary>[FC06] 設定單一 Register (通常為 16bit)</summary>
        /// <param name="addr">Register 位址。 如 0x04D3</param>
        /// <param name="value">欲寫入的數值</param>
        /// <param name="result">裝置端回傳的訊息。如沒有回應則回傳 null</param>
        /// <returns>Status Code</returns>
        private Stat WriteSingleRegister(ushort addr, ushort value, out List<byte> result) {
            //mSW.Restart();
            Stat stt = mModbusRTU.WriteSingleRegister(mDevID, addr, value, out result);
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
            Stat stt = mModbusRTU.WriteMultiRegisters(mDevID, addr, value, out result);
            //mSW.Stop();
            //Console.WriteLine("ReadHoldingRegister >> " + mSW.ElapsedMilliseconds.ToString() + " ms");
            return CheckReturnValue(stt, result);
        }

        #endregion

        #region Function - Core

        #region Settings
        /// <summary>設定吋動(Jog)時的速度</summary>
        /// <param name="rpm">轉速</param>
        /// <returns>Status Code</returns>
        /// <remarks>2015/08/31 測試 BLE FLEX 時，尚未找出此數值對應的動作</remarks>
        public Stat SetJogVelocity(ushort rpm) {
            List<byte> result;
            return WriteSingleRegister(0x0287, rpm, out result);
        }

        /// <summary>設定類比輸入致能模式</summary>
        /// <param name="aiMode">欲更改的模式</param>
        /// <returns>Status Code</returns>
        public Stat SetAnalogyInputMode(AnalogyInputMode aiMode) {
            List<byte> result;
            Stat stt = WriteSingleRegister(0x10E3, (ushort)aiMode, out result);
            if (stt == Stat.SUCCESS) stt = Configuration();
            return stt;
        }

        /// <summary>設定單組轉速，編號 0 ~ 15 (共 16 組)</summary>
        /// <param name="index">欲更改的轉速編號。 #0 ~ #15</param>
        /// <param name="rpm">欲更改的轉速值，0、80 ~ 4000</param>
        /// <returns>Status Code</returns>
        public Stat SetVelocity(byte index, ushort rpm) {
            List<byte> result;
            return WriteSingleRegister((ushort)(0x0481 + (index * 2)), rpm, out result);
        }

        /// <summary>設定連續多組轉速，編號 0 ~ 15 (共 16 組)</summary>
        /// <param name="startIndex">欲連續寫入的第一組編號值。如寫入 3 ~ 8 組轉速，則此處帶入 3</param>
        /// <param name="value">連續性數值，各轉速可輸入範圍為 0 、 80 ~ 4000</param>
        /// <returns>Status Code</returns>
        public Stat SetVelocity(byte startIndex, List<ushort> value) {
            List<byte> result;

            byte countIdx = 0;
            Dictionary<byte, List<ushort>> velData = new Dictionary<byte, List<ushort>>();

            /*-- 使用 for-loop 約耗 9400 CPU Ticks --*/
            //ushort[] zeroValue = new ushort[] { 0 };
            //for (int valIdx = 0; valIdx < value.Count; valIdx++) {
            //    if (valIdx % 4 == 0) velData.Add(++countIdx, new List<ushort>());

            //    velData[countIdx].AddRange(zeroValue);
            //    velData[countIdx].Add(value[valIdx]);
            //}

            /*-- 使用 List 約耗 8500 CPU Ticks --*/
            int count = 0;
            List<ushort> data = new List<ushort>();
            value.ForEach(val => { data.Add(0); data.Add(val); });

            for (int idx = 0; idx < data.Count; idx += 8) {
                count = (idx + 8) > data.Count ? data.Count - idx : 8;
                velData.Add(countIdx++, data.GetRange(idx, count));
            }

            ushort addr = (ushort)(0x0480 + (startIndex * 2));

            Stat stt = Stat.SUCCESS;

            foreach (KeyValuePair<byte, List<ushort>> item in velData) {
                stt = WriteMultiRegisters(addr, item.Value, out result);
                if (stt != Stat.SUCCESS) break;
                addr += 8;
            }

            return stt;
        }

        /// <summary>設定單組加速時間，編號 0 ~ 15 (共 16 組)</summary>
        /// <param name="index">欲更改的加速時間。 #0 ~ #15</param>
        /// <param name="time">欲更改的加速時間，單位為 0.1sec、範圍 2 ~ 150。 如 200 為 20s</param>
        /// <returns>Status Code</returns>
        public Stat SetAccel(byte index, ushort time) {
            List<byte> result;
            return WriteSingleRegister((ushort)(0x0601 + (index * 2)), time, out result);
        }

        /// <summary>設定連續多組加速時間，編號 0 ~ 15 (共 16 組)</summary>
        /// <param name="startIndex">欲連續寫入的第一組編號值。如寫入 3 ~ 8 組加速時間，則此處帶入 3</param>
        /// <param name="value">連續性數值，各時間單位為 0.1sec、範圍 2 ~ 150。 如 200 為 20s</param>
        /// <returns>Status Code</returns>
        public Stat SetAccel(byte startIndex, List<ushort> value) {
            List<byte> result;

            byte countIdx = 0;
            Dictionary<byte, List<ushort>> velData = new Dictionary<byte, List<ushort>>();

            int count = 0;
            List<ushort> data = new List<ushort>();
            value.ForEach(val => { data.Add(0); data.Add(val); });

            for (int idx = 0; idx < data.Count; idx += 8) {
                count = (idx + 8) > data.Count ? data.Count - idx : 8;
                velData.Add(countIdx++, data.GetRange(idx, count));
            }

            ushort addr = (ushort)(0x0600 + (startIndex * 2));

            Stat stt = Stat.SUCCESS;

            foreach (KeyValuePair<byte, List<ushort>> item in velData) {
                stt = WriteMultiRegisters(addr, item.Value, out result);
                if (stt != Stat.SUCCESS) break;
                addr += 8;
            }

            return stt;
        }

        /// <summary>設定單組減速時間，編號 0 ~ 15 (共 16 組)</summary>
        /// <param name="index">欲更改的減速時間。 #0 ~ #15</param>
        /// <param name="time">欲更改的減速時間，單位為 0.1sec、範圍 2 ~ 150。 如 200 為 20s</param>
        /// <returns>Status Code</returns>
        public Stat SetDecel(byte index, ushort time) {
            List<byte> result;
            return WriteSingleRegister((ushort)(0x0681 + (index * 2)), time, out result);
        }

        /// <summary>設定連續多組減速時間，編號 0 ~ 15 (共 16 組)</summary>
        /// <param name="startIndex">欲連續寫入的第一組編號值。如寫入 3 ~ 8 組減速時間，則此處帶入 3</param>
        /// <param name="value">連續性數值，各時間單位為 0.1sec、範圍 2 ~ 150。 如 200 為 20s</param>
        /// <returns>Status Code</returns>
        public Stat SetDecel(byte startIndex, List<ushort> value) {
            List<byte> result;

            byte countIdx = 0;
            Dictionary<byte, List<ushort>> velData = new Dictionary<byte, List<ushort>>();

            int count = 0;
            List<ushort> data = new List<ushort>();
            value.ForEach(val => { data.Add(0); data.Add(val); });

            for (int idx = 0; idx < data.Count; idx += 8) {
                count = (idx + 8) > data.Count ? data.Count - idx : 8;
                velData.Add(countIdx++, data.GetRange(idx, count));
            }

            ushort addr = (ushort)(0x0680 + (startIndex * 2));

            Stat stt = Stat.SUCCESS;

            foreach (KeyValuePair<byte, List<ushort>> item in velData) {
                stt = WriteMultiRegisters(addr, item.Value, out result);
                if (stt != Stat.SUCCESS) break;
                addr += 8;
            }

            return stt;
        }

        /// <summary>設定單組轉矩限制，編號 0 ~ 15 (共 16 組)</summary>
        /// <param name="index">欲更改的限制轉矩編號。 #0 ~ #15</param>
        /// <param name="torq">欲限制的轉矩值，單位為 % 、 範圍 0 ~ 200%。 如 180 為 180%</param>
        /// <returns>Status Code</returns>
        public Stat SetTorqueLimit(byte index, ushort torq) {
            List<byte> result;
            return WriteSingleRegister((ushort)(0x0701 + (index * 2)), torq, out result);
        }

        /// <summary>設定連續多組轉矩限制，編號 0 ~ 15 (共 16 組)</summary>
        /// <param name="startIndex">欲連續寫入的第一組編號值。如寫入 3 ~ 8 組限制轉矩，則此處帶入 3</param>
        /// <param name="value">連續性數值，各轉矩單位為 % 、 範圍 0 ~ 200%。 如 180 為 180%</param>
        /// <returns>Status Code</returns>
        public Stat SetTorqueLimit(byte startIndex, List<ushort> value) {
            List<byte> result;

            byte countIdx = 0;
            Dictionary<byte, List<ushort>> velData = new Dictionary<byte, List<ushort>>();

            int count = 0;
            List<ushort> data = new List<ushort>();
            value.ForEach(val => { data.Add(0); data.Add(val); });

            for (int idx = 0; idx < data.Count; idx += 8) {
                count = (idx + 8) > data.Count ? data.Count - idx : 8;
                velData.Add(countIdx++, data.GetRange(idx, count));
            }

            ushort addr = (ushort)(0x0700 + (startIndex * 2));

            Stat stt = Stat.SUCCESS;

            foreach (KeyValuePair<byte, List<ushort>> item in velData) {
                stt = WriteMultiRegisters(addr, item.Value, out result);
                if (stt != Stat.SUCCESS) break;
                addr += 8;
            }

            return stt;
        }

        /// <summary>設定停止模式，立即停止或減速停止</summary>
        /// <param name="delayStop">(<see langword="false"/>)立即停止  (<see langword="true"/>)減速停止</param>
        /// <param name="bit">於 NET-INPUT 中的編號，預設為 5</param>
        /// <returns>Status Code</returns>
        public Stat SetStopMode(bool delayStop, Func_NET_IN bit = Func_NET_IN.NET_IN_5) {
            int value;
            List<byte> result;
            Stat stt = ReadHoldingRegister(0x007D, 1, out result);

            if (stt == Stat.SUCCESS) {
                value = (ushort)((result[0] << 8) + result[1]);
                if (delayStop) value |= 0x01 << (byte)bit;
                else value &= (0x01 << (byte)bit) ^ 0xFFFF;

                stt = WriteSingleRegister(0x007D, (ushort)value, out result);
            }

            return stt;
        }

        /// <summary>設定輸入(Input)對應功能</summary>
        /// <param name="func">功能編號，從 0 ~ 6</param>
        /// <param name="param">欲設定的功能參數</param>
        /// <returns>Status Code</returns>
        public Stat SetFuncMap_IN(Func_IN func, Param_IN param) {
            List<byte> result;
            return WriteSingleRegister((ushort)(0x1101 + (byte)func * 2), (ushort)param, out result);
        }

        /// <summary>設定輸出(Output)對應功能</summary>
        /// <param name="func">功能編號，0、1</param>
        /// <param name="param">欲設定的功能參數</param>
        /// <returns>Status Code</returns>
        public Stat SetFuncMap_OUT(Func_OUT func, Param_OUT param) {
            List<byte> result;
            return WriteSingleRegister((ushort)(0x1141 + (byte)func * 2), (ushort)param, out result);
        }

        /// <summary>設定網路通訊輸入(Net-Input)對應功能</summary>
        /// <param name="func">功能編號，從 0 ~ 15</param>
        /// <param name="param">欲設定的功能參數</param>
        /// <returns>Status Code</returns>
        public Stat SetFuncMap_NET_IN(Func_NET_IN func, Param_NET_IN param) {
            List<byte> result;
            Stat stt = WriteSingleRegister((ushort)(0x1161 + (byte)func * 2), (ushort)param, out result);
            if (stt == Stat.SUCCESS) stt = Configuration();
            return stt;
        }

        /// <summary>設定網路通訊輸出(Net-Output)對應功能</summary>
        /// <param name="func">功能編號，從 0 ~ 15</param>
        /// <param name="param">欲設定的功能參數</param>
        /// <returns>Status Code</returns>
        public Stat SetFuncMap_NET_OUT(Func_NET_OUT func, Param_NET_OUT param) {
            List<byte> result;
            Stat stt = WriteSingleRegister((ushort)(0x1181 + (byte)func * 2), (ushort)param, out result);
            if (stt == Stat.SUCCESS) stt = Configuration();
            return stt;
        }

        #endregion

        #region State

        /// <summary>取得當前控制器錯誤代碼</summary>
        /// <returns>(Null)讀取失敗 (HasValue)當前數值</returns>
        public int? GetAlarmCode() {
            int? value = null;

            List<byte> result;
            Stat stt = ReadHoldingRegister(0x0080, 2, out result);

            if (stt == Stat.SUCCESS)
                value = (result[0] << 24) + (result[1] << 16) + (result[2] << 8) + result[3];

            return value;
        }

        /// <summary>取得當前控制器警告代碼</summary>
        /// <returns>(Null)讀取失敗 (HasValue)當前數值</returns>
        public int? GetWarningCode() {
            int? value = null;

            List<byte> result;
            Stat stt = ReadHoldingRegister(0x0096, 2, out result);

            if (stt == Stat.SUCCESS)
                value = (result[0] << 24) + (result[1] << 16) + (result[2] << 8) + result[3];

            return value;
        }

        /// <summary>取得當前通訊錯誤代碼</summary>
        /// <returns>(Null)讀取失敗 (HasValue)當前數值</returns>
        public int? GetCommunicateErrorCode() {
            int? value = null;

            List<byte> result;
            Stat stt = ReadHoldingRegister(0x00AC, 2, out result);

            if (stt == Stat.SUCCESS)
                value = (result[0] << 24) + (result[1] << 16) + (result[2] << 8) + result[3];

            return value;
        }

        /// <summary>取得當前馬達回授速度</summary>
        /// <returns>(Null)讀取失敗 (HasValue)當前數值</returns>
        public double? GetMotorVelocity() {
            double? value = null;

            /*-- 設定轉速 --*/
            List<byte> result;
            Stat stt = ReadHoldingRegister(0x0102, 2, out result);
            if (stt != Stat.SUCCESS) return value;

            int point = (result[0] << 24) + (result[1] << 16) + (result[2] << 8) + result[3];
            if (3 < point || point < 1) point = 1;
            else if (point == 1) point = 10;
            else if (point == 2) point = 100;
            else if (point == 3) point = 1000;

            stt = ReadHoldingRegister(0x0100, 2, out result);

            if (stt == Stat.SUCCESS) {
                int speed = (result[0] << 24) + (result[1] << 16) + (result[2] << 8) + result[3];
                value = speed / (double)point;
            }

            return value;
        }

        /// <summary>取得當前皮帶傳送速度</summary>
        /// <returns>(Null)讀取失敗 (HasValue)當前數值</returns>
        public double? GetBeltVelocity() {
            double? value = null;

            /*-- 設定轉速 --*/
            List<byte> result;
            Stat stt = ReadHoldingRegister(0x0106, 2, out result);
            if (stt != Stat.SUCCESS) return value;

            int point = (result[0] << 24) + (result[1] << 16) + (result[2] << 8) + result[3];
            if (3 < point || point < 1) point = 1;
            else if (point == 1) point = 10;
            else if (point == 2) point = 100;
            else if (point == 3) point = 1000;

            stt = ReadHoldingRegister(0x0104, 2, out result);

            if (stt == Stat.SUCCESS) {
                int speed = (result[0] << 24) + (result[1] << 16) + (result[2] << 8) + result[3];
                value = speed / (double)point;
            }

            return value;
        }

        /// <summary>取得當前附載率</summary>
        /// <returns>(Null)讀取失敗 (HasValue)當前數值</returns>
        public int? GetLoad() {
            int? value = null;

            List<byte> result;
            Stat stt = ReadHoldingRegister(0x0108, 2, out result);

            if (stt == Stat.SUCCESS)
                value = (result[0] << 24) + (result[1] << 16) + (result[2] << 8) + result[3];

            return value;
        }

        /// <summary>取得類比輸入的速度</summary>
        /// <returns>(Null)讀取失敗 (HasValue)當前數值</returns>
        public int? GetAnalogyVelocity() {
            int? value = null;

            List<byte> result;
            Stat stt = ReadHoldingRegister(0x010C, 2, out result);
            if (stt == Stat.SUCCESS) {
                value = (result[0] << 24) + (result[1] << 16) + (result[2] << 8) + result[3];
                if (value == 0xFFFF) value = -1;    //0xFFFF 表示未啟用此功能
            }

            return value;
        }

        /// <summary>取得類比輸入的限制轉矩</summary>
        /// <returns>(Null)讀取失敗 (HasValue)當前數值</returns>
        public int? GetAnalogyTorqueLimit() {
            int? value = null;

            List<byte> result;
            Stat stt = ReadHoldingRegister(0x0110, 2, out result);

            if (stt == Stat.SUCCESS) {
                value = (result[0] << 24) + (result[1] << 16) + (result[2] << 8) + result[3];
                if (value == 0xFFFF) value = -1;    //0xFFFF 表示未啟用此功能
            }

            return value;
        }

        /// <summary>取得類比輸入電壓</summary>
        /// <returns>(Null)讀取失敗 (HasValue)當前數值</returns>
        public int? GetAnalogyVoltage() {
            int? value = null;

            List<byte> result;
            Stat stt = ReadHoldingRegister(0x0116, 2, out result);

            if (stt == Stat.SUCCESS) {
                value = (result[0] << 24) + (result[1] << 16) + (result[2] << 8) + result[3];
                if (value == 0xFFFF) value = -1;    //0xFFFF 表示未啟用此功能
            }

            return value;
        }

        /// <summary>取得吋動(Jog)速度</summary>
        /// <returns>(Null)讀取失敗 (HasValue)當前數值</returns>
        public int? GetJogVelocity() {
            int? value = null;

            List<byte> result;
            Stat stt = ReadHoldingRegister(0x0286, 4, out result);

            if (stt == Stat.SUCCESS) {
                value = (result[0] << 24) + (result[1] << 16) + (result[2] << 8) + result[3];
                if (value == 0xFFFF) value = -1;    //0xFFFF 表示未啟用此功能
            }

            return value;
        }

        /// <summary>取得當前實際 I/O 輸出入狀態</summary>
        /// <returns>讀取的 I/O 狀態</returns>
        public IOState GetIOState() {
            IOState ioStt = null;

            List<byte> result;
            Stat stt = ReadHoldingRegister(0x00D4, 2, out result);

            if (stt == Stat.SUCCESS) ioStt = new IOState(result);

            return ioStt;
        }

        /// <summary>取得當前類比輸入致能狀態</summary>
        /// <returns>(Null)讀取失敗 (HasValue)當前數值</returns>
        public AnalogyInputMode? GetAnalogyInputMode() {
            AnalogyInputMode? value = null;

            List<byte> result;
            Stat stt = ReadHoldingRegister(0x10E2, 2, out result);

            if (stt == Stat.SUCCESS)
                value = (AnalogyInputMode)result[3];

            return value;
        }

        /// <summary>取得特定編號的轉速設定值</summary>
        /// <param name="index">欲讀取的轉速編號</param>
        /// <returns>(Null)讀取失敗 (HasValue)當前數值</returns>
        public int? GetVelocity(byte index) {
            int? value = null;

            List<byte> result;
            ushort addr = (ushort)(0x0480 + index * 2);
            Stat stt = ReadHoldingRegister(addr, 2, out result);

            if (stt == Stat.SUCCESS) {
                value = (result[0] << 24) + (result[1] << 16) + (result[2] << 8) + result[3];
                if (value == 0xFFFF) value = -1;    //0xFFFF 表示未啟用此功能
            }

            return value;
        }

        /// <summary>取得連續編號的轉速設定值</summary>
        /// <param name="startIndex">起始編號值。如欲讀取 3 ~ 8，則此處帶入 3</param>
        /// <param name="count">欲讀取的數量。如欲讀取 3 ~ 8，則此處帶入 6</param>
        /// <returns>已讀取的數值</returns>
        public List<int> GetVelocity(byte startIndex, byte count) {
            List<int> value = null;

            int tempValue = 0;
            ushort data1stCount = (ushort)(count * 2);
            ushort dataLeftCount = 0;

            if (data1stCount > 16) {
                dataLeftCount = (ushort)(data1stCount - 16);
                data1stCount = 16;
            }

            List<byte> result;
            ushort addr = (ushort)(0x0480 + startIndex * 2);
            Stat stt = ReadHoldingRegister(addr, data1stCount, out result);

            if (stt == Stat.SUCCESS) {
                value = new List<int>();
                for (int idx = 0; idx < result.Count; idx += 4) {
                    tempValue = (result[idx] << 24) + (result[idx + 1] << 16) + (result[idx + 2] << 8) + result[idx + 3];
                    if (tempValue == 0xFFFF) tempValue = -1;    //0xFFFF 表示未啟用此功能
                    value.Add(tempValue);
                }
            } else return value;

            if (dataLeftCount > 0) {
                stt = ReadHoldingRegister((ushort)(addr + 16), dataLeftCount, out result);

                if (stt == Stat.SUCCESS) {
                    for (int idx = 0; idx < result.Count; idx += 4) {
                        tempValue = (result[idx] << 24) + (result[idx + 1] << 16) + (result[idx + 2] << 8) + result[idx + 3];
                        if (tempValue == 0xFFFF) tempValue = -1;    //0xFFFF 表示未啟用此功能
                        value.Add((int)tempValue);
                    }
                }
            }

            return value;
        }

        /// <summary>取得特定編號的加速時間設定值</summary>
        /// <param name="index">欲讀取的加速時間編號</param>
        /// <returns>(Null)讀取失敗 (HasValue)當前數值</returns>
        public int? GetAccel(byte index) {
            int? value = null;

            List<byte> result;
            ushort addr = (ushort)(0x0600 + index * 2);
            Stat stt = ReadHoldingRegister(addr, 2, out result);

            if (stt == Stat.SUCCESS) {
                value = (result[0] << 24) + (result[1] << 16) + (result[2] << 8) + result[3];
                if (value == 0xFFFF) value = -1;    //0xFFFF 表示未啟用此功能
            }

            return value;
        }

        /// <summary>取得連續編號的加速時間設定值</summary>
        /// <param name="startIndex">起始編號值。如欲讀取 3 ~ 8，則此處帶入 3</param>
        /// <param name="count">欲讀取的數量。如欲讀取 3 ~ 8，則此處帶入 6</param>
        /// <returns>已讀取的數值</returns>
        public List<int> GetAccel(byte startIndex, byte count) {
            List<int> value = null;

            int tempValue = 0;
            ushort data1stCount = (ushort)(count * 2);
            ushort dataLeftCount = 0;

            if (data1stCount > 16) {
                dataLeftCount = (ushort)(data1stCount - 16);
                data1stCount = 16;
            }

            List<byte> result;
            ushort addr = (ushort)(0x0600 + startIndex * 2);
            Stat stt = ReadHoldingRegister(addr, data1stCount, out result);

            if (stt == Stat.SUCCESS) {
                value = new List<int>();
                for (int idx = 0; idx < result.Count; idx += 4) {
                    tempValue = (result[idx] << 24) + (result[idx + 1] << 16) + (result[idx + 2] << 8) + result[idx + 3];
                    if (tempValue == 0xFFFF) tempValue = -1;    //0xFFFF 表示未啟用此功能
                    value.Add((int)tempValue);
                }
            } else return value;

            if (dataLeftCount > 0) {
                stt = ReadHoldingRegister((ushort)(addr + 16), dataLeftCount, out result);

                if (stt == Stat.SUCCESS) {
                    for (int idx = 0; idx < result.Count; idx += 4) {
                        tempValue = (result[idx] << 24) + (result[idx + 1] << 16) + (result[idx + 2] << 8) + result[idx + 3];
                        if (tempValue == 0xFFFF) tempValue = -1;    //0xFFFF 表示未啟用此功能
                        value.Add((int)tempValue);
                    }
                }
            }

            return value;
        }

        /// <summary>取得特定編號的減速時間設定值</summary>
        /// <param name="index">欲讀取的減速時間編號</param>
        /// <returns>(Null)讀取失敗 (HasValue)當前數值</returns>
        public int? GetDecel(byte index) {
            int? value = null;

            List<byte> result;
            ushort addr = (ushort)(0x0680 + index * 2);
            Stat stt = ReadHoldingRegister(addr, 2, out result);

            if (stt == Stat.SUCCESS) {
                value = (result[0] << 24) + (result[1] << 16) + (result[2] << 8) + result[3];
                if (value == 0xFFFF) value = -1;    //0xFFFF 表示未啟用此功能
            }

            return value;
        }

        /// <summary>取得連續編號的減速時間設定值</summary>
        /// <param name="startIndex">起始編號值。如欲讀取 3 ~ 8，則此處帶入 3</param>
        /// <param name="count">欲讀取的數量。如欲讀取 3 ~ 8，則此處帶入 6</param>
        /// <returns>已讀取的數值</returns>
        public List<int> GetDecel(byte startIndex, byte count) {
            List<int> value = null;

            int tempValue = 0;
            ushort data1stCount = (ushort)(count * 2);
            ushort dataLeftCount = 0;

            if (data1stCount > 16) {
                dataLeftCount = (ushort)(data1stCount - 16);
                data1stCount = 16;
            }

            List<byte> result;
            ushort addr = (ushort)(0x0680 + startIndex * 2);
            Stat stt = ReadHoldingRegister(addr, data1stCount, out result);

            if (stt == Stat.SUCCESS) {
                value = new List<int>();
                for (int idx = 0; idx < result.Count; idx += 4) {
                    tempValue = (result[idx] << 24) + (result[idx + 1] << 16) + (result[idx + 2] << 8) + result[idx + 3];
                    if (tempValue == 0xFFFF) tempValue = -1;    //0xFFFF 表示未啟用此功能
                    value.Add((int)tempValue);
                }
            } else return value;

            if (dataLeftCount > 0) {
                stt = ReadHoldingRegister((ushort)(addr + 16), dataLeftCount, out result);

                if (stt == Stat.SUCCESS) {
                    for (int idx = 0; idx < result.Count; idx += 4) {
                        tempValue = (result[idx] << 24) + (result[idx + 1] << 16) + (result[idx + 2] << 8) + result[idx + 3];
                        if (tempValue == 0xFFFF) tempValue = -1;    //0xFFFF 表示未啟用此功能
                        value.Add((int)tempValue);
                    }
                }
            }

            return value;
        }

        /// <summary>取得特定編號的轉矩限制設定值</summary>
        /// <param name="index">欲讀取的限制轉矩編號</param>
        /// <returns>(Null)讀取失敗 (HasValue)當前數值</returns>
        public int? GetTorqueLimit(byte index) {
            int? value = null;

            List<byte> result;
            ushort addr = (ushort)(0x0700 + index * 2);
            Stat stt = ReadHoldingRegister(addr, 2, out result);

            if (stt == Stat.SUCCESS) {
                value = (result[0] << 24) + (result[1] << 16) + (result[2] << 8) + result[3];
                if (value == 0xFFFF) value = -1;    //0xFFFF 表示未啟用此功能
            }

            return value;
        }

        /// <summary>取得連續編號的轉矩限制設定值</summary>
        /// <param name="startIndex">起始編號值。如欲讀取 3 ~ 8，則此處帶入 3</param>
        /// <param name="count">欲讀取的數量。如欲讀取 3 ~ 8，則此處帶入 6</param>
        /// <returns>已讀取的數值</returns>
        public List<int> GetTorqueLimit(byte startIndex, byte count) {
            List<int> value = null;

            int tempValue = 0;
            ushort data1stCount = (ushort)(count * 2);
            ushort dataLeftCount = 0;

            if (data1stCount > 16) {
                dataLeftCount = (ushort)(data1stCount - 16);
                data1stCount = 16;
            }

            List<byte> result;
            ushort addr = (ushort)(0x0700 + startIndex * 2);
            Stat stt = ReadHoldingRegister(addr, data1stCount, out result);

            if (stt == Stat.SUCCESS) {
                value = new List<int>();
                for (int idx = 0; idx < result.Count; idx += 4) {
                    tempValue = (result[idx] << 24) + (result[idx + 1] << 16) + (result[idx + 2] << 8) + result[idx + 3];
                    if (tempValue == 0xFFFF) tempValue = -1;    //0xFFFF 表示未啟用此功能
                    value.Add((int)tempValue);
                }
            } else return value;

            if (dataLeftCount > 0) {
                stt = ReadHoldingRegister((ushort)(addr + 16), dataLeftCount, out result);

                if (stt == Stat.SUCCESS) {
                    for (int idx = 0; idx < result.Count; idx += 4) {
                        tempValue = (result[idx] << 24) + (result[idx + 1] << 16) + (result[idx + 2] << 8) + result[idx + 3];
                        if (tempValue == 0xFFFF) tempValue = -1;    //0xFFFF 表示未啟用此功能
                        value.Add((int)tempValue);
                    }
                }
            }

            return value;
        }

        /// <summary>取得當前輸出(Output)功能及其對應參數</summary>
        /// <returns>對應參數</returns>
        public Dictionary<Func_OUT, Param_OUT> GetFuncMap_Out() {
            Dictionary<Func_OUT, Param_OUT> mapping = null;

            List<byte> result;
            Stat stt = ReadHoldingRegister(0x1140, 4, out result);

            if (stt == Stat.SUCCESS) {
                int tempVal = 0;
                mapping = new Dictionary<Func_OUT, Param_OUT>();
                foreach (byte item in Enum.GetValues(typeof(Func_OUT))) {
                    tempVal = (result[item * 4 + 2] << 8) + result[item * 4 + 3];
                    mapping.Add((Func_OUT)item, (Param_OUT)tempVal);
                }
            }

            return mapping;
        }

        /// <summary>取得當前輸入(Input)功能及其對應參數</summary>
        /// <returns>對應參數</returns>
        public Dictionary<Func_IN, Param_IN> GetFuncMap_In() {
            Dictionary<Func_IN, Param_IN> mapping = null;

            List<byte> result;
            Stat stt = ReadHoldingRegister(0x1100, 14, out result);

            if (stt == Stat.SUCCESS) {
                int tempVal = 0;
                mapping = new Dictionary<Func_IN, Param_IN>();
                foreach (byte item in Enum.GetValues(typeof(Func_IN))) {
                    tempVal = (result[item * 4 + 2] << 8) + result[item * 4 + 3];
                    mapping.Add((Func_IN)item, (Param_IN)tempVal);
                }
            }

            return mapping;
        }

        /// <summary>取得當前網路通訊輸出(Net-Output)功能及其對應參數</summary>
        /// <returns>對應參數</returns>
        public Dictionary<Func_NET_OUT, Param_NET_OUT> GetFuncMap_NET_OUT() {
            Dictionary<Func_NET_OUT, Param_NET_OUT> mapping = null;

            List<byte> result;
            List<byte> remainData = new List<byte>();
            Stat stt = ReadHoldingRegister(0x1180, 16, out result);

            if (stt != Stat.SUCCESS) return mapping;

            remainData.AddRange(result);

            stt = ReadHoldingRegister(0x1190, 16, out result);

            if (stt == Stat.SUCCESS) {
                remainData.AddRange(result);

                int tempVal = 0;
                mapping = new Dictionary<Func_NET_OUT, Param_NET_OUT>();
                foreach (byte item in Enum.GetValues(typeof(Func_NET_OUT))) {
                    tempVal = (remainData[item * 4 + 2] << 8) + remainData[item * 4 + 3];
                    mapping.Add((Func_NET_OUT)item, (Param_NET_OUT)tempVal);
                }
            }

            return mapping;
        }

        /// <summary>取得當前網路通訊輸入(Net-Input)功能及其對應參數</summary>
        /// <returns>對應參數</returns>
        public Dictionary<Func_NET_IN, Param_NET_IN> GetFuncMap_NET_IN() {
            Dictionary<Func_NET_IN, Param_NET_IN> mapping = null;

            List<byte> result;
            List<byte> remainData = new List<byte>();
            Stat stt = ReadHoldingRegister(0x1160, 16, out result);

            if (stt != Stat.SUCCESS) return mapping;

            remainData.AddRange(result);

            stt = ReadHoldingRegister(0x1170, 16, out result);

            if (stt == Stat.SUCCESS) {
                remainData.AddRange(result);

                int tempVal = 0;
                mapping = new Dictionary<Func_NET_IN, Param_NET_IN>();
                foreach (byte item in Enum.GetValues(typeof(Func_NET_IN))) {
                    tempVal = (remainData[item * 4 + 2] << 8) + remainData[item * 4 + 3];
                    mapping.Add((Func_NET_IN)item, (Param_NET_IN)tempVal);
                }
            }

            return mapping;
        }

        /// <summary>取得當前是否有開啟減速停止</summary>
        /// <param name="bit">於 NET-INPUT 中的編號，預設為 5</param>
        /// <returns>(Null)讀取失敗  (<see langword="false"/>)立即停止  (<see langword="true"/>)減速停止</returns>
        public bool? GetStopMode(Func_NET_OUT bit = Func_NET_OUT.NET_OUT_5) {
            bool? value = null;

            List<byte> result;
            Stat stt = ReadHoldingRegister(0x007D, 1, out result);

            if (stt == Stat.SUCCESS) {
                int andVal = 0x01 << (byte)bit;
                value = (result[1] & andVal) == andVal ? true : false;
            }

            return value;
        }

        #endregion

        #region Action
        /// <summary>清除控制器錯誤</summary>
        /// <returns>Status Code</returns>
        /// <remarks>此為 Falling-Triggered</remarks>
        public Stat CleanAlarm() {
            List<byte> result;
            Stat stt = WriteSingleRegister(0x0181, 1, out result);
            if (stt == Stat.SUCCESS) stt = WriteSingleRegister(0x0181, 0, out result);
            return stt;
        }

        /// <summary>命令控制器重新計算命令，類似 RESET。需於馬達停止且無錯誤時才可使用</summary>
        /// <returns>Status Code</returns>
        public Stat Configuration() {
            List<byte> result;
            Stat stt = WriteSingleRegister(0x018D, 1, out result);
            if (stt == Stat.SUCCESS) stt = mModbusRTU.WriteSingleRegister(0x018D, 0, out result);
            return stt;
        }

        /// <summary>命令馬達執行正轉</summary>
        /// <param name="bit">於 NET-IN 的編號，預設為 3</param>
        /// <returns>Status Code</returns>
        public Stat Forward(Func_NET_IN bit = Func_NET_IN.NET_IN_3) {
            int value;
            List<byte> result;
            Stat stt = ReadHoldingRegister(0x007D, 1, out result);

            if (stt == Stat.SUCCESS) {
                value = (result[0] << 8) + result[1];
                value |= 0x01 << (byte)bit;

                stt = WriteSingleRegister(0x007D, (ushort)value, out result);
            }

            return stt;
        }

        /// <summary>命令馬達執行反轉</summary>
        /// <param name="bit">於 NET-IN 的編號，預設為 4</param>
        /// <returns>Status Code</returns>
        public Stat Reverse(Func_NET_IN bit = Func_NET_IN.NET_IN_4) {
            int value;
            List<byte> result;
            Stat stt = ReadHoldingRegister(0x007D, 1, out result);

            if (stt == Stat.SUCCESS) {
                value = (result[0] << 8) + result[1];
                value |= 0x01 << (byte)bit;

                stt = WriteSingleRegister(0x007D, (ushort)value, out result);
            }

            return stt;
        }

        /// <summary>命令馬達停止運轉，僅限由 FWD 或 REV 所觸發的動作</summary>
        /// <param name="fwdBit">於 NET-IN 的編號，預設為 3</param>
        /// <param name="revBit">於 NET-IN 的編號，預設為 4</param>
        /// <returns>Status Code</returns>
        public Stat MotionStop(Func_NET_IN fwdBit = Func_NET_IN.NET_IN_3, Func_NET_IN revBit = Func_NET_IN.NET_IN_4) {
            int value;
            List<byte> result;
            Stat stt = ReadHoldingRegister(0x007D, 1, out result);

            if (stt == Stat.SUCCESS) {
                value = (result[0] << 8) + result[1];
                value &= ((0x01 << (byte)fwdBit) + (0x01 << (byte)revBit)) ^ 0xFFFF;

                stt = WriteSingleRegister(0x007D, (ushort)value, out result);
            }

            return stt;
        }

        /// <summary>更改剎車狀態</summary>
        /// <param name="release">(<see langword="true"/>)解除剎車 (<see langword="false"/>)保持剎車</param>
        /// <param name="bit">於 NET-IN 的編號，預設為 6</param>
        /// <returns>Status Code</returns>
        public Stat Breaker(bool release, Func_NET_IN bit = Func_NET_IN.NET_IN_6) {
            int value;
            List<byte> result;
            Stat stt = ReadHoldingRegister(0x007D, 1, out result);

            if (stt == Stat.SUCCESS) {
                value = (result[0] << 8) + result[1];
                if (release) value |= 0x01 << (byte)bit;
                else value &= (0x01 << (byte)bit) ^ 0xFFFF;

                stt = WriteSingleRegister(0x007D, (ushort)value, out result);
            }

            return stt;
        }

        #endregion

        #endregion

    }
}
