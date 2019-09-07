using System;
using BinarySerialization;

namespace Texnomic.DNS.Factories
{
    public class DomainFactory : ISubtypeFactory
    {
        public bool TryGetKey(Type ValueType, out object Key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetType(object Key, out Type Type)
        {
            throw new NotImplementedException();
        }
    }
}
