using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Colorful;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PipelineNet.ChainsOfResponsibility;
using PipelineNet.MiddlewareResolver;
using Serilog;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Options;
using Texnomic.DNS.Protocols;
using Texnomic.DNS.Servers;
using Texnomic.DNS.Servers.Middlewares;
using Texnomic.DNS.Servers.Options;
using Texnomic.DNS.Servers.ResponsibilityChain;
using Texnomic.SecureDNS.Terminal.Enums;
using Texnomic.SecureDNS.Terminal.Options;
using Texnomic.SecureDNS.Terminal.Properties;

using Console = Colorful.Console;

namespace Texnomic.SecureDNS.Terminal
{
    internal class Program
    {
        private static IHostBuilder HostBuilder;

        private static readonly string Stage = Environment.GetEnvironmentVariable("SecureDNS_ENVIRONMENT") ?? "Production";

        private static readonly IConfiguration Configurations = new ConfigurationBuilder()
                                                                    .SetBasePath(Directory.GetCurrentDirectory())
                                                                    .AddJsonFile("AppSettings.json", false, true)
                                                                    .AddJsonFile($"AppSettings.{Stage}.json", true, true)
                                                                    .AddUserSecrets<Program>(true, true)
                                                                    .AddEnvironmentVariables()
                                                                    .Build();

        public static async Task Main(string[] Arguments)
        {
            Splash();

            BuildHost();

            await HostBuilder.RunConsoleAsync();
        }

        private static void BuildHost()
        {
            HostBuilder = new HostBuilder()
                 .ConfigureAppConfiguration(ConfigureApp)
                 .ConfigureServices(ConfigureServices)
                 .ConfigureLogging(ConfigureLogging)
                 .UseSerilog(ConfigureLogger, writeToProviders: true);

            var Options = Configurations.GetSection("Terminal Options").Get<TerminalOptions>();

            if (Options.Mode == Mode.Daemon)
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
        }

        private static void ConfigureApp(HostBuilderContext HostBuilderContext, IConfigurationBuilder Configuration)
        {
            Configuration.AddConfiguration(Configurations);
        }
        private static void ConfigureLogging(HostBuilderContext HostBuilderContext, ILoggingBuilder Logging)
        {
            Logging.AddConsole();
        }
        private static void ConfigureLogger(HostBuilderContext HostBuilderContext, LoggerConfiguration LoggerConfiguration)
        {
            LoggerConfiguration.ReadFrom.Configuration(Configurations);
        }
        private static void ConfigureServices(HostBuilderContext HostBuilderContext, IServiceCollection Services)
        {
            Services.Configure<ProxyResponsibilityChainOptions>(Configurations.GetSection("Proxy Responsibility Chain"));
            Services.Configure<HostTableMiddlewareOptions>(Configurations.GetSection("HostTable Middleware"));
            Services.Configure<FilterListsMiddlewareOptions>(Configurations.GetSection("FilterLists Middleware"));
            Services.Configure<ProxyServerOptions>(Configurations.GetSection("Proxy Server"));
            Services.Configure<HTTPsOptions>(Configurations.GetSection("HTTPs Protocol"));
            Services.Configure<TLSOptions>(Configurations.GetSection("TLS Protocol"));
            Services.Configure<TerminalOptions>(Configurations.GetSection("Terminal Options"));

            Services.AddSingleton<MemoryCache>();
            Services.AddSingleton<HostTableMiddleware>();
            Services.AddSingleton<FilterListsMiddleware>();
            Services.AddSingleton<ResolverMiddleware>();
            Services.AddSingleton<IMiddlewareResolver, ServerMiddlewareActivator>();
            Services.AddSingleton<IAsyncResponsibilityChain<IMessage, IMessage>, ProxyResponsibilityChain>();

            var Options = Configurations.GetSection("Terminal Options").Get<TerminalOptions>();

            if(Options.Protocol == Protocol.HTTPs)
            {
                Services.AddSingleton<IProtocol, HTTPs>();
            }

            if (Options.Protocol == Protocol.TLS)
            {
                Services.AddSingleton<IProtocol, TLS>();
            }

            if (Options.Mode == Mode.GUI)
            {
                Services.AddSingleton<ProxyServer>();
                Services.AddHostedService<GUI>();
            }

            if (Options.Mode == Mode.CLI)
            {
                Services.AddSingleton<ProxyServer>();
                Services.AddHostedService<CLI>();
            }

            if (Options.Mode == Mode.Daemon)
            {
                Services.AddHostedService<ProxyServer>();
            }
        }

    }
}
