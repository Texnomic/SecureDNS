namespace Texnomic.SecureDNS.Abstractions;
#nullable enable
public interface IAnswer : IQuestion
{
    TimeSpan? TimeToLive { get; set; }

    ushort Length { get; set; }

    IRecord? Record { get; set; }
}