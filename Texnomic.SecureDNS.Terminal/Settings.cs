using System.Threading;
using CommandLine;

namespace Texnomic.SecureDNS.Terminal
{
    public class Settings
    {
        [Option('b', "binding", Required = true, Default = "0.0.0.0:53", HelpText = "DNS Server IP EndPoint Binding.")]
        public string ServerIPEndPoint { get; set; } = "0.0.0.0:53";

        [Option('s', "seq", Required = true, Default = "http://127.0.0.1:5341", HelpText = "Seq Server API HTTP EndPoint.")]
        public string SeqUriEndPoint { get; set; } = "http://127.0.0.1:5341";

        public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
    }
}
