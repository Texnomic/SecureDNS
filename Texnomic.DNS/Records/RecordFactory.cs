using BinarySerialization;
using System;
using System.Collections.Generic;
using Texnomic.DNS.Enums;

namespace Texnomic.DNS.Records
{
    public class RecordFactory : ISubtypeFactory
    {
        static readonly SortedDictionary<RecordType, Type> TypesDictionary = new SortedDictionary<RecordType, Type>
        {
            { RecordType.A, typeof(A) },
            { RecordType.CNAME, typeof(CName) }
        };


        static readonly SortedDictionary<Type, RecordType> KeysDictionary = new SortedDictionary<Type, RecordType>
        {
            { typeof(A), RecordType.A },
            { typeof(CName), RecordType.CNAME }
        };

        public bool TryGetKey(Type ValueType, out object Key)
        {
            Key = KeysDictionary.GetValueOrDefault(ValueType);

            if (Key == default)
            {
                throw new NotImplementedException();
            }

            return true;
        }

        public bool TryGetType(object Key, out Type Type)
        {
            Type = TypesDictionary.GetValueOrDefault((RecordType)Key);

            if (Type == default)
            {
                throw new NotImplementedException();
            }

            return true;
        }
    }
}
