using System;
using System.Drawing;
using System.Linq;
using Terminal.Gui;
using System.Threading;
using System.Threading.Tasks;
using Colorful;
using PipelineNet.MiddlewareResolver;
using Serilog;
using Texnomic.DNS.Servers;
using Texnomic.DNS.Servers.ResponsibilityChain;
using Texnomic.SecureDNS.CLI.Properties;
using Console = Colorful.Console;

namespace Texnomic.SecureDNS.CLI
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                Console.Title = "Texnomic SecureDNS";

                var Speed = new Figlet(FigletFont.Load(Resources.Speed));

                Console.WriteWithGradient(Speed.ToAscii(" Texnomic").ConcreteValue.ToArray(), System.Drawing.Color.Yellow, System.Drawing.Color.Fuchsia, 14);
                
                Console.WriteWithGradient(Speed.ToAscii(" SecureDNS").ConcreteValue.ToArray(), System.Drawing.Color.Yellow, System.Drawing.Color.Fuchsia, 14);

                Console.WriteLine(" > Loading...");

                Thread.Sleep(3000);

                Console.ReplaceAllColorsWithDefaults();

                App.Run();
            }
            catch (Exception Error)
            {
                Console.WriteLine($"Fatal Error: {Error.Message}");
                Console.ReadLine();
            }
        }
    }
}
