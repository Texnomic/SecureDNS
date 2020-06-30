using System;
using Destructurama.Attributed;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Core.DataTypes
{
    #nullable enable
    public class Answer : Question, IAnswer
    {
        [LogAsScalar(true)]
        public TimeSpan? TimeToLive { get; set; }

        [NotLogged]
        public ushort Length { get; set; }

        public IRecord? Record { get; set; }
    }

}
