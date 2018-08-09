using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CtItsParameter
{
	class RtfBuilder
	{
		#region Declaration - Fields
		public enum ansicpgN
		{
			SimplifiedChinese = 936,
			TraditionalChinese = 950
		}
		public enum charsetN
		{
			GB2312 = 134,
			Big5 = 136
		}

		public enum langN
		{
			ChineseTaiwan = 1028,
			ChinesePRC = 2052,
		}
		private ansicpgN _ansicpg = ansicpgN.TraditionalChinese;

		private charsetN _charset = charsetN.Big5;

		private langN _lang = langN.ChineseTaiwan;

		private string _defaultFont = "Microsoft JhengHei UI";

		private Color _defaultForeColor = Color.Black;

		private Color _defaultBackColor = Color.Gold;

		private int _fontIndex = 1;

		private int _colorIndex = 3;

		private Dictionary<string, int> _fontTable;

		private Dictionary<Color, int> _colorTable;

		private List<string> _rtfList;

		private float _defaultFontSize;

		#endregion

		#region Declaration - Properties
		private Encoding _encoding { get => Encoding.GetEncoding((int)_ansicpg); }

		private string _fcharset { get => $@"fcharset{ ((int)_charset).ToString()}"; }

		public string Header { get => $@"\rtf\asni\asnicpg{((int)_ansicpg).ToString()}\deff0"; }

		public string FontTable { get => FontTbl(); }

		public string ColorTable { get => ColorTbl(); }

		public string ViewStart { get => @"\viewkind4\uc4\pard"; }

		public string ViewEnd { get => @"\par"; }

		public string Lang { get => $@"lang{((int)_lang)}"; }

		public float FontHeigh { get => (new Font(_defaultFont, _defaultFontSize)).Height; }

		#endregion

		#region Function - Constructors
		public RtfBuilder()
		{
			_ansicpg = ansicpgN.TraditionalChinese;
			_charset = charsetN.Big5;
			_lang = langN.ChineseTaiwan;
			_fontTable = new Dictionary<string, int>();
			_fontTable.Add("標楷體", 0);
			_colorTable = new Dictionary<Color, int>();
			_colorTable.Add(Color.Black, 1);
			_colorTable.Add(Color.Blue, 2);
			_rtfList = new List<string>();
		}

		public RtfBuilder(Font font)
		{
			_fontTable = new Dictionary<string, int>();
			_fontTable.Add(font.Name, 0);
			_defaultFontSize = font.Size;
			_colorTable = new Dictionary<Color, int>();
			_colorTable.Add(_defaultForeColor, 1);
			_colorTable.Add(_defaultBackColor, 2);
			_rtfList = new List<string>();
		}
		public RtfBuilder(Font font, Color foreColor, Color backColor)
		{
			_fontTable = new Dictionary<string, int>();
			_fontTable.Add(font.Name, 0);
			_defaultFontSize = font.Size;
			_colorTable = new Dictionary<Color, int>();
			_defaultForeColor = foreColor;
			_defaultBackColor = backColor;
			_colorTable.Add(_defaultForeColor, 1);
			_colorTable.Add(_defaultBackColor, 2);
			_rtfList = new List<string>();
		}

		public RtfBuilder(Color foreColor, Color backColor)
		{
			_fontTable = new Dictionary<string, int>();
			_fontTable.Add(_defaultFont, 0);
			_defaultFontSize = 12;
			_colorTable = new Dictionary<Color, int>();
			_defaultForeColor = foreColor;
			_defaultBackColor = backColor;
			_colorTable.Add(_defaultForeColor, 1);
			_colorTable.Add(_defaultBackColor, 2);
			_rtfList = new List<string>();
		}

		public RtfBuilder(ansicpgN ansicpg, charsetN charset, langN lang)
		{
			_ansicpg = ansicpg;
			_charset = charset;
			_lang = lang;
			_fontTable = new Dictionary<string, int>();
			_fontTable.Add(_defaultFont, 0);
			_defaultFontSize = 12;
			_colorTable = new Dictionary<Color, int>();
			_colorTable.Add(_defaultForeColor, 1);
			_colorTable.Add(_defaultBackColor, 2);
			_rtfList = new List<string>();
		}

		public RtfBuilder(ansicpgN ansicpg, charsetN charset, langN lang, Font font)
		{
			_ansicpg = ansicpg;
			_charset = charset;
			_lang = lang;
			_fontTable = new Dictionary<string, int>();
			_fontTable.Add(font.Name, 0);
			_defaultFontSize = font.Size;
			_colorTable = new Dictionary<Color, int>();
			_colorTable.Add(_defaultForeColor, 1);
			_colorTable.Add(_defaultBackColor, 2);
			_rtfList = new List<string>();
		}

		public RtfBuilder(ansicpgN ansicpg, charsetN charset, langN lang, Font font, Color foreColor, Color backColor)
		{
			_ansicpg = ansicpg;
			_charset = charset;
			_lang = lang;
			_fontTable = new Dictionary<string, int>();
			_fontTable.Add(font.Name, 0);
			_defaultFontSize = font.Size;
			_colorTable = new Dictionary<Color, int>();
			_defaultForeColor = foreColor;
			_defaultBackColor = backColor;
			_colorTable.Add(_defaultForeColor, 1);
			_colorTable.Add(_defaultBackColor, 2);
			_rtfList = new List<string>();
		}

		public RtfBuilder(string text)
		{
			_ansicpg = ansicpgN.TraditionalChinese;
			_charset = charsetN.Big5;
			_lang = langN.ChineseTaiwan;
			_fontTable = new Dictionary<string, int>();
			_fontTable.Add(_defaultFont, 0);
			_defaultFontSize = 12;
			_colorTable = new Dictionary<Color, int>();
			_colorTable.Add(_defaultForeColor, 1);
			_colorTable.Add(_defaultBackColor, 2);
			_rtfList = new List<string>();
			Append(text);
		}
		public RtfBuilder(string text, Color fontcolor)
		{
			_ansicpg = ansicpgN.TraditionalChinese;
			_charset = charsetN.Big5;
			_lang = langN.ChineseTaiwan;
			_fontTable = new Dictionary<string, int>();
			_fontTable.Add(_defaultFont, 0);
			_defaultFontSize = 12;
			_colorTable = new Dictionary<Color, int>();
			_colorTable.Add(_defaultForeColor, 1);
			_colorTable.Add(_defaultBackColor, 2);
			_rtfList = new List<string>();
			Append(text,fontcolor);
		}
		public RtfBuilder(string text, Font font, Color fontcolor)
		{
			_ansicpg = ansicpgN.TraditionalChinese;
			_charset = charsetN.Big5;
			_lang = langN.ChineseTaiwan;
			_fontTable = new Dictionary<string, int>();
			_fontTable.Add(font.Name, 0);
			_defaultFontSize = font.Size;
			_colorTable = new Dictionary<Color, int>();
			_colorTable.Add(_defaultForeColor, 1);
			_colorTable.Add(_defaultBackColor, 2);
			_rtfList = new List<string>();
			Append(text, font, fontcolor);
		}
		#endregion

		#region Function - Public Methods
		public string ToRTF()
		{
			return "{" + Header + FontTable + ColorTable + ViewStart + String.Join("", _rtfList) + ViewEnd + "}";
		}
		public void Append(string text)
		{
			_rtfList.Add($@"\cf1\{Lang}\f0\fs24 " + text);
		}

		public void Append(string text, bool highlight = false)
		{
			if (highlight) _rtfList.Add($@"\cf0\{Lang}\f0\fs{_defaultFontSize * 2} " + text);
			else _rtfList.Add($@"\cf1\{Lang}\f0\fs{_defaultFontSize * 2}\highlight2 " + text);
		}

		public void Append(string text, Font font)
		{
			if (!_fontTable.ContainsKey(font.Name)) { _fontTable.Add(font.Name, _fontIndex); _fontIndex++; }
			int fN = _fontTable[font.Name];
			int fsN = (int)font.Size;
			_rtfList.Add($@"\cf1\{Lang}\f{fN}\fs{fsN * 2} " + text);
		}

		public void Append(string text, Color fontcolor)
		{
			if (!_colorTable.ContainsKey(fontcolor)) { _colorTable.Add(fontcolor, _colorIndex); _colorIndex++; }
			int cfN = _colorTable[fontcolor];
			_rtfList.Add($@"\cf{cfN}\{Lang}\f0\fs{_defaultFontSize*2} " + text);
		}

		public void Append(string text, Font font, Color fontcolor)
		{
			if (!_fontTable.ContainsKey(font.Name)) { _fontTable.Add(font.Name, _fontIndex); _fontIndex++; }
			if (!_colorTable.ContainsKey(fontcolor)) { _colorTable.Add(fontcolor, _colorIndex); _colorIndex++; }
			int fN = _fontTable[font.Name];
			int cfN = _colorTable[fontcolor];
			int fsN = (int)font.Size;
			_rtfList.Add($@"\cf{cfN}\{Lang}\f{fN}\fs{fsN * 2}\" + text);
		}

		public void Append(string text, Font font, Color fontcolor, Color higtlight)
		{
			if (!_fontTable.ContainsKey(font.Name)) { _fontTable.Add(font.Name, _fontIndex); _fontIndex++; }
			if (!_colorTable.ContainsKey(fontcolor)) { _colorTable.Add(fontcolor, _colorIndex); _colorIndex++; }
			if (!_colorTable.ContainsKey(higtlight)) { _colorTable.Add(higtlight, _colorIndex); _colorIndex++; }
			int fN = _fontTable[font.Name];
			int cfN = _colorTable[fontcolor];
			int highlightN = _colorTable[higtlight];
			int fsN = (int)font.Size;
			_rtfList.Add($@"\cf{cfN}\highlight{highlightN}\{Lang}\f{fN}\fs{fsN * 2}\clvertalc " + text);
		}

		public void Clear()
		{
			_rtfList.Clear();
		}
		#endregion

		#region Function - Private Methods
		private string FontTbl()
		{
			string rtf = string.Empty;

			if (_fontTable != null && _fontTable.Count > 0)
			{
				foreach (KeyValuePair<string, int> item in _fontTable)
				{
					string fontName = item.Key;
					rtf += "{" + $@"\f{item.Value}\fnil\{_fcharset} {FontToHex(fontName)}" + "}";
				}
			}
			return @"{\fonttbl" + rtf + "}";
		}

		private string ColorTbl()
		{
			string rtf = string.Empty;
			if (_colorTable != null && _colorTable.Count > 0)
			{
				foreach (KeyValuePair<Color, int> item in _colorTable)
				{
					rtf += ColorRGB(item.Key);
				}
			}
			return @"{\colortbl ;" + rtf + "}";
		}

		private string ColorRGB(Color color)
		{
			return $@"\red{color.R.ToString()}\green{color.G.ToString()}\blue{color.B.ToString()};";
		}

		private string FontToHex(string fontname)
		{
			var bf = _encoding.GetBytes(fontname);
			return @"\'" + BitConverter.ToString(bf).ToLower().Replace("-", @"\'");
		}
		#endregion
	}
}
