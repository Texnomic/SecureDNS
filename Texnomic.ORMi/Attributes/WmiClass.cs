using System;

namespace Texnomic.ORMi.Attributes
{
    public class WmiClass : Attribute
    {
        public WmiClass(string Name, string Namespace)
        {
            this.Name = Name;
            this.Namespace = Namespace;
        }

        public string Name { get; private set; }
        public string Namespace { get; private set; }
    }
}
