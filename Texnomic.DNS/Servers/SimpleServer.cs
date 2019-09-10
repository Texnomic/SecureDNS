using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PipelineNet.ChainsOfResponsibility;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Servers
{
    public class SimpleServer : IHostedService, IDisposable
    {
        private readonly List<Task> Workers;
        private readonly IAsyncResponsibilityChain<Message, Message> ResponsibilityChain;

        private readonly UdpClient UdpClient;

        public SimpleServer(IAsyncResponsibilityChain<Message, Message> ResponsibilityChain)
        {
            this.ResponsibilityChain = ResponsibilityChain;
            Workers = new List<Task>();
            UdpClient = new UdpClient(53);
        }

        public async Task StartAsync(CancellationToken CancellationToken)
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var UdpReceiveResult = await UdpClient.ReceiveAsync();
                    //Workers.Add(ResolveAsync(UdpReceiveResult));
                    ResolveAsync(UdpReceiveResult);
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
                var Request = Message.FromArray(UdpReceiveResult.Buffer);
                ID = Request.ID;
                var Response = await ResponsibilityChain.Execute(Request);
                var ResponseBytes = Response.ToArray();
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

                //Console.WriteLine(Error);
            }
        }

        public async Task StopAsync(CancellationToken CancellationToken)
        {

        }

        public void Dispose()
        {
            UdpClient.Dispose();
        }

    }
}
