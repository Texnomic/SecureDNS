namespace Texnomic.SecureDNS.Servers.Proxy;

public sealed class TCPServer(
    IOptionsMonitor<ProxyResponsibilityChainOptions> ProxyResponsibilityChainOptions,
    IOptionsMonitor<ProxyServerOptions> ProxyServerOptions,
    IMiddlewareResolver MiddlewareResolver,
    ILogger Logger)
    : IHostedService, IDisposable
{
    private readonly List<Task> Workers = [];
    private readonly BufferBlock<(IMessage, Socket)> IncomingQueue = new();
    private readonly BufferBlock<(IMessage, Socket)> OutgoingQueue = new();
    private readonly TcpListener TcpListener = new(ProxyServerOptions.CurrentValue.IPEndPoint);

    private CancellationToken CancellationToken;

    public async Task StartAsync(CancellationToken Token)
    {
        CancellationToken = Token;

        TcpListener.Start();

        for (var I = 0; I < ProxyServerOptions.CurrentValue.Threads; I++)
        {
            Workers.Add(Task.Factory.StartNew(ReceiveAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
            Workers.Add(Task.Factory.StartNew(ResolveAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
            Workers.Add(Task.Factory.StartNew(SendAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
        }

        Logger?.Information("TCP Server Started with {@Threads} Threads. Listening On {@IPEndPoint}", ProxyServerOptions.CurrentValue.Threads, ProxyServerOptions.CurrentValue.IPEndPoint.ToString());

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
                var ClientSocket = await TcpListener.AcceptSocketAsync(CancellationToken);

                var Prefix = new byte[2];

                await ClientSocket.ReceiveAsync(Prefix, SocketFlags.None);

                var Size = BinaryPrimitives.ReadUInt16BigEndian(Prefix);

                var Buffer = new byte[Size];

                await ClientSocket.ReceiveAsync(Buffer, SocketFlags.None);

                var Query = Deserialize(Buffer);

                await IncomingQueue.SendAsync((Query, ClientSocket), CancellationToken);

                Logger?.Verbose("Received {@Query} From {@RemoteEndPoint}.", Query, ClientSocket.RemoteEndPoint.ToString());
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

        Socket ClientSocket = null;

        while (!CancellationToken.IsCancellationRequested)
        {
            try
            {
                (Query, ClientSocket) = await IncomingQueue.ReceiveAsync(CancellationToken);

                var Answer = await ResponsibilityChain.Execute(Query);

                await OutgoingQueue.SendAsync((Answer, ClientSocket), CancellationToken);
                    
                Logger?.Information("Resolved {@Answer} For {@Domain} with {@ResponseCode} To {@RemoteEndPoint}.", Answer, Answer.Questions.First().Domain.Name, Answer.ResponseCode, ClientSocket.RemoteEndPoint.ToString());
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ObjectDisposedException Error)
            {
                ResponsibilityChain = new ProxyResponsibilityChain(ProxyResponsibilityChainOptions, MiddlewareResolver);

                var ErrorMessage = Handle(Error, Query.ID, "Resolving", ResponseCode.ServerFailure);

                await OutgoingQueue.SendAsync((ErrorMessage, ClientSocket), CancellationToken);
            }
            catch (Exception Error)
            {
                var ErrorMessage = Handle(Error, Query.ID, "Resolving", ResponseCode.ServerFailure);

                await OutgoingQueue.SendAsync((ErrorMessage, ClientSocket), CancellationToken);
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
                var (Answer, ClientSocket) = await OutgoingQueue.ReceiveAsync(CancellationToken);

                var Bytes = Serialize(Answer);

                var Prefix = new byte[2];

                BinaryPrimitives.WriteUInt16BigEndian(Prefix, (ushort)Bytes.Length);

                await ClientSocket.SendAsync(Prefix, SocketFlags.None);

                await ClientSocket.SendAsync(Bytes, SocketFlags.None);

                Logger?.Verbose("Sent {@Answer} To {@RemoteEndPoint}.", Answer, ClientSocket.RemoteEndPoint.ToString());
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

    private Message Handle(Exception Error, ushort ID, string Stage, ResponseCode Response)
    {
        Logger?.Error(Error, $"{@Error} Occurred While {Stage} Message.", Error);

        return new Message()
        {
            ID = ID,
            MessageType = MessageType.Response,
            ResponseCode = Response,
        };
    }

    private Message Handle(Exception Error, byte[] Bytes, string Stage, ResponseCode Response)
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
            TcpListener.Stop();
        }

        IsDisposed = true;
    }

    ~TCPServer()
    {
        Dispose(false);
    }

}