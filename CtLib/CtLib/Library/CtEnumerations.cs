namespace CtLib.Library {

    /// <summary>數字進制</summary>
    public enum NumericFormats : byte {
        /// <summary>二進制</summary>
        Binary = 2,
        /// <summary>八進制</summary>
        Octal = 8,
        /// <summary>十進制</summary>
        Decimal = 10,
        /// <summary>十六進制</summary>
        Hexadecimal = 16
    }

    /// <summary>路徑種類</summary>
    public enum SystemPath : byte {
        /// <summary>CtLib 或 CAMPro 之資料夾主路徑</summary>
        MainDirectory = 0,
        /// <summary>Config 資料夾</summary>
        Configuration = 1,
        /// <summary>Log 記錄檔資料夾</summary>
        Log = 2,
        /// <summary>Recipe 參數資料夾</summary>
        Recipe = 3,
        /// <summary>Project 專案程式資料夾</summary>
        Project = 4,
        /// <summary>使用者帳號與密碼紀錄檔案</summary>
        UserManagement = 5,
        ///<summary>影像辨識模組</summary>
        Model = 6
        
    }

    /// <summary>指定事件記錄檔項目的事件型別</summary>
    public enum EventLogType : byte {
        /// <summary>錯誤事件。 這表示是使用者應該知道的重要問題，通常是功能或資料的遺失。</summary>
        Error = 1,
        /// <summary>警告事件。 這表示不是立即重要的問題，但是可能表示將來會引發問題的狀況。</summary>
        Warning = 2,
        /// <summary>資訊事件。 這表示重要成功的作業。</summary>
        Information = 4,
        /// <summary>失敗稽核事件。 這表示受稽核的存取嘗試失敗時發生的安全性事件，例如無法開啟檔案。</summary>
        FailureAudit = 16,
        /// <summary>成功稽核事件。 這表示受稽核存取嘗試成功時所發生的安全性事件，例如登錄成功。</summary>
        SuccessAudit = 8
    }

    /// <summary>加解密模式</summary>
    public enum CryptoMode : byte {
        /// <summary>Base64</summary>
        Base64 = 64,
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
        Input,
        /// <summary>Output 輸出端點</summary>
        Output,
        /// <summary>In/Out 可輸出輸入之端點</summary>
        InOut
    }

    /// <summary>設備類型</summary>
    public enum Devices : byte {
        /// <summary>Adept ACE</summary>
        AdeptACE,
        /// <summary>Beckhoff Ads Devices</summary>
        Beckhoff,
        /// <summary>CAMPro System Objects</summary>
        CAMPro,
        /// <summary>Wago Devices</summary>
        WAGO,
        /// <summary>Delta Devices</summary>
        DELTA,
        /// <summary>Universal Robots</summary>
        UR,
        /// <summary>Oriental Motors</summary>
        Oriental,
        /// <summary>IAI Devices</summary>
        IAI,
        ///<summary>HighFreq Devices</summary>
        MITSUBISHI_PLC
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
        None,
        /// <summary>回車符號(Carriage Return) 即 "\r" "0x0D"</summary>
        Cr,
        /// <summary>換行符號(Line feed) 即 "\n" "0x0A"</summary>
        Lf,
        /// <summary>MS DOS/Windows 之換行符號，即 "\r\n" "0x0D 0x0A" "<see cref="System.Environment.NewLine"/>"</summary>
        CrLf,
        /// <summary>使用者自訂</summary>
        Custom
    }

    /// <summary>介面語系</summary>
    public enum UILanguage : byte {
        /// <summary>中文(繁體)</summary>
        TraditionalChinese = 0,
        /// <summary>中文(簡體)</summary>
        SimplifiedChinese = 1,
        /// <summary>英文</summary>
        English = 2
    }

    /// <summary>Windows CodePage for Encoding。請參考 "http://en.wikipedia.org/wiki/Windows_code_page"</summary>
    public enum CodePages : int {
		/// <summary>簡體中文 (GB2312) 《信息技术信息交换用汉字编码字符集 基本集》又稱 GB0</summary>
		GB2312 = 936,
        /// <summary>繁體中文 (Big5)</summary>
        BIG5 = 950,
		/// <summary>Unicode (UTF-16)</summary>
		UTF16 = 1200,
		/// <summary>西歐語系 (Mac)</summary>
		MACINTOSH = 10000,
		/// <summary>Unicode (UTF-32)</summary>
		UTF32 = 12000,
		/// <summary>Unicode (UTF-32 位元組由大到小)</summary>
		UTF32BE = 12001,
		/// <summary>US-ASCII</summary>
		ASCII = 20127,
		/// <summary>簡體中文 (GB18030) 《信息技术信息交换用汉字编码字符集 基本集的扩充》</summary>
		GB18030 = 54936,
		/// <summary>Unicode (UTF-8)</summary>
		UTF8 = 65001
    }

    /// <summary>通訊交握之資料格式</summary>
    public enum TransDataFormats : byte {
        /// <summary>以 <see cref="string"/> 方式回傳，會以 <seealso cref="CodePages"/> 將資料轉為相對應的編碼並回傳</summary>
        @String = 0,
		/// <summary>以 <see cref="System.Collections.Generic.IEnumerable{T}"/> 方式回傳，可能為 <see cref="System.Array"/> 或 <seealso cref="System.Collections.Generic.List{T}"/>。 T 為 <seealso cref="byte"/></summary>
		EnumerableOfByte = 1
	}

    /// <summary>CtMsgBox 所使用的按鈕</summary>
    public enum MsgBoxBtn : byte {
        /// <summary>確認(OK)鈕</summary>
        OK = 1,
        /// <summary>取消(Cancel)鈕</summary>
        Cancel = 2,
        /// <summary>是(Yes)鈕</summary>
        Yes = 4,
        /// <summary>否(Yes)鈕</summary>
        No = 8,
        /// <summary>確認與取消 (OK + Cancel)</summary>
        OkCancel = 3,
        /// <summary>是與否 (Yes + No)</summary>
        YesNo = 12
    }

    /// <summary>CtMsgBox 樣式</summary>
    public enum MsgBoxStyle : byte {
        /// <summary>資訊。圖案為藍底「i」</summary>
        Information,
        /// <summary>警告。圖案為黃底「!」</summary>
        Warning,
        /// <summary>詢問。圖案為藍底「？」</summary>
        Question,
        /// <summary>錯誤。圖案為紅底「X」</summary>
        Error,
        /// <summary>無樣式</summary>
        None
    }

    /// <summary>資料排列順序</summary>
    public enum BitSequence : byte {
        /// <summary>閱讀順序。如 [0]0x01 [1]0xFF，實際上為 0x01FF</summary>
        Reading,
        /// <summary>電腦傳送順序。如 [0]0x02 [1]0xFE，實際上為 0xFE02</summary>
        Computer
    }

	/// <summary>網際網路控制訊息通訊協定(ICMP)回應訊息傳送至電腦的狀態</summary>
	/// <remarks>已對應至 <see cref="System.Net.NetworkInformation.IPStatus"/></remarks>
	public enum PingStatus {
		/// <summary>ICMP 回應要求失敗，原因不明</summary>
		Unknown = -1,
		/// <summary>ICMP 回應要求成功；已收到 ICMP 回應的回覆，<see cref="System.Net.NetworkInformation.PingReply"/> 將包含有效資料</summary>
		Success = 0,
		/// <summary>沒有在指定時間內收到 ICMP 回應的回覆。回覆允許的預設時間為 5 秒，請於 Send / AsyncSend 更改此逾時設定時間</summary>
		Timeout = 11010,
		/// <summary>ICMP 回應要求失敗，因為來源和目的電腦之間沒有有效的路由</summary>
		BadRoute = 11012,
		/// <summary>ICMP 回應要求失敗，因為目的端 IP 位址無法接收 ICMP 回應要求或不應該出現在任何 IP 資料包的目的位址欄位中</summary>
		BadDestination = 11018,
		/// <summary>ICMP 回應要求失敗，因為無法連接 ICMP 回應訊息中指定的目的電腦；問題的實際原因不明</summary>
		Unreachable = 11040,
		/// <summary>ICMP 回應要求失敗，因為 ICMP 回應訊息中指定的來源位址和目的位址不在同一個範圍內</summary>
		ScopeMismatch = 11045
	}

	/// <summary>序列化之方法</summary>
	public enum Serialization {
		/// <summary>使用物件序列化為二進位資料</summary>
		Binary,
		/// <summary>使用 SOAP 格式序列化物件</summary>
		SOAP,
		/// <summary>使用 XML 格式進行物件的序列化</summary>
		XML,
		/// <summary>使用 .Net 的資料合約序列化</summary>
		DataContract
	}
}
