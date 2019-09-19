using System;
using Serilog;
using System.IO;
using ElectronNET.API;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog.Extensions.Logging;
using Texnomic.SecureDNS.Configurations;
using Texnomic.SecureDNS.Extensions;

namespace Texnomic.SecureDNS
{
    public class Program
    {
        private static string[] Arguments;
        private static IConfiguration Configurations => GetConfiguration();
        private static readonly LoggerProviderCollection LoggerProviders = new LoggerProviderCollection();

        public static void Main(string[] Args)
        {
            Arguments = Args;

            Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(Configurations)
                        .WriteTo.Providers(LoggerProviders)
                        .CreateLogger();

            try
            {
                Log.Information("Getting the motors running...");

                CreateHostBuilder().Build().Run();
            }
            catch (Exception Error)
            {
                Log.Fatal(Error, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder(Arguments)
                       .ConfigureAppConfiguration(SetAppConfiguration)
                       .ConfigureWebHostDefaults(SetWebHostDefaults);
        }

        private static void SetWebHostDefaults(IWebHostBuilder WebHostBuilder)
        {
            WebHostBuilder.UseElectron(Arguments)
                          .UseStartup<Startup>()
                          .UseConfiguration(Configurations)
                          .UseSerilog(providers: LoggerProviders);
        }

        private static void SetAppConfiguration(HostBuilderContext HostBuilderContext, IConfigurationBuilder ConfigurationBuilder)
        {
            var Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            ConfigurationBuilder.AddDatabaseConfigurations(OptionsBuilder =>
                OptionsBuilder.UseSqlite($"Data Source={Directory}\\SecureDNS.sqlite;"));
        }

        public static IConfiguration GetConfiguration()
        {
            var EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json", true, true)
                .AddJsonFile($"AppSettings.{EnvironmentName}.json", true, true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
