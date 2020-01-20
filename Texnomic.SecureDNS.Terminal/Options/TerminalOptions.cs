using Texnomic.SecureDNS.Terminal.Enums;

namespace Texnomic.SecureDNS.Terminal.Options
{
    public class TerminalOptions
    {
        public Mode Mode { get; set; } = Mode.GUI;

        public Protocol Protocol { get; set; } = Protocol.HTTPs;
    }
}
