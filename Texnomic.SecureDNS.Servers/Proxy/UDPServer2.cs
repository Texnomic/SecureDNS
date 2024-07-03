namespace Texnomic.SecureDNS.Servers.Proxy;

public sealed class UDPServer2(
    IOptionsMonitor<ProxyResponsibilityChainOptions> ProxyResponsibilityChainOptions,
    IOptionsMonitor<ProxyServerOptions> ProxyServerOptions,
    IMiddlewareResolver MiddlewareResolver,
    ILogger Logger)
    : IHostedService, IDisposable
{
    private readonly List<Task> Threads = [];
    private readonly UdpClient Server = new();

    private CancellationToken CancellationToken;

    private int Affinity = 0;

    public async Task StartAsync(CancellationToken Token)
    {
        CancellationToken = Token;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            //https://stackoverflow.com/questions/5199026/c-sharp-async-udp-listener-socketexception
            Server.Client.IOControl(-1744830452, new byte[4], null);
        }

        Server.Client.Bind(ProxyServerOptions.CurrentValue.IPEndPoint);

        for (var I = 0; I < ProxyServerOptions.CurrentValue.Threads; I++)
        {
            Threads.Add(Task.Factory.StartNew(ResolveAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
        }

        Logger?.Information("UDP Server Started with {@Threads} Threads. Listening On {@IPEndPoint}", ProxyServerOptions.CurrentValue.Threads, ProxyServerOptions.CurrentValue.IPEndPoint.ToString());

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
            
        var ProcessThread = Process.GetCurrentProcess().Threads[Environment.CurrentManagedThreadId];

        ProcessThread.ProcessorAffinity = (IntPtr) AffinityPointer;

        ProcessThread.PriorityLevel = ThreadPriorityLevel.Highest;

        Logger?.Debug("UDP Server Started Thread {@Thread} with Affinity {@Affinity} & Priority {@Priority}.", Environment.CurrentManagedThreadId, AffinityPointer, ThreadPriorityLevel.Highest);
            
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