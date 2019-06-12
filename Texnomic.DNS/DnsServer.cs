using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using BinarySerialization;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Texnomic.DNS.Models;
using Texnomic.DNS.Resolvers;
using System.Collections.Generic;

namespace Texnomic.DNS
{
    public class DnsServer<TResolver> : IDisposable where TResolver : IResolver, new()
    {
        readonly int Threads;
        readonly List<Task> Workers;
        readonly BlockingCollection<(Message, IPEndPoint)> IncomingCollection, OutcomingCollection;

        public event EventHandler<RequestedEventArgs> Requested;
        public event EventHandler<ResolvedEventArgs> Resolved;
        public event EventHandler<RespondedEventArgs> Responded;
        public event EventHandler<EventArgs> Started;
        public event EventHandler<EventArgs> Stopped;
        public event EventHandler<ErroredEventArgs> Errored;

        public bool IsRunning { get; private set; }

        public DnsServer()
        {
            Threads = Environment.ProcessorCount;

            Workers = new List<Task>();

            IncomingCollection = OutcomingCollection = new BlockingCollection<(Message, IPEndPoint)>();
        }
        public DnsServer(int Threads)
        {
            this.Threads = Threads;

            Workers = new List<Task>();

            IncomingCollection = OutcomingCollection = new BlockingCollection<(Message, IPEndPoint)>();
        }

        public async Task StartAsync()
        {
            if (IsRunning) return;

            IsRunning = true;

            for (int i = 0; i < Threads; i++)
            {
                Workers.Add(ReceiveAsync());
                Workers.Add(ResolveAsync());
                Workers.Add(ForwardAsync());
            }

            Started?.Invoke(this, EventArgs.Empty);

            await Task.WhenAll(Workers);
        }

        public async Task StopAsync()
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

            var Receiver = CreateUdpClient();

            while (IsRunning)
            {
                try
                {
                    var Result = await Receiver.ReceiveAsync();

                    var Request = await Serializer.DeserializeAsync<Message>(Result.Buffer);

                    IncomingCollection.Add((Request, Result.RemoteEndPoint));

                    Requested?.Invoke(this, new RequestedEventArgs(Request, Result.RemoteEndPoint));
                }
                catch (BindingException Error)
                {
                    Errored?.Invoke(this, new ErroredEventArgs(Error));
                    IsRunning = false;
                    break;
                }
                catch (SocketException Error)
                {
                    Errored?.Invoke(this, new ErroredEventArgs(Error));
                    IsRunning = false;
                    break;
                }
                catch (Exception Error)
                {
                    Errored?.Invoke(this, new ErroredEventArgs(Error));
                    IsRunning = false;
                    break;
                }
            }

            Receiver.Dispose();
        }
        private async Task ResolveAsync()
        {
            var Resolver = new TResolver();

            while (IsRunning)
            {
                try
                {
                    (var Request, var Remote) = IncomingCollection.Take();

                    var Response = await Resolver.ResolveAsync(Request);

                    OutcomingCollection.Add((Response, Remote));

                    Resolved?.Invoke(this, new ResolvedEventArgs(Request, Response, Remote));
                }
                catch (BindingException Error)
                {
                    Errored?.Invoke(this, new ErroredEventArgs(Error));
                    IsRunning = false;
                    break;
                }
                catch (SocketException Error)
                {
                    Errored?.Invoke(this, new ErroredEventArgs(Error));
                    IsRunning = false;
                    break;
                }
                catch (Exception Error)
                {
                    Errored?.Invoke(this, new ErroredEventArgs(Error));
                    IsRunning = false;
                    break;
                }
            }
        }
        private async Task ForwardAsync()
        {
            var Serializer = new BinarySerializer();

            var Sender = CreateUdpClient();

            while (IsRunning)
            {
                try
                {
                    (var Response, var Remote) = OutcomingCollection.Take();

                    var Stream = new MemoryStream();

                    await Serializer.SerializeAsync(Stream, Response);

                    var Bytes = Stream.ToArray();

                    await Sender.SendAsync(Bytes, Bytes.Length, Remote);

                    Responded?.Invoke(this, new RespondedEventArgs(Response, Remote));
                }
                catch (BindingException Error)
                {
                    Errored?.Invoke(this, new ErroredEventArgs(Error));
                    IsRunning = false;
                    break;
                }
                catch (SocketException Error)
                {
                    Errored?.Invoke(this, new ErroredEventArgs(Error));
                    IsRunning = false;
                    break;
                }
                catch (Exception Error)
                {
                    Errored?.Invoke(this, new ErroredEventArgs(Error));
                    IsRunning = false;
                    break;
                }
            }

            Sender.Dispose();
        }

        private UdpClient CreateUdpClient()
        {
            var Receiver = new UdpClient(53)
            {
                ExclusiveAddressUse = false
            };

            Receiver.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            return Receiver;
        }

        public void Dispose()
        {
            IncomingCollection.Dispose();
            OutcomingCollection.Dispose();
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

        public class ErroredEventArgs : EventArgs
        {
            public readonly Exception Error;

            public ErroredEventArgs(Exception Error)
            {
                this.Error = Error;
            }
        }
    }
}
