using System.Net;
using System.Net.Sockets;

namespace ServerOperation
{
    public class Communication
    {
        #region - Member -

        //private Socket sRecvCmd;
        //private Socket sRecvMap;
        public int recvCmdPort = 400;
        public int recvMapPort = 600;
        public int recvDataPort = 800;

        #endregion

        #region - Constructor -

        public Communication(int commandPort,int filePort,int statusPort)
        {
            recvCmdPort = commandPort;
            recvMapPort = filePort;
            recvDataPort = statusPort;
        }

        #endregion

        #region - Method -

        /// <summary>
        /// 將指定<see cref="Socket"/>連接至指定Server
        /// </summary>
        /// <param name="sServer">要用來連接的<see cref="Socket"/></param>
        /// <param name="serverIP">要連接至的Server IP</param>
        /// <param name="serverPort">要連接至的Server Port</param>
        /// <returns><see cref="Socket"/>連線狀態 True:已連線/False:已斷開</returns>
        public bool ConnectServer(ref Socket sServer, string serverIP, int serverPort) {
            if (sServer != null) {
                sServer.Close();
            }
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIP.ToString()), serverPort);
            sServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sServer.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);//設置接收資料超時
            sServer.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);
            sServer.Connect(ipEndPoint);
            return sServer.Connected;
        }

        /// <summary>
        /// 將指定<see cref="Socket"/>連線斷開
        /// </summary>
        /// <param name="sServer">要斷開連線的<see cref="Socket"/></param>
        /// <returns><see cref="Socket"/>連線狀態 True:已連線/False:已斷開</returns>
        public bool DisconnectServer(Socket sServer) {
            if (sServer != null) {
                try {
                    sServer.Shutdown(SocketShutdown.Both);
                    sServer.Close();
                } catch (SocketException se) {
                    System.Console.WriteLine("[Socket DisConnect ]" + se.ToString());
                }
            }
            return sServer.Connected;
        }

        public bool ServoOn(ref Socket sClient,int connectPort)
        {
            IPEndPoint recvCmdLocalEndPoint = new IPEndPoint(IPAddress.Any, connectPort);
            sClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sClient.Bind(recvCmdLocalEndPoint);
            sClient.Listen(1);
            return true;
        }
        public Socket ClientAccept(Socket sClient, int receiveTimeOut = 5000,int sendTimeout = 5000,int sendBuffer = 8192,int receiveBuffer = 1024)
        {
            Socket client = sClient.Accept();
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, receiveTimeOut);
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, sendTimeout);
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, sendBuffer); 
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, receiveBuffer);
            return client;
        }

        #endregion

    }
}
