using BinarySerialization;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Records
{
    public class ETH : IRecord
    {
        [FieldOrder(0)]
        public string Address { get; set; }
    }
}
