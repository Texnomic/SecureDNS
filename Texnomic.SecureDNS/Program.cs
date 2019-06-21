using ElectronNET.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Texnomic.SecureDNS
{
    public class Program
    {
        public static void Main(string[] Args)
        {
            CreateHostBuilder(Args).Build()
                                   .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureWebHostDefaults(WebBuilder => WebBuilder
                       .UseElectron(args)
                       .UseStartup<Startup>());
        }                                           
    }
}
