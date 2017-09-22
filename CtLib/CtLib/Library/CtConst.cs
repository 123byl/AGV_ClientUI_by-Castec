using CtLib.Module.Ultity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CtLib.Library {

    /// <summary>常數集合</summary>
    public static class CtConst {

        #region Version

        /// <summary>CtConst 版本訊息</summary>
        /// <remarks><code>
        /// 0.0.0  Chi Sha [2007/02/20]
        ///     + Const
        ///     
        /// 1.0.0  Ahern [2014/07/16]
        ///     + 從舊版CtLib搬移
        ///     
        /// 1.0.1  Ahern [2014/09/13]
        ///     + Random Generator
        ///     
        /// 1.0.2  Ahern [2014/09/16]
        ///     \ 將const使用static readonly取代
        ///     
        /// 1.0.3  Ahern [2015/02/11]
        ///     - 將預設資料夾路徑部分移除，各程式改以 Resource 路徑為主
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 0, 3, "2015/02/11", "Ahern Kuo");

        #endregion

        #region File Name

        /// <summary>一般操作紀錄檔檔名</summary>
        public const string FILE_TRACELOG = "Trace.log";
        /// <summary>錯誤、警告等紀錄檔檔名</summary>
        public const string FILE_ALARMLOG = "Alarm.log";
        /// <summary>CAMPro 系統錯誤紀錄檔檔名</summary>
        public const string FILE_REPORTLOG = "Report.log";

        #endregion

        #region Char
        /// <summary>Delimiters for Opcode using. '[' ']' '='</summary>
        public static readonly char[] CHR_OPCODE = new char[] { '[', ']', '=' };
        /// <summary>Delimiters for command arguments. ',' ' ' 'Chr(9)'</summary>
        public static readonly char[] CHR_DELIMITERS = new char[] { ',', ' ', '\x09' };
        /// <summary>Delimiter for command arguments. ';'</summary>
        public static readonly char[] CHR_CMDARG = new char[] { ';' };
        /// <summary>Delimiter for comments. '/' ';'</summary>
        public static readonly char[] CHR_COMMENTS = new char[] { '/', ';' };
        /// <summary>Delimiter for command line port index. '@'</summary>
        public static readonly char[] CHR_PORT = new char[] { '@' };
        /// <summary>For space and tab specifically. ' ' 'Chr(9)'</summary>
        public static readonly char[] CHR_WSPACES = new char[] { ' ', '\x09' };
        /// <summary>For CR and LF specifically. '\n' '\r'</summary>
        public static readonly char[] CHR_CRLF = new char[] { '\n', '\r' };
        /// <summary>Left parenthesis. '(' '['</summary>
        public static readonly char[] CHR_LEFT_PAREN = new char[] { '(', '[' };
        /// <summary>Right parenthesis. ')' ']'</summary>
        public static readonly char[] CHR_RIGHT_PAREN = new char[] { ')', ']' };
        /// <summary>Left/right brackets. '[' ']'</summary>
        public static readonly char[] CHR_BRACKET = new char[] { '[', ']' };
        /// <summary>Left/right parenthesis. '(' ')'</summary>
        public static readonly char[] CHR_PAREN = new char[] { '(', ')' };
        /// <summary>Delimiter for directory path. '/' '\'</summary>
        public static readonly char[] CHR_PATH = new char[] { '/', '\\' };
        /// <summary>Delimiter for time stamp. '/' ',' ':' ' '</summary>
        public static readonly char[] CHR_TIME = new char[] { '/', ',', ':', ' ' };
        /// <summary>Delimiters for Hiwin response data. '>'</summary>
        public static readonly char[] CHR_HIWIN_DL = new char[] { '>' };
        /// <summary>Delimiters for normal seperator ' ',','</summary>
        public static readonly char[] CHR_SEPERATOR = new char[] { ' ',',' };

        /// <summary>字元「,」</summary>
        public static readonly char[] CHR_COMMA = new char[] { ',' };
        /// <summary>字元「.」</summary>
        public static readonly char[] CHR_PERIOD = new char[] { '.' };
        /// <summary>字元「=」</summary>
        public static readonly char[] CHR_EQUAL = new char[] { '=' };
        /// <summary>字元「-」</summary>
        public static readonly char[] CHR_DASH = new char[] { '-' };
        /// <summary>字元「:」</summary>
        public static readonly char[] CHR_COLON = new char[] { ':' };
        /// <summary>字元「_」</summary>
        public static readonly char[] CHR_UNDERSCORE = new char[] { '_' };
        /// <summary>字元「*」</summary>
        public static readonly char[] CHR_STAR = new char[] { '*' };
        /// <summary>字元「[」</summary>
        public static readonly char[] CHR_LBRACKET = new char[] { '[' };
        /// <summary>字元「]」</summary>
        public static readonly char[] CHR_RBRACKET = new char[] { ']' };
        /// <summary>字元「(」</summary>
        public static readonly char[] CHR_LPAREN = new char[] { '(' };
        /// <summary>字元「)」</summary>
        public static readonly char[] CHR_RPAREN = new char[] { ')' };
        /// <summary>字元「×」</summary>
        public static readonly char[] CHR_TIMES = new char[] { '×' };
        /// <summary>字元「 」</summary>
        public static readonly char[] CHR_SPACE = new char[] { ' ' };

        #endregion

        #region String
        /// <summary>換行符號與其相對應字串</summary>
        public static readonly Dictionary<EndChar, string> END_OF_CHAR = new Dictionary<EndChar,string>{
            {EndChar.NONE , ""},
            {EndChar.CR , "\r"},
            {EndChar.LF , "\n"},
            {EndChar.CRLF , Environment.NewLine},
        };
        /// <summary>Windows 新行，即 CrLf 或 \r\n</summary>
        public static readonly string NewLine = Environment.NewLine;
        /// <summary>新行，等同 Enter 鍵。即 Cr 或 \r 或 0x0D 或 U+000D</summary>
        public static readonly string Cr = "\r";
        /// <summary>換行。即 Lf 或 \n 或 0x0A 或 U+000A</summary>
        public static readonly string Lf = "\n";
        #endregion

        #region Random Generator
        /// <summary>產生範圍於 0 ~ 2,147,483,6747 之正整數亂數</summary>
        /// <returns>亂數</returns>
        /// <example><code>
        /// int rndVal = RandomInt();
        /// MessageBox.Show("取得隨機亂數: " + rndVal.ToString());
        /// </code></example>
        public static int RandomInt() {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            return rnd.Next();
        }

        /// <summary>產生範圍於 0 ~ max 之正整數亂數</summary>
        /// <returns>亂數</returns>
        /// <example>
        /// 取得 0 ~ 100 之亂數
        /// <code>
        /// int rndVal = RandomInt(100);
        /// MessageBox.Show("彩卷號碼: " + rndVal.ToString());
        /// </code>
        /// 取得 1 ~ 100 之亂數
        /// <code>
        /// int rndVal = RandomInt(99) + 1;
        /// MessageBox.Show("中獎員工工號: " + rndVal.ToString());
        /// </code>
        /// </example>
        public static int RandomInt(int max) {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            return rnd.Next(max + 1);
        }

        /// <summary>產生範圍於 min ~ max 之正整數亂數</summary>
        /// <returns>亂數</returns>
        /// <example><code>
        /// int rndVal = RandomInt(1, 100); //取得 1~100 之亂數
        /// </code></example>
        public static int RandomInt(int min, int max) {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            return rnd.Next(min, max + 1);
        }

        /// <summary>產生範圍於 0.0 ~ 1.0 之倍精度浮點數亂數</summary>
        /// <returns>亂數</returns>
        /// <example>
        /// 取得 0 ~ 1 之小數點亂數
        /// <code>
        /// double rndVal = RandomDouble();
        /// </code>
        /// 取得 0 ~ 100 之亂數
        /// <code>
        /// int rndVal = (int)(RandomDouble() * 100);
        /// </code>
        /// </example>
        public static double RandomDouble() {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            return rnd.NextDouble();
        }

        /// <summary>產生範圍於 0 ~ max 之倍精度浮點數亂數</summary>
        /// <returns>亂數</returns>
        /// <example><code>
        /// double rndVal = RandomDouble(99.9F); //取得 0 ~ 99.9 亂數
        /// </code></example>
        public static double RandomDouble(double max) {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            return rnd.NextDouble() * max;
        }

        /// <summary>產生範圍於 min ~ max 之倍精度浮點數亂數</summary>
        /// <returns>亂數</returns>
        /// <example><code>
        /// double rndVal = RandomDouble(0.2F, 89.9F);  //取得 0.2 ~ 89.9 之亂數
        /// </code></example>
        public static double RandomDouble(double min, double max) {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            return min + rnd.NextDouble() * (max - min);
        }
        #endregion
    }
}
