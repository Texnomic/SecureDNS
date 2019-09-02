using BinarySerialization;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Records
{
    /// <summary>
    /// Name Server Record
    /// </summary>
    public class NS : IRecord
    {
        [FieldOrder(0)]
        public Domain Domain { get; set; }
    }
}
