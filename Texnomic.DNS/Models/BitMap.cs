using BinarySerialization;

namespace Texnomic.DNS.Models
{
    public class BitMap
    {
        [FieldOrder(0)] 
        public byte[] Data { get; set; }
    }
}
