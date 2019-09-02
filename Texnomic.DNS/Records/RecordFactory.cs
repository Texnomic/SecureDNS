using BinarySerialization;
using System;
using System.Collections.Generic;
using Texnomic.DNS.Enums;

namespace Texnomic.DNS.Records
{
    public class RecordFactory : ISubtypeFactory
    {
        private static readonly Dictionary<RecordType, Type> TypesDictionary = new Dictionary<RecordType, Type>
        {
            { RecordType.A, typeof(A) },
            { RecordType.AAAA, typeof(AAAA) },
            { RecordType.CNAME, typeof(CName) }
        };


        private static readonly Dictionary<Type, RecordType> KeysDictionary = new Dictionary<Type, RecordType>
        {
            { typeof(A), RecordType.A },
            { typeof(AAAA), RecordType.AAAA },
            { typeof(CName), RecordType.CNAME }
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

            //if (Type == default) throw new NotImplementedException();

            return true;
        }
    }
}
