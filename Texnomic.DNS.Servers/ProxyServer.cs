using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using BinarySerialization;
using Microsoft.Extensions.Hosting;
using Nethereum.Util;
using PipelineNet.ChainsOfResponsibility;
using Serilog;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Servers.Events;
using Texnomic.DNS.Servers.Extensions;
using Texnomic.DNS.Servers.Options;
using Microsoft.Extensions.Options;
using PipelineNet.MiddlewareResolver;
using Texnomic.DNS.Servers.ResponsibilityChain;

namespace Texnomic.DNS.Servers
{
    public sealed class ProxyServer : IHostedService, IDisposable
    {
        private readonly ILogger Logger;
        private readonly List<Task> Workers;
        private readonly IOptionsMonitor<ProxyServerOptions> Options;
        private readonly BinarySerializer BinarySerializer;
        private readonly IMiddlewareResolver MiddlewareResolver;
        private readonly IOptionsMonitor<ProxyResponsibilityChainOptions> ProxyResponsibilityChainOptions;
        private readonly BufferBlock<(IMessage, IPEndPoint)> IncomingQueue;
        private readonly BufferBlock<(IMessage, IPEndPoint)> OutgoingQueue;

        public event EventHandler<QueriedEventArgs> Queried;
        public event EventHandler<ResolvedEventArgs> Resolved;
        public event EventHandler<AnsweredEventArgs> Answered;
        public event EventHandler<EventArgs> Started;
        public event EventHandler<EventArgs> Stopped;
        public event EventHandler<ErroredEventArgs> Errored;

        private CancellationToken CancellationToken;

        private UdpClient UdpClient;

        public ProxyServer(IOptionsMonitor<ProxyResponsibilityChainOptions> ProxyResponsibilityChainOptions,
            IOptionsMonitor<ProxyServerOptions> ProxyServerOptions,
            IMiddlewareResolver MiddlewareResolver,
            ILogger Logger)
        {
            Options = ProxyServerOptions;

            this.MiddlewareResolver = MiddlewareResolver;

            this.ProxyResponsibilityChainOptions = ProxyResponsibilityChainOptions;

            this.Logger = Logger;

            BinarySerializer = new BinarySerializer();

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

            for (var I = 0; I < Options.CurrentValue.Threads; I++)
            {
                Workers.Add(Task.Factory.StartNew(ReceiveAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
                Workers.Add(Task.Factory.StartNew(ResolveAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
                Workers.Add(Task.Factory.StartNew(SendAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
            }

            Logger?.Information("Server Started with {@Threads} Threads. Listening On {@IPEndPoint}", Options.CurrentValue.Threads, Options.CurrentValue.IPEndPoint.ToString());

            Started?.Invoke(this, EventArgs.Empty);

            await Task.Yield();
        }

        public List<TaskStatus> Status()
        {
            return Workers.Select(Worker => Worker.Status).ToList();
        }

        public async Task StopAsync(CancellationToken Token)
        {
            UdpClient.Close();

            await Task.WhenAll(Workers);

            IncomingQueue.Complete();
            OutgoingQueue.Complete();

            Logger?.Information("Server Stopped.");

            Stopped?.Invoke(this, EventArgs.Empty);
        }

        private async ValueTask<Message> DeserializeAsync(byte[] Bytes)
        {
            try
            {
                return await BinarySerializer.DeserializeAsync<Message>(Bytes);
            }
            catch (Exception Error)
            {
                Logger?.Error(Error, "{@Error} Occurred While Deserializing Message.", Error);

                Logger?.Debug(Error, "{@Error} Occurred While Deserializing {@Bytes}.", Error, Bytes);

                Errored?.Invoke(this, new ErroredEventArgs(Error));

                return new Message()
                {
                    ID = BitConverter.ToUInt16(Bytes.Slice(2)),
                    MessageType = MessageType.Response,
                    ResponseCode = ResponseCode.FormatError,
                };
            }
        }
        private async ValueTask<byte[]> SerializeAsync(IMessage Message)
        {
            try
            {
                return await BinarySerializer.SerializeAsync(Message);
            }
            catch (Exception Error)
            {
                Logger?.Error(Error, "{@Error} Occurred While Serializing {@Message}.", Error, Message);

                Errored?.Invoke(this, new ErroredEventArgs(Error));

                var ErrorMessage = new Message()
                {
                    ID = Message.ID,
                    MessageType = MessageType.Response,
                    ResponseCode = ResponseCode.FormatError,
                };

                return await BinarySerializer.SerializeAsync(ErrorMessage);
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

                    var Message = await DeserializeAsync(Result.Buffer);

                    switch (Message.MessageType)
                    {
                        case MessageType.Query:
                            {
                                await IncomingQueue.SendAsync((Message, Result.RemoteEndPoint), CancellationToken);

                                Logger?.Verbose("Received {@Query} From {@RemoteEndPoint}.", Message,
                                    Result.RemoteEndPoint.ToString());

                                Queried?.Invoke(this, new QueriedEventArgs(Message, Result.RemoteEndPoint));

                                break;
                            }

                        case MessageType.Response:
                            {
                                await OutgoingQueue.SendAsync((Message, Result.RemoteEndPoint), CancellationToken);

                                Logger?.Debug("Queueing (Outgoing) Format Error {@Answer} To {@RemoteEndPoint}.", Message,
                                    Result.RemoteEndPoint.ToString());

                                break;
                            }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception Error) when (Handler(Error))
                {
                    Logger?.Error(Error, "{@Error} Occurred While Receiving Message.", Error);

                    Errored?.Invoke(this, new ErroredEventArgs(Error));
                }
                catch (Exception Error)
                {
                    Logger?.Fatal(Error, "Fatal {@Error} Occurred While Receiving Message.", Error);

                    Errored?.Invoke(this, new ErroredEventArgs(Error));

                    await StopAsync(CancellationToken);
                }
            }
        }

        private async Task ResolveAsync()
        {
            Thread.CurrentThread.Name = "Resolver";

            var ResponsibilityChain = new ProxyResponsibilityChain(ProxyResponsibilityChainOptions, MiddlewareResolver);

            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var (Query, RemoteEndPoint) = await IncomingQueue.ReceiveAsync(CancellationToken)
                        .WithCancellation(CancellationToken);

                    var Answer = await ResponsibilityChain.Execute(Query);

                    await OutgoingQueue.SendAsync((Answer, RemoteEndPoint), CancellationToken);

                    Logger?.Information("Resolved {@Answer} For {@Domain} with {@ResponseCode} To {@RemoteEndPoint}.", Answer, Answer.Questions[0].Domain.Name, Answer.ResponseCode, RemoteEndPoint.ToString());

                    Resolved?.Invoke(this, new ResolvedEventArgs(Query, Answer, RemoteEndPoint));
                }
                catch (Exception Error) when (Handler(Error))
                {
                    Logger?.Error(Error, "{@Error} Occurred While Resolving Message.", Error);

                    Errored?.Invoke(this, new ErroredEventArgs(Error));
                }
                catch (Exception Error)
                {
                    Logger?.Fatal(Error, "Fatal {@Error} Occurred While Resolving Message.", Error);

                    Errored?.Invoke(this, new ErroredEventArgs(Error));

                    await StopAsync(CancellationToken);
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

                    var Bytes = await SerializeAsync(Answer);

                    await UdpClient.SendAsync(Bytes, Bytes.Length, RemoteEndPoint);

                    Logger?.Verbose("Sent {@Answer} To {@RemoteEndPoint}.", Answer, RemoteEndPoint.ToString());

                    Answered?.Invoke(this, new AnsweredEventArgs(Answer, RemoteEndPoint));

                }
                catch (Exception Error) when(Handler(Error))
                {
                    Logger?.Error(Error, "{@Error} Occurred While Sending Message.", Error);

                    Errored?.Invoke(this, new ErroredEventArgs(Error));
                }
                catch (Exception Error)
                {
                    Logger?.Fatal(Error, "Fatal {@Error} Occurred While Sending Message.", Error);

                    Errored?.Invoke(this, new ErroredEventArgs(Error));

                    await StopAsync(CancellationToken);
                }
            }
        }


        private static bool Handler(Exception Error)
        {
            switch (Error)
            {
                case TimeoutException Timeout:
                case OperationCanceledException OperationCanceled:
                case ArgumentNullException ArgumentNull:
                case ArgumentOutOfRangeException ArgumentOutOfRange:
                case InvalidOperationException InvalidOperation:
                case CryptographicUnexpectedOperationException CryptographicUnexpectedOperation:
                    {
                        return true;
                    }
                default:
                    return false;
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
                UdpClient.Dispose();
            }

            IsDisposed = true;
        }

        ~ProxyServer()
        {
            Dispose(false);
        }
    }
}
