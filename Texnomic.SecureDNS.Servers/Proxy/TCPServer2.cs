using System.Buffers.Binary;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PipelineNet.MiddlewareResolver;

using Serilog;
using Texnomic.SecureDNS.Abstractions.Enums;
using Texnomic.SecureDNS.Core;
using Texnomic.SecureDNS.Extensions;
using Texnomic.SecureDNS.Serialization;
using Texnomic.SecureDNS.Servers.Proxy.Options;
using Texnomic.SecureDNS.Servers.Proxy.ResponsibilityChain;


namespace Texnomic.SecureDNS.Servers.Proxy;

public sealed class TCPServer2 : IHostedService, IDisposable
{
    private readonly ILogger Logger;
    private readonly List<Task> Threads;
    private readonly IOptionsMonitor<ProxyServerOptions> Options;
    private readonly IMiddlewareResolver MiddlewareResolver;
    private readonly IOptionsMonitor<ProxyResponsibilityChainOptions> ProxyResponsibilityChainOptions;
    private readonly TcpListener TcpListener;

    private CancellationToken CancellationToken;


    public TCPServer2(IOptionsMonitor<ProxyResponsibilityChainOptions> ProxyResponsibilityChainOptions,
        IOptionsMonitor<ProxyServerOptions> ProxyServerOptions,
        IMiddlewareResolver MiddlewareResolver,
        ILogger Logger)
    {
        Options = ProxyServerOptions;

        this.MiddlewareResolver = MiddlewareResolver;

        this.ProxyResponsibilityChainOptions = ProxyResponsibilityChainOptions;

        this.Logger = Logger;

        Threads = new List<Task>();

        TcpListener = new TcpListener(Options.CurrentValue.IPEndPoint);
    }

    public async Task StartAsync(CancellationToken Token)
    {
        CancellationToken = Token;

        //TcpListener.Server.Listen();

        TcpListener.Start();

        for (var I = 0; I < ProxyServerOptions.Threads; I++)
        {
            Threads.Add(Task.Factory.StartNew(ResolveAsync, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
        }

        Logger?.Information("TCP Server Started with {@Threads} Threads. Listening On {@IPEndPoint}", ProxyServerOptions.Threads, Options.CurrentValue.IPEndPoint.ToString());

        await Task.Yield();
    }


    public async Task StopAsync(CancellationToken Token)
    {
        await Task.WhenAll(Threads);

        Logger?.Information("TCP Server Stopped.");
    }

    private async Task ResolveAsync()
    {
        Logger?.Debug("TCP Server Started Thread {@Thread}.", Environment.CurrentManagedThreadId);

        var ProxyResponsibilityChain = new ProxyResponsibilityChain(ProxyResponsibilityChainOptions, MiddlewareResolver);

        while (!CancellationToken.IsCancellationRequested)
        {
            Socket Client = default;

            var Prefix = new byte[2];

            var Buffer = Array.Empty<byte>();

            try
            {
                Client = await TcpListener.AcceptSocketAsync().WithCancellation(CancellationToken);

                Prefix = new byte[2];

                await Client.ReceiveAsync(Prefix, SocketFlags.None);
                    
                var Size = BinaryPrimitives.ReadUInt16BigEndian(Prefix);

                Buffer = new byte[Size];

                await Client.ReceiveAsync(Buffer, SocketFlags.None);

                var Query = DnSerializer.Deserialize(Buffer);

                var Answer = await ProxyResponsibilityChain.Execute(Query);

                var RawAnswer = DnSerializer.Serialize(Answer);

                BinaryPrimitives.WriteUInt16BigEndian(Prefix, (ushort)RawAnswer.Length);

                await Client.SendAsync(Prefix, SocketFlags.None);

                await Client.SendAsync(RawAnswer, SocketFlags.None);

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
                    ID = BinaryPrimitives.ReadUInt16BigEndian(Buffer[2..2]),
                    MessageType = MessageType.Response,
                    ResponseCode = ResponseCode.ServerFailure,
                };

                var RawMessage = DnSerializer.Serialize(Message);

                BinaryPrimitives.WriteUInt16BigEndian(Prefix, (ushort) RawMessage.Length);

                await Client.SendAsync(Prefix, SocketFlags.None);

                await Client.SendAsync(RawMessage, SocketFlags.None);
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
            TcpListener.Stop();
        }

        IsDisposed = true;
    }

    ~TCPServer2()
    {
        Dispose(false);
    }

}