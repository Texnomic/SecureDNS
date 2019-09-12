using System;
using BinarySerialization;

namespace Texnomic.DNS.Abstractions
{
    public interface IBase64String
    {
        [FieldOrder(0)]
        byte[] Bytes { get; set; }

        [Ignore]
        string Base64 { get; set; }
    }
}
