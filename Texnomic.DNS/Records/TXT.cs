using BinarySerialization;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Records
{
    public class TXT : IRecord
    {
        [FieldOrder(0)]
        public string Text { get; set; }
    }
}
