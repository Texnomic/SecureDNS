using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;

namespace Texnomic.DNS.Factories
{
    public class RecordFactory : ISubtypeFactory
    {
        private readonly Dictionary<RecordType, Type> TypesDictionary;

        private readonly Dictionary<Type, RecordType> KeysDictionary;

        public RecordFactory()
        {
            TypesDictionary = Assembly.GetExecutingAssembly()
                                      .GetTypes()
                                      .Where(Type => Type.GetInterfaces().Contains(typeof(IRecord)))
                                      .ToDictionary(Type => (RecordType)Enum.Parse(typeof(RecordType), Type.Name));

            KeysDictionary = TypesDictionary.ToDictionary(Pair => Pair.Value, Pair => Pair.Key);
        }

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
