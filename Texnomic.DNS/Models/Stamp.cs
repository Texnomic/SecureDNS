using BinarySerialization;
using System;
using System.Threading.Tasks;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Models
{
    public class Stamp
    {
        [FieldOrder(0), FieldBitLength(8)]
        public StampProtocol Protocol { get; set; }

        [FieldOrder(1)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DnsCrypt)]
        public DNSCryptStamp DNSCrypt { get; set; }

        [FieldOrder(2)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DoH)]
        public DoHStamp DoH { get; set; }

        [FieldOrder(3)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DoT)]
        public DoTStamp DoT { get; set; }

        [FieldOrder(4)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DoU)]
        public PlainStamp DoU { get; set; }

        [FieldOrder(5)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DNSCryptRelay)]
        public DNSCryptRelayStamp DNSCryptRelay { get; set; }

        public static Stamp FromString(string Stamp)
        {
            var Bytes = Decode(Stamp[7..]);

            var BinarySerializer = new BinarySerializer();

            return BinarySerializer.Deserialize<Stamp>(Bytes);
        }

        public static async Task<Stamp> FromStringAsync(string Stamp)
        {
            var Bytes = Decode(Stamp[7..]);

            var BinarySerializer = new BinarySerializer();

            return await BinarySerializer.DeserializeAsync<Stamp>(Bytes);
        }

        public override string ToString()
        {
            var BinarySerializer = new BinarySerializer();

            var Bytes = BinarySerializer.Serialize(this);

            var Stamp = Encode(Bytes);

            return string.Concat("sdns://", Stamp);
        }

        public async Task<string> ToStringAsync()
        {
            var BinarySerializer = new BinarySerializer();

            var Bytes = await BinarySerializer.SerializeAsync(this);

            var Stamp = Encode(Bytes);

            return string.Concat("sdns://", Stamp);
        }

        private static string Encode(byte[] Stamp)
        {
            return Convert.ToBase64String(Stamp)
                .Replace("=", "")
                .Replace("/", "_")
                .Replace("+", "-");
        }

        private static string ToBase64(string Stamp)
        {
            return Stamp
                .PadRight(Stamp.Length + (4 - Stamp.Length % 4) % 4, '=')
                .Replace("_", "/")
                .Replace("-", "+");
        }

        private static byte[] Decode(string Stamp)
        {
            return Convert.FromBase64String(ToBase64(Stamp));
        }
    }


}
