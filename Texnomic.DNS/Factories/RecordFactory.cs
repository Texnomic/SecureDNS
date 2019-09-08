using System;
using System.Collections.Generic;
using BinarySerialization;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Records;

namespace Texnomic.DNS.Factories
{
    public class RecordFactory : ISubtypeFactory
    {
        private static readonly Dictionary<RecordType, Type> TypesDictionary = new Dictionary<RecordType, Type>
        {
            { RecordType.A, typeof(A) },
            { RecordType.AAAA, typeof(AAAA) },
            { RecordType.CNAME, typeof(CName) },
            { RecordType.HINFO, typeof(HINFO) },
            { RecordType.NAPTR, typeof(NAPTR) },
            { RecordType.NULL, typeof(NULL) },
            { RecordType.ETH, typeof(ETH) },
            { RecordType.TXT, typeof(TXT) },
            { RecordType.SOA, typeof(SOA) },
            { RecordType.WKS, typeof(WKS) },
            { RecordType.PTR, typeof(PTR) },
            { RecordType.MX, typeof(MX) },
            { RecordType.MB, typeof(MB) },
            { RecordType.MD, typeof(MD) },
            { RecordType.MF, typeof(MF) },
            { RecordType.MG, typeof(MG) },
            { RecordType.MR, typeof(MR) },
        };


        private static readonly Dictionary<Type, RecordType> KeysDictionary = new Dictionary<Type, RecordType>
        {
            { typeof(A), RecordType.A },
            { typeof(AAAA), RecordType.AAAA },
            { typeof(CName), RecordType.CNAME },
            { typeof(HINFO), RecordType.HINFO },
            { typeof(NAPTR), RecordType.NAPTR},
            { typeof(NULL), RecordType.NULL },
            { typeof(ETH), RecordType.ETH },
            { typeof(TXT), RecordType.TXT },
            { typeof(SOA), RecordType.SOA },
            { typeof(WKS), RecordType.WKS },
            { typeof(PTR), RecordType.PTR },
            { typeof(MX), RecordType.MX},
            { typeof(MB), RecordType.MB},
            { typeof(MD), RecordType.MD},
            { typeof(MF), RecordType.MF},
            { typeof(MG), RecordType.MG},
            { typeof(MR), RecordType.MR},
        };

        public bool TryGetKey(Type ValueType, out object Key)
        {
            Key = KeysDictionary.GetValueOrDefault(ValueType);

            if (Key == default) throw new NotImplementedException();

            return true;
        }

        public bool TryGetType(object Key, out Type Type)
        {
            Type = TypesDictionary.GetValueOrDefault((RecordType)Key);

            return true;
        }
    }
}
