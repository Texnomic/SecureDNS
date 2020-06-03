using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Texnomic.Socks5.Options;
using Texnomic.Socks5.WebProxy.Options;

namespace Texnomic.Socks5.WebProxy
{
    public class Socks5WebProxy : IWebProxy
    {
        private readonly IOptionsMonitor<Socks5WebProxyOptions> ProxyOptions;
        private readonly IOptionsMonitor<Socks5Options> Socks5Options;
        private Socket ServerSocket;
        private CancellationTokenSource CancellationTokenSource;

        public Uri GetProxy(Uri Destination) => ProxyOptions.CurrentValue.Uri;
        public bool IsBypassed(Uri Host) => false;
        public ICredentials Credentials { get; set; }

        public Socks5WebProxy(IOptionsMonitor<Socks5WebProxyOptions> ProxyOptions, IOptionsMonitor<Socks5Options> Socks5Options)
        {
            this.ProxyOptions = ProxyOptions;
            this.Socks5Options = Socks5Options;
            ProxyOptions.OnChange(Initialize);
            Initialize(ProxyOptions.CurrentValue);
        }

        private void Initialize(Socks5WebProxyOptions Options)
        {
            CancellationTokenSource = new CancellationTokenSource(Options.Timeout);

            ServerSocket = new Socket(Options.IPAddress.AddressFamily, Options.SocketType, Options.ProtocolType);

            ServerSocket.Bind(Options.IPEndPoint);

            ServerSocket.Listen(Options.BackLog);

            Task.Factory.StartNew(ReceiveAsync, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }


        private async Task ReceiveAsync()
        {
            while (ServerSocket.IsBound)
            {
                var ClientSocket = await ServerSocket.AcceptAsync();

                var Buffer = new byte[512];

                var Size = await ClientSocket.ReceiveAsync(Buffer, SocketFlags.None);

                Buffer = Buffer[..Size];

                var Line = Encoding.UTF8.GetString(Buffer);

                var ProxyRequest = Line.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

                if (ProxyRequest.Length != 2) throw new Exception();

                var Connect = ProxyRequest[0].Split(' ');

                if (Connect.Length != 3) throw new Exception();
                if (Connect[0] != "CONNECT") throw new Exception();
                if (Connect[2] != "HTTP/1.1") throw new Exception();

                var Host = ProxyRequest[1].Split(' ');

                if (Host.Length != 2) throw new Exception();
                if (Host[0] != "Host:") throw new Exception();

                var EndPoint = Host[1].Split(':');

                var Socks5 = new Socks5(Socks5Options);

                var Socks5Socket = await Socks5.Tunnel(EndPoint[0], int.Parse(EndPoint[1]));

                var ProxyResponse = Encoding.UTF8.GetBytes("HTTP/1.1 200 Connection established\r\nProxy-Agent: Texnomic-Socks5WebProxy\r\n\r\n");

                await ClientSocket.SendAsync(ProxyResponse, SocketFlags.None);

                _ = Pipe(ClientSocket, Socks5Socket);

                _ = Pipe(Socks5Socket, ClientSocket);
            }
        }

        private async Task Pipe(Socket Reader, Socket Writer)
        {
            var Buffer = new byte[512];

            while (Reader.Connected && Writer.Connected)
            {
                var Size = await Reader.ReceiveAsync(Buffer, SocketFlags.None);

                if (Size == 0) break;

                await Writer.SendAsync(Buffer[..Size], SocketFlags.None);
            }

            Reader.Close();

            Writer.Close();
        }
    }
}
