using System;
using BinarySerialization;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Models
{
    public class Base64String : IBase64String
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
