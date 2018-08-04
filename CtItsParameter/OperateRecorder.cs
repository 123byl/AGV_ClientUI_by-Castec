using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CtItsParameter.Interface;

namespace CtItsParameter
{
	class OperateRecorder
	{
		private int key;
		public static OperateRecorder Record { get; } = new OperateRecorder();
		public SortedList<int, StepInfo> UndoList { get; protected set; }
		public SortedList<int,StepInfo> RedoList { get; protected set; }
		private int GetKey { get { var r = key;key++;return r;  } } 

		private OperateRecorder ()
		{
			UndoList = new SortedList<int, StepInfo>();
			RedoList = new SortedList<int, StepInfo>();
			key = 0;
		}

		public void Add(StepInfo info)
		{
			UndoList.Add(GetKey,info);
		}
		public List<StepInfo> Undo(int key)
		{
			List<StepInfo> list = new List<StepInfo>();
			int index = UndoList.IndexOfKey(key);
			if (index !=-1)
			{
				for (int i = UndoList.Count-1; i>= index;i--)
				{
					StepInfo info = UndoList.Values[i];
					list.Add(info);
					RedoList.Add(GetKey, info);
					UndoList.RemoveAt(i);
				}
				return list;
			}
			return null;
		}

		public List<StepInfo> Redo(int key)
		{
			List<StepInfo> list = new List<StepInfo>();
			int index = RedoList.IndexOfKey(key);
			if (index != -1)
			{
				for(int i = RedoList.Count-1;i>=index;i--)
				{
					StepInfo info = RedoList.Values[i];
					list.Add(info);
					UndoList.Add(GetKey, info);
					RedoList.RemoveAt(i);
				}
				return list;
			}
			return null;
		}

	}

	public class StepInfo
	{

		public enum EOperateType
		{
			Add,
			Edit,
			Remove,
		}
		public EOperateType Operate { get; protected set; }
	}

	public class StepInfo<T> : StepInfo where T : IOperate
	{
		public T CurrentInfo { get; protected set; }
		public T NewInfo { get; protected set; }

		public StepInfo(EOperateType operate, T cur, T @new)
		{
			Operate = operate;
			CurrentInfo = cur;
			NewInfo = @new;
		}
	}
	public class StringParameterStepInfo : StepInfo<StringParameter>
	{
		public StringParameterStepInfo(EOperateType operate, StringParameter cur, StringParameter @new) : base(operate, cur, @new)
		{
		}
	}
	public class BoolParameterStepInfo : StepInfo<BoolParameter>
	{
		public BoolParameterStepInfo(EOperateType operate, BoolParameter cur, BoolParameter @new) : base(operate, cur, @new)
		{
		}
	}
	public class Int32ParameterStepInfo : StepInfo<Int32Parameter>
	{
		public Int32ParameterStepInfo(EOperateType operate, Int32Parameter cur, Int32Parameter @new) : base(operate, cur, @new)
		{
		}
	}
	public class SingleParameterStepInfo : StepInfo<SingleParameter>
	{
		public SingleParameterStepInfo(EOperateType operate, SingleParameter cur, SingleParameter @new) : base(operate, cur, @new)
		{
		}
	}
}
