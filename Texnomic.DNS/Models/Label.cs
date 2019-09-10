using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;

namespace Texnomic.DNS.Models
{
    public class Label : ILabel
    {
        public LabelType Type { get; set; }

        public long Offset { get; set; }

        public ushort Count { get; set; }

        public string Text { get; set; }
    }
}
