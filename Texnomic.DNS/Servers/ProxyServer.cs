using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;
using PipelineNet.ChainsOfResponsibility;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Servers
{
    public class ProxyServer : IHostedService, IDisposable
    {
        private readonly int Threads;
        private readonly List<Task> Workers;
        private readonly IAsyncResponsibilityChain<Message, Message> ResponsibilityChain;
        private readonly BufferBlock<UdpReceiveResult> SerializerQueue;
        private readonly BufferBlock<(Message, IPEndPoint)> IncomingQueue, OutgoingQueue;

        public event EventHandler<RequestedEventArgs> Requested;
        public event EventHandler<ResolvedEventArgs> Resolved;
        public event EventHandler<RespondedEventArgs> Responded;
        public event EventHandler<EventArgs> Started;
        public event EventHandler<EventArgs> Stopped;
        public event EventHandler<ErrorEventArgs> Error;

        public bool IsRunning { get; private set; }

        private const int Port = 53;

        private readonly UdpClient UdpClient;

        private readonly IPEndPoint IPEndPoint;


        public ProxyServer(IAsyncResponsibilityChain<Message, Message> ResponsibilityChain, int Threads = 0)
        {
            this.Threads = Threads == 0 ? Environment.ProcessorCount : Threads;

            this.ResponsibilityChain = ResponsibilityChain;

            Workers = new List<Task>();

            UdpClient = new UdpClient();

            IPEndPoint = new IPEndPoint(IPAddress.Any, Port);

            UdpClient.Client.Bind(IPEndPoint);

            IncomingQueue = OutgoingQueue = new BufferBlock<(Message, IPEndPoint)>();

            SerializerQueue = new BufferBlock<UdpReceiveResult>();
        }

        public async Task StartAsync(CancellationToken CancellationToken)
        {
            if (IsRunning) return;

            IsRunning = true;

            Workers.Add(Task.Factory.StartNew(ReceiveAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default));

            for (var I = 0; I < Threads; I++)
            {
                Workers.Add(Task.Factory.StartNew(SerializeAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default));
                Workers.Add(Task.Factory.StartNew(ResolveAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default));
                Workers.Add(Task.Factory.StartNew(ForwardAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default));
            }

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
            SerializerQueue.Complete();

            Stopped?.Invoke(this, EventArgs.Empty);
        }

        private async Task ReceiveAsync()
        {
            while (IsRunning)
            {
                try
                {
                    var Result = await UdpClient.ReceiveAsync();

                    await SerializerQueue.SendAsync(Result);

                    Requested?.Invoke(this, new RequestedEventArgs(Result.RemoteEndPoint));
                }
                catch (Exception Error)
                {
                    this.Error?.Invoke(this, new ErrorEventArgs(Error));
                    Console.WriteLine(Error.Message);
                }
            }
        }

        private async Task SerializeAsync()
        {
            while (IsRunning)
            {
                Message Request = null;

                var Result = await SerializerQueue.ReceiveAsync();

                try
                {
                    Request = await Message.FromArrayAsync(Result.Buffer);

                    await IncomingQueue.SendAsync((Request, Result.RemoteEndPoint));
                }
                catch (Exception Error)
                {
                    this.Error?.Invoke(this, new ErrorEventArgs(Error));
                    Console.WriteLine(Error.Message);

                    await SendError(Request?.ID ?? 0, Result.RemoteEndPoint);
                }
            }
        }
        private async Task ResolveAsync()
        {
            while (IsRunning)
            {
                var (Request, Remote) = await IncomingQueue.ReceiveAsync();

                try
                {
                    var Response = await ResponsibilityChain.Execute(Request);

                    if (Response == null)
                    {

                    }

                    await OutgoingQueue.SendAsync((Response, Remote));

                    Resolved?.Invoke(this, new ResolvedEventArgs(Request, Response, Remote));
                }
                catch (Exception Error)
                {
                    this.Error?.Invoke(this, new ErrorEventArgs(Error));
                    Console.WriteLine(Error.Message);

                    await SendError(Request.ID, Remote);
                }
            }
        }
        private async Task ForwardAsync()
        {
            while (IsRunning)
            {
                try
                {
                    var (Response, Remote) = await OutgoingQueue.ReceiveAsync();

                    var Bytes = Response.ToArray();

                    await UdpClient.SendAsync(Bytes, Bytes.Length, Remote);

                    Responded?.Invoke(this, new RespondedEventArgs(Response, Remote));
                }
                catch (Exception Error)
                {
                    this.Error?.Invoke(this, new ErrorEventArgs(Error));
                    Console.WriteLine(Error.Message);
                }
            }
        }

        private async Task SendError(ushort ID, IPEndPoint RemoteEndPoint)
        {
            var Response = new Message()
            {
                ID = ID,
                MessageType = MessageType.Response,
                ResponseCode = ResponseCode.FormatError,
            };

            var ResponseBytes = Response.ToArray();

            await UdpClient.SendAsync(ResponseBytes, ResponseBytes.Length, RemoteEndPoint);

            Responded?.Invoke(this, new RespondedEventArgs(Response, RemoteEndPoint));
        }

        public class RequestedEventArgs : EventArgs
        {
            public readonly IPEndPoint EndPoint;

            public RequestedEventArgs(IPEndPoint EndPoint)
            {
                this.EndPoint = EndPoint;
            }
        }

        public class ResolvedEventArgs : EventArgs
        {
            public readonly Message Request;
            public readonly Message Response;
            public readonly IPEndPoint EndPoint;

            public ResolvedEventArgs(Message Request, Message Response, IPEndPoint EndPoint)
            {
                this.Request = Request;
                this.Response = Response;
                this.EndPoint = EndPoint;
            }
        }

        public class RespondedEventArgs : EventArgs
        {
            public readonly Message Response;
            public readonly IPEndPoint EndPoint;

            public RespondedEventArgs(Message Response, IPEndPoint EndPoint)
            {
                this.Response = Response;
                this.EndPoint = EndPoint;
            }
        }

        public class ErrorEventArgs : EventArgs
        {
            public readonly Exception Error;

            public ErrorEventArgs(Exception Error)
            {
                this.Error = Error;
            }
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
