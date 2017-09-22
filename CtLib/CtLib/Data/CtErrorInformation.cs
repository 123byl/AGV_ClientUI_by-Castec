using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CtLib.Library {

    /// <summary>
    /// 錯誤相關訊息
    /// <para>內含時間、代碼、解決方法等。亦可用於錯誤發報</para>
    /// </summary>
    public class CtErrorInfo {
        /// <summary>事件發生之時間點</summary>
        public DateTime Time { get; internal set; }
        /// <summary>設備</summary>
        public Devices Device { get; internal set; }
        /// <summary>錯誤代碼</summary>
        public int ErrorCode { get; internal set; }
        /// <summary>錯誤資訊</summary>
        public string Information { get; internal set; }
        /// <summary>解除方法</summary>
        public string Solution { get; internal set; }
        /// <summary>錯誤訊息之標題</summary>
        public string Title { get; internal set; }

        /// <summary>建立帶有預設值的警報資訊</summary>
        /// <param name="device">設備</param>
        /// <param name="errCode">錯誤代碼</param>
        /// <param name="errInfo">錯誤資訊</param>
        /// <param name="errSol">解除方法</param>
        public CtErrorInfo(Devices device, int errCode, string errInfo, string errSol = "") {
            Time = DateTime.Now;
            Device = device;
            ErrorCode = errCode;
            Information = errInfo;
            Solution = errSol;
        }

        /// <summary>建立帶有預設值的警報資訊 (含標題)</summary>
        /// <param name="device">設備</param>
        /// <param name="errCode">錯誤代碼</param>
        /// <param name="errInfo">錯誤資訊</param>
        /// <param name="errSol">解除方法</param>
        /// <param name="errTitle">錯誤訊息標題</param>
        public CtErrorInfo(Devices device, int errCode, string errTitle, string errInfo, string errSol = "") {
            Time = DateTime.Now;
            Device = device;
            ErrorCode = errCode;
            Information = errInfo;
            Solution = errSol;
            Title = errTitle;
        }
    }
}
