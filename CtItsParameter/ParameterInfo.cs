using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CtItsParameter.Interface;


namespace CtItsParameter
{
	public enum EParameterType
	{
		String,
		Boolean,
		Int32,
		Single
	}

	public abstract class ParameterInfo : IParameter
	{
		public string Name { get; protected set; }

		public string Description { get; protected set; }
	}

	public abstract class ParameterInfo<T> : ParameterInfo, IParameter, IParameter<T>, IOperate, IOperate<T>
	{
		public T Value { get; protected set; }

		public T Default { get; protected set; }

		public T Min { get; protected set; }

		public T Max { get; protected set; }

		public bool IsModify { get; protected set; } = false;

		public virtual void Edit(T newValue)
		{
			IsModify = true;
		}

		public ParameterInfo(string name, string description, T value, T @default, T min, T max)
		{
			Name = name;
			Description = description;
			Value = value;
			Default = @default;
			Min = min;
			Max = max;
		}
	}

	public class StringParameter : ParameterInfo<string>
	{
		public StringParameter(string name, string description, string value, string @default, string min = null, string max = null) : base(name, description, value, @default, min, max)
		{ }

		public override void Edit(string newValue)
		{
			base.Edit(newValue);
			var current = this.Clone();
			this.Value = newValue;
			var @new = this;
			OperateRecorder.Record.Add(new StringParameterStepInfo(StepInfo.EOperateType.Edit, current, @new));
		}

		public StringParameter Clone()
		{
			return new StringParameter(Name, Description, Value, Default);
		}
	}

	public class BoolParameter : ParameterInfo<bool?>
	{
		public BoolParameter(string name, string description, bool? value, bool? @default, bool? min = null, bool? max = null) : base(name, description, value, @default, min, max)
		{
		}

		public override void Edit(bool? newValue)
		{
			base.Edit(newValue);
			var current = this.Clone();
			this.Value = Value;
			var @new = this;
			OperateRecorder.Record.Add(new BoolParameterStepInfo(StepInfo.EOperateType.Edit, current, @new));
		}

		public BoolParameter Clone()
		{
			return new BoolParameter(Name, Description, Value, Default);
		}
	}

	public class Int32Parameter : ParameterInfo<int?>
	{
		public Int32Parameter(string name, string description, int? value, int? @default, int? min, int? max) : base(name, description, value, @default, min, max)
		{
		}

		public override void Edit(int? newValue)
		{
			var current = this.Clone();
			this.Value = newValue;
			var @new = this;
			OperateRecorder.Record.Add(new Int32ParameterStepInfo(StepInfo.EOperateType.Edit, current, @new));
		}

		public Int32Parameter Clone()
		{
			return new Int32Parameter(Name, Description, Value, Default, Min, Min);
		}
	}

	public class SingleParameter : ParameterInfo<float>
	{
		public SingleParameter(string name, string description, float value, float @default, float min, float max) : base(name, description, value, @default, min, max)
		{
		}

		public override void Edit(float newValue)
		{
			var current = this.Clone();
			this.Value = newValue;
			var @new = this;
			OperateRecorder.Record.Add(new SingleParameterStepInfo(StepInfo.EOperateType.Edit, current, @new));
		}
		public SingleParameter Clone()
		{
			return new SingleParameter(Name, Description, Value, Default, Min, Max);
		}

	}
}
