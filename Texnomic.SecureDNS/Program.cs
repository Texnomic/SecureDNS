using System.IO;
using System.Reflection;
using ElectronNET.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Texnomic.SecureDNS.Extensions;

namespace Texnomic.SecureDNS
{
    public class Program
    {
        public static string[] Arguments;

        public static void Main(string[] Args)
        {
            Arguments = Args;

            CreateHostBuilder().Build().Run();
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
                          .UseStartup<Startup>();
        }

        private static void SetAppConfiguration(HostBuilderContext HostBuilderContext, IConfigurationBuilder ConfigurationBuilder)
        {
            var Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            ConfigurationBuilder.AddDatabaseConfigurations(OptionsBuilder =>
                OptionsBuilder.UseSqlite($"Data Source={Directory}\\SecureDNS.sqlite;"));
        }
    }
}
