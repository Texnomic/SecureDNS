using CommandLine;
using Texnomic.SecureDNS.Terminal.Enums;

namespace Texnomic.SecureDNS.Terminal.Options
{
    public class TerminalOptions
    {
        [Option('m', "mode", Required = false, Default = Mode.GUI, HelpText = "Proxy Server Operating Mode.")]
        public Mode Mode { get; set; } = Mode.GUI;
    }
}
