using Serilog;
using System.IO;
using ElectronNET.API;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Texnomic.SecureDNS.Configurations;
using Texnomic.SecureDNS.Extensions;

namespace Texnomic.SecureDNS
{
    public class Program
    {
        private static string[] Arguments;

        public static void Main(string[] Args)
        {
            Arguments = Args;

            CreateHostBuilder().Build().Run();
        }

        public static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder(Arguments)
                       .ConfigureLogging(SetLogging)
                       .ConfigureAppConfiguration(SetAppConfiguration)
                       .ConfigureWebHostDefaults(SetWebHostDefaults);

        }

        private static void SetLogging(HostBuilderContext HostBuilderContext, ILoggingBuilder LoggingBuilder)
        {
            //Workaround to clear all providers before Serilog takes over.
            LoggingBuilder.ClearProviders();
        }

        private static void SetWebHostDefaults(IWebHostBuilder WebHostBuilder)
        {
            WebHostBuilder.UseElectron(Arguments)
                          .UseStartup<Startup>()
                          .UseConfiguration(JsonConfigurationProvider.BuildConfigurations())
                          .UseSerilog(ConfigureLogger, writeToProviders: true);
        }

        private static void ConfigureLogger(WebHostBuilderContext WebHostBuilderContext, LoggerConfiguration LoggerConfiguration)
        {
            LoggerConfiguration.ReadFrom.Configuration(JsonConfigurationProvider.BuildConfigurations());
        }

        private static void SetAppConfiguration(HostBuilderContext HostBuilderContext, IConfigurationBuilder ConfigurationBuilder)
        {
            var Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            ConfigurationBuilder.AddDatabaseConfigurations(OptionsBuilder =>
                OptionsBuilder.UseSqlite($"Data Source={Directory}\\SecureDNS.sqlite;"));
        }
    }
}
