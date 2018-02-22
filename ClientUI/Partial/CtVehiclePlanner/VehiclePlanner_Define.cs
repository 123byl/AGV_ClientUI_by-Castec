using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehiclePlanner.Partial.CtVehiclePlanner {

    #region Declration - Enums

    /// <summary>
    /// 車子運動方向
    /// </summary>
    /// <remarks>
    /// 前後左右列舉值為鍵盤方向鍵之對應值
    /// 若是任意更改則會造成鍵盤控制發生例外
    /// </remarks>
    public enum MotionDirection {
        /// <summary>
        /// 往前
        /// </summary>
        Forward = 38,
        /// <summary>
        /// 往後
        /// </summary>
        Backward = 40,
        /// <summary>
        /// 左旋
        /// </summary>
        LeftTrun = 37,
        /// <summary>
        /// 右旋
        /// </summary>
        RightTurn = 39,
        /// <summary>
        /// 停止
        /// </summary>
        Stop = 4
    }

    /// <summary>
    /// 檔案類型
    /// </summary>
    public enum FileType {
        Ori,
        Map,
    }

    /// <summary>
    /// VehiclePlanner事件列舉
    /// </summary>
    public enum VehiclePlannerEvents {
        /// <summary>
        /// 標示物變更事件
        /// </summary>
        MarkerChanged,
        /// <summary>
        /// 清空地圖
        /// </summary>
        NewMap,
        /// <summary>
        /// 程式關閉
        /// </summary>
        Dispose,

    }

    #endregion Declaration - Enums

    #region Declaration - Delegates

    /// <summary>
    /// Console顯示訊息事件
    /// </summary>
    /// <param name="msg"></param>
    public delegate void ConsoleMessagEventHandler(string msg);

    /// <summary>
    /// 錯誤訊息事件
    /// </summary>
    /// <param name="err"></param>
    public delegate void ErrorMessageEventHandler(string err);

    /// <summary>
    /// 檔案選擇方法委派
    /// </summary>
    /// <param name="fileList"></param>
    /// <returns></returns>
    public delegate string DelSelectFile(string fileList);

    #endregion Declaration - Delegates

    #region Declaration - Const

    /// <summary>
    /// 屬性名稱宣告
    /// </summary>
    public static class PropertyDeclaration {
        public const string iTSs = "iTSs";
        public const string MainVisible = "MainVisible";
        public const string IsMotorServoOn = "IsMotorServoOn";
        public const string IsConnected = "IsConnected";
        public const string IsScanning = "IsScanning";
        public const string Status = "Status";
        public const string IsAutoReport = "IsAutoReport";
        public const string Velocity = "Velocity";
        public const string MapCenter = "MapCenter";
        public const string IsBypassSocket = "IsBypassSocket";
        public const string IsBypassLoadFile = "IsBypassLoadFile";
        public const string HostIP = "HostIP";
        public const string UserData = "UserData";

        public const string Culture = "Culture";
    }

    #endregion Declaration - Const

}
