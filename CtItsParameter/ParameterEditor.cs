using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace CtItsParameter
{
	class ParameterEditor
	{
		#region Declaration -Fields
		internal const string ParameterWord = "Parameter";
		internal const string NameWord = "Name";
		internal const string TypeWord = "Type";
		internal const string DescriptionWord = "Description";
		internal const string ValueWord = "Value";
		internal const string DefaultWord = "Default";
		internal const string MaxWord = "Max";
		internal const string MinWord = "Min";

		internal const string SectionGroup = "section";
		internal const string ValuesGroup = "values";
		internal const string ValueGroup = "value";
		internal const string NameGroup = "name";

		private Dictionary<string, ParameterInfo> _parameters;

		private DataTable _parametertable;
		#endregion

		#region Declaration - Properties
		public Dictionary<string, ParameterInfo> Parameters { get => _parameters; }

		public DataTable ParameterTable { get => _parametertable; }
		#endregion
		#region Function - Constructors
		public ParameterEditor()
		{
			_parameters = new Dictionary<string, ParameterInfo>();
			_parametertable = new DataTable();
			_parametertable.Columns.Add(ParameterWord);
			_parametertable.Columns.Add(NameWord);
			_parametertable.Columns.Add(TypeWord);
			_parametertable.Columns.Add(DescriptionWord);
			_parametertable.Columns.Add(ValueWord);
			_parametertable.Columns.Add(DefaultWord);
			_parametertable.Columns.Add(MaxWord);
			_parametertable.Columns.Add(MinWord);
		}
		#endregion

		#region Function - Public Methods
		public void Read(string path)
		{
			var exist = File.Exists(path);
			var subname = Path.GetExtension(path);
			if (exist && subname == ".ini")
			{
				string content = File.ReadAllText(path);
				Analysis(content);
			}
		}
		#endregion

		#region Function - Private Methods
		private void Analysis(string content)
		{
			string section = $@"\[\s*(?'{SectionGroup}'[^\]\s]*)\s*\](?'{ValuesGroup}'[^\[]*)";
			MatchCollection matchs = new Regex(section).Matches(content);
			foreach (Match match in matchs)
			{
				var name = match.Groups[SectionGroup].Value.Trim();
				string pattern = $@"\s*(?'{NameGroup}'[\S=]*)\s*=\s*(?'{ValueGroup}'[\S]*)";
				MatchCollection ms = new Regex(pattern).Matches(match.Groups[ValuesGroup].Value);

				var type = (from Match m in ms where m.Groups[NameGroup].Value == TypeWord select m.Groups[ValueGroup]).First().Value.Trim();
				var description = (from Match m in ms where m.Groups[NameGroup].Value == DescriptionWord select m.Groups[ValueGroup]).First().Value.Trim();
				var valuestr = (from Match m in ms where m.Groups[NameGroup].Value == ValueWord select m.Groups[ValueGroup]).First().Value.Trim();
				var defaultstr = (from Match m in ms where m.Groups[NameGroup].Value == DefaultWord select m.Groups[ValueGroup]).First().Value.Trim();
				var mindata = (from Match m in ms where m.Groups[NameGroup].Value == MinWord select m).Any();
				var minstr = mindata ? (from Match m in ms where m.Groups[NameGroup].Value == MinWord select m.Groups[ValueGroup]).First().Value.Trim() : null;
				var maxdata = (from Match m in ms where m.Groups[NameGroup].Value == MaxWord select m).Any();
				var maxstr = maxdata ? (from Match m in ms where m.Groups[NameGroup].Value == MaxWord select m.Groups[ValueGroup]).First().Value.Trim() : null;


				switch (type)
				{
					case nameof(EParameterType.String):
						{
							string value = valuestr;
							string @default = defaultstr;
							_parameters.Add(name, new StringParameter(name, description, value, @default));
							TableAdd(name, type, valuestr, defaultstr, minstr, maxstr, description);
						}
						break;
					case nameof(EParameterType.Boolean):
						{
							bool value = Convert.ToBoolean(valuestr);
							bool @default = Convert.ToBoolean(defaultstr);
							_parameters.Add(name, new BoolParameter(name, description, value, @default));
							TableAdd(name, type, valuestr, defaultstr, minstr, maxstr, description);
						}
						break;
					case nameof(EParameterType.Int32):
						{
							int value = Convert.ToInt32(valuestr);
							int @default = Convert.ToInt32(defaultstr);
							int min = Convert.ToInt32(minstr);
							int max = Convert.ToInt32(maxstr);
							_parameters.Add(name, new Int32Parameter(name, description, value, @default, min, max));
							TableAdd(name, type, valuestr, defaultstr, minstr, maxstr, description);
						}
						break;
					case nameof(EParameterType.Single):
						{
							float value = Convert.ToSingle(valuestr);
							float @default = Convert.ToSingle(defaultstr);
							float min = Convert.ToSingle(minstr);
							float max = Convert.ToSingle(maxstr);
							_parameters.Add(name, new SingleParameter(name, description, value, @default, min, max));
							TableAdd(name, type, valuestr, defaultstr, minstr, maxstr, description);
						}
						break;
				}

			}
		}

		private void TableAdd(string name, string type, string value, string @default, string min, string max, string description)
		{
			DataRow row = _parametertable.NewRow();
			row[ParameterWord] = name;
			row[NameWord] = new RtfBuilder(name).ToRTF();
			row[TypeWord] = new RtfBuilder(type).ToRTF();
			row[ValueWord] = new RtfBuilder(value).ToRTF();
			row[DefaultWord] = new RtfBuilder(@default).ToRTF();
			row[MinWord] = new RtfBuilder(min).ToRTF();
			row[MaxWord] = new RtfBuilder(max).ToRTF();
			row[DescriptionWord] = new RtfBuilder(description).ToRTF();
			_parametertable.Rows.Add(row);
			var h = new RtfBuilder(name).FontHeigh;
		}
		#endregion
	}
}

