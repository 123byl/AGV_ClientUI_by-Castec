using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CtLib.Module.Net {

	/// <summary>提供網路模組的內部擴充方法</summary>
	internal static class Extension {

		/// <summary>取得此 <see cref="Socket"/> 是否有成功連線至端點。採用 <see cref="Socket.Poll(int, SelectMode)"/> 進行檢查</summary>
		/// <param name="socket">欲檢查的 <see cref="Socket"/> 物件</param>
		/// <returns>(True)連線至端點  (False)未連線</returns>
		internal static bool IsConnected(this Socket socket) {
			return socket.Connected && !(socket.Poll(1000, SelectMode.SelectRead) && (socket.Available == 0));
		}

	}
}
