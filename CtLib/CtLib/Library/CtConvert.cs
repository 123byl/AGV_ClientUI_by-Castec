using CtLib.Module.Ultity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CtLib.Library {
    /// <summary>
    /// 物件類別轉換
    /// <para>包含了常見的類型轉換，如將 string "1" 轉為 int 的 1 </para>
    /// <para>方法簡易分類: C- 為型態轉換，如 <see cref="CtConvert.CBool"/>;  To- 為數值轉換，如 <see cref="CtConvert.ToHex(int, int)"/></para>
    /// </summary>
    /// <remarks>
    /// 型態轉換部分，可以直接透過 <see cref="System.Convert"/> 達成
    /// <para>但這邊要獨立出來的原因是要讓不常使用 C# 的人，只要 using CtLib.Library 也可快速使用</para>
    /// </remarks>
    public static class CtConvert {

        #region Version

        /// <summary>
        /// CtConvert 版本訊息
        /// <para>當前版本: 1.0.1 [2014/09/11]</para>
        /// </summary>
        /// <remarks><code>
        /// 1.0.0  Ahern [2014/07/14]
        ///     + 將 VB.net 中常用之轉換轉移至此
        ///     
        /// 1.0.1  Ahern [2014/09/11]
        ///     \ CStr加入支援陣列與泛型
        ///     
        /// 1.0.2  Ahern [2015/04/23]
        ///     \ CStr獨立出轉換List同時設定進制部分
        ///     
        /// 1.0.3  Ahern [2015/05/25]
        ///     \ CStr 改以 IEnumerable 轉換，並增加連接字符與數值格式
        ///     \ 部分 ConvertAll 改以直接 Lambda 表示    
        /// 
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 0, 3, "2015/05/25", "Ahern Kuo");

        #endregion

        #region Functions - Core

        #region Converter
        /// <summary>將物件數值轉為字串(String)</summary>
        /// <param name="value">欲轉換之物件</param>
        /// <param name="connector">如果是陣列或列舉，欲連接各元素的字串。如 "x" 可得到如 "9x8x7x6x5"</param>
        /// <param name="format">如果為 float 或 double，數字表示格式。如 "0.0#"、"0.##" 等</param>
        /// <returns>數值字串</returns>
        /// <example>
        /// 數值轉換
        /// <code>
        /// int intVal = 99;
        /// string strVal = CtConvert.CStr(intVal); //得到 "99"
        /// </code>
        /// 
        /// List 或 Array 轉換
        /// <code>
        /// List&lt;int&gt; lstVal = new List&lt;int&gt; { 99, 98, 97, 96 };
        /// string strVal = CtConvert.CStr(lstVal, " ◎ ", "0.0"); //得到 "99.0 ◎ 98.0 ◎ 97.0 ◎ 96.0"
        /// //format 參數請參考 MSDN 之 String Format
        /// </code>
        /// 
        /// 如果需轉換進制，請使用如 <see cref="CStr(List&lt;byte&gt;, NumericFormats, string)"/>
        /// </example>
        public static string CStr(object value, string connector = ", ", string format = "0.0##") {
            string strTemp = "";

            if (value is IEnumerable) {
                IEnumerable emb = value as IEnumerable;

                /*-- IEnumerable 沒有 ForEach 或是 from-in 可使用，僅能使用 foreach --*/
                List<string> strList = new List<string>();
                foreach (var item in emb)
                    strList.Add(string.Format("{0:" + format + "}", item));

                strTemp = string.Join(connector, strList.ToArray());
            } else strTemp = value.ToString();

            return strTemp;
        }

        /// <summary>將 List&lt;byte&gt; 轉換為對等的一維進制字串</summary>
        /// <param name="value">欲轉換的數值集合</param>
        /// <param name="baseFormat">進制</param>
        /// <param name="connector">欲連接各元素的字串。如 "x" 可得到如 "9x8x7x6x5"</param>
        /// <returns>一維字串</returns>
        /// <example><code>
        /// List&lt;byte&gt; temp = new List&lt;byte&gt; { 16, 255 };
        /// string strVal = CtConvert.CStr(temp); //得到 "10 FF"
        /// </code></example>
        public static string CStr(List<byte> value, NumericFormats baseFormat, string connector = " ") {
            List<string> data = value.ConvertAll(val => Convert.ToString(val, (int)baseFormat).PadLeft(2, '0').ToUpper());
            return string.Join(connector, data.ToArray());
        }

        /// <summary>將 List&lt;ushort&gt; 轉換為對等的一維進制字串</summary>
        /// <param name="value">欲轉換的數值集合</param>
        /// <param name="baseFormat">進制</param>
        /// <param name="connector">欲連接各元素的字串。如 "x" 可得到如 "9x8x7x6x5"</param>
        /// <returns>一維字串</returns>
        /// <example><code>
        /// List&lt;ushort&gt; temp = new List&lt;ushort&gt; { 16, 255 };
        /// string strVal = CtConvert.CStr(temp); //得到 "10 FF"
        /// </code></example>
        public static string CStr(List<ushort> value, NumericFormats baseFormat, string connector = " ") {
            List<string> data = value.ConvertAll(val => Convert.ToString(val, (int)baseFormat).PadLeft(2, '0').ToUpper());
            return string.Join(connector, data.ToArray());
        }

        /// <summary>將 List&lt;int&gt; 轉換為對等的一維進制字串</summary>
        /// <param name="value">欲轉換的數值集合</param>
        /// <param name="baseFormat">進制</param>
        /// <param name="connector">欲連接各元素的字串。如 "x" 可得到如 "9x8x7x6x5"</param>
        /// <returns>一維字串</returns>
        /// <example><code>
        /// List&lt;int&gt; temp = new List&lt;int&gt; { 16, 255 };
        /// string strVal = CtConvert.CStr(temp); //得到 "10 FF"
        /// </code></example>
        public static string CStr(List<int> value, NumericFormats baseFormat, string connector = " ") {
            List<string> data = value.ConvertAll(val => Convert.ToString(val, (int)baseFormat).PadLeft(2, '0').ToUpper());
            return string.Join(connector, data.ToArray());
        }

        /// <summary>將物件數值轉為布林值(Boolean)</summary>
        /// <param name="value">欲轉換之物件</param>
        /// <returns>布林值</returns>
        /// <example>
        /// int/byte/short... 等數值類型轉換成 bool  → 只有 0 為 false，其餘為 true
        /// <code>
        /// bool result1 = CtConvert.CBool(-2.9);   //得到 true
        /// bool result2 = CtConvert.CBool(0);      //得到 false
        /// bool result3 = CtConvert.CBool(315);    //得到 true
        /// </code>
        /// 
        /// string 轉為 bool  → <see cref="Boolean.FalseString"/> 與 <seealso cref="Boolean.TrueString"/>，只能轉換 "true" "True" "false" "False"
        /// <code>
        /// bool result1 = CtConvert.CBool("true");  //得到 true
        /// bool result2 = CtConvert.CBool("False"); //得到 false
        /// bool Result3 = CtConvert.CBool("0");     //失敗！跳 Exception，沒辦法轉換
        /// </code>
        /// </example>
        public static bool CBool(object value) {
            return Convert.ToBoolean(value);
        }

        /// <summary>將物件數值轉為8位元帶符號整數(SByte)</summary>
        /// <param name="value">欲轉換之物件</param>
        /// <returns>數值</returns>
        /// <example><code>
        /// sbyte sbytVal = CtConvert.CSByte("9");  //得到 9
        /// </code></example>
        public static sbyte CSByte(object value) {
            return Convert.ToSByte(value);
        }

        /// <summary>將物件數值轉為16位元帶符號整數(Short, Int16)</summary>
        /// <param name="value">欲轉換之物件</param>
        /// <returns>數值</returns>
        /// <example><code>
        /// short val = CtConvert.CShort("8");  //得到 8
        /// </code></example>
        public static short CShort(object value) {
            return Convert.ToInt16(value);
        }

        /// <summary>將物件數值轉為帶符號整數(Int, Int32)</summary>
        /// <param name="value">欲轉換之物件</param>
        /// <returns>數值</returns>
        /// <example><code>
        /// int val = CtConvert.CInt("99"); //得到 99
        /// </code></example>
        public static int CInt(object value) {
            return Convert.ToInt32(value);
        }

        /// <summary>將物件數值轉為帶符號整數(Long, Int64)</summary>
        /// <param name="value">欲轉換之物件</param>
        /// <returns>數值</returns>
        /// <example><code>
        /// long val = CtConvert.CInt("85"); //得到 85
        /// </code></example>
        public static long CLong(object value) {
            return Convert.ToInt64(value);
        }

        /// <summary>將物件數值轉為8位元正整數(Byte)</summary>
        /// <param name="value">欲轉換之物件</param>
        /// <returns>數值</returns>
        /// <example><code>
        /// byte val = CtConvert.CByte("255"); //得到 255
        /// </code></example>
        public static byte CByte(object value) {
            return Convert.ToByte(value);
        }

        /// <summary>將物件數值轉為16位元正整數(UShort, UInt16)</summary>
        /// <param name="value">欲轉換之物件</param>
        /// <returns>數值</returns>
        /// <example><code>
        /// ushort val = CtConvert.CUShort("1278"); //得到 1278
        /// </code></example>
        public static ushort CUShort(object value) {
            return Convert.ToUInt16(value);
        }

        /// <summary>將物件數值轉為無符號整數(UInt, UInt32)</summary>
        /// <param name="value">欲轉換之物件</param>
        /// <returns>數值</returns>
        /// <example><code>
        /// uint val = CtConvert.CUInt("1"); //得到 1
        /// </code></example>
        public static uint CUInt(object value) {
            return Convert.ToUInt32(value);
        }

        /// <summary>將物件數值轉為無符號整數(ULong, UInt64)</summary>
        /// <param name="value">欲轉換之物件</param>
        /// <returns>數值</returns>
        /// <example><code>
        /// ulong val = CtConvert.CULong("8765"); //得到 8765
        /// </code></example>
        public static ulong CULong(object value) {
            return Convert.ToUInt64(value);
        }

        /// <summary>將物件數值轉為單精度浮點數(Float, Single)</summary>
        /// <param name="value">欲轉換之物件</param>
        /// <returns>數值</returns>
        /// <example><code>
        /// float val = CtConvert.CFloat("99.98"); //得到 99.98F
        /// </code></example>
        public static float CFloat(object value) {
            return Convert.ToSingle(value);
        }

        /// <summary>將物件數值轉為倍精度浮點數(Double)</summary>
        /// <param name="value">欲轉換之物件</param>
        /// <returns>數值</returns>
        /// <example><code>
        /// double val = CtConvert.CDbl("681.375"); //得到 681.375
        /// </code></example>
        public static double CDbl(object value) {
            return Convert.ToDouble(value);
        }

        /// <summary>將時間字串轉換為 DateTime 物件</summary>
        /// <param name="date">時間格式。如 "1999/12/31"、"2099/12/31 23:58:57"、"0123/1/1 2:54:13 PM"</param>
        /// <returns>DateTime 物件</returns>
        public static DateTime CDateTime(string date) {
            return DateTime.Parse(date);
        }

        /// <summary>轉換類型</summary>
        /// <param name="obj">欲轉換之物件</param>
        /// <param name="typ">欲轉換之型別</param>
        /// <returns>轉換完成之物件</returns>
        /// <example><code>
        /// UniversalClass otherObj;
        /// MyClass myObj = CtConvert.CType(otherObj, typeof(MyClass)); //將 otherObj 轉換為 MyClass 物件
        /// </code></example>
        public static object CType(object obj, Type typ) {
            return Convert.ChangeType(obj, typ);
        }

        /// <summary>
        /// 轉換類型，將物件轉換為 out 物件之類型
        /// <para>此方法無法用於轉換 string，請使用 <see cref="CStr(object,string,string)"/> 或直接 <seealso cref="string.ToString()"/></para>
        /// </summary>
        /// <typeparam name="T">欲轉換之類型，不可為 string</typeparam>
        /// <param name="srcObj">欲轉換之物件</param>
        /// <param name="tarObj">欲存放轉換完成之物件</param>
        public static void CType<T>(object srcObj, out T tarObj) {
            tarObj = (T)srcObj;
        }
        #endregion

        #region Type
        /// <summary>取得該物件類別(Type)</summary>
        /// <param name="obj">欲取得類別的物件</param>
        /// <returns>該物件類別(Type)</returns>
        /// <example><code>
        /// Type typeObj = CtConvert.GetType((byte)99);   //得到 byte
        /// </code></example>
        public static Type GetType(object obj) {
            return obj.GetType();
        }
        #endregion

        #region Convert Numeric Format
        /// <summary>將 ASCII 轉為字元。如輸入 65 或 0x41 將得到 "A" </summary>
        /// <param name="value">欲轉換之ASCII</param>
        /// <returns>相對應的字元</returns>
        /// <example><code>
        /// string asciiStr0 = CtConvert.ASCII(30);  //得到數字 0
        /// string asciiStrD = CtConvert.ASCII(68);  //得到大寫 D
        /// </code></example>
        /// <remarks>ASCII 對照表請參考 "http://zh.wikipedia.org/wiki/ASCII"</remarks>
        public static string ASCII(int value) {
            char asc = Convert.ToChar(value);
            return asc.ToString();
        }

        /// <summary>將字元轉為 ASCII 。如 'A' 將回傳 65 </summary>
        /// <param name="value">欲轉換之字元</param>
        /// <returns>ASCII</returns>
        /// <example><code>
        /// int asciiVal = CtConvert.ASCII('W');    //得到 87
        /// </code></example>
        public static int ASCII(char value) {
            return Convert.ToInt32(value);
        }

        /// <summary>將整數轉為二進位字串。如輸入 9 將得到 "1001"</summary>
        /// <param name="value">欲轉換之數值</param>
        /// <returns>二進位字串</returns>
        /// <example><code>
        /// string bnrStr1 = CtConvert.ToBinary(77);    //得到 "1001101"
        /// string bnrStr2 = CtConvert.ToBinary(166);   //得到 "10100110"
        /// </code></example>
        public static string ToBinary(int value) {
            return Convert.ToString(value, 2);
        }

        /// <summary>
        /// 將整數轉為二進位字串，並於左方自動補滿 "0" 至特定位數
        /// <para>如輸入 (9, 8) 將得到 "00001001"</para>
        /// </summary>
        /// <param name="value">欲轉換之數值</param>
        /// <param name="padLeft">總共需補滿至多少位數</param>
        /// <returns>二進位字串</returns>
        /// <example><code>
        /// string bnrStr1 = CtConvert.ToBinary(77, 8);  //得到 "01001101"
        /// string bnrStr2 = CtConvert.ToBinary(6, 8);   //得到 "00000110"
        /// string bnrStr3 = CtConvert.ToBinary(6, 1);   //失敗！跳 Exception
        /// </code></example>
        public static string ToBinary(int value, int padLeft) {
            string strTemp = Convert.ToString(value, 2);
            if (padLeft > 0) strTemp = strTemp.PadLeft(padLeft, '0');
            return strTemp;
        }

        /// <summary>將整數轉為二進位字串，將自動補滿至特定進制之位數。如 (2, HEXADECIMAL) 原為 "10" 將補滿為 "0010" </summary>
        /// <param name="value">欲轉換之數值</param>
        /// <param name="srcFormat">欲補滿的進制</param>
        /// <returns></returns>
        /// <example><code>
        /// string bnrStr1 = CtConvert.ToBinary(5, Formats.OCTAL);         //得到 "0101"
        /// string bnrStr2 = CtConvert.ToBinary(5, Formats.HEXADECIMAL);   //得到 "00000101"
        /// </code></example>
        public static string ToBinary(int value, NumericFormats srcFormat) {
            string strTemp = Convert.ToString(value, 2);
            switch (srcFormat) {
                case NumericFormats.OCTAL:
                    strTemp = strTemp.PadLeft(Convert.ToString(value, 8).Length * 4, '0');
                    break;
                case NumericFormats.HEXADECIMAL:
                    strTemp = strTemp.PadLeft(Convert.ToString(value, 16).Length * 4, '0');
                    break;
            }
            return strTemp;
        }

        /// <summary>將整數轉為十六進位字串。如輸入 15 可得 "F"</summary>
        /// <param name="value">欲轉換之數值</param>
        /// <param name="padLeft">是否補滿0</param>
        /// <returns>十六進位字串</returns>
        /// <example><code>
        /// string hexStr1 = CtConvert.ToHex(99);           //得到 "63"
        /// string hexStr2 = CtConvert.ToHex(10, true);     //得到 "0A"
        /// </code></example>
        public static string ToHex(int value, bool padLeft = true) {
            string strTemp = Convert.ToString(value, 16).ToUpper();
            if (padLeft && (strTemp.Length % 2) > 0) strTemp = "0" + strTemp;
            return strTemp;
        }

        /// <summary>將整數主為十六進位字串，但將自動補滿至特定位數。如輸入 (15, 4) 可得 "000F"</summary>
        /// <param name="value">欲轉換之數值</param>
        /// <param name="padLeft">左方欲補滿的位數</param>
        /// <returns>十六進位字串</returns>
        /// <example><code>
        /// string hexStr = CtConvert.ToHex(10, 8);     //得到 "0000000A"
        /// </code></example>
        public static string ToHex(int value, int padLeft) {
            string strTemp = Convert.ToString(value, 16).ToUpper();
            if (padLeft > 0) strTemp = strTemp.PadLeft(padLeft, '0');
            return strTemp;
        }

        /// <summary>將字串轉為對應的正整數。如 ("A", 16) = 0x0A = 10, ("6E", 16) = 0x6E = 110</summary>
        /// <param name="value">欲轉換之數值</param>
        /// <param name="baseFormat">進制格式。如 (BINARY)2, (OCTAL)8, (DECIMAL)10, (HEXADECIMAL)16</param>
        /// <returns>對應的正整數</returns>
        /// <example>
        /// 將十六進制之字串轉為 int
        /// <code>
        /// int val = CtConvert.ToInteger("9AC", NumericFormats.HEXADECIMAL);   //得到 2476
        /// </code>
        /// 
        /// 將二進制之字串轉為 int
        /// <code>
        /// int val = CtConvert.ToInteger("10010111", NumericFormats.BINARY);   //得到 151
        /// </code>
        /// </example>
        public static int ToInteger(string value, NumericFormats baseFormat) {
            return Convert.ToInt32(value, (int)baseFormat);
        }
        #endregion

        #region Checking
        /// <summary>檢查字串是否為數字，以十進位判斷。如果是十六進位請直接使用 <see cref="ToInteger"/> 判斷</summary>
        /// <param name="value">欲判斷的數字</param>
        /// <returns>(True)數字 (False)非數字</returns>
        public static bool IsNumeric(string value) {
            decimal dec;
            if (decimal.TryParse(value, out dec)) return true;
            else return false;
        }
        #endregion

        #endregion
    }
}
