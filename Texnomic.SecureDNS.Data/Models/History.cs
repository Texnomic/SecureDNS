using System;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Data.Models
{
    public class History
    {
        public ushort ID { get; set; }
        public IMessage Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
