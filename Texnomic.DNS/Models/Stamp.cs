using BinarySerialization;
using System;
using System.Threading.Tasks;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Factories;

namespace Texnomic.DNS.Models
{
    public class Stamp
    {
        [FieldOrder(0), FieldBitLength(8)]
        public StampProtocol Protocol { get; set; }

        [FieldOrder(1)]
        [SubtypeFactory(nameof(Protocol), typeof(StampFactory))]
        public IStamp Value { get; set; }

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
