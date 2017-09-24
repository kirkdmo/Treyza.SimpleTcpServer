using System;
using System.Net.Sockets;

namespace Treyza.SimpleTcpServer.Events
{
    public class MessageEventArgs : EventArgs
    {
        public TcpClient Client { get; set; }
        public string Message { get; set; }
    }
}
