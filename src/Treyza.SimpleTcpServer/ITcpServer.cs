using System.Net.Sockets;
using System.Threading.Tasks;

namespace Treyza.SimpleTcpServer
{
    public interface ITcpServer
    {
        byte Delimiter { get; set; }

        Task Start();
        Task Stop();
        void FireClientDisconnected(object sender, TcpClient client);
        void FireClientConnected(object sender, TcpClient client);
        void FireMessageReceived(object sender, TcpClient client, string message);
    }
}
