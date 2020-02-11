using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;

namespace Texnomic.DNS.Factories
{
    public class StampFactory : ISubtypeFactory
    {
        private readonly Dictionary<StampProtocol, Type> TypesDictionary;

        private readonly Dictionary<Type, StampProtocol> KeysDictionary;

        public StampFactory()
        {
            TypesDictionary = Assembly.GetExecutingAssembly()
                                      .GetTypes()
                                      .Where(Type => Type.GetInterfaces().Contains(typeof(IStamp)))
                                      .ToDictionary(Type => Enum.Parse<StampProtocol>(Type.Name.Replace("Stamp", "")));

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
            Type = TypesDictionary.GetValueOrDefault((StampProtocol)Key);

            return true;
        }
    }
}
