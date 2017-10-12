using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CtLib.Library;
using CtLib.Module.Utility;

using Ace.Core.Server;
using Ace.Core.Server.Scripting;

namespace CtLib.Module.Adept {
	/// <summary>Adept ACE 腳本類相關控制</summary>
	[Serializable]
	public sealed class CtAceScript {

		#region Declaration - Fields
		/// <summary>V+ 相關物件連結，用於控制 Task</summary>
		private VpObjects mVpObj;
		/// <summary>Adept ACE IAceServer Interface</summary>
		private IAceServer mIServer;
		#endregion

		#region Function - Constructors
		/// <summary>建立一 Task 相關控制物件</summary>
		/// <param name="aceSrv">Ace Server</param>
		/// <param name="links">V+ 相關物件連結，用於控制 Task</param>
		internal CtAceScript(IAceServer aceSrv, VpObjects links) {
			mVpObj = links;
			mIServer = aceSrv;
		}
		#endregion

		#region Function - Public Operations
		/// <summary>編譯 <see cref="ICSharpProgram"/></summary>
		/// <param name="path">C# 物件路徑，如 "/Program/LoadRecipe"</param>
		public void CompileCSharpProgram(string path) {
			ICSharpProgram program = mIServer.Root[path] as ICSharpProgram;
			if (program == null) throw new ArgumentNullException("Can not find specified program");
			program.Compile();
		}

		/// <summary>執行 <see cref="ICSharpProgram"/></summary>
		/// <param name="path">C# 物件路徑，如 "/Program/LoadRecipe"</param>
		public void ExecuteCSharpProgram(string path) {
			ICSharpProgram program = mIServer.Root[path] as ICSharpProgram;
			if (program == null) throw new ArgumentNullException("Can not find specified program");
			program.Execute();
		}
		#endregion
	}
}
