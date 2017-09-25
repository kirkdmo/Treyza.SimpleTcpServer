using System;
using System.Net;
using System.Net.Sockets;
using Treyza.SimpleTcpServer.Events;

namespace Treyza.SimpleTcpServer
{
    public class SimpleTcpServer : ITcpServer, IDisposable
    {
        private readonly ServerListener _listener;

        public byte Delimiter { get; set; } = 13;

        public event EventHandler<ClientEventArgs> OnConnected;
        public event EventHandler<ClientEventArgs> OnDisconnected;
        public event EventHandler<MessageEventArgs> OnMessage; 


        public SimpleTcpServer(IPAddress address, int port)
        {
            _listener = new ServerListener(address, port, this);    
        }

        public void Start()
        {
            _listener.Start();
        }

        public void Stop()
        {
            _listener.Stop();
        }

        public void FireClientDisconnected(object sender, TcpClient client)
        {



            OnDisconnected?.Invoke(this, new ClientEventArgs { Client = client});
        }

        public void FireClientConnected(object sender, TcpClient client)
        {
            OnConnected?.Invoke(this, new ClientEventArgs{ Client = client});

        }

        public void FireMessageReceived(object sender, TcpClient client, string message)
        {

            OnMessage?.Invoke(this, new MessageEventArgs { Client = client, Message =  message});
        }

        public void Dispose()
        {
            _listener?.Dispose();
        }
    }
}
