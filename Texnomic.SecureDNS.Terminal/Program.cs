using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Colorful;
using CommandLine;
using Destructurama;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Texnomic.DNS.Servers;
using Texnomic.DNS.Servers.Middlewares;
using Texnomic.DNS.Servers.ResponsibilityChain;
using Texnomic.FilterLists.Enums;
using Texnomic.SecureDNS.Terminal.Properties;

using Console = Colorful.Console;

namespace Texnomic.SecureDNS.Terminal
{
    internal class Program
    {
        private static async Task Main(string[] Args)
        {
            try
            {
                Console.Title = "Texnomic SecureDNS";

                var Speed = new Figlet(FigletFont.Load(Resources.Speed));

                Console.WriteWithGradient(Speed.ToAscii(" Texnomic").ConcreteValue.ToArray(), System.Drawing.Color.Yellow, System.Drawing.Color.Fuchsia, 14);

                Console.WriteWithGradient(Speed.ToAscii(" SecureDNS").ConcreteValue.ToArray(), System.Drawing.Color.Yellow, System.Drawing.Color.Fuchsia, 14);

                Console.WriteLine("");

                if (Args.Length == 0)
                {
                    Console.WriteLine(" > Loading...");

                    Console.WriteLine("");

                    Thread.Sleep(2500);

                    RunGUI();
                }
                else
                {
                    Parser.Default.ParseArguments<Settings>(Args)
                                  .WithParsed(RunCMD);
                }

            }
            catch (Exception Error)
            {
                Console.WriteLine($"Fatal Error: {Error.Message}");
                Console.ReadLine();
            }
        }

        private static void RunCMD(Settings Settings)
        {
            Log.Logger = new LoggerConfiguration()
                            .Destructure.UsingAttributes()
                            .WriteTo.Seq(Settings.SeqUriEndPoint, compact: true)
                            .CreateLogger();

            var FilterTags = new Tags[]
            {
                Tags.Malware,
                Tags.Phishing,
                Tags.Crypto,
            };

            var ServiceCollection = new ServiceCollection();
            ServiceCollection.AddSingleton(Log.Logger);
            ServiceCollection.AddSingleton(FilterTags);
            ServiceCollection.AddSingleton<FilterMiddleware>();
            ServiceCollection.AddSingleton<GoogleHTTPsMiddleware>();

            var ServerMiddlewareActivator = new ServerMiddlewareActivator(ServiceCollection.BuildServiceProvider());

            var Middlewares = new List<Type>()
            {
                typeof(FilterMiddleware),
                typeof(GoogleHTTPsMiddleware),
            };

            var ServerResponsibilityChain = new ProxyResponsibilityChain(Middlewares, ServerMiddlewareActivator);

            var ProxyServer = new ProxyServer(ServerResponsibilityChain, Log.Logger, IPEndPoint.Parse(Settings.ServerIPEndPoint));

            ProxyServer.Started += (Sender, Args) => Console.WriteLine($"Server Started.");

            ProxyServer.Stopped += (Sender, Args) => Console.WriteLine($"Server Stopped.");

            ProxyServer.Errored += (Sender, Args) => Console.WriteLine($"Server Error: {Args.Error.Message}");

            ProxyServer.StartAsync(Settings.CancellationTokenSource.Token).Wait();

            Console.WriteLine("Press Any Key To Exits...");

            Console.ReadKey();

            ProxyServer.StopAsync(Settings.CancellationTokenSource.Token).Wait();
        }

        private static void RunGUI()
        {
            Console.ReplaceAllColorsWithDefaults();

            var App = new App(new Settings());
            App.Run();
        }
    }
}
