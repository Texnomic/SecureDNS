using System;

namespace Texnomic.DNS.Abstractions
{
    public interface IAnswer : IQuestion
    {
        uint TTL { get; set; }

        TimeSpan TimeToLive { get; set; }

        ushort Length { get; set; }

        IRecord Record { get; set; }

        string Data { get; set; }
    }
}
