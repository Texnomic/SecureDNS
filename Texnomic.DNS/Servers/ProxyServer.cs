using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization;
using Microsoft.Extensions.Hosting;
using PipelineNet.ChainsOfResponsibility;
using PipelineNet.Pipelines;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Servers
{
    public class ProxyServer : IHostedService, IDisposable
    {
        private readonly int Threads;
        private readonly List<Task> Workers;
        private readonly IAsyncResponsibilityChain<Message, Message> ResponsibilityChain;
        private readonly BlockingCollection<(Message, IPEndPoint)> IncomingCollection, OutgoingCollection;

        public event EventHandler<RequestedEventArgs> Requested;
        public event EventHandler<ResolvedEventArgs> Resolved;
        public event EventHandler<RespondedEventArgs> Responded;
        public event EventHandler<EventArgs> Started;
        public event EventHandler<EventArgs> Stopped;
        public event EventHandler<ErrorEventArgs> Error;

        public bool IsRunning { get; private set; }

        private readonly UdpClient UdpClient;

        public ProxyServer(IAsyncResponsibilityChain<Message, Message> ResponsibilityChain, int Threads = 0)
        {
            this.Threads = Threads == 0 ? Environment.ProcessorCount : Threads;

            this.ResponsibilityChain = ResponsibilityChain;

            Workers = new List<Task>();

            UdpClient = new UdpClient(53);

            IncomingCollection = OutgoingCollection = new BlockingCollection<(Message, IPEndPoint)>();
        }

        public async Task StartAsync(CancellationToken CancellationToken)
        {
            if (IsRunning) return;

            IsRunning = true;

            for (var I = 0; I < Threads; I++)
            {
                Workers.Add(ReceiveAsync());
                Workers.Add(ResolveAsync());
                Workers.Add(ForwardAsync());
            }

            Started?.Invoke(this, EventArgs.Empty);

            await Task.WhenAll(Workers);
        }

        public async Task StopAsync(CancellationToken CancellationToken)
        {
            if (!IsRunning) return;

            IsRunning = false;

            await Task.WhenAll(Workers);

            Dispose();

            Stopped?.Invoke(this, EventArgs.Empty);
        }

        private async Task ReceiveAsync()
        {
            var Serializer = new BinarySerializer();

            while (IsRunning)
            {
                try
                {
                    var Result = await UdpClient.ReceiveAsync();

                    var Request = await Serializer.DeserializeAsync<Message>(Result.Buffer);

                    IncomingCollection.Add((Request, Result.RemoteEndPoint));

                    Requested?.Invoke(this, new RequestedEventArgs(Request, Result.RemoteEndPoint));
                }
                catch (BindingException Error)
                {
                    this.Error?.Invoke(this, new ErrorEventArgs(Error));
                    IsRunning = false;
                    break;
                }
                catch (SocketException Error)
                {
                    this.Error?.Invoke(this, new ErrorEventArgs(Error));
                    IsRunning = false;
                    break;
                }
                catch (Exception Error)
                {
                    this.Error?.Invoke(this, new ErrorEventArgs(Error));
                    IsRunning = false;
                    break;
                }
            }

            UdpClient.Dispose();
        }
        private async Task ResolveAsync()
        {
            while (IsRunning)
            {
                try
                {
                    var (Request, Remote) = IncomingCollection.Take();

                    var Response = await ResponsibilityChain.Execute(Request);

                    OutgoingCollection.Add((Response, Remote));

                    Resolved?.Invoke(this, new ResolvedEventArgs(Request, Response, Remote));
                }
                catch (BindingException Error)
                {
                    this.Error?.Invoke(this, new ErrorEventArgs(Error));
                    IsRunning = false;
                    break;
                }
                catch (SocketException Error)
                {
                    this.Error?.Invoke(this, new ErrorEventArgs(Error));
                    IsRunning = false;
                    break;
                }
                catch (Exception Error)
                {
                    this.Error?.Invoke(this, new ErrorEventArgs(Error));
                    IsRunning = false;
                    break;
                }
            }
        }
        private async Task ForwardAsync()
        {
            var Serializer = new BinarySerializer();

            while (IsRunning)
            {
                try
                {
                    var (Response, Remote) = OutgoingCollection.Take();

                    var Stream = new MemoryStream();

                    await Serializer.SerializeAsync(Stream, Response);

                    var Bytes = Stream.ToArray();

                    await UdpClient.SendAsync(Bytes, Bytes.Length, Remote);

                    Responded?.Invoke(this, new RespondedEventArgs(Response, Remote));
                }
                catch (BindingException Error)
                {
                    this.Error?.Invoke(this, new ErrorEventArgs(Error));
                    IsRunning = false;
                    break;
                }
                catch (SocketException Error)
                {
                    this.Error?.Invoke(this, new ErrorEventArgs(Error));
                    IsRunning = false;
                    break;
                }
                catch (Exception Error)
                {
                    this.Error?.Invoke(this, new ErrorEventArgs(Error));
                    IsRunning = false;
                    break;
                }
            }

            UdpClient.Dispose();
        }

        public void Dispose()
        {
            UdpClient.Dispose();
            IncomingCollection.Dispose();
            OutgoingCollection.Dispose();
        }

        public class RequestedEventArgs : EventArgs
        {
            public readonly Message Request;
            public readonly IPEndPoint EndPoint;

            public RequestedEventArgs(Message Request, IPEndPoint EndPoint)
            {
                this.Request = Request;
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
    }
}
