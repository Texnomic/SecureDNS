using System.IO;
using System.Text;
using BinarySerialization;
using Destructurama.Attributed;
using Nerdbank.Streams;
using Newtonsoft.Json;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Extensions;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Records
{
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // /                   TXT-DATA                    /
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

    /// <summary>
    /// Text Resource Record <see href="https://tools.ietf.org/html/rfc1035#section-3.3.14">(TXT)</see>
    /// </summary>
    public class TXT : IBinarySerializable, IRecord
    {
        [Ignore]
        public string Text { get; set; }

        [Ignore]
        public Certificate Certificate { get; set; }

        public static implicit operator string(TXT TXT)
        {
            return TXT.ToString();
        }

        public static implicit operator TXT(string Text)
        {
            return new TXT()
            {
                Text = Text
            };
        }

        public override string ToString()
        {
            return Certificate != null ? JsonConvert.SerializeObject(Certificate) : Text;
        }

        public void Serialize(Stream Stream, Endianness Endianness, BinarySerializationContext SerializationContext)
        {
            if (Text != null)
            {
                Stream.WriteByte((byte)Text.Length);

                var Bytes = Encoding.ASCII.GetBytes(Text);

                Stream.Write(Bytes);
            }

            if (Certificate != null)
            {
                var BinarySerializer = new BinarySerializer();

                var Bytes = BinarySerializer.Serialize(Certificate);

                Stream.WriteByte((byte)Bytes.Length);

                Stream.Write(Bytes);
            }
        }

        public void Deserialize(Stream Stream, Endianness Endianness, BinarySerializationContext SerializationContext)
        {
            var Length = Stream.ReadByte();

            var Bytes = new byte[Length];

            Stream.Read(Bytes);

            var Magic = Encoding.ASCII.GetString(Bytes[..4]);

            if (Magic == "DNSC")
            {
                var BinarySerializer = new BinarySerializer();

                Certificate = BinarySerializer.Deserialize<Certificate>(Bytes);
            }
            else
            {
                Text = Encoding.ASCII.GetString(Bytes);
            }
        }
    }
}
