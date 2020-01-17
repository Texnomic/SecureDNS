using System;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.FilterLists.Enums;
using Texnomic.FilterLists.Models;

namespace Texnomic.DNS.Servers.Options
{
    public class FilterMiddlewareOptions
    {
        public Func<FilterList, bool> Predicate { get; set; } = (List) =>
        {
            return List.Syntax == Syntax.Hosts &&
                   List.Tags.Contains(Tags.Malware) &&
                   (List.Maintainers == null ? false : List.Maintainers.Contains(Maintainers.HostsFileDotNet));
        };

        public IMessage Template { get; set; } = new Message()
        {
            MessageType = MessageType.Response,
            ResponseCode = ResponseCode.Refused,
        };
    }
}
