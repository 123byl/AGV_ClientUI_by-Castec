using System;
using System.Collections.Generic;
using System.Linq;

using Ace.Core.Server;
using Ace.Core.Server.Motion;
using Ace.Adept.Server.Desktop;
using Ace.Adept.Server.Motion;

using CtLib.Library;
using CtLib.Module.Adept.Extension;
using CtLib.Module.Utility;

namespace CtLib.Module.Adept {

	#region Declaration - Supported Enumerations
	/// <summary>An enumeration of possible encoder errors</summary>
	public enum AdeptEncoderError {
		/// <summary>No encoder error has been detected</summary>
		None = 0,
		/// <summary>An error reading the absolute encoder has been detected</summary>
		AbsoluteError = 1,
		/// <summary>
		/// An error reading the A/B phases of an incremental encoder has been detected.
		/// <para>This is due to an illegal state change, such as from high-high to low-low, without</para>
		/// <para>going through the required high-low or low-high steps.</para>
		/// </summary>
		QuadratureError = 2,
		/// <summary>
		/// An error reading the index phase of an incremental encoder has been detected
		/// <para>This is typically due to the counts between index pulses being set correctly.</para>
		/// </summary>
		IndexError = 3
	}

	/// <summary>Encoder type codes</summary>
	public enum AdeptEncoderType {
		/// <summary>Unknown or unspecified encoder type</summary>
		Undefined = 0,
		/// <summary>Incremental encoder</summary>
		Incremental = 1,
		/// <summary>Sigma-I encoder</summary>
		Sigma1 = 10,
		/// <summary>Sigma-II encoder</summary>
		Sigma2 = 11,
		/// <summary>17-bit Sigma-II encoder</summary>
		Sigma2_17Bit = 12,
		/// <summary>20-bit Sigma-V encoder</summary>
		Sigma5_20Bit = 13,
		/// <summary>Panasonic bus-line encoder</summary>
		PanasonicBusline = 21,
		/// <summary>Tamagawa Serial Absolute (TSA) encoder</summary>
		Tamagawa = 51,
		/// <summary>Mitsubishi encoder</summary>
		Mitsubishi = 52
	}

	/// <summary>Homing configuration options for incremental encoders</summary>
	public enum AdeptHomeConfig {
		/// <summary></summary>Initial calibration search to one of the sensor of the motor
		HomeToSensor = 0,
		/// <summary>Initial calibration search to one of the hard stops of the motor</summary>
		HomeToStop = 1,
		/// <summary>No initial calibration search</summary>
		HomeToHere = 2,
		/// <summary>No zero index search</summary>
		NoZeroIndex = 4,
		/// <summary>Home sensor position is reported as the first transition moving onto the home sensor</summary>
		HomeOnToHomeSensor = 8,
		/// <summary>Initial calibration search to an overtravel rather than a stop or limit switch</summary>
		HomeToOverTravel = 524288
	}

	/// <summary>Motor input polarity bits. These are only used with SMI-6 applications</summary>
	public enum AdeptMotorPolarity {
		/// <summary>No bits are active</summary>
		None = 0,
		/// <summary>Home sensor is active-high</summary>
		Home = 1,
		/// <summary>Positive overtravel is active-high</summary>
		PositiveOverTravel = 2,
		/// <summary>Negative overtravel is active-high</summary>
		NegativeOverTravel = 4,
		/// <summary>Drive fault is active-high</summary>
		DriveFault = 8
	}

	/// <summary>Servo status bits</summary>
	public enum AdeptMotorStateBits {
		/// <summary>Set when the motor is calibrated</summary>
		Calibrated = int.MinValue,
		/// <summary>Home sensor active</summary>
		HomeActive = 1,
		/// <summary>Brake released</summary>
		BrakeReleased = 512,
		/// <summary>Amplifier enabled</summary>
		AmplifierEnabled = 1024,
		/// <summary>High power enabled (does not mean amplifier is enabled)</summary>
		PowerEnabled = 4096,
		/// <summary>No pending servo command is active (indicates when moves are done, etc)</summary>
		CommandDone = 65536,
		/// <summary>Set when motor is able to commutate</summary>
		CommunicationReady = 131072,
		/// <summary>Set when the motor meets the desired tolerance criteria</summary>
		InTolerance = 16777216,
		/// <summary>Set when a new index is found (cleared by starting an index search)</summary>
		IndexFound = 33554432,
		/// <summary>Set when the motor is off</summary>
		MotorOff = 1073741824
	}
	#endregion

	#region Declaration - Supported Classes
	/// <summary>Adept 馬達資訊</summary>
	[Serializable]
	public class MotorInfo {

		#region Properties

		///<summary>Gets the amp temperature</summary>
		public float AmpTemperature { get; private set; }
		///<summary>Amplifier board temperature (degrees C)</summary>
		public float BaseBoardTemperature { get; private set; }
		///<summary>Encoder temperature (°C)</summary>
		public float EncoderTemperature { get; private set; }
		/// <summary>0-based index of the motor within the robot</summary>
		public int MotorNumber { get; private set; }
		/// <summary>
		/// Gets or sets the current motor position. The preferred way to set the position
		/// <para>during calibration is to use the AdjustPosition command, since there is no gap</para>
		/// <para>in time between getting and setting position that can result in minor position errors</para>
		/// </summary>
		public int Position { get; private set; }
		/// <summary>Gets the encoder alarm</summary>
		public int EncoderAlarm { get; private set; }
		/// <summary>Gets the motor status bits</summary>
		public AdeptMotorStateBits MotorStatus { get; private set; }
		/// <summary>Gets or sets the robot number. The first robot in the system is robot 1</summary>
		public int RobotNumber { get; private set; }
		/// <summary>Gets AC voltage</summary>
		public float AcVoltage { get; private set; }
		/// <summary>Gets DC voltage</summary>
		public float DcVoltage { get; private set; }
		#endregion

		#region Constructors
		/// <summary>建構馬達資訊</summary>
		/// <param name="motor"><see cref="IMotor"/></param>
		public MotorInfo(IMotor motor) {
			this.AmpTemperature = motor.AmpTemperature;
			this.BaseBoardTemperature = motor.BaseBoardTemperature;
			try {
				this.EncoderTemperature = motor.EncoderTemperature;
			} catch (Exception) {
			}
			this.MotorNumber = motor.MotorNumber;
			this.Position = motor.Position;
			this.MotorStatus = (AdeptMotorStateBits)motor.Status.StatusBits;
			this.RobotNumber = motor.Robot.RobotNumber;
			this.AcVoltage = motor.AmpACInputRmsVoltage;
			this.DcVoltage = motor.DCInputVoltage;
		}
		#endregion
	}
	#endregion

	/// <summary>
	/// Adept Robot 相關移動控制模組
	/// <para>包含移動至點位、單軸移動等相關移動命令</para>
	/// </summary>
	[Serializable]
	public sealed class CtAceMotion {

		#region Declaration - Definitons
		/// <summary>預設加減速。依序為: 加速、減速、速度</summary>
		private static readonly int[] DEFAULT_ACCEL = { 20, 20, 60 };
		#endregion

		#region Declaration - Fields
		/// <summary>是否含有 SmartController</summary>
		private bool mWithCtrl;
		/// <summary>AceServer，用於取得相關 AceObject</summary>
		private IAceServer mIServer;
		/// <summary>V+ 連結器集合</summary>
		private VpObjects mVpObj;
		/// <summary>機器手臂集合</summary>
		private Dictionary<int, IRobot> mRobots;
		/// <summary>暫存 CartesianMove</summary>
		private CartesianMove mCsMove;
		#endregion

		#region Function - Constructors
		/// <summary>建立機器手臂移動控制模組</summary>
		/// <param name="aceSrv">AceServer，用於取得相關 AceObject</param>
		/// <param name="links">V+ 連結器</param>
		/// <param name="robots">機器手臂</param>
		/// <param name="withCtrl">是否含有 SmartController</param>
		internal CtAceMotion(IAceServer aceSrv, bool withCtrl, VpObjects links, Dictionary<int, IRobot> robots) {
			mIServer = aceSrv;
			mWithCtrl = withCtrl;
			mVpObj = links;
			mRobots = robots;
		}
		#endregion

		#region Function - Moving
		/// <summary>以當前位置為基礎，移動特定距離。請於手臂停止且不需使用時，執行 <see cref="Detach(int)"/> 以釋放控制權</summary>
		/// <param name="robotNum">手臂編號。預設為 1 開始</param>
		/// <param name="axis">欲移動的軸或方向</param>
		/// <param name="distance">欲移動距離</param>
		public void MoveDistance(int robotNum, Axis axis, double distance) {
			int jCount = mRobots[robotNum].JointCount;
			List<double> decompose = mRobots[robotNum].WorldLocation.ToList();
			switch (axis) {
				case Axis.X:
					decompose[0] += distance;
					break;
				case Axis.Y:
					decompose[1] += distance;
					break;
				case Axis.Z:
					if (jCount > 2) decompose[2] += distance;
					else throw (new Exception("此 Robot 無法移動 Z 軸"));
					break;
				case Axis.Yaw:
					if (jCount > 5) decompose[3] += distance;
					else throw (new Exception("此 Robot 無法移動 Yaw 軸"));
					break;
				case Axis.Pitch:
					if (jCount > 5) decompose[4] += distance;
					else throw (new Exception("此 Robot 無法移動 Pitch 軸"));
					break;
				case Axis.Roll:
					if (jCount > 3) decompose[5] += distance;
					else throw (new Exception("此 Robot 無法移動 Roll(Theta) 軸"));
					break;
			}

			MoveToLocation(robotNum, decompose);
		}

		/// <summary>設定 CartesianMove 全域變數</summary>
		/// <param name="accel">加減速配方。依序為 加速、減速、穩態速度</param>
		/// <param name="motionMode">移動路徑模式，直線或各軸內插</param>
		/// <param name="speedMode">速度單位，IPS、MMPS、Percent</param>
		/// <param name="scurfProfile">加減速曲線，從0到8。請參考Adept ACE Controller S-Curve Profile setting utility</param>
		/// <param name="moveMode">各點間之到點模式，Blend、NoNull、Coarse、Fine</param>
		/// <param name="percent">Coarse、Fine之到點百分比，其餘模式請忽略</param>
		public void CreateCartesianMove(int[] accel,
										MoveEndMode moveMode = MoveEndMode.NoNull,
										MotionMode motionMode = MotionMode.StrightLine,
										SpeedMode speedMode = SpeedMode.Percent,
										int scurfProfile = 0,
										int percent = 80) {
			if (mCsMove == null) mCsMove = mIServer.CreateObject(typeof(CartesianMove)) as CartesianMove;
			mCsMove.Param.Accel = accel[0];
			mCsMove.Param.Decel = accel[1];
			mCsMove.Param.Speed = accel[2];
			mCsMove.Param.MotionEnd = (MotionEnd)moveMode;   //完整到點
			mCsMove.Param.SpeedMode = (MoveSpeedMode)speedMode;
			mCsMove.Param.Straight = (motionMode == MotionMode.StrightLine) ? true : false;  //直線運動
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
		private void MoveToLocation(int robotNum, Transform3D location, int[] accel, VPlusVariableType varType = VPlusVariableType.Location, MotionMode motionMode = MotionMode.StrightLine, SpeedMode speedMode = SpeedMode.Percent, int scurfProfile = 0) {

			/*-- 檢查相關屬性 --*/
			if (accel.Length != 3) throw (new Exception("加減速設定數量錯誤"));

			/*-- 修改 Cartesian 物件 --*/
			IRobot robot = mRobots[robotNum];
			CreateCartesianMove(accel, MoveEndMode.NoNull, motionMode, speedMode, scurfProfile);
			mCsMove.Robot = robot;

			/*-- 建立 Transform3D --*/
			Transform3D loc = location;
			if (varType == VPlusVariableType.PrecisionPoint)
				loc = robot.JointToWorld(location.ToArray());

			mCsMove.WorldLocation = loc;

			int range = robot.InRange(loc);
			if (range == 0) {
				mCsMove.MoveConfiguration = robot.GetMoveConfiguration(robot.JointPosition);
				robot.Move(mCsMove);
			} else throw (new Exception("目標點位將造成第 " + range + " 軸超出範圍！"));
		}

		/// <summary>移動至特定座標(單點)。請於手臂停止且不需使用時，執行 <see cref="Detach(int)"/> 以釋放控制權</summary>
		/// <param name="robotNum">手臂編號。預設為 1 開始</param>
		/// <param name="location">目標座標</param>
		/// <param name="accel">加減速配方。依序為 加速、減速、穩態速度</param>
		/// <param name="varType">座標型態。Location或PrecisionPoint，兩者不相容！</param>
		/// <param name="motionMode">移動路徑模式，直線或各軸內插</param>
		/// <param name="speedMode">速度單位，IPS、MMPS、Percent</param>
		/// <param name="scurfProfile">加減速曲線，從0到8。請參考Adept ACE Controller S-Curve Profile setting utility</param>
		public void MoveToLocation(int robotNum, List<double> location, int[] accel, VPlusVariableType varType = VPlusVariableType.Location, MotionMode motionMode = MotionMode.StrightLine, SpeedMode speedMode = SpeedMode.Percent, int scurfProfile = 0) {

			/*-- 檢查相關屬性 --*/
			if (accel.Length != 3) throw (new Exception("加減速設定數量錯誤"));

			/*-- 修改 Cartesian 物件 --*/
			IRobot robot = mRobots[robotNum];
			CreateCartesianMove(accel, MoveEndMode.NoNull, motionMode, speedMode, scurfProfile);
			mCsMove.Robot = robot;

			/*-- 建立 Transform3D --*/
			Transform3D loc = null;
			if (varType == VPlusVariableType.Location) {
				if (location.Count == 3)
					loc = new Transform3D(location[0], location[1], location[2]);
				else if (location.Count == 6)
					loc = new Transform3D(location[0], location[1], location[2], location[3], location[4], location[5]);
				else throw (new Exception("點位數量錯誤"));

			} else if (varType == VPlusVariableType.PrecisionPoint)
				loc = robot.JointToWorld(location.ToArray());

			mCsMove.WorldLocation = loc;

			int range = robot.InRange(loc);
			if (range == 0) {
				mCsMove.MoveConfiguration = robot.GetMoveConfiguration(robot.JointPosition);
				robot.Move(mCsMove);
			} else throw (new Exception("目標點位將造成第 " + range + " 軸超出範圍！"));
		}

		/// <summary>移動至特定座標(單點)。請於手臂停止且不需使用時，執行 <see cref="Detach(int)"/> 以釋放控制權</summary>
		/// <param name="robotNum">手臂編號。預設為 1 開始</param>
		/// <param name="location">目標座標</param>
		/// <param name="varType">座標型態。Location或PrecisionPoint，兩者不相容！</param>
		/// <param name="motionMode">移動路徑模式，直線或各軸內插</param>
		/// <param name="speedMode">速度單位，IPS、MMPS、Percent</param>
		/// <param name="scurfProfile">加減速曲線，從0到8。請參考Adept ACE Controller S-Curve Profile setting utility</param>
		public void MoveToLocation(int robotNum, List<double> location, VPlusVariableType varType = VPlusVariableType.Location, MotionMode motionMode = MotionMode.StrightLine, SpeedMode speedMode = SpeedMode.Percent, int scurfProfile = 0) {
			MoveToLocation(robotNum, location.ToList(), DEFAULT_ACCEL, varType, motionMode, speedMode, scurfProfile);
		}

		/// <summary>移動至特定座標(連續多點)。請於手臂停止且不需使用時，執行 <see cref="Detach(int)"/> 以釋放控制權</summary>
		/// <param name="robotNum">手臂編號。預設為 1 開始</param>
		/// <param name="location">目標座標集合，TKey=該點順序 TValue=座標集合，如第一點 (0 /*0為第一點*/, List(Of double){300, -200, 270, 0, 180, 90})</param>
		/// <param name="accel">加減速配方。依序為 加速、減速、穩態速度</param>
		/// <param name="varType">座標型態。Location或PrecisionPoint，兩者不相容！</param>
		/// <param name="motionMode">移動路徑模式，直線或各軸內插</param>
		/// <param name="moveMode">各點間之到點模式，Blend、NoNull、Coarse、Fine</param>
		/// <param name="percent">Coarse、Fine之到點百分比，其餘模式請忽略</param>
		/// <param name="speedMode">速度單位，IPS、MMPS、Percent</param>
		/// <param name="scurfProfile">加減速曲線，從0到8。請參考Adept ACE Controller S-Curve Profile setting utility</param>
		public void MoveToLocation(int robotNum, Dictionary<int, List<double>> location, int[] accel, VPlusVariableType varType = VPlusVariableType.Location, MotionMode motionMode = MotionMode.StrightLine, MoveEndMode moveMode = MoveEndMode.NoNull, int percent = 80, SpeedMode speedMode = SpeedMode.Percent, int scurfProfile = 0) {

			/*-- 檢查相關屬性 --*/
			if (accel.Length != 3) throw (new Exception("加減速設定數量錯誤"));

			/*-- 修改 Cartesian 物件 --*/
			IRobot robot = mRobots[robotNum];
			CreateCartesianMove(accel, moveMode, motionMode, speedMode, scurfProfile, percent);
			mCsMove.Robot = robot;

			/*-- 建立 Transform3D --*/
			List<Transform3D> locs = new List<Transform3D>();

			for (int idx = 0; idx < location.Count; idx++) {
				if (location.ContainsKey(idx)) {
					if (varType == VPlusVariableType.Location) {
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

		/// <summary>移動至特定座標(連續多點)。請於手臂停止且不需使用時，執行 <see cref="Detach(int)"/> 以釋放控制權</summary>
		/// <param name="robotNum">手臂編號。預設為 1 開始</param>
		/// <param name="location">目標座標集合，TKey=該點順序 TValue=座標集合，如第一點 (1, List(Of double){300, -200, 270, 0, 180, 90})</param>
		/// <param name="varType">座標型態。Location或PrecisionPoint，兩者不相容！</param>
		/// <param name="motionMode">移動路徑模式，直線或各軸內插</param>
		/// <param name="moveMode">各點間之到點模式，Blend、NoNull、Coarse、Fine</param>
		/// <param name="percent">Coarse、Fine之到點百分比，其餘模式請忽略</param>
		/// <param name="speedMode">速度單位，IPS、MMPS、Percent</param>
		/// <param name="scurfProfile">加減速曲線，從0到8。請參考Adept ACE Controller S-Curve Profile setting utility</param>
		public void MoveToLocation(int robotNum, Dictionary<int, List<double>> location, VPlusVariableType varType = VPlusVariableType.Location, MotionMode motionMode = MotionMode.StrightLine, MoveEndMode moveMode = MoveEndMode.NoNull, int percent = 80, SpeedMode speedMode = SpeedMode.Percent, int scurfProfile = 0) {
			MoveToLocation(robotNum, location, DEFAULT_ACCEL, varType, motionMode, moveMode, percent, speedMode, scurfProfile);
		}

		/// <summary>移動至特定座標(變數、單點)。請於手臂停止且不需使用時，執行 <see cref="Detach(int)"/> 以釋放控制權</summary>
		/// <param name="robotNum">手臂編號。預設為 1 開始</param>
		/// <param name="varName">變數名稱，如是 PrecisionPoint(Joint) 請帶入 "#" 字號，如 @"#loc.safe"</param>
		/// <param name="varType">座標型態。Location或PrecisionPoint，兩者不相容！</param>
		/// <param name="motionMode">移動路徑模式，直線或各軸內插</param>
		/// <param name="speedMode">速度單位，IPS、MMPS、Percent</param>
		/// <param name="scurfProfile">加減速曲線，從0到8。請參考Adept ACE Controller S-Curve Profile setting utility</param>
		public void MoveToLocation(int robotNum, string varName, VPlusVariableType varType = VPlusVariableType.Location, MotionMode motionMode = MotionMode.StrightLine, SpeedMode speedMode = SpeedMode.Percent, int scurfProfile = 0) {
			Transform3D loc = null;
			switch (varType) {
				case VPlusVariableType.Location:
					loc = mVpObj.Link(link => link.ListL(varName));
					break;
				case VPlusVariableType.PrecisionPoint:
					PrecisionPoint pp = mVpObj.Link(link => link.ListPP(varName));
					loc = mRobots[robotNum].JointToWorld(pp.ToArray());
					break;
				default:
					throw (new Exception("不支援的變數型態"));
			}
			MoveToLocation(robotNum, loc, DEFAULT_ACCEL, VPlusVariableType.Location, motionMode, speedMode, scurfProfile);
		}

		/// <summary>
		/// Jog 移動。以指定的 Axis 或 Joint 為主 (均以 0 為起始，亦可使用 Axis 與 CtAce.Joint 進行轉換)
		/// <para>速度給予正值則將往正方向移動，反之則往負方向移動</para>
		/// </summary>
		/// <param name="robotNum">手臂編號。預設為 1 開始</param>
		/// <param name="mode">移動模式</param>
		/// <param name="speed">速度。範圍為 -1~1，以百分比為主</param>
		/// <param name="axis">軸。World (0)X (1)Y... || Joint (0)J1 (1)J2...</param>
		/// <example><code language="C#">
		/// /*-- 往 「負 X」(X減) 方向移動 --*/
		/// ace.Motion.Jog(1, JogMode.World, -0.8, (int)Axis.X);
		/// </code></example>
		public void Jog(int robotNum, JogMode mode, double speed, int axis) {
			if (mWithCtrl) {
				IAdeptRobot robot = mRobots[robotNum] as IAdeptRobot;
				robot.Jog((ManualControlMode)mode, speed, axis, null);
				/* 測試發現不需 .WaitMoveDone ... 可能預設是 80% 往下吧 ?! */
			} else {
				Transform3D wLoc;
				PrecisionPoint pLoc;
				mVpObj.Link(link => link.Jog((int)mode, axis, (int)speed, out wLoc, out pLoc));
			}
		}

		/// <summary>Jog 移動。指定座標移動特定距離 (正負方向由 distance 決定)</summary>
		/// <param name="robotNum">手臂編號。預設為 1 開始</param>
		/// <param name="distance">移動距離。正值往正方向移動，負值則相反</param>
		/// <param name="axis">坐標軸</param>
		/// <example><code language="C#">
		/// /*-- 往 Y 負的方向移動 0.5mm --*/
		/// ace.Motion.Jog(1, Axis.Y, -0.5);
		/// </code></example>
		public void Jog(int robotNum, Axis axis, double distance) {
			int jCount = mRobots[robotNum].JointCount;
			double[] decompose = mVpObj.Link(link => link.WhereWorld(robotNum)).ToArray();
			switch (axis) {
				case Axis.X:
					decompose[0] += distance;
					break;
				case Axis.Y:
					decompose[1] += distance;
					break;
				case Axis.Z:
					if (jCount > 2) decompose[2] += distance;
					else throw (new Exception("此 Robot 無法移動 Z 軸"));
					break;
				case Axis.Yaw:
					if (jCount > 5) decompose[3] += distance;
					else throw (new Exception("此 Robot 無法移動 Yaw 軸"));
					break;
				case Axis.Pitch:
					if (jCount > 5) decompose[4] += distance;
					else throw (new Exception("此 Robot 無法移動 Pitch 軸"));
					break;
				case Axis.Roll:
					if (jCount > 3) decompose[5] += distance;
					else throw (new Exception("此 Robot 無法移動 Roll(Theta) 軸"));
					break;
			}

			if (mWithCtrl) {
				IAdeptRobot robot = mRobots[robotNum] as IAdeptRobot;
				robot.Jog(ManualControlMode.JogToWorldLocation, 1, -1, decompose);
				/* 測試發現不需 .WaitMoveDone ... 可能預設是 80% 往下吧 ?! */
			} else {
				throw new NotSupportedException("iCobra 目前尚不支援第三方 Jog 命令");
			}
		}

		/// <summary>Jog 移動至特定 World 座標</summary>
		/// <param name="robotNum">手臂編號。預設為 1 開始</param>
		/// <param name="speed">速度。範圍為 -1~1，以百分比為主</param>
		/// <param name="location">目的座標(World)</param>
		/// <remarks>因 ManualControlMode.JogToJointLocation 會跳 Invalid Jog Mode，尚不知道原因，故先取消 Joint 特定座標移動</remarks>
		/// <example><code language="C#">
		/// /*-- 使用 Jog 方式以 80% 的速度移動至座標 {100, 200, 370, 0, 180, 90} --*/
		/// ace.Motion.Jog(1, 0.8, new List&lt;double&gt; {100, 200, 370, 0, 180, 90});
		/// </code></example>
		public void Jog(int robotNum, double speed, IEnumerable<double> location) {

			if (location.Count() != 6) throw (new Exception("點位輸入錯誤"));

			if (mWithCtrl) {
				IAdeptRobot robot = mRobots[robotNum] as IAdeptRobot;
				robot.Jog(ManualControlMode.JogToWorldLocation, speed, -1, location.ToArray());
				/* 測試發現不需 .WaitMoveDone ... 可能預設是 80% 往下吧 ?! */
			} else {
				throw new NotSupportedException("iCobra 目前尚不支援第三方 Jog 命令");
			}
		}

		/// <summary>
		/// Jog 移動。以指定的 Axis 或 Joint 為主，並回傳最後座標 (均以 0 為起始，亦可使用 Axis 與 CtAce.Joint 進行轉換)
		/// <para>速度給予正值則將往正方向移動，反之則往負方向移動</para>
		/// </summary>
		/// <param name="robotNum">手臂編號。預設為 1 開始</param>
		/// <param name="mode">移動模式</param>
		/// <param name="speed">速度。範圍為 -1~1，以百分比為主</param>
		/// <param name="axis">軸。World (0)X (1)Y... || Joint (0)J1 (1)J2...</param>
		/// <param name="currLoc">回傳最後座標</param>
		/// <example><code language="C#">
		/// /*-- 往 「負 X」(X減) 方向移動 --*/
		/// ace.Motion.Jog(1, JogMode.World, -0.8, (int)Axis.X);
		/// </code></example>
		public void Jog(int robotNum, JogMode mode, double speed, int axis, out List<double> currLoc) {
			IRobot robot = mRobots[robotNum];
			Jog(robotNum, mode, speed, axis);
			robot.WaitMoveDone();
			CtTimer.Delay(50);

			currLoc = mVpObj.Link(link => (mode == JogMode.Joint) ? link.WhereJoint(robotNum).ToList() : link.WhereWorld(robotNum).ToList());
		}

		/// <summary>Jog 移動。指定座標移動特定距離 (正負方向由 distance 決定)</summary>
		/// <param name="robotNum">手臂編號。預設為 1 開始</param>
		/// <param name="distance">移動距離。正值往正方向移動，負值則相反</param>
		/// <param name="axis">坐標軸</param>
		/// <param name="currLoc">回傳最後座標</param>
		/// <example><code language="C#">
		/// /*-- 往 Y 負的方向移動 0.5mm --*/
		/// ace.Motion.Jog(1, Axis.Y, -0.5);
		/// </code></example>
		public void Jog(int robotNum, Axis axis, double distance, out List<double> currLoc) {
			IRobot robot = mRobots[robotNum];
			Jog(robotNum, axis, distance);
			robot.WaitMoveDone();
			CtTimer.Delay(50);

			currLoc = mVpObj.Link(link => link.WhereWorld(robotNum)).ToList();
		}

		/// <summary>Jog 移動至特定 World 座標</summary>
		/// <param name="robotNum">手臂編號。預設為 1 開始</param>
		/// <param name="speed">速度。範圍為 -1~1，以百分比為主</param>
		/// <param name="location">目的座標(World)</param>
		/// <param name="currLoc">回傳最後座標</param>
		/// <remarks>因 ManualControlMode.JogToJointLocation 會跳 Invalid Jog Mode，尚不知道原因，故先取消 Joint 特定座標移動</remarks>
		/// <example><code language="C#">
		/// /*-- 使用 Jog 方式以 80% 的速度移動至座標 {100, 200, 370, 0, 180, 90} --*/
		/// ace.Motion.Jog(1, 0.8, new List&lt;double&gt; {100, 200, 370, 0, 180, 90});
		/// </code></example>
		public void Jog(int robotNum, double speed, IEnumerable<double> location, out List<double> currLoc) {
			IRobot robot = mRobots[robotNum];
			Jog(robotNum, speed, location);
			robot.WaitMoveDone();
			CtTimer.Delay(50);

			currLoc = mVpObj.Link(link => link.WhereWorld(robotNum)).ToList();
		}

		/// <summary>等待 Robot 移動完畢</summary>
		/// <param name="robotNum">手臂編號。預設為 1 開始</param>
		public void WaitMoveDone(int robotNum) {
			if (mRobots.ContainsKey(robotNum)) {
				mRobots[robotNum].WaitMoveDone();
				mRobots[robotNum].AutomaticControlActive = false;
			}

			/*-- 以下尚未測試 --*/
			//IRobot robot = mRobots[robotNum];
			//do {
			//	CtTimer.Delay(10);
			//} while (robot.TrajectoryState != TrajectoryState.Idle);
		}

		/// <summary>釋放 <see cref="IRobot"/> 的控制權，等同 V+ 的 DETACH 指令</summary>
		/// <param name="robotNum">欲釋放控制權的手臂代號。預設為 1 開始</param>
		public void Detach(int robotNum) {
			if (mRobots.ContainsKey(robotNum)) {
				mRobots[robotNum].AutomaticControlActive = false;
			}
		}
		#endregion

		#region Function - State Check

		/// <summary>檢查座標是否於 Robot 可到達之範圍</summary>
		/// <param name="loc">欲檢查之座標點位</param>
		/// <returns>(<see langword="true"/>)可到達  (<see langword="false"/>)不可到達</returns>
		/// <example><code language="C#">
		/// List&lt;double&gt; loc = new List&lt;double&gt; { 100, 200, 370, 0, 180, 90 };
		/// bool result = mAce.Motion.IsInRange(loc);
		/// MessageBox.Show(result? "此座標可到達" : "此座標不可到達");
		/// </code></example>
		public bool IsInRange(List<double> loc) {
			Transform3D tempLoc = null;
			if (loc.Count == 6) tempLoc = new Transform3D(loc[0], loc[1], loc[2], loc[3], loc[4], loc[5]);
			else if (loc.Count == 3) tempLoc = new Transform3D(loc[0], loc[1], loc[2]);

			int inRange = mRobots.First().Value.InRange(tempLoc);

			return (inRange == 0) ? true : false;
		}

		/// <summary>檢查座標是否於 Robot 可到達之範圍</summary>
		/// <param name="robotNum">手臂編號。預設為 1 開始</param>
		/// <param name="loc">欲檢查之座標點位</param>
		/// <returns>(<see langword="true"/>)可到達  (<see langword="false"/>)不可到達</returns>
		/// <example><code language="C#">
		/// List&lt;double&gt; loc = new List&lt;double&gt; { 100, 200, 370, 0, 180, 90 };
		/// bool result = mAce.Motion.IsInRange(2, loc);
		/// MessageBox.Show(result? "Robot 2 可到達此座標" : "Robot 2 無法到達此座標");
		/// </code></example>
		public bool IsInRange(int robotNum, List<double> loc) {
			Transform3D tempLoc = null;
			if (loc.Count == 6) tempLoc = new Transform3D(loc[0], loc[1], loc[2], loc[3], loc[4], loc[5]);
			else if (loc.Count == 3) tempLoc = new Transform3D(loc[0], loc[1], loc[2]);

			int inRange = mRobots[robotNum].InRange(tempLoc);

			return (inRange == 0) ? true : false;
		}

		/// <summary>取得當前手臂的姿態為左臂或右臂</summary>
		/// <param name="robotNum">手臂編號。預設為 1 開始</param>
		/// <returns>手臂姿態</returns>
		public ArmPosture GetCurrentPosture(int robotNum) {
			IRobot robot = mRobots[robotNum];
			MoveConfiguration mvConfig = robot.GetMoveConfiguration(robot.JointPosition) as MoveConfiguration;
			return (ArmPosture)mvConfig.RightyLefty;
		}
		#endregion

		#region Function - State Object
		/// <summary>取得馬達資訊</summary>
		/// <param name="robotNum">機器手臂編號</param>
		/// <param name="motorIdx">馬達編號</param>
		/// <returns>馬達資訊</returns>
		public MotorInfo GetMotor(int robotNum, int motorIdx) {
			IAdeptRobot robot = mRobots[robotNum] as IAdeptRobot;
			if (robot != null) {
				IMotor motor = robot.Motors.FirstOrDefault(m => m.MotorNumber == motorIdx);
				return motor == null ? null : new MotorInfo(motor);
			} else return null;
		}

		/// <summary>取得機器手臂所有的馬達資訊</summary>
		/// <param name="robotNum">機器手臂編號</param>
		/// <returns>馬達資訊</returns>
		public IEnumerable<MotorInfo> GetMotors(int robotNum) {
			IAdeptRobot robot = mRobots[robotNum] as IAdeptRobot;
			if (robot != null) {
				return robot.Motors.Select(motor => new MotorInfo(motor));
			} else return null;
		}

		/// <summary>取得多隻機器手臂之馬達資訊</summary>
		/// <param name="robotNum">機器手臂編號集合</param>
		/// <returns>馬達資訊</returns>
		public List<MotorInfo> GetMotors(params int[] robotNum) {
			List<MotorInfo> dataColl = new List<MotorInfo>();
			robotNum.ForEach(
				num => {
					IAdeptRobot robot = mRobots[num] as IAdeptRobot;
					if (robotNum != null) {
						dataColl.AddRange(robot.Motors.Select(motor => new MotorInfo(motor)));
					}
				}
			);
			return dataColl;
		}
		#endregion
	}
}
