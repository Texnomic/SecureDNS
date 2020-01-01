using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

        public bool IsRunning { get; private set; }

        private const int Port = 53;

        private readonly IPEndPoint IPEndPoint;

        private readonly UdpClient UdpClient;

        public ProxyServer(IAsyncResponsibilityChain<IMessage, IMessage> ResponsibilityChain, ILogger Logger, int Threads = 0)
        {
            this.Threads = Threads == 0 ? Environment.ProcessorCount : Threads;

            this.ResponsibilityChain = ResponsibilityChain;

            this.Logger = Logger;

            BinarySerializer = new BinarySerializer();

            Workers = new List<Task>();

            UdpClient = new UdpClient();

            IPEndPoint = new IPEndPoint(IPAddress.Any, Port);

            UdpClient.Client.Bind(IPEndPoint);

            IncomingQueue = new BufferBlock<(IMessage, IPEndPoint)>();

            OutgoingQueue = new BufferBlock<(IMessage, IPEndPoint)>();
        }

        public async Task StartAsync(CancellationToken CancellationToken)
        {
            if (IsRunning) return;

            IsRunning = true;

            for (var I = 0; I < Threads; I++)
            {
                Workers.Add(Task.Factory.StartNew(ReceiveAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default));
                Workers.Add(Task.Factory.StartNew(ResolveAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default));
                Workers.Add(Task.Factory.StartNew(SendAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default));
            }

            Logger.Information("Server Started with {@Threads} Threads. Listening On {@IPEndPoint}", Threads, IPEndPoint);

            Started?.Invoke(this, EventArgs.Empty);

            await Task.WhenAll(Workers);
        }

        public async Task StopAsync(CancellationToken CancellationToken)
        {
            if (!IsRunning) return;

            IsRunning = false;

            await Task.WhenAll(Workers);

            IncomingQueue.Complete();
            OutgoingQueue.Complete();

            Logger.Information("Server Stopped.");

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
                Logger.Error(Error, "{@Error} Occurred While Deserializing Message.", Error);

                Logger.Debug(Error, "{@Error} Occurred While Deserializing {@Bytes}.", Error, Bytes);

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
                Logger.Error(Error, "{@Error} Occurred While Serializing {@Message}.", Error, Message);

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
                Logger.Error(Error, "{@Error} Occurred While Resolving {@Query}.", Error, Query);

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
            while (IsRunning)
            {
                try
                {
                    var Result = await UdpClient.ReceiveAsync();

                    var Message = await DeserializeAsync(Result.Buffer);

                    switch (Message.MessageType)
                    {
                        case MessageType.Query:
                            {
                                await IncomingQueue.SendAsync((Message, Result.RemoteEndPoint));

                                Logger.Information("Received {@Query} From {@RemoteEndPoint}.", Message,
                                    Result.RemoteEndPoint);

                                Queried?.Invoke(this, new QueriedEventArgs(Message, Result.RemoteEndPoint));

                                break;
                            }

                        case MessageType.Response:
                            {
                                await OutgoingQueue.SendAsync((Message, Result.RemoteEndPoint));

                                Logger.Debug("Queueing (Outgoing) Format Error {@Answer} To {@RemoteEndPoint}.", Message,
                                    Result.RemoteEndPoint);

                                break;
                            }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (SocketException Error)
                {
                    Logger.Error(Error, "{@Error} Occurred While Receiving Message.", Error);

                    Errored?.Invoke(this, new ErroredEventArgs(Error));
                }
                catch (Exception Error)
                {
                    Logger.Fatal(Error, "Fatal {@Error} Occurred While Receiving Message.", Error);

                    Errored?.Invoke(this, new ErroredEventArgs(Error));

                    await StopAsync(new CancellationToken(true));
                }
            }
        }

        private async Task ResolveAsync()
        {
            while (IsRunning)
            {
                var (Query, RemoteEndPoint) = await IncomingQueue.ReceiveAsync();

                try
                {
                    var Answer = await ExecuteAsync(Query);

                    await OutgoingQueue.SendAsync((Answer, RemoteEndPoint));

                    Logger.Information("Resolved {@Query} with {@ResponseCode} {@Answer} To {@RemoteEndPoint}.", Query,
                        Answer.ResponseCode, Answer, RemoteEndPoint);

                    Resolved?.Invoke(this, new ResolvedEventArgs(Query, Answer, RemoteEndPoint));
                }
                catch (Exception Error)
                {
                    Logger.Fatal(Error, "Fatal {@Error} Occurred While Receiving Message.", Error);

                    Errored?.Invoke(this, new ErroredEventArgs(Error));

                    await StopAsync(new CancellationToken(true));
                }
            }
        }

        private async Task SendAsync()
        {
            while (IsRunning)
            {
                try
                {
                    var (Answer, RemoteEndPoint) = await OutgoingQueue.ReceiveAsync();

                    var Bytes = await SerializeAsync(Answer);

                    await UdpClient.SendAsync(Bytes, Bytes.Length, RemoteEndPoint);

                    Logger.Information("Sent {@Answer} To {@RemoteEndPoint}.", Answer, RemoteEndPoint);

                    Answered?.Invoke(this, new AnsweredEventArgs(Answer, RemoteEndPoint));

                }
                catch (Exception Error)
                {
                    Logger.Fatal(Error, "Fatal {@Error} Occurred While Sending Message.", Error);

                    Errored?.Invoke(this, new ErroredEventArgs(Error));

                    await StopAsync(new CancellationToken(true));
                }
            }
        }

        private void Debug(byte[] Bytes)
        {
            var Binary = Bytes.ToList().Select(Byte => Convert.ToString(Byte, 2).PadLeft(8, '0')).ToList();
            var Message = string.Join(' ', Binary);
            Console.WriteLine(Message);
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
