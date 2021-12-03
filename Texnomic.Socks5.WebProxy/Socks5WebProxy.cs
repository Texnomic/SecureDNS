using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Texnomic.Socks5.Options;
using Texnomic.Socks5.WebProxy.Options;

namespace Texnomic.Socks5.WebProxy
{
    public class Socks5WebProxy : IHostedService, IDisposable
    {
        private readonly IOptionsMonitor<Socks5WebProxyOptions> ProxyOptions;
        private readonly IOptionsMonitor<Socks5Options> Socks5Options;
        private Socket ServerSocket;
        private CancellationToken CancellationToken;

        public Socks5WebProxy(IOptionsMonitor<Socks5WebProxyOptions> ProxyOptions, IOptionsMonitor<Socks5Options> Socks5Options)
        {
            this.ProxyOptions = ProxyOptions;
            this.Socks5Options = Socks5Options;
            ProxyOptions.OnChange(async (ChangedOptions) => await StartAsync(CancellationToken));
        }

        public async Task StartAsync(CancellationToken Token)
        {
            CancellationToken = Token;

            ServerSocket = new Socket(ProxyOptions.CurrentValue.IPAddress.AddressFamily, ProxyOptions.CurrentValue.SocketType, ProxyOptions.CurrentValue.ProtocolType);

            ServerSocket.Bind(ProxyOptions.CurrentValue.IPEndPoint);

            ServerSocket.Listen(ProxyOptions.CurrentValue.BackLog);

            _ = Task.Factory.StartNew(ReceiveAsync, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

            await Task.Yield();
        }

        public async Task StopAsync(CancellationToken Token)
        {
            await Task.Yield();
        }

        private async Task ReceiveAsync()
        {
            while (ServerSocket.IsBound && !CancellationToken.IsCancellationRequested)
            {
                var ClientSocket = await ServerSocket.AcceptAsync();

                var Buffer = new byte[512];

                var Size = await ClientSocket.ReceiveAsync(Buffer, SocketFlags.None);

                Buffer = Buffer[..Size];

                var Lines = Encoding.UTF8.GetString(Buffer);

                var ProxyRequest = Lines.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

                if (ProxyRequest.Length < 2) throw new Exception();

                var Connect = ProxyRequest[0].Split(' ');

                if (Connect.Length != 3) throw new Exception();
                if (Connect[0] != "CONNECT" && Connect[0] != "GET") throw new Exception();
                if (Connect[2] != "HTTP/1.1") throw new Exception();

                var Host = ProxyRequest[1].Split(' ');

                if (Host.Length != 2) throw new Exception();
                if (Host[0] != "Host:") throw new Exception();

                var EndPoint = Host[1].Split(':');

                var Port = EndPoint.Length == 2 ? int.Parse(EndPoint[1]) : Connect[1].StartsWith("http://") ? 80 : 443;

                var Socks5 = new Socks5(Socks5Options);

                var Socks5Socket = await Socks5.Connect(EndPoint[0], Port);

                var ProxyResponse = Encoding.UTF8.GetBytes("HTTP/1.1 200 Connection established\r\nProxy-Agent: Texnomic-Socks5WebProxy\r\n\r\n");

                await ClientSocket.SendAsync(ProxyResponse, SocketFlags.None);

                _ = Pipe(ClientSocket, Socks5Socket);

                _ = Pipe(Socks5Socket, ClientSocket);
            }
        }

        private static async Task Pipe(Socket Reader, Socket Writer)
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

        private bool IsDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool Disposing)
        {
            if (IsDisposed) return;

            if (Disposing)
            {
                ServerSocket.Dispose();
            }

            IsDisposed = true;
        }

        ~Socks5WebProxy()
        {
            Dispose(false);
        }

    }
}
