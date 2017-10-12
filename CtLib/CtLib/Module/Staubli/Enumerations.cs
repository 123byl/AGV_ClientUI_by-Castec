using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtLib.Module.Staubli {

    /// <summary>任務狀態</summary>
    public enum TaskState {
        /// <summary>閒置</summary>
        Idle = 0,
        /// <summary>狀態轉換中</summary>
        Transition = 1,
        /// <summary>執行中</summary>
        Running = 2,
        /// <summary>中斷、單步執行中</summary>
        Stepping = 3,
        /// <summary>停止</summary>
        Stopped = 4
    }

    /// <summary>手臂直徑</summary>
    public enum Diameter {
        /// <summary>未定義或違法的數值</summary>
        Invalid,
        /// <summary>D20</summary>
        D20,
        /// <summary>D25</summary>
        D25
    }

    /// <summary>手臂長度</summary>
    public enum Length {
        /// <summary>未定義或違法的數值</summary>
        Invalid,
        /// <summary>L100</summary>
        L100,
        /// <summary>L200</summary>
        L200,
        /// <summary>L400</summary>
        L400,
        /// <summary>L600</summary>
        L600
    }

    /// <summary>手臂安裝方式</summary>
    public enum Mount {
        /// <summary>未定義或違法的數值</summary>
        Invalid,
        /// <summary>正裝，安裝於平面上</summary>
        Floor,
        /// <summary>倒掛，安裝於天花板</summary>
        Ceiling,
        /// <summary>牆</summary>
        Wall
    }

    /// <summary>手臂樣式</summary>
    public enum ArmType {
        /// <summary>未定義或違法的數值</summary>
        Invalid,
        /// <summary>擬人化手臂(六軸)</summary>
        Anthropomorphic6,
        /// <summary>擬人化手臂(五軸)</summary>
        Anthropomorphic5,
        /// <summary>水平關節型(四軸)</summary>
        Scara,
        /// <summary>Eisenmann</summary>
        Eisenmann
    }

    /// <summary>I/O 定義狀態</summary>
    public enum IoDefine {
        /// <summary>已定義</summary>
        Defined,
        /// <summary>未定義</summary>
        Undefined,
        /// <summary>違法的名稱</summary>
        InvalidName
    }

    /// <summary>CS8 I/O 類型</summary>
    public enum IoType {
        /// <summary>數位輸入</summary>
        DIn,
        /// <summary>數位輸出</summary>
        DOut,
        /// <summary>類比輸入</summary>
        AIn,
        /// <summary>類比輸出</summary>
        AOut,
        /// <summary>串列埠</summary>
        Serial,
        /// <summary>TCP/IP 之 Socket 用戶端</summary>
        SocketClient,
        /// <summary>TCP/IP 之 Socket 伺服端</summary>
        SocketServer
    }
}
