using Ace.HSVision.Server.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtLib.Module.Adept {

    /// <summary>
    /// 控制器類型
    /// <para>此將影響連線方式及相關物件建立方式</para>
    /// </summary>
    public enum ControllerType : byte {
        /// <summary>SmartController CX/EX/ES</summary>
        SmartController = 0,
        /// <summary>嵌入式、整合式控制器，如 i-Series Robot</summary>
        Embedded = 1
    }

    /// <summary>Adept Industrial Robot Series</summary>
    public enum RobotType : byte {
        /// <summary>Adept Python Linear Modules</summary>
        Python = 1,
        /// <summary>Custom X-Y tables</summary>
        LinearModule = 2,
        /// <summary>Adept iCobra SCARA Robots (Without SmartController, using RS-232)</summary>
        iCOBRA = 3,
        /// <summary>Adept sCobra SCARA Robots (With SmartController)</summary>
        sCOBRA = 4,
        /// <summary>Adept Quattro Parallel Robots</summary>
        Quattro = 5,
        /// <summary>Adept Viper Six-Axis Robots</summary>
        Viper = 6
    }

    /// <summary>座標軸</summary>
    /// <remarks>0-Base</remarks>
    public enum Axis : byte {
        /// <summary>X</summary>
        X = 0,
        /// <summary>Y</summary>
        Y = 1,
        /// <summary>Z</summary>
        Z = 2,
        /// <summary>Yaw 或稱 rX</summary>
        Yaw = 3,
        /// <summary>Pitch 或稱 rY</summary>
        Pitch = 4,
        /// <summary>Roll 或稱 rZ 、 Theta</summary>
        Roll = 5
    }

    /// <summary>Joint 列舉</summary>
    /// <remarks>0-Base</remarks>
    public enum Joint : byte {
        /// <summary>第一軸</summary>
        Joint1 = 0,
		/// <summary>第二軸</summary>
		Joint2 = 1,
		/// <summary>第三軸</summary>
		Joint3 = 2,
		/// <summary>第四軸</summary>
		Joint4 = 3,
		/// <summary>第五軸</summary>
		Joint5 = 4,
		/// <summary>第六軸</summary>
		Joint6 = 5
    }

    /// <summary>路徑模式</summary>
    public enum MotionMode : byte {
        /// <summary>直線模式。以兩點間最短距離來移動</summary>
        StrightLine = 0,
        /// <summary>Joint內插。以 Robot Joint 最短距離來移動</summary>
        JointsInterpolated = 1
    }

    /// <summary>移動到點模式</summary>
    public enum MoveEndMode : byte {
        /// <summary>
        /// 滑順到點
        /// <para>若於開始減速前設定下一個命令，會以滑順的方式往下一點前進</para>
        /// <para>無須設置百分比</para>
        /// </summary>
        Blend = 1,
        /// <summary>完整到點</summary>
        NoNull = 0,
        /// <summary>
        /// 粗略百分比
        /// <para>粗略到達設置的百分比後(精準度較低)，往下一點前進</para>
        /// <para>請設置百分比，預設為 80%</para>
        /// </summary>
        SettleCoarse = 4,
        /// <summary>
        /// 精準百分比
        /// <para>到達設置的百分比後(較高精準度)，往下一點前進</para>
        /// <para>請設置百分比，預設為 80%</para>
        /// </summary>
        SettleFine = 2
    }

    /// <summary>速度模式</summary>
    public enum SpeedMode : byte {
        /// <summary>
        /// 百分比
        /// <para>適用於stright-line與joints-interpolated</para>
        /// </summary>
        Percent = 0,
        /// <summary>
        /// 毫米/每秒 (MMPS)
        /// <para>僅適用於stright-line</para>
        /// </summary>
        MillimeterPerSecond = 1,
        /// <summary>
        /// 英吋/每秒 (IPS)
        /// <para>僅適用於stright-line</para>
        /// </summary>
        InchesPerSecond = 2
    }

    /// <summary>Jog 模式</summary>
    public enum JogMode : byte {
        /// <summary>Manual mode without selection</summary>
        None = 0,
        /// <summary>Free-joint mode</summary>
        Free = 1,
        /// <summary>Individual joint control</summary>
        Joint = 2,
        /// <summary>World coordinates control</summary>
        World = 3,
        /// <summary>Tool coordinates control</summary>
        Tool = 4,
        /// <summary>Computer control enabled</summary>
        COMP = 5,
        /// <summary>Jogging toward a joint location</summary>
        JogToJointLocation = 6,
		/// <summary>Jogging toward a world location</summary>
		JogToWorldLocation = 7
    }

    /// <summary>Adept ACE Task 執行狀態</summary>
    public enum TaskState : sbyte {
        /// <summary>閒置，該Task尚未開始執行</summary>
        Idle = 0,
        /// <summary>執行中</summary>
        Executing = 4,
        /// <summary>發生例外而停止</summary>
        Exception = 2,
        /// <summary>暫停，但沒有發生任何例外情況</summary>
        Paused = 5,
        /// <summary>中斷或取消，同時會觸發例外狀況</summary>
        Abort = 3,
        /// <summary>執行完畢</summary>
        Complete = 1,
        /// <summary>違法事項</summary>
        Invalid = -1
    }

    /// <summary>Adept ACE V+ 變數類型</summary>
    public enum VPlusVariableType : byte {
        /// <summary>實數，對應於float或double</summary>
        Real = 0,
        /// <summary>字串</summary>
        @String = 1,
        /// <summary>世界座標，對應於Transform3D</summary>
        Location = 2,
        /// <summary>Joint，對應於PrecisionPoint</summary>
        PrecisionPoint = 3
    }

    /// <summary>Adept ACE 變數種類</summary>
    public enum VariableType : byte {
        /// <summary>V+ 變數，如 Real、Location、PrecisionPoint、String等</summary>
        VPlus = 0,
        /// <summary>Numeric Variable，即 IVariableNumeric，僅 float 數值</summary>
        Numeric = 1,
        /// <summary>String Variable，即 IVariableString，僅 String 數值</summary>
        @String = 2
    }

    /// <summary>支援的 Vision Tool 類型</summary>
    public enum VisionToolType : byte {
        /// <summary>攝影機</summary>
        VisionSource = 0,
        /// <summary>Locator Tool</summary>
        Locator = 1,
        /// <summary>Custom Vision Tool (CVT)</summary>
        CustomVisionTool = 2,
        /// <summary>Blob Analyzer Tool</summary>
        BlobAnalyzer = 3,
        /// <summary>ImageProcessing Tool</summary>
        ImageProcessing = 4,
		/// <summary>Locator Tool Model</summary>
		LocatorModel = 5,
		/// <summary>Edge Locator Tool</summary>
		EdgeLocator = 6,
		/// <summary>Line Finder Tool</summary>
		LineFinder = 7,
		/// <summary>Arc Finder Tool</summary>
		ArcFinder = 8,
		/// <summary>Point Finder Tool</summary>
		PointFinder = 9,
        /// <summary>Calculated Line Tool</summary>
        CalculatedLine=10,
        /// <summary>Calculated Point Tool</summary>
        CalculatedPoint=11,
        ///<summary>Calculated Arc Tool</summary>
        CalculatedArc=12,
        ///<summary>Calculated Frame Tool</summary>
        CalculatedFrame = 13,
        ///<summary>Sub CVT</summary>
        SubCVT=14
    }

	/// <summary>機器手臂姿態</summary>
	public enum ArmPosture : byte {
		/// <summary>未改變或未定義的姿態</summary>
		Undefined = 0,
		/// <summary>左臂</summary>
		Lefty = 1,
		/// <summary>右臂</summary>
		Righty = 2
	}

	/// <summary>可供 <see cref="MarkerOverlayCollection"/> 使用的自定義顏色</summary>
	public enum CustomColor {
		/// <summary>淡藍色</summary>
		LIGHT_BLUE = 0xFF8000,
		/// <summary>酒紅色</summary>
		WINE_RED = 0x6D00D8,
		/// <summary>橄欖/土黃色</summary>
		OLIVE = 0x009898,
		/// <summary>藍綠色</summary>
		TEAL = 0x58B000,
		/// <summary>淡紫色</summary>
		PURPLE = 0xFF8080
	}
}
