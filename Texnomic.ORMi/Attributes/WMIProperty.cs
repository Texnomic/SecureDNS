using System;

namespace Texnomic.ORMi.Attributes
{
    public class WmiProperty : Attribute
    {
        public WmiProperty(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
