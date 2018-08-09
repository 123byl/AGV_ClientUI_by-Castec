using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CtItsParameter.Interface;
using System.Drawing;

namespace CtItsParameter
{
	public enum EParameterType
	{
		String,
		Boolean,
		Int32,
		Single
	}

	public abstract class ParameterInfo : IParameter ,IOperate
	{
		public string Name { get; protected set; }

		public string Description { get; protected set; }
		public bool IsModify { get; protected set; } = false;
	}

	public abstract class ParameterInfo<T> : ParameterInfo, IParameter, IParameter<T>, IOperate<T,ParameterInfo<T>>
	{
		public enum Property
		{
			Name,
			Descriotion,
			Value,
			Default,
			Min,
			Max
		}
		public T Value { get; protected set; }

		public T Default { get; protected set; }

		public T Min { get; protected set; }

		public T Max { get; protected set; }

		public virtual void Edit(T newValue)
		{
			IsModify = true;
		}

		public void Undo(ParameterInfo<T> oldValue)
		{
			Value = oldValue.Value;
			IsModify = oldValue.IsModify;
		}

		public void Redo(ParameterInfo<T> newValue)
		{
			Value = newValue.Value;
			IsModify = newValue.IsModify;
		}

		public string ToRtf(Property property)
		{
			switch(property)
			{
				case Property.Name:
					return new RtfBuilder(Name).ToRTF();
				case Property.Descriotion:
					return new RtfBuilder(Description).ToRTF();
				case Property.Value:
					switch(IsModify)
					{
						case true:
							return new RtfBuilder(Value.ToString(),Color.Red).ToRTF();
						default:
							return new RtfBuilder(Value.ToString()).ToRTF();
					}
				case Property.Default:
					return new RtfBuilder(Default.ToString()).ToRTF();
				case Property.Max:
					return new RtfBuilder(Max.ToString()).ToRTF();
				case Property.Min:
					return new RtfBuilder(Min.ToString()).ToRTF();
				default:
					return new RtfBuilder("").ToRTF();
			}
		}

		public ParameterInfo(string name, string description, T value, T @default, T min, T max,bool modify = false)
		{
			Name = name;
			Description = description;
			Value = value;
			Default = @default;
			Min = min;
			Max = max;
			IsModify = modify;
		}
	}

	public class StringParameter : ParameterInfo<string>
	{
		public StringParameter(string name, string description, string value, string @default, string min = null, string max = null,bool modify =false) : base(name, description, value, @default, min, max,modify)
		{ }

		public override void Edit(string newValue)
		{
			var current = this.Clone();
			base.Edit(newValue);
			this.Value = newValue;
			var @new = this.Clone();
			OperateRecorder.Record.Add(new StringParameterStepInfo(StepInfo.EOperateType.Edit, current, @new));
		}

		public StringParameter Clone()
		{
			return new StringParameter(Name, Description, Value, Default,null,null,IsModify);
		}
	}

	public class BoolParameter : ParameterInfo<bool?>
	{
		public BoolParameter(string name, string description, bool? value, bool? @default, bool? min = null, bool? max = null,bool modify=false) : base(name, description, value, @default, min, max,modify)
		{
		}

		public override void Edit(bool? newValue)
		{
			var current = this.Clone();
			base.Edit(newValue);
			this.Value = newValue;
			var @new = this.Clone() ;
			OperateRecorder.Record.Add(new BoolParameterStepInfo(StepInfo.EOperateType.Edit, current, @new));
		}

		public BoolParameter Clone()
		{
			return new BoolParameter(Name, Description, Value, Default,null,null,IsModify);
		}
	}

	public class Int32Parameter : ParameterInfo<int?>
	{
		public Int32Parameter(string name, string description, int? value, int? @default, int? min, int? max,bool modify =false) : base(name, description, value, @default, min, max,modify)
		{
		}

		public override void Edit(int? newValue)
		{
			var current = this.Clone();
			base.Edit(newValue);
			this.Value = newValue;
			var @new = this.Clone();
			OperateRecorder.Record.Add(new Int32ParameterStepInfo(StepInfo.EOperateType.Edit, current, @new));
		}

		public Int32Parameter Clone()
		{
			return new Int32Parameter(Name, Description, Value, Default, Min, Min,IsModify);
		}
	}

	public class SingleParameter : ParameterInfo<float?>
	{
		public SingleParameter(string name, string description, float? value, float? @default, float? min, float? max,bool modify = false) : base(name, description, value, @default, min, max,modify)
		{
		}

		public override void Edit(float? newValue)
		{
			var current = this.Clone();
			base.Edit(newValue);
			this.Value = newValue;
			var @new = this.Clone();
			OperateRecorder.Record.Add(new SingleParameterStepInfo(StepInfo.EOperateType.Edit, current, @new));
		}
		public SingleParameter Clone()
		{
			return new SingleParameter(Name, Description, Value, Default, Min, Max,IsModify);
		}

	}
}
