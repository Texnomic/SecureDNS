using System;

namespace Texnomic.SecureDNS.Abstractions
{
    public interface IAnswer : IQuestion
    {
        TimeSpan TimeToLive { get; set; }

        ushort Length { get; set; }

        IRecord Record { get; set; }
    }
}
