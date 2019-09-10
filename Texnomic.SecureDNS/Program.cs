using ElectronNET.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Texnomic.SecureDNS
{
    public class Program
    {
        public static void Main(string[] Args)
        {
            CreateHostBuilder(Args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] Args)
        {
            return Host.CreateDefaultBuilder(Args)
                       .ConfigureWebHostDefaults(WebBuilder => WebBuilder
                       .UseElectron(Args)
                       .UseStartup<Startup>());
        }
    }
}
