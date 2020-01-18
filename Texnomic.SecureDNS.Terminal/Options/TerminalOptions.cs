using CommandLine;
using Texnomic.SecureDNS.Terminal.Enums;

namespace Texnomic.SecureDNS.Terminal.Options
{
    public class TerminalOptions
    {
        [Option('m', "mode", Required = false, Default = OperatingMode.TerminalGUI, HelpText = "Proxy Server Operating Mode.")]
        public OperatingMode Mode { get; set; } = OperatingMode.TerminalGUI;
    }
}
