using System;
using BinarySerialization;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Factories
{
    public class EpochFactory : ISubtypeFactory
    {
        public bool TryGetKey(Type ValueType, out object Key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetType(object Key, out Type Type)
        {
            Type = typeof(Epoch);
            return true;
        }
    }
}
