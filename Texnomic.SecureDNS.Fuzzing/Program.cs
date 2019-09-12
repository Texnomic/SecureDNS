using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PipelineNet.MiddlewareResolver;
using Texnomic.DNS.Servers;
using Texnomic.DNS.Servers.ResponsibilityChain;

namespace Texnomic.SecureDNS.Fuzzing
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var ActivatorMiddlewareResolver = new ActivatorMiddlewareResolver();
                var ServerResponsibilityChain = new ServerResponsibilityChain(ActivatorMiddlewareResolver);
                var Server = new ProxyServer(ServerResponsibilityChain);
                Server.Started += Server_Started;
                Server.Requested += Server_Requested;
                Server.Resolved += Server_Resolved;
                Server.Responded += Server_Responded;
                Server.Error += Server_Error;
                Server.Stopped += Server_Stopped;
                await Server.StartAsync(new CancellationToken());

                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.StackTrace);
                Console.ReadLine();
            }
        }

        private static void Server_Stopped(object Sender, EventArgs e)
        {
            Console.WriteLine("[Server] Stopped");
        }

        private static void Server_Started(object Sender, EventArgs e)
        {
            Console.WriteLine("[Server] Started");
        }

        private static void Server_Error(object Sender, ProxyServer.ErrorEventArgs e)
        {
            Console.WriteLine($"[Error] EndPoint: {e.Error.Message}");
        }

        private static void Server_Responded(object Sender, ProxyServer.RespondedEventArgs e)
        {
            Console.WriteLine($"[Responded] EndPoint: {e.EndPoint} | Record: {e.Response.Questions.First().Type.ToString().PadLeft(5)} | Domain: {e.Response.Questions.First().Domain.Name}");
        }

        private static void Server_Resolved(object Sender, ProxyServer.ResolvedEventArgs e)
        {
            Console.WriteLine($"[Resolved] EndPoint: {e.EndPoint} | Record: {e.Request.Questions.First().Type.ToString().PadLeft(5)} | Domain: {e.Request.Questions.First().Domain.Name}");
        }

        private static void Server_Requested(object Sender, ProxyServer.RequestedEventArgs e)
        {
            Console.WriteLine($"[Requested] EndPoint: {e.EndPoint}");
        }
    }
}
