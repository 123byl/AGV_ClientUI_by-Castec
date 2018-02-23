using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using SerialCommunicationData;

namespace VehiclePlanner.Core {

    /// <summary>
    /// IiTSController介面實作
    /// </summary>
    internal partial class CtVehiclePlanner : IiTSController {
        
        /// <summary>
        /// iTS相關操作
        /// </summary>
        private IiTSController mITS = null;

        public string HostIP {
            get {
                return mITS.HostIP;
            }
        }

        public DelInputBox InputBox {
            get {
                return mITS.InputBox;
            }

            set {
                mITS.InputBox = value;
            }
        }

        public bool IsAutoReport {
            get {
                return mITS.IsAutoReport;
            }
        }

        public bool IsBypassSocket {
            get {
                return mITS.IsBypassSocket;
            }

            set {
                mITS.IsBypassSocket = value;
            }
        }

        public bool IsConnected {
            get {
                return mITS.IsConnected;
            }
        }

        public bool IsMotorServoOn {
            get {
                return mITS.IsMotorServoOn;
            }

            set {
                mITS.IsMotorServoOn = value;
            }
        }

        public bool IsScanning {
            get {
                return mITS.IsScanning;
            }
        }

        public DelSelectFile SelectFile {
            get {
                return mITS.SelectFile;
            }

            set {
                mITS.SelectFile = value;
            }
        }

        public IStatus Status {
            get {
                return mITS.Status;
            }
        }

        public int Velocity {
            get {
                return mITS.Velocity;
            }

            set {
                mITS.Velocity = value;
            }
        }

        public void AutoReport(bool auto) {
            mITS.AutoReport(auto);
        }

        public void ConnectToITS(bool cnn, string hostIP = "") {
            mITS.ConnectToITS(cnn, hostIP);
        }

        public void DoCharging(string powerName) {
            mITS.DoCharging(powerName);
        }

        public void DoPositionComfirm() {
            mITS.DoPositionComfirm();
        }

        public void DoRunningByGoalName(string goalName) {
            mITS.DoRunningByGoalName(goalName);
        }

        public void FindPath(string goalName) {
            mITS.FindPath(goalName);
        }

        public void GetGoalNames() {
            mITS.GetGoalNames();
        }

        public void GetMap() {
            mITS.GetMap();
        }

        public void GetOri() {
            mITS.GetOri();
        }

        public void MotionContorl(MotionDirection direction) {
            mITS.MotionContorl(direction);
        }

        public void RequestLaser() {
            mITS.RequestLaser();
        }

        public IDocument RequestMapFile(string mapName) {
            return mITS.RequestMapFile(mapName);
        }

        public string RequestMapList() {
            return mITS.RequestMapList();
        }

        public void SendAndSetMap(string mapPath) {
            mITS.SendAndSetMap(mapPath);
        }

        public void SetPosition(IPair oldPosition, IPair newPosition) {
            mITS.SetPosition(oldPosition, newPosition);
        }

        public void SetServoMode(bool servoOn) {
            mITS.SetServoMode(servoOn);
        }

        public void SetWorkVelocity(int velocity) {
            mITS.SetWorkVelocity(velocity);
        }

        public void StartScan(bool scan) {
            mITS.StartScan(scan);
        }

    }

}
