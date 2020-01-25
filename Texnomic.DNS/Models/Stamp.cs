using BinarySerialization;
using System;
using System.Text;
using Texnomic.DNS.Abstractions.Enums;

namespace Texnomic.DNS.Models
{
    public class Stamp
    {
        public Stamp()
        {

        }

        public Stamp(string Stamp)
        {
            if (!Stamp.StartsWith("sdns://")) throw new ArgumentException("Stamp Uri Must Start With SDNS://");

            var StampBytes = Decode(Stamp[7..]);

            Protocol = (StampProtocol)StampBytes[0];

            switch (Protocol)
            {
                case StampProtocol.DnsCrypt:
                    ParseDnsCrypt(ref StampBytes);
                    break;

                case StampProtocol.DoH:
                    ParseDoH(ref StampBytes);
                    break;

                case StampProtocol.DNSCryptRelay:
                    ParseDnsCryptReply(ref StampBytes);
                    break;

                case StampProtocol.Plain:
                case StampProtocol.TLS:
                case StampProtocol.Unknown:
                default:
                    break;
            }
        }

        private void ParseDnsCrypt(ref byte[] StampBytes)
        {
            //Too Short
            if (StampBytes.Length < 66) return;

            DnsSec = Convert.ToBoolean((StampBytes[1] >> 0) & 1);

            NoLog = Convert.ToBoolean((StampBytes[1] >> 1) & 1);

            NoFilter = Convert.ToBoolean((StampBytes[1] >> 2) & 1);

            var Pointer = 9;

            var HostnameLength = StampBytes[Pointer++];

            Hostname = Encoding.UTF8.GetString(StampBytes[Pointer..(Pointer + HostnameLength)]);

            Pointer += HostnameLength;

            var PublicLeyLength = StampBytes[Pointer++];

            PublicKey = StampBytes[Pointer..(Pointer + PublicLeyLength)];

            Pointer += PublicLeyLength;

            var ProviderNameLength = StampBytes[Pointer++];

            ProviderName = Encoding.UTF8.GetString(StampBytes[Pointer..(Pointer + ProviderNameLength)]);
        }

        private void ParseDoH(ref byte[] StampBytes)
        {
            //Too Short
            if (StampBytes.Length < 22) return;

            DnsSec = Convert.ToBoolean((StampBytes[1] >> 0) & 1);

            NoLog = Convert.ToBoolean((StampBytes[1] >> 1) & 1);

            NoFilter = Convert.ToBoolean((StampBytes[1] >> 2) & 1);

            var Pointer = 9;

            var AddressLength = StampBytes[Pointer++];

            Address = Encoding.UTF8.GetString(StampBytes[Pointer..(Pointer + AddressLength)]);

            Pointer += AddressLength;

            var HashLength = StampBytes[Pointer++];

            Hash = StampBytes[Pointer..(Pointer + HashLength)];

            Pointer += HashLength;

            var HostnameLength = StampBytes[Pointer++];

            Hostname = Encoding.UTF8.GetString(StampBytes[Pointer..(Pointer + HostnameLength)]);

            Pointer += HostnameLength;

            var PathLength = StampBytes[Pointer++];

            Path = Encoding.UTF8.GetString(StampBytes[Pointer..(Pointer + PathLength)]);
        }

        private void ParseDnsCryptReply(ref byte[] StampBytes)
        {
            //Too Short
            if (StampBytes.Length < 13) return;

            var Pointer = 1;

            var AddressLength = StampBytes[Pointer++];

            Address = Encoding.UTF8.GetString(StampBytes[Pointer..(Pointer + AddressLength)]);
        }


        [FieldOrder(0), FieldBitLength(8)]
        public StampProtocol Protocol { get; set; }

        [FieldOrder(1)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DnsCrypt)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DoH)]
        [FieldBitLength(1)]
        public bool DnsSec { get; set; }

        [FieldOrder(2)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DnsCrypt)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DoH)]
        [FieldBitLength(1)]
        public bool NoLog { get; set; }

        [FieldOrder(3)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DnsCrypt)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DoH)]
        [FieldBitLength(1)]
        public bool NoFilter { get; set; }

        [FieldOrder(4)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DnsCrypt)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DoH)]
        [FieldBitLength(5)]
        public byte Flags { get; set; }

        [FieldOrder(5)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DnsCrypt)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DoH)]
        [FieldBitLength(8)]
        public int HostnameLength { get; set; }

        [FieldOrder(6)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DnsCrypt)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DoH)]
        [FieldLength(nameof(HostnameLength))]
        public string Hostname { get; set; }

        [FieldOrder(7)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DnsCrypt)]
        [FieldBitLength(8)]
        public int PublicKeyLength { get; set; }

        [FieldOrder(8)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DnsCrypt)]
        [FieldLength(nameof(PublicKeyLength))]
        public byte[] PublicKey { get; set; }

        [FieldOrder(9)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DnsCrypt)]
        [FieldBitLength(8)]
        public int ProviderNameLength { get; set; }

        [FieldOrder(10)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DnsCrypt)]
        [FieldLength(nameof(ProviderNameLength))]
        public string ProviderName { get; set; }

        [FieldOrder(11)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DNSCryptRelay)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DoH)]
        [FieldBitLength(8)]
        public int AddressLength { get; set; }

        [FieldOrder(12)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DNSCryptRelay)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DoH)]
        [FieldLength(nameof(AddressLength))]
        public string Address { get; set; }


        [FieldOrder(11)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DoH)]
        [FieldBitLength(8)]
        public int HashLength { get; set; }

        [FieldOrder(12)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DoH)]
        [FieldLength(nameof(HashLength))]
        public byte[] Hash { get; set; }

        [FieldOrder(11)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DoH)]
        [FieldBitLength(8)]
        public int PathLength { get; set; }

        [FieldOrder(12)]
        [SerializeWhen(nameof(Protocol), StampProtocol.DoH)]
        [FieldLength(nameof(PathLength))]
        public string Path { get; set; }
    }
}
