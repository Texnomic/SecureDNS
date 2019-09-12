using BinarySerialization;

namespace Texnomic.DNS.Abstractions
{
    public interface IAnswer : IQuestion
    {
        [FieldOrder(3)]
        ITimeToLive TimeToLive { get; set; }

        [FieldOrder(4)]
        ushort Length { get; set; }

        [FieldOrder(5)]
        IRecord Record { get; set; }

        [Ignore]
        string Data { get; set; }
    }
}
