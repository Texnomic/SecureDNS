using BinarySerialization;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Records
{
    public class CName : IRecord
    {
        [FieldOrder(0)]
        [SerializeUntil((byte)0)]
        public Label[] Labels { get; set; }
    }
}
