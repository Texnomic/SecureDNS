using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Colorful;
using Common.Logging;
using Common.Logging.Serilog;
using Destructurama;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PipelineNet.ChainsOfResponsibility;
using PipelineNet.MiddlewareResolver;
using Serilog;
using Tmds.Systemd;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Middlewares;
using Texnomic.SecureDNS.Middlewares.Options;
using Texnomic.SecureDNS.Protocols;
using Texnomic.SecureDNS.Protocols.Options;
using Texnomic.SecureDNS.Servers.Proxy;
using Texnomic.SecureDNS.Servers.Proxy.ResponsibilityChain;
using Texnomic.SecureDNS.Terminal.Enums;
using Texnomic.SecureDNS.Terminal.Options;

using Console = Colorful.Console;
using Protocol = Texnomic.SecureDNS.Terminal.Enums.Protocol;

namespace Texnomic.SecureDNS.Terminal
{
    internal class Program
    {
        private static IHostBuilder HostBuilder;

        private static string Stage;

        private static IConfigurationRoot Configurations;

        private static TerminalOptions Options;


        public static async Task Main(string[] Arguments)
        {
            Splash();

            Stage = Environment.GetEnvironmentVariable("SecureDNS_ENVIRONMENT") ?? "Production";

            Configurations = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json", false, true)
                .AddJsonFile($"AppSettings.{Stage}.json", true, true)
                .AddUserSecrets<Program>(true, true)
                .AddEnvironmentVariables()
                .Build();


            Options = Configurations.GetSection("Terminal Options").Get<TerminalOptions>();

            BuildHost();

            await HostBuilder.RunConsoleAsync();
        }

        private static byte[] ReadResource(string Name)
        {
            var MainAssembly = Assembly.GetExecutingAssembly();

            var ResourceName = MainAssembly.GetManifestResourceNames()
                .Single(Resource => Resource.EndsWith(Name));

            using var Stream = MainAssembly.GetManifestResourceStream(ResourceName);

            var Buffer = new byte[Stream.Length];

            Stream.Read(Buffer);

            return Buffer;
        }

        private static void BuildHost()
        {
            if (!File.Exists("AppSettings.json"))
                File.WriteAllBytes("AppSettings.json", ReadResource("AppSettings.json"));

            HostBuilder = new HostBuilder()
                .ConfigureAppConfiguration(ConfigureApp)
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(ConfigureLogging)
                .UseSerilog(ConfigureLogger, writeToProviders: true);

            var Options = Configurations.GetSection("Terminal Options").Get<TerminalOptions>();

            if (Options.Mode != Mode.Daemon) return;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                HostBuilder = HostBuilder.UseWindowsService();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                HostBuilder = HostBuilder.UseSystemd();
            }
        }

        private static void Splash()
        {
            Console.Title = "Texnomic SecureDNS";

            var Speed = new Figlet(FigletFont.Load(ReadResource("Speed.flf")));

            Console.WriteWithGradient(Speed.ToAscii(" Texnomic").ConcreteValue.ToArray(), System.Drawing.Color.Yellow,
                System.Drawing.Color.Fuchsia, 14);

            Console.WriteWithGradient(Speed.ToAscii(" SecureDNS").ConcreteValue.ToArray(), System.Drawing.Color.Yellow,
                System.Drawing.Color.Fuchsia, 14);

            Console.WriteLine("");
        }

        private static void ConfigureApp(HostBuilderContext HostBuilderContext, IConfigurationBuilder Configuration)
        {
            Configuration.AddConfiguration(Configurations);
        }

        private static void ConfigureLogging(HostBuilderContext HostBuilderContext, ILoggingBuilder Logging)
        {
            if (Options.Mode == Mode.Daemon)
            {
                Logging.AddJournal(JournalOptions =>
                {
                    JournalOptions.DropWhenBusy = true;
                    JournalOptions.SyslogIdentifier = "SecureDNS";
                });
            }
        }

        private static void ConfigureLogger(HostBuilderContext HostBuilderContext, LoggerConfiguration LoggerConfiguration)
        {
            LoggerConfiguration.ReadFrom.Configuration(Configurations);
            LoggerConfiguration.Destructure.UsingAttributes();
            LoggerConfiguration.Enrich.WithThreadId();
        }

        private static void ConfigureServices(HostBuilderContext HostBuilderContext, IServiceCollection Services)
        {
            Services.Configure<ProxyResponsibilityChainOptions>(Configurations.GetSection("Proxy Responsibility Chain"));
            Services.Configure<HostTableMiddlewareOptions>(Configurations.GetSection("HostTable Middleware"));
            Services.Configure<ResolverMiddlewareOptions>(Configurations.GetSection("Resolver Middleware"));
            Services.Configure<FilterListsMiddlewareOptions>(Configurations.GetSection("FilterLists Middleware"));
            Services.Configure<ProxyServerOptions>(Configurations.GetSection("Proxy Server"));
            Services.Configure<DNSCryptOptions>(Configurations.GetSection("DNSCrypt Protocol"));
            Services.Configure<ENSOptions>(Configurations.GetSection("ENS Protocol"));
            Services.Configure<HTTPsOptions>(Configurations.GetSection("HTTPs Protocol"));
            Services.Configure<TLSOptions>(Configurations.GetSection("TLS Protocol"));
            Services.Configure<TCPOptions>(Configurations.GetSection("TCP Protocol"));
            Services.Configure<UDPOptions>(Configurations.GetSection("UDP Protocol"));
            Services.Configure<TerminalOptions>(Configurations.GetSection("Terminal Options"));

            Services.AddSingleton<MemoryCache>();
            Services.AddSingleton<HostTableMiddleware>();
            Services.AddSingleton<FilterListsMiddleware>();
            Services.AddScoped<ENSMiddleware>();
            Services.AddScoped<ResolverMiddleware>();
            Services.AddScoped<ILog, SerilogCommonLogger>();
            Services.AddScoped<IMiddlewareResolver, ServerMiddlewareActivator>();
            Services.AddScoped<IAsyncResponsibilityChain<IMessage, IMessage>, ProxyResponsibilityChain>();

            switch (Options.Protocol)
            {
                case Protocol.DNSCrypt:
                    Services.AddScoped<IProtocol, DNSCrypt>();
                    break;
                case Protocol.HTTPs:
                    Services.AddScoped<IProtocol, HTTPs>();
                    break;
                case Protocol.TLS:
                    Services.AddScoped<IProtocol, TLS>();
                    break;
                case Protocol.TCP:
                    Services.AddScoped<IProtocol, TCP>();
                    break;
                case Protocol.UDP:
                    Services.AddScoped<IProtocol, UDP>();
                    break;
                default:
                    Services.AddScoped<IProtocol, DNSCrypt>();
                    break;
            }

            switch (Options.Mode)
            {
                case Mode.GUI:
                    Services.AddScoped<UDPServer>();
                    Services.AddHostedService<GUI>();
                    break;
                case Mode.CLI:
                    Services.AddScoped<UDPServer>();
                    Services.AddHostedService<CLI>();
                    break;
                case Mode.Daemon:
                    Daemonize();
                    Services.AddHostedService<UDPServer>();
                    break;
                default:
                    Services.AddScoped<UDPServer>();
                    Services.AddHostedService<GUI>();
                    break;
            }
        }

        private static void Daemonize()
        {
            if (ServiceManager.IsRunningAsService) return;

            CreateUnitFile();

            ExecuteShell("systemctl daemon-reload");

            ExecuteShell("systemctl start securedns.service");

            Environment.Exit(0);
        }

        private static void CreateUnitFile()
        {
            const string UnitFile = "/etc/systemd/system/securedns.service";

            if (File.Exists(UnitFile)) return;

            var Lines = new[]
            {
                "[Unit]",
                "[Service]",
                $"WorkingDirectory={Environment.CurrentDirectory}",
                $"ExecStart={Assembly.GetExecutingAssembly().Location}",
                "[Install]",
                "WantedBy=multi-user.target"
            };

            File.WriteAllLines(UnitFile, Lines);
        }

        private static string ExecuteShell(string Command)
        {
            var Shell = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{Command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            Shell.Start();

            var Result = Shell.StandardOutput.ReadToEnd();

            Shell.WaitForExit();

            if(Shell.ExitCode != 0)
                throw new ApplicationException($"Shell Command: \"{Command}\" Execution Failed.");

            return Result;

        }
    }
}
