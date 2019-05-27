using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Texnomic.SecureDNS.DNS.Enums;

namespace Texnomic.SecureDNS.DNS
{
    public class Header
    {
        private readonly BitArray Bits;

        public Header(byte[] Bytes)
        {
            Bits = new BitArray(Bytes);
        }

        public Header(BitArray Bits)
        {
            this.Bits = Bits;
        }

        public ushort ID => Bits.GetUInt16(0);

        public MessageType MessageType => (MessageType)Bits.GetInt(16);

        public OperationCode OperationCode => (OperationCode)(Bits.GetInt(17) + Bits.GetInt(18) + Bits.GetInt(19) + Bits.GetInt(20));

        public AuthoritativeAnswer AuthoritativeAnswer => (AuthoritativeAnswer)Bits.GetInt(21);

        public bool Truncated => Bits.Get(22);

        public RecursionDesired RecursionDesired => (RecursionDesired)Bits.GetInt(23);

        public bool RecursionAvailable => Bits.Get(24);

        public int Zero => Bits.GetInt(25);

        /// <summary>
        /// Used by DNSSEC Only.
        /// </summary>
        public bool AuthenticatedData => Bits.Get(26);

        /// <summary>
        /// Used by DNSSEC Only.
        /// </summary>
        public bool CheckingDisabled => Bits.Get(27);

        public ResponseCode ResponseCode => (ResponseCode)(Bits.GetInt(26) + Bits.GetInt(27) + Bits.GetInt(28) + Bits.GetInt(29));

        public ushort QuestionsCount => Bits.GetUInt16(30);

        public ushort AnswersCount => Bits.GetUInt16(46);

        public ushort AuthorityCount => Bits.GetUInt16(62);

        public ushort AdditionalCount => Bits.GetUInt16(78);
    }
}
