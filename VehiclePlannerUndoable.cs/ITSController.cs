using Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehiclePlanner.Core;

namespace VehiclePlannerUndoable.cs
{
    /// <summary>
    /// 以Undoable實作之ITS控制器介面
    /// </summary>
    public interface IITSController_Undoable : IBaseITSController
    {

    }

    /// <summary>
    /// ITS控制器
    /// </summary>
    public class ITSController : BaseiTSController<Serializable, Serializable>, IITSController_Undoable
    {

        #region Declaration - Fields

        /// <summary>
        /// 序列传输用户端
        /// </summary>
        private SerialClient mClient = new SerialClient();

        #endregion Declaration - Fields

        #region Declaration - Properties

        /// <summary>
        /// 是否已连线
        /// </summary>
        public override bool IsConnected => mClient.ConnectStatus == AsyncSocket.EConnectStatus.Connect ;

        #endregion Declaration - Properties

        #region Function - Public Methods
        
        public override void AutoReport(bool auto)
        {
            throw new NotImplementedException();
        }
        
        public override void GetOri()
        {
            throw new NotImplementedException();
        }
        
        #endregion Function - Public Methods

        #region Function - Private Methods

        /// <summary>
        /// 用户端初始化
        /// </summary>
        protected override void ClientInitial()
        {
            if (mClient != null)
            {
                try
                {
                    mClient.Dispose();
                }
                catch (Exception)
                {

                }
            }
            mClient = new SerialClient();
            mClient.ConnectStatusChangedEvent += MClient_ConnectStatusChangedEvent;
            mClient.ReceivedSerialDataEvent += MClient_ReceivedSerialDataEvent;
        }
        
        /// <summary>
        /// 连线至伺服端
        /// </summary>
        /// <param name="ip">伺服端IP</param>
        /// <param name="port">伺服端埠号</param>
        protected override void ClientConnect(string ip, int port) => mClient.Connect(ip, port);

        /// <summary>
        /// 停止与伺服端连线
        /// </summary>
        protected override void ClientStop() => mClient.Disconnect();
        
        
        protected override string RequestOriList()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// 传送命令
        /// </summary>
        /// <param name="packet">命令物件</param>
        /// <returns>是否传送成功</returns>
        protected override bool ClientSend(Serializable packet)
        {
            bool success = true;
            try
            {
                mClient.Send(packet);
            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }

        /// <summary>
        /// 检测是否等待中的命令
        /// </summary>
        /// <param name="v">等待的命令</param>
        /// <param name="packet">要发送的命令</param>
        /// <returns>是否等待中命令</returns>
        protected override bool IsWaitingCmd(CtTaskCompletionSource<Serializable, Serializable> v, Serializable packet) => v.WaitCmd.TxID == packet.TxID;

        /// <summary>
        /// 检测是否等待中的回复
        /// </summary>
        /// <param name="v">等待的命令</param>
        /// <param name="response">收到的回复</param>
        /// <returns>是否是等待中的命令</returns>
        protected override bool IsWaitingResponse(CtTaskCompletionSource<Serializable, Serializable> v, Serializable response) => v.WaitCmd.TxID == response.TxID;

        /// <summary>
        /// 产生命令标题
        /// </summary>
        /// <param name="packet">命令物件</param>
        /// <returns>命令标题</returns>
        protected override string CmdTitle(Serializable packet) => $"{packet.ToString()}({packet.TxID}):";


        protected override void DoReceiveAction(Serializable reponse)
        {
            throw new NotImplementedException();
        }

        protected override BaseRequeLaser ImpRequestLaser()
        {
            throw new NotImplementedException();
        }


        protected override BaseDoPositionComfirm ImpDoPositionComfirm()
        {
            throw new NotImplementedException();
        }

        protected override BaseGoTo ImpGoTo(string goalName)
        {
            throw new NotImplementedException();
        }


        protected override BaseRequestPath ImpRequestPath(string goalName)
        {
            throw new NotImplementedException();
        }

        protected override BaseRequestGoalList ImpRequestGoalList()
        {
            throw new NotImplementedException();
        }

        public override BaseFileReturn RequestMapFile(string mapName)
        {
            throw new NotImplementedException();
        }

        protected override BaseBoolReturn ImpSetServoMode(bool servoOn)
        {
            throw new NotImplementedException();
        }

        protected override BaseBoolReturn ImpSetWorkVelocity(int velocity)
        {
            throw new NotImplementedException();
        }

        protected override BaseListReturn ImpRequestMapList()
        {
            throw new NotImplementedException();
        }

        protected override BaseBoolReturn UploadMapToAGV(string mapPath)
        {
            throw new NotImplementedException();
        }

        protected override BaseBoolReturn ChangeMap(string mapName)
        {
            throw new NotImplementedException();
        }

        protected override BaseBoolReturn StartManualControl(bool start)
        {
            throw new NotImplementedException();
        }

        protected override BaseBoolReturn StopScanning()
        {
            throw new NotImplementedException();
        }

        protected override BaseSetScanningOriFileName SetScanningOriFileName(string oriName)
        {
            throw new NotImplementedException();
        }

        protected override BaseBoolReturn SetManualVelocity(int leftVelocity, int rightVelocity)
        {
            throw new NotImplementedException();
        }

        #endregion Function - Private Methods

        #region Function - Events

        /// <summary>
        /// 资料接收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MClient_ReceivedSerialDataEvent(object sender, ReceivedSerialDataEventArgs e)
        {
        }

        /// <summary>
        /// 连线状态变更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MClient_ConnectStatusChangedEvent(object sender, AsyncSocket.ConnectStatusChangedEventArgs e)
        {
        }
        
        #endregion Function - Events

    }
}
