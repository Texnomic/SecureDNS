using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using Terminal.Gui;
using System.Threading;
using System.Threading.Tasks;
using Colorful;
using CommandLine;
using PipelineNet.MiddlewareResolver;
using Serilog;
using Texnomic.DNS.Servers;
using Texnomic.DNS.Servers.ResponsibilityChain;
using Texnomic.SecureDNS.CLI.Properties;
using Attribute = Terminal.Gui.Attribute;
using Console = Colorful.Console;

namespace Texnomic.SecureDNS.CLI
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
                .WriteTo.Seq(Settings.SeqUriEndPoint.ToString(), compact: true)
                .CreateLogger();

            var ActivatorMiddlewareResolver = new ActivatorMiddlewareResolver();

            var ServerResponsibilityChain = new ProxyResponsibilityChain(ActivatorMiddlewareResolver);

            var ProxyServer = new ProxyServer(ServerResponsibilityChain, Log.Logger, IPEndPoint.Parse(Settings.ServerIPEndPoint));

            ProxyServer.Started += (Sender, Args) => Console.WriteLine($"Server Started.");

            ProxyServer.Stopped += (Sender, Args) => Console.WriteLine($"Server Stopped.");

            ProxyServer.Errored += (Sender, Args) => Console.WriteLine($"Server Error: {Args.Error.Message}");

            ProxyServer.StartAsync(Settings.CancellationToken).Wait();
        }

        private static void RunGUI()
        {
            Console.ReplaceAllColorsWithDefaults();

            var App = new App(new Settings());
            App.Run();
        }
    }
}
