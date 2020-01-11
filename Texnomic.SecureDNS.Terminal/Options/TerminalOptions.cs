using CommandLine;
using Texnomic.SecureDNS.Terminal.Enums;

namespace Texnomic.SecureDNS.Terminal.Options
{
    public class TerminalOptions
    {
        [Option('b', "binding", Required = true, Default = "0.0.0.0:53", HelpText = "Proxy Server IP EndPoint Binding.")]
        public string ServerIPEndPoint { get; set; } = "0.0.0.0:53";

        [Option('s', "seq", Required = true, Default = "http://127.0.0.1:5341", HelpText = "Seq Server API HTTP EndPoint.")]
        public string SeqUriEndPoint { get; set; } = "http://127.0.0.1:5341";

        [Option('m', "mode", Required = false, Default = OperatingMode.TerminalGUI, HelpText = "Proxy Server Operating Mode.")]
        public OperatingMode Mode { get; set; } = OperatingMode.TerminalGUI;
    }
}
