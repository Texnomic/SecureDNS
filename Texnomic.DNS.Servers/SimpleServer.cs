using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization;
using Microsoft.Extensions.Hosting;
using PipelineNet.ChainsOfResponsibility;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Servers
{
    public class SimpleServer : IHostedService, IDisposable
    {
        private readonly IAsyncResponsibilityChain<IMessage, IMessage> ResponsibilityChain;
        private readonly BinarySerializer BinarySerializer;
        private readonly UdpClient UdpClient;

        public SimpleServer(IAsyncResponsibilityChain<IMessage, IMessage> ResponsibilityChain)
        {
            this.ResponsibilityChain = ResponsibilityChain;
            UdpClient = new UdpClient(53);
            BinarySerializer = new BinarySerializer();
        }

        public async Task StartAsync(CancellationToken CancellationToken)
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var UdpReceiveResult = await UdpClient.ReceiveAsync();
                    await ResolveAsync(UdpReceiveResult);
                }
                catch (Exception Error)
                {
                    Console.WriteLine(Error);
                }
            }
        }

        public async Task ResolveAsync(UdpReceiveResult UdpReceiveResult)
        {
            ushort ID = 0;

            try
            {
                var Request = await BinarySerializer.DeserializeAsync<Message>(UdpReceiveResult.Buffer);
                ID = Request.ID;
                var Response = await ResponsibilityChain.Execute(Request);
                var ResponseBytes = await BinarySerializer.SerializeAsync(Response);
                await UdpClient.SendAsync(ResponseBytes, ResponseBytes.Length, UdpReceiveResult.RemoteEndPoint);
            }
            catch (Exception Error)
            {
                var Response = new Message()
                {
                    ID = ID,
                    MessageType = MessageType.Response,
                    ResponseCode = ResponseCode.FormatError,
                };

                var ResponseBytes = Response.ToArray();

                await UdpClient.SendAsync(ResponseBytes, ResponseBytes.Length, UdpReceiveResult.RemoteEndPoint);

                Console.WriteLine(Error);
            }
        }

        public async Task StopAsync(CancellationToken CancellationToken)
        {

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

        ~SimpleServer()
        {
            Dispose(false);
        }
    }
}
