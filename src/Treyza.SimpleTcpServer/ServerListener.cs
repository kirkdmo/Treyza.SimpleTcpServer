using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Treyza.SimpleTcpServer
{
    public class ServerListener : IDisposable
    {
        private readonly ITcpServer _tcpServer;
        private readonly TcpListener _listener;
        private CancellationTokenSource _cancellationToken;

        

        public ServerListener(IPAddress address, int port, ITcpServer tcpServer)
        {
            

            _tcpServer = tcpServer;
            _listener = new TcpListener(address, port);
            
            
        }


        public  void Start()
        {

            _cancellationToken = new CancellationTokenSource();
            _listener.Start();

            Task.Run(async () => await ListenerLoop());


        }

        public void Stop()
        {
            _listener.Stop();
            _cancellationToken.Cancel();
            

        }
        public async Task ListenerLoop()
        {
            var lingerOption = new LingerOption(true, 0);
            while (!_cancellationToken.IsCancellationRequested)
            {
                

                var client = await _listener.AcceptTcpClientAsync();
                client.LingerState = lingerOption;
                _tcpServer.FireClientConnected(_tcpServer, client);

                Task.Run(async () => await ReadLoop(client));
            }
        }

        public async Task ReadLoop(TcpClient client)
        {
            var delimiter = _tcpServer.Delimiter;
            var queuedMsg = new List<byte>();
            while (!_cancellationToken.IsCancellationRequested)
            {


                if (!IsSocketConnected(client.Client))
                {
                    
                    _tcpServer.FireClientDisconnected(_tcpServer, client);
                    client.GetStream().Close();
                    client.Close();
                    
                    break;
                }

                var bytesAvailable = client.Available;
                if (bytesAvailable == 0) continue;

                var bytesReceived = new List<byte>();

                while (client.Available > 0 && client.Connected)
                {
                    
                    var nextByte = new byte[1];
                    await client.GetStream().ReadAsync(nextByte, 0, 1, _cancellationToken.Token);
                    bytesReceived.AddRange(nextByte);

                    if (nextByte[0] == delimiter)
                    {
                        var msg = Encoding.ASCII.GetString(queuedMsg.ToArray()) ;
                        _tcpServer.FireMessageReceived(_tcpServer, client, msg);

                        
                        queuedMsg.Clear();

                    }
                    else
                    {
                        queuedMsg.AddRange(nextByte);
                    }
                }
                

            }
        }

        private static bool IsSocketConnected(Socket s)
        {
            if (s == null) return false;
            // https://stackoverflow.com/questions/2661764/how-to-check-if-a-socket-is-connected-disconnected-in-c
            var part1 = s.Poll(1000, SelectMode.SelectRead);
            var part2 = (s.Available == 0);
            return (!part1 || !part2) && s.Connected;
        }

        public void Dispose()
        {
            _cancellationToken?.Dispose();
            
        }
    }
}
