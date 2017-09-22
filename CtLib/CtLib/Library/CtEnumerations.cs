namespace CtLib.Library {

    /// <summary>數字進制</summary>
    public enum NumericFormats : byte {
        /// <summary>二進制</summary>
        BINARY = 2,
        /// <summary>八進制</summary>
        OCTAL = 8,
        /// <summary>十進制</summary>
        DECIMAL = 10,
        /// <summary>十六進制</summary>
        HEXADECIMAL = 16
    }

    /// <summary>路徑種類</summary>
    public enum SystemPath : byte {
        /// <summary>CtLib 或 CAMPro 之資料夾主路徑</summary>
        MAIN_DIRECTORY = 0,
        /// <summary>Config 資料夾</summary>
        CONFIG = 1,
        /// <summary>Log 記錄檔資料夾</summary>
        LOG = 2,
        /// <summary>Recipe 參數資料夾</summary>
        RECIPE = 3,
        /// <summary>Project 專案程式資料夾</summary>
        PROJECT = 4,
        /// <summary>使用者帳號與密碼紀錄檔案</summary>
        USER_MANAGER = 5
    }

    /// <summary>指定事件記錄檔項目的事件型別</summary>
    public enum EventLogType : byte {
        /// <summary>錯誤事件。 這表示是使用者應該知道的重要問題，通常是功能或資料的遺失。</summary>
        ERROR = 1,
        /// <summary>警告事件。 這表示不是立即重要的問題，但是可能表示將來會引發問題的狀況。</summary>
        WARNING = 2,
        /// <summary>資訊事件。 這表示重要成功的作業。</summary>
        INFORMATION = 4,
        /// <summary>失敗稽核事件。 這表示受稽核的存取嘗試失敗時發生的安全性事件，例如無法開啟檔案。</summary>
        FAILURE_AUDIT = 16,
        /// <summary>成功稽核事件。 這表示受稽核存取嘗試成功時所發生的安全性事件，例如登錄成功。</summary>
        SUCCESS_AUDIT = 8
    }

    /// <summary>加解密模式</summary>
    public enum CryptoMode : byte {
        /// <summary>Base64</summary>
        BASE64 = 64,
        /// <summary>Advanced Encryption Standard</summary>
        AES256 = 255
    }

    /// <summary>湊雜碼模式</summary>
    public enum HashMode : byte {
        /// <summary>Message-Digest Algorithm 5</summary>
        MD5,
        /// <summary>Secure Hash Algorithm</summary>
        SHA1,
        /// <summary>Secure Hash Algorithm - 256bit</summary>
        SHA2_256,
        /// <summary>Secure Hash Algorithm - 384bit</summary>
        SHA2_384,
        /// <summary>Secure Hash Algorithm - 512bit</summary>
        SHA2_512
    }

    /// <summary>I/O 類型</summary>
    public enum IOTypes : byte {
        /// <summary>Input 輸入端點</summary>
        INPUT,
        /// <summary>Output 輸出端點</summary>
        OUTPUT,
        /// <summary>In/Out 可輸出輸入之端點</summary>
        INOUT
    }

    /// <summary>各類 Opcode 範圍</summary>
    public enum OpcodeRange : ushort {

        /*-- System --*/
        /// <summary>Opcode 於 System 的基數</summary>
        SYSTEM_BASE = 0,
        /// <summary>Opcode 於 System 的範圍結尾</summary>
        SYSTEM_RANGE = 999,

        /*-- CAMPro --*/
        /// <summary>Opcode 於 CAMPro 的基數</summary>
        CAMPRO_BASE = 1000,
        /// <summary>Opcode 於 CAMPro 的範圍結尾</summary>
        CAMPRO_RANGE = 1999,

        /*-- Mobile Robot --*/
        /// <summary>Opcode 於 Mobile 的基數</summary>
        MOBILE_BASE = 2000,
        /// <summary>Opcode 於 Mobile 的範圍結尾</summary>
        MOBILE_RANGE = 2999,

        /*-- WAGO --*/
        /// <summary>Opcode 於 WAGO module 的基數</summary>
        WAGO_IO_BASE = 11000,
        /// <summary>Opcode 於 WAGO module 的範圍結尾</summary>
        WAGO_IO_RANGE = 11999,

        /*-- Beckhoff --*/
        /// <summary>Opcode 於 Beckhoff module 的基數</summary>
        BECKHOFF_BASE = 12000,
        /// <summary>Opcode 於 Beckhoff module 的範圍結尾</summary>
        BECKHOFF_RANGE = 12999,

        /*-- ARCL --*/
        /// <summary>Opcode 於 ARCL module 的基數</summary>
        ARCL_BASE = 21000,
        /// <summary>Opcode 於 ARCL module 的範圍結尾</summary>
        ARCL_RANGE = 21999,

        /*-- ACE --*/
        /// <summary>Opcode 於 Ace module 的基數</summary>
        ACE_BASE = 22000,
        /// <summary>Opcode 於 Ace module 的範圍結尾</summary>
        ACE_RANGE = 22999,

        /*-- Vision --*/
        /// <summary>Opcode 於 Vision module 的基數</summary>
        VISION_BASE = 30000,
        /// <summary>Opcode 於 Vision module 的範圍結尾</summary>
        VISION_RANGE = 30999,

        /*-- Dimmer --*/
        /// <summary>Opcode 於 調光器模組 的基數</summary>
        DIMMER_BASE = 31000,
        /// <summary>Opcode 於 調光器模組 的範圍結尾</summary>
        DIMMER_RANGE = 31999,

        /*-- Stäubli --*/
        /// <summary>Opcode 於 Stäubli Module 的基數</summary>
        STAUBLI_BASE = 40000,
        /// <summary>Opcode 於 Stäubli Module 的範圍結尾</summary>
        STAUBLI_RANGE = 40999,

        /*-- Delta --*/
        /// <summary>Opcode 於 Delta Module 的基數</summary>
        DELTA_BASE = 50000,
        /// <summary>Opcode 於 Delta Module 的範圍結尾</summary>
        DELTA_RANGE = 50999,

        /*-- Other --*/
        /// <summary>Opcode 於 MySQL module 的基數</summary>
        MYSQL_BASE = 60000,
        /// <summary>Opcode 於 MySQL module 的範圍結尾</summary>
        MYSQL_RANGE = 60099,
        /// <summary>Opcode 於 SECS/GEM module 的基數</summary>
        CIM_BASE = 60100,
        /// <summary>Opcode 於 SECS/GEM module 的範圍結尾</summary>
        CIM_RANGE = 60199,
        /// <summary>Opcode 於 Brillian RFID module 的基數</summary>
        BRILLIAN_RFID_BASE = 60300,
        /// <summary>Opcode 於 Brillian RFID module 的範圍結尾</summary>
        BRILLIAN_RFID_RANGE = 60399
    }

    /// <summary>設備類型</summary>
    public enum Devices : byte {
        /// <summary>Adept ACE</summary>
        ADEPT_ACE,
        /// <summary>Beckhoff Ads Devices</summary>
        BECKHOFF_PLC,
        /// <summary>CAMPro System Objects</summary>
        CAMPro,
        /// <summary>Wago Devices</summary>
        WAGO,
        /// <summary>Delta Devices</summary>
        DELTA
    }

    /// <summary>設備狀態</summary>
    public enum EquipState : byte {

        /*-- Initial State --*/
        /// <summary>
        /// 設備初始化中
        /// <para>System Initializing</para>
        /// </summary>
        EI_SYSTEM_INIT = 1,
        /// <summary>
        /// 設備等待初始化
        /// <para>System Stand-By for Initial</para>
        /// </summary>
        EI_STAND_BY = 2,

        /*-- Operating State --*/
        /// <summary>
        /// 設備閒置
        /// <para>Equipment Idle</para>
        /// </summary>
        OP_IDLE = 3,
        /// <summary>
        /// 設備正常執行中
        /// <para>Equipment Operating/Executing in fine state</para>
        /// </summary>
        OP_EXECUTING = 4,
        /// <summary>
        /// 設備暫停
        /// <para>Equipment stand-by after pause operation</para>
        /// </summary>
        OP_PAUSE = 5,
        /// <summary>
        /// 設備停止
        /// <para>Equipment Stop/Complete (Without any exception)</para>
        /// </summary>
        OP_STOP = 6,
        /// <summary>
        /// 設備停止(操作取消)
        /// <para>Equipment stopped by abort operation</para>
        /// </summary>
        OP_ABORT = 7,
        /// <summary>
        /// 設備復歸中
        /// <para>Equipment recovering</para>
        /// </summary>
        OP_RECOVERY = 8,
        /// <summary>
        /// 設備停止(安全門開啟)
        /// <para>Equipment stopped by safety-door opened</para>
        /// </summary>
        OP_DOOR_OPEN = 9,

        /*-- Error State --*/
        /// <summary>
        /// 系統錯誤(警告)
        /// <para>Equipment warning raised</para>
        /// </summary>
        ER_WARNING = 10,
        /// <summary>
        /// 系統錯誤(初始化失敗)
        /// <para>System initial failed</para>
        /// </summary>
        ER_INIT_FAILED = 11,
        /// <summary>
        /// 系統錯誤(輕微等級)
        /// <para>Equipment light error</para>
        /// </summary>
        ER_LIGHT_ERROR = 12,
        /// <summary>
        /// 系統錯誤(一般等級)
        /// <para>Equipment normal error</para>
        /// </summary>
        ER_NORMAL_ERROR = 13,
        /// <summary>
        /// 系統錯誤(嚴重等級)
        /// <para>Equipment fatal error</para>
        /// </summary>
        ER_FATAL_ERROR = 14,
        /// <summary>
        /// 緊急停止
        /// <para>Equipment emergency stop</para>
        /// </summary>
        ER_EMERGENCY_STOP = 15,
        /// <summary>
        /// 設備處於非安全區域
        /// <para>Equipment stay in unsafe region</para>
        /// </summary>
        ER_UNSAFEZONE = 16,

        /*-- Special State --*/
        /// <summary>
        /// 未知狀態
        /// <para>System can not handle equipment state</para>
        /// </summary>
        OT_UNKNOWN = 0
    }

    /// <summary>用於讀取/寫入串流資料時之結尾符號</summary>
    public enum EndChar : byte {
        /// <summary>不帶任何結尾符號</summary>
        NONE,
        /// <summary>回車符號(Carriage Return) 即 "\r" "0x0D"</summary>
        CR,
        /// <summary>換行符號(Line feed) 即 "\n" "0x0A"</summary>
        LF,
        /// <summary>MS DOS/Windows 之換行符號，即 "\r\n" "0x0D 0x0A" "<see cref="System.Environment.NewLine"/>"</summary>
        CRLF,
        /// <summary>使用者自訂</summary>
        CUSTOM
    }

    /// <summary>介面語系</summary>
    public enum UILanguage : byte {
        /// <summary>中文(繁體)</summary>
        TRADITIONAL_CHINESE = 0,
        /// <summary>中文(簡體)</summary>
        SIMPLIFIED_CHINESE = 1,
        /// <summary>英文</summary>
        ENGLISH = 2
    }

    /// <summary>Socket 對應端點</summary>
    public enum SocketModes : byte {
        /// <summary>用戶端</summary>
        CLIENT = 0,
        /// <summary>伺服端</summary>
        SERVER = 1
    }

    /// <summary>Socket 連線協議</summary>
    public enum CommunicationModes : byte {
        /// <summary>此連線為 TCP/IP</summary>
        TCP_IP,
        /// <summary>此連線為 Telnet</summary>
        TELNET
    }

    /// <summary>Windows CodePage for Encoding。請參考 "http://en.wikipedia.org/wiki/Windows_code_page"</summary>
    public enum CodePages : int {
        /// <summary>Traditional Chinese (Taiwan, HongKong)</summary>
        BIG5 = 950,
        /// <summary>ASCII Latin 1 / Western European</summary>
        ASCII = 1252,
        /// <summary>BMP of ISO-10646, UTF-8</summary>
        UTF8 = 65001
    }

    /// <summary>事件回傳的資料格式</summary>
    public enum TransmissionDataFormats : byte {
        /// <summary>以 string 方式回傳，會以 CodePage 屬性將資料轉為 string 並回傳</summary>
        STRING,
        /// <summary>以 List(Of byte) 方式回傳</summary>
        BYTE_ARRAY
    }

    /// <summary>CtMsgBox 所使用的按鈕</summary>
    public enum MsgBoxButton : byte {
        /// <summary>確認(OK)鈕</summary>
        OK = 1,
        /// <summary>取消(Cancel)鈕</summary>
        CANCEL = 2,
        /// <summary>是(Yes)鈕</summary>
        YES = 4,
        /// <summary>否(Yes)鈕</summary>
        NO = 8,
        /// <summary>確認與取消 (OK + Cancel)</summary>
        OK_CANCEL = 3,
        /// <summary>是與否 (Yes + No)</summary>
        YES_NO = 12
    }

    /// <summary>CtMsgBox 樣式</summary>
    public enum MsgBoxStyle : byte {
        /// <summary>資訊。圖案為藍底「i」</summary>
        INFORMATION,
        /// <summary>警告。圖案為黃底「!」</summary>
        WARNING,
        /// <summary>詢問。圖案為藍底「？」</summary>
        QUESTION,
        /// <summary>錯誤。圖案為紅底「X」</summary>
        ERROR,
        /// <summary>無樣式</summary>
        NONE
    }
}
