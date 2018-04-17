using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

using CtLib.Module.Utility;

namespace CtLib.Library {
	/// <summary>
	/// 物件類別轉換
	/// <para>包含了常見的類型轉換，如將 string "1" 轉為 int 的 1 </para>
	/// <para>方法簡易分類: C- 為型態轉換，如 <see cref="CBool"/>;  To- 為數值轉換，如 <see cref="ToHex(int, int)"/></para>
	/// </summary>
	/// <remarks>
	/// 型態轉換部分，可以直接透過 <see cref="Convert"/> 達成
	/// <para>但這邊要獨立出來的原因是要讓不常使用 C# 的人，只要 using CtLib.Library 也可快速使用</para>
	/// </remarks>
	public static class CtConvert {

		#region Version

		/// <summary>
		/// CtConvert 版本訊息
		/// <para>當前版本: 1.0.5 [2016/04/11]</para>
		/// </summary>
		/// <remarks><code language="C#">
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
		/// 1.0.4  Ahern [2015/06/10]
		///     + ToBoolSequence
		///     + ToNumericSequence
		///     
		/// 1.0.5  Ahern [2016/04/11]
		///		\ ToBoolSequence，修正功能並直接套用泛型
		///     \ ToNumericSequence，修正功能並直接套用泛型
		///     
		/// 1.0.6  Ahern [2016/05/24]
		///		+ String Extension (實驗用)
		/// 
		/// </code></remarks>
		public static CtVersion Version { get { return new CtVersion(1, 0, 6, "2016/05/24", "Ahern Kuo"); } }

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
		/// <code language="C#">
		/// int intVal = 99;
		/// string strVal = CtConvert.CStr(intVal); //得到 "99"
		/// </code>
		/// 
		/// List 或 Array 轉換
		/// <code language="C#">
		/// List&lt;int&gt; lstVal = new List&lt;int&gt; { 99, 98, 97, 96 };
		/// string strVal = CtConvert.CStr(lstVal, " ◎ ", "0.0"); //得到 "99.0 ◎ 98.0 ◎ 97.0 ◎ 96.0"
		/// //format 參數請參考 MSDN 之 String Format
		/// </code>
		/// 
		/// 如果需轉換進制，請使用如 <see cref="CStr(IEnumerable{int}, NumericFormats, string)"/>
		/// </example>
		public static string CStr(object value, string connector = ", ", string format = "0.0##") {
			string strTemp = "";

			if (value is string) strTemp = value as string;
			else if (value is IEnumerable) {
				IEnumerable emb = value as IEnumerable;

				/*-- IEnumerable 沒有 ForEach 或是 from-in 可使用，僅能使用 foreach --*/
				List<string> strList = new List<string>();
				foreach (var item in emb)
					strList.Add(string.Format("{0:" + format + "}", item));

				strTemp = string.Join(connector, strList);
			} else strTemp = value.ToString();

			return strTemp;
		}

		/// <summary>將 List&lt;byte&gt; 轉換為對等的一維進制字串</summary>
		/// <param name="value">欲轉換的數值集合</param>
		/// <param name="baseFormat">進制</param>
		/// <param name="connector">欲連接各元素的字串。如 "x" 可得到如 "9x8x7x6x5"</param>
		/// <returns>一維字串</returns>
		/// <example><code language="C#">
		/// List&lt;byte&gt; temp = new List&lt;byte&gt; { 16, 255 };
		/// string strVal = CtConvert.CStr(temp); //得到 "10 FF"
		/// </code></example>
		public static string CStr(IEnumerable<byte> value, NumericFormats baseFormat, string connector = " ") {
			IEnumerable<string> data = value.Select(val => Convert.ToString(val, (int)baseFormat).PadLeft(2, '0').ToUpper());
			return string.Join(connector, data);
		}

		/// <summary>將 List&lt;ushort&gt; 轉換為對等的一維進制字串</summary>
		/// <param name="value">欲轉換的數值集合</param>
		/// <param name="baseFormat">進制</param>
		/// <param name="connector">欲連接各元素的字串。如 "x" 可得到如 "9x8x7x6x5"</param>
		/// <returns>一維字串</returns>
		/// <example><code language="C#">
		/// List&lt;ushort&gt; temp = new List&lt;ushort&gt; { 16, 255 };
		/// string strVal = CtConvert.CStr(temp); //得到 "10 FF"
		/// </code></example>
		public static string CStr(IEnumerable<ushort> value, NumericFormats baseFormat, string connector = " ") {
			IEnumerable<string> data = value.Select(val => Convert.ToString(val, (int)baseFormat).PadLeft(2, '0').ToUpper());
			return string.Join(connector, data);
		}

		/// <summary>將 List&lt;int&gt; 轉換為對等的一維進制字串</summary>
		/// <param name="value">欲轉換的數值集合</param>
		/// <param name="baseFormat">進制</param>
		/// <param name="connector">欲連接各元素的字串。如 "x" 可得到如 "9x8x7x6x5"</param>
		/// <returns>一維字串</returns>
		/// <example><code language="C#">
		/// List&lt;int&gt; temp = new List&lt;int&gt; { 16, 255 };
		/// string strVal = CtConvert.CStr(temp); //得到 "10 FF"
		/// </code></example>
		public static string CStr(IEnumerable<int> value, NumericFormats baseFormat, string connector = " ") {
			IEnumerable<string> data = value.Select(val => Convert.ToString(val, (int)baseFormat).PadLeft(2, '0').ToUpper());
			return string.Join(connector, data);
		}

		/// <summary>將物件數值轉為布林值(Boolean)</summary>
		/// <param name="value">欲轉換之物件</param>
		/// <returns>布林值</returns>
		/// <example>
		/// int/byte/short... 等數值類型轉換成 bool  → 只有 0 為 false，其餘為 true
		/// <code language="C#">
		/// bool result1 = CtConvert.CBool(-2.9);   //得到 true
		/// bool result2 = CtConvert.CBool(0);      //得到 false
		/// bool result3 = CtConvert.CBool(315);    //得到 true
		/// </code>
		/// 
		/// string 轉為 bool  → <see cref="Boolean.FalseString"/> 與 <seealso cref="Boolean.TrueString"/>，只能轉換 "true" "True" "false" "False"
		/// <code language="C#">
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
		/// <example><code language="C#">
		/// sbyte sbytVal = CtConvert.CSByte("9");  //得到 9
		/// </code></example>
		public static sbyte CSByte(object value) {
			return Convert.ToSByte(value);
		}

		/// <summary>將物件數值轉為16位元帶符號整數(Short, Int16)</summary>
		/// <param name="value">欲轉換之物件</param>
		/// <returns>數值</returns>
		/// <example><code language="C#">
		/// short val = CtConvert.CShort("8");  //得到 8
		/// </code></example>
		public static short CShort(object value) {
			return Convert.ToInt16(value);
		}

		/// <summary>將物件數值轉為帶符號整數(Int, Int32)</summary>
		/// <param name="value">欲轉換之物件</param>
		/// <returns>數值</returns>
		/// <example><code language="C#">
		/// int val = CtConvert.CInt("99"); //得到 99
		/// </code></example>
		public static int CInt(object value) {
			return Convert.ToInt32(value);
		}

		/// <summary>將物件數值轉為帶符號整數(Long, Int64)</summary>
		/// <param name="value">欲轉換之物件</param>
		/// <returns>數值</returns>
		/// <example><code language="C#">
		/// long val = CtConvert.CInt("85"); //得到 85
		/// </code></example>
		public static long CLong(object value) {
			return Convert.ToInt64(value);
		}

		/// <summary>將物件數值轉為8位元正整數(Byte)</summary>
		/// <param name="value">欲轉換之物件</param>
		/// <returns>數值</returns>
		/// <example><code language="C#">
		/// byte val = CtConvert.CByte("255"); //得到 255
		/// </code></example>
		public static byte CByte(object value) {
			return Convert.ToByte(value);
		}

		/// <summary>將物件數值轉為16位元正整數(UShort, UInt16)</summary>
		/// <param name="value">欲轉換之物件</param>
		/// <returns>數值</returns>
		/// <example><code language="C#">
		/// ushort val = CtConvert.CUShort("1278"); //得到 1278
		/// </code></example>
		public static ushort CUShort(object value) {
			return Convert.ToUInt16(value);
		}

		/// <summary>將物件數值轉為無符號整數(UInt, UInt32)</summary>
		/// <param name="value">欲轉換之物件</param>
		/// <returns>數值</returns>
		/// <example><code language="C#">
		/// uint val = CtConvert.CUInt("1"); //得到 1
		/// </code></example>
		public static uint CUInt(object value) {
			return Convert.ToUInt32(value);
		}

		/// <summary>將物件數值轉為無符號整數(ULong, UInt64)</summary>
		/// <param name="value">欲轉換之物件</param>
		/// <returns>數值</returns>
		/// <example><code language="C#">
		/// ulong val = CtConvert.CULong("8765"); //得到 8765
		/// </code></example>
		public static ulong CULong(object value) {
			return Convert.ToUInt64(value);
		}

		/// <summary>將物件數值轉為單精度浮點數(Float, Single)</summary>
		/// <param name="value">欲轉換之物件</param>
		/// <returns>數值</returns>
		/// <example><code language="C#">
		/// float val = CtConvert.CFloat("99.98"); //得到 99.98F
		/// </code></example>
		public static float CFloat(object value) {
			return Convert.ToSingle(value);
		}

		/// <summary>將物件數值轉為倍精度浮點數(Double)</summary>
		/// <param name="value">欲轉換之物件</param>
		/// <returns>數值</returns>
		/// <example><code language="C#">
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
		/// <example><code language="C#">
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
			tarObj = (T)Convert.ChangeType(srcObj, typeof(T));
		}
		#endregion

		#region Type
		/// <summary>取得該物件類別(Type)</summary>
		/// <param name="obj">欲取得類別的物件</param>
		/// <returns>該物件類別(Type)</returns>
		/// <example><code language="C#">
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
		/// <example><code language="C#">
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
		/// <example><code language="C#">
		/// int asciiVal = CtConvert.ASCII('W');    //得到 87
		/// </code></example>
		public static int ASCII(char value) {
			return Convert.ToInt32(value);
		}

		/// <summary>將整數轉為二進位字串。如輸入 9 將得到 "1001"</summary>
		/// <param name="value">欲轉換之數值</param>
		/// <returns>二進位字串</returns>
		/// <example><code language="C#">
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
		/// <example><code language="C#">
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
		/// <returns>二進位字串</returns>
		/// <example><code language="C#">
		/// string bnrStr1 = CtConvert.ToBinary(5, Formats.OCTAL);         //得到 "0101"
		/// string bnrStr2 = CtConvert.ToBinary(5, Formats.HEXADECIMAL);   //得到 "00000101"
		/// </code></example>
		public static string ToBinary(int value, NumericFormats srcFormat) {
			string strTemp = Convert.ToString(value, 2);
			switch (srcFormat) {
				case NumericFormats.Octal:
					strTemp = strTemp.PadLeft(Convert.ToString(value, 8).Length * 4, '0');
					break;
				case NumericFormats.Hexadecimal:
					strTemp = strTemp.PadLeft(Convert.ToString(value, 16).Length * 4, '0');
					break;
			}
			return strTemp;
		}

		/// <summary>將整數轉為十六進位字串。如輸入 15 可得 "F"</summary>
		/// <param name="value">欲轉換之數值</param>
		/// <param name="padLeft">是否補滿0</param>
		/// <returns>十六進位字串</returns>
		/// <example><code language="C#">
		/// string hexStr1 = CtConvert.ToHex(99);           //得到 "63"
		/// string hexStr2 = CtConvert.ToHex(10, true);     //得到 "0A"
		/// </code></example>
		public static string ToHex(int value, bool padLeft = true) {
			string strTemp = Convert.ToString(value, 16).ToUpper();
			if (padLeft && (strTemp.Length % 2) > 0) strTemp = "0" + strTemp;
			return strTemp;
		}

		/// <summary>將 64Bit 整數轉為十六進位字串。如輸入 15 可得 "F"</summary>
		/// <param name="value">欲轉換之數值</param>
		/// <param name="padLeft">是否補滿0</param>
		/// <returns>十六進位字串</returns>
		/// <example><code language="C#">
		/// string hexStr1 = CtConvert.ToHex(99);           //得到 "63"
		/// string hexStr2 = CtConvert.ToHex(10, true);     //得到 "0A"
		/// </code></example>
		public static string ToHex(long value, bool padLeft = true) {
			string strTemp = Convert.ToString(value, 16).ToUpper();
			if (padLeft && (strTemp.Length % 2) > 0) strTemp = "0" + strTemp;
			return strTemp;
		}

		/// <summary>將整數轉為十六進位字串，但將自動補滿至特定位數。如輸入 (15, 4) 可得 "000F"</summary>
		/// <param name="value">欲轉換之數值</param>
		/// <param name="padLeft">左方欲補滿的位數</param>
		/// <returns>十六進位字串</returns>
		/// <example><code language="C#">
		/// string hexStr = CtConvert.ToHex(10, 8);     //得到 "0000000A"
		/// </code></example>
		public static string ToHex(int value, int padLeft) {
			string strTemp = Convert.ToString(value, 16).ToUpper();
			if (padLeft > 0) strTemp = strTemp.PadLeft(padLeft, '0');
			return strTemp;
		}

		/// <summary>將 64Bit 整數轉為十六進位字串，但將自動補滿至特定位數。如輸入 (15, 4) 可得 "000F"</summary>
		/// <param name="value">欲轉換之數值</param>
		/// <param name="padLeft">左方欲補滿的位數</param>
		/// <returns>十六進位字串</returns>
		/// <example><code language="C#">
		/// string hexStr = CtConvert.ToHex(10, 8);     //得到 "0000000A"
		/// </code></example>
		public static string ToHex(long value, int padLeft) {
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
		/// <code language="C#">
		/// int val = CtConvert.ToInteger("9AC", NumericFormats.Hexadecimal);   //得到 2476
		/// </code>
		/// 
		/// 將二進制之字串轉為 int
		/// <code language="C#">
		/// int val = CtConvert.ToInteger("10010111", NumericFormats.BINARY);   //得到 151
		/// </code>
		/// </example>
		public static int ToInteger(string value, NumericFormats baseFormat) {
			return Convert.ToInt32(value, (int)baseFormat);
		}
		#endregion

		#region Checking
		/// <summary>檢查字串是否為數字，以十進位判斷。如果是十六進位請直接使用 <see cref="ToInteger(string, NumericFormats)"/> 判斷</summary>
		/// <param name="value">欲判斷的數字</param>
		/// <returns>(<see langword="true"/>)數字 (<see langword="false"/>)非數字</returns>
		public static bool IsNumeric(string value) {
			decimal dec;
			if (decimal.TryParse(value, out dec)) return true;
			else return false;
		}
		#endregion

		#region Convert Serial Data (Bit) And Decimal Numeric

		#region List<T> → List<bool>
		/* ------------------------------------------------------------------------------------------
         * 使用泛型速度會慢3倍，uint 約 1200 Ticks (< 1 ms)，T 約 3500 Ticks (< 2ms)
         * 但為了簡潔有力，且實際消耗時間不多，故直接使用泛型
         * ------------------------------------------------------------------------------------------ */
		/// <summary>
		/// 將多筆數值 <see cref="List{T}"/> 轉換為 bool 數值 (List&lt;bool&gt;)
		/// <para>可用於如 Modbus，將各 bit 轉換成相對應的 bool</para>
		/// <para>例如 0x03 = 0011 = (IO:04)OFF (IO:03)OFF (IO:02)ON (IO:01)ON</para>
		/// </summary>
		/// <typeparam name="T">整數型態。如 <see cref="byte"/>, <see cref="short"/>, <see cref="int"/>, <see cref="long"/> 等等</typeparam>
		/// <param name="count">
		/// 最後轉換出來的所需位數。預設 -1 則表示忽略！
		/// <para>例如總共 28 個 I/O，但須 Ceiling(28/8) = 4 byte (4 * 8 = 32 bit，回傳 32 個 bool) 才可描述完</para>
		/// <para>如果此引數輸入 28，則最後將只回傳數量 28 個的 bool 陣列，免去裁減</para>
		/// </param>
		/// <param name="data">原始數值</param>
		/// <param name="bolVal">轉換後的 <see cref="List{T}"/>，T 為 <seealso cref="bool"/></param>
		public static void ToBoolSequence<T>(List<T> data, out List<bool> bolVal, int count = -1) where T : struct {
			int size = Marshal.SizeOf(typeof(T)) * 8;
			int maxCount = size;
			ulong valTemp = 0;
			List<bool> bRet = new List<bool>();
			for (int idx = 0; idx < data.Count; idx++) {
				valTemp = Convert.ToUInt64(data[idx]);
				if (count > -1) {
					if (count <= size) maxCount = count;
					else maxCount = ((idx * size) < count) ? size : count % size;
				}
				for (byte bit = 0; bit < maxCount; bit++) {
					bRet.Add((valTemp & 0x01) == 1);
					valTemp >>= 1;
				}
			}
			bolVal = bRet;
		}

		#endregion

		#region List<bool> → List<T>

		/* ------------------------------------------------------------------------------------------
         * 使用泛型速度會慢1倍，uint 約 1100 Ticks (< 1 ms)，T 約 3000 Ticks (< 2ms)
         * 為了擴充性及簡化 Code，故直接套用泛型
         * ------------------------------------------------------------------------------------------ */
		/// <summary>
		/// 將順向的 List(Of bool) 轉換為資料傳輸用的 List(Of T)
		/// <para>此方法並未將高低位元對調！詳情請看註解</para>
		/// </summary>
		/// <typeparam name="T">可轉換的資料型態，從 <see cref="byte"/> 至 <seealso cref="ulong"/></typeparam>
		/// <param name="bolVal">含有 <see cref="bool"/> 順序(遞增)的集合。如 [0]I/O 97  [1]I/O 98  [2]I/O 99  [3]I/O 100  [4]I/O 101</param>
		/// <param name="bytVal">資料傳輸格式，未將高低位元對調。如 [0..15]1010011001011001 = [bit 15..0]1001101001100101 = [out]0x9A65 </param>
		/// <remarks>
		/// 此方法未將高低位元對調！以 byte 為例
		/// <code language="C#">
		/// List&lt;bool&gt; data = new List&lt;bool&gt; { true,false,true,false,false,true,true,false,false,true,false,true,true,false,false,true };
		/// </code>
		/// data 或許對應 I/O 2001 至 2016 (遞增)
		/// <para>於閱讀順序(二進制)上來看，2001 為低位元，2016 為高位元，即</para>
		/// <code language="C#">[0..15]1010011001011001  --> 閱讀順序 --> [15..0]1001101001100101</code>
		/// 而 [15..0]1001101001100101 可表示為 0x9A65
		/// <para>如果傳入引數 [out]bytVal 之 T 為 byte，則將依序回傳 [0]0x9A [1]0x65</para>
		/// <para>於特定機台上，如 Wago 750-352 之 Modbus TCP 應用上，需要將高低位元對調，意即 [0]0x65 [1]0x9A</para>
		/// <para>表示需先傳送低位元，再傳送高位元... 因裝置需求不同，請依照實際需求再行進行下一步應用！</para>
		/// </remarks>
		public static void ToNumericSequence<T>(List<bool> bolVal, out List<T> bytVal) where T : struct {
			List<T> valTemp = new List<T>();
			int size = Marshal.SizeOf(typeof(T)) * 8 - 1;
			Type type = typeof(T);
			ulong tar = 0;

			byte idx = 0;
			foreach (bool val in bolVal) {
				if (val) tar |= (1UL << idx);
				if (idx > 0 && idx % size == 0) {
					valTemp.Add((T)Convert.ChangeType(tar, type));
					tar = 0;
					idx = 0;
				} else idx++;
			}

			if (idx != 0) valTemp.Add((T)Convert.ChangeType(tar, type));

			bytVal = valTemp;
		}


		#endregion

		#region IEnumerable<byte> → T
		/// <summary>將 <see cref="IEnumerable{T1}"/> 列舉值轉換成 <see cref="List{T2}"/> <para>T1 為 <see cref="byte"/>；T2 為 <see cref="ushort"/></para></summary>
		/// <param name="data">欲轉換的列舉值。如 List&lt;byte&gt; 、 byte[]</param>
		/// <param name="seq">閱讀順序或網路傳送順序</param>
		/// <returns>轉換完畢之數值</returns>
		public static List<ushort> ToUShort(IEnumerable<byte> data, BitSequence seq = BitSequence.Reading) {
			List<ushort> value = new List<ushort>();

			if (seq == BitSequence.Reading) {
				for (int idx = 0; idx < data.Count(); idx += 2) {
					value.Add((ushort)((data.ElementAt(idx) << 8) + data.ElementAt(idx + 1)));
				}
			} else {
				for (int idx = 0; idx < data.Count(); idx += 2) {
					value.Add((ushort)((data.ElementAt(idx + 1) << 8) + data.ElementAt(idx)));
				}
			}

			return value;
		}

		/// <summary>將 <see cref="IEnumerable{T1}"/> 列舉值轉換成 <see cref="List{T2}"/> <para>T1 為 <see cref="byte"/>；T2 為 <see cref="int"/></para></summary>
		/// <param name="data">欲轉換的列舉值。如 List&lt;byte&gt; 、 byte[]</param>
		/// <param name="seq">閱讀順序或網路傳送順序</param>
		/// <returns>轉換完畢之數值</returns>
		public static List<int> ToInteger(IEnumerable<byte> data, BitSequence seq = BitSequence.Reading) {
			List<int> value = new List<int>();

			if (seq == BitSequence.Reading) {
				for (int idx = 0; idx < data.Count(); idx += 4) {
					value.Add(
						(data.ElementAt(idx) << 24) + (data.ElementAt(idx + 1) << 16) +
						(data.ElementAt(idx + 2) << 8) + data.ElementAt(idx + 3)
					);
				}
			} else {
				for (int idx = 0; idx < data.Count(); idx += 4) {
					value.Add(
						(data.ElementAt(idx + 3) << 24) + (data.ElementAt(idx + 2) << 16) +
						(data.ElementAt(idx + 1) << 8) + data.ElementAt(0)
					);
				}
			}

			return value;
		}

		/// <summary>將 <see cref="IEnumerable{T1}"/> 列舉值轉換成 <see cref="List{T2}"/> <para>T1 為 <see cref="byte"/>；T2 為 <see cref="ushort"/></para></summary>
		/// <param name="data">欲轉換的列舉值。如 List&lt;byte&gt; 、 byte[]</param>
		/// <param name="startIndex">起始的列舉值索引</param>
		/// <param name="count">欲轉換的數量</param>
		/// <param name="seq">閱讀順序或網路傳送順序</param>
		/// <returns>轉換完畢之數值</returns>
		/// <remarks>如果數量少，建議直接使用 Bit Shift 的方式來執行，CPU 執行時間可大幅降低</remarks>
		public static List<ushort> ToUShort(IEnumerable<byte> data, int startIndex, int count, BitSequence seq = BitSequence.Reading) {
			List<ushort> value = new List<ushort>();

			if (seq == BitSequence.Reading) {
				for (int idx = startIndex; idx < startIndex + count; idx += 2) {
					value.Add((ushort)((data.ElementAt(idx) << 8) + data.ElementAt(idx + 1)));
				}
			} else {
				for (int idx = startIndex; idx < startIndex + count; idx += 2) {
					value.Add((ushort)((data.ElementAt(idx + 1) << 8) + data.ElementAt(idx)));
				}
			}

			return value;
		}

		/// <summary>將 <see cref="IEnumerable{T1}"/> 列舉值轉換成 <see cref="List{T2}"/> <para>T1 為 <see cref="byte"/>；T2 為 <see cref="int"/></para></summary>
		/// <param name="data">欲轉換的列舉值。如 List&lt;byte&gt; 、 byte[]</param>
		/// <param name="startIndex">起始的列舉值索引</param>
		/// <param name="count">欲轉換的數量</param>
		/// <param name="seq">閱讀順序或網路傳送順序</param>
		/// <returns>轉換完畢之數值</returns>
		/// <remarks>如果數量少，建議直接使用 Bit Shift 的方式來執行，CPU 執行時間可大幅降低</remarks>
		public static List<int> ToInteger(IEnumerable<byte> data, int startIndex, int count, BitSequence seq = BitSequence.Reading) {
			List<int> value = new List<int>();

			if (seq == BitSequence.Reading) {
				for (int idx = startIndex; idx < startIndex + count; idx += 4) {
					value.Add(
						(data.ElementAt(idx) << 24) + (data.ElementAt(idx + 1) << 16) +
						(data.ElementAt(idx + 2) << 8) + data.ElementAt(idx + 3)
					);
				}
			} else {
				for (int idx = startIndex; idx < startIndex + count; idx += 4) {
					value.Add(
						(data.ElementAt(idx + 3) << 24) + (data.ElementAt(idx + 2) << 16) +
						(data.ElementAt(idx + 1) << 8) + data.ElementAt(0)
					);
				}
			}

			return value;
		}
		#endregion

		#endregion

		#region Chinese Traditional / Chinese Simplified Translate

		#region Definition

		private static readonly int LOCALE_SYSTEM_DEFAULT = 0x0800;
		private static readonly uint LOCALE_NAME_SIMPLIFIED = 0x02000000;
		private static readonly uint LOCALE_NAME_TRADITIONAL = 0x04000000;

		#endregion

		#region Windows API

		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int LCMapString(int locale, int dwMapFlags, string lpSrcStr, int cchSrc, [Out] string lpDestStr, int cchDest);


		/// <summary>For a locale specified by name, maps an input character string to another using a specified transformation, or generates a sort key for the input string</summary>
		/// <param name="lpLocaleName">Pointer to a locale name, or one of the following predefined value</param>
		/// <param name="dwMapFlags">Flag specifying the type of transformation to use during string mapping or the type of sort key to generate</param>
		/// <param name="lpSrcStr">Pointer to a source string that the function maps or uses for sort key generation. This string cannot have a size of 0</param>
		/// <param name="cchSrc">Size, in characters, of the source string indicated by lpSrcStr</param>
		/// <param name="lpDestStr">Pointer to a buffer in which this function retrieves the mapped string or sort key</param>
		/// <param name="cchDest">Size, in characters, of the buffer indicated by lpDestStr</param>
		/// <param name="lpVersionInformation">Reserved; must be NULL</param>
		/// <param name="lpReserved">Reserved; must be NULL</param>
		/// <param name="sortHandle">Reserved; must be 0</param>
		/// <returns></returns>
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		static extern int LCMapStringEx(
							  string lpLocaleName,          //  LPCWSTR      lpLocaleName
							  uint dwMapFlags,              //  DWORD        dwMapFlags
							  string lpSrcStr,              //  LPCWSTR      lpSrcStr
							  int cchSrc,                   //  int          cchSrc
							  [Out]
							  IntPtr lpDestStr,             //  LPWSTR       lpDestStr
							  int cchDest,                  //  int          cchDest
							  IntPtr lpVersionInformation,  //  LPNLSVERSIONINFO lpVersionInformation
							  IntPtr lpReserved,            //  LPVOID       lpReserved
							  IntPtr sortHandle             //  LPARAM       sortHandle
		);

		#endregion

		#region Translate

		/// <summary>將繁體中文轉換為簡體中文</summary>
		/// <param name="argSource">欲轉換的繁體字串</param>
		/// <returns>相對應的簡體字串</returns>
		/// <remarks>
		/// 來源: http://jian-zhoung.blogspot.tw/2012/07/c.html 與 http://pinvoke.net/default.aspx/kernel32/LCMapStringEx.html 
		/// 目前於 Windows 10 測試時，使用 LCMapStringEx 有機會導致 app crash
		/// </remarks>
		public static string ToSimplified(string argSource) {
			/*-- Using LCMapStringEx --*/
			//string result = string.Empty;
			//int length = argSource.Length;
			//IntPtr ptr = Marshal.AllocHGlobal(length);
			//IntPtr zeroPtr = IntPtr.Zero;
			//try {
			//	LCMapStringEx("LOCALE_NAME_SYSTEM_DEFAULT", LOCALE_NAME_SIMPLIFIED, argSource, length, ptr, length, zeroPtr, zeroPtr, zeroPtr);
			//	result = Marshal.PtrToStringUni(ptr, length);
			//} finally {
			//	Marshal.FreeHGlobal(ptr);
			//}

			/*-- Using LCMapString --*/
			string result = new string(' ', argSource.Length);
			LCMapString(LOCALE_SYSTEM_DEFAULT, (int)LOCALE_NAME_SIMPLIFIED, argSource, argSource.Length, result, argSource.Length);

			return result;
		}

		/// <summary>將簡體中文轉換為繁體中文</summary>
		/// <param name="argSource">欲轉換的簡體字串</param>
		/// <returns>相對應的繁體字串</returns>
		/// <remarks>
		/// 來源: http://jian-zhoung.blogspot.tw/2012/07/c.html 與 http://pinvoke.net/default.aspx/kernel32/LCMapStringEx.html 
		/// 目前於 Windows 10 測試時，使用 LCMapStringEx 有機會導致 app crash
		/// </remarks>
		public static string ToTraditional(string argSource) {
			/*-- Using LCMapStringEx --*/
			//string result = string.Empty;
			//int length = argSource.Length;
			//IntPtr ptr = Marshal.AllocHGlobal(length);
			//IntPtr zeroPtr = IntPtr.Zero;
			//try {
			//	LCMapStringEx("LOCALE_NAME_SYSTEM_DEFAULT", LOCALE_NAME_TRADITIONAL, argSource, length, ptr, length, zeroPtr, zeroPtr, zeroPtr);
			//	result = Marshal.PtrToStringUni(ptr, length);
			//} finally {
			//	Marshal.FreeHGlobal(ptr);
			//}

			/*-- Using LCMapString --*/
			string result = new string(' ', argSource.Length);
			LCMapString(LOCALE_SYSTEM_DEFAULT, (int)LOCALE_NAME_TRADITIONAL, argSource, argSource.Length, result, argSource.Length);

			return result;
		}

		#endregion

		#endregion

		#endregion

		#region Functions - Extensions
		/// <summary>將 <see cref="string"/> 轉換為相對應的實值型態</summary>
		/// <typeparam name="TConvert">實值類別，如 <see cref="int"/>、<seealso cref="long"/>、<seealso cref="double"/> 等</typeparam>
		/// <param name="str">欲轉換的文字</param>
		/// <returns>相對應的實值類別</returns>
		/// <exception cref="ArgumentNullException">欲轉換的字串為空</exception>
		/// <exception cref="NotSupportedException">欲轉換的字串內容為不支援的格式</exception>
		public static TConvert Parse<TConvert>(this string str) where TConvert : struct {
			return (TConvert)TypeDescriptor.GetConverter(typeof(TConvert)).ConvertFromString(str);
		}

		/// <summary>嘗試將 <see cref="string"/> 轉換為相對應的實值型態</summary>
		/// <typeparam name="TConvert">實值類別，如 <see cref="int"/>、<seealso cref="long"/>、<seealso cref="double"/> 等</typeparam>
		/// <param name="str">欲轉換的文字</param>
		/// <param name="target">欲轉換的實值類別</param>
		/// <returns>(<see langword="true"/>)轉換成功  (<see langword="false"/>)轉換失敗</returns>
		public static bool TryParse<TConvert>(this string str, out TConvert target) where TConvert : struct {
			bool converted = true;
			TConvert tempObj = default(TConvert);
			try {
				tempObj = (TConvert)TypeDescriptor.GetConverter(typeof(TConvert)).ConvertFromString(str);
			} catch (Exception ex) {
				converted = false;
				CtStatus.Report(Stat.ER_SYSTEM, ex);
			}
			target = tempObj;
			return converted;
		}
		#endregion
	}
}
