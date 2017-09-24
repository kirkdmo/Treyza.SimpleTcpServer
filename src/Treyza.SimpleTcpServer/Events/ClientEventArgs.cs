using System;
using System.Net.Sockets;

namespace Treyza.SimpleTcpServer.Events
{
    public class ClientEventArgs : EventArgs
    {
        public TcpClient Client { get; set; }
    }
}
