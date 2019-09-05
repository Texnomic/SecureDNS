using BinarySerialization;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Records
{
    /// <summary>
    /// Name Server Record
    /// </summary>
    public class NS : IRecord
    {
        [FieldOrder(0)]
        public IDomain Domain { get; set; }
    }
}
