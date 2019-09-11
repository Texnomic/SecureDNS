using System;
using BinarySerialization;

namespace Texnomic.DNS.Models
{
    public class Base64String
    {
        [FieldOrder(0)]
        public byte[] Bytes { get; set; }

        [Ignore]
        public string Base64
        {
            get => Convert.ToBase64String(Bytes);
            set => Bytes = Convert.FromBase64String(value);
        }
    }
}
