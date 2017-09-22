using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CtLib.Library;

using Ace.Adept.Server.Controls;
using Ace.Adept.Server.Motion.Robots;
using Ace.Core.Server;
using Ace.Adept.Server.Desktop;
using Ace.Adept.Server.Desktop.Connection;
using Ace.Adept.Server.Motion;

namespace CtLib.Module.Adept {

    /// <summary>
    /// Adept Robot 相關移動控制模組
    /// <para>包含移動至點位、單軸移動等相關移動命令</para>
    /// </summary>
    public class CtAceMotion {

        #region Declaration - Definitons
        /// <summary>預設加減速。依序為: 加速、減速、速度</summary>
        private static readonly int[] DEFAULT_ACCEL = { 20, 20, 60 };
        #endregion

        #region Declaration - Members
        /// <summary>是否含有SmartController</summary>
        private bool mWithCtrl;
        /// <summary>V+相關物件連結，用於取得變數與Robot現在位置</summary>
        private IVpLink mVpLink;
        /// <summary>AceServer，用於取得相關 AceObject</summary>
        private IAceServer mIServer;
        /// <summary>SmartController，用於取得變數與Robot並控制移動</summary>
        private IAdeptController mICtrl;
        /// <summary>i-Cobra Robot</summary>
        private iCobra miCobra;
        /// <summary>暫存 CartesianMove</summary>
        private CartesianMove mCsMove;
        #endregion

        #region Function - Constructors
        /// <summary>建立Robot移動控制模組</summary>
        /// <param name="vpLink">V+相關物件連結，用於取得變數與Robot現在位置</param>
        /// <param name="aceSrv">AceServer，用於取得相關 AceObject</param>
        /// <param name="ctrl">SmartController 或 iCobra 物件，請視有無控制器帶入相對應物件</param>
        public CtAceMotion(IVpLink vpLink, IAceServer aceSrv, object ctrl) {
            mVpLink = vpLink;
            mIServer = aceSrv;

            if (ctrl is IAdeptController) {
                mICtrl = ctrl as IAdeptController;
                mWithCtrl = true;
            } else {
                miCobra = ctrl as iCobra;
                mWithCtrl = false;
            }
        }
        #endregion

        #region Function - Moving
        /// <summary>以當前位置為基礎，移動特定距離</summary>
        /// <param name="robotNum">手臂編號。預設為 1 開始</param>
        /// <param name="axis">欲移動的軸或方向</param>
        /// <param name="distance">欲移動距離</param>
        public void MoveDistance(int robotNum, CtAce.Axis axis, double distance) {
            double[] decompose = mVpLink.WhereWorld(robotNum).ToArray();
            switch (axis) {
                case CtAce.Axis.X:
                    decompose[0] += distance;
                    break;
                case CtAce.Axis.Y:
                    decompose[1] += distance;
                    break;
                case CtAce.Axis.Z:
                    if (decompose.Length > 2) decompose[2] += distance;
                    else throw (new Exception("此 Robot 無法移動 Z 軸"));
                    break;
                case CtAce.Axis.YAW:
                    if (decompose.Length > 3) decompose[3] += distance;
                    else throw (new Exception("此 Robot 無法移動 Yaw 軸"));
                    break;
                case CtAce.Axis.PITCH:
                    if (decompose.Length > 4) decompose[4] += distance;
                    else throw (new Exception("此 Robot 無法移動 Pitch 軸"));
                    break;
                case CtAce.Axis.ROLL:
                    if (decompose.Length > 5) decompose[5] += distance;
                    else throw (new Exception("此 Robot 無法移動 Roll(Theta) 軸"));
                    break;
            }

            /*-- 建立Transform3D並移動 --*/
            Transform3D loc = null;
            if (decompose.Length == 3)
                loc = new Transform3D(decompose[0], decompose[1], decompose[2]);
            else if (decompose.Length == 6)
                loc = new Transform3D(decompose[0], decompose[1], decompose[2], decompose[3], decompose[4], decompose[5]);

            if (loc != null) mVpLink.Moves(loc);
        }

        /// <summary>設定 CartesianMove 全域變數</summary>
        /// <param name="accel">加減速配方。依序為 加速、減速、穩態速度</param>
        /// <param name="motionMode">移動路徑模式，直線或各軸內插</param>
        /// <param name="speedMode">速度單位，IPS、MMPS、Percent</param>
        /// <param name="scurfProfile">加減速曲線，從0到8。請參考Adept ACE Controller S-Curve Profile setting utility</param>
        /// <param name="moveMode">各點間之到點模式，Blend、NoNull、Coarse、Fine</param>
        /// <param name="percent">Coarse、Fine之到點百分比，其餘模式請忽略</param>
        public void CreateCartesianMove(int[] accel,
                                        CtAce.MoveEndMode moveMode = CtAce.MoveEndMode.NONULL,
                                        CtAce.MotionMode motionMode = CtAce.MotionMode.STRIGHT_LINE,
                                        CtAce.SpeedMode speedMode = CtAce.SpeedMode.PERCENT,
                                        int scurfProfile = 0,
                                        int percent = 80) {
            if (mCsMove == null) mCsMove = mIServer.CreateObject(typeof(CartesianMove)) as CartesianMove;
            mCsMove.Param.Accel = accel[0];
            mCsMove.Param.Decel = accel[1];
            mCsMove.Param.Speed = accel[2];
            mCsMove.Param.MotionEnd = (MotionEnd)moveMode;   //完整到點
            mCsMove.Param.SpeedMode = (MoveSpeedMode)speedMode;
            mCsMove.Param.Straight = (motionMode == CtAce.MotionMode.STRIGHT_LINE) ? true : false;  //直線運動
            mCsMove.Param.SCurveProfile = scurfProfile;   //加減速曲線
            mCsMove.Param.SettlePercent = percent;
        }

        /// <summary>移動至特定座標(單點)</summary>
        /// <param name="robotNum">手臂編號。預設為 1 開始</param>
        /// <param name="location">目標座標</param>
        /// <param name="accel">加減速配方。依序為 加速、減速、穩態速度</param>
        /// <param name="varType">座標型態。Location或PrecisionPoint，兩者不相容！</param>
        /// <param name="motionMode">移動路徑模式，直線或各軸內插</param>
        /// <param name="speedMode">速度單位，IPS、MMPS、Percent</param>
        /// <param name="scurfProfile">加減速曲線，從0到8。請參考Adept ACE Controller S-Curve Profile setting utility</param>
        public void MoveToLocation(int robotNum, List<double> location, int[] accel, CtAce.VPlusVariableType varType = CtAce.VPlusVariableType.LOCATION, CtAce.MotionMode motionMode = CtAce.MotionMode.STRIGHT_LINE, CtAce.SpeedMode speedMode = CtAce.SpeedMode.PERCENT, int scurfProfile = 0) {

            /*-- 檢查相關屬性 --*/
            if (accel.Length != 3) throw (new Exception("加減速設定數量錯誤"));

            /*-- 修改 Cartesian 物件 --*/
            IAdeptRobot robot = mWithCtrl ? mICtrl.GetRobot(robotNum) : miCobra as IAdeptRobot;
            CreateCartesianMove(accel, CtAce.MoveEndMode.NONULL, motionMode, speedMode, scurfProfile);
            mCsMove.Robot = robot;

            /*-- 建立 Transform3D --*/
            Transform3D loc = null;
            if (varType == CtAce.VPlusVariableType.LOCATION) {
                if (location.Count == 3)
                    loc = new Transform3D(location[0], location[1], location[2]);
                else if (location.Count == 6)
                    loc = new Transform3D(location[0], location[1], location[2], location[3], location[4], location[5]);
                else throw (new Exception("點位數量錯誤"));

            } else if (varType == CtAce.VPlusVariableType.PRECISION_POINT)
                loc = robot.JointToWorld(location.ToArray());

            mCsMove.WorldLocation = loc;

            int range = robot.InRange(loc);
            if (range == 0) {
                mCsMove.MoveConfiguration = robot.GetMoveConfiguration(robot.JointPosition);
                robot.Move(mCsMove);
            } else throw (new Exception("目標點位將造成第 " + range + " 軸超出範圍！"));
        }

        /// <summary>移動至特定座標(單點)</summary>
        /// <param name="robotNum">手臂編號。預設為 1 開始</param>
        /// <param name="location">目標座標</param>
        /// <param name="varType">座標型態。Location或PrecisionPoint，兩者不相容！</param>
        /// <param name="motionMode">移動路徑模式，直線或各軸內插</param>
        /// <param name="speedMode">速度單位，IPS、MMPS、Percent</param>
        /// <param name="scurfProfile">加減速曲線，從0到8。請參考Adept ACE Controller S-Curve Profile setting utility</param>
        public void MoveToLocation(int robotNum, List<double> location, CtAce.VPlusVariableType varType = CtAce.VPlusVariableType.LOCATION, CtAce.MotionMode motionMode = CtAce.MotionMode.STRIGHT_LINE, CtAce.SpeedMode speedMode = CtAce.SpeedMode.PERCENT, int scurfProfile = 0) {
            MoveToLocation(robotNum, location.ToList(), DEFAULT_ACCEL, varType, motionMode, speedMode, scurfProfile);
        }

        /// <summary>移動至特定座標(連續多點)</summary>
        /// <param name="robotNum">手臂編號。預設為 1 開始</param>
        /// <param name="location">目標座標集合，TKey=該點順序 TValue=座標集合，如第一點 (0 /*0為第一點*/, List(Of double){300, -200, 270, 0, 180, 90})</param>
        /// <param name="accel">加減速配方。依序為 加速、減速、穩態速度</param>
        /// <param name="varType">座標型態。Location或PrecisionPoint，兩者不相容！</param>
        /// <param name="motionMode">移動路徑模式，直線或各軸內插</param>
        /// <param name="moveMode">各點間之到點模式，Blend、NoNull、Coarse、Fine</param>
        /// <param name="percent">Coarse、Fine之到點百分比，其餘模式請忽略</param>
        /// <param name="speedMode">速度單位，IPS、MMPS、Percent</param>
        /// <param name="scurfProfile">加減速曲線，從0到8。請參考Adept ACE Controller S-Curve Profile setting utility</param>
        public void MoveToLocation(int robotNum, Dictionary<int, List<double>> location, int[] accel, CtAce.VPlusVariableType varType = CtAce.VPlusVariableType.LOCATION, CtAce.MotionMode motionMode = CtAce.MotionMode.STRIGHT_LINE, CtAce.MoveEndMode moveMode = CtAce.MoveEndMode.NONULL, int percent = 80, CtAce.SpeedMode speedMode = CtAce.SpeedMode.PERCENT, int scurfProfile = 0) {

            /*-- 檢查相關屬性 --*/
            if (accel.Length != 3) throw (new Exception("加減速設定數量錯誤"));

            /*-- 修改 Cartesian 物件 --*/
            IAdeptRobot robot = mWithCtrl ? mICtrl.GetRobot(robotNum) : miCobra as IAdeptRobot;
            CreateCartesianMove(accel, moveMode, motionMode, speedMode, scurfProfile, percent);
            mCsMove.Robot = robot;

            /*-- 建立 Transform3D --*/
            List<Transform3D> locs = new List<Transform3D>();

            for (int idx = 0; idx < location.Count; idx++) {
                if (location.ContainsKey(idx)) {
                    if (varType == CtAce.VPlusVariableType.LOCATION) {
                        if (location[idx].Count == 3) locs.Add(new Transform3D(location[idx][0], location[idx][1], location[idx][2]));
                        else if (location[idx].Count == 6) locs.Add(new Transform3D(location[idx][0], location[idx][1], location[idx][2], location[idx][3], location[idx][4], location[idx][5]));
                        else break;
                    } else locs.Add(robot.JointToWorld(location[idx].ToArray()));
                }
            }

            /*-- 檢查是否可到達點位，確認後進行移動。因想要知道是第幾點壞掉，所以特別使用 for 取代 foreach --*/
            int range = -1;
            for (int idx = 0; idx < locs.Count; idx++) {
                range = robot.InRange(locs[idx]);
                if (range == 0) {
                    mCsMove.MoveConfiguration = robot.GetMoveConfiguration(robot.JointPosition);
                    mCsMove.WorldLocation = locs[idx];
                    robot.Move(mCsMove);
                } else
                    throw (new Exception("欲移動至第 " + idx + " 點時，此點位第 " + range + " 軸將超出範圍"));
                robot.WaitMoveDone();
            }

        }

        /// <summary>移動至特定座標(連續多點)</summary>
        /// <param name="robotNum">手臂編號。預設為 1 開始</param>
        /// <param name="location">目標座標集合，TKey=該點順序 TValue=座標集合，如第一點 (1, List(Of double){300, -200, 270, 0, 180, 90})</param>
        /// <param name="varType">座標型態。Location或PrecisionPoint，兩者不相容！</param>
        /// <param name="motionMode">移動路徑模式，直線或各軸內插</param>
        /// <param name="moveMode">各點間之到點模式，Blend、NoNull、Coarse、Fine</param>
        /// <param name="percent">Coarse、Fine之到點百分比，其餘模式請忽略</param>
        /// <param name="speedMode">速度單位，IPS、MMPS、Percent</param>
        /// <param name="scurfProfile">加減速曲線，從0到8。請參考Adept ACE Controller S-Curve Profile setting utility</param>
        public void MoveToLocation(int robotNum, Dictionary<int, List<double>> location, CtAce.VPlusVariableType varType = CtAce.VPlusVariableType.LOCATION, CtAce.MotionMode motionMode = CtAce.MotionMode.STRIGHT_LINE, CtAce.MoveEndMode moveMode = CtAce.MoveEndMode.NONULL, int percent = 80, CtAce.SpeedMode speedMode = CtAce.SpeedMode.PERCENT, int scurfProfile = 0) {
            MoveToLocation(robotNum, location, DEFAULT_ACCEL, varType, motionMode, moveMode, percent, speedMode, scurfProfile);
        }

        /// <summary>移動至特定座標(變數、單點)</summary>
        /// <param name="robotNum">手臂編號。預設為 1 開始</param>
        /// <param name="varName">變數名稱，如是 PrecisionPoint(Joint) 請帶入 "#" 字號，如 @"#loc.safe"</param>
        /// <param name="varType">座標型態。Location或PrecisionPoint，兩者不相容！</param>
        /// <param name="motionMode">移動路徑模式，直線或各軸內插</param>
        /// <param name="speedMode">速度單位，IPS、MMPS、Percent</param>
        /// <param name="scurfProfile">加減速曲線，從0到8。請參考Adept ACE Controller S-Curve Profile setting utility</param>
        public void MoveToLocation(int robotNum, string varName, CtAce.VPlusVariableType varType = CtAce.VPlusVariableType.LOCATION, CtAce.MotionMode motionMode = CtAce.MotionMode.STRIGHT_LINE, CtAce.SpeedMode speedMode = CtAce.SpeedMode.PERCENT, int scurfProfile = 0) {
            List<double> loc = null;
            switch (varType) {
                case CtAce.VPlusVariableType.LOCATION:
                    loc = mVpLink.ListL(varName).ToArray().ToList();
                    break;
                case CtAce.VPlusVariableType.PRECISION_POINT:
                    loc = ConvertPP2Double(mVpLink.ListPP(varName));
                    break;
                default:
                    throw (new Exception("不支援的變數型態"));
            }
            MoveToLocation(robotNum, loc, DEFAULT_ACCEL, varType, motionMode, speedMode, scurfProfile);
        }

        /// <summary>
        /// Jog 移動。以指定的 Axis 或 Joint 為主 (均以 0 為起始，亦可使用 CtAce.Axis 與 CtAce.Joint 進行轉換)
        /// <para>速度給予正值則將往正方向移動，反之則往負方向移動</para>
        /// </summary>
        /// <param name="robotNum">手臂編號。預設為 1 開始</param>
        /// <param name="mode">移動模式</param>
        /// <param name="speed">速度。範圍為 -1~1，以百分比為主</param>
        /// <param name="axis">軸。World (0)X (1)Y... || Joint (0)J1 (1)J2...</param>
        /// <example><code>
        /// /*-- 往 「負 X」(X減) 方向移動 --*/
        /// ace.Motion.Jog(1, CtAce.JogMode.WORLD, -0.8, (int)CtAce.Axis.X);
        /// </code></example>
        public void Jog(int robotNum, CtAce.JogMode mode, double speed, int axis) {
            if (mVpLink != null) {
                IAdeptRobot robot = mWithCtrl ? mICtrl.GetRobot(robotNum) : miCobra as IAdeptRobot;
                robot.Jog((ManualControlMode)mode, speed, axis, null);
                /* 測試發現不需 .WaitMoveDone ... 可能預設是 80% 往下吧 ?! */
            }
        }

        /// <summary>Jog 移動。指定座標移動特定距離 (正負方向由 distance 決定)</summary>
        /// <param name="robotNum">手臂編號。預設為 1 開始</param>
        /// <param name="distance">移動距離。正值往正方向移動，負值則相反</param>
        /// <param name="axis">坐標軸</param>
        /// <example><code>
        /// /*-- 往 Y 負的方向移動 0.5mm --*/
        /// ace.Motion.Jog(1, CtAce.Axis.Y, -0.5);
        /// </code></example>
        public void Jog(int robotNum, CtAce.Axis axis, double distance) {
            if (mVpLink != null) {

                double[] decompose = mVpLink.WhereWorld(robotNum).ToArray();
                switch (axis) {
                    case CtAce.Axis.X:
                        decompose[0] += distance;
                        break;
                    case CtAce.Axis.Y:
                        decompose[1] += distance;
                        break;
                    case CtAce.Axis.Z:
                        if (decompose.Length > 2) decompose[2] += distance;
                        else throw (new Exception("此 Robot 無法移動 Z 軸"));
                        break;
                    case CtAce.Axis.YAW:
                        if (decompose.Length > 3) decompose[3] += distance;
                        else throw (new Exception("此 Robot 無法移動 Yaw 軸"));
                        break;
                    case CtAce.Axis.PITCH:
                        if (decompose.Length > 4) decompose[4] += distance;
                        else throw (new Exception("此 Robot 無法移動 Pitch 軸"));
                        break;
                    case CtAce.Axis.ROLL:
                        if (decompose.Length > 5) decompose[5] += distance;
                        else throw (new Exception("此 Robot 無法移動 Roll(Theta) 軸"));
                        break;
                }

                IAdeptRobot robot = mWithCtrl ? mICtrl.GetRobot(robotNum) : miCobra as IAdeptRobot;
                robot.Jog(ManualControlMode.JogToWorldLocation, 1, -1, decompose);
                /* 測試發現不需 .WaitMoveDone ... 可能預設是 80% 往下吧 ?! */
            }
        }

        /// <summary>Jog 移動至特定 World 座標</summary>
        /// <param name="robotNum">手臂編號。預設為 1 開始</param>
        /// <param name="speed">速度。範圍為 -1~1，以百分比為主</param>
        /// <param name="location">目的座標(World)</param>
        /// <remarks>因 ManualControlMode.JogToJointLocation 會跳 Invalid Jog Mode，尚不知道原因，故先取消 Joint 特定座標移動</remarks>
        /// <example><code>
        /// /*-- 使用 Jog 方式以 80% 的速度移動至座標 {100, 200, 370, 0, 180, 90} --*/
        /// ace.Motion.Jog(1, 0.8, new List&lt;double&gt; {100, 200, 370, 0, 180, 90});
        /// </code></example>
        public void Jog(int robotNum, double speed, IEnumerable<double> location) {
            if (mVpLink != null) {

                if (location.Count() != 6) throw (new Exception("點位輸入錯誤"));

                IAdeptRobot robot = mWithCtrl ? mICtrl.GetRobot(robotNum) : miCobra as IAdeptRobot;
                robot.Jog(ManualControlMode.JogToWorldLocation, speed, -1, location.ToArray());
                /* 測試發現不需 .WaitMoveDone ... 可能預設是 80% 往下吧 ?! */
            }
        }

        /// <summary>
        /// Jog 移動。以指定的 Axis 或 Joint 為主，並回傳最後座標 (均以 0 為起始，亦可使用 CtAce.Axis 與 CtAce.Joint 進行轉換)
        /// <para>速度給予正值則將往正方向移動，反之則往負方向移動</para>
        /// </summary>
        /// <param name="robotNum">手臂編號。預設為 1 開始</param>
        /// <param name="mode">移動模式</param>
        /// <param name="speed">速度。範圍為 -1~1，以百分比為主</param>
        /// <param name="axis">軸。World (0)X (1)Y... || Joint (0)J1 (1)J2...</param>
        /// <param name="currLoc">回傳最後座標</param>
        /// <example><code>
        /// /*-- 往 「負 X」(X減) 方向移動 --*/
        /// ace.Motion.Jog(1, CtAce.JogMode.WORLD, -0.8, (int)CtAce.Axis.X);
        /// </code></example>
        public void Jog(int robotNum, CtAce.JogMode mode, double speed, int axis, out List<double> currLoc) {
            if (mVpLink != null) {
                IAdeptRobot robot = mWithCtrl ? mICtrl.GetRobot(robotNum) : miCobra as IAdeptRobot;

                /* 測試發現不需 .WaitMoveDone ... 可能預設是 80% 往下吧 ?! */
                robot.Jog((ManualControlMode)mode, speed, axis, null);
            }
            currLoc = mode == CtAce.JogMode.JOINT ? ConvertPP2Double(mVpLink.WhereJoint(robotNum)) : mVpLink.WhereWorld(robotNum).ToArray().ToList();
        }

        /// <summary>Jog 移動。指定座標移動特定距離 (正負方向由 distance 決定)</summary>
        /// <param name="robotNum">手臂編號。預設為 1 開始</param>
        /// <param name="distance">移動距離。正值往正方向移動，負值則相反</param>
        /// <param name="axis">坐標軸</param>
        /// <param name="currLoc">回傳最後座標</param>
        /// <example><code>
        /// /*-- 往 Y 負的方向移動 0.5mm --*/
        /// ace.Motion.Jog(1, CtAce.Axis.Y, -0.5);
        /// </code></example>
        public void Jog(int robotNum, CtAce.Axis axis, double distance, out List<double> currLoc) {
            if (mVpLink != null) {

                double[] decompose = mVpLink.WhereWorld(robotNum).ToArray();
                switch (axis) {
                    case CtAce.Axis.X:
                        decompose[0] += distance;
                        break;
                    case CtAce.Axis.Y:
                        decompose[1] += distance;
                        break;
                    case CtAce.Axis.Z:
                        if (decompose.Length > 2) decompose[2] += distance;
                        else throw (new Exception("此 Robot 無法移動 Z 軸"));
                        break;
                    case CtAce.Axis.YAW:
                        if (decompose.Length > 3) decompose[3] += distance;
                        else throw (new Exception("此 Robot 無法移動 Yaw 軸"));
                        break;
                    case CtAce.Axis.PITCH:
                        if (decompose.Length > 4) decompose[4] += distance;
                        else throw (new Exception("此 Robot 無法移動 Pitch 軸"));
                        break;
                    case CtAce.Axis.ROLL:
                        if (decompose.Length > 5) decompose[5] += distance;
                        else throw (new Exception("此 Robot 無法移動 Roll(Theta) 軸"));
                        break;
                }

                IAdeptRobot robot = mWithCtrl ? mICtrl.GetRobot(robotNum) : miCobra as IAdeptRobot;
                robot.Jog(ManualControlMode.JogToWorldLocation, 1, -1, decompose);
                /* 測試發現不需 .WaitMoveDone ... 可能預設是 80% 往下吧 ?! */
            }
            currLoc = mVpLink.WhereWorld(robotNum).ToArray().ToList();
        }

        /// <summary>Jog 移動至特定 World 座標</summary>
        /// <param name="robotNum">手臂編號。預設為 1 開始</param>
        /// <param name="speed">速度。範圍為 -1~1，以百分比為主</param>
        /// <param name="location">目的座標(World)</param>
        /// <param name="currLoc">回傳最後座標</param>
        /// <remarks>因 ManualControlMode.JogToJointLocation 會跳 Invalid Jog Mode，尚不知道原因，故先取消 Joint 特定座標移動</remarks>
        /// <example><code>
        /// /*-- 使用 Jog 方式以 80% 的速度移動至座標 {100, 200, 370, 0, 180, 90} --*/
        /// ace.Motion.Jog(1, 0.8, new List&lt;double&gt; {100, 200, 370, 0, 180, 90});
        /// </code></example>
        public void Jog(int robotNum, double speed, IEnumerable<double> location, out List<double> currLoc) {
            if (mVpLink != null) {

                if (location.Count() != 6) throw (new Exception("點位輸入錯誤"));

                IAdeptRobot robot = mWithCtrl ? mICtrl.GetRobot(robotNum) : miCobra as IAdeptRobot;
                robot.Jog(ManualControlMode.JogToWorldLocation, speed, -1, location.ToArray());
                /* 測試發現不需 .WaitMoveDone ... 可能預設是 80% 往下吧 ?! */
            }
            currLoc = mVpLink.WhereWorld(robotNum).ToArray().ToList();
        }

        /// <summary>等待 Robot 移動完畢</summary>
        /// <param name="robotNum">手臂編號。預設為 1 開始</param>
        public void WaitMoveDone(int robotNum) {
            IAdeptRobot robot = mWithCtrl ? mICtrl.GetRobot(robotNum) : miCobra as IAdeptRobot;
            robot.WaitMoveDone();
        }
        #endregion

        #region Function - Methods

        /// <summary>將 PrecisionPoint 轉為 List&lt;double&gt;</summary>
        /// <param name="pp">欲轉換的 PrecisionPoint</param>
        /// <returns>座標 List&lt;double&gt;</returns>
        private List<double> ConvertPP2Double(PrecisionPoint pp) {
            List<double> loc = new List<double>();
            for (byte idx = 0; idx < pp.Length; idx++) {
                loc.Add(pp.GetJoint(idx));
            }
            return loc;
        }

        /// <summary>檢查座標是否於 Robot 可到達之範圍</summary>
        /// <param name="loc">欲檢查之座標點位</param>
        /// <returns>(True)可到達  (False)不可到達</returns>
        /// <example><code>
        /// List&lt;double&gt; loc = new List&lt;double&gt; { 100, 200, 370, 0, 180, 90 };
        /// bool result = mAce.Motion.IsInRange(loc);
        /// MessageBox.Show(result? "此座標可到達" : "此座標不可到達");
        /// </code></example>
        public bool IsInRange(List<double> loc) {
            int inRange = -1;

            Transform3D tempLoc = null;
            if (loc.Count == 6) tempLoc = new Transform3D(loc[0], loc[1], loc[2], loc[3], loc[4], loc[5]);
            else if (loc.Count == 3) tempLoc = new Transform3D(loc[0], loc[1], loc[2]);

            if (tempLoc != null) {
                if (mWithCtrl && mICtrl.RobotCount > 0)
                    inRange = mICtrl.GetRobot(1).InRange(tempLoc);
                else if (miCobra != null)
                    inRange = miCobra.InRange(tempLoc);
            }

            return (inRange == 0) ? true : false;
        }

        /// <summary>檢查座標是否於 Robot 可到達之範圍</summary>
        /// <param name="robotNum">手臂編號。預設為 1 開始</param>
        /// <param name="loc">欲檢查之座標點位</param>
        /// <returns>(True)可到達  (False)不可到達</returns>
        /// <example><code>
        /// List&lt;double&gt; loc = new List&lt;double&gt; { 100, 200, 370, 0, 180, 90 };
        /// bool result = mAce.Motion.IsInRange(2, loc);
        /// MessageBox.Show(result? "Robot 2 可到達此座標" : "Robot 2 無法到達此座標");
        /// </code></example>
        public bool IsInRange(int robotNum, List<double> loc) {
            int inRange = -1;

            Transform3D tempLoc = null;
            if (loc.Count == 6) tempLoc = new Transform3D(loc[0], loc[1], loc[2], loc[3], loc[4], loc[5]);
            else if (loc.Count == 3) tempLoc = new Transform3D(loc[0], loc[1], loc[2]);

            if (tempLoc != null) {
                if (mWithCtrl)
                    inRange = mICtrl.GetRobot(robotNum).InRange(tempLoc);
                else if (miCobra != null)
                    inRange = miCobra.InRange(tempLoc);
            }

            return (inRange == 0) ? true : false;
        }
        #endregion
    }
}
