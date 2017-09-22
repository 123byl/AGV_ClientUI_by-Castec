using CtLib.Module.Ultity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CtLib.Library {

    /// <summary>
    /// I/O 相關資訊
    /// <para>如 I/O 編號、名稱、狀態等等</para>
    /// </summary>
    /// <remarks>
    /// 目前有特別針對調整: Adept、Beckhoff、Wago
    /// <para>如還有新的設備請再增加</para>
    /// </remarks>
    public class CtIOData : IComparable, IComparer<CtIOData> {

        #region Version

        /// <summary>CtIOData 版本訊息</summary>
        /// <remarks><code>
        /// 1.0.0  Ahern [2015/02/04]
        ///     + 從 CtNexcom_IO 獨立至此
        ///     
        /// 1.1.0  Ahern [2015/05/29]
        ///     + Inherit IComarable for .Sort() and .OrderBy()
        ///     + Inherit IComparer&lt;CtIOData&gt; where may need Comparator
        ///     
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 1, 0, "2015/05/29", "Ahern Kuo");

        #endregion

        #region Declaration - Members
        /// <summary>I/O 識別編號。如 "Y0001"</summary>
        /// <remarks>因我方 I/O 以 X(input)、Y(output) 且依照卡號區分，如 Y0018 表示輸出地一張卡第八埠。故此處以 string 為主</remarks>
        public string ID { get; set; }
        /// <summary>當前狀態</summary>
        public bool State { get; set; }
        /// <summary>註解</summary>
        public string Comment { get; set; }
        /// <summary>裝置</summary>
        public Devices Device { get; set; }
        /// <summary>Input/Output</summary>
        public IOTypes @Type { get; set; }
        /// <summary>
        /// 變數名稱
        /// <para>[Beckhoff PLC] 相對應的變數，如 "MAIN.pF_AutoRun"</para>
        /// <para>[Delta PLC] 相對應的繼電器，如 "M3"</para>
        /// </summary>
        public string Variable { get; set; }
        /// <summary>
        /// 索引值
        /// <para>[Beckhoff PLC] Array Index 的索引。如 pA_Flag[7] = MAIN.pF_AutoRun，則此處帶入 7</para>
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// I/O 編號
        /// <para>[Adept] 相對應於 IO Panel 的 編號，如 8, 1008 等等</para>
        /// <para>[Wago]  Coil/Input 編號，如 3,6,513,516 等</para>
        /// </summary>
        public int IONum { get; set; }
        /// <summary>
        /// Register 編號
        /// <para>[Beckhoff I/O] 相對應的 Register 編號</para>
        /// <para>[Wago I/O] 相對應的 Register 編號</para>
        /// </summary>
        public ushort RegNum { get; set; }

        /// <summary>
        /// Register 位元索引
        /// <para>[Beckhoff I/O] 相對應的 Register 之 Bit 數</para>
        /// <para>[Wago I/O] 相對應於 Register 之 Bit 數</para>
        /// </summary>
        public byte RegBit { get; set; }

        /// <summary>
        /// 用於 Dictionary 之索引數值
        /// <para>如有使用可再透過轉換轉成 <see cref="Enum"/> 或是相關 TKey</para>
        /// </summary>
        public ushort EnumIdx { get; set; }
        #endregion

        #region Function - Constructors
        /// <summary>建立帶預設值之 IOData。請於之後自行填入</summary>
        public CtIOData() {
            ID = "N/A";
            IONum = 0;
            Comment = "";
            Device = Devices.ADEPT_ACE;
            Type = IOTypes.INOUT;
            State = false;
            EnumIdx = 0;
            Index = -1;
        }

        /// <summary>建立 Adept 專用之 IOData。請直接帶入 IO 編號與註解，其餘將自動填入</summary>
        /// <param name="ioNum">I/O 編號。如 36, 1008, 2003 等</param>
        /// <param name="comment">註解。如 "破真空"</param>
        /// <param name="stt">當前狀態。(True)ON  (False)OFF</param>
        /// <param name="enumidx">於 Dictionary 或是相關陣列之索引值</param>
        public CtIOData(int ioNum, string comment, ushort enumidx = 0, bool stt = false) {
            ID = ioNum.ToString();
            IONum = ioNum;
            Comment = comment;
            Device = Devices.ADEPT_ACE;
            Type = (ioNum > 2000) ? IOTypes.INOUT : ((ioNum > 1000) ? IOTypes.INPUT : IOTypes.OUTPUT);
            State = stt;
            EnumIdx = enumidx;
        }

        /// <summary>建立 Beckhoff PLC 專用之 IOData</summary>
        /// <param name="id">I/O 名稱。如 "X0013" "Y0027"</param>
        /// <param name="type">此 I/O 點之輸入輸出類型</param>
        /// <param name="variable">對應的變數名稱</param>
        /// <param name="comment">註解。如 "Clamp 上升位置"</param>
        /// <param name="idx">於 IO Array 之索引值。預設為 -1 (忽略)</param>
        /// <param name="stt">當前狀態。(True)ON  (False)OFF</param>
        /// <param name="enumidx">於 Dictionary 或是相關陣列之索引值</param>
        public CtIOData(string id, IOTypes type, string variable, string comment, ushort enumidx, int idx, bool stt = false) {
            ID = id;
            Comment = comment;
            Device = Devices.BECKHOFF_PLC;
            Type = type;
            Variable = variable;
            Index = idx;
            State = stt;
            EnumIdx = enumidx;
        }

        /// <summary>建立 Delta PLC 專用之 IOData</summary>
        /// <param name="id">I/O 名稱。如 "X0013" "Y0027"</param>
        /// <param name="type">此 I/O 點之輸入輸出類型</param>
        /// <param name="variable">對應的繼電器名稱，如 "M200"</param>
        /// <param name="comment">註解。如 "Clamp 上升位置"</param>
        /// <param name="stt">當前狀態。(True)ON  (False)OFF</param>
        /// <param name="enumidx">於 Dictionary 或是相關陣列之索引值</param>
        public CtIOData(string id, IOTypes type, string variable, string comment, ushort enumidx, bool stt = false) {
            ID = id;
            Comment = comment;
            Device = Devices.BECKHOFF_PLC;
            Type = type;
            Variable = variable;
            State = stt;
            EnumIdx = enumidx;
        }

        /// <summary>建立 Wago I/O 專用之 IOData</summary>
        /// <param name="id">I/O 名稱。如 "X0042" "Y0019"</param>
        /// <param name="type">此 I/O 點之輸入輸出類型</param>
        /// <param name="ioNum">Coil/Input 之編號。如 5, 517</param>
        /// <param name="regNum">暫存器(Register)編號。如 Reg 4073 第 7 Bit，則此處帶入 4073</param>
        /// <param name="regIdx">暫存器內之對應 bit 數。如 Reg 4073 第 7 Bit，則此處帶入 7</param>
        /// <param name="comment">註解。如 "夾爪 Servo ON"</param>
        /// <param name="stt">當前狀態。(True)ON  (False)OFF</param>
        /// <param name="enumidx">於 Dictionary 或是相關陣列之索引值</param>
        public CtIOData(string id, IOTypes type, int ioNum, ushort regNum, byte regIdx, string comment, ushort enumidx = 0, bool stt = false) {
            ID = id;
            IONum = ioNum;
            RegNum = regNum;
            RegBit = regIdx;
            Comment = comment;
            State = stt;
            Device = Devices.WAGO;
            Type = type;
            EnumIdx = enumidx;
        }
        #endregion

        #region Function - Inheritance
        /// <summary>嘗試進行兩個 CtIOData 的比較。實作 IComparable 所需條件</summary>
        /// <param name="obj">欲比較的 CtIOData</param>
        /// <returns>(-1)引數物件比較大  (0)相等  (1)原先物件較大</returns>
        /// <remarks>據測試 .Sort() 與 .OrderBy() 都會觸發此段程式</remarks>
        int IComparable.CompareTo(object obj) {
            CtIOData temp = obj as CtIOData;

            /*-- 如果裝置不一樣，直接以 Enum 比較 --*/
            if (this.Device != temp.Device)
                return this.Device.CompareTo(temp.Device);
            else {  /*-- 同樣來源裝置，依照不同來源比較不同的部位 --*/
                if (this.Device == Devices.ADEPT_ACE) {
                    /* Adept ACE: 比較 I/O 大小 */
                    return this.IONum.CompareTo(temp.IONum);
                } else if (this.Device == Devices.BECKHOFF_PLC || this.Device == Devices.DELTA) {
                    /* Beckhoff | Delta: 先比較是不是都是 X 或 Y */
                    if (this.ID[0] == temp.ID[0]) {
                        int ioA, ioB;
                        if (int.TryParse(this.ID.ToLower().Replace("x", "").Replace("y", ""), out ioA)
                            && int.TryParse(temp.ID.ToLower().Replace("x", "").Replace("y", ""), out ioB))
                            return ioA.CompareTo(ioB);  //同樣的 X/Y 情況下，將後面的數字轉成 int 並直接比較大小
                        else return string.Compare(this.ID, temp.ID);   //如果是 X/Y 之外的新成員，直接以字串比較

                        /* 如果 X/Y 不同，直接使用字串比較就可 */
                    } else return string.Compare(this.ID, temp.ID);
                } else if (this.Device == Devices.WAGO) {
                    /* Wago: 先比較 Register 編號，如果相同再比較 Bit */
                    if (this.RegNum != temp.RegNum)
                        return this.RegNum.CompareTo(temp.RegNum);
                    else
                        return this.RegBit.CompareTo(temp.RegBit);

                    /* 如果以上條件都失敗，直接回傳相等大小 */
                } else return 0;
            }
        }

        /// <summary>嘗試進行兩個 CtIOData 的比較。實作 IComparer 所需條件</summary>
        /// <param name="dataA">第一個欲比較的 CtIOData</param>
        /// <param name="dataB">第二個欲比較的 CtIOData</param>
        /// <returns>(-1)B比較大  (0)相等  (1)A較大</returns>
        /// <remarks>如果有需要 IComparer 比較子的，可以直接套用</remarks>
        int IComparer<CtIOData>.Compare(CtIOData dataA, CtIOData dataB) {
            /*-- 如果裝置不一樣，直接以 Enum 比較 --*/
            if (dataA.Device != dataB.Device)
                return dataA.Device.CompareTo(dataB.Device);
            else {  /*-- 同樣來源裝置，依照不同來源比較不同的部位 --*/
                if (dataA.Device == Devices.ADEPT_ACE) {
                    /* Adept ACE: 比較 I/O 大小 */
                    return dataA.IONum.CompareTo(dataB.IONum);
                } else if (dataA.Device == Devices.BECKHOFF_PLC || dataA.Device == Devices.DELTA) {
                    /* Beckhoff | Delta: 先比較是不是都是 X 或 Y */
                    if (dataA.ID[0] == dataB.ID[0]) {
                        int ioA, ioB;
                        if (int.TryParse(dataA.ID.ToLower().Replace("x", "").Replace("y", ""), out ioA)
                            && int.TryParse(dataB.ID.ToLower().Replace("x", "").Replace("y", ""), out ioB))
                            return ioA.CompareTo(ioB);  //同樣的 X/Y 情況下，將後面的數字轉成 int 並直接比較大小
                        else return string.Compare(dataA.ID, dataB.ID);   //如果是 X/Y 之外的新成員，直接以字串比較

                        /* 如果 X/Y 不同，直接使用字串比較就可 */
                    } else return string.Compare(dataA.ID, dataB.ID);
                } else if (dataA.Device == Devices.WAGO) {
                    /* Wago: 先比較 Register 編號，如果相同再比較 Bit */
                    if (dataA.RegNum != dataB.RegNum)
                        return dataA.RegNum.CompareTo(dataB.RegNum);
                    else
                        return dataA.RegBit.CompareTo(dataB.RegBit);

                    /* 如果以上條件都失敗，直接回傳相等大小 */
                } else return 0;
            }
        }

        #endregion

    }
}
