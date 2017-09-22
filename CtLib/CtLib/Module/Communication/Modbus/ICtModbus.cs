using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CtLib.Library;

namespace CtLib.Module.Modbus {
    /// <summary>標準 Modbus 之格式</summary>
    interface ICtModbus {

        /// <summary>[FC01] 讀取 Coil (InOut, Output) 狀態</summary>
        /// <param name="addr">IO Address</param>
        /// <param name="coilCount">連續讀取的數量。如欲 0~7 則輸入 8 </param>
        /// <param name="state">各點的狀態  (True)ON  (False)OFF</param>
        /// <returns>Status Code</returns>
        Stat ReadCoilStatus(ushort addr, ushort coilCount, out List<bool> state);

        /// <summary>[FC03] 讀取暫存器(Register)數值，可為輸入或輸出之暫存器(Input, Output, InOut)</summary>
        /// <param name="addr">Register Address</param>
        /// <param name="regCount">連續讀取的暫存器數量。如 4000~4007 則輸入 8</param>
        /// <param name="value">(True)ON  (False)OFF</param>
        /// <returns>Status Code</returns>
        /// <remarks>以 Wago IO 為例，一個 Register 都是 16bit，所以回傳的資訊以 ushort (uint16) 為主</remarks>
        Stat ReadHoldingRegister(ushort addr, ushort regCount, out List<byte> value);

        /// <summary>[FC02] 讀取輸入 Bit 目前狀態，僅可讀取輸入端點(Input Only)</summary>
        /// <param name="addr">起始的 IO Address</param>
        /// <param name="bitCount">連續讀取的長度。如要讀取 0~7 則 addr=0 length=8</param>
        /// <param name="value">各 IO 的 ON OFF 狀態</param>
        /// <returns>Status Code</returns>
        Stat ReadInputStatus(ushort addr, ushort bitCount, out List<bool> value);

        /// <summary>[FC04] 讀取暫存器(Register)數值，僅可讀取輸入暫存器(Input Only)</summary>
        /// <param name="addr">起始的 Register Address</param>
        /// <param name="regCount">連續讀取的長度。如要讀取 512~514 則 addr=512 length=3</param>
        /// <param name="value">各 Resiger 的數值</param>
        /// <returns>Status Code</returns>
        Stat ReadInputRegisters(ushort addr, ushort regCount, out List<byte> value);

        /// <summary>[FC05] 寫入單一 bit 狀態，並回傳結果  [Force Single Coil]</summary>
        /// <param name="addr">IO Address</param>
        /// <param name="value">欲寫入之狀態  (True)ON (False)OFF</param>
        /// <param name="result">回傳之結果</param>
        /// <returns>Status Code</returns>
        Stat WriteSingleCoil(ushort addr, bool value, out List<byte> result);

        /// <summary>[FC06] 寫入單一 Register 數值，並回傳結果  [Preset Single Register]</summary>
        /// <param name="addr">Register Address</param>
        /// <param name="value">欲寫入之數值</param>
        /// <param name="result">回傳之結果</param>
        /// <returns>Status Code</returns>
        Stat WriteSingleRegister(ushort addr, ushort value, out List<byte> result);

        /// <summary>[FC15] 寫入連續 Bit 狀態  [Force Multi Coils]</summary>
        /// <param name="addr">起始 IO Address</param>
        /// <param name="value">欲寫入之狀態</param>
        /// <param name="result">回傳之結果</param>
        /// <returns>Status Code</returns>
        Stat WriteMultiCoils(ushort addr, List<bool> value, out List<byte> result);

        /// <summary>[FC16] 寫入連續 Register 數值  [Preset Multi Registers]</summary>
        /// <param name="addr">起始 Register Address</param>
        /// <param name="data">欲寫入的資料</param>
        /// <param name="result">回傳之結果</param>
        /// <returns>Status Code</returns>
        Stat WriteMultiRegisters(ushort addr, List<ushort> data, out List<byte> result);
    }
}
