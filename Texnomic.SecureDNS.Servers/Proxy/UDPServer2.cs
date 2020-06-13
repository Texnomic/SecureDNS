using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PipelineNet.MiddlewareResolver;

using Serilog;
using Texnomic.SecureDNS.Abstractions.Enums;
using Texnomic.SecureDNS.Core;
using Texnomic.SecureDNS.Extensions;
using Texnomic.SecureDNS.Serialization;
using Texnomic.SecureDNS.Servers.Proxy.Options;
using Texnomic.SecureDNS.Servers.Proxy.ResponsibilityChain;

namespace Texnomic.SecureDNS.Servers.Proxy
{
    public sealed class UDPServer2 : IHostedService, IDisposable
    {
        private readonly ILogger Logger;
        private readonly List<Task> Threads;
        private readonly IOptionsMonitor<ProxyServerOptions> Options;
        private readonly IMiddlewareResolver MiddlewareResolver;
        private readonly IOptionsMonitor<ProxyResponsibilityChainOptions> ProxyResponsibilityChainOptions;
        private readonly UdpClient Server;

        private CancellationToken CancellationToken;

        private int Affinity;

        public UDPServer2(IOptionsMonitor<ProxyResponsibilityChainOptions> ProxyResponsibilityChainOptions, IOptionsMonitor<ProxyServerOptions> ProxyServerOptions, IMiddlewareResolver MiddlewareResolver, ILogger Logger)
        {
            Options = ProxyServerOptions;

            this.MiddlewareResolver = MiddlewareResolver;

            this.ProxyResponsibilityChainOptions = ProxyResponsibilityChainOptions;

            this.Logger = Logger;

            Threads = new List<Task>();

            Server = new UdpClient();

            Affinity = 0;
        }

        public async Task StartAsync(CancellationToken Token)
        {
            CancellationToken = Token;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //https://stackoverflow.com/questions/5199026/c-sharp-async-udp-listener-socketexception
                Server.Client.IOControl(-1744830452, new byte[4], null);
            }

            Server.Client.Bind(Options.CurrentValue.IPEndPoint);

            for (var I = 0; I < ProxyServerOptions.Threads; I++)
            {
                Threads.Add(Task.Factory.StartNew(ResolveAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
            }

            Logger?.Information("UDP Server Started with {@Threads} Threads. Listening On {@IPEndPoint}", ProxyServerOptions.Threads, Options.CurrentValue.IPEndPoint.ToString());

            await Task.Yield();
        }


        public async Task StopAsync(CancellationToken Token)
        {
            await Task.WhenAll(Threads);

            Logger?.Information("UDP Server Stopped.");
        }

        private async Task ResolveAsync()
        {
            var AffinityPointer = (0b00000001 << Affinity++);
            
            var ProcessThread = Process.GetCurrentProcess().Threads[Thread.CurrentThread.ManagedThreadId];

            ProcessThread.ProcessorAffinity = (IntPtr) AffinityPointer;

            ProcessThread.PriorityLevel = ThreadPriorityLevel.Highest;

            Logger?.Debug("UDP Server Started Thread {@Thread} with Affinity {@Affinity} & Priority {@Priority}.", Thread.CurrentThread.ManagedThreadId, AffinityPointer, ThreadPriorityLevel.Highest);
            
            var ProxyResponsibilityChain = new ProxyResponsibilityChain(ProxyResponsibilityChainOptions, MiddlewareResolver);

            while (!CancellationToken.IsCancellationRequested)
            {
                UdpReceiveResult Client = default; 

                try
                {
                    Client = await Server.ReceiveAsync().WithCancellation(CancellationToken);

                    var Query = DnSerializer.Deserialize(Client.Buffer);

                    var Answer = await ProxyResponsibilityChain.Execute(Query);

                    var RawAnswer = DnSerializer.Serialize(Answer);

                    await Server.SendAsync(RawAnswer, RawAnswer.Length, Client.RemoteEndPoint);

                    Logger?.Information("Resolved Query {@QueryID} To {@RemoteEndPoint} with {@ResponseCode} For {@Domain}.", 
                                    Query.ID, Client.RemoteEndPoint.ToString(), Answer?.ResponseCode, Answer?.Questions.FirstOrDefault()?.Domain.Name);

                    Logger?.Debug("Resolved {@Answer} For {@RemoteEndPoint}", Answer, Client.RemoteEndPoint);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception Error)
                {
                    Logger?.Debug(Error, $"{@Error} Occurred While Processing.", Error);
                }
                finally
                {
                    var Message = new Message()
                    {
                        ID = BinaryPrimitives.ReadUInt16BigEndian(Client.Buffer[..2]),
                        MessageType = MessageType.Response,
                        ResponseCode = ResponseCode.ServerFailure,
                    };

                    var RawMessage = DnSerializer.Serialize(Message);

                    await Server.SendAsync(RawMessage, RawMessage.Length, Client.RemoteEndPoint);
                }
            }
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
                Server.Dispose();
            }

            IsDisposed = true;
        }

        ~UDPServer2()
        {
            Dispose(false);
        }

    }
}
