using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtItsParameter.Interface
{
	public interface IParameter
	{
		/// <summary>
		/// 參數名稱
		/// </summary>
		string Name { get; }
		/// <summary>
		/// 參數說明
		/// </summary>
		string Description { get; }
	}

	public interface IParameter<T> : IParameter
	{
		/// <summary>
		/// 參數設定值
		/// </summary>
		T Value { get; }
		/// <summary>
		/// 參數預設值
		/// </summary>
		T Default { get; }
		/// <summary>
		/// 參數最小值
		/// </summary>
		T Min { get; }
		/// <summary>
		/// 參數最大值
		/// </summary>
		T Max { get; }
	}
}
