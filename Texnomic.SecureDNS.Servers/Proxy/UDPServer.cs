using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Nethereum.Util;
using PipelineNet.MiddlewareResolver;
using Serilog;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;
using Texnomic.SecureDNS.Core;
using Texnomic.SecureDNS.Extensions;
using Texnomic.SecureDNS.Serialization;
using Texnomic.SecureDNS.Servers.Proxy.Options;
using Texnomic.SecureDNS.Servers.Proxy.ResponsibilityChain;

namespace Texnomic.SecureDNS.Servers.Proxy
{
    public sealed class UDPServer : IHostedService, IDisposable
    {
        private readonly ILogger Logger;
        private readonly List<Task> Workers;
        private readonly IOptionsMonitor<ProxyServerOptions> Options;
        private readonly IMiddlewareResolver MiddlewareResolver;
        private readonly IOptionsMonitor<ProxyResponsibilityChainOptions> ProxyResponsibilityChainOptions;
        private readonly BufferBlock<(IMessage, IPEndPoint)> IncomingQueue;
        private readonly BufferBlock<(IMessage, IPEndPoint)> OutgoingQueue;


        private CancellationToken CancellationToken;

        private UdpClient UdpClient;

        public UDPServer(IOptionsMonitor<ProxyResponsibilityChainOptions> ProxyResponsibilityChainOptions,
            IOptionsMonitor<ProxyServerOptions> ProxyServerOptions,
            IMiddlewareResolver MiddlewareResolver,
            ILogger Logger)
        {
            Options = ProxyServerOptions;

            this.MiddlewareResolver = MiddlewareResolver;

            this.ProxyResponsibilityChainOptions = ProxyResponsibilityChainOptions;

            this.Logger = Logger;

            Workers = new List<Task>();

            IncomingQueue = new BufferBlock<(IMessage, IPEndPoint)>();

            OutgoingQueue = new BufferBlock<(IMessage, IPEndPoint)>();
        }

        public async Task StartAsync(CancellationToken Token)
        {
            CancellationToken = Token;

            UdpClient = new UdpClient();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //https://stackoverflow.com/questions/5199026/c-sharp-async-udp-listener-socketexception
                UdpClient.Client.IOControl(-1744830452, new byte[4], null);
            }

            UdpClient.Client.Bind(Options.CurrentValue.IPEndPoint);

            for (var I = 0; I < ProxyServerOptions.Threads; I++)
            {
                Workers.Add(Task.Factory.StartNew(ReceiveAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
                Workers.Add(Task.Factory.StartNew(ResolveAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
                Workers.Add(Task.Factory.StartNew(SendAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
            }

            Logger?.Information("UDP Server Started with {@Threads} Threads. Listening On {@IPEndPoint}", ProxyServerOptions.Threads, Options.CurrentValue.IPEndPoint.ToString());

            await Task.Yield();
        }

        public List<TaskStatus> Status()
        {
            return Workers.Select(Worker => Worker.Status).ToList();
        }

        public async Task StopAsync(CancellationToken Token)
        {
            await Task.WhenAll(Workers);

            IncomingQueue.Complete();
            OutgoingQueue.Complete();

            Logger?.Information("Server Stopped.");
        }

        private IMessage Deserialize(byte[] Bytes)
        {
            try
            {
                return DnSerializer.Deserialize(Bytes);
            }
            catch (Exception Error)
            {
                Logger?.Error(Error, "{@Error} Occurred While Deserializing {@Bytes}.", Error, BitConverter.ToString(Bytes).Replace("-", ", 0x"));

                return new Message()
                {
                    ID = BitConverter.ToUInt16(Bytes.Slice(2)),
                    MessageType = MessageType.Response,
                    ResponseCode = ResponseCode.FormatError,
                };
            }
        }
        private byte[] Serialize(IMessage Message)
        {
            try
            {
                return DnSerializer.Serialize(Message);
            }
            catch (Exception Error)
            {
                Logger?.Error(Error, "{@Error} Occurred While Serializing {@Message}.", Error, Message);

                var ErrorMessage = new Message()
                {
                    ID = Message.ID,
                    MessageType = MessageType.Response,
                    ResponseCode = ResponseCode.FormatError,
                };

                return DnSerializer.Serialize(ErrorMessage);
            }
        }

        private async Task ReceiveAsync()
        {
            Thread.CurrentThread.Name = "Receiver";


            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var Result = await UdpClient.ReceiveAsync()
                                                .WithCancellation(CancellationToken);

                    var Message = Deserialize(Result.Buffer);

                    await IncomingQueue.SendAsync((Message, Result.RemoteEndPoint), CancellationToken);

                    Logger?.Verbose("Received {@Query} From {@RemoteEndPoint}.", Message, Result.RemoteEndPoint.ToString());
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception Error)
                {
                    Logger?.Error(Error, "{@Error} Occurred While Receiving Message.", Error);
                }
            }
        }

        private async Task ResolveAsync()
        {
            Thread.CurrentThread.Name = "Resolver";

            var ResponsibilityChain = new ProxyResponsibilityChain(ProxyResponsibilityChainOptions, MiddlewareResolver);

            IMessage Query = null;

            IPEndPoint RemoteEndPoint = null;

            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    (Query, RemoteEndPoint) = await IncomingQueue.ReceiveAsync(CancellationToken)
                                                                 .WithCancellation(CancellationToken);

                    var Answer = await ResponsibilityChain.Execute(Query);

                    await OutgoingQueue.SendAsync((Answer, RemoteEndPoint), CancellationToken);

                    Logger?.Information("Resolved {@Answer} For {@Domain} with {@ResponseCode} To {@RemoteEndPoint}.", Answer, Answer.Questions.First().Domain.Name, Answer.ResponseCode, RemoteEndPoint.ToString());
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ObjectDisposedException Error)
                {
                    ResponsibilityChain = new ProxyResponsibilityChain(ProxyResponsibilityChainOptions, MiddlewareResolver);

                    var ErrorMessage = Handle(Error, Query.ID, "Resolving", ResponseCode.ServerFailure);

                    await OutgoingQueue.SendAsync((ErrorMessage, RemoteEndPoint), CancellationToken);
                }
                catch (Exception Error)
                {
                    var ErrorMessage = Handle(Error, Query.ID, "Resolving", ResponseCode.ServerFailure);

                    await OutgoingQueue.SendAsync((ErrorMessage, RemoteEndPoint), CancellationToken);
                }
            }
        }

        private async Task SendAsync()
        {
            Thread.CurrentThread.Name = "Sender";

            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var (Answer, RemoteEndPoint) = await OutgoingQueue.ReceiveAsync(CancellationToken)
                                                                             .WithCancellation(CancellationToken);

                    var Bytes = Serialize(Answer);

                    await UdpClient.SendAsync(Bytes, Bytes.Length, RemoteEndPoint);

                    Logger?.Verbose("Sent {@Answer} To {@RemoteEndPoint}.", Answer, RemoteEndPoint.ToString());

                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception Error)
                {
                    Logger?.Error(Error, "{@Error} Occurred While Sending Message.", Error);
                }
            }
        }

        private IMessage Handle(Exception Error, ushort ID, string Stage, ResponseCode Response)
        {
            Logger?.Error(Error, $"{@Error} Occurred While {Stage} Message.", Error);

            return new Message()
            {
                ID = ID,
                MessageType = MessageType.Response,
                ResponseCode = Response,
            };
        }

        private IMessage Handle(Exception Error, byte[] Bytes, string Stage, ResponseCode Response)
        {
            Logger?.Error(Error, $"{@Error} Occurred While {Stage} {@Bytes}.", Error, BitConverter.ToString(Bytes).Replace("-", ", 0x"));

            return new Message()
            {
                ID = BitConverter.ToUInt16(Bytes.Slice(2)),
                MessageType = MessageType.Response,
                ResponseCode = Response
            };
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
                UdpClient.Dispose();
            }

            IsDisposed = true;
        }

        ~UDPServer()
        {
            Dispose(false);
        }

    }
}
