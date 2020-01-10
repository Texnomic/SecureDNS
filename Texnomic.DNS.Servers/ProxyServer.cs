using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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

namespace Texnomic.DNS.Servers
{
    public class ProxyServer : IHostedService, IDisposable
    {
        private readonly int Threads;
        private readonly ILogger Logger;
        private readonly List<Task> Workers;
        private readonly BinarySerializer BinarySerializer;
        private readonly IAsyncResponsibilityChain<IMessage, IMessage> ResponsibilityChain;
        private readonly BufferBlock<(IMessage, IPEndPoint)> IncomingQueue;
        private readonly BufferBlock<(IMessage, IPEndPoint)> OutgoingQueue;

        public event EventHandler<QueriedEventArgs> Queried;
        public event EventHandler<ResolvedEventArgs> Resolved;
        public event EventHandler<AnsweredEventArgs> Answered;
        public event EventHandler<EventArgs> Started;
        public event EventHandler<EventArgs> Stopped;
        public event EventHandler<ErroredEventArgs> Errored;

        private CancellationToken CancellationToken;

        private const int Port = 53;

        private readonly IPEndPoint IPEndPoint;

        private readonly UdpClient UdpClient;

        public ProxyServer(IAsyncResponsibilityChain<IMessage, IMessage> ResponsibilityChain, ILogger Logger, IPEndPoint IPEndPoint = null, int Threads = 0)
        {
            this.Threads = Threads == 0 ? Environment.ProcessorCount : Threads;

            this.ResponsibilityChain = ResponsibilityChain;

            this.Logger = Logger;

            BinarySerializer = new BinarySerializer();

            Workers = new List<Task>();

            UdpClient = new UdpClient();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //https://stackoverflow.com/questions/5199026/c-sharp-async-udp-listener-socketexception
                UdpClient.Client.IOControl(-1744830452, new byte[4], null);
            }

            this.IPEndPoint = IPEndPoint ?? new IPEndPoint(IPAddress.Any, Port);

            UdpClient.Client.Bind(this.IPEndPoint);

            IncomingQueue = new BufferBlock<(IMessage, IPEndPoint)>();

            OutgoingQueue = new BufferBlock<(IMessage, IPEndPoint)>();
        }

        public async Task StartAsync(CancellationToken Token)
        {
            CancellationToken = Token;

            for (var I = 0; I < Threads; I++)
            {
                Workers.Add(Task.Factory.StartNew(ReceiveAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
                Workers.Add(Task.Factory.StartNew(ResolveAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
                Workers.Add(Task.Factory.StartNew(SendAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
            }

            Logger?.Information("Server Started with {@Threads} Threads. Listening On {@IPEndPoint}", Threads * 3, IPEndPoint.ToString());

            Started?.Invoke(this, EventArgs.Empty);

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
        private async ValueTask<IMessage> ExecuteAsync(IMessage Query)
        {
            try
            {
                return await ResponsibilityChain.Execute(Query);
            }
            catch (Exception Error)
            {
                Logger?.Error(Error, "{@Error} Occurred While Resolving {@Query}.", Error, Query);

                Errored?.Invoke(this, new ErroredEventArgs(Error));

                return new Message()
                {
                    ID = Query.ID,
                    MessageType = MessageType.Response,
                    ResponseCode = ResponseCode.FormatError,
                };
            }
        }

        private async Task ReceiveAsync()
        {
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

                                Logger?.Verbose("Received {@Query} From {@RemoteEndPoint}.", Message, Result.RemoteEndPoint.ToString());

                                Queried?.Invoke(this, new QueriedEventArgs(Message, Result.RemoteEndPoint));

                                break;
                            }

                        case MessageType.Response:
                            {
                                await OutgoingQueue.SendAsync((Message, Result.RemoteEndPoint), CancellationToken);

                                Logger?.Debug("Queueing (Outgoing) Format Error {@Answer} To {@RemoteEndPoint}.", Message, Result.RemoteEndPoint.ToString());

                                break;
                            }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (SocketException Error)
                {
                    Logger?.Error(Error, "{@Error} Occurred While Receiving Message.", Error);

                    Errored?.Invoke(this, new ErroredEventArgs(Error));
                }
                catch (OperationCanceledException Error)
                {
                    Logger?.Error(Error, "{@Error} Operation Canceled While Receiving Message.", Error);

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
            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var (Query, RemoteEndPoint) = await IncomingQueue.ReceiveAsync(CancellationToken)
                        .WithCancellation(CancellationToken);


                    var Answer = await ExecuteAsync(Query);

                    await OutgoingQueue.SendAsync((Answer, RemoteEndPoint), CancellationToken);

                    Logger?.Information("Resolved {@Query} with {@ResponseCode} {@Answer} To {@RemoteEndPoint}.", Query,
                        Answer.ResponseCode, Answer, RemoteEndPoint.ToString());

                    Resolved?.Invoke(this, new ResolvedEventArgs(Query, Answer, RemoteEndPoint));
                }
                catch (OperationCanceledException Error)
                {
                    Logger?.Error(Error, "{@Error} Operation Canceled While Resolving Message.", Error);

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
                catch (OperationCanceledException Error)
                {
                    Logger?.Error(Error, "{@Error} Operation Canceled While Sending Message.", Error);

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

        private Task<UdpReceiveResult> UdpReceiveAsync()
        {
            return Task.Factory.FromAsync(UdpClient.BeginReceive, EndReceive, this);
        }

        private static UdpReceiveResult EndReceive(IAsyncResult AsyncResult)
        {
            var Client = (UdpClient)AsyncResult.AsyncState;

            IPEndPoint RemoteEp = null;

            var Buffer = Client.EndReceive(AsyncResult, ref RemoteEp);

            return new UdpReceiveResult(Buffer, RemoteEp);
        }

        private bool IsDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
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
