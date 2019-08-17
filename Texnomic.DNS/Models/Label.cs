using Texnomic.DNS.Enums;

namespace Texnomic.DNS.Models
{
    public class Label
    {
        public LabelType Type { get; set; }

        public ushort Count { get; set; }

        public string Text { get; set; }
    }
}
