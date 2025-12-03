using Serilog;
using ElectronNET.API;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Texnomic.SecureDNS.Configurations;
using Texnomic.SecureDNS.Extensions;

namespace Texnomic.SecureDNS;

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
            .UseSerilog((hostContext, services, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(JsonConfigurationProvider.BuildConfigurations());
            })
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
            .UseConfiguration(JsonConfigurationProvider.BuildConfigurations());
    }

    private static void SetAppConfiguration(HostBuilderContext HostBuilderContext, IConfigurationBuilder ConfigurationBuilder)
    {
        var Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        ConfigurationBuilder.AddDatabaseConfigurations(OptionsBuilder =>
            OptionsBuilder.UseSqlite($"Data Source={Directory}\\SecureDNS.sqlite;"));
    }
}