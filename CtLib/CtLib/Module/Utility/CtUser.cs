using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtLib.Module.Utility {

    /// <summary>
    /// 紀錄使用者相關資料，包含帳號、密碼、權限等資訊
    /// </summary>
    public class UserData {
        /// <summary>帳號</summary>
        public string Account { get; private set; }
        /// <summary>密碼</summary>
        public string Password { get; set; }

        /// <summary>使用者等級</summary>
        public AccessLevel Level { get; set; }

        /// <summary>建立日期</summary>
        public DateTime BuiltTime { get; set; }

        /// <summary>由誰建立</summary>
        public string Creator { get; set; }

        /// <summary>建立一全空的資料，請手動指定屬性</summary>
        public UserData() {
            Account = "N/A";
            Password = "";
            Level = AccessLevel.None;
            Creator = "Unknown";
        }

		/// <summary>建立一包含預設值之使用者資料</summary>
		/// <param name="usrAccount">使用者帳號</param>
		/// <param name="usrPassword">使用者密碼</param>
		/// <param name="usrLevel">使用者權限</param>
		public UserData(string usrAccount, string usrPassword, AccessLevel usrLevel) {
            Account = usrAccount;
            Password = usrPassword;
            Level = usrLevel;
            BuiltTime = DateTime.Now;
            Creator = "Unknown";
        }

		/// <summary>建立完整使用者資料</summary>
		/// <param name="usrAccount">使用者帳號</param>
		/// <param name="usrPassword">使用者密碼</param>
		/// <param name="usrLevel">使用者權限</param>
		/// <param name="usrTime">帳號建立時間</param>
		/// <param name="usrBuilt">帳號建立者(由誰建立)</param>
		public UserData(string usrAccount, string usrPassword, AccessLevel usrLevel, DateTime usrTime, string usrBuilt) {
            Account = usrAccount;
            Password = usrPassword;
            Level = usrLevel;
            BuiltTime = usrTime;
            Creator = usrBuilt;
        }
    }

    /// <summary>使用者層級</summary>
    public enum AccessLevel : byte {
		/// <summary>CASTEC 管理員</summary>
		/// <remarks>包含軟體工程師、客服等</remarks>
		Administrator = byte.MaxValue,
        /// <summary>[客戶端] 工程師</summary>
        Engineer = 2,
        /// <summary>[客戶端] 操作員</summary>
        Operator = 1,
        /// <summary>尚未登入，或是已登出</summary>
        None = 0
    }

    /// <summary>決定讀/寫檔案時之檔案內容順序</summary>
    /// <remarks>建議可以混放，這樣加上AES256加密比較不容易被破解</remarks>
    internal enum UserDataSequence : byte {
        /// <summary>使用者資料檔 之 帳號名稱存放順序</summary>
        Account = 2,
        /// <summary>使用者資料檔 之 密碼存放順序</summary>
        Password = 1,
        /// <summary>使用者資料檔 之 等級存放順序</summary>
        AccessLevel = 4,
        /// <summary>使用者資料檔 之 建立日期存放順序</summary>
        BuiltDate = 0,
		/// <summary>使用者資料檔 之 建立者存放順序</summary>
		Creator = 3
    }
}
