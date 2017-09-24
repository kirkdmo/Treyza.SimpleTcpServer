using System.Net.Sockets;

namespace Treyza.SimpleTcpServer
{
    public interface ITcpServer
    {
        byte Delimiter { get; set; }

        void Start();
        void Stop();
        void FireClientDisconnected(object sender, TcpClient client);
        void FireClientConnected(object sender, TcpClient client);
        void FireMessageReceived(object sender, TcpClient client, string message);
    }
}
