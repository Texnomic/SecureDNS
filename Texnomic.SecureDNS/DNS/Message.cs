using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Texnomic.SecureDNS.DNS
{
    public class Message
    {
        private readonly BitArray Bits;

        public Message(byte[] Bytes)
        {
            Bits = new BitArray(Bytes);
        }
    }
}
