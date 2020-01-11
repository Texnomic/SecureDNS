using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Colorful;
using CommandLine;
using Destructurama;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PipelineNet.ChainsOfResponsibility;
using PipelineNet.Middleware;
using PipelineNet.MiddlewareResolver;
using Serilog;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Models;
using Texnomic.DNS.Servers;
using Texnomic.DNS.Servers.Middlewares;
using Texnomic.DNS.Servers.Options;
using Texnomic.DNS.Servers.ResponsibilityChain;
using Texnomic.FilterLists.Enums;
using Texnomic.SecureDNS.Terminal.Enums;
using Texnomic.SecureDNS.Terminal.Options;
using Texnomic.SecureDNS.Terminal.Properties;

using Console = Colorful.Console;

namespace Texnomic.SecureDNS.Terminal
{
    internal class Program
    {
        private static IHostBuilder HostBuilder;

        private static TerminalOptions Options;

        public static async Task Main(string[] Arguments)
        {
            Splash();

            Parser.Default.ParseArguments<TerminalOptions>(Arguments)
                          .WithParsed(StartWithOptions)
                          .WithNotParsed(StartWithoutOptions);

            BuildHost();

            await HostBuilder.RunConsoleAsync();
        }

        private static void StartWithOptions(TerminalOptions TerminalOptions)
        {
            Options = TerminalOptions;
        }

        private static void StartWithoutOptions(IEnumerable<Error> Errors)
        {
            Options = new TerminalOptions();
        }

        private static void BuildHost()
        {
            HostBuilder = new HostBuilder()
                 .ConfigureAppConfiguration(ConfigureApp)
                 .ConfigureServices(ConfigureServices)
                 .ConfigureLogging(ConfigureLogging);

            if (Options.Mode == OperatingMode.Daemon)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    HostBuilder = HostBuilder.UseWindowsService();
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    HostBuilder = HostBuilder.UseSystemd();
                }
            }
        }

        private static void Splash()
        {
            Console.Title = "Texnomic SecureDNS";

            var Speed = new Figlet(FigletFont.Load(Resources.Speed));

            Console.WriteWithGradient(Speed.ToAscii(" Texnomic").ConcreteValue.ToArray(), System.Drawing.Color.Yellow, System.Drawing.Color.Fuchsia, 14);

            Console.WriteWithGradient(Speed.ToAscii(" SecureDNS").ConcreteValue.ToArray(), System.Drawing.Color.Yellow, System.Drawing.Color.Fuchsia, 14);

            Console.WriteLine("");

            Console.WriteLine(" > Loading...");

            Console.WriteLine("");

            Thread.Sleep(1500);
        }

        private static void ConfigureApp(HostBuilderContext HostBuilderContext, IConfigurationBuilder Configuration)
        {
            Configuration.AddEnvironmentVariables();
        }
        private static void ConfigureLogging(HostBuilderContext HostBuilderContext, ILoggingBuilder Logging)
        {
            Logging.AddConsole();
        }
        private static void ConfigureServices(HostBuilderContext HostBuilderContext, IServiceCollection Services)
        {
            Services.AddOptions();
            Services.AddOptions<FilterMiddlewareOptions>();
            Services.AddOptions<ProxyResponsibilityChainOptions>();

            Services.AddSingleton(Log.Logger);
            Services.AddSingleton(Options);
            Services.AddSingleton(new ProxyServerOptions() { IPEndPoint = IPEndPoint.Parse(Options.ServerIPEndPoint) });

            Services.AddSingleton<IAsyncMiddleware<IMessage, IMessage>, FilterMiddleware>();
            Services.AddSingleton<IAsyncMiddleware<IMessage, IMessage>, GoogleHTTPsMiddleware>();
            Services.AddSingleton<IMiddlewareResolver, ServerMiddlewareActivator>();
            Services.AddSingleton<IAsyncResponsibilityChain<IMessage, IMessage>, ProxyResponsibilityChain>();

            if (Options.Mode == OperatingMode.TerminalGUI)
            {
                Services.AddSingleton<ProxyServer>();
                Services.AddSingleton<IHostedService, TerminalGUI>();
            }

            if (Options.Mode == OperatingMode.TerminalCMD)
            {
                Services.AddSingleton<ProxyServer>();
                Services.AddSingleton<IHostedService, TerminalCMD>();
            }

            if (Options.Mode == OperatingMode.Daemon)
            {
                Services.AddSingleton<IHostedService, ProxyServer>();
            }
        }

    }
}
