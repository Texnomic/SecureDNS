using System;
using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Records
{
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // /                   MADNAME                     /
    // /                                               /
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

    /// <summary>
    /// Mail Agent Delivery Resource Record <see href="https://tools.ietf.org/html/rfc1035#section-3.3.4">(MD)</see>
    /// </summary>
    [Obsolete]
    public class MD : IRecord
    {
        [FieldOrder(0)]
        public Domain Domain { get; set; }
    }
}
