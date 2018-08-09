using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;

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

		public delegate void ParameterHeaderEvent(object sender, ParameterHeaderEventArgs e);
		public event EventHandler LoadFinish;
		public event ParameterHeaderEvent ParameterChange;
		#endregion

		#region Declaration - Properties
		public Dictionary<string, ParameterInfo> Parameters { get => _parameters; }
		public DataTable ParameterTable { get => _parametertable; }
		public SortedList<int, StepInfo> UndoList => OperateRecorder.Record.UndoList;
		public SortedList<int, StepInfo> RedoList => OperateRecorder.Record.RedoList;
		public int UndoCount => OperateRecorder.Record.UndoList.Count;
		public int RedoCount => OperateRecorder.Record.RedoList.Count;
		public bool IsLoad { get; private set; } = false;
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
				IsLoad = true;
				_parametertable.DefaultView.Sort = $"{ParameterWord} ASC";
			}
			ParameterChange?.Invoke(this, new ParameterHeaderEventArgs(UndoCount, RedoCount,IsLoad));
		}
		public void Clear()
		{
			_parameters.Clear();
			_parametertable.Rows.Clear();
			IsLoad = false;
			ParameterChange?.Invoke(this, new ParameterHeaderEventArgs(UndoCount, RedoCount,IsLoad));
		}

		public void Save(string path)
		{
			var directory = Path.GetDirectoryName(path);
			var exist = Directory.Exists(directory);
			var subname = Path.GetExtension(path);
			if (exist && subname == ".ini")
			{
				StringBuilder builder = new StringBuilder();
				foreach (var item in _parameters.Values)
				{
					if (item is StringParameter)
					{
						var parameter = item as StringParameter;
						BuilderAppend(builder, parameter.Name, EParameterType.String, parameter.Value, parameter.Description, parameter.Default);
					}
					else if (item is BoolParameter)
					{
						var parameter = item as BoolParameter;
						BuilderAppend(builder, parameter.Name, EParameterType.Boolean, parameter.Value, parameter.Description, parameter.Default);
					}
					else if (item is Int32Parameter)
					{
						var parameter = item as Int32Parameter;
						BuilderAppend(builder, parameter.Name, EParameterType.Int32, parameter.Value, parameter.Description, parameter.Default, parameter.Min, parameter.Max);
					}
					else if (item is SingleParameter)
					{
						var parameter = item as SingleParameter;
						BuilderAppend(builder, parameter.Name, EParameterType.Single, parameter.Value, parameter.Description, parameter.Default, parameter.Min, parameter.Max);
					}
				}
				File.WriteAllText(path, builder.ToString(), Encoding.UTF8);
				Clear();
			}
		}

		public void Edit(string parameter, string Type, string value)
		{
			switch (Type)
			{
				case nameof(EParameterType.String):
					{
						var data = (_parameters[parameter]) as StringParameter;
						if (data.Value.ToString() != value)
						{
							data.Edit(value);
							var rows = _parametertable.Select($"{ParameterWord}='{parameter}'");
							int index = _parametertable.Rows.IndexOf(rows[0]);
							_parametertable.Rows[index][ValueWord] = new RtfBuilder(data.Value.ToString(), Color.Red).ToRTF();
						}
					}
					break;
				case nameof(EParameterType.Boolean):
					{
						var data = (_parameters[parameter]) as BoolParameter;
						if (data.Value.ToString() != value)
						{
							bool @new;
							if (bool.TryParse(value, out @new))
							{
								data.Edit(@new);
								var rows = _parametertable.Select($"{ParameterWord}='{parameter}'");
								int index = _parametertable.Rows.IndexOf(rows[0]);
								_parametertable.Rows[index][ValueWord] = new RtfBuilder(data.Value.ToString(), Color.Red).ToRTF();
							}
						}
					}
					break;
				case nameof(EParameterType.Int32):
					{
						var data = (_parameters[parameter]) as Int32Parameter;
						if (data.Value.ToString() != value)
						{
							int @new;
							if (int.TryParse(value, out @new) && data.Max != data.Min && @new < data.Max && @new > data.Min)
							{
								data.Edit(@new);
								var rows = _parametertable.Select($"{ParameterWord}='{parameter}'");
								int index = _parametertable.Rows.IndexOf(rows[0]);
								_parametertable.Rows[index][ValueWord] = new RtfBuilder(data.Value.ToString(), Color.Red).ToRTF();
							}
						}
					}
					break;
				case nameof(EParameterType.Single):
					{
						var data = (_parameters[parameter]) as SingleParameter;
						if (data.Value.ToString() != value)
						{
							float @new;
							if (float.TryParse(value, out @new) && data.Max != data.Min && @new < data.Max && @new > data.Min)
							{
								data.Edit(@new);
								var rows = _parametertable.Select($"{ParameterWord}='{parameter}'");
								int index = _parametertable.Rows.IndexOf(rows[0]);
								_parametertable.Rows[index][ValueWord] = new RtfBuilder(data.Value.ToString(), Color.Red).ToRTF();
							}
						}
					}
					break;
			}
			ParameterChange?.Invoke(this, new ParameterHeaderEventArgs(UndoCount,RedoCount,IsLoad));
		}
		public void Undo()
		{
			if (OperateRecorder.Record.UndoList.Count > 0)
			{
				int key = OperateRecorder.Record.UndoList.Keys[OperateRecorder.Record.UndoList.Count - 1];
				var list = OperateRecorder.Record.Undo(key);
				foreach (var item in list)
				{

					if (item is StringParameterStepInfo)
					{
						var info = item as StringParameterStepInfo;
						var name = info.OldInfo.Name;
						(_parameters[name] as StringParameter).Undo(info.OldInfo);
						foreach (DataRow row in _parametertable.Rows)
						{
							if (row[ParameterWord].ToString() == name)
							{
								row[ValueWord] = (_parameters[name] as StringParameter).ToRtf(ParameterInfo<string>.Property.Value);
							}
						}
					}
					else if (item is BoolParameterStepInfo)
					{
						var info = item as BoolParameterStepInfo;
						var name = info.OldInfo.Name;
						foreach (DataRow row in _parametertable.Rows)
						{
							if (row[ParameterWord].ToString() == name)
							{
								row[ValueWord] = (_parameters[name] as BoolParameter).ToRtf(ParameterInfo<bool?>.Property.Value);
							}
						}
					}
					else if (item is Int32ParameterStepInfo)
					{
						var info = item as Int32ParameterStepInfo;
						var name = info.OldInfo.Name;
						foreach (DataRow row in _parametertable.Rows)
						{
							if (row[ParameterWord].ToString() == name)
							{
								row[ValueWord] = (_parameters[name] as Int32Parameter).ToRtf(ParameterInfo<int?>.Property.Value);
							}
						}
					}
					else if (item is SingleParameterStepInfo)
					{
						var info = item as SingleParameterStepInfo;
						var name = info.OldInfo.Name;
						foreach (DataRow row in _parametertable.Rows)
						{
							if (row[ParameterWord].ToString() == name)
							{
								row[ValueWord] = (_parameters[name] as SingleParameter).ToRtf(ParameterInfo<float?>.Property.Value);
							}
						}
					}
				}
			}
			ParameterChange?.Invoke(this, new ParameterHeaderEventArgs(UndoCount,RedoCount,IsLoad));
		}

		public void Redo()
		{
			if (OperateRecorder.Record.RedoList.Count > 0)
			{
				int key = OperateRecorder.Record.RedoList.Keys[OperateRecorder.Record.RedoList.Count - 1];
				var list = OperateRecorder.Record.Redo(key);
				foreach (var item in list)
				{
					if (item is StringParameterStepInfo)
					{
						var info = item as StringParameterStepInfo;
						var name = info.NewInfo.Name;
						(_parameters[name] as StringParameter).Redo(info.NewInfo);
						foreach (DataRow row in _parametertable.Rows)
						{
							if (row[ParameterWord].ToString() == name)
							{
								row[ValueWord] = (_parameters[name] as StringParameter).ToRtf(ParameterInfo<string>.Property.Value);
							}
						}
					}
					else if (item is BoolParameterStepInfo)
					{
						var info = item as BoolParameterStepInfo;
						var name = info.NewInfo.Name;
						(_parameters[name] as BoolParameter).Redo(info.NewInfo);
						foreach (DataRow row in _parametertable.Rows)
						{
							if (row[ParameterWord].ToString() == name)
							{
								row[ValueWord] = (_parameters[name] as BoolParameter).ToRtf(ParameterInfo<bool?>.Property.Value);
							}
						}
					}
					if (item is Int32ParameterStepInfo)
					{
						var info = item as Int32ParameterStepInfo;
						var name = info.NewInfo.Name;
						(_parameters[name] as Int32Parameter).Redo(info.NewInfo);
						foreach (DataRow row in _parametertable.Rows)
						{
							if (row[ParameterWord].ToString() == name)
							{
								row[ValueWord] = (_parameters[name] as Int32Parameter).ToRtf(ParameterInfo<int?>.Property.Value);
							}
						}
					}
					if (item is SingleParameterStepInfo)
					{
						var info = item as SingleParameterStepInfo;
						var name = info.NewInfo.Name;
						(_parameters[name] as SingleParameter).Redo(info.NewInfo);
						foreach (DataRow row in _parametertable.Rows)
						{
							if (row[ParameterWord].ToString() == name)
							{
								row[ValueWord] = (_parameters[name] as SingleParameter).ToRtf(ParameterInfo<float?>.Property.Value);
							}
						}
					}
				}
			}
			ParameterChange?.Invoke(this, new ParameterHeaderEventArgs(UndoCount,RedoCount,IsLoad));
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
				string pattern = $@"\s*(?'{NameGroup}'[\S=]*)\s*=\s*(?'{ValueGroup}'[^\r]*)";
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
			LoadFinish?.Invoke(this, EventArgs.Empty);
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
		private void BuilderAppend(StringBuilder builder, string name, EParameterType type, object value, string description, object @default, object min = null, object max = null)
		{
			builder.AppendLine($"[{name}]");
			builder.AppendLine($"{TypeWord}={type.ToString()}");
			builder.AppendLine($"{ValueWord}={value.ToString()}");
			builder.AppendLine($"{DescriptionWord}={description}");
			builder.AppendLine($"{DefaultWord}={@default.ToString()}");
			if (max != null) builder.AppendLine($"{MaxWord}={max.ToString()}");
			if (min != null) builder.AppendLine($"{MinWord}= {min.ToString()}");
			builder.AppendLine();
		}
		#endregion
	}
	public class ParameterHeaderEventArgs : EventArgs
	{
		public int RedoCount { get; private set; }
		public int UndoCount { get; private set; }
		public bool IsLoad { get; private set; }

		public ParameterHeaderEventArgs(int undo, int redo,bool isLoad) : base()
		{
			UndoCount = undo;
			RedoCount = redo;
			IsLoad = isLoad;
		}
	}
}


