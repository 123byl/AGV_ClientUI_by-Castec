using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using CtLib.Library;

using Staubli.Robotics.Soap.Proxies.ServerV0;
using Staubli.Robotics.Soap.Proxies.ServerV1;
using Staubli.Robotics.Soap.Proxies.ServerV2;
using Staubli.Robotics.Soap.Proxies.ServerV3;

namespace CtLib.Module.Staubli {

    /// <summary>Stäubli 機器手臂</summary>
    public class StaubliRobot {

        #region Properties
        /// <summary>取得手臂型號</summary>
        public string Model { get; }
        /// <summary>取得 Tuning</summary>
        public string Tuning { get; }
        /// <summary>取得第三軸直徑</summary>
        public Diameter Axis3Diameter { get; }
        /// <summary>取得第三軸長度</summary>
        public Length Axis3Length { get; }
        /// <summary>取得安裝方式</summary>
        public Mount MountType { get; }
        /// <summary>取得手臂樣式</summary>
        public ArmType Style { get; }
        #endregion

        #region Constructors
        internal StaubliRobot(Robot robot) {
            Model = robot.arm;
            Tuning = robot.tuning;
            Axis3Diameter = (Diameter)robot.diameterAxis3;
            Axis3Length = (Length)robot.lengthAxis3;
            MountType = (Mount)robot.mountType;
            Style = (ArmType)robot.kinematic;
        }
        #endregion

        #region Overrides
        /// <summary>取得此手臂的文字描述</summary>
        /// <returns>文字描述</returns>
        public override string ToString() {
            return $"Model:{Model}, Style:{Style}, 3Axis:{Axis3Diameter}&{Axis3Length}, Mount:{MountType}, Tune:{Tuning}";
        }
        #endregion
    }

    /// <summary>I/O 狀況</summary>
    public class IoState {

        #region Properties
        /// <summary>取得 I/O 位置</summary>
        public string Path { get; }
        /// <summary>取得當前狀態</summary>
        public bool Value { get; }
        /// <summary>取得是否已被鎖定(無法更改)</summary>
        public bool Locked { get; }
        /// <summary>取得是否為模擬中</summary>
        public bool Simulated { get; }
        /// <summary>取得此 I/O 註解描述</summary>
        public string Description { get; }
        /// <summary>取得此 I/O 的定義狀態</summary>
        public IoDefine Define { get; }
        #endregion

        #region Constructors
        internal IoState(SoapPhysicalIoState phyIO) {
            Path = string.Empty;
            Value = phyIO.value > 0.1 ? true : false;
            Locked = phyIO.locked;
            Simulated = phyIO.simulated;
            Description = phyIO.description;
            Define = (IoDefine)phyIO.state;
        }

        internal IoState(string path, SoapPhysicalIoState phyIO) {
            Path = path;
            Value = phyIO.value > 0.1 ? true : false;
            Locked = phyIO.locked;
            Simulated = phyIO.simulated;
            Description = phyIO.description;
            Define = (IoDefine)phyIO.state;
        }
        #endregion

        #region Overrides
        /// <summary>取得此 I/O 的文字描述</summary>
        /// <returns>文字描述</returns>
        public override string ToString() {
            return $"Path:{Path}, {(Value ? "On" : "Off")}, {(Locked ? "Lock" : "Unlock")}, {(Simulated ? "Simulate" : "Actual")}, {Define} Desc:{Description}";
        }
        #endregion
    }

    /// <summary>實體 I/O 資訊</summary>
    public class PhysicalIO {

        #region Properties
        /// <summary>取得實體路徑</summary>
        public string Path { get; }
        /// <summary>取得描述註解</summary>
        public string Description { get; }
        /// <summary>取得 I/O 類型</summary>
        public IoType Type { get; }
        #endregion

        #region Constructors
        internal PhysicalIO(XElement elmt) {
            var attrs = elmt.Attributes();
            foreach (var attr in attrs) {
                switch (attr.Name.LocalName.ToLower()) {
                    case "name":
                    case "path":
                        Path = attr.Value;
                        break;
                    case "desc":
                        Description = attr.Value;
                        break;
                    case "type":
                        Type = attr.Value.ToEnum<IoType>();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Overrides
        /// <summary>取得此 I/O 的文字描述</summary>
        /// <returns>文字描述</returns>
        public override string ToString() {
            return $"Path:{Path}, Type:{Type}, Desc:{Description}";
        }
        #endregion
    }

    /// <summary>VAL3 任務</summary>
    public class VAL3Task {

        #region Properties
        /// <summary>取得任務名稱</summary>
        public string Name { get; }
        /// <summary>取得優先權</summary>
        public int Priority { get; }
        /// <summary>取得建立者名稱</summary>
        public string CreateBy { get; }
        /// <summary>取得執行錯誤</summary>
        public int RuntimeError { get; }
        /// <summary>取得執行錯誤描述</summary>
        public string RuntimeErrorDescription { get; }
        /// <summary>取得當前狀態</summary>
        public TaskState State { get; }
        /// <summary>取得當前執行到的該行程式</summary>
        public ProgramLine Line { get; }
        #endregion

        #region Constructors
        internal VAL3Task(SoapTask tsk) {
            Name = tsk.name;
            Priority = tsk.priority;
            CreateBy = tsk.createdBy;
            RuntimeError = tsk.runtimeError;
            RuntimeErrorDescription = tsk.runtimeErrorDescription;
            State = (TaskState)tsk.state;
            Line = new ProgramLine(tsk.programLine);
        }
        #endregion

        #region Overrides
        /// <summary>取得此任務的文字描述</summary>
        /// <returns>文字描述</returns>
        public override string ToString() {
            return $"Task:{Name}, Pri:{Priority}, State:{State}, Err:{RuntimeError}{(string.IsNullOrEmpty(RuntimeErrorDescription) ? "" : "/" + RuntimeErrorDescription)}, {Line.ToString()}, By:{CreateBy}";
        }
        #endregion
    }

    /// <summary>程式內的單行內容</summary>
    public class ProgramLine {

        #region Properties
        /// <summary>取得隸屬於的 Application 名稱</summary>
        public string Application { get; }
        /// <summary>取得此程式名稱</summary>
        public string Name { get; }
        /// <summary>取得行號</summary>
        public int Line { get; }
        /// <summary>取得該行內容</summary>
        public string Content { get; }
        #endregion

        #region Constructors
        internal ProgramLine(SoapProgramLine prg) {
            Application = prg.appName;
            Name = prg.pgmName;
            Line = prg.lineNumber;
            Content = prg.lineContent;
        }
        #endregion

        #region Overrides
        /// <summary>取得此程式行的文字描述</summary>
        /// <returns>文字描述</returns>
        public override string ToString() {
            return $"App:{Application}, Prg:{Name}, Line:{Line}, Content:{Content}";
        }
        #endregion
    }

    /// <summary>VAL3 應用程式</summary>
    public class VAL3Application {

        #region Properties
        /// <summary>取得應用程式名稱</summary>
        public string Name { get; }
        /// <summary>取得目前是否已載入</summary>
        public bool Loaded { get; }
        #endregion

        #region Constructors
        internal VAL3Application(VALApplication app) {
            Name = app.name;
            Loaded = app.loaded;
        }
        #endregion

        #region Overrides
        /// <summary>取得此應用程式的文字描述</summary>
        /// <returns>文字描述</returns>
        public override string ToString() {
            return $"App:{Name}, {(Loaded ? "Loaded" : "Unloaded")}";
        }
        #endregion
    }

    /// <summary>VAL3 變數類型</summary>
    public enum VAL3VariableType {
        /// <summary>布林值</summary>
        Boolean,
        /// <summary>整數數值</summary>
        Numeric,
        /// <summary>字串</summary>
        String,
        /// <summary>類比 I/O</summary>
        AnalogyIO,
        /// <summary>數位 I/O</summary>
        DigitalIO,
        /// <summary>軟體 I/O</summary>
        SoftwareIO,
        /// <summary>手臂軸(Joint)數值</summary>
        Joint,
        /// <summary>參考的手臂軸(Joint)數值</summary>
        JointReference,
        /// <summary>手臂世界座標(World)數值</summary>
        Point,
        /// <summary>參考的世界座標</summary>
        PointReference,
        /// <summary>Frame</summary>
        Frame,
        /// <summary>Tool</summary>
        Tool,
        /// <summary>Speed Configurations</summary>
        MotionConfig,
        /// <summary>Transform</summary>
        Transform
    }

    /// <summary>VAL3 存取範圍</summary>
    public enum VAL3Access {
        /// <summary>公開</summary>
        Public,
        /// <summary>私有</summary>
        Private
    }

    /// <summary>移動的混和模式</summary>
    public enum BlendMode {
        /// <summary>無混和</summary>
        Off,
        /// <summary>joint</summary>
        Joint,
        /// <summary>混和模式</summary>
        Cartesian
    }

    /// <summary>VAL3 變數數值基底</summary>
    public abstract class VariableValue {

        /// <summary>群組中的索引</summary>
        protected int mIdx = 0;

        /// <summary>取得於群組中的索引值</summary>
        public int Index { get { return mIdx; } }

        /// <summary>由 <see cref="XElement"/> 產生對應的變數</summary>
        /// <param name="elmt">欲判斷的 XML 節點資訊</param>
        /// <param name="varType">對應的變數型態</param>
        /// <returns>VAL3 變數</returns>
        internal static Dictionary<int, VariableValue> Factory(XElement elmt, VAL3VariableType varType) {
            Dictionary<int, VariableValue> retColl = null;
            if (elmt.HasElements) {
                retColl = elmt.Elements()
                    .Select(
                        subElmt => {
                            VariableValue retVal = null;
                            switch (varType) {
                                case VAL3VariableType.Boolean:
                                    retVal = new BoolValue(subElmt);
                                    break;
                                case VAL3VariableType.Numeric:
                                    retVal = new NumericValue(subElmt);
                                    break;
                                case VAL3VariableType.String:
                                    retVal = new StringValue(subElmt);
                                    break;
                                case VAL3VariableType.MotionConfig:
                                    retVal = new MotionValue(subElmt);
                                    break;
                                case VAL3VariableType.AnalogyIO:
                                case VAL3VariableType.DigitalIO:
                                case VAL3VariableType.SoftwareIO:
                                    retVal = new IOLink(subElmt);
                                    break;
                                case VAL3VariableType.Joint:
                                case VAL3VariableType.JointReference:
                                    retVal = new JointValue(subElmt);
                                    break;
                                case VAL3VariableType.Point:
                                case VAL3VariableType.PointReference:
                                case VAL3VariableType.Frame:
                                case VAL3VariableType.Tool:
                                case VAL3VariableType.Transform:
                                    retVal = new PointValue(subElmt);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException("varType", $"Invalid variable type enum value '{(int)varType}'");
                            }
                            return retVal;
                        }
                    ).ToDictionary(
                        varVal => varVal.Index,
                        varVal => varVal
                    );
            } else {
                //避免外面因為 null 跳掉而已
                retColl = new Dictionary<int, VariableValue>();
            }
            return retColl;
        }

        /// <summary>取得此變數數值的文字描述，僅數值描述</summary>
        /// <returns>當前數值</returns>
        public abstract override string ToString();
        /// <summary>取得此變數數值的文字描述，含索引值與數值描述</summary>
        /// <param name="prefix">字串前綴</param>
        /// <returns>索引值與數值描述</returns>
        public abstract string ToFullString(string prefix = "");
    }

    /// <summary>VAL3 <see cref="bool"/> 變數數值</summary>
    public class BoolValue : VariableValue {

        /// <summary>取得此 VAL3 變數數值</summary>
        public bool Value { get; } = false;

        internal BoolValue(XElement elmt) {
            foreach (var attr in elmt.Attributes()) {
                switch (attr.Name.LocalName) {
                    case "key":
                        mIdx = int.Parse(attr.Value);
                        break;
                    case "value":
                        Value = bool.Parse(attr.Value);
                        break;
                }
            }
        }

        /// <summary>取得此變數的文字描述</summary>
        /// <returns>文字描述</returns>
        public override string ToString() {
            return Value.ToString();
        }

        /// <summary>取得此變數數值的文字描述，含索引值與數值描述</summary>3
        /// <param name="prefix">字串前綴</param>
        /// <returns>索引值與數值描述</returns>
        public override string ToFullString(string prefix = "") {
            return $"{prefix}[{mIdx}] {Value}";
        }
    }

    /// <summary>VAL3 <see cref="string"/> 變數數值</summary>
    public class StringValue : VariableValue {

        /// <summary>取得此 VAL3 變數數值</summary>
        public string Value { get; } = string.Empty;

        internal StringValue(XElement elmt) {
            foreach (var attr in elmt.Attributes()) {
                switch (attr.Name.LocalName) {
                    case "key":
                        mIdx = int.Parse(attr.Value);
                        break;
                    case "value":
                        Value = attr.Value;
                        break;
                }
            }
        }

        /// <summary>取得此變數的文字描述</summary>
        /// <returns>文字描述</returns>
        public override string ToString() {
            return Value;
        }

        /// <summary>取得此變數數值的文字描述，含索引值與數值描述</summary>
        /// <param name="prefix">字串前綴</param>
        /// <returns>索引值與數值描述</returns>
        public override string ToFullString(string prefix = "") {
            return $"{prefix}[{mIdx}] {Value}";
        }
    }

    /// <summary>VAL3 IO 變數對應</summary>
    public class IOLink : VariableValue {

        /// <summary>取得此 VAL3 變數數值</summary>
        public string Link { get; } = string.Empty;

        internal IOLink(XElement elmt) {
            foreach (var attr in elmt.Attributes()) {
                switch (attr.Name.LocalName) {
                    case "key":
                        mIdx = int.Parse(attr.Value);
                        break;
                    case "link":
                        Link = attr.Value;
                        break;
                }
            }
        }

        /// <summary>取得此變數的文字描述</summary>
        /// <returns>文字描述</returns>
        public override string ToString() {
            return Link;
        }

        /// <summary>取得此變數數值的文字描述，含索引值與數值描述</summary>
        /// <param name="prefix">字串前綴</param>
        /// <returns>索引值與數值描述</returns>
        public override string ToFullString(string prefix = "") {
            return $"{prefix}[{mIdx}] {Link}";
        }
    }

    /// <summary>VAL3 <see cref="int"/> 變數數值</summary>
    public class NumericValue : VariableValue {

        /// <summary>取得此 VAL3 變數數值</summary>
        public int Value { get; } = 0;

        internal NumericValue(XElement elmt) {
            foreach (var attr in elmt.Attributes()) {
                switch (attr.Name.LocalName) {
                    case "key":
                        mIdx = int.Parse(attr.Value);
                        break;
                    case "value":
                        Value = int.Parse(attr.Value);
                        break;
                }
            }
        }

        /// <summary>取得此變數的文字描述</summary>
        /// <returns>文字描述</returns>
        public override string ToString() {
            return Value.ToString();
        }

        /// <summary>取得此變數數值的文字描述，含索引值與數值描述</summary>
        /// <param name="prefix">字串前綴</param>
        /// <returns>索引值與數值描述</returns>
        public override string ToFullString(string prefix = "") {
            return $"{prefix}[{mIdx}] {Value}";
        }
    }

    /// <summary>VAL3 軸座標(Joint)變數數值</summary>
    public class JointValue : VariableValue {

        /// <summary>取得 Joint 1 數值</summary>
        public double J1 { get; } = 0;
        /// <summary>取得 Joint 2 數值</summary>
        public double J2 { get; } = 0;
        /// <summary>取得 Joint 3 數值</summary>
        public double J3 { get; } = 0;
        /// <summary>取得 Joint 4 數值</summary>
        public double J4 { get; } = 0;
        /// <summary>取得 Joint 5 數值</summary>
        public double J5 { get; } = 0;
        /// <summary>取得 Joint 6 數值</summary>
        public double J6 { get; } = 0;
        /// <summary>參考的父層名稱</summary>
        public string Father { get; } = string.Empty;

        internal JointValue(XElement elmt) {
            foreach (var attr in elmt.Attributes()) {
                switch (attr.Name.LocalName) {
                    case "key":
                        mIdx = int.Parse(attr.Value);
                        break;
                    case "j1":
                        J1 = double.Parse(attr.Value);
                        break;
                    case "j2":
                        J2 = double.Parse(attr.Value);
                        break;
                    case "j3":
                        J3 = double.Parse(attr.Value);
                        break;
                    case "j4":
                        J4 = double.Parse(attr.Value);
                        break;
                    case "j5":
                        J5 = double.Parse(attr.Value);
                        break;
                    case "j6":
                        J6 = double.Parse(attr.Value);
                        break;
                    case "fatherId":
                        Father = attr.Value;
                        break;
                }
            }
        }

        /// <summary>取得所有的 Joint 數值</summary>
        /// <returns>Joint 數值</returns>
        public Dictionary<int, double> JointValues() {
            return new Dictionary<int, double> {
                { 1, J1 }, { 2, J2 }, { 3, J3 },
                { 4, J4 }, { 5, J5 }, { 6, J6 }
            };
        }

        /// <summary>取得此變數的文字描述</summary>
        /// <returns>文字描述</returns>
        public override string ToString() {
            return $"{J1:F6}, {J2:F6}, {J3:F6}, {J4:F6}, {J5:F6}, {J6:F6}";
        }

        /// <summary>取得此變數數值的文字描述，含索引值與數值描述</summary>
        /// <returns>索引值與數值描述</returns>
        public override string ToFullString(string prefix = "") {
            var str = $"{prefix}[{mIdx}] {J1:F6}, {J2:F6}, {J3:F6}, {J4:F6}, {J5:F6}, {J6:F6}";
            if (!string.IsNullOrEmpty(Father)) str += $", {Father}";
            return str;
        }
    }

    /// <summary>VAL3 世界座標(World)變數</summary>
    public class PointValue : VariableValue {

        /// <summary>取得 X 數值</summary>
        public double X { get; } = 0;
        /// <summary>取得 Y 數值</summary>
        public double Y { get; } = 0;
        /// <summary>取得 Z 數值</summary>
        public double Z { get; } = 0;
        /// <summary>取得 Yaw(rX) 數值</summary>
        public double Yaw { get; } = 0;
        /// <summary>取得 Pitch(rY) 數值</summary>
        public double Pitch { get; } = 0;
        /// <summary>取得 Roll(rZ) 數值</summary>
        public double Roll { get; } = 0;
        /// <summary>取得參考的父層名稱</summary>
        public string Father { get; } = string.Empty;
        /// <summary>取得 ioLink</summary>
        public string IOLink { get; } = string.Empty;

        internal PointValue(XElement elmt) {
            foreach (var attr in elmt.Attributes()) {
                switch (attr.Name.LocalName) {
                    case "key":
                        mIdx = int.Parse(attr.Value);
                        break;
                    case "x":
                        X = double.Parse(attr.Value);
                        break;
                    case "y":
                        Y = double.Parse(attr.Value);
                        break;
                    case "z":
                        Z = double.Parse(attr.Value);
                        break;
                    case "rx":
                        Yaw = double.Parse(attr.Value);
                        break;
                    case "ry":
                        Pitch = double.Parse(attr.Value);
                        break;
                    case "rz":
                        Roll = double.Parse(attr.Value);
                        break;
                    case "fatherId":
                        Father = attr.Value;
                        break;
                    case "ioLink":
                        IOLink = attr.Value;
                        break;
                }
            }
        }

        /// <summary>取得所有的座標數值</summary>
        /// <returns>座標數值</returns>
        public Dictionary<string, double> JointValues() {
            return new Dictionary<string, double> {
                { "X", X }, { "Y", Y }, { "Z", Z },
                { "Yaw", Yaw }, { "Pitch", Pitch }, { "Roll", Roll }
            };
        }
        /// <summary>取得此變數的文字描述</summary>
        /// <returns>文字描述</returns>
        public override string ToString() {
            return $"{X:F6}, {Y:F6}, {Z:F6}, {Yaw:F6}, {Pitch:F6}, {Roll:F6}";
        }

        /// <summary>取得此變數數值的文字描述，含索引值與數值描述</summary>
        /// <param name="prefix">字串前綴</param>
        /// <returns>索引值與數值描述</returns>
        public override string ToFullString(string prefix = "") {
            var str = $"{prefix}[{mIdx}] {X:F6}, {Y:F6}, {Z:F6}, {Yaw:F6}, {Pitch:F6}, {Roll:F6}";
            if (!string.IsNullOrEmpty(Father)) str += $", {Father}";
            if (!string.IsNullOrEmpty(IOLink)) str += $", {IOLink}";
            return str;
        }
    }

    /// <summary>VAL3 運動定義</summary>
    public class MotionValue : VariableValue {

        /// <summary>取得最大允許關節速度，以機器人的額定速度的 % 表示</summary>
        public int Velocity { get; } = 0;
        /// <summary>取得最大允許關節加速度，以機器人的額定加速度的 % 表示</summary>
        public int Accel { get; } = 0;
        /// <summary>取得最大允許關節減速度，以機器人的額定減速度的 % 表示</summary>
        public int Decel { get; } = 0;
        /// <summary>取得在混合模式 joint 和 Cartesian 中，在混合開始的目標點和下一個目標點之間的距離，根據軟體應用的長度單位用毫米或英寸表示</summary>
        public double Leave { get; } = 0;
        /// <summary>取得在混合模式 joint 和 Cartesian 中，在混合停止的目標點和下一個目標點之間的距離，根據軟體應用的長度單位用毫米或英寸表示</summary>
        public double Reach { get; } = 0;
        /// <summary>取得工具中心點的最大允許旋轉速度，用度 / 秒表示</summary>
        public double RotateMaxVelocity { get; } = 0;
        /// <summary>取得工具中心點的最大允許平移速度，根據軟體應用單位長度用 "毫米/秒" 或 "英寸/秒" 表示</summary>
        public double TranslateMaxVelocity { get; } = 0;
        /// <summary>取得混合模式</summary>
        public BlendMode Blend { get; } = BlendMode.Off;

        internal MotionValue(XElement elmt) {
            foreach (var attr in elmt.Attributes()) {
                switch (attr.Name.LocalName) {
                    case "key":
                        mIdx = int.Parse(attr.Value);
                        break;
                    case "vel":
                        Velocity = int.Parse(attr.Value);
                        break;
                    case "accel":
                        Accel = int.Parse(attr.Value);
                        break;
                    case "decel":
                        Decel = int.Parse(attr.Value);
                        break;
                    case "leave":
                        Leave = double.Parse(attr.Value);
                        break;
                    case "reach":
                        Reach = double.Parse(attr.Value);
                        break;
                    case "rmax":
                        RotateMaxVelocity = double.Parse(attr.Value);
                        break;
                    case "tmax":
                        TranslateMaxVelocity = double.Parse(attr.Value);
                        break;
                    case "blend":
                        Blend = attr.Value.ToEnum<BlendMode>();
                        break;
                }
            }
        }

        /// <summary>取得此變數的文字描述</summary>
        /// <returns>文字描述</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            if (Velocity > 0) sb.AppendFormat("[Velo]{0}", Velocity);
            if (Accel > 0) sb.AppendFormat(" [Acel]{0}", Accel);
            if (Decel > 0) sb.AppendFormat(" [Dcel]{0}", Decel);
            if (Reach > 0) sb.AppendFormat(" [Rech]{0}", Reach);
            if (Leave > 0) sb.AppendFormat(" [Leav]{0}", Leave);
            if (RotateMaxVelocity > 0) sb.AppendFormat(" [RVel]{0}", RotateMaxVelocity);
            if (TranslateMaxVelocity > 0) sb.AppendFormat(" [TVel]{0}", TranslateMaxVelocity);
            sb.AppendFormat(" [Blnd]{0}", Blend);
            return sb.ToString();
        }

        /// <summary>取得此變數數值的文字描述，含索引值與數值描述</summary>
        /// <param name="prefix">字串前綴</param>
        /// <returns>索引值與數值描述</returns>
        public override string ToFullString(string prefix = "") {
            return $"{prefix}[{mIdx}] {this.ToString()}";
        }
    }

    /// <summary>VAL3 變數基底</summary>
    public class VAL3Variable {

        /// <summary>變數名稱</summary>
        protected string mName = string.Empty;
        /// <summary>數值集合</summary>
        protected Dictionary<int, VariableValue> mValues;

        /// <summary>取得此變數名稱</summary>
        public string Name { get { return mName; } }
        /// <summary>取得此變數的存取範圍</summary>
        public VAL3Access Access { get; }
        /// <summary>取得此變數為何者型態</summary>
        public VAL3VariableType VariableType { get; }

        internal VAL3Variable(XElement elmt) {
            mName = elmt.Attribute("name").Value;
            Access = elmt.Attribute("access").Value.ToEnum<VAL3Access>();
            VariableType = GetVarType(elmt.Attribute("type").Value);
            mValues = VariableValue.Factory(elmt, VariableType);
        }

        private VAL3VariableType GetVarType(string typeStr) {
            switch (typeStr) {
                case "bool":
                    return VAL3VariableType.Boolean;
                case "num":
                    return VAL3VariableType.Numeric;
                case "string":
                    return VAL3VariableType.String;
                case "pointRx":
                    return VAL3VariableType.Point;
                case "pointRs":
                    return VAL3VariableType.PointReference;
                case "jointRx":
                    return VAL3VariableType.Joint;
                case "jointRs":
                    return VAL3VariableType.JointReference;
                case "frame":
                    return VAL3VariableType.Frame;
                case "tool":
                    return VAL3VariableType.Tool;
                case "dio":
                    return VAL3VariableType.DigitalIO;
                case "aio":
                    return VAL3VariableType.AnalogyIO;
                case "sio":
                    return VAL3VariableType.SoftwareIO;
                case "mdesc":
                    return VAL3VariableType.MotionConfig;
                case "trsf":
                    return VAL3VariableType.Transform;
                default:
                    throw new ArgumentException($"Can not analysis VAL3 variable type '{typeStr}'", "typeStr");
            }
        }

        private string GetVarType(VAL3VariableType varType) {
            switch (varType) {
                case VAL3VariableType.Boolean:
                    return "bool";
                case VAL3VariableType.Numeric:
                    return "num";
                case VAL3VariableType.String:
                    return "string";
                case VAL3VariableType.AnalogyIO:
                    return "aio";
                case VAL3VariableType.DigitalIO:
                    return "dio";
                case VAL3VariableType.SoftwareIO:
                    return "sio";
                case VAL3VariableType.Joint:
                    return "jointRx";
                case VAL3VariableType.JointReference:
                    return "jointRs";
                case VAL3VariableType.Point:
                    return "pointRx";
                case VAL3VariableType.PointReference:
                    return "pointRs";
                case VAL3VariableType.Frame:
                    return "frame";
                case VAL3VariableType.Tool:
                    return "tool";
                case VAL3VariableType.MotionConfig:
                    return "mdesc";
                case VAL3VariableType.Transform:
                    return "trsf";
                default:
                    throw new ArgumentException($"Can not analysis VAL3 variable type '{(int)varType}'", "varType");
            }
        }

        /// <summary>取得數值</summary>
        /// <typeparam name="TVar">繼承於 <see cref="VariableValue"/> 之 VAL3 變數數值</typeparam>
        /// <param name="index">於群組內的索引，從 0 開始</param>
        /// <returns>VAL3 變數數值</returns>
        public TVar GetValue<TVar>(int index) where TVar : VariableValue {
            if (mValues.ContainsKey(index)) {
                return (TVar)mValues[index];
            } else {
                return null;
            }
        }

        /// <summary>取得此變數的文字描述</summary>
        /// <returns>文字描述</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine($"{mName}, {GetVarType(VariableType)}, {Access}");
            if (mValues.Count > 0) {
                mValues.ForEach(val => sb.AppendLine(val.Value.ToFullString("\t")));
            }
            return sb.ToString();
        }
    }
}
