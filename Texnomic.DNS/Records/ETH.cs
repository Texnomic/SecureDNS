using BinarySerialization;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Records
{
    public class ETH : IRecord
    {
        [FieldOrder(0)]
        public string Resolver { get; set; }

        [FieldOrder(1)] 
        public string Registrant { get; set; }

        [FieldOrder(2)] 
        public string Contract { get; set; }
    }
}
