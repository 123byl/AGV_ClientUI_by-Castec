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
