using System;
using Destructurama.Attributed;
using System.Collections.Generic;
using System.Linq;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;

namespace Texnomic.SecureDNS.Core.DataTypes
{
    [LogAsScalar(true)]
    public class Domain : IDomain
    {
        public LabelType LabelType { get; set; }
        public ushort? Pointer { get; set; }
        public IEnumerable<ILabel> Labels { get; set; }

        public Domain()
        {
            LabelType = LabelType.Normal;
            Labels = new List<ILabel>();
        }

        public static implicit operator string(Domain Domain)
        {
            return Domain?.ToString();
        }

        public static implicit operator Domain(string FQDN)
        {
            return FromString(FQDN);
        }

        public override string ToString()
        {
            return string.Join('.', Labels);
        }

        public static Domain FromString(string FQDN)
        {
            var Array = FQDN.Split('.', StringSplitOptions.RemoveEmptyEntries);

            var Domain = new Domain();

            var Labels = new List<ILabel>();

            foreach (var Label in Array)
            {
                Labels.Add((Label)Label);
            }

            Domain.Labels = Labels;

            return Domain;
        }
    }
}
