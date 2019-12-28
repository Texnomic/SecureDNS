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
using PipelineNet.ChainsOfResponsibility;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Servers
{
    public class ProxyServer : IHostedService, IDisposable
    {
        private readonly int Threads;
        private readonly List<Task> Workers;
        private readonly BinarySerializer BinarySerializer;
        private readonly IAsyncResponsibilityChain<IMessage, IMessage> ResponsibilityChain;
        private readonly BufferBlock<(IMessage, IPEndPoint)> IncomingQueue, OutgoingQueue;

        public event EventHandler<RequestedEventArgs> Requested;
        public event EventHandler<ResolvedEventArgs> Resolved;
        public event EventHandler<RespondedEventArgs> Responded;
        public event EventHandler<EventArgs> Started;
        public event EventHandler<EventArgs> Stopped;
        public event EventHandler<ErrorEventArgs> Error;

        public bool IsRunning { get; private set; }

        private const int Port = 53;

        private readonly UdpClient UdpClient;

        public ProxyServer(IAsyncResponsibilityChain<IMessage, IMessage> ResponsibilityChain, int Threads = 0)
        {
            this.Threads = Threads == 0 ? Environment.ProcessorCount : Threads;

            this.ResponsibilityChain = ResponsibilityChain;

            BinarySerializer = new BinarySerializer();

            Workers = new List<Task>();

            UdpClient = new UdpClient();

            var IPEndPoint = new IPEndPoint(IPAddress.Any, Port);

            UdpClient.Client.Bind(IPEndPoint);

            IncomingQueue = new BufferBlock<(IMessage, IPEndPoint)>();

            OutgoingQueue = new BufferBlock<(IMessage, IPEndPoint)>();
        }

        public async Task StartAsync(CancellationToken CancellationToken)
        {
            if (IsRunning) return;

            IsRunning = true;

            Workers.Add(Task.Factory.StartNew(ReceiveAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default));

            for (var I = 0; I < Threads; I++)
            {
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

            Stopped?.Invoke(this, EventArgs.Empty);
        }

        private async Task ReceiveAsync()
        {
            while (IsRunning)
            {
                Message Request = null;

                try
                {
                    var Result = await UdpClient.ReceiveAsync();

                    //Debug(Result.Buffer);

                    Request = await BinarySerializer.DeserializeAsync<Message>(Result.Buffer);

                    await IncomingQueue.SendAsync((Request, Result.RemoteEndPoint));

                    Requested?.Invoke(this, new RequestedEventArgs(Request, Result.RemoteEndPoint));
                }
                catch (Exception Error)
                {
                    this.Error?.Invoke(this, new ErrorEventArgs(Request, null, Error));

                    Console.WriteLine(Error.Message);
                }
            }
        }

        private async Task ResolveAsync()
        {
            while (IsRunning)
            {
                IMessage Response = null;

                var (Request, Remote) = await IncomingQueue.ReceiveAsync();

                try
                {
                    Response = await ResponsibilityChain.Execute(Request);

                    await OutgoingQueue.SendAsync((Response, Remote));

                    Resolved?.Invoke(this, new ResolvedEventArgs(Request, Response, Remote));
                }
                catch (Exception Error)
                {
                    this.Error?.Invoke(this, new ErrorEventArgs(Request, Response, Error));

                    Console.WriteLine(Error.Message);

                    await SendError(Request.ID, Remote);
                }
            }
        }
        private async Task ForwardAsync()
        {
            while (IsRunning)
            {
                var (Response, Remote) = await OutgoingQueue.ReceiveAsync();

                try
                {
                    var Bytes = await BinarySerializer.SerializeAsync(Response);

                    //Debug(Bytes);

                    await UdpClient.SendAsync(Bytes, Bytes.Length, Remote);

                    Responded?.Invoke(this, new RespondedEventArgs(Response, Remote));
                }
                catch (Exception Error)
                {
                    this.Error?.Invoke(this, new ErrorEventArgs(null, Response, Error));
                    Console.WriteLine(Error.Message);
                }
            }
        }

        private async Task SendError(ushort ID, IPEndPoint RemoteEndPoint)
        {
            try
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
            catch (Exception Error)
            {
                Console.WriteLine(Error.Message);
            }
        }

        private void Debug(byte[] Bytes)
        {
            var Binary = Bytes.ToList().Select(Byte => Convert.ToString(Byte, 2).PadLeft(8, '0')).ToList();
            var Message = string.Join(' ', Binary);
            Console.WriteLine(Message);
        }

        public class RequestedEventArgs : EventArgs
        {
            public readonly IMessage Request;
            public readonly IPEndPoint EndPoint;

            public RequestedEventArgs(IMessage Request, IPEndPoint EndPoint)
            {
                this.Request = Request;
                this.EndPoint = EndPoint;
            }
        }

        public class ResolvedEventArgs : EventArgs
        {
            public readonly IMessage Request;
            public readonly IMessage Response;
            public readonly IPEndPoint EndPoint;

            public ResolvedEventArgs(IMessage Request, IMessage Response, IPEndPoint EndPoint)
            {
                this.Request = Request;
                this.Response = Response;
                this.EndPoint = EndPoint;
            }
        }

        public class RespondedEventArgs : EventArgs
        {
            public readonly IMessage Response;
            public readonly IPEndPoint EndPoint;

            public RespondedEventArgs(IMessage Response, IPEndPoint EndPoint)
            {
                this.Response = Response;
                this.EndPoint = EndPoint;
            }
        }

        public class ErrorEventArgs : EventArgs
        {
            public readonly IMessage Request;
            public readonly IMessage Response;
            public readonly Exception Error;

            public ErrorEventArgs(IMessage Request, IMessage Response, Exception Error)
            {
                this.Request = Request;
                this.Response = Response;
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
