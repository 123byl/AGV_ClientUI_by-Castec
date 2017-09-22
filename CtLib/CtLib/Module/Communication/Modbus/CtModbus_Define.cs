
namespace CtLib.Module.Modbus {
    /// <summary>Modbus 功能碼(Function Code)</summary>
    /// <remarks>
    /// 各廠商之名稱可能稍有不同，但其代表意義相同！這邊以 Advance, Wago 為主
    /// <para>關於 Modbus 可參考 "http://www.simplymodbus.ca/FAQ.htm"</para>
    /// </remarks>
    public enum ModbusFunction : byte {
        /// <summary>
        /// 讀取輸入輸出端(InOut, Coil)當前狀態
        /// <para>可讀取連續性資料，回傳十六進制表示各bit狀態，如 0xD9 = 1101 1001 分別表示各 Coil 狀態， "0" OFF, "1" ON</para>
        /// </summary>
        READ_COIL_STATUS = 0x01,
        /// <summary>
        /// 讀取輸入端(Input)當前狀態
        /// <para>可讀取連續性資料，回傳十六進制表示各bit狀態，如 0xD9 = 1101 1001 分別表示各 Coil 狀態， "0" OFF, "1" ON</para>
        /// <para>此功能與 READ_COIL_STATUS 最大不同在於此功能僅限 Input</para>
        /// </summary>
        READ_INPUT_STATUS = 0x02,
        /// <summary>
        /// 讀取暫存器(Register)當前資料
        /// <para>可讀取連續的暫存器，回傳每個暫存器的數值，數值部分需參考各廠商手冊，例如 Wago 750-352 (16bit Register) 將回傳 0x0D 0xF7 (2byte)</para>
        /// </summary>
        READ_HOLDING_REGISTERS = 0x03,
        /// <summary>
        /// 讀取輸入(Input)暫存器(Register)當前資料
        /// <para>可讀取連續的輸入暫存器，回傳每個暫存器的數值，數值部分需參考各廠商手冊，例如 Wago 750-352 (16bit Register) 將回傳 0x0D 0xF7 (2byte)</para>
        /// <para>此功能與 READ_HOLDING_REGISTERS 最大不同在於此功能僅限 Input</para>
        /// </summary>
        READ_INPUT_REGISTERS = 0x04,
        /// <summary>
        /// 設定單一輸入輸出(InOut, Coil)數值
        /// <para>寫入單一狀態，通常以 2byte 為主，如 "0x0000" OFF, "0xFF00" ON</para>
        /// </summary>
        FORCE_SINGLE_COIL = 0x05,
        /// <summary>
        /// 設定單一暫存器(Register)數值
        /// <para>僅寫入單一暫存器，須搭配各廠商手冊，例如 IAI Robo Cylinder (16bit Register) 需寫入如 0x040C (2byte)</para>
        /// </summary>
        PRESET_SINGLE_REGISTER = 0x06,
        /// <summary>
        /// 設定連續多個輸入輸出(InOut, Coil)數值
        /// <para>帶入資料以二進制表示各bit狀態，然後再轉為十六進制寫入設備。如 Output 0~7: 1001 0110，則寫入 0x96 、 Coil 數量 8 (個數) 、 Data 數量 1 (byte)</para>
        /// </summary>
        FORCE_MULTI_COILS = 0x0F,
        /// <summary>
        /// 設定連續多個暫存器(Register)數值
        /// <para>須搭配各廠商手冊，例如 Wago 750-352 (16bit Register) 需寫入如 0x040C (2byte)。</para>
        /// <para>如 4000~4002 可帶入 0x040C 0xFFFF 0x8080 ， Reg 數量 3 (個數)、 Data 數量 6 (byte)</para>
        /// </summary>
        PRESET_MULTI_REGISTERS = 0x10,
        /// <summary>
        /// Modbus 例外事項
        /// <para>各家廠商之 Exception Function 不盡相同，但大多大於 0x80 者都是指 Exception。詳細 Error Code 請查手冊</para>
        /// </summary>
        EXCEPTION = 0x80
    }
}
