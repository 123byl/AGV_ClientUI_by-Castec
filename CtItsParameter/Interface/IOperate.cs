using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtItsParameter.Interface
{
	public interface IOperate
	{
		bool IsModify { get; }
	}
	public interface IOperate<T,TStructure> : IOperate
	{
		void Edit(T newValue);

		void Undo(TStructure oldValue);

		void Redo(TStructure newValue);
	}
}
